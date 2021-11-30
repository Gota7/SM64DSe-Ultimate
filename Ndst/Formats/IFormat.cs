using System.IO;

namespace Ndst.Formats {
    public interface IFormat {
        bool IsType(byte[] data);
        void Read(BinaryReader r, byte[] rawData);
        void Write(BinaryWriter w);
        void Extract(string path);
        void Pack(string path, string romPath, string patchPath);
        string GetFormat();
        bool IsOfFormat(string str);
    }
}