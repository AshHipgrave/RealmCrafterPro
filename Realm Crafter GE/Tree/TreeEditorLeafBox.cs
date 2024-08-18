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
    public class TreeEditorLeafBox : TreePropertiesUpdater
    {
        TreeEditorLeafGroup Group;
        List<Line3D> Lines = new List<Line3D>();
        List<TreeEditorLeaf> Leaves = new List<TreeEditorLeaf>();
        Vector3 Position = new Vector3(), Scale = new Vector3(1, 1, 1);
        int Count = 1;
        int Seed = 0;
        bool Selected = false;
        Entity Mesh;

        public TreeEditorLeafBox(TreeEditorLeafGroup group)
        {
            Group = group;
            Mesh = Entity.CreatePivot();

            for (int i = 0; i < 12; ++i)
                Lines.Add(new Line3D(0, 0, 0, 0, 0, 0, false));
            Rebuild();
        }

        public override void Show()
        {
            foreach (Line3D L in Lines)
                L.Visible = true;
        }

        public override void Hide()
        {
            foreach (Line3D L in Lines)
                L.Visible = false;
        }

        public override void Remove()
        {
            foreach (Line3D L in Lines)
                L.Free();
            Lines.Clear();
            Mesh.Free();
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
        }

        public void Moved(object sender, EventArgs e)
        {

        }

        public void Moving(object sender, EventArgs e)
        {
            Program.GE.TreeRender.Saved = false;

            Position.X = Mesh.X();
            Position.Y = Mesh.Y();
            Position.Z = Mesh.Z();
            Scale.X = Mesh.ScaleX();
            Scale.Y = Mesh.ScaleY();
            Scale.Z = Mesh.ScaleZ();

            System.Windows.Forms.TreeNode Node = Program.GE.TreeProperties.FindNode(this);
            if (Node != null)
                Node.Text = this.ToString();

            Rebuild();

            Program.GE.TreeProperties.SuspendEvents = true;
            Program.GE.TreeProperties.PositionX.Value = Convert.ToDecimal(Position.X);
            Program.GE.TreeProperties.PositionY.Value = Convert.ToDecimal(Position.Y);
            Program.GE.TreeProperties.PositionZ.Value = Convert.ToDecimal(Position.Z);
            Program.GE.TreeProperties.ScaleX.Value = Convert.ToDecimal(Scale.X);
            Program.GE.TreeProperties.ScaleY.Value = Convert.ToDecimal(Scale.Y);
            Program.GE.TreeProperties.ScaleZ.Value = Convert.ToDecimal(Scale.Z);
            Program.GE.TreeProperties.SuspendEvents = false;
        }

        public override void Read(System.Xml.XmlTextReader x)
        {
            UpdatePosition(Convert.ToSingle(x.GetAttribute("x")),
                Convert.ToSingle(x.GetAttribute("y")),
                Convert.ToSingle(x.GetAttribute("z")));

            UpdateScale(Convert.ToSingle(x.GetAttribute("sx")),
                Convert.ToSingle(x.GetAttribute("sy")),
                Convert.ToSingle(x.GetAttribute("sz")));
        }

        public override void Write(System.Xml.XmlTextWriter x)
        {
            x.WriteStartElement("leafbox");

            x.WriteAttributeString("x", Position.X.ToString());
            x.WriteAttributeString("y", Position.Y.ToString());
            x.WriteAttributeString("z", Position.Z.ToString());
            x.WriteAttributeString("sx", Scale.X.ToString());
            x.WriteAttributeString("sy", Scale.Y.ToString());
            x.WriteAttributeString("sz", Scale.Z.ToString());

            x.WriteEndElement();
        }

        public override void Export(System.IO.BinaryWriter writer, string outputDir, string name, int index)
        {
        }

        public void Rebuild()
        {
            if (Lines.Count == 0)
                return;

            Mesh.Position(Position.X, Position.Y, Position.Z);
            Mesh.Scale(Scale.X, Scale.Y, Scale.Z);

            float HalfX = Scale.X * 0.5f;
            float HalfY = Scale.Y * 0.5f;
            float HalfZ = Scale.Z * 0.5f;
            float MinX = Position.X - HalfX;
            float MinY = Position.Y - HalfY;
            float MinZ = Position.Z - HalfZ;
            float MaxX = Position.X + HalfX;
            float MaxY = Position.Y + HalfY;
            float MaxZ = Position.Z + HalfZ;

            Lines[0].SetPositions(MinX, MinY, MinZ, MaxX, MinY, MinZ);
            Lines[1].SetPositions(MinX, MinY, MinZ, MinX, MinY, MaxZ);
            Lines[2].SetPositions(MaxX, MinY, MinZ, MaxX, MinY, MaxZ);
            Lines[3].SetPositions(MinX, MinY, MaxZ, MaxX, MinY, MaxZ);

            Lines[4].SetPositions(MinX, MaxY, MinZ, MaxX, MaxY, MinZ);
            Lines[5].SetPositions(MinX, MaxY, MinZ, MinX, MaxY, MaxZ);
            Lines[6].SetPositions(MaxX, MaxY, MinZ, MaxX, MaxY, MaxZ);
            Lines[7].SetPositions(MinX, MaxY, MaxZ, MaxX, MaxY, MaxZ);

            Lines[8].SetPositions(MinX, MinY, MinZ, MinX, MaxY, MinZ);
            Lines[9].SetPositions(MaxX, MinY, MinZ, MaxX, MaxY, MinZ);
            Lines[10].SetPositions(MinX, MinY, MaxZ, MinX, MaxY, MaxZ);
            Lines[11].SetPositions(MaxX, MinY, MaxZ, MaxX, MaxY, MaxZ);

            if (Selected)
            {
                foreach (Line3D L in Lines)
                    L.SetColor(255, 0, 0);
            }
            else
            {
                foreach (Line3D L in Lines)
                    L.SetColor(255, 255, 255);
            }

            foreach (TreeEditorLeaf L in Leaves)
                L.Remove();
            Leaves.Clear();

            Random R = new Random(Seed);

            for (int i = 0; i < Count; ++i)
            {
                TreeEditorLeaf L = new TreeEditorLeaf(Group);
                L.Position = new Vector3(
                    (Convert.ToSingle(R.NextDouble()) * Scale.X) + (Position.X - (Scale.X * 0.5f)),
                    (Convert.ToSingle(R.NextDouble()) * Scale.Y) + (Position.Y - (Scale.Y * 0.5f)),
                    (Convert.ToSingle(R.NextDouble()) * Scale.Z) + (Position.Z - (Scale.Z * 0.5f)));

                L.ReBuild();

                Leaves.Add(L);
            }
        }

        public void FinalizeLeaves()
        {
            foreach (TreeEditorLeaf L in Leaves)
            {
                Group.Leaves.Add(L);
            }
            Leaves.Clear();
            Group.Placers.Remove(this);
            Remove();
            Program.ActiveTree.UpdatePropertyPanel();
        }

        public override void UpdatePosition(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;
            Position.X = x;
            Position.Y = y;
            Position.Z = z;
            Rebuild();
        }

        public override void UpdateScale(float x, float y, float z)
        {
            Program.GE.TreeRender.Saved = false;
            Scale.X = x;
            Scale.Y = y;
            Scale.Z = z;
            Rebuild();
        }

        public override void UpdateCount(int count)
        {
            Count = count;
            Rebuild();
        }

        public override void UpdateSeed(int seed)
        {
            Seed = seed;
            Rebuild();
        }

        public override void NotifySelected()
        {
            Selected = true;
            Rebuild();
        }

        public override void NotifyUnselected()
        {
            Selected = false;
            Rebuild();
        }

        public override void UpdateProperties(TreeEditorProperties propertiesPanel)
        {
            propertiesPanel.ClearProperties();
            propertiesPanel.SwitchToLeafPlacement();

            propertiesPanel.PositionX.Value = Convert.ToDecimal(Position.X);
            propertiesPanel.PositionY.Value = Convert.ToDecimal(Position.Y);
            propertiesPanel.PositionZ.Value = Convert.ToDecimal(Position.Z);
            propertiesPanel.ScaleX.Value = Convert.ToDecimal(Scale.X);
            propertiesPanel.ScaleY.Value = Convert.ToDecimal(Scale.Y);
            propertiesPanel.ScaleZ.Value = Convert.ToDecimal(Scale.Z);
            propertiesPanel.LeafCount.Value = Convert.ToDecimal(Count);
            propertiesPanel.Seed.Value = Convert.ToDecimal(Seed);

            propertiesPanel.PositionX.Enabled = true;
            propertiesPanel.PositionY.Enabled = true;
            propertiesPanel.PositionZ.Enabled = true;
            propertiesPanel.ScaleX.Enabled = true;
            propertiesPanel.ScaleY.Enabled = true;
            propertiesPanel.ScaleZ.Enabled = true;
        }

        public override string ToString()
        {
            return "Auto Placer";
        }
    }
}
