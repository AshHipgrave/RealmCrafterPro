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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using RealmCrafter;
using RealmCrafter.ClientZone;
using RealmCrafter.ServerZone;
using RenderingServices;
using RottParticles;
using Emitter = RealmCrafter.ClientZone.Emitter;
using NGUINet;

namespace RealmCrafter_GE
{
    public class Undo
    {
        // Possible undo actions

        #region Actions enum
        public enum Actions
        {
            Move,
            Rotate,
            Scale,
            Select,
            Unselect,
            Create,
            Delete
        } ;
        #endregion

        // The undo stack
        public static Stack<Undo> Stack = new Stack<Undo>(64); // Remove public

        // Perform an undo

        // Details for this undo
        private Actions Action;
        private object Info, Info2;
        private float InfoX, InfoY, InfoZ;

        // Constructors
        public Undo(Actions Action, object Info, object Info2, float X, float Y, float Z)
        {
            this.Action = Action;
            this.Info = Info;
            this.Info2 = Info2;
            this.InfoX = X;
            this.InfoY = Y;
            this.InfoZ = Z;

            Stack.Push(this);
        }

        public Undo(Actions Action, object Info, float X, float Y, float Z)
        {
            this.Action = Action;
            this.Info = Info;
            this.Info2 = null;
            this.InfoX = X;
            this.InfoY = Y;
            this.InfoZ = Z;

            Stack.Push(this);
        }

        public Undo(Actions Action, object Info, object Info2)
        {
            this.Action = Action;
            this.Info = Info;
            this.Info2 = Info2;
            this.InfoX = 0f;
            this.InfoY = 0f;
            this.InfoZ = 0f;

            Stack.Push(this);
        }

        public Undo(Actions Action, object Info)
        {
            this.Action = Action;
            this.Info = Info;
            this.Info2 = null;
            this.InfoX = 0f;
            this.InfoY = 0f;
            this.InfoZ = 0f;

            Stack.Push(this);
        }

