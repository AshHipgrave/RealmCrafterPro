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

#include <vector>
#include <Color.h>
#include <Vector2.h>
#include <Vector3.h>
#include <string>
#include <List.h>
#include <Type.h>
#include <IEventHandler.h>

namespace NGin
{
	namespace GUI
	{
		class IGUIManager;
		class IControl;
		class EventArgs
		{
		};

		class MouseEventArgs : public EventArgs
		{
			NGin::Math::Vector2 _Location;

		public:

			MouseEventArgs(NGin::Math::Vector2 location)
			{
				_Location = location;
			}

			//! Get location of mouse during event
			NGin::Math::Vector2 Location() { return _Location; }

		};

		//! Type Definition for EventHandler structure
		/*!
		The EventHandler structure is the default callback
		event structure for controls. TODO: See EventHandler Docs
		*/
		typedef NGin::IEventHandler<IControl, EventArgs> EventHandler;

		//! Type Definition for MouseEventHandler structure
		typedef NGin::IEventHandler<IControl, MouseEventArgs> MouseEventHandler;

		//! Update Parameters structure for updating the GUI system.
		/*!
		When the executing program updates, it must also update the GUI.
		GUIUpdateParameters is used to send information about mouse and
		keyboard events.
		*/
		struct GUIUpdateParameters
		{
			GUIUpdateParameters()
			{
				LeftDown = RightDown = Handled = ModalProc = MouseOver = false;
				MouseBusy = false;
				InputBuffer = new std::vector<int>();
				MouseThumb = 0;
			}

			~GUIUpdateParameters()
			{
				delete InputBuffer;
			}

			//! Position of the mouse (units based on resolution)
			NGin::Math::Vector2 MousePosition;

			//! Status of Left Mouse Button; true is 'down'
			bool LeftDown;
			
			//! Status of Right Mouse Button; true is 'down'
			bool RightDown;

			//! Status of control keys
			bool KeyLeft, KeyRight, KeyUp, KeyDown, KeyHome, KeyEnd, KeyInsert, KeyDelete;

			//! Mouse is over a gadget
			/*! This value is important if your project
			 receives input from the mouse but not through
			 NGUI. If this value is true, do not process
			 clicks.*/
			bool MouseBusy;

			//! Used internally, initialized to false
			bool Handled;

			//! Used internally, initialized to false
			bool ModalProc;

			//! Used internally, initialized to false
			bool MouseOver;

			//! Used to see what the mouse if hovering over
			IControl* MouseThumb;

			//! Pointer to a list of input characters
			std::vector<int>* InputBuffer;
		};

		//! Control Interface class
		/*!
		Base class for all GUI Controls
		*/
		class IControl
		{
		protected:

			NGin::Math::Color _ForeColor, _BackColor;
			List<IControl*> _Controls;
			IControl* _Parent;
			bool _Enabled, _Visible, _Locked;
			NGin::Math::Vector2 _Location, _Size, _MaxSize, _MinSize;
			NGin::Math::Vector2 _GlobalLocation;
			NGin::Math::Vector2 _ScreenScale;
			std::string _Text, _Name;
			int _Skin;
			NGin::Type _Type;
			void* _Tag;
			IGUIManager* _Manager;

			virtual void Remove(IControl* Control);

			virtual void OnTransform();
			virtual void OnNameChange();
			virtual void OnTextChange();
			virtual void OnEnabledChange();
			virtual void OnVisibleChange();
			virtual void OnBackColorChange();
			virtual void OnForeColorChange();
			virtual void OnSkinChange();
			virtual void OnSizeChange();
			virtual void OnLocationChange();


		public:

			IControl(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager);
			virtual ~IControl();

			//! Get the 'Global Location' of the control
			virtual NGin::Math::Vector2 GlobalLocation();

			//! Get Control Children
			virtual List<IControl*>* Controls();

			//! Get Control Type
			/*!
			TODO: Types
			*/
			virtual NGin::Type GetType();

			//! Control Location
			__declspec(property(get=zzGet_Location, put=zzPut_Location)) NGin::Math::Vector2 Location;
			virtual void zzPut_Location(NGin::Math::Vector2 Location);
			virtual NGin::Math::Vector2 zzGet_Location();
			
