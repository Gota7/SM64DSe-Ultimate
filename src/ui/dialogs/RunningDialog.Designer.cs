using System.ComponentModel;

namespace SM64DSe.ui.dialogs
{
    partial class RunningDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.Windows.Forms.Label label;
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label
            // 
            label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            label.Location = new System.Drawing.Point(93, 39);
            label.Name = "label";
            label.Size = new System.Drawing.Size(257, 53);
            label.TabIndex = 0;
            label.Text = "ROM is running. Waiting for exit..";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 73);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(420, 35);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
            // 
            // RunningDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 131);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(label);
            this.MaximumSize = new System.Drawing.Size(466, 187);
            this.MinimumSize = new System.Drawing.Size(466, 187);
            this.Name = "RunningDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "RunningDialog";
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ProgressBar progressBar1;

        private System.Windows.Forms.Label label1;

        #endregion
    }
}