        public static void Perform(GE G)
        {
            if (Stack.Count > 0)
            {
                Undo U = Stack.Pop();

                G.SuppressZoneUndo = true;
                switch (U.Action)
                {
                    // Restore previous position
                    case Actions.Move:
                        G.MoveSelection(-U.InfoX, -U.InfoY, -U.InfoZ);
                        break;
                    // Restore previous rotation
                    case Actions.Rotate:
                        G.TurnSelection(-U.InfoX, -U.InfoY, -U.InfoZ);
                        break;
                    // Restore previous scale
                    case Actions.Scale:
                        // Absolute
                        if ((string)U.Info == "A")
                        {
                            G.ScaleSelectionAbsolute(U.InfoX, U.InfoY, U.InfoZ);
                        }
                        // Relative
                        else
                        {
                            G.ScaleSelection((1f / U.InfoX) - 1f, (1f / U.InfoY) - 1f, (1f / U.InfoZ) - 1f);
                        }
                        break;
                    // Undo selection of an object
                    case Actions.Select:
                        G.WorldSelectObject((ZoneObject)U.Info, 2);
                        break;
                    // Reselect an object which was unselected
                    case Actions.Unselect:
                        G.WorldSelectObject((ZoneObject)U.Info, 1);
                        break;
                    // Restore deleted objects
                    case Actions.Delete:
                        LinkedList<ZoneObject> Objects = (LinkedList<ZoneObject>)U.Info;
                        while (Objects.Count > 0)
                        {
                            LinkedListNode<ZoneObject> Node = Objects.First;
                            TreeNode TN;
                            string Name;
                            // Scenery
                            if (Node.Value is Scenery)
                            {
                                Scenery S = (Scenery)Node.Value;
                                G.CurrentClientZone.Sceneries.Add(S);
                                Name = Media.GetMeshName(S.MeshID);
                                TN =
                                    new TreeNode(
                                        Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
                                TN.Tag = S;
                                G.m_ZoneList.WorldZonesTree.Nodes[0].Nodes.Add(TN);
                            }
                            // Terrain
                            else if (Node.Value is RCTTerrain)
                            {
                                RCTTerrain T = (RCTTerrain)Node.Value;
                                G.CurrentClientZone.Terrains.Add(T);
                                TN = new TreeNode("Terrain " + (G.CurrentClientZone.Terrains.IndexOf(T) + 1).ToString());
                                TN.Tag = T;
                                G.m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
                            }
                            // Emitter
                            else if (Node.Value is Emitter)
                            {
                                Emitter Emi = (Emitter)Node.Value;
                                G.CurrentClientZone.Emitters.Add(Emi);
                                if (!string.IsNullOrEmpty(Emi.ConfigName))
                                {
                                    Emi.Config =
                                        EmitterConfig.Load(
                                            @"Data\Emitter Configs\" + Emi.ConfigName + ".rpc", G.Camera, 0);
                                }
                                if (Emi.Config != null)
                                {
                                    uint Texture = Media.GetTexture(Emi.TextureID, false);
                                    if (Texture != 0)
                                    {
                                        Emi.Config.ChangeTexture(Texture, false);
                                    }
                                    Entity EmitterEN = General.CreateEmitter(Emi.Config);
                                    EmitterEN.Parent(Emi.EN, false);
                                    TN = new TreeNode(Emi.Config.Name);
                                }
                                else
                                {
                                    TN = new TreeNode("Unknown emitter");
                                }
                                TN.Tag = Emi;
                                G.m_ZoneList.WorldZonesTree.Nodes[2].Nodes.Add(TN);
                            }
                            // Water
                            else if (Node.Value is Water)
                            {
                                Water W = (Water)Node.Value;
                                G.CurrentClientZone.Waters.Add(W);
                                Name = Media.GetTextureName((int)W.TextureID[0]);
                                TN =
                                    new TreeNode(
                                        Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
                                TN.Tag = W;
                                G.m_ZoneList.WorldZonesTree.Nodes[3].Nodes.Add(TN);
                            }
                            // Collision box
                            else if (Node.Value is ColBox)
                            {
                                ColBox CB = (ColBox)Node.Value;
                                G.CurrentClientZone.ColBoxes.Add(CB);
                                TN =
                                    new TreeNode("Collision box " +
                                                 (G.CurrentClientZone.ColBoxes.IndexOf(CB) + 1).ToString());
                                TN.Tag = CB;
                                G.m_ZoneList.WorldZonesTree.Nodes[4].Nodes.Add(TN);
                            }
                            // Sound zone
                            else if (Node.Value is SoundZone)
                            {
                                SoundZone SZ = (SoundZone)Node.Value;
                                G.CurrentClientZone.SoundZones.Add(SZ);
                                if (SZ.SoundID < 65535)
                                {
                                    Name = Media.GetSoundName(SZ.SoundID);
                                    Name = Name.Substring(0, Name.Length - 1);
                                }
                                else if (SZ.MusicID < 65535)
                                {
                                    Name = Media.GetMusicName(SZ.MusicID);
                                }
                                else
                                {
                                    Name = "Unknown sound";
                                }
                                TN = new TreeNode(Path.GetFileNameWithoutExtension(Name));
                                TN.Tag = SZ;
                                G.m_ZoneList.WorldZonesTree.Nodes[5].Nodes.Add(TN);
                            }
                            // Dynamic light
                            else if (Node.Value is RealmCrafter.ClientZone.Light)
                            {
                                RealmCrafter.ClientZone.Light L = (RealmCrafter.ClientZone.Light)Node.Value;
                                G.CurrentClientZone.Lights.Add(L);
                                TN = new TreeNode("Light");
                                TN.Tag = L;
                                Program.GE.m_ZoneList.WorldZonesTree.Nodes[6].Nodes.Add(TN);
                            }
                            // Menu Control
                            else if (Node.Value is RealmCrafter.ClientZone.MenuControl)
                            {
                                RealmCrafter.ClientZone.MenuControl Mc = (RealmCrafter.ClientZone.MenuControl)Node.Value;
                                G.CurrentClientZone.MenuControls.Add(Mc);
                            }
                            // Trigger
                            else if (Node.Value is Trigger)
                            {
                                Trigger T = (Trigger)Node.Value;
                                G.Triggers.Add(T);
                                G.CurrentServerZone.Triggers.Add(T);
                                TN = new TreeNode(T.Script);
                                TN.Tag = T;
                                T.Label = GE.GUIManager.CreateLabel("PORTALNAME", new NVector2(0, 0), new NVector2(0, 0));
                                T.ReBuild();
                                G.m_ZoneList.WorldZonesTree.Nodes[7].Nodes.Add(TN);
                            }
                            // Portal
                            else if (Node.Value is Portal)
                            {
                                Portal P = (Portal)Node.Value;
                                G.Portals.Add(P);
                                G.CurrentServerZone.Portals.Add(P);
                                TN = new TreeNode(P.Name);
                                TN.Tag = P;
                                P.Label = GE.GUIManager.CreateLabel("PORTALNAME", new NVector2(0, 0), new NVector2(0, 0));
                                P.ReBuild();
                                G.m_ZoneList.WorldZonesTree.Nodes[9].Nodes.Add(TN);
                            }
                            // Waypoint
                            else if (Node.Value is Waypoint)
                            {
                                Waypoint W = (Waypoint)Node.Value;
                                G.Waypoints.Add(W);
                                G.CurrentServerZone.Waypoints.Add(W);
                                TN = new TreeNode("Waypoint " + (W.ServerID + 1).ToString());
                                TN.Tag = W;
                                W.Label = GE.GUIManager.CreateLabel("WAYPOINTLABEL", new NVector2(0, 0), new NVector2(0, 0));
                                Actor A = Actor.Index[W.ActorID];
                                if (A != null)
                                    W.Label.Text = A.Race + " [" + A.Class + "]";
                                else
                                    W.Label.Text = "UNKNOWN ACTORID";

                                G.m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Add(TN);
                            }
                            // Show main entity
                            Node.Value.EN.Visible = true;
                            // Re-select
                            G.WorldSelectObject(Node.Value, 1);
                            Objects.RemoveFirst();
                        }
                        Program.GE.m_ZoneList.AddObjectsCount();
                        break;
                    // Remove a created object
                    case Actions.Create:
                        ZoneObject Obj = (ZoneObject)U.Info;
                        // Free object type specific stuff
                        if (Obj is Scenery)
                        {
                            Scenery S = (Scenery)Obj;
                            if (S.SceneryID > 0)
                            {
                                G.CurrentServerZone.Instances[0].OwnedScenery[S.SceneryID - 1] = null;
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
                            G.CurrentServerZone.Triggers.Remove(T);

                            if (T.Label != null)
                            {
                                GE.GUIManager.Destroy(T.Label);
                                T.Label = null;
                            }

                        }
                        else if (Obj is Waypoint)
                        {
                            Waypoint W = Obj as Waypoint;
                            // Find waypoints connected to this one, and remove the connections
                            for (int j = 0; j < G.CurrentServerZone.Waypoints.Count; ++j)
                            {
                                Waypoint jW = G.CurrentServerZone.Waypoints[j];
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
                            // Remove this waypoint
                            W.Prev = null;
                            W.NextA = null;
                            W.NextB = null;

                            if (W.WaypointLinkAEN != null)
                            {
                                W.WaypointLinkAEN.Free();
                            }
                            if (W.WaypointLinkBEN != null)
                            {
                                W.WaypointLinkBEN.Free();
                            }
                            W.WaypointLinkAEN = null;
                            W.WaypointLinkBEN = null;
                            G.CurrentServerZone.Waypoints.Remove(W);

                            if (W.Label != null)
                            {
                                GE.GUIManager.Destroy(W.Label);
                                W.Label = null;
                            }
                        }
                        else if (Obj is Portal)
                        {
                            Portal P = Obj as Portal;
                            P.Name = "";
                            P.LinkArea = "";
                            P.LinkName = "";

                            if (P.Label != null)
                            {
                                GE.GUIManager.Destroy(P.Label);
                                P.Label = null;
                            }

                            G.CurrentServerZone.Portals.Remove(P);
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
                                    Program.GE.m_TerrainEditor.EditorModeButton_Click(null, System.EventArgs.Empty);
                                //Program.GE.m_TerrainEditor.Terrain = null;
                                //Program.GE.m_TerrainEditor.EditorMode = false;
                                //Program.GE.m_TerrainEditor.RefreshEditorButton(0);
                            }
                            T.Terrain = null;
                        }
                        // Remove from zone
                        G.RemoveObject(Obj);
                        // Free entity

                        if (!(Obj is RCTTerrain))
                            Obj.EN.Free();
                        break;
                }
                G.SuppressZoneUndo = false;
            }
        }

        // Clear all undos
        public static void Clear()
        {
            while (Stack.Count > 0)
            {
                Undo U = Stack.Pop();
                if (U.Action == Actions.Delete)
                {
                    LinkedList<ZoneObject> List = (LinkedList<ZoneObject>)U.Info;
                    while (List.Count > 0)
                    {
                        LinkedListNode<ZoneObject> Node = List.First;
                        Node.Value.EN.Free();
                        List.RemoveFirst();
                    }
                }
            }
        }

        public static Undo CreateAdditiveUndo(Actions Action, object Info, float X, float Y, float Z)
        {
            // If last undo is of the same type, do not create another
            if (Stack.Count > 0)
            {
                Undo LastUndo = Stack.Peek();
                if (LastUndo.Action == Action && LastUndo.Info == Info)
                {
                    LastUndo.InfoX += X;
                    LastUndo.InfoY += Y;
                    LastUndo.InfoZ += Z;
                    return LastUndo;
                }
            }

            return new Undo(Action, Info, null, X, Y, Z);
        }

        public static Undo CreateMultiplyUndo(Actions Action, object Info, float X, float Y, float Z)
        {
            // If last undo is of the same type, do not create another
            if (Stack.Count > 0)
            {
                Undo LastUndo = Stack.Peek();
                if (LastUndo.Action == Action && LastUndo.Info == Info)
                {
                    LastUndo.InfoX *= X;
                    LastUndo.InfoY *= Y;
                    LastUndo.InfoZ *= Z;
                    return LastUndo;
                }
            }

            return new Undo(Action, Info, null, X, Y, Z);
        }

        public static Undo CreateNonRepeatingUndo(Actions Action, object Info, float X, float Y, float Z)
        {
            // If last undo is of the same type, do not create another
            if (Stack.Count > 0)
            {
                Undo LastUndo = Stack.Peek();
                if (LastUndo.Action == Action && LastUndo.Info == Info)
                {
                    return LastUndo;
                }
            }

            return new Undo(Action, Info, null, X, Y, Z);
        }
    }
}