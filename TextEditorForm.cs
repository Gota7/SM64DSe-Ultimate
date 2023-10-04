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
using System.Xml;
using System.IO;

namespace SM64DSe
{
    public partial class TextEditorForm : Form
    {
        public TextEditorForm()
        {
            InitializeComponent();
        }

        public class Message
        {
            public string msgData;
            public ushort width;
            public ushort height;
            public int index;

            const int limit = 45; // Length of preview text to be shown

            public Message(string msgData, ushort width, ushort height, int index = 0)
            {
                this.msgData = msgData;
                this.width = width;
                this.height = height;
                this.index = index;
            }

            public Message(ushort width, ushort height, int index = 0)
                : this("", width, height, index)
            {

            }

            public override string ToString()
            {
                string shortversion = msgData.Replace("\r\n", " ");
                shortversion = (msgData.Length > limit) ? msgData.Substring(0, limit - 3) + "..." : msgData;
                shortversion = string.Format("[{0:X4}] {1}", index, shortversion);
                return shortversion;
            }
        }

        List<Message> m_Messages;

        NitroFile file;

        int langIndex = -1;
        bool copyMessage;
        bool warned = false;

        string[] langs = new string[0];
        string[] langNames = new string[0];

        public static BiDictionaryOneToOne<byte, string> BASIC_EUR_US_CHARS = new BiDictionaryOneToOne<byte,string>();
        public static BiDictionaryOneToOne<byte, string> EXTENDED_ASCII_CHARS = new BiDictionaryOneToOne<byte,string>();
        public static BiDictionaryOneToOne<byte, string> JAP_CHARS = new BiDictionaryOneToOne<byte, string>();

        public static Dictionary<string, uint> BASIC_EUR_US_SIZES = new Dictionary<string, uint>();
        public static Dictionary<string, uint> EXTENDED_ASCII_SIZES = new Dictionary<string, uint>();
        public static Dictionary<string, uint> JAP_SIZES = new Dictionary<string, uint>();

        static TextEditorForm()
        {
            LoadCharList("assets/extended_ascii.txt", EXTENDED_ASCII_CHARS, EXTENDED_ASCII_SIZES);
            LoadCharList("assets/basic_eur_us_chars.txt", BASIC_EUR_US_CHARS, BASIC_EUR_US_SIZES);
            LoadCharList("assets/jap_chars.txt", JAP_CHARS, JAP_SIZES);
        }

        private void TextEditorForm_Load(object sender, EventArgs e)
        {
            NitroROM.Version theVersion = Program.m_ROM.m_Version;

            if (theVersion == NitroROM.Version.EUR)
            {
                lblVer.Text = "EUR";
                langs = new string[] { "English", "Français", "Deutsch", "Italiano", "Español" };
                langNames = new string[] { "eng", "frn", "gmn", "itl", "spn" };
            }
            else if (theVersion == NitroROM.Version.JAP)
            {
                lblVer.Text = "JAP";
                langs = new string[] { "Japanese", "English" };
                langNames = new string[] { "jpn", "nes" };
            }
            else if (theVersion == NitroROM.Version.USA_v1)
            {
                lblVer.Text = "USAv1";
                langs = new string[] { "English", "Japanese" };
                langNames = new string[] { "nes", "jpn" };
            }
            else if (theVersion == NitroROM.Version.USA_v2)
            {
                lblVer.Text = "USAv2";
                langs = new string[] { "English", "Japanese" };
                langNames = new string[] { "nes", "jpn" };
            }

            for (int i = 0; i < langs.Length; i++)
            {
                btnLanguages.DropDownItems.Add(langs[i]).Tag = i;
            }

            copyMessage = false;
        }

