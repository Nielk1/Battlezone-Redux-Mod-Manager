using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public enum TaskNodeState
    {
        None, // no state flags
        Waiting,
        Running,
        Delayed,
        Finished,
    }

    public partial class TaskNode : ObservableObject, IProgress<double?>
    {
        public string Text { get; private set; }
        public IImage? ImageSource { get; private set; }
        [ObservableProperty]
        public double? _percent;

        [ObservableProperty]
        public TaskNodeState _state;

        public TaskNode(string text, IImage? image, double? percent)
        {
            Text = text;
            ImageSource = image ?? ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
            Percent = percent;
        }

        public void Report(double? value)
        {
            Percent = value;
        }
    }
}
