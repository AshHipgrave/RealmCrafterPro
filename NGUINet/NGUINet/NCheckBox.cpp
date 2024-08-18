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

#include "NCheckBox.h"

namespace NGUINet
{
	void NCheckBox_CheckedChange(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NCheckBox^>(Control) != nullptr)
				dynamic_cast<NCheckBox^>(Control)->__CheckedChange();
	}

	void NCheckBox::__CheckedChange()
	{
		if(&NCheckBox::CheckedChange != nullptr)
			CheckedChange(this, System::EventArgs::Empty);
	}

	NCheckBox::NCheckBox(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::ICheckBox*)_Handle)->CheckedChange()->AddEvent(&NCheckBox_CheckedChange);
	}

	bool NCheckBox::Checked::get()
	{
		return ((NGin::GUI::ICheckBox*)_Handle)->Checked;
	}

	void NCheckBox::Checked::set(bool value)
	{
		((NGin::GUI::ICheckBox*)_Handle)->Checked = value;
	}

}