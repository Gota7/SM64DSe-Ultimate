using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using SM64DSe.core.cli.utils;
using SM64DSe.Patcher;

namespace SM64DSe.core.cli.workers
{
    public class DLInserter : CLIWorker<DlOptions>
    {
        public override void Execute(DlOptions options)
        {
            this.SetupRom(options.RomPath);

            // Ensure the build folder exists
            if (!Directory.Exists(options.BuildFolderPath))
            {
                Log.Error($"Folder {options.BuildFolderPath} not found.");
                Environment.Exit(1);
                return;
            }

            // Ensure the target list file exists
            if (!File.Exists(options.TargetListPath))
            {
                Log.Error($"Target list {options.TargetListPath} not found.");
                Environment.Exit(1);
                return;
            }

            this.EnsurePatch(options.Force);

            // key: folder name
            // value: rom directory
            Dictionary<string, string> targets = TargetParser.ParseFile(options.TargetListPath);
            
            // For each target
            foreach (var target in targets)
            {
                string folderPath = Path.Combine(options.BuildFolderPath, target.Key);
                
                string codePath0 = Path.Combine(folderPath, options.NewCodeLo);
                string codePath1 = Path.Combine(folderPath, options.NewCodeHi);
                
                // Ensure the binaries exist
                foreach (var path in new[] {codePath0, codePath1})
                {
                    if (!File.Exists(path))
                    {
                        Log.Error($"Binary file {path} not found.");
                        Environment.Exit(1);
                        return;
                    }
                }

                PatchMaker pm = new PatchMaker(
                    new DirectoryInfo(folderPath),
                    0x02400000
                );

                byte[] dl = pm.MakeDynamicLibraryFromBinaries();

                if (dl != null)
                {
                    Log.Information($"Created DL from '{target.Key}' in '{target.Value}'");
                    FileInserter.InsertFile(target.Value, dl, options);
                }
            }
        }
    }
}
