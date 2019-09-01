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

        public static string[] GetModTypes(string path, out bool error)
        {
            IEnumerable<string> paths = GetInis(path);
            FileIniDataParser parser = new FileIniDataParser();
            bool hadIniParseError = false;
            string[] types = paths.ToList().Select(dr =>
            {
                try
                {
                    IniData data = parser.ReadFile(dr);
                    return data?["WORKSHOP"]?["mapType"]?.Trim('"');
                }
                catch (IniParser.Exceptions.ParsingException)
                {
                    hadIniParseError = true;
                    return null;
                }
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
            error = hadIniParseError;
            return types;
        }

        public static string[] GetModNames(string path, out bool error)
        {
            IEnumerable<string> paths = GetInis(path);
            FileIniDataParser parser = new FileIniDataParser();
            bool hadIniParseError = false;
            string[] niceNames = paths.ToList().Select(dr =>
            {
                try
                {
                    IniData data = parser.ReadFile(dr);
                    return data?["MODMANAGER"]?["name"]?.Trim('"');
                }catch(IniParser.Exceptions.ParsingException)
                {
                    hadIniParseError = true;
                    return null;
                }
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
            error = hadIniParseError;
            if (niceNames.Length > 0) return niceNames;
            niceNames = paths.ToList().Select(dr =>
            {
                try
                {
                    IniData data = parser.ReadFile(dr);
                    return data?["DESCRIPTION"]?["missionName"]?.Trim('"');
                }
                catch (IniParser.Exceptions.ParsingException)
                {
                    hadIniParseError = true;
                    return null;
                }
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
            error = hadIniParseError;
            return niceNames;
        }

        public static string[] GetModTags(string path)
        {
            IEnumerable<string> paths = GetInis(path);
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
                    hadIniParseError = true;
                    return new string[] { };
                }
            }).Where(dr => !string.IsNullOrWhiteSpace(dr)).GroupBy(dr => dr).OrderByDescending(dr => dr.Count()).ThenBy(dr => dr.Key).Select(dr => dr.Key).ToArray();
            if (hadIniParseError)
                return new string[] { "PARSE ERROR" }.Union(tags).ToArray();
            return tags;
        }
    }
}
