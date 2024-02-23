using CommandLine;

namespace SM64DSe.core.cli.options
{
    [Verb("batches", HelpText = "Execute commands in batches from a text file.")]
    public class BatchesOptions : AbstractFileOptions
    {
        [Value(1, Required = true, HelpText = "Path to the text file containing commands.")]
        public string BatchesFile { get; set; }
        
        [Option("force", Required = false, HelpText = "Force the editor to patch the file when dealing with vanilla rom.")]
        public bool Force { get; set; }
    }
}