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
namespace RealmCrafter_GE.Dockable_Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using WeifenLuo.WinFormsUI.Docking;
    using NGUINet;

    public partial class WorldTerrainEditor : DockContent
    {
        public RealmCrafter.RCT.RCTerrain Terrain;
        public RealmCrafter.RCT.TerrainManager TManager;
        protected SelectedTerrainTool CurrentTool = SelectedTerrainTool.RaiseLower;
        public RCTTest.GrassTypesSelector GrassSelector = new RCTTest.GrassTypesSelector();
        RCTTest.AutoTextureDialog D = new RCTTest.AutoTextureDialog();
        RCTTest.RCTImportDialog ImportDialog;

        protected NVector2 FirstPos = new NVector2(0, 0);
        protected float FirstRadius = 0.0f;
        protected bool FirstWait = false, FirstPlaced = false;

        protected bool editMode = false;
        protected bool MouseLocked = false;

        protected int LastSelected = 0;
        protected double[] StoredMins = new double[5];
        protected double[] StoredMaxs = new double[5];
        protected double[] AutoStoredMins = new double[5];
        protected double[] AutoStoredMaxs = new double[5];
        protected int[] AutoStoredHeightMins = new int[5];
        protected int[] AutoStoredHeightMaxs = new int[5];

        public WorldTerrainEditor()
        {
            InitializeComponent();

            ImportDialog = new RCTTest.RCTImportDialog();

            ToolTip Tip = new ToolTip();

            Tip.AutoPopDelay = 10000;
            Tip.InitialDelay = 100;
            Tip.ReshowDelay = 100;
            Tip.ShowAlways = true;

            StoredMins[0] = 0.0;
            StoredMins[1] = 0.0;
            StoredMins[2] = 0.0;
            StoredMins[3] = 0.0;
            StoredMins[4] = 0.0;
            StoredMaxs[0] = 0.0;
            StoredMaxs[1] = 0.0;
            StoredMaxs[2] = 0.0;
            StoredMaxs[3] = 0.0;
            StoredMaxs[4] = 0.0;
            AutoStoredMins[0] = 0.0;
            AutoStoredMins[1] = 0.0;
            AutoStoredMins[2] = 0.0;
            AutoStoredMins[3] = 0.0;
            AutoStoredMins[4] = 0.0;
            AutoStoredMaxs[0] = 0.0;
            AutoStoredMaxs[1] = 0.0;
            AutoStoredMaxs[2] = 0.0;
            AutoStoredMaxs[3] = 0.0;
            AutoStoredMaxs[4] = 0.0;

            Tip.SetToolTip(RaiseLowerTool, "Raise / Lower Tool");
            Tip.SetToolTip(SetHeightTool, "Fix Terrain Height");
            Tip.SetToolTip(SmoothTool, "Smooth Tool");
            Tip.SetToolTip(ErodeTool, "Erosion Tool");
            Tip.SetToolTip(RampTool, "Ramp Tool");
            Tip.SetToolTip(PaintHoleTool, "Paint Holes Tool");
            Tip.SetToolTip(PaintTool, "Paint Tool");
            Tip.SetToolTip(GrassTool, "Paint Grass Tool");

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void RefreshEditorButton(int mode)
        {
            if (mode == 0)
            {
                EditorModeButton.Text = "Select one terrain for editing";
                EditorModeButton.Enabled = false;
                TerrainRaiseLowerToolGroup.Visible = false;
                TerrainSetHeightToolGroup.Visible = false;
                TerrainRampToolGroup.Visible = false;
                TerrainSmoothToolGroup.Visible = false;
                TerrainErodeToolGroup.Visible = false;
                TerrainPaintToolGroup.Visible = false;
                TerrainPaintHoleToolGroup.Visible = false;
                TerrainGrassToolGroup.Visible = false;
                //ToolsGroup.Visible = false;
                ToolsGroup.Height = 0;

                if (TManager != null)
                    TManager.BrushRadius = 0.0f;
            }
            else if (mode == 1)
            {
                EditorModeButton.Text = "Enter Edit Mode";
                EditorModeButton.Enabled = true;
                TerrainRaiseLowerToolGroup.Visible = false;
                TerrainSetHeightToolGroup.Visible = false;
                TerrainRampToolGroup.Visible = false;
                TerrainSmoothToolGroup.Visible = false;
                TerrainErodeToolGroup.Visible = false;
                TerrainPaintToolGroup.Visible = false;
                TerrainPaintHoleToolGroup.Visible = false;
                TerrainGrassToolGroup.Visible = false;
                //ToolsGroup.Visible = false;
                ToolsGroup.Height = 0;

                if (TManager != null)
                    TManager.BrushRadius = 0.0f;
            }
            else if (mode == 2)
            {
                EditorModeButton.Text = "Exit Edit Mode";
                EditorModeButton.Enabled = true;
            }
        }

        public void EditorModeButton_Click(object sender, EventArgs e)
        {
            if (editMode == true)
            {
                Program.GE.m_ZoneList.WorldZonesTree.Enabled = true;
                editMode = false;
                RefreshEditorButton(1);

                Terrain = null;

                return;
            }

            // Invalid
            if (Program.GE.ZoneSelected.Count != 1 || Program.GE.ZoneSelected[0].GetType() != typeof(RealmCrafter.ClientZone.RCTTerrain))
                return;

            Program.GE.m_ZoneList.WorldZonesTree.Enabled = false;
            editMode = true;
            RefreshEditorButton(2);

            Terrain = (Program.GE.ZoneSelected[0] as RealmCrafter.ClientZone.RCTTerrain).Terrain;
            int TexIndex = (listView1.SelectedIndices.Count > 0) ? listView1.SelectedIndices[0] : 0;
            if (TexIndex < 0)
                TexIndex = 0;
            PaintScaleTrackbar.Value = Convert.ToInt32((1.0f / Terrain.GetTextureScale(TexIndex) * 50.0f));
            PaintScaleTrackbar_Scroll(PaintScaleTrackbar, EventArgs.Empty);
            ResetPreviewTextures();

            for (int i = 0; i < 8; ++i)
            {
                string GrassTypeName = Terrain.GetGrassType(i);
                if (GrassTypeName.Length > 0)
                    GrassSelectionList.Items[i] = GrassTypeName;
            }

            
            UpdateToolGUI();
            ToolsGroup.Height = 127;
            
        }

        public bool EditorMode
        {
            get { return editMode; }
            set { editMode = value; }
        }

        public void ImportTerrain()
        {
            if (ImportDialog.ShowDialog() == DialogResult.Cancel)
                return;

            if (ImportDialog.Plugin == null)
                return;

            RCTPlugin.IRCTPlugin Plugin = ImportDialog.Plugin;

            string CurrentPath = Environment.CurrentDirectory;
            RCTPlugin.RCTImportData ImportData = Plugin.Import(RenderingServices.RenderWrapper.bbdx2_GetIDirect3DDevice9());
            Environment.CurrentDirectory = CurrentPath;

            if (ImportData == null)
                return;

            if (ImportData.Width != ImportData.Height)
            {
                MessageBox.Show("Plugin Error: Width and Height are not of the same size!");
                return;
            }

            if (!(ImportData.Width == 32
                || ImportData.Width == 64
                || ImportData.Width == 128
                || ImportData.Width == 256
                || ImportData.Width == 512
                || ImportData.Width == 1024
                || ImportData.Width == 2048))
            {
                MessageBox.Show("Plugin Error: Terrain width is invalid!");
                return;
            }

            if (ImportData.TexturePaths.Length != 5)
            {
                MessageBox.Show("Plugin Error: Expected five texture paths!");
                return;
            }

            if (ImportData.TextureScales.Length != 5)
            {
                MessageBox.Show("Plugin Error: Expected five texture scales!");
                return;
            }

            if (ImportData.GrassTypes.Length != 8)
            {
                MessageBox.Show("Plugin Error: Expected eight grass types!");
                return;
            }

            if (ImportData.HeightData.Length != (ImportData.Width + 1) * (ImportData.Height + 1))
            {
                MessageBox.Show("Plugin Error: Height data length does not match suggested length!");
                return;
            }

            if (ImportData.SplatData.Length != (ImportData.Width + 1) * (ImportData.Height + 1))
            {
                MessageBox.Show("Plugin Error: Splat data length does not match suggested length!");
                return;
            }

            if (ImportData.GrassData.Length != (ImportData.Width + 1) * (ImportData.Height + 1))
            {
                MessageBox.Show("Plugin Error: Grass data length does not match the suggested length!");
                return;
            }

            Terrain = TManager.CreateT1(Convert.ToInt32(ImportData.Width));

            // Create terrain directories
            System.IO.Directory.CreateDirectory("Data\\Textures\\Terrain\\");

            for (int i = 0; i < 5; ++i)
            {
                if (ImportData.TexturePaths[i].Length > 0)
                {
                    string DestPath = System.IO.Path.Combine(@"Data\Textures\Terrain\", System.IO.Path.GetFileName(ImportData.TexturePaths[i]));
                    string RCPath = System.IO.Path.Combine(@"Terrain\", System.IO.Path.GetFileName(ImportData.TexturePaths[i]));
                    try
                    {
                        System.IO.File.Copy(ImportData.TexturePaths[i], DestPath);
                        int ID = RealmCrafter.Media.AddTextureToDatabase(RCPath, 0);
                        
                        // Add to media manager display
                        if (ID == -1)
                        {
                            MessageBox.Show("No free texture ID found!", "Error");
                        }
                        else if (ID == -2)
                        {
                            MessageBox.Show("Texture already exists in database!", "Error");
                        }
                        else if (ID < 0) //Shouldn't be seen
                        {
                            MessageBox.Show("Undefined Error adding texture!", "Error");
                        }
                        else
                        {
                            MediaDialogs.AddTexture(RCPath + Blitz.StrFromInt(0, 1), (ushort)ID, Program.GE.MediaWindow.MediaTree.Nodes[1]);

                            Program.TextureList.Add(RealmCrafter.Media.GetTextureName(ID) + " ID: " + ID);
                        }
                    }
                    catch (System.IO.IOException e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    ImportData.TexturePaths[i] = DestPath;
                }
            }

            Terrain.SetTexture(0, ImportData.TexturePaths[0]);
            Terrain.SetTexture(1, ImportData.TexturePaths[1]);
            Terrain.SetTexture(2, ImportData.TexturePaths[2]);
            Terrain.SetTexture(3, ImportData.TexturePaths[3]);
            Terrain.SetTexture(4, ImportData.TexturePaths[4]);

            Terrain.SetTextureScale(0, ImportData.TextureScales[0]);
            Terrain.SetTextureScale(1, ImportData.TextureScales[1]);
            Terrain.SetTextureScale(2, ImportData.TextureScales[2]);
            Terrain.SetTextureScale(3, ImportData.TextureScales[3]);
            Terrain.SetTextureScale(4, ImportData.TextureScales[4]);

            Terrain.SetGrassType(0, ImportData.GrassTypes[0]);
            Terrain.SetGrassType(1, ImportData.GrassTypes[1]);
            Terrain.SetGrassType(2, ImportData.GrassTypes[2]);
            Terrain.SetGrassType(3, ImportData.GrassTypes[3]);
            Terrain.SetGrassType(4, ImportData.GrassTypes[4]);
            Terrain.SetGrassType(5, ImportData.GrassTypes[5]);
            Terrain.SetGrassType(6, ImportData.GrassTypes[6]);
            Terrain.SetGrassType(7, ImportData.GrassTypes[7]);

            //Terrain.SetTextureScale(new NVector2(15, 15));

            for (int z = 0; z < ImportData.Width + 1; ++z)
            {
                for (int x = 0; x < ImportData.Width + 1; ++x)
                {
                    Terrain.SetHeight(x, z, ImportData.HeightData[(z * (ImportData.Width + 1)) + x]);
                    RealmCrafter.RCT.RCSplat tSplat = new RealmCrafter.RCT.RCSplat();
                    tSplat.Set(ImportData.SplatData[(z * (ImportData.Width + 1)) + x].S0,
                        ImportData.SplatData[(z * (ImportData.Width + 1)) + x].S1,
                        ImportData.SplatData[(z * (ImportData.Width + 1)) + x].S2,
                        ImportData.SplatData[(z * (ImportData.Width + 1)) + x].S3);
                    Terrain.SetColorChunk(new NVector2(Convert.ToSingle(x), Convert.ToSingle(z)), 0, 0, tSplat);
                    Terrain.SetGrass(x, z, ImportData.GrassData[(z * (ImportData.Width + 1)) + x]);
                }
            }

            foreach (RCTPlugin.RCTTerrainPosition Position in ImportData.ExclusionZones)
            {
                Terrain.SetExclusion(Position.X, Position.Y, true);
            }

            RealmCrafter.ClientZone.RCTTerrain T = new RealmCrafter.ClientZone.RCTTerrain(Program.GE.CurrentClientZone);
            T.Path = @".\Data\Terrains\" + Program.GE.CurrentClientZone.Name + System.Environment.TickCount.ToString() + ".te";
            T.Terrain = Terrain;
            T.Terrain.Tag = new List<Program.TerrainTagItem>();

            // Add to tree view and select
            TreeNode TN = new TreeNode("Terrain " + (Program.GE.CurrentClientZone.Terrains.IndexOf(T) + 1));
            TN.Tag = T;
            Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
            Program.GE.m_ZoneRender.WorldSelectObject(T, 3);

            Program.GE.m_ZoneList.AddObjectsCount();
            Program.GE.SetWorldSavedStatus(false);

        }

        public void UpdatePosition(float capx, float capz)
        {
            TManager.BrushPosition = new NVector2(capx, capz);

            

            if (Terrain == null && !Program.GE.RenderingPanel.Focused)
                return;



            NVector2 PaintPosition = new NVector2(capx * 0.5f, capz * 0.5f);
            PaintPosition.X += (Terrain.GetPosition().X * 0.5f);
            PaintPosition.Y += (Terrain.GetPosition().Z * 0.5f);

            //Program.GE.IsMouseDown

            if (Program.GE.IsLMouseDown || Program.GE.IsRMouseDown)
            {
                Program.GE.SetWorldSavedStatus(false);
                if (MouseLocked == false)
                {
                    MouseLocked = true;
                    TManager.EditorLocked = true;
                }

                switch (CurrentTool)
                {
                    case SelectedTerrainTool.RaiseLower:
                        {
                            try
                            {
                                float Str = Convert.ToSingle(RaiseLowerStrengthTextbox.Text);
                                //if(MouseDown == -1)
                                //   Str = -Str;
                                Terrain.Raise(TManager.BrushRadius,
                                    TManager.BrushHardness,
                                    PaintPosition,
                                    TManager.BrushIsCircle,
                                    KeyState.Get(Keys.LButton) ? Str : -Str);
                            }
                            catch (System.InvalidCastException)
                            {

                            }
                            break;
                        }
                    case SelectedTerrainTool.SetHeight:
                        {
                            try
                            {
                                if (KeyState.Get(Keys.LButton))
                                {
                                    Terrain.SetHeight(TManager.BrushRadius,
                                        TManager.BrushHardness,
                                        PaintPosition,
                                        TManager.BrushIsCircle,
                                        Convert.ToSingle(SetHeightHeightTextbox.Text));
                                }
                            }
                            catch (System.InvalidCastException)
                            {

                            }
                            break;
                        }
                    case SelectedTerrainTool.Ramp:
                        {
                            if (KeyState.Get(Keys.LButton))
                            {
                                if (FirstPlaced == false)
                                {
                                    TManager.SetSecondBrush(TManager.BrushRadius,
                                        0.0f,
                                        TManager.BrushPosition,
                                        true);
                                    FirstPos = PaintPosition;//TManager.BrushPosition;
                                    FirstRadius = TManager.BrushRadius;
                                    FirstWait = true;
                                }
                                else
                                {
                                    if (FirstWait == false)
                                    {
                                        //FirstPos.X *= 0.5f;
                                        //FirstPos.Y *= 0.5f;
                                        Terrain.Ramp(FirstPos, PaintPosition, FirstRadius * 2, TManager.BrushRadius * 2);
                                        FirstWait = true;
                                        TManager.SetSecondBrush(0.0f, 0.0f, new NVector2(0, 0), true);
                                    }
                                }
                            }
                            else if (KeyState.Get(Keys.RButton))
                            {
                                FirstPlaced = false;
                                FirstWait = false;
                                TManager.SetSecondBrush(0.0f, 0.0f, new NVector2(0, 0), true);
                            }
                            break;
                        }
                    case SelectedTerrainTool.Smooth:
                        {
                            try
                            {
                                if (KeyState.Get(Keys.LButton))
                                {
                                    Terrain.Smooth(TManager.BrushRadius,
                                        TManager.BrushHardness,
                                        PaintPosition,
                                        TManager.BrushIsCircle,
                                        1.0f);
                                }
                            }
                            catch (System.InvalidCastException)
                            {

                            }
                            break;
                        }
                    case SelectedTerrainTool.Erode:
                        {

                            break;
                        }
                    case SelectedTerrainTool.PaintHole:
                        {
                            Terrain.PaintHole(TManager.BrushRadius,
                                PaintPosition,
                                TManager.BrushIsCircle,
                                !KeyState.Get(Keys.RButton));
                            break;
                        }
                    case SelectedTerrainTool.Texture:
                        {
                            try
                            {
                                if (KeyState.Get(Keys.LButton))
                                {
                                    ListViewItem SelectedTextureItem = listView1.SelectedItems.Count > 0 ? listView1.SelectedItems[0] : null;
                                    int SelectedTexture = 0;
                                    if (SelectedTextureItem != null)
                                        SelectedTexture = Convert.ToInt32(SelectedTextureItem.Tag.ToString());

                                    if (SelectedTexture == 0)
                                        SelectedTexture = 2;
                                    else if (SelectedTexture == 2)
                                        SelectedTexture = 0;

                                    Terrain.Paint(TManager.BrushRadius,
                                        TManager.BrushHardness,
                                        PaintPosition,
                                        TManager.BrushIsCircle,
                                        Convert.ToSingle(PaintStrengthTextbox.Text),
                                        SelectedTexture,
                                        Convert.ToSingle(TextureSlope.Min), Convert.ToSingle(TextureSlope.Max), -5000.0f, 5000.0f);

                                    //Program.GE.Text = SelectedTexture.ToString();

                                    //MessageBox.Show(String.Format("Index: {0}; Tag: {1}; Selected: {2}; Slope: {3}, {4}", SelectedTexture, SelectedTextureItem.Tag, listView1.SelectedIndices[0],
                                    //    TextureSlope.Min, TextureSlope.Max));
                                }
                            }
                            catch (System.InvalidCastException)
                            {

                            }
                            break;
                        }
                    case SelectedTerrainTool.Grass:
                        {
                            try
                            {
                                byte GrassMask = 0;

                                foreach (int i in GrassSelectionList.SelectedIndices)
                                {
                                    GrassMask |= Convert.ToByte(Convert.ToInt32(Math.Pow(2.0, Convert.ToDouble(i))));
                                }

                                Terrain.PaintGrass(TManager.BrushRadius,
                                    PaintPosition,
                                    TManager.BrushIsCircle,
                                    GrassMask,
                                    KeyState.Get(Keys.LButton));
                            }
                            catch (System.InvalidCastException)
                            {

                            }
                            break;
                        }
                }
            }
            else
            {
                if (FirstWait)
                {
                    FirstPlaced = !FirstPlaced;
                    FirstWait = false;
                }

                if (MouseLocked == true)
                {
                    MouseLocked = false;
                    TManager.EditorLocked = false;
                }
            }
           
        }

        private void UpdateToolGUI()
        {
            TerrainRaiseLowerToolGroup.Visible = false;
            TerrainSetHeightToolGroup.Visible = false;
            TerrainRampToolGroup.Visible = false;
            TerrainSmoothToolGroup.Visible = false;
            TerrainErodeToolGroup.Visible = false;
            TerrainPaintToolGroup.Visible = false;
            TerrainPaintHoleToolGroup.Visible = false;
            TerrainGrassToolGroup.Visible = false;
            if (TManager != null)
                TManager.SetSecondBrush(0.0f, 0.0f, new NVector2(0, 0), true);

            switch (CurrentTool)
            {
                case SelectedTerrainTool.RaiseLower:
                    {
                        TerrainRaiseLowerToolGroup.Visible = true;
                        break;
                    }
                case SelectedTerrainTool.SetHeight:
                    {
                        TerrainSetHeightToolGroup.Visible = true;
                        break;
                    }
                case SelectedTerrainTool.Ramp:
                    {
                        if (FirstPlaced)
                        {
                            if (TManager != null)
                                TManager.SetSecondBrush(FirstRadius, 0.0f, FirstPos, true);
                        }
                        TerrainRampToolGroup.Visible = true;
                        break;
                    }
                case SelectedTerrainTool.Smooth:
                    {
                        TerrainSmoothToolGroup.Visible = true;
                        break;
                    }
                case SelectedTerrainTool.Erode:
                    {
                        TerrainErodeToolGroup.Visible = true;
                        break;
                    }
                case SelectedTerrainTool.PaintHole:
                    {
                        TerrainPaintHoleToolGroup.Visible = true;
                        break;
                    }
                case SelectedTerrainTool.Texture:
                    {
                        TerrainPaintToolGroup.Visible = true;
                        break;
                    }
                case SelectedTerrainTool.Grass:
                    {
                        TerrainGrassToolGroup.Visible = true;
                        break;
                    }
            }

            try
            {
                UpdateManagerBrush();
            }
            catch
            { }
        }

        private void UpdateManagerBrush()
        {
            switch (CurrentTool)
            {
                case SelectedTerrainTool.RaiseLower:
                    {
                        TManager.BrushRadius = Convert.ToSingle(RaiseLowerRadiusTextbox.Text);
                        TManager.BrushHardness = Convert.ToSingle(RaiseLowerHardnessTextbox.Text);
                        TManager.BrushIsCircle = !RaiseLowerSquareBrushCheckbox.Checked;
                        break;
                    }
                case SelectedTerrainTool.SetHeight:
                    {
                        TManager.BrushRadius = Convert.ToSingle(SetHeightRadiusTextbox.Text);
                        TManager.BrushHardness = 1.0f;
                        TManager.BrushIsCircle = !SetHeightSquareBrushCheckbox.Checked;
                        break;
                    }
                case SelectedTerrainTool.Ramp:
                    {
                        TManager.BrushRadius = Convert.ToSingle(RampRadiusTextbox.Text);
                        TManager.BrushHardness = 1.0f;
                        TManager.BrushIsCircle = true;
                        break;
                    }
                case SelectedTerrainTool.Smooth:
                    {
                        TManager.BrushRadius = Convert.ToSingle(SmoothRadiusTextbox.Text);
                        TManager.BrushHardness = 1.0f;
                        TManager.BrushIsCircle = !SmoothSquareBrushCheckbox.Checked;
                        break;
                    }
                case SelectedTerrainTool.Erode:
                    {

                        break;
                    }
                case SelectedTerrainTool.PaintHole:
                    {
                        TManager.BrushRadius = Convert.ToSingle(HoleRadiusTextbox.Text);
                        TManager.BrushHardness = 1.0f;
                        TManager.BrushIsCircle = !HoleSquareBrushCheckbox.Checked;
                        break;
                    }
                case SelectedTerrainTool.Texture:
                    {
                        TManager.BrushRadius = Convert.ToSingle(PaintRadiusTextbox.Text);
                        TManager.BrushHardness = Convert.ToSingle(PaintHardnessTextbox.Text);
                        TManager.BrushIsCircle = !PaintSquareBrushCheckbox.Checked;
                        break;
                    }
                case SelectedTerrainTool.Grass:
                    {
                        TManager.BrushRadius = Convert.ToSingle(GrassRadiusTextbox.Text);
                        TManager.BrushHardness = 1.0f;
                        TManager.BrushIsCircle = !GrassSquareBrushCheckbox.Checked;
                        break;
                    }
            }
        }


        private void RaiseLowerRadiusTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(RaiseLowerRadiusTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            RaiseLowerRadiusTextbox.Text = V;
        }

        private void RaiseLowerHardnessTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = (Convert.ToSingle(RaiseLowerHardnessTrackbar.Value) / 100.0f).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            RaiseLowerHardnessTextbox.Text = V;
        }

        private void RaiseLowerStrengthTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = (Convert.ToSingle(RaiseLowerStrengthTrackbar.Value) / 100.0f).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            RaiseLowerStrengthTextbox.Text = V;
        }

        private void RaiseLowerSquareBrushCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagerBrush();
        }

        private void RaiseLowerRadiusTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(RaiseLowerRadiusTextbox.Text);
            }
            catch
            {
                RaiseLowerRadiusTextbox.Text = "0.00";
                RaiseLowerRadiusTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                RaiseLowerRadiusTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void RaiseLowerHardnessTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(RaiseLowerHardnessTextbox.Text);
            }
            catch
            {
                RaiseLowerHardnessTextbox.Text = "0.00";
                RaiseLowerHardnessTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                V *= 100.0f;
                RaiseLowerHardnessTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void RaiseLowerStrengthTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(RaiseLowerStrengthTextbox.Text);
            }
            catch
            {
                RaiseLowerStrengthTextbox.Text = "0.00";
                RaiseLowerStrengthTrackbar.Value = 0;
            }

            try
            {
                UpdateManagerBrush();
                V *= 100.0f;
                RaiseLowerStrengthTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void SetHeightRadiusTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(SetHeightRadiusTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            SetHeightRadiusTextbox.Text = V;
        }

        private void SetHeightHeightTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(SetHeightHeightTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            SetHeightHeightTextbox.Text = V;
        }

        private void SetHeightSquareBrushCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagerBrush();
        }

        private void SetHeightRadiusTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(SetHeightRadiusTextbox.Text);
            }
            catch
            {
                SetHeightRadiusTextbox.Text = "0.00";
                SetHeightRadiusTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                SetHeightRadiusTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void SetHeightHeightTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(SetHeightHeightTextbox.Text);
            }
            catch
            {
                SetHeightHeightTextbox.Text = "0.00";
                SetHeightHeightTrackbar.Value = 0;
            }

            try
            {
                UpdateManagerBrush();
                SetHeightHeightTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void SmoothRadiusTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(SmoothRadiusTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            SmoothRadiusTextbox.Text = V;
        }

        private void SmoothSquareBrushCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagerBrush();
        }

        private void SmoothRadiusTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(SmoothRadiusTextbox.Text);
            }
            catch
            {
                SmoothRadiusTextbox.Text = "0.00";
                SmoothRadiusTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                SmoothRadiusTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }


        private void HoleRadiusTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(HoleRadiusTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            HoleRadiusTextbox.Text = V;
        }

        private void HoleRadiusTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(HoleRadiusTextbox.Text);
            }
            catch
            {
                HoleRadiusTextbox.Text = "0.00";
                HoleRadiusTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                HoleRadiusTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void HoleSquareBrushCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagerBrush();
        }


        private void PaintRadiusTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(PaintRadiusTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            PaintRadiusTextbox.Text = V;
        }

        private void PaintHardnessTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = (Convert.ToSingle(PaintHardnessTrackbar.Value) / 100.0f).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            PaintHardnessTextbox.Text = V;
        }

        private void PaintStrengthTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = (Convert.ToSingle(PaintStrengthTrackbar.Value) / 100.0f).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            PaintStrengthTextbox.Text = V;
        }

        private void PaintRadiusTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(PaintRadiusTextbox.Text);
            }
            catch
            {
                PaintRadiusTextbox.Text = "0.00";
                PaintRadiusTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                PaintRadiusTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void PaintHardnessTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(PaintHardnessTextbox.Text);
            }
            catch
            {
                PaintHardnessTextbox.Text = "0.00";
                PaintHardnessTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                V *= 100.0f;
                PaintHardnessTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void PaintStrengthTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(PaintStrengthTextbox.Text);
            }
            catch
            {
                PaintStrengthTextbox.Text = "0.00";
                PaintStrengthTrackbar.Value = 0;
            }

            try
            {
                UpdateManagerBrush();
                V *= 100.0f;
                PaintStrengthTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void PaintSquareBrushCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagerBrush();
        }

        private void RaiseLowerTool_Click(object sender, EventArgs e)
        {
            CurrentTool = SelectedTerrainTool.RaiseLower;
            UpdateToolGUI();
        }

        private void SetHeightTool_Click(object sender, EventArgs e)
        {
            CurrentTool = SelectedTerrainTool.SetHeight;
            UpdateToolGUI();
        }

        private void RampTool_Click(object sender, EventArgs e)
        {
            CurrentTool = SelectedTerrainTool.Ramp;
            UpdateToolGUI();
        }

        private void SmoothTool_Click(object sender, EventArgs e)
        {
            CurrentTool = SelectedTerrainTool.Smooth;
            UpdateToolGUI();
        }

        private void ErodeTool_Click(object sender, EventArgs e)
        {
            CurrentTool = SelectedTerrainTool.Erode;
            UpdateToolGUI();
        }

        private void PaintTool_Click(object sender, EventArgs e)
        {
            CurrentTool = SelectedTerrainTool.Texture;
            UpdateToolGUI();
        }

        private void PaintHoleTool_Click(object sender, EventArgs e)
        {
            //Terrain.SetGrassType(0, "Test Grass 2");
            CurrentTool = SelectedTerrainTool.PaintHole;
            UpdateToolGUI();
        }


        private void GrassTool_Click(object sender, EventArgs e)
        {
            CurrentTool = SelectedTerrainTool.Grass;
            UpdateToolGUI();
        }


        private void GrassRadiusTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(GrassRadiusTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            GrassRadiusTextbox.Text = V;
        }

        private void GrassRadiusTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(GrassRadiusTextbox.Text);
            }
            catch
            {
                GrassRadiusTextbox.Text = "0.00";
                GrassRadiusTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                GrassRadiusTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void GrassSquareBrushCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagerBrush();
        }

        private void GrassAssignType_Click(object sender, EventArgs e)
        {
            if (GrassSelectionList.SelectedIndex == -1)
                return;

            GrassSelector.SelectedType = GrassSelectionList.SelectedItem as string;
            DialogResult Dr = GrassSelector.ShowDialog();

            if (Dr == DialogResult.Cancel)
                return;

            Terrain.SetGrassType(GrassSelectionList.SelectedIndex, GrassSelector.SelectedType);
            int Index = GrassSelectionList.SelectedIndex;
            GrassSelectionList.Items[Index] = GrassSelector.SelectedType;
            GrassSelectionList.SelectedIndex = Index;
        }

        private void GrassUnassignType_Click(object sender, EventArgs e)
        {
            if (GrassSelectionList.SelectedIndex == -1)
                return;

            Terrain.SetGrassType(GrassSelectionList.SelectedIndex, "");
            int Index = GrassSelectionList.SelectedIndex;
            GrassSelectionList.Items[Index] = "Unassigned";
            GrassSelectionList.SelectedIndex = Index;
        }

        private void RampRadiusTrackbar_Scroll(object sender, EventArgs e)
        {
            String V = Convert.ToSingle(RampRadiusTrackbar.Value).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            RampRadiusTextbox.Text = V;
        }

        private void RampRadiusTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(RampRadiusTextbox.Text);
            }
            catch
            {
                RampRadiusTextbox.Text = "0.00";
                RampRadiusTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                RampRadiusTrackbar.Value = Convert.ToInt32(V);
            }
            catch
            {

            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            AssignTextureButton_Click(sender, e);
        }

        private void AssignTextureButton_Click(object sender, EventArgs e)
        {
            if (Terrain == null)
                return;

            ListViewItem SelectedTextureItem = listView1.SelectedItems.Count > 0 ? listView1.SelectedItems[0] : null;
            int SelectedTexture = 0;
            if (SelectedTextureItem != null)
                SelectedTexture = Convert.ToInt32(SelectedTextureItem.Tag.ToString());

            // TODO: Make this part GE compatible!
            //if (TempFindTexture.ShowDialog() == DialogResult.Cancel)
            //    return;
            ushort ID = MediaDialogs.GetTexture(false, 65535);
            if (ID == 65535)
            {

            }
            else
            {
                string Path = RealmCrafter.Media.GetTextureName(ID);
                Path = Path.Substring(0, Path.Length - 1);
                Path = System.IO.Path.Combine(@".\Data\Textures\", Path);
                Terrain.SetTexture(SelectedTexture, Path);
            }


            //string Path = TempFindTexture.FileName;
            //string Path = "";

            //Terrain.SetTexture(SelectedTexture, Path);
            ResetPreviewTextures();
        }


        public void ResetPreviewTextures()
        {
            if (Terrain == null)
                return;

            for (int i = 0; i < 5; ++i)
            {
                string TexPath = Terrain.GetTexture(i);
                Bitmap Img = RenderingServices.RenderWrapper.LoadTextureBitmap(TexPath);


                string Filename = System.IO.Path.GetFileName(TexPath);

//                if (Img == null)
//                    Filename += " (ERR)";

                for (int f = 0; f < 5; ++f)
                    //if (Convert.ToInt32(listView1.Items[f].Tag.ToString()) == i)
                    if (listView1.Items[f].ImageIndex == i)
                    {
                        listView1.Items[f].Text = Filename;
                        //listView1.Items[f].ImageIndex = i;
                    }

                //listView1.Items[i].ImageIndex = i;
                //listView1.Items[i].Text = Filename;

                Image I = TextureImageListBase.Images[i];
                if (Img != null)
                    I = Img.GetThumbnailImage(48, 48, null, IntPtr.Zero);

                TextureImageList.Images[i] = I;
            }
        }

        public void LoadToolOptions(String path)
        {
            // Reset all values
            RaiseLowerRadiusTrackbar.Value = 1;
            RaiseLowerHardnessTrackbar.Value = 1;
            RaiseLowerStrengthTrackbar.Value = 0;
            SetHeightRadiusTrackbar.Value = 1;
            SetHeightHeightTrackbar.Value = 0;
            SmoothRadiusTrackbar.Value = 1;
            PaintRadiusTrackbar.Value = 1;
            PaintHardnessTrackbar.Value = 1;
            PaintStrengthTrackbar.Value = 0;
            GrassRadiusTrackbar.Value = 1;
            RampRadiusTrackbar.Value = 1;

            XmlTextReader X = null;

            try
            {
                // Open file
                X = new XmlTextReader(path);

                // Read elements
                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("toolitem", StringComparison.CurrentCultureIgnoreCase))
                    {
                        String Name = X.GetAttribute("name");
                        String Value = X.GetAttribute("value");

                        try
                        {
                            if (Name.Equals("RaiseLowerRadius", StringComparison.CurrentCultureIgnoreCase))
                                RaiseLowerRadiusTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("RaiseLowerHardness", StringComparison.CurrentCultureIgnoreCase))
                                RaiseLowerHardnessTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("RaiseLowerStength", StringComparison.CurrentCultureIgnoreCase))
                                RaiseLowerStrengthTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("SetHeightRadius", StringComparison.CurrentCultureIgnoreCase))
                                SetHeightRadiusTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("SetHeightHeight", StringComparison.CurrentCultureIgnoreCase))
                                SetHeightHeightTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("SmoothRadius", StringComparison.CurrentCultureIgnoreCase))
                                SmoothRadiusTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("PaintRadius", StringComparison.CurrentCultureIgnoreCase))
                                PaintRadiusTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("PaintHardness", StringComparison.CurrentCultureIgnoreCase))
                                PaintHardnessTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("PaintStrength", StringComparison.CurrentCultureIgnoreCase))
                                PaintStrengthTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("PaintHoleRadius", StringComparison.CurrentCultureIgnoreCase))
                                HoleRadiusTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("GrassRadius", StringComparison.CurrentCultureIgnoreCase))
                                GrassRadiusTrackbar.Value = Convert.ToInt32(Value);
                            if (Name.Equals("RampRadius", StringComparison.CurrentCultureIgnoreCase))
                                RampRadiusTrackbar.Value = Convert.ToInt32(Value);

                            if (Name.Equals("TextureMin0", StringComparison.CurrentCultureIgnoreCase))
                                StoredMins[0] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMin1", StringComparison.CurrentCultureIgnoreCase))
                                StoredMins[1] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMin2", StringComparison.CurrentCultureIgnoreCase))
                                StoredMins[2] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMin3", StringComparison.CurrentCultureIgnoreCase))
                                StoredMins[3] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMin4", StringComparison.CurrentCultureIgnoreCase))
                                StoredMins[4] = Convert.ToDouble(Value);

                            if (Name.Equals("TextureMax0", StringComparison.CurrentCultureIgnoreCase))
                                StoredMaxs[0] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMax1", StringComparison.CurrentCultureIgnoreCase))
                                StoredMaxs[1] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMax2", StringComparison.CurrentCultureIgnoreCase))
                                StoredMaxs[2] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMax3", StringComparison.CurrentCultureIgnoreCase))
                                StoredMaxs[3] = Convert.ToDouble(Value);
                            if (Name.Equals("TextureMax4", StringComparison.CurrentCultureIgnoreCase))
                                StoredMaxs[4] = Convert.ToDouble(Value);

                            if (Name.Equals("AutoTextureMin0", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMins[0] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMin1", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMins[1] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMin2", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMins[2] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMin3", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMins[3] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMin4", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMins[4] = Convert.ToDouble(Value);

                            if (Name.Equals("AutoTextureMax0", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMaxs[0] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMax1", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMaxs[1] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMax2", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMaxs[2] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMax3", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMaxs[3] = Convert.ToDouble(Value);
                            if (Name.Equals("AutoTextureMax4", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredMaxs[4] = Convert.ToDouble(Value);


                            if (Name.Equals("AutoTextureHeightMin0", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMins[0] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMin1", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMins[1] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMin2", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMins[2] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMin3", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMins[3] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMin4", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMins[4] = Convert.ToInt32(Value);

                            if (Name.Equals("AutoTextureHeightMax0", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMaxs[0] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMax1", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMaxs[1] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMax2", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMaxs[2] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMax3", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMaxs[3] = Convert.ToInt32(Value);
                            if (Name.Equals("AutoTextureHeightMax4", StringComparison.CurrentCultureIgnoreCase))
                                AutoStoredHeightMaxs[4] = Convert.ToInt32(Value);

                            
                        }
                        catch
                        {
                            // Normally an invalid input (will be ignored)
                        }

                    }

                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                // So what?
            }
            catch (Exception e)
            {
                throw new Exception("LoadToolOptions() failed: \n" + e.Message);
            }
            finally
            {
                // Cleanup
                if (X != null)
                    X.Close();
            }

            TextureSlope.Min = StoredMins[0];
            TextureSlope.Max = StoredMaxs[0];
            RaiseLowerRadiusTrackbar_Scroll(RaiseLowerRadiusTrackbar, EventArgs.Empty);
            RaiseLowerHardnessTrackbar_Scroll(RaiseLowerHardnessTrackbar, EventArgs.Empty);
            RaiseLowerStrengthTrackbar_Scroll(RaiseLowerStrengthTrackbar, EventArgs.Empty);
            SetHeightRadiusTrackbar_Scroll(SetHeightRadiusTrackbar, EventArgs.Empty);
            SetHeightHeightTrackbar_Scroll(SetHeightHeightTrackbar, EventArgs.Empty);
            SmoothRadiusTrackbar_Scroll(SmoothRadiusTrackbar, EventArgs.Empty);
            PaintRadiusTrackbar_Scroll(PaintRadiusTrackbar, EventArgs.Empty);
            PaintHardnessTrackbar_Scroll(PaintHardnessTrackbar, EventArgs.Empty);
            PaintStrengthTrackbar_Scroll(PaintStrengthTrackbar, EventArgs.Empty);
            HoleRadiusTrackbar_Scroll(HoleRadiusTrackbar, EventArgs.Empty);
            GrassRadiusTrackbar_Scroll(GrassRadiusTrackbar, EventArgs.Empty);
            RampRadiusTrackbar_Scroll(RampRadiusTrackbar, EventArgs.Empty);
        }

        private void WriteTool(XmlTextWriter x, String name, String value)
        {
            x.WriteStartElement("toolitem");

            x.WriteStartAttribute("name");
            x.WriteString(name);
            x.WriteEndAttribute();

            x.WriteStartAttribute("value");
            x.WriteString(value);
            x.WriteEndAttribute();

            x.WriteEndElement();
        }

        public void SaveToolOptions(String path)
        {
            try
            {
                // Write XML file
                XmlTextWriter X = new XmlTextWriter(path, Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                // Base parts
                X.WriteStartDocument();
                X.WriteStartElement("terraintools");

                WriteTool(X, "RaiseLowerRadius", RaiseLowerRadiusTrackbar.Value.ToString());
                WriteTool(X, "RaiseLowerHardness", RaiseLowerHardnessTrackbar.Value.ToString());
                WriteTool(X, "RaiseLowerStength", RaiseLowerStrengthTrackbar.Value.ToString());
                WriteTool(X, "SetHeightRadius", SetHeightRadiusTrackbar.Value.ToString());
                WriteTool(X, "SetHeightHeight", SetHeightHeightTrackbar.Value.ToString());
                WriteTool(X, "SmoothRadius", SmoothRadiusTrackbar.Value.ToString());
                WriteTool(X, "PaintRadius", PaintRadiusTrackbar.Value.ToString());
                WriteTool(X, "PaintHardness", PaintHardnessTrackbar.Value.ToString());
                WriteTool(X, "PaintStrength", PaintStrengthTrackbar.Value.ToString());
                WriteTool(X, "PaintHoleRadius", HoleRadiusTrackbar.Value.ToString());
                WriteTool(X, "GrassRadius", GrassRadiusTrackbar.Value.ToString());
                WriteTool(X, "RampRadius", RampRadiusTrackbar.Value.ToString());

                WriteTool(X, "TextureMin0", StoredMins[0].ToString());
                WriteTool(X, "TextureMin1", StoredMins[1].ToString());
                WriteTool(X, "TextureMin2", StoredMins[2].ToString());
                WriteTool(X, "TextureMin3", StoredMins[3].ToString());
                WriteTool(X, "TextureMin4", StoredMins[4].ToString());
                WriteTool(X, "TextureMax0", StoredMaxs[0].ToString());
                WriteTool(X, "TextureMax1", StoredMaxs[1].ToString());
                WriteTool(X, "TextureMax2", StoredMaxs[2].ToString());
                WriteTool(X, "TextureMax3", StoredMaxs[3].ToString());
                WriteTool(X, "TextureMax4", StoredMaxs[4].ToString());
                WriteTool(X, "AutoTextureMin0", AutoStoredMins[0].ToString());
                WriteTool(X, "AutoTextureMin1", AutoStoredMins[1].ToString());
                WriteTool(X, "AutoTextureMin2", AutoStoredMins[2].ToString());
                WriteTool(X, "AutoTextureMin3", AutoStoredMins[3].ToString());
                WriteTool(X, "AutoTextureMin4", AutoStoredMins[4].ToString());
                WriteTool(X, "AutoTextureMax0", AutoStoredMaxs[0].ToString());
                WriteTool(X, "AutoTextureMax1", AutoStoredMaxs[1].ToString());
                WriteTool(X, "AutoTextureMax2", AutoStoredMaxs[2].ToString());
                WriteTool(X, "AutoTextureMax3", AutoStoredMaxs[3].ToString());
                WriteTool(X, "AutoTextureMax4", AutoStoredMaxs[4].ToString());

                WriteTool(X, "AutoTextureHeightMin0", AutoStoredHeightMins[0].ToString());
                WriteTool(X, "AutoTextureHeightMin1", AutoStoredHeightMins[1].ToString());
                WriteTool(X, "AutoTextureHeightMin2", AutoStoredHeightMins[2].ToString());
                WriteTool(X, "AutoTextureHeightMin3", AutoStoredHeightMins[3].ToString());
                WriteTool(X, "AutoTextureHeightMin4", AutoStoredHeightMins[4].ToString());
                WriteTool(X, "AutoTextureHeightMax0", AutoStoredHeightMaxs[0].ToString());
                WriteTool(X, "AutoTextureHeightMax1", AutoStoredHeightMaxs[1].ToString());
                WriteTool(X, "AutoTextureHeightMax2", AutoStoredHeightMaxs[2].ToString());
                WriteTool(X, "AutoTextureHeightMax3", AutoStoredHeightMaxs[3].ToString());
                WriteTool(X, "AutoTextureHeightMax4", AutoStoredHeightMaxs[4].ToString());


                // Close and clean
                X.WriteEndElement();
                X.WriteEndDocument();
                X.Flush();
                X.Close();
            }
            catch (Exception e)
            {
                // Eek an error
                MessageBox.Show("Error: " + e.Message, "Save Terrain Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PaintScaleTrackbar_Scroll(object sender, EventArgs e)
        {
            Program.GE.SetWorldSavedStatus(false);
            String V = (Convert.ToSingle(PaintScaleTrackbar.Value) / 50.0f).ToString();
            if (V.IndexOf(".") == -1)
                V += ".00";
            PaintScaleTextbox.Text = V;

            int TexIndex = (listView1.SelectedIndices.Count > 0) ? listView1.SelectedIndices[0] : 0;
            if (TexIndex < 0)
                TexIndex = 0;
            if (Terrain != null)
                Terrain.SetTextureScale(TexIndex, 1.0f / (Convert.ToSingle(PaintScaleTrackbar.Value) / 50.0f));
        }

        private void PaintScaleTextbox_TextChanged(object sender, EventArgs e)
        {
            Program.GE.SetWorldSavedStatus(false);
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(PaintScaleTextbox.Text) * 50.0f;
            }
            catch
            {
                PaintScaleTextbox.Text = "0.00";
                PaintScaleTrackbar.Value = 1;
            }

            try
            {
                UpdateManagerBrush();
                PaintScaleTrackbar.Value = Convert.ToInt32(V);

                int TexIndex = (listView1.SelectedIndices.Count > 0) ? listView1.SelectedIndices[0] : 0;
                if (TexIndex < 0)
                    TexIndex = 0;
                if (Terrain != null)
                    Terrain.SetTextureScale(TexIndex, 1.0f / (V / 50.0f));
            }
            catch
            {

            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LastSelected > -1 && LastSelected < 5)
            {
                StoredMins[LastSelected] = TextureSlope.Min;
                StoredMaxs[LastSelected] = TextureSlope.Max;
            }

            int TexIndex = (listView1.SelectedIndices.Count > 0) ? listView1.SelectedIndices[0] : 0;
            if (TexIndex < 0)
                TexIndex = 0;
            PaintScaleTrackbar.Value = Convert.ToInt32((1.0f / Terrain.GetTextureScale(TexIndex) * 50.0f));
            PaintScaleTrackbar_Scroll(PaintScaleTrackbar, EventArgs.Empty);

            

            LastSelected = TexIndex;
            if (LastSelected > -1 && LastSelected < 5)
            {
                TextureSlope.Min = StoredMins[LastSelected];
                TextureSlope.Max = StoredMaxs[LastSelected];
            }
        }

        public void AutoTextureTerrain()
        {
            if (Terrain == null && editMode == false)
                return;

            D.ConfigureTexture0.SlopeMin = AutoStoredMins[0];
            D.ConfigureTexture1.SlopeMin = AutoStoredMins[1];
            D.ConfigureTexture2.SlopeMin = AutoStoredMins[2];
            D.ConfigureTexture3.SlopeMin = AutoStoredMins[3];
            D.ConfigureTexture4.SlopeMin = AutoStoredMins[4];

            D.ConfigureTexture0.SlopeMax = AutoStoredMaxs[0];
            D.ConfigureTexture1.SlopeMax = AutoStoredMaxs[1];
            D.ConfigureTexture2.SlopeMax = AutoStoredMaxs[2];
            D.ConfigureTexture3.SlopeMax = AutoStoredMaxs[3];
            D.ConfigureTexture4.SlopeMax = AutoStoredMaxs[4];

            D.ConfigureTexture0.HeightMin = AutoStoredHeightMins[0];
            D.ConfigureTexture1.HeightMin = AutoStoredHeightMins[1];
            D.ConfigureTexture2.HeightMin = AutoStoredHeightMins[2];
            D.ConfigureTexture3.HeightMin = AutoStoredHeightMins[3];
            D.ConfigureTexture4.HeightMin = AutoStoredHeightMins[4];

            D.ConfigureTexture0.HeightMax = AutoStoredHeightMaxs[0];
            D.ConfigureTexture1.HeightMax = AutoStoredHeightMaxs[1];
            D.ConfigureTexture2.HeightMax = AutoStoredHeightMaxs[2];
            D.ConfigureTexture3.HeightMax = AutoStoredHeightMaxs[3];
            D.ConfigureTexture4.HeightMax = AutoStoredHeightMaxs[4];

            D.ConfigureTexture0.TexturePath = Terrain.GetTexture(0);
            D.ConfigureTexture1.TexturePath = Terrain.GetTexture(1);
            D.ConfigureTexture2.TexturePath = Terrain.GetTexture(2);
            D.ConfigureTexture3.TexturePath = Terrain.GetTexture(3);
            D.ConfigureTexture4.TexturePath = Terrain.GetTexture(4);

            D.ConfigureTexture0.AllowTextureChange = false;
            D.ConfigureTexture1.AllowTextureChange = false;
            D.ConfigureTexture2.AllowTextureChange = false;
            D.ConfigureTexture3.AllowTextureChange = false;
            D.ConfigureTexture4.AllowTextureChange = false;

            if (D.ShowDialog() == DialogResult.Cancel)
                return;

            AutoStoredMins[0] = D.ConfigureTexture0.SlopeMin;
            AutoStoredMins[1] = D.ConfigureTexture1.SlopeMin;
            AutoStoredMins[2] = D.ConfigureTexture2.SlopeMin;
            AutoStoredMins[3] = D.ConfigureTexture3.SlopeMin;
            AutoStoredMins[4] = D.ConfigureTexture4.SlopeMin;

            AutoStoredMaxs[0] = D.ConfigureTexture0.SlopeMax;
            AutoStoredMaxs[1] = D.ConfigureTexture1.SlopeMax;
            AutoStoredMaxs[2] = D.ConfigureTexture2.SlopeMax;
            AutoStoredMaxs[3] = D.ConfigureTexture3.SlopeMax;
            AutoStoredMaxs[4] = D.ConfigureTexture4.SlopeMax;

            AutoStoredHeightMins[0] = D.ConfigureTexture0.HeightMin;
            AutoStoredHeightMins[1] = D.ConfigureTexture1.HeightMin;
            AutoStoredHeightMins[2] = D.ConfigureTexture2.HeightMin;
            AutoStoredHeightMins[3] = D.ConfigureTexture3.HeightMin;
            AutoStoredHeightMins[4] = D.ConfigureTexture4.HeightMin;

            AutoStoredHeightMins[0] = D.ConfigureTexture0.HeightMax;
            AutoStoredHeightMaxs[1] = D.ConfigureTexture1.HeightMax;
            AutoStoredHeightMaxs[2] = D.ConfigureTexture2.HeightMax;
            AutoStoredHeightMaxs[3] = D.ConfigureTexture3.HeightMax;
            AutoStoredHeightMaxs[4] = D.ConfigureTexture4.HeightMax;

            float Center = Convert.ToSingle(Terrain.GetSize()) / 2.0f;
            NVector2 BrushPosition = new NVector2(Center, Center);
            float BrushRadius = Center + 1;
            bool BrushIsCircle = false;

            //Terrain.Paint(50.0f, 1.0f, new NVector2(0, 0), false, 1.0f, 3, 1, 0, -1000, 5000);

            Terrain.Paint(BrushRadius, 1.0f, BrushPosition, BrushIsCircle,
                1.0f, 0,
                Convert.ToSingle(D.ConfigureTexture0.SlopeMin),
                Convert.ToSingle(D.ConfigureTexture0.SlopeMax),
                Convert.ToSingle(D.ConfigureTexture0.HeightMin),
                Convert.ToSingle(D.ConfigureTexture0.HeightMax));

            Terrain.Paint(BrushRadius, 1.0f, BrushPosition, BrushIsCircle,
                1.0f, 1,
                Convert.ToSingle(D.ConfigureTexture1.SlopeMin),
                Convert.ToSingle(D.ConfigureTexture1.SlopeMax),
                Convert.ToSingle(D.ConfigureTexture1.HeightMin),
                Convert.ToSingle(D.ConfigureTexture1.HeightMax));

            Terrain.Paint(BrushRadius, 1.0f, BrushPosition, BrushIsCircle,
                1.0f, 2,
                Convert.ToSingle(D.ConfigureTexture2.SlopeMin),
                Convert.ToSingle(D.ConfigureTexture2.SlopeMax),
                Convert.ToSingle(D.ConfigureTexture2.HeightMin),
                Convert.ToSingle(D.ConfigureTexture2.HeightMax));

            Terrain.Paint(BrushRadius, 1.0f, BrushPosition, BrushIsCircle,
                1.0f, 3,
                Convert.ToSingle(D.ConfigureTexture3.SlopeMin),
                Convert.ToSingle(D.ConfigureTexture3.SlopeMax),
                Convert.ToSingle(D.ConfigureTexture3.HeightMin),
                Convert.ToSingle(D.ConfigureTexture3.HeightMax));

            Terrain.Paint(BrushRadius, 1.0f, BrushPosition, BrushIsCircle,
                1.0f, 4,
                Convert.ToSingle(D.ConfigureTexture4.SlopeMin),
                Convert.ToSingle(D.ConfigureTexture4.SlopeMax),
                Convert.ToSingle(D.ConfigureTexture4.HeightMin),
                Convert.ToSingle(D.ConfigureTexture4.HeightMax));

            //MessageBox.Show(String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", BrushRadius, 1.0f, BrushPosition, BrushIsCircle,
            //    1.0f, 3,
            //    Convert.ToSingle(D.ConfigureTexture3.SlopeMin),
            //    Convert.ToSingle(D.ConfigureTexture3.SlopeMax),
            //    Convert.ToSingle(D.ConfigureTexture3.HeightMin),
            //    Convert.ToSingle(D.ConfigureTexture3.HeightMax)));


            SmoothBlendTerrainTextures(Terrain);
        }

        private void SmoothBlendTerrainTextures(RealmCrafter.RCT.RCTerrain terrain)
        {
            int TerrainWidth = terrain.GetSize() + 1;
            RealmCrafter.RCT.RCSplat[] Splats = new RealmCrafter.RCT.RCSplat[TerrainWidth * TerrainWidth];

            for (int z = 0; z < TerrainWidth; ++z)
            {
                for (int x = 0; x < TerrainWidth; ++x)
                {
                    RealmCrafter.RCT.RCSplat Up = terrain.GetColorChunk(new NVector2(0, 0), x, z + 1);
                    RealmCrafter.RCT.RCSplat Down = terrain.GetColorChunk(new NVector2(0, 0), x, z - 1);
                    RealmCrafter.RCT.RCSplat Right = terrain.GetColorChunk(new NVector2(0, 0), x + 1, z);
                    RealmCrafter.RCT.RCSplat Left = terrain.GetColorChunk(new NVector2(0, 0), x - 1, z);
                    RealmCrafter.RCT.RCSplat Center = terrain.GetColorChunk(new NVector2(0, 0), x, z);

                    float S0 = Convert.ToSingle(Up.S0)
                        + Convert.ToSingle(Down.S0)
                        + Convert.ToSingle(Right.S0)
                        + Convert.ToSingle(Left.S0)
                        + Convert.ToSingle(Center.S0);

                    float S1 = Convert.ToSingle(Up.S1)
                        + Convert.ToSingle(Down.S1)
                        + Convert.ToSingle(Right.S1)
                        + Convert.ToSingle(Left.S1)
                        + Convert.ToSingle(Center.S1);

                    float S2 = Convert.ToSingle(Up.S2)
                        + Convert.ToSingle(Down.S2)
                        + Convert.ToSingle(Right.S2)
                        + Convert.ToSingle(Left.S2)
                        + Convert.ToSingle(Center.S2);

                    float S3 = Convert.ToSingle(Up.S3)
                        + Convert.ToSingle(Down.S3)
                        + Convert.ToSingle(Right.S3)
                        + Convert.ToSingle(Left.S3)
                        + Convert.ToSingle(Center.S3);

                    S0 /= 5.0f;
                    S1 /= 5.0f;
                    S2 /= 5.0f;
                    S3 /= 5.0f;

                    RealmCrafter.RCT.RCSplat Out = new RealmCrafter.RCT.RCSplat();
                    Out.Set(Convert.ToByte(S0),
                        Convert.ToByte(S1),
                        Convert.ToByte(S2),
                        Convert.ToByte(S3));

                    Splats[z * TerrainWidth + x] = Out;
                }
            }

            for (int z = 0; z < TerrainWidth; ++z)
            {
                for (int x = 0; x < TerrainWidth; ++x)
                {
                    terrain.SetColorChunk(new NVector2(0, 0), x, z, Splats[z * TerrainWidth + x]);
                }
            }
        }




    }

    public enum SelectedTerrainTool
    {
        RaiseLower,
        SetHeight,
        Ramp,
        Smooth,
        Erode,
        PaintHole,
        Texture,
        Grass
    }
}