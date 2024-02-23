using System;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.utils.DynamicLibraries;

namespace SM64DSe.core.cli
{
    public abstract class CLIWorker<T>
    {
        public abstract int Execute(T options);

        protected void SetupRom(AbstractRomOptions options)
        {
            if (options.RomPath == null && Program.m_ROM == null)
            {
                throw new Exception("--rom option must be set when not executing in batches.");
            }

            if (options.RomPath != null && Program.m_ROM != null)
            {
                throw new Exception("A rom seem to already be loaded. You cannot use --rom option in batches.");
            }

            if (Program.m_ROM != null)
                return;
            
            // Extracting path
            string path = options.RomPath;
            
            // Ensure rom provided is .nds format
            if (!path.EndsWith(".nds"))
            {
                Log.Error("Only support .nds file. Aborting patch.");
                throw new FormatException();
            }

            // Ensure the rom file exists
            if (!File.Exists(path))
            {
                Log.Error($"File path not found. Aborting patch.");
                throw new FileNotFoundException();
            }
            
            // Load object database
            ObjectDatabase.Initialize();
            
            Program.m_IsROMFolder = false;
            Program.m_ROMPath = path;
            Program.m_ROM = new NitroROM(path);
        }

        protected void EnsurePatch(bool force)
        {
            Program.m_ROM.BeginRW();
            if (Program.m_ROM.NeedsPatch())
            {
                if (!force)
                {
                    Log.Error($"Vanilla rom requires additional patch to be used by the editor. Aborting patch. Use --force option to allow the editor to patch automatically the required patches.");
                    throw new FormatException();
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
                    throw new Exception();
                }
            }
            
            // Load file tables
            Program.m_ROM.LoadTables();
            Program.m_ROM.EndRW();
            
            if (!DLUtils.HasDynamicLibrarySupport())
            {
                if (!force)
                {
                    Log.Error($"rom requires shifting table patch to be compliant with .sp2 patch. Aborting patch. Use --force option to allow the editor to patch automatically the required patches.");
                    throw new FormatException();
                }
                
                DLUtils.ApplyDynamicLibraryPatch();
                Log.Information("Dynamic libraries support patch applied.");
            }
            
        }
    }
}