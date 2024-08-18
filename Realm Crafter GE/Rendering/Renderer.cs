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
// Wrapper for BBDX renderer by Rob W (rottbott@hotmail.com)
// Written November 2006

using System;
using System.Collections.Generic;

namespace RenderingServices
{
    // OOP wrapper around an unmanaged 3D entity
    public class Entity
    {
        // List of all entities
        public static LinkedList<Entity> EntitiesList = new LinkedList<Entity>();

        // Unmanaged handle used by the DLL
        public uint Handle;
        public ushort ID;
        public byte IsAnim = 0;
        public byte Flags = 0;

        public Dictionary<string, object> Parameters = new Dictionary<string, object>();

        // Creation
        public Entity()
        {
            Vis = true;
            Handle = 0;
            ID = 65535;
            EntitiesList.AddLast(this);
            MeshLOD_Low = MeshLOD_Medium = 65535;
        }

        public static void ResendAllParameters()
        {
            foreach (Entity E in EntitiesList)
            {
                E.ResendParameters();
            }
        }

        public void ResendParameters()
        {
            foreach (KeyValuePair<string, object> Kp in Parameters)
            {
                string Name = Kp.Key;
                object Value = Kp.Value;

                if (Value is Vector1)
                {
                    RenderingServices.RenderWrapper.EntityConstantFloat(Handle, Name, (Value as Vector1).X);
                }
                if (Value is Vector2)
                {
                    RenderingServices.RenderWrapper.EntityConstantFloat2(Handle, Name, (Value as Vector2).X,
                                                                         (Value as Vector2).Y);
                }
                if (Value is Vector3)
                {
                    RenderingServices.RenderWrapper.EntityConstantFloat3(Handle, Name, (Value as Vector3).X,
                                                                         (Value as Vector3).Y, (Value as Vector3).Z);
                }
                if (Value is Vector4)
                {
                    RenderingServices.RenderWrapper.EntityConstantFloat4(Handle, Name, (Value as Vector4).X,
                                                                         (Value as Vector4).Y, (Value as Vector4).Z,
                                                                         (Value as Vector4).W);
                }
            }
        }

        public static Entity CreateCamera(Entity Parent)
        {
            Entity E = new Entity();
            if (Parent != null)
            {
                E.Handle = RenderWrapper.burryjimpol(Parent.Handle);
            }
            else
            {
                E.Handle = RenderWrapper.burryjimpol(0);
            }
            return E;
        }

        public static Entity CreateCamera()
        {
            return CreateCamera(null);
        }

        public static Entity CreatePivot(Entity Parent)
        {
            Entity E = new Entity();
            if (Parent != null)
            {
                E.Handle = RenderWrapper.carjacking(Parent.Handle);
            }
            else
            {
                E.Handle = RenderWrapper.carjacking(0);
            }
            return E;
        }

        public static Entity FromHandle(uint handle)
        {
            if (handle == 0)
                return null;

            Entity Out = new Entity();
            Out.Handle = handle;

            return Out;
        }

        public static Entity CreatePivot()
        {
            return CreatePivot(null);
        }

        public static Entity CreateMesh(Entity Parent)
        {
            Entity E = new Entity();
            if (Parent != null)
            {
                E.Handle = RenderWrapper.pathlier(Parent.Handle);
            }
            else
            {
                E.Handle = RenderWrapper.pathlier(0);
            }
            return E;
        }

        public static Entity CreateMesh()
        {
            return CreateMesh(null);
        }

        public static Entity CreateTerrain(int GridSize, Entity Parent)
        {
            Entity E = new Entity();
            if (Parent != null)
            {
                E.Handle = RenderWrapper.pandamaka(GridSize, Parent.Handle);
            }
            else
            {
                E.Handle = RenderWrapper.pandamaka(GridSize, 0);
            }
            //E.Shader = Shaders.LitObject2;
            return E;
        }

        public static Entity CreateTerrain(int GridSize)
        {
            return CreateTerrain(GridSize, null);
        }

        public static Entity LoadMesh(string Filename, Entity Parent, bool Animated)
        {
            // Check file exists
            if (!System.IO.File.Exists(Filename))
            {
                return null;
            }

            // Attempt to load
            uint Handle;
            if (Parent != null)
            {
                Handle = RenderWrapper.jalkming(Filename, Parent.Handle, Animated ? 1 : 0);
            }
            else
            {
                Handle = RenderWrapper.jalkming(Filename, 0, Animated ? 1 : 0);
            }
            if (Handle != 0)
            {
                Entity E = new Entity();
                E.Handle = Handle;
                E.Shader = Shaders.LitObject1;
                return E;
            }
            else
            {
                return null;
            }
        }

        public static Entity LoadMesh(string Filename)
        {
            return LoadMesh(Filename, null, false);
        }

        public static Entity CreateSAQuad(Entity Parent)
        {
            uint Handle = 0;
            if (Parent != null)
            {
                Handle = RenderWrapper.jesusownzall(Parent.Handle, Shaders.SAQuad);
            }
            else
            {
                Handle = RenderWrapper.jesusownzall(0, Shaders.SAQuad);
            }
            if (Handle != 0)
            {
                Entity E = new Entity();
                E.Handle = Handle;
                E.Order = -1;
                return E;
            }
            return null;
        }

