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


        private bool _gameFilterBZ98R;
        public bool GameFilterBZ98R
        {
            get { return _gameFilterBZ98R; }
            set
            {
                if (SetProperty(ref _gameFilterBZ98R, value))
                {
                    //if (!value && !GameFilterBZCC)
                    //    GameFilterBZCC = true;
                    UpdateFilter();
                    ApplyFilter();
                }
            }
        }
        private bool _gameFilterBZCC;
        public bool GameFilterBZCC
        {
            get { return _gameFilterBZCC; }
            set
            {
                if (SetProperty(ref _gameFilterBZCC, value))
                {
                    //if (!value && !GameFilterBZ98R)
                    //    GameFilterBZ98R = true;
                    UpdateFilter();
                    ApplyFilter();
                }
            }
        }

        public ObservableCollectionView<ModFilter> Filters { get; private set; }


        private void UpdateFilter()
        {
            FilteredMods.Filter = (entry) =>
            {
                if (!_gameFilterBZ98R && entry.GameId == GameId.Battlezone98Redux)
                    return false;
                if (!_gameFilterBZCC && entry.GameId == GameId.BattlezoneComatCommander)
                    return false;

                if (Filters.Count > 0)
                    return Filters.All((filter) =>
                    {
                        return filter.Active.HasValue ? (filter.Active.Value ? filter.IsVisible(entry) : !filter.IsVisible(entry)) : true;
                    });

                return true;
            };
            ApplyFilter();
        }

        [ObservableProperty]
        private bool _isBusy;

        public ManageModsViewModel()
        {
            _gameFilterBZ98R = true;
            _gameFilterBZCC = true;
            ModsInternal = new Dictionary<string, ModData>();
            AllMods = new MtObservableCollection<ModData>();
            FilteredMods = new ObservableCollectionView<ModData>(AllMods);
            FilteredMods.IsTracking = false;
            modsLock = new SemaphoreSlim(1, 1);
            modsLocks = new Dictionary<string, SemaphoreSlim>();
            FilteredMods.Order = entry => entry.Title;
            //FilteredMods.Ascending = false;
            Filters = new ObservableCollectionView<ModFilter>(new List<ModFilter>
            {
                // AI made this list, kinda amazing, not what we wanted but perfect examples
                /*new ModFilter("Battlezone 98 Redux", true, (entry) => entry.GameId == GameId.Battlezone98Redux),
                new ModFilter("Battlezone Combat Commander", true, (entry) => entry.GameId == GameId.BattlezoneComatCommander),
                new ModFilter("Active", true, (entry) => entry.Active),
                new ModFilter("Inactive", true, (entry) => !entry.Active),
                new ModFilter("Installed", true, (entry) => entry.Installed),
                new ModFilter("Not Installed", true, (entry) => !entry.Installed),
                new ModFilter("Update Available", true, (entry) => entry.UpdateAvailable),
                new ModFilter("No Update Available", true, (entry) => !entry.UpdateAvailable),
                new ModFilter("Downloading", true, (entry) => entry.Downloading),
                new ModFilter("Not Downloading", true, (entry) => !entry.Downloading),
                new ModFilter("Downloaded", true, (entry) => entry.Downloaded),
                new ModFilter("Not Downloaded", true, (entry) => !entry.Downloaded),
                new ModFilter("Installed", true, (entry) => entry.Installed),
                new ModFilter("Not Installed", true, (entry) => !entry.Installed),
                new ModFilter("Enabled", true, (entry) => entry.Enabled),
                new ModFilter("Disabled", true, (entry) => !entry.Enabled),
                new ModFilter("Has Workshop Data", true, (entry) => entry.WorkshopData != null),
                new ModFilter("No Workshop Data", true, (entry) => entry.WorkshopData == null),
                new ModFilter("Has Metadata", true, (entry) => entry.Metadata != null),
                new ModFilter("No Metadata", true, (entry) => entry.Metadata == null),*/

                new ModFilter("Has Cloud Data", null, (entry) => {
                    return entry.IonDriverData != null;
                }),
                new ModFilter("Needs Update", null, (entry) => {
                    return entry.WorkshopData?.HasUpdate ?? false;
                }),
            });

            Filters.PropertyChanged += (sender, e) => UpdateFilter();
            foreach(var filter in Filters)
            {
                filter.PropertyChanged += (sender, e) => UpdateFilter();
            }
        }

        private CancellationTokenSource? filterDebounceCancellationToken;
        private void ApplyFilter()
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
                            value.PropertyChanged += (sender, e) => ApplyFilter();
                            ApplyFilter();
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

    public partial class ModFilter : ObservableObject
    {
        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private bool? _active;

        public Func<ModData, bool> IsVisible { get; private set; }

        public ModFilter(string text, bool? active, Func<ModData, bool> isVisible)
        {
            _text = text;
            _active = active;
            IsVisible = isVisible;
        }
    }
}
