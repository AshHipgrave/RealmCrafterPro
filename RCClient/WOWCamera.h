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

#include "Actors.h"
#include <vector3.h>

class ActorInstance;

// Implements a combined Third/First Person Camera
class WOWCamera
{
protected:

	ActorInstance* Player;
	uint Head, LastEN;

	float Pitch;
	float Yaw;
	float Zoom;

	float YawSpeed;
	float PitchSpeed;
	float ZoomSpeed;

	

	float InternalYaw;

	float HeadX, HeadY, HeadZ;

	float CameraDestX, CameraDestY, CameraDestZ;
	float CameraDestPitch, CameraDestYaw;

	int InvertAxis1;
	int InvertAxis3;

	bool Moved;

	float Lerp(float a, float b, float t);

public:

	float FCamX, FCamY, FCamZ;

	WOWCamera();

	void SetActorInstance(ActorInstance* player);
	ActorInstance* GetActorInstance();
	bool MovedThisFrame();

	void Update(uint camera);
};