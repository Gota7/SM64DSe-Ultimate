namespace SM64DSe {
    partial class CoinFormationForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.formationType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.distance = new System.Windows.Forms.NumericUpDown();
            this.numCoins = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.horizontalRotation = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.verticalRotation = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.ok = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.val_posZ = new System.Windows.Forms.NumericUpDown();
            this.val_posY = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.lbl_posY = new System.Windows.Forms.Label();
            this.val_posX = new System.Windows.Forms.NumericUpDown();
            this.lbl_posX = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.distance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCoins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalRotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalRotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posX)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Formation Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // formationType
            // 
            this.formationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formationType.FormattingEnabled = true;
            this.formationType.Items.AddRange(new object[] {
            "Line",
            "Ring"});
            this.formationType.Location = new System.Drawing.Point(12, 25);
            this.formationType.Name = "formationType";
            this.formationType.Size = new System.Drawing.Size(189, 21);
            this.formationType.TabIndex = 2;
            this.formationType.SelectedIndexChanged += new System.EventHandler(this.formationType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Coin Distance / Radius:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // distance
            // 
            this.distance.DecimalPlaces = 3;
            this.distance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.distance.Location = new System.Drawing.Point(12, 161);
            this.distance.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.distance.Name = "distance";
            this.distance.Size = new System.Drawing.Size(189, 20);
            this.distance.TabIndex = 4;
            this.distance.Value = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.distance.ValueChanged += new System.EventHandler(this.distance_ValueChanged);
            // 
            // numCoins
            // 
            this.numCoins.Location = new System.Drawing.Point(12, 116);
            this.numCoins.Name = "numCoins";
            this.numCoins.Size = new System.Drawing.Size(189, 20);
            this.numCoins.TabIndex = 6;
            this.numCoins.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numCoins.ValueChanged += new System.EventHandler(this.numCoins_ValueChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(189, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "Number of Coins:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // horizontalRotation
            // 
            this.horizontalRotation.DecimalPlaces = 3;
            this.horizontalRotation.Increment = new decimal(new int[] {
            225,
            0,
            0,
            65536});
            this.horizontalRotation.Location = new System.Drawing.Point(12, 206);
            this.horizontalRotation.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.horizontalRotation.Name = "horizontalRotation";
            this.horizontalRotation.Size = new System.Drawing.Size(189, 20);
            this.horizontalRotation.TabIndex = 8;
            this.horizontalRotation.ValueChanged += new System.EventHandler(this.horizontalRotation_ValueChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(189, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "Horizontal Rotation:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // verticalRotation
            // 
            this.verticalRotation.DecimalPlaces = 3;
            this.verticalRotation.Increment = new decimal(new int[] {
            225,
            0,
            0,
            65536});
            this.verticalRotation.Location = new System.Drawing.Point(12, 251);
            this.verticalRotation.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.verticalRotation.Name = "verticalRotation";
            this.verticalRotation.Size = new System.Drawing.Size(189, 20);
            this.verticalRotation.TabIndex = 10;
            this.verticalRotation.ValueChanged += new System.EventHandler(this.verticalRotation_ValueChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 229);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(189, 19);
            this.label5.TabIndex = 9;
            this.label5.Text = "Vertical Rotation:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(12, 277);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(189, 23);
            this.ok.TabIndex = 11;
            this.ok.Text = "Make Coins!";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(189, 19);
            this.label6.TabIndex = 12;
            this.label6.Text = "Position:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // val_posZ
            // 
            this.val_posZ.DecimalPlaces = 3;
            this.val_posZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.val_posZ.Location = new System.Drawing.Point(157, 71);
            this.val_posZ.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_posZ.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.val_posZ.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            -2147483648});
            this.val_posZ.Name = "val_posZ";
            this.val_posZ.Size = new System.Drawing.Size(50, 20);
            this.val_posZ.TabIndex = 18;
            this.val_posZ.ValueChanged += new System.EventHandler(this.val_posZ_ValueChanged);
            // 
            // val_posY
            // 
            this.val_posY.DecimalPlaces = 3;
            this.val_posY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.val_posY.Location = new System.Drawing.Point(85, 71);
            this.val_posY.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_posY.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.val_posY.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            -2147483648});
            this.val_posY.Name = "val_posY";
            this.val_posY.Size = new System.Drawing.Size(50, 20);
            this.val_posY.TabIndex = 16;
            this.val_posY.ValueChanged += new System.EventHandler(this.val_posY_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(142, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Z:";
            // 
            // lbl_posY
            // 
            this.lbl_posY.AutoSize = true;
            this.lbl_posY.Location = new System.Drawing.Point(70, 73);
            this.lbl_posY.Name = "lbl_posY";
            this.lbl_posY.Size = new System.Drawing.Size(17, 13);
            this.lbl_posY.TabIndex = 14;
            this.lbl_posY.Text = "Y:";
            // 
            // val_posX
            // 
            this.val_posX.DecimalPlaces = 3;
            this.val_posX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.val_posX.Location = new System.Drawing.Point(17, 71);
            this.val_posX.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_posX.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.val_posX.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            -2147483648});
            this.val_posX.Name = "val_posX";
            this.val_posX.Size = new System.Drawing.Size(50, 20);
            this.val_posX.TabIndex = 15;
            this.val_posX.ValueChanged += new System.EventHandler(this.val_posX_ValueChanged);
            // 
            // lbl_posX
            // 
            this.lbl_posX.AutoSize = true;
            this.lbl_posX.Location = new System.Drawing.Point(3, 73);
            this.lbl_posX.Name = "lbl_posX";
            this.lbl_posX.Size = new System.Drawing.Size(17, 13);
            this.lbl_posX.TabIndex = 19;
            this.lbl_posX.Text = "X:";
            // 
            // CoinFormationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 306);
            this.Controls.Add(this.val_posX);
            this.Controls.Add(this.lbl_posX);
            this.Controls.Add(this.val_posZ);
            this.Controls.Add(this.val_posY);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbl_posY);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.verticalRotation);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.horizontalRotation);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numCoins);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.distance);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.formationType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CoinFormationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Coin Formation";
            ((System.ComponentModel.ISupportInitialize)(this.distance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCoins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalRotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalRotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox formationType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown distance;
        private System.Windows.Forms.NumericUpDown numCoins;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown horizontalRotation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown verticalRotation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown val_posZ;
        private System.Windows.Forms.NumericUpDown val_posY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbl_posY;
        private System.Windows.Forms.NumericUpDown val_posX;
        private System.Windows.Forms.Label lbl_posX;
    }
}