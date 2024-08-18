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
#include "ThreadCooking.h"
#include "..\LockableQueue.h"
#include "..\BBDX\BBThread.h"

#define PHYSX_PRIORITY_HIGH 100
#define PHYSX_PRIORITY_LOW 50
#define PHYSX_TYPEID 0x50485953

// Cooking data structure
struct CookInfo
{
	irr::scene::ISceneNode* Node;
	void* Buffer;
	unsigned int BufferLength;
	MemoryWriteBuffer* WriteBuffer;
	bool Deleted;
	int Priority;

	CookInfo()
		: Deleted(false), WriteBuffer(NULL), Node(NULL), Buffer(NULL), BufferLength(0), Priority(0)
	{
	}
};

std::list<CookInfo*> CookList;
volatile bool JobDone = true;
//LockableQueue<CookInfo*> InQueue1;
//LockableQueue<CookInfo*> InQueue2;
//LockableQueue<CookInfo*> OutQueue;



// Cooking Thread
/*void ASyncInjectThread()
{
	while(true)
	{
		CookInfo* Info = NULL;

		if(InQueue1.GetSize() > 0)
			Info = InQueue1.Pop();
		else if(InQueue2.GetSize() > 0)
			Info = InQueue2.Pop();

		if(Info == NULL)
		{
			Sleep(0);
			continue;
		}

		// Check if this order was cancelled (ASyncCancelInject)
		if(Info->Deleted)
		{
			// Push it back to the main thread for deletion and cleanup
			OutQueue.Push(Info);
			continue;
		}


		// Generate indices
		NxU32* Indices = new NxU32[Info->BufferLength];
		for(unsigned int i = 0; i < Info->BufferLength; ++i)
			Indices[i] = i;

		// Setup the mesh
		NxTriangleMeshDesc MeshDesc;
		MeshDesc.numVertices = Info->BufferLength;
		MeshDesc.numTriangles = Info->BufferLength / 3;
		MeshDesc.pointStrideBytes = sizeof(NxVec3);
		MeshDesc.triangleStrideBytes = 3 * sizeof(NxU32);
		MeshDesc.points = Info->Buffer;
		MeshDesc.triangles = Indices;
		MeshDesc.flags = 0;

		// Cook the mesh
		MemoryWriteBuffer* WriteBuffer = new MemoryWriteBuffer();
		if(!CookTriangleMesh(MeshDesc, *WriteBuffer))
		{
			delete WriteBuffer;
			Info->WriteBuffer = NULL;
		}else
		{
			Info->WriteBuffer = WriteBuffer;
		}

		delete[] Indices;

		// Order complete, send it back to the main thread
		OutQueue.Push(Info);
	}
}*/

int CookTask(ASyncJobDesc* jobDesc)
{
	CookInfo* Info = (CookInfo*)jobDesc->UserData;

	// Generate indices
	NxU32* Indices = new NxU32[Info->BufferLength];
	for(unsigned int i = 0; i < Info->BufferLength; ++i)
		Indices[i] = i;

	// Setup the mesh
	NxTriangleMeshDesc MeshDesc;
	MeshDesc.numVertices = Info->BufferLength;
	MeshDesc.numTriangles = Info->BufferLength / 3;
	MeshDesc.pointStrideBytes = sizeof(NxVec3);
	MeshDesc.triangleStrideBytes = 3 * sizeof(NxU32);
	MeshDesc.points = Info->Buffer;
	MeshDesc.triangles = Indices;
	MeshDesc.flags = 0;

	// Cook the mesh
	MemoryWriteBuffer* WriteBuffer = new MemoryWriteBuffer();
	if(!CookTriangleMesh(MeshDesc, *WriteBuffer))
	{
		delete WriteBuffer;
		Info->WriteBuffer = NULL;
	}else
	{
		Info->WriteBuffer = WriteBuffer;
	}

	delete[] Indices;

	return 1;
}

