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
#include "IFont.h"

namespace NGin
{
	namespace GUI
	{
		//! Text alignment enumeration
		enum TextAlign
		{
			//! Horizontal left align
			TextAlign_Left = 0,

			//! Horizontal center align
			TextAlign_Center = 1,

			//! Horizontal right align
			TextAlign_Right = 2,

			//! Vertical top align
			TextAlign_Top = 0,

			//! Vertical middle align
			TextAlign_Middle = 1,

			//! Vertical bottom align
			TextAlign_Bottom = 2
		};

		class IFont;

		//! Label Interface class
		/*!
		TODO: Link
		*/
		class ILabel : public IControl
		{
		public:

			ILabel(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~ILabel() {}

			//! Control font
			__declspec(property(get=zzGet_Font, put=zzPut_Font)) IFont* Font;
			virtual void zzPut_Font(IFont* Font) = 0;
			virtual IFont* zzGet_Font() = 0;

			//! Horizontal text alignment
			__declspec(property(get=zzGet_Align, put=zzPut_Align)) TextAlign Align;
			virtual void zzPut_Align(TextAlign Alignment) = 0;
			virtual TextAlign zzGet_Align() = 0;

			//! Vertical text alignment
			__declspec(property(get=zzGet_VAlign, put=zzPut_VAlign)) TextAlign VAlign;
			virtual void zzPut_VAlign(TextAlign Alignment) = 0;
			virtual TextAlign zzGet_VAlign() = 0;

			//! Multiline support
			/*! Setting multiline to true will make the label
			wrap text based upon its X Size. It will also allow
			inline WString processing to use the \n character.*/
			__declspec(property(get=zzGet_Multiline, put=zzPut_Multiline)) bool Multiline;
			virtual void zzPut_Multiline(bool Multiline) = 0;
			virtual bool zzGet_Multiline() = 0;

			//! Inline WString processing support
			/*! Setting this to true will allow the label to
			process escaped data. This includes:
			\n - New line (Multiline must be turned on for this)
			\c - Color support. Use \c=#000000 or \c=Black. See Label Color documentation.*/
			__declspec(property(get=zzGet_InlineStringProcessing, put=zzPut_InlineStringProcessing)) bool InlineStringProcessing;
			virtual void zzPut_InlineStringProcessing(bool InlineStringProcessing) = 0;
			virtual bool zzGet_InlineStringProcessing() = 0;

			//! Disable/Enable auto resizing
			/*! Labels will resize by default (On the X axis, or Y if multiline)
			disable this to always retain a size.*/
			__declspec(property(get=zzGet_ScissorWindow, put=zzPut_ScissorWindow)) NGin::Math::Vector2 ScissorWindow;
			virtual void zzPut_ScissorWindow(NGin::Math::Vector2 ScissorWindow) = 0;
			virtual NGin::Math::Vector2 zzGet_ScissorWindow() = 0;

			//! Force Scissoring
			/*! Enabling scissoring on this object will prevent text from
			spilling over its set bounds.
			<b>Note:</b> This will give strange results if AutoResize is false.*/
			__declspec(property(get=zzGet_ForceScissoring, put=zzPut_ForceScissoring)) bool ForceScissoring;
			virtual void zzPut_ForceScissoring(bool ForceScissoring) = 0;
			virtual bool zzGet_ForceScissoring() = 0;

			//! Internal scrolled value of the text to offset rendering
			/*! Setting an offset will modify the text position upon rendering.
			This should be used in conjunction with ForceScissoring to provide
			a scrolling window effect on a text block.*/
			__declspec(property(get=zzGet_ScrollOffset, put=zzPut_ScrollOffset)) NGin::Math::Vector2 ScrollOffset;
			virtual void zzPut_ScrollOffset(NGin::Math::Vector2 &Offset) = 0;
			virtual NGin::Math::Vector2 zzGet_ScrollOffset() = 0;
			
			//! Get the height of the contained text
			__declspec(property(get=zzGet_InternalHeight)) float InternalHeight;
			virtual float zzGet_InternalHeight() = 0;

			//! Get the width of the contained text, incase it has slipped its bounds.
			__declspec(property(get=zzGet_InternalWidth)) float InternalWidth;
			virtual float zzGet_InternalWidth() = 0;

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

			//! Returns the ILabel Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
};
	}
}