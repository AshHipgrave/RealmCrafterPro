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
#define NOMINMAX
#include "PhysXBBDX.h"
#include "PhysXErrorStream.h"
#include <stdio.h>
#include <windows.h>

#include "PhysXUserAllocator.h"
#include <NxPhysics.h>
#include <NxCapsuleController.h>
#include <NxControllerManager.h>
#include "cooking.h"
#include "stream.h"
#include "Utilities.h"

#include <List.h>
#include <ArrayList.h>

#include "ThreadCooking.h"
#include "..\BBDX\IOThread.h"
#include <list>

struct DisplaceInfo
{
	irr::scene::ISceneNode* Node;
	NxController* Controller;
	NxVec3 Displacement;
};
NGin::List<DisplaceInfo*> Displacements;

// Globals
NxPhysicsSDK*	gPhysicsSDK = NULL;
NxScene*			gScene = NULL;
NxControllerManager* gManager = NULL;
UserAllocator*	gMyAllocator = NULL;

irr::core::vector3df PickedPosition;
irr::core::vector3df PickedNormal;
float PickedDistance = 0;
irr::scene::ISceneNode* PickedEntity = 0;
std::list<irr::scene::ISceneNode*> DynamicObjects;

DLLPRE void* DLLEX bbdx2_GetNxScene()
{
	return gScene;
}

DLLPRE void* DLLEX bbdx2_GetNxPhysicsSDK()
{
	return gPhysicsSDK;
}

// Structures (Move these to own files)
class CollisionContactReport : public NxUserControllerHitReport
{
public:

	virtual NxControllerAction  onShapeHit(const NxControllerShapeHit& hit)
	{
		// Obtain node handle (and verify it)
		irr::scene::ISceneNode* E = (irr::scene::ISceneNode*)hit.controller->getActor()->userData;
		if(E == 0)
			return NX_ACTION_NONE;

		// Create a hit instance to store this collisions
		irr::scene::CollisionHitInstance HitInstance;
		HitInstance.Position = irr::core::vector3df(hit.worldPos.x, hit.worldPos.y, hit.worldPos.z);
		HitInstance.Normal = irr::core::vector3df(hit.worldNormal.x, hit.worldNormal.y, hit.worldNormal.z);

		// Add to list of collisions
		E->CollisionHits.push_back(HitInstance);

		return NX_ACTION_NONE;
	}

	virtual NxControllerAction  onControllerHit(const NxControllersHit& hit)
	{
		return NX_ACTION_NONE;
	}
};


class RaycastReport : public NxUserRaycastReport
{
	virtual bool onHit(const NxRaycastHit& hit)
	{
		// If we hit a valid shape, that is closed than any other shape
		if(hit.shape != NULL)// && hit.distance < PickedDistance)
		{
			// Get node and verify
			irr::scene::ISceneNode* E = (irr::scene::ISceneNode*)hit.shape->getActor().userData;
			if(E != 0)
			{
				// Check that the node is pickable (used EntityPickMode)
				if(E->Pickable)
				{
					if(hit.distance < PickedDistance)
					{
						PickedDistance = hit.distance;
						PickedEntity = E;
						PickedPosition = irr::core::vector3df(hit.worldImpact.x, hit.worldImpact.y, hit.worldImpact.z);
						PickedNormal = irr::core::vector3df(hit.worldNormal.x, hit.worldNormal.y, hit.worldNormal.z);
					}
				}
			}
		}

		return true;
	}
};

struct CollisionPair
{
	int Source, Destination;

	void Set(int source, int destination)
	{
		Source = source;
		Destination = destination;
	}
};


RaycastReport gQueryReport;
CollisionContactReport gContactReport;
irr::core::array<CollisionPair> CollisionPairs;
HANDLE CookingThread = NULL;
DWORD CookingThreadID = 0;


