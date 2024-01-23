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

namespace SM64DSe
{
    public partial class ParticleTextureForm : Form
    {
        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();
        private List<Particle.System.Def> m_SysDefs;
        private List<Particle.Texture> m_TexDefs;
        private int internalTexs;
        private int totalTexs;
        private int prevFormat;

        public ParticleTextureForm(List<Particle.System.Def> sysDefs, List<Particle.Texture> textures, int internalTexs, int totalTexs)
        {
            InitializeComponent();
            m_SysDefs = sysDefs;
            m_TexDefs = textures;
            this.internalTexs = internalTexs;
            this.totalTexs = totalTexs;
        }

        private void ParticleTextureForm_Load(object sender, EventArgs e)
        {
            cmbRepeatX.Items.Add(Particle.Texture.RepeatMode.CLAMP);
            cmbRepeatX.Items.Add(Particle.Texture.RepeatMode.REPEAT);
            cmbRepeatX.Items.Add(Particle.Texture.RepeatMode.FLIP);
            cmbRepeatX.SelectedIndex = -1;

            cmbRepeatY.Items.Add(Particle.Texture.RepeatMode.CLAMP);
            cmbRepeatY.Items.Add(Particle.Texture.RepeatMode.REPEAT);
            cmbRepeatY.Items.Add(Particle.Texture.RepeatMode.FLIP);
            cmbRepeatY.SelectedIndex = -1;

            cmbFormat.Items.Add("A3I5");
            cmbFormat.Items.Add("Color4");
            cmbFormat.Items.Add("Color16");
            cmbFormat.Items.Add("Color256");
            cmbFormat.Items.Add("Texel4x4 (unsupported)");
            cmbFormat.Items.Add("A5I3");
            cmbFormat.Items.Add("Direct");
            cmbFormat.SelectedIndex = prevFormat = -1;

            GenerateListBoxItems();
            UpdateToolBar();
        }

        private void ParticleTextureForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((ParticleViewerForm)Owner).TextureFormUpdate(totalTexs, internalTexs);
        }

        private void lbxTexDef_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshImage();
            PopulatePaletteSettings();
            UpdateToolBar();

