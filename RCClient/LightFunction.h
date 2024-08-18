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
#include <xmlwrapper.h>

#include "Temp.h"
#include <color.h>
#include <Blitzplus.h>

namespace RealmCrafter
{

	class LightFunction
	{
	public:

		class Event
		{
		public:

			class TriggerTime
			{
			public:

				int TimeH, TimeM, TimeMS;
				bool UseGameTime;

				TriggerTime() : TimeH(0), TimeM(0), TimeMS(1), UseGameTime(false) {}
			};

			TriggerTime Time;
			bool Interpolate;
			NGin::Math::Color Color;
			int Radius;

			Event() : Interpolate(false), Radius(0) {}

			Event(const Event &other)
			{
				Time.TimeH = other.Time.TimeH;
				Time.TimeM = other.Time.TimeM;
				Time.TimeMS = other.Time.TimeMS;
				Time.UseGameTime = other.Time.UseGameTime;
				Interpolate = other.Interpolate;
				Color.R = other.Color.R;
				Color.G = other.Color.G;
				Color.B = other.Color.B;
				Radius = other.Radius;
			}
		};

		std::vector<Event*> Events;
		int LastTick, CurrentEvent, LastTimeM, LastTimeH;
		int LastEventTimeH, LastEventTimeM, LastEventTimeS, LastEventTimeMS;
		bool WaitTick;
		std::string Name;

		NGin::Math::Color CurrentColor;
		int CurrentRadius;

		LightFunction();
		~LightFunction();
		
		void ClearEvents();

		void Update(int timeH, int timeM, int timeS);
		void Compile(NGin::XMLReader* X);
	};
}
