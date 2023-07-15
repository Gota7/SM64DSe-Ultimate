using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ndst.Formats;
using Newtonsoft.Json;

namespace Ndst {
    
    // DS Unit Code.
    public enum UnitCode {
        NDS,
        Reserved,
        NDS_DSI,
        DSI
    }
    
    // NDS Rom.
    public class ROM {
        public string GameTitle;
        public string GameCode;
        public string MakerCode;
        public UnitCode UnitCode;
        public byte EncryptionSeedSelect;
        public byte DeviceCapacity;
        public ushort Revision;
        public byte Version;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public byte Flags;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint Arm9EntryAddress;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint Arm9LoadAddress;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint Arm7EntryAddress;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint Arm7LoadAddress;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint NormalCardControlRegisterSettings;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint SecureCardControlRegisterSettings;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public ushort SecureAreaCRC;
        public ushort SecureTransferTimeout;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint Arm9Autoload;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint Arm7Autoload;
        public ulong SecureDisable;
        [JsonConverter(typeof(HexStringJsonConverter))]
        public uint HeaderSize;
        [JsonIgnore]
        public byte[] NintendoLogo;
        [JsonIgnore]
        public Filesystem Filesystem;
        [JsonIgnore]
        public List<Overlay> Arm9Overlays = new List<Overlay>();
        [JsonIgnore]
        public List<Overlay> Arm7Overlays = new List<Overlay>();
        [JsonIgnore]
        public byte[] Arm9;
        [JsonIgnore]
        public byte[] Arm7;
        [JsonIgnore]
        public byte[] Banner;
        [JsonIgnore]
        ConversionInfo ConversionInfo;

        // Banner lengths.
        public static readonly Dictionary<ushort, uint> BANNER_LENGTHS = new Dictionary<ushort, uint>() {
            { 0x0001, 0x840 },
            { 0x0002, 0x940 },
            { 0x0003, 0x1240 },
            { 0x0103, 0x23C0 },
        };

        // Needed for JSON.
        internal ROM() {}

        // Create a ROM from extracted content.
        public ROM(string srcPath) {
            Pack(srcPath);
        }
        
