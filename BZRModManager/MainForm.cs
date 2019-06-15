using Monitor.Core.Utilities;
using Newtonsoft.Json;
using SteamVent.SteamCmd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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

        FileStream steamcmd_log = null;
        TextWriter steamcmd_log_writer = null;
        FileStream steamcmdfull_log = null;
        TextWriter steamcmdfull_log_writer = null;

        public static SettingsContainer settings;
        private bool cbFallbackSteamCmdWindowHandlingSet = false;

        public MainForm()
        {
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

            Mods[AppIdBZ98] = new Dictionary<string, ModItem>();
            Mods[AppIdBZCC] = new Dictionary<string, ModItem>();

            this.FormClosing += Form1_FormClosing;
            SteamCmd.SteamCmdStatusChange += Steam_SteamCmdStatusChange;
            //SteamCmd.SteamCmdCommandChange += Steam_SteamCmdCommandChange;
            SteamCmd.SteamCmdOutput += Steam_SteamCmdOutput;
            SteamCmd.SteamCmdOutputFull += Steam_SteamCmdOutputFull;
            SteamCmd.SteamCmdArgs += Steam_SteamCmdArgs;

            if (!Directory.Exists("log")) Directory.CreateDirectory("log");
            string logdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            Trace.Listeners.Add(new TextWriterTraceListener($"log\\{logdate}-bzrmodmanager.log"));
            Trace.AutoFlush = true;
            steamcmd_log = File.OpenWrite($"log\\{logdate}-steamcmd.log");
            steamcmd_log_writer = new StreamWriter(steamcmd_log);
            steamcmdfull_log = File.OpenWrite($"log\\{logdate}-steamcmd-full.log");
            steamcmdfull_log_writer = new StreamWriter(steamcmdfull_log);

            //SteamCmd.SteamCmdOutput += SteamCmd_Log;
            //SteamCmd.SteamCmdOutputFull += SteamCmdFull_Log;
            //SteamCmd.SteamCmdInput += SteamCmd_Log;
            //SteamCmd.SteamCmdInput += SteamCmdFull_Log;
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

        /*private void Steam_SteamCmdCommandChange(object sender, SteamCmdCommandChangeEventArgs e)
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
        }*/

        TaskControl ActivatingSteamCmd = null;
        private void Steam_SteamCmdStatusChange(object sender, SteamCmdStatusChangeEventArgs e)
        {
            switch (e.Status)
            {
                case ESteamCmdStatus.Closed:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            LogSteamCmd("\r\nEXIT\r\n\r\n", true);
                            LogSteamCmdFull("\r\nEXIT\r\n\r\n", true);
                            this.SetSteamCmdStatusText(e.Status.ToString());
                        });
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

        public TaskControl AddTask(string Name, int MaxValue)
        {
            TaskControl ctrl = new TaskControl(Name, MaxValue);
            this.Invoke((MethodInvoker)delegate
            {
                pnlTasks.Controls.Add(ctrl);
                pnlTasks.Refresh();
            });
            return ctrl;
        }
        public void EndTask(TaskControl ctrl)
        {
            if (ctrl != null)
                this.Invoke((MethodInvoker)delegate
                {
                    pnlTasks.Controls.Remove(ctrl);
                    pnlTasks.Refresh();
                });
        }

        Task UpdateBZ98RModListsTask = null;
        //TaskControl UpdateBZ98RModListsTaskControl = null;
        private void UpdateBZ98RModLists()
        {
            if ( UpdateBZ98RModListsTask == null
              || UpdateBZ98RModListsTask.IsCanceled
              || UpdateBZ98RModListsTask.IsCompleted
              || UpdateBZ98RModListsTask.IsFaulted)
            {
                //EndTask(UpdateBZ98RModListsTaskControl);
                TaskControl UpdateBZ98RModListsTaskControl = AddTask("Update BZ98 Mod List", 0);
                UpdateBZ98RModListsTask = Task.Factory.StartNew(() =>
                {
                    lock (ModStatus)
                    {
                        Semaphore loadSemaphore = new Semaphore(0, 2);
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                TaskControl UpdateTask = UpdateBZ98RModListsTaskControl.AddTask("Update BZ98 Mod List (SteamCmd)", 0);
                                List<WorkshopItemStatus> stats = SteamCmd.WorkshopStatus(AppIdBZ98);
                                stats?.ForEach(dr =>
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
                                    Mods[AppIdBZ98][ModId].HasUpdate = dr.HasUpdate;
                                });
                                UpdateBZ98RModListsTaskControl.EndTask(UpdateTask);
                            }
                            finally
                            {
                                loadSemaphore.Release();
                            }
                        });

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                TaskControl UpdateTask = UpdateBZ98RModListsTaskControl.AddTask("Update BZ98 Mod List (Git)", 0);
                                List<GitModStatus> stats = GitContext.WorkshopItemsOnDrive(AppIdBZ98);
                                stats.ForEach(dr =>
                                {
                                    string ModId = GitMod.GetUniqueId(dr.ModWorkshopId);
                                    if (!Mods[AppIdBZ98].ContainsKey(ModId))
                                    {
                                        Mods[AppIdBZ98][ModId] = new GitMod(AppIdBZ98, dr);
                                    }
                                    else
                                    {
                                        ((GitMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
                                    }
                                });
                                UpdateBZ98RModListsTaskControl.EndTask(UpdateTask);
                            }
                            finally
                            {
                                loadSemaphore.Release();
                            }
                        });

                        if (settings.BZ98RSteamPath != null)
                        {
                            TaskControl UpdateTask = UpdateBZ98RModListsTaskControl.AddTask("Update BZ98 Mod List (Steam)", 0);
                            SteamContext.WorkshopItemsOnDrive(settings.BZ98RSteamPath, AppIdBZ98)?.ForEach(dr =>
                            {
                                string ModId = SteamMod.GetUniqueId(dr);
                                if (!Mods[AppIdBZ98].ContainsKey(ModId))
                                {
                                    Mods[AppIdBZ98][ModId] = new SteamMod(AppIdBZ98, dr);
                                }
                            });
                            UpdateBZ98RModListsTaskControl.EndTask(UpdateTask);
                        }

                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();

                        this.Invoke((MethodInvoker)delegate
                        {
                            lvModsBZ98R.BeginUpdate();
                            Mods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                            lvModsBZ98R.DataSource = Mods[AppIdBZ98].Values.ToList<ILinqListViesItem>();
                            lvModsBZ98R.EndUpdate();

                            EndTask(UpdateBZ98RModListsTaskControl);
                        });
                    }
                });
            }
        }

        Task UpdateBZCCModListsTask = null;
        //TaskControl UpdateBZCCModListsTaskControl = null;
        private void UpdateBZCCModLists()
        {
            if (UpdateBZCCModListsTask == null
              || UpdateBZCCModListsTask.IsCanceled
              || UpdateBZCCModListsTask.IsCompleted
              || UpdateBZCCModListsTask.IsFaulted)
            {
                //EndTask(UpdateBZCCModListsTaskControl);
                TaskControl UpdateBZCCModListsTaskControl = AddTask("Update BZCC Mod List", 0);
                UpdateBZCCModListsTask = Task.Factory.StartNew(() =>
                {
                    lock (ModStatus)
                    {
                        Semaphore loadSemaphore = new Semaphore(0, 2);
                        Task.Factory.StartNew(() =>
                        {
                            TaskControl UpdateTask = UpdateBZCCModListsTaskControl.AddTask("Update BZCC Mod List (SteamCmd)", 0);
                            List<WorkshopItemStatus> stats = SteamCmd.WorkshopStatus(AppIdBZCC);
                            stats?.ForEach(dr =>
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
                                Mods[AppIdBZCC][ModId].HasUpdate = dr.HasUpdate;
                            });
                            UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
                            loadSemaphore.Release();
                        });
                        Task.Factory.StartNew(() =>
                        {
                            TaskControl UpdateTask = UpdateBZCCModListsTaskControl.AddTask("Update BZCC Mod List (Git)", 0);
                            List<GitModStatus> stats = GitContext.WorkshopItemsOnDrive(AppIdBZCC);
                            stats?.ForEach(dr =>
                            {
                                string ModId = GitMod.GetUniqueId(dr.ModWorkshopId);
                                if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                {
                                    Mods[AppIdBZCC][ModId] = new GitMod(AppIdBZCC, dr);
                                }
                                else
                                {
                                    ((GitMod)Mods[AppIdBZCC][ModId]).Workshop = dr;
                                }
                            });
                            UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
                            loadSemaphore.Release();
                        });
                        if (settings.BZCCSteamPath != null)
                        {
                            TaskControl UpdateTask = UpdateBZCCModListsTaskControl.AddTask("Update BZCC Mod List (Steam)", 0);
                            SteamContext.WorkshopItemsOnDrive(settings.BZCCSteamPath, AppIdBZCC)?.ForEach(dr =>
                            {
                                string ModId = SteamMod.GetUniqueId(dr);
                                if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                {
                                    Mods[AppIdBZCC][ModId] = new SteamMod(AppIdBZCC, dr);
                                }
                            });
                            UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
                        }
                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();

                        this.Invoke((MethodInvoker)delegate
                        {
                            lvModsBZCC.BeginUpdate();
                            Mods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                            lvModsBZCC.DataSource = Mods[AppIdBZCC].Values.ToList<ILinqListViesItem>();
                            lvModsBZCC.EndUpdate();

                            EndTask(UpdateBZCCModListsTaskControl);
                        });
                    }
                });
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

        int exitingStage = 0;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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
                if(restart)
                {
                    string file = this.GetType().Assembly.Location;
                    string app = System.IO.Path.GetFileNameWithoutExtension(file);
                    Process.Start(app);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ActivatingSteamCmd = AddTask($"Activating SteamCMD", 0);
            /*new Thread(() =>
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
            }).Start();*/

            new Thread(() =>
            {
                SteamCmd.Download();
                try { SteamCmd.WorkshopDownloadItem(AppIdBZ98, 1); } catch (SteamCmdWorkshopDownloadException) { }// catch (SteamCmdInactiveException) { }
                try { SteamCmd.WorkshopDownloadItem(AppIdBZCC, 1); } catch (SteamCmdWorkshopDownloadException) { }// catch (SteamCmdInactiveException) { }
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
                        SteamCmd.WorkshopDownloadItem(AppId, workshopID);
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
                    string[] branches = GitContext.GetModBranches(text);
                    if (branches.Length > 0)
                    {
                        success = true;
                        TaskControl DownloadModTaskControl = AddTask($"Download {(AppId == AppIdBZ98 ? "BZ98" : AppId == AppIdBZCC ? "BZCC" : AppId.ToString())} Mod - Git - \"{text}\"", 0);
                        Task.Factory.StartNew(() =>
                        {
                            GitContext.WorkshopDownloadItem(AppId, text, branches);
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
            catch { }

            return success;
        }

        private void btnRefreshBZ98R_Click(object sender, EventArgs e) { this.UpdateBZ98RModLists(); }
        private void btnRefreshBZCC_Click(object sender, EventArgs e) { this.UpdateBZCCModLists(); }

        private void btnUpdateBZ98R_Click(object sender, EventArgs e) { this.UpdateBZ98RMods(Control.ModifierKeys == Keys.Shift); }
        private void btnUpdateBZCC_Click(object sender, EventArgs e) { this.UpdateBZCCMods(Control.ModifierKeys == Keys.Shift); }

        private void btnDependenciesBZ98R_Click(object sender, EventArgs e) { this.GetDependenciesBZCCMods(); }

        Task UpdateBZ98RModsTask = null;
        private void UpdateBZ98RMods(bool agressive)
        {
            if (UpdateBZ98RModsTask == null
              || UpdateBZ98RModsTask.IsCanceled
              || UpdateBZ98RModsTask.IsCompleted
              || UpdateBZ98RModsTask.IsFaulted)
            {
                UpdateBZ98RModsTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Update BZ98 Mods", 0);
                    lock (Mods[AppIdBZ98])
                    {
                        List<KeyValuePair<string, ModItem>> ModList = Mods[AppIdBZ98].ToList();
                        UpdateTaskControl.Maximum = ModList.Count;
                        object CounterClock = new object();
                        int Counter = 0;
                        List<KeyValuePair<string, ModItem>> NoUpdateMods = ModList.Where(dr => !(dr.Value is SteamCmdMod) && !(dr.Value is GitMod)).ToList();
                        List<KeyValuePair<string, ModItem>> SteamCmdMods = ModList.Where(dr => (dr.Value is SteamCmdMod)).ToList();
                        List<KeyValuePair<string, ModItem>> GitMods = ModList.Where(dr => (dr.Value is GitMod)).ToList();
                        NoUpdateMods.ForEach(dr =>
                        {
                            UpdateTaskControl.Value = ++Counter;
                        });
                        Semaphore MergeTasks = new Semaphore(0,1);
                        new Thread(() =>
                        {
                            SteamCmdMods.ForEach(dr =>
                            {
                                SteamCmdMod modSteam = dr.Value as SteamCmdMod;
                                if (agressive || (modSteam?.HasUpdate ?? false))
                                {
                                    if (modSteam != null)
                                    {
                                        TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZ98 Mod - SteamCmd - {modSteam.Workshop.WorkshopId} - {modSteam.Name}", 0);
                                        SteamCmdWorkshopDownloadException ex_ = null;
                                        do
                                        {
                                            ex_ = null;
                                            try { SteamCmd.WorkshopDownloadItem(AppIdBZ98, modSteam.Workshop.WorkshopId); } catch (SteamCmdWorkshopDownloadException ex) { ex_ = ex; }
                                        } while (ex_ != null && ex_.Message.StartsWith("ERROR! Timeout downloading item "));
                                        UpdateTaskControl.EndTask(DownloadModTaskControl);
                                    }
                                }
                                lock (CounterClock)
                                {
                                    UpdateTaskControl.Value = ++Counter;
                                }
                            });
                            MergeTasks.Release();
                        }).Start();
                        GitMods.ForEach(dr =>
                        {
                            GitMod mod = dr.Value as GitMod;
                            if (mod != null)
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZ98 Mod - Git - {mod.Workshop.ModWorkshopId} - {mod.Name}", 0);
                                GitContext.Pull(mod.Workshop.GitPath);
                                UpdateTaskControl.EndTask(DownloadModTaskControl);
                            }
                            lock (CounterClock)
                            {
                                UpdateTaskControl.Value = ++Counter;
                            }
                        });
                        MergeTasks.WaitOne();
                        EndTask(UpdateTaskControl);

                        UpdateBZ98RModLists();
                    }
                });
            }
        }

        Task UpdateBZCCModsTask = null;
        private void UpdateBZCCMods(bool agressive)
        {
            if (UpdateBZCCModsTask == null
              || UpdateBZCCModsTask.IsCanceled
              || UpdateBZCCModsTask.IsCompleted
              || UpdateBZCCModsTask.IsFaulted)
            {
                UpdateBZCCModsTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Update BZCC Mods", 0);
                    lock (Mods[AppIdBZCC])
                    {
                        List<KeyValuePair<string, ModItem>> ModList = Mods[AppIdBZCC].ToList();
                        UpdateTaskControl.Maximum = ModList.Count;
                        object CounterClock = new object();
                        int Counter = 0;
                        List<KeyValuePair<string, ModItem>> NoUpdateMods = ModList.Where(dr => !(dr.Value is SteamCmdMod) && !(dr.Value is GitMod)).ToList();
                        List<KeyValuePair<string, ModItem>> SteamCmdMods = ModList.Where(dr => (dr.Value is SteamCmdMod)).ToList();
                        List<KeyValuePair<string, ModItem>> GitMods = ModList.Where(dr => (dr.Value is GitMod)).ToList();
                        NoUpdateMods.ForEach(dr =>
                        {
                            UpdateTaskControl.Value = ++Counter;
                        });
                        Semaphore MergeTasks = new Semaphore(0,1);
                        new Thread(() =>
                        {
                            SteamCmdMods.ForEach(dr =>
                            {
                                SteamCmdMod modSteam = dr.Value as SteamCmdMod;
                                if (agressive || (modSteam?.HasUpdate ?? false))
                                {
                                    if (modSteam != null)
                                    {
                                        TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - SteamCmd - {modSteam.Workshop.WorkshopId} - {modSteam.Name}", 0);
                                        SteamCmdWorkshopDownloadException ex_ = null;
                                        do
                                        {
                                            ex_ = null;
                                            try { SteamCmd.WorkshopDownloadItem(AppIdBZCC, modSteam.Workshop.WorkshopId); } catch (SteamCmdWorkshopDownloadException ex) { ex_ = ex; }
                                        } while (ex_ != null && ex_.Message.StartsWith("ERROR! Timeout downloading item "));
                                        UpdateTaskControl.EndTask(DownloadModTaskControl);
                                    }
                                }
                                lock (CounterClock)
                                {
                                    UpdateTaskControl.Value = ++Counter;
                                }
                            });
                            MergeTasks.Release();
                        }).Start();
                        GitMods.AsParallel().WithDegreeOfParallelism(2).ForAll(dr =>
                        {
                            GitMod mod = dr.Value as GitMod;
                            if (mod != null)
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - Git - {mod.Workshop.ModWorkshopId} - {mod.Name}", 0);
                                GitContext.Pull(mod.Workshop.GitPath);
                                UpdateTaskControl.EndTask(DownloadModTaskControl);
                            }
                            lock (CounterClock)
                            {
                                UpdateTaskControl.Value = ++Counter;
                            }
                        });
                        MergeTasks.WaitOne();
                        EndTask(UpdateTaskControl);

                        UpdateBZCCModLists();
                    }
                });
            }
        }

        Task GetDependenciesBZCCModsTask = null;
        private void GetDependenciesBZCCMods()
        {
            if (GetDependenciesBZCCModsTask == null
              || GetDependenciesBZCCModsTask.IsCanceled
              || GetDependenciesBZCCModsTask.IsCompleted
              || GetDependenciesBZCCModsTask.IsFaulted)
            {
                GetDependenciesBZCCModsTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Get BZCC Mod Dependencies", 0);
                    lock (Mods[AppIdBZCC])
                    {
                        List<string> SteamCmdDependencies = new List<string>();
                        HashSet<UInt64> DependenciesGotten = new HashSet<UInt64>();
                        List<KeyValuePair<string, ModItem>> ModList = Mods[AppIdBZCC].ToList();
                        UpdateTaskControl.Maximum = ModList.Count;
                        int Counter = 0;
                        ModList.ForEach(dr =>
                        {
                            UpdateTaskControl.Value = ++Counter;
                            SteamCmdMod mod = dr.Value as SteamCmdMod;
                            if (mod != null)
                            {
                                SteamCmdDependencies.AddRange(BZCCTools.GetAssetDependencies($"steamcmd\\steamapps\\workshop\\content\\{mod.AppId}\\{mod.Workshop.WorkshopId}"));
                                DependenciesGotten.Add(mod.Workshop.WorkshopId);
                            }
                        });
                        EndTask(UpdateTaskControl);
                        List<string> SteamCmdDependenciesList = SteamCmdDependencies.Distinct().ToList();
                        UpdateTaskControl = AddTask("Download BZCC Mod Dependencies", SteamCmdDependenciesList.Count);
                        object CounterClock = new object();
                        Counter = 0;
                        SteamCmdDependenciesList.ForEach(dr =>
                        {
                            UInt64 tmpLong = 0;
                            if (UInt64.TryParse(dr, out tmpLong) && !DependenciesGotten.Contains(tmpLong))
                            {
                                TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - SteamCmd - {tmpLong}", 0);
                                SteamCmdWorkshopDownloadException ex_ = null;
                                do
                                {
                                    ex_ = null;
                                    try { SteamCmd.WorkshopDownloadItem(AppIdBZCC, tmpLong); } catch (SteamCmdWorkshopDownloadException ex) { ex_ = ex; }
                                } while (ex_ != null && ex_.Message.StartsWith("ERROR! Timeout downloading item "));
                                UpdateTaskControl.EndTask(DownloadModTaskControl);
                            }
                            lock (CounterClock)
                            {
                                UpdateTaskControl.Value = ++Counter;
                            }
                        });
                        EndTask(UpdateTaskControl);
                        this.Invoke((MethodInvoker)delegate
                        {
                            UpdateBZCCModLists();
                        });
                    }
                });
            }
        }

        public class SettingsContainer
        {
            public string BZ98RSteamPath { get; set; }
            public string BZCCSteamPath { get; set; }
            public string BZ98RGogPath { get; set; }
            public string BZCCMyDocsPath { get; set; }
            public bool FallbackSteamCmdHandling { get; set; }
        }
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
            if(tabControl1.SelectedIndex == 2)
            {
                txtBZ98RSteam.Text = settings.BZ98RSteamPath;
                txtBZCCSteam.Text = settings.BZCCSteamPath;
                txtBZ98RGog.Text = settings.BZ98RGogPath;
                txtBZCCMyDocs.Text = settings.BZCCMyDocsPath;
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

        private void btnBZCCMyDocsApply_Click(object sender, EventArgs e)
        {
            settings.BZCCMyDocsPath = txtBZCCMyDocs.Text;
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

        private void btnGOGBZCCASM_Click(object sender, EventArgs e)
        {
            if(ofdGOGBZCCASM.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (BZCCTools.CheckGameNeedsAsmPatch(ofdGOGBZCCASM.FileName))
                    {
                        try
                        {
                            BZCCTools.ApplyGameAsmPatch(ofdGOGBZCCASM.FileName);
                            MessageBox.Show("File patched, a backup of the original was created.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("File does not appear to require patching.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGOGBZCCASMAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Version 2.0.180 of Battlezone Combat Commander has a bug that prevents joining some modded Multiplayer Games. When a modded game uses a mod that has dependencies, GOG is unable to join the session because it thinks it doesn't have them. Steam is unaffected because it can download mods, so this check is skipped. This patch will skip checking all mods except the first (the config mod) which will validate properly.", "About", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void pnlTasks_Resize(object sender, EventArgs e)
        {
            pnlTasks.Refresh();
        }

        private void DisableEverything()
        {
            lvModsBZ98R.Enabled = false;
            lvModsBZCC.Enabled = false;
            btnBZ98RSteamApply.Enabled = false;
            btnBZCCMyDocsApply.Enabled = false;
            btnBZCCSteamApply.Enabled = false;
            btnDependenciesBZ98R.Enabled = false;
            btnDownloadBZ98R.Enabled = false;
            btnDownloadBZCC.Enabled = false;
            btnGOGBZCCASM.Enabled = false;
            btnGOGBZCCASMAbout.Enabled = false;
            btnRefreshBZ98R.Enabled = false;
            btnRefreshBZCC.Enabled = false;
            btnUpdateBZ98R.Enabled = false;
            btnUpdateBZCC.Enabled = false;
            txtBZ98RGogApply.Enabled = false;
            txtBZ98RGog.Enabled = false;
            txtBZ98RSteam.Enabled = false;
            txtBZCCMyDocs.Enabled = false;
            txtBZCCSteam.Enabled = false;
            txtDownloadBZ98R.Enabled = false;
            txtDownloadBZCC.Enabled = false;
        }
    }

    public enum InstallStatus
    {
        Unknown,
        Uninstalled,
        ForceDisabled,
        ForceEnabled,
        Linked,
        Collision,
    }

    public abstract class ModItem : ILinqListViesItem
    {
        public abstract string UniqueID { get; }
        public abstract InstallStatus InstalledSteam { get; }
        public abstract InstallStatus InstalledGog { get; }
        public int AppId { get; protected set; }
        public abstract string ModType { get; }
        public abstract string[] ModTags { get; }
        public abstract string WorkshopIdOutput { get; }
        public abstract string ModSource { get; }

        public string IconKey { get { return UniqueID; } }
        public string Name { get { return ToString(); } }
        public Image LargeIcon { get; set; }
        public Image SmallIcon { get; set; }
        public ListViewItem ListViewItemCache { get; set; }
        public bool HasUpdate { get; internal set; }

        public override string ToString()
        {
            //if (Workshop != null) return Workshop.WorkshopId.ToString();
            return "UNKNOWN MOD";
        }

        public abstract void ToggleGog();
        public abstract void ToggleSteam();
    }

    public class SteamMod : ModItem
    {
        public long WorkshopId { get; set; }

        public override string UniqueID { get { return GetUniqueId(WorkshopId); } }
        public static string GetUniqueId(long workshopId) { return workshopId.ToString().PadLeft(long.MaxValue.ToString().Length, '0') + "-Steam"; }
        public override InstallStatus InstalledSteam { get { return InstallStatus.ForceEnabled; } } // forced
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
                        string[] ModTypes = BZ98RTools.GetModTypes(Path.Combine(workshopFolder, WorkshopId.ToString()));
                        if (ModTypes?.Length > 0)
                        {
                            return string.Join(", ", ModTypes);
                        }
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                        string ModType = BZCCTools.GetModType(Path.Combine(workshopFolder, WorkshopId.ToString()));
                        if (ModType != null) return ModType;
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
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                    {
                        string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZ98RSteamPath, AppId);
                        string[] ModTags = BZ98RTools.GetModTags(Path.Combine(workshopFolder, WorkshopId.ToString()));
                        return ModTags;
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
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

        public SteamMod(int AppId, long WorkshopId)
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
                    string[] ModNames = BZ98RTools.GetModNames(Path.Combine(workshopFolder, WorkshopId.ToString()));
                    if (ModNames?.Length > 0)
                    {
                        if (ModNames.Length == 1) return ModNames[0];
                        return string.Join(" / ", ModNames);
                    }
                }

            }
            if (AppId == MainForm.AppIdBZCC)
            {
                if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                {
                    string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                    string ModName = BZCCTools.GetModName(Path.Combine(workshopFolder, WorkshopId.ToString()));
                    if (ModName != null) return ModName;
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
            //if (InstalledSteam == InstallStatus.Uninstalled) { }
            //ListViewItemCache = null;
        }
    }

    public class SteamCmdMod : ModItem
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
                        JunctionPoint.Create(destinationFolder, sourceFolder, false);
                    }
                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    if ((MainForm.settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                    {
                        string sourceFolder = Path.GetFullPath($"steamcmd\\steamapps\\workshop\\content\\{AppId}\\{Workshop.WorkshopId}");
                        string destinationFolder = Path.Combine(MainForm.settings.BZCCMyDocsPath, "gogWorkshop", Workshop.WorkshopId.ToString());

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
    }

    public class GitMod : ModItem
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
                    string[] ModTypes = BZ98RTools.GetModTypes(Workshop.ModPath);
                    if (ModTypes?.Length > 0)
                    {
                        return string.Join(", ", ModTypes);
                    }

                }
                if (AppId == MainForm.AppIdBZCC)
                {
                    string ModType = BZCCTools.GetModType(Workshop.ModPath, Workshop.ModWorkshopId);
                    if (ModType != null) return ModType;
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
                    string[] ModNames = BZ98RTools.GetModNames(Path.Combine(workshopFolder, Workshop.ModWorkshopId));
                    if (ModNames?.Length > 0)
                    {
                        return string.Join(" / ", ModNames);
                    }
                }
            }
            if (AppId == MainForm.AppIdBZCC)
            {
                if ((MainForm.settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                {
                    string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, AppId);
                    string ModName = BZCCTools.GetModName(Path.Combine(workshopFolder, Workshop.ModWorkshopId));
                    if (ModName != null) return ModName;
                }
            }
            return "UNKNOWN MOD";
        }
    }
}
