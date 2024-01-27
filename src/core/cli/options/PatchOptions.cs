using CommandLine;

namespace SM64DSe.core.cli
{
    [Verb("patch", HelpText = "Apply a patch to the ROM")]
    public class PatchOptions
    {
        [Value(1, Required = false, HelpText = "Path to the rom")]
        public string romPath { get; set; }
        
        [Value(2, Required = true, HelpText = "Path to the patch file (format .sp2).")]
        public string PatchFile { get; set; }
    }
}