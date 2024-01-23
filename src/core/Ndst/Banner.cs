using System.IO;
using Ndst.Graphics;

namespace Ndst {

    // DS banner.
    public class Banner {
        public Screen Icon;
        public string[] Title;
        
        // Read banner.
        public Banner(BinaryReader r) {
            ushort version = r.ReadUInt16();
            r.ReadBytes(4 * 2); // CRCs.
            r.ReadBytes(0x16); // Reserved.
            Graphic graphic = new Graphic();
            graphic.Read(r, 4, 4, true, true);
            Palette palette = new Palette();
            palette.Read(r, 0x20);
            graphic.Palette = palette;
            Icon = Screen.GenerateDefault(graphic);
            Title = new string[6];
            for (int i = 0; i < Title.Length; i++) {
                Title[i] = r.ReadFixedLenWide(0x80);
            }
            // IDK how to do the different versions here...
        }

        // Create a banner from an extracted ROM.
        public Banner(string srcPath, string patchPath) {

        }

        // Write a banner.
        public void Write(BinaryWriter w) {

        }

    }

}