        public static Entity CreateSAQuad()
        {
            return CreateSAQuad(null);
        }

        public static Entity CreateCube(Entity Parent)
        {
            uint Handle;
            if (Parent != null)
            {
                Handle = RenderWrapper.catonbox(CubeTemplate, Parent.Handle);
            }
            else
            {
                Handle = RenderWrapper.catonbox(CubeTemplate, 0);
            }
            if (Handle == 0)
            {
                return null;
            }
            RenderWrapper.fluxcapa(Handle);
            Entity E = new Entity();
            E.Handle = Handle;
            return E;
        }

        public static Entity CreateCube()
        {
            return CreateCube(null);
        }

        public static Entity CreateSphere(Entity Parent)
        {
            uint Handle;
            if (Parent != null)
            {
                Handle = RenderWrapper.catonbox(SphereTemplate, Parent.Handle);
            }
            else
            {
                Handle = RenderWrapper.catonbox(SphereTemplate, 0);
            }
            if (Handle == 0)
            {
                return null;
            }
            RenderWrapper.fluxcapa(Handle);
            Entity E = new Entity();
            E.Handle = Handle;
            return E;
        }

        public static Entity CreateSphere()
        {
            return CreateSphere(null);
        }

        public Entity Copy(Entity Parent)
        {
            Entity E = new Entity();
            if (Parent != null)
            {
                E.Handle = RenderWrapper.catonbox(Handle, Parent.Handle);
            }
            else
            {
                E.Handle = RenderWrapper.catonbox(Handle, 0);
            }
            return E;
        }

        public Entity Copy()
        {
            return Copy(null);
        }

        // Sphere template mesh
        private static uint SphereTemplate, CubeTemplate;

        public static bool LoadSphereMesh(string Filename)
        {
            if (!System.IO.File.Exists(Filename))
            {
                return false;
            }
            SphereTemplate = RenderWrapper.jalkming(Filename, 0, 0);
            if (SphereTemplate == 0)
            {
                return false;
            }
            RenderWrapper.bumofcow(SphereTemplate);
            return true;
        }

        public static bool LoadCubeMesh(string Filename)
        {
            if (!System.IO.File.Exists(Filename))
            {
                return false;
            }
            CubeTemplate = RenderWrapper.jalkming(Filename, 0, 0);
            if (CubeTemplate == 0)
            {
                return false;
            }
            RenderWrapper.bumofcow(CubeTemplate);
            return true;
        }

        // Free entity
        public void Free()
        {
            EntitiesList.Remove(this);
            if(Handle != 0)
                RenderWrapper.getmeshake(Handle);
            Handle = 0;
        }

        // Mesh building
        public float MeshWidth
        {
            get { return RenderWrapper.manonworld(Handle); }
        }

        public float MeshHeight
        {
            get { return RenderWrapper.jumpin(Handle); }
        }

        public float MeshDepth
        {
            get { return RenderWrapper.needsleep(Handle); }
        }

        public int RenderMask
        {
            get { return RenderWrapper.bbdx2_GetEntityRenderMask(Handle); }
            set { RenderWrapper.bbdx2_SetEntityRenderMask(Handle, value); }
        }

        public void PositionMesh(float X, float Y, float Z)
        {
            RenderWrapper.softskin(Handle, X, Y, Z);
        }

        public void RotateMesh(float Pitch, float Yaw, float Roll)
        {
            RenderWrapper.setmedown(Handle, Pitch, Yaw, Roll);
        }

        public void ScaleMesh(float X, float Y, float Z)
        {
            RenderWrapper.soreneck(Handle, X, Y, Z);
        }

        public void FitMesh(float X, float Y, float Z, float Width, float Height, float Depth, bool Uniform)
        {
            RenderWrapper.reachout(Handle, X, Y, Z, Width, Height, Depth, Uniform);
        }

        public uint GetSurface(int Index)
        {
            return RenderWrapper.getupgetout(Handle, Index);
        }

        public uint CreateSurface()
        {
            return RenderWrapper.andtakeit(Handle);
        }

        public static int ClearSurface(uint Surface)
        {
            return RenderWrapper.wannabe(Surface);
        }

        public static int AddVertex(uint Surface, float X, float Y, float Z, float U, float V)
        {
            return RenderWrapper.withoutyou(Surface, X, Y, Z, U, V, 0f);
        }

        public static int AddVertex(uint Surface, float X, float Y, float Z)
        {
            return RenderWrapper.withoutyou(Surface, X, Y, Z, 0f, 0f, 0f);
        }

        public static int AddTriangle(uint Surface, int Vertex1, int Vertex2, int Vertex3)
        {
            return RenderWrapper.tellme(Surface, Vertex1, Vertex2, Vertex3);
        }

        public string SurfaceTexture(int SurfaceIndex, int TextureIndex)
        {
            return RenderWrapper.TexNameFromSurf(Handle, TextureIndex, SurfaceIndex);
        }

        public int CountSurfaces()
        {
            return RenderWrapper.openeyes(Handle);
        }

        public static int CountVertices(uint Surface)
        {
            return RenderWrapper.aboveandbeyond(Surface);
        }

        public static float VertexX(uint Surface, int Index)
        {
            return RenderWrapper.laptop(Surface, Index);
        }

