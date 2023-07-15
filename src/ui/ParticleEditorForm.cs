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

namespace SM64DSe
{
    public partial class ParticleEditorForm : Form
    {
        private Particle.Texture m_Texture;
        private NitroFile m_ParticleTexFile;
        private string m_Name;
        private int prevFormat;

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();
        private FolderBrowserDialog m_FolderBrowserDialogue = new FolderBrowserDialog();

        public ParticleEditorForm(string fileName)
        {
            InitializeComponent();

            m_Name = fileName;
        }

        public ParticleEditorForm()
            : this(null) { }

        private void TextureEditorForm_Load(object sender, System.EventArgs e)
        {
            if (m_Name == null)
            {
                m_ROMFileSelect.ReInitialize("Select a SPT file to load", new string[] { ".spt" });
                DialogResult result = m_ROMFileSelect.ShowDialog();
                if (result != DialogResult.OK)
                {
                    Close();
                    return;
                }

                m_Name = m_ROMFileSelect.m_SelectedFile;
                m_ParticleTexFile = Program.m_ROM.GetFileFromName(m_Name);

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

                LoadTexture();
                RefreshImage();
                PopulatePaletteSettings();
                UpdateForm();
                ResetColourButtonValue(btnModelPalettesSelectedColour);
            }
        }

        public void LoadTexture()
        {
            //Console.WriteLine($"offset = 0x{Convert.ToString(offset, 16)}");
            if (m_ParticleTexFile.Read32(0x0) != 0x53505420)
            {
                MessageBox.Show(string.Format("This SPT file is invalid."),
                    "Bad texture");
                Close();
                return;
            }

            uint flags = m_ParticleTexFile.Read32(0x04);
            uint texelArrSize = m_ParticleTexFile.Read32(0x08);
            uint palOffset = m_ParticleTexFile.Read32(0x0c);
            uint palSize = m_ParticleTexFile.Read32(0x10);
            uint totalSize = m_ParticleTexFile.Read32(0x1c);

            byte[] texels = m_ParticleTexFile.ReadBlock(0x20, texelArrSize);
            byte[] palette = m_ParticleTexFile.ReadBlock(palOffset, palSize);

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

            m_Texture = new Particle.Texture(texels, palette, width, height,
                (byte)(color0Transp ? 1 : 0), type, repeatX, repeatY, 0);
        }

        public void UpdateForm()
        {
            cmbRepeatX.SelectedIndex = (int)m_Texture.m_RepeatX;
            cmbRepeatY.SelectedIndex = (int)m_Texture.m_RepeatY;
            cmbFormat.SelectedIndex = m_Texture.m_Tex.m_TexType - 1;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog export = new SaveFileDialog();
            export.FileName = $"{m_Name}.png";//Default name
            export.DefaultExt = ".png";//Because most particle textures have transparency
            export.Filter = "Image Files (*.bmp,*.png) | *.bmp;*.png";
            if (export.ShowDialog() == DialogResult.Cancel)
                return;

            string extension = export.FileName.Substring(export.FileName.Length - 4, 4);
            if (extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                SaveTextureAsPNG(m_Texture, export.FileName);
            else
                SaveTextureAsBMP(m_Texture, export.FileName);
        }

        private static void SaveTextureAsBMP(Particle.Texture currentTexture, String fileName)
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

        private static void SaveTextureAsPNG(Particle.Texture currentTexture, String fileName)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            uint flags = (uint)m_Texture.m_Tex.m_TexType;
            flags |= ((uint)Math.Log(m_Texture.m_Tex.m_Width, 2) - 3) << 4;
            flags |= ((uint)Math.Log(m_Texture.m_Tex.m_Height, 2) - 3) << 8;
            flags |= (uint)(m_Texture.m_RepeatX == Particle.Texture.RepeatMode.REPEAT ? 0x1000 : m_Texture.m_RepeatX == Particle.Texture.RepeatMode.FLIP ? 0x5000 : 0);
            flags |= (uint)(m_Texture.m_RepeatY == Particle.Texture.RepeatMode.REPEAT ? 0x2000 : m_Texture.m_RepeatY == Particle.Texture.RepeatMode.FLIP ? 0xa000 : 0);
            flags |= (uint)(m_Texture.m_Tex.m_Colour0Mode != 0 ? 1 : 0) << 16;

            uint texelArrSize = (uint)m_Texture.m_Tex.m_RawTextureData.Length;
            uint palOffset = 0x20 + texelArrSize;
            uint palSize = (uint)m_Texture.m_Tex.m_RawPaletteData.Length;
            uint totalSize = palOffset + palSize;

            m_ParticleTexFile.Write32(0x0, 0x53505420); // "SPT " in ascii
            m_ParticleTexFile.Write32(0x04, flags); // flags

            m_ParticleTexFile.Write32(0x08, texelArrSize); // texelArrSize
            m_ParticleTexFile.Write32(0x0c, palOffset); // palOffset
            m_ParticleTexFile.Write32(0x10, palSize); // palSize
            m_ParticleTexFile.Write32(0x1c, totalSize); // totalSize

            m_ParticleTexFile.WriteBlock(0x20, m_Texture.m_Tex.m_RawTextureData);
            m_ParticleTexFile.WriteBlock(palOffset, m_Texture.m_Tex.m_RawPaletteData);

            // make it so the file is only as big as it needs to be
            if (m_ParticleTexFile.m_Data.Length - (int)totalSize > 0)
                m_ParticleTexFile.RemoveSpace(totalSize, (uint)m_ParticleTexFile.m_Data.Length - totalSize);

            m_ParticleTexFile.SaveChanges();
        }

