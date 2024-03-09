using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BZRModManager.Views;

public partial class MainView : UserControl
{
    //public ObservableObject ViewModel { get; set; }
    public MainView()
    {
        InitializeComponent();
        //ViewModel = (ObservableObject?)DataContext;
    }

}
