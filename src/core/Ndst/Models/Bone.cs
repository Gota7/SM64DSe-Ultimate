using System.Collections.Generic;

namespace Ndst.Models {

    // A bone.
    public class Bone {
        public Mat4 Offset = Mat4.Identity;
        public Dictionary<int, float> VertexWeights = new Dictionary<int, float>();
    }

}