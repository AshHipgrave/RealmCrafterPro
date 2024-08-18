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

#include "NVectorConverter.h"
#include "TreeType.h"
#include "TreeZone.h"
#include "TreeInstance.h"
#include "TreeCollision.h"
#include <ITreeManager.h>

using namespace LT;
using namespace System;
using namespace System::Runtime::InteropServices;

namespace LTNet
{
	ref class TreeZone;
	ref class TreeType;
	ref class TreeInstance;

	public ref class TreePointLight
	{
	public:

		float PositionX;
		float PositionY;
		float PositionZ;
		System::Drawing::Color^ Color;
		int Radius;
		bool Active;
		float Distance;
	};


	public ref class TreeManager
	{
		ITreeManager* _TreeManager;
		IntPtr _Render, _Reset, _Lost;

	public:

		TreeManager(ITreeManager* handle);

		unsigned int GetTreesDrawn();
		unsigned int GetLODsDrawn();
		
		//bool Render(cli::array<float>^ view, cli::array<float>^ projection, NGUINet::NVector3^ cameraPosition);
		bool Render(cli::array<float>^ view, cli::array<float>^ projection, cli::array<float>^ lightProjection, float x, float y, float z);
		bool Update(float x, float y, float z);

		TreeType^ LoadTreeType(String^ path);
		bool ReloadTreeType(TreeType^ oldType, String^ path);
		TreeType^ FindTreeType(String^ name);

		TreeZone^ CreateZone();
		TreeZone^ LoadZone(String^ path);
		bool SaveZone(TreeZone^ zone, String^ path);
		void UnloadZone(TreeZone^ zone);

		property float LODDistance
		{
			float get();
			void set(float value);
		}

		property float RenderDistance
		{
			float get();
			void set(float value);
		}

		property float PointLightDistance
		{
			float get();
			void set(float value);
		}

		void SetShadowMap(System::IntPtr handle);

		void SetLightNormal(int index, float x, float y, float z);
		void SetLightColor(int index, System::Drawing::Color^ color);
		void SetAmbientLight(System::Drawing::Color^ color);
		void SetPointLights(cli::array<TreePointLight^>^ lights);
		void SetPointLights(IntPtr lightPtr, IntPtr count, IntPtr stride);
		void SetFog(System::Drawing::Color^ color, float data1, float data2);

		void SetSwayDirection(float directionx, float directionz);
		void SetSwayPower(float power);
		void SetQualityLevel(int qualityLevel);

		IntPtr GetDirectionBuffer();
		IntPtr GetColorBuffer();
		void SetDirectionBuffer(IntPtr Buffer);
		void SetColorBuffer(IntPtr Buffer);

		event System::EventHandler^ CollisionChanged;
		event System::EventHandler^ TreeRender;

		void __CollisionChangedEvent(ITreeZone* treeZone, LT::CollisionEventArgs* E);

		IntPtr GetRenderCallback();	
		IntPtr GetLostCallback();
		IntPtr GetResetCallback();

		void __TreeRender(int rtIndex);

		static TreeManager^ LastTreeManager = nullptr;

		static TreeManager^ CreateTreeManager(IntPtr device, String^ trunkPath, String^ leafPath, String^ lodPath);
	};
}