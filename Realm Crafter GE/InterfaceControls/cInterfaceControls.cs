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
/* 
 * This interface controls manager
 * Author: Yeisnier Dominguez Silva, March 2009
 */

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;
using NGUINet;
using System.Collections.Generic;
using System.Xml;
using RealmCrafter_GE.Property_Interfaces;

using NGUINet;

namespace RealmCrafter_GE
{
    struct cHierarchyControls
    {
        public string Name;
        public cControl selfControl;
        public List<cControl> lpControls;
        public List<cHierarchyControls> lpHierarchyControls;

        public cHierarchyControls(string interfaceName)
        {
            Name = interfaceName;
            selfControl = null;
            lpControls = new List<cControl>();
            lpHierarchyControls = new List<cHierarchyControls>();
        }

        void SaveHierarchy(XmlTextWriter X, cHierarchyControls h, bool isRoot)
        {
            bool isElement = (h.selfControl == null);
            if (isRoot) 
            { 
                X.WriteStartElement("interfaces");
                X.WriteStartAttribute("set");
                X.WriteString(h.Name);
                X.WriteEndAttribute();

                isElement = true; 
            }
            else
            if (isElement)
            {
                X.WriteStartElement("interface");

                X.WriteStartAttribute("set");
                X.WriteString(h.Name);
                X.WriteEndAttribute();
            }
            else
                h.selfControl.SaveControl(X);

            for (int i = 0; i < h.lpControls.Count; i++) h.lpControls[i].SaveControl(X);

            foreach (cHierarchyControls hc in h.lpHierarchyControls)
                SaveHierarchy(X, hc, false);

            if ( isElement ) X.WriteEndElement();
        }
        public void SaveHierarchy( XmlTextWriter X )
        {
            SaveHierarchy(X, this, true);
        }
        
        public void VisibleControls(bool visible)
        {
            VisibleControls(this, visible);
        }
        void VisibleControls(cHierarchyControls HierarchyControl, bool visible)
        {
            if (HierarchyControl.selfControl != null) HierarchyControl.selfControl.control.Visible = visible;
            foreach (cControl c in HierarchyControl.lpControls)  c.control.Visible = visible;
            foreach (cHierarchyControls h in HierarchyControl.lpHierarchyControls)
                VisibleControls(h, visible);
        }
    }

    public partial class cInterfaceControls
    {
        cHierarchyControls root;

        // find Hierarchy Parent
        bool HasHierarchy(cHierarchyControls Hierarchy, string Name, ref cHierarchyControls selfHierarchy)
        {
            foreach (cHierarchyControls h in Hierarchy.lpHierarchyControls)
            {
                if (h.Name == Name) { selfHierarchy = h; return true; }
                if (HasHierarchy(h, Name, ref selfHierarchy)) return true;
            }

            return false;
        }
        cControl CreateControl(string controlName, string toSaveName, string toSaveParent, string controlType, XmlTextReader X)
        {
            cControl control = null;
            if (controlType == "button")
                control = new cButton(controlName, toSaveName, toSaveParent);
            else
            if (controlType == "picturebox")
                control = new cPictureBox(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "label")
                control = new cLabel(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "checkbox")
                control = new cCheckBox(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "combobox")
                control = new cComboBox(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "trackbar")
                control = new cTrackBar(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "listbox")
                control = new cListBox(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "textbox")
                control = new cTextBox(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "scrollbar")
                control = new cTextBox(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "progressbar")
                control = new cProgressBar(controlName, toSaveName, toSaveParent);
            else
            if ( controlType == "image")
                control = new cImage(controlName, toSaveName, toSaveParent);
            else
            if (controlType == "radar")
                control = new cRadar(controlName, toSaveName, toSaveParent);
            else
            if (controlType == "window")
                control = new cWindow(controlName, toSaveName, toSaveParent);

            if (control != null) control.LoadControl(X);
            return control;
        }
        void CreateControl(XmlTextReader X)
        {
            string controlName = X.GetAttribute("name");
            string controlType = X.GetAttribute("type");
            string controlParent = X.GetAttribute("parent");

            cHierarchyControls parentHierarchy = root;
            HasHierarchy(root, controlParent, ref parentHierarchy);

            string separator = "::";
            string[] aTokens = controlName.Split(separator.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            string name = aTokens[aTokens.Length - 1];
            cControl control = CreateControl(name, controlName, controlParent, controlType, X);

            if (control != null)
                if (control is cWindow || control is cPictureBox)
                {
                    cHierarchyControls newHierarchy = new cHierarchyControls(controlName);
                    newHierarchy.selfControl = control;
                    parentHierarchy.lpHierarchyControls.Add(newHierarchy);
                }
                else
                {
                    if (parentHierarchy.selfControl != null)
                        control.control.Parent = parentHierarchy.selfControl.control;
                    parentHierarchy.lpControls.Add(control);
                }
        }

        void AddControls(TreeNode HierarchyNode, cHierarchyControls HierarchyControl )
        {
            TreeNode newHierarchyNode, ControlNode;
            foreach (cHierarchyControls h in HierarchyControl.lpHierarchyControls)
            {
                newHierarchyNode = HierarchyNode.Nodes.Add(h.Name);
                newHierarchyNode.Tag = h;
                foreach (cControl c in h.lpControls)
                {
                    ControlNode = newHierarchyNode.Nodes.Add(c.control.Name);
                    ControlNode.Tag = c;
                }

                AddControls(newHierarchyNode, h);
            }
        }
        public void AddToTreeView(TreeView tvHierarchy)
        {
            TreeNode RootNode, HierarchyNode, ControlNode;

            RootNode = tvHierarchy.Nodes.Add(root.Name);
            RootNode.Tag = new cMainInterface();

            AddControls(RootNode, root);
        }

        public void Show() { root.VisibleControls(true); }
        public void Hide() { root.VisibleControls(false); }

        public void LoadFromXML(string xmlName)
        {
            // Reader
            XmlTextReader X = null;
            //try
            {
                // Open file
                X = new XmlTextReader(xmlName);

                // Read next element
                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "interfaces")
                        root = new cHierarchyControls(X.GetAttribute("set"));
                    else
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "interface")
                        root.lpHierarchyControls.Add( new cHierarchyControls(X.GetAttribute("set")) );
                    else
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "component")
                        CreateControl(X);
                }
            }
            //catch (Exception except)
            //{
            //    // Eek an error
            //    MessageBox.Show("Error: " + except.Message, "cInterfaceControls:LoadFromXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //}
            //finally
            {
                // Cleanup
                X.Close();
            }
        }
        public void SaveToXML(string xmlName)
        {
            XmlTextWriter X = null;
            try
            {
                // Create file
                X = new XmlTextWriter(xmlName, Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                // Base parts
                X.WriteStartDocument();
                root.SaveHierarchy(X);
                X.WriteEndDocument();
            }
            catch (Exception except)
            {
                // Eek an error
                MessageBox.Show("Error: " + except.Message, "cInterfaceControls:SaveToXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                X.Flush();
                X.Close();
            }
        }
    }
}