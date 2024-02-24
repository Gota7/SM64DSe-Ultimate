using System;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;

namespace SM64DSe.core.cli.workers
{
    public class Generator: CLIWorker<GenerateOptions>
    {
        public override int Execute(GenerateOptions options)
        {
            // Setup rom
            this.SetupRom(options);

            if (options.Format != "header")
                throw new ArgumentException("Only header format is supported today.");

            string output = options.Output;
            if (this._currentDirectory != null)
            {
                output = Path.Combine(_currentDirectory, output);
            }
            
            switch (options.Type)
            {
                case GenerationTarget.sound:
                    SoundHeaderGenerator.Generate(output);
                    Log.Information($"header file {output} generated for sounds.");
                    break;
                case GenerationTarget.filesystem:
                    FileHeaderGenerator.Generate(output);
                    Log.Information($"header file {output} generated for filesystem.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0;
        }
    }
}