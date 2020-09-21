using Microsoft.Win32;
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

        private const int MAX_OTHER_STEAMCMD_ERROR = 5;

        object ModStatus = new object();
        Dictionary<int, Dictionary<string, ModItem>> Mods = new Dictionary<int, Dictionary<string, ModItem>>();
        Dictionary<int, Dictionary<string, WorkshopMod>> FoundMods = new Dictionary<int, Dictionary<string, WorkshopMod>>();

        FileStream steamcmd_log = null;
        TextWriter steamcmd_log_writer = null;
        FileStream steamcmdfull_log = null;
        TextWriter steamcmdfull_log_writer = null;

        public static SettingsContainer settings;
        private bool cbFallbackSteamCmdWindowHandlingSet = false;

        private bool ForceUpdateMode = false;

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

            Mods[AppIdBZ98] = new Dictionary<string, ModItem>();
            Mods[AppIdBZCC] = new Dictionary<string, ModItem>();
            FoundMods[AppIdBZ98] = new Dictionary<string, WorkshopMod>();
            FoundMods[AppIdBZCC] = new Dictionary<string, WorkshopMod>();

            List<string> Filters = new List<string>() { "new" };
            lvFindModsBZ98R.TypeFilter = Filters;
            lvFindModsBZCC.TypeFilter = Filters;

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

        Task UpdateBZ98RModListsTask = null;
        //TaskControl UpdateBZ98RModListsTaskControl = null;
        private void UpdateBZ98RModLists()
        {
            if (UpdateBZ98RModListsTask == null
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
                                        //var modTmp = new SteamCmdMod(AppIdBZ98, dr);
                                        //if (modTmp.Exists())
                                        //    Mods[AppIdBZ98][ModId] = modTmp;
                                        Mods[AppIdBZ98][ModId] = new SteamCmdMod(AppIdBZ98, dr);
                                    }
                                    else
                                    {
                                        ((SteamCmdMod)Mods[AppIdBZ98][ModId]).Workshop = dr;
                                    }
                                    Mods[AppIdBZ98][ModId].HasUpdate = dr.HasUpdate;
                                    Mods[AppIdBZ98][ModId].FolderOnlyDetection = dr.FolderOnlyDetection;
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
                            Task.Factory.StartNew(() =>
                            {
                                try
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
                                finally
                                {
                                    loadSemaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            loadSemaphore.Release();
                        }

                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();

                        lock (Mods[AppIdBZ98])
                            this.Invoke((MethodInvoker)delegate
                            {
                                lvModsBZ98R.BeginUpdate();
                                Mods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                lvModsBZ98R.DataSource = Mods[AppIdBZ98].Values.ToList<ILinqListViewItemMods>();
                                lvModsBZ98R.EndUpdate();

                                //lock (Mods[AppIdBZ98])
                                {
                                    //lock (FoundMods[AppIdBZ98]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                                    {
                                        foreach (var kv in Mods[AppIdBZ98])
                                        {
                                            if (FoundMods[AppIdBZ98].ContainsKey(kv.Key))
                                                FoundMods[AppIdBZ98][kv.Key].Known = true;
                                        }
                                        lvFindModsBZ98R.BeginUpdate();
                                        FoundMods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                        lvFindModsBZ98R.DataSource = FoundMods[AppIdBZ98].Values.ToList<ILinqListViewFindModsItem>();
                                        lvFindModsBZ98R.EndUpdate();
                                    }
                                }

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
                        Semaphore loadSemaphore = new Semaphore(0, 3);
                        Task.Factory.StartNew(() =>
                        {
                            try
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
                                    Mods[AppIdBZCC][ModId].FolderOnlyDetection = dr.FolderOnlyDetection;
                                });
                                UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
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
                            }
                            finally
                            {
                                loadSemaphore.Release();
                            }
                        });
                        if (settings.BZCCSteamPath != null)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    TaskControl UpdateTask = UpdateBZCCModListsTaskControl.AddTask("Update BZCC Mod List (Steam)", 0);
                                    HashSet<UInt64> Dependencies = new HashSet<UInt64>();
                                    SteamContext.WorkshopItemsOnDrive(settings.BZCCSteamPath, AppIdBZCC)?.ForEach(dr =>
                                    {
                                        string ModId = SteamMod.GetUniqueId(dr);
                                        if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                        {
                                            SteamMod mod = new SteamMod(AppIdBZCC, dr);
                                            Mods[AppIdBZCC][ModId] = mod;

                                            string workshopFolder = SteamContext.WorkshopFolder(MainForm.settings.BZCCSteamPath, MainForm.AppIdBZCC);
                                            string[] Deps = BZCCTools.GetAssetDependencies(Path.Combine(workshopFolder, mod.WorkshopId.ToString()));
                                            if (Deps != null)
                                            {
                                                foreach (string Dep in Deps)
                                                {
                                                    UInt64 DepL;
                                                    if (UInt64.TryParse(Dep, out DepL))
                                                    {
                                                        Dependencies.Add(DepL);
                                                    }
                                                }
                                            }
                                        }
                                    });
                                    foreach (var dr in Dependencies)
                                    {
                                        string ModId = SteamMod.GetUniqueId(dr);
                                        if (!Mods[AppIdBZCC].ContainsKey(ModId))
                                        {
                                            SteamMod mod = new SteamMod(AppIdBZCC, dr);
                                            Mods[AppIdBZCC][ModId] = mod;
                                        }
                                    }
                                    UpdateBZCCModListsTaskControl.EndTask(UpdateTask);
                                }
                                finally
                                {
                                    loadSemaphore.Release();
                                }
                            });
                        }
                        else
                        {
                            loadSemaphore.Release();
                        }
                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();
                        loadSemaphore.WaitOne();

                        lock (Mods[AppIdBZCC])
                            this.Invoke((MethodInvoker)delegate
                            {
                                lvModsBZCC.BeginUpdate();
                                Mods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                lvModsBZCC.DataSource = Mods[AppIdBZCC].Values.ToList<ILinqListViewItemMods>();
                                lvModsBZCC.EndUpdate();

                                //lock (Mods[AppIdBZCC])
                                {
                                    //lock (FoundMods[AppIdBZCC]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                                    {
                                        foreach (var kv in Mods[AppIdBZCC])
                                        {
                                            if (FoundMods[AppIdBZCC].ContainsKey(kv.Key))
                                                FoundMods[AppIdBZCC][kv.Key].Known = true;
                                        }
                                        lvFindModsBZCC.BeginUpdate();
                                        FoundMods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                                        lvFindModsBZCC.DataSource = FoundMods[AppIdBZCC].Values.ToList<ILinqListViewFindModsItem>();
                                        lvFindModsBZCC.EndUpdate();
                                    }
                                }

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
                if (restart)
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
                        string[] branches = GitContext.GetModBranches(text);
                        if (branches.Length > 0)
                        {
                            success = true;

                            using (MultiSelectDialog dlg = new MultiSelectDialog("Branch Select", branches.Select(dr => new Tuple<string, bool>(dr, dr == "baked" || dr.StartsWith("baked-")))))
                            {
                                if (dlg.ShowDialog() == DialogResult.OK)
                                {
                                    TaskControl DownloadModTaskControl = AddTask($"Download {(AppId == AppIdBZ98 ? "BZ98" : AppId == AppIdBZCC ? "BZCC" : AppId.ToString())} Mod - Git - \"{text}\"", 0);
                                    Task.Factory.StartNew(() =>
                                    {
                                        GitContext.WorkshopDownloadItem(AppId, text, dlg.Selected);
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
                            MessageBox.Show("Workshop ID was not detected, GIT download attempted.\r\ngit.exe not found in PATH.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        Semaphore MergeTasks = new Semaphore(0, 1);
                        new Thread(() =>
                        {
                            try
                            {
                                SteamCmdMods.ForEach(dr =>
                                {
                                    SteamCmdMod modSteam = dr.Value as SteamCmdMod;
                                    if (agressive || (modSteam?.HasUpdate ?? false) || (modSteam?.FolderOnlyDetection ?? false))
                                    {
                                        if (modSteam != null)
                                        {
                                            TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZ98 Mod - SteamCmd - {modSteam.Workshop.WorkshopId} - {modSteam.Name}", 0);
                                            SteamCmdException ex_ = null;
                                            int OtherErrorCounter = 0;
                                            do
                                            {
                                                ex_ = null;
                                                try
                                                {
                                                    SteamCmd.WorkshopDownloadItem(AppIdBZ98, modSteam.Workshop.WorkshopId);
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
                                            UpdateTaskControl.EndTask(DownloadModTaskControl);
                                        }
                                    }
                                    lock (CounterClock)
                                    {
                                        UpdateTaskControl.Value = ++Counter;
                                    }
                                });
                            }
                            finally
                            {
                                MergeTasks.Release();
                            }
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
                        Semaphore MergeTasks = new Semaphore(0, 1);
                        new Thread(() =>
                        {
                            try
                            {
                                SteamCmdMods.ForEach(dr =>
                                {
                                    SteamCmdMod modSteam = dr.Value as SteamCmdMod;
                                    if (agressive || (modSteam?.HasUpdate ?? false) || (modSteam?.FolderOnlyDetection ?? false))
                                    {
                                        if (modSteam != null)
                                        {
                                            TaskControl DownloadModTaskControl = UpdateTaskControl.AddTask($"Download BZCC Mod - SteamCmd - {modSteam.Workshop.WorkshopId} - {modSteam.Name}", 0);
                                            SteamCmdException ex_ = null;
                                            int OtherErrorCounter = 0;
                                            do
                                            {
                                                ex_ = null;
                                                try
                                                {
                                                    SteamCmd.WorkshopDownloadItem(AppIdBZCC, modSteam.Workshop.WorkshopId);
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
                                            UpdateTaskControl.EndTask(DownloadModTaskControl);
                                        }
                                    }
                                    lock (CounterClock)
                                    {
                                        UpdateTaskControl.Value = ++Counter;
                                    }
                                });
                            }
                            finally
                            {
                                MergeTasks.Release();
                            }
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
                                string[] Dependencies = null;
                                try
                                {
                                    Dependencies = BZCCTools.GetAssetDependencies($"steamcmd\\steamapps\\workshop\\content\\{mod.AppId}\\{mod.Workshop.WorkshopId}");
                                }
                                catch { }
                                if (Dependencies != null)
                                    SteamCmdDependencies.AddRange(Dependencies);
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
                                SteamCmdException ex_ = null;
                                int OtherErrorCounter = 0;
                                do
                                {
                                    ex_ = null;
                                    try
                                    {
                                        SteamCmd.WorkshopDownloadItem(AppIdBZCC, tmpLong);
                                    }
                                    catch (SteamCmdWorkshopDownloadException ex)
                                    {
                                        ex_ = ex;
                                        if(!ex_.Message.StartsWith("ERROR! Timeout downloading item "))
                                            OtherErrorCounter++;
                                    }
                                    catch (SteamCmdException ex)
                                    {
                                        ex_ = ex;
                                        OtherErrorCounter++;
                                    }
                                } while (ex_ != null && OtherErrorCounter < MAX_OTHER_STEAMCMD_ERROR);
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
            public string BZCCGogPath { get; set; }
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
            if (tabControl1.SelectedIndex == 4)
            {
                txtBZ98RSteam.Text = settings.BZ98RSteamPath;
                txtBZCCSteam.Text = settings.BZCCSteamPath;
                txtBZ98RGog.Text = settings.BZ98RGogPath;
                txtBZCCMyDocs.Text = settings.BZCCMyDocsPath;
                txtBZCCGog.Text = settings.BZCCGogPath;
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
            btnRefreshBZ98R.Enabled = false;
            btnRefreshBZCC.Enabled = false;
            btnUpdateBZ98R.Enabled = false;
            btnUpdateBZCC.Enabled = false;
            btnHardUpdateBZ98R.Enabled = false;
            btnHardUpdateBZCC.Enabled = false;
            btnBZ98RGogApply.Enabled = false;
            txtBZ98RGog.Enabled = false;
            txtBZ98RSteam.Enabled = false;
            txtBZCCMyDocs.Enabled = false;
            txtBZCCSteam.Enabled = false;
            txtDownloadBZ98R.Enabled = false;
            txtDownloadBZCC.Enabled = false;
            cbBZ98RTypeMod.Enabled = false;
            cbBZ98RTypeMultiplayer.Enabled = false;
            cbBZ98RTypeError.Enabled = false;
            cbBZ98RTypeCampaign.Enabled = false;
            cbBZ98RTypeInstantAction.Enabled = false;
            cbBZCCTypeError.Enabled = false;
            cbBZCCTypeConfig.Enabled = false;
            cbBZCCTypeAddon.Enabled = false;
            cbBZCCTypeAsset.Enabled = false;
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

        Task FindModsBZ98RTask = null;
        private void FindModsBZ98R(bool AutoDownload = false)
        {
            if (FindModsBZ98RTask == null
             || FindModsBZ98RTask.IsCanceled
             || FindModsBZ98RTask.IsCompleted
             || FindModsBZ98RTask.IsFaulted)
            {
                FindModsBZ98RTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZ98 Mods", 0);
                    List<WorkshopMod> ModsFound = WorkshopContext.GetMods(AppIdBZ98, null);

                    lock (ModStatus)
                    lock (Mods[AppIdBZ98])
                    {
                        //lock (FoundMods[AppIdBZ98]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                        {
                            FoundMods[AppIdBZ98].Clear();
                            foreach (WorkshopMod mod in ModsFound)
                            {
                                mod.Known = Mods[AppIdBZ98].ContainsKey(mod.UniqueID);
                                FoundMods[AppIdBZ98][mod.UniqueID] = mod;
                                if(AutoDownload && !Mods[AppIdBZ98].ContainsKey(mod.UniqueID)) // some sort of strange race condition or something, Known isn't right
                                    DownloadMod(mod.URL, AppIdBZ98);
                            }
                            EndTask(UpdateTaskControl);
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            lvFindModsBZ98R.BeginUpdate();
                            FoundMods[AppIdBZ98].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                            lvFindModsBZ98R.DataSource = FoundMods[AppIdBZ98].Values.ToList<ILinqListViewFindModsItem>();
                            lvFindModsBZ98R.EndUpdate();
                        });
                    }
                });
            }
        }

        Task FindModsBZCCTask = null;
        private void FindModsBZCC(bool AutoDownload = false)
        {
            if (FindModsBZCCTask == null
             || FindModsBZCCTask.IsCanceled
             || FindModsBZCCTask.IsCompleted
             || FindModsBZCCTask.IsFaulted)
            {
                FindModsBZCCTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZCC Mods", 0);
                    List<WorkshopMod> ModsFound = WorkshopContext.GetMods(AppIdBZCC, new string[] { "config", "addon" }); // we only need these two as Asset type can be collected via dependency scan

                    lock (ModStatus)
                    lock (Mods[AppIdBZCC])
                    {
                        //lock (FoundMods[AppIdBZCC]) // let's try using the mod collection as our lock context and ignore the FoundMods collection for locking
                        {
                            FoundMods[AppIdBZCC].Clear();
                            foreach (WorkshopMod mod in ModsFound)
                            {
                                mod.Known = Mods[AppIdBZCC].ContainsKey(mod.UniqueID);
                                FoundMods[AppIdBZCC][mod.UniqueID] = mod;
                                if(AutoDownload && !Mods[AppIdBZCC].ContainsKey(mod.UniqueID)) // some sort of strange race condition or something, Known isn't right
                                    DownloadMod(mod.URL, AppIdBZCC);
                            }
                            EndTask(UpdateTaskControl);
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            lvFindModsBZCC.BeginUpdate();
                            FoundMods[AppIdBZCC].Values.ToList().ForEach(dr => dr.ListViewItemCache = null);
                            lvFindModsBZCC.DataSource = FoundMods[AppIdBZCC].Values.ToList<ILinqListViewFindModsItem>();
                            lvFindModsBZCC.EndUpdate();
                        });
                    }
                });
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

        Task GetMpGamesBZ98RTask = null;
        private void GetMpGamesBZ98R()
        {
            if (GetMpGamesBZ98RTask == null
             || GetMpGamesBZ98RTask.IsCanceled
             || GetMpGamesBZ98RTask.IsCompleted
             || GetMpGamesBZ98RTask.IsFaulted)
            {
                GetMpGamesBZ98RTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZ98 Multiplayer Games", 0);
                    MultiplayerGamelistData data = MultiplayerSessionServer.GetMpGamesBZ98R();
                    EndTask(UpdateTaskControl);

                    this.Invoke((MethodInvoker)delegate
                    {
                        lvMultiplayerBZ98R.BeginUpdate();
                        lvMultiplayerBZ98R.DataSource = data;
                        lvMultiplayerBZ98R.EndUpdate();
                    });
                });
            }
        }

        Task GetMpGamesBZCCTask = null;
        private void GetMpGamesBZCC()
        {
            if (GetMpGamesBZCCTask == null
             || GetMpGamesBZCCTask.IsCanceled
             || GetMpGamesBZCCTask.IsCompleted
             || GetMpGamesBZCCTask.IsFaulted)
            {
                GetMpGamesBZCCTask = Task.Factory.StartNew(() =>
                {
                    TaskControl UpdateTaskControl = AddTask("Find BZCC Multiplayer Games", 0);
                    MultiplayerGamelistData data = MultiplayerSessionServer.GetMpGamesBZCC();
                    EndTask(UpdateTaskControl);

                    this.Invoke((MethodInvoker)delegate
                    {
                        lvMultiplayerBZCC.BeginUpdate();
                        lvMultiplayerBZCC.DataSource = data;
                        lvMultiplayerBZCC.EndUpdate();
                    });
                });
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
                btnMultiJoinSteam.Enabled = !string.IsNullOrWhiteSpace(settings.BZ98RSteamPath);
                btnMultiJoinGOG.Enabled = !string.IsNullOrWhiteSpace(settings.BZ98RGogPath);

                btnMultiGetModSteam.Enabled = !string.IsNullOrWhiteSpace(settings.BZ98RSteamPath);
                btnGetModSteamCmd.Enabled = !string.IsNullOrWhiteSpace(settings.BZ98RGogPath);
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
                btnMultiJoinSteam.Enabled = !string.IsNullOrWhiteSpace(settings.BZCCSteamPath);
                btnMultiJoinGOG.Enabled = !string.IsNullOrWhiteSpace(settings.BZCCGogPath);

                btnMultiGetModSteam.Enabled = !string.IsNullOrWhiteSpace(settings.BZCCSteamPath);
                btnGetModSteamCmd.Enabled = !string.IsNullOrWhiteSpace(settings.BZCCMyDocsPath);
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
                    string Password = string.Empty;
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
                    else
                    {
                        Process.Start(new ProcessStartInfo()
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
                    List<string> Mods = new List<string>();
                    Mods.Add(session.Game.Mod ?? "0");
                    if (session.Game.Mods != null)
                        Mods.AddRange(session.Game.Mods);
                    string ModsString = string.Join(";", Mods);
                    string Password = string.Empty;
                    if (session.Status.HasPassword ?? false)
                    {
                        PasswordDialog dlg = new PasswordDialog("Password");
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            Password = dlg.Password;
                            string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                            Process.Start(new ProcessStartInfo()
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
                        string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                        Process.Start(new ProcessStartInfo()
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
                    string Password = string.Empty;
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
                    else
                    {
                        Process.Start(new ProcessStartInfo()
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
                    List<string> Mods = new List<string>();
                    Mods.Add(session.Game.Mod ?? "0");
                    if (session.Game.Mods != null)
                        Mods.AddRange(session.Game.Mods);
                    string ModsString = string.Join(";", Mods);
                    string Password = string.Empty;
                    if (session.Status.HasPassword ?? false)
                    {
                        PasswordDialog dlg = new PasswordDialog("Password");
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            Password = dlg.Password;
                            string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                            Process.Start(new ProcessStartInfo()
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
                        string RichString = string.Join(null, $"N,{session.Name.Length},{session.Name},{ModsString.Length},{ModsString},{session.Address["NAT"]},{Password.Length},{Password}".Select(dr => $"{((int)dr):x2}"));
                        Process.Start(new ProcessStartInfo()
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
    }

    public enum InstallStatus
    {
        Unknown,
        Uninstalled,
        ForceDisabled,
        ForceEnabled,
        Linked,
        Collision,
        Missing,
    }

    public abstract class ModItem : ILinqListViewItemMods
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
        public bool FolderOnlyDetection { get; internal set; }

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
        public UInt64 WorkshopId { get; set; }

        public override string UniqueID { get { return GetUniqueId(WorkshopId); } }
        public static string GetUniqueId(UInt64 workshopId) { return workshopId.ToString().PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-Steam"; }
        public override InstallStatus InstalledSteam {
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
    }
}
