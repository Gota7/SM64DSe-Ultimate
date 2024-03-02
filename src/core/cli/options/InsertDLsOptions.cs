using CommandLine;

namespace SM64DSe.core.cli.options
{
    [Verb("insertDLs", HelpText = "Generate a DL from precompiled binaries and insert it to the ROM")]
    public class InsertDLsOptions : AbstractFileOptions
    {
        [Value(1, Required = true, HelpText = "Path to the build folder")]
        public string BuildFolderPath { get; set; }

        [Value(2, Required = true, HelpText = "Path to the target list")]
        public string TargetListPath { get; set; }
        
        [Option("newcode-lo", Required = false, HelpText = "", Default = "newcode_lo")]
        public string NewCodeLo { get; set; }
        
        [Option("newcode-hi", Required = false, HelpText = "", Default = "newcode_hi")]
        public string NewCodeHi { get; set; }
    }
}
