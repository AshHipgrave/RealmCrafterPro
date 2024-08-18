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

#include <IGUIManager.h>

namespace RealmCrafter
{
	struct SSpells
	{
		unsigned short Spells[1000];
		unsigned short Levels[1000];

		unsigned short MemSpells[10];
		unsigned short MemLevels[10];

		SSpells()
		{
			for(int i = 0; i < 1000; ++i)
				Spells[i] = 65535;
			for(int i = 0; i < 10; ++i)
				MemSpells[i] = 65535;

			memset(Levels, 0, sizeof(Levels));
			memset(MemLevels, 0, sizeof(MemLevels));
		}
	};

	class SpellEventArgs
	{
		int Index;
		int Page;
		int Offset;

	public:

		SpellEventArgs(int index, int page, int offset)
			: Index(index), Page(page), Offset(offset)
		{
		}

		int GetIndex()
		{
			return Index;
		}

		int GetPage()
		{
			return Page;
		}

		int GetOffset()
		{
			return Offset;
		}
	};

	class IPlayerSpells;
	typedef NGin::IEventHandler<IPlayerSpells, SpellEventArgs> SpellEvent;

	class IPlayerSpells
	{
	public:

		virtual ~IPlayerSpells() { };

		virtual void Initialize() = 0;
		virtual void Update(SSpells* playerSpells, NGin::Math::Vector2 &mousePosition) = 0;

		virtual void SetVisible(bool visible) = 0;
		virtual bool GetVisible() = 0;
		virtual bool IsActiveWindow() = 0;
		virtual void BringToFront() = 0;

		virtual NGin::GUI::EventHandler* Closed() = 0;
		virtual SpellEvent* Click() = 0;
		virtual SpellEvent* RightClick() = 0;
	};

	extern "C" __declspec(dllexport) IPlayerSpells* CreatePlayerSpells(NGin::GUI::IGUIManager* guiManager);
}