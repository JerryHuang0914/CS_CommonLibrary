using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace jh.csharp.CommonLibrary
{
    public class AsynchronousServer
    {
        private readonly IPAddress ipAddress;
        private SingleWait_EventWaitHandle acceptDone = new SingleWait_EventWaitHandle(false);
        private SingleWait_EventWaitHandle receiveDone = new SingleWait_EventWaitHandle(false);
        public String IP_Address
        {
            get
            {
                return ipAddress.ToString();
            }
        }

        // The port number for the remote device.
        public readonly int Port = 9453;
        public AsynchronousServer(String IP_Addr, int Port)
        {
            ipAddress = IPAddress.Parse(IP_Addr);
            this.Port = Port;
        }
    }
}