int CookSync(ASyncJobDesc* jobDesc)
{
	CookInfo* Info = (CookInfo*)jobDesc->UserData;

	// Free existing configuration
	if(Info->Node->NxHandle != 0)
		bbdx2_FreeCollisionInstance(Info->Node);

	// Check if it failed
	if(Info->WriteBuffer == 0)
	{
		OutputDebugString("ThreadCooking failed!\n");
	}else
	{
		// Setup shape
		NxTriangleMeshShapeDesc* MeshShape = new NxTriangleMeshShapeDesc();

		MemoryReadBuffer ReadBuffer(Info->WriteBuffer->data);

		NxTriangleMesh* Mesh = gPhysicsSDK->createTriangleMesh(ReadBuffer);

		// Setup shape
		MeshShape->meshData = Mesh;

		NxActorDesc MeshActor;
		MeshActor.shapes.pushBack(MeshShape);

		// Create Actor
		NxActor* Actor = gScene->createActor(MeshActor);

		for(irr::u32 i = 0; i < MeshActor.shapes.size(); ++i)
			delete MeshActor.shapes[i];

		if(Actor == 0)
		{
			Info->Node->NxHandle = 0;
			Info->Node->NxHandleType = 0;
			bbdx2_SetNxType(Info->Node);
		}else
		{
			// Important data
			Actor->userData = Info->Node;
			Info->Node->NxHandle = Actor;
			Info->Node->NxHandleType = 3;
			Info->Node->TriMeshArray.push_back(Mesh);
			bbdx2_SetNxType(Info->Node);
			bbdx2_UpdateStaticPose(Info->Node);
		}
	}

	// Remove thread lock status
	Info->Node->LockedInThread = false;

	// If FreeEntity() was called while it was locked, then delete it
	if(Info->Node->LockedDelete)
		getmeshake(Info->Node);

	delete[] Info->Buffer;
	if(Info->WriteBuffer != 0)
		delete Info->WriteBuffer;
	delete Info;

	JobDone = true;
	return 1;
}

// Cook a mesh
DLLPRE bool bbdx2_ASyncInjectCollisionMesh(irr::scene::ISceneNode* node, void* triangleList, unsigned int vertexCount, int priority)
{
	// If the node is null or is already being processed
	if(node == 0)
		return false;
	if(node->LockedInThread)
		return false;

	// Setup the cooking data
	CookInfo* Info = new CookInfo();
	Info->Node = node;
	Info->Buffer = new char[vertexCount * 12];
	memcpy(Info->Buffer, triangleList, vertexCount * 12);
	Info->BufferLength = vertexCount;
	Info->Node->LockedInThread = true;
	Info->Priority = priority > 0 ? PHYSX_PRIORITY_HIGH : PHYSX_PRIORITY_LOW;

	CookList.push_back( Info );


	// Add our order
// 	if(priority > 0)
// 		InQueue1.Push(Info);
// 	else
// 		InQueue2.Push(Info);
	return true;
}

bool CookInfoSearchPredicate(CookInfo* &info, void* args)
{
	if(info == NULL || args == NULL)
		return false;

	return (info->Node == args);
}

// Clear an existing mesh if it has been cooked
DLLPRE void bbdx2_ASyncCancelInject(irr::scene::ISceneNode* node)
{
	for(std::list<CookInfo*>::iterator it = CookList.begin(); it != CookList.end(); ++it)
	{
		if( (*it)->Node == node )
		{
			CookInfo* Info = *it;

			delete[] Info->Buffer;
			Info->Buffer = NULL;
			Info->BufferLength = 0;
			Info->Node = NULL;

			if(Info->WriteBuffer != NULL)
				delete Info->WriteBuffer;

			CookList.remove(Info);
			node->LockedInThread = false;

			delete Info;
			break;
		}
	}

// 	std::list<CookInfo*> Deleted;
// 	InQueue1.Remove(&CookInfoSearchPredicate, node, Deleted);
// 	InQueue2.Remove(&CookInfoSearchPredicate, node, Deleted);

//  	char DBO[512];
//  	sprintf(DBO, "Cancel: %x - %i\n", node, Deleted.size());
//  	OutputDebugStringA(DBO);

// 	for(std::list<CookInfo*>::iterator It = Deleted.begin(); It != Deleted.end(); ++It)
// 	{
// 		(*It)->Deleted = true;
// 	}

// 	// If an instance was removed, then it means the async thread isn't holding it.
// 	// Otherwise, we must assume that its either being processed or waiting to be
// 	// completed.
// 	if(Deleted.size() > 0)
// 	{
// 		node->LockedInThread = false;
// 	}
// 
// 	Deleted.clear();

	
	

}

