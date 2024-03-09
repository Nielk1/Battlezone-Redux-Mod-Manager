using BZRModManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamVent.SteamCmd;
using System;
using System.Reflection;

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
    private SteamCmdViewModel vmSteamCmd = new SteamCmdViewModel();

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
                ContentViewModel = null;
                break;
            case "steam_cmd":
                ContentViewModel = vmSteamCmd;
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
        SteamCmd.DownloadAsync();
        SteamCmd.TestRunAsync();
    }

    private void Steam_SteamCmdOutputFull(object sender, string msg)
    {
        vmSteamCmd.RawLog += msg;
    }

    private void Steam_SteamCmdOutput(object sender, string msg)
    {
        vmSteamCmd.CleanLog += msg;
    }
}
