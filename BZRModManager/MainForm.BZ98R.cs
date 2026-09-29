using BZRModManager.ModItem;
using SteamVent.Common;
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
        private async Task UpdateBZ98RModListsAsync()
        {
            if (Interlocked.CompareExchange(ref _updateBZ98RModListsRunning, 1, 0) == 1)
                return; // a scan is already in flight; do not re-enter
            try
            {
                TaskControl UpdateBZ98RModListsTaskControl = AddTask("Update BZ98 Mod List", 0);
                List<ILinqListViewItemMods> modsSnapshot;
                List<ILinqListViewFindModsItem> findSnapshot;
                HashSet<string> FoundModIDs = new HashSet<string>();

                // Scan all three sources concurrently. Each scan serializes its own writes to the
                // shared Mods[AppIdBZ98] dictionary under ModsLock (this fixes the previous
                // unsynchronised concurrent writes); the I/O itself runs in parallel. This replaces
                // the old Semaphore(0,3) + three fire-and-forget launches + WaitOne()x3.
                await Task.WhenAll(
                    ScanBZ98R_SteamCmdAsync(UpdateBZ98RModListsTaskControl, FoundModIDs),
                    ScanBZ98R_GitAsync(UpdateBZ98RModListsTaskControl, FoundModIDs),
                    ScanBZ98R_SteamAsync(UpdateBZ98RModListsTaskControl, FoundModIDs));

                // Finalize the shared collections under the lock (single owner, no nesting).
                await ModsLock.WaitAsync();
                try
                {
                    foreach (string KnownMod in Mods[AppIdBZ98].Keys.ToList())
                    {
                        if (!FoundModIDs.Contains(KnownMod))
                            Mods[AppIdBZ98].Remove(KnownMod);
                    }

                    foreach (var kv in Mods[AppIdBZ98])
                    {
                        if (FoundMods[AppIdBZ98].ContainsKey(kv.Key))
                            FoundMods[AppIdBZ98][kv.Key].Known = true;
                    }

                    Mods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                    modsSnapshot = Mods[AppIdBZ98].Values.ToList<ILinqListViewItemMods>();
                    FoundMods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                    findSnapshot = FoundMods[AppIdBZ98].Values.ToList<ILinqListViewFindModsItem>();
                }
                finally
                {
                    ModsLock.Release();
                }

                EndTask(UpdateBZ98RModListsTaskControl);

                // Marshal ONLY the pure UI refresh asynchronously, outside all locks.
                UiInvoke(() =>
                {
                    lvModsBZ98R.BeginUpdate();
                    lvModsBZ98R.DataSource = modsSnapshot;
                    lvModsBZ98R.EndUpdate();
                    lvFindModsBZ98R.BeginUpdate();
                    lvFindModsBZ98R.DataSource = findSnapshot;
                    lvFindModsBZ98R.EndUpdate();
                });
            }
            finally
            {
                Interlocked.Exchange(ref _updateBZ98RModListsRunning, 0);
            }
        }

        private async Task ScanBZ98R_SteamCmdAsync(TaskControl parent, HashSet<string> FoundModIDs)
        {
            TaskControl UpdateTask = parent.AddTask("Update BZ98 Mod List (SteamCmd)", 0);
            try
            {
                List<WorkshopItemStatus> stats = await SteamCmd.WorkshopStatusAsync(AppIdBZ98);
                if (stats != null)
                {
                    await ModsLock.WaitAsync();
                    try
                    {
                        stats.ForEach(dr =>
                        {
                            string ModId = SteamCmdMod.GetUniqueId(dr.WorkshopId);
                            if (!Mods[AppIdBZ98].ContainsKey(ModId))
                                Mods[AppIdBZ98][ModId] = new SteamCmdMod(AppIdBZ98, dr);
                            else
                                ((SteamCmdMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
                            Mods[AppIdBZ98][ModId].HasUpdate = dr.HasUpdate;
                            Mods[AppIdBZ98][ModId].FolderOnlyDetection = dr.Detection.HasFlag(WorkshopItemStatus.WorkshopDetectionType.Folder);
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

        private async Task ScanBZ98R_GitAsync(TaskControl parent, HashSet<string> FoundModIDs)
        {
            TaskControl UpdateTask = parent.AddTask("Update BZ98 Mod List (Git)", 0);
            try
            {
                await foreach (var dr in GitContext.WorkshopItemsOnDriveAsync(AppIdBZ98))
                {
                    await ModsLock.WaitAsync();
                    try
                    {
                        string ModId = GitMod.GetUniqueId(dr.ModWorkshopId);
                        if (!Mods[AppIdBZ98].ContainsKey(ModId))
                            Mods[AppIdBZ98][ModId] = new GitMod(AppIdBZ98, dr);
                        else
                            ((GitMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
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

        private async Task ScanBZ98R_SteamAsync(TaskControl parent, HashSet<string> FoundModIDs)
        {
            if (settings.BZ98RSteamPath == null)
                return;
            TaskControl UpdateTask = parent.AddTask("Update BZ98 Mod List (Steam)", 0);
            try
            {
                await foreach (var dr in SteamContext.WorkshopItemsOnDriveAsync(settings.BZ98RSteamPath, AppIdBZ98))
                {
                    await ModsLock.WaitAsync();
                    try
                    {
                        string ModId = SteamMod.GetUniqueId(dr);
                        if (!Mods[AppIdBZ98].ContainsKey(ModId))
                            Mods[AppIdBZ98][ModId] = new SteamMod(AppIdBZ98, dr);
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

        private async Task UpdateBZ98RModsAsync(bool agressive)
        {
            if (Interlocked.CompareExchange(ref _updateBZ98RModsRunning, 1, 0) == 1)
                return;
            try
            {
                TaskControl UpdateTaskControl = AddTask("Update BZ98 Mods", 0);

                List<KeyValuePair<string, ModItemBase>> ModList;
                await ModsLock.WaitAsync();
                try { ModList = Mods[AppIdBZ98].ToList(); }
                finally { ModsLock.Release(); }

                UpdateTaskControl.Maximum = ModList.Count;
                List<KeyValuePair<string, ModItemBase>> NoUpdateMods = ModList.Where(dr => !(dr.Value is SteamCmdMod) && !(dr.Value is GitMod)).ToList();
                List<KeyValuePair<string, ModItemBase>> SteamCmdMods = ModList.Where(dr => (dr.Value is SteamCmdMod)).ToList();
                List<KeyValuePair<string, ModItemBase>> GitMods = ModList.Where(dr => (dr.Value is GitMod)).ToList();

                // Shared thread-safe progress counter (replaces lock(CounterClock) + ++Counter).
                int counter = 0;
                foreach (var dr in NoUpdateMods)
                    UpdateTaskControl.Value = Interlocked.Increment(ref counter);

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
                                    TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZ98 Mod - SteamCmd - {modSteam.Workshop.WorkshopId} - {modSteam.Name}", 0);
                                    SteamCmdException ex_ = null;
                                    int OtherErrorCounter = 0;
                                    do
                                    {
                                        ex_ = null;
                                        try
                                        {
                                            await SteamCmd.WorkshopDownloadItemAsync(AppIdBZ98, modSteam.Workshop.WorkshopId);
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
                        GitMods.ForEach(dr =>
                        {
                            GitMod mod = dr.Value as GitMod;
                            if (mod != null)
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZ98 Mod - Git - {mod.Workshop.ModWorkshopId} - {mod.Name}", 0);
                                GitContext.Pull(settings.GitPath, mod.Workshop.GitPath);
                                UpdateTaskControl.EndTask(DownloadModTaskControl);
                            }
                            UpdateTaskControl.Value = Interlocked.Increment(ref counter);
                        });
                    }));

                EndTask(UpdateTaskControl);

                await this.UpdateBZ98RModListsAsync();
            }
            finally
            {
                Interlocked.Exchange(ref _updateBZ98RModsRunning, 0);
            }
        }

        private async Task FindModsBZ98RAsync(bool AutoDownload = false)
        {
            if (Interlocked.CompareExchange(ref _findModsBZ98RRunning, 1, 0) == 1)
                return;
            try
            {
                TaskControl UpdateTaskControl = AddTask("Find BZ98 Mods", 0);
                List<WorkshopMod> ModsFound = WorkshopContext.GetMods(AppIdBZ98, null);
                List<ILinqListViewFindModsItem> findSnapshot;
                List<string> AutoDownloadURLs = new List<string>();

                await ModsLock.WaitAsync();
                try
                {
                    FoundMods[AppIdBZ98].Clear();
                    foreach (WorkshopMod mod in ModsFound)
                    {
                        mod.Known = Mods[AppIdBZ98].ContainsKey(mod.UniqueID);
                        FoundMods[AppIdBZ98][mod.UniqueID] = mod;
                        if (AutoDownload && !Mods[AppIdBZ98].ContainsKey(mod.UniqueID))
                            AutoDownloadURLs.Add(mod.URL);
                    }
                    EndTask(UpdateTaskControl);
                    FoundMods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                    findSnapshot = FoundMods[AppIdBZ98].Values.ToList<ILinqListViewFindModsItem>();
                }
                finally
                {
                    ModsLock.Release();
                }

                UiInvoke(() =>
                {
                    lvFindModsBZ98R.BeginUpdate();
                    lvFindModsBZ98R.DataSource = findSnapshot;
                    lvFindModsBZ98R.EndUpdate();
                });

                // Kick off auto-downloads AFTER releasing ModsLock, so we never hold the lock across
                // a modal dialog / long download.
                foreach (string url in AutoDownloadURLs)
                    _ = DownloadMod(url, AppIdBZ98);
            }
            finally
            {
                Interlocked.Exchange(ref _findModsBZ98RRunning, 0);
            }
        }

        private async Task GetMpGamesBZ98RAsync()
        {
            if (Interlocked.CompareExchange(ref _getMpGamesBZ98RRunning, 1, 0) == 1)
                return;
            try
            {
                TaskControl UpdateTaskControl = AddTask("Find BZ98 Multiplayer Games", 0);
                MultiplayerGamelistData data = await Task.Run(() => MultiplayerSessionServer.GetMpGamesBZ98R());
                EndTask(UpdateTaskControl);

                UiInvoke(() =>
                {
                    if ((data.EndpointVersion ?? 0) > 0)
                        MessageBox.Show("Please update your mod manager to ensure the MP game list functions properly.\r\nThe API has been updated and may no longer be compatable.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    lvMultiplayerBZ98R.BeginUpdate();
                    lvMultiplayerBZ98R.DataSource = data;
                    lvMultiplayerBZ98R.EndUpdate();
                });
            }
            finally
            {
                Interlocked.Exchange(ref _getMpGamesBZ98RRunning, 0);
            }
        }
    }
}
