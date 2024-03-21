using BZRModManager.Models;
using BZRModManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels;

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

    [RelayCommand]
    public void ChangeContent(string parameter)
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
                vmTasks.ClearFinishedTasks();
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

        // We are blocking the UI thread here somehow, need to move this logic to another location.
        // Should rework the entire system into tasks run by a task handler.
        SemaphoreSlim tmpSem = new SemaphoreSlim(0, 1);
        vmTasks.RegisterTask("SteamCmd Startup", null, null, async (Node) =>
        {
            await Task.Delay(1000);
            Node.Active = true;
            await Task.Delay(1000);
            await SteamCmd.DownloadAsync();
            await SteamCmd.TestRunAsync();
            tmpSem.Release();
            tmpSem.Release();
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("SteamCmd Workshop Status BZ98R", null, null, async (Node) =>
        {
            await tmpSem.WaitAsync();
            Node.Active = true;
            List<WorkshopItemStatus> mods = await SteamCmd.WorkshopStatusAsync(301650, Node);
            vmManageMods.AddMods(301650, mods);
        }).ConfigureAwait(false);

        vmTasks.RegisterTask("SteamCmd Workshop Status BZCC", null, null, async (Node) =>
        {
            await tmpSem.WaitAsync();
            Node.Active = true;
            List<WorkshopItemStatus> mods = await SteamCmd.WorkshopStatusAsync(624970, Node);
            vmManageMods.AddMods(624970, mods);
        }).ConfigureAwait(false);
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
