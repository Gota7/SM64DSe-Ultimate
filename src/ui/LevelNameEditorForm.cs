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
        private List<string> m_LevelNames;
        private List<string> m_ShortNames;
        private List<uint> m_OverlayIDs;
        private int m_BiggestInternalLevelNameSize = 0;
        private bool m_UpdatingTextBoxes = false;
        private bool m_UpdatingListBox = false;

        public LevelNameEditorForm(string fileName)
        {
            InitializeComponent();

            m_LevelNames = Strings.LevelNames();
            m_ShortNames = Strings.ShortLvlNames();
            m_OverlayIDs = new List<uint>();
            CalculateBiggestInternalLevelNameSize(false); // false because the labels haven't been added yet

            Program.m_ROM.BeginRW();
            for (int i = 0; i < m_LevelNames.Count; i++)
            {
                m_OverlayIDs.Add(Program.m_ROM.Read32(0x758c8 + (uint)(i * 4)));
                lstLevels.Items.Add(GetLevelLabel(i));
            }
            Program.m_ROM.EndRW();
        }

        public LevelNameEditorForm()
            : this(null) { }

        private void LevelNameEditorForm_Load(object sender, System.EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (m_LevelNames.Count != m_ShortNames.Count)
                throw new ArgumentException("The amount of full level names is not equal to the amount of short level names.");

            // save the XML
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(Application.StartupPath, "Levels.xml"), settings))
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

            Close();

            // save the overlay ids
            /*Program.m_ROM.BeginRW();
            for (int i = 0; i < m_LevelNames.Count; i++)
                Program.m_ROM.Write32(0x758c8 + (uint)(i * 4), m_OverlayIDs[i]);
            Program.m_ROM.EndRW();*/
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
    }
}
