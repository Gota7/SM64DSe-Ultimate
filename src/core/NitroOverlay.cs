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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM64DSe
{
    public class NitroOverlay : INitroROMBlock
    {
        public NitroOverlay(NitroROM rom, uint id)
        {
            m_ROM = rom;
            m_ID = id;

            if (Program.m_IsROMFolder) {
                List<Ndst.Overlay> overlays = JsonConvert.DeserializeObject<List<Ndst.Overlay>>(NitroROM.GetExtractedLines("__ROM__/arm9Overlays.json"));
                Ndst.Overlay o = overlays.Where(x => x.Id == id).ElementAt(0);
                m_FileID = o.FileId;
                m_RAMAddr = o.RAMAddress;
                m_Data = NitroROM.GetExtractedBytes("__ROM__/Arm9/" + id + ".bin");
                if ((o.Flags & 0x01000000) > 0) {
                    Jap77.Decompress(ref m_Data);
                }
                return;
            }

            bool autorw = !m_ROM.CanRW();
            if (autorw) m_ROM.BeginRW();

            m_OVTEntryAddr = m_ROM.GetOverlayEntryOffset(m_ID);
            m_FileID = m_ROM.GetFileIDFromOverlayID(m_ID);

            m_RAMAddr = m_ROM.Read32(m_OVTEntryAddr + 0x04);
            Byte flags = m_ROM.Read8(m_OVTEntryAddr + 0x1F);

            m_Data = m_ROM.ExtractFile(m_FileID);
            if ((flags & 0x01) == 0x01)
                Jap77.Decompress(ref m_Data);

            if (autorw) m_ROM.EndRW();
        }

        public uint GetRAMAddr() { return m_RAMAddr; }

        public uint ReadPointer(uint addr)
        {
            uint ptr = Read32(addr);
            if (ptr < m_RAMAddr) return 0xFFFFFFFF;
            return ptr - m_RAMAddr;
        }

        public void WritePointer(uint addr, uint ptr)
        {
            if (ptr == 0xFFFFFFFF) ptr = 0;
            else ptr += m_RAMAddr;
            Write32(addr, ptr);
        }

        public void SaveChangesOld() {

            if (Program.m_IsROMFolder) {
                // first, ensure that the size is aligned to 4 byte boundary
                if (m_Data.Length % 4 != 0) {
                    SetSize((uint)((m_Data.Length + 3) & ~3));
                }
                List<Ndst.Overlay> overlays = JsonConvert.DeserializeObject<List<Ndst.Overlay>>(NitroROM.GetExtractedLines("__ROM__/arm9Overlays.json"));
                Ndst.Overlay o = overlays.Where(x => x.Id == m_ID).ElementAt(0);
                o.RAMSize = (uint)m_Data.Length;
                o.Flags &= 0xFFFFFFFE;
                NitroROM.WriteExtractedBytes("__ROM__/Arm9/" + m_ID + ".bin", m_Data);
                string toWrite = JsonConvert.SerializeObject(overlays, Formatting.Indented);
                NitroROM.WriteExtractedLines("__ROM__/arm9Overlays.json", toWrite);
                return;
            }

            bool autorw = !m_ROM.CanRW();
            if (autorw) m_ROM.BeginRW();

            // first, ensure that the size is aligned to 4 byte boundary
            if (m_Data.Length % 4 != 0)
            {
                SetSize((uint)((m_Data.Length + 3) & ~3));
            }

            // reinsert file data
            m_ROM.ReinsertFileOld(m_FileID, m_Data);
            Update();

            // fix overlay size
            m_ROM.Write32(m_OVTEntryAddr + 0x08, (uint)((m_Data.Length + 3) & ~3));

            // tweak the overlay table entry
            byte flags = m_ROM.Read8(m_OVTEntryAddr + 0x1F);
            flags &= 0xFE; // [Treeki] disable compression :)
            m_ROM.Write8(m_OVTEntryAddr + 0x1F, flags);

            if (autorw) m_ROM.EndRW();

        }

        public override void SaveChanges()
        {
            // first, ensure that the size is aligned to 4 byte boundary
            if (m_Data.Length % 4 != 0)
            {
                SetSize((uint)((m_Data.Length + 3) & ~3));
            }

            if (Program.m_IsROMFolder)
            {
                List<Ndst.Overlay> overlays = JsonConvert.DeserializeObject<List<Ndst.Overlay>>(NitroROM.GetExtractedLines("__ROM__/arm9Overlays.json"));
                Ndst.Overlay o = overlays.Where(x => x.Id == m_ID).ElementAt(0);
                o.RAMSize = (uint)m_Data.Length;
                o.Flags &= 0xFFFFFFFE;
                NitroROM.WriteExtractedBytes("__ROM__/Arm9/" + m_ID + ".bin", m_Data);
                string toWrite = JsonConvert.SerializeObject(overlays, Formatting.Indented);
                NitroROM.WriteExtractedLines("__ROM__/arm9Overlays.json", toWrite);
                return;
            }

            NitroROM.OverlayEntry[] overlayEntries = m_ROM.GetOverlayEntries();
            NitroROM.OverlayEntry ovEntry = overlayEntries[m_ID];
            ovEntry.RAMSize = (uint)m_Data.Length;
            ovEntry.Flags &= 0xfeffffff;
            overlayEntries[m_ID] = ovEntry;

            // reinsert file data (must not be done while RW is active)
            m_ROM.ReinsertFile(m_FileID, m_Data);
            Update();

            bool autorw = !m_ROM.CanRW();
            if (autorw) m_ROM.BeginRW();

            // tweak the overlay table entry (m_OVTEntryAddr is fixed by Update())
            byte flags = m_ROM.Read8(m_OVTEntryAddr + 0x1F);
            flags &= 0xfe; // [Treeki] disable compression :)
            m_ROM.Write8(m_OVTEntryAddr + 0x1F, flags);

            if (autorw) m_ROM.EndRW();
        }

        public uint GetSize()
        {
            return (uint)m_Data.Length;
        }

        public void SetSize(uint newsize)
        {
            Array.Resize(ref m_Data, (int)newsize);
        }

        public void SetInitializer(uint address, uint size)
        {

            if (Program.m_IsROMFolder) {
                List<Ndst.Overlay> overlays = JsonConvert.DeserializeObject<List<Ndst.Overlay>>(NitroROM.GetExtractedLines("__ROM__/arm9Overlays.json"));
                Ndst.Overlay o = overlays.Where(x => x.Id == m_ID).ElementAt(0);
                o.StaticInitStart = address;
                o.StaticInitEnd = address + size;
                string toWrite = JsonConvert.SerializeObject(overlays, Formatting.Indented);
                NitroROM.WriteExtractedLines("__ROM__/arm9Overlays.json", toWrite);
                return;
            }

            bool autorw = !m_ROM.CanRW();
            if (autorw) m_ROM.BeginRW();

            m_ROM.Write32(m_OVTEntryAddr + 0x10, address);
            m_ROM.Write32(m_OVTEntryAddr + 0x14, address + size);
            m_ROM.m_OverlayEntries[m_ID].StaticInitStart = address;
            m_ROM.m_OverlayEntries[m_ID].StaticInitEnd = address + size;
            if (autorw) m_ROM.EndRW();
        }

        public void Update() { m_OVTEntryAddr = m_ROM.GetOverlayEntryOffset(m_ID); }


        private uint m_ID;
        private ushort m_FileID;
        private uint m_OVTEntryAddr;
        private uint m_RAMAddr;
    }
}
