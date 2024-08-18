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

#include "TerrainManager.h"
#include "NVector2.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace RealmCrafter
{
namespace RCT
{
	public ref class RCSplat
	{
	public:

		System::Byte S0, S1, S2, S3;

		void Set(System::Byte s0, System::Byte s1, System::Byte s2, System::Byte s3)
		{
			S0 = s0;
			S1 = s1;
			S2 = s2;
			S3 = s3;
		}
	};

	public ref class RCTerrain
	{
	protected:

		RealmCrafter::RCT::ITerrain* Handle;
		System::Object^ _Tag;

	public:
		RCTerrain(IntPtr handle);
		~RCTerrain();

		IntPtr GetHandle();

		int GetSize();

		void SetTextureScale(int index, float scale);
		float GetTextureScale(int index);

		void SetTexture(int index, System::String^ path);
		System::String^ GetTexture(int index);

		void SetGrassType(int index, System::String^ type);
		System::String^ GetGrassType(int index);
		
		void SetHeight(int x, int y, float height);
		void SetHeight(int chunkX, int chunkY, int x, int y, float height);

		float GetHeight(int x, int y);
		float GetHeight(int chunkX, int chunkY, int x, int y);

		void SetGrass(int x, int y, System::Byte gr);
		System::Byte GetGrass(int x, int y);

		void SetExclusion(int x, int y, bool exclude);
		bool GetExclusion(int x, int y);


		NGUINet::NVector3^ GetPosition();
		void SetPosition(NGUINet::NVector3^ position);
		
		RCSplat^ GetColorChunk(NGUINet::NVector2^ position, int x, int y);
		void SetColorChunk(NGUINet::NVector2^ position, int x, int y, RCSplat^ color);

		void MgrDstr();

		void Raise(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float change);
		void Smooth(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float change);
		void SetHeight(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float height);
		void Paint(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float strength, int texture, float min, float max, float minHeight, float maxHeight);
		void PaintHole(float radius, NGUINet::NVector2^ position, bool circular, bool erase);
		void PaintGrass(float radius, NGUINet::NVector2^ position, bool circular, System::Byte grassMask, bool additive);
		void Ramp(NGUINet::NVector2^ startPosition, NGUINet::NVector2^ endPosition, float startWidth, float endWidth);

		property System::Object^ Tag
		{
			System::Object^ get();
			void set(System::Object^);
		}
	};
}
}