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
#include "RCT.h"
#include <BlitzPlus.h>
#include <NGinString.h>
#include "ITimerManager.h"
#include <ITerrainManager.h>
#include "Default Project.h"

void Terrain_CollisionChanged(ITerrain* terrain, RealmCrafter::RCT::CollisionEventArgs* e)
{
	std::vector<TerrainTagItem*>* TerrainTag = (std::vector<TerrainTagItem*>*)terrain->GetTag();
	if(TerrainTag == 0)
	{
		DebugLog("Terrain had no Tag, cannot set collision data");
		return;
	}

	// Get world coordinate of chunk
	NGin::Math::Vector3 ChunkPos = e->GetPosition();
	ChunkPos += terrain->GetPosition();
	NGin::Math::Vector3 WorldPosition = ChunkPos;

	float CollisionENX = 0.0f;
	float CollisionENZ = 0.0f;

	if(Me != NULL)
	{
		ChunkPos.X -= (RealmCrafter::SectorVector::SectorSize * Me->Position.SectorX);
		ChunkPos.Z -= (RealmCrafter::SectorVector::SectorSize * Me->Position.SectorZ);
		CollisionENX = EntityX(Me->CollisionEN, true);
		CollisionENZ = EntityZ(Me->CollisionEN, true);
	}

	bool Found = false;
	for(int i = 0; i < TerrainTag->size(); ++i)
	{
		// Found the correct tag
		if(    fabs((*TerrainTag)[i]->Position.X - WorldPosition.X) < 5.0f
			&& fabs((*TerrainTag)[i]->Position.Z - WorldPosition.Z) < 5.0f)
		{
			// RCT wants to delete it
			if(e->GetTriangleList() == 0)
			{
				(*TerrainTag)[i]->Active = false;
				bbdx2_ASyncCancelInject((*TerrainTag)[i]->Mesh);
				bbdx2_FreeCollisionInstance((*TerrainTag)[i]->Mesh);

				FreeEntity((*TerrainTag)[i]->Mesh);
				delete (*TerrainTag)[i];
				(*TerrainTag)[i] = NULL;

				TerrainTag->erase(TerrainTag->begin() + i);
				return;
			}
			else // Make it!
			{
				(*TerrainTag)[i]->Active = true;

				// See if its within 200 units (1.5 tiles is 192)
				if(ChunkPos.X > CollisionENX - 200.0f && ChunkPos.X < CollisionENX + 200.0f
					&& ChunkPos.Z > CollisionENZ - 200.0f && ChunkPos.Z < CollisionENZ + 200.0f)
				{
					//printf("Put: %f, %f;\n", ChunkPos.X, ChunkPos.Z);
					bbdx2_InjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount());
				}else
				{
					bbdx2_ASyncInjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount(), e->GetHighPriority() ? 1 : 0);
				}
			}

			// Reposition it
			PositionEntity((*TerrainTag)[i]->Mesh, ChunkPos.X, ChunkPos.Y, ChunkPos.Z);
			Found = true;
			break;
		}
	}

	if(!Found)
	{
		// RCT wants to delete it
		if(e->GetTriangleList() == 0)
		{
			return;
			//bbdx2_ASyncCancelInject(Mesh);
			//bbdx2_FreeCollisionInstance(Mesh);
		}
		else // Make it!
		{
			uint Mesh = CreateMesh();
			TagEntity(Mesh, "TERRAIN");
			HideEntity(Mesh);

			// See if its within 200 units (1.5 tiles is 192)
			if(ChunkPos.X > CollisionENX - 200.0f && ChunkPos.X < CollisionENX + 200.0f
				&& ChunkPos.Z > CollisionENZ - 200.0f && ChunkPos.Z < CollisionENZ + 200.0f)
			{
				//printf("Put: %f, %f;\n", ChunkPos.X, ChunkPos.Z);
				bbdx2_InjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount());
			}else
			{
				bbdx2_ASyncInjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount(), e->GetHighPriority() ? 1 : 0);
			}

			PositionEntity(Mesh, ChunkPos.X, ChunkPos.Y, ChunkPos.Z);
			EntityType(Mesh, 2);
			EntityPickMode(Mesh, 2);

			TerrainTagItem* TagItem = new TerrainTagItem();
			TagItem->Active = e->GetTriangleList() != NULL;
			TagItem->Mesh = Mesh;
			TagItem->Position = WorldPosition;
			TerrainTag->push_back(TagItem);
		}
	}
}

