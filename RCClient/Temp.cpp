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
#include "Temp.h"
#include "Items.h"

namespace RealmCrafter
{
	// Resolve sectorvector
	const float SectorVector::SectorSize = 768.0f;
}

void DUMPSTRING(const char* CStr, int Len)
{

	printf("+--+--+--+--+--+--+--+--+  +--+--+--+--+--+--+--+--+  +--------+ +--------+\n");

	
	char Later[16];
	for(int i = 0; i < 16; ++i)
		Later[i] = 0;

	int l = 0;
	for(int i = 0; i < Len; ++i)
	{
		if(CStr[i] < 16)
			printf("|0");
		else
			printf("|");
		printf("%i", /*(const unsigned char)*/CStr[i]);
		Later[l] = CStr[i];
		++l;

		if(l == 8)
			printf("|  ");

		if(l == 16)
		{
			printf("|  |");
			
			for(int f = 0; f < 16; ++f)
			{
				if(Later[f] < 32)
					printf("X");
				else
					printf("%c", Later[f]);

				if(f == 7)
					printf("| |");
			}
			printf("|\n");
			l = 0;
		}
	}
	
	for(; l < 16; ++l)
	{
		printf("|  ");
		Later[l] = '.';

		if(l == 8)
			printf("|  ");

		if(l == 15)
		{
			printf("|  |");
			
			for(int f = 0; f < 16; ++f)
			{
				if(Later[f] < 32)
					printf("X");
				else
					printf("%c", Later[f]);

				if(f == 7)
					printf("| |");
			}
			printf("|\n");
		}

	}

	printf("+--+--+--+--+--+--+--+--+  +--+--+--+--+--+--+--+--+  +--------+ +--------+\n");
}


// Dump a string into a memory table
void DUMPSTRING(string& Str)
{
	const char* CStr = Str.c_str();
	int Len = Str.length();
	
	DUMPSTRING(CStr, Len);

}

//ClassDef(RCE_Message, RCE_MessageList, RCE_MessageDelete);

//#define ClassDef(ClassName, ClassList, ClassDelete) 

RCE_Message::RCE_Message()
{
	ClearMemory(RCE_Message);
	RCE_MessageList.Add(this);
}

RCE_Message::~RCE_Message()
{
	RCE_MessageList.Remove(this);
}

List<RCE_Message*> RCE_Message::RCE_MessageList;
List<RCE_Message*> RCE_Message::RCE_MessageDelete;

void RCE_Message::Delete(RCE_Message* Item)
{	
	foreachc(dLIt, RCE_Message, RCE_MessageDelete)
	{
		if(*dLIt == Item)
			return;
		nextc(dLIt, RCE_Message, RCE_MessageDelete);
	}
	RCE_MessageDelete.Add(Item);
}

void RCE_Message::Clean()
{	
	foreachc(dLIt, RCE_Message, RCE_MessageDelete)
	{		/*ClassName::ClassList.Remove(*dLIt);*/ 
		delete *dLIt;		
		nextc(dLIt, RCE_Message, RCE_MessageDelete)	
	}	
	
	RCE_Message::RCE_MessageDelete.Clear();
}



void RCE_CreateMessages()
{
	if(RCE_MoveToFirstMessage() != 0)
	{
		do
		{
			RCE_Message* M = new RCE_Message();
			M->Connection = RCE_GetMessageConnection();
			M->FromID = M->Connection;
			M->MessageType = RCE_GetMessageType();

			int Length = RCE_MessageLength();
			//printf("Recv: %i-", Length);
			if(Length > 0)
			{
				char* MsgBank = (char*)malloc(Length);
				RCE_GetMessageData(MsgBank);
				//char a, b;
				//a = MsgBank[334];
				//b = MsgBank[336];
				//printf("%s   %x %x\n", MsgBank, (int)a, (int)b);

				M->MessageData.Set(MsgBank, Length);

				free(MsgBank);
			}

			//if(Length == 798)
			//{
				//printf("%s", M->MessageData.Substr(334, 3));
			//	printf("%i", ShortFromStr(M->MessageData.Substr(1, 2).c_str()));
			//}
			//printf("\n");
			
			//RCE_DeleteListMsg();
		}while(RCE_AreMoreMessage() != 0);
	}
}