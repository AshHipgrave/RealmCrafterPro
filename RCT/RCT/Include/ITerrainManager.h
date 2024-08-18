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
#include <ArrayList.h>
#include <ITerrain.h>
#include <STerrainVertex.h>
#include <Vector3.h>
#include <Color.h>
#include <IEventHandler.h>
#include <IGrassType.h>

namespace RealmCrafter
{
	namespace RCT
	{
		// Declarations
		class ITerrain;
		struct FrameStatistics;

		//! Event arguments structure used when a terrain needs its collision mesh created
		class CollisionEventArgs
		{
		protected:

			float* TriangleList;
			NGin::Math::Vector3 Position;
			NGin::Math::Vector3 TPosition;
			unsigned int VertexCount;
			bool priority;

		public:

			CollisionEventArgs(float* triangleList, NGin::Math::Vector3 position, unsigned int vertexCount, NGin::Math::Vector3 terrainPosition, bool highPriority)
			{
				TriangleList = triangleList;
				Position = position;
				VertexCount = vertexCount;
				TPosition = terrainPosition;
				priority = highPriority;
			}

			//! Get whether collision chunk is high priority
			bool GetHighPriority()
			{
				return priority;
			}

			//! GetTerrainPosition
			NGin::Math::Vector3 GetTerrainPosition()
			{
				return TPosition;
			}

			//! Get the buffer of triangle positions
			float* GetTriangleList()
			{
				return TriangleList;
			}

			//! Get the position of the mesh
			NGin::Math::Vector3 GetPosition()
			{
				return Position;
			}

			//! Get the number of vertices (Vector3s) in the buffer
			unsigned int GetVertexCount()
			{
				return VertexCount;
			}
		};

		//! Type Definition for CollisionEventHandler structure
		/*!
		This callback is used when a chunk has to create a mesh instance in
		the collision system.
		*/
		typedef NGin::IEventHandler<ITerrain, CollisionEventArgs> CollisionEventHandler;

		/*! Terrain Manager Interface Class */
		class ITerrainManager
		{
		public:

			virtual ~ITerrainManager() {}

			//! Create a T1 (LOD) Terrain
			/*!
			\param Size Size of the terrain as a multiple of 32
			*/
			virtual ITerrain* CreateT1(int size) = 0;

			//! Save a T1
			virtual bool SaveT1(ITerrain* terrain, std::string path) = 0;

			//! Load a T1
			virtual ITerrain* LoadT1(std::string path) = 0;

			//! Update the terrain system
			/*!
			\param Position Position of the camera for 'relative' chunk updates
			*/
			virtual void Update(NGin::Math::Vector3 &position, bool updateCollisions) = 0;

			//! Render terrains
			/*!
			\param ViewProjection Float array of a 4x4 ViewProjection matrix
			\returns Statistics related to the render
			*/
			virtual FrameStatistics Render(int rtIndex, float* viewProjection, const float* lightProjection, void* shadowMap) = 0;
			virtual FrameStatistics Render(int rtIndex, float* viewProjection, const float* lightProjection, void* shadowMap, NGin::Math::Vector3 offset) = 0;

			//! Render depth pass
			virtual void RenderDepth(const float* viewProjection) = 0;
			virtual void RenderDepth(const float* viewProjection, NGin::Math::Vector3 &offset) = 0;

			virtual void SetLightNormal(int index, NGin::Math::Vector3 &normal) = 0;
			virtual void SetLightColor(int index, NGin::Math::Color &color) = 0;
			virtual void SetAmbientLight(NGin::Math::Color &color) = 0;
			virtual void SetPointLights(void** lights, int count, int stride) = 0;
			virtual void SetFog(NGin::Math::Color& color, NGin::Math::Vector2& data) = 0;

			//! Device was lost
			virtual void OnDeviceLost() = 0;

			//! Device was reset
			virtual void OnDeviceReset() = 0;

			//! CollisionEventHandler for CollisionChanged event
			virtual CollisionEventHandler* CollisionChanged() = 0;

			virtual void SetBrushRadius(float radius) = 0;
			virtual void SetBrushHardness(float hardness) = 0;
			virtual void SetBrushPosition(NGin::Math::Vector2 position) = 0;
			virtual void SetBrushIsCircle(bool circle) = 0;
			virtual void SetEditorMode(bool editorMode) = 0;
			virtual void SetSecondBrush(float radius, float hardness, NGin::Math::Vector2 position, bool circle) = 0;

			virtual float GetBrushRadius() = 0;
			virtual float GetBrushHardness() = 0;
			virtual NGin::Math::Vector2 GetBrushPosition() = 0;
			virtual bool GetBrushIsCircle() = 0;
			virtual bool GetEditorMode() = 0;

			virtual void SetEditorLock(bool lock) = 0;
			virtual bool GetEditorLock() = 0;

			virtual bool LoadGrassTypes(std::string path) = 0;
			virtual bool SaveGrassTypes(std::string path) = 0;
			virtual IGrassType* CreateGrassType() = 0;
			virtual IGrassType* FindGrassType(std::string name) = 0;
			virtual void RemoveGrassType(IGrassType* type) = 0;
			virtual void FetchGrassTypes(NGin::ArrayList<IGrassType*> &outTypesList) = 0;

			virtual void SetGrassDrawDistance(float distance) = 0;
			virtual float GetGrassDrawDistance() = 0;

			virtual void SetGrassSwayPower(float power) = 0;
			virtual void SetGrassSwayDirection(NGin::Math::Vector2 direction) = 0;
			virtual float GetGrassSwayPower() = 0;
			virtual NGin::Math::Vector2 GetGrassSwayDirection() = 0;

			virtual void SetTerrainRender(bool render) = 0;
			virtual void SetGrassRender(bool render) = 0;
			virtual bool GetTerrainRender() const = 0;
			virtual bool GetGrassRender() const = 0;
			virtual void SetCollisionRender(bool render) = 0;
			virtual bool GetCollisionRender() const = 0;
		};

		//! Create a terrain manager
		ITerrainManager* CreateTerrainManager(IDirect3DDevice9* device, std::string t1EffectPath, std::string t1EditorPath, std::string grassEffectPath, std::string t1DepthPath);
	}
}
