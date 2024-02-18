using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.cli.utils;
using SM64DSe.core.utils.SP2;
using SM64DSe.Patcher;

namespace SM64DSe.core.cli.workers
{
    public class Compiler: CLIWorker<CompileOptions>
    {
        public static readonly byte[] EMPTY_FILE = { 0xde, 0xad, 0xbe, 0xef };

        public override int Execute(CompileOptions options)
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
                    return MakeDynamicLibrary(options);
                case CompileOptionsType.OVERLAY:
                    return MakeOverlay(options);
                case CompileOptionsType.CLEAN:
                    return PatchCompiler.cleanPatch(new DirectoryInfo(options.Source));
                default:
                    // unknown
                    return 1;
            }
        }

        private int MakeOverlay(CompileOptions options)
        {
            // Setup rom
            this.SetupRom(options.RomPath);
            this.EnsurePatch(options.Force);
            
            throw new NotImplementedException("Make overlay is not implemented yet.");
        }

        private int MakeDynamicLibrary(CompileOptions options)
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

            FileInserter.InsertFile(options.InternalPath, data, options);
            return 0;
        }
    }
}