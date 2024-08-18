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
using RealmCrafter.ClientZone;
using RenderingServices;
using RealmCrafter;
using NGUINet;

namespace RealmCrafter_GE
{
    // Classes for 3D representations of server-side objects
    public class Trigger : ZoneObject
    {
        public float X, Y, Z, Size;
        public string Script = "";

        public bool IsSquare;
        public float Width, Height, Depth;

        public int ServerID;
        public Entity MoverPivot;
        public NLabel Label;

        // Constructor
        public Trigger(RealmCrafter.ServerZone.Zone serverZone, GE G, int ID)
            : base()
        {
            ServerID = ID;

            // Add to list
            G.Triggers.Add(this);
            serverZone.Triggers.Add(this);

            // Create mesh
            EN = Entity.CreateSphere();
            EN.Shader = Shaders.FullbrightAlpha;
            EN.Texture(G.BlueTex);
            EN.AlphaNoSolid(0.5f);
            EN.AlphaState = true;
            EN.Scale(2f, 2f, 2f);
            EN.RenderMask = 16;
            Collision.EntityType(EN, (byte)CollisionType.Triangle);
            Collision.SetCollisionMesh(EN);
            Collision.EntityPickMode(EN, PickMode.Polygon);


            Label = GE.GUIManager.CreateLabel("PORTALNAME", new NVector2(0, 0), new NVector2(0, 0));
            Label.Visible = false;

            MoverPivot = Entity.CreatePivot();
        }

        public void ReBuild()
        {
            if (EN != null)
                EN.Free();

            if (IsSquare)
            {
                EN = Entity.CreateCube();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.Texture(Program.GE.BlueTex);
                EN.AlphaNoSolid(0.5f);
                EN.AlphaState = true;
                EN.RenderMask = 16;
                Collision.EntityType(EN, (byte)CollisionType.Triangle);
                Collision.SetCollisionMesh(EN);
                Collision.EntityPickMode(EN, PickMode.Box);

                EN.Position(X, Y, Z);
                EN.Scale(Width * 0.5f, Height * 0.5f, Depth * 0.5f);
            }
            else
            {
                EN = Entity.CreateSphere();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.Texture(Program.GE.BlueTex);
                EN.AlphaNoSolid(0.5f);
                EN.AlphaState = true;
                EN.RenderMask = 16;
                Collision.EntityType(EN, (byte)CollisionType.Triangle);
                Collision.SetCollisionMesh(EN);
                Collision.EntityPickMode(EN, PickMode.Box);

                EN.Position(X, Y, Z);
                EN.Scale(Size, Size, Size);
            }

            Label.Text = "Trigger: " + Script;
        }

        public void UpdateLabel()
        {
            UpdateLabel(false);
        }

        public void UpdateLabel(bool hideonly)
        {
            Label.Visible = false;

            if (hideonly)
                return;

            if (Program.GE.RenderingPanelCurrentIndex != -3)
                return;
            if (Program.GE.RenderToggleEditor.Checked)
                return;

            float PHeight = Height * 0.6f;
            if (!IsSquare)
                PHeight = Size * 1.1f;

            float DistX = Program.GE.Camera.X() - X;
            float DistY = Program.GE.Camera.Y() - Y + PHeight;
            float DistZ = Program.GE.Camera.Z() - Z;
            float Dist = Math.Abs(DistX * DistX + DistY * DistY + DistZ * DistZ);

            if (Dist < 500 * 500)
            {
                RenderWrapper.bbdx2_ManagedProjectVector3(X, Y + PHeight, Z);

                float PrX = RenderWrapper.bbdx2_ProjectedX();
                float PrY = RenderWrapper.bbdx2_ProjectedY();
                float PrZ = RenderWrapper.bbdx2_ProjectedZ();


                PrX -= (Label.InternalWidth * 0.5f);

                Label.Location = new NVector2(PrX, PrY);


                if (PrZ > 0.1f && PrZ < 1.0f && PrX > 0.0f && PrX < 1.0f && PrY > 0.0f && PrY < 1.0f)
                    Label.Visible = true;
            }
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        // Update server info
        public override void UpdateServerVersion(RealmCrafter.ServerZone.Zone CurrentServerZone)
        {
            X = EN.X();
            Y = EN.Y();
            Z = EN.Z();
            Size = EN.ScaleX();
            Width = EN.ScaleX() * 2;
            Height = EN.ScaleY() * 2;
            Depth = EN.ScaleZ() * 2;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new ScaleTool(MoverPivot, new EventHandler(Scaling), new EventHandler(Scaled));
            (Program.Transformer as ScaleTool).Multiplier = 0.05f;
            ScaleTool ST = Program.Transformer as ScaleTool;
            if (!IsSquare)
            {
                ST.CopyXToY = true;
                ST.CopyXToZ = true;
            }
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            UpdateServerVersion(Program.GE.CurrentServerZone);
            Program.GE.SetWorldSavedStatus(false);
        }


        public override void Scaling(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Scale(MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleX() / 200.0f);
            UpdateServerVersion(Program.GE.CurrentServerZone);
            Program.GE.SetWorldSavedStatus(false);
        }

    }

