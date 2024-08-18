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
//******************************************************
//* NGUI Radar Interface                               *
//* Aut: Yeisnier Domínguez Silva (yeisnier@gmail.com) *
//* Notes: None                                        *
//******************************************************
#pragma once

#include "IControl.h"

namespace NGin
{
	namespace GUI
	{
		//! Radar Interface class
		/*!
		TODO: Link
		*/
		class IRadar : public IControl
		{
		public:

			IRadar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
			virtual ~IRadar() {}

			virtual bool SetImage(std::string RadarImage="", std::string RadarBorder="" ) = 0;

			//! Minimum texture coordinate
			__declspec(property(get=zzGet_MinTexCoord, put=zzPut_MinTexCoord)) Math::Vector2 MinTexCoord;
			virtual void zzPut_MinTexCoord(Math::Vector2 Coord) = 0;
			virtual Math::Vector2 zzGet_MinTexCoord() = 0;

			//! Maximum texture coordinate
			__declspec(property(get=zzGet_MaxTexCoord, put=zzPut_MaxTexCoord)) Math::Vector2 MaxTexCoord;
			virtual void zzPut_MaxTexCoord(Math::Vector2 Coord) = 0;
			virtual Math::Vector2 zzGet_MaxTexCoord() = 0;

			//! View Radius
			__declspec(property(get=zzGet_ViewRadius, put=zzPut_ViewRadius)) Math::Vector2 ViewRadius;
			virtual void zzPut_ViewRadius(Math::Vector2 viewRadius) = 0;
			virtual Math::Vector2 zzGet_ViewRadius() = 0;

			//! Radar Image Top
			__declspec(property(get=zzGet_RadarImageTop, put=zzPut_RadarImageTop)) Math::Vector2 RadarImageTop;
			virtual void zzPut_RadarImageTop(Math::Vector2 RadarImageTop) = 0;
			virtual Math::Vector2 zzGet_RadarImageTop() = 0;

			//! Radar Image Size
			__declspec(property(get=zzGet_RadarImageSize, put=zzPut_RadarImageSize)) Math::Vector2 RadarImageSize;
			virtual void zzPut_RadarImageSize(Math::Vector2 RadarImageSize) = 0;
			virtual Math::Vector2 zzGet_RadarImageSize() = 0;

			//! Returns the IRadar Type
			/*!
			TODO: Types
			*/
			static NGin::Type TypeOf();
		};
	}
}