        // Create a new ROM.
        public ROM(string filePath, string conversionPath) {

            // Build system.
            if (!conversionPath.Equals("")) {
                Directory.CreateDirectory(conversionPath);
                ConversionInfo = new ConversionInfo(conversionPath);
            }

            // Read the file.
            using (Stream s = new FileStream(filePath, FileMode.Open)) {

                // Read general data.
                BinaryReader r = new BinaryReader(s);
                GameTitle = r.ReadFixedLen(0xC);
                GameCode = r.ReadFixedLen(0x4);
                MakerCode = r.ReadFixedLen(0x2);
                UnitCode = (UnitCode)r.ReadByte();
                EncryptionSeedSelect = r.ReadByte();
                DeviceCapacity = r.ReadByte();
                r.ReadBytes(7);
                Revision = r.ReadUInt16();
                Version = r.ReadByte();
                Flags = r.ReadByte();
                uint arm9Offset = r.ReadUInt32();
                Arm9EntryAddress = r.ReadUInt32();
                Arm9LoadAddress = r.ReadUInt32();
                uint arm9Size = r.ReadUInt32();
                uint arm7Offset = r.ReadUInt32();
                Arm7EntryAddress = r.ReadUInt32();
                Arm7LoadAddress = r.ReadUInt32();
                uint arm7Size = r.ReadUInt32();
                uint fntOffset = r.ReadUInt32();
                uint fntSize = r.ReadUInt32();
                uint fatOffset = r.ReadUInt32();
                uint fatSize = r.ReadUInt32();
                uint arm9OverlayOffset = r.ReadUInt32();
                uint arm9OverlayLength = r.ReadUInt32();
                uint arm7OverlayOffset = r.ReadUInt32();
                uint arm7OverlayLength = r.ReadUInt32();
                NormalCardControlRegisterSettings = r.ReadUInt32();
                SecureCardControlRegisterSettings = r.ReadUInt32();
                uint iconBannerOffset = r.ReadUInt32();
                SecureAreaCRC =  r.ReadUInt16();
                SecureTransferTimeout = r.ReadUInt16();
                Arm9Autoload = r.ReadUInt32();
                Arm7Autoload = r.ReadUInt32();
                SecureDisable = r.ReadUInt64();
                uint romSize = r.ReadUInt32() + 0x88;
                HeaderSize = r.ReadUInt32();

                // Read logo.
                r.BaseStream.Position = 0xC0;
                NintendoLogo = r.ReadBytes(0x9C);
                r.ReadUInt16(); // Nintendo logo CRC. It is literally just the CRC of the logo.
                r.ReadUInt16(); // Header CRC. It is the CRC of everything up to this point (0 to 0x15E).

                // Get banner.
                r.BaseStream.Position = iconBannerOffset;
                ushort bannerVersion = r.ReadUInt16();
                r.BaseStream.Position -= 2;
                Banner = r.ReadBytes((int)BANNER_LENGTHS[bannerVersion]);

                // Code binaries.
                r.BaseStream.Position = arm9Offset;
                Arm9 = r.ReadBytes((int)arm9Size);
                r.BaseStream.Position = arm7Offset;
                Arm7 = r.ReadBytes((int)arm7Size);

                // Get overlays.
                Arm9Overlays = Overlay.LoadOverlays(r, arm9OverlayOffset, arm9OverlayLength);
                Arm7Overlays = Overlay.LoadOverlays(r, arm7OverlayOffset, arm7OverlayLength);
                for (int i = 0; i < Arm9Overlays.Count; i++) {
                    r.BaseStream.Position = fatOffset + Arm9Overlays[i].FileId * 8;
                    uint fileStart = r.ReadUInt32();
                    uint fileEnd = r.ReadUInt32();
                    r.BaseStream.Position = fileStart;
                    Arm9Overlays[i].Data = r.ReadBytes((int)(fileEnd - fileStart));
                }
                for (int i = 0; i < Arm7Overlays.Count; i++) {
                    r.BaseStream.Position = fatOffset + Arm7Overlays[i].FileId * 8;
                    uint fileStart = r.ReadUInt32();
                    uint fileEnd = r.ReadUInt32();
                    r.BaseStream.Position = fileStart;
                    Arm7Overlays[i].Data = r.ReadBytes((int)(fileEnd - fileStart));
                }

                // Read filesystem.
                Filesystem = new Filesystem(r, fntOffset, fntSize, fatOffset, fatSize, !conversionPath.Equals(""), ConversionInfo);
                
                // Dispose.
                r.Dispose();

            }

        }

