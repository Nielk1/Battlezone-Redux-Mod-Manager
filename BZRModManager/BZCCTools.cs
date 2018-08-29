using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager
{
    class BZCCTools
    {
        private static string GetIni(string path)
        {
            string ID = Path.GetFileName(path);
            return Path.Combine(path, ID + ".ini");
        }

        public static string GetModType(string path)
        {
            string pathini = GetIni(path);
            if (!File.Exists(pathini)) return null;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data?["WORKSHOP"]?["modType"]?.Trim('"');
        }

        public static string GetModName(string path)
        {
            string pathini = GetIni(path);
            if (!File.Exists(pathini)) return null;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            string prettyName = data?["MODMANAGER"]?["name"]?.Trim('"');
            if (!string.IsNullOrWhiteSpace(prettyName)) return prettyName;
            return data?["WORKSHOP"]?["modName"]?.Trim('"');
        }

        public static string[] GetModTags(string path)
        {
            string pathini = GetIni(path);
            if (!File.Exists(pathini)) return null;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data?["WORKSHOP"]?["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.ToArray() ?? new string[] { };
        }

        public static string[] GetAssetDependencies(string path)
        {
            string pathini = GetIni(path);
            if (!File.Exists(pathini)) return null;
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data?["WORKSHOP"]?["assetDependencies"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.ToArray() ?? new string[] { };
        }

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
    }
}
