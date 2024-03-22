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
            AvaloniaProperty.Register<TaskItemPanel, string>(nameof(Text), "Text", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<IImage?> ImageSourceProperty =
            AvaloniaProperty.Register<TaskItemPanel, IImage?>(nameof(ImageSource), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public IImage? ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly StyledProperty<double?> PercentProperty =
            AvaloniaProperty.Register<TaskItemPanel, double?>(nameof(Percent), default(double?), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public double? Percent
        {
            get => GetValue(PercentProperty);
            set => SetValue(PercentProperty, value);
        }

        public static readonly StyledProperty<bool> ActiveProperty =
            AvaloniaProperty.Register<TaskItemPanel, bool>(nameof(Active), false, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public bool Active
        {
            get => GetValue(ActiveProperty);
            set => SetValue(ActiveProperty, value);
        }

        public static readonly StyledProperty<bool> DelayedProperty =
            AvaloniaProperty.Register<TaskItemPanel, bool>(nameof(Delayed), false, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public bool Delayed
        {
            get => GetValue(DelayedProperty);
            set => SetValue(DelayedProperty, value);
        }

        public static readonly StyledProperty<bool> FinishedProperty =
            AvaloniaProperty.Register<TaskItemPanel, bool>(nameof(Finished), false, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public bool Finished
        {
            get => GetValue(FinishedProperty);
            set => SetValue(FinishedProperty, value);
        }
    }
}
