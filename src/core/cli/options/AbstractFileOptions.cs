using CommandLine;

namespace SM64DSe.core.cli.options
{
    // meant to be used as a base class
    public abstract class AbstractFileOptions: AbstractRomOptions
    {
        [Option("force", Required = false, HelpText = "Force the editor to patch the file when dealing with vanilla rom.")]
        public bool Force { get; set; }

        [Option("create", Required = false, HelpText = "Create the file entry if it does not exist.")]
        public bool Create { get; set; }

        [Option('r', "recursive", Required = false, HelpText = "Create parent directory of the file you want to create. Has no effect if --create is not provided.")]
        public bool Recursive { get; set; }
    }
}