        private void btnLoadSPT_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a SPT file to load", new String[] { ".spt" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            else
            {
                m_Name = m_ROMFileSelect.m_SelectedFile;
                m_ParticleTexFile = Program.m_ROM.GetFileFromName(m_Name);

                LoadTexture();
                RefreshImage();
                PopulatePaletteSettings();
                UpdateForm();
            }
        }

        private void btnImportTexture_Click(object sender, EventArgs e)
        {
            if (cmbFormat.SelectedIndex == -1 || cmbFormat.SelectedIndex == 4
                || cmbRepeatX.SelectedIndex == -1 || cmbRepeatY.SelectedIndex == -1)
                return;

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select an image";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.Cancel) return;
                Bitmap bmp = new Bitmap(ofd.FileName);

                m_Texture = new Particle.Texture(bmp, cmbFormat.SelectedIndex + 1,
                    (Particle.Texture.RepeatMode)cmbRepeatX.SelectedIndex,
                    (Particle.Texture.RepeatMode)cmbRepeatY.SelectedIndex, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load image. Details:\n" + ex.Message, "Failed to image");
            }

            RefreshImage();
            PopulatePaletteSettings();
        }

        public void RefreshImage()
        {
            m_Texture.m_Tex.FromRaw(m_Texture.m_Tex.m_RawTextureData, m_Texture.m_Tex.m_RawPaletteData);
            pbxTexture.Image = m_Texture.m_Tex.ToBitmap();
            pbxTexture.Refresh();
        }

        private void btnModelPalettesSelectedColour_Click(object sender, EventArgs e)
        {
            if (!grdPalette.IsAColourSelected()) return;

            Color? selectedColour = GetColourDialogueResult(btnModelPalettesSelectedColour);
            if (selectedColour == null) return;

            ushort colourBGR15 = Helper.ColorToBGR15((Color)selectedColour);
            int selectedColourIndex = grdPalette.GetSelectedColourIndex();

            DataHelper.Write16(m_Texture.m_Tex.m_RawPaletteData, (uint)(selectedColourIndex * 2), colourBGR15);
            grdPalette.SetColourAtIndex((Color)selectedColour, selectedColourIndex);

            RefreshImage();
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

        private void PopulatePaletteSettings()
        {
            byte[] palette = m_Texture.m_Tex.m_RawPaletteData;

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

        private void ResetColourButtonValue(Button button)
        {
            button.Text = null;
            button.BackColor = Color.Transparent;
            button.ForeColor = Color.Black;
        }

        private void cmbRepeatX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRepeatX.SelectedIndex != -1)
                m_Texture.m_RepeatX = (Particle.Texture.RepeatMode)cmbRepeatX.SelectedIndex;
        }

        private void cmbRepeatY_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRepeatY.SelectedIndex != -1)
                m_Texture.m_RepeatY = (Particle.Texture.RepeatMode)cmbRepeatY.SelectedIndex;
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
