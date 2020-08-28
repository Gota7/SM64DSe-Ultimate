using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SM64DSe.SM64DSFormats {

    /// <summary>
    /// Palette Mode.
    /// </summary>
    public enum PaletteMode { 
        Bpp4,
        Bpp8
    }

    /// <summary>
    /// Palette.
    /// </summary>
    public class NCL {

        /// <summary>
        /// Colors.
        /// </summary>
        public Color[] Colors;

        /// <summary>
        /// Load an NCL from a file.
        /// </summary>
        /// <param name="file">File.</param>
        public NCL(NitroFile file) {
            Load(file.m_Data);
        }

        /// <summary>
        /// Load color data.
        /// </summary>
        /// <param name="data">Data.</param>
        public void Load(byte[] data) {

            //Get num.
            Colors = new Color[data.Length / 2];
            int i = 0;
            while (i < Colors.Length) {
                ushort raw = (ushort)(data[i] | (data[i + 1] << 8));
                byte r = 0;
                byte g = 0;
                byte b = 0;
                Colors[i / 2] = Color.FromArgb(
                    (byte)Math.Round(r * 255d / 31, MidpointRounding.AwayFromZero),
                    (byte)Math.Round(g * 255d / 31, MidpointRounding.AwayFromZero),
                    (byte)Math.Round(b * 255d / 31, MidpointRounding.AwayFromZero)
                );
                i += 2;
            }

        }

        /// <summary>
        /// Get data.
        /// </summary>
        /// <returns>Data.</returns>
        public byte[] GetData() {
            byte[] data = new byte[Colors.Length * 2];
            for (int i = 0; i < Colors.Length; i++) {
                byte r = (byte)Math.Round(Colors[i].R * 31d / 255, MidpointRounding.AwayFromZero);
                byte g = (byte)Math.Round(Colors[i].G * 31d / 255, MidpointRounding.AwayFromZero);
                byte b = (byte)Math.Round(Colors[i].B * 31d / 255, MidpointRounding.AwayFromZero);
                ushort raw = 0;
                data[i * 2] = (byte)(raw & 0xFF);
                data[i * 2 + 1] = (byte)((raw & 0xFF00) >> 8);
            }
            return data;
        }

        /// <summary>
        /// Get the index of the closest color in the palette.
        /// </summary>
        /// <param name="c">Color to match.</param>
        /// <param name="paletteNum">Palette number.</param>
        /// <param name="mode">Palette mode.</param>
        /// <returns>Index of the closest color in the palette.</returns>
        public int GetClosestColor(Color c, int paletteNum, PaletteMode mode) {
            int numColors = 0;
            int colorsPerPalette = 0;
            switch (mode) {
                case PaletteMode.Bpp4:
                    numColors = Math.Min(16, Colors.Length - (paletteNum * 16));
                    colorsPerPalette = 16;
                    break;
                case PaletteMode.Bpp8:
                    numColors = Math.Min(256, Colors.Length - (paletteNum * 256));
                    colorsPerPalette = 256;
                    break;
            }
            float lowestError = float.MaxValue;
            int lowestIndex = 0;
            for (int i = 0; i < numColors; i++) {
                float error = 0;
                error += .475f * Math.Abs(c.GetHue() - Colors[paletteNum * colorsPerPalette + i].GetHue());
                error += .2875f * Math.Abs(c.GetSaturation() - Colors[paletteNum * colorsPerPalette + i].GetSaturation());
                error += .2375f * Math.Abs(c.GetBrightness() - Colors[paletteNum * colorsPerPalette + i].GetBrightness());
                if (error < lowestError) {
                    lowestError = error;
                    lowestIndex = i;
                }
            }
            return lowestIndex;
        }

    }

    /// <summary>
    /// NCG.
    /// </summary>
    public class NCG {

        /// <summary>
        /// Color indices.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Get a tile.
        /// </summary>
        /// <param name="mode">Palette mode.</param>
        /// <param name="dataInd">Data index.</param>
        /// <returns>Tile.</returns>
        private byte[,] GetTile(PaletteMode mode, ref int dataInd) {
            byte[,] t = new byte[8, 8];
            return t;
        }
    
    }

}
