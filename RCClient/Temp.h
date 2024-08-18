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
#pragma once

//#include <windows.h>
#include <crtdbg.h>
#ifdef _DEBUG
	//#define new new(_NORMAL_BLOCK, __FILE__, __LINE__)
	#undef THIS_FILE
	static char THIS_FILE[] = __FILE__;
#endif

#include <NGinString.h>
#include <List.h>
#include <string>
#include <sstream>
#include <algorithm>
#include <locale>
#include <SDKMain.h>

	// Haxes!
#ifdef CreateWindow
#undef CreateWindow
#endif

#ifdef CreateDialog
#undef CreateDialog
#endif

#ifdef LoadCursor
#undef LoadCursor
#endif

namespace std
{

}

using namespace std;


void DUMPSTRING(string& Str);
void DUMPSTRING(const char* CStr, int Length);





#if !defined(DLLIN)
#define DLLIN extern "C" _declspec(dllimport)
#endif

#pragma comment(lib, "RCEnet.lib")


// Maximum client ID - decrease to speed up host but limit max players (do not set higher than 65535)
#define RCE_MaxClientID  5000

// RCE_Connect errors
#define RCE_PortInUse        -1
#define ConnectionNotFound     -2
#define RCE_TimedOut         -3
#define RCE_ServerFull       -4
#define RCE_ConnectionInUse  -5

// Message destinations
#define RCE_UDPReply  -1
#define RCE_All       5001

#define RCE_Diff	   6

// Internal packet types
#define RCE_NewClient     0
#define RCE_KeepAlive     1
#define RCE_Confirmation  2
#define RCE_NewGroup      3
#define RCE_KillGroup     4
#define RCE_ClientGone    5
#define RCE_GroupRename   6
#define RCE_ChangeGroup   7
#define RCE_PlayerJoined  8
#define RCE_PlayerGone    9
#define RCE_GroupMsg      10
#define ConnectionGone      11

// Local message types for user
#define RCE_NewPlayer           100 // Host only, clients use RCE_PlayerJoinedGroup
#define RCE_PlayerTimedOut      101 // Also host only, clients only get RCE_PlayerHasLeft or RCE_PlayerLeftGroup
#define RCE_PlayerHasLeft       102
#define RCE_GroupCreated        103
#define RCE_GroupDeleted        104
#define RCE_Disconnected        105
#define RCE_GroupRenamed        106
#define RCE_ChangedGroup        107
#define RCE_PlayerLeftGroup     108
#define RCE_PlayerJoinedGroup   109
#define ConnectionHasLeft         110
#define RCE_PlayerKicked        111 // Host only

struct RCE_Message
{
	int Connection;
	int MessageType;
	NGin::CString MessageData;
	int FromID;

	ClassDecl(RCE_Message, RCE_MessageList, RCE_MessageDelete);
};

#include "Packets.h"

DLLIN int RCE_Connect(const char* Hostname, int HostPort, int MyPort, const char* MyName, const char* MyData, const char* LogFile, int Append);
DLLIN void RCE_Disconnect(int peerHandle);
DLLIN int RCE_FSend(int Destination, int MessageType, const char* MessageData, int ReliableFlag, int Length);
DLLIN void RCE_StartLog(int Connection, const char* LogFile, int Append);
DLLIN void RCE_StopLog(int Connection);
DLLIN int RCE_Update();
DLLIN void RCE_SetPlayerGroup(int Connection, int GroupIP, int PlayerID);

inline bool CRCE_Send(unsigned int Connection, int Destination, int MessageType, NGin::CString MessageData, bool Reliable = false, int PlayerFrom = 0, int MessageLength = 0, int ConfirmID = 0)
{
	return RCE_FSend(Destination, MessageType, MessageData.c_str(), Reliable ? 1 : 0, (MessageLength > 0) ? MessageLength : MessageData.Length());
}



DLLIN int  RCE_MoveToFirstMessage();
DLLIN int  RCE_AreMoreMessage();
DLLIN int  RCE_GetMessageConnection();
DLLIN int  RCE_GetMessageType();
DLLIN int  RCE_GetMessageData(char* Buffer);
DLLIN int  RCE_MessageLength();

void RCE_CreateMessages();



// Blitz Module
//inline void CentreGadget(unsigned int Gad){}



//inline unsigned int BBCreateTextArea(int X, int Y, int Width, int Height, unsigned int Group, int Style = 0){return 0;}
//inline void BBAddTextAreaText(unsigned int TextArea, string Data){}

//inline string LSet(string, int) {return "NULL";}


// Erm, do something with this, its useful. Maybe import Debugging?
inline void Warning(string Message)
{
	printf("Warning: %s\n", Message.c_str());
}




