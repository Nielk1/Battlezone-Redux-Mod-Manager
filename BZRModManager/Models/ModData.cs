using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    public partial class ModData : ObservableObject
    {
        public static IImage FallbackImage => ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
        public GameId GameId { get; private set; }
        public string ModId { get; private set; }

        [ObservableProperty]
        public string _title;

        [ObservableProperty]
        private IImage? _image;

        private string _loadedImage; // the last image we loaded to avoid double-actions
        private SemaphoreSlim UpdateImageLock = new SemaphoreSlim(0, 1);
        private SemaphoreSlim DecorateMetadataLock = new SemaphoreSlim(1, 1);
        private bool FirstRun = true; // used to make data cache update stop image update, but only once

        [ObservableProperty]
        private bool _visibleInViewport = true; // used to prevent image loading

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
        private async void UpdateData()
        {
            Title = IonDriverData?.WorkshopName
                 ?? IonDriverData?.Name
                 ?? WorkshopData?.Title
                 ?? WorkshopData?.WorkshopId.ToString()
                 ?? ModId;
            await UpdateImageAsync();
        }

        private async Task UpdateImageAsync()
        {
            await UpdateImageLock.WaitAsync();
            try
            {
                if (!_visibleInViewport)
                {
                    Image = null;
                    return;
                }
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

                            if (_loadedImage == localImage && Image != null)
                                return; // we already have the correct image loaded

                            string? remoteAsset = $"https://gamelistassets.iondriver.com/{gameIdString}/{IonDriverData.Image}";
                            Uri uri = new Uri(remoteAsset);

                            // TODO Datetime check should be added to prevent spurious web requests
                            IImage? image = await AssetCache.Instance.GetImageAsync(uri, localImage);//, new Point(256, 256));

                            if (image != null)
                            {
                                _loadedImage = localImage;
                                Image = image;
                            }
                        }
                    }
                }
            }
            finally
            {
                UpdateImageLock.Release(); // make sure we always unlock
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
                            if ((!File.Exists(localMetadata) || new FileInfo(localMetadata).CreationTimeUtc.AddDays(1) < DateTime.UtcNow) && !string.IsNullOrWhiteSpace(remoteMetadata))
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
                    UpdateImageLock.Release();
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
            /*if (VisibleInViewport != inViewport)
            {
                OnPropertyChanging(nameof(Image));
                VisibleInViewport = inViewport;
                if (!VisibleInViewport)
                {
                    Image = null;
                }
                //else if (Image == null)
                //{
                //    await UpdateImageAsync();
                //}
                OnPropertyChanged(nameof(Image));
            }*/
        }

        public ModData(GameId gameId, string modId)
        {
            GameId = gameId;
            ModId = modId;

            _loadedImage = null;
            _ionDriverData = null;
            _workshopData = null;
            _image = null;

            _title = ModId;

            //this.WhenPropertyChanged(md => md.VisibleInViewport)
            //    .Subscribe(async _ =>
            //    {
            //        await UpdateImageAsync().ConfigureAwait(false);
            //    });

            DownloadMetadata();
            UpdateData();
        }
    }
}
