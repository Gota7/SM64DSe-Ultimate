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

        private DynamicLibraryManager _dynamicLibraryManager;

        public DL_Editor(DynamicLibraryManager dynamicLibraryManager)
        {
            this._dynamicLibraryManager = dynamicLibraryManager;
            InitializeComponent();
            
            // Load file list to ui
            availableTree.Nodes.Clear();
            ROMFileSelect.LoadFileList(this.availableTree);
            
            UpdateCurrent();
        }

        private void UpdateCurrent()
        {
            // Clear current
            currentTree.Nodes.Clear();
            
            // From the DynamicLibraryManager load the current libraries in the 
            foreach (var currentLibrary in _dynamicLibraryManager.GetCurrentLibrariesFilenames())
            {
                currentTree.Nodes.Add(currentLibrary);
            }
        }


        private void btnRemove_ButtonClick(object sender, EventArgs e)
        { 
            if (currentTree.SelectedNode != null) {
                _dynamicLibraryManager.Remove(currentTree.SelectedNode.Text);
                UpdateCurrent();
            }
        }

        private void btnAdd_ButtonClick(object sender, EventArgs e)
        {
            // Ensure a node is properly selected
            if (availableTree.SelectedNode != null)
            {
                ushort fileId = Program.m_ROM.GetFileIDFromName(availableTree.SelectedNode.Tag.ToString());
                string filename = Program.m_ROM.GetFileNameFromID(fileId);
                
                // Add it to the dynamic library manager
                _dynamicLibraryManager.Add(filename);
                UpdateCurrent();
            }

        }

    }
}
