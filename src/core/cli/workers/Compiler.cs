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
        private string sources;
        public override int Execute(CompileOptions options)
        {
            sources = options.Source;
            if (_currentDirectory != null)
            {
                sources = Path.Combine(_currentDirectory, sources);
            }
            
            Log.Information($"Compile source {sources} to {options.Type} in rom {options.RomPath} in path {options.InternalPath}");

            // Ensure the source directory exists
            if (!Directory.Exists(sources))
            {
                Log.Error($"Directory {sources} not found. Aborting patch.");
                throw new DirectoryNotFoundException();
            }
            
            switch (options.Type)
            {
                case CompileOptionsType.DL:
                    return MakeDynamicLibrary(options);
                case CompileOptionsType.OVERLAY:
                    return MakeOverlay(options);
                case CompileOptionsType.TARGET:
                    return MakeTarget(options);
                case CompileOptionsType.CLEAN:
                    return PatchCompiler.cleanPatch(new DirectoryInfo(sources));
                default:
                    // unknown
                    return 1;
            }
        }

        private int MakeOverlay(CompileOptions options)
        {
            throw new NotImplementedException("Make overlay is not implemented yet.");
        }

        private int MakeTarget(CompileOptions options)
        {
            Env[] envs = ParseEnvs(options.Env);
            string additionalEnvs = "";
            if (options.Env != null)
            {
                foreach (var env in envs)
                {
                    additionalEnvs += $"{env.GetName()}={env.GetValue()} ";
                }
            }
            
            string makeTemplate = "make {0}";
            string make = String.Format(
                makeTemplate, 
                additionalEnvs
            );

            int result = PatchCompiler.runProcess(make, sources);
            if (result != 0)
            {
                throw new Exception($"make command failed with result {result}");
            }
            return 0;
        }

        static Env[] ParseEnvs(IEnumerable<string> envs)
        {
            List<Env> environments = new List<Env>();
            
            // Parse the environment variables provided by the --env option
            foreach (string s in envs)
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

            return environments.ToArray();
        }

        private int MakeDynamicLibrary(CompileOptions options)
        {
            // Setup rom
            this.SetupRom(options);
            this.EnsurePatch(options.Force);
            
            PatchMaker pm = new PatchMaker(
                new DirectoryInfo(sources), 
                0x02400000
            );
            
            byte[] data = pm.MakeDynamicLibrary(ParseEnvs(options.Env));

            FileInserter.InsertFile(options.InternalPath, data, options);
            return 0;
        }
    }
}