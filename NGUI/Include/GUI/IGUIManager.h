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

#include <IGUIRenderer.h>
#include <vector4.h>
#include "IControl.h"
#include "IWindow.h"
#include "IPictureBox.h"
#include "IRadar.h"
#include "IButton.h"
#include "ICheckBox.h"
#include "IScrollBar.h"
#include "IProgressBar.h"
#include "ITrackBar.h"
#include "IListBox.h"
#include "IComboBox.h"
#include "ITextbox.h"
#include "ILabel.h"
#include "IFont.h"
#include "ISkin.h"
#include "ITabControl.h"

// Haxes!
#ifdef CreateWindow
#undef CreateWindow
#endif

#ifdef CreateDialog
#undef CreateDialog
#endif

#ifdef LoadCursor
#undef LoadCursor
#endif

namespace NGin
{
	namespace GUI
	{
		struct GUIVertex
		{
			Math::Vector3 Position;
			unsigned int Color;
			Math::Vector2 TexCoord;
		};

		class IFont;
		class ILabel;
		class ISkin;

		//! GUI Manager Interface class
		/*!
		TODO: GUI Manager
		*/
		class IGUIManager : public IControl
		{
		public:

			IGUIManager(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IGUIManager() {}

			//! Used to check if an object is being prematurely deleted
			virtual bool GetDeleteLock() = 0;

			//! Create a window
			virtual IWindow* CreateWindow(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a picturebox
			virtual IPictureBox* CreatePictureBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a tab control
			virtual ITabControl* CreateTabControl(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;
			
			//! Create a radar
			virtual IRadar* CreateRadar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a button
			virtual IButton* CreateButton(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a checkbox
			virtual ICheckBox* CreateCheckBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;
 
 			//! Create a label
 			virtual ILabel* CreateLabel(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a scrollbar
			virtual IScrollBar* CreateScrollBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size, ScrollOrientation ScrollOrientation) = 0;

			//! Create a progressbar
			virtual IProgressBar* CreateProgressBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a trackbar
			virtual ITrackBar* CreateTrackBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a listbox
			virtual IListBox* CreateListBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a combobox
			virtual IComboBox* CreateComboBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Create a textbox
			virtual ITextBox* CreateTextBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size) = 0;

			//! Load a font
			virtual IFont* LoadFont(std::string Path, int Size) = 0;

			//! Load a cursor
			virtual int LoadCursor(std::string Path) = 0;

			//! Load a skin
			virtual int LoadAndAddSkin(std::string SkinFile) = 0;

			//! Set the location of fonts for loading
			virtual void FontDirectory(std::string FontPath) = 0;

			//! Get the location of fonts
			virtual std::string FontDirectory() = 0;

			//! Get the screen resolution
			virtual NGin::Math::Vector2 GetResolution() = 0;

			//! Get the renderer used to draw controls
			virtual IGUIRenderer* GetRenderer() = 0;

			//! Get a skin interface 
			virtual ISkin* GetSkin(int Index) = 0;

			//! Set the mouse cursor
			virtual void SetCursor(int Cursor) = 0;

			//! Set the focused control
			virtual void ControlFocus(IControl* Control) = 0;

			//! Get the focused control
			virtual IControl* ControlFocus() = 0;

			//! Cursor visibility
			__declspec(property(get=zzGet_CursorVisible, put=zzPut_CursorVisible)) bool CursorVisible;
			virtual void zzPut_CursorVisible(bool CursorVisible) = 0;
			virtual bool zzGet_CursorVisible() = 0;

			//! Import a properties file ready to apply across components
			virtual bool ImportProperties(std::string Name) = 0;

			//! Apply an imported property set
			virtual void SetProperties(std::string SetName) = 0;

			//! Apply a single property from a single set. Option depth parameter can be used if minor sets are used.
			virtual void SetSingleProperty(std::string SetName, std::string ControlName, int SearchDepth) = 0;

			//! Apply a single property from a single set
			virtual void SetSingleProperty(std::string SetName, std::string ControlName) = 0;

			//! Get "Self"
			virtual IControl* GetSelf() = 0;

			//! Destroy a control
			virtual void Destroy(IControl* Control) = 0;

			//! Resize a rectangle to the viewport
			virtual void CorrectRect(Math::Vector4 &Rectangle) = 0;
			
			//! DeviceLost raised
			/*!
			Call this when the main application loses the device.
			TODO: Device status
			*/
			virtual void OnDeviceLost() = 0;

			//! DeviceReset raised
			/*!
			Call this when the main application resets the device.
			TODO: Device status
			*/
			virtual void OnDeviceReset(NGin::Math::Vector2& NewResolution) = 0;

			//! Allow the GUI to update all controls
			/*!
			TODO: GUI Update
			*/
			virtual bool Update(GUIUpdateParameters* Parameters) = 0;

			//! Render the GUI
			/*!
			TODO: GUI Rendering
			*/
			virtual void Render() = 0;

			//! Used to set the rendering skin for custom controls
			virtual void SetSkin(int Index) = 0;

			//! Used to set the rendering texture for custom controls
			virtual void SetTexture(IGUITexture* Texture) = 0;

			//! Used to set the rendering location for custom controls
			virtual void SetPosition(NGin::Math::Vector2& Position) = 0;

			//! Brings a control to the front (queued)
			virtual void BringToFront(IControl* Control) = 0;

			//! Sends a control to the back (queued)
			virtual void SendToBack(IControl* Control) = 0;
		};

		//#if defined(EXPORTS)
		//#define DLLEX __declspec(dllexport)
		//#else
		//#define DLLEX __declspec(dllimport)
		//#endif

		//! Create the GUI Manager in order to use the GUI system
		/*!
		TODO: GUI Manager
		\param D3DDevice Pointer to IDirect3DDevice9 structure which the GUI system will use
		\param Resolution Resolution of the screen
		\return Handle of the GUI Manager
		*/
		IGUIManager* CreateGUIManager(NGin::GUI::IGUIRenderer* Renderer, NGin::Math::Vector2& Resolution);
	}
}