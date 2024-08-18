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

#include "NButton.h"

namespace NGUINet
{
	void NButton_Click(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NButton^>(Control) != nullptr)
				dynamic_cast<NButton^>(Control)->__Click();
	}

	void NButton_RightClick(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NButton^>(Control) != nullptr)
				dynamic_cast<NButton^>(Control)->__RightClick();
	}

	void NButton_MouseDown(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NButton^>(Control) != nullptr)
				dynamic_cast<NButton^>(Control)->__MouseDown();
	}

	void NButton_MouseEnter(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NButton^>(Control) != nullptr)
				dynamic_cast<NButton^>(Control)->__MouseEnter();
	}

	void NButton_MouseLeave(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NButton^>(Control) != nullptr)
				dynamic_cast<NButton^>(Control)->__MouseLeave();
	}

	void NButton_MouseMove(NGin::GUI::IControl* Sender, NGin::GUI::MouseEventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NButton^>(Control) != nullptr)
				dynamic_cast<NButton^>(Control)->__MouseMove(E->Location().X, E->Location().Y);
	}



	void NButton::__Click()
	{
		if(&NButton::Click != nullptr)
			Click(this, System::EventArgs::Empty);
	}

	void NButton::__RightClick()
	{
		if(&NButton::RightClick != nullptr)
			RightClick(this, System::EventArgs::Empty);
	}

	void NButton::__MouseDown()
	{
		if(&NButton::MouseDown != nullptr)
			MouseDown(this, System::EventArgs::Empty);
	}

	void NButton::__MouseEnter()
	{
		if(&NButton::MouseEnter != nullptr)
			MouseEnter(this, System::EventArgs::Empty);
	}


	void NButton::__MouseLeave()
	{
		if(&NButton::MouseLeave != nullptr)
			MouseLeave(this, System::EventArgs::Empty);
	}

	void NButton::__MouseMove(float x, float y)
	{
		System::Windows::Forms::MouseEventArgs^ Mev = gcnew System::Windows::Forms::MouseEventArgs(
			System::Windows::Forms::MouseButtons::None,
			0,
			Convert::ToInt32(x),
			Convert::ToInt32(y),
			0);

		if(&NButton::MouseMove != nullptr)
			MouseMove(this, Mev);
	}

	NButton::NButton(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::IButton*)_Handle)->Click()->AddEvent(&NButton_Click);
		((NGin::GUI::IButton*)_Handle)->RightClick()->AddEvent(&NButton_RightClick);
		((NGin::GUI::IButton*)_Handle)->MouseDown()->AddEvent(&NButton_MouseDown);
		((NGin::GUI::IButton*)_Handle)->MouseEnter()->AddEvent(&NButton_MouseEnter);
		((NGin::GUI::IButton*)_Handle)->MouseLeave()->AddEvent(&NButton_MouseLeave);
		((NGin::GUI::IButton*)_Handle)->MouseMove()->AddEvent(&NButton_MouseMove);
	}


	bool NButton::SetUpImage(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IButton*)_Handle)->SetUpImage(cPath.AsCString().c_str());
	}

	bool NButton::SetHoverImage(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IButton*)_Handle)->SetHoverImage(cPath.AsCString().c_str());
	}

	bool NButton::SetDownImage(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IButton*)_Handle)->SetDownImage(cPath.AsCString().c_str());
	}


	bool NButton::SetUpImage(System::String^ Path, System::Drawing::Color^ Mask)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IButton*)_Handle)->SetUpImage(cPath.AsCString().c_str(), NGUINet::NVectorConverter::ToColor(Mask).ToARGB());
	}

	bool NButton::SetHoverImage(System::String^ Path, System::Drawing::Color^ Mask)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IButton*)_Handle)->SetHoverImage(cPath.AsCString().c_str(), NGUINet::NVectorConverter::ToColor(Mask).ToARGB());
	}

	bool NButton::SetDownImage(System::String^ Path, System::Drawing::Color^ Mask)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return ((NGin::GUI::IButton*)_Handle)->SetDownImage(cPath.AsCString().c_str(), NGUINet::NVectorConverter::ToColor(Mask).ToARGB());
	}


	NGUINet::NTextAlign NButton::Align::get()
	{
		return (NGUINet::NTextAlign)((NGin::GUI::IButton*)_Handle)->Align;
	}

	void NButton::Align::set(NTextAlign value)
	{
		((NGin::GUI::IButton*)_Handle)->Align = (NGin::GUI::TextAlign)(int)value;
	}

	NGUINet::NTextAlign NButton::VAlign::get()
	{
		return (NGUINet::NTextAlign)((NGin::GUI::IButton*)_Handle)->Align;
	}

	void NButton::VAlign::set(NTextAlign value)
	{
		((NGin::GUI::IButton*)_Handle)->VAlign = (NGin::GUI::TextAlign)(int)value;
	}

	bool NButton::UseBorder::get()
	{
		return ((NGin::GUI::IButton*)_Handle)->UseBorder;
	}

	void NButton::UseBorder::set(bool value)
	{
		((NGin::GUI::IButton*)_Handle)->UseBorder = value;
	}
}