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

namespace irr
{
	namespace scene
	{
		class ISceneManager;
		class ISceneNode;
	}
}

struct SShaderProfile
{
	irr::u32 HighNear, HighFar;
	irr::u32 MediumNear, MediumFar;
	irr::u32 LowNear, LowFar;
	irr::core::stringc Name;
	float RangeScale;

	SShaderProfile(irr::c8* Name)
	{
		Name = Name;
		RangeScale = 1.0f;
		HighNear = HighFar = MediumNear = MediumFar = LowNear = LowFar = 0;
	}

	irr::u32 GetEffect(irr::scene::ISceneNode* Node, irr::scene::ISceneManager* SceneManager);

	irr::u32 GetEffect(int QualityLevel, float Range)
	{
		if(QualityLevel == 2)
			if(Range < RangeScale)
				return HighNear;
			else
				return HighFar;
		else if(QualityLevel == 1)
			if(Range < RangeScale)
				return MediumNear;
			else
				return MediumFar;
		else
			if(Range < RangeScale)
				return LowNear;
			else
				return LowFar;
		
		return LowFar;
	}
};