using Avalonia.Collections;
using BZRModManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using MyToolkit.Collections;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class ManageModsViewModel : ViewModelBase
    {
        /// <summary>
        /// Observable collection of all mods
        /// </summary>
        public MtObservableCollection<ModData> AllMods { get; private set; }
        /// <summary>
        /// Filtered view of mods
        /// </summary>
        public ObservableCollectionView<ModData> FilteredMods { get; private set; }
        /// <summary>
        /// Dictionary to prevent remaking mods that are already in the list
        /// </summary>
        private Dictionary<string, ModData> ModsInternal { get; set; }
        /// <summary>
        /// Lock when modifying mods collections
        /// </summary>
        private SemaphoreSlim modsLock;
        /// <summary>
        /// Locks for individual mods
        /// </summary>
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
                    UpdateFilter();
                }
            }
        }

        [ObservableProperty]
        private bool _isBusy;

        public ManageModsViewModel()
        {
            ModsInternal = new Dictionary<string, ModData>();
            AllMods = new MtObservableCollection<ModData>();
            FilteredMods = new ObservableCollectionView<ModData>(AllMods);
            FilteredMods.IsTracking = false;
            modsLock = new SemaphoreSlim(1, 1);
            modsLocks = new Dictionary<string, SemaphoreSlim>();
            FilteredMods.Order = entry => entry.Title;
            //FilteredMods.Ascending = false;
        }

        private CancellationTokenSource filterDebounceCancellationToken;
        private void UpdateFilter()
        {
            IsBusy = true;
            filterDebounceCancellationToken?.Cancel();
            filterDebounceCancellationToken = new CancellationTokenSource();
            Task.Run(async () =>
            {
                CancellationToken tok = filterDebounceCancellationToken.Token;
                /*try
                {
                    await Task.Delay(1000, tok);
                }
                catch (System.Threading.Tasks.TaskCanceledException) { }*/
                await Task.Delay(100);
                if (tok.IsCancellationRequested)
                    return;
                FilteredMods.IsTracking = true;
                FilteredMods.IsTracking = false;
                IsBusy = false;
            }, filterDebounceCancellationToken.Token);
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
                            value.PropertyChanged += (sender, e) => UpdateFilter();
                            UpdateFilter();
                        }
                        else
                        {
                            valueLock = modsLocks[key];
                        }
                        value.DownloadMetadata();
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
