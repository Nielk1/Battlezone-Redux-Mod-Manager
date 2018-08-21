﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace BZRModManager
{
    public partial class MainForm : Form
    {
        SteamCmdContext SteamCmd = SteamCmdContext.GetInstance();

        public const int AppIdBZ98 = 301650;
        public const int AppIdBZCC = 624970;

        object ModStatus = new object();
        Dictionary<int, Dictionary<string, ModItem>> Mods = new Dictionary<int, Dictionary<string, ModItem>>();

        public MainForm()
        {
            InitializeComponent();

            Mods[AppIdBZ98] = new Dictionary<string, ModItem>();
            Mods[AppIdBZCC] = new Dictionary<string, ModItem>();

            this.FormClosing += Form1_FormClosing;
            SteamCmd.SteamCmdStatusChange += Steam_SteamCmdStatusChange;
            SteamCmd.SteamCmdCommandChange += Steam_SteamCmdCommandChange;
            SteamCmd.SteamCmdOutput += Steam_SteamCmdOutput;
            SteamCmd.SteamCmdOutputFull += Steam_SteamCmdOutputFull;
            SteamCmd.SteamCmdInput += Steam_SteamCmdInput;
        }

        private void Steam_SteamCmdOutput(object sender, string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                LogSteamCmd(msg, false);
            });
        }

        private void Steam_SteamCmdOutputFull(object sender, string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                LogSteamCmdFull(msg, false);
            });
        }

        private void Steam_SteamCmdInput(object sender, string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                LogSteamCmd(msg, true);
                LogSteamCmdFull(msg, true);
            });
        }

        private void Steam_SteamCmdCommandChange(object sender, SteamCmdCommandChangeEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (string.IsNullOrWhiteSpace(e.Command))
                {
                    tsslSteamCmdCommand.Enabled = false;
                    tsslSteamCmdCommand.Text = "none";
                }
                else
                {
                    tsslSteamCmdCommand.Enabled = true;
                    tsslSteamCmdCommand.Text = e.Command;
                    e?.Arguments?.ToList()?.ForEach(dr => tsslSteamCmdCommand.Text += ((dr != null) ? " " + dr : " \\0"));

                    Log($"SteamCmd Command:\t\"{tsslSteamCmdCommand.Text}\"");
                }
            });
        }

        private void Steam_SteamCmdStatusChange(object sender, SteamCmdStatusChangeEventArgs e)
        {
            switch (e.Status)
            {
                case SteamCmdStatus.Active:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.SetSteamCmdStatusText(e.Status.ToString());
                    });
                    new Thread(() => SteamCmd.LoginAnonymous()).Start();
                    break;
                case SteamCmdStatus.LoggedIn:
                case SteamCmdStatus.LoggedInAnon:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.SetSteamCmdStatusText(e.Status.ToString());
                    });
                    new Thread(() => {
                        try { SteamCmd.WorkshopDownloadItem(AppIdBZ98, 1); } catch (SteamCmdWorkshopDownloadException) { }
                        try { SteamCmd.WorkshopDownloadItem(AppIdBZCC, 1); } catch (SteamCmdWorkshopDownloadException) { }
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.UpdateBZ98RModLists();
                            this.UpdateBZCCModLists();
                        });
                    }).Start();
                    break;
                default:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.SetSteamCmdStatusText(e.Status.ToString());
                    });
                    break;
            }
        }

        private void UpdateBZ98RModLists()
        {
            lock (ModStatus)
            {
                SteamCmd.WorkshopStatus(AppIdBZ98).ForEach(dr =>
                {
                    string ModId = SteamCmdMod.GetUniqueId(dr.WorkshopId);
                    if (!Mods[AppIdBZ98].ContainsKey(ModId))
                    {
                        Mods[AppIdBZ98][ModId] = new SteamCmdMod(AppIdBZ98, dr);
                    }
                    else
                    {
                        ((SteamCmdMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
                    }
                });

                lvModsBZ98R.BeginUpdate();
                lvModsBZ98R.DataSource = Mods[AppIdBZ98].Values.ToList<ILinqListViesItem>();
                lvModsBZ98R.EndUpdate();
            }
        }
        private void UpdateBZCCModLists()
        {
            lock (ModStatus)
            {
                SteamCmd.WorkshopStatus(AppIdBZCC).ForEach(dr =>
                {
                    string ModId = SteamCmdMod.GetUniqueId(dr.WorkshopId);
                    if (!Mods[AppIdBZCC].ContainsKey(ModId))
                    {
                        Mods[AppIdBZCC][ModId] = new SteamCmdMod(AppIdBZCC, dr);
                    }
                    else
                    {
                        ((SteamCmdMod)Mods[AppIdBZCC][ModId]).Workshop = dr;
                    }
                });

                lvModsBZCC.BeginUpdate();
                lvModsBZCC.DataSource = Mods[AppIdBZCC].Values.ToList<ILinqListViesItem>();
                lvModsBZCC.EndUpdate();
            }
        }

        private void SetSteamCmdStatusText(string text)
        {
            tsslSteamCmd.Text = text;
            Log($"SteamCmd Status:\t\t\"{text}\"");
        }

        private void Log(string text)
        {
            lock (txtLog)
            {
                txtLog.AppendText(text + "\r\n");
                txtLog.ScrollToCaret();
            }
        }

        private void LogSteamCmd(string text, bool input)
        {
            lock (txtLogSteamCmd)
            {
                if (text != null)
                {
                    //Color orig = txtLogSteamCmd.SelectionColor;
                    if (input) txtLogSteamCmd.SelectionColor = Color.DarkGray;
                    txtLogSteamCmd.AppendText(text);
                    //txtLogSteamCmdFull.SelectionColor = orig;
                    txtLogSteamCmd.SelectionColor = Color.White;
                    txtLogSteamCmd.ScrollToCaret();
                }
            }
        }

        private void LogSteamCmdFull(string text, bool input)
        {
            string badstring = "\\src\\common\\contentmanifest.cpp (650) : Assertion Failed: !m_bIsFinalized\r\n";

            lock (txtLogSteamCmdFull)
            {
                if (text != null)
                {
                    if (input)
                    {
                        //Color orig = txtLogSteamCmdFull.SelectionColor;
                        txtLogSteamCmdFull.SelectionColor = Color.DarkGray;
                        txtLogSteamCmdFull.AppendText(text);
                        //txtLogSteamCmdFull.SelectionColor = orig;
                        txtLogSteamCmdFull.SelectionColor = Color.White;
                    }
                    else
                    {
                        //Color orig = txtLogSteamCmdFull.SelectionColor;

                        List<string> items = new List<string>() { text };
                        items = items.SelectMany(dr =>
                            dr.Split(new string[] { badstring }, StringSplitOptions.None)
                                .SelectMany(dx => new string[] { badstring, dx })).Skip(1).ToList();

                        items.ForEach(dr =>
                        {
                            //txtLogSteamCmdFull.SelectionColor = orig;
                            txtLogSteamCmdFull.SelectionColor = Color.White;
                            if (dr == badstring) txtLogSteamCmdFull.SelectionColor = Color.Yellow;
                            txtLogSteamCmdFull.AppendText(dr);
                        });
                        //txtLogSteamCmdFull.SelectionColor = orig;
                        txtLogSteamCmdFull.SelectionColor = Color.White;
                    }
                    txtLogSteamCmdFull.ScrollToCaret();
                }
            }
        }

        bool exiting = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (exiting)
            {

            }
            else if (SteamCmd.Status == SteamCmdStatus.Closed)
            {
            }
            else if (SteamCmd.Status == SteamCmdStatus.Exiting)
            {
                e.Cancel = true;
            }
            else
            {
                exiting = true;
                new Thread(() =>
                {
                    while (SteamCmd.Status != SteamCmdStatus.Closed)
                    {
                        Thread.Sleep(100);
                    }
                    try
                    {
                        this?.Invoke((MethodInvoker)delegate
                        {
                            this?.Close();
                        });
                    }
                    catch
                    {
                        SteamCmd.ForceKill();
                    }
                }).Start();
                SteamCmd.Shutdown();
                e.Cancel = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    SteamCmd.Init();
                }
                catch (SteamCmdMissingException)
                {
                    SteamCmd.Download();
                    SteamCmd.Init();
                }
            }).Start();
        }

        private void btnDownloadBZ98R_Click(object sender, EventArgs e) { DownloadMod(txtDownloadBZ98R.Text, AppIdBZ98); }
        private void btnDownloadBZCC_Click(object sender, EventArgs e) { DownloadMod(txtDownloadBZCC.Text, AppIdBZCC); }
        private void DownloadMod(string text, int AppId)
        {
            try
            {
                int workshopID = -1;
                try
                {
                    workshopID = int.Parse(HttpUtility.ParseQueryString(new Uri(text).Query)["id"]);
                }
                catch (UriFormatException)
                {
                    workshopID = int.Parse(text);
                }
                if (workshopID > -1)
                {
                    SteamCmd.WorkshopDownloadItem(AppId, workshopID);
                    switch(AppId)
                    {
                        case AppIdBZ98:
                            UpdateBZ98RModLists();
                            break;
                        case AppIdBZCC:
                            UpdateBZCCModLists();
                            break;
                    }
                }
            }
            catch { }
        }

        private void btnRefreshBZ98R_Click(object sender, EventArgs e) { this.UpdateBZ98RModLists(); }
        private void btnRefreshBZCC_Click(object sender, EventArgs e) { this.UpdateBZCCModLists(); }

        private void btnUpdateBZ98R_Click(object sender, EventArgs e) { this.UpdateBZ98RMods(); }
        private void btnUpdateBZCC_Click(object sender, EventArgs e) { this.UpdateBZCCMods(); }

        private void btnDependenciesBZ98R_Click(object sender, EventArgs e) { this.GetDependenciesBZCCMods(); }
        private void UpdateBZ98RMods()
        {
            new Thread(() =>
            {
                lock (Mods[AppIdBZ98])
                {
                    Mods[AppIdBZ98].ToList().ForEach(dr =>
                    {
                        SteamCmdMod mod = dr.Value as SteamCmdMod;
                        if (mod != null)
                        {
                            try { SteamCmd.WorkshopDownloadItem(AppIdBZ98, mod.Workshop.WorkshopId); } catch (SteamCmdWorkshopDownloadException) { }
                        }
                    });
                }
            }).Start();
        }

        private void UpdateBZCCMods()
        {
            new Thread(() =>
            {
                lock (Mods[AppIdBZCC])
                {
                    Mods[AppIdBZCC].ToList().ForEach(dr =>
                    {
                        SteamCmdMod mod = dr.Value as SteamCmdMod;
                        if (mod != null)
                        {
                            try { SteamCmd.WorkshopDownloadItem(AppIdBZCC, mod.Workshop.WorkshopId); } catch (SteamCmdWorkshopDownloadException) { }
                        }
                    });
                }
            }).Start();
        }

        private void GetDependenciesBZCCMods()
        {
            new Thread(() =>
            {
                lock (Mods[AppIdBZCC])
                {
                    List<string> Dependencies = new List<string>();
                    HashSet<long> DependenciesGotten = new HashSet<long>();
                    Mods[AppIdBZCC].ToList().ForEach(dr =>
                    {
                        SteamCmdMod mod = dr.Value as SteamCmdMod;
                        if (mod != null)
                        {
                            Dependencies.AddRange(BZCCTools.GetAssetDependencies($"steamcmd\\steamapps\\workshop\\content\\{mod.AppId}\\{mod.Workshop.WorkshopId}"));
                            DependenciesGotten.Add(mod.Workshop.WorkshopId);
                        }
                    });
                    Dependencies.Distinct().ToList().ForEach(dr =>
                    {
                        long tmpLong = 0;
                        if (long.TryParse(dr, out tmpLong) && !DependenciesGotten.Contains(tmpLong))
                        {
                            try { SteamCmd.WorkshopDownloadItem(AppIdBZCC, tmpLong); } catch (SteamCmdWorkshopDownloadException) { }
                        }
                    });
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateBZCCModLists();
                    });
                }
            }).Start();
        }
    }

    public enum InstallStatus
    {
        Uninstalled,
        ForceDisabled,
        ForceEnabled,
        Linked,
    }

    public abstract class ModItem : ILinqListViesItem
    {
        public abstract string UniqueID { get; }
        public abstract InstallStatus InstalledSteam { get; }
        public abstract InstallStatus InstalledGog { get; }
        public int AppId { get; protected set; }
        public abstract string ModType { get; }
        public abstract string[] ModTags { get; }

        public string IconKey { get { return UniqueID; } }
        public string Name { get { return ToString(); } }
        public Image LargeIcon { get; set; }
        public Image SmallIcon { get; set; }
        public ListViewItem ListViewItemCache { get; set; }

        public override string ToString()
        {
            //if (Workshop != null) return Workshop.WorkshopId.ToString();
            return "UNKNOWN MOD";
        }
    }

    public class SteamMod : ModItem
    {
        public override string UniqueID { get { return "UNKNOWN" + "-Steam"; } }
        public override InstallStatus InstalledSteam { get { return InstallStatus.ForceEnabled; } } // forced
        public override InstallStatus InstalledGog { get { return InstallStatus.Uninstalled; } } // TODO: Dynamic
        public override string ModType { get { return "UNKNON"; } }
        public override string[] ModTags { get { return new string[] { "UNKNON" }; } }
    }

    public class SteamCmdMod : ModItem
    {
        public WorkshopItemStatus Workshop { get; set; }
        public override string UniqueID { get { return GetUniqueId(Workshop.WorkshopId); } }
        public static string GetUniqueId(long workshopId) { return workshopId.ToString().PadLeft(long.MaxValue.ToString().Length, '0') + "-SteamCmd"; }

        public override InstallStatus InstalledSteam { get { return InstallStatus.ForceDisabled; } } // forced
        public override InstallStatus InstalledGog { get { return InstallStatus.Uninstalled; } } // TODO: Dynamic

        public override string ModType
        {
            get
            {
                if (AppId == MainForm.AppIdBZ98)
                {
                    string[] ModTypes = BZ98RTools.GetModTypes($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                    if (ModTypes?.Length > 0)
                    {
                        return string.Join(", ", ModTypes);
                    }

                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    string ModType = BZCCTools.GetModType($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                    if(ModType != null) return ModType;
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

        public SteamCmdMod(int AppId, WorkshopItemStatus Workshop)
        {
            this.AppId = AppId;
            this.Workshop = Workshop;
        }

        public override string ToString()
        {
            if (AppId == MainForm.AppIdBZ98)
            {
                string[] ModNames = BZ98RTools.GetModNames($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                if (ModNames?.Length > 0)
                {
                    if (ModNames.Length == 1) return ModNames[0];
                    return string.Join(" / ", ModNames);
                }

            }
            if (AppId == MainForm.AppIdBZCC)
            {
                string ModName = BZCCTools.GetModName($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                if (ModName != null) return ModName;
            }
            return UniqueID;
        }
    }

    public class GitMod : ModItem
    {
        public override string UniqueID { get { return "UNKNOWN" + "-Git"; } }

        public override InstallStatus InstalledSteam { get { return InstallStatus.Uninstalled; } } // TODO: Dynamic
        public override InstallStatus InstalledGog { get { return InstallStatus.Uninstalled; } } // TODO: Dynamic

        public override string ModType { get { return "UNKNON"; } }
        public override string[] ModTags { get { return new string[] { "UNKNON" }; } }
    }
}
