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
#include "LT.h"
#include "Default Project.h"

// List of active trees (for sectors)
std::list<unsigned int> TreesList;

void TreeManager_CollisionChanged(LT::ITreeZone* sender, LT::CollisionEventArgs* e)
{
	// Reject if is unusable
	if(e == 0)
		return;

	const LT::STreeCollisionData* CollisionData = e->GetCollisionData();

	// Instance removal
	if(CollisionData == 0)
	{
		uint MasterPivot = reinterpret_cast<uint>(e->GetTreeInstance()->GetTag());
		if(MasterPivot != 0)
		{
			std::string ETag = EntityTag(MasterPivot);
			if(ETag.length() > 0)
			{
				NGin::Math::Vector3* Pos = reinterpret_cast<NGin::Math::Vector3*>(toInt(ETag));
				if(Pos != NULL)
					delete Pos;
			}

			TreesList.remove(MasterPivot);
			FreeEntity(MasterPivot);
		}
		return;
	}

	if(CollisionData->InstanceCount == 0)
		return;

	// New Instance
	uint MasterPivot = reinterpret_cast<uint>(e->GetTreeInstance()->GetTag());
	if(MasterPivot != 0)
	{
		std::string ETag = EntityTag(MasterPivot);
		if(ETag.length() > 0)
		{
			NGin::Math::Vector3* Pos = reinterpret_cast<NGin::Math::Vector3*>(toInt(ETag));
			if(Pos != NULL)
				delete Pos;
		}

		FreeEntity(MasterPivot);
		TreesList.remove(MasterPivot);
	}
	MasterPivot = CreatePivot();
	TreesList.push_back(MasterPivot);
	EntityType(MasterPivot, 2);

	uint Desc = bbdx2_CreatePhysicsDesc(1, MasterPivot);

	for(uint i = 0; i < CollisionData->InstanceCount; ++i)
	{
		switch(CollisionData->Instances[i].Type)
		{
		case LT::TCT_Sphere:
			{
				float Low = CollisionData->Instances[i].Dimensions.X;
				if(CollisionData->Instances[i].Dimensions.Z < Low)
					Low = CollisionData->Instances[i].Dimensions.Z;
				if(CollisionData->Instances[i].Dimensions.Y < Low)
					Low = CollisionData->Instances[i].Dimensions.Y;

				bbdx2_AddSphere(Desc,
					CollisionData->Instances[i].Offset.X, CollisionData->Instances[i].Offset.Y, CollisionData->Instances[i].Offset.Z,
					Low, -1.0f);

				break;
			}
		case LT::TCT_Capsule:
			{
				float Low = CollisionData->Instances[i].Dimensions.X;
				if(CollisionData->Instances[i].Dimensions.Z < Low)
					Low = CollisionData->Instances[i].Dimensions.Z;

				bbdx2_AddCapsule(Desc,
					CollisionData->Instances[i].Offset.X, CollisionData->Instances[i].Offset.Y, CollisionData->Instances[i].Offset.Z,
					Low, CollisionData->Instances[i].Dimensions.Y, -1.0f);

				break;
			}
		case LT::TCT_Box:
			{
				NGin::Math::Vector3 Min = CollisionData->Instances[i].Offset - (CollisionData->Instances[i].Dimensions * 0.5f);

				bbdx2_AddBox(Desc,
					CollisionData->Instances[i].Offset.X,
					CollisionData->Instances[i].Offset.Y,
					CollisionData->Instances[i].Offset.Z,
					CollisionData->Instances[i].Dimensions.X,
					CollisionData->Instances[i].Dimensions.Y,
					CollisionData->Instances[i].Dimensions.Z,
					-1.0f);

				break;
			}
		}
	}

	bbdx2_ClosePhysicsDesc(Desc);

	const LT::ITreeInstance* Instance = e->GetTreeInstance();
	NGin::Math::Matrix Transform = Instance->GetTransform();
	NGin::Math::Vector3 Position = Transform.Translation();
	NGin::Math::Vector3 Rotation = Transform.RotationDeg();
	NGin::Math::Vector3* WorldPosition = new NGin::Math::Vector3(Position);

	if(Me != NULL)
	{
		Position.X -= RealmCrafter::SectorVector::SectorSize * Me->Position.SectorX;
		Position.Z -= RealmCrafter::SectorVector::SectorSize * Me->Position.SectorZ;
	}

	PositionEntity(MasterPivot, Position.X, Position.Y, Position.Z);
	RotateEntity(MasterPivot, Rotation.X, Rotation.Y, Rotation.Z);
	TagEntity(MasterPivot, toString(reinterpret_cast<int>(WorldPosition)));

	// Naughty cast, you're not supposed to edit during the update but we know that
	// SetTag only touches a user variable
	((LT::ITreeInstance*)Instance)->SetTag(reinterpret_cast<void*>(MasterPivot));
}