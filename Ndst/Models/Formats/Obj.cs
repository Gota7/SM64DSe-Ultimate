using System.Collections.Generic;

namespace Ndst.Models {

    public class WavefrontObj : IModelFormat
    {
        public string FormatName() => "Obj";

        public bool IsOfFormat(string filePath) {
            return filePath.ToLower().EndsWith(".obj");
        }

        public Model FromFile(string filePath) {
            throw new System.NotImplementedException();
        }

        public void WriteFile(string filePath, Model m) {

            // Set up.
            List<string> o = new List<string>();
            Dictionary<int, int> vertOffs = new Dictionary<int, int>();
            Dictionary<int, int> texOffs = new Dictionary<int, int>();
            Dictionary<int, int> NormOffs = new Dictionary<int, int>();
            int vertNum = 0;
            int vertTexNum = 0;
            int vertNormNum = 0;
            foreach (var h in m.Meshes) {
                foreach (var v in h.Vertices) {
                    
                }
            }

            // Finish writing the model.
            System.IO.File.WriteAllLines(filePath, o);

            // Observe a mesh.
            void ObserveMesh(Mesh h) {

            }

            // Write a mesh.
            void WriteMesh(Mesh h, int id) {
                int vertOff = vertOffs[id];
                foreach (var f in h.Faces) {
                    string[] vertices = new string[f.VertexIndices.Count];
                    for (int i = 0; i < vertices.Length; i++) {
                        vertices[i] = (vertOff + f.VertexIndices[i]).ToString();
                        if (f.HasTextures) {
                            vertices[i] += "/" + f.VertexTextureIndices[i];
                        }
                    }
                    o.Add("f " + string.Join(" ", vertices));
                }
            }

        }

    }

}