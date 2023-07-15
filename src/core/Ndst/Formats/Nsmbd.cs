using System.IO;

namespace Ndst.Formats {

    // New Super Mario Bros. Domain sponsored model.
    public class Nsmbd : IFormat {

        public bool IsType(byte[] data) {
            return data.Length >= 4 && data[0] == 'B' && data[1] == 'M' && data[2] == 'D' && data[3] == '0';
        }

        public void Read(BinaryReader r, byte[] rawData) {

            // Header info.
            long basePos = r.BaseStream.Position;
            r.BaseStream.Position = basePos + 0xD;
            ushort numBlocks = r.ReadUInt16();
            bool readTex = numBlocks > 1;
            uint mdlOff = r.ReadUInt32();
            r.BaseStream.Position = basePos + mdlOff;

            // Read the model information.
            

        }

        public void Write(BinaryWriter w) {
            throw new System.NotImplementedException();
        }

        public void Extract(string path) {
            throw new System.NotImplementedException();
        }

        public void Pack(string path) {
            throw new System.NotImplementedException();
        }

        public string GetFormat() {
            return "Nsmbd";
        }

        public byte[] ContainedFile() {
            return null;
        }

        public bool IsOfFormat(string str) {
            return str.Equals("Nsmbd");
        }

        public string GetPathExtension() => "";

    }

}