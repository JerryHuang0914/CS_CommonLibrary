using jh.csharp.CommonLibrary;
using System;
using System.Windows.Forms;

namespace CommonLibraryExample
{
    public partial class FormAsyncServer : ToolWindow
    {
        public int Port
        {
            get
            {
                return (int)numPort.Value;
            }
            private set
            {
                if (value > numPort.Maximum)
                {
                    numPort.Value = numPort.Maximum;
                }
                else if (value < numPort.Minimum)
                {
                    numPort.Value = numPort.Minimum;
                }
                numPort.Value = value;
            }
        }
        private AsynchronousServer server;
        public FormAsyncServer()
        {
            InitializeComponent();
            server = new AsynchronousServer();
            server.ConnectedClientChanged_EventHandler += new EventHandler<ConnectionStateChanged_EventArgs>(connectedClientChanged);
            server.MessageReceived_EventHandler += new EventHandler<MessageReceived_EventArgs>(messageReceived);
            cmbIpAddress.DataSource = server.IP_Addresses;
        }

        private void messageReceived(object sender,MessageReceived_EventArgs meag)
        {
            showMessage(meag.EndPointString,"Receive", meag.ReceivedMessage);
        }

        private void connectedClientChanged(object sender, ConnectionStateChanged_EventArgs earg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<object, ConnectionStateChanged_EventArgs>(connectedClientChanged),sender,earg);
            }
            else
            {
                cmbSendTo.DataSource = server.ConnectedClients;
                showMessage(earg.IP + ":" + earg.Port,"Connection", (earg.Connected?"Connected":"Disconnected"));
            }
        }

        private void showMessage(String node,String tag,String msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<String,String, String>(showMessage), node,tag, msg);
            }
            else
            {
                ListViewItem li = new ListViewItem(node);
                li.SubItems.Add(tag);
                li.SubItems.Add(msg);
                lsvMsgs.Items.Add(li);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start(server.IP_Addresses[cmbIpAddress.SelectedIndex], Port);            
            btnStop.Enabled = server.IsRunning;
            btnStart.Enabled = !btnStop.Enabled;
            showMessage("Server","State", "Server is " + (server.IsRunning ? "running.":"stopped."));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            btnStop.Enabled = server.IsRunning;
            btnStart.Enabled = !btnStop.Enabled;
            showMessage("Server","State","Server is " + (server.IsRunning ? "running." : "stopped."));
        }

        private void FormAsyncServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (server != null)
            {
                server.Stop();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtSendMsg.Text.Length > 0)
            {
                if (cmbSendTo.SelectedIndex >= 0)
                {
                    String msg = txtSendMsg.Text;
                    if (server.Send(cmbSendTo.SelectedIndex, msg))
                    {
                        showMessage("Server","Send", msg);
                    }
                    else
                    {
                        showMessage("Server", "Fail", "Fail to send message:" + msg);
                    }
                }
                txtSendMsg.Text = "";
            }
        }

        private void txtSendMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSend_Click(btnSend, e);
            }
        }
    }
}
