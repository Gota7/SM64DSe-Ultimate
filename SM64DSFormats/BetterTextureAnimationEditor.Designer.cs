namespace SM64DSe.SM64DSFormats
{
    partial class BetterTextureAnimationEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BetterTextureAnimationEditor));
            this.textureView = new System.Windows.Forms.Panel();
            this.valueSettingPanel2 = new System.Windows.Forms.Panel();
            this.lblScaling = new System.Windows.Forms.Label();
            this.numScaling = new System.Windows.Forms.NumericUpDown();
            this.lblInterpolation = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnCancelKeyFrames = new System.Windows.Forms.Button();
            this.lblKeyframeVal = new System.Windows.Forms.Label();
            this.numKeyframeValue = new System.Windows.Forms.NumericUpDown();
            this.btnGenerateAnim = new System.Windows.Forms.Button();
            this.lblFrame = new System.Windows.Forms.Label();
            this.btnDeleteKeyframe = new System.Windows.Forms.Button();
            this.numKeyframe = new System.Windows.Forms.NumericUpDown();
            this.cbSelectInterpolation = new System.Windows.Forms.ComboBox();
            this.valueSettingPanel1 = new System.Windows.Forms.Panel();
            this.lblProperty = new System.Windows.Forms.Label();
            this.btnSetKeyframes = new System.Windows.Forms.Button();
            this.lblValue = new System.Windows.Forms.Label();
            this.checkSetAll = new System.Windows.Forms.CheckBox();
            this.btnCreateNew = new System.Windows.Forms.Button();
            this.btnSaveAnim = new System.Windows.Forms.Button();
            this.btnDeleteAnim = new System.Windows.Forms.Button();
            this.numValue = new System.Windows.Forms.NumericUpDown();
            this.cbSelectProperty = new System.Windows.Forms.ComboBox();
            this.tvMaterials = new System.Windows.Forms.TreeView();
            this.timelinePanel = new System.Windows.Forms.Panel();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStepLeft = new System.Windows.Forms.Button();
            this.btnStepRight = new System.Windows.Forms.Button();
            this.btnPrevPart = new System.Windows.Forms.Button();
            this.btnNextPart = new System.Windows.Forms.Button();
            this.btnCreateAnimData = new System.Windows.Forms.Button();
            this.btnDeleteAnimData = new System.Windows.Forms.Button();
            this.textureView.SuspendLayout();
            this.valueSettingPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScaling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyframeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyframe)).BeginInit();
            this.valueSettingPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numValue)).BeginInit();
            this.SuspendLayout();
            // 
            // textureView
            // 
            this.textureView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureView.Controls.Add(this.valueSettingPanel2);
            this.textureView.Controls.Add(this.valueSettingPanel1);
            this.textureView.Location = new System.Drawing.Point(191, 12);
            this.textureView.Name = "textureView";
            this.textureView.Size = new System.Drawing.Size(592, 350);
            this.textureView.TabIndex = 0;
            this.textureView.Paint += new System.Windows.Forms.PaintEventHandler(this.textureView_Paint);
            // 
            // valueSettingPanel2
            // 
            this.valueSettingPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.valueSettingPanel2.BackColor = System.Drawing.SystemColors.Control;
            this.valueSettingPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueSettingPanel2.Controls.Add(this.lblScaling);
            this.valueSettingPanel2.Controls.Add(this.numScaling);
            this.valueSettingPanel2.Controls.Add(this.lblInterpolation);
            this.valueSettingPanel2.Controls.Add(this.btnReset);
            this.valueSettingPanel2.Controls.Add(this.btnCancelKeyFrames);
            this.valueSettingPanel2.Controls.Add(this.lblKeyframeVal);
            this.valueSettingPanel2.Controls.Add(this.numKeyframeValue);
            this.valueSettingPanel2.Controls.Add(this.btnGenerateAnim);
            this.valueSettingPanel2.Controls.Add(this.lblFrame);
            this.valueSettingPanel2.Controls.Add(this.btnDeleteKeyframe);
            this.valueSettingPanel2.Controls.Add(this.numKeyframe);
            this.valueSettingPanel2.Controls.Add(this.cbSelectInterpolation);
            this.valueSettingPanel2.Location = new System.Drawing.Point(306, 106);
            this.valueSettingPanel2.Name = "valueSettingPanel2";
            this.valueSettingPanel2.Size = new System.Drawing.Size(140, 244);
            this.valueSettingPanel2.TabIndex = 8;
            this.valueSettingPanel2.Visible = false;
            // 
            // lblScaling
            // 
            this.lblScaling.AutoSize = true;
            this.lblScaling.Location = new System.Drawing.Point(4, 94);
            this.lblScaling.Name = "lblScaling";
            this.lblScaling.Size = new System.Drawing.Size(45, 13);
            this.lblScaling.TabIndex = 13;
            this.lblScaling.Text = "Scaling:";
            // 
            // numScaling
            // 
            this.numScaling.Cursor = System.Windows.Forms.Cursors.Default;
            this.numScaling.Location = new System.Drawing.Point(55, 90);
            this.numScaling.Maximum = new decimal(new int[] {
            32000,
            0,
            0,
            0});
            this.numScaling.Minimum = new decimal(new int[] {
            32000,
            0,
            0,
            -2147483648});
            this.numScaling.Name = "numScaling";
            this.numScaling.Size = new System.Drawing.Size(79, 20);
            this.numScaling.TabIndex = 12;
            this.numScaling.Tag = "0";
            this.numScaling.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numScaling.ValueChanged += new System.EventHandler(this.numScaling_ValueChanged);
            // 
            // lblInterpolation
            // 
            this.lblInterpolation.AutoSize = true;
            this.lblInterpolation.Location = new System.Drawing.Point(3, 16);
            this.lblInterpolation.Name = "lblInterpolation";
            this.lblInterpolation.Size = new System.Drawing.Size(65, 13);
            this.lblInterpolation.TabIndex = 11;
            this.lblInterpolation.Text = "Interpolation";
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(2, 187);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(131, 23);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnCancelKeyFrames
            // 
            this.btnCancelKeyFrames.Enabled = false;
            this.btnCancelKeyFrames.Location = new System.Drawing.Point(2, 216);
            this.btnCancelKeyFrames.Name = "btnCancelKeyFrames";
            this.btnCancelKeyFrames.Size = new System.Drawing.Size(131, 23);
            this.btnCancelKeyFrames.TabIndex = 10;
            this.btnCancelKeyFrames.Text = "Cancel";
            this.btnCancelKeyFrames.UseVisualStyleBackColor = true;
            this.btnCancelKeyFrames.Click += new System.EventHandler(this.btnCancelKeyFrames_Click);
            // 
            // lblKeyframeVal
            // 
            this.lblKeyframeVal.AutoSize = true;
            this.lblKeyframeVal.Location = new System.Drawing.Point(3, 63);
            this.lblKeyframeVal.Name = "lblKeyframeVal";
            this.lblKeyframeVal.Size = new System.Drawing.Size(37, 13);
            this.lblKeyframeVal.TabIndex = 9;
            this.lblKeyframeVal.Text = "Value:";
            // 
            // numKeyframeValue
            // 
            this.numKeyframeValue.DecimalPlaces = 7;
            this.numKeyframeValue.Enabled = false;
            this.numKeyframeValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numKeyframeValue.Location = new System.Drawing.Point(42, 61);
            this.numKeyframeValue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numKeyframeValue.Name = "numKeyframeValue";
            this.numKeyframeValue.Size = new System.Drawing.Size(91, 20);
            this.numKeyframeValue.TabIndex = 8;
            this.numKeyframeValue.ValueChanged += new System.EventHandler(this.numKeyframeValue_ValueChanged);
            // 
            // btnGenerateAnim
            // 
            this.btnGenerateAnim.Enabled = false;
            this.btnGenerateAnim.Location = new System.Drawing.Point(3, 145);
            this.btnGenerateAnim.Name = "btnGenerateAnim";
            this.btnGenerateAnim.Size = new System.Drawing.Size(131, 23);
            this.btnGenerateAnim.TabIndex = 7;
            this.btnGenerateAnim.Text = "Generate Animation";
            this.btnGenerateAnim.UseVisualStyleBackColor = true;
            this.btnGenerateAnim.Click += new System.EventHandler(this.btnGenerateAnim_Click);
            // 
            // lblFrame
            // 
            this.lblFrame.AutoSize = true;
            this.lblFrame.Location = new System.Drawing.Point(3, 171);
            this.lblFrame.Name = "lblFrame";
            this.lblFrame.Size = new System.Drawing.Size(39, 13);
            this.lblFrame.TabIndex = 5;
            this.lblFrame.Text = "Frame:";
            this.lblFrame.Visible = false;
            // 
            // btnDeleteKeyframe
            // 
            this.btnDeleteKeyframe.Enabled = false;
            this.btnDeleteKeyframe.Location = new System.Drawing.Point(3, 116);
            this.btnDeleteKeyframe.Name = "btnDeleteKeyframe";
            this.btnDeleteKeyframe.Size = new System.Drawing.Size(131, 23);
            this.btnDeleteKeyframe.TabIndex = 3;
            this.btnDeleteKeyframe.Text = "Delete Keyframe";
            this.btnDeleteKeyframe.UseVisualStyleBackColor = true;
            this.btnDeleteKeyframe.Click += new System.EventHandler(this.btnDeleteKeyframe_Click);
            // 
            // numKeyframe
            // 
            this.numKeyframe.Cursor = System.Windows.Forms.Cursors.Default;
            this.numKeyframe.Enabled = false;
            this.numKeyframe.Location = new System.Drawing.Point(42, 169);
            this.numKeyframe.Name = "numKeyframe";
            this.numKeyframe.Size = new System.Drawing.Size(91, 20);
            this.numKeyframe.TabIndex = 1;
            this.numKeyframe.Visible = false;
            // 
            // cbSelectInterpolation
            // 
            this.cbSelectInterpolation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelectInterpolation.FormattingEnabled = true;
            this.cbSelectInterpolation.Items.AddRange(new object[] {
            "Linear",
            "None"});
            this.cbSelectInterpolation.Location = new System.Drawing.Point(2, 32);
            this.cbSelectInterpolation.Name = "cbSelectInterpolation";
            this.cbSelectInterpolation.Size = new System.Drawing.Size(131, 21);
            this.cbSelectInterpolation.TabIndex = 0;
            this.cbSelectInterpolation.SelectedIndexChanged += new System.EventHandler(this.cbSelectInterpolation_SelectedIndexChanged);
            // 
            // valueSettingPanel1
            // 
            this.valueSettingPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.valueSettingPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.valueSettingPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueSettingPanel1.Controls.Add(this.lblProperty);
            this.valueSettingPanel1.Controls.Add(this.btnSetKeyframes);
            this.valueSettingPanel1.Controls.Add(this.lblValue);
            this.valueSettingPanel1.Controls.Add(this.checkSetAll);
            this.valueSettingPanel1.Controls.Add(this.btnCreateNew);
            this.valueSettingPanel1.Controls.Add(this.btnSaveAnim);
            this.valueSettingPanel1.Controls.Add(this.btnDeleteAnim);
            this.valueSettingPanel1.Controls.Add(this.numValue);
            this.valueSettingPanel1.Controls.Add(this.cbSelectProperty);
            this.valueSettingPanel1.Location = new System.Drawing.Point(452, 106);
            this.valueSettingPanel1.Name = "valueSettingPanel1";
            this.valueSettingPanel1.Size = new System.Drawing.Size(140, 244);
            this.valueSettingPanel1.TabIndex = 0;
            // 
            // lblProperty
            // 
            this.lblProperty.AutoSize = true;
            this.lblProperty.Location = new System.Drawing.Point(3, 16);
            this.lblProperty.Name = "lblProperty";
            this.lblProperty.Size = new System.Drawing.Size(95, 13);
            this.lblProperty.TabIndex = 12;
            this.lblProperty.Text = "Animation Property";
            // 
            // btnSetKeyframes
            // 
            this.btnSetKeyframes.Enabled = false;
            this.btnSetKeyframes.Location = new System.Drawing.Point(2, 147);
            this.btnSetKeyframes.Name = "btnSetKeyframes";
            this.btnSetKeyframes.Size = new System.Drawing.Size(131, 23);
            this.btnSetKeyframes.TabIndex = 7;
            this.btnSetKeyframes.Text = "Set Keyframes";
            this.btnSetKeyframes.UseVisualStyleBackColor = true;
            this.btnSetKeyframes.Click += new System.EventHandler(this.btnSetKeyframes_Click);
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(3, 94);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(37, 13);
            this.lblValue.TabIndex = 5;
            this.lblValue.Text = "Value:";
            // 
            // checkSetAll
            // 
            this.checkSetAll.AutoSize = true;
            this.checkSetAll.Location = new System.Drawing.Point(6, 62);
            this.checkSetAll.Name = "checkSetAll";
            this.checkSetAll.Size = new System.Drawing.Size(91, 17);
            this.checkSetAll.TabIndex = 4;
            this.checkSetAll.Text = "Set All Values";
            this.checkSetAll.UseVisualStyleBackColor = true;
            // 
            // btnCreateNew
            // 
            this.btnCreateNew.Enabled = false;
            this.btnCreateNew.Location = new System.Drawing.Point(2, 118);
            this.btnCreateNew.Name = "btnCreateNew";
            this.btnCreateNew.Size = new System.Drawing.Size(131, 23);
            this.btnCreateNew.TabIndex = 3;
            this.btnCreateNew.Text = "Create New Animation";
            this.btnCreateNew.UseVisualStyleBackColor = true;
            this.btnCreateNew.Click += new System.EventHandler(this.btnCreateNew_Click);
            // 
            // btnSaveAnim
            // 
            this.btnSaveAnim.Enabled = false;
            this.btnSaveAnim.Location = new System.Drawing.Point(2, 216);
            this.btnSaveAnim.Name = "btnSaveAnim";
            this.btnSaveAnim.Size = new System.Drawing.Size(131, 23);
            this.btnSaveAnim.TabIndex = 2;
            this.btnSaveAnim.Text = "Save Animation";
            this.btnSaveAnim.UseVisualStyleBackColor = true;
            this.btnSaveAnim.Click += new System.EventHandler(this.btnSaveAnim_Click);
            // 
            // btnDeleteAnim
            // 
            this.btnDeleteAnim.Enabled = false;
            this.btnDeleteAnim.Location = new System.Drawing.Point(3, 187);
            this.btnDeleteAnim.Name = "btnDeleteAnim";
            this.btnDeleteAnim.Size = new System.Drawing.Size(131, 23);
            this.btnDeleteAnim.TabIndex = 2;
            this.btnDeleteAnim.Text = "Delete Animation";
            this.btnDeleteAnim.UseVisualStyleBackColor = true;
            this.btnDeleteAnim.Click += new System.EventHandler(this.btnDeleteAnim_Click);
            // 
            // numValue
            // 
            this.numValue.DecimalPlaces = 7;
            this.numValue.Enabled = false;
            this.numValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numValue.Location = new System.Drawing.Point(42, 92);
            this.numValue.Maximum = new decimal(new int[] {
            32000,
            0,
            0,
            0});
            this.numValue.Minimum = new decimal(new int[] {
            32000,
            0,
            0,
            -2147483648});
            this.numValue.Name = "numValue";
            this.numValue.Size = new System.Drawing.Size(91, 20);
            this.numValue.TabIndex = 1;
            this.numValue.ValueChanged += new System.EventHandler(this.numValue_ValueChanged);
            // 
            // cbSelectProperty
            // 
            this.cbSelectProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelectProperty.Enabled = false;
            this.cbSelectProperty.FormattingEnabled = true;
            this.cbSelectProperty.Items.AddRange(new object[] {
            "Translation X",
            "Translation Y",
            "Rotation",
            "Scale"});
            this.cbSelectProperty.Location = new System.Drawing.Point(2, 32);
            this.cbSelectProperty.Name = "cbSelectProperty";
            this.cbSelectProperty.Size = new System.Drawing.Size(131, 21);
            this.cbSelectProperty.TabIndex = 0;
            this.cbSelectProperty.SelectedIndexChanged += new System.EventHandler(this.cbSelectProperty_SelectedIndexChanged);
            // 
            // tvMaterials
            // 
            this.tvMaterials.Location = new System.Drawing.Point(12, 12);
            this.tvMaterials.Name = "tvMaterials";
            this.tvMaterials.Size = new System.Drawing.Size(173, 413);
            this.tvMaterials.TabIndex = 1;
            this.tvMaterials.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvMaterials_AfterSelect);
            // 
            // timelinePanel
            // 
            this.timelinePanel.Location = new System.Drawing.Point(221, 393);
            this.timelinePanel.Name = "timelinePanel";
            this.timelinePanel.Size = new System.Drawing.Size(532, 88);
            this.timelinePanel.TabIndex = 2;
            this.timelinePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.timelinePanel_Paint);
            this.timelinePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.timelinePanel_MouseDown);
            this.timelinePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.timelinePanel_MouseMove);
            // 
            // btnPlay
            // 
            this.btnPlay.Image = global::SM64DSe.Properties.Resources.Play;
            this.btnPlay.Location = new System.Drawing.Point(221, 368);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(24, 23);
            this.btnPlay.TabIndex = 3;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::SM64DSe.Properties.Resources.Stop;
            this.btnStop.Location = new System.Drawing.Point(251, 368);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(24, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStepLeft
            // 
            this.btnStepLeft.Image = global::SM64DSe.Properties.Resources.stepLeft;
            this.btnStepLeft.Location = new System.Drawing.Point(299, 368);
            this.btnStepLeft.Name = "btnStepLeft";
            this.btnStepLeft.Size = new System.Drawing.Size(24, 23);
            this.btnStepLeft.TabIndex = 5;
            this.btnStepLeft.UseVisualStyleBackColor = true;
            this.btnStepLeft.Click += new System.EventHandler(this.btnStepLeft_Click);
            // 
            // btnStepRight
            // 
            this.btnStepRight.Image = global::SM64DSe.Properties.Resources.stepRight;
            this.btnStepRight.Location = new System.Drawing.Point(329, 368);
            this.btnStepRight.Name = "btnStepRight";
            this.btnStepRight.Size = new System.Drawing.Size(24, 23);
            this.btnStepRight.TabIndex = 6;
            this.btnStepRight.UseVisualStyleBackColor = true;
            this.btnStepRight.Click += new System.EventHandler(this.btnStepRight_Click);
            // 
            // btnPrevPart
            // 
            this.btnPrevPart.Image = global::SM64DSe.Properties.Resources.stepLeft;
            this.btnPrevPart.Location = new System.Drawing.Point(191, 393);
            this.btnPrevPart.Name = "btnPrevPart";
            this.btnPrevPart.Size = new System.Drawing.Size(24, 88);
            this.btnPrevPart.TabIndex = 7;
            this.btnPrevPart.UseVisualStyleBackColor = true;
            this.btnPrevPart.Click += new System.EventHandler(this.btnPrevPart_Click);
            // 
            // btnNextPart
            // 
            this.btnNextPart.Image = global::SM64DSe.Properties.Resources.stepRight;
            this.btnNextPart.Location = new System.Drawing.Point(759, 393);
            this.btnNextPart.Name = "btnNextPart";
            this.btnNextPart.Size = new System.Drawing.Size(24, 88);
            this.btnNextPart.TabIndex = 8;
            this.btnNextPart.UseVisualStyleBackColor = true;
            this.btnNextPart.Click += new System.EventHandler(this.btnNextPart_Click);
            // 
            // btnCreateAnimData
            // 
            this.btnCreateAnimData.Enabled = false;
            this.btnCreateAnimData.Location = new System.Drawing.Point(12, 431);
            this.btnCreateAnimData.Name = "btnCreateAnimData";
            this.btnCreateAnimData.Size = new System.Drawing.Size(173, 22);
            this.btnCreateAnimData.TabIndex = 9;
            this.btnCreateAnimData.Text = "Create AnimationData for Area";
            this.btnCreateAnimData.UseVisualStyleBackColor = true;
            this.btnCreateAnimData.Click += new System.EventHandler(this.btnCreateAnimData_Click);
            // 
            // btnDeleteAnimData
            // 
            this.btnDeleteAnimData.Enabled = false;
            this.btnDeleteAnimData.Location = new System.Drawing.Point(12, 459);
            this.btnDeleteAnimData.Name = "btnDeleteAnimData";
            this.btnDeleteAnimData.Size = new System.Drawing.Size(173, 22);
            this.btnDeleteAnimData.TabIndex = 10;
            this.btnDeleteAnimData.Text = "Delete AnimationData for Area";
            this.btnDeleteAnimData.UseVisualStyleBackColor = true;
            this.btnDeleteAnimData.Click += new System.EventHandler(this.btnDeleteAnimData_Click);
            // 
            // BetterTextureAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 493);
            this.Controls.Add(this.btnDeleteAnimData);
            this.Controls.Add(this.btnCreateAnimData);
            this.Controls.Add(this.btnNextPart);
            this.Controls.Add(this.btnPrevPart);
            this.Controls.Add(this.btnStepRight);
            this.Controls.Add(this.timelinePanel);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnStepLeft);
            this.Controls.Add(this.tvMaterials);
            this.Controls.Add(this.textureView);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BetterTextureAnimationEditor";
            this.Text = "BetterTextureAnimationEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BetterTextureAnimationEditor_FormClosing);
            this.textureView.ResumeLayout(false);
            this.valueSettingPanel2.ResumeLayout(false);
            this.valueSettingPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScaling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyframeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyframe)).EndInit();
            this.valueSettingPanel1.ResumeLayout(false);
            this.valueSettingPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel textureView;
        private System.Windows.Forms.TreeView tvMaterials;
        private System.Windows.Forms.Panel valueSettingPanel1;
        private System.Windows.Forms.Panel timelinePanel;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStepLeft;
        private System.Windows.Forms.Button btnStepRight;
        private System.Windows.Forms.ComboBox cbSelectProperty;
        private System.Windows.Forms.Button btnPrevPart;
        private System.Windows.Forms.Button btnNextPart;
        private System.Windows.Forms.Button btnSaveAnim;
        private System.Windows.Forms.NumericUpDown numValue;
        private System.Windows.Forms.Button btnCreateNew;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.CheckBox checkSetAll;
        private System.Windows.Forms.Button btnSetKeyframes;
        private System.Windows.Forms.Panel valueSettingPanel2;
        private System.Windows.Forms.Button btnGenerateAnim;
        private System.Windows.Forms.Label lblFrame;
        private System.Windows.Forms.Button btnDeleteAnim;
        private System.Windows.Forms.NumericUpDown numKeyframe;
        private System.Windows.Forms.ComboBox cbSelectInterpolation;
        private System.Windows.Forms.Label lblKeyframeVal;
        private System.Windows.Forms.NumericUpDown numKeyframeValue;
        private System.Windows.Forms.Button btnDeleteKeyframe;
        private System.Windows.Forms.Label lblInterpolation;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnCancelKeyFrames;
        private System.Windows.Forms.Label lblProperty;
        private System.Windows.Forms.Label lblScaling;
        private System.Windows.Forms.NumericUpDown numScaling;
        private System.Windows.Forms.Button btnCreateAnimData;
        private System.Windows.Forms.Button btnDeleteAnimData;
    }
}