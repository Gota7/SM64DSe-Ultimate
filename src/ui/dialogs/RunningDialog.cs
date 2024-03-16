using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SM64DSe.ui.dialogs
{
    public partial class RunningDialog : Form
    {
        private string executable;
        public RunningDialog(string executable)
        {
            this.executable = executable;
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RunProcessAsync();
        }
        
        private void RunProcessAsync()
        {
            Process process = new Process();
            process.StartInfo.FileName = this.executable;
            process.StartInfo.Arguments = Program.m_ROMPath;
            process.EnableRaisingEvents = true;
            
            process.Exited += (sender, e) =>
            {
                this.Close();
                process.Dispose();
            };

            process.Start();
        }
    }
}