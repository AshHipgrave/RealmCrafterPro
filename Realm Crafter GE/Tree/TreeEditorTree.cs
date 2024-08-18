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
using System.Text;
using System.Windows.Forms;
using System.Xml;
using RenderingServices;
using System.IO;

namespace RealmCrafter_GE
{
    public class TreeEditorTree
    {
        TreeEditorProperties PropertyPanel = null;

        public string Name = "";

        public List<TreeEditorTrunk> Trunks = new List<TreeEditorTrunk>();
        public List<TreeEditorLeafGroup> Leaves = new List<TreeEditorLeafGroup>();
        public List<TreeEditorPhysObj> Physics = new List<TreeEditorPhysObj>();
        public TreeEditorSwayCenter SwayCenter;
        public TreeBoundingBox BoundingBox = new TreeBoundingBox();

        public TreeEditorTree(TreeEditorProperties propertyPanel)
        {
            PropertyPanel = propertyPanel;
            SwayCenter = new TreeEditorSwayCenter();
            PropertyPanel.TreeComponents.Nodes[3].Nodes[0].Tag = SwayCenter;
            SwayCenter.ReBuild();

            Name = "tree1";

//             TreeEditorMeshTrunk T = new TreeEditorMeshTrunk();
//             T.MeshSrc = "Tree\\tree1.b3d";
//             Trunks.Add(T);
// 
//             TreeEditorLeafGroup L = new TreeEditorLeafGroup();
//             Leaves.Add(L);
// 
//             TreeEditorPhysCube C = new TreeEditorPhysCube();
//             Physics.Add(C);
// 
//             TreeEditorPhysSphere S = new TreeEditorPhysSphere();
//             Physics.Add(S);
// 
//             TreeEditorPhysCapsule A = new TreeEditorPhysCapsule();
//             Physics.Add(A);

            

            UpdatePropertyPanel();
        }

        public bool Visible
        {
            set
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }

        public void RecalculateBoundingBox()
        {
            BoundingBox = new TreeBoundingBox();

            foreach (TreeEditorTrunk T in Trunks)
            {
                T.RecalculateBoundingBox(ref BoundingBox);
            }

            foreach (TreeEditorLeafGroup L in Leaves)
            {
                L.RecalculateBoundingBox(ref BoundingBox);
            }

        }

        public void Hide()
        {
            foreach (TreeEditorTrunk T in Trunks)
            {
                T.Hide();
            }

            foreach (TreeEditorLeafGroup L in Leaves)
            {
                L.Hide();
            }

            foreach (TreeEditorPhysObj P in Physics)
            {
                P.Hide();
            }

            SwayCenter.Hide();
        }

        public void Show()
        {
            foreach (TreeEditorTrunk T in Trunks)
            {
                T.Show();
            }

            foreach (TreeEditorLeafGroup L in Leaves)
            {
                L.Show();
            }

            foreach (TreeEditorPhysObj P in Physics)
            {
                P.Show();
            }

            SwayCenter.Show();
        }

        public void AddTrunkMesh(string path)
        {
            TreeEditorMeshTrunk T = new TreeEditorMeshTrunk();
            T.MeshSrc = path;
            Trunks.Add(T);
            UpdatePropertyPanel();
        }

        public void AddLeafGroup()
        {
            TreeEditorLeafGroup L = new TreeEditorLeafGroup();
            Leaves.Add(L);
            UpdatePropertyPanel();
        }

        public void AddPhysCapsule()
        {
            TreeEditorPhysCapsule A = new TreeEditorPhysCapsule();
            Physics.Add(A);
            UpdatePropertyPanel();
        }

        public void AddPhysCube()
        {
            TreeEditorPhysCube C = new TreeEditorPhysCube();
            Physics.Add(C);
            UpdatePropertyPanel();
        }

        public void AddPhysSphere()
        {
            TreeEditorPhysSphere S = new TreeEditorPhysSphere();
            Physics.Add(S);
            UpdatePropertyPanel();
        }

        public void StartLODRender(float angle)
        {
            RecalculateBoundingBox();

            foreach (TreeEditorTrunk T in Trunks)
            {
                T.StartLODRender(angle, ref BoundingBox);
            }

            foreach (TreeEditorLeafGroup L in Leaves)
            {
                L.StartLODRender(angle, ref BoundingBox);
            }

            foreach (TreeEditorPhysObj P in Physics)
            {
                P.Hide();
            }

            SwayCenter.Hide();
        }

