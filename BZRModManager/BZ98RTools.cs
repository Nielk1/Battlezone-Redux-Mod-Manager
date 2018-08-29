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
    class BZ98RTools
    {
        private static IEnumerable<string> GetInis(string path)
        {
            IEnumerable<string> paths = Directory.EnumerateFiles(path, "*.ini", SearchOption.TopDirectoryOnly);
            return paths;
        }

        public static string[] GetModTypes(string path)
        {
            IEnumerable<string> paths = GetInis(path);
            FileIniDataParser parser = new FileIniDataParser();
            return paths.ToList().Select(dr =>
            {
                IniData data = parser.ReadFile(dr);
                return data?["WORKSHOP"]?["mapType"]?.Trim('"');
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
        }

        public static string[] GetModNames(string path)
        {
            IEnumerable<string> paths = GetInis(path);
            FileIniDataParser parser = new FileIniDataParser();
            string[] niceNames = paths.ToList().Select(dr =>
            {
                IniData data = parser.ReadFile(dr);
                return data?["MODMANAGER"]?["name"]?.Trim('"');
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
            if (niceNames.Length > 0) return niceNames;
            return paths.ToList().Select(dr =>
            {
                IniData data = parser.ReadFile(dr);
                return data?["DESCRIPTION"]?["missionName"]?.Trim('"');
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
        }

        public static string[] GetModTags(string path)
        {
            IEnumerable<string> paths = GetInis(path);
            FileIniDataParser parser = new FileIniDataParser();
            return paths.ToList().SelectMany(dr =>
            {
                IniData data = parser.ReadFile(dr);
                return data?["WORKSHOP"]?["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim()) ?? new string[] { };
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).GroupBy(dr => dr).OrderByDescending(dr => dr.Count()).ThenBy(dr => dr.Key).Select(dr => dr.Key).ToArray();
        }
    }
}
