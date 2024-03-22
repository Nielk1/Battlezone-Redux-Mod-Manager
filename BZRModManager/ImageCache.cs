using Avalonia.Media;
using Avalonia.Media.Imaging;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager
{
    internal class ImageCache
    {
        private static readonly Lazy<ImageCache> lazyInstance = new Lazy<ImageCache>(() => new ImageCache());
        public static ImageCache Instance = lazyInstance.Value;

        private ImageCache() { }

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
                    string localFile = Path.Combine("imagecache", rankedOption.cacheKey);
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
                        string localFile = Path.Combine("imagecache", rankedOption.cacheKey);
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
