using Avalonia.Controls;
using BZRModManager.Models;
using BZRModManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SteamVent;
using SteamVent.Common;
using SteamVent.InterProc;
using SteamVent.InterProc.Interfaces;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels;

// TODO make sure to fulfill attribution requirements for Flaticon before any release, probably via the about tab

public partial class MainViewModel : ViewModelBase
{
#if !DEBUG
    public string VersionString => $"{Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "VERSION READ ERROR"}";
#endif
#if DEBUG
    public string VersionString => $"{Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "VERSION READ ERROR"} - DEV";
#endif

    #region UI_Pages

    [ObservableProperty]
    private ViewModelBase _contentViewModel;

    static public SettingsContainer settings;

    private ManageModsViewModel vmManageMods;
    private LogsViewModel vmLogs;
    private TasksViewModel vmTasks;
    private SettingsViewModel vmSettings;

    public string? TaskCount => vmTasks.TaskCount > 0 ? vmTasks.TaskCount.ToString() : null;
    public bool ManageModsIsBusy => SteamCmdWorking_BZ98R || SteamCmdWorking_BZCC || vmManageMods.IsBusy;

    [RelayCommand]
    public async Task ChangeContent(string parameter)
    {
        switch (parameter)
        {
            case "manage_mods":
                ContentViewModel = vmManageMods;
                break;
            case "get_mods":
                ContentViewModel = null;
                break;
            case "multiplayer":
                ContentViewModel = null;
                break;
            case "chat":
                ContentViewModel = null;
                break;
            case "settings":
                ContentViewModel = vmSettings;
                break;
            case "tasks":
                await vmTasks.ClearFinishedTasks();
                ContentViewModel = vmTasks;
                break;
            case "logs":
                ContentViewModel = vmLogs;
                break;
            case "about":
                ContentViewModel = null;
                break;
            default:
                ContentViewModel = null;
                break;
        }
    }

    #endregion UI_Pages

    SteamCmdContext SteamCmd = SteamCmdContext.Instance;

    public MainViewModel()
    {
        if(Design.IsDesignMode)
        {
            settings = new SettingsContainer();
        }
        else
        {
            if (!File.Exists("settings.json"))
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(new SettingsContainer()));
            settings = JsonConvert.DeserializeObject<SettingsContainer>(System.IO.File.ReadAllText("settings.json"));
        }

        vmManageMods = new ManageModsViewModel();
        vmLogs = new LogsViewModel();
        vmTasks = new TasksViewModel();
        vmSettings = new SettingsViewModel();

        //SteamCmd.PropertyChanged += SteamCmd_PropertyChanged;

        SteamCmd.SteamCmdOutput += Steam_SteamCmdOutput;
        SteamCmd.SteamCmdOutputFull += Steam_SteamCmdOutputFull;

        vmTasks.PropertyChanged += (sender, e) =>
        {
            switch(e.PropertyName)
            {
                case "TaskCount":
                    OnPropertyChanged(new PropertyChangedEventArgs("TaskCount"));
                    break;
            }
        };

        vmManageMods.PropertyChanged += (sender, e) =>
        {
            switch(e.PropertyName)
            {
                case "IsBusy":
                    OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
                    break;
            }
        };

        ContentViewModel = vmManageMods;

        StartupSteam();

        StartupTasks();
    }


    ISteamClient SteamClient = null;
    Int32 Pipe = 0;
    Int32 User = 0;
    ISteamApps SteamApps = null;
    private void StartupSteam()
    {
        Steam.Load();
        SteamClient = Steam.CreateInterface<ISteamClient017>();
        if (SteamClient != null)
        {
            Pipe = SteamClient.CreateSteamPipe();
            User = SteamClient.ConnectToGlobalUser(Pipe);
            if (User > 0)
            {
                SteamApps = SteamClient.GetISteamApps<ISteamApps008>(User, Pipe);
            }
        }
    }

    private void ShutdownSteam()
    {
        if (SteamClient == null)
            return;

        SteamClient.ReleaseUser(Pipe, User);
        SteamClient.BReleaseSteamPipe(Pipe);
    }

    bool SteamCmdStartupDone = false;
    bool SteamCmdWorking_BZ98R = true;
    bool SteamCmdWorking_BZCC = true;
    bool SteamWorking_BZ98R = true;
    bool SteamWorking_BZCC = true;
    private void StartupTasks()
    {
        if (Design.IsDesignMode)
            return;

        SemaphoreSlim SteamStartupLock = new SemaphoreSlim(0, 1);
        vmTasks.RegisterTask("SteamCmd Startup", null, null, async (Node) =>
        {
            Node.State = TaskNodeState.Running;
            await SteamCmd.DownloadAsync();
            await SteamCmd.TestRunAsync();
            SteamCmdStartupDone = true;
            SteamStartupLock.Release();
            SteamStartupLock.Release();
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("SteamCmd Workshop Status BZ98R", null, null, async (Node) =>
        {
            if (!SteamCmdStartupDone)
                await SteamStartupLock.WaitAsync();
            await WorkshopModScan(301650, Node);
            SteamCmdWorking_BZ98R = false;
            OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("SteamCmd Workshop Status BZCC", null, null, async (Node) =>
        {
            if (!SteamCmdStartupDone)
                await SteamStartupLock.WaitAsync();
            await WorkshopModScan(624970, Node);
            SteamCmdWorking_BZCC = false;
            OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("Steam Workshop Status BZ98R", null, null, async (Node) =>
        {
            await SteamWorkshopModScan(301650, Node);
            SteamWorking_BZ98R = false;
            OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("Steam Workshop Status BZCC", null, null, async (Node) =>
        {
            await SteamWorkshopModScan(624970, Node);
            SteamWorking_BZCC = false;
            OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
        }).ConfigureAwait(false);
    }

    public void Shutdown()
    {
        ShutdownSteam();
    }

    private async Task WorkshopModScan(uint appId, TaskNode Node)
    {
        Node.State = TaskNodeState.Waiting;

        // dynamicly adjust status based on being busy
        Node.StatusReceivedEvent += (ESteamCmdTaskStatus value) =>
        {
            switch (value)
            {
                case ESteamCmdTaskStatus.Waiting:
                    Node.State = TaskNodeState.Delayed;
                    break;
                case ESteamCmdTaskStatus.WaitingToStart:
                case ESteamCmdTaskStatus.Running:
                case ESteamCmdTaskStatus.Finished:
                    Node.State = TaskNodeState.Running;
                    break;
            }
        };
        List<WorkshopItemStatus> mods = await SteamCmd.WorkshopStatusAsync(appId, Node, Node);
        vmManageMods.AddInternalWorkshopModData(appId, mods);
    }

    private async Task SteamWorkshopModScan(uint appId, TaskNode Node)
    {
        Node.State = TaskNodeState.Waiting;

        if (SteamApps.BIsAppInstalled(appId))
        {
            string InstallDir = SteamApps.GetAppInstallDir(appId);
            InstallDir = Path.GetDirectoryName(InstallDir);
            InstallDir = Path.GetDirectoryName(InstallDir);
            List<WorkshopItemStatus> mods = await SteamVent.FileSystem.Workshop.WorkshopStatusAsync(InstallDir, appId, Node);
            vmManageMods.AddExternalWorkshopModData(appId, mods);
        }
        Node.State = TaskNodeState.Finished;
    }

    private void Steam_SteamCmdOutputFull(object sender, string msg)
    {
        vmLogs.RawLog += msg;
    }

    private void Steam_SteamCmdOutput(object sender, string msg)
    {
        vmLogs.CleanLog += msg;
    }
}
