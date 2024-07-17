using Avalonia.Controls;
using Avalonia.Media;
using BZRModManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MyToolkit.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class LogsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _rawLog;
        [ObservableProperty]
        private string _cleanLog;
        //public string RawLog { get; internal set; }
        //public string CleanLog { get; internal set; }
        
        
        //public ObservableCollectionView<ModFilter> Filters { get; private set; }
        public ObservableCollection<LogItem> LogItems { get; private set; }

        
        public LogsViewModel()
        {
            RawLog = string.Empty;
            CleanLog = string.Empty;

            LogItems = new ObservableCollection<LogItem>();

            // this doesn't seem to be working
            if (Design.IsDesignMode)
            {
                LogItems.Add(new LogItem { Lines = 1, Color = Brushes.Gray, Type = "Type", SubType = "SubType", Text = "Text Msg 1" });
                LogItems.Add(new LogItem { Lines = 1, Color = Brushes.Red, Type = "Type", SubType = "SubType", Text = "Text Msg 2" });
                LogItems.Add(new LogItem { Lines = 1, Color = Brushes.Orange, Type = "Type", SubType = "SubType", Text = "Text Msg 3" });
                LogItems.Add(new LogItem { Lines = 1, Color = Brushes.Green, Type = "Type", SubType = "SubType", Text = "Text Msg 4" });
                LogItems.Add(new LogItem { Lines = 1, Color = Brushes.Blue, Type = "Type", SubType = "SubType", Text = "Text Msg 5" });
                LogItems.Add(new LogItem { Lines = 1, Color = Brushes.Yellow, Type = "Type", SubType = "SubType", Text = "Text Msg 6" });
            }
        }

        internal void AddLogItem(IBrush Color, string Type, string SubType, string Text)
        {
            lock (LogItems)
            {
                //LogItem? lastItem = LogItems.LastOrDefault();
                //if (lastItem != null && lastItem.Lines < 8 && lastItem.Color == Color && lastItem.Type == Type && lastItem.SubType == SubType)
                //{
                //    lastItem.Text += Environment.NewLine + Text;
                //    lastItem.Lines++;
                //    return;
                //}
                LogItems.Add(new LogItem { Lines = 1, Color = Color, Type = Type, SubType = SubType, Text = Text });
            }
        }
    }
}
