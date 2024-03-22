using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public enum TaskNodeState
    {
        None, // no state flags
        Waiting,
        Running,
        Delayed,
        Finished,
    }

    public partial class TaskNode : ObservableObject, IProgress<double?>, IObserver<ESteamCmdTaskStatus>
    {
        public string Text { get; }
        public IImage? ImageSource { get; }
        [ObservableProperty]
        public double? _percent;

        [ObservableProperty]
        public TaskNodeState _state;

        public TaskNode(string text, IImage? image, double? percent)
        {
            Text = text;
            ImageSource = image ?? ImageHelper.LoadFromResource(new Uri("avares://BZRModManager/Assets/modmanager.ico"));
            Percent = percent;
        }

        public void Report(double? value)
        {
            Percent = value;
        }


        public delegate void StatusReceived(ESteamCmdTaskStatus value);
        public event StatusReceived StatusReceivedEvent;
        void IObserver<ESteamCmdTaskStatus>.OnNext(ESteamCmdTaskStatus value)
        {
            StatusReceivedEvent?.Invoke(value);
        }

        // we are not a true IObserver, so no event for finalizing by removing all observers
        void IObserver<ESteamCmdTaskStatus>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<ESteamCmdTaskStatus>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
