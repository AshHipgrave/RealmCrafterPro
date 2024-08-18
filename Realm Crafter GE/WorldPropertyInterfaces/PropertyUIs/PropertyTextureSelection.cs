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
using System.Windows.Forms;
using RenderingServices;
using RealmCrafter;
using System.Drawing.Design;

namespace RealmCrafter_GE.Property_Interfaces.PropertyUIs
{
    public class PropertyTextureSelection : UITypeEditor
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
        public PropertyTextureSelection()
        {
            TreeView MainTree = new TreeView();
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
            //this.SuspendLayout();

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

            //// TextureSelectionDialog
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(552, 288);
            //this.Controls.Add(this.TextureSelectionCancel);
            //this.Controls.Add(this.TextureSelectionSelect);
            //this.Controls.Add(this.TextureSelectionTree);
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            //this.MaximizeBox = false;
            //this.MinimizeBox = false;
            //this.Name = "TextureSelectionDialog";
            //this.ShowIcon = false;
            //this.ShowInTaskbar = false;
            //this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            //this.Text = "Choose Texture";
            //this.Shown += new System.EventHandler(this.TextureSelectionDialog_Shown);
            //this.ResumeLayout(false);
        }

        // Clean up any resources being used
        protected void Dispose(bool disposing)
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

            //   base.Dispose(disposing);
        }

        // Select button click events
        private void TextureSelectionSelect_Click(object sender, EventArgs e)
        {
            if (TextureSelectionTree.SelectedNode != null)
            {
                if (TextureSelectionTree.SelectedNode.Tag != null)
                {
                    Result = (ushort) TextureSelectionTree.SelectedNode.Tag;
                    //            Close();
                    return;
                }

                if (TextureSelectionTree.SelectedNode == TextureSelectionTree.Nodes[0])
                {
                    Result = 65535;
                    //         Close();
                    return;
                }
            }
            MessageBox.Show("No texture selected!", "Choose Texture");
        }

        private void TextureSelectionCancel_Click(object sender, EventArgs e)
        {
            //   Close();
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
            Program.GE.RenderingPanel.MouseClick -=
                new System.Windows.Forms.MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
            System.Windows.Forms.Application.Idle -= new EventHandler(Program.GE.MainLoop);
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
}