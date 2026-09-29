using BZRModManager.ModItem;
using IniParser;
using IniParser.Model;
using Monitor.Core.Utilities;
using SteamVent.Common;
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
        private async Task UpdateBZCCModListsAsync()
        {
            if (Interlocked.CompareExchange(ref _updateBZCCModListsRunning, 1, 0) == 1)
                return; // a scan is already in flight; do not re-enter
            try
            {
                TaskControl UpdateBZCCModListsTaskControl = AddTask("Update BZCC Mod List", 0);
                List<ILinqListViewItemMods> modsSnapshot;
                List<ILinqListViewFindModsItem> findSnapshot;
                HashSet<string> FoundModIDs = new HashSet<string>();

                await Task.WhenAll(
                    ScanBZCC_SteamCmdAsync(UpdateBZCCModListsTaskControl, FoundModIDs),
                    ScanBZCC_GitAsync(UpdateBZCCModListsTaskControl, FoundModIDs),
                    ScanBZCC_SteamAsync(UpdateBZCCModListsTaskControl, FoundModIDs));

                await ModsLock.WaitAsync();
                try
                {
                    foreach (string KnownMod in Mods[AppIdBZCC].Keys.ToList())
                        if (!FoundModIDs.Contains(KnownMod))
                            Mods[AppIdBZCC].Remove(KnownMod);
                    foreach (var kv in Mods[AppIdBZCC])
                        if (FoundMods[AppIdBZCC].ContainsKey(kv.Key))
                            FoundMods[AppIdBZCC][kv.Key].Known = true;
                    Mods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                    modsSnapshot = Mods[AppIdBZCC].Values.ToList<ILinqListViewItemMods>();
                    FoundMods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                    findSnapshot = FoundMods[AppIdBZCC].Values.ToList<ILinqListViewFindModsItem>();
                }
                finally
                {
                    ModsLock.Release();
                }

                EndTask(UpdateBZCCModListsTaskControl);

                UiInvoke(() =>
                {
                    lvModsBZCC.BeginUpdate();
                    lvModsBZCC.DataSource = modsSnapshot;
                    lvModsBZCC.EndUpdate();
                    lvFindModsBZCC.BeginUpdate();
                    lvFindModsBZCC.DataSource = findSnapshot;
                    lvFindModsBZCC.EndUpdate();
                });
            }
            finally
            {
                Interlocked.Exchange(ref _updateBZCCModListsRunning, 0);
            }
        }

        private async Task ScanBZCC_SteamCmdAsync(TaskControl parent, HashSet<string> FoundModIDs)
        {
            TaskControl UpdateTask = parent.AddTask("Update BZCC Mod List (SteamCmd)", 0);
            try
            {
                List<WorkshopItemStatus> stats = await SteamCmd.WorkshopStatusAsync(AppIdBZCC);
                if (stats != null)
                {
                    await ModsLock.WaitAsync();
                    try
                    {
                        stats.ForEach(dr =>
                        {
                            string ModId = SteamCmdMod.GetUniqueId(dr.WorkshopId);
                            if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                Mods[AppIdBZCC][ModId] = new SteamCmdMod(AppIdBZCC, dr);
                            else
                                ((SteamCmdMod)Mods[AppIdBZCC][ModId]).Workshop = dr;
                            Mods[AppIdBZCC][ModId].HasUpdate = dr.HasUpdate;
                            Mods[AppIdBZCC][ModId].FolderOnlyDetection = dr.Detection.HasFlag(WorkshopItemStatus.WorkshopDetectionType.Folder);
                            FoundModIDs.Add(ModId);
                        });
                    }
                    finally
                    {
                        ModsLock.Release();
                    }
                }
            }
            finally
            {
                parent.EndTask(UpdateTask);
            }
        }

        private async Task ScanBZCC_GitAsync(TaskControl parent, HashSet<string> FoundModIDs)
        {
            TaskControl UpdateTask = parent.AddTask("Update BZCC Mod List (Git)", 0);
            try
            {
                await foreach (var dr in GitContext.WorkshopItemsOnDriveAsync(AppIdBZCC))
                {
                    await ModsLock.WaitAsync();
                    try
                    {
                        string ModId = GitMod.GetUniqueId(dr.ModWorkshopId);
                        if (!Mods[AppIdBZCC].ContainsKey(ModId))
                            Mods[AppIdBZCC][ModId] = new GitMod(AppIdBZCC, dr);
                        else
                            ((GitMod)Mods[AppIdBZCC][ModId]).Workshop = dr;
                        FoundModIDs.Add(ModId);
                    }
                    finally
                    {
                        ModsLock.Release();
                    }
                }
            }
            finally
            {
                parent.EndTask(UpdateTask);
            }
        }

        private async Task ScanBZCC_SteamAsync(TaskControl parent, HashSet<string> FoundModIDs)
        {
            if (settings.BZCCSteamPath == null)
                return;
            TaskControl UpdateTask = parent.AddTask("Update BZCC Mod List (Steam)", 0);
            try
            {
                HashSet<UInt64> Dependencies = new HashSet<UInt64>();
                await foreach (var dr in SteamContext.WorkshopItemsOnDriveAsync(settings.BZCCSteamPath, AppIdBZCC))
                {
                    await ModsLock.WaitAsync();
                    try
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
                                        Dependencies.Add(DepL);
                                }
                            }
                        }
                        FoundModIDs.Add(ModId);
                    }
                    finally
                    {
                        ModsLock.Release();
                    }
                }
                foreach (var dr in Dependencies)
                {
                    await ModsLock.WaitAsync();
                    try
                    {
                        string ModId = SteamMod.GetUniqueId(dr);
                        if (!Mods[AppIdBZCC].ContainsKey(ModId))
                        {
                            SteamMod mod = new SteamMod(AppIdBZCC, dr);
                            Mods[AppIdBZCC][ModId] = mod;
                        }
                    }
                    finally
                    {
                        ModsLock.Release();
                    }
                }
            }
            finally
            {
                parent.EndTask(UpdateTask);
            }
        }

        private async Task UpdateBZCCModsAsync(bool agressive)
        {
            if (Interlocked.CompareExchange(ref _updateBZCCModsRunning, 1, 0) == 1)
                return;
            try
            {
                TaskControl UpdateTaskControl = AddTask("Update BZCC Mods", 0);

                List<KeyValuePair<string, ModItemBase>> ModList;
                await ModsLock.WaitAsync();
                try { ModList = Mods[AppIdBZCC].ToList(); }
                finally { ModsLock.Release(); }

                UpdateTaskControl.Maximum = ModList.Count;
                List<KeyValuePair<string, ModItemBase>> NoUpdateMods = ModList.Where(dr => !(dr.Value is SteamCmdMod) && !(dr.Value is GitMod)).ToList();
                List<KeyValuePair<string, ModItemBase>> SteamCmdMods = ModList.Where(dr => (dr.Value is SteamCmdMod)).ToList();
                List<KeyValuePair<string, ModItemBase>> GitMods = ModList.Where(dr => (dr.Value is GitMod)).ToList();

                // Shared thread-safe progress counter (replaces lock(CounterClock) + ++Counter).
                int counter = 0;
                foreach (var dr in NoUpdateMods)
                    UpdateTaskControl.Value = Interlocked.Increment(ref counter);

                // SteamCmd downloads (async) and Git pulls (AsParallel, degree 2) run concurrently.
                await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        foreach (var dr in SteamCmdMods)
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
                                            await SteamCmd.WorkshopDownloadItemAsync(AppIdBZCC, modSteam.Workshop.WorkshopId);
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
                            UpdateTaskControl.Value = Interlocked.Increment(ref counter);
                        }
                    }),
                    Task.Run(() =>
                    {
                        GitMods.AsParallel().WithDegreeOfParallelism(2).ForAll(dr =>
                        {
                            GitMod mod = dr.Value as GitMod;
                            if (mod != null)
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - Git - {mod.Workshop.ModWorkshopId} - {mod.Name}", 0);
                                GitContext.Pull(settings.GitPath, mod.Workshop.GitPath);
                                UpdateTaskControl.EndTask(DownloadModTaskControl);
                            }
                            UpdateTaskControl.Value = Interlocked.Increment(ref counter);
                        });
                    }));

                EndTask(UpdateTaskControl);

                await this.UpdateBZCCModListsAsync();
            }
            finally
            {
                Interlocked.Exchange(ref _updateBZCCModsRunning, 0);
            }
        }

        private async Task GetDependenciesBZCCModsAsync()
        {
            if (Interlocked.CompareExchange(ref _getDependenciesBZCCModsRunning, 1, 0) == 1)
                return;
            try
            {
                TaskControl UpdateTaskControl = AddTask("Get BZCC Mod Dependencies", 0);

                List<string> SteamCmdDependencies = new List<string>();
                HashSet<UInt64> DependenciesGotten = new HashSet<UInt64>();

                // Gather asset dependencies off the UI thread (blocking file I/O), holding ModsLock
                // only for the brief collection copy.
                await Task.Run(() =>
                {
                    List<KeyValuePair<string, ModItemBase>> ModList;
                    ModsLock.Wait();
                    try { ModList = Mods[AppIdBZCC].ToList(); }
                    finally { ModsLock.Release(); }

                    UpdateTaskControl.Maximum = ModList.Count;
                    int counter = 0;
                    foreach (var dr in ModList)
                    {
                        UpdateTaskControl.Value = ++counter;
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
                    }
                });
                EndTask(UpdateTaskControl);

                List<string> SteamCmdDependenciesList = SteamCmdDependencies.Distinct().ToList();
                UpdateTaskControl = AddTask("Download BZCC Mod Dependencies", SteamCmdDependenciesList.Count);
                int counter2 = 0;
                foreach (var dr in SteamCmdDependenciesList)
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
                                await SteamCmd.WorkshopDownloadItemAsync(AppIdBZCC, tmpLong);
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
                    UpdateTaskControl.Value = ++counter2;
                }
                EndTask(UpdateTaskControl);

                await this.UpdateBZCCModListsAsync();
            }
            finally
            {
                Interlocked.Exchange(ref _getDependenciesBZCCModsRunning, 0);
            }
        }

        private async Task FindModsBZCCAsync(bool AutoDownload = false)
        {
            if (Interlocked.CompareExchange(ref _findModsBZCCRunning, 1, 0) == 1)
                return;
            try
            {
                TaskControl UpdateTaskControl = AddTask("Find BZCC Mods", 0);
                List<WorkshopMod> ModsFound = WorkshopContext.GetMods(AppIdBZCC, new string[] { "config", "addon" }); // we only need these two as Asset type can be collected via dependency scan
                List<ILinqListViewFindModsItem> findSnapshot;
                List<string> AutoDownloadURLs = new List<string>();

                await ModsLock.WaitAsync();
                try
                {
                    FoundMods[AppIdBZCC].Clear();
                    foreach (WorkshopMod mod in ModsFound)
                    {
                        mod.Known = Mods[AppIdBZCC].ContainsKey(mod.UniqueID);
                        FoundMods[AppIdBZCC][mod.UniqueID] = mod;
                        if (AutoDownload && !Mods[AppIdBZCC].ContainsKey(mod.UniqueID))
                            AutoDownloadURLs.Add(mod.URL);
                    }
                    EndTask(UpdateTaskControl);
                    FoundMods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                    findSnapshot = FoundMods[AppIdBZCC].Values.ToList<ILinqListViewFindModsItem>();
                }
                finally
                {
                    ModsLock.Release();
                }

                UiInvoke(() =>
                {
                    lvFindModsBZCC.BeginUpdate();
                    lvFindModsBZCC.DataSource = findSnapshot;
                    lvFindModsBZCC.EndUpdate();
                });

                foreach (string url in AutoDownloadURLs)
                    _ = DownloadMod(url, AppIdBZCC);
            }
            finally
            {
                Interlocked.Exchange(ref _findModsBZCCRunning, 0);
            }
        }

        private async Task GetMpGamesBZCCAsync()
        {
            if (Interlocked.CompareExchange(ref _getMpGamesBZCCRunning, 1, 0) == 1)
                return;
            try
            {
                TaskControl UpdateTaskControl = AddTask("Find BZCC Multiplayer Games", 0);
                MultiplayerGamelistData data = await Task.Run(() => MultiplayerSessionServer.GetMpGamesBZCC());
                EndTask(UpdateTaskControl);

                UiInvoke(() =>
                {
                    if ((data.EndpointVersion ?? 0) > 0)
                        MessageBox.Show("Please update your mod manager to ensure the MP game list functions properly.\r\nThe API has been updated and may no longer be compatable.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    lvMultiplayerBZCC.BeginUpdate();
                    lvMultiplayerBZCC.DataSource = data;
                    lvMultiplayerBZCC.EndUpdate();
                });
            }
            finally
            {
                Interlocked.Exchange(ref _getMpGamesBZCCRunning, 0);
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
                            JunctionPoint.Delete(destinationFolder);
                    }
                    else
                    {
                        Directory.Delete(destinationFolder);
                    }
                }
                if (!Directory.Exists(destinationFolder))
                    JunctionPoint.Create(destinationFolder, sourceFolder, true);

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
