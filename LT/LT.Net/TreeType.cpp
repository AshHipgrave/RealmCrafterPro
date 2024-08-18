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
#include "Stdafx.h"
#include "TreeType.h"

namespace LTNet
{
	TreeType::TreeType(ITreeType* handle)
		: _TreeType(handle)
	{
	}

	System::String^ TreeType::Name::get()
	{
		return gcnew System::String(_TreeType->GetName());
	}

	ITreeType* TreeType::GetHandle()
	{
		return _TreeType;
	}

	bool TreeType::operator==(TreeType^ a, TreeType^ b)
	{
		bool anull = false;
		bool bnull = false;

		try
		{
			System::String^ N = a->Name;
		}catch(NullReferenceException^ e)
		{
			(void)e;
			anull = true;
		}

		try
		{
			System::String^ N = b->Name;
		}catch(NullReferenceException^ e)
		{
			(void)e;
			bnull = true;
		}
		
		if(anull && bnull)
			return true;

		if(anull || bnull)
			return false;
		
		return a->_TreeType == b->_TreeType;
	}

	bool TreeType::operator!=(TreeType^ a, TreeType^ b)
	{
		return !(a == b);
	}
}