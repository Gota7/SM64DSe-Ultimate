using System;
using System.Collections.Generic;
using System.Linq;
using Ndst.Formats;
using Newtonsoft.Json;

namespace Ndst {

    // File conversion.
    public class FileConversion {
        public string Conversion; // Conversion ID.
        public int ConversionNumber; // Number of the conversion to do.
        public bool FromMassConversion; // If from converting multiple files.
        public string CustomExtension; // Custom file extension.

        public FileConversion(string conversion, int conversionNumber, bool fromMassConversion = false, string customExtension = "") {
            Conversion = conversion;
            ConversionNumber = conversionNumber;
            FromMassConversion = fromMassConversion;
            CustomExtension = customExtension;
        }

    }

    // Mass file conversion.
    public class MassFileConversion {
        public Dictionary<string, FileConversion> Files = new Dictionary<string, FileConversion>();
        public string ConversionPath;
        public string ConversionType;

        public MassFileConversion(Dictionary<string, FileConversion> files, string conversionPath, string conversionType) {
            Files = files;
            ConversionPath = conversionPath;
            ConversionType = conversionType;
        }

    }

    // Ninja build system.
    public class NinjaBuildSystem {
        string RomPath;
        string PatchPath;
        string ConversionPath;
        List<MassFileConversion> Conversions = new List<MassFileConversion>();
        Dictionary<string, int> NumFileConversions = new Dictionary<string, int>();
        Dictionary<string, List<FileConversion>> FileConversions = new Dictionary<string, List<FileConversion>>();

        // New system.
        public NinjaBuildSystem(string romPath, string patchPath, string conversionPath = null) {
            RomPath = romPath;
            PatchPath = patchPath;
            ConversionPath = conversionPath;
        }

        // Add file.
        public void AddFile(string filePath) {
            NumFileConversions.Add(filePath, 0);
            FileConversions.Add(filePath, new List<FileConversion>());
            FileConversions[filePath].Add(new FileConversion("copy", NumFileConversions[filePath]++, false)); // First conversion is always copy.
        }

        // Add a single file conversion (like compression).
        public void AddFileConversion(string filePath, string conversion, string customExtension) {
            FileConversions[filePath].Add(new FileConversion(conversion, NumFileConversions[filePath]++, false, customExtension));
        }

        // Add a conversion that involves multiple files (like graphics).
        public void AddConversion(List<string> filesInvolved, string conversionFile, string conversionType) {
            Dictionary<string, FileConversion> fs = new Dictionary<string, FileConversion>();
            foreach (var f in filesInvolved) {
                var fc = new FileConversion("", NumFileConversions[f]++, true);
                fs.Add(f, fc);
                FileConversions[f].Add(fc);
            }
            MassFileConversion m = new MassFileConversion(fs, conversionFile, conversionType);
        }

        // Make sure NARCs get all files built.
        public void AddAllNarcFiles(string[] conversionFile, string romPath, string patchPath) {
            string currFile = "";
            foreach (var str in conversionFile) {
                    string s = str.Replace(" ", "").Replace("\t", "");
                    if (s.StartsWith("-")) {
                        s = s.Substring(1);
                        if (s.Equals("Narc")) {
                            currFile += ".txt";
                            var archiveList = Helper.ReadROMLines(currFile, romPath, patchPath);
                            foreach (var f in archiveList) {
                                string newFile = f.Split(' ')[0];
                                if (!newFile.StartsWith("../")) throw new Exception("Archive file paths must start with ../");
                                AddFile(newFile.Substring(3));
                            }
                        }
                    } else {
                        currFile = s;
                    }
                }
        }

        // Get copy.
        private string GetSelectiveCopyBuild(string path) {
            string fetchedPath = "";
            if (System.IO.File.Exists(PatchPath + "/" + path)) {
                fetchedPath = PatchPath.Replace(" ", "$ ").Replace(":", "$:") + "/" + path.Replace(" ", "$ ").Replace(":", "$:");
            } else {
                fetchedPath = RomPath.Replace(" ", "$ ").Replace(":", "$:") + "/" + path.Replace(" ", "$ ").Replace(":", "$:");
            }
            return fetchedPath;
        }

