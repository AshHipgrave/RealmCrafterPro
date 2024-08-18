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
#include "IGUITexture.h"
#include <Vector4.h>

namespace NGin
{
	namespace GUI
	{
		//! PictureBox Interface class
		/*!
		TODO: Link
		*/
		class IPictureBox : public IControl
		{
		public:

			IPictureBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IPictureBox() {}

			//! Set the picturebox background image
			/*!
			\param ImageFile Path to file to load
			\return Load status, returns false on error
			*/
			virtual bool SetImage(std::string ImageFile) = 0;

			//! Set the picturebox background image with a mask
			/*!
			\param ImageFile Path to file to load
			\param Mask Integer value of color to mask
			\return Load status, returns false on error
			*/
			virtual bool SetImage(std::string ImageFile, unsigned int Mask) = 0;

			//! Set the box texture using a pre-loaded file
			virtual void SetImage(IGUITexture* texture) = 0;

			//! Set the background image of the box directly
			virtual void SetImage(void* Texture) = 0;

			//! Minimum texture coordinate
			__declspec(property(get=zzGet_MinTexCoord, put=zzPut_MinTexCoord)) Math::Vector2 MinTexCoord;
			virtual void zzPut_MinTexCoord(Math::Vector2 Coord) = 0;
			virtual Math::Vector2 zzGet_MinTexCoord() = 0;

			//! Maximum texture coordinate
			__declspec(property(get=zzGet_MaxTexCoord, put=zzPut_MaxTexCoord)) Math::Vector2 MaxTexCoord;
			virtual void zzPut_MaxTexCoord(Math::Vector2 Coord) = 0;
			virtual Math::Vector2 zzGet_MaxTexCoord() = 0;

			//! PictureBox controls are used for cursors, so we remove MouseBusy modification for them
			__declspec(property(get=zzGet_IsMouseCursor, put=zzPut_IsMouseCursor)) bool IsMouseCursor;
			virtual void zzPut_IsMouseCursor(bool isMouseCursor) = 0;
			virtual bool zzGet_IsMouseCursor() = 0;

			//! Force an overlay scissor region for controls which are 'children' of custom controls
			__declspec(property(get=zzGet_ForceScissoring, put=zzPut_ForceScissoring)) bool ForceScissoring;
			virtual void zzPut_ForceScissoring(bool forceScissoring) = 0;
			virtual bool zzGet_ForceScissoring() = 0;

			__declspec(property(get=zzGet_ScissorWindow, put=zzPut_ScissorWindow)) Math::Vector4 ScissorWindow;
			virtual void zzPut_ScissorWindow(Math::Vector4 &scissorWindow) = 0;
			virtual Math::Vector4 zzGet_ScissorWindow() = 0;

			//! Returns the IPictureBox Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};
	}
}