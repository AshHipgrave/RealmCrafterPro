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

#include "CPictureBoxHost.h"

namespace RealmCrafter
{
	namespace SDK
	{
		CPictureBoxHost::CPictureBoxHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL)
		{

		}

		CPictureBoxHost::~CPictureBoxHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);
			Handle = NULL;
		}

		NGin::GUI::IControl* CPictureBoxHost::GetHandle()
		{
			return Handle;
		}

		bool CPictureBoxHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			if(name.compare("MITX") == 0)
			{
				Handle->MinTexCoord = reader->ReadVector2();
			}else if(name.compare("MATX") == 0)
			{
				Handle->MaxTexCoord = reader->ReadVector2();
			}else if(name.compare("IMAG") == 0)
			{
				std::string Path = reader->ReadString();
				Handle->SetImage(Path);
				printf("ImagePath: %s\n", Path.c_str());
			}else
			{
				return IControlHost::ReadProperty(reader, name);
			}

			return true;
		}

		void CPictureBoxHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CPictureBoxHost* Host = new CPictureBoxHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create PictureBox handle
			Host->Handle = manager->GetGUIManager()->CreatePictureBox("UserPictureBox", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(75.0f, 23.0f) / manager->GetGUIManager()->GetResolution());

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
