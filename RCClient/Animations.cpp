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

#include "Animations.h"

ClassDef(AnimSet, AnimSetList, AnimSetDelete);

AnimSet* AnimList[1000];

// Plays a given animation at the given speed
void PlayAnimation(ActorInstance* AI, int Mode, float Speed, int Seq, bool FixedSpeed)
{
	//printf("Animation(%x, %i, %i)\n", AI, Mode, Seq);
	AnimSet* A = 0;
	if(AI->Gender == 0)
		A = AnimList[AI->Actor->MAnimationSet];
	else
		A = AnimList[AI->Actor->FAnimationSet];

	if(A->AnimEnd[Seq] == 0 || A->AnimStart[Seq] > A->AnimEnd[Seq])
		return;

	if(FixedSpeed)
	{
		float Length = A->AnimEnd[Seq] - A->AnimStart[Seq];
		if(Length < 1.0f)
			Length = 1.0f;
		Speed *= Length;
	}
	Animate(AI->EN, Mode, Speed * A->AnimSpeed[Seq], AI->AnimSeqs[Seq], 1.0f);
	
}

// Creates a new animation set
int CreateAnimSet()
{
	AnimSet* A = new AnimSet();
	A->ID = -1;
	for(int i = 0; i < 1000; ++i)
		if(AnimList[i] == 0)
		{
			A->ID = i;
			break;
		}

	if(A->ID == -1)
	{
		AnimSet::Delete(A);
		AnimSet::Clean();
		return -1;
	}

	AnimList[A->ID] = A;
	A->Name = "New Animation Set";
	A->AnimName[149] = "Walk";
	A->AnimName[148] = "Run";
	A->AnimName[147] = "Swim idle";
	A->AnimName[146] = "Swim slow";
	A->AnimName[145] = "Swim fast";
	A->AnimName[144] = "Ride idle";
	A->AnimName[143] = "Ride walk";
	A->AnimName[142] = "Ride run";
	A->AnimName[141] = "Default attack";
	A->AnimName[140] = "Right hand attack";
	A->AnimName[139] = "Two hand attack";
	A->AnimName[138] = "Staff attack";
	A->AnimName[137] = "Default parry";
	A->AnimName[136] = "Right hand parry";
	A->AnimName[135] = "Two hand parry";
	A->AnimName[134] = "Staff parry";
	A->AnimName[133] = "Shield parry";
	A->AnimName[132] = "Hit 1";
	A->AnimName[131] = "Hit 2";
	A->AnimName[130] = "Hit 3";
	A->AnimName[129] = "Death 1";
	A->AnimName[128] = "Death 2";
	A->AnimName[127] = "Death 3";
	A->AnimName[126] = "Jump";
	A->AnimName[125] = "Idle";
	A->AnimName[124] = "Yawn";
	A->AnimName[123] = "Look around";
	A->AnimName[122] = "Sit down";
	A->AnimName[121] = "Sit idle";
	A->AnimName[120] = "Stand up";
	A->AnimName[119] = "Strafe Left";
	A->AnimName[118] = "Strafe Right";
	for(int i = 0; i < 150; ++i)
		A->AnimSpeed[i] = 1.0f;
	return A->ID;
}

// Loads all animation sets
int LoadAnimSets(string Filename)
{
	int Sets = 0;

	FILE* F = ReadFile(Filename);
	if(F == 0)
		RuntimeError("Could not read animation sets!");

	while(!Eof(F))
	{
		AnimSet* A = new AnimSet();
		A->ID = ReadShort(F);
		AnimList[A->ID] = A;
		A->Name = ReadString(F);

		for(int i = 0; i < 150; ++i)
		{
			A->AnimName[i] = ReadString(F);
			A->AnimStart[i] = ReadShort(F);
			A->AnimEnd[i] = ReadShort(F);
			A->AnimSpeed[i] = ReadFloat(F);
		}

		++Sets;
	}

	CloseFile(F);
	return Sets;
}

// Saves all animation sets
bool SaveAnimSets(string Filename)
{
	FILE* F = WriteFile(Filename);
	if(F == 0)
		RuntimeError("Could not write to animations file!");

	foreachc(It, AnimSet, AnimSetList)
	{
		AnimSet* A = (*It);

		WriteShort(F, A->ID);
		WriteString(F, A->Name);
		for(int i = 0; i < 150; ++i)
		{
			WriteString(F, A->AnimName[i]);
			WriteShort(F, A->AnimStart[i]);
			WriteShort(F, A->AnimEnd[i]);
			WriteFloat(F, A->AnimSpeed[i]);
		}
		nextc(It, AnimSet, AnimSetList);
	}

	CloseFile(F);
	return true;
}

// Finds an animation in a given set
int FindAnimation(AnimSet* A, string AnimName)
{
	AnimName = stringToUpper(AnimName);
	for(int i = 0; i < 150; ++i)
		if(stringToUpper(A->AnimName[i]).compare(AnimName) == 0)
			return i;
	return -1;
}

// Finds the current animation number of an actor
int CurrentSeq(ActorInstance* AI)
{
	int Seq = AnimSeq(AI->EN);
	if(Seq > 0)
		for(int i = 149; i >= 0; --i)
			if(AI->AnimSeqs[i] == Seq)
				return i;
	return -1;
}
