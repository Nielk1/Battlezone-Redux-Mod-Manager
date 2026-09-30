namespace BZRModManager
{
    public class SettingsContainer
    {
        public string BZ98RSteamPath { get; set; }
        public string BZCCSteamPath { get; set; }
        public string BZ98RGogPath { get; set; }
        public string BZCCGogPath { get; set; }
        public string BZCCMyDocsPath { get; set; }
        public string GitPath { get; set; }
        public bool FallbackSteamCmdHandling { get; set; }

        /// <summary>
        /// Optional directory that SteamCmd should store its data (steamapps, etc.) in, passed to
        /// SteamCmd as <c>+force_install_dir</c>. When null or empty SteamCmd keeps its default
        /// location (the "steamcmd" folder next to the application).
        /// </summary>
        public string SteamCmdInstallDir { get; set; }
    }
}
