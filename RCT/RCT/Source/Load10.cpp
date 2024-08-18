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
#include "SaveLoad.h"
#include <NGinString.h>

namespace RealmCrafter
{
	namespace RCT
	{
		ITerrain* Load10(CTerrainManager* terrainManager, FILE* f)
		{
			// Locals used for loading the terrain
			NGin::WString TexturePaths[5] = {"", "", "", "", ""};
			NGin::WString GrassNames[8] = {"", "", "", "", "", "", "", ""};
			float TextureScales[5] = {1, 1, 1, 1, 1};
			NGin::Math::Vector3 Position(0, 0, 0);
			unsigned int Size = 0;
			float Height = 0.0f;
			T1TexCol Splat;
			unsigned char Exclusion = 0, Grass = 0;

			// Read Textures
			for(int i = 0; i < 5; ++i)
			{
				unsigned int Length = 0;
				fread(&Length, sizeof(unsigned int), 1, f);

				if(Length == 0)
					continue;

				//TexturePaths[i].Pad(Length);
				//fread((void*)TexturePaths[i].w_str(), sizeof(unsigned short), Length, f);

				unsigned char* RawString = new unsigned char[Length];
				fread(RawString, sizeof(unsigned char), Length, f);
				TexturePaths[i].Set((const char*)RawString, Length);
				delete RawString;
			}

			// Read Grasses
			for(int i = 0; i < 8; ++i)
			{
				unsigned int Length = 0;
				fread(&Length, sizeof(unsigned int), 1, f);

				if(Length == 0)
					continue;

				unsigned short* RawString = new unsigned short[Length];
				fread(RawString, sizeof(unsigned short), Length, f);
				GrassNames[i].Set(RawString, Length);
				delete RawString;
			}

			// Texture scale
			fread(TextureScales, sizeof(float), 5, f);
			//NGin::Math::Vector2 TextureScale;
			//fread(&TextureScale, sizeof(NGin::Math::Vector2), 1, f);

			// Terrain "position"
			fread(&Position, sizeof(NGin::Math::Vector3), 1, f);

			// Terrain size
			fread(&Size, sizeof(unsigned int), 1, f);

			// Create terrain
			ITerrain* Terrain = terrainManager->CreateT1(Size);
			if(Terrain == 0)
				return 0;

			for(int i = 0; i < 5; ++i)
				Terrain->SetTexture(i, std::string(TexturePaths[i].AsCString().c_str()));
			for(int i = 0; i < 5; ++i)
				Terrain->SetTextureScale(i, TextureScales[i]);
			//Terrain->SetTextureScale(TextureScale);
			
			for(int i = 0; i < 8; ++i)
				Terrain->SetGrassType(i, GrassNames[i].AsCString().c_str());


			// Read raw position data
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					fread(&Height, sizeof(float), 1, f);
					Terrain->SetHeight(x, z, Height);
				}
			}

			// Read raw texture data
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					fread(&Splat, sizeof(T1TexCol), 1, f);
					Terrain->SetColorChunk(NGin::Math::Vector2(0, 0), x, z, Splat);
				}
			}

			// Read raw exclusion data
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					fread(&Exclusion, sizeof(unsigned char), 1, f);
					Terrain->SetExclusion(x, z, Exclusion == 0 ? true : false);
				}
			}

			// Read grasses
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					fread(&Grass, sizeof(unsigned char), 1, f);
					Terrain->SetGrass(x, z, Grass);
				}
			}

			fclose(f);

			// Done
			return Terrain;
		}
	}
}