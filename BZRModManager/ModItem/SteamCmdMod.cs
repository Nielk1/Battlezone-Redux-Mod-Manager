using Monitor.Core.Utilities;
using SteamVent.Common;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ModItem
{
    public class SteamCmdMod : ModItemBase
    {
        public WorkshopItemStatus Workshop { get; set; }

        public override string UniqueID { get { return GetUniqueId(Workshop.WorkshopId); } }
        public static string GetUniqueId(UInt64 workshopId) { return workshopId.ToString().PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-SteamCmd"; }

        public override InstallStatus InstalledSteam { get { return InstallStatus.ForceDisabled; } } // forced
        public override InstallStatus InstalledGog
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", Workshop.WorkshopId.ToString());

                        if (!Directory.Exists(destinationFolder)) return InstallStatus.Uninstalled;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) == sourceFolder) return InstallStatus.Linked;
                        return InstallStatus.Collision;
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", Workshop.WorkshopId.ToString());

                        if (!Directory.Exists(destinationFolder)) return InstallStatus.Uninstalled;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) == sourceFolder) return InstallStatus.Linked;
                        return InstallStatus.Collision;
                    }
                }
                return InstallStatus.Unknown;
            }
        } // TODO: Dynamic

        public override string ModType
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    bool hadError;
                    string[] ModTypes = BZ98RTools.GetModTypes(Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}"), out hadError);
                    if (ModTypes?.Length > 0)
                    {
                        return (hadError ? "!" : string.Empty) + string.Join(", ", ModTypes);
                    }
                    if (hadError)
                    {
                        return "PARSE ERROR";
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    try
                    {
                        string ModType = BZCCTools.GetModType(Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}"));
                        if (ModType != null) return ModType;
                    }
                    catch
                    {
                        return "PARSE ERROR";
                    }
                }
                return "UNKNOWN";
            }
        }
        public override string[] ModTags
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    string[] ModTags = BZ98RTools.GetModTags(Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}"));
                    return ModTags;
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    string[] ModTags = BZCCTools.GetModTags(Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}"));
                    return ModTags;
                }
                return new string[] { "UNKNON" };
            }
        }

        public override string WorkshopIdOutput { get { return Workshop.WorkshopId.ToString(); } }
        public override string ModSource { get { return "SteamCmd"; } }

        public override string FilePath
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98 || AppId == MainForm.AppIdBZCC)
                    return Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                return null;
            }
        }

        public SteamCmdMod(int AppId, WorkshopItemStatus Workshop)
        {
            this.AppId = AppId;
            this.Workshop = Workshop;
        }

        public override string ManagerName
        {
            get
            {
                string contentPath = Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                if (AppId == MainForm.AppIdBZ98)
                {
                    string[] names = BZ98RTools.GetModManagerNames(contentPath, out _);
                    return names?.Length > 0 ? string.Join(" / ", names) : null;
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    return BZCCTools.GetModManagerName(contentPath);
                }
                return null;
            }
        }

        public override string GeneratedName
        {
            get
            {

            if (AppId == MainForm.AppIdBZ98)
            {
                bool hadError;
                string[] ModNames = BZ98RTools.GetModMissionNames(Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}"), out hadError);
                if (ModNames?.Length > 0)
                {
                    return (hadError ? "!" : string.Empty) + string.Join(" / ", ModNames);
                }
                if (hadError)
                {
                    return Workshop.WorkshopId + " (PARSE ERROR)";
                }

            }
            if (AppId == MainForm.AppIdBZCC)
            {
                try
                {
                    string ModName = BZCCTools.GetGeneratedName(Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}"));
                    if (ModName != null) return ModName;
                }
                catch
                {
                    return Workshop.WorkshopId + " (PARSE ERROR)";
                }
            }
            return UniqueID;
            }
        }

        public override string GetWorkshopId()
        {
            return Workshop != null ? Workshop.WorkshopId.ToString() : null;
        }

        public override void ToggleGog()
        {
            if (InstalledGog == InstallStatus.Uninstalled)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", Workshop.WorkshopId.ToString());

                        if (Directory.Exists(destinationFolder)) return;
                        try
                        {
                            JunctionPoint.Create(destinationFolder, sourceFolder, false);
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", Workshop.WorkshopId.ToString());

                        if (Directory.Exists(destinationFolder)) return;
                        try
                        {
                            JunctionPoint.Create(destinationFolder, sourceFolder, false);
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
            }
            else if (InstalledGog == InstallStatus.Linked)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", Workshop.WorkshopId.ToString());

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.Combine(SteamContext.SteamCmdRoot, $"steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", Workshop.WorkshopId.ToString());

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
            }
        }
        public override void ToggleSteam()
        {
            //if (InstalledSteam == InstallStatus.Uninstalled) { }
            //ListViewItemCache = null;
        }

        /*public bool Exists()
        {
            return Directory.Exists(Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}"));
        }*/

        public override bool Delete()
        {
            if (Directory.Exists(FilePath))
            {
                Directory.Delete(FilePath, true);
                return true;
            }
            return false;
        }
    }
}
