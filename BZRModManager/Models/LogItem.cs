using Avalonia.Media;
using BZRModManager.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public partial class LogItem : ObservableObject
    {
        public IBrush Color { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        
        [ObservableProperty]
        public string _text;
        [ObservableProperty]
        public int _lines;
    }
}
