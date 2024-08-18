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
#include "CTimer.h"
#include "CTimerManager.h"

namespace NGin
{

CTimer::CTimer(ITimerManager* Mgr)
{
	// Defaults
	_Manager = (CTimerManager*)Mgr;
	_LastUpdate = _Interval = 0;
	_Enabled = _AutoReset = _Ticked = false;
	_Event = new ElapsedEventHandler();

	// Register
	_Manager->RegisterTimer(this);
}

CTimer::CTimer(ITimerManager* Mgr, int Interval)
{
	// Defaults
	_Manager = (CTimerManager*)Mgr;
	_LastUpdate;
	_Enabled = _AutoReset = _Ticked = false;
	_Event = new ElapsedEventHandler();
	_Interval = Interval;

	// Register
	_Manager->RegisterTimer(this);
}

CTimer::~CTimer()
{
	// Delete
	delete _Event;

	// UnRegister
	_Manager->UnRegisterTimer(this);
}


void CTimer::Update()
{
	// Timer isn't active
	if(!_Enabled)
		return;

	// Timer can only tick once
	if(!_AutoReset && _Ticked)
		return;

	int TimeDifference = _Manager->Now() - _LastUpdate;

	// Update
	if(TimeDifference > _Interval)
	{
		// Log and reset
		_Ticked = true;
		_LastUpdate = _Manager->Now();
		
		// Setup the callback
		ElapsedEventArgs* Args = new ElapsedEventArgs();
		Args->SignalTime = _LastUpdate;

		// Call
		_Event->Execute(this, Args);

		// Free up resource
		delete Args;
	}
}

void CTimer::Start()
{
	_Enabled = true;
	_Ticked = false;
	_LastUpdate = _Manager->Now();
}

void CTimer::Stop()
{
	_Enabled = false;
}


#pragma region Properties

bool CTimer::AutoReset()
{
	return _AutoReset;
}

void CTimer::AutoReset(bool Reset)
{
	_AutoReset = Reset;
}


bool CTimer::Enabled()
{
	return _Enabled;
}

void CTimer::Enabled(bool IsEnabled)
{
	if(IsEnabled)
		Start();
	else
		Stop();
}

int CTimer::Interval()
{
	return _Interval;
}

void CTimer::Interval(int NewInterval)
{
	_Interval = NewInterval;
}


ElapsedEventHandler* CTimer::Elapsed()
{
	return _Event;
}


#pragma endregion

}