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
// Realm Crafter Media Dialogs module by Rob W (rottbott@hotmail.com)
// Original version August 2004, first revision February 2005, second revision May 2005, C# port September 2006

using System;
using System.Windows.Forms;
using RenderingServices;
using RealmCrafter;

namespace RealmCrafter_GE
{
    // Mesh selection dialog window
    public class MeshSelectionDialog : Form
    {
        // Members
        public ushort Result;
        public string DefaultPath;
        private TreeView MainMediaTree;
        private TreeView MeshSelectionTree;
        private Button MeshSelectionSelect;
        public Button MeshSelectionCancel;
        private System.ComponentModel.IContainer components = null;
        private Entity MediaMeshEN;

        // Constructor
        public MeshSelectionDialog(TreeView MainTree)
        {
            InitializeComponents();
            this.MainMediaTree = MainTree;
        }

        // Create and initialise all components
        private void InitializeComponents()
        {
            // Create all components
            this.MeshSelectionTree = new System.Windows.Forms.TreeView();
            this.MeshSelectionSelect = new System.Windows.Forms.Button();
            this.MeshSelectionCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // MeshSelectionTree
            this.MeshSelectionTree.Location = new System.Drawing.Point(12, 12);
            this.MeshSelectionTree.Name = "MeshSelectionTree";
            this.MeshSelectionTree.Size = new System.Drawing.Size(263, 240);
            this.MeshSelectionTree.TabIndex = 1;

            // MeshSelectionSelect
            this.MeshSelectionSelect.Location = new System.Drawing.Point(388, 258);
            this.MeshSelectionSelect.Name = "MeshSelectionSelect";
            this.MeshSelectionSelect.Size = new System.Drawing.Size(73, 23);
            this.MeshSelectionSelect.TabIndex = 2;
            this.MeshSelectionSelect.Text = "Select";
            this.MeshSelectionSelect.UseVisualStyleBackColor = true;
            this.MeshSelectionSelect.Click += new System.EventHandler(this.MeshSelectionSelect_Click);

            // MeshSelectionNone
            this.MeshSelectionCancel.Location = new System.Drawing.Point(467, 258);
            this.MeshSelectionCancel.Name = "MeshSelectionCancel";
            this.MeshSelectionCancel.Size = new System.Drawing.Size(73, 23);
            this.MeshSelectionCancel.TabIndex = 3;
            this.MeshSelectionCancel.Text = "Cancel";
            this.MeshSelectionCancel.UseVisualStyleBackColor = true;
            this.MeshSelectionCancel.Click += new System.EventHandler(this.MeshSelectionCancel_Click);

            // MeshSelectionDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(287, 288);
            this.ClientSize = new System.Drawing.Size(552, 288);
            this.Controls.Add(this.MeshSelectionCancel);
            this.Controls.Add(this.MeshSelectionSelect);
            this.Controls.Add(this.MeshSelectionTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeshSelectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Mesh";
            this.Shown += new System.EventHandler(this.MeshSelectionDialog_Shown);
            this.ResumeLayout(false);
        }

        // Clean up any resources being used
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Select button click events
        private void MeshSelectionSelect_Click(object sender, EventArgs e)
        {
            if (MeshSelectionTree.SelectedNode != null)
            {
                if (MeshSelectionTree.SelectedNode.Tag != null)
                {
                    Result = (ushort) MeshSelectionTree.SelectedNode.Tag;
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                if (MeshSelectionTree.SelectedNode == MeshSelectionTree.Nodes[0])
                {
                    Result = 65535;
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
            }
            MessageBox.Show("No mesh selected!", "Choose Mesh");
        }

        private void MeshSelectionCancel_Click(object sender, EventArgs e)
        {
            if (MediaMeshEN != null)
            {
                MediaMeshEN.Free();
                MediaMeshEN = null;
            }
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0010)
            {
                if (Program.GE.WorldPanelLoop)
                {
                    System.Windows.Forms.Application.Idle +=
                        new EventHandler(Program.GE.m_ZoneRender.WorldRender_MainLoop);
                    Program.GE.RenderingPanel.MouseClick +=
                        new System.Windows.Forms.MouseEventHandler(Program.GE.m_ZoneRender.RenderingPanel_MouseClick);
                }

                else
                {
                    System.Windows.Forms.Application.Idle += new EventHandler(Program.GE.MainLoop);
                    Program.GE.RenderingPanel.MouseClick +=
                        new System.Windows.Forms.MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
                }

                System.Windows.Forms.Application.Idle -= new EventHandler(Idle);
                Program.GE.UpdateRenderingPanel(Program.GE.RenderingPanelPreviousIndex);

                if (MediaMeshEN != null)
                {
                    MediaMeshEN.Free();
                    MediaMeshEN = null;
                }
            }

            base.WndProc(ref m);
        }

        // Form shown
        private void MeshSelectionDialog_Shown(object sender, EventArgs e)
        {
            // Show all meshes in project
            MeshSelectionTree.BeginUpdate();
            MeshSelectionTree.Nodes.Clear();
            MeshSelectionTree.Nodes.Add((TreeNode) MainMediaTree.Nodes[0].Clone());
            MeshSelectionTree.CollapseAll();
            MeshSelectionTree.Nodes[0].Expand();
            MeshSelectionTree.EndUpdate();

            // Expand default folder
            if (DefaultPath != "")
            {
                string Folder;
                TreeNode CurrentNode = MeshSelectionTree.Nodes[0];
                for (int i = 0; i < DefaultPath.Length; ++i)
                {
                    if (DefaultPath.Substring(i, 1) == @"/" || DefaultPath.Substring(i, 1) == @"\")
                    {
                        Folder = DefaultPath.Substring(0, i);
                        DefaultPath = DefaultPath.Substring(i + 1);

                        CurrentNode = CurrentNode.Nodes[Folder];
                        if (CurrentNode == null)
                        {
                            break;
                        }
                        CurrentNode.Expand();
                    }
                }
                CurrentNode = CurrentNode.Nodes[DefaultPath];
                if (CurrentNode != null)
                {
                    CurrentNode.Expand();
                }
            }

            MeshSelectionTree.Nodes.Insert(0, new TreeNode("No Mesh      "));
            MeshSelectionTree.Nodes[0].NodeFont = MeshSelectionTree.Nodes[1].NodeFont;

            // Move the render panel
            Program.GE.UpdateRenderingPanel((int)RealmCrafter_GE.GE.GETab.MESHDIALOG);
            if (Program.GE.WorldPanelLoop)
            {
                System.Windows.Forms.Application.Idle -= new EventHandler(Program.GE.m_ZoneRender.WorldRender_MainLoop);
                Program.GE.RenderingPanel.MouseClick -=
                    new System.Windows.Forms.MouseEventHandler(Program.GE.m_ZoneRender.RenderingPanel_MouseClick);
            }
            else
            {
                System.Windows.Forms.Application.Idle -= new EventHandler(Program.GE.MainLoop);
                Program.GE.RenderingPanel.MouseClick -=
                    new System.Windows.Forms.MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
            }
            System.Windows.Forms.Application.Idle += new EventHandler(Idle);

            MeshSelectionTree.AfterSelect += new TreeViewEventHandler(MeshSelectionTree_AfterSelect);

            TreeNode TN = GetNodeFromShortTag(MeshSelectionTree.Nodes[1], this.Result);

            if (Result < 65535 && TN != null)
            {
                MeshSelectionTree.SelectedNode = TN;
                TreeViewEventArgs E = new TreeViewEventArgs(TN, TreeViewAction.ByMouse);
                MeshSelectionTree_AfterSelect(MeshSelectionTree, E);
            }

            //MoreImportantPanel.BringToFront();
            //MoreImportantPanel.Focus();
        }

        private TreeNode GetNodeFromShortTag(TreeNode TV, ushort Tag)
        {
            foreach (TreeNode N in TV.Nodes)
            {
                if (N.Tag != null && (ushort) N.Tag == Tag)
                {
                    return N;
                }
                else
                {
                    TreeNode R = GetNodeFromShortTag(N, Tag);
                    if (R != null)
                    {
                        return R;
                    }
                }
            }

            return null;
        }

        private void MeshSelectionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Collapse && e.Action != TreeViewAction.Expand)
            {
                if (e.Node == MeshSelectionTree.Nodes[0])
                {
                    if (MediaMeshEN != null)
                    {
                        MediaMeshEN.Free();
                        MediaMeshEN = null;
                    }
                    return;
                }

                // Get type and media ID of selected node
                string SelectedMediaType = e.Node.FullPath.Substring(0, 2);
                int NewMediaID = 65535;
                if (e.Node.Tag != null)
                {
                    NewMediaID = (ushort) e.Node.Tag;
                }

                switch (SelectedMediaType)
                {
                        // Mesh
                    case "Me":
                        if (NewMediaID < 65535)
                        {
                            if (MediaMeshEN != null)
                            {
                                MediaMeshEN.Free();
                                MediaMeshEN = null;
                            }

                            MediaMeshEN = Media.GetMesh(NewMediaID);
                            if (MediaMeshEN != null)
                            {
                                MediaMeshEN.Position(0, -20f, 350f);
                                // Set scale ( Marian Voicu )
                                float meshScales = Media.LoadedMeshScales[NewMediaID];
                                if ((decimal) meshScales == 0)
                                {
                                    meshScales = 1.0F;
                                }
                                Media.SizeEntity(MediaMeshEN, 150f * meshScales, 150f * meshScales, 150f * meshScales,
                                                 true);
                                // end ( MV )
                                MediaMeshEN.Visible = true;
                            }
                            else
                            {
                                SelectedMediaType = "";
                            }
                        }
                        break;
                }
            }
        }

        public void Idle(object sender, EventArgs e)
        {
            RenderingServices.Render.RenderWorld();
        }
    }

