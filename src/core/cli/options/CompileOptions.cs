using System.Collections.Generic;
using CommandLine;

namespace SM64DSe.core.cli.options
{
    public enum CompileOptionsType
    {
        DL,
        OVERLAY,
        CLEAN,
    }
    
    [Verb("compile", HelpText = "Compile ")]
    public class CompileOptions : FileOptions
    {
        [Value(1, Required = true, HelpText = "Output type (DL | OVERLAY | CLEAN)")]
        public CompileOptionsType Type { get; set; }
        
        [Value(2, Required = false, HelpText = "Path to the rom")]
        public string RomPath { get; set; }
        
        [Value(3, Required = true, HelpText = "Source code to compile")]
        public string Source { get; set; }
        
        [Value(4, Required = false, HelpText = "Internal path to insert the result in.")]
        public string InternalPath { get; set; }
        
        [Option('e', "env", HelpText = "Environment variable for the makefile.\nE.g. --env SOURCES=sources/peach/")]
        public IEnumerable<string>Env { get; set; }
    }
}