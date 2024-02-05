﻿/*
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Forms;

namespace SM64DSe
{
    // NitroROM class
    // made especially for SM64DS, but most of it can be used with any NDS ROM

    public partial class NitroROM
    {
        const int ROM_END_MARGIN = 0x88;

        string[] m_MsgData;
        public BinaryReader arm9R;
        public BinaryWriter arm9W;
        public uint headerSize = 0x4000;

        public NitroROM(string path)
        {
            LoadROM(path);
        }

        public NitroROM(string basePath, string patchPath) {
            LoadROMExtracted(basePath, patchPath);
        }

        public void LoadROM(string path) {
            m_Path = path;
            m_CanRW = false;

            BeginRW();

            m_FileStream.Position = 0x00;
            char[] gametitle = m_BinReader.ReadChars(12);
            if (new string(gametitle) != "S.MARIO64DS\0") {
                EndRW();
                throw new Exception("This file isn't a Super Mario 64 DS ROM.");
            }

            m_FileStream.Position = 0x0C;
            uint gamecode = m_BinReader.ReadUInt32();
            m_FileStream.Position = 0x1E;
            byte romversion = m_BinReader.ReadByte();

            switch (gamecode) {
                case 0x454D5341: // ASME / USA
                    if (romversion == 0x01) {
                        m_Version = Version.USA_v2;

                        m_LevelOvlIDTableOffset = 0x742B4;
                        m_FileTableOffset = 0x11244;
                        m_FileTableLength = 1824;
                    } else {
                        m_Version = Version.USA_v1;

                        m_LevelOvlIDTableOffset = 0x73594;
                        m_FileTableOffset = 0x1123C;
                        m_FileTableLength = 1824;
                    }
                    break;

                case 0x4A4D5341: // ASMJ / JAP
                    m_Version = Version.JAP;

                    m_LevelOvlIDTableOffset = 0x73B38;
                    m_FileTableOffset = 0x1123C;
                    m_FileTableLength = 1824;
                    break;

                case 0x504D5341: // ASMP / EUR
                    m_Version = Version.EUR;

                    m_LevelOvlIDTableOffset = 0x758C8;
                    m_FileTableOffset = 0x13098;
                    m_FileTableLength = 2058;
                    break;

                default:
                    m_Version = Version.UNK;
                    EndRW();
                    throw new Exception("Unknown ROM version. Tell Mega-Mario about it.");
            }

            m_FileStream.Position = 0x28;
            ARM9RAMAddress = m_BinReader.ReadUInt32();

            m_FileStream.Position = 0x30;
            ARM7Offset = m_BinReader.ReadUInt32();
            m_FileStream.Position += 0x04;
            ARM7RAMAddress = m_BinReader.ReadUInt32();
            ARM7Size = m_BinReader.ReadUInt32();

            m_FileStream.Position = 0x40;
            FNTOffset = m_BinReader.ReadUInt32();
            FNTSize = m_BinReader.ReadUInt32();
            FATOffset = m_BinReader.ReadUInt32();
            FATSize = m_BinReader.ReadUInt32();
            OVTOffset = m_BinReader.ReadUInt32();
            OVTSize = m_BinReader.ReadUInt32();
            // no need to bother about ARM7 overlays... there's none in SM64DS

            m_FileStream.Position = 0x80;
            m_UsedSize = m_BinReader.ReadUInt32();
            //m_UsedSize += ROM_END_MARGIN;

            m_FileStream.Position = FNTOffset + 6;
            ushort numdirs = m_BinReader.ReadUInt16();
            ushort numfiles = (ushort)(FATSize / 8);

            m_DirEntries = new DirEntry[numdirs];
            m_FileEntries = new FileEntry[numfiles];

            m_FileStream.Position = FATOffset;
            for (ushort f = 0; f < numfiles; f++) {
                uint start = m_BinReader.ReadUInt32();
                uint end = m_BinReader.ReadUInt32();

                FileEntry fe;
                fe.ID = f;
                fe.InternalID = 0xFFFF;
                fe.ParentID = 0;
                fe.Offset = start;
                fe.Size = end - start;
                fe.Name = fe.FullName = "";
                fe.Data = null;
                m_FileEntries[f] = fe;
            }

            DirEntry root;
            root.ID = 0xF000;
            root.ParentID = 0;
            root.Name = root.FullName = "";
            m_DirEntries[0] = root;

            uint tableoffset = FNTOffset;
            for (ushort d = 0; d < numdirs; d++) {
                m_FileStream.Position = tableoffset;
                uint subtableoffset = FNTOffset + m_BinReader.ReadUInt32();
                ushort first_fileid = m_BinReader.ReadUInt16();
                ushort cur_fileid = first_fileid;

                m_FileStream.Position = subtableoffset;
                for (; ; )
                {
                    byte type_len = m_BinReader.ReadByte();

                    if (type_len == 0x00) break;
                    else if (type_len > 0x80) {
                        DirEntry dir;

                        dir.Name = new string(m_BinReader.ReadChars(type_len & 0x7F));
                        dir.ID = m_BinReader.ReadUInt16();
                        dir.ParentID = (ushort)(d + 0xF000);
                        dir.FullName = "";

                        m_DirEntries[dir.ID - 0xF000] = dir;
                    } else if (type_len < 0x80) {
                        char[] _name = m_BinReader.ReadChars(type_len & 0x7F);

                        m_FileEntries[cur_fileid].ParentID = (ushort)(d + 0xF000);
                        m_FileEntries[cur_fileid].Name = new string(_name);
                        cur_fileid++;
                    }
                }

                tableoffset += 8;
            }

            for (int i = 0; i < m_DirEntries.Length; i++) {
                if (m_DirEntries[i].ParentID > 0xF000)
                    m_DirEntries[i].FullName = m_DirEntries[m_DirEntries[i].ParentID - 0xF000].FullName + "/" + m_DirEntries[i].Name;
                else
                    m_DirEntries[i].FullName = m_DirEntries[i].Name;
            }

            for (int i = 0; i < m_FileEntries.Length; i++) {
                if (m_FileEntries[i].ParentID > 0xF000)
                    m_FileEntries[i].FullName = m_DirEntries[m_FileEntries[i].ParentID - 0xF000].FullName + "/" + m_FileEntries[i].Name;
                else
                    m_FileEntries[i].FullName = m_FileEntries[i].Name;
            }

            uint numoverlays = OVTSize / 0x20;
            m_OverlayEntries = new OverlayEntry[numoverlays];

            for (uint i = 0; i < numoverlays; i++) {
                m_FileStream.Position = OVTOffset + (i * 0x20);
                OverlayEntry oe;

                oe.EntryOffset = (uint)m_FileStream.Position;
                oe.ID = m_BinReader.ReadUInt32();
                oe.RAMAddress = m_BinReader.ReadUInt32();
                oe.RAMSize = m_BinReader.ReadUInt32();
                oe.BSSSize = m_BinReader.ReadUInt32();
                oe.StaticInitStart = this.m_BinReader.ReadUInt32();
                oe.StaticInitEnd = this.m_BinReader.ReadUInt32();
                oe.FileID = (ushort)this.m_BinReader.ReadUInt32();
                oe.Flags = this.m_BinReader.ReadUInt32();
                m_OverlayEntries[(int)oe.ID] = oe;
            }

            if (m_Version == Version.EUR) {
                //screw the l that looks like a 1
                //            \/
                NitroOverlay ov0 = new NitroOverlay(this, 0);
                //And of course, fix those hardcoded values
                //Who expected a new object to be inserted, anyway?
                m_FileTableOffset = ov0.ReadPointer(0xA4);
                m_FileTableLength = ov0.Read32(0x9C);

                // Hack for new overlay 0.
                if (m_FileTableLength > 0x2000) {
                    m_FileTableOffset = ov0.ReadPointer(0xAC) + 4;
                    m_FileTableLength = ov0.Read32(0xA4);
                }
            }

            LoadTables();
            EndRW();

            UpdateStrings();
        }

        public void LoadROMExtracted(string basePath, string patchPath) {

            // Get general info.
            Ndst.ROM r = JsonConvert.DeserializeObject<Ndst.ROM>(GetExtractedLines("__ROM__/header.json"));
            ARM9RAMAddress = r.Arm9EntryAddress;
            ARM7RAMAddress = r.Arm7EntryAddress;
            switch (r.GameCode) {
                // US.
                case "ASME":
                    if (r.Version == 0x01) {
                        m_Version = Version.USA_v2;
                        m_LevelOvlIDTableOffset = 0x742B4 - r.HeaderSize;
                        m_FileTableOffset = 0x11244;
                        m_FileTableLength = 1824;
                    } else {
                        m_Version = Version.USA_v1;
                        m_LevelOvlIDTableOffset = 0x73594 - r.HeaderSize;
                        m_FileTableOffset = 0x1123C;
                        m_FileTableLength = 1824;
                    }
                    break;
                // Jap.
                case "ASMJ":
                    m_Version = Version.JAP;
                    m_LevelOvlIDTableOffset = 0x73B38 - r.HeaderSize;
                    m_FileTableOffset = 0x1123C;
                    m_FileTableLength = 1824;
                    break;
                // Assume EUR.
                case "ASMP":
                    m_Version = Version.EUR;
                    m_LevelOvlIDTableOffset = 0x758C8 - r.HeaderSize;
                    m_FileTableOffset = 0x13098;
                    m_FileTableLength = 2058;
                    break;
            }

            // Read overlays.
            List<Ndst.Overlay> ovs = JsonConvert.DeserializeObject<List<Ndst.Overlay>>(Ndst.Helper.ReadROMText("__ROM__/arm9Overlays.json", Program.m_ROMBasePath, Program.m_ROMPatchPath));
            m_OverlayEntries = new OverlayEntry[ovs.Count];
            for (int i = 0; i < ovs.Count; i++) {
                OverlayEntry oe;
                oe.EntryOffset = 0xFFFFFFFF;
                oe.ID = ovs[i].Id;
                oe.RAMAddress = ovs[i].RAMAddress;
                oe.RAMSize = ovs[i].RAMSize;
                oe.BSSSize = ovs[i].BSSSize;
                oe.StaticInitStart = ovs[i].StaticInitStart;
                oe.StaticInitEnd = ovs[i].StaticInitEnd;
                oe.FileID = ovs[i].FileId;
                oe.Flags = ovs[i].Flags;
                m_OverlayEntries[(int)oe.ID] = oe;
            }

            if (m_Version == Version.EUR) {
                //screw the l that looks like a 1
                //            \/
                NitroOverlay ov0 = new NitroOverlay(this, 0);
                //And of course, fix those hardcoded values
                //Who expected a new object to be inserted, anyway?
                m_FileTableOffset = ov0.ReadPointer(0xA4);
                m_FileTableLength = ov0.Read32(0x9C);

                // Hack for new overlay 0.
                if (m_FileTableLength > 0x2000)
                {
                    m_FileTableOffset = ov0.ReadPointer(0xAC) + 4;
                    m_FileTableLength = ov0.Read32(0xA4);
                    if (File.Exists(Program.m_ROMPatchPath + "/../ASM/Overlays/filenames/filenamesWhole.h")) {
                        m_FileTableOffset = uint.MaxValue; // Hack for new overlay 0 having inconsistent placement with more files.
                    }
                }
            }

            // Read files.
            ushort currFolderId = 1;
            string[] fileList = Ndst.Helper.ReadROMLines("__ROM__/files.txt", Program.m_ROMBasePath, Program.m_ROMPatchPath);
            Dictionary<string, Ndst.Folder> folders = new Dictionary<string, Ndst.Folder>();
            Ndst.Filesystem Filesystem = new Ndst.Filesystem();
            folders.Add("", Filesystem);
            List<FileEntry> newFiles = new List<FileEntry>();
            List<DirEntry> newFolders = new List<DirEntry>();
            DirEntry root = new DirEntry();
            root.ID = 0xF000;
            newFolders.Add(root);
            foreach (var s in fileList) {
                AddFileToFilesystem(s);
            }
            m_DirEntries = newFolders.ToArray();
            m_FileEntries = newFiles.ToArray();
            if (arm9R != null) {
                arm9R.Dispose();
                arm9W.Dispose();
            }
            var mo = new MemoryStream(GetExtractedBytes("__ROM__/arm9.bin"));
            arm9R = new BinaryReader(mo);
            arm9W = new BinaryWriter(mo);
            LoadTables();
            UpdateStrings();

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
                fe.InternalID = 0xFFFF;
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
                while (fe.ID >= newFiles.Count) {
                    newFiles.Add(new FileEntry() { Name = "", FullName = "", ID = (ushort)newFiles.Count, InternalID = 0xFFFF });
                }
                newFiles[fe.ID] = fe;

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

        }

        public void SaveArm9() {
            File.WriteAllBytes(Program.m_ROMPatchPath + "/__ROM__/arm9.bin", ((MemoryStream)arm9W.BaseStream).ToArray());
        }

        private static bool BuildingROM = false;
        public static void BuildROM(bool run = false) {
            if (BuildingROM) return;
            BuildingROM = true;
            //Ndst.NinjaBuildSystem.GenerateBuildSystem(Program.m_ROMBasePath, Program.m_ROMPatchPath, Program.m_ROMConversionPath, "Tmp.nds");
            ProcessStartInfo p = new ProcessStartInfo();
            p.CreateNoWindow = true;
            p.WindowStyle = ProcessWindowStyle.Hidden;
            p.FileName = "Ndst";
            p.Arguments = "-n " + Program.m_ROMBasePath + " " + Program.m_ROMPatchPath + " " + Program.m_ROMConversionPath + " Tmp.nds";
            p.UseShellExecute = false;
            p.RedirectStandardOutput = true;
            Process proc = Process.Start(p);
            proc.WaitForExit();

            p = new ProcessStartInfo();
            p.CreateNoWindow = true;
            p.WindowStyle = ProcessWindowStyle.Hidden;
            p.FileName = "ninja";
            p.UseShellExecute = false;
            p.RedirectStandardOutput = true;

            proc = Process.Start(p);
            ProgressDialog pd = new ProgressDialog("Building ROM", 100, null);
            pd.Show();
            proc.EnableRaisingEvents = true;
            proc.OutputDataReceived += OnROMBuildOutput;
            proc.Exited += OnROMBuilt;
            proc.BeginOutputReadLine();

            void OnROMBuildOutput(object sender, DataReceivedEventArgs e) {
                if (e == null || e.Data == null) return;
                int progress = 100;
                if (e.Data.Contains("[")) {
                    try {
                        string[] nums = e.Data.Split('[')[1].Split(']')[0].Split('/');
                        progress = (int)(float.Parse(nums[0]) / float.Parse(nums[1]) * 100);
                    }
                    catch {}
                }
                try { pd.Invoke(new Action(() => pd.UpdateProgress(e.Data, progress))); } catch { }
            }

            void OnROMBuilt(object sender, EventArgs e) {
                if (File.Exists(Program.m_ROMBuildPath)) { File.Delete(Program.m_ROMBuildPath); }
                File.Move("Tmp.nds", Program.m_ROMBuildPath);
                pd.Invoke(new Action(() => pd.OperationDone()));
                BuildingROM = false;
                if (run) Process.Start(Program.m_ROMBuildPath);
            }

        }

        public static void RunROM() {
            BuildROM(true);
        }

        public static FileStream GetExtractedStream(string path) {
            FileStream ret = null;
            if (File.Exists(Program.m_ROMPatchPath + "/" + path)) {
                ret = new FileStream(Program.m_ROMPatchPath + "/" + path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            } else {
                ret = new FileStream(Program.m_ROMBasePath + "/" + path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            return ret;
        }

        public static string GetExtractedLines(string path) {
            if (File.Exists(Program.m_ROMPatchPath + "/" + path)) {
                return File.ReadAllText(Program.m_ROMPatchPath + "/" + path);
            } else {
                return File.ReadAllText(Program.m_ROMBasePath + "/" + path);
            }
        }

        public static void WriteExtractedLines(string path, string lines) {
            Directory.CreateDirectory(Path.GetDirectoryName(Program.m_ROMPatchPath + "/" + path));
            File.WriteAllText(Program.m_ROMPatchPath + "/" + path, lines);
        }

        public static byte[] GetExtractedBytes(string path) {
            if (File.Exists(Program.m_ROMPatchPath + "/" + path)) {
                return File.ReadAllBytes(Program.m_ROMPatchPath + "/" + path);
            } else {
                return File.ReadAllBytes(Program.m_ROMBasePath + "/" + path);
            }
        }

        public static void WriteExtractedBytes(string path, byte[] file) {
            Directory.CreateDirectory(Path.GetDirectoryName(Program.m_ROMPatchPath + "/" + path));
            File.WriteAllBytes(Program.m_ROMPatchPath + "/" + path, file);
        }

        public void LoadTables()
        {
            m_FileTable = new ushort[m_FileTableLength];

            NitroOverlay ovl0 = new NitroOverlay(this, 0);

            // Read table from overlay.
            if (m_FileTableOffset != uint.MaxValue)
            {
                for (uint i = 0; i < m_FileTableLength; i++)
                {
                    if (ovl0.Read32(m_FileTableOffset + (i * 4)) != 0 || m_Version != Version.EUR)
                    {
                        uint str_offset = ovl0.ReadPointer(m_FileTableOffset + (i * 4));
                        string fname = ovl0.ReadString(str_offset, 0);
                        ushort id = GetFileIDFromName(fname);
                        m_FileTable[i] = id;
                        m_FileEntries[id].InternalID = (ushort)i;
                    }
                    else
                        m_FileTable[i] = 0xffff;
                }
            }

            // Read table from an external file.
            else {
                for (int i = 0; i < m_FileTable.Length; i++) {
                    m_FileTable[i] = 0xffff;
                }
                var wholeNames = File.ReadAllLines(Program.m_ROMPatchPath + "/../ASM/Overlays/filenames/filenamesWhole.h");
                int currInd = 0;
                foreach (var line in wholeNames) {
                    if (line.Contains("\"")) {
                        string name = line.Replace(" ", "").Replace("\t", "").Replace("\"", "").Replace(",", "");
                        ushort id = GetFileIDFromName(name);
                        m_FileTable[currInd] = id;
                        m_FileEntries[id].InternalID = (ushort)currInd++;
                    }
                }
            }

            if (Program.m_IsROMFolder) {
                var r = new BinaryReader(new MemoryStream(GetExtractedBytes("__ROM__/arm9.bin")));
                r.BaseStream.Position = m_LevelOvlIDTableOffset;
                m_LevelOvlIDTable = new uint[52];
                for (uint i = 0; i < 52; i++)
                    m_LevelOvlIDTable[i] = r.ReadUInt32();
                r.Dispose();
                return;
            }

            m_FileStream.Position = m_LevelOvlIDTableOffset;
            m_LevelOvlIDTable = new uint[52];
            for (uint i = 0; i < 52; i++)
                m_LevelOvlIDTable[i] = m_BinReader.ReadUInt32();
        }

        public void BeginRW(bool buffered)
        {
            if (Program.m_IsROMFolder) return;
            if (m_CanRW) return;

            m_Buffered = buffered;
            if (m_Buffered)
	        {
                byte[] buf = File.ReadAllBytes(m_Path);
                // indirect way of filling the memorystream
                // initializing it directly would create a non-resizable stream
                m_FileStream = new MemoryStream(buf.Length);
                m_FileStream.Write(buf, 0, buf.Length);
	        }
	        else
                m_FileStream = File.Open(m_Path, FileMode.Open, FileAccess.ReadWrite);

            m_BinReader = new BinaryReader(m_FileStream, Encoding.ASCII);
            m_BinWriter = new BinaryWriter(m_FileStream, Encoding.ASCII);
            m_CanRW = true;
        }

        public void BeginRW() { BeginRW(false); }

        public void EndRW(bool keep)
        {
            if (Program.m_IsROMFolder) return;
            if (!m_CanRW) return;

	        if (m_Buffered)
	        {
                if (keep)
                    this.m_FileStream.Position = 0L;
                    File.WriteAllBytes(m_Path, ((MemoryStream)m_FileStream).ToArray());
		        m_Buffered = false;
	        }

            m_CanRW = false;
            m_FileStream.Close();
        }
        public void EndRW() { EndRW(true); }

        public bool CanRW() {
            if (Program.m_IsROMFolder) return true;
            return m_CanRW;
        }

        private static Dictionary<string, ushort> FileListingNameIds = null;
        private Dictionary<string, ushort> GetExtractedFileNameIds() {
            if (FileListingNameIds != null) return FileListingNameIds;
            var fileList = Ndst.Helper.ReadROMLines("__ROM__/files.txt", Program.m_ROMBasePath, Program.m_ROMPatchPath);
            var ret = new Dictionary<string, ushort>();
            for (int i = 0; i < fileList.Length; i++) {
                var vals = fileList[i].Split(' ');
                ret.Add(vals[0].Substring(3), (ushort)Ndst.Helper.ReadStringNumber(vals[1]));
            }
            FileListingNameIds = ret;
            return ret;
        }

        public ushort GetFileIDFromName(string name)
        {
            if (Program.m_IsROMFolder) {
                var list = GetExtractedFileNameIds();
                if (list.ContainsKey(name)) {
                    return list[name];
                }
                return 0xFFFF;
            }

            foreach (FileEntry fe in m_FileEntries)
            {
                if (fe.FullName == name)
                    return fe.ID;
            }

            return 0xFFFF;
        }
        public ushort GetFileIDFromOverlayID(uint ovlid) { return m_OverlayEntries[ovlid].FileID; } // TODO!!!!!
        public ushort GetFileIDFromInternalID(ushort intid) { return m_FileTable[intid]; }

        // return 0 if does not exist
        public ushort GetDirIDFromName(string name)
        {
            string dirName = name.EndsWith("/") ? name.Substring(0, name.Length - 1) : name;
            foreach (DirEntry de in m_DirEntries)
            {
                if (de.FullName == dirName)
                    return de.ID;
            }

            return 0x0000;
        }

        public bool FileExists(string name)
        {
            ushort id = GetFileIDFromName(name);
            if (id < 0xF000)
                return true;

            string[] narcNames = (m_Version == Version.EUR ? new String[] { "ar1", "arc0", "c2d", "cee", "cef", "ceg", "cei", "ces", "en1", "vs1", "vs2", "vs3", "vs4" }
                : new String[] { "ar1", "arc0", "c2d", "en1", "vs1", "vs2", "vs3", "vs4" });
            foreach (string narcName in narcNames)
            {
                NARC narc = new NARC(this, GetFileIDFromName("ARCHIVE/" + narcName + ".narc"));
                id = narc.GetFileIDFromName(name);
                if (id < 0xF000)
                    return true;
            }

            return false;
        }

        public NitroFile GetFileFromName(string name)
        {
            ushort id = GetFileIDFromName(name);
            if (id < 0xF000)
                return new NitroFile(this, id);

            string[] narcs = (m_Version == Version.EUR ? new String[]{ "ar1", "arc0", "c2d", "cee", "cef", "ceg", "cei", "ces", "en1", "vs1", "vs2", "vs3", "vs4" }
                : new String[] { "ar1", "arc0", "c2d", "en1", "vs1", "vs2", "vs3", "vs4" });
            foreach (string narc in narcs)
            {
                NARC thenarc = new NARC(this, GetFileIDFromName("ARCHIVE/" + narc + ".narc"));
                id = thenarc.GetFileIDFromName(name);
                if (id < 0xF000)
                    return new NARCFile(thenarc, id);
            }

            throw new Exception("NitroROM: cannot find file '" + name + "'");
        }

        public NitroFile GetFileFromInternalID(ushort intid) // TODO!!!
        {
            if (intid >= 0x8000)
            {
                string[] narcs = { "", "", "vs1", "vs2", "vs3", "vs4" };
                ushort narcid = (ushort)(m_Version == Version.EUR ? ((intid >> 10) & 0x1F) : ((intid >> 12) & 0x7));
                ushort fileid = (ushort)(intid & 0x3FF);

                string narcname = "ARCHIVE/" + narcs[narcid] + ".narc";

                return new NARCFile(new NARC(this, GetFileIDFromName(narcname)), fileid);
            }
            return new NitroFile(this, m_FileTable[intid]);
        }

        /*/// <summary>
        /// Get internal ID.
        /// </summary>
        /// <param name="n">Nitro file.</param>
        /// <returnsInternal id.></returns>
        public ushort GetInternalID(NitroFile n) {
            var narc = n as NARCFile;
            if (narc != null) {
                //narc.m_Narc.
            } else {
                return (ushort)m_FileTable.ToList().IndexOf(n.m_ID);
            }
        }*/

        public String GetInternalLevelNameFromID(int id)
        {
            ushort actSelectID = GetActSelectorIdByLevelID(id);
            
            return NameForActSelectID(actSelectID);
        }

        public ushort GetActSelectorIdByLevelID(int id)
        {
            if (Program.m_IsROMFolder) {
                arm9R.BaseStream.Position = Helper.GetActSelectorIDTableAddress() + id - headerSize;
                return arm9R.ReadByte();
            }
            BeginRW();
            ushort actSelectID = Read8((uint)(Helper.GetActSelectorIDTableAddress() + id));
            EndRW();
            return actSelectID;
        }

        public String GetActDescription(int levelID, int actID)
        {
            int actSelectId = GetActSelectorIdByLevelID(levelID);
            if (actSelectId < 16)
                return m_MsgData[0x1B4 + actSelectId * 7 + actID];
            return "Star"+ (actID+1);
        }

        public String NameForActSelectID(int actSelectID)
        {
            if (actSelectID < 29)
            {
                try
                {
                    string ret = m_MsgData[0x196 + actSelectID];
                    while (ret.StartsWith(" ")) {
                        ret = ret.Substring(1);
                    }
                    return ret;
                }
                catch (Exception)
                {
                    return "Name not Found";
                }
            }
            if (actSelectID == 29)
            {
                return "Hub";
            }
            if (actSelectID == 255)
            {
                return "Test Map";
            }
            return actSelectID.ToString();
        }

        public void UpdateStrings()
        {
            NitroFile msgFile;
            try
            {
                msgFile = GetFileFromName("data/message/msg_data_nes.bin");
                Console.WriteLine(" !Any Other!");
            }
            catch (Exception)
            {
                try
                {
                    msgFile = GetFileFromName("data/message/msg_data_eng.bin");
                }
                catch (Exception)
                {
                    Console.WriteLine(" !Not Supported!");
                    return;
                }
            }
            BiDictionaryOneToOne<byte, string> BASIC_EUR_US_CHARS = new BiDictionaryOneToOne<byte, string>();
            Dictionary<string, uint> BASIC_EUR_US_SIZES = new Dictionary<string, uint>();

            TextEditorForm.LoadCharList("assets/basic_eur_us_chars.txt", BASIC_EUR_US_CHARS, BASIC_EUR_US_SIZES);

            // Most of this is copied from TextEditorForm.ReadStrings()!

            uint inf1size = msgFile.Read32(0x24);
            ushort numentries = msgFile.Read16(0x28);

            m_MsgData = new string[numentries];
            
            for (int i = 0; i < m_MsgData.Length; i++)
            {
                uint straddr = msgFile.Read32((uint)(0x30 + i * 8));
                straddr += 0x20 + inf1size + 0x8;

                int length = 0;

                string thetext = "";
                for (; ; )
                {
                    byte cur;
                    try
                    {
                        cur = msgFile.Read8(straddr);
                    }
                    catch
                    {
                        break;
                    }
                    straddr++;
                    length++;
                    char thechar = '\0';
                    
                        if ((cur >= 0x00 && cur <= 0x4F) || (cur >= 0xEE && cur <= 0xFB))
                        {
                            thetext += BASIC_EUR_US_CHARS.GetByFirst(cur);
                            straddr += (BASIC_EUR_US_SIZES[BASIC_EUR_US_CHARS.GetByFirst(cur)] - 1);
                            //length += (int)(BASIC_EUR_US_SIZES[BASIC_EUR_US_CHARS.GetByFirst(cur)] - 1);
                        }
                        //I don't care about these for now
                        /*
                        else if (cur >= 0x50 && cur <= 0xCF)
                        {
                            thetext += EXTENDED_ASCII_CHARS.GetByFirst(cur);
                            straddr += (EXTENDED_ASCII_SIZES[EXTENDED_ASCII_CHARS.GetByFirst(cur)] - 1);
                            length += (int)(EXTENDED_ASCII_SIZES[EXTENDED_ASCII_CHARS.GetByFirst(cur)] - 1);
                        }
                        */

                    if (thechar != '\0')
                        thetext += thechar;
                    else if (cur == 0xFD)
                        thetext += "\r\n";
                    else if (cur == 0xFF)
                        break;
                    else if (cur == 0xFE)// Special Character
                    {
                        int len = msgFile.Read8(straddr);
                        thetext += "[\\r]";
                        thetext += String.Format("{0:X2}", cur);
                        for (int spec = 0; spec < len - 1; spec++)
                        {
                            thetext += String.Format("{0:X2}", msgFile.Read8((uint)(straddr + spec)));
                        }
                        //length += (len - 1);// Already increased by 1 at start
                        straddr += (uint)(len - 1);
                    }
                }

                m_MsgData[i] = thetext;
            }
        }

        public string GetFileNameFromID(ushort id)
        {
            return m_FileEntries[id].FullName;
        }

        public FileEntry[] GetFileEntries()
        {
            return m_FileEntries;
        }

        public DirEntry[] GetDirEntries()
        {
            return m_DirEntries;
        }

        public uint GetOverlayEntryOffset(uint ovlid) { return m_OverlayEntries[ovlid].EntryOffset; }

        public OverlayEntry[] GetOverlayEntries()
        {
            return m_OverlayEntries;
        }

        public byte Read8(uint addr) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; return m_BinReader.ReadByte();
            } else {
                arm9R.BaseStream.Position = addr - headerSize;
                return arm9R.ReadByte();
            }
        }

        public ushort Read16(uint addr) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; return m_BinReader.ReadUInt16();
            } else {
                arm9R.BaseStream.Position = addr - headerSize;
                return arm9R.ReadUInt16();
            }
        }

        public uint Read32(uint addr) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; return m_BinReader.ReadUInt32();
            } else {
                arm9R.BaseStream.Position = addr - headerSize;
                return arm9R.ReadUInt32();
            }
        }

        public byte[] ReadBlock(uint addr, int size) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; return m_BinReader.ReadBytes(size);
            } else {
                arm9R.BaseStream.Position = addr - headerSize;
                return arm9R.ReadBytes(size);
            }
        }

        public void Write8(uint addr, byte value) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; m_BinWriter.Write(value);
            } else {
                arm9W.BaseStream.Position = addr - headerSize;
                arm9W.Write(value);
            }
        }

        public void Write16(uint addr, ushort value) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; m_BinWriter.Write(value);
            } else {
                arm9W.BaseStream.Position = addr - headerSize;
                arm9W.Write(value);
            }
        }

        public void Write32(uint addr, uint value) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; m_BinWriter.Write(value);
            } else {
                arm9W.BaseStream.Position = addr - headerSize;
                arm9W.Write(value);
            }
        }
        public void WriteBlock(uint addr, byte[] data) {
            if (!Program.m_IsROMFolder) {
                m_FileStream.Position = addr; m_BinWriter.Write(data);
            } else {
                arm9W.BaseStream.Position = addr - headerSize;
                arm9W.Write(data);
            }
        }

        public uint GetLevelOverlayID(int levelid) { return m_LevelOvlIDTable[levelid]; }

        public void MakeRoom(uint addr, uint amount)
        {
            if (Program.m_IsROMFolder) {
                return;
            }
            uint actualend = m_UsedSize + ROM_END_MARGIN;
            if (addr < actualend)
            {
                m_FileStream.Position = addr;
                byte[] tomove = m_BinReader.ReadBytes((int)(actualend - addr));
                m_FileStream.Position = addr + amount;
                m_BinWriter.Write(tomove);
            }
        }

        // Not ever called for extracted ROMs.
        private void FixPtrAt(uint addr, uint fixstart, int delta)
        {
            m_FileStream.Position = addr;
	        uint temp = m_BinReader.ReadUInt32();
	        if (temp >= fixstart)
	        {
		        temp += (uint)delta;
		        m_FileStream.Position = addr;
		        m_BinWriter.Write(temp);
	        }
        }

        public void AutoFix(ushort fileid, uint fixstart, int delta)
        {
            if (Program.m_IsROMFolder) {
                return;
            }

        	// fix the internal variables
            if (ARM7Offset >= fixstart) ARM7Offset += (uint)delta;
            if (FNTOffset >= fixstart) FNTOffset += (uint)delta;
            if (FATOffset >= fixstart) FATOffset += (uint)delta;
            if (m_UsedSize >= fixstart) m_UsedSize += (uint)delta;
            else m_UsedSize = (uint)(fixstart + delta);

            for (int i = 0; i < m_FileEntries.Length; i++)
            {
	            if (m_FileEntries[i].ID == fileid)
		            continue;

	            if (m_FileEntries[i].Offset >= fixstart)
		            m_FileEntries[i].Offset += (uint)delta;
            }

            if (OVTOffset >= fixstart)
            {
	            OVTOffset += (uint)delta;

	            for (int i = 0; i < m_OverlayEntries.Length; i++)
		            m_OverlayEntries[i].EntryOffset += (uint)delta;
            }

            // fix the actual ROM
            FixPtrAt(0x20, fixstart, delta); // ARM9 bin offset
            FixPtrAt(0x30, fixstart, delta); // ARM7 bin offset
            FixPtrAt(0x40, fixstart, delta); // Filename table offset
            FixPtrAt(0x48, fixstart, delta); // FAT offset
            FixPtrAt(0x50, fixstart, delta); // ARM9 overlay offset
            FixPtrAt(0x58, fixstart, delta); // ARM7 overlay offset
            FixPtrAt(0x68, fixstart, delta); // Icon/Internal title offset
            //FixPtrAt(0x70, fixstart, delta);
            //FixPtrAt(0x74, fixstart, delta);
            //FixPtrAt(0x80, fixstart, delta);
            FixPtrAt(0x160, fixstart, delta);

            m_FileStream.Position = 0x80;
            m_BinWriter.Write(m_UsedSize);

            for (uint i = 0; i < (FATSize / 8); i++)
            {
	            m_FileStream.Position = FATOffset + (i*8);
	            uint start = m_BinReader.ReadUInt32();
	            uint end = m_BinReader.ReadUInt32();

	            if (i != fileid)
	            {
                    FixPtrAt(FATOffset + (i * 8), fixstart, delta);
	            }

	            if ((start < fixstart) && (end == fixstart) && (i != fileid))
		            continue;

                FixPtrAt(FATOffset + (i * 8) + 4, fixstart, delta);
            }
        }

        public byte[] ExtractFile(ushort fileid)
        {
            if (Program.m_IsROMFolder) {
                NitroFile f = new NitroFile(this, fileid);
                return f.m_Data;
            }
            bool autorw = !m_CanRW;
            if (autorw) BeginRW();

            FileEntry fe = m_FileEntries[fileid];

            m_FileStream.Position = fe.Offset;
            byte[] data = m_BinReader.ReadBytes((int)fe.Size);

            if (autorw) EndRW();
            return data;
        }

        public void ReinsertFile(ushort fileid, byte[] data)
        {
            if (Program.m_IsROMFolder) {
                ReinsertFileOld(fileid, data);
                return;
            }
            if (Program.m_ROM.m_Version != Version.EUR) {
                ReinsertFileOld(fileid, data);
                return;
            }
            if (!this.StartFilesystemEdit())
                return;

            Array.Resize<byte>(ref data, (data.Length + 3) / 4 * 4);
            NitroROM.FileEntry fileEntry = this.m_FileEntries[(int)fileid];
            fileEntry.Data = data;
            this.m_FileEntries[(int)fileid] = fileEntry;
            this.SaveFilesystem();
            this.LoadROM(this.m_Path);
            this.BeginRW();
            this.LoadTables();
            this.EndRW();
        }

        public void ReinsertFileOld(ushort fileid, byte[] data) {
            if (Program.m_IsROMFolder) {
                NitroFile f = new NitroFile(Program.m_ROM, fileid);
                f.m_Data = data;
                f.SaveChanges();
                return;
            }
            bool autorw = !m_CanRW;
            if (autorw) BeginRW();

            int datalength = (data.Length + 3) & ~3;

            FileEntry fe = m_FileEntries[fileid];

            UInt32 fileend = fe.Offset + fe.Size;
            int delta = (int)(datalength - fe.Size);

            // move data that comes after the file
            MakeRoom(fileend, (uint)delta);

            // write the new data for the file
            m_FileStream.Position = fe.Offset;
            m_BinWriter.Write(data);
            fe.Size = (uint)datalength;

            AutoFix(fileid, fileend, delta);

            // fix file sizes
            if (delta != 0) {
                m_FileEntries[fileid].Size = (uint)datalength;

                for (int o = 0; o < m_OverlayEntries.Length; o++) {
                    if (m_OverlayEntries[o].FileID == fileid)
                        m_OverlayEntries[o].RAMSize = (uint)datalength;
                }
            }

            // fix the header CRC16... we never know :P
            // as an example NO$GBA won't load the ROM if this CRC16 is wrong
            ushort hcrc = CalcCRC16(0, 0x15E);
            m_FileStream.Position = 0x15E;
            m_BinWriter.Write(hcrc);
            if (autorw) EndRW();
        }

        public void RewriteSizeTables() {
            m_FileStream.Position = 0x4C;
            m_BinWriter.Write(FATSize);
            m_FileStream.Position = 0x54;
            m_BinWriter.Write(OVTSize);
        }

        public uint AddOverlay(uint ramaddr)
        {
            
            // find an usable overlay ID
	        uint id = 0;
	        foreach (OverlayEntry _oe in m_OverlayEntries)
	        {
		        if (_oe.ID > id)
			        id = _oe.ID;
	        }
	        id++;

            // add a file for the overlay
            ushort fileid = (ushort)(FATSize / 8);

	        MakeRoom(FATOffset + FATSize, 8);
	        AutoFix(0xFFFF, FATOffset + FATSize, 8);

	        FATSize += 8;
	        m_FileStream.Position = 0x4C;
	        m_BinWriter.Write(FATSize);

	        m_FileStream.Position = FATOffset + (fileid * 8);
	        uint fileaddr = m_UsedSize;
	        m_BinWriter.Write(fileaddr);
	        m_BinWriter.Write(fileaddr);

	        Array.Resize(ref m_FileEntries, m_FileEntries.Length+1);
            FileEntry fe;
            fe.ID = fileid;
            fe.InternalID = 0xFFFF;
            fe.ParentID = 0;
            fe.Offset = fileaddr;
            fe.Size = 0;
            fe.Name = fe.FullName = "";
            fe.Data = null;
            m_FileEntries[fileid] = fe;

	        // and add an overlay entry
	        uint entryaddr = OVTOffset + OVTSize;

	        MakeRoom(entryaddr, 0x20);
	        AutoFix(0xFFFF, entryaddr, 0x20);

	        OVTSize += 0x20;
	        m_FileStream.Position = 0x54;
	        m_BinWriter.Write(OVTSize);

	        m_FileStream.Position = entryaddr;
	        m_BinWriter.Write(id);
	        m_BinWriter.Write(ramaddr);
	        m_BinWriter.Write((uint)0);
	        m_BinWriter.Write((uint)0);
	        m_BinWriter.Write(ramaddr);
	        m_BinWriter.Write(ramaddr);
	        m_BinWriter.Write((uint)fileid);
	        m_BinWriter.Write((uint)0);

	        Array.Resize(ref m_OverlayEntries, m_OverlayEntries.Length+1);
            OverlayEntry oe;
            oe.EntryOffset = entryaddr;
            oe.ID = id;
            oe.FileID = fileid;
            oe.RAMAddress = ramaddr;
            oe.RAMSize = 0;
            oe.BSSSize = 0;
            oe.StaticInitStart = ramaddr;
            oe.StaticInitEnd = ramaddr + 4;
            oe.Flags = 0;
	        m_OverlayEntries[id] = oe;

            return id;
        }


        public enum Version
	    {
		    UNK = -1,
		    USA_v1,
		    USA_v2,
            JAP,
		    EUR
	    };
        public Version m_Version;


        public string m_Path { get; private set; }

        private bool m_CanRW;
        private bool m_Buffered;
        private Stream m_FileStream;
        private BinaryReader m_BinReader;
        private BinaryWriter m_BinWriter;

        private uint m_LevelOvlIDTableOffset;
        private uint m_FileTableOffset, m_FileTableLength;
        private ushort[] m_FileTable;
        private uint[] m_LevelOvlIDTable;

        private uint m_UsedSize;

        public uint ARM9RAMAddress { get; private set; }
        public uint ARM7Offset { get; private set; }
        public uint ARM7RAMAddress{ get; private set; }
        public uint ARM7Size { get; private set; }

        private uint FNTOffset, FNTSize;
        internal uint FATOffset, FATSize;
        internal uint OVTOffset, OVTSize;

        public struct DirEntry
        {
            public ushort ID;
            public ushort ParentID;
            public string Name;
            public string FullName;
        }

        public struct FileEntry
        {
            public ushort ID;
            public ushort InternalID;
            public ushort ParentID;
            public string Name;
            public string FullName;
            public uint Offset;
            public uint Size;
            public byte[] Data;
        }

        public struct OverlayEntry
        {
            public uint EntryOffset;
            public uint ID;
            public ushort FileID;
            public uint RAMAddress;
            public uint RAMSize, BSSSize;
            public uint StaticInitStart;
            public uint StaticInitEnd;
            public uint Flags;
        }

        private DirEntry[] m_DirEntries;
        internal FileEntry[] m_FileEntries;
        internal OverlayEntry[] m_OverlayEntries;
    }


    public class INitroROMBlock
    {
        public INitroROMBlock() { }

        public INitroROMBlock(byte[] data)
        {
            m_Data = data;
        }

        public byte Read8(uint addr) { return m_Data[addr]; }
	    public ushort Read16(uint addr) { return (ushort)(m_Data[addr] | (m_Data[addr+1]<<8)); }
	    public uint Read32(uint addr) { return (uint)(m_Data[addr] | (m_Data[addr+1]<<8) | (m_Data[addr+2]<<16) | (m_Data[addr+3]<<24)); }
        public uint ReadVar(uint addr, uint size)
        {
            switch(size)
            {
                case 1: return Read8(addr);
                case 2: return Read16(addr);
                case 4: return Read32(addr);
                default: throw new InvalidDataException("Size must be 1, 2, or 4, not " + size + "!");
            }
        }

	    public byte[] ReadBlock(uint addr, uint len)
	    {
		    byte[] ret = new byte[len];
		    Array.Copy(m_Data, (int)addr, ret, 0, (int)len);
		    return ret;
	    }

	    // reads a string until the specified length or until a null byte
	    // if length is zero, no length limit is applied
	    public string ReadString(uint addr, int len)
	    {
		    string result = "";

		    for (int i = 0; ; i++)
		    {
			    if ((len > 0) && (i >= len)) break;

			    char ch = (char)m_Data[addr + i];
			    if (ch == 0) break;

			    result += ch;
		    }

		    return result;
	    }

        public void Write8(uint addr, byte value) { AutoResize(addr, 1); m_Data[addr] = value; }
        public void Write16(uint addr, ushort value) { AutoResize(addr, 2); m_Data[addr] = (byte)(value & 0xFF); m_Data[addr + 1] = (byte)(value >> 8); }
        public void Write32(uint addr, uint value) { AutoResize(addr, 4); m_Data[addr] = (byte)(value & 0xFF); m_Data[addr + 1] = (byte)((value >> 8) & 0xFF); m_Data[addr + 2] = (byte)((value >> 16) & 0xFF); m_Data[addr + 3] = (byte)(value >> 24); }
        public void WriteVar(uint addr, uint size, uint value)
        {
            switch (size)
            {
                case 1: Write8(addr, (byte)value); break;
                case 2: Write16(addr, (ushort)value); break;
                case 4: Write32(addr, value); break;
                default: throw new InvalidDataException("Size must be 1, 2, or 4, not " + size + "!");
            }
        }

        public void WriteBlock(uint addr, byte[] data)
	    {
		    AutoResize(addr, (uint)data.Length);
		    Array.Copy(data, 0, m_Data, addr, data.Length);
	    }

        public void WriteString(uint addr, string str, int len)
        {
            AutoResize(addr, (uint)((len > 0) ? len : (str.Length + 1)));

            int i = 0;
            for (; ; i++)
            {
                if ((len > 0) && (i >= len)) break;
                if (i >= str.Length) break;

                m_Data[addr + i] = (byte)str[i];
            }

            if ((len == 0) || (i < len))
                m_Data[addr + i] = 0;
        }

        public void RemoveSpace(uint addr, uint size)
        {
            WriteBlock(addr, ReadBlock(addr + size, (uint)m_Data.Length - addr - size));
            Array.Resize(ref m_Data, m_Data.Length - (int)size);
        }
        public void AddSpace(uint addr, uint size)
        {
            //will get auto-resized
            WriteBlock(addr + size, ReadBlock(addr, (uint)m_Data.Length - addr));
            if (size >= 0x80000000u)
                Array.Resize(ref m_Data, m_Data.Length + (int)size);
        }
        public void ResizeSpace(uint addr, uint oldLen, uint newLen)
        {
            if (oldLen == newLen)
                return;
            if (oldLen < newLen)
                AddSpace(addr + oldLen, newLen - oldLen);
            else
                RemoveSpace(addr + newLen, oldLen - newLen);
        }

        public void Clear()
        {
            Array.Resize(ref m_Data, 0);
        }

        private void AutoResize(uint addr, uint size)
	    {
		    if ((addr + size) > m_Data.Length)
			    Array.Resize(ref m_Data, (int)(addr + size));
	    }

        public void FixPtrAt(uint addr, uint fixstart, int delta)
        {
            uint temp = Read32(addr);
            if (temp >= fixstart)
                Write32(addr, temp + (uint)delta);
        }

        // To be implemented by subclasses
        public virtual void SaveChanges() { }

	    public NitroROM m_ROM;
	    public byte[] m_Data;
    }
}
