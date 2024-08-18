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

#include <Vector2.h>
#include <Color.h>
#include "NVector2.h"
#include "NVector3.h"
#include <Vector3.h>

using namespace System;

namespace NGUINet
{
	class NVectorConverter
	{
	public:
		static NGin::Math::Vector3 ToVector3(NGUINet::NVector3^ In);
		static NGUINet::NVector3^ FromVector3(NGin::Math::Vector3 In);

		static NGin::Math::Vector2 ToVector2(NGUINet::NVector2^ In);
		static NGUINet::NVector2^ FromVector2(NGin::Math::Vector2 In);

		static NGin::Math::Color ToColor(System::Drawing::Color^ In);
		static System::Drawing::Color^ FromColor(NGin::Math::Color In);
	};
}
