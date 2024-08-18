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
#include "COptionsMenu.h"

using namespace NGin;
using namespace NGin::GUI;
using namespace NGin::Math;

namespace RealmCrafter
{
	COptionsMenu::COptionsMenu()
	{

	}

	COptionsMenu::~COptionsMenu()
	{

	}

	void COptionsMenu::Initialize()
	{
		// Use a macro to make life easier
		#define R(X, Y) (Vector2(X, Y) / GUIManager->GetResolution())
		
		Window = GUIManager->CreateWindow("COptionsMenu::Window", Vector2(0.4f, 0.4f), R(284, 304));
		Window->Closed()->AddEvent(this, &COptionsMenu::CancelButton_Click);
		Window->Modal = true;
		Window->Text = "Settings";

		TabControl = GUIManager->CreateTabControl("COptionsMenu::TabControl", Vector2(0, 0), R(268, 230));
		OKButton = GUIManager->CreateButton("COptionsMenu::OKButton", R(100, 240), R(75, 23));
		CancelButton = GUIManager->CreateButton("COptionsMenu::CancelButton", R(181, 240), R(75, 23));
		
		TabControl->AddTab(LanguageString[LS_GraphicsOptions], R(70, 1).X);
		TabControl->AddTab(LanguageString[LS_ControlOptions], R(70, 1).X);
		TabControl->AddTab(LanguageString[LS_OtherOptions], R(50, 1).X);

		OKButton->Text = LanguageString[LS_OK];
		OKButton->Click()->AddEvent(this, &COptionsMenu::OKButton_Click);
		CancelButton->Text = LanguageString[LS_Cancel];
		CancelButton->Click()->AddEvent(this, &COptionsMenu::CancelButton_Click);

		OKButton->Parent = Window;
		CancelButton->Parent = Window;
		TabControl->Parent = Window;

		OKButton->Align = TextAlign_Center;
		OKButton->VAlign = TextAlign_Center;
		
#pragma region Graphics Options
		// Create controls
		ResolutionLabel = GUIManager->CreateLabel("COptionsMenu::ResolutionLabel", R(8, 21), Vector2(0, 0));
		AntiAliasLabel = GUIManager->CreateLabel("COptionsMenu::AntiAliasLabel", R(8, 67), Vector2(0, 0));
		ShadowDetailLabel = GUIManager->CreateLabel("COptionsMenu::ShadowDetailLabel", R(8, 94), Vector2(0, 0));
		AnisotropyLabel = GUIManager->CreateLabel("COptionsMenu::AnisotropyLabel", R(8, 121), Vector2(0, 0));
		EffectQualityLabel = GUIManager->CreateLabel("COptionsMenu::EffectQualityLabel", R(8, 148), Vector2(0, 0));
		GrassDistanceLabel = GUIManager->CreateLabel("COptionsMenu::GrassDistanceLabel", R(8, 172), Vector2(0, 0));

		ResolutionCombo = GUIManager->CreateComboBox("COptionsMenu::ResolutionCombo", R(11, 37), R(121, 121));
		WindowCombo = GUIManager->CreateComboBox("COptionsMenu::WindowCombo", R(138, 37), R(121, 60));
		AntiAliasCombo = GUIManager->CreateComboBox("COptionsMenu::AntiAliasCombo", R(138, 64), R(121, 90));
		ShadowDetailCombo = GUIManager->CreateComboBox("COptionsMenu::ShadowDetailCombo", R(138, 91), R(121, 90));
		AnisotropyCombo = GUIManager->CreateComboBox("COptionsMenu::AnisotropyCombo", R(138, 118), R(121, 90));
		EffectQualityCombo = GUIManager->CreateComboBox("COptionsMenu::EffectQualityCombo", R(138, 145), R(121, 70));
		
		GrassDistanceTrack = GUIManager->CreateTrackBar("COptionsMenu::GrassDistanceTrack", R(138, 172), R(121, 45));
		
		// Labels
		ResolutionLabel->Text = LanguageString[LS_SelectResolution];
		AntiAliasLabel->Text = LanguageString[LS_EnableAA];
		ShadowDetailLabel->Text = LanguageString[LS_ShadowDetail];
		EffectQualityLabel->Text = LanguageString[LS_ShaderQuality];
		AnisotropyLabel->Text = LanguageString[LS_AnisotropyLevel];
		GrassDistanceLabel->Text = LanguageString[LS_GrassDistance];

		// Windowed
		WindowCombo->AddItem(LanguageString[LS_OptionFullscreen]);
		WindowCombo->AddItem(LanguageString[LS_WindowedMode]);

		// Resolution
		int Count = CountGraphicsModes();
		int ResSelection = 0;
		ArrayList<string> AllResolutions;

		for(int i = 0; i < Count; ++i)
		{
			// Get width and limit it
			int Width = GraphicsModeWidth(i);
			if(Width > 800)
			{
				// Get height and make list string
				int Height = GraphicsModeHeight(i);
				string ModeName = std::toString(Width) + " x " + std::toString(Height);

				bool Found = false;

				// Check if it exists already
				for(int f = 0; f < AllResolutions.Size(); ++f)
					if(AllResolutions[f] == ModeName)
						Found = true;

				// Default selection
				if(ModeName == string("1024 x 768") && !Found)
					ResSelection = ResolutionCombo->ItemCount();

				// Add
				if(!Found)
				{
					ResolutionCombo->AddItem(ModeName);
					AllResolutions.Add(ModeName);
				}
			}
		}
		ResolutionCombo->SelectedIndex = ResSelection;

		// AntiAliasing
		Count = CountMultipleSamples(1);
		AntiAliasCombo->AddItem("None");
		if(Count > 0)
			for(int i = 2; i <= Count; ++i)
				AntiAliasCombo->AddItem(string("x") + std::toString(i));

		// Shadow box
		ShadowDetailCombo->AddItem(LanguageString[LS_ShadowDisabled]);
		//ShadowDetailCombo->AddItem(LanguageString[LS_ShadowPlayers]);
		ShadowDetailCombo->AddItem(LanguageString[LS_ShadowAllActors]);
		ShadowDetailCombo->AddItem(LanguageString[LS_ShadowScenery]);
		ShadowDetailCombo->AddItem(LanguageString[LS_ShadowTrees]);

		// Shader Quality
		EffectQualityCombo->AddItem(LanguageString[LS_QualityLow]);
		EffectQualityCombo->AddItem(LanguageString[LS_QualityMedium]);
		EffectQualityCombo->AddItem(LanguageString[LS_QualityHigh]);

		// Anisotropy
		AnisotropyCombo->AddItem(LanguageString[LS_Disabled]);
		AnisotropyCombo->AddItem("x4");
		AnisotropyCombo->AddItem("x8");
		AnisotropyCombo->AddItem("x16");

		// Grass draw distance
		GrassDistanceTrack->Minimum = 0;
		GrassDistanceTrack->Maximum = 150;
		GrassDistanceTrack->TickFrequency = 5;

		// Parents
		ResolutionLabel->Parent = TabControl->TabPanel(0);
		AntiAliasLabel->Parent = TabControl->TabPanel(0);
		ShadowDetailLabel->Parent = TabControl->TabPanel(0);
		EffectQualityLabel->Parent = TabControl->TabPanel(0);
		AnisotropyLabel->Parent = TabControl->TabPanel(0);
		GrassDistanceLabel->Parent = TabControl->TabPanel(0);
		ResolutionCombo->Parent = TabControl->TabPanel(0);
		WindowCombo->Parent = TabControl->TabPanel(0);
		AntiAliasCombo->Parent = TabControl->TabPanel(0);
		ShadowDetailCombo->Parent = TabControl->TabPanel(0);
		EffectQualityCombo->Parent = TabControl->TabPanel(0);
		AnisotropyCombo->Parent = TabControl->TabPanel(0);
		GrassDistanceTrack->Parent = TabControl->TabPanel(0);
#pragma endregion
		
#pragma region Control Options

		ControlEditor = CreateControlEditor(GUIManager, "COptionsMenu::ControlEditor", R(1, 1), R(268, 208));

		ControlEditor->WaitForInputKeyCode()->ClearEvents();
		ControlEditor->WaitForInputKeyCode()->AddEvent(this, &COptionsMenu::ControlEditor_WaitForInputKeyCode);
		ControlEditor->SetLayout(ControlLayout);

		ControlEditor->Parent = TabControl->TabPanel(1);

#pragma endregion

#pragma region Misc Options


		VolumeLabel = GUIManager->CreateLabel("COptionsMenu::VolumeLabel", R(12, 9), Vector2(0, 0));
		VolumeTrack = GUIManager->CreateTrackBar("COptionsMenu::VolumeTrack", R(12, 25), R(250, 32));

		VolumeLabel->Text = LanguageString[LS_SndVolume];

		VolumeTrack->Minimum = 0;
		VolumeTrack->Maximum = 100;
		VolumeTrack->TickFrequency = 5;

		VolumeLabel->Parent = TabControl->TabPanel(2);
		VolumeTrack->Parent = TabControl->TabPanel(2);

#pragma endregion

#pragma region Load Settings

		// Set up initial graphics options
		int Width = 1024, Height = 768, AA = 0, Windowed = 1, Anisotropy = 0, ShadowDetail = 0, ShaderQuality = 0, GrassDistance = 40;
		float DefaultVolume = 0.0f;

		// Read options file
		XMLReader* X = ReadXMLFile("Data\\Options.xml");
		if(X == 0)
			RuntimeError("Could not open Data\\Options.xml");

		// Get values
		while(X->Read())
		{
			string ElementNameLower = stringToLower(X->GetNodeName());

			// If its a <cfg> element
			if(X->GetNodeType() == XNT_Element && ElementNameLower.compare("cfg") == 0)
			{
				string AttributeNameLower = stringToLower(X->GetAttributeString("name"));

				// Find the config item
				if(AttributeNameLower.compare("width") == 0)
					Width = X->GetAttributeInt("value");
				else if(AttributeNameLower.compare("height") == 0)
					Height = X->GetAttributeInt("value");
				else if(AttributeNameLower.compare("antialias") == 0)
					AA = X->GetAttributeInt("value");
				else if(AttributeNameLower.compare("anisotropy") == 0)
					Anisotropy = X->GetAttributeInt("value");
				else if(AttributeNameLower.compare("windowed") == 0)
					Windowed = X->GetAttributeInt("value");
				else if(AttributeNameLower.compare("volume") == 0)
					DefaultVolume = X->GetAttributeFloat("value");
				else if(AttributeNameLower.compare("eula") == 0)
					EULAAccepted = X->GetAttributeFloat("value");
				else if(AttributeNameLower.compare("shadowdetail") == 0)
					ShadowDetail = X->GetAttributeInt("value");
				else if(AttributeNameLower.compare("shaderquality") == 0)
					ShaderQuality = X->GetAttributeInt("value");
				else if(AttributeNameLower.compare("grassdistance") == 0)
					GrassDistance = X->GetAttributeInt("value");
			}
		}

		// Close file
		delete X;

		// Locals
		SelectedResolution = toString(Width) + " x " + toString(Height);
		SelectedAA = AA;
		SelectedShadows = ShadowDetail;
		SelectedAnisotropy = Anisotropy;
		SelectedWindowed = Windowed > 0;
		SelectedVolume = DefaultVolume;
		SelectedQuality = ShaderQuality;
		SelectedGrassDistance = GrassDistance;

		// Also have to set globals
		GFXAntiAlias = SelectedAA;
		GFXAnisotropyLevel = SelectedAnisotropy;
		GFXWindowed = SelectedWindowed;
		RealmCrafter::Globals->DefaultVolume = SelectedVolume;
		GFXShadowDetail = SelectedShadows;
		GFXShaderQuality = SelectedQuality;
		GFXWidth = Width;
		GFXHeight = Height;

#pragma endregion

		Window->Visible = false;
		#undef R
	}

