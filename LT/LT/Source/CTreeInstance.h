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

#include "ITreeInstance.h"
#include "CTreeZone.h"

namespace LT
{
	// Declarations
	struct STreeSector;

	// Tree Instance
	class CTreeInstance : public ITreeInstance
	{
	protected:

		NGin::Math::Vector3 Position;
		NGin::Math::Vector3 Scale;
		//NGin::Math::Quaternion Rotation;
		NGin::Math::Vector3 Rotation;
		NGin::Math::Matrix Transform;
		ITreeType* Type;
		STreeSector* ParentSector;
		bool CollisionsActive;
		void* Tag;

		void RebuildTransform();

	public:

		CTreeInstance(ITreeType* baseType, STreeSector* parentSector);
		virtual ~CTreeInstance();

		virtual void* GetTag() const;
		virtual void SetTag(void* tag);

		virtual void UpdateCollisions(bool set);

		virtual void SetPosition(NGin::Math::Vector3 &position);
		virtual void SetScale(NGin::Math::Vector3 &scale);
		//virtual void SetRotation(NGin::Math::Quaternion &rotation);
		virtual void SetRotation(NGin::Math::Vector3 &rotation);
		virtual NGin::Math::Matrix GetTransform() const;

		virtual NGin::Math::Vector3 GetRotation() const;
		virtual NGin::Math::Vector3 GetScale() const;

		virtual STreeSector* GetParentSector() const;
		virtual void Destroy();

		virtual ITreeType* GetType();
	};
}
