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

#include <ArrayList.h>
#include <Color.h>
#include <Vector2.h>
#include <Vector3.h>
#include <string>
#include <List.h>
#include <Type.h>
#include <IEventHandler.h>

#include <IGUIManager.h>

namespace NGin
{
	namespace GUI
	{

		NGin::Math::Vector2 IControl::GlobalLocation()
		{
			return _GlobalLocation;
		}

		void IControl::Remove(IControl* Control)
		{
			_Controls.Remove(Control);
		}

		void IControl::OnTransform()
		{
			if(_Parent != 0)
				_GlobalLocation = _Parent->GlobalLocation() + _Location;
			else
				_GlobalLocation = _Location;

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->OnTransform();

				nextc(CIt, IControl, _Controls);
			}
		}

		void IControl::OnNameChange()
		{

		}

		void IControl::OnTextChange()
		{

		}
		
		void IControl::OnEnabledChange()
		{

		}

		void IControl::OnVisibleChange()
		{

		}

		void IControl::OnBackColorChange()
		{

		}

		void IControl::OnForeColorChange()
		{

		}

		void IControl::OnSkinChange()
		{

		}

		void IControl::OnSizeChange()
		{

		}

		void IControl::OnLocationChange()
		{

		}

		IControl::IControl(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager)
		{
			_Type = NGin::Type("ICTL", "IControl GUI Interface");
			_Parent = 0;
			_Skin = 0;
			_Locked = false;
			_Enabled = true;
			_Visible = true;
			_ScreenScale = ScreenScale;
			_GlobalLocation = NGin::Math::Vector2(0.0f, 0.0f);
			_MaxSize = NGin::Math::Vector2(1.0f, 1.0f) / _ScreenScale;
			_MinSize = NGin::Math::Vector2(0.0f, 0.0f);
			_BackColor = NGin::Math::Color(1.0f, 1.0f, 1.0f, 1.0f);
			_ForeColor = NGin::Math::Color(0.0f, 0.0f, 0.0f, 1.0f);
			_Tag = 0;
			_Manager = Manager;
		}

		IControl::~IControl()
		{
			if(_Manager != 0)
			{
				if(!_Manager->GetDeleteLock())
					GlobalFail(std::string("Error: Premature deletion of NGUI Control!\n") + this->GetType().GuidString() + ": " + this->Name);

				if(_Manager->ControlFocus() == this)
					_Manager->ControlFocus(NULL);
			}

			if(_Parent != 0)
				_Parent->Remove(this);

			ArrayList<IControl*> Children;

			// Delete every control
			foreachc(CIt, IControl, _Controls)
			{
				Children.Add(*CIt);

				nextc(CIt, IControl, _Controls);
			}

			for(unsigned int i = 0; i < Children.Size(); ++i)
				delete Children[i];
		}

		//! Get Control Children
		List<IControl*>* IControl::Controls() {return &_Controls;}

		//! Get Control Type
		/*!
		TODO: Types
		*/
		NGin::Type IControl::GetType(){return _Type;}

		//! Control Location
		void IControl::zzPut_Location(NGin::Math::Vector2 Location) {_Location = Location;OnTransform();OnLocationChange();}
		NGin::Math::Vector2 IControl::zzGet_Location() {return _Location;}
		
		//! Control Size
		void IControl::zzPut_Size(NGin::Math::Vector2 Size)
		{
			_Size = Size;
			if(_Size.X > _MaxSize.X)
				_Size.X = _MaxSize.X;
			if(_Size.Y > _MaxSize.Y)
				_Size.Y = _MaxSize.Y;

			if(_Size.X < _MinSize.X)
				_Size.X = _MinSize.X;
			if(_Size.Y < _MinSize.Y)
				_Size.Y = _MinSize.Y;

			OnTransform();
			OnSizeChange();
		}
		NGin::Math::Vector2 IControl::zzGet_Size() {return _Size;}

		
		//! Control Maximum size
		void IControl::zzPut_MaximumSize(NGin::Math::Vector2 MaximumSize) {_MaxSize = MaximumSize;Size = _Size;}
		NGin::Math::Vector2 IControl::zzGet_MaximumSize() {return _MaxSize;}

		//! Control Minimum size
		void IControl::zzPut_MinimumSize(NGin::Math::Vector2 MinimumSize) {_MinSize = MinimumSize;Size = _Size;}
		NGin::Math::Vector2 IControl::zzGet_MinimumSize() {return _MinSize;}

