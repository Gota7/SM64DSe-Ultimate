using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SM64DSe.Patcher {
    
    /// <summary>
    /// Handles ARM 9 binaries.
    /// </summary>
    public class Arm9BinaryHandler {

        /// <summary>
        /// Directory for ASM patch stuff.
        /// </summary>
        public string ASMDir;

        public List<Arm9BinSection> sections;
        Arm9BinSection nullSection;
        NitroFile f;

        public Arm9BinaryHandler() {
            f = Program.m_ROM.GetFileFromName("arm9.bin");
        }

        public void load() {
            decompress();
            loadSections();
        }
        public void newSection(int ramAddr, int ramLen, int fileOffs, int bssSize) {
            Console.Out.WriteLine(String.Format("SECTION {0:X8} - {1:X8} - {2:X8}", ramAddr, ramAddr + ramLen, ramAddr + ramLen + bssSize));

            byte[] data = new byte[ramLen];
            Array.Copy(f.m_Data, fileOffs, data, 0, ramLen);
            Arm9BinSection s = new Arm9BinSection(data, ramAddr, bssSize);
            sections.Add(s);
        }

        public void loadSections() {
            if (isCompressed)
                throw new Exception("Can't load sections of compressed arm9");

            sections = new List<Arm9BinSection>();

            uint copyTableBegin = (uint)(f.Read32(getCodeSettingsOffs() + 0x00) - Program.m_ROM.ARM9RAMAddress);
            int copyTableEnd = (int)(f.Read32(getCodeSettingsOffs() + 0x04) - Program.m_ROM.ARM9RAMAddress);
            int dataBegin = (int)(f.Read32(getCodeSettingsOffs() + 0x08) - Program.m_ROM.ARM9RAMAddress);

            newSection((int)Program.m_ROM.ARM9RAMAddress, dataBegin, 0x0, 0);
            sections[0].real = false;

            while (copyTableBegin < copyTableEnd) {
                int start = (int)f.Read32(copyTableBegin);
                copyTableBegin += 4;
                int size = (int)f.Read32(copyTableBegin);
                copyTableBegin += 4;
                int bsssize = (int)f.Read32(copyTableBegin);
                copyTableBegin += 4;

                newSection(start, size, dataBegin, bsssize);
                dataBegin += size;
            }
        }
        //020985f0 02098620
        public void saveSections() {
            Console.Out.WriteLine("Saving sections...");
            Program.m_ROM.BeginRW();
            MemoryStream o = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(o);
            foreach (Arm9BinSection s in sections) {
                Console.Out.WriteLine(String.Format("{0:X8} - {1:X8} - {2:X8}: {3:X8}",
                    s.ramAddr, s.ramAddr + s.len, s.ramAddr + s.len + s.bssSize, o.Position));

                bw.Write(s.data);
                while (o.Length % 4 != 0) {
                    bw.Write((byte)0);
                }
            }

            uint sectionTableAddr = Program.m_ROM.ARM9RAMAddress + 0xE00;
            MemoryStream o2 = new MemoryStream();
            BinaryWriter bw2 = new BinaryWriter(o2);
            foreach (Arm9BinSection s in sections) {
                if (!s.real) continue;
                if (s.len == 0) continue;
                bw2.Write((uint)s.ramAddr);
                bw2.Write((uint)s.len);
                bw2.Write((uint)s.bssSize);
            }

            //Write BSS sections last
            //because they overwrite huge areas with zeros (?)
            foreach (Arm9BinSection s in sections) {
                if (!s.real) continue;
                if (s.len != 0) continue;
                bw2.Write((uint)s.ramAddr);
                bw2.Write((uint)s.len);
                bw2.Write((uint)s.bssSize);
            }

            byte[] data = o.ToArray();
            byte[] sectionTable = o2.ToArray();
            Array.Copy(sectionTable, 0, data, sectionTableAddr - Program.m_ROM.ARM9RAMAddress, sectionTable.Length);
            f.m_Data = data;
            f.SaveChanges();

            f.Write32(getCodeSettingsOffs() + 0x00, (uint)sectionTableAddr);
            Console.Out.WriteLine(String.Format("{0:X8} {1:X8}", getCodeSettingsOffs() + 0x04, (uint)o2.Position + sectionTableAddr));
            f.Write32(getCodeSettingsOffs() + 0x04, (uint)o2.Position + sectionTableAddr);
            f.Write32(getCodeSettingsOffs() + 0x08, (uint)(sections[0].len + Program.m_ROM.ARM9RAMAddress));

            Console.Out.WriteLine("DONE");
        }

        private int _codeSettingsOffs = -1;

        public uint getCodeSettingsOffs() {
            // Find the end of the settings
            // This old method doesn't work with The Legendary Starfy :\ -Treeki
            //return (int)(f.getUintAt(0x90C) - ROM.arm9RAMAddress);
            if (_codeSettingsOffs == -1) {
                for (uint i = 0; i < 0x8000; i += 4) {
                    if (f.Read32(i) == 0xDEC00621 && f.Read32(i + 4) == 0x2106C0DE) {
                        _codeSettingsOffs = (int)i - 0x1C;
                        break;
                    }
                }
            }

            return (uint)_codeSettingsOffs;
        }


        public int decompressionRamAddr {
            get {
                return (int)f.Read32(getCodeSettingsOffs() + 0x14);
            }

            set {
                f.Write32(getCodeSettingsOffs() + 0x14, (uint)value);
            }
        }

        public bool isCompressed {
            get {
                return decompressionRamAddr != 0;
            }
        }

        public void decompress() {
            /*if (!isCompressed) return;


            int decompressionOffs = decompressionRamAddr - (int)Program.m_ROM.ARM9RAMAddress;

            int compDatSize = (int)(f.Read32((uint)decompressionOffs - 8) & 0xFFFFFF);
            int compDatOffs = decompressionOffs - compDatSize;
            //Console.Out.WriteLine("OFFS: " + compDatOffs.ToString("X"));
            //Console.Out.WriteLine("SIZE: " + compDatSize.ToString("X"));

            byte[] data = f.m_Data;
            byte[] compData = new byte[compDatSize];
            Array.Copy(data, compDatOffs, compData, 0, compDatSize);
            byte[] decompData = Program.m_ROM..DecompressOverlay(compData);
            byte[] newData = new byte[data.Length - compData.Length + decompData.Length];
            Array.Copy(data, newData, data.Length);
            Array.Copy(decompData, 0, newData, compDatOffs, decompData.Length);

            f.m_Data = newData;
            f.SaveChanges();
            decompressionRamAddr = 0;*/
        }



        public void writeToRamAddr(int ramAddr, uint val, int ov, int numBytes = 4) //DY: added numBytes parameter
        {
            /*if (ov != -1) {
                foreach (Overlay of in Program.m_ROM.)
                    if (of.ovId == ov) {
                        if (of.containsRamAddr(ramAddr)) {
                            //Console.Out.WriteLine(String.Format("WRITETO {0:X8} {1:X8}: ov {2:X8}", ramAddr, val, of.ovId));
                            makeBinBackup((int)of.ovId);
                            of.writeToRamAddr(ramAddr, val, numBytes);  //DY: added numBytes parameter
                            return;
                        } else throw new Exception("WRITE: Overlay ID " + ov + " doesn't contain addr " + ramAddr + " :(");
                    }

                throw new Exception("WRITE: Overlay ID " + ov + " not found :(");
            } else {
                foreach (Arm9BinSection s in sections)
                    if (s.containsRamAddr(ramAddr)) {
                        //Console.Out.WriteLine(String.Format("WRITETO {0:X8} {1:X8}: {2:X8}", ramAddr, val, s.ramAddr));
                        makeBinBackup(-1);
                        s.writeToRamAddr(ramAddr, val, numBytes);    //DY: added numBytes parameter
                        return;
                    }
                foreach (Overlay of in ROM.arm9ovs)
                    if (of.containsRamAddr(ramAddr)) {
                        //Console.Out.WriteLine(String.Format("WRITETO {0:X8} {1:X8}: ov {2:X8}", ramAddr, val, of.ovId));
                        makeBinBackup((int)of.ovId);
                        of.writeToRamAddr(ramAddr, val);
                        return;
                    }
            }
            throw new Exception("WRITE: Addr " + ramAddr + " is not in arm9 binary or overlays");*/
        }

        public uint readFromRamAddr(int ramAddr, int ov) {
            /*if (ov != -1) {
                foreach (var of in Program.m_ROM.GetOverlayEntries())
                    if (of.ID == ov) {
                        var o = new NitroOverlay(Program.m_ROM, of.ID);
                        if (of.containsRamAddr(ramAddr))
                            return of.readFromRamAddr(ramAddr);
                        else throw new Exception("READ: Overlay ID " + ov + " doesn't contain addr " + ramAddr + " :(");
                    }

                throw new Exception("READ: Overlay ID " + ov + " not found :(");
            } else {
                foreach (Arm9BinSection s in sections)
                    if (s.containsRamAddr(ramAddr))
                        return s.readFromRamAddr(ramAddr);

                foreach (Overlay of in ROM.arm9ovs)
                    if (of.containsRamAddr(ramAddr))
                        return of.readFromRamAddr(ramAddr);

                throw new Exception("READ: Addr " + ramAddr + " is not in arm9 binary or overlays");
            }*/
            return 0;
        }

        public void makeBinBackup(int file) {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(ASMDir + "/bak");
            //Console.Out.WriteLine("Backing up " + file + " "+dir.FullName);
            if (!dir.Exists)
                dir.Create();

            dir = new System.IO.DirectoryInfo(ASMDir);
            System.IO.FileStream fs;

            string filename;
            if (file == -1)
                filename = dir.FullName + "/bak/" + "main.bin";
            else
                filename = dir.FullName + "/bak/" + file + ".bin";

            if (System.IO.File.Exists(filename)) return;

            fs = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew);

            NitroFile f = Program.m_ROM.GetFileFromName("arm9.bin");
            if (file != -1) {
                NitroOverlay ov = new NitroOverlay(Program.m_ROM, (uint)file);
                ov.SaveChanges();
                fs.Write(ov.m_Data, 0, f.m_Data.Length);
                fs.Close();
            } else {
                fs.Write(f.m_Data, 0, f.m_Data.Length);
                fs.Close();
            }
        }

        public void restoreFromBackup() {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(ASMDir + "/bak");
            if (!dir.Exists) return;

            /*foreach (System.IO.FileInfo f in dir.GetFiles()) {
                string n = f.Name;
                if (!n.EndsWith(".bin")) continue;

                NitroFile ff = null;
                NitroOverlay oo = null;

                n = n.Substring(0, n.Length - 4);
                if (n == "main")
                    ff = Program.m_ROM.GetFileFromName("arm9.bin");
                else {
                    uint num = 0;
                    if (UInt32.TryParse(n, out num)) {
                        oo = new NitroOverlay(Program.m_ROM, num);
                        ff = oo.
                    }
                }

                if (ff == null) continue;

                Console.Out.WriteLine("Restoring " + f + ", " + ff.m_Name);
                System.IO.FileStream fs = f.OpenRead();
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                ff.m_Data = data;
                ff.SaveChanges();
            }*/
        }

    }

}
