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

#include "ITreeZone.h"
#include <map>
#include "ITreeType.h"
#include <vector>
#include "ITreeInstance.h"
#include <vector3.h>
#include "CTreeInstance.h"
#include <Vector2.h>
#include "Frustum.h"
#include "CTreeManager.h"
#include <list>
#include "ITreeMeshBuffer.h"
#include "ITreeRenderer.h"
#include "CTreeType.h"

namespace LT
{
	class CTreeManager;

	template<typename T> inline void UniqueTreeInsert(T type, std::list<T> &treeTypes)
	{
		for(std::list<T>::iterator It = treeTypes.begin(); It != treeTypes.end(); It++)
		{
			T Tmp = *It;
			if(Tmp == type)
				return;
		}

		treeTypes.push_back(type);
	}

	class CTreeZone;
	class CTreeInstance;
	struct STreeSector
	{
	private:

		struct LODSet
		{
			std::list<ITreeInstance*> Instances;
		};

	public:

		CTreeZone* Zone;
		std::map<ITreeType*, std::vector<ITreeInstance*> > Trees;
		std::map<ITreeType*, std::vector<ITreeMeshBuffer*> > LODBuffers;
		NGin::Math::Vector2 Min, Max;

		STreeSector(NGin::Math::Vector2 &min, NGin::Math::Vector2 &max, CTreeZone* zone);

		void InstanceReBuild();

		void ReBuild(ITreeRenderer* renderer);
	};


	// Tree Zone
	class CTreeZone : public ITreeZone
	{
	protected:

		struct TreeTypeRef
		{
			ITreeType* TreeType;
			int RefCount;

			TreeTypeRef() : TreeType(0), RefCount(0) {}
		};

		std::vector<TreeTypeRef*> TreeTypes;
		std::vector<std::vector<STreeSector*> > SectorMap;
		
		float MinX, MinZ, MaxX, MaxZ;
		int GridX, GridZ; // These control the global grid offset (saves calculation time)
		CTreeManager* Manager;

		bool Locked;
		std::list<STreeSector*> LockedSectors;

		STreeSector* GetZoneSector(float x, float z, bool recursing = false);
		int GetInstanceIndex(ITreeType* instance) const;

		friend CTreeInstance;

	public:

		CTreeZone(CTreeManager* manager);
		virtual ~CTreeZone();

		virtual ITreeInstance* AddTreeInstance(ITreeType* type, NGin::Math::Vector3 &position);
		virtual void Remove(ITreeInstance* instance);

		virtual void Lock();
		virtual void Unlock();
		virtual bool AllowReBuild(STreeSector* sector);
		virtual void RequestReBuild(STreeSector* sector);

		virtual void Destroy();

		virtual bool Save(const std::string &path) const;
		virtual bool Load(const std::string &path);

		virtual void Render(int rtIndex, RealmCrafter::RCT::Frustum &viewFrustum, NGin::Math::Vector3 &cameraPosition, unsigned int &treesDrawn, unsigned int &lodsDrawn, NGin::Math::Vector3 &offset);
		virtual void RenderDepth(RealmCrafter::RCT::Frustum &viewFrustum, NGin::Math::Vector3 &cameraPosition, unsigned int &treesDrawn, unsigned int &lodsDrawn, NGin::Math::Vector3 &offset);
	};
}
