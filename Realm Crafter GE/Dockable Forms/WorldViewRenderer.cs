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
// Realmcrafter World rendering form for use in WinForms dockable interface
// August 2008
// Author:Shane

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NGUINet;
using RealmCrafter;
using RealmCrafter.ClientZone;
using RealmCrafter.ServerZone;
using RenderingServices;
using RottParticles;
using WeifenLuo.WinFormsUI.Docking;
using Emitter=RealmCrafter.ClientZone.Emitter;
using Light=RealmCrafter.ClientZone.Light;
using Timer=MLib.Timer;
using Zone=RealmCrafter.ClientZone.Zone;
using System.Threading;

namespace RealmCrafter_GE
{
    /*
     * This was rushed to get it working in time for the deadline,
     * as such, some of the world rendering declarations are in the main GE class, 
     * rather than dedicated to this class
     * quite a bit of refactoring will need to be done to this class
     * and the new renderer to make it easily extendable
     */

    public class WorldViewRenderer : DockContent
    {
        private System.ComponentModel.IContainer components = null;

        #region Delta timing
        private const float BaseFPS = 30f;
        private const int DeltaFrames = 10;
        private static float Delta;
        private static int[] DeltaBuffer;
        private static int DeltaBufferIndex;
        private static int DeltaTime, MilliSecs;
        private float SpeedFrames; // coef for DeltaFrames
        #endregion

        #region Zone editor mouselook
        // private float Program.GE.Program.GE.CameraSpeed = 0.2f;
        private bool Mouselooking;
        private int OldMouseX, OldMouseY;
        private bool RealTimeUpdate = true;
        private float WorldCamDPitch, WorldCamDYaw;
        private float WorldCamPitch;
        private float WorldCamX, WorldCamY = 20f;
        private float WorldCamYaw;
        private float WorldCamZ;
        private bool DuplicateKey;
        #endregion

        #region Zones stuff
        private float CreateTimeLimit;
        private bool CtrlZWasDown;
        public bool MouseDragging;
        public int WaypointLinkMode = 0;
        public Waypoint LinkingWaypoint = null;
        public List<ZoneObject> Portals = new List<ZoneObject>();
        private Entity Plane; // = Entity.CreateCube();
        public uint RedTex;
        internal bool SuppressZoneTransforms, SuppressZoneUndo;
        public List<ZoneObject> Triggers = new List<ZoneObject>();
        /*public Entity[] WaypointEN = new Entity[2000],
                        WaypointLinkAEN = new Entity[2000],
                        WaypointLinkBEN = new Entity[2000];
    */
        public List<ZoneObject> Waypoints = new List<ZoneObject>();
        private bool ZoneShortcutWasDown;
        #endregion

        // Constructor
        public WorldViewRenderer()
        {
            InitializeComponent();

            DeltaBuffer = new int[DeltaFrames];
            for (int i = 0; i < DeltaFrames; ++i)
            {
                DeltaBuffer[i] = 35;
            }

            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        #region Initialise
        // ReSharper disable RedundantDelegateCreation
        // Create and initialise all components
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WorldViewRenderer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 351);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HideOnClose = true;
            this.MinimizeBox = false;
            this.Name = "WorldViewRenderer";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TabText = "World Renderer";
            this.Text = "World Renderer";
            this.Load += new System.EventHandler(this.WorldViewRenderer_Load);
            this.Shown += new System.EventHandler(this.WorldRender_Show);
            this.ResumeLayout(false);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        // ReSharper restore RedundantDelegateCreation
        #endregion

        private void WorldViewRenderer_Load(object sender, EventArgs e)
        {
            Plane = Entity.CreateCube();
            Plane.Scale(10000000, 1, 10000000);
            Collision.EntityType(Plane, (byte)CollisionType.Triangle);
            Collision.SetCollisionMesh(Plane);
        }

        public void ResetCamera()
        {
            Program.GE.Camera.Rotate(WorldCamPitch, WorldCamYaw, 0f);
            Program.GE.Camera.Position(WorldCamX, WorldCamY, WorldCamZ);
        }

        private void WorldRender_Show(object sender, EventArgs e)
        {
            // Move the render panel
//            Program.GE.UpdateRenderingPanel(-3);

            // Program.GE.Camera = Program.GE.Program.GE.Camera;
        }

        public void WorldRender_MainLoop(object sender, EventArgs e)
        {
            while (Program.AppStillIdle)
            {
                #region Timing code
                Timer.Update();
                DeltaTime = Timer.DeltaTime;
                MilliSecs = Timer.Ticks;

                // Store delta time
                DeltaBuffer[DeltaBufferIndex] = DeltaTime;
                DeltaBufferIndex++;
                if (DeltaBufferIndex == DeltaFrames)
                {
                    DeltaBufferIndex = 0;
                }

                SpeedFrames = 1;

                float Time = 0;
                for (int i = 0; i < SpeedFrames * DeltaFrames; i++)
                {
                    Time += DeltaBuffer[i];
                }

                Time /= DeltaFrames;
                float FPS = 1000f / Time;
                Delta = BaseFPS / FPS;
                if (Delta > 5f)
                {
                    Delta = 5f;
                }
                #endregion
                Thread.Sleep(0);
                // Update Lights (can happen regardless of application settings)
                LightFunctionList.Update();

                if (Program.GE.CurrentClientZone != null && Program.GE.CurrentClientZone.Lights != null)
                {
                    foreach (RealmCrafter.ClientZone.Light L in Program.GE.CurrentClientZone.Lights)
                    {
                        if (L.Function != null)
                        {
                            L.Handle.Color(
                                Convert.ToInt32(L.Function.Color.R),
                                Convert.ToInt32(L.Function.Color.G),
                                Convert.ToInt32(L.Function.Color.B));
                            L.Handle.Radius(Convert.ToSingle(L.Function.Radius));
                        }
                    }
                }

                if (CreateTimeLimit > 0)
                {
                    CreateTimeLimit -= Delta;
                }

                if (Program.GE.m_propertyWindow.UpdateTimer > 0)
                {
                    Program.GE.m_propertyWindow.UpdateTimer -= Delta;
                }

                RealTimeUpdate = FPS >= 30;
                #region Shortcuts
                if (DockPanel != null)
                {
                    if (DockPanel.ActivePane != null)
                    {
                        if (DockPanel.ActivePane.CaptionText == Text)
                        {
                            if (ZoneShortcutWasDown)
                            {
                                if (!KeyState.Get(Keys.F1) && !KeyState.Get(Keys.F2) && !KeyState.Get(Keys.F3))
                                {
                                    ZoneShortcutWasDown = false;
                                }
                            }
                            else
                            {
                                if (KeyState.Get(Keys.F1))
                                {
                                    // Set to Move Mode
                                    Program.GE.ObjectMove_Click(null, null);
                                    Program.GE.RenderingPanel.Focus();
                                    ZoneShortcutWasDown = true;
                                }
                                else if (KeyState.Get(Keys.F2))
                                {
                                    // Set to rotate mode
                                    Program.GE.ObjectRotate_Click(null, null);
                                    Program.GE.RenderingPanel.Focus();
                                    ZoneShortcutWasDown = true;
                                }
                                else if (KeyState.Get(Keys.F3))
                                {
                                    // Set to scale mode
                                    Program.GE.ObjectScale_Click(null, null);
                                    Program.GE.RenderingPanel.Focus();
                                    ZoneShortcutWasDown = true;
                                }
                #endregion

                                #region Ctrl+Z to undo
                                if (KeyState.Get(Keys.Z) && KeyState.Get(Keys.ControlKey))
                                {
                                    if (CtrlZWasDown == false)
                                    {
                                        CtrlZWasDown = true;
                                        Undo.Perform(Program.GE);
                                        Program.GE.m_propertyWindow.RefreshObjectWindow();
                                    }
                                }
                                else
                                {
                                    CtrlZWasDown = false;
                                }
                                #endregion

                                #region Ctrl+Q to quit
                                if (KeyState.Get(Keys.Q) && KeyState.Get(Keys.ControlKey))
                                {
                                    Program.GE.Close();
                                }
                                #endregion

                                #region Ctrl+S to save world view
                                if (KeyState.Get(Keys.S) && KeyState.Get(Keys.ControlKey))
                                {
                                    Program.GE.SaveHotkeyDown = true;

                                    if (!Program.GE.WorldSaved)
                                    {
                                        Program.GE.SaveWorld();
                                    }
                                }
                                else
                                {
                                    Program.GE.SaveHotkeyDown = false;
                                }
                                #endregion

                                #region Ctrl+V to duplicate selection
                                if (KeyState.Get(Keys.V) && KeyState.Get(Keys.ControlKey))
                                {
                                    if (DuplicateKey == false)
                                    {
                                        DuplicateKey = true;
                                        LinkedList<ZoneObject> CreatedObjects = new LinkedList<ZoneObject>();
                                        for (int i = 0; i < Program.GE.ZoneSelected.Count; ++i)
                                        {
                                            ZoneObject ZO = WorldDuplicateObject((ZoneObject)Program.GE.ZoneSelected[i]);
                                            if (ZO != null)
                                            {
                                                CreatedObjects.AddLast(ZO);
                                            }
                                        }
                                        // Clear current selections
                                        while (Program.GE.ZoneSelected.Count > 0)
                                        {
                                            Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(
                                                (ZoneObject)Program.GE.ZoneSelected[0],
                                                false);
                                        }
                                        // Select all newly created objects
                                        LinkedListNode<ZoneObject> CreatedObjectsNode = CreatedObjects.First;
                                        while (CreatedObjectsNode != null)
                                        {
                                            WorldSelectObject(CreatedObjectsNode.Value, 1);
                                            CreatedObjectsNode = CreatedObjectsNode.Next;
                                        }

                                        CreatedObjects.Clear();
                                        Program.GE.m_ZoneList.AddObjectsCount();
                                    }
                                }
                                else
                                {
                                    DuplicateKey = false;
                                }
                                #endregion
                            }
                        }
                    }
                    if (DockPanel.ActivePane != null)
                    {
                        if (DockPanel.ActivePane.CaptionText == Text)
                        {
                            #region ZZ
                            // Program.GE.Program.GE.Camera = Program.GE.Camera;
                            int CamX = (int)Program.GE.Camera.X();
                            int CamY = (int)Program.GE.Camera.Y();
                            int CamZ = (int)Program.GE.Camera.Z();
                            string CamPos = "Camera: " + CamX + ", " + CamY + ", " +
                                            CamZ;
                            Program.GE.WorldCameraPositionA.Text = CamPos;

                            if (KeyState.Get(Keys.Escape))
                            {
                                while (Program.GE.ZoneSelected.Count > 0)
                                {
                                    Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(
                                        (ZoneObject)Program.GE.ZoneSelected[0], false);
                                }
                                WaypointLinkMode = 0;

                                Program.GE.m_propertyWindow.RefreshObjectWindow();
                            }

                            #region Mouselook
                            if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Rotate))
                            {
                                // Enter mouselook mode
                                if (!Mouselooking)
                                {
                                    Program.GE.Mouselooking = true;
                                    Mouselooking = true;
                                    Cursor.Position =
                                        Program.GE.RenderingPanel.PointToScreen(
                                            new Point(Program.GE.RenderingPanel.Size.Width / 2,
                                                      Program.GE.RenderingPanel.Size.Height / 2));
                                    OldMouseX = Cursor.Position.X;
                                    OldMouseY = Cursor.Position.Y;
                                    Cursor.Hide();
                                }

                                    // Continue mouselook mode
                                else
                                {
                                    // Rotate Camera
                                    int MouseMoveX = Cursor.Position.X - OldMouseX;
                                    int MouseMoveY = Cursor.Position.Y - OldMouseY;
                                    WorldCamDPitch += MouseMoveY;
                                    WorldCamDYaw -= MouseMoveX;
                                    if (WorldCamDPitch > 89f)
                                    {
                                        WorldCamDPitch = 89f;
                                    }
                                    else if (WorldCamDPitch < -89f)
                                    {
                                        WorldCamDPitch = -89f;
                                    }

                                    WorldCamPitch = Lerp(WorldCamPitch, WorldCamDPitch, 2f);
                                    WorldCamYaw = Lerp(WorldCamYaw, WorldCamDYaw, 2f);

                                    Program.GE.Camera.Rotate(WorldCamPitch, WorldCamYaw, 0f);

                                    // Position mouse at screen centre
                                    Cursor.Position =
                                        Program.GE.RenderingPanel.PointToScreen(
                                            new Point(Program.GE.RenderingPanel.Size.Width / 2,
                                                      Program.GE.RenderingPanel.Size.Height / 2));
                                    OldMouseX = Cursor.Position.X;
                                    OldMouseY = Cursor.Position.Y;
                                }
                            }
                            else if (Mouselooking)
                            {
                                // Leave mouselook mode
                                Cursor.Show();
                                Mouselooking = false;
                            }

                            // Move Program.GE.Camera
                            // Forward/backwarsd
                            if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Forward))
                            {
                                Program.GE.Camera.Move(0f, 0f, 25f * Delta * Program.GE.CameraSpeed);
                            }
                            else if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Backward))
                            {
                                Program.GE.Camera.Move(0f, 0f, -25f * Delta * Program.GE.CameraSpeed);
                            }

