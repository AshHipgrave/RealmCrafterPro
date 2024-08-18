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

#include "BlitzPlus.h"

struct BBCommandData
{
	typedef float (tCosfn)(float angle);
	typedef float (tSinfn)(float angle);
	typedef unsigned int (tMilliSecsfn)();
	typedef void (tMoveMousefn)(int x, int y);
	typedef void (tFlushKeysfn)();
	typedef void (tFlushMousefn)();
	typedef void (tFlushJoyfn)();
	typedef int (tMouseXfn)();
	typedef int (tMouseYfn)();
	typedef int (tMouseXSpeedfn)();
	typedef int (tMouseYSpeedfn)();
	typedef int (tMouseZSpeedfn)();
	typedef void (tSetVibrationfn)(float leftMotor, float rightMotor);
	typedef void (tRuntimeErrorfn)(std::string message);
	typedef int (tRandfn)(int from, int to);
	typedef FILE* (tReadFilefn)(std::string filename);
	typedef FILE* (tWriteFilefn)(std::string filename);
	typedef FILE* (tOpenFilefn)(std::string filename);
	typedef void (tCloseFilefn)(FILE* f);
	typedef unsigned char (tReadBytefn)(FILE* f);
	typedef unsigned short (tReadShortfn)(FILE* f);
	typedef unsigned int (tReadIntfn)(FILE* f);
	typedef float (tReadFloatfn)(FILE* f);
	typedef std::string (tReadStringfn)(FILE* f);
	typedef void (tWriteBytefn)(FILE* f, unsigned char B);
	typedef void (tWriteShortfn)(FILE* f, unsigned short B);
	typedef void (tWriteIntfn)(FILE* f, unsigned int B);
	typedef void (tWriteFloatfn)(FILE* f, float B);
	typedef void (tWriteStringfn)(FILE* f, std::string B);
	typedef unsigned int (tFilePosfn)(FILE* f);
	typedef void (tSeekFilefn)(FILE* f, unsigned int pos);
	typedef unsigned int (tFileSizefn)(std::string filename);
	typedef bool (tEoffn)(FILE* f);
	typedef std::string (tReadLinefn)(FILE* f);
	typedef void (tWriteLinefn)(FILE* f, std::string B);
	typedef unsigned int (tReadDirfn)(std::string directory);
	typedef std::string (tNextFilefn)(unsigned int dir);
	typedef void (tCloseDirfn)(unsigned int dir);
	typedef int (tFileTypefn)(std::string file);
	typedef bool (tDownloadFilefn)(std::string file, std::string saveTo);
	typedef void (tBBDeleteFilefn)(std::string file);

	tCosfn* Cos;
	tSinfn* Sin;
	tMilliSecsfn* MilliSecs;
	tMoveMousefn* MoveMouse;
	tFlushKeysfn* FlushKeys;
	tFlushMousefn* FlushMouse;
	tFlushJoyfn* FlushJoy;
	tMouseXfn* MouseX;
	tMouseYfn* MouseY;
	tMouseXSpeedfn* MouseXSpeed;
	tMouseYSpeedfn* MouseYSpeed;
	tMouseZSpeedfn* MouseZSpeed;
	tSetVibrationfn* SetVibration;
	tRuntimeErrorfn* RuntimeError;
	tRandfn* Rand;
	tReadFilefn* ReadFile;
	tWriteFilefn* WriteFile;
	tOpenFilefn* OpenFile;
	tCloseFilefn* CloseFile;
	tReadBytefn* ReadByte;
	tReadShortfn* ReadShort;
	tReadIntfn* ReadInt;
	tReadFloatfn* ReadFloat;
	tReadStringfn* ReadString;
	tWriteBytefn* WriteByte;
	tWriteShortfn* WriteShort;
	tWriteIntfn* WriteInt;
	tWriteFloatfn* WriteFloat;
	tWriteStringfn* WriteString;
	tFilePosfn* FilePos;
	tSeekFilefn* SeekFile;
	tFileSizefn* FileSize;
	tEoffn* Eof;
	tReadLinefn* ReadLine;
	tWriteLinefn* WriteLine;
	tReadDirfn* ReadDir;
	tNextFilefn* NextFile;
	tCloseDirfn* CloseDir;
	tFileTypefn* FileType;
	tDownloadFilefn* DownloadFile;
	tBBDeleteFilefn* BBDeleteFile;
};

namespace BlitzPlus
{

extern "C" __declspec(dllexport) void* GetBBCommandData()
{
	BBCommandData* Commands = new BBCommandData();

	Commands->Cos = &Cos;
	Commands->Sin = &Sin;
	Commands->MilliSecs = &MilliSecs;
	Commands->MoveMouse = &MoveMouse;
	Commands->FlushKeys = &FlushKeys;
	Commands->FlushMouse = &FlushMouse;
	Commands->FlushJoy = &FlushJoy;
	Commands->MouseX = &MouseX;
	Commands->MouseY = &MouseY;
	Commands->MouseXSpeed = &MouseXSpeed;
	Commands->MouseYSpeed = &MouseYSpeed;
	Commands->MouseZSpeed = &MouseZSpeed;
	Commands->SetVibration = &SetVibration;
	Commands->RuntimeError = &RuntimeError;
	Commands->Rand = &Rand;
	Commands->ReadFile = &ReadFile;
	Commands->WriteFile = &WriteFile;
	Commands->OpenFile = &OpenFile;
	Commands->CloseFile = &CloseFile;
	Commands->ReadByte = &ReadByte;
	Commands->ReadShort = &ReadShort;
	Commands->ReadInt = &ReadInt;
	Commands->ReadFloat = &ReadFloat;
	Commands->ReadString = &ReadString;
	Commands->WriteByte = &WriteByte;
	Commands->WriteShort = &WriteShort;
	Commands->WriteInt = &WriteInt;
	Commands->WriteFloat = &WriteFloat;
	Commands->WriteString = &WriteString;
	Commands->FilePos = &FilePos;
	Commands->SeekFile = &SeekFile;
	Commands->FileSize = &FileSize;
	Commands->Eof = &Eof;
	Commands->ReadLine = &ReadLine;
	Commands->WriteLine = &WriteLine;
	Commands->ReadDir = &ReadDir;
	Commands->NextFile = &NextFile;
	Commands->CloseDir = &CloseDir;
	Commands->FileType = &FileType;
	Commands->DownloadFile = &DownloadFile;
	Commands->BBDeleteFile = &BBDeleteFile;

	return Commands;
}

}
