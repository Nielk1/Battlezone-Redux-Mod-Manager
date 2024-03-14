using BZRModManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class TasksViewModel : ViewModelBase
    {
        public ObservableCollection<TaskNode> Tasks { get; set; }

        public TasksViewModel()
        {
            Tasks = new ObservableCollection<TaskNode>(new List<TaskNode> {
                new TaskNode("Task 0", null, 0d),
                new TaskNode("Task 1", null, 12.5d),
                new TaskNode("Task 2", null, 25.0d),
                new TaskNode("Task 3", null, 37.5d),
                new TaskNode("Task 4", null, 50.0d),
                new TaskNode("Task 5", null, 62.5d),
                new TaskNode("Task 6", null, 75.0d),
                new TaskNode("Task 7", null, 87.5d),
                new TaskNode("Task 8", null, 100d),
                new TaskNode("Task 9", null, null),
            });
        }
    }
}
