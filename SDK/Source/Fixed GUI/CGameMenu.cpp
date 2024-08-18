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
#pragma once

#include "CGameMenu.h"

using namespace NGin;
using namespace NGin::GUI;
using namespace NGin::Math;
using namespace std;

namespace RealmCrafter
{
	CGameMenu::CGameMenu(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), QuitStart(0), Quitting(false)
	{
		OptionsClickEvent = new NGin::GUI::EventHandler();
		LogoutEvent = new NGin::GUI::EventHandler();
		QuitEvent = new NGin::GUI::EventHandler();
		LogoutTimerStartEvent = new NGin::GUI::EventHandler();
	}

	CGameMenu::~CGameMenu()
	{
	}

	void CGameMenu::Initialize()
	{
#define R(x, y) (NGin::Math::Vector2(x, y) / GUIManager->GetResolution())

#pragma region Main GUI

		// Center window
		Vector2 Size = R(186, 204);
		Vector2 Position = ((R(GUIManager->GetResolution().X, GUIManager->GetResolution().Y)) / 2) - (Size / 2);
		
		Window = GUIManager->CreateWindow("GameMenu::Window", Position, Size);
		OptionsButton = GUIManager->CreateButton("GameMenu::OptionsButton", R(12, 12), R(150, 24));
		LogoutButton = GUIManager->CreateButton("GameMenu::LogoutButton", R(12, 42), R(150, 24));
		QuitButton = GUIManager->CreateButton("GameMenu::QuitButton", R(12, 72), R(150, 24));
		ResumeButton = GUIManager->CreateButton("GameMenu::ResumeButton", R(12, 132), R(150, 24));

		Window->Text = "Menu";
		OptionsButton->Text = "Settings";
		LogoutButton->Text = "Logout";
		QuitButton->Text = "Exit Game";
		ResumeButton->Text = "Resume Game";

		OptionsButton->Click()->AddEvent(this, &CGameMenu::OptionsButton_Click);
		LogoutButton->Click()->AddEvent(this, &CGameMenu::LogoutButton_Click);
		QuitButton->Click()->AddEvent(this, &CGameMenu::QuitButton_Click);
		ResumeButton->Click()->AddEvent(this, &CGameMenu::ResumeButton_Click);

		OptionsButton->Parent = Window;
		LogoutButton->Parent = Window;
		QuitButton->Parent = Window;
		ResumeButton->Parent = Window;

		Window->Modal = true;
		Window->CloseButton = false;
		Window->Closed()->AddEvent(this, &CGameMenu::ResumeButton_Click);
		Window->Visible = false;
#pragma endregion

#pragma region Confirm GUI

		Size = R(395, 115);
		Position = ((R(GUIManager->GetResolution().X, GUIManager->GetResolution().Y)) / 2) - (Size / 2);

		MessageWindow = GUIManager->CreateWindow("GameMenu::MessageWindow", Position, Size);
		MessageLabel = GUIManager->CreateLabel("GameMenu::MessageLabel", R(50, 12), R(312, 35));
		MessageIcon = GUIManager->CreatePictureBox("GameMenu::MessageIcon", R(12, 12), R(32, 32));
		OKButton = GUIManager->CreateButton("GameMenu::OKButton", R(119, 47), R(75, 23));
		CancelButton = GUIManager->CreateButton("GameMenu::CancelButton", R(200, 47), R(75, 23));

		MessageIcon->SetImage(string("Data\\UI\\Warning.png"));
		MessageLabel->Multiline = true;
		MessageLabel->VAlign = TextAlign_Middle;
		OKButton->Text = "OK";
		CancelButton->Text = "Cancel";

		MessageLabel->Parent = MessageWindow;
		MessageIcon->Parent = MessageWindow;
		OKButton->Parent = MessageWindow;
		CancelButton->Parent = MessageWindow;

		Window->Modal = true;

		OKButton->Click()->AddEvent(this, &CGameMenu::OKButton_Click);
		CancelButton->Click()->AddEvent(this, &CGameMenu::CancelButton_Click);
		Window->Closed()->AddEvent(this, &CGameMenu::CancelButton_Click);

		MessageWindow->Visible = false;
#pragma endregion

#pragma region Quit Progress

		Size = R(239, 108);
		Position = ((R(GUIManager->GetResolution().X, GUIManager->GetResolution().Y)) / 2) - (Size / 2);

		QuitWindow = GUIManager->CreateWindow("GameMenu::QuitWindow", Position, Size);
		QuitProgress = GUIManager->CreateProgressBar("GameMenu::QuitProgress", R(12, 12), R(200, 23));
		QuitCancel = GUIManager->CreateButton("GameMenu::QuitCancel", R(74, 41), R(75, 23));

		QuitWindow->Text = "Leaving...";
		QuitWindow->Modal = true;
		QuitWindow->CloseButton = false;
		QuitWindow->Visible = false;

		QuitProgress->Parent = QuitWindow;
		QuitCancel->Text = "Cancel";
		QuitCancel->Parent = QuitWindow;
		QuitCancel->Click()->AddEvent(this, &CGameMenu::QuitCancel_Click);

		// Create a timer with no interval.. meaning its called once per frame!
		QuitTimer = Globals->TimerManager->CreateTimer(0);
		QuitTimer->AutoReset(true);
		QuitTimer->Stop();
		QuitTimer->Elapsed()->AddEvent(this, &CGameMenu::QuitTimer_Elapsed);

#undef R

		// Apply propertysheet
		GUIManager->SetProperties("GameMenu");
	}

