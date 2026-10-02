using IniParser;
using IniParser.Configuration;
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
        private static IEnumerable<string> GetInis(string path, string workshopID)
        {
            List<string> paths = new List<string>();
            if(workshopID != null) paths.Add(Path.Combine(path, workshopID + ".ini"));
            paths.Add(Path.Combine(path, $"{Path.GetFileName(path)}.ini"));
            paths.Add(Path.Combine(path, "mod.ini"));
            return paths.Distinct();
        }

        public static string GetModType(string path, string workshopID = null)
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            try
            {
                var paths = GetInis(path, workshopID);

                foreach(string pathini in paths)
                {
                    if (File.Exists(pathini)){
                        IniData data = parser.Parse(File.ReadAllText(pathini));
                        return data?["WORKSHOP"]?["modType"]?.Trim('"');
                    }
                }

            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            return null;
        }

        public static string GetModManagerName(string path, string workshopID = null)
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            try
            {
                var paths = GetInis(path, workshopID);

                foreach(string pathini in paths)
                {
                    if (File.Exists(pathini)){
                        IniData data = parser.Parse(File.ReadAllText(pathini));
                        string? specialname = data?["MODMANAGER"]?["name"]?.Trim('"');
                        if (!string.IsNullOrEmpty(specialname))
                            return specialname;
                    }
                }

            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            return null;
        }

        // The in-game ini based name ([WORKSHOP]::modName).
        public static string GetGeneratedName(string path, string workshopID = null)
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            try
            {
                var paths = GetInis(path, workshopID);

                foreach(string pathini in paths)
                {
                    if (File.Exists(pathini)){
                        IniData data = parser.Parse(File.ReadAllText(pathini));
                        return data?["WORKSHOP"]?["modName"]?.Trim('"');
                    }
                }

            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            return null;
        }

        public static string[] GetModTags(string path, string workshopID = null)
        {
            var paths = GetInis(path, workshopID);

            bool hadIniParseError = false;
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            string[] tags = null;
            foreach(string dr in paths)
            {
                try
                {
                    if (File.Exists(dr)) {
                        IniData data = parser.Parse(File.ReadAllText(dr));
                        tags = data?["WORKSHOP"]?["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.ToArray() ?? new string[] { };
                        break;
                    }
                }
                catch (IniParser.Exceptions.ParsingException)
                {
                    hadIniParseError = true;
                }
            }
            if (hadIniParseError)
                return new string[] { "PARSE ERROR" }.Union(tags).ToArray();
            return tags;
        }

        public static string[] GetAssetDependencies(string path, string workshopID = null)
        {
            var paths = GetInis(path, workshopID);

            bool hadIniParseError = false;
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            string[] assetDependencies = null;
            foreach(string dr in paths)
            {
                try
                {
                    if (File.Exists(dr)) {
                        IniData data = parser.Parse(File.ReadAllText(dr));
                        assetDependencies = data?["WORKSHOP"]?["assetDependencies"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.Where(dr => dr != null && dr.Length > 0)?.ToArray() ?? new string[] { };
                        break;
                    }
                }
                catch (IniParser.Exceptions.ParsingException)
                {
                    hadIniParseError = true;
                }
            }
            //if (hadIniParseError)
            //    return new string[] { "PARSE ERROR" }.Union(assetDependencies).ToArray();
            return assetDependencies;
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
