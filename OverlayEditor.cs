using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe {
    public partial class OverlayEditor : Form {

        public List<NitroROM.OverlayEntry> Overlays = new List<NitroROM.OverlayEntry>();
        public List<NitroROM.FileEntry> Files = new List<NitroROM.FileEntry>();

        public OverlayEditor() {
            InitializeComponent();
            Init();
        }

        private void Init() {
            if (!Program.m_ROM.StartFilesystemEdit()) {
                Close();
                return;
            }
            Overlays = Program.m_ROM.m_OverlayEntries.ToList();
            Files = Program.m_ROM.m_FileEntries.ToList();
            TreeUpdate();
            tree.NodeMouseClick += TreeHandler;
            tree.AfterSelect += TreeHandler;
            tree.KeyUp += TreeHandler;
        }

        private void TreeUpdate() {
            tree.SelectedNode = null;
            tree.Nodes.Clear();
            foreach (var e in Overlays) {
                tree.Nodes.Add("Overlay " + e.ID);
                tree.Nodes[tree.Nodes.Count - 1].ContextMenuStrip = rightClickMenu;
            }
            tree.SelectedNode = tree.Nodes[0];
            MenuUpdate();
        }

        private void TreeHandler(object sender, EventArgs e) {
            MenuUpdate();
        }

        private void MenuUpdate() {
            var o = Overlays[tree.SelectedNode.Index];
            status.Text = "Overlay " + o.ID + " Selected.";
            idBox.Value = o.ID;
            ramAddressBox.Value = o.RAMAddress;
            ramSizeBox.Value = o.RAMSize;
            bssSizeBox.Value = o.BSSSize;
            staticInitStartBox.Value = o.StaticInitStart;
            staticInitEndBox.Value = o.StaticInitEnd;
            flagsBox.Value = o.Flags;
        }

        public void saveChangesButton_Click(object sender, EventArgs e) {
            for (int i = 0; i < Overlays.Count; i++) {
                var x = Overlays[i];
                x.EntryOffset = (uint)(Program.m_ROM.OVTOffset + i * 0x20);
                Overlays[i] = x;
            }
            Program.m_ROM.m_OverlayEntries = Overlays.ToArray();
            Program.m_ROM.m_FileEntries = Files.ToArray();
            Program.m_ROM.OVTSize = (uint)(0x20 * Overlays.Count);
            Program.m_ROM.FATSize = (uint)(0x8 * Files.Count);
            Program.m_ROM.RewriteSizeTables();
            Program.m_ROM.SaveFilesystem();
            Program.m_ROM.StartFilesystemEdit();
        }

        public void closeButton_Click(object sender, EventArgs e) {
            Program.m_ROM.RevertFilesystem();
            Close();
        }

        public void updateButton_Click(object sender, EventArgs e) {
            var o = Overlays[tree.SelectedNode.Index];
            o.ID = (uint)idBox.Value;
            o.RAMAddress = (uint)ramAddressBox.Value;
            o.RAMSize = (uint)ramSizeBox.Value;
            o.BSSSize = (uint)bssSizeBox.Value;
            o.StaticInitStart = (uint)staticInitStartBox.Value;
            o.StaticInitEnd = (uint)staticInitEndBox.Value;
            o.Flags = (uint)flagsBox.Value;
            Overlays[tree.SelectedNode.Index] = o;
            MenuUpdate();
        }

        private void addAboveToolStripMenuItem_Click(object sender, EventArgs e) {
            AddOverlay(tree.SelectedNode.Index + 1);
        }

        private void addBelowToolStripMenuItem_Click(object sender, EventArgs e) {
            AddOverlay(tree.SelectedNode.Index);
        }

        public void AddOverlay(int index) {
            Overlays.Add(new NitroROM.OverlayEntry() {
                FileID = (ushort)Files.Count,
                BSSSize = 0,
                EntryOffset = 0,
                Flags = 0,
                ID = (uint)Overlays.Count,
                RAMAddress = 0,
                RAMSize = 0x20,
                StaticInitEnd = 4,
                StaticInitStart = 0
            });
            Files.Add(new NitroROM.FileEntry() {
                FullName = "",
                Name = "",
                Data = new byte[0x20],
                ID = (ushort)Files.Count,
                InternalID = 0xFFFF,
                Offset = uint.MaxValue,
                ParentID = 0,
                Size = 0x20
            });
            TreeUpdate();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e) {
            if (tree.SelectedNode.Index > 1) {
                var tmp = Overlays[tree.SelectedNode.Index];
                Overlays[tree.SelectedNode.Index] = Overlays[tree.SelectedNode.Index - 1];
                Overlays[tree.SelectedNode.Index - 1] = tmp;
                TreeUpdate();
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e) {
            if (tree.SelectedNode.Index < tree.Nodes.Count - 1) {
                var tmp = Overlays[tree.SelectedNode.Index];
                Overlays[tree.SelectedNode.Index] = Overlays[tree.SelectedNode.Index + 1];
                Overlays[tree.SelectedNode.Index + 1] = tmp;
                TreeUpdate();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            DeleteOverlay(tree.SelectedNode.Index);
        }

        public void DeleteOverlay(int index) {
            var o = Overlays[index];
            uint fileId = o.FileID;
            Overlays.RemoveAt(index);
            var f = Files[(int)fileId];
            f.Data = new byte[0x20];
            f.Size = 0x20;
            TreeUpdate();
        }
    
    }
}
