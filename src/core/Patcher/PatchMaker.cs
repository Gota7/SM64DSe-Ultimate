/* 
 * Adopted from NSMBe's patch maker
 */ 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SM64DSe.core.utils.SP2;

namespace SM64DSe.Patcher
{
    public class PatchMaker
    {
        Arm9BinaryHandler handler;
        DirectoryInfo romdir;
        uint m_CodeAddr;

        const uint baseAddress = 0x02400000;

        public PatchMaker(DirectoryInfo romdir, uint codeAddr)
        {
            //handler = new Arm9BinaryHandler();
            this.romdir = romdir;
            m_CodeAddr = codeAddr;
        }
		
        public void compilePatch()
        {
            PatchCompiler.compilePatch(m_CodeAddr, romdir);
        }

        public void alignStream(Stream stream, int modulus)
        {
            byte[] zero = { 0x00 };
            while (stream.Position % modulus != 0)
                stream.Write(zero, 0, 1);
        }

        public static bool PatchToSupportBigASMHacks()
        {
            bool autorw = Program.m_ROM.CanRW();
            if (!autorw) Program.m_ROM.BeginRW();
            bool hasPatch = false;
            if (Program.m_IsROMFolder) {
                Program.m_ROM.arm9R.BaseStream.Position = 0x6590 - Program.m_ROM.headerSize;
                hasPatch = Program.m_ROM.arm9R.ReadUInt32() != 0;
            } else {
                hasPatch = Program.m_ROM.Read32(0x6590) != 0;
            }
            if (hasPatch) //the patch makes this not 0
            {
                if (!autorw) Program.m_ROM.EndRW();
                return true;
            }
            if (!autorw) Program.m_ROM.EndRW();

            if (MessageBox.Show("This requires the ROM to be further patched. " +
                "Continue with the patch?", "Table Shifting Patch", MessageBoxButtons.YesNo) == DialogResult.No)
                return false;

            if (!autorw) Program.m_ROM.BeginRW();
            NitroOverlay ov2 = new NitroOverlay(Program.m_ROM, 2);

            if (Program.m_IsROMFolder) {

                Program.m_ROM.arm9R.BaseStream.Position = 0x90864 - Program.m_ROM.headerSize;
                byte[] actorTable = Program.m_ROM.arm9R.ReadBytes(0x61c);

                Program.m_ROM.arm9W.BaseStream.Position = 0x6590 - Program.m_ROM.headerSize;
                Program.m_ROM.arm9W.Write(actorTable);

                Program.m_ROM.arm9W.BaseStream.Position = 0x90864 - Program.m_ROM.headerSize;
                Program.m_ROM.arm9W.Write(new byte[0x61c]);

                Program.m_ROM.arm9W.BaseStream.Position = 0x1a198 - Program.m_ROM.headerSize;
                Program.m_ROM.arm9W.Write((uint)0x02006590);

                // TODO!!!

                return true;

            }

            //Move the ACTOR_SPAWN_TABLE so it can expand
            Program.m_ROM.WriteBlock(0x6590, Program.m_ROM.ReadBlock(0x90864, 0x61c));
            Program.m_ROM.WriteBlock(0x90864, new byte[0x61c]);

            //Adjust pointers
            Program.m_ROM.Write32(0x1a198, 0x02006590);

            //Move the OBJ_TO_ACTOR_TABLE so it can expand
            Program.m_ROM.WriteBlock(0x4b00, ov2.ReadBlock(0x0210cbf4 - ov2.GetRAMAddr(), 0x28c));
            ov2.WriteBlock(0x0210cbf4 - ov2.GetRAMAddr(), new byte[0x28c]);

            //Adjust pointers
            ov2.Write32(0x020fe890 - ov2.GetRAMAddr(), 0x02004b00);
            ov2.Write32(0x020fe958 - ov2.GetRAMAddr(), 0x02004b00);
            ov2.Write32(0x020fea44 - ov2.GetRAMAddr(), 0x02004b00);

            //Add the dynamic library loading and cleanup code
            Program.m_ROM.WriteBlock(0x90864, Properties.Resources.dynamic_library_loader);

            //Add the hooks (by replacing LoadObjBankOverlays())
            Program.m_ROM.WriteBlock(0x2df70, Properties.Resources.static_overlay_loader);
            
            if (!autorw) Program.m_ROM.EndRW();
            ov2.SaveChanges();

            return true;
        }

