//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
#include "StdAfx.h"
#include "ControlHost.h"
#include <SDKMain.h>

namespace SDKNet
{
		

	ControlHost::ControlHost(void)
	{
	}

	// 'FormOpen' packet received from the server which must be processed
	void ControlHost::ProcessFormOpen(cli::array<System::Byte>^ Packet)
	{
		System::IntPtr DPtr = System::Runtime::InteropServices::Marshal::AllocHGlobal(Packet->Length);

		System::Runtime::InteropServices::Marshal::Copy(Packet, 0, DPtr, Packet->Length);

		RealmCrafter::PacketReader Reader((char*)DPtr.ToPointer(), Packet->Length, false);

		RealmCrafter::Globals->ControlHostManager->ProcessFormOpen(Reader);

		System::Runtime::InteropServices::Marshal::FreeHGlobal(DPtr);
	}

	// 'UpdateProperties' packet received from the server.
	void ControlHost::ProcessUpdateProperties(cli::array<System::Byte>^ Packet)
	{
		unsigned char* Data = new unsigned char[Packet->Length];
		for(int i = 0; i < Packet->Length; ++i)
			Data[i] = (unsigned char)Packet[i];

		RealmCrafter::PacketReader Reader((char*)Data, Packet->Length, true);

		RealmCrafter::Globals->ControlHostManager->ProcessUpdateProperties(Reader);

		delete[] Data;
	}

	// 'FormClose' packet received
	void ControlHost::ProcessFormClosed(cli::array<System::Byte>^ Packet)
	{
		unsigned char* Data = new unsigned char[Packet->Length];
		for(int i = 0; i < Packet->Length; ++i)
			Data[i] = (unsigned char)Packet[i];

		RealmCrafter::PacketReader Reader((char*)Data, Packet->Length, true);

		RealmCrafter::Globals->ControlHostManager->ProcessFormClosed(Reader);

		delete[] Data;
	}

	// ReadoutAbsoluteLocation
	cli::array<System::Byte>^ ControlHost::ReadoutAbsoluteLocation(cli::array<System::Byte>^ Packet)
	{
		char* Data = new char[Packet->Length];
		for(int i = 0; i < Packet->Length; ++i)
			Data[i] = (char)Packet[i];

		RealmCrafter::Globals->ControlHostManager->ReadoutAbsoluteLocation(Data, Packet->Length);

		for(int i = 0; i < Packet->Length; ++i)
			Packet[i] = (System::Byte)Data[i];

		return Packet;
	}
}