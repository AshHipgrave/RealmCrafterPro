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

#include "Temp.h"
#include <NGinString.h>
#include <List.h>
#include <BlitzPlus.h>

#include "RottParticles.h"


#include "Default Project.h"

#define W_Sun   0
#define W_Rain  1
#define W_Snow  2
#define W_Fog   3
#define W_Storm 4
#define W_Wind  5

class CEnvironmentManager
{
protected:

	int Year, Day, TimeH, TimeM, TimeFactor;
	int TimeUpdate; 

public:

	// Constructor
	CEnvironmentManager();

	// Move to manager, not SDK
	int GetTimeH();
	int GetTimeM();
	int GetTimeS();

	// Set the current time
	// Move to SDK AND Manager
	void SetTime(int timeH, int timeM);

	// Set the current Day/Year
	// Move to SDK AND MAnager
	void SetDate(int day, int year);

	// Rebuild the environment settings using a server update
	// Consider removal?
	void ResetEnvironment(NGin::CString& networkPacket);
	
	// Change the weather
	// Move SDK.. consider overall removal?
	//void SetWeather(int weather);

	// Load environment media
	void LoadArea(std::string zonename, std::string environmentName);

	// Update the environment
	void Update(float deltaTime);

	void SetCameraUnderWater(bool under, unsigned char destR, unsigned char destG, unsigned char destB);

	bool PlayWetFootstep();	
};

// Removes all particles from a given emitter which are beneath a water area or a catch plane
void RemoveUnderwaterParticles(int E);

extern CEnvironmentManager* Environment;

