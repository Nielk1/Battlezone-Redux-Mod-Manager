using BZRModManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamVent.SteamCmd;
using System;
using System.Reflection;
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
        Task.Run(async () =>
        {
            await SteamCmd.DownloadAsync();
            await SteamCmd.TestRunAsync();
        }).ConfigureAwait(false);

        Task.Run(async () =>
        {
            await foreach (WorkshopItemStatus status in SteamCmd.WorkshopStatusAsync(301650))
            {
                status.ToString();
            }

        }).ConfigureAwait(false);

        Task.Run(async () =>
        {
            await foreach (WorkshopItemStatus status in SteamCmd.WorkshopStatusAsync(624970))
            {
                status.ToString();
            }

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
