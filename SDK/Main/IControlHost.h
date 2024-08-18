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

#include <SDKMain.h>
#include "IControlHostManager.h"

namespace RealmCrafter
{
	namespace SDK
	{
		class IControlHostManager;

		//! Interface for custom control hosts
		class IControlHost
		{
		public:

			//! Default constructor, MUST pass a manager object and a hierarchy master.
			IControlHost(IControlHostManager* manager, IControlHost* master) : Manager(manager), Master(master), AllocID(-1),
				LocationFlags(0), SizeFlags(0) {}
			virtual ~IControlHost() {}

			//! Get the handle of the NGUI control within
			virtual NGin::GUI::IControl* GetHandle() = 0;

			//! Get master handle
			virtual IControlHost* GetMaster();

			//! Get the servers allocated ID of this control
			virtual int GetAllocID();

			//! Have the controlhost read a propertyset
			virtual bool ReadProperty(PacketReader* reader, std::string &name);

			//! Send an event (with empty args)
			virtual void SendEvent(std::string name);

			//! Reset control size and position on resolution change
			virtual void ResetSize();

		protected:

			char LocationFlags, SizeFlags;
			NGin::Math::Vector2 Location, Size;
			NGin::GUI::IControl* BaseHandle;
			IControlHostManager* Manager;
			int AllocID;
			IControlHost* Master;

		};
	}
}