void Terrain_CollisionChangedOLD(ITerrain* terrain, CollisionEventArgs* e)
{
	//return;
	ArrayList<TerrainTagItem*>* TerrainTag = (ArrayList<TerrainTagItem*>*)terrain->GetTag();
	if(TerrainTag == 0)
	{
		DebugLog("Terrain had no Tag, cannot set collision data");
		return;
	}

	float MeX = 0.0f;
	float MeZ = 0.0f;
	float CollisionENX = 0.0f;
	float CollisionENZ = 0.0f;

	if(Me != NULL)
	{
		MeX = Me->Position.X + (Me->Position.SectorSize * Me->Position.SectorX);
		MeZ = Me->Position.Z + (Me->Position.SectorSize * Me->Position.SectorZ);
		CollisionENX = EntityX(Me->CollisionEN, true);
		CollisionENZ = EntityZ(Me->CollisionEN, true);
	}

	bool Found = false;
	for(int i = 0; i < TerrainTag->Size(); ++i)
	{
		if(((int)(*TerrainTag)[i]->Position.X) == ((int)e->GetPosition().X)
			&& ((int)(*TerrainTag)[i]->Position.Z) == ((int)e->GetPosition().Z))
		{
			if(e->GetTriangleList() == 0)
			{
				bbdx2_ASyncCancelInject((*TerrainTag)[i]->Mesh);
				bbdx2_FreeCollisionInstance((*TerrainTag)[i]->Mesh);
			}
			else
			{
				if(e->GetPosition().X > MeX - 192.0f && e->GetPosition().X < MeX + 192.0f
					&& e->GetPosition().Z > MeZ - 192.0f && e->GetPosition().Z < MeZ + 192.0f)
				{
					bbdx2_InjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount());
				}else
				{
					if(e->GetPosition().X > CollisionENX - 192.0f && e->GetPosition().X < CollisionENX + 192.0f
						&& e->GetPosition().Z > CollisionENZ - 192.0f && e->GetPosition().Z < CollisionENZ + 192.0f)
					{
						bbdx2_InjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount());
					}else
					{
						bbdx2_ASyncInjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount(), e->GetHighPriority() ? 1 : 0);
						//bbdx2_InjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount());
					}
				}
			}
			PositionEntity((*TerrainTag)[i]->Mesh, e->GetPosition().X + terrain->GetPosition().X, e->GetPosition().Y + terrain->GetPosition().Y, e->GetPosition().Z + terrain->GetPosition().Z);

			Found = true;
		}
	}

	if(!Found)
	{

		uint Mesh = CreateMesh();
		TagEntity(Mesh, "TERRAIN");



		if(e->GetTriangleList() == 0)
		{
			bbdx2_ASyncCancelInject(Mesh);
			bbdx2_FreeCollisionInstance(Mesh);
		}
		else
		{
			// If a tile is beneath the actor, it must be pushed immediately otherwise he'll fall through
			if(e->GetPosition().X > MeX - 192.0f && e->GetPosition().X < MeX + 192.0f
				&& e->GetPosition().Z > MeZ - 192.0f && e->GetPosition().Z < MeZ + 192.0f)
			{
				bbdx2_InjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount());
			}else
			{
				if(e->GetPosition().X > CollisionENX - 192.0f && e->GetPosition().X < CollisionENX + 128.0f
					&& e->GetPosition().Z > CollisionENZ - 192.0f && e->GetPosition().Z < CollisionENZ + 128.0f)
				{
					bbdx2_InjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount());
				}else
				{
					bbdx2_ASyncInjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount(), e->GetHighPriority() ? 1 : 0);
					//bbdx2_InjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount());
				}
			}
		}


		PositionEntity(Mesh, e->GetPosition().X + terrain->GetPosition().X, e->GetPosition().Y + terrain->GetPosition().Y, e->GetPosition().Z + terrain->GetPosition().Z);
		EntityType(Mesh, 2);
		EntityPickMode(Mesh, 2);

		TerrainTagItem* TagItem = new TerrainTagItem();
		TagItem->Mesh = Mesh;
		TagItem->Position = e->GetPosition();
		TerrainTag->Add(TagItem);
	}
}