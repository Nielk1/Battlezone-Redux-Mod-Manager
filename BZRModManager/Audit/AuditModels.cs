using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BZRModManager.Audit
{
    /// <summary>
    /// How severe an audit finding is.
    /// </summary>
    public enum AuditSeverity
    {
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// The recommended response to an audit finding. This is what makes an
    /// audit item *actionable*: some items just need the mod developer to be
    /// informed, others say "this mod is superseded/deprecated, use that one
    /// instead", and so on.
    /// </summary>
    public enum AuditAction
    {
        /// <summary>Informational only; there is nothing concrete to do about it.</summary>
        None,
        /// <summary>The problem is on the mod author's side; let them know about it.</summary>
        InformModDeveloper,
        /// <summary>The mod is superseded or deprecated; uninstall it and install the replacement instead.</summary>
        UseReplacementMod,
        /// <summary>The mod is missing asset dependencies; install the missing ones.</summary>
        InstallMissingDependency,
        /// <summary>A junction/link is dead (its target no longer exists); remove it.</summary>
        RemoveDeadLink
    }

    public static class AuditActionText
    {
        public static string Label(AuditAction action)
        {
            switch (action)
            {
                case AuditAction.InformModDeveloper:
                    return "notify the mod developer about this";
                case AuditAction.UseReplacementMod:
                    return "this mod is superseded/deprecated - uninstall it and install the replacement instead";
                case AuditAction.InstallMissingDependency:
                    return "install the missing dependency(ies) listed below";
                case AuditAction.RemoveDeadLink:
                    return "delete the dead junction from the game's mod folder (its target no longer exists)";
                default:
                    return null;
            }
        }
    }

    /// <summary>The display name for a game app id.</summary>
    public static class AuditGames
    {
        public static string Name(int appId)
        {
            if (appId == MainForm.AppIdBZ98) return "BZ98R";
            if (appId == MainForm.AppIdBZCC) return "BZCC";
            return appId.ToString();
        }
    }

    /// <summary>
    /// Workshop id validation shared by the audits.
    /// </summary>
    public static class WorkshopIds
    {
        /// <summary>
        /// Workshop ids are 64 bit values, but the ids Steam actually issues are
        /// far smaller. Anything above this ("not even in the valid range") is a
        /// placeholder or typo'd id and can never be a real workshop mod, so it
        /// is classified as invalid without ever being asked of Steam.
        /// </summary>
        public const ulong MaxPlausibleWorkshopId = 999_999_999_999UL;

        /// <summary>True when the text is a positive decimal number (whether or not it is a plausible workshop id).</summary>
        public static bool IsNumeric(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;
            return ulong.TryParse(id.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>True when the id is a plausible live workshop id (numeric, non-zero, in range).</summary>
        public static bool IsValid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;
            return ulong.TryParse(id.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out ulong n)
                && n > 0 && n <= MaxPlausibleWorkshopId;
        }
    }

    /// <summary>
    /// A single actionable finding produced by an audit engine.
    /// </summary>
    public sealed class AuditItem
    {
        public string Game { get; set; }
        /// <summary>One-line description of the problem.</summary>
        public string Summary { get; set; }
        public AuditSeverity Severity { get; set; }
        /// <summary>The recommended response; <see cref="AuditAction.None"/> for informational findings.</summary>
        public AuditAction Action { get; set; }

        /// <summary>Workshop id of the mod involved, when there is one.</summary>
        public string ModId { get; set; }
        public string ModName { get; set; }
        /// <summary>Relevant local path (mod folder, junction, ...), when there is one.</summary>
        public string Path { get; set; }
        /// <summary>Extra detail specific to the finding (bad ini files, missing targets, ...).</summary>
        public string Detail { get; set; }
        /// <summary>Clickable links (workshop pages, replacements, missing dependencies, ...).</summary>
        public List<string> Links { get; } = new List<string>();

        /// <summary>A Steam Workshop page link for a numeric id, or null when the id is not a workshop id.</summary>
        public static string WorkshopLink(string id)
        {
            if (id != null && ulong.TryParse(id, out ulong n) && n != 0)
                return $"https://steamcommunity.com/workshop/filedetails/?id={n}";
            return null;
        }

        /// <summary>Renders the item the way it is written to the audit log.</summary>
        public string ToText()
        {
            StringBuilder sb = new StringBuilder();

            string tag = Severity == AuditSeverity.Error ? "ERROR  " :
                         Severity == AuditSeverity.Warning ? "WARNING" : "INFO   ";

            sb.Append('[').Append(tag).Append("] ");
            if (!string.IsNullOrWhiteSpace(Game)) sb.Append(Game).Append(": ");
            sb.Append(Summary);

            string action = AuditActionText.Label(Action);
            if (action != null)
                sb.Append("\r\n    Action: ").Append(action);

            if (!string.IsNullOrWhiteSpace(ModId))
                sb.Append("\r\n    ID: ").Append(ModId);

            if (!string.IsNullOrWhiteSpace(ModName))
                sb.Append("\r\n    Name: ").Append(ModName);

            if (!string.IsNullOrWhiteSpace(Path))
                sb.Append("\r\n    Path: ").Append(Path);

            if (!string.IsNullOrWhiteSpace(Detail))
                sb.Append("\r\n    ").Append(Detail);

            foreach (string link in Links)
            {
                if (!string.IsNullOrWhiteSpace(link))
                    sb.Append("\r\n    Link: ").Append(link);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// One entry in the remote per-game audit database (bz98r/audit.json, bzcc/audit.json),
    /// keyed by Steam Workshop id.
    /// </summary>
    public sealed class RemoteAuditEntry
    {
        /// <summary>"Broken", "Superseded" or "Deprecated" (free text, compared case-insensitively).</summary>
        public string Status { get; set; }
        /// <summary>When superseded/deprecated: the workshop id (or name) of the replacement mod.</summary>
        public string NewID { get; set; }
        /// <summary>Optional human-readable note from the audit database (reason, caveat, ...).</summary>
        public string Note { get; set; }
    }

    /// <summary>
    /// A snapshot of one mod from the manager's mod lists, taken under the mods
    /// lock so the audit engines never touch the live UI-bound objects.
    /// </summary>
    public sealed class AuditedMod
    {
        public int AppId { get; set; }
        public string Name { get; set; }
        public string UniqueID { get; set; }
        /// <summary>Steam Workshop id when the mod has one.</summary>
        public string WorkshopId { get; set; }
        /// <summary>The mod's content folder on disk.</summary>
        public string FilePath { get; set; }
        /// <summary>The mod type string as the mod list computed it (carries parse-error markers).</summary>
        public string ModType { get; set; }
    }

    /// <summary>
    /// A game mod folder where mods are installed/injected for a game, i.e. a
    /// place where per-mod folders (and junctions) are expected to live.
    /// </summary>
    public sealed class ModFolder
    {
        /// <summary>"Steam" or "GoG".</summary>
        public string Platform { get; set; }
        /// <summary>"BZ98R" or "BZCC".</summary>
        public string Game { get; set; }
        public int AppId { get; set; }
        public string Path { get; set; }
    }
}