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
#include "LOD.h"
#include "CChunk.h"
#include <STerrainVertex.h>

namespace RealmCrafter
{
namespace RCT
{

LOD::LOD( IDirect3DDevice9* device, int chunkSize)
	: device_(device),
	chunksInUse_(0),
	dirty_(false),
	chunkSize_(chunkSize)
{
	memset( registeredChunks_, 0, sizeof(registeredChunks_) );

	int vertexCount = chunkSize_ * chunkSize_ * 6;
	device->CreateVertexBuffer( MAX_REGISTERED_CHUNKS * vertexCount * sizeof(T1VertexLOD), D3DUSAGE_WRITEONLY | D3DUSAGE_DYNAMIC, NULL, D3DPOOL_DEFAULT, &vertices_, NULL);
}

LOD::~LOD()
{
	if( vertices_ )
		vertices_->Release();
	vertices_ = NULL;
}

void LOD::OnDeviceLost()
{
	if( vertices_ )
		vertices_->Release();
	vertices_ = NULL;
}

void LOD::OnDeviceReset()
{
	int vertexCount = chunkSize_ * chunkSize_ * 6;
	device_->CreateVertexBuffer( MAX_REGISTERED_CHUNKS * vertexCount * sizeof(T1VertexLOD), D3DUSAGE_WRITEONLY | D3DUSAGE_DYNAMIC, NULL, D3DPOOL_DEFAULT, &vertices_, NULL);

	dirty_ = true;
}

void LOD::Update()
{
	int vertexCount = chunkSize_ * chunkSize_ * 6;

	if( dirty_ )
	{
		// Rebuild
		T1VertexLOD* vertices = NULL;

		int lockSize = chunksInUse_ * vertexCount * sizeof(T1VertexLOD);
		vertices_->Lock(0, lockSize, (void**)&vertices, D3DLOCK_DISCARD);

		for(int i = 0; i < chunksInUse_; ++i)
		{
			int written = BuildChunk( registeredChunks_[i], vertices );

			vertices += written;
		}

		vertices_->Unlock();
		dirty_ = false;
	}
}

void LOD::Draw( ID3DXEffect* effect )
{
	(void)effect;

	int primitiveCount = chunkSize_ * chunkSize_ * 2;

	if( !device_ )
		return;

	device_->SetStreamSource(0, vertices_, 0, sizeof(T1VertexLOD));

	// Draw chunk
	device_->DrawPrimitive(D3DPT_TRIANGLELIST, 0, chunksInUse_ * primitiveCount);
}

inline void CalculateNormal( D3DXVECTOR3* outNormal, T1VertexLOD* center, T1VertexLOD* north, T1VertexLOD* south, T1VertexLOD* east, T1VertexLOD* west )
{
	D3DXVECTOR3* C = (D3DXVECTOR3*)center;
	D3DXVECTOR3* N = (D3DXVECTOR3*)north;
	D3DXVECTOR3* S = (D3DXVECTOR3*)south;
	D3DXVECTOR3* E = (D3DXVECTOR3*)east;
	D3DXVECTOR3* W = (D3DXVECTOR3*)west;

	D3DXPLANE NE, NW, SE, SW;

	D3DXPlaneFromPoints( &NE, E, N, C );
	D3DXPlaneFromPoints( &NW, C, N, W );
	D3DXPlaneFromPoints( &SE, C, S, E );
	D3DXPlaneFromPoints( &SW, E, C, S );

	memset( outNormal, 0, sizeof(D3DXVECTOR3));
	D3DXVec3Add( outNormal, (D3DXVECTOR3*)&NE, (D3DXVECTOR3*)&NW );
	D3DXVec3Add( outNormal, outNormal, (D3DXVECTOR3*)&SE );
	D3DXVec3Add( outNormal, outNormal, (D3DXVECTOR3*)&SW );
	D3DXVec3Normalize( outNormal, outNormal );
}

int LOD::BuildChunk( CChunk* chunk, T1VertexLOD* vertices )
{
	CTerrain* terrain = chunk->GetTerrain();
	NGin::Math::Vector2 chunkPosition = chunk->ChunkPosition;

	T1VertexLOD heightMatrix[12][12];
	int chunkStep = 64 / chunkSize_;
	float fChunkStep = (float)chunkStep;

	int yPos = -chunkStep;
	float fY = -fChunkStep;

	for(int y = 0; y < chunkSize_ + 3; ++y)
	{
		int xPos = -chunkStep;
		float fX = -fChunkStep;

		for(int x = 0; x < chunkSize_ + 3; ++x)
		{
			heightMatrix[x][y].X = fX + chunkPosition.X;
			heightMatrix[x][y].Z = fY + chunkPosition.Y;
			heightMatrix[x][y].U = fX;
			heightMatrix[x][y].V = fY;
			heightMatrix[x][y].Y = terrain->GetHeightChunk( chunkPosition, xPos, yPos );
			heightMatrix[x][y].Splat = terrain->GetColorChunk( chunkPosition, xPos, yPos );

			xPos += chunkStep;
			fX += fChunkStep;
		}

		yPos += chunkStep;
		fY += fChunkStep;
	}

	// Generate Normals
	for(int y = 1; y < chunkSize_ + 2; ++y)
	{
		for(int x = 1; x < chunkSize_ + 2; ++x)
		{
			D3DXVECTOR3 normal;

			CalculateNormal( &normal, &heightMatrix[x][y], &heightMatrix[x][y - 1], &heightMatrix[x][y + 1], &heightMatrix[x + 1][y], &heightMatrix[x - 1][y] );

			heightMatrix[x][y].NX = normal.x;
			heightMatrix[x][y].NY = normal.y;
			heightMatrix[x][y].NZ = normal.z;
		}
	}

	// Build buffer
	for(int y = 1; y < chunkSize_ + 1; ++y)
	{
		for(int x = 1; x < chunkSize_ + 1; ++x)
		{
			// Tri 0
			vertices[2] = heightMatrix[x][y];
			vertices[1] = heightMatrix[x + 1][y];
			vertices[0] = heightMatrix[x][y + 1];

			vertices[5] = heightMatrix[x + 1][y];
			vertices[4] = heightMatrix[x + 1][y + 1];
			vertices[3] = heightMatrix[x][y + 1];

			vertices += 6;
		}
	}

	return chunkSize_ * chunkSize_ * 6;
}

void LOD::AddChunk( CChunk* chunk )
{
	// Don't re-add
	if( FindChunk( chunk ) != -1 )
		return;

	if( chunksInUse_ == MAX_REGISTERED_CHUNKS )
		return;

	registeredChunks_[chunksInUse_++] = chunk;
	dirty_ = true;
}

void LOD::RemoveChunk( CChunk* chunk )
{
	int pos = FindChunk( chunk );
	if( pos == -1 )
		return;

	for(int i = pos; i < chunksInUse_ - 1; ++i)
	{
		registeredChunks_[i] = registeredChunks_[i + 1];
	}
	chunksInUse_--;

	dirty_ = true;
}

int LOD::FindChunk( CChunk* chunk )
{
	for(int i = 0; i < chunksInUse_; ++i)
	{
		if( registeredChunks_[i] == chunk )
			return i;
	}

	return -1;
}

}
}