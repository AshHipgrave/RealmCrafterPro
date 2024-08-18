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

#include "CLabelHost.h"

namespace RealmCrafter
{
	namespace SDK
	{
		CLabelHost::CLabelHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL)
		{

		}

		CLabelHost::~CLabelHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);
			Handle = NULL;
		}

		NGin::GUI::IControl* CLabelHost::GetHandle()
		{
			return Handle;
		}

		bool CLabelHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			if(name.compare("HALN") == 0)
			{
				char Align = reader->ReadByte();

				if(Align >= 0 && Align < 3)
					Handle->Align = (NGin::GUI::TextAlign)Align;
				else
					Handle->Align = NGin::GUI::TextAlign_Left;
			}else if(name.compare("VALN") == 0)
			{
				char Align = reader->ReadByte();

				if(Align >= 0 && Align < 3)
					Handle->VAlign = (NGin::GUI::TextAlign)Align;
				else
					Handle->VAlign = NGin::GUI::TextAlign_Left;
			}else if(name.compare("FLAG") == 0)
			{
				unsigned char Flags = reader->ReadByte();

				Handle->Multiline = (Flags & 1) > 0;
				Handle->InlineStringProcessing = (Flags & 2) > 0;
				Handle->ForceScissoring = (Flags & 4) > 0;
			}else if(name.compare("ZZML") == 0)
			{
				Handle->Multiline = reader->ReadByte() > 0;
			}else if(name.compare("ZZSP") == 0)
			{
				Handle->InlineStringProcessing = reader->ReadByte() > 0;
			}else if(name.compare("ZZFS") == 0)
			{
				Handle->ForceScissoring = reader->ReadByte() > 0;

			}else if(name.compare("SCWN") == 0)
			{
				Handle->ScissorWindow = reader->ReadVector2();
			}else if(name.compare("SCOF") == 0)
			{
				Handle->ScrollOffset = reader->ReadVector2();
			}else
			{
				return IControlHost::ReadProperty(reader, name);
			}

			return true;
		}

		void CLabelHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CLabelHost* Host = new CLabelHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create Label handle
			Host->Handle = manager->GetGUIManager()->CreateLabel("UserLabel", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(75.0f, 23.0f) / manager->GetGUIManager()->GetResolution());

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
