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

#include <IPictureBox.h>
#include "CGUIManager.h"

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CPictureBox : public IPictureBox
		{
		protected:

			CGUIManager* Manager;
			IGUIMeshBuffer* MeshBuffer;
			IGUITexture* Texture;
			Math::Vector2 MinTex, MaxTex;
			bool MouseCursor;
			Math::Vector4 ScissorRegion;
			bool UseScissor;

			void RebuildMesh();

			virtual void OnSizeChange();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset();
			virtual void OnBackColorChange();
			virtual bool Update(GUIUpdateParameters* Parameters);
			virtual void Render();

			virtual bool SetImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask);

		public:

			CPictureBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager);
			virtual ~CPictureBox();
			virtual bool Initialize(CGUIManager* Manager);

			virtual bool SetImage(std::string ImageFile);
			virtual bool SetImage(std::string ImageFile, unsigned int Mask);
			virtual void SetImage(IGUITexture* texture);
			virtual void SetImage(void* Texture);

			virtual Math::Vector2 zzGet_MinTexCoord();
			virtual Math::Vector2 zzGet_MaxTexCoord();

			virtual void zzPut_MinTexCoord(Math::Vector2 Coord);
			virtual void zzPut_MaxTexCoord(Math::Vector2 Coord);

			void zzPut_IsMouseCursor(bool isMouseCursor);
			bool zzGet_IsMouseCursor();

			virtual void zzPut_ForceScissoring(bool forceScissoring);
			virtual bool zzGet_ForceScissoring();

			virtual void zzPut_ScissorWindow(Math::Vector4 &scissorWindow);
			virtual Math::Vector4 zzGet_ScissorWindow();

		};
	}
}