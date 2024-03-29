﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager
{
    class GitContext
    {
        public static string[] GetModBranches(string gitExePath, string url)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                //FileName = "git.exe",
                FileName = gitExePath,
                Arguments = $"ls-remote \"{url.Replace("\"", "\"\"")}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                //RedirectStandardInput = true,
                RedirectStandardError = true,
            };
            info.EnvironmentVariables.Add("GIT_TERMINAL_PROMPT", "0");
            Process git = Process.Start(info);

            System.IO.StreamReader ischkout = git.StandardOutput;
            string output = string.Empty;
            while (git.HasExited == false)
            { 
                Thread.Sleep(100);
                output += ischkout.ReadToEnd();
            }
            output += ischkout.ReadToEnd();
            string[] lines = output.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            //return lines.Select(dr => dr.Split('\t')[1]).Where(dr => dr == "refs/heads/baked" || dr == "refs/heads/baked-dev").Select(dr => dr.Substring(11)).ToArray();
            return lines.Select(dr => dr.Split('\t')[1]).Where(dr => dr.StartsWith("refs/heads/")).Select(dr => dr.Substring(11)).ToArray();
        }

        public static void WorkshopDownloadItem(string gitExePath, UInt32 appId, string url, string[] branches)
        {
            string name = url.Split('/').Last();
            if (name.EndsWith(".git")) name = name.Substring(0, name.LastIndexOf(".git"));
            foreach (string branch in branches)
            {
                string outputDir = Path.Combine("git", appId.ToString(), name, branch);
                if (!Directory.Exists(outputDir)) {
                    Directory.CreateDirectory(outputDir);
                    Checkout(gitExePath, url, branch, Path.GetFullPath(outputDir));
                }
            }
        }

        private static void Checkout(string gitExePath, string url, string branch, string outputDir)
        {
            //if (!Directory.Exists(outputDir))
            {
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    //FileName = "git.exe",
                    FileName = gitExePath,
                    Arguments = $"init",
                    WorkingDirectory = outputDir,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                info.EnvironmentVariables.Add("GIT_TERMINAL_PROMPT", "0");
                Process git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);

                // create a local repo with the needed branch
                info.Arguments = $"clone checkout -b {branch}";
                git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);

                // enable sparse checkout
                info.Arguments = $"config core.sparseCheckout true";
                git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);

                // add remote but only track target branch, omit any tags, use origin
                info.Arguments = $"remote add -t {branch} --no-tags origin \"{url.Replace("\"", "\"\"")}\"";
                git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);

                // target baked subfolder and/or config.json files, if we find a root level config.json we will have to un-sparse
                string sparseCheckoutPath = Path.Combine(outputDir, ".git\\info\\sparse-checkout");
                File.WriteAllText(sparseCheckoutPath, "config.json\r\nbaked\r\n");

                // fetch a shallow clone
                //info.Arguments = $"fetch --depth 1 origin";
                info.Arguments = $"fetch origin";
                git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);

                // pull files for working copy
                //info.Arguments = $"pull origin {branch}";
                //git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100); 

                //if(File.Exists(Path.Combine(outputDir, "config.json")))
                //{
                //    // this is a full path git, so we eliminate the sparse checkout
                //    File.Delete(sparseCheckoutPath);
                //    Pull(outputDir); // re-pull data
                //}
                Pull(gitExePath, outputDir);
            }
        }

        public static List<GitModStatus> WorkshopItemsOnDrive(int appId)
        {
            string gitFolder = Path.Combine("git", appId.ToString());
            if (!Directory.Exists(gitFolder)) return new List<GitModStatus>();
            return Directory.EnumerateDirectories(gitFolder) // get individual mod folders
                .SelectMany(dr => Directory.GetDirectories(dr)) // get branches
                .Where(dr => File.Exists(Path.Combine(dr, "config.json")) || File.Exists(Path.Combine(dr, "baked", "config.json")))
                .SelectMany(dr =>
                {
                    string basePath = dr;
                    string jsonFile = Path.Combine(basePath, "config.json");
                    if (!File.Exists(jsonFile))
                    {
                        basePath = Path.Combine(basePath, "baked");
                        jsonFile = Path.Combine(basePath, "config.json");
                    }
                    GitModConfig data = JsonConvert.DeserializeObject<GitModConfig>(File.ReadAllText(jsonFile));

                    return data.mods.Select(mod =>
                    {
                        string ModName = mod.name;
                        string ModWorkshopId = mod.workshopid;
                        string ModPath = Path.Combine(basePath, mod.folder);
                        string ModIni = Path.Combine(ModPath, ModWorkshopId + ".ini");
                        if (!File.Exists(ModIni)) return null;
                        return new GitModStatus()
                        {
                            GitPath = dr,
                            ModName = ModName,
                            ModWorkshopId = ModWorkshopId,
                            ModPath = ModPath,
                        };
                    }).Where(dx => dx != null);
                }).ToList();
        }

        public static void Pull(string gitExePath, string gitPath)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                //FileName = "git.exe",
                FileName = gitExePath,
                Arguments = $"pull",
                WorkingDirectory = gitPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                //RedirectStandardOutput = true,
                //RedirectStandardInput = true,
                //RedirectStandardError = true,
            };
            info.EnvironmentVariables.Add("GIT_TERMINAL_PROMPT", "0");
            Process git = Process.Start(info);

            //System.IO.StreamReader ischkout = git.StandardOutput;
            //string output = string.Empty;
            while (git.HasExited == false)
            {
                //output += ischkout.ReadToEnd();
                Thread.Sleep(100);
            }

            // try to fix sparse-checkout config file
            string sparseCheckoutPath = Path.Combine(gitPath, ".git\\info\\sparse-checkout");
            if (File.Exists(sparseCheckoutPath) && !File.ReadLines(sparseCheckoutPath).Contains("/*"))
            {
                if(File.Exists(Path.Combine(gitPath, "config.json")))
                {
                    File.WriteAllText(sparseCheckoutPath, "config.json\r\nbaked\r\n/*\r\n");

                    info.Arguments = $"read-tree --reset -u HEAD";
                    git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);

                    info.Arguments = $"pull";
                    git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);
                }
            }
            else
            {
                if (!File.Exists(Path.Combine(gitPath, "config.json")))
                {
                    // we are not sparse but there's no config.json
                    File.WriteAllText(sparseCheckoutPath, "config.json\r\nbaked\r\n");

                    info.Arguments = $"read-tree --reset -u HEAD";
                    git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);

                    info.Arguments = $"pull";
                    git = Process.Start(info); while (git.HasExited == false) Thread.Sleep(100);
                }
            }
        }
    }

    public class GitModConfig
    {
        public GitModConfigMod[] mods { get; set; }
    }
    public class GitModConfigMod
    {
        public string name { get; set; }
        public string folder { get; set; }
        public string image { get; set; }
        public string workshopid { get; set; }
        public string desc { get; set; }
        public string version { get; set; }
    }
    public class GitModStatus
    {
        public string GitPath { get; set; }
        public string ModName { get; set; }
        public string ModWorkshopId { get; set; }
        public string ModPath { get; set; }
    }
}
