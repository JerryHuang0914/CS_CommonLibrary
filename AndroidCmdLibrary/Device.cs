using System;
using System.Threading;

namespace jh.csharp.AndroidCmdLibrary
{
    public class Device : DeviceComponent,IDevice
    {
        public String Platform
        {
            get
            {
                return "Android";
            }
        }
        private String id;
        public String ID
        {
            get
            {
                return id;
            }
        }       
        private bool isDeviceAgentReady = false;
        public bool IsConnected
        {
            get
            {
                return isDeviceAgentReady;
            }
        }    
        public Actions Actions
        {
            get
            {
                Object obj = DeviceComponents["Actions"];
                if (obj != null)
                {
                    return (Actions)obj;
                }
                else
                {
                    return null;
                }             
            }
        }
        public SIM_Info SIM1
        {
            get
            {
                Object obj = DeviceComponents["SIM1"];
                if (obj != null)
                {
                    return (SIM_Info)obj;
                }
                else
                {
                    return null;
                }
            }
        }
        public SIM_Info SIM2
        {
            get
            {
                Object obj = DeviceComponents["SIM2"];
                if (obj != null)
                {
                    return (SIM_Info)obj;
                }
                else
                {
                    return null;
                }
            }
        }
        public ProductInfo ProductInfo
        {
            get
            {
                Object obj = DeviceComponents["ProductInfo"];
                if (obj != null)
                {
                    return (ProductInfo)obj;
                }
                else
                {
                    return null;
                }
            }
        }
        public AirplaneMode AirplaneMode
        {
            get
            {
                Object obj = DeviceComponents["AirplaneMode"];
                if (obj != null)
                {
                    return (AirplaneMode)obj;
                }
                else
                {
                    return null;
                }
            }
        }
        public Auxiliary Auxiliary
        {
            get
            {
                Object obj = DeviceComponents["Auxiliary"];
                if (obj != null)
                {
                    return (Auxiliary)obj;
                }
                else
                {
                    return null;
                }
            }
        }
        public Wifi Wifi
        {
            get
            {
                Object obj = DeviceComponents["Wifi"];
                if (obj != null)
                {
                    return (Wifi)obj;
                }
                else
                {
                    return null;
                }
            }
        }
        public Telephony Telephony
        {
            get
            {
                Object obj = DeviceComponents["Telephony"];
                if (obj != null)
                {
                    return (Telephony)obj;
                }
                else
                {
                    return null;
                }
            }
        }

        public Device(String androidID) : this(androidID, 2)
        {

        }

        public Device(String androidID, int simSlotNumber)
        {
            if (androidID.Length > 0)
            {
                id = androidID;
                DeviceComponents.Add("ProductInfo", new ProductInfo(this));
                DeviceComponents.Add("Actions", new Actions(this));
                DeviceComponents.Add("AirplaneMode", new AirplaneMode(this));
                DeviceComponents.Add("Auxiliary", new Auxiliary(this));
                for (int index = 0; index < simSlotNumber; index++)
                {
                    DeviceComponents.Add("SIM" + (index+1).ToString(), new SIM_Info(this,index));                    
                }
                DeviceComponents.Add("Wifi", new Wifi(this));
                DeviceComponents.Add("Telephony", new Telephony(this));
                this.Wifi.RefreshState();
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }

        #region AutoRefreshWwanInfo
        private bool refreshWwanInfo_flag = false;
        private Thread tdRefreshWwanInfo;
        private int refreshWwanInfo_Inberval = 15000;
        public void StartAutoRefreshWwanInfo()
        {
            refreshWwanInfo_flag = true;
            tdRefreshWwanInfo = new Thread(refreshWwanInfo_Runnable);
            tdRefreshWwanInfo.Start();
        }

        public void StartAutoRefreshWwanInfo(int refreshInterval_InMilliseconds)
        {
            refreshWwanInfo_Inberval = refreshInterval_InMilliseconds;
            StartAutoRefreshWwanInfo();
        }

        public void StopAutoRefreshWwanInfo()
        {
            refreshWwanInfo_flag = false;
            if (tdRefreshWwanInfo != null)
            {
                tdRefreshWwanInfo.Join(refreshWwanInfo_Inberval);
                tdRefreshWwanInfo.Abort();
                tdRefreshWwanInfo = null;
            }
        }

        private void refreshWwanInfo_Runnable()
        {
            DateTime startTime = DateTime.Now;
            int sleepTime = 0;
            while (refreshWwanInfo_flag)
            {
                startTime = DateTime.Now;
                if (SIM1 != null)
                {
                    SIM1.RefreshState();
                }
                if (SIM2 != null)
                {
                    SIM2.RefreshState();
                }
                sleepTime = refreshWwanInfo_Inberval - (int)DateTime.Now.Subtract(startTime).TotalMilliseconds;
                if(sleepTime>0)
                {
                    Thread.Sleep(sleepTime);
                }
            }
        }
        #endregion AutoRefreshWwanInfo              
    }
}
