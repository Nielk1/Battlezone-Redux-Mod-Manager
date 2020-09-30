using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BZRModManager
{
    class BZCCTools
    {
        private static string GetIni(string path, string workshopID)
        {
            if(workshopID != null) return Path.Combine(path, workshopID + ".ini");
            string ID = Path.GetFileName(path);
            return Path.Combine(path, ID + ".ini");
        }

        public static string GetModType(string path, string workshopID = null)
        {
            string pathini = GetIni(path, workshopID);
            if (!File.Exists(pathini)) return null;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data?["WORKSHOP"]?["modType"]?.Trim('"');
        }

        public static string GetModName(string path, string workshopID = null)
        {
            string pathini = GetIni(path, workshopID);
            if (!File.Exists(pathini)) return null;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            string prettyName = data?["MODMANAGER"]?["name"]?.Trim('"');
            if (!string.IsNullOrWhiteSpace(prettyName)) return prettyName;
            return data?["WORKSHOP"]?["modName"]?.Trim('"');
        }

        public static string[] GetModTags(string path, string workshopID = null)
        {
            Regex TargetHeader = new Regex("^\\[WORKSHOP\\]", RegexOptions.IgnoreCase);
            Regex AnyHeader = new Regex("^\\[[^\\]]*\\]", RegexOptions.IgnoreCase);

            string[] paths = new string[] { GetIni(path, workshopID) };
            FileIniDataParser parser = new FileIniDataParser();
            bool hadIniParseError = false;
            string[] tags = paths.ToList().SelectMany(dr =>
            {
                try
                {
                    IniData data = parser.ReadFile(dr);
                    return data?["WORKSHOP"]?["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim()) ?? new string[] { };
                }
                catch (IniParser.Exceptions.ParsingException)
                {
                    try
                    {
                        // try more agressive parsing
                        string[] RawIniLines = File.ReadAllLines(dr);
                        RawIniLines = RawIniLines.SkipWhile(line => !TargetHeader.IsMatch(line)).TakeWhile(line => !AnyHeader.IsMatch(line) || TargetHeader.IsMatch(line)).ToArray();
                        IniData data = parser.Parser.Parse(string.Join("\r\n", RawIniLines));
                        var retVal = data?["WORKSHOP"]?["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim()) ?? new string[] { };
                        hadIniParseError = true; // we still had an error as we had to use agressive selection
                        return retVal;
                    }
                    catch (IniParser.Exceptions.ParsingException)
                    {
                        hadIniParseError = true;
                        return new string[] { };
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        hadIniParseError = true;
                        return new string[] { };
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        hadIniParseError = true;
                        return new string[] { };
                    }
                }
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).GroupBy(dr => dr).OrderByDescending(dr => dr.Count()).ThenBy(dr => dr.Key).Select(dr => dr.Key).ToArray();
            if (hadIniParseError)
                return new string[] { "PARSE ERROR" }.Union(tags).ToArray();
            return tags;
        }

        public static string[] GetAssetDependencies(string path, string workshopID = null)
        {
            string pathini = GetIni(path, workshopID);
            if (!File.Exists(pathini)) return null;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data?["WORKSHOP"]?["assetDependencies"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.Where(dr => dr != null && dr.Length > 0)?.ToArray() ?? new string[] { };
        }

        /// <summary>
        /// Check if the game is the highest released version at the time of this release, which means it has a bug that needs fixing
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool NeedsJoinShellFix(string path)
        {
            string exePath = path;
            if (!File.Exists(exePath)) return false;
            FileVersionInfo exeVersionInfo = FileVersionInfo.GetVersionInfo(exePath);
            if (exeVersionInfo.FileVersion != "2.0.185") return false;
            return true;
        }

        /*
        public static bool CheckGameNeedsAsmPatch(string path)
        {
            string exePath = path;
            if (!File.Exists(exePath)) return false;
            FileInfo exeInfo = new FileInfo(exePath);
            if (exeInfo.Length != 5222400) return false;
            FileVersionInfo exeVersionInfo = FileVersionInfo.GetVersionInfo(exePath);
            if (exeVersionInfo.FileVersion != "2.0.180") return false;

            byte[] hash = null;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(exePath))
                {
                    hash = md5.ComputeHash(stream);
                }
            }
            if (hash == null) return false;
            if (BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant() != "BB0995F2C191C1C65AECA499763716E3") return false;

            return true;
        }

        public static void ApplyGameAsmPatch(string path)
        {
            string exePath = path;
            string basePath = Path.GetDirectoryName(path);
            string exeBackupPath = Path.Combine(basePath, "battlezone2.exe.backup");
            for (int i = 0; File.Exists(exeBackupPath); i++)
            {
                exeBackupPath = Path.Combine(basePath, $"battlezone2.exe.{i}.backup");
            }
            File.Copy(exePath, exeBackupPath);
            using (FileStream stream = File.OpenWrite(exePath))
            {
                stream.Position = 0x210CAC; // jump to ASM instruction
                stream.WriteByte(0x14); // change it
            }
        }
        */
    }
}