        // Save the ROM.
        public void Save(string filePath) {
            
            // New file.
            using (FileStream s = new FileStream(filePath, FileMode.OpenOrCreate)) {

                // Writer setup.
                s.SetLength(0);
                BinaryWriter w = new BinaryWriter(s);

                // Write header.
                w.WriteFixedLen(GameTitle, 0xC);
                w.WriteFixedLen(GameCode, 0x4);
                w.WriteFixedLen(MakerCode, 0x2);
                w.Write((byte)UnitCode);
                w.Write(EncryptionSeedSelect);
                w.Write(DeviceCapacity);
                w.Write0s(7);
                w.Write(Revision);
                w.Write(Version);
                w.Write(Flags);
                w.SaveOffset("arm9Offset");
                w.Write(Arm9EntryAddress);
                w.Write(Arm9LoadAddress);
                w.Write((uint)Arm9.Length);
                w.SaveOffset("arm7Offset");
                w.Write(Arm7EntryAddress);
                w.Write(Arm7LoadAddress);
                w.Write((uint)Arm7.Length);
                w.SaveOffset("fntOffset");
                w.SaveOffset("fntSize");
                w.SaveOffset("fatOffset");
                w.SaveOffset("fatSize");
                w.SaveOffset("arm9OverlayOffset");
                w.Write((uint)(Arm9Overlays.Count * 0x20));
                w.SaveOffset("arm7OverlayOffset");
                w.Write((uint)(Arm7Overlays.Count * 0x20));
                w.Write(NormalCardControlRegisterSettings);
                w.Write(SecureCardControlRegisterSettings);
                w.SaveOffset("iconBannerOffset");
                w.Write(SecureAreaCRC);
                w.Write(SecureTransferTimeout);
                w.Write(Arm9Autoload);
                w.Write(Arm7Autoload);
                w.Write(SecureDisable);
                w.SaveOffset("romSize");
                w.Write(HeaderSize);
                w.Write0s(0xC0 - (uint)w.BaseStream.Position);

                // Write logo and CRCs.
                w.Write(NintendoLogo);
                w.Write(CalcCRC(NintendoLogo));
                BinaryReader r = new BinaryReader(s);
                long bakPos = w.BaseStream.Position;
                r.BaseStream.Position = 0;
                ushort crc = CalcCRC(r.ReadBytes((int)bakPos));
                w.BaseStream.Position = bakPos;
                w.Write(crc);
                w.Align(HeaderSize);

                // Write Arm9.
                w.WriteOffset("arm9Offset");
                w.Write(Arm9);
                w.Align(0x200);

                // Arm9 overlay table.
                List<Tuple<long, uint, Overlay>> arm9OverlayOffs = new List<Tuple<long, uint, Overlay>>();
                if (Arm9Overlays.Count > 0) {
                    w.WriteOffset("arm9OverlayOffset");
                    Overlay.WriteOverlays(w, Arm9Overlays);
                    w.Align(0x200);
                    foreach (var o in Arm9Overlays) {
                        arm9OverlayOffs.Add(new Tuple<long, uint, Overlay>(w.BaseStream.Position, (uint)o.Data.Length, o));
                        w.Write(o.Data);
                        w.Align(0x200);
                    }
                } else {
                    w.WriteOffset("arm9OverlayOffset", 0, 0);
                }

                // Write Arm7.
                w.WriteOffset("arm7Offset");
                w.Write(Arm7);
                w.Align(0x200);

                // Arm7 overlay table.
                List<Tuple<long, uint, Overlay>> arm7OverlayOffs = new List<Tuple<long, uint, Overlay>>();
                if (Arm7Overlays.Count > 0) {
                    w.WriteOffset("arm7OverlayOffset");
                    Overlay.WriteOverlays(w, Arm7Overlays);
                    w.Align(0x200);
                    foreach (var o in Arm7Overlays) {
                        arm7OverlayOffs.Add(new Tuple<long, uint, Overlay>(w.BaseStream.Position, (uint)o.Data.Length, o));
                        w.Write(o.Data);
                        w.Align(0x200);
                    }
                } else {
                    w.WriteOffset("arm7OverlayOffset", 0, 0);
                }

                // Write filesystem.
                Filesystem.WriteFilesystem(w, arm9OverlayOffs, arm7OverlayOffs, Banner);

            }

        }

        // Calculate the CRC.
        public ushort CalcCRC(byte[] bytes) {
            uint crc = 0xFFFF;
            uint[] val = {0xC0C1, 0xC181, 0xC301, 0xC601, 0xCC01, 0xD801, 0xF001, 0xA001};
            for (uint i = 0; i < bytes.Length; i++) {
                crc ^= bytes[i];
                for (int j = 0; j < 8; j++) {
                    bool carry = ((crc & 0x1) == 0x1);
                    crc >>= 1;
                    if (carry)
                        crc ^= (val[j] << (7 - j));
                }
            }
            return (ushort)crc;
        }

