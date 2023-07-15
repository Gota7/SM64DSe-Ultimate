using System.IO;

namespace Ndst.Graphics {

    // Contains tiles.
    public class Graphic {
        public Palette Palette;
        public bool Is4BPP;
        public bool FirstColorTransparent;
        public bool IsEnpg;
        public Tile[,] Tiles;

        // A tile.
        public class Tile {
            public byte[,] Colors = new byte[8,8];
            
            public void Read(BinaryReader r, bool is4BPP) {
                if (is4BPP) {
                    for (int i = 0; i < 8; i++) {
                        for (int j = 0; j < 4; j++) {
                            byte col = r.ReadByte();
                            Colors[j * 2, i] = (byte)(col & 0x0F);
                            Colors[j * 2 + 1, i] = (byte)((col & 0xF0) >> 4);
                        }
                    }
                } else {
                    for (int i = 0; i < 8; i++) {
                        for (int j = 0; j < 8; j++) {
                            Colors[j, i] = r.ReadByte();
                        }
                    }
                }
            }

            public void Write(BinaryWriter w, bool is4BPP) {
                if (is4BPP) {
                    for (int i = 0; i < 8; i++) {
                        for (int j = 0; j < 4; j++) {
                            byte col = (byte)((Colors[j * 2 + 1, i] << 4) & 0xF0);
                            col |= (byte)(Colors[j * 2, i] & 0x0F);
                            w.Write(col);
                        }
                    }
                } else {
                    for (int i = 0; i < 8; i++) {
                        for (int j = 0; j < 8; j++) {
                            w.Write(Colors[j, i]);
                        }
                    }
                }
            }

            public Tile FlipX() {
                Tile ret = new Tile();
                for (int i = 0; i < 8; i++) {
                    for (int j = 0; j < 8; j++) {
                        ret.Colors[j, i] = Colors[8 - j - 1, i];
                    }
                }
                return ret;
            }

            public Tile FlipY() {
                Tile ret = new Tile();
                for (int i = 0; i < 8; i++) {
                    for (int j = 0; j < 8; j++) {
                        ret.Colors[i, j] = Colors[i, 8 - j - 1];
                    }
                }
                return ret;
            }

        }

        public void Read(BinaryReader r, int widthInTiles, int heightInTiles, bool is4BPP, bool firstColorTransparent, bool isEnpg = false) {
            Is4BPP = is4BPP;
            IsEnpg = isEnpg;
            FirstColorTransparent = firstColorTransparent;
            Tiles = new Tile[widthInTiles, heightInTiles];
            if (IsEnpg) {
                for (int i = 0; i < widthInTiles; i++) {
                    for (int j = 0; j < heightInTiles; j++) {
                        Tiles[i, j] = new Tile();
                    }
                }
                for (int i = 0; i < heightInTiles * 8; i++) { // Enpg goes by row and not by tile. Why???
                    for (int j = 0; j < widthInTiles * 8; j++) {
                        int tileX = j / 8;
                        int pixelX = j % 8;
                        int tileY = i / 8;
                        int pixelY = i % 8;
                        Tiles[tileX, tileY].Colors[pixelX, pixelY] = r.ReadByte();
                    }
                }
            } else {
                for (int i = 0; i < heightInTiles; i++) {
                    for (int j = 0; j < widthInTiles; j++) {
                        Tiles[j, i] = new Tile();
                        Tiles[j, i].Read(r, Is4BPP);
                    }
                }
            }
        }

        public void Write(BinaryWriter w) {
            if (IsEnpg) {
                for (int i = 0; i < Tiles.GetLength(1) * 8; i++) {
                    for (int j = 0; j < Tiles.GetLength(0) * 8; j++) {
                        int tileX = j / 8;
                        int pixelX = j % 8;
                        int tileY = i / 8;
                        int pixelY = i % 8;
                        w.Write(Tiles[tileX, tileY].Colors[pixelX, pixelY]);
                    }
                }
            } else {
                for (int i = 0; i < Tiles.GetLength(1); i++) {
                    for (int j = 0; j < Tiles.GetLength(0); j++) {
                        Tiles[j, i].Write(w, Is4BPP);
                    }
                }
            }
        }

    }

}