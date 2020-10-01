using Monitor.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ModItem
{
    public class GitMod : ModItemBase
    {
        public GitModStatus Workshop { get; set; }

        public override string UniqueID { get { return GetUniqueId(Workshop.ModWorkshopId); } }

        public override InstallStatus InstalledSteam
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(workshopFolder, Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return InstallStatus.Uninstalled;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) == sourceFolder) return InstallStatus.Linked;
                        return InstallStatus.Collision;
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(workshopFolder, Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return InstallStatus.Uninstalled;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) == sourceFolder) return InstallStatus.Linked;
                        return InstallStatus.Collision;
                    }
                }
                return InstallStatus.Uninstalled;
            }
        } // TODO: Dynamic
        public override InstallStatus InstalledGog
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return InstallStatus.Uninstalled;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) == sourceFolder) return InstallStatus.Linked;
                        return InstallStatus.Collision;
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return InstallStatus.Uninstalled;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) == sourceFolder) return InstallStatus.Linked;
                        return InstallStatus.Collision;
                    }
                }
                return InstallStatus.Uninstalled;
            }
        } // TODO: Dynamic

        public override string ModType
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    bool hadError;
                    string[] ModTypes = BZ98RTools.GetModTypes(Workshop.ModPath, out hadError);
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
                        string ModType = BZCCTools.GetModType(Workshop.ModPath, Workshop.ModWorkshopId);
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
                    string[] ModTags = BZ98RTools.GetModTags(Workshop.ModPath);
                    return ModTags;
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    string[] ModTags = BZCCTools.GetModTags(Workshop.ModPath, Workshop.ModWorkshopId);
                    return ModTags;
                }
                return new string[] { "UNKNON" };
            }
        }

        public override string WorkshopIdOutput { get { return Workshop.ModWorkshopId.ToString(); } }
        public override string ModSource { get { return "Git"; } }

        public override string FilePath
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98 || AppId == MainForm.AppIdBZCC)
                    return Path.GetFullPath(Workshop.ModPath);
                return null;
            }
        }

        public static string GetUniqueId(string modWorkshopId)
        {
            return modWorkshopId + "-Git";
        }

        public GitMod(int AppId, GitModStatus Workshop)
        {
            this.AppId = AppId;
            this.Workshop = Workshop;
        }

        public override void ToggleGog()
        {
            if (InstalledGog == InstallStatus.Uninstalled)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", Workshop.ModWorkshopId);

                        if (Directory.Exists(destinationFolder)) return;
                        JunctionPoint.Create(destinationFolder, sourceFolder, false);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", Workshop.ModWorkshopId);

                        if (Directory.Exists(destinationFolder)) return;
                        JunctionPoint.Create(destinationFolder, sourceFolder, false);
                    }
                }
            }
            else if (InstalledGog == InstallStatus.Linked)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
            }
        }
        public override void ToggleSteam()
        {
            if (InstalledSteam == InstallStatus.Uninstalled)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(workshopFolder, Workshop.ModWorkshopId);

                        if (Directory.Exists(destinationFolder)) return;
                        JunctionPoint.Create(destinationFolder, sourceFolder, false);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(workshopFolder, Workshop.ModWorkshopId);

                        if (Directory.Exists(destinationFolder)) return;
                        JunctionPoint.Create(destinationFolder, sourceFolder, false);
                    }
                }
            }
            else if (InstalledSteam == InstallStatus.Linked)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(workshopFolder, Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string sourceFolder = Path.GetFullPath(Workshop.ModPath);
                        string destinationFolder = Path.Combine(workshopFolder, Workshop.ModWorkshopId);

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Workshop.ModName)) return Workshop.ModName;
            if (AppId == MainForm.AppIdBZ98)
            {
                if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                {
                    string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                    bool hadError;
                    string[] ModNames = BZ98RTools.GetModNames(Path.Combine(workshopFolder, Workshop.ModWorkshopId), out hadError);
                    if (ModNames?.Length > 0)
                    {
                        return (hadError ? "!" : string.Empty) + string.Join(" / ", ModNames);
                    }
                    if (hadError)
                    {
                        return Workshop.ModWorkshopId + " (PARSE ERROR)";
                    }
                }
            }
            if (AppId == MainForm.AppIdBZCC)
            {
                if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                {
                    try
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string ModName = BZCCTools.GetModName(Path.Combine(workshopFolder, Workshop.ModWorkshopId));
                        if (ModName != null) return ModName;
                    }
                    catch
                    {
                        return Workshop.ModWorkshopId + " (PARSE ERROR)";
                    }
                }
            }
            return "UNKNOWN MOD";
        }

        public override bool Delete()
        {
            if (Directory.Exists(Path.GetDirectoryName(FilePath)))
            {
                Directory.Delete(Path.GetDirectoryName(FilePath), true);
                return true;
            }
            return false;
        }
    }
}
