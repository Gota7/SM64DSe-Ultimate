using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.utils.SP2;
using SM64DSe.Patcher;

namespace SM64DSe.core.cli.workers
{
    public class Compiler: CLIWorker<CompileOptions>
    {
        public static readonly byte[] EMPTY_FILE = { 0xde, 0xad, 0xbe, 0xef };

        public override void Execute(CompileOptions options)
        {
            Log.Information($"Compile source {options.Source} to {options.Type} in rom {options.RomPath} in path {options.InternalPath}");

            // Ensure the source directory exists
            if (!Directory.Exists(options.Source))
            {
                Log.Error($"Directory {options.Source} not found. Aborting patch.");
                throw new DirectoryNotFoundException();
            }
            
            switch (options.Type)
            {
                case CompileOptionsType.DL:
                    MakeDynamicLibrary(options);
                    break;
                case CompileOptionsType.OVERLAY:
                    MakeOverlay(options);
                    break;
                case CompileOptionsType.CLEAN:
                    PatchCompiler.cleanPatch(new DirectoryInfo(options.Source));
                    break;
            }
        }

        private void MakeOverlay(CompileOptions options)
        {
            // Setup rom
            this.SetupRom(options.RomPath);
            this.EnsurePatch(options.Force);
            
            throw new NotImplementedException("Make overlay is not implemented yet.");
        }

        private void MakeDynamicLibrary(CompileOptions options)
        {
            // Setup rom
            this.SetupRom(options.RomPath);
            this.EnsurePatch(options.Force);
            
            PatchMaker pm = new PatchMaker(
                new DirectoryInfo(options.Source), 
                0x02400000
            );

            List<Env> environments = new List<Env>();
            
            // Parse the environment variables provided by the --env option
            foreach (string s in options.Env)
            {
                string[] content = s.Split('=');
                if (content.Length != 2)
                    throw new ArgumentException("--env option should be in format [NAME]=[VALUE]");

                if (content[0] == "TARGET")
                    throw new ArgumentException("TARGET is reserved by the editor, you cannot change it.");

                string value = content[1];
                if (content[1].StartsWith("\"") && content[1].EndsWith("\""))
                {
                    value = content[1].Substring(1, content[1].Length - 1);
                }
                
                environments.Add(new Env(content[0], value));
            }
            
            byte[] data = pm.MakeDynamicLibrary(environments.ToArray());
            
            if (Program.m_ROM.FileExists(options.InternalPath))
            {
                Log.Warning($"Replacing {options.InternalPath} with the dynamic library content.");
                NitroFile dlFile = Program.m_ROM.GetFileFromName(options.InternalPath);
                dlFile.m_Data = data;
                dlFile.SaveChanges();
            }
            else if(options.Create)
            {
                Log.Warning($"You used --create option, it will change the filesystem.");
                Program.m_ROM.StartFilesystemEdit(); // Start editing mode
                 
                string[] splitted = options.InternalPath.Split('/');
                string pathBuilder = "";
                for (var i = 0; i < splitted.Length - 1; i++)
                {
                    string tmp = $"{pathBuilder}{splitted[i]}/";
                    
                    // Check if the directory does not exist.
                    if (Program.m_ROM.GetDirIDFromName(tmp) == 0)
                    {
                        if (options.Recursive)
                        {
                            if (pathBuilder.Length == 0)
                            {
                                throw new InvalidOperationException("Cannot create directory in root.");
                            }
                            else
                            {
                                Log.Warning($"Creating internal directory {tmp}.");
                                Program.m_ROM.AddDir(pathBuilder, splitted[i], GetFilesystemRoot());
                            }
                        }
                        else
                        {
                            throw new DirectoryNotFoundException(
                                $"Internal directory {pathBuilder} not found in the ROM. Use --recursive option to create them.");
                        }
                    }
                    
                    pathBuilder = tmp;
                }
                
                // add to file system
                Program.m_ROM.AddFile(pathBuilder, splitted[splitted.Length - 1], data, GetFilesystemRoot());
                // save
                Program.m_ROM.SaveFilesystem();
            }
            else
            {
                throw new FileNotFoundException($"You tried to compile and insert a dynamic library to a non-existent file ({options.InternalPath}). If you want to create it you can use --create option.");
            }
        }

        private static TreeNode GetFilesystemRoot()
        {
            TreeView fileTree = new TreeView();
            ROMFileSelect.LoadFileList(fileTree);
            return fileTree.Nodes[0];
        }
    }
}