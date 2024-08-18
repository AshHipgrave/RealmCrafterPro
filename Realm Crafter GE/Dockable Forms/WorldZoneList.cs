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
using RealmCrafter.ClientZone;
using RealmCrafter.ServerZone;
using RenderingServices;
using RottParticles;
using WeifenLuo.WinFormsUI.Docking;
using Emitter=RealmCrafter.ClientZone.Emitter;
using System.ComponentModel;

namespace RealmCrafter_GE
{
    /// <summary>
    /// Realmcrafter World zone list form for use in WinForms dockable interface
    /// August 2008
    /// Author: Shane smith
    /// </summary>
    public partial class WorldZoneList : DockContent
    {
        public WorldZoneList()
        {
            InitializeComponent();
        }

        private void WorldZoneList_Load(object sender, EventArgs e)
        {
        }

        public void AddObjectsCount()
        {
            WorldZonesTree.Nodes[0].Text = "Scenery objects (" + WorldZonesTree.Nodes[0].Nodes.Count + ")";
            WorldZonesTree.Nodes[1].Text = "Terrains (" + WorldZonesTree.Nodes[1].Nodes.Count + ")";
            WorldZonesTree.Nodes[2].Text = "Emitters (" + WorldZonesTree.Nodes[2].Nodes.Count + ")";
            WorldZonesTree.Nodes[3].Text = "Water areas (" + WorldZonesTree.Nodes[3].Nodes.Count + ")";
            WorldZonesTree.Nodes[4].Text = "Collision boxes (" + WorldZonesTree.Nodes[4].Nodes.Count + ")";
            WorldZonesTree.Nodes[5].Text = "Sound zones (" + WorldZonesTree.Nodes[5].Nodes.Count + ")";
            WorldZonesTree.Nodes[6].Text = "Dynamic lights (" + WorldZonesTree.Nodes[6].Nodes.Count + ")";
            WorldZonesTree.Nodes[7].Text = "Triggers (" + WorldZonesTree.Nodes[7].Nodes.Count + ")";
            WorldZonesTree.Nodes[8].Text = "Waypoints (" + WorldZonesTree.Nodes[8].Nodes.Count + ")";
            WorldZonesTree.Nodes[9].Text = "Portals (" + WorldZonesTree.Nodes[9].Nodes.Count + ")";
            WorldZonesTree.Nodes[10].Text = "Tree Placers (" + WorldZonesTree.Nodes[10].Nodes.Count + ")";
            WorldZonesTree.Nodes[11].Text = "Trees (" + WorldZonesTree.Nodes[11].Nodes.Count + ")";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void ZoneObjectListCheck(ZoneObject Obj, bool Checked)
        {
            if (Obj is Scenery)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[0]);
            }
            else if (Obj is RCTTerrain)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[1]);
            }
            else if (Obj is Emitter)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[2]);
            }
            else if (Obj is Water)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[3]);
            }
            else if (Obj is ColBox)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[4]);
            }
            else if (Obj is SoundZone)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[5]);
            }
            else if (Obj is RealmCrafter.ClientZone.Light)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[6]);
            }
            else if (Obj is RealmCrafter.ClientZone.MenuControl)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[12]);
            }
            else if (Obj is Trigger)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[7]);
            }
            else if (Obj is Waypoint)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[8]);
            }
            else if (Obj is Portal)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[9]);
            }
            else if (Obj is Tree)
            {
                if (Checked)
                    Program.GE.ZoneSelected.Add(Obj);
                else
                    Program.GE.ZoneSelected.Remove(Obj);
            }
            else if (Obj is TreePlacerNode)
            {
                ZoneObjectNodeCheck(Obj, Checked, (Obj as TreePlacerNode).Parent.TN);
            }
        }

        public void ZoneObjectListCheckCurentNode(ZoneObject Obj, bool Checked)
        {
            if (Obj is Scenery)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[0]);
            }
            else if (Obj is RCTTerrain)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[1]);
            }
            else if (Obj is Emitter)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[2]);
            }
            else if (Obj is Water)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[3]);
            }
            else if (Obj is ColBox)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[4]);
            }
            else if (Obj is SoundZone)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[5]);
            }
            else if (Obj is RealmCrafter.ClientZone.Light)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[6]);
            }
            else if (Obj is RealmCrafter.ClientZone.MenuControl)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[12]);
            }
            else if (Obj is Trigger)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[7]);
            }
            else if (Obj is Waypoint)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[8]);
            }
            else if (Obj is Portal)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[9]);
            }
            else if (Obj is Tree)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[11]);
                //                 if (Checked)
                //                     Program.GE.ZoneSelected.Add(Obj);
                //                 else
                //                     Program.GE.ZoneSelected.Remove(Obj);
            }
            else if (Obj is TreePlacerNode)
            {
                ZoneObjectNodeCheck(Obj, Checked, (Obj as TreePlacerNode).Parent.TN);
            }
        }

        public void ZoneObjectListCheckLastSelection(ZoneObject Obj, bool Checked) // add new (Marian Voicu)
        {
            if (Obj is Scenery)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[0]);
            }
            else if (Obj is RCTTerrain)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[1]);
            }
            else if (Obj is Emitter)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[2]);
                Collision.EntityPickMode(Obj.EN, PickMode.Sphere);
                return;
            }
            else if (Obj is Water)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[3]);
            }
            else if (Obj is ColBox)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[4]);
            }
            else if (Obj is SoundZone)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[5]);
            }
            else if (Obj is RealmCrafter.ClientZone.Light)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[6]);
            }
            else if (Obj is RealmCrafter.ClientZone.MenuControl)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[12]);
            }
            else if (Obj is Trigger)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[7]);
            }
            else if (Obj is Waypoint)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[8]);
            }
            else if (Obj is Portal)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[9]);
            }
            else if (Obj is Tree)
            {
                ZoneObjectNodeCheck(Obj, Checked, WorldZonesTree.Nodes[11]);
                //                 if (Checked)
                //                     Program.GE.ZoneSelected.Add(Obj);
                //                 else
                //                     Program.GE.ZoneSelected.Remove(Obj);
            }
            else if (Obj is TreePlacerNode)
            {
                ZoneObjectNodeCheck(Obj, Checked, (Obj as TreePlacerNode).Parent.TN);
            }

            if (Obj != null && Obj.EN != null)
                Collision.EntityPickMode(Obj.EN, PickMode.Polygon);
        }

        public void ZoneObjectListRemove(ZoneObject Obj)
        {
            if (Obj as Scenery != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[0]);
            }
            else if (Obj as RCTTerrain != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[1]);
            }
            else if (Obj as Emitter != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[2]);
            }
            else if (Obj as Water != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[3]);
            }
            else if (Obj as ColBox != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[4]);
            }
            else if (Obj as SoundZone != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[5]);
            }
            else if (Obj as RealmCrafter.ClientZone.Light != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[6]);
            }
            else if (Obj as RealmCrafter.ClientZone.MenuControl != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[12]);
            }
            else if (Obj as Trigger != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[7]);
            }
            else if (Obj as Waypoint != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[8]);
            }
            else if (Obj as Portal != null)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[9]);
            }
            else if (Obj is Tree)
            {
                ZoneObjectNodeRemove(Obj, WorldZonesTree.Nodes[11]);
                //Program.GE.ZoneSelected.Remove(Obj);
            }
            else if (Obj is TreePlacerNode)
            {
                ZoneObjectNodeRemove(Obj, (Obj as TreePlacerNode).Parent.TN);
            }
        }

        private void ZoneObjectNodeCheck(ZoneObject Obj, bool Checked, TreeNode Root)
        {
            for (int i = 0; i < Root.Nodes.Count; ++i)
            {
                if (Root.Nodes[i].Tag == Obj)
                {
                    Root.Nodes[i].Checked = Checked;
                    break;
                }
            }
        }

        private void ZoneObjectNodeRemove(ZoneObject Obj, TreeNode Root)
        {
            for (int i = 0; i < Root.Nodes.Count; ++i)
            {
                if (Root.Nodes[i].Tag == Obj)
                {
                    Root.Nodes[i].Remove();
                    break;
                }
            }
        }

        private void WorldZonesTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Program.GE.lastNodeSelect = Program.GE.nodeSelect;
            Program.GE.nodeSelect = e.Node.Index;

            if (e.Node.Level == 0) // level 2
            {
                if (e.Action.ToString() == "Expand")
                {
                    return;
                }

                for (int i = 0; i < e.Node.GetNodeCount(false); i++)
                {
                    e.Node.Nodes[i].Checked = e.Node.Checked;
                }

                return;
            }
            else // level 2
            {
                ZoneObject Obj = (ZoneObject) e.Node.Tag;
                if (e.Node.Checked && Obj != null)
                {
                    if (!Program.GE.ZoneSelected.Contains(Obj))
                    {
                        Program.GE.ZoneSelected.Add(Obj);
                        Program.GE.AddSelectionBox(Obj.EN);
                        if (Obj is Emitter)
                        {
                            Collision.EntityPickMode(Obj.EN, PickMode.Sphere);
                        }
                        else
                        {
                            Collision.EntityPickMode(Obj.EN, PickMode.Polygon);
                        }
                        if (!Program.GE.SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Select, Obj);
                        }
                    }
                }
                else if(Obj != null)
                {
                    Program.GE.ZoneSelected.Remove(Obj);
                    Program.GE.ClearSelectionBox(Obj.EN);
                    if (Obj is Emitter)
                    {
                        Collision.EntityPickMode(Obj.EN, PickMode.Sphere);
                    }
                    else
                    {
                        Collision.EntityPickMode(Obj.EN, PickMode.Polygon);
                    }
                    if (!Program.GE.SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Unselect, Obj);
                    }
                }

                //Program.GE.ZoneUpdateSetupTab();
                Program.GE.WorldSelectedObjectsLabel.Text = "Selected objects: " +
                                                            Program.GE.ZoneSelected.Count.ToString();
                Program.GE.RenderingPanel.Focus();
                return;
            }
        }

        private void WorldZonesTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0) // level 2
            {
                if (e.Action.ToString() == "Expand")
                {
                    return;
                }

                for (int i = 0; i < e.Node.GetNodeCount(false); i++)
                {
                    e.Node.Nodes[i].Checked = e.Node.Checked;
                }

                return;
            }
            else // level 2
            {
                ZoneObject Obj = (ZoneObject) e.Node.Tag;
                if (e.Node.Checked)
                {
                    if (!Program.GE.ZoneSelected.Contains(Obj))
                    {
                        Program.GE.ZoneSelected.Add(Obj);
                        Program.GE.AddSelectionBox(Obj.EN);
                        if (!Program.GE.SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Select, Obj);
                        }
                    }
                }
                else
                {
                    Program.GE.ZoneSelected.Remove(Obj);
                    Program.GE.ClearSelectionBox(Obj.EN);
                    if (!Program.GE.SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Unselect, Obj);
                    }
                }

                //Program.GE.ZoneUpdateSetupTab();
                Program.GE.m_ZoneRender.Focus();
                Program.GE.WorldSelectedObjectsLabel.Text = "Selected objects: " +
                                                            Program.GE.ZoneSelected.Count.ToString();
                Program.GE.m_propertyWindow.RefreshObjectWindow();

                if (Program.GE.ZoneSelected.Count != 1 || Program.GE.ZoneSelected[0].GetType() != typeof(RCTTerrain))
                {
                    Program.GE.m_TerrainEditor.RefreshEditorButton(0);
                }
                else
                {
                    Program.GE.m_TerrainEditor.RefreshEditorButton(1);
                }

                return;
            }
        }

        private void goToObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WorldZonesTree.SelectedNode.Level >= 1)
            {
                TreeNode GotoObjectNode;
                if (WorldZonesTree.SelectedNode.Level >= 1)
                {
                    GotoObjectNode = WorldZonesTree.SelectedNode;
                    ZoneObject Obj = (ZoneObject)GotoObjectNode.Tag;
                    if (Obj.EN != null)
                    {
                        Program.GE.m_ZoneRender.PositionCamera(Obj.EN.X(), Obj.EN.Y() + 250, Obj.EN.Z() - 75);
                        Program.GE.Camera.Point(Obj.EN);
                    }
                }
            }
        }

        private void WorldZonesTree_MouseClick(object sender, MouseEventArgs e)
        {
            WorldZonesTree.SelectedNode = WorldZonesTree.GetNodeAt(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                if (WorldZonesTree.SelectedNode.Level >= 1)
                {
                    ObjectMenuStrip.Show(WorldZonesTree, e.Location);
                }
            }
        }

        private void deleteObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WorldZonesTree.SelectedNode.Level >= 1)
            {
                TreeNode GotoObjectNode = WorldZonesTree.SelectedNode;
                ZoneObject Obj = (ZoneObject)GotoObjectNode.Tag; ;
                // Add to list of deleted objects in undo info
                //UndoList.AddLast(Obj);
                // Remove from selection list
                Program.GE.ClearSelectionBox(Obj.EN);
                Program.GE.m_ZoneList.ZoneObjectListRemove(Obj);
                Program.GE.m_ZoneList.AddObjectsCount();
                // Free object type specific stuff
                if (Obj is Scenery)
                {
                    Scenery S = Obj as Scenery;
                    if (S.SceneryID > 0)
                    {
                        Program.GE.CurrentServerZone.Instances[0].OwnedScenery[
                            S.SceneryID - 1] = null;
                    }
                }
                else if (Obj is TreePlacerArea)
                {
                    (Obj as TreePlacerArea).Remove();
                }
                else if (Obj is RCTTerrain)
                {
                    RCTTerrain T = (RCTTerrain)Obj;
                    GE.TerrainManager.Destroy(T.Terrain);

                    try
                    {
                        System.IO.File.Delete(T.Path);
                    }
                    catch
                    {
                    }

                    T.Path = "";
                    if (Program.GE.m_TerrainEditor.Terrain == T.Terrain)
                    {
                        if (Program.GE.m_TerrainEditor.EditorMode)
                            Program.GE.m_TerrainEditor.EditorModeButton_Click(null, EventArgs.Empty);
                        //Program.GE.m_TerrainEditor.Terrain = null;
                        //Program.GE.m_TerrainEditor.EditorMode = false;
                        //Program.GE.m_TerrainEditor.RefreshEditorButton(0);
                    }
                    T.Terrain = null;
                }
                else if (Obj is Tree)
                {
                    Tree T = Obj as Tree;
                    T.Instance.Destroy();

                    if (Program.Transformer != null)
                    {
                        Program.Transformer.Free();
                        Program.Transformer = null;
                    }
                }
                else if (Obj is Emitter)
                {
                    General.FreeEmitter(Obj.EN.GetChild(1), true, false);
                }
                else if (Obj is Water)
                {
                    Water W = (Water)Obj;
                    WaterArea.WaterList.Remove(W.ServerWater);
                }
                else if (Obj is Trigger)
                {
                    Trigger T = Obj as Trigger;
                    T.Script = "";

                    if (T.Label != null)
                    {
                        GE.GUIManager.Destroy(T.Label);
                        T.Label = null;
                    }

                    Program.GE.CurrentServerZone.Triggers.Remove(T);
                }
                else if (Obj is Waypoint)
                {
                    Waypoint W = Obj as Waypoint;


                    if (W.Label != null)
                    {
                        GE.GUIManager.Destroy(W.Label);
                        W.Label = null;
                    }

                    // Find waypoints connected to this one, and remove the connections
                    for (int j = 0; j < Program.GE.CurrentServerZone.Waypoints.Count; ++j)
                    {
                        Waypoint jW = Program.GE.CurrentServerZone.Waypoints[j];

                        if (jW.NextA == W)
                        {
                            jW.WaypointLinkAEN.Free();
                            jW.WaypointLinkAEN = null;
                            jW.NextA = null;
                        }

                        if (jW.NextB == W)
                        {
                            jW.WaypointLinkBEN.Free();
                            jW.WaypointLinkBEN = null;
                            jW.NextB = null;
                        }
                    }

                    W.NextA = null;
                    W.NextB = null;
                    W.Prev = null;

                    if (W.WaypointLinkAEN != null)
                        W.WaypointLinkAEN.Free();
                    if (W.WaypointLinkBEN != null)
                        W.WaypointLinkBEN.Free();
                    W.WaypointLinkAEN = null;
                    W.WaypointLinkBEN = null;

                    Program.GE.CurrentServerZone.Waypoints.Remove(W);

                }
                else if (Obj is Portal)
                {
                    Portal P = Obj as Portal;
                    P.Name = "";
                    P.LinkName = "";
                    P.LinkArea = "";

                    if (P.Label != null)
                    {
                        GE.GUIManager.Destroy(P.Label);
                        P.Label = null;
                    }

                    Program.GE.CurrentServerZone.Portals.Remove(P);

                }
                // Remove from zone
                Program.GE.RemoveObject(Obj);
                // Hide entity
                if(Obj.EN != null && !(Obj is Tree))
                    Obj.EN.Visible = false;

                Program.GE.ZoneSelected.Remove(Obj);
            }

            Program.GE.WorldSelectedObjectsLabel.Text = "Selected objects: 0";
            Program.GE.SetWorldSavedStatus(false);

            Program.GE.m_ZoneList.AddObjectsCount();
            Program.GE.m_propertyWindow.RefreshObjectWindow();
        }
    

        private void duplicateObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WorldZonesTree.SelectedNode.Level >= 1)
            {
                TreeNode GotoObjectNode;
                if (WorldZonesTree.SelectedNode.Level >= 1)
                {
                    GotoObjectNode = WorldZonesTree.SelectedNode;
                    ZoneObject Obj = (ZoneObject)GotoObjectNode.Tag;
                    ZoneObject ZO = Program.GE.m_ZoneRender.WorldDuplicateObject(Obj);
                    AddObjectsCount();
                }
            }
        }

        private void selectObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WorldZonesTree.SelectedNode.Level >= 1)
            {

                WorldZonesTree.SelectedNode.Checked = true;
            }
        }
    }
}