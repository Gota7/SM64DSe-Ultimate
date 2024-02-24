using System.ComponentModel;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;

namespace SM64DSe.core.cli.workers
{
    public class Batches: CLIWorker<BatchesOptions>
    {
        private string[] commands;
        
        public override int Execute(BatchesOptions options)
        {
            // Setup rom
            this.SetupRom(options);
            this.EnsurePatch(options.Force);

            if (!File.Exists(options.BatchesFile))
            {
                Log.Error($"File path not found. Aborting.");
                throw new FileNotFoundException();
            }

            Worker._currentDirectory = Directory.GetParent(options.BatchesFile).FullName;
            Log.Debug($"Setting current directory to {_currentDirectory}");
            
            commands = File.ReadAllLines(options.BatchesFile);

            if (options.WithProgressBar)
            {
                ProgressDialog progdlg = new ProgressDialog($"Performing batches", commands.Length);
                BackgroundWorker lazyman = new BackgroundWorker();
            
                lazyman.WorkerReportsProgress = true;
                lazyman.WorkerSupportsCancellation = false;
                lazyman.DoWork += new DoWorkEventHandler(this.OnPatchRun);
                lazyman.ProgressChanged += new ProgressChangedEventHandler(progdlg.RW_ProgressChanged);
                lazyman.RunWorkerCompleted += new RunWorkerCompletedEventHandler(progdlg.RW_Completed);
                lazyman.RunWorkerAsync(progdlg);
                progdlg.ShowDialog();
            }
            else
            {
                OnPatchRun(null, null);
            }
            return 0;
        }

        public void OnPatchRun(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker lazyman = (BackgroundWorker)sender;
            
            // 8 / 73 - 8 /73 * 100
            
            for (var i = 0; i < commands.Length; i++)
            {
                var command = commands[i];
                if(command.StartsWith("#") || command.Trim().Length == 0)
                    continue;
                Log.Debug($"Executing command \"{command}\"");
                string[] args = command.Split(' ');
                CLIService.Run(args);
                if (lazyman != null)
                {
                    lazyman.ReportProgress((int) i * 100 + (i * 100) / commands.Length);
                }
            }
        }
    }
}