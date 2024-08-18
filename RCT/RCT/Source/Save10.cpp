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
#include "nginstring.h"

namespace RealmCrafter
{
	namespace RCT
	{
		bool Save10(ITerrain* terrain, FILE* f)
		{
			// Write version header
			unsigned char VersionMajor = 1, VersionMinor = 0;
			fwrite(&VersionMajor, 1, 1, f);
			fwrite(&VersionMinor, 1, 1, f);

			// Write Textures
			for(int i = 0; i < 5; ++i)
			{
				std::string TexturePath = terrain->GetTexture(i);

				// Write the path (gruesome)
				unsigned int Length = TexturePath.length();
				fwrite(&Length, sizeof(unsigned int), 1, f);
				fwrite(TexturePath.c_str(), sizeof(unsigned char), Length, f);

				char OO[1024];
				sprintf(OO, "T(%i); L(%u); S(%s);\n", i, Length, TexturePath.c_str());
				OutputDebugString(OO);
			}

			// Write grasses
			for(int i = 0; i < 8; ++i)
			{
				NGin::WString GrassName = terrain->GetGrassType(i).c_str();

				// Write the path
				unsigned int Length = GrassName.Length();
				fwrite(&Length, sizeof(unsigned int), 1, f);
				fwrite(GrassName.w_str(), sizeof(unsigned short), Length, f);
			}

			// Texture scale
			//NGin::Math::Vector2 TextureScale = terrain->GetTextureScale();
			//fwrite(&TextureScale, sizeof(NGin::Math::Vector2), 1, f);
			float TextureScales[5] = {1, 1, 1, 1, 1};
			for(int i = 0; i < 5; ++i)
				TextureScales[i] = terrain->GetTextureScale(i);
			fwrite(TextureScales, sizeof(float), 5, f);

			// Terrain "position"
			NGin::Math::Vector3 Position(0, 0, 0);
			fwrite(&Position, sizeof(NGin::Math::Vector3), 1, f);

			// Terrain size
			unsigned int Size = terrain->GetSize();
			fwrite(&Size, sizeof(unsigned int), 1, f);

			// Write raw position data
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					float Height = terrain->GetHeight(x, z);
					fwrite(&Height, sizeof(float), 1, f);
				}
			}

			// Write raw texture data
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					T1TexCol Splat = terrain->GetColorChunk(NGin::Math::Vector2(0, 0), x, z);
					fwrite(&Splat, sizeof(T1TexCol), 1, f);
				}
			}

			// Write raw exclusion data
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					unsigned char Exclusion = terrain->GetExclusion(x, z) ? 1 : 0;
					fwrite(&Exclusion, sizeof(unsigned char), 1, f);
				}
			}

			// Write raw grass data
			for(int z = 0; z <= Size; ++z)
			{
				for(int x = 0; x <= Size; ++x)
				{
					unsigned char Grass = terrain->GetGrass(x, z);
					fwrite(&Grass, sizeof(unsigned char), 1, f);
				}
			}

			// Done
			return true;
		}
	}
}