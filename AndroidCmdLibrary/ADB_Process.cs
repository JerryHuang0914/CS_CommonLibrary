using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace jh.csharp.AndroidCmdLibrary
{
    public class ADB_Process
    {
        private static String workingDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "adb";
        private static String adbPath = workingDirectory + "\\adb.exe";
        private static String aaptPath = workingDirectory + "\\aapt.exe";
        public static void StartADB()
        {
            Process psADB = new Process();
            try
            {
                psADB.StartInfo = new ProcessStartInfo(adbPath);
                psADB.StartInfo.WorkingDirectory = workingDirectory;
                psADB.StartInfo.Arguments = "start-server";
                psADB.StartInfo.CreateNoWindow = true;
                psADB.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psADB.StartInfo.RedirectStandardOutput = true;
                psADB.StartInfo.UseShellExecute = false;
                psADB.Start();
            }
            catch
            {
            }
            finally
            {
                if (psADB != null)
                {
                    psADB.Close();
                }
            }
        }
        public static void ADB_Reboot()
        {
            int deviceCount = GetDeivcesList().Count;
            if (deviceCount <= 0)
            {
                Process psADB = new Process();
                try
                {
                    psADB.StartInfo = new ProcessStartInfo(adbPath);
                    psADB.StartInfo.WorkingDirectory = workingDirectory;
                    psADB.StartInfo.Arguments = "reboot";
                    psADB.StartInfo.CreateNoWindow = true;
                    psADB.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    psADB.StartInfo.RedirectStandardOutput = true;
                    psADB.StartInfo.UseShellExecute = false;
                    psADB.Start();
                }
                catch
                {

                }
                finally
                {
                    if (psADB != null)
                    {
                        psADB.Close();
                    }
                }
            }
        }

        public static int RunAdbCommand(String argument, out String standardOutput, out String standardError, bool waitForExit = false)
        {
            int exitCode = -1;
            standardOutput = "";
            standardError = "";
            Process psADB = new Process();
            try
            {
                psADB.StartInfo = new ProcessStartInfo(adbPath);
                psADB.StartInfo.WorkingDirectory = workingDirectory;
                psADB.StartInfo.Arguments = argument;
                psADB.StartInfo.CreateNoWindow = true;
                psADB.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psADB.StartInfo.RedirectStandardOutput = true;
                psADB.StartInfo.RedirectStandardError = true;
                psADB.StartInfo.UseShellExecute = false;
                psADB.Start();
                if (standardOutput != null)
                {
                    standardOutput = psADB.StandardOutput.ReadToEnd().Trim(new char[] { '\n', '\r' });
                }
                if (standardError != null)
                {
                    standardError = psADB.StandardError.ReadToEnd().Trim(new char[] { '\n', '\r' });
                }
                if (waitForExit)
                {
                    psADB.WaitForExit();
                }
                exitCode = psADB.ExitCode;
            }
            catch
            {

            }
            finally
            {
                if (psADB != null)
                {
                    psADB.Close();
                }
            }
            return exitCode;
        }

        public static int RunAdbCommand(String argument,bool waitForExit=false)
        {
            String stdOutput="", stdError = "";
            return RunAdbCommand(argument, out stdOutput, out stdError, waitForExit);
        }

        public static List<String> GetPackagesList()
        {
            return GetPackagesList("", "");
        }
        public static List<String> GetPackagesList(String keyword)
        {
            return GetPackagesList("", keyword);
        }
        public static List<String> GetPackagesList(String deviceID, String keyword)
        {
            List<String> packages = new List<string>();
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            argument += "shell \"pm list packages\"";
            String returnedString = "";
            if (keyword != null && keyword.Length > 0)
            {
                argument = argument.TrimEnd('\"') + " | grep " + keyword + "\"";
            }
            Process psADB = new Process();
            try
            {
                psADB.StartInfo = new ProcessStartInfo(adbPath);
                psADB.StartInfo.WorkingDirectory = workingDirectory;
                psADB.StartInfo.Arguments = argument;
                psADB.StartInfo.CreateNoWindow = true;
                psADB.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psADB.StartInfo.RedirectStandardOutput = true;
                psADB.StartInfo.UseShellExecute = false;
                psADB.Start();
                returnedString = psADB.StandardOutput.ReadToEnd();
                packages.AddRange(returnedString.Replace("\r", "").TrimEnd('\n').Split('\n'));
                if (packages[packages.Count - 1].Length == 0)
                {
                    packages.RemoveAt(packages.Count - 1);
                }
            }
            catch
            {
            }
            finally
            {
                if (psADB != null)
                {
                    psADB.Close();
                }
            }
            return packages;
        }

        public static List<AdbDeviceInfomation> GetDeivcesList()
        {
            List<AdbDeviceInfomation> lstDeiviceList = new List<AdbDeviceInfomation>();
            Process psADB = new Process();
            try
            {
                psADB.StartInfo = new ProcessStartInfo(adbPath);
                psADB.StartInfo.WorkingDirectory = workingDirectory;
                psADB.StartInfo.Arguments = "devices";
                psADB.StartInfo.CreateNoWindow = true;
                psADB.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psADB.StartInfo.RedirectStandardOutput = true;
                psADB.StartInfo.UseShellExecute = false;
                psADB.Start();
                String str = psADB.StandardOutput.ReadToEnd().TrimEnd(new char[] { '\r', '\n' }).Replace("\r\n", "\n");
                String[] strReturnInfos = str.Split('\n');
                foreach (String info in strReturnInfos)
                {
                    if (info.Contains("\t") && info.ToLower().Contains("device")) //get the online device
                    {
                        String id = info.Split('\t')[0];
                        String status = "Connected";
                        lstDeiviceList.Add(new AdbDeviceInfomation(id, status));
                    }
                    else if (info.Contains("\t") && info.ToLower().Contains("offline")) //get the offline device
                    {
                        String id = info.Split('\t')[0];
                        String status = "Offline";
                        lstDeiviceList.Add(new AdbDeviceInfomation(id, status));
                    }
                }
                psADB.WaitForExit();
            }
            catch
            {
            }
            finally
            {
                if (psADB != null)
                {
                    psADB.Close();
                }
            }
            return lstDeiviceList;
        }

        internal static void Dial(String deviceID, String dialNumber)
        {
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            argument += "shell am start -a android.intent.action.CALL -d tel:" + dialNumber;
            RunAdbCommand(argument);
        }

        internal static String GetPhoneCallState(int timeout_inMilliSeconds)
        {
            return GetPhoneCallState("", timeout_inMilliSeconds);
        }
        internal static String GetPhoneCallState(String deviceID, int timeout_inMilliSeconds)
        {
            String result = "";
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            deleteAsusApiResult(deviceID);
            argument += "shell am startservice --user 0 -n com.asus.at/.MainService -a GetPhoneState";
            RunAdbCommand(argument);
            result = GetAsusApiResult(deviceID, timeout_inMilliSeconds);
            return result;
        }

        public static String GetPhoneNumber(String deviceID, int timeout_inMilliSeconds)
        {
            String result = "";
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            deleteAsusApiResult(deviceID);
            argument += "shell am startservice --user 0 -n com.asus.at/.MainService -a GetPhoneNumber";
            RunAdbCommand(argument);
            result = GetAsusApiResult(deviceID, timeout_inMilliSeconds);
            return result;
        }

        internal static bool AnswerCall(String deviceID, int timeout_inMilliSeconds)
        {
            bool isPhoneRing = false;
            bool isTimeout = false;
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            argument += "shell input keyevent KEYCODE_HEADSETHOOK";
            DateTime startTime = DateTime.Now;
            #region Wait the phone call
            do
            {
                isTimeout = DateTime.Now.Subtract(startTime).TotalMilliseconds > timeout_inMilliSeconds;
                String state = GetPhoneCallState(deviceID, 5000);
                isPhoneRing = state.Equals("RINGING");
                Thread.Sleep(250);
            } while (!(isTimeout || isPhoneRing));
            #endregion Wait the phone call
            if (isPhoneRing)
            {
                RunAdbCommand(argument); //Hangs up
            }
            else
            {
                EndCall(deviceID);
            }
            return isPhoneRing;
        }

        internal static void EndCall(String deviceID)
        {
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            argument += "shell service call phone 5";
            RunAdbCommand(argument);
        }

        private static void deleteAsusApiResult(String deviceID)
        {
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            argument += "shell rm -f /sdcard/ATST/ToolInfo/APIResult";
            RunAdbCommand(argument);
        }

        public static String GetAsusApiResult(String deviceID, int timeout_inMilliSeconds)
        {
            String stdOutput = "", stdError = "";
            String argument = "";
            bool isTimeout = false;
            bool isResultOK = false;
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            argument += "shell cat /sdcard/ATST/ToolInfo/APIResult";
            DateTime startTime = DateTime.Now;
            try
            {
                do
                {
                    RunAdbCommand(argument, out stdOutput, out stdError);
                    Thread.Sleep(500);
                    isResultOK = !stdOutput.ToLower().Contains("no such file or directory") && stdOutput.Trim().Length > 0;
                    isTimeout = DateTime.Now.Subtract(startTime).TotalMilliseconds > timeout_inMilliSeconds;
                } while (!(isTimeout || isResultOK));
            }
            catch (ThreadInterruptedException tie)
            { }
            deleteAsusApiResult(deviceID);
            if (isResultOK)
            {
                return stdOutput;
            }
            else
            {
                return "ERROR";
            }
        }

        public static String SetMobileDataStatus(Boolean enable, int timeout_inMilliSeconds)
        {
            return SetMobileDataStatus("", enable, timeout_inMilliSeconds);
        }
        public static String SetMobileDataStatus(String deviceID, bool enable, int timeout_inMilliSeconds)
        {
            String result = "";
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            deleteAsusApiResult(deviceID);
            argument += "shell am startservice --user 0 -n com.asus.at/.MainService -a SetMobileData -e status ";
            if (enable)
            {
                argument += "on";
            }
            else
            {
                argument += "off";
            }
            RunAdbCommand(argument);
            result = GetAsusApiResult(deviceID, timeout_inMilliSeconds);
            return result;
        }

        public static String SetWiFiState(bool enable, int timeout_inMilliSeconds)
        {
            return SetWiFiState("", enable, timeout_inMilliSeconds);
        }
        public static String SetWiFiState(String deviceID, bool enable, int timeout_inMilliSeconds)
        {
            String result = "";
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            deleteAsusApiResult(deviceID);
            argument += "shell am startservice --user 0 -n com.asus.at/.MainService -a SetWiFiState -e State ";
            if (enable)
            {
                argument += "on";
            }
            else
            {
                argument += "off";
            }
            RunAdbCommand(argument);
            result = GetAsusApiResult(deviceID, timeout_inMilliSeconds);
            return result;
        }
    
        public static void SetSimCardsEnable(String deviceID,bool sim1_enable,bool sim2_enable)
        {
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            deleteAsusApiResult(deviceID);
            argument += "shell am broadcast -a android.intent.action.DUAL_SIM_MODE --ei  mode ";
            int flag = 0;
            flag = flag | (((sim2_enable) ? 1 : 0) << 1) | ((sim1_enable) ? 1 : 0);
            argument += flag.ToString();
            RunAdbCommand(argument);
        }

        internal static bool IsServiceRunning(String deviceID,String serviceName)
        {
            bool isRunning = false;
            String argument = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                argument += "-s " + deviceID + " ";
            }
            deleteAsusApiResult(deviceID);
            argument += "shell am startservice --user 0 -n com.asus.at/.MainService -a IsServiceRunning -e ServiceName " + serviceName;
            RunAdbCommand(argument);
            String result = GetAsusApiResult(deviceID, 5000);
            isRunning = result.ToLower().Equals("true");
            return isRunning;
        }

        internal static void CaptureScreen(String deviceID,String path)
        {
            String idParam="";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            if (deviceID != null && deviceID.Trim().Length > 0)
            {
                idParam = "-s " + deviceID + " ";
            }
            RunAdbCommand(idParam + "shell screencap -p /sdcard/ScreenCap.png", true);
            RunAdbCommand(idParam + "pull /sdcard/ScreenCap.png " + path, true);
            RunAdbCommand(idParam + "shell rm /sdcard/ScreenCap.png");
        }

        internal static bool IsAirplaneModeOn(String deviceID)
        {
            String stdOutput = "",stdError="";
            String cmd = "";
            bool result = false;
            try
            {
                if (deviceID != null && deviceID.Trim().Length > 0)
                {
                    cmd += "-s " + deviceID + " ";
                }
                cmd += "shell settings get global airplane_mode_on";
                RunAdbCommand(cmd, out stdOutput,out stdError, false);
                stdOutput = stdOutput.Trim();
                if (stdOutput.Length > 0 && stdOutput.Contains("1"))
                {
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        internal static void SetAirplaneMode(String deviceID, bool enable)
        {
            String cmd = "";
            if (deviceID != null && deviceID.Trim().Length > 0)
            {
                cmd += "-s " + deviceID + " ";
            }
            cmd+= "shell am broadcast -a android.intent.action.AIRPLANE_MODE --ez state "+ enable.ToString();
            RunAdbCommand(cmd);
        }

        internal static void StartLogcatProcess(String androidID, String logcatFolderOnDevice, bool isRecordRadioLog, bool isRecordEventsLog)
        {
            String timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            RunAdbCommand("shell mkdir -p " + logcatFolderOnDevice,true);
            string cmdlog = "";
            if (androidID != null && androidID.Trim().Length > 0)
            {
                cmdlog += "-s " + androidID + " ";
            }
            cmdlog += "logcat -v time ";
            if (isRecordRadioLog)
            {
                cmdlog += "-b radio ";
                timestamp += "_radio";
            }
            if (isRecordEventsLog)
            {
                cmdlog += "-b events ";
                timestamp += "_events";
            }
            cmdlog += " -f" + "\"" + logcatFolderOnDevice + "/" + timestamp + "_logcat.txt\" -r102400 -n8 &";
            Process psLogcat = new Process();
            psLogcat.StartInfo = new ProcessStartInfo(adbPath);
            psLogcat.StartInfo.Arguments = cmdlog;
            psLogcat.StartInfo.CreateNoWindow = true;
            psLogcat.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psLogcat.StartInfo.RedirectStandardOutput = true;
            psLogcat.StartInfo.UseShellExecute = false;
            psLogcat.Start();
            //psLogcat.WaitForExit();
            psLogcat.Close();
        }

        internal static void KillLogcatProcess(String androidID)
        {
            String cmd = "";
            if (androidID != null && androidID.Trim().Length > 0)
            {
                cmd += "-s " + androidID + " ";
            }
            cmd+= "shell ps logcat";
            Process ps = new Process();
            ps.StartInfo = new ProcessStartInfo(adbPath);
            ps.StartInfo.WorkingDirectory = workingDirectory;
            ps.StartInfo.Arguments = cmd;
            ps.StartInfo.CreateNoWindow = true;
            ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.RedirectStandardError = true;
            ps.StartInfo.UseShellExecute = false;
            ps.Start();
            String temp1 = ps.StandardOutput.ReadToEnd().Trim(new char[] { '\n', '\r' });
            ps.Close();
            String[] list = temp1.Split('\n');
            if (list.Length > 1)
            {
                for (int i = 1; i < list.Length; i++)
                {
                    string[] splited1Str = list[i].Split(' ');
                    int nonSpaceCount = 0;
                    foreach (String splited2Str in splited1Str) // The PID is at the 2nd non-space slot
                    {
                        if (splited2Str.Length > 0)
                        {
                            nonSpaceCount++;
                            if (nonSpaceCount == 2)
                            {
                                cmd = "";
                                if (androidID != null && androidID.Trim().Length > 0)
                                {
                                    cmd += "-s " + androidID + " ";
                                }
                                cmd += "shell kill "+splited2Str;
                                Process psKill = new Process();
                                psKill.StartInfo = new ProcessStartInfo(adbPath);
                                psKill.StartInfo.WorkingDirectory = workingDirectory;
                                psKill.StartInfo.Arguments = cmd;
                                psKill.StartInfo.CreateNoWindow = true;
                                psKill.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                psKill.StartInfo.RedirectStandardOutput = true;
                                psKill.StartInfo.RedirectStandardError = true;
                                psKill.StartInfo.UseShellExecute = false;
                                psKill.Start();
                                psKill.Close();
                                break;
                            }
                        }
                    }
                }
            }
            
        }
    }

    public class AdbDeviceInfomation
    {
        public String ID { get; private set; }
        public String ConnectingStatus { get; private set; }
        public AdbDeviceInfomation(String id, String connectingStatus)
        {
            ID = id;
            ConnectingStatus = connectingStatus;
        }
    }   
}
