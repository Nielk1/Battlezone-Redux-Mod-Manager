using Monitor.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager
{
    class SteamContext
    {
        public static string WorkshopFolder(string steamPath, int appId)
        {
            return Path.Combine(steamPath, "workshop", "content", appId.ToString());
        }

        public static async IAsyncEnumerable<UInt64> WorkshopItemsOnDriveAsync(string steamPath, int appId)
        {
            string workshopFolder = WorkshopFolder(steamPath, appId);

            if (!Directory.Exists(workshopFolder))
                yield break;

            foreach (string dr in Directory.EnumerateDirectories(workshopFolder))
            {
                if (JunctionPoint.Exists(dr))
                    continue;

                if (!UInt64.TryParse(Path.GetFileName(dr), out UInt64 id) || id == 0) // not numeric (0 is not valid)
                    continue;

                if (!Directory.EnumerateFiles(dr).Any())
                    continue;

                yield return id;

                // Gives the caller/UI an opportunity to run between entries.
                await Task.Yield();
            }
        }
    }
}
