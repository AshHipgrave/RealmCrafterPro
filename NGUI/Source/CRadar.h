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

#include <IRadar.h>
#include "CGUIManager.h"

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CRadar : public IRadar
		{
		protected:
			CGUIManager* Manager;
			Math::Vector2 vViewRadius, vRadarImageSize, vRadarImageTop;

			virtual bool Update(GUIUpdateParameters* Parameters);

		public:
			IPictureBox *pRadar, *pRadarBorder;
			CRadar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager);
			virtual ~CRadar();

			virtual void Initialize(CGUIManager* Manager);
			virtual bool SetImage(std::string RadarImage="", std::string RadarBorder="" ) override;

			virtual Math::Vector2 zzGet_MinTexCoord();
			virtual Math::Vector2 zzGet_MaxTexCoord();
			virtual Math::Vector2 zzGet_ViewRadius();
			virtual Math::Vector2 zzGet_RadarImageTop();
			virtual Math::Vector2 zzGet_RadarImageSize();

			virtual void zzPut_MinTexCoord(Math::Vector2 Coord);
			virtual void zzPut_MaxTexCoord(Math::Vector2 Coord);
			virtual void zzPut_ViewRadius(Math::Vector2 viewRadius);
			virtual void zzPut_RadarImageTop(Math::Vector2 radarImageTop);
			virtual void zzPut_RadarImageSize(Math::Vector2 radarImageSize);

			// override from IControl
			virtual void zzPut_Location(NGin::Math::Vector2 Location);
			virtual void zzPut_Size(NGin::Math::Vector2 Size);
			virtual void zzPut_Visible(bool Visible);
			virtual void zzPut_BackColor(NGin::Math::Color BackColor);
			virtual void zzPut_ForeColor(NGin::Math::Color ForeColor);
		};
	}
}