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

#include "GrassType.h"
#include "NVectorConverter.h"
#include <nginstring.h>

cli::interior_ptr<const System::Char> PtrToStringChars(const System::String^ s);

namespace RealmCrafter
{
namespace RCT
{
	GrassType::GrassType(IGrassType* type)
	{
		handle = type;
	}

	System::String^ GrassType::ToString()
	{
		return Name;
	}
		
	System::String^ GrassType::Name::get()
	{
		return gcnew System::String(handle->GetName().c_str());
	}

	void GrassType::Name::set(System::String^ name)
	{
		pin_ptr<const wchar_t> wName = PtrToStringChars(name);

		handle->SetName(std::string(WString(wName).AsCString().c_str()));
	}

	System::String^ GrassType::Texture::get()
	{
		return gcnew System::String(handle->GetTexture().c_str());
	}

	void GrassType::Texture::set(System::String^ texture)
	{
		pin_ptr<const wchar_t> wTexture = PtrToStringChars(texture);

		handle->SetTexture(std::string(WString(wTexture).AsCString().c_str()));
	}

	NGUINet::NVector2^ GrassType::Scale::get()
	{
		return NGUINet::NVectorConverter::FromVector2(handle->GetScale());
	}

	void GrassType::Scale::set(NGUINet::NVector2^ scale)
	{
		handle->SetScale(NGUINet::NVectorConverter::ToVector2(scale));
	}

	float GrassType::Offset::get()
	{
		return handle->GetOffset();
	}

	void GrassType::Offset::set(float offset)
	{
		handle->SetOffset(offset);
	}

	float GrassType::Coverage::get()
	{
		return handle->GetCoverage();
	}

	void GrassType::Coverage::set(float coverage)
	{
		handle->SetCoverage(coverage);
	}

	float GrassType::HeightVariance::get()
	{
		return handle->GetHeightVariance();
	}

	void GrassType::HeightVariance::set(float heightVariance)
	{
		handle->SetHeightVariance(heightVariance);
	}

	IGrassType* GrassType::Handle::get()
	{
		return handle;
	}
}
}