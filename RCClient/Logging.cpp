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

#include "Logging.h"

// Struct setup
ClassDef(LogFile, LogFileList, LogFileDelete);


// Starts a log and returns the handle
FILE* StartLog(string LogName, bool Append)
{
	string LogFileName = "Data\\Logs\\";
	LogFileName.append(LogName);
	LogFileName.append(".txt");
	
	FILE* F = 0;

	if(Append == false || FileType(LogFileName) != 1)
		F = WriteFile(LogFileName);
	else
	{
		F = OpenFile(LogFileName);
		if(F != 0)
			SeekFile(F, FileSize(LogFileName));
	}

	if(F != 0)
	{
		LogFile* L = new LogFile();
		L->File = F;
	}

	return F;
}

void WriteLog(FILE* LogHandle, string Dat, bool Timestamp, bool Datestamp)
{
	string Data = Dat;

	if(Timestamp)
	{
		string Time = CurrentTime();
		Time = string("[") + Time;
		Time.append("]");
		Data = Time + Data;
	}

	if(Datestamp)
	{
		string Date = CurrentDate();
		Date = string("[") + Date;
		Date.append("]");
		Data = Date + Data;
	}

	WriteLine(LogHandle, Data);
}

void StopLog(FILE* LogHandle)
{
	foreachc(LIt, LogFile, LogFileList)
	{
		if((*LIt)->File == LogHandle)
		{
			LogFile::Delete(*LIt);
			break;
		}

		nextc(LIt, LogFile, LogFileList)
	}
	LogFile::Clean();

	CloseFile(LogHandle);
}

void CloseAllLogs()
{
	foreachc(LIt, LogFile, LogFileList)
	{

		CloseFile((*LIt)->File);
		LogFile::Delete(*LIt);

		nextc(LIt, LogFile, LogFileList)
	}
	LogFile::Clean();
	LogFile::LogFileList.Clear();
}
