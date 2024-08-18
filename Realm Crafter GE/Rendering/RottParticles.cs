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
// RottParticles particle system by Rob W (rottbott@hotmail.com)
// Original version July 2004, C# port November 2006

using System;
using System.IO;
using System.Runtime.InteropServices;
using RenderingServices;
using RealmCrafter_GE;

namespace RottParticles
{
    // General particle system functions
    public static class General
    {
        // Create an emitter
        public static Entity CreateEmitter(EmitterConfig C, float Scale)
        {
            Emitter E = new Emitter(C, Scale);
            return E.EmitterEN;
        }

        public static Entity CreateEmitter(EmitterConfig C)
        {
            return CreateEmitter(C, 1f);
        }

        // Emitter control
        public static int EmitterActiveParticles(Entity EN)
        {
            Emitter E = EN.ExtraData as Emitter;
            if (E != null)
            {
                return E.ActiveParticles;
            }
            else
            {
                return -1;
            }
        }

        public static void EnableEmitter(Entity EN, bool Enable)
        {
            Emitter E = EN.ExtraData as Emitter;
            if (E != null)
            {
                E.Enabled = Enable;
                E.MeshEN.Visible = Enable;
            }
        }

        public static void ShowEmitter(Entity EN, bool Show)
        {
            Emitter E = EN.ExtraData as Emitter;
            if (E != null)
            {
                E.MeshEN.Visible = Show;
            }
        }

        public static void ScaleEmitter(Entity EN, float Scale)
        {
            Emitter E = EN.ExtraData as Emitter;
            if (E != null)
            {
                E.Scale = Scale;
            }
        }

        public static void KillEmitter(Entity EN, bool FreeConfig, bool FreeTex)
        {
            Emitter E = EN.ExtraData as Emitter;
            if (E != null)
            {
                E.Kill(FreeConfig, FreeTex);
            }
        }

        public static void FreeEmitter(Entity EN, bool FreeConfig, bool FreeTex)
        {
            Emitter E = EN.ExtraData as Emitter;
            if (E != null)
            {
                E.Free(FreeConfig, FreeTex);
            }
        }

        // Update the system
        public static void Update(float Delta)
        {
            // Update emitters
            Emitter E = Emitter.FirstEmitter;
            while (E != null)
            {
                E.Update(Delta);
                E = E.NextEmitter;
            }
        }

        public static void Update()
        {
            Update(1f);
        }

        // Clear all emitters and configs
        public static void Clear(bool FreeTextures)
        {
            Emitter E = Emitter.FirstEmitter;

            while (E != null)
            {
                E.Free(false, false);
                E = E.NextEmitter;
            }

            EmitterConfig EC = EmitterConfig.FirstConfig;
            while (EC != null)
            {
                EC.Free(FreeTextures);
                EC = EC.NextConfig;
            }
        }
    }

    // Invididual particle class
    public class Particle
    {
        // Settings
        public Emitter E;
        private int FirstVertex;
        public float Scale, X, Y, Z;
        public float VX, VY, VZ;
        public float FX, FY, FZ;
        public float R, G, B, A;
        private float TimeToLive;
        public bool InUse;
        public int TexFrame;
        public float TexChange;

        // Linked list
        public Particle NextParticle;

