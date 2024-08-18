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

#include "NComboBox.h"

namespace NGUINet
{
	void NComboBox_SelectedIndexChanged(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NComboBox^>(Control) != nullptr)
				dynamic_cast<NComboBox^>(Control)->__SelectedIndexChanged();
	}

	void NComboBox::__SelectedIndexChanged()
	{
		if(&NComboBox::SelectedIndexChanged != nullptr)
			SelectedIndexChanged(this, System::EventArgs::Empty);
	}

	NComboBox::NComboBox(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::IComboBox*)_Handle)->SelectedIndexChanged()->AddEvent(&NComboBox_SelectedIndexChanged);
	}

	int NComboBox::AddItem(System::String^ Value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(Value);
		NGin::WString cValue(pinnedValue);

		return ((NGin::GUI::IComboBox*)_Handle)->AddItem(cValue.AsCString().c_str());
	}

	void NComboBox::DeleteItem(int Index)
	{
		return ((NGin::GUI::IComboBox*)_Handle)->DeleteItem(Index);
	}

	int NComboBox::ItemCount()
	{
		return ((NGin::GUI::IComboBox*)_Handle)->ItemCount();
	}

	int NComboBox::SelectedIndex::get()
	{
		return ((NGin::GUI::IComboBox*)_Handle)->SelectedIndex;
	}

	void NComboBox::SelectedIndex::set(int value)
	{
		((NGin::GUI::IComboBox*)_Handle)->SelectedIndex = value;
	}

	System::String^ NComboBox::SelectedValue::get()
	{
		return gcnew System::String((const char*)(((NGin::GUI::IComboBox*)_Handle)->SelectedValue.c_str()));
	}

	void NComboBox::SelectedValue::set(System::String^ value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(value);
		NGin::WString cValue(pinnedValue);

		((NGin::GUI::IComboBox*)_Handle)->SelectedValue = cValue.AsCString().c_str();
	}

	System::String^ NComboBox::ItemValue::get(int index)
	{
		return gcnew System::String((const char*)(((NGin::GUI::IComboBox*)_Handle)->ItemValue[index].c_str()));
	}

	void NComboBox::ItemValue::set(int index, System::String^ value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(value);
		NGin::WString cValue(pinnedValue);

		((NGin::GUI::IComboBox*)_Handle)->ItemValue[index] = cValue.AsCString().c_str();
	}
}