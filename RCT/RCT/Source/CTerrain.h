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

namespace RealmCrafter
{
	namespace RCT
	{
		class CQuadTree;
	}
}

// Includes
#include <d3dx9.h>
#include <ArrayList.h>
#include <List.h>
#include <Vector3.h>
#include <Vector2.h>
#include "ITerrain.h"
#include "CChunk.h"
#include "CTerrainManager.h"
#include "CQuadTree.h"
#include "LOD.h"

namespace RealmCrafter
{
	namespace RCT
	{


		// Declarations
		class CChunk;
		class CTerrainManager;
		struct IndexBuffer;
		struct T1TexCol;
		

		// Terrain class
		class CTerrain : public ITerrain
		{
		protected:

			friend class CChunk;

			// Device handle
			IDirect3DDevice9* Device;
			CTerrainManager* Manager;
			NGin::Math::Vector3 Position;
			bool PositionChanged;
			RCT::LOD* LOD4_;
			RCT::LOD* LOD3_;

			// Two dimensional array of tiles
			NGin::ArrayList<NGin::ArrayList<CChunk*> > Chunks;
			CQuadTree QuadTree;

			// Texture structure for this terrain
			struct TerrainTexture
			{
				// Texture handle
				IDirect3DTexture9* Handle;

				// Texture path
				std::string Path;

				TerrainTexture()
				{
					Handle = 0;
				}

				~TerrainTexture()
				{
					// Free texture if necessary
					if(Handle != 0)
						Handle->Release();
					Handle = 0;
				}
			};

			// Textures for this terrain
			TerrainTexture* Textures[5];
			float TextureScales[5];

			// Grasses for this terrain
			IGrassType* Grasses[8];

			// Tag
			void* Tag;

			// Builds an index buffer for different LOD setups
			void BuildIndices(int lod, char sides, IndexBuffer* masterIndexBuffer);

		public:

			CTerrain(CTerrainManager* manager, IDirect3DDevice9* device, int segments);
			~CTerrain();

			// Set the position of the terrain
			virtual void SetPosition(NGin::Math::Vector3 &position);

			// Get the position of the terrain
			virtual NGin::Math::Vector3 GetPosition();

			// Get terrain manager
			virtual ITerrainManager* GetManager();

			// Device was lost
			virtual void OnDeviceLost();

			// Device was reset
			virtual void OnDeviceReset();

			// Update terrain (Recalculate chunk LOD)
			void Update(NGin::Math::Vector3& position, bool updateCollisions);

			// Render terrain
			void Render(int rtIndex, ID3DXEffect* effect, FrameStatistics &stats, D3DXMATRIX* viewProjection, float* renderBox, std::list<CChunk*>* renderChunks, bool isDepth, NGin::Math::Vector3 &offset);

			// Render LODS
			void RenderLOD(ID3DXEffect* effect);

			// Get the size of the terrain (used on its creation)
			virtual int GetSize();

			// Set the scale of the terrain texture
			virtual void SetTextureScale(int index, float scale);

			// Get the scale of the terrain texture
			virtual float GetTextureScale(int index);

			// Set texture for index
			virtual bool SetTexture(int Index, std::string& path);

			// Get texture for index
			virtual std::string GetTexture(int index);

			virtual void SetGrassType(int index, std::string type);
			virtual std::string GetGrassType(int index);
			virtual int GetGrassIndex(IGrassType* type);

			// Set exclusion of a position
			virtual void SetExclusion(int x, int y, bool exclude);

			// Get exclusion of a position
			virtual bool GetExclusion(int x, int y);

			// Set the grass mask of a position
			virtual void SetGrass(int x, int y, unsigned int grass);

			// Get the grass mask of a position
			virtual unsigned char GetGrass(int x, int y);

			// Set global height
			virtual void SetHeight(int x, int y, float height);

			// Set local height
			virtual void SetHeight(int chunkX, int chunkY, int x, int y, float height);

			// Get global height
			virtual float GetHeight(int x, int y);

			// Get local height
			virtual float GetHeight(int chunkX, int chunkY, int x, int y);

			// Get half height
			float GetHeightChunk(NGin::Math::Vector2 &position, int x, int y);

			// Get color
			virtual T1TexCol GetColorChunk(NGin::Math::Vector2 &position, int x, int y);
			
			// Set color
			virtual void SetColorChunk(NGin::Math::Vector2 &position, int x, int y, T1TexCol color);

			// Set a void* tag for this terrain
			virtual void SetTag(void* tag);

			// Get a void* tag for this terrain
			virtual void* GetTag();
			
			// IndexBuffer*[][] For LOD and Sides
			static IndexBuffer*** Buffers;

			// Default terrain textures
			static IDirect3DTexture9* DefaultTextures[5];

			// No hole texture
			static IDirect3DTexture9* NoHoleTexture;

			// Remove all useless data (terrain manager died)
			static void CleanUp();
		};
	}
}
