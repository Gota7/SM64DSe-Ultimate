using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SM64DSe.SM64DSFormats;
using SM64DSe.ImportExport.Writers.InternalWriters;
using SM64DSe.ImportExport;
using SM64DSe.ImportExport.Loaders.InternalLoaders;
using System.Xml;
using System.IO;

namespace SM64DSe
{
    public partial class LevelNameEditorForm : Form
    {
        const int OVERLAY_ID_TABLE_OFFSET_1 = 0x758c8;
        const int OVERLAY_ID_TABLE_OFFSET_2 = 0x8f808;
        const int COURSE_ID_TABLE_OFFSET_1 = 0x75298;
        const int COURSE_ID_TABLE_OFFSET_2 = 0x8f73c;
        const int COURSE_PART_TABLE_OFFSET_1 = 0x75264;
        const int COURSE_PART_TABLE_OFFSET_2 = 0x8f670;

        private List<string> m_LevelNames;
        private List<string> m_ShortNames;
        private List<uint> m_OverlayIDs;
        private List<byte> m_CourseIDs;
        private List<byte> m_CourseParts;
        private int m_BiggestInternalLevelNameSize = 0;
        private bool m_UpdatingTextBoxes = false;
        private bool m_UpdatingListBox = false;

        public LevelNameEditorForm(string fileName)
        {
            InitializeComponent();

            m_LevelNames = Strings.LevelNames();
            m_ShortNames = Strings.ShortLvlNames();
            m_OverlayIDs = new List<uint>();
            m_CourseIDs = new List<byte>();
            m_CourseParts = new List<byte>();
            CalculateBiggestInternalLevelNameSize(false); // false because the labels haven't been added yet

            Program.m_ROM.BeginRW();
            for (uint i = 0; i < m_LevelNames.Count; i++)
            {
                if (i < 52)
                {
                    m_OverlayIDs.Add(Program.m_ROM.Read32(OVERLAY_ID_TABLE_OFFSET_1 + (i * 4)));
                    m_CourseIDs.Add(Program.m_ROM.Read8(COURSE_ID_TABLE_OFFSET_1 + i));
                    m_CourseParts.Add(Program.m_ROM.Read8(COURSE_PART_TABLE_OFFSET_1 + i));
                }
                else
                {
                    m_OverlayIDs.Add(Program.m_ROM.Read32(OVERLAY_ID_TABLE_OFFSET_2 + ((i - 52) * 4)));
                    m_CourseIDs.Add(Program.m_ROM.Read8(COURSE_ID_TABLE_OFFSET_2 + (i - 52)));
                    m_CourseParts.Add(Program.m_ROM.Read8(COURSE_PART_TABLE_OFFSET_2 + (i - 52)));
                }

                lstLevels.Items.Add(GetLevelLabel((int)i));
            }
            Program.m_ROM.EndRW();

            if (lstLevels.Items.Count >= 256)
                btnAddLevelSlot.Enabled = false;
        }

        public LevelNameEditorForm()
            : this(null) { }

        private void LevelNameEditorForm_Load(object sender, System.EventArgs e)
        {

        }

