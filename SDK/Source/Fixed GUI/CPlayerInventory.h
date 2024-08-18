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

#include <IGUIManager.h>
#include "IItemButton.h"
#include "IPlayerInventory.h"

namespace RealmCrafter
{
	class CPlayerInventory : public IPlayerInventory
	{
		SInventory InternalHandle;
		

		NGin::GUI::IGUIManager* GUIManager;
		IItemButton* Slots[50];
		NGin::GUI::IButton* UseButton, *DropButton;
		NGin::GUI::ILabel* MoneyLabel;
		NGin::GUI::IWindow* Window;

		IItemButton* MouseOver;
		unsigned int MouseOverTimer;

		void CreateInventoryButton(int slotID, std::string backgroundImage);
		void Slots_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void Slots_RightClick(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void Slots_MouseEnter(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void UseButton_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void DropButton_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);

		

	public:

		CPlayerInventory(NGin::GUI::IGUIManager* guiManager);
		virtual ~CPlayerInventory();

		virtual void Initialize();
		virtual void Update(SInventory* playerInventory, NGin::Math::Vector2 &mousePosition);

		virtual int GetSlot(IItemButton* button);

		virtual void SetVisible(bool visible);
		virtual bool GetVisible();
		virtual bool IsActiveWindow();
		virtual void BringToFront();

		virtual void SetMoney(std::string money);

		virtual NGin::GUI::EventHandler* Closed();

		
	};
}