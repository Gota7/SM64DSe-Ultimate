namespace SM64DSe
{
    partial class LevelDisplaySettingsForm
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
            this.checkTextures = new System.Windows.Forms.CheckBox();
            this.checkVtxColors = new System.Windows.Forms.CheckBox();
            this.checkWireframe = new System.Windows.Forms.CheckBox();
            this.checkPolylistTypes = new System.Windows.Forms.CheckBox();
            this.box_LevelModel = new System.Windows.Forms.GroupBox();
            this.box_3dView = new System.Windows.Forms.GroupBox();
            this.valFarClipping = new System.Windows.Forms.NumericUpDown();
            this.valFOV = new System.Windows.Forms.TrackBar();
            this.lblFarClipping = new System.Windows.Forms.Label();
            this.lblFov = new System.Windows.Forms.Label();
            this.btnBottom = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnToogleOrtho = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnTop = new System.Windows.Forms.Button();
            this.btnFront = new System.Windows.Forms.Button();
            this.box_LevelModel.SuspendLayout();
            this.box_3dView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valFarClipping)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valFOV)).BeginInit();
            this.SuspendLayout();
            // 
            // checkTextures
            // 
            this.checkTextures.AutoSize = true;
            this.checkTextures.Location = new System.Drawing.Point(6, 19);
            this.checkTextures.Name = "checkTextures";
            this.checkTextures.Size = new System.Drawing.Size(97, 17);
            this.checkTextures.TabIndex = 7;
            this.checkTextures.Text = "Show Textures";
            this.checkTextures.UseVisualStyleBackColor = true;
            this.checkTextures.Click += new System.EventHandler(this.checkTextures_Click);
            // 
            // checkVtxColors
            // 
            this.checkVtxColors.AutoSize = true;
            this.checkVtxColors.Location = new System.Drawing.Point(6, 42);
            this.checkVtxColors.Name = "checkVtxColors";
            this.checkVtxColors.Size = new System.Drawing.Size(118, 17);
            this.checkVtxColors.TabIndex = 8;
            this.checkVtxColors.Text = "Show Vertex Colors";
            this.checkVtxColors.UseVisualStyleBackColor = true;
            this.checkVtxColors.Click += new System.EventHandler(this.checkVtxColors_Click);
            // 
            // checkWireframe
            // 
            this.checkWireframe.AutoSize = true;
            this.checkWireframe.Location = new System.Drawing.Point(6, 65);
            this.checkWireframe.Name = "checkWireframe";
            this.checkWireframe.Size = new System.Drawing.Size(104, 17);
            this.checkWireframe.TabIndex = 9;
            this.checkWireframe.Text = "Show Wireframe";
            this.checkWireframe.UseVisualStyleBackColor = true;
            this.checkWireframe.Click += new System.EventHandler(this.checkWireframe_Click);
            // 
            // checkPolylistTypes
            // 
            this.checkPolylistTypes.AutoSize = true;
            this.checkPolylistTypes.Location = new System.Drawing.Point(6, 88);
            this.checkPolylistTypes.Name = "checkPolylistTypes";
            this.checkPolylistTypes.Size = new System.Drawing.Size(197, 17);
            this.checkPolylistTypes.TabIndex = 10;
            this.checkPolylistTypes.Text = "Indicate Draw Call Type (for experts)";
            this.checkPolylistTypes.UseVisualStyleBackColor = true;
            this.checkPolylistTypes.Click += new System.EventHandler(this.checkPolylistTypes_Click);
            // 
            // box_LevelModel
            // 
            this.box_LevelModel.Controls.Add(this.checkTextures);
            this.box_LevelModel.Controls.Add(this.checkVtxColors);
            this.box_LevelModel.Controls.Add(this.checkWireframe);
            this.box_LevelModel.Controls.Add(this.checkPolylistTypes);
            this.box_LevelModel.Location = new System.Drawing.Point(14, 28);
            this.box_LevelModel.Name = "box_LevelModel";
            this.box_LevelModel.Size = new System.Drawing.Size(218, 112);
            this.box_LevelModel.TabIndex = 11;
            this.box_LevelModel.TabStop = false;
            this.box_LevelModel.Text = "Level Model";
            // 
            // box_3dView
            // 
            this.box_3dView.Controls.Add(this.valFarClipping);
            this.box_3dView.Controls.Add(this.valFOV);
            this.box_3dView.Controls.Add(this.lblFarClipping);
            this.box_3dView.Controls.Add(this.lblFov);
            this.box_3dView.Controls.Add(this.btnBottom);
            this.box_3dView.Controls.Add(this.btnRight);
            this.box_3dView.Controls.Add(this.btnToogleOrtho);
            this.box_3dView.Controls.Add(this.btnLeft);
            this.box_3dView.Controls.Add(this.btnBack);
            this.box_3dView.Controls.Add(this.btnTop);
            this.box_3dView.Controls.Add(this.btnFront);
            this.box_3dView.Location = new System.Drawing.Point(14, 146);
            this.box_3dView.Name = "box_3dView";
            this.box_3dView.Size = new System.Drawing.Size(218, 218);
            this.box_3dView.TabIndex = 12;
            this.box_3dView.TabStop = false;
            this.box_3dView.Text = "3d View";
            // 
            // valFarClipping
            // 
            this.valFarClipping.Location = new System.Drawing.Point(83, 152);
            this.valFarClipping.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.valFarClipping.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.valFarClipping.Name = "valFarClipping";
            this.valFarClipping.Size = new System.Drawing.Size(120, 20);
            this.valFarClipping.TabIndex = 11;
            this.valFarClipping.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.valFarClipping.ValueChanged += new System.EventHandler(this.valFarClipping_ValueChanged);
            // 
            // valFOV
            // 
            this.valFOV.LargeChange = 15;
            this.valFOV.Location = new System.Drawing.Point(43, 101);
            this.valFOV.Maximum = 120;
            this.valFOV.Minimum = 5;
            this.valFOV.Name = "valFOV";
            this.valFOV.Size = new System.Drawing.Size(157, 45);
            this.valFOV.SmallChange = 5;
            this.valFOV.TabIndex = 10;
            this.valFOV.Value = 5;
            this.valFOV.ValueChanged += new System.EventHandler(this.valFOV_ValueChanged);
            // 
            // lblFarClipping
            // 
            this.lblFarClipping.AutoSize = true;
            this.lblFarClipping.Location = new System.Drawing.Point(9, 154);
            this.lblFarClipping.Name = "lblFarClipping";
            this.lblFarClipping.Size = new System.Drawing.Size(62, 13);
            this.lblFarClipping.TabIndex = 9;
            this.lblFarClipping.Text = "far Clipping:";
            // 
            // lblFov
            // 
            this.lblFov.AutoSize = true;
            this.lblFov.Location = new System.Drawing.Point(6, 106);
            this.lblFov.Name = "lblFov";
            this.lblFov.Size = new System.Drawing.Size(31, 13);
            this.lblFov.TabIndex = 8;
            this.lblFov.Text = "FOV:";
            // 
            // btnBottom
            // 
            this.btnBottom.Location = new System.Drawing.Point(77, 77);
            this.btnBottom.Name = "btnBottom";
            this.btnBottom.Size = new System.Drawing.Size(65, 23);
            this.btnBottom.TabIndex = 7;
            this.btnBottom.Text = "Bottom";
            this.btnBottom.UseVisualStyleBackColor = true;
            this.btnBottom.Click += new System.EventHandler(this.PerspectiveBtn_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(147, 48);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(65, 23);
            this.btnRight.TabIndex = 5;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.PerspectiveBtn_Click);
            // 
            // btnToogleOrtho
            // 
            this.btnToogleOrtho.Location = new System.Drawing.Point(77, 48);
            this.btnToogleOrtho.Name = "btnToogleOrtho";
            this.btnToogleOrtho.Size = new System.Drawing.Size(65, 23);
            this.btnToogleOrtho.TabIndex = 4;
            this.btnToogleOrtho.Text = "Ortho";
            this.btnToogleOrtho.UseVisualStyleBackColor = true;
            this.btnToogleOrtho.Click += new System.EventHandler(this.btnToogleOrtho_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(6, 48);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(65, 23);
            this.btnLeft.TabIndex = 3;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.PerspectiveBtn_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(147, 19);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(65, 23);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.PerspectiveBtn_Click);
            // 
            // btnTop
            // 
            this.btnTop.Location = new System.Drawing.Point(77, 19);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(65, 23);
            this.btnTop.TabIndex = 1;
            this.btnTop.Text = "Top";
            this.btnTop.UseVisualStyleBackColor = true;
            this.btnTop.Click += new System.EventHandler(this.PerspectiveBtn_Click);
            // 
            // btnFront
            // 
            this.btnFront.Location = new System.Drawing.Point(6, 19);
            this.btnFront.Name = "btnFront";
            this.btnFront.Size = new System.Drawing.Size(65, 23);
            this.btnFront.TabIndex = 0;
            this.btnFront.Text = "Front";
            this.btnFront.UseVisualStyleBackColor = true;
            this.btnFront.Click += new System.EventHandler(this.PerspectiveBtn_Click);
            // 
            // LevelDisplaySettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 376);
            this.Controls.Add(this.box_3dView);
            this.Controls.Add(this.box_LevelModel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LevelDisplaySettingsForm";
            this.Text = "Level Display Settings";
            this.box_LevelModel.ResumeLayout(false);
            this.box_LevelModel.PerformLayout();
            this.box_3dView.ResumeLayout(false);
            this.box_3dView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valFarClipping)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valFOV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox checkTextures;
        private System.Windows.Forms.CheckBox checkVtxColors;
        private System.Windows.Forms.CheckBox checkWireframe;
        private System.Windows.Forms.CheckBox checkPolylistTypes;
        private System.Windows.Forms.GroupBox box_LevelModel;
        private System.Windows.Forms.GroupBox box_3dView;
        private System.Windows.Forms.Button btnBottom;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnToogleOrtho;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnTop;
        private System.Windows.Forms.Button btnFront;
        private System.Windows.Forms.NumericUpDown valFarClipping;
        private System.Windows.Forms.Label lblFarClipping;
        private System.Windows.Forms.Label lblFov;
        private System.Windows.Forms.TrackBar valFOV;
    }
}