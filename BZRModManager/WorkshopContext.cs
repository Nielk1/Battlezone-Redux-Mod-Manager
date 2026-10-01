using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    // Set UrlPrefix to the directory containing the proxy's .htaccess to use its cache.
    // For direct Steam requests, leave the default prefix and set ApiKey.
    class WorkshopContext
    {
        //public static string UrlPrefix { get; set; } = "https://api.steampowered.com/";
        public static string UrlPrefix { get; set; } = "https://api.battlezone.report/";
        public static string ApiKey { get; set; } = "";

        private static readonly HttpClient Client = new HttpClient();

        // Bounded parallelism for the per-item GetDetails enrichment pass: high enough to
        // keep the Find Mods lists filling smoothly, low enough not to hammer Steam.
        private const int EnrichParallelism = 8;

        // ------------------------------------------------------------------
        // Local cache
        //
        // Every item's GetDetails JSON (cache\items\<id>.json) and every preview image
        // (cache\previews\<id>.png) is stored on disk. A file younger than CacheDuration
        // is served as-is without any web request; otherwise it is refreshed, and when
        // the refresh fails a stale copy is used if one exists.
        // ------------------------------------------------------------------
        public static TimeSpan CacheDuration { get; set; } = TimeSpan.FromDays(1);
        public static string CacheDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "cache");

        // A definitive "this ID does not exist" answer (an item_not_found /
        // appid_not_allowed error body, a 403/404 status, an empty
        // publishedfiledetails array, or result != 1) is cached as a self-describing
        // sentinel, so a mistyped or deleted ID is not re-requested on every lookup
        // (the server debounces, but the cache is what keeps local lookups cheap).
        // appid_not_allowed can never resolve for us (we query without an AppID), so
        // it is cached for a long time; every other negative expires after
        // NegativeCacheDuration, after which the next lookup re-queries normally
        // (an ID can start existing later: newly published or re-listed item).
        // A bad sentinel can always be removed by deleting the cache file.
        public static TimeSpan NegativeCacheDuration { get; set; } = TimeSpan.FromHours(1);
        public static TimeSpan PermanentNegativeCacheDuration { get; set; } = TimeSpan.FromDays(3650);
        internal const string NegativeCacheSentinel = "{\"not_found\":true}";
        internal const string PermanentNegativeCacheSentinel = "{\"not_found\":true,\"permanent\":true}";

        internal static string ItemJsonCachePath(string id) => Path.Combine(CacheDirectory, "items", id + ".json");
        internal static string PreviewImageCachePath(string id) => Path.Combine(CacheDirectory, "previews", id + ".png");

        // True when the cache file exists, is no older than CacheDuration, and read back
        // intact. Corrupt entries are treated as a miss.
        internal static bool TryReadFreshCache(string path, out byte[] data)
        {
            data = null;
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists) return false;
                if (info.LastWriteTimeUtc + CacheDuration <= DateTime.UtcNow) return false;
                data = File.ReadAllBytes(path);
                return data.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // True when the cache file exists and read back intact, regardless of age.
        internal static bool TryReadAnyCache(string path, out byte[] data)
        {
            data = null;
            try
            {
                if (!File.Exists(path)) return false;
                data = File.ReadAllBytes(path);
                return data.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // True when the cache file holds the generic ("not found") sentinel and is
        // no older than NegativeCacheDuration.
        internal static bool TryReadFreshNegativeCache(string path)
        {
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists) return false;
                if (info.LastWriteTimeUtc + NegativeCacheDuration <= DateTime.UtcNow) return false;
                return File.ReadAllText(path) == NegativeCacheSentinel;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // True when the cache file holds the permanent negative sentinel (an ID that
        // can never resolve for this app, e.g. an appid_not_allowed item) and is
        // still inside its long window.
        internal static bool HasPermanentNegativeCache(string path)
        {
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists) return false;
                if (info.LastWriteTimeUtc + PermanentNegativeCacheDuration <= DateTime.UtcNow) return false;
                return File.ReadAllText(path) == PermanentNegativeCacheSentinel;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static bool IsNegativeSentinel(byte[] data)
        {
            if (data == null || data.Length != NegativeCacheSentinelBytes.Length) return false;
            for (int i = 0; i < data.Length; i++)
                if (data[i] != NegativeCacheSentinelBytes[i]) return false;
            return true;
        }

        // Atomic write so a crash never leaves a half-written cache entry behind.
        // Sentinel entries never overwrite existing real data, so an unexpected
        // response (or an empty body) can only ever *add* a negative, never clobber
        // a good cache entry.
        internal static void WriteCache(string path, byte[] data)
        {
            try
            {
                if (IsNegativeSentinel(data))
                {
                    if (File.Exists(path)) return;
                }
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                var tmp = path + ".tmp" + Guid.NewGuid().ToString("N");
                File.WriteAllBytes(tmp, data);
                if (File.Exists(path)) File.Replace(tmp, path, null);
                else File.Move(tmp, path);
            }
            catch (Exception)
            {
                // Cache write failures are non-fatal; the data was still served to the caller.
            }
        }

        static readonly byte[] NegativeCacheSentinelBytes = Encoding.UTF8.GetBytes(NegativeCacheSentinel);
        static readonly byte[] PermanentNegativeCacheSentinelBytes = Encoding.UTF8.GetBytes(PermanentNegativeCacheSentinel);

        internal static HttpClient ImageClient => Client;

        // Async, streaming replacement for the old GetMods: yields one WorkshopMod at a
        // time as the QueryFiles pages arrive, so the Find Mods lists can fill in over
        // time instead of waiting for the whole result set. Tags are ORed, as in the
        // old browse calls; pass null or an empty array to get all files. Every item is
        // enriched through GetItemAsync, which is served from the JSON cache while the
        // entry is still fresh.
        public static async IAsyncEnumerable<WorkshopMod> GetModsAsync(int appid, string[] tags,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (appid <= 0) throw new ArgumentOutOfRangeException(nameof(appid));

            var requiredTags = tags?.Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim()).Distinct(StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
            var seenIds = new HashSet<string>(StringComparer.Ordinal);
            var cursor = "*";
            var seenCursors = new HashSet<string>(StringComparer.Ordinal) { cursor };

            while (true)
            {
                var request = new Dictionary<string, object>
                {
                    ["query_type"] = 21, // Most recently updated.
                    ["page"] = 1, ["cursor"] = cursor, ["numperpage"] = 50,
                    ["appid"] = appid, ["filetype"] = 0
                };
                if (requiredTags.Length > 0)
                {
                    request["requiredtags"] = requiredTags;
                    request["match_all_tags"] = false;
                }

                var pageJson = await FetchRawAsync("IPublishedFileService/QueryFiles/v1/", request, cancellationToken);
                var pageIds = new List<string>();
                bool hasDetails;
                string next;
                using (var document = JsonDocument.Parse(pageJson))
                {
                    var response = Response(document);
                    hasDetails = response.TryGetProperty("publishedfiledetails", out var items) &&
                        items.ValueKind == JsonValueKind.Array;
                    if (hasDetails)
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            if (item.ValueKind != JsonValueKind.Object ||
                                (Text(item, "result") is string result && result != "1")) continue;
                            var id = Text(item, "publishedfileid");
                            if (!string.IsNullOrEmpty(id)) pageIds.Add(id);
                        }
                    }
                    next = Text(response, "next_cursor");
                }

                // Steam can omit the array on the terminal, empty page (and may repeat the cursor).
                if (!hasDetails || pageIds.Count == 0)
                {
                    if (!hasDetails && next != cursor)
                        throw new InvalidDataException("QueryFiles omitted publishedfiledetails.");
                    break;
                }

                // Enrich every item on this page with GetItem (GetDetails). Fresh items are
                // served from the JSON cache without any web request; a failure on a single
                // item drops just that item instead of aborting the whole search.
                using var gate = new SemaphoreSlim(EnrichParallelism);
                var tasks = pageIds.Select(async id =>
                {
                    await gate.WaitAsync(cancellationToken);
                    try
                    {
                        return await GetItemAsync(id, appid, cancellationToken);
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetDetails failed for " + id + ": " + ex.Message);
                        return null;
                    }
                    finally
                    {
                        gate.Release();
                    }
                }).ToArray();
                await Task.WhenAll(tasks);

                // Yield in page order so the list fills top-to-bottom.
                for (int i = 0; i < tasks.Length; i++)
                {
                    var mod = tasks[i].Result;
                    if (mod == null) continue;
                    if (!seenIds.Add(mod.UniqueID)) continue; // Steam can repeat IDs across pages.
                    // The preview image follows the same cache rules as the item JSON.
                    await mod.EnsurePreviewImageAsync(cancellationToken);
                    yield return mod;
                }

                if (string.IsNullOrEmpty(next)) break;
                if (!seenCursors.Add(next))
                    throw new InvalidDataException("QueryFiles repeated a cursor on a nonempty page.");
                cursor = next; // Pass the opaque cursor through without decoding it.
            }
        }

        // Async replacement for the old GetItem. Details include file_type (2 for a
        // collection) and the immediate children. Returns null when Steam reports that
        // the ID does not exist. Served from the JSON cache while the entry is fresh;
        // falls back to a stale entry when the web request fails.
        public static async Task<WorkshopMod> GetItemAsync(string id, int appid = 0, CancellationToken cancellationToken = default)
        {
            if (!ulong.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) || parsed == 0)
                throw new ArgumentException("A positive decimal published file ID is required.", nameof(id));
            if (appid < 0) throw new ArgumentOutOfRangeException(nameof(appid));

            var request = new Dictionary<string, object>
            {
                ["publishedfileids"] = new[] { id }, ["includechildren"] = true
            };
            if (appid > 0) request["appid"] = appid;

            var cachePath = ItemJsonCachePath(id);
            byte[] json;
            if (TryReadFreshNegativeCache(cachePath) || HasPermanentNegativeCache(cachePath))
            {
                // This ID is known to be unresolvable; trust that for the negative window.
                return null;
            }
            if (TryReadFreshCache(cachePath, out json) && !IsNegativeSentinel(json))
            {
                // The cached entry is new enough: no web request.
            }
            else
            {
                // A negative sentinel is only honored inside its own window (checked
                // above); a stale one is a miss, so the ID is re-queried for real.
                try
                {
                    json = await FetchRawAsync("IPublishedFileService/GetDetails/v1/", request, cancellationToken);
                    WriteCache(cachePath, json);
                }
                catch (OperationCanceledException) { throw; }
                catch (WorkshopApiException ex) when (ex.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
                {
                    // A definitive "not found / access restricted" answer; remember it so
                    // lookups of this ID don't hammer the API.
                    if (ex.ErrorBody != null)
                    {
                        if (HasErrorCode(ex.ErrorBody, "appid_not_allowed") && appid == 0)
                        {
                            // We query without an AppID, so this can never resolve for us.
                            WriteCache(cachePath, PermanentNegativeCacheSentinelBytes);
                            return null;
                        }
                        if (HasErrorCode(ex.ErrorBody, "item_not_found"))
                        {
                            WriteCache(cachePath, NegativeCacheSentinelBytes);
                            return null;
                        }
                    }
                    WriteCache(cachePath, NegativeCacheSentinelBytes);
                    return null;
                }
                catch (Exception)
                {
                    // The network failed; a stale entry is better than nothing.
                    if (!TryReadAnyCache(cachePath, out json)) throw;
                    // A stale "not found" is still the best answer we have.
                    if (IsNegativeSentinel(json)) return null;
                }
            }

            var document = JsonDocument.Parse(json);
            if (!TryGetDetailsResponse(document, out var details))
            {
                // Definitive not found (result != 1 or no publishedfiledetails);
                // remember that so we don't keep asking.
                WriteCache(cachePath, NegativeCacheSentinelBytes);
                return null;
            }
            foreach (var item in details.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object && Text(item, "publishedfileid") == id)
                    if (Text(item, "result") == "1") return WorkshopMod.FromSteam(item);
                    WriteCache(cachePath, NegativeCacheSentinelBytes);
                    return null;
            }
            throw new InvalidDataException("GetDetails returned a different published file ID.");
        }

        // The .NET 5+ HttpRequestException no longer carries the HTTP response, so a
        // definitive-error body (e.g. {"error":"appid_not_allowed",...}) is kept on
        // this derived type instead.
        private sealed class WorkshopApiException : HttpRequestException
        {
            public string ErrorBody { get; }
            public WorkshopApiException(HttpStatusCode statusCode, string errorBody)
                : base(HttpRequestError.HttpProtocolError,
                    "The Workshop API returned " + (int)statusCode + ".",
                    null, statusCode)
            {
                ErrorBody = errorBody;
            }
        }

        private static async Task<byte[]> FetchRawAsync(string path, object request, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(UrlPrefix, UriKind.Absolute, out var prefix) ||
                (prefix.Scheme != Uri.UriSchemeHttps && prefix.Scheme != Uri.UriSchemeHttp) ||
                !string.IsNullOrEmpty(prefix.Query) || !string.IsNullOrEmpty(prefix.Fragment))
                throw new InvalidOperationException("UrlPrefix must be an absolute URL to the API root.");

            var url = UrlPrefix.TrimEnd('/') + "/" + path + "?input_json=" +
                Uri.EscapeDataString(JsonSerializer.Serialize(request));
            if (!string.IsNullOrEmpty(ApiKey)) url += "&key=" + Uri.EscapeDataString(ApiKey);

            using var response = await Client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // Best-effort capture of a small error body so the caller can tell a
                // definitive not found from a transient failure (the status code alone
                // is not enough for appid_not_allowed-style answers).
                string body = null;
                try
                {
                    using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    using var reader = new StreamReader(stream, Encoding.UTF8, false, 8192, leaveOpen: true);
                    var buffer = new char[8192];
                    var count = reader.Read(buffer, 0, buffer.Length);
                    body = new string(buffer, 0, count);
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception) { /* A missing body is fine; the status code still drives the negative. */ }
                throw new WorkshopApiException(response.StatusCode, body);
            }
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        // GetDetails-specific validation: a response that exists but reports
        // result != 1 or carries no publishedfiledetails is a definitive
        // "not found" (false), while a body with no response object at all is
        // malformed (throw) and must never be negative-cached.
        internal static bool TryGetDetailsResponse(JsonDocument document, out JsonElement details)
        {
            details = default;
            if (!document.RootElement.TryGetProperty("response", out var response) || response.ValueKind != JsonValueKind.Object)
                throw new InvalidDataException("The Workshop API returned no response object.");
            if (Text(response, "result") is string result && result != "1") return false;
            if (!response.TryGetProperty("publishedfiledetails", out details) ||
                details.ValueKind != JsonValueKind.Array || details.GetArrayLength() == 0)
                return false;
            return true;
        }

        // True when the body is {"error": "<expected>", ...}, as the proxy returns
        // for definitive not-found answers.
        internal static bool HasErrorCode(string body, string expected)
        {
            if (string.IsNullOrWhiteSpace(body)) return false;
            try
            {
                using var document = JsonDocument.Parse(body);
                return document.RootElement.ValueKind == JsonValueKind.Object &&
                    Text(document.RootElement, "error") == expected;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static JsonElement Response(JsonDocument document)
        {
            if (!document.RootElement.TryGetProperty("response", out var response) || response.ValueKind != JsonValueKind.Object)
                throw new InvalidDataException("The Workshop API returned no response object.");
            if (Text(response, "result") is string result && result != "1")
                throw new InvalidDataException("The Workshop API returned result " + result + ".");
            return response;
        }

        internal static string Text(JsonElement item, string name)
        {
            if (item.ValueKind != JsonValueKind.Object || !item.TryGetProperty(name, out var value)) return null;
            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.GetRawText(),
                _ => null
            };
        }
    }

    public class WorkshopMod : ILinqListViewFindModsItem
    {
        public string IconKey => UniqueID;
        public string Name => Title ?? ID;
        public string ModSource => "Workshop";
        public Image SmallIcon => null;
        public ListViewItem ListViewItemCache { get; set; }
        // The manager uses this for its own "new" marker, not Workshop search tags.
        public string[] Tags { get; set; }
        public string UniqueID => GetUniqueId(ID);
        public static string GetUniqueId(string workshopId) =>
            workshopId.PadLeft(ulong.MaxValue.ToString(CultureInfo.InvariantCulture).Length, '0') + "-SteamCmd";

        public bool Known
        {
            get => !(Tags?.Contains("new") ?? true);
            set
            {
                if (value) Tags = null;
                else if (!(Tags?.Contains("new") ?? false)) Tags = new[] { "new" };
            }
        }

        public string URL { get; set; }
        public string ID { get; set; }
        public string Image { get; private set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? AuthorUrl { get; set; }
        public int? FileType { get; private set; }
        public bool IsCollection => FileType == 2;
        //public long TimeUpdated { get; private set; }
        public IReadOnlyList<string> Children { get; private set; } = Array.Empty<string>();
        public string RawJson { get; private set; }

        private Image largeIcon;
        private byte[] _imageBytes;
        private readonly object iconLock = new object();

        // The preview bytes are loaded once by EnsurePreviewImageAsync (with the same
        // cache rules as the item JSON); the icon is then decoded lazily with no network
        // access, so virtual row rendering never blocks on a download.
        public Image LargeIcon
        {
            get
            {
                lock (iconLock)
                {
                    if (largeIcon == null && _imageBytes != null)
                    {
                        try
                        {
                            using var memory = new MemoryStream(_imageBytes);
                            using var bitmap = new Bitmap(memory);
                            largeIcon = new Bitmap(bitmap); // Own the pixels after the stream closes.
                        }
                        catch (Exception) { } // A corrupt preview is optional; keep the item usable.
                    }
                    return largeIcon;
                }
            }
        }

        // Loads the preview image honoring the shared cache rules: a fresh cache file is
        // used without any web request, otherwise the image is downloaded and cached, and
        // if the download fails a stale cache file is used when one exists.
        public async Task EnsurePreviewImageAsync(CancellationToken cancellationToken = default)
        {
            if (_imageBytes != null || string.IsNullOrEmpty(Image) || string.IsNullOrEmpty(ID)) return;

            var cachePath = WorkshopContext.PreviewImageCachePath(ID);
            if (WorkshopContext.TryReadFreshCache(cachePath, out var cached) && cached.Length > 0)
            {
                _imageBytes = cached;
                return;
            }

            try
            {
                using var response = await WorkshopContext.ImageClient.GetAsync(Image, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException("The preview image request returned " + (int)response.StatusCode + ".");
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                if (bytes.Length == 0) return;
                _imageBytes = bytes;
                WorkshopContext.WriteCache(cachePath, bytes);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception)
            {
                // A preview is optional; a stale copy is better than nothing.
                if (WorkshopContext.TryReadAnyCache(cachePath, out var stale) && stale.Length > 0)
                    _imageBytes = stale;
            }
        }

        public WorkshopMod(string image) { Image = image; }

        internal static WorkshopMod FromSteam(JsonElement item)
        {
            var id = WorkshopContext.Text(item, "publishedfileid");
            var creator = WorkshopContext.Text(item, "creator");
            var mod = new WorkshopMod(WorkshopContext.Text(item, "preview_url"))
            {
                ID = id,
                URL = "https://steamcommunity.com/sharedfiles/filedetails/?id=" + Uri.EscapeDataString(id),
                Title = WorkshopContext.Text(item, "title"),
                Author = creator, // QueryFiles supplies a SteamID, not the author's display name.
                AuthorUrl = string.IsNullOrEmpty(creator) ? null : "https://steamcommunity.com/profiles/" + creator,
                RawJson = item.GetRawText()
            };
            if (int.TryParse(WorkshopContext.Text(item, "file_type"), out var type)) mod.FileType = type;
            //if (long.TryParse(WorkshopContext.Text(item, "time_updated"), out var updated)) mod.TimeUpdated = updated;
            if (item.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
                mod.Children = children.EnumerateArray()
                    .Select(child => WorkshopContext.Text(child, "publishedfileid"))
                    .Where(childId => !string.IsNullOrEmpty(childId)).ToArray();
            return mod;
        }
    }
}
