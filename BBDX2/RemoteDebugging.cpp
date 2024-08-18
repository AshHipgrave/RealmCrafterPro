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
#include "Common\JaredsUtils.h"
#include <iostream>
#include <winsock.h>
#include <string>
#include <list>
#include "BBDX\DebugFrameProfiler.h"

SOCKET DebuggingClient;
bool DebuggingOpen = false;

extern "C" __declspec(dllexport) int StartRemoteDebugging(const char* address, int port)
{
	WSADATA WSAData;
	WORD Version = MAKEWORD(2, 0);
	int Error = 0;

	if(WSAStartup(Version, &WSAData) != 0)
	{
		OutputDebugStringA("StartRemoveDebugging Failed: WinSock2 is an incorrect version.\n");
		return 0;
	}

	DebuggingClient = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	SOCKADDR_IN SockAddr;
	memset(&SockAddr, 0, sizeof(SOCKADDR_IN));

	HOSTENT* HostAddr = gethostbyname(address);

	SockAddr.sin_family = AF_INET;
	SockAddr.sin_port = htons(port);
	SockAddr.sin_addr.s_addr = ((IN_ADDR*)HostAddr->h_addr)->s_addr;

	if(connect(DebuggingClient, (const sockaddr*)&SockAddr, sizeof(SOCKADDR_IN)) == SOCKET_ERROR)
	{
		OutputDebugStringA("StartRemoteDebugging Failed: Could not connect to host.\n");
		return 0;
	}

	//int Val = 1;
	//setsockopt(DebuggingClient, IPPROTO_TCP, TCP_NODELAY, (const char*)&Val, 4);

	char NameBuffer[1021];
	GetModuleFileNameA(NULL, NameBuffer, 1021);

	std::string NameStr = "Module: ";
	NameStr += NameBuffer;
	int Pos = NameStr.find_last_of('\\');
	if(Pos != std::string::npos)
		NameStr = std::string("Module: ") + NameStr.substr(Pos + 1);

	char SendBuffer[1024];

	memcpy(SendBuffer + 3, NameStr.c_str(), NameStr.size());
	SendBuffer[0] = 3;
	SendBuffer[1] = 0;
	SendBuffer[2] = NameStr.size();

	send(DebuggingClient, SendBuffer, NameStr.size() + 3, 0);

	NameStr = "Version: ";
	NameStr += __J_MODULEVERSION;

	memcpy(SendBuffer + 3, NameStr.c_str(), NameStr.size());
	SendBuffer[0] = 3;
	SendBuffer[1] = 1;
	SendBuffer[2] = NameStr.size();

	send(DebuggingClient, SendBuffer, NameStr.size() + 3, 0);

	DebuggingOpen = true;
	return 1;
}

extern "C" __declspec(dllexport) int DebuggerConnected()
{
	if(DebuggingOpen)
		return 1;
	return 0;
}

extern "C" __declspec(dllexport) void StaticDebugStr(int id, const char* data)
{
	if(!DebuggingOpen)
		return;

	int Len = strlen(data);

	char SendBuffer[1024];
	SendBuffer[0] = 3;
	SendBuffer[1] = id;
	SendBuffer[2] = Len;
	memcpy(SendBuffer + 3, data, Len);

	send(DebuggingClient, SendBuffer, Len + 3, 0);
}

struct TimerEvent
{
	std::string Name;
	int Id;
	unsigned int StartTime;
};

int AllocatedTimerIDs = 0;
std::list<TimerEvent*> Events;
bool FrameOpen = false;
std::string TPrefix = "";

extern "C" __declspec(dllexport) void SetDebugPrefix(const char* prefix)
{
	TPrefix = prefix;
}



extern "C" __declspec(dllexport) void StartDebugTimer(const char* name)
{
	FP_PushTask( name );
	return;

	// JB: Deprecated, using HUD profiler
/*
	if(!DebuggingOpen)
		return 0;

	if(!FrameOpen)
		return 0;

	TimerEvent* E = new TimerEvent();
	E->Name = TPrefix + name;
	E->Id = ++AllocatedTimerIDs;
	E->StartTime = timeGetTime();

	Events.push_back(E);

	return E->Id;
*/
}

extern "C" __declspec(dllexport) void StopDebugTimer(/*int id*/)
{
	FP_PopTask();

	/*
	if(!DebuggingOpen)
		return;

	if(!FrameOpen)
		return;

	for(std::list<TimerEvent*>::reverse_iterator It = Events.rbegin(); It != Events.rend(); ++It)
	{
		if((*It)->Id == id)
		{
			int Len = (int)(timeGetTime() - (*It)->StartTime);

			int PacketLength = (*It)->Name.size() + 4 + 2;

			char* Packet = new char[PacketLength];
			Packet[0] = 4;
			((int*)(Packet + 1))[0] = Len;
			Packet[5] = (*It)->Name.size();
			memcpy(Packet + 6, (*It)->Name.c_str(), Packet[5]);

			send(DebuggingClient, Packet, PacketLength, 0);

			delete[] Packet;

			return;
		}
	}
	*/
}

extern "C" __declspec(dllexport) void DebugActor(int id, unsigned short sx, unsigned short sz, float x, float z, char me)
{
	if(!DebuggingOpen)
		return;

	if(!FrameOpen)
		return;

	char* Packet = new char[18];
	Packet[0] = 10;
	memcpy(Packet+1, &id, 4);
	memcpy(Packet+5, &sx, 2);
	memcpy(Packet+7, &sz, 2);
	memcpy(Packet+9, &x, 4);
	memcpy(Packet+13, &z, 4);
	Packet[17] = me;
	
	send(DebuggingClient, Packet, 18, 0);

	delete[] Packet;
}

extern "C" __declspec(dllexport) void BeginDebugFrame()
{
	if(!DebuggingOpen)
		return;

	char Pkt = 5;
	send(DebuggingClient, &Pkt, 1, 0);

	FrameOpen = true;
}

extern "C" __declspec(dllexport) void EndDebugFrame()
{
	FP_FrameSync();
	return;

	if(!DebuggingOpen)
		return;

	for(std::list<TimerEvent*>::reverse_iterator It = Events.rbegin(); It != Events.rend(); ++It)
	{
		delete (*It);
	}
	Events.clear();
	AllocatedTimerIDs = 0;

	char Pkt = 6;
	send(DebuggingClient, &Pkt, 1, 0);

	FrameOpen = false;
}