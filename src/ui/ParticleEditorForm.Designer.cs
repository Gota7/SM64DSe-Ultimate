namespace SM64DSe
{
    partial class ParticleEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParticleEditorForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnImportTexture = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLoadSPT = new System.Windows.Forms.ToolStripButton();
            this.pbxTexture = new System.Windows.Forms.PictureBox();
            this.btnModelPalettesSelectedColour = new System.Windows.Forms.Button();
            this.lblModelPalettesPaletteSelectedColour = new System.Windows.Forms.Label();
            this.grdPalette = new SM64DSe.PaletteColourGrid();
            this.cmbRepeatY = new System.Windows.Forms.ComboBox();
            this.lblRepeatY = new System.Windows.Forms.Label();
            this.lblRepeatX = new System.Windows.Forms.Label();
            this.cmbRepeatX = new System.Windows.Forms.ComboBox();
            this.lblFormat = new System.Windows.Forms.Label();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
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
            this.btnLoadSPT});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(740, 27);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(44, 24);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save the SPT file";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnImportTexture
            // 
            this.btnImportTexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportTexture.Name = "btnImportTexture";
            this.btnImportTexture.Size = new System.Drawing.Size(110, 24);
            this.btnImportTexture.Text = "Import Texture";
            this.btnImportTexture.Click += new System.EventHandler(this.btnImportTexture_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(108, 24);
            this.btnExport.Text = "Export Texture";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnLoadSPT
            // 
            this.btnLoadSPT.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoadSPT.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadSPT.Name = "btnLoadSPT";
            this.btnLoadSPT.Size = new System.Drawing.Size(74, 24);
            this.btnLoadSPT.Text = "Load SPT";
            this.btnLoadSPT.Click += new System.EventHandler(this.btnLoadSPT_Click);
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
            // cmbRepeatY
            // 
            this.cmbRepeatY.FormattingEnabled = true;
            this.cmbRepeatY.Location = new System.Drawing.Point(143, 419);
            this.cmbRepeatY.Name = "cmbRepeatY";
            this.cmbRepeatY.Size = new System.Drawing.Size(121, 24);
            this.cmbRepeatY.TabIndex = 46;
            this.cmbRepeatY.SelectedIndexChanged += new System.EventHandler(this.cmbRepeatY_SelectedIndexChanged);
            // 
            // lblRepeatY
            // 
            this.lblRepeatY.AutoSize = true;
            this.lblRepeatY.Location = new System.Drawing.Point(13, 426);
            this.lblRepeatY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRepeatY.Name = "lblRepeatY";
            this.lblRepeatY.Size = new System.Drawing.Size(110, 17);
            this.lblRepeatY.TabIndex = 45;
            this.lblRepeatY.Text = "Repeat Mode Y:";
            // 
            // lblRepeatX
            // 
            this.lblRepeatX.AutoSize = true;
            this.lblRepeatX.Location = new System.Drawing.Point(13, 396);
            this.lblRepeatX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRepeatX.Name = "lblRepeatX";
            this.lblRepeatX.Size = new System.Drawing.Size(110, 17);
            this.lblRepeatX.TabIndex = 44;
            this.lblRepeatX.Text = "Repeat Mode X:";
            // 
            // cmbRepeatX
            // 
            this.cmbRepeatX.FormattingEnabled = true;
            this.cmbRepeatX.Location = new System.Drawing.Point(143, 389);
            this.cmbRepeatX.Name = "cmbRepeatX";
            this.cmbRepeatX.Size = new System.Drawing.Size(121, 24);
            this.cmbRepeatX.TabIndex = 43;
            this.cmbRepeatX.SelectedIndexChanged += new System.EventHandler(this.cmbRepeatX_SelectedIndexChanged);
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(13, 457);
            this.lblFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(119, 17);
            this.lblFormat.TabIndex = 42;
            this.lblFormat.Text = "Format on import:";
            // 
            // cmbFormat
            // 
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Location = new System.Drawing.Point(143, 450);
            this.cmbFormat.Margin = new System.Windows.Forms.Padding(4);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(121, 24);
            this.cmbFormat.TabIndex = 41;
            this.cmbFormat.SelectedIndexChanged += new System.EventHandler(this.cmbFormat_SelectedIndexChanged);
            // 
            // ParticleEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 507);
            this.Controls.Add(this.cmbRepeatY);
            this.Controls.Add(this.lblRepeatY);
            this.Controls.Add(this.lblRepeatX);
            this.Controls.Add(this.cmbRepeatX);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.btnModelPalettesSelectedColour);
            this.Controls.Add(this.lblModelPalettesPaletteSelectedColour);
            this.Controls.Add(this.grdPalette);
            this.Controls.Add(this.pbxTexture);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ParticleEditorForm";
            this.Text = "SPT Editor";
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
        private System.Windows.Forms.ToolStripButton btnLoadSPT;
        private System.Windows.Forms.ToolStripButton btnImportTexture;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.PictureBox pbxTexture;
        private PaletteColourGrid grdPalette;
        private System.Windows.Forms.Button btnModelPalettesSelectedColour;
        private System.Windows.Forms.Label lblModelPalettesPaletteSelectedColour;
        private System.Windows.Forms.ComboBox cmbRepeatY;
        private System.Windows.Forms.Label lblRepeatY;
        private System.Windows.Forms.Label lblRepeatX;
        private System.Windows.Forms.ComboBox cmbRepeatX;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbFormat;
    }
}