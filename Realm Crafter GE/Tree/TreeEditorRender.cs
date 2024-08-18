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
using WeifenLuo.WinFormsUI.Docking;
using RenderingServices;
using System.Runtime.InteropServices;

namespace RealmCrafter_GE
{
    public partial class TreeEditorRender : DockContent
    {
        bool LeftDown = false;
        bool RightDown = false;
        bool MiddleDown = false;
        int MouseX = 0, MouseY = 0;

        bool Started = false;
        Entity Camera = null;
        List<Line3D> Lines = new List<Line3D>();

        float CamPitch = 45.0f;
        float CamYaw = 45.0f;
        float CamZoom = 20.0f;
        Entity CamOrigin;

        bool saved = true;
        public bool SupressSave = false;

        public bool Saved
        {
            get { return saved; }
            set
            {
                if (SupressSave)
                    return;

                if(saved == value)
                    return;
                
                saved = value;

                if (saved == false)
                {
                    this.Text = "Tree Editor *";
                    this.TabText = "Tree Editor *";
                    //Program.GE.SetWorldSavedStatus(false);
                }
                else
                {
                    this.Text = "Tree Editor";
                    this.TabText = "Tree Editor";
                }
            }
        }
        


        public TreeEditorRender()
        {
            InitializeComponent();
            this.Text = "Tree Editor";
            this.TabText = "Tree Editor";
            RenderPanel.MouseWheel += new MouseEventHandler(RenderPanel_MouseWheel);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            //Hide();
        }

        public void UpdateWorldButtonSelection()
        {
            if (Program.ActiveTree == null || Program.ActiveTree.Name.Length == 0)
                return;

            Program.GE.TreeProperties.UpdateWorldButtonSelection();
        }

        public void CreateLOD(string path, float angle)
        {
            if (Program.ActiveTree == null || Program.ActiveTree.Name.Length == 0)
                return;

            Render.Graphics3D(512, 512, 32, 2, 0, 0, "");

            bool HiddenTool = false;
            if (Program.Transformer != null && Program.Transformer.Visible)
            {
                HiddenTool = true;
                Program.Transformer.Visible = false;
            }

            HideRender();
            Program.ActiveTree.StartLODRender(angle);

            Render.RenderWorld();
            RenderWrapper.bbdx2_SaveLastFrame(path);

            Program.ActiveTree.EndLODRender();
            ShowRender();

            if (HiddenTool && Program.Transformer != null)
                Program.Transformer.Visible = true;

            Render.Graphics3D(Program.GE.RenderingPanel.Width, Program.GE.RenderingPanel.Height, 32, 2, 0, 0,
                                  @".\Data\DefaultTex.png");
        }

