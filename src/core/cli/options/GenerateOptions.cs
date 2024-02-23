using CommandLine;

namespace SM64DSe.core.cli.options
{
    public enum GenerationTarget
    {
        sound,
        filesystem,
    }
    
    [Verb("generate", HelpText = "Generate a file ")]
    public class GenerateOptions: AbstractRomOptions
    {
        [Value(1, Required = true, HelpText = "Target data ( sound | filesystem ).")]
        public GenerationTarget Type { get; set; }
        
        [Value(2, Required = true, HelpText = "Path for the generated file.")]
        public string Output { get; set; }
        
        [Option("format", Required = false, HelpText = "format of the generated content", Default = "header")]
        public string Format { get; set; }
    }
}