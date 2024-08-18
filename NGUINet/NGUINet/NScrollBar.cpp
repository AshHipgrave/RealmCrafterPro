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

#include "NScrollBar.h"

namespace NGUINet
{
	void NScrollBar_Scroll(NGin::GUI::IControl* Sender, NGin::GUI::ScrollEventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		System::Windows::Forms::ScrollEventArgs^ Ev = gcnew System::Windows::Forms::ScrollEventArgs(
			System::Windows::Forms::ScrollEventType::ThumbTrack, E->OldValue(), E->NewValue());

		if(Control != nullptr)
			if(dynamic_cast<NScrollBar^>(Control) != nullptr)
				dynamic_cast<NScrollBar^>(Control)->__Scroll(Ev);
	}

	void NScrollBar::__Scroll(System::Windows::Forms::ScrollEventArgs^ Ev)
	{
		if(&NScrollBar::Scroll != nullptr)
			Scroll(this, Ev);
	}

	NScrollBar::NScrollBar(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::IScrollBar*)_Handle)->Scroll()->AddEvent(&NScrollBar_Scroll);
	}

	int NScrollBar::LargeChange::get()
	{
		return ((NGin::GUI::IScrollBar*)_Handle)->LargeChange;
	}

	void NScrollBar::LargeChange::set(int value)
	{
		((NGin::GUI::IScrollBar*)_Handle)->LargeChange = value;
	}

	int NScrollBar::SmallChange::get()
	{
		return ((NGin::GUI::IScrollBar*)_Handle)->SmallChange;
	}

	void NScrollBar::SmallChange::set(int value)
	{
		((NGin::GUI::IScrollBar*)_Handle)->SmallChange = value;
	}

	int NScrollBar::Minimum::get()
	{
		return ((NGin::GUI::IScrollBar*)_Handle)->Minimum;
	}

	void NScrollBar::Minimum::set(int value)
	{
		((NGin::GUI::IScrollBar*)_Handle)->Minimum = value;
	}

	int NScrollBar::Maximum::get()
	{
		return ((NGin::GUI::IScrollBar*)_Handle)->Maximum;
	}

	void NScrollBar::Maximum::set(int value)
	{
		((NGin::GUI::IScrollBar*)_Handle)->Maximum = value;
	}

	int NScrollBar::Value::get()
	{
		return ((NGin::GUI::IScrollBar*)_Handle)->Value;
	}

	void NScrollBar::Value::set(int value)
	{
		((NGin::GUI::IScrollBar*)_Handle)->Value = value;
	}
}