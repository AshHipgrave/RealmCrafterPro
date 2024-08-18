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
//#include "ILabel.h"

namespace NGin
{
	namespace GUI
	{
		enum TextAlign;

		//! Button Interface class
		/*!
		TODO: Button Link
		*/
		class IButton : public IControl
		{
		public:

			IButton(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IButton() {}

			//! Horizontal text alignment
			__declspec(property(get=zzGet_Align, put=zzPut_Align)) TextAlign Align;
			virtual TextAlign zzGet_Align() = 0;
			virtual void zzPut_Align(TextAlign Alignment) = 0;

			//! Vertical text alignment
			__declspec(property(get=zzGet_VAlign, put=zzPut_VAlign)) TextAlign VAlign;
			virtual TextAlign zzGet_VAlign() = 0;
			virtual void zzPut_VAlign(TextAlign Alignment) = 0;

			//! UseBorder property.
			__declspec(property(get=zzGet_UseBorder, put=zzPut_UseBorder)) bool UseBorder;
			virtual bool zzGet_UseBorder() = 0;
			virtual void zzPut_UseBorder(bool UseBorder) = 0;

			//! Set the button background image
			/*!
			\param ImageFile Path to file to load
			\return Load status, returns false on error
			*/
			virtual bool SetUpImage(std::string ImageFile) = 0;
			virtual bool SetHoverImage(std::string ImageFile) = 0;
			virtual bool SetDownImage(std::string ImageFile) = 0;

			//! Set the button background image with a mask
			/*!
			\param ImageFile Path to file to load
			\param Mask Integer value of color to mask
			\return Load status, returns false on error
			*/
			virtual bool SetUpImage(std::string ImageFile, unsigned int Mask) = 0;
			virtual bool SetHoverImage(std::string ImageFile, unsigned int Mask) = 0;
			virtual bool SetDownImage(std::string ImageFile, unsigned int Mask) = 0;

			//! Set the background image of the box directly
			virtual void SetUpImage(void* Texture) = 0;
			virtual void SetHoverImage(void* Texture) = 0;
			virtual void SetDownImage(void* Texture) = 0;

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

			//! Added for the sake of RC
			__declspec(property(get=zzGet_Down, put=zzPut_Down)) bool Down;
			virtual bool zzGet_Down() = 0;
			virtual void zzPut_Down(bool Down) = 0;

			//! Returns the IButton Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};

	}
}