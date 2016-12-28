using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace dev.jerry_h.pc_tools.CommonLibrary
{
    public partial class ListView_DisableAutoCheckedOnDoubleClick : ListView
    {
        protected override void WndProc(ref Message m)
        {
            // Filter WM_LBUTTONDBLCLK
            if (m.Msg != 0x203)
            {
                base.WndProc(ref m);
            }
            else
            {
                base.CheckBoxes = false;
                base.WndProc(ref m);
                base.CheckBoxes = true;
            }
        }

        public ListView_DisableAutoCheckedOnDoubleClick()
        {
            InitializeComponent();
        }

        public ListView_DisableAutoCheckedOnDoubleClick(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

    }
}
