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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RealmCrafter_GE.Dockable_Forms.ScriptEditor;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;
using System.ComponentModel;
using RenderingServices;
using Scripting;
using SizeType = RealmCrafter_GE.Dockable_Forms.ScriptEditor.SizeType;
using System.Xml;

namespace RealmCrafter_GE.Dockable_Forms
{
    public partial class SimpleScriptEditor : DockContent
    {
        public ScriptEditorForm ActiveScript
        {
            get { return ScriptDock.ActiveDocument as ScriptEditorForm; }
        }
        public enum SyntaxStyles
        {
            RCSCRIPT = 0,
            BVM = 1,
            BVMALT = 2
        }
        public string Extension = Program.ScriptExtension;
        public SyntaxStyles SyntaxHighlighting = SyntaxStyles.RCSCRIPT;
        public FormEditor CurrentForm = null;

        public SimpleScriptEditor()
        {
            InitializeComponent();
 
           
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public ProjectFiles m_ProjectExplorer = new ProjectFiles();
        public Toolbox m_ToolBox = new Toolbox();
        public EditorProperties m_Properties = new EditorProperties();

        public void LoadFile(string filename, bool formEditor)
        {
            if (!formEditor)
            {
                ScriptForm ScriptDoc = CreateNewDocument(filename);
                bool Loaded = ScriptDoc.LoadFile(filename);
                if (Loaded)
                {
                    ScriptDoc.Show(ScriptDock);
                }
            }
            else
            {
                FormEditor NewScript = new FormEditor();
                NewScript.TabText = filename + " [Designer]";
                NewScript.LoadedFileName = filename + " [Designer]";

                if (NewScript.LoadFile(filename))
                {
                    NewScript.Show(ScriptDock);
                }
            }
        }
        public void OpenSpecifiedScript(String ScriptName)
        {
            bool isOpen = false;
            foreach (ScriptEditorForm documentForm in Program.GE.m_ScriptView.ScriptDock.Documents)
            {
                string file = ScriptName;
                if (file == documentForm.TabText || file + " *" == documentForm.TabText)
                {
                    // Some how this works. Selects document
                    documentForm.Hide();
                    documentForm.Show();
                    isOpen = true;
                    break;
                }
            }

            // Open the files
            if (!isOpen)
            {
                Program.GE.m_ScriptView.LoadFile(ScriptName, false);
            }
        }

        public void FixDockSizing()
        {
            // Random ones are nulled it seems so messy ifs - Ben
            if (m_ProjectExplorer != null)
            {
                m_ProjectExplorer.Dock = DockStyle.Fill;
                if(m_ProjectExplorer.DockPanel != null)
                    m_ProjectExplorer.Size = m_ProjectExplorer.DockPanel.Size;
            }

            if (m_Properties != null)
            {
                m_Properties.Dock = DockStyle.Fill;
                if (m_Properties.DockPanel != null)
                    m_Properties.Size = m_Properties.DockPanel.Size;
            }

            if (m_ToolBox != null)
            {
                m_ToolBox.Dock = DockStyle.Fill;
                if (m_ToolBox.DockPanel != null)
                    m_ToolBox.Size = m_ToolBox.DockPanel.Size;
            }


            this.Dock = DockStyle.Fill;
            this.Size = this.DockPanel.Size;          



        }

        public void UpdateRenderingPanel(bool editorIsVisible)
        {
            if (editorIsVisible && ScriptDock.ActiveDocument is FormEditor)
            {
                //if (!ScriptDock.ActiveDocument.DockHandler.Form.IsDisposed && !Program.GE.RenderingPanel.IsDisposed)
                if (CurrentForm == null || Program.GE.RenderingPanel.Parent != ScriptDock.ActiveDocument.DockHandler.Form)
                {
                    Program.GE.RenderingPanel.Visible = true;
                    Program.GE.RenderingPanel.Parent = ScriptDock.ActiveDocument.DockHandler.Form;
                    Program.GE.RenderingPanel.Dock = DockStyle.Fill;

                    Render.Graphics3D(Program.GE.RenderingPanel.Width, Program.GE.RenderingPanel.Height, 32, 2, 0, 0,
                              @".\Data\DefaultTex.png");

                    if ((ScriptDock.ActiveDocument as FormEditor) != CurrentForm)
                    {
                        if (CurrentForm != null)
                        {
                            CurrentForm.RemoveGUI();
                            CurrentForm.SizeChanged -= new EventHandler(RenderingPanel_SizeChanged);
                        }

                        CurrentForm = (ScriptDock.ActiveDocument as FormEditor);
                        CurrentForm.SizeChanged += new EventHandler(RenderingPanel_SizeChanged);
                        CurrentForm.SetupGUI();

                        m_Properties.ControlsList.Items.Clear();
                        foreach (EditorControlInstance C in CurrentForm.AllControls)
                        {
                            EditorPropertyInstance P = C.FindPropertyInstance("name");
                            if (P != null)
                                m_Properties.ControlsList.Items.Add(P.Value.ToString());
                            else
                                m_Properties.ControlsList.Items.Add("ERROR");
                        }

                    }
                }
            }
            else
            {
                if (CurrentForm != null)
                {
                    CurrentForm.RemoveGUI();
                    CurrentForm.SelectedControl = null;
                    m_Properties.PropertyGrid.SelectedObject = null;
                    m_Properties.ControlsList.Items.Clear();
                    CurrentForm.SizeChanged -= new EventHandler(RenderingPanel_SizeChanged);
                }
                CurrentForm = null;
            }

            if (!editorIsVisible && GE.GUIManager != null)
            {
                Program.GE.Parameters.MousePosition = new NGUINet.NVector2(0, 0);
                GE.GUIManager.Update(Program.GE.Parameters);
            }
        }

        public void RenderingPanel_SizeChanged(object sender, EventArgs e)
        {
            Render.Graphics3D(Program.GE.RenderingPanel.Width, Program.GE.RenderingPanel.Height, 32, 2, 0, 0,
                              @".\Data\DefaultTex.png");
            if (sender.Equals(CurrentForm))
            {
                 CurrentForm.RemoveGUI();
                 CurrentForm.SetupGUI();
            }
        }

        bool MouseDown = false;
        bool CanDrag = false;
        bool Dragging = false;
        Point MousePos;
        int Direction = 0;
        bool Sizing = false;

        bool CheckSelBox(Point mousePos, float x, float y, float w, float h)
        {
            if((float)mousePos.X > x && (float)mousePos.Y > y &&
                (float)mousePos.X < x + w && (float)mousePos.Y < y + h)
                return true;
            return false;
        }

        public void RenderPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (CurrentForm != null && CurrentForm.SelectedControl != null && CurrentForm.SelectedControl.EditorControl.DefaultEvent.Length > 0)
            {
                int EventIndex = 0;
                foreach (EditorEvent E in CurrentForm.SelectedControl.EditorControl.Events)
                {
                    if (E.Name.Equals(CurrentForm.SelectedControl.EditorControl.DefaultEvent, StringComparison.CurrentCultureIgnoreCase))
                    {
                        break;
                    }
                    ++EventIndex;
                }
                if (EventIndex == CurrentForm.SelectedControl.EditorControl.Events.Count)
                    return;

                string CurrentFunc = CurrentForm.SelectedControl.GetEventValue(EventIndex);
                if (CurrentFunc.Length == 0)
                {
                    string ControlName = "Unknown" + Environment.TickCount.ToString();

                    EditorPropertyInstance P = CurrentForm.SelectedControl.FindPropertyInstance("name");
                    if (P != null)
                        ControlName = P.Value.ToString();

                    CurrentFunc = ControlName + "_" + CurrentForm.SelectedControl.EditorControl.DefaultEvent;
                    CurrentForm.SelectedControl.SetEventValue(EventIndex, CurrentFunc);
                }else
                {
                    CurrentForm.SelectedControl.GoToMethod(CurrentFunc);
                }
            }
        }

