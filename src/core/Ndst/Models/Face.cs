using System.Collections.Generic;

namespace Ndst.Models {

    // A 3d face.
    public class Face {
        public List<int> VertexIndices = new List<int>();
        public List<int> VertexColorIndices = new List<int>();
        public List<int> VertexTextureIndices = new List<int>();
        public List<int> VertexNormalIndices = new List<int>();
        public bool HasColors => VertexColorIndices.Count > 0;
        public bool HasTextures => VertexColorIndices.Count > 0;
        public bool HasNormalIndices => VertexNormalIndices.Count > 0;
    }

}