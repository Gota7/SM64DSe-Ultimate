using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ndst.Formats;

namespace Ndst {

    // Folder.
    public class Folder {
        public string Name = "";
        public ushort Id;
        public ushort FirstFileId;
        public Folder Parent = null;
        public List<Folder> Folders = new List<Folder>();
        public List<File> Files = new List<File>();

        public override string ToString() => "0x" + Id.ToString("X") + " - " + Name;
    
        public List<Folder> GetAllFolders() {
            List<Folder> ret = new List<Folder>();
            ret.AddRange(Folders);
            foreach (var f in Folders) {
                ret.AddRange(f.GetAllFolders());
            }
            return ret;
        }

        public List<File> GetAllFiles() {
            List<File> ret = new List<File>();
            ret.AddRange(Files);
            foreach (var f in Folders) {
                ret.AddRange(f.GetAllFiles());
            }
            return ret;
        }
    }

    // File.
    public class File {
        public string Name;
        public ushort Id;
        public IFormat Data;

        public override string ToString() => "0x" + Id.ToString("X") + " - " + Name;
    }

    // NDS Filesystem.
    // ID Convention: IDs start at 0, and first name the ARM9 overlays, then the ARM7 overlays. Then they go for files in the root folder, then recursively go through the subfolders.
    // Folder IDs start at 0 with the root, and recursively increase for each sub folder.
    // Both IDs are alphabetical.
    public class Filesystem : Folder {

        // Default.
        public Filesystem() {}

        // Create a new filesystem.
        public Filesystem(BinaryReader r, uint fntOff, uint fntSize, uint fatOff, uint fatSize, bool convertFiles, ConversionInfo conversionInfo) {

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
                        r.BaseStream.Position = fatOff + currId * 8;
                        uint startOff = r.ReadUInt32();
                        uint endOff = r.ReadUInt32();
                        r.BaseStream.Position = startOff;
                        byte[] fileData = r.ReadBytes((int)(endOff - startOff));
                        IFormat newData = null;
                        if (convertFiles) {
                            string filePath = name;
                            Folder currFolder = ret;
                            while (currFolder != null) {
                                filePath = currFolder.Name + "/" + filePath;
                                currFolder = currFolder.Parent;
                            }
                            if (filePath.StartsWith("/")) filePath = filePath.Substring(1);
                            newData = FormatUtil.DoExtractionConversion(conversionInfo, r, startOff, filePath, fileData);
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

            // Read root.
            var root = ReadFolder(0);
            Folders = root.Folders;
            Files = root.Files;
            FirstFileId = root.FirstFileId;

        }

        // Write a filesystem.
        public void WriteFilesystem(BinaryWriter w, List<Tuple<long, uint, Overlay>> arm9Offs, List<Tuple<long, uint, Overlay>> arm7Offs, byte[] banner) {
            
            // Write FNT block.
            long fntBase = w.BaseStream.Position;
            w.WriteOffset("fntOffset");
            List<Folder> folders = GetAllFolders().OrderBy(x => x.Id).ToList();
            folders.Insert(0, this);
            List<File> files = GetAllFiles().OrderBy(x => x.Id).ToList();
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
                foreach (var f in folder.Files) {
                    w.Write((byte)f.Name.Length);
                    w.Write(f.Name.ToCharArray());
                }
                foreach (var f in folder.Folders) {
                    w.Write((byte)(f.Name.Length | 0x80));
                    w.Write(f.Name.ToCharArray());
                    w.Write((ushort)(f.Id | 0xF000));
                }
                w.Write((byte)0);
                foreach (var f in folder.Folders) {
                    WriteFolder(f);
                }
            }
            WriteFolder(this);
            w.WriteOffset("fntSize", fntBase);
            w.Align(0x200);
            
            // Write FAT block.
            w.WriteOffset("fatOffset");
            long fatBase = w.BaseStream.Position;
            uint maxFileId = files.Last().Id;
            foreach (var o in arm9Offs) {
                if (o.Item3.FileId > maxFileId) {
                    maxFileId = o.Item3.FileId;
                }
            }
            foreach (var o in arm7Offs) {
                if (o.Item3.FileId > maxFileId) {
                    maxFileId = o.Item3.FileId;
                }
            }
            w.Write0s((uint)(maxFileId + 1) * 8);
            foreach (var o in arm9Offs) {
                w.BaseStream.Position = fatBase + o.Item3.FileId * 8;
                w.Write((uint)o.Item1);
                w.Write((uint)o.Item1 + o.Item2);
            }
            foreach (var o in arm7Offs) {
                w.BaseStream.Position = fatBase + o.Item3.FileId * 8;
                w.Write((uint)o.Item1);
                w.Write((uint)o.Item1 + o.Item2);
            }
            for (int i = 0; i < files.Count; i++) {
                w.BaseStream.Position = fatBase + files[i].Id * 8;
                w.SaveOffset("file" + files[i].Id);
                w.SaveOffset("file" + files[i].Id + "end");
            }
            w.BaseStream.Position = fatBase + (maxFileId + 1) * 8;
            w.WriteOffset("fatSize", fatBase);
            w.Align(0x200);

            // Write banner.
            w.WriteOffset("iconBannerOffset");
            w.Write(banner);
            w.Align(0x200);

            // Write files.
            for (int i = 0; i < files.Count; i++) {
                w.Align(0x200);
                w.WriteOffset("file" + files[i].Id);
                w.Write(files[i].Data);
                w.WriteOffset("file" + files[i].Id + "end");
            }

            // Write ROM size.
            w.BaseStream.Position -= 0x88;
            w.WriteOffset("romSize");

        }

    }

}