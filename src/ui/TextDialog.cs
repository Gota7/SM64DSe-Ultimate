// Decompiled with JetBrains decompiler
// Type: SM64DSe.TextDialog
// Assembly: SM64DSe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: ABA5C26D-8571-4083-8DE0-4D1667F321FB
// Assembly location: C:\Users\samsp\Documents\The Shining\Cutscene\FileSystemEditor\FileSystemEditor.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SM64DSe {

    /// <summary>
    /// Text dialog. Credits to HayashiSTL
    /// </summary>
    public class TextDialog : Form {
        private IContainer components;
        private TextBox txtText;
        private Button btnCancel;
        private Button btnOK;
        private Label lblSubtitle;

        public string typedText {
            get {
                return this.txtText.Text;
            }
            set { txtText.Text = value; }
        }

        public TextDialog(string text, string title) {
            this.InitializeComponent();
            this.Text = title;
            this.lblSubtitle.Text = text;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void Dispose(bool disposing) {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.StartPosition = FormStartPosition.CenterParent;
            this.txtText = new TextBox();
            this.btnCancel = new Button();
            this.btnOK = new Button();
            this.lblSubtitle = new Label();
            this.SuspendLayout();
            this.txtText.Location = new Point(15, 25);
            this.txtText.Name = "txtText";
            this.txtText.Size = new Size(261, 20);
            this.txtText.TabIndex = 0;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(201, 51);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.btnOK.Location = new Point(15, 51);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new Point(12, 9);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new Size(219, 13);
            this.lblSubtitle.TabIndex = 3;
            this.lblSubtitle.Text = "Clue: the title may have to do with the word...";
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(288, 86);
            this.Controls.Add((Control)this.lblSubtitle);
            this.Controls.Add((Control)this.btnOK);
            this.Controls.Add((Control)this.btnCancel);
            this.Controls.Add((Control)this.txtText);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = nameof(TextDialog);
            this.Text = "All star";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