        private void TreeEditorRender_Load(object sender, EventArgs e)
        {
            //Render.Init(RenderPanel.Handle, 0);
            //Render.Graphics3D(1024, 768, 32, 2, 0, 0,
            //                  @".\Data\DefaultTex.png");
            Started = true;

            Camera = Program.GE.Camera;
//             Camera = Entity.CreateCamera();
//             //Camera.CameraClsColor(0, 128, 255);
//             RenderWrapper.bbdx2_CameraClsColorAlpha(0, 0, 128, 255, 0);
//             Camera.Position(0, 100, 0);
//             Camera.Rotate(90, 0, 0);
//             RenderWrapper.jackhammer(Camera.Handle, 1);

            CamOrigin = Entity.CreatePivot();

            //Shaders.Line = Shaders.Load("Data\\Game Data\\Shaders\\Default\\3DLine.fx");
            //Shaders.LitObject1 = Shaders.Load("Data\\Game Data\\Shaders\\Default\\LitObject_High.fx");


            for (float x = -32.0f; x <= 32.0f; x += 2.0f)
            {
                Line3D Line = new Line3D(x, 0, -32, x, 0.0f, 32, false);

                if (x != 0)
                    Line.SetColor(200, 200, 200);
                else
                    Line.SetColor(255, 255, 255);
                Lines.Add(Line);
            }

            for (float y = -32; y <= 32; y += 2.0f)
            {
                Line3D Line = new Line3D(-32.0f, 0, y, 32.0f, 0.0f, y, false);
                if (y != 0)
                    Line.SetColor(200, 200, 200);
                else
                    Line.SetColor(255, 255, 255);
                Lines.Add(Line);
            }

//            Application.Idle += new EventHandler(Application_Idle);
//
//             RenderWrapper.LoadUserDefinedPP_FromXML("Data\\Game Data\\PostProcess\\PostProcess.xml");
//             RenderWrapper.SetPP_Pipeline("Default2");
//
//             Render.Graphics3D(1022, 948, 32, 2, 0, 0,
//                                   @".\Data\DefaultTex.png");

//             Program.Manager = LTNet.TreeManager.CreateTreeManager(RenderWrapper.bbdx2_GetIDirect3DDevice9(),
//                 @"Data\Game Data\Shaders\Default\Trunk.fx",
//                 @"Data\Game Data\Shaders\Default\Leaf.fx",
//                 @"Data\Game Data\Shaders\Default\LOD.fx");
// 
//             RenderWrapper.SetRenderSolidCallback(0, Program.Manager.GetRenderCallback());
//             RenderWrapper.SetDeviceLostCallback(0, Program.Manager.GetLostCallback());
//             RenderWrapper.SetDeviceResetXYCallback(0, Program.Manager.GetResetCallback());
// 
//             Program.Manager.TreeRender += new EventHandler(Manager_TreeRender);
//             Program.Manager.CollisionChanged += new EventHandler(Manager_CollisionChanged);
// 
//             LTNet.TreeType TType = Program.Manager.LoadTreeType(@"Data\Trees\tree1.lt");
//             //if (TType == null)
//             //    throw new Exception("Could not load tree1.lt!");
// 
//             Program.CurrentTreeZone = Program.Manager.CreateZone();
//             //if (Program.CurrentTreeZone.AddTreeInstance(TType, new NGUINet.NVector3(50, 0, 50)) == null)
//             //    throw new Exception("Tree Instance is invalid!");




            Program.ActiveTree = new TreeEditorTree(Program.GE.TreeProperties);
            //Program.ActiveTree.Save("tree1.xml");
            //Program.ActiveTree.Load("tree1.xml");

            

            Program.ActiveTree.Visible = false;
            HideRender();

            RenderPanel.Visible = false;
            RenderPanel = Program.GE.RenderingPanel;
        }


        public void HideRender()
        {
            foreach (Line3D L in Lines)
            {
                L.Visible = false;
            }
        }

        public void ShowRender()
        {
            foreach (Line3D L in Lines)
            {
                L.Visible = true;
            }
        }

        public void Application_Idle(object sender, EventArgs e)
        {

            float YawX = Convert.ToSingle(Math.Sin(Convert.ToDouble(CamYaw) * (Math.PI / 180.0))) * -CamZoom;
            float YawZ = Convert.ToSingle(Math.Cos(Convert.ToDouble(CamYaw) * (Math.PI / 180.0))) * -CamZoom;

            float PitchZ = Convert.ToSingle(Math.Sin(Convert.ToDouble(CamPitch - 90) * (Math.PI / 180.0)));
            float PitchY = Convert.ToSingle(Math.Cos(Convert.ToDouble(CamPitch - 90) * (Math.PI / 180.0))) * CamZoom;

            float X = (YawX * PitchZ) + CamOrigin.X();
            float Y = PitchY + CamOrigin.Y();
            float Z = (YawZ * PitchZ) + CamOrigin.Z();

            Camera.Position(X, Y, Z);
            Camera.Point(CamOrigin);

            Point Mouse = new Point();
            
            if(RenderPanel != null)
                Mouse = RenderPanel.PointToClient(Cursor.Position);

            if (Program.Transformer != null)
                Program.Transformer.Update(Mouse.X, Mouse.Y);

            if(Program.Manager != null)
                Program.Manager.Update(Camera.X(), Camera.Y(), Camera.Z());
            Collision.UpdateWorld();
            Render.RenderWorld();
        }

        private void RenderPanel_SizeChanged(object sender, EventArgs e)
        {


        }

        private void Cross(float x1, float y1, float z1, float x2, float y2, float z2, ref float xo, ref float yo, ref float zo)
        {
            xo = y1 * z2 - z1 * y2;
            yo = z1 * x2 - x1 * z2;
            zo = x1 * y2 - y1 * x2;
        }

