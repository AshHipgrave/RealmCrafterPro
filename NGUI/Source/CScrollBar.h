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

#include <IScrollBar.h>
#include "CGUIManager.h"

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CScrollBar : public IScrollBar
		{
		protected:

			CGUIManager* Manager;
			IGUIMeshBuffer* MeshBuffer;
			int _LargeChange, _SmallChange, _Minimum, _Maximum, _Value;
			int UpState, DownState, GripState;
			bool Dragging;
			float DragOffset;
			ScrollEventHandler* ScrollEvent;
			ScrollOrientation ScrollType;

			void RebuildMesh();

			virtual void OnSizeChange();
			virtual void OnSkinChange();
			virtual void OnValueChange();
			virtual void OnBackColorChange();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset();
			virtual bool Update(GUIUpdateParameters* Parameters);
			virtual void Render();

		public:

			CScrollBar(NGin::Math::Vector2& ScreenScale, ScrollOrientation ScrollOrientation, IGUIManager* Manager);
			virtual ~CScrollBar();
			virtual bool Initialize(CGUIManager* Manager);

			virtual int zzGet_LargeChange();
			virtual int zzGet_SmallChange();
			virtual int zzGet_Minimum();
			virtual int zzGet_Maximum();
			virtual int zzGet_Value();

			virtual void zzPut_LargeChange(int LargeChange);
			virtual void zzPut_SmallChange(int SmallChange);
			virtual void zzPut_Minimum(int Minimum);
			virtual void zzPut_Maximum(int Maximum);
			virtual void zzPut_Value(int Value);

			virtual ScrollEventHandler* Scroll();
		};
	}
}