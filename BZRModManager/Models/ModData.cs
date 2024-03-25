using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis;
using MyToolkit.MVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public enum GameId
    {
        Battlezone98Redux = 301650,
        BattlezoneComatCommander = 624970,
    }
    public static class Dummy
    {
        public static IImage FallbackImage => ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
    }
    public partial class ModData : ObservableObject
    {
        public GameId GameId { get; private set; }
        public string ModId { get; private set; }

        [ObservableProperty]
        public string _title;

        public Task<IImage?> LiveImage => GetLiveImageAsync();
        private string _loadedImage; // the last image we loaded to avoid double-actions
        private IImage? _imageCache; // the last loaded image, might be null if we unloaded it for perf
        private SemaphoreSlim GetLiveImageLock = new SemaphoreSlim(0, 1);
        private SemaphoreSlim DecorateMetadataLock = new SemaphoreSlim(1, 1);
        private bool FirstRun = true; // used to make data cache update stop image update, but only once

        private bool _visible = true; // used to prevent image loading

        private IonDriverMod? _ionDriverData;
        public IonDriverMod? IonDriverData
        {
            get { return _ionDriverData; }
            set
            {
                if (SetProperty(ref _ionDriverData, value))
                {
                    DownloadMetadata();
                    UpdateData();
                }
            }
        }

        private WorkshopItemStatus? _workshopData;
        public WorkshopItemStatus? WorkshopData
        {
            get { return _workshopData; }
            set
            {
                if (SetProperty(ref _workshopData, value))
                {
                    DownloadMetadata();
                    UpdateData();
                }
            }
        }
        // this is called by property setters and thus could be triggered by DecorateMedia downloading new metadata
        private void UpdateData()
        {
            Title = IonDriverData?.WorkshopName
                 ?? IonDriverData?.Name
                 ?? WorkshopData?.Title
                 ?? WorkshopData?.WorkshopId.ToString()
                 ?? ModId;
        }

        /// <summary>
        /// Get the mod's image, might be locally stored, might need to download it
        /// </summary>
        /// <returns></returns>
        private async Task<IImage?> GetLiveImageAsync()
        {
            await GetLiveImageLock.WaitAsync();
            try
            {
                if (!_visible)
                    return null;
                if (IonDriverData != null)
                {
                    if (IonDriverData?.Image != null)
                    {
                        string? gameIdString = null;
                        switch (GameId)
                        {
                            case GameId.Battlezone98Redux:
                                gameIdString = "bz98r";
                                break;
                            case GameId.BattlezoneComatCommander:
                                gameIdString = "bzcc";
                                break;
                        }

                        if (gameIdString != null)
                        {
                            string localImage = Path.Combine("cache", "nielk1", GameId.ToString("D"), "mod", $"{ModId}{Path.GetExtension(IonDriverData.Image)}");

                            // if the image is already set, just spit out the existing image (might do a date check to reload)
                            if (_imageCache != null && _loadedImage == localImage)
                            {
                                return _imageCache;
                            }

                            string? remoteAsset = $"https://gamelistassets.iondriver.com/{gameIdString}/{IonDriverData.Image}";
                            Uri uri = new Uri(remoteAsset);

                            // check if local properties are changed or not (or maybe do a datetime check)
                            IImage? image = await AssetCache.Instance.GetImageAsync(uri, localImage);
                            if (image != null)
                            {
                                _loadedImage = localImage;
                                _imageCache = image;
                                return _imageCache;
                            }

                        }
                    }
                }
                return null;
            }
            finally
            {
                GetLiveImageLock.Release(); // make sure we always unlock
            }
        }

        private CancellationTokenSource DecorateCancelTokenSource;
        // TODO consider dates, maybe include the last pulled date in said source string
        private async Task DownloadMetadataInternal(CancellationToken? token = null)
        {
            await DecorateMetadataLock.WaitAsync();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i > 0) await Task.Delay(1000);

                    try
                    {
                        // check existing files in cache and apply them or default
                        string localMetadata = Path.Combine("cache", "nielk1", GameId.ToString("D"), "mod", $"{ModId}.json");

                        string gameIdString = null;
                        switch (GameId)
                        {
                            case GameId.Battlezone98Redux:
                                gameIdString = "bz98r";
                                break;
                            case GameId.BattlezoneComatCommander:
                                gameIdString = "bzcc";
                                break;
                        }

                        string? remoteMetadata = gameIdString != null ? $"https://gamelistassets.iondriver.com/{gameIdString}/getdata.php?mods={ModId}" : null;

                        // download images now
                        {
                            // load nielk1 metadata, download if not found
                            // TODO add marker for 404s or something, timestamp can help here too for if it comes to exist
                            if (!File.Exists(localMetadata) && !string.IsNullOrWhiteSpace(remoteMetadata))
                            {
                                // we might get multiple mods worth of data so we need to find our mod, save everything to cache since we got it
                                string? rawJson = await AssetCache.Instance.GetData(remoteMetadata, null);
                                if (!string.IsNullOrWhiteSpace(rawJson))
                                {
                                    IonDriverDataExtract? ionDriverDataTmp = JsonConvert.DeserializeObject<IonDriverDataExtract>(rawJson);
                                    if (ionDriverDataTmp != null && ionDriverDataTmp.Mods != null)
                                    {
                                        foreach (var pair in ionDriverDataTmp.Mods)
                                        {
                                            string localMetadataOtherMod = Path.Combine("cache", "nielk1", GameId.ToString("D"), "mod", $"{pair.Key}.json");

                                            if (!Directory.Exists(Path.GetDirectoryName(localMetadataOtherMod)))
                                                Directory.CreateDirectory(Path.GetDirectoryName(localMetadataOtherMod));

                                            if (token?.IsCancellationRequested ?? false) return;
                                            if (pair.Key == ModId || !File.Exists(localMetadataOtherMod))
                                            {
                                                // save the mod data for whatever mod we got
                                                File.WriteAllText(localMetadataOtherMod, JsonConvert.SerializeObject(pair.Value));

                                                if (pair.Key == ModId)
                                                {
                                                    IonDriverData = pair.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                IonDriverData = JsonConvert.DeserializeObject<IonDriverMod>(File.ReadAllText(localMetadata));
                            }
                        }
                        return; // we got to the end, return
                    }
                    catch (System.IO.IOException ex)
                    {

                    }
                }
            }
            finally
            {
                if (FirstRun)
                {
                    FirstRun = false;
                    GetLiveImageLock.Release();
                }
                DecorateMetadataLock.Release();
            }
        }
        internal void DownloadMetadata()
        {
            DecorateCancelTokenSource?.Cancel();
            DecorateCancelTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                await DownloadMetadataInternal(DecorateCancelTokenSource.Token);
            }, DecorateCancelTokenSource.Token);
        }

        internal void UpdateVisibility(bool inViewport)
        {
            if (_visible != inViewport)
            {
                OnPropertyChanging(nameof(LiveImage));
                _visible = inViewport;
                if (!_visible)
                {
                    _imageCache = null;
                }
                OnPropertyChanged(nameof(LiveImage));
            }
        }

        public ModData() : this((GameId)0, "0")
        {
        }
        public ModData(GameId gameId, string modId)
        {
            GameId = gameId;
            ModId = modId;

            _ionDriverData = null;
            _workshopData = null;

            DownloadMetadata();
            UpdateData();
        }
    }
}