			//! Control Size
			__declspec(property(get=zzGet_Size, put=zzPut_Size)) NGin::Math::Vector2 Size;
			virtual void zzPut_Size(NGin::Math::Vector2 Size);
			virtual NGin::Math::Vector2 zzGet_Size();

			
			//! Control Maximum size
			__declspec(property(get=zzGet_MaximumSize, put=zzPut_MaximumSize)) NGin::Math::Vector2 MaximumSize;
			virtual void zzPut_MaximumSize(NGin::Math::Vector2 MaximumSize);
			virtual NGin::Math::Vector2 zzGet_MaximumSize();

			//! Control Minimum size
			__declspec(property(get=zzGet_MinimumSize, put=zzPut_MinimumSize)) NGin::Math::Vector2 MinimumSize;
			virtual void zzPut_MinimumSize(NGin::Math::Vector2 MinimumSize);
			virtual NGin::Math::Vector2 zzGet_MinimumSize();

			//! Control Name
			__declspec(property(get=zzGet_Name, put=zzPut_Name)) std::string Name;
			virtual void zzPut_Name(std::string Name);
			virtual std::string zzGet_Name();

			//! Control Text
			__declspec(property(get=zzGet_Text, put=zzPut_Text)) std::string Text;
			virtual void zzPut_Text(std::string Text);
			virtual std::string zzGet_Text();

			//! Control Enabled
			__declspec(property(get=zzGet_Enabled, put=zzPut_Enabled)) bool Enabled;
			virtual void zzPut_Enabled(bool Enabled);
			virtual bool zzGet_Enabled();

			//! Control Visibility
			__declspec(property(get=zzGet_Visible, put=zzPut_Visible)) bool Visible;
			virtual void zzPut_Visible(bool Visible);
			virtual bool zzGet_Visible();

			//! Control Background color
			__declspec(property(get=zzGet_BackColor, put=zzPut_BackColor)) NGin::Math::Color BackColor;
			virtual void zzPut_BackColor(NGin::Math::Color BackColor);
			virtual NGin::Math::Color zzGet_BackColor();

			//! Control Foreground color
			__declspec(property(get=zzGet_ForeColor, put=zzPut_ForeColor)) NGin::Math::Color ForeColor;
			virtual void zzPut_ForeColor(NGin::Math::Color ForeColor);
			virtual NGin::Math::Color zzGet_ForeColor();

			//! Control Skin
			__declspec(property(get=zzGet_Skin, put=zzPut_Skin)) int Skin;
			virtual void zzPut_Skin(int Skin);
			virtual int zzGet_Skin();

			//! Control Locked status
			__declspec(property(get=zzGet_Locked, put=zzPut_Locked)) bool Locked;
			virtual void zzPut_Locked(bool Locked);
			virtual bool zzGet_Locked();

			//! Control Tag
			/*!
			Control tags are a pointer to any given object.
			The control will not handle this object in any way, and you are
			responsible for what happens to it. Remember to clear the tag if
			you delete it.
			\param Tag Pointer to tag object
			*/
			__declspec(property(get=zzGet_Tag, put=zzPut_Tag)) void* Tag;
			virtual void zzPut_Tag(void* Tag);
			virtual void* zzGet_Tag();


			//! Control Parent
			__declspec(property(get=zzGet_Parent, put=zzPut_Parent)) IControl* Parent;
			virtual IControl* zzGet_Parent();
			virtual void zzPut_Parent(IControl* Parent);

			//! Override for lost devices
			/*!
			TODO: Custom control documentation
			*/
			virtual void OnDeviceLost();

			//! Override for reset devices
			/*!
			TODO: Custom control documentation
			*/
			virtual void OnDeviceReset();

			//! Override for control update
			/*!
			TODO: Custom control documentation
			*/
			virtual bool Update(GUIUpdateParameters* Parameters);

			//! Override for control rendering
			/*!
			TODO: Custom control documentation
			*/
			virtual void Render();

			//! Bring the control to the top of all controls
			virtual void BringToFront();

			//! Send the control to the bottom of all controls
			virtual void SendToBack();

		};

	}
}