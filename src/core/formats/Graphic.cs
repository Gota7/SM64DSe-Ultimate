using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        /// Get the number of palettes.
        /// </summary>
        /// <param name="mode">Palette mode.</param>
        /// <returns>Number of palettes.</returns>
        public int NumPalettes(PaletteMode mode) => mode == PaletteMode.Bpp4 ? Colors.Length / 16 : Colors.Length / 256;

        /// <summary>
        /// Get the number of colors in the last palette.
        /// </summary>
        /// <param name="mode">Palette mode.</param>
        /// <returns>Number of colors in the last palette.</returns>
        public int ColorsInLastPalette(PaletteMode mode) => mode == PaletteMode.Bpp4 ? (Colors.Length % 16 == 0 ? 16 : Colors.Length % 16) : (Colors.Length % 256 == 0 ? 256 : Colors.Length % 256);

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
                byte r = (byte)((raw & 0b11111) >> 0);
                byte g = (byte)((raw & 0b1111100000) >> 5);
                byte b = (byte)((raw & 0b111110000000000) >> 10);
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
                ushort raw = (ushort)(r & 0b11111);
                raw |= (ushort)((g & 0b11111) << 5);
                raw |= (ushort)((b & 0b11111) << 10);
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

        /// <summary>
        /// Generate a palette.
        /// </summary>
        /// <param name="colors">Colors used. Repeats are encouraged.</param>
        /// <param name="paletteNum">Starting palette ID to replace. This always assumes 4bpp.</param>
        /// <param name="numRows">Number of rows of 16 to replace.</param>
        /// <param name="alphaFirst">If the fist color in the palette is always transparent.</param>
        public void GenPalette(List<Color> colors, int paletteNum, int numRows, bool alphaFirst = true) {
            byte[] rawColors = new byte[colors.Count * 3];
            for (int i = 0; i < colors.Count; i++) {
                rawColors[i * 3] = colors[i].B;
                rawColors[i * 3 + 1] = colors[i].G;
                rawColors[i * 3 + 2] = colors[i].R;
            }
            int alphaOff = alphaFirst ? 1 : 0;
            NeuQuant nq = new NeuQuant(rawColors, colors.Count, 7, numRows * 16 - alphaOff);
            byte[] ret = nq.Process();
            int startColor = paletteNum * 16;
            List<Color> newColors = Colors.ToList();
            for (int i = 0; i < numRows * 16; i++) {
                if (i == 0 && alphaFirst) {
                    newColors[startColor] = Color.FromArgb(0, 0, 0, 0);
                    continue;
                }
                if (newColors.Count <= startColor + i) {
                    newColors.Add(new Color());
                }
                newColors[startColor + i] = Color.FromArgb(ret[i * 3 + 2], ret[i * 3 + 1], ret[i * 3]);
                Colors = newColors.ToArray();
            }

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
        /// Number of tiles.
        /// </summary>
        /// <param name="mode">Mode.</param>
        /// <returns>Number of tiles.</returns>
        public int NumTiles(PaletteMode mode) {
            if (mode == PaletteMode.Bpp4) {
                return Data.Length / 32;
            } else {
                return Data.Length / 64;
            }
        }

        /// <summary>
        /// Get a tile.
        /// </summary>
        /// <param name="mode">Palette mode.</param>
        /// <param name="dataInd">Data index.</param>
        /// <returns>Tile.</returns>
        private byte[,] GetTile(PaletteMode mode, ref int dataInd) {
            byte[,] t = new byte[8, 8];
            bool isFrontNybble = true;
            for (int r = 0; r < 8; r++) {
                for (int c = 0; c < 8; c++) {
                    if (mode == PaletteMode.Bpp4) {
                        if (isFrontNybble) {
                            t[c, r] = (byte)((Data[dataInd] & 0xF0) >> 4);
                        } else {
                            t[c, r] = (byte)(Data[dataInd] & 0xF);
                            dataInd++;
                        }
                        isFrontNybble = !isFrontNybble;
                    } else {
                        t[c, r] = Data[dataInd];
                        dataInd++;
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// Get the tiles in the NCG.
        /// </summary>
        /// <param name="mode">Palette mode.</param>
        /// <returns>Tiles.</returns>
        public byte[][,] GetTiles(PaletteMode mode) {
            int numTiles = NumTiles(mode);
            byte[][,] tiles = new byte[numTiles][,];
            int dataInd = 0;
            for (int i = 0; i < numTiles; i++) {
                tiles[i] = GetTile(mode, ref dataInd);
            }
            return tiles;
        }

        /// <summary>
        /// Get a list of possible dimensions for the graphic.
        /// </summary>
        /// <param name="mode">Palette mode.</param>
        /// <returns>List of possible dimensions.</returns>
        public List<Tuple<int, int>> PossibleDimensions(PaletteMode mode) {
            int numTiles = NumTiles(mode);
            List<Tuple<int, int>> possible = new List<Tuple<int, int>>();
            int currNum = 8;
            List<int> dividers = new List<int>();
            while (currNum < numTiles) {
                if (numTiles % 8 == 0) {
                    dividers.Add(currNum);
                    dividers.Add(numTiles / currNum);
                    possible.Add(new Tuple<int, int>(currNum, numTiles / currNum));
                }
                currNum += 8;
            }
            return possible;
        }

        /// <summary>
        /// Generate a tile.
        /// </summary>
        /// <param name="mode">Palette mode.</param>
        /// <param name="pal">Palette.</param>
        /// <param name="paletteNum">Palette number.</param>
        /// <param name="data">Bitmap data.</param>
        /// <param name="xPtr">X position in bitmap.</param>
        /// <param name="yPtr">Y position in bitmap.</param>
        /// <returns>Generated tile.</returns>
        private List<byte> GenTile(PaletteMode mode, NCL pal, int paletteNum, Bitmap data, ref int xPtr, ref int yPtr) {
            List<byte> ret = new List<byte>();
            bool isFirstNybble = true;
            int numPixelsDone = 0;
            int scanX = 0;
            int scanY = 0;
            while (numPixelsDone < 64) {
                if (mode == PaletteMode.Bpp4) {
                    if (isFirstNybble) {
                        ret.Add((byte)(pal.GetClosestColor(data.GetPixel(xPtr + scanX, yPtr + scanY), paletteNum, mode) << 4));
                    } else {
                        ret[ret.Count - 1] |= (byte)pal.GetClosestColor(data.GetPixel(xPtr + scanX, yPtr + scanY), paletteNum, mode);
                        numPixelsDone++;
                        scanX++;
                    }
                    isFirstNybble = !isFirstNybble;
                } else {
                    ret.Add((byte)pal.GetClosestColor(data.GetPixel(xPtr + scanX, yPtr + scanY), paletteNum, mode));
                    numPixelsDone++;
                    scanX++;
                }
                if (scanX == 8) {
                    scanX = 0;
                    scanY++;
                }
            }
            xPtr += 8;
            yPtr += 8;
            return ret;
        }

        /// <summary>
        /// Set a graphic tile.
        /// </summary>
        /// <param name="tileNum">Tile number to set.</param>
        /// <param name="mode">Palette mode.</param>
        /// <param name="pal">Palette.</param>
        /// <param name="paletteNum">Palette number.</param>
        /// <param name="data">Bitmap data.</param>
        /// <param name="dataXPos">Bitmap data X position.</param>
        /// <param name="dataYPos">Bitmap data Y position.</param>
        public void SetTile(int tileNum, PaletteMode mode, NCL pal, int paletteNum, Bitmap data, int dataXPos, int dataYPos) {
            List<byte> newTileData = GenTile(mode, pal, paletteNum, data, ref dataXPos, ref dataYPos);
            int dataOff = tileNum * 64;
            if (mode == PaletteMode.Bpp4) {
                dataOff = tileNum * 32;
            }
            for (int i = 0; i < newTileData.Count; i++) {
                Data[dataOff + i] = newTileData[i];
            }
        }

    }

    /// <summary>
    /// Screen data.
    /// </summary>
    public class NSC {

        /// <summary>
        /// Palette mode.
        /// </summary>
        public PaletteMode PaletteMode;

        /// <summary>
        /// Width of the NSC.
        /// </summary>
        public uint Width;

        /// <summary>
        /// Height of the NSC.
        /// </summary>
        public uint Height;

        /// <summary>
        /// 8-bits per character, unless the BG is extended.
        /// </summary>
        public bool AffineMode;

        /// <summary>
        /// Tiles.
        /// </summary>
        public Tile[] Tiles;

        /// <summary>
        /// Tile.
        /// </summary>
        public class Tile {

            /// <summary>
            /// NCG Id.
            /// </summary>
            public ushort Id;

            /// <summary>
            /// Palette Id.
            /// </summary>
            public byte PalId;

            /// <summary>
            /// Flip horizontally.
            /// </summary>
            public bool FlipX;

            /// <summary>
            /// Flip vertically.
            /// </summary>
            public bool FlipY;

        }

    }

}
