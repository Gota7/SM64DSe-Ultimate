using System;
using System.IO;
using SM64DSe.core.cli.workers;
using FileOptions = SM64DSe.core.cli.options.FileOptions;

namespace SM64DSe.core.cli.utils
{
    public class Copy
    {
        private byte[] data = null;
        private string target;
        
        public Copy(string target)
        {
            // Load bytes from internal path
            if (RomUriUtils.IsRomUri(target))
            {
                Uri uri = RomUriUtils.Parse(target);
                if (uri.Authority != "file")
                    throw new NotImplementedException($"Only file are supported: Requested {uri.Authority}.");

                string internalPath = RomUriUtils.GetInternalPathFromUri(uri);
                
                if (!Program.m_ROM.FileExists(internalPath))
                    throw new FileNotFoundException($"File {internalPath} does not exists in rom filesystem.");

                NitroFile file = Program.m_ROM.GetFileFromName(internalPath);
                data = new byte[file.m_Data.Length];
                file.m_Data.CopyTo(data, 0);
            }
            // Load bytes from system
            else
            {
                if (!File.Exists(target))
                    throw new FileNotFoundException($"File {target} does not exists.");

                data = File.ReadAllBytes(target);
            }
        }

        public int To(string value, FileOptions options)
        {
            if (target == value)
                throw new ArgumentException("Cannot have same target and value.");

            if (RomUriUtils.IsRomUri(value))
            {
                Uri uri = RomUriUtils.Parse(value);
                FileInserter.InsertFile(RomUriUtils.GetInternalPathFromUri(uri), data, options);
            }
            else
            {
                File.WriteAllBytes(value, data);
            }
            return 0;
        }
    }
}