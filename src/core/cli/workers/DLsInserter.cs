using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.cli.utils;
using SM64DSe.Patcher;

namespace SM64DSe.core.cli.workers
{
    public class DLsInserter : CLIWorker<InsertDLsOptions>
    {
        public override int Execute(InsertDLsOptions options)
        {
            this.SetupRom(options.RomPath);

            // Ensure the build folder exists
            if (!Directory.Exists(options.BuildFolderPath))
            {
                Log.Error($"Folder {options.BuildFolderPath} not found.");
                return 1;
            }

            // Ensure the target list file exists
            if (!File.Exists(options.TargetListPath))
            {
                Log.Error($"Target list {options.TargetListPath} not found.");
                return 1;
            }

            this.EnsurePatch(options.Force);

            // key: folder name
            // value: rom directory
            Dictionary<string, string> targets = TargetParser.ParseFile(options.TargetListPath);
            
            // For each target
            foreach (var target in targets)
            {
                string folderPath = Path.Combine(options.BuildFolderPath, target.Key);
                
                string symFilePath   = Path.Combine(folderPath, options.NewCodeLo + ".sym");
                string loBinFilePath = Path.Combine(folderPath, options.NewCodeLo + ".bin");
                string hiBinFilePath = Path.Combine(folderPath, options.NewCodeHi + ".bin");
                
                // Ensure the binaries exist
                foreach (var path in new[] {symFilePath, loBinFilePath, hiBinFilePath})
                {
                    if (!File.Exists(path))
                    {
                        Log.Error($"Required file '{path}' not found.");
                        return 1;
                    }
                }

                PatchMaker pm = new PatchMaker(
                    new DirectoryInfo(folderPath),
                    0x02400000
                );

                byte[] dl = pm.MakeDynamicLibraryFromBinaries(options.NewCodeLo, options.NewCodeHi);

                if (dl != null)
                {
                    Log.Information($"Created DL from '{target.Key}' in '{target.Value}'");
                    FileInserter.InsertFile(target.Value, dl, options);
                }
            }

            return 0;
        }
    }
}
