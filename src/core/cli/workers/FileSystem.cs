using System;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.cli.utils;
using FileOptions = SM64DSe.core.cli.options.FileOptions;

namespace SM64DSe.core.cli.workers
{
    public class FileSystem: CLIWorker<FileSystemOptions>
    {
        public override int Execute(FileSystemOptions options)
        {
            Log.Information($"Execute {options.Operation} on {options.Target} - {options.Destination}");

            // Setup rom
            this.SetupRom(options.RomPath);
            this.EnsurePatch(options.Force);

            switch (options.Operation)
            {
                case FileSystemOperation.cp:
                    return Copy(options.Target, options.Destination, options);
                case FileSystemOperation.rm:
                    return Remove(options.Target);
                default:
                    return 1;
            }
        }

        protected int Copy(string target, string destination, FileOptions options)
        {
            if (!RomUriUtils.IsRomUri(target) && !RomUriUtils.IsRomUri(destination))
                throw new ArgumentException("At least one of the arguments should be a rom target.");

            return new Copy(target).To(destination, options);
        }

        protected int Remove(string target)
        {
            if (!RomUriUtils.IsRomUri(target))
                throw new ArgumentException($"Only rom target is supported. Provided {target}. Example rom://file/data/message/msg_data_eng.bin");
            
            Uri uri = RomUriUtils.Parse(target);
            switch (uri.Authority)
            {
                case "file":
                    return RemoveFile(RomUriUtils.GetInternalPathFromUri(uri));
                case "overlay":
                    throw new NotImplementedException("Removing an overlay is not yet supported.");
                default:
                    throw new ArgumentException("Authority not supported.");
            }
        }
        
        protected int RemoveFile(string internalPath)
        {
            if (!Program.m_ROM.FileExists(internalPath))
                throw new FileNotFoundException($"File {internalPath} does not exists.");

            Program.m_ROM.StartFilesystemEdit();
            Program.m_ROM.SafeRemoveFile(internalPath);
            Program.m_ROM.SaveFilesystem();
            
            return 0;
        }
    }
}