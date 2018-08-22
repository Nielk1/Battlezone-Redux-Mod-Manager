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

        public static List<long> WorkshopItemsOnDrive(string steamPath, int appId)
        {
            string workshopFolder = WorkshopFolder(steamPath, appId);
            return Directory.EnumerateDirectories(workshopFolder)
                .Where(dr => !JunctionPoint.Exists(dr)) // not a junction
                .Select(dr =>
                {
                    long tmp = -1;
                    if (!long.TryParse(Path.GetFileName(dr), out tmp))
                    {
                        tmp = -1;
                    }
                    return tmp;
                })
                .Where(dr => dr > -1) // not numeric
                .Where(dr => Directory.GetFiles(Path.Combine(workshopFolder, dr.ToString())).Length > 0) // not empty
                .ToList();
        }
    }
}
