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

#include <vector2.h>
#include <vector3.h>
#include <ITerrainManager.h>

namespace RealmCrafter
{
	namespace RCT
	{

		//! FrameStatistics structure
		struct FrameStatistics
		{
			//! Number of triangles drawn this frame
			unsigned int TrianglesDrawn;

			//! Number of terrain tiles (32x32) drawn
			unsigned int ChunksDrawn;

			//! Number of terrain files (32x32) culled
			unsigned int ChunksCulled;

			FrameStatistics()
			{
				TrianglesDrawn = ChunksDrawn = ChunksCulled = 0;
			}
		};

		struct T1TexCol;

		//! Terrain interface
		class ITerrain
		{
		public:

			ITerrain() {}
			virtual ~ITerrain() {}

			//! Set the position of the terrain
			virtual void SetPosition(NGin::Math::Vector3 &position) = 0;

			//! Get the position of the terrain
			virtual NGin::Math::Vector3 GetPosition() = 0;

			//! Get the size of the terrain (used on its creation)
			virtual int GetSize() = 0;

			//! Set the scale of the terrain texture
			virtual void SetTextureScale(int index, float scale) = 0;

			//! Get the scale of the terrain texture
			virtual float GetTextureScale(int index) = 0;

			//! Set the texture of a certain channel
			/*!
			\param Index Channel Index (0-4)
			\param Path Path to texture file
			*/
			virtual bool SetTexture(int index, std::string& path) = 0;

			//! Set the name of a grass type as a particular index
			virtual void SetGrassType(int index, std::string type) = 0;

			//! Get the name of a grass type at a particular index
			virtual std::string GetGrassType(int index) = 0;

			//! Get the texture path of a certain channel
			virtual std::string GetTexture(int index) = 0;

			//! Set the grass mask of a position
			virtual void SetGrass(int x, int y, unsigned int grass) = 0;

			//! Get the grass mask of a position
			virtual unsigned char GetGrass(int x, int y) = 0;

			//! Set exclusion of a position
			virtual void SetExclusion(int x, int y, bool exclude) = 0;

			//! Get exclusion of a position
			virtual bool GetExclusion(int x, int y) = 0;

			//! Set the height of the terrain
			virtual void SetHeight(int x, int y, float height) = 0;

			//! Set the height of the terrain
			virtual void SetHeight(int chunkX, int chunkY, int x, int y, float height) = 0;

			//! Get the height of the terrain
			virtual float GetHeight(int x, int y) = 0;

			//! Get the height of the terrain
			virtual float GetHeight(int chunkX, int chunkY, int x, int y) = 0;
			
			//! Get the color of the terrain
			virtual T1TexCol GetColorChunk(NGin::Math::Vector2 &position, int x, int y) = 0;

			//! Set the color of the terrain
			virtual void SetColorChunk(NGin::Math::Vector2 &position, int x, int y, T1TexCol color) = 0;

			//! Set a void* tag for this terrain
			virtual void SetTag(void* tag) = 0;

			//! Get a void* tag for this terrain
			virtual void* GetTag() = 0;
		};

		void Transform_Raise(ITerrain* terrain, float radius, float hardness, NGin::Math::Vector2 position, bool circular, float change);
		void Transform_Smooth(ITerrain* terrain, float radius, float hardness, NGin::Math::Vector2 position, bool circular, float change);
		void Transform_SetHeight(ITerrain* terrain, float radius, float hardness, NGin::Math::Vector2 position, bool circular, float height);
		void Transform_Paint(ITerrain* terrain, float radius, float hardness, NGin::Math::Vector2 position, bool circular, float strength, int texture, float min, float max, float minHeight, float maxHeight);
		void Transform_PaintHole(ITerrain* Terrain, float Radius, NGin::Math::Vector2 Position, bool Circular, bool Erase);
		void Transform_PaintGrass(ITerrain* Terrain, float Radius, NGin::Math::Vector2 Position, bool Circular, unsigned char grassMask, bool additive);
		void Transform_Ramp(ITerrain* Terrain, NGin::Math::Vector2 startPosition, NGin::Math::Vector2 endPosition, float startWidth, float endWidth);
	}
}

