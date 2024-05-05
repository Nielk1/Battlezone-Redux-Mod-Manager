using BZRModManager.Models;
using System;
using System.IO;

namespace BZRModManager
{
    public class SettingsContainer
    {
        public string? BZ98RSteamPath { get; set; }
        public string? BZCCSteamPath { get; set; }
        public string? BZ98RGogPath { get; set; }
        public string? BZCCGogPath { get; set; }
        public string? BZCCMyDocsPath { get; set; }
        public string? GitPath { get; set; }
        public bool ManageSteam { get; set; }
        public bool ManageSteamCmd { get; set; }
        //public bool FallbackSteamCmdHandling { get; set; }

        /// <summary>
        /// Get the Library Path Override for the desired AppId.
        /// </summary>
        /// <remarks>
        /// Path must exist or null will be returned.
        /// </remarks>
        /// <param name="appId">App's Steam AppID</param>
        /// <returns>Path from settings or null.</returns>
        public string? GetLibraryPathOverrideForAppId(UInt32 appId)
        {
            string? LibraryPath = null;

            switch (appId)
            {
                case (UInt32)GameId.Battlezone98Redux:
                    LibraryPath = BZ98RSteamPath;
                    break;
                case (UInt32)GameId.BattlezoneComatCommander:
                    LibraryPath = BZCCSteamPath;
                    break;
            }

            if (LibraryPath != null && Directory.Exists(LibraryPath))
                return LibraryPath;

            return null;
        }
    }
}
