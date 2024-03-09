using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;

namespace BZRModManager.Controls
{
    public class MainNavButton : TemplatedControl
    {
        public static readonly StyledProperty<Geometry?> IconDataProperty =
            AvaloniaProperty.Register<MainNavButton, Geometry?>(nameof(IconData), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public Geometry? IconData
        {
            get => GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }
        
        public static readonly StyledProperty<IImage?> ImageSourceProperty =
            AvaloniaProperty.Register<MainNavButton, IImage?>(nameof(ImageSource), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public IImage? ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<MainNavButton, string>(nameof(Text), "Text", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<RelayCommand?> CommandProperty =
            AvaloniaProperty.Register<MainNavButton, RelayCommand?>(nameof(Command), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public RelayCommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}