	void COptionsMenu::Save()
	{
		// Write XML file
		FILE* F = WriteFile("Data\\Options.xml");
		if(F == 0)
			RuntimeError("Could not write to Data\\Options.xml!");
		WriteLine(F, "<?xml version=\"1.0\"?>");
		WriteLine(F, "<options>");

		// Get selected resolution
		int Divider = SelectedResolution.find("x", 0);
		if(Divider == -1)
			RuntimeError("Invalid resolution format!");
		string SWidth = SelectedResolution.substr(0, Divider - 1);
		string SHeight = SelectedResolution.substr(Divider + 1);
		SWidth = trim(SWidth);
		SHeight = trim(SHeight);

		// Resolution
		WriteLine(F, string("\t<cfg name=\"Width\" value=\"") + SWidth + string("\" />"));
		WriteLine(F, string("\t<cfg name=\"Height\" value=\"") + SHeight + string("\" />"));

		// AntiAlias
		WriteLine(F, string("\t<cfg name=\"AntiAlias\" value=\"") + toString(SelectedAA) + string("\" />"));

		// Windowed
		WriteLine(F, string("\t<cfg name=\"Windowed\" value=\"") + toString((SelectedWindowed) ? 1 : 0) + string("\" />"));

		// Anisotropy
		WriteLine(F, string("\t<cfg name=\"Anisotropy\" value=\"") + toString(SelectedAnisotropy) + string("\" />"));

		// Volume
		WriteLine(F, string("\t<cfg name=\"Volume\" value=\"") + toString(SelectedVolume) + string("\" />"));

		// EULA
		WriteLine(F, string("\t<cfg name=\"EULA\" value=\"") + toString(EULAAccepted) + string("\" />"));

		// Shadows
		WriteLine(F, string("\t<cfg name=\"ShadowDetail\" value=\"") + toString(SelectedShadows) + string("\" />"));

		// Shaders
		WriteLine(F, string("\t<cfg name=\"ShaderQuality\" value=\"") + toString(SelectedQuality) + string("\" />"));

		// Grass distance
		WriteLine(F, string("\t<cfg name=\"GrassDistance\" value=\"") + toString(SelectedGrassDistance) + string("\" />"));

		// Done, close
		WriteLine(F, "</options>");
		CloseFile(F);

		// Save controls
		ControlLayout->Save("Data\\Game Data\\Controls.xml");
	}

