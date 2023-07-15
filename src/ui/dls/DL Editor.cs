using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SM64DSe.SM64DSFormats
{
    public partial class DL_Editor : Form
    {

        LevelEditorForm lf;
        NitroOverlay no;
        uint tableAddr;

        public DL_Editor(LevelEditorForm lf)
        {
            InitializeComponent();

            this.availableTree.Nodes.Clear();
            ROMFileSelect.LoadFileList(this.availableTree);
            this.lf = lf;
            no = new NitroOverlay(Program.m_ROM, (uint)(103 + lf.m_LevelID));
            tableAddr = no.Read32(0x30);

            uint count = no.Read16(tableAddr);
            for (uint i = 0; i < count; i++) {

                currentTree.Nodes.Add(Program.m_ROM.GetFileNameFromID(Program.m_ROM.GetFileIDFromInternalID(no.Read16(i*2 + tableAddr + 2))));

            }

        }


        private void btnRemove_ButtonClick(object sender, EventArgs e)
        {

            if (currentTree.SelectedNode != null) {
                currentTree.SelectedNode.Remove();
            }

        }

        private void btnAdd_ButtonClick(object sender, EventArgs e)
        {

            if (availableTree.SelectedNode != null) {

                currentTree.Nodes.Add(Program.m_ROM.GetFileNameFromID(Program.m_ROM.GetFileIDFromName(availableTree.SelectedNode.Tag.ToString())));

            }

        }

        private void btnSave_ButtonClick(object sender, EventArgs e)
        {

            MemoryStream src = new MemoryStream(no.m_Data);
            BinaryReader br = new BinaryReader(src);
            byte[] oldData = br.ReadBytes((int)tableAddr);

            MemoryStream o = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(o);
            bw.Write(oldData);
            bw.Write((UInt16)currentTree.Nodes.Count);
            foreach (TreeNode n in currentTree.Nodes) {
                bw.Write(Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(n.Text)].InternalID);
            }
            no.m_Data = o.ToArray();
            no.SaveChanges();

        }
    }
}