                            // Left/Right
                            if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Left))
                            {
                                Program.GE.Camera.Move(-25f * Delta * Program.GE.CameraSpeed, 0f, 0f);
                            }
                            else if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Right))
                            {
                                Program.GE.Camera.Move(25f * Delta * Program.GE.CameraSpeed, 0f, 0f);
                            }

                            // Up/Down
                            if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Up))
                            {
                                Program.GE.Camera.Move(0f, 25f * Delta * Program.GE.CameraSpeed, 0f);
                            }
                            else if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Down))
                            {
                                Program.GE.Camera.Move(0f, -25f * Delta * Program.GE.CameraSpeed, 0f);
                            }

                            WorldCamX = Program.GE.Camera.X();
                            WorldCamY = Program.GE.Camera.Y();
                            WorldCamZ = Program.GE.Camera.Z();
                            Program.GE.RepositionGrid();

                            if (Program.GE.CurrentClientZone != null)
                            {
                                //Program.GE.CurrentClientZone.Stars.Position(Program.GE.Camera.X(),
                                //                                            Program.GE.Camera.Y(),
                                //                                            Program.GE.Camera.Z());
                                //Program.GE.CurrentClientZone.Cloud.Position(Program.GE.Camera.X(),
                                //                                            Program.GE.Camera.Y(),
                                //                                            Program.GE.Camera.Z());

                            }
                            #endregion

                            if (Program.GE.ZoneSelected.Count > 0 && !Mouselooking && !KeyState.Get(Keys.ControlKey))
                            {
                                bool MoveReorientate = false;

                                // Scale mouse position to back-buffer co-ordinates
                                Point MousePos = Program.GE.RenderingPanel.PointToClient(Cursor.Position);

                                // Get movement speed scale
                                float Speed = 2f;
                                if (KeyState.Get(Keys.ShiftKey))
                                {
                                    MoveReorientate = true;
                                    Speed = 0.2f;
                                }

                                // Selected tab
                                switch (Program.GE.SetWorldButtonSelection)
                                {
                                    // Movement tab
                                    case (int)GE.WorldButtonSelection.MOVE:
                                        // Keys
                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_Translate_Z_Plus))
                                        {
                                            MoveSelection(0f, 0f, Delta * Speed);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if(Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_Translate_Z_Minus))
                                        {
                                            MoveSelection(0f, 0f, Delta * -Speed);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if(Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_Translate_X_Plus))
                                        {
                                            MoveSelection(Delta * Speed, 0f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_Translate_X_Minus))
                                        {
                                            MoveSelection(Delta * -Speed, 0f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_Translate_Y_Plus))
                                        {
                                            MoveSelection(0f, Delta * Speed, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_Translate_Y_Minus))
                                        {
                                            MoveSelection(0f, Delta * -Speed, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (KeyState.Get(Keys.RButton))
                                        {
                                            ZoneObject CollisionMesh;
                                            ZoneObject Object = (ZoneObject)Program.GE.ZoneSelected[0];
                                            if (!MouseDragging)
                                            {
                                                for (int i = 1; i < Program.GE.ZoneSelected.Count + 1; i++)
                                                {
                                                    CollisionMesh = (ZoneObject)Program.GE.ZoneSelected[i - 1];
                                                    Collision.EntityPickMode(CollisionMesh.EN, PickMode.Unpickable);
                                                }
                                                OldMouseX = Cursor.Position.X;
                                                OldMouseY = Cursor.Position.Y;
                                                MouseDragging = true;
                                            }
                                            else
                                            {
                                                int MouseX = MousePos.X;
                                                int MouseY = MousePos.Y;
                                                //int MouseX = (MousePos.X * 1024) /
                                                //             Program.GE.RenderingPanel.Size.Width;
                                                //int MouseY = (MousePos.Y * 768) /
                                                 //            Program.GE.RenderingPanel.Size.Height;
                                                int MouseMoveX = Cursor.Position.X - OldMouseX;
                                                int MouseMoveY = Cursor.Position.Y - OldMouseY;
                                                Collision.EntityPickMode(Plane, PickMode.Unpickable);
                                                Entity E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
                                                if (E == null)
                                                {
                                                    Plane.Position(0, Program.GE.GridHeight, 0);
                                                    // Plane.Shader = Shaders.FullbrightAlpha;
                                                    // Plane.Alpha(0.5f);
                                                    Collision.EntityPickMode(Plane, PickMode.Polygon);
                                                    E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
                                                    if (E != null && Program.IsRendered(E))
                                                    {
                                                        if (MouseMoveX != 0 || MouseMoveY != 0)
                                                        {
                                                            float PosX = Collision.PickedX();
                                                            float PosY = Collision.PickedY();
                                                            float PosZ = Collision.PickedZ();
                                                            if (Object is Portal || Object is Waypoint ||
                                                                Object is Trigger)
                                                            {
                                                                PosY = PosY + 5;
                                                            }

                                                            PositionSelection(PosX, PosY, PosZ);

                                                            if (MoveReorientate &&
                                                                (Object is Scenery || Object is ColBox))
                                                            {
                                                                float NormalX = 0f, NormalY = 1f, NormalZ = 0f;
                                                                NormalX = Collision.PickedNX();
                                                                NormalY = Collision.PickedNY();
                                                                NormalZ = Collision.PickedNZ();
                                                                Object.EN.AlignToVector(NormalX, NormalY, NormalZ, 2);
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (E != null && Program.IsRendered(E))
                                                {
                                                    if (MouseMoveX != 0 || MouseMoveY != 0)
                                                    {
                                                        float PosX = Collision.PickedX();
                                                        float PosY = Collision.PickedY();
                                                        float PosZ = Collision.PickedZ();
                                                        if (Object is Portal || Object is Waypoint ||
                                                            Object is Trigger)
                                                        {
                                                            PosY = PosY + 5;
                                                        }

                                                        PositionSelection(PosX, PosY, PosZ);

                                                        if (MoveReorientate &&
                                                            (Object is Scenery || Object is ColBox))
                                                        {
                                                            float NormalX = 0f, NormalY = 1f, NormalZ = 0f;
                                                            NormalX = Collision.PickedNX();
                                                            NormalY = Collision.PickedNY();
                                                            NormalZ = Collision.PickedNZ();
                                                            Object.EN.AlignToVector(NormalX, NormalY, NormalZ, 2);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MoveSelection(Delta * Speed * MouseMoveX, 0f,
                                                                  Delta * Speed * MouseMoveY);
                                                }

                                                if (RealTimeUpdate && (MouseMoveX != 0 || MouseMoveY != 0))
                                                {
                                                    Program.GE.m_propertyWindow.TimedRefreshObjectWindow();
                                                }

                                                MouseDragging = true;
                                            }
                                        }
                                        else
                                        {
                                            if (MouseDragging)
                                            {
                                                for (int i = 1; i < Program.GE.ZoneSelected.Count + 1; i++)
                                                {
                                                    ZoneObject Object = (ZoneObject)Program.GE.ZoneSelected[i - 1];
                                                    Collision.EntityPickMode(Object.EN, PickMode.Polygon);

                                                    if(Object is Tree)
                                                        Object.Moved(null, null);
                                                }
                                            }
                                            MouseDragging = false;
                                        }

                                        break;
                                    // Rotation tab
                                    case (int)GE.WorldButtonSelection.ROTATE:
                                        // Keys
                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_RotatePitch_Minus))
                                        {
                                            TurnSelection(Delta * -Speed, 0f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_RotatePitch_Plus))
                                        {
                                            TurnSelection(Delta * Speed, 0f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_RotatePitch_Plus))
                                        {
                                            TurnSelection(0f, Delta * Speed, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_RotatePitch_Minus))
                                        {
                                            TurnSelection(0f, Delta * -Speed, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_RotateRoll_Plus))
                                        {
                                            TurnSelection(0f, 0f, Delta * Speed);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_RotateRoll_Minus))
                                        {
                                            TurnSelection(0f, 0f, Delta * -Speed);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        // mouse
                                        if (KeyState.Get(Keys.RButton))
                                        {
                                            if (!MouseDragging)
                                            {
                                                OldMouseX = Cursor.Position.X;
                                                MouseDragging = true;
                                            }
                                            else
                                            {
                                                int MouseMoveX = Cursor.Position.X - OldMouseX;
                                                OldMouseX = Cursor.Position.X;
                                                if (OldMouseX > 0)
                                                {
                                                    TurnSelection(0f, Delta * MouseMoveX, 0f);
                                                }

                                                if (RealTimeUpdate && MouseMoveX != 0)
                                                {
                                                    Program.GE.m_propertyWindow.TimedRefreshObjectWindow();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MouseDragging = false;
                                        }

                                        // Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        break;
                                    // Scaling tab
                                    case (int)GE.WorldButtonSelection.SCALE:
                                        // Keys                                    
                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_ScaleZ_Plus))
                                        {
                                            ScaleSelection(0f, 0f, Delta * Speed * 0.01f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_ScaleZ_Minus))
                                        {
                                            ScaleSelection(0f, 0f, Delta * Speed * -0.01f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_ScaleX_Plus))
                                        {
                                            ScaleSelection(Delta * Speed * 0.01f, 0f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_ScaleX_Minus))
                                        {
                                            ScaleSelection(Delta * Speed * -0.01f, 0f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_ScaleY_Plus))
                                        {
                                            ScaleSelection(0f, Delta * Speed * 0.01f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_ScaleY_Minus))
                                        {
                                            ScaleSelection(0f, Delta * Speed * -0.01f, 0f);
                                            Program.GE.m_propertyWindow.RefreshObjectWindow();
                                        }

                                        // Mouse
                                        if (KeyState.Get(Keys.RButton))
                                        {
                                            if (!MouseDragging)
                                            {
                                                OldMouseX = Cursor.Position.X;
                                                MouseDragging = true;
                                            }
                                            else
                                            {
                                                int MouseMoveX = Cursor.Position.X - OldMouseX;
                                                OldMouseX = Cursor.Position.X;
                                                if (OldMouseX > 0)
                                                {
                                                    float Scale = Delta * MouseMoveX * 0.07f;
                                                    ScaleSelection(Scale, Scale, Scale);
                                                }

                                                if (RealTimeUpdate && MouseMoveX != 0)
                                                {
                                                    Program.GE.m_propertyWindow.TimedRefreshObjectWindow();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MouseDragging = false;
                                        }
                                        break;
                                }
                            }

                            if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Grid_Up))
                            {
                                Program.GE.GridHeight += Delta;
                                Program.GE.RepositionGrid();
                            }
                            else if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Grid_Down))
                            {
                                Program.GE.GridHeight -= Delta;
                                Program.GE.RepositionGrid();
                            }
                            else if(Program.KeyBindings.IsActionPressed(KeyBinding.Action.Grid_Reset))
                            {
                                Program.GE.GridHeight = 0f;
                                Program.GE.RepositionGrid();
                            }

                            #region Delete
                            if (Program.KeyBindings.IsActionPressed(KeyBinding.Action.Object_Delete))
                            {
                                if (Program.GE.ZoneSelected.Count > 0 && !Mouselooking)
                                {
                                    // Warn if deleting more than 10 objects at once
                                    DialogResult Result = DialogResult.OK;
                                    if (Program.GE.ZoneSelected.Count > 0)
                                    {
                                        Result =
                                            MessageBox.Show(
                                                "Really delete these " + Program.GE.ZoneSelected.Count +
                                                " objects?", "Realm Crafter", MessageBoxButtons.OKCancel);
                                    }

                                    // Delete each object
                                    if (Result == DialogResult.OK)
                                    {
                                        // Create undo point
                                        LinkedList<ZoneObject> UndoList = new LinkedList<ZoneObject>();
                                        Undo U = new Undo(Undo.Actions.Delete, UndoList);

                                        ZoneObject Obj;
                                        for (int i = 0; i < Program.GE.ZoneSelected.Count; ++i)
                                        {
                                            Obj = (ZoneObject)Program.GE.ZoneSelected[i];
                                            // Add to list of deleted objects in undo info
                                            UndoList.AddLast(Obj);
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

                                                TreeNode TN = null;
                                                foreach(TreeNode iTN in Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Nodes)
                                                {
                                                    if(iTN.Tag != null && iTN.Tag == T)
                                                        TN = iTN;
                                                }
                                                if(TN != null)
                                                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Remove(TN);

                                                try
                                                {
                                                    File.Delete(T.Path);
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

                                                W.Prev = null;
                                                W.NextA = null;
                                                W.NextB = null;

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
                                                P.LinkArea = "";
                                                P.LinkName = "";

                                                if (P.Label != null)
                                                {
                                                    GE.GUIManager.Destroy(P.Label);
                                                    P.Label = null;
                                                }


                                                Program.GE.CurrentServerZone.Portals.Remove(P);
                                            }
                                            else
                                            {
                                                if (Obj.EN != null)
                                                    Program.GE.ClearSelectionBox(Obj.EN);
                                            }
                                            // Remove from zone
                                            Program.GE.RemoveObject(Obj);
                                            // Hide entity
                                            if(Obj.EN != null && !(Obj is Tree))
                                                Obj.EN.Visible = false;
                                        }

                                        if (Program.Transformer != null)
                                        {
                                            Program.Transformer.Free();
                                            Program.Transformer = null;
                                        }

                                        Program.GE.ZoneSelected.Clear();
                                        Program.GE.WorldSelectedObjectsLabel.Text = "Selected objects: 0";
                                        Program.GE.SetWorldSavedStatus(false);
                                        Program.GE.m_ZoneList.AddObjectsCount();
                                    }

                                    Program.GE.m_propertyWindow.RefreshObjectWindow();
                                }
                            }
                            #endregion

                            #endregion
                        }
                    }
                }

                if (Program.GE.m_TerrainEditor.EditorMode && !Mouselooking)
                {
                    Point P = this.PointToClient(Cursor.Position);
                    if (P.X > 0 && P.Y > 0 && P.X < Program.GE.RenderingPanel.Width && P.Y < Program.GE.RenderingPanel.Height)
                    {
                        Entity E = Collision.CameraPick(null, P.X, P.Y);
                        if (E != null && Program.IsRendered(E))
                        {
                            Program.GE.m_TerrainEditor.UpdatePosition(Collision.PickedX(), Collision.PickedZ());
                        }
                    }
                }

                Point Mouse = Program.GE.RenderingPanel.PointToClient(Cursor.Position);

                if (Program.Transformer != null)
                    Program.Transformer.Update(Mouse.X, Mouse.Y);

                Environment3D.Update(Delta);

                bool movementInput = Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Forward) ||
                    Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Backward) ||
                    Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Left) ||
                    Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Right) ||
                    Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Up) ||
                    Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Down) ||
                    Program.KeyBindings.IsActionPressed(KeyBinding.Action.Camera_Rotate);

               GE.TerrainManager.Update(new NVector3(Program.GE.Camera.X(), Program.GE.Camera.Y(), Program.GE.Camera.Z()),
                    !(movementInput));
                Program.UpdateTerrainManager();