        private void SaveXML()
		{
            if (m_LevelNames.Count != m_ShortNames.Count)
                throw new ArgumentException("The amount of full level names is not equal to the amount of short level names.");

            Program.InitLocalFolder();

            // save the XML
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(Program.GetLocalLevelsXmlPath(), settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment("SM64DS Editor Infinity");
                writer.WriteStartElement("Levels");

                for (int i = 0; i < m_LevelNames.Count; i++)
                {
                    writer.WriteStartElement("Level");

                    writer.WriteElementString("Name", m_LevelNames[i]);
                    writer.WriteElementString("ShortName", m_ShortNames[i]);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void SaveLevelTables(bool overlayAdded = false)
		{
            string parent = Directory.GetParent(Program.m_ROM.m_Path).FullName;
            string bakFolderPath = Path.Combine(parent, "bak");
            string bakFilePath = Path.Combine(bakFolderPath, "main.bin");

            bool applyToBak = false;

            if (Directory.Exists(bakFolderPath) && File.Exists(bakFilePath))
            {
                applyToBak = true;

                if (overlayAdded)
                {
                    string msg = "NSMBe 'bak' folder and 'bak/main.bin' detected.\nChanges will be applied to NSMBe's backup file to prevent the overlay id table from being overwritten after inserting code with NSMBe.";
                    MessageBox.Show(msg, "NSMBe 'bak' folder detected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string msg = "NSMBe 'bak' folder and 'bak/main.bin' detected.\nWould you like to apply your changes to NSMBe's backup file? (recommended)\nNot doing this will require manually hex editing the backup file with the correct course ids, course parts and overlay ids each time you insert code with NSMBe.";
                    if (MessageBox.Show(msg, "NSMBe 'bak' folder detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        applyToBak = false;
                    }
                }
            }

            Program.m_ROM.BeginRW();
            byte[] bak = applyToBak ? File.ReadAllBytes(bakFilePath) : null;

            for (uint i = 0; i < m_LevelNames.Count; i++)
			{
                if (i < 52)
                {
                    Program.m_ROM.Write32(OVERLAY_ID_TABLE_OFFSET_1 + (i * 4), m_OverlayIDs[(int)i]);
                    Program.m_ROM.Write8(COURSE_ID_TABLE_OFFSET_1 + i, m_CourseIDs[(int)i]);
                    Program.m_ROM.Write8(COURSE_PART_TABLE_OFFSET_1 + i, m_CourseParts[(int)i]);

                    if (applyToBak)
					{
                        Helper.Write32(bak, OVERLAY_ID_TABLE_OFFSET_1 - 0x4000 + (i * 4), m_OverlayIDs[(int)i]);
                        Helper.Write8(bak, COURSE_ID_TABLE_OFFSET_1 - 0x4000 + i, m_CourseIDs[(int)i]);
                        Helper.Write8(bak, COURSE_PART_TABLE_OFFSET_1 - 0x4000 + i, m_CourseParts[(int)i]);
					}
                }
                else
                {
                    Program.m_ROM.Write32(OVERLAY_ID_TABLE_OFFSET_2 + ((i - 52) * 4), m_OverlayIDs[(int)i]);
                    Program.m_ROM.Write8(COURSE_ID_TABLE_OFFSET_2 + (i - 52), m_CourseIDs[(int)i]);
                    Program.m_ROM.Write8(COURSE_PART_TABLE_OFFSET_2 + (i - 52), m_CourseParts[(int)i]);

                    if (applyToBak)
                    {
                        Helper.Write32(bak, OVERLAY_ID_TABLE_OFFSET_2 - 0x4000 + ((i - 52) * 4), m_OverlayIDs[(int)i]);
                        Helper.Write8(bak, COURSE_ID_TABLE_OFFSET_2 - 0x4000 + (i - 52), m_CourseIDs[(int)i]);
                        Helper.Write8(bak, COURSE_PART_TABLE_OFFSET_2 - 0x4000 + (i - 52), m_CourseParts[(int)i]);
                    }
                }
            }

            Program.m_ROM.EndRW();
            if (applyToBak)
                File.WriteAllBytes(bakFilePath, bak);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveXML();
            SaveLevelTables();
            Close();
        }

        private void btnImportXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a XML to import";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel) return;

            try
            {
                List<string> levelNames = new List<string>();
                List<string> shortNames = new List<string>();

                using (XmlReader reader = XmlReader.Create(ofd.FileName))
                {
                    reader.MoveToContent();

                    while (reader.Read())
                    {
                        if (reader.NodeType.Equals(XmlNodeType.Element))
                        {
                            if (reader.LocalName == "Name")
                            {
                                reader.MoveToContent();
                                levelNames.Add(reader.ReadElementContentAsString());
                            }
                            else if (reader.LocalName == "ShortName")
                            {
                                reader.MoveToContent();
                                shortNames.Add(reader.ReadElementContentAsString());
                            }
                        }
                    }
                }

                m_LevelNames = levelNames;
                m_ShortNames = shortNames;

                CalculateBiggestInternalLevelNameSize();
                for (int i = 0; i < m_LevelNames.Count; i++)
                    lstLevels.Items[i] = GetLevelLabel(i);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong:\n{ex}", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
            }
        }

        private void lstLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_UpdatingListBox || lstLevels.SelectedIndex < 0)
                return;

            m_UpdatingTextBoxes = true;
            txtLevelName.Text = m_LevelNames[lstLevels.SelectedIndex];
            txtShortName.Text = m_ShortNames[lstLevels.SelectedIndex];
            lblOverlayID.Text = m_OverlayIDs[lstLevels.SelectedIndex] + "";
            txtCourseID.Text = m_CourseIDs[lstLevels.SelectedIndex] + "";
            txtCoursePart.Text = m_CourseParts[lstLevels.SelectedIndex] + "";
            m_UpdatingTextBoxes = false;
        }

        private void txtLevelName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstLevels.SelectedIndex < 0)
                return;

            m_UpdatingListBox = true;
            CalculateBiggestInternalLevelNameSize();
            m_LevelNames[lstLevels.SelectedIndex] = txtLevelName.Text;
            lstLevels.Items[lstLevels.SelectedIndex] = GetLevelLabel(lstLevels.SelectedIndex);
            m_UpdatingListBox = false;
        }

        private void txtShortName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstLevels.SelectedIndex < 0)
                return;

