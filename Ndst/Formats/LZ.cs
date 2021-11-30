using System;
using System.IO;

namespace Ndst.Formats {

    // An LZ compressed file.
    public class LZFile : IFormat {
        public bool HasHeader;
        public byte[] CompressedData;

        public bool IsType(byte[] data) {
            if (data.Length > 4) {
                if (data[0] == 'L' && data[1] == 'Z' && data[2] == '7' && data[3] == '7') {
                    HasHeader = true;
                    return true;
                } else if (data[0] == 0x10) {
                    HasHeader = false;
                    byte[] testData = new byte[data.Length];
                    Array.Copy(data, testData, testData.Length);
                    try {
                        LZ77.Decompress(ref testData, false);
                        return true;
                    } catch {}
                }
            }
            return false;
        }

        public void Read(BinaryReader r, byte[] rawData) {
            CompressedData = rawData;
        }

        public void Write(BinaryWriter w) {
            w.Write(CompressedData);
        }

        public void Extract(string path) {

            // Decompress data.
            byte[] testData = new byte[CompressedData.Length];
            Array.Copy(CompressedData, testData, testData.Length);
            LZ77.Decompress(ref testData, HasHeader);

            // Get folder and file info.
            string fileName = Path.GetFileName(path);
            string folder = Path.GetDirectoryName(path);

            // Write the uncompressed and compressed files.
            System.IO.File.WriteAllBytes(path, testData);
            Directory.CreateDirectory(folder + "/.LZ");
            System.IO.File.WriteAllBytes(folder + "/.LZ/" + fileName + ".LZ", CompressedData);
            System.IO.File.WriteAllBytes(folder + "/.LZ/" + fileName + ".md5", Helper.Md5Sum(testData));

        }

        public void Pack(string path, string romPath, string patchPath) {
            
            // Check LZ info if exists.
            bool usePatch = System.IO.File.Exists(patchPath + "/" + path);
            string fileName = Path.GetFileName(path);
            string folder = Path.GetDirectoryName(path);
            string patchPrefix = (usePatch ? patchPath : romPath) + "/";
            if (System.IO.File.Exists(patchPrefix + folder + "/.LZ/" + fileName + ".LZ") && System.IO.File.Exists(patchPrefix + folder + "/.LZ/" + fileName + ".md5")) {
                byte[] hash = Helper.ReadROMFile(folder + "/.LZ/" + fileName + ".md5", romPath, patchPath);
                byte[] file = Helper.ReadROMFile(path, romPath, patchPath);
                if (Helper.BytesMatch(hash, Helper.Md5Sum(file))) {
                    CompressedData = Helper.ReadROMFile(folder + "/.LZ/" + fileName + ".LZ", romPath, patchPath);
                } else {
                    RecompressData(file);
                }
            } else {
                RecompressData(Helper.ReadROMFile(path, romPath, patchPath));
            }

            // Recompress data.
            void RecompressData(byte[] data) {
                byte[] hash = Helper.Md5Sum(data);
                LZ77.LZ77_Compress(ref data, HasHeader);
                CompressedData = data;
                Directory.CreateDirectory(patchPrefix + folder + "/.LZ");
                System.IO.File.WriteAllBytes(patchPrefix + folder + "/.LZ/" + fileName + ".LZ", CompressedData);
                System.IO.File.WriteAllBytes(patchPrefix + folder + "/.LZ/" + fileName + ".md5", hash);
            }

        }

        public string GetFormat() {
            return HasHeader ? "LZH" : "LZ";
        }

        public bool IsOfFormat(string str) {
            if (str.Equals("LZ")) {
                HasHeader = false;
                return true;
            } else if (str.Equals("LZH")) {
                HasHeader = true;
                return true;
            }
            return false;
        }

    }

}