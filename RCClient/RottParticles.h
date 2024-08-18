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

// To do --------------------------------------------------------------------------------------------------------------------
// -----/
// Particle bounce/destroy planes

// Constants ----------------------------------------------------------------------------------------------------------------

// Emitter shapes
#define RP_Sphere   1
#define RP_Cylinder 2
#define RP_Box      3

// Config velocities calculation modes
#define RP_Normal            1
#define RP_ShapeBased        2
#define RP_HeavilyShapeBased 3

// Config force modifier shaping modes
#define RP_Linear    1
#define RP_Spherical 2

// structs --------------------------------------------------------------------------------------------------------------------

struct RP_EmitterConfig;

// An actual particle emitter
struct RP_Emitter
{
	uint MeshEN, EmitterEN;
	RP_EmitterConfig* Config;
	bool Enabled;
	int KillMode;
	int ToSpawn;
	int ActiveParticles;
	float Scale;

	ClassDecl(RP_Emitter, RP_EmitterList, RP_EmitterDelete);
};

// A single particle (4 vertices)
struct RP_Particle
{
	RP_Emitter* E;
	uint FirstVertex;
	float Scale;
	float X, Y, Z;
	float VX, VY, VZ;
	float FX, FY, FZ;
	float R, G, B;
	float A;
	bool InUse;
	float TimeToLive;
	int TexFrame;
	float TexChange;

	ClassDecl(RP_Particle, RP_ParticleList, RP_ParticleDelete);
};

// A configuration template for emitters
struct RP_EmitterConfig
{
	string Name;
	int MaxParticles, ParticlesPerFrame;
	uint Texture, TexAcross, TexDown, RndStartFrame, TexAnimSpeed;
	int VShapeBased;
	float VelocityX, VelocityY, VelocityZ;
	float VelocityRndX, VelocityRndY, VelocityRndZ;
	float ForceX, ForceY, ForceZ;
	float ForceModX, ForceModY, ForceModZ;
	int ForceShaping;
	float ScaleStart, ScaleChange;
	int Lifespan;
	int RStart, GStart, BStart;
	float RChange, GChange, BChange;
	float AlphaStart, AlphaChange;
	uint FaceEntity;
	int BlendMode;
	int Shape;
	float MinRadius, MaxRadius;
	float Width, Height, Depth;
	int ShapeAxis;
	int DefaultTextureID; // Realm Crafter specific

	ClassDecl(RP_EmitterConfig, RP_EmitterConfigList, RP_EmitterConfigDelete);
};

void RP_Update(float Delta = 1.0f);
void RP_CreateParticle(RP_Emitter* E);
void RP_SpawnParticle(RP_Particle* P);
void RP_SetParticleFrame(RP_Particle* P, int Frame);
void RP_UpdateParticleVertices(RP_Particle* P);
bool RP_ConfigLifespan(uint ID, int Lifespan);
bool RP_ConfigSpawnRate(uint ID, int SpawnRate);
bool RP_ConfigVelocityMode(uint ID, int Level);
bool RP_ConfigVelocityX(uint ID, float VelocityX);
bool RP_ConfigVelocityY(uint ID, float VelocityY);
bool RP_ConfigVelocityZ(uint ID, float VelocityZ);
bool RP_ConfigVelocityRndX(uint ID, float VelocityX);
bool RP_ConfigVelocityRndY(uint ID, float VelocityY);
bool RP_ConfigVelocityRndZ(uint ID, float VelocityZ);
bool RP_ConfigForceX(uint ID, float VelocityX);
bool RP_ConfigForceY(uint ID, float VelocityY);
bool RP_ConfigForceZ(uint ID, float VelocityZ);
bool RP_ConfigForceModMode(uint ID, int Mode);
bool RP_ConfigForceModX(uint ID, float ForceModX);
bool RP_ConfigForceModY(uint ID, float ForceModY);
bool RP_ConfigForceModZ(uint ID, float ForceModZ);
bool RP_ConfigInitialRed(uint ID, int Cl);
bool RP_ConfigInitialGreen(uint ID, int Cl);
bool RP_ConfigInitialBlue(uint ID, int Cl);
bool RP_ConfigRedChange(uint ID, float Cl);
bool RP_ConfigGreenChange(uint ID, float Cl);
bool RP_ConfigBlueChange(uint ID, float Cl);
bool RP_ConfigInitialAlpha(uint ID, float Alpha);
bool RP_ConfigAlphaChange(uint ID, float Alpha);
bool RP_ConfigFaceEntity(uint ID, uint Entity);
bool RP_ConfigBlendMode(uint ID, int Blend);
bool RP_ConfigMaxParticles(uint ID, int MaxParticles);
bool RP_ConfigInitialScale(uint ID, float Scale);
bool RP_ConfigScaleChange(uint ID, float Scale);
bool RP_ConfigTexture(uint ID, uint Texture, int TilesX, int TilesY, bool FreePreviousTexture = true);
bool RP_ConfigTextureAnimSpeed(uint ID, int Speed);
bool RP_ConfigTextureRandomStartFrame(uint ID, int Flag);
bool RP_ConfigShapeSphere(uint ID, float MinRadius = 0.0f, float MaxRadius = 0.0f);
bool RP_ConfigShapeCylinder(uint ID, float MinRadius, float MaxRadius, float Length, int Axis);
bool RP_ConfigShapeBox(uint ID, float Width, float Height, float Depth);
uint RP_CreateEmitterConfig(int MaxParticles, int SpawnRate, uint FaceEntity, uint Texture, int TilesX = 1, int TilesY = 1, const char* Name = "New Emitter");
uint  RP_CopyEmitterConfig(uint ID);
bool RP_FreeEmitterConfig(uint ID, uint FreeTex);
uint RP_LoadEmitterConfig(const char* File, uint Texture, uint FaceEntity);
uint RP_CreateEmitter(uint Configuration, float Scale = 1.0f);
int RP_EmitterActiveParticles(uint ID);
bool RP_EnableEmitter(uint ID);
bool RP_DisableEmitter(uint ID);
bool RP_HideEmitter(uint ID);
bool RP_ShowEmitter(uint ID);
bool RP_ScaleEmitter(uint ID, float Scale);
bool RP_KillEmitter(uint ID, bool FreeConfig = false, bool FreeTex = false);
bool RP_FreeEmitter(uint ID, bool FreeConfig = false, bool FreeTex = false);
void RP_Clear(bool Configs = true, bool Textures = true);
