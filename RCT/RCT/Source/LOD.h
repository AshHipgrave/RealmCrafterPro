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
//* HISTORY:
//*
//*  Jared B
//*    - Adding LOD3 and LOD4 classes for the lowest LOD chunk levels. These gather chunks to minimize overuse of draw
//*      calls.
//*
//*******************************************************************************************************************************
#pragma once

#include <d3dx9.h>

namespace RealmCrafter
{
	namespace RCT
	{
		const int MAX_REGISTERED_CHUNKS = 1024;

		class CChunk;
		struct T1VertexLOD;

		class LOD
		{
		public:

			LOD( IDirect3DDevice9* device, int chunkSize );
			~LOD();

			void Update();
			void Draw( ID3DXEffect* effect );

			void AddChunk( CChunk* chunk );
			void RemoveChunk( CChunk* chunk );

			void OnDeviceLost();
			void OnDeviceReset();

		private:

			int FindChunk( CChunk* chunk );
			int BuildChunk( CChunk* chunk, T1VertexLOD* vertices );

			int chunkSize_;

			IDirect3DDevice9* device_;
			IDirect3DVertexBuffer9* vertices_;

			CChunk* registeredChunks_[MAX_REGISTERED_CHUNKS];
			int chunksInUse_;

			bool dirty_;

		};
	}
}