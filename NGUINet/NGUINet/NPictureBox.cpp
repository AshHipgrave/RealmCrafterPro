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

#include "NPictureBox.h"

namespace NGUINet
{
	NPictureBox::NPictureBox(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
	}

	bool NPictureBox::SetImage(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IPictureBox*)_Handle)->SetImage(cPath.AsCString().c_str());
	}

	bool NPictureBox::SetImage(System::String^ Path, System::Drawing::Color^ Mask)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IPictureBox*)_Handle)->SetImage(cPath.AsCString().c_str(), NGUINet::NVectorConverter::ToColor(Mask).ToARGB());
	}

	NGUINet::NVector2^ NPictureBox::MaxTexCoord::get()
	{
		return NGUINet::NVectorConverter::FromVector2(((NGin::GUI::IPictureBox*)_Handle)->MaxTexCoord);
	}

	void NPictureBox::MaxTexCoord::set(NGUINet::NVector2^ In)
	{
		((NGin::GUI::IPictureBox*)_Handle)->MaxTexCoord = NGUINet::NVectorConverter::ToVector2(In);
	}

	NGUINet::NVector2^ NPictureBox::MinTexCoord::get()
	{
		return NGUINet::NVectorConverter::FromVector2(((NGin::GUI::IPictureBox*)_Handle)->MinTexCoord);
	}

	void NPictureBox::MinTexCoord::set(NGUINet::NVector2^ In)
	{
		((NGin::GUI::IPictureBox*)_Handle)->MinTexCoord = NGUINet::NVectorConverter::ToVector2(In);
	}

}