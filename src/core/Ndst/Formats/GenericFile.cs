using System.IO;

namespace Ndst.Formats {

    // Generic data.
    public class GenericFile : IFormat {
        public byte[] Data;

        public bool IsType(byte[] data) {
            return true;
        }

        public void Read(BinaryReader r, byte[] rawData) {
            Data = rawData;
        }

        public void Write(BinaryWriter w) {
            w.Write(Data);
        }

        public void Extract(string path) {
            System.IO.File.WriteAllBytes(path, Data);
        }

        public void Pack(string path) {
            Data = System.IO.File.ReadAllBytes(path);
        }

        public string GetFormat() {
            return "None";
        }

        public bool IsOfFormat(string str) {
            return true;
        }

        public byte[] ContainedFile() {
            return null;
        }

        public string GetPathExtension() => "";

    }

}