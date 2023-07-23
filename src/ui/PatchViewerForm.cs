using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class PatchViewerForm : Form
    {
        class CommandInfo
        {
            public enum State
            {
                WAITING,
                SUCCESS,
                FAILED,
                WARNING,
            }

            public CommandInfo(string command)
            {
                this.command = command;
            }

            public override string ToString()
            {
                return "[" + GetStateChar() + "] " + command;
            }

            public SolidBrush GetTextColor()
            {
                switch (state)
                {
                    case State.SUCCESS:
                        return new SolidBrush(Color.Green);
                    case State.FAILED:
                        return new SolidBrush(Color.Red);
                    case State.WARNING:
                        return new SolidBrush(Color.Orange);
                    case State.WAITING:
                    default:
                        return new SolidBrush(Color.Black);
                }
            }

            private string GetStateChar()
            {
                switch (state)
                {
                    case State.SUCCESS:
                    case State.WARNING:
                        return "V";
                    case State.FAILED:
                        return "X";
                    case State.WAITING:
                    default:
                        return "-";
                }
            }

            public string command = "";
            public string description = "";
            public State state = State.WAITING;
        }

        private CommandInfo[] commandInfos;
        private string basePath;
        private string patchPath;
        private bool filesystemEditStarted;

        private System.Timers.Timer patchTimer = new System.Timers.Timer(1000);
        private int secondCounter;

        public PatchViewerForm(string patchPath)
        {
            InitializeComponent();
            patchTimer.Elapsed += Timer_Tick;
            LoadPatch(patchPath);
        }

        private void lstCommands_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            CommandInfo item = (CommandInfo)(sender as ListBox).Items[e.Index];

            e.DrawBackground();
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            g.DrawString(item.ToString(), e.Font, item.GetTextColor(), new PointF(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCommandInfo.Text = lstCommands.SelectedIndex < 0 ? "" : commandInfos[lstCommands.SelectedIndex].description;
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(ApplyPatch);
            thread.IsBackground = true;
            thread.Start();
        }

        private void PatchViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanCloseWindow())
            {
                System.Media.SystemSounds.Hand.Play();
                e.Cancel = true;
            }
        }

        private void btnReloadScript_Click(object sender, EventArgs e)
        {
            if (!CanCloseWindow())
			{
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            LoadPatch(patchPath);
        }

        private void btnImportScript_Click(object sender, EventArgs e)
        {
            if (!CanCloseWindow())
            {
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "SM64DSe Patch(*.sp2)|*.sp2";
            o.RestoreDirectory = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            LoadPatch(o.FileName);
        }

        private bool CanCloseWindow()
		{
            IEnumerable<CommandInfo.State> states = commandInfos.Select(c => c.state);
            return !states.Contains(CommandInfo.State.WAITING) || states.Contains(CommandInfo.State.FAILED);
        }

        private void UpdateForm(int refreshIndex)
        {
            pbProgress.Invoke(new MethodInvoker(delegate { pbProgress.Value = commandInfos.Where(c => c.state == CommandInfo.State.SUCCESS || c.state == CommandInfo.State.WARNING).Count(); }));
            lblProgress.Invoke(new MethodInvoker(delegate { lblProgress.Text = $"Progress: {pbProgress.Value * 100 / pbProgress.Maximum}%"; }));
            lstCommands.Invoke(new MethodInvoker(delegate { lstCommands.Items[refreshIndex] = lstCommands.Items[refreshIndex]; }));
            txtCommandInfo.Invoke(new MethodInvoker(delegate { txtCommandInfo.Text = lstCommands.SelectedIndex < 0 ? "" : commandInfos[lstCommands.SelectedIndex].description; }));

            IEnumerable<CommandInfo.State> states = commandInfos.Select(c => c.state);
            bool enableRetryButton = !states.Contains(CommandInfo.State.WAITING) || states.Contains(CommandInfo.State.FAILED);

            btnRetry.GetCurrentParent().Invoke(new MethodInvoker(delegate { btnRetry.Enabled = enableRetryButton; }));
            btnReloadScript.GetCurrentParent().Invoke(new MethodInvoker(delegate { btnReloadScript.Enabled = enableRetryButton; }));
            btnImportScript.GetCurrentParent().Invoke(new MethodInvoker(delegate { btnImportScript.Enabled = enableRetryButton; }));
        }

        private void LoadPatch(string patchPath)
		{
            this.patchPath = patchPath;
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

            lstCommands.Items.Clear();
            lstCommands.Items.AddRange(commandInfos);

            pbProgress.Minimum = pbProgress.Value = 0;
            pbProgress.Maximum = commandInfos.Count();
            pbProgress.Step = 1;
            lblProgress.BackColor = Color.Transparent;

            secondCounter = 0;
            lblTimeElapsed.Text = "Time elapsed: 0s";

            Thread thread = new Thread(ApplyPatch);
            thread.IsBackground = true;
            thread.Start();
        }

        private void Timer_Tick(object source, System.Timers.ElapsedEventArgs e)
		{
            secondCounter++;
            lblTimeElapsed.Invoke(new MethodInvoker(delegate { lblTimeElapsed.Text = "Time elapsed: " + secondCounter + "s"; }));
        }

        private void ApplyPatch()
        {
            patchTimer.Start();

            filesystemEditStarted = false;

            TreeView dummyTree = new TreeView();
            ROMFileSelect.LoadFileList(dummyTree);
            TreeNode dummyNode = dummyTree.Nodes[0];

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
                    UpdateForm(i);
                    ProcessCommand(p, info, dummyNode);
                    if (info.state != CommandInfo.State.WARNING)
                        info.state = CommandInfo.State.SUCCESS;
                    UpdateForm(i);
                }
                catch (Exception ex)
                {
                    info.description = ex.Message;
                    info.state = CommandInfo.State.FAILED;
                    UpdateForm(i);
                    System.Media.SystemSounds.Hand.Play();
                    patchTimer.Stop();
                    StopFilesystemEditIfNecessary();
                    break;
                }
            }

            patchTimer.Stop();
            StopFilesystemEditIfNecessary();
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
