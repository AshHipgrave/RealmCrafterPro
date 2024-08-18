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
//COPYRIGHTNOTICE

#pragma once

#include <vector>

#include "ITreeType.h"

#include "ITreeManager.h"
#include "ITreeMeshBuffer.h"
#include "ITreeTexture.h"

#include <AABB.h>

namespace LT
{
	struct STreeBuffer
	{
		ITreeMeshBuffer* Buffer;
		ITreeTexture* DiffuseMap;
		ITreeTexture* NormalMap;
		ITreeTexture* MaskMap;
		float Size;

		STreeBuffer() : Buffer(0), DiffuseMap(0), NormalMap(0), MaskMap(0), Size(1.0f) {}
	};
	
	// Tree Type
	class CTreeType : public ITreeType
	{
	protected:

		ITreeManager* Manager;
		std::string Name, NameLower;
		std::vector<STreeBuffer*> TrunkSurfaces;
		std::vector<STreeBuffer*> LeafSurfaces;
		std::vector<ITreeTexture*> LODTextures;
		NGin::Math::AABB BoundingBox;
		NGin::Math::Vector3 SwayCenter;
		STreeCollisionData* CollisionData;

		virtual void Unload();

	public:

		CTreeType(ITreeManager* manager, std::string &name);
		virtual ~CTreeType();

		virtual const STreeCollisionData* GetCollisionData();
		virtual const char* GetName() const;
		virtual bool IsType(const char* nameLower) const;

		virtual int GetTrunkSurfaceCount() const;
		virtual int GetLeafSurfaceCount() const;
		virtual void BeginTrunk(int index);
		virtual void BeginLeaf(int index);
		virtual void DrawTrunk(int index);
		virtual void DrawLeaf(int index);
		virtual void BeginLOD(int index);

		virtual void DrawLOD(ITreeMeshBuffer* buffer);

		virtual NGin::Math::AABB GetBoundingBox() const;
		virtual int GetLODTexturesCount() const;

		static ITreeType* LoadTreeType(ITreeManager* manager, FILE* file, const std::string &path, CTreeType* treeType);
	};
}
