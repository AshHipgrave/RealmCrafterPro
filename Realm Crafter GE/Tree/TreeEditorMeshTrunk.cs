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
using System.IO;

namespace RealmCrafter_GE
{
    public class TreeEditorMeshTrunk : TreeEditorTrunk
    {
        Entity Mesh;
        string MeshPath = "";
        string DiffusePath = "", NormalPath = "";

        public TreeEditorMeshTrunk()
        {

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

            if (Program.GE.SetWorldButtonSelection == (int)GE.WorldButtonSelection.SCALE)
            {
                Program.Transformer = new ScaleTool(Mesh, new EventHandler(Moving), new EventHandler(Moved));
            }

            if (Program.GE.SetWorldButtonSelection == (int)GE.WorldButtonSelection.ROTATE)
            {
                Program.Transformer = new RotateTool(Mesh, new EventHandler(Moving), new EventHandler(Moved));
            }
        }

        public void Moved(object sender, EventArgs e)
        {
            System.Windows.Forms.TreeNode Node = Program.GE.TreeProperties.FindNode(this);
            if (Node != null)
                Node.Text = this.ToString();
        }

        public void Moving(object sender, EventArgs e)
        {
            Program.GE.TreeRender.Saved = false;
            Program.GE.TreeProperties.SuspendEvents = true;
            Program.GE.TreeProperties.PositionX.Value = Convert.ToDecimal(Mesh.X());
            Program.GE.TreeProperties.PositionY.Value = Convert.ToDecimal(Mesh.Y());
            Program.GE.TreeProperties.PositionZ.Value = Convert.ToDecimal(Mesh.Z());
            Program.GE.TreeProperties.ScaleX.Value = Convert.ToDecimal(Mesh.ScaleX());
            Program.GE.TreeProperties.ScaleY.Value = Convert.ToDecimal(Mesh.ScaleY());
            Program.GE.TreeProperties.ScaleZ.Value = Convert.ToDecimal(Mesh.ScaleZ());
            Program.GE.TreeProperties.RotationX.Value = Convert.ToDecimal(Mesh.Pitch());
            Program.GE.TreeProperties.RotationY.Value = Convert.ToDecimal(Mesh.Yaw());
            Program.GE.TreeProperties.RotationZ.Value = Convert.ToDecimal(Mesh.Roll());
            Program.GE.TreeProperties.SuspendEvents = false;
        }

        public string MeshSrc
        {
            get { return MeshPath; }
            set
            {
                Program.GE.TreeRender.Saved = false;
                MeshPath = value;

                if (Mesh != null)
                {
                    Mesh.Free();
                    Mesh = null;
                }

                Mesh = Entity.LoadMesh(MeshPath);
                if (Mesh == null)
                    throw new Exception("Could not load mesh file: " + value);
                else
                {
                    Mesh.Shader = Program.TrunkEditorShader;
                    RenderWrapper.EntityConstantFloat(Mesh.Handle, "Selected", 0.0f);

                    Collision.SetCollisionMesh(Mesh);
                    Collision.EntityPickMode(Mesh, PickMode.Polygon);

                    if (Program.ActiveTree != null)
                        Program.ActiveTree.UpdateNode(this);
                }

            }
        }

        public override bool HasInternalEntity(Entity entity)
        {
            if (Mesh != null && Mesh.Handle == entity.Handle)
                return true;
            return false;
        }

        public override void RecalculateBoundingBox(ref TreeBoundingBox box)
        {
            if (Mesh != null)
            {
//                 Vector3 T = new Vector3(Mesh.MeshWidth * Mesh.ScaleX(), Mesh.MeshHeight * Mesh.ScaleY(), Mesh.MeshDepth * Mesh.ScaleZ());
//                 T.X -= Mesh.X();
//                 T.Y -= Mesh.Y();
//                 T.Z -= Mesh.Z();
                Vector3 T1 = new Vector3(
                    RenderWrapper.bbdx2_GetTransformedBoundingBoxMinX(Mesh.Handle),
                    RenderWrapper.bbdx2_GetTransformedBoundingBoxMinY(Mesh.Handle),
                    RenderWrapper.bbdx2_GetTransformedBoundingBoxMinZ(Mesh.Handle));

                Vector3 T2 = new Vector3(
                    RenderWrapper.bbdx2_GetTransformedBoundingBoxMaxX(Mesh.Handle),
                    RenderWrapper.bbdx2_GetTransformedBoundingBoxMaxY(Mesh.Handle),
                    RenderWrapper.bbdx2_GetTransformedBoundingBoxMaxZ(Mesh.Handle));


                box.AddPoint(T1);
                box.AddPoint(T2);
            }
        }

