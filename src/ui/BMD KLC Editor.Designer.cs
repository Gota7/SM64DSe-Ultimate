namespace SM64DSe
{
    partial class BMD_KLC_Editor
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
            this.tvModelContent = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panProperties = new System.Windows.Forms.Panel();
            this.box_position = new System.Windows.Forms.GroupBox();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvModelContent
            // 
            this.tvModelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvModelContent.Location = new System.Drawing.Point(3, 3);
            this.tvModelContent.Name = "tvModelContent";
            this.tvModelContent.Size = new System.Drawing.Size(332, 344);
            this.tvModelContent.TabIndex = 0;
            this.tvModelContent.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvModelContent_AfterSelect);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panProperties);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tvModelContent);
            this.splitContainer1.Size = new System.Drawing.Size(512, 350);
            this.splitContainer1.SplitterDistance = 170;
            this.splitContainer1.TabIndex = 1;
            // 
            // panProperties
            // 
            this.panProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panProperties.Controls.Add(this.box_position);
            this.panProperties.Location = new System.Drawing.Point(3, 3);
            this.panProperties.Name = "panProperties";
            this.panProperties.Size = new System.Drawing.Size(164, 344);
            this.panProperties.TabIndex = 0;
            // 
            // box_position
            // 
            this.box_position.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.box_position.Location = new System.Drawing.Point(4, 4);
            this.box_position.Name = "box_position";
            this.box_position.Size = new System.Drawing.Size(155, 52);
            this.box_position.TabIndex = 0;
            this.box_position.TabStop = false;
            this.box_position.Text = "Position";
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(441, 368);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(83, 23);
            this.btnSaveChanges.TabIndex = 2;
            this.btnSaveChanges.Text = "SaveChanges";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // BMD_KLC_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 403);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.splitContainer1);
            this.Name = "BMD_KLC_Editor";
            this.Text = "BMD_KLC_Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvModelContent;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panProperties;
        private System.Windows.Forms.GroupBox box_position;
        private System.Windows.Forms.Button btnSaveChanges;
    }
}