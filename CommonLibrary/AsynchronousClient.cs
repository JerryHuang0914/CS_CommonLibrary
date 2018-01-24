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
        private readonly IPAddress ipAddress;
        public String IP_Address
        {
            get
            {
                return ipAddress.ToString();
            }
        }

        // The port number for the remote device.
        public readonly int Port = 9453;
        private SingleWait_EventWaitHandle receiveDone = new SingleWait_EventWaitHandle(false);
        private String response = String.Empty;        
        private Socket client;

        private bool __is_connected = false;
        private bool is_connected
        {
            get
            {
                return __is_connected;
            }
            set
            {
                if (__is_connected != value)
                {
                    if (ConnectionStateChanged_EventHandler != null)
                    {
                        ConnectionStateChanged_EventHandler.Invoke(this, new ConnectionStateChanged_EventArgs(value));
                    }
                }
                __is_connected = value;
            }
        }
        public bool IsConnected
        {
            get
            {
                return __is_connected;
            }
        }
        public EventHandler<ConnectionStateChanged_EventArgs> ConnectionStateChanged_EventHandler;
        public EventHandler<MessageReceived_EventArgs> MessageReceived_EventHandler;
        private bool listening_flag = false;
        private readonly int listen_interval = 2500;
        public AsynchronousClient(String IP_Addr,int Port=9453)
        {
            ipAddress = IPAddress.Parse(IP_Addr);
            this.Port = Port;
            // Establish the remote endpoint for the socket.
            
            // Create a TCP/IP socket.
            client = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        }

        private void Connect()
        {
            // Connect to a remote device.
            try
            {               
                // Connect to the remote endpoint.
                client.BeginConnect(ipAddress,Port,
                    new AsyncCallback(ConnectCallback), client);
            }
            catch (Exception e)
            {
                is_connected = false;
                Console.WriteLine(e.ToString());
            }
        }

        public void Disconnect()
        {
            if (client != null)
            {
                stop_listen();
                client.Disconnect(true);
            }
        }

        public void Dispose()
        {
            if (client != null)
            {
                stop_listen();
                client.Disconnect(false);
                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {                
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;                
                // Complete the connection.
                client.EndConnect(ar);
                is_connected = client.Connected;
                if (is_connected)
                {
                    start_listen();
                }
            }
            catch (Exception e)
            {
                is_connected = false;
                Console.WriteLine(e.ToString());
            }
        }

        private void start_listen()
        {
            listening_flag = true;
            new Thread(listening_runnable).Start();
        }

        private void stop_listen()
        {
            listening_flag = false;
            receiveDone.Set();
            Thread.CurrentThread.Join(listen_interval);
        }

        private void receive()
        {
            try
            {
                // Create the state object.
                AsynchronousStateObject state_obj = new AsynchronousStateObject();
                state_obj.workSocket = client;
                // Begin receiving the data from the remote device.
                client.BeginReceive(state_obj.buffer, 0, AsynchronousStateObject.BufferSize, 0,
                    new AsyncCallback(receive_callback), state_obj);
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
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, AsynchronousStateObject.BufferSize, 0,
                        new AsyncCallback(receive_callback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                        if (MessageReceived_EventHandler != null)
                        {
                            MessageReceived_EventHandler.Invoke(this, new MessageReceived_EventArgs(response));
                        }
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void listening_runnable()
        {
            while (listening_flag)
            {
                if(client!=null && IsConnected)
                {
                    receive();
                    receiveDone.WaitOne();
                }
                Thread.Sleep(listen_interval);
            }
        }

        public void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,new AsyncCallback(send_callback), client);           
        }

        private void send_callback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket socket = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = socket.EndSend(ar);
                // Signal that all bytes have been sent.
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class ConnectionStateChanged_EventArgs : EventArgs
    {
        public bool Connected { get;}
        public ConnectionStateChanged_EventArgs(bool connected)
        {
            Connected = connected;
        }
    }

    public class MessageReceived_EventArgs : EventArgs
    {
        public String ReceivedMessage { get; }
        public MessageReceived_EventArgs(String message)
        {
            ReceivedMessage = message;
        }
    }
}