DLLPRE void bbdx2_SetupPhysX()
{
	// Initialize PhysicsSDK
	NxPhysicsSDKDesc desc;
	NxSDKCreateError errorCode = NXCE_NO_ERROR;
	ErrorStream* CollisionErrors = new ErrorStream();
	gPhysicsSDK = NxCreatePhysicsSDK(NX_PHYSICS_SDK_VERSION, NULL, CollisionErrors, desc, &errorCode);
	if(gPhysicsSDK == NULL) 
	{
		char DBO[2048];
		sprintf(DBO, "SDK create error (%d - %s).", errorCode, getNxSDKCreateError(errorCode));
		MessageBoxA(0, DBO, "PhysX Error", MB_OK);
		exit(0);
		return;
	}


	// The settings for the VRD host and port are found in SampleCommonCode/SamplesVRDSettings.h
	if (gPhysicsSDK->getFoundationSDK().getRemoteDebugger())
		gPhysicsSDK->getFoundationSDK().getRemoteDebugger()->connect("localhost", NX_DBG_DEFAULT_PORT, NX_DBG_EVENTMASK_EVERYTHING);


	gPhysicsSDK->setParameter(NX_SKIN_WIDTH, 0.05f);

	// Create a scene
	NxSceneDesc sceneDesc;
	sceneDesc.gravity				= NxVec3(0.0f, -9.81f, 0.0f);

	//sceneDesc.simType = NX_SIMULATION_SW;


	gScene = gPhysicsSDK->createScene(sceneDesc);
	if(gScene == NULL) 
	{
		MessageBoxA(0, "Error: Unable to create a PhysX scene!", "PhysX Error", MB_OK);
		exit(0);
		return;
	}

	gScene->setTiming(1.0f / 100.0f, 8, NX_TIMESTEP_FIXED);

	// Allocator and Controller manager
	gMyAllocator = new UserAllocator();
	gManager = NxCreateControllerManager(gMyAllocator);

	// Cooking
	if(!InitCooking(gMyAllocator, CollisionErrors))
	{
		MessageBoxA(0, "Unable to initialize PhysX Cooking!", "PhysX Error", MB_OK);
		exit(0);
		return;
	}

	//CookingThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)ASyncInjectThread, NULL, 0, &CookingThreadID);
}

class PhysicsDesc
{
public:

	bool IsStatic;
	NxActorDesc ActorDesc;
	irr::scene::ISceneNode* Node;
};

DLLPRE PhysicsDesc* bbdx2_CreatePhysicsDesc(int isStatic, irr::scene::ISceneNode* node)
{
	// Free existing configuration
	if(node->NxHandle != 0)
		bbdx2_FreeCollisionInstance(node);

	PhysicsDesc* Desc = new PhysicsDesc();
	Desc->IsStatic = isStatic > 0;
	Desc->Node = node;

	Desc->ActorDesc.density		= 10.0f;
	Desc->ActorDesc.globalPose.setColumnMajor44(node->getAbsoluteTransformation().M);
	Desc->ActorDesc.globalPose.M(0, 0) = 1.0f;
	Desc->ActorDesc.globalPose.M(1, 1) = 1.0f;
	Desc->ActorDesc.globalPose.M(2, 2) = 1.0f;

	return Desc;
}

DLLPRE void bbdx2_AddSphere(PhysicsDesc* d, float x, float y, float z, float radius, float mass)
{
	if(d == 0)
		return;

	NxSphereShapeDesc* SphereDesc = new NxSphereShapeDesc();
	SphereDesc->localPose.t.x = x;
	SphereDesc->localPose.t.y = y;
	SphereDesc->localPose.t.z = z;
	SphereDesc->radius = radius;
	SphereDesc->mass = mass;

	d->ActorDesc.shapes.pushBack(SphereDesc);
}

DLLPRE void bbdx2_AddCapsule(PhysicsDesc* d, float x, float y, float z, float width, float height, float mass)
{
	if(d == 0)
		return;

	NxCapsuleShapeDesc* Desc = new NxCapsuleShapeDesc();
	Desc->localPose.t.x = x;
	Desc->localPose.t.y = y;
	Desc->localPose.t.z = z;
	Desc->radius = width;
	Desc->height = height;
	Desc->mass = mass;

	d->ActorDesc.shapes.pushBack(Desc);
}

DLLPRE void bbdx2_AddBox(PhysicsDesc* d, float x, float y, float z, float width, float height, float depth, float mass)
{
	if(d == 0)
		return;

	NxBoxShapeDesc* Desc = new NxBoxShapeDesc();
	Desc->localPose.t.x = x;
	Desc->localPose.t.y = y;
	Desc->localPose.t.z = z;
	Desc->dimensions.x = width * 0.5f;
	Desc->dimensions.y = height * 0.5f;
	Desc->dimensions.z = depth * 0.5f;
	Desc->mass = mass;

	d->ActorDesc.shapes.pushBack(Desc);
}

