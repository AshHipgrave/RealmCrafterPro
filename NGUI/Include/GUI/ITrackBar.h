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
		//! TrackBar Interface class
		/*!
		TODO: Link
		*/
		class ITrackBar : public IControl
		{
		public:

			ITrackBar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~ITrackBar() {}

			//! Value of the trackbar
			__declspec(property(get=zzGet_Value, put=zzPut_Value)) int Value;
			virtual void zzPut_Value(int Value) = 0;
			virtual int zzGet_Value() = 0;

			//! TrackBar Minimum value
			__declspec(property(get=zzGet_Minimum, put=zzPut_Minimum)) int Minimum;
			virtual void zzPut_Minimum(int Minimum) = 0;
			virtual int zzGet_Minimum() = 0;

			//! TrackBar Maximum value
			__declspec(property(get=zzGet_Maximum, put=zzPut_Maximum)) int Maximum;
			virtual void zzPut_Maximum(int Maximum) = 0;
			virtual int zzGet_Maximum() = 0;

			//! TrackBar Maximum value
			__declspec(property(get=zzGet_TickFrequency, put=zzPut_TickFrequency)) int TickFrequency;
			virtual void zzPut_TickFrequency(int TickFrequency) = 0;
			virtual int zzGet_TickFrequency() = 0;

			//! EventHandler for valuechanged event
			/*!
			ValueChanged event called whenever the users moves the trackbar
			*/
			virtual EventHandler* ValueChanged() = 0;

			//! Returns the ITrackBar Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};
	}
}