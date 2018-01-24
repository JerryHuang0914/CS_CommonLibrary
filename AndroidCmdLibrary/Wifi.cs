using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace jh.csharp.AndroidCmdLibrary
{
    public class Wifi : DeviceComponent
    {
        private Device device;
        private bool enable = false;
        public bool Enable
        {
            get
            {
                if (!isDataRefreshed)
                {
                    RefreshState();
                }
                return enable;
            }
            set
            {
                throw (new NotImplementedException());
                //Set state command here
                RefreshState();
            }
        }
        public bool GetEnable_InsLib()
        {
            return Enable;
        }
        public void SetEnable_InsLib(object boolEnable)
        {
            Enable = Convert.ToBoolean(boolEnable);
        }
        private String ssid = "";
        public String SSID
        {
            get
            {
                if (!isDataRefreshed)
                {
                    RefreshState();
                }
                return ssid;
            }
        }
        public String GetSSID_InsLib()
        {
            return SSID;
        }
        private String ipAddress = "";
        public String IP_Address
        {
            get
            {
                if (!isDataRefreshed)
                {
                    RefreshState();
                }
                return ipAddress;
            }
        }
        public String GetIP_Address_InsLib()
        {
            return IP_Address;
        }
        public String macAddress = "";
        public String MAC_Address
        {
            get
            {
                if (!isDataRefreshed)
                {
                    RefreshState();
                }
                return macAddress;
            }
        }
        public String GetMAC_Address_InsLib()
        {
            return MAC_Address;
        }
        private int signalLevel = 0;
        public int SignalLevel
        {
            get
            {
                if (!isDataRefreshed)
                {
                    RefreshState();
                }
                return signalLevel;
            }
        }
        public int GetSignalLevel_InsLib()
        {
            return SignalLevel;
        }
        public Wifi(Device device)
        {
            this.device = device;
        }
        #region for refresh state control
        public void RefreshState()
        {
            String stdOutput = "", stdError="";
            ipAddress = "";
            ssid = "";
            macAddress = "";
            signalLevel = 0;
            String cmd = "-s " + device.ID + " shell dumpsys wifi";
            ADB_Process.RunAdbCommand(cmd, out stdOutput,out stdError, false);
            foreach (String spilitedStr in stdOutput.Split('\n'))
            {
                String line = spilitedStr.Trim();
                String[] spilitedLine = line.Split(new char[] { '=', ' ' });
                int keywordIndex = 0;
                int valueIndex = 1;
                try
                {
                    switch (spilitedLine[keywordIndex])
                    {
                        #region Enable
                        case "Wi-Fi":
                            valueIndex = 2;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                enable = spilitedLine[valueIndex].ToLower().Equals("enable");
                            }
                            else
                            {
                                enable = false;
                            }
                            break;
                        #endregion Enable
                        #region Stay-awake
                        case "Stay-awake":
                            //    valueIndex = 2;
                            //    if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            //    {

                            //    }
                            //    else
                            //    {

                            //    }
                            break;
                        #endregion Stay-awake
                        #region SSID
                        case "ssid":
                            valueIndex = 1;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                ssid = spilitedLine[valueIndex];
                            }
                            else
                            {
                                ssid = "";
                            }
                            break;
                        #endregion SSID
                        #region IPAddress
                        case "ip_address":
                            valueIndex = 1;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                ipAddress = spilitedLine[valueIndex];
                            }
                            else
                            {
                                ipAddress = "";
                            }
                            break;
                        #endregion IPAddress
                        #region MAC Address
                        case "address":
                            valueIndex = 1;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                macAddress = spilitedLine[valueIndex];
                            }
                            else
                            {
                                macAddress = "";
                            }
                            break;
                        #endregion MAC Address
                        #region Signal Level
                        case "mLastSignalLevel":
                            valueIndex = 1;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                signalLevel = Convert.ToInt32(spilitedLine[valueIndex]);
                            }
                            else
                            {
                                signalLevel = 0;
                            }
                            break;
                        #endregion Signal Level
                    }
                }
                catch
                {

                }
            }
            isDataRefreshed = true;
            resetRefreshedFlag_Start();
        }
        private bool isDataRefreshed = false;
        private Timer tmrRefresh;
        private const int noNeedToRefresh_Interval = 2000;
        public void resetRefreshedFlag_Start()
        {
            if (tmrRefresh == null)
            {
                tmrRefresh = new Timer(new TimerCallback(tmrRefreshedFlagTurnOff), null, 0, noNeedToRefresh_Interval);
            }
        }
        public void tmrRefreshedFlagTurnOff(object obj)
        {
            isDataRefreshed = false;
            tmrRefresh.Dispose();
            tmrRefresh = null;
        }
        #endregion for refresh state control
    }
}
