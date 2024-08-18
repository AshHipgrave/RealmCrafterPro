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

#include <IEventHandler.h>
#include "ITimerManager.h"

namespace NGin
{
	// Definitions
	class ElapsedEventArgs;
	class ITimer;
	typedef IEventHandler<ITimer, ElapsedEventArgs> ElapsedEventHandler;

	//! Timer Interface Class
	class ITimer
	{
	public:

		ITimer(){};
		virtual ~ITimer(){};

		//! Starts raising the Elapsed event
		/*!
		If AutoReset is set to false, the ITimer raises the Elapsed event only one.
		*/
		virtual void Start() = 0;

		//! Stops raising the Elapsed event
		virtual void Stop() = 0;

		//! Gets the AutoReset value
		virtual bool AutoReset() = 0;

		//! Sets the AutoReset value
		/*! When set to true, the timer will tick continuously
		*/
		virtual void AutoReset(bool Reset) = 0;

		//! Gets the Enabled value
		virtual bool Enabled() = 0;

		//! Sets the Enabled value
		/*! Setting to true calls Start(), false calls Stop()
		*/
		virtual void Enabled(bool IsEnabled) = 0;

		//! Gets the tick interval value in milliseconds
		virtual int Interval() = 0;

		//! Sets the tick interval value in milliseconds
		virtual void Interval(int NewInterval) = 0;

		//! Gets the ElapsedEventHandler to add callbacks to the timer
		virtual ElapsedEventHandler* Elapsed() = 0;
	};

	//! ElapsedEventArgs structure
	class ElapsedEventArgs
	{
	public:

		//! The time, in milliseconds, which the callback was called
		int SignalTime;
	};
}