DLLPRE void bbdx2_ClosePhysicsDesc(PhysicsDesc* d)
{
	if(d == 0)
		return;

	NxBodyDesc BodyDesc;
	BodyDesc.angularDamping	= 0.5f;
	if(!d->IsStatic)
		d->ActorDesc.body = &BodyDesc;

	// Create
	NxActor* Actor = gScene->createActor(d->ActorDesc);

	// CLean shapes
	for(NxU32 i = 0; i < d->ActorDesc.shapes.size(); ++i)
		delete d->ActorDesc.shapes[i];
	d->ActorDesc.shapes.clear();

	// Set important data
	Actor->userData = d->Node;
	d->Node->NxHandle = Actor;
	d->Node->NxHandleType = 10;
	bbdx2_SetNxType(d->Node);

	if(!d->IsStatic)
	{
		DynamicObjects.push_back(d->Node);
	}

	delete d;
}

// Tells the engine that two groups will collide with each other
DLLPRE void bbdx2_Collisions(int source, int destination)
{
	// Increment Source/Dest incase we need to use or avoid 0/0
	++source;
	++destination;

	// Check if it exists
	bool Found = false;
	for(int i = 0; i < CollisionPairs.size(); ++i)
		if(CollisionPairs[i].Source == source && CollisionPairs[i].Destination == destination)
			Found = true;
	if(Found)
		return;

	// Add it
	CollisionPair Pair;
	Pair.Source = source;
	Pair.Destination = destination;
	CollisionPairs.push_back(Pair);
}

// Resets the groups of the NxShapes within this object
DLLPRE void bbdx2_SetNxType(irr::scene::ISceneNode* node)
{
	if(node->NxHandleType == 0)
		return;

	switch(node->NxHandleType)
	{
	case 1: // Capsule
		{
			NxController* Controller = (NxController*)node->NxHandle;

			NxU32 ShapeCount = Controller->getActor()->getNbShapes();
			NxShape*const* Shapes = Controller->getActor()->getShapes();

			for(NxU32 i = 0; i < ShapeCount; ++i)
				Shapes[i]->setGroup(node->EntityType);
			break;
		}
	case 2: // Box
		{
			NxActor* Actor = (NxActor*)node->NxHandle;
			
			NxU32 ShapeCount = Actor->getNbShapes();
			NxShape*const* Shapes = Actor->getShapes();

			for(NxU32 i = 0; i < ShapeCount; ++i)
				Shapes[i]->setGroup(node->EntityType);
			break;
		}
	case 3: // Triangle
		{
			NxActor* Actor = (NxActor*)node->NxHandle;
			
			NxU32 ShapeCount = Actor->getNbShapes();
			NxShape*const* Shapes = Actor->getShapes();

			for(NxU32 i = 0; i < ShapeCount; ++i)
				Shapes[i]->setGroup(node->EntityType);
			break;
		}
	case 10: // Dynamic group
		{
			NxActor* Actor = (NxActor*)node->NxHandle;

			NxU32 ShapeCount = Actor->getNbShapes();
			NxShape*const* Shapes = Actor->getShapes();

			for(NxU32 i = 0; i < ShapeCount; ++i)
				Shapes[i]->setGroup(node->EntityType);
			break;
		}
	}

}

// Change the group of a node
DLLPRE void bbdx2_EntityType(irr::scene::ISceneNode* node, int type)
{
	// Offset all groups to provide a "default" group ID of 1
	++type;
	node->EntityType = type;

	// Change the NxShape group
	bbdx2_SetNxType(node);
}

DLLPRE int bbdx2_GetEntityType(irr::scene::ISceneNode* node)
{
	return node->EntityType - 1;
}

// Remove any PhysX actors used with a node
DLLPRE void bbdx2_FreeCollisionInstance(irr::scene::ISceneNode* node)
{
	switch(node->NxHandleType)
	{
	case 1: // Capsule
		{
			NxController* Controller = (NxController*)node->NxHandle;
			gManager->releaseController(*Controller);
			break;
		}
	case 2: // Box
		{
			NxActor* Actor = (NxActor*)node->NxHandle;
			gScene->releaseActor(*Actor);
			break;
		}
	case 3: // Triangle
		{
			NxActor* Actor = (NxActor*)node->NxHandle;
			gScene->releaseActor(*Actor);

			for(int i = 0; i < node->TriMeshArray.size(); ++i)
				gPhysicsSDK->releaseTriangleMesh(*((NxTriangleMesh*)node->TriMeshArray[i]));
			node->TriMeshArray.clear();

			break;
		}
	case 10: // Dynamic group
		{
			NxActor* Actor = (NxActor*)node->NxHandle;
			gScene->releaseActor(*Actor);
			DynamicObjects.remove(node);
			break;
		}
	}

	foreachf(DIt, DisplaceInfo, Displacements)
	{
		DisplaceInfo* Info = (*DIt);

		if(Info->Node == node)
		{
			Info->Node = 0;
		}

		nextf(DIt, DisplaceInfo, Displacements);
	}


	node->NxHandle = 0;
	node->NxHandleType = 0;
}

