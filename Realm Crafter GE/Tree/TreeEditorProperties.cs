//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using RenderingServices;
using System.IO;

namespace RealmCrafter_GE
{
    public partial class TreeEditorProperties : DockContent
    {
        public bool SuspendEvents = false;
        public TreeNode SelectedNode = null;

        public TreeEditorProperties()
        {
            InitializeComponent();
            ClearProperties();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            //Hide();
        }

        public void RealShow()
        {
            Program.GE.m_ZoneList.WorldZonesTree.Visible = false;
            Program.GE.m_propertyWindow.ObjectProperties.Visible = false;

            foreach (Control C in Program.GE.m_CreateWindow.Controls)
            {
                C.Parent = Program.GETreeManager;
            }
            Program.GETreeManager.TreesList.Parent = Program.GE.m_CreateWindow;
            Program.GETreeManager.TreesList.Visible = true;


            Controls.Remove(TreeComponents);
            TreeComponents.Parent = Program.GE.m_ZoneList.WorldZonesTree.Parent;
            TreeComponents.Visible = true;

            TreePropertiesGroup.Parent = Program.GE.m_propertyWindow;
            TreePropertiesGroup.Dock = DockStyle.Fill;
        }

        public void RealHide()
        {
            TreeComponents.Parent = this;
            TreePropertiesGroup.Parent = this;

            foreach (Control C in Program.GETreeManager.Controls)
            {
                C.Parent = Program.GE.m_CreateWindow;
            }
            Program.GETreeManager.TreesList.Parent = Program.GETreeManager;
            Program.GETreeManager.TreesList.Visible = false;

            Program.GE.m_ZoneList.WorldZonesTree.Visible = true;
            Program.GE.m_propertyWindow.ObjectProperties.Visible = true;
        }

        private TreeNode SubSearch(TreeNode node, TreePropertiesUpdater needle)
        {
            if ((node.Tag as TreePropertiesUpdater) == needle)
                return node;

            foreach (TreeNode Node in node.Nodes)
            {
                TreeNode SubNode = SubSearch(Node, needle);
                if (SubNode != null)
                    return SubNode;
            }

            return null;
        }

        private TreeNode SubSearch(TreeNode node, Entity needle)
        {
            if ((node.Tag as TreePropertiesUpdater) != null && (node.Tag as TreePropertiesUpdater).HasInternalEntity(needle))
                return node;

            foreach (TreeNode Node in node.Nodes)
            {
                TreeNode SubNode = SubSearch(Node, needle);
                if (SubNode != null)
                    return SubNode;
            }

            return null;
        }

        public TreeNode FindNode(TreePropertiesUpdater needle)
        {
            TreeNode Node = null;

            foreach (TreeNode SubNode in TreeComponents.Nodes)
            {
                Node = SubSearch(SubNode, needle);
                if (Node != null)
                    break;
            }

            return Node;
        }

        public TreeNode FindNode(Entity needle)
        {
            TreeNode Node = null;

            foreach (TreeNode SubNode in TreeComponents.Nodes)
            {
                Node = SubSearch(SubNode, needle);
                if (Node != null)
                    break;
            }

            return Node;
        }

        private void TreeComponents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SuspendEvents = true;
            if (SelectedNode != null)
                (SelectedNode.Tag as TreePropertiesUpdater).NotifyUnselected();
            SelectedNode = null;

            if ((e.Node.Tag as TreePropertiesUpdater) != null)
            {
                (e.Node.Tag as TreePropertiesUpdater).UpdateProperties(this);

                (e.Node.Tag as TreePropertiesUpdater).NotifySelected();
                (e.Node.Tag as TreePropertiesUpdater).UpdateWorldButtonSelection();
                SelectedNode = e.Node;
            }
            else
                ClearProperties();

            addLeafToolStripMenuItem.Enabled = false;
            singleLeadToolStripMenuItem.Enabled = false;
            leafPlacerToolStripMenuItem.Enabled = false;
            finalizeObjectToolStripMenuItem.Enabled = false;

