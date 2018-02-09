using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace jh.csharp.CommonLibrary
{
    // State object for receiving data from remote device.
    public class AsynchronousStateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class ConnectionStateChanged_EventArgs : EventArgs
    {
        public String EndPointString { get { return ipEndPoint.ToString(); } }
        public String IP {get{return ipEndPoint.Address.ToString();}}
        public int Port { get { return ipEndPoint.Port; } }
        public bool Connected { get; }
        private readonly IPEndPoint ipEndPoint = null;
        public ConnectionStateChanged_EventArgs(IPEndPoint endPoint, bool connected)
        {
            ipEndPoint = endPoint;
            Connected = connected;
        }
        public ConnectionStateChanged_EventArgs(IPAddress address, int port, bool connected)
        {
            ipEndPoint = new IPEndPoint(address, port);
            Connected = connected;
        }
    }

    public class MessageReceived_EventArgs : EventArgs
    {
        public String EndPointString{get{return ipEndPoint.ToString();}}
        public String IP { get { return ipEndPoint.Address.ToString(); } }
        public int Port { get { return ipEndPoint.Port; } }
        public String ReceivedMessage { get; }
        private readonly IPEndPoint ipEndPoint = null;
        public MessageReceived_EventArgs(IPEndPoint endPoint, String message)
        {
            ipEndPoint = endPoint;
            ReceivedMessage = message;
        }
        public MessageReceived_EventArgs(IPAddress address,int port, String message)
        {
            ipEndPoint = new IPEndPoint(address,port);
            ReceivedMessage = message;
        }
    }
}
