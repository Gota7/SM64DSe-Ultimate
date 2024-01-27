using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SM64DSe.core.utils.SP2;

namespace SM64DSe
{
    public partial class PatchViewerForm : Form
    {
        private CommandInfo[] commandInfos;
        private int secondCounter;

        private SP2Patcher currentPatcher = null;

        public PatchViewerForm(string patchPath)
        {
            InitializeComponent();
            SetupSp2Patch(patchPath);
            StartSp2Patch();
        }

        private void SetupSp2Patch(string patchPath)
        {
            currentPatcher = new SP2Patcher(patchPath);
            commandInfos = currentPatcher.LoadPatches();
            
            // Setup UI
            lstCommands.Items.Clear();
            lstCommands.Items.AddRange(commandInfos);

            pbProgress.Minimum = pbProgress.Value = 0;
            pbProgress.Maximum = commandInfos.Count();
            pbProgress.Step = 1;
            lblProgress.BackColor = Color.Transparent;

            secondCounter = 0;
        }

        private void StartSp2Patch()
        {
            Thread thread = new Thread(() =>
            {
                currentPatcher.ApplyPatch((commandInfo, index) =>
                {
                    this.commandInfos[index] = commandInfo;
                    UpdateForm(index);
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void lstCommands_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            CommandInfo item = (CommandInfo)(sender as ListBox).Items[e.Index];

            e.DrawBackground();
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            g.DrawString(item.ToString(), e.Font, item.GetTextColor(), new PointF(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCommandInfo.Text = lstCommands.SelectedIndex < 0 ? "" : commandInfos[lstCommands.SelectedIndex].description;
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            if (currentPatcher != null)
                StartSp2Patch();
        }

        private void PatchViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanCloseWindow())
            {
                System.Media.SystemSounds.Hand.Play();
                e.Cancel = true;
            }
        }

        private void btnReloadScript_Click(object sender, EventArgs e)
        {
            if (!CanCloseWindow())
			{
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            // Reload
            commandInfos = currentPatcher.LoadPatches();
            // Start
            StartSp2Patch();
        }

        private void btnImportScript_Click(object sender, EventArgs e)
        {
            if (!CanCloseWindow())
            {
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "SM64DSe Patch(*.sp2)|*.sp2";
            o.RestoreDirectory = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            // Load from file
            SetupSp2Patch(o.FileName);
        }

        private bool CanCloseWindow()
		{
            IEnumerable<CommandInfo.State> states = commandInfos.Select(c => c.state);
            return !states.Contains(CommandInfo.State.WAITING) || states.Contains(CommandInfo.State.FAILED);
        }

        private void UpdateForm(int refreshIndex)
        {
            pbProgress.Invoke(new MethodInvoker(delegate { pbProgress.Value = commandInfos.Where(c => c.state == CommandInfo.State.SUCCESS || c.state == CommandInfo.State.WARNING).Count(); }));
            lblProgress.Invoke(new MethodInvoker(delegate { lblProgress.Text = $"Progress: {pbProgress.Value * 100 / pbProgress.Maximum}%"; }));
            lstCommands.Invoke(new MethodInvoker(delegate { lstCommands.Items[refreshIndex] = lstCommands.Items[refreshIndex]; }));
            txtCommandInfo.Invoke(new MethodInvoker(delegate { txtCommandInfo.Text = lstCommands.SelectedIndex < 0 ? "" : commandInfos[lstCommands.SelectedIndex].description; }));

            IEnumerable<CommandInfo.State> states = commandInfos.Select(c => c.state);
            bool enableRetryButton = !states.Contains(CommandInfo.State.WAITING) || states.Contains(CommandInfo.State.FAILED);

            btnRetry.GetCurrentParent().Invoke(new MethodInvoker(delegate { btnRetry.Enabled = enableRetryButton; }));
            btnReloadScript.GetCurrentParent().Invoke(new MethodInvoker(delegate { btnReloadScript.Enabled = enableRetryButton; }));
            btnImportScript.GetCurrentParent().Invoke(new MethodInvoker(delegate { btnImportScript.Enabled = enableRetryButton; }));
        }
    }
}