// Sets the default slope restriction for actors
float SlopeRestriction = 0.0f;
DLLPRE void bbdx2_SetSlopeRestriction(float restriction)
{
	SlopeRestriction = restriction;
}

// Set the radius of a node (and add a capsule for it)
DLLPRE void bbdx2_EntityRadius(irr::scene::ISceneNode* node, float x, float y)
{
	//x = 2.0f;
	//y = 2.0f;

	// Free any existing configuration
	if(node->NxHandle != 0)
		bbdx2_FreeCollisionInstance(node);

	// Capsule setup
	NxCapsuleControllerDesc CapDesc;
	CapDesc.position.x = node->getAbsolutePosition().X;
	CapDesc.position.y = node->getAbsolutePosition().Y;
	CapDesc.position.z = node->getAbsolutePosition().Z;
	CapDesc.radius = x;
	CapDesc.height = y;
	CapDesc.upDirection = NX_Y;
	CapDesc.slopeLimit = SlopeRestriction;
	CapDesc.callback = &gContactReport;
	CapDesc.skinWidth = 0.01f;//2.0f;//0.01f;
	
	// Create
	NxController* Controller = gManager->createController(gScene, CapDesc);

	// Set important data
	Controller->getActor()->userData = node;
	Controller->setInteraction(NXIF_INTERACTION_USE_FILTER);
	NxShape*const* Shapes = Controller->getActor()->getShapes();
	Shapes[0]->setGroup(1 << 1);

	node->NxHandle = Controller;
	node->NxHandleType = 1;
	bbdx2_SetNxType(node);
}

// Remove collisions for one frame
DLLPRE void bbdx2_ResetEntity(irr::scene::ISceneNode* node)
{
	node->Reset = true;
}

// Setup a box collision node
DLLPRE void bbdx2_EntityBox(irr::scene::ISceneNode* node, float x, float y, float z, float w, float h, float d)
{
	// Free existing configuration
	if(node->NxHandle != 0)
		bbdx2_FreeCollisionInstance(node);

	// Create body
	NxBoxShapeDesc boxDesc;
	boxDesc.dimensions = NxVec3(w, h, d);
	boxDesc.group = 1 << 1;
	boxDesc.skinWidth = 0.01f;

	NxActorDesc actorDesc;
	actorDesc.shapes.pushBack(&boxDesc);
	actorDesc.density		= 10.0f;
	actorDesc.globalPose.t.x = node->getAbsolutePosition().X;
	actorDesc.globalPose.t.y = node->getAbsolutePosition().Y;
	actorDesc.globalPose.t.z = node->getAbsolutePosition().Z;
	
	// Create
	NxActor* Actor = gScene->createActor(actorDesc);

	// Set important data
	Actor->userData = node;
	node->NxHandle = Actor;
	node->NxHandleType = 2;
	bbdx2_SetNxType(node);
}

// Cook a meshbuffer and return its shape information
bool GetSurfaceShape(irr::scene::IAnimatedMeshSceneNode* node, int surfaceIndex, NxTriangleMeshShapeDesc* MeshShape)
{
	// Mesh information
	irr::u32 IndexCount = 0;
	irr::u32 VertexCount = 0;
	irr::core::vector3df* Vertices = 0;
	irr::u32* Indices = 0;

	// Mesh mesh data
	TempGenBuffers(node, surfaceIndex, &Vertices, &Indices, &VertexCount, &IndexCount);

	

	// Get the node scale and premultiply vertices

	irr::core::vector3df Scale = node->getScale();
	Scale = getAbsoluteScale(smgr, node);

	//if((int)node->getParent() != (int)smgr)
	//	Scale = node->getAbsoluteTransformation().getScale();//getAbsoluteScale(smgr, node);//node->getScale();

	for(irr::u32 i = 0; i < VertexCount; ++i)
		Vertices[i] *= Scale;

	// Setup the mesh
	NxTriangleMeshDesc MeshDesc;
	MeshDesc.numVertices = VertexCount;
	MeshDesc.numTriangles = IndexCount / 3;
	MeshDesc.pointStrideBytes = sizeof(NxVec3);
	MeshDesc.triangleStrideBytes = 3 * sizeof(NxU32);
	MeshDesc.points = Vertices;
	MeshDesc.triangles = Indices;
	MeshDesc.flags = 0;

	// Cook the mesh
	MemoryWriteBuffer WriteBuffer;
	if(!CookTriangleMesh(MeshDesc, WriteBuffer))
	{
		OutputDebugString("Failed to cook triangle mesh!\n");

		// Free buffer
		TempRemBuffers(&Vertices, &Indices);

		return false;
	}

	MemoryReadBuffer ReadBuffer(WriteBuffer.data);
	NxTriangleMesh* Mesh = gPhysicsSDK->createTriangleMesh(ReadBuffer);

	// Setup shape
	MeshShape->meshData = Mesh;
	MeshShape->shapeFlags = NX_SF_FEATURE_INDICES;

	// Free buffer
	TempRemBuffers(&Vertices, &Indices);


	return true;
}





