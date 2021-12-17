/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SM64DSe
{
    public class NARC : NitroFile
    {
        public NARC(NitroROM rom, ushort id) : base(rom, id)
        {
            if (Program.m_IsROMFolder) {
                string[] files = Ndst.Helper.ReadROMLines("__ROM__/files.txt", Program.m_ROMBasePath, Program.m_ROMPatchPath);
                string filePath2 = "";
                foreach (var f in files) {
                    string[] fileParam = f.Split(' ');
                    if (Ndst.Helper.ReadStringNumber(fileParam[1]) == id) {
                        filePath2 = fileParam[0].Substring(3);
                    }
                }

                ushort currFolderId = 1;
                string[] fileList = Ndst.Helper.ReadROMLines(filePath2 + ".txt", Program.m_ROMBasePath, Program.m_ROMPatchPath);
                Dictionary<string, Ndst.Folder> folders = new Dictionary<string, Ndst.Folder>();
                Ndst.Filesystem Filesystem = new Ndst.Filesystem();
                folders.Add("", Filesystem);
                List<FileEntry> newFiles = new List<FileEntry>();
                List<DirEntry> newFolders = new List<DirEntry>();
                DirEntry root2 = new DirEntry();
                root2.ID = 0xF000;
                newFolders.Add(root2);
                foreach (var s in fileList) {
                    AddFileToFilesystem(s);
                }
                m_DirEntries = newFolders.ToArray();
                m_FileEntries = newFiles.ToArray();

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
                    ushort fileId = (ushort)Ndst.Helper.ReadStringNumber(fileProperties[1]);

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
                    Ndst.File f = new Ndst.File();
                    f.Name = fileName;
                    f.Id = fileId;

                    // Finally add the file to the folder.
                    FileEntry fe = new FileEntry();
                    fe.ID = fileId;
                    fe.Name = fileName;
                    fe.FullName = (folderPath.Equals("") ? "" : (folderPath + "/")) + fe.Name;
                    AddFileToFolder(f, folderPath, ref fe);

                }

                // Add a file to a folder.
                void AddFileToFolder(Ndst.File f, string folderPath, ref FileEntry fe) {

                    // First check if the folder exists.
                    if (!folders.ContainsKey(folderPath)) {
                        AppendFolder(folderPath);
                    }

                    // Add the file to the folder.
                    folders[folderPath].Files.Add(f);
                    fe.ParentID = (ushort)(folders[folderPath].Id | 0xF000);
                    newFiles.Add(fe);

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
                    Ndst.Folder f = new Ndst.Folder() { Id = currFolderId++, Name = folderName };
                    folders[baseFolderPath].Folders.Add(f);
                    folders.Add(folderPath, f);
                    DirEntry de = new DirEntry();
                    de.Name = f.Name;
                    de.ID = (ushort)(f.Id | 0xF000);
                    de.FullName = folderPath;
                    de.ParentID = (ushort)(folders[baseFolderPath].Id | 0xF000);
                    newFolders.Add(de);
                }
                return;
            }

            FATOffset = 0x1C;
            FATSize = Read32(0x14) - 0xC;
            FNTOffset = 0x1C + FATSize + 0x8;
            FNTSize = Read32(FNTOffset - 0x4) - 0x8;
            IMGOffset = FNTOffset + FNTSize + 0x8;
            IMGSize = Read32(IMGOffset - 0x4) - 0x8;

            ushort numdirs = Read16(FNTOffset + 0x6);
            ushort numfiles = (ushort)(FATSize / 8);

            m_DirEntries = new DirEntry[numdirs];
            m_FileEntries = new FileEntry[numfiles];

            for (ushort f = 0; f < numfiles; f++)
            {
                uint start = Read32((uint)(FATOffset + (f*8)));
                uint end = Read32((uint)(FATOffset + (f*8) + 4));

                FileEntry fe;
                fe.ID = f;
                fe.ParentID = 0;
                fe.Offset = start;
                fe.Size = end - start;
                fe.Name = fe.FullName = "";
                m_FileEntries[f] = fe;
            }

            DirEntry root;
            root.ID = 0xF000;
            root.ParentID = 0;
            root.Name = root.FullName = "";
            m_DirEntries[0] = root;

            uint tableoffset = FNTOffset;
            for (ushort d = 0; d < numdirs; d++)
            {
                uint subtableoffset = FNTOffset + Read32(tableoffset);
                ushort first_fileid = Read16(tableoffset + 0x4);
                ushort cur_fileid = first_fileid;

                for (; ; )
                {
                    byte type_len = Read8(subtableoffset);
                    subtableoffset++;

                    if (type_len == 0x00) break;
                    else if (type_len > 0x80)
                    {
                        DirEntry dir;

                        dir.Name = ReadString(subtableoffset, type_len & 0x7F);
                        subtableoffset += (uint)(type_len & 0x7F);
                        dir.ID = Read16(subtableoffset);
                        subtableoffset += 0x2;
                        dir.ParentID = (ushort)(d + 0xF000);
                        dir.FullName = "";

                        m_DirEntries[dir.ID - 0xF000] = dir;
                    }
                    else if (type_len < 0x80)
                    {
                        m_FileEntries[cur_fileid].ParentID = (ushort)(d + 0xF000);
                        m_FileEntries[cur_fileid].Name = ReadString(subtableoffset, type_len & 0x7F);
                        subtableoffset += (uint)(type_len & 0x7F);
                        cur_fileid++;
                    }
                }

                tableoffset += 8;
            }

            for (int i = 0; i < m_DirEntries.Length; i++)
            {
                if (m_DirEntries[i].ParentID > 0xF000)
                    m_DirEntries[i].FullName = m_DirEntries[m_DirEntries[i].ParentID - 0xF000].FullName + "/" + m_DirEntries[i].Name;
                else
                    m_DirEntries[i].FullName = m_DirEntries[i].Name;
            }

            for (int i = 0; i < m_FileEntries.Length; i++)
            {
                if (m_FileEntries[i].ParentID > 0xF000)
                    m_FileEntries[i].FullName = m_DirEntries[m_FileEntries[i].ParentID - 0xF000].FullName + "/" + m_FileEntries[i].Name;
                else
                    m_FileEntries[i].FullName = m_FileEntries[i].Name;
            }
        }

        private string PathFromFileID(ushort id, bool forcePatch) {
            if (forcePatch) {
                return Program.m_ROMPatchPath + "/" + GetFileNameFromID(id);
            }
            return Ndst.Helper.ROMUsePatch(GetFileNameFromID(id), Program.m_ROMPatchPath) ? (Program.m_ROMPatchPath + "/" + GetFileNameFromID(id)) : (Program.m_ROMBasePath + "/" + GetFileNameFromID(id));
        }

        public ushort GetFileIDFromName(string name)
        {
            foreach (FileEntry fe in m_FileEntries)
            {
                if (fe.FullName == name)
                    return fe.ID;
            }

            return 0xFFFF;
        }

        public string GetFileNameFromID(ushort id)
        {
            return m_FileEntries[id].FullName;
        }

        public FileEntry[] GetFileEntries()
        {
            return m_FileEntries;
        }

        public void MakeRoom(uint addr, uint amount)
        {
            if (Program.m_IsROMFolder) {
                return;
            }

            int filelen = m_Data.Length;

            byte[] tomove = ReadBlock(addr, (uint)(filelen - addr));
            WriteBlock(addr + amount, tomove);
        }

        public byte[] ExtractFile(ushort fileid)
        {
            if (Program.m_IsROMFolder) {
                return File.ReadAllBytes(PathFromFileID(fileid, false));
            }

            FileEntry fe = m_FileEntries[fileid];
            return ReadBlock(IMGOffset + fe.Offset, fe.Size);
        }

        public void ReinsertFile(ushort fileid, byte[] data)
        {
            if (Program.m_IsROMFolder) {
                File.WriteAllBytes(PathFromFileID(fileid, true), data);
                return;
            }

            int datalength = (data.Length + 3) & ~3;

            FileEntry fe = m_FileEntries[fileid];

            UInt32 fileend = IMGOffset + fe.Offset + fe.Size;
            int delta = (int)(data.Length - fe.Size);
            uint existingPadding = (fileid != m_FileEntries.Length - 1) ? (m_FileEntries[fileid + 1].Offset -
                (fe.Offset + fe.Size)) : 0;
            uint entryEndPadded = (uint)(((fe.Offset + fe.Size) + 3) & ~3);
            uint roomNeeded = 0;

            /*
             * Example of making room:
             * 0	5	8   11 12
             * |    |   |   |  |
             * |    |   |   |  ^new end of file padded to next 4 byte boundary
			 * |    |   |   ^new end of file
             * |    |   ^offset of next file (3 bytes of padding)
             * |    ^current file end
             * ^offset of file
             *          
             * Here we round up to the next 4 byte boundary the length of the new file - this will be 
             * the offset of the next file and do the same for the existing file's length.
             * The difference is the amount of room needed - we must ensure that file offsets are on 
             * 4 byte boundaries and that any existing padding is overwritten
             */ 

            // move data that comes after the file
            if (delta > existingPadding)
            {
                uint newEndPadded = (uint)(((fe.Offset + data.Length) + 3) & ~3);
                uint newEnd = (uint)(fe.Offset + data.Length);
                roomNeeded = (uint)(newEndPadded - entryEndPadded);

                MakeRoom(fileend, (uint)roomNeeded);
            }

            // write the new data for the file
            WriteBlock(IMGOffset + fe.Offset, data);
            fe.Size = (uint)data.Length;

            // fix the FAT
            for (uint f = 0; f < (FATSize / 8); f++)
            {
                uint start = Read32(FATOffset + (f * 8));
                uint end = Read32(FATOffset + (f * 8) + 4);

                if (f > fileid)
                {
                    // Update start offsets of following files
                    start = (uint)((int)start + roomNeeded);
                    Write32(FATOffset + (f * 8), start);

                    // Update the end offsets of following files
                    end = (uint)((int)end + roomNeeded);
                    Write32(FATOffset + (f * 8) + 4, end);
                }

                if (f == fileid)
                {
                    // Update current file end
                    end = (uint)(fe.Offset + data.Length);
                    Write32(FATOffset + (f * 8) + 4, end);
                }
            }

            // fix misc stuff
            IMGSize = (uint)(IMGSize + roomNeeded);
            Write32(IMGOffset - 0x4, IMGSize + 0x8);

            Write32(0x8, (uint)((m_Data.Length + 3) & ~3));

            SaveChanges();
        }


        private uint FNTOffset, FNTSize;
        private uint FATOffset, FATSize;
        private uint IMGOffset, IMGSize;

        private struct DirEntry
        {
            public ushort ID;
            public ushort ParentID;
            public string Name;
            public string FullName;
        }

        public struct FileEntry
        {
            public ushort ID;
            public ushort ParentID;
            public string Name;
            public string FullName;
            public uint Offset;
            public uint Size;
        }

        private DirEntry[] m_DirEntries;
        private FileEntry[] m_FileEntries;
    }
}
