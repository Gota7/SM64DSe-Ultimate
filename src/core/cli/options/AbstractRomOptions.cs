using CommandLine;

namespace SM64DSe.core.cli.options
{
    public class AbstractRomOptions
    {
        [Option("rom", Required = false, HelpText = "path to the rom (nds format)", Default = null)]
        public string RomPath { get; set; }
    }
}