using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data["WORKSHOP"]["modType"]?.Trim('"');
        }

        public static string GetModName(string path)
        {
            string pathini = GetIni(path);
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data["WORKSHOP"]["modName"]?.Trim('"');
        }

        public static string[] GetModTags(string path)
        {
            string pathini = GetIni(path);
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data["WORKSHOP"]["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.ToArray() ?? new string[] { };
        }

        public static string[] GetAssetDependencies(string path)
        {
            string pathini = GetIni(path);
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(pathini);
            return data["WORKSHOP"]["assetDependencies"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim())?.ToArray() ?? new string[] { };
        }
    }
}
