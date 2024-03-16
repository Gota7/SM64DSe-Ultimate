namespace SM64DSe
{
    partial class SettingsForm
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.cbAutoUpdateODB = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.grpGeneralSettings = new System.Windows.Forms.GroupBox();
            this.browserBtn = new System.Windows.Forms.Button();
            this.emulatorLabel = new System.Windows.Forms.Label();
            this.emulatorExecutablePath = new System.Windows.Forms.TextBox();
            this.grpModelAndCollisionMapImportationSettings = new System.Windows.Forms.GroupBox();
            this.chkUseSimpleModelAndCollisionMapImporters = new System.Windows.Forms.CheckBox();
            this.chkRememberLastUsedCollisionTypeAssignments = new System.Windows.Forms.CheckBox();
            this.chkDisableTextureSizeWarning = new System.Windows.Forms.CheckBox();
            this.chkRememberLastUsedModelImportationSettings = new System.Windows.Forms.CheckBox();
            this.openFileExe = new System.Windows.Forms.OpenFileDialog();
            this.grpGeneralSettings.SuspendLayout();
            this.grpModelAndCollisionMapImportationSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbAutoUpdateODB
            // 
            this.cbAutoUpdateODB.AutoSize = true;
            this.cbAutoUpdateODB.Location = new System.Drawing.Point(9, 29);
            this.cbAutoUpdateODB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbAutoUpdateODB.Name = "cbAutoUpdateODB";
            this.cbAutoUpdateODB.Size = new System.Drawing.Size(242, 24);
            this.cbAutoUpdateODB.TabIndex = 0;
            this.cbAutoUpdateODB.Text = "Auto-update object database";
            this.cbAutoUpdateODB.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(295, 354);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(174, 354);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(112, 35);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // grpGeneralSettings
            // 
            this.grpGeneralSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpGeneralSettings.Controls.Add(this.browserBtn);
            this.grpGeneralSettings.Controls.Add(this.emulatorLabel);
            this.grpGeneralSettings.Controls.Add(this.emulatorExecutablePath);
            this.grpGeneralSettings.Controls.Add(this.cbAutoUpdateODB);
            this.grpGeneralSettings.Location = new System.Drawing.Point(18, 18);
            this.grpGeneralSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpGeneralSettings.Name = "grpGeneralSettings";
            this.grpGeneralSettings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpGeneralSettings.Size = new System.Drawing.Size(393, 138);
            this.grpGeneralSettings.TabIndex = 3;
            this.grpGeneralSettings.TabStop = false;
            this.grpGeneralSettings.Text = "General Settings";
            // 
            // browserBtn
            // 
            this.browserBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browserBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browserBtn.Location = new System.Drawing.Point(277, 92);
            this.browserBtn.Name = "browserBtn";
            this.browserBtn.Size = new System.Drawing.Size(107, 34);
            this.browserBtn.TabIndex = 3;
            this.browserBtn.Text = "Browse";
            this.browserBtn.UseVisualStyleBackColor = true;
            this.browserBtn.Click += new System.EventHandler(this.browserBtn_Click);
            // 
            // emulatorLabel
            // 
            this.emulatorLabel.Location = new System.Drawing.Point(7, 72);
            this.emulatorLabel.Name = "emulatorLabel";
            this.emulatorLabel.Size = new System.Drawing.Size(303, 21);
            this.emulatorLabel.TabIndex = 2;
            this.emulatorLabel.Text = "Emulator executable";
            // 
            // emulatorExecutablePath
            // 
            this.emulatorExecutablePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.emulatorExecutablePath.Location = new System.Drawing.Point(8, 96);
            this.emulatorExecutablePath.Name = "emulatorExecutablePath";
            this.emulatorExecutablePath.Size = new System.Drawing.Size(260, 26);
            this.emulatorExecutablePath.TabIndex = 1;
            // 
            // grpModelAndCollisionMapImportationSettings
            // 
            this.grpModelAndCollisionMapImportationSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkUseSimpleModelAndCollisionMapImporters);
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkRememberLastUsedCollisionTypeAssignments);
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkDisableTextureSizeWarning);
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkRememberLastUsedModelImportationSettings);
            this.grpModelAndCollisionMapImportationSettings.Location = new System.Drawing.Point(17, 166);
            this.grpModelAndCollisionMapImportationSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpModelAndCollisionMapImportationSettings.Name = "grpModelAndCollisionMapImportationSettings";
            this.grpModelAndCollisionMapImportationSettings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpModelAndCollisionMapImportationSettings.Size = new System.Drawing.Size(392, 175);
            this.grpModelAndCollisionMapImportationSettings.TabIndex = 4;
            this.grpModelAndCollisionMapImportationSettings.TabStop = false;
            this.grpModelAndCollisionMapImportationSettings.Text = "Model and Collision Map Importation Settings";
            // 
            // chkUseSimpleModelAndCollisionMapImporters
            // 
            this.chkUseSimpleModelAndCollisionMapImporters.AutoSize = true;
            this.chkUseSimpleModelAndCollisionMapImporters.Location = new System.Drawing.Point(10, 31);
            this.chkUseSimpleModelAndCollisionMapImporters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkUseSimpleModelAndCollisionMapImporters.Name = "chkUseSimpleModelAndCollisionMapImporters";
            this.chkUseSimpleModelAndCollisionMapImporters.Size = new System.Drawing.Size(355, 24);
            this.chkUseSimpleModelAndCollisionMapImporters.TabIndex = 3;
            this.chkUseSimpleModelAndCollisionMapImporters.Text = "Use simple model and collision map importers";
            this.chkUseSimpleModelAndCollisionMapImporters.UseVisualStyleBackColor = true;
            // 
            // chkRememberLastUsedCollisionTypeAssignments
            // 
            this.chkRememberLastUsedCollisionTypeAssignments.AutoSize = true;
            this.chkRememberLastUsedCollisionTypeAssignments.Location = new System.Drawing.Point(10, 103);
            this.chkRememberLastUsedCollisionTypeAssignments.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkRememberLastUsedCollisionTypeAssignments.Name = "chkRememberLastUsedCollisionTypeAssignments";
            this.chkRememberLastUsedCollisionTypeAssignments.Size = new System.Drawing.Size(369, 24);
            this.chkRememberLastUsedCollisionTypeAssignments.TabIndex = 2;
            this.chkRememberLastUsedCollisionTypeAssignments.Text = "Remember last used collision type assignments";
            this.chkRememberLastUsedCollisionTypeAssignments.UseVisualStyleBackColor = true;
            // 
            // chkDisableTextureSizeWarning
            // 
            this.chkDisableTextureSizeWarning.AutoSize = true;
            this.chkDisableTextureSizeWarning.Location = new System.Drawing.Point(10, 138);
            this.chkDisableTextureSizeWarning.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkDisableTextureSizeWarning.Name = "chkDisableTextureSizeWarning";
            this.chkDisableTextureSizeWarning.Size = new System.Drawing.Size(232, 24);
            this.chkDisableTextureSizeWarning.TabIndex = 1;
            this.chkDisableTextureSizeWarning.Text = "Disable texture size warning";
            this.chkDisableTextureSizeWarning.UseVisualStyleBackColor = true;
            // 
            // chkRememberLastUsedModelImportationSettings
            // 
            this.chkRememberLastUsedModelImportationSettings.AutoSize = true;
            this.chkRememberLastUsedModelImportationSettings.Location = new System.Drawing.Point(10, 66);
            this.chkRememberLastUsedModelImportationSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkRememberLastUsedModelImportationSettings.Name = "chkRememberLastUsedModelImportationSettings";
            this.chkRememberLastUsedModelImportationSettings.Size = new System.Drawing.Size(372, 24);
            this.chkRememberLastUsedModelImportationSettings.TabIndex = 0;
            this.chkRememberLastUsedModelImportationSettings.Text = "Remember last used model importation settings";
            this.chkRememberLastUsedModelImportationSettings.UseVisualStyleBackColor = true;
            // 
            // openFileExe
            // 
            this.openFileExe.DefaultExt = "exe";
            this.openFileExe.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 403);
            this.Controls.Add(this.grpModelAndCollisionMapImportationSettings);
            this.Controls.Add(this.grpGeneralSettings);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(435, 443);
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.grpGeneralSettings.ResumeLayout(false);
            this.grpGeneralSettings.PerformLayout();
            this.grpModelAndCollisionMapImportationSettings.ResumeLayout(false);
            this.grpModelAndCollisionMapImportationSettings.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.OpenFileDialog openFileExe;

        private System.Windows.Forms.Button browserBtn;
        private System.Windows.Forms.TextBox emulatorExecutablePath;
        private System.Windows.Forms.Label emulatorLabel;

        #endregion

        private System.Windows.Forms.CheckBox cbAutoUpdateODB;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox grpGeneralSettings;
        private System.Windows.Forms.GroupBox grpModelAndCollisionMapImportationSettings;
        private System.Windows.Forms.CheckBox chkRememberLastUsedModelImportationSettings;
        private System.Windows.Forms.CheckBox chkDisableTextureSizeWarning;
        private System.Windows.Forms.CheckBox chkRememberLastUsedCollisionTypeAssignments;
        private System.Windows.Forms.CheckBox chkUseSimpleModelAndCollisionMapImporters;
    }
}