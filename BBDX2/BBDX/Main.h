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
#ifndef _BBDX_MAIN
#define _BBDX_MAIN 1
#define WIN32_LEAN_AND_MEAN

// Includes (main)
//#define _CRTDBG_MAP_ALLOC
//#include <crtdbg.h>
//
//#ifdef _DEBUG
//#define DEBUG_CLIENTBLOCK new( _CLIENT_BLOCK, __FILE__, __LINE__)
//#else
//#define DEBUG_CLIENTBLOCK
//#endif

#include <stdio.h>
#include <wchar.h>
#include <irrlicht.h>
#include <windows.h>
#include <math.h>
#include <fstream>

#include "Render Targets\cRenderTargetManager.h"
#include "Render Targets\cRenderTarget.h"
#include "Render Targets\cPP_Effect.h"
//#include "..\GUI\IGUIManager.h"

// Namespaces
using namespace irr::scene;
using namespace irr::core;
using namespace irr::video;
using namespace irr::io;

// Libraries
//#pragma comment(lib, "User32.Lib")
//#pragma comment(lib, "Irrlicht.lib")
//#pragma comment(lib, "IrrKlang.lib")

// Collisiona nd picking lists
irr::core::array<ISceneNode*> SpherePickingNode = 0;
irr::core::array<ISceneNode*> PolyPickingNode = 0;
irr::core::array<ISceneNode*> CollisionEntities = 0;

// Data class to pass information between blitz and C
class BBContain
{
public:
	unsigned int idat[96];
	char *sdat[16];
	float fdat[4];
};

// Data class of the LoadthreadMesh function
struct ThreadData
{
	IAnimatedMeshSceneNode* node;
	irr::c8* filename;
	ISceneNode* parent;
	HANDLE*  hand;
};

// BBThread is a Blitz-compatible threading class
class BBThread
{
public:
	ThreadData* myc;
	char	done;	// Is the thread complete?
	DWORD	TID;	// Thread ID
	HANDLE	TTH;	// Thread Handle
};

// Nuclear Glory Vectors
struct NGCP_VEC3
{
	float x, y, z;
};

struct NGCPfl_VEC3
{
	float x, y, z;
};

// Macros
#define DLLPRE extern "C" _declspec(dllexport)
#define DLLIMP __declspec(dllimport)
#define DLLEX
#define ExtractAnimSeq(node,first,last) growth(node, first, last)
#define Animate(node,mode,speed,seq) gisranlo(node,mode,speed,seq)

// Function definitions
irr::s32 GetCollisionID();
void* LO_loadthreadmesh(BBThread* td);
DLLPRE void DLLEX gisranlo(IAnimatedMeshSceneNode* node, int mode, float speed, int seq);
DLLPRE int DLLEX growth(IAnimatedMeshSceneNode* node, int first, int last);
DLLPRE void DLLEX getmeshake(ISceneNode* node);
DLLPRE void DLLEX smokinhot(ISceneNode* node, ITexture* tex, int forgetframe, irr::s32 index, irr::s32 surface);
DLLPRE void DLLEX crystalclear(ISceneNode* Node, ISceneNode* Parent, int Global);
DLLPRE void DLLEX jockgnome(IAnimatedMeshSceneNode* node);
DLLPRE void DLLEX shogun(ISceneNode* node, irr::f32 a);


// Globals
irr::IrrlichtDevice*		device = 0;				// Irrlichts Device handle
irr::video::IVideoDriver*	driver = 0;				// Renderer handle
irr::scene::ISceneManager*	smgr;					// Scene Manager handle
HWND						hWnd;					// Window handle
HINSTANCE					hInstance;				// Instance handle
IAnimatedMesh*				RecentMesh;				// Used for threadmesh (mostly unused for the moment)
SColor						BackGround;				// Background colour
irr::s32					SceneObjectCount = 0;	// Used for collisions (for the moment)
int							screenWidth;			// Used to track screen resolution
int							screenHeight;			// 
int							enableAnis;				// Unused (Used to be for anisotropic filtering)
ITexture*					DefaultTexture = 0;		// Default texture incase one won't load

// Post Process Variables and Functions Defined
cRenderTargetManager*		pRTM;
std::list<cPP_Effect *>		lpPP_Effects;
cPP_Effect *				ActivePP_Effect;
bool						bEnablePP_Pipeline;


//IGUIManager* GUIManager = 0;

// This is a basic debug flag, for setting bounding boxen in the scene
bool DEBUGMODE = false;

#define SB sprintf(out,
#define OU

#endif