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
#include "Stdafx.h"
#include "TreeInstance.h"

namespace LTNet
{

	TreeInstance::TreeInstance(ITreeInstance* handle)
		: _TreeInstance(handle), _Tag(nullptr)
	{

	}

// 	System::Object^ TreeInstance::Tag::get()
// 	{
// 		return _Tag;
// 	}
// 
// 	void TreeInstance::Tag::set(System::Object^ object)
// 	{
// 		_Tag = object;
// 	}

	unsigned int TreeInstance::IntTag::get()
	{
		return reinterpret_cast<unsigned int>(_TreeInstance->GetTag());
	}

	void TreeInstance::IntTag::set(unsigned int value)
	{
		_TreeInstance->SetTag(reinterpret_cast<void*>(value));
	}

	void TreeInstance::Destroy()
	{
		_TreeInstance->Destroy();
	}


	bool TreeInstance::operator==(TreeInstance^ a, TreeInstance^ b)
	{
		bool anull = false;
		bool bnull = false;

		try
		{
			unsigned int N = a->IntTag;
		}catch(NullReferenceException^ e)
		{
			(void)e;
			anull = true;
		}

		try
		{
			unsigned int N = b->IntTag;
		}catch(NullReferenceException^ e)
		{
			(void)e;
			bnull = true;
		}

		if(anull && bnull)
			return true;

		if(anull || bnull)
			return false;

		return a->_TreeInstance == b->_TreeInstance;
	}

	bool TreeInstance::operator!=(TreeInstance^ a, TreeInstance^ b)
	{
		return !(a->_TreeInstance == b->_TreeInstance);
	}

	void TreeInstance::SetPosition(float x, float y, float z)
	{
		_TreeInstance->SetPosition(NGin::Math::Vector3(x, y, z));
	}

	void TreeInstance::SetScale(float x, float y, float z)
	{
		_TreeInstance->SetScale(NGin::Math::Vector3(x, y, z));
	}

	void TreeInstance::SetRotation(float x, float y, float z)
	{
		//NGin::Math::Matrix M;
		//M.RotationDeg(NGin::Math::Vector3(x, y, z));

		//_TreeInstance->SetRotation(M.Rotation());

// 		_TreeInstance->SetRotation(
// 			NGin::Math::Quaternion::CreateFromAxisAngle(NGin::Math::Vector3(1, 0, 0), x)
// 			* NGin::Math::Quaternion::CreateFromAxisAngle(NGin::Math::Vector3(0, 1, 0), y)
// 			* NGin::Math::Quaternion::CreateFromAxisAngle(NGin::Math::Vector3(0, 0, 1), z));
		_TreeInstance->SetRotation(NGin::Math::Vector3(x, y, z));
	}


	// 		void SetRotation(NGUINet::Quaternion^ rotation);
	// 		NGUINet::Matrix^ GetTransform();

	TreeType^ TreeInstance::GetTreeType()
	{
		return gcnew TreeType(_TreeInstance->GetType());
	}

	float TreeInstance::X()
	{
		NGin::Math::Matrix Transform = _TreeInstance->GetTransform();
		NGin::Math::Vector3 Position = Transform.Translation();

		return Position.X;
	}

	float TreeInstance::Y()
	{
		NGin::Math::Matrix Transform = _TreeInstance->GetTransform();
		NGin::Math::Vector3 Position = Transform.Translation();

		return Position.Y;
	}

	float TreeInstance::Z()
	{
		NGin::Math::Matrix Transform = _TreeInstance->GetTransform();
		NGin::Math::Vector3 Position = Transform.Translation();

		return Position.Z;
	}

	float TreeInstance::Pitch()
	{
		NGin::Math::Vector3 Rotation = _TreeInstance->GetRotation();

		return Rotation.X;
	}

	float TreeInstance::Yaw()
	{
		NGin::Math::Vector3 Rotation = _TreeInstance->GetRotation();

		return Rotation.Y;
	}

	float TreeInstance::Roll()
	{
		NGin::Math::Vector3 Rotation = _TreeInstance->GetRotation();

		return Rotation.Z;
	}

	float TreeInstance::ScaleX()
	{
		NGin::Math::Vector3 Scale = _TreeInstance->GetScale();

		return Scale.X;
	}

	float TreeInstance::ScaleY()
	{
		NGin::Math::Vector3 Scale = _TreeInstance->GetScale();

		return Scale.Y;
	}

	float TreeInstance::ScaleZ()
	{
		NGin::Math::Vector3 Scale = _TreeInstance->GetScale();

		return Scale.Z;
	}

}