using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Regex TargetHeader = new Regex("^\\[WORKSHOP\\]", RegexOptions.IgnoreCase);
            Regex AnyHeader = new Regex("^\\[[^\\]]*\\]", RegexOptions.IgnoreCase);

            try
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
                        try
                        {
                            // try more agressive parsing
                            string[] RawIniLines = File.ReadAllLines(dr);
                            RawIniLines = RawIniLines.SkipWhile(line => !TargetHeader.IsMatch(line)).TakeWhile(line => !AnyHeader.IsMatch(line) || TargetHeader.IsMatch(line)).ToArray();
                            RawIniLines = RawIniLines.Where(line => line.Contains("=") && !line.StartsWith(";")).Prepend(RawIniLines[0]).ToArray();
                            IniData data = parser.Parser.Parse(string.Join("\r\n", RawIniLines));
                            var retVal = data?["WORKSHOP"]?["mapType"]?.Trim('"');
                            hadIniParseError = true; // we still had an error as we had to use agressive selection
                            return retVal;
                        }
                        catch (IniParser.Exceptions.ParsingException)
                        {
                            hadIniParseError = true;
                            return null;
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            hadIniParseError = true;
                            return null;
                        }
                    }
                }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
                error = hadIniParseError;
                return types;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                error = true;
                return new string[] { };
            }
        }

        public static string[] GetModNames(string path, out bool error)
        {
            Regex TargetHeader = new Regex("^\\[DESCRIPTION\\]", RegexOptions.IgnoreCase);
            Regex AnyHeader = new Regex("^\\[[^\\]]*\\]", RegexOptions.IgnoreCase);

            try
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
                    }
                    catch (IniParser.Exceptions.ParsingException)
                    {
                        hadIniParseError = true;
                        return null;
                    }
                    catch (System.IO.FileNotFoundException)
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
                        try
                        {
                        // try more agressive parsing
                        string[] RawIniLines = File.ReadAllLines(dr);
                            RawIniLines = RawIniLines.SkipWhile(line => !TargetHeader.IsMatch(line)).TakeWhile(line => !AnyHeader.IsMatch(line) || TargetHeader.IsMatch(line)).ToArray();
                            RawIniLines = RawIniLines.Where(line => line.Contains("=") && !line.StartsWith(";")).Prepend(RawIniLines[0]).ToArray();
                            IniData data = parser.Parser.Parse(string.Join("\r\n", RawIniLines));
                            var retVal = data?["DESCRIPTION"]?["missionName"]?.Trim('"');
                            hadIniParseError = true; // we still had an error as we had to use agressive selection
                        return retVal;
                        }
                        catch (IniParser.Exceptions.ParsingException)
                        {
                            hadIniParseError = true;
                            return null;
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            hadIniParseError = true;
                            return null;
                        }
                    }
                }).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().OrderBy(dr => dr).ToArray();
                error = hadIniParseError;
                return niceNames;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                error = true;
                return null;
            }
        }

        public static string[] GetModTags(string path)
        {
            Regex TargetHeader = new Regex("^\\[WORKSHOP\\]", RegexOptions.IgnoreCase);
            Regex AnyHeader = new Regex("^\\[[^\\]]*\\]", RegexOptions.IgnoreCase);

            try
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
                        try
                        {
                        // try more agressive parsing
                        string[] RawIniLines = File.ReadAllLines(dr);
                            RawIniLines = RawIniLines.SkipWhile(line => !TargetHeader.IsMatch(line)).TakeWhile(line => !AnyHeader.IsMatch(line) || TargetHeader.IsMatch(line)).ToArray();
                            RawIniLines = RawIniLines.Where(line => line.Contains("=") && !line.StartsWith(";")).Prepend(RawIniLines[0]).ToArray();
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
                    }
                }).Where(dr => !string.IsNullOrWhiteSpace(dr)).GroupBy(dr => dr).OrderByDescending(dr => dr.Count()).ThenBy(dr => dr.Key).Select(dr => dr.Key).ToArray();
                if (hadIniParseError)
                    return new string[] { "PARSE ERROR" }.Union(tags).ToArray();
                return tags;

            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return new string[] { "PARSE ERROR" };
            }
        }
    }
}
