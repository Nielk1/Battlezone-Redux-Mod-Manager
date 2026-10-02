using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BZRModManager.Audit
{
    /// <summary>
    /// Checks installed BZCC mods for missing asset dependencies. Every finding
    /// is an "install the missing dependency" item with workshop links for the
    /// missing asset mods.
    /// </summary>
    public class DependencyEngine : AuditEngine
    {
        public override string Name => "BZCC Dependencies";
        public override string Description => "checks installed BZCC mods for missing asset dependencies";

        protected override List<AuditItem> Execute(AuditContext context)
        {
            List<AuditItem> items = new List<AuditItem>();
            int progress = 0;

            foreach (ModFolder folder in context.FoldersFor(MainForm.AppIdBZCC))
            {
                if (!Directory.Exists(folder.Path))
                    continue;

                string[] dirs;
                try
                {
                    dirs = Directory.EnumerateDirectories(folder.Path).ToArray();
                }
                catch
                {
                    continue; // the INI integrity audit already reports unscannable folders
                }

                foreach (string dir in dirs)
                {
                    progress++;
                    context.Report(progress);

                    string folderName = Path.GetFileName(dir);
                    string[] dependencies = BZCCTools.GetAssetDependencies(dir);
                    if (dependencies == null || dependencies.Length == 0)
                        continue;

                    HashSet<string> installed = new HashSet<string>(dirs.Select(dx => Path.GetFileName(dx)));
                    List<string> missing = dependencies.Where(dx => !string.IsNullOrWhiteSpace(dx) && !installed.Contains(dx.Trim())).ToList();
                    if (missing.Count == 0)
                        continue;

                    var item = new AuditItem
                    {
                        Game = folder.Game,
                        Severity = AuditSeverity.Warning,
                        Action = AuditAction.InstallMissingDependency,
                        Summary = $"installed mod \"{folderName}\" is missing {missing.Count} asset dependency(ies)",
                        ModId = folderName,
                        Path = dir,
                        Detail = string.Join(", ", missing),
                        Links = { AuditItem.WorkshopLink(folderName) }
                    };

                    foreach (string dep in missing)
                    {
                        string link = AuditItem.WorkshopLink(dep.Trim());
                        if (link != null)
                            item.Links.Add(link);
                    }

                    items.Add(item);
                }
            }

            return items;
        }
    }
}