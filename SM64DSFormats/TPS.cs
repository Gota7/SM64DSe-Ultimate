using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;
using QuickGraph;
using System.Diagnostics;

namespace SM64DSe.SM64DSFormats
{
    public class TPS
    {
        NitroFile p_File;
        public int p_TexType;

        // true width and height
        public int p_width;
        public int p_height;

        //other header info
        public uint p_magic;
        public uint p_flags;
        public uint p_texelArrSize;
        public uint p_palleteOffset;
        public uint p_palleteSize;
        public uint p_unk14;
        public uint p_unk18;
        public uint p_totalSize;

        //texture and palette data
        public byte[] p_RawTextureData, p_RawPaletteData;

        // texture stored as 8bit ARGB
        protected byte[] p_ARGB;

        public override string ToString()
        {
            /*string textureDataString = "", paletteDataString = "";
            for (int i = 0; i < p_texelArrSize; i++)
            {
                textureDataString += Convert.ToString(p_RawTextureData[i], 16).PadLeft(2, '0');
                if (i % 16 == 15)
                    textureDataString += Environment.NewLine;
                else
                    textureDataString += " ";
            }
            for (int i = 0; i < p_palleteSize; i++)
            {
                paletteDataString += Convert.ToString(p_RawPaletteData[i], 16).PadLeft(2, '0');
                if (i % 16 == 15)
                    paletteDataString += Environment.NewLine;
                else
                    paletteDataString += " ";
            }

            string r = "";
            for (int i = 0; i < p_ARGB.Length; i++)
            {
                r += Convert.ToString(p_ARGB[i], 16).PadLeft(2, '0');
                if (i % 16 == 15)
                    r += Environment.NewLine;
                else
                    r += " ";
            }*/

            return $"Flags: 0x{Convert.ToString(p_flags, 16)}{Environment.NewLine}" +
                $"Width: {p_width}{Environment.NewLine}" +
                $"Height: {p_height}{Environment.NewLine}" +
                $"texelArrSize: 0x{Convert.ToString(p_texelArrSize, 16)}{Environment.NewLine}" +
                $"palleteOffset: 0x{Convert.ToString(p_palleteOffset, 16)}{Environment.NewLine}" +
                $"palleteSize: 0x{Convert.ToString(p_palleteSize, 16)}{Environment.NewLine}" +
                $"totalSize: 0x{Convert.ToString(p_totalSize, 16)}{Environment.NewLine}";
        }

        public void CalculateWidthHeight()
        {
            p_width = (int)Math.Pow(2, ((p_flags & 0x78) >> 3) + 1);
            p_height = (int)Math.Pow(2, ((p_flags & 0x780) >> 7) + 1);
            //p_width = 1 << ((((int)p_flags & 0xf << 4) >> 4) + 3);
            //p_height = 1 << ((((int)p_flags & 0xf << 8) >> 8) + 3);
        }

        public void SetWidthHeightFlags()
        {
            // calculate flags from width & height
            uint log2Width = (uint)Math.Log(p_width, 2) - 1;
            uint log2Height = (uint)Math.Log(p_height, 2) - 1;
            p_flags = (uint)(p_flags & ~0x7F8) | log2Width << 3 | log2Height << 7;
        }

        public TPS(NitroFile tps)
        {
            p_File = tps;
            p_flags = tps.Read32(0x04);
            if ((p_flags & 0x7) == 0)
                throw new ArgumentException("Texture Type cannot be zero.");
            p_texelArrSize = tps.Read32(0x08);
            p_palleteOffset = tps.Read32(0x0c);
            p_palleteSize = tps.Read32(0x10);
            p_unk14 = tps.Read32(0x14);
            p_unk18 = tps.Read32(0x18);
            p_totalSize = tps.Read32(0x1c);

            CalculateWidthHeight();

            p_RawTextureData = tps.ReadBlock(0x20, ((p_flags & 0x7) != 5) ? p_texelArrSize : (p_texelArrSize + (p_texelArrSize / 2)));
            p_RawPaletteData = (p_palleteSize > 0) ? tps.ReadBlock(p_palleteOffset, p_palleteSize) : null;
            p_ARGB = new byte[p_width * p_height * 4];

            ArgbFromData();
        }

