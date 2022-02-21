namespace SM64DSe
{
    partial class ParticleEdtorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParticleEdtorForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnImportTexture = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLoadTPS = new System.Windows.Forms.ToolStripButton();
            this.pbxTexture = new System.Windows.Forms.PictureBox();
            this.chkRepeatS = new System.Windows.Forms.CheckBox();
            this.chkRepeatT = new System.Windows.Forms.CheckBox();
            this.chkFlipS = new System.Windows.Forms.CheckBox();
            this.chkFlipT = new System.Windows.Forms.CheckBox();
            this.chkTransparent = new System.Windows.Forms.CheckBox();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.lblCompression = new System.Windows.Forms.Label();
            this.btnModelPalettesSelectedColour = new System.Windows.Forms.Button();
            this.lblModelPalettesPaletteSelectedColour = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.grdPalette = new SM64DSe.PaletteColourGrid();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdPalette)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnImportTexture,
            this.toolStripSeparator1,
            this.btnExport,
            this.toolStripSeparator2,
            this.btnLoadTPS});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(740, 31);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(44, 28);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save the TPS file";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnImportTexture
            // 
            this.btnImportTexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportTexture.Name = "btnImportTexture";
            this.btnImportTexture.Size = new System.Drawing.Size(110, 28);
            this.btnImportTexture.Text = "Import Texture";
            this.btnImportTexture.Click += new System.EventHandler(this.btnImportTexture_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(108, 28);
            this.btnExport.Text = "Export Texture";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // btnLoadTPS
            // 
            this.btnLoadTPS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoadTPS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadTPS.Name = "btnLoadTPS";
            this.btnLoadTPS.Size = new System.Drawing.Size(74, 28);
            this.btnLoadTPS.Text = "Load TPS";
            this.btnLoadTPS.Click += new System.EventHandler(this.btnLoadTPS_Click);
            // 
            // pbxTexture
            // 
            this.pbxTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxTexture.Location = new System.Drawing.Point(16, 34);
            this.pbxTexture.Margin = new System.Windows.Forms.Padding(4);
            this.pbxTexture.Name = "pbxTexture";
            this.pbxTexture.Size = new System.Drawing.Size(320, 320);
            this.pbxTexture.TabIndex = 11;
            this.pbxTexture.TabStop = false;
            this.pbxTexture.Click += new System.EventHandler(this.pbxTexture_Click);
            // 
            // chkRepeatS
            // 
            this.chkRepeatS.AutoSize = true;
            this.chkRepeatS.Location = new System.Drawing.Point(16, 362);
            this.chkRepeatS.Margin = new System.Windows.Forms.Padding(4);
            this.chkRepeatS.Name = "chkRepeatS";
            this.chkRepeatS.Size = new System.Drawing.Size(89, 21);
            this.chkRepeatS.TabIndex = 13;
            this.chkRepeatS.Text = "Repeat S";
            this.chkRepeatS.UseVisualStyleBackColor = true;
            this.chkRepeatS.CheckedChanged += new System.EventHandler(this.chkRepeatS_CheckedChanged);
            // 
            // chkRepeatT
            // 
            this.chkRepeatT.AutoSize = true;
            this.chkRepeatT.Location = new System.Drawing.Point(16, 390);
            this.chkRepeatT.Margin = new System.Windows.Forms.Padding(4);
            this.chkRepeatT.Name = "chkRepeatT";
            this.chkRepeatT.Size = new System.Drawing.Size(89, 21);
            this.chkRepeatT.TabIndex = 14;
            this.chkRepeatT.Text = "Repeat T";
            this.chkRepeatT.UseVisualStyleBackColor = true;
            this.chkRepeatT.CheckedChanged += new System.EventHandler(this.chkRepeatT_CheckedChanged);
            // 
            // chkFlipS
            // 
            this.chkFlipS.AutoSize = true;
            this.chkFlipS.Location = new System.Drawing.Point(16, 418);
            this.chkFlipS.Margin = new System.Windows.Forms.Padding(4);
            this.chkFlipS.Name = "chkFlipS";
            this.chkFlipS.Size = new System.Drawing.Size(65, 21);
            this.chkFlipS.TabIndex = 15;
            this.chkFlipS.Text = "Flip S";
            this.chkFlipS.UseVisualStyleBackColor = true;
            this.chkFlipS.CheckedChanged += new System.EventHandler(this.chkFlipS_CheckedChanged);
            // 
            // chkFlipT
            // 
            this.chkFlipT.AutoSize = true;
            this.chkFlipT.Location = new System.Drawing.Point(16, 447);
            this.chkFlipT.Margin = new System.Windows.Forms.Padding(4);
            this.chkFlipT.Name = "chkFlipT";
            this.chkFlipT.Size = new System.Drawing.Size(65, 21);
            this.chkFlipT.TabIndex = 16;
            this.chkFlipT.Text = "Flip T";
            this.chkFlipT.UseVisualStyleBackColor = true;
            this.chkFlipT.CheckedChanged += new System.EventHandler(this.chkFlipT_CheckedChanged);
            // 
            // chkTransparent
            // 
            this.chkTransparent.AutoSize = true;
            this.chkTransparent.Location = new System.Drawing.Point(16, 475);
            this.chkTransparent.Margin = new System.Windows.Forms.Padding(4);
            this.chkTransparent.Name = "chkTransparent";
            this.chkTransparent.Size = new System.Drawing.Size(176, 21);
            this.chkTransparent.TabIndex = 17;
            this.chkTransparent.Text = "First Color Transparent";
            this.chkTransparent.UseVisualStyleBackColor = true;
            this.chkTransparent.CheckedChanged += new System.EventHandler(this.chkTransparent_CheckedChanged);
            // 
            // cmbFormat
            // 
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Location = new System.Drawing.Point(201, 385);
            this.cmbFormat.Margin = new System.Windows.Forms.Padding(4);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(160, 24);
            this.cmbFormat.TabIndex = 18;
            this.cmbFormat.SelectedIndexChanged += new System.EventHandler(this.cmbCompression_SelectedIndexChanged);
            // 
            // lblCompression
            // 
            this.lblCompression.AutoSize = true;
            this.lblCompression.Location = new System.Drawing.Point(201, 362);
            this.lblCompression.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCompression.Name = "lblCompression";
            this.lblCompression.Size = new System.Drawing.Size(119, 17);
            this.lblCompression.TabIndex = 19;
            this.lblCompression.Text = "Format on import:";
            // 
            // btnModelPalettesSelectedColour
            // 
            this.btnModelPalettesSelectedColour.Location = new System.Drawing.Point(617, 385);
            this.btnModelPalettesSelectedColour.Margin = new System.Windows.Forms.Padding(4);
            this.btnModelPalettesSelectedColour.Name = "btnModelPalettesSelectedColour";
            this.btnModelPalettesSelectedColour.Size = new System.Drawing.Size(100, 28);
            this.btnModelPalettesSelectedColour.TabIndex = 20;
            this.btnModelPalettesSelectedColour.Text = "#XXXXXX";
            this.btnModelPalettesSelectedColour.UseVisualStyleBackColor = true;
            this.btnModelPalettesSelectedColour.Click += new System.EventHandler(this.btnModelPalettesSelectedColour_Click);
            // 
            // lblModelPalettesPaletteSelectedColour
            // 
            this.lblModelPalettesPaletteSelectedColour.AutoSize = true;
            this.lblModelPalettesPaletteSelectedColour.Location = new System.Drawing.Point(604, 362);
            this.lblModelPalettesPaletteSelectedColour.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblModelPalettesPaletteSelectedColour.Name = "lblModelPalettesPaletteSelectedColour";
            this.lblModelPalettesPaletteSelectedColour.Size = new System.Drawing.Size(112, 17);
            this.lblModelPalettesPaletteSelectedColour.TabIndex = 21;
            this.lblModelPalettesPaletteSelectedColour.Text = "Selected Colour:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(201, 418);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 28);
            this.button1.TabIndex = 23;
            this.button1.Text = "Change Format";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.btnChangeFormat_Click);
            // 
            // grdPalette
            // 
            this.grdPalette.AllowUserToAddRows = false;
            this.grdPalette.AllowUserToDeleteRows = false;
            this.grdPalette.AllowUserToResizeColumns = false;
            this.grdPalette.AllowUserToResizeRows = false;
            this.grdPalette.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.grdPalette.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdPalette.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPalette.ColumnHeadersVisible = false;
            this.grdPalette.EnableHeadersVisualStyles = false;
            this.grdPalette.Location = new System.Drawing.Point(371, 34);
            this.grdPalette.Margin = new System.Windows.Forms.Padding(4);
            this.grdPalette.MultiSelect = false;
            this.grdPalette.Name = "grdPalette";
            this.grdPalette.ReadOnly = true;
            this.grdPalette.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.grdPalette.RowHeadersVisible = false;
            this.grdPalette.RowHeadersWidth = 23;
            this.grdPalette.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.grdPalette.RowTemplate.Height = 16;
            this.grdPalette.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grdPalette.Size = new System.Drawing.Size(347, 320);
            this.grdPalette.TabIndex = 12;
            this.grdPalette.CurrentCellChanged += new System.EventHandler(this.grdPalette_CurrentCellChanged);
            // 
            // ParticleEdtorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 507);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnModelPalettesSelectedColour);
            this.Controls.Add(this.lblModelPalettesPaletteSelectedColour);
            this.Controls.Add(this.lblCompression);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.chkTransparent);
            this.Controls.Add(this.chkFlipT);
            this.Controls.Add(this.chkFlipS);
            this.Controls.Add(this.chkRepeatT);
            this.Controls.Add(this.chkRepeatS);
            this.Controls.Add(this.grdPalette);
            this.Controls.Add(this.pbxTexture);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ParticleEdtorForm";
            this.Text = "TPS Editor";
            this.Load += new System.EventHandler(this.TextureEditorForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdPalette)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoadTPS;
        private System.Windows.Forms.ToolStripButton btnImportTexture;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.PictureBox pbxTexture;
        private PaletteColourGrid grdPalette;
        private System.Windows.Forms.CheckBox chkRepeatS;
        private System.Windows.Forms.CheckBox chkRepeatT;
        private System.Windows.Forms.CheckBox chkFlipS;
        private System.Windows.Forms.CheckBox chkFlipT;
        private System.Windows.Forms.CheckBox chkTransparent;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.Label lblCompression;
        private System.Windows.Forms.Button btnModelPalettesSelectedColour;
        private System.Windows.Forms.Label lblModelPalettesPaletteSelectedColour;
        private System.Windows.Forms.Button button1;
    }
}