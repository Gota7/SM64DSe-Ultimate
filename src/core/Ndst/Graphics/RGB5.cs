using System.Drawing;

namespace Ndst.Graphics {

    // An NDS color.
    public struct RGB5 {
        public ushort Val;
        public byte R5 { get => Get5BitCol(0); set { Set5BitCol(0, value); } }
        public byte G5 { get => Get5BitCol(5); set { Set5BitCol(5, value); } }
        public byte B5 { get => Get5BitCol(10); set { Set5BitCol(10, value); } }
        public byte R8 { get => To8BitCol(Get5BitCol(0)); set { Set5BitCol(0, To5BitCol(value)); } } 
        public byte G8 { get => To8BitCol(Get5BitCol(5)); set { Set5BitCol(5, To5BitCol(value)); } } 
        public byte B8 { get => To8BitCol(Get5BitCol(10)); set { Set5BitCol(10, To5BitCol(value)); } }
        public const byte MAX_COMPONENT_VALUE = 0b11111; 

        public RGB5(ushort col) {
            Val = col;
        }

        // Get 5 bit color.
        private byte Get5BitCol(byte pos) {
            return (byte)((Val >> pos) & 0x1F);
        }

        // Set a 5 bit color.
        private void Set5BitCol(byte pos, byte col) {
            ushort mask = (ushort)(~((0x1F << pos)));
            Val &= mask;
            Val |= (ushort)((col & 0x1F) << pos);
        }

        // To an 8 bit color.
        private byte To8BitCol(byte bit5Col) {
            return (byte)(bit5Col << 3 | bit5Col >> 2);
        }

        // To a 5 bit color.
        private byte To5BitCol(byte bit8Col) {
            return (byte)((bit8Col >> 3) & 0x1F);
        }

        // Get the color.
        public Color RGB8 {

            get {
                return Color.FromArgb(R8, G8, B8);
            }

            set {
                R8 = value.R;
                G8 = value.G;
                B8 = value.B;
            }

        }

    }

}