        public void ReadStrings(string fileName)
        {
            file = Program.m_ROM.GetFileFromName(fileName);

            uint inf1size = file.Read32(0x24);
            ushort numTextEntries = file.Read16(0x28);

            m_Messages = new List<Message>(numTextEntries);

            for (int i = 0; i < numTextEntries; i++)
            {
                uint widthAddr = (uint)(0x20 + 0x10 + (i * 8) + 0x4);
                uint heightAddr = (uint)(0x20 + 0x10 + (i * 8) + 0x6);
                m_Messages.Add(new Message(file.Read16(widthAddr), file.Read16(heightAddr), i));
            }

            lbxMsgList.Items.Clear();//Reset list of messages
            lbxMsgList.BeginUpdate();// Only draw when EndUpdate is called, much faster, expecially for Mono

            for (int i = 0; i < numTextEntries; i++)
            {
                uint straddr = file.Read32((uint)(0x30 + i * 8));
                straddr += 0x20 + inf1size + 0x8;

                int length = 0;

                string thetext = "";
                for (; ; )
                {
                    byte cur;
                    try
                    {
                        cur = file.Read8(straddr);
                    }
                    catch
                    {
                        break;
                    }
                    straddr++;
                    length++;
                    char thechar = '\0';

                    /*if ((cur >= 0x00) && (cur <= 0x09))
                        thechar = (char)('0' + cur);
                    else if ((cur >= 0x0A) && (cur <= 0x23))
                        thechar = (char)('A' + cur - 0x0A);
                    else if ((cur >= 0x2D) && (cur <= 0x46))
                        thechar = (char)('a' + cur - 0x2D);
                    else if ((cur >= 0x50) && (cur <= 0xCF))//Extended ASCII Characters
                        thechar = (char)(0x30 + cur);*/
                    // Some characters are two bytes long, can skip the second

                    if (langNames[langIndex] == "jpn")
                    {
                        if (JAP_CHARS.GetFirstToSecond().ContainsKey(cur))
                        {
                            thetext += JAP_CHARS.GetByFirst(cur);
                            straddr += (JAP_SIZES[JAP_CHARS.GetByFirst(cur)] - 1);
                            length += (int)(JAP_SIZES[JAP_CHARS.GetByFirst(cur)] - 1);
                        }
                    }
                    else
                    {
                        if ((cur >= 0x00 && cur <= 0x4F) || (cur >= 0xEE && cur <= 0xFB))
                        {
                            thetext += BASIC_EUR_US_CHARS.GetByFirst(cur);
                            straddr += (BASIC_EUR_US_SIZES[BASIC_EUR_US_CHARS.GetByFirst(cur)] - 1);
                            length += (int)(BASIC_EUR_US_SIZES[BASIC_EUR_US_CHARS.GetByFirst(cur)] - 1);
                        }
                        else if (cur >= 0x50 && cur <= 0xCF)
                        {
                            thetext += EXTENDED_ASCII_CHARS.GetByFirst(cur);
                            straddr += (EXTENDED_ASCII_SIZES[EXTENDED_ASCII_CHARS.GetByFirst(cur)] - 1);
                            length += (int)(EXTENDED_ASCII_SIZES[EXTENDED_ASCII_CHARS.GetByFirst(cur)] - 1);
                        }
                    }

                    if (thechar != '\0')
                        thetext += thechar;
                    else if (cur == 0xFD)
                        thetext += "\r\n";
                    else if (cur == 0xFF)
                        break;
                    else if (cur == 0xFE)// Special Character
                    {
                        int len = file.Read8(straddr);
                        thetext += "[\\r]";
                        thetext += string.Format("{0:X2}", cur);
                        for (int spec = 0; spec < len - 1; spec++)
                        {
                            thetext += string.Format("{0:X2}", file.Read8((uint)(straddr + spec)));
                        }
                        length += (len - 1);// Already increased by 1 at start
                        straddr += (uint)(len - 1);
                    }
                }

                m_Messages[i].msgData = thetext;

                lbxMsgList.Items.Add(m_Messages[i]);

                btnImport.Enabled = true; btnExport.Enabled = true;
            }
            lbxMsgList.EndUpdate();
        }