        // Spawn this particle
        public void Spawn()
        {
            uint Surf = E.MeshEN.GetSurface(1);
            InUse = true;
            E.ActiveParticles++;

            // Initial forces
            FX = E.Config.ForceX;
            FY = E.Config.ForceY;
            FZ = E.Config.ForceZ;

            // Unshaped initial velocities
            VX = E.Config.VelocityX + E.Rand(-E.Config.VelocityRndX, E.Config.VelocityRndX);
            VY = E.Config.VelocityY + E.Rand(-E.Config.VelocityRndY, E.Config.VelocityRndY);
            VZ = E.Config.VelocityZ + E.Rand(-E.Config.VelocityRndZ, E.Config.VelocityRndZ);

            // Shape initial position (and velocities if required)
            float Dist, FDist, Height, Pitch, Yaw;
            switch (E.Config.Shape)
            {
                case EmitterShape.Sphere:
                    // Position
                    Dist = (float) E.Rand(E.Config.MinRadius, E.Config.MaxRadius) * E.Scale;
                    Pitch = E.Rand(-90f, 90f);
                    Yaw = E.Rand(-180f, 180f);
                    Y = (float) (Blitz.Sin(Pitch)) * Dist;
                    FDist = (float) (Blitz.Cos(Pitch)) * Dist;
                    X = (float) (Blitz.Cos(Yaw)) * FDist;
                    Z = (float) (Blitz.Sin(Yaw)) * FDist;

                    // Velocity
                    if (E.Config.VShape == VelocityShape.Shaped)
                    {
                        VX = Math.Abs(VX) * Math.Sign((float) (Blitz.Cos(Yaw)) * FDist);
                        VY = Math.Abs(VY) * Math.Sign((float) (Blitz.Sin(Pitch)) * Dist);
                        VZ = Math.Abs(VZ) * Math.Sign((float) (Blitz.Sin(Yaw)) * FDist);
                    }
                    else if (E.Config.VShape == VelocityShape.HeavilyShaped)
                    {
                        VX = Math.Abs(VX) * (((float) (Blitz.Cos(Yaw)) * FDist) / E.Config.MaxRadius);
                        VY = Math.Abs(VY) * (((float) (Blitz.Sin(Pitch)) * Dist) / E.Config.MaxRadius);
                        VZ = Math.Abs(VZ) * (((float) (Blitz.Sin(Yaw)) * FDist) / E.Config.MaxRadius);
                    }

                    break;
                case EmitterShape.Cylinder:
                    // Position
                    Dist = E.Rand(E.Config.MinRadius, E.Config.MaxRadius) * E.Scale;
                    Yaw = E.Rand(-180f, 180f);
                    Height = E.Rand(E.Config.Depth * -0.5f, E.Config.Depth * 0.5f) * E.Scale;
                    switch (E.Config.ShapeAxis)
                    {
                            // Cylinder lies along X axis
                        case 1:
                            X = Height;
                            Y = (float) (Blitz.Cos(Yaw)) * Dist;
                            Z = (float) (Blitz.Sin(Yaw)) * Dist;
                            break;
                            // Cylinder lies along Y axis
                        case 2:
                            Y = Height;
                            X = (float) (Blitz.Cos(Yaw)) * Dist;
                            Z = (float) (Blitz.Sin(Yaw)) * Dist;
                            break;
                            // Cylinder lies along Z axis
                        case 3:
                            Z = Height;
                            X = (float) (Blitz.Cos(Yaw)) * Dist;
                            Y = (float) (Blitz.Sin(Yaw)) * Dist;
                            break;
                    }

                    // Velocity
                    if (E.Config.VShape == VelocityShape.Shaped)
                    {
                        switch (E.Config.ShapeAxis)
                        {
                                // Cylinder lies along X axis
                            case 1:
                                VX = Math.Abs(VX) * Math.Sign(Height);
                                VY = Math.Abs(VY) * Math.Sign((float) (Blitz.Cos(Yaw)) * Dist);
                                VZ = Math.Abs(VZ) * Math.Sign((float) (Blitz.Sin(Yaw)) * Dist);
                                break;
                                // Cylinder lies along Y axis
                            case 2:
                                VY = Math.Abs(VY) * Math.Sign(Height);
                                VX = Math.Abs(VX) * Math.Sign((float) (Blitz.Cos(Yaw)) * Dist);
                                VZ = Math.Abs(VZ) * Math.Sign((float) (Blitz.Sin(Yaw)) * Dist);
                                break;
                                // Cylinder lies along Z axis
                            case 3:
                                VZ = Math.Abs(VZ) * Math.Sign(Height);
                                VX = Math.Abs(VX) * Math.Sign((float) (Blitz.Cos(Yaw)) * Dist);
                                VY = Math.Abs(VY) * Math.Sign((float) (Blitz.Sin(Yaw)) * Dist);
                                break;
                        }
                    }
                    else if (E.Config.VShape == VelocityShape.HeavilyShaped)
                    {
                        switch (E.Config.ShapeAxis)
                        {
                                // Cylinder lies along X axis
                            case 1:
                                VX = 0f;
                                VY = Math.Abs(VY) * (((float) (Blitz.Cos(Yaw)) * Dist) / E.Config.MaxRadius);
                                VZ = Math.Abs(VZ) * (((float) (Blitz.Sin(Yaw)) * Dist) / E.Config.MaxRadius);
                                break;
                                // Cylinder lies along Y axis
                            case 2:
                                VY = 0f;
                                VX = Math.Abs(VX) * (((float) (Blitz.Cos(Yaw)) * Dist) / E.Config.MaxRadius);
                                VZ = Math.Abs(VZ) * (((float) (Blitz.Sin(Yaw)) * Dist) / E.Config.MaxRadius);
                                break;
                                // Cylinder lies along Z axis
                            case 3:
                                VZ = 0f;
                                VX = Math.Abs(VX) * (((float) (Blitz.Cos(Yaw)) * Dist) / E.Config.MaxRadius);
                                VY = Math.Abs(VY) * (((float) (Blitz.Sin(Yaw)) * Dist) / E.Config.MaxRadius);
                                break;
                        }
                    }

                    break;
                case EmitterShape.Box:
                    // Position
                    X = E.Rand(E.Config.Width * -0.5f, E.Config.Width * 0.5f) * E.Scale;
                    Y = E.Rand(E.Config.Height * -0.5f, E.Config.Height * 0.5f) * E.Scale;
                    Z = E.Rand(E.Config.Depth * -0.5f, E.Config.Depth * 0.5f) * E.Scale;

                    // Velocity
                    if (E.Config.VShape == VelocityShape.Shaped)
                    {
                        VX = Math.Abs(VX) * Math.Sign(X);
                        VY = Math.Abs(VY) * Math.Sign(Y);
                        VZ = Math.Abs(VZ) * Math.Sign(Z);
                    }
                    else if (E.Config.VShape == VelocityShape.HeavilyShaped)
                    {
                        float TempX = X / (E.Config.Width * 0.5f);
                        float TempY = Y / (E.Config.Height * 0.5f);
                        float TempZ = Z / (E.Config.Depth * 0.5f);
                        if (float.IsNaN(TempX))
                        {
                            TempX = 0f;
                        }
                        if (float.IsNaN(TempY))
                        {
                            TempY = 0f;
                        }
                        if (float.IsNaN(TempZ))
                        {
                            TempZ = 0f;
                        }
                        if (Math.Abs(TempX) > Math.Abs(TempY) && Math.Abs(TempX) > Math.Abs(TempZ))
                        {
                            VX = Math.Abs(VX) * Math.Sign(TempX);
                            VY = 0f;
                            VZ = 0f;
                        }
                        else if (Math.Abs(TempY) > Math.Abs(TempX) && Math.Abs(TempY) > Math.Abs(TempZ))
                        {
                            VY = Math.Abs(VY) * Math.Sign(TempY);
                            VX = 0f;
                            VZ = 0f;
                        }
                        else
                        {
                            VZ = Math.Abs(VZ) * Math.Sign(TempZ);
                            VX = 0f;
                            VY = 0f;
                        }
                    }
                    break;
            }

            // Transform positions and velocities to global space (so emitter shapes can be moved/rotated)
            Entity.TFormPoint(X, Y, Z, E.EmitterEN, null);
            X = Entity.TFormedX();
            Y = Entity.TFormedY();
            Z = Entity.TFormedZ();
            Entity.TFormVector(VX, VY, VZ, E.EmitterEN, null);
            VX = Entity.TFormedX();
            VY = Entity.TFormedY();
            VZ = Entity.TFormedZ();

            // Initial texture frame
            if (E.Config.RndStartFrame)
            {
                SetFrame(E.Rand(0, (E.Config.TexAcross * E.Config.TexDown) - 1));
            }
            else
            {
                SetFrame(0);
            }

            // Other bits
            Scale = E.Config.ScaleStart * E.Scale;
            R = E.Config.RStart;
            G = E.Config.GStart;
            B = E.Config.BStart;
            A = E.Config.AlphaStart;
            TimeToLive = E.Config.Lifespan;

            // Set vertex colours
            Entity.VertexColor(Surf, FirstVertex, (int) R, (int) G, (int) B, A * 255);
            Entity.VertexColor(Surf, FirstVertex + 1, (int) R, (int) G, (int) B, A * 255);
            Entity.VertexColor(Surf, FirstVertex + 2, (int) R, (int) G, (int) B, A * 255);
            Entity.VertexColor(Surf, FirstVertex + 3, (int) R, (int) G, (int) B, A * 255);

            E.ToSpawn -= 1;
        }

