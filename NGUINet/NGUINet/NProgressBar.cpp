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

#include "NProgressBar.h"

namespace NGUINet
{
	void NProgressBar_Click(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NProgressBar^>(Control) != nullptr)
				dynamic_cast<NProgressBar^>(Control)->__Click();
	}

	void NProgressBar_RightClick(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NProgressBar^>(Control) != nullptr)
				dynamic_cast<NProgressBar^>(Control)->__RightClick();
	}

	void NProgressBar_MouseDown(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NProgressBar^>(Control) != nullptr)
				dynamic_cast<NProgressBar^>(Control)->__MouseDown();
	}

	void NProgressBar_MouseEnter(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NProgressBar^>(Control) != nullptr)
				dynamic_cast<NProgressBar^>(Control)->__MouseEnter();
	}

	void NProgressBar_MouseLeave(NGin::GUI::IControl* Sender, NGin::GUI::EventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NProgressBar^>(Control) != nullptr)
				dynamic_cast<NProgressBar^>(Control)->__MouseLeave();
	}

	void NProgressBar_MouseMove(NGin::GUI::IControl* Sender, NGin::GUI::MouseEventArgs* E)
	{
		NGUINet::NControl^ Control = (NGUINet::NControl^)System::Runtime::InteropServices::GCHandle::FromIntPtr((IntPtr)Sender->Tag).Target;

		if(Control != nullptr)
			if(dynamic_cast<NProgressBar^>(Control) != nullptr)
				dynamic_cast<NProgressBar^>(Control)->__MouseMove(E->Location().X, E->Location().Y);
	}



	void NProgressBar::__Click()
	{
		if(&NProgressBar::Click != nullptr)
			Click(this, System::EventArgs::Empty);
	}

	void NProgressBar::__RightClick()
	{
		if(&NProgressBar::RightClick != nullptr)
			RightClick(this, System::EventArgs::Empty);
	}

	void NProgressBar::__MouseDown()
	{
		if(&NProgressBar::MouseDown != nullptr)
			MouseDown(this, System::EventArgs::Empty);
	}

	void NProgressBar::__MouseEnter()
	{
		if(&NProgressBar::MouseEnter != nullptr)
			MouseEnter(this, System::EventArgs::Empty);
	}


	void NProgressBar::__MouseLeave()
	{
		if(&NProgressBar::MouseLeave != nullptr)
			MouseLeave(this, System::EventArgs::Empty);
	}

	void NProgressBar::__MouseMove(float x, float y)
	{
		System::Windows::Forms::MouseEventArgs^ Mev = gcnew System::Windows::Forms::MouseEventArgs(
			System::Windows::Forms::MouseButtons::None,
			0,
			Convert::ToInt32(x),
			Convert::ToInt32(y),
			0);

		if(&NProgressBar::MouseMove != nullptr)
			MouseMove(this, Mev);
	}

	NProgressBar::NProgressBar(IntPtr Handle, NGUINet::NGUIManager^ Manager) : NControl(Handle, Manager)
	{
		System::Runtime::InteropServices::GCHandle GCObject = System::Runtime::InteropServices::GCHandle::Alloc(this);
		_Handle->Tag = (void*)System::Runtime::InteropServices::GCHandle::ToIntPtr(GCObject);
		
		((NGin::GUI::IProgressBar*)_Handle)->Click()->AddEvent(&NProgressBar_Click);
		((NGin::GUI::IProgressBar*)_Handle)->RightClick()->AddEvent(&NProgressBar_RightClick);
		((NGin::GUI::IProgressBar*)_Handle)->MouseDown()->AddEvent(&NProgressBar_MouseDown);
		((NGin::GUI::IProgressBar*)_Handle)->MouseEnter()->AddEvent(&NProgressBar_MouseEnter);
		((NGin::GUI::IProgressBar*)_Handle)->MouseLeave()->AddEvent(&NProgressBar_MouseLeave);
		((NGin::GUI::IProgressBar*)_Handle)->MouseMove()->AddEvent(&NProgressBar_MouseMove);
	}
	int NProgressBar::Value::get()
	{
		return ((NGin::GUI::IProgressBar*)_Handle)->Value;
	}

	void NProgressBar::Value::set(int value)
	{
		((NGin::GUI::IProgressBar*)_Handle)->Value = value;
	}
}