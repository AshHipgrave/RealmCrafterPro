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

#include <vector>
#include <IGUIManager.h>
#include "IItemButton.h"
#include "IMouseToolTip.h"

namespace RealmCrafter
{
	class CMouseToolTip : public IMouseToolTip
	{
		NGin::GUI::IGUIManager* GUIManager;
		NGin::GUI::IPictureBox* ToolPanel;
		IItemButton* Parent;

	public:

		CMouseToolTip(NGin::GUI::IGUIManager* guiManager, IItemButton* parent, std::vector<std::string> &lines, std::vector<NGin::Math::Color> &colors);
		virtual ~CMouseToolTip();

		RealmCrafter::IItemButton* GetParent();
		NGin::GUI::IPictureBox* GetDisplay();
	};

	
}