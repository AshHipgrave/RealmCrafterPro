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

#include "NVectorConverter.h"
#include "TreeInstance.h"
#include <ITreeManager.h>

using namespace LT;
using namespace System;
using namespace System::Runtime::InteropServices;

namespace LTNet
{
	ref class TreeInstance;

	public enum ETreeCollisionType
	{
		TCT_Sphere = 0,
		TCT_Capsule = 1,
		TCT_Box = 2,
	};

	public ref class TreeCollisionData
	{
	public:

		ref class DataInstance
		{
		public:
			ETreeCollisionType Type;
			float OffsetX;
			float OffsetY;
			float OffsetZ;
			float DimensionsX;
			float DimensionsY;
			float DimensionsZ;
		};

		cli::array<DataInstance^>^ Instances;
		unsigned int InstanceCount;

		TreeCollisionData()
			: Instances(nullptr), InstanceCount(0)
		{
		}
	};

	public ref class CollisionChangedEventArgs : public System::EventArgs
	{
	protected:

		TreeCollisionData^ _CollisionData;
		TreeInstance^ _TreeInstance;

	public:

		CollisionChangedEventArgs(TreeCollisionData^ collisionData, TreeInstance^ treeInstance)
			: _CollisionData(collisionData), _TreeInstance(treeInstance)
		{
		}

		property TreeCollisionData^ CollisionData
		{
			TreeCollisionData^ get()
			{
				return _CollisionData;
			}
		}

		property LTNet::TreeInstance^ TreeInstance
		{
			LTNet::TreeInstance^ get()
			{
				return _TreeInstance;
			}
		}
	};
}