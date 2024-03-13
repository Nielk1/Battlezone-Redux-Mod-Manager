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
        //public ObservableCollection<TaskNode> Tasks { get; }
        //public ObservableCollection<TaskNode> SelectedTasks { get; }

        public TasksViewModel()
        {
            //SelectedTasks = new ObservableCollection<TaskNode>();
            //Tasks = new ObservableCollection<TaskNode>
            //{
            //    new TaskNode("Animals", new ObservableCollection<TaskNode>
            //    {
            //        new TaskNode("Mammals", new ObservableCollection<TaskNode>
            //        {
            //            new TaskNode("Lion"), new TaskNode("Cat"), new TaskNode("Zebra")
            //        })
            //    }),
            //    new TaskNode("Birds", new ObservableCollection<TaskNode>
            //    {
            //        new TaskNode("Robin"), new TaskNode("Condor"),
            //        new TaskNode("Parrot"), new TaskNode("Eagle")
            //    }),
            //    new TaskNode("Insects", new ObservableCollection<TaskNode>
            //    {
            //        new TaskNode("Locust"), new TaskNode("House Fly"),
            //        new TaskNode("Butterfly"), new TaskNode("Moth")
            //    }),
            //};
            //
            //var moth = Tasks.Last().SubTasks?.Last();
            //if (moth != null) SelectedTasks.Add(moth);
        }
    }
}
