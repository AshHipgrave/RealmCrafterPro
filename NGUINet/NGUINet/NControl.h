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

using namespace System;
using namespace System::Runtime::InteropServices;

namespace NGUINet
{
	ref class NGUIManager;

	public enum NTextAlign
	{
		Left = 0,
		Center = 1,
		Right = 2,

		Top = 0,
		Middle = 1,
		Bottom = 2
	};

	public ref class NControl
	{
	protected:
		NGin::GUI::IControl* _Handle;
		NGUINet::NGUIManager^ _Manager;
		System::Object^ _Tag;

	public:
		NControl(IntPtr Handle, NGUINet::NGUIManager^ Manager);
		NControl();

		void BringToFront();

		property IntPtr Handle
		{
			IntPtr get();
		}

		property NGUINet::NVector2^ Location
		{
			NGUINet::NVector2^ get();
			void set(NGUINet::NVector2^);
		}

		property NGUINet::NVector2^ Size
		{
			NGUINet::NVector2^ get();
			void set(NGUINet::NVector2^);
		}

		property NGUINet::NVector2^ MaximumSize
		{
			NGUINet::NVector2^ get();
			void set(NGUINet::NVector2^);
		}

		property NGUINet::NVector2^ MinimumSize
		{
			NGUINet::NVector2^ get();
			void set(NGUINet::NVector2^);
		}

		property System::String^ Name
		{
			System::String^ get();
			void set(System::String^);
		}

		property System::String^ Text
		{
			System::String^ get();
			void set(System::String^);
		}

		property bool Enabled
		{
			bool get();
			void set(bool);
		}

		property bool Visible
		{
			bool get();
			void set(bool);
		}

		property System::Drawing::Color^ BackColor
		{
			System::Drawing::Color^ get();
			void set(System::Drawing::Color^);
		}

		property System::Drawing::Color^ ForeColor
		{
			System::Drawing::Color^ get();
			void set(System::Drawing::Color^);
		}

		property int Skin
		{
			int get();
			void set(int);
		}

		property bool Locked
		{
			bool get();
			void set(bool);
		}

		property System::Object^ Tag
		{
			System::Object^ get();
			void set(System::Object^);
		}

		property NGUINet::NControl^ Parent
		{
			NGUINet::NControl^ get();
			void set(NGUINet::NControl^);
		}
	};
}