        // Sets the current texture frame
        public void SetFrame(int Frame)
        {
            // Get X and Y of frame
            int X = Frame % E.Config.TexAcross;
            int Y = 0;
            if (Frame > E.Config.TexAcross - 1)
            {
                if (E.Config.TexAcross > 1)
                {
                    Y = ((Frame - X) / (E.Config.TexAcross - 1)) - 1;
                }
                else
                {
                    Y = Frame;
                }
            }

            // Get teture co-ordinates
            float MinU = (1f / (float) (E.Config.TexAcross)) * (float) X;
            float MaxU = (1f / (float) (E.Config.TexAcross)) * (float) (X + 1);
            float MinV = (1f / (float) (E.Config.TexDown)) * (float) Y;
            float MaxV = (1f / (float) (E.Config.TexDown)) * (float) (Y + 1);

            // Apply co-ordinates to vertices
            uint Surf = E.MeshEN.GetSurface(1);
            Entity.VertexTexCoords(Surf, FirstVertex, MinU, MaxV);
            Entity.VertexTexCoords(Surf, FirstVertex + 1, MinU, MinV);
            Entity.VertexTexCoords(Surf, FirstVertex + 2, MaxU, MinV);
            Entity.VertexTexCoords(Surf, FirstVertex + 3, MaxU, MaxV);

            // Done
            TexFrame = Frame;
            TexChange = (float) (E.Config.TexAnimSpeed + 1);
        }

