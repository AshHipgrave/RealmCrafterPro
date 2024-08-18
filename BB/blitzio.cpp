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
#include <windows.h>
#include "BlitzPlus.h"

namespace BlitzPlus
{

	FILE* ReadFile(std::string Filename)
	{
		return fopen(Filename.c_str(), "rb");
	}

	FILE* WriteFile(std::string Filename)
	{
		return fopen(Filename.c_str(), "wb");
	}

	FILE* OpenFile(std::string Filename)
	{
		FILE* F = fopen(Filename.c_str(),"r+b");
		if(F == 0)
			F = fopen(Filename.c_str(), "w+b");
		return F;
	}

	void CloseFile(FILE* F)
	{
		fclose(F);
	}

	unsigned char ReadByte(FILE* F)
	{
		unsigned char B;
		fread(&B,sizeof(unsigned char),1,F);
		return B;
	}

	unsigned short ReadShort(FILE* F)
	{
		return ReadByte(F) | (ReadByte(F) << 8);
	}

	unsigned int ReadInt(FILE* F)
	{
		unsigned int B;
		fread(&B,sizeof(unsigned int),1,F);
		return B;
	}

	float ReadFloat(FILE* F)
	{
		float B;
		fread(&B,sizeof(float),1,F);
		return B;
	}

	std::string ReadString(FILE* F)
	{
		unsigned int ToRead = ReadInt(F);

		char* Str = (char*)malloc(ToRead+1);
		fread(Str,1, ToRead,F);
		Str[ToRead] = 0;
		std::string Out;
		Out.assign((const char*)Str, ToRead);
		free(Str);

		return Out;
	}

	std::string ReadLine(FILE* F)
	{
		uint Pos = ftell(F);

		int Length = 0;
		uchar A = 0;
		uchar Beyond = 0;
		for(;feof(F) == 0;++Length)
		{


			fread(&A, 1, 1, F);
			if(A == '\r')
			{
				fread(&A, 1, 1, F);
				if(A== '\n')
				{
					Beyond = 2;
					break;
				}
				else
					++Length;
			}else if(A == '\n')
			{
				Beyond = 1;
				break;
			}
		}


		fseek(F, Pos, SEEK_SET);

		char* Data = (char*)malloc(Length + 1);
		
		fread(Data, 1, Length, F);

		Data[Length] = 0;
		if(Beyond == 0)
			Data[Length - 1] = 0;

		std::string Out = Data;
		free(Data);

		if(Beyond > 0)
			fseek(F, Beyond, SEEK_CUR);

		return Out;
	}

	void WriteByte(FILE* F, unsigned char B)
	{
		fwrite((void*)&B,sizeof(unsigned char),1,F);
	}

	void WriteShort(FILE* F, unsigned short B)
	{
		fwrite((void*)&B,sizeof(unsigned short),1,F);
	}

	void WriteInt(FILE* F, unsigned int B)
	{
		fwrite((void*)&B,sizeof(unsigned int),1,F);
	}

	void WriteFloat(FILE* F, float B)
	{
		fwrite((void*)&B,sizeof(float),1,F);
	}

	void WriteString(FILE* F, std::string B)
	{
		unsigned int Length = B.length();
		const char* Data = B.c_str();
		WriteInt(F,Length);
		fwrite((void*)Data,Length,1,F);
	}


	void WriteLine(FILE* F, std::string B)
	{
		uint Length = B.length();
		fwrite((void*)B.c_str(), Length, 1, F);
		fwrite((void*)"\r\n", 1, 2, F);
	}


	unsigned int FilePos(FILE* F)
	{
		return ftell(F);
	}

	void SeekFile(FILE* F, unsigned int Pos)
	{
		fseek(F, Pos, SEEK_SET);
	}

	unsigned int FileSize(std::string Filename)
	{
		FILE* F = fopen(Filename.c_str(),"r");
		if(!F) return 0;

		fseek(F,0,SEEK_END);
		unsigned int Length = ftell(F);
		fclose(F);

		return Length;
	}

	bool Eof(FILE* F)
	{
		uint Pos = ftell(F);
		fseek(F, 0, SEEK_END);
		uint Size = ftell(F);
		if(Pos >= Size)
			return true;

		fseek(F, Pos, SEEK_SET);

		return false;

		//if(feof(F) != 0)
		//	return true;
		//else
		//	return false;
	}



	struct DirRead
	{
		WIN32_FIND_DATA FInfo; // We don't really use this
		HANDLE FHand; // Handle of the current file
		bool ReadFirst; // Starts loop!
		std::string DName;
	};

	uint ReadDir(std::string Directory)
	{
		DirRead* D = new DirRead();
		D->DName = Directory + "\\*";
		D->ReadFirst = false;
		HANDLE THand = FindFirstFile(D->DName.c_str(), &(D->FInfo));
		FindClose(THand);
		if(THand == INVALID_HANDLE_VALUE)
		{
			delete D;
			return 0;
		}
		return (uint)D;
	}

	std::string NextFile(uint Dir)
	{
		DirRead* D = (DirRead*)Dir;

		if(D->ReadFirst == false)
		{

			D->FHand = FindFirstFile(D->DName.c_str(), &(D->FInfo));

			D->ReadFirst = true;

			if(D->FHand == INVALID_HANDLE_VALUE)
				return "";
			else
				return D->FInfo.cFileName;
		}else
		{
			if(FindNextFile(D->FHand, &(D->FInfo)) < 1)
				return "";

			return D->FInfo.cFileName;
		}
	}

	void CloseDir(uint Dir)
	{
		DirRead* D = (DirRead*)Dir;

		if(D->ReadFirst)
			FindClose(D->FHand);

		delete D;
	}

	int FileType(std::string File)
	{
		DWORD FAttributes = GetFileAttributes(File.c_str());
		if(FAttributes == INVALID_FILE_ATTRIBUTES)
			return 0;
		if(FAttributes & FILE_ATTRIBUTE_DIRECTORY)
			return 2;
		return 1;
	}

	void BBDeleteFile(std::string File)
	{
		DeleteFile(File.c_str());
	}
}
