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
    public partial class LevelDisplaySettingsForm : Form
    {
        LevelEditorForm m_parent;
        public LevelDisplaySettingsForm(LevelEditorForm parent)
        {
            InitializeComponent();
            m_parent = parent;
            checkTextures.Checked = m_parent.m_levelModelDisplayFlags[0];
            checkVtxColors.Checked = m_parent.m_levelModelDisplayFlags[1];
            checkWireframe.Checked = m_parent.m_levelModelDisplayFlags[2];
            checkPolylistTypes.Checked = m_parent.m_levelModelDisplayFlags[3];
            valFarClipping.Value = m_parent.GetFarClipping();
            valFOV.Value = m_parent.GetFOVangle();
        }

        private void checkTextures_Click(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            m_parent.setDisplayFlag(0, check.Checked);
        }

        private void checkVtxColors_Click(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            m_parent.setDisplayFlag(1, check.Checked);
        }

        private void checkWireframe_Click(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            m_parent.setDisplayFlag(2, check.Checked);
        }

        private void checkPolylistTypes_Click(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            m_parent.setDisplayFlag(3, check.Checked);
        }

        private void PerspectiveBtn_Click(object sender, EventArgs e)
        {
            
            if (sender == btnTop)
                    m_parent.ChangePerspective((float)Math.PI / 2,(float)Math.PI / 2);

            else if(sender == btnBottom)
                m_parent.ChangePerspective((float)Math.PI / 2, -(float)Math.PI / 2);

            else if (sender == btnLeft)
                m_parent.ChangePerspective((float)Math.PI, 0);

            else if (sender == btnRight)
                m_parent.ChangePerspective(0, 0);

            else if(sender == btnFront)
                m_parent.ChangePerspective((float)Math.PI / 2,0);

            else if(sender == btnBack)
                m_parent.ChangePerspective(-(float)Math.PI / 2,0);
        }

        private void valFarClipping_ValueChanged(object sender, EventArgs e)
        {
            m_parent.SetFarClipping((int)valFarClipping.Value);
        }

        private void valFOV_ValueChanged(object sender, EventArgs e)
        {
            m_parent.SetFOV((int)valFOV.Value);
        }

        private void btnToogleOrtho_Click(object sender, EventArgs e)
        {
            m_parent.ToogleOrtho();
        }
    }
}
