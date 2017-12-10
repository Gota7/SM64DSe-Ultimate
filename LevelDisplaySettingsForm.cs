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
    }
}
