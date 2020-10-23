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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Web;
using SM64DSe.ImportExport.LevelImportExport;
using System.Globalization;

namespace SM64DSe
{
    public partial class MainForm : Form
    {
        private void LoadROM(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("The specified file doesn't exist.", Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Program.m_ROMPath != "")
            {
                while (Program.m_LevelEditors.Count > 0)
                    Program.m_LevelEditors[0].Close();

                
                Program.m_ROM.EndRW();
            }

            Program.m_ROMPath = filename;
            try { Program.m_ROM = new NitroROM(Program.m_ROMPath); }
            catch (Exception ex)
            {
                string msg;

                if (ex is IOException)
                    msg = "The ROM couldn't be opened. Close any program that may be using it and try again.";
                else
                    msg = "The following error occured while loading the ROM:\n" + ex.Message;

                MessageBox.Show(msg, Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (Program.m_ROM != null)
                {
                    Program.m_ROM.EndRW();
                }
                Program.m_ROMPath = "";
                return;
            }

            Program.m_ROM.BeginRW();
            if (Program.m_ROM.NeedsPatch())
            {
                DialogResult res = MessageBox.Show(
                    "This ROM needs to be patched before the editor can work with it.\n\n" +
                    "Do you want to first make a backup of it in case the patching\n" +
                    "operation goes wrong somehow?",
                    Program.AppTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    sfdSaveFile.FileName = Program.m_ROMPath.Substring(0, Program.m_ROMPath.Length - 4) + "_bak.nds";
                    if (sfdSaveFile.ShowDialog(this) == DialogResult.OK)
                    {
                        Program.m_ROM.EndRW();
                        File.Copy(Program.m_ROMPath, sfdSaveFile.FileName, true);
                    }
                }
                else if (res == DialogResult.Cancel)
                {
                    Program.m_ROM.EndRW();
                    Program.m_ROMPath = "";
                    return;
                }

                // switch to buffered RW mode (faster for patching)
                Program.m_ROM.EndRW();
                Program.m_ROM.BeginRW(true);

                try { Program.m_ROM.Patch(); }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "An error occured while patching your ROM.\n" + 
                        "No changes have been made to your ROM.\n" + 
                        "Try using a different ROM. If the error persists, report it to Mega-Mario, with the details below:\n\n" + 
                        ex.Message + "\n" + 
                        ex.StackTrace,
                        Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(ex.StackTrace);
                    Program.m_ROM.EndRW(false);
                    Program.m_ROMPath = "";
                    return;
                }
            }

            Program.m_ROM.LoadTables();
            Program.m_ROM.EndRW();

            // Program.m_ShaderCache = new ShaderCache();

            btnRefresh.Enabled = true;
            cbLevelListDisplay.Enabled = true;

            if (cbLevelListDisplay.SelectedIndex == -1)
                cbLevelListDisplay.SelectedIndex = 0;
            else
                btnRefresh.PerformClick();

            this.tvFileList.Nodes.Clear();
            ROMFileSelect.LoadFileList(this.tvFileList);
            this.tvARM9Overlays.Nodes.Clear();
            ROMFileSelect.LoadOverlayList(this.tvARM9Overlays);

            btnASMHacking.Enabled = true;
            btnTools.Enabled = true;
            btnMore.Enabled = true;
        }

        private void EnableOrDisableASMHackingCompilationAndGenerationFeatures()
        {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR)
            {
                btnASMHacking.DropDownItems.Remove(mnitASMHackingCompilation);
                btnASMHacking.DropDownItems.Remove(mnitASMHackingGeneration);
                btnASMHacking.DropDownItems.Remove(tssASMHacking001);
            }
            else
            {
                if (btnASMHacking.DropDownItems.IndexOf(mnitASMHackingCompilation) < 0)
                {
                    btnASMHacking.DropDownItems.Insert(0, mnitASMHackingCompilation);
                    btnASMHacking.DropDownItems.Insert(1, mnitASMHackingGeneration);
                    btnASMHacking.DropDownItems.Insert(2, tssASMHacking001);
                }
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            Text = Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate;
            Program.m_ROMPath = "";
            Program.m_LevelEditors = new List<LevelEditorForm>();

            btnMore.DropDownItems.Add("Dump Object Info", null, btnDumpObjInfo_Click);

            slStatusLabel.Text = "Ready";
            ObjectDatabase.Initialize();

            if (args.Length >= 1) LoadROM(args[0]);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ObjectDatabase.LoadFallback();
            try { ObjectDatabase.Load(); }
            catch { }

            if (!Properties.Settings.Default.AutoUpdateODB)
                return;

            ObjectDatabase.m_WebClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(this.ODBDownloadProgressChanged);
            //ObjectDatabase.m_WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.ODBDownloadDone);
            ObjectDatabase.m_WebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.ODBDownloadDone);

