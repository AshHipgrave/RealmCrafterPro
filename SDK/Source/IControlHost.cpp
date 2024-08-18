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
#include <windows.h>

namespace RealmCrafter
{
	namespace SDK
	{
		// Get servers allocated ID
		int IControlHost::GetAllocID()
		{
			return AllocID;
		}

		// Send event with no arguments
		void IControlHost::SendEvent(std::string name)
		{
			PacketWriter Pa;
			Pa.Write(AllocID);
			Pa.Write(name, false);

			Manager->PostEventPacket(Pa);
		}

		// Get master handle
		IControlHost* IControlHost::GetMaster()
		{
			return Master;
		}

		// Reset control size and position on resolution change
		void IControlHost::ResetSize()
		{
			if(SizeFlags == 0)
				BaseHandle->Size = Size / Manager->GetGUIManager()->GetResolution();
			else if(SizeFlags == 1)
				BaseHandle->Size = Size;
		}

		// Read a property from a packet (this implementation is generally called last).
		bool IControlHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// If its an 'end of definition' then have the property reader stop.
			if(name.compare("ENDL") == 0)
			{
				NGin::Math::Vector2 SetLocation = Location;

				if(Location.X < 0.0f)
					SetLocation.X += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().X : 1.0f;
				if(Location.Y < 0.0f)
					SetLocation.Y += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().Y : 1.0f;

				if(LocationFlags == 0)
					BaseHandle->Location = SetLocation / Manager->GetGUIManager()->GetResolution();
				else if(LocationFlags == 1)
					BaseHandle->Location = SetLocation;
				else if(LocationFlags == 2)
					BaseHandle->Location = SetLocation - (BaseHandle->Size * NGin::Math::Vector2(0.5f, 0.5f));

				return false;
			}
				

			// Read in various properties
			if(name.compare("ALID") == 0)
			{
				AllocID = reader->ReadInt32();
			}else if(name.compare("LCTN") == 0)
			{
				LocationFlags = reader->ReadByte();
				Location = reader->ReadVector2();
				NGin::Math::Vector2 SetLocation = Location;

				if(Location.X < 0.0f)
					SetLocation.X += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().X : 1.0f;
				if(Location.Y < 0.0f)
					SetLocation.Y += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().Y : 1.0f;

				if(LocationFlags == 0)
					BaseHandle->Location = SetLocation / Manager->GetGUIManager()->GetResolution();
				else if(LocationFlags == 1)
					BaseHandle->Location = SetLocation;
				else if(LocationFlags == 2)
					BaseHandle->Location = SetLocation - (BaseHandle->Size * NGin::Math::Vector2(0.5f, 0.5f));
			}else if(name.compare("ZZLC") == 0)
			{
				Location = reader->ReadVector2();
				NGin::Math::Vector2 SetLocation = Location;

				if(Location.X < 0.0f)
					SetLocation.X += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().X : 1.0f;
				if(Location.Y < 0.0f)
					SetLocation.Y += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().Y : 1.0f;

				if(LocationFlags == 0)
					BaseHandle->Location = SetLocation / Manager->GetGUIManager()->GetResolution();
				else if(LocationFlags == 1)
					BaseHandle->Location = SetLocation;
				else if(LocationFlags == 2)
					BaseHandle->Location = SetLocation - (BaseHandle->Size * NGin::Math::Vector2(0.5f, 0.5f));
			}else if(name.compare("ZZPT") == 0)
			{
				LocationFlags = reader->ReadByte();

				NGin::Math::Vector2 SetLocation = Location;

				if(Location.X < 0.0f)
					SetLocation.X += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().X : 1.0f;
				if(Location.Y < 0.0f)
					SetLocation.Y += (LocationFlags == 0) ? Manager->GetGUIManager()->GetResolution().Y : 1.0f;

				if(LocationFlags == 0)
					BaseHandle->Location = SetLocation / Manager->GetGUIManager()->GetResolution();
				else if(LocationFlags == 1)
					BaseHandle->Location = SetLocation;
				else if(LocationFlags == 2)
					BaseHandle->Location = SetLocation - (BaseHandle->Size * NGin::Math::Vector2(0.5f, 0.5f));

			}else if(name.compare("SIZE") == 0)
			{
				SizeFlags = reader->ReadByte();
				Size = reader->ReadVector2();

				if(SizeFlags == 0)
					BaseHandle->Size = Size / Manager->GetGUIManager()->GetResolution();
				else if(SizeFlags == 1)
					BaseHandle->Size = Size;
			}else if(name.compare("ZZSZ") == 0)
			{
				Size = reader->ReadVector2();

				if(SizeFlags == 0)
					BaseHandle->Size = Size / Manager->GetGUIManager()->GetResolution();
				else if(SizeFlags == 1)
					BaseHandle->Size = Size;
			}else if(name.compare("ZZST") == 0)
			{
				SizeFlags = reader->ReadByte();

				if(SizeFlags == 0)
					BaseHandle->Size = Size / Manager->GetGUIManager()->GetResolution();
				else if(SizeFlags == 1)
					BaseHandle->Size = Size;

			}else if(name.compare("ENBL") == 0)
			{
				BaseHandle->Enabled = reader->ReadByte() > 0;
			}else if(name.compare("VSBL") == 0)
			{
				BaseHandle->Visible = reader->ReadByte() > 0;
			}else if(name.compare("NAME") == 0)
			{
				BaseHandle->Name = reader->ReadString();
			}else if(name.compare("TEXT") == 0)
			{
				BaseHandle->Text = reader->ReadString();
			}else if(name.compare("BCOL") == 0)
			{
				BaseHandle->BackColor = reader->ReadColor();
			}else if(name.compare("FCOL") == 0)
			{
				BaseHandle->ForeColor = reader->ReadColor();
			}else if(name.compare("SKIN") == 0)
			{
				BaseHandle->Skin = reader->ReadInt32();
			}else if(name.compare("CHLD") == 0)
			{
				int ChildCount = reader->ReadUInt16();

				for(int i = 0; i < ChildCount; ++i)
				{
					// Copy subcontrol to whole new packet.. this way missing properties get a little protection
					unsigned int PacketLength = reader->ReadUInt32();
					char* PacketData = new char[PacketLength];
					reader->Read(PacketData, PacketLength);
					PacketReader NewReader(PacketData, PacketLength, false); // Note: COPY flag is false... we have to clean up

					Manager->CreateControlTreeFromPacket(Master, this, NewReader);

					delete[] PacketData;
				}
			}else
			{
				OutputDebugStringA((std::string("SDKError: Unknown PropertyString in IControlHost: ") + name + "\n").c_str());
				return false;
			}

			return true;
		}
	}
}