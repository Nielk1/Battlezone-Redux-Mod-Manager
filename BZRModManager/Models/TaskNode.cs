using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public class TaskNode
    {
        public string Text { get; }
        public IImage? ImageSource { get; }
        public double? Percent { get; }

        public TaskNode(string text, IImage? image, double? percent)
        {
            Text = text;
            ImageSource = image ?? ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
            Percent = percent;
        }
    }
}
