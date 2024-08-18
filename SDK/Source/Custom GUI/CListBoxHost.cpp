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

#include "CListBoxHost.h"

namespace RealmCrafter
{
	namespace SDK
	{
		CListBoxHost::CListBoxHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL), SelectedIndex(0)
		{

		}

		CListBoxHost::~CListBoxHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);
			Handle = NULL;
		}

		NGin::GUI::IControl* CListBoxHost::GetHandle()
		{
			return Handle;
		}

		bool CListBoxHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			if(name.compare("ADIT") == 0)
			{
				int Count = reader->ReadInt32();

				for(int i = 0; i < Count; ++i)
					Handle->AddItem(reader->ReadString());

			}else if(name.compare("DLIT") == 0)
			{
				Handle->DeleteItem(reader->ReadInt32());
			}else if(name.compare("SLIN") == 0)
			{
				SelectedIndex = reader->ReadInt32();
				Handle->SelectedIndex = SelectedIndex;
			}else if(name.compare("SLVA") == 0)
			{
				Handle->SelectedValue = reader->ReadString();
			}else if(name.compare("ITVA") == 0)
			{
				Handle->ItemValue[reader->ReadInt32()] = reader->ReadString();
			}else if(name.compare("STIT") == 0)
			{
				Handle->SelectedIndex = 0;
				int Cnt = Handle->ItemCount();
				for(int i = 0; i < Cnt; ++i)
					Handle->DeleteItem(0);

				int Count = reader->ReadInt32();

				for(int i = 0; i < Count; ++i)
					Handle->AddItem(reader->ReadString());
				Handle->SelectedIndex = SelectedIndex;
			}else
			{
				return IControlHost::ReadProperty(reader, name);
			}

			return true;
		}

		void CListBoxHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CListBoxHost* Host = new CListBoxHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create ListBox handle
			Host->Handle = manager->GetGUIManager()->CreateListBox("UserListBox", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(75.0f, 23.0f) / manager->GetGUIManager()->GetResolution());
			Host->Handle->SelectedIndexChanged()->AddEvent(Host, &CListBoxHost::Handle_SelectedIndexChanged);

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

		void CListBoxHost::Handle_SelectedIndexChanged(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			PacketWriter Pa;
			Pa.Write(AllocID);
			Pa.Write(std::string("SLIN"), false);
			Pa.Write(Handle->SelectedIndex);

			Manager->PostEventPacket(Pa);
		}
	}
}
