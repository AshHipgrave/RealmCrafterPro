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
#include "CMouseToolTip.h"

using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;

// TODO: Make this part of Globals
RealmCrafter::CMouseToolTip* CurrentMouseToolTip = NULL;

namespace RealmCrafter
{
	IMouseToolTip* CreateMouseToolTip(NGin::GUI::IGUIManager* guiManager, IItemButton* parent, std::vector<std::string> &lines, std::vector<NGin::Math::Color> &colors)
	{
		return new CMouseToolTip(guiManager, parent, lines, colors);
	}

	CMouseToolTip::CMouseToolTip(NGin::GUI::IGUIManager* guiManager, IItemButton* parent, std::vector<std::string> &lines, std::vector<NGin::Math::Color> &colors)
		: GUIManager(guiManager), Parent(parent)
	{
		ToolPanel = guiManager->CreatePictureBox("CMouseToolTip::Panel", Vector2(0, 0), Vector2(150, 150) / guiManager->GetResolution());
		
		// Set background to window BG (this can be replaced with an image if necessary)
		ISkin* Skin = GUIManager->GetSkin(1);
		if(Skin != NULL)
		{
			ToolPanel->MinTexCoord = Skin->GetCoord(Window_Fill);
			ToolPanel->MaxTexCoord = ToolPanel->MinTexCoord + Skin->GetSize(Window_Fill);
		}

		float X = 5 / guiManager->GetResolution().X;
		float Y = 5 / guiManager->GetResolution().Y;
		float Yp = 15 / guiManager->GetResolution().Y;
		float Ye = 5 / guiManager->GetResolution().Y;
		float Width = 0.0f;
		ILabel* L;


		for(int i = 0; i < (int)(lines.size() < colors.size() ? lines.size() : colors.size()); ++i)
		{
			L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0));
			Y += Yp;
			L->Parent = ToolPanel;
			L->Text = lines[i];
			L->ForeColor = colors[i];
			if(L->InternalWidth > Width)
				Width = L->InternalWidth;
		}
			
			
			
			
			//L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
			//L->Text = std::string("Type: ") + GetItemType(Itm);

// 
// 
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Health: ") + std::toString(Itm->Hea; "20%";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Value: ") + "200";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Mass: ") + "1";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Can Stack");
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Damage: ") + "5";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Damage Type: ") + "Piercing";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Weapon Type: ") + "One Handed";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Race: ") + "Human";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } L = guiManager->CreateLabel("CMouseToolTip::Label", Vector2(X, Y), Vector2(0, 0)); Y += Yp; L->Parent = ToolPanel;
// 			L->Text = std::string("Class: ") + "Fighter";
// 			if(L->InternalWidth > Width) { Width = L->InternalWidth; } 
// 		}

		ToolPanel->Size = Vector2(Width + X * 2.0f, Y + Ye);
		ToolPanel->Visible = false;
		
	}

	CMouseToolTip::~CMouseToolTip()
	{
		GUIManager->Destroy(ToolPanel);
	}


	IItemButton* CMouseToolTip::GetParent()
	{
		return Parent;
	}

	NGin::GUI::IPictureBox* CMouseToolTip::GetDisplay()
	{
		return ToolPanel;
	}
}