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
    public class TreeEditorLeafGroup : TreePropertiesUpdater
    {
        public List<TreeEditorLeaf> Leaves = new List<TreeEditorLeaf>();
        public List<TreeEditorLeafBox> Placers = new List<TreeEditorLeafBox>();
        public float Scale = 1.0f, LastScale = 1.0f;
        string DiffusePath = "", NormalPath = "";
        public uint DiffuseHandle = 0, NormalHandle = 0;
        public Entity Mesh;

        public TreeEditorLeafGroup()
        {
            DiffuseHandle = Render.LoadTexture("BBDXINTERNAL");
            NormalHandle = Render.LoadTexture("BBDXINTERNAL");
        }

        public override void UpdateWorldButtonSelection()
        {
            if (Program.Transformer != null)
                Program.Transformer.Free();
            Program.Transformer = null;

            if (Program.GE.SetWorldButtonSelection == (int)GE.WorldButtonSelection.SCALE)
            {
                Entity P = Entity.CreatePivot();
                P.Scale(Scale, Scale, Scale);
                LastScale = Scale;
                Program.Transformer = new ScaleTool(P, new EventHandler(Moving), new EventHandler(Moved));
            }
        }

        public void Moved(object sender, EventArgs e)
        {
            (Program.Transformer as ScaleTool).Parent.Scale(Scale, Scale, Scale);
            LastScale = Scale;
        }

        public void Moving(object sender, EventArgs e)
        {
            if ((Program.Transformer as ScaleTool) != null)
            {
                float sx = (Program.Transformer as ScaleTool).Parent.ScaleX();
                float sy = (Program.Transformer as ScaleTool).Parent.ScaleY();
                float sz = (Program.Transformer as ScaleTool).Parent.ScaleZ();

                if (sx != LastScale)
                    UpdateScale(sx, 1.0f, 1.0f);
                else if (sy != LastScale)
                    UpdateScale(sy, 1.0f, 1.0f);
                else if (sz != LastScale)
                    UpdateScale(sz, 1.0f, 1.0f);

                Program.GE.TreeProperties.SuspendEvents = true;
                Program.GE.TreeProperties.ScaleX.Value = Convert.ToDecimal(Scale);
                Program.GE.TreeProperties.SuspendEvents = false;
                //(Program.Transformer as ScaleTool).Parent.Scale(Scale, Scale, Scale);
                //LastScale = Scale;
            }
        }

        public override void RecalculateBoundingBox(ref TreeBoundingBox box)
        {
            foreach (TreeEditorLeaf L in Leaves)
            {
                L.RecalculateBoundingBox(ref box);
            }
        }

        public override void StartLODRender(float angle, ref TreeBoundingBox box)
        {
            foreach (TreeEditorLeaf L in Leaves)
            {
                L.StartLODRender(angle, ref box);
            }
        }

        public override void EndLODRender()
        {
            foreach (TreeEditorLeaf L in Leaves)
            {
                L.EndLODRender();
            }
        }

        public override void Remove()
        {
            foreach (TreeEditorLeaf L in Leaves)
            {
                if (L.Mesh != null)
                    L.Mesh.Free();
                L.Mesh = null;
            }
            Leaves.Clear();

            if (NormalHandle != 0)
                Render.FreeTexture(NormalHandle);
            if (DiffuseHandle != 0)
                Render.FreeTexture(DiffuseHandle);
            NormalHandle = 0;
            DiffuseHandle = 0;
        }

        public override void Show()
        {
            foreach (TreeEditorLeaf L in Leaves)
                L.Show();
            foreach (TreeEditorLeafBox L in Placers)
                L.Show();
        }

        public override void Hide()
        {
            foreach (TreeEditorLeaf L in Leaves)
                L.Hide();
            foreach (TreeEditorLeafBox L in Placers)
                L.Hide();
        }

        public override void Read(System.Xml.XmlTextReader x)
        {
            UpdateDiffuseTexture(x.GetAttribute("diffuse"));
            UpdateNormalTexture(x.GetAttribute("normal"));

            Scale = Convert.ToSingle(x.GetAttribute("scale"));
        }

        public override void Write(System.Xml.XmlTextWriter x)
        {
            x.WriteStartElement("leafgroup");

            x.WriteAttributeString("scale", Scale.ToString());
            x.WriteAttributeString("diffuse", DiffusePath);
            x.WriteAttributeString("normal", NormalPath);

            foreach (TreeEditorLeaf L in Leaves)
                L.Write(x);
            foreach (TreeEditorLeafBox L in Placers)
                L.Write(x);

            x.WriteEndElement();
        }

        public override void Export(System.IO.BinaryWriter writer, string outputDir, string name, int index)
        {
            if(DiffusePath.Length > 0)
                File.Copy(DiffusePath, outputDir + name + "_leaf" + index.ToString() + "_diffuse" + Path.GetExtension(DiffusePath), true);

            if(NormalPath.Length > 0)
                File.Copy(NormalPath, outputDir + name + "_leaf" + index.ToString() + "_normal" + Path.GetExtension(NormalPath), true);

            Program.WriteString(writer, name + "_leaf" + index.ToString() + "_diffuse" + Path.GetExtension(DiffusePath));
            Program.WriteString(writer, name + "_leaf" + index.ToString() + "_normal" + Path.GetExtension(NormalPath));

            writer.Write(Scale);
            writer.Write(Leaves.Count);
            foreach (TreeEditorLeaf L in Leaves)
            {
                L.Export(writer, outputDir, name, index);
            }
        }

        public override void UpdateScale(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;

            Scale = x;

            foreach (TreeEditorLeaf L in Leaves)
                L.ReBuild();
            foreach (TreeEditorLeafBox L in Placers)
                L.Rebuild();
        }

        public override void UpdateDiffuseTexture(string path)
        {
            Program.GE.TreeRender.Saved = false;

            if (path.Length == 0)
                return;

            if(DiffuseHandle != 0)
                Render.FreeTexture(DiffuseHandle);
            DiffusePath = System.IO.Path.GetFullPath(path);
            DiffuseHandle = Render.LoadTexture(DiffusePath, 8);

            Program.GE.TreeProperties.SuspendEvents = true;
            UpdateProperties(Program.GE.TreeProperties);
            Program.GE.TreeProperties.SuspendEvents = false;

            foreach (TreeEditorLeaf L in Leaves)
                L.ReBuild();
            foreach (TreeEditorLeafBox L in Placers)
                L.Rebuild();
            
        }

        public override void UpdateNormalTexture(string path)
        {
            Program.GE.TreeRender.Saved = false;

            if (path.Length == 0)
                return;

            if (NormalHandle != 0)
                Render.FreeTexture(NormalHandle);
            NormalPath = System.IO.Path.GetFullPath(path);
            NormalHandle = Render.LoadTexture(NormalPath);

            Program.GE.TreeProperties.SuspendEvents = true;
            UpdateProperties(Program.GE.TreeProperties);
            Program.GE.TreeProperties.SuspendEvents = false;

            foreach (TreeEditorLeaf L in Leaves)
                L.ReBuild();
            foreach (TreeEditorLeafBox L in Placers)
                L.Rebuild();
            
        }

        public override void UpdateProperties(TreeEditorProperties propertiesPanel)
        {
            propertiesPanel.ClearProperties();

            propertiesPanel.ScaleX.Value = Convert.ToDecimal(Scale);
            propertiesPanel.ScaleX.Enabled = true;

            propertiesPanel.TextureDiffuse.Text = DiffusePath.Length > 0 ? System.IO.Path.GetFileName(DiffusePath) : "Not Set";
            propertiesPanel.TextureNormal.Text = NormalPath.Length > 0 ? System.IO.Path.GetFileName(NormalPath) : "Not Set";

            propertiesPanel.TextureDiffuse.Enabled = true;
            propertiesPanel.TextureNormal.Enabled = true;
        }

        public override string ToString()
        {
            return "Leaf Group: " + System.IO.Path.GetFileName(DiffusePath);
        }
    }
}