            int texDefID = lbxTexDef.SelectedIndex;
            if (texDefID != -1)
            {
                cmbRepeatX.SelectedIndex = (int)m_TexDefs[texDefID].m_RepeatX;
                cmbRepeatY.SelectedIndex = (int)m_TexDefs[texDefID].m_RepeatY;
                cmbFormat.SelectedIndex = m_TexDefs[texDefID].m_Tex.m_TexType - 1;
                if (cmbFormat.SelectedIndex == 4)
                    cmbFormat.SelectedIndex = -1;
            }
        }

        private void cmbRepeatX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRepeatX.SelectedIndex == -1)
                return;

            int texDefID = lbxTexDef.SelectedIndex;
            if (texDefID == -1)
            {
                cmbRepeatX.SelectedIndex = -1;
                return;
            }

            m_TexDefs[texDefID].Unload();
            m_TexDefs[texDefID].m_RepeatX = (Particle.Texture.RepeatMode)cmbRepeatX.SelectedIndex;
            m_TexDefs[texDefID].Load();
        }

        private void cmbRepeatY_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRepeatY.SelectedIndex == -1)
                return;

            int texDefID = lbxTexDef.SelectedIndex;
            if (texDefID == -1)
            {
                cmbRepeatY.SelectedIndex = -1;
                return;
            }

            m_TexDefs[texDefID].Unload();
            m_TexDefs[texDefID].m_RepeatY = (Particle.Texture.RepeatMode)cmbRepeatY.SelectedIndex;
            m_TexDefs[texDefID].Load();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdPalette_CurrentCellChanged(object sender, EventArgs e)
        {
            if (!grdPalette.IsAColourSelected())
            {
                ResetColourButtonValue(btnModelPalettesSelectedColour);
                return;
            }

            Color colour = (Color)grdPalette.GetSelectedColour();
            SetColourButtonValue(btnModelPalettesSelectedColour, colour);
        }

        private void btnModelPalettesSelectedColour_Click(object sender, EventArgs e)
        {
            if (!grdPalette.IsAColourSelected()) return;

            Color? selectedColour = GetColourDialogueResult(btnModelPalettesSelectedColour);
            if (selectedColour == null) return;

            ushort colourBGR15 = Helper.ColorToBGR15((Color)selectedColour);
            int selectedColourIndex = grdPalette.GetSelectedColourIndex();

            DataHelper.Write16(m_TexDefs[lbxTexDef.SelectedIndex].m_Tex.m_RawPaletteData, (uint)(selectedColourIndex * 2), colourBGR15);
            grdPalette.SetColourAtIndex((Color)selectedColour, selectedColourIndex);

            RefreshImage();
        }

        private void internalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_TexDefs.Insert(internalTexs, new Particle.Texture());
            totalTexs++;
            internalTexs++;

            UpdateSpriteIDs();
            GenerateListBoxItems();
        }

        private void externalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_TexDefs.Add(new Particle.Texture());
            totalTexs++;

            GenerateListBoxItems();
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxTexDef.SelectedIndex != -1)
            {
                if (CanTextureBeRemoved(lbxTexDef.SelectedIndex))
                {
                    m_TexDefs.RemoveAt(lbxTexDef.SelectedIndex);
                    totalTexs--;
                    if (lbxTexDef.SelectedIndex < internalTexs)
                        internalTexs--;

                    UpdateSpriteIDs();
                    GenerateListBoxItems();
                }
                else
                {
                    // this should never happen, but you never know
                    MessageBox.Show("This texture is used by particle systems, remove all references to this texture to be able to remove it.", "Unable to remove texture");
                }
            }
            else
            {
                MessageBox.Show("Select a texture to remove.", "No texture selected");
            }
        }

        private void loadExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int texDefID = lbxTexDef.SelectedIndex;

            // this shouldn't happen but you never know
            if (texDefID == -1)
                return;

            m_ROMFileSelect.ReInitialize("Select a SPT file to load", new String[] { ".spt" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            else
            {
                try
                {
                    NitroFile SPT = Program.m_ROM.GetFileFromName(m_ROMFileSelect.m_SelectedFile);

                    if (SPT.Read32(0x0) != 0x53505420)
                        throw new Exception("Invalid SPT header.");

                    uint flags = SPT.Read32(0x04);
                    uint texelArrSize = SPT.Read32(0x08);
                    uint palOffset = SPT.Read32(0x0c);
                    uint palSize = SPT.Read32(0x10);
                    uint totalSize = SPT.Read32(0x1c);

                    byte[] texels = SPT.ReadBlock(0x20, texelArrSize);
                    byte[] palette = SPT.ReadBlock(palOffset, palSize);

                    int width = 1 << (((int)flags >> 4 & 0xf) + 3);
                    int height = 1 << (((int)flags >> 8 & 0xf) + 3);
                    bool color0Transp = ((flags & 0x8) | (flags & 0x10000)) != 0;
                    int type = (int)flags & 0x7;
                    Particle.Texture.RepeatMode repeatX = (flags & 0x4000) != 0 ?
                        Particle.Texture.RepeatMode.FLIP : (flags & 0x1000) != 0 ?
                        Particle.Texture.RepeatMode.REPEAT :
                        Particle.Texture.RepeatMode.CLAMP;
                    Particle.Texture.RepeatMode repeatY = (flags & 0x8000) != 0 ?
                        Particle.Texture.RepeatMode.FLIP : (flags & 0x2000) != 0 ?
                        Particle.Texture.RepeatMode.REPEAT :
                        Particle.Texture.RepeatMode.CLAMP;

                    m_TexDefs[texDefID].Unload();

                    m_TexDefs[texDefID] = new Particle.Texture(texels, palette, width, height,
                        (byte)(color0Transp ? 1 : 0), type, repeatX, repeatY, texDefID);

                    m_TexDefs[texDefID].Load();

                    UpdateParticleTextures(texDefID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load external SPT. Details:\n" + ex.Message, "Failed to load SPT");
                }

                RefreshImage();
                PopulatePaletteSettings();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // this shouldn't happen but you never know
            if (lbxTexDef.SelectedIndex == -1)
                return;

            SaveFileDialog export = new SaveFileDialog();
            export.FileName = $"particleTex{Convert.ToString(lbxTexDef.SelectedIndex, 16)}.png";//Default name
            export.DefaultExt = ".png";//Because most particle textures have transparency
            export.Filter = "Image Files (*.bmp,*.png) | *.bmp;*.png";
            if (export.ShowDialog() == DialogResult.Cancel)
                return;

            string extension = export.FileName.Substring(export.FileName.Length - 4, 4);
            if (extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                SaveTextureAsPNG(m_TexDefs[lbxTexDef.SelectedIndex], export.FileName);
            else
                SaveTextureAsBMP(m_TexDefs[lbxTexDef.SelectedIndex], export.FileName);
        }

        private void replaceSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int texDefID = lbxTexDef.SelectedIndex;

            // this shouldn't happen but you never know
            if (texDefID == -1 || cmbFormat.SelectedIndex == -1 || cmbFormat.SelectedIndex == 4 ||
                cmbRepeatX.SelectedIndex == -1 || cmbRepeatY.SelectedIndex == -1)
                return;

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select an image";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.Cancel) return;
                Bitmap bmp = new Bitmap(ofd.FileName);

                Particle.Texture oldTex = m_TexDefs[texDefID];

                m_TexDefs[texDefID].Unload();

                m_TexDefs[texDefID] = new Particle.Texture(bmp, cmbFormat.SelectedIndex + 1,
                    (Particle.Texture.RepeatMode)cmbRepeatX.SelectedIndex,
                    (Particle.Texture.RepeatMode)cmbRepeatY.SelectedIndex, lbxTexDef.SelectedIndex);

                m_TexDefs[texDefID].Load();

                /*for (int i = 0; i < oldTex.m_Tex.m_ARGB.Length; i++)
                {
                    Console.WriteLine($"o = {oldTex.m_Tex.m_ARGB[i]}, n = {m_TexDefs[texDefID].m_Tex.m_ARGB[i]}");
                }
                Console.WriteLine($"ow = {oldTex.m_Tex.m_Width}, nw = {m_TexDefs[texDefID].m_Tex.m_Width}");
                Console.WriteLine($"oh = {oldTex.m_Tex.m_Height}, nh = {m_TexDefs[texDefID].m_Tex.m_Height}");*/

                //Console.WriteLine($" raw: {m_TexDefs[texDefID].m_Tex.m_RawTextureData.Length}");
                //Console.WriteLine($"ARGB: {m_TexDefs[texDefID].m_Tex.m_ARGB.Length}");

                UpdateParticleTextures(texDefID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load image. Details:\n" + ex.Message, "Failed to image");
            }

            RefreshImage();
            PopulatePaletteSettings();
        }

        private void GenerateListBoxItems()
        {
            lbxTexDef.SelectedIndex = -1;
            lbxTexDef.Items.Clear();
            for (int i = 0; i < totalTexs; i++)
            {
                lbxTexDef.Items.Add($"particleTex{Convert.ToString(i, 16).PadLeft(2, '0')}.spt{(i >= internalTexs ? " (ext)" : "")}");
            }
        }

        private void UpdateSpriteIDs()
        {
            for (int i = 0; i < m_TexDefs.Count; i++)
            {
                m_TexDefs[i].m_SpriteID = i;
            }
            for (int i = 0; i < m_SysDefs.Count; i++)
            {
                ParticleSysDefProperties.GenerateProperties(m_SysDefs[i], m_TexDefs);
            }
        }

        private void UpdateToolBar()
        {
            if (lbxTexDef.SelectedIndex == -1)
            {
                replaceSelectedToolStripMenuItem.Visible =
                    loadExternalToolStripMenuItem.Visible =
                    exportToolStripMenuItem.Visible =
                    removeSelectedToolStripMenuItem.Visible =
                    cmbRepeatX.Visible =
                    lblRepeatX.Visible =
                    cmbRepeatY.Visible =
                    lblRepeatY.Visible =
                    cmbFormat.Visible =
                    lblFormat.Visible = false;

                return;
            }

            removeSelectedToolStripMenuItem.Enabled = CanTextureBeRemoved(lbxTexDef.SelectedIndex);

            if (lbxTexDef.SelectedIndex < internalTexs)
            {
                // internal
                replaceSelectedToolStripMenuItem.Visible = 
                    exportToolStripMenuItem.Visible =
                    removeSelectedToolStripMenuItem.Visible =
                    cmbRepeatX.Visible =
                    lblRepeatX.Visible =
                    cmbRepeatY.Visible =
                    lblRepeatY.Visible =
                    cmbFormat.Visible =
                    lblFormat.Visible = true;

                loadExternalToolStripMenuItem.Visible = false;
            }
            else
            {
                // external
                replaceSelectedToolStripMenuItem.Visible =
                    cmbRepeatX.Visible =
                    lblRepeatX.Visible =
                    cmbRepeatY.Visible =
                    lblRepeatY.Visible =
                    cmbFormat.Visible =
                    lblFormat.Visible = false;

                loadExternalToolStripMenuItem.Visible =
                    removeSelectedToolStripMenuItem.Visible =
                    exportToolStripMenuItem.Visible = true;
            }
        }

        private void RefreshImage()
        {
            if (lbxTexDef.SelectedIndex == -1)
                pbxTexture.Image = null;
            else
            {
                m_TexDefs[lbxTexDef.SelectedIndex].m_Tex.FromRaw(m_TexDefs[lbxTexDef.SelectedIndex].m_Tex.m_RawTextureData, m_TexDefs[lbxTexDef.SelectedIndex].m_Tex.m_RawPaletteData);
                pbxTexture.Image = m_TexDefs[lbxTexDef.SelectedIndex].m_Tex.ToBitmap();
            }

            pbxTexture.Refresh();
        }

        private void PopulatePaletteSettings()
        {
            int texDefID = lbxTexDef.SelectedIndex;
            if (texDefID == -1 || texDefID >= internalTexs)
            {
                grdPalette.ClearColours();
                return;
            }

            byte[] palette = m_TexDefs[texDefID].m_Tex.m_RawPaletteData;

            int nColours = palette.Length / 2;
            Color[] paletteColours = new Color[nColours];
            for (int i = 0; i < nColours; i++)
            {
                ushort palColour = (ushort)(palette[(i * 2)] | (palette[(i * 2) + 1] << 8));
                paletteColours[i] = Helper.BGR15ToColor(palColour);
            }

            grdPalette.SetColours(paletteColours);

            btnModelPalettesSelectedColour.Enabled = true;
        }

        private void ResetColourButtonValue(Button button)
        {
            button.Text = null;
            button.BackColor = Color.Transparent;
            button.ForeColor = Color.Black;
        }

        private void SetColourButtonValue(Button button, Color colour)
        {
            string hexColourString = Helper.GetHexColourString(colour);
            button.Text = hexColourString;
            button.BackColor = colour;
            float luma = 0.2126f * colour.R + 0.7152f * colour.G + 0.0722f * colour.B;
            if (luma < 50)
                button.ForeColor = Color.White;
            else
                button.ForeColor = Color.Black;
        }

        private Color? GetColourDialogueResult(Button button)
        {
            ColorDialog colourDialogue = new ColorDialog();
            DialogResult result = colourDialogue.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                Color colour = colourDialogue.Color;
                SetColourButtonValue(button, colour);
                return colour;
            }
            return null;
        }

        bool CanTextureBeRemoved(int textureId)
        {
            for (int i = 0; i < m_SysDefs.Count; i++)
            {
                Particle.System.Def sysDef = m_SysDefs[i];

                // main texture
                Particle.MainInfo info = sysDef.m_MainInfo;
                if (m_TexDefs.IndexOf(info.m_Sprite) == textureId)
                    return false;

                // texseq textures
                if (sysDef.m_TexSeq != null)
                {
                    Particle.TextureSequence texSeq = sysDef.m_TexSeq;
                    for (int j = 0; j < texSeq.m_Sprites.Length; j++)
                    {
                        if (texSeq.m_Sprites[j] != null && m_TexDefs.IndexOf(texSeq.m_Sprites[j]) == textureId)
                            return false;
                    }
                }

                // glitter texture
                if (sysDef.m_Glitter != null)
                {
                    Particle.Glitter glitter = sysDef.m_Glitter;
                    if (m_TexDefs.IndexOf(glitter.m_Sprite) == textureId)
                        return false;
                }
            }

            return true;
        }

        private void UpdateParticleTextures(int textureId)
        {
            // m_SpriteID contains the previous id, update all of the textures that are still using it
            Particle.Texture newTexture = m_TexDefs[textureId];

            for (int i = 0; i < m_SysDefs.Count; i++)
            {
                Particle.System.Def sysDef = m_SysDefs[i];

                // main texture
                Particle.MainInfo info = sysDef.m_MainInfo;
                if (info.m_Sprite.m_SpriteID == textureId)
                    info.m_Sprite = newTexture;

                // texseq textures
                if (sysDef.m_TexSeq != null)
                {
                    Particle.TextureSequence texSeq = sysDef.m_TexSeq;
                    for (int j = 0; j < texSeq.m_Sprites.Length; j++)
                    {
                        if (texSeq.m_Sprites[j] != null && texSeq.m_Sprites[j].m_SpriteID == textureId)
                            texSeq.m_Sprites[j] = newTexture;
                    }
                }

                // glitter texture
                if (sysDef.m_Glitter != null)
                {
                    Particle.Glitter glitter = sysDef.m_Glitter;
                    if (glitter.m_Sprite.m_SpriteID == textureId)
                        glitter.m_Sprite = newTexture;
                }
            }
        }

        private static void SaveTextureAsBMP(Particle.Texture currentTexture, string fileName)
        {
            try
            {
                currentTexture.m_Tex.ToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save the texture.\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private static void SaveTextureAsPNG(Particle.Texture currentTexture, string fileName)
        {
            try
            {
                currentTexture.m_Tex.ToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save the texture.\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private void cmbFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFormat.SelectedIndex == -1)
                return;

            if (cmbFormat.SelectedIndex == 4)
            {
                MessageBox.Show("The Texel4x4 format is not supported by particles.", "Texture format not supported.");
                cmbFormat.SelectedIndex = prevFormat;
                return;
            }

            prevFormat = cmbFormat.SelectedIndex;
        }
    }
}
