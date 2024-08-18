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

#include "Temp.h"
#include <NGinString.h>
#include <List.h>
#include <BlitzPlus.h>

#include <type.h>

class IShaderParameter
{
protected:
	NGin::Type _Type;

public:

	IShaderParameter() { _Type = Type("RCSP", ""); }

	virtual NGin::Type GetType() { return _Type; }
	virtual void FromString(string& Str) = 0;

	static NGin::Type TypeOf() { return Type("RCSP", ""); }
};

class SPVector1 : public IShaderParameter
{
public:
	float X;

	SPVector1() : IShaderParameter()
	{
		_Type = Type("SPV1", "");
		X = 0;
	}

	virtual void FromString(string& Str)
	{
		X = std::toFloat(Str);
	}

	static NGin::Type TypeOf() { return Type("SPV1", ""); }
};

class SPVector2 : public IShaderParameter
{
public:
	float X, Y;

	SPVector2() : IShaderParameter()
	{
		_Type = Type("SPV2", "");
		X = Y = 0;
	}

	virtual void FromString(string& Str)
	{
		int Comma1 = Str.find(",", 0);

		if(Comma1 != -1)
		{
			string sX = Str.substr(0, Comma1);
			string sY = Str.substr(Comma1 + 1);

			sX = std::trim(sX);
			sY = std::trim(sY);

			X = std::toFloat(sX);
			Y = std::toFloat(sY);
		}
	}

	static NGin::Type TypeOf() { return Type("SPV2", ""); }
};

class SPVector3 : public IShaderParameter
{
public:
	float X, Y, Z;

	SPVector3() : IShaderParameter()
	{
		_Type = Type("SPV3", "");
		X = Y = Z = 0;
	}

	virtual void FromString(std::string& Str)
	{
		int Comma1 = Str.find(",", 0);
		int Comma2 = Str.find(",", Comma1 + 1);

		if(Comma1 != -1 && Comma2 != -1)
		{
			string sX = Str.substr(0, Comma1);
			string sY = Str.substr(Comma1 + 1, Comma2 - Comma1 - 1);
			string sZ = Str.substr(Comma2 + 1);
			
			sX = std::trim(sX);
			sY = std::trim(sY);
			sZ = std::trim(sZ);

			X = std::toFloat(sX);
			Y = std::toFloat(sY);
			Z = std::toFloat(sZ);
		}
	}

	static NGin::Type TypeOf() { return Type("SPV3", ""); }
};

class SPVector4 : public IShaderParameter
{
public:
	float X, Y, Z, W;

	SPVector4() : IShaderParameter()
	{
		_Type = Type("SPV4", "");
		X = Y = Z = W = 0;
	}

	virtual void FromString(string& Str)
	{
		int Comma1 = Str.find(",", 0);
		int Comma2 = Str.find(",", Comma1 + 1);
		int Comma3 = Str.find(",", Comma2 + 1);

		if(Comma1 != -1 && Comma2 != -1 && Comma3 != -1)
		{
			string sX = Str.substr(0, Comma1);
			string sY = Str.substr(Comma1 + 1, Comma2 - Comma1 - 1);
			string sZ = Str.substr(Comma2 + 1, Comma3 - Comma2 - 1);
			string sW = Str.substr(Comma3 + 1);

			sX = std::trim(sX);
			sY = std::trim(sY);
			sZ = std::trim(sZ);
			sW = std::trim(sW);

			X = std::toFloat(sX);
			Y = std::toFloat(sY);
			Z = std::toFloat(sZ);
			W = std::toFloat(sW);
		}
	}

	static NGin::Type TypeOf() { return Type("SPV4", ""); }
};

struct LoadedEffect
{
	std::string Name;
	std::string Path;
	int ID;
	uint Effect;
};
extern ArrayList<LoadedEffect*> LoadedShaders;
extern ArrayList<uint> LoadedProfiles;
extern uint DefaultStatic, DefaultAnimated;

uint GetProfile(int ID);
int GetShader(int ID);
void LoadShaders();

