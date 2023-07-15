using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ndst.Formats;

namespace Ndst {

    // Conversion stage.
    public class ConversionStage {
        public string FromConversionFilePath;
        public string ConversionStep;
    }

    // Conversion information.
    public class ConversionInfo {
        string ConversionFolder;
        Dictionary<string, List<ConversionStage>> Files = new Dictionary<string, List<ConversionStage>>();
        Dictionary<string, IFormat> CurrentFormats = new Dictionary<string, IFormat>();

        public ConversionInfo(string conversionFolder) {
            ConversionFolder = conversionFolder;
        }

        // Add a file to convert.
        public void AddFileConversion(string filePath, string conversion, IFormat latestConversion) {
            if (!Files.ContainsKey(filePath)) {
                Files.Add(filePath, new List<ConversionStage>());
                CurrentFormats.Add(filePath, latestConversion);
            }
            Files[filePath].Add(new ConversionStage() { ConversionStep = conversion });
            CurrentFormats[filePath] = latestConversion;
        }

        // TODO: Multiple file conversions!!!

        // Write conversion info.
        public void WriteConversionInfo() {
            List<string> ret = new List<string>();
            foreach (var f in Files) {
                if (f.Value.Count < 1 || f.Value.Where(x => !x.ConversionStep.Equals("None")).Count() == 0) {
                    continue;
                }
                ret.Add(f.Key);
                f.Value.Reverse();
                foreach (var c in f.Value) {
                    if (c.ConversionStep.Equals("None")) {
                        continue;
                    }
                    ret.Add("\t- " + (c.FromConversionFilePath == null ? c.ConversionStep : ("*" + c.FromConversionFilePath)));
                }
                f.Value.Reverse();
            }
            System.IO.File.WriteAllLines(ConversionFolder + "/conversions.txt", ret);
        }

        // Write built files.
        public void WriteBuiltFiles(string folder) {
            foreach (var f in CurrentFormats) {
                if (f.Value != null) {
                    Directory.CreateDirectory(Path.GetDirectoryName(folder + "/" + f.Key));
                    f.Value.Extract(folder + "/" + f.Key);
                }
            }
        }

    }

}