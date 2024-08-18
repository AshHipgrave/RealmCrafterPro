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

#include "IGrassType.h"
#include "CChunk.h"
#include <dictionary.h>
#include <d3dx9.h>
#include <list>

namespace RealmCrafter
{
	namespace RCT
	{
		class CChunk;

		class CGrassType : public IGrassType
		{
		protected:

			IDirect3DTexture9* Texture;
			IDirect3DDevice9* Device;

			std::string Name, Path;
			NGin::Math::Vector2 Scale;
			float Offset, Coverage, HeightVariance;
			//NGin::Dictionary<CChunk*, SGrassBuffer*> Buffers;
			NGin::ArrayList<SGrassBuffer*> Buffers;

			virtual void ReBuild(CChunk* chunk, SGrassBuffer* buffer);

		public:

			CGrassType(IDirect3DDevice9* device);
			virtual ~CGrassType();

			virtual std::string GetName();
			virtual void SetName(std::string& name);

			virtual std::string GetTexture();
			virtual void SetTexture(std::string& path);

			virtual NGin::Math::Vector2 GetScale();
			virtual void SetScale(NGin::Math::Vector2& scale);

			virtual float GetOffset();
			virtual void SetOffset(float offset);

			virtual float GetCoverage();
			virtual void SetCoverage(float coverage);

			virtual float GetHeightVariance();
			virtual void SetHeightVariance(float heightVariance);

			virtual void CreateGrass(CChunk* chunk);
			virtual void RemoveGrass(CChunk* chunk);
			virtual void Update();
			virtual void Invalidate();
			virtual void Invalidate(CChunk* chunk);
			virtual void Render(ID3DXEffect* effect, std::list<CChunk*> &renderChunks, NGin::Math::Vector3 &cameraPosition, float drawDistance, NGin::Math::Vector3 &offset);
		};
	}
}