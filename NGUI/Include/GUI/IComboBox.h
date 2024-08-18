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

#include "IControl.h"

namespace NGin
{
	namespace GUI
	{
		//! ComboBox Interface class
		/*!
		TODO: ComboBox Link
		*/
		class IComboBox : public IControl
		{
		public:

			IComboBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IComboBox() {}

			//! Selected item
			__declspec(property(get=zzGet_SelectedIndex, put=zzPut_SelectedIndex)) int SelectedIndex;
			virtual void zzPut_SelectedIndex(int SelectedIndex) = 0;
			virtual int zzGet_SelectedIndex() = 0;

			//! Value of the selected index
			__declspec(property(get=zzGet_SelectedValue, put=zzPut_SelectedValue)) std::string SelectedValue;
			virtual void zzPut_SelectedValue(std::string SelectedValue) = 0;
			virtual std::string zzGet_SelectedValue() = 0;

			//! Value of an item
			__declspec(property(get=zzGet_ItemValue, put=zzPut_ItemValue)) std::string ItemValue[];
			virtual void zzPut_ItemValue(int Index, std::string Value) = 0;
			virtual std::string zzGet_ItemValue(int Index) = 0;

			//! Add an item to the listbox
			/*!
			\param Value WString that will appear in the listbox
			\return Index of item
			*/
			virtual int AddItem(std::string Value) = 0;

			//! Delete an item
			virtual void DeleteItem(int Index) = 0;

			//! Get the number of items in the box
			virtual int ItemCount() = 0;

			//! EventHandler for selectedindexchanged event
			/*!
			SelectedIndexChanged event is called when a user selects an item
			*/
			virtual EventHandler* SelectedIndexChanged() = 0;

			//! Returns the IComboBox Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};
	}
}