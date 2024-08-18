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
#include "stdafx.h"

#include "NVectorConverter.h"

namespace NGUINet
{
	NGin::Math::Vector3 NVectorConverter::ToVector3(NGUINet::NVector3^ In)
	{
		return NGin::Math::Vector3(In->X, In->Y, In->Z);
	}

	NGUINet::NVector3^ NVectorConverter::FromVector3(NGin::Math::Vector3 In)
	{
		return gcnew NGUINet::NVector3(In.X, In.Y, In.Z);
	}

	NGin::Math::Vector2 NVectorConverter::ToVector2(NGUINet::NVector2^ In)
	{
		return NGin::Math::Vector2(In->X, In->Y);
	}

	NGUINet::NVector2^ NVectorConverter::FromVector2(NGin::Math::Vector2 In)
	{
		return gcnew NGUINet::NVector2(In.X, In.Y);
	}

	NGin::Math::Color NVectorConverter::ToColor(System::Drawing::Color^ In)
	{
		NGin::Math::Color cColor;
		cColor.R = Convert::ToSingle(In->R) / 255.0f;
		cColor.G = Convert::ToSingle(In->G) / 255.0f;
		cColor.B = Convert::ToSingle(In->B) / 255.0f;
		cColor.A = Convert::ToSingle(In->A) / 255.0f;

		return cColor;
	}

	System::Drawing::Color^ NVectorConverter::FromColor(NGin::Math::Color In)
	{
		return System::Drawing::Color::FromArgb(In.ToARGB());
	}
}