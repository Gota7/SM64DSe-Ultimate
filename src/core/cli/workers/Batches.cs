using System;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;

namespace SM64DSe.core.cli.workers
{
    public class Batches: CLIWorker<BatchesOptions>
    {
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

            string[] commands = File.ReadAllLines(options.BatchesFile);
            foreach (string command in commands)
            {
                if(command.StartsWith("#") || command.Trim().Length == 0)
                    continue;
                Log.Debug($"Executing command \"{command}\"");
                string[] args = command.Split(' ');
                CLIService.Run(args);
            }

            return 0;
        }
    }
}