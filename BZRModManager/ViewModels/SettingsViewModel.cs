using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RSteam))]
        [NotifyPropertyChangedFor(nameof(b2BZ98RSteam))]
        private string _txtBZ98RSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCSteam))]
        [NotifyPropertyChangedFor(nameof(b2BZCCSteam))]
        private string _txtBZCCSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RGog))]
        [NotifyPropertyChangedFor(nameof(b2BZ98RGog))]
        private string _txtBZ98RGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCMyDocs))]
        [NotifyPropertyChangedFor(nameof(b2BZCCMyDocs))]
        private string _txtBZCCMyDocs;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCGog))]
        [NotifyPropertyChangedFor(nameof(b2BZCCGog))]
        private string _txtBZCCGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bGit))]
        [NotifyPropertyChangedFor(nameof(b2Git))]
        private string _txtGit;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RSteam))]
        [NotifyPropertyChangedFor(nameof(b2BZ98RSteam))]
        private string _strBZ98RSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCSteam))]
        [NotifyPropertyChangedFor(nameof(b2BZCCSteam))]
        private string _strBZCCSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RGog))]
        [NotifyPropertyChangedFor(nameof(b2BZ98RGog))]
        private string _strBZ98RGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCMyDocs))]
        [NotifyPropertyChangedFor(nameof(b2BZCCMyDocs))]
        private string _strBZCCMyDocs;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCGog))]
        [NotifyPropertyChangedFor(nameof(b2BZCCGog))]
        private string _strBZCCGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bGit))]
        [NotifyPropertyChangedFor(nameof(b2Git))]
        private string _strGit;

        [ObservableProperty]
        private bool _chkManageSteamCmd;

        [ObservableProperty]
        private bool _chkManageSteam;

        public bool bBZ98RSteam => StrBZ98RSteam != TxtBZ98RSteam;
        public bool bBZCCSteam => StrBZCCSteam != TxtBZCCSteam;
        public bool bBZ98RGog => StrBZ98RGog != TxtBZ98RGog;
        public bool bBZCCMyDocs => StrBZCCMyDocs != TxtBZCCMyDocs;
        public bool bBZCCGog => StrBZCCGog != TxtBZCCGog;
        public bool bGit => StrGit != TxtGit;

        public bool b2BZ98RSteam => !string.IsNullOrEmpty(StrBZ98RSteam) && StrBZ98RSteam == TxtBZ98RSteam;
        public bool b2BZCCSteam => !string.IsNullOrEmpty(StrBZCCSteam) && StrBZCCSteam == TxtBZCCSteam;
        public bool b2BZ98RGog => !string.IsNullOrEmpty(StrBZ98RGog) && StrBZ98RGog == TxtBZ98RGog;
        public bool b2BZCCMyDocs => !string.IsNullOrEmpty(StrBZCCMyDocs) && StrBZCCMyDocs == TxtBZCCMyDocs;
        public bool b2BZCCGog => !string.IsNullOrEmpty(StrBZCCGog) && StrBZCCGog == TxtBZCCGog;
        public bool b2Git => !string.IsNullOrEmpty(StrGit) && StrGit == TxtGit;


        public SettingsViewModel()
        {
            StrBZ98RSteam = MainViewModel.settings.BZ98RSteamPath;
            StrBZCCSteam = MainViewModel.settings.BZCCSteamPath;
            StrBZ98RGog = MainViewModel.settings.BZ98RGogPath;
            StrBZCCMyDocs = MainViewModel.settings.BZCCMyDocsPath;
            StrBZCCGog = MainViewModel.settings.BZCCGogPath;
            StrGit = MainViewModel.settings.GitPath;
            ChkManageSteamCmd = MainViewModel.settings.ManageSteamCmd;
            ChkManageSteam = MainViewModel.settings.ManageSteam;

            TxtBZ98RSteam = StrBZ98RSteam;
            TxtBZCCSteam = StrBZCCSteam;
            TxtBZ98RGog = StrBZ98RGog;
            TxtBZCCMyDocs = StrBZCCMyDocs;
            TxtBZCCGog = StrBZCCGog;
            TxtGit = StrGit;
        }

        public event EventHandler ManageSettingChanged;

        SemaphoreSlim settingsProtect = new SemaphoreSlim(1, 1);
        private void SaveSettings()
        {
            settingsProtect.Wait();
            try
            {
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(MainViewModel.settings));
            }
            finally
            {
                settingsProtect.Release();
            }
        }
        partial void OnStrBZ98RSteamChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.BZ98RSteamPath = StrBZ98RSteam;
                SaveSettings();
            }
        }
        partial void OnStrBZCCSteamChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.BZCCSteamPath = StrBZCCSteam;
                SaveSettings();
            }
        }
        partial void OnStrBZ98RGogChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.BZ98RGogPath = StrBZ98RGog;
                SaveSettings();
            }
        }
        partial void OnStrBZCCMyDocsChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.BZCCMyDocsPath = StrBZCCMyDocs;
                SaveSettings();
            }
        }
        partial void OnStrBZCCGogChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.BZCCGogPath = StrBZCCGog;
                SaveSettings();
            }
        }
        partial void OnStrGitChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.GitPath = StrGit;
                SaveSettings();
            }
        }
        partial void OnChkManageSteamCmdChanged(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.ManageSteamCmd = ChkManageSteamCmd;
                SaveSettings();
                ManageSettingChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        partial void OnChkManageSteamChanged(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                MainViewModel.settings.ManageSteam = ChkManageSteam;
                SaveSettings();
                ManageSettingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [RelayCommand]
        public void QuickFind(string parameter)
        {
            switch (parameter)
            {
                case "BZ98RSteam":
                    try
                    {
                        foreach (string basePath in SteamVent.FileSystem.SteamProcessInfo.GetSteamLibraryPaths())
                        {
                            string gameFolder = Path.Combine(basePath, "steamapps", "common", "Battlezone 98 Redux");
                            if (Directory.Exists(gameFolder) && File.Exists(Path.Combine(gameFolder, "battlezone98redux.exe")))
                            {
                                TxtBZ98RSteam = basePath;
                                return;
                            }
                        }
                    }
                    catch { }
                    return;
                case "BZCCSteam":
                    try
                    {
                        foreach (string basePath in SteamVent.FileSystem.SteamProcessInfo.GetSteamLibraryPaths())
                        {
                            string gameFolder = Path.Combine(basePath, "steamapps", "common", "BZ2R");
                            if (Directory.Exists(gameFolder) && File.Exists(Path.Combine(gameFolder, "battlezone2.exe")))
                            {
                                TxtBZCCSteam = basePath;
                                return;
                            }
                        }
                    }
                    catch { }
                    return;
                case "BZ98RGOG":
                    {
                        string path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\GOG.com\Games\1454067812", "path", null) as string;
                        TxtBZ98RGog = path;
                    }
                    return;
                case "BZCCMyDocs":
                    {
                        string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        TxtBZCCMyDocs = Path.Combine(docsPath, "My Games", "Battlezone Combat Commander");
                    }
                    return;
                case "BZCCGOG":
                    {
                        string path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\GOG.com\Games\1120387413", "path", null) as string;
                        TxtBZCCGog = path;
                    }
                    return;
                case "GIT":
                    {
                        string FullPathToGit = Where("git.exe");
                        if (!string.IsNullOrWhiteSpace(FullPathToGit) && File.Exists(FullPathToGit))
                        {
                            TxtGit = FullPathToGit;
                            return;
                        }
                        FullPathToGit = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Atlassian", "SourceTree", "git_local", "bin", "git.exe");
                        if (!string.IsNullOrWhiteSpace(FullPathToGit) && File.Exists(FullPathToGit))
                        {
                            TxtGit = FullPathToGit;
                            return;
                        }
                    }
                    return;
            }
        }

        [RelayCommand]
        public void Apply(string parameter)
        {
            switch (parameter)
            {
                case "BZ98RSteam":
                    StrBZ98RSteam = TxtBZ98RSteam;
                    break;
                case "BZCCSteam":
                    StrBZCCSteam = TxtBZCCSteam;
                    break;
                case "BZ98RGOG":
                    StrBZ98RGog = TxtBZ98RGog;
                    break;
                case "BZCCMyDocs":
                    StrBZCCMyDocs = TxtBZCCMyDocs;
                    break;
                case "BZCCGOG":
                    StrBZCCGog = TxtBZCCGog;
                    break;
                case "GIT":
                    StrGit = TxtGit;
                    break;
            }
        }

        private static string Where(string file)
        {
            var paths = Environment.GetEnvironmentVariable("PATH").Split(';');
            var extensions = Environment.GetEnvironmentVariable("PATHEXT").Split(';');
            return (from p in new[] { Environment.CurrentDirectory }.Concat(paths)
                    from e in new[] { string.Empty }.Concat(extensions)
                    let path = Path.Combine(p.Trim(), file + e.ToLower())
                    where File.Exists(path)
                    select path).FirstOrDefault();
        }
    }
}
