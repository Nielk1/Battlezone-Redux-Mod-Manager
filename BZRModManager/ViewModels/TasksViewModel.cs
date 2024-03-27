using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using BZRModManager.Controls;
using BZRModManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public delegate void TaskNodeDelegate(TaskNode taskNode);
    public delegate Task TaskNodeDelegateAsync(TaskNode taskNode);
    public partial class TasksViewModel : ViewModelBase
    {
        public ObservableCollection<TaskNode> Tasks { get; set; }
        private SemaphoreSlim TasksLock = new SemaphoreSlim(1, 1);

        public int TaskCount => Design.IsDesignMode ? 99 : Tasks.Where(x => x.State != TaskNodeState.Finished).Count();

        public bool IsEmpty => Tasks.Count == 0;

        public TasksViewModel()
        {
            Tasks = new ObservableCollection<TaskNode>();
            Tasks.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(new PropertyChangedEventArgs("IsEmpty"));
                OnPropertyChanged(new PropertyChangedEventArgs("TaskCount"));
            };  
        }

        public Task RegisterTask(string name, IImage? image, double? percent, TaskNodeDelegateAsync value)
        {
            TaskNode taskNode = new TaskNode(name, image, percent);
            TasksLock.Wait();
            try
            {
                Tasks.Add(taskNode);
            }
            finally
            {
                TasksLock.Release();
            }
            return Task.Run(async () =>
            {
                taskNode.State = TaskNodeState.Waiting;
                await value.Invoke(taskNode);
                taskNode.State = TaskNodeState.Finished;
                if (!taskNode.Percent.HasValue)
                    taskNode.Percent = 1;
                OnPropertyChanged(new PropertyChangedEventArgs("TaskCount")); // because the count didn't actually change, a sub-property did
            });
        }

        public Task RegisterTask(string name, IImage? image, double? percent, TaskNodeDelegate value)
        {
            TaskNode taskNode = new TaskNode(name, image, percent);
            TasksLock.Wait();
            try
            {
                Tasks.Add(taskNode);
            }
            finally
            {
                TasksLock.Release();
            }
            Task t = Task.Run(() =>
            {
                taskNode.State = TaskNodeState.Waiting;
                value.Invoke(taskNode);
                taskNode.State = TaskNodeState.Finished;
                if (!taskNode.Percent.HasValue)
                    taskNode.Percent = 1;
                OnPropertyChanged(new PropertyChangedEventArgs("TaskCount")); // because the count didn't actually change, a sub-property did
            });
            return t;
        }

        public async Task ClearFinishedTasks()
        {
            await TasksLock.WaitAsync();
            try
            {
                Tasks.RemoveMany(Tasks.Where(x => x.State == TaskNodeState.Finished));
                OnPropertyChanged(new PropertyChangedEventArgs("IsEmpty"));
                OnPropertyChanged(new PropertyChangedEventArgs("TaskCount"));
            }
            finally
            {
                TasksLock.Release();
            }
        }
    }
}