DLLPRE void bbdx2_InjectCollisionMesh(irr::scene::ISceneNode* node, void* triangleList, unsigned int vertexCount)
{
		// Free existing configuration
	if(node->NxHandle != 0)
		bbdx2_FreeCollisionInstance(node);

	NxTriangleMeshShapeDesc* MeshShape = new NxTriangleMeshShapeDesc();

	NxU32* Indices = new NxU32[vertexCount];
	for(unsigned int i = 0; i < vertexCount; ++i)
		Indices[i] = i;

	// Setup the mesh
	NxTriangleMeshDesc MeshDesc;
	MeshDesc.numVertices = vertexCount;
	MeshDesc.numTriangles = vertexCount / 3;
	MeshDesc.pointStrideBytes = sizeof(NxVec3);
	MeshDesc.triangleStrideBytes = 3 * sizeof(NxU32);
	MeshDesc.points = triangleList;
	MeshDesc.triangles = Indices;
	MeshDesc.flags = 0;

	// Cook the mesh
	MemoryWriteBuffer WriteBuffer;
	unsigned int S = GetTickCount();
	if(!CookTriangleMesh(MeshDesc, WriteBuffer))
	{
		OutputDebugString("Failed to cook triangle mesh!\n");

		return ;
	}
	unsigned int E = GetTickCount();

	MemoryReadBuffer ReadBuffer(WriteBuffer.data);
	NxTriangleMesh* Mesh = gPhysicsSDK->createTriangleMesh(ReadBuffer);

	// Setup shape
	MeshShape->meshData = Mesh;

	NxActorDesc MeshActor;
	MeshActor.shapes.pushBack(MeshShape);

	// Create Actor
	NxActor* Actor = gScene->createActor(MeshActor);

	for(irr::u32 i = 0; i < MeshActor.shapes.size(); ++i)
		delete MeshActor.shapes[i];
	delete Indices;

	if(Actor == 0)
	{
		node->NxHandle = 0;
		node->NxHandleType = 0;
		bbdx2_SetNxType(node);
		return;
	}
	
	// Important data
	Actor->userData = node;
	node->NxHandle = Actor;
	node->NxHandleType = 3;
	bbdx2_SetNxType(node);
	bbdx2_UpdateStaticPose(node);
}

// Setup a trianglemesh for triangle collision nodes
DLLPRE void bbdx2_SetCollisionMesh(irr::scene::ISceneNode* node)
{
	// Free existing configuration
	if(node->NxHandle != 0)
		bbdx2_FreeCollisionInstance(node);

	// Fetch meshbuffer count
	irr::scene::IAnimatedMeshSceneNode* AnimNode = (irr::scene::IAnimatedMeshSceneNode*)node;
	irr::u32 SurfaceCount = AnimNode->getLocalMesh()->getMesh(0)->getMeshBufferCount();
	NxActorDesc MeshActor;

	for(irr::u32 i = 0; i < SurfaceCount; ++i)
	{
		// Cook this surface/meshbuffer
		NxTriangleMeshShapeDesc* MeshShape = new NxTriangleMeshShapeDesc();
		if(GetSurfaceShape(AnimNode, i, MeshShape))
			MeshActor.shapes.pushBack(MeshShape);
	}

	// Create Actor
	NxActor* Actor = gScene->createActor(MeshActor);

	for(irr::u32 i = 0; i < MeshActor.shapes.size(); ++i)
		delete MeshActor.shapes[i];

	if(Actor == 0)
	{
		node->NxHandle = 0;
		node->NxHandleType = 0;
		bbdx2_SetNxType(node);
		return;
	}
	
	// Important data
	Actor->userData = node;
	node->NxHandle = Actor;
	node->NxHandleType = 3;
	bbdx2_SetNxType(node);
	bbdx2_UpdateStaticPose(node);
}


