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
#pragma once

#include <IGUIManager.h>
#include <ArrayList.h>
#include "CSkin.h"
using namespace NGin;

namespace NGin
{
	namespace GUI
	{

		class CSkin;
		class CPropertySet;
		class CGUIManager : public IGUIManager//, public IControl
		{
			struct IntCursor
			{
				IGUITexture* Texture;
				std::string Name;
				NGin::Math::Vector2 Size;
			};

			IGUIRenderer* Renderer;
			
			NGin::Math::Vector2 Resolution, Scale;
			ArrayList<CSkin*> Skins;
			ArrayList<IFont*> Fonts;
			ArrayList<IntCursor*> Cursors;
			int CurrentSkin;

			std::string FontDir;

			IControl* _ControlFocus;

			ILabel* Lbl;
			IPictureBox* CursorImage;

			ArrayList<CPropertySet*> PropertySets;
			List<IControl*> DeleteQueue;
			List<IControl*> FrontQueue;
			List<IControl*> BackQueue;

			bool UsingPerfHUD;
			bool LeftBusyLastFrame;
			bool RightBusyLastFrame;
			bool DeleteLock;

		public:

			CGUIManager(NGin::Math::Vector2& ScreenScale);
			virtual ~CGUIManager();
			virtual bool Initialize(IGUIRenderer* renderer, NGin::Math::Vector2 &ScreenRes);

			virtual void BringToFront(IControl* Control);
			virtual void SendToBack(IControl* Control);

			virtual bool GetDeleteLock();

			virtual IWindow* CreateWindow(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual IPictureBox* CreatePictureBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual ITabControl* CreateTabControl(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual IRadar* CreateRadar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
 			virtual IButton* CreateButton(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
 			virtual ICheckBox* CreateCheckBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual ILabel* CreateLabel(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual IScrollBar* CreateScrollBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size, ScrollOrientation ScrollOrientation);
			virtual IProgressBar* CreateProgressBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual ITrackBar* CreateTrackBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual IListBox* CreateListBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual IComboBox* CreateComboBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);
			virtual ITextBox* CreateTextBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size);

			virtual int LoadCursor(std::string Path);
			virtual IFont* LoadFont(std::string Path, int Size);
			virtual void FontDirectory(std::string FontPath);
			
			virtual std::string FontDirectory();

			virtual int LoadAndAddSkin(std::string SkinFile);
			virtual ISkin* GetSkin(int Index);
			virtual NGin::Math::Vector2 GetResolution();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset(NGin::Math::Vector2& NewResolution);
			virtual IGUIRenderer* GetRenderer();

			virtual bool Update(GUIUpdateParameters* Parameters);
			virtual void Render();
			virtual void SetSkin(int Index);
			virtual void SetTexture(IGUITexture* texture);
			virtual void SetPosition(NGin::Math::Vector2& Position);

			virtual void ControlFocus(IControl* Control);
			virtual IControl* ControlFocus();

			virtual void SetCursor(int Cursor);

			virtual bool zzGet_CursorVisible();
			virtual void zzPut_CursorVisible(bool CursorVisible);

			virtual bool ImportProperties(std::string Name);
			virtual void SetProperties(std::string SetName);
			virtual void SetSingleProperty(std::string SetName, std::string ControlName);
			virtual void SetSingleProperty(std::string SetName, std::string ControlName, int SearchDepth);

			virtual IControl* GetSelf();

			virtual void Destroy(IControl* Control);

			virtual void CorrectRect(Math::Vector4 &Rectangle);
		};


	}
}