        private List<byte> EncodeString(string msg)
        {
            string newMsg = msg.Replace("[\\r]", "\r");
            char[] newTextByte = newMsg.ToCharArray();
            List<byte> encodedString = new List<byte>();

            int i = 0;
            while (i < newTextByte.Length)
            {
                /*
                // Upper
                // nintendo encoding = ('A' + cur - 0x0A);
                // ascii = A + ne - 0x0A
                // ascii - A + 0x0A = ne
                if (Char.IsNumber(newTextByte[i]))// Numeric
                    encodedString.Add((byte)(newTextByte[i] - '0'));
                else if (newTextByte[i] >= 0x41 && newTextByte[i] <= 0x5A)//Uppercase
                    encodedString.Add((byte)(newTextByte[i] - 'A' + 0x0A));
                else if (newTextByte[i] >= 0x61 && newTextByte[i] <= 0x7A)// Lowercase
                    encodedString.Add((byte)(newTextByte[i] - 'a' + 0x2D));
                else if (newTextByte[i] >= 0x80 && newTextByte[i] < (0xFF + 0x01))// Extended characters 128 to 255
                    encodedString.Add((byte)(newTextByte[i] - 0x30));// Character - offset of 0x30 to get Nintendo character*/

                if (langNames[langIndex] == "jpn")
                {
                    if (JAP_CHARS.GetSecondToFirst().ContainsKey("" + newTextByte[i]))
                        encodedString.Add(JAP_CHARS.GetBySecond("" + newTextByte[i]));
                }
                else
                {
                    if (BASIC_EUR_US_CHARS.GetSecondToFirst().ContainsKey("" + newTextByte[i]))
                        encodedString.Add(BASIC_EUR_US_CHARS.GetBySecond("" + newTextByte[i]));
                    else if (EXTENDED_ASCII_CHARS.GetSecondToFirst().ContainsKey("" + newTextByte[i]))
                        encodedString.Add(EXTENDED_ASCII_CHARS.GetBySecond("" + newTextByte[i]));
                }
                if (newTextByte[i].Equals('\r'))// New Line is \r\n
                {
                    i++;// Point after r
                    if (newTextByte[i].Equals('\n'))
                    {
                        encodedString.Add((byte)0xFD);
                        i++;
                        continue;
                    }
                    // 0xFE denotes special character
                    else if (newTextByte[i].Equals('F') && newTextByte[i + 1].Equals('E'))
                    {
                        //FE 05 03 00 06 - [R) glyph
                        //FE 07 01 00 00 00 XX - number of stars till you get XX
                        string byte2 = "" + newTextByte[i + 2] + newTextByte[i + 3];
                        int len = int.Parse(byte2, System.Globalization.NumberStyles.HexNumber);
                        for (int j = 0; j < (len * 2); j += 2)
                        {
                            string temp = "" + newTextByte[i + j] + newTextByte[i + j + 1];
                            encodedString.Add((byte)int.Parse(temp, System.Globalization.NumberStyles.HexNumber));
                        }
                        i += (len * 2);

                        continue;
                    }
                    else
                    {
                        // Special characters [\r]C [\r]S [\r]s [\r]D [\r]A [\r]B [\r]X [\r]Y

                        string specialChar = "[\\r]" + newTextByte[i];
                        uint size = 0;
                        byte val = 0xFF;

                        if (langNames[langIndex] == "jpn")
                        {
                            size = JAP_SIZES[specialChar];
                            val = JAP_CHARS.GetBySecond(specialChar);
                        }
                        else
                        {
                            if (BASIC_EUR_US_SIZES.ContainsKey(specialChar))
                                size = BASIC_EUR_US_SIZES[specialChar];
                            else if (EXTENDED_ASCII_SIZES.ContainsKey(specialChar))
                                size = EXTENDED_ASCII_SIZES[specialChar];

                            if (BASIC_EUR_US_CHARS.GetSecondToFirst().ContainsKey(specialChar))
                                val = BASIC_EUR_US_CHARS.GetBySecond(specialChar);
                            else if (EXTENDED_ASCII_CHARS.GetSecondToFirst().ContainsKey(specialChar))
                                val = EXTENDED_ASCII_CHARS.GetBySecond(specialChar);
                        }

                        for (int j = 0; j < size; j++)
                        {
                            encodedString.Add((byte)(val + j));
                        }

                        i++;
                        continue;
                    }
                }
                i++;
            }

            encodedString.Add(0xFF);// End of message

            return encodedString;
        }

