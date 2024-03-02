using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Serilog;
using SM64DSe.core.utils.Github;
using SM64DSe.Properties;

namespace SM64DSe.core.updater
{
    public class Updater
    {
        public static GitHubRelease CheckUpdate()
        {
            if (Program.AppVersion == "dev@next")
                return null;
            
            var attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyCodeSource), false)
                .Cast<AssemblyCodeSource>();
            if (!attributes.Any())
            {
                Log.Warning("No AssemblyCodeSource found.");
                return null;
            }

            AssemblyCodeSource assemblyCodeSource = attributes.First();
            string repository = assemblyCodeSource.Value;
            Log.Debug($"Binaries from {repository}.");

            List<GitHubRelease> releases;
            try
            {
                releases = GitHubHelper.GetReleases(repository);
            }
            catch (Exception e)
            {
                Log.Warning("Something went wrong while trying to get repository releases. Aborting.", e);
                return null;
            }

            if (releases.Count == 0)
            {
                Log.Warning($"No release was find for repository {repository}");
                return null;
            }

            if (releases[0].Name == Program.AppVersion)
            {
                Log.Debug("The editor is up to date.");
                return null;
            }

            return releases[0];
        }
    }
}