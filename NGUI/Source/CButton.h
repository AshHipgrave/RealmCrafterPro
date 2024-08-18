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

#include <IButton.h>
#include <ILabel.h>
#include "CGUIManager.h"

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CButton : public IButton
		{
		protected:

			CGUIManager* Manager;
			IGUIMeshBuffer* MeshBuffer;
			int ButtonState;
			bool UsingRight;
			ILabel* Caption;
			EventHandler* ClickEvent;
			EventHandler* RightClickEvent;
			EventHandler* DownEvent;
			MouseEventHandler* MouseMoveEvent;
			EventHandler* MouseLeaveEvent;
			EventHandler* MouseEnterEvent;
			bool _MouseOver;
			NGin::Math::Vector2 LastMouse;

			IGUITexture* UpTexture, *DownTexture, *HoverTexture;
			bool _UseBorder;

			bool _Down;

			void RebuildMesh();

			virtual void OnTransform();
			virtual void OnTextChange();
			virtual void OnEnabledChange();
			virtual void OnBackColorChange();
			virtual void OnForeColorChange();
			virtual void OnSkinChange();
			virtual void OnSizeChange();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset();
			virtual bool Update(GUIUpdateParameters* Parameters);
			virtual void Render();

			virtual bool SetUpImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask);
			virtual bool SetDownImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask);
			virtual bool SetHoverImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask);


		public:

			CButton(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager);
			virtual ~CButton();
			virtual bool Initialize(CGUIManager* Manager);

			virtual TextAlign zzGet_Align();
			virtual TextAlign zzGet_VAlign();
			virtual void zzPut_Align(TextAlign Alignment);
			virtual void zzPut_VAlign(TextAlign Alignment);

			virtual bool SetUpImage(std::string ImageFile);
			virtual bool SetHoverImage(std::string ImageFile);
			virtual bool SetDownImage(std::string ImageFile);

			virtual bool SetUpImage(std::string ImageFile, unsigned int Mask);
			virtual bool SetHoverImage(std::string ImageFile, unsigned int Mask);
			virtual bool SetDownImage(std::string ImageFile, unsigned int Mask);

			virtual void SetUpImage(void* Texture);
			virtual void SetHoverImage(void* Texture);
			virtual void SetDownImage(void* Texture);

			virtual EventHandler* Click();
			virtual EventHandler* RightClick();
			virtual EventHandler* MouseDown();

			virtual EventHandler* MouseEnter();
			virtual EventHandler* MouseLeave();
			virtual MouseEventHandler* MouseMove();

			virtual bool zzGet_Down();
			virtual void zzPut_Down(bool Down);

			virtual void zzPut_UseBorder(bool UseBorder);
			virtual bool zzGet_UseBorder();
		};
	}
}