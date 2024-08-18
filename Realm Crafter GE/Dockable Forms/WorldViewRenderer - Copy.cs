// Realmcrafter World rendering form for use in WinForms dockable interface
// August 2008
// Author:Shane

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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
        #endregion

        #region Zones stuff
        private float CreateTimeLimit;
        private bool CtrlZWasDown;
        private bool MouseDragging;
        public int WaypointLinkMode = 0;
        public Waypoint LinkingWaypoint = null;
        public List<ZoneObject> Portals = new List<ZoneObject>();

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
        }

        #region Initialise
        // ReSharper disable RedundantDelegateCreation
        // Create and initialise all components
        private void InitializeComponent()
        {
            SuspendLayout();
            //
            // WorldViewRenderer
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new Size(552, 351);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HideOnClose = true;
            this.MinimizeBox = false;
            this.Name = "WorldViewRenderer";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.TabText = "World Renderer";
            this.Text = "World Renderer";
            this.Shown += new System.EventHandler(WorldRender_Show);
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

        /*      protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0010)
            {
                Program.GE.UpdateRenderingPanel(Program.GE.RenderingPanelPreviousIndex);
                
                /* Program.GE.RenderingPanel.MouseClick -= new System.Windows.Forms.MouseEventHandler(RenderingPanel_MouseClick);
                 * Program.GE.RenderingPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
                 * System.Windows.Forms.Application.Idle += new EventHandler(Program.GE.MainLoop);
                 * System.Windows.Forms.Application.Idle -= new EventHandler(WorldRender_MainLoop);
                 */
        /*
            }

            base.WndProc(ref m);
        }
        */

        private static void WorldRender_Show(object sender, EventArgs e)
        {
            // Move the render panel
            Program.GE.UpdateRenderingPanel(-3);

            DeltaBuffer = new int[DeltaFrames];
            for (int i = 0; i < DeltaFrames; ++i)
            {
                DeltaBuffer[i] = 35;
            }

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

                    #region Ctrl+S to save current tab
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

                    if (DockPanel.ActivePane != null)
                    {
                        if (DockPanel.ActivePane.CaptionText == Text)
                        {
                            #region ZZ
                            // Program.GE.Program.GE.Camera = Program.GE.Camera;
                            int CamX = (int) Program.GE.Camera.X();
                            int CamY = (int) Program.GE.Camera.Y();
                            int CamZ = (int) Program.GE.Camera.Z();
                            string CamPos = "Camera: " + CamX + ", " + CamY + ", " +
                                            CamZ;
                            Program.GE.WorldCameraPositionA.Text = CamPos;

                            if (KeyState.Get(Keys.Escape))
                            {
                                while (Program.GE.ZoneSelected.Count > 0)
                                {
                                    Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(
                                        (ZoneObject) Program.GE.ZoneSelected[0], false);
                                }
                                WaypointLinkMode = 0;

                                Program.GE.m_propertyWindow.RefreshObjectWindow();
                            }

                            #region Mouselook
                            if (KeyState.Get(Keys.Space))
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

                                    // Move Program.GE.Camera
                                    if (KeyState.Get(Keys.LButton))
                                    {
                                        Program.GE.Camera.Move(0f, 0f, 25f * Delta * Program.GE.CameraSpeed);
                                    }
                                    else if (KeyState.Get(Keys.RButton))
                                    {
                                        Program.GE.Camera.Move(0f, 0f, -25f * Delta * Program.GE.CameraSpeed);
                                    }

                                    WorldCamX = Program.GE.Camera.X();
                                    WorldCamY = Program.GE.Camera.Y();
                                    WorldCamZ = Program.GE.Camera.Z();
                                    Program.GE.RepositionGrid();

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

                            if (Program.GE.CurrentClientZone != null)
                            {
                                Program.GE.CurrentClientZone.Stars.Position(Program.GE.Camera.X(), Program.GE.Camera.Y(),
                                                                            Program.GE.Camera.Z());
                                Program.GE.CurrentClientZone.Cloud.Position(Program.GE.Camera.X(), Program.GE.Camera.Y(),
                                                                            Program.GE.Camera.Z());
                            }
                            #endregion

                            if (Program.GE.ZoneSelected.Count > 0 && !Mouselooking && !KeyState.Get(Keys.ControlKey))
                            {
                                bool MoveReorientate = false;

                                // Scale mouse position to back-buffer co-ordinates
                                Point MousePos = Program.GE.RenderingPanel.PointToClient(Cursor.Position);
                                if (Program.GE.RenderingPanel.IsAccessible)
                                {
                                    int MouseX = (MousePos.X * 1024) / Program.GE.RenderingPanel.Size.Width;
                                    int MouseY = (MousePos.Y * 768) / Program.GE.RenderingPanel.Size.Height;
                                }

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
                                    case (int) GE.WorldButtonSelection.MOVE:
                                        // Keys
                                        if (KeyState.Get(Keys.Up))
                                        {
                                            MoveSelection(0f, 0f, Delta * Speed);
                                        }

                                        if (KeyState.Get(Keys.Down))
                                        {
                                            MoveSelection(0f, 0f, Delta * -Speed);
                                        }

                                        if (KeyState.Get(Keys.Right))
                                        {
                                            MoveSelection(Delta * Speed, 0f, 0f);
                                        }

                                        if (KeyState.Get(Keys.Left))
                                        {
                                            MoveSelection(Delta * -Speed, 0f, 0f);
                                        }

                                        if (KeyState.Get(Keys.A))
                                        {
                                            MoveSelection(0f, Delta * Speed, 0f);
                                        }

                                        if (KeyState.Get(Keys.Z))
                                        {
                                            MoveSelection(0f, Delta * -Speed, 0f);
                                        }
                                        // Mouse
                                        /* if (KeyState.Get(Keys.RButton))
                                     {
                                         SetSelectionPickMode(0);
                                         Entity E = Collision.Program.GE.CameraPick(Program.GE.Camera, MouseX, MouseY);
                                         if (E != null)
                                             PositionSelection(Collision.PickedX(), Collision.PickedY(), Collision.PickedZ());
                                         SetSelectionPickMode(2);
                                     }*/
                                        if (KeyState.Get(Keys.RButton))
                                        {
                                            if (!MouseDragging)
                                            {
                                                OldMouseX = Cursor.Position.X;
                                                OldMouseY = Cursor.Position.Y;
                                                MouseDragging = true;
                                            }
                                            else
                                            {
                                                int MouseX = (MousePos.X * 1024) / Program.GE.RenderingPanel.Size.Width;
                                                int MouseY = (MousePos.Y * 768) / Program.GE.RenderingPanel.Size.Height;
                                                int MouseMoveX = Cursor.Position.X - OldMouseX;
                                                int MouseMoveY = Cursor.Position.Y - OldMouseY;
                                                ZoneObject Object = (ZoneObject) Program.GE.ZoneSelected[0];

                                                Collision.EntityPickMode(Object.EN, PickMode.Unpickable);
                                                Entity E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
                                                Collision.EntityPickMode(Object.EN, PickMode.Polygon);

                                                if (E != null)// && (Object is Terrain == false))
                                                {
                                                    float PosX = Collision.PickedX();
                                                    float PosY = Collision.PickedY();
                                                    float PosZ = Collision.PickedZ();
                                                    if (Object is Portal || Object is Waypoint || Object is Trigger)
                                                    {
                                                        PosY = PosY + 5;
                                                    }

                                                    PositionSelection(PosX, PosY, PosZ);
                                                    if (MoveReorientate && (Object is Scenery || Object is ColBox))
                                                    {
                                                        float NormalX = 0f, NormalY = 1f, NormalZ = 0f;
                                                        NormalX = Collision.PickedNX();
                                                        NormalY = Collision.PickedNY();
                                                        NormalZ = Collision.PickedNZ();
                                                        Object.EN.AlignToVector(NormalX, NormalY, NormalZ, 2);
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
                                            }
                                        }

                                            /*else if (KeyState.Get(Keys.RButton))
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
                                                TurnSelection(0f, Delta * (float)MouseMoveX, 0f);
                                        }
                                    }*/
                                        else
                                        {
                                            MouseDragging = false;
                                        }

                                        break;
                                        // Rotation tab
                                    case (int) GE.WorldButtonSelection.ROTATE:
                                        // Keys
                                        if (KeyState.Get(Keys.Up))
                                        {
                                            TurnSelection(Delta * -Speed, 0f, 0f);
                                        }

                                        if (KeyState.Get(Keys.Down))
                                        {
                                            TurnSelection(Delta * Speed, 0f, 0f);
                                        }

                                        if (KeyState.Get(Keys.Right))
                                        {
                                            TurnSelection(0f, Delta * Speed, 0f);
                                        }

                                        if (KeyState.Get(Keys.Left))
                                        {
                                            TurnSelection(0f, Delta * -Speed, 0f);
                                        }

                                        if (KeyState.Get(Keys.A))
                                        {
                                            TurnSelection(0f, 0f, Delta * Speed);
                                        }

                                        if (KeyState.Get(Keys.Z))
                                        {
                                            TurnSelection(0f, 0f, Delta * -Speed);
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
                                    case (int) GE.WorldButtonSelection.SCALE:
                                        // Keys                                    
                                        if (KeyState.Get(Keys.Up))
                                        {
                                            ScaleSelection(0f, 0f, Delta * Speed * 0.01f);
                                        }

                                        if (KeyState.Get(Keys.Down))
                                        {
                                            ScaleSelection(0f, 0f, Delta * Speed * -0.01f);
                                        }

                                        if (KeyState.Get(Keys.Right))
                                        {
                                            ScaleSelection(Delta * Speed * 0.01f, 0f, 0f);
                                        }

                                        if (KeyState.Get(Keys.Left))
                                        {
                                            ScaleSelection(Delta * Speed * -0.01f, 0f, 0f);
                                        }

                                        if (KeyState.Get(Keys.A))
                                        {
                                            ScaleSelection(0f, Delta * Speed * 0.01f, 0f);
                                        }

                                        if (KeyState.Get(Keys.Z))
                                        {
                                            ScaleSelection(0f, Delta * Speed * -0.01f, 0f);
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

                            if (KeyState.Get(Keys.PageUp))
                            {
                                Program.GE.GridHeight += Delta;
                                Program.GE.RepositionGrid();
                            }
                            else if (KeyState.Get(Keys.PageDown))
                            {
                                Program.GE.GridHeight -= Delta;
                                Program.GE.RepositionGrid();
                            }
                            else if (KeyState.Get(Keys.F12))
                            {
                                Program.GE.GridHeight = 0f;
                                Program.GE.RepositionGrid();
                            }

                            #region Delete
                            if (KeyState.Get(Keys.Delete))
                            {
                                if (Program.GE.ZoneSelected.Count > 0 && !Mouselooking)
                                {
                                    // Warn if deleting more than 10 objects at once
                                    DialogResult Result = DialogResult.OK;
                                    if (Program.GE.ZoneSelected.Count > 10)
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
                                            Obj = (ZoneObject) Program.GE.ZoneSelected[i];
                                            // Add to list of deleted objects in undo info
                                            UndoList.AddLast(Obj);
                                            // Remove from selection list
                                            Program.GE.ClearSelectionBox(Obj.EN);
                                            Program.GE.m_ZoneList.ZoneObjectListRemove(Obj);
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
                                            else if (Obj is Emitter)
                                            {
                                                General.FreeEmitter(Obj.EN.GetChild(1), true, false);
                                            }
                                            else if (Obj is Water)
                                            {
                                                Water W = (Water) Obj;
                                                WaterArea.WaterList.Remove(W.ServerWater);
                                            }
                                            else if (Obj is Trigger)
                                            {
                                                Trigger T = Obj as Trigger;
                                                Program.GE.CurrentServerZone.TriggerScript[T.ServerID] = "";
                                                Program.GE.CurrentServerZone.TriggerMethod[T.ServerID] = "";
                                            }
                                            else if (Obj is Waypoint)
                                            {
                                                Waypoint W = Obj as Waypoint;
                                                // Find waypoints connected to this one, and remove the connections
                                                for (int j = 0; j < 2000; ++j)
                                                {
                                                    if (Program.GE.CurrentServerZone.NextWaypointA[j] == W.ServerID)
                                                    {
                                                        Program.GE.WaypointLinkAEN[j].Free();
                                                        Program.GE.WaypointLinkAEN[j] = null;
                                                        Program.GE.CurrentServerZone.NextWaypointA[j] = 2000;
                                                    }

                                                    if (Program.GE.CurrentServerZone.NextWaypointB[j] == W.ServerID)
                                                    {
                                                        Program.GE.WaypointLinkBEN[j].Free();
                                                        Program.GE.WaypointLinkBEN[j] = null;
                                                        Program.GE.CurrentServerZone.NextWaypointB[j] = 2000;
                                                    }
                                                }
                                                // Remove spawn point if there is one
                                                int SpawnNum = Program.GE.CurrentServerZone.GetSpawnPoint(W.ServerID);
                                                if (SpawnNum > -1)
                                                {
                                                    Program.GE.CurrentServerZone.SpawnMax[SpawnNum] = 0;
                                                    Program.GE.CurrentServerZone.SpawnFrequency[SpawnNum] = 10;
                                                    Program.GE.CurrentServerZone.SpawnSize[SpawnNum] = 0f;
                                                    Program.GE.CurrentServerZone.SpawnRange[SpawnNum] = 0f;
                                                    Program.GE.CurrentServerZone.SpawnScript[SpawnNum] = "";
                                                    Program.GE.CurrentServerZone.SpawnActorScript[SpawnNum] = "";
                                                    Program.GE.CurrentServerZone.SpawnDeathScript[SpawnNum] = "";
                                                }

                                                // Remove this waypoint
                                                Program.GE.CurrentServerZone.PrevWaypoint[W.ServerID] = 2005;
                                                Program.GE.CurrentServerZone.NextWaypointA[W.ServerID] = 2005;
                                                Program.GE.CurrentServerZone.NextWaypointB[W.ServerID] = 2005;
                                                if (Program.GE.WaypointLinkAEN[W.ServerID] != null)
                                                {
                                                    Program.GE.WaypointLinkAEN[W.ServerID].Free();
                                                }

                                                if (Program.GE.WaypointLinkBEN[W.ServerID] != null)
                                                {
                                                    Program.GE.WaypointLinkBEN[W.ServerID].Free();
                                                }

                                                Program.GE.WaypointLinkAEN[W.ServerID] = null;
                                                Program.GE.WaypointLinkBEN[W.ServerID] = null;
                                            }
                                            else if (Obj is Portal)
                                            {
                                                Portal P = Obj as Portal;
                                                Program.GE.CurrentServerZone.PortalName[P.ServerID] = "";
                                                Program.GE.CurrentServerZone.PortalLinkArea[P.ServerID] = "";
                                                Program.GE.CurrentServerZone.PortalLinkName[P.ServerID] = "";
                                            }
                                            // Remove from zone
                                            Program.GE.RemoveObject(Obj);
                                            // Hide entity
                                            Obj.EN.Visible = false;
                                        }

                                        Program.GE.ZoneSelected.Clear();
                                        Program.GE.WorldSelectedObjectsLabel.Text = "Selected objects: 0";
                                        Program.GE.SetWorldSavedStatus(false);
                                    }

                                    Program.GE.m_propertyWindow.RefreshObjectWindow();
                                }
                            }
                            #endregion

                            #endregion
                        }
                    }

                    General.Update(Delta);
                    Render.RenderWorld();
                    Collision.UpdateWorld();
                }
            }
        }

        public void RenderingPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (Mouselooking)
            {
                return;
            }

            Point MousePos = Program.GE.RenderingPanel.PointToClient(Cursor.Position);
            int MouseX = (MousePos.X * 1024) / Program.GE.RenderingPanel.Size.Width;
            int MouseY = (MousePos.Y * 768) / Program.GE.RenderingPanel.Size.Height;

            // Left click (select an object)

            #region Left Click
            if (e.Button == MouseButtons.Left)
            {
                Entity E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
                if (E != null)
                {
                    #region Waypoint Linking
                    // Waypoint linking
                    if (WaypointLinkMode > 0)
                    {
                        ZoneObject Z = E.ExtraData as ZoneObject;
                        Waypoint WP = Z as Waypoint;
                        if (WP != null)
                        {
                            if (WP.ServerID != LinkingWaypoint.ServerID)
                            {
                                if (WaypointLinkMode == 1)
                                {
                                    // Remove current link if any
                                    if (Program.GE.CurrentServerZone.NextWaypointA[LinkingWaypoint.ServerID] < 2000)
                                    {
                                        Program.GE.CurrentServerZone.PrevWaypoint[
                                            Program.GE.CurrentServerZone.NextWaypointA[LinkingWaypoint.ServerID]] = 2000;
                                    }
                                    // Set new link
                                    Program.GE.CurrentServerZone.NextWaypointA[LinkingWaypoint.ServerID] =
                                        (ushort) WP.ServerID;
                                    Program.GE.CurrentServerZone.PrevWaypoint[WP.ServerID] =
                                        (ushort) LinkingWaypoint.ServerID;
                                    if (Program.GE.WaypointLinkAEN[LinkingWaypoint.ServerID] == null)
                                    {
                                        Program.GE.WaypointLinkAEN[LinkingWaypoint.ServerID] =
                                            Program.GE.WaypointLinkTemplate.Copy();
                                        Program.GE.WaypointLinkAEN[LinkingWaypoint.ServerID].Shader =
                                            Shaders.FullbrightAlpha;
                                        Program.GE.WaypointLinkAEN[LinkingWaypoint.ServerID].Texture(
                                            Program.GE.OrangeTex);
                                    }
                                }
                                else
                                {
                                    // Set new link
                                    Program.GE.CurrentServerZone.NextWaypointB[LinkingWaypoint.ServerID] =
                                        (ushort) WP.ServerID;
                                    if (Program.GE.WaypointLinkBEN[LinkingWaypoint.ServerID] == null)
                                    {
                                        Program.GE.WaypointLinkBEN[LinkingWaypoint.ServerID] =
                                            Program.GE.WaypointLinkTemplate.Copy();
                                        Program.GE.WaypointLinkBEN[LinkingWaypoint.ServerID].Shader =
                                            Shaders.FullbrightAlpha;
                                        Program.GE.WaypointLinkBEN[LinkingWaypoint.ServerID].Texture(Program.GE.BlueTex);
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
                Collision.EntityPickMode(Obj.EN, Mode);
            }
        }

        public void PositionSelection(float X, float Y, float Z)
        {
            ZoneObject Obj = (ZoneObject) Program.GE.ZoneSelected[0];
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
                Obj.EN.Translate(X, Y, Z);
                Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
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
            X -= Obj.EN.Pitch();
            Y -= Obj.EN.Yaw();
            Z -= Obj.EN.Roll();
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

                Obj.EN.Turn(X, Y, Z, false);
                // Special cases
                if (Obj is Trigger || Obj is Waypoint || Obj is SoundZone || Obj is Water ||
                    Obj is Light)
                {
                    Obj.EN.Rotate(0f, 0f, 0f);
                }

                Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
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
                if (Program.GE.CurrentServerZone.GetSpawnPoint(((Waypoint) Obj).ServerID) < 0)
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
                Undo.CreateNonRepeatingUndo(Undo.Actions.Scale, "A", Obj.EN.ScaleX() * 20f, Obj.EN.ScaleY() * 20f,
                                            Obj.EN.ScaleZ() * 20f);
            }

            // Perform scale
            Obj.EN.Scale(X * 0.05f, Y * 0.05f, Z * 0.05f, false);
            // Special cases
            Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
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
                    continue;

                // Special cases
                if (Obj is SoundZone || Obj is Portal || Obj is Trigger || Obj is Light)
                {
                    Y = X;
                    Z = X;
                }
                else if (Obj is Waypoint)
                {
                    if (Program.GE.CurrentServerZone.GetSpawnPoint(((Waypoint) Obj).ServerID) < 0)
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

                Obj.EN.ScaleRelative(X, Y, Z, false);
                // Special cases
                Obj.UpdateServerVersion(Program.GE.CurrentServerZone);
            }

            Program.GE.SetWorldSavedStatus(false);
        }

        private static float Lerp(float Start, float Dest, float Proportion)
        {
            return Start + ((Dest - Start) / Proportion);
        }

        private void WorldSelectObject(ZoneObject Obj, int Mode)
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
                    while (Program.GE.ZoneSelected.Count > 0)
                    {
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
            Entity E = Collision.CameraPick(Program.GE.Camera, MouseX, MouseY);
            if (E != null)
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
                    string[] ZoneFiles = Directory.GetFiles(@"Data\Areas\");
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
                    Program.GE.CurrentServerZone.Save();

                    // Add to UI
                    Program.GE.TotalZones++;
                    Program.GE.ActorStartZoneCombo.Items.Add(ZoneName);
                    Program.GE.WorldPlacePortalLinkCombo.Items.Add(ZoneName);
                    Program.GE.WorldObjectPortalLinkCombo.Items.Add(ZoneName);
                    Program.GE.WorldZoneWeatherLinkCombo.Items.Add(ZoneName);
                    Program.GE.ProjectZones.Text = "Zones: " + Program.GE.TotalZones;

                    // Reload tree
                    Program.GE.m_ZoneList.WorldZonesTree.Nodes.Clear();
                    Program.GE.TotalZones = 0;
                    string[] Zones = Directory.GetFiles(@"Data\Areas\");
                    foreach (string S_ in Zones)
                    {
                        Program.GE.TotalZones++;
                        string Name = Path.GetFileNameWithoutExtension(S_);

                        Program.GE.ActorStartZoneCombo.Items.Add(Name);
                        Program.GE.WorldPlacePortalLinkCombo.Items.Add(Name);
                        Program.GE.WorldObjectPortalLinkCombo.Items.Add(Name);
                        Program.GE.WorldZoneWeatherLinkCombo.Items.Add(Name);
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

                    // end reload
                    Program.GE.m_ZoneList.UpdateZoneList();

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
                    if (Program.GE.PlaceSceneryID == 65535)
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
                    Program.GE.MediaAddDialog.InitialDirectory = "";
                    Program.GE.MediaAddDialog.Filter = @"Bitmap (.bmp)|*.BMP";
                    Program.GE.MediaAddDialog.FilterIndex = 1;
                    Program.GE.MediaAddDialog.Multiselect = false;
                    string OldTitle = Program.GE.MediaAddDialog.Title;
                    Program.GE.MediaAddDialog.Title = "Select a heightmap";
                    Result = Program.GE.MediaAddDialog.ShowDialog();
                    if (Result == DialogResult.OK)
                    {
                        // Load bitmap and get terrain detail
                        string HM = Program.GE.MediaAddDialog.FileName;
                        Bitmap B = new Bitmap(HM);
                        B.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        if (B.Width > 1024)
                        {
                            MessageBox.Show("Heightmaps larger than 1024x1024 are not supported!", "Error");
                            return;
                        }
                        else if (B.Width != 32 && B.Width != 64 && B.Width != 128 && B.Width != 256 && B.Width != 512 &&
                                 B.Width != 1024)
                        {
                            MessageBox.Show("Heightmaps sizes must be a power of two!", "Error");
                            return;
                        }

                        // Create terrain
                        Terrain T = new Terrain(Program.GE.CurrentClientZone);

                        // Get node heights
                        T.GridSize = B.Width;
                        T.Heights = new float[T.GridSize + 1,T.GridSize + 1];
                        for (int TerrainX = 0; TerrainX < T.GridSize; ++TerrainX)
                        {
                            for (int TerrainZ = 0; TerrainZ < T.GridSize; ++TerrainZ)
                            {
                                T.Heights[TerrainX, TerrainZ] = B.GetPixel(TerrainX, TerrainZ).R / 255f;
                            }
                        }

                        // Parent mesh
                        T.EN = Entity.CreateMesh();
                        uint Surf = T.EN.CreateSurface();
                        Entity.AddVertex(Surf, 0f, 0f, 0f, 0f, 0f);
                        Entity.AddVertex(Surf, T.GridSize, 0f, 0f, 1f, 0f);
                        Entity.AddVertex(Surf, T.GridSize, 0f, T.GridSize, 1f, 1f);
                        Entity.AddVertex(Surf, 0f, 0f, T.GridSize, 0f, 1f);
                        Entity.AddVertex(Surf, 0f, 1f, 0f);

                        // Child meshes
                        int Chunks = T.GridSize / Terrain.ChunkDetail;
                        for (int ChunkX = 0; ChunkX < Chunks; ++ChunkX)
                        {
                            for (int ChunkZ = 0; ChunkZ < Chunks; ++ChunkZ)
                            {
                                Entity EN = Entity.CreateMesh();
                                Surf = EN.CreateSurface();
                                EN.Shader = Shaders.Terrain;
                                int V1, V3;
                                float VX, VZ, TrueVX, TrueVZ;
                                float GridSizeF = T.GridSize + 1f;

                                // Vertices
                                for (int TerrainX = 0; TerrainX <= Terrain.ChunkDetail; ++TerrainX)
                                {
                                    for (int TerrainZ = 0; TerrainZ <= Terrain.ChunkDetail; ++TerrainZ)
                                    {
                                        VX = TerrainX;
                                        VZ = TerrainZ;
                                        TrueVX = ((ChunkX * Terrain.ChunkDetail) + TerrainX);
                                        TrueVZ = ((ChunkZ * Terrain.ChunkDetail) + TerrainZ);
                                        Entity.AddVertex(
                                            Surf,
                                            VX,
                                            T.Heights[
                                                (ChunkX * Terrain.ChunkDetail) + TerrainX,
                                                (ChunkZ * Terrain.ChunkDetail) + TerrainZ],
                                            VZ,
                                            TrueVX / GridSizeF,
                                            1f - (TrueVZ / GridSizeF));
                                    }
                                }

                                // Triangles
                                for (int TerrainX = 0; TerrainX < Terrain.ChunkDetail; ++TerrainX)
                                {
                                    for (int TerrainZ = 0; TerrainZ < Terrain.ChunkDetail; ++TerrainZ)
                                    {
                                        V1 = (TerrainX * (Terrain.ChunkDetail + 1)) + TerrainZ;
                                        V3 = ((TerrainX + 1) * (Terrain.ChunkDetail + 1)) + TerrainZ;
                                        Entity.AddTriangle(Surf, V1 + 1, V3, V1);
                                        Entity.AddTriangle(Surf, V1 + 1, V3 + 1, V3);
                                    }
                                }

                                EN.UpdateNormals();
                                EN.UpdateHardwareBuffers();
                                Collision.EntityType(EN, (byte) CollisionType.Triangle);
                                Collision.SetCollisionMesh(EN);
                                Collision.EntityPickMode(EN, PickMode.Polygon);
                                EN.Parent(T.EN, true);
                                EN.Position(ChunkX * Terrain.ChunkDetail, 0f, ChunkZ * Terrain.ChunkDetail);
                                EN.ExtraData = T;
                            }
                        }

                        // Transform
                        T.EN.Position(PosX, PosY, PosZ);
                        T.EN.Scale(1f, 10f, 1f);

                        // Texturing
                        T.DetailTexScale = 2f;
                        T.BaseTexID = 65535;
                        T.DetailTexID = 65535;

                        // Set detail etc.
                        T.Detail = 2000;
                        T.Morph = true;
                        T.Shading = false;

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

                    Program.GE.MediaAddDialog.Multiselect = true;
                    Program.GE.MediaAddDialog.Title = OldTitle;
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
                    W.TextureID = Program.GE.PlaceWaterID;
                    W.TexHandle = Media.GetTexture(Program.GE.PlaceWaterID, false);
                    W.NormalTexHandle = Render.LoadTexture(@"Data\Textures\Water\Shader Water.png");
                    if (W.TexHandle != 0)
                    {
                        Render.ScaleTexture(W.TexHandle, 1f / W.TexScale, 1f / W.TexScale);
                        W.EN.Texture(W.TexHandle, 1);
                    }

                    if (W.NormalTexHandle != 0)
                    {
                        Render.ScaleTexture(W.NormalTexHandle, 1f / W.TexScale, 1f / W.TexScale);
                        W.EN.Texture(W.NormalTexHandle);
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

                    if (Program.GE.WorldPlaceGridCheck.Checked)
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

                    #region Trigger
                case 8:
                    if (Program.GE.m_CreateWindow.WorldPlaceTriggerScriptCombo.SelectedItem != null &&
                        Program.GE.m_CreateWindow.WorldPlaceTriggerFunctionCombo.SelectedItem != null)
                    {
                        // Find a free one
                        Found = false;
                        for (int i = 0; i < 150; ++i)
                        {
                            if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.TriggerScript[i]))
                            {
                                // Set up new trigger
                                Program.GE.CurrentServerZone.TriggerScript[i] =
                                    (string) Program.GE.m_CreateWindow.WorldPlaceTriggerScriptCombo.SelectedItem;
                                if (Program.GE.m_CreateWindow.WorldPlaceTriggerFunctionCombo.SelectedItem != null)
                                {
                                    ListBoxItem LBI =
                                        (ListBoxItem)
                                        Program.GE.m_CreateWindow.WorldPlaceTriggerFunctionCombo.SelectedItem;
                                    Program.GE.CurrentServerZone.TriggerMethod[i] = LBI.Name;
                                }
                                else
                                {
                                    Program.GE.CurrentServerZone.TriggerMethod[i] = "";
                                }

                                Program.GE.CurrentServerZone.TriggerSize[i] = 5f;
                                Program.GE.CurrentServerZone.TriggerX[i] = PosX;
                                Program.GE.CurrentServerZone.TriggerY[i] = PosY;
                                Program.GE.CurrentServerZone.TriggerZ[i] = PosZ;
                                // Create 3D entity
                                Trigger T = new Trigger(Program.GE, i);
                                T.EN.Position(PosX, PosY, PosZ);
                                // Create undo
                                if (!SuppressZoneUndo)
                                {
                                    new Undo(Undo.Actions.Create, T);
                                }

                                // Add to tree view and select
                                TN = new TreeNode(Program.GE.CurrentServerZone.TriggerScript[i]);
                                TN.Tag = T;
                                Program.GE.m_ZoneList.WorldZonesTree.Nodes[7].Nodes.Add(TN);
                                WorldSelectObject(T, 3);

                                Found = true;
                                break;
                            }
                        }
                        // No spare trigger - tell user
                        if (!Found)
                        {
                            MessageBox.Show("Maximum number of triggers already placed in this zone", "Error");
                        }
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
                        // Find a free one
                        Found = false;
                        for (int i = 0; i < 100; ++i)
                        {
                            if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.PortalName[i]))
                            {
                                // Set up new portal
                                Program.GE.CurrentServerZone.PortalName[i] =
                                    Program.GE.m_CreateWindow.WorldPlacePortalNameText.Text;
                                if (Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.SelectedItem != null)
                                {
                                    Program.GE.CurrentServerZone.PortalLinkArea[i] =
                                        (string) Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.SelectedItem;
                                }
                                else
                                {
                                    Program.GE.CurrentServerZone.PortalLinkArea[i] = "";
                                }

                                if (Program.GE.m_CreateWindow.WorldPlacePortalLinkNameCombo.SelectedItem != null)
                                {
                                    Program.GE.CurrentServerZone.PortalLinkName[i] =
                                        (string) Program.GE.m_CreateWindow.WorldPlacePortalLinkNameCombo.SelectedItem;
                                }
                                else
                                {
                                    Program.GE.CurrentServerZone.PortalLinkName[i] = "";
                                }

                                Program.GE.CurrentServerZone.PortalSize[i] = 10f;
                                Program.GE.CurrentServerZone.PortalX[i] = PosX;
                                Program.GE.CurrentServerZone.PortalY[i] = PosY;
                                Program.GE.CurrentServerZone.PortalZ[i] = PosZ;
                                // Create 3D entity
                                Portal P = new Portal(Program.GE, i);
                                P.EN.Position(PosX, PosY + 8f, PosZ);
                                P.EN.Scale(10f, 10f, 10f);
                                // Create undo
                                if (!SuppressZoneUndo)
                                {
                                    new Undo(Undo.Actions.Create, P);
                                }
                                // Add to tree view and select
                                TN = new TreeNode(Program.GE.CurrentServerZone.PortalName[i]);
                                TN.Tag = P;
                                Program.GE.m_ZoneList.WorldZonesTree.Nodes[9].Nodes.Add(TN);
                                WorldSelectObject(P, 3);

                                Found = true;
                                break;
                            }
                        }
                        // No spare portal - tell user
                        if (!Found)
                        {
                            MessageBox.Show("Maximum number of portals already placed in this zone", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a name for your portal", "Error");
                    }
                    break;
                    #endregion

                    #region Waypoint
                case 9:
                    // Find a free one
                    Found = false;
                    for (int i = 0; i < 2000; ++i)
                    {
                        if (Program.GE.CurrentServerZone.PrevWaypoint[i] == 2005)
                        {
                            // Set up new waypoint
                            Program.GE.CurrentServerZone.PrevWaypoint[i] = 2000;
                            Program.GE.CurrentServerZone.NextWaypointA[i] = 2000;
                            Program.GE.CurrentServerZone.NextWaypointB[i] = 2000;
                            Program.GE.CurrentServerZone.WaypointX[i] = PosX;
                            Program.GE.CurrentServerZone.WaypointY[i] = PosY;
                            Program.GE.CurrentServerZone.WaypointZ[i] = PosZ;
                            // Create 3D entity
                            Waypoint WP = new Waypoint(Program.GE, i);
                            WP.EN.Position(PosX, PosY + 3f, PosZ);
                            WP.EN.Scale(3f, 3f, 3f);
                            // Create undo
                            if (!SuppressZoneUndo)
                            {
                                new Undo(Undo.Actions.Create, WP);
                            }
                            // Add to tree view and select
                            Program.GE.WaypointEN[i] = WP.EN;
                            TN = new TreeNode("Waypoint " + Waypoints.Count);
                            TN.Tag = WP;
                            Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Add(TN);
                            WorldSelectObject(WP, 3);

                            Found = true;
                            break;
                        }
                    }
                    // No spare waypoint - tell user
                    if (!Found)
                    {
                        MessageBox.Show("Maximum number of waypoints already placed in this zone", "Error");
                    }

                    break;
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
            }

            Program.GE.SetWorldSavedStatus(false);
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
            else if (Z is Terrain)
            {
                // Copy object details
                Terrain T = (Terrain) Z;
                Terrain TNew = new Terrain(Program.GE.CurrentClientZone);
                TNew.DetailTexScale = T.DetailTexScale;
                TNew.BaseTexID = T.BaseTexID;
                TNew.DetailTexID = T.DetailTexID;
                TNew.Detail = T.Detail;
                TNew.Morph = T.Morph;
                TNew.Shading = T.Shading;
                TNew.GridSize = T.GridSize;
                TNew.Heights = (float[,]) T.Heights.Clone();
                // Parent mesh
                TNew.EN = Entity.CreateMesh();
                uint Surf = TNew.EN.CreateSurface();
                Entity.AddVertex(Surf, 0f, 0f, 0f, 0f, 0f);
                Entity.AddVertex(Surf, (float) TNew.GridSize, 0f, 0f, 1f, 0f);
                Entity.AddVertex(Surf, (float) TNew.GridSize, 0f, (float) TNew.GridSize, 1f, 1f);
                Entity.AddVertex(Surf, 0f, 0f, (float) TNew.GridSize, 0f, 1f);
                Entity.AddVertex(Surf, 0f, 1f, 0f);
                // Child meshes
                int Chunks = TNew.GridSize / Terrain.ChunkDetail;
                for (int ChunkX = 0; ChunkX < Chunks; ++ChunkX)
                {
                    for (int ChunkZ = 0; ChunkZ < Chunks; ++ChunkZ)
                    {
                        Entity EN = Entity.CreateMesh();
                        Surf = EN.CreateSurface();
                        EN.Shader = Shaders.Terrain;
                        int V1, V3;
                        float VX, VZ, TrueVX, TrueVZ;
                        float GridSizeF = (float) T.GridSize + 1f;
                        // Vertices
                        for (int TerrainX = 0; TerrainX <= Terrain.ChunkDetail; ++TerrainX)
                        {
                            for (int TerrainZ = 0; TerrainZ <= Terrain.ChunkDetail; ++TerrainZ)
                            {
                                VX = (float) TerrainX;
                                VZ = (float) TerrainZ;
                                TrueVX = (float) ((ChunkX * Terrain.ChunkDetail) + TerrainX);
                                TrueVZ = (float) ((ChunkZ * Terrain.ChunkDetail) + TerrainZ);
                                Entity.AddVertex(Surf,
                                                 VX,
                                                 T.Heights[
                                                     (ChunkX * Terrain.ChunkDetail) + TerrainX,
                                                     (ChunkZ * Terrain.ChunkDetail) + TerrainZ],
                                                 VZ,
                                                 TrueVX / GridSizeF,
                                                 1f - (TrueVZ / GridSizeF));
                            }
                        }
                        // Triangles
                        for (int TerrainX = 0; TerrainX < Terrain.ChunkDetail; ++TerrainX)
                        {
                            for (int TerrainZ = 0; TerrainZ < Terrain.ChunkDetail; ++TerrainZ)
                            {
                                V1 = (TerrainX * (Terrain.ChunkDetail + 1)) + TerrainZ;
                                V3 = ((TerrainX + 1) * (Terrain.ChunkDetail + 1)) + TerrainZ;
                                Entity.AddTriangle(Surf, V1 + 1, V3, V1);
                                Entity.AddTriangle(Surf, V1 + 1, V3 + 1, V3);
                            }
                        }
                        EN.UpdateNormals();
                        EN.UpdateHardwareBuffers();
                        Collision.EntityType(EN, (byte) CollisionType.Triangle);
                        Collision.SetCollisionMesh(EN);
                        Collision.EntityPickMode(EN, PickMode.Polygon);
                        EN.Parent(TNew.EN, true);
                        EN.Position(ChunkX * Terrain.ChunkDetail, 0f, ChunkZ * Terrain.ChunkDetail);
                        EN.ExtraData = TNew;
                    }
                }
                // Apply textures
                if (T.BaseTexID < 65535)
                {
                    uint Tex = Media.GetTexture(T.BaseTexID, false);
                    if (Tex != 0)
                    {
                        for (int j = 1; j <= T.EN.CountChildren(); ++j)
                        {
                            T.EN.GetChild(j).Texture(Tex);
                        }
                        Media.UnloadTexture(T.BaseTexID);
                    }
                }
                if (T.DetailTexID < 65535)
                {
                    T.DetailTex = Media.GetTexture(T.DetailTexID, false);
                    if (T.DetailTex != 0)
                    {
                        Render.ScaleTexture(T.DetailTex, T.DetailTexScale * 0.01f, T.DetailTexScale * 0.01f);
                        for (int j = 1; j <= T.EN.CountChildren(); ++j)
                        {
                            T.EN.GetChild(j).Texture(T.DetailTex, 1);
                        }
                    }
                }
                // Transform
                TNew.EN.Position(T.EN.X(), T.EN.Y(), T.EN.Z());
                TNew.EN.Rotate(T.EN.Pitch(), T.EN.Yaw(), T.EN.Roll());
                TNew.EN.Scale(T.EN.ScaleX(), T.EN.ScaleY(), T.EN.ScaleZ());
                // Create undo
                if (!SuppressZoneUndo)
                {
                    new Undo(Undo.Actions.Create, TNew);
                }
                // Add to tree view
                TreeNode TN =
                    new TreeNode("Terrain " + (Program.GE.CurrentClientZone.Terrains.IndexOf(TNew) + 1).ToString());
                TN.Tag = TNew;
                Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
                return TNew;
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
                WNew.TextureID = W.TextureID;
                WaterArea SW = new WaterArea(Program.GE.CurrentServerZone);
                WNew.ServerWater = SW;
                SW.Damage = W.ServerWater.Damage;
                SW.DamageType = W.ServerWater.DamageType;
                WNew.TexHandle = Media.GetTexture(WNew.TextureID, false);
                WNew.NormalTexHandle = Render.LoadTexture(@"Data\Textures\Water\Shader Water.png");
                if (WNew.TexHandle != 0)
                {
                    Render.ScaleTexture(WNew.TexHandle, 1f / WNew.TexScale, 1f / WNew.TexScale);
                    WNew.EN.Texture(WNew.TexHandle, 1);
                }
                if (WNew.NormalTexHandle != 0)
                {
                    Render.ScaleTexture(WNew.NormalTexHandle, 1f / WNew.TexScale, 1f / WNew.TexScale);
                    WNew.EN.Texture(WNew.NormalTexHandle);
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
                Name = Media.GetTextureName(WNew.TextureID);
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
                Program.GE.WorldZonesTree.Nodes[4].Nodes.Add(TN);
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
                Light L = (Light)Z;
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
                Trigger T = (Trigger) Z;
                // Find a free one
                for (int i = 0; i < 150; ++i)
                {
                    if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.TriggerScript[i]))
                    {
                        // Set up new trigger
                        Program.GE.CurrentServerZone.TriggerScript[i] =
                            Program.GE.CurrentServerZone.TriggerScript[T.ServerID];
                        Program.GE.CurrentServerZone.TriggerMethod[i] =
                            Program.GE.CurrentServerZone.TriggerMethod[T.ServerID];
                        // Create 3D entity
                        Trigger TNew = new Trigger(Program.GE, i);
                        TNew.EN.Position(T.EN.X(), T.EN.Y(), T.EN.Z());
                        TNew.EN.Scale(T.EN.ScaleX(), T.EN.ScaleY(), T.EN.ScaleZ());
                        TNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, TNew);
                        }
                        // Add to tree view and select
                        TreeNode TN = new TreeNode(Program.GE.CurrentServerZone.TriggerScript[i]);
                        TN.Tag = TNew;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[7].Nodes.Add(TN);
                        return TNew;
                    }
                }
            }
                // Portal
            else if (Z is Portal)
            {
                Portal P = (Portal) Z;
                // Find a free one
                for (int i = 0; i < 100; ++i)
                {
                    if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.PortalName[i]))
                    {
                        // Set up new portal
                        Program.GE.CurrentServerZone.PortalName[i] =
                            Program.GE.CurrentServerZone.PortalName[P.ServerID] + " (copy)";
                        Program.GE.CurrentServerZone.PortalLinkArea[i] =
                            Program.GE.CurrentServerZone.PortalLinkArea[P.ServerID];
                        Program.GE.CurrentServerZone.PortalLinkName[i] =
                            Program.GE.CurrentServerZone.PortalLinkName[P.ServerID];
                        // Create 3D entity
                        Portal PNew = new Portal(Program.GE, i);
                        PNew.EN.Position(P.EN.X(), P.EN.Y(), P.EN.Z());
                        PNew.EN.Rotate(0f, P.EN.Yaw(), 0f);
                        PNew.EN.Scale(P.EN.ScaleX(), P.EN.ScaleY(), P.EN.ScaleZ());
                        PNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, PNew);
                        }
                        // Add to tree view
                        TreeNode TN = new TreeNode(Program.GE.CurrentServerZone.PortalName[i]);
                        TN.Tag = PNew;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[9].Nodes.Add(TN);
                        return PNew;
                    }
                }
            }
                // Waypoint
            else if (Z is Waypoint)
            {
                Waypoint WP = (Waypoint) Z;
                // Find a free one
                for (int i = 0; i < 2000; ++i)
                {
                    if (Program.GE.CurrentServerZone.PrevWaypoint[i] == 2005)
                    {
                        // Set up new waypoint
                        Program.GE.CurrentServerZone.PrevWaypoint[i] = 2000;
                        Program.GE.CurrentServerZone.NextWaypointA[i] =
                            Program.GE.CurrentServerZone.NextWaypointA[WP.ServerID];
                        Program.GE.CurrentServerZone.NextWaypointB[i] =
                            Program.GE.CurrentServerZone.NextWaypointB[WP.ServerID];
                        // Create 3D entity
                        Waypoint WPNew = new Waypoint(Program.GE, i);
                        WPNew.EN.Position(WP.EN.X(), WP.EN.Y(), WP.EN.Z());
                        WPNew.EN.Scale(3f, 3f, 3f);
                        WPNew.UpdateServerVersion(Program.GE.CurrentServerZone);
                        // Create undo
                        if (!SuppressZoneUndo)
                        {
                            new Undo(Undo.Actions.Create, WPNew);
                        }
                        // Add to tree view and select
                        Program.GE.WaypointEN[i] = WPNew.EN;
                        TreeNode TN = new TreeNode("Waypoint " + Waypoints.Count.ToString());
                        TN.Tag = WPNew;
                        Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Add(TN);
                        return WPNew;
                    }
                }
            }
            return null;
        }
    }
}