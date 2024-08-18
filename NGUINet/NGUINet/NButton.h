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

#include "NGUINet.h"
#include "NControl.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace NGUINet
{

	public ref class NButton : NControl
	{
	protected:


	public:
		NButton(IntPtr Handle, NGUINet::NGUIManager^ Manager);

		property NTextAlign Align
		{
			NTextAlign get();
			void set(NTextAlign);
		}

		property NTextAlign VAlign
		{
			NTextAlign get();
			void set(NTextAlign);
		}

		property bool UseBorder
		{
			bool get();
			void set(bool);
		}

		bool SetUpImage(System::String^ Path);
		bool SetHoverImage(System::String^ Path);
		bool SetDownImage(System::String^ Path);

		bool SetUpImage(System::String^ Path, System::Drawing::Color^ Mask);
		bool SetHoverImage(System::String^ Path, System::Drawing::Color^ Mask);
		bool SetDownImage(System::String^ Path, System::Drawing::Color^ Mask);

		event System::EventHandler^ Click;
		event System::EventHandler^ RightClick;
		event System::EventHandler^ MouseDown;
		event System::EventHandler^ MouseEnter;
		event System::EventHandler^ MouseLeave;
		event System::Windows::Forms::MouseEventHandler^ MouseMove;
		void __Click();
		void __RightClick();
		void __MouseDown();
		void __MouseEnter();
		void __MouseLeave();
		void __MouseMove(float x, float y);
	};
}
