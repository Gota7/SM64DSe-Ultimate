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
    public partial class ParticleEdtorForm : Form
    {
        private int previousFormatIndex;
        bool checkboxesBeingUpdated = false;

        enum CompressionType
        {
            A3I5 = 1,
            COLOR_4,
            COLOR_16,
            COLOR_256,
            TEXEL_4x4,
            A5I3,
            DIRECT,

            NUM_COMPRESSION_TYPES
        }

        private TPS p_ParticleTex;
        private NitroFile p_ParticleTexFile;
        private string p_Name;

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();
        private FolderBrowserDialog m_FolderBrowserDialogue = new FolderBrowserDialog();

        public ParticleEdtorForm(string fileName)
        {
            InitializeComponent();

            p_Name = fileName;
        }

        public ParticleEdtorForm()
            : this(null) { }

        private void TextureEditorForm_Load(object sender, System.EventArgs e)
        {
            if (p_Name == null)
            {
                m_ROMFileSelect.ReInitialize("Select a TPS file to load", new String[] { ".tps" });
                DialogResult result = m_ROMFileSelect.ShowDialog();
                if (result != DialogResult.OK)
                {
                    Close();
                }
                else
                {
                    p_Name = m_ROMFileSelect.m_SelectedFile;
                    p_ParticleTexFile = Program.m_ROM.GetFileFromName(p_Name);
                    p_ParticleTex = new TPS(p_ParticleTexFile);

                    cmbFormat.Items.Add("A3I5");
                    cmbFormat.Items.Add("Color4");
                    cmbFormat.Items.Add("Color16");
                    cmbFormat.Items.Add("Color256");
                    cmbFormat.Items.Add("Texel4x4");
                    cmbFormat.Items.Add("A5I3");
                    cmbFormat.Items.Add("Direct");
                    cmbFormat.SelectedIndex = previousFormatIndex = (int)(p_ParticleTex.p_flags & 0x7) - 1;
                    RefreshImage();
                    PopulatePaletteSettings();
                    UpdateForm();
                    ResetColourButtonValue(btnModelPalettesSelectedColour);
                }
            }
        }

        public void UpdateForm()
        {
            checkboxesBeingUpdated = true;
            chkRepeatS.Checked = p_ParticleTex.GetRepeatSFlag();
            chkRepeatT.Checked = p_ParticleTex.GetRepeatTFlag();
            chkFlipS.Checked = p_ParticleTex.GetFlipSFlag();
            chkFlipT.Checked = p_ParticleTex.GetFlipTFlag();
            chkTransparent.Checked = p_ParticleTex.GetTransparentFlag();
            checkboxesBeingUpdated = false;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog export = new SaveFileDialog();
            int i = p_Name.LastIndexOf('/');
            export.FileName = p_Name.Substring(++i, p_Name.Length - i).Replace(".tps", ".png");//Default name
            export.DefaultExt = ".png";//Because most particle textures have transparency
            export.Filter = "Image Files (*.bmp,*.png) | *.bmp;*.png";
            if (export.ShowDialog() == DialogResult.Cancel)
                return;

            string extension = export.FileName.Substring(export.FileName.Length - 4, 4);
            if (extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                SaveTextureAsPNG(p_ParticleTex, export.FileName);
            else
                SaveTextureAsBMP(p_ParticleTex, export.FileName);
        }

        private static void SaveTextureAsBMP(TPS currentTexture, String fileName)
        {
            try
            {
                currentTexture.ArgbToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save the texture.\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private static void SaveTextureAsPNG(TPS currentTexture, String fileName)
        {
            try
            {
                currentTexture.ArgbToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save the texture.\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            p_ParticleTex.SaveTPS();
        }

        private void btnLoadTPS_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a TPS file to load", new String[] { ".tps" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            else
            {
                p_Name = m_ROMFileSelect.m_SelectedFile;
                p_ParticleTexFile = Program.m_ROM.GetFileFromName(p_Name);
                p_ParticleTex = new TPS(p_ParticleTexFile);

                cmbFormat.SelectedIndex = previousFormatIndex = (int)(p_ParticleTex.p_flags & 0x7) - 1;
                RefreshImage();
                PopulatePaletteSettings();
                UpdateForm();
            }
        }

        private void btnImportTexture_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an image";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel) return;
            Bitmap bmp = new Bitmap(ofd.FileName);

            try
            {
                p_ParticleTex.FromBMP(bmp, cmbFormat.SelectedIndex + 1);
                PopulatePaletteSettings();

                if (chkFlipS.Checked)
                    p_ParticleTex.SetFlipSFlag();
                if (chkFlipT.Checked)
                    p_ParticleTex.SetFlipTFlag();
                if (chkRepeatS.Checked)
                    p_ParticleTex.SetRepeatSFlag();
                if (chkRepeatT.Checked)
                    p_ParticleTex.SetRepeatTFlag();
                if (chkTransparent.Checked)
                    p_ParticleTex.SetTransparentFlag();

                RefreshImage();

                UpdateForm();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Invalid format.");
            }
            catch (IndexOutOfRangeException indEx)
            {
                MessageBox.Show(indEx.Message, "Something went wrong.");
            }
        }

        private void chkRepeatS_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkboxesBeingUpdated)
               p_ParticleTex.SetRepeatSFlag();
        }

        private void chkRepeatT_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkboxesBeingUpdated)
                p_ParticleTex.SetRepeatTFlag();
        }

        private void chkFlipS_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkboxesBeingUpdated)
                p_ParticleTex.SetFlipSFlag();
        }

        private void chkFlipT_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkboxesBeingUpdated)
                p_ParticleTex.SetFlipTFlag();
        }

        private void chkTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkboxesBeingUpdated)
            {
                p_ParticleTex.SetTransparentFlag();
                RefreshImage();
            }
        }

        private void cmbCompression_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFormat.SelectedIndex + 1 == 5)
            {
                MessageBox.Show("The 4x4 texel format is not currently supported, please select a different format.", "Format not supported", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbFormat.SelectedIndex = previousFormatIndex;
            }

            previousFormatIndex = cmbFormat.SelectedIndex;
        }

        public void RefreshImage()
        {
            p_ParticleTex.ArgbFromData();
            pbxTexture.Image = p_ParticleTex.ArgbToBitmap();
            pbxTexture.Refresh();
        }

        private void pbxTexture_Click(object sender, EventArgs e)
        {

        }

        private void btnModelPalettesSelectedColour_Click(object sender, EventArgs e)
        {
            if (!grdPalette.IsAColourSelected()) return;

            Color? selectedColour = GetColourDialogueResult(btnModelPalettesSelectedColour);
            if (selectedColour == null) return;

            ushort colourBGR15 = Helper.ColorToBGR15((Color)selectedColour);
            int selectedColourIndex = grdPalette.GetSelectedColourIndex();

            DataHelper.Write16(p_ParticleTex.p_RawPaletteData, (uint)(selectedColourIndex * 2), colourBGR15);
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
            byte[] palette = p_ParticleTex.p_RawPaletteData;

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

        private void btnChangeFormat_Click(object sender, EventArgs e)
        {
            /*try
            {
                p_ParticleTex.ChangeFormat((uint)(cmbFormat.SelectedIndex + 1));
                p_ParticleTex.DataFromBitmap(p_ParticleTex.ArgbToBitmap()); // update the raw image to the new format
                RefreshImage();
                PopulatePaletteSettings();
                lblDebug.Text = p_ParticleTex.ToString();
            }
            catch(ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Invalid format.");
            }*/
        }
    }
}