        public static void LoadCharList(string txtName, BiDictionaryOneToOne<byte, string> charList,
            Dictionary<string, uint> sizeList)
        {
            string filename = Path.Combine(Application.StartupPath, txtName);
            string text = File.ReadAllText(filename);

            string[] lines = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                // Ignore comments
                if (lines[i].ToCharArray()[0] == '#')
                    continue;

                string[] pair = lines[i].Split('=');
                if (pair.Length < 3)
                    continue;

                try { 
                    charList.Add(byte.Parse(pair[0]), pair[2]); 
                    sizeList.Add(pair[2], uint.Parse(pair[1]));
                }
                catch (Exception e) { MessageBox.Show("Error in " + filename + "\n\n" + "Line " + i + "\n\n" + 
                    pair[0] + "\t" + pair[1] + "\t" + pair[2] + "\n\n" + e.Message); }
            }

        }

        private void lbxMsgList_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = lbxMsgList.SelectedIndex != -1;

            if (lbxMsgList.SelectedIndex == -1)
                return;

            Message selectedMessage = (Message)lbxMsgList.SelectedItem;

            tbxMsgPreview.Text = selectedMessage.msgData;
            nudWidth.Value = selectedMessage.width;
            nudHeight.Value = selectedMessage.height;

            if (copyMessage)
                txtEdit.Text = selectedMessage.msgData;
        }

        private void btnUpdateString_Click(object sender, EventArgs e)
        {
            if (lbxMsgList.SelectedIndex == -1)
                return;

            Message selectedMessage = (Message)lbxMsgList.SelectedItem;

            selectedMessage.msgData = txtEdit.Text;

            lbxMsgList.Items[lbxMsgList.SelectedIndex] = selectedMessage;
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            WriteData();

            int index = lbxMsgList.SelectedIndex;
            ReadStrings(file.m_Name); //Reload texts after saving
            lbxMsgList.SelectedIndex = index;
            Program.m_ROM.UpdateStrings();
        }

        private void WriteData()
        {
            uint infSize = 0x10 + (((uint)m_Messages.Count + 3) * 8);
            uint datSize = 0x8;

            uint datHeaderOffset = 0x20 + infSize;
            uint datStartOffset = datHeaderOffset + 0x8;

            uint infOffset = 0x30;
            uint datOffset = datStartOffset;

            // it starts with 0xff
            file.Write8(datOffset, 0xff);
            datOffset++;
            datSize++;

            foreach (Message message in m_Messages)
            {
                List<byte> entry = EncodeString(message.msgData);
                file.WriteBlock(datOffset, entry.ToArray<byte>());

                file.Write32(infOffset + 0x00, datOffset - datStartOffset);
                file.Write16(infOffset + 0x04, message.width);
                file.Write16(infOffset + 0x08, message.height);

                infOffset += 8;
                datOffset += (uint)entry.Count;
                datSize += (uint)entry.Count;
            }

            for (int i = 0; i < 3; i++)
            {
                file.Write32(infOffset + 0x00, 0);
                file.Write32(infOffset + 0x04, 0);

                infOffset += 8;
            }

            uint align = 16 - (datOffset % 16);
            if (align == 16)
                align = 0;

            uint totalSize = datOffset + align;
            datSize += align;

            for (uint i = 0; i < align; i++)
            {
                file.Write8(datOffset + i, 0xff);
            }

            file.Write32(0x00, 0x4d455347); // "MESG"
            file.Write32(0x04, 0x626d6731); // "bmg1"
            file.Write32(0x08, totalSize + 1);
            file.Write32(0x0c, 2); // unknown, set value (2) in known files
            // file.Write32(0x10, 0); // unknown, different value (1 or 3) in known files
            file.Write32(0x14, 0); // unknown, set value (0) in known files
            file.Write32(0x18, 0); // unknown, set value (0) in known files
            file.Write32(0x1c, 0); // unknown, set value (0) in known files

            file.Write32(0x20, 0x494e4631); // "INF1"
            file.Write32(0x24, infSize);
            file.Write16(0x28, (ushort)m_Messages.Count);
            file.Write16(0x2a, 0x40); // unknown, set value (0x40) in known files
            file.Write32(0x2c, 0); // unknown, set value (0) in known files

            file.Write32(datHeaderOffset + 0x00, 0x44415431); // "DAT1"
            file.Write32(datHeaderOffset + 0x04, datSize + 1);

            // make it so the file is only as big as it needs to be
            if (file.m_Data.Length - (int)totalSize > 0)
                file.RemoveSpace(totalSize, (uint)file.m_Data.Length - totalSize);

            // Save changes
            file.SaveChanges();
        }

        private void btnCoins_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]C";
        }

        private void btnStarFull_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]S";
        }

        private void btnStarEmpty_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]s";
        }

        private void btnDPad_Click(object sender, EventArgs e)
        {
            // FE05030000 doesn't always appear, depends on message
            txtEdit.Text += "[\\r]D";
        }

        private void btnA_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]A";
        }

        private void btnB_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]B";
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]X";
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]Y";
        }

        private void btnL_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]FE05030005";
        }

        private void btnR_Click(object sender, EventArgs e)
        {
            txtEdit.Text += "[\\r]FE05030006";
        }

        void btnLanguages_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            langIndex = int.Parse(e.ClickedItem.Tag.ToString());
            ReadStrings("data/message/msg_data_" + langNames[int.Parse(e.ClickedItem.Tag.ToString())] + ".bin");
        }
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To begin editing, select a language using the drop-down menu. This will display \n" +
                            "all of the languages available for your ROM Version.\n\n" +
                            "Next, click on the string you want to edit on the left-hand side.\n" +
                            "The full text will then be displayed in the upper-right box.\n\n" +
                            "Type your new text in the text box on the right-hand side.\n" +
                            "When done editing an entry, click 'Update String'.\n\nWhen you have finished, click " +
                            "on 'Save Changes'\n\n" +
                            "Use the buttons under the text editing box to insert the special characters.\n" + 
                            "[\\r] is the special character used by the text editor to indicate special characters.\n");
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportXML();
        }

        private void ImportXML()
        {
            MessageBox.Show("Sorry, due to a rewrite of the text editor, this feature is currently unavailable.", "Feature unavailable");
            /*OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Document (.xml)|*.xml";//Filter by .xml
            DialogResult dlgeResult = ofd.ShowDialog();
            if (dlgeResult == DialogResult.Cancel)
                return;

            lbxMsgList.Items.Clear();
            lbxMsgList.BeginUpdate();

            using (XmlReader reader = XmlReader.Create(ofd.FileName))
            {
                reader.MoveToContent();

                int i = 0;
                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        switch (reader.LocalName)
                        {
                            case "Text":
                                if (i < m_MsgData.Length)
                                {
                                    string temp = reader.ReadElementContentAsString();
                                    temp = temp.Replace("\n", "\r\n");
                                    temp = temp.Replace("[\\r]", "\r");
                                    m_MsgData[i] = temp;
                                    string shortversion = m_MsgData[i].Replace("\r\n", " ");
                                    shortversion = (m_MsgData[i].Length > limit) ? m_MsgData[i].Substring(0, limit - 3) + "..." : m_MsgData[i];
                                    lbxMsgList.Items.Add(string.Format("[{0:X4}] {1}", i, shortversion));
                                }
                                i++;
                                break;
                        }
                    }
                }
            }
            lbxMsgList.EndUpdate();

            for (int i = 0; i < m_MsgData.Length; i++)
            {
                UpdateEntries(m_MsgData[i], i);
                List<byte> entry = EncodeString(m_MsgData[i]);
                file.WriteBlock(m_StringHeaderData[i] + m_DAT1Start, entry.ToArray<byte>());
            }

            file.SaveChanges();

            ReadStrings("data/message/msg_data_" + langNames[langIndex] + ".bin");//Reload texts after saving*/
        }

        private void ExportXML()
        {
            MessageBox.Show("Sorry, due to a rewrite of the text editor, this feature is currently unavailable.", "Feature unavailable");
            /*SaveFileDialog saveXML = new SaveFileDialog();
            saveXML.FileName = "SM64DS Texts";//Default name
            saveXML.DefaultExt = ".xml";//Default file extension
            saveXML.Filter = "XML Document (.xml)|*.xml";//Filter by .xml
            if (saveXML.ShowDialog() == DialogResult.Cancel)
                return;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(saveXML.FileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment(Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate);
                writer.WriteStartElement("SM64DS_Texts");

                for (int i = 0; i < m_MsgData.Length; i++)
                {
                    writer.WriteStartElement("Text");
                    writer.WriteAttributeString("index", i.ToString());
                    writer.WriteAttributeString("id", String.Format("{0:X4}", i));
                    writer.WriteString(m_MsgData[i]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }*/
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportXML();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (file == null)
                return;

            RefreshListBox();
        }

        private void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            if (lbxMsgList.SelectedIndex == -1)
                return;

            Message selectedMessage = (Message)lbxMsgList.SelectedItem;
            selectedMessage.width = (ushort)nudWidth.Value;
        }

        private void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            if (lbxMsgList.SelectedIndex == -1)
                return;

            Message selectedMessage = (Message)lbxMsgList.SelectedItem;
            selectedMessage.height = (ushort)nudHeight.Value;
        }

        private void chkCopy_CheckedChanged(object sender, EventArgs e)
        {
            copyMessage = !copyMessage;
        }

        private void btnAddAbove_Click(object sender, EventArgs e)
        {
            if (lbxMsgList.SelectedIndex < 0)
                return;

            Message selectedMessage = (Message)lbxMsgList.SelectedItem;
            int index = m_Messages.IndexOf(selectedMessage);

            bool userSure = WarnUser(
                $"Are you sure you want to add a text entry above 0x{Convert.ToString(index, 16)}?\n" +
                $"Keep in mind that this changes the index of ALL entries that come after!",
                "Text entry deletion confirmation");

            if (!userSure)
                return;

            Message newMessage = new Message("New text message", 117, 6);
            m_Messages.Insert(index, newMessage);

            RefreshListBox();
            SelectMessage(newMessage);
        }

        private void btnAddBelow_Click(object sender, EventArgs e)
        {
            if (lbxMsgList.SelectedIndex < 0)
                return;

            Message selectedMessage = (Message)lbxMsgList.SelectedItem;
            int index = m_Messages.IndexOf(selectedMessage);

            bool userSure = WarnUser(
                $"Are you sure you want to add a text entry below 0x{Convert.ToString(index, 16)}?\n" +
                $"Keep in mind that this changes the index of ALL entries that come after!",
                "Text entry deletion confirmation");

            if (!userSure)
                return;

            Message newMessage = new Message("New text message", 117, 6);
            m_Messages.Insert(index + 1, newMessage);

            RefreshListBox();
            SelectMessage(newMessage);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbxMsgList.SelectedIndex < 0)
                return;

            Message selectedMessage = (Message)lbxMsgList.SelectedItem;
            int index = m_Messages.IndexOf(selectedMessage);

            bool userSure = WarnUser(
                $"Are you sure you want to delete text entry 0x{Convert.ToString(index, 16)}?\n" +
                $"Keep in mind that this changes the index of ALL entries that come after!",
                "Text entry deletion confirmation");

            if (!userSure)
                return;

            m_Messages.Remove(selectedMessage);

            RefreshListBox();
        }

        private bool WarnUser(string text, string caption)
        {
            if (!warned)
            {
                DialogResult dialogResult = MessageBox.Show(text, caption, MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                    warned = true;
            }

            return warned;
        }

        private void RefreshListBox()
        {
            lbxMsgList.BeginUpdate();

            for (int i = 0; i < m_Messages.Count; i++)
                m_Messages[i].index = i;

            string searchString = txtSearch.Text;

            if (searchString == null || searchString.Equals(""))
            {
                lbxMsgList.Items.Clear();
                lbxMsgList.Items.AddRange(m_Messages.ToArray());
            }
            else
            {
                string searchStringLower = searchString.ToLowerInvariant();

                lbxMsgList.Items.Clear();
                lbxMsgList.Items.AddRange(m_Messages.Where(m => m.msgData.ToLowerInvariant().Contains(searchStringLower)).ToArray());
            }

            lbxMsgList.EndUpdate();
        }

        private void SelectMessage(Message message)
        {
            lbxMsgList.SelectedIndex = lbxMsgList.Items.IndexOf(message);
        }
    }
}
