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
// NGUINet.h

#pragma once

#include <IGUIManager.h>
#include <nginstring.h>
#include "NVector2.h"
#include "NVectorConverter.h"
#include "NGUIUpdateParameters.h"

#include "NControl.h"


#include <vcclr.h>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace NGUINet {

	ref class NControl;
	ref class NWindow;
	ref class NPictureBox;
	ref class NRadar;
	ref class NButton;
	ref class NCheckBox;
	ref class NLabel;
	ref class NScrollBar;
	ref class NProgressBar;
	ref class NTrackBar;
	ref class NListBox;
	ref class NComboBox;
	ref class NTextBox;

	public ref class NGUIManager
	{
	protected:
		NGin::GUI::IGUIManager* _Manager;
		System::Collections::Generic::List<NGUINet::NControl^>^ _Controls;
		IntPtr _Render, _Reset, _Lost;

	public:
		NGUIManager(IntPtr D3DDevice, NVector2^ Resolution, System::String^ ShaderPath);
		~NGUIManager();


		IntPtr GetRenderCallback();	
		IntPtr GetLostCallback();
		IntPtr GetResetCallback();
		void Register(NGUINet::NControl^ Control);
		void UnRegister(NGUINet::NControl^ Control);
		System::Collections::Generic::List<NGUINet::NControl^>^ Controls();

		NWindow^ CreateWindow(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NPictureBox^ CreatePictureBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NRadar^ CreateRadar(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);

		/// <summary>
		/// Creates a GUI Button
		/// </summary>
		/// <param name="Name">Control Name</param>
		/// <param name="Location">Control Position</param>
		/// <param name="Size">Control Size</param>
		/// <returns>Button object</return>
		NButton^ CreateButton(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NCheckBox^ CreateCheckBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NLabel^ CreateLabel(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NScrollBar^ CreateScrollBar(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size, bool VerticalScroll);
		NProgressBar^ CreateProgressBar(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NTrackBar^ CreateTrackBar(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NListBox^ CreateListBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NComboBox^ CreateComboBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);
		NTextBox^ CreateTextBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size);

		IntPtr LoadFont(System::String^ Path, int Size);
		int LoadCursor(System::String^ Path);
		int LoadAndAddSkin(System::String^ Path);
		NGUINet::NVector2^ GetResolution();
		void SetCursor(int Cursor);
		bool ImportProperties(System::String^ Path);
		bool SetProperties(System::String^ SetName);
		void Destroy(NGUINet::NControl^ Control);

		void OnDeviceLost();
		void OnDeviceReset(NGUINet::NVector2^ NewResolution);
		bool Update(NGUINet::NGUIUpdateParameters^ Parameters);
		void Render();
		int GetHandle();

		property NGUINet::NControl^ ControlFocus
		{
			NGUINet::NControl^ get();
			void set(NGUINet::NControl^);
		}

		property System::String^ FontDirectory
		{
			System::String^ get();
			void set(System::String^);
		}

		property bool CursorVisible
		{
			bool get();
			void set(bool);
		}
	};
}
