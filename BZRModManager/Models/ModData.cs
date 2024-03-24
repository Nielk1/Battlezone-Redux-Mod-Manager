using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
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
        public GameId GameId { get; private set; }
        public string ModId { get; private set; }

        [ObservableProperty]
        public string _title;

        [ObservableProperty]
        private IImage? _image;

        private IonDriverMod? _ionDriverData;
        public IonDriverMod? IonDriverData
        {
            get
            {
                return _ionDriverData;
            }
            set
            {
                _ionDriverData = value;
                UpdateData();
                DecorateMedia();
            }
        }

        private WorkshopItemStatus? _workshopData;
        public WorkshopItemStatus? WorkshopData
        {
            get
            {
                return _workshopData;
            }
            set
            {
                _workshopData = value;
                UpdateData();
                DecorateMedia();
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


        private CancellationTokenSource DecorateCancelTokenSource;
        // look at adding debounce, store a property for what the image source is and if it doesn't change, leave Image alone
        // also consider dates, maybe include the last pulled date in said source string
        private async Task DecorateMediaInternal(CancellationToken token)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i > 0) await Task.Delay(1000);

                try
                {
                    // check existing files in cache and apply them or default
                    string localMetadata = Path.Combine("cache", "nielk1", GameId.ToString("D"), "mod", $"{ModId}.json");

                    if (token.IsCancellationRequested) return;

                    // local images only, no downloads
                    {
                        bool GotImage = false;
                        // load local nielk1 file if it exists
                        if (File.Exists(localMetadata))
                        {
                            IonDriverMod? ionDriverDataTmp = JsonConvert.DeserializeObject<IonDriverMod>(File.ReadAllText(localMetadata));
                            if (ionDriverDataTmp != null)
                            {
                                if (token.IsCancellationRequested) return;
                                IonDriverData = ionDriverDataTmp;
                                if (IonDriverData.Image != null)
                                {
                                    string localImage = Path.Combine("cache", "nielk1", GameId.ToString("D"), "mod", $"{ModId}{Path.GetExtension(IonDriverData.Image)}");
                                    IImage? image = await AssetCache.Instance.GetImageAsync(null, localImage);
                                    if (image != null)
                                    {
                                        GotImage = true;
                                        Image = image;

                                        //return; // if the date isn't too old
                                    }
                                }
                            }
                        }

                        if (token.IsCancellationRequested) return;
                        // we didn't get an image so load a default if none is set yet
                        if (!GotImage)
                        {
                            Image ??= await AssetCache.Instance.GetImageAsync(new Uri("avares://BZRModManager/Assets/modmanager.ico"), null);
                        }
                    }

                    if (token.IsCancellationRequested) return;
                    // wait a bit
                    await Task.Delay(1000);
                    if (token.IsCancellationRequested) return;

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
                        IonDriverMod localMetadataTemp = null;

                        // download nielk1 metadata and image as needed
                        if (!File.Exists(localMetadata) && !string.IsNullOrWhiteSpace(remoteMetadata))
                        {
                            // don't store local data since this can have more data in it than needed that we need to split up
                            string? rawJson = await AssetCache.Instance.GetData(remoteMetadata, null);
                            if (!string.IsNullOrWhiteSpace(rawJson))
                            {
                                //File.WriteAllText(localMetadata, rawJson);
                                IonDriverDataExtract? ionDriverDataTmp = JsonConvert.DeserializeObject<IonDriverDataExtract>(rawJson);
                                if (ionDriverDataTmp != null && ionDriverDataTmp.Mods != null)
                                {
                                    foreach (var pair in ionDriverDataTmp.Mods)
                                    {
                                        string localMetadataOtherMod = Path.Combine("cache", "nielk1", GameId.ToString("D"), "mod", $"{pair.Key}.json");

                                        if (!Directory.Exists(Path.GetDirectoryName(localMetadataOtherMod)))
                                            Directory.CreateDirectory(Path.GetDirectoryName(localMetadataOtherMod));

                                        if (token.IsCancellationRequested) return;
                                        if (pair.Key == ModId)// || !File.Exists(localMetadataOtherMod))
                                        {
                                            // save the mod data for whatever mod we got
                                            File.WriteAllText(localMetadataOtherMod, JsonConvert.SerializeObject(pair.Value));

                                            if (pair.Key == ModId)
                                            {
                                                localMetadataTemp = pair.Value;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (token.IsCancellationRequested) return;
                        // if we didn't already load localMetadataTemp from http, try loading it from file
                        if (localMetadataTemp == null && File.Exists(localMetadata))
                        {
                            localMetadataTemp = JsonConvert.DeserializeObject<IonDriverMod>(File.ReadAllText(localMetadata));
                        }

                        // if we have localMetadataTemp use it
                        if (localMetadataTemp != null)
                        {
                            IonDriverData = localMetadataTemp;

                            if (_ionDriverData.Image != null)
                            {
                                string? remoteAsset = $"https://gamelistassets.iondriver.com/{gameIdString}/{IonDriverData.Image}";

                                Uri uri = new Uri(remoteAsset);
                                string localImage = Path.Combine("cache", "nielk1", GameId.ToString("D"), "mod", $"{ModId}{Path.GetExtension(IonDriverData.Image)}");
                                if (token.IsCancellationRequested) return;
                                // check if local properties are changed or not (or maybe do a datetime check)
                                IImage? image = await AssetCache.Instance.GetImageAsync(uri, localImage, token);
                                if (image != null)
                                    Image = image;
                            }
                        }
                    }
                    return; // we got to the end, return
                }
                catch (System.IO.IOException ex)
                {

                }
            }
        }
        internal void DecorateMedia()
        {
            DecorateCancelTokenSource?.Cancel();
            DecorateCancelTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                await DecorateMediaInternal(DecorateCancelTokenSource.Token);
            }, DecorateCancelTokenSource.Token);
        }

        public ModData(GameId gameId, string modId)
        {
            GameId = gameId;
            ModId = modId;

            //UpdateImageList(null);

            _workshopData = null;
            //_imageSource = ("avares://BZRModManager/Assets/modmanager.ico", null);
            _image = ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
        }
    }
}
