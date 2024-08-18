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
#include "CPlayerParty.h"
#include "RealmCrafter.h"

namespace RealmCrafter
{
	IPlayerParty* CreatePlayerParty(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerParty(guiManager);
	}

	CPlayerParty::CPlayerParty(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), Window(NULL)
	{
		ClickEventHandler = new NGin::GUI::EventHandler();
		LeaveEventHandler = new NGin::GUI::EventHandler();
	}

	CPlayerParty::~CPlayerParty()
	{
		GUIManager->Destroy(Window);

		delete ClickEventHandler;
		delete LeaveEventHandler;
	}

	void CPlayerParty::Initialize()
	{
#define R(x, y) (NGin::Math::Vector2(x, y) / GUIManager->GetResolution())

		NGin::Math::Vector2 windowSize = R(378, 383);

		Window = GUIManager->CreateWindow("PartyWindow::Window", NGin::Math::Vector2(0.3f, 0.3f),NGin:: Math::Vector2(0.4f, 0.4f));
		Window->Text = "Party";

		LeaveButton = GUIManager->CreateButton("PartyWindow::LeaveButton", NGin::Math::Vector2(0.6f, 0.85f), NGin::Math::Vector2(0.35f, 0.1f));
		LeaveButton->Text = "Leave";
		LeaveButton->Parent = Window;
		LeaveButton->Click()->AddEvent(this, &CPlayerParty::LeaveButton_Click);

		float Y = 0.05f;
		for(int i = 0; i < 8; ++i)
		{
			NGin::GUI::ILabel* label;

			label = GUIManager->CreateLabel(std::string("PartyWindow::Name") + std::toString(i), NGin::Math::Vector2(0.05f, Y), NGin::Math::Vector2(0, 0));
			label->Text = "";
			label->ForeColor = NGin::Math::Color(0, 255, 0);
			label->Parent = Window;
			label->MouseEnter()->AddEvent(this, &CPlayerParty::label_MouseEnter);
			label->MouseLeave()->AddEvent(this, &CPlayerParty::label_MouseLeave);
			label->Click()->AddEvent(this, &CPlayerParty::label_Click);
			Y += 0.07f;

			Names.push_back(label);
		}

		GUIManager->SetProperties("PartyWindow");
#undef R
	}

	void CPlayerParty::Update(std::string* names, int nameCount)
	{
		int i = 0;
		for(; i < nameCount && i < (int)Names.size(); ++i)
		{
			Names[i]->Text = names[i];
			Names[i]->ForeColor = NGin::Math::Color(0, 255, 0);
		}

		for(; i < (int)Names.size(); ++i)
		{
			Names[i]->Text = "";
		}
	}

	void CPlayerParty::LeaveButton_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		LeaveEventHandler->Execute(control, e);
	}

	void CPlayerParty::label_MouseEnter(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		sender->ForeColor = NGin::Math::Color(0, 125, 255);
	}

	void CPlayerParty::label_MouseLeave(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		sender->ForeColor = NGin::Math::Color(0, 255, 0);
	}

	void CPlayerParty::label_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		ClickEventHandler->Execute(sender, e);
	}

	void CPlayerParty::SetVisible(bool visible)
	{
		Window->Visible = visible;
		if(visible)
			Window->BringToFront();
	}

	bool CPlayerParty::GetVisible()
	{
		return Window->Visible;
	}

	bool CPlayerParty::IsActiveWindow()
	{
		return Window->IsActiveWindow();
	}

	NGin::GUI::EventHandler* CPlayerParty::Closed()
	{
		return Window->Closed();
	}

	NGin::GUI::EventHandler* CPlayerParty::Click()
	{
		return ClickEventHandler;
	}

	NGin::GUI::EventHandler* CPlayerParty::Leave()
	{
		return LeaveEventHandler;
	}
}