;

// Move an entity (called from PositionEntity(), MoveEntity() and TranslateEntity())
DLLPRE void bbdx2_PhysXMoveEntity(irr::scene::ISceneNode* node, float x, float y, float z)
{
	// If its not an actor, then leave it
	if(node->NxHandleType != 1)
		return;
	
	// Build a collision mask from the associated groups
	NxController* Controller = (NxController*)node->NxHandle;

	bool Found = false;
	foreachf(DIt, DisplaceInfo, Displacements)
	{
		DisplaceInfo* Info = (*DIt);

		if(Info->Node == node)
		{
			// Node already exists, modify displacement and mask
			Info->Displacement += NxVec3(x, y, z);

			// Move BBDX entity so that other transform calls will work
			Info->Node->setPosition(Info->Node->getPosition() + irr::core::vector3df(x, y, z));
			Info->Node->updateAbsolutePosition();

			Found = true;
			break;
		}

		nextf(DIt, DisplaceInfo, Displacements);
	}

	if(!Found)
	{
		// Not found, setup some new data
		DisplaceInfo* Info = new DisplaceInfo();
		node->setPosition(node->getPosition() + irr::core::vector3df(x, y, z));
		node->updateAbsolutePosition();
		Info->Node = node;
		Info->Displacement = NxVec3(x, y, z);
		Info->Controller = Controller;
		Displacements.Add(Info);
	}
}

// Transform a static object
DLLPRE void bbdx2_UpdateStaticPose(irr::scene::ISceneNode* node)
{
	// It must be a box or a mesh
	if(node->NxHandleType != 2 && node->NxHandleType != 3 && node->NxHandleType != 10)
		return;

	// Build a pose matrix
	irr::core::matrix4 World;
	World.setRotationDegrees(getAbsoluteRotation(smgr, node));
	//World.Scale(node->Scale);
	World.setTranslation(node->getAbsolutePosition());

	// Set actor pose
	NxActor* Actor = (NxActor*)node->NxHandle;
	NxMat34 NxWorld;
	NxWorld.setColumnMajor44(World.M);
	Actor->setGlobalPose(NxWorld);
}

