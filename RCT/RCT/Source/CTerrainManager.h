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
#include <Vector4.h>
#include <ITerrainManager.h>
#include "CTerrain.h"
#include <xmlwrapper.h>
#include "CGrassType.h"
#include <ArrayList.h>

namespace RealmCrafter
{
	namespace RCT
	{

		class CTerrain;
		class CGrassType;

		// Terrain Manager
		class CTerrainManager : public ITerrainManager
		{
		protected:

			friend class CTerrain;

			// Device handle
			IDirect3DDevice9* Device;
			CollisionEventHandler* CollisionChangedEvent;

			// T1 Data
			ID3DXEffect* T1Effect;
			ID3DXEffect* T1Editor;
			ID3DXEffect* GrassEffect;
			ID3DXEffect* T1DepthEffect;
			IDirect3DVertexDeclaration9* T1Declaration;
			IDirect3DVertexDeclaration9* T1LODDeclaration;
			IDirect3DVertexDeclaration9* GrassDeclaration;
			NGin::ArrayList<CTerrain*> T1List;

			// Lighting data
			NGin::Math::Vector3 DirectionalNormals[3];
			NGin::Math::Vector3 DirectionalColors[3];
			NGin::Math::Vector3 AmbientLight;

			// Point lights
			struct PointLight
			{
				NGin::Math::Vector3 Position;
				NGin::Math::Color Color;
				int Radius;
				bool Active;
				float Distance;
			};
			NGin::ArrayList<PointLight*> PointLights;

			// Fog data
			NGin::Math::Color FogColor;
			NGin::Math::Vector2 FogData;

			// Editor brush data
			bool EditorMode;
			bool CircleBrush[2];
			NGin::Math::Vector2 BrushPosition[2];
			float BrushRadius[2];
			float BrushHardness[2];
			bool EditorLock;
			bool RenderTerrain, RenderGrass;
			bool RenderCollision;

			NGin::ArrayList<CGrassType*> GrassTypes;
			float GrassDrawDistance;
			float GrassSwayPower;
			NGin::Math::Vector2 GrassSwayDirection;
			NGin::Math::Vector3 CameraPosition;

		public:

			// Constructor
			CTerrainManager(IDirect3DDevice9* device, ID3DXEffect* t1Effect, ID3DXEffect* t1Editor, ID3DXEffect* grassEffect, ID3DXEffect* t1Depth);
			~CTerrainManager();

			// Create a T1 Terrain
			virtual ITerrain* CreateT1(int size);

			// Save a T1
			virtual bool SaveT1(ITerrain* terrain, std::string path);

			// Load a T1
			virtual ITerrain* LoadT1(std::string path);

			// Update all terrains
			virtual void Update(NGin::Math::Vector3 &position, bool updateCollisions);

			// Render depth pass
			virtual void RenderDepth(const float* viewProjection);
			virtual void RenderDepth(const float* viewProjection, NGin::Math::Vector3 &offset);
			
			// Render all terrains
			virtual FrameStatistics Render(int rtIndex, float* viewProjection, const float* lightProjection, void* shadowMap);
			virtual FrameStatistics Render(int rtIndex, float* viewProjection, const float* lightProjection, void* shadowMap, NGin::Math::Vector3 offset);

			// Device was lost
			virtual void OnDeviceLost();

			// Device was reset
			virtual void OnDeviceReset();

			// Collision changed event
			virtual CollisionEventHandler* CollisionChanged();


			virtual void SetLightNormal(int index, NGin::Math::Vector3 &normal);
			virtual void SetLightColor(int index, NGin::Math::Color &color);
			virtual void SetAmbientLight(NGin::Math::Color &color);
			virtual void SetPointLights(void** lights, int count, int stride);
			virtual void GetClosestLights(NGin::Math::Vector3** colors, NGin::Math::Vector4** positions, NGin::Math::Vector3 &position);
			virtual void SetFog(NGin::Math::Color& color, NGin::Math::Vector2& data);

			virtual void SetBrushRadius(float radius);
			virtual void SetBrushHardness(float hardness);
			virtual void SetBrushPosition(NGin::Math::Vector2 position);
			virtual void SetBrushIsCircle(bool circle);
			virtual void SetEditorMode(bool editorMode);
			virtual void SetSecondBrush(float radius, float hardness, NGin::Math::Vector2 position, bool circle);

			virtual float GetBrushRadius();
			virtual float GetBrushHardness();
			virtual NGin::Math::Vector2 GetBrushPosition();
			virtual bool GetBrushIsCircle();
			virtual bool GetEditorMode();

			virtual void SetEditorLock(bool lock);
			virtual bool GetEditorLock();

			virtual bool LoadGrassTypes(std::string path);
			virtual bool SaveGrassTypes(std::string path);
			virtual IGrassType* CreateGrassType();
			virtual IGrassType* FindGrassType(std::string name);
			virtual void RemoveGrassType(IGrassType* type);
			virtual void FetchGrassTypes(NGin::ArrayList<IGrassType*> &outTypesList);

			virtual void SetGrassDrawDistance(float distance);
			virtual float GetGrassDrawDistance();

			virtual void SetGrassSwayPower(float power);
			virtual void SetGrassSwayDirection(NGin::Math::Vector2 direction);
			virtual float GetGrassSwayPower();
			virtual NGin::Math::Vector2 GetGrassSwayDirection();

			virtual void SetTerrainRender(bool render);
			virtual void SetGrassRender(bool render);
			virtual bool GetTerrainRender() const;
			virtual bool GetGrassRender() const;
			virtual void SetCollisionRender(bool render);
			virtual bool GetCollisionRender() const;
		};
	}
}
