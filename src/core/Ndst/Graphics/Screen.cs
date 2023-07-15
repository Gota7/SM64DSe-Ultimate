using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Ndst.Graphics {

    [Flags]
    public enum FlipFlags : byte {
        None = 0,
        Horizontal = 0b1 << 0,
        Vertical = 0b1 << 1
    }

    // Lays out graphic tiles.
    public class Screen {
        public Graphic Graphic;
        public bool Affine;
        public Tile[,] Tiles;

        // A screen tile.
        public class Tile {
            public ushort TileIndex;
            public byte PaletteIndex;
            public FlipFlags FlipFlags;

            public void Read(BinaryReader r, bool affine, ref ushort affinePrefix, int tileCount) {
                if (affine) {
                    PaletteIndex = 0;
                    FlipFlags = FlipFlags.None;
                    ushort index = r.ReadByte();
                    TileIndex = (ushort)(index + affinePrefix);
                    if (index == 0xFF) {
                        affinePrefix += 0x100;
                    }
                    if (affinePrefix >= tileCount) {
                        affinePrefix = 0;
                    }
                } else {
                    ushort val = r.ReadUInt16();
                    TileIndex = (ushort)(val & 0x3FF);
                    PaletteIndex = (byte)((val >> 12) & 0xF);
                    FlipFlags = FlipFlags.None;
                    if ((val & 0x400) > 0) FlipFlags |= FlipFlags.Horizontal;
                    if ((val & 0x800) > 0) FlipFlags |= FlipFlags.Vertical;
                }
            }

        }

        public static Screen GenerateDefault(Graphic g) {
            Screen s = new Screen();
            s.Affine = false;
            s.Graphic = g;
            s.Tiles = new Tile[g.Tiles.GetLength(0), g.Tiles.GetLength(1)];
            for (int i = 0; i < g.Tiles.GetLength(1); i++) {
                for (int j = 0; j < g.Tiles.GetLength(0); j++) {
                    s.Tiles[j, i] = new Tile();
                    s.Tiles[j, i].TileIndex = (ushort)(i * g.Tiles.GetLength(0) + j);
                }
            }
            return s;
        }

        public void Read(BinaryReader r, int widthInTiles, int heightInTiles, bool affine) {
            Affine = affine;
            Tiles = new Tile[widthInTiles, heightInTiles];
            ushort affinePrefix = 0;
            for (int i = 0; i < heightInTiles; i++) {
                for (int j = 0; j < widthInTiles; j++) {
                    Tiles[j, i] = new Tile();
                    Tiles[j, i].Read(r, Affine, ref affinePrefix, widthInTiles * heightInTiles);
                }
            }
        }

        public Image<Argb32> ToImage() {
            Image<Argb32> ret = new Image<Argb32>(Configuration.Default, Tiles.GetLength(0) * 8, Tiles.GetLength(1) * 8);
            for (int i = 0; i < Tiles.GetLength(0); i++) {
                for (int j = 0; j < Tiles.GetLength(1); j++) {
                    WriteScreenTile(i, j);
                }
            }
            return ret;
            void WriteScreenTile(int tileX, int tileY) {
                Tile t = Tiles[tileX, tileY];
                int indexX = t.TileIndex % Graphic.Tiles.GetLength(0);
                int indexY = t.TileIndex / Graphic.Tiles.GetLength(0);
                var g = Graphic.Tiles[indexX, indexY];
                if ((t.FlipFlags & FlipFlags.Horizontal) > 0) { g = g.FlipX(); }
                if ((t.FlipFlags & FlipFlags.Vertical) > 0) { g = g.FlipY(); }
                WriteGraphicTile(g, tileX * 8, tileY * 8, Graphic.Palette, t.PaletteIndex);
            }
            void WriteGraphicTile(Graphic.Tile t, int x, int y, Palette pal, int palIndex) {
                for (int i = 0; i < 8; i++) {
                    for (int j = 0; j < 8; j++) {
                        Argb32 col = new Argb32();
                        int ind = t.Colors[i, j];
                        if (ind == 0 && Graphic.FirstColorTransparent) {
                            col.A = 0;
                        } else {
                            col.A = 255;
                            var c = pal.Colors[ind + palIndex * pal.IndexSize];
                            col.R = c.R8;
                            col.G = c.G8;
                            col.B = c.B8;
                        }
                        ret[x + i, y + j] = col;
                    }
                }
            }
        }

        // Create a screen from an image.
        public static Screen FromImage(Image<Argb32> img, bool optimizeGraphic, bool affine, bool is4Bpp, bool firstColorTransparent, bool isEnpg, int graphicWidthTiles, int graphicHeightTiles, int colorsPerPalette, Argb32? backgroundColor) {

            // TODO: ADD SUPPORT FOR OPTIMIZING THE GRAPHIC! (Identical tiles, flips, etc.)

            // Dimensions check.
            if (!optimizeGraphic) {
                if (img.Width != graphicWidthTiles * 8 || img.Height != graphicHeightTiles * 8) {
                    throw new Exception("Image size must be " + (graphicWidthTiles * 8) + "x" + (graphicHeightTiles * 8) + "!");
                }
            } else {
                if (img.Width % 8 != 0 || img.Height % 8 != 0) {
                    throw new Exception("Image size must be a multiple of 8 in both dimensions!");
                }
            }

            // Create stuff.
            Screen s = new Screen();
            s.Affine = affine;
            s.Graphic = new Graphic();
            s.Graphic.FirstColorTransparent = firstColorTransparent;
            s.Graphic.Is4BPP = is4Bpp;
            s.Graphic.IsEnpg = isEnpg;
            s.Graphic.Tiles = new Graphic.Tile[graphicWidthTiles, graphicHeightTiles];
            s.Graphic.Palette = new Palette();
            s.Graphic.Palette.IndexSize = colorsPerPalette;

            // Take into account background/transparent color.
            bool oneLessColor = backgroundColor != null || firstColorTransparent;
            int numColorsToGenerate = colorsPerPalette;
            if (oneLessColor) numColorsToGenerate--;
            if (firstColorTransparent) backgroundColor = new Argb32(0, 0, 0, 0);

            // Get the graphic.
            int[,] newGraphic;
            s.Graphic.Palette.Colors = Palette.LimitColorPalette(img, numColorsToGenerate, backgroundColor, out newGraphic);
            if (oneLessColor) s.Graphic.Palette.Colors.Insert(0, new RGB5(0));
            for (int i = 0; i < graphicWidthTiles * 8; i++) {
                for (int j = 0; j < graphicHeightTiles * 8; j++) {
                    int tileX = i / 8;
                    int tileY = j / 8;
                    int pixelX = i % 8;
                    int pixelY = j % 8;
                    if (pixelX == 0 && pixelY == 0) {
                        s.Graphic.Tiles[tileX, tileY] = new Graphic.Tile();
                    }
                    s.Graphic.Tiles[tileX, tileY].Colors[pixelX, pixelY] = (byte)newGraphic[i, j];
                }
            }
            var sTemp = Screen.GenerateDefault(s.Graphic);
            s.Tiles = sTemp.Tiles;
            while (s.Graphic.Palette.Colors.Count < s.Graphic.Palette.IndexSize) {
                s.Graphic.Palette.Colors.Add(new RGB5());
            }

            // Return the final screen.
            return s;

        }

    }

}