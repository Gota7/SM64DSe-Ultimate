using System;
using System.IO;
using Serilog;
using SM64DSe.Patcher;

namespace SM64DSe.core.cli.workers
{
    public class DLInserter : CLIWorker<InsertDLOptions>
    {
        public override void Execute(InsertDLOptions options)
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

            try
            {
                using (StreamReader reader = new StreamReader(options.TargetListPath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            // Split the line at the first colon
                            string[] parts = line.Split(new [] {':'}, 2);

                            if (parts.Length != 2)
                            {
                                Log.Error($"Invalid syntax in {options.TargetListPath}: {line}");
                                Environment.Exit(1);
                                return;
                            }

                            string folderName = parts[0].Trim();
                            string folderPath = Path.Combine(options.BuildFolderPath, folderName);
                            string internalPath = parts[1].Trim();

                            string codePath0 = Path.Combine(folderPath, "newcode.bin");
                            string codePath1 = Path.Combine(folderPath, "newcode1.bin");

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
                                Log.Information($"Created DL from '{folderName}'");

                                FileInserter.InsertFile(internalPath, dl, options);
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error($"Error reading the target list: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}