        public void RenderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseDown)
            {
                Direction = 0;

                if(CurrentForm != null && CurrentForm.SelectedControl != null)
                {
                    float x = CurrentForm.SelectedPosition.X;
                    float y = CurrentForm.SelectedPosition.Y;
                    float w = CurrentForm.SelectedSize.X;
                    float h = CurrentForm.SelectedSize.Y;

                    float hw = w * 0.5f;
                    float hh = h * 0.5f;

                    float t = y - 7;
                    float m = y + (hh) - 3;
                    float b = y + h;

                    float l = x - 7;
                    float c = x + (hw) - 3;
                    float r = x + w;

                    Direction += CheckSelBox(e.Location, c, t, 7, 7) ? 1 : 0;
                    Direction += CheckSelBox(e.Location, c, b, 7, 7) ? 2 : 0;
                    Direction += CheckSelBox(e.Location, l, m, 7, 7) ? 3 : 0;
                    Direction += CheckSelBox(e.Location, r, m, 7, 7) ? 4 : 0;
                    Direction += CheckSelBox(e.Location, l, t, 7, 7) ? 5 : 0;
                    Direction += CheckSelBox(e.Location, r, t, 7, 7) ? 6 : 0;
                    Direction += CheckSelBox(e.Location, l, b, 7, 7) ? 7 : 0;
                    Direction += CheckSelBox(e.Location, r, b, 7, 7) ? 8 : 0;

                    switch (Direction)
                    {
                        case 1:
                        case 2:
                            {
                                CurrentForm.Cursor = Cursors.SizeNS;
                                break;
                            }
                        case 3:
                        case 4:
                            {
                                CurrentForm.Cursor = Cursors.SizeWE;
                                break;
                            }
                        case 5:
                        case 8:
                            {
                                CurrentForm.Cursor = Cursors.SizeNWSE;
                                break;
                            }
                        case 6:
                        case 7:
                            {
                                CurrentForm.Cursor = Cursors.SizeNESW;
                                break;
                            }
                        default:
                            {
                                CurrentForm.Cursor = Cursors.Default;
                                break;
                            }
                    }
                }
            }
            else
            {
                if (CanDrag && !Dragging && !Sizing)
                {
                    CurrentForm.Cursor = Cursors.SizeAll;
                    Dragging = true;
                }
                else if (Dragging && !Sizing)
                {
                    Point Diff = new Point(e.Location.X - MousePos.X, e.Location.Y - MousePos.Y);
                    MousePos = e.Location;

                    if (CurrentForm != null && CurrentForm.SelectedControl != null && CurrentForm.SelectedControl != CurrentForm.Master)
                    {
                        EditorControlInstance C = CurrentForm.SelectedControl;

                        EditorPropertyInstance PosTypeP = C.FindPropertyInstance("positiontype");
                        EditorPropertyInstance PosP = C.FindPropertyInstance("location");
                        PositionType PType = PositionType.Absolute;
                        Vector2 Lcn = new Vector2();
                        if (PosTypeP != null)
                            PType = (PositionType)PosTypeP.Value;
                        if (PosP != null)
                            Lcn = (Vector2)((Vector2)PosP.Value).Clone();

                        Vector2 VDiff = new Vector2((float)Diff.X, (float)Diff.Y);
                        if (PType != PositionType.Absolute)
                        {
                            VDiff.X /= (float)Program.GE.RenderingPanel.Width;
                            VDiff.Y /= (float)Program.GE.RenderingPanel.Height;
                        }

                        Lcn.X += VDiff.X;
                        Lcn.Y += VDiff.Y;

                        if (PosP != null)
                        {
                            PosP.Value = Lcn;

                            // Write property update
                            PacketWriter Pa = new PacketWriter();
                            Pa.Write(C.AllocID);
                            Pa.Write(PosP.EditorProperty.SDKName, false);
                            EditorProperty.WritePacket(Pa, PosP.EditorProperty.Type, PosP.Value);

                            // Update
                            SDKNet.ControlHost.ProcessUpdateProperties(Pa.ToArray());
                        }

                        if (PType != PositionType.Absolute)
                        {
                            VDiff.X *= (float)Program.GE.RenderingPanel.Width;
                            VDiff.Y *= (float)Program.GE.RenderingPanel.Height;
                        }

                        CurrentForm.SelectedPosition.X += VDiff.X;
                        CurrentForm.SelectedPosition.Y += VDiff.Y;

                        PacketWriter SPa = new PacketWriter();
                        SPa.Write(1001);
                        SPa.Write("VSBL", false);
                        SPa.Write((byte)1);
                        SPa.Write(1001);
                        SPa.Write("ZZLC", false);
                        SPa.Write(CurrentForm.SelectedPosition.X);
                        SPa.Write(CurrentForm.SelectedPosition.Y);
                        SDKNet.ControlHost.ProcessUpdateProperties(SPa.ToArray());
                    }
                }
                else if (Sizing)
                {
                    Point Diff = new Point(e.Location.X - MousePos.X, e.Location.Y - MousePos.Y);
                    MousePos = e.Location;

                    if (CurrentForm != null && CurrentForm.SelectedControl != null && CurrentForm.SelectedControl != CurrentForm.Master)
                    {
                        EditorControlInstance C = CurrentForm.SelectedControl;

                        EditorPropertyInstance PosTypeP = C.FindPropertyInstance("positiontype");
                        EditorPropertyInstance PosP = C.FindPropertyInstance("location");
                        EditorPropertyInstance SizeTypeP = C.FindPropertyInstance("sizetype");
                        EditorPropertyInstance SizeP = C.FindPropertyInstance("size");
                        PositionType PType = PositionType.Absolute;
                        SizeType SType = SizeType.Absolute;
                        Vector2 Lcn = new Vector2();
                        Vector2 Siz = new Vector2();

                        if (PosTypeP != null)
                            PType = (PositionType)PosTypeP.Value;
                        if (PosP != null)
                            Lcn = (Vector2)((Vector2)PosP.Value).Clone();
                        if (SizeTypeP != null)
                            SType = (SizeType)SizeTypeP.Value;
                        if (SizeP != null)
                            Siz = (Vector2)((Vector2)SizeP.Value).Clone();

                        Vector2 PDiff = new Vector2((float)Diff.X, (float)Diff.Y);
                        Vector2 SDiff = new Vector2((float)Diff.X, (float)Diff.Y);

                        if (PType != PositionType.Absolute)
                        {
                            PDiff.X /= (float)Program.GE.RenderingPanel.Width;
                            PDiff.Y /= (float)Program.GE.RenderingPanel.Height;
                        }
                        if (SType != SizeType.Absolute)
                        {
                            SDiff.X /= (float)Program.GE.RenderingPanel.Width;
                            SDiff.Y /= (float)Program.GE.RenderingPanel.Height;
                        }

                        // Do X Minus first
                        if (Direction == 5 || Direction == 3 || Direction == 7)
                        {
                            Lcn.X += PDiff.X;
                            Siz.X -= SDiff.X;
                        }
                        else if (Direction == 6 || Direction == 4 || Direction == 8)
                        {
                            Siz.X += SDiff.X;
                        }

                        // Do Y Minus
                        if (Direction == 5 || Direction == 1 || Direction == 6)
                        {
                            Lcn.Y += PDiff.Y;
                            Siz.Y -= SDiff.Y;
                        }
                        else if (Direction == 7 || Direction == 2 || Direction == 8)
                        {
                            Siz.Y += SDiff.Y;
                        }

                        // Write property update
                        PacketWriter Pa = new PacketWriter();

                        // Location property
                        if (PosP != null)
                        {
                            PosP.Value = Lcn;

                            Pa.Write(C.AllocID);
                            Pa.Write(PosP.EditorProperty.SDKName, false);
                            EditorProperty.WritePacket(Pa, PosP.EditorProperty.Type, PosP.Value);
                        }

                        // Size property
                        if (SizeP != null)
                        {
                            SizeP.Value = Siz;

                            Pa.Write(C.AllocID);
                            Pa.Write(SizeP.EditorProperty.SDKName, false);
                            EditorProperty.WritePacket(Pa, SizeP.EditorProperty.Type, SizeP.Value);
                        }

                        if (Pa.Length > 0)
                        {
                            // Update
                            SDKNet.ControlHost.ProcessUpdateProperties(Pa.ToArray());
                        }

                        if (PType != PositionType.Absolute)
                        {
                            PDiff.X *= (float)Program.GE.RenderingPanel.Width;
                            PDiff.Y *= (float)Program.GE.RenderingPanel.Height;
                        }
                        if (SType != SizeType.Absolute)
                        {
                            SDiff.X *= (float)Program.GE.RenderingPanel.Width;
                            SDiff.Y *= (float)Program.GE.RenderingPanel.Height;
                        }


                        // Update the selection box
                        if (Direction == 5 || Direction == 3 || Direction == 7)
                        {
                            CurrentForm.SelectedPosition.X += PDiff.X;
                            CurrentForm.SelectedSize.X -= SDiff.X;
                        }
                        else if (Direction == 6 || Direction == 4 || Direction == 8)
                        {
                            CurrentForm.SelectedSize.X += SDiff.X;
                        }

                        if (Direction == 5 || Direction == 1 || Direction == 6)
                        {
                            CurrentForm.SelectedPosition.Y += PDiff.Y;
                            CurrentForm.SelectedSize.Y -= SDiff.Y;
                        }
                        else if (Direction == 7 || Direction == 2 || Direction == 8)
                        {
                            CurrentForm.SelectedSize.Y += SDiff.Y;
                        }
                        
                        PacketWriter SPa = new PacketWriter();
                        SPa.Write(1001);
                        SPa.Write("VSBL", false);
                        SPa.Write((byte)1);
                        SPa.Write(1001);
                        SPa.Write("ZZLC", false);
                        SPa.Write(CurrentForm.SelectedPosition.X);
                        SPa.Write(CurrentForm.SelectedPosition.Y);
                        SPa.Write(1001);
                        SPa.Write("ZZSZ", false);
                        SPa.Write(CurrentForm.SelectedSize.X);
                        SPa.Write(CurrentForm.SelectedSize.Y);
                        SDKNet.ControlHost.ProcessUpdateProperties(SPa.ToArray());
                    }

                }
            }
        }

        void RenderPanel_MouseWheel(object sender, MouseEventArgs e)
        {


        }

        public void RenderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            MouseDown = true;
            MousePos = e.Location;

            if (CurrentForm != null)
            {
                if (Direction != 0)
                {
                    Sizing = true;
                }
                else
                {
                    EditorControlInstance Inst = CurrentForm.Master.FindControl(e.Location);
                    if (Inst != null && Inst != CurrentForm.SelectedControl)
                        CurrentForm.SelectControl(Inst);

                    if ((float)e.X > CurrentForm.SelectedPosition.X && (float)e.Y > CurrentForm.SelectedPosition.Y
                        && (float)e.X < CurrentForm.SelectedPosition.X + CurrentForm.SelectedSize.X
                        && (float)e.Y < CurrentForm.SelectedPosition.Y + CurrentForm.SelectedSize.Y)
                    {
                        CanDrag = true;
                    }
                }
            }
        }

        public void RenderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            // Show context menu
            if (e.Button == MouseButtons.Right)
            {
                if (CurrentForm != null)
                {
                    // If the mouse clicked outside of the current selection
                    if (CurrentForm.SelectedControl == null || !CheckSelBox(e.Location, CurrentForm.SelectedPosition.X, CurrentForm.SelectedPosition.Y,
                    CurrentForm.SelectedSize.X, CurrentForm.SelectedSize.Y))
                    {
                        // Select the control we clicked
                        EditorControlInstance Inst = CurrentForm.Master.FindControl(e.Location);
                        if (Inst != null && Inst != CurrentForm.SelectedControl)
                            CurrentForm.SelectControl(Inst);
                    }
                    else
                    {
                        // Test selected control to see if there is a higher one
                        EditorControlInstance Inst = CurrentForm.Master.FindControl(e.Location);
                        if (Inst != null && Inst != CurrentForm.SelectedControl)
                            CurrentForm.SelectControl(Inst);
                    }

                    if (CurrentForm.SelectedControl == null)
                    {
                        CurrentForm.sendToBackToolStripMenuItem.Enabled = false;
                        CurrentForm.bringToFrontToolStripMenuItem.Enabled = false;
                        CurrentForm.deleteToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        CurrentForm.sendToBackToolStripMenuItem.Enabled = true;
                        CurrentForm.bringToFrontToolStripMenuItem.Enabled = true;
                        CurrentForm.deleteToolStripMenuItem.Enabled = true;
                    }

                    CurrentForm.GUIRightClickMenu.Show(Program.GE.RenderingPanel, e.Location);
                }
                return;
            }

            if (CurrentForm != null)
            {
                CurrentForm.Cursor = Cursors.Default;

                // Reselect to update properties
                if (Dragging && CurrentForm.SelectedControl != null)
                    CurrentForm.SelectControl(CurrentForm.SelectedControl);

                if (Dragging || Sizing)
                    CurrentForm.SetSaved(false);
            }

            Dragging = false;
            CanDrag = false;
            MouseDown = false;
            Sizing = false;
            
        }

        public void RenderPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                if (CurrentForm != null)
                    CurrentForm.deleteToolStripMenuItem_Click(sender, null);
            }
        }

        public void Application_Idle(object sender, EventArgs e)
        {
            //Point Mouse = Program.GE.RenderingPanel.PointToClient(Cursor.Position);
            //Program.GE.Parameters.MousePosition = new NGUINet.NVector2(Mouse.X, Mouse.Y);
            Program.GE.Parameters.MousePosition = new NGUINet.NVector2(0, 0);
            GE.GUIManager.Update(Program.GE.Parameters);
            Program.GE.Parameters.InputBuffer.Clear();

            Collision.UpdateWorld();
            Render.RenderWorld();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender, e);
        }

        private ScriptForm CreateNewDocument(string FormName)
        {
            ScriptForm NewScript = new ScriptForm();
            NewScript.TabText = FormName;
            NewScript.LoadedFileName = FormName;

            return NewScript;
        }

        private ScriptForm CreateNewScript(String Script)
        {
            ScriptForm NewScript = new ScriptForm();
            NewScript.LoadFile(Script);
            TreeNode TN = new TreeNode(Script);
            TN.ImageIndex = 1;
            TN.SelectedImageIndex = 1;
            TN.StateImageIndex = 1;
            m_ProjectExplorer.ProjectTree.Nodes[0].Nodes.Add(TN);
            ;
            return NewScript;
        }

        private void SimpleScriptEditor_Load(object sender, EventArgs e)
        {
            m_ProjectExplorer.Show(ScriptDock);
            m_ToolBox.Show(ScriptDock, DockState.DockRight);
            m_Properties.Show(ScriptDock, DockState.DockRight);
        }

        private void ScriptDock_DragDrop(object sender, DragEventArgs e)
        {
            MessageBox.Show(e.Data.ToString());
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null)
            {
                ActiveScript.SaveFile();
            }
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Printing.Print();
            }
        }

        private void printPreviewToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Printing.PrintPreview();
            }
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Clipboard.Cut();
            }
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Clipboard.Copy();
            }
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Clipboard.Paste();
            }
        }

        private void undoToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.UndoRedo.Undo();
            }
        }

        private void redoToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.UndoRedo.Redo();
            }
        }

        private void SaveAllToolStripButton_Click(object sender, EventArgs e)
        {
            SaveAllDocuments();
        }

        public void CloseDocument(ScriptEditorForm Document)
        {          
            Document.Close();
        }

        public void SaveAllDocuments()
        {
            ScriptEditorForm CurrentScript = null;

            if (ActiveScript != null)
            {
                CurrentScript = ActiveScript;
            }
            foreach (ScriptEditorForm Script in ScriptDock.Documents)
            {
                Script.Activate();
                Script.SaveFile();
            }

            if (CurrentScript != null)
            {
                CurrentScript.Activate();
            }
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.FindReplace.ShowFind();
            }
        }

        public void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewScriptForm TE = new NewScriptForm();
            if (TE.ShowDialog() == DialogResult.Cancel)
                return;

            string ScriptName = TE.ScriptName;
            TE.Dispose();

            if (string.IsNullOrEmpty(ScriptName))
            {
                return;
            }
            if (ScriptName.Length < 3 || !ScriptName.Substring(ScriptName.Length - 3).Equals(".cs", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptName += ".cs";
            }

            Program.GE.RefreshScripts();
            foreach (string S in GE.ScriptsList)
            {
                if (ScriptName == S)
                {
                    MessageBox.Show("Error: Script already exists", "Existing script", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }
            }
            if (File.Exists(@"Data\Server Data\Scripts\" + ScriptName))
            {
                MessageBox.Show("Error: Script already exists", "Existing script", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            else
            {
                // Create new scripts..
                if (TE.ScriptType == 0)
                {
                    try
                    {
                        FileStream NewFile = File.Create(@"Data\Server Data\Scripts\" + ScriptName);
                        NewFile.Close();

                        StreamWriter Writer = new StreamWriter(@"Data\Server Data\Scripts\" + ScriptName);
                        
                        foreach(string L in NewScriptForm.DefaultScript)
                            Writer.WriteLine(L.Replace("%ScriptName%", Path.GetFileNameWithoutExtension(ScriptName)));
                        Writer.Close();

                        
                    }
                    catch (IOException err)
                    {
                        MessageBox.Show("Error: " + err.ToString());
                        return;
                    }

                    AddScriptFileToCSProj(ScriptName);

                    ScriptForm ScriptDoc = CreateNewScript(ScriptName);
                    Program.GE.RefreshScripts();
                    ScriptDoc.Show(this.ScriptDock);
                }else
                {
                    try
                    {
                        FileStream NewFile = File.Create(@"Data\Server Data\Scripts\" + ScriptName);
                        NewFile.Close();

                        StreamWriter Writer = new StreamWriter(@"Data\Server Data\Scripts\" + ScriptName);
                        
                        foreach(string L in NewScriptForm.DefaultFormScript)
                            Writer.WriteLine(L.Replace("%ScriptName%", Path.GetFileNameWithoutExtension(ScriptName)));
                        Writer.Close();
                    }
                    catch (IOException err)
                    {
                        MessageBox.Show("Error: " + err.ToString());
                        return;
                    }

                    try
                    {
                        FileStream NewFile = File.Create(@"Data\Server Data\Scripts\" + ScriptName.Substring(0, ScriptName.Length - 2) + "Designer.cs");
                        NewFile.Close();

                        StreamWriter Writer = new StreamWriter(@"Data\Server Data\Scripts\" + ScriptName.Substring(0, ScriptName.Length - 2) + "Designer.cs");
                        
                        foreach(string L in NewScriptForm.DefaultDesignerScript)
                            Writer.WriteLine(L.Replace("%ScriptName%", Path.GetFileNameWithoutExtension(ScriptName)));
                        Writer.Close();
                    }
                    catch (IOException err)
                    {
                        MessageBox.Show("Error: " + err.ToString());
                        return;
                    }

                    try
                    {
                        FileStream NewFile = File.Create(@"Data\Server Data\Scripts\" + ScriptName.Substring(0, ScriptName.Length - 2) + "xml");
                        NewFile.Close();

                        StreamWriter Writer = new StreamWriter(@"Data\Server Data\Scripts\" + ScriptName.Substring(0, ScriptName.Length - 2) + "xml");
                        
                        foreach(string L in NewScriptForm.DefaultDesignerData)
                            Writer.WriteLine(L.Replace("%ScriptName%", Path.GetFileNameWithoutExtension(ScriptName)));
                        Writer.Close();
                    }
                    catch (IOException err)
                    {
                        MessageBox.Show("Error: " + err.ToString());
                        return;
                    }

                    AddScriptFormToCSProj(ScriptName, ScriptName.Substring(0, ScriptName.Length - 2) + "Designer.cs", ScriptName.Substring(0, ScriptName.Length - 2) + "xml");

//                     FormEditor NewScript = new FormEditor();
//                     NewScript.LoadFile(ScriptName);
                    TreeNode TN = new TreeNode(ScriptName);
                    TN.ImageIndex = 2;
                    TN.SelectedImageIndex = 2;
                    TN.StateImageIndex = 2;
                    m_ProjectExplorer.ProjectTree.Nodes[0].Nodes.Add(TN);
                    Program.GE.RefreshScripts();
                    //NewScript.Show(this.ScriptDock);

                    LoadFile(ScriptName, true);
                }
            }
        }

        private void AddScriptFileToCSProj(string scriptName)
        {
            try
            {
                XmlDocument Doc = new XmlDocument();
                Doc.Load(@"Data\Server Data\Scripts\Scripts.csproj");
                XmlNamespaceManager mgr = new XmlNamespaceManager(Doc.NameTable);
                mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

                XmlNode ContentGroup = Doc.SelectSingleNode("/x:Project/x:ItemGroup/x:Compile", mgr).ParentNode;

                XmlElement CompileElement = Doc.CreateElement("Compile", ContentGroup.NamespaceURI);
                CompileElement.SetAttribute("Include", scriptName);
                ContentGroup.AppendChild(CompileElement);

                Doc.Save(@"Data\Server Data\Scripts\Scripts.csproj");
            }
            catch (System.Exception)
            {

            }
        }

        private void AddScriptFormToCSProj(string scriptName, string designerName, string xmlName)
        {
//             try
//             {
                XmlDocument Doc = new XmlDocument();
                Doc.Load(@"Data\Server Data\Scripts\Scripts.csproj");
                XmlNamespaceManager mgr = new XmlNamespaceManager(Doc.NameTable);
                mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

                XmlNode ContentGroup = Doc.SelectSingleNode("/x:Project/x:ItemGroup/x:Compile", mgr).ParentNode;

                XmlElement CompileElement = Doc.CreateElement("Compile", ContentGroup.NamespaceURI);
                CompileElement.SetAttribute("Include", scriptName);

                XmlElement DependentUponElement1 = Doc.CreateElement("DependentUpon", ContentGroup.NamespaceURI);
                XmlElement DependentUponElement2 = Doc.CreateElement("DependentUpon", ContentGroup.NamespaceURI);
                XmlText DependentUponText1 = Doc.CreateTextNode(scriptName);
                XmlText DependentUponText2 = Doc.CreateTextNode(scriptName);
                DependentUponElement1.AppendChild(DependentUponText1);
                DependentUponElement2.AppendChild(DependentUponText2);

                XmlElement DesignerElement = Doc.CreateElement("Compile", ContentGroup.NamespaceURI);
                XmlElement DefinitionElement = Doc.CreateElement("Content", ContentGroup.NamespaceURI);
                DesignerElement.SetAttribute("Include", designerName);
                DefinitionElement.SetAttribute("Include", xmlName);
                DesignerElement.AppendChild(DependentUponElement1);
                DefinitionElement.AppendChild(DependentUponElement2);

                ContentGroup.AppendChild(CompileElement);
                ContentGroup.AppendChild(DesignerElement);
                ContentGroup.AppendChild(DefinitionElement);

                Doc.Save(@"Data\Server Data\Scripts\Scripts.csproj");
//             }
//             catch (System.Exception)
//             {
// 
//             }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null)
            {
                ActiveScript.SaveFile();
            }
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAllToolStripButton_Click(sender, e);
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Printing.PrintPreview();
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Printing.Print();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null)
            {
                ActiveScript.Close();
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptEditorForm[] Scripts = new ScriptEditorForm[ScriptDock.DocumentsCount];
            int count = 0;
            foreach (ScriptEditorForm Script in ScriptDock.Documents)
            {
                Scripts[count] = Script;
                count++;
            }

            for (int i = 0; i < Scripts.Length; i++)
            {
                Scripts[i].Activate();
                Scripts[i].Close();
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.UndoRedo.Undo();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.UndoRedo.Redo();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Clipboard.Cut();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Clipboard.Copy();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Clipboard.Paste();
            }
        }

        private void seltcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Selection.SelectAll();
            }
        }

        private void projectExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_ProjectExplorer.Visible)
            {
                m_ProjectExplorer.Hide();
            }
            else
            {
                m_ProjectExplorer.Show();
            }
        }

        private void incrementalSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.FindReplace.IncrementalSearcher.Show();
            }
        }

        private void snippetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Snippets.ShowSnippetList();
            }
        }

        private void mainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptMainToolBar.Visible = !ScriptMainToolBar.Visible;
        }

        private void goToLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.GoTo.ShowGoToDialog();
            }
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Zoom = ActiveScript.ScriptText.Zoom + 1;
            }
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Zoom = ActiveScript.ScriptText.Zoom - 1;
            }
        }

        private void zoomResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Zoom = ActiveScript.DefaultZoom;
            }
        }

        private void navigateBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptForm[] Scripts = new ScriptForm[ScriptDock.DocumentsCount];
            int count = 0;
            int SwitchTo = 0;
            foreach (ScriptForm Script in ScriptDock.Documents)
            {
                Scripts[count] = Script;
                if (Script == ActiveScript)
                {
                    SwitchTo = count - 1;
                }
                count++;
            }
            if (SwitchTo >= 0 && SwitchTo < ScriptDock.DocumentsCount)
            {
                Scripts[SwitchTo].Activate();
            }
        }

        private void navigateForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptForm[] Scripts = new ScriptForm[ScriptDock.DocumentsCount];
            int count = 0;
            int SwitchTo = 0;
            foreach (ScriptForm Script in ScriptDock.Documents)
            {
                Scripts[count] = Script;
                if (Script == ActiveScript)
                {
                    SwitchTo = count + 1;
                }
                count++;
            }
            if (SwitchTo >= 0 && SwitchTo < ScriptDock.DocumentsCount)
            {
                Scripts[SwitchTo].Activate();
            }
        }

        private void increaseIndentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Indentation.ShowGuides = !ActiveScript.ScriptText.Indentation.ShowGuides;
            }
        }

        private void whitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                if (ActiveScript.ScriptText.Whitespace.Mode == ScintillaNet.WhitespaceMode.VisibleAlways)
                {
                    ActiveScript.ScriptText.Whitespace.Mode = ScintillaNet.WhitespaceMode.Invisible;
                }
                else
                {
                    ActiveScript.ScriptText.Whitespace.Mode = ScintillaNet.WhitespaceMode.VisibleAlways;
                }
            }
        }

        private void toggleCommentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Lexing.ToggleLineComment();
            }
        }

        private void increaseCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Lexing.LineComment();
            }
        }

        private void decreaseCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Lexing.LineUncomment();
            }
        }

        public void CloseAllDocuments()
        {
            ScriptForm[] Scripts = new ScriptForm[ScriptDock.DocumentsCount];
            int count = 0;
            foreach (ScriptForm Script in ScriptDock.Documents)
            {
                Scripts[count] = Script;
                count++;
            }

            for (int i = 0; i < Scripts.Length; i++)
            {
                Scripts[i].Activate();
                Scripts[i].Close();
            }
        }

        public void SetCursorPosition()
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                CursorPos.Text = "Ln: " + (ActiveScript.ScriptText.Caret.LineNumber + 1);
            }
            else
            {
                CursorPos.Text = "";
            }
        }

        public void CloseAllButThis(ScriptForm Current)
        {
            ScriptForm[] Scripts = new ScriptForm[ScriptDock.DocumentsCount];
            int count = 0;
            foreach (ScriptForm Script in ScriptDock.Documents)
            {
                if (Script != Current)
                {
                    Scripts[count] = Script;
                    count++;
                }
            }

            for (int i = 0; i < Scripts.Length; i++)
            {
                if (Scripts[i] != null)
                {
                    Scripts[i].Activate();
                    Scripts[i].Close();
                }
            }
        }

        private void highlightCurrentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Caret.HighlightCurrentLine = !ActiveScript.ScriptText.Caret.HighlightCurrentLine;
            }
        }

        private void toggleBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                Line currentLine = ActiveScript.ScriptText.Lines.Current;
                if (ActiveScript.ScriptText.Markers.GetMarkerMask(currentLine) == 0)
                {
                    currentLine.AddMarker(0);
                }
                else
                {
                    currentLine.DeleteMarker(0);
                }
            }
        }

        private void nextBookarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                Line l = ActiveScript.ScriptText.Lines.Current.FindNextMarker(1);
                if (l != null)
                    l.Goto();
            }
        }

        private void previousBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                Line l = ActiveScript.ScriptText.Lines.Current.FindPreviousMarker(1);
                if (l != null)
                    l.Goto();
            }
        }

        private void deleteAllBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveScript != null && ActiveScript.ScriptText != null)
            {
                ActiveScript.ScriptText.Markers.DeleteAll(0);
            }
        }

        private void fromTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateNewScript F = new TemplateNewScript();
            F.ShowDialog();

        }

        private void ScriptDock_ActiveDocumentChanged(object sender, EventArgs e)
        {
            UpdateRenderingPanel(Program.GE.RenderingPanelCurrentIndex == -6);
        }
    }
}