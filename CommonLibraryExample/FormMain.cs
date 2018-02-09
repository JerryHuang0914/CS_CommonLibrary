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

namespace CommonLibraryExample
{
    public partial class FormMain : Form
    {
        int server_count = 1;
        int client_count = 1;
        public FormMain()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void asyncServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAsyncServer frm = new FormAsyncServer();
           
            //frmServer.Shown += new EventHandler(DockContent_ShownEventHandler);
            //frmServer.Disposed += new EventHandler(DockContent_DisposedEventHandler);
            frm.SuspendLayout();
            frm.DockAreas = DockAreas.DockLeft | DockAreas.Document;
            frm.CloseButtonVisible = true;
            frm.ShowHint = DockState.Document;
            frm.Visible = true;
            frm.Show(dockPanel1);
            frm.Text = "Server_" + server_count++;
            frm.TabText = frm.Text;
            frm.ResumeLayout(true);
        }

        private void asyncClientToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormAsyncClient frm = new FormAsyncClient();            
            frm.SuspendLayout();
            frm.DockAreas = DockAreas.DockRight | DockAreas.Float;
            frm.CloseButtonVisible = true;
            frm.ShowHint = DockState.DockRight;
            frm.Visible = true;
            frm.Show(dockPanel1);
            frm.Text = "Client_" + client_count++;
            frm.TabText = frm.Text;
            frm.ResumeLayout(true);
        }
    }
}