	void COptionsMenu::CopyControlsToLocal()
	{
		SelectedResolution = ResolutionCombo->SelectedValue;

		if(AntiAliasCombo->SelectedIndex == 0)
			SelectedAA = 0;
		else
			SelectedAA = toInt(AntiAliasCombo->SelectedValue.substr(1));

		if(AnisotropyCombo->SelectedIndex == 0)
			SelectedAnisotropy = 0;
		else
			SelectedAnisotropy = toInt(AnisotropyCombo->SelectedValue.substr(1));

		SelectedShadows = ShadowDetailCombo->SelectedIndex;
		SelectedQuality = EffectQualityCombo->SelectedIndex;
		SelectedWindowed = WindowCombo->SelectedIndex > 0;
		SelectedVolume = ((float)VolumeTrack->Value) / 100.0f;
		SelectedGrassDistance = GrassDistanceTrack->Value;

		// Update control IDs from editor IDs
		const std::vector<RealmCrafter::IControlLayout::SLayoutInstance*>* Instances = ControlLayout->GetInstances();
		for(unsigned int i = 0; i < Instances->size(); ++i)
			Instances->at(i)->ControlID = Instances->at(i)->EditID;

		// Also have to set globals
		GFXAntiAlias = SelectedAA;
		GFXAnisotropyLevel = SelectedAnisotropy;
		GFXWindowed = SelectedWindowed;
		RealmCrafter::Globals->DefaultVolume = SelectedVolume;
		GFXShadowDetail = SelectedShadows;
		GFXShaderQuality = SelectedQuality;

		// Get selected resolution
		int Divider = SelectedResolution.find("x", 0);
		if(Divider == -1)
			RuntimeError("Invalid resolution format!");
		string SWidth = SelectedResolution.substr(0, Divider - 1);
		string SHeight = SelectedResolution.substr(Divider + 1);
		SWidth = trim(SWidth);
		SHeight = trim(SHeight);
		GFXWidth = toInt(SWidth);
		GFXHeight = toInt(SHeight);
	}	

