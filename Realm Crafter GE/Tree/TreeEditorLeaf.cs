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
    public class TreeEditorLeaf : TreePropertiesUpdater
    {
        public TreeEditorLeafGroup Group;
        public Vector3 Position = new Vector3();
        public Entity Mesh;
        public bool Selected = false;

        public TreeEditorLeaf(TreeEditorLeafGroup group)
        {
            Group = group;
        }

        ~TreeEditorLeaf()
        {
            if (Mesh != null)
                Mesh.Free();
            Mesh = null;
        }

        public override void UpdateWorldButtonSelection()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();
            Program.Transformer = null;

            if (Program.GE.SetWorldButtonSelection == (int)GE.WorldButtonSelection.MOVE)
            {
                Program.Transformer = new MoveTool(Mesh, new EventHandler(Moving), new EventHandler(Moved));
            }
        }

        public void Moved(object sender, EventArgs e)
        {
            Position.X = Mesh.X();
            Position.Y = Mesh.Y();
            Position.Z = Mesh.Z();

            System.Windows.Forms.TreeNode Node = Program.GE.TreeProperties.FindNode(this);
            if (Node != null)
                Node.Text = this.ToString();
        }

        public void Moving(object sender, EventArgs e)
        {
            Program.GE.TreeRender.Saved = false;
            Program.GE.TreeProperties.SuspendEvents = true;
            Program.GE.TreeProperties.PositionX.Value = Convert.ToDecimal(Position.X);
            Program.GE.TreeProperties.PositionY.Value = Convert.ToDecimal(Position.Y);
            Program.GE.TreeProperties.PositionZ.Value = Convert.ToDecimal(Position.Z);
            Program.GE.TreeProperties.SuspendEvents = false;
        }

        public override void RecalculateBoundingBox(ref TreeBoundingBox box)
        {
            box.AddPoint(Position, Group.Scale);
        }

        public override void StartLODRender(float angle, ref TreeBoundingBox box)
        {
            if (Mesh != null)
            {
                Mesh.Shader = Program.LeafEditorShaderLOD;
                RenderWrapper.EntityConstantFloat(Mesh.Handle, "Angle", angle);
                RenderWrapper.EntityConstantFloat3(Mesh.Handle, "BoxMin", box.Min.X, box.Min.Y, box.Min.Z);
                RenderWrapper.EntityConstantFloat3(Mesh.Handle, "BoxMax", box.Max.X, box.Max.Y, box.Max.Z);
            }
        }

        public override bool HasInternalEntity(Entity entity)
        {
            if (Mesh != null && Mesh.Handle == entity.Handle)
                return true;
            return false;
        }

        public override void EndLODRender()
        {
            if (Mesh != null)
                Mesh.Shader = Program.LeafEditorShader;
        }

        public override void Show()
        {
            if (Mesh != null)
                Mesh.Visible = true;
        }

        public override void Hide()
        {
            if (Mesh != null)
                Mesh.Visible = false;
        }

        public override void Remove()
        {
            if (Mesh != null)
                Mesh.Free();
            Mesh = null;

            if (Group != null)
                Group.Leaves.Remove(this);
        }

        public override void Read(System.Xml.XmlTextReader x)
        {
            UpdatePosition(Convert.ToSingle(x.GetAttribute("x")),
                Convert.ToSingle(x.GetAttribute("y")),
                Convert.ToSingle(x.GetAttribute("z")));
        }

        public override void Write(System.Xml.XmlTextWriter x)
        {
            x.WriteStartElement("leaf");

            x.WriteAttributeString("x", Position.X.ToString());
            x.WriteAttributeString("y", Position.Y.ToString());
            x.WriteAttributeString("z", Position.Z.ToString());

            x.WriteEndElement();
        }

        public override void Export(System.IO.BinaryWriter writer, string outputDir, string name, int index)
        {
            if (Mesh == null)
            {
                writer.Write(0.0f);
                writer.Write(0.0f);
                writer.Write(0.0f);
            }
            else
            {
                writer.Write(Mesh.X());
                writer.Write(Mesh.Y());
                writer.Write(Mesh.Z());
            }
        }

        public void ReBuild()
        {
            bool Visible = true;
            if (Mesh != null)
            {
                Visible = Mesh.Visible;
                Mesh.Free();
            }

            Mesh = Entity.CreateMesh();
            uint S = Mesh.CreateSurface();

            Entity.AddVertex(S, 0, 0, 0, 0, 0);
            Entity.AddVertex(S, 0, 0, 0, 1, 0);
            Entity.AddVertex(S, 0, 0, 0, 0, 1);
            Entity.AddVertex(S, 0, 0, 0, 1, 1);

            if (Selected)
            {
                Entity.VertexColor(S, 0, 0, 0, 0, 1.0f);
                Entity.VertexColor(S, 1, 0, 0, 0, 1.0f);
                Entity.VertexColor(S, 2, 0, 0, 0, 1.0f);
                Entity.VertexColor(S, 3, 0, 0, 0, 1.0f);
            }
            else
            {
                Entity.VertexColor(S, 0, 255, 255, 255, 1.0f);
                Entity.VertexColor(S, 1, 255, 255, 255, 1.0f);
                Entity.VertexColor(S, 2, 255, 255, 255, 1.0f);
                Entity.VertexColor(S, 3, 255, 255, 255, 1.0f);
            }

            Entity.AddTriangle(S, 2, 1, 0);
            Entity.AddTriangle(S, 2, 3, 1);

            Mesh.UpdateHardwareBuffers();
            Mesh.Shader = Program.LeafEditorShader;

            Mesh.Position(Position.X, Position.Y, Position.Z);
            Mesh.Texture(Group.DiffuseHandle, 0);
            Mesh.Texture(Group.NormalHandle, 1);

            Mesh.Visible = Visible;

            uint CollisionHandle = RenderWrapper.bbdx2_CreatePhysicsDesc(1, Mesh.Handle);
            RenderWrapper.bbdx2_AddSphere(CollisionHandle, 0, 0, 0, Group.Scale * 0.5f, -1);
            RenderWrapper.bbdx2_ClosePhysicsDesc(CollisionHandle);

            //Collision.EntityRadius(Mesh, Group.Scale, Group.Scale);
            Collision.EntityPickMode(Mesh, PickMode.Sphere);

            RenderWrapper.EntityConstantFloat(Mesh.Handle, "BillboardScale", Group.Scale);
            RenderWrapper.EntityConstantFloat(Mesh.Handle, "Selected", Selected ? 1.0f : 0.0f);
        }

        public override void UpdatePosition(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;

            Position = new Vector3(x, y, z);
            
            if(Mesh != null)
                Mesh.Position(Position.X, Position.Y, Position.Z);

            if (Program.ActiveTree != null)
                Program.ActiveTree.UpdateNode(this);
        }

        public override void UpdateProperties(TreeEditorProperties propertiesPanel)
        {
            propertiesPanel.ClearProperties();

            propertiesPanel.PositionX.Value = Convert.ToDecimal(Mesh.X());
            propertiesPanel.PositionY.Value = Convert.ToDecimal(Mesh.Y());
            propertiesPanel.PositionZ.Value = Convert.ToDecimal(Mesh.Z());

            propertiesPanel.PositionX.Enabled = true;
            propertiesPanel.PositionY.Enabled = true;
            propertiesPanel.PositionZ.Enabled = true;
        }

        public override void NotifySelected()
        {
            Selected = true;
            ReBuild();
        }

        public override void NotifyUnselected()
        {
            Selected = false;
            ReBuild();
        }

        public override string ToString()
        {
            return "Leaf: " + Position.ToString();
        }
    }
}
