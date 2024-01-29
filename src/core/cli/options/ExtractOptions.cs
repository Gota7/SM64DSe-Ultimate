using CommandLine;

namespace SM64DSe.core.cli.options
{
    [Verb("extract", HelpText = "Extract data from the ROM")]
    public class ExtractOptions
    {
        [Value(1, Required = false, HelpText = "Path to the rom")]
        public string romPath { get; set; }
        
        [Value(2, Required = true, HelpText = "Path to output the result")]
        public string Output { get; set; }
    }
}