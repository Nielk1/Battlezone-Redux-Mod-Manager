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
    public partial class Form1 : Form
    {
        SteamCmdContext SteamCmd = SteamCmdContext.GetInstance();

        object ModStatus = new object();
        Dictionary<long, BZ98RModItem> BZ98R_Mods = new Dictionary<long, BZ98RModItem>();
        Dictionary<long, BZCCModItem> BZCC_Mods = new Dictionary<long, BZCCModItem>();

        public Form1()
        {
            InitializeComponent();

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
                LogSteamCMD(msg, false);
            });
        }

        private void Steam_SteamCmdOutputFull(object sender, string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                LogSteamCMDFull(msg, false);
            });
        }

        private void Steam_SteamCmdInput(object sender, string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                LogSteamCMD(msg, true);
                LogSteamCMDFull(msg, true);
            });
        }

        private void Steam_SteamCmdCommandChange(object sender, SteamCmdCommandChangeEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (string.IsNullOrWhiteSpace(e.Command))
                {
                    tsslSteamCMDCommand.Enabled = false;
                    tsslSteamCMDCommand.Text = "none";
                }
                else
                {
                    tsslSteamCMDCommand.Enabled = true;
                    tsslSteamCMDCommand.Text = e.Command;
                    e?.Arguments?.ToList()?.ForEach(dr => tsslSteamCMDCommand.Text += ((dr != null) ? " " +  dr : " \\0"));

                    Log($"SteamCMD Command:\t\"{tsslSteamCMDCommand.Text}\"");
                }
            });
        }

        private void Steam_SteamCmdStatusChange(object sender, SteamCmdStatusChangeEventArgs e)
        {
            switch(e.Status)
            {
                case SteamCmdStatus.Active:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.SetSteamCMDStatusText(e.Status.ToString());
                    });
                    new Thread(() => SteamCmd.LoginAnonymous()).Start();
                    break;
                case SteamCmdStatus.LoggedIn:
                case SteamCmdStatus.LoggedInAnon:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.SetSteamCMDStatusText(e.Status.ToString());
                    });
                    new Thread(() => {
                        try { SteamCmd.WorkshopDownloadItem(301650, 1); } catch (SteamCmdWorkshopDownloadException) { }
                        try { SteamCmd.WorkshopDownloadItem(624970, 1); } catch (SteamCmdWorkshopDownloadException) { }
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.UpdateModLists();
                        });
                    }).Start();
                    break;
                default:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.SetSteamCMDStatusText(e.Status.ToString());
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
                SteamCmd.WorkshopStatus(301650).ForEach(dr =>
                {
                    if (!BZ98R_Mods.ContainsKey(dr.WorkshopId))
                        BZ98R_Mods[dr.WorkshopId] = new BZ98RModItem();
                    BZ98R_Mods[dr.WorkshopId].Workshop = dr;
                });

                lbModsBZ98R.BeginUpdate();
                lbModsBZ98R.Items.Clear();
                BZ98R_Mods.Select(dr => dr.Value).ToList().ForEach(dr => lbModsBZ98R.Items.Add(dr));
                lbModsBZ98R.EndUpdate();
            }
        }
        private void UpdateBZCCModLists()
        {
            lock (ModStatus)
            {
                SteamCmd.WorkshopStatus(624970).ForEach(dr =>
                {
                    if (!BZCC_Mods.ContainsKey(dr.WorkshopId))
                        BZCC_Mods[dr.WorkshopId] = new BZCCModItem();
                    BZCC_Mods[dr.WorkshopId].Workshop = dr;
                });

                lbModsBZCC.BeginUpdate();
                lbModsBZCC.Items.Clear();
                BZCC_Mods.Select(dr => dr.Value).ToList().ForEach(dr => lbModsBZCC.Items.Add(dr));
                lbModsBZCC.EndUpdate();
            }
        }

        private void SetSteamCMDStatusText(string text)
        {
            tsslSteamCMD.Text = text;
            Log($"SteamCMD Status:\t\t\"{text}\"");
        }

        private void Log(string text)
        {
            lock(txtLog)
            {
                txtLog.AppendText(text + "\r\n");
            }
        }

        private void LogSteamCMD(string text, bool input)
        {
            lock (txtLogSteamCMD)
            {
                if (text != null)
                {
                    Color orig = txtLogSteamCMD.SelectionColor;
                    if (input) txtLogSteamCMD.SelectionColor = Color.DarkGray;
                    txtLogSteamCMD.AppendText(text);
                    txtLogSteamCMD.SelectionColor = orig;
                }
            }
        }

        private void LogSteamCMDFull(string text, bool input)
        {
            lock (txtLogSteamCMDFull)
            {
                if (text != null)
                {
                    Color orig = txtLogSteamCMDFull.SelectionColor;
                    if (input) txtLogSteamCMDFull.SelectionColor = Color.DarkGray;
                    txtLogSteamCMDFull.AppendText(text);
                    txtLogSteamCMDFull.SelectionColor = orig;
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

        private void btnDownloadBZ98R_Click(object sender, EventArgs e)
        {
            DownloadBZ98RMod();
        }

        private void btnDownloadBZCC_Click(object sender, EventArgs e)
        {
            DownloadBZCCMod();
        }

        private void DownloadBZ98RMod()
        {
            new Thread(() =>
            {
                try
                {
                    int workshopID = -1;
                    try
                    {
                        workshopID = int.Parse(HttpUtility.ParseQueryString(new Uri(txtDownloadBZ98R.Text).Query)["id"]);
                    }
                    catch (UriFormatException)
                    {
                        workshopID = int.Parse(txtDownloadBZ98R.Text);
                    }
                    if (workshopID > -1)
                    {
                        SteamCmd.WorkshopDownloadItem(301650, workshopID);
                        UpdateBZ98RModLists();
                    }
                }
                catch { }
            }).Start();
        }

        private void DownloadBZCCMod()
        {
            new Thread(() =>
            {
                try
                {

                    int workshopID = -1;
                    try
                    {
                        workshopID = int.Parse(HttpUtility.ParseQueryString(new Uri(txtDownloadBZCC.Text).Query)["id"]);
                    }
                    catch (UriFormatException)
                    {
                        workshopID = int.Parse(txtDownloadBZCC.Text);
                    }
                    if (workshopID > -1)
                    {
                        SteamCmd.WorkshopDownloadItem(624970, workshopID);
                        UpdateBZCCModLists();
                    }
                }
                catch { }
            }).Start();
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

    public class BZ98RModItem
    {
        public WorkshopItemStatus Workshop { get; set; }

        public override string ToString()
        {
            if(Workshop != null) return Workshop.WorkshopId.ToString();
            return "UNKNOWN MOD";
        }
    }
    public class BZCCModItem
    {
        public WorkshopItemStatus Workshop { get; set; }

        public override string ToString()
        {
            if (Workshop != null) return Workshop.WorkshopId.ToString();
            return "UNKNOWN MOD";
        }
    }
}
