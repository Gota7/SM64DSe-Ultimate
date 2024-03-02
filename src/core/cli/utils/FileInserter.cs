using System;
using System.IO;
using System.Windows.Forms;
using Serilog;
using SM64DSe.core.cli.options;

namespace SM64DSe.core.cli.utils
{
    public class FileInserter
    {
        public static void InsertFile(string internalPath, byte[] data, AbstractFileOptions options)
        {
            if (Program.m_ROM.FileExists(internalPath))
            {
                Log.Warning($"Replacing {internalPath}.");
                NitroFile dlFile = Program.m_ROM.GetFileFromName(internalPath);
                dlFile.m_Data = data;
                dlFile.SaveChanges();
            }
            else if(options.Create)
            {
                Log.Warning($"You used --create option, it will change the filesystem.");
                Program.m_ROM.StartFilesystemEdit(); // Start editing mode
                 
                string[] splitted = internalPath.Split('/');
                string pathBuilder = "";
                for (var i = 0; i < splitted.Length - 1; i++)
                {
                    string tmp = $"{pathBuilder}{splitted[i]}/";
                    
                    // Check if the directory does not exist.
                    if (Program.m_ROM.GetDirIDFromName(tmp) == 0)
                    {
                        if (options.Recursive)
                        {
                            if (pathBuilder.Length == 0)
                            {
                                throw new InvalidOperationException("Cannot create directory in root.");
                            }
                            else
                            {
                                Log.Warning($"Creating internal directory {tmp}.");
                                Program.m_ROM.AddDir(pathBuilder, splitted[i], GetFilesystemRoot());
                            }
                        }
                        else
                        {
                            throw new DirectoryNotFoundException(
                                $"Internal directory {pathBuilder} not found in the ROM. Use --recursive option to create them.");
                        }
                    }
                    
                    pathBuilder = tmp;
                }
                
                // add to file system
                Program.m_ROM.AddFile(pathBuilder, splitted[splitted.Length - 1], data, GetFilesystemRoot());
                // save
                Program.m_ROM.SaveFilesystem();
            }
            else
            {
                throw new FileNotFoundException($"You tried insert data to a non-existent file ({internalPath}). If you want to create it you can use --create option.");
            }
        }

        private static TreeNode GetFilesystemRoot()
        {
            TreeView fileTree = new TreeView();
            ROMFileSelect.LoadFileList(fileTree);
            return fileTree.Nodes[0];
        }
    }
}
