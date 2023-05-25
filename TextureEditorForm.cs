using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SM64DSe.SM64DSFormats;
using OpenTK;

namespace SM64DSe
{
    public partial class TextureEditorForm : Form
    {
        private BMD m_Model;
        private BTP m_BTP;
        private string m_ModelName;
        private string m_BtpName;

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();
        private FolderBrowserDialog m_FolderBrowserDialogue = new FolderBrowserDialog();

        private System.Windows.Forms.Timer m_BTPTimer;
        private int timerCount = 0;

        public TextureEditorForm(string btpFileName, string bmdFileName)
        {
            InitializeComponent();

            m_ModelName = bmdFileName;
            m_BtpName = btpFileName;
        }

        public TextureEditorForm()
            : this(null, null) { }

        private void TextureEditorForm_Load(object sender, System.EventArgs e)
        {
            if (m_ModelName == null)
            {
                m_ROMFileSelect.ReInitialize("Select a BMD file to load",new String[] {".bmd"});
                DialogResult result = m_ROMFileSelect.ShowDialog();
                if (result != DialogResult.OK)
                {
                    Close();
                }
                else
                {
                    m_ModelName = m_ROMFileSelect.m_SelectedFile;
                }
            }

            LoadTextures();
            InitTimer();

            if (!string.IsNullOrWhiteSpace(m_BtpName))
            {
                ClearBTPTextBoxes();
                LoadBTP(m_BtpName);
            }
        }

        private void LoadTextures()
        {
            // Load the model
            m_Model = new BMD(Program.m_ROM.GetFileFromName(m_ModelName));

            lbxTextures.Items.Clear();

            for (int i = 0; i < m_Model.m_TextureIDs.Count; i++)
            {
                lbxTextures.Items.Add(m_Model.m_TextureIDs.Keys.ElementAt(i));
            }

            lbxPalettes.Items.Clear();

            for (int i = 0; i < m_Model.m_PaletteIDs.Keys.Count; i++)
            {
                lbxPalettes.Items.Add(m_Model.m_PaletteIDs.Keys.ElementAt(i));
            }
        }

        private void InitTimer()
        {
            m_BTPTimer = new System.Windows.Forms.Timer();
            m_BTPTimer.Interval = (int)(1000f / 30f);
            m_BTPTimer.Tick += new EventHandler(m_BTPTimer_Tick);
        }

        private void StartTimer()
        {
            timerCount = 0;
            btnMatPreview.Enabled = false;
            btnMatPreviewStop.Enabled = true;
            m_BTPTimer.Start();
        }

        private void StopTimer()
        {
            m_BTPTimer.Stop();
            btnMatPreview.Enabled = true;
            btnMatPreviewStop.Enabled = false;
        }

        private void lbxTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btnReplaceSelected.Visible = true;

                if (lbxTextures.SelectedIndex == -1 || lbxTextures.SelectedIndex >= lbxTextures.Items.Count)
                    return;
                string texName = lbxTextures.Items[lbxTextures.SelectedIndex].ToString();
                Console.WriteLine(m_Model.m_Textures[texName].m_PaletteID);

                if (rbTexAllInBMD.Checked && m_Model.m_Textures.ContainsKey(texName))
                {
                    if (m_Model.m_Textures[texName].m_PaletteID >= 0 && m_Model.m_Textures[texName].m_PaletteID < lbxPalettes.Items.Count)
                    {
                        lbxPalettes.Enabled = true;
                        lbxPalettes.SelectedIndex = (int)m_Model.m_Textures[texName].m_PaletteID;
                    }
                    else if (m_Model.m_Textures[texName].m_PaletteID == 0xFFFFFFFF)
                    {
                        lbxPalettes.SelectedIndex = -1;
                        lbxPalettes.Enabled = false;
                        lblPalette.Text = "Palette: None";
                    }
                }

                if (rbTexAllInBMD.Checked)
                {
                    NitroTexture currentTexture = m_Model.m_Textures[texName];
                    if (currentTexture.m_PaletteID == 0xFFFFFFFF)
                    {
                        RefreshImage(currentTexture);
                        lblTexture.Text = "Texture: (ID " + currentTexture.m_TextureID + ")";
                    }
                    else if (lbxPalettes.SelectedIndex > -1)
                    {
                        string palName = lbxPalettes.SelectedItem.ToString();
                        currentTexture = NitroTexture.ReadFromBMD(m_Model, m_Model.m_TextureIDs[texName],
                            m_Model.m_PaletteIDs[palName]);

                        RefreshImage(currentTexture);

                        lblTexture.Text = "Texture: (ID " + m_Model.m_TextureIDs[texName] + ")";
                    }
                }

