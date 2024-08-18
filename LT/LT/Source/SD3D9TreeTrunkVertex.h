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

#include <d3dx9.h>

namespace LT
{
	const FLOAT NormalToByte(128.0f);
	const D3DXVECTOR3 NormalToPositive(1.0f, 1.0f, 1.0f);

	//! D3D9 Compatible vertex structure for trunks
	struct SD3D9TreeTrunkVertex
	{
		//! Position
		float PositionX, PositionY, PositionZ;

		//! TexCoords
		D3DXFLOAT16 TexCoordU, TexCoordV;

		//! Normals
		unsigned char NormalX, NormalY, NormalZ, NormalA__Undefined;

		//! BiNormals
		unsigned char BiNormalX, BiNormalY, BiNormalZ, BiNormalA__Undefined;

		//! Tangents
		unsigned char TangentX, TangentY, TangentZ, TangentA__Undenfined;

		SD3D9TreeTrunkVertex()
		{
			PositionX = PositionY = PositionZ = 0.0f;
			TexCoordU = TexCoordV = 0;
			NormalX = NormalY = NormalZ = NormalA__Undefined = 0;
			BiNormalX = BiNormalY = BiNormalZ = BiNormalA__Undefined = 0;
			TangentX = TangentY = TangentZ = TangentA__Undenfined = 0;
		}

		inline void SetPosition(const D3DXVECTOR3& position)
		{
			PositionX = position.x;
			PositionY = position.y;
			PositionZ = position.z;
		}

		inline void SetTexCoord(float u, float v)
		{
			D3DXFloat32To16Array(&TexCoordU, &u, 1);
			D3DXFloat32To16Array(&TexCoordV, &v, 1);
		}

		inline void SetNormal(const D3DXVECTOR3& normal)
		{
			D3DXVECTOR3 Set;
			D3DXVec3Add(&Set, &normal, &NormalToPositive);
			D3DXVec3Scale(&Set, &Set, NormalToByte);

			NormalX = (unsigned char)(unsigned int)Set.x;
			NormalY = (unsigned char)(unsigned int)Set.y;
			NormalZ = (unsigned char)(unsigned int)Set.z;
		}

		inline void SetBiNormal(const D3DXVECTOR3& biNormal)
		{
			D3DXVECTOR3 Set;
			D3DXVec3Add(&Set, &biNormal, &NormalToPositive);
			D3DXVec3Scale(&Set, &Set, NormalToByte);

			BiNormalX = (unsigned char)(unsigned int)Set.x;
			BiNormalY = (unsigned char)(unsigned int)Set.y;
			BiNormalZ = (unsigned char)(unsigned int)Set.z;
		}

		inline void SetTangent(const D3DXVECTOR3& tangent)
		{
			D3DXVECTOR3 Set;
			D3DXVec3Add(&Set, &tangent, &NormalToPositive);
			D3DXVec3Scale(&Set, &Set, NormalToByte);

			TangentX = (unsigned char)(unsigned int)Set.x;
			TangentY = (unsigned char)(unsigned int)Set.y;
			TangentZ = (unsigned char)(unsigned int)Set.z;
		}

	};
}
