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

#include "CSelectionSizeHost.h"

namespace RealmCrafter
{
	namespace SDK
	{
		CSelectionSizeHost::CSelectionSizeHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL)
		{

		}

		CSelectionSizeHost::~CSelectionSizeHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);
			Handle = NULL;
		}

		NGin::GUI::IControl* CSelectionSizeHost::GetHandle()
		{
			return Handle;
		}

		bool CSelectionSizeHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			return IControlHost::ReadProperty(reader, name);

			return true;
		}

		void CSelectionSizeHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CSelectionSizeHost* Host = new CSelectionSizeHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create Button handle
			Host->Handle = new NGin::GUI::CSelectionSize(NGin::Math::Vector2(1.0f, 1.0f) / manager->GetGUIManager()->GetResolution(), manager->GetGUIManager());
			Host->Handle->Initialize();
			Host->Handle->Locked = true;
			Host->Handle->Name = "UserControl";
			Host->Handle->Text = "UserControl";
			Host->Handle->Location = NGin::Math::Vector2(0.1f, 0.1f);
			

			Host->Handle->Locked = false;
			Host->Handle->Parent = manager->GetGUIManager();

			Host->Handle->Click()->AddEvent(Host, &CSelectionSizeHost::Handle_Click);
			Host->Handle->MouseDown()->AddEvent(Host, &CSelectionSizeHost::Handle_MouseDown);

			Host->Handle->Size = NGin::Math::Vector2(0.2f, 0.2f);

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

		void CSelectionSizeHost::Handle_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			SendEvent("LCLK");
		}

		void CSelectionSizeHost::Handle_MouseDown(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			SendEvent("MDWN");
		}
	}
}
