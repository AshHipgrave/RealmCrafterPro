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

#include "NControl.h"

namespace NGUINet
{
	NControl::NControl(IntPtr Handle, NGUINet::NGUIManager^ Manager)
	{
		_Handle = (NGin::GUI::IControl*)(void*)Handle;
		_Manager = Manager;
		_Manager->Register(this);
		_Tag = nullptr;
	}

	NControl::NControl()
	{
		_Handle = 0;
		_Tag = nullptr;
		_Manager = nullptr;
	}

	IntPtr NControl::Handle::get()
	{
		return (IntPtr)_Handle;
	}

	void NControl::BringToFront()
	{
		_Handle->BringToFront();
	}

	NGUINet::NVector2^ NControl::Location::get()
	{
		return NGUINet::NVectorConverter::FromVector2(_Handle->Location);
	}

	void NControl::Location::set(NGUINet::NVector2^ In)
	{
		_Handle->Location = NGUINet::NVectorConverter::ToVector2(In);
	}

	NGUINet::NVector2^ NControl::Size::get()
	{
		return NGUINet::NVectorConverter::FromVector2(_Handle->Size);
	}

	void NControl::Size::set(NGUINet::NVector2^ In)
	{
		_Handle->Size = NGUINet::NVectorConverter::ToVector2(In);
	}

	NGUINet::NVector2^ NControl::MaximumSize::get()
	{
		return NGUINet::NVectorConverter::FromVector2(_Handle->MaximumSize);
	}

	void NControl::MaximumSize::set(NGUINet::NVector2^ In)
	{
		_Handle->MaximumSize = NGUINet::NVectorConverter::ToVector2(In);
	}

	NGUINet::NVector2^ NControl::MinimumSize::get()
	{
		return NGUINet::NVectorConverter::FromVector2(_Handle->MinimumSize);
	}

	void NControl::MinimumSize::set(NGUINet::NVector2^ In)
	{
		_Handle->MinimumSize = NGUINet::NVectorConverter::ToVector2(In);
	}

	System::String^ NControl::Name::get()
	{
		return gcnew System::String((const char*)_Handle->Name.c_str());
	}

	void NControl::Name::set(System::String^ value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(value);
		NGin::WString cValue(pinnedValue);

		_Handle->Name = cValue.AsCString().c_str();
	}

	System::String^ NControl::Text::get()
	{
		return gcnew System::String((const char*)_Handle->Text.c_str());
	}

	void NControl::Text::set(System::String^ value)
	{
		pin_ptr<const wchar_t> pinnedValue = PtrToStringChars(value);
		NGin::WString cValue(pinnedValue);

		_Handle->Text = cValue.AsCString().c_str();
	}

	bool NControl::Enabled::get()
	{
		return _Handle->Enabled;
	}

	void NControl::Enabled::set(bool value)
	{
		_Handle->Enabled = value;
	}

	bool NControl::Visible::get()
	{
		return _Handle->Visible;
	}

	void NControl::Visible::set(bool value)
	{
		_Handle->Visible = value;
	}

	System::Drawing::Color^ NControl::BackColor::get()
	{
		return NVectorConverter::FromColor(_Handle->BackColor);
	}

	void NControl::BackColor::set(System::Drawing::Color^ value)
	{
		_Handle->BackColor = NVectorConverter::ToColor(value);
	}

	System::Drawing::Color^ NControl::ForeColor::get()
	{
		return NVectorConverter::FromColor(_Handle->ForeColor);
	}

	void NControl::ForeColor::set(System::Drawing::Color^ value)
	{
		_Handle->ForeColor = NVectorConverter::ToColor(value);
	}

	int NControl::Skin::get()
	{
		return _Handle->Skin;
	}

	void NControl::Skin::set(int value)
	{
		_Handle->Skin = value;
	}

	bool NControl::Locked::get()
	{
		return _Handle->Locked;
	}

	void NControl::Locked::set(bool value)
	{
		_Handle->Locked = value;
	}

	System::Object^ NControl::Tag::get()
	{
		return _Tag;
	}
	
	void NControl::Tag::set(System::Object^ value)
	{
		_Tag = value;
	}

	NGUINet::NControl^ NControl::Parent::get()
	{
		array<NGUINet::NControl^>^ Ctrls = _Manager->Controls()->ToArray();
		NGin::GUI::IControl* cCtrl = _Handle->Parent;

		for(int i = 0; i < Ctrls->Length; ++i)
			if((NGin::GUI::IControl*)(void*)Ctrls[i]->Handle == cCtrl)
				return Ctrls[i];

		return nullptr;
	}
	
	void NControl::Parent::set(NGUINet::NControl^ value)
	{
		_Handle->Parent = (NGin::GUI::IControl*)(void*)value->Handle;
	}

}