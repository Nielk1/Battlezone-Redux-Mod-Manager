using BZRModManager.Models;
using MyToolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class DesignManageModsViewModel : ManageModsViewModel
    {
        public DesignManageModsViewModel() : base()
        {
            AllMods = new MtObservableCollection<ModData>();
            for (int i = 0; i < 100; i++)
            {
                AllMods.Add(new ModData(i % 2 == 0 ? GameId.Battlezone98Redux : GameId.BattlezoneComatCommander, $"testmod{i}")
                {
                    Title = $"Test Mod {i}",
                    Description = "This is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod",
                    HasUpdate = i % 3 == 0,
                    //IonDriverData = i % 4 == 0 ? new IonDriverMod() { } : null,
                });
            }
            FilteredMods = new ObservableCollectionView<ModData>(AllMods);
        }
    }
}
