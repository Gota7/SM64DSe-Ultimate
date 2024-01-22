namespace SM64DSe
{
    partial class ParticleTextureForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClose = new System.Windows.Forms.Button();
            this.btnModelPalettesSelectedColour = new System.Windows.Forms.Button();
            this.lblModelPalettesPaletteSelectedColour = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.pbxTexture = new System.Windows.Forms.PictureBox();
            this.lbxTexDef = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.replaceSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadExternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.internalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.externalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbRepeatX = new System.Windows.Forms.ComboBox();
            this.lblRepeatX = new System.Windows.Forms.Label();
            this.lblRepeatY = new System.Windows.Forms.Label();
            this.cmbRepeatY = new System.Windows.Forms.ComboBox();
            this.grdPalette = new SM64DSe.PaletteColourGrid();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPalette)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(743, 532);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnModelPalettesSelectedColour
            // 
            this.btnModelPalettesSelectedColour.Location = new System.Drawing.Point(743, 383);
            this.btnModelPalettesSelectedColour.Margin = new System.Windows.Forms.Padding(4);
            this.btnModelPalettesSelectedColour.Name = "btnModelPalettesSelectedColour";
            this.btnModelPalettesSelectedColour.Size = new System.Drawing.Size(100, 28);
            this.btnModelPalettesSelectedColour.TabIndex = 33;
            this.btnModelPalettesSelectedColour.Text = "#XXXXXX";
            this.btnModelPalettesSelectedColour.UseVisualStyleBackColor = true;
            this.btnModelPalettesSelectedColour.Click += new System.EventHandler(this.btnModelPalettesSelectedColour_Click);
            // 
            // lblModelPalettesPaletteSelectedColour
            // 
            this.lblModelPalettesPaletteSelectedColour.AutoSize = true;
            this.lblModelPalettesPaletteSelectedColour.Location = new System.Drawing.Point(730, 360);
            this.lblModelPalettesPaletteSelectedColour.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblModelPalettesPaletteSelectedColour.Name = "lblModelPalettesPaletteSelectedColour";
            this.lblModelPalettesPaletteSelectedColour.Size = new System.Drawing.Size(112, 17);
            this.lblModelPalettesPaletteSelectedColour.TabIndex = 34;
            this.lblModelPalettesPaletteSelectedColour.Text = "Selected Colour:";
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(192, 425);
            this.lblFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(119, 17);
            this.lblFormat.TabIndex = 32;
            this.lblFormat.Text = "Format on import:";
            // 
            // cmbFormat
            // 
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Location = new System.Drawing.Point(322, 418);
            this.cmbFormat.Margin = new System.Windows.Forms.Padding(4);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(121, 24);
            this.cmbFormat.TabIndex = 31;
            this.cmbFormat.SelectedIndexChanged += new System.EventHandler(this.cmbFormat_SelectedIndexChanged);
            // 
            // pbxTexture
            // 
            this.pbxTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxTexture.Location = new System.Drawing.Point(195, 32);
            this.pbxTexture.Margin = new System.Windows.Forms.Padding(4);
            this.pbxTexture.Name = "pbxTexture";
            this.pbxTexture.Size = new System.Drawing.Size(320, 320);
            this.pbxTexture.TabIndex = 24;
            this.pbxTexture.TabStop = false;
            // 
            // lbxTexDef
            // 
            this.lbxTexDef.FormattingEnabled = true;
            this.lbxTexDef.ItemHeight = 16;
            this.lbxTexDef.Location = new System.Drawing.Point(8, 32);
            this.lbxTexDef.Name = "lbxTexDef";
            this.lbxTexDef.Size = new System.Drawing.Size(180, 532);
            this.lbxTexDef.TabIndex = 35;
            this.lbxTexDef.SelectedIndexChanged += new System.EventHandler(this.lbxTexDef_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceSelectedToolStripMenuItem,
            this.loadExternalToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.addTextureToolStripMenuItem,
            this.removeSelectedToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(883, 28);
            this.menuStrip1.TabIndex = 36;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // replaceSelectedToolStripMenuItem
            // 
            this.replaceSelectedToolStripMenuItem.Name = "replaceSelectedToolStripMenuItem";
            this.replaceSelectedToolStripMenuItem.Size = new System.Drawing.Size(137, 24);
            this.replaceSelectedToolStripMenuItem.Text = "Replace Selected";
            this.replaceSelectedToolStripMenuItem.Click += new System.EventHandler(this.replaceSelectedToolStripMenuItem_Click);
            // 
            // loadExternalToolStripMenuItem
            // 
            this.loadExternalToolStripMenuItem.Name = "loadExternalToolStripMenuItem";
            this.loadExternalToolStripMenuItem.Size = new System.Drawing.Size(113, 24);
            this.loadExternalToolStripMenuItem.Text = "Load External";
            this.loadExternalToolStripMenuItem.Click += new System.EventHandler(this.loadExternalToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // addTextureToolStripMenuItem
            // 
            this.addTextureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.internalToolStripMenuItem,
            this.externalToolStripMenuItem});
            this.addTextureToolStripMenuItem.Name = "addTextureToolStripMenuItem";
            this.addTextureToolStripMenuItem.Size = new System.Drawing.Size(103, 24);
            this.addTextureToolStripMenuItem.Text = "Add Texture";
            // 
            // internalToolStripMenuItem
            // 
            this.internalToolStripMenuItem.Name = "internalToolStripMenuItem";
            this.internalToolStripMenuItem.Size = new System.Drawing.Size(145, 26);
            this.internalToolStripMenuItem.Text = "Internal";
            this.internalToolStripMenuItem.Click += new System.EventHandler(this.internalToolStripMenuItem_Click);
            // 
            // externalToolStripMenuItem
            // 
            this.externalToolStripMenuItem.Name = "externalToolStripMenuItem";
            this.externalToolStripMenuItem.Size = new System.Drawing.Size(145, 26);
            this.externalToolStripMenuItem.Text = "External";
            this.externalToolStripMenuItem.Click += new System.EventHandler(this.externalToolStripMenuItem_Click);
            // 
            // removeSelectedToolStripMenuItem
            // 
            this.removeSelectedToolStripMenuItem.Enabled = false;
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(138, 24);
            this.removeSelectedToolStripMenuItem.Text = "Remove Selected";
            this.removeSelectedToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedToolStripMenuItem_Click);
            // 
            // cmbRepeatX
            // 
            this.cmbRepeatX.FormattingEnabled = true;
            this.cmbRepeatX.Location = new System.Drawing.Point(322, 357);
            this.cmbRepeatX.Name = "cmbRepeatX";
            this.cmbRepeatX.Size = new System.Drawing.Size(121, 24);
            this.cmbRepeatX.TabIndex = 37;
            this.cmbRepeatX.SelectedIndexChanged += new System.EventHandler(this.cmbRepeatX_SelectedIndexChanged);
            // 
            // lblRepeatX
            // 
            this.lblRepeatX.AutoSize = true;
            this.lblRepeatX.Location = new System.Drawing.Point(192, 364);
            this.lblRepeatX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRepeatX.Name = "lblRepeatX";
            this.lblRepeatX.Size = new System.Drawing.Size(110, 17);
            this.lblRepeatX.TabIndex = 38;
            this.lblRepeatX.Text = "Repeat Mode X:";
            // 
            // lblRepeatY
            // 
            this.lblRepeatY.AutoSize = true;
            this.lblRepeatY.Location = new System.Drawing.Point(192, 394);
            this.lblRepeatY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRepeatY.Name = "lblRepeatY";
            this.lblRepeatY.Size = new System.Drawing.Size(110, 17);
            this.lblRepeatY.TabIndex = 39;
            this.lblRepeatY.Text = "Repeat Mode Y:";
            // 
            // cmbRepeatY
            // 
            this.cmbRepeatY.FormattingEnabled = true;
            this.cmbRepeatY.Location = new System.Drawing.Point(322, 387);
            this.cmbRepeatY.Name = "cmbRepeatY";
            this.cmbRepeatY.Size = new System.Drawing.Size(121, 24);
            this.cmbRepeatY.TabIndex = 40;
            this.cmbRepeatY.SelectedIndexChanged += new System.EventHandler(this.cmbRepeatY_SelectedIndexChanged);
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
            this.grdPalette.Location = new System.Drawing.Point(523, 32);
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
            this.grdPalette.Size = new System.Drawing.Size(320, 320);
            this.grdPalette.TabIndex = 25;
            this.grdPalette.CurrentCellChanged += new System.EventHandler(this.grdPalette_CurrentCellChanged);
            // 
            // ParticleTextureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 572);
            this.Controls.Add(this.cmbRepeatY);
            this.Controls.Add(this.lblRepeatY);
            this.Controls.Add(this.lblRepeatX);
            this.Controls.Add(this.cmbRepeatX);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lbxTexDef);
            this.Controls.Add(this.btnModelPalettesSelectedColour);
            this.Controls.Add(this.lblModelPalettesPaletteSelectedColour);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.grdPalette);
            this.Controls.Add(this.pbxTexture);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ParticleTextureForm";
            this.Text = "Particle Texture Editor (SPA Edition)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ParticleTextureForm_FormClosed);
            this.Load += new System.EventHandler(this.ParticleTextureForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPalette)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnModelPalettesSelectedColour;
        private System.Windows.Forms.Label lblModelPalettesPaletteSelectedColour;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbFormat;
        private PaletteColourGrid grdPalette;
        private System.Windows.Forms.PictureBox pbxTexture;
        private System.Windows.Forms.ListBox lbxTexDef;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem replaceSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem internalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem externalToolStripMenuItem;
        private System.Windows.Forms.ComboBox cmbRepeatX;
        private System.Windows.Forms.Label lblRepeatX;
        private System.Windows.Forms.Label lblRepeatY;
        private System.Windows.Forms.ComboBox cmbRepeatY;
        private System.Windows.Forms.ToolStripMenuItem loadExternalToolStripMenuItem;
    }
}