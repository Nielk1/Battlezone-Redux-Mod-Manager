using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
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

        public string Name
        {
            get
            {
                return InternalGetName(_workshopData);
            }
        }
        private string InternalGetName(WorkshopItemStatus workshopData)
        {
            return workshopData?.Title ?? workshopData?.WorkshopId.ToString() ?? ModId;
        }

        [ObservableProperty]
        private IImage? _image;


        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(ImageUpdatedCommand))]
        private (string?, string?) _imageSource;


        private CancellationTokenSource ImageChangeCancelTokenSource;
        //private async Task ImageUpdatedCommand()
        partial void OnImagesListChanged(List<(string url, string cacheKey)> value)
        {
            if (value != null)
            {
                ImageChangeCancelTokenSource?.Cancel();
                ImageChangeCancelTokenSource = new CancellationTokenSource();
                Task.Run(async () =>
                {
                    var data = await ImageCache.Instance.GetImageAsync((_imageSource.Item1, _imageSource.Item2), value);
                    if (data.image != null)
                    {
                        Image = data.image;
                        ImageSource = (data.url, data.cacheKey); // if it doesn't change it shouldn't trigger, we hope
                    }
                }, ImageChangeCancelTokenSource.Token).Wait();
            }
        }

        private List<(string url, string cacheKey)> UpdateImageList(WorkshopItemStatus newWorkshopItemStatus)
        {
            List<(string url, string cacheKey)> newImagesList = new List<(string url, string cacheKey)>();

            {
                string? url = newWorkshopItemStatus?.Image;
                if (!string.IsNullOrWhiteSpace(url))
                    newImagesList.Add((url, @$"steam\{(uint)GameId}\mod-{ModId}"));
            }
            newImagesList.Add(("avares://BZRModManager/Assets/modmanager.ico", null));

            if (ImagesList == null || !ImagesList.SequenceEqual(newImagesList))
                return newImagesList;
            return null;
        }


        private WorkshopItemStatus _workshopData;
        [ObservableProperty]
        private List<(string url, string cacheKey)> _imagesList;

        public WorkshopItemStatus WorkshopData
        {
            get
            {
                return _workshopData;
            }
            set
            {
                WorkshopItemStatus _workshopDataOld = _workshopData;

                string _nameNew = InternalGetName(value);
                bool _nameNewBool = Name != _nameNew;
                if (_nameNewBool) OnPropertyChanging(nameof(Name));

                List<(string url, string cacheKey)>  newImagesList = UpdateImageList(value);
                if(newImagesList != null) OnPropertyChanging(nameof(ImagesList));

                _workshopData = value;
                if (_nameNewBool) OnPropertyChanged(nameof(Name));

                ImagesList = newImagesList;
                if (newImagesList != null) OnPropertyChanged(nameof(ImagesList));
            }
        }

        public ModData(GameId gameId, string modId)
        {
            GameId = gameId;
            ModId = modId;

            UpdateImageList(null);

            _workshopData = null;
            _imageSource = ("avares://BZRModManager/Assets/modmanager.ico", null);
            _image = ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
        }
    }
}
