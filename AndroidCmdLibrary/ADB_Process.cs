using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
            String stdOutput = "", stdError = "";
            String arg = "start-server";
            RunAdbCommand(arg, out stdOutput, out stdError, false);
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
		
		public static int RunAaptCommand(String argument, out String standardOutput, out String standardError, bool waitForExit = false)
        {
            int exitCode = -1;
            standardOutput = "";
            standardError = "";
            Process ps = new Process();
            try
            {
                ps.StartInfo = new ProcessStartInfo(aaptPath);
                ps.StartInfo.WorkingDirectory = workingDirectory;
                ps.StartInfo.Arguments = argument;
                ps.StartInfo.CreateNoWindow = true;
                ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                ps.StartInfo.RedirectStandardOutput = true;
                ps.StartInfo.RedirectStandardError = true;
                ps.StartInfo.UseShellExecute = false;
                ps.Start();
                if (standardOutput != null)
                {
                    standardOutput = ps.StandardOutput.ReadToEnd().Trim(new char[] { '\n', '\r' });
                }
                if (standardError != null)
                {
                    standardError = ps.StandardError.ReadToEnd().Trim(new char[] { '\n', '\r' });
                }
                if (waitForExit)
                {
                    ps.WaitForExit();
                }
                exitCode = ps.ExitCode;
            }
            catch
            {

            }
            finally
            {

                if (ps != null)
                {

                    ps.Close();
                }
            }

            return exitCode;
        }
		
        public static List<String> GetPackagesList(String deviceID = "", String keyword = "")
        {        
            List<String> packages = new List<string>();
            String stdOutput = "", stdError = "";
            String arg = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                arg += "-s " + deviceID + " ";
            }
            arg += "shell \"pm list packages\"";
            if (keyword != null && keyword.Length > 0)
            {
                arg = arg.TrimEnd('\"') + " | grep " + keyword + "\"";
            }
            RunAdbCommand(arg, out stdOutput, out stdError,false);
            if (stdOutput != null && stdOutput.Length > 0)
            {
                packages.AddRange(stdOutput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            }           
            return packages;
        }

        public static String GetPackageVersionNumber(String packageName, String deviceID = "")
        {
            String stdOutput="", stdError="";
            String arg = "";
            if (deviceID != null && deviceID.Length > 0)
            {
                arg += "-s " + deviceID + " ";
            }
            arg += "shell \"dumpsys package " + packageName + " | grep versionName=\"";
            RunAdbCommand(arg, out stdOutput, out stdError, true);
            if (stdOutput != null && stdOutput.Length > 0)
            {
                return stdOutput.Replace("versionName=", "");
            }
            else
            {
                return "Not Installed";
            }
        }

        public static String GetApkFileVersionName(String apkPath)
        {
            String version = "Unknow";
            String ver_pattern = @"versionName\s*=\s*('|)(?<version>(\d+\.)?(\d+\.)?(\*|\d+))('|)";
            Regex reg = new Regex(ver_pattern);
            String stdOutput = "", stdError = "";
            String arg = "dump badging \""+apkPath+"\"";
            RunAaptCommand(arg, out stdOutput, out stdError, true);
            if(stdOutput!=null && stdOutput.Length > 0)
            {
                Match match = reg.Match(stdOutput);
                if (match.Success)
                {
                    version = match.Groups["version"].Value;
                }
            }
            return version;
        }


        public static int Install_Apk(String apkPath,String deviceID = "")
        {
            String stdOutput = "", stdError = "";

            String arg = "install -r \""+ apkPath+"\"";
            if (deviceID != null && deviceID.Length > 0)
            {

                arg = "-s " + deviceID + " "+arg;
            }
            int errorcode = RunAdbCommand(arg, out stdOutput, out stdError, true);
            if(stdError!=null && stdError.Length > 0)
            {
                //System.Windows.Forms.MessageBox.Show(stdError);
            }

            return errorcode;
        }

		public static List<AdbDeviceInfomation> GetDeivcesList()
        {
            List<AdbDeviceInfomation> lstDeiviceList = new List<AdbDeviceInfomation>();
            String stdOutput = "", stdError = "";
            String arg = "devices";
            RunAdbCommand(arg, out stdOutput, out stdError, true);
            if (stdOutput != null && stdOutput.Length > 0)
            {
                String[] strReturnInfos = stdOutput.Split('\n');
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
            String stdOutput = "", stdError = "";
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
            RunAaptCommand(cmdlog, out stdOutput, out stdError, false);
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
