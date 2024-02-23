using System;
using CommandLine;
using Serilog;
using SM64DSe.core.cli.options;

namespace SM64DSe.core.cli
{
    public class CLIService
    {
        public static void Run(string[] args)
        {
            Parser.Default.ParseArguments<PatchOptions, CompileOptions, InsertDLsOptions, FileSystemOptions, GenerateOptions, BatchesOptions>(args)
                .WithParsed<PatchOptions>(options => HandleWorkerExecution(new core.cli.workers.Patcher(), options))
                .WithParsed<CompileOptions>(options => HandleWorkerExecution(new core.cli.workers.Compiler(), options))
                .WithParsed<InsertDLsOptions>(options => HandleWorkerExecution(new core.cli.workers.DLsInserter(), options))
                .WithParsed<FileSystemOptions>(options => HandleWorkerExecution(new core.cli.workers.FileSystem(), options))
                .WithParsed<GenerateOptions>(options => HandleWorkerExecution(new core.cli.workers.Generator(), options))
                .WithParsed<BatchesOptions>(options => HandleWorkerExecution(new core.cli.workers.Batches(), options));
        }
        
        // Define a method to handle worker execution and return codes
        private static void HandleWorkerExecution<T>(CLIWorker<T> worker, T options)
        {
            try
            {
               worker.Execute(options);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                Log.CloseAndFlush();
                ConsoleUtils.FreeConsole();
                Environment.Exit(1);
            }
        }
    }
}