﻿using Microsoft.Win32;
using Newtonsoft.Json;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using BZRModManager.ModItem;
using IniParser;
using System.Net;

namespace BZRModManager
{
    public partial class MainForm : Form
    {
        SteamCmdContext SteamCmd = SteamCmdContext.GetInstance();

        public const int AppIdBZ98 = 301650;
        public const int AppIdBZCC = 624970;

        private const int MAX_OTHER_STEAMCMD_ERROR = 5;

        object ModStatus = new object();
        Dictionary<int, Dictionary<string, ModItemBase>> Mods = new Dictionary<int, Dictionary<string, ModItemBase>>();
        Dictionary<int, Dictionary<string, WorkshopMod>> FoundMods = new Dictionary<int, Dictionary<string, WorkshopMod>>();

        FileStream steamcmd_log = null;
        TextWriter steamcmd_log_writer = null;
        FileStream steamcmdfull_log = null;
        TextWriter steamcmdfull_log_writer = null;

        public static SettingsContainer settings;
        private bool cbFallbackSteamCmdWindowHandlingSet = false;

        private bool ForceUpdateMode = false;

        private Process RestoreWhenProcessCloses = null;

        public MainForm(bool ForceUpdateMode = false)
        {
            this.ForceUpdateMode = ForceUpdateMode;

            LoadSettings();
            //SteamCmd.ShowProcessWindow = settings.FallbackSteamCmdHandling;
            InitializeComponent();
            cbFallbackSteamCmdWindowHandling.Checked = settings.FallbackSteamCmdHandling;
            cbFallbackSteamCmdWindowHandlingSet = true;


            pnlTasks.HorizontalScroll.Maximum = 0;
            pnlTasks.AutoScroll = false;
            pnlTasks.VerticalScroll.Visible = false;
            pnlTasks.AutoScroll = true;

            this.Text += " - Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
#if DEBUG
            this.Text += " - DEV";
#endif

            Mods[AppIdBZ98] = new Dictionary<string, ModItemBase>();
            Mods[AppIdBZCC] = new Dictionary<string, ModItemBase>();
            FoundMods[AppIdBZ98] = new Dictionary<string, WorkshopMod>();
            FoundMods[AppIdBZCC] = new Dictionary<string, WorkshopMod>();

            List<string> Filters = new List<string>() { "new" };
            lvFindModsBZ98R.TypeFilter = Filters;
            lvFindModsBZCC.TypeFilter = Filters;

            this.FormClosing += MainForm_FormClosing;
            SteamCmd.SteamCmdStatusChange += Steam_SteamCmdStatusChange;
            //SteamCmd.SteamCmdCommandChange += Steam_SteamCmdCommandChange;
            SteamCmd.SteamCmdOutput += Steam_SteamCmdOutput;
            SteamCmd.SteamCmdOutputFull += Steam_SteamCmdOutputFull;
            SteamCmd.SteamCmdArgs += Steam_SteamCmdArgs;

            if (!Directory.Exists("log")) Directory.CreateDirectory("log");

            foreach (string oldLog in Directory.EnumerateFiles("log", "*.log", SearchOption.TopDirectoryOnly).ToList())
                File.Delete(oldLog);

            string logdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            Trace.Listeners.Add(new TextWriterTraceListener($"log\\{logdate}-bzrmodmanager.log"));
            Trace.AutoFlush = true;
            steamcmd_log = File.OpenWrite($"log\\{logdate}-steamcmd.log");
            steamcmd_log_writer = new StreamWriter(steamcmd_log);
            steamcmdfull_log = File.OpenWrite($"log\\{logdate}-steamcmd-full.log");
            steamcmdfull_log_writer = new StreamWriter(steamcmdfull_log);

            SteamCmd.SteamCmdOutput += SteamCmd_Log;
            SteamCmd.SteamCmdOutputFull += SteamCmdFull_Log;
            SteamCmd.SteamCmdArgs += SteamCmd_Log;
            SteamCmd.SteamCmdArgs += SteamCmdFull_Log;
        }

        ~MainForm()
        {
            steamcmd_log_writer.Close();
            steamcmdfull_log_writer.Close();
        }

        private void SteamCmdFull_Log(object sender, string msg)
        {
            lock (steamcmdfull_log_writer)
            {
                steamcmdfull_log_writer.Write(msg);
                steamcmdfull_log_writer.Flush();
            }
        }

        private void SteamCmd_Log(object sender, string msg)
        {
            lock (steamcmd_log_writer)
            {
                steamcmd_log_writer.Write(msg);
                steamcmd_log_writer.Flush();
            }
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

        private void Steam_SteamCmdArgs(object sender, string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                Log($"SteamCmd Started:\t\t{msg}");
                LogSteamCmd(msg + "\r\n", true);
                LogSteamCmdFull(msg + "\r\n", true);
            });
        }

