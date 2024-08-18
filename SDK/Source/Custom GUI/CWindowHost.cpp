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

#include "CWindowHost.h"

void DebugLog(const char* Msg);

namespace RealmCrafter
{
	namespace SDK
	{
		CWindowHost::CWindowHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL)
		{
			
		}

		CWindowHost::~CWindowHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);
			Handle = NULL;
		}

		NGin::GUI::IControl* CWindowHost::GetHandle()
		{
			return Handle;
		}

		bool CWindowHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			if(name.compare("CBTN") == 0)
			{
				Handle->CloseButton = reader->ReadByte() > 0;
			}else if(name.compare("MODL") == 0)
			{
				Handle->Modal = reader->ReadByte() > 0;
			}else
			{
				return IControlHost::ReadProperty(reader, name);
			}

			return true;
		}

		void CWindowHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CWindowHost* Host = new CWindowHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create window handle
			Host->Handle = manager->GetGUIManager()->CreateWindow("UserWindow", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(0.3f, 0.3f));
			Host->Handle->Closed()->AddEvent(Host, &CWindowHost::Handle_Closed);

			// Parenting
			if(e->Parent != NULL)
			{
				NGin::GUI::IControl* ParentHandle = e->Parent->GetHandle();
				if(ParentHandle != NULL)
					Host->Handle->Parent = ParentHandle;
			}

			// Set 'basehandle'.. it exists just to prevent our need to cast all the time.
			Host->BaseHandle = Host->Handle;

			// Read default properties
			while(e->Reader->GetLocation() < e->Reader->GetLength() - 1)
			{
				std::string Name = e->Reader->ReadString(4);

				if(!Host->ReadProperty(e->Reader, Name))
					break;
			}
		}

		void CWindowHost::Handle_Closed(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			SendEvent("CLOS");
		}
	}
}
