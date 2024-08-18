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

#include "CControlHostManager.h"
#include <windows.h>

void DebugLog(const char* Msg)
{
	OutputDebugStringA(Msg);
}

namespace RealmCrafter
{
	namespace SDK
	{
		CControlHostManager::CControlHostManager(NGin::GUI::IGUIManager* guiManager)
			: GUIManager(guiManager)
		{

		}

		CControlHostManager::~CControlHostManager()
		{

		}

		IControlHostManager::ControlHostFactory* CControlHostManager::RegisterTemplateFactory(NGin::Type type)
		{
			// Find and return type if it exists
			if(ControlTemplates.find(type) != ControlTemplates.end())
				return ControlTemplates[type];
			
			// Create type
			ControlHostFactory* HostFactory = new ControlHostFactory();
			ControlTemplates[type] = HostFactory;

			return HostFactory;
		}

		// Reset control sizes after resolution change
		void CControlHostManager::ResetControlSizes()
		{
			for(std::list<IControlHost*>::iterator It = ControlHosts.begin(); It != ControlHosts.end(); ++It)
			{
				(*It)->ResetSize();
			}
		}

		// Readout a 'global location' from a given control
		// This is used by the Form Designer in GE to locate a control onscreen
		void CControlHostManager::ReadoutAbsoluteLocation(char* data, int length)
		{
			PacketReader reader(data, length, false);
			PacketWriter writer;
			int AllocID = reader.ReadInt32();
			writer.Write(AllocID);

			for(std::list<IControlHost*>::iterator It = ControlHosts.begin(); It != ControlHosts.end(); ++It)
			{
				if((*It)->GetAllocID() == AllocID)
				{
					NGin::Math::Vector2 Location = (*It)->GetHandle()->GlobalLocation();
					writer.Write(Location);

					if(writer.GetLength() == length)
						memcpy(data, writer.GetBytes(), length);
					else
						OutputDebugStringA("Couldn't copy ReadOut, SrcLength != DestLength\n");

					return;
				}
			}
		}

		void CControlHostManager::ProcessFormOpen(PacketReader &packet)
		{
			CreateControlTreeFromPacket(NULL, NULL, packet);
		}

		void CControlHostManager::CreateControlTreeFromPacket(IControlHost* master, IControlHost* parent, PacketReader &reader)
		{
			// Read Type ID
			int GuidInt = reader.ReadInt32();
			NGin::Type CreateType(reinterpret_cast<const char*>(&GuidInt), "");

			// See if type exists in host DB (error out if it doesn't)
			std::map<NGin::Type, ControlHostFactory*>::iterator HostIt = ControlTemplates.find(CreateType);
			if(HostIt == ControlTemplates.end())
			{
				// Not a fatal issue... the GUI won't appear correctly but we can skip over broken controls
				OutputDebugStringA((std::string("SDKError: Couldn't understand type: ") + CreateType.GuidString() + "\n").c_str());
				return;
			}

			ControlFactoryEventArgs Args;
			Args.Reader = &reader;
			Args.Master = master;
			Args.Parent = parent;

			// It exists, so create it
			(*HostIt).second->Execute(this, &Args);
		}

		void CControlHostManager::RegisterControlHost(IControlHost* host)
		{
			ControlHosts.push_back(host);
		}

		// 'FormClose' packet received
		void CControlHostManager::ProcessFormClosed(PacketReader &packet)
		{
			int AllocID = packet.ReadInt32();

			IControlHost* Master = NULL;
			for(std::list<IControlHost*>::iterator It = ControlHosts.begin(); It != ControlHosts.end(); ++It)
			{
				if((*It)->GetAllocID() == AllocID)
				{
					Master = *It;
					break;
				}
			}

			if(Master != NULL)
			{
				std::list<IControlHost*> Removals;
				std::list<IControlHost*>::iterator It = ControlHosts.begin();
				while(It != ControlHosts.end())
				{
					if((*It) != Master && (*It)->GetMaster() == Master)
					{
						Removals.push_back(*It);
						++It;
						//ControlHosts.erase(It++);
					}else
						++It;
				}

				Removals.push_back(Master);

				for(std::list<IControlHost*>::iterator It = Removals.begin(); It != Removals.end(); ++It)
				{
					ControlHosts.remove(*It);
					delete *It;
				}

				
			}
		}

		NGin::GUI::IGUIManager* CControlHostManager::GetGUIManager()
		{
			return GUIManager;
		}

		// Client wants to sent packet to server
		void CControlHostManager::PostEventPacket(PacketWriter &pa)
		{
			EventsPacket.Write(pa);
		}

		// Pop event data
		void CControlHostManager::PopEventPacket(PacketWriter &pa)
		{
			pa.Write(EventsPacket);
			EventsPacket.Clear();
		}

		void CControlHostManager::ProcessUpdateProperties(PacketReader &packet)
		{
			while(packet.GetLocation() < packet.GetLength() - 1)
			{
				int AllocID = packet.ReadInt32();
				std::string PropertyName = packet.ReadString(4);

				bool Found = false;
				for(std::list<IControlHost*>::iterator It = ControlHosts.begin(); It != ControlHosts.end(); ++It)
				{
					if((*It)->GetAllocID() == AllocID)
					{
						// Return on failure since the remaining packet will be invalid
						if(!(*It)->ReadProperty(&packet, PropertyName))
							return;

						Found = true;
						break;
					}
				}

				// Not found? Return here, since the packet is corrupt
				if(!Found)
				{
					return;
				}
			}
		}
	}
}