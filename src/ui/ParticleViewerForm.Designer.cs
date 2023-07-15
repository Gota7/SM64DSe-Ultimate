namespace SM64DSe
{
    partial class ParticleViewerForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadSPAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceSPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllSPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParticleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFreeze = new System.Windows.Forms.Button();
            this.btnDisplay = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lbxSysDef = new System.Windows.Forms.ListBox();
            this.pgSysDefProps = new System.Windows.Forms.PropertyGrid();
            this.glModelView = new SM64DSe.FormControls.ModelGLControlWithMarioSizeReference();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSPAToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.replaceSPDToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.addParticleToolStripMenuItem,
            this.removeSelectedToolStripMenuItem,
            this.editTexturesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1846, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadSPAToolStripMenuItem
            // 
            this.loadSPAToolStripMenuItem.Name = "loadSPAToolStripMenuItem";
            this.loadSPAToolStripMenuItem.Size = new System.Drawing.Size(85, 26);
            this.loadSPAToolStripMenuItem.Text = "Load SPA";
            this.loadSPAToolStripMenuItem.Click += new System.EventHandler(this.loadSPAToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(54, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // replaceSPDToolStripMenuItem
            // 
            this.replaceSPDToolStripMenuItem.Name = "replaceSPDToolStripMenuItem";
            this.replaceSPDToolStripMenuItem.Size = new System.Drawing.Size(107, 26);
            this.replaceSPDToolStripMenuItem.Text = "Replace SPD";
            this.replaceSPDToolStripMenuItem.Click += new System.EventHandler(this.replaceSPDToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSPDToolStripMenuItem,
            this.exportAllSPDToolStripMenuItem,
            this.exportCppToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(66, 26);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // exportSPDToolStripMenuItem
            // 
            this.exportSPDToolStripMenuItem.Name = "exportSPDToolStripMenuItem";
            this.exportSPDToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.exportSPDToolStripMenuItem.Text = "Export SPD";
            this.exportSPDToolStripMenuItem.Click += new System.EventHandler(this.exportSPDToolStripMenuItem_Click);
            // 
            // exportAllSPDToolStripMenuItem
            // 
            this.exportAllSPDToolStripMenuItem.Name = "exportAllSPDToolStripMenuItem";
            this.exportAllSPDToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.exportAllSPDToolStripMenuItem.Text = "Export All (SPD)";
            this.exportAllSPDToolStripMenuItem.Click += new System.EventHandler(this.exportAllSPDToolStripMenuItem_Click);
            // 
            // exportCppToolStripMenuItem
            // 
            this.exportCppToolStripMenuItem.Name = "exportCppToolStripMenuItem";
            this.exportCppToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.exportCppToolStripMenuItem.Text = "Export C++";
            this.exportCppToolStripMenuItem.Click += new System.EventHandler(this.exportCppToolStripMenuItem_Click);
            // 
            // addParticleToolStripMenuItem
            // 
            this.addParticleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedToolStripMenuItem,
            this.importSPDToolStripMenuItem});
            this.addParticleToolStripMenuItem.Name = "addParticleToolStripMenuItem";
            this.addParticleToolStripMenuItem.Size = new System.Drawing.Size(103, 26);
            this.addParticleToolStripMenuItem.Text = "Add Particle";
            // 
            // copySelectedToolStripMenuItem
            // 
            this.copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            this.copySelectedToolStripMenuItem.Size = new System.Drawing.Size(187, 26);
            this.copySelectedToolStripMenuItem.Text = "Copy Selected";
            this.copySelectedToolStripMenuItem.Click += new System.EventHandler(this.copySelectedToolStripMenuItem_Click);
            // 
            // importSPDToolStripMenuItem
            // 
            this.importSPDToolStripMenuItem.Name = "importSPDToolStripMenuItem";
            this.importSPDToolStripMenuItem.Size = new System.Drawing.Size(187, 26);
            this.importSPDToolStripMenuItem.Text = "Import SPD";
            this.importSPDToolStripMenuItem.Click += new System.EventHandler(this.importSPDToolStripMenuItem_Click);
            // 
            // removeSelectedToolStripMenuItem
            // 
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(138, 26);
            this.removeSelectedToolStripMenuItem.Text = "Remove Selected";
            this.removeSelectedToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedToolStripMenuItem_Click);
            // 
            // editTexturesToolStripMenuItem
            // 
            this.editTexturesToolStripMenuItem.Name = "editTexturesToolStripMenuItem";
            this.editTexturesToolStripMenuItem.Size = new System.Drawing.Size(107, 26);
            this.editTexturesToolStripMenuItem.Text = "Edit Textures";
            this.editTexturesToolStripMenuItem.Click += new System.EventHandler(this.editTexturesToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.glModelView);
            this.splitContainer1.Size = new System.Drawing.Size(1846, 613);
            this.splitContainer1.SplitterDistance = 590;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnStop);
            this.splitContainer2.Panel1.Controls.Add(this.btnFreeze);
            this.splitContainer2.Panel1.Controls.Add(this.btnDisplay);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(590, 613);
            this.splitContainer2.SplitterDistance = 30;
            this.splitContainer2.TabIndex = 0;
            // 
            // btnStop
            // 
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnStop.Location = new System.Drawing.Point(240, 0);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(120, 30);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnFreeze
            // 
            this.btnFreeze.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFreeze.Location = new System.Drawing.Point(120, 0);
            this.btnFreeze.Margin = new System.Windows.Forms.Padding(4);
            this.btnFreeze.Name = "btnFreeze";
            this.btnFreeze.Size = new System.Drawing.Size(120, 30);
            this.btnFreeze.TabIndex = 7;
            this.btnFreeze.Text = "Freeze";
            this.btnFreeze.UseVisualStyleBackColor = true;
            this.btnFreeze.Click += new System.EventHandler(this.btnFreeze_Click);
            // 
            // btnDisplay
            // 
            this.btnDisplay.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnDisplay.Location = new System.Drawing.Point(0, 0);
            this.btnDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.btnDisplay.Name = "btnDisplay";
            this.btnDisplay.Size = new System.Drawing.Size(120, 30);
            this.btnDisplay.TabIndex = 6;
            this.btnDisplay.Text = "Display";
            this.btnDisplay.UseVisualStyleBackColor = true;
            this.btnDisplay.Click += new System.EventHandler(this.btnDisplay_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lbxSysDef);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pgSysDefProps);
            this.splitContainer3.Size = new System.Drawing.Size(590, 579);
            this.splitContainer3.SplitterDistance = 126;
            this.splitContainer3.TabIndex = 0;
            // 
            // lbxSysDef
            // 
            this.lbxSysDef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxSysDef.FormattingEnabled = true;
            this.lbxSysDef.ItemHeight = 16;
            this.lbxSysDef.Location = new System.Drawing.Point(0, 0);
            this.lbxSysDef.Name = "lbxSysDef";
            this.lbxSysDef.Size = new System.Drawing.Size(126, 579);
            this.lbxSysDef.TabIndex = 0;
            this.lbxSysDef.SelectedIndexChanged += new System.EventHandler(this.lbxSysDef_SelectedIndexChanged);
            // 
            // pgSysDefProps
            // 
            this.pgSysDefProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSysDefProps.Location = new System.Drawing.Point(0, 0);
            this.pgSysDefProps.Name = "pgSysDefProps";
            this.pgSysDefProps.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgSysDefProps.Size = new System.Drawing.Size(460, 579);
            this.pgSysDefProps.TabIndex = 0;
            this.pgSysDefProps.ToolbarVisible = false;
            this.pgSysDefProps.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgSysDefProps_PropertyValueChanged);
            // 
            // glModelView
            // 
            this.glModelView.BackColor = System.Drawing.Color.Black;
            this.glModelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glModelView.Location = new System.Drawing.Point(0, 0);
            this.glModelView.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.glModelView.Name = "glModelView";
            this.glModelView.Size = new System.Drawing.Size(1251, 613);
            this.glModelView.TabIndex = 1;
            this.glModelView.VSync = false;
            // 
            // ParticleViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1846, 641);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ParticleViewerForm";
            this.Text = "Particle Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ParticleViewerForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ParticleViewerForm_FormClosed);
            this.Load += new System.EventHandler(this.ParticleViewerForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadSPAToolStripMenuItem;
        private FormControls.ModelGLControlWithMarioSizeReference glModelView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnFreeze;
        private System.Windows.Forms.Button btnDisplay;
        private System.Windows.Forms.ListBox lbxSysDef;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.PropertyGrid pgSysDefProps;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceSPDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSPDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllSPDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCppToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addParticleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSPDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTexturesToolStripMenuItem;
    }
}