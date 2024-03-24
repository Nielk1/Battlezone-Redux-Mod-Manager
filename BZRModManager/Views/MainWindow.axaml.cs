using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace BZRModManager.Views;

public partial class MainWindow : Window
{
    private Control? mainNav;
    public MainWindow()
    {
        InitializeComponent();

        mainNav = (this.Content as MainView)?.FindControl<Control>("MainNav");

        PointerPressed += InputElement_OnPointerPressed;
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen) return;

        PointerPoint originalPoint = e.GetCurrentPoint(this);

        if (originalPoint.Position.Y > 32) // 32 is the height of the title bar
        {
            Rect? r = mainNav?.Bounds;
            if (!r.HasValue)
                return;
            if (originalPoint.Position.X > r.Value.Width + r.Value.Left + r.Value.Right)
                return;
        }

        BeginMoveDrag(e);
    }
}
