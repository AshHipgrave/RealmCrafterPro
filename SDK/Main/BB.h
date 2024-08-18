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

#include <string>

#ifdef RC_SDK

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
extern BBCommandData* BBCommands;



inline float Cos(float angle)
{
    return BBCommands->Cos( angle);
}

inline float Sin(float angle)
{
    return BBCommands->Sin( angle);
}

inline unsigned int MilliSecs()
{
    return BBCommands->MilliSecs();
}

inline void MoveMouse(int x, int y)
{
    return BBCommands->MoveMouse( x,  y);
}

inline void FlushKeys()
{
    return BBCommands->FlushKeys();
}

inline void FlushMouse()
{
    return BBCommands->FlushMouse();
}

inline void FlushJoy()
{
    return BBCommands->FlushJoy();
}

inline int MouseX()
{
    return BBCommands->MouseX();
}

inline int MouseY()
{
    return BBCommands->MouseY();
}

inline int MouseXSpeed()
{
    return BBCommands->MouseXSpeed();
}

inline int MouseYSpeed()
{
    return BBCommands->MouseYSpeed();
}

inline int MouseZSpeed()
{
    return BBCommands->MouseZSpeed();
}

inline void SetVibration(float leftMotor, float rightMotor)
{
    return BBCommands->SetVibration( leftMotor,  rightMotor);
}

inline void RuntimeError(std::string message)
{
	return BBCommands->RuntimeError(message);
}

inline int Rand(int from, int to = RAND_MAX)
{
	return BBCommands->Rand(from, to);
}

inline FILE* ReadFile(std::string filename)
{
	return BBCommands->ReadFile(filename);
}

inline FILE* WriteFile(std::string filename)
{
	return BBCommands->WriteFile(filename);
}

inline FILE* OpenFile(std::string filename)
{
	return BBCommands->OpenFile(filename);
}

inline void CloseFile(FILE* f)
{
	return BBCommands->CloseFile(f);
}

inline unsigned char ReadByte(FILE* f)
{
	return BBCommands->ReadByte(f);
}

inline unsigned short ReadShort(FILE* f)
{
	return BBCommands->ReadShort(f);
}

inline unsigned int ReadInt(FILE* f)
{
	return BBCommands->ReadInt(f);
}

inline float ReadFloat(FILE* f)
{
	return BBCommands->ReadFloat(f);
}

inline std::string ReadString(FILE* f)
{
	return BBCommands->ReadString(f);
}

inline void WriteByte(FILE* f, unsigned char B)
{
	return BBCommands->WriteByte(f, B);
}

inline void WriteShort(FILE* f, unsigned short B)
{
	return BBCommands->WriteShort(f, B);
}

inline void WriteInt(FILE* f, unsigned int B)
{
	return BBCommands->WriteInt(f, B);
}

inline void WriteFloat(FILE* f, float B)
{
	return BBCommands->WriteFloat(f, B);
}

inline void WriteString(FILE* f, std::string B)
{
	return BBCommands->WriteString(f, B);
}

inline unsigned int FilePos(FILE* f)
{
	return BBCommands->FilePos(f);
}

inline void SeekFile(FILE* f, unsigned int pos)
{
	return BBCommands->SeekFile(f, pos);
}

inline unsigned int FileSize(std::string filename)
{
	return BBCommands->FileSize(filename);
}

inline bool Eof(FILE* f)
{
	return BBCommands->Eof(f);
}

inline std::string ReadLine(FILE* f)
{
	return BBCommands->ReadLine(f);
}

inline void WriteLine(FILE* f, std::string B)
{
	return BBCommands->WriteLine(f, B);
}

inline unsigned int ReadDir(std::string directory)
{
	return BBCommands->ReadDir(directory);
}

inline std::string NextFile(unsigned int dir)
{
	return BBCommands->NextFile(dir);
}

inline void CloseDir(uint dir)
{
	return BBCommands->CloseDir(dir);
}

inline int FileType(std::string file)
{
	return BBCommands->FileType(file);
}

inline bool DownloadFile(std::string file, std::string saveTo)
{
	return BBCommands->DownloadFile(file, saveTo);
}

inline void BBDeleteFile(std::string file)
{
	BBCommands->BBDeleteFile(file);
}


#endif


