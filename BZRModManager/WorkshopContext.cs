using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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

        // Returns normal Workshop files, newest update first. Tags are ORed, as in the
        // old browse calls; pass null or an empty array to get all files.
        public static List<WorkshopMod> GetMods(int appid, string[] tags)
        {
            if (appid <= 0) throw new ArgumentOutOfRangeException(nameof(appid));

            var mods = new Dictionary<string, WorkshopMod>(StringComparer.Ordinal);
            var cursor = "*";
            var seenCursors = new HashSet<string>(StringComparer.Ordinal) { cursor };
            var requiredTags = tags?.Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim()).Distinct(StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();

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

                using var document = Fetch("IPublishedFileService/QueryFiles/v1/", request);
                var response = Response(document);
                if (!response.TryGetProperty("publishedfiledetails", out var items))
                {
                    // Steam can omit the array on the terminal, empty page.
                    if (Text(response, "next_cursor") == cursor) break;
                    throw new InvalidDataException("QueryFiles omitted publishedfiledetails.");
                }
                if (items.ValueKind != JsonValueKind.Array)
                    throw new InvalidDataException("QueryFiles returned invalid publishedfiledetails.");
                if (items.GetArrayLength() == 0) break; // Steam may repeat this cursor.

                foreach (var item in items.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object ||
                        (Text(item, "result") is string result && result != "1")) continue;
                    var id = Text(item, "publishedfileid");
                    if (!string.IsNullOrEmpty(id)) mods[id] = WorkshopMod.FromSteam(item);
                }

                var next = Text(response, "next_cursor");
                if (string.IsNullOrEmpty(next)) break;
                if (!seenCursors.Add(next))
                    throw new InvalidDataException("QueryFiles repeated a cursor on a nonempty page.");
                cursor = next; // Pass the opaque cursor through without decoding it.
            }

            //return mods.Values.OrderByDescending(m => m.TimeUpdated).ThenBy(m => m.ID, StringComparer.Ordinal).ToList();
            return mods.Values.ToList();
        }

        // Details include file_type (2 for a collection) and its immediate children.
        // Returns null when Steam reports that the ID does not exist.
        public static WorkshopMod GetItem(string id, int appid = 0)
        {
            if (!ulong.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) || parsed == 0)
                throw new ArgumentException("A positive decimal published file ID is required.", nameof(id));
            if (appid < 0) throw new ArgumentOutOfRangeException(nameof(appid));

            var request = new Dictionary<string, object>
            {
                ["publishedfileids"] = new[] { id }, ["includechildren"] = true
            };
            if (appid > 0) request["appid"] = appid;
            JsonDocument document;
            try { document = Fetch("IPublishedFileService/GetDetails/v1/", request); }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }

            using (document)
            {
                var response = Response(document);
                if (!response.TryGetProperty("publishedfiledetails", out var details) ||
                    details.ValueKind != JsonValueKind.Array || details.GetArrayLength() == 0) return null;
                foreach (var item in details.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object && Text(item, "publishedfileid") == id)
                        return Text(item, "result") == "1" ? WorkshopMod.FromSteam(item) : null;
                }
                throw new InvalidDataException("GetDetails returned a different published file ID.");
            }
        }

        private static JsonDocument Fetch(string path, object request)
        {
            if (!Uri.TryCreate(UrlPrefix, UriKind.Absolute, out var prefix) ||
                (prefix.Scheme != Uri.UriSchemeHttps && prefix.Scheme != Uri.UriSchemeHttp) ||
                !string.IsNullOrEmpty(prefix.Query) || !string.IsNullOrEmpty(prefix.Fragment))
                throw new InvalidOperationException("UrlPrefix must be an absolute URL to the API root.");

            var url = UrlPrefix.TrimEnd('/') + "/" + path + "?input_json=" +
                Uri.EscapeDataString(JsonSerializer.Serialize(request));
            if (!string.IsNullOrEmpty(ApiKey)) url += "&key=" + Uri.EscapeDataString(ApiKey);
            using var response = Client.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            using var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
            return JsonDocument.Parse(stream);
        }

        private static JsonElement Response(JsonDocument document)
        {
            if (!document.RootElement.TryGetProperty("response", out var response) ||
                response.ValueKind != JsonValueKind.Object)
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

        internal static HttpClient ImageClient => Client;
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
        private bool iconAttempted;
        private readonly object iconLock = new object();
        public Image LargeIcon
        {
            get
            {
                lock (iconLock)
                {
                    if (!iconAttempted && !string.IsNullOrEmpty(Image))
                    {
                        iconAttempted = true;
                        try
                        {
                            var bytes = WorkshopContext.ImageClient.GetByteArrayAsync(Image).GetAwaiter().GetResult();
                            using var memory = new MemoryStream(bytes);
                            using var bitmap = new Bitmap(memory);
                            largeIcon = new Bitmap(bitmap); // Own the pixels after the stream closes.
                        }
                        catch (Exception) { } // A preview is optional; keep the item usable.
                    }
                    return largeIcon;
                }
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
