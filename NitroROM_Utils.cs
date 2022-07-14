using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe {

    /// <summary>
    /// Utilities for editing the filesystem. Credits to Josh65536.
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

        public void SaveFilesystem() {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binWriter = new BinaryWriter((Stream)memoryStream, Encoding.ASCII);
            List<NitroROM.FileEntry> list1 = ((IEnumerable<NitroROM.FileEntry>)this.m_FileEntries).ToList<NitroROM.FileEntry>();
            list1.RemoveAll((Predicate<NitroROM.FileEntry>)(x => x.Name == ""));
            list1.Sort((Comparison<NitroROM.FileEntry>)((x, y) =>
            {
                if ((int)x.ParentID > (int)y.ParentID)
                    return 1;
                return (int)x.ParentID >= (int)y.ParentID ? string.Compare(x.Name, y.Name) : -1;
            }));
            this.m_BinReader.BaseStream.Position = 0L;
            binWriter.Write(this.m_BinReader.ReadBytes(16384));
            this.m_BinReader.BaseStream.Position = 44L;
            int count = this.m_BinReader.ReadInt32();
            this.m_BinReader.BaseStream.Position = 16384L;
            binWriter.Write(this.m_BinReader.ReadBytes(count));
            int position1 = (int)binWriter.BaseStream.Position;
            binWriter.BaseStream.Position = 48L;
            binWriter.Write(position1);
            this.m_BinReader.BaseStream.Position = 48L;
            int num1 = this.m_BinReader.ReadInt32();
            binWriter.BaseStream.Position = (long)position1;
            this.m_BinReader.BaseStream.Position = (long)num1;
            binWriter.Write(this.m_BinReader.ReadBytes(150308));
            int[] numArray1 = new int[this.m_OverlayEntries.Length + list1.Count];
            int[] numArray2 = new int[this.m_OverlayEntries.Length + list1.Count];
            numArray1[0] = (int)binWriter.BaseStream.Position;
            int length = list1.Max<NitroROM.FileEntry>((Func<NitroROM.FileEntry, int>)(x => x.InternalID != ushort.MaxValue ? (int)x.InternalID : 0)) + 1;
            NitroOverlay nitroOverlay = new NitroOverlay(this, 0U);
            int position2 = (int)binWriter.BaseStream.Position;
            binWriter.Write(nitroOverlay.ReadBlock(0U, 156U));
            binWriter.Write(length);
            binWriter.Write(nitroOverlay.Read32(160U));
            binWriter.Write(uint.MaxValue);
            binWriter.Write(nitroOverlay.ReadBlock(168U, 24U));
            int[] numArray3 = new int[length];
            for (int index = 0; index < list1.Count; ++index) {
                if (list1[index].InternalID != ushort.MaxValue) {
                    numArray3[(int)list1[index].InternalID] = (int)binWriter.BaseStream.Position - position2 + 34251808;
                    binWriter.Write((list1[index].FullName + "\0").ToCharArray());
                    Helper.AlignWriter(binWriter, 4U);
                }
            }
            Helper.WritePosAndRestore(binWriter, (uint)(position2 + 164), (uint)(34251808 - position2));
            foreach (int num2 in numArray3)
                binWriter.Write(num2);
            numArray2[0] = (int)binWriter.BaseStream.Position;
            for (int index = 1; index < this.m_OverlayEntries.Length; ++index) {
                numArray1[index] = (int)binWriter.BaseStream.Position;
                NitroROM.FileEntry fileEntry = this.m_FileEntries[(int)this.m_OverlayEntries[index].FileID];
                if (fileEntry.Data != null) {
                    binWriter.Write(fileEntry.Data);
                } else {
                    this.m_BinReader.BaseStream.Position = (long)fileEntry.Offset;
                    binWriter.Write(this.m_BinReader.ReadBytes((int)fileEntry.Size));
                }
                numArray2[index] = (int)binWriter.BaseStream.Position;
                Helper.AlignWriter(binWriter, 4U);
            }
            Helper.WritePosAndRestore(binWriter, 80U, 0U);
            for (int index = 0; index < this.m_OverlayEntries.Length; ++index) {
                binWriter.Write(index);
                binWriter.Write(this.m_OverlayEntries[index].RAMAddress);
                binWriter.Write(this.m_OverlayEntries[index].RAMSize);
                binWriter.Write(this.m_OverlayEntries[index].BSSSize);
                binWriter.Write(this.m_OverlayEntries[index].StaticInitStart);
                binWriter.Write(this.m_OverlayEntries[index].StaticInitEnd);
                binWriter.Write(index);
                binWriter.Write((uint)((long)this.m_OverlayEntries[index].Flags & (long)~(index == 0 ? 16777216 : 0)));
            }
            for (int index = 0; index < list1.Count; ++index) {
                numArray1[index + this.m_OverlayEntries.Length] = (int)binWriter.BaseStream.Position;
                if (list1[index].Data != null) {
                    binWriter.Write(list1[index].Data);
                    NitroROM.FileEntry fileEntry = list1[index];
                    fileEntry.Data = (byte[])null;
                    list1[index] = fileEntry;
                } else {
                    this.m_BinReader.BaseStream.Position = (long)list1[index].Offset;
                    binWriter.Write(this.m_BinReader.ReadBytes((int)list1[index].Size));
                }
                numArray2[index + this.m_OverlayEntries.Length] = (int)binWriter.BaseStream.Position;
                Helper.AlignWriter(binWriter, 4U);
            }
            int position3 = (int)binWriter.BaseStream.Position;
            binWriter.Write(new byte[8 * this.m_DirEntries.Length]);
            int[] numArray4 = new int[this.m_DirEntries.Length];
            List<NitroROM.DirEntry> list2 = ((IEnumerable<NitroROM.DirEntry>)this.m_DirEntries).ToList<NitroROM.DirEntry>();
            list2.RemoveAt(0);
            list2.Sort((Comparison<NitroROM.DirEntry>)((x, y) =>
            {
                if ((int)x.ParentID > (int)y.ParentID)
                    return 1;
                return (int)x.ParentID >= (int)y.ParentID ? string.Compare(x.Name, y.Name) : -1;
            }));
            int index1 = 0;
            int index2 = 0;
            for (int index3 = 0; index3 < this.m_DirEntries.Length; ++index3) {
                numArray4[index3] = (int)binWriter.BaseStream.Position - position3;
                for (; index1 < list1.Count && (int)list1[index1].ParentID == index3 + 61440; ++index1) {
                    binWriter.Write((byte)list1[index1].Name.Length);
                    binWriter.Write(list1[index1].Name.ToCharArray());
                }
                for (; index2 < list2.Count && (int)list2[index2].ParentID == index3 + 61440; ++index2) {
                    binWriter.Write((byte)(list2[index2].Name.Length + 128));
                    binWriter.Write(list2[index2].Name.ToCharArray());
                    binWriter.Write(list2[index2].ID);
                }
                binWriter.Write((byte)0);
            }
            Helper.AlignWriter(binWriter, 4U);
            int position4 = (int)binWriter.BaseStream.Position;
            binWriter.BaseStream.Position = (long)position3;
            int index4 = 0;
            for (int index3 = 0; index3 < this.m_DirEntries.Length; ++index3) {
                binWriter.Write(numArray4[index3]);
                binWriter.Write((ushort)(index4 + this.m_OverlayEntries.Length));
                if (index3 == 0)
                    binWriter.Write((ushort)this.m_DirEntries.Length);
                else
                    binWriter.Write(this.m_DirEntries[index3].ParentID);
                while (index4 < list1.Count && (int)list1[index4].ParentID == index3 + 61440)
                    ++index4;
            }
            binWriter.BaseStream.Position = (long)position4;
            for (int index3 = 0; index3 < numArray1.Length; ++index3) {
                binWriter.Write(numArray1[index3]);
                binWriter.Write(numArray2[index3]);
            }
            int position5 = (int)binWriter.BaseStream.Position;
            binWriter.BaseStream.Position = 64L;
            binWriter.Write(position3);
            binWriter.Write(position4 - position3);
            binWriter.Write(position4);
            binWriter.Write(position5 - position4);
            binWriter.BaseStream.Position = 20L;
            byte num3 = 0;
            int num4 = position5;
            while (num4 > 131072) {
                num4 >>= 1;
                ++num3;
            }
            binWriter.Write(num3);
            binWriter.BaseStream.Position = 128L;
            binWriter.Write(position5);
            this.m_FileStream.Close();
            this.m_FileStream = (Stream)memoryStream;
            this.m_BinReader = new BinaryReader(this.m_FileStream, Encoding.ASCII);
            this.m_BinWriter = binWriter;
            this.FixCRC16();
            this.AllowEmptySpaceInOv0();
            this.m_BinWriter.BaseStream.SetLength((long)position5);
            this.EndRW(true);
            this.LoadROM(this.m_Path);
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
