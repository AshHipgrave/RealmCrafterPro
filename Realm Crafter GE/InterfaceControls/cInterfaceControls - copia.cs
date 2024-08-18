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

        void SaveHierarchy(XmlTextWriter X, cHierarchyControls h, string p)
        {
            bool isElement = (h.selfControl == null);
            string parentName = (p.Length <= 0 || p == Name) ? h.Name : (p + "::" + h.Name);
            if (p.Length <= 0) 
            { 
                X.WriteStartElement("interfaces");
                X.WriteStartAttribute("set");
                X.WriteString(parentName);
                X.WriteEndAttribute();

                isElement = true; 
            }
            else
            if (isElement)
            {
                X.WriteStartElement("interface");

                X.WriteStartAttribute("set");
                X.WriteString(parentName);
                X.WriteEndAttribute();
            }
            else
                h.selfControl.SaveControl(X, parentName);

            for (int i = 0; i < h.lpControls.Count; i++)
                h.lpControls[i].SaveControl(X, parentName);

            foreach (cHierarchyControls hc in h.lpHierarchyControls)
                SaveHierarchy(X, hc, parentName);

            if ( isElement ) X.WriteEndElement();
        }
        public void SaveHierarchy( XmlTextWriter X )
        {
            SaveHierarchy(X, this, "");
        }
        
        public void VisibleControls(bool visible)
        {
            VisibleControls(this, visible);
        }
        void VisibleControls(cHierarchyControls HierarchyControl, bool visible)
        {
            if (HierarchyControl.selfControl != null) HierarchyControl.selfControl.control.Visible = visible;
            foreach (cControl c in HierarchyControl.lpControls) c.control.Visible = visible;
            foreach (cHierarchyControls h in HierarchyControl.lpHierarchyControls)
                VisibleControls(h, visible);
        }
    }

    public partial class cInterfaceControls
    {
        cHierarchyControls root, currentHierarchy, prevHierarchy;

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

        cHierarchyControls FindHierarchyParent(string[] aTokens)
        {
            cHierarchyControls parentHierarchy = currentHierarchy;
            for (int i = 0; i < aTokens.Length - 1; i++)
            {
                string t = aTokens[i];
                if (t == parentHierarchy.Name) continue;
                if (HasHierarchy(root, t, ref parentHierarchy)) continue;

                parentHierarchy = new cHierarchyControls(t);
                if (i >= 1)
                {
                    cHierarchyControls parent = root;
                    foreach (cHierarchyControls h in root.lpHierarchyControls)
                        if (h.Name == aTokens[i - 1]) { parent = h; break; }

                    parent.lpHierarchyControls.Add( parentHierarchy );
                }
                else
                    root.lpHierarchyControls.Add(parentHierarchy);
            }

            return parentHierarchy;
        }

        bool HasControlType( ref string controlName, string type )
        {
            type = type.ToLower();
            if ( controlName.Length <= type.Length ||
                 !controlName.ToLower().Contains(type) ) return false;

            string testType = controlName.ToLower().Substring(controlName.Length - type.Length, type.Length);
            if (testType == type)
            {
                controlName = controlName.Remove(controlName.Length - type.Length, type.Length);
                return true;
            }

            return false;
        }
        cControl CreateControl(ref string controlName, XmlTextReader X)
        {
            cControl control = null;
            if (HasControlType(ref controlName, "button"))
                control = new cButton(controlName);
            else
            if (HasControlType(ref controlName, "label"))
                control = new cLabel(controlName);
            else
            if (HasControlType(ref controlName, "checkbox"))
                control = new cCheckBox(controlName);
            else
            if (HasControlType(ref controlName, "combobox"))
                control = new cComboBox(controlName);
            else
            if (HasControlType(ref controlName, "trackbar"))
                control = new cTrackBar(controlName);
            else
            if (HasControlType(ref controlName, "listbox"))
                control = new cListBox(controlName);
            else
            if (HasControlType(ref controlName, "textbox"))
                control = new cTextBox(controlName);
            else
            if (HasControlType(ref controlName, "scrollbar"))
                control = new cTextBox(controlName);
            else
            if (HasControlType(ref controlName, "progressbar"))
                control = new cProgressBar(controlName);
            else
            if (HasControlType(ref controlName, "image"))
                control = new cImage(controlName);
            else 
            if (HasControlType(ref controlName, "window"))
                control = new cWindow(controlName);

            if (control != null) control.LoadControl(X);
            return control;
        }
        void CreateControl(XmlTextReader X)
        {
            string controlName = X.GetAttribute("name");

            string separator = "::";
            string[] aTokens = controlName.Split(separator.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            controlName = aTokens[aTokens.Length-1];
            cHierarchyControls HierarchyForAddControl = FindHierarchyParent(aTokens);

            cControl control = CreateControl(ref controlName, X);
            if (control != null)
                if (control is cWindow)
                {
                    cHierarchyControls newHierarchy = new cHierarchyControls(controlName);
                    newHierarchy.selfControl = control;
                    HierarchyForAddControl.lpHierarchyControls.Add(newHierarchy);
                }
                else
                {
                    if (HierarchyForAddControl.selfControl != null)
                        control.control.Parent = HierarchyForAddControl.selfControl.control;
                    HierarchyForAddControl.lpControls.Add(control);
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
            RootNode.Tag = root;

            AddControls(RootNode, root);
        }

        public void Show() { root.VisibleControls(true); }
        public void Hide() { root.VisibleControls(false); }

        public void LoadFromXML(string xmlName)
        {
            // Reader
            XmlTextReader X = null;
            try
            {
                // Open file
                X = new XmlTextReader(xmlName);

                // Read next element
                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "interfaces")
                    {
                        root = new cHierarchyControls(X.GetAttribute("set"));
                        currentHierarchy = prevHierarchy = root;
                    }
                    else
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "interface")
                    {
                        prevHierarchy = currentHierarchy;
                        currentHierarchy = new cHierarchyControls(X.GetAttribute("set"));
                        prevHierarchy.lpHierarchyControls.Add(currentHierarchy);
                    }
                    else
                    if (X.NodeType == XmlNodeType.EndElement && X.Name.ToLower() == "interface")
                        currentHierarchy = prevHierarchy;
                    else
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "component")
                        CreateControl(X);
                }
            }
            catch (Exception except)
            {
                // Eek an error
                MessageBox.Show("Error: " + except.Message, "cInterfaceControls:LoadFromXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
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