using Monitor.Core.Utilities;
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
                        string sourceFolder = Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
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
                        string sourceFolder = Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
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
                    string[] ModTypes = BZ98RTools.GetModTypes($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}", out hadError);
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
                        string ModType = BZCCTools.GetModType($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
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
                    string[] ModTags = BZ98RTools.GetModTags($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                    return ModTags;
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    string[] ModTags = BZCCTools.GetModTags($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                    return ModTags;
                }
                return new string[] { "UNKNON" };
            }
        }

        public override string WorkshopIdOutput { get { return Workshop.WorkshopId.ToString(); } }
        public override string ModSource { get { return "SteamCmd"; } }

        public SteamCmdMod(int AppId, WorkshopItemStatus Workshop)
        {
            this.AppId = AppId;
            this.Workshop = Workshop;
        }

        public override string ToString()
        {
            if (AppId == MainForm.AppIdBZ98)
            {
                bool hadError;
                string[] ModNames = BZ98RTools.GetModNames($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}", out hadError);
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
                    string ModName = BZCCTools.GetModName($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                    if (ModName != null) return ModName;
                }
                catch
                {
                    return Workshop.WorkshopId + " (PARSE ERROR)";
                }
            }
            return UniqueID;
        }

        public override void ToggleGog()
        {
            if (InstalledGog == InstallStatus.Uninstalled)
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    if ((MainForm.settings?.BZ98RGogPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
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
                        string sourceFolder = Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
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
                        string sourceFolder = Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
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
                        string sourceFolder = Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
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
    }
}
