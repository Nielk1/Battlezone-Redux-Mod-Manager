using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Web;
using System.Collections.Specialized;
using SteamVent;
using BZRModManager.Models;

namespace BZRModManager.ViewModels;
public partial class GetModsViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _enableUrlTypeBZ98RSteamCmd;
    [ObservableProperty]
    private bool _enableUrlTypeBZ98RSteam;
    [ObservableProperty]
    private bool _enableUrlTypeBZ98RGit;

    [ObservableProperty]
    private bool _enableUrlTypeBZCCSteamCmd;
    [ObservableProperty]
    private bool _enableUrlTypeBZCCSteam;
    [ObservableProperty]
    private bool _enableUrlTypeBZCCGit;

    [ObservableProperty]
    private string _url;

    SteamVent.Adaptive.SteamContext Steam = new SteamVent.Adaptive.SteamContext();
    public GetModsViewModel()
    {
        Steam.GetSteamApps();
    }

    internal void Refresh()
    {
        OnUrlChanged(_url);
    }

    internal void Shutdown()
    {
        ShutdownSteam();
    }
    private void ShutdownSteam()
    {
        Steam?.Dispose();
        Steam = null;
    }

    partial void OnUrlChanged(string value)
    {
        if ((value?.Length ?? 0) == 0)
        {
            EnableUrlTypeBZ98RSteamCmd = false;
            EnableUrlTypeBZ98RSteam = false;
            EnableUrlTypeBZ98RGit = false;

            EnableUrlTypeBZCCSteamCmd = false;
            EnableUrlTypeBZCCSteam = false;
            EnableUrlTypeBZCCGit = false;

            return;
        }

        bool ManageSteamBZ98R = false;
        bool ManageSteamBZCC = false;
        if (MainViewModel.settings.ManageSteam)
        {
            ManageSteamBZ98R = Steam.GetSteamApps().GetAppInstalled((UInt32)GameId.Battlezone98Redux);
            ManageSteamBZCC = Steam.GetSteamApps().GetAppInstalled((UInt32)GameId.BattlezoneComatCommander);
        }


        if (UInt64.TryParse(value, out UInt64 workshopID))
        {
            EnableUrlTypeBZ98RSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
            EnableUrlTypeBZ98RSteam = MainViewModel.settings.ManageSteam && ManageSteamBZ98R;
            EnableUrlTypeBZ98RGit = false;

            EnableUrlTypeBZCCSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
            EnableUrlTypeBZCCSteam = MainViewModel.settings.ManageSteam && ManageSteamBZCC;
            EnableUrlTypeBZCCGit = false;

            return;
        }

        if (Uri.TryCreate(value, new UriCreationOptions(), out Uri? uri))
        {
            if (uri?.Host == "steamcommunity.com")
            {
                if (!string.IsNullOrWhiteSpace(uri.Query))
                {
                    NameValueCollection? qs = HttpUtility.ParseQueryString(uri.Query);
                    if (qs != null)
                    {
                        string[]? ids = qs?.GetValues("id");

                        if (ids != null && ids.Length == 1)
                        {
                            if (UInt64.TryParse(ids[0], out workshopID))
                            {
                                EnableUrlTypeBZ98RSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
                                EnableUrlTypeBZ98RSteam = MainViewModel.settings.ManageSteam && ManageSteamBZ98R;
                                EnableUrlTypeBZ98RGit = false;

                                EnableUrlTypeBZCCSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
                                EnableUrlTypeBZCCSteam = MainViewModel.settings.ManageSteam && ManageSteamBZCC;
                                EnableUrlTypeBZCCGit = false;

                                return;
                            }
                        }
                    }
                }
            }
        }

        EnableUrlTypeBZ98RSteamCmd = false;
        EnableUrlTypeBZ98RSteam = false;
        EnableUrlTypeBZ98RGit = false;

        EnableUrlTypeBZCCSteamCmd = false;
        EnableUrlTypeBZCCSteam = false;
        EnableUrlTypeBZCCGit = false;
    }
}