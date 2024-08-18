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
#include <stdlib.h>
#include <vector3.h>
#include <d3dx9.h>

namespace RealmCrafter
{
	namespace RCT
	{

		//! Coordinate color of a T1 Terrain
		struct T1TexCol
		{
			unsigned char S0, S1, S2, S3;

			T1TexCol()
			{
				S0 = S1 = S2 = S3 = 0;
			}

			//! Set all components at once
			void Set(unsigned char s0, unsigned char s1, unsigned char s2, unsigned char s3)
			{
				S0 = s0;
				S1 = s1;
				S2 = s2;
				S3 = s3;
			}
		};

		//! Single data structure of a T1 vertex
		struct T1Data
		{
			float Height;
			T1TexCol Splat;
			unsigned char GrassTypes;

			T1Data()
			{
				Height = 0;
				GrassTypes = 0;
			}
		};

		struct T1VertexLOD
		{
			float X, Y, Z;
			float NX, NY, NZ;
			float U, V;
			T1TexCol Splat;
		};

		struct T1Vertex
		{
			float X, nY, Z, oY;
			float NX, NY, NZ;

			void Set(float x, float y, float z)
			{
				X = x;
				Z = z;
				oY = y;
			}
		};

		//! Structure of a T1 vertex
		struct T1VertexREM
		{
			char X, Z;
			unsigned char NX, NZ;
			unsigned short oY, nY;

			T1VertexREM()
			{
				X = Z = 0;
				NX = NZ = 128;
				oY = nY = 0;
			}

			void Set(char x, unsigned short y, char z)
			{
				X = x;
				Z = z;
				oY = y;

				NX = (unsigned char)(((float)rand()) / ((float)RAND_MAX) * 255.0f);
				NZ = (unsigned char)(((float)rand()) / ((float)RAND_MAX) * 255.0f);
			}
		};

		//! Structure of a grass vertex
		struct SGrassVertex
		{
			NGin::Math::Vector3 Position;
			float U, V, Stem;

			inline void Set(float x, float y, float z, float u, float v, float stem)
			{
				Position.X = x;
				Position.Y = y;
				Position.Z = z;
				U = u;
				V = v;
				Stem = stem;
			}
		};

		class CChunk;
			
		//! Buffer containing grass of a type
		struct SGrassBuffer
		{
			IDirect3DVertexBuffer9* Vertices;
			IDirect3DIndexBuffer9* Indices;
			CChunk* Chunk;
			unsigned int VertexCount;
			unsigned int IndexCount;
			bool Valid;
			unsigned int Seed;

			SGrassBuffer(unsigned int seed, CChunk* chunk)
			{
				Seed = seed;
				Vertices = 0;
				Indices = 0;
				VertexCount = 0;
				IndexCount = 0;
				Valid = false;
				Chunk = chunk;
			}

			~SGrassBuffer()
			{
				if(Vertices != 0)
					Vertices->Release();

				if(Indices != 0)
					Indices->Release();

				Vertices = 0;
				Indices = 0;
				VertexCount = 0;
				IndexCount = 0;
				Valid = false;
			}

			inline void Invalidate()
			{
				if(Vertices != 0)
					Vertices->Release();

				if(Indices != 0)
					Indices->Release();

				Vertices = 0;
				Indices = 0;
				VertexCount = 0;
				IndexCount = 0;
				Valid = false;
			}
		};
	}
}
