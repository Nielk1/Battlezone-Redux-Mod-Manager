using BZRModManager.ModItem;
using IniParser;
using IniParser.Model;
using Monitor.Core.Utilities;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    public partial class MainForm
    {
        Task UpdateBZCCModListsTask = null;
        //TaskControl UpdateBZCCModListsTaskControl = null;
        private void UpdateBZCCModLists()
        {
            if (UpdateBZCCModListsTask == null
              || UpdateBZCCModListsTask.IsCanceled
              || UpdateBZCCModListsTask.IsCompleted
              || UpdateBZCCModListsTask.IsFaulted)
            {
                //EndTask(UpdateBZCCModListsTaskControl);
                TaskControl UpdateBZCCModListsTaskControl = AddTask("Update BZCC Mod List", 0);
                UpdateBZCCModListsTask = Task.Factory.StartNew(() =>
                {
                    lock (ModStatus)
                    {
                        HashSet<string> FoundModIDs = new HashSet<string>();

                        Semaphore loadSemaphore = new Semaphore(0, 3);
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                TaskControl UpdateTask = UpdateBZCCModListsTaskControl.AddTask("Update BZCC Mod List (SteamCmd)", 0);
                                List<WorkshopItemStatus> stats = SteamCmd.WorkshopStatus(AppIdBZCC);
                                stats?.ForEach(dr =>
                                {
                                    string ModId = SteamCmdMod.GetUniqueId(dr.WorkshopId);
                                    if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                    {
                                        Mods[AppIdBZCC][ModId] = new SteamCmdMod(AppIdBZCC, dr);
                                    }
                                    else
                                    {
                                        ((SteamCmdMod)Mods[AppIdBZCC][ModId]).Workshop = dr;
                                    }
                                    Mods[AppIdBZCC][ModId].HasUpdate = dr.HasUpdate;
                                    Mods[AppIdBZCC][ModId].FolderOnlyDetection = dr.FolderOnlyDetection;
                                    FoundModIDs.Add(ModId);
                                });
                                UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
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
                                TaskControl UpdateTask = UpdateBZCCModListsTaskControl.AddTask("Update BZCC Mod List (Git)", 0);
                                List<GitModStatus> stats = GitContext.WorkshopItemsOnDrive(AppIdBZCC);
                                stats?.ForEach(dr =>
                                {
                                    string ModId = GitMod.GetUniqueId(dr.ModWorkshopId);
                                    if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                    {
                                        Mods[AppIdBZCC][ModId] = new GitMod(AppIdBZCC, dr);
                                    }
                                    else
                                    {
                                        ((GitMod)Mods[AppIdBZCC][ModId]).Workshop = dr;
                                    }
                                    FoundModIDs.Add(ModId);
                                });
                                UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
                            }
                            finally
                            {
                                loadSemaphore.Release();
                            }
                        });

                        if (settings.BZCCSteamPath != null)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    TaskControl UpdateTask = UpdateBZCCModListsTaskControl.AddTask("Update BZCC Mod List (Steam)", 0);
                                    HashSet<UInt64> Dependencies = new HashSet<UInt64>();
                                    SteamContext.WorkshopItemsOnDrive(settings.BZCCSteamPath, AppIdBZCC)?.ForEach(dr =>
                                    {
                                        string ModId = SteamMod.GetUniqueId(dr);
                                        if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                        {
                                            SteamMod mod = new SteamMod(AppIdBZCC, dr);
                                            Mods[AppIdBZCC][ModId] = mod;

                                            string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, MainForm.AppIdBZCC);
                                            string[] Deps = BZCCTools.GetAssetDependencies(Path.Combine(workshopFolder, mod.WorkshopId.ToString()));
                                            if (Deps != null)
                                            {
                                                foreach (string Dep in Deps)
                                                {
                                                    UInt64 DepL;
                                                    if (UInt64.TryParse(Dep, out DepL))
                                                    {
                                                        Dependencies.Add(DepL);
                                                    }
                                                }
                                            }
                                        }
                                        FoundModIDs.Add(ModId);
                                    });
                                    foreach (var dr in Dependencies)
                                    {
                                        string ModId = SteamMod.GetUniqueId(dr);
                                        if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                        {
                                            SteamMod mod = new SteamMod(AppIdBZCC, dr);
                                            Mods[AppIdBZCC][ModId] = mod;
                                        }
                                    }
                                    UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
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

                        lock (Mods[AppIdBZCC])
                        {
                            foreach (string KnownMod in Mods[AppIdBZCC].Keys.ToList())
                            {
                                if (!FoundModIDs.Contains(KnownMod))
                                    Mods[AppIdBZCC].Remove(KnownMod);
                            }

                            this.Invoke((MethodInvoker)delegate
                            {
                                lvModsBZCC.BeginUpdate();
                                Mods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                lvModsBZCC.DataSource = Mods[AppIdBZCC].Values.ToList<ILinqListViewItemMods>();
                                lvModsBZCC.EndUpdate();

                                //lock (Mods[AppIdBZCC])
                                {
                                    //lock (FoundMods[AppIdBZCC]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                                    {
                                        foreach (var kv in Mods[AppIdBZCC])
                                        {
                                            if (FoundMods[AppIdBZCC].ContainsKey(kv.Key))
                                                FoundMods[AppIdBZCC][kv.Key].Known = true;
                                        }
                                        lvFindModsBZCC.BeginUpdate();
                                        FoundMods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                        lvFindModsBZCC.DataSource = FoundMods[AppIdBZCC].Values.ToList<ILinqListViewFindModsItem>();
                                        lvFindModsBZCC.EndUpdate();
                                    }
                                }

                                EndTask(UpdateBZCCModListsTaskControl);
                            });
                        }
                    }
                });
            }
        }

        Task UpdateBZCCModsTask = null;
        private void UpdateBZCCMods(bool agressive)
        {
            if (UpdateBZCCModsTask == null
              || UpdateBZCCModsTask.IsCanceled
              || UpdateBZCCModsTask.IsCompleted
              || UpdateBZCCModsTask.IsFaulted)
            {
                UpdateBZCCModsTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Update BZCC Mods", 0);
                    lock (Mods[AppIdBZCC])
                    {
                        List<KeyValuePair<string, ModItemBase>> ModList = Mods[AppIdBZCC].ToList();
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
                                            TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - SteamCmd - {modSteam.Workshop.WorkshopId} - {modSteam.Name}", 0);
                                            SteamCmdException ex_ = null;
                                            int OtherErrorCounter = 0;
                                            do
                                            {
                                                ex_ = null;
                                                try
                                                {
                                                    SteamCmd.WorkshopDownloadItem(AppIdBZCC, modSteam.Workshop.WorkshopId);
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
                        GitMods.AsParallel().WithDegreeOfParallelism(2).ForAll(dr =>
                        {
                            GitMod mod = dr.Value as GitMod;
                            if (mod != null)
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - Git - {mod.Workshop.ModWorkshopId} - {mod.Name}", 0);
                                GitContext.Pull(settings.GitPath, mod.Workshop.GitPath);
                                UpdateTaskControl.EndTask(DownloadModTaskControl);
                            }
                            lock (CounterClock)
                            {
                                UpdateTaskControl.Value = ++Counter;
                            }
                        });
                        MergeTasks.WaitOne();
                        EndTask(UpdateTaskControl);

                        UpdateBZCCModLists();
                    }
                });
            }
        }

        Task GetDependenciesBZCCModsTask = null;
        private void GetDependenciesBZCCMods()
        {
            if (GetDependenciesBZCCModsTask == null
              || GetDependenciesBZCCModsTask.IsCanceled
              || GetDependenciesBZCCModsTask.IsCompleted
              || GetDependenciesBZCCModsTask.IsFaulted)
            {
                GetDependenciesBZCCModsTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Get BZCC Mod Dependencies", 0);
                    lock (Mods[AppIdBZCC])
                    {
                        List<string> SteamCmdDependencies = new List<string>();
                        HashSet<UInt64> DependenciesGotten = new HashSet<UInt64>();
                        List<KeyValuePair<string, ModItemBase>> ModList = Mods[AppIdBZCC].ToList();
                        UpdateTaskControl.Maximum = ModList.Count;
                        int Counter = 0;
                        ModList.ForEach(dr =>
                        {
                            UpdateTaskControl.Value = ++Counter;
                            SteamCmdMod mod = dr.Value as SteamCmdMod;
                            if (mod != null)
                            {
                                string[] Dependencies = null;
                                try
                                {
                                    Dependencies = BZCCTools.GetAssetDependencies($"steamcmd\\steamapps\\workshop\\content\\{mod.AppId}\\{mod.Workshop.WorkshopId}");
                                }
                                catch { }
                                if (Dependencies != null)
                                    SteamCmdDependencies.AddRange(Dependencies);
                                DependenciesGotten.Add(mod.Workshop.WorkshopId);
                            }
                        });
                        EndTask(UpdateTaskControl);
                        List<string> SteamCmdDependenciesList = SteamCmdDependencies.Distinct().ToList();
                        UpdateTaskControl = AddTask("Download BZCC Mod Dependencies", SteamCmdDependenciesList.Count);
                        object CounterClock = new object();
                        Counter = 0;
                        SteamCmdDependenciesList.ForEach(dr =>
                        {
                            UInt64 tmpLong = 0;
                            if (UInt64.TryParse(dr, out tmpLong) && !DependenciesGotten.Contains(tmpLong))
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - SteamCmd - {tmpLong}", 0);
                                SteamCmdException ex_ = null;
                                int OtherErrorCounter = 0;
                                do
                                {
                                    ex_ = null;
                                    try
                                    {
                                        SteamCmd.WorkshopDownloadItem(AppIdBZCC, tmpLong);
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
                            lock (CounterClock)
                            {
                                UpdateTaskControl.Value = ++Counter;
                            }
                        });
                        EndTask(UpdateTaskControl);
                        this.Invoke((MethodInvoker)delegate
                        {
                            UpdateBZCCModLists();
                        });
                    }
                });
            }
        }

        Task FindModsBZCCTask = null;
        private void FindModsBZCC(bool AutoDownload = false)
        {
            if (FindModsBZCCTask == null
             || FindModsBZCCTask.IsCanceled
             || FindModsBZCCTask.IsCompleted
             || FindModsBZCCTask.IsFaulted)
            {
                FindModsBZCCTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZCC Mods", 0);
                    List<WorkshopMod> ModsFound = WorkshopContext.GetMods(AppIdBZCC, new string[] { "config", "addon" }); // we only need these two as Asset type can be collected via dependency scan

                    lock (ModStatus)
                        lock (Mods[AppIdBZCC])
                        {
                            //lock (FoundMods[AppIdBZCC]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                            {
                                FoundMods[AppIdBZCC].Clear();
                                foreach (WorkshopMod mod in ModsFound)
                                {
                                    mod.Known = Mods[AppIdBZCC].ContainsKey(mod.UniqueID);
                                    FoundMods[AppIdBZCC][mod.UniqueID] = mod;
                                    if (AutoDownload && !Mods[AppIdBZCC].ContainsKey(mod.UniqueID)) // some sort of strange race condition or something, Known isn't right
                                        DownloadMod(mod.URL, AppIdBZCC);
                                }
                                EndTask(UpdateTaskControl);
                            }

                            this.Invoke((MethodInvoker)delegate
                            {
                                lvFindModsBZCC.BeginUpdate();
                                FoundMods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                lvFindModsBZCC.DataSource = FoundMods[AppIdBZCC].Values.ToList<ILinqListViewFindModsItem>();
                                lvFindModsBZCC.EndUpdate();
                            });
                        }
                });
            }
        }

        Task GetMpGamesBZCCTask = null;
        private void GetMpGamesBZCC()
        {
            if (GetMpGamesBZCCTask == null
             || GetMpGamesBZCCTask.IsCanceled
             || GetMpGamesBZCCTask.IsCompleted
             || GetMpGamesBZCCTask.IsFaulted)
            {
                GetMpGamesBZCCTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZCC Multiplayer Games", 0);
                    MultiplayerGamelistData data = MultiplayerSessionServer.GetMpGamesBZCC();
                    EndTask(UpdateTaskControl);

                    this.Invoke((MethodInvoker)delegate
                    {
                        if ((data.EndpointVersion ?? 0) > 0)
                        {
                            MessageBox.Show("Please update your mod manager to ensure the MP game list functions properly.\r\nThe API has been updated and may no longer be compatable.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        lvMultiplayerBZCC.BeginUpdate();
                        lvMultiplayerBZCC.DataSource = data;
                        lvMultiplayerBZCC.EndUpdate();
                    });
                });
            }
        }

        private void TryBzccMpJoinFix(string path, bool steam)
        {
            string destinationFolder = steam ? Path.Combine(SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, MainForm.AppIdBZCC), "bzrmm_bzccjoinfix") : Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", "bzrmm_bzccjoinfix");
            string sourceFolder = Path.GetFullPath(Path.Combine("fixes", "bzrmm_bzccjoinfix"));

            bool NeedFix = BZCCTools.NeedsJoinShellFix(path);
            
            if (NeedFix)
            {
                if (Directory.Exists(destinationFolder))
                {
                    if (JunctionPoint.Exists(destinationFolder))
                    {
                        if (JunctionPoint.GetTarget(destinationFolder) != sourceFolder)
                        {
                            JunctionPoint.Delete(destinationFolder);
                        }
                    }
                    else
                    {
                        Directory.Delete(destinationFolder);
                    }
                }
                if (!Directory.Exists(destinationFolder))
                {
                    JunctionPoint.Create(destinationFolder, sourceFolder, true);
                }

                string LaunchIni = Path.Combine(MainForm.settings.BZCCMyDocsPath, "launch.ini");
                if (File.Exists(LaunchIni))
                {
                    try
                    {
                        FileIniDataParser parser = new FileIniDataParser();
                        IniData data = parser.ReadFile(LaunchIni);
                        string[] activeAddons = (data["config"]?["activeAddons"] ?? string.Empty).Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (activeAddons.Length == 0 || activeAddons.Last() != "bzrmm_bzccjoinfix")
                        {
                            data["config"]["activeAddons"] = string.Join(",", activeAddons.Append("bzrmm_bzccjoinfix"));
                            File.WriteAllText(LaunchIni, data.ToString());
                        }
                    }
                    catch { }
                }
                else
                {
                    File.WriteAllText(LaunchIni, "[config]\r\nactiveAddons = bzrmm_bzccjoinfix");
                }
            }
            else
            {
                if (Directory.Exists(destinationFolder))
                    Directory.Delete(destinationFolder, true);
            }
        }
    }
}
