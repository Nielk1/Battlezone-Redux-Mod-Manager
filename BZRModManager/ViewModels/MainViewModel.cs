using Avalonia.Controls;
using BZRModManager.Models;
using BZRModManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
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
using System.Runtime.CompilerServices;
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
    public bool ManageModsIsBusy => vmManageMods.IsBusy
                                 || SteamCmdWorking_BZ98R
                                 || SteamCmdWorking_BZCC
                                 || SteamWorking_BZ98R
                                 || SteamWorking_BZCC;

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
        if (Design.IsDesignMode)
        {
            settings = new SettingsContainer();
        }
        else
        {
            if (!File.Exists("settings.json"))
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(new SettingsContainer()));
            settings = JsonConvert.DeserializeObject<SettingsContainer>(System.IO.File.ReadAllText("settings.json")) ?? throw new Exception("Failed to load settings");
        }

        vmManageMods = new ManageModsViewModel();
        vmLogs = new LogsViewModel();
        vmTasks = new TasksViewModel();
        vmSettings = new SettingsViewModel();
        vmSettings.ManageSettingChanged += (sender, e) =>
        {
            ListModsTask();
        };

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

        ListModsTask();
    }

    SteamVent.Adaptive.SteamContext Steam = new SteamVent.Adaptive.SteamContext();

    private void ShutdownSteam()
    {
        Steam?.Dispose();
        Steam = null;
    }

    bool SteamCmdStartupDone = false;
    bool SteamCmdWorking_BZ98R = false;
    bool SteamCmdWorking_BZCC = false;
    bool SteamWorking_BZ98R = false;
    bool SteamWorking_BZCC = false;
    SemaphoreSlim ListModsTaskLock = new SemaphoreSlim(1, 1);
    FifoSemaphoreSlim ListModsTaskQueueLock = new FifoSemaphoreSlim();
    int ListStack = 0;
    private void ListModsTask()
    {
        if (Design.IsDesignMode)
            return;

        ListModsTaskLock.Wait();
        try
        {
            ListStack++;
            vmTasks.RegisterTask("List Mods Pending", null, null, async (Node) =>
            {
                Node.State = TaskNodeState.Waiting;
                await ListModsTaskQueueLock.WaitAsync();
                try
                {
                    await vmManageMods.ClearMods();
                    Node.Percent = 1;
                    Node.State = TaskNodeState.Finished;
                    OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));

                    // skip subtasks if we have more pending list tasks
                    if (ListStack == 1)
                    {
                        List<Task> Tasks = new List<Task>();

                        if (settings.ManageSourceSteamCmd)
                        {
                            SemaphoreSlim? SteamCmdStartupLock = null;
                            if (!SteamCmdStartupDone)
                                SteamCmdStartupLock = new SemaphoreSlim(0, 1);

                            Tasks.Add(vmTasks.RegisterTask("SteamCmd Workshop Status BZ98R", null, null, async (Node) =>
                            {
                                SteamCmdWorking_BZ98R = true;
                                if (SteamCmdStartupLock != null)
                                    await SteamCmdStartupLock.WaitAsync();
                                await InternalWorkshopModScan(301650, Node);
                                SteamCmdWorking_BZ98R = false;
                                OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
                            }));

                            Tasks.Add(vmTasks.RegisterTask("SteamCmd Workshop Status BZCC", null, null, async (Node) =>
                            {
                                SteamCmdWorking_BZCC = true;
                                if (SteamCmdStartupLock != null)
                                    await SteamCmdStartupLock.WaitAsync();
                                await InternalWorkshopModScan(624970, Node);
                                SteamCmdWorking_BZCC = false;
                                OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
                            }));

                            if (!SteamCmdStartupDone)
                            {
                                Tasks.Add(vmTasks.RegisterTask("SteamCmd Startup", null, null, async (Node) =>
                                {
                                    Node.State = TaskNodeState.Running;
                                    await SteamCmd.DownloadAsync();
                                    await SteamCmd.TestRunAsync();
                                    SteamCmdStartupDone = true;
                                    SteamCmdStartupLock?.Release();
                                    SteamCmdStartupLock?.Release();
                                }));
                            }
                        }

                        if (settings.SourceSteam || settings.ManageSteam)
                        {
                            Tasks.Add(vmTasks.RegisterTask("Steam Workshop Status BZ98R", null, null, async (Node) =>
                            {
                                SteamWorking_BZ98R = true;
                                await ExternalWorkshopModScan(301650, Node);
                                SteamWorking_BZ98R = false;
                                OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
                            }));

                            Tasks.Add(vmTasks.RegisterTask("Steam Workshop Status BZCC", null, null, async (Node) =>
                            {
                                SteamWorking_BZCC = true;
                                await ExternalWorkshopModScan(624970, Node);
                                SteamWorking_BZCC = false;
                                OnPropertyChanged(new PropertyChangedEventArgs("ManageModsIsBusy"));
                            }));
                        }

                        await Task.WhenAll(Tasks);
                    }
                }
                finally
                {
                    ListStack--;
                    ListModsTaskQueueLock.Release();
                    Node.State = TaskNodeState.Finished;
                }
            }).ConfigureAwait(false);
        }
        finally
        {
            ListModsTaskLock.Release();
        }
    }

    public void Shutdown()
    {
        ShutdownSteam();
    }

    private async Task InternalWorkshopModScan(uint appId, TaskNode Node)
    {
        Node.State = TaskNodeState.Waiting;

        List<WorkshopItemStatus>? mods = await SteamCmd.WorkshopStatusAsync(appId, Node, (value) =>
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
                default:
                    Node.State = TaskNodeState.Finished;
                    break;
            }
        });
        if (mods != null)
            await vmManageMods.AddInternalWorkshopModData(appId, mods);
        Node.State = TaskNodeState.Finished;
    }

    private async Task ExternalWorkshopModScan(UInt32 appId, TaskNode Node)
    {
        Node.State = TaskNodeState.Running;

        string? SteamLibraryPath = settings.GetLibraryPathOverrideForAppId(appId);

        if (SteamLibraryPath != null && Directory.Exists(SteamLibraryPath))
        {
            List<WorkshopItemStatus>? mods = await Steam.GetSteamWorkshop().WorkshopStatusAsync(SteamLibraryPath, appId, Node, settings.ManageSteam); // only talk to steam via bridge if we might manage steam because bridge can be strange
            if (mods != null)
                await vmManageMods.AddExternalWorkshopModData(appId, mods);
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
