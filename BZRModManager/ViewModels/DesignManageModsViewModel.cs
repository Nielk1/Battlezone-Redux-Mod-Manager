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
                AllMods.Add(new ModData(GameId.Battlezone98Redux, $"testmod{i}")
                {
                    Title = $"Test Mod {i}",
                    Description = "This is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod this is a test mod",
                });
            }
            FilteredMods = new ObservableCollectionView<ModData>(AllMods);
        }
    }
}
