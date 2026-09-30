using System.Collections.Generic;

namespace BZRModManager
{
    /// <summary>
    /// Serializable snapshot of the mod links (junctions) that were installed at the time the
    /// SteamCmd data location was changed. Written by the "Uninstall all installed mods" button
    /// and consumed by the "Reinstall saved mods" button so the links can be recreated (against
    /// the new SteamCmd data location) afterwards.
    /// </summary>
    public class InstalledModsBackup
    {
        public string FileVersion { get; set; } = "1";
        public List<InstalledModsBackupEntry> Mods { get; set; } = new List<InstalledModsBackupEntry>();
    }

    public class InstalledModsBackupEntry
    {
        /// <summary>App ID (BZRModManager.MainForm.AppIdBZ98 / AppIdBZCC) the mod belongs to.</summary>
        public int AppId { get; set; }

        /// <summary>The mod's unique ID (see the *Mod.GetUniqueId methods), e.g. "000012345-SteamCmd".</summary>
        public string ModUniqueId { get; set; }

        /// <summary>"GOG" or "Steam" - the game install the mod was junctioned into.</summary>
        public string InstallTarget { get; set; }

        /// <summary>The mod's display name at save time, purely informational.</summary>
        public string ModName { get; set; }
    }
}