        public void makeOverlay(uint overlayID)
        {
            FileInfo f = new FileInfo(romdir.FullName + "/newcode.bin");
            if (!f.Exists) return;
            FileStream fs = f.OpenRead();
            FileInfo symFile = new FileInfo(romdir.FullName + "/newcode.sym");
            StreamReader symStr = symFile.OpenText();

            byte[] newdata = new byte[fs.Length];
            fs.Read(newdata, 0, (int)fs.Length);
            fs.Close();


            BinaryWriter newOvl = new BinaryWriter(new MemoryStream());
            BinaryReader newOvlR = new BinaryReader(newOvl.BaseStream);

            try
            {
                newOvl.Write(newdata);
                alignStream(newOvl.BaseStream, 4);

                uint staticInitCount = 0;

                while (!symStr.EndOfStream)
                {
                    string line = symStr.ReadLine();

                    if (line.Contains("_Z4initv")) //gcc name mangling of init()
                    {
                        uint addr = (uint)parseHex(line.Substring(0, 8));
                        newOvl.Write(addr);
                        ++staticInitCount;
                    }
                }

                /*if (newOvl.BaseStream.Length > 0x4d20)
                    throw new InvalidDataException
                        ("The overlay must have no more than 19776 bytes; this one will have " + newOvl.BaseStream.Length);*/

                NitroOverlay ovl = new NitroOverlay(Program.m_ROM, overlayID);
                newOvl.BaseStream.Position = 0;
                ovl.SetInitializer(ovl.GetRAMAddr() + (uint)newOvl.BaseStream.Length - 4 * staticInitCount,
                    4 * staticInitCount);
                ovl.SetSize((uint)newOvl.BaseStream.Length);
                ovl.WriteBlock(0, newOvlR.ReadBytes((int)newOvl.BaseStream.Length));
                ovl.SaveChanges();
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox("Error", ex).ShowDialog();
                return;
            }
            finally
            {
                symStr.Close();
                newOvl.Dispose();
                newOvlR.Close();
            }
        }

