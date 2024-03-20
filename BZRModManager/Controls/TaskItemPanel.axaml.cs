using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;

namespace BZRModManager.Controls
{
    public class TaskItemPanel : TemplatedControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<MainNavButton, string>(nameof(Text), "Text", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<IImage?> ImageSourceProperty =
            AvaloniaProperty.Register<MainNavButton, IImage?>(nameof(ImageSource), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public IImage? ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly StyledProperty<double?> PercentProperty =
            AvaloniaProperty.Register<MainNavButton, double?>(nameof(Percent), default(double?), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public double? Percent
        {
            get => GetValue(PercentProperty);
            set => SetValue(PercentProperty, value);
        }

        public static readonly StyledProperty<bool> ActiveProperty =
            AvaloniaProperty.Register<MainNavButton, bool>(nameof(Active), false, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public bool Active
        {
            get => GetValue(ActiveProperty);
            set => SetValue(ActiveProperty, value);
        }
    }
}
