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

#include "CTreeInstance.h"

namespace LT
{
	CTreeInstance::CTreeInstance(ITreeType* baseType, STreeSector* parentSector)
		: ParentSector(parentSector), Type(baseType), Scale(1, 1, 1), CollisionsActive(false), Tag(0)
	{
	}

	CTreeInstance::~CTreeInstance()
	{
		if(CollisionsActive)
			UpdateCollisions(false);
	}

	void* CTreeInstance::GetTag() const
	{
		return Tag;
	}

	void CTreeInstance::SetTag(void* tag)
	{
		Tag = tag;
	}

	void CTreeInstance::UpdateCollisions(bool set)
	{
		if(ParentSector == 0)
			return;
		if(Type == 0)
			return;

		// Remove this instance
		if(!set)
		{
			CollisionEventArgs Args(0, this);
			ParentSector->Zone->Manager->CollisionChanged()->Execute(ParentSector->Zone, &Args);
			CollisionsActive = false;
		}else
		{
			//NGin::Math::Vector3 Scale = Transform.Scale();

			STreeCollisionData TData;
			const STreeCollisionData* OData = Type->GetCollisionData();
			TData.InstanceCount = OData->InstanceCount;
			TData.Instances = new STreeCollisionData::DataInstance[TData.InstanceCount];
			for(unsigned int i = 0; i < TData.InstanceCount; ++i)
			{
				TData.Instances[i].Type = OData->Instances[i].Type;
				TData.Instances[i].Offset = OData->Instances[i].Offset * Scale;
				TData.Instances[i].Dimensions = OData->Instances[i].Dimensions * Scale;
			}

			CollisionEventArgs Args(&TData, this);
			ParentSector->Zone->Manager->CollisionChanged()->Execute(ParentSector->Zone, &Args);

			delete[] TData.Instances;
			CollisionsActive = true;
		}
	}

	void CTreeInstance::RebuildTransform()
	{
		//Transform = Rotation.ToMatrix();
		//D3DXMatrixRotationQuaternion((D3DXMATRIX*)&Transform, (D3DXQUATERNION*)&Rotation);
 		Transform.MakeIdentity();
		//Transform.Scale(Scale);
		Transform.RotationDeg(Rotation);
		Transform.Translation(Position);
 		
		NGin::Math::Matrix MatScale;
		MatScale.Scale(Scale);

		Transform = Transform * MatScale;
 		

// 		D3DXMATRIX MatRotation, MatScale, MatTranslation;
// 		D3DXMatrixRotationYawPitchRoll(&MatRotation, Rotation.Y * NGin::DEGTORAD, Rotation.X * NGin::DEGTORAD, Rotation.Z * NGin::DEGTORAD);
// 		D3DXMatrixScaling(&MatScale, Scale.X, Scale.Y, Scale.Z);
// 		D3DXMatrixTranslation(&MatTranslation, Position.X, Position.Y, Position.Z);
// 
// 		D3DXMATRIX MatFinal;
// 
// 		D3DXMatrixMultiply(&MatFinal, &MatTranslation, &MatRotation);
// 		//D3DXMatrixMultiply(&MatFinal, &MatFinal, &MatScale);
// 		MatFinal.m[0][0] = Scale.X;
// 		MatFinal.m[1][1] = Scale.Y;
// 		MatFinal.m[2][2] = Scale.Z;
// 		MatFinal.m[3][3] = 1.0f;
// 
// 		memcpy(Transform.M, MatFinal.m, 16 * 4);

		if(CollisionsActive)
			UpdateCollisions(true);
	}

	void CTreeInstance::SetPosition(NGin::Math::Vector3 &position)
	{
		// Don't move the tree outside its bounds. Later, this will move it to a new zone
		if(ParentSector != 0)
			if(position.X < ParentSector->Min.X
				|| position.Z < ParentSector->Min.Y
				|| position.X > ParentSector->Max.X
				|| position.Z > ParentSector->Max.Y)
				return;

		Position = position;
		RebuildTransform();
		if(ParentSector != 0)
			ParentSector->InstanceReBuild();
	}

	void CTreeInstance::SetScale(NGin::Math::Vector3 &scale)
	{
		Scale = scale;
		RebuildTransform();
		if(ParentSector != 0)
			ParentSector->InstanceReBuild();
	}

	void CTreeInstance::SetRotation(NGin::Math::Vector3 &rotation)
	{
		Rotation = rotation;
		RebuildTransform();
	}

	ITreeType* CTreeInstance::GetType()
	{
		return Type;
	}

	NGin::Math::Vector3 CTreeInstance::GetRotation() const
	{
		return Rotation;
	}


	NGin::Math::Vector3 CTreeInstance::GetScale() const
	{
		return Scale;
	}

	NGin::Math::Matrix CTreeInstance::GetTransform() const
	{
		return Transform;
	}

	STreeSector* CTreeInstance::GetParentSector() const
	{
		return ParentSector;
	}

	void CTreeInstance::Destroy()
	{
		if(ParentSector != 0)
			ParentSector->Zone->Remove(this);
	}
}
