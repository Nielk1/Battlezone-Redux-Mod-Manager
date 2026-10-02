using System;
using System.Collections.Generic;

namespace BZRModManager.Audit
{
    /// <summary>
    /// Checks the manager's mod lists against the remote audit database.
    /// Produces actionable items: "notify the mod developer" for broken mods,
    /// "use the replacement instead" for superseded/deprecated mods.
    /// </summary>
    public class RemoteDataEngine : AuditEngine
    {
        public override string Name => "Mod Status";
        public override string Description => "checks mods against the remote audit database (broken / superseded / deprecated)";

        protected override List<AuditItem> Execute(AuditContext context)
        {
            List<AuditItem> items = new List<AuditItem>();
            int progress = 0;

            foreach (AuditedMod mod in context.Mods)
            {
                progress++;
                context.Report(progress);

                if (string.IsNullOrWhiteSpace(mod.WorkshopId))
                    continue;

                if (!context.Remote.TryGetValue(mod.AppId, out Dictionary<string, RemoteAuditEntry> data) || data == null)
                    continue;

                if (!data.TryGetValue(mod.WorkshopId, out RemoteAuditEntry entry) || entry == null)
                    continue;

                AuditItem item = BuildItem(mod, entry);
                if (item != null)
                    items.Add(item);
            }

            return items;
        }

        private static AuditItem BuildItem(AuditedMod mod, RemoteAuditEntry entry)
        {
            string status = (entry.Status ?? string.Empty).Trim();
            if (status.Length == 0)
                return null;

            string game = AuditGames.Name(mod.AppId);
            string modLink = AuditItem.WorkshopLink(mod.WorkshopId);
            string replacementLink = AuditItem.WorkshopLink(entry.NewID);

            if (status.Equals("Broken", StringComparison.OrdinalIgnoreCase))
            {
                return new AuditItem
                {
                    Game = game,
                    Severity = AuditSeverity.Error,
                    Action = AuditAction.InformModDeveloper,
                    Summary = $"mod \"{mod.Name}\" ({mod.WorkshopId}) is known broken",
                    ModId = mod.WorkshopId,
                    ModName = mod.Name,
                    Path = mod.FilePath,
                    Detail = Note(entry.Note),
                    Links = { modLink }
                };
            }

            if (status.Equals("Superseded", StringComparison.OrdinalIgnoreCase)
                || status.Equals("Deprecated", StringComparison.OrdinalIgnoreCase))
            {
                AuditItem item = new AuditItem
                {
                    Game = game,
                    Severity = AuditSeverity.Warning,
                    Action = AuditAction.UseReplacementMod,
                    Summary = $"mod \"{mod.Name}\" ({mod.WorkshopId}) is {status.ToLowerInvariant()}" +
                              (string.IsNullOrWhiteSpace(entry.NewID) ? string.Empty : $" - replaced by {entry.NewID}"),
                    ModId = mod.WorkshopId,
                    ModName = mod.Name,
                    Path = mod.FilePath,
                    Detail = Note(entry.Note) ?? (string.IsNullOrWhiteSpace(entry.NewID) ? null : $"Replacement: {entry.NewID}"),
                    Links = { modLink }
                };

                if (replacementLink != null)
                    item.Links.Add(replacementLink);

                return item;
            }

            // Unknown status: report it informationally so a new database entry
            // type is visible in the log without being hidden.
            return new AuditItem
            {
                Game = game,
                Severity = AuditSeverity.Information,
                Action = AuditAction.None,
                Summary = $"mod \"{mod.Name}\" ({mod.WorkshopId}) is listed as \"{status}\"",
                ModId = mod.WorkshopId,
                ModName = mod.Name,
                Path = mod.FilePath,
                Detail = Note(entry.Note),
                Links = { modLink }
            };
        }

        private static string Note(string note)
        {
            return string.IsNullOrWhiteSpace(note) ? null : $"Note: {note.Trim()}";
        }
    }
}