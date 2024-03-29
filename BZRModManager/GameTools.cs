using BZRModManager.Models;
using DynamicData;
using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BZRModManager
{
    internal class GameTools
    {
        private static IEnumerable<string> GetInis(string path)
        {
            IEnumerable<string> paths = Directory.EnumerateFiles(path, "*.ini", SearchOption.TopDirectoryOnly);
            return paths;
        }

        public static IEnumerable<ModIniData> GetModData(string path)
        {
            Regex AnyHeader = new Regex(@"^\[[^\]]*\]", RegexOptions.IgnoreCase);
            IEnumerable<string> paths = GetInis(path);
            FileIniDataParser parser = new FileIniDataParser();
            List<ModIniData> mods = new List<ModIniData>();
            foreach (string iniPath in paths)
            {
                ModIniData modIniData = new ModIniData(Path.GetFileNameWithoutExtension(iniPath));
                try
                {
                    string rawIni = File.ReadAllText(iniPath, Encoding.GetEncoding(1252)); // TODO confirm this encoding is correct for BZCC as well or if it's just for BZ98R
                    rawIni = Regex.Replace(rawIni, "^//.*$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline); // remove lines with bad comment method
                    rawIni = Regex.Replace(rawIni, @"^ *[^\[]((?!=).)* *$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline); // remove lines without an equal sign and not starting with a [ (header)
                    while (rawIni.Contains("\r"))
                        rawIni = rawIni.Replace("\r", "\n");
                    while (rawIni.Contains("\n\n"))
                        rawIni = rawIni.Replace("\n\n", "\n");
                    string rawIniRebuld = string.Empty;
                    HashSet<string> RepeatKeyDetect = new HashSet<string>();
                    foreach (string line in rawIni.Split('\n'))
                    {
                        if (AnyHeader.IsMatch(line))
                        {
                            RepeatKeyDetect = new HashSet<string>();
                            rawIniRebuld += line + "\n";
                        }
                        else
                        {
                            string key = line.Split('=', 2)[0].Trim();
                            if(key.Length > 0 && RepeatKeyDetect.Add(key))
                            {
                                rawIniRebuld += line + "\n";
                            }
                        }
                    }
                    using (var stream = new MemoryStream())
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(rawIniRebuld);
                        writer.Flush();
                        stream.Position = 0;

                        using (var reader = new StreamReader(stream))
                        {
                            IniData data = parser.ReadData(reader); ;
                            modIniData.ModName = data?["WORKSHOP"]?["modName"]?.Trim('"');
                            modIniData.ModManagerName = data?["MODMANAGER"]?["name"]?.Trim('"');
                            modIniData.ModType = data?["WORKSHOP"]?["modType"]?.Trim('"');
                            if (string.IsNullOrWhiteSpace(modIniData.ModType))
                                modIniData.ModType = data?["WORKSHOP"]?["mapType"]?.Trim('"');
                            modIniData.Description = data?["DESCRIPTION"]?["missionName"]?.Trim('"');
                            modIniData.CustomTags = data?["WORKSHOP"]?["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.ToList() ?? new List<string>();
                            modIniData.AssetDependencies = data?["WORKSHOP"]?["assetDependencies"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.Where(dr => dr != null && dr.Length > 0)?.ToList() ?? new List<string>();
                        }
                    }
                }
                catch (IniParser.Exceptions.ParsingException)
                {
                    modIniData.ParseError = true;
                }
                mods.Add(modIniData);
            }
            return mods;
        }

        /// <summary>
        /// Check if the game is the highest released version at the time of this release, which means it has a bug that needs fixing
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool BZCCNeedsJoinShellFix(string path)
        {
            string exePath = path;
            if (!File.Exists(exePath)) return false;
            FileVersionInfo exeVersionInfo = FileVersionInfo.GetVersionInfo(exePath);
            if (exeVersionInfo.FileVersion != "2.0.185") return false;
            return true;
        }
    }
}
