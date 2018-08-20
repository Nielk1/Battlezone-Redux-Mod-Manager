using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        const int AppIdBZ98 = 301650;
        const int AppIdBZCC = 624970;

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
                            this.UpdateModLists();
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

        private void UpdateModLists()
        {
            lock (ModStatus)
            {
                UpdateBZ98RModLists();
                UpdateBZCCModLists();
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
                        Mods[AppIdBZ98][ModId] = new SteamCmdMod();
                    ((SteamCmdMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
                });

                lbModsBZ98R.BeginUpdate();
                lbModsBZ98R.Items.Clear();
                Mods[AppIdBZ98].Select(dr => dr.Value).ToList().ForEach(dr => lbModsBZ98R.Items.Add(dr));
                lbModsBZ98R.EndUpdate();
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
                        Mods[AppIdBZCC][ModId] = new SteamCmdMod();
                    ((SteamCmdMod)Mods[AppIdBZCC][ModId]).Workshop = dr;
                });

                lbModsBZCC.BeginUpdate();
                lbModsBZCC.Items.Clear();
                Mods[AppIdBZCC].Select(dr => dr.Value).ToList().ForEach(dr => lbModsBZCC.Items.Add(dr));
                lbModsBZCC.EndUpdate();
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
                        txtLogSteamCmd.SelectionColor = Color.White;
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
                            txtLogSteamCmd.SelectionColor = Color.White;
                            if (dr == badstring) txtLogSteamCmdFull.SelectionColor = Color.Yellow;
                            txtLogSteamCmdFull.AppendText(dr);
                        });
                        //txtLogSteamCmdFull.SelectionColor = orig;
                        txtLogSteamCmd.SelectionColor = Color.White;
                    }
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

        private void tmrModUpdate_Tick(object sender, EventArgs e)
        {
            if (SteamCmd.Status == SteamCmdStatus.LoggedIn
             || SteamCmd.Status == SteamCmdStatus.LoggedInAnon)
            {
                UpdateModLists();
            }
        }
    }

    public enum InstallStatus
    {
        Uninstalled,
        ForceDisabled,
        ForceEnabled,
        Linked,
    }

    public abstract class ModItem
    {
        public abstract string UniqueID { get; }
        public abstract InstallStatus InstalledSteam { get; }
        public abstract InstallStatus InstalledGog { get; }

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
    }

    public class SteamCmdMod : ModItem
    {
        public WorkshopItemStatus Workshop { get; set; }
        public override string UniqueID { get { return GetUniqueId(Workshop.WorkshopId); } }
        public static string GetUniqueId(long workshopId) { return workshopId.ToString().PadLeft(long.MaxValue.ToString().Length, '0') + "-SteamCmd"; }

        public override InstallStatus InstalledSteam { get { return InstallStatus.ForceDisabled; } } // forced
        public override InstallStatus InstalledGog { get { return InstallStatus.Uninstalled; } } // TODO: Dynamic

        public override string ToString()
        {
            return UniqueID;
        }
    }

    public class SteamGit : ModItem
    {
        public override string UniqueID { get { return "UNKNOWN" + "-Git"; } }

        public override InstallStatus InstalledSteam { get { return InstallStatus.Uninstalled; } } // TODO: Dynamic
        public override InstallStatus InstalledGog { get { return InstallStatus.Uninstalled; } } // TODO: Dynamic
    }
}
