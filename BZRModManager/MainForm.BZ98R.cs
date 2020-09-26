using BZRModManager.ModItem;
using SteamVent.SteamCmd;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    public partial class MainForm
    {
        Task UpdateBZ98RModListsTask = null;
        //TaskControl UpdateBZ98RModListsTaskControl = null;
        private void UpdateBZ98RModLists()
        {
            if (UpdateBZ98RModListsTask == null
              || UpdateBZ98RModListsTask.IsCanceled
              || UpdateBZ98RModListsTask.IsCompleted
              || UpdateBZ98RModListsTask.IsFaulted)
            {
                //EndTask(UpdateBZ98RModListsTaskControl);
                TaskControl UpdateBZ98RModListsTaskControl = AddTask("Update BZ98 Mod List", 0);
                UpdateBZ98RModListsTask = Task.Factory.StartNew(() =>
                {
                    lock (ModStatus)
                    {
                        Semaphore loadSemaphore = new Semaphore(0, 2);
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                TaskControl UpdateTask = UpdateBZ98RModListsTaskControl.AddTask("Update BZ98 Mod List (SteamCmd)", 0);
                                List<WorkshopItemStatus> stats = SteamCmd.WorkshopStatus(AppIdBZ98);
                                stats?.ForEach(dr =>
                                {
                                    string ModId = SteamCmdMod.GetUniqueId(dr.WorkshopId);
                                    if (!Mods[AppIdBZ98].ContainsKey(ModId))
                                    {
                                        //var modTmp = new SteamCmdMod(AppIdBZ98, dr);
                                        //if (modTmp.Exists())
                                        //    Mods[AppIdBZ98][ModId] = modTmp;
                                        Mods[AppIdBZ98][ModId] = new SteamCmdMod(AppIdBZ98, dr);
                                    }
                                    else
                                    {
                                        ((SteamCmdMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
                                    }
                                    Mods[AppIdBZ98][ModId].HasUpdate = dr.HasUpdate;
                                    Mods[AppIdBZ98][ModId].FolderOnlyDetection = dr.FolderOnlyDetection;
                                });
                                UpdateBZ98RModListsTaskControl.EndTask(UpdateTask);
                            }
                            finally
                            {
                                loadSemaphore.Release();
                            }
                        });

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                TaskControl UpdateTask = UpdateBZ98RModListsTaskControl.AddTask("Update BZ98 Mod List (Git)", 0);
                                List<GitModStatus> stats = GitContext.WorkshopItemsOnDrive(AppIdBZ98);
                                stats.ForEach(dr =>
                                {
                                    string ModId = GitMod.GetUniqueId(dr.ModWorkshopId);
                                    if (!Mods[AppIdBZ98].ContainsKey(ModId))
                                    {
                                        Mods[AppIdBZ98][ModId] = new GitMod(AppIdBZ98, dr);
                                    }
                                    else
                                    {
                                        ((GitMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
                                    }
                                });
                                UpdateBZ98RModListsTaskControl.EndTask(UpdateTask);
                            }
                            finally
                            {
                                loadSemaphore.Release();
                            }
                        });

                        if (settings.BZ98RSteamPath != null)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    TaskControl UpdateTask = UpdateBZ98RModListsTaskControl.AddTask("Update BZ98 Mod List (Steam)", 0);
                                    SteamContext.WorkshopItemsOnDrive(settings.BZ98RSteamPath, AppIdBZ98)?.ForEach(dr =>
                                    {
                                        string ModId = SteamMod.GetUniqueId(dr);
                                        if (!Mods[AppIdBZ98].ContainsKey(ModId))
                                        {
                                            Mods[AppIdBZ98][ModId] = new SteamMod(AppIdBZ98, dr);
                                        }
                                    });
                                    UpdateBZ98RModListsTaskControl.EndTask(UpdateTask);
                                }
                                finally
                                {
                                    loadSemaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            loadSemaphore.Release();
                        }

                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();

                        lock (Mods[AppIdBZ98])
                            this.Invoke((MethodInvoker)delegate
                            {
                                lvModsBZ98R.BeginUpdate();
                                Mods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                lvModsBZ98R.DataSource = Mods[AppIdBZ98].Values.ToList<ILinqListViewItemMods>();
                                lvModsBZ98R.EndUpdate();

                                //lock (Mods[AppIdBZ98])
                                {
                                    //lock (FoundMods[AppIdBZ98]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                                    {
                                        foreach (var kv in Mods[AppIdBZ98])
                                        {
                                            if (FoundMods[AppIdBZ98].ContainsKey(kv.Key))
                                                FoundMods[AppIdBZ98][kv.Key].Known = true;
                                        }
                                        lvFindModsBZ98R.BeginUpdate();
                                        FoundMods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                        lvFindModsBZ98R.DataSource = FoundMods[AppIdBZ98].Values.ToList<ILinqListViewFindModsItem>();
                                        lvFindModsBZ98R.EndUpdate();
                                    }
                                }

                                EndTask(UpdateBZ98RModListsTaskControl);
                            });
                    }
                });
            }
        }

        Task UpdateBZ98RModsTask = null;
        private void UpdateBZ98RMods(bool agressive)
        {
            if (UpdateBZ98RModsTask == null
              || UpdateBZ98RModsTask.IsCanceled
              || UpdateBZ98RModsTask.IsCompleted
              || UpdateBZ98RModsTask.IsFaulted)
            {
                UpdateBZ98RModsTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Update BZ98 Mods", 0);
                    lock (Mods[AppIdBZ98])
                    {
                        List<KeyValuePair<string, ModItemBase>> ModList = Mods[AppIdBZ98].ToList();
                        UpdateTaskControl.Maximum = ModList.Count;
                        object CounterClock = new object();
                        int Counter = 0;
                        List<KeyValuePair<string, ModItemBase>> NoUpdateMods = ModList.Where(dr => !(dr.Value is SteamCmdMod) && !(dr.Value is GitMod)).ToList();
                        List<KeyValuePair<string, ModItemBase>> SteamCmdMods = ModList.Where(dr => (dr.Value is SteamCmdMod)).ToList();
                        List<KeyValuePair<string, ModItemBase>> GitMods = ModList.Where(dr => (dr.Value is GitMod)).ToList();
                        NoUpdateMods.ForEach(dr =>
                        {
                            UpdateTaskControl.Value = ++Counter;
                        });
                        Semaphore MergeTasks = new Semaphore(0, 1);
                        new Thread(() =>
                        {
                            try
                            {
                                SteamCmdMods.ForEach(dr =>
                                {
                                    SteamCmdMod modSteam = dr.Value as SteamCmdMod;
                                    if (agressive || (modSteam?.HasUpdate ?? false) || (modSteam?.FolderOnlyDetection ?? false))
                                    {
                                        if (modSteam != null)
                                        {
                                            TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZ98 Mod - SteamCmd - {modSteam.Workshop.WorkshopId} - {modSteam.Name}", 0);
                                            SteamCmdException ex_ = null;
                                            int OtherErrorCounter = 0;
                                            do
                                            {
                                                ex_ = null;
                                                try
                                                {
                                                    SteamCmd.WorkshopDownloadItem(AppIdBZ98, modSteam.Workshop.WorkshopId);
                                                }
                                                catch (SteamCmdWorkshopDownloadException ex)
                                                {
                                                    ex_ = ex;
                                                    if (!ex_.Message.StartsWith("ERROR! Timeout downloading item "))
                                                        OtherErrorCounter++;
                                                }
                                                catch (SteamCmdException ex)
                                                {
                                                    ex_ = ex;
                                                    OtherErrorCounter++;
                                                }
                                            } while (ex_ != null && OtherErrorCounter < MAX_OTHER_STEAMCMD_ERROR);
                                            UpdateTaskControl.EndTask(DownloadModTaskControl);
                                        }
                                    }
                                    lock (CounterClock)
                                    {
                                        UpdateTaskControl.Value = ++Counter;
                                    }
                                });
                            }
                            finally
                            {
                                MergeTasks.Release();
                            }
                        }).Start();
                        GitMods.ForEach(dr =>
                        {
                            GitMod mod = dr.Value as GitMod;
                            if (mod != null)
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZ98 Mod - Git - {mod.Workshop.ModWorkshopId} - {mod.Name}", 0);
                                GitContext.Pull(mod.Workshop.GitPath);
                                UpdateTaskControl.EndTask(DownloadModTaskControl);
                            }
                            lock (CounterClock)
                            {
                                UpdateTaskControl.Value = ++Counter;
                            }
                        });
                        MergeTasks.WaitOne();
                        EndTask(UpdateTaskControl);

                        UpdateBZ98RModLists();
                    }
                });
            }
        }

        Task FindModsBZ98RTask = null;
        private void FindModsBZ98R(bool AutoDownload = false)
        {
            if (FindModsBZ98RTask == null
             || FindModsBZ98RTask.IsCanceled
             || FindModsBZ98RTask.IsCompleted
             || FindModsBZ98RTask.IsFaulted)
            {
                FindModsBZ98RTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZ98 Mods", 0);
                    List<WorkshopMod> ModsFound = WorkshopContext.GetMods(AppIdBZ98, null);

                    lock (ModStatus)
                        lock (Mods[AppIdBZ98])
                        {
                            //lock (FoundMods[AppIdBZ98]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                            {
                                FoundMods[AppIdBZ98].Clear();
                                foreach (WorkshopMod mod in ModsFound)
                                {
                                    mod.Known = Mods[AppIdBZ98].ContainsKey(mod.UniqueID);
                                    FoundMods[AppIdBZ98][mod.UniqueID] = mod;
                                    if (AutoDownload && !Mods[AppIdBZ98].ContainsKey(mod.UniqueID)) // some sort of strange race condition or something, Known isn't right
                                        DownloadMod(mod.URL, AppIdBZ98);
                                }
                                EndTask(UpdateTaskControl);
                            }

                            this.Invoke((MethodInvoker)delegate
                            {
                                lvFindModsBZ98R.BeginUpdate();
                                FoundMods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                lvFindModsBZ98R.DataSource = FoundMods[AppIdBZ98].Values.ToList<ILinqListViewFindModsItem>();
                                lvFindModsBZ98R.EndUpdate();
                            });
                        }
                });
            }
        }

        Task GetMpGamesBZ98RTask = null;
        private void GetMpGamesBZ98R()
        {
            if (GetMpGamesBZ98RTask == null
             || GetMpGamesBZ98RTask.IsCanceled
             || GetMpGamesBZ98RTask.IsCompleted
             || GetMpGamesBZ98RTask.IsFaulted)
            {
                GetMpGamesBZ98RTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZ98 Multiplayer Games", 0);
                    MultiplayerGamelistData data = MultiplayerSessionServer.GetMpGamesBZ98R();
                    EndTask(UpdateTaskControl);

                    this.Invoke((MethodInvoker)delegate
                    {
                        lvMultiplayerBZ98R.BeginUpdate();
                        lvMultiplayerBZ98R.DataSource = data;
                        lvMultiplayerBZ98R.EndUpdate();
                    });
                });
            }
        }
    }
}
