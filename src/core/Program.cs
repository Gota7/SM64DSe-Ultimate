/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CommandLine;
using Serilog;
using SM64DSe.core.cli;
using SM64DSe.core.cli.options;
using SM64DSe.core.updater;

namespace SM64DSe
{
    static class Program
    {
        public static string AppTitle = "SM64DS Editor ULTIMATE";
        // The ProductVersion is extracted from AssemblyInformationalVersion
        public static string AppVersion = Application.ProductVersion;
        public static string AppDate = "Mar 17, 2025";

        public static string ServerURL = "http://kuribo64.net/";

        public static string m_ROMPath;
        public static NitroROM m_ROM;
        public static bool m_IsROMFolder;
        public static string m_ROMBasePath;
        public static string m_ROMPatchPath;
        public static string m_ROMConversionPath;
        public static string m_ROMBuildPath;

        public static List<LevelEditorForm> m_LevelEditors;
        
        [STAThread]
        static void Main(string[] args)
        {
            // Attach console if possible
            ConsoleUtils.AttachConsole();
            
            // Create a logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information($"SM64DSe-Ultimate version {AppVersion}");
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // If first argument is provided (drag and drop)
            if (args.Length == 0 || (args.Length == 1 && args[0] != "--help"))
            {
                string path = args.Length == 1 ? args[0] : null;
                Application.Run(new MainForm(path));
                return;
            }

            // If not, assume the first argument is the command
            CLIService.Run(args);
        }
    }
}
