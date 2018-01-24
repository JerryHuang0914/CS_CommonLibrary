using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jh.csharp.AndroidCmdLibrary
{
    public class WwanInfo
    {
        private IDevice device;
        private String androidID = "";
        private int simSlotNumber = 0;
        public enum MobileModes
        {
            None = 0,
            _2G = 1,
            _3G = 2,
            _4G = 4
        };
        public MobileModes MobileMoode = MobileModes.None;
        public bool IsMobileDataConnected{
            get
            {
                return DataConnectionState.Equals(DataConnectionStates.CONNECTED);
            }
        }
        public int SignalStrength { get; private set; } = -999;
        public String APN_Name{ get; private set;} = "";

        /// <summary>
        ///Call State
        ///0 - CALL_STATE_IDLE(No activity.)
        ///1 - CALL_STATE_RINGING
        ///2 - CALL_STATE_OFFHOOK
        /// </summary>
        public enum CallStates
        {
            IDLE = 0,
            RINGING = 1,
            OFFHOOK = 2
        };
        public CallStates CallState {get; private set;}= CallStates.IDLE;
        public String IncommingCallNumber { get; private set; } = "";          
        
        /// <summary>
        ///Radio service State
        ///0 - STATE_IN_SERVICE(Normal operation condition, the phone is registered with an operator either in home network or in roaming. )
        ///1 - STATE_OUT_OF_SERVICE (Phone is not registered with any operator, the phone can be currently searching a new operator to register to, or not searching to registration at all, or registration is denied, or radio signal is not available. )
        ///2 - STATE_EMERGENCY_ONLY (The phone is registered and locked. Only emergency numbers are allowed. )
        ///3 - STATE_POWER_OFF(Radio of telephony is explicitly powered off.)
        /// </summary>
        public enum ServiceStates
        {
            IN_SERVICE=0,
            OUT_OF_SERVICE=1,
            EMERGENCY_ONLY=2,
            POWER_OFF=3
        }
        public ServiceStates ServiceState { get; private set; } = ServiceStates.OUT_OF_SERVICE;


        /// <summary>
        ///0 - Radio Data Call Activity: DATA_ACTIVITY_NONE(No traffic.)
        ///1 - DATA_ACTIVITY_IN(Currently receiving IP PPP traffic.)
        ///2 - DATA_ACTIVITY_OUT(Currently sending IP PPP traffic.)
        ///3 - DATA_ACTIVITY_INOUT(Currently both sending and receiving IP 
        /// </summary>
        public enum DataActivities
        {
            NONE=0,
            IN=1,
            OUT=2,
            IN_OUT=3
        }
        public DataActivities DataActivity { get; private set; } = DataActivities.NONE;


        /// <summary>
        ///Radio Data Connection State
        ///0 - DATA_DISCONNECTED (Disconnected. IP traffic not available. )
        ///1 - DATA_CONNECTING(Currently setting up a data connection.)
        ///2 - DATA_CONNECTED (Connected. IP traffic should be available.)
        ///3 - DATA_SUSPENDED (Suspended. The connection is up, but IP traffic is temporarily unavailable. For example, in a 2G network, data activity may be suspended when a voice call arrives.)
        /// </summary>
        public enum DataConnectionStates
        {
            UNKNOW=-1,
            DISCONNECTED =0,
            CONNECTING=1,
            CONNECTED=2,
            SUSPENDED=3
        }
        public DataConnectionStates DataConnectionState {get; private set;} = DataConnectionStates.DISCONNECTED;


        public WwanInfo(IDevice device,String androidID, int simSlotNumber=0)
        {
            this.androidID = androidID;
            this.simSlotNumber = simSlotNumber;
            this.device = device;
        }
        
        public void RefreshState()
        {
            String stdOutput = "", stdError = "";
            String cmd = "-s "+androidID+" shell dumpsys telephony.registry";
            if(simSlotNumber==1) //SIM 2
            {
                cmd += "2";
            }
            ADB_Process.RunAdbCommand(cmd, out stdOutput, out stdError, false);
            foreach (String spilitedStr in stdOutput.Split('\n'))
            {
                String line = spilitedStr.Trim();
                String[] spilitedLine = line.Split(new char[]{'=',' '});
                int keywordIndex = 0;
                int valueIndex = 1;
                try
                {
                    switch (spilitedLine[keywordIndex])
                    {
                        #region CallState
                        case "mCallState":
                            valueIndex = 1;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                CallState = (CallStates)Enum.ToObject(typeof(CallStates), Convert.ToInt32(spilitedLine[valueIndex]));
                            }
                            else
                            {
                                CallState = CallStates.IDLE;
                            }
                            break;
                        #endregion CallState
                        #region Incoming Number
                        case "mCallIncomingNumber":
                            valueIndex = 1;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                IncommingCallNumber = spilitedLine[valueIndex];
                            }
                            else
                            {
                                IncommingCallNumber = "";
                            }
                            break;
                        #endregion Incoming Number
                        #region Service State & Mibile Mode
                        case "mServiceState":
                            if (spilitedLine.Length > 1)
                            {
                                if (spilitedLine[1].ToUpper().StartsWith("SIM"))
                                {
                                    valueIndex = 2;
                                }
                                else
                                {
                                    valueIndex = 1;
                                }
                                if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                                {
                                    ServiceState = (ServiceStates)Enum.ToObject(typeof(ServiceStates), Convert.ToInt32(spilitedLine[valueIndex]));
                                }
                                if (line.Contains("LTE")||
                                    line.Contains("WIMAX"))
                                {
                                    MobileMoode |= MobileModes._4G;
                                }
                                if (line.Contains("CDMA")||
                                    line.Contains("UMTS")||
                                    line.Contains("EvDO")||
                                    line.Contains("HSDPA")||
                                    line.Contains("HSUPA")||
                                    line.Contains("HSPA"))
                                                   
                                {
                                    MobileMoode |= MobileModes._3G;
                                }
                                if(line.Contains("GPRS")||
                                   line.Contains("EDGE")||
                                   line.Contains("GSM"))
                                {
                                    MobileMoode |= MobileModes._2G;
                                }
                            }
                            break;
                        #endregion Service State & Mibile Mode                        
                        #region Signal Strength
                        case "mSignalStrength":
                            if (spilitedLine.Length > 1)
                            {
                                int strength = -999;
                                try
                                {
                                    if (spilitedLine[1].ToUpper().Contains("SIM"))  //MTK Dual SIM Solution
                                    {
                                        valueIndex = spilitedLine.Length - 3;
                                        strength = Convert.ToInt32(spilitedLine[valueIndex]);
                                        strength = (int)strength / 4;
                                    }
                                    else
                                    {
                                        valueIndex = spilitedLine.Length - 6;
                                        strength = Convert.ToInt32(spilitedLine[valueIndex]);
                                    }
                                }
                                catch
                                {
                                    strength = -999;
                                }
                                SignalStrength = strength;
                            }
                            break;
                        #endregion Signal Strength
                        #region DataActivity
                        case "mDataActivity":
                            valueIndex = 1;
                            if(spilitedLine.Length>valueIndex && spilitedLine[valueIndex] != null)
                            {
                                DataActivity = (DataActivities)Enum.ToObject(typeof(DataActivities), Convert.ToInt32(spilitedLine[valueIndex]));
                            }
                            break;
                        #endregion DataActivity
                        #region DataConnectionState
                        case "mDataConnectionState":
                            valueIndex = 1;
                            if (spilitedLine.Length > valueIndex && spilitedLine[valueIndex] != null)
                            {
                                DataConnectionState = (DataConnectionStates)Enum.ToObject(typeof(DataConnectionStates),Convert.ToInt32(spilitedLine[valueIndex]));
                            }
                            else
                            {
                                DataConnectionState = DataConnectionStates.UNKNOW;
                            }
                            break;
                        #endregion DataConnectionState                            
                        #region APN
                        case "mDataConnectionApn":
                            valueIndex = 1;
                            if(spilitedLine.Length>valueIndex && spilitedLine[valueIndex] != null){
                                APN_Name = spilitedLine[valueIndex];
                            }
                            else
                            {
                                APN_Name = "";
                            }
                            break;
                        #endregion APN
                        //#region
                        //case "":
                        //    valueIndex = 1;
                        //    if(spilitedLine.Length>valueIndex && spilitedLine[valueIndex] != null){

                        //    }
                        //    break;
                        //#endregion 
                    }
                }
                catch
                {

                }
            }
        }
    }
}