// Point Mouse = PointToClient(Cursor.Position);
// GE.Parameters.MousePosition = new NVector2(Mouse.X, Mouse.Y);
// GE.GUIManager.Update(GE.Parameters);
// GE.Parameters.InputBuffer.Clear();

                //GE.TerrainManager.SetLightColor(0, System.Drawing.Color.FromArgb(255, 0, 0));
                //GE.TerrainManager.SetLightNormal(0, new NVector3(1, -1, 0));
                //GE.TerrainManager.SetLightColor(1, System.Drawing.Color.FromArgb(0, 255, 0));
                //GE.TerrainManager.SetLightNormal(1, new NVector3(-1, -1, 0));
                //GE.TerrainManager.SetLightColor(2, System.Drawing.Color.FromArgb(0, 0, 255));
                //GE.TerrainManager.SetLightNormal(2, new NVector3(0, -1, -1));
                //GE.TerrainManager.SetFog(System.Drawing.Color.White, new NVector2(0, 10000));

                if (Program.GE.CurrentServerZone != null)
                {
                    foreach (Portal P in Program.GE.CurrentServerZone.Portals)
                    {
                        P.UpdateLabel();
                    }

                    foreach (Trigger T in Program.GE.CurrentServerZone.Triggers)
                    {
                        T.UpdateLabel();
                    }

                    foreach (Waypoint W in Program.GE.CurrentServerZone.Waypoints)
                    {
                        W.UpdateLabel();
                    }
                }
                if (Program.GE.CurrentClientZone != null)
                {
                    foreach (MenuControl C in Program.GE.CurrentClientZone.MenuControls)
                        C.UpdateLabel();
                }

                Program.GE.Parameters.MousePosition = new NGUINet.NVector2(0, 0);
                GE.GUIManager.Update(Program.GE.Parameters);

                General.Update(Delta);
                Render.RenderWorld();
                Collision.UpdateWorld();
                if (FPS > 75)
                {
                    System.Threading.Thread.Sleep(8);
                }
            }
        }

        public void RenderingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Program.Transformer != null)
                    Program.Transformer.MouseUp();
                MouseOverLast = false;
            }
        }

        bool MouseOverLast = false;
        public void RenderingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Program.Transformer != null)
                    MouseOverLast = Program.Transformer.MouseDown(Program.GE.Camera, e.X, e.Y);
            }
        }

        public void RenderingPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (Mouselooking)
            {
                return;
            }

            Point MousePos = Program.GE.RenderingPanel.PointToClient(Cursor.Position);
            int MouseX = MousePos.X;// (MousePos.X * 1024) / Program.GE.RenderingPanel.Size.Width;
            int MouseY = MousePos.Y;// (MousePos.Y * 768) / Program.GE.RenderingPanel.Size.Height;

            if (Program.GE.m_TerrainEditor.EditorMode)
            {
                return;
            }

            // Left click (select an object)

            #region Left Click
            if (e.Button == MouseButtons.Left && !MouseOverLast)
            {
                MouseDragging = false;
                Collision.EntityPickMode(Plane, PickMode.Unpickable);
                Collision.UpdateWorld();
                Entity E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
                if (E != null && Program.IsRendered(E))
                {
                    #region Waypoint Linking
                    // Waypoint linking
                    if (WaypointLinkMode > 0)
                    {
                        ZoneObject Z = E.ExtraData as ZoneObject;
                        Waypoint WP = Z as Waypoint;
                        if (WP != null)
                        {
                            if (WP != LinkingWaypoint)
                            {
                                if (WaypointLinkMode == 1)
                                {
                                    // Remove current link if any
                                    if (LinkingWaypoint.NextA != null)
                                    {
                                        LinkingWaypoint.NextA.Prev = null;
                                    }

                                    // Set new link
                                    LinkingWaypoint.NextA = WP;
                                    WP.Prev = LinkingWaypoint;

                                    if (LinkingWaypoint.WaypointLinkAEN == null)
                                    {
                                        LinkingWaypoint.WaypointLinkAEN = Program.GE.WaypointLinkTemplate.Copy();
                                        LinkingWaypoint.WaypointLinkAEN.Shader = Shaders.LitObject1;
                                        LinkingWaypoint.WaypointLinkAEN.Texture(Program.GE.OrangeTex);
                                    }
                                }
                                else
                                {
                                    // Set new link
                                    LinkingWaypoint.NextB = WP;

                                    if (LinkingWaypoint.WaypointLinkBEN == null)
                                    {
                                        LinkingWaypoint.WaypointLinkBEN = Program.GE.WaypointLinkTemplate.Copy();
                                        LinkingWaypoint.WaypointLinkBEN.Shader = Shaders.LitObject1;
                                        LinkingWaypoint.WaypointLinkBEN.Texture(Program.GE.BlueTex);
                                    }
                                }

                                Program.GE.UpdateWaypointLinks();
                                Program.GE.SetWorldSavedStatus(false);
                                Program.GE.RenderingPanel.Focus();
                            }
                            else
                            {
                                MessageBox.Show("You cannot link a waypoint to itself. Linking cancelled.", "Error");
                            }

                            WaypointLinkMode = 0;
                        }
                        else
                        {
                            MessageBox.Show("Please select a waypoint to link to.", "Error");
                        }
                    }

                        #endregion

                    else if (KeyState.Get(Keys.ControlKey))
                    {
                        // Add to selection
                        WorldSelectObject((ZoneObject) E.ExtraData, 1);
                    }
                    else if (KeyState.Get(Keys.ShiftKey))
                    {
                        // Remove from selection
                        WorldSelectObject((ZoneObject) E.ExtraData, 2);
                    }
                    else
                    {
                        // Replace selection
                        WorldSelectObject((ZoneObject) E.ExtraData, 3);

                        if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.MOVE)
                        {
                            if (Program.Transformer != null)
                                Program.Transformer.Free();
                            Program.Transformer = null;

                            if (Program.GE.ZoneSelected.Count > 0)
                                if (Program.GE.ZoneSelected[0] is ZoneObject)
                                    (Program.GE.ZoneSelected[0] as ZoneObject).MoveInit();
                        }

                        if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.SCALE)
                        {
                            if (Program.Transformer != null)
                                Program.Transformer.Free();
                            Program.Transformer = null;

                            if (Program.GE.ZoneSelected.Count > 0)
                                if (Program.GE.ZoneSelected[0] is ZoneObject)
                                    (Program.GE.ZoneSelected[0] as ZoneObject).ScaleInit();
                        }

                        if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.ROTATE)
                        {
                            if (Program.Transformer != null)
                                Program.Transformer.Free();
                            Program.Transformer = null;

                            if (Program.GE.ZoneSelected.Count > 0)
                                if (Program.GE.ZoneSelected[0] is ZoneObject)
                                    (Program.GE.ZoneSelected[0] as ZoneObject).RotateInit();
                        }
                    }
                }
            }
                #endregion

                #region Right click
                // Right click
            else if (e.Button == MouseButtons.Right && !Mouselooking)
            {
                if (CreateTimeLimit < 1)
                {
                    switch (Program.GE.SetWorldButtonSelection)
                    {
                            // Place an object
                        case (int) GE.WorldButtonSelection.CREATE:
                            PlaceObjectInZone(MouseX, MouseY);
                            CreateTimeLimit = 1;
                            break;
                    }
                }
            }
                #endregion

                #region 5-button mouse support
            else if (e.Button == MouseButtons.XButton2)
            {
                switch (Program.GE.SetWorldButtonSelection)
                {
                        // Place an object
                    case (int) GE.WorldButtonSelection.MOVE:
                        Program.GE.ObjectRotate_Click(null, null);
                        break;
                    case (int) GE.WorldButtonSelection.ROTATE:
                        Program.GE.ObjectScale_Click(null, null);
                        break;
                    case (int) GE.WorldButtonSelection.SCALE:
                        Program.GE.ObjectMove_Click(null, null);
                        break;
                }
            }
            else if (e.Button == MouseButtons.XButton1)
            {
                switch (Program.GE.SetWorldButtonSelection)
                {
                        // Place an object
                    case (int) GE.WorldButtonSelection.MOVE:
                        Program.GE.ObjectScale_Click(null, null);
                        break;
                    case (int) GE.WorldButtonSelection.ROTATE:
                        Program.GE.ObjectMove_Click(null, null);
                        break;
                    case (int) GE.WorldButtonSelection.SCALE:
                        Program.GE.ObjectRotate_Click(null, null);
                        break;
                }
            }
            #endregion

            Program.GE.m_propertyWindow.RefreshObjectWindow();
        }

        private void SetSelectionPickMode(PickMode Mode)
        {
            ZoneObject Obj = null;
            for (int i = 0; i < Program.GE.ZoneSelected.Count; ++i)
            {
                Obj = (ZoneObject) Program.GE.ZoneSelected[i];
                if (Obj.EN != null)
                    Collision.EntityPickMode(Obj.EN, Mode);
            }
        }

        public void PositionSelection(float X, float Y, float Z)
        {
            ZoneObject Obj = (ZoneObject) Program.GE.ZoneSelected[0];

            if (Obj.EN == null)
                return;

            X -= Obj.EN.X(true);
            Y -= Obj.EN.Y(true);
            Z -= Obj.EN.Z(true);

            MoveSelection(X, Y, Z);
        }

        internal void MoveSelection(float X, float Y, float Z)
        {
            // Create undo point
            if (!SuppressZoneUndo)
            {
                Undo.CreateAdditiveUndo(Undo.Actions.Move, null, X, Y, Z);
            }

            // Move each object
            ZoneObject Obj = null;
            for (int i = 0; i < Program.GE.ZoneSelected.Count; ++i)
            {
                Obj = (ZoneObject) Program.GE.ZoneSelected[i];

                if (Obj.EN != null)
                    Obj.EN.Translate(X, Y, Z);
                Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
                Obj.UpdateTransform();
                // Special cases
                if (Obj is Waypoint)
                {
                    Program.GE.UpdateWaypointLinks();
                }
            }

            // Update spinners if only one object is selected
            if (Program.GE.ZoneSelected.Count == 1)
            {
                SuppressZoneTransforms = true;

                SuppressZoneTransforms = false;
            }

            Program.GE.SetWorldSavedStatus(false);
        }

        private void RotateSelection(float X, float Y, float Z)
        {
            ZoneObject Obj = (ZoneObject) Program.GE.ZoneSelected[0];
            if (Obj.EN != null)
            {
                X -= Obj.EN.Pitch();
                Y -= Obj.EN.Yaw();
                Z -= Obj.EN.Roll();
            }
            
            TurnSelection(X, Y, Z);
        }

        internal void TurnSelection(float X, float Y, float Z)
        {
            // Create undo point
            if (!SuppressZoneUndo)
            {
                Undo.CreateAdditiveUndo(Undo.Actions.Rotate, null, X, Y, Z);
            }

            // Turn each object
            ZoneObject Obj = null;
            for (int i = 0; i < Program.GE.ZoneSelected.Count; ++i)
            {
                Obj = (ZoneObject) Program.GE.ZoneSelected[i];
                // Special cases
                if (Obj is Portal)
                {
                    X = 0f;
                    Z = 0f;
                }

                if (Obj.EN != null)
                {
                    Obj.EN.Turn(X, Y, Z, false);
                    // Special cases
                    if (Obj is Trigger || Obj is Waypoint || Obj is SoundZone || Obj is Water ||
                        Obj is Light)
                    {
                        Obj.EN.Rotate(0f, 0f, 0f);
                    }
                }

                Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
                Obj.UpdateTransform();
            }

            // Update spinners if only one object is selected
            if (Program.GE.ZoneSelected.Count == 1)
            {
                SuppressZoneTransforms = true;

                SuppressZoneTransforms = false;
            }

            Program.GE.SetWorldSavedStatus(false);
        }

        private void ScaleSelectionAbsolute(float X, float Y, float Z)
        {
            // Limit maximum scale
            if (X > 10000000000f)
            {
                X = 10000000000f;
            }

            if (Y > 10000000000f)
            {
                Y = 10000000000f;
            }

            if (Z > 10000000000f)
            {
                Z = 10000000000f;
            }

            ZoneObject Obj = (ZoneObject) Program.GE.ZoneSelected[0];
            // Special cases
            if (Obj is SoundZone || Obj is Portal || Obj is Trigger || Obj is Light)
            {
                Y = X;
                Z = X;
            }
            else if (Obj is Waypoint)
            {
                //if (Program.GE.CurrentServerZone.GetSpawnPoint(((Waypoint) Obj).ServerID) < 0)
                if((Obj as Waypoint).Max == 0)
                {
                    X = 1f;
                }
                Y = X;
                Z = X;
            }
            else if (Obj is Water)
            {
                Y = 20f;
            }

            // Create undo point
            if (!SuppressZoneUndo)
            {
                if (Obj.EN != null)
                    Undo.CreateNonRepeatingUndo(Undo.Actions.Scale, "A", Obj.EN.ScaleX() * 20f, Obj.EN.ScaleY() * 20f,
                                            Obj.EN.ScaleZ() * 20f);
            }

            // Perform scale
            if (Obj.EN != null)
                Obj.EN.Scale(X * 0.05f, Y * 0.05f, Z * 0.05f, false);
            // Special cases
            Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
            Obj.UpdateTransform();
            // Update spinner values
            SuppressZoneTransforms = true;
        }

        private void ScaleSelection(float X, float Y, float Z)
        {
            X += 1f;
            Y += 1f;
            Z += 1f;

            // Create undo point
            if (!SuppressZoneUndo)
            {
                Undo.CreateMultiplyUndo(Undo.Actions.Scale, "R", X, Y, Z);
            }

            // Scale each object
            ZoneObject Obj = null;
            for (int i = 0; i < Program.GE.ZoneSelected.Count; ++i)
            {
                Obj = (ZoneObject) Program.GE.ZoneSelected[i];
                if (Obj is Light)
                {
                    continue;
                }
                // Special cases
                if (Obj is SoundZone || Obj is Portal || Obj is Trigger || Obj is Light)
                {
                    Y = X;
                    Z = X;
                }
                else if (Obj is Waypoint)
                {
                    //if (Program.GE.CurrentServerZone.GetSpawnPoint(((Waypoint) Obj).ServerID) < 0)
                    if((Obj as Waypoint).Max == 0)
                    {
                        X = 1f;
                    }

                    Y = X;
                    Z = X;
                }
                else if (Obj is Water)
                {
                    Y = 1f;
                }

                if(Obj.EN != null)
                    Obj.EN.ScaleRelative(X, Y, Z, false);

                // Special cases
                Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
                Obj.UpdateTransform();
            }

            Program.GE.SetWorldSavedStatus(false);
        }

        private static float Lerp(float Start, float Dest, float Proportion)
        {
            return Start + ((Dest - Start) / Proportion);
        }

        public void WorldSelectObject(ZoneObject Obj, int Mode)
        {
            switch (Mode)
            {
                    // Add to selection
                case 1:
                    if (!Program.GE.ZoneSelected.Contains(Obj))
                    {
                        Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(Obj, true);
                    }

                    break;
                    // Remove from selection
                case 2:
                    if (Program.GE.ZoneSelected.Contains(Obj))
                    {
                        Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(Obj, false);
                    }

                    break;
                    // Replace selection
                case 3:
                    // Clear current selections

                    if(Program.Transformer != null)
                    {
                        Program.Transformer.Free();
                        Program.Transformer = null;
                    }

                    int Last = 0;
                    while (Program.GE.ZoneSelected.Count > 0)
                    {
                        // Abort if stuck
                        if (Program.GE.ZoneSelected.Count == Last)
                            break;
                        Last = Program.GE.ZoneSelected.Count;

                        Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection((ZoneObject) Program.GE.ZoneSelected[0],
                                                                               false);
                        
                    }

                    // Add new
                    Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(Obj, true);
                    break;
            }

            Program.GE.m_propertyWindow.RefreshObjectWindow();
        }

        public void PlaceObjectInZone(int MouseX, int MouseY)
        {
            DialogResult Result;

            // Cancel if no zone loaded
            if (Program.GE.CurrentClientZone == null && Program.GE.PlaceObject > 0)
            {
                MessageBox.Show("You must create or load a zone before placing objects.", "Error");
                return;
            }

            #region Get position for placement
            float PosX, PosY, PosZ;
            float NormalX = 0f, NormalY = 1f, NormalZ = 0f;
            //Collision.EntityType(Plane, (byte) CollisionType.None);
            Collision.EntityPickMode(Plane, PickMode.Unpickable);
            Entity E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
            if (E != null && Program.IsRendered(E))
            {
                PosX = Collision.PickedX();
                PosY = Collision.PickedY();
                PosZ = Collision.PickedZ();
                NormalX = Collision.PickedNX();
                NormalY = Collision.PickedNY();
                NormalZ = Collision.PickedNZ();
            }
            else
            {
                Plane.Position(0, Program.GE.GridHeight, 0);
              //  Collision.EntityType(Plane, (byte) CollisionType.Box);
               // Collision.SetCollisionMesh(Plane);
                Collision.EntityPickMode(Plane, PickMode.Polygon);
                Collision.UpdateWorld();
                E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
                if (E != null && Program.IsRendered(E))
                {
                    PosX = Collision.PickedX();
                    PosY = Collision.PickedY();
                    PosZ = Collision.PickedZ();
                    NormalX = Collision.PickedNX();
                    NormalY = Collision.PickedNY();
                    NormalZ = Collision.PickedNZ();
                }
                else
                {
                    PosX = Program.GE.Camera.X();
                    PosY = Program.GE.Camera.Y();
                    PosZ = Program.GE.Camera.Z();
                }
            }
            #endregion

            // Object type
            bool Found;
            switch (Program.GE.PlaceObject)
            {
                    #region Entire new zone
                case 0:
                    // Save current world state?
                    if (!Program.GE.WorldSaved)
                    {
                        Result = MessageBox.Show("Save changes to current zone first?", "New zone",
                                                 MessageBoxButtons.YesNoCancel);
                        if (Result == DialogResult.Yes)
                        {
                            Program.GE.SaveWorld();
                        }
                        else if (Result == DialogResult.Cancel)
                        {
                            return;
                        }
                    }

                    // Get name for new zone
                    TextEntry TE = new TextEntry();
                    TE.Text = "Create zone";
                    TE.Description.Text = "Enter a name for the new zone:";
                    TE.ShowDialog();
                    string ZoneName = TE.Result;
                    TE.Dispose();
                    if (string.IsNullOrEmpty(ZoneName) || string.IsNullOrEmpty(ZoneName.Replace(" ", "")))
                    {
                        return;
                    }

                    // Check a zone with this name doesn't already exist
                    string ExistingZoneName;
                    /*   for (int i = 0; i < Program.GE.m_ZoneList.WorldZonesTree.GetNodeCount(false); ++i)
                       {
                           ExistingZoneName = (string)Program.GE.m_ZoneList.WorldZonesTree.Nodes[i].Name;
                           if (ExistingZoneName.ToUpper() == ZoneName.ToUpper())
                           {
                               MessageBox.Show("A zone with that name already exists.", "Error");
                               return;
                           }
                       }*/
                    string[] ZoneFiles = Directory.GetFiles(@"Data\Areas\", "*.dat");
                    foreach (string ZF in ZoneFiles)
                    {
                        ExistingZoneName = Path.GetFileNameWithoutExtension(ZF);
                        if (ExistingZoneName.ToUpper() == ZoneName.ToUpper())
                        {
                            MessageBox.Show("A zone with that name already exists.", "Error");
                            return;
                        }
                    }

                    // Unload current zone, if any
                    Program.GE.UnloadCurrentZone();

                    // Create new zone
                    Program.GE.CurrentClientZone = new Zone(ZoneName);
                    Program.GE.CurrentServerZone = new RealmCrafter.ServerZone.Zone(ZoneName);
                    Program.GE.CurrentClientZone.Save();
                    Program.GE.CurrentServerZone.Save(Program.GE.CurrentClientZone);

                    // Add to UI
                    Program.GE.TotalZones++;
                    Program.GE.ActorStartZoneCombo.Items.Add(ZoneName);
                    //Program.GE.WorldPlacePortalLinkCombo.Items.Add(ZoneName);
                    //Program.GE.WorldObjectPortalLinkCombo.Items.Add(ZoneName);
                    //Program.GE.WorldZoneWeatherLinkCombo.Items.Add(ZoneName);
                    //Program.GE.ProjectZones.Text = "Zones: " + Program.GE.TotalZones;

                    // Reload tree
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Clear();
                    Program.GE.TotalZones = 0;
                    Program.GE.ActorStartZoneCombo.Items.Clear();
                    Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.Items.Clear();
                    string[] Zones = Directory.GetFiles(@"Data\Areas\", "*.dat");
                    foreach (string S_ in Zones)
                    {
                        Program.GE.TotalZones++;
                        string Name = Path.GetFileNameWithoutExtension(S_);

                        Program.GE.ActorStartZoneCombo.Items.Add(Name);
                        Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.Items.Add(Name);
                        //Program.GE.WorldPlacePortalLinkCombo.Items.Add(Name);
                        //Program.GE.WorldObjectPortalLinkCombo.Items.Add(Name);
                        //Program.GE.WorldZoneWeatherLinkCombo.Items.Add(Name);
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add(Name);
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[Program.GE.TotalZones - 1].Name = Name;
                    }

                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Clear();
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Scenery objects");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[0].Name = "Node0";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Terrains");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Name = "Node1";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Emitters");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[2].Name = "Node2";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Water areas");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[3].Name = "Node3";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Collision boxes");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[4].Name = "Node4";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Sound zones");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[5].Name = "Node5";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Dynamic lights");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[6].Name = "Node6";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Triggers");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[7].Name = "Node7";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Waypoints");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Name = "Node8";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Portals");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[9].Name = "Node9";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Tree Placers");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[10].Name = "Node10";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Trees");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[11].Name = "Node11";
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Menu Controls");
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[12].Name = "Node12";

                    // end reload
                    Program.GE.UpdateZoneList();

                    // Reset Program.GE.Camera
                    WorldCamDPitch = 0f;
                    WorldCamDYaw = 0f;
                    WorldCamPitch = 0f;
                    WorldCamYaw = 0f;
                    WorldCamX = 0f;
                    WorldCamY = 0f;
                    WorldCamZ = 0f;
                    Program.GE.Camera.Position(0, 0, 0);
                    Program.GE.Camera.Rotate(0, 0, 0);
                    break;
                    #endregion

                    #region Scenery object
                case 1:
                    // Cancel if no mesh selected
                    if ((Program.GE.PlaceSceneryID == 65535) || Media.GetMeshName(Program.GE.PlaceSceneryID) == "")
                    {
                        MessageBox.Show("You must select a valid mesh before placement.", "Error");
                        return;
                    }

                    // Place mesh
                    Scenery S = new Scenery(Program.GE.CurrentClientZone, Program.GE.PlaceSceneryID);
                    S.EN.Position(PosX, PosY, PosZ);
                    if (Program.GE.m_CreateWindow.WorldPlaceAlignCheck.Checked)
                    {
                        S.EN.AlignToVector(NormalX, NormalY, NormalZ, 2);
                    }

                    if (Program.GE.m_CreateWindow.WorldPlaceGridCheck.Checked)
                    {
                        S.EN.Move(0, -S.EN.Y() + Program.GE.GridHeight, 0); // position to mesh
                    }

                    float Scale = Media.LoadedMeshScales[Program.GE.PlaceSceneryID] * 0.05f;
                    S.EN.Scale(Scale, Scale, Scale);
                    Collision.EntityType(S.EN, (byte) CollisionType.Triangle);
                    Collision.SetCollisionMesh(S.EN);
                    Collision.EntityPickMode(S.EN, PickMode.Polygon);

                    // Create undo
                    if (!SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Create, S);
                    }

                    // Add to tree view and select
                    string Name_mesh = Media.GetMeshName(S.MeshID);
                    TreeNode TN =
                        new TreeNode(
                            Path.GetFileNameWithoutExtension(Name_mesh.Substring(0, Name_mesh.Length - 1)));
                    TN.Tag = S;
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[0].Nodes.Add(TN);
                    WorldSelectObject(S, 3);
                    break;
                    #endregion

                    #region Terrain
                case 2:
                    Forms.CreateTerrainDialog CreateDialog = new RealmCrafter_GE.Forms.CreateTerrainDialog();
                    Result = CreateDialog.ShowDialog();

                    if (Result == DialogResult.OK)
                    {
                        // Create terrain
                        RCTTerrain T = new RCTTerrain(Program.GE.CurrentClientZone);
                        T.Path = @".\Data\Terrains\" + Program.GE.CurrentClientZone.Name + System.Environment.TickCount.ToString() + ".te";
                        T.Terrain = GE.TerrainManager.CreateT1(CreateDialog.TerrainSize);
                        T.Terrain.Tag = new List<Program.TerrainTagItem>();

;

                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, T);
                        }

                        // Add to tree view and select
                        TN = new TreeNode("Terrain " + (Program.GE.CurrentClientZone.Terrains.IndexOf(T) + 1));
                        TN.Tag = T;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
                        WorldSelectObject(T, 3);
                    }

                    break;
                    #endregion

                    #region Emitter
                case 3:
                    string EmitterName = (string) Program.GE.m_CreateWindow.WorldPlaceEmitterCombo.SelectedItem;
                    if (!string.IsNullOrEmpty(EmitterName))
                    {
                        Emitter Emi = new Emitter(Program.GE.CurrentClientZone, EmitterName, Program.GE.Camera, true);
                        // Get texture
                        if (Emi.TextureID < 65535)
                        {
                            uint Tex = Media.GetTexture(Emi.TextureID, true);
                            if (Tex != 0)
                            {
                                Emi.Config.ChangeTexture(Tex);
                                Media.UnloadTexture(Emi.TextureID);
                            }
                            else
                            {
                                Emi.Config.ChangeTexture(Program.GE.DefaultParticleTexture);
                            }
                        }
                        else
                        {
                            Emi.Config.ChangeTexture(Program.GE.DefaultParticleTexture);
                        }

                        // Create emitter
                        Entity EmitterEN = General.CreateEmitter(Emi.Config);
                        EmitterEN.Parent(Emi.EN, false);
                        Emi.EN.Position(PosX, PosY, PosZ);

                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, Emi);
                        }

                        // Add to tree view and select
                        if (Emi.Config != null)
                        {
                            TN = new TreeNode(Emi.Config.Name);
                        }
                        else
                        {
                            TN = new TreeNode("Unknown emitter");
                        }

                        TN.Tag = Emi;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[2].Nodes.Add(TN);
                        WorldSelectObject(Emi, 3);
                    }
                    else
                    {
                        MessageBox.Show("Please select an emitter to place from the drop down list", "Error");
                    }

                    break;
                    #endregion

                    #region Water
                case 4:
                    // Cancel if no texture selected
                    if (Program.GE.PlaceWaterID == 65535)
                    {
                        MessageBox.Show("You must select a valid texture before placement.", "Error");
                        return;
                    }

                    // Place water
                    Water W = new Water(Program.GE.CurrentClientZone);
                    WaterArea SW = new WaterArea(Program.GE.CurrentServerZone);
                    W.ServerWater = SW;
                    W.TextureID[0] = Program.GE.PlaceWaterID;
                    W.TexHandle[0] = Media.GetTexture(Program.GE.PlaceWaterID, false);
                    //W.TextureID[1] = Media.AddTextureToDatabase(@"Data\Textures\Water\Shader Water.png", 0);
                    W.TexHandle[1] = Media.GetTexture((int)W.TextureID[1], false);
