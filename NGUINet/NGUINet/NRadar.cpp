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

#include "NRadar.h"

namespace NGUINet
{
	NRadar::NRadar(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
	}

	bool NRadar::SetImage( System::String^ Path, System::String^ PathBorder )
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		pin_ptr<const wchar_t> pinnedPathBorder = PtrToStringChars(PathBorder);
		NGin::WString cPath(pinnedPath);
		NGin::WString cPathBorder(pinnedPathBorder);

		return ((NGin::GUI::IRadar*)_Handle)->SetImage(cPath.AsCString().c_str(), cPathBorder.AsCString().c_str());
	}

	NGUINet::NVector2^ NRadar::ViewRadius::get()
	{
		return NGUINet::NVectorConverter::FromVector2(((NGin::GUI::IRadar*)_Handle)->ViewRadius);
	}

	void NRadar::ViewRadius::set(NGUINet::NVector2^ In)
	{
		((NGin::GUI::IRadar*)_Handle)->ViewRadius = NGUINet::NVectorConverter::ToVector2(In);
	}

	NGUINet::NVector2^ NRadar::ImageTop::get()
	{
		return NGUINet::NVectorConverter::FromVector2(((NGin::GUI::IRadar*)_Handle)->RadarImageTop);
	}

	void NRadar::ImageTop::set(NGUINet::NVector2^ In)
	{
		((NGin::GUI::IRadar*)_Handle)->RadarImageTop = NGUINet::NVectorConverter::ToVector2(In);
	}

	NGUINet::NVector2^ NRadar::ImageSize::get()
	{
		return NGUINet::NVectorConverter::FromVector2(((NGin::GUI::IRadar*)_Handle)->RadarImageSize);
	}

	void NRadar::ImageSize::set(NGUINet::NVector2^ In)
	{
		((NGin::GUI::IRadar*)_Handle)->RadarImageSize = NGUINet::NVectorConverter::ToVector2(In);
	}

}