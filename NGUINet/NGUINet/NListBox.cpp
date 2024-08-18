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

#include "NListBox.h"

namespace NGUINet
{
	void NListBox_SelectedIndexChanged(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NListBox^>(Control) != nullptr)
				dynamic_cast<NListBox^>(Control)->__SelectedIndexChanged();
	}

	void NListBox::__SelectedIndexChanged()
	{
		if(&NListBox::SelectedIndexChanged != nullptr)
			SelectedIndexChanged(this, System::EventArgs::Empty);
	}

	NListBox::NListBox(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::IListBox*)_Handle)->SelectedIndexChanged()->AddEvent(&NListBox_SelectedIndexChanged);
	}

	int NListBox::AddItem(System::String^ Value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(Value);
		NGin::WString cValue(pinnedValue);

		return ((NGin::GUI::IListBox*)_Handle)->AddItem(cValue.AsCString().c_str());
	}

	void NListBox::DeleteItem(int Index)
	{
		return ((NGin::GUI::IListBox*)_Handle)->DeleteItem(Index);
	}

	int NListBox::ItemCount()
	{
		return ((NGin::GUI::IListBox*)_Handle)->ItemCount();
	}

	int NListBox::SelectedIndex::get()
	{
		return ((NGin::GUI::IListBox*)_Handle)->SelectedIndex;
	}

	void NListBox::SelectedIndex::set(int value)
	{
		((NGin::GUI::IListBox*)_Handle)->SelectedIndex = value;
	}

	System::String^ NListBox::SelectedValue::get()
	{
		return gcnew System::String((const char*)(((NGin::GUI::IListBox*)_Handle)->SelectedValue.c_str()));
	}

	void NListBox::SelectedValue::set(System::String^ value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(value);
		NGin::WString cValue(pinnedValue);

		((NGin::GUI::IListBox*)_Handle)->SelectedValue = cValue.AsCString().c_str();
	}

	System::String^ NListBox::ItemValue::get(int index)
	{
		return gcnew System::String((const char*)(((NGin::GUI::IListBox*)_Handle)->ItemValue[index].c_str()));
	}

	void NListBox::ItemValue::set(int index, System::String^ value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(value);
		NGin::WString cValue(pinnedValue);

		((NGin::GUI::IListBox*)_Handle)->ItemValue[index] = cValue.AsCString().c_str();
	}

	System::Drawing::Color^ NListBox::SelectionForeColor::get()
	{
		return NGUINet::NVectorConverter::FromColor(((NGin::GUI::IListBox*)_Handle)->SelectionForeColor);
	}

	void NListBox::SelectionForeColor::set(System::Drawing::Color^ value)
	{
		((NGin::GUI::IListBox*)_Handle)->SelectionForeColor = NGUINet::NVectorConverter::ToColor(value);
	}

	System::Drawing::Color^ NListBox::SelectionBackColor::get()
	{
		return NGUINet::NVectorConverter::FromColor(((NGin::GUI::IListBox*)_Handle)->SelectionBackColor);
	}

	void NListBox::SelectionBackColor::set(System::Drawing::Color^ value)
	{
		((NGin::GUI::IListBox*)_Handle)->SelectionBackColor = NGUINet::NVectorConverter::ToColor(value);
	}
}