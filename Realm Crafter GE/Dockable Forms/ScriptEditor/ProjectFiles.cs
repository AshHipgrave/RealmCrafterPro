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
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class ProjectFiles : DockContent
    {
        private string Extension = Program.ScriptExtension;

        public ProjectFiles()
        {
            InitializeComponent();
        }

        private void ProjectFiles_Load(object sender, EventArgs e)
        {
//             ProjectTree.Nodes.Add(Program.ProjectName);
//             ProjectTree.Nodes[0].ImageIndex = 0;
//             string[] Scripts = new string[GE.ScriptsList.GetUpperBound(0) + 2];
//             int Count = 0;
//             foreach (string S in GE.ScriptsList)
//             {
//                 if (S.Length > 12 && S.Substring(S.Length - 12, 12).Equals(".designer.cs", StringComparison.CurrentCultureIgnoreCase))
//                     continue;
// 
//                 bool IsForm = false;
//                 string DesignerFile = S.Substring(0, S.Length - 2) + ".xml";
//                 if (File.Exists(@"Data\Server Data\Scripts\" + DesignerFile))
//                     IsForm = true;
// 
//                 TreeNode TN = new TreeNode(S);
//                 TN.ImageIndex = IsForm ? 2 : 1;
//                 TN.SelectedImageIndex = IsForm ? 2 : 1;
//                 TN.StateImageIndex = IsForm ? 2 : 1;
//                 ProjectTree.Nodes[0].Nodes.Add(TN);
//                 Scripts[Count] = S;
//                 ++Count;
//             }
// 
//             ProjectTree.Nodes[0].Expand();

            ProjectRefreshScripts_Click(sender, e);
        }

        private void ProjectTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ProjectTree.SelectedNode != null)
            {
                if (ProjectTree.SelectedNode.Level >= 1)
                {
                    bool isOpen = false;
                    foreach (ScriptEditorForm documentForm in Program.GE.m_ScriptView.ScriptDock.Documents)
                    {
                        string file = ProjectTree.SelectedNode.Text;
                        if (file == documentForm.TabText || file + " *" == documentForm.TabText)
                        {
                            // Some how this works. Selects document
                            documentForm.Hide();
                            documentForm.Show();
                            isOpen = true;
                            break;
                        }
                    }

                    // Open the files
                    if (!isOpen)
                    {
                        Program.GE.m_ScriptView.LoadFile(ProjectTree.SelectedNode.Text, false);
                    }
                }
            }
        }

        private void ProjectRefreshScripts_Click(object sender, EventArgs e)
        {
            Program.GE.RefreshScripts();
            ProjectTree.Nodes.Clear();
            this.Update();
            ProjectTree.Nodes.Add(Program.GE.GameName);
            ProjectTree.Nodes[0].ImageIndex = 0;
            string[] Scripts = new string[GE.ScriptsList.GetUpperBound(0) + 2];
            int Count = 0;
            foreach (string S in GE.ScriptsList)
            {
                if (S.Length > 12 && S.Substring(S.Length - 12, 12).Equals(".designer.cs", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                bool IsForm = false;
                string DesignerFile = S.Substring(0, S.Length - 2) + "xml";
                if (File.Exists(@"Data\Server Data\Scripts\" + DesignerFile))
                    IsForm = true;

                TreeNode TN = new TreeNode(S);
                TN.ImageIndex = IsForm ? 2 : 1;
                TN.SelectedImageIndex = IsForm ? 2 : 1;
                TN.StateImageIndex = IsForm ? 2 : 1;
                ProjectTree.Nodes[0].Nodes.Add(TN);
                Scripts[Count] = S;
                ++Count;
            }

            ProjectTree.Nodes[0].Expand();
        }

        private void ProjectFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                //Program.GE.m_ScriptView.ActiveScript.SaveFile();
            }
        }

        private void ProjectTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteToolStripMenuItem_Click(sender, e);
            }
            if (e.KeyCode == Keys.Enter)
            {
                ProjectTree_MouseDoubleClick(sender, null);
            }
        }

        private void ProjectTree_MouseClick(object sender, MouseEventArgs e)
        {
            ProjectTree.SelectedNode = ProjectTree.GetNodeAt(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                if (ProjectTree.SelectedNode.Level >= 1)
                {
                    ScriptContextMenu.Show(ProjectTree, e.Location);
                }
                else
                {
                    ProjectContextMenu.Show(ProjectTree, e.Location);
                }
                //  contextMenuStrip1.();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectTree_MouseDoubleClick(sender, null);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectTree.SelectedNode != null)
            {
                if (ProjectTree.SelectedNode.Level >= 1)
                {
                    ProjectTree.SelectedNode.BeginEdit();
                }
            }
        }

        private void ProjectTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string file = e.Node.Text;
            if (file == e.Label)
            {
                return;
            }
            if (e.Label == null)
            {
                return;
            }
            if (e.Label == "")
            {
                e.CancelEdit = true;
                return;
            }

            string eLabel = e.Label;
            if (e.Label.Length < 4 || !e.Label.Substring(e.Label.Length - 3).Equals(".cs"))
            {
                MessageBox.Show("Invalid extension!");
                e.CancelEdit = true;
                return;
            }

            if (File.Exists(@"Data\Server Data\Scripts\" + eLabel))
            {
                MessageBox.Show("Error: File already exists");
                e.CancelEdit = true;
                return;
            }

            try
            {
                File.Move(@"Data\Server Data\Scripts\" + file,
                          @"Data\Server Data\Scripts\" + eLabel);
            }
            catch (IOException err)
            {
                MessageBox.Show("Error renaming file: " + err);
                e.CancelEdit = true;
                return;
            }

            try
            {
                File.Move(@"Data\Server Data\Scripts\" + file.Substring(0, file.Length - 2) + "designer.cs",
                          @"Data\Server Data\Scripts\" + eLabel.Substring(0, eLabel.Length - 2) + "designer.cs");
            }
            catch (System.Exception)
            {
            }

            try
            {
                File.Move(@"Data\Server Data\Scripts\" + file.Substring(0, file.Length - 2) + "xml",
                          @"Data\Server Data\Scripts\" + eLabel.Substring(0, eLabel.Length - 2) + "xml");
            }
            catch (System.Exception)
            {
            }

            foreach (ScriptEditorForm documentForm in Program.GE.m_ScriptView.ScriptDock.Documents)
            {
                string tfile = file;
                if (documentForm is FormEditor)
                    tfile += " [Designer]";

                if (tfile == documentForm.TabText || tfile + " *" == documentForm.TabText)
                {
                    if (documentForm.Saved == false)
                    {
                        documentForm.TabText = eLabel + ((documentForm is FormEditor) ? " [Designer] *" : " *");
                    }
                    else
                    {
                        documentForm.TabText = eLabel + ((documentForm is FormEditor) ? " [Designer]" : "");
                    }
                    documentForm.ToolTipText = @"Data\Server Data\Scripts\" + eLabel;
                    documentForm.LoadedFileName = eLabel + ((documentForm is FormEditor) ? " [Designer]" : "");
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectTree.SelectedNode.Level >= 1)
            {
                ProjectTree.LabelEdit = false;
                string file = ProjectTree.SelectedNode.Text;
                DialogResult Result =
                    MessageBox.Show("Are you sure you wish to delete: " + this.ProjectTree.SelectedNode.Text + "?",
                                    "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (Result == DialogResult.Yes)
                {
                    //Try and rename
                    try
                    {
                        File.Delete(@"Data\Server Data\Scripts\" + file);
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        File.Delete(@"Data\Server Data\Scripts\" + file.Substring(0, file.Length - 2) + "designer.cs");
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        File.Delete(@"Data\Server Data\Scripts\" + file.Substring(0, file.Length - 2) + "xml");
                    }
                    catch (Exception)
                    {
                    }

                    // Close file if it's open
                    List<ScriptEditorForm> CloseList = new List<ScriptEditorForm>();
                    foreach (ScriptEditorForm documentForm in Program.GE.m_ScriptView.ScriptDock.Documents)
                    {
                        string tfile = file;
                        if (documentForm is FormEditor)
                            tfile += " [Designer]";

                        if (tfile == documentForm.TabText || tfile + " *" == documentForm.TabText)
                        {
                            documentForm.SetSaved(true); // To stop it bugging us
                            documentForm.Hide();
                            CloseList.Add(documentForm);        
                        }
                    }

                    foreach(ScriptEditorForm documentForm in CloseList)
                        Program.GE.m_ScriptView.CloseDocument(documentForm);

                    ProjectTree.Nodes[0].Nodes.Remove(ProjectTree.SelectedNode);
                }
                ProjectTree.LabelEdit = true;
            }
        }

        private void newScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.newToolStripMenuItem_Click(sender, e);
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.SaveAllDocuments();
        }

        private void closeAllOpenScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.CloseAllDocuments();
        }

        private void scriptFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.GE.m_ScriptView.newToolStripMenuItem_Click(sender, e);
// 
//             OpenFileDialog ofn = new OpenFileDialog();
//             ofn.Filter = "All files (*.*)|*.*|C# Script (*.cs)|*.cs";
//             ofn.Title = "Open script from file";
//             ofn.RestoreDirectory = true;
//             if (ofn.ShowDialog() == DialogResult.Cancel)
//                 return;
//             string path = @"Data\Server Data\Scripts\" + Path.GetFileName(ofn.FileName);
//             try
//             {
//                 while (File.Exists(path))
//                 {
//                     TextEntry TE = new TextEntry();
//                     TE.Text = "Script name already exists";
//                     TE.Description.Text = "Enter a new name for this script:";
//                     TE.ShowDialog();
//                     string ScriptName = TE.Result;
//                     if (ScriptName.Length > 4 && !ScriptName.Substring(ScriptName.Length - 3).Equals(".cs", StringComparison.CurrentCultureIgnoreCase))
//                         ScriptName += ".cs";
//                     TE.Dispose();
//                     if (string.IsNullOrEmpty(ScriptName) || ScriptName.Length < 5)
//                         return;
//                     path = @"Data\Server Data\Scripts\" + ScriptName;
//                 }
//                 File.Copy(ofn.FileName, path);
//             }
//             catch (Exception)
//             {
//                 MessageBox.Show("Error opening file", "File Error",
//                                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//                 return;
//             }
//             TreeNode TN = new TreeNode(Path.GetFileNameWithoutExtension(path));
//             TN.ImageIndex = 1;
//             TN.SelectedImageIndex = 1;
//             TN.StateImageIndex = 1;
//             ProjectTree.Nodes[0].Nodes.Add(TN);
//             Program.GE.m_ScriptView.OpenSpecifiedScript(Path.GetFileNameWithoutExtension(path));
        }

        private void viewDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectTree.SelectedNode != null)
            {
                if (ProjectTree.SelectedNode.Level >= 1)
                {
                    bool isOpen = false;
                    foreach (ScriptEditorForm documentForm in Program.GE.m_ScriptView.ScriptDock.Documents)
                    {
                        string file = ProjectTree.SelectedNode.Text + " [Designer]";
                        if (file == documentForm.TabText || file + " *" == documentForm.TabText)
                        {
                            // Some how this works. Selects document
                            documentForm.Hide();
                            documentForm.Show();
                            isOpen = true;
                            break;
                        }
                    }

                    // Open the files
                    if (!isOpen)
                    {
                        if(File.Exists(@"Data\Server Data\Scripts\" + ProjectTree.SelectedNode.Text.Substring(0, ProjectTree.SelectedNode.Text.Length - 2) + "xml"))
                            Program.GE.m_ScriptView.LoadFile(ProjectTree.SelectedNode.Text, true);
                    }
                }
            }
        }
    }
}