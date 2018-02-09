using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace jh.csharp.CommonLibrary
{
    public class AsynchronousServer
    {
        private const int LISTENER_SIZE = 64;
        public bool IsRunning
        {
            get;
            private set;
        }
        public IPAddress[] IP_Addresses;
        private Socket server;
        private Dictionary<String, Socket> client_list;
        public List<String> ConnectedClients
        {
            get
            {
                return new List<String>(client_list.Keys);
            }
        }
        public IPAddress IpAddress { get; private set; }
        public EventHandler<MessageReceived_EventArgs> MessageReceived_EventHandler;
        public EventHandler<ConnectionStateChanged_EventArgs> ConnectedClientChanged_EventHandler;
        private Thread tdAccepter = null,tdDataReceiver=null;
        
        // The port number for the remote device.
        public int Port
        {
            get; private set;
        } = 9453;

        public AsynchronousServer()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IP_Addresses = Array.FindAll<IPAddress>(ipHostInfo.AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);
            client_list = new Dictionary<String, Socket>();
        }

        public void Start(IPAddress ipAddress, int port)
        {
            if (tdAccepter != null)
            {
                Stop();
            }
            IsRunning = true;
            this.IpAddress = ipAddress;
            this.Port = port;
            tdAccepter = new Thread(accepting_runnable);
            tdAccepter.Start();
            startMsgReceiver();
        }

        public void Stop(int timeout_of_each_socket = 500)
        {
            IsRunning = false;
            if (tdAccepter!=null)            {

                tdAccepter.Interrupt();
                tdAccepter.Join(1000);
            }
            foreach (KeyValuePair<String, Socket> kv in client_list)
            {
                try
                {
                    kv.Value.Shutdown(SocketShutdown.Both);
                }
                catch { }
                finally { kv.Value.Close(timeout_of_each_socket); }
            }
            if (server != null)
            {
                try
                {
                    server.Shutdown(SocketShutdown.Both);
                }
                catch { }
                finally
                {
                    server.Close(timeout_of_each_socket);
                }
            }
        }

        private void accepting_runnable()
        {
            // Bind the socket to the local endpoint and listen for incoming connections.           
            IPEndPoint localEndPoint = new IPEndPoint(IpAddress, Port);
            // Create a TCP/IP socket.  
            server = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(localEndPoint);
            server.Listen(LISTENER_SIZE);
            try
            {
                while (IsRunning)
                {
                    if (server != null)
                    {
                        //Set the event to nonsignaled state.  
                        //acceptDone.Reset();
                        //Start an asynchronous socket to listen for connections.  
                        Console.WriteLine("Waiting for new connection...");
                        IAsyncResult asyncResult = server.BeginAccept(
                            new AsyncCallback(acceptCallback), server);
                        //Wait until a connection is made before continuing.  
                        asyncResult.AsyncWaitHandle.WaitOne();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (ThreadInterruptedException tie)
            {
                
            }
        }

        private void acceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            try
            {
                Socket client = listener.EndAccept(ar);
                IPEndPoint remoteIpEndPoint = client.RemoteEndPoint as IPEndPoint;
                ConnectionStateChanged_EventArgs csea = new ConnectionStateChanged_EventArgs(remoteIpEndPoint.Address, remoteIpEndPoint.Port, true);
                client_list.Add(csea.EndPointString, client);
                if (ConnectedClientChanged_EventHandler != null)
                {
                    
                    ConnectedClientChanged_EventHandler.Invoke(this, csea);
                }
            }
            catch
            {

            }
        }

        private void startMsgReceiver()
        {
            tdDataReceiver = new Thread(msgReceiver_runnable);
            tdDataReceiver.Start();
        }

        private void msgReceiver_runnable()
        {
            try
            {
                while (IsRunning)
                {
                    foreach (KeyValuePair<String, Socket> kv in client_list)
                    {
                        try
                        {
                            AsynchronousStateObject state = new AsynchronousStateObject();
                            state.workSocket = kv.Value;
                            IAsyncResult asyncResult = kv.Value.BeginReceive(state.buffer, 0, AsynchronousStateObject.BufferSize, 0,
                                new AsyncCallback(msgReceiverCallback), state);
                        }
                        catch (SocketException se)
                        {
                            client_list.Remove(kv.Key);
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadInterruptedException tie)
            {

            }
        }

        private void msgReceiverCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            try
            {
                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                AsynchronousStateObject state = (AsynchronousStateObject)ar.AsyncState;
                Socket socket = state.workSocket;
                IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                // Read data from the client socket. 
                int bytesRead = socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = state.sb.ToString();
                    if (MessageReceived_EventHandler != null)
                    {
                        MessageReceived_EventHandler.Invoke(this, new MessageReceived_EventArgs(remoteIpEndPoint, content));
                    }
                }
            }
            catch(ObjectDisposedException ode)
            {

            }
        }

        public bool Send(String ip_addr, int port, String data, int timeout = 6000)
        {
            // Convert the string data to byte data using ASCII encoding.           
            bool result = false;
            Socket client;
            if (client_list.TryGetValue(ip_addr + ":" + port, out client)) {
                // Begin sending the data to the remote device.
                return send(client, data,timeout);
            }
            return result;
        }

        public bool Send(int connected_client_index, String data, int timeout = 6000)
        {
            bool result = false;
            Socket client = (new List<Socket>(client_list.Values))[connected_client_index];
            if (client != null)
            {
                return send(client, data,timeout);
            }
            return result;
        }

        private bool send(Socket socket,String data,int timeout)
        {
            bool result = false;
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            IAsyncResult ar = socket.BeginSend(byteData, 0, byteData.Length, 0,
                   new AsyncCallback(sendCallback), socket);
            ar.AsyncWaitHandle.WaitOne(timeout);
            result = ar.IsCompleted;
            return result;
        }

        private void sendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }   
}
