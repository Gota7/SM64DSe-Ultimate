using System.Collections.Generic;

namespace Ndst.Models {

    // A mesh that contains mesh info.
    public class Mesh {
        public Bone Bone = new Bone();
        public int MaterialIndex;
        public List<Face> Faces = new List<Face>();
        public List<Vec3> Vertices = new List<Vec3>();
        public List<Vec3> VertexTextures = new List<Vec3>();
        public List<Vec3> VertexColors = new List<Vec3>();
        public List<Vec3> VertexNormals = new List<Vec3>();
        bool HasTextures => VertexTextures.Count > 0;
        bool HasColors => VertexColors.Count > 0;
        bool HasNormals => VertexNormals.Count > 0;
    }

}