    // Texture selection dialog window
    public class TextureSelectionDialog : Form
    {
        // Members
        public ushort Result;
        public string DefaultPath;
        private TreeView MainMediaTree;
        private TreeView TextureSelectionTree;
        private Button TextureSelectionSelect;
        public Button TextureSelectionCancel;
        private System.ComponentModel.IContainer components = null;
        private Entity TextureQuad;
        private uint LoadedTexture, DefaultTexture;

        // Constructor
        public TextureSelectionDialog(TreeView MainTree)
        {
            InitializeComponents();
            this.MainMediaTree = MainTree;

            LoadedTexture = 0;
            DefaultTexture = Media.GetTexture(65535, false);
            TextureQuad = Entity.CreateSAQuad();
            TextureQuad.SAQuadLayout(0, 0, 1, 1);
            TextureQuad.Order = -1;
            TextureQuad.Visible = false;
        }

        // Create and initialise all components
        private void InitializeComponents()
        {
            // Create all components
            this.TextureSelectionTree = new System.Windows.Forms.TreeView();
            this.TextureSelectionSelect = new System.Windows.Forms.Button();
            this.TextureSelectionCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // TextureSelectionTree
            this.TextureSelectionTree.Location = new System.Drawing.Point(12, 12);
            this.TextureSelectionTree.Name = "TextureSelectionTree";
            this.TextureSelectionTree.Size = new System.Drawing.Size(263, 240);
            this.TextureSelectionTree.TabIndex = 1;

            // TextureSelectionSelect
            this.TextureSelectionSelect.Location = new System.Drawing.Point(388, 258);
            this.TextureSelectionSelect.Name = "TextureSelectionSelect";
            this.TextureSelectionSelect.Size = new System.Drawing.Size(73, 23);
            this.TextureSelectionSelect.TabIndex = 2;
            this.TextureSelectionSelect.Text = "Select";
            this.TextureSelectionSelect.UseVisualStyleBackColor = true;
            this.TextureSelectionSelect.Click += new System.EventHandler(this.TextureSelectionSelect_Click);

            // TextureSelectionNone
            this.TextureSelectionCancel.Location = new System.Drawing.Point(467, 258);
            this.TextureSelectionCancel.Name = "TextureSelectionNone";
            this.TextureSelectionCancel.Size = new System.Drawing.Size(73, 23);
            this.TextureSelectionCancel.TabIndex = 3;
            this.TextureSelectionCancel.Text = "Cancel";
            this.TextureSelectionCancel.UseVisualStyleBackColor = true;
            this.TextureSelectionCancel.Click += new System.EventHandler(this.TextureSelectionCancel_Click);

            // TextureSelectionDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 288);
            this.Controls.Add(this.TextureSelectionCancel);
            this.Controls.Add(this.TextureSelectionSelect);
            this.Controls.Add(this.TextureSelectionTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureSelectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Texture";
            this.Shown += new System.EventHandler(this.TextureSelectionDialog_Shown);
            this.ResumeLayout(false);
        }

        // Clean up any resources being used
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing && this.TextureQuad != null)
            {
                this.TextureQuad.Free();
                this.TextureQuad = null;
            }

