using AngleSharp.Dom;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BZRModManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using MyToolkit.Collections;
using SteamVent.Common;
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
        public MtObservableCollection<ModData> AllMods { get; protected set; }
        /// <summary>
        /// Filtered view of mods
        /// </summary>
        public ObservableCollectionView<ModData> FilteredMods { get; protected set; }
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

        /// <summary>
        /// Filter definitions for filter button bar
        /// </summary>
        public ObservableCollectionView<ModFilter> Filters { get; private set; }

        [ObservableProperty]
        private string _filterString;

        partial void OnFilterStringChanged(string value)
        {
            UpdateFilter();
        }

        private bool _gameFilterBZ98R;
        public bool GameFilterBZ98R
        {
            get { return _gameFilterBZ98R; }
            set
            {
                if (SetProperty(ref _gameFilterBZ98R, value))
                {
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
                    UpdateFilter();
                    ApplyFilter();
                }
            }
        }

        private void UpdateFilter()
        {
            FilteredMods.Filter = (entry) =>
            {
                if (!_gameFilterBZ98R && entry.GameId == GameId.Battlezone98Redux)
                    return false;
                if (!_gameFilterBZCC && entry.GameId == GameId.BattlezoneComatCommander)
                    return false;

                bool filterStringMatch = string.IsNullOrWhiteSpace(FilterString) || FilterString.ToLowerInvariant().Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(set => set.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).All(dr => entry.Title.ToLowerInvariant().Contains(dr)));

                if (Filters.Count > 0)
                    return Filters.All((filter) =>
                    {
                        return filterStringMatch & (filter.Active.HasValue ? (filter.Active.Value ? filter.IsVisible(entry) : !filter.IsVisible(entry)) : true);
                    });

                return filterStringMatch;
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

                new ModFilter("Has Cloud Data", "Mod has enhanced data from Nielk1's IonDriver server.  Mods without this data may not filter properly or lack other features.", null, (entry) => {
                    return entry.IonDriverData != null;
                }),
                new ModFilter("Needs Update", "Mod needs to be updated.", null, (entry) => {
                    //return entry.WorkshopData?.HasUpdate ?? false;
                    return entry.HasUpdate;
                }),
                new ModFilter("Campaign", "Mod has campaign content.", null, (entry) => {
                    return (entry.IonDriverData?.IondriverTags?.Contains("campaign") ?? false) || (entry.ModType?.Contains(@"campaign") ?? false);
                }),
                new ModFilter("Addon", "Mod has non-mutually-exclusive activation.", null, (entry) => {
                    return (entry.ModType?.Contains(@"mod") ?? false) || (entry.ModType?.Contains(@"addon") ?? false);
                }),
                new ModFilter("SP Addon", "Addon is single player only and will be disabled in multiplayer.", null, (entry) => {
                    return entry.IonDriverData?.IondriverTags?.Contains("sp_addon") ?? false;
                }),
                new ModFilter("MP Addon", "Addon is multiplayer compatible and can remain active in multiplayer.", null, (entry) => {
                    return (entry.IonDriverData?.IondriverTags?.Contains("mp_addon") ?? false) || (entry.ModType?.Contains(@"addon") ?? false);
                }),
                new ModFilter("Shell", "Addon modifies the shell visuals.", null, (entry) => {
                    return entry.IonDriverData?.IondriverTags?.Contains("shell") ?? false;
                }),
                new ModFilter("Textures", "Addon has replacement textures.", null, (entry) => {
                    return entry.IonDriverData?.IondriverTags?.Contains("texture") ?? false;
                }),
                new ModFilter("Reticle", "Addon has replacement reticles.", null, (entry) => {
                    return entry.IonDriverData?.IondriverTags?.Contains("reticle") ?? false;
                }),
                new ModFilter("Sounds", "Addon has replacement sounds.", null, (entry) => {
                    return entry.IonDriverData?.IondriverTags?.Contains("sfx") ?? false;
                }),
                new ModFilter("Music", "Addon has replacement or additional music.", null, (entry) => {
                    return entry.IonDriverData?.IondriverTags?.Contains("music") ?? false;
                }),
                new ModFilter("Asset Package", "Mod is a shared asset package depended on by other mods.", null, (entry) => {
                    return entry.ModType?.Contains(@"asset") ?? false;
                }),
                new ModFilter("Config", "Mod is a mutually exclusive top-level mod. (BZCC Exclusive Mod Type)", null, (entry) => {
                    return entry.ModType?.Contains(@"config") ?? false;
                }),
                new ModFilter("Multiplayer", "Mod contains multiplayer maps.", null, (entry) => {
                    return (entry.IonDriverData?.IondriverTags?.Contains("multiplayer") ?? false) || (entry.ModType?.Contains(@"multiplayer") ?? false);
                }),
                new ModFilter("Instant Action", "Mod contains instant-action maps.", null, (entry) => {
                    return (entry.IonDriverData?.IondriverTags?.Contains("instant_action") ?? false) || (entry.ModType?.Contains(@"instant_action") ?? false);
                }),
            });

            Filters.PropertyChanged += (sender, e) => UpdateFilter();
            foreach(var filter in Filters)
            {
                filter.PropertyChanged += (sender, e) => UpdateFilter();
            }

            /*if (Design.IsDesignMode)
            {
                string key = $"{(int)GameId.Battlezone98Redux}:1";
                ModData value = new ModData(GameId.Battlezone98Redux, "1");
                ModsInternal[key] = value;
                AllMods.Add(value);

                //FilteredMods.IsTracking = true;
                //FilteredMods.IsTracking = false;

                UpdateFilter();
            }*/
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

        // unprotected, make sure you lock up before calling this
        private async Task<(ModData, SemaphoreSlim)> GetModItemAsync(uint appId, string workshopId)
        {
            await modsLock.WaitAsync();
            try
            {
                ModData? value;
                SemaphoreSlim valueLock;

                string key = $"{appId}:{workshopId}";
                if (!ModsInternal.TryGetValue(key, out value))
                {
                    value = new ModData((GameId)appId, workshopId);
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

                return (value, valueLock);
            }
            finally
            {
                modsLock.Release();
            }
        }

        public async Task AddInternalWorkshopModData(uint appId, List<WorkshopItemStatus> mods)
        {
            if (mods != null)
            {
                foreach (WorkshopItemStatus mod in mods)
                {
                    (ModData value, SemaphoreSlim valueLock) = await GetModItemAsync(appId, mod.WorkshopId.ToString());
                    await valueLock.WaitAsync();
                    try
                    {
                        value.InternalWorkshopData = mod;
                    }
                    finally
                    {
                        valueLock.Release();
                    }
                }
            }
        }

        public async Task AddExternalWorkshopModData(uint appId, List<WorkshopItemStatus> mods)
        {
            if (mods != null)
            {
                foreach (WorkshopItemStatus mod in mods)
                {
                    (ModData value, SemaphoreSlim valueLock) = await GetModItemAsync(appId, mod.WorkshopId.ToString());
                    await valueLock.WaitAsync();
                    try
                    {
                        value.ExternalWorkshopData = mod;
                    }
                    finally
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
        private string _toolTip;

        [ObservableProperty]
        private bool? _active;

        public Func<ModData, bool> IsVisible { get; private set; }

        public ModFilter(string text, string tip, bool? active, Func<ModData, bool> isVisible)
        {
            _text = text;
            _toolTip = tip;
            _active = active;
            IsVisible = isVisible;
        }
    }
}