	void COptionsMenu::CopyLocalToControls()
	{
		for(int i = 0; i < ResolutionCombo->ItemCount(); ++i)
			if(ResolutionCombo->ItemValue[i] == toString(SelectedResolution))
				ResolutionCombo->SelectedIndex = i;

		string AAString = string("x") + toString(SelectedAA);
		string AnisotropyString = string("x") + toString(SelectedAnisotropy);

		if(SelectedAA == 0)
			AntiAliasCombo->SelectedIndex = 0;

		for(int i = 1; i < AntiAliasCombo->ItemCount(); ++i)
			if(AntiAliasCombo->ItemValue[i] == toString(AAString))
				AntiAliasCombo->SelectedIndex = i;

		if(SelectedAnisotropy == 0)
			AnisotropyCombo->SelectedIndex = 0;

		for(int i = 1; i < AnisotropyCombo->ItemCount(); ++i)
			if(AnisotropyCombo->ItemValue[i] == toString(AnisotropyString))
				AnisotropyCombo->SelectedIndex = i;

		ShadowDetailCombo->SelectedIndex = SelectedShadows;
		EffectQualityCombo->SelectedIndex = SelectedQuality;
		WindowCombo->SelectedIndex = SelectedWindowed ? 1 : 0;
		VolumeTrack->Value = (int)(SelectedVolume * 100.0f);
		GrassDistanceTrack->Value = SelectedGrassDistance;

		ControlEditor->SetLayout(ControlLayout);
	}


