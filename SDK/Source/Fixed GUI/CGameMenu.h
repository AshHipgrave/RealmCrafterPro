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

#include <IGameMenu.h>

namespace RealmCrafter
{
	// In-Game Menu Dialog
	class CGameMenu : public IGameMenu
	{
	public:

		// CTor
		CGameMenu(NGin::GUI::IGUIManager* guiManager);

		// DTor
		~CGameMenu();

		// Setup window GUI
		virtual void Initialize();

		// Show GUI window
		virtual void Show();

		// Hide GUI window
		virtual void Hide();

		// Get whether the menu is visible
		virtual bool IsVisible();

		// Get callback invoked when 'Settings' is clicked
		virtual NGin::GUI::EventHandler* OptionsClick() const;

		// Get callback invoked when a logout is required
		virtual NGin::GUI::EventHandler* Logout() const;

		// Get callback invoked when an exit is required
		virtual NGin::GUI::EventHandler* Quit() const;

		// Get callback invoked when the quit time starts (to halt movement)
		virtual NGin::GUI::EventHandler* LogoutTimerStart() const;

	private:

		NGin::GUI::IGUIManager* GUIManager;

		void OptionsButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void LogoutButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void QuitButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void ResumeButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void OKButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void CancelButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void QuitCancel_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void QuitTimer_Elapsed(NGin::ITimer* sender, NGin::ElapsedEventArgs* e);

		NGin::GUI::IWindow* Window;
		NGin::GUI::IButton* OptionsButton;
		NGin::GUI::IButton* LogoutButton;
		NGin::GUI::IButton* QuitButton;
		NGin::GUI::IButton* ResumeButton;

		NGin::GUI::IWindow* MessageWindow;
		NGin::GUI::IButton* OKButton;
		NGin::GUI::IButton* CancelButton;
		NGin::GUI::ILabel* MessageLabel;
		NGin::GUI::IPictureBox* MessageIcon;

		NGin::GUI::IWindow* QuitWindow;
		NGin::GUI::IProgressBar* QuitProgress;
		NGin::GUI::IButton* QuitCancel;
		NGin::ITimer* QuitTimer;
		unsigned int QuitStart;
		bool Quitting; // Refers to whether we are logging out or quitting (same code used for both)

		NGin::GUI::EventHandler* OptionsClickEvent;
		NGin::GUI::EventHandler* LogoutEvent;
		NGin::GUI::EventHandler* QuitEvent;
		NGin::GUI::EventHandler* LogoutTimerStartEvent;
	};
}