        // Updates the position etc. for this particle
        public void UpdateVertices()
        {
            uint Surf = E.MeshEN.GetSurface(1);

            // Adjust for mesh position
            float PosX = X - E.MeshEN.X(true);
            float PosY = Y - E.MeshEN.Y(true);
            float PosZ = Z - E.MeshEN.Z(true);

            // Set the new position of each corner vertex
            Entity.TFormVector(-Scale, -Scale, 0, E.Config.FaceEntity, null);
            Entity.VertexCoords(Surf, FirstVertex, PosX + Entity.TFormedX(), PosY + Entity.TFormedY(),
                                PosZ + Entity.TFormedZ());

            Entity.TFormVector(-Scale, Scale, 0, E.Config.FaceEntity, null);
            Entity.VertexCoords(Surf, FirstVertex + 1, PosX + Entity.TFormedX(), PosY + Entity.TFormedY(),
                                PosZ + Entity.TFormedZ());

            Entity.TFormVector(Scale, Scale, 0, E.Config.FaceEntity, null);
            Entity.VertexCoords(Surf, FirstVertex + 2, PosX + Entity.TFormedX(), PosY + Entity.TFormedY(),
                                PosZ + Entity.TFormedZ());

            Entity.TFormVector(Scale, -Scale, 0, E.Config.FaceEntity, null);
            Entity.VertexCoords(Surf, FirstVertex + 3, PosX + Entity.TFormedX(), PosY + Entity.TFormedY(),
                                PosZ + Entity.TFormedZ());

            // Update vertex colours
            Entity.VertexColor(Surf, FirstVertex, (int) R, (int) G, (int) B, A * 255);
            Entity.VertexColor(Surf, FirstVertex + 1, (int) R, (int) G, (int) B, A * 255);
            Entity.VertexColor(Surf, FirstVertex + 2, (int) R, (int) G, (int) B, A * 255);
            Entity.VertexColor(Surf, FirstVertex + 3, (int) R, (int) G, (int) B, A * 255);
        }

        // Update this particle
        public void Update(float Delta)
        {
            // Get emitter mesh's surface
            uint Surf = E.MeshEN.GetSurface(1);

            // Update texture frame if necessary
            if (TexChange > 1f)
            {
                TexChange = TexChange - Delta;
                if (TexChange < 1f)
                {
                    int Frame = TexFrame + 1;
                    if (Frame > (E.Config.TexAcross * E.Config.TexDown) - 1)
                    {
                        Frame = 0;
                    }
                    SetFrame(Frame);
                }
            }

            // Adjust force
            switch (E.Config.FShape)
            {
                case ForceShape.Linear:
                    FX += E.Config.ForceModX * Delta;
                    FY += E.Config.ForceModY * Delta;
                    FZ += E.Config.ForceModZ * Delta;
                    break;
                case ForceShape.Spherical:
                    Entity Temp = Entity.CreatePivot();
                    Temp.Rotate(E.Config.ForceModX * Delta,
                                E.Config.ForceModY * Delta,
                                E.Config.ForceModZ * Delta);
                    Entity.TFormVector(FX, FY, FZ, Temp, null);
                    FX = Entity.TFormedX();
                    FY = Entity.TFormedY();
                    FZ = Entity.TFormedZ();
                    Temp.Free();
                    break;
            }

            // Adjust velocity
            Entity.TFormVector(FX, FY, FZ, E.EmitterEN, null);
            VX += Entity.TFormedX() * Delta;
            VY += Entity.TFormedY() * Delta;
            VZ += Entity.TFormedZ() * Delta;

            // Move
            X += VX * Delta;
            Y += VY * Delta;
            Z += VZ * Delta;

            // Update scale
            Scale += E.Config.ScaleChange * Delta;

            // Update colour
            R += E.Config.RChange * Delta;
            G += E.Config.GChange * Delta;
            B += E.Config.BChange * Delta;
            if (R > 255f)
            {
                R -= 255;
            }
            else if (R < 0f)
            {
                R += 255;
            }
            if (G > 255f)
            {
                G -= 255;
            }
            else if (G < 0f)
            {
                G += 255;
            }
            if (B > 255f)
            {
                B -= 255;
            }
            else if (B < 0f)
            {
                B += 255;
            }

            // Update alpha
            A += E.Config.AlphaChange * Delta;
            if (A < 0f)
            {
                TimeToLive = -1f;
            }

            // Update vertex positions and colours
            UpdateVertices();

            // Count down lifespan
            TimeToLive -= Delta;

            // Lifespan expired
            if (TimeToLive < 0f)
            {
                E.ActiveParticles -= 1;
                if (E.ToSpawn > 0)
                {
                    Spawn();
                }
                else
                {
                    InUse = false;
                    Entity.VertexColor(Surf, FirstVertex, 0, 0, 0, 0f);
                    Entity.VertexColor(Surf, FirstVertex + 1, 0, 0, 0, 0f);
                    Entity.VertexColor(Surf, FirstVertex + 2, 0, 0, 0, 0f);
                    Entity.VertexColor(Surf, FirstVertex + 3, 0, 0, 0, 0f);
                }
            }
        }