            if (disposing && LoadedTexture != 0)
            {
                Render.FreeTexture(this.LoadedTexture);
                this.LoadedTexture = 0;
            }

            base.Dispose(disposing);
        }

        // Select button click events
        private void TextureSelectionSelect_Click(object sender, EventArgs e)
        {
            if (TextureSelectionTree.SelectedNode != null)
            {
                if (TextureSelectionTree.SelectedNode.Tag != null)
                {
                    Result = (ushort) TextureSelectionTree.SelectedNode.Tag;
                    Close();
                    return;
                }

                if (TextureSelectionTree.SelectedNode == TextureSelectionTree.Nodes[0])
                {
                    Result = 65535;
                    Close();
                    return;
                }
            }
            MessageBox.Show("No texture selected!", "Choose Texture");
        }

        private void TextureSelectionCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0010)
            {
                if (Program.GE.WorldPanelLoop)
                {
                    System.Windows.Forms.Application.Idle +=
                        new EventHandler(Program.GE.m_ZoneRender.WorldRender_MainLoop);
                    Program.GE.RenderingPanel.MouseClick +=
                        new System.Windows.Forms.MouseEventHandler(Program.GE.m_ZoneRender.RenderingPanel_MouseClick);
                }

                else
                {
                    System.Windows.Forms.Application.Idle += new EventHandler(Program.GE.MainLoop);
                    Program.GE.RenderingPanel.MouseClick +=
                        new System.Windows.Forms.MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
                }

                System.Windows.Forms.Application.Idle -= new EventHandler(Idle);
                TextureSelectionTree.AfterSelect -= new TreeViewEventHandler(TextureSelectionTree_AfterSelect);
                Program.GE.UpdateRenderingPanel(Program.GE.RenderingPanelPreviousIndex);

                TextureQuad.Visible = false;
            }

