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

// Includes
#include <vector>
#include <stdarg.h>

namespace NGin
{

	template <class Caller, class Args>
	//! IEventHandler class
	/* This template class is used for handling callbacks with events.
	Similar to C#; Caller is sending object; Args is the EventArguments class.
	Callback functions can be added (AddEvent) together to callback lists.
	*/
	class IEventHandler
	{
	public:
		typedef void (CBFN)(Caller* InCaller, Args* InArgs);

		// Default Constructor
		IEventHandler(){}

		//! Construction which accepts a default function
		IEventHandler(CBFN* Function)
		{
			EventList.push_back(Function);
		}

		// Destructor
		~IEventHandler(){}

		//! Execute the callback list
		void Execute(Caller* InCaller, Args* InArgs)
		{
			// Loop through every function and call it
			for(unsigned int i = 0; i < EventList.size(); ++i)
				if(EventList[i])
					EventList[i](InCaller, InArgs);
			for(unsigned int i = 0; i < EventListMethods.size(); ++i)
			{
				void* Arguments = (&InCaller);
				//int Arguments[2] = {(int)InCaller, (int)InArgs};
				unsigned int ArgSize = sizeof(Caller*) + sizeof(Args*);
				void* This = EventListMethods[i].This;
				void* Address = EventListMethods[i].Address;

				_asm
				{
					sub esp, 8
					push [InArgs]
					push [InCaller]
					mov ebx, [This]
					call [Address]
					add esp, 8
				}
			}
		}

		//! Add an event to the call list
		void AddEvent(CBFN* Function)
		{
			EventList.push_back(Function);
		}

		//! Add a method to the call list
		void AddEvent(void* This, ...)
		{
			void* Address;
			va_list ArgList;

			va_start(ArgList, This);
			Address = va_arg(ArgList, void*);
			va_end(ArgList);

			MethodType Mt;
			Mt.This = This;
			Mt.Address = Address;
			EventListMethods.push_back(Mt);
		}

		//! Clear events from this handler
		void ClearEvents()
		{
			EventList.clear();
			EventListMethods.clear();
		}

	private:

		struct MethodType
		{
			void* This;
			void* Address;
		};

		std::vector<CBFN*> EventList;
		std::vector<MethodType> EventListMethods;
	};
}

