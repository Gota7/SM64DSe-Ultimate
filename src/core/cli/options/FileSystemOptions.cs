using CommandLine;

namespace SM64DSe.core.cli.options
{
    public enum FileSystemOperation
    {
        cp,
        rm,
    }
    
    [Verb("fs", HelpText = "manipulate filesystem")]
    public class FileSystemOptions : AbstractFileOptions
    {
        [Value(0, Required = true, HelpText = "operation (cp | rm )")]
        public FileSystemOperation Operation { get; set; }
        
        [Value(1, Required = true, HelpText = "Path to the target.\nIf target is inside rom you must prefix it with rom://file/ or rom://overlay/, \nE.g. \n- rom://file/data/message/msg_data_eng.bin\n- rom://overlay/123")]
        public string Target { get; set; }

        [Value(2, Required = false, HelpText = "Path to the destination (same as Target, must have proper prefix when targeting rom.)")]
        public string Destination { get; set; }
    }
}
