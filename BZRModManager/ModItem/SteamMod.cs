using Monitor.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ModItem
{
    public class SteamMod : ModItemBase
    {
        public UInt64 WorkshopId { get; set; }

        public override string UniqueID { get { return GetUniqueId(WorkshopId); } }
        public static string GetUniqueId(UInt64 workshopId) { return workshopId.ToString().PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-Steam"; }
        public override InstallStatus InstalledSteam
        {
            get
            {
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        if (!Directory.Exists(Path.Combine(workshopFolder, WorkshopId.ToString())))
                        {
                            return InstallStatus.Missing;
                        }
                    }
                }
                return InstallStatus.ForceEnabled;
            }
        } // forced
        public override InstallStatus InstalledGog
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string sourceFolder = Path.Combine(workshopFolder, WorkshopId.ToString());
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", WorkshopId.ToString());

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
                        string sourceFolder = Path.Combine(workshopFolder, WorkshopId.ToString());
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", WorkshopId.ToString());

                        if (!Directory.Exists(Path.Combine(workshopFolder, WorkshopId.ToString()))) return InstallStatus.Missing;
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
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        bool hadError;
                        string[] ModTypes = BZ98RTools.GetModTypes(Path.Combine(workshopFolder, WorkshopId.ToString()), out hadError);
                        if (ModTypes?.Length > 0)
                        {
                            return (hadError ? "!" : string.Empty) + string.Join(", ", ModTypes);
                        }
                        if (hadError)
                        {
                            return "PARSE ERROR";
                        }
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0)
                    {
                        try
                        {
                            string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                            string ModType = BZCCTools.GetModType(Path.Combine(workshopFolder, WorkshopId.ToString()));
                            if (ModType != null) return ModType;
                        }
                        catch
                        {
                            return "PARSE ERROR";
                        }
                    }
                    if (InstalledSteam == InstallStatus.Missing)
                        return "asset?";
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
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string[] ModTags = BZ98RTools.GetModTags(Path.Combine(workshopFolder, WorkshopId.ToString()));
                        return ModTags;
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string[] ModTags = BZCCTools.GetModTags(Path.Combine(workshopFolder, WorkshopId.ToString()));
                        return ModTags;
                    }
                }
                return new string[] { "UNKNON" };
            }
        }
        public override string WorkshopIdOutput { get { return WorkshopId.ToString(); } }
        public override string ModSource { get { return "Steam"; } }

        public override string FilePath
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                    return Path.Combine(workshopFolder, WorkshopId.ToString());
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                    return Path.Combine(workshopFolder, WorkshopId.ToString());
                }
                return null;
            }
        }

        public SteamMod(int AppId, UInt64 WorkshopId)
        {
            this.AppId = AppId;
            this.WorkshopId = WorkshopId;
        }

        public override string ToString()
        {
            if (AppId == MainForm.AppIdBZ98)
            {
                if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                {
                    string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                    bool hadError;
                    string[] ModNames = BZ98RTools.GetModNames(Path.Combine(workshopFolder, WorkshopId.ToString()), out hadError);
                    if (ModNames?.Length > 0)
                    {
                        return (hadError ? "!" : string.Empty) + string.Join(" / ", ModNames);
                    }
                    if (hadError)
                    {
                        return WorkshopId.ToString() + " (PARSE ERROR)";
                    }
                }

            }
            if (AppId == MainForm.AppIdBZCC)
            {
                if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0)
                {
                    try
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string ModName = BZCCTools.GetModName(Path.Combine(workshopFolder, WorkshopId.ToString()));
                        if (ModName != null) return ModName;
                    }
                    catch
                    {
                        return WorkshopId + " (PARSE ERROR)";
                    }
                }
            }
            return UniqueID;
        }

        public override void ToggleGog()
        {
            if (InstalledGog == InstallStatus.Missing)
            {

            }
            else if (InstalledGog == InstallStatus.Uninstalled)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string sourceFolder = Path.Combine(workshopFolder, WorkshopId.ToString());
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", WorkshopId.ToString());

                        if (Directory.Exists(destinationFolder)) return;
                        JunctionPoint.Create(destinationFolder, sourceFolder, false);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string sourceFolder = Path.Combine(workshopFolder, WorkshopId.ToString());
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", WorkshopId.ToString());

                        if (Directory.Exists(destinationFolder)) return;
                        JunctionPoint.Create(destinationFolder, sourceFolder, false);
                    }
                }
            }
            else if (InstalledGog == InstallStatus.Linked)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string sourceFolder = Path.Combine(workshopFolder, WorkshopId.ToString());
                        string destinationFolder = Path.Combine(MainForm.settings.BZ98RGogPath, "mods", WorkshopId.ToString());

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCSteamPath?.Length ?? 0) > 0 && (MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string sourceFolder = Path.Combine(workshopFolder, WorkshopId.ToString());
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", WorkshopId.ToString());

                        if (!Directory.Exists(destinationFolder)) return;
                        if (JunctionPoint.Exists(destinationFolder) && JunctionPoint.GetTarget(destinationFolder) != sourceFolder) return;
                        JunctionPoint.Delete(destinationFolder);
                    }
                }
            }
        }
        public override void ToggleSteam()
        {
            if (InstalledSteam == InstallStatus.Missing)
            {
                Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={WorkshopId}");
            }
        }

        public override bool Delete()
        {
            return false;
        }
    }
}
