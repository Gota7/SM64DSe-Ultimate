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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class LevelSettingsForm : Form
    {
        private Level m_Level;
        private LevelSettings m_LevelSettings;

        public LevelSettingsForm(Level level)
        {
            InitializeComponent();
            m_Level = level;
            m_LevelSettings = level.m_LevelSettings;
        }

        private void LevelSettingsForm_Load(object sender, EventArgs e)
        {
            string[][] bankobjs = new string[8][];
            bankobjs[0] = new string[7];
            bankobjs[1] = new string[7];
            bankobjs[2] = new string[7];
            bankobjs[3] = new string[7];
            bankobjs[4] = new string[7];
            bankobjs[5] = new string[7];
            bankobjs[6] = new string[7];
            bankobjs[7] = new string[53];
            foreach (string[] foo in bankobjs)
                for (int i = 0; i < foo.Length; i++)
                    foo[i] = "";

            foreach (ObjectDatabase.ObjectInfo obj in ObjectDatabase.m_ObjectInfo)
            {
                if (obj.m_BankRequirement != 1)
                    continue;

                bankobjs[obj.m_NumBank][obj.m_BankSetting] += obj.m_ID.ToString() + ",";
            }

            ComboBox[] combos = { cbxBank0, cbxBank1, cbxBank2, cbxBank3, cbxBank4, cbxBank5, cbxBank6, cbxBank7 };
            for (int b = 0; b < 8; b++)
            {
                for (int i = 0; i < bankobjs[b].Length; i++)
                {
                    string txt = string.Format("[{0}] - ", i);
                    string[] objs = bankobjs[b][i].Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
                    if (objs.Length > 0)
                    {
                        foreach (string _objid in objs)
                        {
                            int objid = int.Parse(_objid);
                            txt += string.Format("{0} ({1}), ", ObjectDatabase.m_ObjectInfo[objid].m_Name, objid);
                        }
                        txt = txt.Substring(0, txt.Length - 2);
                    }
                    else
                        txt += "(none)";
                    combos[b].Items.Add(txt);
                }

                combos[b].SelectedIndex = (int)m_LevelSettings.ObjectBanks[b];
            }

            cbxBackground.SelectedIndex = m_LevelSettings.Background;
            nudGeneralMinimumNumberOfAreas.Value = m_Level.m_NumAreas;

            txtMusicByte01.Text = m_LevelSettings.MusicBytes[0].ToString();
            txtMusicByte02.Text = m_LevelSettings.MusicBytes[1].ToString();
            txtMusicByte03.Text = m_LevelSettings.MusicBytes[2].ToString();

            chkFarCamera.Checked = m_LevelSettings.CameraStartZoomedOut == 1;

            nudBMD.Value = m_LevelSettings.BMDFileID;
            nudKCL.Value = m_LevelSettings.KCLFileID;
            nudICG.Value = m_LevelSettings.MinimapTsetFileID;
            nudICL.Value = m_LevelSettings.MinimapPalFileID;

            int levelID = ((LevelEditorForm)Owner).m_LevelID;

            if (Program.m_IsROMFolder) {
                Program.m_ROM.arm9R.BaseStream.Position = Helper.GetCourseIDTableAddress(levelID) + (levelID < 52 ? levelID : levelID - 52) - Program.m_ROM.headerSize;
                txtActSelectorID.Text = Program.m_ROM.arm9R.ReadByte().ToString();
                return;
            }
            Program.m_ROM.BeginRW();
            txtActSelectorID.Text = Program.m_ROM.Read8((uint)(Helper.GetCourseIDTableAddress(levelID) + (levelID < 52 ? levelID : levelID - 52))).ToString();
            Program.m_ROM.EndRW();
        }

        private void cbxBankX_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            ComboBox cbx = (ComboBox)sender;
            e.ItemWidth = cbx.Width;

            string txt = (string)cbx.Items[e.Index];
            int width = e.ItemWidth - (int)e.Graphics.MeasureString(txt.Substring(0, 6), cbx.Font).Width;
            e.ItemHeight = (int)e.Graphics.MeasureString(txt.Substring(6), cbx.Font, width).Height;
        }

        private void cbxBankX_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cbx = (ComboBox)sender;

            Color txtcolor;
            if ((e.State & DrawItemState.ComboBoxEdit) != 0)
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.ControlLightLight), e.Bounds);
                txtcolor = SystemColors.ControlText;
            }
            else
            {
                e.DrawBackground();

                if ((e.State & (DrawItemState.Focus | DrawItemState.Selected)) != 0)
                    txtcolor = SystemColors.HighlightText;
                else
                    txtcolor = SystemColors.ControlText;
            }

            Brush txtbrush = new SolidBrush(txtcolor);

            string txt = (string)cbx.Items[e.Index];

            int margin = (int)e.Graphics.MeasureString(txt.Substring(0, 6), cbx.Font).Width;
            RectangleF rect = e.Bounds;
            rect.X += margin;
            rect.Width -= margin;

            e.Graphics.DrawString(txt.Substring(0, 6), cbx.Font, txtbrush, e.Bounds.Location);
            e.Graphics.DrawString(txt.Substring(6), cbx.Font, txtbrush, rect);

            e.DrawFocusRectangle();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_LevelSettings.Background = (byte)cbxBackground.SelectedIndex;
            m_Level.m_NumAreas = (byte)(nudGeneralMinimumNumberOfAreas.Value);
            m_LevelSettings.ObjectBanks[0] = (uint)cbxBank0.SelectedIndex;
            m_LevelSettings.ObjectBanks[1] = (uint)cbxBank1.SelectedIndex;
            m_LevelSettings.ObjectBanks[2] = (uint)cbxBank2.SelectedIndex;
            m_LevelSettings.ObjectBanks[3] = (uint)cbxBank3.SelectedIndex;
            m_LevelSettings.ObjectBanks[4] = (uint)cbxBank4.SelectedIndex;
            m_LevelSettings.ObjectBanks[5] = (uint)cbxBank5.SelectedIndex;
            m_LevelSettings.ObjectBanks[6] = (uint)cbxBank6.SelectedIndex;
            m_LevelSettings.ObjectBanks[7] = (uint)cbxBank7.SelectedIndex;

            try
            {
                m_LevelSettings.MusicBytes[0] = byte.Parse(txtMusicByte01.Text);
                m_LevelSettings.MusicBytes[1] = byte.Parse(txtMusicByte02.Text);
                m_LevelSettings.MusicBytes[2] = byte.Parse(txtMusicByte03.Text);
            }
            catch { }

            m_LevelSettings.CameraStartZoomedOut = chkFarCamera.Checked ? (byte)1 : (byte)0;

            m_LevelSettings.BMDFileID = (ushort)nudBMD.Value;
            m_LevelSettings.KCLFileID = (ushort)nudKCL.Value;
            m_LevelSettings.MinimapTsetFileID = (ushort)nudICG.Value;
            m_LevelSettings.MinimapPalFileID = (ushort)nudICL.Value;

            int levelID = ((LevelEditorForm)Owner).m_LevelID;

            if (Program.m_IsROMFolder) {
                Program.m_ROM.arm9W.BaseStream.Position = Helper.GetCourseIDTableAddress(levelID) + (levelID < 52 ? levelID : levelID - 52) - Program.m_ROM.headerSize;
                Program.m_ROM.arm9W.Write(byte.Parse(txtActSelectorID.Text));
                Program.m_ROM.SaveArm9();
                return;
            }

            try
            {
                Program.m_ROM.BeginRW();
                Program.m_ROM.Write8((uint)(Helper.GetCourseIDTableAddress(levelID) + (levelID < 52 ? levelID : levelID - 52)), byte.Parse(txtActSelectorID.Text));
                Program.m_ROM.EndRW();
            }
            catch { }

            string parent = Directory.GetParent(Program.m_ROM.m_Path).FullName;
            string bakFolderPath = Path.Combine(parent, "bak");
            string bakFilePath = Path.Combine(bakFolderPath, "main.bin");

            if (Directory.Exists(bakFolderPath) && File.Exists(bakFilePath))
            {
                string msg = "NSMBe 'bak' folder and 'bak/main.bin' detected.\nWould you like to apply your changes to NSMBe's backup file? (recommended)\nNot doing this will require manually hex editing the backup file with the correct course id each time you insert code with NSMBe.";
                if (MessageBox.Show(msg, "NSMBe 'bak' folder detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    byte[] bak = File.ReadAllBytes(bakFilePath);
                    Helper.Write8(bak, Helper.GetCourseIDTableAddress(levelID) - 0x4000 + (uint)(levelID < 52 ? levelID : levelID - 52), byte.Parse(txtActSelectorID.Text));
                    File.WriteAllBytes(bakFilePath, bak);
                }
            }
        }

        private void cbxBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((LevelEditorForm)Owner).UpdateSkybox(cbxBackground.SelectedIndex);
        }

        private void LevelSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((LevelEditorForm)Owner).UpdateSkybox(-1);
        }

        private void groupBox3_Enter(object sender, EventArgs e) {

        }
    }
}
