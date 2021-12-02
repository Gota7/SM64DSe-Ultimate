using System.IO;
using Ndst.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Ndst.Formats {

    // Nintendo image.
    public class Enpg : IFormat {
        public Screen Image;

        public bool IsType(byte[] data) {
            return data.Length == 0x10200;
        }

        public void Read(BinaryReader r, byte[] rawData) {
            Graphic g = new Graphic();
            g.Read(r, 256 / 8, 256 / 8, false, true, true); // Always 256x256 image with transparent first color and 8bpp.
            Palette p = new Palette();
            p.IndexSize = 256;
            p.Read(r, 0x200); // Always 256 colors which is 0x200 bytes.
            g.Palette = p;
            Image = Screen.GenerateDefault(g);
        }

        public void Write(BinaryWriter w) {
            Image.Graphic.Write(w);
            Image.Graphic.Palette.Write(w);
        }

        public void Extract(string path) {
            Image.ToImage().SaveAsPng(path + ".png");
        }

        public void Pack(string path) {
            Image<Argb32> img = Image<Argb32>.Load<Argb32>(path); // Don't add extensions, copied to not have it.
            Image = Screen.FromImage(img, false, false, false, true, true, 256 / 8, 256 / 8, 256, null);
        }

        public string GetFormat() {
            return "Enpg";
        }

        public byte[] ContainedFile() {
            return null;
        }

        public bool IsOfFormat(string str) {
            return str.Equals("Enpg");
        }

        public string GetPathExtension() => ".png";
        
    }

}