        TaskControl RemovingSteamCmd = null;
        TaskControl ActivatingSteamCmd = null;
        private void Steam_SteamCmdStatusChange(object sender, SteamCmdStatusChangeEventArgs e)
        {
            switch (e.Status)
            {
                case ESteamCmdStatus.Closed:
                    this.Invoke((MethodInvoker)delegate
                    {
                        LogSteamCmd("\r\nEXIT\r\n\r\n", true);
                        LogSteamCmdFull("\r\nEXIT\r\n\r\n", true);
                        this.SetSteamCmdStatusText(e.Status.ToString());
                    });
                    break;
                default:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.SetSteamCmdStatusText(e.Status.ToString());
                    });
                    break;
            }
        }

        private void UpdateActiveTaskStatus()
        {
            lock (ActiveTasksLock)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    tsslActiveTasks.Text = ActiveTasks.ToString();
                });
            }
        }

        object ActiveTasksLock = new object();
        int ActiveTasks = 0;
        public TaskControl AddTask(string Name, int MaxValue)
        {
            TaskControl ctrl = new TaskControl(Name, MaxValue);
            this.Invoke((MethodInvoker)delegate
            {
                pnlTasks.Controls.Add(ctrl);
                pnlTasks.Refresh();
            });
            lock (ActiveTasksLock)
            {
                ActiveTasks++;
                UpdateActiveTaskStatus();
            }
            return ctrl;
        }
        public void EndTask(TaskControl ctrl)
        {
            if (ctrl != null)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    pnlTasks.Controls.Remove(ctrl);
                    pnlTasks.Refresh();
                });
                lock (ActiveTasksLock)
                {
                    ActiveTasks--;
                    UpdateActiveTaskStatus();
                }
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
                //txtLog.ScrollToCaret();
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
                    //txtLogSteamCmd.ScrollToCaret();
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
                    //txtLogSteamCmdFull.ScrollToCaret();
                }
            }
        }

        int exitingStage = 0;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (exitingStage == 0)
            {
                this.Text = this.Text + " (Shutting Down)";
                DisableEverything();
                //this.Enabled = false;
                exitingStage = 1;

                if (SteamCmd.Status == ESteamCmdStatus.Closed)
                {
                    return; // exit nicely
                }
            }

            if (exitingStage == 1)
            {
                if (SteamCmd.Status == ESteamCmdStatus.Closed)
                {
                    return; // exit nicely
                }
                //else if (SteamCmd.Status == ESteamCmdStatus.Exiting)
                //{
                //    e.Cancel = true;
                //}

                // normal wait till it closes loop
                new Thread(() =>
                {
                    while (SteamCmd.Status != ESteamCmdStatus.Closed)// && SteamCmd.Active)
                    {
                        Thread.Sleep(100);
                    }
                    try
                    {
                        this?.Invoke((MethodInvoker)delegate
                        {
                            exitingStage = 3;

                            SteamCmd.SteamCmdStatusChange -= Steam_SteamCmdStatusChange;
                            //SteamCmd.SteamCmdCommandChange -= Steam_SteamCmdCommandChange;
                            //SteamCmd.SteamCmdOutput -= Steam_SteamCmdOutput;
                            //SteamCmd.SteamCmdOutputFull -= Steam_SteamCmdOutputFull;
                            //SteamCmd.SteamCmdInput -= Steam_SteamCmdInput;
                            //SteamCmd.SteamCmdOutput -= SteamCmd_Log;
                            //SteamCmd.SteamCmdOutputFull -= SteamCmdFull_Log;
                            //SteamCmd.SteamCmdInput -= SteamCmd_Log;
                            //SteamCmd.SteamCmdInput -= SteamCmdFull_Log;

                            this?.Close();
                        });
                    }
                    catch
                    {
                        //SteamCmd.ForceKill();
                    }
                }).Start();

                //new Thread(() =>
                //{
                //    SteamCmd.Shutdown();
                //}).Start();

                e.Cancel = true;

                exitingStage = 2;
            }

            if (exitingStage == 3)
            {
                if (restart)
                {
                    string file = this.GetType().Assembly.Location;
                    string app = System.IO.Path.GetFileNameWithoutExtension(file);
                    Process.Start(app);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.modmanager;
            ActivatingSteamCmd = AddTask($"Activating SteamCMD", 0);
            new Thread(() =>
            {
                SteamCmd.Download();
                if (exitingStage > 1) return;
                //this.Invoke((MethodInvoker)delegate
                //{
                    this.UpdateBZ98RModLists();
                    this.UpdateBZCCModLists();
                //});
                EndTask(ActivatingSteamCmd);
                ActivatingSteamCmd = null;

                if (ForceUpdateMode)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        tabControl1.SelectedTab = tpTasks;

                        DisableEverything();
                    });

                    while (!(UpdateBZ98RModListsTask == null || UpdateBZ98RModListsTask.IsCanceled || UpdateBZ98RModListsTask.IsCompleted || UpdateBZ98RModListsTask.IsFaulted)
                        || !(UpdateBZCCModListsTask  == null || UpdateBZCCModListsTask.IsCanceled  || UpdateBZCCModListsTask.IsCompleted  || UpdateBZCCModListsTask.IsFaulted))
                    {
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(1000);

                    FindModsBZ98R(true);
                    FindModsBZCC(true);

                    while (!(FindModsBZ98RTask == null || FindModsBZ98RTask.IsCanceled || FindModsBZ98RTask.IsCompleted || FindModsBZ98RTask.IsFaulted)
                        || !(FindModsBZCCTask  == null || FindModsBZCCTask.IsCanceled  || FindModsBZCCTask.IsCompleted  || FindModsBZCCTask.IsFaulted))
                    {
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(1000);

                    this.UpdateBZ98RMods(true);
                    this.UpdateBZCCMods(true);
                    this.GetDependenciesBZCCMods();

                    while (!(UpdateBZ98RModsTask         == null || UpdateBZ98RModsTask.IsCanceled         || UpdateBZ98RModsTask.IsCompleted         || UpdateBZ98RModsTask.IsFaulted)
                        || !(UpdateBZCCModsTask          == null || UpdateBZCCModsTask.IsCanceled          || UpdateBZCCModsTask.IsCompleted          || UpdateBZCCModsTask.IsFaulted)
                        || !(GetDependenciesBZCCModsTask == null || GetDependenciesBZCCModsTask.IsCanceled || GetDependenciesBZCCModsTask.IsCompleted || GetDependenciesBZCCModsTask.IsFaulted))
                    {
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(1000);

                    Close();
                }
            }).Start();
        }

        private void btnDownloadBZ98R_Click(object sender, EventArgs e) { if (DownloadMod(txtDownloadBZ98R.Text, AppIdBZ98)) txtDownloadBZ98R.Clear(); }
        private void btnDownloadBZCC_Click(object sender, EventArgs e) { if (DownloadMod(txtDownloadBZCC.Text, AppIdBZCC)) txtDownloadBZCC.Clear(); }
        private bool DownloadMod(string text, UInt32 AppId)
        {
            bool success = false;

            try
            {
                UInt64 workshopID = 0;
                try
                {
                    workshopID = UInt64.Parse(HttpUtility.ParseQueryString(new Uri(text).Query)["id"]);
                }
                catch (UriFormatException)
                {
                    workshopID = UInt64.Parse(text);
                }
                if (workshopID > 0)
                {
                    success = true;
                    TaskControl DownloadModTaskControl = AddTask($"Download {(AppId == AppIdBZ98 ? "BZ98" : AppId == AppIdBZCC ? "BZCC" : AppId.ToString())} Mod - SteamCmd - {workshopID}", 0);
                    Task.Factory.StartNew(() =>
                    {
                        SteamCmdException ex_ = null;
                        int OtherErrorCounter = 0;
                        do
                        {
                            ex_ = null;
                            try
                            {
                                SteamCmd.WorkshopDownloadItem(AppId, workshopID);
                            }
                            catch (SteamCmdWorkshopDownloadException ex)
                            {
                                ex_ = ex;
                                if (!ex_.Message.StartsWith("ERROR! Timeout downloading item "))
                                    OtherErrorCounter++;
                            }
                            catch (SteamCmdException ex)
                            {
                                ex_ = ex;
                                OtherErrorCounter++;
                            }
                        } while (ex_ != null && OtherErrorCounter < MAX_OTHER_STEAMCMD_ERROR);


                        this.Invoke((MethodInvoker)delegate
                        {
                            switch (AppId)
                            {
                                case AppIdBZ98:
                                    UpdateBZ98RModLists();
                                    break;
                                case AppIdBZCC:
                                    UpdateBZCCModLists();
                                    break;
                            }
                        });
                        EndTask(DownloadModTaskControl);
                    });
                }
            }
            catch { }

            try
            {
                if (!success)
                {
                    try
                    {
                        string[] branches = GitContext.GetModBranches(settings.GitPath, text);
                        if (branches.Length > 0)
                        {
                            success = true;

                            using (MultiSelectDialog dlg = new MultiSelectDialog("Branch Select", branches.Select(dr => new Tuple<string, object, bool>(dr == "baked" || dr.StartsWith("baked-") ? $"{dr} (Direct)" : dr, dr, dr == "baked" || dr.StartsWith("baked-")))))
                            {
                                if (dlg.ShowDialog() == DialogResult.OK)
                                {
                                    TaskControl DownloadModTaskControl = AddTask($"Download {(AppId == AppIdBZ98 ? "BZ98" : AppId == AppIdBZCC ? "BZCC" : AppId.ToString())} Mod - Git - \"{text}\"", 0);
                                    Task.Factory.StartNew(() =>
                                    {
                                        GitContext.WorkshopDownloadItem(settings.GitPath, AppId, text, dlg.Selected);
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            switch (AppId)
                                            {
                                                case AppIdBZ98:
                                                    UpdateBZ98RModLists();
                                                    break;
                                                case AppIdBZCC:
                                                    UpdateBZCCModLists();
                                                    break;
                                            }
                                        });
                                        EndTask(DownloadModTaskControl);
                                    });
                                }
                            }
                        }
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        if (ex.Message == @"The system cannot find the file specified")
                        {
                            MessageBox.Show("Workshop ID was not detected, GIT download attempted.\r\ngit.exe not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch { }

            return success;
        }

        private void btnRefreshBZ98R_Click(object sender, EventArgs e) { this.UpdateBZ98RModLists(); }
        private void btnRefreshBZCC_Click(object sender, EventArgs e) { this.UpdateBZCCModLists(); }

        private void btnUpdateBZ98R_Click(object sender, EventArgs e) { this.UpdateBZ98RMods(false); }
        private void btnHardUpdateBZ98R_Click(object sender, EventArgs e) { this.UpdateBZ98RMods(true); }
        private void btnUpdateBZCC_Click(object sender, EventArgs e) { this.UpdateBZCCMods(false); }
        private void btnHardUpdateBZCC_Click(object sender, EventArgs e) { this.UpdateBZCCMods(true); }

        private void btnDependenciesBZ98R_Click(object sender, EventArgs e) { this.GetDependenciesBZCCMods(); }

        private void LoadSettings()
        {
            if (!File.Exists("settings.json"))
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(new SettingsContainer()));

            settings = JsonConvert.DeserializeObject<SettingsContainer>(File.ReadAllText("settings.json"));
        }
        private void SaveSettings()
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tabControl1.SelectedIndex == 4)
            if (tabControl1.SelectedTab == tpSettings)
            {
                txtBZ98RSteam.Text = settings.BZ98RSteamPath;
                txtBZCCSteam.Text = settings.BZCCSteamPath;
                txtBZ98RGog.Text = settings.BZ98RGogPath;
                txtBZCCMyDocs.Text = settings.BZCCMyDocsPath;
                txtBZCCGog.Text = settings.BZCCGogPath;
                txtGit.Text = settings.GitPath;
            }
        }

        private void btnBZ98RSteamApply_Click(object sender, EventArgs e)
        {
            settings.BZ98RSteamPath = txtBZ98RSteam.Text;
            SaveSettings();
        }

        private void btnBZCCSteamApply_Click(object sender, EventArgs e)
        {
            settings.BZCCSteamPath = txtBZCCSteam.Text;
            SaveSettings();
        }

        private void txtBZ98RGogApply_Click(object sender, EventArgs e)
        {
            settings.BZ98RGogPath = txtBZ98RGog.Text;
            SaveSettings();
        }

        private void btnBZCCRGogApply_Click(object sender, EventArgs e)
        {
            settings.BZCCGogPath = txtBZCCGog.Text;
            SaveSettings();
        }

        private void btnBZCCMyDocsApply_Click(object sender, EventArgs e)
        {
            settings.BZCCMyDocsPath = txtBZCCMyDocs.Text;
            SaveSettings();
        }

        private void btnGitApply_Click(object sender, EventArgs e)
        {
            settings.GitPath = txtGit.Text;
            SaveSettings();
        }

        bool restart = false;
        private void cbFallbackSteamCmdWindowHandling_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFallbackSteamCmdWindowHandlingSet)
            {
                settings.FallbackSteamCmdHandling = cbFallbackSteamCmdWindowHandling.Checked;
                SaveSettings();
                if (MessageBox.Show("The application must restart to apply this setting. Restart?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    restart = true;
                    this.Close();
                }
            }
        }

        private void pnlTasks_Resize(object sender, EventArgs e)
        {
            pnlTasks.Refresh();
        }

        /// <summary>
        /// Used during shutdown delay and during automatic mod to prevent interaction with interface.
        /// </summary>
        private void DisableEverything()
        {
            txtDownloadBZ98R.Enabled = false;
            lvModsBZ98R.Enabled = false;
            btnDependenciesBZ98R.Enabled = false;
            btnDownloadBZ98R.Enabled = false;
            btnRefreshBZ98R.Enabled = false;
            btnUpdateBZ98R.Enabled = false;
            btnHardUpdateBZ98R.Enabled = false;
            cbBZ98RTypeMod.Enabled = false;
            cbBZ98RTypeMultiplayer.Enabled = false;
            cbBZ98RTypeError.Enabled = false;
            cbBZ98RTypeCampaign.Enabled = false;
            cbBZ98RTypeInstantAction.Enabled = false;

            txtDownloadBZCC.Enabled = false;
            lvModsBZCC.Enabled = false;
            btnDownloadBZCC.Enabled = false;
            btnRefreshBZCC.Enabled = false;
            btnUpdateBZCC.Enabled = false;
            btnHardUpdateBZCC.Enabled = false;
            cbBZCCTypeError.Enabled = false;
            cbBZCCTypeConfig.Enabled = false;
            cbBZCCTypeAddon.Enabled = false;
            cbBZCCTypeAsset.Enabled = false;

            lvFindModsBZ98R.Enabled = false;
            lvFindModsBZCC.Enabled = false;
            btnFindMods.Enabled = false;
            btnDownloadSelectedFoundMods.Enabled = false;
            rbFindModsIcon.Enabled = false;
            rbFindModsTable.Enabled = false;
            cbFindModsNewOnly.Enabled = false;

            btnFixSteamCmd.Enabled = false;
            cbFallbackSteamCmdWindowHandling.Enabled = false;
            btnMultiRefresh.Enabled = false;
            btnMultiJoinSteam.Enabled = false;
            btnMultiJoinGOG.Enabled = false;
            btnGetModSteamCmd.Enabled = false;
            rbFindGamesMap.Enabled = false;
            rbFindGamesTable.Enabled = false;
            lvMultiplayerBZ98R.Enabled = false;
            lvMultiplayerBZCC.Enabled = false;
            lvPlayers.Enabled = false;

            btnBZ98RSteamFind.Enabled = false;
            txtBZ98RSteam.Enabled = false;
            btnBZ98RSteamApply.Enabled = false;
            btnBZCCSteamFind.Enabled = false;
            txtBZCCSteam.Enabled = false;
            btnBZCCSteamApply.Enabled = false;
            btnBZ98RGogFind.Enabled = false;
            txtBZ98RGog.Enabled = false;
            btnBZ98RGogApply.Enabled = false;
            btnBZCCMyDocsFind.Enabled = false;
            txtBZCCMyDocs.Enabled = false;
            btnBZCCMyDocsApply.Enabled = false;
            btnBZCCGogFind.Enabled = false;
            txtBZCCGog.Enabled = false;
            btnBZCCRGogApply.Enabled = false;
            btnGitFind.Enabled = false;
            txtGit.Enabled = false;
            btnGitApply.Enabled = false;

            btnRunAudit.Enabled = false;
        }

        private void cbBZ98RType_CheckedChanged(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (cbBZ98RTypeMod.Checked) Filters.Add("mod");
            if (cbBZ98RTypeMultiplayer.Checked) Filters.Add("multiplayer");
            if (cbBZ98RTypeInstantAction.Checked) Filters.Add("instant_action");
            if (cbBZ98RTypeCampaign.Checked) Filters.Add("campaign");
            if (cbBZ98RTypeError.Checked) Filters.Add("!");

            lvModsBZ98R.TypeFilter = Filters;
            lvModsBZ98R.Refresh();
        }

        private void cbBZCCType_CheckedChanged(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (cbBZCCTypeAddon.Checked) Filters.Add("addon");
            if (cbBZCCTypeConfig.Checked) Filters.Add("config");
            if (cbBZCCTypeAsset.Checked) Filters.Add("asset");
            if (cbBZCCTypeError.Checked) Filters.Add("UNKNOWN");

            lvModsBZCC.TypeFilter = Filters;
            lvModsBZCC.Refresh();
        }

        private void btnBZ98RGogFind_Click(object sender, EventArgs e)
        {
            string path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\GOG.com\Games\1454067812", "path", null) as string;
            txtBZ98RGog.Text = path;
        }
        private void btnBZCCGogFind_Click(object sender, EventArgs e)
        {
            string path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\GOG.com\Games\1120387413", "path", null) as string;
            txtBZCCGog.Text = path;
        }

        private void btnBZCCMyDocsFind_Click(object sender, EventArgs e)
        {
            string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            txtBZCCMyDocs.Text = Path.Combine(docsPath, "My Games", "Battlezone Combat Commander");
        }

        private void btnBZ98RSteamFind_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string basePath in SteamVent.FileSystem.SteamProcessInfo.GetSteamLibraryPaths())
                {
                    string gameFolder = Path.Combine(basePath, "steamapps", "common", "Battlezone 98 Redux");
                    if (Directory.Exists(gameFolder) && File.Exists(Path.Combine(gameFolder, "battlezone98redux.exe")))
                    {
                        txtBZ98RSteam.Text = Path.Combine(basePath, "steamapps");
                        return;
                    }
                }
            }
            catch { }
        }

        private void btnGitFind_Click(object sender, EventArgs e)
        {
            string FullPathToGit = Where("git.exe");
            if (!string.IsNullOrWhiteSpace(FullPathToGit) && File.Exists(FullPathToGit))
            {
                txtGit.Text = FullPathToGit;
                return;
            }
            FullPathToGit = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Atlassian", "SourceTree", "git_local", "bin", "git.exe");
            if (!string.IsNullOrWhiteSpace(FullPathToGit) && File.Exists(FullPathToGit))
            {
                txtGit.Text = FullPathToGit;
                return;
            }
        }

        private static string Where(string file)
        {
            var paths = Environment.GetEnvironmentVariable("PATH").Split(';');
            var extensions = Environment.GetEnvironmentVariable("PATHEXT").Split(';');
            return (from p in new[] { Environment.CurrentDirectory }.Concat(paths)
                    from e in new[] { string.Empty }.Concat(extensions)
                    let path = Path.Combine(p.Trim(), file + e.ToLower())
                    where File.Exists(path)
                    select path).FirstOrDefault();
        }

        private void btnBZCCSteamFind_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string basePath in SteamVent.FileSystem.SteamProcessInfo.GetSteamLibraryPaths())
                {
                    string gameFolder = Path.Combine(basePath, "steamapps", "common", "BZ2R");
                    if (Directory.Exists(gameFolder) && File.Exists(Path.Combine(gameFolder, "battlezone2.exe")))
                    {
                        txtBZCCSteam.Text = Path.Combine(basePath, "steamapps");
                        return;
                    }
                }
            }
            catch { }
        }

        private void btnFindMods_Click(object sender, EventArgs e)
        {
            if (tcFindMods.SelectedTab == tpFindModsBZ98R)
            {
                FindModsBZ98R();
            }

            if (tcFindMods.SelectedTab == tpFindModsBZCC)
            {
                FindModsBZCC();
            }
        }

        private void rbFindMods_CheckedChanged(object sender, EventArgs e)
        {
            lvFindModsBZ98R.View = rbFindModsIcon.Checked ? View.LargeIcon : View.Details;
            lvFindModsBZCC.View = rbFindModsIcon.Checked ? View.LargeIcon : View.Details;
        }

        private void cbFindModsNewOnly_CheckedChanged(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (cbFindModsNewOnly.Checked) Filters.Add("new");
            if (Filters.Count == 0)
                Filters = null;

            lvFindModsBZ98R.TypeFilter = Filters;
            lvFindModsBZCC.TypeFilter = Filters;
            lvFindModsBZ98R.Refresh();
            lvFindModsBZCC.Refresh();
        }

        private void btnDownloadSelectedFoundMods_Click(object sender, EventArgs e)
        {
            if (tcFindMods.SelectedTab == tpFindModsBZ98R)
            {
                if (FindModsBZ98RTask == null
                 || FindModsBZ98RTask.IsCanceled
                 || FindModsBZ98RTask.IsCompleted
                 || FindModsBZ98RTask.IsFaulted)
                {
                    List<ILinqListViewFindModsItem> Items = new List<ILinqListViewFindModsItem>();
                    foreach (int idx in lvFindModsBZ98R.SelectedIndices)
                        Items.Add(lvFindModsBZ98R.GetItemAtVirtualIndex(idx));

                    foreach (ILinqListViewFindModsItem item in Items)
                        DownloadMod(item.URL, AppIdBZ98);
                }
            }

            if (tcFindMods.SelectedTab == tpFindModsBZCC)
            {
                if (FindModsBZCCTask == null
                 || FindModsBZCCTask.IsCanceled
                 || FindModsBZCCTask.IsCompleted
                 || FindModsBZCCTask.IsFaulted)
                {
                    List<ILinqListViewFindModsItem> Items = new List<ILinqListViewFindModsItem>();
                    foreach (int idx in lvFindModsBZCC.SelectedIndices)
                        Items.Add(lvFindModsBZCC.GetItemAtVirtualIndex(idx));

                    foreach (ILinqListViewFindModsItem item in Items)
                        DownloadMod(item.URL, AppIdBZCC);
                }
            }
        }

        private void btnFixSteamCmd_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This operation is very slow as SteamCmd will be removed and reloaded and all mods will be updated!\r\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                RemovingSteamCmd = AddTask($"Removing SteamCMD", 0);
                new Thread(() =>
                {
                    SteamCmd.Purge();
                    EndTask(RemovingSteamCmd);
                    RemovingSteamCmd = null;

                    ActivatingSteamCmd = AddTask($"Activating SteamCMD", 0);
                    SteamCmd.Download();
                    if (exitingStage > 1) return;
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.UpdateBZ98RModLists();
                        this.UpdateBZCCModLists();
                    });
                    EndTask(ActivatingSteamCmd);
                    ActivatingSteamCmd = null;
                }).Start();
            }
        }

        private void btnMultiRefresh_Click(object sender, EventArgs e)
        {
            if (tcMultiplayer.SelectedTab == tpMultiplayerBZ98R)
            {
                GetMpGamesBZ98R();
            }

            if (tcMultiplayer.SelectedTab == tpMultiplayerBZCC)
            {
                GetMpGamesBZCC();
            }
        }

        private void rbFindGames_CheckedChanged(object sender, EventArgs e)
        {
            lvMultiplayerBZ98R.View = rbFindGamesMap.Checked ? View.LargeIcon : View.Details;
            lvMultiplayerBZCC.View = rbFindGamesMap.Checked ? View.LargeIcon : View.Details;
            //lvPlayers.View = rbFindGamesMap.Checked ? View.LargeIcon : View.Details;
        }

        private void lvMultiplayerBZ98R_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
            foreach (int idx in lvMultiplayerBZ98R.SelectedIndices)
                Items.Add(lvMultiplayerBZ98R.GetItemAtVirtualIndex(idx).SessionItem);
            lvPlayers.BeginUpdate();
            lvPlayers.DataSource = Items.FirstOrDefault()?.Players;
            lvPlayers.EndUpdate();

            if (Items.Count > 0)
            {
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();

                btnMultiJoinSteam.Enabled = !string.IsNullOrWhiteSpace(settings.BZ98RSteamPath);
                btnMultiJoinGOG.Enabled = !string.IsNullOrWhiteSpace(settings.BZ98RGogPath);

                if(!string.IsNullOrWhiteSpace(settings.BZ98RSteamPath))
                {
                    if (session?.Level?.Mod != null && UInt64.TryParse(session.Level.Mod, out _))
                    {
                        bool HaveMod = false;
                        try
                        {
                            //lock (ModStatus)
                            //lock (Mods[AppIdBZ98])
                            if (Mods[AppIdBZ98].ContainsKey(session.Level.Mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-Steam"))
                                HaveMod = true;
                        }
                        catch { }
                        btnMultiGetModSteam.Enabled = !HaveMod;
                    }
                    else
                    {
                        btnMultiGetModSteam.Enabled = false;
                    }
                }
                else
                {
                    btnMultiGetModSteam.Enabled = false;
                }
                if (!string.IsNullOrWhiteSpace(settings.BZ98RGogPath))
                {
                    if (session?.Level?.Mod != null && UInt64.TryParse(session.Level.Mod, out _))
                    {
                        bool HaveMod = false;
                        try
                        {
                            //lock (ModStatus)
                            //lock (Mods[AppIdBZ98])
                            if (Mods[AppIdBZ98].ContainsKey(session.Level.Mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-SteamCmd"))
                                HaveMod = true;
                        }
                        catch { }
                        btnGetModSteamCmd.Enabled = !HaveMod;
                    }
                    else
                    {
                        btnGetModSteamCmd.Enabled = false;
                    }
                }
                else
                {
                    btnGetModSteamCmd.Enabled = false;
                }
            }
            else
            {
                btnMultiJoinSteam.Enabled = false;
                btnMultiJoinGOG.Enabled = false;
                btnMultiGetModSteam.Enabled = false;
                btnGetModSteamCmd.Enabled = false;
            }
        }

        private void lvMultiplayerBZCC_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
            foreach (int idx in lvMultiplayerBZCC.SelectedIndices)
                Items.Add(lvMultiplayerBZCC.GetItemAtVirtualIndex(idx).SessionItem);
            lvPlayers.BeginUpdate();
            lvPlayers.DataSource = Items.FirstOrDefault()?.Players;
            lvPlayers.EndUpdate();

            if (Items.Count > 0)
            {
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();

                btnMultiJoinSteam.Enabled = !string.IsNullOrWhiteSpace(settings.BZCCSteamPath);
                btnMultiJoinGOG.Enabled = !string.IsNullOrWhiteSpace(settings.BZCCGogPath);

                List<string> ModsIDs = new List<string>();
                if (session?.Game?.Mod != null)
                    if (UInt64.TryParse(session.Game.Mod, out _))
                        ModsIDs.Add(session.Game.Mod);
                if (session?.Game?.Mods != null)
                    foreach (string mod in session.Game.Mods)
                        if (UInt64.TryParse(mod, out _))
                            ModsIDs.Add(mod);



                if (!string.IsNullOrWhiteSpace(settings.BZCCSteamPath) && ModsIDs.Count > 0)
                {
                    bool MissingAMod = false;
                    try
                    {
                        //lock (ModStatus)
                        //lock (Mods[AppIdBZCC])
                        foreach (string mod in ModsIDs)
                            if (!Mods[AppIdBZCC].ContainsKey(mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-Steam"))
                            {
                                MissingAMod = true;
                                break;
                            }
                    }
                    catch { }
                    btnMultiGetModSteam.Enabled = MissingAMod;
                }
                else
                {
                    btnMultiGetModSteam.Enabled = false;
                }

                if (!string.IsNullOrWhiteSpace(settings.BZCCMyDocsPath) && ModsIDs.Count > 0)
                {
                    bool MissingAMod = false;
                    try
                    {
                        //lock (ModStatus)
                        //lock (Mods[AppIdBZCC])
                        foreach (string mod in ModsIDs)
                            if (!Mods[AppIdBZCC].ContainsKey(mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-SteamCmd"))
                            {
                                MissingAMod = true;
                                break;
                            }
                    }
                    catch { }
                    btnGetModSteamCmd.Enabled = MissingAMod;
                }
                else
                {
                    btnGetModSteamCmd.Enabled = false;
                }
            }
            else
            {
                btnMultiJoinSteam.Enabled = false;
                btnMultiJoinGOG.Enabled = false;
                btnMultiGetModSteam.Enabled = false;
                btnGetModSteamCmd.Enabled = false;
            }
        }

        private void tcMultiplayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvPlayers.BeginUpdate();
            lvMultiplayerBZ98R.SelectedIndices.Clear();
            lvMultiplayerBZCC.SelectedIndices.Clear();
            lvPlayers.DataSource = null;
            lvPlayers.EndUpdate();
            btnMultiJoinSteam.Enabled = false;
            btnMultiJoinGOG.Enabled = false;
            btnMultiGetModSteam.Enabled = false;
            btnGetModSteamCmd.Enabled = false;
        }

        private void lvPlayers_DoubleClick(object sender, EventArgs e)
        {
            List<string> Items = new List<string>();
            foreach (int idx in lvPlayers.SelectedIndices)
                Items.Add(lvPlayers.GetItemAtVirtualIndex(idx).URL);
            string URL = Items.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(URL))
                Process.Start(URL);
        }

        private void btnGetModSteamCmd_Click(object sender, EventArgs e)
        {
            if (tcMultiplayer.SelectedTab == tpMultiplayerBZ98R)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZ98R.SelectedIndices)
                    Items.Add(lvMultiplayerBZ98R.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                if (session?.Level?.Mod != null)
                    if (UInt64.TryParse(session.Level.Mod, out _))
                        DownloadMod(session.Level.Mod, AppIdBZ98);
            }

            if (tcMultiplayer.SelectedTab == tpMultiplayerBZCC)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZ98R.SelectedIndices)
                    Items.Add(lvMultiplayerBZ98R.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                if (session?.Game?.Mod != null)
                    if (UInt64.TryParse(session.Game.Mod, out _))
                        DownloadMod(session.Game.Mod, AppIdBZCC);
                if (session?.Game?.Mods != null)
                    foreach (string mod in session.Game.Mods)
                        if (UInt64.TryParse(mod, out _))
                            DownloadMod(mod, AppIdBZCC);
            }
        }

        private void btnMultiGetModSteam_Click(object sender, EventArgs e)
        {
            if (tcMultiplayer.SelectedTab == tpMultiplayerBZ98R)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZ98R.SelectedIndices)
                    Items.Add(lvMultiplayerBZ98R.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                if (session?.Level?.Mod != null)
                    if (UInt64.TryParse(session.Level.Mod, out _))
                        lock (ModStatus)
                            lock (Mods[AppIdBZ98])
                                if (!Mods[AppIdBZ98].ContainsKey(session.Level.Mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-Steam"))
                                    Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={session.Level.Mod}");
            }

            if (tcMultiplayer.SelectedTab == tpMultiplayerBZCC)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZ98R.SelectedIndices)
                    Items.Add(lvMultiplayerBZ98R.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                List<string> ModsIDs = new List<string>();
                if (session?.Game?.Mod != null)
                    if (UInt64.TryParse(session.Game.Mod, out _))
                        ModsIDs.Add(session.Game.Mod);
                if (session?.Game?.Mods != null)
                    foreach (string mod in session.Game.Mods)
                        if (UInt64.TryParse(mod, out _))
                            ModsIDs.Add(mod);
                if(ModsIDs.Count > 0)
                    lock (ModStatus)
                        lock (Mods[AppIdBZCC])
                            foreach(string mod in ModsIDs)
                                if (!Mods[AppIdBZCC].ContainsKey(mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-Steam"))
                                {
                                    Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={session.Level.Mod}");
                                    break;
                                }
            }
        }

        private void btnMultiJoinSteam_Click(object sender, EventArgs e)
        {
            if (tcMultiplayer.SelectedTab == tpMultiplayerBZ98R)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZ98R.SelectedIndices)
                    Items.Add(lvMultiplayerBZ98R.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                if (session == null)
                    return;

                string EXE = Path.Combine(settings.BZ98RSteamPath, "common", "Battlezone 98 Redux", "battlezone98redux.exe");
                if(File.Exists(EXE))
                {
                    if (!string.IsNullOrWhiteSpace(session.Level.Mod) && session.Level.Mod != "0" && !Directory.Exists(Path.Combine(SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppIdBZ98), session.Level.Mod)))
                    {
                        bool failedToGetMod = false;
                        DialogResult result = MessageBox.Show("Mod is missing, attempt to install from known mod?", "Missing Mod", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                        switch (result)
                        {
                            case DialogResult.Yes:
                                {
                                    lock (ModStatus)
                                        lock (Mods[AppIdBZ98])
                                            if (Mods[AppIdBZ98].ContainsKey($"{session.Level.Mod}-Git")) // steam can only try to install from Git
                                            {
                                                if (Mods[AppIdBZ98][$"{session.Level.Mod}-Git"].InstalledSteam != InstallStatus.Linked)
                                                    Mods[AppIdBZ98][$"{session.Level.Mod}-Git"].ToggleSteam();
                                            }
                                            else
                                            {
                                                failedToGetMod = true;
                                            }
                                }
                                break;
                            case DialogResult.No:
                                break;
                            case DialogResult.Cancel:
                                return;
                        }

                        if (failedToGetMod)
                            if (MessageBox.Show("Failed to get mod. Continue?", "Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                return;
                    }

                    // passwords don't work on BZR due to the comma required being eaten
                    /*string Password = string.Empty;
                    if (session.Status.HasPassword ?? false)
                    {
                        PasswordDialog dlg = new PasswordDialog("Password");
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            Password = dlg.Password;
                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = EXE,
                                WorkingDirectory = Path.GetDirectoryName(EXE),
                                Arguments = $"-connect-galaxy-lobby={session.Address["LobbyID"]}\",\"{Password}"
                            });
                            this.WindowState = FormWindowState.Minimized;
                        }
                    }
                    else*/
                    {
                        RestoreWhenProcessCloses = Process.Start(new ProcessStartInfo()
                        {
                            FileName = EXE,
                            WorkingDirectory = Path.GetDirectoryName(EXE),
                            Arguments = $"-connect-galaxy-lobby={session.Address["LobbyID"]}"
                        });
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
            }

            if (tcMultiplayer.SelectedTab == tpMultiplayerBZCC)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZCC.SelectedIndices)
                    Items.Add(lvMultiplayerBZCC.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                if (session == null)
                    return;

                string EXE = Path.Combine(settings.BZCCSteamPath, "common", "BZ2R", "battlezone2.exe");
                if (File.Exists(EXE))
                {
                    List<string> ModsOfGame = new List<string>();
                    ModsOfGame.Add(session.Game.Mod ?? "0");
                    if (session.Game.Mods != null)
                        ModsOfGame.AddRange(session.Game.Mods);

                    HashSet<string> MissingMods = new HashSet<string>();
                    foreach(string mod in ModsOfGame)
                    {
                        if (mod == "0")
                            continue;
                        if(Directory.Exists(Path.Combine(SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppIdBZCC), mod)))
                            continue;
                        MissingMods.Add(mod);
                    }

                    if (MissingMods.Count > 0)
                    {
                        bool failedToGetMod = false;
                        DialogResult result = MessageBox.Show("Mods are missing, attempt to install from known mods?", "Missing Mods", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                        switch (result)
                        {
                            case DialogResult.Yes:
                                {
                                    foreach (string mod in MissingMods)
                                    {
                                        lock (ModStatus)
                                            lock (Mods[AppIdBZCC])
                                                if (Mods[AppIdBZCC].ContainsKey($"{mod}-Git")) // steam can only try to install from Git
                                                {
                                                    if (Mods[AppIdBZCC][$"{mod}-Git"].InstalledSteam != InstallStatus.Linked)
                                                        Mods[AppIdBZCC][$"{mod}-Git"].ToggleSteam();
                                                }
                                                else
                                                {
                                                    failedToGetMod = true;
                                                }
                                    }
                                }
                                break;
                            case DialogResult.No:
                                break;
                            case DialogResult.Cancel:
                                return;
                        }

                        if (failedToGetMod)
                            if (MessageBox.Show("Failed to get some mods. Continue?", "Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                return;
                    }

                    string ModsString = string.Join(";", ModsOfGame);
                    string Password = string.Empty;
                    if (session.Status.HasPassword ?? false)
                    {
                        PasswordDialog dlg = new PasswordDialog("Password");
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            TryBzccMpJoinFix(EXE, true);

                            Password = dlg.Password;
                            string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                            RestoreWhenProcessCloses = Process.Start(new ProcessStartInfo()
                            {
                                FileName = EXE,
                                WorkingDirectory = Path.GetDirectoryName(EXE),
                                Arguments = $"-connect-mp {RichString}"
                            });
                            this.WindowState = FormWindowState.Minimized;
                        }
                    }
                    else
                    {
                        TryBzccMpJoinFix(EXE, true);

                        string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                        RestoreWhenProcessCloses = Process.Start(new ProcessStartInfo()
                        {
                            FileName = EXE,
                            WorkingDirectory = Path.GetDirectoryName(EXE),
                            Arguments = $"-connect-mp {RichString}"
                        });
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
            }
        }

        private void btnMultiJoinGOG_Click(object sender, EventArgs e)
        {
            if (tcMultiplayer.SelectedTab == tpMultiplayerBZ98R)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZ98R.SelectedIndices)
                    Items.Add(lvMultiplayerBZ98R.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                if (session == null)
                    return;

                string EXE = Path.Combine(settings.BZ98RGogPath, "battlezone98redux.exe");
                if (File.Exists(EXE))
                {
                    if (!string.IsNullOrWhiteSpace(session.Level.Mod) && session.Level.Mod != "0" && !Directory.Exists(Path.Combine(MainForm.settings.BZ98RGogPath, "mods", session.Level.Mod)))
                    {
                        bool failedToGetMod = false;
                        DialogResult result = MessageBox.Show("Mod is missing, attempt to install from known mod?", "Missing Mod", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                        switch (result)
                        {
                            case DialogResult.Yes:
                                {
                                    string padModId = UInt64.TryParse(session.Level.Mod, out _) ? session.Level.Mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') : null;
                                    lock (ModStatus)
                                        lock (Mods[AppIdBZ98])
                                            if (padModId != null && Mods[AppIdBZ98].ContainsKey($"{padModId}-SteamCmd"))
                                            {
                                                if (Mods[AppIdBZ98][$"{padModId}-SteamCmd"].InstalledGog != InstallStatus.Linked)
                                                    Mods[AppIdBZ98][$"{padModId}-SteamCmd"].ToggleGog();
                                            }
                                            else if (padModId != null && Mods[AppIdBZ98].ContainsKey($"{padModId}-Steam"))
                                            {
                                                if (Mods[AppIdBZ98][$"{padModId}-Steam"].InstalledGog != InstallStatus.Linked)
                                                    Mods[AppIdBZ98][$"{padModId}-Steam"].ToggleGog();
                                            }
                                            else if (Mods[AppIdBZ98].ContainsKey($"{session.Level.Mod}-Git"))
                                            {
                                                if (Mods[AppIdBZ98][$"{session.Level.Mod}-Git"].InstalledGog != InstallStatus.Linked)
                                                    Mods[AppIdBZ98][$"{session.Level.Mod}-Git"].ToggleGog();
                                            }
                                            else
                                            {
                                                failedToGetMod = true;
                                            }
                                }
                                break;
                            case DialogResult.No:
                                break;
                            case DialogResult.Cancel:
                                return;
                        }

                        if (failedToGetMod)
                            if (MessageBox.Show("Failed to get mod. Continue?", "Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                return;
                    }

                    // passwords don't work on BZR due to the comma required being eaten
                    /*string Password = string.Empty;
                    if (session.Status.HasPassword ?? false)
                    {
                        PasswordDialog dlg = new PasswordDialog("Password");
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            Password = dlg.Password;
                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = EXE,
                                WorkingDirectory = Path.GetDirectoryName(EXE),
                                Arguments = $"-connect-galaxy-lobby={session.Address["LobbyID"]}\\,{Password}"
                            });
                            this.WindowState = FormWindowState.Minimized;
                        }
                    }
                    else*/
                    {
                        RestoreWhenProcessCloses = Process.Start(new ProcessStartInfo()
                        {
                            FileName = EXE,
                            WorkingDirectory = Path.GetDirectoryName(EXE),
                            Arguments = $"-connect-galaxy-lobby={session.Address["LobbyID"]}"
                        });
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
            }

            if (tcMultiplayer.SelectedTab == tpMultiplayerBZCC)
            {
                List<MultiplayerGamelistData_Session> Items = new List<MultiplayerGamelistData_Session>();
                foreach (int idx in lvMultiplayerBZCC.SelectedIndices)
                    Items.Add(lvMultiplayerBZCC.GetItemAtVirtualIndex(idx).SessionItem);
                MultiplayerGamelistData_Session session = Items.FirstOrDefault();
                if (session == null)
                    return;

                string EXE = Path.Combine(settings.BZCCGogPath, "battlezone2.exe");
                if (File.Exists(EXE))
                {
                    List<string> ModsOfGame = new List<string>();
                    ModsOfGame.Add(session.Game.Mod ?? "0");
                    if (session.Game.Mods != null)
                        ModsOfGame.AddRange(session.Game.Mods);

                    HashSet<string> MissingMods = new HashSet<string>();
                    foreach (string mod in ModsOfGame)
                    {
                        if (mod == "0")
                            continue;
                        if (Directory.Exists(Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", mod)))
                            continue;
                        MissingMods.Add(mod);
                    }

                    if (MissingMods.Count > 0)
                    {
                        bool failedToGetMod = false;
                        DialogResult result = MessageBox.Show("Mods are missing, attempt to install from known mods?", "Missing Mods", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                        switch(result)
                        {
                            case DialogResult.Yes:
                                {
                                    foreach (string mod in MissingMods)
                                    {
                                        string padModId = UInt64.TryParse(mod, out _) ? mod.PadLeft(UInt64.MaxValue.ToString().Length, '0') : null;
                                        lock (ModStatus)
                                            lock (Mods[AppIdBZCC])
                                                if (padModId != null && Mods[AppIdBZCC].ContainsKey($"{padModId}-SteamCmd"))
                                                {
                                                    if (Mods[AppIdBZCC][$"{padModId}-SteamCmd"].InstalledGog != InstallStatus.Linked)
                                                        Mods[AppIdBZCC][$"{padModId}-SteamCmd"].ToggleGog();
                                                }
                                                else if (padModId != null && Mods[AppIdBZCC].ContainsKey($"{padModId}-Steam"))
                                                {
                                                    if (Mods[AppIdBZCC][$"{padModId}-Steam"].InstalledGog != InstallStatus.Linked)
                                                        Mods[AppIdBZCC][$"{padModId}-Steam"].ToggleGog();
                                                }
                                                else if (Mods[AppIdBZCC].ContainsKey($"{mod}-Git"))
                                                {
                                                    if (Mods[AppIdBZCC][$"{mod}-Git"].InstalledGog != InstallStatus.Linked)
                                                        Mods[AppIdBZCC][$"{mod}-Git"].ToggleGog();
                                                }
                                                else
                                                {
                                                    failedToGetMod = true;
                                                }
                                    }
                                }
                                break;
                            case DialogResult.No:
                                break;
                            case DialogResult.Cancel:
                                return;
                        }

                        if (failedToGetMod)
                            if (MessageBox.Show("Failed to get some mods. Continue?", "Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                return;
                    }

                    string ModsString = string.Join(";", ModsOfGame);
                    string Password = string.Empty;
                    if (session.Status.HasPassword ?? false)
                    {
                        PasswordDialog dlg = new PasswordDialog("Password");
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            TryBzccMpJoinFix(EXE, false);

                            Password = dlg.Password;
                            string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                            RestoreWhenProcessCloses = Process.Start(new ProcessStartInfo()
                            {
                                FileName = EXE,
                                WorkingDirectory = Path.GetDirectoryName(EXE),
                                Arguments = $"-connect-mp {RichString}"
                            });
                            this.WindowState = FormWindowState.Minimized;
                        }
                    }
                    else
                    {
                        TryBzccMpJoinFix(EXE, false);

                        string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                        RestoreWhenProcessCloses = Process.Start(new ProcessStartInfo()
                        {
                            FileName = EXE,
                            WorkingDirectory = Path.GetDirectoryName(EXE),
                            Arguments = $"-connect-mp {RichString}"
                        });
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
            }
        }

        private void btnSteamAward_Click(object sender, EventArgs e)
        {
            Process.Start(@"https://steamcommunity.com/sharedfiles/filedetails/?id=2270251770");
        }

        private void btnDiscord_Click(object sender, EventArgs e)
        {
            Process.Start(@"http://discord.battlezone.report/");
        }

        private void btnGithub_Click(object sender, EventArgs e)
        {
            Process.Start(@"https://github.com/Nielk1/Battlezone-Redux-Mod-Manager");
        }

        private void btnRunAudit_Click(object sender, EventArgs e)
        {
            AuditMods();
        }


        FileStream audit_log = null;
        TextWriter audit_log_writer = null;
        private void LogAuditItem(string text, bool start = false)
        {
            if (start || audit_log_writer == null)
            {
                audit_log_writer?.Close();
                audit_log?.Close();

                string logdate = DateTime.Now.ToString("yyyyMMddHHmmss");
                audit_log = File.OpenWrite($"log\\{logdate}-audit.log");
                audit_log_writer = new StreamWriter(audit_log);
            }

            if (text != null)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (txtLogSteamCmd)
                    {
                        if (text != null)
                        {
                            if (start)
                            {
                                txtAuditLog.AppendText("==========================\r\n");
                                audit_log_writer.Write("==========================\r\n");
                            }
                            txtAuditLog.AppendText(text);
                            audit_log_writer.Write(text);
                            txtAuditLog.AppendText("\r\n");
                            audit_log_writer.Write("\r\n");
                            txtAuditLog.AppendText(start ? "==========================\r\n" : "--------------------------\r\n");
                            audit_log_writer.Write(start ? "==========================\r\n" : "--------------------------\r\n");

                            audit_log_writer.Flush();
                        }
                    }
                });
            }
        }


        Task ModAuditTask = null;
        private void AuditMods()
        {
            if (ModAuditTask == null
             || ModAuditTask.IsCanceled
             || ModAuditTask.IsCompleted
             || ModAuditTask.IsFaulted)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnRunAudit.Enabled = false;
                    txtAuditLog.Clear();
                });

                ModAuditTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Auditing", 0);

                    LogAuditItem("Mod Audit", true);

                    {
                        WebClient client = new WebClient();
                        TaskControl subtask = UpdateTaskControl.AddTask("Downloading Audit Data", 2);
                        try
                        {
                            client.DownloadFile(@"https://gamelistassets.iondriver.com/bz98r/audit.json", $"bz98r_audit.json");
                        }
                        catch { }
                        subtask.Value = 1;
                        try
                        {
                            client.DownloadFile(@"https://gamelistassets.iondriver.com/bzcc/audit.json", $"bzcc_audit.json");
                        }
                        catch { }
                        subtask.Value = 2;
                        UpdateTaskControl.EndTask(subtask);
                    }

                    Dictionary<string, AuditData> BZ98R_Audit = new Dictionary<string, AuditData>();
                    if (File.Exists($"bz98r_audit.json"))
                        try
                        {
                            BZ98R_Audit = JsonConvert.DeserializeObject<Dictionary<string, AuditData>>(File.ReadAllText($"bz98r_audit.json"));
                        }
                        catch { }
                    Dictionary<string, AuditData> BZCC_Audit = new Dictionary<string, AuditData>();
                    if(File.Exists($"bzcc_audit.json"))
                        try
                        {
                            BZCC_Audit = JsonConvert.DeserializeObject<Dictionary<string, AuditData>>(File.ReadAllText($"bzcc_audit.json"));
                        }
                        catch { }

                    lock (ModStatus)
                    {
                        {
                            var ModList = Mods[AppIdBZ98].ToList();
                            TaskControl subtask = UpdateTaskControl.AddTask("Checking BZ98 Mods", ModList.Count);
                            int progress = 0;
                            foreach (var mod in ModList)
                            {
                                if (mod.Value.ModType.Contains("!"))
                                    LogAuditItem($"BZ98 mod ini Error, notify mod author\r\n" +
                                                 $"ID: {mod.Value.UniqueID}\r\n" +
                                                (ulong.TryParse(mod.Value.WorkshopIdOutput, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={mod.Value.WorkshopIdOutput}\r\n" : string.Empty) +
                                                 $"Name: {mod.Value.Name}\r\n" +
                                                 $"Path: {mod.Value.FilePath}");
                                if (BZ98R_Audit.ContainsKey(mod.Value.WorkshopIdOutput))
                                {
                                    if (BZ98R_Audit[mod.Value.WorkshopIdOutput].Status == "Broken")
                                        LogAuditItem($"BZ98 mod known broken\r\n" +
                                                     $"ID: {mod.Value.UniqueID}\r\n" +
                                                    (ulong.TryParse(mod.Value.WorkshopIdOutput, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={mod.Value.WorkshopIdOutput}\r\n" : string.Empty) +
                                                     $"Name: {mod.Value.Name}\r\n" +
                                                     $"Path: {mod.Value.FilePath}");
                                    if (BZ98R_Audit[mod.Value.WorkshopIdOutput].Status == "Superseded")
                                        LogAuditItem($"BZ98 mod known superseded\r\n" +
                                                     $"ID: {mod.Value.UniqueID}\r\n" +
                                                    (ulong.TryParse(mod.Value.WorkshopIdOutput, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={mod.Value.WorkshopIdOutput}\r\n" : string.Empty) +
                                                     $"Name: {mod.Value.Name}\r\n" +
                                                     $"Path: {mod.Value.FilePath}\r\n" +
                                                     $"Replacement Mod: {(ulong.TryParse(BZ98R_Audit[mod.Value.WorkshopIdOutput].NewID, out _) ? $"https://steamcommunity.com/workshop/filedetails/?id={BZ98R_Audit[mod.Value.WorkshopIdOutput].NewID}" : BZ98R_Audit[mod.Value.WorkshopIdOutput].NewID)}");
                                }
                                progress++;
                                subtask.Value = progress;
                            }
                            UpdateTaskControl.EndTask(subtask);
                        }

                        {
                            var ModList = Mods[AppIdBZCC].ToList();
                            TaskControl subtask = UpdateTaskControl.AddTask("Checking BZCC Mods", ModList.Count);
                            int progress = 0;
                            foreach (var mod in ModList)
                            {
                                if (mod.Value.ModType.Contains("UNKNOWN"))
                                    LogAuditItem($"BZCC mod ini Error, notify mod author\r\n" +
                                                 $"ID: {mod.Value.UniqueID}\r\n" +
                                                (ulong.TryParse(mod.Value.WorkshopIdOutput, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={mod.Value.WorkshopIdOutput}\r\n" : string.Empty) +
                                                 $"Name: {mod.Value.Name}\r\n" +
                                                 $"Path: {mod.Value.FilePath}");
                                if (BZCC_Audit.ContainsKey(mod.Value.WorkshopIdOutput))
                                {
                                    if (BZCC_Audit[mod.Value.WorkshopIdOutput].Status == "Broken")
                                        LogAuditItem($"BZCC mod known broken\r\n" +
                                                     $"ID: {mod.Value.UniqueID}\r\n" +
                                                    (ulong.TryParse(mod.Value.WorkshopIdOutput, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={mod.Value.WorkshopIdOutput}\r\n" : string.Empty) +
                                                     $"Name: {mod.Value.Name}\r\n" +
                                                     $"Path: {mod.Value.FilePath}");
                                    if (BZCC_Audit[mod.Value.WorkshopIdOutput].Status == "Superseded")
                                        LogAuditItem($"BZCC mod known superseded\r\n" +
                                                     $"ID: {mod.Value.UniqueID}\r\n" +
                                                    (ulong.TryParse(mod.Value.WorkshopIdOutput, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={mod.Value.WorkshopIdOutput}\r\n" : string.Empty) +
                                                     $"Name: {mod.Value.Name}\r\n" +
                                                     $"Path: {mod.Value.FilePath}\r\n" +
                                                     $"Replacement Mod: {(ulong.TryParse(BZCC_Audit[mod.Value.WorkshopIdOutput].NewID, out _) ? $"https://steamcommunity.com/workshop/filedetails/?id={BZCC_Audit[mod.Value.WorkshopIdOutput].NewID}" : BZCC_Audit[mod.Value.WorkshopIdOutput].NewID)}");
                                }
                                progress++;
                                subtask.Value = progress;
                            }
                            UpdateTaskControl.EndTask(subtask);
                        }
                    }

                    List<(string Platform, string Game, int AppID, string Path)> WorkshopDestinations = new List<(string Platform, string Game, int AppID, string Path)>();

                    if ((settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                        WorkshopDestinations.Add(("Steam", "BZ98R", AppIdBZ98, SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppIdBZ98)));

                    if ((settings?.BZCCSteamPath?.Length ?? 0) > 0)
                        WorkshopDestinations.Add(("Steam", "BZCC", AppIdBZCC, SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppIdBZCC)));

                    if ((settings?.BZ98RGogPath?.Length ?? 0) > 0)
                        WorkshopDestinations.Add(("Gog", "BZ98R", AppIdBZ98, Path.Combine(MainForm.settings.BZ98RGogPath, "mods")));

                    if ((settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                        WorkshopDestinations.Add(("Gog", "BZCC", AppIdBZCC, Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop")));

                    FileIniDataParser parser = new FileIniDataParser();
                    foreach (var workshopDestination in WorkshopDestinations)
                    {
                        if (Directory.Exists(workshopDestination.Path))
                        {
                            TaskControl subtask = UpdateTaskControl.AddTask($"Checking Installed {workshopDestination.Platform} {workshopDestination.Game} Mods", 0);

                            if (workshopDestination.AppID == AppIdBZCC)
                            {
                                HashSet<string> ModDirs = new HashSet<string>();
                                foreach (string dir in Directory.GetDirectories(workshopDestination.Path))
                                {
                                    string folderName = Path.GetFileName(dir);
                                    string iniFile = Path.Combine(dir, folderName + ".ini");
                                    if (File.Exists(iniFile))
                                    {
                                        try
                                        {
                                            parser.ReadFile(iniFile);
                                            ModDirs.Add(folderName);
                                        }
                                        catch
                                        {
                                            LogAuditItem($"Installed BZCC mod ini parse error, notify mod author\r\n" +
                                                         $"ID: {folderName}\r\n" +
                                                        (ulong.TryParse(folderName, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={folderName}\r\n" : string.Empty) +
                                                         $"Path: {dir}");
                                        }
                                    }
                                    else
                                    {
                                        if (File.Exists(Path.Combine(dir, "Mod.ini")))
                                        {
                                            ModDirs.Add(folderName);
                                        }

                                        LogAuditItem($"Installed BZCC mod ini not found, notify mod author\r\n" +
                                                     $"ID: {folderName}\r\n" +
                                                     (ulong.TryParse(folderName, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={folderName}\r\n" : string.Empty) +
                                                     $"Path: {dir}");
                                    }
                                }

                                foreach (string dir in Directory.GetDirectories(workshopDestination.Path))
                                {
                                    string folderName = Path.GetFileName(dir);
                                    string[] dependencies = BZCCTools.GetAssetDependencies(dir);
                                    List<string> MissingDependencies = (dependencies?.Where(dr => !ModDirs.Contains(dr)) ?? new string[] { }).ToList();

                                    if (MissingDependencies.Count > 0)
                                    {
                                        int ctr = 0;
                                        LogAuditItem($"Installed BZCC mod missing installed dependencies\r\n" +
                                                     $"ID: {folderName}\r\n" +
                                                     (ulong.TryParse(folderName, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={folderName}\r\n" : string.Empty) +
                                                     $"Path: {dir}\r\n" +
                                                     $"Missing: {string.Join(",", MissingDependencies)}\r\n" +  
                                                     $"Links: {string.Join("\r\n", MissingDependencies.Where(dr => ulong.TryParse(dr, out _)).Select(dr => (ctr++ > 0 ? "       " : string.Empty) + $"https://steamcommunity.com/workshop/filedetails/?id={dr}"))}");
                                    }
                                }
                            }
                            else if (workshopDestination.AppID == AppIdBZ98)
                            {
                                foreach (string dir in Directory.GetDirectories(workshopDestination.Path))
                                {
                                    string folderName = Path.GetFileName(dir);
                                    List<string> IniErrors = new List<string>();
                                    var FoundSubInis = Directory.EnumerateFiles(dir, "*.ini", SearchOption.TopDirectoryOnly);
                                    foreach (string iniFile in FoundSubInis)
                                    {
                                        try
                                        {
                                            parser.ReadFile(iniFile);
                                        }
                                        catch
                                        {
                                            string fileName = Path.GetFileName(iniFile);
                                            IniErrors.Add(fileName);
                                        }
                                    }
                                    if (IniErrors.Count > 0)
                                    {
                                        LogAuditItem($"Installed BZ98R mod ini parse error, notify mod author\r\n" +
                                                     $"ID: {folderName}\r\n" +
                                                     (ulong.TryParse(folderName, out _) ? $"Link: https://steamcommunity.com/workshop/filedetails/?id={folderName}\r\n" : string.Empty) +
                                                     $"Path: {dir}\r\n" +
                                                     $"Inis: {string.Join(",", IniErrors)}");
                                    }
                                }
                            }

                            UpdateTaskControl.EndTask(subtask);
                        }
                    }

                    EndTask(UpdateTaskControl);

                    this.Invoke((MethodInvoker)delegate
                    {
                        btnRunAudit.Enabled = true;
                    });
                });
            }
        }

        private void txtAuditLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.LinkText))
                Process.Start(e.LinkText);
        }


        /*private void tmrRestore_Tick(object sender, EventArgs e)
        {
            if(RestoreWhenProcessCloses != null && RestoreWhenProcessCloses.HasExited)
            {
                RestoreWhenProcessCloses = null;
                this.WindowState = FormWindowState.Normal;
            }
        }*/
    }

    public class AuditData
    {
        public string Status { get; set; }
        public string NewID { get; set; }
    }
}