        // Extract the filesystem. So in order for this to work, an "original filesystem" will be extracted, and a "patch filesystem" will have all the edits and file additions.
        public void Extract(string destFolder) {

            // Create folder if needed.
            Directory.CreateDirectory(destFolder);

            // Extract ROM info.
            Directory.CreateDirectory(destFolder + "/" + "__ROM__");
            System.IO.File.WriteAllText(destFolder + "/" + "__ROM__" + "/" + "header.json", JsonConvert.SerializeObject(this, Formatting.Indented));
            System.IO.File.WriteAllBytes(destFolder + "/" + "__ROM__" + "/" + "nintendoLogo.bin", NintendoLogo);
            System.IO.File.WriteAllBytes(destFolder + "/" + "__ROM__" + "/" + "banner.bin", Banner);

            // Extract code.
            Directory.CreateDirectory(destFolder + "/" + "__ROM__");
            System.IO.File.WriteAllBytes(destFolder + "/" + "__ROM__" + "/" + "arm9.bin", Arm9);
            System.IO.File.WriteAllBytes(destFolder + "/" + "__ROM__" + "/" + "arm7.bin", Arm7);
            System.IO.File.WriteAllText(destFolder + "/" + "__ROM__" + "/" + "arm9Overlays.json", JsonConvert.SerializeObject(Arm9Overlays, Formatting.Indented));
            System.IO.File.WriteAllText(destFolder + "/" + "__ROM__" + "/" + "arm7Overlays.json", JsonConvert.SerializeObject(Arm7Overlays, Formatting.Indented));
            Directory.CreateDirectory(destFolder + "/" + "__ROM__" + "/" + "Arm9");
            Directory.CreateDirectory(destFolder + "/" + "__ROM__" + "/" + "Arm7");
            foreach (var o in Arm9Overlays) {
                System.IO.File.WriteAllBytes(destFolder + "/" + "__ROM__" + "/" + "Arm9" + "/" + o.Id + ".bin", o.Data);
            }
            foreach (var o in Arm7Overlays) {
                System.IO.File.WriteAllBytes(destFolder + "/" + "__ROM__" + "/" + "Arm7" + "/" + o.Id + ".bin", o.Data);
            }

            // Extract files.
            List<Tuple<string, ushort>> fileInfo = new List<Tuple<string, ushort>>();
            void ExtractFiles(string path, string relativePath, Folder folder) {
                Directory.CreateDirectory(path);
                foreach (var f in folder.Folders) {
                    ExtractFiles(path + "/" + f.Name, relativePath + "/" + f.Name, f);
                }
                foreach (var f in folder.Files) {
                    string fInfo = relativePath + "/" + f.Name;
                    fInfo += " 0x" + f.Id.ToString("X");
                    if (ConversionInfo == null) f.Data.Extract(path + "/" + f.Name);
                    fileInfo.Add(new Tuple<string, ushort>(fInfo, f.Id));
                }
            }
            ExtractFiles(destFolder, "..", Filesystem);
            fileInfo = fileInfo.OrderBy(x => x.Item2).ToList();
            System.IO.File.WriteAllLines(destFolder + "/" + "__ROM__" + "/files.txt", fileInfo.Select(x => x.Item1));
            if (ConversionInfo != null) {
                ConversionInfo.WriteBuiltFiles(destFolder);
                ConversionInfo.WriteConversionInfo();
            }

        }

        // Pack a ROM.
        public void Pack(string romFolder) {

            // File reading content.
            byte[] ReadFile(string path) {
                return System.IO.File.ReadAllBytes(romFolder + "/" + path);
            }
            string[] ReadFileList(string path) {
                return System.IO.File.ReadAllLines(romFolder + "/" + path);
            }
            T ReadJSON<T>(string path) {
                return JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(romFolder + "/" + path));
            }
            List<ushort> validFileIds = new List<ushort>();
            void VerifyFiles(IEnumerable<ushort> fileIds) {
                foreach (var u in fileIds) {
                    if (validFileIds.Contains(u)) {
                        throw new Exception("ERROR: Duplicate overlay file ID in use: 0x" + u.ToString("X"));
                    } else {
                        validFileIds.Add(u);
                    }
                }
            }

            // Get header info and copy it.
            ROM headerInfo = ReadJSON<ROM>("__ROM__/header.json");
            GameTitle = headerInfo.GameTitle;
            GameCode = headerInfo.GameCode;
            MakerCode = headerInfo.MakerCode;
            UnitCode = headerInfo.UnitCode;
            EncryptionSeedSelect = headerInfo.EncryptionSeedSelect;
            DeviceCapacity = headerInfo.DeviceCapacity;
            Revision = headerInfo.Revision;
            Version = headerInfo.Version;
            Flags = headerInfo.Flags;
            Arm9EntryAddress = headerInfo.Arm9EntryAddress;
            Arm9LoadAddress = headerInfo.Arm9LoadAddress;
            Arm7EntryAddress = headerInfo.Arm7EntryAddress;
            Arm7LoadAddress = headerInfo.Arm7LoadAddress;
            NormalCardControlRegisterSettings = headerInfo.NormalCardControlRegisterSettings;
            SecureCardControlRegisterSettings = headerInfo.SecureCardControlRegisterSettings;
            SecureAreaCRC = headerInfo.SecureAreaCRC;
            SecureTransferTimeout = headerInfo.SecureTransferTimeout;
            Arm9Autoload = headerInfo.Arm9Autoload;
            Arm7Autoload = headerInfo.Arm7Autoload;
            SecureDisable = headerInfo.SecureDisable;
            HeaderSize = headerInfo.HeaderSize;

            // Get the Nintendo logo and banner.
            NintendoLogo = ReadFile("__ROM__/nintendoLogo.bin");
            Banner = ReadFile("__ROM__/banner.bin");