        // Constructor
        public Particle(Emitter Parent)
        {
            // Store emitter of this particle
            E = Parent;

            // Add vertices to emitter mesh
            uint Surf = E.MeshEN.GetSurface(1);
            int V1 = Entity.AddVertex(Surf, 0f, 0f, 0f, 0f, 1f);
            int V2 = Entity.AddVertex(Surf, 0f, 0f, 0f, 0f, 0f);
            int V3 = Entity.AddVertex(Surf, 0f, 0f, 0f, 1f, 0f);
            int V4 = Entity.AddVertex(Surf, 0f, 0f, 0f, 1f, 1f);
            Entity.VertexColor(Surf, V1, 0, 0, 0, 0f);
            Entity.VertexColor(Surf, V2, 0, 0, 0, 0f);
            Entity.VertexColor(Surf, V3, 0, 0, 0, 0f);
            Entity.VertexColor(Surf, V4, 0, 0, 0, 0f);
            Entity.AddTriangle(Surf, V1, V2, V3);
            Entity.AddTriangle(Surf, V1, V3, V4);

            // Start not in use
            InUse = false;
            FirstVertex = V1;

            // Maintain linked list
            this.NextParticle = E.FirstParticle;
            E.FirstParticle = this;
        }
    }

    // Emitter instance class
    public class Emitter : IDisposable
    {
        // Settings
        public Entity MeshEN, EmitterEN;
        public EmitterConfig Config;
        public bool Enabled;
        public byte KillMode;
        public int ToSpawn;
        public int ActiveParticles;
        public float Scale;

        // Random number generator
        private Random Rnd;

        public float Rand(float Min, float Max)
        {
            return Min + ((float) (Rnd.NextDouble()) * (Max - Min));
        }

        public int Rand(int Max)
        {
            return Rnd.Next(Max);
        }

        public int Rand(int Min, int Max)
        {
            return Rnd.Next(Min, Max);
        }

        // Linked lists
        public Particle FirstParticle;
        public static Emitter FirstEmitter;
        public Emitter NextEmitter;

        // Update this emitter
        public void Update(float Delta)
        {
            // Move mesh to the position of the 'user handle' entity
            MeshEN.Position(EmitterEN.X(true),
                            EmitterEN.Y(true),
                            EmitterEN.Z(true));

            if (Enabled)
            {
                // Spawn new particles if the emitter is fully active
                if (KillMode == 0)
                {
                    ToSpawn = (int) Math.Ceiling((float) (Config.ParticlesPerFrame) * Delta);
                }
                    // If this emitter is being killed, free it after all particles are dead
                else if (ActiveParticles == 0)
                {
                    switch (KillMode)
                    {
                        case 1:
                            Free(false, false);
                            break;
                        case 2:
                            Free(true, true);
                            break;
                        case 3:
                            Free(true, false);
                            break;
                        case 4:
                            Free(false, true);
                            break;
                    }
                }
            }
                // If this emitter is disabled and with no active particles, hide it
            else if (ActiveParticles == 0)
            {
                MeshEN.Visible = false;
            }

            // Update each particle
            Particle P = FirstParticle;
            while (P != null)
            {
                if (P.InUse)
                {
                    P.Update(Delta);
                }
                else if (ToSpawn > 0)
                {
                    P.Spawn();
                }
                P = P.NextParticle;
            }

            // Update Direct3D vertex/index buffers
            MeshEN.UpdateHardwareBuffers();
        }

        // Constructor
        public Emitter(EmitterConfig C, float Scale)
        {
            // Create random number generator
            Rnd = new Random(Environment.TickCount);

            Enabled = true;
            Config = C;
            this.Scale = Scale;
            EmitterEN = Entity.CreatePivot();
            EmitterEN.ExtraData = this;
            MeshEN = Entity.CreateMesh();
            MeshEN.CreateSurface();
            MeshEN.FX = 1 + 2 + 8 + 32 + 64;
            MeshEN.AlphaState = true;
            MeshEN.Texture(C.Texture);
            switch (C.BlendMode)
            {
                case 1:
                    MeshEN.Shader = Shaders.FullbrightAlpha;
                    break;
                case 2:
                    MeshEN.Shader = Shaders.FullbrightMultiply;
                    break;
                case 3:
                    MeshEN.Shader = Shaders.FullbrightAdd;
                    break;
            }

            // Create initial particles
            for (int i = 0; i < C.MaxParticles; ++i)
            {
                new Particle(this);
            }

            // Maintain linked list
            this.NextEmitter = FirstEmitter;
            FirstEmitter = this;
        }

