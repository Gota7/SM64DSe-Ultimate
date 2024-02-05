using CommandLine;

namespace SM64DSe.core.cli
{
    [Verb("insertDL", HelpText = "Generate a DL from precompiled binaries and insert it to the ROM")]
    public class InsertDLOptions : FileInsertOptions
    {
        [Value(0, Required = true, HelpText = "Path to the rom")]
        public string RomPath { get; set; }

        [Value(1, Required = true, HelpText = "Path to the build folder")]
        public string BuildFolderPath { get; set; }

        [Value(2, Required = true, HelpText = "Path to the target list")]
        public string TargetListPath { get; set; }
    }
}