                if (rbTexAsRefInBTP.Checked)
                {
                    txtBTPTextureName.Text = texName;
                    if (m_Model.m_TextureIDs.ContainsKey(texName))
                        lblTexture.Text = "Texture: (ID " + m_Model.m_TextureIDs[texName] + ")";
                }
            }
            // let's not annoy the user with these error messages
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                pbxTexture.Image = new Bitmap(1, 1);
                btnReplaceSelected.Visible = false;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                pbxTexture.Image = new Bitmap(1, 1);
            }
        }

        private void lbxPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbxPalettes.SelectedIndex < 0)
                    return;
                string palName = lbxPalettes.Items[lbxPalettes.SelectedIndex].ToString();
                if (rbTexAsRefInBTP.Checked)
                    txtBTPPaletteName.Text = palName;
                if (lbxTextures.SelectedIndex != -1 && lbxPalettes.SelectedIndex != -1 && lbxPalettes.SelectedIndex < lbxPalettes.Items.Count)
                {
                    string texName = lbxTextures.Items[lbxTextures.SelectedIndex].ToString();
                    if (m_Model.m_TextureIDs.ContainsKey(texName) && m_Model.m_PaletteIDs.ContainsKey(palName))
                    {
                        NitroTexture currentTexture = NitroTexture.ReadFromBMD(m_Model, m_Model.m_TextureIDs[texName],
                            m_Model.m_PaletteIDs[palName]);

                        RefreshImage(currentTexture);

                        lblPalette.Text = "Palette: (ID " + m_Model.m_PaletteIDs[palName] + ")";
                    }
                }
            }
            // let's not annoy the user with these error messages
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                pbxTexture.Image = new Bitmap(1, 1);
            }
        }

        private void RefreshImage(NitroTexture currentTexture)
        {
            pbxTexture.Image = currentTexture.ToBitmap();
            pbxTexture.Refresh();
        }

        private void btnExportAll_Click(object sender, EventArgs e)
        {
            m_FolderBrowserDialogue.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
            DialogResult result = m_FolderBrowserDialogue.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = m_FolderBrowserDialogue.SelectedPath;
                for (int i = 0; i < m_Model.m_Textures.Values.Count; i++)
                {
                    NitroTexture currentTexture = m_Model.m_Textures.Values.ElementAt(i);

                    SaveTextureAsPNG(currentTexture, folderName + "/" + currentTexture.m_TextureName + ".png");
                }
                MessageBox.Show("Successfully exported " + m_Model.m_Textures.Values.Count + " texture(s) to:\n" + folderName);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lbxTextures.SelectedIndex != -1 && lbxPalettes.SelectedIndex != -1)
            {
                string texName = lbxTextures.SelectedItem.ToString();
                string palName = lbxPalettes.SelectedItem.ToString();
                NitroTexture currentTexture = NitroTexture.ReadFromBMD(m_Model, m_Model.m_TextureIDs[texName],
                    m_Model.m_PaletteIDs[palName]);

                SaveFileDialog export = new SaveFileDialog();
                export.FileName = currentTexture.m_TextureName;//Default name
                export.DefaultExt = ".bmp";//Because the texture has a palette
                export.Filter = "Image Files (*.bmp,*.png) | *.bmp;*.png";
                if (export.ShowDialog() == DialogResult.Cancel)
                    return;

                string extension = export.FileName.Substring(export.FileName.Length - 4, 4);
                if (extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                    SaveTextureAsPNG(currentTexture, export.FileName);
                else
                    SaveTextureAsBMP(currentTexture, export.FileName);
            }
            else
            {
                MessageBox.Show("Please select a texture first.");
            }
        }

        private static void SaveTextureAsBMP(NitroTexture currentTexture, String fileName)
        {
            try
            {
                currentTexture.ToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save texture " + currentTexture.m_TextureName + ".\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private static void SaveTextureAsPNG(NitroTexture currentTexture, String fileName)
        {
            try
            {
                currentTexture.ToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save texture " + currentTexture.m_TextureName + ".\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private void btnReplaceSelected_Click(object sender, EventArgs e)
        {
            // TODO: Ideally this will be done by loading the BMD to a ModelBase object, modifying the 
            // selected textures and then writing the ModelBase object to BMD again.

            if (lbxTextures.SelectedIndex != -1)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select an image";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.Cancel) return;

                bool hasPalette = lbxPalettes.SelectedIndex > -1;

                string texName = lbxTextures.SelectedItem.ToString();
                string palName = hasPalette ? lbxPalettes.SelectedItem.ToString() : null;
                uint texIndex = m_Model.m_TextureIDs[texName];
                uint palIndex = hasPalette ? m_Model.m_PaletteIDs[palName] : 0xFFFFFFFF;
                int srcTexType = m_Model.m_Textures[texName].m_TexType;

                try
                {
                    Bitmap bmp = new Bitmap(ofd.FileName);

                    int dstTexType = BestTexTypeForBitmap(bmp, srcTexType, chkReplaceCompressTexture.Checked);
                    NitroTexture tex = NitroTexture.FromBitmapAndType(
                        texIndex, texName, 
                        palIndex, palName, 
                        bmp, dstTexType
                    );

                    // Update texture entry
                    uint curoffset = m_Model.m_Textures[texName].m_EntryOffset;

                    m_Model.m_File.Write32(curoffset + 0x08, (uint)tex.m_TextureDataLength);
                    m_Model.m_File.Write16(curoffset + 0x0C, (ushort)(8 << (int)((tex.m_DSTexParam >> 20) & 0x7)));
                    m_Model.m_File.Write16(curoffset + 0x0E, (ushort)(8 << (int)((tex.m_DSTexParam >> 23) & 0x7)));
                    m_Model.m_File.Write32(curoffset + 0x10, tex.m_DSTexParam);

                    // Update palette entry
                    if (tex.m_RawPaletteData != null)
                    {
                        curoffset = m_Model.m_Textures[texName].m_PalEntryOffset;

                        m_Model.m_File.Write32(curoffset + 0x08, (uint)tex.m_RawPaletteData.Length);
                        m_Model.m_File.Write32(curoffset + 0x0C, 0xFFFFFFFF);
                    }

                    // Write new texture and texture palette data

                    NitroTexture oldTex = m_Model.m_Textures[texName];

                    // Check whether we need to make room for additional data
                    uint oldTexDataSize = (uint)oldTex.m_RawTextureData.Length;
                    uint newTexDataSize = (uint)((tex.m_RawTextureData.Length + 3) & ~3);
                    uint oldPalDataSize = (uint)oldTex.m_PaletteDataLength;
                    uint newPalDataSize = (uint)((tex.m_PaletteDataLength + 3) & ~3);

                    uint texDataOffset = m_Model.m_File.Read32(oldTex.m_EntryOffset + 0x04);
                    // If necessary, make room for additional texture data
                    if (newTexDataSize > oldTexDataSize) m_Model.AddSpace(texDataOffset + oldTexDataSize, newTexDataSize - oldTexDataSize);

                    m_Model.m_File.WriteBlock(texDataOffset, tex.m_RawTextureData);

                    uint palDataOffset = m_Model.m_File.Read32(oldTex.m_PalEntryOffset + 0x04);
                    // If necessary, make room for additional palette data
                    if (newPalDataSize > oldPalDataSize) m_Model.AddSpace(palDataOffset + oldPalDataSize, newPalDataSize - oldPalDataSize);
                    // Reload palette data offset
                    palDataOffset = m_Model.m_File.Read32(oldTex.m_PalEntryOffset + 0x04);

                    if (tex.m_RawPaletteData != null) m_Model.m_File.WriteBlock(palDataOffset, tex.m_RawPaletteData);

                    if (chkReplaceAdjustTexCoords.Checked)
                    {
                        // Tried just modifying the materials' texture scale but whilst the models displayed fine 
                        // in the editor, they didn't display properly in the game. 
                        // Instead we'll scale the texture co-ordinates (modify the GX command parameters) 
                        // and update material settings as needed. Uses the same logic as when we read and write BMDs.

                        if (oldTex.m_Width != tex.m_Width || oldTex.m_Height != tex.m_Height)
                        {
                            AdjustTextureCoordinates(oldTex, tex);
                        }
                    }

                    m_Model.m_File.SaveChanges();

                    LoadTextures();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.Source + ex.TargetSite + ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please select a texture first.");
            }
        }

        private void AdjustTextureCoordinates(NitroTexture oldTex, NitroTexture tex)
        {
            List<byte> matIDs = new List<byte>();
            for (uint i = 0; i < m_Model.m_NumMatChunks; i++)
            {
                uint matChunkOffset = m_Model.m_MatChunksOffset + (i * 48);
                uint matTexID = m_Model.m_File.Read32(matChunkOffset + 0x04);
                if (matTexID == oldTex.m_TextureID)
                {
                    matIDs.Add((byte)i);
                }
            }

            HashSet<uint> updatedDLists = new HashSet<uint>();
            for (uint chunkIdx = 0; chunkIdx < m_Model.m_NumModelChunks; chunkIdx++)
            {
                uint modelChunkOffset = m_Model.m_ModelChunksOffset + (chunkIdx * 64);
                uint numMatGroups = m_Model.m_File.Read32(modelChunkOffset + 0x30);
                if (numMatGroups > 0)
                {
                    uint matGroupIDsOffset = m_Model.m_File.Read32(modelChunkOffset + 0x34);
                    uint polyIDsOffset = m_Model.m_File.Read32(modelChunkOffset + 0x38);
                    for (uint matGrpIdx = 0; matGrpIdx < numMatGroups; matGrpIdx++)
                    {
                        byte matID = m_Model.m_File.Read8(matGroupIDsOffset + matGrpIdx);
                        if (matIDs.Contains(matID))
                        {
                            uint matOffset = (uint)(m_Model.m_MatChunksOffset + (matID * 48));
                            MatGroupTexCoordSettings mat = ReadMatGroupTexCoordSettings(matOffset);

                            byte polyID = m_Model.m_File.Read8(polyIDsOffset + matGrpIdx);
                            uint pchunkoffset = m_Model.m_File.Read32((uint)(m_Model.m_PolyChunksOffset + (polyID * 8) + 4));
                            uint dloffset = m_Model.m_File.Read32(pchunkoffset + 0x0C);
                            if (updatedDLists.Contains(dloffset)) { continue; }
                            else { updatedDLists.Add(dloffset); }
                            uint dlsize = m_Model.m_File.Read32(pchunkoffset + 0x08);

                            Vector2 tcscale = new Vector2(tex.m_Width, tex.m_Height);

                            float largesttc = ScaleTexCoords(mat, dloffset, dlsize, oldTex, tcscale, false);

                            float _tcscale = largesttc / (32767f / 16f);
                            if (_tcscale > 1f)
                            {
                                _tcscale = (float)Math.Ceiling(_tcscale * 4096f) / 4096f;
                                mat.m_TexCoordScale = new Vector2(_tcscale, _tcscale);
                                Vector2.Divide(ref tcscale, _tcscale, out tcscale);
                                mat.m_TexParams |= 0x40000000;
                            }

                            ScaleTexCoords(mat, dloffset, dlsize, oldTex, tcscale, true);

                            WriteMatGroupTexCoordSettings(matOffset, mat);
                        }
                    }
                }
            }
        }

        private float ScaleTexCoords(MatGroupTexCoordSettings mat, uint dloffset, uint dlsize,
            NitroTexture oldTex, Vector2 tcscale, bool save)
        {
            Vector2 texsize = new Vector2(oldTex.m_Width, oldTex.m_Height);

            float largesttc = 0f;

            uint dlend = dloffset + dlsize;
            for (uint pos = dloffset; pos < dlend;)
            {
                byte[] cmds = m_Model.m_File.ReadBlock(pos, 4);
                pos += 4;

                foreach (byte cmd in cmds)
                {
                    if (cmd == 0x22)
                    {
                        uint param = m_Model.m_File.Read32(pos);
                        short s = (short)(param & 0xFFFF);
                        short t = (short)(param >> 16);
                        Vector2 texCoord = new Vector2(s / 16.0f, t / 16.0f);
                        texCoord = Vector2.Add(texCoord, mat.m_TexCoordTrans);
                        texCoord = Vector2.Multiply(texCoord, mat.m_TexCoordScale);

                        texCoord = Vector2.Divide(texCoord, texsize);
                        texCoord = Vector2.Multiply(texCoord, tcscale);

                        if (Math.Abs(texCoord.X) > largesttc) largesttc = Math.Abs(texCoord.X);
                        if (Math.Abs(texCoord.Y) > largesttc) largesttc = Math.Abs(texCoord.Y);

                        if (save)
                        {
                            s = (short)(texCoord.X * 16);
                            t = (short)(texCoord.Y * 16);
                            param = ((uint)t << 16) | (ushort)s;
                            m_Model.m_File.Write32(pos, param);
                        }
                    }
                    pos += (uint)GX3D.GX_CMD_LENGTH[cmd];
                }
            }

            return largesttc;
        }

        private struct MatGroupTexCoordSettings
        {
            public uint m_TexParams;
            public Vector2 m_TexCoordScale;
            public Vector2 m_TexCoordTrans;

            public byte TexGenMode
            {
                get { return (byte)(m_TexParams >> 30); }
            }
        }

        private MatGroupTexCoordSettings ReadMatGroupTexCoordSettings(uint matOffset)
        {
            MatGroupTexCoordSettings mat = new MatGroupTexCoordSettings();
            mat.m_TexParams = m_Model.m_File.Read32(matOffset + 0x20);
            if (mat.TexGenMode == 0)
            {
                mat.m_TexCoordScale = new Vector2(1.0f, 1.0f);
                mat.m_TexCoordTrans = new Vector2(0.0f, 0.0f);
            }
            else
            {
                mat.m_TexCoordScale = new Vector2(m_Model.m_File.Read32(matOffset + 0x0C) / 4096f, m_Model.m_File.Read32(matOffset + 0x10) / 4096f);
                mat.m_TexCoordTrans = new Vector2(m_Model.m_File.Read32(matOffset + 0x18) / 4096f, m_Model.m_File.Read32(matOffset + 0x1C) / 4096f);
            }
            return mat;
        }

        private void WriteMatGroupTexCoordSettings(uint matOffset, MatGroupTexCoordSettings mat)
        {
            m_Model.m_File.Write32(matOffset + 0x20, mat.m_TexParams);
            m_Model.m_File.Write32(matOffset + 0x0C, (uint)(mat.m_TexCoordScale.X * 4096));
            m_Model.m_File.Write32(matOffset + 0x10, (uint)(mat.m_TexCoordScale.Y * 4096));
            m_Model.m_File.Write32(matOffset + 0x18, (uint)(mat.m_TexCoordTrans.X * 4096));
            m_Model.m_File.Write32(matOffset + 0x1C, (uint)(mat.m_TexCoordTrans.Y * 4096));
        }

        private static int BestTexTypeForBitmap(Bitmap bmp, int srcTexType, bool compress = true)
        {
            if (srcTexType == 7)
            {
                return 7;
            }

            bool alpha = NitroTexture.BitmapUsesTranslucency(bmp);
            int nColours = NitroTexture.CountColoursInBitmap(bmp);

            if (alpha)
            {
                return ((nColours <= 8) ? 6 : 1);
            }
            else
            {
                if (compress) return 5;
                else if (nColours <= 4) return 2;
                else if (nColours <= 16) return 3;
                else if (nColours <= 256) return 4;
                else return 7;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            m_Model.m_File.SaveChanges();
        }

        private void btnLoadBTP_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a BTP file to load", new String[] { ".btp" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                ClearBTPTextBoxes();
                LoadBTP(m_ROMFileSelect.m_SelectedFile);
            }
        }

        private void LoadBTP(String filename)
        {
            try
            {
                NitroFile file = Program.m_ROM.GetFileFromName(filename);
                m_BTP = new BTP(file);

                LoadOnlyBTPReferencedTextures();

                PopulateBTPListBoxes();

                EnableBTPFormControls();
            }
            catch (Exception ex) { MessageBox.Show("Error loading BTP:\n" + ex.Message + "\n" + ex.StackTrace); }
        }

        private void PopulateBTPListBoxes()
        {
            lbxBTPFrames.Items.Clear();
            lbxBTPMaterials.Items.Clear();

            for (int i = 0; i < m_BTP.NumFrames(); i++)
            {
                lbxBTPFrames.Items.Add(String.Format("{0:D3}", i));
            }

            for (int i = 0; i < m_BTP.m_MaterialData.Keys.Count; i++)
            {
                lbxBTPMaterials.Items.Add(m_BTP.m_MaterialData.Keys.ElementAt(i));
            }

            if (lbxBTPMaterials.Items.Count > 0)
                lbxBTPMaterials.SelectedIndex = 0;
        }

        private void EnableBTPFormControls()
        {
            rbTexAsRefInBTP.Enabled = true; rbTexAsRefInBTP.Checked = true;

            btnBTPAddTexture.Enabled = true; btnBTPAddTexture.Visible = true;
            btnBTPRemoveTexture.Enabled = true; btnBTPRemoveTexture.Visible = true;
            btnBTPAddPalette.Enabled = true; btnBTPAddPalette.Visible = true;
            btnBTPRemovePalette.Enabled = true; btnBTPRemovePalette.Visible = true;
            btnBTPRenameTexture.Enabled = true; btnBTPRenameTexture.Visible = true;
            btnBTPRenamePalette.Enabled = true; btnBTPRenamePalette.Visible = true;

            txtBTPTextureName.Enabled = true; txtBTPTextureName.Visible = true;
            txtBTPPaletteName.Enabled = true; txtBTPPaletteName.Visible = true;
        }

        private void DisableBTPFormControls()
        {
            btnBTPAddTexture.Enabled = false; btnBTPAddTexture.Visible = false;
            btnBTPRemoveTexture.Enabled = false; btnBTPRemoveTexture.Visible = false;
            btnBTPAddPalette.Enabled = false; btnBTPAddPalette.Visible = false;
            btnBTPRemovePalette.Enabled = false; btnBTPRemovePalette.Visible = false;
            btnBTPRenameTexture.Enabled = false; btnBTPRenameTexture.Visible = false;
            btnBTPRenamePalette.Enabled = false; btnBTPRenamePalette.Visible = false;

            txtBTPTextureName.Enabled = false; txtBTPTextureName.Visible = false;
            txtBTPPaletteName.Enabled = false; txtBTPPaletteName.Visible = false;
        }

        private void m_BTPTimer_Tick(object sender, EventArgs e)
        {
            string matName = lbxBTPMaterials.Items[lbxBTPMaterials.SelectedIndex].ToString();

            if (timerCount >= m_BTP.m_MaterialData[matName].m_NumFrames)
            {
                StopTimer();
                return;
            }

            for (int i = m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges; i <
                m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges + m_BTP.m_MaterialData[matName].m_NumFrameChanges; i++)
            {
                if (timerCount == m_BTP.m_Frames[i].m_FrameNum)
                {
                    lbxBTPFrames.SelectedIndex = m_BTP.m_Frames[i].m_FrameChangeID;
                    break;
                }
            }

            timerCount++;
        }

        private void ClearBTPTextBoxes()
        {
            txtBTPFrameLength.Text = "";
            txtBTPFramePalID.Text = "";
            txtBTPFrameTexID.Text = "";
            txtBTPMatNumFrameChanges.Text = "";
            txtBTPMatStartOffsetFrameChanges.Text = "";
            lbxBTPFrames.Items.Clear();
            lbxBTPMaterials.Items.Clear();
        }

        private void lbxBTPMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;

            if (index == -1)
                return;

            string matName = lbxBTPMaterials.Items[index].ToString();

            txtBTPMatNumFrameChanges.Text = m_BTP.m_MaterialData[matName].m_NumFrameChanges.ToString();
            txtBTPMatStartOffsetFrameChanges.Text = m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges.ToString();
            txtBTPMaterialName.Text = matName;
        }

        private void lbxBTPFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;

            if (index == -1 || index >= lbxBTPFrames.Items.Count)
                return;

            for (int i = 0; i < m_BTP.m_MaterialData.Values.Count; i++)
            {
                BTP.BTPMaterialData matData = m_BTP.m_MaterialData.Values.ElementAt(i);
                if (index >= matData.m_StartOffsetFrameChanges &&
                    index < matData.m_StartOffsetFrameChanges + matData.m_NumFrameChanges)
                {
                    lbxBTPMaterials.SelectedIndex = i;
                }
            }

            txtBTPFrameTexID.Text = m_BTP.m_Frames[index].m_TextureID.ToString();
            txtBTPFramePalID.Text = m_BTP.m_Frames[index].m_PaletteID.ToString();
            txtBTPFrameLength.Text = m_BTP.m_Frames[index].m_Length.ToString();

            if (m_BTP.m_Frames[index].m_TextureID >= 0 && m_BTP.m_Frames[index].m_TextureID < lbxTextures.Items.Count)
                lbxTextures.SelectedIndex = (int)m_BTP.m_Frames[index].m_TextureID;
            if (m_BTP.m_Frames[index].m_PaletteID >= 0 && m_BTP.m_Frames[index].m_PaletteID < lbxPalettes.Items.Count)
                lbxPalettes.SelectedIndex = (int)m_BTP.m_Frames[index].m_PaletteID;
        }

        private void btnMatPreview_Click(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;

            if (lbxBTPMaterials.Items.Count <= 0)
                return;
            if (!(index >= 0 && index < m_BTP.m_MaterialData.Count))
                lbxBTPMaterials.SelectedIndex = index = 0;

            string matName = lbxBTPMaterials.Items[index].ToString();
            if (m_BTP.m_MaterialData[matName].m_NumFrameChanges > 0 &&
                m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges < m_BTP.NumFrames())
            {
                StartTimer();
            }
        }

        private void rbTexAllInBMD_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTexAllInBMD.Checked)
            {
                rbTexAsRefInBTP.Checked = false;
                DisableBTPFormControls();
                LoadTextures();
            }
        }

        private void rbTexAsRefInBTP_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTexAsRefInBTP.Checked)
            {
                rbTexAllInBMD.Checked = false;
                EnableBTPFormControls();
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void LoadOnlyBTPReferencedTextures()
        {
            /* This is necessary as the texture and palette ID's in the BTP file don't reference the 
             * texture and palette ID's within the BMD model - they reference the ones named in the BTP file
            */

            lbxTextures.Items.Clear();
            lbxPalettes.Items.Clear();

            for (int i = 0; i < m_BTP.m_TextureNames.Count; i++)
            {
                lbxTextures.Items.Add(m_BTP.m_TextureNames[i]);
            }

            for (int i = 0; i < m_BTP.m_PaletteNames.Count; i++)
            {
                lbxPalettes.Items.Add(m_BTP.m_PaletteNames[i]);
            }
        }

        private void btnBTPAddTexture_Click(object sender, EventArgs e)
        {
            string newTexName = txtBTPTextureName.Text;
            if (newTexName.Length > 0)
            {
                m_BTP.AddTexture(newTexName);
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPAddPalette_Click(object sender, EventArgs e)
        {
            string newPalName = txtBTPPaletteName.Text;
            if (newPalName.Length > 0)
            {
                m_BTP.AddPalette(newPalName);
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPRemoveTexture_Click(object sender, EventArgs e)
        {
            int index = lbxTextures.SelectedIndex;
            if (index >= 0 && index < lbxTextures.Items.Count)
            {
                m_BTP.RemoveTexture(lbxTextures.Items[index].ToString());
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPRemovePalette_Click(object sender, EventArgs e)
        {
            int index = lbxPalettes.SelectedIndex;
            if (index >= 0 && index < lbxPalettes.Items.Count)
            {
                m_BTP.RemovePalette(lbxPalettes.Items[index].ToString());
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPAddFrame_Click(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (txtBTPFrameLength.Text.Equals("") || txtBTPFrameTexID.Text.Equals("") || txtBTPFramePalID.Text.Equals(""))
            {
                MessageBox.Show("Invalid frame values entered.");
                return;
            }
            try
            {
                uint textureID = uint.Parse(txtBTPFrameTexID.Text);
                uint paletteID = uint.Parse(txtBTPFramePalID.Text);
                int length = int.Parse(txtBTPFrameLength.Text);

                m_BTP.AddFrame(textureID, paletteID, length, (index != -1) ? index + 1 : index);

                LoadOnlyBTPReferencedTextures();
                PopulateBTPListBoxes();
            }
            catch { MessageBox.Show("Invalid frame values entered."); }
        }

        private void btnBTPRemoveFrame_Click(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0)
            {
                m_BTP.RemoveFrame(index);
                LoadOnlyBTPReferencedTextures();
                PopulateBTPListBoxes();
                if (index < m_BTP.m_Frames.Count)
                    lbxBTPFrames.SelectedIndex = index;
                else
                    lbxBTPFrames.SelectedIndex = m_BTP.m_Frames.Count - 1;
            }
        }

        private void btnBTPAddMaterial_Click(object sender, EventArgs e)
        {
            string matName = txtBTPMaterialName.Text;
            if (!matName.Equals(""))
            {
                try
                {
                    ushort numFrameChanges = ushort.Parse(txtBTPMatNumFrameChanges.Text);
                    ushort startOffsetFrameChanges = ushort.Parse(txtBTPMatStartOffsetFrameChanges.Text);

                    m_BTP.AddMaterial(matName, numFrameChanges, startOffsetFrameChanges);

                    PopulateBTPListBoxes();
                }
                catch { MessageBox.Show("Invalid material data entered."); }
            }
        }

        private void btnBTPRemoveMaterial_Click(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;
            if (index != -1)
            {
                m_BTP.RemoveMaterial(lbxBTPMaterials.Items[index].ToString());
                PopulateBTPListBoxes();
                if (index < m_BTP.m_MaterialData.Count)
                    lbxBTPMaterials.SelectedIndex = index;
                else
                    lbxBTPMaterials.SelectedIndex = m_BTP.m_MaterialData.Count - 1;
            }
        }

        private void btnMatPreviewStop_Click(object sender, EventArgs e)
        {
            StopTimer();
        }

        private void btnSaveBTP_Click(object sender, EventArgs e)
        {
            try
            {
                m_BTP.SaveChanges();
                LoadBTP(m_BTP.m_FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when trying to save the BTP file: " + ex.Message + "\n\n" +
                    ex.StackTrace);
            }
        }

        private void txtBTPMatStartOffsetFrameChanges_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;
            if (index >= 0 && !txtBTPMatStartOffsetFrameChanges.Text.Equals(""))
            {
                try
                {
                    string matName = lbxBTPMaterials.Items[index].ToString();
                    ushort startOffsetFrameChanges = ushort.Parse(txtBTPMatStartOffsetFrameChanges.Text);
                    m_BTP.SetMaterialStartOffsetFrameChanges(matName, startOffsetFrameChanges);
                }
                catch { }
            }
        }

        private void txtBTPMatNumFrameChanges_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;
            if (index >= 0 && !txtBTPMatNumFrameChanges.Text.Equals(""))
            {
                try
                {
                    string matName = lbxBTPMaterials.Items[index].ToString();
                    ushort numFrameChanges = ushort.Parse(txtBTPMatNumFrameChanges.Text);
                    m_BTP.SetMaterialNumFrameChanges(matName, numFrameChanges);
                }
                catch { }
            }
        }

        private void txtBTPFrameTexID_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0 && !txtBTPFrameTexID.Text.Equals(""))
            {
                try
                {
                    uint textureID = uint.Parse(txtBTPFrameTexID.Text);
                    m_BTP.m_Frames[index].m_TextureID = textureID;
                }
                catch { }
            }
        }

        private void txtBTPFramePalID_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0 && !txtBTPFramePalID.Text.Equals(""))
            {
                try
                {
                    uint paletteID = uint.Parse(txtBTPFramePalID.Text);
                    m_BTP.m_Frames[index].m_PaletteID = paletteID;
                }
                catch { }
            }
        }

        private void txtBTPFrameLength_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0 && !txtBTPFrameLength.Text.Equals(""))
            {
                try
                {
                    int length = int.Parse(txtBTPFrameLength.Text);
                    m_BTP.m_Frames[index].m_Length = length;
                }
                catch { }
            }
        }

        private void btnBTPRenameTexture_Click(object sender, EventArgs e)
        {
            int index = lbxTextures.SelectedIndex;
            string newName = txtBTPTextureName.Text;
            if (index >= 0 && !newName.Equals(""))
            {
                m_BTP.m_TextureNames[index] = newName;
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPRenamePalette_Click(object sender, EventArgs e)
        {
            int index = lbxPalettes.SelectedIndex;
            string newName = txtBTPPaletteName.Text;
            if (index >= 0 && !newName.Equals(""))
            {
                m_BTP.m_PaletteNames[index] = newName;
                LoadOnlyBTPReferencedTextures();
            }
        }

    }
}
