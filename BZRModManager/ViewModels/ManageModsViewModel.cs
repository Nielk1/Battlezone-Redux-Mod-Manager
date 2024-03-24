using Avalonia.Collections;
using BZRModManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using MyToolkit.Collections;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class ManageModsViewModel : ViewModelBase
    {
        public MtObservableCollection<ModData> AllMods { get; private set; }

        public ObservableCollectionView<ModData> FilteredMods { get; private set; }
        public Dictionary<string, ModData> ModsInternal { get; private set; }
        private SemaphoreSlim modsLock;
        private Dictionary<string, SemaphoreSlim> modsLocks;


        private GameId? _gameFilter = null;
        public GameId? GameFilter
        {
            get { return _gameFilter; }
            set
            {
                if (SetProperty(ref _gameFilter, value))
                {
                    FilteredMods.Filter = entry => !_gameFilter.HasValue || entry.GameId == _gameFilter;
                }
            }
        }

        public ManageModsViewModel()
        {
            ModsInternal = new Dictionary<string, ModData>();
            AllMods = new MtObservableCollection<ModData>();
            FilteredMods = new ObservableCollectionView<ModData>(AllMods);
            modsLock = new SemaphoreSlim(1, 1);
            modsLocks = new Dictionary<string, SemaphoreSlim>();
            FilteredMods.Order = entry => entry.Title;
            //FilteredMods.Ascending = false;
        }

        public async Task AddWorkshopModData(uint appId, List<WorkshopItemStatus> mods)
        {
            if (mods != null)
            {
                foreach (WorkshopItemStatus mod in mods)
                {
                    ModData value;
                    SemaphoreSlim valueLock;
                    await modsLock.WaitAsync();
                    try
                    {
                        string key = $"{appId}:{mod.WorkshopId}";
                        if (!ModsInternal.TryGetValue(key, out value))
                        {
                            value = new ModData((GameId)appId, mod.WorkshopId.ToString());
                            ModsInternal[key] = value;
                            valueLock = modsLocks[key] = new SemaphoreSlim(1, 1);
                            AllMods.Add(value);
                        }
                        else
                        {
                            valueLock = modsLocks[key];
                        }
                        value.DecorateMedia();
                    }
                    finally
                    {
                        modsLock.Release();
                    }
                    await valueLock.WaitAsync();
                    try
                    {
                        value.WorkshopData = mod;
                    }
                    catch
                    {
                        valueLock.Release();
                    }
                }
            }
        }
    }
}
