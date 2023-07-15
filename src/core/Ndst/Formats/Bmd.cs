using System.IO;

namespace Ndst.Formats {

    // SM64DS BMD model.
    public class Bmd : IFormat {

        public bool IsType(byte[] data) {
            return true;
        }

        public void Read(BinaryReader r, byte[] rawData) {
            
        }

        public void Write(BinaryWriter w) {
            
        }

        public void Extract(string path) {
            throw new System.NotImplementedException();
        }

        public void Pack(string path) {
            throw new System.NotImplementedException();
        }

        public string GetFormat() {
            return "Bmd";
        }

        public byte[] ContainedFile() {
            return null;
        }

        public bool IsOfFormat(string str) {
            return str.Equals("Bmd");
        }

        public string GetPathExtension() => "";

    }

}