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
using RenderingServices;

namespace RealmCrafter_GE
{
    public class GScaleTool : TransformTool
    {
        public const float ToolScale = 2.0f;
        private Entity Mesh;
        public Entity Parent;
        public float Multiplier = 0.2f;
        bool SelectedX = false;
        bool SelectedY = false;
        bool SelectedZ = false;
        int LastX = 0;
        int LastY = 0;
        Vector3 Offset = new Vector3();
        Vector3 Scale = new Vector3(0, 0, 0);
        Vector3 MeshScale = new Vector3(1, 1, 1);

        bool FirstUpdate = false;

        public bool AllowScaleX = true;
        public bool AllowScaleY = true;
        public bool AllowScaleZ = true;
        public bool CopyXToZ = false;
        public bool CopyXToY = false;

        public EventHandler ObjectScaling;
        public EventHandler ObjectScaled;

        public GScaleTool(Entity parent)
        {
            BuildMesh();
            Parent = parent;
        }

        public GScaleTool(Entity parent, EventHandler objectScaling, EventHandler objectScaled)
        {
            Parent = parent;
            if (Parent != null)
                MeshScale = new Vector3(Parent.ScaleX(), Parent.ScaleY(), Parent.ScaleZ());
            ObjectScaling = objectScaling;
            ObjectScaled = objectScaled;

            BuildMesh();
        }

        ~GScaleTool()
        {
            Free();
        }

        public override bool HasParent(Entity parent)
        {
            if (parent != null && Parent != null)
            {
                if (parent.Handle == Parent.Handle)
                    return true;
            }

            return false;
        }
        public override bool Visible
        {
            get
            {
                if (Mesh == null)
                    return false;
                return Mesh.Visible;
            }
            set
            {
                if (Mesh == null)
                    return;
                Mesh.Visible = value;
            }
        }

        public override void Free()
        {
            if (Mesh != null)
                Mesh.Free();
            Mesh = null;
        }

        public override void Update(int x, int y)
        {
            if (Mesh == null)
                return;

            float MoveX = Convert.ToSingle(x - LastX);
            float MoveY = Convert.ToSingle(y - LastY);
            LastX = x;
            LastY = y;

            RenderWrapper.bbdx2_ManagedUnProjectVector3(Convert.ToSingle(x), Convert.ToSingle(y), 0.0f);
            Vector3 LineStart = new Vector3(RenderWrapper.bbdx2_ProjectedX(), RenderWrapper.bbdx2_ProjectedY(), RenderWrapper.bbdx2_ProjectedZ());
            RenderWrapper.bbdx2_ManagedUnProjectVector3(Convert.ToSingle(x), Convert.ToSingle(y), 1.0f);
            Vector3 LineEnd = new Vector3(RenderWrapper.bbdx2_ProjectedX(), RenderWrapper.bbdx2_ProjectedY(), RenderWrapper.bbdx2_ProjectedZ());

            if (SelectedX && AllowScaleX)
                if (RenderWrapper.bbdx2_RayToPlane(0, 1, 0, Parent.X(true), Parent.Y(true), Parent.Z(true), LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z) > 0)
                    Scale.X = RenderWrapper.bbdx2_ProjectedX() - Offset.X;

            if (SelectedY && AllowScaleY)
                if (!SelectedX && RenderWrapper.bbdx2_RayToPlane(1, 0, 0, Parent.X(true), Parent.Y(true), Parent.Z(true), LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z) > 0)
                    Scale.Y = RenderWrapper.bbdx2_ProjectedY() - Offset.Y;
                else if (!SelectedZ && RenderWrapper.bbdx2_RayToPlane(0, 0, 1, Parent.X(true), Parent.Y(true), Parent.Z(true), LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z) > 0)
                    Scale.Y = RenderWrapper.bbdx2_ProjectedY() - Offset.Y;

            if (SelectedZ && AllowScaleZ)
                if (RenderWrapper.bbdx2_RayToPlane(0, 1, 0, Parent.X(true), Parent.Y(true), Parent.Z(true), LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z) > 0)
                    Scale.Z = RenderWrapper.bbdx2_ProjectedZ() - Offset.Z;

            if (CopyXToZ)
            {
                Scale.Z = Scale.X;
                MeshScale.Z = MeshScale.X;
            }

            if (CopyXToY)
            {
                Scale.Y = Scale.X;
                MeshScale.Y = MeshScale.X;
            }

            if (Parent != null && FirstUpdate)
            {
                Parent.Scale(
                    (Scale.X * Multiplier) + MeshScale.X,
                    (Scale.Y * Multiplier) + MeshScale.Y,
                    (Scale.Z * Multiplier) + MeshScale.Z);


            }
            else
            {
                FirstUpdate = true;
            }

            if (SelectedX || SelectedY || SelectedZ)
                if (ObjectScaling != null)
                    ObjectScaling(this, EventArgs.Empty);

            if (Parent != null)
                Mesh.Position(Parent.X(true), Parent.Y(true), Parent.Z(true));
        }