        public static float VertexY(uint Surface, int Index)
        {
            return RenderWrapper.watchingspace(Surface, Index);
        }

        public static float VertexZ(uint Surface, int Index)
        {
            return RenderWrapper.garagedoor(Surface, Index);
        }

        public static void VertexCoords(uint Surface, int Index, float X, float Y, float Z)
        {
            RenderWrapper.waverider(Surface, Index, X, Y, Z);
        }

        public static void VertexTexCoords(uint Surface, int Index, float U, float V)
        {
            RenderWrapper.happyclap(Surface, Index, U, V);
        }

        public static void VertexColor(uint Surface, int Index, int Red, int Green, int Blue, float Alpha)
        {
            RenderWrapper.urallnoobs(Surface, Index, Red, Green, Blue, (int) Alpha);
        }

        public void UpdateHardwareBuffers()
        {
            RenderWrapper.jockgnome(Handle);
        }

        public void UpdateNormals()
        {
            RenderWrapper.eveclassic(Handle);
        }

        // Camera settings
        public void CameraRange(float Min, float Max)
        {
            RenderWrapper.makemegame(Handle, Min, Max);
        }

        public void CameraClsColor(int Red, int Green, int Blue)
        {
            RenderWrapper.bbdx2_CameraClsColorAlpha(Handle, Red, Green, Blue, 0);
        }

        // Screen-aligned quad settings
        public void SAQuadLayout(float X, float Y, float Width, float Height)
        {
            RenderWrapper.hardrock(Handle, X, Y, Width, Height);
        }

        public void SAQuadColor(int Red, int Green, int Blue)
        {
            RenderWrapper.makemav(Handle, Red, Green, Blue);
        }

        public void SAQuadAlpha(float Alpha)
        {
            RenderWrapper.shogun(Handle, Alpha);
        }

        // Terrain settings
        public void ModifyTerrain(int X, int Z, float Height)
        {
            RenderWrapper.laggysven(Handle, X, Z, Height);
        }

        public void UpdateTerrain()
        {
            RenderWrapper.tikamakis(Handle);
        }

        public float TerrainHeight(int X, int Z)
        {
            return RenderWrapper.trexban(Handle, X, Z);
        }

        public int TerrainSize()
        {
            return RenderWrapper.jacklingmo(Handle);
        }

        // Transformation
        public void Position(float X, float Y, float Z)
        {
            RenderWrapper.chaosdigs(Handle, X, Y, Z, false);
        }

        public void Position(float X, float Y, float Z, bool Global)
        {
            RenderWrapper.chaosdigs(Handle, X, Y, Z, Global);
        }

        public void Translate(float X, float Y, float Z)
        {
            RenderWrapper.rzrtool(Handle, X, Y, Z, false);
        }

        public void Translate(float X, float Y, float Z, bool Global)
        {
            RenderWrapper.rzrtool(Handle, X, Y, Z, Global);
        }

        public void Move(float X, float Y, float Z)
        {
            RenderWrapper.gobstoper(Handle, X, Y, Z);
        }

        public void Rotate(float Pitch, float Yaw, float Roll)
        {
            RenderWrapper.mingja(Handle, Pitch, Yaw, Roll, false);
        }

        public void Rotate(float Pitch, float Yaw, float Roll, bool Global)
        {
            RenderWrapper.mingja(Handle, Pitch, Yaw, Roll, Global);
        }

        public void Turn(float Pitch, float Yaw, float Roll)
        {
            RenderWrapper.moolad(Handle, Pitch, Yaw, Roll, false);
        }

        public void Turn(float Pitch, float Yaw, float Roll, bool Global)
        {
            RenderWrapper.moolad(Handle, Pitch, Yaw, Roll, Global);
        }

        public void Scale(float X, float Y, float Z)
        {
            RenderWrapper.jaffamak(Handle, X, Y, Z, false);
        }

        public void Scale(float X, float Y, float Z, bool Global)
        {
            RenderWrapper.jaffamak(Handle, X, Y, Z, Global);
        }

        public void ScaleRelative(float X, float Y, float Z)
        {
            ScaleRelative(X, Y, Z, false);
        }

        public void ScaleRelative(float X, float Y, float Z, bool Global)
        {
            X *= RenderWrapper.tcpwnsall(Handle, Global);
            Y *= RenderWrapper.habaki(Handle, Global);
            Z *= RenderWrapper.kimono(Handle, Global);
            if (X < 0.000006f)
            {
                X = 0.000006f;
            }
            if (Y < 0.000006f)
            {
                Y = 0.000006f;
            }
            if (Z < 0.000006f)
            {
                Z = 0.000006f;
            }
            RenderWrapper.jaffamak(Handle, X, Y, Z, Global);
        }

        public float X(bool Global)
        {
            return RenderWrapper.jingsu(Handle, Global);
        }

        public float X()
        {
            return RenderWrapper.jingsu(Handle, false);
        }

        public float Y(bool Global)
        {
            return RenderWrapper.sostrong(Handle, Global);
        }

        public float Y()
        {
            return RenderWrapper.sostrong(Handle, false);
        }

        public float Z(bool Global)
        {
            return RenderWrapper.kisheadgone(Handle, Global);
        }

