using BZRModManager.Models;
using MyToolkit.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ViewModels
{
    public partial class DesignGetModsViewModel : GetModsViewModel
    {
        public DesignGetModsViewModel() : base()
        {
            GitBranches = new ObservableCollection<string>(new string[] { "master", "dev" });
            for (int i = 0; i < 10; i++)
            {
                GitBranches.Add($"branch{i}");
            }
        }
    }
}
