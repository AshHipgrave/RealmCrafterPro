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

#include <AABB.h>
#include <d3dx9.h>

namespace RealmCrafter
{
	namespace RCT
	{

		// View Frustum class
		class Frustum
		{
		public:

			// Planes that make frustum
			D3DXPLANE Planes[6];

			// Set this Frustums' values using the combined VP
			void FromViewProjection(D3DXMATRIX &viewProjection)
			{
				// Left plane
				Planes[0].a = viewProjection._14 + viewProjection._11;
				Planes[0].b = viewProjection._24 + viewProjection._21;
				Planes[0].c = viewProjection._34 + viewProjection._31;
				Planes[0].d = viewProjection._44 + viewProjection._41;

				// Right plane
				Planes[1].a = viewProjection._14 - viewProjection._11;  
				Planes[1].b = viewProjection._24 - viewProjection._21;
				Planes[1].c = viewProjection._34 - viewProjection._31;
				Planes[1].d = viewProjection._44 - viewProjection._41;

				// Top plane
				Planes[2].a = viewProjection._14 - viewProjection._12;  
				Planes[2].b = viewProjection._24 - viewProjection._22;
				Planes[2].c = viewProjection._34 - viewProjection._32;
				Planes[2].d = viewProjection._44 - viewProjection._42;

				// Bottom plane
				Planes[3].a = viewProjection._14 + viewProjection._12;  
				Planes[3].b = viewProjection._24 + viewProjection._22;
				Planes[3].c = viewProjection._34 + viewProjection._32;
				Planes[3].d = viewProjection._44 + viewProjection._42;

				// Near plane
				Planes[4].a = viewProjection._13;  
				Planes[4].b = viewProjection._23;
				Planes[4].c = viewProjection._33;
				Planes[4].d = viewProjection._43;

				// Far plane
				Planes[5].a = viewProjection._14 - viewProjection._13;  
				Planes[5].b = viewProjection._24 - viewProjection._23;
				Planes[5].c = viewProjection._34 - viewProjection._33;
				Planes[5].d = viewProjection._44 - viewProjection._43; 

				for(int i = 0; i < 6; ++i)
					D3DXPlaneNormalize(&Planes[i], &Planes[i]);
			}

			// Check if a sphere is in the frustum
			bool SphereInFrustum(NGin::Math::Vector3 &Point, float Radius)
			{
				D3DXVECTOR4 P(Point.X, Point.Y, Point.Z, 1.0f);
				for(int i = 0; i < 6; ++i)
					if(D3DXPlaneDot(&Planes[i], &P) + Radius < 0.0f)
						return false;
				return true;
			}

			// Check if a Box is in the Frustum
			int BoxInFrustum(NGin::Math::AABB &Box)
			{
				bool Intersect = false;
				int Result = 0;
				NGin::Math::Vector3 Min, Max;

				for(int i = 0; i < 6; ++i)
				{
					if(Planes[i].a > 0)
					{
						Min.X = Box._Min.X;
						Max.X = Box._Max.X;
					}
					else
					{
						Min.X = Box._Max.X;
						Max.X = Box._Min.X;
					}

					if(Planes[i].b > 0)
					{
						Min.Y = Box._Min.Y;
						Max.Y = Box._Max.Y;
					}
					else
					{
						Min.Y = Box._Max.Y;
						Max.Y = Box._Min.Y;
					}

					if(Planes[i].c > 0)
					{
						Min.Z = Box._Min.Z;
						Max.Z = Box._Max.Z;
					}
					else
					{
						Min.Z = Box._Max.Z;
						Max.Z = Box._Min.Z;
					}

					D3DXVECTOR3 pMin(Min.X, Min.Y, Min.Z);
					D3DXVECTOR3 pMax(Max.X, Max.Y, Max.Z);

					D3DXVECTOR4 p4Min(Min.X, Min.Y, Min.Z, 1);
					D3DXVECTOR4 p4Max(Max.X, Max.Y, Max.Z, 1);

					D3DXVECTOR3 pNorm(Planes[i].a, Planes[i].b, Planes[i].c);

					if(D3DXPlaneDotCoord(&Planes[i], &pMax) < 0)
					{
						Result = 0;
						return Result;
					}

					if(D3DXPlaneDotCoord(&Planes[i], &pMin) < 0)
						Intersect = true;
				}

				if(Intersect)
					Result = 1;
				else
					Result = 2;

				return Result;
			}
		};
	}
}
