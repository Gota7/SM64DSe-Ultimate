using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScintillaNET;
using SM64DS_SCRIPT_LIB;

namespace SM64DSe
{

    public partial class KuppaScriptEditor : Form
    {

        Scintilla kps;
        Scintilla kpl;

        private const int BACK_COLOR = 0x2F2F2F;
        private const int FORE_COLOR = 0xB7B7B7;

        string oldFile;
        NitroFile nf;


        public KuppaScriptEditor()
        {
            InitializeComponent();
        }

        private void KuppaScriptEditor_Load(object sender, EventArgs e)
        {

            // CREATE CONTROL
            kps = new Scintilla();
            kpsPanel.Controls.Add(kps);
            kpl = new Scintilla();
            kplPanel.Controls.Add(kpl);

            // BASIC CONFIG
            InitStyle(ref kps);
            InitStyle(ref kpl);
            LoadKPL("kuppaLib.kpl");

            // HIGHLIGHTING
            kpl.Lexer = Lexer.Container;
            kpl.StyleNeeded += new EventHandler<StyleNeededEventArgs>(this.KPL_StyleNeeded);
            StyleKPL(0, kpl.Text.Length);

            kps.Lexer = Lexer.Container;
            kps.StyleNeeded += new EventHandler<StyleNeededEventArgs>(this.KPS_StyleNeeded);
            StyleKPL(0, kps.Text.Length);

        }


        private void InitNumberMargin(Scintilla s)
        {

            s.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            s.Styles[Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            s.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            s.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums = s.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

        }

        private void LoadKPL(string path)
        {
            if (File.Exists(path))
            {
                kpl.Text = File.ReadAllText(path);
            }
            else {
                MessageBox.Show("KPL file not found!");
                this.Close();
            }
        }

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }


        public void InitStyle(ref Scintilla s) {

            s.Dock = DockStyle.Fill;
            s.WrapMode = WrapMode.None;
            s.IndentationGuides = IndentView.LookBoth;
            s.StyleResetDefault();
            s.Styles[Style.Default].Font = "Consolas";
            s.Styles[Style.Default].Size = 10;
            s.Styles[Style.Default].BackColor = IntToColor(0x212121);
            s.Styles[Style.Default].ForeColor = IntToColor(0xE7E7E7);
            s.CaretForeColor = IntToColor(0xFFFFFF);
            s.StyleClearAll();
            s.ScrollWidth = 1;
            s.ScrollWidthTracking = true;
            InitNumberMargin(s);

        }

        private void KPL_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            var startPos = kpl.GetEndStyled();
            var endPos = e.Position;

            if (startPos >= 500) { startPos -= 500; } else { startPos = 0; }
            if ((kpl.Text.Length - endPos) >= 500) { endPos += 500; } else { endPos = kpl.Text.Length; }

            StyleKPL(startPos, endPos);
        }

        private void KPS_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            var startPos = kps.GetEndStyled();
            var endPos = e.Position;

            if (startPos >= 500) { startPos -= 500; } else { startPos = 0; }
            if ((kps.Text.Length - endPos) >= 500) { endPos += 500; } else { endPos = kps.Text.Length; }