        public void FromBMP(Bitmap bmp, int textype)
        {
            int dswidth = 0, dsheight = 0, widthPowerOfTwo = 8, heightPowerOfTwo = 8;
            GetDSWidthAndHeight(bmp.Width, bmp.Height, out dswidth, out dsheight, out widthPowerOfTwo, out heightPowerOfTwo);

            // cheap resizing for textures whose dimensions aren't power-of-two
            if ((widthPowerOfTwo != bmp.Width) || (heightPowerOfTwo != bmp.Height))
            {
                Bitmap newbmp = new Bitmap(widthPowerOfTwo, heightPowerOfTwo);
                Graphics g = Graphics.FromImage(newbmp);
                g.DrawImage(bmp, new Rectangle(0, 0, widthPowerOfTwo, heightPowerOfTwo));
                bmp = newbmp;
            }

            p_width = bmp.Width;
            p_height = bmp.Height;

            p_flags = 0xffff0000;
            SetWidthHeightFlags();
            p_flags = (uint)(p_flags & ~0x7) | (uint)textype;

            p_TexType = textype;
            p_ARGB = new byte[p_width * p_height * 4];
            DataFromBitmap(bmp);

            p_texelArrSize = (uint)p_RawTextureData.Length;
            p_palleteSize = (uint)p_RawPaletteData.Length;
            p_palleteOffset = 0x20 + p_texelArrSize;
            p_totalSize = p_unk14 = p_palleteOffset + p_palleteSize;
        }

        public Bitmap ArgbToBitmap()
        {
            Bitmap bmp = new Bitmap(p_width, p_height);

            for (int y = 0; y < p_height; y++)
            {
                for (int x = 0; x < p_width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(p_ARGB[((y * p_width) + x) * 4 + 3],
                     p_ARGB[((y * p_width) + x) * 4 + 2],
                     p_ARGB[((y * p_width) + x) * 4 + 1],
                     p_ARGB[((y * p_width) + x) * 4]));
                }
            }

            return bmp;
        }

