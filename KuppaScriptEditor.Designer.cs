namespace SM64DSe
{
    partial class KuppaScriptEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KuppaScriptEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.saveDKLButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveBINButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveAsDKLButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveAsBINButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveKPLButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.kpsPanel = new System.Windows.Forms.Panel();
            this.kplPanel = new System.Windows.Forms.Panel();
            this.exporter = new System.Windows.Forms.SaveFileDialog();
            this.importer = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveDKLButton,
            this.saveBINButton,
            this.toolStripStatusLabel1,
            this.saveAsDKLButton,
            this.saveAsBINButton,
            this.toolStripStatusLabel2,
            this.saveKPLButton});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // saveDKLButton
            // 
            this.saveDKLButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveDKLButton.DropDownButtonWidth = 0;
            this.saveDKLButton.Image = ((System.Drawing.Image)(resources.GetObject("saveDKLButton.Image")));
            this.saveDKLButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveDKLButton.Name = "saveDKLButton";
            this.saveDKLButton.Size = new System.Drawing.Size(60, 20);
            this.saveDKLButton.Text = "Save DKL";
            this.saveDKLButton.ButtonClick += new System.EventHandler(this.saveDKLButton_ButtonClick);
            // 
            // saveBINButton
            // 
            this.saveBINButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveBINButton.DropDownButtonWidth = 0;
            this.saveBINButton.Image = ((System.Drawing.Image)(resources.GetObject("saveBINButton.Image")));
            this.saveBINButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveBINButton.Name = "saveBINButton";
            this.saveBINButton.Size = new System.Drawing.Size(58, 20);
            this.saveBINButton.Text = "Save BIN";
            this.saveBINButton.ButtonClick += new System.EventHandler(this.saveBINButton_ButtonClick);
            // 
            // saveAsDKLButton
            // 
            this.saveAsDKLButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveAsDKLButton.DropDownButtonWidth = 0;
            this.saveAsDKLButton.Image = ((System.Drawing.Image)(resources.GetObject("saveAsDKLButton.Image")));
            this.saveAsDKLButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAsDKLButton.Name = "saveAsDKLButton";
            this.saveAsDKLButton.Size = new System.Drawing.Size(76, 20);
            this.saveAsDKLButton.Text = "Save As DKL";
            this.saveAsDKLButton.ButtonClick += new System.EventHandler(this.saveAsDKLButton_ButtonClick);
            // 
            // saveAsBINButton
            // 
            this.saveAsBINButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveAsBINButton.DropDownButtonWidth = 0;
            this.saveAsBINButton.Image = ((System.Drawing.Image)(resources.GetObject("saveAsBINButton.Image")));
            this.saveAsBINButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAsBINButton.Name = "saveAsBINButton";
            this.saveAsBINButton.Size = new System.Drawing.Size(74, 20);
            this.saveAsBINButton.Text = "Save As BIN";
            this.saveAsBINButton.ButtonClick += new System.EventHandler(this.saveAsBINButton_ButtonClick);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // saveKPLButton
            // 
            this.saveKPLButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.saveKPLButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveKPLButton.DropDownButtonWidth = 0;
            this.saveKPLButton.Image = ((System.Drawing.Image)(resources.GetObject("saveKPLButton.Image")));
            this.saveKPLButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveKPLButton.Name = "saveKPLButton";
            this.saveKPLButton.Size = new System.Drawing.Size(59, 20);
            this.saveKPLButton.Text = "Save KPL";
            this.saveKPLButton.ButtonClick += new System.EventHandler(this.saveKPLButton_ButtonClick);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.kpsPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.kplPanel);
            this.splitContainer1.Size = new System.Drawing.Size(800, 404);
            this.splitContainer1.SplitterDistance = 387;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // kpsPanel
            // 
            this.kpsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpsPanel.Location = new System.Drawing.Point(0, 0);
            this.kpsPanel.Name = "kpsPanel";
            this.kpsPanel.Size = new System.Drawing.Size(385, 402);
            this.kpsPanel.TabIndex = 0;
            // 
            // kplPanel
            // 
            this.kplPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kplPanel.Location = new System.Drawing.Point(0, 0);
            this.kplPanel.Name = "kplPanel";
            this.kplPanel.Size = new System.Drawing.Size(408, 402);
            this.kplPanel.TabIndex = 1;
            // 
            // exporter
            // 
            this.exporter.Filter = "Kuppa Script|*.kps|Dynamic Kuppa Library|*.dkl|Kuppa Binary|*.bin";
            this.exporter.RestoreDirectory = true;
            // 
            // importer
            // 
            this.importer.Filter = "Kuppa Script|*.kps|Dynamic Kuppa Library|*.dkl|Kuppa Binary|*.bin";
            this.importer.RestoreDirectory = true;
            // 
            // KuppaScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "KuppaScriptEditor";
            this.Text = "Kuppa Script Editor";
            this.Load += new System.EventHandler(this.KuppaScriptEditor_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSplitButton saveDKLButton;
        private System.Windows.Forms.ToolStripSplitButton saveBINButton;
        private System.Windows.Forms.ToolStripSplitButton saveAsDKLButton;
        private System.Windows.Forms.ToolStripSplitButton saveAsBINButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSplitButton saveKPLButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel kpsPanel;
        private System.Windows.Forms.Panel kplPanel;
        private System.Windows.Forms.SaveFileDialog exporter;
        private System.Windows.Forms.OpenFileDialog importer;
    }
}