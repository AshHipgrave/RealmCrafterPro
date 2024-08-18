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

// Minimim tolerance of 32-bit floating numbers;
// used in comparison tests to assist with innacuracies
#define NGIN_FP_TOLERANCE 0.0001f

#include "TypeDefs.h"
#include <string>

namespace NGin
{
	//class IEngine;
	//inline bool FloatEqual(float Float1, float Float2);
	void GlobalFail(std::string Message);

	//! 64-bit representation of PI
	extern double PI64;

	//! 64-bit multiplier for Degrees to Radians conversion
	extern double DEGTORAD64;

	//! 64-bit multiplier for Radians to Degrees conversion
	extern double RADTODEG64;

	//! 32-bit representation of PI
	extern float PI;

	//! 32-bit multiplier for Degrees to Radians
	extern float DEGTORAD;

	//! 32-bit multiplier for Radians to Degrees
	extern float RADTODEG;
}