using BZRModManager.ModItem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    /// <summary>
    /// Settings-tab controls for the SteamCmd data location (<c>+force_install_dir</c>),
    /// plus the quick "uninstall all / reinstall all" mod-link helpers that make it
    /// safe to change that location.
    /// </summary>
    public partial class MainForm
    {
        /// <summary>
        /// File (next to settings.json) that records the installed mod links when they
        /// are removed in bulk, so they can be restored later.
        /// </summary>
        private const string InstalledModsBackupFile = "installed_mods.json";

        /// <summary>
        /// Collects all currently installed (junctioned) mod links across both games and
        /// both install targets (GOG and Steam).
        /// </summary>
        private List<InstalledModsBackupEntry> GetInstalledModLinks()
        {
            List<InstalledModsBackupEntry> retVal = new List<InstalledModsBackupEntry>();
            Dictionary<string, ModItemBase> byId = new Dictionary<string, ModItemBase>(StringComparer.Ordinal);

            ModsLock.Wait();
            try
            {
                foreach (KeyValuePair<int, Dictionary<string, ModItemBase>> kv in Mods)
                    foreach (ModItemBase mod in kv.Value.Values)
                    {
                        if (mod.InstalledGog == InstallStatus.Linked)
                            retVal.Add(new InstalledModsBackupEntry { AppId = kv.Key, ModUniqueId = mod.UniqueID, InstallTarget = "GOG" });
                        if (mod.InstalledSteam == InstallStatus.Linked)
                            retVal.Add(new InstalledModsBackupEntry { AppId = kv.Key, ModUniqueId = mod.UniqueID, InstallTarget = "Steam" });
                        byId[mod.UniqueID] = mod;
                    }
            }
            finally
            {
                ModsLock.Release();
            }

            // Fill in display names outside the lock (reading them can touch the disk).
            foreach (InstalledModsBackupEntry entry in retVal)
            {
                ModItemBase mod;
                if (byId.TryGetValue(entry.ModUniqueId, out mod))
                {
                    try { entry.ModName = mod.Name; } catch { }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Clears the row caches of both mod list views so their install states re-render.
        /// </summary>
        private void RefreshModListsUI()
        {
            UiInvoke(() =>
            {
                ModsLock.Wait();
                try
                {
                    foreach (Dictionary<string, ModItemBase> dict in Mods.Values)
                        foreach (ModItemBase mod in dict.Values)
                            mod.ListViewItemCache = null;
                }
                finally
                {
                    ModsLock.Release();
                }

                lvModsBZ98R.Refresh();
                lvModsBZCC.Refresh();
            });
        }

        private void btnSteamCmdInstallDirFind_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select the folder SteamCmd should store its data in";
            if (dlg.ShowDialog() == DialogResult.OK)
                txtSteamCmdInstallDir.Text = dlg.SelectedPath;
        }

        private async void btnSteamCmdInstallDirApply_Click(object sender, EventArgs e)
        {
            string oldPath = (settings.SteamCmdInstallDir ?? string.Empty).Trim();
            string newPath = (txtSteamCmdInstallDir.Text ?? string.Empty).Trim();

            string defaultRoot = Path.GetFullPath("steamcmd");
            string oldFull = oldPath.Length > 0 ? Path.GetFullPath(oldPath) : defaultRoot;
            string newFull = newPath.Length > 0 ? Path.GetFullPath(newPath) : defaultRoot;

            if (string.Equals(oldFull, newFull, StringComparison.OrdinalIgnoreCase))
                return; // nothing changed

            // Never relocate SteamCmd's data while any mods are installed (junctioned).
            List<InstalledModsBackupEntry> installed = GetInstalledModLinks();
            if (installed.Count > 0)
            {
                MessageBox.Show(
                    $"The SteamCmd data location cannot be changed while {installed.Count} installed mod link(s) are still in place.\r\n\r\n" +
                    $"Use the \"Uninstall All Installed Mods\" button first - it removes the links and saves a list to {InstalledModsBackupFile} - then apply the new location, then use \"Reinstall Saved Mods\" to restore the links.\r\n\r\n" +
                    "The current location was not changed.",
                    "Mods Installed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                $"Change the SteamCmd data location?\r\n\r\nFrom: {oldFull}\r\nTo:   {newFull}",
                "Change SteamCmd Data Location", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            // Offer to move the existing data.
            bool moved = false;
            if (Directory.Exists(oldFull))
            {
                if (MessageBox.Show(
                    $"Move the existing SteamCmd data (downloaded mods, etc.) from\r\n{oldFull}\r\nto\r\n{newFull}\r\n?\r\n\r\nChoose No to leave it in place (it will no longer be used by SteamCmd).",
                    "Move Existing Mods", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    TaskControl moveTask = AddTask("Moving SteamCmd data", 0);
                    try
                    {
                        await Task.Run(() => MoveSteamCmdData(oldFull, newFull));
                        moved = true;
                    }
                    catch (Exception ex)
                    {
                        EndTask(moveTask);
                        MessageBox.Show($"Failed to move the SteamCmd data:\r\n{ex.Message}\r\n\r\nThe current location was not changed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    EndTask(moveTask);
                }
            }

            settings.SteamCmdInstallDir = string.Equals(newFull, defaultRoot, StringComparison.OrdinalIgnoreCase) ? null : newFull;
            SteamCmd.ForceInstallDir = settings.SteamCmdInstallDir; // null/empty → no +force_install_dir argument
            SaveSettings();

            Log($"SteamCmd data location changed to {(settings.SteamCmdInstallDir ?? defaultRoot)}{(moved ? " (data moved)" : "")}");
            MessageBox.Show(
                $"The SteamCmd data location is now\r\n{settings.SteamCmdInstallDir ?? defaultRoot}\r\nSteamCmd will use it on its next start.",
                "SteamCmd Data Location", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // The mods now live in the new place (or back in the default one); refresh the lists.
            await Task.WhenAll(this.UpdateBZ98RModListsAsync(), this.UpdateBZCCModListsAsync());
        }


        private void btnUninstallAllMods_Click(object sender, EventArgs e)
        {
            List<InstalledModsBackupEntry> installed = GetInstalledModLinks();
            if (installed.Count == 0)
            {
                MessageBox.Show("No installed (junctioned) mods were found; there is nothing to uninstall.", "No Installed Mods", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                $"Uninstall (unjunction) all {installed.Count} installed mod link(s) and save them to a list in\r\n{InstalledModsBackupFile}\r\n?\r\n\r\n" +
                "Use this before changing the SteamCmd data location. Afterwards use \"Reinstall Saved Mods\" to restore the links.",
                "Uninstall All Installed Mods", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            // Remove the junctions. Toggling a Linked mod uninstalls it; mods in any other
            // state (Collision, Missing, ...) are left alone.
            ModsLock.Wait();
            try
            {
                foreach (Dictionary<string, ModItemBase> dict in Mods.Values)
                    foreach (ModItemBase mod in dict.Values)
                    {
                        if (mod.InstalledGog == InstallStatus.Linked)
                            mod.ToggleGog();
                        if (mod.InstalledSteam == InstallStatus.Linked)
                            mod.ToggleSteam();
                    }
            }
            finally
            {
                ModsLock.Release();
            }

            // Save a list of the mods that were junctioned (and where) so they can be reinstalled later.
            string backupPath = Path.GetFullPath(InstalledModsBackupFile);
            File.WriteAllText(backupPath, JsonConvert.SerializeObject(new InstalledModsBackup { Mods = installed }, Formatting.Indented));

            Log($"Uninstalled {installed.Count} installed mod link(s); saved the list to {backupPath}");
            RefreshModListsUI();

            MessageBox.Show(
                $"Uninstalled {installed.Count} installed mod link(s) and saved the list to\r\n{backupPath}\r\n\r\nYou can now change the SteamCmd data location, then press \"Reinstall Saved Mods\" to restore the links.",
                "Uninstall All Installed Mods", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnReinstallAllMods_Click(object sender, EventArgs e)
        {
            string backupPath = Path.GetFullPath(InstalledModsBackupFile);
            if (!File.Exists(backupPath))
            {
                MessageBox.Show($"No saved list of installed mods was found in\r\n{backupPath}\r\n\r\nUse \"Uninstall All Installed Mods\" first - it saves the list there.", "No Saved List", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            InstalledModsBackup backup;
            try
            {
                backup = JsonConvert.DeserializeObject<InstalledModsBackup>(File.ReadAllText(backupPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read {InstalledModsBackupFile}:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (backup?.Mods == null || backup.Mods.Count == 0)
            {
                MessageBox.Show($"The saved list in\r\n{backupPath}\r\ncontains no mods.", "No Saved List", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int restored = 0;
            List<string> notRestored = new List<string>();
            int counter = 0;

            TaskControl reinstallTask = AddTask($"Reinstalling {backup.Mods.Count} saved mod link(s)", backup.Mods.Count);
            await Task.Run(async () =>
            {
                foreach (InstalledModsBackupEntry entry in backup.Mods)
                {
                    if (await ReinstallModLinkAsync(entry))
                        restored++;
                    else
                        notRestored.Add($"{entry.ModUniqueId} ({entry.InstallTarget})");

                    reinstallTask.Value = ++counter;
                }
            });
            EndTask(reinstallTask);

            RefreshModListsUI();

            Log($"Reinstalled {restored} of {backup.Mods.Count} saved mod link(s)");
            string message = $"Reinstalled {restored} of {backup.Mods.Count} saved mod link(s).";
            if (notRestored.Count > 0)
                message += $"\r\n\r\nNot reinstalled:\r\n{string.Join("\r\n", notRestored)}";
            MessageBox.Show(message, "Reinstall Saved Mods", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// Restores a single saved mod link. Refreshes the mod list once if the mod is not
        /// known yet (e.g. the lists were not loaded, or the SteamCmd data location just
        /// changed and the mods reappeared in their new place).
        /// </summary>
        private async Task<bool> ReinstallModLinkAsync(InstalledModsBackupEntry entry)
        {
            ModItemBase mod = null;

            await ModsLock.WaitAsync();
            try
            {
                Dictionary<string, ModItemBase> dict;
                if (Mods.TryGetValue(entry.AppId, out dict))
                    dict.TryGetValue(entry.ModUniqueId, out mod);
            }
            finally
            {
                ModsLock.Release();
            }

            if (mod == null)
            {
                if (entry.AppId == AppIdBZ98)
                    await UpdateBZ98RModListsAsync();
                else if (entry.AppId == AppIdBZCC)
                    await UpdateBZCCModListsAsync();

                await ModsLock.WaitAsync();
                try
                {
                    Dictionary<string, ModItemBase> dict;
                    if (Mods.TryGetValue(entry.AppId, out dict))
                        dict.TryGetValue(entry.ModUniqueId, out mod);
                }
                finally
                {
                    ModsLock.Release();
                }
            }

            if (mod == null)
                return false;

            try
            {
                if (entry.InstallTarget == "GOG")
                {
                    if (mod.InstalledGog != InstallStatus.Linked)
                        mod.ToggleGog();
                    return mod.InstalledGog == InstallStatus.Linked;
                }

                if (mod.InstalledSteam != InstallStatus.Linked)
                    mod.ToggleSteam();
                return mod.InstalledSteam == InstallStatus.Linked;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Moves the SteamCmd data directory from one location to another. The launcher
        /// executable is left in place (SteamVent always launches steamcmd\steamcmd.exe
        /// from the application directory); everything else (steamapps, etc.) is moved.
        /// </summary>
        private static void MoveSteamCmdData(string sourceRoot, string destinationRoot)
        {
            if (!Directory.Exists(sourceRoot))
                return;

            Directory.CreateDirectory(destinationRoot);

            // only the content files need to move
            string dir = Path.Combine(sourceRoot, "steamapps", "workshop");
            string dest = Path.Combine(destinationRoot, "steamapps", "workshop");
            if (Directory.Exists(dir))
                MoveDirectory(dir, dest);
        }

        private static void MoveDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (string file in Directory.EnumerateFiles(source))
            {
                string dest = Path.Combine(destination, Path.GetFileName(file));
                if (File.Exists(dest)) File.Delete(dest);
                try
                {
                    File.Move(file, dest);
                }
                catch (IOException)
                {
                    File.Copy(file, dest, true); // cross-volume
                    File.Delete(file);
                }
            }

            foreach (string dir in Directory.EnumerateDirectories(source))
                MoveDirectory(dir, Path.Combine(destination, Path.GetFileName(dir)));

            Directory.Delete(source);
        }
    }
}
