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
#include "Gubbins.h"

std::vector<GubbinTemplate*> GubbinTemplate::Templates;

GubbinTemplate::GubbinTemplate()
	: ID(0), Name("Unknown")
{

}

GubbinTemplate::~GubbinTemplate()
{
	for(int i = 0; i < ActorTemplates.size(); ++i)
	{
		delete ActorTemplates[i];
	}
	ActorTemplates.clear();
}

GubbinTemplate* GubbinTemplate::FromID(int id)
{
	for(int i = 0; i < Templates.size(); ++i)
		if(Templates[i]->ID == id)
			return Templates[i];
	return NULL;
}

std::string GubbinTemplate::ReadByteString(FILE* f)
{
	int Len = ReadByte(f);
	char* Data = new char[Len + 1];
	Data[Len] = 0;
	

	fread(Data, 1, Len, f);

	std::string Out = Data;
	delete[] Data;

	return Out;
}

void GubbinTemplate::Load()
{
	FILE* F = ReadFile("Data\\Game Data\\GubbinTemplates.dat");
	if(F == 0)
		return;

	char FileVersion = ReadByte(F);
	if(FileVersion != 1)
		RuntimeError(std::string("GubbinTemplates.dat FileVersion header was incorrect. Received '") + toString((int)FileVersion) + "' but was expecting '1'");

	int TemplateCount = ReadShort(F);
	
	for(int i = 0; i < TemplateCount; ++i)
	{
		GubbinTemplate* T = new GubbinTemplate();
		T->ID = ReadShort(F);
		T->Name = ReadByteString(F);
		printf("Template: %i:%s\n", (int)T->ID, T->Name.c_str());

		int ActorsCount = ReadShort(F);

		for(int a = 0; a < ActorsCount; ++a)
		{
			GubbinActorTemplate* AT = new GubbinActorTemplate();

			AT->ActorID = ReadShort(F);
			AT->Gender = ReadByte(F);
			AT->Emitter = ReadByteString(F);
			AT->MeshID = ReadShort(F);
			AT->UseLight = ReadByte(F) > 0;
			AT->LightRadius = ReadFloat(F);
			AT->LightColor.X = ReadFloat(F);
			AT->LightColor.Y = ReadFloat(F);
			AT->LightColor.Z = ReadFloat(F);
			AT->LightFunction = ReadByteString(F);
		
			AT->AnimationType = ReadByte(F);
			AT->AnimationStartFrame = ReadShort(F);
			AT->AnimationEndFrame = ReadShort(F);

			AT->AssignedBoneName = ReadByteString(F);

			AT->Position.X = ReadFloat(F);
			AT->Position.Y = ReadFloat(F);
			AT->Position.Z = ReadFloat(F);
			AT->Scale.X = ReadFloat(F);
			AT->Scale.Y = ReadFloat(F);
			AT->Scale.Z = ReadFloat(F);
			AT->Rotation.X = ReadFloat(F);
			AT->Rotation.Y = ReadFloat(F);
			AT->Rotation.Z = ReadFloat(F);

			T->ActorTemplates.push_back(AT);
		}

		Templates.push_back(T);
	}

	CloseFile(F);
}