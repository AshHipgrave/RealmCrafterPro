// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#include "CReadMemory.h"
//#include "Encryption.h"
#include <stdio.h>
#include <windows.h>
#include <string.h>
#include <iostream>
#include <fstream>

//E_Enc BF_encrypt;
E_Start BF_Start = 0;
E_Enc BF_encrypt = 0;
E_Dnc BF_decrypt = 0;
bool EncryptionLoaded = false;

void CheckLibrary()
{
	if(EncryptionLoaded == false)
	{
		EncryptionLoaded = true;
		HINSTANCE eDC = LoadLibrary("rc64.dll");
		if(!eDC)
		{
			MessageBox(NULL, "Could not load \"rc64.dll\" - Please check it exists","ERROR", MB_ICONERROR | MB_OK);
			exit(0);
			return;
		}

		BF_Start = (E_Start)GetProcAddress(eDC,"BF_Start");
		BF_encrypt = (E_Enc)GetProcAddress(eDC,"BF_encrypt");
		BF_decrypt = (E_Dnc)GetProcAddress(eDC,"BF_decrypt");
		if(BF_Start == 0){MessageBox(NULL,"Could not load function: BF_Start in rc64.dll","Library Module",MB_OK | MB_ICONERROR);exit(0);}
		if(BF_encrypt == 0){MessageBox(NULL,"Could not load function: BF_encrypt in rc64.dll","Library Module",MB_OK | MB_ICONERROR);exit(0);}
		if(BF_decrypt == 0){MessageBox(NULL,"Could not load function: BF_decrypt in rc64.dll","Library Module",MB_OK | MB_ICONERROR);exit(0);}

		BF_Start(5378);
	}
}

namespace irr
{
namespace io
{

	CReadMemory::CReadMemory(char* buffer, unsigned int length, const char* path, bool encrypted)
	{
		Filename = path;
		FileSize = length;
		Position = 0;
		memory = buffer;
		PushedBuffer = true;

		if(!encrypted)
			return;

		CheckLibrary();
		
		int SizeLeft;
		for (int Offset = 0; Offset < length; Offset += 64)
		{
			SizeLeft = length - Offset;
			if (SizeLeft < 64)
			{
				BF_decrypt(memory + Offset, SizeLeft);
			}
			else
			{
				BF_decrypt(memory + Offset, 64);
			}
		}
	}

CReadMemory::CReadMemory(const c8* fileName)
: FileSize(0)
{
	#ifdef _DEBUG
	setDebugName("CReadMemory");
	#endif

	PushedBuffer = false;
	
	CheckLibrary();

	Filename = fileName;
	FileSize = 0;
	std::fstream file;
	file.open(Filename.c_str(),std::ios::in|std::ios::binary);

	if(!file.is_open() || !file.good())
	{
		return;
	}

	file.seekg (0, std::ios::end);
	int length = file.tellg();
	file.seekg (0, std::ios::beg);
	FileSize = length;

	memory = (char*)malloc(length + 1);

	file.read(memory,length);

	int SizeLeft;
	for (int Offset = 0; Offset < length; Offset += 64)
	{
		SizeLeft = length - Offset;
		if (SizeLeft < 64)
		{
			BF_decrypt(memory + Offset, SizeLeft);
		}
		else
		{
			BF_decrypt(memory + Offset, 64);
		}
	}

	file.close();


	/*std::fstream str;
	str.open("blah.b3d", std::ios::out | std::ios::binary);

	//str.write(memory,sizeof(memory));
	//str.close();

	//FILE* fp = fopen("blah.b3d","w");
	char c = 0;
	for(int i = 0; i < getSize(); ++i)
	{
		read(&c,1);
		str.write(&c,1);
	}
	//fputs(memory,fp);
	//fclose(fp);
	str.close();
	seek(0);*/

}


CReadMemory::~CReadMemory()
{
	if(!PushedBuffer)
		free(memory);
}



//! returns if file is open
inline bool CReadMemory::isOpen()
{
	return true;
}



//! returns how much was read
s32 CReadMemory::read(void* buffer, s32 sizeToRead)
{
	if (!isOpen())
		return 0;

	// We want to read from decrypted memory here, not the file
	//s32 ret = fread(buffer, 1, sizeToRead, File);
	memcpy(buffer, memory + Position, sizeToRead);
	Position += sizeToRead;
	//return ret;
	return sizeToRead;

}

//! changes position in file, returns true if successful
//! if relativeMovement==true, the pos is changed relative to current pos,
//! otherwise from begin of file
bool CReadMemory::seek(s32 finalPos, bool relativeMovement)
{
	if(relativeMovement)
	{
		Position += finalPos;
	}else{
		Position = finalPos;
	}
	return true;
}

//! returns size of file
s32 CReadMemory::getSize()
{
	return FileSize;
}

//! returns where in the file we are.
s32 CReadMemory::getPos()
{
	return Position;
}

//! returns name of file
const c8* CReadMemory::getFileName()
{
	return Filename.c_str();
}




} // end namespace io
} // end namespace irr

