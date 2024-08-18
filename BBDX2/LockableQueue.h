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

#include <list>
#include "Common\JaredsUtils.h"

// Lockable queue class
template<typename T> class LockableQueue
{
protected:

	std::list<T> LinkedList;
	CRITICAL_SECTION CriticalSection;

public:

	typedef bool (PredicateTest)(T &test, void* arg);

	LockableQueue()
	{
		InitializeCriticalSection(&CriticalSection);
	}

	~LockableQueue()
	{
		DeleteCriticalSection(&CriticalSection);
	}

	void Push(T item)
	{
		EnterCriticalSection(&CriticalSection);

		LinkedList.push_back(item);

		LeaveCriticalSection(&CriticalSection);
	}

	unsigned int GetSize()
	{
		unsigned int Size = 0;

		EnterCriticalSection(&CriticalSection);

		Size = LinkedList.size();

		LeaveCriticalSection(&CriticalSection);

		return Size;
	}

	void Remove(PredicateTest* predicate, void* args, std::list<T> &outDeletion)
	{
		EnterCriticalSection(&CriticalSection);

		for(std::list<T>::iterator It = LinkedList.begin(); It != LinkedList.end(); ++It)
		{
			T Val = *It;
			if(predicate(Val, args))
				outDeletion.push_back(Val);
		}

		for(std::list<T>::iterator It = outDeletion.begin(); It != outDeletion.end(); ++It)
		{
			LinkedList.remove(*It);
		}

		LeaveCriticalSection(&CriticalSection);
	}

	T Pop()
	{
		T Out = NULL;

		EnterCriticalSection(&CriticalSection);

		if(LinkedList.size() > 0)
		{
			Out = *(LinkedList.begin());
			LinkedList.pop_front();
		}

		LeaveCriticalSection(&CriticalSection);

		return Out;
	}
};
