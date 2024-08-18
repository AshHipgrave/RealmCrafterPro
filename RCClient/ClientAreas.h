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
#include "BlitzPlus.h"
#include <ITerrainManager.h>

#include "Media.h"
#include "Default Project.h"
#include "Gooey.h"
#include "LightFunction.h"
#include <AABB.h>

#define MaxFogFar 3000.0f
#define TerrainChunkDetail 32


extern char Outdoors;
extern unsigned char AmbientR, AmbientG, AmbientB;
extern float DefaultLightPitch, DefaultLightYaw;
extern int LoadingTexID, LoadingMusicID;
extern int MapTexID;
extern float SlopeRestrict;
extern NGin::Math::AABB ZoneSize;
extern NGin::Math::Vector3 MenuCameraPosition, MenuCameraLookAt, MenuActorSpawn;

struct Remove_Surf
{
	int ID;

	ClassDecl(Remove_Surf, Remove_SurfList, Remove_SurfDelete);
};

struct Cluster
{
	float XC, YC, ZC;
	int Mesh, Surf[200];

	ClassDecl(Cluster, ClusterList, ClusterDelete);
};

struct Scenery
{
	unsigned char SceneryID; // Set by user, used for scenery ownerships
	int EN;
	uint CollisionEN;
	RealmCrafter::SectorVector Position;
	int MeshID;
	char AnimationMode; // 0 = no animation, 1 = constant animation (loops), 2 = constant (ping-pongs), 3 = animate when selected
	float ScaleX, ScaleY, ScaleZ;
	string Lightmap;     // Lightmap filename
	string RCTE;         // Used by toolbox editors only
	int TextureID;     // To alter the texture loaded automatically with the model, if required (65535 to ignore)
	bool CatchRain;
	char Locked;
	bool Interactive; 

	ClassDecl(Scenery, SceneryList, SceneryDelete);
};

struct Water
{
	int EN;
	unsigned char Opacity;
	unsigned char Red, Green, Blue;
	RealmCrafter::SectorVector Position;
	float ScaleX, ScaleZ;
	int TexID[4];
	int TexHandle[4];
	float TexScale, U, V;
	int ServerWater; // Used by editor only

	ClassDecl(Water, WaterList, WaterDelete);
};

struct ColBox
{
	RealmCrafter::SectorVector Position;
	int EN;
	float ScaleX, ScaleY, ScaleZ;

	ClassDecl(ColBox, ColBoxList, ColBoxDelete);
};

struct Emitter
{
	RealmCrafter::SectorVector Position;
	int Config, EN;
	string ConfigName;
	int TexID;

	ClassDecl(Emitter, EmitterList, EmitterDelete);
};

//extern float TerrainHeights[1024][1024];
struct Terrain
{ 
//	int EN;
//	int BaseTexID, DetailTexID;
//	uint DetailTex;
//	float DetailTexScale;
//	float ScaleX, ScaleY, ScaleZ;
//	int GridSize;
//	int Detail;
//	unsigned char Morph, Shading;
	string Path;
	RealmCrafter::RCT::ITerrain* Handle;


	ClassDecl(Terrain, TerrainList, TerrainDelete);
};
#define Terrain_ChunkDetail 32

struct ZoneLight
{
	RealmCrafter::SectorVector Position;
	int R, G, B;
	float Radius;
	uint Handle;
	RealmCrafter::LightFunction* Function;

	ClassDecl(ZoneLight, ZoneLightList, ZoneLightDelete);
};

struct SoundZone
{
	RealmCrafter::SectorVector Position;
	int EN;
	float Radius;
	int SoundID, MusicID; // Can be one or the other
	int RepeatTime; // Number of seconds to wait before repeating the sound, or -1 to play it once only
	unsigned char Volume; // Volume (1% - 100%)
	int LoadedSound;
	string MusicFilename;
	char Is3D;
	bool ChannelStopped;
	int Channel, Timer;
	float Fade; // Variables for updating the sound zone in the client

	ClassDecl(SoundZone, SoundZoneList, SoundZoneDelete);
};

// Generated from scenery objects to prevent rain/snow particles falling through
struct CatchPlane
{
	float Y;
	RealmCrafter::SectorVector Min, Max;

	ClassDecl(CatchPlane, CatchPlaneList, CatchPlaneDelete);
};


int CreateSubdividedPlane(int XDivs, int ZDivs, float UScale = 1.0f, float VScale = 1.0f, int Parent = 0);
bool LoadArea(string Name, int CameraEN, bool DisplayItems = false, bool UpdateRottNet = false, bool showLoadingBar = true, bool disableAllCollisions = false);
void UnloadArea();
void ChunkTerrain(int Mesh, float chx = 10.0f, float chy = 10.0f, float chz = 10.0f, float XPos = 0.0f, float YPos = 0.0f, float ZPos = 0.0f);
float NearestPower(float N, float Snapper);
int RemoveSurface(int Ent);

