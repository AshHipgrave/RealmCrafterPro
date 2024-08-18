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

#include "CGUIManager.h"
#include <ArrayList.h>
#include <Vector2.h>
#include <string>
#include <XMLWrapper.h>

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CPropertySet
		{
			struct SPropertyState
			{
				std::string Name;

				Math::Vector2 Location, Size;
				std::string Text, Skin, UpImage, DownImage, HoverImage;
				bool Visible, Enabled, UseBorder;
				TextAlign Align, VAlign;
				Math::Color ForeColor, BackColor, SelectionForeColor, SelectionBackColor;

				bool UseLocation, RelativeLocationX, RelativeLocationY, CenterLocationX, CenterLocationY, UseSize, RelativeSizeX, RelativeSizeY, UseText,
					UseSkin, UseUpImage, UseDownImage, UseHoverImage, UseVisible, UseEnabled,
					UseUseBorder, UseAlign, UseVAlign, UseForeColor, UseBackColor, UseSelectionForeColor, UseSelectionBackColor;

				// Radar Settings
				Math::Vector2 vViewRadius, vRadarImageTop, vRadarImageSize;
				std::string borderImage;
				bool UseBorderImage, UseViewRadius, UseRadarImageTop, UseRadarImageSize;
			
				
				SPropertyState()
				{
					Visible = Enabled = UseBorder = false;
					Align = VAlign = TextAlign_Left;

					UseLocation = RelativeLocationX = RelativeLocationY = CenterLocationX = CenterLocationY = UseSize = UseText =
						UseSkin = UseUpImage = UseDownImage = UseHoverImage = UseVisible = UseEnabled = RelativeSizeX = RelativeSizeY = 
						UseUseBorder = UseAlign = UseVAlign = UseForeColor = UseBackColor = UseSelectionForeColor = UseSelectionBackColor = false;
					
					UseBorderImage = UseViewRadius = UseRadarImageTop = UseRadarImageSize = false;
				}
			};

			std::string Name;
			CGUIManager* Manager;
			ArrayList<SPropertyState*> ComponentStates;

			void RecursiveApplySet(IControl* Parent, std::string ControlName, int Depth, int &DepthAt);

		public:

			CPropertySet(std::string Name, CGUIManager* GUIManager);
			~CPropertySet();

			void Process(XMLReader* X);
			std::string GetName();
			void ApplySet(IControl* ManagerSet, std::string ControlName, int Depth);
			void ApplyControl(IControl* Control);
		};

	}
}
