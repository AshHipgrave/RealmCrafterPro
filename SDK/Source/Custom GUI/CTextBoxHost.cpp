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

#include "CTextBoxHost.h"

namespace RealmCrafter
{
	namespace SDK
	{
		CTextBoxHost::CTextBoxHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL)
		{
			if(Globals->TimerManager != NULL)
			{
				// This timer will periodically change for text changes and update the server
				TextChangedTimer = Globals->TimerManager->CreateTimer(1000);
				TextChangedTimer->AutoReset(true);
				TextChangedTimer->Elapsed()->AddEvent(this, &CTextBoxHost::TextChangedTimer_Elapsed);
				TextChangedTimer->Start();
			}else
			{
				TextChangedTimer = NULL;
			}
		}

		CTextBoxHost::~CTextBoxHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);

			if(TextChangedTimer != NULL)
			{
				TextChangedTimer->Stop();
				delete TextChangedTimer;
			}

			TextChangedTimer = NULL;
			Handle = NULL;
		}

		NGin::GUI::IControl* CTextBoxHost::GetHandle()
		{
			return Handle;
		}

		bool CTextBoxHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			if(name.compare("PACH") == 0)
			{
				Handle->PasswordCharacter = reader->ReadString();
			}else if(name.compare("VAEX") == 0)
			{
				Handle->ValidationExpression = reader->ReadString();
			}else if(name.compare("TEXT") == 0)
			{
				Handle->Text = reader->ReadString();
				LastText = Handle->Text;

			}else
			{
				return IControlHost::ReadProperty(reader, name);
			}

			return true;
		}

		void CTextBoxHost::TextChangedTimer_Elapsed(NGin::ITimer* timer, NGin::ElapsedEventArgs* e)
		{
			if(Handle->Text.compare(LastText) != 0)
			{
				LastText = Handle->Text;

				PacketWriter Pa;
				Pa.Write(AllocID);
				Pa.Write(std::string("TXTC"), false);
				Pa.Write(LastText, true);

				Manager->PostEventPacket(Pa);
			}
		}

		void CTextBoxHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CTextBoxHost* Host = new CTextBoxHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create TextBox handle
			Host->Handle = manager->GetGUIManager()->CreateTextBox("UserTextBox", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(75.0f, 23.0f) / manager->GetGUIManager()->GetResolution());



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
	}
}
