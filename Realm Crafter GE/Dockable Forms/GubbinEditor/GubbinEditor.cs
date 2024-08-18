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
using RealmCrafter;
using System.IO;

namespace RealmCrafter_GE.Dockable_Forms.GubbinEditor
{
    public partial class GubbinEditor : DockContent
    {
        public List<GubbinTemplate> Templates = new List<GubbinTemplate>();

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

        public bool saved = true;

        public Entity TransformPivot;
        public Entity GubbinPreviewMesh;
        public int GubbinPreviewSeq = 0;
        public RottParticles.EmitterConfig GubbinPreviewConfig;
        public Entity GubbinPreviewEmitterEN = null;
        public RealmCrafter.ClientZone.Light PreviewLight = null;


        public ActorInstance ActorPreview;
        public ActorGubbinPropertyInterface PropertyInterface;
        public List<string> BoneNames = new List<string>();

        public GubbinEditor()
        {
            InitializeComponent();
            Saved = true;

            
        }

        public bool Saved
        {
            get
            {
                return saved;
            }
            set
            {
                saved = value;
                if (saved == false)
                {
                    this.Text = "Gubbin Editor *";
                    this.TabText = "Gubbin Editor *";
                }
                else
                {
                    this.Text = "Gubbin Editor";
                    this.TabText = "Gubbin Editor";
                }
            }
        }