	void CGameMenu::Show()
	{
		Window->Visible = true;
		Window->BringToFront();
	}

	void CGameMenu::Hide()
	{
		if(QuitWindow->Visible)
		{
			QuitCancel_Click(QuitCancel, NULL);
		}
		else if(MessageWindow->Visible)
		{
			MessageWindow->Visible = false;
			Window->BringToFront();
			return;
		}else
		{
			Window->Visible = false;
		}
	}

	void CGameMenu::OptionsButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		if(OptionsClickEvent != NULL)
			OptionsClickEvent->Execute(sender, NULL);
	}

	void CGameMenu::LogoutButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		Quitting = false;

		MessageWindow->Text = "Confirm...";
		MessageLabel->Text = "Are you sure you want to logout?";
		MessageWindow->Visible = true;
		MessageWindow->BringToFront();
	}

	void CGameMenu::QuitButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		Quitting = true;

		MessageWindow->Text = "Confirm...";
		MessageLabel->Text = "Are you sure you want to exit?";
		MessageWindow->Visible = true;
		MessageWindow->BringToFront();
	}

	void CGameMenu::ResumeButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		Window->Visible = false;
	}

	void CGameMenu::OKButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		MessageWindow->Visible = false;
		Window->Visible = false;

		QuitProgress->Value = 0;
		QuitWindow->Visible = true;
		QuitWindow->BringToFront();
		QuitStart = MilliSecs();
		QuitTimer->Start();
		if(LogoutTimerStartEvent != NULL)
			LogoutTimerStartEvent->Execute(Window, NULL);
	}

	void CGameMenu::CancelButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		Window->Visible = true;
		MessageWindow->Visible = false;
		Window->BringToFront();
	}

	void CGameMenu::QuitCancel_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		// Stop timer
		QuitTimer->Stop();
		QuitStart = 0;

		// Hide quit window
		QuitWindow->Visible = false;
		Window->Visible = false;
	}

	void CGameMenu::QuitTimer_Elapsed(NGin::ITimer* sender, NGin::ElapsedEventArgs* e)
	{
		if(QuitStart == 0)
			return;
		if(Globals->QuitActive == false)
		{
			QuitCancel_Click(QuitCancel, NULL);
			return;
		}

		// We use 'millisecs' to elapse the progressbar since
		// ITimer isn't precise and works to the nearest values
		unsigned int ElapsedTime = MilliSecs() - QuitStart;
		int Progress = ElapsedTime / 100;
		Progress = Progress > 100 ? 100 : Progress;

		// Update Bar
		QuitProgress->Value = Progress;

		// Quit/Logout if necessary
		if(ElapsedTime > 10000)
		{
			QuitTimer->Stop();
			QuitStart = 0;
			QuitWindow->Visible = false;
			MessageWindow->Visible = false;
			Window->Visible = false;

			// Callbacks
			if(Quitting)
			{
				if(QuitEvent != NULL)
					QuitEvent->Execute(Window, NULL);
			}else
			{
				if(LogoutEvent != NULL)
					LogoutEvent->Execute(Window, NULL);
			}
		}
	}

	bool CGameMenu::IsVisible()
	{
		return Window->Visible || MessageWindow->Visible || QuitWindow->Visible;
	}

	NGin::GUI::EventHandler* CGameMenu::OptionsClick() const
	{
		return OptionsClickEvent;
	}

	NGin::GUI::EventHandler* CGameMenu::Logout() const
	{
		return LogoutEvent;
	}

	NGin::GUI::EventHandler* CGameMenu::Quit() const
	{
		return QuitEvent;
	}

	NGin::GUI::EventHandler* CGameMenu::LogoutTimerStart() const
	{
		return LogoutTimerStartEvent;
	}
}