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

#include "NVector3.h"

namespace NGUINet
{
	NVector3::NVector3()
	{
		_X = 0.0f;
		_Y = 0.0f;
		_Z = 0.0f;
	}

	NVector3::NVector3(float x, float y, float z)
	{
		_X = x;
		_Y = y;
		_Z = z;
	}

	float NVector3::X::get()
	{
		return _X;
	}

	void NVector3::X::set(float value)
	{
		_X = value;
	}

	float NVector3::Y::get()
	{
		return _Y;
	}

	void NVector3::Y::set(float value)
	{
		_Y = value;
	}

	float NVector3::Z::get()
	{
		return _Z;
	}

	void NVector3::Z::set(float value)
	{
		_Z = value;
	}
}