using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public partial class TaskNode : ObservableObject, IProgress<double?>
    {
        [ObservableProperty]
        public bool _active;
        public string Text { get; }
        public IImage? ImageSource { get; }
        [ObservableProperty]
        public double? _percent;

        public TaskNode(string text, IImage? image, double? percent)
        {
            Text = text;
            ImageSource = image ?? ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
            Percent = percent * 100d;
        }

        public void Report(double? value)
        {
            Percent = value * 100d;
        }
    }
}
