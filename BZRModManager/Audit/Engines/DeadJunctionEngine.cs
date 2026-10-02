using Monitor.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BZRModManager.Audit
{
    /// <summary>
    /// Finds dead junction points in a game's target mod folders: junctions whose
    /// target folder no longer exists (the mod content was moved, renamed or
    /// deleted out from under the link, or the manager was moved).
    /// Each finding is a "remove the dead junction" item.
    /// </summary>
    public class DeadJunctionEngine : AuditEngine
    {
        public override string Name => "Dead Junctions";
        public override string Description => "finds junctions in the game mod folders that point to a missing folder";

        protected override List<AuditItem> Execute(AuditContext context)
        {
            List<AuditItem> items = new List<AuditItem>();
            int progress = 0;

            foreach (ModFolder folder in context.ModFolders)
            {
                if (!Directory.Exists(folder.Path))
                    continue;

                string[] dirs;
                try
                {
                    // A dead junction still shows up in the directory listing
                    // (its name is there, only its target is gone), so this is
                    // exactly the set we need to walk.
                    dirs = Directory.EnumerateDirectories(folder.Path).ToArray();
                }
                catch (Exception ex)
                {
                    items.Add(new AuditItem
                    {
                        Game = folder.Game,
                        Severity = AuditSeverity.Warning,
                        Action = AuditAction.None,
                        Summary = $"could not scan {folder.Platform} mod folder for junctions: {ex.Message}",
                        Path = folder.Path
                    });
                    continue;
                }

                foreach (string dir in dirs)
                {
                    progress++;
                    context.Report(progress);

                    string name = Path.GetFileName(dir);

                    bool isJunction;
                    try
                    {
                        isJunction = JunctionPoint.Exists(dir);
                    }
                    catch
                    {
                        // Not a reparse point we can read (or no permission): not our problem here.
                        continue;
                    }
                    if (!isJunction)
                        continue;

                    string target;
                    try
                    {
                        target = JunctionPoint.GetTarget(dir);
                    }
                    catch
                    {
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(target))
                        continue;

                    if (Directory.Exists(target) || File.Exists(target))
                        continue; // the junction is alive

                    items.Add(new AuditItem
                    {
                        Game = folder.Game,
                        Severity = AuditSeverity.Error,
                        Action = AuditAction.RemoveDeadLink,
                        Summary = $"junction \"{name}\" points to a folder that no longer exists",
                        ModId = name,
                        Path = dir,
                        Detail = $"Target: {target}",
                        Links = { AuditItem.WorkshopLink(name) }
                    });
                }
            }

            return items;
        }
    }
}