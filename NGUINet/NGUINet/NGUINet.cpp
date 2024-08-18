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
// This is the main DLL file.

#include "stdafx.h"

//#include "NVector2.h"
#include "NGUINet.h"

#include "NWindow.h"
#include "NPictureBox.h"
#include "NRadar.h"
#include "NButton.h"
#include "NCheckBox.h"
#include "NLabel.h"
#include "NScrollBar.h"
#include "NProgressBar.h"
#include "NTrackBar.h"
#include "NListBox.h"
#include "NComboBox.h"
#include "NTextBox.h"

__declspec(dllimport) NGin::GUI::IGUIRenderer* CreateDirect3D9GUIRenderer(void* device,
													 const char* effectPath);

namespace NGUINet
{
	NGin::GUI::IGUIManager* LastManager = 0;

	void RenderCallback()
	{
		if(LastManager != 0)
			LastManager->Render();
	}

	void LostCallback()
	{
		if(LastManager != 0)
			LastManager->OnDeviceLost();
	}

	void ResetCallback(float X, float Y)
	{
		if(LastManager != 0)
			LastManager->OnDeviceReset(NGin::Math::Vector2(X, Y));
	}

	IntPtr NGUIManager::GetRenderCallback()
	{
		return _Render;
	}



	IntPtr NGUIManager::GetLostCallback()
	{
		return _Lost;
	}

	IntPtr NGUIManager::GetResetCallback()
	{
		return _Reset;
	}

#pragma region Constructors
	NGUIManager::NGUIManager(IntPtr D3DDevice, NVector2^ Resolution, System::String^ ShaderPath)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(ShaderPath);
		NGin::WString wShaderPath(pinnedPath);

		NGin::GUI::IGUIRenderer* GUIRenderer = NULL;

		GUIRenderer = CreateDirect3D9GUIRenderer((void*)D3DDevice, wShaderPath.AsCString().c_str());
		
		_Manager = NGin::GUI::CreateGUIManager(GUIRenderer, NVectorConverter::ToVector2(Resolution));

		_Controls = gcnew System::Collections::Generic::List<NGUINet::NControl^>();
		LastManager = _Manager;
		_Render = (IntPtr)&RenderCallback;
		_Reset = (IntPtr)&ResetCallback;
		_Lost = (IntPtr)&LostCallback;
	}

	NGUIManager::~NGUIManager()
	{
		LastManager = 0;
	}

	void NGUIManager::Register(NGUINet::NControl^ Control)
	{
		_Controls->Add(Control);
	}

	void NGUIManager::UnRegister(NGUINet::NControl^ Control)
	{
		_Controls->Remove(Control);
	}

	System::Collections::Generic::List<NGUINet::NControl^>^ NGUIManager::Controls()
	{
		return _Controls;
	}

	int NGUIManager::GetHandle()
	{
		return (int)_Manager;
	}

#pragma endregion

#pragma region CreateControls
	NWindow^ NGUIManager::CreateWindow(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateWindow(nName.AsCString().c_str(), nLocation, nSize);

		NWindow^ mControl = gcnew NWindow((IntPtr)nControl, this);

		return mControl;
	}

	NPictureBox^ NGUIManager::CreatePictureBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreatePictureBox(std::string(nName.AsCString().c_str()), nLocation, nSize);

		NPictureBox^ mControl = gcnew NPictureBox((IntPtr)nControl, this);

		return mControl;
	}

	NRadar^ NGUIManager::CreateRadar( System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size )
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);

		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateRadar(nName.AsCString().c_str(), nLocation, nSize);
		return gcnew NRadar((IntPtr)nControl, this);
	}

	NButton^ NGUIManager::CreateButton(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateButton(nName.AsCString().c_str(), nLocation, nSize);

		NButton^ mControl = gcnew NButton((IntPtr)nControl, this);

		return mControl;
	}

	NCheckBox^ NGUIManager::CreateCheckBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateCheckBox(nName.AsCString().c_str(), nLocation, nSize);

		NCheckBox^ mControl = gcnew NCheckBox((IntPtr)nControl, this);

		return mControl;
	}

	NLabel^ NGUIManager::CreateLabel(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateLabel(nName.AsCString().c_str(), nLocation, nSize);

		NLabel^ mControl = gcnew NLabel((IntPtr)nControl, this);

		return mControl;
	}

	NScrollBar^ NGUIManager::CreateScrollBar(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size, bool bVerticalScroll)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::ScrollOrientation Orientation = NGin::GUI::VerticalScroll;
		if(!bVerticalScroll)
			Orientation = NGin::GUI::HorizontalScroll;

		NGin::GUI::IControl* nControl = _Manager->CreateScrollBar(nName.AsCString().c_str(), nLocation, nSize, Orientation);

		NScrollBar^ mControl = gcnew NScrollBar((IntPtr)nControl, this);

		return mControl;
	}

	NProgressBar^ NGUIManager::CreateProgressBar(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateProgressBar(nName.AsCString().c_str(), nLocation, nSize);

		NProgressBar^ mControl = gcnew NProgressBar((IntPtr)nControl, this);

		return mControl;
	}

	NTrackBar^ NGUIManager::CreateTrackBar(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateTrackBar(nName.AsCString().c_str(), nLocation, nSize);

		NTrackBar^ mControl = gcnew NTrackBar((IntPtr)nControl, this);

		return mControl;
	}

	NListBox^ NGUIManager::CreateListBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateListBox(nName.AsCString().c_str(), nLocation, nSize);

		NListBox^ mControl = gcnew NListBox((IntPtr)nControl, this);

		return mControl;
	}

	NComboBox^ NGUIManager::CreateComboBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateComboBox(nName.AsCString().c_str(), nLocation, nSize);

		NComboBox^ mControl = gcnew NComboBox((IntPtr)nControl, this);

		return mControl;
	}

	NTextBox^ NGUIManager::CreateTextBox(System::String^ Name, NGUINet::NVector2^ Location, NGUINet::NVector2^ Size)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(Name);
		
		NGin::WString nName(pinnedName);
		NGin::Math::Vector2 nLocation = NVectorConverter::ToVector2(Location);
		NGin::Math::Vector2 nSize = NVectorConverter::ToVector2(Size);

		NGin::GUI::IControl* nControl = _Manager->CreateTextBox(nName.AsCString().c_str(), nLocation, nSize);

		NTextBox^ mControl = gcnew NTextBox((IntPtr)nControl, this);

		return mControl;
	}
