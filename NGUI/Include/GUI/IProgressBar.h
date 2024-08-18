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

#include "IControl.h"

namespace NGin
{
	namespace GUI
	{
		//! ProgressBar Interface class
		/*!
		TODO: Link
		*/
		class IProgressBar : public IControl
		{
		public:

			IProgressBar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IProgressBar() {}

			//! Progressbar value, from 0 - 100
			__declspec(property(get=zzGet_Value, put=zzPut_Value)) int Value;
			virtual void zzPut_Value(int Value) = 0;
			virtual int zzGet_Value() = 0;

			//! EventHandler for click event
			/*!
			Click event is called whenever a user has clicked this control
			*/
			virtual EventHandler* Click() = 0;

			//! EventHandler for right click event
			/*!
			Click event is called whenever a user has right clicked this control
			*/
			virtual EventHandler* RightClick() = 0;

			//! EventHandler for the mouse being held down
			virtual EventHandler* MouseDown() = 0;

			//! EventHandler for the mouse entering the control bounds
			virtual EventHandler* MouseEnter() = 0;

			//! EventHandler for the mouse leaving the control bounds
			virtual EventHandler* MouseLeave() = 0;

			//! EventHandler for when a mouse is moved over a control
			virtual MouseEventHandler* MouseMove() = 0;

			//! Returns the IProgressBar Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};
	}
}