        void Multiply(ref Vector3 a, float b)
        {
            a.X *= b;
            a.Y *= b;
            a.Z *= b;
        }

        void Add(ref Vector3 a, Vector3 b)
        {
            a.X += b.X;
            a.Y += b.Y;
            a.Z += b.Z;
        }

        float Length(ref Vector2 a)
        {
            return Convert.ToSingle(Math.Sqrt(Convert.ToDouble(a.X * a.X + a.Y * a.Y)));
        }

        Vector3 GetCenter(Vector3 Min, Vector3 Max)
        {
            return new Vector3(
            Min.X + ((Max.X - Min.X) * 0.5f),
            Min.Y + ((Max.Y - Min.Y) * 0.5f),
            Min.Z + ((Max.Z - Min.Z) * 0.5f));
        }

        float Length(ref Vector3 Min, ref Vector3 Max, ref Vector3 Center)
        {
            Vector3 Temp = new Vector3(
            Min.X + ((Max.X - Min.X) * 0.5f),
            Min.Y + ((Max.Y - Min.Y) * 0.5f),
            Min.Z + ((Max.Z - Min.Z) * 0.5f));

            Temp.X -= Center.X;
            Temp.Y -= Center.Y;
            Temp.Z -= Center.Z;

            return Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Temp.X * Temp.X + Temp.Y * Temp.Y + Temp.Z * Temp.Z)));
        }

        void Normalize(ref Vector2 a)
        {
            float Len = Length(ref a);
            a.X /= Len;
            a.Y /= Len;
        }

        public override bool MouseDown(Entity camera, int x, int y)
        {
            LastX = x;
            LastY = y;
            this.Scale = new Vector3(0, 0, 0);

            if (Parent != null)
                this.MeshScale = new Vector3(Parent.ScaleX(), Parent.ScaleY(), Parent.ScaleZ());
            else
                this.MeshScale = new Vector3(1, 1, 1);

            if(CopyXToZ)
                this.MeshScale.Z = this.MeshScale.X;

            float Scale = 0.1f * ToolScale;
            Vector3 BoxXMin = new Vector3(0.7f * ToolScale, -0.5f * Scale, -0.5f * Scale);
            Vector3 BoxXMax = new Vector3(1.0f * ToolScale, 0.5f * Scale, 0.5f * Scale);
            Vector3 BoxYMin = new Vector3(-0.5f * Scale, 0.7f * ToolScale, -0.5f * Scale);
            Vector3 BoxYMax = new Vector3(0.5f * Scale, 1.0f * ToolScale, 0.5f * Scale);
            Vector3 BoxZMin = new Vector3(-0.5f * Scale, -0.5f * Scale, 0.7f * ToolScale);
            Vector3 BoxZMax = new Vector3(0.5f * Scale, 0.5f * Scale, 1.0f * ToolScale);

            Vector3 BoxMins = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 BoxXZMax = new Vector3(0.3f * ToolScale, 0.03f, 0.3f * ToolScale);
            Vector3 BoxXYMax = new Vector3(0.3f * ToolScale, 0.3f * ToolScale, 0.03f);
            Vector3 BoxZYMax = new Vector3(0.03f, 0.3f * ToolScale, 0.3f * ToolScale);

            Vector3 Center = new Vector3(0, 0, 0);
            if (Parent != null)
                Center = new Vector3(Parent.X(true), Parent.Y(true), Parent.Z(true));

            Vector3 Camera = new Vector3(0, 0, 0);
            if (camera != null)
                Camera = new Vector3(camera.X(true), camera.Y(true), camera.Z(true));

            Vector3 Dist = new Vector3(Center.X - Camera.X, Center.Y - Camera.Y, Center.Z - Camera.Z);
            float Depth = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Dist.X * Dist.X + Dist.Y * Dist.Y + Dist.Z * Dist.Z)));
            float MeshScale = (2.0f * Convert.ToSingle(Math.Sin(1.5708 * 0.5)) * Depth) * 0.05f;

            Multiply(ref BoxXMin, MeshScale);
            Multiply(ref BoxXMax, MeshScale);
            Multiply(ref BoxYMin, MeshScale);
            Multiply(ref BoxYMax, MeshScale);
            Multiply(ref BoxZMin, MeshScale);
            Multiply(ref BoxZMax, MeshScale);

            Multiply(ref BoxMins, MeshScale);
            Multiply(ref BoxXZMax, MeshScale);
            Multiply(ref BoxXYMax, MeshScale);
            Multiply(ref BoxZYMax, MeshScale);

            Vector3 BoxXOffset = GetCenter(BoxXMin, BoxXMax);
            Vector3 BoxYOffset = GetCenter(BoxYMin, BoxYMax);
            Vector3 BoxZOffset = GetCenter(BoxZMin, BoxZMax);
            Vector3 BoxXZOffset = GetCenter(BoxMins, BoxXZMax);
            Vector3 BoxXYOffset = GetCenter(BoxMins, BoxXYMax);
            Vector3 BoxZYOffset = GetCenter(BoxMins, BoxZYMax);

            Add(ref BoxXMin, Center);
            Add(ref BoxXMax, Center);
            Add(ref BoxYMin, Center);
            Add(ref BoxYMax, Center);
            Add(ref BoxZMin, Center);
            Add(ref BoxZMax, Center);
            Add(ref BoxMins, Center);
            Add(ref BoxXZMax, Center);
            Add(ref BoxXYMax, Center);
            Add(ref BoxZYMax, Center);

            RenderWrapper.bbdx2_ManagedUnProjectVector3(Convert.ToSingle(x), Convert.ToSingle(y), 0.0f);
            Vector3 LineStart = new Vector3(RenderWrapper.bbdx2_ProjectedX(), RenderWrapper.bbdx2_ProjectedY(), RenderWrapper.bbdx2_ProjectedZ());
            RenderWrapper.bbdx2_ManagedUnProjectVector3(Convert.ToSingle(x), Convert.ToSingle(y), 1.0f);
            Vector3 LineEnd = new Vector3(RenderWrapper.bbdx2_ProjectedX(), RenderWrapper.bbdx2_ProjectedY(), RenderWrapper.bbdx2_ProjectedZ());



            int Hit = 0;
            Hit = RenderWrapper.bbdx2_RayToAABB(BoxXMin.X, BoxXMin.Y, BoxYMin.Z, BoxXMax.X, BoxXMax.Y, BoxXMax.Z, LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z);
            if (Hit != 0)
            {
                FirstUpdate = false;
                Offset = BoxXOffset;
                SelectedX = true;
                BuildMesh();
                return true;
            }

            Hit = RenderWrapper.bbdx2_RayToAABB(BoxYMin.X, BoxYMin.Y, BoxYMin.Z, BoxYMax.X, BoxYMax.Y, BoxYMax.Z, LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z);
            if (Hit != 0)
            {
                FirstUpdate = false;
                Offset = BoxYOffset;
                SelectedY = true;
                BuildMesh();
                return true;
            }

            Hit = RenderWrapper.bbdx2_RayToAABB(BoxZMin.X, BoxZMin.Y, BoxZMin.Z, BoxZMax.X, BoxZMax.Y, BoxZMax.Z, LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z);
            if (Hit != 0)
            {
                FirstUpdate = false;
                Offset = BoxZOffset;
                SelectedZ = true;
                BuildMesh();
                return true;
            }

            Hit = RenderWrapper.bbdx2_RayToAABB(BoxMins.X, BoxMins.Y, BoxMins.Z, BoxXZMax.X, BoxXZMax.Y, BoxXZMax.Z, LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z);
            if (Hit != 0)
            {
                FirstUpdate = false;
                Offset = BoxXZOffset;
                SelectedX = true;
                SelectedZ = true;
                BuildMesh();
                return true;
            }

            Hit = RenderWrapper.bbdx2_RayToAABB(BoxMins.X, BoxMins.Y, BoxMins.Z, BoxXYMax.X, BoxXYMax.Y, BoxXYMax.Z, LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z);
            if (Hit != 0)
            {
                FirstUpdate = false;
                Offset = BoxXYOffset;
                SelectedX = true;
                SelectedY = true;
                BuildMesh();
                return true;
            }

            Hit = RenderWrapper.bbdx2_RayToAABB(BoxMins.X, BoxMins.Y, BoxMins.Z, BoxZYMax.X, BoxZYMax.Y, BoxZYMax.Z, LineStart.X, LineStart.Y, LineStart.Z, LineEnd.X, LineEnd.Y, LineEnd.Z);
            if (Hit != 0)
            {
                FirstUpdate = false;
                Offset = BoxZYOffset;
                SelectedZ = true;
                SelectedY = true;
                BuildMesh();
                return true;
            }


            return false;

        }

        public override void MouseUp()
        {
            if (SelectedX || SelectedY || SelectedZ)
                if (ObjectScaled != null)
                    ObjectScaled(this, EventArgs.Empty);

            FirstUpdate = false;
            SelectedX = false;
            SelectedY = false;
            SelectedZ = false;
            BuildMesh();
        }

        void BuildMesh()
        {
            if (Mesh != null)
                Mesh.Free();

            #region Mesh Creation
            Mesh = Entity.CreateMesh();
            Mesh.Order = -1000;

            uint S = Mesh.CreateSurface();
            Entity.AddVertex(S, 0, 0, -0.005f);
            Entity.AddVertex(S, 0.7f * ToolScale, 0, -0.005f);
            Entity.AddVertex(S, 0, 0, 0.005f);
            Entity.AddVertex(S, 0.7f * ToolScale, 0, 0.005f);

            Entity.AddVertex(S, 0, -0.005f, 0);
            Entity.AddVertex(S, 0.7f * ToolScale, -0.005f, 0);
            Entity.AddVertex(S, 0, 0.005f, 0);
            Entity.AddVertex(S, 0.7f * ToolScale, 0.005f, 0);

            Entity.AddVertex(S, -0.005f, 0, 0);
            Entity.AddVertex(S, -0.005f, 0, 0.7f * ToolScale);
            Entity.AddVertex(S, 0.005f, 0, 0);
            Entity.AddVertex(S, 0.005f, 0, 0.7f * ToolScale);

            Entity.AddVertex(S, 0, -0.005f, 0);
            Entity.AddVertex(S, 0, -0.005f, 0.7f * ToolScale);
            Entity.AddVertex(S, 0, 0.005f, 0);
            Entity.AddVertex(S, 0, 0.005f, 0.7f * ToolScale);

            Entity.AddVertex(S, -0.005f, 0, 0);
            Entity.AddVertex(S, -0.005f, 0.7f * ToolScale, 0);
            Entity.AddVertex(S, 0.005f, 0, 0);
            Entity.AddVertex(S, 0.005f, 0.7f * ToolScale, 0);

            Entity.AddVertex(S, 0, 0, -0.005f);
            Entity.AddVertex(S, 0, 0.7f * ToolScale, -0.005f);
            Entity.AddVertex(S, 0, 0, 0.005f);
            Entity.AddVertex(S, 0, 0.7f * ToolScale, 0.005f);

            for (int i = 0; i < 8; ++i)
            {
                if (SelectedX)
                    Entity.VertexColor(S, i + 0, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i + 0, 200, 0, 0, 1.0f);
            }

            for (int i = 0; i < 8; ++i)
            {
                if (SelectedZ)
                    Entity.VertexColor(S, i + 8, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i + 8, 0, 0, 196, 1.0f);
            }

            for (int i = 0; i < 8; ++i)
            {
                if (SelectedY)
                    Entity.VertexColor(S, i + 16, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i + 16, 0, 156, 0, 1.0f);
            }


            Entity.AddTriangle(S, 0, 1, 2);
            Entity.AddTriangle(S, 1, 3, 2);
            Entity.AddTriangle(S, 4, 5, 6);
            Entity.AddTriangle(S, 5, 7, 6);
            Entity.AddTriangle(S, 8, 9, 10);
            Entity.AddTriangle(S, 9, 11, 10);

            Entity.AddTriangle(S, 12, 13, 14);
            Entity.AddTriangle(S, 13, 15, 14);

            Entity.AddTriangle(S, 16, 17, 18);
            Entity.AddTriangle(S, 17, 19, 18);

            Entity.AddTriangle(S, 20, 21, 22);
            Entity.AddTriangle(S, 21, 23, 22);

            float Scale = 0.1f * ToolScale;
            Entity.AddVertex(S, 0.7f * ToolScale, -0.25f * Scale, 0.5f * Scale); // 24
            Entity.AddVertex(S, 0.7f * ToolScale, 0.25f * Scale, 0.5f * Scale); // 25
            Entity.AddVertex(S, 0.7f * ToolScale, 0.5f * Scale, 0.25f * Scale); // 26
            Entity.AddVertex(S, 0.7f * ToolScale, 0.5f * Scale, -0.25f * Scale); // 27
            Entity.AddVertex(S, 0.7f * ToolScale, 0.25f * Scale, -0.5f * Scale); // 28
            Entity.AddVertex(S, 0.7f * ToolScale, -0.25f * Scale, -0.5f * Scale); // 29
            Entity.AddVertex(S, 0.7f * ToolScale, -0.5f * Scale, -0.25f * Scale); // 30
            Entity.AddVertex(S, 0.7f * ToolScale, -0.5f * Scale, 0.25f * Scale); // 31
            Entity.AddVertex(S, 1.0f * ToolScale, 0.0f, 0.0f); // 32

            for (int i = 24; i < 33; ++i)
            {
                if (SelectedX)
                    Entity.VertexColor(S, i, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i, 200, 0, 0, 1.0f);
            }

            Entity.AddTriangle(S, 32, 24, 25);
            Entity.AddTriangle(S, 32, 25, 26);
            Entity.AddTriangle(S, 32, 26, 27);
            Entity.AddTriangle(S, 32, 27, 28);
            Entity.AddTriangle(S, 32, 28, 29);
            Entity.AddTriangle(S, 32, 29, 30);
            Entity.AddTriangle(S, 32, 30, 31);
            Entity.AddTriangle(S, 32, 31, 24);

            Entity.AddVertex(S, 0.5f * Scale, -0.25f * Scale, 0.7f * ToolScale); // 33
            Entity.AddVertex(S, 0.5f * Scale, 0.25f * Scale, 0.7f * ToolScale); // 34
            Entity.AddVertex(S, 0.25f * Scale, 0.5f * Scale, 0.7f * ToolScale); // 35
            Entity.AddVertex(S, -0.25f * Scale, 0.5f * Scale, 0.7f * ToolScale); // 36
            Entity.AddVertex(S, -0.5f * Scale, 0.25f * Scale, 0.7f * ToolScale); // 37
            Entity.AddVertex(S, -0.5f * Scale, -0.25f * Scale, 0.7f * ToolScale); // 38
            Entity.AddVertex(S, -0.25f * Scale, -0.5f * Scale, 0.7f * ToolScale); // 39
            Entity.AddVertex(S, 0.25f * Scale, -0.5f * Scale, 0.7f * ToolScale); // 40
            Entity.AddVertex(S, 0.0f, 0.0f, 1.0f * ToolScale); // 41

            for (int i = 33; i < 42; ++i)
            {
                if (SelectedZ)
                    Entity.VertexColor(S, i, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i, 0, 0, 195, 1.0f);
            }

            Entity.AddTriangle(S, 41, 33, 34);
            Entity.AddTriangle(S, 41, 34, 35);
            Entity.AddTriangle(S, 41, 35, 36);
            Entity.AddTriangle(S, 41, 36, 37);
            Entity.AddTriangle(S, 41, 37, 38);
            Entity.AddTriangle(S, 41, 38, 39);
            Entity.AddTriangle(S, 41, 39, 40);
            Entity.AddTriangle(S, 41, 40, 33);

            Entity.AddVertex(S, -0.25f * Scale, 0.7f * ToolScale, 0.5f * Scale); // 42
            Entity.AddVertex(S, 0.25f * Scale, 0.7f * ToolScale, 0.5f * Scale); // 43
            Entity.AddVertex(S, 0.5f * Scale, 0.7f * ToolScale, 0.25f * Scale); // 44
            Entity.AddVertex(S, 0.5f * Scale, 0.7f * ToolScale, -0.25f * Scale); // 45
            Entity.AddVertex(S, 0.25f * Scale, 0.7f * ToolScale, -0.5f * Scale); // 46
            Entity.AddVertex(S, -0.25f * Scale, 0.7f * ToolScale, -0.5f * Scale); // 47
            Entity.AddVertex(S, -0.5f * Scale, 0.7f * ToolScale, -0.25f * Scale); // 48
            Entity.AddVertex(S, -0.5f * Scale, 0.7f * ToolScale, 0.25f * Scale); // 49
            Entity.AddVertex(S, 0.0f, 1.0f * ToolScale, 0.0f); // 50

            for (int i = 42; i < 51; ++i)
            {
                if (SelectedY)
                    Entity.VertexColor(S, i, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i, 0, 156, 0, 1.0f);
            }

            Entity.AddTriangle(S, 50, 42, 43);
            Entity.AddTriangle(S, 50, 43, 44);
            Entity.AddTriangle(S, 50, 44, 45);
            Entity.AddTriangle(S, 50, 45, 46);
            Entity.AddTriangle(S, 50, 46, 47);
            Entity.AddTriangle(S, 50, 47, 48);
            Entity.AddTriangle(S, 50, 48, 49);
            Entity.AddTriangle(S, 50, 49, 42);

            Entity.AddVertex(S, 0.3f * ToolScale - 0.005f, 0.0f, 0.0f); // 51
            Entity.AddVertex(S, 0.3f * ToolScale - 0.005f, 0.3f * ToolScale, 0.0f); // 52
            Entity.AddVertex(S, 0.3f * ToolScale + 0.005f, 0.0f, 0.0f); // 53
            Entity.AddVertex(S, 0.3f * ToolScale + 0.005f, 0.3f * ToolScale, 0.0f); // 54
            Entity.AddVertex(S, 0.3f * ToolScale, 0.0f, -0.005f); // 55
            Entity.AddVertex(S, 0.3f * ToolScale, 0.3f * ToolScale, -0.005f); // 56
            Entity.AddVertex(S, 0.3f * ToolScale, 0.0f, 0.005f); // 57
            Entity.AddVertex(S, 0.3f * ToolScale, 0.3f * ToolScale, 0.005f); // 58

            Entity.AddVertex(S, 0.3f * ToolScale - 0.005f, 0.0f, 0.0f); // 59
            Entity.AddVertex(S, 0.3f * ToolScale - 0.005f, 0.0f, 0.3f * ToolScale); // 60
            Entity.AddVertex(S, 0.3f * ToolScale + 0.005f, 0.0f, 0.0f); // 61
            Entity.AddVertex(S, 0.3f * ToolScale + 0.005f, 0.0f, 0.3f * ToolScale); // 62
            Entity.AddVertex(S, 0.3f * ToolScale, -0.005f, 0.0f); // 63
            Entity.AddVertex(S, 0.3f * ToolScale, -0.005f, 0.3f * ToolScale); // 64
            Entity.AddVertex(S, 0.3f * ToolScale, 0.005f, 0.0f); // 65
            Entity.AddVertex(S, 0.3f * ToolScale, 0.005f, 0.3f * ToolScale); // 66

            for (int i = 51; i < 67; ++i)
            {
                if (SelectedX)
                    Entity.VertexColor(S, i, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i, 200, 0, 0, 1.0f);
            }

            Entity.AddTriangle(S, 51, 52, 53);
            Entity.AddTriangle(S, 52, 53, 54);
            Entity.AddTriangle(S, 55, 56, 57);
            Entity.AddTriangle(S, 56, 57, 58);
            Entity.AddTriangle(S, 59, 60, 61);
            Entity.AddTriangle(S, 60, 61, 62);
            Entity.AddTriangle(S, 63, 64, 65);
            Entity.AddTriangle(S, 64, 65, 66);

            Entity.AddVertex(S, 0.0f, 0.0f, 0.3f * ToolScale - 0.005f); // 67
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale, 0.3f * ToolScale - 0.005f); // 68
            Entity.AddVertex(S, 0.0f, 0.0f, 0.3f * ToolScale + 0.005f); // 69
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale, 0.3f * ToolScale + 0.005f); // 70
            Entity.AddVertex(S, -0.005f, 0.0f, 0.3f * ToolScale); // 71
            Entity.AddVertex(S, -0.005f, 0.3f * ToolScale, 0.3f * ToolScale); // 72
            Entity.AddVertex(S, 0.005f, 0.0f, 0.3f * ToolScale); // 73
            Entity.AddVertex(S, 0.005f, 0.3f * ToolScale, 0.3f * ToolScale); // 74

            Entity.AddVertex(S, 0.0f, 0.0f, 0.3f * ToolScale - 0.005f); // 75
            Entity.AddVertex(S, 0.3f * ToolScale, 0.0f, 0.3f * ToolScale - 0.005f); // 76
            Entity.AddVertex(S, 0.0f, 0.0f, 0.3f * ToolScale + 0.005f); // 77
            Entity.AddVertex(S, 0.3f * ToolScale, 0.0f, 0.3f * ToolScale + 0.005f); // 78
            Entity.AddVertex(S, 0.3f * ToolScale, -0.005f, 0.3f * ToolScale); // 79
            Entity.AddVertex(S, 0.3f * ToolScale, -0.005f, 0.3f * ToolScale); // 80
            Entity.AddVertex(S, 0.0f, 0.005f, 0.3f * ToolScale); // 81
            Entity.AddVertex(S, 0.3f * ToolScale, 0.005f, 0.3f * ToolScale); // 82

            for (int i = 51 + 16; i < 67 + 16; ++i)
            {
                if (SelectedZ)
                    Entity.VertexColor(S, i, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i, 0, 0, 195, 1.0f);
            }

            Entity.AddTriangle(S, 51 + 16, 52 + 16, 53 + 16);
            Entity.AddTriangle(S, 52 + 16, 53 + 16, 54 + 16);
            Entity.AddTriangle(S, 55 + 16, 56 + 16, 57 + 16);
            Entity.AddTriangle(S, 56 + 16, 57 + 16, 58 + 16);
            Entity.AddTriangle(S, 59 + 16, 60 + 16, 61 + 16);
            Entity.AddTriangle(S, 60 + 16, 61 + 16, 62 + 16);
            Entity.AddTriangle(S, 63 + 16, 64 + 16, 65 + 16);
            Entity.AddTriangle(S, 64 + 16, 65 + 16, 66 + 16);

            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale, 0.0f - 0.005f); // 83
            Entity.AddVertex(S, 0.3f * ToolScale, 0.3f * ToolScale, 0.0f - 0.005f); // 84
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale, 0.0f + 0.005f); // 85
            Entity.AddVertex(S, 0.3f * ToolScale, 0.3f * ToolScale, 0.0f + 0.005f); // 86
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale - 0.005f, 0.0f); // 87
            Entity.AddVertex(S, 0.3f * ToolScale, 0.3f * ToolScale - 0.005f, 0.0f); // 88
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale + 0.005f, 0.0f); // 89
            Entity.AddVertex(S, 0.3f * ToolScale, 0.3f * ToolScale + 0.005f, 0.0f); // 90

            Entity.AddVertex(S, 0.0f - 0.005f, 0.3f * ToolScale, 0.0f); // 91
            Entity.AddVertex(S, 0.0f - 0.005f, 0.3f * ToolScale, 0.3f * ToolScale); // 92
            Entity.AddVertex(S, 0.0f + 0.005f, 0.3f * ToolScale, 0.0f); // 93
            Entity.AddVertex(S, 0.0f + 0.005f, 0.3f * ToolScale, 0.3f * ToolScale); // 94
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale - 0.005f, 0.0f); // 95
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale - 0.005f, 0.3f * ToolScale); // 96
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale + 0.005f, 0.0f); // 97
            Entity.AddVertex(S, 0.0f, 0.3f * ToolScale + 0.005f, 0.3f * ToolScale); // 98

            for (int i = 51 + 32; i < 67 + 32; ++i)
            {
                if (SelectedY)
                    Entity.VertexColor(S, i, 242, 217, 0, 1.0f);
                else
                    Entity.VertexColor(S, i, 0, 156, 0, 1.0f);
            }

            Entity.AddTriangle(S, 51 + 32, 52 + 32, 53 + 32);
            Entity.AddTriangle(S, 52 + 32, 53 + 32, 54 + 32);
            Entity.AddTriangle(S, 55 + 32, 56 + 32, 57 + 32);
            Entity.AddTriangle(S, 56 + 32, 57 + 32, 58 + 32);
            Entity.AddTriangle(S, 59 + 32, 60 + 32, 61 + 32);
            Entity.AddTriangle(S, 60 + 32, 61 + 32, 62 + 32);
            Entity.AddTriangle(S, 63 + 32, 64 + 32, 65 + 32);
            Entity.AddTriangle(S, 64 + 32, 65 + 32, 66 + 32);



            Mesh.UpdateHardwareBuffers();

            Mesh.Shader = Program.TransformEditorShader;
            #endregion
        }
    }
}
