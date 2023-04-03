namespace SM64DSe
{
    partial class AnimationEditorForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationEditorForm));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.gbImportAnimation = new System.Windows.Forms.GroupBox();
			this.chkOptimise = new System.Windows.Forms.CheckBox();
			this.txtScale = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnImportAnimation = new System.Windows.Forms.Button();
			this.btnSelectInputModel = new System.Windows.Forms.Button();
			this.txtInputModel = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnSelectInputAnimation = new System.Windows.Forms.Button();
			this.txtInputAnimation = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtNumFrames = new System.Windows.Forms.TextBox();
			this.lblFrameNum = new System.Windows.Forms.Label();
			this.txtCurrentFrameNum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.btnLastFrame = new System.Windows.Forms.Button();
			this.btnNextFrame = new System.Windows.Forms.Button();
			this.btnPreviousFrame = new System.Windows.Forms.Button();
			this.btnFirstFrame = new System.Windows.Forms.Button();
			this.chkLoopAnimation = new System.Windows.Forms.CheckBox();
			this.btnStopAnimation = new System.Windows.Forms.Button();
			this.btnPlayAnimation = new System.Windows.Forms.Button();
			this.btnOpenBCA = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtBCAName = new System.Windows.Forms.TextBox();
			this.btnOpenBMD = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtBMDName = new System.Windows.Forms.TextBox();
			this.txtModelPreviewScale = new System.Windows.Forms.TextBox();
			this.tsModelPreview = new System.Windows.Forms.ToolStrip();
			this.lblModelPreviewScale = new System.Windows.Forms.ToolStripLabel();
			this.glModelView = new SM64DSe.FormControls.ModelGLControl();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnExportDAE = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.gbImportAnimation.SuspendLayout();
			this.tsModelPreview.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 31);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.gbImportAnimation);
			this.splitContainer1.Panel1.Controls.Add(this.txtNumFrames);
			this.splitContainer1.Panel1.Controls.Add(this.lblFrameNum);
			this.splitContainer1.Panel1.Controls.Add(this.txtCurrentFrameNum);
			this.splitContainer1.Panel1.Controls.Add(this.label9);
			this.splitContainer1.Panel1.Controls.Add(this.btnLastFrame);
			this.splitContainer1.Panel1.Controls.Add(this.btnNextFrame);
			this.splitContainer1.Panel1.Controls.Add(this.btnPreviousFrame);
			this.splitContainer1.Panel1.Controls.Add(this.btnFirstFrame);
			this.splitContainer1.Panel1.Controls.Add(this.chkLoopAnimation);
			this.splitContainer1.Panel1.Controls.Add(this.btnStopAnimation);
			this.splitContainer1.Panel1.Controls.Add(this.btnPlayAnimation);
			this.splitContainer1.Panel1.Controls.Add(this.btnOpenBCA);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.txtBCAName);
			this.splitContainer1.Panel1.Controls.Add(this.btnOpenBMD);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.txtBMDName);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.txtModelPreviewScale);
			this.splitContainer1.Panel2.Controls.Add(this.tsModelPreview);
			this.splitContainer1.Panel2.Controls.Add(this.glModelView);
			this.splitContainer1.Size = new System.Drawing.Size(1059, 530);
			this.splitContainer1.SplitterDistance = 230;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 0;
			// 
			// gbImportAnimation
			// 
			this.gbImportAnimation.Controls.Add(this.chkOptimise);
			this.gbImportAnimation.Controls.Add(this.txtScale);
			this.gbImportAnimation.Controls.Add(this.label5);
			this.gbImportAnimation.Controls.Add(this.btnImportAnimation);
			this.gbImportAnimation.Controls.Add(this.btnSelectInputModel);
			this.gbImportAnimation.Controls.Add(this.txtInputModel);
			this.gbImportAnimation.Controls.Add(this.label4);
			this.gbImportAnimation.Controls.Add(this.btnSelectInputAnimation);
			this.gbImportAnimation.Controls.Add(this.txtInputAnimation);
			this.gbImportAnimation.Controls.Add(this.label3);
			this.gbImportAnimation.Location = new System.Drawing.Point(4, 306);
			this.gbImportAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbImportAnimation.Name = "gbImportAnimation";
			this.gbImportAnimation.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbImportAnimation.Size = new System.Drawing.Size(300, 220);
			this.gbImportAnimation.TabIndex = 19;
			this.gbImportAnimation.TabStop = false;
			this.gbImportAnimation.Text = "Import Animation";
			// 
			// chkOptimise
			// 
			this.chkOptimise.AutoSize = true;
			this.chkOptimise.Checked = true;
			this.chkOptimise.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkOptimise.Location = new System.Drawing.Point(4, 130);
			this.chkOptimise.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.chkOptimise.Name = "chkOptimise";
			this.chkOptimise.Size = new System.Drawing.Size(85, 21);
			this.chkOptimise.TabIndex = 11;
			this.chkOptimise.Text = "Optimise";
			this.chkOptimise.UseVisualStyleBackColor = true;
			this.chkOptimise.CheckedChanged += new System.EventHandler(this.chkOptimise_CheckedChanged);
			// 
			// txtScale
			// 
			this.txtScale.Location = new System.Drawing.Point(112, 154);
			this.txtScale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtScale.Name = "txtScale";
			this.txtScale.Size = new System.Drawing.Size(135, 22);
			this.txtScale.TabIndex = 10;
			this.txtScale.Text = "1";
			this.txtScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(0, 158);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(47, 17);
			this.label5.TabIndex = 9;
			this.label5.Text = "Scale:";
			// 
			// btnImportAnimation
			// 
			this.btnImportAnimation.Location = new System.Drawing.Point(4, 186);
			this.btnImportAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnImportAnimation.Name = "btnImportAnimation";
			this.btnImportAnimation.Size = new System.Drawing.Size(143, 28);
			this.btnImportAnimation.TabIndex = 8;
			this.btnImportAnimation.Text = "Import Animation";
			this.btnImportAnimation.UseVisualStyleBackColor = true;
			this.btnImportAnimation.Click += new System.EventHandler(this.btnImportAnimation_Click);
			// 
			// btnSelectInputModel
			// 
			this.btnSelectInputModel.Location = new System.Drawing.Point(253, 96);
			this.btnSelectInputModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnSelectInputModel.Name = "btnSelectInputModel";
			this.btnSelectInputModel.Size = new System.Drawing.Size(41, 28);
			this.btnSelectInputModel.TabIndex = 7;
			this.btnSelectInputModel.Text = "...";
			this.btnSelectInputModel.UseVisualStyleBackColor = true;
			this.btnSelectInputModel.Click += new System.EventHandler(this.btnSelectInputModel_Click);
			// 
			// txtInputModel
			// 
			this.txtInputModel.Location = new System.Drawing.Point(4, 98);
			this.txtInputModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtInputModel.Name = "txtInputModel";
			this.txtInputModel.Size = new System.Drawing.Size(243, 22);
			this.txtInputModel.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(0, 79);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(176, 17);
			this.label4.TabIndex = 5;
			this.label4.Text = "(Optional) Model to Import:";
			// 
			// btnSelectInputAnimation
			// 
			this.btnSelectInputAnimation.Location = new System.Drawing.Point(253, 48);
			this.btnSelectInputAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnSelectInputAnimation.Name = "btnSelectInputAnimation";
			this.btnSelectInputAnimation.Size = new System.Drawing.Size(41, 28);
			this.btnSelectInputAnimation.TabIndex = 4;
			this.btnSelectInputAnimation.Text = "...";
			this.btnSelectInputAnimation.UseVisualStyleBackColor = true;
			this.btnSelectInputAnimation.Click += new System.EventHandler(this.btnSelectInputAnimation_Click);
			// 
			// txtInputAnimation
			// 
			this.txtInputAnimation.Location = new System.Drawing.Point(4, 50);
			this.txtInputAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtInputAnimation.Name = "txtInputAnimation";
			this.txtInputAnimation.Size = new System.Drawing.Size(243, 22);
			this.txtInputAnimation.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(0, 31);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(205, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Animation File/Animated Model:";
			// 
			// txtNumFrames
			// 
			this.txtNumFrames.Location = new System.Drawing.Point(159, 224);
			this.txtNumFrames.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtNumFrames.Name = "txtNumFrames";
			this.txtNumFrames.ReadOnly = true;
			this.txtNumFrames.Size = new System.Drawing.Size(71, 22);
			this.txtNumFrames.TabIndex = 18;
			// 
			// lblFrameNum
			// 
			this.lblFrameNum.AutoSize = true;
			this.lblFrameNum.Location = new System.Drawing.Point(135, 228);
			this.lblFrameNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblFrameNum.Name = "lblFrameNum";
			this.lblFrameNum.Size = new System.Drawing.Size(12, 17);
			this.lblFrameNum.TabIndex = 17;
			this.lblFrameNum.Text = "/";
			// 
			// txtCurrentFrameNum
			// 
			this.txtCurrentFrameNum.Location = new System.Drawing.Point(55, 224);
			this.txtCurrentFrameNum.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtCurrentFrameNum.Name = "txtCurrentFrameNum";
			this.txtCurrentFrameNum.Size = new System.Drawing.Size(71, 22);
			this.txtCurrentFrameNum.TabIndex = 16;
			this.txtCurrentFrameNum.TextChanged += new System.EventHandler(this.txtCurrentFrameNum_TextChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(3, 228);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(52, 17);
			this.label9.TabIndex = 15;
			this.label9.Text = "Frame:";
			// 
			// btnLastFrame
			// 
			this.btnLastFrame.Location = new System.Drawing.Point(168, 188);
			this.btnLastFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnLastFrame.Name = "btnLastFrame";
			this.btnLastFrame.Size = new System.Drawing.Size(48, 28);
			this.btnLastFrame.TabIndex = 14;
			this.btnLastFrame.Text = "| >";
			this.btnLastFrame.UseVisualStyleBackColor = true;
			this.btnLastFrame.Click += new System.EventHandler(this.btnLastFrame_Click);
			// 
			// btnNextFrame
			// 
			this.btnNextFrame.Location = new System.Drawing.Point(116, 188);
			this.btnNextFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnNextFrame.Name = "btnNextFrame";
			this.btnNextFrame.Size = new System.Drawing.Size(48, 28);
			this.btnNextFrame.TabIndex = 13;
			this.btnNextFrame.Text = ">";
			this.btnNextFrame.UseVisualStyleBackColor = true;
			this.btnNextFrame.Click += new System.EventHandler(this.btnNextFrame_Click);
			// 
			// btnPreviousFrame
			// 
			this.btnPreviousFrame.Location = new System.Drawing.Point(60, 188);
			this.btnPreviousFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnPreviousFrame.Name = "btnPreviousFrame";
			this.btnPreviousFrame.Size = new System.Drawing.Size(48, 28);
			this.btnPreviousFrame.TabIndex = 12;
			this.btnPreviousFrame.Text = "<";
			this.btnPreviousFrame.UseVisualStyleBackColor = true;
			this.btnPreviousFrame.Click += new System.EventHandler(this.btnPreviousFrame_Click);
			// 
			// btnFirstFrame
			// 
			this.btnFirstFrame.Location = new System.Drawing.Point(8, 188);
			this.btnFirstFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnFirstFrame.Name = "btnFirstFrame";
			this.btnFirstFrame.Size = new System.Drawing.Size(48, 28);
			this.btnFirstFrame.TabIndex = 11;
			this.btnFirstFrame.Text = "< |";
			this.btnFirstFrame.UseVisualStyleBackColor = true;
			this.btnFirstFrame.Click += new System.EventHandler(this.btnFirstFrame_Click);
			// 
			// chkLoopAnimation
			// 
			this.chkLoopAnimation.AutoSize = true;
			this.chkLoopAnimation.Checked = true;
			this.chkLoopAnimation.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkLoopAnimation.Location = new System.Drawing.Point(228, 158);
			this.chkLoopAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.chkLoopAnimation.Name = "chkLoopAnimation";
			this.chkLoopAnimation.Size = new System.Drawing.Size(62, 21);
			this.chkLoopAnimation.TabIndex = 9;
			this.chkLoopAnimation.Text = "Loop";
			this.chkLoopAnimation.UseVisualStyleBackColor = true;
			this.chkLoopAnimation.CheckedChanged += new System.EventHandler(this.chkLoopAnimation_CheckedChanged);
			// 
			// btnStopAnimation
			// 
			this.btnStopAnimation.Location = new System.Drawing.Point(135, 153);
			this.btnStopAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnStopAnimation.Name = "btnStopAnimation";
			this.btnStopAnimation.Size = new System.Drawing.Size(81, 28);
			this.btnStopAnimation.TabIndex = 8;
			this.btnStopAnimation.Text = "Stop";
			this.btnStopAnimation.UseVisualStyleBackColor = true;
			this.btnStopAnimation.Click += new System.EventHandler(this.btnStopAnimation_Click);
			// 
			// btnPlayAnimation
			// 
			this.btnPlayAnimation.Location = new System.Drawing.Point(8, 153);
			this.btnPlayAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnPlayAnimation.Name = "btnPlayAnimation";
			this.btnPlayAnimation.Size = new System.Drawing.Size(119, 28);
			this.btnPlayAnimation.TabIndex = 7;
			this.btnPlayAnimation.Text = "Play";
			this.btnPlayAnimation.UseVisualStyleBackColor = true;
			this.btnPlayAnimation.Click += new System.EventHandler(this.btnPlayAnimation_Click);
			// 
			// btnOpenBCA
			// 
			this.btnOpenBCA.Location = new System.Drawing.Point(257, 84);
			this.btnOpenBCA.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnOpenBCA.Name = "btnOpenBCA";
			this.btnOpenBCA.Size = new System.Drawing.Size(41, 28);
			this.btnOpenBCA.TabIndex = 5;
			this.btnOpenBCA.Text = "...";
			this.btnOpenBCA.UseVisualStyleBackColor = true;
			this.btnOpenBCA.Click += new System.EventHandler(this.btnOpenBCA_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 66);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(115, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Animation (BCA):";
			// 
			// txtBCAName
			// 
			this.txtBCAName.Location = new System.Drawing.Point(8, 86);
			this.txtBCAName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtBCAName.Name = "txtBCAName";
			this.txtBCAName.ReadOnly = true;
			this.txtBCAName.Size = new System.Drawing.Size(243, 22);
			this.txtBCAName.TabIndex = 3;
			// 
			// btnOpenBMD
			// 
			this.btnOpenBMD.Location = new System.Drawing.Point(257, 25);
			this.btnOpenBMD.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnOpenBMD.Name = "btnOpenBMD";
			this.btnOpenBMD.Size = new System.Drawing.Size(41, 28);
			this.btnOpenBMD.TabIndex = 2;
			this.btnOpenBMD.Text = "...";
			this.btnOpenBMD.UseVisualStyleBackColor = true;
			this.btnOpenBMD.Click += new System.EventHandler(this.btnOpenBMD_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 7);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "Model (BMD):";
			// 
			// txtBMDName
			// 
			this.txtBMDName.Location = new System.Drawing.Point(8, 27);
			this.txtBMDName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtBMDName.Name = "txtBMDName";
			this.txtBMDName.ReadOnly = true;
			this.txtBMDName.Size = new System.Drawing.Size(243, 22);
			this.txtBMDName.TabIndex = 0;
			// 
			// txtModelPreviewScale
			// 
			this.txtModelPreviewScale.Location = new System.Drawing.Point(128, 4);
			this.txtModelPreviewScale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtModelPreviewScale.Name = "txtModelPreviewScale";
			this.txtModelPreviewScale.Size = new System.Drawing.Size(140, 22);
			this.txtModelPreviewScale.TabIndex = 16;
			this.txtModelPreviewScale.TextChanged += new System.EventHandler(this.txtModelPreviewScale_TextChanged);
			// 
			// tsModelPreview
			// 
			this.tsModelPreview.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tsModelPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblModelPreviewScale});
			this.tsModelPreview.Location = new System.Drawing.Point(0, 0);
			this.tsModelPreview.Name = "tsModelPreview";
			this.tsModelPreview.Size = new System.Drawing.Size(824, 25);
			this.tsModelPreview.TabIndex = 2;
			this.tsModelPreview.Text = "toolStrip1";
			// 
			// lblModelPreviewScale
			// 
			this.lblModelPreviewScale.Name = "lblModelPreviewScale";
			this.lblModelPreviewScale.Size = new System.Drawing.Size(102, 22);
			this.lblModelPreviewScale.Text = "Preview Scale:";
			// 
			// glModelView
			// 
			this.glModelView.BackColor = System.Drawing.Color.Black;
			this.glModelView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glModelView.Location = new System.Drawing.Point(0, 0);
			this.glModelView.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this.glModelView.Name = "glModelView";
			this.glModelView.Size = new System.Drawing.Size(824, 530);
			this.glModelView.TabIndex = 0;
			this.glModelView.VSync = false;
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExportDAE});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1059, 27);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// btnExportDAE
			// 
			this.btnExportDAE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnExportDAE.Image = ((System.Drawing.Image)(resources.GetObject("btnExportDAE.Image")));
			this.btnExportDAE.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnExportDAE.Name = "btnExportDAE";
			this.btnExportDAE.Size = new System.Drawing.Size(89, 24);
			this.btnExportDAE.Text = "Export DAE";
			this.btnExportDAE.ToolTipText = "Export model and animation to COLLADA DAE";
			this.btnExportDAE.Click += new System.EventHandler(this.btnExportToDAE_Click);
			// 
			// AnimationEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1059, 572);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "AnimationEditorForm";
			this.Text = "Model Animation Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AnimationEditorForm_FormClosing);
			this.Load += new System.EventHandler(this.AnimationEditorForm_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.gbImportAnimation.ResumeLayout(false);
			this.gbImportAnimation.PerformLayout();
			this.tsModelPreview.ResumeLayout(false);
			this.tsModelPreview.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private FormControls.ModelGLControl glModelView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnExportDAE;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBMDName;
        private System.Windows.Forms.Button btnOpenBMD;
        private System.Windows.Forms.Button btnOpenBCA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBCAName;
        private System.Windows.Forms.Button btnStopAnimation;
        private System.Windows.Forms.Button btnPlayAnimation;
        private System.Windows.Forms.CheckBox chkLoopAnimation;
        private System.Windows.Forms.Button btnLastFrame;
        private System.Windows.Forms.Button btnNextFrame;
        private System.Windows.Forms.Button btnPreviousFrame;
        private System.Windows.Forms.Button btnFirstFrame;
        private System.Windows.Forms.TextBox txtNumFrames;
        private System.Windows.Forms.Label lblFrameNum;
        private System.Windows.Forms.TextBox txtCurrentFrameNum;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox gbImportAnimation;
        private System.Windows.Forms.Button btnImportAnimation;
        private System.Windows.Forms.Button btnSelectInputModel;
        private System.Windows.Forms.TextBox txtInputModel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSelectInputAnimation;
        private System.Windows.Forms.TextBox txtInputAnimation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtScale;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkOptimise;
        private System.Windows.Forms.ToolStrip tsModelPreview;
        private System.Windows.Forms.ToolStripLabel lblModelPreviewScale;
        private System.Windows.Forms.TextBox txtModelPreviewScale;
    }
}