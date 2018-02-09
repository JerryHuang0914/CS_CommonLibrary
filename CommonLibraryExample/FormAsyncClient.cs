using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using jh.csharp.CommonLibrary;

namespace CommonLibraryExample
{
    public partial class FormAsyncClient : ToolWindow
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
        private AsynchronousClient client;
        public FormAsyncClient()
        {
            InitializeComponent();
            client = new AsynchronousClient();
            client.ConnectionStateChanged_EventHandler += new EventHandler<ConnectionStateChanged_EventArgs>(connectionStateChanged);
            client.MessageReceived_EventHandler += new EventHandler<MessageReceived_EventArgs>(messageReceived);
        }

        private void messageReceived(object sender, MessageReceived_EventArgs meag)
        {
            showMessage("Receive", meag.ReceivedMessage);
        }


        private void connectionStateChanged(object sender,ConnectionStateChanged_EventArgs eags)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, ConnectionStateChanged_EventArgs>(connectionStateChanged), sender, eags);
            }
            else
            {
                btnConnect.Enabled = !eags.Connected;
                btnDisconnect.Enabled = eags.Connected;
                showMessage("Connection", eags.Connected ? "Connected" : "Disconnected");
                if (client.LocalIpEndPoint != null)
                {
                    lblMyIpAddress.Text = client.LocalIpEndPoint.ToString();
                }
            }
        }

        private void showMessage(String header, String msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<String, String>(showMessage), header, msg);
            }
            else
            {
                ListViewItem li = new ListViewItem(header);
                li.SubItems.Add(msg);
                lsvMsgs.Items.Add(li);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            client.Connect(txtIpAddress.Text, Port);
        }

        private void btnDisconnect_Click(object sender,EventArgs e)
        {
            client.Disconnect();
        }

        private void FormAsyncClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
            {
                client.Dispose();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtSendMsg.Text.Length > 0 && client.IsConnected)
            {
                String msg = txtSendMsg.Text;
                if (client.Send(txtSendMsg.Text))
                {
                    showMessage("Send", msg);
                }
                else
                {
                    showMessage("Fail", "Fail to send message:" + msg);
                }
                txtSendMsg.Text = "";
            }
        }

        private void txtSendMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                btnSend_Click(btnSend, e);
            }
        }
    }
}