        public float Z()
        {
            return RenderWrapper.kisheadgone(Handle, false);
        }

        public float Pitch(bool Global)
        {
            return RenderWrapper.kissme(Handle, Global);
        }

        public float Pitch()
        {
            return RenderWrapper.kissme(Handle, false);
        }

        public float Yaw(bool Global)
        {
            return RenderWrapper.lolatme(Handle, Global);
        }

        public float Yaw()
        {
            return RenderWrapper.lolatme(Handle, false);
        }

        public float Roll(bool Global)
        {
            return RenderWrapper.lambdin(Handle, Global);
        }

        public float Roll()
        {
            return RenderWrapper.lambdin(Handle, false);
        }

        public float ScaleX(bool Global)
        {
            return RenderWrapper.tcpwnsall(Handle, Global);
        }

        public float ScaleX()
        {
            return RenderWrapper.tcpwnsall(Handle, false);
        }

        public float ScaleY(bool Global)
        {
            return RenderWrapper.habaki(Handle, Global);
        }

        public float ScaleY()
        {
            return RenderWrapper.habaki(Handle, false);
        }

        public float ScaleZ(bool Global)
        {
            return RenderWrapper.kimono(Handle, Global);
        }

        public float ScaleZ()
        {
            return RenderWrapper.kimono(Handle, false);
        }

        public void AlignToVector(float VectorX, float VectorY, float VectorZ, int Axis)
        {
            RenderWrapper.chinook(Handle, VectorX, VectorY, VectorZ, Axis);
        }

        public void Point(Entity Target)
        {
            RenderWrapper.babygoo(Handle, Target.Handle, 0f);
        }

        public void Point(Entity Target, float Roll)
        {
            RenderWrapper.babygoo(Handle, Target.Handle, Roll);
        }

        // Visual
        private bool Vis;

        public bool Visible
        {
            get { return Vis; }
            set
            {
                Vis = value;
                if (Vis)
                {
                    RenderWrapper.fluxcapa(Handle);
                }
                else
                {
                    RenderWrapper.bumofcow(Handle);
                }
            }
        }

        public bool InheritAnimation
        {
            get
            {
                if (Handle == 0)
                    return false;

                return RenderWrapper.bbdx2_GetInheritAnimation(Handle) > 0;
            }
            set
            {
                if (Handle != 0)
                    RenderWrapper.bbdx2_SetInheritAnimation(Handle, value ? 1 : 0);
            }
        }

        public int FX
        {
            set { RenderWrapper.localiva(Handle, value); }
        }

        public void Alpha(float Alpha)
        {
            RenderWrapper.missedit(Handle, Alpha);
        }

        public void AlphaNoSolid(float Alpha)
        {
            RenderWrapper.makewow(Handle, Alpha);
        }

        public uint Shader
        {
            set { RenderWrapper.slobing(Handle, value); }
        }

        public void Color(int Red, int Green, int Blue)
        {
            RenderWrapper.kickleaves(Handle, Red, Green, Blue);
        }

        public void Texture(uint Texture)
        {
            RenderWrapper.smokinhot(Handle, Texture, 0, 0, -1);
        }

        public void Texture(uint Texture, int Index)
        {
            RenderWrapper.smokinhot(Handle, Texture, 0, Index, -1);
        }

        public void Texture(uint Texture, int Index, int Surface)
        {
            RenderWrapper.smokinhot(Handle, Texture, 0, Index, Surface);
        }

        public int Order
        {
            set { RenderWrapper.chincrank(Handle, value); }
        }

        public bool AlphaState
        {
            get { return RenderWrapper.GetAlphaState(Handle); }
            set { RenderWrapper.SetAlphaState(Handle, value); }
        }

        public bool InView
        {
            get { return RenderWrapper.EntityInView(Handle); }
        }

        // Animation
        public void Animate(int Mode, float Speed, int Sequence)
        {
            RenderWrapper.gisranlo(Handle, Mode, Speed, Sequence);
        }

        public void Animate(int Mode, float Speed)
        {
            RenderWrapper.gisranlo(Handle, Mode, Speed, 0);
        }

        public void Animate(int Mode)
        {
            RenderWrapper.gisranlo(Handle, Mode, 1f, 0);
        }

        public void Animate()
        {
            RenderWrapper.gisranlo(Handle, 1, 1f, 0);
        }

        public int ExtractAnimSeq(int StartFrame, int EndFrame)
        {
            return RenderWrapper.growth(Handle, StartFrame, EndFrame);
        }

        // Level of Detail
        public System.Single _distLOD_High, _distLOD_Medium, _distLOD_Low;
        public ushort _MeshLOD_Medium = 65535, _MeshLOD_Low = 65535;

        public float distLOD_High
        {
            get { return _distLOD_High; }
            set
            {
                _distLOD_High = value;

                if (Handle == 0)
                    return;

                RenderWrapper.closeeyes(Handle, 0, 0, _distLOD_High);
            }
        }

