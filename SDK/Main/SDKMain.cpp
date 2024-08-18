//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################

#include "SDKMain.h"
#include "..\Source\CCameraController.h"
#include "..\Source\Fixed GUI\CControlEditor.h"
#include "..\Source\Fixed GUI\CServerSelector.h"
#include "..\Source\CDefaultEnvironment.h"
#include "..\Source\CControlHostManager.h"
#include "..\Source\Custom GUI\CWindowHost.h"
#include "..\Source\Custom GUI\CButtonHost.h"
#include "..\Source\Custom GUI\CCheckBoxHost.h"
#include "..\Source\Custom GUI\CComboBoxHost.h"
#include "..\Source\Custom GUI\CLabelHost.h"
#include "..\Source\Custom GUI\CListBoxHost.h"
#include "..\Source\Custom GUI\CPictureBoxHost.h"
#include "..\Source\Custom GUI\CProgressBarHost.h"
#include "..\Source\Custom GUI\CTextBoxHost.h"
#include "..\Source\Custom GUI\CTrackBarHost.h"
#include "..\Source\Custom GUI\CSelectionSizeHost.h"
#include "..\Source\Fixed GUI\CGameMenu.h"
#include "..\Source\Fixed GUI\CChatBox.h"
#include "..\Source\Custom GUI\CItemButtonHost.h"
#include "..\Source\DemoGame\CDemoEnvironment.h"
//#include "..\CMCLevelHost.h"

// Input command data
BBDXCommandData* BBDXCommands = NULL;
BBCommandData* BBCommands = NULL;

// Camera controller
RealmCrafter::CCameraController* CameraController = NULL;

// Environment handling
RealmCrafter::IEnvironment* CurrentEnvironment = NULL;
std::vector<RealmCrafter::IEnvironment*> Environments;

// Init SDK
void SDKMain(void* bbdxCommandData, void* bbCommandData, uint camera, NGin::GUI::IGUIManager* guiManager)
{
	// Setup BB and BBDX command imports
	BBDXCommands = reinterpret_cast<BBDXCommandData*>(bbdxCommandData);
	BBCommands = reinterpret_cast<BBCommandData*>(bbCommandData);

	// Create a camera controller
	// User: Modify this to your camera controller
	CameraController = new RealmCrafter::CCameraController();

	// Create fixed dialogs
	RealmCrafter::Globals->ServerSelector = new RealmCrafter::CServerSelector(guiManager);
	RealmCrafter::Globals->GameMenu = new RealmCrafter::CGameMenu(guiManager);
	RealmCrafter::Globals->ChatBox = new RealmCrafter::CChatBox(guiManager);
	RealmCrafter::Globals->ChatBox->Initialize();

	// Add available environments to the Environments list
	// User: Add your environments here
	Environments.push_back(new RealmCrafter::CDefaultEnvironment(camera));
	//Environments.push_back(new RealmCrafter::CDemoEnvironment(camera));

	// Create Control Host Manager
	// User: Add you own types at the end of the list
	RealmCrafter::Globals->ControlHostManager = new RealmCrafter::SDK::CControlHostManager(guiManager);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::IWindow::TypeOf())->AddEvent(RealmCrafter::SDK::CWindowHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::IButton::TypeOf())->AddEvent(RealmCrafter::SDK::CButtonHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::ICheckBox::TypeOf())->AddEvent(RealmCrafter::SDK::CCheckBoxHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::IComboBox::TypeOf())->AddEvent(RealmCrafter::SDK::CComboBoxHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::ILabel::TypeOf())->AddEvent(RealmCrafter::SDK::CLabelHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::IListBox::TypeOf())->AddEvent(RealmCrafter::SDK::CListBoxHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::IPictureBox::TypeOf())->AddEvent(RealmCrafter::SDK::CPictureBoxHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::IProgressBar::TypeOf())->AddEvent(RealmCrafter::SDK::CProgressBarHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::ITextBox::TypeOf())->AddEvent(RealmCrafter::SDK::CTextBoxHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::ITrackBar::TypeOf())->AddEvent(RealmCrafter::SDK::CTrackBarHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::GUI::CSelectionSize::TypeOf())->AddEvent(RealmCrafter::SDK::CSelectionSizeHost::Create);
	RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::Type("ZITB", ""))->AddEvent(RealmCrafter::SDK::CItemButtonHost::Create);
	//RealmCrafter::Globals->ControlHostManager->RegisterTemplateFactory(NGin::Type("ZAPL", ""))->AddEvent(RealmCrafter::SDK::ApplesHost::Create);
}

