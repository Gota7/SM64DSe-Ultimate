using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ndst {

    // Extract options.
    public abstract class ExtractOptions {

        // Deserialize extract options.
        public static T Deserialize<T>(string conversionFolder, string path) where T : ExtractOptions {
            return JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(conversionFolder + "/" + path));
        }

        // Serialize extract options.
        public static void SerializeExtractOptions(ExtractOptions options, string conversionFolder, string path) {
            System.IO.File.WriteAllText(conversionFolder + "/" + path, JsonConvert.SerializeObject(options, Formatting.Indented));
        }

        // Convert files.
        public abstract List<string> GetFileList();
        public abstract void ExtractFiles();
        public abstract void PackFiles();

    }

}