        private (uint, uint)? getInitAndCleanup(string symFileName)
        {
            StreamReader symbolFile = null;
            uint initFuncOffset  = 0;
            uint cleanFuncOffset = 0;

            try
            {
                symbolFile = new StreamReader(new FileStream($"{romdir.FullName}/{symFileName}", FileMode.Open));

                while (!symbolFile.EndOfStream)
                {
                    string line = symbolFile.ReadLine();

                    if (line.Length < 32)
                        continue;
                    
                    string symbol = line.Substring(31);

                    if (symbol == " _Z4initv")
                    {
                        initFuncOffset = uint.Parse(line.Substring(0, 8),
                            System.Globalization.NumberStyles.HexNumber);

                        if (cleanFuncOffset != 0)
                            return (initFuncOffset, cleanFuncOffset);
                    }
                    else if (symbol == " _Z7cleanupv")
                    {
                        cleanFuncOffset = uint.Parse(line.Substring(0, 8),
                            System.Globalization.NumberStyles.HexNumber);

                        if (initFuncOffset != 0)
                            return (initFuncOffset, cleanFuncOffset);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while reading newcode.sym,\n" + ex);
            }
            finally
            {
                if (symbolFile != null)
                    symbolFile.Close();
            }

            if (initFuncOffset == 0)
            {
                if (cleanFuncOffset == 0)
                    throw new Exception("Generating DL failed: init and cleanup functions missing");
                else
                    throw new Exception("Generating DL failed: init function missing");    
            }
            else
                throw new Exception("Generating DL failed: cleanup function missing");
        }

        public byte[] MakeDynamicLibrary(Env[] envs = null)
        {
            string additionalEnvs = "";
            if (envs != null)
            {
                foreach (var env in envs)
                {
                    additionalEnvs += $"{env.GetName()}={env.GetValue()} ";
                }
            }
            
            string makeTemplate = "(make CODEADDR=0x{0} {1} && make CODEADDR=0x{2} TARGET=newcode1 {3})";
            string make = String.Format(
                makeTemplate, 
                baseAddress.ToString("X8"),
                additionalEnvs,
                (baseAddress + 4).ToString("X8"),
                additionalEnvs
                );

            if (PatchCompiler.runProcess(make, romdir.FullName) != 0)
                return null;

            return MakeDynamicLibraryFromBinaries();
        }

        public byte[] MakeDynamicLibraryFromBinaries(string codeLo = "/newcode", string codeHi = "/newcode1")
        {
            byte[] code0 = File.ReadAllBytes($"{romdir.FullName}/{codeLo}.bin");
            byte[] code1 = File.ReadAllBytes($"{romdir.FullName}/{codeHi}.bin");

            if (code0.Length != code1.Length)
                throw new Exception("Generating DL failed: code lengths don't match");

            MemoryStream outputStream = new MemoryStream();
            BinaryWriter output = new BinaryWriter(outputStream);
            List<ushort> relocations = new List<ushort>();

            output.Write((ulong)0);
            output.Write((ulong)0);

            uint alignedCodeSize = (uint)code0.Length & ~3U;
            for (ushort i = 0; i < alignedCodeSize; i += 4)
            {
                uint word0 = BitConverter.ToUInt32(code0, i);
                uint word1 = BitConverter.ToUInt32(code1, i);

                if (word0 == word1)
                {
                    output.Write(word0);
                }
                else if (word0 + 4 == word1) // word0 and word1 are pointers
                {
                    output.Write(word0 - baseAddress + 0x10);

                    relocations.Add(i);
                }
                else if (word0 == word1 + 1 && word0 >> 24 == word1 >> 24) // word0 and word1 are branches
                {
                    uint destAddr = getDestOfBranch((int)word0, baseAddress + i);

                    output.Write((destAddr >> 2) | (word0 & 0xff000000));

                    relocations.Add(i);
                }
                else
                {
                    throw new Exception("Generating DL failed: code files don't match for an unknown reason\nnewcode.bin offset: 0x"
                            + i.ToString("X4") + "\nmismatching words: 0x"
                            + word0.ToString("X8") + " and 0x" + word1.ToString("X8"));
                }
            }

            for (uint i = alignedCodeSize; i < code0.Length; ++i)
                output.Write(code0[i]);

            alignStream(output.BaseStream, 4);

            var relocationOffset = output.BaseStream.Position;
            var addresses = getInitAndCleanup(codeLo + ".sym");
            if (addresses == null) return null;

            uint initFuncOffset  = (((uint, uint))addresses).Item1 - baseAddress + 0x10;
            uint cleanFuncOffset = (((uint, uint))addresses).Item2 - baseAddress + 0x10;

            output.Seek(0, SeekOrigin.Begin);

            output.Write((ushort)relocations.Count);
            output.Write((ushort)relocationOffset);
            output.Write((ushort)initFuncOffset);
            output.Write((ushort)cleanFuncOffset);

            output.Seek(0, SeekOrigin.End);
                
            foreach (ushort relocation in relocations)
                output.Write((ushort)(relocation + 0x10));

            return outputStream.ToArray();
        }

        public byte[] generatePatch()
        {
            Console.Out.WriteLine(string.Format("New code address: {0:X8}", m_CodeAddr));

            FileInfo f = new FileInfo(romdir.FullName + "/newcode.bin");
            if (!f.Exists) return null;
            FileStream fs = f.OpenRead();

            byte[] newdata = new byte[fs.Length];
            fs.Read(newdata, 0, (int)fs.Length);
            fs.Close();

            MemoryStream o = new MemoryStream();
            BinaryWriter extradata = new BinaryWriter(o);

            try
            {
                extradata.Write(newdata);
                alignStream(extradata.BaseStream, 4);
            }
            catch(Exception ex)
            {
                new ExceptionMessageBox("Error generating patch", ex).ShowDialog();
                return null;
            }
            finally
            {
                extradata.Close();
            }

            return o.ToArray();

            /*int hookAddr = codeAddr + extradata.getPos();


            f = new FileInfo(romdir.FullName + "/newcode.sym");
            StreamReader s = f.OpenText();

            while (!s.EndOfStream)
            {
                string l = s.ReadLine();

                int ind = -1;
                if (l.Contains("nsub_"))
                    ind = l.IndexOf("nsub_");
                if (l.Contains("hook_"))
                    ind = l.IndexOf("hook_");
                if (l.Contains("repl_"))
                    ind = l.IndexOf("repl_");

                if (ind != -1)
                {
                    int destRamAddr= parseHex(l.Substring(0, 8));    //Redirect dest addr
                    int ramAddr = parseHex(l.Substring(ind + 5, 8)); //Patched addr
                    uint val = 0;

                    int ovId = -1;
                    if (l.Contains("_ov_"))
                        ovId = parseHex(l.Substring(l.IndexOf("_ov_") + 4, 2));

                    int patchCategory = 0;

                    string cmd = l.Substring(ind, 4);
                    int thisHookAddr = 0;

                    switch(cmd)
                    {
                        case "nsub":
                            val = makeBranchOpcode(ramAddr, destRamAddr, false);
                            break;
                        case "repl":
                            val = makeBranchOpcode(ramAddr, destRamAddr, true);
                            break;
                        case "hook":
                            //Jump to the hook addr
                            thisHookAddr = hookAddr;
                            val = makeBranchOpcode(ramAddr, hookAddr, false);

                            uint originalOpcode = handler.readFromRamAddr(ramAddr, ovId);
                            
                            //TODO: Parse and fix original opcode in case of BL instructions
                            //so it's possible to hook over them too.
                            extradata.writeUInt(originalOpcode);
                            hookAddr += 4;
                            extradata.writeUInt(0xE92D5FFF); //push {r0-r12, r14}
                            hookAddr += 4;
                            extradata.writeUInt(makeBranchOpcode(hookAddr, destRamAddr, true));
                            hookAddr += 4;
                            extradata.writeUInt(0xE8BD5FFF); //pop {r0-r12, r14}
                            hookAddr += 4;
                            extradata.writeUInt(makeBranchOpcode(hookAddr, ramAddr+4, false));
                            hookAddr += 4;
                            extradata.writeUInt(0x12345678);
                            hookAddr += 4;
                            break;
                        default:
                            continue;
                    }

                    //Console.Out.WriteLine(String.Format("{0:X8}:{1:X8} = {2:X8}", patchCategory, ramAddr, val));
                    Console.Out.WriteLine(String.Format("              {0:X8} {1:X8}", destRamAddr, thisHookAddr));

                    handler.writeToRamAddr(ramAddr, val, ovId);
                }
            }

            s.Close();

            int newArenaOffs = codeAddr + extradata.getPos();
            handler.writeToRamAddr(ArenaLoOffs, (uint)newArenaOffs, -1);

            handler.sections.Add(new Arm9BinSection(extradata.getArray(), codeAddr, 0));
            handler.saveSections();*/
        }

        public static uint getDestOfBranch(int branchOpcode, uint srcAddr)
        {
            unchecked
            {
                return (uint)(((branchOpcode & 0x00ffffff) << 8 >> 6) + 8 + srcAddr);
            }
        }

        public static uint makeBranchOpcode(int srcAddr, int destAddr, bool withLink)
        {
            unchecked
            {
                uint res = (uint)0xEA000000;

                if (withLink)
                    res |= 0x01000000;

                int offs = (destAddr / 4) - (srcAddr / 4) - 2;
                offs &= 0x00FFFFFF;
                res |= (uint)offs;

                return res;
            }
        }


        public static uint parseUHex(string s)
        {
            return uint.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        public static int parseHex(string s)
        {
            return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        public void restore() {
            handler.restoreFromBackup();
        }

        public void modifyMakefileSources(string sources)
		{
            string[] lines = File.ReadAllLines(romdir.FullName + "\\Makefile");

            for (int i = 0; i < lines.Length; i++) if (lines[i].StartsWith("SOURCES  := "))
                    lines[i] = "SOURCES  := " + sources;

            File.WriteAllLines(romdir.FullName + "\\Makefile", lines);
        }
    }
}
