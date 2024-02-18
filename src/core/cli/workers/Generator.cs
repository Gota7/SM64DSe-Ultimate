using System;
using Serilog;
using SM64DSe.core.cli.options;

namespace SM64DSe.core.cli.workers
{
    public class Generator: CLIWorker<GenerateOptions>
    {
        public override int Execute(GenerateOptions options)
        {
            // Setup rom
            this.SetupRom(options.RomPath);

            if (options.Format != "header")
                throw new ArgumentException("Only header format is supported today.");
            
            switch (options.Type)
            {
                case GenerationTarget.sound:
                    SoundHeaderGenerator.Generate(options.Output);
                    Log.Information($"header file {options.Output} generated for sounds.");
                    break;
                case GenerationTarget.filesystem:
                    FileHeaderGenerator.Generate(options.Output);
                    Log.Information($"header file {options.Output} generated for filesystem.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0;
        }
    }
}