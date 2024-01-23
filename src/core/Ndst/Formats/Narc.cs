using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ndst.Formats {

    // Nitro archive to house more files.
    public class Narc : Folder, IFormat {
        public static ConversionInfo ConversionInfo; // HACKY. We must set this before calling NARC read.

        public bool IsType(byte[] data) {
            return data.Length >= 4 && data[0] == 'N' && data[1] == 'A' && data[2] == 'R' && data[3] == 'C';
        }

        public void Read(BinaryReader r, byte[] rawData) {
            
            // Place reader.
            long basePos = r.BaseStream.Position;
            r.BaseStream.Position += 0x18; // Get file offsets.

            // FATB, file offset table.
            uint numFiles = r.ReadUInt32();
            Tuple<uint, int>[] fileOffSizes = new Tuple<uint, int>[numFiles];
            for (int i = 0; i < numFiles; i++) {
                uint startOff = r.ReadUInt32();
                uint endOff = r.ReadUInt32();
                fileOffSizes[i] = new Tuple<uint, int>(startOff, (int)(endOff - startOff));
            }
            
            // FNTB, file name table.
            r.ReadUInt32(); // Skip magic.
            long fimgOff = r.ReadUInt32() + r.BaseStream.Position; // Calculate where the FIMG block is.
            long fntOff = r.BaseStream.Position;
            bool convertFiles = ConversionInfo != null;

            // Read root.
            var root = ReadFolder(0);
            Folders = root.Folders;
            Files = root.Files;
            FirstFileId = root.FirstFileId;

            // Read a folder.
            Folder ReadFolder(ushort id, Folder parent = null, string folderName = "") {

                // Get folder info.
                r.BaseStream.Position = fntOff + 8 * id;
                Folder ret = new Folder();
                ret.Parent = parent;
                ret.Id = id;
                ret.Name = folderName;
                uint off = r.ReadUInt32();
                ret.FirstFileId = r.ReadUInt16();
                r.ReadUInt16(); // Parent ID. Root folder is total number of folders.
                r.BaseStream.Position = fntOff + off;

                // Data.
                byte control = r.ReadByte();
                ushort currId = ret.FirstFileId;
                while (control != 0) {
                    bool folder = (control & 0x80) != 0;
                    control &= 0x7F;
                    string name = new string(r.ReadChars(control));
                    if (folder) {
                        ushort subId = (ushort)(r.ReadUInt16() & 0xFFF);
                        long bakPos = r.BaseStream.Position;
                        Folder newFolder = ReadFolder(subId, ret, name);
                        r.BaseStream.Position = bakPos;
                        ret.Folders.Add(newFolder);
                    } else {
                        long bakPos = r.BaseStream.Position;
                        r.BaseStream.Position = fileOffSizes[currId].Item1 + fimgOff;
                        byte[] fileData = r.ReadBytes(fileOffSizes[currId].Item2);
                        IFormat newData = null;
                        if (convertFiles) {
                            string filePath = name;
                            Folder currFolder = ret;
                            while (currFolder != null) {
                                filePath = currFolder.Name + "/" + filePath;
                                currFolder = currFolder.Parent;
                            }
                            if (filePath.StartsWith("/")) filePath = filePath.Substring(1);
                            newData = FormatUtil.DoExtractionConversion(ConversionInfo, r, bakPos, filePath, fileData);
                        } else {
                            newData = new GenericFile() {
                                Data = fileData
                            };
                        }
                        r.BaseStream.Position = bakPos;
                        ret.Files.Add(new File() { Name = name, Id = currId++, Data = newData });
                    }
                    control = r.ReadByte();
                }
                
                // Finish.
                return ret;

            }

        }

        public void Write(BinaryWriter w) {
            
            // Write header information.
            long basePos = w.BaseStream.Position;
            w.Write("NARC".ToCharArray()); // Magic.
            w.Write((ushort)0xFFFE); // Byte order.
            w.Write((ushort)0x100); // Version.
            w.SaveOffset("NARC_Size");
            w.Write((ushort)0x10); // Header size.
            w.Write((ushort)0x3); // Number of blocks.

            // Get a list of files, and write the FATB block.
            long ftabOff = w.BaseStream.Position;
            List<File> files = GetAllFiles().OrderBy(x => x.Id).ToList();;
            w.Write("BTAF".ToCharArray());
            w.SaveOffset("NARC_FTAB_Size");
            w.Write(files.Count);
            for (int i = 0; i < files.Count; i++) {
                w.SaveOffset("NARC_File" + i);
                w.SaveOffset("NARC_File" + i + "End");
            }
            w.WriteOffset("NARC_FTAB_Size", ftabOff);

            // Write FNTB block.
            long fntbOff = w.BaseStream.Position;
            w.Write("BTNF".ToCharArray());
            w.SaveOffset("NARC_FNTB_Size");
            long fntBase = w.BaseStream.Position;

            // Write folder data.
            List<Folder> folders = GetAllFolders().OrderBy(x => x.Id).ToList();
            folders.Insert(0, this);
            for (int i = 0; i < folders.Count; i++) {
                w.SaveOffset("folder" + folders[i].Id);
                w.Write(folders[i].FirstFileId);
                var parents = folders.Where(x => x.Folders.Contains(folders[i]));
                if (parents.Count() == 0) {
                    w.Write((ushort)folders.Count);
                } else {
                    w.Write((ushort)(parents.ElementAt(0).Id | 0xF000));
                }
            }
            void WriteFolder(Folder folder) {
                w.WriteOffset("folder" + folder.Id, fntBase);
                foreach (var f in folder.Folders) {
                    w.Write((byte)(f.Name.Length | 0x80));
                    w.Write(f.Name.ToCharArray());
                    w.Write((ushort)(f.Id | 0xF000));
                }
                foreach (var f in folder.Files) {
                    w.Write((byte)f.Name.Length);
                    w.Write(f.Name.ToCharArray());
                }
                w.Write((byte)0);
                foreach (var f in folder.Folders) {
                    WriteFolder(f);
                }
            }
            WriteFolder(this);

            // Alignment.
            while ((w.BaseStream.Position - basePos) % 0x4 != 0) {
                w.Write((byte)0xFF);
            }
            w.WriteOffset("NARC_FNTB_Size", fntbOff);

            // Write files.
            long fimgOff = w.BaseStream.Position;
            w.Write("GMIF".ToCharArray());
            w.SaveOffset("NARC_FIMG_Size");
            long fatOff = w.BaseStream.Position;
            for (int i = 0; i < files.Count; i++) {

                // Write offset and file data.
                w.WriteOffset("NARC_File" + i, fatOff);
                w.Write(files[i].Data);
                w.WriteOffset("NARC_File" + i + "End", fatOff);

                // Alignment.
                while ((w.BaseStream.Position - basePos) % 0x4 != 0) {
                    w.Write((byte)0xFF);
                }

            }
            w.WriteOffset("NARC_FIMG_Size", fimgOff);

            // Write the file size.
            w.WriteOffset("NARC_Size", basePos);

        }

        public void Extract(string destFolder) {

            // Path info.
            string arcName = new DirectoryInfo(destFolder).Name;
            string archiveTextPath = Directory.GetParent(destFolder).FullName;
            destFolder = Directory.GetParent(destFolder).Parent.FullName;
            
            // Extract files.
            List<Tuple<string, ushort>> fileInfo = new List<Tuple<string, ushort>>();
            void ExtractFiles(string path, string relativePath, Folder folder) {
                Directory.CreateDirectory(path);
                foreach (var f in folder.Folders) {
                    ExtractFiles(path + "/" + f.Name, relativePath + "/" + f.Name, f);
                }
                foreach (var f in folder.Files) {
                    string fInfo = relativePath + "/" + f.Name;
                    fInfo += " 0x" + f.Id.ToString("X");
                    f.Data.Extract(path + "/" + f.Name);
                    fileInfo.Add(new Tuple<string, ushort>(fInfo, f.Id));
                }
            }
            ExtractFiles(destFolder, "..", this);
            fileInfo = fileInfo.OrderBy(x => x.Item2).ToList();
            System.IO.File.WriteAllLines(archiveTextPath + "/" + arcName + ".txt", fileInfo.Select(x => x.Item1));

        }

        public void Pack(string archivePath) {

            // Path info.
            string arcName = new DirectoryInfo(archivePath).Name;
            string archiveTextPath = Directory.GetParent(archivePath).FullName;
            archivePath = Directory.GetParent(archivePath).Parent.FullName;

            // File reading content.
            byte[] ReadFile(string path) {
                return System.IO.File.ReadAllBytes(archivePath + "/" + path);
            }
            
            // Read files.
            ushort currFolderId = 1;
            string[] fileList = System.IO.File.ReadAllLines(archiveTextPath + "/" + arcName); // Can't add .txt extension here.
            Dictionary<string, Folder> folders = new Dictionary<string, Folder>();
            folders.Add("", this);
            List<ushort> validFileIds = new List<ushort>();
            foreach (var s in fileList) {
                AddFileToFilesystem(s);
            }

            // Add a file to the filesystem.
            void AddFileToFilesystem(string s) {

                // First get its properties.
                string[] fileProperties = s.Split(' ');
                string filePath = fileProperties[0];
                if (!filePath.StartsWith("../")) {
                    throw new Exception("ERROR: All files must be relative to parent directory \"../\"");
                } else {
                    filePath = filePath.Substring(3);
                }

                // Get file ID.
                ushort fileId = (ushort)Helper.ReadStringNumber(fileProperties[1]);
                if (validFileIds.Contains(fileId)) {
                    throw new Exception("ERROR: File ../" + filePath + " uses duplicate file ID 0x" + fileId.ToString("X"));
                } else {
                    validFileIds.Add(fileId);
                }

                // Proper file name and folder path.
                string fileName = filePath;
                string folderPath = "";
                while (fileName.Contains('/')) {
                    folderPath += "/" + fileName.Substring(0, fileName.IndexOf('/'));
                    fileName = fileName.Substring(fileName.IndexOf('/') + 1);
                }
                if (folderPath.StartsWith("/")) {
                    folderPath = folderPath.Substring(1);
                }

                // New file.
                File f = new File();
                f.Name = fileName;
                f.Id = fileId;
                f.Data = new GenericFile() { Data = ReadFile(filePath) };

                // Finally add the file to the folder.
                AddFileToFolder(f, folderPath);

            }

            // Add a file to a folder.
            void AddFileToFolder(File f, string folderPath) {

                // First check if the folder exists.
                if (!folders.ContainsKey(folderPath)) {
                    AppendFolder(folderPath);
                }

                // Add the file to the folder.
                folders[folderPath].Files.Add(f);

            }

            // Append a folder.
            void AppendFolder(string folderPath) {
                string folderName = folderPath;
                string baseFolderPath = "";
                while (folderName.Contains('/')) {
                    baseFolderPath += "/" + folderName.Substring(0, folderName.IndexOf('/'));
                    folderName = folderName.Substring(folderName.IndexOf('/') + 1);
                }
                if (baseFolderPath.StartsWith("/")) {
                    baseFolderPath = baseFolderPath.Substring(1);
                }
                if (!folders.ContainsKey(baseFolderPath)) {
                    AppendFolder(baseFolderPath);
                }
                Folder f = new Folder() { Id = currFolderId++, Name = folderName };
                folders[baseFolderPath].Folders.Add(f);
                folders.Add(folderPath, f);
            }

            // Fix first file IDs.
            foreach (var f in folders.Values) {
                // f.Files = f.Files.OrderBy(x => x.Name).ToList(); = I don't think this should be done.
                if (f.Files.Count > 0) f.FirstFileId = f.Files.OrderBy(x => x.Id).ElementAt(0).Id;
            }

            // Fix folders with no files.
            void FixFolders(Folder f) {
                foreach (var folder in f.Folders) {
                    FixFolders(folder);
                }
                if (f.Files.Count == 0) {
                    f.FirstFileId = f.Folders[0].FirstFileId;
                }
            }
            FixFolders(folders[""]);
            
        }

        public string GetFormat() {
            return "Narc";
        }

        public byte[] ContainedFile() {
            return null;
        }

        public bool IsOfFormat(string str) {
            return str.Equals("Narc");
        }

        public string GetPathExtension() => ".txt";

    }

}