// Update PhysX
DLLPRE void bbdx2_UpdateWorld()
{
	IOThreadASyncInjectCompleteUpdate();
	ASyncInjectCompleteUpdate();

	

	foreachf(DIt, DisplaceInfo, Displacements)
	{
		DisplaceInfo* Info = *DIt;

		if(Info->Node != 0)
		{

			// Clear current collision list
			Info->Node->CollisionHits.clear();

			if(Info->Node->Reset)
			{
				irr::core::vector3df tGlobalPos = Info->Node->getAbsolutePosition();
				NxExtendedVec3 GlobalPos(tGlobalPos.X, tGlobalPos.Y, tGlobalPos.Z);
				//GlobalPos += Info->Displacement;

				Info->Controller->setPosition(GlobalPos);

				Info->Node->Reset = false;
			}else
			{

				irr::u32 Flags = 0;

				int CollisionMask = 0;
				int ControllerGroup = Info->Node->EntityType;

				for(int i = 0; i < CollisionPairs.size(); ++i)
					if(CollisionPairs[i].Source == ControllerGroup)
						CollisionMask |= 1 << CollisionPairs[i].Destination;

				Info->Controller->move(Info->Displacement, CollisionMask, 0.000001f, Flags);
			

			

			// Set actors new position
			NxVec3 CPos = Info->Controller->getActor()->getGlobalPosition();

			irr::core::vector3df Position(CPos.x, CPos.y, CPos.z);

			if(Info->Node->getParent() != 0)
			{
				Info->Node->setPosition(Position - Info->Node->getParent()->getAbsolutePosition());
			}else
			{
				Info->Node->setPosition(Position);
			}
			Info->Node->updateAbsolutePosition();
			UpdateChildren(Info->Node);

			}

		}

		delete Info;
		nextf(DIt, DisplaceInfo, Displacements);
	}

	Displacements.Clear();

	// Update PhysX
	static unsigned int LastTime = GetTickCount();
	unsigned int Time = GetTickCount();
	gScene->simulate((float)(Time - LastTime)); // TODO: CHANGE ME!
	LastTime = Time;

	for(std::list<irr::scene::ISceneNode*>::iterator It = DynamicObjects.begin(); It != DynamicObjects.end(); ++It)
	{
		irr::scene::ISceneNode* Node = (*It);
		if(Node == 0 || Node->NxHandleType != 10 || Node->NxHandle == 0)
			continue;

		NxActor* Actor = (NxActor*)Node->NxHandle;
		if(!Actor->isSleeping())
		{
			irr::core::matrix4 Mat;
			Actor->getGlobalPose().getColumnMajor44(Mat.M);

			irr::core::vector3df Pos = Mat.getTranslation();
			irr::core::vector3df Rot = Mat.getRotationDegrees();

			int NxType = Node->NxHandleType;
			Node->NxHandleType = -1;
			chaosdigs(Node, Pos.X, Pos.Y, Pos.Z, 0);
			mingja(Node, Rot.X, Rot.Y, Rot.Z, 0);
			Node->NxHandleType = NxType;
		}
	}

	// JB: Removed this, I'm moving the scene entities after I call NxControler::move()
	//uint ActorCount = gScene->getNbActors();
	//NxActor** Actors = gScene->getActors();

	//for(uint i = 0; i < ActorCount; ++i)
	//{
	//	if(Actors[i]->userData != 0)
	//	{
	//		float ArrayMatrix[16];
	//		Actors[i]->getGlobalPose().getColumnMajor44(ArrayMatrix); // Use (float*)&Matrix maybe?

	//		irr::core::matrix4 World;
	//		for(int m = 0; m < 16; ++m)
	//			World.M[m] = ArrayMatrix[m];

	//		

	//		irr::scene::ISceneNode* Object = (irr::scene::ISceneNode*)Actors[i]->userData;

	//		if(Object->EntityType == 7)
	//			continue;

	//		irr::core::vector3df Position = World.getTranslation();

	//		// Why reset rotation? Its not a limiting thing.
	//		//irr::core::vector3df Rotation = World.getRotationDegrees();


	//		if(Object->getParent() != 0)
	//		{
	//			Object->setPosition(Position - Object->getParent()->getAbsolutePosition());

	//			//if(Object->NxHandleType != 1)
	//			//	Object->setRotation(Rotation - getAbsoluteRotation(smgr, Object->getParent()));
	//		}else
	//		{
	//			Object->setPosition(Position);
	//				
	//			//if(Object->NxHandleType != 1)
	//			//	Object->setRotation(Rotation);
	//		}

	//		Object->updateAbsolutePosition();
	//		UpdateChildren(Object);
	//	}
	//}


	// Fetch simulation results
	gScene->flushStream();
	gScene->fetchResults(NX_RIGID_BODY_FINISHED, true);

}

// Check the picking abilities of a node
DLLPRE void bbdx2_EntityPickMode(irr::scene::ISceneNode* node, int picktype)
{
	if(picktype < 1)
		node->Pickable = false;
	else
		node->Pickable = true;

	//if(strcmp(node->getTag(), "TERRAIN") == 0)
	//{
	//	char OO[128];
	//	sprintf(OO, "Terrain Change: %i\n", picktype);
	//	OutputDebugString(OO);
	//}

	bbdx2_SetNxType(node);
}

// Pick a line
DLLPRE irr::scene::ISceneNode* bbdx2_LinePick(float startX, float startY, float startZ, float endX, float endY, float endZ, float radius)
{
	// Convert given coordinates to a PhysX Ray
	NxVec3 Start = NxVec3(startX, startY, startZ);
	NxVec3 End = NxVec3(endX, endY, endZ);
	PickedDistance = End.magnitude();
	//End -= Start;
	End.normalize();

	NxRay Ray;
	Ray.orig = Start;
	Ray.dir = End;

	// Reset globals
	//PickedDistance = NX_MAX_F32;
	PickedEntity = 0;
	PickedPosition = irr::core::vector3df(0, 0, 0);
	PickedNormal = irr::core::vector3df(0, 0, 0);

	// If a tiny, or empty, radius is sent, then we perform a fast raycast
	// else, we perform a sphere test
	if(radius < 0.0001f)
	{
		NxU32 ShapeCount = gScene->raycastAllShapes(Ray, gQueryReport, NX_ALL_SHAPES);
	}else
	{
		// Setup the test data
		NxCapsule TestCapsule;
		TestCapsule.radius = radius;
		TestCapsule.p0 = TestCapsule.p1 = Ray.orig;

		NxU32 Flags = NX_SF_STATICS | NX_SF_DYNAMICS;
		NxSweepQueryHit Results[100];

		// Perform test
		NxU32 HitCount = gScene->linearCapsuleSweep(TestCapsule, Ray.dir * PickedDistance, Flags, NULL, 100, Results, NULL);

		// Square picking distance. NxSweepQueryHit doesn't provide a
		// distance, so we'll generate a squared one to save CPU cycles.
		PickedDistance *= PickedDistance;

		// Loop through hits
		for(NxU32 i = 0; i < HitCount; ++i)
		{
			// Shape must exist and store a valid entity
			if(Results[i].hitShape != NULL)
			{
				irr::scene::ISceneNode* E = (irr::scene::ISceneNode*)Results[i].hitShape->getActor().userData;
				if(E != 0)
				{
					// Test pickability
					if(E->Pickable)
					{
						// Test distance
						float DistanceSQ = (Results[i].point - TestCapsule.p0).magnitudeSquared();
						if(DistanceSQ < PickedDistance)
						{
							// Object passed, copy results
							PickedDistance = DistanceSQ;
							PickedEntity = E;
							PickedPosition = irr::core::vector3df(Results[i].point.x, Results[i].point.y, Results[i].point.z);
							PickedNormal = irr::core::vector3df(Results[i].normal.x, Results[i].normal.y, Results[i].normal.z);
						}
					}
				}
			}
		}
	}


	// If we found something, then return it
	if(PickedEntity != 0)
		return PickedEntity;
	
	return 0;
}

