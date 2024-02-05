﻿using CommandLine;

namespace SM64DSe.core.cli
{
    [Verb("insertDLs", HelpText = "Generate a DL from precompiled binaries and insert it to the ROM")]
    public class InsertDLsOptions : FileOptions
    {
        [Value(0, Required = true, HelpText = "Path to the rom")]
        public string RomPath { get; set; }

        [Value(1, Required = true, HelpText = "Path to the build folder")]
        public string BuildFolderPath { get; set; }

        [Value(2, Required = true, HelpText = "Path to the target list")]
        public string TargetListPath { get; set; }
        
        [Option("newcode-lo", Required = false, HelpText = "", Default = "newcode_lo.bin")]
        public string NewCodeLo { get; set; }
        
        [Option("newcode-hi", Required = false, HelpText = "", Default = "newcode_hi.bin")]
        public string NewCodeHi { get; set; }
    }
}