        // Free this emitter
        public void Free(bool FreeConfig, bool FreeTex)
        {
            // Free config/texture if required
            if (FreeConfig)
            {
                Config.Free(FreeTex);
            }
            else if (FreeTex)
            {
                // Set all instances of this texture in RottParticles to 0
                EmitterConfig C = EmitterConfig.FirstConfig;
                while (C != null)
                {
                    if (C != Config && C.Texture == Config.Texture)
                    {
                        C.ChangeTexture(0, false);
                    }
                    C = C.NextConfig;
                }
                // Free the texture
                Render.FreeTexture(Config.Texture);
                Config.ChangeTexture(0, false);
            }

            // Maintain linked list
            Emitter E = FirstEmitter;
            if (E == this)
            {
                FirstEmitter = NextEmitter;
            }
            else
            {
                while (E != null)
                {
                    if (E.NextEmitter == this)
                    {
                        E.NextEmitter = this.NextEmitter;
                        NextEmitter = null;
                        break;
                    }
                    E = E.NextEmitter;
                }
            }

            Dispose();
        }

        public void Kill(bool FreeConfig, bool FreeTex)
        {
            KillMode = 1;
            if (FreeConfig)
            {
                if (FreeTex)
                {
                    KillMode = 2;
                }
                else
                {
                    KillMode = 3;
                }
            }
            else if (FreeTex)
            {
                KillMode = 4;
            }
        }

        // Dispose of this object and unmanaged resources
        public void Dispose()
        {
            // Free 3D entities
            MeshEN.Free();
            EmitterEN.Free();
        }
    }

    // Emitter template class
    public class EmitterConfig
    {
        // Settings
        public string Name;
        private int MaxParts;

        public int MaxParticles
        {
            get { return MaxParts; }
            set
            {
                int Difference = value - MaxParts;
                MaxParts = value;

                // For each emitter using this config, add/remove particles as required
                Emitter E = Emitter.FirstEmitter;
                while (E != null)
                {
                    if (E.Config == this)
                    {
                        // Add particles
                        if (Difference > 0)
                        {
                            for (int i = 0; i < Difference; ++i)
                            {
                                new Particle(E);
                            }
                        }
                            // Remove particles (rebuild emitter mesh)
                        else if (Difference < 0)
                        {
                            E.FirstParticle = null;
                            E.ActiveParticles = 0;
                            Entity.ClearSurface(E.MeshEN.GetSurface(1));
                            for (int i = 0; i < MaxParts; ++i)
                            {
                                new Particle(E);
                            }
                        }
                    }
                    E = E.NextEmitter;
                }
            }
        }

        public int ParticlesPerFrame;
        private uint Tex;

        public uint Texture
        {
            get { return Tex; }
        }

        public void ChangeTexture(uint NewTex, bool FreePrevious)
        {
            // Change texture
            if (FreePrevious && Tex != 0)
            {
               // Render.FreeTexture(Tex);
            }
            Tex = NewTex;

            // For each emitter using this config, update the texture
            if (Tex != 0)
            {
                Emitter E = Emitter.FirstEmitter;
                while (E != null)
                {
                    if (E.Config == this)
                    {
                        E.MeshEN.Texture(Tex);
                    }
                    E = E.NextEmitter;
                }
            }
        }

        public void ChangeTexture(uint NewTex)
        {
            ChangeTexture(NewTex, true);
        }

        public int TexAcross, TexDown, TexAnimSpeed;
        public bool RndStartFrame;
        private int Blend;

        public int BlendMode
        {
            get { return Blend; }
            set
            {
                Blend = value;

                // For each emitter using this config, update the blend mode
                Emitter E = Emitter.FirstEmitter;
                while (E != null)
                {
                    if (E.Config == this)
                    {
                        switch (Blend)
                        {
                            case 1:
                                E.MeshEN.Shader = Shaders.FullbrightAlpha;
                                break;
                            case 2:
                                E.MeshEN.Shader = Shaders.FullbrightMultiply;
                                break;
                            case 3:
                                E.MeshEN.Shader = Shaders.FullbrightAdd;
                                break;
                        }
                    }
                    E = E.NextEmitter;
                }
            }
        }

        public VelocityShape VShape;
        public float VelocityX, VelocityY, VelocityZ;
        public float VelocityRndX, VelocityRndY, VelocityRndZ;
        public ForceShape FShape;
        public float ForceX, ForceY, ForceZ;
        public float ForceModX, ForceModY, ForceModZ;
        public float ScaleStart, ScaleChange;
        public int Lifespan;
        public byte RStart, GStart, BStart;
        public float RChange, GChange, BChange;
        public float AlphaStart, AlphaChange;
        public Entity FaceEntity;
        public EmitterShape Shape;
        public float MinRadius, MaxRadius, Width, Height, Depth;
        public int ShapeAxis;
        public ushort DefaultTextureID; // Realm Crafter specific

