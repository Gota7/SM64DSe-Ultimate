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
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.changeOutputKPSButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveKPLButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.kpsPanel = new System.Windows.Forms.Panel();
            this.kplPanel = new System.Windows.Forms.Panel();
            this.exporter = new System.Windows.Forms.SaveFileDialog();
            this.importer = new System.Windows.Forms.OpenFileDialog();
            this.kpsSelector = new System.Windows.Forms.SaveFileDialog();
            this.dklMode = new System.Windows.Forms.RadioButton();
            this.binMode = new System.Windows.Forms.RadioButton();
            this.saveKPSBox = new System.Windows.Forms.CheckBox();
            this.openROMBox = new System.Windows.Forms.CheckBox();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncommentCurrentLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1115, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.importToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeOutputKPSButton,
            this.toolStripStatusLabel5,
            this.saveKPLButton,
            this.toolStripStatusLabel4});
            this.statusStrip1.Location = new System.Drawing.Point(0, 593);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1115, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // changeOutputKPSButton
            // 
            this.changeOutputKPSButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.changeOutputKPSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.changeOutputKPSButton.DropDownButtonWidth = 0;
            this.changeOutputKPSButton.Image = ((System.Drawing.Image)(resources.GetObject("changeOutputKPSButton.Image")));
            this.changeOutputKPSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.changeOutputKPSButton.Name = "changeOutputKPSButton";
            this.changeOutputKPSButton.Size = new System.Drawing.Size(117, 20);
            this.changeOutputKPSButton.Text = "Change Output KPS";
            this.changeOutputKPSButton.ButtonClick += new System.EventHandler(this.saveKPLButton_ButtonClick);
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel5.Text = "|";
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
            this.saveKPLButton.ButtonClick += new System.EventHandler(this.saveKPLButton_ButtonClick_1);
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel4.Text = "|";
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
            this.splitContainer1.Size = new System.Drawing.Size(1115, 569);
            this.splitContainer1.SplitterDistance = 539;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // kpsPanel
            // 
            this.kpsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpsPanel.Location = new System.Drawing.Point(0, 0);
            this.kpsPanel.Name = "kpsPanel";
            this.kpsPanel.Size = new System.Drawing.Size(537, 567);
            this.kpsPanel.TabIndex = 0;
            // 
            // kplPanel
            // 
            this.kplPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kplPanel.Location = new System.Drawing.Point(0, 0);
            this.kplPanel.Name = "kplPanel";
            this.kplPanel.Size = new System.Drawing.Size(571, 567);
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
            // kpsSelector
            // 
            this.kpsSelector.Filter = "Kuppa Script|*.kps";
            this.kpsSelector.RestoreDirectory = true;
            // 
            // dklMode
            // 
            this.dklMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dklMode.Checked = true;
            this.dklMode.Location = new System.Drawing.Point(199, 594);
            this.dklMode.Name = "dklMode";
            this.dklMode.Size = new System.Drawing.Size(46, 21);
            this.dklMode.TabIndex = 3;
            this.dklMode.TabStop = true;
            this.dklMode.Text = "DKL";
            this.dklMode.UseVisualStyleBackColor = true;
            // 
            // binMode
            // 
            this.binMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.binMode.Location = new System.Drawing.Point(251, 596);
            this.binMode.Name = "binMode";
            this.binMode.Size = new System.Drawing.Size(43, 17);
            this.binMode.TabIndex = 4;
            this.binMode.Text = "BIN";
            this.binMode.UseVisualStyleBackColor = true;
            // 
            // saveKPSBox
            // 
            this.saveKPSBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveKPSBox.Checked = true;
            this.saveKPSBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveKPSBox.Location = new System.Drawing.Point(300, 596);
            this.saveKPSBox.Name = "saveKPSBox";
            this.saveKPSBox.Size = new System.Drawing.Size(75, 17);
            this.saveKPSBox.TabIndex = 5;
            this.saveKPSBox.Text = "Save KPS";
            this.saveKPSBox.UseVisualStyleBackColor = true;
            // 
            // openROMBox
            // 
            this.openROMBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openROMBox.Location = new System.Drawing.Point(381, 596);
            this.openROMBox.Name = "openROMBox";
            this.openROMBox.Size = new System.Drawing.Size(130, 17);
            this.openROMBox.TabIndex = 6;
            this.openROMBox.Text = "Open ROM On Save";
            this.openROMBox.UseVisualStyleBackColor = true;
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uncommentCurrentLineToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // uncommentCurrentLineToolStripMenuItem
            // 
            this.uncommentCurrentLineToolStripMenuItem.Name = "uncommentCurrentLineToolStripMenuItem";
            this.uncommentCurrentLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.uncommentCurrentLineToolStripMenuItem.Size = new System.Drawing.Size(299, 22);
            this.uncommentCurrentLineToolStripMenuItem.Text = "(Un)comment Current Line";
            this.uncommentCurrentLineToolStripMenuItem.Click += new System.EventHandler(this.uncommentCurrentLineToolStripMenuItem_Click);
            // 
            // KuppaScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 615);
            this.Controls.Add(this.openROMBox);
            this.Controls.Add(this.saveKPSBox);
            this.Controls.Add(this.binMode);
            this.Controls.Add(this.dklMode);
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
        private System.Windows.Forms.ToolStripSplitButton changeOutputKPSButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel kpsPanel;
        private System.Windows.Forms.Panel kplPanel;
        private System.Windows.Forms.SaveFileDialog exporter;
        private System.Windows.Forms.OpenFileDialog importer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripSplitButton saveKPLButton;
        private System.Windows.Forms.SaveFileDialog kpsSelector;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.RadioButton dklMode;
        private System.Windows.Forms.RadioButton binMode;
        private System.Windows.Forms.CheckBox saveKPSBox;
        private System.Windows.Forms.CheckBox openROMBox;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncommentCurrentLineToolStripMenuItem;
    }
}