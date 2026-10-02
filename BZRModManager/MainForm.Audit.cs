using BZRModManager.Audit;
using BZRModManager.ModItem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    public partial class MainForm
    {
        private void btnRunAudit_Click(object sender, EventArgs e)
        {
            RunAudit();
        }

        private void txtAuditLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.LinkText))
                Process.Start(new ProcessStartInfo(e.LinkText) { UseShellExecute = true });
        }

        // One audit log file per run; every line is also appended to the audit log box.
        FileStream audit_log = null;
        TextWriter audit_log_writer = null;

        private void OpenAuditLog()
        {
            CloseAuditLog();
            string logdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            audit_log = File.OpenWrite($"log\\{logdate}-audit.log");
            audit_log_writer = new StreamWriter(audit_log);
        }

        private void CloseAuditLog()
        {
            audit_log_writer?.Close();
            audit_log_writer = null;
            audit_log?.Close();
            audit_log = null;
        }

        private void LogAuditLine(string text)
        {
            UiInvoke(() =>
            {
                lock (txtAuditLog)
                {
                    txtAuditLog.AppendText(text + "\r\n");
                }
            });

            if (audit_log_writer != null)
            {
                lock (audit_log_writer)
                {
                    audit_log_writer.WriteLine(text);
                    audit_log_writer.Flush();
                }
            }
        }

        /// <summary>
        /// The game mod folders an audit looks at for a given platform setup:
        /// the Steam workshop content folders and the GoG mod folders. These are
        /// also the "target mod folders" where per-mod junctions live.
        /// </summary>
        private static List<ModFolder> BuildAuditModFolders()
        {
            List<ModFolder> folders = new List<ModFolder>();

            if ((settings?.BZ98RSteamPath?.Length ?? 0) > 0)
                folders.Add(new ModFolder { Platform = "Steam", Game = "BZ98R", AppId = AppIdBZ98, Path = SteamContext.WorkshopFolder(settings.BZ98RSteamPath, AppIdBZ98) });

            if ((settings?.BZCCSteamPath?.Length ?? 0) > 0)
                folders.Add(new ModFolder { Platform = "Steam", Game = "BZCC", AppId = AppIdBZCC, Path = SteamContext.WorkshopFolder(settings.BZCCSteamPath, AppIdBZCC) });

            if ((settings?.BZ98RGogPath?.Length ?? 0) > 0)
                folders.Add(new ModFolder { Platform = "GoG", Game = "BZ98R", AppId = AppIdBZ98, Path = Path.Combine(settings.BZ98RGogPath, "mods") });

            if ((settings?.BZCCMyDocsPath?.Length ?? 0) > 0)
                folders.Add(new ModFolder { Platform = "GoG", Game = "BZCC", AppId = AppIdBZCC, Path = Path.Combine(settings.BZCCMyDocsPath, "gogWorkshop") });

            return folders;
        }

        /// <summary>
        /// Runs the full audit: fetches the remote audit database, snapshots the
        /// live mod lists, runs every registered audit engine and writes the
        /// actionable findings to the audit log box and the log folder.
        /// </summary>
        private void RunAudit()
        {
            if (Interlocked.CompareExchange(ref _modAuditRunning, 1, 0) == 1)
                return; // an audit is already running

            UiInvoke(() =>
            {
                btnRunAudit.Enabled = false;
                txtAuditLog.Clear();
            });

            Task.Factory.StartNew(() =>
            {
                TaskControl UpdateTaskControl = AddTask("Mod Audit", 0);
                try
                {
                    AuditContext context = new AuditContext();
                    AuditReport report = new AuditReport();

                    {
                        // 1) Fetch the remote per-game audit databases.
                        TaskControl subtask = UpdateTaskControl.AddTask("Downloading Audit Data", 2);
                        try
                        {
                            WebClient client = new WebClient();
                            DownloadAuditData(client, @"https://gamelistassets.iondriver.com/bz98r/audit.json", "bz98r_audit.json", AppIdBZ98, context.Remote);
                            subtask.Value = 1;
                            DownloadAuditData(client, @"https://gamelistassets.iondriver.com/bzcc/audit.json", "bzcc_audit.json", AppIdBZCC, context.Remote);
                            subtask.Value = 2;
                        }
                        finally
                        {
                            UpdateTaskControl.EndTask(subtask);
                        }
                    }

                    // 2) Snapshot the live mod lists so the engines never touch UI-bound objects.
                    ModsLock.Wait();
                    try
                    {
                        foreach (var mods in Mods)
                        {
                            foreach (ModItemBase mod in mods.Value.Values)
                            {
                                context.Mods.Add(new AuditedMod
                                {
                                    AppId = mods.Key,
                                    Name = mod.Name,
                                    UniqueID = mod.UniqueID,
                                    WorkshopId = mod.WorkshopIdOutput,
                                    FilePath = mod.FilePath,
                                    ModType = mod.ModType
                                });
                            }
                        }
                    }
                    finally
                    {
                        ModsLock.Release();
                    }

                    context.ModFolders.AddRange(BuildAuditModFolders());

                    // 3) Run every audit engine, one task progress bar per engine.
                    foreach (AuditEngine engine in AuditEngine.DefaultEngines())
                    {
                        TaskControl subtask = UpdateTaskControl.AddTask(engine.Name, 0);
                        try
                        {
                            context.Progress = v => subtask.Value = v;
                            report.AddSection(engine, engine.Run(context));
                        }
                        finally
                        {
                            context.Progress = null;
                            UpdateTaskControl.EndTask(subtask);
                        }
                    }

                    // 4) Write the report to the audit log box and the log folder.
                    OpenAuditLog();
                    try
                    {
                        foreach (string line in report.ToText().Split(new[] { "\r\n" }, StringSplitOptions.None))
                            LogAuditLine(line);
                    }
                    finally
                    {
                        CloseAuditLog();
                    }
                }
                catch (Exception ex)
                {
                    LogAuditLine($"Audit failed: {ex.Message}");
                }
                finally
                {
                    // Re-enable the audit button even if auditing threw, so it can be retried.
                    UpdateTaskControl.EndTask(UpdateTaskControl);
                    UiInvoke(() =>
                    {
                        btnRunAudit.Enabled = true;
                    });
                    Interlocked.Exchange(ref _modAuditRunning, 0);
                }
            });
        }

        /// <summary>
        /// Downloads one remote audit database file and parses it into
        /// <paramref name="remote"/>. Failures (offline, bad file) are not fatal:
        /// the audit then simply runs without that game's remote data.
        /// </summary>
        private static void DownloadAuditData(WebClient client, string url, string file, int appId, Dictionary<int, Dictionary<string, RemoteAuditEntry>> remote)
        {
            try
            {
                client.DownloadFile(url, file);
            }
            catch
            {
                // Offline: fall back to whatever copy is lying around (if any).
            }

            Dictionary<string, RemoteAuditEntry> data = null;
            if (File.Exists(file))
            {
                try
                {
                    data = JsonConvert.DeserializeObject<Dictionary<string, RemoteAuditEntry>>(StripLineComments(File.ReadAllText(file)));
                }
                catch
                {
                    // A malformed audit file must never break the audit itself.
                }
            }

            if (data != null && data.Count > 0)
                remote[appId] = data;
        }

        /// <summary>
        /// The audit database files in the wild are annotated with "//" line
        /// comments, which are not valid JSON. Strip those (string literals are
        /// respected) before handing the text to the JSON parser.
        /// </summary>
        private static string StripLineComments(string text)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            int i = 0;
            bool inString = false;

            while (i < text.Length)
            {
                char c = text[i];

                if (inString)
                {
                    sb.Append(c);
                    if (c == '\\' && i + 1 < text.Length)
                    {
                        sb.Append(text[++i]);
                        i++;
                        continue;
                    }
                    if (c == '"')
                        inString = false;
                    i++;
                    continue;
                }

                if (c == '"')
                {
                    inString = true;
                    sb.Append(c);
                    i++;
                    continue;
                }

                if (c == '/' && i + 1 < text.Length && text[i + 1] == '/')
                {
                    while (i < text.Length && text[i] != '\n' && text[i] != '\r')
                        i++;
                    continue;
                }

                sb.Append(c);
                i++;
            }

            return sb.ToString();
        }
    }
}