DLLOUT void ResolutionChanged(float newWidth, float newHeight)
{
	NGin::Math::Vector2 NewResolution(newWidth, newHeight);

	// Can add custom controls here for resolution update capture
	RealmCrafter::Globals->ChatBox->ResolutionChange(NewResolution);
}

// Change existing environment to a new one
DLLOUT void ChangeEnvironment(std::string environmentName, std::string zoneName)
{
	// Unload existing
	if(CurrentEnvironment != NULL)
		CurrentEnvironment->UnloadArea();
	CurrentEnvironment = NULL;

	// Find new
	for(unsigned int i = 0; i < Environments.size(); ++i)
	{
		if(Environments[i]->GetName() == environmentName)
		{
			CurrentEnvironment = Environments[i];
			CurrentEnvironment->LoadArea(zoneName);
			break;
		}
	}
}

// Hide the current environment
DLLOUT void EnvHide()
{
	if(CurrentEnvironment == NULL)
		return;

	CurrentEnvironment->Hide();
}

// Show the current environment
DLLOUT void EnvShow()
{
	if(CurrentEnvironment == NULL)
		return;

	CurrentEnvironment->Show();
}

// Set the current time
DLLOUT void SetEnvTime(int timeH, int timeM, float secondInterpolation)
{
	if(CurrentEnvironment == NULL)
		return;

	CurrentEnvironment->SetTime(timeH, timeM, secondInterpolation);
}

// Set the current date
DLLOUT void SetEnvDate(int* day, int* year)
{
	if(CurrentEnvironment == NULL)
		return;

	CurrentEnvironment->SetDate(day, year);
}

// Update current environment
DLLOUT void UpdateEnvironment(float deltaTime)
{
	// This function is called each frame, and its also used for updating other SDK 'dynamic' objects
	if(RealmCrafter::Globals->ChatBox != NULL)
		((RealmCrafter::CChatBox*)RealmCrafter::Globals->ChatBox)->Update();

	// The real environment stuff
	if(CurrentEnvironment != NULL)
		CurrentEnvironment->Update(deltaTime);
}

// Set whether camera is underwater
DLLOUT void SetEnvCameraUnderWater(bool under, unsigned char destR, unsigned char destG, unsigned char destB)
{
	if(CurrentEnvironment == NULL)
		return;

	CurrentEnvironment->SetCameraUnderWater(under, destR, destG, destB);
}

// Update current environment
DLLOUT void ProcessEnvNetPacket(RealmCrafter::PacketReader &reader)
{
	if(CurrentEnvironment == NULL)
		return;

	CurrentEnvironment->ProcessNetPacket(reader);
}

// Checked whether the footstep sound is 'wet'
DLLOUT bool EnvPlayWetFootstep()
{
	if(CurrentEnvironment == NULL)
		return false;

	return CurrentEnvironment->PlayWetFootstep();
}


DLLOUT RealmCrafter::ICameraController* GetCameraController()
{
	return CameraController;
}

DLLOUT RealmCrafter::IControlEditor* CreateControlEditor(NGin::GUI::IGUIManager* manager, std::string name, NGin::Math::Vector2 location, NGin::Math::Vector2 size)
{
	return RealmCrafter::CControlEditor::Create(manager, name, location, size);
}