//                    W.TexHandle[1] = Render.LoadTexture(@"Data\Textures\Water\Shader Water.png");
                    if ((uint)W.TexHandle[0] != 0)
                    {
                        Render.ScaleTexture((uint)W.TexHandle[0], 1f / W.TexScale, 1f / W.TexScale);
                        W.EN.Texture((uint)W.TexHandle[0], 1);
                    }

                    if ((uint)W.TexHandle[1] != 0)
                    {
                        Render.ScaleTexture((uint)W.TexHandle[1], 1f / W.TexScale, 1f / W.TexScale);
                        W.EN.Texture((uint)W.TexHandle[1]);
                    }

                    W.EN.Position(PosX, PosY, PosZ);
                    W.UpdateServerVersion(Program.GE.CurrentServerZone);
                    Collision.EntityType(W.EN, (byte) CollisionType.Box);
                    Collision.SetCollisionMesh(W.EN);
                    Collision.EntityPickMode(W.EN, PickMode.Polygon);
                    // Create undo
                    if (!SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Create, W);
                    }
                    // Add to tree view and select
                    Name_mesh = Media.GetTextureName(Program.GE.PlaceWaterID);
                    TN =
                        new TreeNode(
                            Path.GetFileNameWithoutExtension(Name_mesh.Substring(0, Name_mesh.Length - 1)));
                    TN.Tag = W;
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[3].Nodes.Add(TN);
                    WorldSelectObject(W, 3);
                    break;
                    #endregion

                    #region Collision box
                case 5:
                    // Create box
                    ColBox CB = new ColBox(Program.GE.CurrentClientZone, true);
                    CB.EN.Position(PosX, PosY, PosZ);
                    // Create undo
                    if (!SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Create, CB);
                    }
                    // Add to tree view and select
                    TN = new TreeNode("Collision box " + Program.GE.CurrentClientZone.ColBoxes.Count);
                    TN.Tag = CB;
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[4].Nodes.Add(TN);
                    WorldSelectObject(CB, 3);
                    break;
                    #endregion

                    #region Sound zone
                case 6:
                    // Create sound zone
                    SoundZone SZ = new SoundZone(Program.GE.CurrentClientZone, true);
                    SZ.EN.Texture(Program.GE.OrangeTex);
                    SZ.EN.Position(PosX, PosY, PosZ);
                    // Create undo
                    if (!SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Create, SZ);
                    }
                    // Add to tree view and select
                    TN = new TreeNode("Unknown sound");
                    TN.Tag = SZ;
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[5].Nodes.Add(TN);
                    WorldSelectObject(SZ, 3);
                    break;
                    #endregion

                    #region Dynamic light
                case 7:
                    Light L = new Light(Program.GE.CurrentClientZone,
                                        true);
                    L.EN.Position(PosX, PosY, PosZ);

                    if (Program.GE.m_CreateWindow.WorldPlaceGridCheck.Checked)
                    {
                        L.EN.Move(0, -L.EN.Y() + Program.GE.GridHeight, 0); // position to mesh
                    }

                    Collision.EntityType(L.EN, (byte) CollisionType.Triangle);
                    Collision.SetCollisionMesh(L.EN);
                    Collision.EntityPickMode(L.EN, PickMode.Polygon);

                    // Undo
                    if (!SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Create, L);
                    }

                    // Add to tree and select
                    string tName_mesh = "Light";
                    TreeNode tTN = new TreeNode(tName_mesh);
                    tTN.Tag = L;
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[6].Nodes.Add(tTN);
                    WorldSelectObject(L, 3);

                    break;
                    #endregion
                #region MenuControl
                case 14:

                    MenuControl Mc = new MenuControl(Program.GE.CurrentClientZone,
                        true);

                    Mc.EN.Position(PosX, PosY, PosZ);

                    if (Program.GE.m_CreateWindow.WorldPlaceGridCheck.Checked)
                    {
                        Mc.EN.Move(0, -Mc.EN.Y() + Program.GE.GridHeight, 0); // position to mesh
                    }

                    Collision.EntityType(Mc.EN, (byte)CollisionType.Triangle);
                    Collision.SetCollisionMesh(Mc.EN);
                    Collision.EntityPickMode(Mc.EN, PickMode.Polygon);

                    // Undo
                    if (!SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Create, Mc);
                    }

                    // Add to tree and select
                    TN = new TreeNode("MenuControl");
                    TN.Tag = Mc;
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[12].Nodes.Add(TN);
                    WorldSelectObject(Mc, 3);


                    break;
                #endregion

                #region Trigger
                case 8:
                    if (Program.GE.m_CreateWindow.WorldPlaceTriggerScriptCombo.SelectedItem != null)
                    {
                        //                         // Find a free one
                        //                         Found = false;
                        //                         for (int i = 0; i < 150; ++i)
                        //                         {
                        //                             if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.TriggerScript[i]))
                        //                             {

                        // Set up new trigger
                        Trigger T = new Trigger(Program.GE.CurrentServerZone, Program.GE, Program.GE.CurrentServerZone.Triggers.Count);

                        T.Script = (string)Program.GE.m_CreateWindow.WorldPlaceTriggerScriptCombo.SelectedItem;
                        T.Size = 5;
                        T.X = PosX;
                        T.Y = PosY;
                        T.Z = PosZ;

                        T.ReBuild();
                           

                        T.EN.Position(PosX, PosY, PosZ);
                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, T);
                        }

                        // Add to tree view and select
                        TN = new TreeNode(T.Script);
                        TN.Tag = T;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[7].Nodes.Add(TN);
                        WorldSelectObject(T, 3);

                        //Found = true;
                        //break;
                        //                             }
                        //                         }
                        //                         // No spare trigger - tell user
                        //                         if (!Found)
                        //                         {
                        //                             MessageBox.Show("Maximum number of triggers already placed in this zone", "Error");
                        //                         }
                    }
                    // No script/function selected
                    else
                    {
                        MessageBox.Show("Select a script/function for this trigger", "Error");
                    }

                    break;
                    #endregion

                    #region Portal
                case 10:
                    if (!string.IsNullOrEmpty(Program.GE.m_CreateWindow.WorldPlacePortalNameText.Text))
                    {
                        //                         // Find a free one
                        //                         Found = false;
                        //                         for (int i = 0; i < 100; ++i)
                        //                         {
                        //                             if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.PortalName[i]))
                        //                             {


                        // Make sure portal with same name doesnt exist
                        bool foundPortal = false;
                        foreach (Portal port in Program.GE.CurrentServerZone.Portals)
                            if (port.Name == Program.GE.m_CreateWindow.WorldPlacePortalNameText.Text)
                            {
                                foundPortal = true;
                                break;
                            }

                        if (foundPortal)
                        {
                            MessageBox.Show("A portal with this name already exists in the zone.", "Error");
                            break;
                        }

                        // Set up new portal
                        Portal P = new Portal(Program.GE.CurrentServerZone, Program.GE, Program.GE.CurrentServerZone.Portals.Count);

                        P.Name = Program.GE.m_CreateWindow.WorldPlacePortalNameText.Text;

                        if (Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.SelectedItem != null)
                        {
                            P.LinkArea = (string)Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.SelectedItem;
                        }
                        else
                        {
                            P.LinkName = "";
                        }

                        if (Program.GE.m_CreateWindow.WorldPlacePortalLinkNameCombo.SelectedItem != null)
                        {
                            P.LinkName = (string)Program.GE.m_CreateWindow.WorldPlacePortalLinkNameCombo.SelectedItem;
                        }
                        else
                        {
                            P.LinkName = "";
                        }



                        P.Size = 10.0f;
                        P.X = PosX;
                        P.Y = PosY;
                        P.Z = PosZ;
                        P.ReBuild();
                        P.EN.Position(PosX, PosY, PosZ);
                        P.EN.Scale(10f, 10f, 10f);
                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, P);
                        }
                        // Add to tree view and select
                        TN = new TreeNode(P.Name);
                        TN.Tag = P;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[9].Nodes.Add(TN);
                        WorldSelectObject(P, 3);

                        //                                 Found = true;
                        //                                 break;
                        //                             }
                        //                         }
                        //                         // No spare portal - tell user
                        //                         if (!Found)
                        //                         {
                        //                             MessageBox.Show("Maximum number of portals already placed in this zone", "Error");
                        //                         }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a name for your portal", "Error");
                    }
                    break;
                    #endregion

                    #region Waypoint
                case 9:
                    {
                        //                     // Find a free one
                        //                     Found = false;
                        //                     for (int i = 0; i < 2000; ++i)
                        //                     {
                        //                         if (Program.GE.CurrentServerZone.PrevWaypoint[i] == 2005)
                        //                         {

                        // Set up new waypoint
                        Waypoint WP = new Waypoint(Program.GE.CurrentServerZone, Program.GE, Program.GE.CurrentServerZone.Waypoints.Count);

                        WP.Prev = null;
                        WP.NextA = null;
                        WP.NextB = null;
                        WP.X = PosX;
                        WP.Y = PosY;
                        WP.Z = PosZ;

                        WP.EN.Position(PosX, PosY + 3f, PosZ);
                        WP.EN.Scale(3f, 3f, 3f);
                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, WP);
                        }
                        // Add to tree view and select
                        WP.WaypointEN = WP.EN;
                        TN = new TreeNode("Waypoint " + Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Count);
                        TN.Tag = WP;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Add(TN);
                        WorldSelectObject(WP, 3);

                        // 
                        //                             Found = true;
                        //                             break;
                        //                         }
                        //                     }
                        //                     // No spare waypoint - tell user
                        //                     if (!Found)
                        //                     {
                        //                         MessageBox.Show("Maximum number of waypoints already placed in this zone", "Error");
                        //                     }

                        break;
                    }
                    #endregion

                    #region Duplicate selection
                case 11:
                    // Duplicate all objects and add the new objects to a list
                    LinkedList<ZoneObject> CreatedObjects = new LinkedList<ZoneObject>();
                    for (int i = 0; i < Program.GE.ZoneSelected.Count; ++i)
                    {
                        ZoneObject ZO = WorldDuplicateObject((ZoneObject) Program.GE.ZoneSelected[i]);
                        if (ZO != null)
                        {
                            CreatedObjects.AddLast(ZO);
                        }
                    }
                    // Clear current selections
                    while (Program.GE.ZoneSelected.Count > 0)
                    {
                        Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection((ZoneObject) Program.GE.ZoneSelected[0],
                                                                               false);
                    }
                    // Select all newly created objects
                    LinkedListNode<ZoneObject> CreatedObjectsNode = CreatedObjects.First;
                    while (CreatedObjectsNode != null)
                    {
                        WorldSelectObject(CreatedObjectsNode.Value, 1);
                        CreatedObjectsNode = CreatedObjectsNode.Next;
                    }

                    CreatedObjects.Clear();
                    break;
                    #endregion

                    #region Simple Tree Placement
                case 12:

                    // Check a tree was selected
                    object SelectedNode = Program.GE.m_CreateWindow.WorldPlaceTreeCombo.SelectedItem;
                    if (SelectedNode == null)
                        break;
                    if(!(SelectedNode is StoredTree))
                        break;
                    if(Program.Manager == null)
                        break;
                    if(Program.CurrentTreeZone == null)
                        break;

                    StoredTree Store = SelectedNode as StoredTree;
                    if (Store.LTType == null)
                        break;

                    // Get some randoms
                    float Yaw = 0.0f;
                    float ScaleY = 1.0f;

                    if(Program.GE.m_CreateWindow.TreeRandomRotationCheckbox.Checked)
                        Yaw = Convert.ToSingle(new Random().NextDouble()) * 360.0f;

                    float Offset = Convert.ToSingle(Program.GE.m_CreateWindow.TreeHeightVarianceSpinner.Value);
                    ScaleY += (Convert.ToSingle(new Random().NextDouble()) * Offset) - (Offset * 0.5f);

                    // Checks complete, lets place this thing!
                    LTNet.TreeInstance Instance = Program.CurrentTreeZone.AddTreeInstance(Store.LTType, PosX, PosY, PosZ);
                    Instance.SetScale(1.0f, ScaleY, 1.0f);
                    Instance.SetRotation(0, Yaw, 0);

                    break;
                    #endregion

                    #region Tree Placer Objects
                case 13:

                    TreePlacerArea A = new TreePlacerArea(Program.GE.CurrentClientZone, PosX, PosZ);

                    if (!SuppressZoneUndo)
                    {
                        new Undo(Undo.Actions.Create, A);
                    }

                    Program.GE.m_ZoneList.WorldZonesTree.Nodes[10].Nodes.Add(A.TN);
                    WorldSelectObject(A, 3);
                    break;


                    #endregion
            }
            Program.GE.m_ZoneList.AddObjectsCount();
            Program.GE.SetWorldSavedStatus(false);
        }
        public void PositionCamera(float X, float Y, float Z)
        {
            Program.GE.Camera.Position(X, Y, Z);
        }

        public ZoneObject WorldDuplicateObject(ZoneObject Z)
        {
            // Scenery
            if (Z is Scenery)
            {
                // Copy object details
                Scenery S = (Scenery) Z;
                Scenery SNew = new Scenery(Program.GE.CurrentClientZone, S.MeshID);
                SNew.AnimationMode = S.AnimationMode;
                SNew.CatchRain = S.CatchRain;
                SNew.Lightmap = S.Lightmap;
                SNew.RCTE = S.RCTE;
                SNew.TextureID = S.TextureID;
                SNew.EN.Position(S.EN.X(), S.EN.Y(), S.EN.Z());
                SNew.EN.Rotate(S.EN.Pitch(), S.EN.Yaw(), S.EN.Roll());
                SNew.EN.Scale(S.EN.ScaleX(), S.EN.ScaleY(), S.EN.ScaleZ());
                Collision.EntityType(SNew.EN, (byte) CollisionType.Triangle);
                Collision.SetCollisionMesh(SNew.EN);
                Collision.EntityPickMode(SNew.EN, PickMode.Polygon);
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, SNew);
                }
                // Add to tree view
                string Name = Media.GetMeshName(SNew.MeshID);
                TreeNode TN =
                    new TreeNode(System.IO.Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
                TN.Tag = SNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[0].Nodes.Add(TN);
                return SNew;
            }
                // Terrain
            else if (Z is RCTTerrain)
            {
                RCTTerrain OldTerrain = Z as RCTTerrain;
                string NewPath = "";

                // Copy file
                try
                {
                    NewPath = @".\Data\Terrains\" + Program.GE.CurrentClientZone.Name + System.Environment.TickCount.ToString() + ".te";
                    File.Copy(OldTerrain.Path, NewPath);
                }
                catch(System.Exception e)
                {
                    MessageBox.Show("Error copying terrain!\n" + e.Message);
                    return null;
                }

                if (NewPath.Length == 0)
                    MessageBox.Show("Path not generated!");

                RCTTerrain T = new RCTTerrain(Program.GE.CurrentClientZone);
                T.Path = NewPath;
                T.Terrain = GE.TerrainManager.LoadT1(NewPath);
                T.Terrain.Tag = new List<Program.TerrainTagItem>();

                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, T);
                }

                // Add to tree view and select
                TreeNode TN = new TreeNode("Terrain " + (Program.GE.CurrentClientZone.Terrains.IndexOf(T) + 1));
                TN.Tag = T;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
                WorldSelectObject(T, 3);

                // Copy object details
                //Terrain T = (Terrain) Z;
                //Terrain TNew = new Terrain(Program.GE.CurrentClientZone);
                //TNew.DetailTexScale = T.DetailTexScale;
                //TNew.BaseTexID = T.BaseTexID;
                //TNew.DetailTexID = T.DetailTexID;
                //TNew.Detail = T.Detail;
                //TNew.Morph = T.Morph;
                //TNew.Shading = T.Shading;
                //TNew.GridSize = T.GridSize;
                //TNew.Heights = (float[,]) T.Heights.Clone();
                //// Parent mesh
                //TNew.EN = Entity.CreateMesh();
                //uint Surf = TNew.EN.CreateSurface();
                //Entity.AddVertex(Surf, 0f, 0f, 0f, 0f, 0f);
                //Entity.AddVertex(Surf, (float) TNew.GridSize, 0f, 0f, 1f, 0f);
                //Entity.AddVertex(Surf, (float) TNew.GridSize, 0f, (float) TNew.GridSize, 1f, 1f);
                //Entity.AddVertex(Surf, 0f, 0f, (float) TNew.GridSize, 0f, 1f);
                //Entity.AddVertex(Surf, 0f, 1f, 0f);
                //// Child meshes
                //int Chunks = TNew.GridSize / Terrain.ChunkDetail;
                //for (int ChunkX = 0; ChunkX < Chunks; ++ChunkX)
                //{
                //    for (int ChunkZ = 0; ChunkZ < Chunks; ++ChunkZ)
                //    {
                //        Entity EN = Entity.CreateMesh();
                //        Surf = EN.CreateSurface();
                //        EN.Shader = Shaders.Terrain;
                //        int V1, V3;
                //        float VX, VZ, TrueVX, TrueVZ;
                //        float GridSizeF = (float) T.GridSize + 1f;
                //        // Vertices
                //        for (int TerrainX = 0; TerrainX <= Terrain.ChunkDetail; ++TerrainX)
                //        {
                //            for (int TerrainZ = 0; TerrainZ <= Terrain.ChunkDetail; ++TerrainZ)
                //            {
                //                VX = (float) TerrainX;
                //                VZ = (float) TerrainZ;
                //                TrueVX = (float) ((ChunkX * Terrain.ChunkDetail) + TerrainX);
                //                TrueVZ = (float) ((ChunkZ * Terrain.ChunkDetail) + TerrainZ);
                //                Entity.AddVertex(Surf,
                //                                 VX,
                //                                 T.Heights[
                //                                     (ChunkX * Terrain.ChunkDetail) + TerrainX,
                //                                     (ChunkZ * Terrain.ChunkDetail) + TerrainZ],
                //                                 VZ,
                //                                 TrueVX / GridSizeF,
                //                                 1f - (TrueVZ / GridSizeF));
                //            }
                //        }
                //        // Triangles
                //        for (int TerrainX = 0; TerrainX < Terrain.ChunkDetail; ++TerrainX)
                //        {
                //            for (int TerrainZ = 0; TerrainZ < Terrain.ChunkDetail; ++TerrainZ)
                //            {
                //                V1 = (TerrainX * (Terrain.ChunkDetail + 1)) + TerrainZ;
                //                V3 = ((TerrainX + 1) * (Terrain.ChunkDetail + 1)) + TerrainZ;
                //                Entity.AddTriangle(Surf, V1 + 1, V3, V1);
                //                Entity.AddTriangle(Surf, V1 + 1, V3 + 1, V3);
                //            }
                //        }
                //        EN.UpdateNormals();
                //        EN.UpdateHardwareBuffers();
                //        Collision.EntityType(EN, (byte) CollisionType.Triangle);
                //        Collision.SetCollisionMesh(EN);
                //        Collision.EntityPickMode(EN, PickMode.Polygon);
                //        EN.Parent(TNew.EN, true);
                //        EN.Position(ChunkX * Terrain.ChunkDetail, 0f, ChunkZ * Terrain.ChunkDetail);
                //        EN.ExtraData = TNew;
                //    }
                //}
                //// Apply textures
                //if (T.BaseTexID < 65535)
                //{
                //    uint Tex = Media.GetTexture(T.BaseTexID, false);
                //    if (Tex != 0)
                //    {
                //        for (int j = 1; j <= T.EN.CountChildren(); ++j)
                //        {
                //            T.EN.GetChild(j).Texture(Tex);
                //        }
                //        Media.UnloadTexture(T.BaseTexID);
                //    }
                //}
                //if (T.DetailTexID < 65535)
                //{
                //    T.DetailTex = Media.GetTexture(T.DetailTexID, false);
                //    if (T.DetailTex != 0)
                //    {
                //        Render.ScaleTexture(T.DetailTex, T.DetailTexScale * 0.01f, T.DetailTexScale * 0.01f);
                //        for (int j = 1; j <= T.EN.CountChildren(); ++j)
                //        {
                //            T.EN.GetChild(j).Texture(T.DetailTex, 1);
                //        }
                //    }
                //}
                //// Transform
                //TNew.EN.Position(T.EN.X(), T.EN.Y(), T.EN.Z());
                //TNew.EN.Rotate(T.EN.Pitch(), T.EN.Yaw(), T.EN.Roll());
                //TNew.EN.Scale(T.EN.ScaleX(), T.EN.ScaleY(), T.EN.ScaleZ());
                // Create undo
                //if (!SuppressZoneUndo)
                //{
                //    new Undo(Undo.Actions.Create, TNew);
                //}
                //// Add to tree view
                //TreeNode TN =
                //    new TreeNode("Terrain " + (Program.GE.CurrentClientZone.Terrains.IndexOf(TNew) + 1).ToString());
                //TN.Tag = TNew;
                //Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
                //return TNew;
            }
                // Emitter
            else if (Z is Emitter)
            {
                // Copy object details
                Emitter E = (Emitter) Z;
                Emitter ENew = new Emitter(Program.GE.CurrentClientZone, E.ConfigName, Program.GE.Camera, true);
                ENew.TextureID = E.TextureID;
                if (ENew.TextureID < 65535)
                {
                    uint Tex = Media.GetTexture(ENew.TextureID, false);
                    if (Tex != 0)
                    {
                        ENew.Config.ChangeTexture(Tex);
                        Media.UnloadTexture(ENew.TextureID);
                    }
                    else
                    {
                        ENew.Config.ChangeTexture(Program.GE.DefaultParticleTexture);
                    }
                }
                else
                {
                    ENew.Config.ChangeTexture(Program.GE.DefaultParticleTexture);
                }
                Entity EmitterEN = RottParticles.General.CreateEmitter(ENew.Config);
                EmitterEN.Parent(ENew.EN, false);
                ENew.EN.Position(E.EN.X(), E.EN.Y(), E.EN.Z());
                ENew.EN.Rotate(E.EN.Pitch(), E.EN.Yaw(), E.EN.Roll());
                ENew.EN.Scale(E.EN.ScaleX(), E.EN.ScaleY(), E.EN.ScaleZ());
                Collision.EntityType(ENew.EN, (byte) CollisionType.Triangle);
                Collision.SetCollisionMesh(ENew.EN);
                Collision.EntityPickMode(ENew.EN, PickMode.Polygon);
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, ENew);
                }
                // Add to tree view
                TreeNode TN;
                if (ENew.Config != null)
                {
                    TN = new TreeNode(ENew.Config.Name);
                }
                else
                {
                    TN = new TreeNode("Unknown emitter");
                }
                TN.Tag = ENew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[2].Nodes.Add(TN);
                return ENew;
            }
                // Water
            else if (Z is Water)
            {
                // Copy object details
                Water W = (Water) Z;
                Water WNew = new Water(Program.GE.CurrentClientZone);
                WNew.Alpha = W.Alpha;
                WNew.Blue = W.Blue;
                WNew.Green = W.Green;
                WNew.Red = W.Red;
                WNew.TexScale = W.TexScale;
                WaterArea SW = new WaterArea(Program.GE.CurrentServerZone);
                WNew.ServerWater = SW;
                SW.Damage = W.ServerWater.Damage;
                SW.DamageType = W.ServerWater.DamageType;
                for (int i = 0; i < 4; ++i)
                {
                    WNew.TextureID[i] = (int)W.TextureID[i];
                    WNew.TexHandle[i] = (uint)W.TexHandle[i];
                    if ((uint)WNew.TexHandle[i] != 0) WNew.EN.Texture((uint)WNew.TexHandle[i], i);
                    Render.ScaleTexture((uint)WNew.TexHandle[i], 1f / WNew.TexScale, 1f / WNew.TexScale);
                }
                WNew.EN.Position(W.EN.X(), W.EN.Y(), W.EN.Z());
                WNew.EN.Scale(W.EN.ScaleX(), W.EN.ScaleY(), W.EN.ScaleZ());
                WNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                Collision.EntityType(WNew.EN, (byte) CollisionType.Box);
                Collision.SetCollisionMesh(WNew.EN);
                Collision.EntityPickMode(WNew.EN, PickMode.Polygon);
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, WNew);
                }
                // Add to tree view
                Name = Media.GetTextureName((int)WNew.TextureID[0]);
                TreeNode TN =
                    new TreeNode(System.IO.Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
                TN.Tag = WNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[3].Nodes.Add(TN);
                return WNew;
            }
                // Collision box
            else if (Z is ColBox)
            {
                // Copy object details
                ColBox CB = (ColBox) Z;
                ColBox CBNew = new ColBox(Program.GE.CurrentClientZone, true);
                CBNew.EN.Position(CB.EN.X(), CB.EN.Y(), CB.EN.Z());
                CBNew.EN.Rotate(CB.EN.Pitch(), CB.EN.Yaw(), CB.EN.Roll());
                CBNew.EN.Scale(CB.EN.ScaleX(), CB.EN.ScaleY(), CB.EN.ScaleZ());
                Collision.EntityType(CBNew.EN, (byte) CollisionType.Box);
                Collision.SetCollisionMesh(CBNew.EN);
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, CBNew);
                }
                // Add to tree view
                TreeNode TN = new TreeNode("Collision box " + Program.GE.CurrentClientZone.ColBoxes.Count.ToString());
                TN.Tag = CBNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[4].Nodes.Add(TN);
                return CBNew;
            }
                // Sound zone
            else if (Z is SoundZone)
            {
                // Copy object details
                SoundZone SZ = (SoundZone) Z;
                SoundZone SZNew = new SoundZone(Program.GE.CurrentClientZone, true);
                SZNew.Is3D = SZ.Is3D;
                SZNew.MusicFilename = SZ.MusicFilename;
                SZNew.MusicID = SZ.MusicID;
                SZNew.RepeatTime = SZ.RepeatTime;
                SZNew.SoundID = SZ.SoundID;
                SZNew.Volume = SZ.Volume;
                SZNew.EN.Texture(Program.GE.OrangeTex);
                SZNew.EN.Position(SZ.EN.X(), SZ.EN.Y(), SZ.EN.Z());
                SZNew.EN.Scale(SZ.EN.ScaleX(), SZ.EN.ScaleY(), SZ.EN.ScaleZ());
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, SZNew);
                }
                // Add to tree view
                string Name = "Unknown sound";
                if (SZNew.SoundID < 65535)
                {
                    Name = Media.GetSoundName(SZNew.SoundID);
                    Name = Name.Substring(0, Name.Length - 1);
                }
                else if (SZNew.MusicID < 65535)
                {
                    Name = Media.GetMusicName(SZNew.MusicID);
                }
                TreeNode TN = new TreeNode(Name);
                TN.Tag = SZNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[5].Nodes.Add(TN);
                return SZNew;
            }
                // Dynamic light
            else if (Z is RealmCrafter.ClientZone.Light)
            {
                Light L = (Light) Z;
                Light LNew = new Light(Program.GE.CurrentClientZone, true);
                LNew.EN.Position(L.EN.X(), L.EN.Y(), L.EN.Z());
                LNew.Red = L.Red;
                LNew.Green = L.Green;
                LNew.Blue = L.Blue;
                LNew.Radius = L.Radius;

                LNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                TreeNode TN = new TreeNode("Light");
                TN.Tag = LNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[6].Nodes.Add(TN);
                return LNew;
            }
            // Trigger
            else if (Z is Trigger)
            {
                Trigger T = (Trigger)Z;
                //                 // Find a free one
                //                 for (int i = 0; i < 150; ++i)
                //                 {
                //                     if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.TriggerScript[i]))
                //                     {

                // Set up new trigger
                Trigger TNew = new Trigger(Program.GE.CurrentServerZone, Program.GE, Program.GE.CurrentServerZone.Triggers.Count);
                TNew.Script = T.Script;
                TNew.Size = T.Size;
                TNew.IsSquare = T.IsSquare;
                TNew.Width = T.Width;
                TNew.Height = T.Height;
                TNew.Depth = T.Depth;
                TNew.X = T.X;
                TNew.Y = T.Y;
                TNew.Z = T.Z;

//                 TNew.EN.Position(T.EN.X(), T.EN.Y(), T.EN.Z());
//                 TNew.EN.Scale(T.EN.ScaleX(), T.EN.ScaleY(), T.EN.ScaleZ());
//                 TNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                TNew.ReBuild();

                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, TNew);
                }
                // Add to tree view and select
                TreeNode TN = new TreeNode(T.Script);
                TN.Tag = TNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[7].Nodes.Add(TN);
                return TNew;

                //                     }
                //                 }
            }
            // Portal
            else if (Z is Portal)
            {
                Portal P = (Portal)Z;
                //                 // Find a free one
                //                 for (int i = 0; i < 100; ++i)
                //                 {
                //                     if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.PortalName[i]))
                //                     {

                // Set up new portal
                Portal PNew = new Portal(Program.GE.CurrentServerZone, Program.GE, Program.GE.CurrentServerZone.Portals.Count);
                PNew.Name = P.Name + " (copy)";
                PNew.LinkArea = P.LinkArea;
                PNew.LinkName = P.LinkName;
                PNew.Size = P.Size;
                PNew.Yaw = P.Yaw;
                PNew.IsSquare = P.IsSquare;
                PNew.Width = P.Width;
                PNew.Height = P.Height;
                PNew.Depth = P.Depth;
                PNew.X = P.X;
                PNew.Y = P.Y;
                PNew.Z = P.Z;

                PNew.ReBuild();
//                 PNew.EN.Position(P.EN.X(), P.EN.Y(), P.EN.Z());
//                 PNew.EN.Rotate(0f, P.EN.Yaw(), 0f);
//                 PNew.EN.Scale(P.EN.ScaleX(), P.EN.ScaleY(), P.EN.ScaleZ());
//                PNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, PNew);
                }
                // Add to tree view
                TreeNode TN = new TreeNode(PNew.Name);
                TN.Tag = PNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[9].Nodes.Add(TN);
                return PNew;

                //                     }
                //                 }
            }
            // Waypoint
            else if (Z is Waypoint)
            {
                Waypoint WP = (Waypoint)Z;
                //                 // Find a free one
                //                 for (int i = 0; i < 2000; ++i)
                //                 {
                //                     if (Program.GE.CurrentServerZone.PrevWaypoint[i] == 2005)
                //                     {

                // Set up new waypoint
                Waypoint WPNew = new Waypoint(Program.GE.CurrentServerZone, Program.GE, Program.GE.CurrentServerZone.Waypoints.Count);

                WPNew.Prev = null;
                WPNew.NextA = WP;
                WPNew.NextB = WP;

                WPNew.EN.Position(WP.EN.X(), WP.EN.Y(), WP.EN.Z());
                WPNew.EN.Scale(3f, 3f, 3f);
                WPNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, WPNew);
                }
                // Add to tree view and select
                WPNew.WaypointEN = WPNew.EN;
                TreeNode TN = new TreeNode("Waypoint " + Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Count.ToString());
                TN.Tag = WPNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Add(TN);
                return WPNew;

                //                     }
                //                 }
            }
            return null;
        }
    }
}
