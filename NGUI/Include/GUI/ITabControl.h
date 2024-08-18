//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
#pragma once

#include <IControl.h>

namespace NGin
{
	namespace GUI
	{
		//! Tab Control
		class ITabControl : public IControl
		{
		public:

			ITabControl(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) { }
			virtual ~ITabControl() { }

			//! Add a new tab with the given name and width
			virtual int AddTab(std::string name, float width) = 0;

			//! Remove a tab at the given index
			virtual void RemoveTab(int index) = 0;

			//! Switch to another tab
			virtual void SwitchTo(int index) = 0;

			//! Get the container handle for a tab
			virtual IPictureBox* TabPanel(int index) = 0;

			//! Event triggered when a tab page changes
			virtual EventHandler* TabChanged() = 0;

			//! Get the number of tabs
			virtual int GetTabCount() = 0;

			//! Get the selected tab index
			virtual int GetSelectedTabIndex() = 0;

			//! Set a tab width
			virtual void SetTabWidth(int index, float width) = 0;

			//! Returns the Control Type
			static NGin::Type TypeOf();
		};
	}
}