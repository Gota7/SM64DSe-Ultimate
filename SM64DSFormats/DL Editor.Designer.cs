namespace SM64DSe.SM64DSFormats
{
    partial class DL_Editor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DL_Editor));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.btnRemove = new System.Windows.Forms.ToolStripSplitButton();
            this.btnAdd = new System.Windows.Forms.ToolStripSplitButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.availableTree = new System.Windows.Forms.TreeView();
            this.currentTree = new System.Windows.Forms.TreeView();
            this.btnSave = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.toolStripStatusLabel1,
            this.btnSave,
            this.toolStripStatusLabel2,
            this.btnRemove});
            this.statusStrip1.Location = new System.Drawing.Point(0, 282);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(521, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // btnRemove
            // 
            this.btnRemove.AutoSize = false;
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemove.DropDownButtonWidth = 0;
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(100, 20);
            this.btnRemove.Text = "Remove";
            this.btnRemove.ButtonClick += new System.EventHandler(this.btnRemove_ButtonClick);
            // 
            // btnAdd
            // 
            this.btnAdd.AutoSize = false;
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAdd.DropDownButtonWidth = 0;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 20);
            this.btnAdd.Text = "Add";
            this.btnAdd.ButtonClick += new System.EventHandler(this.btnAdd_ButtonClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.availableTree);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.currentTree);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(521, 282);
            this.splitContainer1.SplitterDistance = 254;
            this.splitContainer1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Available:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(261, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Current:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(105, 17);
            // 
            // availableTree
            // 
            this.availableTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableTree.Location = new System.Drawing.Point(0, 23);
            this.availableTree.Name = "availableTree";
            this.availableTree.Size = new System.Drawing.Size(252, 257);
            this.availableTree.TabIndex = 1;
            // 
            // currentTree
            // 
            this.currentTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentTree.Location = new System.Drawing.Point(0, 23);
            this.currentTree.Name = "currentTree";
            this.currentTree.Size = new System.Drawing.Size(261, 257);
            this.currentTree.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = false;
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.DropDownButtonWidth = 0;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 20);
            this.btnSave.Text = "Save";
            this.btnSave.ButtonClick += new System.EventHandler(this.btnSave_ButtonClick);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(100, 17);
            // 
            // DL_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 304);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DL_Editor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dynamic Library Editor";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSplitButton btnRemove;
        private System.Windows.Forms.ToolStripSplitButton btnAdd;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView availableTree;
        private System.Windows.Forms.TreeView currentTree;
        private System.Windows.Forms.ToolStripSplitButton btnSave;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}