// Pick from the camera
DLLPRE irr::scene::ISceneNode* bbdx2_CameraPick(int x, int y)
{
	// Convert x/y into a 3D line
	D3DXVECTOR3 Near(x, y, 0.0f);
	D3DXVECTOR3 Far(x, y, 1.0f);

	bbdx2_UnProjectVector3(&Near, &Near);
	bbdx2_UnProjectVector3(&Far, &Far);

	// Pick and return
	return bbdx2_LinePick(Near.x, Near.y, Near.z, Far.x - Near.x, Far.y - Near.y, Far.z - Near.z, 0.0f);
}

// Return pick data
DLLPRE float bbdx2_PickedX()
{
	return PickedPosition.X;
}

DLLPRE float bbdx2_PickedY()
{
	return PickedPosition.Y;
}

DLLPRE float bbdx2_PickedZ()
{
	return PickedPosition.Z;
}

DLLPRE float bbdx2_PickedNX()
{
	return PickedNormal.X;
}

DLLPRE float bbdx2_PickedNY()
{
	return PickedNormal.Y;
}

DLLPRE float bbdx2_PickedNZ()
{
	return PickedNormal.Z;
}

// Return collision data
// Note: The game counts collisions from 1 not 0
DLLPRE int bbdx2_CountCollisions(irr::scene::ISceneNode* node)
{
	return node->CollisionHits.size();
}


DLLPRE float bbdx2_CollisionX(irr::scene::ISceneNode* node, int index)
{
	--index;

	return node->CollisionHits[index].Position.X;
}

DLLPRE float bbdx2_CollisionY(irr::scene::ISceneNode* node, int index)
{
	--index;

	return node->CollisionHits[index].Position.Y;
}

DLLPRE float bbdx2_CollisionZ(irr::scene::ISceneNode* node, int index)
{
	--index;

	return node->CollisionHits[index].Position.Z;
}

DLLPRE float bbdx2_CollisionNX(irr::scene::ISceneNode* node, int index)
{
	--index;

	return node->CollisionHits[index].Normal.X;
}

DLLPRE float bbdx2_CollisionNY(irr::scene::ISceneNode* node, int index)
{
	--index;

	return node->CollisionHits[index].Normal.Y;
}

DLLPRE float bbdx2_CollisionNZ(irr::scene::ISceneNode* node, int index)
{
	--index;

	return node->CollisionHits[index].Normal.Z;
}


const char* getNxSDKCreateError(const NxSDKCreateError& errorCode) 
{
	switch(errorCode) 
	{
		case NXCE_NO_ERROR: return "NXCE_NO_ERROR";
		case NXCE_PHYSX_NOT_FOUND: return "NXCE_PHYSX_NOT_FOUND";
		case NXCE_WRONG_VERSION: return "NXCE_WRONG_VERSION";
		case NXCE_DESCRIPTOR_INVALID: return "NXCE_DESCRIPTOR_INVALID";
		case NXCE_CONNECTION_ERROR: return "NXCE_CONNECTION_ERROR";
		case NXCE_RESET_ERROR: return "NXCE_RESET_ERROR";
		case NXCE_IN_USE_ERROR: return "NXCE_IN_USE_ERROR";
		default: return "Unknown error";
	}
};

