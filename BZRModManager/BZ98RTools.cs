using IniParser;
using IniParser.Configuration;
using IniParser.Model;
using IniParser.Parser;
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
            IEnumerable<string> paths = Directory.EnumerateFiles(path, "*.ini", SearchOption.TopDirectoryOnly)
                .Append(Path.Combine(path, "mod.ini"));
            return paths;
        }

        public static string[] GetModTypes(string path, out bool error)
        {
            bool hadIniParseError = false;

            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            try
            {
                var paths = GetInis(path);

                string[] types = paths
                    .Select(dr =>
                    {
                        try
                        {
                            if (File.Exists(dr))
                            {
                                IniData data = parser.Parse(File.ReadAllText(dr));
                                return data?["WORKSHOP"]?["mapType"]?.Trim('"');
                            }
                        }
                        catch (System.Exception)
                        {
                            // Catches rare catastrophic IO or unparseable errors
                            hadIniParseError = true;
                            return null;
                        }
                        return null;
                    })
                    .Where(dr => !string.IsNullOrWhiteSpace(dr))
                    .Distinct()
                    .OrderBy(dr => dr)
                    .ToArray();

                error = hadIniParseError;
                return types;
            }
            catch (DirectoryNotFoundException)
            {
                error = true;
                return System.Array.Empty<string>();
            }
        }

        public static string[] GetModManagerNames(string path, out bool error)
        {
            try
            {
                IEnumerable<string> paths = GetInis(path);

                var parser = new IniDataParser();
                parser.Configuration.SkipInvalidLines = true;
                parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
                parser.Configuration.AllowDuplicateSections = true;
                parser.Configuration.AllowKeysWithoutSection = true;

                bool hadIniParseError = false;
                string[] niceNames = paths.ToList().Select(dr =>
                {
                    try
                    {
                        if (File.Exists(dr))
                        {
                            IniData data = parser.Parse(File.ReadAllText(dr));
                            string? specialname = data?["MODMANAGER"]?["name"]?.Trim('"');
                            if (!string.IsNullOrEmpty(specialname))
                                return specialname;
                        }
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
                    return null;
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

        // The in-game ini based names ([DESCRIPTION]::missionName).
        public static string[] GetModMissionNames(string path, out bool error)
        {
            try
            {
                IEnumerable<string> paths = GetInis(path);

                var parser = new IniDataParser();
                parser.Configuration.SkipInvalidLines = true;
                parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
                parser.Configuration.AllowDuplicateSections = true;
                parser.Configuration.AllowKeysWithoutSection = true;

                bool hadIniParseError = false;
                string[] niceNames = paths.ToList().Select(dr =>
                {
                    try
                    {
                        if (File.Exists(dr))
                        {
                            IniData data = parser.Parse(File.ReadAllText(dr));
                            return data?["DESCRIPTION"]?["missionName"]?.Trim('"');
                        }
                    }
                    catch (IniParser.Exceptions.ParsingException)
                    {
                        /*try
                        {
                        // try more agressive parsing
                        string[] RawIniLines = File.ReadAllLines(dr);
                            RawIniLines = RawIniLines.SkipWhile(line => !TargetHeader.IsMatch(line)).TakeWhile(line => !AnyHeader.IsMatch(line) || TargetHeader.IsMatch(line)).ToArray();
                            try { RawIniLines = RawIniLines.Where(line => line.Contains("=") && !line.StartsWith(";")).Prepend(RawIniLines[0]).ToArray(); } catch { }
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
                        }*/
                        hadIniParseError = true;
                        return null;
                    }
                    return null;
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
            try
            {
                IEnumerable<string> paths = GetInis(path);

                var parser = new IniDataParser();
                parser.Configuration.SkipInvalidLines = true;
                parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
                parser.Configuration.AllowDuplicateSections = true;
                parser.Configuration.AllowKeysWithoutSection = true;

                bool hadIniParseError = false;
                string[] tags = paths.ToList().SelectMany(dr =>
                {
                    try
                    {
                        if (File.Exists(dr))
                        {
                            IniData data = parser.Parse(File.ReadAllText(dr));
                            return data?["WORKSHOP"]?["customtags"]?.Trim('"')?.Split(',')?.Select(dx => dx.Trim()) ?? new string[] { };
                        }
                    }
                    catch (IniParser.Exceptions.ParsingException)
                    {
                        /*try
                        {
                        // try more agressive parsing
                        string[] RawIniLines = File.ReadAllLines(dr);
                            RawIniLines = RawIniLines.SkipWhile(line => !TargetHeader.IsMatch(line)).TakeWhile(line => !AnyHeader.IsMatch(line) || TargetHeader.IsMatch(line)).ToArray();
                            try { RawIniLines = RawIniLines.Where(line => line.Contains("=") && !line.StartsWith(";")).Prepend(RawIniLines[0]).ToArray(); } catch { }
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
                        }*/
                        hadIniParseError = true;
                        return new string[] { };
                    }
                    return new string[] { };
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
