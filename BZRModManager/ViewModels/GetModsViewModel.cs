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
using System.Net.Http;
using System.Threading;
using System.Collections.ObjectModel;
using DynamicData;
using System.ComponentModel;

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
    private bool _enableUrlTypeSteamCmd;
    [ObservableProperty]
    private bool _enableUrlTypeSteam;

    public bool EnableUrlTypeGit => GitBranches.Count > 0;
    public ObservableCollection<string> GitBranches { get; set; }

    [ObservableProperty]
    private string _url;

    [ObservableProperty]
    private bool _fromUrlIsBusy;

    SteamVent.Adaptive.SteamContext Steam = new SteamVent.Adaptive.SteamContext();
    public GetModsViewModel()
    {
        GitBranches = new ObservableCollection<string>();
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

    private CancellationTokenSource? workshopDebounceCancellationToken;
    private SemaphoreSlim workshopWebLock = new SemaphoreSlim(1, 1);
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

            EnableUrlTypeSteamCmd = false;
            EnableUrlTypeSteam = false;
            GitBranches.Clear();
            OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));

            return;
        }

        bool ManageSteamBZ98R = false;
        bool ManageSteamBZCC = false;
        if (MainViewModel.settings.ManageSteam)
        {
            ManageSteamBZ98R = Steam.GetSteamApps().GetAppInstalled((UInt32)GameId.Battlezone98Redux);
            ManageSteamBZCC = Steam.GetSteamApps().GetAppInstalled((UInt32)GameId.BattlezoneComatCommander);
        }

        if (UInt64.TryParse(value, out UInt64 workshopId))
        {
            ProcWorkshopId(workshopId, ManageSteamBZ98R, ManageSteamBZCC);
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
                            if (UInt64.TryParse(ids[0], out workshopId))
                            {
                                EnableUrlTypeBZ98RSteamCmd = false;
                                EnableUrlTypeBZ98RSteam = false;
                                EnableUrlTypeBZ98RGit = false;

                                EnableUrlTypeBZCCSteamCmd = false;
                                EnableUrlTypeBZCCSteam = false;
                                EnableUrlTypeBZCCGit = false;

                                EnableUrlTypeSteamCmd = false;
                                EnableUrlTypeSteam = false;
                                GitBranches.Clear();
                                OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));

                                ProcWorkshopId(workshopId, ManageSteamBZ98R, ManageSteamBZCC);
                                return;
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

                EnableUrlTypeSteamCmd = false;
                EnableUrlTypeSteam = false;
                GitBranches.Clear();
                OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));

                return;
            }

            // confirmed git
            if (uri?.Scheme == "git")
            {
                ProcGit(uri);

                //EnableUrlTypeBZ98RSteamCmd = false;
                //EnableUrlTypeBZ98RSteam = false;
                //EnableUrlTypeBZ98RGit = true;
                //
                //EnableUrlTypeBZCCSteamCmd = false;
                //EnableUrlTypeBZCCSteam = false;
                //EnableUrlTypeBZCCGit = true;

                return;
            }

            // possible git
            if (uri != null)
            {
                //if (ProcGitAsync(uri))
                //    return;

                //EnableUrlTypeBZ98RSteamCmd = false;
                //EnableUrlTypeBZ98RSteam = false;
                //EnableUrlTypeBZ98RGit = true;
                //
                //EnableUrlTypeBZCCSteamCmd = false;
                //EnableUrlTypeBZCCSteam = false;
                //EnableUrlTypeBZCCGit = true;
                //
                //return;

                ProcGit(uri);
                return;
            }
        }

        EnableUrlTypeBZ98RSteamCmd = false;
        EnableUrlTypeBZ98RSteam = false;
        EnableUrlTypeBZ98RGit = false;

        EnableUrlTypeBZCCSteamCmd = false;
        EnableUrlTypeBZCCSteam = false;
        EnableUrlTypeBZCCGit = false;

        EnableUrlTypeSteamCmd = false;
        EnableUrlTypeSteam = false;
        GitBranches.Clear();
        OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));
    }

    private void ProcGit(Uri uri)
    {
        FromUrlIsBusy = true;
        workshopDebounceCancellationToken?.Cancel();
        workshopDebounceCancellationToken = new CancellationTokenSource();

        EnableUrlTypeBZ98RSteamCmd = false;
        EnableUrlTypeBZ98RSteam = false;
        EnableUrlTypeBZ98RGit = false;

        EnableUrlTypeBZCCSteamCmd = false;
        EnableUrlTypeBZCCSteam = false;
        EnableUrlTypeBZCCGit = false;

        EnableUrlTypeSteamCmd = false;
        EnableUrlTypeSteam = false;
        GitBranches.Clear();
        OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));

        Task.Run(async () =>
        {
            CancellationToken tok = workshopDebounceCancellationToken.Token;

            await Task.Delay(500); // this is basically how we debounce

            if (tok.IsCancellationRequested)
                return;

            await workshopWebLock.WaitAsync();
            try
            {
                if (tok.IsCancellationRequested)
                    return;

                if (MainViewModel.settings.GitPath == null || !System.IO.File.Exists(MainViewModel.settings.GitPath))
                {
                    EnableUrlTypeBZ98RSteamCmd = false;
                    EnableUrlTypeBZ98RSteam = false;
                    EnableUrlTypeBZ98RGit = false;

                    EnableUrlTypeBZCCSteamCmd = false;
                    EnableUrlTypeBZCCSteam = false;
                    EnableUrlTypeBZCCGit = false;

                    EnableUrlTypeSteamCmd = false;
                    EnableUrlTypeSteam = false;
                    GitBranches.Clear();
                    OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));

                    return;
                }

                string[] branches = await GitContext.GetModBranchesAsync(MainViewModel.settings.GitPath, uri.ToString());

                if (branches.Count() > 0)
                {
                    EnableUrlTypeBZ98RSteamCmd = false;
                    EnableUrlTypeBZ98RSteam = false;
                    EnableUrlTypeBZ98RGit = true;

                    EnableUrlTypeBZCCSteamCmd = false;
                    EnableUrlTypeBZCCSteam = false;
                    EnableUrlTypeBZCCGit = true;

                    EnableUrlTypeSteamCmd = false;
                    EnableUrlTypeSteam = false;
                    GitBranches.Clear();
                    GitBranches.AddRange(branches);
                    OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));
                }
                else
                {
                    EnableUrlTypeBZ98RSteamCmd = false;
                    EnableUrlTypeBZ98RSteam = false;
                    EnableUrlTypeBZ98RGit = false;

                    EnableUrlTypeBZCCSteamCmd = false;
                    EnableUrlTypeBZCCSteam = false;
                    EnableUrlTypeBZCCGit = false;

                    EnableUrlTypeSteamCmd = false;
                    EnableUrlTypeSteam = false;
                    GitBranches.Clear();
                    OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                EnableUrlTypeBZ98RSteamCmd = false;
                EnableUrlTypeBZ98RSteam = false;
                EnableUrlTypeBZ98RGit = false;

                EnableUrlTypeBZCCSteamCmd = false;
                EnableUrlTypeBZCCSteam = false;
                EnableUrlTypeBZCCGit = false;

                EnableUrlTypeSteamCmd = false;
                EnableUrlTypeSteam = false;
                GitBranches.Clear();
                OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));

                if (ex.Message == @"The system cannot find the file specified")
                {
                    //MessageBox.Show("Workshop ID was not detected, GIT download attempted.\r\ngit.exe not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return;
                }

                return;
            }
            finally
            {
                FromUrlIsBusy = false;
                workshopWebLock.Release();
            }
        }, workshopDebounceCancellationToken.Token);

        //return true;
    }

    private bool ProcWorkshopId(ulong workshopId, bool ManageSteamBZ98R, bool ManageSteamBZCC)
    {
        // no steam based systems are active, so abort early
        if (!MainViewModel.settings.ManageSourceSteamCmd && !(MainViewModel.settings.ManageSteam && (ManageSteamBZ98R || ManageSteamBZCC)))
            return false;

        // the first mod id used by this game is over 650000000 so use that as a cutoff
        if (workshopId > 650000000)
        {
            FromUrlIsBusy = true;
            workshopDebounceCancellationToken?.Cancel();
            workshopDebounceCancellationToken = new CancellationTokenSource();
            Task.Run(async () =>
            {
                CancellationToken tok = workshopDebounceCancellationToken.Token;

                await Task.Delay(500); // this is basically how we debounce

                if (tok.IsCancellationRequested)
                    return;

                await workshopWebLock.WaitAsync();
                try
                {
                    if (tok.IsCancellationRequested)
                        return;

                    UInt32? appId = await SteamVent.Web.SteamWorkshop.WorkshopAppIdFromWebAsync(workshopId);
                    if (appId.HasValue)
                    {
                        if (appId == (UInt32)GameId.Battlezone98Redux)
                        {
                            EnableUrlTypeBZ98RSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
                            EnableUrlTypeBZ98RSteam = MainViewModel.settings.ManageSteam && ManageSteamBZ98R;
                            EnableUrlTypeBZ98RGit = false;

                            EnableUrlTypeSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
                            EnableUrlTypeSteam = MainViewModel.settings.ManageSteam && ManageSteamBZ98R;
                            GitBranches.Clear();
                            OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));
                        }
                        else if (appId == (UInt32)GameId.BattlezoneComatCommander)
                        {
                            EnableUrlTypeBZCCSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
                            EnableUrlTypeBZCCSteam = MainViewModel.settings.ManageSteam && ManageSteamBZCC;
                            EnableUrlTypeBZCCGit = false;

                            EnableUrlTypeSteamCmd = MainViewModel.settings.ManageSourceSteamCmd;
                            EnableUrlTypeSteam = MainViewModel.settings.ManageSteam && ManageSteamBZCC;
                            GitBranches.Clear();
                            OnPropertyChanged(new PropertyChangedEventArgs("EnableUrlTypeGit"));
                        }
                    }
                }
                finally
                {
                    FromUrlIsBusy = false;
                    workshopWebLock.Release();
                }
            }, workshopDebounceCancellationToken.Token);

            return true;
        }

        return false;
    }
}