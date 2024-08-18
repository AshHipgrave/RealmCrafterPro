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

class ILabel;

#include <ILabel.h>
#include "CGUIManager.h"

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CLabel : public ILabel
		{
		protected:

			CGUIManager* Manager;
			IGUIMeshBuffer* MeshBuffer;
			IFont* _Font;
			TextAlign _Align, _VAlign;
			bool _Multiline, _InlineStringProcessing;
			float _InternalHeight, _InternalWidth;

			int ButtonState;
			bool UsingRight;
			EventHandler* ClickEvent;
			EventHandler* RightClickEvent;
			EventHandler* DownEvent;
			MouseEventHandler* MouseMoveEvent;
			EventHandler* MouseLeaveEvent;
			EventHandler* MouseEnterEvent;
			bool _MouseOver;
			NGin::Math::Vector2 LastMouse;

			NGin::Math::Vector2 _ScrollOffset;
			NGin::Math::Vector2 _ScissorWindow;
			bool _ForceScissoring;

			void RebuildMesh();

			virtual void OnTextChange();
			virtual void OnForeColorChange();
			virtual void OnSkinChange();
			virtual void OnSizeChange();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset();
			virtual bool Update(GUIUpdateParameters* Parameters);
			virtual void Render();

		public:

			CLabel(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager);
			virtual ~CLabel();
			virtual bool Initialize(CGUIManager* Manager);

			virtual IFont* zzGet_Font();
			virtual void zzPut_Font(IFont* Font);
			
			virtual TextAlign zzGet_Align();
			virtual TextAlign zzGet_VAlign();
			virtual void zzPut_Align(TextAlign Alignment);
			virtual void zzPut_VAlign(TextAlign Alignment);
			
			virtual bool zzGet_Multiline();
			virtual bool zzGet_InlineStringProcessing();
			virtual void zzPut_Multiline(bool Multiline);
			virtual void zzPut_InlineStringProcessing(bool InlineStringProcessing);

			virtual EventHandler* Click();
			virtual EventHandler* RightClick();
			virtual EventHandler* MouseDown();

			virtual EventHandler* MouseEnter();
			virtual EventHandler* MouseLeave();
			virtual MouseEventHandler* MouseMove();
			
			virtual void zzPut_ScissorWindow(NGin::Math::Vector2 AutoResize);
			virtual NGin::Math::Vector2 zzGet_ScissorWindow();
			virtual void zzPut_ForceScissoring(bool ForceScissoring);
			virtual bool zzGet_ForceScissoring();
			virtual void zzPut_ScrollOffset(NGin::Math::Vector2 &Offset);
			virtual NGin::Math::Vector2 zzGet_ScrollOffset();
			virtual float zzGet_InternalHeight();
			virtual float zzGet_InternalWidth();
		};
	}
}