        public void RenderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (LeftDown)
            {
                int MovedX = e.X - MouseX;
                int MovedY = e.Y - MouseY;
 
                CamYaw += Convert.ToSingle(MovedX) * 0.4f;
                CamPitch -= Convert.ToSingle(MovedY) * 0.4f;

                if (CamPitch > 89.0f)
                    CamPitch = 89.0f;
                if (CamPitch < -89.0f)
                    CamPitch = -89.0f;

                MouseX = e.X;
                MouseY = e.Y;
            }

            if (MiddleDown)
            {
                int MovedX = e.X - MouseX;
                int MovedY = e.Y - MouseY;

                float Dx = CamOrigin.X() - Camera.X();
                float Dy = CamOrigin.Y() - Camera.Y();
                float Dz = CamOrigin.Z() - Camera.Z();
                float Mag = Convert.ToSingle(Math.Sqrt(Convert.ToDouble((Dx * Dx) + (Dy * Dy) + (Dz * Dz))));
                Dx /= Mag;
                Dy /= Mag;
                Dz /= Mag;

                float Upx = 0.0f, Upy = 0.0f, Upz = 0.0f;
                float Rtx = 0.0f, Rty = 0.0f, Rtz = 0.0f;
                Cross(Dx, Dy, Dz, 1, 0, 0, ref Upx, ref Upy, ref Upz);
                Cross(Dx, Dy, Dz, 0, 1, 0, ref Rtx, ref Rty, ref Rtz);

                float MoveX = Rtx * Convert.ToSingle(MovedX) * 0.05f;
                float MoveY = Rty * Convert.ToSingle(MovedX) * 0.05f;
                float MoveZ = Rtz * Convert.ToSingle(MovedX) * 0.05f;

                MoveX += Upx * Convert.ToSingle(MovedY) * 0.05f;
                MoveY += Upy * Convert.ToSingle(MovedY) * 0.05f;
                MoveZ += Upz * Convert.ToSingle(MovedY) * 0.05f;

                CamOrigin.Translate(MoveX, MoveY, MoveZ);

                MouseX = e.X;
                MouseY = e.Y;
            }

            if (RightDown)
            {
                int MovedX = e.X - MouseX;
                int MovedY = e.Y - MouseY;

                CamZoom += Convert.ToSingle(MovedY) * 0.1f;
                if (CamZoom < 1.0f)
                    CamZoom = 1.0f;
                if (CamZoom > 80.0f)
                    CamZoom = 80.0f;

                MouseX = e.X;
                MouseY = e.Y;
            }
            
            
        }

        void RenderPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            
            
        }

        public void RenderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !RightDown && !MiddleDown)
            {
                if (Program.Transformer != null)
                    if (Program.Transformer.MouseDown(Camera, e.X, e.Y))
                        return;

                Entity Picked = Collision.CameraPick(Camera, e.X, e.Y);
                if (Picked != null && Program.IsRendered(Picked))
                {
                    TreeNode Node = Program.GE.TreeProperties.FindNode(Picked);
                    if (Node != null)
                    {
                        Program.GE.TreeProperties.TreeComponents.SelectedNode = Node;
                        return;
                    }
                }

                LeftDown = true;
                MouseX = e.X;
                MouseY = e.Y;
            }

            if (e.Button == MouseButtons.Right && !LeftDown && !MiddleDown)
            {
                RightDown = true;
                MouseX = e.X;
                MouseY = e.Y;
            }

            if (e.Button == MouseButtons.Middle && !LeftDown && !RightDown)
            {
                MiddleDown = true;
                MouseX = e.X;
                MouseY = e.Y;
            }
        }

        public void RenderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LeftDown = false;
                if (Program.Transformer != null)
                    Program.Transformer.MouseUp();
            }

            if (e.Button == MouseButtons.Right)
                RightDown = false;

            if (e.Button == MouseButtons.Middle)
                MiddleDown = false;
        }

        private void RenderPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        public void TreeEditorRender_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
                Program.GE.TreeProperties.deleteToolStripMenuItem_Click(sender, EventArgs.Empty);
        }

        public void TreeEditorRender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
                Program.GE.TreeProperties.deleteToolStripMenuItem_Click(sender, EventArgs.Empty);
        }
    }
}