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
#include "CPlayerStats.h"
#include "RealmCrafter.h"

namespace RealmCrafter
{
	IPlayerStats* CreatePlayerStats(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerStats(guiManager);
	}

	CPlayerStats::CPlayerStats(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), Window(NULL)
	{
		
	}

	CPlayerStats::~CPlayerStats()
	{
		GUIManager->Destroy(Window);
	}

	void CPlayerStats::Initialize()
	{
#define R(x, y) (NGin::Math::Vector2(x, y) / GUIManager->GetResolution())

		NGin::Math::Vector2 windowSize = R(311, 270);

		Window = GUIManager->CreateWindow("NewCharStats::Window", NGin::Math::Vector2(0.5f, 0.5f) - (windowSize * 0.5f), windowSize);
		Window->Text = "Character";

		NGin::GUI::ILabel* label;
		
		label = GUIManager->CreateLabel("NewCharStats::Label", R(12, 9), R(0, 0));
		label->Text = "Name: ";
		label->Parent = Window;

		label = GUIManager->CreateLabel("NewCharStats::Label", R(12, 22), R(0, 0));
		label->Text = "Level: ";
		label->Parent = Window;

		label = GUIManager->CreateLabel("NewCharStats::Label", R(12, 35), R(0, 0));
		label->Text = "XP: ";
		label->Parent = Window;

		label = GUIManager->CreateLabel("NewCharStats::Label", R(12, 48), R(0, 0));
		label->Text = "Money: ";
		label->Parent = Window;

		label = GUIManager->CreateLabel("NewCharStats::Label", R(12, 61), R(0, 0));
		label->Text = "Reputation: ";
		label->Parent = Window;

		NameLabel = GUIManager->CreateLabel("NewCharStats::Label", R(148, 9), R(130, 0));
		NameLabel->Align = NGin::GUI::TextAlign_Right;
		NameLabel->Parent = Window;

		LevelLabel = GUIManager->CreateLabel("NewCharStats::Label", R(148, 22), R(130, 0));
		LevelLabel->Align = NGin::GUI::TextAlign_Right;
		LevelLabel->Parent = Window;

		XPLabel = GUIManager->CreateLabel("NewCharStats::Label", R(148, 35), R(130, 0));
		XPLabel->Align = NGin::GUI::TextAlign_Right;
		XPLabel->Parent = Window;

		GoldLabel = GUIManager->CreateLabel("NewCharStats::Label", R(148, 48), R(130, 0));
		GoldLabel->Align = NGin::GUI::TextAlign_Right;
		GoldLabel->Parent = Window;

		ReputationLabel = GUIManager->CreateLabel("NewCharStats::Label", R(148, 61), R(130, 0));
		ReputationLabel->Align = NGin::GUI::TextAlign_Right;
		ReputationLabel->Parent = Window;

		for(int i = 0; i < 10; ++i)
		{
			label = GUIManager->CreateLabel("NewCharStats::Label", R(12, (float)88 + i * 13), R(0, 0));
			label->Text = "";
			label->Parent = Window;

			AttributeNames.push_back(label);

			label = GUIManager->CreateLabel("NewCharStats::Label", R(148, (float)88 + i * 13), R(130, 0));
			label->Text = "";
			label->Align = NGin::GUI::TextAlign_Right;
			label->Parent = Window;

			AttributeStats.push_back(label);
		}

		GUIManager->SetProperties("NewCharStats");
		Window->Visible = true;
#undef R
	}

	void CPlayerStats::Update(SDK::IActorInstance* actorInstance)
	{
		NameLabel->Text = actorInstance->GetName();
		LevelLabel->Text = std::toString(actorInstance->GetLevel());
		XPLabel->Text = std::toString(actorInstance->GetXP());
		GoldLabel->Text = std::toString(actorInstance->GetMoney());
		ReputationLabel->Text = std::toString(actorInstance->GetReputation());

		SAttributes* attributes = actorInstance->GetAttributes();

		int i = 0;
		int v = 0;
		for(; i < 40 && v < (int)AttributeNames.size(); ++i, ++v)
		{
			if(Globals->Attributes->Hidden[i] || Globals->Attributes->Name[i].length() == 0)
			{
				--v;
				continue;
			}

			AttributeNames[v]->Text = Globals->Attributes->Name[i];
			AttributeStats[v]->Text = std::toString(attributes->Value[i]) + " / " + std::toString(attributes->Maximum[i]);
		}
	}

	void CPlayerStats::SetVisible(bool visible)
	{
		Window->Visible = visible;
		if(visible)
			Window->BringToFront();
	}

	bool CPlayerStats::GetVisible()
	{
		return Window->Visible;
	}

	bool CPlayerStats::IsActiveWindow()
	{
		return Window->IsActiveWindow();
	}

	NGin::GUI::EventHandler* CPlayerStats::Closed()
	{
		return Window->Closed();
	}
}