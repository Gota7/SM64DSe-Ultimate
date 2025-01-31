using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe {

    /// <summary>
    /// Utilities for editing the filesystem. Credits to HayashiSTL
    /// </summary>
    public partial class NitroROM {

        public bool StartFilesystemEdit() {
            if (this.CanRW()) {
                int num = (int)MessageBox.Show("Error", "The file is being read from or written to by another process in this editor.");
                return false;
            }
            this.BeginRW(true);
            return true;
        }

        public void SaveFilesystem()
        {
            MemoryStream newBinStream = new MemoryStream();
            BinaryWriter newBinWriter = new BinaryWriter(newBinStream, Encoding.ASCII);
            int oldPos;
            List<FileEntry> fileEntries = m_FileEntries.ToList();
            fileEntries.RemoveAll(x => x.Name == ""); //file IDs don't matter; remove the excess space.
            fileEntries.Sort((x, y) => x.ParentID > y.ParentID ? 1 :
                                       x.ParentID < y.ParentID ? -1 :
                                       String.Compare(x.Name, y.Name));

            ////////////
            // HEADER //
            ////////////
            // just copy it for now, no offsets or sizes are known yet
            // We can assume a constant arm9 offset and size because editing the size
            // can break overlays and the bss.
            m_BinReader.BaseStream.Position = 0;
            newBinWriter.Write(m_BinReader.ReadBytes(0x4000));

            //////////
            // ARM9 //
            //////////
            m_BinReader.BaseStream.Position = 0x2c;
            int arm9Size = m_BinReader.ReadInt32(); //includes ITCM
            m_BinReader.BaseStream.Position = 0x4000;
            newBinWriter.Write(m_BinReader.ReadBytes(arm9Size));

            //////////
            // ARM7 //
            //////////
            Helper.AlignWriter(newBinWriter, 0x200); // align arm7 offset to 512-byte boundary

            oldPos = (int)newBinWriter.BaseStream.Position;
            newBinWriter.BaseStream.Position = 0x30;
            newBinWriter.Write(oldPos); // ARM7 offset
            m_BinReader.BaseStream.Position = 0x30;
            int oldArm7Offset = m_BinReader.ReadInt32();

            newBinWriter.BaseStream.Position = oldPos;
            m_BinReader.BaseStream.Position = oldArm7Offset;
            newBinWriter.Write(m_BinReader.ReadBytes(0x24b24)); //also assume a constant arm7 size

            ///////////////
            // Overlay 0 //
            ///////////////
            m_OverlayEntries[0].Flags &= 0xfeffffff;// overlay 0 is now decompressed

            int[] newFileOffsets = new int[m_OverlayEntries.Length + fileEntries.Count];
            int[] newFileEndOffsets = new int[m_OverlayEntries.Length + fileEntries.Count];
            newFileOffsets[0] = (int)newBinWriter.BaseStream.Position;

            int numOverlay0IDs = fileEntries.Max(x => x.InternalID == 0xffff ? 0 : x.InternalID) + 1; //overlay 0 IDs DO matter.

            NitroOverlay overlay = new NitroOverlay(this, 0);
            oldPos = (int)newBinWriter.BaseStream.Position;
            newBinWriter.Write(overlay.ReadBlock(0, 0x9c));
            newBinWriter.Write(numOverlay0IDs);
            newBinWriter.Write(overlay.Read32(0xa0));
            newBinWriter.Write(0xffffffff); //supposed to be the address of the entries; not known yet.
            newBinWriter.Write(overlay.ReadBlock(0xa8, 0x18));

            int[] ov0FileNameOffsets = new int[numOverlay0IDs];
            for (int i = 0; i < fileEntries.Count; ++i)
            {
                if (fileEntries[i].InternalID == 0xffff)
                    continue;

                ov0FileNameOffsets[fileEntries[i].InternalID] =
                    (int)newBinWriter.BaseStream.Position - oldPos + 0x020aa420;
                newBinWriter.Write((fileEntries[i].FullName + '\0').ToCharArray());
                Helper.AlignWriter(newBinWriter, 4);
            }
            Helper.WritePosAndRestore(newBinWriter, (uint)(oldPos + 0xa4), (uint)(0x020aa420 - oldPos)); //now said address is known
            foreach (int offset in ov0FileNameOffsets)
                newBinWriter.Write(offset);

            newFileEndOffsets[0] = (int)newBinWriter.BaseStream.Position;

            ////////////////////
            // Other Overlays //
            ////////////////////
            for (int i = 1; i < m_OverlayEntries.Length; ++i)
            {
                newFileOffsets[i] = (int)newBinWriter.BaseStream.Position;
                FileEntry fEntry = m_FileEntries[m_OverlayEntries[i].FileID];
                if (fEntry.Data != null)
                    newBinWriter.Write(fEntry.Data);
                else
                {
                    m_BinReader.BaseStream.Position = fEntry.Offset;
                    newBinWriter.Write(m_BinReader.ReadBytes((int)fEntry.Size));
                }
                newFileEndOffsets[i] = (int)newBinWriter.BaseStream.Position;
                Helper.AlignWriter(newBinWriter, 4);
            }

            Helper.WritePosAndRestore(newBinWriter, 0x50, 0);
            for (int i = 0; i < m_OverlayEntries.Length; ++i)
            {
                newBinWriter.Write(i);
                newBinWriter.Write(m_OverlayEntries[i].RAMAddress);
                newBinWriter.Write(m_OverlayEntries[i].RAMSize);
                newBinWriter.Write(m_OverlayEntries[i].BSSSize);
                newBinWriter.Write(m_OverlayEntries[i].StaticInitStart);
                newBinWriter.Write(m_OverlayEntries[i].StaticInitEnd);
                newBinWriter.Write(i); //assign file IDs
                newBinWriter.Write(m_OverlayEntries[i].Flags);
            }

            ///////////
            // Files //
            ///////////
            for (int i = 0; i < fileEntries.Count; ++i)
            {
                newFileOffsets[i + m_OverlayEntries.Length] = (int)newBinWriter.BaseStream.Position;
                if (fileEntries[i].Data != null)
                {
                    newBinWriter.Write(fileEntries[i].Data);
                    FileEntry file = fileEntries[i];
                    file.Data = null;
                    fileEntries[i] = file;
                }
                else
                {
                    m_BinReader.BaseStream.Position = fileEntries[i].Offset;
                    newBinWriter.Write(m_BinReader.ReadBytes((int)fileEntries[i].Size));
                }
                newFileEndOffsets[i + m_OverlayEntries.Length] = (int)newBinWriter.BaseStream.Position;
                Helper.AlignWriter(newBinWriter, 4);
            }

            ////////////
            // Banner //
            ////////////
            m_BinReader.BaseStream.Position = 0x68;
            uint bannerOffset = m_BinReader.ReadUInt32();

            Helper.WritePosAndRestore(newBinWriter, 0x68, 0);

            m_BinReader.BaseStream.Position = bannerOffset;
            newBinWriter.Write(m_BinReader.ReadBytes(0xa00));

            /////////////////////
            // File Name Table //
            /////////////////////
            int newFNTOffset = (int)newBinWriter.BaseStream.Position;
            newBinWriter.Write(new byte[8 * m_DirEntries.Length]); // offsets to subtables are not known yet

            int[] subTableOffsets = new int[m_DirEntries.Length];
            List<DirEntry> sortedDirs = m_DirEntries.ToList();
            sortedDirs.RemoveAt(0); //the root directory is not named
            sortedDirs.Sort((x, y) => x.ParentID > y.ParentID ? 1 :
                                      x.ParentID < y.ParentID ? -1 :
                                      string.Compare(x.Name, y.Name));

            int fileListIter = 0, dirListIter = 0; //iterates by parent ID
            for (int i = 0; i < m_DirEntries.Length; ++i)
            {
                subTableOffsets[i] = (int)newBinWriter.BaseStream.Position - newFNTOffset;
                while (fileListIter < fileEntries.Count && fileEntries[fileListIter].ParentID == i + 0xf000)
                {
                    newBinWriter.Write((byte)fileEntries[fileListIter].Name.Length);
                    newBinWriter.Write(fileEntries[fileListIter].Name.ToCharArray());
                    ++fileListIter;
                }
                while (dirListIter < sortedDirs.Count && sortedDirs[dirListIter].ParentID == i + 0xf000)
                {
                    newBinWriter.Write((byte)(sortedDirs[dirListIter].Name.Length + 0x80));
                    newBinWriter.Write(sortedDirs[dirListIter].Name.ToCharArray());
                    newBinWriter.Write(sortedDirs[dirListIter].ID);
                    ++dirListIter;
                }
                newBinWriter.Write((byte)0);
            }
            Helper.AlignWriter(newBinWriter, 4);
            int newFATOffset = (int)newBinWriter.BaseStream.Position;
            newBinWriter.BaseStream.Position = newFNTOffset;

            fileListIter = 0;
            for (int i = 0; i < m_DirEntries.Length; ++i)
            {
                newBinWriter.Write(subTableOffsets[i]);
                newBinWriter.Write((ushort)(fileListIter + m_OverlayEntries.Length));
                if (i == 0)
                    newBinWriter.Write((ushort)m_DirEntries.Length);
                else
                    newBinWriter.Write(m_DirEntries[i].ParentID);
                while (fileListIter < fileEntries.Count && fileEntries[fileListIter].ParentID == i + 0xf000)
                    ++fileListIter;
            }

            ///////////////////////////
            // File Allocation Table //
            ///////////////////////////
            newBinWriter.BaseStream.Position = newFATOffset;
            for (int i = 0; i < newFileOffsets.Length; ++i)
            {
                newBinWriter.Write(newFileOffsets[i]);
                newBinWriter.Write(newFileEndOffsets[i]);
            }

            int endOfFile = (int)newBinWriter.BaseStream.Position;
            newBinWriter.BaseStream.Position = 0x40;
            newBinWriter.Write(newFNTOffset);
            newBinWriter.Write(newFATOffset - newFNTOffset);
            newBinWriter.Write(newFATOffset);
            newBinWriter.Write(endOfFile - newFATOffset);

            newBinWriter.BaseStream.Position = 0x14;
            byte deviceCapacity = 0;
            { //This is it's own block to hide the shameless use of a boringly-named variable.
                int num = endOfFile;
                while (num > 0x20000 /*128 KB*/)
                {
                    num >>= 1;
                    ++deviceCapacity;
                }
            }
            newBinWriter.Write(deviceCapacity);
            newBinWriter.BaseStream.Position = 0x80;
            newBinWriter.Write(endOfFile);

            //this is the proper way to copy a new stream back to the base stream:)
            m_FileStream.Close();
            m_FileStream = newBinStream;
            m_BinReader = new BinaryReader(m_FileStream, Encoding.ASCII);
            m_BinWriter = newBinWriter;

            FixCRC16();
            AllowEmptySpaceInOv0();

            m_BinWriter.BaseStream.SetLength(endOfFile);
            EndRW(true);
            LoadROM(m_Path);
        }

        public void RevertFilesystem() {
            this.EndRW(false);
        }

        public void RenameNode(string oldName, string newFullName, TreeNode currNode) {
            currNode.Tag = (object)(newFullName + ((string)currNode.Tag).Substring(oldName.Length));
            foreach (TreeNode node in currNode.Nodes)
                this.RenameNode(oldName, newFullName, node);
        }

        public void RenameFile(string filename, string newName, TreeNode root) {
            int fileIdFromName = (int)this.GetFileIDFromName(filename);
            if (fileIdFromName == (int)ushort.MaxValue) {
                int num1 = (int)MessageBox.Show("This is a bug and shouldn't happen.", "Sorry,");
            } else if (fileIdFromName >= 32768) {
                int num2 = (int)MessageBox.Show("Manipulation of archives not supported", "Sorry,");
            } else {
                int length = filename.LastIndexOf('/') + 1;
                string str1 = filename.Substring(0, length) + newName;
                this.m_FileEntries[fileIdFromName].Name = newName;
                this.m_FileEntries[fileIdFromName].FullName = str1;
                TreeNode fileOrDir = ROMFileSelect.GetFileOrDir(filename, root);
                string str2;
                string str3 = str2 = newName;
                fileOrDir.Text = str2;
                fileOrDir.Name = str3;
                fileOrDir.Tag = (object)str1;
            }
        }

        public void RenameDir(string dirname, string newName, TreeNode root) {
            int index1 = (int)this.GetDirIDFromName(dirname) - 61440;
            if (dirname.StartsWith("ARCHIVE")) {
                int num = (int)MessageBox.Show("Manipulation of archives not supported", "Sorry,");
            } else {
                int length = dirname.LastIndexOf('/') + 1;
                string newFullName = dirname.Substring(0, length) + newName;
                this.m_DirEntries[index1].Name = newName;
                for (int index2 = 0; index2 < this.m_DirEntries.Length; ++index2) {
                    if (this.m_DirEntries[index2].FullName.StartsWith(dirname))
                        this.m_DirEntries[index2].FullName = newFullName + this.m_DirEntries[index2].FullName.Substring(dirname.Length);
                }
                for (int index2 = 0; index2 < this.m_FileEntries.Length; ++index2) {
                    if (this.m_FileEntries[index2].FullName.StartsWith(dirname))
                        this.m_FileEntries[index2].FullName = newFullName + this.m_FileEntries[index2].FullName.Substring(dirname.Length);
                }
                TreeNode fileOrDir = ROMFileSelect.GetFileOrDir(dirname, root);
                fileOrDir.Name = fileOrDir.Text = newName;
                this.RenameNode(dirname, newFullName, fileOrDir);
            }
        }

        public void RemoveDir(string dirname, TreeNode root) {
            if (dirname.StartsWith("ARCHIVE")) {
                int num = (int)MessageBox.Show("Manipulation of archives not supported", "Sorry,");
            } else {
                List<int> fileIDs = new List<int>();
                List<int> dirIDs = new List<int>();
                for (int index = 0; index < this.m_FileEntries.Length; ++index) {
                    if (this.m_FileEntries[index].FullName.StartsWith(dirname))
                        fileIDs.Add(index);
                }
                for (int index = 0; index < this.m_DirEntries.Length; ++index) {
                    if (this.m_DirEntries[index].FullName.StartsWith(dirname))
                        dirIDs.Add(index);
                }
                this.RemoveFileEntriesAndCorrectIDs(fileIDs);
                this.RemoveDirEntriesAndCorrectIDs(dirIDs);
                ROMFileSelect.GetFileOrDir(dirname, root).Remove();
            }
        }

        private void RemoveDirEntriesAndCorrectIDs(List<int> dirIDs) {
            dirIDs.Sort();
            int index1 = 0;
            List<int> intList = new List<int>(this.m_DirEntries.Length);
            for (int index2 = 0; index2 < this.m_DirEntries.Length; ++index2) {
                intList.Add(index1);
                if (index1 < dirIDs.Count && index2 == dirIDs[index1]) {
                    ++index1;
                } else {
                    NitroROM.DirEntry dirEntry = this.m_DirEntries[index2];
                    dirEntry.ID -= (ushort)index1;
                    this.m_DirEntries[index2 - index1] = dirEntry;
                }
            }
            Array.Resize<NitroROM.DirEntry>(ref this.m_DirEntries, this.m_DirEntries.Length - index1);
            for (int index2 = 1; index2 < this.m_DirEntries.Length; ++index2) {
                NitroROM.DirEntry dirEntry = this.m_DirEntries[index2];
                dirEntry.ParentID -= (ushort)intList[(int)dirEntry.ParentID - 61440];
                this.m_DirEntries[index2] = dirEntry;
            }
            for (int index2 = 0; index2 < this.m_FileEntries.Length; ++index2) {
                if (this.m_FileEntries[index2].ParentID != (ushort)0) {
                    NitroROM.FileEntry fileEntry = this.m_FileEntries[index2];
                    fileEntry.ParentID -= (ushort)intList[(int)fileEntry.ParentID - 61440];
                    this.m_FileEntries[index2] = fileEntry;
                }
            }
        }

        private void RemoveFileEntriesAndCorrectIDs(List<int> fileIDs) {
            fileIDs.Sort();
            if (fileIDs.Count == 0)
                return;
            int index = 0;
            for (int fileId = fileIDs[0]; fileId < this.m_FileEntries.Length; ++fileId) {
                if (index < fileIDs.Count && fileId == fileIDs[index]) {
                    ++index;
                } else {
                    NitroROM.FileEntry fileEntry = this.m_FileEntries[fileId];
                    fileEntry.ID -= (ushort)index;
                    this.m_FileEntries[fileId - index] = fileEntry;
                }
            }
            Array.Resize<NitroROM.FileEntry>(ref this.m_FileEntries, this.m_FileEntries.Length - index);
        }

        public void RemoveFile(string filename, TreeNode root) {
            int fileIdFromName = (int)this.GetFileIDFromName(filename);
            if (fileIdFromName == (int)ushort.MaxValue) {
                int num1 = (int)MessageBox.Show("This is a bug and shouldn't happen.", "Sorry,");
            } else if (fileIdFromName >= 32768) {
                int num2 = (int)MessageBox.Show("Manipulation of archives not supported", "Sorry,");
            } else {
                this.AllowEmptySpaceInOv0();
                this.RemoveFileEntriesAndCorrectIDs(new List<int>()
                {
          fileIdFromName
        });
                ROMFileSelect.GetFileOrDir(filename, root).Remove();
            }
        }

        public void AddDir(string path, string newName, TreeNode root) {
            if (path.StartsWith("ARCHIVE")) {
                int num = (int)MessageBox.Show("Manipulation of archives not supported", "Sorry,");
            } else {
                ushort dirIdFromName = this.GetDirIDFromName(path.TrimEnd('/'));
                Array.Resize<NitroROM.DirEntry>(ref this.m_DirEntries, this.m_DirEntries.Length + 1);
                NitroROM.DirEntry dirEntry;
                dirEntry.ID = (ushort)(this.m_DirEntries.Length - 1 + 61440);
                dirEntry.ParentID = dirIdFromName;
                dirEntry.Name = newName;
                dirEntry.FullName = path + newName;
                this.m_DirEntries[this.m_DirEntries.Length - 1] = dirEntry;
                TreeNode treeNode = ROMFileSelect.GetFileOrDir(path.TrimEnd('/'), root).Nodes.Add(newName, newName);
                treeNode.Tag = (object)(path + newName + "/");
                treeNode.EnsureVisible();
                treeNode.TreeView.SelectedNode = treeNode;
            }
        }

        private int GetFirstOv0Space() {
            List<ushort> list = ((IEnumerable<NitroROM.FileEntry>)this.m_FileEntries).Select<NitroROM.FileEntry, ushort>((Func<NitroROM.FileEntry, ushort>)(x => x.InternalID)).ToList<ushort>();
            list.Sort();
            for (int index = 0; index < list.Count; ++index) {
                if ((int)list[index] != index)
                    return index;
            }
            return list.Count;
        }

        private void AddFileEntriesAndCorrectIDs(
          string path,
          List<string> filenames,
          List<string> fullNames) {
            string dirname = path.TrimEnd('/');
            uint num1 = (uint)this.GetDirIDFromName(dirname) - 61440U;
            int num2 = Array.FindLastIndex<NitroROM.FileEntry>(this.m_FileEntries, (Predicate<NitroROM.FileEntry>)(x => x.FullName.StartsWith(dirname))) + 1;
            if (num2 == 0)
                num2 = this.m_FileEntries.Length;
            Array.Resize<NitroROM.FileEntry>(ref this.m_FileEntries, this.m_FileEntries.Length + fullNames.Count);
            for (int index = this.m_FileEntries.Length - fullNames.Count - 1; index >= num2; --index) {
                NitroROM.FileEntry fileEntry = this.m_FileEntries[index];
                fileEntry.ID += (ushort)fullNames.Count;
                this.m_FileEntries[index + fullNames.Count] = fileEntry;
            }
            for (int index = num2; index < num2 + fullNames.Count; ++index) {
                NitroROM.FileEntry fileEntry = this.m_FileEntries[index];
                fileEntry.InternalID = ushort.MaxValue;
                this.m_FileEntries[index] = fileEntry;
            }
            for (int index = 0; index < fullNames.Count; ++index) {
                NitroROM.FileEntry fileEntry;
                fileEntry.ID = (ushort)(num2 + index);
                fileEntry.InternalID = (ushort)this.GetFirstOv0Space();
                fileEntry.Name = filenames[index];
                fileEntry.FullName = path + filenames[index];
                fileEntry.ParentID = (ushort)(num1 + 61440U);
                fileEntry.Data = File.ReadAllBytes(fullNames[index]);
                fileEntry.Offset = uint.MaxValue;
                fileEntry.Size = (uint)fileEntry.Data.Length;
                this.m_FileEntries[num2 + index] = fileEntry;
            }
        }

        public void AddFile(
          string path,
          List<string> filenames,
          List<string> fullNames,
          TreeNode root) {
            if (path.StartsWith("ARCHIVE")) {
                int num1 = (int)MessageBox.Show("Manipulation of archives not supported", "Sorry,");
            } else {
                try {
                    for (int index = 0; index < filenames.Count; ++index) {
                        if (filenames[index].Length >= 128)
                            throw new InvalidDataException("File name \"" + filenames[index] + "\" is too long.\nMake it less than 128 letters long.");
                        using (FileStream fileStream = File.OpenRead(fullNames[index])) {
                            if (fileStream.Length == 0L)
                                throw new InvalidDataException("The file \"" + fullNames[index] + "\" is empty.");
                        }
                    }
                } catch (Exception ex) {
                    int num2 = (int)MessageBox.Show(ex.Message, "File cannot be read");
                    return;
                }
                this.AddFileEntriesAndCorrectIDs(path, filenames, fullNames);
                TreeNode fileOrDir = ROMFileSelect.GetFileOrDir(path.TrimEnd('/'), root);
                for (int index = 0; index < filenames.Count; ++index) {
                    TreeNode treeNode = fileOrDir.Nodes.Add(filenames[index], filenames[index]);
                    treeNode.Tag = (object)(path + filenames[index]);
                    if (index == filenames.Count - 1) {
                        treeNode.EnsureVisible();
                        treeNode.TreeView.SelectedNode = treeNode;
                    }
                }
            }
        }

        public void AddSPA(List<string> filenames, List<string> fullNames, TreeNode root)
        {
            string path = "data";
            this.AddFileEntriesAndCorrectIDs(path, filenames, fullNames);
            TreeNode fileOrDir = ROMFileSelect.GetFileOrDir(path.TrimEnd('/'), root);
            for (int index = 0; index < filenames.Count; ++index)
            {
                TreeNode treeNode = fileOrDir.Nodes.Add(filenames[index], filenames[index]);
                treeNode.Tag = (object)(path + filenames[index]);
                if (index == filenames.Count - 1)
                {
                    treeNode.EnsureVisible();
                    treeNode.TreeView.SelectedNode = treeNode;
                }
            }
        }

        private void AddFileEntriesAndCorrectIDs(string path, List<string> filenames, List<string> fullNames, byte[] data)
        {
            string dirname = path.TrimEnd('/');

            uint dirID = GetDirIDFromName(dirname) - 0xf000u;
            int whereToInsert = Array.FindLastIndex(m_FileEntries, x => x.FullName.StartsWith(path)) + 1;
            if (whereToInsert == 0)
                whereToInsert = m_FileEntries.Length;

            Array.Resize(ref m_FileEntries, m_FileEntries.Length + fullNames.Count);
            for (int i = m_FileEntries.Length - fullNames.Count - 1; i >= whereToInsert; --i)
            {
                FileEntry file = m_FileEntries[i];
                file.ID += (ushort)fullNames.Count;
                m_FileEntries[i + fullNames.Count] = file;
            }

            //make sure valid ov0FileIDs are unique; GetFirstOv0Space relies on it
            for (int i = whereToInsert; i < whereToInsert + fullNames.Count; ++i)
            {
                FileEntry file = m_FileEntries[i];
                file.InternalID = 0xffff;
                m_FileEntries[i] = file;
            }
            for (int i = 0; i < fullNames.Count; ++i)
            {
                FileEntry file;
                file.ID = (ushort)(whereToInsert + i);
                file.InternalID = (ushort)GetFirstOv0Space();
                file.Name = filenames[i];
                file.FullName = path + filenames[i];
                file.ParentID = (ushort)(dirID + 0xf000);
                file.Data = data;
                file.Offset = 0xffffffff;
                file.Size = (uint)file.Data.Length;
                m_FileEntries[whereToInsert + i] = file;
            }
        }

        public void AddFile(string path, string filename, byte[] filedata, TreeNode root)
        {
            if (path.StartsWith("ARCHIVE"))
                throw new Exception("Manipulation of archives not supported.");

            try
            {
                if (filename.Length >= 128)
                    throw new InvalidDataException("File name \"" + filename + "\" is too long.\n" +
                        "Make it less than 128 letters long.");
            }
            catch (Exception e)
            {
                throw new Exception("File cannot be added:\n" + e.Message);
            }

            AddFileEntriesAndCorrectIDs(path, new List<string>(new string[] { filename }), new List<string>(new string[] { filename }), filedata);
            TreeNode node = ROMFileSelect.GetFileOrDir(path.TrimEnd('/'), root);
            TreeNode newNode = node.Nodes.Add(filename, filename);
            newNode.Tag = path + filename;
            newNode.EnsureVisible();
            newNode.TreeView.SelectedNode = newNode;
        }

        public void FixCRC16() {
            int num1 = !this.m_CanRW ? 1 : 0;
            if (num1 != 0)
                this.BeginRW();
            ushort num2 = this.CalcCRC16(0U, 350U);
            this.m_FileStream.Position = 350L;
            this.m_BinWriter.Write(num2);
            if (num1 == 0)
                return;
            this.EndRW();
        }

        private void AllowEmptySpaceInOv0() {
            this.m_FileStream.Position = 100856L;
            this.m_BinWriter.Write(3786412032U);
            this.m_FileStream.Position = 100864L;
            this.m_BinWriter.Write(452985101);
        }

    }

}
