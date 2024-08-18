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
// Realm Crafter GE for RC 2.x, by Rob W (rottbott@hotmail.com)
// Original version February 2005, C# version October 2006

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using IrrlichtSound;
using RealmCrafter;
using RealmCrafter.ClientZone;
using RealmCrafter.ServerZone;
using RealmCrafter_GE.Dockable_Forms;
using RealmCrafter_GE.Property_Interfaces;
using RealmCrafter_GE.Forms;
using RealmCrafter_GE.Utilities.Fullscreen;
using RenderingServices;
using RottParticles;
using RealmCrafter_GE.PostProcess;
using Emitter=RealmCrafter.ClientZone.Emitter;
using Environment=RealmCrafter.Environment;
using Light=RenderingServices.Light;
using Timer=MLib.Timer;
using Zone=RealmCrafter.ClientZone.Zone;
using WeifenLuo.WinFormsUI.Docking;
using NGUINet;
using RealmCrafter_GE.Dockable_Forms.GubbinEditor;
using System.Text.RegularExpressions;

namespace RealmCrafter_GE
{
	public sealed partial class GE : Form
	{
		// The manager is in charge of all actions related to NGUI
		public static NGUIManager GUIManager;

		// The manager is in charge of all actions related to RCT
		public static RealmCrafter.RCT.TerrainManager TerrainManager;
		public static RCTTest.GrassTypesEditor GrassTypesEditor;

		// These parameters are passed to the GUIManager when it updates.
		// It stores the keypresses and mouse information
		public NGUIUpdateParameters Parameters = new NGUIUpdateParameters();

		// Rendering Panel original location (To hand control back)

		private ConfigureParameter ParametersForm = null;

        public MediaManager MediaWindow;

		#region ActorsTabControlSelectedIndex enum
		public enum ActorsTabControlSelectedIndex
		{
			ACTORS = 0,
			FACTIONS,
			ANIMATION,
			ABILITIES,
			SETTINGS,
			MESH_ADJUSTMENT,
			CHARACTER_SET
		} ;
		#endregion

		#region ControlHostSelectedIndex enum
		public enum GETab
		{
            WORLD = 0,
			ACTORS = 1,
			ITEMS = 2,			
            POSTPROCESS = 3,
            EMITTER = 4,
            MEDIA = 5, // NOTE WINDOW

            MESHDIALOG = -1,
            TEXTUREDIALOG = -2,
            NEWWORLD = -3,
			INTERFACE = -4,
            TREE = -5,
            SCRIPT = -6,
            GUBBIN = -7,
			
		} ;
		#endregion

		#region PlaceObjectType enum
		public enum PlaceObjectType
		{
			NONE = -1,
			ZONE = 0,
			SCENERY,
			TERRAIN,
			EMITTER,
			WATER,
			COLLISION,
			SOUND,
			DYNAMIC,
			TRIGGER,
			WAYPOINT,
			PORTAL,
			DUPLICATE
		} ;
		#endregion

		#region WorldButtonSelection enum
		public enum WorldButtonSelection
		{
			CREATE = 0,
			MOVE,
			ROTATE,
			SCALE,
			OBJECT_SETUP,
			ZONE_SETUP,
			SUNS_AND_MOONS,
			YEAR,
			EMITTERS
		} ;
		#endregion

		private readonly string[] UpdatesList = {
													@"Data\Areas", @"Data\Emitter Configs", @"Data\Game Data",
													@"Data\UI", @"Data\Meshes", @"Data\Music", @"Data\Sounds",
													@"Data\Textures", @"Data\Skins", @"Data\Fonts", @"Data\Terrains",
													@"Data\Trees"
												};

		//bool ActorsCharacterSetAdjustmentFlag = false;

		// if true, reset character offset is posible => CharacterOffsetX = 0, CharacterOffsetY = 0 ...
		private bool EmittersChanged;
		public ushort ActorSettingsSelecterImageID = 65535; // using in Actors/Settings -> Select 
		public bool ActorsSaved = true;
		public bool EmittersSaved = true;
		private int FactionsDataGridViewSelectRows = -1;
		public bool InterfaceSaved = true;
		public bool PostProcessSaved = true;
		public bool ItemsSaved = true;

		public int lastNodeSelect; // from tree world zones
		public int lastNodeSelectLevel2; // from tree world zones
		private bool LoadToolBar = true;
		private bool MediaTreeSelectedItem = false;
		// Member variables
		public int nodeSelect; // from tree world zones
		public int nodeSelectetLevel2; // from tree world zones
		private bool PanelMouseHover = false;
		public int RenderingPanelCurrentIndex = 0;
		public int RenderingPanelPreviousIndex = 0;
		private bool ResetElementsCharacterSet = true;
		private FullScreen _FullScreen;
		// Saving settings
		private bool ShowUnsavedIndicator = true;
		public bool WorldPanelLoop;

		public bool WorldSaved = true;

		#region DockableWindows
		public WorldCreateWindow m_CreateWindow = new WorldCreateWindow();
		public WorldPropertyWindow m_propertyWindow = new WorldPropertyWindow();
		public WorldTerrainEditor m_TerrainEditor = new WorldTerrainEditor();
		public SimpleScriptEditor m_ScriptView = new SimpleScriptEditor();
		public GubbinEditor m_GubbinEditor = new GubbinEditor();
		public WorldZoneList m_ZoneList = new WorldZoneList();
		public WorldViewRenderer m_ZoneRender = new WorldViewRenderer();
		public PostProcessWindows m_PostProcess = new PostProcessWindows();
		public InterfaceEditor m_Interface = new InterfaceEditor();
		public InterfaceHierarchyWindows m_InterfaceHierarchy = new InterfaceHierarchyWindows();
		private DeserializeDockContent m_deserializeDockContent;
		private string configFile, configFilePP;

		public TreeEditorRender TreeRender = new TreeEditorRender();
		public TreeEditorProperties TreeProperties = new TreeEditorProperties();
		#endregion

		#region Static forms
		public WorldYearSetup YearSetup = new WorldYearSetup();
		public WorldZoneSetup ZoneSetupForm = new WorldZoneSetup();
		#endregion

		#region 3D entities etc.
		private ActorInstance ActorCharacterSetPreview;
		private ActorInstance ActorPreview;
		private ActorInstance ActorPreviewAnimation;
		public Entity Camera;
		public Entity CharacterMesh;
		private float CharacterSetCamDistance = 5f;
		private float CharacterSetCamPitch = 0f, CharacterSetCamYaw = 0f;
		public Light DefaultLight;
		public uint DefaultParticleTexture;
		private byte EmitterBGB = 0;
		private byte EmitterBGG = 0;
		private byte EmitterBGR = 0;
		private float EmitterCamDistance = 20f;
		private float EmitterCamPitch = 0f, EmitterCamYaw = 0f;
		private Line3D[,] Grid = new Line3D[GridDetail + 1,2];
		private float GubbinToolCamDistance = 40f;
		private float GubbinToolCamPitch = 0f, GubbinToolCamYaw = 0f;
		private ActorInstance GubbinToolPreview;
		private Channel MediaAudioPreview; //, CharacterSetAudio;
		public Entity MediaMeshEN, MediaTextureEN;
		public EmitterConfig ParticlePreviewConfig;
		public Entity ParticlePreviewEN;
		public int PlaceObject;
		public Entity PortalTemplate;
		public int SelectedMediaID = 65535;
		public string SelectedMediaType;
		public Entity WaypointLinkTemplate;
		public Entity WaypointTemplate;
		#endregion

		#region Zone editor mouselook
		public float CameraSpeed = 0.2f;
		public bool Mouselooking = false;
		public int OldMouseX = 0, OldMouseY = 0;
		public float WorldCamDPitch = 0f, WorldCamDYaw = 0f;
		public float WorldCamPitch = 0f;
		public float WorldCamX = 0f, WorldCamY = 20f;
		public float WorldCamYaw = 0f;
		public float WorldCamZ = 0f;
		#endregion

		#region Zones stuff
		private const int GridDetail = 64 * 4; // Experimental values
		private const int GridSize = 2048 * 4; // Experimental values
		public uint BlueTex;
		public Zone CurrentClientZone;
		public RealmCrafter.ServerZone.Zone CurrentServerZone;
		public float GridHeight = 0f;
		private Waypoint LinkingWaypoint = null;
		public bool LoadingAZone = false;
		public bool MouseDragging = false;
		public uint OrangeTex;
		public ushort PlaceSceneryID = 65535, PlaceWaterID = 65535;
		public List<ZoneObject> Portals = new List<ZoneObject>();
		public uint RedTex;
		private int ShaderQuality = 0;
		private bool ShowGrid;
		internal bool SuppressZoneTransforms = false, SuppressZoneUndo = false;
		private bool SupressChange = false;

		public List<ZoneObject> Triggers = new List<ZoneObject>();
// 
//         public Entity[] WaypointEN = new Entity[2000],
//                         WaypointLinkAEN = new Entity[2000],
//                         WaypointLinkBEN = new Entity[2000];

		private int WaypointLinkMode = 0;
		public List<ZoneObject> Waypoints = new List<ZoneObject>();
		public uint YellowTex;
		public ArrayList ZoneSelected = new ArrayList();
		#endregion

		#region UI settings and selections
		public static string[] ScriptsList;
		private long AdjustPressedTime;
		private bool AllowResize = true;

		private Control CharacterSetAdjustPressedButton,
						CharacterSetPreviewPressedButton;

		private bool CtrlZWasDown = false;
		public bool DefalutZoneNoSave = false;
		private Control EmitterCamPressedButton;
		public string GameName;
		private Control GubbinToolAdjustPressedButton;
		private Control GubbinToolCamPressedButton;

		private float GubbinToolOffsetX,
					  GubbinToolOffsetY,
					  GubbinToolOffsetZ,
					  GubbinToolRotationX,
					  GubbinToolRotationY,
					  GubbinToolRotationZ,
					  GubbinToolScale;

		private string GubbinToolSelectedBone;
		private Entity GubbinToolSelectedMesh;
		private bool IgnoreAnimationGenderCheckChange = false; //Ignore Animation gender check change.
		private bool ItemAppearanceOpen = true;
		private bool ItemAttributesOpen = true;
		private bool ItemGeneralOpen = true;
		private bool ItemSpecificOpen = true;
		private bool LoadingActorCharacterSetPreview = false;
		private bool LoadingActorPreview = false;
		private bool LoadingActorPreviewAnimation = false;
		private bool LoadingGubbinToolPreview = false;
		private bool RememberLayout = false;
		public bool SaveHotkeyDown = false;

		/*
				// Character offset
				private float CharacterOffsetX, CharacterOffsetY, CharacterOffsetZ,
								CharacterRotationX, CharacterRotationY, CharacterRotationZ,
								CharacterScale = 1;
		*/
		private Actor SelectedActor, SelectedActorAnimation, SelectedActorCharacterSet;
		private Component SelectedIC;
		private Item SelectedItem;
		public int SetWorldButtonSelection = (int) WorldButtonSelection.CREATE;
		private bool SuppressAbilityChange = false;

		private bool SuppressActorChange = false;

		public bool SuppressActorSettingChange = false;

		private bool SuppressAnimSetAnimChange = false;

		private bool SuppressAnimSetChange = false;

		private bool SuppressCharacterSet = false,
					 SuppressEmitterChange = false;

		private bool SuppressItemChange = false;
		public int TotalActors;
		public int TotalItems;
		public int TotalZones;
		#endregion

		#region Delta timing
        public const float BaseFPS = 30f;
        public const int DeltaFrames = 10;
        public static float Delta;
		public static int[] DeltaBuffer;
        public static int DeltaBufferIndex;
        public static int DeltaTime, MilliSecs;
        public float SpeedFrames = 0F; // coef for DeltaFrames
		#endregion

        #region Files
        public const string ItemsFile = @"Data\Server Data\Items.s3db";
        #endregion

        // Initialisation code
		public GE()
		{
			nodeSelect = -1;
			lastNodeSelect = -1;
			nodeSelectetLevel2 = -1;
			lastNodeSelectLevel2 = -1;
			m_TerrainEditor.RefreshEditorButton(0);
			InitializeComponent();
		}

		#region Form events

		// Form initialisation event
		private void GE_Load(object sender, EventArgs e)
		{
			// The "Character Set" feature isn't implemented in RCClient, so Mark said to 
			// hide the tab in GE for now. The only way to hide the tab is to remove the 
			// TabPage from the TabControl.
			this.ActorsTabControl.TabPages.RemoveAt((int)ActorsTabControlSelectedIndex.CHARACTER_SET);
			this.ActorsTabControl.TabPages.RemoveAt((int)ActorsTabControlSelectedIndex.MESH_ADJUSTMENT);

			#region Load GE settings from file
			// Default settings
			ShowUnsavedIndicator = true;
			AllowResize = true;
			CameraSpeed = 0.2f;
			int SavedX = 0, SavedY = 0, SavedWidth = 1024, SavedHeight = 768;
			bool Maximised = false;
			RememberLayout = true;
			ShowGrid = true;
			ShaderQuality = 0;

			// Read settings from file
			BinaryReader F = Blitz.ReadFile(@"Data\GUE.dat");
			if (F != null)
			{
				ShowUnsavedIndicator = F.ReadBoolean();
				AllowResize = F.ReadBoolean();
				CameraSpeed = F.ReadSingle();
				RememberLayout = F.ReadBoolean();
				SavedX = F.ReadInt32();
				SavedY = F.ReadInt32();
				SavedWidth = F.ReadInt32();
				SavedHeight = F.ReadInt32();
				Maximised = F.ReadBoolean();
				if (F.BaseStream.Length > 24)
				{
					ShowGrid = F.ReadBoolean();
				}
				if (F.BaseStream.Length > 28) // StreamPos = 25, Post Int32 = 29
				{
					ShaderQuality = F.ReadInt32();
				}
				F.Close();
			}

			// Set fixed/variable form size
			if (AllowResize)
			{
				MaximizeBox = true;
				FormBorderStyle = FormBorderStyle.Sizable;
			}
			else
			{
				MaximizeBox = false;
				Width = 1024;
				Height = 768;
				FormBorderStyle = FormBorderStyle.FixedSingle;
			}
			// Set saved form layout
			if (RememberLayout)
			{
				Location = new Point(SavedX, SavedY);
				Width = SavedWidth;
				Height = SavedHeight;
				if (Maximised)
				{
					WindowState = FormWindowState.Maximized;
				}
				if (LoadToolBar)
				{
					try
					{
						ToolStripManager.LoadSettings(this);
					}
					catch (Exception exc)
					{
					}
					finally
					{
						WorldViewPanelToolbar.Visible = true;
						WorldViewObjectsToolbar.Visible = true;
						WorldWaypointToolbar.Visible = true;
						WorldZonesToolbar.Visible = true;
						TerrainToolBar.Visible = true;
						WorldRenderToggle.Visible = true;
					}
				}
				RenderWrapper.SetQualityLevel(ShaderQuality);
			}
			#endregion

			// Initial delta time buffer
			DeltaBuffer = new int[DeltaFrames];
			for (int i = 0; i < DeltaFrames; ++i)
			{
				DeltaBuffer[i] = 35;
			}

			// Create texture preview quad
			MediaTextureEN = Entity.CreateSAQuad();
			MediaTextureEN.SAQuadLayout(0.125f, 0f, 0.75f, 1f);
			MediaTextureEN.Visible = false;

			// Interface components
//             InterfaceControls.LoadFromXML(@"Data\Interface.xml");
//             InterfaceControls.SetVisibilityControls( false );
//             UpdateInterfaceGameComponentsList();
//             UpdateInterfaceLayout();

//             listBoxResolution.SetSelected(26, true); //1024x768
//             string[] parse_resolution = ((string) listBoxResolution.SelectedItem).Split('x');
			//InterfaceResWidthSpinner.Maximum = InterfaceResXPosSpinner.Maximum = (decimal)UInt16.Parse(parse_resolution[0]);
			//InterfaceResHeightSpinner.Maximum = InterfaceResYPosSpinner.Maximum = (decimal)UInt16.Parse(parse_resolution[1]);

			// Grid lines
			for (int i = 0; i <= GridDetail; ++i)
			{
				int Pos = (i - (GridDetail / 2)) * (GridSize / GridDetail);
				Grid[i, 0] = new Line3D(Pos, 0, GridSize / -2, Pos, 0, GridSize / 2, true);
				Grid[i, 1] = new Line3D(GridSize / -2, 0, Pos, GridSize / 2, 0, Pos, true);

				//   Grid[i, 0].SetColor(255, 255, 255);
				//   Grid[i, 1].SetColor(255, 255, 255);

				Grid[i, 0].SetColor(0, 0, 255);
				Grid[i, 1].SetColor(0, 0, 255);
				//  Alternate black grid
				//  Grid[i, 0].SetColor(0, 0, 0);
				//  Grid[i, 1].SetColor(0, 0, 0);
			}

			// Set initial state for rendering panel
            //ControlHost.TabIndex = 1;
            ControlHost.TabIndex = 0;
            UpdateRenderingPanel((int)GETab.WORLD);

            MediaWindow = new MediaManager(this, RenderingPanel);

			// Year display
			/*WorldYearLength.Value = RealmCrafter.Environment.MonthStartDay[0];
			WorldYearTimeCompression.Value = RealmCrafter.Environment.TimeFactor;
			WorldYearCurrentYear.Value = RealmCrafter.Environment.Year;
			WorldYearCurrentDay.Value = RealmCrafter.Environment.Day + 1;
			WorldYearMonthCombo.SelectedIndex = 0;
			WorldYearSeasonCombo.SelectedIndex = 0;
			*/
			// Suns/moons display

			// Set up UI
			UpdateRaceClassLists();
			ActorMSoundCombo.SelectedIndex = 0;
			ActorFSoundCombo.SelectedIndex = 0;
			ActorHairColourNCombo.SelectedIndex = 0;
			ActorSlotDisabledCombo.SelectedIndex = 0;
			SetSelectedActor(0);
			//  DM_OUT
			//			if (GubbinToolActorCombo.Items.Count > 0)
			//				GubbinToolActorCombo.SelectedIndex = 0;
//             ItemGubbinCheckbox1.Text = Actors3D.GubbinJoints[0];
//             ItemGubbinCheckbox2.Text = Actors3D.GubbinJoints[1];
//             ItemGubbinCheckbox3.Text = Actors3D.GubbinJoints[2];
//             ItemGubbinCheckbox4.Text = Actors3D.GubbinJoints[3];
//             ItemGubbinCheckbox5.Text = Actors3D.GubbinJoints[4];
//             ItemGubbinCheckbox6.Text = Actors3D.GubbinJoints[5];
			SetSelectedItem(0);
			ItemWeaponDamageCheck.Checked = Item.WeaponDamageEnabled;
			ItemArmourDamageCheck.Checked = Item.ArmourDamageEnabled;
			//WorldPlaceObjectTypeCombo.SelectedIndex = 0;
			//WorldPlaceSceneryButton.Text = NiceMeshName(PlaceSceneryID);
			//WorldPlaceWaterButton.Text = NiceTextureName(PlaceWaterID);
			//WorldZoneFogNearSlider.Maximum = (int)RealmCrafter.ClientZone.Zone.MaxFogFar - 100;
			//WorldZoneFogFarSlider.Maximum = (int)RealmCrafter.ClientZone.Zone.MaxFogFar;
			//WorldPlacePortalLinkCombo.SelectedIndex = 0;

			SetItemsSavedStatus(true);

			// Disable saving buttons in demo
			if (Program.DemoVersion)
			{
				//BMainSaveActors.Enabled = false;
				//BMainSaveItems.Enabled = false;
				//BMainSaveWorld.Enabled = false;
				//BMainSaveInterface.Enabled = false;
				BMainSaveAll.Enabled = false;
				//BMainSaveQuit.Enabled = false;
				EmitterSaveButton.Enabled = false;
				//BProjectBuildFull.Enabled = false;
				//BProjectBuildMinimum.Enabled = false;
				//BProjectBuildServer.Enabled = false;
				//BProjectBuildUpdate.Enabled = false;
			}
			string[] Zones = Directory.GetFiles(@"Data\Areas\", "*.dat");
			////string[] ZoneList = new string[Zones.GetUpperBound(0) + 2];
			////ZoneList[0] = "";
			WorldZones.BeginUpdate();
			foreach (string S in Zones)
			{
				WorldZones.Items.Add(Path.GetFileNameWithoutExtension(S));
			}

			WorldZones.EndUpdate();

			configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "RealmCrafter.WorldDock.config");
			if (File.Exists(configFile))
			{
				m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
				WorldDock.LoadFromXml(configFile, m_deserializeDockContent);
			}
			else
			{
				m_CreateWindow.Show(WorldDock);
				m_propertyWindow.Show(m_CreateWindow.Pane, DockAlignment.Bottom, 0.6);
				m_TerrainEditor.Show(m_CreateWindow.Pane, DockAlignment.Bottom, 0.6);
				m_ZoneList.Show(WorldDock);
				m_ZoneRender.Show(WorldDock);

				// interface dock
				m_Interface.Show(WorldDock);
				m_InterfaceHierarchy.Show(m_CreateWindow.Pane, DockAlignment.Left, 0.5);

				// these three lines are needed to stop a thread deadlock
				m_ScriptView.Show(WorldDock);
				m_ScriptView.Visible = false;
				m_ScriptView.Hide();

				TreeRender.Show(WorldDock, DockState.Document);
				TreeRender.Visible = false;
				TreeRender.Hide();

				m_GubbinEditor.Show(WorldDock, DockState.Document);
				m_GubbinEditor.Visible = false;
				m_GubbinEditor.Hide();

//                 m_Interface.Show(WorldDock, DockState.Document);
//                 m_Interface.Visible = false;
//                 m_Interface.Hide();
			}

			configFilePP = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "RealmCrafter.PPDock.config");
			if (File.Exists(configFilePP))
			{
				m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
				dockPostProcess.LoadFromXml(configFilePP, m_deserializeDockContent);
			}
			else
			{
				// PostProcess Dock
				m_PostProcess.Show(dockPostProcess);

			}

			_FullScreen = new FullScreen(this);
			Activate();
			MainToolbar.Visible = true;
		}

		private IDockContent GetContentFromPersistString(string persistString)
		{
			if (persistString == typeof(PostProcessWindows).ToString())
			{
				return m_PostProcess;
			}
			else if (persistString == typeof(WorldCreateWindow).ToString())
			{
				return m_CreateWindow;
			}
			else if (persistString == typeof(WorldPropertyWindow).ToString())
			{
				return m_propertyWindow;
			}
			else if (persistString == typeof(WorldViewRenderer).ToString())
			{
				return m_ZoneRender;
			}
			else if (persistString == typeof(WorldZoneList).ToString())
			{
				return m_ZoneList;
			}
			else if (persistString == typeof(SimpleScriptEditor).ToString())
			{
				return m_ScriptView;
			}
			else if (persistString == typeof(WorldTerrainEditor).ToString())
			{
				return m_TerrainEditor;
			}
			else if (persistString == typeof(GubbinEditor).ToString())
			{
				return m_GubbinEditor;
			}
			else
			{
				return null;
			}
		}

		// Form closed event
		private void GE_FormClosed(object sender, FormClosedEventArgs e)
		{
			ToolStripManager.SaveSettings(this);
			bool isMaximised = WindowState == FormWindowState.Maximized;
			WindowState = FormWindowState.Normal;

			// Save GE settings to file
			FileStream FS = new FileStream(@"Data\GUE.dat", FileMode.Create,
										   FileAccess.Write);
			BinaryWriter F = new BinaryWriter(FS);

			F.Write(ShowUnsavedIndicator);
			F.Write(AllowResize);
			F.Write(CameraSpeed);
			F.Write(RememberLayout);
			F.Write(Location.X);
			F.Write(Location.Y);
			F.Write(Width);
			F.Write(Height);
			F.Write(isMaximised);
			F.Write(ShowGrid);
			F.Write(ShaderQuality);

			F.Close();

			if (RememberLayout)
			{
				WorldDock.SaveAsXml(configFile);
				dockPostProcess.SaveAsXml(configFilePP);
			}
			else
			{
				if (File.Exists(configFile)) File.Delete(configFile);
				if (File.Exists(configFilePP)) File.Delete(configFilePP);
			}

			if (TerrainManager != null)
				m_TerrainEditor.SaveToolOptions((Program.TestingVersion ? @".\GUE\RCTConfig.xml" : @"..\..\Data\GUE\RCTConfig.xml"));
		}

		private void GE_Resize(object sender, EventArgs e)
		{
            if(m_ScriptView != null)
                m_ScriptView.FixDockSizing();
		}

		// Main tab unselected
		private void ControlHost_Deselected(object sender, TabControlEventArgs e)
		{
			EmitterCamPressedButton = null;
			if (e.TabPageIndex == 2)
			{
				UpdateRaceClassLists();
				//UpdateRenderingPanel(2);
				ClearActorPreviewAnimation();
				// Clear Character Set

				
			}
			else if (e.TabPageIndex == 4)
			{
				/*     if (this.Controls.Contains(WorldPanel))
				{
					WorldPanel.Hide();
				}
				if (this.Controls.Contains(WorldZonesTreePanel))
				{
					WorldZonesTreePanel.Hide();
				}
			*/
			}
			MouseDragging = false;
		}

		// Main tab selected
		private void ControlHost_Selected(object sender, TabControlEventArgs e)
		{
			MLib.Timer.MaxFPS = 100;
			if (RenderingPanelCurrentIndex == (int)GETab.NEWWORLD)
			{
				WorldCamPitch = Camera.Pitch();
				WorldCamX = Camera.X();
				WorldCamY = Camera.Y();
				WorldCamZ = Camera.Z();
				WorldCamYaw = Camera.Yaw();
			}
			Camera.Rotate(0, 0, 0);
			Camera.Position(0, 0, 0);
			if (e.TabPageIndex == (int)GETab.WORLD)
			{
				//LeftDynamicControlsPanel.Hide();
				//World3DView.Hide();

				/* 
				m_CreateWindow.Show(WorldDock);
				m_propertyWindow.Show(WorldDock);
				m_ZoneList.Show(WorldDock);
				m_ZoneRender.Show(WorldDock);
				*/

				/*if (this.Controls.Contains(WorldPanel))
				{
					WorldPanel.Show();
				}
				if (this.Controls.Contains(WorldZonesTreePanel))
				{
					WorldZonesTreePanel.Show();
				 * 
				}*
				 */

				if (ZoneSelected.Count > 0)
				{
					ZoneObject Obj = (ZoneObject) ZoneSelected[0];
					//Camera.Position(Obj.EN.X(), Obj.EN.Y() + 10, Obj.EN.Z());
					//Camera.Rotate(90, 0, 0);
					WorldCamPitch = 90;
					WorldCamYaw = 0;
				}
				if (WorldPanelLoop == false)
				{
					RenderingPanel.MouseClick -=
						new MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
					RenderingPanel.MouseClick +=
						new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
					Application.Idle -= new EventHandler(Program.GE.MainLoop);
					Application.Idle += new EventHandler(m_ZoneRender.WorldRender_MainLoop);
					WorldPanelLoop = true;
				}
				m_ZoneRender.Hide();
				m_ZoneRender.Show();
				m_ZoneRender.Activate();
			}
            else if (e.TabPageIndex == (int)GETab.EMITTER)
			{
				EmitterEditorGroup.Parent = TabEmitters;
				EmitterEditorGroup.Show();
				EmitterEditorGroup.Dock = DockStyle.Fill;
			}

			if (e.TabPageIndex == (int)GETab.WORLD)
			{
                UpdateRenderingPanel((int)GETab.NEWWORLD); //new world panel
			}
			else if (e.TabPageIndex == (int)GETab.POSTPROCESS)
			{
				UpdateRenderingPanel((int)GETab.POSTPROCESS); 
				if (WorldPanelLoop == true)
				{
					RenderingPanel.MouseClick -=
						new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
					RenderingPanel.MouseClick += new MouseEventHandler(RenderingPanel_MouseClick);
					Application.Idle -= new EventHandler(m_ZoneRender.WorldRender_MainLoop);
					Application.Idle += new EventHandler(MainLoop);
					WorldPanelLoop = false;
				}
			}
			else
			{
				UpdateRenderingPanel(e.TabPageIndex);

				if (WorldPanelLoop == true)
				{
					RenderingPanel.MouseClick -=
						new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
					RenderingPanel.MouseClick += new MouseEventHandler(RenderingPanel_MouseClick);
					Application.Idle -= new EventHandler(m_ZoneRender.WorldRender_MainLoop);
					Application.Idle += new EventHandler(MainLoop);
					WorldPanelLoop = false;
				}
			}
		}

		#endregion

		#region Toolbar events
		private void GE_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!AllSaved())
			{
				if (MessageBox.Show("Really quit without saving?", "Realm Crafter", MessageBoxButtons.OKCancel) ==
					DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		private void BMainQuitNoSave_Click(object sender, EventArgs e)
		{
			if (!AllSaved())
			{
				if (MessageBox.Show("Really quit without saving?", "Realm Crafter", MessageBoxButtons.OKCancel) ==
					DialogResult.OK)
				{
					Close();
				}
			}
			else
			{
				Close();
			}
		}

		private void BMainSaveAll_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion)
			{
				SaveAll();
				if (Program.ActiveTree != null && TreeRender.Saved == false)
				{
					Program.GETreeManager.Save();
				}
			}
		}

		private void BMainSaveQuit_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion)
			{
				SaveAll();
			}
			Close();
		}

		private void BMainSaveActors_Click(object sender, EventArgs e)
		{
			// Actors
			if (!Program.DemoVersion && !ActorsSaved)
			{
				ActorSaveGeneralSettings();
				AnimSet.Save(@"Data\Game Data\Animations.dat");
				Ability.Save(Ability.SpellDatabase);
				Actor.SaveFactions(Actor.FactionDatabase);
                Actor.Save(Actor.ActorDatabase);
				SetActorsSavedStatus(true);
			}
		}

		private void BMainSaveItems_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion && !ItemsSaved)
			{
				Item.Save(ItemsFile);

				Actors3D.SaveGubbinNames();

				FileStream FStream = new FileStream(@"Data\Server Data\Misc.dat",
													FileMode.Open,
													FileAccess.Write);
				BinaryWriter F = new BinaryWriter(FStream);
				if (F == null)
				{
					MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
					return;
				}
				F.BaseStream.Seek(12, SeekOrigin.Begin);
				F.Write(Item.WeaponDamageEnabled);
				F.Write(Item.ArmourDamageEnabled);
				F.Close();

				SetItemsSavedStatus(true);
			}
		}

		private void BMainSaveWorld_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion && !WorldSaved)
			{
				SaveWorld();
			}

		}

		private void BMainSaveTrees_Click(object sender, EventArgs e)
		{
			if (Program.ActiveTree != null && TreeRender.Saved == false)
			{
				Program.GETreeManager.Save();
			}
		}

		private void BMainSaveInterface_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion /*&& !InterfaceSaved*/)
			{
				m_Interface.SaveInterface();
			}
		}

		private void BMainSavePostProcess_Click(object sender, EventArgs e)
		{
//             if (!Program.DemoVersion && !PostProcessSaved)
			{
				m_PostProcess.SaveUserPP_Effects( @"Data\Game Data\PostProcess\PostProcess.xml");
				PostProcessSaved = true;
			}
		}

		private void BEditorOptions_Click(object sender, EventArgs e)
		{
			// Show options dialog
			Editor_Options ES = new Editor_Options();
			ES.GEOptionsSaveIndicate.Checked = ShowUnsavedIndicator;
			ES.GEOptionsResizing.Checked = AllowResize;
			ES.GEOptionsRememberLayout.Checked = RememberLayout;
			ES.GEOptionsCameraSpeed.Value = (decimal) (CameraSpeed * 100f);
			ES.GEOptionsShowGrid.Checked = ShowGrid;
			ES.ShaderDetailCombo.SelectedIndex = ShaderQuality;
			ES.ShowDialog();

			// Set new options
			ShowUnsavedIndicator = ES.GEOptionsSaveIndicate.Checked;
			SetItemsSavedStatus(ItemsSaved);
			AllowResize = ES.GEOptionsResizing.Checked;
			RememberLayout = ES.GEOptionsRememberLayout.Checked;
			CameraSpeed = (float) (ES.GEOptionsCameraSpeed.Value) / 100f;
			if (ControlHost.SelectedIndex == (int) GETab.WORLD &&
				SetWorldButtonSelection < (int) WorldButtonSelection.EMITTERS)
			{
				ShowGrid = ES.GEOptionsShowGrid.Checked;
			}

			if (AllowResize)
			{
				FormBorderStyle = FormBorderStyle.Sizable;
				MaximizeBox = true;
			}
			else
			{
				MaximizeBox = false;
				Width = 1024;
				Height = 768;
				FormBorderStyle = FormBorderStyle.FixedSingle;
			}

			//Only add grid if selected tab is world view
			//if (ControlHost.TabIndex == 10)
			if(RenderingPanelCurrentIndex == -3)
			{
				Line3D.SetAllVisible(ShowGrid, true);
			}

			ShaderQuality = ES.ShaderDetailCombo.SelectedIndex;
			RenderWrapper.SetQualityLevel(ShaderQuality);
			ES.Dispose();
		}

		private void BUndo_Click(object sender, EventArgs e)
		{
			if (ControlHost.SelectedIndex == (int) GETab.WORLD &&
				SetWorldButtonSelection < (int) WorldButtonSelection.EMITTERS)
			{
				Undo.Perform(this);
			}
		}
		#endregion

		// Save all changes
		private bool AllSaved()
		{
			return ItemsSaved && ActorsSaved && WorldSaved && EmittersSaved && PostProcessSaved;
		}

		public void SaveAll()
		{
			// Actors
			if (!ActorsSaved)
			{
				ActorSaveGeneralSettings();
				AnimSet.Save(@"Data\Game Data\Animations.dat");
				Ability.Save(Ability.SpellDatabase);
				Actor.SaveFactions(Actor.FactionDatabase);
				Actor.Save(Actor.ActorDatabase);
				SetActorsSavedStatus(true);
			}

			// Items
			if (!ItemsSaved)
			{
				Item.Save(ItemsFile);

				Actors3D.SaveGubbinNames();

				FileStream FStream = new FileStream(@"Data\Server Data\Misc.dat",
													FileMode.Open,
													FileAccess.Write);
				BinaryWriter F = new BinaryWriter(FStream);
				if (F == null)
				{
					MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
					return;
				}
				F.BaseStream.Seek(12, SeekOrigin.Begin);
				F.Write(Item.WeaponDamageEnabled);
				F.Write(Item.ArmourDamageEnabled);
				F.Close();

				SetItemsSavedStatus(true);
			}

			// Current emitter
			if (!EmittersSaved)
			{
				if (ParticlePreviewConfig != null)
				{
					ParticlePreviewConfig.Save(@"Data\Emitter Configs\" + ParticlePreviewConfig.Name + ".rpc");
					SetEmittersSavedStatus(true);
				}
			}

			// Postprocess
//             if (!PostProcessSaved)
			{
				m_PostProcess.SaveUserPP_Effects(@"Data\Game Data\PostProcess\PostProcess.xml");
				PostProcessSaved = true;
			}

			// World
			if (!WorldSaved)
			{
				SaveWorld();
			}

			if (m_GubbinEditor.Saved == false)
			{
				m_GubbinEditor.Save();
			}

			// Interface
			m_Interface.SaveInterface();
		}

		// Run the damage type selection/editing dialog
		public int GetDamageType(int Current)
		{
			// Create window
			int Result = 0;
			Damage_Types Win = new Damage_Types();

			// Set initial damage type names
			for (int i = 0; i < 20; ++i)
			{
				Win.DamageTypeText[i].Text = Item.DamageTypes[i];
				if (i == Current)
				{
					Win.DamageTypeRadio[i].Checked = true;
				}
			}

			// Run window
			Win.ShowDialog();

			// Update damage type names and selected
			for (int i = 0; i < 20; ++i)
			{
				Item.DamageTypes[i] = Win.DamageTypeText[i].Text;
				if (Win.DamageTypeRadio[i].Checked)
				{
					if (string.IsNullOrEmpty(Item.DamageTypes[i]))
					{
						Result = Current;
					}
					else
					{
						Result = i;
					}
				}
			}

			// JB: Save changes
			Item.SaveDamageTypes(@"Data\Server Data\Damage.dat");

			// Update resistances list in actors tab
			ActorResistancesList.Items.Clear();
			ActorResistancesList.BeginUpdate();
			for (int i = 0; i < Item.TotalDamageTypes; ++i)
			{
				if (Item.DamageTypes[i] != "")
				{
                    ActorResistancesList.Items.Add(new ListBoxItem(Item.DamageTypes[i], (uint)i));
				}
			}
			ActorResistancesList.EndUpdate();

			// Done
			Win.Dispose();
			return Result;
		}

		// Run the projectile selection/editing dialog
		private int GetProjectile(int Current)
		{
			// Create window
			int Result = 0;
			ProjectilesEditor Win = new ProjectilesEditor();
			Win.GEInstance = this;

			// Run window
			Win.ShowDialog();

			// Get result
			Result = Win.Selected;

			// Done
			Win.Dispose();
			return Result;
		}

		// Run the attribute editing dialog
		private void EditAttributes()
		{
			// Run window
			AttributesEditor Win = new AttributesEditor();
			Win.ShowDialog();

			// If accepted, update attribute lists
			if (Win.Saved)
			{
				ItemAttributesList.Items.Clear();
				ActorAttributesList.Items.Clear();
				for (int i = 0; i < Attributes.TotalAttributes; ++i)
				{
					if (Attributes.Names[i].Length > 0)
					{
                        ListBoxItem LBI = new ListBoxItem(Attributes.Names[i], (uint)i);
						ItemAttributesList.Items.Add(LBI);
						ActorAttributesList.Items.Add(LBI);
					}
				}

				//UpdateInterfaceGameComponentsList();
			}

			// Free window
			Win.Dispose();
		}

		// Updates lists of actor races and classes available
		public void UpdateRaceClassLists()
		{
			// Clear
			string ItemRace = "", ItemClass = "";
			if (ItemRaceCombo.SelectedIndex > 0)
			{
				ItemRace = ItemRaceCombo.SelectedItem.ToString();
			}
			if (ItemClassCombo.SelectedIndex > 0)
			{
				ItemClass = ItemClassCombo.SelectedItem.ToString();
			}
			string AbilityRace = "", AbilityClass = "";
			if (AbilityExclusiveRaceCombo.SelectedIndex > 0)
			{
				AbilityRace = AbilityExclusiveRaceCombo.SelectedItem.ToString();
			}
			if (AbilityExclusiveClassCombo.SelectedIndex > 0)
			{
				AbilityClass = AbilityExclusiveClassCombo.SelectedItem.ToString();
			}
			ItemRaceCombo.Items.Clear();
			ItemRaceCombo.Items.Add("(None - available to all races)");
			ItemClassCombo.Items.Clear();
			ItemClassCombo.Items.Add("(None - available to all classes)");
			AbilityExclusiveRaceCombo.Items.Clear();
			AbilityExclusiveRaceCombo.Items.Add("(None - available to all races)");
			AbilityExclusiveClassCombo.Items.Clear();
			AbilityExclusiveClassCombo.Items.Add("(None - available to all classes)");

			// Refill
			Actor A = Actor.FirstActor;
			while (A != null)
			{
				// Add race to lists if required
				if (ItemRaceCombo.FindStringExact(A.Race) < 0)
				{
					ItemRaceCombo.Items.Add(A.Race);
					AbilityExclusiveRaceCombo.Items.Add(A.Race);
				}

				// Add class to lists if required
				if (ItemClassCombo.FindStringExact(A.Class) < 0)
				{
					ItemClassCombo.Items.Add(A.Class);
					AbilityExclusiveClassCombo.Items.Add(A.Class);
				}

				A = A.NextActor;
			}

			// Reset selections
			if (ItemRace != "")
			{
				ItemRaceCombo.SelectedIndex = ItemRaceCombo.FindStringExact(ItemRace);
			}
			else
			{
				ItemRaceCombo.SelectedIndex = 0;
			}
			if (ItemClass != "")
			{
				ItemClassCombo.SelectedIndex = ItemClassCombo.FindStringExact(ItemClass);
			}
			else
			{
				ItemClassCombo.SelectedIndex = 0;
			}
			if (AbilityRace != "")
			{
				AbilityExclusiveRaceCombo.SelectedIndex = AbilityExclusiveRaceCombo.FindStringExact(AbilityRace);
			}
			else
			{
				AbilityExclusiveRaceCombo.SelectedIndex = 0;
			}
			if (AbilityClass != "")
			{
				AbilityExclusiveClassCombo.SelectedIndex = AbilityExclusiveClassCombo.FindStringExact(AbilityClass);
			}
			else
			{
				AbilityExclusiveClassCombo.SelectedIndex = 0;
			}
		}

		// Updates the rendering panel for a certain tab
		public void UpdateRenderingPanel(int Index)
		{

			// for interface tab
			if (RenderingPanelCurrentIndex == (int)GETab.INTERFACE)
			{
				m_InterfaceHierarchy.Enabled = false;
				m_InterfaceHierarchy.UnCheckAllNodes();
				m_Interface.HideInterface();
			}
			if ( Index == (int)GETab.INTERFACE )
				m_InterfaceHierarchy.RestoreStateAllNodes();

            if (RenderingPanelCurrentIndex == (int)GETab.TREE)
			{
				if (TreeRender != null)
					TreeRender.HideRender();
				if (TreeProperties != null)
					TreeProperties.RealHide();
				if (Program.ActiveTree != null)
					Program.ActiveTree.Visible = false;
				Application.Idle -= new EventHandler(TreeRender.Application_Idle);
				RenderingPanel.MouseMove -= new MouseEventHandler(TreeRender.RenderPanel_MouseMove);
				RenderingPanel.MouseDown -= new MouseEventHandler(TreeRender.RenderPanel_MouseDown);
				RenderingPanel.MouseUp -= new MouseEventHandler(TreeRender.RenderPanel_MouseUp);
				RenderingPanel.PreviewKeyDown -= new PreviewKeyDownEventHandler(TreeRender.TreeEditorRender_PreviewKeyDown);
				RenderingPanel.MouseClick +=
								new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
				Application.Idle += new EventHandler(m_ZoneRender.WorldRender_MainLoop);
			}

			if (RenderingPanelCurrentIndex == (int)GETab.SCRIPT)
			{
				m_ScriptView.UpdateRenderingPanel(false);
				Application.Idle -= new EventHandler(m_ScriptView.Application_Idle);
				Application.Idle += new EventHandler(m_ZoneRender.WorldRender_MainLoop);
				RenderingPanel.MouseMove -= new MouseEventHandler(m_ScriptView.RenderPanel_MouseMove);
				RenderingPanel.MouseDown -= new MouseEventHandler(m_ScriptView.RenderPanel_MouseDown);
				RenderingPanel.MouseUp -= new MouseEventHandler(m_ScriptView.RenderPanel_MouseUp);
				RenderingPanel.MouseDoubleClick -= new MouseEventHandler(m_ScriptView.RenderPanel_MouseDoubleClick);
				RenderingPanel.PreviewKeyDown -= new PreviewKeyDownEventHandler(m_ScriptView.RenderPanel_PreviewKeyDown);
				RenderingPanel.MouseClick +=
								new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
			}

			if (RenderingPanelCurrentIndex == (int)GETab.GUBBIN)
			{
				if (m_GubbinEditor != null)
					m_GubbinEditor.HideRender(Index == (int)GETab.MESHDIALOG);
				Application.Idle -= new EventHandler(m_GubbinEditor.Application_Idle);
				RenderingPanel.MouseMove -= new MouseEventHandler(m_GubbinEditor.RenderPanel_MouseMove);
				RenderingPanel.MouseDown -= new MouseEventHandler(m_GubbinEditor.RenderPanel_MouseDown);
				RenderingPanel.MouseUp -= new MouseEventHandler(m_GubbinEditor.RenderPanel_MouseUp);
				RenderingPanel.PreviewKeyDown -= new PreviewKeyDownEventHandler(m_GubbinEditor.GubbinEditor_PreviewKeyDown);
				RenderingPanel.MouseClick +=
								new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
				Application.Idle += new EventHandler(m_ZoneRender.WorldRender_MainLoop);
			}

			// post processing tab
			if (RenderingPanelCurrentIndex == (int)GETab.POSTPROCESS) RenderWrapper.EnablePP_Pipeline(false);
			else
                if (Index == (int)GETab.POSTPROCESS) RenderWrapper.EnablePP_Pipeline(m_PostProcess.chbxEnable.Checked);

			RenderingPanelPreviousIndex = RenderingPanelCurrentIndex;
			//if (RenderingPanelPreviousIndex == -3)
			//{
			//    WorldCamPitch = Camera.Pitch();
			//    WorldCamX = Camera.X();
			//    WorldCamY = Camera.Y();
			//    WorldCamZ = Camera.Z();
			//    WorldCamYaw = Camera.Yaw();
			//}
			RenderingPanelCurrentIndex = Index;

			if (RenderingPanelCurrentIndex != RenderingPanelPreviousIndex)
			{
				if (Program.Transformer != null)
					Program.Transformer.Free();
				Program.Transformer = null;

				if (Program.GE.m_ZoneList.WorldZonesTree.Nodes.Count > 9)
				{
					foreach (TreeNode TN in Program.GE.m_ZoneList.WorldZonesTree.Nodes[10].Nodes)
					{
						if (TN.Tag is TreePlacerArea)
							(TN.Tag as TreePlacerArea).Hide();
					}
				}
			}

			// Hide all entities
			if (MediaMeshEN != null)
			{
				MediaMeshEN.Visible = false;
			}
			if (MediaTextureEN != null)
			{
				MediaTextureEN.Visible = false;
			}
			if (ActorPreview != null && ActorPreview.EN != null)
			{
				((Entity) ActorPreview.EN).Visible = false;
				Actors3D.HideGubbins(ActorPreview);
			}
			if (GubbinToolPreview != null && GubbinToolPreview.EN != null)
			{
				((Entity) GubbinToolPreview.EN).Visible = false;
				Actors3D.HideGubbins(GubbinToolPreview);
			}
			if (ActorPreviewAnimation != null)
			{
				Actors3D.HideGubbins(ActorPreviewAnimation);
			}
			if (GubbinToolSelectedMesh != null)
			{
				GubbinToolSelectedMesh.Visible = false;
			}
//             Component C = Component.FirstComponent;
//             while (C != null)
//             {
//                 if (C.displayed_name != null)
//                 {
//                     Entity EN = (Entity) C.Handle;
//                     EN.Visible = false;
//                 }
//                 C = C.NextComponent;
//             }
			if (ParticlePreviewEN != null)
			{
				General.ShowEmitter(ParticlePreviewEN, false);
			}
			if (CurrentClientZone != null)
			{
				CurrentClientZone.SetVisible(false);
				CurrentClientZone.SetListVisible(Triggers, false);
				CurrentClientZone.SetListVisible(Portals, false);
				CurrentClientZone.SetListVisible(Waypoints, false);

			}

			if (CurrentServerZone != null)
			{
				for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
				{
					Waypoint WP = CurrentServerZone.Waypoints[i];

					if (WP.WaypointLinkAEN != null)
						WP.WaypointLinkAEN.Visible = false;
					if (WP.WaypointLinkBEN != null)
						WP.WaypointLinkBEN.Visible = false;

				}
			}

			Line3D.SetAllVisible(false, false);

			if (Program.GE.CurrentServerZone != null)
			{
				foreach (Portal P in Program.GE.CurrentServerZone.Portals)
				{
					P.UpdateLabel();
				}

				foreach (Trigger T in Program.GE.CurrentServerZone.Triggers)
				{
					T.UpdateLabel();
				}

				foreach (Waypoint W in Program.GE.CurrentServerZone.Waypoints)
				{
					W.UpdateLabel();
				}
			}
			if (Program.GE.CurrentClientZone != null)
			{
				foreach (MenuControl C in Program.GE.CurrentClientZone.MenuControls)
					C.UpdateLabel();
			}


			// Default camera settings
			//Camera.Position(0f, 0f, 0f);
			//Camera.Rotate(0f, 0f, 0f);
			Camera.CameraRange(1f, 1000f);
			Camera.CameraClsColor(0, 120, 255);
			Render.FogColor(255, 255, 255);
			Render.FogRange(990f, 999f);
			Render.FogMode(0);
			Render.AmbientLight(100, 100, 100);
			DefaultLight.Direction(0f, -5f / 11.18f, 10f / 11.18f);
			//Environment3D.Hide();
			RenderWrapper.bbdx2_SetRenderMask(0);

			switch (Index)
			{
					// Media manager
				case (int)GETab.MEDIA:

					// Adjust panel
					RenderingPanel.Visible = true;
					RenderingPanel.Parent = MediaWindow.MediaPreview3DPanel;
                    RenderingPanel.Size = MediaWindow.MediaPreview3DPanel.Size;
					RenderingPanel.Dock = DockStyle.Fill;
					// Show entities
					if (MediaMeshEN != null && SelectedMediaType == "Me")
					{
						MediaMeshEN.Visible = true;
					}
					if (MediaTextureEN != null && SelectedMediaType == "Te")
					{
						MediaTextureEN.Visible = true;
					}

						RenderingPanel.MouseClick -=
							new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
						Application.Idle -= new EventHandler(m_ZoneRender.WorldRender_MainLoop);
                        Application.Idle -= new EventHandler(MainLoop);

                     

						Application.Idle += new EventHandler(MediaWindow.UpdateMedia);
             
					break;
					// Actor editor
				case (int)GETab.ACTORS:
					// Actor preview
					if (ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.ACTORS)
					{
						RenderingPanel.Visible = true;
						RenderingPanel.Parent = ActorPreviewPanel;
						RenderingPanel.Size = ActorPreviewPanel.Size;
						RenderingPanel.Dock = DockStyle.Fill;
						if (ActorPreview != null && ActorPreview.EN != null)
						{
							((Entity) ActorPreview.EN).Visible = true;
							Actors3D.ShowGubbins(ActorPreview);
						}
					}
						// Animation actor preview (Marian Voicu)
					else if (ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.ANIMATION)
					{
						RenderingPanel.Visible = true;
						RenderingPanel.Parent = ActorsAnimationPanel;
						RenderingPanel.Size = ActorsAnimationPanel.Size;
						RenderingPanel.Dock = DockStyle.Fill;

						if(ActorPreviewAnimation != null)
							Actors3D.ShowGubbins(ActorPreviewAnimation);

						// if (ActorPreviewAnimation != null && ActorPreviewAnimation.EN != null)
						//    ((Entity)ActorPreviewAnimation.EN).Visible = true;
					}
						// end (MV)
						// Character preview (Marian Voicu)

					else
					{
						RenderingPanel.Parent = this;
						RenderingPanel.Visible = false;
						RenderingPanel.Dock = DockStyle.None;
						RenderingPanel.Size = new Size(0, 0);
					}
					break;
					// World editor
				case (int)GETab.WORLD:
					// World view
					if (SetWorldButtonSelection < (int) WorldButtonSelection.EMITTERS)
					{
						Environment3D.Show();
						// Adjust panel
						RenderingPanel.Visible = true;
						//     RenderingPanel.Parent = World3DView;
						//     RenderingPanel.Size = World3DView.Size;
						RenderingPanel.Dock = DockStyle.Fill;
						// Show entities
						if (CurrentClientZone != null)
						{
							CurrentClientZone.SetVisible(true);
							CurrentClientZone.SetListVisible(Triggers, true);
							CurrentClientZone.SetListVisible(Portals, true);
							CurrentClientZone.SetListVisible(Waypoints, true);
						}
						if (CurrentServerZone != null)
						{
							for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
							{
								Waypoint WP = CurrentServerZone.Waypoints[i];

								if (WP.WaypointLinkAEN != null)
									WP.WaypointLinkAEN.Visible = true;
								if (WP.WaypointLinkBEN != null)
									WP.WaypointLinkBEN.Visible = true;

							}
						}
						Line3D.SetAllVisible(true, false);
						Line3D.SetAllVisible(ShowGrid, true);

						if (TreeRender != null)
							TreeRender.HideRender();
						if (m_GubbinEditor != null)
							m_GubbinEditor.HideRender(false);


						// Camera settings
						//Camera.Position(WorldCamX, WorldCamY, WorldCamZ);
						//Camera.Rotate(WorldCamPitch, WorldCamYaw, 0f);
						if (CurrentClientZone != null)
						{
							Render.FogMode(1);
							CurrentClientZone.SetViewDistance(Camera, CurrentClientZone.FogNear,
															  CurrentClientZone.FogFar);
							Camera.CameraClsColor(CurrentClientZone.FogR, CurrentClientZone.FogG, CurrentClientZone.FogB);
							Render.FogColor(CurrentClientZone.FogR, CurrentClientZone.FogG, CurrentClientZone.FogB);
							Render.AmbientLight(CurrentClientZone.AmbientR, CurrentClientZone.AmbientG,
												CurrentClientZone.AmbientB);
							//DefaultLight.Direction(???);
						}
						RenderingPanel.Focus();
					}
						// Emitter editor
					else if (SetWorldButtonSelection == (int) WorldButtonSelection.EMITTERS)
					{
						// Adjust panel
						RenderingPanel.Visible = true;
						RenderingPanel.Parent = EmitterPreviewPanel;
						RenderingPanel.Size = EmitterPreviewPanel.Size;
						RenderingPanel.Dock = DockStyle.Fill;
						// Show entities
						if (ParticlePreviewEN != null)
						{
							General.ShowEmitter(ParticlePreviewEN, true);
						}
						// Camera settings
						UpdateEmitterCamera();
						Camera.CameraClsColor(EmitterBGR, EmitterBGG, EmitterBGB);
					}
					break;
				// PostProcess editor
				case (int)GETab.POSTPROCESS:
					// Adjust panel
					RenderingPanel.Visible = true;
					RenderingPanel.Parent = dockPostProcess;
					RenderingPanel.Size = dockPostProcess.Size;
					RenderingPanel.Dock = DockStyle.Fill;
					RenderingPanel.Focus();

					if (CurrentClientZone != null)
					{
						CurrentClientZone.SetVisible(true);
						CurrentClientZone.SetListVisible(Triggers, true);
						CurrentClientZone.SetListVisible(Portals, true);
						CurrentClientZone.SetListVisible(Waypoints, true);
					}

					if (CurrentServerZone != null)
					{
						for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
						{
							Waypoint WP = CurrentServerZone.Waypoints[i];

							if (WP.WaypointLinkAEN != null)
								WP.WaypointLinkAEN.Visible = true;
							if (WP.WaypointLinkBEN != null)
								WP.WaypointLinkBEN.Visible = true;

						}
					}

					// Camera settings
					Camera.Position(WorldCamX, WorldCamY, WorldCamZ);
					Camera.Rotate(WorldCamPitch, WorldCamYaw, 0f);

					if (WorldPanelLoop == false)
					{
						RenderingPanel.MouseClick -=
							new MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
//                         RenderingPanel.MouseClick +=
//                             new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
						Application.Idle -= new EventHandler(Program.GE.MainLoop);
						Application.Idle += new EventHandler(m_ZoneRender.WorldRender_MainLoop);
						WorldPanelLoop = true;
					}
					break;
				case (int)GETab.EMITTER:
					// Adjust panel
					RenderingPanel.Visible = true;
					RenderingPanel.Parent = EmitterPreviewPanel;
					RenderingPanel.Size = EmitterPreviewPanel.Size;
					RenderingPanel.Dock = DockStyle.Fill;
					// Show entities
					if (ParticlePreviewEN != null)
					{
						General.ShowEmitter(ParticlePreviewEN, true);
					}
					// Camera settings
					UpdateEmitterCamera();
					Camera.CameraClsColor(EmitterBGR, EmitterBGG, EmitterBGB);
					break;
				case (int)GETab.MESHDIALOG: // GetMesh MediaDialog
					// Adjust panel
                    if (RenderingPanelPreviousIndex == (int)GETab.NEWWORLD)
					{
						WorldCamPitch = Camera.Pitch();
						WorldCamX = Camera.X();
						WorldCamY = Camera.Y();
						WorldCamZ = Camera.Z();
						WorldCamYaw = Camera.Yaw();
					}
					Camera.Position(0, 0, 0);
					Camera.Rotate(0, 0, 0);
					RenderingPanel.Visible = true;
					RenderingPanel.Parent = MediaDialogs.MeshSelector;
					RenderingPanel.Size = new Size(263, 240);
					RenderingPanel.Location = new Point(281, 12);
					RenderingPanel.Dock = DockStyle.None;
					break;
				case (int)GETab.TEXTUREDIALOG: // GetTexture MediaDialog
					// Adjust panel
                    if (RenderingPanelPreviousIndex == (int)GETab.NEWWORLD || RenderingPanelPreviousIndex == (int)GETab.WORLD)
					{
						WorldCamPitch = Camera.Pitch();
						WorldCamX = Camera.X();
						WorldCamY = Camera.Y();
						WorldCamZ = Camera.Z();
						WorldCamYaw = Camera.Yaw();
					}
					Camera.Position(0, 0, 0);
					Camera.Rotate(0, 0, 0);
					RenderingPanel.Visible = true;
					RenderingPanel.Parent = MediaDialogs.TextureSelector;
					RenderingPanel.Size = new Size(263, 240);
					RenderingPanel.Location = new Point(281, 12);
					RenderingPanel.Dock = DockStyle.None;
					break;
				case (int)GETab.NEWWORLD: //New World Editor panel
					if (CurrentClientZone != null)
					{
						CurrentClientZone.SetVisible(true);
						CurrentClientZone.SetListVisible(Triggers, true);
						CurrentClientZone.SetListVisible(Portals, true);
						CurrentClientZone.SetListVisible(Waypoints, true);
					}

					if (CurrentServerZone != null)
					{
						for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
						{
							Waypoint WP = CurrentServerZone.Waypoints[i];

							if (WP.WaypointLinkAEN != null)
								WP.WaypointLinkAEN.Visible = true;
							if (WP.WaypointLinkBEN != null)
								WP.WaypointLinkBEN.Visible = true;

						}
					}

					RenderingPanel.Visible = true;
					RenderingPanel.Parent = m_ZoneRender;
					RenderingPanel.Size = new Size(263, 240);
					RenderingPanel.Location = new Point(281, 12);
					RenderingPanel.Dock = DockStyle.Fill;

					if (EmittersChanged == true)
					{
						int NumEmitters = 0;

						//Array EmittersUsed=Array.CreateInstance( typeof(int), ZoneSelected.Count );
						List<int> EmittersUsed = new List<int>();

						if (CurrentClientZone != null)
						{
							if (ZoneSelected.Count > 0)
							{
								for (int i = 0; i <= ZoneSelected.Count - 1; i++)
								{
									ZoneObject Obj = (ZoneObject)ZoneSelected[i];
									if (Obj is Emitter)
									{
										NumEmitters++;
										EmittersUsed.Add(i);
									}
								}
								if (NumEmitters > 0)
								{
									//Array.Reverse(EmittersUsed);
									EmittersUsed.Reverse();
									for (int i = 0; i <= EmittersUsed.Count - 1; i++)
									{
										Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(
											(ZoneObject)Program.GE.ZoneSelected[EmittersUsed[i]], false);
									}
								}
							}
							CurrentClientZone.ReloadEmitters();


							m_ZoneList.WorldZonesTree.Nodes[2].Nodes.Clear();
							for (int i = 0; i < Program.GE.CurrentClientZone.Emitters.Count; ++i)
							{
								Emitter EM = (Emitter)Program.GE.CurrentClientZone.Emitters[i];
								TreeNode TN;
								if (EM.Config != null)
								{
									TN = new TreeNode(EM.Config.Name);
								}
								else
								{
									TN = new TreeNode("Unknown emitter");
								}

								TN.Tag = EM;
								m_ZoneList.WorldZonesTree.Nodes[2].Nodes.Add(TN);
							}
							m_ZoneList.AddObjectsCount();

						}
						EmittersChanged = false;
					}
					if (CurrentClientZone != null)
					{
						Render.FogMode(1);
						CurrentClientZone.SetViewDistance(Camera, CurrentClientZone.FogNear, CurrentClientZone.FogFar);
						Camera.CameraClsColor(CurrentClientZone.FogR, CurrentClientZone.FogG, CurrentClientZone.FogB);
						Render.FogColor(CurrentClientZone.FogR, CurrentClientZone.FogG, CurrentClientZone.FogB);
						Render.AmbientLight(CurrentClientZone.AmbientR, CurrentClientZone.AmbientG,
											CurrentClientZone.AmbientB);
						//DefaultLight.Direction(???);
					}

					Line3D.SetAllVisible(true, false);
					Line3D.SetAllVisible(ShowGrid, true);
					if (TreeRender != null)
						TreeRender.HideRender();
					if (m_GubbinEditor != null)
						m_GubbinEditor.HideRender(false);

					if (Program.GE.m_ZoneList.WorldZonesTree.Nodes.Count > 9)
					{
						foreach (TreeNode TN in Program.GE.m_ZoneList.WorldZonesTree.Nodes[10].Nodes)
						{
							if (TN.Tag is TreePlacerArea)
								(TN.Tag as TreePlacerArea).Show();
						}
					}

					RenderingPanel.Focus();
					Camera.Position(WorldCamX, WorldCamY, WorldCamZ);
					Camera.Rotate(WorldCamPitch, WorldCamYaw, 0f);
					UpdateRenderMask();

					if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.MOVE)
					{
						if (Program.Transformer != null)
							Program.Transformer.Free();
						Program.Transformer = null;

						if (Program.GE.ZoneSelected.Count > 0)
							if (Program.GE.ZoneSelected[0] is ZoneObject)
								(Program.GE.ZoneSelected[0] as ZoneObject).MoveInit();
					}

					if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.SCALE)
					{
						if (Program.Transformer != null)
							Program.Transformer.Free();
						Program.Transformer = null;

						if (Program.GE.ZoneSelected.Count > 0)
							if (Program.GE.ZoneSelected[0] is ZoneObject)
								(Program.GE.ZoneSelected[0] as ZoneObject).ScaleInit();
					}

					if (Program.GE.SetWorldButtonSelection == (int)RealmCrafter_GE.GE.WorldButtonSelection.ROTATE)
					{
						if (Program.Transformer != null)
							Program.Transformer.Free();
						Program.Transformer = null;

						if (Program.GE.ZoneSelected.Count > 0)
							if (Program.GE.ZoneSelected[0] is ZoneObject)
								(Program.GE.ZoneSelected[0] as ZoneObject).RotateInit();
					}

					if (WorldPanelLoop == false)
					{
						RenderingPanel.MouseClick -=
							new MouseEventHandler(Program.GE.RenderingPanel_MouseClick);
						RenderingPanel.MouseClick +=
							new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
						Application.Idle -= new EventHandler(Program.GE.MainLoop);
						Application.Idle += new EventHandler(m_ZoneRender.WorldRender_MainLoop);
						WorldPanelLoop = true;
					}
					break;
				case (int)GETab.INTERFACE:
					RenderingPanel.Visible = true;
					RenderingPanel.Parent = m_Interface.InterfacePanelRender;
					m_InterfaceHierarchy.Enabled = true;
					RenderingPanel.Size = new Size(263, 240);
					RenderingPanel.Location = new Point(281, 12);
					RenderingPanel.Dock = DockStyle.Fill;
					Camera.CameraClsColor(0, 50, 100);
                    //RenderingPanel.MouseDown += RenderingPanel_MouseDown;
                    //RenderingPanel.MouseMove += RenderingPanel_MouseMove;
					RenderingPanel.Focus();
					Camera.Position(WorldCamX, WorldCamY, WorldCamZ);
					Camera.Rotate(WorldCamPitch, WorldCamYaw, 0f);
                    Application.Idle += new EventHandler(MainLoop);
					if (WorldPanelLoop == true)
					{
                        
						RenderingPanel.MouseClick -=
							new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
						RenderingPanel.MouseClick += new MouseEventHandler(RenderingPanel_MouseClick);
						Application.Idle -= new EventHandler(m_ZoneRender.WorldRender_MainLoop);
						
						WorldPanelLoop = false;
					}
					break;
                case (int)GETab.TREE:
					{
						RenderingPanel.Visible = true;
						RenderingPanel.Parent = TreeRender;
						RenderingPanel.Dock = DockStyle.Fill;

						Render.Graphics3D(Program.GE.RenderingPanel.Width, Program.GE.RenderingPanel.Height, 32, 2, 0, 0,
								  @".\Data\DefaultTex.png");

						RenderingPanel.MouseMove += new MouseEventHandler(TreeRender.RenderPanel_MouseMove);
						RenderingPanel.MouseDown += new MouseEventHandler(TreeRender.RenderPanel_MouseDown);
						RenderingPanel.MouseUp += new MouseEventHandler(TreeRender.RenderPanel_MouseUp);
						RenderingPanel.PreviewKeyDown += new PreviewKeyDownEventHandler(TreeRender.TreeEditorRender_PreviewKeyDown);
						RenderingPanel.MouseClick -=
							new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
						Application.Idle -= new EventHandler(m_ZoneRender.WorldRender_MainLoop);
						Application.Idle -= new EventHandler(m_GubbinEditor.Application_Idle);

						if (TreeRender != null)
						{
							TreeRender.ShowRender();

							if (TreeProperties != null)
								TreeProperties.RealShow();

						}
						if (Program.ActiveTree != null)
							Program.ActiveTree.Visible = true;

						Application.Idle += new EventHandler(TreeRender.Application_Idle);

						break;
					}
				case (int)GETab.SCRIPT:
					{
						Program.GE.m_ScriptView.UpdateRenderingPanel(true);
						Application.Idle -= new EventHandler(m_ZoneRender.WorldRender_MainLoop);
						Application.Idle += new EventHandler(m_ScriptView.Application_Idle);
						RenderingPanel.MouseMove += new MouseEventHandler(m_ScriptView.RenderPanel_MouseMove);
						RenderingPanel.MouseDoubleClick += new MouseEventHandler(m_ScriptView.RenderPanel_MouseDoubleClick);
						RenderingPanel.MouseDown += new MouseEventHandler(m_ScriptView.RenderPanel_MouseDown);
						RenderingPanel.MouseUp += new MouseEventHandler(m_ScriptView.RenderPanel_MouseUp);
						RenderingPanel.PreviewKeyDown += new PreviewKeyDownEventHandler(m_ScriptView.RenderPanel_PreviewKeyDown);
						RenderingPanel.MouseClick -=
							new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);

						break;
					}
                case (int)GETab.GUBBIN:
					{
						RenderingPanel.Visible = true;
						RenderingPanel.Parent = m_GubbinEditor.splitContainer1.Panel2;
						RenderingPanel.Dock = DockStyle.Fill;

						Render.Graphics3D(Program.GE.RenderingPanel.Width, Program.GE.RenderingPanel.Height, 32, 2, 0, 0,
								  @".\Data\DefaultTex.png");

						RenderingPanel.MouseMove += new MouseEventHandler(m_GubbinEditor.RenderPanel_MouseMove);
						RenderingPanel.MouseDown += new MouseEventHandler(m_GubbinEditor.RenderPanel_MouseDown);
						RenderingPanel.MouseUp += new MouseEventHandler(m_GubbinEditor.RenderPanel_MouseUp);
						RenderingPanel.PreviewKeyDown += new PreviewKeyDownEventHandler(m_GubbinEditor.GubbinEditor_PreviewKeyDown);
						RenderingPanel.MouseClick -=
							new MouseEventHandler(m_ZoneRender.RenderingPanel_MouseClick);
						Application.Idle -= new EventHandler(m_ZoneRender.WorldRender_MainLoop);

						if (m_GubbinEditor != null)
						{
							m_GubbinEditor.ShowRender(RenderingPanelPreviousIndex == (int)GETab.MESHDIALOG);

						}

						Application.Idle += new EventHandler(m_GubbinEditor.Application_Idle);

						break;
					}
				default:
					// Adjust panel
					RenderingPanel.Parent = this;
					RenderingPanel.Visible = false;
					RenderingPanel.Dock = DockStyle.None;
					RenderingPanel.Size = new Size(0, 0);
					break;
			}
			RenderingPanel.Focus();
		}

		private void RenderingPanel_SizeChanged(object sender, EventArgs e)
		{
			if (RenderingPanel.Width > 0 && RenderingPanel.Height > 0)
			{
				Render.Graphics3D(RenderingPanel.Width, RenderingPanel.Height, 32, 3, 0, 0,
								  Program.TestingVersion ? @".\Data\DefaultTex.png" : @"..\..\Data\GUE\DefaultTex.PNG");
			}
		}

		public void RefreshScripts()
		{
			string[] ScriptFiles = Directory.GetFiles(@"Data\Server Data\Scripts", "*.cs");

			string[] TempArray = new string[ScriptFiles.GetUpperBound(0) + 1];
			int Count = 0;
			foreach (string S in ScriptFiles)
			{
				if (Path.GetExtension(S) == "." + Program.ScriptExtension)
				{
					TempArray[Count] = Path.GetFileName(S);
					++Count;
				}
			}

			ScriptsList = new string[Count];
			//GE.ScriptsList = (string[]ResizeArray
			Array.Copy(TempArray, ScriptsList, Count);

			AbilityScriptCombo.BeginUpdate();
			AbilityScriptCombo.Items.Clear();
			ItemUseScriptCombo.BeginUpdate();
			ItemUseScriptCombo.Items.Clear();
			foreach (string S in GE.ScriptsList)
			{
				AbilityScriptCombo.Items.Add(S);
				ItemUseScriptCombo.Items.Add(S);
			}
			ItemUseScriptCombo.EndUpdate();
			AbilityScriptCombo.EndUpdate();
			
			ZoneSetupForm.UpdateZoneSetup(true);
			m_CreateWindow.UpdateScripts();
		}

		public static List<string> ActorList()
		{
			List<string> ActorNames = new List<string>();

            foreach(Actor actor in Actor.Index.Values)
                ActorNames.Add(actor.Race.ToString() + " [" + actor.Class.ToString() + "]" + "  ID: " + actor.ID.ToString());

            return ActorNames;
            /*
			Actor Ac = Actor.FirstActor;
			Ac = Actor.FirstActor;
			int NumActors = 1;
			while (Ac != null)
			{
				if (Ac.ID > NumActors)
				{
					NumActors = Ac.ID;
				}

				Ac = Ac.NextActor;
			}
			for (int i = 0; i < NumActors + 5; i++)
			{
				ActorNames.Add("");
			}

			Ac = Actor.FirstActor;

			while (Ac != null)
			{
				ActorNames.Insert(Ac.ID + 1,
								  Ac.Race.ToString() + " [" + Ac.Class.ToString() + "]" + "  ID: " + Ac.ID.ToString());
				Ac = Ac.NextActor;
			}
			bool more;
			for (;;)
			{
				more = ActorNames.Remove("");
				if (more == false)
				{
					break;
				}
			}
			ActorNames.Insert(0, "");

			return ActorNames;
            */
		}


		#region Helper functions
		// Linearly interpolates between two values
		private static float Lerp(float Start, float Dest, float Proportion)
		{
			return Start + ((Dest - Start) / Proportion);
		}

		// Replaces an index in a ComboBox or ListBox, keeping the current selection
		private static void ReplaceComboItem(ComboBox C, int Idx, object NewItem)
		{
			object CurrentSelection;
			if (C.SelectedIndex == Idx)
			{
				CurrentSelection = NewItem;
			}
			else
			{
				CurrentSelection = C.SelectedItem;
			}

			C.Items.RemoveAt(Idx);
			int NewIdx = C.Items.Add(NewItem);

			C.SelectedItem = CurrentSelection;
		}

		private static void ReplaceComboItem(ListBox C, int Idx, object NewItem)
		{
			object CurrentSelection;
			if (C.SelectedIndex == Idx)
			{
				CurrentSelection = NewItem;
			}
			else
			{
				CurrentSelection = C.SelectedItem;
			}

			C.Items.RemoveAt(Idx);
			int NewIdx = C.Items.Add(NewItem);

			C.SelectedItem = CurrentSelection;
		}

		// Loads a texture, assigns it to an entity, and frees it again
		public static void PerformTexture(Entity EN, string TexFile, int Flags)
		{
			uint Tex = Render.LoadTexture(TexFile, Flags);
			EN.Texture(Tex);
			Render.FreeTexture(Tex);
		}

		public static void PerformTexture(Entity EN, string TexFile)
		{
			PerformTexture(EN, TexFile, 0);
		}

		public static void PerformTexture(object Handle, string TexFile)
		{
			Entity EN = (Entity) Handle;
			PerformTexture(EN, TexFile, 0);
		}

		public static void PerformTexture(object Handle, string TexFile, int Flags)
		{
			Entity EN = (Entity) Handle;
			PerformTexture(EN, TexFile, Flags);
		}

		// Returns the filename only for a media ID
		public static string NiceMeshName(int ID)
		{
			if (ID >= 65535)
			{
				return "No mesh set";
			}

			string MediaName = Media.GetMeshName(ID);
			if (!string.IsNullOrEmpty(MediaName))
			{
				return Path.GetFileName(MediaName.Remove(MediaName.Length - 1));
			}
			else
			{
				return "";
			}
		}

		public static string NiceTextureName(int ID)
		{
			if (ID >= 65535)
			{
				return "No texture set";
			}

			string MediaName = Media.GetTextureName(ID);
			if (!string.IsNullOrEmpty(MediaName))
			{
				return Path.GetFileName(MediaName.Remove(MediaName.Length - 1));
			}
			else
			{
				return "";
			}
		}

		public static string NiceSoundName(int ID)
		{
			if (ID >= 65535 || ID < 0)
			{
				return "No sound set";
			}

			string MediaName = Media.GetSoundName(ID);
			if (!string.IsNullOrEmpty(MediaName))
			{
				return Path.GetFileName(MediaName.Remove(MediaName.Length - 1));
			}
			else
			{
				return "";
			}
		}

		public static string NiceMusicName(int ID)
		{
			if (ID >= 65535)
			{
				return "No music set";
			}

			return Path.GetFileName(Media.GetMusicName(ID));
		}

		// Removes a node from a treeview, along with all its parents which no longer have children
		private void RemoveTreeViewNode(TreeNode T)
		{
			if (T == null)
				return;

			TreeNode OldParent;
			TreeNode Parent = T.Parent;
			T.Remove();
			
			if (Parent == null)
				return;

			if (Parent.Parent != null)
			{
				while (Parent.Nodes.Count == 0)
				{
					OldParent = Parent;
					Parent = OldParent.Parent;
					if (Parent == null)
						return;

					Parent.Nodes.Remove(OldParent);
				}
			}
		}

		// Fill a list with the functions available within a given script
		public ListBoxItem[] GetScriptFunctionsList(string Script)
		{
			if (string.IsNullOrEmpty(Script))
			{
				return null;
			}

			// Go through each line of the file
			BinaryReader F = Blitz.ReadFile(@"Data\Server Data\Scripts\" + Script);
			if (F == null)
			{
				return null;
			}

			long Length = F.BaseStream.Length;
			string NewLine;
			Regex FuncSig = new Regex("public\\s*void\\s*");
			Regex ArgSig = new Regex("\\s*\\(");

			List<ListBoxItem> Result = new List<ListBoxItem>();

			while (F.BaseStream.Position < Length)
			{
				NewLine = Blitz.ReadLine(F);

				if (FuncSig.IsMatch(NewLine))
				{
					string[] L = FuncSig.Split(NewLine);

					if (L.Length > 1 && ArgSig.IsMatch(L[1]))
					{
						L = ArgSig.Split(L[1]);
						Result.Add(new ListBoxItem(L[0], 0));
					}
				}
			}

			F.Close();

			if (Result.Count > 0)
				return Result.ToArray();
			return null;
		}

		// Returns a list of portals within a zone
		public List<string> ZonePortalNames(RealmCrafter.ServerZone.Zone Z)
		{
			List<string> PortalsList = new List<string>();

			for (int i = 0; i < Z.Portals.Count; ++i)
			{
				if (!string.IsNullOrEmpty(Z.Portals[i].Name))
				{
					PortalsList.Add(Z.Portals[i].Name);
				}
			}

			return PortalsList;
		}

		// Copy a folder and all its subfolders (RECURSIVE)
		public static void CopyTree(string Dir, string DestinationDir)
		{
			// Create destination folder if required
			if (!Directory.Exists(DestinationDir))
			{
				Directory.CreateDirectory(DestinationDir);
			}

			// Copy each file in this folder
			string[] Files = Directory.GetFiles(Dir);
			foreach (string S in Files)
			{
				try
				{
					File.Copy(S, DestinationDir + @"\" + Path.GetFileName(S), true);
				}
				catch (System.Exception ex)
				{
					MessageBox.Show("Exception thrown when copying file: " + S + ".\nThis may cause problems when running the built project\n\nException Info:\n" + ex.Message);
				}
				
			}

			// Recursively copy each subfolder
			string[] Folders = Directory.GetDirectories(Dir);
			foreach (string S in Folders)
			{
				if (S != "." && S != "..")
				{
					string SubDir = Path.GetFileName(S);
					CopyTree(Dir + @"\" + SubDir, DestinationDir + @"\" + SubDir);
				}
			}
		}

		// Safely copies a file (i.e. checks for its existence etc. first)
		public static void SafeCopyFile(string Source, string Destination)
		{
			if (File.Exists(Source))
			{
				try
				{
					File.Copy(Source, Destination, true);
				}
				catch(IOException e)
				{
					MessageBox.Show("Error copying: " + Source + " to " + @"Game\" + Destination + "\nError: " + e.Message +
									"\nPlease copy manually to the Game folder");
				}
			}
		}
		#endregion

		
	



		#region Actors tab events
		private void ActorsTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Load actors of GubbinToolActorCombo

			if (ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.FACTIONS)
			{
				LoadValueFromFactionsDataGridView();
			}
			else if (ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.ANIMATION)
			{
				ActorAnimationListBox.Items.Clear();
				for (int i = 0; i < ActorsList.Items.Count; i++)
				{
					ActorAnimationListBox.Items.Add(ActorsList.Items[i]);
				}
				NoActorSelectedLabel.Visible = false;
			}
			else if (ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.ACTORS)
			{
				// Camera
				Camera.Position(0, 0, 0);
				Camera.Rotate(0, 0, 0);
			}
			ClearActorPreviewAnimation();
            UpdateRenderingPanel((int)GETab.ACTORS);
		}

		public void LoadValueFromFactionsDataGridView()
		{
			FactionsDataGridView.Rows.Clear();
			FactionsDataGridView.Columns.Clear();
			byte noFaction = 0;
			for (byte i = 0; i < Actor.TotalFactions; i++)
			{
				if (Actor.FactionNames[i] == "")
				{
					continue;
				}
				FactionsDataGridView.Columns.Add("col1", Actor.FactionNames[i]);
				FactionsDataGridView.Rows.Add();
				FactionsDataGridView.Rows[noFaction].HeaderCell.Value = Actor.FactionNames[i];
				noFaction++;
			}

			FactionsDataGridView.RowHeadersWidth = 130;

			for (byte i = 0; i < noFaction; i++)
			{
				for (byte j = 0; j < noFaction; j++)
				{
					FactionsDataGridView.Rows[i].Cells[j].Value = Actor.FactionDefaultRatings[i, j] - 100;
				}
			}
		}

		private void ActorNewButton_Click(object sender, EventArgs e)
		{
			try
			{
				SelectedActor = new Actor();
                Actor.Index.Add(SelectedActor.ID, SelectedActor);
				TotalActors++;
				SelectedActor.Race = "New actor";
				SelectedActor.Class = "None";
				int Idx =
					ActorsList.Items.Add(new ListBoxItem(SelectedActor.Race + " [" + SelectedActor.Class + "]",
                                                         (uint)SelectedActor.ID));


				SetSelectedActor(Idx);
				SetActorsSavedStatus(false);
			}
			catch (ActorException)
			{
				MessageBox.Show("Limit of 65535 actors already reached!", "Error");
			}
		}

		private void ActorCopyButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				Actor NewActor = new Actor();
                Actor.Index.Add(NewActor.ID, NewActor);
				TotalActors++;
				NewActor.Race = SelectedActor.Race;
				NewActor.Class = "Copy";
				NewActor.Description = SelectedActor.Description;
				NewActor.StartArea = SelectedActor.StartArea;
				NewActor.StartPortal = SelectedActor.StartPortal;
				NewActor.Radius = SelectedActor.Radius;
				NewActor.Scale = SelectedActor.Scale;
				NewActor.BloodTexID = SelectedActor.BloodTexID;
				NewActor.Genders = SelectedActor.Genders;
				NewActor.MaleAnimationSet = SelectedActor.MaleAnimationSet;
				NewActor.FemaleAnimationSet = SelectedActor.FemaleAnimationSet;
				NewActor.Playable = SelectedActor.Playable;
				NewActor.Rideable = SelectedActor.Rideable;
				NewActor.Aggressiveness = SelectedActor.Aggressiveness;
				NewActor.AggressiveRange = SelectedActor.AggressiveRange;
				NewActor.TradeMode = SelectedActor.TradeMode;
				NewActor.EType = SelectedActor.EType;
				NewActor.InventorySlots = SelectedActor.InventorySlots;
				NewActor.DefaultDamageType = SelectedActor.DefaultDamageType;
				NewActor.DefaultFaction = SelectedActor.DefaultFaction;
				NewActor.XPMultiplier = SelectedActor.XPMultiplier;
				NewActor.PolyCollision = SelectedActor.PolyCollision;
				for (int i = 0; i < Attributes.TotalAttributes; ++i)
				{
					NewActor.Attributes.Value[i] = SelectedActor.Attributes.Value[i];
					NewActor.Attributes.Maximum[i] = SelectedActor.Attributes.Maximum[i];
				}
				for (int i = 0; i < Item.TotalDamageTypes; ++i)
				{
					NewActor.Resistances[i] = SelectedActor.Resistances[i];
				}

				NewActor.MaleMesh = SelectedActor.MaleMesh;
				NewActor.FemaleMesh = SelectedActor.MaleMesh;
				NewActor.DefaultGubbins.Clear();
				NewActor.DefaultGubbins.AddRange(SelectedActor.DefaultGubbins);

				NewActor.Beards.Clear();
				NewActor.MaleHairs.Clear();
				NewActor.FemaleHairs.Clear();
				for (int i = 0; i < SelectedActor.Beards.Count; ++i)
				{
					List<GubbinTemplate> TList = new List<GubbinTemplate>();
					TList.AddRange(SelectedActor.Beards[i]);
					NewActor.Beards.Add(TList);
				}

				for (int i = 0; i < SelectedActor.MaleHairs.Count; ++i)
				{
					List<GubbinTemplate> TList = new List<GubbinTemplate>();
					TList.AddRange(SelectedActor.MaleHairs[i]);
					NewActor.MaleHairs.Add(TList);
				}

				for (int i = 0; i < SelectedActor.FemaleHairs.Count; ++i)
				{
					List<GubbinTemplate> TList = new List<GubbinTemplate>();
					TList.AddRange(SelectedActor.FemaleHairs[i]);
					NewActor.FemaleHairs.Add(TList);
				}

				NewActor.MaleFaceIDs = new ActorTextureSet[SelectedActor.MaleFaceIDs.Length];
				NewActor.FemaleFaceIDs = new ActorTextureSet[SelectedActor.FemaleFaceIDs.Length];
				NewActor.MaleBodyIDs = new ActorTextureSet[SelectedActor.MaleBodyIDs.Length];
				NewActor.FemaleBodyIDs = new ActorTextureSet[SelectedActor.FemaleBodyIDs.Length];

				SelectedActor.MaleFaceIDs.CopyTo(NewActor.MaleFaceIDs, 0);
				SelectedActor.FemaleFaceIDs.CopyTo(NewActor.FemaleFaceIDs, 0);
				SelectedActor.MaleBodyIDs.CopyTo(NewActor.MaleBodyIDs, 0);
				SelectedActor.FemaleBodyIDs.CopyTo(NewActor.FemaleBodyIDs, 0);

				for (int i = 0; i < 16; ++i)
				{
					NewActor.MaleSpeechIDs[i] = SelectedActor.MaleSpeechIDs[i];
					NewActor.FemaleSpeechIDs[i] = SelectedActor.FemaleSpeechIDs[i];
				}
                int Idx = ActorsList.Items.Add(new ListBoxItem(NewActor.Race + " [" + NewActor.Class + "]", (uint)NewActor.ID));


				SetSelectedActor(Idx);
				SetActorsSavedStatus(false);
			}
		}

		private void ActorDeleteButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{

				SuppressActorChange = true;
				ActorsList.Items.RemoveAt(ActorsList.SelectedIndex);
				SuppressActorChange = false;
                Actor.Index.Remove(SelectedActor.ID);
				SelectedActor.Delete();
				SelectedActor = null;
				SetSelectedActor(0);
				SetActorsSavedStatus(false);
			}
		}

		private void ActorsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SuppressActorChange)
			{
				SetSelectedActor(ActorsList.SelectedIndex);
			}
		}

		private void ActorSubTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ActorSubTabs.SelectedTab == ActorSubTabPreview)
			{
				ClearActorPreview();
				LoadActorPreview();
			}
		}

		private void ActorRaceText_TextChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.Race != ActorRaceText.Text)
				{
					SelectedActor.Race = ActorRaceText.Text;
					SetActorsSavedStatus(false);
				}

				// Update the actors listbox and spawn point combo
				ActorsList.BeginUpdate();
				SuppressActorChange = true;
				int Idx = ActorsList.SelectedIndex;
				ListBoxItem LBI = (ListBoxItem) ActorsList.SelectedItem;
				//ListBoxItem SpawnLBI = WorldObjectWaypointActor.SelectedItem as ListBoxItem;
				LBI = new ListBoxItem(SelectedActor.Race + " [" + SelectedActor.Class + "]", LBI.Value);
				ActorsList.Items.RemoveAt(Idx);
				Idx = ActorsList.Items.Add(LBI);
				ActorsList.SelectedIndex = Idx;
				//    WorldObjectWaypointActor.Items.Clear();
				//   WorldObjectWaypointActor.Items.Add("(None)");
				//    WorldObjectWaypointActor.SelectedIndex = 0;

				Actor Ac = Actor.FirstActor;
				/* while (Ac != null)
				{
					// Marian Voicu
					CharacterSetExclusiveRaceComboBox.Items.Add(Ac.Race);
					// end (MV)
					WorldObjectWaypointActor.Items.Add(new ListBoxItem(Ac.Race + " [" + Ac.Class + "]", Ac.ID));
					if (SpawnLBI != null && Ac.ID == SpawnLBI.Value)
					{
						WorldObjectWaypointActor.SelectedIndex = WorldObjectWaypointActor.Items.Count - 1;
					}
					Ac = Ac.NextActor;
				} */
				SuppressActorChange = false;
				ActorsList.EndUpdate();
			}
		}

		private void ActorClassText_TextChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.Class != ActorClassText.Text)
				{
					SelectedActor.Class = ActorClassText.Text;
					SetActorsSavedStatus(false);
				}

				// Update the actors listbox and spawn point combo
				ActorsList.BeginUpdate();
				SuppressActorChange = true;
				int Idx = ActorsList.SelectedIndex;
				ListBoxItem LBI = (ListBoxItem) ActorsList.SelectedItem;
				//ListBoxItem SpawnLBI = WorldObjectWaypointActor.SelectedItem as ListBoxItem;
				LBI = new ListBoxItem(SelectedActor.Race + " [" + SelectedActor.Class + "]", LBI.Value);
				ActorsList.Items.RemoveAt(Idx);
				Idx = ActorsList.Items.Add(LBI);
				ActorsList.SelectedIndex = Idx;
				//WorldObjectWaypointActor.Items.Clear();
				//WorldObjectWaypointActor.Items.Add("(None)");
//                WorldObjectWaypointActor.SelectedIndex = 0;

				/*  Actor Ac = Actor.FirstActor;
				while (Ac != null)
				{
					CharacterSetExclusiveClassComboBox.Items.Add(Ac.Class);
					WorldObjectWaypointActor.Items.Add(new ListBoxItem(Ac.Race + " [" + Ac.Class + "]", Ac.ID));
					if (SpawnLBI != null && Ac.ID == SpawnLBI.Value)
					{
						WorldObjectWaypointActor.SelectedIndex = WorldObjectWaypointActor.Items.Count - 1;
					}
					Ac = Ac.NextActor;
				}*/
				SuppressActorChange = false;
				ActorsList.EndUpdate();
			}
		}

		private void ActorDescriptionText_TextChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.Description != ActorDescriptionText.Text)
				{
					SelectedActor.Description = ActorDescriptionText.Text;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorGendersCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.Genders != ActorGendersCombo.SelectedIndex)
				{
					SelectedActor.Genders = (byte) ActorGendersCombo.SelectedIndex;
					SetActorsSavedStatus(false);
					EnableActorGadgets();
				}
			}
		}

		private void ActorAttributesList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ActorAttributesList.SelectedIndex >= 0 && SelectedActor != null)
			{
				ListBoxItem LBI = (ListBoxItem) ActorAttributesList.Items[ActorAttributesList.SelectedIndex];
				ActorAttributeSpinner.Value = SelectedActor.Attributes.Value[LBI.Value];
				ActorAttributeMaxSpinner.Value = SelectedActor.Attributes.Maximum[LBI.Value];
			}
			else
			{
				ActorAttributeSpinner.Value = 0;
				ActorAttributeMaxSpinner.Value = 0;
			}
		}

		private void ActorAttributeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ActorAttributesList.SelectedIndex >= 0 && SelectedActor != null)
			{
				if (SelectedActor.Attributes.Value[ActorAttributesList.SelectedIndex] !=
					(int) ActorAttributeSpinner.Value)
				{
					SelectedActor.Attributes.Value[ActorAttributesList.SelectedIndex] =
						(int) ActorAttributeSpinner.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorAttributeMaxSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ActorAttributesList.SelectedIndex >= 0 && SelectedActor != null)
			{
				if (SelectedActor.Attributes.Maximum[ActorAttributesList.SelectedIndex] !=
					(int) ActorAttributeMaxSpinner.Value)
				{
					SelectedActor.Attributes.Maximum[ActorAttributesList.SelectedIndex] =
						(int) ActorAttributeMaxSpinner.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorPlayableCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (ActorPlayableCheck.Checked != SelectedActor.Playable)
				{
					SelectedActor.Playable = ActorPlayableCheck.Checked;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorRideableCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (ActorRideableCheck.Checked != SelectedActor.Rideable)
				{
					SelectedActor.Rideable = ActorRideableCheck.Checked;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorStartZoneCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.StartArea != (string) ActorStartZoneCombo.SelectedItem)
				{
					SelectedActor.StartArea = (string) ActorStartZoneCombo.SelectedItem;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorStartPortalText_TextChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.StartPortal != ActorStartPortalText.Text)
				{
					SelectedActor.StartPortal = ActorStartPortalText.Text;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorAggressionCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.Aggressiveness != ActorAggressionCombo.SelectedIndex)
				{
					SelectedActor.Aggressiveness = (byte) ActorAggressionCombo.SelectedIndex;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorAggressionRangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.AggressiveRange != ActorAggressionRangeSpinner.Value)
				{
					SelectedActor.AggressiveRange = (int) ActorAggressionRangeSpinner.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorXPSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.XPMultiplier != ActorXPSpinner.Value)
				{
					SelectedActor.XPMultiplier = (int) ActorXPSpinner.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorTradeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if (SelectedActor.TradeMode != ActorTradeCombo.SelectedIndex)
				{
					SelectedActor.TradeMode = (byte) ActorTradeCombo.SelectedIndex;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorEnvironmentCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				if ((int) SelectedActor.EType != ActorEnvironmentCombo.SelectedIndex)
				{
					SelectedActor.EType = (Actor.Environment) ActorEnvironmentCombo.SelectedIndex;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorMAnimSetCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ListBoxItem LBI = (ListBoxItem) ActorMAnimSetCombo.SelectedItem;
				if (SelectedActor.MaleAnimationSet != LBI.Value)
				{
					SelectedActor.MaleAnimationSet = (ushort) LBI.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorFAnimSetCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ListBoxItem LBI = (ListBoxItem) ActorFAnimSetCombo.SelectedItem;
				if (SelectedActor.FemaleAnimationSet != LBI.Value)
				{
					SelectedActor.FemaleAnimationSet = (ushort) LBI.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorMSoundCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ActorMSoundButton.Text = NiceSoundName(SelectedActor.MaleSpeechIDs[ActorMSoundCombo.SelectedIndex]);
			}
		}

		private void ActorFSoundCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ActorFSoundButton.Text = NiceSoundName(SelectedActor.FemaleSpeechIDs[ActorFSoundCombo.SelectedIndex]);
			}
		}

		private void ActorMSoundButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				SelectedActor.MaleSpeechIDs[ActorMSoundCombo.SelectedIndex] = MediaDialogs.GetSound(true,
																									SelectedActor.
																										MaleSpeechIDs[
																										ActorMSoundCombo
																											.
																											SelectedIndex
																										]);
				SetActorsSavedStatus(false);

				ActorMSoundButton.Text = NiceSoundName(SelectedActor.MaleSpeechIDs[ActorMSoundCombo.SelectedIndex]);
			}
		}

		private void ActorFSoundButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				SelectedActor.FemaleSpeechIDs[ActorFSoundCombo.SelectedIndex] = MediaDialogs.GetSound(true,
																									  SelectedActor.
																										  FemaleSpeechIDs
																										  [
																										  ActorFSoundCombo
																											  .
																											  SelectedIndex
																										  ]);
				SetActorsSavedStatus(false);

				ActorFSoundButton.Text = NiceSoundName(SelectedActor.FemaleSpeechIDs[ActorFSoundCombo.SelectedIndex]);
			}
		}

		private void ActorMBodyButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ushort NewMesh = MediaDialogs.GetMesh(true, "Actors", SelectedActor.MaleMesh);
				if (NewMesh < 65535)
				{
					SelectedActor.MaleMesh = NewMesh;
					SetActorsSavedStatus(false);

					ActorMBodyButton.Text = NiceMeshName(SelectedActor.MaleMesh);
				}
			}
		}

		private void ActorFBodyButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ushort NewMesh = MediaDialogs.GetMesh(true, "Actors", SelectedActor.FemaleMesh);
				if (NewMesh < 65535)
				{
					SelectedActor.FemaleMesh = NewMesh;
					SetActorsSavedStatus(false);

					ActorFBodyButton.Text = NiceMeshName(SelectedActor.FemaleMesh);
				}
			}
		}

		private void ActorMHairButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				GubbinSetEditor Editor = new GubbinSetEditor();
				Editor.GubbinSets = SelectedActor.MaleHairs;

				if (Editor.ShowDialog() == DialogResult.OK)
					SetActorsSavedStatus(false);

				SetActorsSavedStatus(false);
			}
		}

		private void ActorFHairButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				GubbinSetEditor Editor = new GubbinSetEditor();
				Editor.GubbinSets = SelectedActor.FemaleHairs;

				if (Editor.ShowDialog() == DialogResult.OK)
					SetActorsSavedStatus(false);
			}
		}

		private void ActorMFaceButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				RealmCrafter_GE.ActorTextureSetEditor Edt = new RealmCrafter_GE.ActorTextureSetEditor();
				Edt.TextureSet = SelectedActor.MaleFaceIDs;
				if (Edt.ShowDialog() == DialogResult.OK)
				{
					SelectedActor.MaleFaceIDs = Edt.TextureSet;
					SetActorsSavedStatus(false);
				}

//                 SelectedActor.MaleFaceIDs[ActorMFaceNCombo.SelectedIndex] = 
//                     MediaDialogs.GetTexture(true, "Actors", SelectedActor.MaleFaceIDs[ActorMFaceNCombo.SelectedIndex]);
//                ActorMFaceButton.Text = NiceTextureName(SelectedActor.MaleFaceIDs[ActorMFaceNCombo.SelectedIndex]);
			}
		}

		private void ActorFFaceButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				RealmCrafter_GE.ActorTextureSetEditor Edt = new RealmCrafter_GE.ActorTextureSetEditor();
				Edt.TextureSet = SelectedActor.FemaleFaceIDs;
				if (Edt.ShowDialog() == DialogResult.OK)
				{
					SelectedActor.FemaleFaceIDs = Edt.TextureSet;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorMBodyTexButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				RealmCrafter_GE.ActorTextureSetEditor Edt = new RealmCrafter_GE.ActorTextureSetEditor();
				Edt.TextureSet = SelectedActor.MaleBodyIDs;
				if (Edt.ShowDialog() == DialogResult.OK)
				{
					SelectedActor.MaleBodyIDs = Edt.TextureSet;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorFBodyTexButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				RealmCrafter_GE.ActorTextureSetEditor Edt = new RealmCrafter_GE.ActorTextureSetEditor();
				Edt.TextureSet = SelectedActor.FemaleBodyIDs;
				if (Edt.ShowDialog() == DialogResult.OK)
				{
					SelectedActor.FemaleBodyIDs = Edt.TextureSet;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorBeardButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				GubbinSetEditor Editor = new GubbinSetEditor();
				Editor.GubbinSets = SelectedActor.Beards;

				if (Editor.ShowDialog() == DialogResult.OK)
					SetActorsSavedStatus(false);
			}
		}

		private void ActorBloodButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				SelectedActor.BloodTexID = MediaDialogs.GetTexture(false, SelectedActor.BloodTexID);
				SetActorsSavedStatus(false);

				ActorBloodButton.Text = NiceTextureName(SelectedActor.BloodTexID);
			}
		}

		private void ActorGubbinButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
//                 SelectedActor.MeshIDs[ActorGubbinNCombo.SelectedIndex + 2] = MediaDialogs.GetMesh(true, "Gubbins",
//                                                                                                   SelectedActor.MeshIDs[
//                                                                                                       ActorGubbinNCombo.
//                                                                                                           SelectedIndex +
//                                                                                                       2]);
//                 SetActorsSavedStatus(false);
// 
//                 ActorGubbinButton.Text = NiceMeshName(SelectedActor.MeshIDs[ActorGubbinNCombo.SelectedIndex + 2]);

				GubbinSelector Sel = new GubbinSelector();
				Sel.CurrentSaved = SelectedActor.DefaultGubbins;

				if (Sel.ShowDialog() == DialogResult.OK)
					SetActorsSavedStatus(false);


			}
		}

		private void ActorScaleSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				float NewValue = (float) ActorScaleSpinner.Value / 100f;
				if (Math.Abs(SelectedActor.Scale - NewValue) > 0.001)
				{
					SelectedActor.Scale = NewValue;
					SetActorsSavedStatus(false);
				}
			}
		}


		private void ActorSlotDisabledCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ActorSlotDisableCheck.Checked =
					!Actor.GetFlag(SelectedActor.InventorySlots, ActorSlotDisabledCombo.SelectedIndex);
			}
		}

		private void ActorSlotDisableCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				int Value = SelectedActor.InventorySlots;
				int Flag = ActorSlotDisabledCombo.SelectedIndex;
				Value &= 0xFFFF ^ (1 << Flag);
				if (!ActorSlotDisableCheck.Checked)
				{
					Value |= 1 << Flag;
				}

				if (Value != SelectedActor.InventorySlots)
				{
					SelectedActor.InventorySlots = (ushort) Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorResistancesList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null && ActorResistancesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) ActorResistancesList.Items[ActorResistancesList.SelectedIndex];
				ActorResistanceSpinner.Value = SelectedActor.Resistances[LBI.Value];
			}
			else
			{
				ActorResistanceSpinner.Value = 0;
			}
		}

		private void ActorResistanceSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null && ActorResistancesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) ActorResistancesList.Items[ActorResistancesList.SelectedIndex];
				if (SelectedActor.Resistances[LBI.Value] != ActorResistanceSpinner.Value)
				{
					SelectedActor.Resistances[LBI.Value] = (ushort) ActorResistanceSpinner.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorFactionCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null && ActorFactionCombo.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) ActorFactionCombo.Items[ActorFactionCombo.SelectedIndex];
				if (SelectedActor.DefaultFaction != LBI.Value)
				{
					SelectedActor.DefaultFaction = (byte) LBI.Value;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorChangeAttributesButton_Click(object sender, EventArgs e)
		{
			EditAttributes();
		}

		private void ActorHairColourNCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				ActorHairColourButton.BackColor =
					Color.FromArgb(SelectedActor.HairColors[ActorHairColourNCombo.SelectedIndex]);
			}
		}

		private void ActorHairColourButton_Click(object sender, EventArgs e)
		{
			if (SelectedActor != null)
			{
				DialogResult D = ColourDialog.ShowDialog();
				if (D == DialogResult.OK)
				{
					SelectedActor.HairColors[ActorHairColourNCombo.SelectedIndex] = ColourDialog.Color.ToArgb();
					ActorHairColourButton.BackColor = ColourDialog.Color;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void ActorVisualBubbleColour_Click(object sender, EventArgs e)
		{
			DialogResult D = ColourDialog.ShowDialog();
			if (D == DialogResult.OK)
			{
				ActorVisualBubbleColour.BackColor = ColourDialog.Color;
				SetActorsSavedStatus(false);
			}
		}

		private void ResetActorPreviewCameraButton_Click(object sender, EventArgs e)
		{
			ClearActorPreview();
			LoadActorPreview();
		}

		private void ActorPreviewFemale_CheckedChanged(object sender, EventArgs e)
		{
			ClearActorPreview();
			LoadActorPreview();
		}

		private void ActorSettingChanged(object sender, EventArgs e)
		{
			if (!SuppressActorSettingChange)
			{
				SetActorsSavedStatus(false);
			}
		}

		/* Backed up testanimation click function
		*  private void TestAnimationButton_Click(object sender, EventArgs e)
		*  {
		*      RenderingPanel.Visible = true;
		*      if (ActorPreviewAnimation == null)
		*      {
		*          MessageBox.Show("Incomplete profile", "Warning");
		*          return; // profile incomplete
		*      }
		*      if (AnimSetAnimsList.SelectedIndex < 0)
		*      {
		*          // not set animation
		*          MessageBox.Show("Select animation", "Warning");
		*          return;
		*      }
		*      // AnimSet A = new AnimSet();
		*      //Get current animsetlist *modification*
		*      ListBoxItem LBI = (ListBoxItem)AnimSetList.Items[AnimSetList.SelectedIndex];
		*      AnimSet A = AnimSet.Index[LBI.Value];
		*      //*modification*
		*      ((Entity)ActorPreviewAnimation.CollisionEN).Position(0f, 0f, 65f);
		*      string str = AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex].ToString();
		*      AnimSet.PlayAnimation(ActorPreviewAnimation, 1, (float)AnimSetSpeedSpinner.Value * 0.01F, A.GetAnimInt(str));
		*  }
		* */

		private void ResetAnimationCameraButton_Click(object sender, EventArgs e)
		{
			Camera.Rotate(0, 0, 0);
			Camera.Position(0, 0, 0);
			if (ActorPreviewAnimation != null)
			{
				((Entity) ActorPreviewAnimation.CollisionEN).Point(Camera);
				((Entity) ActorPreviewAnimation.CollisionEN).Turn(0, 180, 0);
			}
		}

		private void ActorAnimationListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ActorAnimationListBox.SelectedIndex < 0)
			{
				return; // index < 0 
			}
			//Reset camera
			Camera.Position(0, 0, 0);
			Camera.Rotate(0, 0, 0);

			// Remove error message
			NoActorSelectedLabel.Hide();
			this.Update();

			ClearActorPreviewAnimation();

			// Get selected actor
			int index = ActorAnimationListBox.SelectedIndex;
			ListBoxItem LBI = (ListBoxItem) ActorAnimationListBox.Items[index];
			SelectedActorAnimation = Actor.Index[LBI.Value];

			LoadActorPreviewAnimation(); //Fallback in case next command fails
			AnimSetAnimsList_SelectedIndexChanged(null, null); //Animate selection in animation list
		}

		private void MaleFemaleAnimationRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (!IgnoreAnimationGenderCheckChange)
			{
				ClearActorPreviewAnimation();
				LoadActorPreviewAnimation();
				AnimSetAnimsList_SelectedIndexChanged(null, null);
			}
		}

		// Settings (Marian Voicu)
		private void ActorSettingsUseRadarCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			SetActorsSavedStatus(false);
		}

		private void ActorSettingsSelecterImageButton_Click(object sender, EventArgs e)
		{
			ushort Result = MediaDialogs.GetTexture(false, ActorSettingsSelecterImageID);
			if (Result < 65535)
			{
				ActorSettingsSelecterImageButton.Text = NiceTextureName(Result);
				ActorSettingsSelecterImageID = Result;
				SetActorsSavedStatus(false);
			}
			else
			{
				ActorSettingsSelecterImageButton.Text = "No texture set";
				ActorSettingsSelecterImageID = 65535;
				SetActorsSavedStatus(false);
			}
		}

		private void ActprsSettingsDiscoverMapInComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetActorsSavedStatus(false);
		}

		private void ActprsSettingsShowActorsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			SetActorsSavedStatus(false);
		}

		private void ActprsSettingsEnemyButton_Click(object sender, EventArgs e)
		{
		}

		private void ActprsSettingsFriendlyButton_Click(object sender, EventArgs e)
		{
			DialogResult D = ColourDialog.ShowDialog();
			if (D == DialogResult.OK)
			{
				ActorsSettingsFriendlyButton.BackColor = ColourDialog.Color;
				SetActorsSavedStatus(false);
			}
		}

		private void ActprsSettingsPlayerButton_Click(object sender, EventArgs e)
		{
			DialogResult D = ColourDialog.ShowDialog();
			if (D == DialogResult.OK)
			{
				ActorsSettingsPlayerButton.BackColor = ColourDialog.Color;
				SetActorsSavedStatus(false);
			}
		}

		private void ActprsSettingsNPCButton_Click(object sender, EventArgs e)
		{
			DialogResult D = ColourDialog.ShowDialog();
			if (D == DialogResult.OK)
			{
				ActorsSettingsNPCButton.BackColor = ColourDialog.Color;
				SetActorsSavedStatus(false);
			}
		}

		private void ActprsSettingsOtherButton_Click(object sender, EventArgs e)
		{
			DialogResult D = ColourDialog.ShowDialog();
			if (D == DialogResult.OK)
			{
				ActorsSettingsOtherButton.BackColor = ColourDialog.Color;
				SetActorsSavedStatus(false);
			}
		}

		// end (MV)

		#endregion

		#region Actors tab functions
		public void SetSelectedActor(int Index)
		{
			if (Index < 0)
			{
				ClearActorPreview();
				return;
			}

			if (Index < ActorsList.Items.Count)
			{
				ActorsList.SelectedIndex = Index;
			}

			ActorsList.BeginUpdate();
			ActorsList.Enabled = false;

			ClearActorPreview();

			// No actor is selected, blank everything
			if (ActorsList.SelectedIndex < 0)
			{
				SelectedActor = null;
				ActorIDLabel.Text = "Actor ID: No actor selected";
				ActorRaceText.Text = "";
				ActorClassText.Text = "";
				ActorDescriptionText.Text = "";
				if (ActorFactionCombo.Items.Count > 0)
				{
					ActorFactionCombo.SelectedIndex = 0;
				}
				ActorAttributeSpinner.Value = 0;
				ActorAttributeMaxSpinner.Value = 0;
				ActorPlayableCheck.Checked = false;
				ActorRideableCheck.Checked = false;
				if (ActorStartZoneCombo.Items.Count > 0)
				{
					ActorStartZoneCombo.SelectedIndex = 0;
				}
				ActorStartPortalText.Text = "";
				ActorAggressionCombo.SelectedIndex = 0;
				ActorAggressionRangeSpinner.Value = 0;
				ActorXPSpinner.Value = 0;
				ActorTradeCombo.SelectedIndex = 0;
				ActorSlotDisableCheck.Checked = false;
				ActorEnvironmentCombo.SelectedIndex = 0;
				if (ActorMAnimSetCombo.Items.Count > 0)
				{
					ActorMAnimSetCombo.SelectedIndex = 0;
				}
				if (ActorFAnimSetCombo.Items.Count > 0)
				{
					ActorFAnimSetCombo.SelectedIndex = 0;
				}
				ActorMSoundButton.Text = "No sound set";
				ActorFSoundButton.Text = "No sound set";
				ActorMBodyButton.Text = "No mesh set";
				ActorFBodyButton.Text = "No mesh set";
				ActorMHairButton.Text = "No mesh set";
				ActorFHairButton.Text = "No mesh set";
				ActorBeardButton.Text = "No mesh set";
				ActorGubbinButton.Text = "No mesh set";
				ActorBloodButton.Text = "No texture set";
				ActorMFaceButton.Text = "No texture set";
				ActorFFaceButton.Text = "No texture set";
				ActorMBodyButton.Text = "No texture set";
				ActorFBodyButton.Text = "No texture set";
				ActorScaleSpinner.Value = 100;
				ActorSlotDisableCheck.Checked = false;
			}
				// An actor is selected
			else
			{
				// Get selected actor
				ListBoxItem LBI = (ListBoxItem) ActorsList.Items[Index];
				SelectedActor = Actor.Index[LBI.Value];

				// Description

				ActorIDLabel.Text = "Actor ID: " + SelectedActor.ID.ToString();
				ActorRaceText.Text = SelectedActor.Race;
				ActorClassText.Text = SelectedActor.Class;
				ActorDescriptionText.Text = SelectedActor.Description;
				ActorGendersCombo.SelectedIndex = SelectedActor.Genders;
				ActorFactionCombo.SelectedIndex =
					ActorFactionCombo.FindStringExact(Actor.FactionNames[SelectedActor.DefaultFaction]);

				// Attributes
				if (ActorAttributesList.SelectedIndex >= 0)
				{
					LBI = (ListBoxItem) ActorAttributesList.Items[ActorAttributesList.SelectedIndex];
					ActorAttributeSpinner.Value = SelectedActor.Attributes.Value[LBI.Value];
					ActorAttributeMaxSpinner.Value = SelectedActor.Attributes.Maximum[LBI.Value];
				}
				else
				{
					ActorAttributeSpinner.Value = 0;
					ActorAttributeMaxSpinner.Value = 0;
				}
				if (ActorResistancesList.SelectedIndex >= 0)
				{
					LBI = (ListBoxItem) ActorResistancesList.Items[ActorResistancesList.SelectedIndex];
					ActorResistanceSpinner.Value = SelectedActor.Resistances[LBI.Value];
				}
				else
				{
					ActorResistanceSpinner.Value = 0;
				}

				// Behaviour
				ActorPlayableCheck.Checked = SelectedActor.Playable;
				ActorRideableCheck.Checked = SelectedActor.Rideable;
				for (int i = 0; i < ActorStartZoneCombo.Items.Count; ++i)
				{
					if ((string) ActorStartZoneCombo.Items[i] == SelectedActor.StartArea)
					{
						ActorStartZoneCombo.SelectedIndex = i;
						break;
					}
				}
				ActorStartPortalText.Text = SelectedActor.StartPortal;
				ActorAggressionCombo.SelectedIndex = SelectedActor.Aggressiveness;
				ActorAggressionRangeSpinner.Value = (decimal) SelectedActor.AggressiveRange;
				ActorXPSpinner.Value = SelectedActor.XPMultiplier;
				ActorTradeCombo.SelectedIndex = SelectedActor.TradeMode;
				ActorSlotDisableCheck.Checked =
					!Actor.GetFlag(SelectedActor.InventorySlots, ActorSlotDisabledCombo.SelectedIndex);
				ActorEnvironmentCombo.SelectedIndex = (int) SelectedActor.EType;
				if (AnimSet.Index[SelectedActor.MaleAnimationSet] != null)
				{
					int Idx = ActorMAnimSetCombo.FindStringExact(AnimSet.Index[SelectedActor.MaleAnimationSet].Name);
					if (Idx >= 0)
					{
						ActorMAnimSetCombo.SelectedIndex = Idx;
					}
				}
				if (AnimSet.Index[SelectedActor.FemaleAnimationSet] != null)
				{
					int Idx = ActorFAnimSetCombo.FindStringExact(AnimSet.Index[SelectedActor.FemaleAnimationSet].Name);
					if (Idx >= 0)
					{
						ActorFAnimSetCombo.SelectedIndex = Idx;
					}
				}
				ActorMSoundButton.Text = NiceSoundName(SelectedActor.MaleSpeechIDs[ActorMSoundCombo.SelectedIndex]);
				ActorFSoundButton.Text = NiceSoundName(SelectedActor.FemaleSpeechIDs[ActorFSoundCombo.SelectedIndex]);

				// Appearance
				ActorMBodyButton.Text = NiceMeshName(SelectedActor.MaleMesh);
				ActorFBodyButton.Text = NiceMeshName(SelectedActor.FemaleMesh);
				ActorMHairButton.Text = "Click to Edit";
				ActorFHairButton.Text = "Click to Edit";
				ActorBeardButton.Text = "Click to Edit";
				ActorBloodButton.Text = NiceTextureName(SelectedActor.BloodTexID);
				decimal ScaleValue = (decimal) (SelectedActor.Scale * 100f);
				if (ScaleValue < 1)
				{
					ScaleValue = 1;
					SelectedActor.Scale = 0.01f;
				}
				ActorScaleSpinner.Value = ScaleValue;
				ActorHairColourButton.BackColor =
					Color.FromArgb(SelectedActor.HairColors[ActorHairColourNCombo.SelectedIndex]);

				// Preview
				if (ActorSubTabs.SelectedTab == ActorSubTabPreview)
				{
					LoadActorPreview();
				}
			}
			EnableActorGadgets();

			ActorsList.Enabled = true;
			ActorsList.EndUpdate();
			ActorsList.Focus();
		}

		private void EnableActorGadgets()
		{
			// Enable everything
			ActorMBodyButton.Enabled = true;
			ActorMBodyTexButton.Enabled = true;
			ActorMHairButton.Enabled = true;
			ActorMFaceButton.Enabled = true;
			ActorMAnimSetCombo.Enabled = true;
			ActorMSoundButton.Enabled = true;
			ActorFBodyButton.Enabled = true;
			ActorFBodyTexButton.Enabled = true;
			ActorFHairButton.Enabled = true;
			ActorFFaceButton.Enabled = true;
			ActorFAnimSetCombo.Enabled = true;
			ActorFSoundButton.Enabled = true;

			if (SelectedActor != null)
			{
				// Disable male gadgets
				if (SelectedActor.Genders == 2)
				{
					ActorMBodyButton.Enabled = false;
					ActorMBodyTexButton.Enabled = false;
					ActorMHairButton.Enabled = false;
					ActorMFaceButton.Enabled = false;
					ActorMAnimSetCombo.Enabled = false;
					ActorMSoundButton.Enabled = false;
				}

				// Disable female gadgets
				if (SelectedActor.Genders == 1 || SelectedActor.Genders == 3)
				{
					ActorFBodyButton.Enabled = false;
					ActorFBodyTexButton.Enabled = false;
					ActorFHairButton.Enabled = false;
					ActorFFaceButton.Enabled = false;
					ActorFAnimSetCombo.Enabled = false;
					ActorFSoundButton.Enabled = false;
				}
			}
		}

		public void SetActorsSavedStatus(bool Saved)
		{
			ActorsSaved = Saved;
			if (!Saved && ShowUnsavedIndicator)
			{
				TabActors.Text = "Actors *";
			}
			else
			{
				TabActors.Text = "Actors";
			}
		}

		private void ActorSaveGeneralSettings()
		{
			// Money
			BinaryWriter F = Blitz.WriteFile(@"Data\Game Data\Money.dat");
			if (F == null)
			{
				MessageBox.Show(@"Could not open Data\Game Data\Money.dat!");
				return;
			}
			Blitz.WriteString(MoneyUnits1.Text, F);
			Blitz.WriteString(MoneyUnits2.Text, F);
			F.Write((ushort) MoneyMultiplier2.Value);
			Blitz.WriteString(MoneyUnits3.Text, F);
			F.Write((ushort) MoneyMultiplier3.Value);
			Blitz.WriteString(MoneyUnits4.Text, F);
			F.Write((ushort) MoneyMultiplier4.Value);
			F.Close();

			// Other.dat (client)
			FileStream FStream = new FileStream(@"Data\Game Data\Other.dat", FileMode.Open,
												FileAccess.Write);
			F = new BinaryWriter(FStream);
			if (F == null)
			{
				MessageBox.Show(@"File not found: Data\Game Data\Other.dat!", "Error");
				return;
			}
			F.Write((byte) ActorVisualShowNametags.SelectedIndex);
			F.Write(ActorVisualCollisionsCheck.Checked);
			if (ActorVisualViewModes.SelectedIndex == 0)
			{
				F.Write((byte) 1);
			}
			else if (ActorVisualViewModes.SelectedIndex == 2)
			{
				F.Write((byte) 2);
			}
			else if (ActorVisualViewModes.SelectedIndex == 1)
			{
				F.Write((byte) 3);
			}
			F.BaseStream.Position += 4;
			F.Write(ActorRequireMemorisationCheck.Checked);
			F.Write((byte) (ActorVisualBubbles.SelectedIndex + 1));
			F.Write(ActorVisualBubbleColour.BackColor.R);
			F.Write(ActorVisualBubbleColour.BackColor.G);
			F.Write(ActorVisualBubbleColour.BackColor.B);

			// Radar
			F.Write(Convert.ToByte(ActorsSettingsShowActorsCheckBox.Checked));
			F.Write(Convert.ToUInt16(ActorsSettingsPlayerButton.Tag));
			F.Write(Convert.ToUInt16(ActorsSettingsEnemyButton.Tag));
			F.Write(Convert.ToUInt16(ActorsSettingsFriendlyButton.Tag));
			F.Write(Convert.ToUInt16(ActorsSettingsNPCButton.Tag));
			F.Write(Convert.ToUInt16(ActorsSettingsOtherButton.Tag));

			F.Close();

			// Misc.dat (server)
			FStream = new FileStream(@"Data\Server Data\Misc.dat", FileMode.Open,
									 FileAccess.Write);
			F = new BinaryWriter(FStream);
			if (F == null)
			{
				MessageBox.Show(@"File not found: Data\Server Data\Misc.dat!", "Error");
				return;
			}
			F.Write((int) ActorStartMoney.Value);
			F.Write((int) ActorStartReputation.Value);
			F.BaseStream.Position += 1;
			F.Write((ushort) ActorCombatDelaySpinner.Value);
			F.Write((byte) (ActorCombatFormula.SelectedIndex + 1));
			F.BaseStream.Position += 2;
			F.Write((byte) ActorCombatFactionHit.Value);
			F.BaseStream.Position += 2;
			F.Write(ActorRequireMemorisationCheck.Checked); 
			F.Close();

			// Combat.dat (client)
			F = Blitz.WriteFile(@"Data\Game Data\Combat.dat");
			F.Write((ushort) ActorCombatDelaySpinner.Value);
			F.Write((byte) (ActorCombatDamage.SelectedIndex + 1));
			F.Close();
		}

		private void ClearActorPreview()
		{
			if (ActorPreview != null)
			{
				// Do not clear until loading is complete
				while (LoadingActorPreview)
				{
					Thread.Sleep(0);
				}

				LoadingActorPreview = true;
				Actors3D.SafeFreeActorInstance(ActorPreview);
				ActorPreview = null;
				LoadingActorPreview = false;
			}
		}

		private void LoadActorPreview()
		{
			// Avoid loading more than one actor previews at a time
			//if (LoadingActorPreview)
			//    return;

			// Load actor model
			LoadingActorPreview = true;
			ActorPreview = new ActorInstance(SelectedActor);
			if (ActorPreviewFemale.Checked && SelectedActor.Genders == 0)
			{
				ActorPreview.Gender = 1;
			}
			bool Result = Actors3D.LoadActorInstance3D(ActorPreview, 0.5f / SelectedActor.Scale);
			if (Result == false)
			{
				ActorPreview.Dispose();
				ActorPreview = null;
				return;
			}
			if (ActorPreview.ShadowEN != null)
			{
				((Entity) ActorPreview.ShadowEN).Free();
				ActorPreview.ShadowEN = null;
			}
			Actors3D.ShowGubbins(ActorPreview);
			((Entity) ActorPreview.CollisionEN).Position(0f, 0f, 65f);
			AnimSet.PlayAnimation(ActorPreview, 1, 0.5f, (int) AnimSet.Anim.Idle);
			LoadingActorPreview = false;
		}

		private void ClearActorPreviewAnimation()
		{
			if (ActorPreviewAnimation != null)
			{
				// Do not clear until loading is complete
				while (LoadingActorPreviewAnimation)
				{
					Thread.Sleep(0);
				}

				LoadingActorPreviewAnimation = true;
				Actors3D.SafeFreeActorInstance(ActorPreviewAnimation);
				ActorPreviewAnimation = null;
				LoadingActorPreviewAnimation = false;
			}
		}

		private void LoadActorPreviewAnimation()
		{
			// Avoid loading more than one actor previews at a time
			//if (LoadingActorPreview)
			//    return;

			// Load actor model
			LoadingActorPreviewAnimation = true;
			ActorPreviewAnimation = new ActorInstance(SelectedActorAnimation);

			//Stop infinite loop caused by radiobutton changes.
			IgnoreAnimationGenderCheckChange = true;
			switch (SelectedActorAnimation.Genders)
				//0 for normal (male and female), 1 for male only, 2 for female only, 3 for no genders
			{
				case 0:
					MaleanimationRadioButton.Visible = true;
					FemaleAnimationRadioButton.Visible = true;
					break;
				case 1:
					MaleanimationRadioButton.Visible = true;
					MaleanimationRadioButton.Checked = true;
					FemaleAnimationRadioButton.Checked = false;
					FemaleAnimationRadioButton.Visible = false;

					break;
				case 2:
					MaleanimationRadioButton.Visible = false;
					MaleanimationRadioButton.Checked = false;
					FemaleAnimationRadioButton.Visible = true;
					FemaleAnimationRadioButton.Checked = true;

					break;
				case 3:
					MaleanimationRadioButton.Visible = false;
					FemaleAnimationRadioButton.Visible = false;
					break;
			}
			IgnoreAnimationGenderCheckChange = false;

			if (FemaleAnimationRadioButton.Checked && SelectedActorAnimation.Genders == 0)
			{
				ActorPreviewAnimation.Gender = 1;
			}
			else if (SelectedActorAnimation.Genders == 2)
			{
				ActorPreviewAnimation.Gender = 1;
			}
			else
			{
				ActorPreviewAnimation.Gender = 0;
			}

			//ActorPreviewAnimation.Gender = 1;
			bool Result = Actors3D.LoadActorInstance3D(ActorPreviewAnimation, 0.5f / SelectedActorAnimation.Scale);

			if (Result == false)
			{
				ActorPreviewAnimation.Dispose();
				ActorPreviewAnimation = null;
				return;
			}
			if (ActorPreviewAnimation.ShadowEN != null)
			{
				((Entity) ActorPreviewAnimation.ShadowEN).Free();
				ActorPreviewAnimation.ShadowEN = null;
			}
			Actors3D.ShowGubbins(ActorPreviewAnimation);
		   
			((Entity) ActorPreviewAnimation.CollisionEN).Position(0f, 0f, 70f);
			AnimSet.PlayAnimation(ActorPreviewAnimation, 1, 0, (int) AnimSet.Anim.Walk);
			LoadingActorPreviewAnimation = false;
		}
		#endregion



		#region Factions tab events
		private void NewFaction_Click(object sender, EventArgs e)
		{
			bool Found = false;
			for (int i = 0; i < Actor.TotalFactions; ++i)
			{
				if (string.IsNullOrEmpty(Actor.FactionNames[i]))
				{
					Actor.FactionNames[i] = "New faction";
					for (int j = 0; j < Actor.TotalFactions; ++j)
					{
						Actor.FactionDefaultRatings[i, j] = 0;
						Actor.FactionDefaultRatings[j, i] = 0;
					}
					SetActorsSavedStatus(false);
					Found = true;
					break;
				}
			}
			// No free faction ID found
			if (!Found)
			{
				MessageBox.Show("Limit of 100 factions reached!", "Error");
			}
			LoadValueFromFactionsDataGridView();
			FactionsDataGridViewSelectRows = FactionsDataGridView.RowCount - 1;
			FactionNameText.Text = FactionsDataGridView.Rows[FactionsDataGridViewSelectRows].HeaderCell.Value.ToString();
			FactionsDataGridView.Rows[FactionsDataGridViewSelectRows].Cells[FactionsDataGridViewSelectRows].Selected =
				true;
			if (FactionsDataGridViewSelectRows > 0)
			{
				FactionsDataGridView.Rows[0].Cells[0].Selected = false;
			}

			ActorFactionCombo.Items.Clear();
			ActorFactionCombo.BeginUpdate();
			for (int i = 0; i < Actor.TotalFactions; ++i)
			{
				if (Actor.FactionNames[i] != "")
				{
                    ActorFactionCombo.Items.Add(new ListBoxItem(Actor.FactionNames[i], (uint)i));
				}
			}
			ActorFactionCombo.EndUpdate();
			SetSelectedActor(ActorsList.SelectedIndex);
			// Animation sets
		}

		private void DeleteFaction_Click(object sender, EventArgs e)
		{
			if (FactionsDataGridViewSelectRows < 0)
			{
				MessageBox.Show("Line selected from delete not existing!", "Error");
				return;
			}
			for (int i = FactionsDataGridViewSelectRows; i < Actor.TotalFactions - 1; i++)
			{
				Actor.FactionNames[i] = Actor.FactionNames[i + 1];
			}
			Actor.FactionNames[Actor.TotalFactions - 1] = "";
			//FactionsDataGridView.Rows.RemoveAt(FactionsDataGridViewSelectRows);
			//FactionsDataGridView.Columns.RemoveAt(FactionsDataGridViewSelectRows);
			LoadValueFromFactionsDataGridView();
			SetActorsSavedStatus(false);
			if (FactionsDataGridView.RowCount <= 0)
			{
				FactionsDataGridViewSelectRows = -1;
			}
			else
			{
				FactionNameText.Text = FactionsDataGridView.Rows[0].HeaderCell.Value.ToString();
				FactionsDataGridView.Rows[0].Cells[0].Selected = true;
				//FactionsDataGridView.Rows[FactionsDataGridViewSelectRows].Cells[FactionsDataGridViewSelectRows] .Selected = false;
				FactionsDataGridViewSelectRows = 0;
			}
			ActorFactionCombo.Items.Clear();
			ActorFactionCombo.BeginUpdate();
			for (int i = 0; i < Actor.TotalFactions; ++i)
			{
				if (Actor.FactionNames[i] != "")
				{
                    ActorFactionCombo.Items.Add(new ListBoxItem(Actor.FactionNames[i], (uint)i));
				}
			}
			ActorFactionCombo.EndUpdate();
			SetSelectedActor(ActorsList.SelectedIndex);
		}

		private void RenameFactionButton_Click(object sender, EventArgs e)
		{
			//Hack to stop crash 
			if (FactionsDataGridViewSelectRows == -1)
				return;
			Actor.FactionNames[FactionsDataGridViewSelectRows] = FactionNameText.Text;
			FactionsDataGridView.Rows[FactionsDataGridViewSelectRows].HeaderCell.Value = FactionNameText.Text;
			FactionsDataGridView.Columns[FactionsDataGridViewSelectRows].HeaderText = FactionNameText.Text;
			// LoadValueFromFactionsDataGridView();
			FactionsDataGridView.Rows[FactionsDataGridViewSelectRows].Cells[FactionsDataGridViewSelectRows].Selected =
				true;
			if (FactionsDataGridViewSelectRows > 0)
			{
				FactionsDataGridView.Rows[0].Cells[0].Selected = false;
			}
			ActorFactionCombo.Items.Clear();
			ActorFactionCombo.BeginUpdate();
			for (int i = 0; i < Actor.TotalFactions; ++i)
			{
				if (Actor.FactionNames[i] != "")
				{
                    ActorFactionCombo.Items.Add(new ListBoxItem(Actor.FactionNames[i], (uint)i));
				}
			}
			ActorFactionCombo.EndUpdate();
			SetSelectedActor(ActorsList.SelectedIndex);
		}

		private void FactionsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			int Value = 0;
			try
			{
				Value = Convert.ToInt32(FactionsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
			}
			catch
			{
				Value = 0;
			}
			

			if (Value < -100)
			{
				Value = -100;
				FactionsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = -100;
			}
			else if (Value > 155)
			{
				Value = 155;
				FactionsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 155;
			}

			Actor.FactionDefaultRatings[e.ColumnIndex, e.RowIndex] = (byte)(Value + 100);
			Actor.FactionDefaultRatings[e.RowIndex, e.ColumnIndex] = (byte)(Value + 100);
			FactionsDataGridView.Rows[e.ColumnIndex].Cells[e.RowIndex].Value = Value;

//             if (Convert.ToInt32(FactionsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) < 100
//                 || Convert.ToInt32(FactionsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) > 155)
//             {
//                 // limit by zero
//                 FactionsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
//             }
//             byte temp = (byte) (Convert.ToInt32(FactionsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) + 100);
//             Actor.FactionDefaultRatings[e.ColumnIndex, e.RowIndex] = temp;
//             Actor.FactionDefaultRatings[e.RowIndex, e.ColumnIndex] = temp;
//             FactionsDataGridView.Rows[e.ColumnIndex].Cells[e.RowIndex].Value = temp - 100;
		}

		private void FactionsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				FactionNameText.Text = "";
				return;
			}
			FactionsDataGridViewSelectRows = e.RowIndex;
			string str = FactionsDataGridView.Rows[FactionsDataGridViewSelectRows].HeaderCell.Value.ToString();
			FactionNameText.Text = str;
		}
		#endregion

		#region Animation sets tab events
		private void AnimSetList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SuppressAnimSetChange && AnimSetList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSetNameText.Text = AnimSet.Index[LBI.Value].Name;
				AnimSetAnimsList.Items.Clear();
				AnimSetAnimsList.BeginUpdate();
				for (int i = 149; i >= 0; --i)
				{
					if (AnimSet.Index[LBI.Value].AnimName[i] != "")
					{
                        AnimSetAnimsList.Items.Add(new ListBoxItem(AnimSet.Index[LBI.Value].AnimName[i], (uint)i));
					}
				}
				AnimSetAnimsList.EndUpdate();
				if (AnimSetAnimsList.Items.Count > 0)
				{
					AnimSetAnimsList.SelectedIndex = 0;
				}
			}
		}

		private void AnimSetNew_Click(object sender, EventArgs e)
		{
			try
			{
				AnimSet A = new AnimSet();
                AnimSetList.SelectedIndex = AnimSetList.Items.Add(new ListBoxItem(A.Name, (uint)A.ID));
                ActorMAnimSetCombo.Items.Add(new ListBoxItem(A.Name, (uint)A.ID));
                ActorFAnimSetCombo.Items.Add(new ListBoxItem(A.Name, (uint)A.ID));
				SetActorsSavedStatus(false);
			}
			catch (AnimSetException)
			{
				MessageBox.Show(
					"Limit of " + AnimSet.TotalAnimationSets.ToString() + " animation sets already reached!", "Error");
			}
		}

		private void AnimSetDuplicate_Click(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];

				try
				{
					AnimSet A = new AnimSet();
					A.Name = AnimSet.Index[LBI.Value].Name + " (copy)";
					for (int i = 0; i < 150; ++i)
					{
						A.AnimName[i] = AnimSet.Index[LBI.Value].AnimName[i];
						A.AnimStart[i] = AnimSet.Index[LBI.Value].AnimStart[i];
						A.AnimEnd[i] = AnimSet.Index[LBI.Value].AnimEnd[i];
						A.AnimSpeed[i] = AnimSet.Index[LBI.Value].AnimSpeed[i];
					}
                    AnimSetList.SelectedIndex = AnimSetList.Items.Add(new ListBoxItem(A.Name, (uint)A.ID));
                    ActorMAnimSetCombo.Items.Add(new ListBoxItem(A.Name, (uint)A.ID));
                    ActorFAnimSetCombo.Items.Add(new ListBoxItem(A.Name, (uint)A.ID));
					SetActorsSavedStatus(false);
				}
				catch (AnimSetException)
				{
					MessageBox.Show(
						"Limit of " + AnimSet.TotalAnimationSets.ToString() + " animation sets already reached!",
						"Error");
				}
			}
		}

		private void AnimSetDelete_Click(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];

				// Remove from actor tab lists
				int Idx = ActorMAnimSetCombo.FindStringExact(AnimSet.Index[LBI.Value].Name);
				ActorMAnimSetCombo.Items.RemoveAt(Idx);
				Idx = ActorFAnimSetCombo.FindStringExact(AnimSet.Index[LBI.Value].Name);
				ActorFAnimSetCombo.Items.RemoveAt(Idx);

				// Delete animation set
				AnimSet.Index[LBI.Value].Delete();

				// Remove from main list
				SuppressAnimSetChange = true;
				AnimSetList.Items.RemoveAt(AnimSetList.SelectedIndex);
				SuppressAnimSetChange = false;

				SetActorsSavedStatus(false);
			}
		}

		private void AnimSetNameText_TextChanged(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];

				if (AnimSet.Index[LBI.Value].Name != AnimSetNameText.Text)
				{
					// Update the actor tab lists
					int Idx = ActorMAnimSetCombo.FindStringExact(AnimSet.Index[LBI.Value].Name);
					bool Selected = (Idx == ActorMAnimSetCombo.SelectedIndex);
					ActorMAnimSetCombo.Items.RemoveAt(Idx);
					Idx = ActorMAnimSetCombo.Items.Add(new ListBoxItem(AnimSetNameText.Text, LBI.Value));
					if (Selected)
					{
						ActorMAnimSetCombo.SelectedIndex = Idx;
					}

					Idx = ActorFAnimSetCombo.FindStringExact(AnimSet.Index[LBI.Value].Name);
					Selected = (Idx == ActorFAnimSetCombo.SelectedIndex);
					ActorFAnimSetCombo.Items.RemoveAt(Idx);
					Idx = ActorFAnimSetCombo.Items.Add(new ListBoxItem(AnimSetNameText.Text, LBI.Value));
					if (Selected)
					{
						ActorFAnimSetCombo.SelectedIndex = Idx;
					}

					// Update animation set
					AnimSet.Index[LBI.Value].Name = AnimSetNameText.Text;
					SetActorsSavedStatus(false);

					// Update the main list
					SuppressAnimSetChange = true;
					Idx = AnimSetList.SelectedIndex;
					LBI = new ListBoxItem(AnimSetNameText.Text, LBI.Value);
					AnimSetList.Items.RemoveAt(Idx);
					Idx = AnimSetList.Items.Add(LBI);
					AnimSetList.SelectedIndex = Idx;
					SuppressAnimSetChange = false;
				}
			}
		}

		private void AnimSetAnimsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SuppressAnimSetAnimChange && AnimSetList.SelectedIndex >= 0 && AnimSetAnimsList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSet A = AnimSet.Index[LBI.Value];
				LBI = (ListBoxItem) AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex];
				AnimSetAnimNameText.Text = A.AnimName[LBI.Value];
				AnimSetStartFrameSpinner.Value = A.AnimStart[LBI.Value];
				AnimSetEndFrameSpinner.Value = A.AnimEnd[LBI.Value];
				AnimSetSpeedSpinner.Value = (decimal) (A.AnimSpeed[LBI.Value] * 100f);

				if (ActorPreviewAnimation == null)
				{
					NoActorSelectedLabel.Show();

					InvalidFrameLabel.Hide();
					return; // profile incomplete
				}

				if (AnimSetAnimsList.SelectedIndex < 0)
				{
					// not set animation
					InvalidFrameLabel.Hide();
					return;
				}
				if (A.AnimEnd[LBI.Value] == 0 || A.AnimStart[LBI.Value] > A.AnimEnd[LBI.Value])
				{
					InvalidFrameLabel.Show();
					return;
				}
				RenderingPanel.Visible = true;
				InvalidFrameLabel.Hide();
				((Entity) ActorPreviewAnimation.CollisionEN).Position(0f, 0f, 70f);
				string str = AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex].ToString();
				AnimSet.PlayAnimation(ActorPreviewAnimation, 1, (float) AnimSetSpeedSpinner.Value * 0.01F,
									  A.GetAnimInt(str));
			}
		}

		private void AnimSetAnimAdd_Click(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSet A = AnimSet.Index[LBI.Value];

				// Find first free animation
				bool Found = false;
				for (int i = 149; i >= 0; --i)
				{
					if (A.AnimName[i] == "")
					{
						A.AnimName[i] = "New animation";
						A.AnimStart[i] = 0;
						A.AnimEnd[i] = 0;
						A.AnimSpeed[i] = 1f;
						SetActorsSavedStatus(false);

                        int Idx = AnimSetAnimsList.Items.Add(new ListBoxItem(A.AnimName[i], (uint)i));
						AnimSetAnimsList.SelectedIndex = Idx;

						Found = true;
						break;
					}
				}

				// No free animation available
				if (!Found)
				{
					MessageBox.Show("Limit of 150 animations per set already reached!", "Error");
				}
			}
		}

		private void AnimSetAnimRemove_Click(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0 && AnimSetAnimsList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSet A = AnimSet.Index[LBI.Value];
				LBI = (ListBoxItem) AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex];

				A.AnimName[LBI.Value] = "";
				A.AnimStart[LBI.Value] = 0;
				A.AnimEnd[LBI.Value] = 0;
				A.AnimSpeed[LBI.Value] = 1f;
				SetActorsSavedStatus(false);

				AnimSetAnimsList.Items.RemoveAt(AnimSetAnimsList.SelectedIndex);
				if (AnimSetAnimsList.Items.Count > 0)
				{
					AnimSetAnimsList.SelectedIndex = 0;
				}
			}
		}

		private void AnimSetAnimNameText_TextChanged(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0 && AnimSetAnimsList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSet A = AnimSet.Index[LBI.Value];
				LBI = (ListBoxItem) AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex];

				if (A.AnimName[LBI.Value] != AnimSetAnimNameText.Text)
				{
					A.AnimName[LBI.Value] = AnimSetAnimNameText.Text;
					SetActorsSavedStatus(false);

					// Update the main anim sets list
					SuppressAnimSetAnimChange = true;
					int Idx = AnimSetAnimsList.SelectedIndex;
					LBI = new ListBoxItem(AnimSetAnimNameText.Text, LBI.Value);
					AnimSetAnimsList.Items.RemoveAt(Idx);
					AnimSetAnimsList.Items.Insert(Idx, LBI);
					AnimSetAnimsList.SelectedIndex = Idx;
					SuppressAnimSetAnimChange = false;
				}
			}
		}

		private void AnimSetStartFrameSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0 && AnimSetAnimsList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSet A = AnimSet.Index[LBI.Value];
				LBI = (ListBoxItem) AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex];

				if (A.AnimStart[LBI.Value] != AnimSetStartFrameSpinner.Value)
				{
					A.AnimStart[LBI.Value] = (ushort) AnimSetStartFrameSpinner.Value;
					if (ActorPreviewAnimation != null)
					{
						string str = AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex].ToString();
						AnimSet.UpdatePreviewFrames(ActorPreviewAnimation, A, A.GetAnimInt(str));
						AnimSet.PlayAnimation(ActorPreviewAnimation, 1, (float)AnimSetSpeedSpinner.Value * 0.01F,
											  A.GetAnimInt(str));
					}
					SetActorsSavedStatus(false);
				}
			}
		}

		private void AnimSetEndFrameSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0 && AnimSetAnimsList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSet A = AnimSet.Index[LBI.Value];
				LBI = (ListBoxItem) AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex];

				if (A.AnimEnd[LBI.Value] != AnimSetEndFrameSpinner.Value)
				{
					A.AnimEnd[LBI.Value] = (ushort) AnimSetEndFrameSpinner.Value;
					if (ActorPreviewAnimation != null)
					{
						string str = AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex].ToString();
						AnimSet.UpdatePreviewFrames(ActorPreviewAnimation, A, A.GetAnimInt(str));
						AnimSet.PlayAnimation(ActorPreviewAnimation, 1, (float)AnimSetSpeedSpinner.Value * 0.01F,
											  A.GetAnimInt(str));
					}
					SetActorsSavedStatus(false);
				}
			}
		}

		private void AnimSetSpeedSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (AnimSetList.SelectedIndex >= 0 && AnimSetAnimsList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AnimSetList.Items[AnimSetList.SelectedIndex];
				AnimSet A = AnimSet.Index[LBI.Value];
				LBI = (ListBoxItem) AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex];

				float NewValue = (float) AnimSetSpeedSpinner.Value / 100f;
				if (Math.Abs(A.AnimSpeed[LBI.Value] - NewValue) > 0.0001f)
				{
					A.AnimSpeed[LBI.Value] = NewValue;
					if (ActorPreviewAnimation != null)
					{
						string str = AnimSetAnimsList.Items[AnimSetAnimsList.SelectedIndex].ToString();
						AnimSet.UpdatePreviewFrames(ActorPreviewAnimation, A, A.GetAnimInt(str));
						AnimSet.PlayAnimation(ActorPreviewAnimation, 1, (float)AnimSetSpeedSpinner.Value * 0.01F,
											  A.GetAnimInt(str));
					}
					SetActorsSavedStatus(false);
				}
			}
		}
		#endregion

		#region Abilities tab events
		private void AbilitiesList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SuppressAbilityChange && AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];
				AbilityNameText.Text = Ability.Index[LBI.Value].Name;
				AbilityDescriptionText.Text = Ability.Index[LBI.Value].Description;
				AbilityThumbnailButton.Text = NiceTextureName(Ability.Index[LBI.Value].ThumbnailTexID);
				AbilityRechargeSpinner.Value = Ability.Index[LBI.Value].RechargeTime / 1000;
				if (Ability.Index[LBI.Value].ExclusiveRace != "")
				{
					AbilityExclusiveRaceCombo.SelectedIndex =
						AbilityExclusiveRaceCombo.FindStringExact(Ability.Index[LBI.Value].ExclusiveRace);
				}
				else
				{
					AbilityExclusiveRaceCombo.SelectedIndex = 0;
				}
				if (Ability.Index[LBI.Value].ExclusiveClass != "")
				{
					AbilityExclusiveClassCombo.SelectedIndex =
						AbilityExclusiveClassCombo.FindStringExact(Ability.Index[LBI.Value].ExclusiveClass);
				}
				else
				{
					AbilityExclusiveClassCombo.SelectedIndex = 0;
				}
				if (Ability.Index[LBI.Value].Script != "")
				{
					AbilityScriptCombo.SelectedIndex =
						AbilityScriptCombo.FindStringExact(Ability.Index[LBI.Value].Script);
				}
				else
				{
					AbilityScriptCombo.SelectedIndex = 0;
				}
				UpdateAbilityScriptFunction();
			}
		}

		private void AbilityNew_Click(object sender, EventArgs e)
		{
			try
			{
				Ability A = new Ability();
                Ability.Index.Add(A.ID, A);
				AbilitiesList.SelectedIndex = AbilitiesList.Items.Add(new ListBoxItem(A.Name, A.ID));
				SetActorsSavedStatus(false);
			}
			catch (AbilityException)
			{
				MessageBox.Show("Limit of 65535 abilities already reached!", "Error");
			}
		}

		private void AbilityDelete_Click(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				// Delete ability
                Ability.Index.Remove(LBI.Value);
				Ability.Index[LBI.Value].Delete();

				// Remove from main list
				SuppressAbilityChange = true;
				AbilitiesList.Items.RemoveAt(AbilitiesList.SelectedIndex);
				SuppressAbilityChange = false;

				SetActorsSavedStatus(false);
			}
		}

		private void AbilityNameText_TextChanged(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				if (Ability.Index[LBI.Value].Name != AbilityNameText.Text)
				{
					Ability.Index[LBI.Value].Name = AbilityNameText.Text;
					SetActorsSavedStatus(false);
				}

				// Update the abilities list
				SuppressAbilityChange = true;
				int Idx = AbilitiesList.SelectedIndex;
				LBI = new ListBoxItem(Ability.Index[LBI.Value].Name, LBI.Value);
				AbilitiesList.Items.RemoveAt(Idx);
				Idx = AbilitiesList.Items.Add(LBI);
				AbilitiesList.SelectedIndex = Idx;
				SuppressAbilityChange = false;
			}
		}

		private void AbilityDescriptionText_TextChanged(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				if (Ability.Index[LBI.Value].Description != AbilityDescriptionText.Text)
				{
					Ability.Index[LBI.Value].Description = AbilityDescriptionText.Text;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void AbilityThumbnailButton_Click(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ushort TToUse = 65535;
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];
				if (LBI != null)
				{
					TToUse = Ability.Index[LBI.Value].ThumbnailTexID;
				}

				ushort Result = MediaDialogs.GetTexture(false, "", TToUse);
				if (Result < 65535)
				{
					LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

					Ability.Index[LBI.Value].ThumbnailTexID = Result;
					SetActorsSavedStatus(false);
					AbilityThumbnailButton.Text = NiceTextureName(Ability.Index[LBI.Value].ThumbnailTexID);
				}
				else
				{
					AbilityThumbnailButton.Text = "No texture set";
				}
			}
		}

		private void AbilityRechargeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				if (Ability.Index[LBI.Value].RechargeTime / 1000 != AbilityRechargeSpinner.Value)
				{
					Ability.Index[LBI.Value].RechargeTime = (int) AbilityRechargeSpinner.Value * 1000;
					SetActorsSavedStatus(false);
				}
			}
		}

		private void AbilityExclusiveRaceCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				if (AbilityExclusiveRaceCombo.SelectedIndex > 0)
				{
					if (Ability.Index[LBI.Value].ExclusiveRace != AbilityExclusiveRaceCombo.SelectedItem.ToString())
					{
						Ability.Index[LBI.Value].ExclusiveRace = AbilityExclusiveRaceCombo.SelectedItem.ToString();
						SetActorsSavedStatus(false);
					}
				}
				else if (Ability.Index[LBI.Value].ExclusiveRace != "")
				{
					Ability.Index[LBI.Value].ExclusiveRace = "";
					SetActorsSavedStatus(false);
				}
			}
		}

		private void AbilityExclusiveClassCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				if (AbilityExclusiveClassCombo.SelectedIndex > 0)
				{
					if (Ability.Index[LBI.Value].ExclusiveClass != AbilityExclusiveClassCombo.SelectedItem.ToString())
					{
						Ability.Index[LBI.Value].ExclusiveClass = AbilityExclusiveClassCombo.SelectedItem.ToString();
						SetActorsSavedStatus(false);
					}
				}
				else if (Ability.Index[LBI.Value].ExclusiveClass != "")
				{
					Ability.Index[LBI.Value].ExclusiveClass = "";
					SetActorsSavedStatus(false);
				}
			}
		}

		private void AbilityScriptCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Clear list of script functions
			AbilityMethodCombo.Items.Clear();
			AbilityMethodCombo.Enabled = false;
			// If an ability is selected
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				// Change script
				if (AbilityScriptCombo.SelectedIndex > 0 &&
					Ability.Index[LBI.Value].Script != AbilityScriptCombo.SelectedItem.ToString())
				{
					Ability.Index[LBI.Value].Script = AbilityScriptCombo.SelectedItem.ToString();
					SetActorsSavedStatus(false);
				}
				else if (AbilityScriptCombo.SelectedIndex == 0 && Ability.Index[LBI.Value].Script != "")
				{
					Ability.Index[LBI.Value].Script = "";
					SetActorsSavedStatus(false);
				}
				// Update list of available script functions
				ListBoxItem[] LBIs = GetScriptFunctionsList(Ability.Index[LBI.Value].Script);
				if (LBIs != null)
				{
					AbilityMethodCombo.Items.AddRange(LBIs);
					AbilityMethodCombo.Enabled = true;

					UpdateAbilityScriptFunction();
				}
			}
		}

		private void AbilityMethodCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				if (Ability.Index[LBI.Value].Method != AbilityMethodCombo.SelectedItem.ToString())
				{
					Ability.Index[LBI.Value].Method = AbilityMethodCombo.SelectedItem.ToString();
					SetActorsSavedStatus(false);
				}
			}
		}
		#endregion

		#region Abilities tab functions
		private void UpdateAbilityScriptFunction()
		{
			if (AbilitiesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) AbilitiesList.Items[AbilitiesList.SelectedIndex];

				if (AbilityMethodCombo.Items.Count > 0)
				{
					int Idx = AbilityMethodCombo.FindString(Ability.Index[LBI.Value].Method);
					if (Idx > 0)
					{
						AbilityMethodCombo.SelectedIndex = Idx;
					}
					else
					{
						AbilityMethodCombo.SelectedIndex = 0;
						if (Ability.Index[LBI.Value].Method != AbilityMethodCombo.SelectedItem.ToString())
						{
							Ability.Index[LBI.Value].Method = AbilityMethodCombo.SelectedItem.ToString();
						}
					}
				}
				else
				{
					Ability.Index[LBI.Value].Method = "";
				}
			}
		}
		#endregion



		#region Items tab events
		private void ItemNewButton_Click(object sender, EventArgs e)
		{
			try
			{
				SelectedItem = new Item();
				TotalItems++;
				SelectedItem.Name = "New item";
                int Idx = ItemsList.Items.Add(new ListBoxItem(SelectedItem.Name, SelectedItem.ID));
                Item.Index.Add(SelectedItem.ID, SelectedItem);
				SetSelectedItem(Idx);
				SetItemsSavedStatus(false);
                
			}
			catch (ItemException)
			{
				MessageBox.Show("Limit of 65535 items already reached!", "Error");
			}
		}

		private void ItemDuplicateButton_Click(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				Item NewItem = new Item();
                Item.Index.Add(NewItem.ID, NewItem);
				TotalItems++;
				NewItem.Name = SelectedItem.Name + " (copy)";
				NewItem.ExclusiveClass = SelectedItem.ExclusiveClass;
				NewItem.ExclusiveRace = SelectedItem.ExclusiveRace;
				NewItem.Script = SelectedItem.Script;
				NewItem.Method = SelectedItem.Method;
				NewItem.IType = SelectedItem.IType;
				NewItem.Value = SelectedItem.Value;
				NewItem.Mass = SelectedItem.Mass;
				NewItem.Stackable = SelectedItem.Stackable;
				NewItem.ThumbnailTexID = SelectedItem.ThumbnailTexID;

				NewItem.GubbinTemplates.Clear();
				NewItem.GubbinTemplates.AddRange(SelectedItem.GubbinTemplates.ToArray());

				NewItem.TakesDamage = SelectedItem.TakesDamage;
				NewItem.SlotType = SelectedItem.SlotType;
				NewItem.WeaponDamage = SelectedItem.WeaponDamage;
				NewItem.WeaponDamageType = SelectedItem.WeaponDamageType;
				NewItem.WType = SelectedItem.WType;
				NewItem.RangedProjectile = SelectedItem.RangedProjectile;
				NewItem.RangedAnimation = SelectedItem.RangedAnimation;
				NewItem.Range = SelectedItem.Range;
				NewItem.ArmourLevel = SelectedItem.ArmourLevel;
				NewItem.EatEffectsLength = SelectedItem.EatEffectsLength;
				NewItem.ImageID = SelectedItem.ImageID;
				NewItem.MiscData = SelectedItem.MiscData;
//                 for (int i = 0; i < 6; ++i)
//                 {
//                     NewItem.Gubbins[i] = SelectedItem.Gubbins[i];
//                 }
				for (int i = 0; i < Attributes.TotalAttributes; ++i)
				{
					NewItem.Attributes.Value[i] = SelectedItem.Attributes.Value[i];
				}
				int Idx = ItemsList.Items.Add(new ListBoxItem(NewItem.Name, NewItem.ID));
				SetSelectedItem(Idx);
				SetItemsSavedStatus(false);
			}
		}

		private void ItemDeleteButton_Click(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				SuppressItemChange = true;
                Item.Index.Remove((uint)(ItemsList.Items[ItemsList.SelectedIndex] as ListBoxItem).Value);
				ItemsList.Items.RemoveAt(ItemsList.SelectedIndex);
                
				SuppressItemChange = false;
                Item.Index.Remove(SelectedActor.ID);
				SelectedItem.Delete();
				SelectedItem = null;
				SetSelectedItem(0);
				SetItemsSavedStatus(false);
			}
		}

		private void ItemWeaponDamageCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (Item.WeaponDamageEnabled != ItemWeaponDamageCheck.Checked)
			{
				Item.WeaponDamageEnabled = ItemWeaponDamageCheck.Checked;
				SetItemsSavedStatus(false);
			}
		}

		private void ItemArmourDamageCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (Item.ArmourDamageEnabled != ItemArmourDamageCheck.Checked)
			{
				Item.ArmourDamageEnabled = ItemArmourDamageCheck.Checked;
				SetItemsSavedStatus(false);
			}
		}

		private void ItemsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SuppressItemChange)
			{
				SetSelectedItem(ItemsList.SelectedIndex);
			}
		}

		private void ItemTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.IType != (Item.ItemType) (ItemTypeCombo.SelectedIndex + 1))
				{
					SelectedItem.IType = (Item.ItemType) (ItemTypeCombo.SelectedIndex + 1);
					SetItemsSavedStatus(false);
				}
			}
			UpdateItemInventorySlotList();
			UpdateItemSpecificGadgets();
		}

		private void ItemInventorySlotCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				switch (SelectedItem.IType)
				{
					case Item.ItemType.Armour:
						if (SelectedItem.SlotType != (Inventory.SlotType) (ItemInventorySlotCombo.SelectedIndex + 2))
						{
							SelectedItem.SlotType = (Inventory.SlotType) (ItemInventorySlotCombo.SelectedIndex + 2);
							SetItemsSavedStatus(false);
						}
						break;
					case Item.ItemType.Ring:
						if (SelectedItem.SlotType != (Inventory.SlotType) (ItemInventorySlotCombo.SelectedIndex + 9))
						{
							SelectedItem.SlotType = (Inventory.SlotType) (ItemInventorySlotCombo.SelectedIndex + 9);
							SetItemsSavedStatus(false);
						}
						break;
				}
			}
		}

		private void ItemAttributesList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ItemAttributesList.SelectedIndex >= 0 && SelectedItem != null)
			{
				ItemAttributeValueSpinner.Value = SelectedItem.Attributes.Value[ItemAttributesList.SelectedIndex];
			}
			else
			{
				ItemAttributeValueSpinner.Value = 0;
			}
		}

		private void ItemUseScriptCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Clear list of script functions
			ItemUseFunctionCombo.Items.Clear();
			ItemUseFunctionCombo.Enabled = false;
			// If an item is selected
			if (SelectedItem != null)
			{
				// Change script
				if (ItemUseScriptCombo.SelectedIndex > 0 &&
					SelectedItem.Script != ItemUseScriptCombo.SelectedItem.ToString())
				{
					SelectedItem.Script = ItemUseScriptCombo.SelectedItem.ToString();
					SetItemsSavedStatus(false);
				}
				else if (ItemUseScriptCombo.SelectedIndex == 0 && SelectedItem.Script != "")
				{
					SelectedItem.Script = "";
					SetItemsSavedStatus(false);
				}
				// Update list of available script functions
				ListBoxItem[] LBI = GetScriptFunctionsList(SelectedItem.Script);
				if (LBI != null)
				{
					ItemUseFunctionCombo.Items.AddRange(LBI);
					ItemUseFunctionCombo.Enabled = true;
					UpdateItemScriptFunction();
				}
			}
		}

		private void ItemUseFunctionCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.Method != ItemUseFunctionCombo.SelectedItem.ToString())
				{
					SelectedItem.Method = ItemUseFunctionCombo.SelectedItem.ToString();
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemDamageCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.TakesDamage != ItemDamageCheckbox.Checked)
				{
					SelectedItem.TakesDamage = ItemDamageCheckbox.Checked;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemStackCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.Stackable != ItemStackCheckbox.Checked)
				{
					SelectedItem.Stackable = ItemStackCheckbox.Checked;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemMassSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.Mass != (int) ItemMassSpinner.Value)
				{
					SelectedItem.Mass = (int) ItemMassSpinner.Value;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemValueSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.Value != (int) ItemValueSpinner.Value)
				{
					SelectedItem.Value = (int) ItemValueSpinner.Value;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemNameBox_TextChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.Name != ItemNameBox.Text)
				{
					SelectedItem.Name = ItemNameBox.Text;
					SetItemsSavedStatus(false);
				}
				// Update the items listbox
				ItemsList.BeginUpdate();
				SuppressItemChange = true;
				int Idx = ItemsList.SelectedIndex;
				ListBoxItem LBI = (ListBoxItem) ItemsList.SelectedItem;
				LBI = new ListBoxItem(SelectedItem.Name, LBI.Value);
				ItemsList.Items.RemoveAt(Idx);
				Idx = ItemsList.Items.Add(LBI);
				ItemsList.SelectedIndex = Idx;
				SuppressItemChange = false;
				ItemsList.EndUpdate();
			}
		}

		private void ItemAttributeValueSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null && ItemAttributesList.SelectedIndex >= 0)
			{
				ListBoxItem LBI = (ListBoxItem) ItemAttributesList.Items[ItemAttributesList.SelectedIndex];
				if (SelectedItem.Attributes.Value[LBI.Value] != (int) ItemAttributeValueSpinner.Value)
				{
					SelectedItem.Attributes.Value[LBI.Value] = (int) ItemAttributeValueSpinner.Value;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemGubbinCheckbox1_CheckedChanged(object sender, EventArgs e)
		{
//             if (SelectedItem != null)
//             {
//                 if (SelectedItem.Gubbins[0] != ItemGubbinCheckbox1.Checked)
//                 {
//                     SelectedItem.Gubbins[0] = ItemGubbinCheckbox1.Checked;
//                     SetItemsSavedStatus(false);
//                 }
//             }
		}

		private void ItemGubbinCheckbox2_CheckedChanged(object sender, EventArgs e)
		{
//             if (SelectedItem != null)
//             {
//                 if (SelectedItem.Gubbins[1] != ItemGubbinCheckbox2.Checked)
//                 {
//                     SelectedItem.Gubbins[1] = ItemGubbinCheckbox2.Checked;
//                     SetItemsSavedStatus(false);
//                 }
//             }
		}

		private void ItemGubbinCheckbox3_CheckedChanged(object sender, EventArgs e)
		{
//             if (SelectedItem != null)
//             {
//                 if (SelectedItem.Gubbins[2] != ItemGubbinCheckbox3.Checked)
//                 {
//                     SelectedItem.Gubbins[2] = ItemGubbinCheckbox3.Checked;
//                     SetItemsSavedStatus(false);
//                 }
//             }
		}

		private void ItemGubbinCheckbox4_CheckedChanged(object sender, EventArgs e)
		{
//             if (SelectedItem != null)
//             {
//                 if (SelectedItem.Gubbins[3] != ItemGubbinCheckbox4.Checked)
//                 {
//                     SelectedItem.Gubbins[3] = ItemGubbinCheckbox4.Checked;
//                     SetItemsSavedStatus(false);
//                 }
//             }
		}

		private void ItemGubbinCheckbox5_CheckedChanged(object sender, EventArgs e)
		{
//             if (SelectedItem != null)
//             {
//                 if (SelectedItem.Gubbins[4] != ItemGubbinCheckbox5.Checked)
//                 {
//                     SelectedItem.Gubbins[4] = ItemGubbinCheckbox5.Checked;
//                     SetItemsSavedStatus(false);
//                 }
//             }
		}

		private void ItemGubbinCheckbox6_CheckedChanged(object sender, EventArgs e)
		{
//             if (SelectedItem != null)
//             {
//                 if (SelectedItem.Gubbins[5] != ItemGubbinCheckbox6.Checked)
//                 {
//                     SelectedItem.Gubbins[5] = ItemGubbinCheckbox6.Checked;
//                     SetItemsSavedStatus(false);
//                 }
//             }
		}

		private void ItemWeaponRangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (Math.Abs(SelectedItem.Range - (float) ItemWeaponRangeSpinner.Value) > 0.001)
				{
					SelectedItem.Range = (float) ItemWeaponRangeSpinner.Value;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemWeaponRangedAnimText_TextChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.RangedAnimation != ItemWeaponRangedAnimText.Text)
				{
					SelectedItem.RangedAnimation = ItemWeaponRangedAnimText.Text;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemWeaponTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.WType != (Item.WeaponType) (ItemWeaponTypeCombo.SelectedIndex + 1))
				{
					SelectedItem.WType = (Item.WeaponType) (ItemWeaponTypeCombo.SelectedIndex + 1);
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemMiscDataBox_TextChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.MiscData != ItemMiscDataBox.Text)
				{
					SelectedItem.MiscData = ItemMiscDataBox.Text;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemEffectsDurationSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.EatEffectsLength != (int) ItemEffectsDurationSpinner.Value)
				{
					SelectedItem.EatEffectsLength = (uint) ItemEffectsDurationSpinner.Value;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemArmourLevelSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.ArmourLevel != (int) ItemArmourLevelSpinner.Value)
				{
					SelectedItem.ArmourLevel = (int) ItemArmourLevelSpinner.Value;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemDamageSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (SelectedItem.WeaponDamage != (int) ItemDamageSpinner.Value)
				{
					SelectedItem.WeaponDamage = (int) ItemDamageSpinner.Value;
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemWeaponDamageTypeButton_Click(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				int Result = GetDamageType(SelectedItem.WeaponDamageType);
				if (SelectedItem.WeaponDamageType != Result)
				{
					SelectedItem.WeaponDamageType = Result;
					ItemWeaponDamageTypeButton.Text = Item.DamageTypes[SelectedItem.WeaponDamageType];
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemsCollapseGeneral_Click(object sender, EventArgs e)
		{
			ItemGeneralOpen = !ItemGeneralOpen;

			if (ItemGeneralOpen)
			{
				ItemGeneralGroup.Visible = true;
				ItemCollapseGeneral.Text = "General (-)";
			}
			else
			{
				ItemGeneralGroup.Visible = false;
				ItemCollapseGeneral.Text = "General (+)";
			}

			UpdateGroupLocations();
		}

		private void ItemsCollapseAttributes_Click(object sender, EventArgs e)
		{
			ItemAttributesOpen = !ItemAttributesOpen;

			if (ItemAttributesOpen)
			{
				ItemAttributesGroup.Visible = true;
				ItemCollapseAttributes.Text = "Attributes (-)";
			}
			else
			{
				ItemAttributesGroup.Visible = false;
				ItemCollapseAttributes.Text = "Attributes (+)";
			}

			UpdateGroupLocations();
		}

		private void ItemCollapseAppearance_Click(object sender, EventArgs e)
		{
			ItemAppearanceOpen = !ItemAppearanceOpen;

			if (ItemAppearanceOpen)
			{
				ItemAppearanceGroup.Visible = true;
				ItemCollapseAppearance.Text = "Appearance (-)";
			}
			else
			{
				ItemAppearanceGroup.Visible = false;
				ItemCollapseAppearance.Text = "Appearance (+)";
			}

			UpdateGroupLocations();
		}

		private void ItemsCollapseSpecific_Click(object sender, EventArgs e)
		{
			ItemSpecificOpen = !ItemSpecificOpen;

			if (ItemSpecificOpen)
			{
				ItemSpecificGroup.Visible = true;
				ItemCollapseSpecific.Text = "Type-specific settings (-)";
			}
			else
			{
				ItemSpecificGroup.Visible = false;
				ItemCollapseSpecific.Text = "Type-specific settings (+)";
			}

			UpdateGroupLocations();
		}

		private void ItemMaleMeshButton_Click(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				GubbinSelector Sel = new GubbinSelector();
				Sel.CurrentSaved = SelectedItem.GubbinTemplates;

				if (Sel.ShowDialog() == DialogResult.OK)
					SetItemsSavedStatus(false);
//                 SelectedItem.MMeshID = MediaDialogs.GetMesh(true, "Items", SelectedItem.MMeshID);
//                 SetItemsSavedStatus(false);
// 
//                 ItemMaleMeshButton.Text = NiceMeshName(SelectedItem.MMeshID);
			}
		}

		private void ItemFemaleMeshButton_Click(object sender, EventArgs e)
		{
//             if (SelectedItem != null)
//             {
//                 SelectedItem.FMeshID = MediaDialogs.GetMesh(true, "Items", SelectedItem.FMeshID);
//                 SetItemsSavedStatus(false);
// 
//                 ItemFemaleMeshButton.Text = NiceMeshName(SelectedItem.FMeshID);
//             }
		}

		private void ItemThumbnailButton_Click(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				ushort Result = MediaDialogs.GetTexture(false, "Items", (ushort)SelectedItem.ThumbnailTexID);
				if (Result < 65535)
				{
					SelectedItem.ThumbnailTexID = Result;
					SetItemsSavedStatus(false);
					ItemThumbnailButton.Text = NiceTextureName(SelectedItem.ThumbnailTexID);
				}
				else
				{
					SelectedItem.ThumbnailTexID = 65535;
					SetItemsSavedStatus(false);
					ItemThumbnailButton.Text = "No texture set";
				}
			}
		}

		private void ItemDisplayImageButton_Click(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				SelectedItem.ImageID = MediaDialogs.GetTexture(true, (ushort)SelectedItem.ImageID);
				SetItemsSavedStatus(false);

                ItemDisplayImageButton.Text = NiceTextureName((ushort)SelectedItem.ImageID);
			}
		}

		private void ItemRaceCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (ItemRaceCombo.SelectedIndex > 0)
				{
					if (SelectedItem.ExclusiveRace != ItemRaceCombo.SelectedItem.ToString())
					{
						SelectedItem.ExclusiveRace = ItemRaceCombo.SelectedItem.ToString();
						SetItemsSavedStatus(false);
					}
				}
				else if (SelectedItem.ExclusiveRace != "")
				{
					SelectedItem.ExclusiveRace = "";
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemClassCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				if (ItemClassCombo.SelectedIndex > 0)
				{
					if (SelectedItem.ExclusiveClass != ItemClassCombo.SelectedItem.ToString())
					{
						SelectedItem.ExclusiveClass = ItemClassCombo.SelectedItem.ToString();
						SetItemsSavedStatus(false);
					}
				}
				else if (SelectedItem.ExclusiveClass != "")
				{
					SelectedItem.ExclusiveClass = "";
					SetItemsSavedStatus(false);
				}
			}
		}

		private void ItemAttributesChangeButton_Click(object sender, EventArgs e)
		{
			EditAttributes();
		}

		private void ItemWeaponProjectileButton_Click(object sender, EventArgs e)
		{
			if (SelectedItem != null)
			{
				int Result = GetProjectile(SelectedItem.RangedProjectile);
				if (Result < 10001 && SelectedItem.RangedProjectile != Result)
				{
					SelectedItem.RangedProjectile = Result;
					if (Result < 10000 && Projectile.Index[Result] != null)
					{
						ItemWeaponProjectileButton.Text = Projectile.Index[Result].Name;
					}
					else
					{
						ItemWeaponProjectileButton.Text = "(None)";
					}
					SetItemsSavedStatus(false);
				}
			}
		}
		#endregion

		#region Items tab functions
		public void SetSelectedItem(int Index)
		{
			if (Index < ItemsList.Items.Count)
			{
				ItemsList.SelectedIndex = Index;
			}

			// No item is selected, blank everything
			if (ItemsList.SelectedIndex < 0)
			{
				SelectedItem = null;
				ItemIDLabel.Text = "Item ID: No item selected";
				ItemNameBox.Text = "";
				ItemTypeCombo.SelectedIndex = (int) Item.ItemType.Other - 1;
				ItemValueSpinner.Value = 1;
				ItemMassSpinner.Value = 0;
				ItemStackCheckbox.Checked = false;
				ItemDamageCheckbox.Checked = false;
				ItemUseScriptCombo.SelectedIndex = 0;
				ItemAttributeValueSpinner.Value = 0;
				ItemThumbnailButton.Text = "No texture set";
				ItemMaleMeshButton.Text = "No mesh set";
//                 ItemFemaleMeshButton.Text = "No mesh set";
//                 ItemGubbinCheckbox1.Checked = false;
//                 ItemGubbinCheckbox2.Checked = false;
//                 ItemGubbinCheckbox3.Checked = false;
//                 ItemGubbinCheckbox4.Checked = false;
//                 ItemGubbinCheckbox5.Checked = false;
//                 ItemGubbinCheckbox6.Checked = false;
			}
				// An item is selected
			else
			{
				// Get selected item
				ListBoxItem LBI = (ListBoxItem) ItemsList.Items[Index];
				SelectedItem = Item.Index[(uint)LBI.Value];

				// General
				ItemIDLabel.Text = "Item ID: " + SelectedItem.ID.ToString();
				ItemNameBox.Text = SelectedItem.Name;
				ItemTypeCombo.SelectedIndex = (int) SelectedItem.IType - 1;
				UpdateItemInventorySlotList();
				if (SelectedItem.Value < 1)
				{
					SelectedItem.Value = 1;
				}
				ItemValueSpinner.Value = SelectedItem.Value;
				ItemMassSpinner.Value = SelectedItem.Mass;
				ItemStackCheckbox.Checked = SelectedItem.Stackable;
				ItemDamageCheckbox.Checked = SelectedItem.TakesDamage;
				if (!string.IsNullOrEmpty(SelectedItem.ExclusiveRace))
				{
					ItemRaceCombo.SelectedIndex = ItemRaceCombo.FindStringExact(SelectedItem.ExclusiveRace);
				}
				else
				{
					ItemRaceCombo.SelectedIndex = 0;
				}
				if (!string.IsNullOrEmpty(SelectedItem.ExclusiveClass))
				{
					ItemClassCombo.SelectedIndex = ItemClassCombo.FindStringExact(SelectedItem.ExclusiveClass);
				}
				else
				{
					ItemClassCombo.SelectedIndex = 0;
				}
				if (!string.IsNullOrEmpty(SelectedItem.Script))
				{
					int Idx = ItemUseScriptCombo.FindStringExact(SelectedItem.Script);
					if (Idx >= 0)
					{
						ItemUseScriptCombo.SelectedIndex = Idx;
					}
					else
					{
						if (ItemUseScriptCombo.Items.Count > 0)
							ItemUseScriptCombo.SelectedIndex = 0;
					}
				}
				else
				{
					if(ItemUseScriptCombo.Items.Count > 0)
						ItemUseScriptCombo.SelectedIndex = 0;
				}
				UpdateItemScriptFunction();

				// Appearance
				ItemThumbnailButton.Text = NiceTextureName(SelectedItem.ThumbnailTexID);
//                 ItemMaleMeshButton.Text = NiceMeshName(SelectedItem.MMeshID);
//                 ItemFemaleMeshButton.Text = NiceMeshName(SelectedItem.FMeshID);
//                 ItemGubbinCheckbox1.Checked = SelectedItem.Gubbins[0];
//                 ItemGubbinCheckbox2.Checked = SelectedItem.Gubbins[1];
//                 ItemGubbinCheckbox3.Checked = SelectedItem.Gubbins[2];
//                 ItemGubbinCheckbox4.Checked = SelectedItem.Gubbins[3];
//                 ItemGubbinCheckbox5.Checked = SelectedItem.Gubbins[4];
//                 ItemGubbinCheckbox6.Checked = SelectedItem.Gubbins[5];

				// Attributes
				if (ItemAttributesList.SelectedIndex >= 0)
				{
					LBI = (ListBoxItem) ItemAttributesList.Items[ItemAttributesList.SelectedIndex];
					ItemAttributeValueSpinner.Value = SelectedItem.Attributes.Value[LBI.Value];
				}
				else
				{
					ItemAttributeValueSpinner.Value = 0;
				}

				// Type-specific
				UpdateItemSpecificGadgets();
			}
		}

		private void UpdateItemScriptFunction()
		{
			if (SelectedItem != null)
			{
				if (ItemUseFunctionCombo.Items.Count > 0)
				{
					int Idx = ItemUseFunctionCombo.FindString(SelectedItem.Method);
					if (Idx > 0)
					{
						ItemUseFunctionCombo.SelectedIndex = Idx;
					}
					else
					{
						ItemUseFunctionCombo.SelectedIndex = 0;
						if (SelectedItem.Method != ItemUseFunctionCombo.SelectedText)
						{
							SelectedItem.Method = ItemUseFunctionCombo.SelectedText;
							SetItemsSavedStatus(false);
						}
					}
				}
				else
				{
					SelectedItem.Method = "";
				}
			}
		}

		private void UpdateItemSpecificGadgets()
		{
			// Hide all
			ItemDamageLabel.Visible = false;
			ItemDamageSpinner.Visible = false;
			ItemWeaponTypeLabel.Visible = false;
			ItemWeaponTypeCombo.Visible = false;
			ItemWeaponDamageTypeLabel.Visible = false;
			ItemWeaponDamageTypeButton.Visible = false;
			ItemWeaponProjectileLabel.Visible = false;
			ItemWeaponProjectileButton.Visible = false;
			ItemWeaponRangedAnimLabel.Visible = false;
			ItemWeaponRangedAnimText.Visible = false;
			ItemWeaponRangeLabel.Visible = false;
			ItemWeaponRangeSpinner.Visible = false;
			ItemArmourLevelLabel.Visible = false;
			ItemArmourLevelSpinner.Visible = false;
			ItemEffectsDurationLabel.Visible = false;
			ItemEffectsDurationSpinner.Visible = false;
			ItemDisplayImageLabel.Visible = false;
			ItemDisplayImageButton.Visible = false;
			ItemMiscDataLabel.Visible = false;
			ItemMiscDataBox.Visible = false;

			// Show relevant
			switch (ItemTypeCombo.SelectedIndex + 1)
			{
				case (int) Item.ItemType.Weapon:
					ItemDamageLabel.Visible = true;
					ItemDamageSpinner.Visible = true;
					ItemWeaponTypeLabel.Visible = true;
					ItemWeaponTypeCombo.Visible = true;
					ItemWeaponDamageTypeLabel.Visible = true;
					ItemWeaponDamageTypeButton.Visible = true;
					ItemWeaponProjectileLabel.Visible = true;
					ItemWeaponProjectileButton.Visible = true;
					ItemWeaponRangedAnimLabel.Visible = true;
					ItemWeaponRangedAnimText.Visible = true;
					ItemWeaponRangeLabel.Visible = true;
					ItemWeaponRangeSpinner.Visible = true;
					if (SelectedItem != null)
					{
						if (SelectedItem.WeaponDamage < 1)
						{
							SelectedItem.WeaponDamage = 1;
						}
						ItemDamageSpinner.Value = SelectedItem.WeaponDamage;
						ItemWeaponTypeCombo.SelectedIndex = (int) SelectedItem.WType - 1;
						ItemWeaponDamageTypeButton.Text = Item.DamageTypes[SelectedItem.WeaponDamageType];
						if (SelectedItem.RangedProjectile < 10000 &&
							Projectile.Index[SelectedItem.RangedProjectile] != null)
						{
							ItemWeaponProjectileButton.Text = Projectile.Index[SelectedItem.RangedProjectile].Name;
						}
						else
						{
							ItemWeaponProjectileButton.Text = "(None)";
						}
						ItemWeaponRangedAnimText.Text = SelectedItem.RangedAnimation;
						if (SelectedItem.Range < 0.1f)
						{
							SelectedItem.Range = 0.1f;
						}
						ItemWeaponRangeSpinner.Value = (decimal) SelectedItem.Range;
					}
					break;
				case (int) Item.ItemType.Armour:
					ItemArmourLevelLabel.Visible = true;
					ItemArmourLevelSpinner.Visible = true;
					if (SelectedItem != null)
					{
						ItemArmourLevelSpinner.Value = SelectedItem.ArmourLevel;
					}
					break;
				case (int) Item.ItemType.Potion:
					goto case (int) Item.ItemType.Ingredient;
				case (int) Item.ItemType.Ingredient:
					ItemEffectsDurationLabel.Visible = true;
					ItemEffectsDurationSpinner.Visible = true;
					if (SelectedItem != null)
					{
						if (SelectedItem.EatEffectsLength < 1)
						{
							SelectedItem.EatEffectsLength = 1;
						}
						ItemEffectsDurationSpinner.Value = SelectedItem.EatEffectsLength;
					}
					break;
				case (int) Item.ItemType.Image:
					ItemDisplayImageLabel.Visible = true;
					ItemDisplayImageButton.Visible = true;
					if (SelectedItem != null)
					{
						ItemDisplayImageButton.Text = NiceTextureName((ushort)SelectedItem.ImageID);
					}
					break;
				case (int) Item.ItemType.Other:
					ItemMiscDataLabel.Visible = true;
					ItemMiscDataBox.Visible = true;
					if (SelectedItem != null)
					{
						ItemMiscDataBox.Text = SelectedItem.MiscData;
					}
					break;
			}
		}

		private void UpdateItemInventorySlotList()
		{
			ItemInventorySlotCombo.Items.Clear();
			switch (ItemTypeCombo.SelectedIndex + 1)
			{
				case (int) Item.ItemType.Weapon:
					ItemInventorySlotCombo.Enabled = false;
					ItemInventorySlotCombo.Items.Add("Weapon");
					ItemInventorySlotCombo.SelectedIndex = 0;
					break;
				case (int) Item.ItemType.Armour:
					ItemInventorySlotCombo.Enabled = true;
					ItemInventorySlotCombo.Items.Add("Shield");
					ItemInventorySlotCombo.Items.Add("Hat");
					ItemInventorySlotCombo.Items.Add("Chest");
					ItemInventorySlotCombo.Items.Add("Hands");
					ItemInventorySlotCombo.Items.Add("Belt");
					ItemInventorySlotCombo.Items.Add("Legs");
					ItemInventorySlotCombo.Items.Add("Feet");
					if (SelectedItem != null)
					{
						if (SelectedItem.SlotType < Inventory.SlotType.Shield ||
							SelectedItem.SlotType > Inventory.SlotType.Feet)
						{
							SelectedItem.SlotType = Inventory.SlotType.Chest;
						}
						ItemInventorySlotCombo.SelectedIndex = (int) SelectedItem.SlotType - 2;
					}
					break;
				case (int) Item.ItemType.Ring:
					ItemInventorySlotCombo.Enabled = true;
					ItemInventorySlotCombo.Items.Add("Ring");
					ItemInventorySlotCombo.Items.Add("Amulet");
					if (SelectedItem != null)
					{
						if (SelectedItem.SlotType != Inventory.SlotType.Ring &&
							SelectedItem.SlotType != Inventory.SlotType.Amulet)
						{
							SelectedItem.SlotType = Inventory.SlotType.Ring;
						}
						ItemInventorySlotCombo.SelectedIndex = (int) SelectedItem.SlotType - 9;
					}
					break;
				default:
					ItemInventorySlotCombo.Enabled = false;
					if (SelectedItem != null)
					{
						SelectedItem.SlotType = Inventory.SlotType.Backpack;
					}
					ItemInventorySlotCombo.Items.Add("Backpack");
					ItemInventorySlotCombo.SelectedIndex = 0;
					break;
			}
		}

		private void SetItemsSavedStatus(bool Saved)
		{
			ItemsSaved = Saved;
			if (!Saved && ShowUnsavedIndicator)
			{
				TabItems.Text = "Items *";
			}
			else
			{
				TabItems.Text = "Items";
			}
		}

		private void UpdateGroupLocations()
		{
			// All closed
			if (!ItemAppearanceOpen && !ItemSpecificOpen && !ItemGeneralOpen && !ItemAttributesOpen)
			{
				ItemCollapseAttributes.Location = new Point(260, 35);
				ItemCollapseAppearance.Location = new Point(154, 68);
				ItemCollapseSpecific.Location = new Point(260, 68);
			}
				// All open
			else if (ItemAppearanceOpen && ItemSpecificOpen && ItemGeneralOpen && ItemAttributesOpen)
			{
				ItemAttributesGroup.Location = new Point(530, 45);
				ItemCollapseAttributes.Location = new Point(530, 35);
				ItemAppearanceGroup.Location = new Point(154, 440);
				ItemCollapseAppearance.Location = new Point(154, 430);
				ItemSpecificGroup.Location = new Point(530, 440);
				ItemCollapseSpecific.Location = new Point(530, 430);
			}
				// General open
			else if (ItemGeneralOpen)
			{
				ItemAppearanceGroup.Location = new Point(154, 440);
				ItemCollapseAppearance.Location = new Point(154, 430);
				ItemAttributesGroup.Location = new Point(530, 45);
				ItemCollapseAttributes.Location = new Point(530, 35);
				if (ItemAttributesOpen)
				{
					if (ItemAppearanceOpen)
					{
						ItemSpecificGroup.Location = new Point(530, 440);
						ItemCollapseSpecific.Location = new Point(530, 430);
					}
					else
					{
						ItemSpecificGroup.Location = new Point(260, 440);
						ItemCollapseSpecific.Location = new Point(260, 430);
					}
				}
				else
				{
					ItemSpecificGroup.Location = new Point(530, 78);
					ItemCollapseSpecific.Location = new Point(530, 68);
				}
			}
				// General closed
			else if (!ItemGeneralOpen)
			{
				ItemAppearanceGroup.Location = new Point(154, 78);
				ItemCollapseAppearance.Location = new Point(154, 68);
				if (ItemAppearanceOpen)
				{
					ItemAttributesGroup.Location = new Point(530, 45);
					ItemCollapseAttributes.Location = new Point(530, 35);
				}
				else
				{
					ItemAttributesGroup.Location = new Point(260, 45);
					ItemCollapseAttributes.Location = new Point(260, 35);
				}
				if (ItemAttributesOpen)
				{
					ItemSpecificGroup.Location = new Point(ItemAttributesGroup.Location.X, 440);
					ItemCollapseSpecific.Location = new Point(ItemAttributesGroup.Location.X, 430);
				}
				else
				{
					ItemSpecificGroup.Location = new Point(ItemAttributesGroup.Location.X, 78);
					ItemCollapseSpecific.Location = new Point(ItemAttributesGroup.Location.X, 68);
				}
			}

			ItemsList.Focus();
		}
		#endregion

		#region Particles tab events
		private void EmitterEmitterCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!SuppressEmitterChange)
			{
				DialogResult Result = DialogResult.OK;
				if (!EmittersSaved)
				{
					Result = MessageBox.Show("Lose all changes to the current emitter?", "Change emitter",
											 MessageBoxButtons.OKCancel);
				}

				if (Result == DialogResult.OK)
				{
					if (ParticlePreviewConfig != null)
					{
						if (EmitterEmitterCombo.SelectedIndex ==
							EmitterEmitterCombo.Items.IndexOf(ParticlePreviewConfig.Name))
						{
							return;
						}
					}

					SetEmitter((string) EmitterEmitterCombo.SelectedItem);
					SetEmittersSavedStatus(true);
					EmitterEditorGroup.Focus();
				}
				else
				{
					SuppressEmitterChange = true;
					EmitterEmitterCombo.SelectedIndex = EmitterEmitterCombo.Items.IndexOf(ParticlePreviewConfig.Name);
					SuppressEmitterChange = false;
				}
			}
		}

		private void EmitterNewButton_Click(object sender, EventArgs e)
		{
			DialogResult Result = DialogResult.OK;
			if (!EmittersSaved)
			{
				Result = MessageBox.Show("Lose all changes to the current emitter?", "Change emitter",
										 MessageBoxButtons.OKCancel);
			}

			if (Result == DialogResult.OK)
			{
				TextEntry TE = new TextEntry();
				TE.Text = "Create emitter";
				TE.Description.Text = "Enter a name for the new emitter:";
				TE.ShowDialog();
				if (TE.Result != "")
				{
					EmitterConfig.CreateAsFile(@"Data\Emitter Configs\" + TE.Result + ".rpc");
					int Idx = EmitterEmitterCombo.Items.Add(TE.Result);
					//WorldPlaceEmitterCombo.Items.Add(TE.Result);
					EmitterEmitterCombo.SelectedIndex = Idx;
					SetEmittersSavedStatus(true);
				}
				TE.Dispose();
			}
		}

		private void EmitterSaveButton_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion && ParticlePreviewConfig != null)
			{
				ParticlePreviewConfig.Save(@"Data\Emitter Configs\" + ParticlePreviewConfig.Name + ".rpc");
				EmittersChanged = true;
				SetEmittersSavedStatus(true);
			}
		}

		private void EmitterDeleteButton_Click(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				if (MessageBox.Show("Really delete this emitter?", "Delete emitter", MessageBoxButtons.OKCancel) ==
					DialogResult.OK)
				{
					File.Delete(@"Data\Emitter Configs\" + ParticlePreviewConfig.Name + ".rpc");
					General.FreeEmitter(ParticlePreviewEN, true, false);
					ParticlePreviewEN = null;
					EmitterEmitterCombo.Items.RemoveAt(EmitterEmitterCombo.SelectedIndex);
					//WorldPlaceEmitterCombo.Items.Remove(ParticlePreviewConfig.Name);
					SetEmittersSavedStatus(true);
				}
			}
		}

		// Camera control
		private void EmitterPreviewUpButton_MouseDown(object sender, MouseEventArgs e)
		{
			EmitterCamPressedButton = EmitterPreviewUpButton;
		}

		private void EmitterPreviewLeftButton_MouseDown(object sender, MouseEventArgs e)
		{
			EmitterCamPressedButton = EmitterPreviewLeftButton;
		}

		private void EmitterPreviewDownButton_MouseDown(object sender, MouseEventArgs e)
		{
			EmitterCamPressedButton = EmitterPreviewDownButton;
		}

		private void EmitterPreviewRightButton_MouseDown(object sender, MouseEventArgs e)
		{
			EmitterCamPressedButton = EmitterPreviewRightButton;
		}

		private void EmitterPreviewInButton_MouseDown(object sender, MouseEventArgs e)
		{
			EmitterCamPressedButton = EmitterPreviewInButton;
		}

		private void EmitterPreviewOutButton_MouseDown(object sender, MouseEventArgs e)
		{
			EmitterCamPressedButton = EmitterPreviewOutButton;
		}

		private void EmitterPreviewButton_MouseUp(object sender, MouseEventArgs e)
		{
			EmitterCamPressedButton = null;
		}

		private void EmitterPreviewResetButton_Click(object sender, EventArgs e)
		{
			EmitterCamPitch = 0f;
			EmitterCamYaw = 0f;
			EmitterCamDistance = 20f;
			UpdateEmitterCamera();
		}

		private void EmitterPreviewBGColour_Click(object sender, EventArgs e)
		{
			DialogResult D = ColourDialog.ShowDialog();
			if (D == DialogResult.OK)
			{
				EmitterBGR = ColourDialog.Color.R;
				EmitterBGG = ColourDialog.Color.G;
				EmitterBGB = ColourDialog.Color.B;
				EmitterPreviewBGColour.BackColor = Color.FromArgb(EmitterBGR, EmitterBGG, EmitterBGB);
				EmitterPreviewAmountLabel.BackColor = Color.FromArgb(EmitterBGR, EmitterBGG, EmitterBGB);
				Camera.CameraClsColor(EmitterBGR, EmitterBGG, EmitterBGB);
			}
		}

		// Emitter config control
		private void EmitterMaxParticlesSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				int NewValue = (int) EmitterMaxParticlesSpinner.Value;
				if (ParticlePreviewConfig.MaxParticles != NewValue)
				{
					ParticlePreviewConfig.MaxParticles = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterRateSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				int NewValue = (int) EmitterRateSpinner.Value;
				if (ParticlePreviewConfig.ParticlesPerFrame != NewValue)
				{
					ParticlePreviewConfig.ParticlesPerFrame = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterLifeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				int NewValue = (int) EmitterLifeSpinner.Value;
				if (ParticlePreviewConfig.Lifespan != NewValue)
				{
					ParticlePreviewConfig.Lifespan = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterBlendCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				int NewValue = EmitterBlendCombo.SelectedIndex + 1;
				if (ParticlePreviewConfig.BlendMode != NewValue)
				{
					ParticlePreviewConfig.BlendMode = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterSizeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterSizeSpinner.Value;
				if (ParticlePreviewConfig.ScaleStart != NewValue)
				{
					ParticlePreviewConfig.ScaleStart = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterSizeChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterSizeChangeSpinner.Value;
				if (ParticlePreviewConfig.ScaleChange != NewValue)
				{
					ParticlePreviewConfig.ScaleChange = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterAlphaSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterAlphaSpinner.Value;
				if (ParticlePreviewConfig.AlphaStart != NewValue)
				{
					ParticlePreviewConfig.AlphaStart = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterAlphaChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterAlphaChangeSpinner.Value;
				if (ParticlePreviewConfig.AlphaChange != NewValue)
				{
					ParticlePreviewConfig.AlphaChange = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterShapeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				EmitterShape NewValue = (EmitterShape) (EmitterShapeCombo.SelectedIndex + 1);
				if (ParticlePreviewConfig.Shape != NewValue)
				{
					ParticlePreviewConfig.Shape = NewValue;
					SetEmittersSavedStatus(false);
					EnableEmitterShapeGadgets();
				}
			}
		}

		private void EmitterAxisXRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null && EmitterAxisXRadio.Checked)
			{
				if (ParticlePreviewConfig.ShapeAxis != 1)
				{
					ParticlePreviewConfig.ShapeAxis = 1;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterAxisYRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null && EmitterAxisYRadio.Checked)
			{
				if (ParticlePreviewConfig.ShapeAxis != 2)
				{
					ParticlePreviewConfig.ShapeAxis = 2;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterAxisZRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null && EmitterAxisZRadio.Checked)
			{
				if (ParticlePreviewConfig.ShapeAxis != 3)
				{
					ParticlePreviewConfig.ShapeAxis = 3;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterWidthSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterWidthSpinner.Value;
				if (ParticlePreviewConfig.Width != NewValue)
				{
					ParticlePreviewConfig.Width = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterHeightSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterHeightSpinner.Value;
				if (ParticlePreviewConfig.Height != NewValue)
				{
					ParticlePreviewConfig.Height = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterDepthSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterDepthSpinner.Value;
				if (ParticlePreviewConfig.Depth != NewValue)
				{
					ParticlePreviewConfig.Depth = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterInnerRadiusSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterInnerRadiusSpinner.Value;
				if (ParticlePreviewConfig.MinRadius != NewValue)
				{
					ParticlePreviewConfig.MinRadius = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterOuterRadiusSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterOuterRadiusSpinner.Value;
				if (ParticlePreviewConfig.MaxRadius != NewValue)
				{
					ParticlePreviewConfig.MaxRadius = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterColourButton_Click(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				DialogResult D = ColourDialog.ShowDialog();
				if (D == DialogResult.OK)
				{
					ParticlePreviewConfig.RStart = ColourDialog.Color.R;
					ParticlePreviewConfig.GStart = ColourDialog.Color.G;
					ParticlePreviewConfig.BStart = ColourDialog.Color.B;
					EmitterColourButton.BackColor = Color.FromArgb(ParticlePreviewConfig.RStart,
																   ParticlePreviewConfig.GStart,
																   ParticlePreviewConfig.BStart);
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterRChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterRChangeSpinner.Value;
				if (ParticlePreviewConfig.RChange != NewValue)
				{
					ParticlePreviewConfig.RChange = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterGChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterGChangeSpinner.Value;
				if (ParticlePreviewConfig.GChange != NewValue)
				{
					ParticlePreviewConfig.GChange = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterBChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterBChangeSpinner.Value;
				if (ParticlePreviewConfig.BChange != NewValue)
				{
					ParticlePreviewConfig.BChange = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterFramesAcrossSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				int NewValue = (int) EmitterFramesAcrossSpinner.Value;
				if (ParticlePreviewConfig.TexAcross != NewValue)
				{
					ParticlePreviewConfig.TexAcross = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterFramesDownSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				int NewValue = (int) EmitterFramesDownSpinner.Value;
				if (ParticlePreviewConfig.TexDown != NewValue)
				{
					ParticlePreviewConfig.TexDown = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterTexAnimSpeedSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				int NewValue = (int) EmitterTexAnimSpeedSpinner.Value;
				if (ParticlePreviewConfig.TexAnimSpeed != NewValue)
				{
					ParticlePreviewConfig.TexAnimSpeed = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterRandomFrameCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				if (ParticlePreviewConfig.RndStartFrame != EmitterRandomFrameCheck.Checked)
				{
					ParticlePreviewConfig.RndStartFrame = EmitterRandomFrameCheck.Checked;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterTextureButton_Click(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				ushort Result = MediaDialogs.GetTexture(false, "Particles", ParticlePreviewConfig.DefaultTextureID);
				if (Result < 65535 && ParticlePreviewConfig.Texture != Result)
				{
					ParticlePreviewConfig.DefaultTextureID = Result;
					ParticlePreviewConfig.ChangeTexture(Media.GetTexture(Result, false));
					SetEmittersSavedStatus(false);
					EmitterTextureButton.Text = NiceTextureName(Result);
				}
			}
		}

		private void EmitterVXSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterVXSpinner.Value;
				if (ParticlePreviewConfig.VelocityX != NewValue)
				{
					ParticlePreviewConfig.VelocityX = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterVYSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterVYSpinner.Value;
				if (ParticlePreviewConfig.VelocityY != NewValue)
				{
					ParticlePreviewConfig.VelocityY = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterVZSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterVZSpinner.Value;
				if (ParticlePreviewConfig.VelocityZ != NewValue)
				{
					ParticlePreviewConfig.VelocityZ = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterRXSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterRXSpinner.Value;
				if (ParticlePreviewConfig.VelocityRndX != NewValue)
				{
					ParticlePreviewConfig.VelocityRndX = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterRYSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterRYSpinner.Value;
				if (ParticlePreviewConfig.VelocityRndY != NewValue)
				{
					ParticlePreviewConfig.VelocityRndY = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterRZSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterRZSpinner.Value;
				if (ParticlePreviewConfig.VelocityRndZ != NewValue)
				{
					ParticlePreviewConfig.VelocityRndZ = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterVShapeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				VelocityShape NewValue =
					(VelocityShape) (EmitterVShapeCombo.SelectedIndex + 1);
				if (ParticlePreviewConfig.VShape != NewValue)
				{
					ParticlePreviewConfig.VShape = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterFXSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterFXSpinner.Value;
				if (ParticlePreviewConfig.ForceX != NewValue)
				{
					ParticlePreviewConfig.ForceX = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterFYSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterFYSpinner.Value;
				if (ParticlePreviewConfig.ForceY != NewValue)
				{
					ParticlePreviewConfig.ForceY = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterFZSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterFZSpinner.Value;
				if (ParticlePreviewConfig.ForceZ != NewValue)
				{
					ParticlePreviewConfig.ForceZ = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterForceXChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterForceXChangeSpinner.Value;
				if (ParticlePreviewConfig.ForceModX != NewValue)
				{
					ParticlePreviewConfig.ForceModX = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterForceYChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterForceYChangeSpinner.Value;
				if (ParticlePreviewConfig.ForceModY != NewValue)
				{
					ParticlePreviewConfig.ForceModY = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterForceZChangeSpinner_ValueChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				float NewValue = (float) EmitterForceZChangeSpinner.Value;
				if (ParticlePreviewConfig.ForceModZ != NewValue)
				{
					ParticlePreviewConfig.ForceModZ = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}

		private void EmitterForceShapingCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ParticlePreviewConfig != null)
			{
				ForceShape NewValue =
					(ForceShape) (EmitterForceShapingCombo.SelectedIndex + 1);
				if (ParticlePreviewConfig.FShape != NewValue)
				{
					ParticlePreviewConfig.FShape = NewValue;
					SetEmittersSavedStatus(false);
				}
			}
		}
		#endregion

		#region Particles tab functions
		public void SetEmitter(string Name)
		{
			// Free old
			if (ParticlePreviewEN != null)
			{
				General.FreeEmitter(ParticlePreviewEN, true, true);
			}
			// Load new
			EmitterConfig EC = EmitterConfig.Load(@"Data\Emitter Configs\" + Name + ".rpc",
												  Camera, DefaultParticleTexture);
			ParticlePreviewEN = General.CreateEmitter(EC);
			EmitterTextureButton.Text = "Default texture";
			if (EC.DefaultTextureID < 65535)
			{
				uint Tex = Media.GetTexture(EC.DefaultTextureID, true);
				if (Tex != 0)
				{
					EC.ChangeTexture(Tex, false);
					EmitterTextureButton.Text = NiceTextureName(EC.DefaultTextureID);
				}
			}

			// Set gadget values
			EmitterMaxParticlesSpinner.Value = EC.MaxParticles;
			EmitterRateSpinner.Value = EC.ParticlesPerFrame;
			EmitterFramesAcrossSpinner.Value = EC.TexAcross;
			EmitterFramesDownSpinner.Value = EC.TexDown;
			EmitterTexAnimSpeedSpinner.Value = EC.TexAnimSpeed;
			EmitterRandomFrameCheck.Checked = EC.RndStartFrame;
			EmitterVShapeCombo.SelectedIndex = (int) EC.VShape - 1;
			EmitterVXSpinner.Value = (decimal) EC.VelocityX;
			EmitterVYSpinner.Value = (decimal) EC.VelocityY;
			EmitterVZSpinner.Value = (decimal) EC.VelocityZ;
			EmitterRXSpinner.Value = (decimal) EC.VelocityRndX;
			EmitterRYSpinner.Value = (decimal) EC.VelocityRndY;
			EmitterRZSpinner.Value = (decimal) EC.VelocityRndZ;
			EmitterForceShapingCombo.SelectedIndex = (int) EC.FShape - 1;
			EmitterFXSpinner.Value = (decimal) EC.ForceX;
			EmitterFYSpinner.Value = (decimal) EC.ForceY;
			EmitterFZSpinner.Value = (decimal) EC.ForceZ;
			EmitterForceXChangeSpinner.Value = (decimal) EC.ForceModX;
			EmitterForceYChangeSpinner.Value = (decimal) EC.ForceModY;
			EmitterForceZChangeSpinner.Value = (decimal) EC.ForceModZ;
			EmitterSizeSpinner.Value = (decimal) EC.ScaleStart;
			EmitterSizeChangeSpinner.Value = (decimal) EC.ScaleChange;
			EmitterLifeSpinner.Value = EC.Lifespan;
			EmitterBlendCombo.SelectedIndex = EC.BlendMode - 1;
			EmitterShapeCombo.SelectedIndex = (int) EC.Shape - 1;
			EmitterInnerRadiusSpinner.Value = (decimal) EC.MinRadius;
			EmitterOuterRadiusSpinner.Value = (decimal) EC.MaxRadius;
			EmitterWidthSpinner.Value = (decimal) EC.Width;
			EmitterHeightSpinner.Value = (decimal) EC.Height;
			EmitterDepthSpinner.Value = (decimal) EC.Depth;
			EmitterAlphaSpinner.Value = (decimal) EC.AlphaStart;
			EmitterAlphaChangeSpinner.Value = (decimal) EC.AlphaChange;
			if (EC.ShapeAxis == 1)
			{
				EmitterAxisXRadio.Checked = true;
			}
			else if (EC.ShapeAxis == 2)
			{
				EmitterAxisYRadio.Checked = true;
			}
			else
			{
				EmitterAxisZRadio.Checked = true;
			}
			EmitterRChangeSpinner.Value = (decimal) EC.RChange;
			EmitterGChangeSpinner.Value = (decimal) EC.GChange;
			EmitterBChangeSpinner.Value = (decimal) EC.BChange;
			EmitterColourButton.BackColor = Color.FromArgb(EC.RStart, EC.GStart, EC.BStart);

			ParticlePreviewConfig = EC;

			EnableEmitterShapeGadgets();
		}

		private void EnableEmitterShapeGadgets()
		{
			if (ParticlePreviewConfig != null)
			{
				switch (ParticlePreviewConfig.Shape)
				{
					case EmitterShape.Box:
						EmitterWidthSpinner.Enabled = true;
						EmitterHeightSpinner.Enabled = true;
						EmitterDepthSpinner.Enabled = true;
						EmitterInnerRadiusSpinner.Enabled = false;
						EmitterOuterRadiusSpinner.Enabled = false;
						EmitterAxisXRadio.Enabled = false;
						EmitterAxisYRadio.Enabled = false;
						EmitterAxisZRadio.Enabled = false;
						break;
					case EmitterShape.Cylinder:
						EmitterWidthSpinner.Enabled = false;
						EmitterHeightSpinner.Enabled = false;
						EmitterDepthSpinner.Enabled = true;
						EmitterInnerRadiusSpinner.Enabled = true;
						EmitterOuterRadiusSpinner.Enabled = true;
						EmitterAxisXRadio.Enabled = true;
						EmitterAxisYRadio.Enabled = true;
						EmitterAxisZRadio.Enabled = true;
						break;
					case EmitterShape.Sphere:
						EmitterWidthSpinner.Enabled = false;
						EmitterHeightSpinner.Enabled = false;
						EmitterDepthSpinner.Enabled = false;
						EmitterInnerRadiusSpinner.Enabled = true;
						EmitterOuterRadiusSpinner.Enabled = true;
						EmitterAxisXRadio.Enabled = false;
						EmitterAxisYRadio.Enabled = false;
						EmitterAxisZRadio.Enabled = false;
						break;
				}
			}
		}

		private void UpdateEmitterCamera()
		{
			Camera.Position(0f, 0f, 0f);
			Camera.Rotate(EmitterCamPitch, EmitterCamYaw, 0f);
			Camera.Move(0f, 0f, -EmitterCamDistance);
		}

		private void SetEmittersSavedStatus(bool Saved)
		{
			EmittersSaved = Saved;
			if (!Saved && ShowUnsavedIndicator)
			{
				; //WorldTabEmitters.Text = "Emitters *";
			}
			else
			{
				; //WorldTabEmitters.Text = "Emitters";
			}
		}
		#endregion

		#region World tab events
		// A mouse click in the 3D view
		public void RenderingPanel_MouseClick(object sender, MouseEventArgs e)
		{
			PanelMouseHover = true;
			// In the world tab
			if (ControlHost.SelectedIndex == (int) GETab.WORLD &&
				SetWorldButtonSelection <= (int) WorldButtonSelection.OBJECT_SETUP && !Mouselooking)
			{
				// Scale mouse position to back-buffer co-ordinates
				Point MousePos = RenderingPanel.PointToClient(Cursor.Position);
				int MouseX = MousePos.X;// (MousePos.X * 1024) / RenderingPanel.Size.Width;
				int MouseY = MousePos.Y;// (MousePos.Y * 768) / RenderingPanel.Size.Height;

				// Left click (select an object)
				if (e.Button == MouseButtons.Left)
				{
					Entity E = Collision.CameraPick(Camera, MouseX, MouseY);
					if (E != null && Program.IsRendered(E))
					{
						// Waypoint linking
						if (WaypointLinkMode > 0)
						{
							ZoneObject Z = E.ExtraData as ZoneObject;
							Waypoint WP = Z as Waypoint;
							if (WP != null)
							{
								if (WP != LinkingWaypoint)
								{
									if (WaypointLinkMode == 1)
									{
										// Remove current link if any
										if (LinkingWaypoint.NextA != null)
										{
											LinkingWaypoint.NextA.Prev = null;
										}
										// Set new link
										LinkingWaypoint.NextA = WP;
										WP.Prev = LinkingWaypoint;

										if (LinkingWaypoint.WaypointLinkAEN == null)
										{
											LinkingWaypoint.WaypointLinkAEN = WaypointLinkTemplate.Copy();
											LinkingWaypoint.WaypointLinkAEN.Shader = Shaders.LitObject1;
											LinkingWaypoint.WaypointLinkAEN.Texture(OrangeTex);
										}
									}
									else
									{
										// Set new link
										LinkingWaypoint.NextB = WP;
										if (LinkingWaypoint.WaypointLinkBEN == null)
										{
											LinkingWaypoint.WaypointLinkBEN = WaypointLinkTemplate.Copy();
											LinkingWaypoint.WaypointLinkBEN.Shader = Shaders.LitObject1;
											LinkingWaypoint.WaypointLinkBEN.Texture(BlueTex);
										}
									}
									UpdateWaypointLinks();
									SetWorldSavedStatus(false);
									RenderingPanel.Focus();
								}
								else
								{
									MessageBox.Show("You cannot link a waypoint to itself. Linking cancelled.", "Error");
								}
								WaypointLinkMode = 0;
							}
							else
							{
								MessageBox.Show("Please select a waypoint to link to.", "Error");
							}
						}
							// Add to selection
						else if (KeyState.Get(Keys.ControlKey))
						{
							WorldSelectObject((ZoneObject) E.ExtraData, 1);
						}
							// Remove from selection
						else if (KeyState.Get(Keys.ShiftKey))
						{
							WorldSelectObject((ZoneObject) E.ExtraData, 2);
						}
							// Replace selection
						else
						{
							WorldSelectObject((ZoneObject) E.ExtraData, 3);
						}
					}
				}
					// Right click
				else if (e.Button == MouseButtons.Right)
				{
					switch (SetWorldButtonSelection)
					{
							// Place an object
						case (int) WorldButtonSelection.CREATE:
//                            PlaceObjectInZone(MouseX, MouseY);
							break;
					}
				}
			}
		}

		private void RenderingPanel_MouseHover(object sender, EventArgs e)
		{
			PanelMouseHover = true;
		}

		private void RenderingPanel_MouseLeave(object sender, EventArgs e)
		{
			PanelMouseHover = false;
		}

		private void RenderingPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.X <= 0 || e.X >= RenderingPanel.Size.Width || e.Y <= 0 || e.Y >= RenderingPanel.Size.Height)
			{
				PanelMouseHover = false;
			}

			if (e.Button == MouseButtons.Left)
			{
             
				//if (GE.GUIManager.ControlFocus != null /*&& !(GE.GUIManager.ControlFocus is NWindow)*/)
				if (Parameters.MouseThumb != null)
				{
					//NControl control = GE.GUIManager.ControlFocus;
					NControl control = Parameters.MouseThumb;
					cControl GUIControl = (cControl)control.Tag;

					if(m_propertyWindow.ObjectProperties.SelectedObject != GUIControl)
						m_propertyWindow.ObjectProperties.SelectedObject = GUIControl;


					Point Mouse = PointToClient(Cursor.Position);
					NVector2 a = new NVector2(Mouse.X, Mouse.Y);
					NVector2 c = new NVector2(a.X - b.X, a.Y - b.Y);

					GUIControl.SynchonizeLocation( c );
					m_propertyWindow.ObjectProperties.Refresh();
					SetWorldSavedStatus(false);
					b = a;

					// update NGUINet
					Mouse = RenderingPanel.PointToClient(Cursor.Position);
					Parameters.MousePosition = new NVector2(Mouse.X, Mouse.Y);
					//GUIManager.Update(Parameters);
					Parameters.InputBuffer.Clear();
					Render.RenderWorld();
				}
			}
		}

		// Events for TreeZone (Marian Voicu)
		#endregion

		#region World tab functions
		// Loading/saving
		public void SaveWorld()
		{
			if (CurrentClientZone != null)
			{
				CurrentClientZone.Save();
			}
			if (CurrentServerZone != null)
			{
				CurrentServerZone.Save(CurrentClientZone);
			}
			Environment.SaveEnvironment(true);

			if (m_GubbinEditor.Saved == false)
			{
				m_GubbinEditor.Save();
			}

			SetWorldSavedStatus(true);
		}

		public void SetWorldSavedStatus(bool Saved)
		{
			WorldSaved = Saved;
			if (!Saved && ShowUnsavedIndicator)
			{
				TabWorld.Text = "World *";
			}
			else
			{
				TabWorld.Text = "World";
			}
		}

		public void UpdateZoneLoadProgress(int Value)
		{
			if (Value < 100)
			{
				MainProgress.Visible = true;
				MainProgress.Value = Value;
				Application.DoEvents();
			}
			else
			{
				MainProgress.Visible = false;
			}
		}

		public void UnloadCurrentZone()
		{
			// Clear the undo system
			Undo.Clear();

			// Clear current selections
			if (SetWorldButtonSelection == (int) WorldButtonSelection.CREATE)
			{
				ZoneSelected.Clear();
			}
			else
			{
				while (ZoneSelected.Count > 0)
				{
					if (lastNodeSelect == nodeSelect)
					{
						ZoneSelected.Clear();
					}
					else
					{
						Program.GE.m_ZoneList.ZoneObjectListCheck((ZoneObject) ZoneSelected[0], false);
					}
				}
			}

			if (Program.Transformer != null)
				Program.Transformer.Free();
			Program.Transformer = null;

			// Client zone
			if (CurrentClientZone != null)
			{
				CurrentClientZone.Unload();
				CurrentClientZone = null;
			}
			// Server zone
			if (CurrentServerZone != null)
			{
				CurrentServerZone.Delete();
				CurrentServerZone = null;
			}

			// ZoneObjects representing server objects
			ZoneObject Obj;
			for (int i = 0; i < Triggers.Count; ++i)
			{
				Obj = Triggers[i];
				Obj.Freeing();
				if (Obj.EN != null)
				{
					Obj.EN.Free();
				}

				if ((Obj as Trigger).Label != null)
				{
					GUIManager.Destroy((Obj as Trigger).Label);
					(Obj as Trigger).Label = null;
				}
			}
			Triggers.Clear();
			for (int i = 0; i < Portals.Count; ++i)
			{
				Obj = Portals[i];
				Obj.Freeing();
				if (Obj.EN != null)
				{
					Obj.EN.Free();
				}

				if ((Obj as Portal).Label != null)
				{
					GUIManager.Destroy((Obj as Portal).Label);
					(Obj as Portal).Label = null;
				}
			}
			Portals.Clear();
			for (int i = 0; i < Waypoints.Count; ++i)
			{
				Obj = Waypoints[i];
				Obj.Freeing();
				if (Obj.EN != null)
				{
					Obj.EN.Free();
				}

				if ((Obj as Waypoint).Label != null)
				{
					GUIManager.Destroy((Obj as Waypoint).Label);
					(Obj as Waypoint).Label = null;
				}

				Waypoint WP = Waypoints[i] as Waypoint;
				if (WP.WaypointLinkAEN != null)
					WP.WaypointLinkAEN.Free();
				if (WP.WaypointLinkBEN != null)
					WP.WaypointLinkBEN.Free();
				WP.WaypointLinkAEN = null;
				WP.WaypointLinkBEN = null;
			}
			Waypoints.Clear();
			
		}

		// Object selection
		public void WorldSelectObject(ZoneObject Obj, int Mode)
		{
			switch (Mode)
			{
					// Add to selection
				case 1:
					if (!ZoneSelected.Contains(Obj))
					{
						Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(Obj, true);
					}
					break;
					// Remove from selection
				case 2:
					if (ZoneSelected.Contains(Obj))
					{
						Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(Obj, false);
					}
					break;
					// Replace selection
				case 3:
					// Clear current selections
					while (ZoneSelected.Count > 0)
					{
						Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection((ZoneObject) ZoneSelected[0], false);
					}

					// Add new
					Program.GE.m_ZoneList.ZoneObjectListCheckLastSelection(Obj, true);
					break;
			}

			Program.GE.m_propertyWindow.RefreshObjectWindow();
		}

		public void ClearSelectionBox(Entity EN)
		{
			if (EN == null)
				return;
			Entity ChildEN;
			int Children = EN.CountChildren();
			for (int i = 1; i <= Children; ++i)
			{
				ChildEN = EN.GetChild(i);
				if (ChildEN.Name == "GE selection box")
				{
					if (Program.Transformer != null && Program.Transformer.HasParent(ChildEN))
					{
						Program.Transformer.Free();
						Program.Transformer = null;
					}
					ChildEN.Free();
					break;
				}
			}
		}

		public void AddSelectionBox(Entity EN)
		{
			if (EN == null)
				return;

			// Remove old box if any
			ClearSelectionBox(EN);

			// Add new box
			Entity SelectionBox = Entity.CreateCube();
			SelectionBox.Name = "GE selection box";
			SelectionBox.Shader = Shaders.WireFrame;
			SelectionBox.Texture(YellowTex);
			//   SelectionBox.AlphaNoSolid(0.5f);
			SelectionBox.Parent(EN, false);
			MeshMinMaxVertices MMV = new MeshMinMaxVertices(EN);
			float Width = (MMV.MaxX - MMV.MinX) * 0.501f;
			float Height = (MMV.MaxY - MMV.MinY) * 0.501f;
			float Depth = (MMV.MaxZ - MMV.MinZ) * 0.501f;
			SelectionBox.Scale(Width, Height, Depth);
			Width = MMV.MinX + Width;
			Height = MMV.MinY + Height;
			Depth = MMV.MinZ + Depth;
			SelectionBox.Move(Width, Height, Depth);
		}

		// Object control

		public void RemoveObject(ZoneObject Obj)
		{
			if (Obj as Trigger != null)
			{
				Triggers.Remove(Obj);
			}
			else if (Obj as Portal != null)
			{
				Portals.Remove(Obj);
			}
			else if (Obj as Waypoint != null)
			{
				Waypoints.Remove(Obj);
			}
			else
			{
				CurrentClientZone.RemoveObject(Obj);
			}
		}

		private void SetSelectionPickMode(PickMode Mode)
		{
			ZoneObject Obj = null;
			for (int i = 0; i < ZoneSelected.Count; ++i)
			{
				Obj = (ZoneObject) ZoneSelected[i];
				Collision.EntityPickMode(Obj.EN, Mode);
			}
		}

		public void PositionSelection(float X, float Y, float Z)
		{
			ZoneObject Obj = (ZoneObject) ZoneSelected[0];
			X -= Obj.EN.X(true);
			Y -= Obj.EN.Y(true);
			Z -= Obj.EN.Z(true);
			MoveSelection(X, Y, Z);
		}

		internal void MoveSelection(float X, float Y, float Z)
		{
			// Create undo point
			if (!SuppressZoneUndo)
			{
				Undo.CreateAdditiveUndo(Undo.Actions.Move, null, X, Y, Z);
			}

			// Move each object
			ZoneObject Obj = null;
			for (int i = 0; i < ZoneSelected.Count; ++i)
			{
				Obj = (ZoneObject) ZoneSelected[i];
				Obj.EN.Translate(X, Y, Z);
				Obj.UpdateServerVersion(CurrentServerZone);
				// Special cases
				if (Obj is Waypoint)
				{
					UpdateWaypointLinks();
				}
			}

			// Update spinners if only one object is selected
			if (ZoneSelected.Count == 1)
			{
				SuppressZoneTransforms = true;
				SuppressZoneTransforms = false;
			}

			SetWorldSavedStatus(false);
		}

		private void RotateSelection(float X, float Y, float Z)
		{
			ZoneObject Obj = (ZoneObject) ZoneSelected[0];
			X -= Obj.EN.Pitch();
			Y -= Obj.EN.Yaw();
			Z -= Obj.EN.Roll();
			TurnSelection(X, Y, Z);
		}

		internal void TurnSelection(float X, float Y, float Z)
		{
			// Create undo point
			if (!SuppressZoneUndo)
			{
				Undo.CreateAdditiveUndo(Undo.Actions.Rotate, null, X, Y, Z);
			}

			// Turn each object
			ZoneObject Obj = null;
			for (int i = 0; i < ZoneSelected.Count; ++i)
			{
				Obj = (ZoneObject) ZoneSelected[i];
				// Special cases
				if (Obj is Portal)
				{
					X = 0f;
					Z = 0f;
				}
				Obj.EN.Turn(X, Y, Z, false);
				// Special cases
				if (Obj is Trigger || Obj is Waypoint || Obj is SoundZone || Obj is Water ||
					Obj is RealmCrafter.ClientZone.Light || Obj is RealmCrafter.ClientZone.MenuControl)
				{
					Obj.EN.Rotate(0f, 0f, 0f);
				}
				Obj.UpdateServerVersion(CurrentServerZone);
			}

			// Update spinners if only one object is selected
			if (ZoneSelected.Count == 1)
			{
				SuppressZoneTransforms = true;
				SuppressZoneTransforms = false;
			}

			SetWorldSavedStatus(false);
		}

		public void ScaleSelectionAbsolute(float X, float Y, float Z)
		{
			// Limit maximum scale
			if (X > 10000000000f)
			{
				X = 10000000000f;
			}
			if (Y > 10000000000f)
			{
				Y = 10000000000f;
			}
			if (Z > 10000000000f)
			{
				Z = 10000000000f;
			}

			ZoneObject Obj = (ZoneObject) ZoneSelected[0];
			// Special cases
			if (Obj is SoundZone || Obj is Portal || Obj is Trigger || Obj is RealmCrafter.ClientZone.Light || Obj is RealmCrafter.ClientZone.MenuControl)
			{
				Y = X;
				Z = X;
			}
			else if (Obj is Waypoint)
			{
				//if (CurrentServerZone.GetSpawnPoint(((Waypoint) Obj).ServerID) < 0)
				if((Obj as Waypoint).Max == 0)
				{
					X = 1f;
				}
				Y = X;
				Z = X;
			}
			else if (Obj is Water)
			{
				Y = 20f;
			}
			// Create undo point
			if (!SuppressZoneUndo)
			{
				Undo.CreateNonRepeatingUndo(Undo.Actions.Scale, "A", Obj.EN.ScaleX() * 20f, Obj.EN.ScaleY() * 20f,
											Obj.EN.ScaleZ() * 20f);
			}
			// Perform scale
			Obj.EN.Scale(X * 0.05f, Y * 0.05f, Z * 0.05f, false);
			// Special cases
			Obj.UpdateServerVersion(CurrentServerZone);
			// Update spinner values
			SuppressZoneTransforms = true;

			SuppressZoneTransforms = false;
		}

		public void ScaleSelection(float X, float Y, float Z)
		{
			X += 1f;
			Y += 1f;
			Z += 1f;

			// Create undo point
			if (!SuppressZoneUndo)
			{
				Undo.CreateMultiplyUndo(Undo.Actions.Scale, "R", X, Y, Z);
			}

			// Scale each object
			ZoneObject Obj = null;
			for (int i = 0; i < ZoneSelected.Count; ++i)
			{
				Obj = (ZoneObject) ZoneSelected[i];
				// Special cases
				if (Obj is SoundZone || Obj is Portal || Obj is Trigger || Obj is RealmCrafter.ClientZone.Light || Obj is RealmCrafter.ClientZone.MenuControl)
				{
					Y = X;
					Z = X;
				}
				else if (Obj is Waypoint)
				{
					//if (CurrentServerZone.GetSpawnPoint(((Waypoint) Obj).ServerID) < 0)
					if((Obj as Waypoint).Max == 0)
					{
						X = 1f;
					}
					Y = X;
					Z = X;
				}
				else if (Obj is Water)
				{
					Y = 1f;
				}
				Obj.EN.ScaleRelative(X, Y, Z, false);
				// Special cases
				Obj.UpdateServerVersion(CurrentServerZone);
			}

			// Update spinners if only one object is selected
			if (ZoneSelected.Count == 1)
			{
				SuppressZoneTransforms = true;
				decimal XScale = (decimal) (Obj.EN.ScaleX() * 20f);
				decimal YScale = (decimal) (Obj.EN.ScaleY() * 20f);
				decimal ZScale = (decimal) (Obj.EN.ScaleZ() * 20f);
				// Limit maximum scale
				if (XScale > 10000000000)
				{
					XScale = 10000000000;
				}
				if (YScale > 10000000000)
				{
					YScale = 10000000000;
				}
				if (ZScale > 10000000000)
				{
					ZScale = 10000000000;
				}
				SuppressZoneTransforms = false;
			}

			SetWorldSavedStatus(false);
		}

		// Other
		public void ScaleEntireZone(float F)
		{
			Scenery S;
			Water W;
			ColBox CB;
			Emitter E;
			SoundZone SZ;
			RCTTerrain T;
			Trigger Tr;
			Portal P;
			Waypoint WP;

			// Sceneries
			for (int i = 0; i < CurrentClientZone.Sceneries.Count; ++i)
			{
				S = (Scenery) CurrentClientZone.Sceneries[i];
				S.EN.Scale(S.EN.ScaleX() * F, S.EN.ScaleY() * F, S.EN.ScaleZ() * F);
				S.EN.Position(S.EN.X() * F, S.EN.Y() * F, S.EN.Z() * F);
			}

			// Water areas
			for (int i = 0; i < CurrentClientZone.Waters.Count; ++i)
			{
				W = (Water) CurrentClientZone.Waters[i];
				W.EN.Scale(W.EN.ScaleX() * F, W.EN.ScaleY() * F, W.EN.ScaleZ() * F);
				W.EN.Position(W.EN.X() * F, W.EN.Y() * F, W.EN.Z() * F);
				W.UpdateServerVersion(CurrentServerZone);
			}

			// Collision boxes
			for (int i = 0; i < CurrentClientZone.ColBoxes.Count; ++i)
			{
				CB = (ColBox) CurrentClientZone.ColBoxes[i];
				CB.EN.Scale(CB.EN.ScaleX() * F, CB.EN.ScaleY() * F, CB.EN.ScaleZ() * F);
				CB.EN.Position(CB.EN.X() * F, CB.EN.Y() * F, CB.EN.Z() * F);
			}

			// Emitters
			for (int i = 0; i < CurrentClientZone.Emitters.Count; ++i)
			{
				E = (Emitter) CurrentClientZone.Emitters[i];
				E.EN.Scale(E.EN.ScaleX() * F, E.EN.ScaleY() * F, E.EN.ScaleZ() * F);
				E.EN.Position(E.EN.X() * F, E.EN.Y() * F, E.EN.Z() * F);
			}

			// Sound zones
			for (int i = 0; i < CurrentClientZone.SoundZones.Count; ++i)
			{
				SZ = (SoundZone) CurrentClientZone.SoundZones[i];
				SZ.EN.Scale(SZ.EN.ScaleX() * F, SZ.EN.ScaleY() * F, SZ.EN.ScaleZ() * F);
				SZ.EN.Position(SZ.EN.X() * F, SZ.EN.Y() * F, SZ.EN.Z() * F);
			}

			// Terrains
			for (int i = 0; i < CurrentClientZone.Terrains.Count; ++i)
			{
				T = (RCTTerrain) CurrentClientZone.Terrains[i];
				//T.EN.Scale(T.EN.ScaleX() * F, T.EN.ScaleY() * F, T.EN.ScaleZ() * F);
				//T.EN.Position(T.EN.X() * F, T.EN.Y() * F, T.EN.Z() * F);
				T.Terrain.SetPosition(new NVector3(T.Terrain.GetPosition().X * F, T.Terrain.GetPosition().Y * F, T.Terrain.GetPosition().Z * F));
			}

			// Triggers
			for (int i = 0; i < Triggers.Count; ++i)
			{
				Tr = (Trigger) Triggers[i];
				Tr.EN.Scale(Tr.EN.ScaleX() * F, Tr.EN.ScaleY() * F, Tr.EN.ScaleZ() * F);
				Tr.EN.Position(Tr.EN.X() * F, Tr.EN.Y() * F, Tr.EN.Z() * F);
				Tr.UpdateServerVersion(CurrentServerZone);
			}

			// Portals
			for (int i = 0; i < Portals.Count; ++i)
			{
				P = (Portal) Portals[i];
				P.EN.Scale(P.EN.ScaleX() * F, P.EN.ScaleY() * F, P.EN.ScaleZ() * F);
				P.EN.Position(P.EN.X() * F, P.EN.Y() * F, P.EN.Z() * F);
				P.UpdateServerVersion(CurrentServerZone);
			}

			// Spawn points
			for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
			{
				CurrentServerZone.Waypoints[i].Size *= F;
			}

			// Waypoints
			for (int i = 0; i < Waypoints.Count; ++i)
			{
				WP = (Waypoint) Waypoints[i];
				WP.EN.Position(WP.EN.X() * F, WP.EN.Y() * F, WP.EN.Z() * F);
				WP.UpdateServerVersion(CurrentServerZone);

				WP.EN.Scale(WP.Size, WP.Size, WP.Size);

//                 int SpawnNum = CurrentServerZone.GetSpawnPoint(i);
//                 if (SpawnNum >= 0)
//                 {
//                     WP.EN.Scale(CurrentServerZone.SpawnSize[SpawnNum], CurrentServerZone.SpawnSize[SpawnNum],
//                                 CurrentServerZone.SpawnSize[SpawnNum]);
//                 }
			}
			UpdateWaypointLinks();

			SetWorldSavedStatus(false);
		}

		public void UpdateWaypointLinks()
		{
			for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
			{
				Waypoint WP = CurrentServerZone.Waypoints[i];
				if (WP.NextA != null)
				{
					WP.WaypointLinkAEN.Scale(1, 1, WP.WaypointEN.Distance(WP.NextA.WaypointEN));
					WP.WaypointLinkAEN.Position(WP.WaypointEN.X(), WP.WaypointEN.Y(), WP.WaypointEN.Z());
					WP.WaypointLinkAEN.Point(WP.NextA.WaypointEN);
				}
				if (WP.NextB != null)
				{
					WP.WaypointLinkBEN.Scale(1, 1, WP.WaypointEN.Distance(WP.NextB.WaypointEN));
					WP.WaypointLinkBEN.Position(WP.WaypointEN.X(), WP.WaypointEN.Y(), WP.WaypointEN.Z());
					WP.WaypointLinkBEN.Point(WP.NextB.WaypointEN);
				}
			}
		}

		public void RepositionGrid()
		{
			int SquareSize = GridSize / GridDetail;
			for (int i = 0; i <= GridDetail; ++i)
			{
				int XOffset = (int) WorldCamX - ((int) WorldCamX % SquareSize);
				int ZOffset = (int) WorldCamZ - ((int) WorldCamZ % SquareSize);
				int Pos = (i - (GridDetail / 2)) * SquareSize;
				Grid[i, 0].SetPositions(XOffset + Pos, GridHeight, ZOffset + (GridSize / -2), XOffset + Pos, GridHeight,
										ZOffset + (GridSize / 2));
				Grid[i, 1].SetPositions(XOffset + (GridSize / -2), GridHeight, ZOffset + Pos, XOffset + (GridSize / 2),
										GridHeight, ZOffset + Pos);
			}
		}
		#endregion

		#region MouseWheel Functions
		void TabMedia_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (PanelMouseHover)
			{
				float MaxCamFar = -300;
				float MaxCamClose = 300f;
				float CamZ = Camera.Z() + e.Delta / 10;

				if (!(CamZ >= MaxCamFar))
				{
					Camera.Position(Camera.X(), Camera.Y(), MaxCamFar);
				}
				else if (!(CamZ <= MaxCamClose))
				{
					Camera.Position(Camera.X(), Camera.Y(), MaxCamClose);
				}
				else
				{
					Camera.Position(Camera.X(), Camera.Y(), CamZ);
				}
			}
		}
		private void ActorsAnimsTab_MouseWheel(object sender, MouseEventArgs e)
		{
			if (PanelMouseHover)
			{
				float MaxCamFar = -150f;
				float MaxCamClose = 50f;
				float CamZ = Camera.Z() + e.Delta / 20;

				if (!(CamZ >= MaxCamFar))
				{
					Camera.Position(Camera.X(), Camera.Y(), MaxCamFar);
				}
				else if (!(CamZ <= MaxCamClose))
				{
					Camera.Position(Camera.X(), Camera.Y(), MaxCamClose);
				}
				else
				{
					Camera.Position(Camera.X(), Camera.Y(), CamZ);
				}
			}
		}
		void ActorsGubbinToolTab_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (PanelMouseHover)
			{
				float MaxCamFar = -300;
				float MaxCamClose = 300f;
				float CamZ = Camera.Z() + e.Delta / 10;

				if (!(CamZ >= MaxCamFar))
				{
					Camera.Position(Camera.X(), Camera.Y(), MaxCamFar);
				}
				else if (!(CamZ <= MaxCamClose))
				{
					Camera.Position(Camera.X(), Camera.Y(), MaxCamClose);
				}
				else
				{
					Camera.Position(Camera.X(), Camera.Y(), CamZ);
				}
			}
		}
		#endregion

		#region Main rendering loop
		public void MainLoop(object sender, EventArgs e)
		{

			while (Program.AppStillIdle)
			{
				#region Timing code
				Timer.Update();
				DeltaTime = Timer.DeltaTime;
				MilliSecs = Timer.Ticks;

				// Store delta time
				DeltaBuffer[DeltaBufferIndex] = DeltaTime;
				DeltaBufferIndex++;
				if (DeltaBufferIndex == DeltaFrames)
				{
					DeltaBufferIndex = 0;
				}

				// Take average of last DeltaFrames frames to get delta time coefficient
				// ( Marian Voicu )
				if (ControlHost.SelectedIndex != (int) GETab.MEDIA)
				{
					SpeedFrames = 1;
				}
				// end ( MV )
				float Time = 0;
				for (int i = 0; i < /* end MV */ SpeedFrames * DeltaFrames; i++)
				{
					Time += DeltaBuffer[i];
				}
				Time /= (float) DeltaFrames;
				float FPS = 1000f / Time;
				Delta = BaseFPS / FPS;
				if (Delta > 5f)
				{
					Delta = 5f;
				}
				#endregion

				// Main form is active
				if (ActiveForm == this)
				{
					#region Update emitter editor gadgets
					if (ControlHost.SelectedIndex == (int)GETab.EMITTER)
					{
						// Preview camera adjustment buttons
						if (EmitterCamPressedButton != null)
						{
							if (EmitterCamPressedButton == EmitterPreviewInButton)
							{
								EmitterCamDistance -= 2f * Delta;
								if (EmitterCamDistance < 2f)
								{
									EmitterCamDistance = 2f;
								}
							}
							else if (EmitterCamPressedButton == EmitterPreviewOutButton)
							{
								EmitterCamDistance += 2f * Delta;
								if (EmitterCamDistance > 1000f)
								{
									EmitterCamDistance = 1000f;
								}
							}
							else if (EmitterCamPressedButton == EmitterPreviewLeftButton)
							{
								EmitterCamYaw -= 2f * Delta;
								if (EmitterCamYaw < -180f)
								{
									EmitterCamYaw += 360f;
								}
							}
							else if (EmitterCamPressedButton == EmitterPreviewRightButton)
							{
								EmitterCamYaw += 2f * Delta;
								if (EmitterCamYaw > 180f)
								{
									EmitterCamYaw -= 360f;
								}
							}
							else if (EmitterCamPressedButton == EmitterPreviewDownButton)
							{
								EmitterCamPitch += 2f * Delta;
								if (EmitterCamPitch > 89f)
								{
									EmitterCamPitch = 89f;
								}
							}
							else if (EmitterCamPressedButton == EmitterPreviewUpButton)
							{
								EmitterCamPitch -= 2f * Delta;
								if (EmitterCamPitch < -89f)
								{
									EmitterCamPitch = -89f;
								}
							}
							UpdateEmitterCamera();
						}

						// Active particles display
						if (ParticlePreviewEN != null)
						{
							EmitterPreviewAmountLabel.Text = "Active particles: " +
															 General.EmitterActiveParticles(
																 ParticlePreviewEN).ToString();
						}
					}
					#endregion


					// Ctrl+ shortcuts

					#region Ctrl+S to save current tab
					if (KeyState.Get(Keys.S) && KeyState.Get(Keys.ControlKey))
					{
						SaveHotkeyDown = true;
					}
					else if (SaveHotkeyDown)
					{
						switch (ControlHost.SelectedIndex)
						{
								// Actors
							case (int) GETab.ACTORS:
								if (!ActorsSaved)
								{
									ActorSaveGeneralSettings();
									AnimSet.Save(@"Data\Game Data\Animations.dat");
									Ability.Save(Ability.SpellDatabase);
									Actor.SaveFactions(Actor.FactionDatabase);
                                    Actor.Save(Actor.ActorDatabase);
									SetActorsSavedStatus(true);
								}
								break;
								// Items
							case (int) GETab.ITEMS:
								if (!ItemsSaved)
								{
									Item.Save(ItemsFile);
									Actors3D.SaveGubbinNames();
									FileStream FStream = new FileStream(
										@"Data\Server Data\Misc.dat", FileMode.Open,
										FileAccess.Write);
									BinaryWriter F = new BinaryWriter(FStream);
									if (F == null)
									{
										MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
										return;
									}
									F.BaseStream.Seek(12, SeekOrigin.Begin);
									F.Write(Item.WeaponDamageEnabled);
									F.Write(Item.ArmourDamageEnabled);
									F.Close();
									SetItemsSavedStatus(true);
								}
								break;
								// World
							case (int) GETab.WORLD:
								// Zone
								if (SetWorldButtonSelection != (int) WorldButtonSelection.EMITTERS)
								{
									if (!WorldSaved)
									{
										SaveWorld();
									}
								}
									// Emitter
								else
								{
									if (!EmittersSaved)
									{
										if (ParticlePreviewConfig != null)
										{
											ParticlePreviewConfig.Save(@"Data\Emitter Configs\" +
																	   ParticlePreviewConfig.Name + ".rpc");
											SetEmittersSavedStatus(true);
										}
									}
								}
								break;
								// Interface
							case (int) GETab.INTERFACE:
//                                 // Interface
//                                 if (!InterfaceSaved)
//                                 {
//                                     Component.SaveAll(@"Data\Game Data\Interface.dat");
//                                     SetInterfaceSavedStatus(true);
//                                 }
								break;
						}
						SaveHotkeyDown = false;
					}
					#endregion

					#region Ctrl+Q to quit
					if (KeyState.Get(Keys.Q) && KeyState.Get(Keys.ControlKey))
					{
						Close();
					}
					#endregion

					#region Ctrl+Z to undo
					if (KeyState.Get(Keys.Z) && KeyState.Get(Keys.ControlKey))
					{
						if (CtrlZWasDown == false)
						{
							CtrlZWasDown = true;
							if (ControlHost.SelectedIndex == (int) GETab.WORLD &&
								SetWorldButtonSelection < (int) WorldButtonSelection.EMITTERS)
							{
								Undo.Perform(this);
							}
						}
					}
					else
					{
						CtrlZWasDown = false;
					}
					#endregion

					// Actor preview controls
                  
					if (MediaWindow.Visible)
					{                    
						if (true)
						{
							#region Mouse rotation
							if (MediaMeshEN != null)
							{
								// Scale mouse position to back-buffer co-ordinates
								Point MousePos = RenderingPanel.PointToClient(Cursor.Position);
								int MouseX = MousePos.X;// (MousePos.X * 1024) / RenderingPanel.Size.Width;

								// Mouse button down?
								if (KeyState.Get(Keys.LButton))
								{
									if (!MouseDragging)
									{
										OldMouseX = Cursor.Position.X;
										OldMouseY = Cursor.Position.Y;
										MouseDragging = true;
									}
									else
									{
										// Rotate whit ... ( Marian Voicu )
										int MouseMoveX = Cursor.Position.X - OldMouseX;
										int MouseMoveY = Cursor.Position.Y - OldMouseY;
										OldMouseX = Cursor.Position.X;
										OldMouseY = Cursor.Position.Y;
										MediaMeshEN.Turn(Delta * MouseMoveY * 2f, Delta * (float)MouseMoveX * 2f, 0f);
										// end ( MV )
									}
								}
								else
								{
									MouseDragging = false;
								}
							}
							#endregion
                      
							//TabMedia.Focus();
						}
					}
					if (ControlHost.SelectedIndex == (int) GETab.ACTORS &&
						ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.ACTORS &&
						ActorSubTabs.SelectedTab == ActorSubTabPreview && ActorPreview != null && PanelMouseHover)
					{
						#region Mouse rotation
						// Scale mouse position to back-buffer co-ordinates
						Point MousePos = RenderingPanel.PointToClient(Cursor.Position);
						int MouseX = MousePos.X;// (MousePos.X * 1024) / RenderingPanel.Size.Width;

						// Mouse button down?
						if (KeyState.Get(Keys.LButton))
						{
							if (!MouseDragging)
							{
								OldMouseX = Cursor.Position.X;
								OldMouseY = Cursor.Position.Y;
								MouseDragging = true;
							}
							else
							{
								// Rotate whit ... ( Marian Voicu )
								int MouseMoveX = Cursor.Position.X - OldMouseX;
								int MouseMoveY = Cursor.Position.Y - OldMouseY;
								OldMouseX = Cursor.Position.X;
								OldMouseY = Cursor.Position.Y;
								((Entity) ActorPreview.CollisionEN).Turn(Delta * (float) MouseMoveY * 2f,
																		 Delta * (float) MouseMoveX * 2f, 0f);
								// end ( MV )
							}
						}
						else
						{
							MouseDragging = false;
						}
						#endregion

						#region Keyboard Zoom in/out and rotation
						if (KeyState.Get(Keys.A)) // Zoom in
						{
							((Entity) ActorPreview.CollisionEN).Translate(0f, 0f, Delta * -2f);
						}
						else if (KeyState.Get(Keys.Z)) // Zoom out
						{
							((Entity) ActorPreview.CollisionEN).Translate(0f, 0f, Delta * 2f);
						}
						else if (KeyState.Get(Keys.C)) // Reset position
						{
							((Entity) ActorPreview.CollisionEN).Position(0f, 0f, 65f);
							((Entity) ActorPreview.CollisionEN).Rotate(0f, 0f, 0f);
						}
						#endregion
					}
					// Animation preview controls ( Marian Voicu )

					if (ControlHost.SelectedIndex == (int) GETab.ACTORS &&
						ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.ANIMATION)
						// && PanelMouseHover)
					{
						if (PanelMouseHover)
						{
							#region Mouse rotation
							// Scale mouse position to back-buffer co-ordinates
							Point MousePos = RenderingPanel.PointToClient(Cursor.Position);
							int MouseX = MousePos.X;// (MousePos.X * 1024) / RenderingPanel.Size.Width;

							// Mouse button down?
							if (KeyState.Get(Keys.LButton))
							{
								if (!MouseDragging)
								{
									OldMouseX = Cursor.Position.X;
									OldMouseY = Cursor.Position.Y;
									MouseDragging = true;
								}
								else
								{
									// Rotate whit ... ( Marian Voicu )
									int MouseMoveX = Cursor.Position.X - OldMouseX;
									int MouseMoveY = Cursor.Position.Y - OldMouseY;
									OldMouseX = Cursor.Position.X;
									OldMouseY = Cursor.Position.Y;
									if (ActorPreviewAnimation != null)
									{
										((Entity) ActorPreviewAnimation.CollisionEN).Turn(
											Delta * (float) MouseMoveY * 2f,
											Delta * (float) MouseMoveX * 2f,
											0f);
									}
									// end ( MV )
								}
							}
							else
							{
								MouseDragging = false;
							}

							//if (
							#endregion

							#region Keyboard Zoom in/out and rotation
							if (KeyState.Get(Keys.A)) // Zoom in
							{
								((Entity) ActorPreviewAnimation.CollisionEN).Translate(0f, 0f, Delta * -2f);
							}
							else if (KeyState.Get(Keys.Z)) // Zoom out
							{
								((Entity) ActorPreviewAnimation.CollisionEN).Translate(0f, 0f, Delta * 2f);
							}
							else if (KeyState.Get(Keys.C)) // Reset position
							{
								((Entity) ActorPreviewAnimation.CollisionEN).Position(0f, 0f, 65f);
								((Entity) ActorPreviewAnimation.CollisionEN).Rotate(0f, 0f, 0f);
							}
							#endregion

							// Sets focus to the tab so mousewheel function works
							ActorsAnimsTab.Focus();
						}
					}
					// end ( MV )
					// Actor mesh adjustment ( Marian Voicu )
					if (ControlHost.SelectedIndex == (int) GETab.ACTORS &&
						ActorsTabControl.SelectedIndex == (int) ActorsTabControlSelectedIndex.MESH_ADJUSTMENT &&
						PanelMouseHover)
					{
						#region Mouse rotation
						// Scale mouse position to back-buffer co-ordinates
						Point MousePos = RenderingPanel.PointToClient(Cursor.Position);
						int MouseX = MousePos.X;// (MousePos.X * 1024) / RenderingPanel.Size.Width;

						// Mouse button down?
						if (KeyState.Get(Keys.LButton))
						{
							if (!MouseDragging)
							{
								OldMouseX = Cursor.Position.X;
								OldMouseY = Cursor.Position.Y;
								MouseDragging = true;
							}
							else
							{
								int MouseMoveX = Cursor.Position.X - OldMouseX;
								int MouseMoveY = Cursor.Position.Y - OldMouseY;
								OldMouseX = Cursor.Position.X;
								OldMouseY = Cursor.Position.Y;
							}
						}
						else
						{
							MouseDragging = false;
						}
						#endregion

						#region Keyboard Zoom in/out and rotation
						if (KeyState.Get(Keys.A)) // Zoom in
						{
							((Entity) GubbinToolPreview.CollisionEN).Translate(0f, 0f, Delta * -2f);
						}
						else if (KeyState.Get(Keys.Z)) // Zoom out
						{
							((Entity) GubbinToolPreview.CollisionEN).Translate(0f, 0f, Delta * 2f);
						}
						else if (KeyState.Get(Keys.C)) // Reset position
						{
							((Entity) GubbinToolPreview.CollisionEN).Position(0f, 0f, 65f);
							((Entity) GubbinToolPreview.CollisionEN).Rotate(0f, 0f, 0f);
						}
						#endregion
					}
					// end ( MV )
					// Zones tab controls

					#region oldzoneview
					/*           else if (ControlHost.SelectedIndex == (int)ControlHostSelectedIndex.WORLD &&
							 SetWorldButtonSelection < (int)WorldButtonSelection.EMITTERS && PanelMouseHover)
					//WorldTabs.SelectedIndex <= 7
					{
						#region Update camera position label
						int CamX = (int)Camera.X();
						int CamY = (int)Camera.Y();
						int CamZ = (int)Camera.Z();
						string CamPos = "Camera: " + CamX.ToString() + ", " + CamY.ToString() + ", " +
										CamZ.ToString();
						//   WorldCameraPositionA.Text = CamPos;
						//WorldCameraPositionA.Location = new Point(6, 4);
						#endregion

						#region Mouselook
						if (KeyState.Get(Keys.Space))
						{
							// Enter mouselook mode
							if (!Mouselooking)
							{
								Mouselooking = true;
								Cursor.Position =
									World3DView.PointToScreen(new Point(World3DView.Size.Width / 2,
																		World3DView.Size.Height / 2));
								OldMouseX = Cursor.Position.X;
								OldMouseY = Cursor.Position.Y;
								Cursor.Hide();
							}
							// Continue mouselook mode
							else
							{
								// Rotate camera
								int MouseMoveX = Cursor.Position.X - OldMouseX;
								int MouseMoveY = Cursor.Position.Y - OldMouseY;
								WorldCamDPitch += MouseMoveY;
								WorldCamDYaw -= MouseMoveX;
								if (WorldCamDPitch > 89f)
								{
									WorldCamDPitch = 89f;
								}
								else if (WorldCamDPitch < -89f)
								{
									WorldCamDPitch = -89f;
								}
								WorldCamPitch = Lerp(WorldCamPitch, WorldCamDPitch, 2f);
								WorldCamYaw = Lerp(WorldCamYaw, WorldCamDYaw, 2f);
								if (DoBeep == true &&
									(Camera.Pitch() != WorldCamPitch || Camera.Yaw() != WorldCamYaw))
								{
									SystemSounds.Beep.Play();
									DoBeep = false;
								}
								Camera.Rotate(WorldCamPitch, WorldCamYaw, 0f);
								// Move camera
								if (KeyState.Get(Keys.LButton))
								{
									Camera.Move(0f, 0f, 25f * Delta * CameraSpeed);
									if (DoBeep == true)
									{
										SystemSounds.Beep.Play();
										DoBeep = false;
									}
								}
								else if (KeyState.Get(Keys.RButton))
								{
									Camera.Move(0f, 0f, -25f * Delta * CameraSpeed);
									if (DoBeep == true)
									{
										SystemSounds.Beep.Play();
										DoBeep = false;
									}
								}
								WorldCamX = Camera.X();
								WorldCamY = Camera.Y();
								WorldCamZ = Camera.Z();
								RepositionGrid();
								// Position mouse at screen centre
								Cursor.Position =
									World3DView.PointToScreen(new Point(World3DView.Size.Width / 2,
																		World3DView.Size.Height / 2));
								OldMouseX = Cursor.Position.X;
								OldMouseY = Cursor.Position.Y;
							}
						}
						// Leave mouselook mode
						else if (Mouselooking)
						{
							Cursor.Show();
							Mouselooking = false;
							DoBeep = false;
						}
						#endregion

						#region Update sky spheres
						if (CurrentClientZone != null)
						{
							CurrentClientZone.Stars.Position(Camera.X(), Camera.Y(), Camera.Z());
							CurrentClientZone.Cloud.Position(Camera.X(), Camera.Y(), Camera.Z());
						}
						#endregion

						#region Tool shortcuts
						if (ZoneShortcutWasDown)
						{
							if (!KeyState.Get(Keys.F1) && !KeyState.Get(Keys.F2) && !KeyState.Get(Keys.F3) &&
								!KeyState.Get(Keys.F4) && !KeyState.Get(Keys.F5))
							{
								ZoneShortcutWasDown = false;
							}
						}
						else
						{
							if (KeyState.Get(Keys.F1))
							{
								//WorldTabs.SelectedIndex = 0;
								RenderingPanel.Focus();
								ZoneShortcutWasDown = true;
							}
							else if (KeyState.Get(Keys.F2))
							{
								//WorldTabs.SelectedIndex = 1;
								RenderingPanel.Focus();
								ZoneShortcutWasDown = true;
							}
							else if (KeyState.Get(Keys.F3))
							{
								//WorldTabs.SelectedIndex = 2;
								RenderingPanel.Focus();
								ZoneShortcutWasDown = true;
							}
							else if (KeyState.Get(Keys.F4))
							{
								//WorldTabs.SelectedIndex = 3;
								RenderingPanel.Focus();
								ZoneShortcutWasDown = true;
							}
							else if (KeyState.Get(Keys.F5))
							{
								//WorldTabs.SelectedIndex = 4;
								RenderingPanel.Focus();
								ZoneShortcutWasDown = true;
							}
						}
						#endregion

						#region Cursor/A/Z keys and mouse dragging
						if (ZoneSelected.Count > 0 && !Mouselooking && !KeyState.Get(Keys.ControlKey))
						{
							// Scale mouse position to back-buffer co-ordinates
							Point MousePos = RenderingPanel.PointToClient(Cursor.Position);
							if (RenderingPanel.IsAccessible)
							{
								int MouseX = (MousePos.X * 1024) / RenderingPanel.Size.Width;
								int MouseY = (MousePos.Y * 768) / RenderingPanel.Size.Height;
							}

							// Get movement speed scale
							float Speed = 2f;
							if (KeyState.Get(Keys.ShiftKey))
							{
								Speed = 0.2f;
							}

							// Selected tab
							switch (SetWorldButtonSelection)
							{
								// Movement tab
								case (int)WorldButtonSelection.MOVE:
									// Keys
									if (KeyState.Get(Keys.Up))
									{
										MoveSelection(0f, 0f, Delta * Speed);
									}
									if (KeyState.Get(Keys.Down))
									{
										MoveSelection(0f, 0f, Delta * -Speed);
									}
									if (KeyState.Get(Keys.Right))
									{
										MoveSelection(Delta * Speed, 0f, 0f);
									}
									if (KeyState.Get(Keys.Left))
									{
										MoveSelection(Delta * -Speed, 0f, 0f);
									}
									if (KeyState.Get(Keys.A))
									{
										MoveSelection(0f, Delta * Speed, 0f);
									}
									if (KeyState.Get(Keys.Z))
									{
										MoveSelection(0f, Delta * -Speed, 0f);
									}
									// Mouse
									/* if (KeyState.Get(Keys.RButton))
									 {
										 SetSelectionPickMode(0);
										 Entity E = Collision.CameraPick(Camera, MouseX, MouseY);
										 if (E != null)
											 PositionSelection(Collision.PickedX(), Collision.PickedY(), Collision.PickedZ());
										 SetSelectionPickMode(2);
									 }
									if (KeyState.Get(Keys.RButton))
									{
										if (!MouseDragging)
										{
											OldMouseX = Cursor.Position.X;
											OldMouseY = Cursor.Position.Y;
											MouseDragging = true;
										}
										else
										{
											int MouseMoveX = Cursor.Position.X - OldMouseX;
											int MouseMoveY = Cursor.Position.Y - OldMouseY;
											OldMouseX = Cursor.Position.X;
											OldMouseY = Cursor.Position.Y;
											MoveSelection(Delta * Speed * MouseMoveX, 0f, Delta * Speed * MouseMoveY);
										}
									}
									/*else if (KeyState.Get(Keys.RButton))
								{
									if (!MouseDragging)
									{
										OldMouseX = Cursor.Position.X;
										MouseDragging = true;
									}
									else
									{
										int MouseMoveX = Cursor.Position.X - OldMouseX;
										OldMouseX = Cursor.Position.X;
										if (OldMouseX > 0)
											TurnSelection(0f, Delta * (float)MouseMoveX, 0f);
									}
								}
									else
									{
										MouseDragging = false;
									}

									break;
								// Rotation tab
								case (int)WorldButtonSelection.ROTATE:
									// Keys
									if (KeyState.Get(Keys.Up))
									{
										TurnSelection(Delta * -Speed, 0f, 0f);
									}
									if (KeyState.Get(Keys.Down))
									{
										TurnSelection(Delta * Speed, 0f, 0f);
									}
									if (KeyState.Get(Keys.Right))
									{
										TurnSelection(0f, Delta * Speed, 0f);
									}
									if (KeyState.Get(Keys.Left))
									{
										TurnSelection(0f, Delta * -Speed, 0f);
									}
									if (KeyState.Get(Keys.A))
									{
										TurnSelection(0f, 0f, Delta * Speed);
									}
									if (KeyState.Get(Keys.Z))
									{
										TurnSelection(0f, 0f, Delta * -Speed);
									}
									// mouse
									if (KeyState.Get(Keys.RButton))
									{
										if (!MouseDragging)
										{
											OldMouseX = Cursor.Position.X;
											MouseDragging = true;
										}
										else
										{
											int MouseMoveX = Cursor.Position.X - OldMouseX;
											OldMouseX = Cursor.Position.X;
											if (OldMouseX > 0)
											{
												TurnSelection(0f, Delta * (float)MouseMoveX, 0f);
											}
										}
									}
									else
									{
										MouseDragging = false;
									}
									break;
								// Scaling tab
								case (int)WorldButtonSelection.SCALE:
									// Keys                                    
									if (KeyState.Get(Keys.Up))
									{
										ScaleSelection(0f, 0f, Delta * Speed * 0.01f);
									}
									if (KeyState.Get(Keys.Down))
									{
										ScaleSelection(0f, 0f, Delta * Speed * -0.01f);
									}
									if (KeyState.Get(Keys.Right))
									{
										ScaleSelection(Delta * Speed * 0.01f, 0f, 0f);
									}
									if (KeyState.Get(Keys.Left))
									{
										ScaleSelection(Delta * Speed * -0.01f, 0f, 0f);
									}
									if (KeyState.Get(Keys.A))
									{
										ScaleSelection(0f, Delta * Speed * 0.01f, 0f);
									}
									if (KeyState.Get(Keys.Z))
									{
										ScaleSelection(0f, Delta * Speed * -0.01f, 0f);
									}
									// Mouse
									if (KeyState.Get(Keys.RButton))
									{
										if (!MouseDragging)
										{
											OldMouseX = Cursor.Position.X;
											MouseDragging = true;
										}
										else
										{
											int MouseMoveX = Cursor.Position.X - OldMouseX;
											OldMouseX = Cursor.Position.X;
											if (OldMouseX > 0)
											{
												float Scale = Delta * (float)MouseMoveX * 0.07f;
												ScaleSelection(Scale, Scale, Scale);
											}
										}
									}
									else
									{
										MouseDragging = false;
									}
									break;
							}
						}
						#endregion

						#region Page Up/Down to move grid
						if (KeyState.Get(Keys.PageUp))
						{
							GridHeight += Delta;
							RepositionGrid();
						}
						else if (KeyState.Get(Keys.PageDown))
						{
							GridHeight -= Delta;
							RepositionGrid();
						}
						else if (KeyState.Get(Keys.F12))
						{
							GridHeight = 0f;
							RepositionGrid();
						}
						#endregion

						#region Object deletion
						if (RenderingPanel.Focused && KeyState.Get(Keys.Delete))
						{
							if (ZoneSelected.Count > 0 && !Mouselooking)
							{
								// Warn if deleting more than 10 objects at once
								DialogResult Result = DialogResult.OK;
								if (ZoneSelected.Count > 10)
								{
									Result =
										MessageBox.Show(
											"Really delete these " + ZoneSelected.Count.ToString() + " objects?",
											"Realm Crafter", MessageBoxButtons.OKCancel);
								}

								// Delete each object
								if (Result == DialogResult.OK)
								{
									// Create undo point
									LinkedList<ZoneObject> UndoList = new LinkedList<ZoneObject>();
									Undo U = new Undo(Undo.Actions.Delete, UndoList);

									ZoneObject Obj;
									for (int i = 0; i < ZoneSelected.Count; ++i)
									{
										Obj = (ZoneObject)ZoneSelected[i];
										// Add to list of deleted objects in undo info
										UndoList.AddLast(Obj);
										// Remove from selection list
										ClearSelectionBox(Obj.EN);
										Program.GE.m_ZoneList.ZoneObjectListRemove(Obj);
										// Free object type specific stuff
										if (Obj is Scenery)
										{
											Scenery S = Obj as Scenery;
											if (S.SceneryID > 0)
											{
												CurrentServerZone.Instances[0].OwnedScenery[S.SceneryID - 1] = null;
											}
										}
										else if (Obj is Emitter)
										{
											RottParticles.General.FreeEmitter(Obj.EN.GetChild(1), true, false);
										}
										else if (Obj is Water)
										{
											Water W = (Water)Obj;
											WaterArea.WaterList.Remove(W.ServerWater);
										}
										else if (Obj is Trigger)
										{
											Trigger T = Obj as Trigger;
											CurrentServerZone.TriggerScript[T.ServerID] = "";
											CurrentServerZone.TriggerMethod[T.ServerID] = "";
										}
										else if (Obj is Waypoint)
										{
											Waypoint W = Obj as Waypoint;
											// Find waypoints connected to this one, and remove the connections
											for (int j = 0; j < 2000; ++j)
											{
												if (CurrentServerZone.NextWaypointA[j] == W.ServerID)
												{
													WaypointLinkAEN[j].Free();
													WaypointLinkAEN[j] = null;
													CurrentServerZone.NextWaypointA[j] = 2000;
												}
												if (CurrentServerZone.NextWaypointB[j] == W.ServerID)
												{
													WaypointLinkBEN[j].Free();
													WaypointLinkBEN[j] = null;
													CurrentServerZone.NextWaypointB[j] = 2000;
												}
											}
											// Remove spawn point if there is one
											int SpawnNum = CurrentServerZone.GetSpawnPoint(W.ServerID);
											if (SpawnNum > -1)
											{
												CurrentServerZone.SpawnMax[SpawnNum] = 0;
												CurrentServerZone.SpawnFrequency[SpawnNum] = 10;
												CurrentServerZone.SpawnSize[SpawnNum] = 0f;
												CurrentServerZone.SpawnRange[SpawnNum] = 0f;
												CurrentServerZone.SpawnScript[SpawnNum] = "";
												CurrentServerZone.SpawnActorScript[SpawnNum] = "";
												CurrentServerZone.SpawnDeathScript[SpawnNum] = "";
											}
											// Remove this waypoint
											CurrentServerZone.PrevWaypoint[W.ServerID] = 2005;
											CurrentServerZone.NextWaypointA[W.ServerID] = 2005;
											CurrentServerZone.NextWaypointB[W.ServerID] = 2005;
											if (WaypointLinkAEN[W.ServerID] != null)
											{
												WaypointLinkAEN[W.ServerID].Free();
											}
											if (WaypointLinkBEN[W.ServerID] != null)
											{
												WaypointLinkBEN[W.ServerID].Free();
											}
											WaypointLinkAEN[W.ServerID] = null;
											WaypointLinkBEN[W.ServerID] = null;
										}
										else if (Obj is Portal)
										{
											Portal P = Obj as Portal;
											CurrentServerZone.PortalName[P.ServerID] = "";
											CurrentServerZone.PortalLinkArea[P.ServerID] = "";
											CurrentServerZone.PortalLinkName[P.ServerID] = "";
										}
										// Remove from zone
										RemoveObject(Obj);
										// Hide entity
										Obj.EN.Visible = false;
									}
									ZoneSelected.Clear();
									WorldSelectedObjectsLabel.Text = "Selected objects: 0";
									SetWorldSavedStatus(false);
								}
							}
						}
						#endregion
					*/
					#endregion

					#region GUI_Update (by Silva 2009)
					if (ControlHost.SelectedIndex == (int)GETab.WORLD) // if TAB world OPEN
					{
						Point Mouse = RenderingPanel.PointToClient(Cursor.Position);
						Parameters.MousePosition = new NVector2(Mouse.X, Mouse.Y);
						GUIManager.Update(Parameters);
						Parameters.InputBuffer.Clear();
					}
					#endregion

					if (RenderingPanelCurrentIndex == (int)GETab.ACTORS)
					{
						ActorInstance PInst = null;

						if (ActorsTabControl.SelectedIndex == (int)ActorsTabControlSelectedIndex.ACTORS)
							PInst = ActorPreview;
						else if (ActorsTabControl.SelectedIndex == (int)ActorsTabControlSelectedIndex.ANIMATION)
							PInst = ActorPreviewAnimation;

						// Update lights
						if (PInst != null && PInst.EN != null)
						{
							foreach (GubbinPreviewInstance P in PInst.GubbinEN)
							{
								P.Update(PInst.EN as Entity);
							}

							foreach (GubbinPreviewInstance P in PInst.HatENs)
							{
								P.Update(PInst.EN as Entity);
							}

							foreach (GubbinPreviewInstance P in PInst.BeardENs)
							{
								P.Update(PInst.EN as Entity);
							}

							foreach (GubbinPreviewInstance P in PInst.ShieldENs)
							{
								P.Update(PInst.EN as Entity);
							}

							foreach (GubbinPreviewInstance P in PInst.WeaponENs)
							{
								P.Update(PInst.EN as Entity);
							}

							foreach (GubbinPreviewInstance P in PInst.ChestENs)
							{
								P.Update(PInst.EN as Entity);
							}

						}
					}



					General.Update(Delta);
					GE.TerrainManager.Update(new NVector3(Program.GE.Camera.X(), Program.GE.Camera.Y(), Program.GE.Camera.Z()),
						!(KeyState.Get(Keys.Space) && (KeyState.Get(Keys.LButton) || KeyState.Get(Keys.RButton))));
					Render.RenderWorld();
					Collision.UpdateWorld();
					if (FPS > 80)
					{
						System.Threading.Thread.Sleep(5);
					}
					else if (FPS > 40)
					{
						System.Threading.Thread.Sleep(2);
					}
					else if (FPS > 20)
					{
						System.Threading.Thread.Sleep(1);
					}
				}
				System.Threading.Thread.Sleep(10);
			}
		}
		#endregion

		#region Panels Toolbar

		private void btPostProcess_Click(object sender, EventArgs e)
		{
			if (m_PostProcess.Visible) m_PostProcess.Hide();
			else m_PostProcess.Show(dockPostProcess);
		}

		private void WorldToolCreateProperty_Click(object sender, EventArgs e)
		{
			if (m_propertyWindow.Visible == false)
			{
				m_propertyWindow.Show();
			}
			else
			{
				m_propertyWindow.Hide();
			}
		}

		private void WorldToolCreateList_Click(object sender, EventArgs e)
		{
			if (m_ZoneList.Visible == false)
			{
				m_ZoneList.Show(WorldDock);
			}
			else
			{
				m_ZoneList.Hide();
			}
		}

		private void WorldToolCreateRender_Click(object sender, EventArgs e)
		{
			if (m_ZoneRender.Visible == false)
			{
				m_ZoneRender.Show();
			}
			else
			{
				m_ZoneRender.Hide();
			}
		}

		private void SimpleScripter_Click(object sender, EventArgs e)
		{
			if (m_ScriptView.Visible == false)
			{
				m_ScriptView.Show();
			}
			else
			{
				m_ScriptView.Hide();
			}
		}

		private void WorldCreateWindow_Click(object sender, EventArgs e)
		{
			if (m_CreateWindow.Visible == false)
			{
				m_CreateWindow.Show();
			}
			else
			{
				m_CreateWindow.Hide();
			}
		}

		private void InterfaceHierarchy_Click(object sender, EventArgs e)
		{
			if (m_InterfaceHierarchy.Visible)   m_InterfaceHierarchy.Hide();
			else                                m_InterfaceHierarchy.Show(WorldDock);
		}

		private void InterfaceEditor_Click(object sender, EventArgs e)
		{
			if (m_Interface.Visible)    m_Interface.Hide();
			else                        m_Interface.Show(WorldDock);
		}

		private void ZoneSetup_Click(object sender, EventArgs e)
		{
			ZoneSetupForm.Show();
			ZoneSetupForm.BringToFront();
		}

		private void WorldYearSetup_Click(object sender, EventArgs e)
		{
			YearSetup.Show();
			YearSetup.BringToFront();
		}

		#endregion

		#region Tools toolbar
		private void ToolsToolRefreshScripts_Click(object sender, EventArgs e)
		{
			RefreshScripts();
		}
		#endregion

		#region Objects toolbar
		public void ObjectMove_Click(object sender, EventArgs e)
		{
			m_ZoneRender.MouseDragging = false;
			ObjectMove.Checked = true;
			ObjectRotate.Checked = false;
			ObjectScale.Checked = false;
			SetWorldButtonSelection = (int) WorldButtonSelection.MOVE;
			m_CreateWindow.WorldPlaceObjectTypeCombo.SelectedIndex = -1;

			if (Program.Transformer != null)
				Program.Transformer.Free();
			Program.Transformer = null;

			if (Program.GE.ZoneSelected.Count > 0)
				if (Program.GE.ZoneSelected[0] is ZoneObject)
					(Program.GE.ZoneSelected[0] as ZoneObject).MoveInit();

			if (RenderingPanelCurrentIndex == -5)
				TreeRender.UpdateWorldButtonSelection();
			if (RenderingPanelCurrentIndex == -7)
				m_GubbinEditor.UpdateWorldButtonSelection();
		}

		public void ObjectRotate_Click(object sender, EventArgs e)
		{
			m_ZoneRender.MouseDragging = false;
			ObjectMove.Checked = false;
			ObjectRotate.Checked = true;
			ObjectScale.Checked = false;
			SetWorldButtonSelection = (int) WorldButtonSelection.ROTATE;
			m_CreateWindow.WorldPlaceObjectTypeCombo.SelectedIndex = -1;

			if (Program.Transformer != null)
				Program.Transformer.Free();
			Program.Transformer = null;

			if (Program.GE.ZoneSelected.Count > 0)
				if (Program.GE.ZoneSelected[0] is ZoneObject)
					(Program.GE.ZoneSelected[0] as ZoneObject).RotateInit();


			if (RenderingPanelCurrentIndex == -5)
				TreeRender.UpdateWorldButtonSelection();
			if (RenderingPanelCurrentIndex == -7)
				m_GubbinEditor.UpdateWorldButtonSelection();
		}

		public void ObjectScale_Click(object sender, EventArgs e)
		{
			m_ZoneRender.MouseDragging = false;
			ObjectMove.Checked = false;
			ObjectRotate.Checked = false;
			ObjectScale.Checked = true;
			SetWorldButtonSelection = (int) WorldButtonSelection.SCALE;
			m_CreateWindow.WorldPlaceObjectTypeCombo.SelectedIndex = -1;
			
			if (Program.Transformer != null)
				Program.Transformer.Free();
			Program.Transformer = null;

			if (Program.GE.ZoneSelected.Count > 0)
				if (Program.GE.ZoneSelected[0] is ZoneObject)
					(Program.GE.ZoneSelected[0] as ZoneObject).ScaleInit();

			if (RenderingPanelCurrentIndex == -5)
				TreeRender.UpdateWorldButtonSelection();
			if (RenderingPanelCurrentIndex == -7)
				m_GubbinEditor.UpdateWorldButtonSelection();
		}

		public void UncheckObjectControls()
		{
			ObjectMove.Checked = false;
			ObjectRotate.Checked = false;
			ObjectScale.Checked = false;
			if (RenderingPanelCurrentIndex == -5)
				TreeRender.UpdateWorldButtonSelection();
			if (RenderingPanelCurrentIndex == -7)
				m_GubbinEditor.UpdateWorldButtonSelection();
		}
		#endregion

		#region Waypoint toolbar
		private void WaypointLinkingModeA_Click(object sender, EventArgs e)
		{
			Program.GE.m_ZoneRender.WaypointLinkMode = 1;
			Program.GE.m_ZoneRender.LinkingWaypoint = (Waypoint) ZoneSelected[0];
		}

		private void WaypointRemoveA_Click(object sender, EventArgs e)
		{
			Waypoint W = (Waypoint) ZoneSelected[0];
			if (W.NextA != null)
			{
				W.NextA.Prev = null;
				W.NextA = null;

				if (W.WaypointLinkAEN != null)
				{
					W.WaypointLinkAEN.Free();
					W.WaypointLinkAEN = null;
				}
				SetWorldSavedStatus(false);
				RenderingPanel.Focus();
			}
		}

		private void WaypointLinkingModeB_Click(object sender, EventArgs e)
		{
			Program.GE.m_ZoneRender.WaypointLinkMode = 2;
			Program.GE.m_ZoneRender.LinkingWaypoint = (Waypoint) ZoneSelected[0];
		}

		private void WaypointRemoveB_Click(object sender, EventArgs e)
		{
			Waypoint W = (Waypoint) ZoneSelected[0];
			if (W.NextB != null)
			{
				if(W.NextB.Prev == W)
					W.NextB.Prev = null;
				W.NextB = null;

				if (W.WaypointLinkBEN != null)
				{
					W.WaypointLinkBEN.Free();
					W.WaypointLinkBEN = null;
				}

				SetWorldSavedStatus(false);
				RenderingPanel.Focus();
			}
		}
		#endregion

		#region Zones toolbar
		private void WorldNewZone_Click(object sender, EventArgs e)
		{
			DialogResult Result;

			#region Entire new zone
			// Save current world state?
			if (!Program.GE.WorldSaved)
			{
				Result = MessageBox.Show("Save changes to current zone first?", "New zone",
										 MessageBoxButtons.YesNoCancel);
				if (Result == DialogResult.Yes)
				{
					Program.GE.SaveWorld();
				}
				else if (Result == DialogResult.Cancel)
				{
					return;
				}
			}

			// Get name for new zone
			TextEntry TE = new TextEntry();
			TE.Text = "Create zone";
			TE.Description.Text = "Enter a name for the new zone:";
			TE.ShowDialog();
			string ZoneName = TE.Result;
			TE.Dispose();
			if (string.IsNullOrEmpty(ZoneName) || string.IsNullOrEmpty(ZoneName.Replace(" ", "")))
			{
				return;
			}

			// Check a zone with this name doesn't already exist
			string ExistingZoneName;
			/*   for (int i = 0; i < Program.GE.m_ZoneList.WorldZonesTree.GetNodeCount(false); ++i)
			   {
				   ExistingZoneName = (string)Program.GE.m_ZoneList.WorldZonesTree.Nodes[i].Name;
				   if (ExistingZoneName.ToUpper() == ZoneName.ToUpper())
				   {
					   MessageBox.Show("A zone with that name already exists.", "Error");
					   return;
				   }
			   }*/
			string[] ZoneFiles = Directory.GetFiles(@"Data\Areas\", "*.dat");
			foreach (string ZF in ZoneFiles)
			{
				ExistingZoneName = Path.GetFileNameWithoutExtension(ZF);
				if (ExistingZoneName.ToUpper() == ZoneName.ToUpper())
				{
					MessageBox.Show("A zone with that name already exists.", "Error");
					return;
				}
			}

			// Unload current zone, if any
			Program.GE.UnloadCurrentZone();

			// Create new zone
			Program.GE.CurrentClientZone = new Zone(ZoneName);
			Program.GE.CurrentServerZone = new RealmCrafter.ServerZone.Zone(ZoneName);
			Program.GE.CurrentClientZone.Save();
			Program.GE.CurrentServerZone.Save(Program.GE.CurrentClientZone);

			// Add to UI
			Program.GE.TotalZones++;
			Program.GE.ActorStartZoneCombo.Items.Add(ZoneName);
			//Program.GE.WorldPlacePortalLinkCombo.Items.Add(ZoneName);
			//Program.GE.WorldObjectPortalLinkCombo.Items.Add(ZoneName);
			//Program.GE.WorldZoneWeatherLinkCombo.Items.Add(ZoneName);
			//Program.GE.ProjectZones.Text = "Zones: " + Program.GE.TotalZones;

			// Reload tree
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Clear();
			Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.Items.Clear();
			Program.GE.TotalZones = 0;
			string[] Zones = Directory.GetFiles(@"Data\Areas\", "*.dat");
			ActorStartZoneCombo.Items.Clear();
			foreach (string S_ in Zones)
			{
				Program.GE.TotalZones++;
				string Name = Path.GetFileNameWithoutExtension(S_);

				Program.GE.ActorStartZoneCombo.Items.Add(Name);
				Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.Items.Add(Name);
				//Program.GE.WorldPlacePortalLinkCombo.Items.Add(Name);
				//Program.GE.WorldObjectPortalLinkCombo.Items.Add(Name);
				//Program.GE.WorldZoneWeatherLinkCombo.Items.Add(Name);
				Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add(Name);
				Program.GE.m_ZoneList.WorldZonesTree.Nodes[Program.GE.TotalZones - 1].Name = Name;
			}
			Program.GE.CurrentClientZone.SetViewDistance(Program.GE.Camera,
														 Program.GE.CurrentClientZone.FogNear,
														 Program.GE.CurrentClientZone.FogFar);

			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Clear();
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Scenery objects");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[0].Name = "Node0";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Terrains");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[1].Name = "Node1";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Emitters");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[2].Name = "Node2";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Water areas");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[3].Name = "Node3";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Collision boxes");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[4].Name = "Node4";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Sound zones");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[5].Name = "Node5";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Dynamic lights");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[6].Name = "Node6";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Triggers");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[7].Name = "Node7";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Waypoints");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[8].Name = "Node8";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Portals");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[9].Name = "Node9";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Tree Placers");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[10].Name = "Node10";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Trees");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[11].Name = "Node11";
			Program.GE.m_ZoneList.WorldZonesTree.Nodes.Add("Menu Controls");
			Program.GE.m_ZoneList.WorldZonesTree.Nodes[12].Name = "Node12";
			m_ZoneList.AddObjectsCount();
			// end reload
			Program.GE.UpdateZoneList();
			ZoneSetupForm.UpdateZoneSetup(false);
			// Reset Program.GE.Camera
			WorldCamDPitch = 0f;
			WorldCamDYaw = 0f;
			WorldCamPitch = 0f;
			WorldCamYaw = 0f;
			WorldCamX = 0f;
			WorldCamY = 100f;
			WorldCamZ = 0f;
			Program.GE.Camera.Position(0,100, 0);
			Program.GE.Camera.Rotate(0, 45, 0);
			#endregion
		}

		private void WorldDuplicateZone_Click(object sender, EventArgs e)
		{
			if (Program.GE.CurrentServerZone != null)
			{
				// Save
				if (!Program.GE.WorldSaved)
				{
					DialogResult Result = MessageBox.Show(
						"Would you like to save changes to this zone before making a copy?",
						"Realm Crafter",
						MessageBoxButtons.YesNoCancel);
					if (Result == DialogResult.Yes)
					{
						Program.GE.SaveWorld();
					}
					else if (Result == DialogResult.Cancel)
					{
						return;
					}
				}

				// Get name for new zone
				TextEntry TE = new TextEntry();
				TE.Text = "Duplicate zone";
				TE.Description.Text = "Enter a name for the new zone:";
				TE.ShowDialog();
				if (string.IsNullOrEmpty(TE.Result))
				{
					return;
				}

				string ZoneName = TE.Result;
				TE.Dispose();

				// Check a zone with this name doesn't already exist
				string ExistingZoneName;
				for (int i = 0; i < m_ZoneList.WorldZonesTree.GetNodeCount(false); ++i)
				{
					ExistingZoneName = m_ZoneList.WorldZonesTree.Name;
					if (ExistingZoneName.ToUpper() == ZoneName.ToUpper())
					{
						MessageBox.Show("A zone with that name already exists.", "Error");
						return;
					}
				}

				// Copy files
				GE.SafeCopyFile(@"Data\Areas\" + Program.GE.CurrentClientZone.Name + ".dat",
										@"Data\Areas\" + ZoneName + ".dat");
                GE.SafeCopyFile(@"Data\Areas\" + Program.GE.CurrentClientZone.Name + ".ltz",
										@"Data\Areas\" + ZoneName + ".ltz");
                GE.SafeCopyFile(@"Data\Server Data\Areas\" + Program.GE.CurrentServerZone.Name + ".dat",
										@"Data\Server Data\Areas\" + ZoneName + ".dat");
				for (int i = 0; i < 100; ++i)
				{
                    GE.SafeCopyFile(
						@"Data\Server Data\Areas\Ownerships\" + Program.GE.CurrentServerZone.Name + " (" + i.ToString() +
						").dat",
						@"Data\Server Data\Areas\Ownerships\" + ZoneName + " (" + i.ToString() + ").dat");
				}

				string PathExt = System.Environment.TickCount.ToString();

				foreach (RCTTerrain T in Program.GE.CurrentClientZone.Terrains)
				{
					string NewPath = T.Path;
					NewPath = NewPath.Replace(".te", "");
					NewPath += "_" + PathExt + ".te";
					//NewPath = NewPath.Replace(Program.GE.CurrentClientZone.Name, ZoneName);

					File.Copy(T.Path, NewPath, true);
				}

				// Unload current zone
				Program.GE.UnloadCurrentZone();

#region LoadNewZone
								m_ZoneList.WorldZonesTree.Nodes.Clear();
				m_ZoneList.WorldZonesTree.Nodes.Add("Scenery objects");
				m_ZoneList.WorldZonesTree.Nodes[0].Name = "Node0";
				m_ZoneList.WorldZonesTree.Nodes.Add("Terrains");
				m_ZoneList.WorldZonesTree.Nodes[1].Name = "Node1";
				m_ZoneList.WorldZonesTree.Nodes.Add("Emitters");
				m_ZoneList.WorldZonesTree.Nodes[2].Name = "Node2";
				m_ZoneList.WorldZonesTree.Nodes.Add("Water areas");
				m_ZoneList.WorldZonesTree.Nodes[3].Name = "Node3";
				m_ZoneList.WorldZonesTree.Nodes.Add("Collision boxes");
				m_ZoneList.WorldZonesTree.Nodes[4].Name = "Node4";
				m_ZoneList.WorldZonesTree.Nodes.Add("Sound zones");
				m_ZoneList.WorldZonesTree.Nodes[5].Name = "Node5";
				m_ZoneList.WorldZonesTree.Nodes.Add("Dynamic lights");
				m_ZoneList.WorldZonesTree.Nodes[6].Name = "Node6";
				m_ZoneList.WorldZonesTree.Nodes.Add("Triggers");
				m_ZoneList.WorldZonesTree.Nodes[7].Name = "Node7";
				m_ZoneList.WorldZonesTree.Nodes.Add("Waypoints");
				m_ZoneList.WorldZonesTree.Nodes[8].Name = "Node8";
				m_ZoneList.WorldZonesTree.Nodes.Add("Portals");
				m_ZoneList.WorldZonesTree.Nodes[9].Name = "Node9";
				m_ZoneList.WorldZonesTree.Nodes.Add("Tree Placers");
				m_ZoneList.WorldZonesTree.Nodes[10].Name = "Node10";
				m_ZoneList.WorldZonesTree.Nodes.Add("Trees");
				m_ZoneList.WorldZonesTree.Nodes[11].Name = "Node11";
				m_ZoneList.WorldZonesTree.Nodes.Add("Menu Controls");
				m_ZoneList.WorldZonesTree.Nodes[12].Name = "Node12";

				// Load new zone
				Program.GE.DefalutZoneNoSave = true;
				Program.GE.CurrentClientZone = Zone.Load(ZoneName, Program.GE.Camera,
														 true,
														 new LoadProgressUpdate(
															 Program.GE.UpdateZoneLoadProgress));
				Program.GE.CurrentServerZone = RealmCrafter.ServerZone.Zone.Load(ZoneName, Program.GE.CurrentClientZone);
				Environment3D.SetTime(12, 0);
				ZoneSetupForm.WorldZoneSetupTimeTrackbar.Value = 1440 / 2;
				ZoneSetupForm.WorldZoneSetupTimeTrackbar_Scroll(ZoneSetupForm.WorldZoneSetupTimeTrackbar, EventArgs.Empty);
				

				////WorldSetupDefalutZoneCheckBox.Checked = CurrentClientZone.DefaultZone;
				Program.GE.DefalutZoneNoSave = false;
				
				if (Program.GE.CurrentClientZone == null || Program.GE.CurrentServerZone == null)
				{
					MessageBox.Show("Error loading zone from file!", "Error");
					Program.GE.LoadingAZone = false;
					return;
				}

				#region Match water with server water
				Water W;
				WaterArea SW;
				for (int i = 0; i < Program.GE.CurrentClientZone.Waters.Count; ++i)
				{
					W = (Water) Program.GE.CurrentClientZone.Waters[i];


					for (int j = 0; j < WaterArea.WaterList.Count; ++j)
					{
						SW = (WaterArea) WaterArea.WaterList[j];
						if (SW.Zone == Program.GE.CurrentServerZone)
						{
							float ScaleX = W.EN.ScaleX();
							float ScaleZ = W.EN.ScaleZ();
							if (Math.Abs(SW.Width - ScaleX) <= float.Epsilon &&
								Math.Abs(SW.Depth - ScaleZ) <= float.Epsilon &&
								Math.Abs(SW.X - (W.EN.X() - (ScaleX * 0.5f))) <= float.Epsilon &&
								Math.Abs(SW.Z - (W.EN.Z() - (ScaleZ * 0.5f))) <= float.Epsilon &&
								Math.Abs(SW.Y - W.EN.Y()) <= float.Epsilon)
							{
								W.ServerWater = SW;
								break;
							}
						}
					}

					// A match could not be found - delete this water area
					if (W.ServerWater == null)
					{
						for (int it = 0; i < 4; ++i)
						{
							Media.UnloadTexture((int)W.TextureID[it]);
							//Render.FreeTexture((uint)W.TexHandle[it]);
						}
						W.EN.Free();
						Program.GE.CurrentClientZone.RemoveObject(W);
					}
				}
				#endregion

				#region Add objects to tree view and set collision types
				Media.LockMeshes();
				Media.LockTextures();
				Media.LockSounds();
				string Name;
				Scenery S;
				//RCTTerrain T;
				Emitter EM;
				ColBox CB;
				SoundZone SZ;
				RealmCrafter.ClientZone.Light LT;

				for (int i = 0; i < Program.GE.CurrentClientZone.Sceneries.Count; ++i)
				{
					S = (Scenery) Program.GE.CurrentClientZone.Sceneries[i];
					if (Collision.EntityType(S.EN) == (int) CollisionType.Sphere)
					{
						Collision.SetCollisionMesh(S.EN);
						Collision.EntityPickMode(S.EN, PickMode.Polygon);
					}
					else if (Collision.EntityType(S.EN) == (int) CollisionType.Box)
					{
						Collision.SetCollisionMesh(S.EN);
					}

					Name = Media.GetMeshName(S.MeshID);
					TreeNode TN =
						new TreeNode(Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
					TN.Tag = S;
					m_ZoneList.WorldZonesTree.Nodes[0].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Terrains.Count; ++i)
				{
					RCTTerrain T = (RCTTerrain) Program.GE.CurrentClientZone.Terrains[i];
					TreeNode TN = new TreeNode("Terrain " + (i + 1).ToString());
					TN.Tag = T;
					m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Emitters.Count; ++i)
				{
					EM = (Emitter) Program.GE.CurrentClientZone.Emitters[i];
					TreeNode TN;
					if (EM.Config != null)
					{
						TN = new TreeNode(EM.Config.Name);
					}
					else
					{
						TN = new TreeNode("Unknown emitter");
					}

					TN.Tag = EM;
					m_ZoneList.WorldZonesTree.Nodes[2].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Waters.Count; ++i)
				{
					W = (Water) Program.GE.CurrentClientZone.Waters[i];
					Collision.SetCollisionMesh(W.EN);
					Collision.EntityPickMode(W.EN, PickMode.Polygon);
					Name = Media.GetTextureName((int)W.TextureID[0]);
//                    Name = "ahhh";
					TreeNode TN =
						new TreeNode(Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
					TN.Tag = W;
					m_ZoneList.WorldZonesTree.Nodes[3].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.ColBoxes.Count; ++i)
				{
					CB = (ColBox) Program.GE.CurrentClientZone.ColBoxes[i];
					TreeNode TN = new TreeNode("Collision box " + (i + 1).ToString());
					TN.Tag = CB;
					m_ZoneList.WorldZonesTree.Nodes[4].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.SoundZones.Count; ++i)
				{
					SZ = (SoundZone) Program.GE.CurrentClientZone.SoundZones[i];
					SZ.EN.Texture(Program.GE.OrangeTex);
					if (SZ.SoundID < 65535)
					{
						Name = Media.GetSoundName(SZ.SoundID);
						if(Name.Length > 1)
							Name = Name.Substring(0, Name.Length - 1);
					}
					else if (SZ.MusicID < 65535)
					{
						Name = Media.GetMusicName(SZ.MusicID);
					}
					else
					{
						Name = "Unknown sound";
					}

					TreeNode TN = new TreeNode(Path.GetFileNameWithoutExtension(Name));
					TN.Tag = SZ;
					m_ZoneList.WorldZonesTree.Nodes[5].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Lights.Count; ++i)
				{
					LT = (RealmCrafter.ClientZone.Light)Program.GE.CurrentClientZone.Lights[i];
					TreeNode TN = new TreeNode("Light " + (i + 1).ToString());
					TN.Tag = LT;
					m_ZoneList.WorldZonesTree.Nodes[6].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.MenuControls.Count; ++i)
				{
					MenuControl Mc = (RealmCrafter.ClientZone.MenuControl)Program.GE.CurrentClientZone.MenuControls[i];
					TreeNode TN = new TreeNode("MenuControl " + (i + 1));
					TN.Tag = Mc;
					m_ZoneList.WorldZonesTree.Nodes[12].Nodes.Add(TN);
				}

				Media.UnlockMeshes();
				Media.UnlockTextures();
				Media.UnlockSounds();
				#endregion

				m_ZoneList.WorldZonesTree.CollapseAll();

				#region Create entities representing server side objects
				// Triggers
				for (int i = 0; i < CurrentServerZone.Triggers.Count; ++i)
				{
					Trigger Tr = CurrentServerZone.Triggers[i];
					if (!string.IsNullOrEmpty(Tr.Script))
					{
						TreeNode TN = new TreeNode(Tr.Script);
						TN.Tag = Tr;
						m_ZoneList.WorldZonesTree.Nodes[7].Nodes.Add(TN);
					}
				}

				// Portals
				for (int i = 0; i < CurrentServerZone.Portals.Count; ++i)
				{
					Portal P = CurrentServerZone.Portals[i];
					if (!string.IsNullOrEmpty(P.Name))
					{
						TreeNode TN = new TreeNode(P.Name);
						TN.Tag = P;
						m_ZoneList.WorldZonesTree.Nodes[9].Nodes.Add(TN);
					}
				}

				// Waypoints/spawn points
				for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
				{
					Waypoint WP = CurrentServerZone.Waypoints[i];
					WP.WaypointEN = WP.EN;
					TreeNode TN = new TreeNode("Waypoint " + (i).ToString());
					TN.Tag = WP;
					m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Add(TN);
				}

				// Waypoint links
				for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
				{
					Waypoint WP = CurrentServerZone.Waypoints[i];

					if (WP.NextA != null)
					{
						WP.WaypointLinkAEN = Program.GE.WaypointLinkTemplate.Copy();
						WP.WaypointLinkAEN.Shader = Shaders.LitObject1;
						WP.WaypointLinkAEN.Texture(Program.GE.OrangeTex);
					}
					if (WP.NextB != null)
					{
						WP.WaypointLinkBEN = Program.GE.WaypointLinkTemplate.Copy();
						WP.WaypointLinkBEN.Shader = Shaders.LitObject1;
						WP.WaypointLinkBEN.Texture(Program.GE.BlueTex);
					}
				}

				Program.GE.UpdateWaypointLinks();
				#endregion

				m_ZoneList.AddObjectsCount();
				Program.GE.ZoneSetupForm.UpdateZoneSetup(false);
				Program.GE.SetWorldSavedStatus(true);
				Program.GE.Camera.Position(0, 0, 0);
				Program.GE.WorldSelectedObjectsLabel.Text = "Selected objects: 0";
				Program.GE.GridHeight = 0f;
				Program.GE.RepositionGrid();
                Program.GE.UpdateRenderingPanel((int)GETab.NEWWORLD);
				Program.GE.LoadingAZone = false;
#endregion

				//RealmCrafter.ClientZone.Zone TempZone = RealmCrafter.ClientZone.Zone.Load(ZoneName, Program.GE.Camera, false, null);
				foreach (RCTTerrain T in Program.GE.CurrentClientZone.Terrains)
				{
					string NewPath = T.Path;
					NewPath = NewPath.Replace(".te", "");
					NewPath += "_" + PathExt + ".te";
					//NewPath = NewPath.Replace(Program.GE.CurrentClientZone.Name, ZoneName);

					T.Path = NewPath;
				}

				Program.GE.CurrentClientZone.Save();

				//DuplicateZoneTerrain.Duplicate(ZoneName, Program.GE.CurrentClientZone.Name);

				// Add to UI
				Program.GE.TotalZones++;
				Program.GE.ActorStartZoneCombo.Items.Add(ZoneName);
				Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.Items.Add(ZoneName);
				// Program.GE.WorldObjectPortalLinkCombo.Items.Add(ZoneName);

				// WorldZoneWeatherLinkCombo.Items.Add(ZoneName);
				//Program.GE.ProjectZones.Text = "Zones: " + Program.GE.TotalZones.ToString();
				m_ZoneList.AddObjectsCount();
				// refresh tree
				Program.GE.UpdateZoneList();
			}
		}

		private void WorldDeleteZone_Click(object sender, EventArgs e)
		{
			if (Program.GE.CurrentClientZone == null)
			{
				return;
			}

			DialogResult Result = MessageBox.Show("Really delete the zone: " + Program.GE.CurrentClientZone.Name + "?",
												  "Realm Crafter", MessageBoxButtons.YesNo);
			if (Result == DialogResult.Yes)
			{
				// Unload current zone
				string ZoneName = Program.GE.CurrentClientZone.Name;

				foreach (ZoneObject Obj in Program.GE.CurrentClientZone.Terrains)
				{
					if (Obj is RCTTerrain)
					{
						RCTTerrain T = Obj as RCTTerrain;

						try
						{
							File.Delete(T.Path);
						}
						catch (System.Exception ex)
						{
							MessageBox.Show(ex.Message);
						}
						
					}
				}

				Program.GE.UnloadCurrentZone();

				// Delete files from disk
				File.Delete(@"Data\Areas\" + ZoneName + ".dat");
				File.Delete(@"Data\Areas\" + ZoneName + ".ltz");
				File.Delete(@"Data\Server Data\Areas\" + ZoneName + ".dat");

                string envFile = @"Data\Areas\" + ZoneName + ".env";
                if(File.Exists(envFile))
                    File.Delete(envFile);

				for (int i = 0; i < 100; ++i)
				{
                    string ownershipFile = @"Data\Server Data\Areas\Ownerships\" + ZoneName + " (" + i.ToString() + ").dat";
					if (File.Exists(ownershipFile))
					{
                        File.Delete(ownershipFile);
					}
				}

				// Remove from UI
				Program.GE.TotalZones--;
				Program.GE.ActorStartZoneCombo.Items.Remove(ZoneName);
				Program.GE.m_CreateWindow.WorldPlacePortalLinkCombo.Items.Remove(ZoneName);
				////Program.GE.m_CreateWindow.WorldObjectPortalLinkCombo.Items.Remove(ZoneName);
				////WorldZoneWeatherLinkCombo.Items.Remove(ZoneName);
				Program.GE.UpdateZoneListEmpty();
				//Program.GE.ProjectZones.Text = "Zones: " + Program.GE.TotalZones.ToString();
				m_ZoneList.AddObjectsCount();
			}
		}

		private void WorldZones_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SupressChange)
			{
				return;
			}

			object OriginalSelection = WorldZones.SelectedItem;
			if (!Program.GE.LoadingAZone)
			{
				////Program.GE.nodeSelect = WorldZonesTreNode.Index;
				string ZoneToLoad = WorldZones.SelectedItem.ToString();
				////Save
				if (!Program.GE.WorldSaved)
				{
					DialogResult Result = MessageBox.Show(
						"Would you like to save changes to this zone first?",
						"Realm Crafter",
						MessageBoxButtons.YesNoCancel);
					if (Result == DialogResult.Yes)
					{
						Program.GE.SaveWorld();
					}
					else if (Result == DialogResult.Cancel)
					{
						SupressChange = true;
						if (CurrentClientZone != null)
						{
							WorldZones.Text = Program.GE.CurrentClientZone.Name;
						}
						SupressChange = false;
						return;
					}
				}

				Program.GE.SetWorldSavedStatus(true);
				Program.GE.LoadingAZone = true;
				////Unload old
				Program.GE.UnloadCurrentZone();

				m_ZoneList.WorldZonesTree.Nodes.Clear();
				m_ZoneList.WorldZonesTree.Nodes.Add("Scenery objects");
				m_ZoneList.WorldZonesTree.Nodes[0].Name = "Node0";
				m_ZoneList.WorldZonesTree.Nodes.Add("Terrains");
				m_ZoneList.WorldZonesTree.Nodes[1].Name = "Node1";
				m_ZoneList.WorldZonesTree.Nodes.Add("Emitters");
				m_ZoneList.WorldZonesTree.Nodes[2].Name = "Node2";
				m_ZoneList.WorldZonesTree.Nodes.Add("Water areas");
				m_ZoneList.WorldZonesTree.Nodes[3].Name = "Node3";
				m_ZoneList.WorldZonesTree.Nodes.Add("Collision boxes");
				m_ZoneList.WorldZonesTree.Nodes[4].Name = "Node4";
				m_ZoneList.WorldZonesTree.Nodes.Add("Sound zones");
				m_ZoneList.WorldZonesTree.Nodes[5].Name = "Node5";
				m_ZoneList.WorldZonesTree.Nodes.Add("Dynamic lights");
				m_ZoneList.WorldZonesTree.Nodes[6].Name = "Node6";
				m_ZoneList.WorldZonesTree.Nodes.Add("Triggers");
				m_ZoneList.WorldZonesTree.Nodes[7].Name = "Node7";
				m_ZoneList.WorldZonesTree.Nodes.Add("Waypoints");
				m_ZoneList.WorldZonesTree.Nodes[8].Name = "Node8";
				m_ZoneList.WorldZonesTree.Nodes.Add("Portals");
				m_ZoneList.WorldZonesTree.Nodes[9].Name = "Node9";
				m_ZoneList.WorldZonesTree.Nodes.Add("Tree Placers");
				m_ZoneList.WorldZonesTree.Nodes[10].Name = "Node10";
				m_ZoneList.WorldZonesTree.Nodes.Add("Trees");
				m_ZoneList.WorldZonesTree.Nodes[11].Name = "Node11";
				m_ZoneList.WorldZonesTree.Nodes.Add("Menu Controls");
				m_ZoneList.WorldZonesTree.Nodes[12].Name = "Node12";

				// Load new zone
				Program.GE.DefalutZoneNoSave = true;
				Program.GE.CurrentClientZone = Zone.Load(ZoneToLoad, Program.GE.Camera,
														 true,
														 new LoadProgressUpdate(
															 Program.GE.UpdateZoneLoadProgress));
				Program.GE.CurrentServerZone = RealmCrafter.ServerZone.Zone.Load(ZoneToLoad, Program.GE.CurrentClientZone);
				Environment3D.SetTime(12, 0);
				ZoneSetupForm.WorldZoneSetupTimeTrackbar.Value = 1440 / 2;
				ZoneSetupForm.WorldZoneSetupTimeTrackbar_Scroll(ZoneSetupForm.WorldZoneSetupTimeTrackbar, EventArgs.Empty);
				

				////WorldSetupDefalutZoneCheckBox.Checked = CurrentClientZone.DefaultZone;
				Program.GE.DefalutZoneNoSave = false;
				
				if (Program.GE.CurrentClientZone == null || Program.GE.CurrentServerZone == null)
				{
					MessageBox.Show("Error loading zone from file!", "Error");
					Program.GE.LoadingAZone = false;
					return;
				}

				#region Match water with server water
				Water W;
				WaterArea SW;
				for (int i = 0; i < Program.GE.CurrentClientZone.Waters.Count; ++i)
				{
					W = (Water) Program.GE.CurrentClientZone.Waters[i];


					for (int j = 0; j < WaterArea.WaterList.Count; ++j)
					{
						SW = (WaterArea) WaterArea.WaterList[j];
						if (SW.Zone == Program.GE.CurrentServerZone)
						{
							float ScaleX = W.EN.ScaleX();
							float ScaleZ = W.EN.ScaleZ();

//                             string Out = string.Format(
//                                 "X: {0}, {1}\n"
//                                 + "Z: {2}, {3}\n"
//                                 + "Y: {4}, {5}\n"
//                                 + "W: {6}, {7}\n"
//                                 + "D: {8}, {9}\n",
//                                 new object[] { 
//                                     SW.X, (W.EN.X() - (ScaleX * 0.5f)),
//                                     SW.Z, (W.EN.Z() - (ScaleZ * 0.5f)),
//                                     SW.Y, W.EN.Y(),
//                                     SW.Width, ScaleX,
//                                     SW.Depth, ScaleZ });
//                             MessageBox.Show(Out);



							if (Math.Abs(SW.Width - ScaleX) <= 5.0f &&
								Math.Abs(SW.Depth - ScaleZ) <= 5.0f &&
								Math.Abs(SW.X - (W.EN.X() - (ScaleX * 0.5f))) <= 5.0f &&
								Math.Abs(SW.Z - (W.EN.Z() - (ScaleZ * 0.5f))) <= 5.0f &&
								Math.Abs(SW.Y - W.EN.Y()) <= 5.0f)
							{
								W.ServerWater = SW;
								break;
							}
						}
					}

					// A match could not be found - delete this water area
					if (W.ServerWater == null)
					{
						for (int it = 0; i < 4; ++i)
						{
							Media.UnloadTexture((int)W.TextureID[it]);
							//Render.FreeTexture((uint)W.TexHandle[it]);
						}
						W.EN.Free();
						Program.GE.CurrentClientZone.RemoveObject(W);
					}
				}

				List<RealmCrafter.ServerZone.WaterArea> removals = new List<RealmCrafter.ServerZone.WaterArea>();
				foreach (RealmCrafter.ServerZone.WaterArea waterArea in RealmCrafter.ServerZone.WaterArea.WaterList)
				{
					bool found = false;

					for (int i = 0; i < Program.GE.CurrentClientZone.Waters.Count; ++i)
					{
						W = (Water)Program.GE.CurrentClientZone.Waters[i];

						if (W.ServerWater == waterArea)
						{
							found = true;
							break;
						}
					}

					if (!found)
						removals.Add(waterArea);
				}

				foreach (RealmCrafter.ServerZone.WaterArea waterArea in removals)
				{
					RealmCrafter.ServerZone.WaterArea.WaterList.Remove(waterArea);
				}
				#endregion

				#region Add objects to tree view and set collision types
				Media.LockMeshes();
				Media.LockTextures();
				Media.LockSounds();
				string Name;
				Scenery S;
				RCTTerrain T;
				Emitter EM;
				ColBox CB;
				SoundZone SZ;
				RealmCrafter.ClientZone.Light LT;

				for (int i = 0; i < Program.GE.CurrentClientZone.Sceneries.Count; ++i)
				{
					S = (Scenery) Program.GE.CurrentClientZone.Sceneries[i];
					if (Collision.EntityType(S.EN) == (int) CollisionType.Sphere)
					{
						Collision.SetCollisionMesh(S.EN);
						Collision.EntityPickMode(S.EN, PickMode.Polygon);
					}
					else if (Collision.EntityType(S.EN) == (int) CollisionType.Box)
					{
						Collision.SetCollisionMesh(S.EN);
					}

					Name = Media.GetMeshName(S.MeshID);
					TreeNode TN =
						new TreeNode(Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
					TN.Tag = S;
					m_ZoneList.WorldZonesTree.Nodes[0].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Terrains.Count; ++i)
				{
					T = (RCTTerrain) Program.GE.CurrentClientZone.Terrains[i];
					TreeNode TN = new TreeNode("Terrain " + (i + 1).ToString());
					TN.Tag = T;
					m_ZoneList.WorldZonesTree.Nodes[1].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Emitters.Count; ++i)
				{
					EM = (Emitter) Program.GE.CurrentClientZone.Emitters[i];
					TreeNode TN;
					if (EM.Config != null)
					{
						TN = new TreeNode(EM.Config.Name);
					}
					else
					{
						TN = new TreeNode("Unknown emitter");
					}

					TN.Tag = EM;
					m_ZoneList.WorldZonesTree.Nodes[2].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Waters.Count; ++i)
				{
					W = (Water) Program.GE.CurrentClientZone.Waters[i];
					Collision.SetCollisionMesh(W.EN);
					Collision.EntityPickMode(W.EN, PickMode.Polygon);
					Name = Media.GetTextureName((int)W.TextureID[0]);
//                    Name = "ahhh";
					TreeNode TN =
						new TreeNode(Path.GetFileNameWithoutExtension(Name.Substring(0, Name.Length - 1)));
					TN.Tag = W;
					m_ZoneList.WorldZonesTree.Nodes[3].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.ColBoxes.Count; ++i)
				{
					CB = (ColBox) Program.GE.CurrentClientZone.ColBoxes[i];
					TreeNode TN = new TreeNode("Collision box " + (i + 1).ToString());
					TN.Tag = CB;
					m_ZoneList.WorldZonesTree.Nodes[4].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.SoundZones.Count; ++i)
				{
					SZ = (SoundZone) Program.GE.CurrentClientZone.SoundZones[i];
					SZ.EN.Texture(Program.GE.OrangeTex);
					if (SZ.SoundID < 65535)
					{
						Name = Media.GetSoundName(SZ.SoundID);
						if(Name.Length > 1)
							Name = Name.Substring(0, Name.Length - 1);
					}
					else if (SZ.MusicID < 65535)
					{
						Name = Media.GetMusicName(SZ.MusicID);
					}
					else
					{
						Name = "Unknown sound";
					}

					TreeNode TN = new TreeNode(Path.GetFileNameWithoutExtension(Name));
					TN.Tag = SZ;
					m_ZoneList.WorldZonesTree.Nodes[5].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.Lights.Count; ++i)
				{
					LT = (RealmCrafter.ClientZone.Light)Program.GE.CurrentClientZone.Lights[i];
					TreeNode TN = new TreeNode("Light " + (i + 1).ToString());
					TN.Tag = LT;
					m_ZoneList.WorldZonesTree.Nodes[6].Nodes.Add(TN);
				}

				for (int i = 0; i < Program.GE.CurrentClientZone.MenuControls.Count; ++i)
				{
					MenuControl Mc = (RealmCrafter.ClientZone.MenuControl)Program.GE.CurrentClientZone.MenuControls[i];
					TreeNode TN = new TreeNode("MenuControl " + (i + 1));
					TN.Tag = Mc;
					m_ZoneList.WorldZonesTree.Nodes[12].Nodes.Add(TN);
				}

				Media.UnlockMeshes();
				Media.UnlockTextures();
				Media.UnlockSounds();
				#endregion

				m_ZoneList.WorldZonesTree.CollapseAll();

				#region Create entities representing server side objects
				// Triggers
				for (int i = 0; i < CurrentServerZone.Triggers.Count; ++i)
				{
					Trigger Tr = CurrentServerZone.Triggers[i];
					if (!string.IsNullOrEmpty(Tr.Script))
					{
						TreeNode TN = new TreeNode(Tr.Script);
						TN.Tag = Tr;
						m_ZoneList.WorldZonesTree.Nodes[7].Nodes.Add(TN);
					}
				}

				// Portals
				for (int i = 0; i < CurrentServerZone.Portals.Count; ++i)
				{
					Portal P = CurrentServerZone.Portals[i];
					if (!string.IsNullOrEmpty(P.Name))
					{
						TreeNode TN = new TreeNode(P.Name);
						TN.Tag = P;
						m_ZoneList.WorldZonesTree.Nodes[9].Nodes.Add(TN);
					}
				}

				// Waypoints/spawn points
				for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
				{
					Waypoint WP = CurrentServerZone.Waypoints[i];
					WP.WaypointEN = WP.EN;
					TreeNode TN = new TreeNode("Waypoint " + (i).ToString());
					TN.Tag = WP;
					m_ZoneList.WorldZonesTree.Nodes[8].Nodes.Add(TN);
				}

				// Waypoint links
				for (int i = 0; i < CurrentServerZone.Waypoints.Count; ++i)
				{
					Waypoint WP = CurrentServerZone.Waypoints[i];

					if (WP.NextA != null)
					{
						WP.WaypointLinkAEN = Program.GE.WaypointLinkTemplate.Copy();
						WP.WaypointLinkAEN.Shader = Shaders.LitObject1;
						WP.WaypointLinkAEN.Texture(Program.GE.OrangeTex);
					}
					if (WP.NextB != null)
					{
						WP.WaypointLinkBEN = Program.GE.WaypointLinkTemplate.Copy();
						WP.WaypointLinkBEN.Shader = Shaders.LitObject1;
						WP.WaypointLinkBEN.Texture(Program.GE.BlueTex);
					}
				}

				Program.GE.UpdateWaypointLinks();
				#endregion

				m_ZoneList.AddObjectsCount();
				Program.GE.ZoneSetupForm.UpdateZoneSetup(false);
				Program.GE.SetWorldSavedStatus(true);
				Program.GE.Camera.Position(0, 0, 0);
				Program.GE.WorldSelectedObjectsLabel.Text = "Selected objects: 0";
				Program.GE.GridHeight = 0f;
				Program.GE.RepositionGrid();
                Program.GE.UpdateRenderingPanel((int)GETab.NEWWORLD);
				Program.GE.LoadingAZone = false;
				////UpdateZoneList();
			}
		}

		public void UpdateZoneListEmpty()
		{
			WorldZones.Items.Clear();
			string[] Zones = Directory.GetFiles(@"Data\Areas\", "*.dat");
			////string[] ZoneList = new string[Zones.GetUpperBound(0) + 2];
			////ZoneList[0] = "";
			WorldZones.BeginUpdate();
			foreach (string S in Zones)
			{
				WorldZones.Items.Add(Path.GetFileNameWithoutExtension(S));
			}

			WorldZones.EndUpdate();

			if (Program.GE.CurrentClientZone != null)
			{
				WorldZones.Text = Program.GE.CurrentClientZone.Name;
			}
			else
			{
				WorldZones.Text = "";
			}
		}

		public void UpdateZoneList()
		{
			SupressChange = true;
			////object restore = WorldZones.SelectedItem;
			WorldZones.Items.Clear();
			string[] Zones = Directory.GetFiles(@"Data\Areas\", "*.dat");
			////string[] ZoneList = new string[Zones.GetUpperBound(0) + 2];
			////ZoneList[0] = "";
			WorldZones.BeginUpdate();
			foreach (string S in Zones)
			{
				WorldZones.Items.Add(Path.GetFileNameWithoutExtension(S));
			}
			ActorStartZoneCombo.Items.Clear();
			foreach (string S_ in Zones)
			{
				Program.GE.TotalZones++;
				string Name = Path.GetFileNameWithoutExtension(S_);

				Program.GE.ActorStartZoneCombo.Items.Add(Name);
			}
			WorldZones.EndUpdate();
			ActorStartZoneCombo.EndUpdate();
			if (Program.GE.CurrentClientZone != null)
			{
				WorldZones.Text = Program.GE.CurrentClientZone.Name;
			}
			else
			{
				WorldZones.Text = "";
			}
			WorldZones.SelectionStart = WorldZones.Text.Length;
			SupressChange = false;
		}

		public void UpdateSelection()
		{
			UpdateZoneList();
			WorldZones.Text = Program.GE.CurrentClientZone.Name;
		}

		private void WorldZones_TextUpdate(object sender, EventArgs e)
		{
			if (CurrentClientZone != null)
			{
				int temp = WorldZones.SelectionStart;
				string OldZoneName = CurrentClientZone.Name;
				if (File.Exists(@"Data\Areas\" + WorldZones.Text + ".dat"))
				{
					MessageBox.Show("Zone already exists");
					WorldZones.Text = CurrentClientZone.Name;
					return;
				}

				System.Text.RegularExpressions.Regex Reg = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z0-9_ \-]");
				if (WorldZones.Text.Trim().Length < 1)
				{
					MessageBox.Show("Name is too short.");
					WorldZones.Text = CurrentClientZone.Name;
					return;
				}

                if (char.IsWhiteSpace(WorldZones.Text[0]))
                {
                    MessageBox.Show("Zones may not start with empty characters");
                    WorldZones.Text = CurrentClientZone.Name;
                    return;
                }
					
				if(Reg.IsMatch(WorldZones.Text))
				{
					MessageBox.Show("Invalid characters in filename!");
					WorldZones.Text = CurrentClientZone.Name;
					return;
				}
                CurrentClientZone.Name = WorldZones.Text.TrimStart();
                CurrentServerZone.Name = WorldZones.Text.TrimStart(); ;


				//  SaveWorld();
				try
				{
					File.Move(@"Data\Areas\" + OldZoneName + ".dat", @"Data\Areas\" + CurrentClientZone.Name + ".dat");
					File.Move(@"Data\Areas\" + OldZoneName + ".ltz", @"Data\Areas\" + CurrentClientZone.Name + ".ltz");
					File.Move(@"Data\Server Data\Areas\" + OldZoneName + ".dat",
							  @"Data\Server Data\Areas\" + CurrentServerZone.Name + ".dat");
					for (int i = 0; i < 100; ++i)
					{
						if (File.Exists(@"Data\Server Data\Areas\Ownerships\" + OldZoneName + " (" + i.ToString() +
										") Ownerships.dat"))
						{
							File.Move(@"Data\Server Data\Areas\Ownerships\" + OldZoneName + " (" + i.ToString() +
									  ") Ownerships.dat",
									  @"Data\Server Data\Areas\Ownerships\" + CurrentClientZone.Name + " (" +
									  i.ToString() + ") Ownerships.dat");
						}
					}
					UpdateZoneList();
					WorldZones.SelectionStart = temp;
				}
				catch (IOException err)
				{
					CurrentClientZone.Name = OldZoneName;
					CurrentServerZone.Name = OldZoneName;
					WorldZones.Text = OldZoneName;
					MessageBox.Show("Error renaming zone: " + err.Message);
				}
			}
			else
			{
				WorldZones.Text = "";
			}
		}

		public void UpdateMeshTextureMusicSoundLists()
		{
			Program.MeshList.Clear();
			Program.MusicList.Clear();
			Program.SoundsList.Clear();
			Program.TextureList.Clear();
			string Name;
			ushort i;
			// Meshes
			Media.LockMeshes();
			lock (Program.MeshList)
			{
				for (i = 0; i < 65535; ++i)
				{
					Name = Media.GetMeshName(i);
					if (Name.Length > 0)
					{
						Program.MeshList.Add(Name + " ID: " + i);
					}
				}
			}
			Media.UnlockMeshes();
			// Textures
			lock (Program.TextureList)
			{
				Media.LockTextures();
				for (i = 0; i < 65535; ++i)
				{
					Name = Media.GetTextureName(i);
					if (Name.Length > 0)
					{
						Program.TextureList.Add(Name + " ID: " + i);
					}
				}
			}

			Media.UnlockTextures();
			// Sounds
			lock (Program.SoundsList)
			{
				Media.LockSounds();
				for (i = 0; i < 65535; ++i)
				{
					Name = Media.GetSoundName(i);
					if (Name.Length > 0)
					{
						Program.SoundsList.Add(Name + " ID: " + i);
					}
				}
			}
			Media.UnlockSounds();
			// Music
			Media.LockMusic();
			lock (Program.MusicList)
			{
				for (i = 0; i < 65535; ++i)
				{
					Name = Media.GetMusicName(i);
					if (Name.Length > 0)
					{
						Program.MusicList.Add(Name + " ID: " + i);
					}
				}
			}
			Media.UnlockMusic();
		}

	
		private void ParametersForm_Disposed(object sender, EventArgs e)
		{
			ParametersForm = null;
		}
		#endregion

		private void MediaPreviewGroup_Enter(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			throw new ApplicationException("You had an error in your application");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Entity q = null;
			q.Position(5, 5, 5);
		}

		private void FullScreenButton_Click(object sender, EventArgs e)
		{
			if(FullScreenButton.Checked == false)
			{
				_FullScreen.ShowFullScreen();
				FullScreenButton.Checked = true;
			}
			else
			{
				_FullScreen.ShowFullScreen();
				FullScreenButton.Checked = false;
			}
		}

		private void WorldDock_ActiveDocumentChanged(object sender, EventArgs e)
		{
			if (ControlHost.SelectedIndex != (int)GETab.WORLD)
				return;

			if (WorldDock.ActiveDocument != null)
			{
				if (WorldDock.ActiveDocument.DockHandler == m_Interface.DockHandler)
				{
                    UpdateRenderingPanel((int)GETab.INTERFACE);
				}
				else if (WorldDock.ActiveDocument.DockHandler == TreeRender.DockHandler)
				{
                    UpdateRenderingPanel((int)GETab.TREE);
				}
				else if (WorldDock.ActiveDocument.DockHandler == m_ScriptView.DockHandler)
				{
                    UpdateRenderingPanel((int)GETab.SCRIPT);
				}
				else if (WorldDock.ActiveDocument.DockHandler == m_GubbinEditor.DockHandler)
				{
                    UpdateRenderingPanel((int)GETab.GUBBIN);
				}
				else
				{
					if (WorldDock.ActiveDocument.DockHandler == m_ZoneRender.DockHandler)
					{
                        UpdateRenderingPanel((int)GETab.NEWWORLD);

						if (WorldDock.ActiveDocument is WorldViewRenderer)
							(WorldDock.ActiveDocument as WorldViewRenderer).ResetCamera();
					}
				}
			}

		}

		#region GUI_IO_Handles (by Silva 2009)
			NVector2 b;
		public bool IsLMouseDown = false;
		public bool IsRMouseDown = false;
		private void RenderingPanel_MouseDown(object sender, MouseEventArgs e)
		{
			// Update NGUI parameters
			// JB: Hopefully we won't need to capture NGUI input
			//if (e.Button == MouseButtons.Left)  Parameters.LeftDown = true;
			//if (e.Button == MouseButtons.Right) Parameters.RightDown = true;

			// for update WindowProperty with selected control
			if (ControlHost.SelectedIndex == (int)GETab.WORLD &&
				WorldDock.ActiveDocument != null &&
				WorldDock.ActiveDocument.DockHandler == m_Interface.DockHandler)
			{
				m_propertyWindow.RefreshObjectWindow();
			}

			Point Mouse = PointToClient(Cursor.Position);
			b = new NVector2(Mouse.X, Mouse.Y);

			if (e.Button == MouseButtons.Left)
				IsLMouseDown = true;
			if (e.Button == MouseButtons.Right)
				IsRMouseDown = true;

//             if (GE.GUIManager.ControlFocus != null && GE.GUIManager.ControlFocus.Tag is cControl)
//                 m_propertyWindow.ObjectProperties.SelectedObject = GE.GUIManager.ControlFocus.Tag
		}

		private void RenderingPanel_MouseUp(object sender, MouseEventArgs e)
		{
			// Update NGUI parameters
			// JB: Hopefully we don't need these!
			//if (e.Button == MouseButtons.Left)  Parameters.LeftDown = false;
			//if (e.Button == MouseButtons.Right) Parameters.RightDown = false;

//            if (GUIManager.ControlFocus != null) GUIManager.ControlFocus = null;
			if (e.Button == MouseButtons.Left)
				IsLMouseDown = false;
			if (e.Button == MouseButtons.Right)
				IsRMouseDown = false;
		}

		public void GE_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Add the pressed key to the buffer
			Parameters.InputBuffer.Add(e.KeyChar);
		}
		#endregion

		private void TerrainAutoTextureButton_Click(object sender, EventArgs e)
		{
			m_TerrainEditor.AutoTextureTerrain();
		}

		private void TerrainImportButton_Click(object sender, EventArgs e)
		{
			if(CurrentClientZone != null)
				m_TerrainEditor.ImportTerrain();
		}

		void TerrainGrassTypesEditorButton_Click(object sender, System.EventArgs e)
		{
			GrassTypesEditor.ShowDialog();
		}

		void LightFunctionEditorButton_Click(object sender, EventArgs e)
		{
			RealmCrafter.LightFunctionList ListEdit = new LightFunctionList();
			ListEdit.ShowDialog();
			LightFunctionList.SaveFunctions();
		}

		private void EmitterPreviewUpButton_Click(object sender, EventArgs e)
		{

		}

		private void WorldTerrainEditorShow_Click(object sender, EventArgs e)
		{
			if (m_TerrainEditor.Visible == false)
			{
				m_TerrainEditor.Show(WorldDock);
			}
			else
			{
				m_TerrainEditor.Hide();
			}
		}

		private void GubbinEditorToggle_Click(object sender, EventArgs e)
		{
			if (m_GubbinEditor.Visible == false)
			{
				m_GubbinEditor.Show(WorldDock);
			}
			else
			{
				m_GubbinEditor.Hide();
			}
		}

		private void btnMeshHigh_Click(object sender, EventArgs e)
		{

		}



		private void TreeEditorRenderToggle_Click(object sender, EventArgs e)
		{
			if (TreeRender.Visible)
				TreeRender.Hide();
			else
			{
				TreeRender.Show(WorldDock, DockState.Document);
			}
		}



		private void TreePropertiesToggle_Click(object sender, EventArgs e)
		{
//             if (TreeRender.Visible)
//             {
//                 if (TreeProperties.Visible)
//                     TreeProperties.Hide();
//                 else
//                     TreeProperties.Show();
//             }
		}

		private void ActorsSettingsEnemyButton_Click(object sender, EventArgs e)
		{
			PictureBox select = (PictureBox)sender;
			select.Tag = MediaDialogs.GetTexture(true, Convert.ToUInt16(select.Tag) );
			if (Convert.ToUInt16(select.Tag) == 65535) select.ImageLocation = @"";
								else select.ImageLocation = @"Data\\Textures\\" + Media.GetTextureName(Convert.ToUInt16(select.Tag) );
			SetActorsSavedStatus(false);
		}

		private void ActorsSettingsPlayerButton_Paint(object sender, PaintEventArgs e)
		{
		}

		private void ActorsSettingsPlayerButton_VisibleChanged(object sender, EventArgs e)
		{
			PictureBox select = (PictureBox)sender;
			if (Convert.ToUInt16(select.Tag) == 65535) select.ImageLocation = @"";
			else select.ImageLocation = @"Data\\Textures\\" + Media.GetTextureName(Convert.ToUInt16(select.Tag));

		}

		public void UpdateRenderMask()
		{
			int MaskFlags = 0;
			MaskFlags += RenderToggleScenery.Checked ? 1 : 0;
			MaskFlags += RenderToggleGrass.Checked ? 2 : 0;
			MaskFlags += RenderToggleTrees.Checked ? 4 : 0;
			MaskFlags += RenderToggleTerrain.Checked ? 8 : 0;
			MaskFlags += RenderToggleEditor.Checked ? 16 : 0;
			MaskFlags += RenderToggleCollision.Checked ? 32 : 0;

			RenderWrapper.bbdx2_SetRenderMask(MaskFlags);
		}

		private void RenderToggleScenery_Click(object sender, EventArgs e)
		{
			UpdateRenderMask();
		}

		private void RenderReloadShaders_Click(object sender, EventArgs e)
		{
			RenderWrapper.bbdx2_ReloadShaders();
		}

		private void ConfigManagerButton_Click(object sender, EventArgs e)
		{
			BuildManager Mgr = new BuildManager();
			Mgr.ShowDialog();
		}

		private void WorldLODEditorButton_Click(object sender, EventArgs e)
		{
			if (CurrentClientZone == null)
				return;

			LODEditor Editor = new LODEditor();
			Editor.ShowDialog();
		}

        private void MediaManagerButton_Click(object sender, EventArgs e)
        {
            MediaWindow.SetupRenderPanel();
            MediaWindow.ShowDialog();
            
            // Swap tabs to reset tab settings
            int oldIndex = ControlHost.SelectedIndex;
            ControlHost.SelectedIndex = ControlHost.TabCount - 1;
            ControlHost.SelectedIndex = oldIndex;
        }

        private void ProjectButton_Click(object sender, EventArgs e)
        {
            ProjectWindow window = new ProjectWindow(this);
            window.ShowDialog();
            window.Dispose();
        }



        private void ProjectSettingsButton_Click(object sender, EventArgs e)
        {
            ProjectWindow window = new ProjectWindow(this);
            window.ShowDialog();
            window.Dispose();
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            HelpWindow window = new HelpWindow();
            window.Show();
            window.Focus();
        }




	   
	}


	// Listbox item class
	public class ListBoxItem
	{
		public string Name;
		public uint Value;

		public ListBoxItem(string Name, uint Value)
		{
			this.Name = Name;
			this.Value = Value;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	// Quaternion value type
	internal struct Quaternion
	{
		public float W, X, Y, Z;

		public static Quaternion SetAxisAngle(double Angle, double AxisX, double AxisY, double AxisZ)
		{
			Quaternion Result;
			Angle *= 0.5d;
			Result.W = (float) Math.Cos(Angle);
			Result.X = (float) (AxisX * Math.Sin(Angle));
			Result.Y = (float) (AxisY * Math.Sin(Angle));
			Result.Z = (float) (AxisZ * Math.Sin(Angle));
			return Result;
		}

		public static Quaternion Multiply(Quaternion A, Quaternion B)
		{
			Quaternion Result;
			Result.W = (A.W * B.W) - (A.X * B.X) - (A.Y * B.Y) - (A.Z * B.Z);
			Result.X = (A.W * B.X) + (A.X * B.W) + (A.Y * B.Z) - (A.Z * B.Y);
			Result.Y = (A.W * B.Y) - (A.X * B.Z) + (A.Y * B.W) + (A.Z * B.X);
			Result.Z = (A.W * B.Z) + (A.X * B.Y) - (A.Y * B.X) + (A.Z * B.W);
			return Result;
		}
	}
}