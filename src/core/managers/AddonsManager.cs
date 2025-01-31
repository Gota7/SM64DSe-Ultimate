using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Serilog;
using SM64DSe.core.cli;
using SM64DSe.core.utils.Github;

namespace SM64DSe.core.managers
{
    public class AddonObject
    {
        [JsonProperty("name")]
        public string Name;
        
        [JsonProperty("repository")]
        public string Repository;
        
        [JsonProperty("description")]
        public string Description;
    }

    public class LocalAddon
    {
        [JsonProperty("Parent")]
        public AddonObject Parent;
        
        [JsonProperty("path")]
        public string Path;
        
        [JsonProperty("versions")]
        public string[] Versions;
    }
    
    public class AddonsManager
    {
        public static string CommandFile = "commands.sm64ds";
        
        private AddonsManager() {}

        private static AddonsManager _instance;

        private bool _isWorking = false;
        
        public static AddonsManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AddonsManager();
            }
            return _instance;
        }

        public List<AddonObject> GetAddons()
        {
            if (!File.Exists("assets/addons.json"))
            {
                Log.Warning("Cannot find assets/addons.json file.");
                return new List<AddonObject>();
            }

            using (StreamReader r = new StreamReader("assets/addons.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<AddonObject>>(json);
            }
        }

        /**
         * This method will check inside the `addons` folder for valid version
         *
         * (1) It will iterate over all directories
         * (2) It will check if those directories match to a corresponding addon in the `addons.json`
         * (3) It will iterate in those folder to check for versions (a version is valid if it contains a `commands.sm64ds` file
         */
        public List<LocalAddon> GetLocalAddons()
        {
            List<LocalAddon> output = new List<LocalAddon>();
            string addonsFolder = GetAddonsFolder();
            if (!Directory.Exists(addonsFolder))
                return output;

            List<AddonObject> addons = GetAddons();
            Dictionary<string, AddonObject> dict = new Dictionary<string, AddonObject>();
            foreach (AddonObject addonObject in addons)
            {
                dict.Add(Sanitize(addonObject.Name), addonObject);
            }
            
            string[] dirs = Directory.GetDirectories(addonsFolder);
            foreach (string dir in dirs)
            {
                LocalAddon localAddon = new LocalAddon();
                localAddon.Path = dir;
                
                string name = new DirectoryInfo(dir).Name;
                if (dict.ContainsKey(name)) // we have a corresponding addon
                {
                    localAddon.Parent = dict[name];
                }
                
                // Let's iterate over all subfolder to detect if they are valid version
                List<string> paths = new List<string>();
                string[] versions = Directory.GetDirectories(dir);
                foreach (string version in versions)
                {
                    if(!File.Exists(Path.Combine(version, CommandFile)))
                    {
                        Log.Warning($"subfolder {version} does not contain {CommandFile} file - ignoring.");
                        continue;
                    }
                    paths.Add(version);
                }

                localAddon.Versions = paths.ToArray();
                output.Add(localAddon);
            }

            return output;
        }

        public List<GitHubRelease> GetGitHubRelease(AddonObject addon)
        {
            if (addon.Repository.StartsWith("https://github.com/"))
            {
                return GitHubHelper.GetReleases(addon.Repository.Substring("https://github.com/".Length));
            }
            return new List<GitHubRelease>();
        }

        public void DownloadAndExtract(AddonObject addon, GitHubRelease release)
        {
            Log.Information($"Starting downloading release {release.Name}.");
            
            if (_isWorking)
            {
                throw new Exception("Cannot download and extract multiple releases at the same time.");
            }
            
            try
            {
                _isWorking = true;
                this.PerformDownloadAndExtract(addon, release);
            }
            catch (Exception ex)
            {
                Log.Error($"Something went wrong while downloading and extracting release {release.Name}: {ex.Message}");
            }
            finally
            {
                _isWorking = false;
            }
        }

        public string GetAddonsFolder()
        {
            if (Program.m_ROM == null || Program.m_ROM.m_Path == null)
                throw new Exception("m_ROM need to be not null.");
            
            string parentDirectory = Directory.GetParent(Program.m_ROM.m_Path).FullName;
            return Path.Combine(parentDirectory, "addons");
        }

        private void PerformDownloadAndExtract(AddonObject addon, GitHubRelease release)
        {
            string addons = GetAddonsFolder();
            if(!Directory.Exists(addons))
            {
                Log.Warning($"Creating directory {addons}.");
                Directory.CreateDirectory(addons);
            }
            
            Log.Information($"Downloading {release.ZipBallUrl}.");
            string target = Path.Combine(addons, "tmp.zip");
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent: SM64DSe-Ultimate");
                client.DownloadFile(release.ZipBallUrl, target);
            }
            
            FastZip fastZip = new FastZip();

            string targetDir = Path.Combine(addons, Sanitize(addon.Name));
            
            Log.Debug($"Extracting {target} to {targetDir}.");
            fastZip.ExtractZip(target, targetDir, null);
            
            File.Delete(target);
            Log.Information("Download and extract finish.");
        }

        public void PerformInstall(LocalAddon addon, int versionSelected)
        {
            if (addon.Versions.Length < versionSelected || versionSelected < 0)
                throw new Exception("Invalid version selected.");

            string path = addon.Versions[versionSelected];
            string command = Path.Combine(path, CommandFile);
            if (!File.Exists(command))
                throw new Exception($"Missing {CommandFile} in version selected.");
            
            CLIService.Run(new[] {"batches", command, "--force", "--with-progress-bar"});
            
            Log.Debug("batches finished.");
            

            string objects = Path.Combine(path, "objects.json");
            if (File.Exists(objects))
            {
                Log.Warning("The addon installed own an objects.json - it will be copied.");
                ObjectDatabase.LoadFromFile(objects);
                UpdateObjects(
                    Path.Combine(Directory.GetParent(Program.m_ROM.m_Path).FullName, "objects.json"),
                    objects
                    );
            }
        }
        
        private static void UpdateObjects(string firstFilePath, string secondFilePath) 
        {
            try
            {
                // Check if the first file exists
                bool firstFileExists = File.Exists(firstFilePath);

                List<ObjectDatabase.ObjectInfo> firstObjects;
                List<ObjectDatabase.ObjectInfo> secondObjects;

                // Read content of the second file
                using (StreamReader file = File.OpenText(secondFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    secondObjects = (List<ObjectDatabase.ObjectInfo>)serializer.Deserialize(file, typeof(List<ObjectDatabase.ObjectInfo>));
                }

                // If the first file exists, read its content and merge with the second file
                if (firstFileExists)
                {
                    using (StreamReader file = File.OpenText(firstFilePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        firstObjects = (List<ObjectDatabase.ObjectInfo>)serializer.Deserialize(file, typeof(List<ObjectDatabase.ObjectInfo>));
                    }

                    // HashSet to store unique IDs encountered
                    HashSet<ushort> uniqueIDs = new HashSet<ushort>();

                    // Add IDs from firstObjects to HashSet
                    foreach (var obj in firstObjects)
                    {
                        uniqueIDs.Add(obj.m_ID);
                    }

                    // Merge the contents of the second file while avoiding duplicates
                    foreach (var obj in secondObjects)
                    {
                        if (!uniqueIDs.Contains(obj.m_ID))
                        {
                            firstObjects.Add(obj);
                            uniqueIDs.Add(obj.m_ID);
                        }
                    }
                }
                else
                {
                    // If the first file doesn't exist, simply copy the second file
                    firstObjects = secondObjects;
                }

                // Write the merged or copied content back to the first file
                using (StreamWriter file = File.CreateText(firstFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, firstObjects);
                }

                Console.WriteLine("Objects updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static string Sanitize(string name)
        {
            string nName = name.Replace(' ', '-');
            nName = nName.Replace('.', '_');
            nName = nName.Replace('/', '_');
            nName = nName.Replace('\\', '_');
            nName = nName.Replace('"', '_');
            nName = nName.Replace('\'', '_');
            return nName;
        }
    }
}