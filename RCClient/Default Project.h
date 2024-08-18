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
#include <BlitzPlus.h>
#include <NGinString.h>
#include "ITimerManager.h"
#include <ITerrainManager.h>
#include <SDKMain.h>

#include "WOWCamera.h"
#include "Actors.h"
#include "Logging.h"
#include "ClientCombat.h"
#include "Environment.h"
#include "Environment3D.h"
#include "ClientLoaders.h"
#include "Projectiles3D.h"
#include "Spells.h"
#include "MainMenu.h"
#include "ClientNet.h"
#include <ITreeManager.h>
#include "COptionsMenu.h"

#ifdef USE_240
namespace LT
{
	__declspec(dllimport) LT::ITreeRenderer* CreateDirect3D9TreeRenderer(
		IDirect3DDevice9* device, const char* trunkEffect, const char* leafEffect, const char* lodEffect);
}
void TreeManager_CollisionChanged(LT::ITreeZone* sender, LT::CollisionEventArgs* e);
#endif

using namespace RealmCrafter::RCT;
using namespace std;

struct SoundZone;

namespace RealmCrafter
{
	class COptionsMenu;
	class CServerSelector;
}
class Scenery;

extern ASyncFuncs* Perf;

// GUI
extern IGUIManager* GUIManager;
extern RealmCrafter::COptionsMenu* OptionsMenu;

// Terrain
extern ITerrainManager* TerrainManager;

// LT
#ifdef USE_240
extern LT::ITreeRenderer* TreeRenderer;
extern LT::ITreeManager* TreeManager;
extern LT::ITreeZone* TreeZone;
#endif

// Camera
extern RealmCrafter::ICameraController* CameraController;

extern uint MainWindow;
extern int PlayerTarget, ActorSelectEN, ClickMarkerEN, ClickMarkerTimer;
extern Scenery* SceneryTarget;
extern string UpdateGame;
extern bool UpdateMusic;
extern string UpdateHost;
extern int PeerToHost, Connection;
extern int Cam;
extern ActorInstance* Me;
extern int SelectedCharacter;
extern RealmCrafter::SQuestLog* MyQuestLog;

extern int CurrentAreaID, FlashEN, FlashStart, FlashLength;
extern bool PVPEnabled;
extern bool Flashing;
extern bool QuitComplete;
extern bool LogoutComplete;
extern bool PlayerHasTouchedDown;
extern int ZonedMS, RandomImages, EULAAccepted, AlwaysShowEULA;

// Graphics
extern int GFXWidth, GFXHeight, GFXDepth, GFXAnisotropyLevel;
extern int GFXAntiAlias, GFXShadowDetail, GFXShaderQuality;
extern bool GFXWindowed;

// How many frames to take the average of to get the current delta
#define DeltaFrames 6
extern int DeltaBuffer[6];
extern float FPS, Delta;

#define BaseFramerate 30.0f

// Network Timing (1000 / 5)
#define NetworkMS 200
extern int LastNetwork;

// Collision Types
#define C_None      0
#define C_Sphere    1
#define C_Box       2
#define C_Triangle  3
#define C_Actor     4
#define C_Player    5
#define C_Cam       6
#define C_ActorTri1 7
#define C_ActorTri2 8

// Gravity settings
extern float Gravity;
#define JumpStrength 8.0

// Distance moved when moving forwards
#define KeyboardMoveDistance 3.0

// General Purpose Pivot
extern int GPP;

// Other Entities
extern int LootBagEN;

// Variables define within a code block
extern FILE* MainLog;

// Terrain collision structure
struct TerrainTagItem
{
	bool Active;
	uint Mesh;
	NGin::Math::Vector3 Position;
};

// SDK
typedef RealmCrafter::IControlEditor* (tCreateControlEditorfn)(NGin::GUI::IGUIManager* manager, std::string name, NGin::Math::Vector2 location, NGin::Math::Vector2 size);
extern tCreateControlEditorfn* CreateControlEditor;

typedef void (tChangeEnvironmentfn)(std::string environmentName, std::string zoneName);
typedef void (tSetEnvTimefn)(int timeH, int timeM, float secondInterpolation);
typedef void (tSetEnvDatefn)(int* day, int* year);
typedef void (tUpdateEnvironmentfn)(float deltaTime);
typedef void (tSetEnvCameraUnderWaterfn)(bool under, unsigned char destR, unsigned char destG, unsigned char destB);
typedef void (tProcessEnvNetPacketfn)(RealmCrafter::PacketReader& reader);
typedef bool (tEnvPlayWetFootstepfn)();
typedef bool (tResolutionChangedfn)(float newWidth, float newHeight);
extern tChangeEnvironmentfn* ChangeEnvironment;
extern tSetEnvTimefn* SetEnvTime;
extern tSetEnvDatefn* SetEnvDate;
extern tUpdateEnvironmentfn* UpdateEnvironment;
extern tSetEnvCameraUnderWaterfn* SetEnvCameraUnderWater;
extern tProcessEnvNetPacketfn* ProcessEnvNetPacket;
extern tEnvPlayWetFootstepfn* EnvPlayWetFootstep;
extern tResolutionChangedfn* ResolutionChanged;

// Shadow mapping details
extern uint DefaultDepthShader, DefaultAnimatedDepthShader;
extern uint ShadowBlurH, ShadowBlurV;
extern int ShadowQuality;

void PlayerChangedSector(int sectorX, int sectorZ);
void UpdateChatBubbles();
void UpdateNametags();
void UpdateActorInstances();
void UpdateRidingActorInstances();
void UpdateSwimmingActorInstances();
void UpdateCamera();
void UpdateAnimatedScenery();
void UpdateSoundZones();
void PlaySoundZone(SoundZone* SZ);
//void SetDestination(RealmCrafter::SDK::IActorInstance* A, float DestX, float DestZ, float Y, bool TurnMe = true);
NGin::CString Encrypt(NGin::CString S, int Reverse = -1);
float CurveValue(float Current, float Destination, float Curve);
void SortSpells();
bool HighAlphabetical(string &A, string &B);
void ScreenFlash(int R, int G, int B, int TextureID, int Length, float InitialAlpha = 1.0f);
void UpdateScreenFlash();
string Money(int Amount);
uint CreateCentredWindow(string Title, int W, int H, int Parent, int Flags = 15, bool Hidden = false);
void RenderGUI();
void DeviceLost();
void DeviceReset();
void RenderSolid(int rtindex);
void RenderDepth(const float* lightMatrix);
void RenderDepthVP(const float* lightView, const float* lightProjection);
void Terrain_CollisionChanged(ITerrain* terrain, CollisionEventArgs* e);