            if ((e.Node.Tag as TreeEditorLeafGroup) != null)
            {
                addLeafToolStripMenuItem.Enabled = true;
                singleLeadToolStripMenuItem.Enabled = true;
                leafPlacerToolStripMenuItem.Enabled = true;
            }

            if ((e.Node.Tag as TreeEditorLeafBox) != null)
            {
                finalizeObjectToolStripMenuItem.Enabled = true;
            }

            SuspendEvents = false;
        }

        public void SwitchToLeafPlacement()
        {
            TextureDiffuse.Visible = false;
            TextureNormal.Visible = false;

            LeafCount.Visible = true;
            Seed.Visible = true;

            DiffuseLabel.Text = "Count:";
            NormalLabel.Text = "Seed:";
        }

        public void SwitchToNormal()
        {
            TextureDiffuse.Visible = true;
            TextureNormal.Visible = true;

            LeafCount.Visible = false;
            Seed.Visible = false;

            DiffuseLabel.Text = "Diffuse:";
            NormalLabel.Text = "Normal:";
        }

        public void ClearProperties()
        {
            PositionX.Value = 0;
            PositionY.Value = 0;
            PositionZ.Value = 0;
            ScaleX.Value = 0;
            ScaleY.Value = 0;
            ScaleZ.Value = 0;
            RotationX.Value = 0;
            RotationY.Value = 0;
            RotationZ.Value = 0;
            LeafCount.Value = 0;
            Seed.Value = 0;

            PositionX.Enabled = false;
            PositionY.Enabled = false;
            PositionZ.Enabled = false;
            ScaleX.Enabled = false;
            ScaleY.Enabled = false;
            ScaleZ.Enabled = false;
            RotationX.Enabled = false;
            RotationY.Enabled = false;
            RotationZ.Enabled = false;

            TextureDiffuse.Text = "Unused";
            TextureNormal.Text = "Unused";

            TextureDiffuse.Enabled = false;
            TextureNormal.Enabled = false;

            SwitchToNormal();
        }

        public void UpdateWorldButtonSelection()
        {
            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateWorldButtonSelection();
        }

        private void Position_ValueChanged(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdatePosition(
                Convert.ToSingle(PositionX.Value),
                Convert.ToSingle(PositionY.Value),
                Convert.ToSingle(PositionZ.Value));
        }

        private void Rotation_ValueChanged(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateRotation(
                Convert.ToSingle(RotationX.Value),
                Convert.ToSingle(RotationY.Value),
                Convert.ToSingle(RotationZ.Value));
        }

        private void Scale_ValueChanged(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateScale(
                Convert.ToSingle(ScaleX.Value),
                Convert.ToSingle(ScaleY.Value),
                Convert.ToSingle(ScaleZ.Value));
        }

        private void TextureDiffuse_Click(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            string CurrentDir = Environment.CurrentDirectory;
            DialogResult Dr = TextureFileDialog.ShowDialog();
            Environment.CurrentDirectory = CurrentDir;

            if (Dr == DialogResult.Cancel)
                return;

            if (TextureFileDialog.FileName.Length == 0 || System.IO.File.Exists(TextureFileDialog.FileName) == false)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateDiffuseTexture(TextureFileDialog.FileName);
        }

        private void TextureNormal_Click(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            string CurrentDir = Environment.CurrentDirectory;
            DialogResult Dr = TextureFileDialog.ShowDialog();
            Environment.CurrentDirectory = CurrentDir;

            if (Dr == DialogResult.Cancel)
                return;

            if (TextureFileDialog.FileName.Length == 0 || System.IO.File.Exists(TextureFileDialog.FileName) == false)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateNormalTexture(TextureFileDialog.FileName);
        }

        private void TextureMask_Click(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            string CurrentDir = Environment.CurrentDirectory;
            DialogResult Dr = TextureFileDialog.ShowDialog();
            Environment.CurrentDirectory = CurrentDir;

            if (Dr == DialogResult.Cancel)
                return;

            if (TextureFileDialog.FileName.Length == 0 || System.IO.File.Exists(TextureFileDialog.FileName) == false)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateMaskTexture(TextureFileDialog.FileName);
        }

