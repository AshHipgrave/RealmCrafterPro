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

#include "NLabel.h"

namespace NGUINet
{	
	void NLabel_Click(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NLabel^>(Control) != nullptr)
				dynamic_cast<NLabel^>(Control)->__Click();
	}

	void NLabel_RightClick(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NLabel^>(Control) != nullptr)
				dynamic_cast<NLabel^>(Control)->__RightClick();
	}

	void NLabel_MouseDown(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NLabel^>(Control) != nullptr)
				dynamic_cast<NLabel^>(Control)->__MouseDown();
	}

	void NLabel_MouseEnter(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NLabel^>(Control) != nullptr)
				dynamic_cast<NLabel^>(Control)->__MouseEnter();
	}

	void NLabel_MouseLeave(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NLabel^>(Control) != nullptr)
				dynamic_cast<NLabel^>(Control)->__MouseLeave();
	}

	void NLabel_MouseMove(NGin::GUI::IControl* Sender, NGin::GUI::MouseEventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NLabel^>(Control) != nullptr)
				dynamic_cast<NLabel^>(Control)->__MouseMove(E->Location().X, E->Location().Y);
	}



	void NLabel::__Click()
	{
		if(&NLabel::Click != nullptr)
			Click(this, System::EventArgs::Empty);
	}

	void NLabel::__RightClick()
	{
		if(&NLabel::RightClick != nullptr)
			RightClick(this, System::EventArgs::Empty);
	}

	void NLabel::__MouseDown()
	{
		if(&NLabel::MouseDown != nullptr)
			MouseDown(this, System::EventArgs::Empty);
	}

	void NLabel::__MouseEnter()
	{
		if(&NLabel::MouseEnter != nullptr)
			MouseEnter(this, System::EventArgs::Empty);
	}


	void NLabel::__MouseLeave()
	{
		if(&NLabel::MouseLeave != nullptr)
			MouseLeave(this, System::EventArgs::Empty);
	}

	void NLabel::__MouseMove(float x, float y)
	{
		System::Windows::Forms::MouseEventArgs^ Mev = gcnew System::Windows::Forms::MouseEventArgs(
			System::Windows::Forms::MouseButtons::None,
			0,
			Convert::ToInt32(x),
			Convert::ToInt32(y),
			0);

		if(&NLabel::MouseMove != nullptr)
			MouseMove(this, Mev);
	}


	NLabel::NLabel(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::ILabel*)_Handle)->Click()->AddEvent(&NLabel_Click);
		((NGin::GUI::ILabel*)_Handle)->RightClick()->AddEvent(&NLabel_RightClick);
		((NGin::GUI::ILabel*)_Handle)->MouseDown()->AddEvent(&NLabel_MouseDown);
		((NGin::GUI::ILabel*)_Handle)->MouseEnter()->AddEvent(&NLabel_MouseEnter);
		((NGin::GUI::ILabel*)_Handle)->MouseLeave()->AddEvent(&NLabel_MouseLeave);
		((NGin::GUI::ILabel*)_Handle)->MouseMove()->AddEvent(&NLabel_MouseMove);
	}

	IntPtr NLabel::Font::get()
	{
		return (IntPtr)(void*)((NGin::GUI::ILabel*)_Handle)->Font;
	}

	void NLabel::Font::set(IntPtr value)
	{
		((NGin::GUI::ILabel*)_Handle)->Font = (NGin::GUI::IFont*)(void*)value;
	}

	NGUINet::NTextAlign NLabel::Align::get()
	{
		return (NGUINet::NTextAlign)(int)((NGin::GUI::ILabel*)_Handle)->Align;
	}

	void NLabel::Align::set(NGUINet::NTextAlign value)
	{
		((NGin::GUI::ILabel*)_Handle)->Align = (NGin::GUI::TextAlign)(int)value;
	}


	NGUINet::NTextAlign NLabel::VAlign::get()
	{
		return (NGUINet::NTextAlign)(int)((NGin::GUI::ILabel*)_Handle)->VAlign;
	}

	void NLabel::VAlign::set(NGUINet::NTextAlign value)
	{
		((NGin::GUI::ILabel*)_Handle)->VAlign = (NGin::GUI::TextAlign)(int)value;
	}


	bool NLabel::Multiline::get()
	{
		return ((NGin::GUI::ILabel*)_Handle)->Multiline;
	}

	void NLabel::Multiline::set(bool value)
	{
		((NGin::GUI::ILabel*)_Handle)->Multiline = value;
	}


	bool NLabel::InlineStringProcessing::get()
	{
		return ((NGin::GUI::ILabel*)_Handle)->InlineStringProcessing;
	}

	void NLabel::InlineStringProcessing::set(bool value)
	{
		((NGin::GUI::ILabel*)_Handle)->InlineStringProcessing = value;
	}


	NGUINet::NVector2^ NLabel::ScissorWindow::get()
	{
		return NGUINet::NVectorConverter::FromVector2(((NGin::GUI::ILabel*)_Handle)->ScissorWindow);
	}

	void NLabel::ScissorWindow::set(NGUINet::NVector2^ value)
	{
		((NGin::GUI::ILabel*)_Handle)->ScissorWindow = NGUINet::NVectorConverter::ToVector2(value);
	}


	bool NLabel::ForceScissoring::get()
	{
		return ((NGin::GUI::ILabel*)_Handle)->ForceScissoring;
	}

	void NLabel::ForceScissoring::set(bool value)
	{
		((NGin::GUI::ILabel*)_Handle)->ForceScissoring = value;
	}


	NGUINet::NVector2^ NLabel::ScrollOffset::get()
	{
		return NGUINet::NVectorConverter::FromVector2(((NGin::GUI::ILabel*)_Handle)->ScrollOffset);
	}

	void NLabel::ScrollOffset::set(NGUINet::NVector2^ value)
	{
		((NGin::GUI::ILabel*)_Handle)->ScrollOffset = NGUINet::NVectorConverter::ToVector2(value);
	}

	float NLabel::InternalHeight::get()
	{
		return ((NGin::GUI::ILabel*)_Handle)->InternalHeight;
	}

	float NLabel::InternalWidth::get()
	{
		return ((NGin::GUI::ILabel*)_Handle)->InternalWidth;
	}

}