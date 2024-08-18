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
#include "IControlLayout.h"

namespace RealmCrafter
{
	// Control update every arguments
	class ControlEventArgs : public NGin::GUI::EventArgs
	{
		IControlLayout::SLayoutInstance* Instance;
		bool Handled;

	public:

		ControlEventArgs(IControlLayout::SLayoutInstance* instance)
			: Instance(instance), Handled(false)
		{
		}

		// Get the layout instance
		IControlLayout::SLayoutInstance* GetInstance() const
		{
			return Instance;
		}

		// Set whether this event was handled (controls were updated and editing may resume)
		void SetHandled(bool handled)
		{
			Handled = handled;
		}

		// Get whether this event was handled
		bool GetHandled() const
		{
			return Handled;
		}
	};

	// Definition of eventhandler for control editor events
	typedef NGin::IEventHandler<NGin::GUI::IControl, ControlEventArgs> ControlEventHandler;

	// Control Bindings Editor (Interface)
	class IControlEditor : public NGin::GUI::IControl
	{
	public:

		IControlEditor(NGin::Math::Vector2& ScreenScale, NGin::GUI::IGUIManager* Manager)
			: IControl(ScreenScale, Manager) { }
		virtual ~IControlEditor() { }
		
		// Set a control layout to display
		virtual void SetLayout(IControlLayout* layout) = 0;

		// Event
		virtual ControlEventHandler* WaitForInputKeyCode() const = 0;
	};
}