            StyleKPS(startPos, endPos);
        }



        public void StyleKPL(int startPos, int endPos) {
            
            //Syntax highlighting.

            kpl.Styles[1].ForeColor = IntToColor(0xE7E7E7);
            kpl.Styles[2].ForeColor = Color.Red;
            kpl.Styles[3].ForeColor = IntToColor(0x6A76D1);
            kpl.Styles[4].ForeColor = IntToColor(0x6A76D1);
            kpl.Styles[5].ForeColor = Color.Violet;
            kpl.Styles[6].ForeColor = Color.Violet;
            kpl.Styles[7].ForeColor = Color.Cyan;

            KPLStyleState stateToBe = KPLStyleState.UNK;
            KPLStyleState state = KPLStyleState.UNK;

            int lineStartPosition = 0;

            for (int i = startPos; i < endPos; i++) {

                char c = kpl.Text[i];
                state = stateToBe;

                switch (c) {

                    case '\n':
                        lineStartPosition = i;
                        if (state != KPLStyleState.MultiComment)
                        {
                            state = KPLStyleState.UNK;
                            stateToBe = KPLStyleState.UNK;
                        }
                        break;

                    case '\r':
                        lineStartPosition = i;
                        if (state != KPLStyleState.MultiComment)
                        {
                            state = KPLStyleState.UNK;
                            stateToBe = KPLStyleState.UNK;
                        }
                        break;

                    case ':':
                        if (state != KPLStyleState.Comment && state != KPLStyleState.MultiComment)
                        {
                            kpl.StartStyling(lineStartPosition);
                            kpl.SetStyling(i - lineStartPosition, 2);
                        }
                        break;

                    case '/':
                        try
                        {

                            if (state != KPLStyleState.Comment && state != KPLStyleState.MultiComment)
                            {
                                if (kpl.Text[i + 1] == '/' || kpl.Text[i - 1] == '/')
                                {
                                    state = KPLStyleState.Comment;
                                    stateToBe = KPLStyleState.Comment;
                                    i = lineStartPosition;
                                }
                                
                                else if (kpl.Text[i + 1] == '*') {
                                    state = KPLStyleState.MultiComment;
                                    stateToBe = KPLStyleState.MultiComment;
                                    i = lineStartPosition;
                                }
                            }

                            if (state == KPLStyleState.MultiComment) {
                                if (kpl.Text[i - 1] == '*') {
                                    stateToBe = KPLStyleState.UNK;
                                }
                            }

                        } catch {

                        }
                        break;


                    case ']':
                    case ',':
                        if (state != KPLStyleState.Comment && state != KPLStyleState.MultiComment) {

                            stateToBe = KPLStyleState.UNK;
                            state = KPLStyleState.UNK;

                        }
                        break;

                }


                if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
                {

                    if (state != KPLStyleState.Comment && state != KPLStyleState.MultiComment) {
                        try
                        {
                            if (kpl.Text[i - 1] == '[' || kpl.Text[i - 1] == ' ' || kpl.Text[i - 1] == ',')
                            {
                                stateToBe = KPLStyleState.UNK;
                                state = KPLStyleState.Number;

                                if (kpl.Text[i + 1] == 'h' || kpl.Text[i + 1] == 'b' || kpl.Text[i + 1] == 'i')
                                {
                                    stateToBe = KPLStyleState.Flag;
                                }

                            }
                            else if ((kpl.Text[i - 1] != 'a' && kpl.Text[i - 1] != 'b' && kpl.Text[i - 1] != 'c' && kpl.Text[i - 1] != 'd' && kpl.Text[i - 1] != 'e' && kpl.Text[i - 1] != 'f' && kpl.Text[i - 1] != 'g' && kpl.Text[i - 1] != 'h' && kpl.Text[i - 1] != 'i' && kpl.Text[i - 1] != 'j' && kpl.Text[i - 1] != 'k' && kpl.Text[i - 1] != 'l' && kpl.Text[i - 1] != 'm' && kpl.Text[i - 1] != 'n' && kpl.Text[i - 1] != 'o' && kpl.Text[i - 1] != 'p' && kpl.Text[i - 1] != 'q' && kpl.Text[i - 1] != 'R' && kpl.Text[i - 1] != 's' && kpl.Text[i - 1] != 't' && kpl.Text[i - 1] != 'u' && kpl.Text[i - 1] != 'v' && kpl.Text[i - 1] != 'w' && kpl.Text[i - 1] != 'x' && kpl.Text[i - 1] != 'y' && kpl.Text[i - 1] != 'z' && kpl.Text[i - 1] != '1') || kpl.Text[i - 2] == '\t') {
                                state = KPLStyleState.InstructionNumber;
                                stateToBe = KPLStyleState.UNK;
                            }
                        }
                        catch { }
                    }

                }


                switch (state) {

                    case KPLStyleState.Comment:
                        kpl.StartStyling(i);
                        kpl.SetStyling(1, 3);
                        break;

                    case KPLStyleState.MultiComment:
                        kpl.StartStyling(i);
                        kpl.SetStyling(1, 4);
                        break;

                    case KPLStyleState.Number:
                        kpl.StartStyling(i);
                        kpl.SetStyling(1, 5);
                        break;

                    case KPLStyleState.Flag:
                        kpl.StartStyling(i);
                        kpl.SetStyling(1, 6);
                        break;

                    case KPLStyleState.InstructionNumber:
                        kpl.StartStyling(i);
                        kpl.SetStyling(1, 7);
                        break;

                    default:
                        kpl.StartStyling(i);
                        kpl.SetStyling(1, 1);
                        break;

                }

            }

        }


        public void StyleKPS(int startPos, int endPos)
        {

            //Syntax highlighting.

            kps.Styles[1].ForeColor = IntToColor(0xE7E7E7);
            kps.Styles[2].ForeColor = Color.Red;
            kps.Styles[3].ForeColor = IntToColor(0x6A76D1);
            kps.Styles[4].ForeColor = IntToColor(0x6A76D1);
            kps.Styles[5].ForeColor = Color.Violet;
            kps.Styles[6].ForeColor = Color.Violet;
            kps.Styles[7].ForeColor = Color.Cyan;

            kpsStyleState stateToBe = kpsStyleState.UNK;
            kpsStyleState state = kpsStyleState.UNK;

            int lineStartPosition = 0;

            for (int i = startPos; i < endPos; i++)
            {

                char c = kps.Text[i];
                state = stateToBe;

                switch (c)
                {

                    case '\n':
                        lineStartPosition = i;
                        if (state != kpsStyleState.MultiComment)
                        {
                            state = kpsStyleState.UNK;
                            stateToBe = kpsStyleState.UNK;
                        }
                        break;

                    case '\r':
                        lineStartPosition = i;
                        if (state != kpsStyleState.MultiComment)
                        {
                            state = kpsStyleState.UNK;
                            stateToBe = kpsStyleState.UNK;
                        }
                        break;

                    case '/':
                        try
                        {

                            if (state != kpsStyleState.Comment && state != kpsStyleState.MultiComment)
                            {
                                if (kps.Text[i + 1] == '/' || kps.Text[i - 1] == '/')
                                {
                                    state = kpsStyleState.Comment;
                                    stateToBe = kpsStyleState.Comment;
                                    i = lineStartPosition;
                                }

                                else if (kps.Text[i + 1] == '*')
                                {
                                    state = kpsStyleState.MultiComment;
                                    stateToBe = kpsStyleState.MultiComment;
                                    i = lineStartPosition;
                                }
                            }

                            if (state == kpsStyleState.MultiComment)
                            {
                                if (kps.Text[i - 1] == '*')
                                {
                                    stateToBe = kpsStyleState.UNK;
                                }
                            }

                        }
                        catch
                        {

                        }
                        break;

                }


                switch (state)
                {

                    case kpsStyleState.Comment:
                        kps.StartStyling(i);
                        kps.SetStyling(1, 3);
                        break;

                    case kpsStyleState.MultiComment:
                        kps.StartStyling(i);
                        kps.SetStyling(1, 4);
                        break;

                    case kpsStyleState.Character:
                        kps.StartStyling(i);
                        kps.SetStyling(1, 5);
                        break;

                    case kpsStyleState.Argument:
                        kps.StartStyling(i);
                        kps.SetStyling(1, 6);
                        break;

                    case kpsStyleState.TimeFrame:
                        kps.StartStyling(i);
                        kps.SetStyling(1, 7);
                        break;

                    default:
                        kps.StartStyling(i);
                        kps.SetStyling(1, 1);
                        break;

                }

            }

        }

        public enum KPLStyleState {

            UNK, Number, Flag, Header, Comment, CommentPending, MultiComment, InstructionNumber

        }

        public enum kpsStyleState
        {

            UNK, Argument, Comment, CommentPending, MultiComment, TimeFrame, Character

        }


        /// <summary>
        /// Save KPL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveKPLButton_ButtonClick(object sender, EventArgs e)
        {
            File.WriteAllText("kuppaLib.kpl", kpl.Text);
        }


        /// <summary>
        /// Open KPS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ROMFileSelect r = new ROMFileSelect();
            r.ShowDialog();
            string file = r.m_SelectedFile;

            if (file != null && file != "") {

                nf = new NitroFile(Program.m_ROM, Program.m_ROM.GetFileIDFromName(file));

                MemoryStream src = new MemoryStream(nf.m_Data);
                BinaryReader br = new BinaryReader(src);

                if (src.Length >= 0x10)
                {

                    UInt32 magic1 = br.ReadUInt32();
                    UInt32 magic2 = br.ReadUInt32();
                    if (magic1 == 0x00100001)
                    {

                        if (magic2 == 0x00140018)
                        {

                            OpenDkl(nf.m_Data, false);

                        }
                        else if (magic2 == 0x0018001C)
                        {

                            OpenDkl(nf.m_Data, true);

                        }
                        else
                        {
                            OpenBin(nf.m_Data);
                        }

                    }
                    else {

                        OpenBin(nf.m_Data);

                    }

                }

                else {

                    OpenBin(nf.m_Data);

                }

            }

            

        }



        public void OpenBin(byte[] bin) {

            bool garbage = true;
            KuppaLib kl = new KuppaLib(ref garbage, kpl.Text.Split('\n'));
            CommandBinary cb = new CommandBinary(bin);
            kps.Text = string.Join("\n\n", SM64DS_SCRIPT_LIB.KuppaFactory.CommandBinaryToKPS(cb, kl));

        }


        public void OpenDkl(byte[] bin, bool isLong) {

            MemoryStream src = new MemoryStream(bin);
            BinaryReader br = new BinaryReader(src);

            if (isLong)
            {

                string startInfoS = "#save_info ";
                src.Position = 0x20;
                startInfoS += br.ReadByte() + ", ";

                src.Position = 0x30;
                int bit = br.ReadByte();
                if ((bit & 0b1) == 0b1) { startInfoS += 0 + ""; }
                else if ((bit & 0b10) == 0b10) { startInfoS += 1 + ""; }
                else if ((bit & 0b100) == 0b100) { startInfoS += 2 + ""; }
                else if ((bit & 0b1000) == 0b1000) { startInfoS += 3 + ""; }
                else if ((bit & 0b10000) == 0b10000) { startInfoS += 4 + ""; }
                else if ((bit & 0b100000) == 0b100000) { startInfoS += 5 + ""; }
                else if ((bit & 0b1000000) == 0b1000000) { startInfoS += 6 + ""; }
                else if ((bit & 0b10000000) == 0b10000000) { startInfoS += 7 + ""; }
                else if ((bit & 0b100000000) == 0b100000000) { startInfoS += 8 + ""; }
                else if ((bit & 0b1000000000) == 0b1000000000) { startInfoS += 9 + ""; }
                else if ((bit & 0b10000000000) == 0b10000000000) { startInfoS += 10 + ""; }
                else if ((bit & 0b100000000000) == 0b100000000000) { startInfoS += 11 + ""; }
                else if ((bit & 0b1000000000000) == 0b1000000000000) { startInfoS += 12 + ""; }
                else if ((bit & 0b10000000000000) == 0b10000000000000) { startInfoS += 13 + ""; }
                else if ((bit & 0b100000000000000) == 0b100000000000000) { startInfoS += 14 + ""; }
                else if ((bit & 0b1000000000000000) == 0b1000000000000000) { startInfoS += 15 + ""; }

                src.Position = 0x40;
                bool garbage = true;
                KuppaLib kl = new KuppaLib(ref garbage, kpl.Text.Split('\n'));
                CommandBinary cb = new CommandBinary(br.ReadBytes(bin.Length - 0x20));
                kps.Text = startInfoS + "\n\n" + string.Join("\n\n", SM64DS_SCRIPT_LIB.KuppaFactory.CommandBinaryToKPS(cb, kl));

            }
            else {

                src.Position = 0x20;
                bool garbage = true;
                KuppaLib kl = new KuppaLib(ref garbage, kpl.Text.Split('\n'));
                CommandBinary cb = new CommandBinary(br.ReadBytes(bin.Length - 0x20));
                kps.Text = string.Join("\n\n", SM64DS_SCRIPT_LIB.KuppaFactory.CommandBinaryToKPS(cb, kl));

            }

        }


        /// <summary>
        /// Save DKL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveDKLButton_ButtonClick(object sender, EventArgs e)
        {

            if (nf == null)
            {
                MessageBox.Show("No file opened!");
            }
            else
            {

                byte[] h = GetDKL();
                if (h != null)
                {
                    nf.m_Data = h;
                    nf.SaveChanges();
                }

            }

        }


        /// <summary>
        /// Save bin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveBINButton_ButtonClick(object sender, EventArgs e)
        {

            if (nf == null)
            {
                MessageBox.Show("No file opened!");
            }
            else
            {

                byte[] h = GetBIN();
                if (h != null)
                {
                    nf.m_Data = h;
                    nf.SaveChanges();
                }

            }

            }


        /// <summary>
        /// Get the BIN file.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBIN() {

            bool notGarbage = true;
            KuppaLib kl = new KuppaLib(ref notGarbage, kpl.Text.Split('\n'));
            CommandBinary cb = KuppaFactory.KPSToCommandBinary(kps.Text.Split('\n'), kl, ref notGarbage);
            if (notGarbage)
            {

                return cb.toBytes(true);

            }

            return null;

        }


        /// <summary>
        /// Get DKL.
        /// </summary>
        /// <returns></returns>
        public byte[] GetDKL() {

                bool notGarbage = true;
                KuppaLib kl = new KuppaLib(ref notGarbage, kpl.Text.Split('\n'));
                CommandBinary cb = KuppaFactory.KPSToCommandBinary(kps.Text.Split('\n'), kl, ref notGarbage);
                MemoryStream o = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(o);
            if (notGarbage)
            {

                bool isLong = false;
                byte conditionParam1 = 0xFF;
                byte conditionParam2 = 0xFF;
                foreach (string s in kps.Text.Split('\n'))
                {

                    if (s.StartsWith("#save_info"))
                    {

                        isLong = true;
                        string s2 = s.Remove(0, 11);
                        s2.Replace(" ", "");
                        conditionParam1 = byte.Parse(NumberInterpreter.ParseValue(s2.Split(',')[0]));
                        conditionParam2 = (byte)(1 << byte.Parse(NumberInterpreter.ParseValue(s2.Split(',')[1])));

                    }

                }

                if (isLong)
                {

                    bw.Write((UInt16)1);
                    bw.Write((UInt16)(0x10));
                    bw.Write((UInt16)0x1C);
                    bw.Write((UInt16)0x18);
                    bw.Write(new byte[8]);
                    bw.Write((UInt16)0x003C);
                    bw.Write((UInt16)0x00);
                    bw.Write((UInt32)0x0209CAA0);
                    bw.Write((UInt32)0xE12FFF1E);
                    bw.Write((UInt32)0xE51F0010);
                    bw.Write(conditionParam1);
                    bw.Write(new List<byte> { 0x10, 0xD0, 0xE5 }.ToArray());
                    bw.Write(conditionParam2);
                    bw.Write(new List<byte> { 0x20, 0x01, 0xE2 }.ToArray());
                    bw.Write((UInt32)0xE3520000);
                    bw.Write((UInt32)0x112FFF1E);
                    bw.Write(conditionParam2);
                    bw.Write(new List<byte> { 0x10, 0x81, 0xE3 }.ToArray());
                    bw.Write(conditionParam1);
                    bw.Write(new List<byte> { 0x10, 0xC0, 0xE5 }.ToArray());
                    bw.Write((UInt32)0xE1A0000F);
                    bw.Write((UInt32)0xEA803BC1);
                    bw.Write(cb.toBytes());

                }
                else
                {

                    bw.Write((UInt16)1);
                    bw.Write((UInt16)0x10);
                    bw.Write((UInt16)0x18);
                    bw.Write((UInt16)0x14);
                    bw.Write(new byte[8]);
                    bw.Write((UInt16)0x1C);
                    bw.Write((UInt16)0x00);
                    bw.Write((UInt32)0xE12FFF1E);
                    bw.Write((UInt32)0xE1A0000F);
                    bw.Write((UInt32)0xEA803BC1);
                    bw.Write(cb.toBytes());

                }

                while ((o.Length % 4) != 0)
                {
                    bw.Write((byte)0xFF);
                }

                return o.ToArray();

            }

            return null;

        }


        /// <summary>
        /// Close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (nf != null)
            {
                DialogResult dialogResult = MessageBox.Show("Close currently opened file?", "Warning:", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {

                    nf = null;
                    kps.Text = "";

                }
            }
            else {
                 MessageBox.Show("No file opened!");
            }

        }


        /// <summary>
        /// Import.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {

            importer.ShowDialog();
            if (importer.FileName != "") {

                switch (importer.FilterIndex) {

                    case 1:
                        kps.Text = File.ReadAllText(importer.FileName);
                        break;

                    case 2:
                    case 3:
                            byte[] h = File.ReadAllBytes(importer.FileName);
                            MemoryStream src = new MemoryStream(h);
                            BinaryReader br = new BinaryReader(src);

                        if (src.Length >= 0x10)
                        {

                            UInt32 magic1 = br.ReadUInt32();
                            UInt32 magic2 = br.ReadUInt32();
                            if (magic1 == 0x00100001)
                            {

                                if (magic2 == 0x00140018)
                                {

                                    OpenDkl(h, false);

                                }
                                else if (magic2 == 0x0018001C)
                                {

                                    OpenDkl(h, true);

                                }
                                else
                                {
                                    OpenBin(h);
                                }

                            }
                            else
                            {

                                OpenBin(h);

                            }

                        }

                        else
                        {

                            OpenBin(h);

                        }

                        break;

                }
                importer.FileName = "";

            }
        }


        /// <summary>
        /// Export.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

            exporter.ShowDialog();
            if (exporter.FileName != "")
            {

                switch (exporter.FilterIndex)
                {

                    case 1:
                        File.WriteAllText(exporter.FileName, kps.Text);
                        break;

                    case 2:
                        File.WriteAllBytes(exporter.FileName, GetDKL());
                        break;

                    case 3:
                        File.WriteAllBytes(exporter.FileName, GetBIN());
                        break;

                }

                exporter.FileName = "";

            }

        }


        /// <summary>
        /// Save as DKL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsDKLButton_ButtonClick(object sender, EventArgs e)
        {

            ROMFileSelect r = new ROMFileSelect();
            r.ShowDialog();
            string file = r.m_SelectedFile;
            if (file != null && file != "")
            {

                nf = new NitroFile(Program.m_ROM, Program.m_ROM.GetFileIDFromName(file));
                byte[] h = GetDKL();
                if (h != null)
                {
                    nf.m_Data = h;
                    nf.SaveChanges();
                }

            }

        }


        /// <summary>
        /// Save as BIN.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsBINButton_ButtonClick(object sender, EventArgs e)
        {

            ROMFileSelect r = new ROMFileSelect();
            r.ShowDialog();
            string file = r.m_SelectedFile;
            if (file != null && file != "")
            {

                nf = new NitroFile(Program.m_ROM, Program.m_ROM.GetFileIDFromName(file));
                byte[] h = GetBIN();
                if (h != null)
                {
                    nf.m_Data = h;
                    nf.SaveChanges();
                }

            }

        }

    }

}