        // Linked list
        public static EmitterConfig FirstConfig;
        public EmitterConfig NextConfig;

        // Create a new emitter as a file
        public static void CreateAsFile(string Filename)
        {
            FileStream FS = new FileStream(Filename, FileMode.Create, FileAccess.Write);
            BinaryWriter F = new BinaryWriter(FS);

            F.Write(1);
            F.Write(1);
            F.Write(1);
            F.Write(1);
            F.Write(0);
            F.Write(0);
            F.Write((int) VelocityShape.Normal);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(1f);
            F.Write(0f);
            F.Write(10);
            F.Write(1f);
            F.Write(0f);
            F.Write(3);
            F.Write((int) EmitterShape.Sphere);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write(2);
            F.Write((ushort) 65535);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);
            F.Write((int) ForceShape.Linear);
            F.Write((byte) 255);
            F.Write((byte) 255);
            F.Write((byte) 255);
            F.Write(0f);
            F.Write(0f);
            F.Write(0f);

            F.Close();
        }

        // Load from file
        public static EmitterConfig Load(string Filename, Entity FaceEntity, uint Texture)
        {
            FileStream FS = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            BinaryReader F = new BinaryReader(FS);

            int MaxParticles = F.ReadInt32();
            int SpawnRate = F.ReadInt32();
            EmitterConfig EC = new EmitterConfig(MaxParticles, SpawnRate, FaceEntity, Texture);

            EC.Name = System.IO.Path.GetFileNameWithoutExtension(Filename);

            EC.TexAcross = F.ReadInt32();
            EC.TexDown = F.ReadInt32();
            EC.RndStartFrame = (F.ReadInt32() != 0);
            EC.TexAnimSpeed = F.ReadInt32();
            EC.VShape = (VelocityShape) F.ReadInt32();
            EC.VelocityX = F.ReadSingle();
            EC.VelocityY = F.ReadSingle();
            EC.VelocityZ = F.ReadSingle();
            EC.VelocityRndX = F.ReadSingle();
            EC.VelocityRndY = F.ReadSingle();
            EC.VelocityRndZ = F.ReadSingle();
            EC.ForceX = F.ReadSingle();
            EC.ForceY = F.ReadSingle();
            EC.ForceZ = F.ReadSingle();
            EC.ScaleStart = F.ReadSingle();
            EC.ScaleChange = F.ReadSingle();
            EC.Lifespan = F.ReadInt32();
            EC.AlphaStart = F.ReadSingle();
            EC.AlphaChange = F.ReadSingle();
            EC.BlendMode = F.ReadInt32();
            EC.Shape = (EmitterShape) F.ReadInt32();
            EC.MinRadius = F.ReadSingle();
            EC.MaxRadius = F.ReadSingle();
            EC.Width = F.ReadSingle();
            EC.Height = F.ReadSingle();
            EC.Depth = F.ReadSingle();
            EC.ShapeAxis = F.ReadInt32();
            EC.DefaultTextureID = F.ReadUInt16();
            EC.ForceModX = F.ReadSingle();
            EC.ForceModY = F.ReadSingle();
            EC.ForceModZ = F.ReadSingle();
            EC.FShape = (ForceShape) F.ReadInt32();
            EC.RStart = F.ReadByte();
            EC.GStart = F.ReadByte();
            EC.BStart = F.ReadByte();
            EC.RChange = F.ReadSingle();
            EC.GChange = F.ReadSingle();
            EC.BChange = F.ReadSingle();

            F.Close();

            return EC;
        }

        // Save to file
        public void Save(string Filename)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                FileStream FS = new FileStream(Filename, FileMode.Truncate, FileAccess.Write);
                BinaryWriter F = new BinaryWriter(FS);

                F.Write(MaxParticles);
                F.Write(ParticlesPerFrame);
                F.Write(TexAcross);
                F.Write(TexDown);
                if (RndStartFrame)
                {
                    F.Write((int) 1);
                }
                else
                {
                    F.Write((int) 0);
                }
                F.Write(TexAnimSpeed);
                F.Write((int) VShape);
                F.Write(VelocityX);
                F.Write(VelocityY);
                F.Write(VelocityZ);
                F.Write(VelocityRndX);
                F.Write(VelocityRndY);
                F.Write(VelocityRndZ);
                F.Write(ForceX);
                F.Write(ForceY);
                F.Write(ForceZ);
                F.Write(ScaleStart);
                F.Write(ScaleChange);
                F.Write(Lifespan);
                F.Write(AlphaStart);
                F.Write(AlphaChange);
                F.Write(BlendMode);
                F.Write((int) Shape);
                F.Write(MinRadius);
                F.Write(MaxRadius);
                F.Write(Width);
                F.Write(Height);
                F.Write(Depth);
                F.Write(ShapeAxis);
                F.Write(DefaultTextureID);
                F.Write(ForceModX);
                F.Write(ForceModY);
                F.Write(ForceModZ);
                F.Write((int) FShape);
                F.Write(RStart);
                F.Write(GStart);
                F.Write(BStart);
                F.Write(RChange);
                F.Write(GChange);
                F.Write(BChange);

