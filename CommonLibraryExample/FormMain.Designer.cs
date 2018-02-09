namespace CommonLibraryExample
{
    partial class FormMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.asyncClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asyncClientToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.asyncServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.Location = new System.Drawing.Point(0, 24);
            this.dockPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(1241, 488);
            this.dockPanel1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asyncClientToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1241, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // asyncClientToolStripMenuItem
            // 
            this.asyncClientToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asyncClientToolStripMenuItem1,
            this.asyncServerToolStripMenuItem});
            this.asyncClientToolStripMenuItem.Name = "asyncClientToolStripMenuItem";
            this.asyncClientToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.asyncClientToolStripMenuItem.Text = "Connection";
            // 
            // asyncClientToolStripMenuItem1
            // 
            this.asyncClientToolStripMenuItem1.Name = "asyncClientToolStripMenuItem1";
            this.asyncClientToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
            this.asyncClientToolStripMenuItem1.Text = "AsyncClient";
            this.asyncClientToolStripMenuItem1.Click += new System.EventHandler(this.asyncClientToolStripMenuItem1_Click);
            // 
            // asyncServerToolStripMenuItem
            // 
            this.asyncServerToolStripMenuItem.Name = "asyncServerToolStripMenuItem";
            this.asyncServerToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.asyncServerToolStripMenuItem.Text = "AsyncServer";
            this.asyncServerToolStripMenuItem.Click += new System.EventHandler(this.asyncServerToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 512);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem asyncClientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asyncClientToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem asyncServerToolStripMenuItem;
    }
}

