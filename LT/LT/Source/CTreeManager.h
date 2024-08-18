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

#include "ITreeManager.h"
#include "CTreeZone.h"
#include <Matrix.h>
#include <vector3.h>
#include <Color.h>
#include <vector4.h>
#include <ArrayList.h>

namespace LT
{
	class CTreeZone;

	// Tree Manager
	class CTreeManager : public ITreeManager
	{
	protected:

		ITreeRenderer* Renderer;
		unsigned int TreesDrawn;
		unsigned int LODsDrawn;

		CollisionEventHandler* CollisionChangedEvent;

		std::vector<ITreeType*> LoadedTypes;
		std::vector<CTreeZone*> LoadedZones;

		float LODDistance, RenderDistance, PointLightDistance;

	public:

		CTreeManager(ITreeRenderer* renderer);
		virtual ~CTreeManager();

		virtual ITreeRenderer* GetRenderer() const;
		virtual unsigned int GetTreesDrawn() const;
		virtual unsigned int GetLODsDrawn() const;

		virtual CollisionEventHandler* CollisionChanged();

		virtual void SetLODDistance(float distance);
		virtual void SetRenderDistance(float distance);
		virtual void SetPointLightDistance(float distance);
		virtual float GetLODDistance() const;
		virtual float GetRenderDistance() const;
		virtual float GetPointLightDistance() const;

		virtual bool Render(int rtIndex, NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Matrix lightProj);
		virtual bool RenderDepth(NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition);
		virtual bool Render(int rtIndex, NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Matrix lightProj, NGin::Math::Vector3 &offset);
		virtual bool RenderDepth(NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Vector3 &offset);

		virtual bool Update(const float* cameraPosition);

		virtual ITreeType* LoadTreeType(const char* path);
		virtual bool ReloadTreeType(ITreeType* oldType, const char* path);

		virtual ITreeType* FindTreeType(const char* name) const;

		virtual ITreeZone* LoadZone(const char* path);
		virtual ITreeZone* CreateZone();
		virtual void UnloadZone(ITreeZone* zone);

		virtual bool SaveZone(const ITreeZone* zone, const char* path) const;
	};
}
