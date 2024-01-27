
namespace SM64DSe
{
    partial class PatchViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
	        this.lblCommandInfo = new System.Windows.Forms.Label();
	        this.lstCommands = new System.Windows.Forms.ListBox();
	        this.txtCommandInfo = new System.Windows.Forms.RichTextBox();
	        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
	        this.btnRetry = new System.Windows.Forms.ToolStripMenuItem();
	        this.btnReloadScript = new System.Windows.Forms.ToolStripMenuItem();
	        this.btnImportScript = new System.Windows.Forms.ToolStripMenuItem();
	        this.pbProgress = new System.Windows.Forms.ProgressBar();
	        this.lblProgress = new System.Windows.Forms.Label();
	        this.menuStrip1.SuspendLayout();
	        this.SuspendLayout();
	        // 
	        // lblCommandInfo
	        // 
	        this.lblCommandInfo.AutoSize = true;
	        this.lblCommandInfo.Location = new System.Drawing.Point(798, 15);
	        this.lblCommandInfo.Name = "lblCommandInfo";
	        this.lblCommandInfo.Size = new System.Drawing.Size(116, 20);
	        this.lblCommandInfo.TabIndex = 27;
	        this.lblCommandInfo.Text = "Command info:";
	        // 
	        // lstCommands
	        // 
	        this.lstCommands.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
	        this.lstCommands.Font = new System.Drawing.Font("Consolas", 8F);
	        this.lstCommands.FormattingEnabled = true;
	        this.lstCommands.ItemHeight = 15;
	        this.lstCommands.Location = new System.Drawing.Point(14, 88);
	        this.lstCommands.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
	        this.lstCommands.Name = "lstCommands";
	        this.lstCommands.Size = new System.Drawing.Size(898, 698);
	        this.lstCommands.TabIndex = 20;
	        this.lstCommands.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstCommands_DrawItem);
	        this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
	        // 
	        // txtCommandInfo
	        // 
	        this.txtCommandInfo.Location = new System.Drawing.Point(919, 39);
	        this.txtCommandInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
	        this.txtCommandInfo.Name = "txtCommandInfo";
	        this.txtCommandInfo.Size = new System.Drawing.Size(568, 746);
	        this.txtCommandInfo.TabIndex = 28;
	        this.txtCommandInfo.Text = "";
	        // 
	        // menuStrip1
	        // 
	        this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
	        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.btnRetry, this.btnReloadScript, this.btnImportScript });
	        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
	        this.menuStrip1.Name = "menuStrip1";
	        this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
	        this.menuStrip1.Size = new System.Drawing.Size(1501, 33);
	        this.menuStrip1.TabIndex = 37;
	        this.menuStrip1.Text = "menuStrip1";
	        // 
	        // btnRetry
	        // 
	        this.btnRetry.Enabled = false;
	        this.btnRetry.Name = "btnRetry";
	        this.btnRetry.Size = new System.Drawing.Size(242, 29);
	        this.btnRetry.Text = "Retry from failed command";
	        this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
	        // 
	        // btnReloadScript
	        // 
	        this.btnReloadScript.Name = "btnReloadScript";
	        this.btnReloadScript.Size = new System.Drawing.Size(128, 29);
	        this.btnReloadScript.Text = "Reload Script";
	        this.btnReloadScript.Click += new System.EventHandler(this.btnReloadScript_Click);
	        // 
	        // btnImportScript
	        // 
	        this.btnImportScript.Name = "btnImportScript";
	        this.btnImportScript.Size = new System.Drawing.Size(129, 29);
	        this.btnImportScript.Text = "Import Script";
	        this.btnImportScript.Click += new System.EventHandler(this.btnImportScript_Click);
	        // 
	        // pbProgress
	        // 
	        this.pbProgress.Location = new System.Drawing.Point(14, 39);
	        this.pbProgress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
	        this.pbProgress.Name = "pbProgress";
	        this.pbProgress.Size = new System.Drawing.Size(899, 41);
	        this.pbProgress.TabIndex = 38;
	        // 
	        // lblProgress
	        // 
	        this.lblProgress.AutoSize = true;
	        this.lblProgress.BackColor = System.Drawing.Color.Transparent;
	        this.lblProgress.Location = new System.Drawing.Point(583, 14);
	        this.lblProgress.Name = "lblProgress";
	        this.lblProgress.Size = new System.Drawing.Size(103, 20);
	        this.lblProgress.TabIndex = 39;
	        this.lblProgress.Text = "Progress: 0%";
	        // 
	        // PatchViewerForm
	        // 
	        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
	        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
	        this.ClientSize = new System.Drawing.Size(1501, 801);
	        this.Controls.Add(this.lblProgress);
	        this.Controls.Add(this.pbProgress);
	        this.Controls.Add(this.menuStrip1);
	        this.Controls.Add(this.txtCommandInfo);
	        this.Controls.Add(this.lblCommandInfo);
	        this.Controls.Add(this.lstCommands);
	        this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
	        this.Name = "PatchViewerForm";
	        this.Text = "Patch Viewer";
	        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PatchViewerForm_FormClosing);
	        this.menuStrip1.ResumeLayout(false);
	        this.menuStrip1.PerformLayout();
	        this.ResumeLayout(false);
	        this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label lblCommandInfo;
        private System.Windows.Forms.ListBox lstCommands;
        private System.Windows.Forms.RichTextBox txtCommandInfo;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem btnRetry;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lblProgress;
		private System.Windows.Forms.ToolStripMenuItem btnReloadScript;
		private System.Windows.Forms.ToolStripMenuItem btnImportScript;
    }
}