            ObjectDatabase.Update(false);
        }

        private void ODBDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!spbStatusProgress.Visible)
            {
                slStatusLabel.Text = "Updating object database...";
                spbStatusProgress.Visible = true;
            }

            spbStatusProgress.Value = e.ProgressPercentage;
        }

        private void ODBDownloadDone(object sender, DownloadStringCompletedEventArgs e)
        {
            spbStatusProgress.Visible = false;

            if (e.Cancelled || (e.Error != null))
            {
                slStatusLabel.Text = "Object database update " + (e.Cancelled ? "cancelled" : "failed") + ".";
            }
            else
            {
                if (e.Result == "noupdate")
                {
                    slStatusLabel.Text = "Object database already up to date.";
                }
                else
                {
                    slStatusLabel.Text = "Object database updated.";

                    try
                    {
                        File.WriteAllText("objectdb.xml", e.Result);
                    }
                    catch
                    {
                        slStatusLabel.Text = "Object database update failed.";
                    }
                }
            }

            try { ObjectDatabase.Load(); }
            catch { }
        }

        private void btnOpenROM_Click(object sender, EventArgs e)
        {
            if (ofdOpenFile.ShowDialog(this) == DialogResult.OK)
                LoadROM(ofdOpenFile.FileName);
        }

        private void OpenLevel(int levelid)
        {
            if ((levelid < 0) || (levelid >= 52))
                return;

            foreach (LevelEditorForm lvledit in Program.m_LevelEditors)
            {
                if (lvledit.m_LevelID == levelid)
                {
                    lvledit.Focus();
                    return;
                }
            }

           // try
            {
                LevelEditorForm newedit = new LevelEditorForm(Program.m_ROM, levelid);
                newedit.Show();
                Program.m_LevelEditors.Add(newedit);
            }
            /*catch (Exception ex)
            {
                MessageBox.Show("The following error occured while opening the level:\n" + ex.Message,
                    Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }


        private void btnEditLevel_Click(object sender, EventArgs e)
        {
            OpenLevel(lbxLevels.SelectedIndex);
        }

        private void lbxLevels_DoubleClick(object sender, EventArgs e)
        {
            OpenLevel(lbxLevels.SelectedIndex);
        }

        private void lbxLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEditLevel.Enabled = (lbxLevels.SelectedIndex != -1);
            btnEditCollisionMap.Enabled = (lbxLevels.SelectedIndex != -1);
        }

        private void btnDumpObjInfo_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR)
            {
                MessageBox.Show("Only compatible with EUR ROMs.", Program.AppTitle);
                return;
            }

            DumpObjectInfo();
        }

        private void btnUpdateODB_Click(object sender, EventArgs e)
        {
            ObjectDatabase.Update(true);
        }

        private void btnEditorSettings_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog(this);
        }

        private void btnHalp_Click(object sender, EventArgs e)
        {
            string msg = Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate + "\n\n" +
                "A level editor for Super Mario 64 DS.\n" +
                "Coding and design by Mega-Mario (StapleButter), with help from others (see credits).\n" +
                "Provided to you by Kuribo64, the SM64DS hacking department.\n" +
                "\n" +
                "Credits:\n" +
                "- Treeki: the overlay decompression (Jap77), the object list and other help\n" +
                "- Dirbaio: other help\n" +
                "- blank: help with generating collision\n" + 
                "- mibts: ASM hacking template v2, BCA optimisation, level editor enhancements and other help\n" + 
                "- Fiachra Murray: current developer and maintainer\n" + 
                "\n" +
                Program.AppTitle + " is free software. If you paid for it, notify Mega-Mario about it.\n" +
                "\n" +
                "Visit Kuribo64's site (http://kuribo64.net/) for more details.";

            MessageBox.Show(msg, "About " + Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnitDumpAllOvls_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM == null)
                return;
            for (int i = 0; i < 155; i++)
            {
                NitroOverlay overlay = new NitroOverlay(Program.m_ROM, (uint)i);
                string filename = "DecompressedOverlays/overlay_" + i.ToString("0000") + ".bin";
                string dir = "DecompressedOverlays";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                System.IO.File.WriteAllBytes(filename, overlay.m_Data);
            }
            slStatusLabel.Text = "All overlays have been successfully dumped.";
        }

        private void btnEditCollisionMap_Click(object sender, EventArgs e)
        {
            uint overlayID = Program.m_ROM.GetLevelOverlayID(lbxLevels.SelectedIndex);
            NitroOverlay currentOverlay = new NitroOverlay(Program.m_ROM, overlayID);
            NitroFile currentKCL = Program.m_ROM.GetFileFromInternalID(currentOverlay.Read16((uint)(0x6A)));
            if (!Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters)
            {
                ModelAndCollisionMapEditor kclForm =
                    new ModelAndCollisionMapEditor(null, currentKCL.m_Name, 1f, ModelAndCollisionMapEditor.StartMode.CollisionMap);
                kclForm.Show();
            }
            else
            {
                KCLEditorForm kclForm = new KCLEditorForm(currentKCL);
                kclForm.Show();
            }
        }

        private void mnitDecompressOverlaysWithinGame_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM == null)
                return;
            Program.m_ROM.BeginRW();
            Helper.DecompressOverlaysWithinGame();
            Program.m_ROM.EndRW();
            slStatusLabel.Text = "All overlays have been decompressed successfully.";
        }

        private void mnitHexDumpToBinaryFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a hex dump to open.";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string hexDump = File.ReadAllText(ofd.FileName);
                    byte[] binaryData = Helper.HexDumpToBinary(hexDump);
                    System.IO.File.WriteAllBytes(ofd.FileName + ".bin", binaryData);

                    slStatusLabel.Text = "Hex dump successfully converted to binary file.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private void mnitAdditionalPatches_Click(object sender, EventArgs e)
        {
            AdditionalPatchesForm addPatchesForm = new AdditionalPatchesForm();
            addPatchesForm.Show();
        }

        private string m_SelectedFile;
        private string m_SelectedOverlay;

        private void tvFileList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.m_SelectedFile = e.Node == null || e.Node.Tag == null ? "" : e.Node.Tag.ToString();
            if (!(this.m_SelectedFile != ""))
                return;
            string str1;
            if (!this.m_SelectedFile.StartsWith("ARCHIVE") && Program.m_ROM.GetFileIDFromName(this.m_SelectedFile) != ushort.MaxValue) {
                string str2;
                if (this.m_SelectedFile.Last<char>() != '/')
                    str2 = string.Format("File, ID = 0x{0:x4}, Ov0ID = 0x{1:x4}", (object)Program.m_ROM.GetFileIDFromName(this.m_SelectedFile), (object)Program.m_ROM.GetFileEntries()[(int)Program.m_ROM.GetFileIDFromName(this.m_SelectedFile)].InternalID);
                else
                    str2 = string.Format("Directory, ID = 0x{0:x4}", (object)Program.m_ROM.GetDirIDFromName(this.m_SelectedFile.TrimEnd('/')));
                str1 = str2;
            } else
                str1 = "";
            this.slStatusLabel.Text = str1;
        }

        private void btnExtractRaw_Click(object sender, EventArgs e)
        {
            if (m_SelectedFile == null || m_SelectedFile.Equals(""))
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(m_SelectedFile);
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;

            System.IO.File.WriteAllBytes(sfd.FileName, Program.m_ROM.GetFileFromName(m_SelectedFile).m_Data);
        }

        private void btnReplaceRaw_Click(object sender, EventArgs e)
        {
            if (m_SelectedFile == null || m_SelectedFile.Equals(""))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;

            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            file.Clear();
            file.WriteBlock(0, System.IO.File.ReadAllBytes(ofd.FileName));
            file.SaveChanges();
        }

        private void mnitEditSDATINFOBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SDATInfoEditor sdatInfoEditor = new SDATInfoEditor();
            sdatInfoEditor.Show();
        }

        private void btnLZDecompressWithHeader_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            // NitroFile automatically decompresses on load if LZ77 header present
            file.SaveChanges();
        }

        private void btnLZForceDecompression_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.ForceDecompression();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to force decompression of \"" + file.m_Name + "\", " +
                    "this file may not use LZ77 compression (no header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnLZCompressWithHeader_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.Compress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to compress the file \"" + file.m_Name + "\" with " +
                    "LZ77 compression (with header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnLZForceCompression_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.ForceCompression();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to compress the file \"" + file.m_Name + "\" with " +
                    "LZ77 compression (no header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnDecompressOverlay_Click(object sender, EventArgs e)
        {
            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            NitroOverlay ovl = new NitroOverlay(Program.m_ROM, ovlID);
            ovl.SaveChanges();
        }

        private void btnExtractOverlay_Click(object sender, EventArgs e)
        {
            if (m_SelectedOverlay == null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = m_SelectedOverlay;
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;

            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            System.IO.File.WriteAllBytes(sfd.FileName, new NitroOverlay(Program.m_ROM, ovlID).m_Data);
        }

        private void btnReplaceOverlay_Click(object sender, EventArgs e)
        {
            if (m_SelectedOverlay == null)
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;

            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            NitroOverlay ovl = new NitroOverlay(Program.m_ROM, ovlID);
            ovl.Clear();
            ovl.WriteBlock(0, System.IO.File.ReadAllBytes(ofd.FileName));
            ovl.SaveChanges();
        }

        private void tvARM9Overlays_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                m_SelectedOverlay = null;
            else
                m_SelectedOverlay = e.Node.Tag.ToString();
        }

        private void mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The v3 patch applied at the beginning prevents patched ROM's working with NSMBe's useful 
            // ASM patching feature - this simply toggles the application of that patch on and off.

            Program.m_ROM.BeginRW();

            bool suitable = (Program.m_ROM.Read32(0x4AF4) == 0xDEC00621 && Program.m_ROM.Read32(0x4AF8) == 0x2106C0DE) ? true : false;
            if (!suitable)
            {
                Program.m_ROM.Write32(0x4AF4, 0xDEC00621);
                Program.m_ROM.Write32(0x4AF8, 0x2106C0DE);
                uint binend = (Program.m_ROM.Read32(0x2C) + 0x4000);
                Program.m_ROM.Write32(binend, 0xDEC00621);
                Program.m_ROM.Write32(0x4AEC, 0x00000000);
            }
            else
            {
                Program.m_ROM.Write32(0x4AF4, 0x00000000);
                Program.m_ROM.Write32(0x4AF8, 0x00000000);
                uint binend = (Program.m_ROM.Read32(0x2C) + 0x4000);
                Program.m_ROM.Write32(binend, 0x00000000);
                Program.m_ROM.Write32(0x4AEC, 0x02061504);
            }

            Program.m_ROM.EndRW();

            MessageBox.Show("ROM is " + ((suitable) ? "no longer " : "now ") + "suitable for use with NSMBe's ASM patch insertion feature");
        }

        private void mnitToolsModelAndCollisionMapImporter_Click(object sender, EventArgs e)
        {
            new ModelAndCollisionMapEditor().Show();
        }

        private void mnitToolsCollisionMapEditor_Click(object sender, EventArgs e)
        {
            new ModelAndCollisionMapEditor(ModelAndCollisionMapEditor.StartMode.CollisionMap).Show();
        }

        private void mnitToolsModelAnimationEditor_Click(object sender, EventArgs e)
        {
            AnimationEditorForm animationEditorForm = new AnimationEditorForm();
            animationEditorForm.Show();
        }

        private void mnitToolsTextEditor_Click(object sender, EventArgs e)
        {
            new TextEditorForm().Show();
        }

        private void mnitToolsImageEditor_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void mnitToolsBTPEditor_Click(object sender, EventArgs e)
        {
            new TextureEditorForm().Show();
        }

        private void mnitToolsSoundBrowser_Click(object sender, EventArgs e)
        {
            new SoundViewForm().Show();
        }

        private void mnitASMHackingCompilationCodeCompiler_Click(object sender, EventArgs e)
        {
            new CodeCompilerForm().Show();
        }

        private void mnitASMHackingCompilationFixCodeOffsets_Click(object sender, EventArgs e)
        {
            new CodeFixerForm().Show();
        }

        private void platformEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Templates.PlatformTemplateForm().Show();
        }

        private void bMDKLCEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ROMFileSelect rfs = new ROMFileSelect("Select a BMD or KLC file",new string[] { ".bmd", ".klc" });
            if (rfs.ShowDialog(this) == DialogResult.OK)
            {
                new BMD_KLC_Editor(rfs.m_SelectedFile).Show();
            }


        }

        private void cbLevelListDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRefresh.PerformClick();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lbxLevels.Items.Clear();

            int i = 0;
            List<string> ids = new List<string>();
            List<string> internalNames = new List<string>();
            List<string> names = new List<string>();
            foreach (String lvlName in Strings.LevelNames) {
                ids.Add("[" + i + "]");
                internalNames.Add(Program.m_ROM.GetInternalLevelNameFromID(i));
                names.Add(lvlName);
                i++;
            }
            i = 0;
            if (cbLevelListDisplay.SelectedIndex == 0) {
                int maxIdLen = ids.Select(x => x.Length).Max();
                int maxInternalLen = internalNames.Select(x => x.Length).Max();
                foreach (String lvlName in Strings.LevelNames) {
                    string id = ids[i];
                    while (id.Length < maxIdLen + 1) {
                        id += " ";
                    }
                    string internalN = internalNames[i];
                    int numTabs = (maxInternalLen - internalN.Length) / 11;
                    while (internalN.Length < maxInternalLen + 1) {
                        internalN += " ";
                    }
                    for (int j = 0; j < numTabs; j++) {
                        internalN += "\t";
                    }
                    lbxLevels.Items.Add(id + "\t" + internalN + "\t\t" + names[i]);
                    i++;
                }
            } else if (cbLevelListDisplay.SelectedIndex == 1)
            {
                foreach (String lvlName in Strings.LevelNames)
                {
                    lbxLevels.Items.Add(lvlName);
                    i++;
                }
            } else if (cbLevelListDisplay.SelectedIndex == 2)
            {
                foreach (String lvlName in Strings.ShortLvlNames)
                {
                    lbxLevels.Items.Add(i + "\t[" + internalNames[i] + "]");
                    i++;
                }
            } else if (cbLevelListDisplay.SelectedIndex == 3)
            {
                foreach (String lvlName in Strings.ShortLvlNames)
                {
                    lbxLevels.Items.Add(lvlName + " [" + internalNames[i] + "]");
                    i++;
                }
            } else {
                int hubCounter = 1;
                foreach (String lvlName in Strings.ShortLvlNames)
                {
                    ushort selectorId = Program.m_ROM.GetActSelectorIdByLevelID(i);
                    String lvlString = "";
                    if (selectorId < 29)
                    {
                        lvlString = Program.m_ROM.GetInternalLevelNameFromID(i);
                        while (lvlString.StartsWith(" "))
                            lvlString = lvlString.Remove(0, 1);

                        if (selectorId < 16)
                        {
                            if (lvlString.StartsWith((selectorId + 1).ToString()))
                                lvlString = lvlString.Remove(0, selectorId.ToString().Length + 1);
                        }
                        while (lvlString.StartsWith(" "))
                            lvlString = lvlString.Remove(0, 1);


                        String optimizedLvlString = "";
                        char lastChar = ' ';
                        foreach(char c in lvlString)
                        {
                            string letter = c.ToString();
                            if (lastChar == ' ')
                                optimizedLvlString = optimizedLvlString + letter.ToUpper();
                            else
                                optimizedLvlString = optimizedLvlString + letter.ToLower();
                            lastChar = c;
                        }
                        lvlString = optimizedLvlString;
                    }
                    else if (selectorId == 29)
                    {
                        lvlString = "Part " + hubCounter + " of the Hubworld";
                        hubCounter++;
                    }
                    else if (selectorId == 255)
                    {
                        lvlString = "TestMap";
                    }
                    else
                    {
                        lvlString = "Cant find a Levelname for ActSelectorID " + i;
                    }
                    lbxLevels.Items.Add(lvlString);
                    i++;
                }
            }
        }

        private void dLPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Program.m_ROM.m_Version != NitroROM.Version.EUR)
            {

                MessageBox.Show("This is for EUR roms only!");

            }
            else
            {

                bool autorw = Program.m_ROM.CanRW();
                if (!autorw) Program.m_ROM.BeginRW();
                if (Program.m_ROM.Read32(0x6590) != 0) //the patch makes this not 0
                {
                    if (!autorw) Program.m_ROM.EndRW();
                    MessageBox.Show("Rom has already been patched!");
                    return;
                }
                if (!autorw) Program.m_ROM.EndRW();

                if (MessageBox.Show("This will patch the ROM. " +
                    "Continue with the patch?", "Table Shifting Patch", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

                if (!autorw) Program.m_ROM.BeginRW();
                NitroOverlay ov2 = new NitroOverlay(Program.m_ROM, 2);

                //Move the ACTOR_SPAWN_TABLE so it can expand
                Program.m_ROM.WriteBlock(0x6590, Program.m_ROM.ReadBlock(0x90864, 0x61c));
                Program.m_ROM.WriteBlock(0x90864, new byte[0x61c]);

                //Adjust pointers
                Program.m_ROM.Write32(0x1a198, 0x02006590);

                //Move the OBJ_TO_ACTOR_TABLE so it can expand
                Program.m_ROM.WriteBlock(0x4b00, ov2.ReadBlock(0x0210cbf4 - ov2.GetRAMAddr(), 0x28c));
                ov2.WriteBlock(0x0210cbf4 - ov2.GetRAMAddr(), new byte[0x28c]);

                //Adjust pointers
                ov2.Write32(0x020fe890 - ov2.GetRAMAddr(), 0x02004b00);
                ov2.Write32(0x020fe958 - ov2.GetRAMAddr(), 0x02004b00);
                ov2.Write32(0x020fea44 - ov2.GetRAMAddr(), 0x02004b00);

                //Add the dynamic library loading and cleanup code
                Program.m_ROM.WriteBlock(0x90864, Properties.Resources.dynamic_library_loader);

                //Add the hooks (by replacing LoadObjBankOverlays())
                Program.m_ROM.WriteBlock(0x2df70, Properties.Resources.static_overlay_loader);

                if (!autorw) Program.m_ROM.EndRW();
                ov2.SaveChanges();

            }

        }

        private void kuppaScriptEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KuppaScriptEditor kse = new KuppaScriptEditor();
            kse.Show();
        }

        private void editFileSystemToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                MessageBox.Show("This is for EUR ROMs only!");
                return;
            }
            if (new FilesystemEditorForm(this).ShowDialog() != DialogResult.OK)
                return;
            this.LoadROM(Program.m_ROMPath);
        }

        private void editOverlaysToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                MessageBox.Show("This is for EUR ROMs only!");
                return;
            }
            new OverlayEditor().ShowDialog();
        }

        private void importPatchToolStripMenuItem_Click(object sender, EventArgs e) {

            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "SM64DSe Patch|*.sp";
            o.RestoreDirectory = true;
            if (o.ShowDialog() == DialogResult.OK) {

                //Each line.
                var s = File.ReadAllLines(o.FileName);
                string basePath = Path.GetDirectoryName(o.FileName) + "/";
                foreach (var l in s) {

                    //Get parameters.
                    string t = l;
                    if (t.Contains("#")) { t = t.Substring(0, t.IndexOf('#')); }
                    var p = t.Split(' ');
                    if (p.Length == 0) { continue; }

                    //Switch command.
                    switch (p[0].ToLower()) {

                        //Replace file.
                        case "replace":
                            if (ushort.TryParse(p[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out _)) {
                                Program.m_ROM.ReinsertFile(Program.m_ROM.GetFileIDFromInternalID(ushort.Parse(p[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture)), File.ReadAllBytes(basePath + p[2]));
                            } else {
                                Program.m_ROM.ReinsertFile(Program.m_ROM.GetFileIDFromName(p[1]), File.ReadAllBytes(basePath + p[2]));
                            }
                            break;

                        //Replace ARM9.
                        case "replace_arm9":
                            var r = Program.m_ROM;
                            r.BeginRW(true);
                            uint arm9addr = r.Read32(0x20);
                            uint arm9size = r.Read32(0x2C);
                            byte[] newArm9 = File.ReadAllBytes(basePath + p[1]);
                            if (newArm9.Length > arm9size) {
                                r.MakeRoom(arm9addr + arm9size, (uint)(newArm9.Length - arm9size));
                                r.AutoFix(0xFFFF, arm9addr + arm9size, (int)(newArm9.Length - arm9size));
                            }
                            r.Write32(0x2C, (uint)newArm9.Length);
                            r.WriteBlock(arm9addr, newArm9);
                            r.EndRW(true);
                            r.LoadROM(r.m_Path);
                            break;

                        //Replace overlay.
                        case "replace_overlay":
                            NitroOverlay n2 = new NitroOverlay(Program.m_ROM, uint.Parse(p[1]));
                            n2.m_Data = File.ReadAllBytes(basePath + p[2]);
                            n2.SaveChanges();
                            break;

                        //Rename file.
                        case "rename":
                            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                                MessageBox.Show("This is for EUR ROMs only!");
                                continue;
                            }
                            Program.m_ROM.StartFilesystemEdit();
                            ushort fileIdFromName;
                            if (ushort.TryParse(p[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out _)) {
                                fileIdFromName = Program.m_ROM.GetFileIDFromInternalID(ushort.Parse(p[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture));
                            } else {
                                fileIdFromName = Program.m_ROM.GetFileIDFromName(p[1]);
                            }
                            string filename = Program.m_ROM.m_FileEntries[fileIdFromName].FullName;
                            string newName = p[2];
                            int length = filename.LastIndexOf('/') + 1;
                            string str1 = filename.Substring(0, length) + newName;
                            Program.m_ROM.m_FileEntries[fileIdFromName].Name = newName;
                            Program.m_ROM.m_FileEntries[fileIdFromName].FullName = str1;
                            Program.m_ROM.SaveFilesystem();
                            this.tvFileList.Nodes.Clear();
                            ROMFileSelect.LoadFileList(this.tvFileList);
                            this.tvARM9Overlays.Nodes.Clear();
                            ROMFileSelect.LoadOverlayList(this.tvARM9Overlays);
                            break;

                        //Import level XML.
                        case "import_xml":
                            Level lv = new Level(int.Parse(p[1]));
                            try { LevelDataXML_Importer.ImportLevel(lv, basePath + p[2], true); }
                            catch (InvalidDataException ex) { MessageBox.Show(ex.Message); }
                            catch (Exception ex) { new ExceptionMessageBox("Error parsing level, changes have not been saved", ex).ShowDialog();}
                            Program.m_ROM.LoadROM(Program.m_ROM.m_Path);
                            break;

                        //Add overlay.
                        case "add_overlay":
                            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                                MessageBox.Show("This is for EUR ROMs only!");
                                continue;
                            }
                            OverlayEditor oe = new OverlayEditor();
                            oe.AddOverlay(oe.Overlays.Count);
                            var ov = oe.Overlays[oe.Overlays.Count - 1];
                            var overlayFile = File.ReadAllBytes(basePath + p[6]);
                            ov.ID = (uint)(oe.Overlays.Count - 1);
                            ov.RAMAddress = uint.Parse(p[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov.RAMSize = (uint)overlayFile.Length;
                            ov.BSSSize = uint.Parse(p[4], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov.StaticInitStart = uint.Parse(p[2], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov.StaticInitEnd = uint.Parse(p[3], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov.Flags = uint.Parse(p[5], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            oe.Overlays[oe.Overlays.Count - 1] = ov;
                            oe.saveChangesButton_Click(null, new EventArgs());
                            oe.closeButton_Click(null, new EventArgs());
                            NitroOverlay n = new NitroOverlay(Program.m_ROM, (uint)(oe.Overlays.Count - 1));
                            n.m_Data = overlayFile;
                            n.SaveChanges();
                            this.tvFileList.Nodes.Clear();
                            ROMFileSelect.LoadFileList(this.tvFileList);
                            this.tvARM9Overlays.Nodes.Clear();
                            ROMFileSelect.LoadOverlayList(this.tvARM9Overlays);
                            break;

                        //Edit overlay.
                        case "edit_overlay":
                            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                                MessageBox.Show("This is for EUR ROMs only!");
                                continue;
                            }
                            Program.m_ROM.StartFilesystemEdit();
                            var ov2 = Program.m_ROM.m_OverlayEntries[int.Parse(p[1])];
                            ov2.RAMAddress = uint.Parse(p[2], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov2.BSSSize = uint.Parse(p[5], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov2.StaticInitStart = uint.Parse(p[3], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov2.StaticInitEnd = uint.Parse(p[4], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            ov2.Flags = uint.Parse(p[6], NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                            Program.m_ROM.m_OverlayEntries[int.Parse(p[1])] = ov2;
                            Program.m_ROM.SaveFilesystem();
                            break;

                        //Delete overlay.
                        case "delete_overlay":
                            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                                MessageBox.Show("This is for EUR ROMs only!");
                                continue;
                            }
                            OverlayEditor oe2 = new OverlayEditor();
                            oe2.DeleteOverlay(int.Parse(p[1]));
                            oe2.saveChangesButton_Click(null, new EventArgs());
                            oe2.closeButton_Click(null, new EventArgs());
                            break;

                    }
                
                }

            }

        }

    }

}
