using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe {

    /// <summary>
    /// Filesystem editor form. Credits to HayashiSTL.
    /// </summary>
    public class FilesystemEditorForm : Form {
        private string m_SelectedFile = "";
        private NitroROM m_ROM;
        private bool promptOnClose;
        private IContainer components;
        private TreeView tvFiles;
        private Button btnAddFile;
        private Button btnAddDir;
        private Button btnRename;
        private Button btnRemove;
        private Button btnSaveChanges;
        private Button btnRevertChanges;

        public FilesystemEditorForm(MainForm main) {
            this.InitializeComponent();
            this.Text = "HayashiSTL's File System Editor";
            Icon = main.Icon;
            this.m_ROM = Program.m_ROM;
            ROMFileSelect.LoadFileList(this.tvFiles);
            this.DialogResult = DialogResult.Ignore;
            if (this.m_ROM.StartFilesystemEdit())
                return;
            this.Close();
        }

        private bool ValidateFilename(string name) {
            if (name.Length == 0 || name.Length >= 128) {
                int num = (int)MessageBox.Show("The name must be between 1 and 127 characters long inclusive", "Bad file name");
                return false;
            }
            if (name.IndexOfAny(new char[4]
            {
        '/',
        '\\',
        ':',
        char.MinValue
            }) == -1 && Encoding.UTF8.GetByteCount(name) == name.Length)
                return true;
            int num1 = (int)MessageBox.Show("The string must contain only ASCII characters and no /.", "Bad file name");
            return false;
        }

        private void FilesystemEditorForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.promptOnClose) {
                if (MessageBox.Show("Do you want to save your changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    this.m_ROM.SaveFilesystem();
                else
                    this.m_ROM.RevertFilesystem();
                this.DialogResult = DialogResult.OK;
            } else
                this.m_ROM.EndRW(false);
        }

        private void tvFiles_AfterSelect(object sender, TreeViewEventArgs e) {
            this.m_SelectedFile = e.Node == null || e.Node.Tag == null ? "" : e.Node.Tag.ToString();
            if (this.m_SelectedFile == "") {
                this.btnRemove.Enabled = this.btnRename.Enabled = this.btnAddDir.Enabled = this.btnAddFile.Enabled = false;
            } else {
                this.btnRemove.Enabled = this.btnRename.Enabled = true;
                this.btnAddDir.Enabled = this.btnAddFile.Enabled = this.m_SelectedFile.Last<char>() == '/';
            }
        }

        private void btnRename_Click(object sender, EventArgs e) {
            TextDialog textDialog = new TextDialog("Enter New File/Directory name", "Rename");
            textDialog.typedText = Path.GetFileName(m_SelectedFile);
            if (textDialog.typedText == "") { textDialog.typedText = Path.GetDirectoryName(m_SelectedFile); }
            if (textDialog.ShowDialog() != DialogResult.OK)
                return;
            string typedText = textDialog.typedText;
            if (!this.ValidateFilename(typedText))
                return;
            string str = this.m_SelectedFile.Substring(0, this.m_SelectedFile.TrimEnd('/').LastIndexOf('/') + 1);
            if (this.m_SelectedFile.Substring(this.m_SelectedFile.Length - 1) == "/")
                Program.m_ROM.RenameDir(this.m_SelectedFile.TrimEnd('/'), typedText, this.tvFiles.Nodes[0]);
            else
                Program.m_ROM.RenameFile(this.m_SelectedFile, typedText, this.tvFiles.Nodes[0]);
            this.promptOnClose = true;
            this.m_SelectedFile = str + typedText + (this.m_SelectedFile.Substring(this.m_SelectedFile.Length - 1) == "/" ? "/" : "");
        }

        private void btnSaveChanges_Click(object sender, EventArgs e) {
            this.m_ROM.SaveFilesystem();
            this.promptOnClose = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnRevertChanges_Click(object sender, EventArgs e) {
            this.m_ROM.RevertFilesystem();
            this.promptOnClose = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure you want to delete \"" + this.m_SelectedFile + "\"?", "Remove", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            this.m_SelectedFile.Substring(0, this.m_SelectedFile.TrimEnd('/').LastIndexOf('/') + 1);
            if (this.m_SelectedFile.Substring(this.m_SelectedFile.Length - 1) == "/")
                Program.m_ROM.RemoveDir(this.m_SelectedFile.TrimEnd('/'), this.tvFiles.Nodes[0]);
            else
                Program.m_ROM.RemoveFile(this.m_SelectedFile, this.tvFiles.Nodes[0]);
            this.promptOnClose = true;
        }

        private void btnAddFile_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string selectedFile = this.m_SelectedFile;
            List<string> filenames = new List<string>((IEnumerable<string>)openFileDialog.SafeFileNames);
            List<string> fullNames = new List<string>((IEnumerable<string>)openFileDialog.FileNames);
            Program.m_ROM.AddFile(this.m_SelectedFile, filenames, fullNames, this.tvFiles.Nodes[0]);
            this.promptOnClose = true;
        }

        private void btnAddDir_Click(object sender, EventArgs e) {
            TextDialog textDialog = new TextDialog("Enter new directory name", "Add Directory");
            if (textDialog.ShowDialog() != DialogResult.OK)
                return;
            string typedText = textDialog.typedText;
            if (!this.ValidateFilename(typedText))
                return;
            string selectedFile = this.m_SelectedFile;
            Program.m_ROM.AddDir(this.m_SelectedFile, typedText, this.tvFiles.Nodes[0]);
            this.promptOnClose = true;
        }

        protected override void Dispose(bool disposing) {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.tvFiles = new TreeView();
            this.btnAddFile = new Button();
            this.btnAddDir = new Button();
            this.btnRename = new Button();
            this.btnRemove = new Button();
            this.btnSaveChanges = new Button();
            this.btnRevertChanges = new Button();
            this.SuspendLayout();
            this.tvFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.tvFiles.Location = new Point(12, 12);
            this.tvFiles.Name = "tvFiles";
            this.tvFiles.Size = new Size(556, 326);
            this.tvFiles.TabIndex = 0;
            this.tvFiles.AfterSelect += new TreeViewEventHandler(this.tvFiles_AfterSelect);
            this.btnAddFile.Location = new Point(12, 344);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new Size(100, 23);
            this.btnAddFile.TabIndex = 1;
            this.btnAddFile.Text = "Add File";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new EventHandler(this.btnAddFile_Click);
            this.btnAddDir.Location = new Point(12, 373);
            this.btnAddDir.Name = "btnAddDir";
            this.btnAddDir.Size = new Size(100, 23);
            this.btnAddDir.TabIndex = 2;
            this.btnAddDir.Text = "Add Directory";
            this.btnAddDir.UseVisualStyleBackColor = true;
            this.btnAddDir.Click += new EventHandler(this.btnAddDir_Click);
            this.btnRename.Location = new Point(118, 344);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new Size(100, 23);
            this.btnRename.TabIndex = 3;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new EventHandler(this.btnRename_Click);
            this.btnRemove.Location = new Point(118, 373);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new Size(100, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new EventHandler(this.btnRemove_Click);
            this.btnSaveChanges.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnSaveChanges.Location = new Point(468, 344);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new Size(100, 23);
            this.btnSaveChanges.TabIndex = 5;
            this.btnSaveChanges.Text = "Save Changes";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new EventHandler(this.btnSaveChanges_Click);
            this.btnRevertChanges.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnRevertChanges.Location = new Point(468, 373);
            this.btnRevertChanges.Name = "btnRevertChanges";
            this.btnRevertChanges.Size = new Size(100, 23);
            this.btnRevertChanges.TabIndex = 6;
            this.btnRevertChanges.Text = "Revert Changes";
            this.btnRevertChanges.UseVisualStyleBackColor = true;
            this.btnRevertChanges.Click += new EventHandler(this.btnRevertChanges_Click);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(580, 406);
            this.Controls.Add((Control)this.btnRevertChanges);
            this.Controls.Add((Control)this.btnSaveChanges);
            this.Controls.Add((Control)this.btnRemove);
            this.Controls.Add((Control)this.btnRename);
            this.Controls.Add((Control)this.btnAddDir);
            this.Controls.Add((Control)this.btnAddFile);
            this.Controls.Add((Control)this.tvFiles);
            this.Name = nameof(FilesystemEditorForm);
            this.Text = nameof(FilesystemEditorForm);
            this.FormClosed += new FormClosedEventHandler(this.FilesystemEditorForm_FormClosed);
            this.ResumeLayout(false);
        }
    }
}
