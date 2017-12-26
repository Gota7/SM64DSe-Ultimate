using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe.SM64DSFormats
{
    public partial class CreateNewLevelAnimationDialog : Form
    {
        Level m_level;
        int m_area;
        public CreateNewLevelAnimationDialog(Level level, int area)
        {
            InitializeComponent();
            m_level = level;
            m_area = area;
            Text += area;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            m_level.m_TexAnims[m_area].m_NumFrames = (uint)numAnimationLength.Value;
            m_level.m_TexAnims[m_area].m_Defs = new List<LevelTexAnim.Def>();
        }
    }
}
