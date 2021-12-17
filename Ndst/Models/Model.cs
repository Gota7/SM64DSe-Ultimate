using System.Collections.Generic;

namespace Ndst.Models {

    // 3D model.
    public class Model : Node {
        public bool EnableMaterials;
        public List<Mesh> Meshes = new List<Mesh>();

        // Default constructor.
        public Model() {}
        
        // From a model file.
        public Model(string filePath) {
            
        }

        // Write a model.
        public void SaveModel(string filePath, string format) {

        }

    }

}