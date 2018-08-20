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
                    e?.Arguments?.ToList()?.ForEach(dr => tsslSteamCmdCommand.Text += ((dr != null) ? " " +  dr : " \\0"));

                    Log($"SteamCmd Command:\t\"{tsslSteamCmdCommand.Text}\"");
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

        private void SetSteamCmdStatusText(string text)
        {
            tsslSteamCmd.Text = text;
            Log($"SteamCmd Status:\t\t\"{text}\"");
        }

        private void Log(string text)
        {
            lock(txtLog)
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
                    Color orig = txtLogSteamCmd.SelectionColor;
                    if (input) txtLogSteamCmd.SelectionColor = Color.DarkGray;
                    txtLogSteamCmd.AppendText(text);
                    txtLogSteamCmd.SelectionColor = orig;
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
                        Color orig = txtLogSteamCmdFull.SelectionColor;
                        txtLogSteamCmdFull.SelectionColor = Color.DarkGray;
                        txtLogSteamCmdFull.AppendText(text);
                        txtLogSteamCmdFull.SelectionColor = orig;
                    }
                    else
                    {
                        Color orig = txtLogSteamCmdFull.SelectionColor;

                        List<string> items = new List<string>() { text };
                        items = items.SelectMany(dr =>
                            dr.Split(new string[] { badstring }, StringSplitOptions.None)
                                .SelectMany(dx => new string[] { badstring, dx })).Skip(1).ToList();

                        items.ForEach(dr =>
                        {
                            txtLogSteamCmdFull.SelectionColor = orig;
                            if (dr == badstring) txtLogSteamCmdFull.SelectionColor = Color.Yellow;
                            txtLogSteamCmdFull.AppendText(dr);
                        });
                        txtLogSteamCmdFull.SelectionColor = orig;
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
