using CommandLine;

namespace SM64DSe.core.cli.options
{
    [Verb("patch", HelpText = "Apply a patch to the ROM")]
    public class PatchOptions: AbstractRomOptions
    {
        [Value(1, Required = true, HelpText = "Path to the patch file (format .sp2).")]
        public string PatchFile { get; set; }
        
        [Option("force", Required = false, HelpText = "Force the editor to patch the file when dealing with vanilla rom.")]
        public bool Force { get; set; }
    }
}