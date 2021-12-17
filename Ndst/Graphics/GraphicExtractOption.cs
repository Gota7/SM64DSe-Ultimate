using System.Collections.Generic;

namespace Ndst.Graphics {

    // Extract a graphic.
    public class GraphicExtractOptions : ExtractOptions {
        public int ScreenWidthTiles;
        public int ScreenHeightTiles;
        public bool Affine;
        public string ScreenMaskPath;
        public string ScreenPath;
        public int GraphicWidthTiles;
        public int GraphicHeightTiles;
        public bool Is4Bpp;
        public bool OptimizeGraphic;
        public bool FirstColorTransparent;
        public string GraphicPath;
        public int ColorsPerPalette;
        public string PalettePath;
        public string OutputPath;

        public override void ExtractFiles()
        {
            throw new System.NotImplementedException();
        }

        public override List<string> GetFileList() {
            List<string> ret = new List<string>();
            if (ScreenPath != null) ret.Add(ScreenPath);
            if (GraphicPath != null) ret.Add(GraphicPath);
            if (PalettePath != null) ret.Add(PalettePath);
            return ret;
        }

        public override void PackFiles()
        {
            throw new System.NotImplementedException();
        }
        
    }

}