        public static bool BitmapUsesTranslucency(Bitmap bmp)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int a = bmp.GetPixel(x, y).A;
                    if (a >= 8 && a <= 248) { return true; }
                }
            }
            return false;
        }

        public static bool BitmapUsesTransparency(Bitmap bmp)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int a = bmp.GetPixel(x, y).A;
                    if (a < 8) { return true; }
                }
            }
            return false;
        }

        public static int CountColoursInBitmap(Bitmap bmp)
        {
            HashSet<int> colours = new HashSet<int>();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    colours.Add(bmp.GetPixel(x, y).ToArgb());
                }
            }
            return colours.Count;
        }

        protected static void GetDSWidthAndHeight(int texWidth, int texHeight, out int dswidth, out int dsheight, out int widthPowerOfTwo, out int heightPowerOfTwo)
        {
            // (for N=0..7: Size=(8 SHL N); ie. 8..1024 texels)
            widthPowerOfTwo = 8; heightPowerOfTwo = 8;
            dswidth = 0; dsheight = 0;
            while (widthPowerOfTwo < texWidth) { widthPowerOfTwo *= 2; dswidth++; }
            while (heightPowerOfTwo < texHeight) { heightPowerOfTwo *= 2; dsheight++; }
        }

        protected static ushort[] ByteArrayToUShortArray(byte[] bytes)
        {
            ushort[] ushorts = new ushort[bytes.Length / 2];
            for (int i = 0; i < ushorts.Length; i += 2)
            {
                ushorts[i] = (ushort)(bytes[i] | (bytes[i + 1] << 8));
            }
            return ushorts;
        }



        public void DataFromBitmap(Bitmap bmp)
        {
            int texformat = (int)(p_flags & 0x7);
            switch (texformat)
            {
                case 1:
                    A3I5FromBitmap(bmp);
                    return;
                case 2:
                    Color4FromBitmap(bmp);
                    return;
                case 3:
                    Color16FromBitmap(bmp);
                    return;
                case 4:
                    Color256FromBitmap(bmp);
                    return;
                case 5:
                    throw new ArgumentException("Texel format not supported yet.");
                case 6:
                    A5I3FromBitmap(bmp);
                    return;
                case 7:
                    DirectFromBitmap(bmp);
                    return;
                default: throw new ArgumentException($"Texture format must be 1 - 7 (format = {p_flags & 0x7})");
            }
        }

        public void A3I5FromBitmap(Bitmap bmp)
        {
            p_RawTextureData = new byte[bmp.Width * bmp.Height];
            Palette _pal = new Palette(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 32);
            int alphamask = 0xE0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    ushort bgr15 = Helper.ColorToBGR15(c);
                    int a = c.A & alphamask;

                    byte val = (byte)(_pal.FindClosestColorID(bgr15) | a);
                    p_RawTextureData[(y * bmp.Width) + x] = val;
                }
            }

            p_RawPaletteData = _pal.WriteToBytes(16);
        }

        public void Color4FromBitmap(Bitmap bmp)
        {
            p_RawTextureData = new byte[(bmp.Width * bmp.Height) / 4];

            Palette txpal = new Palette(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 4, GetTransparentFlag());

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0, _out = 0; x < bmp.Width; x += 4, _out++)
                {
                    for (int t = 0; t < 4; t++)
                    {
                        Color c = bmp.GetPixel(x + t, y);
                        ushort bgr15 = Helper.ColorToBGR15(c);

                        byte val = (byte)((c.A < 8) ? 0 : txpal.FindClosestColorID(bgr15));
                        p_RawTextureData[(y * (bmp.Width / 4)) + _out] |= (byte)((val & 0x03) << (t * 2));
                    }
                }
            }

            p_RawPaletteData = txpal.WriteToBytes(8);
        }

        public void Color16FromBitmap(Bitmap bmp)
        {
            p_RawTextureData = new byte[(bmp.Width * bmp.Height) / 2];

            Palette txpal = new Palette(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 16, GetTransparentFlag());

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0, _out = 0; x < bmp.Width; x += 2, _out++)
                {
                    for (int t = 0; t < 2; t++)
                    {
                        Color c = bmp.GetPixel(x + t, y);
                        ushort bgr15 = Helper.ColorToBGR15(c);

                        byte val = (byte)((c.A < 8) ? 0 : txpal.FindClosestColorID(bgr15));
                        p_RawTextureData[(y * (bmp.Width / 2)) + _out] |= (byte)((val & 0x0F) << (t * 4));
                    }
                }
            }

            p_RawPaletteData = txpal.WriteToBytes(8);
        }

        public void Color256FromBitmap(Bitmap bmp)
        {
            p_RawTextureData = new byte[p_width * p_height];

            Palette txpal = new Palette(bmp, new Rectangle(0, 0, p_width, p_height), 256, GetTransparentFlag());
            for (int y = 0; y < p_height; y++)
            {
                for (int x = 0; x < p_width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    ushort bgr15 = Helper.ColorToBGR15(c);

                    byte val = (byte)((c.A < 8) ? 0 : txpal.FindClosestColorID(bgr15));
                    p_RawTextureData[(y * p_width) + x] = val;
                }
            }

            p_RawPaletteData = txpal.WriteToBytes(16);
        }

        public void Tex4x4FromBitmap(Bitmap bmp)
        {

        }

        public void A5I3FromBitmap(Bitmap bmp)
        {
            p_RawTextureData = new byte[bmp.Width * bmp.Height];
            Palette _pal = new Palette(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 32);
            int alphamask = 0xF8;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    //Console.WriteLine($"y = {y};{Environment.NewLine}x = {x};");
                    Color c = bmp.GetPixel(x, y);
                    ushort bgr15 = Helper.ColorToBGR15(c);
                    int a = c.A & alphamask;

                    byte val = (byte)(_pal.FindClosestColorID(bgr15) | a);
                    p_RawTextureData[(y * bmp.Width) + x] = val;
                }
            }

            p_RawPaletteData = _pal.WriteToBytes(16);
        }

        public void DirectFromBitmap(Bitmap bmp)
        {
            p_RawTextureData = new byte[(p_width * p_height) * 2];

            for (int y = 0; y < p_height; y++)
            {
                for (int x = 0; x < p_width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    ushort bgr15 = Helper.ColorToBGR15(c);
                    if (c.A > 0) bgr15 |= 0x8000;

                    p_RawTextureData[(((y * p_width) + x) * 2)] = (byte)(bgr15 & 0xFF);
                    p_RawTextureData[(((y * p_width) + x) * 2) + 1] = (byte)(bgr15 >> 8);
                }
            }

            p_RawPaletteData = new byte[0];
        }



        public void ArgbFromData()
        {
            int texformat = (int)(p_flags & 0x7);
            switch (texformat)
            {
                case 1:
                    ArgbFromA3I5();
                    return;
                case 2:
                    ArgbFromColor4();
                    return;
                case 3:
                    ArgbFromColor16();
                    return;
                case 4:
                    ArgbFromColor256();
                    return;
                case 5:
                    throw new ArgumentException("Texel format not supported yet.");
                case 6:
                    ArgbFromA5I3();
                    return;
                case 7:
                    ArgbFromDirect();
                    return;
                default: throw new ArgumentException("Texture type must be 1 - 7");
            }
        }

        public void ArgbFromA3I5()
        {
            for (uint _in = 0, _out = 0; _in < p_RawTextureData.Length; _in++, _out += 4)
            {
                byte texel = p_RawTextureData[_in];
                ushort color = Helper.BytesToUShort16(p_RawPaletteData, ((texel & 0x1F) << 1));

                byte red = (byte)((color & 0x001F) << 3);
                byte green = (byte)((color & 0x03E0) >> 2);
                byte blue = (byte)((color & 0x7C00) >> 7);
                byte _alpha = (byte)(((texel & 0xE0) >> 3) + ((texel & 0xE0) >> 6));
                byte alpha = (byte)((_alpha << 3) | (_alpha >> 2));

                p_ARGB[_out] = blue;
                p_ARGB[_out + 1] = green;
                p_ARGB[_out + 2] = red;
                p_ARGB[_out + 3] = alpha;
            }
        }

        public void ArgbFromColor4()
        {
            byte zeroAlpha = (byte)(GetTransparentFlag() ? 0x00 : 0xFF);
            for (int _in = 0, _out = 0; _in < p_RawTextureData.Length; _in++, _out += 16)
            {
                byte texels = p_RawTextureData[_in];

                ushort color = Helper.BytesToUShort16(p_RawPaletteData, ((texels << 1) & 0x6));
                byte red = (byte)((color & 0x001F) << 3);
                byte green = (byte)((color & 0x03E0) >> 2);
                byte blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out] = blue;
                p_ARGB[_out + 1] = green;
                p_ARGB[_out + 2] = red;
                p_ARGB[_out + 3] = (byte)(((texels & 0x03) != 0) ? (byte)0xFF : zeroAlpha);

                color = Helper.BytesToUShort16(p_RawPaletteData, ((texels >> 1) & 0x6));
                red = (byte)((color & 0x001F) << 3);
                green = (byte)((color & 0x03E0) >> 2);
                blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out + 4] = blue;
                p_ARGB[_out + 5] = green;
                p_ARGB[_out + 6] = red;
                p_ARGB[_out + 7] = (byte)(((texels & 0x0C) != 0) ? (byte)0xFF : zeroAlpha);

                color = Helper.BytesToUShort16(p_RawPaletteData, ((texels >> 3) & 0x6));
                red = (byte)((color & 0x001F) << 3);
                green = (byte)((color & 0x03E0) >> 2);
                blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out + 8] = blue;
                p_ARGB[_out + 9] = green;
                p_ARGB[_out + 10] = red;
                p_ARGB[_out + 11] = (byte)(((texels & 0x30) != 0) ? (byte)0xFF : zeroAlpha);

                color = Helper.BytesToUShort16(p_RawPaletteData, ((texels >> 5) & 0x6));
                red = (byte)((color & 0x001F) << 3);
                green = (byte)((color & 0x03E0) >> 2);
                blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out + 12] = blue;
                p_ARGB[_out + 13] = green;
                p_ARGB[_out + 14] = red;
                p_ARGB[_out + 15] = (byte)(((texels & 0xC0) != 0) ? (byte)0xFF : zeroAlpha);
            }
        }

        public void ArgbFromColor16()
        {
            byte zeroAlpha = (GetTransparentFlag() ? (byte)0x00 : (byte)0xFF);
            for (int _in = 0, _out = 0; _in < p_RawTextureData.Length; _in++, _out += 8)
            {
                byte texels = p_RawTextureData[_in];

                ushort color = Helper.BytesToUShort16(p_RawPaletteData, ((texels << 1) & 0x1E));
                byte red = (byte)((color & 0x001F) << 3);
                byte green = (byte)((color & 0x03E0) >> 2);
                byte blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out] = blue;
                p_ARGB[_out + 1] = green;
                p_ARGB[_out + 2] = red;
                p_ARGB[_out + 3] = (byte)(((texels & 0x0F) != 0) ? (byte)0xFF : zeroAlpha);

                color = Helper.BytesToUShort16(p_RawPaletteData, ((texels >> 3) & 0x1E));
                red = (byte)((color & 0x001F) << 3);
                green = (byte)((color & 0x03E0) >> 2);
                blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out + 4] = blue;
                p_ARGB[_out + 5] = green;
                p_ARGB[_out + 6] = red;
                p_ARGB[_out + 7] = (byte)(((texels & 0xF0) != 0) ? (byte)0xFF : zeroAlpha);
            }
        }

        public void ArgbFromColor256()
        {
            byte zeroAlpha = (byte)(GetTransparentFlag() ? 0x00 : 0xFF);
            for (int _in = 0, _out = 0; _in < p_RawTextureData.Length; _in++, _out += 4)
            {
                byte texel = p_RawTextureData[_in];

                ushort color = Helper.BytesToUShort16(p_RawPaletteData, (texel << 1));
                byte red = (byte)((color & 0x001F) << 3);
                byte green = (byte)((color & 0x03E0) >> 2);
                byte blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out] = blue;
                p_ARGB[_out + 1] = green;
                p_ARGB[_out + 2] = red;
                p_ARGB[_out + 3] = (byte)((texel != 0) ? (byte)0xFF : zeroAlpha);
            }
        }

        public void ArgbFromTex4x4()
        {

        }

        public void ArgbFromA5I3()
        {
            for (int _in = 0, _out = 0; _in < p_RawTextureData.Length; _in++, _out += 4)
            {
                //Console.WriteLine($"_in = {_in};{Environment.NewLine}_out = {_out};{Environment.NewLine}p_RawTextureData.Length = {p_RawTextureData.Length};");
                byte texel = p_RawTextureData[_in];
                ushort color = Helper.BytesToUShort16(p_RawPaletteData, ((texel & 0x07) << 1));
                byte red = (byte)((color & 0x001F) << 3);
                byte green = (byte)((color & 0x03E0) >> 2);
                byte blue = (byte)((color & 0x7C00) >> 7);
                byte alpha = (byte)((texel & 0xF8) | ((texel & 0xF8) >> 5));

                p_ARGB[_out] = blue;
                p_ARGB[_out + 1] = green;
                p_ARGB[_out + 2] = red;
                p_ARGB[_out + 3] = alpha;
            }
        }

        public void ArgbFromDirect()
        {
            for (int _in = 0, _out = 0; _in < p_RawTextureData.Length; _in += 2, _out += 4)
            {
                ushort color = Helper.BytesToUShort16(p_RawTextureData, _in);
                byte red = (byte)((color & 0x001F) << 3);
                byte green = (byte)((color & 0x03E0) >> 2);
                byte blue = (byte)((color & 0x7C00) >> 7);

                p_ARGB[_out] = blue;
                p_ARGB[_out + 1] = green;
                p_ARGB[_out + 2] = red;
                p_ARGB[_out + 3] = (byte)(((color & 0x8000) != 0) ? (byte)0xFF : 0x00);
            }
        }



        public bool GetRepeatSFlag()
        {
            return (p_flags >> 0xb & 0x1) == 1;
        }

        public bool GetRepeatTFlag()
        {
            return (p_flags >> 0xc & 0x1) == 1;
        }

        public bool GetFlipSFlag()
        {
            return (p_flags >> 0xd & 0x1) == 1;
        }

        public bool GetFlipTFlag()
        {
            return (p_flags >> 0xe & 0x1) == 1;
        }

        public bool GetTransparentFlag()
        {
            return (p_flags >> 0xf & 0x1) == 1;
        }

        public void SetRepeatSFlag()
        {
            p_flags ^= 1 << 0xb;
            Console.WriteLine($"Repeat S Flag has been set. (0x{Convert.ToString(p_flags, 16)})");
        }

        public void SetRepeatTFlag()
        {
            p_flags ^= 1 << 0xc;
            Console.WriteLine($"Repeat T Flag has been set. (0x{Convert.ToString(p_flags, 16)})");
        }

        public void SetFlipSFlag()
        {
            p_flags ^= 1 << 0xd;
            Console.WriteLine($"Flip S Flag has been set. (0x{Convert.ToString(p_flags, 16)})");
        }

        public void SetFlipTFlag()
        {
            p_flags ^= 1 << 0xe;
            Console.WriteLine($"Flip T Flag has been set. (0x{Convert.ToString(p_flags, 16)})");
        }

        public void SetTransparentFlag()
        {
            p_flags ^= 1 << 0xf;
            Console.WriteLine($"Transparent: {GetTransparentFlag()}");
            Console.WriteLine("New flags: " + Convert.ToString(p_flags, 16));
            ArgbFromData();
        }



        public void ChangeFormat(uint newFormat)
        {
            if (newFormat == 5)
                throw new ArgumentException("The Texel4x4 format is currently not supported, please use another format.");
            else if (newFormat == 2 && p_RawPaletteData.Length > 4 * 2)
                throw new ArgumentException("The Color4 format can only have 4 colors.");
            else if (newFormat == 3 && p_RawPaletteData.Length > 16 * 2)
                throw new ArgumentException("The Color16 format can only have 16 colors.");
            else if (newFormat == 4 && p_RawPaletteData.Length > 256 * 2)
                throw new ArgumentException("The Color256 format can only have 256 colors.");

            p_flags = (uint)(p_flags & ~0x7) | newFormat;
        }


        
        public override bool Equals(Object obj)
        {
            var tx = obj as TPS;
            if (tx == null) return false;
            if (p_width != tx.p_width || p_height != tx.p_height) return false;
            if (p_TexType != tx.p_TexType || p_flags != tx.p_flags) return false;
            if ((p_RawTextureData == null) != (tx.p_RawTextureData == null)) return false;
            if (p_RawTextureData != null && !p_RawTextureData.SequenceEqual(tx.p_RawTextureData)) return false;
            if ((p_RawPaletteData == null) != (tx.p_RawPaletteData == null)) return false;
            if (p_RawPaletteData != null && !p_RawPaletteData.SequenceEqual(tx.p_RawPaletteData)) return false;
            if ((p_ARGB == null) != (tx.p_ARGB == null)) return false;
            if (p_ARGB != null && !p_ARGB.SequenceEqual(tx.p_ARGB)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = hash * 7 + p_width.GetHashCode();
                hash = hash * 7 + p_height.GetHashCode();
                hash = hash * 7 + p_TexType.GetHashCode();
                hash = hash * 7 + p_flags.GetHashCode();
                hash = hash * 7 + ((p_RawTextureData != null) ? p_RawTextureData.GetHashCode() : -1);
                hash = hash * 7 + ((p_RawPaletteData != null) ? p_RawPaletteData.GetHashCode() : -1);
                hash = hash * 7 + ((p_ARGB != null) ? p_ARGB.GetHashCode() : -1);
                return hash;
            }
        }

        public class Palette
        {
            private static int ColorComparer(ushort c1, ushort c2)
            {
                int r1 = c1 & 0x1F;
                int g1 = (c1 >> 5) & 0x1F;
                int b1 = (c1 >> 10) & 0x1F;
                int r2 = c2 & 0x1F;
                int g2 = (c2 >> 5) & 0x1F;
                int b2 = (c2 >> 10) & 0x1F;

                int tdiff = (r2 - r1) + (g2 - g1) + (b2 - b1);
                if (tdiff == 0)
                    return 0;
                else if (tdiff < 0)
                    return 1;
                else
                    return -1;
            }

            public Palette(Palette pal)
            {
                m_Palette = pal.m_Palette.ToList();
                m_FirstColourTransparent = pal.m_FirstColourTransparent;
            
            }

            public Palette(Bitmap bmp, Rectangle region, int depth, bool firstColourTransparent = false)
            {
                List<ushort> pal = new List<ushort>(depth);
                m_FirstColourTransparent = firstColourTransparent;
                if (m_FirstColourTransparent) depth--;

                // 1. get the colors used within the requested region
                for (int y = region.Top; y < region.Bottom; y++)
                {
                    for (int x = region.Left; x < region.Right; x++)
                    {
                        ushort col15 = Helper.ColorToBGR15(bmp.GetPixel(x, y));
                        if (!pal.Contains(col15))
                            pal.Add(col15);
                    }
                }

                // 2. shrink down the palette by removing colors that
                // are close to others, until it fits within the
                // requested size
                pal.Sort(Palette.ColorComparer);
                int maxdiff = 0;
                while (pal.Count > depth)
                {
                    for (int i = 1; i < pal.Count; )
                    {
                        ushort c1 = pal[i - 1];
                        ushort c2 = pal[i];

                        int r1 = c1 & 0x1F;
                        int g1 = (c1 >> 5) & 0x1F;
                        int b1 = (c1 >> 10) & 0x1F;
                        int r2 = c2 & 0x1F;
                        int g2 = (c2 >> 5) & 0x1F;
                        int b2 = (c2 >> 10) & 0x1F;

                        if (Math.Abs(r1 - r2) <= maxdiff && Math.Abs(g1 - g2) <= maxdiff && Math.Abs(b1 - b2) <= maxdiff)
                        {
                            ushort cmerged = Helper.BlendColorsBGR15(c1, 1, c2, 1);
                            pal[i - 1] = cmerged;
                            pal.RemoveAt(i);
                        }
                        else
                            i++;
                    }

                    maxdiff++;
                }

                if (m_FirstColourTransparent) pal.Insert(0, 0x0000);

                m_Palette = pal;
            }

            public int GetBestColorMode(bool transp)
            {
                if (m_Palette.Count <= 2)
                    return transp ? 1 : 3;

                int[][] rgbs = Array.ConvertAll(m_Palette.ToArray(), col => new int[] { col & 0x1f, col >> 5 & 0x1f, col >> 10 & 0x1f });

                if (!transp)
                {
                    for (int i = 0; i < m_Palette.Count; ++i)
                        for (int j = 0; j < m_Palette.Count - 1; ++j)
                            for (int k = 0; k < m_Palette.Count - 2; ++k)
                            {
                                //get a permutation
                                int iR = i, jR = j, kR = k, lR = 0;
                                if (jR >= iR) ++jR;
                                if (kR >= Math.Min(iR, jR)) ++kR;
                                if (kR >= Math.Max(iR, jR)) ++kR;
                                int[] ijkInOrder = new int[] { iR, jR, kR }; Array.Sort(ijkInOrder);
                                if (lR >= ijkInOrder[0]) ++lR;
                                if (lR >= ijkInOrder[1]) ++lR;
                                if (lR >= ijkInOrder[2]) ++lR;

                                bool canUseColMode3_a = true;
                                bool canUseColMode3_b = true; //this is to help check both (3x+5y)/8 and (5x+3y)/8 with a 3-color pallete.
                                for (int comp = 0; comp < 3; ++comp)
                                {
                                    if (rgbs[kR][comp] != (rgbs[iR][comp] * 5 + rgbs[jR][comp] * 3) / 8 || m_Palette.Count == 4 &&
                                        rgbs[lR][comp] != (rgbs[iR][comp] * 3 + rgbs[jR][comp] * 5) / 8)
                                        canUseColMode3_a = false;
                                    if (m_Palette.Count != 3 || rgbs[kR][comp] != (rgbs[iR][comp] * 3 + rgbs[jR][comp] * 5) / 8)
                                        canUseColMode3_b = false;
                                }
                                    

                                if (canUseColMode3_a || canUseColMode3_b)
                                {
                                    if(lR < kR)
                                    {
                                        int temp = kR;
                                        kR = lR;
                                        lR = temp;
                                    }
                                    if (m_Palette.Count == 4)
                                        m_Palette.RemoveAt(lR);
                                    m_Palette.RemoveAt(kR);
                                    return 3;
                                }
                            }
                }
                if (m_Palette.Count == 4)
                    return 2;

                for (int i = 0; i < 3; ++i)
                    for (int j = 0; j < 2; ++j)
                    {
                        int iR = i, jR = j, kR = 0;
                        if (jR >= iR) ++jR;
                        if (kR >= Math.Min(iR, jR)) ++kR;
                        if (kR >= Math.Max(iR, jR)) ++kR;

                        bool canUseColMode1 = true;
                        for (int comp = 0; comp < 3; ++comp)
                            if (rgbs[kR][comp] != (rgbs[iR][comp] + rgbs[jR][comp]) / 2)
                                canUseColMode1 = false;

                        if(canUseColMode1)
                        {
                            m_Palette.RemoveAt(kR);
                            return 1;
                        }
                    }

                return transp ? 0 : 2;
            }

            public int FindClosestColorID(ushort c, int color_mode = -1)
            {
                int r = c & 0x1F;
                int g = (c >> 5) & 0x1F;
                int b = (c >> 10) & 0x1F;

                int maxdiff = 0;

                int startIndex = (m_FirstColourTransparent) ? 1 : 0;
                if(m_Palette.Count > 1 && (color_mode == 1 || color_mode == 3))
                {
                    ushort r0 = (ushort)(m_Palette[0] >>  0 & 0x1f);
                    ushort g0 = (ushort)(m_Palette[0] >>  5 & 0x1f);
                    ushort b0 = (ushort)(m_Palette[0] >> 10 & 0x1f);
 
                    ushort r1 = (ushort)(m_Palette[1] >>  0 & 0x1f);
                    ushort g1 = (ushort)(m_Palette[1] >>  5 & 0x1f);
                    ushort b1 = (ushort)(m_Palette[1] >> 10 & 0x1f);

                    if (color_mode == 1)
                    {
                        m_Palette.Add((ushort)((r0 + r1) / 2 << 0 |
                                               (g0 + g1) / 2 << 5 |
                                               (b0 + b1) / 2 << 10));
                    }
                    if (color_mode == 3)
                    {
                        m_Palette.Add((ushort)((r0 * 5 + r1 * 3) / 8 << 0 |
                                               (g0 * 5 + g1 * 3) / 8 << 5 |
                                               (b0 * 5 + b1 * 3) / 8 << 10));
                        m_Palette.Add((ushort)((r0 * 3 + r1 * 5) / 8 << 0 |
                                               (g0 * 3 + g1 * 5) / 8 << 5 |
                                               (b0 * 3 + b1 * 5) / 8 << 10));
                    }
                }
                for (; ; )
                {
                    for (int i = startIndex; i < m_Palette.Count; i++)
                    {
                        ushort c1 = m_Palette[i];
                        int r1 = c1 & 0x1F;
                        int g1 = (c1 >> 5) & 0x1F;
                        int b1 = (c1 >> 10) & 0x1F;

                        if (Math.Abs(r1 - r) <= maxdiff && Math.Abs(g1 - g) <= maxdiff && Math.Abs(b1 - b) <= maxdiff)
                        {
                            if(m_Palette.Count > 1)
                            {
                                if (color_mode == 1)
                                    m_Palette.RemoveAt(2);
                                if (color_mode == 3)
                                    m_Palette.RemoveRange(2, 2);
                            }
                            
                            return i;
                        }
                    }

                    maxdiff++;
                }
            }

            public byte[] WriteToBytes(int minLength)
            {
                int length = ((m_Palette.Count + 3) & ~3) * 2;
                if (length < minLength) length = minLength;

                byte[] pal = new byte[length];
                for (int i = 0; i < m_Palette.Count; i++)
                {
                    pal[i * 2] = (byte)(m_Palette[i] & 0xFF);
                    pal[(i * 2) + 1] = (byte)(m_Palette[i] >> 8);
                }
                return pal;
            }

            public static bool AreSimilar(Palette p1, Palette p2, out uint offset)
            {
                offset = 0;
                if (p1.m_Palette.Count > p2.m_Palette.Count)
                    return false;

                bool[] mapped = new bool[p1.m_Palette.Count];

                for (int i = 0; i < p1.m_Palette.Count; i++)
                {
                    for(int j = 0; j < p1.m_Palette.Count; ++j)
                    {
                        ushort c1 = p1.m_Palette[i];
                        ushort c2 = p2.m_Palette[j + (int)offset];

                        if(c1 == c2)
                        {
                            mapped[i] = true;
                            break;
                        }

                        if (p1.m_Palette.Count == 2 && p2.m_Palette.Count == 4 && offset == 0)
                        {
                            offset = 2;
                            j = -1;
                        }
                    }
                }

                return Array.TrueForAll(mapped, b => b);
            }

            public List<ushort> m_Palette;
            private bool m_FirstColourTransparent;
        }

        protected class PaletteEqualityComparer : IEqualityComparer<Palette>
        {
            public bool Equals(Palette x, Palette y) { uint dummy;  return Palette.AreSimilar(x, y, out dummy); }
            public  int GetHashCode(Palette x) {
                return x.m_Palette[0] ^
                       (x.m_Palette.Count > 1 ? x.m_Palette[1] << 15 : 0) ^
                       (x.m_Palette.Count > 2 ? x.m_Palette[2] << 17 : 0) ^
                       (x.m_Palette.Count > 3 ? x.m_Palette[3] << 2 : 0); }
        }

        public void SaveTPS()
        {
            // header
            p_File.Write32(0x0, 0x53505420);
            p_File.Write32(0x4, p_flags);
            p_File.Write32(0x8, p_texelArrSize);
            p_File.Write32(0xc, p_palleteOffset);
            p_File.Write32(0x10, p_palleteSize);
            p_File.Write32(0x14, p_unk14);
            p_File.Write32(0x18, p_unk18);
            p_File.Write32(0x1c, p_totalSize);

            // data
            p_File.WriteBlock(0x20, p_RawTextureData);
            p_File.WriteBlock(p_palleteOffset, p_RawPaletteData);

            Console.WriteLine($"p_File.m_Data.Length = {p_File.m_Data.Length}");
            Console.WriteLine($"p_totalSize = {p_totalSize}");
            Console.WriteLine($"");

            if (p_File.m_Data.Length - (int)p_totalSize > 0)
                p_File.RemoveSpace(p_totalSize, (uint)p_File.m_Data.Length - p_totalSize); // make it so the file is only as big as it needs to be


            p_File.SaveChanges();
        }
    }
}
