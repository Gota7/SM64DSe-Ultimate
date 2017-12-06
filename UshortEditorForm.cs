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
    public partial class RawEditorForm : Form
    {

        LevelEditorForm m_parent;
        bool m_displayInBinary;

        public RawEditorForm(LevelEditorForm levelEditor)
        {
            m_parent = levelEditor;
            m_displayInBinary = false;
            InitializeComponent();
        }

        public void ShowForObject(LevelObject obj)
        {
            if (!Visible)
                Show(m_parent);
            else
                Activate();
            UpdateForObject(obj);
        }

        public void UpdateForObject(LevelObject obj)
        {
            panControls.Controls.Clear();
            Control ctrl;

            int nextSnapY = 7;
            if (obj is StandardObject)
            {
                //Parameter 1
                ctrl = new RawUshortEdit(obj.Parameters[0], valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                ctrl.MouseDown += DeselectOthers;
                ctrl.Tag = "Parameter 1";
                panControls.Controls.Add(ctrl);
                nextSnapY = Helper.snapControlVertically(ctrl, nextSnapY) + 7;

                //Parameter 2
                ctrl = new RawUshortEdit(obj.Parameters[1], valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                ctrl.MouseDown += DeselectOthers;
                panControls.Controls.Add(ctrl);
                ctrl.Tag = "Parameter 2";
                nextSnapY = Helper.snapControlVertically(ctrl, nextSnapY) + 7;

                //Parameter 3
                ctrl = new RawUshortEdit(obj.Parameters[2], valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                ctrl.MouseDown += DeselectOthers;
                panControls.Controls.Add(ctrl);
                ctrl.Tag = "Parameter 3";
                nextSnapY = Helper.snapControlVertically(ctrl, nextSnapY) + 7;
            }
            else if (obj is SimpleObject)
            {
                //Parameter 1
                ctrl = new RawUshortEdit(obj.Parameters[0], valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                ctrl.MouseDown += DeselectOthers;
                ctrl.Tag = "Parameter 1";
                panControls.Controls.Add(ctrl);
                nextSnapY = Helper.snapControlVertically(ctrl, nextSnapY) + 7;
            }
            else if (obj is PathObject)
            {
                //Parameter 1
                ctrl = new RawUshortEdit(obj.Parameters[2], valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                ctrl.MouseDown += DeselectOthers;
                ctrl.Tag = "Parameter 3";
                panControls.Controls.Add(ctrl);
                nextSnapY = Helper.snapControlVertically(ctrl, nextSnapY) + 7;

                //Parameter 2
                ctrl = new RawUshortEdit(obj.Parameters[3], valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                ctrl.MouseDown += DeselectOthers;
                panControls.Controls.Add(ctrl);
                ctrl.Tag = "Parameter 4";
                nextSnapY = Helper.snapControlVertically(ctrl, nextSnapY) + 7;

                //Parameter 3
                ctrl = new RawUshortEdit(obj.Parameters[4], valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                ctrl.MouseDown += DeselectOthers;
                panControls.Controls.Add(ctrl);
                ctrl.Tag = "Parameter 5";
                nextSnapY = Helper.snapControlVertically(ctrl, nextSnapY) + 7;
            }
            else
            {
                ctrl = new Label() { Text = "No Raw Editing available for this Object", Width = 300 };
                panControls.Controls.Add(ctrl);
                Helper.snapControlVertically(ctrl, nextSnapY);
            }
        }

        private void DeselectOthers(object sender, MouseEventArgs e)
        {
            Console.WriteLine("DeselectOthers");
            foreach (Control ctrl in panControls.Controls)
            {
                if ((ctrl is RawUshortEdit) && (ctrl != sender))
                    ((RawUshortEdit)ctrl).Deselect();
            }
        }

        private void RawEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_parent.m_rawEditor = null;
        }
        
        private void btnToogleBinary_Click(object sender, EventArgs e)
        {
            m_displayInBinary = !m_displayInBinary;
            foreach (Control ctrl in panControls.Controls)
            {
                if (ctrl is RawUshortEdit)
                    ((RawUshortEdit)ctrl).SetBinary(m_displayInBinary);
            }
            ((Button)sender).Text = "Display in " + (m_displayInBinary ? "Hex" : "Binary");
        }

        private void SendValueToLevelForm(RawUshortEdit sender)
        {
            m_parent.SetProperty((string)sender.Tag, sender.GetValue());
            m_parent.initializePropertyInterface();
        }

        private void btnToogleHex_Click(object sender, EventArgs e)
        {
            if (valueInput.Hexadecimal)
            {
                valueInput.Hexadecimal = false;
                btnToogleHex.Text = "Dec";
            }
            else
            {
                valueInput.Hexadecimal = true;
                btnToogleHex.Text = "Hex";
            }
        }
    }

    public partial class RawUshortEdit : UserControl
    {
        int m_fieldWidth;
        ushort m_valueToEdit;
        NumericUpDown m_valueInput;
        String m_stringRepresentation;
        int m_selectionStartIndex;
        int m_selectionEndIndex;
        bool m_inBinary;
        bool m_settingValues;

        public delegate void OnValueChanged(RawUshortEdit sender);

        public OnValueChanged ValueChanged = null;

        public RawUshortEdit(ushort value, NumericUpDown valueInput, bool displayBinary = false)
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.UserPaint | 
                ControlStyles.OptimizedDoubleBuffer, 
                true);
            Console.WriteLine("Created");
            m_valueInput = valueInput;
            m_valueInput.ValueChanged += M_valueInput_ValueChanged;
            m_valueToEdit = value;
            m_inBinary = !displayBinary;//Quick workaround for the SetBinary to work
            SetBinary(displayBinary);
            m_selectionStartIndex = -1;
            m_selectionEndIndex = 0;
            m_settingValues = false;
        }

        private void M_valueInput_ValueChanged(object sender, EventArgs e)
        {
            InsertValue((ushort)((NumericUpDown)sender).Value);
            ValueChanged?.Invoke(this);
        }

        public void Deselect()
        {
            m_selectionStartIndex = -1;
            //Console.WriteLine("Deselected");
            Refresh();
        }

        public bool isSelected()
        {
            return (m_selectionStartIndex != -1);
        }

        public bool InsertValue(ushort value)
        {
            if ((m_selectionStartIndex == -1)|| m_settingValues)
                return false;
            int selectionStart = m_selectionStartIndex;
            int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
            string stringValue = Convert.ToString(value, m_inBinary?2:16).PadLeft(selectionLength,'0');
            //Console.WriteLine(stringValue);
            if (stringValue.Length<= selectionLength)
            {
                string stringWithGap = m_stringRepresentation.Remove(selectionStart, selectionLength);
                m_stringRepresentation = stringWithGap.Insert(selectionStart,stringValue);
                Refresh();
                RefreshValue();
                return true;
            }
            return false;
        }

        public ushort GetValue()
        {
            return m_valueToEdit;
        }

        public void SetBinary(bool value)
        {
            if (value == m_inBinary)
                return;
            if (value)
            {
                Width = 320;
                Height = 20;
                m_inBinary = true;
                m_fieldWidth = 20;
                m_stringRepresentation = Convert.ToString(m_valueToEdit, 2).PadLeft(16, '0');
                if (m_selectionStartIndex != -1)
                {
                    m_selectionStartIndex *= 4;
                    m_selectionEndIndex *= 4;
                    m_selectionEndIndex += 3;
                }
            }
            else
            {
                Width = 160;
                Height = 20;
                m_inBinary = false;
                m_fieldWidth = 40;
                m_stringRepresentation = Convert.ToString(m_valueToEdit, 16).PadLeft(4, '0');
                if (m_selectionStartIndex != -1)
                {
                    m_selectionStartIndex /= 4;
                    m_selectionEndIndex /= 4;

                    int selectionStart = m_selectionStartIndex;
                    int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
                    m_settingValues = true; //prevent the valueChanged event

                    m_valueInput.Maximum = (ushort)(Math.Pow(m_inBinary ? 2 : 16, selectionLength) - 1);
                    m_valueInput.Value = Convert.ToUInt16(m_stringRepresentation.Substring(selectionStart, selectionLength), 16);

                    m_settingValues = false;
                }
            }
            Refresh();
        }

        public void RefreshValue()
        {
            if (m_inBinary)
            {
                m_valueToEdit = Convert.ToUInt16(m_stringRepresentation, 2);
            }
            else
            {
                m_valueToEdit = Convert.ToUInt16(m_stringRepresentation, 16);
                Console.WriteLine("Value after insert: " + m_valueToEdit);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor = SystemColors.ButtonShadow;
            Brush selBackColor = new SolidBrush(SystemColors.Highlight);
            Brush fieldColor = new SolidBrush(SystemColors.ButtonFace);
            Brush selFieldColor = new SolidBrush(SystemColors.GradientActiveCaption);
            Brush textColor = new SolidBrush(SystemColors.ControlText);
            Font textFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

            int selectionStart;
            int selectionLength;
            if (m_selectionStartIndex <= m_selectionEndIndex)
            {
                selectionStart = m_selectionStartIndex;
                selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
            }
            else
            {
                selectionStart = m_selectionEndIndex;
                selectionLength = (m_selectionStartIndex - m_selectionEndIndex) + 1;
            }

            e.Graphics.Clear(backColor);

            if (m_selectionStartIndex!=-1)
            {
                Rectangle selectionRect = new Rectangle(
                    selectionStart * m_fieldWidth, 0,
                    selectionLength * m_fieldWidth, Height
                );
                e.Graphics.FillRectangle(selBackColor, selectionRect);
            }
            for (int i = 0; i < (m_inBinary?16:4); i++)
            {
                Rectangle fieldRect = new Rectangle(1 + i*m_fieldWidth, 1, m_fieldWidth - 2, Height - 2);
                if ((i>= selectionStart) && (i< selectionStart + selectionLength) && (m_selectionStartIndex != -1))
                    e.Graphics.FillRectangle(selFieldColor, fieldRect);
                else
                    e.Graphics.FillRectangle(fieldColor, fieldRect);
                if ( m_inBinary && (Math.Floor(i/4.0)%2==0) )
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 100, 100, 50)), fieldRect);
                e.Graphics.DrawString(m_stringRepresentation[i].ToString(), textFont, textColor, fieldRect.Left+(m_inBinary?1:3),fieldRect.Top+2);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_selectionStartIndex = (int)Math.Floor(e.X /(float)m_fieldWidth);
            m_selectionEndIndex = m_selectionStartIndex;
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                m_selectionEndIndex = Math.Min(Math.Max(0,(int)Math.Floor(e.X / (float)m_fieldWidth)), m_inBinary ? 15 : 3);
                Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                if(m_selectionStartIndex > m_selectionEndIndex)
                {
                    int startIndexBefore = m_selectionStartIndex;
                    m_selectionStartIndex = m_selectionEndIndex;
                    m_selectionEndIndex = startIndexBefore;
                }

                int selectionStart = m_selectionStartIndex;
                int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;

                m_settingValues = true; //prevent the valueChanged event

                m_valueInput.Maximum = (ushort)(Math.Pow(m_inBinary ? 2 : 16, selectionLength)-1);
                m_valueInput.Value = Convert.ToUInt16(m_stringRepresentation.Substring(selectionStart, selectionLength), m_inBinary ? 2 : 16);

                m_settingValues = false;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //TODO implement a Direct Digit Writing
        }
    }
}
