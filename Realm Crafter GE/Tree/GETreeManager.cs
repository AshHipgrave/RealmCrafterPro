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
using System.IO;

namespace RealmCrafter_GE
{
    public partial class GETreeManager : Form
    {
        TreeNode NewTreeNode = null;

        public GETreeManager()
        {
            InitializeComponent();
        }

        public void LoadTrees()
        {
            try
            {
                string[] LTFiles = Directory.GetFiles(@"Data\Trees\", "*.lt");

                foreach (string LTFile in LTFiles)
                {
                    string XMLFile = @"Data\LTSaves\" + Path.GetFileNameWithoutExtension(LTFile) + ".xml";

                    if (!File.Exists(XMLFile))
                        XMLFile = "";

                    LTNet.TreeType LTType = null;
                    if(Program.Manager != null)
                        LTType = Program.Manager.LoadTreeType(LTFile);

                    StoredTree Store = new StoredTree(LTFile, XMLFile, LTType);
                    TreeNode Node = new TreeNode(Store.ToString());
                    Node.Tag = Store;

                    if (Store.XMLFile.Length == 0)
                        Node.ForeColor = Color.Red;
                    else if (Store.LTFile.Length == 0 || Store.LTType == null)
                        Node.ForeColor = Color.Yellow;

                    TreesList.Nodes.Add(Node);
                }

                NewTreeNode = new TreeNode("New Tree");
               
                TreesList.Nodes.Add(NewTreeNode);
                TreesList.ContextMenu = new ContextMenu();

                MenuItem deleteButton = new MenuItem();
                deleteButton.Text = "Delete";
                deleteButton.Click += TreesList_DeleteClick;


                TreesList.ContextMenu.MenuItems.Add(deleteButton);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);	
            }
        }

        private void TreesList_DeleteClick(object sender, EventArgs e)
        {
            TreeNode node = TreesList.SelectedNode;

            // Check if New tree button
            if (node == NewTreeNode)
                return;

            DialogResult result = MessageBox.Show("Are you sure you wish to delete " + node.Text + "?", "Delete Tree", MessageBoxButtons.YesNo);
            if(node != null && result == DialogResult.Yes)
            {
                // Remove from tree
                TreesList.Nodes.Remove(node);
                
                // Delete files
                string ltFile = @"Data\Trees\" + node.Text + ".lt";
                string xmlFile = @"Data\LTSaves\" + node.Text + ".xml";


                if (File.Exists(xmlFile))
                    File.Delete(xmlFile);

                if (File.Exists(ltFile))
                    File.Delete(ltFile);

                Program.GE.TreeRender.Saved = false;
            }
            
        }

        private void TreesList_DoubleClick(object sender, EventArgs e)
        {
            StoredTree Store;

            if (TreesList.SelectedNode == null)
                return;

            // New tree
            if (TreesList.SelectedNode == NewTreeNode)
            {
                // Check save status
                if (Program.GE.TreeRender.Saved == false)
                {
                    DialogResult Dr = MessageBox.Show("The tree you are currently editing has not been saved. Should I save first?", "You might lose your work!", MessageBoxButtons.YesNoCancel);
                    if (Dr == DialogResult.Yes)
                        Save();
                    if (Dr == DialogResult.Cancel)
                        return;

                    Program.GE.TreeRender.Saved = true;
                }

                NamePrompt Prompt = new NamePrompt();
                Prompt.Prompt = "Enter a name for the new tree:";
                Prompt.Title = "New Tree";

                if(Prompt.ShowDialog() == DialogResult.Cancel)
                    return;

                if(Prompt.Value.Length == 0)
                    return;

                System.Text.RegularExpressions.Regex Reg = new System.Text.RegularExpressions.Regex("[0-9a-zA-Z]");
                if (!Reg.IsMatch(Prompt.Value))
                {
                    MessageBox.Show("Error: Invalid name! Please use numbers and letters only.");
                    return;
                }

                string[] LTFiles = Directory.GetFiles(@"Data\Trees\", "*.lt");

                string LTFile = @"Data\Trees\" + Prompt.Value + ".lt";
                string XMLFile = @"Data\LTSaves\" + Prompt.Value + ".xml";

                Store = new StoredTree(LTFile, XMLFile, null);
                TreeNode Node = NewTreeNode;
                Node.Tag = Store;
                Node.Text = Store.ToString();

                Program.ActiveTree.Unload();
                Program.ActiveTree.SwayCenter.UpdatePosition(0, 0, 0);
                Program.ActiveTree.Name = Prompt.Value;
                Program.ActiveTree.Save(Store.XMLFile, Path.GetDirectoryName(Store.LTFile) + "\\");
                Program.ActiveTree.Load(Store.XMLFile);

                NewTreeNode = new TreeNode("New Tree");
                TreesList.Nodes.Add(NewTreeNode);

                return;
            }

            if ((TreesList.SelectedNode.Tag as StoredTree) == null)
                return;

            Store = TreesList.SelectedNode.Tag as StoredTree;

            // There is only an LT instance
            if (Store.XMLFile.Length == 0)
                return;

            // Check save status
            if (Program.GE.TreeRender.Saved == false)
            {
                DialogResult Dr = MessageBox.Show("The tree you are currently editing has not been saved. Should I save first?", "You might lose your work!", MessageBoxButtons.YesNoCancel);
                if (Dr == DialogResult.Yes)
                    Save();
                if (Dr == DialogResult.Cancel)
                    return;

                Program.GE.TreeRender.Saved = true;
            }

            // Load tree
            Program.ActiveTree.Load(Store.XMLFile);
        }

        public void Save()
        {
            if (Program.ActiveTree == null || Program.ActiveTree.Name.Length == 0)
                return;
            if (TreesList.SelectedNode == null)
                return;
            if(!(TreesList.SelectedNode.Tag is StoredTree))
                return;

            StoredTree Store = TreesList.SelectedNode.Tag as StoredTree;

            if (Store.ToString().Equals("Unknown"))
                throw new Exception("Could not save LT object as it does not have a name!");

            string Name = Store.ToString();

            if(Store.XMLFile.Length == 0)
                Store.XMLFile = @"Data\LTSaves\" + Name + ".xml";
            Store.LTFile = @"Data\Trees\" + Path.GetFileNameWithoutExtension(Store.XMLFile) + ".lt";

            Program.ActiveTree.Save(Store.XMLFile, Path.GetDirectoryName(Store.LTFile) + "\\");

            if (Program.Manager != null)
            {
                if (Store.LTType == null)
                    Store.LTType = Program.Manager.LoadTreeType(Store.LTFile);
                else
                    Program.Manager.ReloadTreeType(Store.LTType, Store.LTFile);
            }
        }
    }

    public class StoredTree
    {
        public string LTFile = "";
        public string XMLFile = "";
        public LTNet.TreeType LTType = null;

        public StoredTree(string ltFile, string xmlFile, LTNet.TreeType ltType)
        {
            LTFile = ltFile;
            XMLFile = xmlFile;
            LTType = ltType;
        }

        public override string ToString()
        {
            if (LTFile.Length > 0)
                return Path.GetFileNameWithoutExtension(LTFile);
            if (XMLFile.Length > 0)
                return Path.GetFileNameWithoutExtension(XMLFile);
            return "Unknown";
        }
    }
}