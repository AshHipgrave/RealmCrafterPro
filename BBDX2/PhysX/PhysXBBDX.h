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

// Basic
//[X] Collisions(Source, Dest, Method, Response)
//[X] EntityType(Entity, Type)
//[X] GetEntityType(Entity)
//[X] EntityRadius(Node, X, Y, Z)
//[X] EntityBox(Node, x, y, z, w, h, d)

// World update
//[X] Updateworld()

// Mesh
//[X] SetCollisionMesh(Entity)
//InjectCollisionMesh(Entity, Vertices, Count)

// Freeback
//[X] CountCollisions()
//[X] CollisionX/Y/Z/NX/NY/NZ(Entity, Index)

// Raycasting
//[X] EntityPickMode(Node, Mode)
//[X] LinePick(x, y, z, dx, dy, dz)
//[X] CameraPick(Camera, x, y)
//[X] PickedX/Y/Z/NX/NY/NZ()

// Extras
//ResetEntity(Node)

#include "irrlicht.h"
#include <d3dx9.h>

#define DLLPRE extern "C" _declspec(dllexport)
#define DLLPRESTD _declspec(dllexport)
#define DLLEX

extern irr::scene::ISceneManager* smgr;
irr::core::vector3df getAbsoluteScale(irr::scene::ISceneManager* SceneManager, irr::scene::ISceneNode* Node);

DLLPRE void DLLEX TempGenBuffers(irr::scene::IAnimatedMeshSceneNode* node, int index, irr::core::vector3df** vertices, irr::u32** indices, irr::u32* vertexCount, irr::u32* indexCount);
DLLPRE void DLLEX TempRemBuffers(irr::core::vector3df** vertices, irr::u32** indices);
DLLPRE void DLLEX chaosdigs(irr::scene::ISceneNode* Node, float x, float y, float z, int Global);
DLLPRE void DLLEX mingja(irr::scene::ISceneNode* Node, float x, float y, float z, int Global);
DLLPRE void DLLEX bbdx2_UnProjectVector3(D3DXVECTOR3* Point, D3DXVECTOR3* Out);
irr::core::vector3df getAbsoluteRotation(irr::scene::ISceneManager* SceneManager, irr::scene::ISceneNode* Node);
void UpdateChildren(irr::scene::ISceneNode* Node, bool Dummy = false);

DLLPRE void bbdx2_SetupPhysX();
DLLPRE void bbdx2_Collisions(int source, int destination);
DLLPRE void bbdx2_SetNxType(irr::scene::ISceneNode* node);
DLLPRE void bbdx2_EntityType(irr::scene::ISceneNode* node, int type);
DLLPRE int bbdx2_GetEntityType(irr::scene::ISceneNode* node);
DLLPRE void bbdx2_FreeCollisionInstance(irr::scene::ISceneNode* node);
DLLPRE void bbdx2_ResetEntity(irr::scene::ISceneNode* node);
DLLPRE void bbdx2_SetSlopeRestriction(float restriction);
DLLPRE void bbdx2_EntityRadius(irr::scene::ISceneNode* node, float x, float y);
DLLPRE void bbdx2_EntityBox(irr::scene::ISceneNode* node, float x, float y, float z, float w, float h, float d);
//DLLPRE void bbdx2_ActorMove(irr::scene::ISceneNode* node);
DLLPRE void bbdx2_UpdateWorld();
DLLPRE void bbdx2_SetCollisionMesh(irr::scene::ISceneNode* node);
DLLPRE void bbdx2_InjectCollisionMesh(irr::scene::ISceneNode* node, void* triangleList, unsigned int vertexCount);

DLLPRE void bbdx2_PhysXMoveEntity(irr::scene::ISceneNode* node, float x, float y, float z);
DLLPRE void bbdx2_UpdateStaticPose(irr::scene::ISceneNode* node);

DLLPRE void bbdx2_EntityPickMode(irr::scene::ISceneNode* node, int picktype);
DLLPRE irr::scene::ISceneNode* bbdx2_LinePick(float startX, float startY, float startZ, float endX, float endY, float endZ, float radius);
DLLPRE irr::scene::ISceneNode* bbdx2_CameraPick(int x, int y);

DLLPRE float bbdx2_PickedX();
DLLPRE float bbdx2_PickedY();
DLLPRE float bbdx2_PickedZ();
DLLPRE float bbdx2_PickedNX();
DLLPRE float bbdx2_PickedNY();
DLLPRE float bbdx2_PickedNZ();

DLLPRE int bbdx2_CountCollisions(irr::scene::ISceneNode* node);

DLLPRE float bbdx2_CollisionX(irr::scene::ISceneNode* node, int index);
DLLPRE float bbdx2_CollisionY(irr::scene::ISceneNode* node, int index);
DLLPRE float bbdx2_CollisionZ(irr::scene::ISceneNode* node, int index);
DLLPRE float bbdx2_CollisionNX(irr::scene::ISceneNode* node, int index);
DLLPRE float bbdx2_CollisionNY(irr::scene::ISceneNode* node, int index);
DLLPRE float bbdx2_CollisionNZ(irr::scene::ISceneNode* node, int index);