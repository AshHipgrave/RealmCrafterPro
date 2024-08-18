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

// Includes
#include <d3dx9.h>
#include <Vector2.h>
#include <Vector3.h>
#include <AABB.h>
#include <STerrainVertex.h>
#include "CTerrain.h"
#include "EChunkState.h"
#include <ArrayList.h>
#include "LOD.h"

namespace RealmCrafter
{
	namespace RCT
	{

		// Declarations
		class CTerrain;
		struct T1Data;

		// IndexBuffer structure
		struct IndexBuffer
		{
			// Handle of D3D buffer
			IDirect3DIndexBuffer9* Buffer;
			unsigned short* Indices;

			// Number of indices
			int IndexCount;

			IndexBuffer()
			{
				Buffer = 0;
				IndexCount = 0;
				Indices = 0;
			}

			~IndexBuffer()
			{
				// Free buffer
				Buffer->Release();
				Buffer = 0;
				if(Indices != 0)
					delete[] Indices;
				Indices = 0;
			}
		};

		// Terrain Tile Class
		class CChunk
		{
		protected:

			unsigned char LOD;					// Current level of detail
			unsigned char Sides;				// Bitmask of side LOD (Adjacent LOD chunks)
			EChunkState State;					// State of chunk
			unsigned char GrassInvalidation;

			CTerrain* Terrain;					// Parent
			IDirect3DDevice9* Device;			// Device handle
			IDirect3DVertexBuffer9* Vertices;	// Chunk Vertices
			IDirect3DTexture9* SplatTex;		// Chunks 32x32 or 16x16 texture
			IDirect3DTexture9* HoleTex;			// Texture to paint holes
			unsigned char TexLOD;				// Texture level of detail
			int VertexCount;					// Number of vertices
			T1Data* Data;						// Array containing terrain data
			NGin::ArrayList<int> ExclusionData;		// Array of vertices to be excluded from rendering
			RCT::LOD* LOD4_;
			RCT::LOD* LOD3_;
			
			bool GrassActive;

			NGin::Math::AABB BoundingBox;		// Bounding box of chunk

//			NGin::Math::Vector2 ChunkPosition;	// Position of chunk
			NGin::Math::Vector3 LastPosition;	// Last position of the camera

			// Build a normal for a vertex
			void CalculateNormal(int x, int y, T1Vertex* Vertex);

			// Rebuild collision data
			void ReBuildCollisions(bool highDetail);

			

			// Rebuild the vertex buffer
			void ReBuild();

			// Recalculate the bounding box
			void RecalculateBoundingBox();

			bool DELSENT;

		public:

			NGin::Math::Vector2 ChunkPosition;	// Position of chunk

			bool CollisionsActive;

			CChunk(CTerrain* terrain, IDirect3DDevice9* device, NGin::Math::Vector2 &position, T1Data* data, RCT::LOD* lod4, RCT::LOD* lod3);
			~CChunk();

			// Clear collision data
			void ClearCollisions();

			// Device was lost
			virtual void OnDeviceLost();

			// Device was reset
			virtual void OnDeviceReset();

			// Gets current LOD
			unsigned char GetLODLevel();

			// Gets the bounding box
			NGin::Math::AABB GetBoundingBox();

			// Gets exclusion value of position
			unsigned char GetExclusion(int x, int y);

			// Set exclusion of a position
			void SetExclusion(int x, int y, bool exclude);

			// Gets the height
			float GetHeight(int x, int y);

			// Gets the color
			T1TexCol GetColor(int x, int y);

			// Get terrain
			CTerrain* GetTerrain();

			// Sets the color
			void SetColor(int x, int y, T1TexCol color);

			// Sets the height
			void SetHeight(int x, int y, float height);

			// Sets the grass bitmask
			void SetGrass(int x, int y, unsigned char grass);

			// Gets the grass
			unsigned char GetGrass(int x, int y);

			// First update command to calculate LOD level
			void UpdateLOD(NGin::Math::Vector3& position, bool updateCollisions, int &updateCount);

			// Second update command to rebuild based upon adjacent chunks
			void UpdateSides(unsigned char sides);

			// Render this chunk
			void Render(IndexBuffer**** Buffers, ID3DXEffect* effect, FrameStatistics &stats, bool isDepth, NGin::Math::Vector3 &offset);

			// Statis members
			static int PatchSize[5];
			static int PatchSkip[5];
			static float PatchScale[5];
			static float LODDistances[5];
		};
	}
}