    public class Portal : ZoneObject
    {
        public string Name = "";
        public string LinkArea = "", LinkName = "";
        public float X, Y, Z, Size, Yaw;

        public bool IsSquare;
        public float Width, Height, Depth;

        public int ServerID;
        public Entity MoverPivot;
        public NLabel Label;

        bool BuiltSquare = false;

        // Constructor
        public Portal(RealmCrafter.ServerZone.Zone serverZone, GE G, int ID)
            : base()
        {
            ServerID = ID;

            // Add to list
            G.Portals.Add(this);
            serverZone.Portals.Add(this);

            // Create mesh
            EN = G.PortalTemplate.Copy();
            EN.Shader = Shaders.FullbrightAlpha;
            EN.AlphaNoSolid(0.5f);
            EN.AlphaState = true;
            EN.RenderMask = 16;
            Collision.EntityType(EN, (byte)CollisionType.Triangle);
            Collision.SetCollisionMesh(EN);
            Collision.EntityPickMode(EN, PickMode.Box);

            Label = GE.GUIManager.CreateLabel("PORTALNAME", new NVector2(0, 0), new NVector2(0, 0));
            Label.Visible = false;

            MoverPivot = Entity.CreatePivot();
        }

        public void ReBuild()
        {
            if(EN != null)
                EN.Free();

            if (IsSquare)
            {
                EN = Entity.CreateCube();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.AlphaNoSolid(0.5f);
                EN.AlphaState = true;
                EN.RenderMask = 16;
                Collision.EntityType(EN, (byte)CollisionType.Triangle);
                Collision.SetCollisionMesh(EN);
                Collision.EntityPickMode(EN, PickMode.Box);

                EN.Position(X, Y, Z);
                EN.Scale(Width * 0.5f, Height * 0.5f, Depth * 0.5f);
            }
            else
            {
                EN = Program.GE.PortalTemplate.Copy();
                EN.Shader = Shaders.FullbrightAlpha;
                EN.AlphaNoSolid(0.5f);
                EN.AlphaState = true;
                EN.RenderMask = 16;
                Collision.EntityType(EN, (byte)CollisionType.Triangle);
                Collision.SetCollisionMesh(EN);
                Collision.EntityPickMode(EN, PickMode.Box);

                EN.Position(X, Y, Z);
                EN.Scale(Size, Size, Size);
                EN.Rotate(0, Yaw, 0);
            }

            Label.Text = "Portal: " + Name;
        }

        public void UpdateLabel()
        {
            UpdateLabel(false);
        }

        public void UpdateLabel(bool hideonly)
        {
            Label.Visible = false;

            if (hideonly)
                return;
            if (Program.GE.RenderingPanelCurrentIndex != -3)
                return;
            if (Program.GE.RenderToggleEditor.Checked)
                return;

            float PHeight = Height * 0.6f;
            if (!IsSquare)
                PHeight = Size * 1.1f;

            float DistX = Program.GE.Camera.X() - X;
            float DistY = Program.GE.Camera.Y() - Y + PHeight;
            float DistZ = Program.GE.Camera.Z() - Z;
            float Dist = Math.Abs(DistX * DistX + DistY * DistY + DistZ * DistZ);

            if (Dist < 500 * 500)
            {
                RenderWrapper.bbdx2_ManagedProjectVector3(X, Y + PHeight, Z);

                float PrX = RenderWrapper.bbdx2_ProjectedX();
                float PrY = RenderWrapper.bbdx2_ProjectedY();
                float PrZ = RenderWrapper.bbdx2_ProjectedZ();

                
                PrX -= (Label.InternalWidth * 0.5f);

                Label.Location = new NVector2(PrX, PrY);

                
                if (PrZ > 0.1f && PrZ < 1.0f && PrX > 0.0f && PrX < 1.0f && PrY > 0.0f && PrY < 1.0f)
                    Label.Visible = true;
            }
        }

        // Update server info
        public override void UpdateServerVersion(RealmCrafter.ServerZone.Zone CurrentServerZone)
        {
            X = EN.X();
            Y = EN.Y();
            Z = EN.Z();
            Size = EN.ScaleX();
            Yaw = EN.Yaw();
            Width = EN.ScaleX() * 2;
            Height = EN.ScaleY() * 2;
            Depth = EN.ScaleZ() * 2;
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void ScaleInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new ScaleTool(MoverPivot, new EventHandler(Scaling), new EventHandler(Scaled));
            (Program.Transformer as ScaleTool).Multiplier = 0.05f;
            ScaleTool ST = Program.Transformer as ScaleTool;

            if (!IsSquare)
            {
                ST.CopyXToY = true;
                ST.CopyXToZ = true;
            }
        }

