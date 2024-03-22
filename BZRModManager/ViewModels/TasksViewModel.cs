using Avalonia;
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

        public bool IsEmpty
        {
            get => Tasks.Count == 0;
        }

        public TasksViewModel()
        {
            Tasks = new ObservableCollection<TaskNode>(/*new List<TaskNode> {
                new TaskNode("Task 0", null, 0.000d),
                new TaskNode("Task 1", null, 0.125d),
                new TaskNode("Task 2", null, 0.250d),
                new TaskNode("Task 3", null, 0.375d),
                new TaskNode("Task 4", null, 0.500d),
                new TaskNode("Task 5", null, 0.625d),
                new TaskNode("Task 6", null, 0.750d),
                new TaskNode("Task 7", null, 0.875d),
                new TaskNode("Task 8", null, 1.000d),
                new TaskNode("Task 9", null, null),
            }*/);
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
            }
            finally
            {
                TasksLock.Release();
            }
        }
    }
}
