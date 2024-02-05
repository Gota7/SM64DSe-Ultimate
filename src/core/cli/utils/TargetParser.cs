using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SM64DSe.core.cli.utils
{
    public static class TargetParser
    {
        
        public static Dictionary<string, string> ParseFile(string filePath)
        {
            if (Path.GetExtension(filePath) == ".txt")
            {
                return ParseTxtFile(filePath);
            }
            else if (Path.GetExtension(filePath) == ".json")
            {
                return ParseJsonFile(filePath);
            }
            else
            {
                throw new ArgumentException("Unsupported file format. Supported formats are .txt and .json");
            }
        }

        static Dictionary<string, string> ParseTxtFile(string filePath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    string[] parts = line.Trim().Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        result.Add(key, value);
                    }
                    else
                    {
                        throw new FormatException("Malformed line in the .txt file");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading or parsing .txt file: {ex.Message}", ex);
            }

            return result;
        }

        static Dictionary<string, string> ParseJsonFile(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                JObject jsonObject = JObject.Parse(jsonContent);

                Dictionary<string, string> result = jsonObject.ToObject<Dictionary<string, string>>();

                if (result == null)
                {
                    throw new FormatException("Invalid JSON format. The file should contain a valid dictionary (string => string).");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading or parsing .json file: {ex.Message}", ex);
            }
        }
    }
}