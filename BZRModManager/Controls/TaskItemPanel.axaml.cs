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
        public static readonly StyledProperty<TaskNode> TaskNodeProperty =
            AvaloniaProperty.Register<TaskItemPanel, TaskNode>(nameof(TaskNode), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public TaskNode TaskNode
        {
            get => GetValue(TaskNodeProperty);
            set => SetValue(TaskNodeProperty, value);
        }

        public string? Text => TaskNode?.Text;
        public IImage? ImageSource => TaskNode?.ImageSource;
        public double? Percent => TaskNode?.Percent ?? 0d;
        public TaskNodeState State => TaskNode?.State ?? TaskNodeState.None;
    }
}
