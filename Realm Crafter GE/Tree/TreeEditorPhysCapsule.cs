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
    public class TreeEditorPhysCapsule : TreeEditorPhysObj
    {
        Entity Mesh;

        public TreeEditorPhysCapsule()
        {
            Mesh = Program.TreeCapsule.Copy();
            Mesh.Visible = true;
            Mesh.Shader = Program.PhysicsEditorShader;
            Mesh.Alpha(0.5f);
            RenderWrapper.EntityConstantFloat(Mesh.Handle, "Selected", 0.0f);
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
                (Program.Transformer as ScaleTool).AllowScaleZ = false;
                (Program.Transformer as ScaleTool).CopyXToZ = true;
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
            Program.GE.TreeProperties.SuspendEvents = false;
        }

        public override void Read(System.Xml.XmlTextReader x)
        {
            UpdatePosition(Convert.ToSingle(x.GetAttribute("x")),
                Convert.ToSingle(x.GetAttribute("y")),
                Convert.ToSingle(x.GetAttribute("z")));

            UpdateScale(Convert.ToSingle(x.GetAttribute("sx")),
                Convert.ToSingle(x.GetAttribute("sy")),
                0.0f);
        }

        public override void Write(System.Xml.XmlTextWriter x)
        {
            x.WriteStartElement("physcapsule");

            x.WriteAttributeString("x", Mesh.X().ToString());
            x.WriteAttributeString("y", Mesh.Y().ToString());
            x.WriteAttributeString("z", Mesh.Z().ToString());
            x.WriteAttributeString("sx", Mesh.ScaleX().ToString());
            x.WriteAttributeString("sy", Mesh.ScaleY().ToString());

            x.WriteEndElement();
        }

        public override void Export(System.IO.BinaryWriter writer, string outputDir, string name, int index)
        {
            writer.Write(Convert.ToByte(1));

            if (Mesh == null)
            {
                writer.Write(0.0f);
                writer.Write(0.0f);
                writer.Write(0.0f);
                writer.Write(0.0f);
                writer.Write(0.0f);
                writer.Write(0.0f);
            }
            else
            {
                writer.Write(Mesh.X());
                writer.Write(Mesh.Y());
                writer.Write(Mesh.Z());
                writer.Write(Mesh.ScaleX());
                writer.Write(Mesh.ScaleY());
                writer.Write(Mesh.ScaleX());
            }
        }

        public override void UpdatePosition(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;

            if (Mesh == null)
                return;

            Mesh.Position(x, y, z);
        }

        public override void UpdateScale(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;

            if (Mesh == null)
                return;

            Mesh.Scale(x, y, x);
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
            propertiesPanel.ScaleX.Value = Convert.ToDecimal(Mesh.ScaleX());
            propertiesPanel.ScaleY.Value = Convert.ToDecimal(Mesh.ScaleY());

            propertiesPanel.PositionX.Enabled = true;
            propertiesPanel.PositionY.Enabled = true;
            propertiesPanel.PositionZ.Enabled = true;
            propertiesPanel.ScaleX.Enabled = true;
            propertiesPanel.ScaleY.Enabled = true;
        }

        public override string ToString()
        {
            return "Capsule";
        }
    }
}