        public override void RotateInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            if (!IsSquare)
                Program.Transformer = new RotateTool(MoverPivot, new EventHandler(Rotating), new EventHandler(Rotated));
            else Program.Transformer = null;

        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            UpdateServerVersion(Program.GE.CurrentServerZone);
            Program.GE.SetWorldSavedStatus(false);
        }


        public override void Scaling(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Scale(MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleX() / 200.0f, MoverPivot.ScaleX() / 200.0f);
            UpdateServerVersion(Program.GE.CurrentServerZone);
            Program.GE.SetWorldSavedStatus(false);
        }

        public override void Rotating(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Rotate(0, MoverPivot.Yaw(), 0);
            UpdateServerVersion(Program.GE.CurrentServerZone);
            Program.GE.SetWorldSavedStatus(false);
        }
    }

    public class Waypoint : ZoneObject
    {
        public float X, Y, Z;
        public Waypoint NextA, NextB, Prev;
        public int PauseTime;

        public ushort ActorID = 65535;
        public float Size, Range;
        public string Script;
        public ushort Frequency, Max;

        public int ServerID;
        public Entity MoverPivot;

        public Entity WaypointEN, WaypointLinkAEN, WaypointLinkBEN;
        public int TNextA, TNextB, TPrev;

        public NLabel Label;

        // Constructor
        public Waypoint(RealmCrafter.ServerZone.Zone serverZone, GE G, int ID)
            : base()
        {
            ServerID = ID;

            // Add to list
            G.Waypoints.Add(this);
            serverZone.Waypoints.Add(this);

            // Create mesh
            EN = G.WaypointTemplate.Copy();
            EN.Shader = Shaders.FullbrightAlpha;
            EN.AlphaNoSolid(0.5f);
            EN.AlphaState = true;
            EN.RenderMask = 16;
            Collision.EntityType(EN, (byte)CollisionType.Triangle);
            Collision.SetCollisionMesh(EN);
            Collision.EntityPickMode(EN, PickMode.Polygon);

            Label = GE.GUIManager.CreateLabel("WAYPOINTLABEL", new NVector2(0, 0), new NVector2(0, 0));
            Label.Visible = false;

            MoverPivot = Entity.CreatePivot();
        }

        public void UpdateLabel()
        {
            UpdateLabel(false);
        }

        public void UpdateLabel(bool hideonly)
        {
            Label.Visible = false;

            if (hideonly)
                return;

            if (Max == 0)
                return;

            if (Program.GE.RenderingPanelCurrentIndex != -3)
                return;
            if (Program.GE.RenderToggleEditor.Checked)
                return;

            float PHeight = Size * 1.1f;

            float DistX = Program.GE.Camera.X() - X;
            float DistY = Program.GE.Camera.Y() - Y + PHeight;
            float DistZ = Program.GE.Camera.Z() - Z;
            float Dist = Math.Abs(DistX * DistX + DistY * DistY + DistZ * DistZ);

            if (Dist < 500 * 500)
            {
                RenderWrapper.bbdx2_ManagedProjectVector3(X, Y + PHeight, Z);

                float PrX = RenderWrapper.bbdx2_ProjectedX();
                float PrY = RenderWrapper.bbdx2_ProjectedY();
                float PrZ = RenderWrapper.bbdx2_ProjectedZ();


                PrX -= (Label.InternalWidth * 0.5f);

                Label.Location = new NVector2(PrX, PrY);


                if (PrZ > 0.1f && PrZ < 1.0f && PrX > 0.0f && PrX < 1.0f && PrY > 0.0f && PrY < 1.0f)
                    Label.Visible = true;
            }
        }

        // Update server info
        public override void UpdateServerVersion(RealmCrafter.ServerZone.Zone CurrentServerZone)
        {
            X = EN.X();
            Y = EN.Y();
            Z = EN.Z();
            Size = EN.ScaleX();
        }

        public override void Freeing()
        {
            MoverPivot.Free();
            MoverPivot = null;
        }

        public override void UpdateTransformTool()
        {
            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);
        }

        public override void MoveInit()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();

            MoverPivot.Position(EN.X(), EN.Y(), EN.Z());
            MoverPivot.Rotate(EN.Pitch(), EN.Yaw(), EN.Roll());
            MoverPivot.Scale(EN.ScaleX() * 200.0f, EN.ScaleY() * 200.0f, EN.ScaleZ() * 200.0f);

            Program.Transformer = new MoveTool(MoverPivot, new EventHandler(Moving), new EventHandler(Moved));
        }

        public override void Moving(object sender, EventArgs e)
        {
            if (MoverPivot == null)
                return;

            EN.Position(MoverPivot.X(), MoverPivot.Y(), MoverPivot.Z());
            UpdateServerVersion(Program.GE.CurrentServerZone);
            Program.GE.SetWorldSavedStatus(false);
        }
    }

}