        public void LoadTemplates()
        {
            BinaryReader Reader;

            try
            {
                using (Reader = Blitz.ReadFile(@"Data\Game Data\GubbinTemplates.dat"))
                {
                    if (Reader == null)
                        return;

                    byte FileVersion = Reader.ReadByte();

                    if (FileVersion == 1)
                    {
                        Load1_0(Reader);
                    }
                    else
                    {
                        throw new Exception("Unknown GubbinTemplate File Version: " + FileVersion.ToString());
                    }

                    Reader.Close();
                }
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }

        private void Load1_0(BinaryReader F)
        {
            ushort TemplateCount = F.ReadUInt16();
            Templates.Clear();
            TemplatesList.Items.Clear();

            for (int i = 0; i < TemplateCount; ++i)
            {
                GubbinTemplate T = new GubbinTemplate();
                T.ID = F.ReadUInt16();
                T.Name = ASCIIEncoding.ASCII.GetString(F.ReadBytes(F.ReadByte()));

                ushort ActorsCount = F.ReadUInt16();
                for (int a = 0; a < ActorsCount; ++a)
                {
                    GubbinActorTemplate AT = new GubbinActorTemplate();

                    ushort ActorID = F.ReadUInt16();
                    AT.Actor = Actor.Index[ActorID];
                    AT.Gender = F.ReadByte();
                    AT.Emitter = ASCIIEncoding.ASCII.GetString(F.ReadBytes(F.ReadByte()));
                    AT.MeshID = F.ReadUInt16();
                    AT.UseLight = F.ReadByte() > 0;
                    AT.LightRadius = F.ReadSingle();
                    AT.LightColor = new Vector3();
                    AT.LightColor.X = F.ReadSingle();
                    AT.LightColor.Y = F.ReadSingle();
                    AT.LightColor.Z = F.ReadSingle();
                    AT.LightFunction = ASCIIEncoding.ASCII.GetString(F.ReadBytes(F.ReadByte()));

                    //AT.HasAnimation = F.ReadByte() > 0;
                    AT.AnimationType = (GubbinAnimationType)F.ReadByte();
                    AT.AnimationStartFrame = F.ReadUInt16();
                    AT.AnimationEndFrame = F.ReadUInt16();

                    AT.AssignedBoneName = ASCIIEncoding.ASCII.GetString(F.ReadBytes(F.ReadByte()));

                    AT.Position = new Vector3();
                    AT.Scale = new Vector3();
                    AT.Rotation = new Vector3();

                    AT.Position.X = F.ReadSingle();
                    AT.Position.Y = F.ReadSingle();
                    AT.Position.Z = F.ReadSingle();
                    AT.Scale.X = F.ReadSingle();
                    AT.Scale.Y = F.ReadSingle();
                    AT.Scale.Z = F.ReadSingle();
                    AT.Rotation.X = F.ReadSingle();
                    AT.Rotation.Y = F.ReadSingle();
                    AT.Rotation.Z = F.ReadSingle();

                    T.ActorTemplates.Add(AT);
                }

                Templates.Add(T);
                TemplatesList.Items.Add(T);
            }
        }

        public void Save()
        {
            BinaryWriter F = Blitz.WriteFile(@"Data\Game Data\GubbinTemplates.dat");

            F.Write((byte)1); // File version

            // Number of templates
            F.Write((ushort)Templates.Count);
            
            foreach(GubbinTemplate T in Templates)
            {
                F.Write((ushort)T.ID);
                F.Write((byte)T.Name.Length);
                F.Write(ASCIIEncoding.ASCII.GetBytes(T.Name));

                F.Write((ushort)T.ActorTemplates.Count);
                foreach (GubbinActorTemplate AT in T.ActorTemplates)
                {
                    if (AT.Actor != null)
                        F.Write((ushort)AT.Actor.ID);
                    else
                        F.Write((ushort)65535);

                    F.Write((byte)AT.Gender);
                    F.Write((byte)AT.Emitter.Length);
                    F.Write(ASCIIEncoding.ASCII.GetBytes(AT.Emitter));
                    F.Write((ushort)AT.MeshID);

                    F.Write((byte)(AT.UseLight ? 1 : 0));
                    F.Write(AT.LightRadius);
                    F.Write(AT.LightColor.X);
                    F.Write(AT.LightColor.Y);
                    F.Write(AT.LightColor.Z);
                    F.Write((byte)AT.LightFunction.Length);
                    F.Write(ASCIIEncoding.ASCII.GetBytes(AT.LightFunction));

                    F.Write((byte)AT.AnimationType);
                    F.Write((ushort)AT.AnimationStartFrame);
                    F.Write((ushort)AT.AnimationEndFrame);

                    F.Write((byte)AT.AssignedBoneName.Length);
                    F.Write(ASCIIEncoding.ASCII.GetBytes(AT.AssignedBoneName));

                    F.Write(AT.Position.X);
                    F.Write(AT.Position.Y);
                    F.Write(AT.Position.Z);
                    F.Write(AT.Scale.X);
                    F.Write(AT.Scale.Y);
                    F.Write(AT.Scale.Z);
                    F.Write(AT.Rotation.X);
                    F.Write(AT.Rotation.Y);
                    F.Write(AT.Rotation.Z);
                }
            }

            F.Close();


            Saved = true;


        }

        public void UpdateWorldButtonSelection()
        {
            if (Program.Transformer  != null)
                Program.Transformer.Free();

            if (TransformPivot == null)
                return;

            if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.MOVE)
            {
                if (ActorPreview != null && ActorPreview.EN != null)
                {
                    ((Entity)ActorPreview.EN).Animate(0, 1, 0);
                    TransformPivot.Parent(((Entity)ActorPreview.EN).FindChild(PropertyInterface.Template.AssignedBoneName), false);
                    TransformPivot.Position(PropertyInterface.Template.Position.X,
                        PropertyInterface.Template.Position.Y,
                        PropertyInterface.Template.Position.Z);

                    Program.Transformer = new GMoveTool(TransformPivot, new EventHandler(Moving), new EventHandler(Moved));
                }
            }
            else if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.ROTATE)
            {
                if (ActorPreview != null && ActorPreview.EN != null)
                {
                    ((Entity)ActorPreview.EN).Animate(0, 1, 0);
                    TransformPivot.Parent(((Entity)ActorPreview.EN).FindChild(PropertyInterface.Template.AssignedBoneName), false);
                    TransformPivot.Position(PropertyInterface.Template.Position.X,
                        PropertyInterface.Template.Position.Y,
                        PropertyInterface.Template.Position.Z);

                    Program.Transformer = new GRotateTool(TransformPivot, new EventHandler(Rotating), new EventHandler(Rotated));
                }
            }
            else if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.SCALE)
            {
                if (ActorPreview != null && ActorPreview.EN != null)
                {
                    ((Entity)ActorPreview.EN).Animate(0, 1, 0);
                    TransformPivot.Parent(((Entity)ActorPreview.EN).FindChild(PropertyInterface.Template.AssignedBoneName), false);
                    TransformPivot.Position(PropertyInterface.Template.Position.X,
                        PropertyInterface.Template.Position.Y,
                        PropertyInterface.Template.Position.Z);

                    Program.Transformer = new GScaleTool(TransformPivot, new EventHandler(Scaling), new EventHandler(Scaled));
                }
            }
        }


        public void Moving(object sender, EventArgs e)
        {
            if (PropertyInterface != null)
            {
                PropertyInterface.Position = new RenderingServices.Vector3(TransformPivot.X(), TransformPivot.Y(), TransformPivot.Z());
            }
        }

        public void Scaling(object sender, EventArgs e)
        {
            if (PropertyInterface != null)
            {
                PropertyInterface.Scale = new RenderingServices.Vector3(TransformPivot.ScaleX(), TransformPivot.ScaleY(), TransformPivot.ScaleZ());
            }
        }

        public void Rotating(object sender, EventArgs e)
        {
            if (PropertyInterface != null)
            {
                PropertyInterface.Rotation = new RenderingServices.Vector3(TransformPivot.Pitch(), TransformPivot.Yaw(), TransformPivot.Roll());
            }
        }

        public void Moved(object sender, EventArgs e)
        {
//             if (Program.Transformer != null)
//                 Program.Transformer.Free();
// 
//             Program.Transformer = new MoveTool(TransformPivot, new EventHandler(Moving), new EventHandler(Moved));
            
        }


        public void Scaled(object sender, EventArgs e) { }
        public void Rotated(object sender, EventArgs e) { }

        private void GubbinEditor_Load(object sender, EventArgs e)
        {
            Started = true;

            TransformPivot = Entity.CreatePivot();
            Camera = Program.GE.Camera;
            CamOrigin = Entity.CreatePivot();

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

            HideRender(false);

            RenderPanel.Visible = false;
            RenderPanel = Program.GE.RenderingPanel;
        }

        public void HideRender(bool dontResetProperties)
        {
            foreach (Line3D L in Lines)
            {
                L.Visible = false;
            }

            if (ActorPreview != null && ActorPreview.CollisionEN != null)
                ((Entity)ActorPreview.CollisionEN).Visible = false;

            if (GubbinPreviewMesh != null)
                GubbinPreviewMesh.Visible = false;

            if (GubbinPreviewEmitterEN != null)
                GubbinPreviewEmitterEN.Visible = false;

            if (PreviewLight != null)
            {
                PreviewLight.Handle.Enable(0);
                PreviewLight.EN.Visible = false;
                PreviewLight.EN.Position(-10000, -10000, -10000);
                PreviewLight.UpdateLines();
            }

            if(!dontResetProperties)
                Program.GE.m_propertyWindow.ObjectProperties.SelectedObject = null;
        }

        public void ShowRender(bool dontResetProperties)
        {
            foreach (Line3D L in Lines)
            {
                L.Visible = true;
            }

            if (ActorPreview != null && ActorPreview.CollisionEN != null)
                ((Entity)ActorPreview.CollisionEN).Visible = true;

            if (GubbinPreviewMesh != null)
                GubbinPreviewMesh.Visible = true;

            if (GubbinPreviewEmitterEN != null)
                GubbinPreviewEmitterEN.Visible = true;

            if (PreviewLight != null)
            {
                PreviewLight.Handle.Enable(1);
                PreviewLight.EN.Visible = true;
                PreviewLight.EN.Position(PropertyInterface.Template.Position.X,
                    PropertyInterface.Template.Position.Y,
                    PropertyInterface.Template.Position.Z);
                PreviewLight.UpdateLines();
            }

            if(!dontResetProperties)
                Program.GE.m_propertyWindow.ObjectProperties.SelectedObject = PropertyInterface;
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

            if (RenderPanel != null)
                Mouse = RenderPanel.PointToClient(Cursor.Position);

            if (Program.Transformer != null)
                Program.Transformer.Update(Mouse.X, Mouse.Y);

            if (Program.Manager != null)
                Program.Manager.Update(Camera.X(), Camera.Y(), Camera.Z());


            // Update lights
            if(ActorPreview != null && ActorPreview.EN != null && PropertyInterface != null && PropertyInterface.Template != null
                && !string.IsNullOrEmpty(PropertyInterface.Template.AssignedBoneName))
            {
                Entity Bone = ((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN).FindChild(PropertyInterface.Template.AssignedBoneName);
                if (Bone != null)
                {
                    float Bx = Bone.X(true);
                    float By = Bone.Y(true);
                    float Bz = Bone.Z(true);

                    Vector3 Position = PropertyInterface.Template.Position;
                    Vector3 Rotation = PropertyInterface.Template.Rotation;

                    if (PreviewLight != null)
                    {
                        Entity Piv = Entity.CreatePivot();
                        Piv.Parent(Bone, false);
                        Piv.Position(Position.X, Position.Y, Position.Z);
                        Piv.Rotate(Rotation.X, Rotation.Y, Rotation.Z);

                        Bx = Piv.X(true);
                        By = Piv.Y(true);
                        Bz = Piv.Z(true);

                        PreviewLight.EN.Position(Bx, By, Bz);
                        PreviewLight.MoverPivot.Position(Bx, By, Bz);
                        PreviewLight.Handle.Position(Bx, By, Bz);

                        Piv.Free();

                        PreviewLight.UpdateLines();
                    }

                    if (GubbinPreviewEmitterEN != null)
                    {
                        Entity Piv = Entity.CreatePivot();
                        Piv.Parent(Bone, false);
                        Piv.Position(Position.X, Position.Y, Position.Z);
                        Piv.Rotate(Rotation.X, Rotation.Y, Rotation.Z);

                        Bx = Piv.X(true);
                        By = Piv.Y(true);
                        Bz = Piv.Z(true);

                        GubbinPreviewEmitterEN.Position(Bx, By, Bz);    

                        Piv.Free();
                    }
                }
            }

            LightFunctionList.Update();
            if (PreviewLight != null)
            {
                if (PreviewLight.Function != null)
                {
                    PreviewLight.Handle.Color(
                        Convert.ToInt32(PreviewLight.Function.Color.R),
                        Convert.ToInt32(PreviewLight.Function.Color.G),
                        Convert.ToInt32(PreviewLight.Function.Color.B));
                    PreviewLight.Handle.Radius(Convert.ToSingle(PreviewLight.Function.Radius));
                }
            }
            

            RottParticles.General.Update(1.0f);
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

        public void GubbinEditor_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
                
        }

        public void GubbinEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            //if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)

        }

        private void TemplateNewButton_Click(object sender, EventArgs e)
        {
            NamePrompt Np = new NamePrompt();
            Np.Title = "Enter Name...";
            Np.Prompt = "Enter name for Gubbin Template:";

            if (Np.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(Np.Value))
                    return;

                GubbinTemplate T = new GubbinTemplate();

                for (int i = 0; i < 65535; ++i)
                {
                    bool Found = false;
                    for (int tid = 0; tid < Templates.Count; ++tid)
                    {
                        if (Templates[tid].ID == i)
                        {
                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        T.ID = (ushort)i;
                        break;
                    }
                }

                T.Name = Np.Value;
                Templates.Add(T);
                TemplatesList.Items.Add(T);
                TemplatesList.SelectedIndex = TemplatesList.Items.Count - 1;

                Saved = false;
            }
        }

        private void TemplateEditButton_Click(object sender, EventArgs e)
        {
            GubbinTemplate T = TemplatesList.SelectedItem as GubbinTemplate;
            if (T == null)
                return;

            NamePrompt Np = new NamePrompt();
            Np.Title = "Enter Name...";
            Np.Prompt = "Enter name for Gubbin Template:";

            if(Np.ShowDialog() == DialogResult.OK)
            {
                T.Name = Np.Value;

                int Idx = TemplatesList.SelectedIndex;
                TemplatesList.Items.Clear();
                for (int i = 0; i < Templates.Count; ++i)
                    TemplatesList.Items.Add(Templates[i]);
                TemplatesList.SelectedIndex = Idx;

                Saved = false;
            }
        }

        private void TemplateRemoveButton_Click(object sender, EventArgs e)
        {
            GubbinTemplate T = TemplatesList.SelectedItem as GubbinTemplate;
            if (T == null)
                return;

            if (MessageBox.Show("Are you sure you want to delete this template and its contents?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            // Remove any current editor crap
            ClearActorPreview();

            // Remove template
            Templates.Remove(T);
            TemplatesList.Items.Remove(T);
            Saved = false;
        }

        private void TemplatesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Remove current selection stuff here
            ClearActorPreview();

            TemplateActorsList.Items.Clear();
            GubbinTemplate T = TemplatesList.SelectedItem as GubbinTemplate;
            if (T == null)
                return;

            foreach (GubbinActorTemplate At in T.ActorTemplates)
            {
                TemplateActorsList.Items.Add(At.Actor.Race + " [" + At.Actor.Class + "] [" + ((At.Gender == 0) ? "Male" : "Female") + "]");
            }

            if (TemplateActorsList.Items.Count > 0)
                TemplateActorsList.SelectedIndex = 0;
            else
                TemplateActorsList_SelectedIndexChanged(sender, e);


        }

        private void ActorsAddButton_Click(object sender, EventArgs e)
        {
            GubbinTemplate T = TemplatesList.SelectedItem as GubbinTemplate;
            if (T == null)
                return;

            NewActorTemplate TSel = new NewActorTemplate();
            if (TSel.ShowDialog() == DialogResult.Cancel)
                return;

            if (TSel.Actor == null)
                return;

            GubbinActorTemplate Tmpl = new GubbinActorTemplate();

            foreach (GubbinActorTemplate CurrentTemplate in T.ActorTemplates)
            {
                if (CurrentTemplate.Actor == TSel.Actor && CurrentTemplate.Gender == TSel.Gender)
                {
                    MessageBox.Show("Actor Gubbin already exists from this template!");
                    return;
                }
            }


            Tmpl.Actor = TSel.Actor;
            Tmpl.Gender = TSel.Gender;

            T.ActorTemplates.Add(Tmpl);
            TemplateActorsList.Items.Add(Tmpl.Actor.Race + " [" + Tmpl.Actor.Class + "] [" + ((Tmpl.Gender == 0) ? "Male" : "Female") + "]");
            TemplateActorsList.SelectedIndex = TemplateActorsList.Items.Count - 1;

            
        }

        private void ClearActorPreview()
        {


            if (Program.Transformer != null)
                Program.Transformer.Free();
            Program.Transformer = null;

            if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
            {
                Program.GE.m_GubbinEditor.GubbinPreviewMesh.Free();
                Program.GE.m_GubbinEditor.GubbinPreviewMesh = null;
            }

            if (Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN != null)
            {
                RottParticles.General.FreeEmitter(Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN, true, false);
                Program.GE.m_GubbinEditor.GubbinPreviewConfig = null;
            }

            if (Program.GE.m_GubbinEditor.PreviewLight != null)
            {
                Program.GE.m_GubbinEditor.PreviewLight.Freeing();
                if (Program.GE.m_GubbinEditor.PreviewLight.Handle != null)
                    Program.GE.m_GubbinEditor.PreviewLight.Handle.Free();
                if (Program.GE.m_GubbinEditor.PreviewLight.EN != null)
                    Program.GE.m_GubbinEditor.PreviewLight.EN.Free();
            }
            Program.GE.m_GubbinEditor.PreviewLight = null;

            if (TransformPivot != null)
                TransformPivot.Parent(null, false);

            if (ActorPreview != null)
            {
                Actors3D.SafeFreeActorInstance(ActorPreview);
                ActorPreview = null;
                
            }

            
        }

        private void ActorsRemoveButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this Actor Template?", "Delete Actor Template", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TemplateActorsList.Items.RemoveAt(TemplateActorsList.SelectedIndex);
                TemplateActorsList.SelectedIndex = TemplateActorsList.Items.Count - 1;
            }
        }

        private void TemplateActorsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearActorPreview();

            GubbinTemplate T = TemplatesList.SelectedItem as GubbinTemplate;
            if (T == null)
                return;

            if (TemplateActorsList.SelectedIndex == -1)
                return;

            GubbinActorTemplate ActorTemplate = T.ActorTemplates[TemplateActorsList.SelectedIndex];
            if (ActorTemplate == null)
                return;

            // Load new actor
            ActorPreview = new ActorInstance(ActorTemplate.Actor);
            ActorPreview.Gender = ActorTemplate.Gender;
            
            bool Result = Actors3D.LoadActorInstance3D(ActorPreview, 0.1f / ActorTemplate.Actor.Scale);
            if (Result == false)
            {
                ActorPreview.Dispose();
                ActorPreview = null;
                return;
            }
            if (ActorPreview.ShadowEN != null)
            {
                ((Entity)ActorPreview.ShadowEN).Free();
                ActorPreview.ShadowEN = null;
            }
            float MinY = RenderWrapper.bbdx2_GetTransformedBoundingBoxMinY(((Entity)ActorPreview.EN).Handle);
            ((Entity)ActorPreview.CollisionEN).Position(0f, -MinY, 0f);
            CamOrigin.Position(0, -MinY, 0);
            AnimSet.PlayAnimation(ActorPreview, 1, 0.5f, (int)AnimSet.Anim.Idle);

            BoneNames.Clear();
            int ChildCount = ((Entity)ActorPreview.EN).CountChildren();
            for (int i = 0; i < ChildCount; ++i)
            {
                BoneNames.Add(((Entity)ActorPreview.EN).GetChild(i + 1).Name);
            }

            if (BoneNames.Count > 0 && string.IsNullOrEmpty(ActorTemplate.AssignedBoneName))
                ActorTemplate.AssignedBoneName = BoneNames[0];

            // Setup property grid
            PropertyInterface = new ActorGubbinPropertyInterface(ActorTemplate, ActorPreview);

            bool WasSaved = saved;

            // Load all default datas
            PropertyInterface.Mesh = new MeshDisplay(ActorTemplate.MeshID);
            PropertyInterface.Emitter = ActorTemplate.Emitter;
            PropertyInterface.LightEnabled = ActorTemplate.UseLight;

            if (WasSaved)
                Saved = true;

            Program.GE.m_propertyWindow.ObjectProperties.SelectedObject = PropertyInterface;
        }
    }
}