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

        public static readonly StyledProperty<IRelayCommand?> CommandProperty =
            AvaloniaProperty.Register<MainNavButton, IRelayCommand?>(nameof(Command), enableDataValidation: true);

        public IRelayCommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly StyledProperty<object?> CommandParameterProperty =
            AvaloniaProperty.Register<MainNavButton, object?>(nameof(CommandParameter));

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
    }
}
