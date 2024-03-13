using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Models
{
    public class TaskNode
    {
        public ObservableCollection<TaskNode>? SubTasks { get; }
        public string Title { get; }

        public TaskNode(string title)
        {
            Title = title;
        }

        public TaskNode(string title, ObservableCollection<TaskNode> subTasks)
        {
            Title = title;
            SubTasks = subTasks;
        }
    }
}
