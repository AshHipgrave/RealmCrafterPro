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
		//! Window Interface class
		/*!
		TODO: Link
		*/
		class IWindow : public IControl
		{
		public:

			IWindow(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IWindow() {}

			//! Bring the window to the front of the control list
			virtual void BringToFront() = 0;

			//! Check if the window is the topmost (active) window
			virtual bool IsActiveWindow() = 0;

			//! Resize a control to fit within the client bounds of a window
			virtual void ResizeToClientSize(IControl* Control) = 0;

			//! Visibility of the close button
			__declspec(property(get=zzGet_CloseButton, put=zzPut_CloseButton)) bool CloseButton;
			virtual void zzPut_CloseButton(bool CloseButton) = 0;
			virtual bool zzGet_CloseButton() = 0;

			//! Window dialog type; true = modal
			__declspec(property(get=zzGet_Modal, put=zzPut_Modal)) bool Modal;
			virtual void zzPut_Modal(bool Modal) = 0;
			virtual bool zzGet_Modal() = 0;

			//! EventHandler for closed event
			/*!
			Closed event is called when a user has closed the window
			using the upper right close button.
			*/
			virtual EventHandler* Closed() = 0;

			//! EventHandler for move event
			/*!
			Move event is called when a user has finnished moving a
			window but dragging its title bar.
			*/
			virtual EventHandler* Move() = 0;

			//! Returns the IWindow Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};
	}
}