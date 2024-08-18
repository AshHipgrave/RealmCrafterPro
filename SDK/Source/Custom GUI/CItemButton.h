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
#include "SDKMain.h"
#include "IItemButton.h"

namespace RealmCrafter
{
	class CItemButton : public IItemButton
	{
	protected:

		NGin::GUI::IGUIManager* Manager;
		NGin::GUI::IGUIMeshBuffer* MeshBuffer;
		NGin::GUI::IButton* Button;

		NGin::GUI::EventHandler* ClickEvent;
		NGin::GUI::EventHandler* RightClickEvent;
		NGin::GUI::EventHandler* MouseEnterEvent;

		bool DisplayOnly;
		float TimerValue;
		bool CanHoldItems, CanHoldSpells;
		unsigned short HoldingItem;
		unsigned short HoldingSpell;
		unsigned short HoldingAmount;
		std::string BackgroundImage;
		
		void RebuildMesh();

		virtual void OnTransform();
		virtual void OnEnabledChange();
		virtual void OnBackColorChange();
		virtual void OnForeColorChange();
		virtual void OnSkinChange();
		virtual void OnSizeChange();

		virtual void OnDeviceLost();
		virtual void OnDeviceReset();
		virtual bool Update(NGin::GUI::GUIUpdateParameters* Parameters);
		virtual void Render();

		void Button_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void Button_RightClick(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void Button_MouseEnter(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);


	public:

		CItemButton(NGin::Math::Vector2& ScreenScale, NGin::GUI::IGUIManager* Manager);
		virtual ~CItemButton();
		virtual bool Initialize();

		virtual float GetTimerValue();
		virtual void SetTimerValue(float timer);

		virtual unsigned short GetHeldItem();
		virtual unsigned short GetHeldSpell();
		virtual unsigned short GetHeldAmount();
		virtual std::string GetBackgroundImage();
		virtual bool GetCanHoldItems();
		virtual bool GetCanHoldSpells();
		virtual void SetCanHoldItems(bool enable);
		virtual void SetCanHoldSpells(bool enable);
		virtual void SetBackgroundImage(std::string path);
		virtual void SetHeldItem(unsigned short id, unsigned short amount);
		virtual void SetHeldSpell(unsigned short id);

		virtual bool GetDisplayOnly();
		virtual void SetDisplayOnly(bool display);

		virtual NGin::GUI::EventHandler* Click();
		virtual NGin::GUI::EventHandler* RightClick();
		virtual NGin::GUI::EventHandler* MouseEnter();
	};
}