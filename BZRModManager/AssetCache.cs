using Avalonia.Media;
using Avalonia.Media.Imaging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager
{
    internal class AssetCache
    {
        private static readonly Lazy<AssetCache> lazyInstance = new Lazy<AssetCache>(() => new AssetCache());
        public static AssetCache Instance = lazyInstance.Value;

        private AssetCache() { }

        // TODO consider adding a date limiter or something
        public async Task<string?> GetData(string url, string? local)
        {
            if (File.Exists(local) && new FileInfo(local).LastWriteTimeUtc.AddDays(1) < DateTime.UtcNow)
            {
                return File.ReadAllText(local);
            }

            using var httpClient = new HttpClient();

            try
            {
                //var httpRequestMessage = new HttpRequestMessage
                //    {
                //        Method = HttpMethod.Get,
                //        RequestUri = new Uri(url),
                //        Headers = { 
                //        },
                //    };
                //if (File.Exists(localFile))
                //    httpRequestMessage.Headers.Add(HttpRequestHeader.IfModifiedSince.ToString(), File.GetLastWriteTimeUtc(localFile).ToString("r"));
                //var response = await httpClient.SendAsync(httpRequestMessage);
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    if (!string.IsNullOrWhiteSpace(local))
                    {
                        if (!File.Exists(local) || !response.Content.Headers.LastModified.HasValue || response.Content.Headers.LastModified > new FileInfo(local).LastWriteTimeUtc)
                        {
                            string localPath = Path.GetDirectoryName(local);
                            if (!Directory.Exists(localPath))
                                Directory.CreateDirectory(localPath);

                            using (Stream stream = await response.Content.ReadAsStreamAsync())
                            using (FileStream fs = File.OpenWrite(local + ".download"))
                            {
                                stream.CopyTo(fs);
                            }
                            File.Delete(local);
                            File.Move(local + ".download", local);
                            return File.ReadAllText(local);
                        }
                    }
                    else
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // return stale data
            }

            if (File.Exists(local))
            {
                return File.ReadAllText(local);
            }

            return null;
        }

        SemaphoreSlim BusyImagesLock = new SemaphoreSlim(1, 1);
        HashSet<string> BusyImages = new HashSet<string>();
        public async Task<IImage?> GetImageAsync(Uri? url, string? local)//, System.Drawing.Point? size, CancellationToken? token = null)
        {
            int? width = null;
            int? height = null;
            if (local != null)
            {
                if (local.Contains(";"))
                {
                    string[] parts = local.Split(';');
                    if (parts.Length > 1)
                    {
                        width = int.Parse(parts[1]);
                        height = width;
                    }
                    if (parts.Length > 2)
                        height = int.Parse(parts[2]);
                    local = parts[0];
                }
            }

            try
            {
                // TODO: use a reader/writer lock here instead where writing the file elivates to a write lock
                for (; ; )
                {
                    await BusyImagesLock.WaitAsync();
                    try
                    {
                        if (!BusyImages.Contains(local))
                        {
                            BusyImages.Add(local);
                            break;
                        }
                    }
                    finally
                    {
                        BusyImagesLock.Release();
                    }
                    await Task.Delay(1000);
                }

                if (url == null && !string.IsNullOrWhiteSpace(local))
                {
                    if (File.Exists(local))
                    {
                        if ((width.HasValue && height.HasValue) || Path.GetExtension(local).ToLowerInvariant() == ".webp")
                        {
                            return await Task.Run(() =>
                            {
                                SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(local);
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    // only resize if the image is larger than the requested size
                                    if (width.HasValue && height.HasValue && (width.Value < image.Width || height.Value < image.Height))
                                        image.Mutate(img => img.Resize(new ResizeOptions()
                                        {
                                            Mode = ResizeMode.Max,
                                            Size = new Size() { Height = height.Value, Width = width.Value },
                                            //Compand = true,
                                        }));
                                    image.SaveAsPng(ms);
                                    ms.Position = 0;
                                    return new Bitmap(ms);
                                }
                            });
                        }
                        return new Bitmap(local);
                    }
                }

                if (url != null && url.Scheme == @"avares")
                {
                    return ImageHelper.LoadFromResource(url);
                }

                // you can have a null local path, but it's probably a bad idea to allow it for non resource URLs
                // TODO added local check here temporarily to reduce load on server, not saving the result is shitty
                if (!string.IsNullOrWhiteSpace(local) && url != null && (url.Scheme == @"http" || url.Scheme == @"https"))
                {
                    using var httpClient = new HttpClient();
                    //httpClient.Timeout = TimeSpan.FromSeconds(1);
                    try
                    {
                        using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            // Check date headers here?

                            if (!string.IsNullOrWhiteSpace(local))
                            {
                                string localPath = Path.GetDirectoryName(local);
                                if (!Directory.Exists(localPath))
                                    Directory.CreateDirectory(localPath);

                                //if (token?.IsCancellationRequested ?? false) return null;

                                using (Stream stream = await response.Content.ReadAsStreamAsync())
                                using (FileStream fs = File.OpenWrite(local))
                                {
                                    stream.CopyTo(fs);
                                }
                            }
                            else
                            {
                                // no webp support here, probably kill it off?
                                var data = await response.Content.ReadAsByteArrayAsync();
                                return new Bitmap(new MemoryStream(data));
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        //return null;
                    }
                    catch (TaskCanceledException ex)
                    {

                    }
                    if (!string.IsNullOrWhiteSpace(local) && File.Exists(local))
                    {
                        // TODO replace this with logic from data builder to inspect file header instead
                        // We'll probably make an exact size image baker for this but the logic will still help other data sources like Steam
                        if ((width.HasValue && height.HasValue) || Path.GetExtension(local).ToLowerInvariant() == ".webp")
                        {
                            return await Task.Run(() =>
                            {
                                SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(local);
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    // only resize if the image is larger than the requested size
                                    if (width.HasValue && height.HasValue && (width.Value < image.Width || height.Value < image.Height))
                                        image.Mutate(img => img.Resize(new ResizeOptions()
                                        {
                                            Mode = ResizeMode.Max,
                                            Size = new Size() { Height = height.Value, Width = width.Value },
                                            //Compand = true,
                                        }));
                                    image.SaveAsPng(ms);
                                    ms.Position = 0;
                                    return new Bitmap(ms);
                                }
                            });
                        }
                        return new Bitmap(local);
                    }
                }
                return null;
            }
            finally
            {
                await BusyImagesLock.WaitAsync();
                try
                {
                    BusyImages.Remove(local);
                }
                finally
                {
                    BusyImagesLock.Release();
                }
            }
        }

        public async Task<(IImage? image, string url, string cacheKey)> GetImageAsync((string url, string cacheKey) current, List<(string url, string cacheKey)> rankedOptions)
        {
            foreach (var rankedOption in rankedOptions)
            {
                // if we hit the one that's already set just stop
                if (rankedOption.url == current.url && rankedOption.cacheKey == current.cacheKey)
                {
                    return (null, rankedOption.url, rankedOption.cacheKey);
                }

                if (!string.IsNullOrWhiteSpace(rankedOption.cacheKey))
                {
                    string localFile = Path.Combine("cache", rankedOption.cacheKey);
                    if (File.Exists(localFile))
                    {
                        return (new Bitmap(localFile), rankedOption.url, rankedOption.cacheKey);
                    }
                }

                Uri url = new Uri(rankedOption.url);
                if (url.Scheme == @"avares")
                {
                    return (ImageHelper.LoadFromResource(url), rankedOption.url, rankedOption.cacheKey);
                }

                using var httpClient = new HttpClient();
                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    if (!string.IsNullOrWhiteSpace(rankedOption.cacheKey))
                    {
                        string localFile = Path.Combine("cache", rankedOption.cacheKey);
                        string localPath = Path.GetDirectoryName(localFile);
                        if (!Directory.Exists(localPath))
                            Directory.CreateDirectory(localPath);

                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        using (FileStream fs = File.OpenWrite(localFile))
                        {
                            stream.CopyTo(fs);
                        }
                        return (new Bitmap(localFile), rankedOption.url, rankedOption.cacheKey);
                    }
                    else
                    {
                        var data = await response.Content.ReadAsByteArrayAsync();
                        return (new Bitmap(new MemoryStream(data)), rankedOption.url, rankedOption.cacheKey);
                    }
                }
                catch (HttpRequestException ex)
                {
                    //return null;
                }
            }
            return (null, null, null);
        }
    }
}
