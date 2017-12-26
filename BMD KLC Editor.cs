using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SM64DSe.ImportExport;
using SM64DSe.ImportExport.Loaders.InternalLoaders;
using SM64DSe.ImportExport.Writers.InternalWriters;
using OpenTK;
namespace SM64DSe
{
    public partial class BMD_KLC_Editor : Form
    {
        private BMD m_Model;
        private string m_ModelName;
        private NitroFile m_ModelFile;
        private string[] m_primitiveNames = new string[] { "Triangles","Quads","TriangleStrip","QuadStrip" };
        private object m_SelectedContent;
        public BMD_KLC_Editor(string fileName)
        {
            InitializeComponent();
            panProperties.Controls.Clear();
            m_ModelName = fileName;
            if (m_ModelName.EndsWith(".bmd"))
            {
                m_ModelFile = Program.m_ROM.GetFileFromName(m_ModelName);
                m_Model = new BMD(m_ModelFile);

                Text = fileName.Split('/').Last();
                TreeNode modelNode = tvModelContent.Nodes.Add("Model");
                TreeNode chunksNode = modelNode.Nodes.Add("Chunks");

                int i = 0;
                foreach (BMD.ModelChunk chunk in m_Model.m_ModelChunks)
                {
                    TreeNode chunkNode = chunksNode.Nodes.Add(i.ToString(), chunk.m_Name);
                    chunkNode.Tag = chunk;
                    TreeNode matGroupsNode = chunkNode.Nodes.Add("Material Groups");

                    int i2 = 0;
                    foreach (BMD.MaterialGroup matGroup in chunk.m_MatGroups)
                    {
                        TreeNode matGroupNode = matGroupsNode.Nodes.Add(i2.ToString(), matGroup.m_Name);
                        matGroupNode.Tag = matGroup;
                        TreeNode vtxListsNode = matGroupNode.Nodes.Add("Vertex Lists");

                        int i3 = 0;
                        foreach (BMD.VertexList vtxList in matGroup.m_Geometry)
                        {
                            TreeNode vtxListNode = vtxListsNode.Nodes.Add(i3.ToString(), "Vertex Group[" +i3.ToString()+"] ("+m_primitiveNames[vtxList.m_PolyType]+")");
                            vtxListNode.Tag = vtxList;
                            TreeNode vertsNode = vtxListNode.Nodes.Add("Vertices");

                            int i4 = 0;
                            foreach (BMD.Vertex vtx in vtxList.m_VertexList)
                            { 
                                TreeNode vtxNode = vertsNode.Nodes.Add(i4.ToString(), "Vertex[" + i4.ToString()+"] ("+vtx.m_Position + ")");
                                vtxNode.Tag = vtx;
                                vtxNode.Nodes.Add("Position: "+vtx.m_Position);
                                vtxNode.Nodes.Add("Normal: " + vtx.m_Normal);
                                vtxNode.Nodes.Add("Texture Coordinates: "+vtx.m_TexCoord);
                                vtxNode.Nodes.Add("Vertex Color: " + vtx.m_Color);
                                vtxNode.Nodes.Add("Matrix ID: " + vtx.m_MatrixID);
                                i4++;
                            }
                            i3++;
                        }
                        i2++;
                    }
                    i++;
                }
            }
            
        }

        private void tvModelContent_AfterSelect(object sender, TreeViewEventArgs e)
        {
            m_SelectedContent = tvModelContent.SelectedNode.Tag;

            panProperties.Controls.Clear();
            if (m_SelectedContent is BMD.Vertex)
            {
                panProperties.Controls.Add(box_position);
                BMD.Vertex vtx = (BMD.Vertex)m_SelectedContent;
                vtx.m_Position = Vector3.Add(vtx.m_Position, new Vector3(0, 1000, 0));
            }
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            m_Model.m_ModelChunks[0].m_MatGroups[0].m_Alpha = 12;
            BMDImporter.CallBMDWriter(ref m_ModelFile, new BMDLoader(m_ModelName, m_Model).LoadModel(), BMDImporter.BMDExtraImportOptions.DEFAULT).m_File.SaveChanges();
        }
    }
}
