using Newtonsoft.Json;
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
        public static bool HaveGit()
        {
            return File.Exists("git.exe");
        }

        public static string[] GetModBranches(string url)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "git.exe",
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
            while (git.HasExited == false)
            { Thread.Sleep(100); }
            string output = ischkout.ReadToEnd();
            string[] lines = output.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(dr => dr.Split('\t')[1]).Where(dr => dr == "refs/heads/baked" || dr == "refs/heads/baked-dev").Select(dr => dr.Substring(11)).ToArray();
        }

        public static void WorkshopDownloadItem(int appId, string url, string[] branches)
        {
            string name = url.Split('/').Last();
            if (name.EndsWith(".git")) name = name.Substring(0, name.LastIndexOf(".git"));
            foreach (string branch in branches)
            {
                string outputDir = Path.Combine("git", appId.ToString(), name, branch);
                if (!Directory.Exists(outputDir)) {
                    Directory.CreateDirectory(outputDir);
                    Checkout(url, branch, Path.GetFullPath(outputDir));
                }
            }
        }

        private static void Checkout(string url, string branch, string outputDir)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "git.exe",
                Arguments = $"clone --single-branch -b {branch} \"{url.Replace("\"", "\"\"")}\" \"{outputDir.Replace("\"", "\"\"")}\"",
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
        }

        public static List<GitModStatus> WorkshopItemsOnDrive(int appId)
        {
            string gitFolder = Path.Combine("git", appId.ToString());
            if (!Directory.Exists(gitFolder)) return new List<GitModStatus>();
            return Directory.EnumerateDirectories(gitFolder) // get individual mod folders
                .SelectMany(dr => Directory.GetDirectories(dr)) // get branches
                .Where(dr => File.Exists(Path.Combine(dr, "config.json")))
                .SelectMany(dr =>
                {
                    string basePath = dr;
                    string jsonFile = Path.Combine(basePath, "config.json");
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
                            GitPath = Path.GetDirectoryName(basePath),
                            ModName = ModName,
                            ModWorkshopId = ModWorkshopId,
                            ModPath = ModPath,
                        };
                    }).Where(dx => dx != null);
                }).ToList();
        }

        public static void Pull(string gitPath)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "git.exe",
                Arguments = $"-C \"{Path.GetFullPath(gitPath).Replace("\"", "\"\"")}\" pull",
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