#pragma endregion

#pragma region DestroyControls
	void NGUIManager::Destroy(NGUINet::NControl^ Control)
	{
		NGin::GUI::IControl* cControl = (NGin::GUI::IControl*)(void*)Control->Handle;
		_Manager->Destroy(cControl);
		UnRegister(Control);
	}
#pragma endregion
	
#pragma region Skins
	IntPtr NGUIManager::LoadFont(System::String^ Path, int size)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		NGin::GUI::IFont* cFont = _Manager->LoadFont(cPath.AsCString().c_str(), size);

		return (IntPtr)0;
	}

	int NGUIManager::LoadCursor(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return _Manager->LoadCursor(cPath.AsCString().c_str());
	}

	int NGUIManager::LoadAndAddSkin(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return _Manager->LoadAndAddSkin(cPath.AsCString().c_str());
	}

	void NGUIManager::SetCursor(int Cursor)
	{
		_Manager->SetCursor(Cursor);
	}
#pragma endregion
	
#pragma region Properties
	bool NGUIManager::ImportProperties(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		return _Manager->ImportProperties(cPath.AsCString().c_str());
	}

	bool NGUIManager::SetProperties(System::String^ SetName)
	{
		pin_ptr<const wchar_t> pinnedName = PtrToStringChars(SetName);
		NGin::WString cName(pinnedName);

		return _Manager->ImportProperties(cName.AsCString().c_str());
	}
#pragma endregion
	

#pragma region Rendering
	NGUINet::NVector2^ NGUIManager::GetResolution()
	{
		return NGUINet::NVectorConverter::FromVector2(_Manager->GetResolution());
	}

	void NGUIManager::OnDeviceLost()
	{
		_Manager->OnDeviceLost();
	}

	void NGUIManager::OnDeviceReset(NGUINet::NVector2^ NewResolution)
	{
		_Manager->OnDeviceReset(NGUINet::NVectorConverter::ToVector2(NewResolution));
	}

	bool NGUIManager::Update(NGUINet::NGUIUpdateParameters^ Parameters)
	{
		NGin::GUI::GUIUpdateParameters cParameters;
		cParameters.KeyDelete = Parameters->KeyDelete;
		cParameters.KeyDown = Parameters->KeyDown;
		cParameters.KeyEnd = Parameters->KeyEnd;
		cParameters.KeyHome = Parameters->KeyHome;
		cParameters.KeyInsert = Parameters->KeyInsert;
		cParameters.KeyLeft = Parameters->KeyLeft;
		cParameters.KeyRight = Parameters->KeyRight;
		cParameters.KeyUp =Parameters->KeyUp;
		cParameters.LeftDown = Parameters->LeftDown;
		cParameters.RightDown = Parameters->RightDown;
		cParameters.MousePosition = NGUINet::NVectorConverter::ToVector2(Parameters->MousePosition);

		Parameters->MouseThumb = nullptr;
		
		array<int>^ Buffer = Parameters->InputBuffer->ToArray();

		for(int i = 0; i < Buffer->Length; ++i)
			cParameters.InputBuffer->push_back(Buffer[i]);

		bool UpdateResult = _Manager->Update(&cParameters);

		if(cParameters.MouseThumb == 0)
		{
			Parameters->MouseThumb = nullptr;
			return UpdateResult;
		}

		for each(NGUINet::NControl^ C in _Controls)
		{
			if(C->Handle == (IntPtr)(void*)cParameters.MouseThumb)
			{
				Parameters->MouseThumb = C;
				return UpdateResult;
			}
		}

		return UpdateResult;
		
	}

	void NGUIManager::Render()
	{
		_Manager->Render();
	}
#pragma endregion

#pragma region Properties
	NGUINet::NControl^ NGUIManager::ControlFocus::get()
	{
		NGin::GUI::IControl* cControl = _Manager->ControlFocus();

		array<NGUINet::NControl^>^ Controls = _Controls->ToArray();
		for(int i = 0; i < Controls->Length; ++i)
			if((NGin::GUI::IControl*)(void*)Controls[i]->Handle == cControl)
				return Controls[i];

		return nullptr;
	}

	void NGUIManager::ControlFocus::set(NGUINet::NControl^ Control)
	{
		_Manager->ControlFocus((NGin::GUI::IControl*)(void*)Control->Handle);
	}
	
	System::String^ NGUIManager::FontDirectory::get()
	{
		return gcnew System::String((const char*)_Manager->FontDirectory().c_str());
	}

	void NGUIManager::FontDirectory::set(System::String^ Path)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(Path);
		NGin::WString cPath(pinnedPath);

		_Manager->FontDirectory(cPath.AsCString().c_str());
	}

	bool NGUIManager::CursorVisible::get()
	{
		return _Manager->CursorVisible;
	}

	void NGUIManager::CursorVisible::set(bool Visible)
	{
		_Manager->CursorVisible = Visible;
	}
#pragma endregion

}