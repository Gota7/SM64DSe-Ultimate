using System;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.utils.DynamicLibraries;
using SM64DSe.core.utils.SP2;

namespace SM64DSe.core.cli
{
    public static class CliManager
    {
        public static void ExecutePatch(PatchOptions options)
        {
            // Ensure rom provided is .nds format
            if (!options.romPath.EndsWith(".nds"))
            {
                Log.Error("Only support .nds file. Aborting patch.");
                Environment.Exit(1);
                return;
            }

            // Ensure the rom file exist
            if (!File.Exists(options.romPath))
            {
                Log.Error($"File {options.romPath} not found. Aborting patch.");
                Environment.Exit(1);
                return;
            }
            
            // Ensure the patch file exist
            if (!File.Exists(options.PatchFile))
            {
                Log.Error($"File {options.PatchFile} not found. Aborting patch.");
                Environment.Exit(1);
                return;
            }
            
            // Load object database
            ObjectDatabase.Initialize();

            Program.m_IsROMFolder = false;
            Program.m_ROMPath = options.romPath;
            Program.m_ROM = new NitroROM(options.romPath);
            Log.Information($"Patch {options.romPath} with file {options.PatchFile}");
            
            Program.m_ROM.BeginRW();
            if (Program.m_ROM.NeedsPatch())
            {
                if (!options.Force)
                {
                    Log.Error($"Vanilla rom requires additional patch to be used by the editor. Aborting patch. Use --force option to allow the editor to patch automatically the required patches.");
                    Environment.Exit(1);
                    return;
                }
                
                Program.m_ROM.EndRW();
                Program.m_ROM.BeginRW(true);
                
                Log.Warning("vanilla rom detected, editor will patch it to make it editable.");
                try
                {
                    Program.m_ROM.Patch();
                }
                catch (Exception ex)
                {
                    Log.Error(
                        "An error occured while patching your ROM.\n" + 
                        "No changes have been made to your ROM.\n" + 
                        "Try using a different ROM. If the error persists, report it to Mega-Mario, with the details below:\n\n" + 
                        ex.Message + "\n" + 
                        ex.StackTrace);
                    Program.m_ROM.EndRW(false);
                    Program.m_ROMPath = "";
                    Environment.Exit(1);
                    return;
                }
            }
            
            // Load file tables
            Program.m_ROM.LoadTables();
            Program.m_ROM.EndRW();
            
            if (!DLUtils.HasDynamicLibrarySupport())
            {
                if (!options.Force)
                {
                    Log.Error($"rom requires shifting table patch to be compliant with .sp2 patch. Aborting patch. Use --force option to allow the editor to patch automatically the required patches.");
                    Environment.Exit(1);
                    return;
                }
                
                DLUtils.ApplyDynamicLibraryPatch();
                Log.Information("Dynamic libraries support patch applied.");
            }
            
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
        
        public static void ExecuteExtract(ExtractOptions options)
        {
            Log.Information($"Extract {options.romPath} to directory {options.Output}");
            Log.Error("Not supported yet.");
        }
    }
}