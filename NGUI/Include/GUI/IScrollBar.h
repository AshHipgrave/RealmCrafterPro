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
		//! ScrollBar orientation enumeration
		enum ScrollOrientation
		{
			//! ScrollBar scrolls vertically
			VerticalScroll = 0,

			//! ScrollBar scrolls horizontally
			HorizontalScroll = 1
		};

		//! Event arguments for scrolling events
		class ScrollEventArgs : public EventArgs
		{
			int _NewValue;
			int _OldValue;
			ScrollOrientation _ScrollOrientation;

		public:

			ScrollEventArgs(int NewValue, int OldValue, ScrollOrientation ScrollOrientation)
			{
				_NewValue = NewValue;
				_OldValue = OldValue;
				_ScrollOrientation = ScrollOrientation;
			}

			//! Get the new value of the scroll event
			int NewValue() { return _NewValue; }

			//! Get the value before the event occured
			int OldValue() { return _OldValue; }

			//! Get the orientation of the scrollbar
			ScrollOrientation ScrollOrientation() { return _ScrollOrientation; }
		};

		//! Type definition of ScrollEventHandler for scroll events
		typedef NGin::IEventHandler<IControl, ScrollEventArgs> ScrollEventHandler;

		//! ScrollBar Interface class
		/*!
		TODO: Link
		*/
		class IScrollBar : public IControl
		{
		public:

			IScrollBar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IScrollBar() {}

			//! ScrollBar Large change
			__declspec(property(get=zzGet_LargeChange, put=zzPut_LargeChange)) int LargeChange;
			virtual void zzPut_LargeChange(int LargeChange) = 0;
			virtual int zzGet_LargeChange() = 0;

			//! ScrollBar Small change
			__declspec(property(get=zzGet_SmallChange, put=zzPut_SmallChange)) int SmallChange;
			virtual void zzPut_SmallChange(int SmallChange) = 0;
			virtual int zzGet_SmallChange() = 0;

			//! ScrollBar Minimum value
			__declspec(property(get=zzGet_Minimum, put=zzPut_Minimum)) int Minimum;
			virtual void zzPut_Minimum(int Minimum) = 0;
			virtual int zzGet_Minimum() = 0;

			//! ScrollBar Maximum value
			__declspec(property(get=zzGet_Maximum, put=zzPut_Maximum)) int Maximum;
			virtual void zzPut_Maximum(int Maximum) = 0;
			virtual int zzGet_Maximum() = 0;

			//! ScrollBar Value
			__declspec(property(get=zzGet_Value, put=zzPut_Value)) int Value;
			virtual void zzPut_Value(int Value) = 0;
			virtual int zzGet_Value() = 0;

			//! EventHandler for  event
			/*!
			Scroll event is called when a user changes the scrollbar value
			*/
			virtual ScrollEventHandler* Scroll() = 0;

			//! Returns the IScrollBar Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};
	}
}