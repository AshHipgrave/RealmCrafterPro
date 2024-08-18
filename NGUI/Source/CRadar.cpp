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
#include "CRadar.h"

namespace NGin
{
	namespace GUI
	{
		Type IRadar::TypeOf()	{	return Type("CRAD", "CRadar GUI Radar");	}
		CRadar::CRadar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IRadar(ScreenScale, Manager)
		{
			_Type = Type("CRAD", "CRadar GUI Radar");
			vViewRadius = Math::Vector2(200, 200);
			vRadarImageTop = Math::Vector2(0, 0);
			vRadarImageSize = Math::Vector2(1000, 1000);
		}

		CRadar::~CRadar()	{}

		bool CRadar::SetImage( std::string RadarImage, std::string RadarBorder )
		{
			bool result=true;
			if ( !RadarImage.empty() ) result = pRadar->SetImage(RadarImage) & result;
 			if ( !RadarBorder.empty()) result = pRadarBorder->SetImage(RadarBorder) & result;

			return result;
		}

		void CRadar::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;
			pRadar = Manager->CreatePictureBox(Name, Location, Size);
 			pRadarBorder = Manager->CreatePictureBox(Name+"_Border", Location, Size);
		}

		Math::Vector2 CRadar::zzGet_ViewRadius()							{	return vViewRadius;	}
		Math::Vector2 CRadar::zzGet_RadarImageTop()							{	return vRadarImageTop;	}
		Math::Vector2 CRadar::zzGet_RadarImageSize()						{	return vRadarImageSize;	}
		Math::Vector2 CRadar::zzGet_MinTexCoord()							{	return pRadar->MinTexCoord;	}
		Math::Vector2 CRadar::zzGet_MaxTexCoord()							{	return pRadar->MaxTexCoord;	}

		void CRadar::zzPut_MinTexCoord(Math::Vector2 Coord)					{	pRadar->MinTexCoord = Coord; }
		void CRadar::zzPut_MaxTexCoord(Math::Vector2 Coord)					{	pRadar->MaxTexCoord = Coord; }
		void CRadar::zzPut_ViewRadius( Math::Vector2 viewRadius )			{	vViewRadius = viewRadius;	}
		void CRadar::zzPut_RadarImageTop( Math::Vector2 radarImageTop )		{	vRadarImageTop	= radarImageTop;	}
		void CRadar::zzPut_RadarImageSize( Math::Vector2 radarImageSize )	{	vRadarImageSize	= radarImageSize;	}

		void CRadar::zzPut_Location( NGin::Math::Vector2 Location )
		{
			pRadar->Location = Location; pRadarBorder->Location = Location;
			IControl::zzPut_Location(Location);
		}

		void CRadar::zzPut_Size( NGin::Math::Vector2 Size )
		{
			pRadar->Size = Size; pRadarBorder->Size = Size;
			IControl::zzPut_Size(Size);
		}

		void CRadar::zzPut_Visible( bool Visible )
		{
			pRadar->Visible = Visible; pRadarBorder->Visible = Visible;
			IControl::zzPut_Visible(Visible);
		}

		void CRadar::zzPut_BackColor( NGin::Math::Color BackColor )
		{
			pRadar->BackColor = BackColor; /*pRadarBorder->BackColor = BackColor;*/
			IControl::zzPut_BackColor(BackColor);
		}

		void CRadar::zzPut_ForeColor( NGin::Math::Color ForeColor )
		{
			pRadar->ForeColor = ForeColor; /*pRadarBorder->ForeColor = ForeColor;*/
			IControl::zzPut_ForeColor(ForeColor);
		}

		bool CRadar::Update( GUIUpdateParameters* Parameters )
		{
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				if(Parameters->MouseThumb == 0) Parameters->MouseThumb = this;
				Manager->ControlFocus(this);
			}

			return false;
		}


	}
}