	void COptionsMenu::ControlEditor_WaitForInputKeyCode(IControl* sender, RealmCrafter::ControlEventArgs* e)
	{
		// Get input control
		int Ctrl = -1;

		// Keyboard
		for(int i = 0; i < 212; ++i)
			if(KeyDown(i))
				Ctrl = i;

		// Mouse
		int MZSpeed = MouseZSpeed();
		int JXSpeed = JoyXDir();
		int JYSpeed = JoyYDir();
		if(MouseDown(1))
			Ctrl = 501;
		else if(MouseDown(2))
			Ctrl = 502;
		else if(MouseDown(3))
			Ctrl = 503;
		// 	else if(MZSpeed > 0)
		// 		Ctrl = 508;
		// 	else if(MZSpeed < 0)
		// 		Ctrl = 509;

		// Joystick
		if(JoyType() > 0)
		{
			if(JYSpeed > 0)
				Ctrl = 1014;
			else if(JYSpeed < 0)
				Ctrl = 1013;
			else if(JXSpeed > 0)
				Ctrl = 1015;
			else if(JXSpeed < 0)
				Ctrl = 1016;

			for(int i = 1; i < 9; ++i)
				if(JoyHit(i))
					Ctrl = 1000 + i;

			if(JoyHat() > -1)
			{
				if(JoyHat() == 0 || JoyHat() == 45 || JoyHat() == 315)
					Ctrl = 1009;
				else if(JoyHat() == 270)
					Ctrl = 1012;
				else if(JoyHat() == 180 || JoyHat() == 135 || JoyHat() == 225)
					Ctrl = 1010;
				else if(JoyHat() == 90)
					Ctrl = 1011;
			}
		}

		// A control has been selected
		if(Ctrl > -1)
		{
			e->GetInstance()->EditID = Ctrl;
			e->SetHandled(true);
		}
	}

	void COptionsMenu::Show()
	{
		CopyLocalToControls();
		Window->Visible = true;
		TabControl->SwitchTo(0);
		Window->BringToFront();
	}

	void COptionsMenu::Hide()
	{
		Window->Visible = false;
	}

	void COptionsMenu::OKButton_Click(NGin::GUI::IControl* sender, RealmCrafter::ControlEventArgs* e)
	{
		// Copy GUI controls to local variables
		CopyControlsToLocal();

		// Save config
		Save();		

		// Hide
		Hide();

		// Apply
		Apply();
	}

	void COptionsMenu::CancelButton_Click(NGin::GUI::IControl* sender, RealmCrafter::ControlEventArgs* e)
	{
		// Hide
		Hide();
	}

	void COptionsMenu::Apply()
	{
		// Sanity check on AA
		// BBDX will do this for us, but when it fails it'll set AA to '0'; here we set it to the most available number.
		if(GFXAntiAlias > CountMultipleSamples(1))
			GFXAntiAlias = CountMultipleSamples(1);

		Graphics3D(GFXWidth, GFXHeight, 32, (GFXWindowed) ? 2 : 1, GFXAntiAlias, GFXAnisotropyLevel, "Data\\DefaultTex.PNG");
		SetQualityLevel(GFXShaderQuality);
		if(GFXShadowDetail == 0)
		{
			ShadowLevel(0);
			SetShadowMapSize(128);
		}else
		{
			++GFXShadowDetail;
			ShadowLevel(5 - GFXShadowDetail);
			SetShadowMapSize(1024);
		}

		int GrassDistance = SelectedGrassDistance;
		if(GrassDistance > 0)
			GrassDistance += 50;
		if(TerrainManager != NULL)
			TerrainManager->SetGrassDrawDistance((float)GrassDistance);


		GUIManager->SetProperties("MainMenu");
		GUIManager->SetProperties("SpellError");
		GUIManager->SetProperties("TextInput");
		GUIManager->SetProperties("ScriptDialog");
		GUIManager->SetProperties("BubbleOutput");
		GUIManager->SetProperties("QuitProgress");
		GUIManager->SetProperties("ChatHistory");
		GUIManager->SetProperties("AttributeBars");
		GUIManager->SetProperties("MapWindow");
		GUIManager->SetProperties("ChatEntry");
		GUIManager->SetProperties("Compass");
		GUIManager->SetProperties("Radar");
		GUIManager->SetProperties("EffectIconSlots");
		GUIManager->SetProperties("ActionBar");
		GUIManager->SetProperties("PartyWindow");
		GUIManager->SetProperties("SpellsWindow");
		GUIManager->SetProperties("SpellRemove");
		GUIManager->SetProperties("QuestLog");
		GUIManager->SetProperties("CharStats");
		GUIManager->SetProperties("HelpWindow");
		GUIManager->SetProperties("MouseSlot");
		GUIManager->SetProperties("Inventory");
		GUIManager->SetProperties("Trading");
		GUIManager->SetProperties("GameMenu");
	}
}