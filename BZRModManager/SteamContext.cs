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

        public static List<UInt64> WorkshopItemsOnDrive(string steamPath, int appId)
        {
            string workshopFolder = WorkshopFolder(steamPath, appId);
            if (!Directory.Exists(workshopFolder))
                return null;
            return Directory.EnumerateDirectories(workshopFolder)
                .Where(dr => !JunctionPoint.Exists(dr)) // not a junction
                .Select(dr =>
                {
                    UInt64 tmp = 0;
                    if (!UInt64.TryParse(Path.GetFileName(dr), out tmp))
                    {
                        tmp = 0;
                    }
                    return tmp;
                })
                .Where(dr => dr > 0) // not numeric (0 is not valid)
                .Where(dr => Directory.GetFiles(Path.Combine(workshopFolder, dr.ToString())).Length > 0) // not empty
                .ToList();
        }
    }
}
