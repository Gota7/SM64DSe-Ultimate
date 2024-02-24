using System;
using System.IO;
using Serilog;
using SM64DSe.core.cli.options;
using SM64DSe.core.cli.workers;

namespace SM64DSe.core.cli.utils
{
    public class Copy
    {
        private byte[] data = null;
        private string target;
        private string currentDir;
        
        public Copy(string target, string currentDir = null)
        {
            this.target = target;
            this.currentDir = currentDir;
            
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
                if (currentDir != null)
                {
                    string nTarget = Path.Combine(currentDir, target);
                    if (!File.Exists(nTarget))
                    {
                        throw new FileNotFoundException($"File {nTarget} does not exists.");
                    }
                    this.target = nTarget;
                }
                else if (!File.Exists(target))
                {
                    throw new FileNotFoundException($"File {target} does not exists.");
                }

                Log.Debug($"Reading {this.target}");
                data = File.ReadAllBytes(this.target);
            }
        }

        public int To(string value, AbstractFileOptions options)
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
                if (currentDir != null)
                {
                    value = Path.Combine(currentDir, value);
                }
                File.WriteAllBytes(value, data);
            }
            return 0;
        }
    }
}