            m_UpdatingListBox = true;
            m_ShortNames[lstLevels.SelectedIndex] = txtShortName.Text;
            lstLevels.Items[lstLevels.SelectedIndex] = GetLevelLabel(lstLevels.SelectedIndex);
            m_UpdatingListBox = false;
        }

        private void txtCourseID_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstLevels.SelectedIndex < 0)
                return;

            m_CourseIDs[lstLevels.SelectedIndex] = (byte)txtCourseID.Value;
        }

        private void txtCoursePart_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstLevels.SelectedIndex < 0)
                return;

            m_CourseParts[lstLevels.SelectedIndex] = (byte)txtCoursePart.Value;
        }

        private void CalculateBiggestInternalLevelNameSize(bool updateLabels = true)
        {
            int prevSize = m_BiggestInternalLevelNameSize;

            m_BiggestInternalLevelNameSize = m_LevelNames.Select(l => l.Length).Max() + 16;

            if (updateLabels && prevSize != m_BiggestInternalLevelNameSize)
            {
                for (int i = 0; i < m_LevelNames.Count; i++)
                    lstLevels.Items[i] = GetLevelLabel(i);
            }

            Console.WriteLine($"current: {m_BiggestInternalLevelNameSize}, prev: {prevSize}");
        }

        private string GetLevelLabel(int levelID)
        {
            string id = levelID.ToString().PadLeft(3, ' ');

            string internalN = m_LevelNames[levelID];
            int numTabs = ((m_BiggestInternalLevelNameSize + (m_BiggestInternalLevelNameSize % 8)) - (internalN.Length + (8 - internalN.Length % 8))) / 8;

            for (int j = 0; j < numTabs; j++)
                internalN += "\t";

            return id + "\t" + internalN + m_ShortNames[levelID];
        }

		private void btnAddLevelSlot_Click(object sender, EventArgs e)
		{
            if (lstLevels.Items.Count >= 256)
			{
                MessageBox.Show("No more levels can be added.", "Level limit has been reached");
                btnAddLevelSlot.Enabled = false;
                return;
			}

            string msg = "Would you like to add a new level slot?\nThe new slot will contain a copy of the currently selected level.\nThis will save the ROM and XML.\n\nNote: more levels patch required (can be found in the SM64DS Hacking Discord server).";
            if (MessageBox.Show(msg, "Add new level slot?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            
            int newLevelID = m_LevelNames.Count;

            m_LevelNames.Add("Level Slot " + newLevelID);
            m_ShortNames.Add("Level " + newLevelID);

            int levelToCopy = lstLevels.SelectedIndex == -1 ? 0 : lstLevels.SelectedIndex;
            uint newOverlayID = AddLevelOverlay(m_OverlayIDs[levelToCopy]);
            m_OverlayIDs.Add(newOverlayID);
            m_CourseIDs.Add(255);
            m_CourseParts.Add(0);

            lstLevels.SelectedIndex = lstLevels.Items.Add(GetLevelLabel(newLevelID));

            SaveXML();
            SaveLevelTables(true);
        }

        public uint AddLevelOverlay(uint overlayToCopy)
        {
            Program.m_ROM.StartFilesystemEdit();

            List<NitroROM.OverlayEntry> Overlays = Program.m_ROM.m_OverlayEntries.ToList();
            List<NitroROM.FileEntry> Files = Program.m_ROM.m_FileEntries.ToList();

            NitroROM.OverlayEntry original = Overlays.Where(o => o.ID == overlayToCopy).Single();
            byte[] originalData = new NitroOverlay(Program.m_ROM, overlayToCopy).m_Data;
            // NitroROM.FileEntry originalFile = Files.Where(f => f.ID == original.FileID).Single();

            uint newOverlayID = (uint)Overlays.Count;

            Overlays.Add(new NitroROM.OverlayEntry()
            {
                FileID = (ushort)Files.Count,
                BSSSize = original.BSSSize,
                EntryOffset = 0,
                Flags = original.Flags,
                ID = newOverlayID,
                RAMAddress = original.RAMAddress,
                RAMSize = original.RAMSize,
                StaticInitEnd = original.StaticInitEnd,
                StaticInitStart = original.StaticInitStart
            });
            Files.Add(new NitroROM.FileEntry()
            {
                FullName = "",
                Name = "",
                Data = (byte[])originalData.Clone(),
                ID = (ushort)Files.Count,
                InternalID = 0xFFFF,
                Offset = uint.MaxValue,
                ParentID = 0,
                Size = (uint)originalData.Length
            });

            for (int i = 0; i < Overlays.Count; i++)
            {
                var x = Overlays[i];
                x.EntryOffset = (uint)(Program.m_ROM.OVTOffset + i * 0x20);
                Overlays[i] = x;
            }

            Program.m_ROM.m_OverlayEntries = Overlays.ToArray();
            Program.m_ROM.m_FileEntries = Files.ToArray();
            Program.m_ROM.OVTSize = (uint)(0x20 * Overlays.Count);
            Program.m_ROM.FATSize = (uint)(0x8 * Files.Count);
            Program.m_ROM.RewriteSizeTables();
            Program.m_ROM.SaveFilesystem();
            Program.m_ROM.RevertFilesystem();

            return newOverlayID;
        }
	}
}
