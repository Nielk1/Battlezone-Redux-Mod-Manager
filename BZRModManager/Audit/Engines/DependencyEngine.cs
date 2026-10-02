using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BZRModManager.Audit
{
    /// <summary>
    /// Checks installed BZCC mods for asset dependency problems, split by who has
    /// to fix them:
    ///  - a dependency missing from the local install is a *user* problem - the
    ///    item says to install it and carries the workshop link;
    ///  - a dependency that is no longer available on the workshop, or that is
    ///    not even a valid workshop id, is a *mod developer* problem;
    ///  - a mod without a workshop id (a dev/in-dev mod) is assumed to have valid
    ///    non-workshop dependencies and is never checked against the workshop.
    /// </summary>
    public class DependencyEngine : AuditEngine
    {
        public override string Name => "BZCC Dependencies";
        public override string Description => "checks installed BZCC mods for invalid or missing asset dependencies";

        private enum Availability
        {
            Unknown,
            Available,
            Unavailable
        }

        private sealed class ModDependencies
        {
            public string Dir;
            public string ModId;
            // Dependencies that are not valid workshop ids (the mod developer's problem).
            public List<string> Invalid = new List<string>();
            // Valid workshop ids that are not installed locally (availability must be checked).
            public List<string> Missing = new List<string>();
        }

        protected override List<AuditItem> Execute(AuditContext context)
        {
            List<AuditItem> items = new List<AuditItem>();
            List<ModDependencies> mods = new List<ModDependencies>();
            int progress = 0;

            // A dependency counts as installed when its folder exists in any of the
            // local BZCC mod folders (the Steam workshop folder or the GoG folder).
            HashSet<string> installed = new HashSet<string>();
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
                    installed.Add(Path.GetFileName(dir));
            }

            // First pass: classify every dependency using only local information.
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
                    continue;
                }

                foreach (string dir in dirs)
                {
                    progress++;
                    context.Report(progress);

                    string modId = Path.GetFileName(dir);
                    string[] dependencies = BZCCTools.GetAssetDependencies(dir);
                    if (dependencies == null || dependencies.Length == 0)
                        continue;

                    // Only workshop mods are held to workshop rules: a dev mod's
                    // dependencies (in-dev asset mods, custom names, ...) are safe
                    // to assume valid.
                    bool isWorkshopMod = WorkshopIds.IsValid(modId);

                    ModDependencies mod = new ModDependencies { Dir = dir, ModId = modId };
                    foreach (string raw in dependencies)
                    {
                        string dep = raw?.Trim();
                        if (string.IsNullOrWhiteSpace(dep))
                            continue;

                        if (WorkshopIds.IsValid(dep))
                        {
                            if (!installed.Contains(dep) && !mod.Missing.Contains(dep))
                                mod.Missing.Add(dep);
                            continue;
                        }

                        if (isWorkshopMod && !mod.Invalid.Contains(dep))
                            mod.Invalid.Add(dep);
                    }

                    if (mod.Invalid.Count > 0 || mod.Missing.Count > 0)
                        mods.Add(mod);
                }
            }

            // Ask the workshop which of the missing ids still exist. WorkshopContext
            // caches every answer (and negative-caches the "not found" ones), so
            // repeated ids and re-runs stay cheap.
            HashSet<string> checkedIds = new HashSet<string>();
            Dictionary<string, Availability> availability = new Dictionary<string, Availability>();
            foreach (ModDependencies mod in mods)
            {
                foreach (string dep in mod.Missing)
                {
                    if (checkedIds.Contains(dep))
                        continue;
                    checkedIds.Add(dep);

                    progress++;
                    context.Report(progress);
                    availability[dep] = CheckAvailability(dep);
                }
            }

            // Second pass: emit the findings, splitting user problems from mod
            // developer problems.
            foreach (ModDependencies mod in mods)
            {
                if (mod.Invalid.Count > 0)
                {
                    items.Add(new AuditItem
                    {
                        Game = "BZCC",
                        Severity = AuditSeverity.Warning,
                        Action = AuditAction.InformModDeveloper,
                        Summary = $"mod \"{mod.ModId}\" depends on {mod.Invalid.Count} asset(s) that are not valid workshop ids",
                        ModId = mod.ModId,
                        Path = mod.Dir,
                        Detail = string.Join(", ", mod.Invalid),
                        Links = { AuditItem.WorkshopLink(mod.ModId) }
                    });
                }

                List<string> gone = new List<string>();
                List<string> install = new List<string>();
                bool anyUnknown = false;
                foreach (string dep in mod.Missing)
                {
                    availability.TryGetValue(dep, out Availability a);
                    if (a == Availability.Unavailable)
                        gone.Add(dep);
                    else
                    {
                        install.Add(dep);
                        if (a == Availability.Unknown)
                            anyUnknown = true;
                    }
                }

                if (gone.Count > 0)
                {
                    items.Add(new AuditItem
                    {
                        Game = "BZCC",
                        Severity = AuditSeverity.Warning,
                        Action = AuditAction.InformModDeveloper,
                        Summary = $"mod \"{mod.ModId}\" depends on {gone.Count} asset(s) that are no longer available on the workshop",
                        ModId = mod.ModId,
                        Path = mod.Dir,
                        Detail = string.Join(", ", gone),
                        Links = { AuditItem.WorkshopLink(mod.ModId) }
                    });
                }

                if (install.Count > 0)
                {
                    AuditItem item = new AuditItem
                    {
                        Game = "BZCC",
                        Severity = AuditSeverity.Warning,
                        Action = AuditAction.InstallMissingDependency,
                        Summary = $"mod \"{mod.ModId}\" is missing {install.Count} asset dependency(ies) from the local install",
                        ModId = mod.ModId,
                        Path = mod.Dir,
                        Detail = (anyUnknown ? "workshop availability unverified (offline): " : string.Empty) + string.Join(", ", install),
                        Links = { AuditItem.WorkshopLink(mod.ModId) }
                    };
                    foreach (string dep in install)
                    {
                        string link = AuditItem.WorkshopLink(dep);
                        if (link != null)
                            item.Links.Add(link);
                    }
                    items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Asks the workshop whether a published file id still exists. "Unavailable"
        /// is only the definitive not-found answer (WorkshopContext caches it);
        /// anything that merely cannot be reached is "Unknown" and is reported as
        /// a user problem, with a note that the availability could not be verified.
        /// </summary>
        private static Availability CheckAvailability(string id)
        {
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20)))
                {
                    WorkshopMod mod = WorkshopContext.GetItemAsync(id, 0, cts.Token).GetAwaiter().GetResult();
                    return mod == null ? Availability.Unavailable : Availability.Available;
                }
            }
            catch (OperationCanceledException)
            {
                return Availability.Unknown;
            }
            catch
            {
                // Transient network or response problems: unknown is the honest answer.
                return Availability.Unknown;
            }
        }
    }
}