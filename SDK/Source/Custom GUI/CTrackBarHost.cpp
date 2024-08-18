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

#include "CTrackBarHost.h"

namespace RealmCrafter
{
	namespace SDK
	{
		CTrackBarHost::CTrackBarHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL)
		{

		}

		CTrackBarHost::~CTrackBarHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);
			Handle = NULL;
		}

		NGin::GUI::IControl* CTrackBarHost::GetHandle()
		{
			return Handle;
		}

		bool CTrackBarHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			if(name.compare("VALU") == 0)
			{
				int Value = reader->ReadInt32();

				Handle->Value = Value;
			}else if(name.compare("MINM") == 0)
			{
				int Value = reader->ReadInt32();

				Handle->Minimum = Value;
			}else if(name.compare("MAXM") == 0)
			{
				int Value = reader->ReadInt32();

				Handle->Maximum = Value;
			}else if(name.compare("TCKF") == 0)
			{
				int Value = reader->ReadInt32();

				Handle->TickFrequency = Value;
			}else
			{
				return IControlHost::ReadProperty(reader, name);
			}

			return true;
		}

		void CTrackBarHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CTrackBarHost* Host = new CTrackBarHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create TrackBar handle
			Host->Handle = manager->GetGUIManager()->CreateTrackBar("UserTrackBar", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(75.0f, 23.0f) / manager->GetGUIManager()->GetResolution());
			Host->Handle->ValueChanged()->AddEvent(Host, &CTrackBarHost::Handle_ValueChanged);

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

		void CTrackBarHost::Handle_ValueChanged(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			PacketWriter Pa;
			Pa.Write(AllocID);
			Pa.Write(std::string("VALU"), false);
			Pa.Write(Handle->Value);

			Manager->PostEventPacket(Pa);
		}
	}
}
