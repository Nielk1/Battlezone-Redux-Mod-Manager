using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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

        public LogsViewModel()
        {
            RawLog = string.Empty;
            CleanLog = string.Empty;
        }
    }
}
