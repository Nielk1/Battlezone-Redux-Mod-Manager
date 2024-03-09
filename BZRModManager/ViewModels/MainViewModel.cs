using BZRModManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BZRModManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

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
}