        public void EndLODRender()
        {
            foreach (TreeEditorTrunk T in Trunks)
            {
                T.EndLODRender();
            }

            foreach (TreeEditorLeafGroup L in Leaves)
            {
                L.EndLODRender();
            }

            foreach (TreeEditorPhysObj P in Physics)
            {
                P.Show();
            }

            SwayCenter.Show();
        }

        public void Unload()
        {
            foreach (TreeEditorTrunk T in Trunks)
            {
                T.Remove();
            }

            foreach (TreeEditorLeafGroup L in Leaves)
            {
                L.Remove();
            }

            foreach (TreeEditorPhysObj P in Physics)
            {
                P.Remove();
            }

            Trunks.Clear();
            Leaves.Clear();
            Physics.Clear();
        }

        public void Load(string path)
        {
            Program.GE.TreeRender.SupressSave = true;

            XmlTextReader X = null;

            try
            {
                X = new XmlTextReader(path);
                Unload();

                TreeEditorLeafGroup LastLeafGroup = null;

                while (X.Read())
                {
                    // Is root
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("tree", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Name = X.GetAttribute("name");
                    }

                    // Is a trunk
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("trunkmesh", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TreeEditorMeshTrunk T = new TreeEditorMeshTrunk();
                        T.Read(X);
                        Trunks.Add(T);
                    }

                    // Is a leaf group
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("leafgroup", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TreeEditorLeafGroup L = new TreeEditorLeafGroup();
                        L.Read(X);
                        Leaves.Add(L);
                        LastLeafGroup = L;
                    }

                    // Is a leaf group
                    if (X.NodeType == XmlNodeType.EndElement && X.Name.Equals("leafgroup", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (LastLeafGroup != null)
                            LastLeafGroup.UpdateScale(LastLeafGroup.Scale, 0, 0);
                        LastLeafGroup = null;
                    }

                    // Is a leaf
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("leaf", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (LastLeafGroup != null)
                        {
                            TreeEditorLeaf L = new TreeEditorLeaf(LastLeafGroup);
                            L.Read(X);
                            LastLeafGroup.Leaves.Add(L);
                        }
                    }

                    // Is a physics capsule
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("physcapsule", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TreeEditorPhysCapsule P = new TreeEditorPhysCapsule();
                        P.Read(X);
                        Physics.Add(P);
                    }

                    // Is a physics sphere
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("physsphere", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TreeEditorPhysSphere P = new TreeEditorPhysSphere();
                        P.Read(X);
                        Physics.Add(P);
                    }

                    // Is a physics cube
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("physcube", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TreeEditorPhysCube P = new TreeEditorPhysCube();
                        P.Read(X);
                        Physics.Add(P);
                    }

                    // Is a sway center
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("swaycenter", StringComparison.CurrentCultureIgnoreCase))
                    {
                        SwayCenter.Read(X);
                    }
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "LoadShaders", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                X.Close();
            }

            UpdatePropertyPanel();
            RecalculateBoundingBox();
            Program.GE.TreeRender.SupressSave = false;
            Program.GE.TreeRender.Saved = true;
        }

        public void Save(string path, string outputDir)
        {
//             try
//             {
                // Build tree directory structure
                Directory.CreateDirectory(outputDir);

                // Build LODS
                Program.GE.TreeRender.CreateLOD(outputDir + Name + "_lod0.png", 0);
                Program.GE.TreeRender.CreateLOD(outputDir + Name + "_lod1.png", 1);

                // Write export file
                BinaryWriter Writer = new BinaryWriter(File.Open(outputDir + Name + ".lt", FileMode.Create));

                // Write tree header
                Writer.Write(Convert.ToByte(1));
                Writer.Write(Convert.ToByte(2));
                Program.WriteString(Writer, Name);

                SwayCenter.Export(Writer, outputDir, Name, 0);

                Writer.Write(Convert.ToInt32(Physics.Count));
                int Idx = 0;
                foreach (TreeEditorPhysObj P in Physics)
                {
                    P.Export(Writer, outputDir, Name, Idx);
                }

                Writer.Write(Convert.ToByte(2));
                Program.WriteString(Writer, Name + "_lod0.png");
                Program.WriteString(Writer, Name + "_lod1.png");

                Writer.Write(Convert.ToInt32(Trunks.Count));
                Idx = 0;
                foreach (TreeEditorTrunk T in Trunks)
                {
                    T.Export(Writer, outputDir, Name, Idx);
                    ++Idx;
                }

                Writer.Write(Convert.ToInt32(Leaves.Count));
                Idx = 0;
                foreach (TreeEditorLeafGroup L in Leaves)
                {
                    L.Export(Writer, outputDir, Name, Idx);
                    ++Idx;
                }

                Writer.Close();


                // Write XML data
                XmlTextWriter X = new XmlTextWriter(path, Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                X.WriteStartDocument();
                X.WriteStartElement("tree");
                X.WriteAttributeString("name", Name);

                foreach (TreeEditorTrunk T in Trunks)
                {
                    T.Write(X);
                }

                foreach (TreeEditorLeafGroup L in Leaves)
                {
                    L.Write(X);
                }

                foreach (TreeEditorPhysObj P in Physics)
                {
                    P.Write(X);
                }

                SwayCenter.Write(X);

                X.WriteEndElement();
                X.WriteEndDocument();
                X.Flush();
                X.Close();

                Program.GE.TreeRender.Saved = true;
//             }
//             catch (System.Exception e)
//             {
//                 MessageBox.Show("Error: " + e.Message, "SaveTree", MessageBoxButtons.OK, MessageBoxIcon.Error);
//             }

        }

        private TreeNode UpdateTreeNode(TreeNode node, object tagObject)
        {
            if (node.Tag == tagObject)
            {
                node.Text = tagObject.ToString();
                return node;
            }

            foreach (TreeNode N in node.Nodes)
            {
                TreeNode T = UpdateTreeNode(N, tagObject);
                if (T != null)
                    return T;
            }

            return null;
        }

        public void UpdateNode(object tagObject)
        {
            UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[0], tagObject);
            UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[1], tagObject);
            UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[2], tagObject);
            UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[3], tagObject);
        }

        public void Remove(TreePropertiesUpdater obj)
        {
            if (obj == null || obj == SwayCenter)
                return;

            TreeNode T = UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[0], obj);
            if (T == null)
            {
                T = UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[1], obj);
                if (T == null)
                {
                    T = UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[2], obj);
                    if (T == null)
                    {
                        T = UpdateTreeNode(PropertyPanel.TreeComponents.Nodes[3], obj);
                        if (T == null)
                            return;
                    }
                }
            }

            T.Parent.Nodes.Remove(T);

            Trunks.Remove(obj as TreeEditorTrunk);
            Leaves.Remove(obj as TreeEditorLeafGroup);
            Physics.Remove(obj as TreeEditorPhysObj);

            obj.Remove();

        }

        public void UpdatePropertyPanel()
        {
            TreeNode TrunkRoot = PropertyPanel.TreeComponents.Nodes[0];
            TreeNode LeafRoot = PropertyPanel.TreeComponents.Nodes[1];
            TreeNode PhysicsRoot = PropertyPanel.TreeComponents.Nodes[2];

            TrunkRoot.Nodes.Clear();
            LeafRoot.Nodes.Clear();
            PhysicsRoot.Nodes.Clear();

            if (PropertyPanel.SelectedNode != null && (PropertyPanel.SelectedNode.Tag as TreePropertiesUpdater) != null)
                (PropertyPanel.SelectedNode.Tag as TreePropertiesUpdater).NotifyUnselected();
            PropertyPanel.SelectedNode = null;

            foreach (TreeEditorTrunk T in Trunks)
            {
                TreeNode Node = new TreeNode(T.ToString());
                Node.Tag = T;
                TrunkRoot.Nodes.Add(Node);
            }

            foreach (TreeEditorLeafGroup L in Leaves)
            {
                TreeNode LParent = new TreeNode(L.ToString());
                LParent.Tag = L;
                LeafRoot.Nodes.Add(LParent);

                foreach (TreeEditorLeaf I in L.Leaves)
                {
                    TreeNode Node = new TreeNode(I.ToString());
                    Node.Tag = I;
                    LParent.Nodes.Add(Node);
                }

                foreach (TreeEditorLeafBox I in L.Placers)
                {
                    TreeNode Node = new TreeNode(I.ToString());
                    Node.Tag = I;
                    LParent.Nodes.Add(Node);
                }
            }

            foreach (TreeEditorPhysObj P in Physics)
            {
                TreeNode Node = new TreeNode(P.ToString());
                Node.Tag = P;
                PhysicsRoot.Nodes.Add(Node);
            }

        }


    }
}
