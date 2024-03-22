using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class ManageModsViewModel : ViewModelBase
    {
        public ObservableCollection<WorkshopItemStatus> Mods { get; set; }
        public ManageModsViewModel()
        {
            Mods = new ObservableCollection<WorkshopItemStatus>();
        }

        internal void AddMods(int appId, List<WorkshopItemStatus> mods)
        {
            if (mods != null)
            {
                lock (Mods)
                {
                    Mods.AddRange(mods);
                }
            }
        }
    }
}
