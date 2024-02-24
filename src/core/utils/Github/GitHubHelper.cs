using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Serilog;

namespace SM64DSe.core.utils.Github
{ 
    public class GitHubRelease
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("body")]
        public string Body { get; set; }
        
        [JsonProperty("zipball_url")]
        public string ZipBallUrl { get; set; }
        
        [JsonProperty("published_at")]
        public string PublishedAt { get; set; }
        
        [JsonProperty("prerelease")]
        public bool Prerelease { get; set; }
    }

    class GitHubHelper
    {
        private const string GitHubApiUrl = "https://api.github.com/repos/";

        public static List<GitHubRelease> GetReleases(string repository)
        {
            try
            {
                string apiUrl = GitHubApiUrl + repository + "/releases";
                string jsonResponse = SendRequest(apiUrl);
                var releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(jsonResponse);
                return releases;
            }
            catch (WebException ex)
            {
                Log.Error($"WebException occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occurred: {ex.Message}");
            }

            return new List<GitHubRelease>();
        }

        private static string SendRequest(string apiUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0";
            request.Accept = "application/vnd.github.v3+json";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}