        public override void StartLODRender(float angle, ref TreeBoundingBox box)
        {
            if (Mesh != null)
            {
                Mesh.Shader = Program.TrunkEditorShaderLOD;
                RenderWrapper.EntityConstantFloat(Mesh.Handle, "Angle", angle);
                RenderWrapper.EntityConstantFloat3(Mesh.Handle, "BoxMin", box.Min.X, box.Min.Y, box.Min.Z);
                RenderWrapper.EntityConstantFloat3(Mesh.Handle, "BoxMax", box.Max.X, box.Max.Y, box.Max.Z);
            }
        }

        public override void EndLODRender()
        {
            if (Mesh != null)
                Mesh.Shader = Program.TrunkEditorShader;
        }

        public override void Read(System.Xml.XmlTextReader x)
        {
            MeshSrc = x.GetAttribute("src");

            UpdateDiffuseTexture(x.GetAttribute("diffuse"));
            UpdateNormalTexture(x.GetAttribute("normal"));

            UpdatePosition(Convert.ToSingle(x.GetAttribute("x")),
                Convert.ToSingle(x.GetAttribute("y")),
                Convert.ToSingle(x.GetAttribute("z")));

            UpdateScale(Convert.ToSingle(x.GetAttribute("sx")),
                Convert.ToSingle(x.GetAttribute("sy")),
                Convert.ToSingle(x.GetAttribute("sz")));

            UpdateRotation(Convert.ToSingle(x.GetAttribute("pitch")),
                Convert.ToSingle(x.GetAttribute("yaw")),
                Convert.ToSingle(x.GetAttribute("roll")));
        }

        public override void Write(System.Xml.XmlTextWriter x)
        {
            x.WriteStartElement("trunkmesh");

            x.WriteAttributeString("src", MeshPath);
            x.WriteAttributeString("diffuse", DiffusePath);
            x.WriteAttributeString("normal", NormalPath);
            x.WriteAttributeString("x", Mesh.X().ToString());
            x.WriteAttributeString("y", Mesh.Y().ToString());
            x.WriteAttributeString("z", Mesh.Z().ToString());
            x.WriteAttributeString("sx", Mesh.ScaleX().ToString());
            x.WriteAttributeString("sy", Mesh.ScaleY().ToString());
            x.WriteAttributeString("sz", Mesh.ScaleZ().ToString());
            x.WriteAttributeString("pitch", Mesh.Pitch().ToString());
            x.WriteAttributeString("yaw", Mesh.Yaw().ToString());
            x.WriteAttributeString("roll", Mesh.Roll().ToString());

            x.WriteEndElement();
        }

        public override void Export(System.IO.BinaryWriter writer, string outputDir, string name, int index)
        {
            if(DiffusePath.Length > 0)
                File.Copy(DiffusePath, outputDir + name + "_trunk" + index.ToString() + "_diffuse" + Path.GetExtension(DiffusePath), true);

            if(NormalPath.Length > 0)
                File.Copy(NormalPath, outputDir + name + "_trunk" + index.ToString() + "_normal" + Path.GetExtension(NormalPath), true);

            Program.WriteString(writer, name + "_trunk" + index.ToString() + "_diffuse" + Path.GetExtension(DiffusePath));
            Program.WriteString(writer, name + "_trunk" + index.ToString() + "_normal" + Path.GetExtension(NormalPath));

            if(Mesh == null)
            {
                writer.Write(Convert.ToInt32(0));
                writer.Write(Convert.ToInt32(0));
                return;
            }

            int SurfaceCount = Mesh.CountSurfaces();
            int VertexCount = 0;
            int IndexCount = 0;

            uint Surface = Mesh.GetSurface(1);
            VertexCount = Entity.CountVertices(Surface);
            IndexCount = RenderWrapper.CountIndices(Surface);

            writer.Write(VertexCount);

            float PosX = Mesh.X();
            float PosY = Mesh.Y();
            float PosZ = Mesh.Z();
            float ScaX = Mesh.ScaleX();
            float ScaY = Mesh.ScaleY();
            float ScaZ = Mesh.ScaleZ();

            for (int i = 0; i < VertexCount; ++i)
            {
                Entity.TFormVector(Entity.VertexX(Surface, i), Entity.VertexY(Surface, i), Entity.VertexZ(Surface, i), Mesh, null);

                writer.Write(Entity.TFormedX());
                writer.Write(Entity.TFormedY());
                writer.Write(Entity.TFormedZ());

                writer.Write(RenderWrapper.VertexU(Surface, i));
                writer.Write(RenderWrapper.VertexV(Surface, i));
            }

            writer.Write(IndexCount / 3);

            for (int i = 0; i < IndexCount; i += 3)
            {
                writer.Write(Convert.ToUInt16(RenderWrapper.GetIndex(Surface, i + 0)));
                writer.Write(Convert.ToUInt16(RenderWrapper.GetIndex(Surface, i + 1)));
                writer.Write(Convert.ToUInt16(RenderWrapper.GetIndex(Surface, i + 2)));
            }
        }

