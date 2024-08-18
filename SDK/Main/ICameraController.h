//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#pragma once

#include "SDKMain.h"

namespace RealmCrafter
{
	// Definitions
	namespace SDK
	{
		class IActorInstance;
	}

	// Camera Controller Interface
	class ICameraController
	{
	public:

		ICameraController() {}
		virtual ~ICameraController() {}

		// Move the camera or, or around, the given position
		// Note: This function will be called when a zone is loaded as the actor will
		//   move.
		virtual void ResetCameraPosition(float x, float y, float z) = 0;

		// Sets the engines keyboard/mouse/joystick layout class.
		// Note: When this function is called before the main menu is displayed. You
		//   must specify the required controls for this class during the call in order
		//   to allow the user to modify them.
		virtual void SetControlLayout(IControlLayout* layout) = 0;

		// Returns true if the controller moved the player during the last frame updated
		// This prevents some code execution if the actor is idle.
		virtual bool MovedThisFrame() = 0;

		// Update the given actor instanced based around this controller and the camera.
		// Note: Controllers will only ever use the main player character, though ChangeActor()
		//   and related commands can technically alter the given pointer. You can base
		//   assumptions around your knowledge of the game in question.
		virtual void Update(uint camera, SDK::IActorInstance* player) = 0;
	};
}