		//! Control Name
		void IControl::zzPut_Name(std::string Name) {_Name = Name;OnNameChange();}
		std::string IControl::zzGet_Name() {return _Name;}

		//! Control Text
		void IControl::zzPut_Text(std::string Text) {_Text = Text;OnTextChange();}
		std::string IControl::zzGet_Text() {return _Text;}

		//! Control Enabled
		void IControl::zzPut_Enabled(bool Enabled) {_Enabled = Enabled;OnEnabledChange();}
		bool IControl::zzGet_Enabled() {return _Enabled;}

		//! Control Visibility
		void IControl::zzPut_Visible(bool Visible) {_Visible = Visible;OnVisibleChange();}
		bool IControl::zzGet_Visible() {return _Visible;}

		//! Control Background color
		void IControl::zzPut_BackColor(NGin::Math::Color BackColor) {_BackColor = BackColor;OnBackColorChange();}
		NGin::Math::Color IControl::zzGet_BackColor() {return _BackColor;}

		//! Control Foreground color
		void IControl::zzPut_ForeColor(NGin::Math::Color ForeColor) {_ForeColor = ForeColor;OnForeColorChange();}
		NGin::Math::Color IControl::zzGet_ForeColor() {return _ForeColor;}

		//! Control Skin
		void IControl::zzPut_Skin(int Skin){_Skin = Skin;OnSkinChange();}
		int IControl::zzGet_Skin() {return _Skin;}

		//! Control Locked status
		void IControl::zzPut_Locked(bool Locked){_Locked = Locked;}
		bool IControl::zzGet_Locked() {return _Locked;}

		//! Control Tag
		/*!
		Control tags are a pointer to any given object.
		The control will not handle this object in any way, and you are
		responsible for what happens to it. Remember to clear the tag if
		you delete it.
		\param Tag Pointer to tag object
		*/
		void IControl::zzPut_Tag(void* Tag){_Tag = Tag;}
		void* IControl::zzGet_Tag() {return _Tag;}


		//! Control Parent
		IControl* IControl::zzGet_Parent() {return _Parent;}
		void IControl::zzPut_Parent(IControl* Parent)
		{
			// If Parent is NULL, we'll lose this node
			if(Parent == 0)
				return;

			if(_Parent)
				_Parent->Remove(this);

			_Parent = Parent;
			_Parent->Controls()->Add(this);
			_Parent->Location = _Parent->Location;
			//OnTransform();
		}

		//! Override for lost devices
		/*!
		TODO: Custom control documentation
		*/
		void IControl::OnDeviceLost()
		{
			// Copy to arraylist incase anyone needs to be free'd
			ArrayList<IControl*> Children;

			// Delete every control
			foreachc(CIt, IControl, _Controls)
			{
				Children.Add(*CIt);

				nextc(CIt, IControl, _Controls);
			}

			for(unsigned int i = 0; i < Children.Size(); ++i)
				Children[i]->OnDeviceLost();	
		};

		//! Override for reset devices
		/*!
		TODO: Custom control documentation
		*/
		void IControl::OnDeviceReset()
		{
			// Copy to arraylist incase anyone needs to be free'd
			ArrayList<IControl*> Children;

			// Delete every control
			foreachc(CIt, IControl, _Controls)
			{
				Children.Add(*CIt);

				nextc(CIt, IControl, _Controls);
			}

			for(unsigned int i = 0; i < Children.Size(); ++i)
				Children[i]->OnDeviceReset();	
		};

		//! Override for control update
		/*!
		TODO: Custom control documentation
		*/
		bool IControl::Update(GUIUpdateParameters* Parameters){return false;};

		//! Override for control rendering
		/*!
		TODO: Custom control documentation
		*/
		void IControl::Render(){};

		//! Bring the control to the top of all controls
		void IControl::BringToFront()
		{
			if(_Manager != 0)
				_Manager->BringToFront(this);
			//if(_Parent == 0)
			//	return;

			//_Parent->Controls()->Remove(this);
			//_Parent->Controls()->Add(this);
		}

		//! Send the control to the bottom of all controls
		void IControl::SendToBack()
		{
			if(_Manager != 0)
				_Manager->SendToBack(this);
			//if(_Parent == 0)
			//	return;

			//_Parent->Controls()->Remove(this);
			//_Parent->Controls()->Add(this, true);
		}

	}
}