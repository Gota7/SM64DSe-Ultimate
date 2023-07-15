using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM64DSe.Patcher {

    /// <summary>
    /// Binary section.
    /// </summary>
    public class Arm9BinSection {
        public byte[] data;
        public int len;
        public int ramAddr;
        public int bssSize;
        public bool real = true;

        public Arm9BinSection(byte[] data, int ramAddr, int bssSize) {
            this.data = data;
            len = data.Length;
            this.ramAddr = ramAddr;
            this.bssSize = bssSize;
        }

        public bool containsRamAddr(int addr) {
            return addr >= ramAddr && addr < ramAddr + len;
        }

        public uint readFromRamAddr(int addr) {
            addr -= ramAddr;

            return (uint)(data[addr] |
                data[addr + 1] << 8 |
                data[addr + 2] << 16 |
                data[addr + 3] << 24);
        }

        public void writeToRamAddr(int addr, uint val, int numBytes = 4)     //DY: added numBytes parameter
        {
            addr -= ramAddr;

            //DY
            switch (numBytes) {
                case 1:
                    data[addr] = (byte)val;
                    break;
                case 2:
                    data[addr] = (byte)val;
                    data[addr + 1] = (byte)(val >> 8);
                    break;
                case 4:
                    data[addr] = (byte)val;
                    data[addr + 1] = (byte)(val >> 8);
                    data[addr + 2] = (byte)(val >> 16);
                    data[addr + 3] = (byte)(val >> 24);
                    break;
                default:
                    throw new Exception("WRITE: writeToRamAddr() called with invalid numBytes = " + numBytes.ToString());
            }
        }
    }

}
