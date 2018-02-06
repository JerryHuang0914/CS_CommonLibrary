using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace jh.csharp.CommonLibrary
{
    public class Logger
    {
        public enum LogLevels { Super=-1,Error = 0, Warning = 1, Information = 2, Debug = 3, Verbose = 4 };
        public static LogLevels LogLevel = LogLevels.Debug;
        public enum LogTags
        {            
            Prerequisite,
            Action,
            Detail,
            CheckPoint,
            Counter,
            Summary, 
            Conclusion,
            ToolInfo,
            SystemInfo
        }
        public static EventHandler<LoggerLiveMessageEventArgs> LiveLogEventHandler;
        public static String CurrentLogPath
        {
            get
            {
                String path = "";
                if (logIndex == 0)
                {
                    path = _logPath + (_logPath.EndsWith("\\") ? "" : "_") + logStartTime.ToString("yyyyMMdd_HHmmss") + ".log";
                }
                else
                {
                    path = _logPath + (_logPath.EndsWith("\\") ? "" : "_") + logStartTime.ToString("yyyyMMdd_HHmmss") + "." + logIndex;
                }
                return path;
            }
        }
        public static long maxLogSize_MB = 64;
        private static int logIndex = 0;
        private static String _logPath = "";
        private static DateTime logStartTime = DateTime.MinValue;
        private static Queue<String> logMsgQueue = new Queue<String>();
        private static object write_lock = new object();
        private static bool bIsCanceled = false;
        public static void Initialize(String log_path,LogLevels log_level=LogLevels.Debug,long maximum_log_file_size_mb=64)
        {
            logStartTime = DateTime.Now;
            maxLogSize_MB = maximum_log_file_size_mb;
            SetLogPath(log_path);
            bIsCanceled = false;
        }

        public static void SetLogPath(String path)
        {
            Regex rgx = new Regex(@"(?<FileName>(\S|\s)*)(?<Extentsion>\.\S{3,4})$");
            Match m = rgx.Match(path);
            if (m.Success)
            {
                _logPath = m.Groups["FileName"].Value;
            }
            else
            {
                _logPath = path;
            }
        }

        private static void create_new_file(String filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public static void WriteLog(String header, String logMessage, LogLevels logLevel=LogLevels.Information, bool isWriteImmediately=false)
        {
            if (!bIsCanceled)
            {
                LoggerLiveMessageEventArgs log_ea = new LoggerLiveMessageEventArgs(DateTime.Now, LogLevel, header, logMessage);
                if ((int)logLevel <= (int)LogLevel)
                {
                    logMsgQueue.Enqueue(log_ea.Combined_Message);
                }
                if (LiveLogEventHandler != null)
                {
                    LiveLogEventHandler.Invoke(null,log_ea);
                }
                if (isWriteImmediately || logMsgQueue.Count > 10)
                {
                    write_log_to_file();
                }
            }
        }

        private static void write_log_to_file()
        {
            try
            {
                if (!File.Exists(CurrentLogPath))
                {
                    create_new_file(CurrentLogPath);
                }

                FileInfo fi = new FileInfo(CurrentLogPath);
                if (fi.Length > maxLogSize_MB * 1024 * 1024)
                {

                    logIndex++;
                    create_new_file(CurrentLogPath);
                }
                new Thread(write_log_to_file_runnable).Start();
            }
            catch
            {
            }
            finally
            {

            }           
        }

        private static void write_log_to_file_runnable()
        {
            lock (write_lock)
            {


                try
                {

                    StreamWriter sw = new StreamWriter(CurrentLogPath, true);
                    while (logMsgQueue.Count > 0)
                    {
                        try
                        {
                            sw.WriteLine(logMsgQueue.Dequeue());
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    if (sw != null)
                    {
                        sw.Close();
                    }
                }

                catch (Exception ex)
                {

                    System.Windows.Forms.MessageBox.Show("Logger exception occurred, message = " + ex.Message + "\r\n" + ex.StackTrace, "Exception catched", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }
		}
        public static void Cancel()
        {
            bIsCanceled = true;
            if (logMsgQueue.Count > 0)
            {
                WriteLog("Logger", "Logger is canceled, clean up all of the log message(s).", Logger.LogLevels.Debug, true);
                Thread.Sleep(1000);
            }           
        }
        
        public static void Dispose()
        {
            Cancel();
        }    
    }
    
    public class LoggerLiveMessageEventArgs : EventArgs
    {


        public DateTime LogTime { get; }
        public String LogTime_String
        {

            get
            {
                return LogTime.ToString("HH:mm:ss");
            }
        }
        public Logger.LogLevels LogLevel { get;}
        public String Header{ get; }
        public String Message { get; }
        public String Combined_Message
        {
            get
            {
                String timestamp = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "]";
                String levelStr = Enum.GetName(typeof(Logger.LogLevels), (int)LogLevel).Substring(0, 1);
                String logMsg = timestamp + "\t" +
                             levelStr + "\t" +
                             Header + "\t" +
                             Message;
                return logMsg;
            }
        }
        
        public LoggerLiveMessageEventArgs(DateTime time, Logger.LogLevels level,String header,String message)
        {
            LogTime = time;
            LogLevel = level;
            Header = header;
            Message = message;
        }
    }
}

