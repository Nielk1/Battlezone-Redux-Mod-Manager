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
    }
}
