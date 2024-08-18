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

#include <IListBox.h>
#include <ILabel.h>
#include "CGUIManager.h"

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CListBox : public IListBox
		{
		protected:

			struct CListBoxItem
			{
				ILabel* LabelHandle;
				bool Selected;
				int Index;

				CListBoxItem()
				{
					Selected = false;
					Index = 0;
				}

				~CListBoxItem()
				{
					LabelHandle = 0;
				}
			};
			ArrayList<CListBoxItem*> Items;

			CGUIManager* Manager;
			IGUIMeshBuffer* MeshBuffer;
			IScrollBar* VerticalScroll;
			EventHandler* SelectedIndexChangedEvent;
			int ScrollPosition;
			int _SelectedIndex;
			Color _SelectionBackColor, _SelectionForeColor;
			float AllowedSize;
			int VisibleItems;
			int ButtonState;

			void RebuildMesh();
			void CListBox::RecalculateLabels();

			virtual void OnEnabledChange();
			virtual void OnBackColorChange();
			virtual void OnForeColorChange();
			virtual void OnSkinChange();
			virtual void OnSizeChange();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset();
			virtual bool Update(GUIUpdateParameters* Parameters);
			virtual void Render();

		public:

			CListBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager);
			virtual ~CListBox();
			virtual bool Initialize(CGUIManager* Manager);

			virtual int AddItem(std::string Value);

			virtual int zzGet_SelectedIndex();
			virtual void zzPut_SelectedIndex(int SelectedIndex);
			
			virtual std::string zzGet_SelectedValue();
			virtual void zzPut_SelectedValue(std::string SelectedValue);
			
			virtual void zzPut_ItemValue(int Index, std::string Value);
			virtual std::string zzGet_ItemValue(int Index);

			virtual void zzPut_SelectionForeColor(Math::Color Value);
			virtual Math::Color zzGet_SelectionForeColor();

			virtual void zzPut_SelectionBackColor(Math::Color Value);
			virtual Math::Color zzGet_SelectionBackColor();
			
			virtual void DeleteItem(int Index);
			virtual EventHandler* SelectedIndexChanged();
			virtual int ItemCount();

			// Static callback
			static void ListBoxScrollEventCallback(IControl* Sender, ScrollEventArgs* E);
		};
	}
}