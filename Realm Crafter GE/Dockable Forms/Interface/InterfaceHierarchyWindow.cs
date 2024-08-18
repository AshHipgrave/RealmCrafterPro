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
//	Realmcrafter hierarchy window interface
//	Author: Yeisnier Dominguez Silva, March 2009

 namespace RealmCrafter_GE
{
    using System;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using WeifenLuo.WinFormsUI.Docking;
    using RealmCrafter_GE.Dockable_Forms;
    using RealmCrafter_GE.Property_Interfaces;
    using NGUINet;
    using System.ComponentModel;

    
    public partial class InterfaceHierarchyWindows : DockContent
    {
        List<bool> lStateNodeCheck;
        bool bEnabledAfterCheck = true;
        public InterfaceHierarchyWindows()
        {
            lStateNodeCheck = new List<bool>();
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ControlsHierarchy_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ( e.Node.Level == 0)
            {
                Program.GE.m_propertyWindow.ObjectProperties.SelectedObject =
                   e.Node.Tag;
            }
            else
            if (e.Node.Tag is cControl)
            {
                cControl control = (cControl)e.Node.Tag;
                Program.GE.m_propertyWindow.ObjectProperties.SelectedObject = control;

                if ( control.control.Visible == e.Node.Checked )
                    if (control.control.Parent != null) control.control.Parent.Visible = true;
            }
            else
            if (ControlsHierarchy.SelectedNode.Tag is cHierarchyControls && e.Node.Level != 0)
            {
                if (((cHierarchyControls)ControlsHierarchy.SelectedNode.Tag).selfControl != null)
                    Program.GE.m_propertyWindow.ObjectProperties.SelectedObject =
                        ((cHierarchyControls)ControlsHierarchy.SelectedNode.Tag).selfControl;
            }
        }

        void UnCheckNode( TreeNode e )
        {
            lStateNodeCheck.Add(e.Checked);
            e.Checked = false;
            foreach (TreeNode n in e.Nodes) UnCheckNode(n);
        }
        public void UnCheckAllNodes()
        {
            bEnabledAfterCheck = false;

            lStateNodeCheck.Clear();
            if ( ControlsHierarchy.Nodes.Count > 0 )
                UnCheckNode(ControlsHierarchy.Nodes[0]);

            bEnabledAfterCheck = true;
        }

        void RestoreStateNode(TreeNode e)
        {
            e.Checked = lStateNodeCheck[0];
            lStateNodeCheck.RemoveAt(0);
            foreach (TreeNode n in e.Nodes) RestoreStateNode(n);
        }
        public void RestoreStateAllNodes()
        {
            if (ControlsHierarchy.Nodes.Count > 0 && lStateNodeCheck.Count > 0)
                RestoreStateNode(ControlsHierarchy.Nodes[0]);
        }

        private void ControlsHierarchy_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!bEnabledAfterCheck) return;
            if (e.Node.Tag is cControl)
            {
                cControl control = (cControl)e.Node.Tag;
                if (control.control.Parent != null && e.Node.Checked)
                    control.control.Parent.Visible = true;
                control.control.Visible = e.Node.Checked;
            }
            else
                if (e.Node.Tag is cHierarchyControls)
                {
                    if (e.Node.Level != 0)
                    {
                        for (int i = 0; i < e.Node.GetNodeCount(false); i++)
                            e.Node.Nodes[i].Checked = e.Node.Checked;

                        cHierarchyControls h = (cHierarchyControls)e.Node.Tag;
                        if (h.selfControl != null) h.selfControl.control.Visible = e.Node.Checked;
                    }
                }
        }
    }
}