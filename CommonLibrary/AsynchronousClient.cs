using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace jh.csharp.CommonLibrary
{
    // Asynchronous Client Socket Example
    // http://msdn.microsoft.com/en-us/library/bew39x2a.aspx

    public class AsynchronousClient:IDisposable
    {
        public IPEndPoint LocalIpEndPoint { get; private set; } = null;
        public IPAddress RemoteIpAddress
        {
            get;
            private set;
        }
        // The port number for the remote device.
        public int RemotePort
        {
            get; private set;
        } = 9453;
        //private SingleWait_EventWaitHandle receiveDone = new SingleWait_EventWaitHandle(false);
        private String response = String.Empty;        
        private Socket client;

        private bool __is_connected = false;
        public bool IsConnected
        {
            get
            {
                return __is_connected;
            }
            private set
            {
                if (__is_connected != value)
                {
                    __is_connected = value;
                    if (ConnectionStateChanged_EventHandler != null)
                    {
                        ConnectionStateChanged_EventHandler.Invoke(this, new ConnectionStateChanged_EventArgs(RemoteIpAddress,RemotePort,__is_connected));
                    }
                }             
            }
        }
        public EventHandler<ConnectionStateChanged_EventArgs> ConnectionStateChanged_EventHandler;
        public EventHandler<MessageReceived_EventArgs> MessageReceived_EventHandler;
        private bool listening_flag = false;
        private readonly int listen_interval = 2500;
        private Thread tdDataReceiver = null;
        public AsynchronousClient()
        {
            // Create a TCP/IP socket.
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(String remoteIp, int remotePort = 9453)
        {
            if (client == null)
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            RemoteIpAddress = IPAddress.Parse(remoteIp);
            RemotePort = remotePort;
            // Connect to a remote device.
            try
            {               
                // Connect to the remote endpoint.
                client.BeginConnect(RemoteIpAddress, RemotePort,
                    new AsyncCallback(connectCallback), client);
            }
            catch (Exception e)
            {
                IsConnected = false;
                Console.WriteLine(e.ToString());
            }
        }

        public void Disconnect(int timeout=5000)
        {
            if (client != null)
            {
                stop_listen();               
                try
                {
                    if (client.Connected)
                    {
                        client.Disconnect(false);                        
                    }
                    client.Shutdown(SocketShutdown.Both);
                }
                catch {
                    
                }
                finally
                {
                    
                    client.Close(timeout);
                    client = null;
                }
            }
            IsConnected = false;
        }

        public void Dispose()
        {
            Disconnect();
        }

        private void connectCallback(IAsyncResult ar)
        {
            try
            {                
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;                
                // Complete the connection.
                client.EndConnect(ar);
                LocalIpEndPoint = (IPEndPoint)client.LocalEndPoint;
                IsConnected = client.Connected;
                if (IsConnected)
                {
                    start_listen();
                }
                else
                {
                    LocalIpEndPoint = null;
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
                Console.WriteLine(e.ToString());
            }
        }

        private void start_listen()
        {
            if (tdDataReceiver != null)
            {
                stop_listen();
            }
            listening_flag = true;
            tdDataReceiver = new Thread(listening_runnable);
            tdDataReceiver.Start();
        }

        private void stop_listen()
        {
            listening_flag = false;
            if (tdDataReceiver != null)
            {
                tdDataReceiver.Interrupt();
                tdDataReceiver.Join(listen_interval);
            }
            tdDataReceiver = null;
        }

        private void receive()
        {
            try
            {
                // Create the state object.
                AsynchronousStateObject state_obj = new AsynchronousStateObject();
                state_obj.workSocket = client;
                // Begin receiving the data from the remote device.
                IAsyncResult ar =client.BeginReceive(state_obj.buffer, 0, AsynchronousStateObject.BufferSize, 0,
                    new AsyncCallback(receive_callback), state_obj);
                ar.AsyncWaitHandle.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void receive_callback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                AsynchronousStateObject state = (AsynchronousStateObject)ar.AsyncState;
                Socket socket = state.workSocket;
                // Read data from the remote device.
                int bytesRead = socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    response = state.sb.ToString();
                    if (MessageReceived_EventHandler != null)
                    {
                        MessageReceived_EventHandler.Invoke(this, new MessageReceived_EventArgs(RemoteIpAddress, RemotePort, response));
                    }
                }
            }
            catch(ObjectDisposedException ode)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void listening_runnable()
        {
            try { 
                while (listening_flag)
                {
                    if (client != null && IsConnected)
                    {
                        receive();
                    }
                    Thread.Sleep(listen_interval);
                }
            }
            catch(ThreadInterruptedException tie)
            {

            }
        }

        public bool Send(String data,int timeout=60000)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.
            IAsyncResult ar = client.BeginSend(byteData, 0, byteData.Length, 0,new AsyncCallback(send_callback), client);
            ar.AsyncWaitHandle.WaitOne(timeout);
            return ar.IsCompleted;
        }

        private void send_callback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket socket = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = socket.EndSend(ar);   
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
