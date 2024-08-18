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
#include "CPlayerHelp.h"
#include "RealmCrafter.h"

namespace RealmCrafter
{
	IPlayerHelp* CreatePlayerHelp(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerHelp(guiManager);
	}

	CPlayerHelp::CPlayerHelp(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), Window(NULL)
	{
		
	}

	CPlayerHelp::~CPlayerHelp()
	{
		GUIManager->Destroy(Window);
	}

	void CPlayerHelp::Initialize()
	{
#define R(x, y) (NGin::Math::Vector2(x, y) / GUIManager->GetResolution())

		NGin::Math::Vector2 windowSize = R(378, 383);

		Window = GUIManager->CreateWindow("HelpWindow::Window", NGin::Math::Vector2(0.5f, 0.5f) - (windowSize * 0.5f), windowSize);
		Window->Text = "Help";

		
		Label = GUIManager->CreateLabel("HelpWindow::Label", R(12, 9), R(325, 360));
		Label->Parent = Window;
		Label->Multiline = true;
		Label->ScissorWindow = R(325, 330);
		Label->ForceScissoring = true;

		ScrollBar = GUIManager->CreateScrollBar("HelpWindow::ScrollBar", R(340, 9), R(0, 330), NGin::GUI::VerticalScroll);
		ScrollBar->Scroll()->AddEvent(this, &CPlayerHelp::ScrollBar_Scroll);
		ScrollBar->Parent = Window;

		FILE* F = ReadFile("Data\\Game Data\\Help.txt");

		if( F != NULL )
		{
			std::string HelpText = "";

			while(!Eof(F))
				HelpText.append(ReadLine(F));

			CloseFile(F);

			Label->Text = HelpText;
		}

		float TopSize = (Label->InternalHeight * GUIManager->GetResolution().Y) + 7;

		ScrollBar->Minimum = 0;
		ScrollBar->Maximum = (int)(TopSize + 0.5f);
		ScrollBar->LargeChange = 330;

		if(TopSize < 330)
			ScrollBar->Enabled = false;

		GUIManager->SetProperties("HelpWindow");
#undef R
	}

	void CPlayerHelp::ScrollBar_Scroll(NGin::GUI::IControl* control, NGin::GUI::ScrollEventArgs* e)
	{
		Label->ScrollOffset = NGin::Math::Vector2(0, (float)-e->NewValue()) / GUIManager->GetResolution();
	}

	void CPlayerHelp::SetVisible(bool visible)
	{
		Window->Visible = visible;
		if(visible)
			Window->BringToFront();
	}

	bool CPlayerHelp::GetVisible()
	{
		return Window->Visible;
	}

	bool CPlayerHelp::IsActiveWindow()
	{
		return Window->IsActiveWindow();
	}

	NGin::GUI::EventHandler* CPlayerHelp::Closed()
	{
		return Window->Closed();
	}
}