                F.Close();
            }
        }

        // Constructor
        public EmitterConfig(int MaxParticles, int SpawnRate, Entity FaceEntity, uint Texture)
        {
            // Set default values
            this.MaxParticles = MaxParticles;
            this.ParticlesPerFrame = SpawnRate;
            this.FaceEntity = FaceEntity;
            Tex = Texture;
            Name = "New emitter";
            AlphaStart = 1f;
            TexAcross = 1;
            TexDown = 1;
            RndStartFrame = true;
            Lifespan = 100;
            BlendMode = 3;
            ScaleStart = 1f;
            VShape = VelocityShape.Normal;
            FShape = ForceShape.Linear;
            RStart = 255;
            GStart = 255;
            BStart = 255;
            Shape = EmitterShape.Sphere;
            DefaultTextureID = 65535;

            // Maintain linked list
            this.NextConfig = FirstConfig;
            FirstConfig = this;
        }

        // Constructor to copy another emitter config
        public EmitterConfig(EmitterConfig Copy)
        {
            // Copy all values
            Name = Copy.Name + " (copy)";
            MaxParticles = Copy.MaxParticles;
            Tex = Copy.Texture;
            TexAcross = Copy.TexAcross;
            TexDown = Copy.TexDown;
            RndStartFrame = Copy.RndStartFrame;
            TexAnimSpeed = Copy.TexAnimSpeed;
            VShape = Copy.VShape;
            VelocityX = Copy.VelocityX;
            VelocityY = Copy.VelocityY;
            VelocityZ = Copy.VelocityZ;
            VelocityRndX = Copy.VelocityRndX;
            VelocityRndY = Copy.VelocityRndY;
            VelocityRndZ = Copy.VelocityRndZ;
            ForceX = Copy.ForceX;
            ForceY = Copy.ForceY;
            ForceZ = Copy.ForceZ;
            ForceModX = Copy.ForceModX;
            ForceModY = Copy.ForceModY;
            ForceModZ = Copy.ForceModZ;
            FShape = Copy.FShape;
            ScaleStart = Copy.ScaleStart;
            ScaleChange = Copy.ScaleChange;
            Lifespan = Copy.Lifespan;
            RStart = Copy.RStart;
            GStart = Copy.GStart;
            BStart = Copy.BStart;
            RChange = Copy.RChange;
            GChange = Copy.GChange;
            BChange = Copy.BChange;
            AlphaStart = Copy.AlphaStart;
            AlphaChange = Copy.AlphaChange;
            FaceEntity = Copy.FaceEntity;
            BlendMode = Copy.BlendMode;
            Shape = Copy.Shape;
            MinRadius = Copy.MinRadius;
            MaxRadius = Copy.MaxRadius;
            Width = Copy.Width;
            Height = Copy.Height;
            Depth = Copy.Depth;
            ShapeAxis = Copy.ShapeAxis;
            DefaultTextureID = Copy.DefaultTextureID; // Realm Crafter specific

            // Maintain linked list
            this.NextConfig = FirstConfig;
            FirstConfig = this;
        }

        // Free this emitter config
        public void Free(bool FreeTex)
        {
            EmitterConfig C;

            // Free texture if required
            if (FreeTex)
            {
                C = FirstConfig;
                while (C != null)
                {
                    if (C != this && C.Texture == Texture)
                    {
                        C.Tex = 0;
                    }
                    C = C.NextConfig;
                }
                Render.FreeTexture(Texture);
                Tex = 0;
            }

            // Maintain linked list
            C = FirstConfig;
            if (C == this)
            {
                FirstConfig = NextConfig;
            }
            else
            {
                while (C != null)
                {
                    if (C.NextConfig == this)
                    {
                        C.NextConfig = this.NextConfig;
                        NextConfig = null;
                        break;
                    }
                    C = C.NextConfig;
                }
            }
        }
    }

    // Constants
    public enum EmitterShape
    {
        Sphere = 1,
        Cylinder = 2,
        Box = 3
    } ;

    public enum VelocityShape
    {
        Normal = 1,
        Shaped = 2,
        HeavilyShaped = 3
    } ;

    public enum ForceShape
    {
        Linear = 1,
        Spherical = 2
    } ;
}