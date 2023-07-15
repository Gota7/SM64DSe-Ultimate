namespace SM64DSe.SM64DSFormats
{
    partial class CreateNewLevelAnimationDialog
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
            this.lblAnimationLength = new System.Windows.Forms.Label();
            this.numAnimationLength = new System.Windows.Forms.NumericUpDown();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numAnimationLength)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAnimationLength
            // 
            this.lblAnimationLength.AutoSize = true;
            this.lblAnimationLength.Location = new System.Drawing.Point(3, 10);
            this.lblAnimationLength.Name = "lblAnimationLength";
            this.lblAnimationLength.Size = new System.Drawing.Size(126, 13);
            this.lblAnimationLength.TabIndex = 0;
            this.lblAnimationLength.Text = "Animation Length(frames)";
            // 
            // numAnimationLength
            // 
            this.numAnimationLength.Location = new System.Drawing.Point(135, 8);
            this.numAnimationLength.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numAnimationLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAnimationLength.Name = "numAnimationLength";
            this.numAnimationLength.Size = new System.Drawing.Size(120, 20);
            this.numAnimationLength.TabIndex = 1;
            this.numAnimationLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnCreate
            // 
            this.btnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCreate.Location = new System.Drawing.Point(34, 92);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "Create!";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(198, 92);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(293, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "WARNING: This will delete all existing animations in this area";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblAnimationLength);
            this.panel1.Controls.Add(this.numAnimationLength);
            this.panel1.Location = new System.Drawing.Point(12, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(269, 38);
            this.panel1.TabIndex = 5;
            // 
            // CreateNewLevelAnimationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 127);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Name = "CreateNewLevelAnimationDialog";
            this.Text = "Setup Level Animation for area ";
            ((System.ComponentModel.ISupportInitialize)(this.numAnimationLength)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAnimationLength;
        private System.Windows.Forms.NumericUpDown numAnimationLength;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}