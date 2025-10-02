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
using System.Numerics;
using SM64DSe.SM64DSFormats;

namespace SM64DSe
{
    public partial class PlanetCameraSettingsForm : Form
    {
        string[] columns = new string[8]
        {
            "Max Altitude Difference",
            "Min Altitude Difference",
            "Max Horizontal Distance",
            "Target Altitude Range",
            "Target Vertical Offset",
            "Altitude Adjustment Speed",
            "Frames to Transition",
            "Activate in Air",
        };

        PlanetCamSettings m_PlanetCamSettings;

        public PlanetCameraSettingsForm(PlanetCamSettings settings)
        {
            InitializeComponent();

            m_PlanetCamSettings = settings;

            List<string> levelNames = Strings.LevelNames();
            for (int i = 0; i < 52; i++)
                cbxLevels.Items.Add(i + " - " + levelNames[i]);

            FillDataGrid();
        }
        
        private void FillDataGrid()
        {
            txtNumEntries.Text = m_PlanetCamSettings.Count.ToString();

            dataGrid.RowCount = m_PlanetCamSettings.Count;

            if (dataGrid.ColumnCount < columns.Length)
            {
                dataGrid.ColumnCount = columns.Length - 1;

                var checkBoxColumn = new DataGridViewCheckBoxColumn();
                checkBoxColumn.TrueValue = true;
                checkBoxColumn.FalseValue = false;
                checkBoxColumn.ThreeState = false;

                dataGrid.Columns.Add(checkBoxColumn);

                dataGrid.RowHeadersWidth = 60;
                for (int i = 0; i < columns.Length; i++)
                {
                    dataGrid.Columns[i].Width = 80;
                    dataGrid.Columns[i].HeaderText = columns[i];
                }
            }

            for (int i = 0; i < m_PlanetCamSettings.Count; i++)
            {
                dataGrid.Rows[i].HeaderCell.Value = "" + i;

                for (int j = 0; j < 6; j++)
                {
                    short val = m_PlanetCamSettings[i].data[j];

                    dataGrid.Rows[i].Cells[j].Value = ShortToString(val);
                }

                dataGrid.Rows[i].Cells[6].Value = ((ushort)m_PlanetCamSettings[i].data[6]).ToString();
                dataGrid.Rows[i].Cells[7].Value = (m_PlanetCamSettings[i].data[7] & 1) != 0;
            }
        }

        static short ClampShort(BigInteger n)
        {
            if (n < short.MinValue) return short.MinValue;
            if (n > short.MaxValue) return short.MaxValue;
            return (short)n;
        }

        static ushort ClampUShort(BigInteger n)
        {
            if (n < 0) return 0;
            if (n > ushort.MaxValue) return ushort.MaxValue;
            return (ushort)n;
        }

        static short StringToShort(string s)
        {
            s = s.Trim();
            int pointIndex = s.IndexOf('.');

            if (pointIndex == -1) return ClampShort(BigInteger.Parse(s) * 1000);

            int numDecDigits = s.Length - pointIndex - 1;
            s = s.Remove(pointIndex, 1);

            if (numDecDigits > 4)
                s = s.Remove(s.Length - numDecDigits + 4);
            else if (numDecDigits < 4)
                s = s + new string('0', 4 - numDecDigits);

            var n = BigInteger.Parse(s);

            return ClampShort(n.Sign*((BigInteger.Abs(n) + 5) / 10));
        }

        static string ShortToString(short n)
        {
            string sign = n < 0 ? "-" : "";
            string s = Math.Abs((int)n).ToString();

            if (s.Length < 4) s = new string('0', 4 - s.Length) + s;

            return sign + s.Insert(s.Length - 3, ".");
        }

        void dataGrid_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == columns.Length) return;

                string s = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                if (e.ColumnIndex == 6)
                    m_PlanetCamSettings[e.RowIndex].data[e.ColumnIndex] = (short)ClampUShort(BigInteger.Parse(s));
                else if (e.ColumnIndex == 7)
                {
                    if (dataGrid.Rows[e.RowIndex].Cells[7].Value is bool b && b)
                        m_PlanetCamSettings[e.RowIndex].data[e.ColumnIndex] |= 1;
                    else
                        m_PlanetCamSettings[e.RowIndex].data[e.ColumnIndex] &= ~1;
                }
                else
                    m_PlanetCamSettings[e.RowIndex].data[e.ColumnIndex] = StringToShort(s);

                FillDataGrid();
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox(ex).ShowDialog();
                return;
            }
        }

        void dataGrid_CellContentClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 7)
                {
                    if (dataGrid.Rows[e.RowIndex].Cells[7].Value is bool b && b)
                        m_PlanetCamSettings[e.RowIndex].data[7] |= 1;
                    else
                        m_PlanetCamSettings[e.RowIndex].data[7] &= ~1;
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox(ex).ShowDialog();
                return;
            }
        }

        private void CopySettings(int sourceLevel)
        {
            NitroOverlay otherOVL = new NitroOverlay(Program.m_ROM, Program.m_ROM.GetLevelOverlayID(sourceLevel));

            m_PlanetCamSettings.Clear();
            m_PlanetCamSettings.Load(otherOVL);

            FillDataGrid();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (cbxLevels.SelectedIndex != -1)
                CopySettings(cbxLevels.SelectedIndex);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PlanetCamSettings.Entry entry = new PlanetCamSettings.Entry();
            entry.data = new short[8] { 1000, 250, 1000, 250, 0, 24, 30, 0};

            if (dataGrid.SelectedRows.Count == 0)
                m_PlanetCamSettings.Add(entry);
            else
                m_PlanetCamSettings.m_Entries.Insert(dataGrid.SelectedRows[0].Index, entry);

            FillDataGrid();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to delete.");
            else
            {
                m_PlanetCamSettings.m_Entries.RemoveAt(dataGrid.SelectedRows[0].Index);

                FillDataGrid();
            }
        }

        private void btnShiftUp_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (dataGrid.SelectedRows[0].Index != 0)
            {
                var temp = m_PlanetCamSettings[dataGrid.SelectedRows[0].Index];
                m_PlanetCamSettings[dataGrid.SelectedRows[0].Index] = m_PlanetCamSettings[dataGrid.SelectedRows[0].Index - 1];
                m_PlanetCamSettings[dataGrid.SelectedRows[0].Index - 1] = temp;
            }
            FillDataGrid();
        }

        private void btnShiftDown_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (dataGrid.SelectedRows[0].Index != dataGrid.Rows.Count - 1)
            {
                var temp = m_PlanetCamSettings[dataGrid.SelectedRows[0].Index];
                m_PlanetCamSettings[dataGrid.SelectedRows[0].Index] = m_PlanetCamSettings[dataGrid.SelectedRows[0].Index + 1];
                m_PlanetCamSettings[dataGrid.SelectedRows[0].Index + 1] = temp;
            }
            FillDataGrid();
        }
    }
}
