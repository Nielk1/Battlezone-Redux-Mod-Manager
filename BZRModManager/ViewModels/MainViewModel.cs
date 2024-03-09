using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BZRModManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public string Greeting => "Welcome to Avalonia!";

    [ObservableProperty]
    private ObservableObject _contentViewModel;

    //public MainViewModel()
    //{
    //    ContentViewModel = new SteamCmdViewModel();
    //}

    [RelayCommand]
    private void ChangeContent()
    {
        ContentViewModel = new SteamCmdViewModel();
    }
}
