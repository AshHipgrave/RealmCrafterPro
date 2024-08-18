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
#include <Dictionary.h>

#include "ShaderLibrary.h"

extern int LoadedTextures[65535];
extern float LoadedMeshScales[65535];
extern float LoadedMeshX[65535];
extern float LoadedMeshY[65535];
extern float LoadedMeshZ[65535];
extern int LoadedMeshShaders[65535];
extern int LoadedSounds[65535];
extern int TextureFlags[65535];
extern Dictionary<string, IShaderParameter*> LoadedParameters[65535];

extern FILE* LockedMeshes, *LockedTextures, *LockedSounds, *LockedMusic;

struct MeshMinMaxVertices
{
	float MinX, MaxX;
	float MinY, MaxY;
	float MinZ, MaxZ;

	ClassDecl(MeshMinMaxVertices, MeshMinMaxVerticesList, MeshMinMaxVerticesDelete);
};


FILE* LockMeshes();
bool UnlockMeshes();
FILE* LockTextures();
bool UnlockTextures();
FILE* LockSounds();
bool UnlockSounds();
FILE* LockMusic();
bool UnlockMusic();
string GetMeshName(int ID);
string GetTextureName(int ID);
string GetTextureNameNoFlag(int ID);
string GetSoundName(int ID);
string GetMusicName(int ID);
bool SetMeshScale(int ID, float Scale);
bool SetMeshOffset(int ID, float X, float Y, float Z);
bool SetMeshShader(int ID, int Shader);
int GetMesh(int ID);
int GetTexture(int ID, bool Copy = false);
int GetSound(int ID);
void UnloadMesh(int ID);
void UnloadTexture(int ID);
void UnloadSound(int ID);
void SizeEntity(int EN, float Width, float Height, float Depth, bool Uniform = false);
int CopyTexture(int Tex, int Flags);
MeshMinMaxVertices* Mesh_MinMaxVertices(int EN);
MeshMinMaxVertices* MeshMinMaxVerticesTransformed(int EN, float Pitch, float Yaw, float Roll, float ScaleX, float ScaleY, float ScaleZ);
int TempLoadAnimMesh(string Name, int Parent = 0);
int TempLoadMesh(string Name, int Parent = 0);
void LoadEntityParameters();
