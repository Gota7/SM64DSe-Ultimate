namespace SM64DSe
{
    partial class LevelNameEditorForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelNameEditorForm));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnAddLevelSlot = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnImportXML = new System.Windows.Forms.ToolStripButton();
			this.lstLevels = new System.Windows.Forms.ListBox();
			this.txtLevelName = new System.Windows.Forms.TextBox();
			this.txtShortName = new System.Windows.Forms.TextBox();
			this.lblLevelName = new System.Windows.Forms.Label();
			this.lblShortName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblXMLData = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblOverlayID = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.txtCourseID = new System.Windows.Forms.NumericUpDown();
			this.txtCoursePart = new System.Windows.Forms.NumericUpDown();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtCourseID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtCoursePart)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.toolStripSeparator2,
            this.btnAddLevelSlot,
            this.toolStripSeparator1,
            this.btnImportXML});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(805, 25);
			this.toolStrip1.TabIndex = 10;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// btnSave
			// 
			this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(90, 22);
			this.btnSave.Text = "Save and Close";
			this.btnSave.ToolTipText = "Save the level names and overlay IDs";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// btnAddLevelSlot
			// 
			this.btnAddLevelSlot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnAddLevelSlot.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnAddLevelSlot.Name = "btnAddLevelSlot";
			this.btnAddLevelSlot.Size = new System.Drawing.Size(82, 22);
			this.btnAddLevelSlot.Text = "Add level slot";
			this.btnAddLevelSlot.ToolTipText = "Import level names from an XML file";
			this.btnAddLevelSlot.Click += new System.EventHandler(this.btnAddLevelSlot_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// btnImportXML
			// 
			this.btnImportXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnImportXML.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnImportXML.Name = "btnImportXML";
			this.btnImportXML.Size = new System.Drawing.Size(74, 22);
			this.btnImportXML.Text = "Import XML";
			this.btnImportXML.ToolTipText = "Import level names from an XML file";
			this.btnImportXML.Click += new System.EventHandler(this.btnImportXML_Click);
			// 
			// lstLevels
			// 
			this.lstLevels.Font = new System.Drawing.Font("Consolas", 8F);
			this.lstLevels.FormattingEnabled = true;
			this.lstLevels.Location = new System.Drawing.Point(9, 24);
			this.lstLevels.Margin = new System.Windows.Forms.Padding(2);
			this.lstLevels.Name = "lstLevels";
			this.lstLevels.Size = new System.Drawing.Size(427, 368);
			this.lstLevels.TabIndex = 11;
			this.lstLevels.SelectedIndexChanged += new System.EventHandler(this.lstLevels_SelectedIndexChanged);
			// 
			// txtLevelName
			// 
			this.txtLevelName.Location = new System.Drawing.Point(529, 39);
			this.txtLevelName.Margin = new System.Windows.Forms.Padding(2);
			this.txtLevelName.Name = "txtLevelName";
			this.txtLevelName.Size = new System.Drawing.Size(246, 20);
			this.txtLevelName.TabIndex = 12;
			this.txtLevelName.TextChanged += new System.EventHandler(this.txtLevelName_TextChanged);
			// 
			// txtShortName
			// 
			this.txtShortName.Location = new System.Drawing.Point(529, 63);
			this.txtShortName.Margin = new System.Windows.Forms.Padding(2);
			this.txtShortName.Name = "txtShortName";
			this.txtShortName.Size = new System.Drawing.Size(246, 20);
			this.txtShortName.TabIndex = 13;
			this.txtShortName.TextChanged += new System.EventHandler(this.txtShortName_TextChanged);
			// 
			// lblLevelName
			// 
			this.lblLevelName.AutoSize = true;
			this.lblLevelName.Location = new System.Drawing.Point(461, 41);
			this.lblLevelName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblLevelName.Name = "lblLevelName";
			this.lblLevelName.Size = new System.Drawing.Size(65, 13);
			this.lblLevelName.TabIndex = 15;
			this.lblLevelName.Text = "Level name:";
			// 
			// lblShortName
			// 
			this.lblShortName.AutoSize = true;
			this.lblShortName.Location = new System.Drawing.Point(461, 66);
			this.lblShortName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblShortName.Name = "lblShortName";
			this.lblShortName.Size = new System.Drawing.Size(64, 13);
			this.lblShortName.TabIndex = 16;
			this.lblShortName.Text = "Short name:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(466, 166);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 17;
			this.label1.Text = "Overlay ID:";
			// 
			// lblXMLData
			// 
			this.lblXMLData.AutoSize = true;
			this.lblXMLData.Location = new System.Drawing.Point(640, 24);
			this.lblXMLData.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblXMLData.Name = "lblXMLData";
			this.lblXMLData.Size = new System.Drawing.Size(136, 13);
			this.lblXMLData.TabIndex = 18;
			this.lblXMLData.Text = "SM64DSe Level XML Data";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(632, 100);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 13);
			this.label2.TabIndex = 19;
			this.label2.Text = "SM64DS Level Overlay Data";
			// 
			// lblOverlayID
			// 
			this.lblOverlayID.AutoSize = true;
			this.lblOverlayID.Location = new System.Drawing.Point(528, 166);
			this.lblOverlayID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblOverlayID.Name = "lblOverlayID";
			this.lblOverlayID.Size = new System.Drawing.Size(0, 13);
			this.lblOverlayID.TabIndex = 20;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(468, 118);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 13);
			this.label3.TabIndex = 21;
			this.label3.Text = "Course ID:";
			this.toolTip1.SetToolTip(this.label3, "The ID of the course this level slot belongs to.\r\n\r\n0-14: main courses\r\n15-28: si" +
        "de courses\r\n29: hub\r\n255: test map (star / cannon saving are broken)");
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(462, 142);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 13);
			this.label4.TabIndex = 24;
			this.label4.Text = "Course part:";
			this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
			// 
			// toolTip1
			// 
			this.toolTip1.Tag = "";
			// 
			// txtCourseID
			// 
			this.txtCourseID.Location = new System.Drawing.Point(529, 116);
			this.txtCourseID.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.txtCourseID.Name = "txtCourseID";
			this.txtCourseID.Size = new System.Drawing.Size(246, 20);
			this.txtCourseID.TabIndex = 25;
			this.txtCourseID.ValueChanged += new System.EventHandler(this.txtCourseID_ValueChanged);
			// 
			// txtCoursePart
			// 
			this.txtCoursePart.Location = new System.Drawing.Point(529, 140);
			this.txtCoursePart.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.txtCoursePart.Name = "txtCoursePart";
			this.txtCoursePart.Size = new System.Drawing.Size(246, 20);
			this.txtCoursePart.TabIndex = 26;
			this.txtCoursePart.ValueChanged += new System.EventHandler(this.txtCoursePart_ValueChanged);
			// 
			// LevelNameEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(805, 412);
			this.Controls.Add(this.txtCoursePart);
			this.Controls.Add(this.txtCourseID);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblOverlayID);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblXMLData);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblShortName);
			this.Controls.Add(this.lblLevelName);
			this.Controls.Add(this.txtShortName);
			this.Controls.Add(this.txtLevelName);
			this.Controls.Add(this.lstLevels);
			this.Controls.Add(this.toolStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "LevelNameEditorForm";
			this.Text = "Level List Editor";
			this.Load += new System.EventHandler(this.LevelNameEditorForm_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtCourseID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtCoursePart)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnImportXML;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ListBox lstLevels;
        private System.Windows.Forms.TextBox txtLevelName;
        private System.Windows.Forms.TextBox txtShortName;
        private System.Windows.Forms.Label lblLevelName;
        private System.Windows.Forms.Label lblShortName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblXMLData;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblOverlayID;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ToolStripButton btnAddLevelSlot;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.NumericUpDown txtCourseID;
		private System.Windows.Forms.NumericUpDown txtCoursePart;
	}
}