        private void addTrunkMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ActiveTree != null)
            {
                if (MeshOpenDialog.ShowDialog() == DialogResult.Cancel)
                    return;

                string file = "Data/Trees/" + Path.GetFileName(MeshOpenDialog.FileName);
                File.Copy(MeshOpenDialog.FileName, file, true);

                Program.ActiveTree.AddTrunkMesh(file);
            }
        }

        private void addLeafGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ActiveTree != null)
                Program.ActiveTree.AddLeafGroup();
        }

        private void cubeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ActiveTree != null)
                Program.ActiveTree.AddPhysCapsule();
        }

        private void capsuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ActiveTree != null)
                Program.ActiveTree.AddPhysCube();
        }

        private void sphereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ActiveTree != null)
                Program.ActiveTree.AddPhysSphere();
        }

        public void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeComponents.SelectedNode == null)
                return;

            if ((TreeComponents.SelectedNode.Tag as TreePropertiesUpdater) == null)
                return;

            if (Program.ActiveTree == null || Program.ActiveTree.Name.Length == 0)
                return;

            (TreeComponents.SelectedNode.Tag as TreePropertiesUpdater).Remove();
            Program.ActiveTree.Remove(TreeComponents.SelectedNode.Tag as TreePropertiesUpdater);
            if (Program.Transformer != null)
                Program.Transformer.Free();
            Program.Transformer = null;
            UpdateWorldButtonSelection();
        }

        private void Coverage_ValueChanged(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateCount(Convert.ToInt32(LeafCount.Value));
        }

        private void Seed_ValueChanged(object sender, EventArgs e)
        {
            if (SuspendEvents)
                return;

            TreeNode Node = TreeComponents.SelectedNode;
            if (Node == null)
                return;

            if ((Node.Tag as TreePropertiesUpdater) == null)
                return;

            (Node.Tag as TreePropertiesUpdater).UpdateSeed(Convert.ToInt32(Seed.Value));
        }

        private void singleLeadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeComponents.SelectedNode == null)
                return;

            if ((TreeComponents.SelectedNode.Tag as TreeEditorLeafGroup) == null)
                return;

            if (Program.ActiveTree == null || Program.ActiveTree.Name.Length == 0)
                return;

            TreeEditorLeafGroup Group = (TreeComponents.SelectedNode.Tag as TreeEditorLeafGroup);

            TreeEditorLeaf L = new TreeEditorLeaf(Group);
            Group.Leaves.Add(L);
            L.ReBuild();
            Program.ActiveTree.UpdatePropertyPanel();
        }

        private void leafPlacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeComponents.SelectedNode == null)
                return;

            if ((TreeComponents.SelectedNode.Tag as TreeEditorLeafGroup) == null)
                return;

            if (Program.ActiveTree == null || Program.ActiveTree.Name.Length == 0)
                return;

            TreeEditorLeafGroup Group = (TreeComponents.SelectedNode.Tag as TreeEditorLeafGroup);

            TreeEditorLeafBox B = new TreeEditorLeafBox(Group);
            Group.Placers.Add(B);
            Program.ActiveTree.UpdatePropertyPanel();
        }

        private void finalizeObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TreeComponents.SelectedNode == null)
                return;

            if ((TreeComponents.SelectedNode.Tag as TreeEditorLeafBox) == null)
                return;

            if (Program.ActiveTree == null || Program.ActiveTree.Name.Length == 0)
                return;

            (TreeComponents.SelectedNode.Tag as TreeEditorLeafBox).FinalizeLeaves();
            if (Program.Transformer != null)
                Program.Transformer.Free();
            Program.Transformer = null;
            UpdateWorldButtonSelection();

        }

        private void TreeComponents_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                deleteToolStripMenuItem_Click(sender, EventArgs.Empty);
        }
    }
}