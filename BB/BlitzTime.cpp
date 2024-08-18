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
#include "BlitzPlus.h"
#include <windows.h>

namespace BlitzPlus
{

	std::string MonthList[12] = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

	std::string CurrentDate()
	{
		SYSTEMTIME Time;
		GetLocalTime(&Time);

		char Str[24];sprintf(Str, "%i", Time.wDay);
		std::string Day = Str;
		if(Time.wDay < 10)
			Day.append("0", true);

		char Out[128];
		sprintf("%s %s %i", Day.c_str(), MonthList[Time.wMonth - 1], Time.wYear);
		return Out;
	}

	std::string CurrentTime()
	{
		SYSTEMTIME Time;
		GetLocalTime(&Time);

		char Str[24];sprintf(Str, "%i", Time.wHour);
		std::string Hour = Str;
		sprintf(Str, "%i", Time.wMinute);
		std::string Minute = Str;
		sprintf(Str, "%i", Time.wSecond);
		std::string Second = Str;

		if(Time.wHour < 10)
			Hour.append("0", true);
		if(Time.wMinute < 10)
			Minute.append("0", true);
		if(Time.wSecond < 10)
			Second.append("0", true);

		return Hour + ":" + Minute + ":" + Second;

	}

	uint MilliSecs()
	{
		return GetTickCount();
		//return clock();
	}

	void Delay(int TimeMS)
	{
		int Start = GetTickCount();
		while(Start + TimeMS > GetTickCount());
	}
}