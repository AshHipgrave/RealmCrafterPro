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
#include "IGUIManager.h"

namespace NGin
{
	namespace GUI
	{
		//! Font Interface class
		/*!
		GUI Font class to obtain information about font characters
		for rendering onscreen fonts
		*/
		class IFont
		{
		public:

			IFont(){}
			virtual ~IFont(){}

			//! Get texture UV of upper left corner of a character
			/*!
			\param Coordinate ASCII ID of the character
			\return Floating point UV Coordinate
			*/
			virtual NGin::Math::Vector2 GetCoord(int Character) = 0;

			//! Get texture UV of lower right corner of a character
			/*!
			\param Coordinate ASCII ID of the character
			\return Floating point UV Coordinate
			*/
			virtual NGin::Math::Vector2 GetSize(int Character) = 0;

			//! Get size of a character in relation to the screen resolution
			/*!
			\param Coordinate ASCII ID of the character
			\return Floating point size
			*/
			virtual NGin::Math::Vector2 GetScreenCoord(int Character) = 0;

			//! Request that the font insert its texture into the rendering pipeline
			/*!
			This command is called during rendering, before a control will draw an image.
			SetTexture will replace any currently set skin, meaning two draw calls are
			required to render both text and image. Please see advanced documentation to
			obtain more information about this command. TODO: Advanced Documentation.
			*/
			virtual void SetTexture() = 0;

			//! Return the path of the font file
			/*!
			\return Name of font as std::string
			*/
			virtual std::string GetName() = 0;
		};
	}
}