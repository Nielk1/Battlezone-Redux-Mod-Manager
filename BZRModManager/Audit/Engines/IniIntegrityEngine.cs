using IniParser;
using IniParser.Configuration;
using Monitor.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BZRModManager.Audit
{
    /// <summary>
    /// Checks mod INI integrity for both downloaded mods (from the mod lists) and
    /// installed mod folders. Bad, missing or mis-named inis are the mod
    /// developer's problem, so every finding is an "inform mod developer" item.
    /// For BZCC the INI must be named after the mod's own id: a "Mod.ini" (the
    /// game's fallback name) is only acceptable for a non-workshop dev mod.
    /// </summary>
    public class IniIntegrityEngine : AuditEngine
    {
        public override string Name => "INI Integrity";
        public override string Description => "checks mod INI files for parse errors or missing INIs";

        protected override List<AuditItem> Execute(AuditContext context)
        {
            List<AuditItem> items = new List<AuditItem>();
            int progress = 0;

            // 1) Downloaded mods in the manager's lists.
            foreach (AuditedMod mod in context.Mods)
            {
                progress++;
                context.Report(progress);

                AuditItem item = CheckDownloadedMod(mod);
                if (item != null)
                    items.Add(item);
            }

            // 2) Installed mod folders per game.
            foreach (ModFolder folder in context.ModFolders)
            {
                if (!Directory.Exists(folder.Path))
                    continue;

                string[] dirs;
                try
                {
                    dirs = Directory.EnumerateDirectories(folder.Path).ToArray();
                }
                catch (Exception ex)
                {
                    items.Add(new AuditItem
                    {
                        Game = folder.Game,
                        Severity = AuditSeverity.Warning,
                        Action = AuditAction.None,
                        Summary = $"could not scan {folder.Platform} mod folder: {ex.Message}",
                        Path = folder.Path
                    });
                    continue;
                }

                var parser = IniTools.CreateParser();

                foreach (string dir in dirs)
                {
                    progress++;
                    context.Report(progress);

                    string folderName = Path.GetFileName(dir);

                    // A junction whose target is gone cannot be inspected at all; the
                    // Dead Junctions audit reports it instead.
                    try
                    {
                        if (JunctionPoint.Exists(dir))
                        {
                            string target = JunctionPoint.GetTarget(dir);
                            if (!string.IsNullOrWhiteSpace(target) && !Directory.Exists(target))
                                continue;
                        }
                    }
                    catch { }

                    if (folder.AppId == MainForm.AppIdBZCC)
                    {
                        string idLink = AuditItem.WorkshopLink(folderName);
                        string iniFile = Path.Combine(dir, folderName + ".ini");

                        if (File.Exists(iniFile))
                        {
                            try
                            {
                                parser.Parse(File.ReadAllText(iniFile));
                            }
                            catch
                            {
                                items.Add(new AuditItem
                                {
                                    Game = folder.Game,
                                    Severity = AuditSeverity.Warning,
                                    Action = AuditAction.InformModDeveloper,
                                    Summary = $"installed mod \"{folderName}\" has an unreadable INI",
                                    ModId = folderName,
                                    Path = dir,
                                    Detail = $"INI: {Path.GetFileName(iniFile)}",
                                    Links = { idLink }
                                });
                            }
                        }
                        else
                        {
                            // The INI must be named after the mod's own id. A "Mod.ini"
                            // (the game's and manager's fallback name) is tolerated for
                            // a non-workshop dev mod, but for a workshop mod it is an
                            // error the mod developer must fix.
                            bool hasFallbackIni = File.Exists(Path.Combine(dir, "Mod.ini"));
                            if (!WorkshopIds.IsValid(folderName) && hasFallbackIni)
                                continue; // a dev mod with a plain Mod.ini is fine

                            items.Add(new AuditItem
                            {
                                Game = folder.Game,
                                Severity = AuditSeverity.Warning,
                                Action = AuditAction.InformModDeveloper,
                                Summary = WorkshopIds.IsValid(folderName)
                                    ? $"installed mod \"{folderName}\" is missing its \"{folderName}.ini\""
                                    : $"installed mod \"{folderName}\" has no INI",
                                ModId = folderName,
                                Path = dir,
                                Detail = hasFallbackIni
                                    ? "A Mod.ini fallback INI was found, but the INI must be named after the mod's own id"
                                    : "No INI file was found in the mod folder",
                                Links = { idLink }
                            });
                        }
                    }
                    else if (folder.AppId == MainForm.AppIdBZ98)
                    {
                        List<string> badInis = new List<string>();
                        string[] iniFiles;
                        try
                        {
                            iniFiles = Directory.EnumerateFiles(dir, "*.ini", SearchOption.TopDirectoryOnly).ToArray();
                        }
                        catch (Exception ex)
                        {
                            items.Add(new AuditItem
                            {
                                Game = folder.Game,
                                Severity = AuditSeverity.Warning,
                                Action = AuditAction.None,
                                Summary = $"could not read INIs of installed mod \"{folderName}\": {ex.Message}",
                                ModId = folderName,
                                Path = dir,
                                Links = { AuditItem.WorkshopLink(folderName) }
                            });
                            continue;
                        }

                        if (iniFiles.Length == 0)
                        {
                            items.Add(new AuditItem
                            {
                                Game = folder.Game,
                                Severity = AuditSeverity.Warning,
                                Action = AuditAction.InformModDeveloper,
                                Summary = $"installed mod \"{folderName}\" has no INI files at all",
                                ModId = folderName,
                                Path = dir,
                                Links = { AuditItem.WorkshopLink(folderName) }
                            });
                            continue;
                        }

                        foreach (string iniFile in iniFiles)
                        {
                            try
                            {
                                parser.Parse(File.ReadAllText(iniFile));
                            }
                            catch
                            {
                                badInis.Add(Path.GetFileName(iniFile));
                            }
                        }

                        if (badInis.Count > 0)
                        {
                            items.Add(new AuditItem
                            {
                                Game = folder.Game,
                                Severity = AuditSeverity.Warning,
                                Action = AuditAction.InformModDeveloper,
                                Summary = $"installed mod \"{folderName}\" has {badInis.Count} unreadable INI file(s)",
                                ModId = folderName,
                                Path = dir,
                                Detail = string.Join(", ", badInis),
                                Links = { AuditItem.WorkshopLink(folderName) }
                            });
                        }
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Flags a downloaded mod whose INIs are missing or unreadable (the
        /// "!" / "PARSE ERROR" / "UNKNOWN" mod type markers), or - for BZCC -
        /// whose INI is only present as a "Mod.ini" fallback instead of the
        /// required id-prefixed name. All of these are the mod author's problem.
        /// </summary>
        private static AuditItem CheckDownloadedMod(AuditedMod mod)
        {
            if (mod.AppId == MainForm.AppIdBZ98)
            {
                if (mod.ModType == null || (!mod.ModType.Contains("!") && !mod.ModType.Contains("PARSE ERROR")))
                    return null;

                return new AuditItem
                {
                    Game = "BZ98R",
                    Severity = AuditSeverity.Warning,
                    Action = AuditAction.InformModDeveloper,
                    Summary = $"mod \"{mod.Name}\" ({mod.WorkshopId}) has INI parse errors",
                    ModId = mod.WorkshopId,
                    ModName = mod.Name,
                    Path = mod.FilePath,
                    Links = { AuditItem.WorkshopLink(mod.WorkshopId) }
                };
            }

            if (mod.AppId == MainForm.AppIdBZCC)
            {
                bool missingOrBroken = mod.ModType == null
                    || mod.ModType.Contains("UNKNOWN")
                    || mod.ModType.Contains("PARSE ERROR");

                // The INI must be named after the mod's own id. A "Mod.ini" is
                // found and read by the manager (and by the game), but it is
                // still something the mod developer must fix.
                bool misnamed = WorkshopIds.IsValid(mod.WorkshopId)
                    && Directory.Exists(mod.FilePath)
                    && !File.Exists(Path.Combine(mod.FilePath, mod.WorkshopId + ".ini"));

                if (!missingOrBroken && !misnamed)
                    return null;

                return new AuditItem
                {
                    Game = "BZCC",
                    Severity = AuditSeverity.Warning,
                    Action = AuditAction.InformModDeveloper,
                    Summary = missingOrBroken
                        ? $"mod \"{mod.Name}\" ({mod.WorkshopId}) has a missing or unreadable mod INI"
                        : $"mod \"{mod.Name}\" ({mod.WorkshopId}) ships a Mod.ini instead of its \"{mod.WorkshopId}.ini\"",
                    ModId = mod.WorkshopId,
                    ModName = mod.Name,
                    Path = mod.FilePath,
                    Detail = missingOrBroken ? null : "The INI must be named after the mod's own id",
                    Links = { AuditItem.WorkshopLink(mod.WorkshopId) }
                };
            }

            return null;
        }
    }

    /// <summary>
    /// The lenient INI parser configuration the whole manager audits with
    /// (same settings as the mod list building and the old audit code).
    /// </summary>
    public static class IniTools
    {
        public static IniDataParser CreateParser()
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;
            return parser;
        }
    }
}