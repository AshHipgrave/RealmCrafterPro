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
// NGUINet.h

#pragma once

#include <ITerrainManager.h>
#include "NVector2.h"
#include "NVectorConverter.h"
#include "RCTerrain.h"
#include "GrassType.h"

#include <vcclr.h>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace RealmCrafter
{
namespace RCT
{
	ref class RCTerrain;

	public ref class TerrainPointLight
	{
	public:

		NGUINet::NVector3^ Position;
		System::Drawing::Color^ Color;
		int Radius;
		bool Active;
		float Distance;
	};

	public ref class CollisionChangedEventArgs : public System::EventArgs
	{
	protected:

		IntPtr _TriangleList;
		System::UInt32 _VertexCount;
		NGUINet::NVector3^ _Position;
		NGUINet::NVector3^ _TPosition;
		bool _Priority;

	public:

		CollisionChangedEventArgs(IntPtr triangleList, System::UInt32 vertexCount, NGUINet::NVector3^ position, NGUINet::NVector3^ terrainPosition, bool priority);

		property bool HighPriority
		{
			bool get();
		}

		property IntPtr TriangeList
		{
			IntPtr get();
		}

		property System::UInt32 VertexCount
		{
			System::UInt32 get();
		}

		property NGUINet::NVector3^ Position
		{
			NGUINet::NVector3^ get();
		}

		property NGUINet::NVector3^ TerrainPosition
		{
			NGUINet::NVector3^ get();
		}
	};

	public ref class TerrainManager
	{
	protected:
		RealmCrafter::RCT::ITerrainManager* _Manager;
		//System::Collections::Generic::List<NGUINet::NControl^>^ _Controls;
		IntPtr _Render, _ShadowDepth, _Reset, _Lost;
		System::Collections::Generic::List<RCT::RCTerrain^>^ _Terrains;

		
	public:

		TerrainManager(IntPtr device, System::String^ t1EffectPath, System::String^ t1EditorPath, System::String^ grassEffectPath, System::String^ depthEffectPath);
		~TerrainManager();

		void __CollisionChangedEvent(RealmCrafter::RCT::ITerrain* Terrain, RealmCrafter::RCT::CollisionEventArgs* E);


		IntPtr GetRenderCallback();	
		IntPtr GetRenderDepthCallback();	
		IntPtr GetLostCallback();
		IntPtr GetResetCallback();
		//void Register(NGUINet::NControl^ Control);
		//void UnRegister(NGUINet::NControl^ Control);

		RCTerrain^ CreateT1(int size);
		RCTerrain^ LoadT1(System::String^ path);
		bool SaveT1(RCTerrain^ terrain, System::String^ path);
		void Destroy(RCTerrain^ terrain);

		void Update(NGUINet::NVector3^ position, bool updateCollisions);
		void Render(cli::array<float>^ viewProjection, cli::array<float>^ lightProjection, IntPtr ShadowMap);
		void RenderDepth(cli::array<float>^ viewProjection);

		void SetLightNormal(int index, NGUINet::NVector3^ normal);
		void SetLightColor(int index, System::Drawing::Color^ color);
		void SetAmbientLight(System::Drawing::Color^ color);
		void SetPointLights(cli::array<TerrainPointLight^>^ lights);
		void SetPointLights(IntPtr lightPtr, IntPtr count, IntPtr stride);
		void SetFog(System::Drawing::Color^ color, NGUINet::NVector2^ data);

		IntPtr GetDirectionBuffer();
		IntPtr GetColorBuffer();
		void SetDirectionBuffer(IntPtr Buffer);
		void SetColorBuffer(IntPtr Buffer);

		void SetSecondBrush(float radius, float hardness, NGUINet::NVector2^ position, bool circle);

		bool LoadGrassTypes(System::String^ path);
		bool SaveGrassTypes(System::String^ path);
		GrassType^ CreateGrassType();
		GrassType^ FindGrassType(System::String^ name);
		void RemoveGrassType(GrassType^ type);
		cli::array<GrassType^>^ FetchGrassTypes();

		event System::EventHandler^ CollisionChanged;
		event System::EventHandler^ TerrainRender;
		event System::EventHandler^ TerrainRenderDepth;

		property float BrushRadius
		{
			float get();
			void set(float);
		}

		property float BrushHardness
		{
			float get();
			void set(float);
		}

		property NGUINet::NVector2^ BrushPosition
		{
			NGUINet::NVector2^ get();
			void set(NGUINet::NVector2^);
		}

		property bool BrushIsCircle
		{
			bool get();
			void set(bool);
		}

		property bool EditorMode
		{
			bool get();
			void set(bool);
		}

		property bool EditorLocked
		{
			bool get();
			void set(bool);
		}

		property bool RenderTerrain
		{
			bool get();
			void set(bool);
		}

		property bool RenderGrass
		{
			bool get();
			void set(bool);
		}

		property bool RenderCollision
		{
			bool get();
			void set(bool);
		}
// 
// 		property bool TerrainRender
// 		{
// 			bool get();
// 			void set(bool);
// 		}
// 
// 		property bool GrassRender
// 		{
// 			bool get();
// 			void set(bool);
// 		}

		void __TerrainRender(int rtIndex);
		void __TerrainRenderDepth();

		static TerrainManager^ LastTerrainManager = nullptr;
	};
}
}