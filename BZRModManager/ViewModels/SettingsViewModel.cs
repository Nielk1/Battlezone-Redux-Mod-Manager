using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RSteam))]
        private string _txtBZ98RSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCSteam))]
        private string _txtBZCCSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RGog))]
        private string _txtBZ98RGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCMyDocs))]
        private string _txtBZCCMyDocs;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCGog))]
        private string _txtBZCCGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bGit))]
        private string _txtGit;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RSteam))]
        private string _strBZ98RSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCSteam))]
        private string _strBZCCSteam;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZ98RGog))]
        private string _strBZ98RGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCMyDocs))]
        private string _strBZCCMyDocs;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bBZCCGog))]
        private string _strBZCCGog;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(bGit))]
        private string _strGit;

        public bool bBZ98RSteam => StrBZ98RSteam != TxtBZ98RSteam;
        public bool bBZCCSteam => StrBZCCSteam != TxtBZCCSteam;
        public bool bBZ98RGog => StrBZ98RGog != TxtBZ98RGog;
        public bool bBZCCMyDocs => StrBZCCMyDocs != TxtBZCCMyDocs;
        public bool bBZCCGog => StrBZCCGog != TxtBZCCGog;
        public bool bGit => StrGit != TxtGit;

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
                                TxtBZ98RSteam = Path.Combine(basePath, "steamapps");
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
                                TxtBZCCSteam = Path.Combine(basePath, "steamapps");
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
