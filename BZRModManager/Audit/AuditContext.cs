using System;
using System.Collections.Generic;
using System.Linq;

namespace BZRModManager.Audit
{
    /// <summary>
    /// Everything an audit engine needs to run: a snapshot of the manager's mod
    /// lists, the game mod folders that are configured, the remote audit data
    /// and a progress callback. Built once per audit run in MainForm.
    /// </summary>
    public sealed class AuditContext
    {
        /// <summary>Snapshot of the mods in the manager's lists (under the mods lock).</summary>
        public List<AuditedMod> Mods { get; } = new List<AuditedMod>();

        /// <summary>The configured game mod folders (Steam workshop folders, GoG mod folders, ...).</summary>
        public List<ModFolder> ModFolders { get; } = new List<ModFolder>();

        /// <summary>Remote per-game audit data, keyed by app id (empty when the download failed).</summary>
        public Dictionary<int, Dictionary<string, RemoteAuditEntry>> Remote { get; } = new Dictionary<int, Dictionary<string, RemoteAuditEntry>>();

        /// <summary>Optional progress callback, invoked with a monotonically increasing count per engine.</summary>
        public Action<int> Progress { get; set; }

        /// <summary>Report progress without throwing when no progress handler is attached.</summary>
        public void Report(int progress)
        {
            Progress?.Invoke(progress);
        }

        public IEnumerable<AuditedMod> ModsFor(int appId)
        {
            return Mods.Where(dr => dr.AppId == appId);
        }

        public IEnumerable<ModFolder> FoldersFor(int appId)
        {
            return ModFolders.Where(dr => dr.AppId == appId);
        }
    }
}