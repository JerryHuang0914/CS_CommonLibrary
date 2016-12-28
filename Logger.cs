using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Reflection;

namespace dev.jerry_h.pc_tools.CommonLibrary
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
            get { return realLogPath; }
        }
        public static long maxLogSize_MB = 64;
        private static int logIndex = 0;
        private static String _logPath = "";
        private static DateTime logStartTime = DateTime.MinValue;
        private static Queue<String> logMsgQueue = new Queue<String>();
        private static bool isLogWriting = false;
        private static Thread tdWriteLog;
        private static String realLogPath
        {
            get
            {
                String path = "";
                if (logIndex == 0)
                {
                    path = _logPath + (_logPath.EndsWith("\\")?"":"_") + logStartTime.ToString("yyyyMMdd_HHmmss") + ".log";
                }
                else
                {
                    path = _logPath + (_logPath.EndsWith("\\") ? "" : "_") + logStartTime.ToString("yyyyMMdd_HHmmss") + "." + logIndex;
                }
                return path;
            }
        }
        private static bool bIsCanceled = false;
        public static void Initialize(String logPath)
        {
            Initialize(logPath, 64);
        }

        public static void Initialize(String logPath, long maximumLogFileSize_MB)
        {
            logStartTime = DateTime.Now;
            maxLogSize_MB = maximumLogFileSize_MB;
            setLogPath(logPath);
            bIsCanceled = false;
        }

        public static void setLogPath(String path)
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
            createNewFile(realLogPath);
        }

        private static void createNewFile(String filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public static void WriteLog(LogLevels logLevel, String header, String logMessage,bool isWriteImmediately)
        {
            if (!bIsCanceled)
            {
                String timestamp = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "]";
                String levelStr = Enum.GetName(typeof(LogLevels), (int)logLevel).Substring(0, 1);
                String logMsg = ""; 
                if ((int)logLevel <= (int)LogLevel)
                {
                    if (logLevel == LogLevels.Super)
                    {
                        if (header != null && header.Length > 0)
                        {
                            logMsg = header + " : " + logMessage;
                        }
                        else
                        {
                            logMsg = logMessage;
                        }
                    }
                    else
                    {
                        logMsg = timestamp + "\t" +
                                 levelStr + "\t" +
                                 header + "\t" +
                                 logMessage;
                    }
                    logMsgQueue.Enqueue(logMsg);
                }
                if (LiveLogEventHandler != null)
                {
                    LiveLogEventHandler.Invoke(null, new LoggerLiveMessageEventArgs(logMsg));
                }
                if (isWriteImmediately || logMsgQueue.Count > 10)
                {
                    writeLogToFile();
                }
            }
        }

        public static void WriteLog(LogLevels logLevel, String header, String logMessage)
        {
            WriteLog(logLevel, header, logMessage, false);
        }

        public static void WriteLog(String header, String logMessage, bool isWriteImmediately)
        {
            WriteLog(LogLevels.Information, header, logMessage, isWriteImmediately);
        }

        public static void WriteLog(String header, String logMessage)
        {
            WriteLog(LogLevels.Information, header, logMessage, false);
        }

        private static void writeLogToFile()
        {
            if (!isLogWriting)
            {
                try
                {
                    FileInfo fi = new FileInfo(realLogPath);
                    if (fi.Length > maxLogSize_MB * 1024 * 1024)
                    {
                        logIndex++;
                        createNewFile(realLogPath);
                    }
                    if (tdWriteLog != null)
                    {
                        tdWriteLog.Interrupt();
                        tdWriteLog = null;
                    }
                    tdWriteLog = new Thread(writeLogToFile_Runnable);
                    tdWriteLog.Start();
                }
                catch
                {
                }
                finally
                {

                }
            }
        }

        private static void writeLogToFile_Runnable()
        {
            isLogWriting = true;
            try
            {
                StreamWriter sw = new StreamWriter(realLogPath, true);
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
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Logger exception occurred, message = " + ex.Message+"\r\n"+ex.StackTrace, "Exception catched", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning); 
            }
            finally
            {
                isLogWriting = false;
            }
        }

        public static void Cancel()
        {
            WriteLog(Logger.LogLevels.Debug, "Canceled", "Logger is canceled, clean up all of the log message(s).", true);
            bIsCanceled = true;
        }
        
        public void Dispose()
        {

        }
    
    }
    public class LoggerLiveMessageEventArgs : EventArgs
    {
        public readonly String LiveLogMessage = "";
        public LoggerLiveMessageEventArgs(String message)
        {
            LiveLogMessage = message;
        }
    }
}