            // Fetch Arm payloads.
            Arm9 = ReadFile("__ROM__/arm9.bin");
            Arm7 = ReadFile("__ROM__/arm7.bin");

            // Get overlays.
            Arm9Overlays = ReadJSON<List<Overlay>>("__ROM__/arm9Overlays.json");
            VerifyFiles(Arm9Overlays.Select(x => x.FileId));
            Arm7Overlays = ReadJSON<List<Overlay>>("__ROM__/arm7Overlays.json");
            VerifyFiles(Arm7Overlays.Select(x => x.FileId));
            foreach (var o in Arm9Overlays) {
                o.Data = ReadFile("__ROM__/Arm9/" + o.Id + ".bin");
            }
            foreach (var o in Arm7Overlays) {
                o.Data = ReadFile("__ROM__/Arm7/" + o.Id + ".bin");
            }

            // Read files.
            ushort currFolderId = 1;
            string[] fileList = ReadFileList("__ROM__/files.txt");
            Dictionary<string, Folder> folders = new Dictionary<string, Folder>();
            Filesystem = new Filesystem();
            folders.Add("", Filesystem);
            foreach (var s in fileList) {
                AddFileToFilesystem(s);
            }

            // Add a file to the filesystem.
            void AddFileToFilesystem(string s) {

                // First get its properties.
                string[] fileProperties = s.Split(' ');
                string filePath = fileProperties[0];
                if (!filePath.StartsWith("../")) {
                    throw new Exception("ERROR: All files must be relative to parent directory \"../\"");
                } else {
                    filePath = filePath.Substring(3);
                }

                // Get file ID.
                ushort fileId = (ushort)Helper.ReadStringNumber(fileProperties[1]);
                if (validFileIds.Contains(fileId)) {
                    throw new Exception("ERROR: File ../" + filePath + " uses duplicate file ID 0x" + fileId.ToString("X"));
                } else {
                    validFileIds.Add(fileId);
                }

                // Proper file name and folder path.
                string fileName = filePath;
                string folderPath = "";
                while (fileName.Contains('/')) {
                    folderPath += "/" + fileName.Substring(0, fileName.IndexOf('/'));
                    fileName = fileName.Substring(fileName.IndexOf('/') + 1);
                }
                if (folderPath.StartsWith("/")) {
                    folderPath = folderPath.Substring(1);
                }

                // New file.
                File f = new File();
                f.Name = fileName;
                f.Id = fileId;
                f.Data = new GenericFile() { Data = ReadFile(filePath) };

                // Finally add the file to the folder.
                AddFileToFolder(f, folderPath);

            }

            // Add a file to a folder.
            void AddFileToFolder(File f, string folderPath) {

                // First check if the folder exists.
                if (!folders.ContainsKey(folderPath)) {
                    AppendFolder(folderPath);
                }

                // Add the file to the folder.
                folders[folderPath].Files.Add(f);

            }

            // Append a folder.
            void AppendFolder(string folderPath) {
                string folderName = folderPath;
                string baseFolderPath = "";
                while (folderName.Contains('/')) {
                    baseFolderPath += "/" + folderName.Substring(0, folderName.IndexOf('/'));
                    folderName = folderName.Substring(folderName.IndexOf('/') + 1);
                }
                if (baseFolderPath.StartsWith("/")) {
                    baseFolderPath = baseFolderPath.Substring(1);
                }
                if (!folders.ContainsKey(baseFolderPath)) {
                    AppendFolder(baseFolderPath);
                }
                Folder f = new Folder() { Id = currFolderId++, Name = folderName };
                folders[baseFolderPath].Folders.Add(f);
                folders.Add(folderPath, f);
            }

            // Fix first file IDs.
            foreach (var f in folders.Values) {
                // f.Files = f.Files.OrderBy(x => x.Name).ToList(); = I don't think this should be done.
                if (f.Files.Count > 0) f.FirstFileId = f.Files.OrderBy(x => x.Id).ElementAt(0).Id;
            }

            // Fix folders with no files.
            void FixFolders(Folder f) {
                foreach (var folder in f.Folders) {
                    FixFolders(folder);
                }
                if (f.Files.Count == 0) {
                    f.FirstFileId = f.Folders[0].FirstFileId;
                }
            }
            FixFolders(folders[""]);

        }

    }

}