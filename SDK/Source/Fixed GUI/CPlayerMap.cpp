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
#include "CPlayerMap.h"

namespace RealmCrafter
{
	IPlayerMap* CreatePlayerMap(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerMap(guiManager);
	}

	CPlayerMap::CPlayerMap(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), Window(NULL), PictureBox(NULL)
	{
		
	}

	CPlayerMap::~CPlayerMap()
	{
		GUIManager->Destroy(Window);
	}

	void CPlayerMap::Initialize()
	{
		Window = GUIManager->CreateWindow("MapWindow::Window", NGin::Math::Vector2(0.15f, 0.1f), NGin::Math::Vector2(0.6f, 0.8f));
		Window->Text =  "Map";

		PictureBox = GUIManager->CreatePictureBox(std::string("Map PictureBox"), NGin::Math::Vector2(0, 0), NGin::Math::Vector2(1, 1));
		PictureBox->Parent = Window;
		Window->ResizeToClientSize(PictureBox);
		Window->Visible = false;

		GUIManager->SetProperties("MapWindow");
	}

	void CPlayerMap::SetImage(const std::string& imagePath)
	{
		PictureBox->SetImage(imagePath);
	}

	bool CPlayerMap::IsActiveWindow()
	{
		return Window->IsActiveWindow();
	}

	void CPlayerMap::SetVisible(bool visible)
	{
		Window->Visible = visible;

		if(visible)
			Window->BringToFront();
	}

	bool CPlayerMap::GetVisible()
	{
		return Window->Visible;
	}

	NGin::GUI::EventHandler* CPlayerMap::Closed()
	{
		return Window->Closed();
	}
}