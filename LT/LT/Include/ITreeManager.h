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

#include "ITreeRenderer.h"
#include "ITreeType.h"
#include "ITreeInstance.h"
#include "ITreeZone.h"
#include <IEventHandler.h>

namespace LT
{
	//! Event arguments structure used when a tree needs a collision instances generated
	class CollisionEventArgs
	{
	protected:

		STreeCollisionData* CollisionData;
		ITreeInstance* TreeInstance;

	public:

		CollisionEventArgs(STreeCollisionData* collisionData, ITreeInstance* treeInstance)
		{
			CollisionData = collisionData;
			TreeInstance = treeInstance;
		}

		//! Get the data structure for body information
		const STreeCollisionData* GetCollisionData() const
		{
			return CollisionData;
		}

		//! Get the tree instance (constant, as movement during update would cause handling issues)
		const ITreeInstance* GetTreeInstance() const
		{
			return TreeInstance;
		}
	};

	//! Type Definition for CollisionEventHandler structure
	/*!
	This callback is used when a tree comes into collidable range
	*/
	typedef NGin::IEventHandler<ITreeZone, CollisionEventArgs> CollisionEventHandler;

	//! Tree Manager Interface
	class ITreeManager
	{
	public:

		ITreeManager() {};
		virtual ~ITreeManager() {}

		virtual ITreeRenderer* GetRenderer() const = 0;

		//! Get the number of whole trees drawn in the previous frame.
		virtual unsigned int GetTreesDrawn() const = 0;
		virtual unsigned int GetLODsDrawn() const = 0;

		virtual void SetLODDistance(float distance) = 0;
		virtual void SetRenderDistance(float distance) = 0;
		virtual void SetPointLightDistance(float distance) = 0;
		virtual float GetLODDistance() const = 0;
		virtual float GetRenderDistance() const = 0;
		virtual float GetPointLightDistance() const = 0;


		virtual bool Render(int rtIndex, NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Matrix lightProj) = 0;
		virtual bool RenderDepth(NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition) = 0;
		virtual bool Render(int rtIndex, NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Matrix lightProj, NGin::Math::Vector3 &offset) = 0;
		virtual bool RenderDepth(NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Vector3 &offset) = 0;

		virtual bool Update(const float* cameraPosition) = 0;

		virtual ITreeType* LoadTreeType(const char* path) = 0;
		virtual bool ReloadTreeType(ITreeType* oldType, const char* path) = 0;

		virtual ITreeType* FindTreeType(const char* name) const = 0;

		virtual ITreeZone* LoadZone(const char* path) = 0;
		virtual bool SaveZone(const ITreeZone* zone, const char* path) const = 0;
		virtual void UnloadZone(ITreeZone* zone) = 0;

		virtual ITreeZone* CreateZone() = 0;

		virtual CollisionEventHandler* CollisionChanged() = 0;

#ifdef EXPORTS
		__declspec(dllexport)
#else
		__declspec(dllimport)
#endif
			static ITreeManager* CreateTreeManager(ITreeRenderer* renderer);
	};
}
