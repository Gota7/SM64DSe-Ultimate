namespace SM64DSe
{
    partial class ROMFileSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ROMFileSelect));
            this.tvFiles = new System.Windows.Forms.TreeView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkMergeArcs = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tvFiles
            // 
            this.tvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFiles.Location = new System.Drawing.Point(13, 13);
            this.tvFiles.Name = "tvFiles";
            this.tvFiles.PathSeparator = "/";
            this.tvFiles.Size = new System.Drawing.Size(392, 260);
            this.tvFiles.TabIndex = 0;
            this.tvFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFiles_AfterSelect);
            this.tvFiles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvFiles_KeyUp);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(249, 290);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(330, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // checkMergeArcs
            // 
            this.checkMergeArcs.AutoSize = true;
            this.checkMergeArcs.Location = new System.Drawing.Point(13, 279);
            this.checkMergeArcs.Name = "checkMergeArcs";
            this.checkMergeArcs.Size = new System.Drawing.Size(140, 17);
            this.checkMergeArcs.TabIndex = 3;
            this.checkMergeArcs.Text = "Merge from .narc entries";
            this.checkMergeArcs.UseVisualStyleBackColor = true;
            this.checkMergeArcs.CheckedChanged += new System.EventHandler(this.checkMergeArcs_CheckedChanged);
            // 
            // ROMFileSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 325);
            this.Controls.Add(this.checkMergeArcs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tvFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ROMFileSelect";
            this.Text = "ROMFileSelect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvFiles;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox checkMergeArcs;
    }
}