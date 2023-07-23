
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchViewerForm));
			this.lblCommandInfo = new System.Windows.Forms.Label();
			this.lstCommands = new System.Windows.Forms.ListBox();
			this.txtCommandInfo = new System.Windows.Forms.RichTextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.btnRetry = new System.Windows.Forms.ToolStripMenuItem();
			this.btnReloadScript = new System.Windows.Forms.ToolStripMenuItem();
			this.btnImportScript = new System.Windows.Forms.ToolStripMenuItem();
			this.pbProgress = new System.Windows.Forms.ProgressBar();
			this.lblProgress = new System.Windows.Forms.Label();
			this.lblTimeElapsed = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblCommandInfo
			// 
			this.lblCommandInfo.AutoSize = true;
			this.lblCommandInfo.Location = new System.Drawing.Point(709, 12);
			this.lblCommandInfo.Name = "lblCommandInfo";
			this.lblCommandInfo.Size = new System.Drawing.Size(102, 17);
			this.lblCommandInfo.TabIndex = 27;
			this.lblCommandInfo.Text = "Command info:";
			// 
			// lstCommands
			// 
			this.lstCommands.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstCommands.Font = new System.Drawing.Font("Consolas", 8F);
			this.lstCommands.FormattingEnabled = true;
			this.lstCommands.ItemHeight = 15;
			this.lstCommands.Location = new System.Drawing.Point(12, 70);
			this.lstCommands.Name = "lstCommands";
			this.lstCommands.Size = new System.Drawing.Size(799, 559);
			this.lstCommands.TabIndex = 20;
			this.lstCommands.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstCommands_DrawItem);
			this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
			// 
			// txtCommandInfo
			// 
			this.txtCommandInfo.Location = new System.Drawing.Point(817, 31);
			this.txtCommandInfo.Name = "txtCommandInfo";
			this.txtCommandInfo.Size = new System.Drawing.Size(505, 598);
			this.txtCommandInfo.TabIndex = 28;
			this.txtCommandInfo.Text = "";
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRetry,
            this.btnReloadScript,
            this.btnImportScript});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1334, 28);
			this.menuStrip1.TabIndex = 37;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// btnRetry
			// 
			this.btnRetry.Enabled = false;
			this.btnRetry.Name = "btnRetry";
			this.btnRetry.Size = new System.Drawing.Size(206, 24);
			this.btnRetry.Text = "Retry from failed command";
			this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
			// 
			// btnReloadScript
			// 
			this.btnReloadScript.Name = "btnReloadScript";
			this.btnReloadScript.Size = new System.Drawing.Size(112, 24);
			this.btnReloadScript.Text = "Reload Script";
			this.btnReloadScript.Click += new System.EventHandler(this.btnReloadScript_Click);
			// 
			// btnImportScript
			// 
			this.btnImportScript.Name = "btnImportScript";
			this.btnImportScript.Size = new System.Drawing.Size(110, 24);
			this.btnImportScript.Text = "Import Script";
			this.btnImportScript.Click += new System.EventHandler(this.btnImportScript_Click);
			// 
			// pbProgress
			// 
			this.pbProgress.Location = new System.Drawing.Point(12, 31);
			this.pbProgress.Name = "pbProgress";
			this.pbProgress.Size = new System.Drawing.Size(799, 33);
			this.pbProgress.TabIndex = 38;
			// 
			// lblProgress
			// 
			this.lblProgress.AutoSize = true;
			this.lblProgress.BackColor = System.Drawing.Color.Transparent;
			this.lblProgress.Location = new System.Drawing.Point(518, 11);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(93, 17);
			this.lblProgress.TabIndex = 39;
			this.lblProgress.Text = "Progress: 0%";
			// 
			// lblTimeElapsed
			// 
			this.lblTimeElapsed.AutoSize = true;
			this.lblTimeElapsed.BackColor = System.Drawing.Color.Transparent;
			this.lblTimeElapsed.Location = new System.Drawing.Point(659, 11);
			this.lblTimeElapsed.Name = "lblTimeElapsed";
			this.lblTimeElapsed.Size = new System.Drawing.Size(116, 17);
			this.lblTimeElapsed.TabIndex = 40;
			this.lblTimeElapsed.Text = "Time elapsed: 0s";
			// 
			// PatchViewerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1334, 641);
			this.Controls.Add(this.lblTimeElapsed);
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.pbProgress);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.txtCommandInfo);
			this.Controls.Add(this.lblCommandInfo);
			this.Controls.Add(this.lstCommands);
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
		private System.Windows.Forms.Label lblTimeElapsed;
	}
}