using System.Windows.Forms;
using Serilog;
using SM64DSe.core.cli.options;

namespace SM64DSe.core.cli
{
    public static class CliManager
    {
        public static void ExecutePatch(PatchOptions options)
        {
            Log.Information($"Patch {options.romPath} with file {options.PatchFile}");
        }
        
        public static void ExecuteExtract(ExtractOptions options)
        {
            Log.Information($"Extract {options.romPath} to directory {options.Output}");
            Log.Error("Not supported yet.");
        }
    }
}