        public float distLOD_Medium
        {
            get { return _distLOD_Medium; }
            set
            {
                _distLOD_Medium = value;

                if (Handle == 0)
                    return;

                uint iLOD = 0;

                if (_MeshLOD_Medium != 65535)
                {
                    string MeshPath = RealmCrafter.Media.GetMeshName(_MeshLOD_Medium, false);
                    iLOD = RenderWrapper.jalkming(@"Data\Meshes\" + MeshPath, 0, 0);
                }

                RenderWrapper.closeeyes(Handle, iLOD, 1, _distLOD_Medium);

                if (iLOD != 0)
                    RenderWrapper.getmeshake(iLOD);
            }
        }

        public float distLOD_Low
        {
            get { return _distLOD_Low; }
            set
            {
                _distLOD_Low = value;

                if (Handle == 0)
                    return;

                uint iLOD = 0;

                if (_MeshLOD_Low != 65535)
                {
                    string MeshPath = RealmCrafter.Media.GetMeshName(_MeshLOD_Low, false);
                    iLOD = RenderWrapper.jalkming(@"Data\Meshes\" + MeshPath, 0, 0);
                }

                RenderWrapper.closeeyes(Handle, iLOD, 2, _distLOD_Low);

                if (iLOD != 0)
                    RenderWrapper.getmeshake(iLOD);
            }
        }

        public ushort MeshLOD_Medium
        {
            get { return _MeshLOD_Medium; }
            set
            {
                _MeshLOD_Medium = value;

                if (Handle == 0)
                    return;

                uint iLOD = 0;

                if (_MeshLOD_Medium != 65535)
                {
                    string MeshPath = RealmCrafter.Media.GetMeshName(_MeshLOD_Medium, false);
                    iLOD = RenderWrapper.jalkming(@"Data\Meshes\" + MeshPath, 0, 0);
                }

                RenderWrapper.closeeyes(Handle, iLOD, 1, _distLOD_Medium);

                if (iLOD != 0)
                    RenderWrapper.getmeshake(iLOD);
            }
        }

        public ushort MeshLOD_Low
        {
            get { return _MeshLOD_Low; }
            set
            {
                _MeshLOD_Low = value;

                if (Handle == 0)
                    return;

                uint iLOD = 0;

                if (_MeshLOD_Low != 65535)
                {
                    string MeshPath = RealmCrafter.Media.GetMeshName(_MeshLOD_Low, false);
                    iLOD = RenderWrapper.jalkming(@"Data\Meshes\" + MeshPath, 0, 0);
                }

                RenderWrapper.closeeyes(Handle, iLOD, 2, _distLOD_Low);

                if (iLOD != 0)
                    RenderWrapper.getmeshake(iLOD);
            }
        }

        // Hierarchy
        public int CountChildren()
        {
            return RenderWrapper.mindless(Handle);
        }

        public Entity GetChild(int Index)
        {
            // If child already exists as an Entity object, return that
            Entity E;
            uint ChildHandle = RenderWrapper.penondesk(Handle, Index);
            if (ChildHandle == 0)
            {
                return null;
            }
            LinkedListNode<Entity> Node = EntitiesList.First;
            while (Node != null)
            {
                if (Node.Value.Handle == ChildHandle)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }

            // Otherwise create a new Entity object
            E = new Entity();
            E.Handle = ChildHandle;
            return E;
        }

        public Entity FindChild(string Name)
        {
            // If child already exists as an Entity object, return that
            uint ChildHandle = RenderWrapper.rendering(Handle, Name);
            if (ChildHandle == 0)
            {
                return null;
            }
            Entity E;
            LinkedListNode<Entity> Node = EntitiesList.First;
            while (Node != null)
            {
                if (Node.Value.Handle == ChildHandle)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }

            // Otherwise create a new Entity object
            E = new Entity();
            E.Handle = ChildHandle;
            return E;
        }

        public Entity GetParent()
        {
            // If parent already exists as an Entity object, return that
            Entity E;
            uint ParentHandle = RenderWrapper.earthiswarm(Handle);
            if (ParentHandle == 0)
            {
                return null;
            }
            LinkedListNode<Entity> Node = EntitiesList.First;
            while (Node != null)
            {
                if (Node.Value.Handle == ParentHandle)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }

            // Otherwise create a new Entity object
            E = new Entity();
            E.Handle = ParentHandle;
            return E;
        }

        public void Parent(Entity Parent, bool Global)
        {
            if (Parent != null)
            {
                RenderWrapper.crystalclear(Handle, Parent.Handle, Global);
            }
            else
            {
                RenderWrapper.crystalclear(Handle, 0, Global);
            }
        }

        // Other
        public string Name
        {
            get { return RenderWrapper.likeyoumean(Handle); }
            set { RenderWrapper.commands(Handle, value); }
        }

        public string Tag
        {
            get { return RenderWrapper.ocones(Handle); }
            set { RenderWrapper.cones(Handle, value); }
        }

        public object ExtraData;

        public string Class
        {
            get { return RenderWrapper.setthefire(Handle).ToUpper(); }
        }

        public float Distance(Entity Other)
        {
            return (float)Math.Sqrt(RenderWrapper.nottdos(Handle, Other.Handle));
        }

        // TForm functions
        public static void TFormVector(float X, float Y, float Z, Entity Source, Entity Destination)
        {
            if (Source != null)
            {
                if (Destination != null)
                {
                    RenderWrapper.helplost(X, Y, Z, Source.Handle, Destination.Handle);
                }
                else
                {
                    RenderWrapper.helplost(X, Y, Z, Source.Handle, 0);
                }
            }
            else
            {
                if (Destination != null)
                {
                    RenderWrapper.helplost(X, Y, Z, 0, Destination.Handle);
                }
                else
                {
                    RenderWrapper.helplost(X, Y, Z, 0, 0);
                }
            }
        }

        public static void TFormPoint(float X, float Y, float Z, Entity Source, Entity Destination)
        {
            if (Source != null)
            {
                if (Destination != null)
                {
                    RenderWrapper.kieransan(X, Y, Z, Source.Handle, Destination.Handle);
                }
                else
                {
                    RenderWrapper.kieransan(X, Y, Z, Source.Handle, 0);
                }
            }
            else
            {
                if (Destination != null)
                {
                    RenderWrapper.kieransan(X, Y, Z, 0, Destination.Handle);
                }
                else
                {
                    RenderWrapper.kieransan(X, Y, Z, 0, 0);
                }
            }
        }

        public static float TFormedX()
        {
            return RenderWrapper.itsnoteasy();
        }

        public static float TFormedY()
        {
            return RenderWrapper.buymebeer();
        }

        public static float TFormedZ()
        {
            return RenderWrapper.firstdance();
        }

        public void Project()
        {
            RenderWrapper.EntityProject(Handle);
        }

        public static void PointProject(float X, float Y, float Z)
        {
            RenderWrapper.PointProject(X, Y, Z);
        }

        public static float ProjectedX()
        {
            return RenderWrapper.ProjectedX();
        }

        public static float ProjectedY()
        {
            return RenderWrapper.ProjectedY();
        }

        public static float ProjectedZ()
        {
            return RenderWrapper.ProjectedZ();
        }

        // ToString override returns entity class
        public override string ToString()
        {
            return Class;
        }
    }

    // OOP wrapper around an unmanaged 3D line
    public class Line3D : IDisposable
    {
        private static LinkedList<Line3D> LinesList = new LinkedList<Line3D>();
        private uint Handle;
        private bool GridLine;

        // Show/hide
        public bool Visible
        {
            set { RenderWrapper.SetLineVisible(Handle, value); }
        }

        // Constructor
        public Line3D(float StartX, float StartY, float StartZ, float EndX, float EndY, float EndZ, bool gridLine)
        {
            Setup(StartX, StartY, StartZ, EndX, EndY, EndZ, null, true, gridLine);
        }

        public Line3D(float StartX, float StartY, float StartZ, float EndX, float EndY, float EndZ, Entity Parent, bool gridLine)
        {
            Setup(StartX, StartY, StartZ, EndX, EndY, EndZ, Parent, true, gridLine);
        }

        public Line3D(float StartX, float StartY, float StartZ, float EndX, float EndY, float EndZ, bool Register, bool gridLine)
        {
            Setup(StartX, StartY, StartZ, EndX, EndY, EndZ, null, Register, gridLine);
        }

        public Line3D(float StartX, float StartY, float StartZ, float EndX, float EndY, float EndZ, Entity Parent,
                      bool Register, bool gridLine)
        {
            Setup(StartX, StartY, StartZ, EndX, EndY, EndZ, Parent, Register, gridLine);
        }

        private void Setup(float StartX, float StartY, float StartZ, float EndX, float EndY, float EndZ, Entity Parent,
                           bool Register, bool gridLine)
        {
            GridLine = gridLine;
            Handle = RenderWrapper.CreateLine((Parent != null) ? Parent.Handle : 0);
            RenderWrapper.slobing(Handle, Shaders.Line);
            SetPositions(StartX, StartY, StartZ, EndX, EndY, EndZ);

            if (Register)
            {
                LinesList.AddLast(this);
            }
        }

        // Sets the start/end positions of this line
        public void SetPositions(float StartX, float StartY, float StartZ, float EndX, float EndY, float EndZ)
        {
            RenderWrapper.SetLineSize(Handle, StartX, StartY, StartZ, EndX, EndY, EndZ);
        }

        // Sets the colour of this line
        public void SetColor(byte R, byte G, byte B)
        {
            RenderWrapper.SetLineColor(Handle, R, G, B);
        }

        // Frees the unmanaged entity
        void IDisposable.Dispose()
        {
            if (Handle != 0)
            {
                RenderWrapper.FreeLine(Handle);
                Handle = 0;
            }
            LinesList.Remove(this);
        }

        ~Line3D()
        {
            if (Handle != 0)
            {
                RenderWrapper.FreeLine(Handle);
                Handle = 0;
            }
            //LinesList.Remove(this);
        }

        public void Free()
        {
            if (Handle != 0)
            {
                RenderWrapper.FreeLine(Handle);
                Handle = 0;
            }
            LinesList.Remove(this);
        }

        // Free all existing lines
        public static void FreeAll()
        {
            LinkedListNode<Line3D> Line = LinesList.First;
            while (Line != null)
            {
                RenderWrapper.FreeLine(Line.Value.Handle);
                Line.Value.Handle = 0;
                Line = Line.Next;
            }
            LinesList.Clear();
        }

        // Hide/show all existing lines
        public static void SetAllVisible(bool Visible, bool gridOnly)
        {
            LinkedListNode<Line3D> Line = LinesList.First;
            while (Line != null)
            {
                if (!gridOnly || Line.Value.GridLine)
                {
                    Line.Value.Visible = Visible;
                }
                    Line = Line.Next;
            }
        }
    }

    // Shaders
    public static class Shaders
    {
        // Default shaders
        public static uint Animated,
                           LitObject1,
                           /*LitObject2,*/ Terrain,
                           FullbrightAdd,
                           FullbrightMultiply,
                           FullbrightAlpha,
                           SAQuad,
                           Line,
                           Sky,
                           Clouds,
                           WireFrame,
                           DefaultAnimatedDepthShader,
                           DefaultDepthShader,
                           ShadowBlurV,
                           ShadowBlurH;

        // Load a shader and return the handle
        public static uint Load(string Filename)
        {
            return RenderWrapper.deterkis(Filename);
        }
    }

    // OOP wrapper around an unmanaged light
    public class Light
    {
        // Light info
        public int Handle;
        private int Type;

        public override string ToString()
        {
            if (Type == 0)
            {
                return "POINT LIGHT";
            }
            return "DIRECTIONAL LIGHT";
        }

        // Constructors
        private Light()
        {
        }

        public static Light CreatePointLight()
        {
            Light L = new Light();
            L.Handle = RenderWrapper.copliy();
            L.Type = 0;
            return L;
        }

        public static Light CreateDirectionalLight()
        {
            Light L = new Light();
            L.Handle = RenderWrapper.boompx();
            L.Type = 1;
            return L;
        }

        // Free
        public void Free()
        {
            if (Type == 0)
            {
                RenderWrapper.lipphogg(Handle);
            }
            else
            {
                RenderWrapper.jewnjig(Handle);
            }
        }

        // Settings
        public void Position(float X, float Y, float Z)
        {
            RenderWrapper.lovepixels(Handle, X, Y, Z);
        }

        public void Direction(float X, float Y, float Z)
        {
            RenderWrapper.sukusul(Handle, X, Y, Z);
        }

        public void Radius(float Radius)
        {
            RenderWrapper.gabbama(Handle, Radius);
        }

        public void Color(int Red, int Green, int Blue)
        {
            if (Type == 0)
            {
                RenderWrapper.hoklig(Handle, Red, Green, Blue);
            }
            else
            {
                RenderWrapper.gonerum(Handle, Red, Green, Blue);
            }
        }

        public void Enable(int Enable)
        {
            if (Type == 0)
            {
                RenderWrapper.wareflog(Handle, Enable);
            }
            else
            {
                RenderWrapper.klopil(Handle, Enable);
            }
        }
    }

    // Collision/picking functions
    public class Collision
    {
        public static void Collisions(RealmCrafter.CollisionType SourceType, RealmCrafter.CollisionType DestinationType,
            int Method, int Response)
        {
            Collisions((int)SourceType, (int)DestinationType, Method, Response);
        }

        public static void Collisions(int SourceType, int DestinationType,
                                      int Method, int Response)
        {
            RenderWrapper.bbdx2_Collisions((int) SourceType, (int) DestinationType);
        }

        public static void EntityType(Entity Object, byte CollisionType)
        {
            RenderWrapper.bbdx2_EntityType(Object.Handle, CollisionType);
        }

        public static int EntityType(Entity Object)
        {
            return RenderWrapper.bbdx2_GetEntityType(Object.Handle);
        }

        public static void EntityRadius(Entity Object, float XRadius, float YRadius)
        {
            RenderWrapper.bbdx2_EntityRadius(Object.Handle, XRadius, YRadius);
        }

        public static void EntityBox(Entity Object, float X, float Y, float Z, float Width, float Height, float Depth)
        {
            //RenderWrapper.ir_entitybox(Object.Handle, X, Y, Z, Width, Height, Depth);
            RenderWrapper.bbdx2_EntityBox(Object.Handle, X, Y, Z, Width, Height, Depth);
        }

        public static void SetCollisionMesh(Entity Object)
        {
            RenderWrapper.bbdx2_SetCollisionMesh(Object.Handle);
        }

        public static void ResetEntity(Entity Object)
        {
            RenderWrapper.bbdx2_ResetEntity(Object.Handle);
        }

        public static int CountCollisions(Entity Object)
        {
            return RenderWrapper.bbdx2_CountCollisions(Object.Handle);
        }

        public static float CollisionX(Entity Object, int Index)
        {
            return RenderWrapper.bbdx2_CollisionX(Object.Handle, Index);
        }

        public static float CollisionY(Entity Object, int Index)
        {
            return RenderWrapper.bbdx2_CollisionY(Object.Handle, Index);
        }

        public static float CollisionZ(Entity Object, int Index)
        {
            return RenderWrapper.bbdx2_CollisionZ(Object.Handle, Index);
        }

        public static float CollisionNX(Entity Object, int Index)
        {
            return RenderWrapper.bbdx2_CollisionNX(Object.Handle, Index);
        }

        public static float CollisionNY(Entity Object, int Index)
        {
            return RenderWrapper.bbdx2_CollisionNY(Object.Handle, Index);
        }

        public static float CollisionNZ(Entity Object, int Index)
        {
            return RenderWrapper.bbdx2_CollisionNZ(Object.Handle, Index);
        }

        public static void EntityPickMode(Entity Object, PickMode Mode)
        {
            if (Object == null)
                return;

            RenderWrapper.bbdx2_EntityPickMode(Object.Handle, (int) Mode);
        }

        public static Entity LinePick(float X, float Y, float Z, float XDistance, float YDistance, float ZDistance,
                                      float Radius)
        {
            uint Result = RenderWrapper.bbdx2_LinePick(X, Y, Z, XDistance, YDistance, ZDistance);
            if (Result == 0)
            {
                return null;
            }

            LinkedListNode<Entity> Node = Entity.EntitiesList.First;
            while (Node != null)
            {
                if (Node.Value.Handle == Result)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }

            Entity E = new Entity();
            E.Handle = Result;
            return E;
        }

        public static Entity CameraPick(Entity Camera, int X, int Y)
        {
            uint Result = RenderWrapper.bbdx2_CameraPick(X, Y);
            if (Result == 0)
            {
                return null;
            }

            LinkedListNode<Entity> Node = Entity.EntitiesList.First;
            while (Node != null)
            {
                if (Node.Value.Handle == Result)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }

            Entity E = new Entity();
            E.Handle = Result;
            return E;
        }

        public static float PickedX()
        {
            return RenderWrapper.bbdx2_PickedX();
        }

        public static float PickedY()
        {
            return RenderWrapper.bbdx2_PickedY();
        }

        public static float PickedZ()
        {
            return RenderWrapper.bbdx2_PickedZ();
        }

        public static float PickedNX()
        {
            return RenderWrapper.bbdx2_PickedNX();
        }

        public static float PickedNY()
        {
            return RenderWrapper.bbdx2_PickedNY();
        }

        public static float PickedNZ()
        {
            return RenderWrapper.bbdx2_PickedNZ();
        }

        public static void UpdateWorld()
        {
            RenderWrapper.anyoneelse();
        }
    }

    // General functions
    public static class Render
    {
        public static void Init(IntPtr hWnd, uint HInstance)
        {
            RenderWrapper.ghoop(hWnd, HInstance);
        }

        public static void Graphics3D(int Width, int Height, int Depth, int Mode, int AA, int Anisotropy,
                                      string DefaultTexture)
        {
            RenderWrapper.ackno(Width, Height, Depth, Mode, AA, Anisotropy, DefaultTexture);
        }

        public static void EndGraphics()
        {
            RenderWrapper.lickme();
        }

        public static void RenderWorld()
        {
            RenderWrapper.laghimout();
        }

        public static uint LoadTexture(string Filename)
        {
            return RenderWrapper.texttodate(Filename, 1);
        }

        public static uint LoadTexture(string Filename, int Flags)
        {
            return RenderWrapper.texttodate(Filename, Flags);
        }

        public static void ScaleTexture(uint Texture, float X, float Y)
        {
            RenderWrapper.preening(Texture, X, Y);
        }

        public static uint CopyTexture(uint Texture)
        {
            return RenderWrapper.pawnbroker(Texture);
        }

        public static uint GrabTexture(uint Texture)
        {
            return RenderWrapper.GrabTexture(Texture);
        }

        public static void FreeTexture(uint Texture)
        {
            RenderWrapper.emokid(Texture);
        }

        public static void AmbientLight(int Red, int Green, int Blue)
        {
            RenderWrapper.qwedfy(Red, Green, Blue);

            if (RealmCrafter_GE.GE.TerrainManager != null)
                RealmCrafter_GE.GE.TerrainManager.SetAmbientLight(System.Drawing.Color.FromArgb(Red, Green, Blue));
            if (RealmCrafter_GE.Program.Manager != null)
                RealmCrafter_GE.Program.Manager.SetAmbientLight(System.Drawing.Color.FromArgb(Red, Green, Blue));
        }

        public static void FogMode(int Mode)
        {
            RenderWrapper.nofear(0, Mode);
        }

        public static void FogRange(float Min, float Max)
        {
            RenderWrapper.kevlarboobs(0, Min, Max);
        }

        public static void FogColor(int Red, int Green, int Blue)
        {
            RenderWrapper.goodoldkis(0, Red, Green, Blue);
        }

        public static string Convert3DSToB3D(string Filename)
        {
            byte[] Bank = new byte[2048];
            System.Text.Encoding Encoder = System.Text.Encoding.GetEncoding(850);
            byte[] Text = Encoder.GetBytes(Filename);
            Text.CopyTo(Bank, 0);
            RenderWrapper.pears(Bank);
            int TextLength = 0;
            for (int i = 0; i < 2048; ++i)
            {
                if (Bank[i] == 0)
                {
                    TextLength = i;
                    break;
                }
            }
            return Encoder.GetString(Bank, 0, TextLength);
        }

        public static string ConvertXToB3D(string Filename)
        {
            byte[] Bank = new byte[2048];
            System.Text.Encoding Encoder = System.Text.Encoding.GetEncoding(850);
            byte[] Text = Encoder.GetBytes(Filename);
            Text.CopyTo(Bank, 0);
            RenderWrapper.apples(Bank);
            int TextLength = 0;
            for (int i = 0; i < 2048; ++i)
            {
                if (Bank[i] == 0)
                {
                    TextLength = i;
                    break;
                }
            }
            return Encoder.GetString(Bank, 0, TextLength);
        }
    }
}