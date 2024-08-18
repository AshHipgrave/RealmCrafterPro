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
using System.IO;
using Scripting;
using RenderingServices;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class FormEditor : ScriptEditorForm
    {
        public string loadedFileName;
        public bool saved = true;
        public List<EditorControlInstance> AllControls = new List<EditorControlInstance>();
        public EditorControlInstance Master;
        public EditorControlInstance SelectedControl = null;
        public Vector2 SelectedPosition = new Vector2(), SelectedSize = new Vector2();

        public FormEditor()
        {
            InitializeComponent();
          

            
        }

        public bool LoadFile(string filename)
        {
            AllControls = new List<EditorControlInstance>();
            Master = EditorControlInstance.Load(@"Data\Server Data\Scripts\" + Path.GetFileNameWithoutExtension(filename) + ".xml", AllControls);
            Master.Editor = this;

// 
//             ControlProperties Props = (ControlProperties)Activator.CreateInstance(AllControls[1].EditorControl.PropertyEditorType);
// 
//             Props.Parent = AllControls[1];
// 
//             Program.GE.m_ScriptView.m_Properties.PropertyGrid.SelectedObject = Props;

            return true;
        }

        public override void SaveFile()
        {
            string file = loadedFileName.Length > 11 ? loadedFileName.Substring(0, loadedFileName.Length - 11) : loadedFileName;

            foreach (ScriptEditorForm documentForm in Program.GE.m_ScriptView.ScriptDock.Documents)
            {    
                if (file == documentForm.TabText || file + " *" == documentForm.TabText)
                {
                    documentForm.SaveFile();
                    break;
                }
            }

            if (Master != null)
            {
                Master.Save(@"Data\Server Data\Scripts\" + Path.GetFileNameWithoutExtension(file) + ".xml",
                    @"Data\Server Data\Scripts\" + Path.GetFileNameWithoutExtension(file) + ".designer.cs",
                    @"Data\Server Data\Scripts\" + file);

                SetSaved(true);
            }
        }

        public void SelectControl(EditorControlInstance instance)
        {
            for (int i = 0; i < AllControls.Count; ++i)
            {
                if (AllControls[i] == instance)
                {
                    SelectControl(i);
                    return;
                }
            }
        }

        public void SelectControl(int index)
        {
            if(Program.GE.m_ScriptView.m_Properties == null)
                Program.GE.m_ScriptView.m_Properties = new EditorProperties();

                if (SelectedControl != null)
                {
                    // Free anything related to it
                    Program.GE.m_ScriptView.m_Properties.PropertyGrid.SelectedObject = null;
                }

                Program.GE.m_ScriptView.m_Properties.ControlsList.SelectedIndex = index;

                if (index < 0 || index > AllControls.Count)
                    return;

                SelectedControl = AllControls[index];
                ControlProperties Props = (ControlProperties)Activator.CreateInstance(SelectedControl.EditorControl.PropertyEditorType);
                Props.Parent = SelectedControl;

                Program.GE.m_ScriptView.m_Properties.PropertyGrid.SelectedObject = Props;

                Vector2 Pos = AllControls[index].GetAbsolutePosition();
                Vector2 Siz = AllControls[index].GetAbsoluteSize();
                SelectedPosition = Pos;
                SelectedSize = Siz;

                PacketWriter SPa = new PacketWriter();
                SPa.Write(1001);
                SPa.Write("VSBL", false);
                SPa.Write((byte)1);
                SPa.Write(1001);
                SPa.Write("ZZLC", false);
                SPa.Write(Pos.X);
                SPa.Write(Pos.Y);
                SPa.Write(1001);
                SPa.Write("ZZSZ", false);
                SPa.Write(Siz.X);
                SPa.Write(Siz.Y);
                SDKNet.ControlHost.ProcessUpdateProperties(SPa.ToArray());

        }

        public void SetupGUI()
        {
            if (Master != null)
            {
                PacketWriter Pa = new PacketWriter();
                int AllocID = 0;
                Master.CreatePacket(Pa, ref AllocID);

                SDKNet.ControlHost.ProcessFormOpen(Pa.ToArray());
            }

            PacketWriter SPa = new PacketWriter();
            SPa.Write("CSEL", false);
            SPa.Write("ALID", false);
            SPa.Write(1001);
            SPa.Write("VSBL", false);
            SPa.Write((byte)0);
            SPa.Write("ENDL", false);
            SDKNet.ControlHost.ProcessFormOpen(SPa.ToArray());
        }

        public void RemoveGUI()
        {
            if (Master != null)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(Master.AllocID);
                SDKNet.ControlHost.ProcessFormClosed(Pa.ToArray());
            }

            PacketWriter SPa = new PacketWriter();
            SPa.Write(1001);
            SDKNet.ControlHost.ProcessFormClosed(SPa.ToArray());
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = DragDropEffects.None;

            string[] s = drgevent.Data.GetFormats();

            for (int i = 0; i < s.Length; ++i)
            {
                object D = drgevent.Data.GetData(s[i]);

                if ((D as EditorControl) != null)
                    drgevent.Effect = DragDropEffects.All;
            }

            base.OnDragEnter(drgevent);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            string[] s = drgevent.Data.GetFormats();

            for (int i = 0; i < s.Length; ++i)
            {
                EditorControl Command = drgevent.Data.GetData(s[i]) as EditorControl;

                if (Command != null)
                {
                    // Get mouse position
                    Point MousePos = PointToClient(Cursor.Position);

                    // Find control to drop onto
                    if (Master != null)
                    {
                        EditorControlInstance Parent = Master.FindPlacementParent(MousePos);

                        if (Parent != null)
                        {
                            // Find a new name
                            int Cnt = 1;
                            foreach (EditorControlInstance C in AllControls)
                            {
                                if (C.EditorControl == Command)
                                    ++Cnt;
                            }
                            string NewName = Command.DefaultName + Cnt.ToString();
                            
                            // Create control
                            EditorControlInstance Inst = new EditorControlInstance(Command);

                            // Set name property
                            EditorPropertyInstance NameProperty = Inst.FindPropertyInstance("name");
                            if (NameProperty != null)
                                NameProperty.Value = NewName.ToString();
                            else
                                throw new Exception("Property 'name' not found on SDK GUI Control '" + Command.Name + "'");

                            // Calculate its position
                            Vector2 PosAbs = new Vector2((float)MousePos.X, (float)MousePos.Y);
                            Vector2 PosRel = new Vector2((float)(MousePos.X / Program.GE.RenderingPanel.Width),
                                (float)(MousePos.Y / Program.GE.RenderingPanel.Height));

                            Vector2 ParentAbs = Parent.GetAbsolutePosition();
                            Vector2 ParentRel = (Vector2)ParentAbs.Clone();

                            ParentRel.X /= (float)Program.GE.RenderingPanel.Width;
                            ParentRel.Y /= (float)Program.GE.RenderingPanel.Height;

                            PosAbs.X -= ParentAbs.X;
                            PosAbs.Y -= ParentAbs.Y;
                            PosRel.X -= ParentRel.X;
                            PosRel.Y -= ParentRel.Y;

                            // Set position
                            EditorPropertyInstance LocationProperty = Inst.FindPropertyInstance("location");
                            EditorPropertyInstance PositionTypeProperty = Inst.FindPropertyInstance("positiontype");

                            if (LocationProperty != null && PositionTypeProperty != null)
                            {
                                PositionType PType = (PositionType)PositionTypeProperty.Value;

                                if (PType == PositionType.Absolute)
                                    LocationProperty.Value = PosAbs.Clone();
                                else if (PType == PositionType.Relative)
                                    LocationProperty.Value = PosRel.Clone();
                            }

                            // Finalize control
                            Parent.Children.Add(Inst);
                            Inst.Parent = Parent;
                            Inst.Master = Master;
                            AllControls.Add(Inst);

                            Program.GE.m_ScriptView.m_Properties.ControlsList.Items.Add(NewName);

                            SetSaved(false);

                            // Reset GUI display
                            RemoveGUI();
                            SetupGUI();

                            SelectControl(AllControls.Count - 1);
                        }
                    }

                    
                }
            }

            base.OnDragDrop(drgevent);
        }

        public override string LoadedFileName
        {
            get { return loadedFileName; }
            set { loadedFileName = value; }
        }
        
        public override bool Saved
        {
            get { return saved; }
            set { saved = value; }
        }

        public override void SetSaved(bool State)
        {
            if (Saved == State)
            {
                return;
            }
            if (State == false)
            {
                this.TabText = LoadedFileName + " *";
                Saved = false;
            }
            else
            {
                this.TabText = LoadedFileName;
                Saved = true;
            }
        }

        private void SelectionPanel_Paint(object sender, PaintEventArgs e)
        {
//             e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0, 0)), e.ClipRectangle);
//             e.Graphics.FillRectangle(Brushes.Red, 10, 10, 10, 10);
        }

        private void viewCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = loadedFileName.Length > 11 ? loadedFileName.Substring(0, loadedFileName.Length - 11) : loadedFileName;

            Program.GE.m_ScriptView.OpenSpecifiedScript(file);
        }

        private void bringToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedControl != null)
            {
                if (SelectedControl.Parent != null)
                {
                    SelectedControl.Parent.Children.Remove(SelectedControl);
                    SelectedControl.Parent.Children.Add(SelectedControl);

                    RemoveGUI();
                    SetupGUI();

                    SelectControl(SelectedControl);
                    SetSaved(false);
                }
            }
        }

        private void sendToBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedControl != null)
            {
                if (SelectedControl.Parent != null)
                {
                    SelectedControl.Parent.Children.Remove(SelectedControl);
                    SelectedControl.Parent.Children.Insert(0, SelectedControl);

                    RemoveGUI();
                    SetupGUI();

                    SelectControl(SelectedControl);
                    SetSaved(false);
                }
            }
        }

        public void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedControl != null)
            {
                // Can't delete master!
                if (SelectedControl == Master)
                    return;

                if (SelectedControl.Parent != null)
                    SelectedControl.Parent.Children.Remove(SelectedControl);
                SelectedControl.Parent = null;
                AllControls.Remove(SelectedControl);
                SelectedControl = null;
                Program.GE.m_ScriptView.m_Properties.PropertyGrid.SelectedObject = null;

                Program.GE.m_ScriptView.m_Properties.ControlsList.Items.Clear();
                foreach (EditorControlInstance C in AllControls)
                {
                    EditorPropertyInstance P = C.FindPropertyInstance("name");
                    if (P != null)
                        Program.GE.m_ScriptView.m_Properties.ControlsList.Items.Add(P.Value.ToString());
                    else
                        Program.GE.m_ScriptView.m_Properties.ControlsList.Items.Add("ERROR");
                }

                RemoveGUI();
                SetupGUI();
                SetSaved(false);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                deleteToolStripMenuItem_Click(this, null);

                return;
            }
            base.OnKeyUp(e);


        }

        private void FormEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.GE.RenderingPanel.Parent == this)
            {
                this.Controls.Remove(Program.GE.RenderingPanel);
                Program.GE.RenderingPanel.Parent = null;
            }
        }
    }
}