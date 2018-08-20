using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager
{
    class SteamCmdException : Exception
    {
        public SteamCmdException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }

    class SteamCmdMissingException : SteamCmdException
    {
        public SteamCmdMissingException(string msg)
            : base(msg)
        {
        }
    }

    class SteamCmdWorkshopDownloadException : SteamCmdException
    {
        public SteamCmdWorkshopDownloadException(string msg)
            : base(msg)
        {
        }
    }

    public enum SteamCmdStatus
    {
        Unknown,
        Extracting,
        Downloading,
        Installed,
        Starting,
        Active,
        Exiting,
        Closed,
        LoggedIn,
        LoggedInAnon,
    }

    public class SteamCmdStatusChangeEventArgs : EventArgs
    {
        public SteamCmdStatus Status = SteamCmdStatus.Unknown;

        public SteamCmdStatusChangeEventArgs(SteamCmdStatus Status) : base()
        {
            this.Status = Status;
        }
    }

    public class SteamCmdCommandChangeEventArgs : EventArgs
    {
        public string Command = null;
        public string[] Arguments = null;

        public SteamCmdCommandChangeEventArgs(string Command, params string[] Arguments) : base()
        {
            this.Command = Command;
            this.Arguments = Arguments;
        }
    }

    public class SteamCmdContext : IDisposable
    {
        private static string SteamCmdDownloadURL = @"https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

        private static readonly object CoreInstanceMutex = new object();
        private static SteamCmdContext CoreInstance;
        private SteamCmdContext()
        {
            // No more free init, force ppl to use the context properly
            //Init(); // attempt a free init
        }

        #region Dispose
        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Shutdown();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~SteamCmdContext()
        {
            Dispose(false);
        }
        #endregion Dispose

        public static SteamCmdContext GetInstance()
        {
            lock (CoreInstanceMutex)
            {
                if (CoreInstance == null)
                    CoreInstance = new SteamCmdContext();
                return CoreInstance;
            }
        }

        Process proc;
        object procLock = new object();
        object ioLock = new object();

        public bool Active
        {
            get
            {
                lock(procLock)
                {
                    return proc != null && !proc.HasExited && proc.StartTime != null;
                }
            }
        }

        public SteamCmdStatus Status { get; private set; }
        public string Command { get; private set; }

        public delegate void SteamCmdStatusChangeEventHandler(object sender, SteamCmdStatusChangeEventArgs e);
        public event SteamCmdStatusChangeEventHandler SteamCmdStatusChange;

        public delegate void SteamCmdCommandChangeEventHandler(object sender, SteamCmdCommandChangeEventArgs e);
        public event SteamCmdCommandChangeEventHandler SteamCmdCommandChange;

        public delegate void SteamCmdOutputEventHandler(object sender, string msg);
        public event SteamCmdOutputEventHandler SteamCmdOutput;

        public delegate void SteamCmdOutputFullEventHandler(object sender, string msg);
        public event SteamCmdOutputEventHandler SteamCmdOutputFull;

        public delegate void SteamCmdInputEventHandler(object sender, string msg);
        public event SteamCmdInputEventHandler SteamCmdInput;

        public void Download()
        {
            lock (procLock)
            {
                if (File.Exists("steamcmd\\steamcmd.exe")) return;
                string steamcmdzip = Path.GetFileName(SteamCmdDownloadURL);
                if (!File.Exists(steamcmdzip))
                {
                    OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.Downloading));
                    WebClient client = new WebClient();
                    client.DownloadFile(SteamCmdDownloadURL, steamcmdzip);
                }
                if (!Directory.Exists("steamcmd")) Directory.CreateDirectory("steamcmd");
                OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.Extracting));
                ZipFile.ExtractToDirectory(steamcmdzip, "steamcmd");
                OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.Installed));
            }
        }

        public void Init()
        {
            ForceKill();

            lock (procLock)
            {
                if (!Directory.Exists("steamcmd")) throw new SteamCmdMissingException("steamcmd directory missing");
                if (!File.Exists("steamcmd\\steamcmd.exe")) throw new SteamCmdMissingException("SteamCmdContext.exe");

                proc = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        //WorkingDirectory = "steamcmd",
                        FileName = "ConsoleBufferProxy.exe",
                        Arguments = "steamcmd\\steamcmd.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        StandardOutputEncoding = Encoding.Unicode,
                        StandardErrorEncoding = Encoding.Unicode,
                    }
                };

                //proc.OutputDataReceived += (sender, e) =>
                //{
                //    Console.WriteLine(e.Data);
                //};

                OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.Starting));
                proc.Start();
                //string waitOutput = WaitForSteamPrompt();
                while (!ReadLine().Prompt) { }
                OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.Active));
            }
        }

        public void ForceKill()
        {
            Process
            .GetProcessesByName("steamcmd")
            .Where(dr => dr.MainModule?.FileName == Path.GetFullPath("steamcmd\\steamcmd.exe"))
            .ToList()
            .ForEach(dr => dr.Kill());
        }

        protected void OnSteamCmdStatusChange(SteamCmdStatusChangeEventArgs e)
        {
            Status = e.Status;

            //var tmp = SteamCmdStatusChange;
            //if(tmp != null)
            //    tmp(this, e);
            SteamCmdStatusChange?.Invoke(this, e);
        }

        protected void OnSteamCmdCommandChange(SteamCmdCommandChangeEventArgs e)
        {
            Command = e.Command;
            SteamCmdCommandChange?.Invoke(this, e);
        }

        protected void OnSteamCmdOutput(string msg)
        {
            SteamCmdOutput?.Invoke(this, msg);
        }

        protected void OnSteamCmdOutputFull(string msg)
        {
            SteamCmdOutputFull?.Invoke(this, msg);
        }

        protected void OnSteamCmdInput(string msg)
        {
            SteamCmdInput?.Invoke(this, msg);
        }

        public void LoginAnonymous()
        {
            Debug.WriteLine("LoginAnonymous", "SteamCmdContext");
            Debug.Indent();

            try
            {
                OnSteamCmdCommandChange(new SteamCmdCommandChangeEventArgs("login", "anonymous"));
                WriteLine("login anonymous");
                //string output = WaitForSteamPrompt();
                while (!ReadLine().Prompt) { }
                OnSteamCmdCommandChange(new SteamCmdCommandChangeEventArgs(null));
                OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.LoggedInAnon));
            }
            finally
            {
                Debug.Unindent();
            }
        }

        public string WorkshopDownloadItem(int appId, int workshopId)
        {
            Debug.WriteLine($"WorkshopDownloadItem({appId},{workshopId})", "SteamCmdContext");
            Debug.Indent();

            try
            {
                lock (ioLock)
                {
                    OnSteamCmdCommandChange(new SteamCmdCommandChangeEventArgs("workshop_download_item", appId.ToString(), workshopId.ToString()));
                    WriteLine($"workshop_download_item {appId} {workshopId}");
                    //string commandLine = ReadLine();
                    //string statusLine = ReadLine();
                    //while(statusLine.Length == 0 || statusLine == "\r\n" || Regex.IsMatch(statusLine,@"Downloading item [0-9]+ \.\.\."))
                    //{
                    //    statusLine = ReadLine();
                    //}

                    SteamCmdLine output = null;
                    while ((output = ReadLine()) == null || output.Blank || Regex.IsMatch(output.Line, @"Downloading item [0-9]+ \.\.\.")) { }
                    string statusLine = output.Line;

                    OnSteamCmdCommandChange(new SteamCmdCommandChangeEventArgs(null));

                    //Success. Downloaded item 1325933293 to "D:\Data\Programming\BZRModManager\BZRModManager\bin\steamcmd\steamapps\workshop\content\624970\1325933293" (379069888 bytes)
                    //ERROR! Download item 1 failed (File Not Found).
                    //ERROR! Failed to start downloading item 0.

                    //WaitForSteamPrompt();
                    while (!ReadLine().Prompt) { }

                    if (statusLine.StartsWith("ERROR! "))
                    {
                        string errorText = statusLine.Split(new string[] { "ERROR! " }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                        throw new SteamCmdWorkshopDownloadException(errorText);
                    }
                    else if (statusLine.StartsWith("Success. "))
                    {
                        string successText = statusLine.Split(new string[] { "Success. " }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                        //string tmp = @"Downloaded item 1325933293 to ""D:\Data\Programming\BZRModManager\BZRModManager\bin\steamcmd\steamapps\workshop\content\624970\1325933293"" (379069888 bytes)";
                        return successText;
                    }
                    else if (proc.HasExited)
                    {
                        Exception ex = new SteamCmdException("Application Terminated", new SteamCmdWorkshopDownloadException(statusLine));
                        throw ex;
                    }
                    else
                    {
                        Exception ex = new SteamCmdException("Unknown Error", new SteamCmdWorkshopDownloadException(statusLine));
                        throw ex;
                    }
                }
            }
            finally
            {
                Debug.Unindent();
            }
        }

        private Regex WorkshopStatusPattern = new Regex(@"^- Item (?<workshopId>\d+) : (?<status>\w+) \( *(?<size>\d+) bytes, (?<dayw>\w{3}) (?<month>\w{3}) (?<day>[ 0-9]{2}) (?<hour>\d{2}):(?<minutes>\d{2}):(?<seconds>\d{2}) (?<year>\d{4})\),( (?<status2>\w+) (?<status3>\w+)\( (?<size2>\d+)/(?<size3>\d+) bytes \),)?", RegexOptions.IgnoreCase);
        //- Item 1316733327 : installed (  13058599 bytes, Thu Mar  1 00:11:28 2018),
        //- Item 1325933293 : installed ( 379069888 bytes, Mon May 14 18:20:25 2018),
        //- Item 1329659846 : installed ( 753230735 bytes, Fri May 25 02:30:45 2018),
        //- Item 1329976523 : installed (1736190673 bytes, Wed May 30 04:05:38 2018), updated required( 0/0 bytes ),
        //- Item 1369551364 : installed (    709772 bytes, Tue Apr 24 14:02:24 2018),
        //- Item 1374954711 : installed ( 628725126 bytes, Tue May  1 01:25:24 2018),
        //- Item 1374955473 : installed ( 108815664 bytes, Mon May  7 14:32:47 2018),
        //- Item 1374955975 : installed (   1006760 bytes, Tue May  1 01:28:04 2018),
        //- Item 1374956537 : installed ( 162314192 bytes, Tue May  1 01:29:29 2018),
        //- Item 1386405185 : installed ( 168116554 bytes, Tue May 15 03:06:19 2018),

        public List<WorkshopItemStatus> WorkshopStatus(int appId)
        {
            Debug.WriteLine($"WorkshopStatus({appId})", "SteamCmdContext");
            Debug.Indent();

            try
            {
                lock (ioLock)
                {
                    OnSteamCmdCommandChange(new SteamCmdCommandChangeEventArgs("workshop_status", appId.ToString()));
                    WriteLine($"workshop_status {appId}");
                    ////string commandLine = ReadLine();
                    //string statusLine = ReadLine();
                    //while (statusLine.Length == 0 || statusLine == "\r\n" || Regex.IsMatch(statusLine, @"Local workshop items for App [0-9]+: "))
                    //{
                    //    statusLine = ReadLine();
                    //}

                    SteamCmdLine output = null;
                    while ((output = ReadLine()) == null || output.Blank || Regex.IsMatch(output.Line, @"Local workshop items for App [0-9]+: ")) { }
                    string statusLine = output.Line;

                    OnSteamCmdCommandChange(new SteamCmdCommandChangeEventArgs(null));

                    if (statusLine.Equals(" No workshop items referenced on client."))
                    {
                        //WaitForSteamPrompt(); // should just be 1 null line
                        while (!ReadLine().Prompt) { }
                        return null;
                    }
                    else if (statusLine.StartsWith(" Workshop Content folder : "))
                    {
                        string mods = WaitForSteamPrompt();

                        return mods
                            .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(dr => WorkshopStatusPattern.Match(dr))
                            .Where(dr => dr.Success)
                            .Select(dr =>
                            {
                                string datetimeString = $"{dr.Groups["day"].Value} {dr.Groups["month"].Value} {dr.Groups["year"].Value} {dr.Groups["hour"].Value}:{dr.Groups["minutes"].Value}:{dr.Groups["seconds"].Value}";

                                return new WorkshopItemStatus()
                                {
                                    WorkshopId = long.Parse(dr.Groups["workshopId"].Value),
                                    Status = dr.Groups["status"].Value,
                                    Size = long.Parse(dr.Groups["size"].Value),
                                    DateTime = DateTime.Parse(datetimeString),
                                    HasUpdate = dr.Groups["status2"]?.Value == "updated" && dr.Groups["status3"]?.Value == "required",
                                };
                            })
                            .ToList();
                    }
                    else
                    {
                        return null;
                    }
                    //Steam>workshop_status 301650
                    //Local workshop items for App 301650:
                    // No workshop items referenced on client.
                    //
                    //Steam>workshop_download_item 301650 1
                    //Downloading item 1 ...
                    //ERROR! Download item 1 failed (File Not Found).
                    //Steam>workshop_status 301650
                    //Local workshop items for App 301650:
                    // Workshop Content folder : "D:\Data\Programming\BZRModManager\BZRModManager\bin\steamcmd\steamapps\workshop" - no update needed
                    //- Item 1316733327 : installed (13058599 bytes, Thu Mar  1 00:11:28 2018),
                    //- Item 1325933293 : installed (379069888 bytes, Mon May 14 18:20:25 2018),
                    //- Item 1329659846 : installed (753230735 bytes, Fri May 25 02:30:45 2018),
                    //- Item 1329976523 : installed (1736190673 bytes, Wed May 30 04:05:38 2018), updated required( 0/0 bytes ),
                    //- Item 1369551364 : installed (709772 bytes, Tue Apr 24 14:02:24 2018),
                    //- Item 1374954711 : installed (628725126 bytes, Tue May  1 01:25:24 2018),
                    //- Item 1374955473 : installed (108815664 bytes, Mon May  7 14:32:47 2018),
                    //- Item 1374955975 : installed (1006760 bytes, Tue May  1 01:28:04 2018),
                    //- Item 1374956537 : installed (162314192 bytes, Tue May  1 01:29:29 2018),
                    //- Item 1386405185 : installed (168116554 bytes, Tue May 15 03:06:19 2018),
                    //
                }
            }
            finally
            {
                Debug.Unindent();
            }
        }

        private void WriteLine(string input)
        {
            Debug.WriteLine($"WriteLine({input})", "SteamCmdContext");

            OnSteamCmdInput(input + "\r\n");
            proc.StandardInput.WriteLine(input);
        }

        private class SteamCmdLine
        {
            public string Line { get; set; }
            public bool Prompt { get; set; }
            public bool ProcDead { get; set; }
            public bool Blank { get; set; }
        }
        private SteamCmdLine ReadLine()
        {
            Debug.WriteLine("ReadLine()", "SteamCmdContext");
            Debug.Indent();

            try
            {
                lock (ioLock)
                {
                    string retVal = string.Empty;
                    string tmpVal = null;

                    do
                    {
                        tmpVal = ReadLineOrNullTimeout(1000);
                        retVal += tmpVal;
                    } while (tmpVal != null && !tmpVal.EndsWith("\r\n") && (retVal != "Steam>"));

                    return new SteamCmdLine()
                    {
                        Line = retVal,
                        Prompt = (retVal == "Steam>"),
                        ProcDead = proc.HasExited,
                        Blank = retVal.Trim().Length == 0
                    };
                }
            }
            finally
            {
                Debug.Unindent();
            }
        }

        private string WaitForSteamPrompt()
        {
            Debug.WriteLine("WaitForSteamPrompt()", "SteamCmdContext");
            Debug.Indent();

            try
            {
                lock (ioLock)
                {
                    string retVal = string.Empty;
                    SteamCmdLine output = null;
                    while ((output = ReadLine()) == null || !output.Prompt) { retVal += output.Line; }
                    return retVal;
                }
            }
            finally
            {
                Debug.Unindent();
            }
        }

        /*private string ReadLine()
        {
            lock (ioLock)
            {
                return ReadLineOrNullTimeout(1000);
            }
        }*/

        private string ReadLineOrNullTimeout(int timeout)
        {
            string badstring = "\\src\\common\\contentmanifest.cpp (650) : Assertion Failed: !m_bIsFinalized\r\n";

            Debug.WriteLine($"ReadLineOrNullTimeout({timeout})", "SteamCmdContext");
            Debug.Indent();

            try
            {
                lock (ioLock)
                {
                    int timer = 0;
                    string chars = string.Empty;
                    string charsAll = string.Empty;
                    char tP = '\0';
                    char t = '\0';
                    bool forceOnce = true;

                    for (; ; )
                    {
                        if (forceOnce || proc.StandardOutput.Peek() > -1)
                        {
                            forceOnce = false;

                            tP = t;
                            int tn = proc.StandardOutput.Read();
                            t = (char)tn;
                            if (t == '\0') break;
                            //if (tn > 255) break;
                            chars += t;
                            charsAll += t;
                            if (chars == "Steam>")
                            {
                                while (proc.StandardOutput.Peek() > -1)
                                {
                                    // this should only happen if we have more badstrings
                                    if (proc.StandardOutput.Peek() == badstring[0])
                                    {
                                        for (int i = 0; i < badstring.Length; i++)
                                        {
                                            proc.StandardOutput.Read();
                                        }
                                    }
                                }
                                break;
                            }
                            if (tP == '\r' && t == '\n')
                            {
                                if (chars.EndsWith(badstring))
                                {
                                    // we have the bad string, so let's remove it as it's the real cause of our CRLF
                                    chars = chars.Replace(badstring, string.Empty);
                                    t = (char)0;
                                }
                                else
                                {
                                    // this is a normal end of line, we are good now
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // we are null, which means it's time to timeout
                            if (timer >= timeout || proc.HasExited)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(10);
                                timer += 10;
                            }
                        }
                    }

                    OnSteamCmdOutputFull(charsAll);
                    OnSteamCmdOutput(chars);
                    Debug.WriteLine($"return \"{chars.Replace("\r", "\\r").Replace("\n", "\\n")}\"", "SteamCmdContext");
                    return chars;
                }
            }
            finally
            {
                Debug.Unindent();
            }
        }

        /*private string WaitForSteamPrompt()
        {
            lock (ioLock)
            {
                StringBuilder output = new StringBuilder();

                string raw = null;
                while ((raw = ReadLineSkipNull()) != "Steam>" && !proc.HasExited)
                {
                    output.Append(raw);
                }

                return output.ToString();
            }
        }

        private string ReadLine()
        {

            lock (ioLock)
            {
                string raw = string.Empty;

                do
                {
                    raw += ReadLineSmart();
                } while (!raw.EndsWith("\r\n") && !proc.HasExited);

                return raw;
            }
        }

        private string ReadLineSkipNull()
        {
            lock (ioLock)
            {
                string raw = null;
                for (; !proc.HasExited;)
                {
                    raw = ReadLineSmart();
                    if (raw != null) break;
                    Thread.Sleep(100);
                }
                return raw;
            }
        }

        // read a line ending with a newline or end of stream
        // note a line read attempt is forced, even if the stream says it is empty, so we can lock up if not careful
        private string ReadLineSmart()
        {
            string badstring = "\\src\\common\\contentmanifest.cpp (650) : Assertion Failed: !m_bIsFinalized\r\n";

            lock (ioLock)
            {
                bool have = false;
                string chars = string.Empty;
                string chars2 = string.Empty;
                // need to Read once even if Peak is -1 because this console stream breaks all logic
                // hopefully we don't get trapped here forever as a result of waiting for a char that never comes
                do
                {
                    if (proc.StandardOutput.Peek() == (int)'\r')
                    {
                        chars += (char)proc.StandardOutput.Read();
                        chars2 += '\r';
                        if (proc.StandardOutput.Peek() == (int)'\n')
                        {
                            chars += (char)proc.StandardOutput.Read();
                            chars2 += '\n';
                        }

                        if (chars.EndsWith(badstring))
                        {
                            chars = chars.Replace(badstring, string.Empty);
                            if (chars.Length == 0) have = false;

                            if (proc.StandardOutput.Peek() == -1)
                            {
                                have = true; // force an output of an empty string if we must, this is a null terminated badstring so we'll get stuck in a null track otherwise
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (proc.StandardOutput.Peek() == (int)'\n')
                    {
                        chars += (char)proc.StandardOutput.Read();
                        chars2 += '\n';

                        if (chars.EndsWith(badstring))
                        {
                            chars = chars.Replace(badstring, string.Empty);
                            if (chars.Length == 0) have = false;

                            if (proc.StandardOutput.Peek() == -1)
                            {
                                have = true; // force an output of an empty string if we must, this is a null terminated badstring so we'll get stuck in a null track otherwise
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (chars.Length > 0) have = true;

                    if (proc.StandardOutput.Peek() == -1)
                    {
                        //have = true; // force an output of an empty string if we must, this is a null terminated badstring so we'll get stuck in a null track otherwise
                        break;
                    }

                    char t = (char)proc.StandardOutput.Read();
                    chars += t;
                    chars2 += t;
                } while (proc.StandardOutput.Peek() >= 0 && !proc.HasExited);
                OnSteamCmdOutput(chars2);
                if (!have) return null;
                return chars;
            }
        }*/



        public void Shutdown()
        {
            Debug.WriteLine("Shutdown", "SteamCmdContext");
            Debug.Indent();
            try
            {
                lock (procLock)
                    lock (ioLock)
                    {
                        if (Active)
                        {
                            OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.Exiting));
                            OnSteamCmdCommandChange(new SteamCmdCommandChangeEventArgs("exit"));
                            WriteLine("exit");//proc.StandardInput.WriteLine("exit");
                        }
                    }
                proc.WaitForExit();
                OnSteamCmdStatusChange(new SteamCmdStatusChangeEventArgs(SteamCmdStatus.Closed));
            }
            finally
            {
                Debug.Unindent();
            }
        }
    }

    public class WorkshopItemStatus
    {
        public WorkshopItemStatus()
        {
        }

        public long WorkshopId { get; set; }
        public string Status { get; set; }
        public long Size { get; set; }
        public DateTime DateTime { get; set; }
        public bool HasUpdate { get; set; }
    }
}
