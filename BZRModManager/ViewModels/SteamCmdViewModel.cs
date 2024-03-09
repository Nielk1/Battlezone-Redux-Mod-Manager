using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class SteamCmdViewModel : ViewModelBase
    {
        public string RawLog { get; internal set; }
        public string CleanLog { get; internal set; }

        public SteamCmdViewModel()
        {
            RawLog = string.Empty;
            CleanLog = string.Empty;
        }
    }
}