            base.WndProc(ref m);
        }

        // Form shown
        private void TextureSelectionDialog_Shown(object sender, EventArgs e)
        {
            // Show all textures in project
            TextureSelectionTree.BeginUpdate();
            TextureSelectionTree.Nodes.Clear();
            TextureSelectionTree.Nodes.Add((TreeNode) MainMediaTree.Nodes[1].Clone());
            TextureSelectionTree.CollapseAll();
            TextureSelectionTree.Nodes[0].Expand();
            TextureSelectionTree.EndUpdate();

            // Expand default folder
            if (DefaultPath != "")
            {
                string Folder;
                TreeNode CurrentNode = TextureSelectionTree.Nodes[0];
                for (int i = 0; i < DefaultPath.Length; ++i)
                {
                    if (DefaultPath.Substring(i, 1) == @"/" || DefaultPath.Substring(i, 1) == @"\")
                    {
                        Folder = DefaultPath.Substring(0, i);
                        DefaultPath = DefaultPath.Substring(i + 1);

                        CurrentNode = CurrentNode.Nodes[Folder];
                        if (CurrentNode == null)
                        {
                            break;
                        }
                        CurrentNode.Expand();
                    }
                }
                CurrentNode = CurrentNode.Nodes[DefaultPath];
                if (CurrentNode != null)
                {
                    CurrentNode.Expand();
                }
            }

            TextureSelectionTree.Nodes.Insert(0, new TreeNode("No Texture      "));
            TextureSelectionTree.Nodes[0].NodeFont = TextureSelectionTree.Nodes[1].NodeFont;

            // Move the render panel
            Program.GE.UpdateRenderingPanel((int)RealmCrafter_GE.GE.GETab.TEXTUREDIALOG);
            if (Program.GE.WorldPanelLoop)
            {
                System.Windows.Forms.Application.Idle -= new EventHandler(Program.GE.m_ZoneRender.WorldRender_MainLoop);
                Program.GE.RenderingPanel.MouseClick -=
                    new System.Windows.Forms.MouseEventHandler(Program.GE.m_ZoneRender.RenderingPanel_MouseClick);
            }
            else
            {
                System.Windows.Forms.Application.Idle -= new EventHandler(Program.GE.MainLoop);
                Program.GE.RenderingPanel.MouseClick -=
                    new System.Windows.Forms.MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
            }
            System.Windows.Forms.Application.Idle += new EventHandler(Idle);

            TextureSelectionTree.AfterSelect += new TreeViewEventHandler(TextureSelectionTree_AfterSelect);

            TreeNode TN = GetNodeFromShortTag(TextureSelectionTree.Nodes[1], this.Result);

            if (Result < 65535 && TN != null)
            {
                TextureSelectionTree.SelectedNode = TN;
                TreeViewEventArgs E = new TreeViewEventArgs(TN, TreeViewAction.ByMouse);
                TextureSelectionTree_AfterSelect(TextureSelectionTree, E);
            }
        }

        private TreeNode GetNodeFromShortTag(TreeNode TV, ushort Tag)
        {
            foreach (TreeNode N in TV.Nodes)
            {
                if (N.Tag != null && (ushort) N.Tag == Tag)
                {
                    return N;
                }
                else
                {
                    TreeNode R = GetNodeFromShortTag(N, Tag);
                    if (R != null)
                    {
                        return R;
                    }
                }
            }

            return null;
        }

        private void TextureSelectionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Collapse && e.Action != TreeViewAction.Expand)
            {
                if (e.Node == TextureSelectionTree.Nodes[0])
                {
                    if (this.LoadedTexture != 0)
                    {
                        TextureQuad.Visible = false;
                    }
                    return;
                }

                // Get type and media ID of selected node
                string SelectedMediaType = e.Node.FullPath.Substring(0, 2);
                int NewMediaID = 65535;
                if (e.Node.Tag != null)
                {
                    NewMediaID = (ushort) e.Node.Tag;
                }

                switch (SelectedMediaType)
                {
                        // Texture
                    case "Te":
                        if (NewMediaID < 65535)
                        {
                            TextureQuad.Visible = true;
                            uint Tex = Media.GetTexture(NewMediaID, false);
                            if (Tex != 0)
                            {
                                TextureQuad.Texture(Tex, 0);
                                LoadedTexture = Tex;
                                Media.UnloadTexture(NewMediaID);
                            }
                            else
                            {
                                SelectedMediaType = "";
                            }
                        }
                        break;
                }
            }
        }

        public void Idle(object sender, EventArgs e)
        {
            RenderingServices.Render.RenderWorld();
        }
    }

    // Sound selection dialog window
    public class SoundSelectionDialog : Form
    {
        // Members
        public ushort Result;
        public string DefaultPath;
        private TreeView MainMediaTree;
        private TreeView SoundSelectionTree;
        private Button SoundSelectionSelect;
        public Button SoundSelectionCancel;
        private System.ComponentModel.IContainer components = null;

        // Constructor
        public SoundSelectionDialog(TreeView MainTree)
        {
            InitializeComponents();
            this.MainMediaTree = MainTree;
        }

        // Create and initialise all components
        private void InitializeComponents()
        {
            // Create all components
            this.SoundSelectionTree = new System.Windows.Forms.TreeView();
            this.SoundSelectionSelect = new System.Windows.Forms.Button();
            this.SoundSelectionCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // SoundSelectionTree
            this.SoundSelectionTree.Location = new System.Drawing.Point(12, 12);
            this.SoundSelectionTree.Name = "SoundSelectionTree";
            this.SoundSelectionTree.Size = new System.Drawing.Size(263, 240);
            this.SoundSelectionTree.TabIndex = 1;

            // SoundSelectionSelect
            this.SoundSelectionSelect.Location = new System.Drawing.Point(123, 258);
            this.SoundSelectionSelect.Name = "SoundSelectionSelect";
            this.SoundSelectionSelect.Size = new System.Drawing.Size(73, 23);
            this.SoundSelectionSelect.TabIndex = 2;
            this.SoundSelectionSelect.Text = "Select";
            this.SoundSelectionSelect.UseVisualStyleBackColor = true;
            this.SoundSelectionSelect.Click += new System.EventHandler(this.SoundSelectionSelect_Click);

            // SoundSelectionNone
            this.SoundSelectionCancel.Location = new System.Drawing.Point(202, 258);
            this.SoundSelectionCancel.Name = "SoundSelectionCancel";
            this.SoundSelectionCancel.Size = new System.Drawing.Size(73, 23);
            this.SoundSelectionCancel.TabIndex = 3;
            this.SoundSelectionCancel.Text = "Cancel";
            this.SoundSelectionCancel.UseVisualStyleBackColor = true;
            this.SoundSelectionCancel.Click += new System.EventHandler(this.SoundSelectionCancel_Click);

            // SoundSelectionDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 288);
            this.Controls.Add(this.SoundSelectionCancel);
            this.Controls.Add(this.SoundSelectionSelect);
            this.Controls.Add(this.SoundSelectionTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoundSelectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Sound";
            this.Shown += new System.EventHandler(this.SoundSelectionDialog_Shown);
            this.ResumeLayout(false);
        }

        // Clean up any resources being used
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Select button click events
        private void SoundSelectionSelect_Click(object sender, EventArgs e)
        {
            if (SoundSelectionTree.SelectedNode != null)
            {
                if (SoundSelectionTree.SelectedNode.Tag != null)
                {
                    Result = (ushort) SoundSelectionTree.SelectedNode.Tag;
                    Close();
                    return;
                }

                // Selected "No Sound"
                if (SoundSelectionTree.SelectedNode == SoundSelectionTree.Nodes[0])
                {
                    Result = 65535;
                    Close();
                    return;
                }
            }
            MessageBox.Show("No sound selected!", "Choose Sound");
        }

        private void SoundSelectionCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0010)
            {
            }

            base.WndProc(ref m);
        }

        // Form shown
        private void SoundSelectionDialog_Shown(object sender, EventArgs e)
        {
            // Show all sounds in project
            SoundSelectionTree.BeginUpdate();
            SoundSelectionTree.Nodes.Clear();
            SoundSelectionTree.Nodes.Add((TreeNode) MainMediaTree.Nodes[2].Clone());
            SoundSelectionTree.CollapseAll();
            SoundSelectionTree.Nodes[0].Expand();
            SoundSelectionTree.EndUpdate();

            // Expand default folder
            if (DefaultPath != "")
            {
                string Folder;
                TreeNode CurrentNode = SoundSelectionTree.Nodes[0];
                for (int i = 0; i < DefaultPath.Length; ++i)
                {
                    if (DefaultPath.Substring(i, 1) == @"/" || DefaultPath.Substring(i, 1) == @"\")
                    {
                        Folder = DefaultPath.Substring(0, i);
                        DefaultPath = DefaultPath.Substring(i + 1);

                        CurrentNode = CurrentNode.Nodes[Folder];
                        if (CurrentNode == null)
                        {
                            break;
                        }
                        CurrentNode.Expand();
                    }
                }
                CurrentNode = CurrentNode.Nodes[DefaultPath];
                if (CurrentNode != null)
                {
                    CurrentNode.Expand();
                }
            }

            // Add "No Sound" node
            SoundSelectionTree.Nodes.Insert(0, new TreeNode("No Sound      "));
            SoundSelectionTree.Nodes[0].NodeFont = SoundSelectionTree.Nodes[1].NodeFont;

            TreeNode TN = GetNodeFromShortTag(SoundSelectionTree.Nodes[1], this.Result);

            // Select already selected node
            if (Result < 65535 && TN != null)
            {
                SoundSelectionTree.SelectedNode = TN;
            }
        }

        private TreeNode GetNodeFromShortTag(TreeNode TV, ushort Tag)
        {
            foreach (TreeNode N in TV.Nodes)
            {
                if (N.Tag != null && (ushort) N.Tag == Tag)
                {
                    return N;
                }
                else
                {
                    TreeNode R = GetNodeFromShortTag(N, Tag);
                    if (R != null)
                    {
                        return R;
                    }
                }
            }

            return null;
        }
    }

    // Music selection dialog window
    public class MusicSelectionDialog : Form
    {
        // Members
        public ushort Result;
        public string DefaultPath;
        private TreeView MainMediaTree;
        private TreeView MusicSelectionTree;
        private Button MusicSelectionSelect;
        public Button MusicSelectionCancel;
        private System.ComponentModel.IContainer components = null;

        // Constructor
        public MusicSelectionDialog(TreeView MainTree)
        {
            InitializeComponents();
            this.MainMediaTree = MainTree;
        }

        // Create and initialise all components
        private void InitializeComponents()
        {
            // Create all components
            this.MusicSelectionTree = new System.Windows.Forms.TreeView();
            this.MusicSelectionSelect = new System.Windows.Forms.Button();
            this.MusicSelectionCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // MusicSelectionTree
            this.MusicSelectionTree.Location = new System.Drawing.Point(12, 12);
            this.MusicSelectionTree.Name = "MusicSelectionTree";
            this.MusicSelectionTree.Size = new System.Drawing.Size(263, 240);
            this.MusicSelectionTree.TabIndex = 1;

            // MusicSelectionSelect
            this.MusicSelectionSelect.Location = new System.Drawing.Point(123, 258);
            this.MusicSelectionSelect.Name = "MusicSelectionSelect";
            this.MusicSelectionSelect.Size = new System.Drawing.Size(73, 23);
            this.MusicSelectionSelect.TabIndex = 2;
            this.MusicSelectionSelect.Text = "Select";
            this.MusicSelectionSelect.UseVisualStyleBackColor = true;
            this.MusicSelectionSelect.Click += new System.EventHandler(this.MusicSelectionSelect_Click);

            // MusicSelectionNone
            this.MusicSelectionCancel.Location = new System.Drawing.Point(202, 258);
            this.MusicSelectionCancel.Name = "MusicSelectionCancel";
            this.MusicSelectionCancel.Size = new System.Drawing.Size(73, 23);
            this.MusicSelectionCancel.TabIndex = 3;
            this.MusicSelectionCancel.Text = "Cancel";
            this.MusicSelectionCancel.UseVisualStyleBackColor = true;
            this.MusicSelectionCancel.Click += new System.EventHandler(this.MusicSelectionCancel_Click);

            // MusicSelectionDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 288);
            this.Controls.Add(this.MusicSelectionCancel);
            this.Controls.Add(this.MusicSelectionSelect);
            this.Controls.Add(this.MusicSelectionTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MusicSelectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Music";
            this.Shown += new System.EventHandler(this.MusicSelectionDialog_Shown);
            this.ResumeLayout(false);
        }

        // Clean up any resources being used
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Select button click events
        private void MusicSelectionSelect_Click(object sender, EventArgs e)
        {
            if (MusicSelectionTree.SelectedNode != null)
            {
                if (MusicSelectionTree.SelectedNode.Tag != null)
                {
                    Result = (ushort) MusicSelectionTree.SelectedNode.Tag;
                    Close();
                    return;
                }

                // Select no music
                if (MusicSelectionTree.SelectedNode == MusicSelectionTree.Nodes[0])
                {
                    Result = 65535;
                    Close();
                    return;
                }
            }
            MessageBox.Show("No music selected!", "Choose Music");
        }

        private void MusicSelectionCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0010)
            {
            }

            base.WndProc(ref m);
        }

        // Form shown
        private void MusicSelectionDialog_Shown(object sender, EventArgs e)
        {
            // Show all music in project
            MusicSelectionTree.BeginUpdate();
            MusicSelectionTree.Nodes.Clear();
            MusicSelectionTree.Nodes.Add((TreeNode) MainMediaTree.Nodes[3].Clone());
            MusicSelectionTree.CollapseAll();
            MusicSelectionTree.Nodes[0].Expand();
            MusicSelectionTree.EndUpdate();

            // Expand default folder
            if (DefaultPath != "")
            {
                string Folder;
                TreeNode CurrentNode = MusicSelectionTree.Nodes[0];
                for (int i = 0; i < DefaultPath.Length; ++i)
                {
                    if (DefaultPath.Substring(i, 1) == @"/" || DefaultPath.Substring(i, 1) == @"\")
                    {
                        Folder = DefaultPath.Substring(0, i);
                        DefaultPath = DefaultPath.Substring(i + 1);

                        CurrentNode = CurrentNode.Nodes[Folder];
                        if (CurrentNode == null)
                        {
                            break;
                        }
                        CurrentNode.Expand();
                    }
                }
                CurrentNode = CurrentNode.Nodes[DefaultPath];
                if (CurrentNode != null)
                {
                    CurrentNode.Expand();
                }
            }

            MusicSelectionTree.Nodes.Insert(0, new TreeNode("No Music       "));
            MusicSelectionTree.Nodes[0].NodeFont = MusicSelectionTree.Nodes[1].NodeFont;

            TreeNode TN = GetNodeFromShortTag(MusicSelectionTree.Nodes[1], this.Result);

            // Select selected music
            if (Result < 65535 && TN != null)
            {
                MusicSelectionTree.SelectedNode = TN;
            }
        }

        private TreeNode GetNodeFromShortTag(TreeNode TV, ushort Tag)
        {
            foreach (TreeNode N in TV.Nodes)
            {
                if (N.Tag != null && (ushort) N.Tag == Tag)
                {
                    return N;
                }
                else
                {
                    TreeNode R = GetNodeFromShortTag(N, Tag);
                    if (R != null)
                    {
                        return R;
                    }
                }
            }

            return null;
        }
    }

    // Collection of media dialog functions
    internal static class MediaDialogs
    {
        public static int TotalMeshes, TotalTextures, TotalSounds, TotalMusic;

        public enum MeshDialog
        {
            All = 1,
            Animated = 2,
            Static = 3
        } ;

        public enum SoundDialog
        {
            All = 1,
            ThreeD = 2,
            Normal = 3
        } ;

        // Selection dialog instances
        public static MeshSelectionDialog MeshSelector;
        public static TextureSelectionDialog TextureSelector;
        public static SoundSelectionDialog SoundSelector;
        public static MusicSelectionDialog MusicSelector;

        // Add items to the media manager display
        public static void AddMesh(string Name, ushort ID, TreeNode Tree)
        {
            TotalMeshes++;
            // Split path and filename and add to tree view
            string Path = "";
            for (int j = Name.Length - 1; j >= 0; --j)
            {
                if (Name.Substring(j, 1) == @"/" || Name.Substring(j, 1) == @"\")
                {
                    Path = Name.Substring(0, j);
                    Name = Name.Substring(j + 1);
                    break;
                }
            }
            // Animated flag
            if (Name.Substring(Name.Length - 1, 1) == "0")
            {
                Name = Name.Substring(0, Name.Length - 1);
            }
            else
            {
                Name = Name.Substring(0, Name.Length - 1) + " (Animated)";
            }
            // Add to tree view
            AddToTreeView(Tree, Path, Name, ID);
        }

        public static void AddTexture(string Name, ushort ID, TreeNode Tree)
        {
            TotalTextures++;
            // Split path and filename and add to tree view
            string Path = "";
            for (int j = Name.Length - 1; j >= 0; --j)
            {
                if (Name.Substring(j, 1) == @"/" || Name.Substring(j, 1) == @"\")
                {
                    Path = Name.Substring(0, j);
                    Name = Name.Substring(j + 1);
                    break;
                }
            }
            // Texture flags
            int Flags = Blitz.IntFromStr(Name.Substring(Name.Length - 1, 1));
            Name = Name.Substring(0, Name.Length - 1) + " (" + Flags.ToString() + ")";
            // Add to tree view
            AddToTreeView(Tree, Path, Name, ID);
        }

        public static void AddSound(string Name, ushort ID, TreeNode Tree)
        {
            TotalSounds++;
            // Split path and filename and add to tree view
            string Path = "";
            for (int j = Name.Length - 1; j >= 0; --j)
            {
                if (Name.Substring(j, 1) == @"/" || Name.Substring(j, 1) == @"\")
                {
                    Path = Name.Substring(0, j);
                    Name = Name.Substring(j + 1);
                    break;
                }
            }
            // 3D flag
            if (Name.Substring(Name.Length - 1, 1) == "0")
            {
                Name = Name.Substring(0, Name.Length - 1);
            }
            else
            {
                Name = Name.Substring(0, Name.Length - 1) + " (3D)";
            }
            // Add to tree view
            AddToTreeView(Tree, Path, Name, ID);
        }

        public static void AddMusic(string Name, ushort ID, TreeNode Tree)
        {
            TotalMusic++;
            // Split path and filename and add to tree view
            string Path = "";
            for (int j = Name.Length - 1; j >= 0; --j)
            {
                if (Name.Substring(j, 1) == @"/" || Name.Substring(j, 1) == @"\")
                {
                    Path = Name.Substring(0, j);
                    Name = Name.Substring(j + 1);
                    break;
                }
            }
            // Add to tree view
            AddToTreeView(Tree, Path, Name, ID);
        }

        // Loads media names from disk and initialises all media selection gadgets
        public static void Init(TreeView MainTree)
        {
            // Create dialog form instances
            MeshSelector = new MeshSelectionDialog(MainTree);
            TextureSelector = new TextureSelectionDialog(MainTree);
            SoundSelector = new SoundSelectionDialog(MainTree);
            MusicSelector = new MusicSelectionDialog(MainTree);

            TotalMeshes = 0;
            TotalMusic = 0;
            TotalSounds = 0;
            TotalTextures = 0;

            // Fill tree views
            MainTree.BeginUpdate();

            // Load media file names into main tree view
            string Name;
            ushort i;
            DateTime StartTime = DateTime.Now;
            // Meshes
            for (i = 0; i < 65535; ++i)
            {
                Name = Media.GetMeshName(i);
                if (Name.Length > 0)
                {
                    AddMesh(Name, i, MainTree.Nodes[0]);
                    Program.MeshList.Add(Name + " ID: " + i);
                }
            }
            Media.UnlockMeshes();
            DateTime EndTime = DateTime.Now;
            TimeSpan Duration = EndTime - StartTime;
            Program.WriteToLog(Program.MeshList.Count + " meshes data loaded in " + Duration.TotalSeconds + " Seconds");
            // Textures
            StartTime = DateTime.Now;
            Media.LockTextures();
            for (i = 0; i < 65535; ++i)
            {
                Name = Media.GetTextureName(i);
                if (Name.Length > 0)
                {
                    AddTexture(Name, i, MainTree.Nodes[1]);
                    Program.TextureList.Add(Name + " ID: " + i);
                }
            }
            Media.UnlockTextures();
            EndTime = DateTime.Now;
            Duration = EndTime - StartTime;
            Program.WriteToLog(Program.TextureList.Count + " Textures data loaded in " + Duration.TotalSeconds + " Seconds");
            // Sounds
            Media.LockSounds();
            StartTime = DateTime.Now;
            for (i = 0; i < 65535; ++i)
            {
                Name = Media.GetSoundName(i);
                if (Name.Length > 0)
                {
                    AddSound(Name, i, MainTree.Nodes[2]);
                    Program.SoundsList.Add(Name + " ID: " + i);
                }
            }
            Media.UnlockSounds();
            EndTime = DateTime.Now;
            Duration = EndTime - StartTime;
            Program.WriteToLog(Program.SoundsList.Count + " Sounds data loaded in " + Duration.TotalSeconds + " Seconds");
            // Music
            StartTime = DateTime.Now;
            Media.LockMusic();
            for (i = 0; i < 65535; ++i)
            {
                Name = Media.GetMusicName(i);
                if (Name.Length > 0)
                {
                    AddMusic(Name, i, MainTree.Nodes[3]);
                    Program.MusicList.Add(Name + " ID: " + i);
                }
            }
            Media.UnlockMusic();
            EndTime = DateTime.Now;
            Duration = EndTime - StartTime;
            Program.WriteToLog(Program.MusicList.Count + " Music data loaded in " + Duration.TotalSeconds + " Seconds");

            MainTree.EndUpdate();
        }

        // Get the user to select an item from the database
        public static ushort GetMesh(bool AllowNone, string DefaultFolder, ushort LastSelectedID)
        {
            MeshSelector.Result = LastSelectedID;
            //MeshSelector.MeshSelectionCancel.Enabled = AllowNone;
            MeshSelector.DefaultPath = DefaultFolder;
            MeshSelector.ShowDialog();
            return MeshSelector.Result;
        }

        public static ushort GetMesh(bool AllowNone, ushort LastSelectedID)
        {
            return GetMesh(AllowNone, "", LastSelectedID);
        }

        public static ushort GetTexture(bool AllowNone, string DefaultFolder, ushort LastSelectedID)
        {
            TextureSelector.Result = LastSelectedID;
            //TextureSelector.TextureSelectionNone.Enabled = AllowNone;
            TextureSelector.DefaultPath = DefaultFolder;
            TextureSelector.ShowDialog();
            return TextureSelector.Result;
        }

        public static ushort GetTexture(bool AllowNone, ushort LastSelectedID)
        {
            return GetTexture(AllowNone, "", LastSelectedID);
        }

        public static ushort GetSound(bool AllowNone, string DefaultFolder, ushort LastSelectID)
        {
            SoundSelector.Result = LastSelectID;
            //SoundSelector.SoundSelectionNone.Enabled = AllowNone;
            SoundSelector.DefaultPath = DefaultFolder;
            SoundSelector.ShowDialog();
            return SoundSelector.Result;
        }

        public static ushort GetSound(bool AllowNone, ushort LastSelectedID)
        {
            return GetSound(AllowNone, "", LastSelectedID);
        }

        public static ushort GetMusic(bool AllowNone, string DefaultFolder, ushort LastSelectedID)
        {
            MusicSelector.Result = LastSelectedID;
            //MusicSelector.MusicSelectionNone.Enabled = AllowNone;
            MusicSelector.DefaultPath = DefaultFolder;
            MusicSelector.ShowDialog();
            return MusicSelector.Result;
        }

        public static ushort GetMusic(bool AllowNone, ushort LastSelectedID)
        {
            return GetMusic(AllowNone, "", LastSelectedID);
        }

        // Adds a specified filename to a treeview within a specified folder (RECURSIVE)
        private static void AddToTreeView(TreeNode T, string Path, string Filename, ushort MediaID)
        {
            // No path, add to current node
            if (Path.Length == 0)
            {
                TreeNode TN = T.Nodes.Add(Filename);
                TN.Tag = MediaID;
            }
            else
            {
                // Path present, find the right node
                // Find position of first folder
                int Pos, Pos2;
                Pos = Path.IndexOf(@"/");
                Pos2 = Path.IndexOf(@"\");
                if ((Pos2 < Pos && Pos2 > -1) || Pos < 0)
                {
                    Pos = Pos2;
                }

                // Split folder
                if (Pos > -1)
                {
                    string FolderName = Path.Substring(0, Pos);
                    Path = Path.Substring(Pos + 1);
                    // Find the folder in the tree view, creating it if it doesn't exist
                    TreeNode TN = T.Nodes[FolderName];
                    if (TN == null)
                    {
                        TN = T.Nodes.Add(FolderName, FolderName);
                    }
                    AddToTreeView(TN, Path, Filename, MediaID);
                }
                    // No more folders, find the final node
                else
                {
                    TreeNode TN = T.Nodes[Path];
                    if (TN == null)
                    {
                        TN = T.Nodes.Add(Path, Path);
                    }
                    AddToTreeView(TN, "", Filename, MediaID);
                }
            }
        }
    }
}