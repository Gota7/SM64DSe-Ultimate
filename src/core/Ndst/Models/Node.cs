using System.Collections.Generic;

namespace Ndst.Models {

    // A node that contains a mesh index.
    public class Node {
        public string Name = "";
        public Node Parent;
        public List<Node> Children = new List<Node>();
        public List<int> Meshes = new List<int>();

        public void AddNode(Node n) {
            n.Parent = this;
        }
        
    }

}