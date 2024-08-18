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

#include "NTrackBar.h"

namespace NGUINet
{
	void NTrackBar_ValueChanged(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NTrackBar^>(Control) != nullptr)
				dynamic_cast<NTrackBar^>(Control)->__ValueChanged();
	}

	void NTrackBar::__ValueChanged()
	{
		if(&NTrackBar::ValueChanged != nullptr)
			ValueChanged(this, System::EventArgs::Empty);
	}

	NTrackBar::NTrackBar(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::ITrackBar*)_Handle)->ValueChanged()->AddEvent(&NTrackBar_ValueChanged);
	}

	int NTrackBar::TickFrequency::get()
	{
		return ((NGin::GUI::ITrackBar*)_Handle)->TickFrequency;
	}

	void NTrackBar::TickFrequency::set(int value)
	{
		((NGin::GUI::ITrackBar*)_Handle)->TickFrequency = value;
	}

	int NTrackBar::Minimum::get()
	{
		return ((NGin::GUI::ITrackBar*)_Handle)->Minimum;
	}

	void NTrackBar::Minimum::set(int value)
	{
		((NGin::GUI::ITrackBar*)_Handle)->Minimum = value;
	}

	int NTrackBar::Maximum::get()
	{
		return ((NGin::GUI::ITrackBar*)_Handle)->Maximum;
	}

	void NTrackBar::Maximum::set(int value)
	{
		((NGin::GUI::ITrackBar*)_Handle)->Maximum = value;
	}

	int NTrackBar::Value::get()
	{
		return ((NGin::GUI::ITrackBar*)_Handle)->Value;
	}

	void NTrackBar::Value::set(int value)
	{
		((NGin::GUI::ITrackBar*)_Handle)->Value = value;
	}
}