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
#include "Blitzplus.h"
#include <math.h>

namespace BlitzPlus
{

	#define BBDX_PI 3.14159265

	float Floor(float Val)
	{
		return floor(Val);
	}

	float Ceil(float Val)
	{
		return ceil(Val);
	}

	// Returns ROOT!!!
	float Sqr(float Val)
	{
		return sqrt(Val);
	}

	int Abs(int Val)
	{
		return (int)fabs((float)Val);
	}

	float Abs(float Val)
	{
		return fabs(Val);
	}

	float Sgn(float Val)
	{
		if(Val > 0.0f)
			return 1.0f;
		else if(Val < 0.0f)
			return -1.0f;
		return 0.0f;
	}

	int Sgn(int Val)
	{
		if(Val > 0)
			return 1;
		else if(Val < 0)
			return -1;
		return 0;
	}

	float Sin(float Angle)
	{
		double A = (double)Angle;
		return (float)sin(A*(BBDX_PI/180.0));
	}

	float Cos(float Angle)
	{
		double A = (double)Angle;
		return (float)cos(A*(BBDX_PI/180.0));
	}


	float Pow(float Num, float Exp)
	{
		return pow(Num, Exp);
	}

	float Pow(float Num, int Exp)
	{
		return pow(Num, (float)Exp);
	}

	int Pow(int Num, int Exp)
	{
		return (int)pow((float)Num, (float)Exp);
	}
}