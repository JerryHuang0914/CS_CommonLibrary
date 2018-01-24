using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace jh.csharp.AndroidCmdLibrary
{
    public class Telephony : DeviceComponent
    {
        private Device device;
        public Telephony(Device device)
        {
            this.device = device;
        }

        /// <summary>
        ///Call State
        ///0 - CALL_STATE_IDLE(No activity.)
        ///1 - CALL_STATE_RINGING
        ///2 - CALL_STATE_OFFHOOK
        /// </summary>
        public enum CallStates
        {
            //UNKNOW=-1,
            IDLE = 0,
            RINGING = 1,
            OFFHOOK = 2
        };
        public CallStates CallState
        {
            get
            {
                CallStates state = Telephony.CallStates.IDLE;
                if (device != null)
                {
                    if (device.SIM1 != null)
                    {
                        device.SIM1.RefreshState();
                        state |= device.SIM1.CallState;
                    }
                    if (device.SIM2 != null)
                    {
                        device.SIM2.RefreshState();
                        state |= device.SIM2.CallState;
                    }
                }
                return state;
            }
        }
        public bool GetCallConnected_InsLib()
        {
            return CallState.Equals(CallStates.OFFHOOK);
        }

        public void Dial_InsLib(object strDialNumber)
        {
            Dial(Convert.ToString(strDialNumber));
        }
        public void Dial(String dialNumber)
        {
            ADB_Process.Dial(device.ID, dialNumber);
        }

        public void EndCall_InsLib()
        {
            ADB_Process.EndCall(device.ID);
        }

        public bool AnswerCall_InsLib(object intTimeout_inMilliseconds)
        {
            return AnswerCall(Convert.ToInt32(intTimeout_inMilliseconds));
        }
        public bool AnswerCall(int timeout_inMilliSeconds)
        {
            bool isPhoneRing = false;
            bool isTimeout = false;
            String argument = "";
            argument += "-s " + device.ID + " shell input keyevent KEYCODE_HEADSETHOOK";
            DateTime startTime = DateTime.Now;
            #region Wait the phone call
            do
            {
                isTimeout = DateTime.Now.Subtract(startTime).TotalMilliseconds > timeout_inMilliSeconds;
                isPhoneRing = CallState.Equals(CallStates.RINGING);
                Thread.Sleep(500);
            } while (!(isTimeout || isPhoneRing));
            #endregion Wait the phone call
            if (isPhoneRing)
            {
                ADB_Process.RunAdbCommand(argument); //Headset
            }
            else
            {
                EndCall_InsLib();
            }
            return isPhoneRing;
        }
    }
}
