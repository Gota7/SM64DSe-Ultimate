using System;
using System.IO;

namespace Ndst.Formats {

    // Format interface.
    public interface IFormat {
        bool IsType(byte[] data);
        void Read(BinaryReader r, byte[] rawData);
        void Write(BinaryWriter w);
        void Extract(string path);
        void Pack(string path);
        string GetFormat();
        bool IsOfFormat(string str);
        byte[] ContainedFile();
        string GetPathExtension();
    }

    // Format tools.
    public static class FormatUtil {

        // Do conversion during extraction.
        public static IFormat DoExtractionConversion(ConversionInfo c, BinaryReader r, long fileOff, string originalFilePath, byte[] file, bool parentWasLZ = false) {
            IFormat newData = null;
            foreach (var pFormat in Helper.FileFormats) {
                newData = (IFormat)Activator.CreateInstance(pFormat);
                if (newData.IsType(file)) {
                    if (newData as LZFile != null && parentWasLZ) {
                        continue; // No double LZ compression.
                    }
                    if (newData as Enpg != null && !originalFilePath.EndsWith(".enpg")) {
                        continue; // Only assume valid Enpgs are ones that have the extension.
                    }
                    if (newData as Narc != null) {
                        Narc.ConversionInfo = c; // Must do this to allow conversion.
                    }
                    r.BaseStream.Position = fileOff;
                    newData.Read(r, file);
                    byte[] containedFile = newData.ContainedFile();
                    c.AddFileConversion(originalFilePath, newData.GetFormat(), newData);
                    if (containedFile != null) {
                        using (MemoryStream src = new MemoryStream(containedFile)) {
                            BinaryReader br = new BinaryReader(src);
                            DoExtractionConversion(c, br, 0, originalFilePath, containedFile, newData as LZFile != null);
                        }
                    }
                    break;
                }
            }
            return newData;
        }

    }

}