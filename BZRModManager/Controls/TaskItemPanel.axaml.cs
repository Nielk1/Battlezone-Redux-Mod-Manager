using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using BZRModManager.Models;
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

        public static readonly StyledProperty<TaskNodeState> StateProperty =
            AvaloniaProperty.Register<TaskItemPanel, TaskNodeState>(nameof(State), TaskNodeState.None, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public TaskNodeState State
        {
            get => GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }
    }
}