        // Write a build system.
        public void WriteBuildSystem(string outputNds) {

            // Ninja.
            List<string> n = new List<string>();

            // Write rules.
            n.Add("rule copy");
            n.Add("  command = ./Ndst -c $in $out");
            n.Add("");
            n.Add("rule ndst");
            n.Add("  command = ./Ndst $args $in $out");
            n.Add("rule pack");
            n.Add("  command = ./Ndst -p $folder $out");
            n.Add("  rspfile = tmp.rsp");
            n.Add("  rspfile_content = $in");

            // ROM files.
            n.Add("");
            n.Add("build build/__ROM__/header.json: copy " + GetSelectiveCopyBuild("__ROM__/header.json"));
            n.Add("build build/__ROM__/arm9Overlays.json: copy " + GetSelectiveCopyBuild("__ROM__/arm9Overlays.json"));
            n.Add("build build/__ROM__/arm7Overlays.json: copy " + GetSelectiveCopyBuild("__ROM__/arm7Overlays.json"));
            n.Add("build build/__ROM__/arm9.bin: copy " + GetSelectiveCopyBuild("__ROM__/arm9.bin"));
            n.Add("build build/__ROM__/arm7.bin: copy " + GetSelectiveCopyBuild("__ROM__/arm7.bin"));
            n.Add("build build/__ROM__/nintendoLogo.bin: copy " + GetSelectiveCopyBuild("__ROM__/nintendoLogo.bin"));
            n.Add("build build/__ROM__/banner.bin: copy " + GetSelectiveCopyBuild("__ROM__/banner.bin"));
            n.Add("build build/__ROM__/files.txt: copy " + GetSelectiveCopyBuild("__ROM__/files.txt"));

            // Overlays.
            List<Overlay> ov9List = JsonConvert.DeserializeObject<List<Overlay>>(Helper.ReadROMText("__ROM__/arm9Overlays.json", RomPath, PatchPath));
            foreach (var o in ov9List) {
                n.Add("build build/__ROM__/Arm9/" + o.Id + ".bin: copy " + GetSelectiveCopyBuild("__ROM__/Arm9/" + o.Id + ".bin"));
            }
            List<Overlay> ov7List = JsonConvert.DeserializeObject<List<Overlay>>(Helper.ReadROMText("__ROM__/arm7Overlays.json", RomPath, PatchPath));
            foreach (var o in ov7List) {
                n.Add("build build/__ROM__/Arm7/" + o.Id + ".bin: copy " + GetSelectiveCopyBuild("__ROM__/Arm7/" + o.Id + ".bin"));
            }

            // Write conversions.
            List<string> builtFiles = new List<string>();
            foreach (var c in Conversions) {
                string outBuilds = "";
                foreach (var f in c.Files) {
                    outBuilds += f.Key.Replace(" ", "$ ").Replace(":", "$:") + "." + f.Value.ConversionNumber + " ";
                }
                n.Add("build " + outBuilds.Substring(0, outBuilds.Length - 1) + ": ndst " + ConversionPath + "/" + c.ConversionPath);
                n.Add("  args = -t conversion");
            }

            // Write file conversions. Do it in reverse though due to NARC stuff.
            foreach (var f in FileConversions.Reverse()) {

                // Write file conversions.
                for (int i = 0; i < f.Value.Count; i++) {
                    string noSpacesPath = "build/" + f.Key.Replace(" ", "$ ").Replace(":", "$:");
                    string inPath = noSpacesPath + "." + (f.Value[i].ConversionNumber - 1);
                    string outPath = noSpacesPath + "." + f.Value[i].ConversionNumber;
                    string conversion = f.Value[i].Conversion;
                    if (f.Value[i].ConversionNumber == f.Value.Count - 1) {
                        outPath = noSpacesPath;
                        builtFiles.Add(outPath);
                    }
                    if (f.Value[i].ConversionNumber == 0) {
                        string extAppend = "";
                        foreach (var v in f.Value) {
                            extAppend += v.CustomExtension;
                        }
                        n.Add("build " + outPath + ": copy " + GetSelectiveCopyBuild(f.Key) + extAppend);
                        continue;
                    }
                    if (!f.Value[i].FromMassConversion) {
                        n.Add("build " + outPath + ": ndst " + inPath.Replace(" ", "$").Replace(":", "$:"));
                        n.Add("  args = -t " + f.Value[i].Conversion);
                    }
                }
                
            }

            // Finally, build the Nds file.
            n.Add("build " + outputNds + ": pack " + String.Join(" ", builtFiles));
            n.Add("  folder = build"); // Hack because I'm lazy.

            // Write build.
            System.IO.File.WriteAllLines("build.ninja", n);

        }

        // Generate a build system.
        public static void GenerateBuildSystem(string romFolder, string patchFolder, string conversionPath, string outputRomName) {

            // Start.
            NinjaBuildSystem n = new NinjaBuildSystem(romFolder, patchFolder, conversionPath);
            var fileList = Helper.ReadROMLines("__ROM__/files.txt", romFolder, patchFolder);
            foreach (var f in fileList) {
                string fileName = f;
                if (fileName.Contains(" ")) { fileName = f.Split(' ')[0]; }
                n.AddFile(fileName.Substring(3)); // Remove ../
            }

            // Add conversions.
            if (!conversionPath.Equals("")) {
                string currFile = "";
                string[] conversionFile = System.IO.File.ReadAllLines(conversionPath + "/conversions.txt");
                n.AddAllNarcFiles(conversionFile, romFolder, patchFolder);
                foreach (var str in conversionFile) {
                    string s = str.Replace(" ", "").Replace("\t", "");
                    if (s.StartsWith("-")) {
                        s = s.Substring(1);
                        if (s.StartsWith("*")) {
                            // TODO!!!
                        } else {
                            string ext = "";
                            foreach (var format in Helper.FileFormats) {
                                IFormat i = (IFormat)Activator.CreateInstance(format);
                                if (i.IsOfFormat(s)) {
                                    ext = i.GetPathExtension();
                                    break;
                                }
                            }
                            n.AddFileConversion(currFile, s, ext);
                        }
                    } else {
                        currFile = s;
                    }
                }
            }

            // Write build system.
            n.WriteBuildSystem(outputRomName);

        }

    }

}