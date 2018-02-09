using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace CommonLibraryExample
{
    public partial class ToolWindow : DockContent
    {
        internal ToolWindow()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        }
    }
}