        public override void UpdatePosition(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;
            if (Mesh == null)
                return;

            Mesh.Position(x, y, z);
        }

        public override void UpdateRotation(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;
            if (Mesh == null)
                return;

            Mesh.Rotate(x, y, z);
        }

        public override void UpdateScale(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;
            if (Mesh == null)
                return;

            Mesh.Scale(x, y, z);
        }

        public override void UpdateDiffuseTexture(string path)
        {
            Program.GE.TreeRender.Saved = false;
            if (path.Length == 0)
                return;

            DiffusePath = System.IO.Path.GetFullPath(path);

            if (Mesh == null)
                return;

            uint Texture = Render.LoadTexture(DiffusePath);
            Mesh.Texture(Texture, 0);
            Render.FreeTexture(Texture);

            Program.GE.TreeProperties.SuspendEvents = true;
            UpdateProperties(Program.GE.TreeProperties);
            Program.GE.TreeProperties.SuspendEvents = false;
        }

        public override void UpdateNormalTexture(string path)
        {
            Program.GE.TreeRender.Saved = false;
            if (path.Length == 0)
                return;

            NormalPath = System.IO.Path.GetFullPath(path);

            if (Mesh == null)
                return;

            uint Texture = Render.LoadTexture(NormalPath);
            Mesh.Texture(Texture, 1);
            Render.FreeTexture(Texture);

            Program.GE.TreeProperties.SuspendEvents = true;
            UpdateProperties(Program.GE.TreeProperties);
            Program.GE.TreeProperties.SuspendEvents = false;
        }

        public override void NotifySelected()
        {
            if (Mesh != null)
                RenderWrapper.EntityConstantFloat(Mesh.Handle, "Selected", 1.0f);
        }

        public override void NotifyUnselected()
        {
            if (Mesh != null)
                RenderWrapper.EntityConstantFloat(Mesh.Handle, "Selected", 0.0f);
        }

        public override void UpdateProperties(TreeEditorProperties propertiesPanel)
        {
            propertiesPanel.ClearProperties();

            propertiesPanel.PositionX.Value = Convert.ToDecimal(Mesh.X());
            propertiesPanel.PositionY.Value = Convert.ToDecimal(Mesh.Y());
            propertiesPanel.PositionZ.Value = Convert.ToDecimal(Mesh.Z());
            propertiesPanel.ScaleX.Value = Convert.ToDecimal(Mesh.ScaleX());
            propertiesPanel.ScaleY.Value = Convert.ToDecimal(Mesh.ScaleY());
            propertiesPanel.ScaleZ.Value = Convert.ToDecimal(Mesh.ScaleZ());
            propertiesPanel.RotationX.Value = Convert.ToDecimal(Mesh.Pitch());
            propertiesPanel.RotationY.Value = Convert.ToDecimal(Mesh.Yaw());
            propertiesPanel.RotationZ.Value = Convert.ToDecimal(Mesh.Roll());

            propertiesPanel.PositionX.Enabled = true;
            propertiesPanel.PositionY.Enabled = true;
            propertiesPanel.PositionZ.Enabled = true;
            propertiesPanel.ScaleX.Enabled = true;
            propertiesPanel.ScaleY.Enabled = true;
            propertiesPanel.ScaleZ.Enabled = true;
            propertiesPanel.RotationX.Enabled = true;
            propertiesPanel.RotationY.Enabled = true;
            propertiesPanel.RotationZ.Enabled = true;

            propertiesPanel.TextureDiffuse.Text = DiffusePath.Length > 0 ? System.IO.Path.GetFileName(DiffusePath) : "Not Set";
            propertiesPanel.TextureNormal.Text = NormalPath.Length > 0 ? System.IO.Path.GetFileName(NormalPath) : "Not Set";

            propertiesPanel.TextureDiffuse.Enabled = true;
            propertiesPanel.TextureNormal.Enabled = true;
        }

        public override string ToString()
        {
            return "Trunk: " + System.IO.Path.GetFileName(MeshPath);
        }
    }
}