// Called from UpdateWorld() to check for completed orders
void ASyncInjectCompleteUpdate()
{
	// Nothing in the task queue, insert!
	if( JobDone && CookList.size() > 0 )
	{
		CookInfo* info = CookList.front();
		CookList.pop_front();

		// Use new async lib
		bbdx2_ASyncInsertJob( PHYSX_TYPEID, &CookTask, &CookSync, info, info->Priority );
		JobDone = false;
	}

	/*
	// While there are orders left
	int ProcCount = 0;
	while(OutQueue.GetSize() > 0)
	{
		CookInfo* Info = OutQueue.Pop();
		if(Info == 0)
			continue;

		// If it was removed prematurely
		if(Info->Deleted)
		{
			delete[] Info->Buffer;
			Info->Buffer = NULL;
			Info->BufferLength = 0;
			Info->Node = NULL;

			if(Info->WriteBuffer != NULL)
				delete Info->WriteBuffer;

			delete Info;
			continue;
		}

		// Free existing configuration
		if(Info->Node->NxHandle != 0)
			bbdx2_FreeCollisionInstance(Info->Node);

		// Check if it failed
		if(Info->WriteBuffer == 0)
		{
			OutputDebugString("ThreadCooking failed!\n");
		}else
		{
			// Setup shape
			NxTriangleMeshShapeDesc* MeshShape = new NxTriangleMeshShapeDesc();

			MemoryReadBuffer ReadBuffer(Info->WriteBuffer->data);

			//unsigned int S = timeGetTime();
			NxTriangleMesh* Mesh = gPhysicsSDK->createTriangleMesh(ReadBuffer);
// 			unsigned int E = timeGetTime();
// 
// 			char DBO[1024];
// 			sprintf(DBO, "createTriangleMesh() took %ims\n", (E - S));
// 			OutputDebugString(DBO);

			// Setup shape
			MeshShape->meshData = Mesh;

			NxActorDesc MeshActor;
			MeshActor.shapes.pushBack(MeshShape);

			// Create Actor
			NxActor* Actor = gScene->createActor(MeshActor);

			for(irr::u32 i = 0; i < MeshActor.shapes.size(); ++i)
				delete MeshActor.shapes[i];

			if(Actor == 0)
			{
				Info->Node->NxHandle = 0;
				Info->Node->NxHandleType = 0;
				bbdx2_SetNxType(Info->Node);
			}else
			{
// 				char DBO[512];
// 				sprintf(DBO, "Sync Complete: %x\n", Info->Node);
// 				OutputDebugStringA(DBO);

				// Important data
				Actor->userData = Info->Node;
				Info->Node->NxHandle = Actor;
				Info->Node->NxHandleType = 3;
				Info->Node->TriMeshArray.push_back(Mesh);
				bbdx2_SetNxType(Info->Node);
				bbdx2_UpdateStaticPose(Info->Node);
			}
		}

		// Remove thread lock status
		Info->Node->LockedInThread = false;

		// If FreeEntity() was called while it was locked, then delete it
		if(Info->Node->LockedDelete)
			getmeshake(Info->Node);


		delete[] Info->Buffer;
		if(Info->WriteBuffer != 0)
			delete Info->WriteBuffer;
		delete Info;

		++ProcCount;
		if(ProcCount == 16)
			break;
	}

	if(DebuggerConnected())
	{
		char DBO[256];
		sprintf(DBO, "PhysX Thread: %i\n", InQueue1.GetSize() + InQueue2.GetSize());
		StaticDebugStr(3, DBO);
	}else
	{
// 		char DBO[256];
// 		sprintf(DBO, "PhysX Thread: %i\n", InQueue.GetSize());
// 
// 		SetWindowTextA(GetActiveWindow(), DBO);
	}

	*/
}