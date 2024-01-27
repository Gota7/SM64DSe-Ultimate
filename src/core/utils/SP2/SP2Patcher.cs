using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SixLabors.ImageSharp; // preferably should be removed

namespace SM64DSe.core.utils.SP2
{
    public class SP2Patcher
    {
        private CommandInfo[] commandInfos;
        private string basePath;
        private string patchPath;
        private bool filesystemEditStarted;
        private System.Timers.Timer patchTimer = new System.Timers.Timer(1000);

        public SP2Patcher(string patchPath)
        {
            this.patchPath = patchPath;
        }

        public CommandInfo[] LoadPatches()
        {
            basePath = Path.GetDirectoryName(patchPath) + "\\";

            List<string> infosList = File.ReadAllLines(patchPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            for (int i = infosList.Count - 1; i >= 0; i--)
            {
                if (infosList[i].Contains("#"))
                    infosList[i] = infosList[i].Substring(0, infosList[i].IndexOf('#'));

                if (string.IsNullOrWhiteSpace(infosList[i]))
                    infosList.Remove(infosList[i]);
            }

            commandInfos = infosList.Select(l => new CommandInfo(l)).ToArray();
            return commandInfos;
        }
        
        private void CheckLength(string[] s, int minLength)
        {
            if (s.Length < minLength)
                throw new IndexOutOfRangeException("Not enough arguments supplied.");
        }

        private void RemoveFirstAndLastChars(ref string s)
        {
            s = s.Remove(0, 1).Remove(s.Length - 2, 1);
        }

        private void StopFilesystemEditIfNecessary()
        {
            if (filesystemEditStarted)
                Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;
        }

        private void StartFilesystemEditIfNecessary()
        {
            if (!filesystemEditStarted)
                Program.m_ROM.StartFilesystemEdit(); filesystemEditStarted = true;
        }
        
        public void ApplyPatch(Action<CommandInfo, int> onCommandInfoUpdate)
        {
            patchTimer.Start();

            filesystemEditStarted = false;

            TreeView fileTree = new TreeView();
            ROMFileSelect.LoadFileList(fileTree);
            TreeNode rootRode = fileTree.Nodes[0];

            //Each line.
            for (int i = 0; i < commandInfos.Length; i++)
            {
                CommandInfo info = commandInfos[i];

                if (info.state == CommandInfo.State.SUCCESS || info.state == CommandInfo.State.WARNING)
                    continue;

                if (info.state == CommandInfo.State.FAILED)
                    info.state = CommandInfo.State.WAITING;

                //Get parameters.
                string t = info.command;
                string[] p = t.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (p.Length == 0)
                    continue;

                try
                {
                    onCommandInfoUpdate(info, i);
                    ProcessCommand(p, info, rootRode);
                    if (info.state != CommandInfo.State.WARNING)
                        info.state = CommandInfo.State.SUCCESS;
                    onCommandInfoUpdate(info, i);
                }
                catch (Exception ex)
                {
                    info.description = ex.Message;
                    info.state = CommandInfo.State.FAILED;
                    onCommandInfoUpdate(info, i);
                    System.Media.SystemSounds.Hand.Play();
                    patchTimer.Stop();
                    StopFilesystemEditIfNecessary();
                    break;
                }
            }

            patchTimer.Stop();
            StopFilesystemEditIfNecessary();
        }
        
        private void ProcessCommand(string[] p, CommandInfo info, TreeNode dummyNode)
        {
            //Switch command.
            switch (p[0].ToLower())
            {
                case "generate_file_list":
                    CheckLength(p, 2);

                    StopFilesystemEditIfNecessary();

                    RemoveFirstAndLastChars(ref p[1]);

                    string fileListHeaderLoc = basePath + p[1];
                    FileHeaderGenerator.Generate(fileListHeaderLoc);

                    info.description = "File list generated at:\n" + fileListHeaderLoc;

                    break;

                case "generate_sound_list":
                    CheckLength(p, 2);

                    StopFilesystemEditIfNecessary();

                    RemoveFirstAndLastChars(ref p[1]);

                    string soundListHeaderLoc = basePath + p[1];
                    SoundHeaderGenerator.Generate(soundListHeaderLoc);

                    info.description = "Sound list generated at:\n" + soundListHeaderLoc;

                    break;

                /*case "generate_actor_list":
                    CheckLength(p, 2);

                    RemoveFirstAndLastChars(ref p[1]);
                    string actorListHeaderLoc = basePath + p[1];
                    File.WriteAllLines(actorListHeaderLoc, ObjectDatabase.ToCPP());

                    info.description = "Actor list generated at:\n" + actorListHeaderLoc;

                    break; */

                case "compile":
                    CheckLength(p, 3);

                    if (p[1] == "clean")
                    {
                        RemoveFirstAndLastChars(ref p[2]);
                        Patcher.PatchCompiler.cleanPatch(new DirectoryInfo(basePath + p[2]));
                        info.description = "Cleaned code directory " + p[2];
                        return;
                    }

                    CheckLength(p, 5);
                    RemoveFirstAndLastChars(ref p[3]);
                    RemoveFirstAndLastChars(ref p[4]);

                    StopFilesystemEditIfNecessary();

                    DirectoryInfo dir = new DirectoryInfo(basePath + p[3]);
                    Patcher.PatchMaker pm = new Patcher.PatchMaker(dir, 0x02400000);

                    string sourceDirs = string.Join(" ", p).Split('\"').Where(s => !string.IsNullOrWhiteSpace(s)).Last();

                    if (p[1] == "dl")
                    {
                        RemoveFirstAndLastChars(ref p[2]);

                        pm.modifyMakefileSources(sourceDirs);
                        byte[] data = pm.makeDynamicLibrary();

                        NitroFile dlFile = Program.m_ROM.GetFileFromName(p[2]);
                        dlFile.m_Data = data;
                        dlFile.SaveChanges();

                        Patcher.PatchCompiler.cleanPatch(dir);

                        info.description = "Compiled DL " + p[2] + " with code from " + p[4] + " (" + p[3] + ")";
                    }
                    else if (p[1] == "overlay")
                    {
                        uint ovID = Convert.ToUInt32(p[2]);
                        uint addr = new NitroOverlay(Program.m_ROM, ovID).GetRAMAddr();

                        pm.modifyMakefileSources(sourceDirs);
                        pm.compilePatch();
                        pm.makeOverlay(ovID);

                        info.description = "Compiled overlay " + p[2] + " with code from " + p[4] + " (" + p[3] + ")";
                    }

                    break;

                case "add_or_replace":
                    CheckLength(p, 3);

                    try
                    {
                        RemoveFirstAndLastChars(ref p[1]);
                        RemoveFirstAndLastChars(ref p[2]);

                        byte[] data;

                        bool emptyFile = p[3] == "empty_file";

                        if (emptyFile)
						{
                            data = new byte[] { 0xde, 0xad, 0xbe, 0xef };
                        }
                        else
						{
                            p[3] = p[3].Remove(0, 1).Remove(p[3].Length - 2, 1);
                            data = File.ReadAllBytes(basePath + p[3]);
                        }
                        
                        if (Program.m_ROM.FileExists(p[1] + p[2]))
                        {
                            StopFilesystemEditIfNecessary();

                            Program.m_ROM.ReinsertFile(Program.m_ROM.GetFileIDFromName(p[1] + p[2]), data);

                            info.description = "Replaced file\n" + p[1] + p[2] + "\nwith data of\n" + (emptyFile ? p[3] : basePath + p[3]);
                        }
                        else
                        {
                            StartFilesystemEditIfNecessary();

                            Program.m_ROM.AddFile(p[1], p[2], data, dummyNode);

                            info.description = "Added file\n" + p[1] + p[2] + "\nwith data of \n" + (emptyFile ? p[3] : basePath + p[3]);
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be added / replaced:\n" + ex.Message;
                        info.state = CommandInfo.State.WARNING;
                        break;
                    }

                case "replace":
                    CheckLength(p, 4);

                    try
                    {
                        RemoveFirstAndLastChars(ref p[1]);
                        RemoveFirstAndLastChars(ref p[2]);
                        RemoveFirstAndLastChars(ref p[3]);

                        byte[] data = File.ReadAllBytes(basePath + p[3]);

                        if (!Program.m_ROM.FileExists(p[1] + p[2]))
                            throw new Exception("File not found:\n" + p[1] + p[2]);

                        StopFilesystemEditIfNecessary();

                        Program.m_ROM.ReinsertFile(Program.m_ROM.GetFileIDFromName(p[1] + p[2]), data);

                        info.description = "Replaced file\n" + p[1] + p[2] + "\nwith data of\n" + basePath + p[3];

                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be replaced:\n" + ex.Message;
                        info.state = CommandInfo.State.WARNING;
                        break;
                    }
                    
                case "rename":
                    CheckLength(p, 3);

                    try
                    {
                        StartFilesystemEditIfNecessary();

                        RemoveFirstAndLastChars(ref p[1]);
                        RemoveFirstAndLastChars(ref p[2]);

                        if (!Program.m_ROM.FileExists(p[1]))
                            throw new Exception("File not found:\n" + p[1]);

                        Program.m_ROM.RenameFile(p[1], p[2], dummyNode);

                        info.description = "Renamed file\n" + p[1] + "\nto\n" + p[2];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be renamed:\n" + ex.Message;
                        info.state = CommandInfo.State.WARNING;
                        break;
                    }
                    
                case "remove":
                    CheckLength(p, 2);

                    try
                    {
                        StartFilesystemEditIfNecessary();

                        RemoveFirstAndLastChars(ref p[1]);

                        Program.m_ROM.RemoveFile(p[1], dummyNode);

                        info.description = "Removed file\n" + p[1];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be removed:\n" + ex.Message;
                        info.state = CommandInfo.State.WARNING;
                        break;
                    }

                case "add_dir":
                    CheckLength(p, 3);

                    try
                    {
                        StartFilesystemEditIfNecessary();

                        RemoveFirstAndLastChars(ref p[1]);
                        RemoveFirstAndLastChars(ref p[2]);

                        if (Program.m_ROM.GetDirIDFromName(p[1] + p[2]) != 0)
                            throw new Exception("Directory\n" + p[1] + p[2] + "\nalready exists.");

                        Program.m_ROM.AddDir(p[1], p[2], dummyNode);

                        info.description = "Added directory\n" + p[2] + "\nto\n" + p[1];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "Directory couldn't be added:\n" + ex.Message;
                        info.state = CommandInfo.State.WARNING;
                        break;
                    }

                case "rename_dir":
                    CheckLength(p, 3);

                    try
                    {
                        StartFilesystemEditIfNecessary();

                        RemoveFirstAndLastChars(ref p[1]);
                        RemoveFirstAndLastChars(ref p[2]);

                        Program.m_ROM.RenameDir(p[1], p[2], dummyNode);

                        info.description = "Renamed directory\n" + p[1] + "\nto\n" + p[2];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "Directory couldn't be renamed:\n" + ex.Message;
                        info.state = CommandInfo.State.WARNING;
                        break;
                    }

                case "remove_dir":
                    CheckLength(p, 2);

                    try
                    {
                        StartFilesystemEditIfNecessary();

                        RemoveFirstAndLastChars(ref p[1]);

                        Program.m_ROM.RemoveDir(p[1], dummyNode);

                        info.description = "Removed directory\n" + p[1];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "Directory couldn't be removed:\n" + ex.Message;
                        info.state = CommandInfo.State.WARNING;
                        break;
                    }
            }
        }
    }
}