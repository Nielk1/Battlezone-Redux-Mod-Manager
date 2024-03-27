using Avalonia.Controls;
using BZRModManager.Models;
using BZRModManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

    private ManageModsViewModel vmManageMods = new ManageModsViewModel();
    private LogsViewModel vmLogs = new LogsViewModel();
    private TasksViewModel vmTasks = new TasksViewModel();

    public string? TaskCount => vmTasks.TaskCount > 0 ? vmTasks.TaskCount.ToString() : null;
    public bool ManageModsIsBusy => vmManageMods.IsBusy;

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
                ContentViewModel = null;
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

        StartupTasks();
    }

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
            SteamStartupLock.Release();
            SteamStartupLock.Release();
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("SteamCmd Workshop Status BZ98R", null, null, async (Node) =>
        {
            await SteamStartupLock.WaitAsync();
            await WorkshopModScan(301650, Node);
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("SteamCmd Workshop Status BZCC", null, null, async (Node) =>
        {
            await SteamStartupLock.WaitAsync();
            await WorkshopModScan(624970, Node);
        }).ConfigureAwait(false);
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
        vmManageMods.AddWorkshopModData(appId, mods);
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
