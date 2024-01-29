using System;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.utils.DynamicLibraries;
using SM64DSe.core.utils.SP2;

namespace SM64DSe.core.cli.workers
{
    public class Patcher: CLIWorker<PatchOptions>
    {
        public override void Execute(PatchOptions options)
        {
            Log.Information($"Patch {options.romPath} with file {options.PatchFile}");
            this.SetupRom(options.romPath);
            
            // Ensure the patch file exists
            if (!File.Exists(options.PatchFile))
            {
                Log.Error($"File {options.PatchFile} not found. Aborting patch.");
                Environment.Exit(1);
                return;
            }
            
            this.EnsurePatch(options.Force);
            
            Log.Information("Starting .sp2 patches");
            SP2Patcher patcher = new SP2Patcher(options.PatchFile);
            patcher.LoadPatches();
            bool success = true;
            patcher.ApplyPatch((commandInfo, index) =>
            {
                switch (commandInfo.state)
                {
                    case CommandInfo.State.WAITING:
                        break;
                    case CommandInfo.State.SUCCESS:
                        Log.Debug($"- {commandInfo.command}: {commandInfo.state}.");
                        break;
                    case CommandInfo.State.FAILED:
                        success = false;
                        Log.Error($"- {commandInfo.command}: {commandInfo.state}.\n{commandInfo.description}");
                        break;
                    case CommandInfo.State.WARNING:
                        Log.Warning($"- {commandInfo.command}: {commandInfo.state}.\n{commandInfo.description}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            if (!success)
            {
                Log.Error("Patch failed. See logs above.");
                Environment.Exit(1);
            }
            else
            {
                Log.Information("Patch applied with success. Congrats!");
                Environment.Exit(0);
            }
        }
    }
}