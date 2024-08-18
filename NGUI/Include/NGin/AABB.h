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

#include <string>
#include "Globals.h"
#include "Vector3.h"
#include "matrix.h"

namespace NGin
{
	namespace Math
	{
		
		//! Axis Aligned Bounding Box
		class AABB
		{
		public:
			Vector3 _Min, _Max;
			bool _SetFirst;

		public:

			//! Default Constructor.
			AABB() : _Min(0.0f, 0.0f, 0.0f), _Max(0.0f, 0.0f, 0.0f), _SetFirst(false) {};

			//! Custom Constructor.
			AABB(Vector3& Min, Vector3& Max) : _Min(Min), _Max(Max), _SetFirst(true) {};

			//! Get the minimum point of the box.
			Vector3 Min() { return _Min; }

			//! Set the minimum point of the box
			void Min(Vector3& New) { _Min = New; }

			//! Get the maximum point of the box.
			Vector3 Max() { return _Max; }

			//! Set the maximum point of the box
			void Max(Vector3& New) { _Max = New; }

			bool IntersectsWithBox(AABB& Other)
			{
				return (_Min <= Other._Max && _Max >= Other._Min);
			}

			//! Transform by a matrix
			virtual void Transform(NGin::Math::Matrix& mat)
			{
				Vector3 Edges[8];
				Edges[0].Reset(_Max.X, _Max.Y, _Max.Z);
				Edges[1].Reset(_Max.X, _Min.Y, _Max.Z);
				Edges[2].Reset(_Max.X, _Max.Y, _Min.Z);
				Edges[3].Reset(_Max.X, _Min.Y, _Min.Z);
				Edges[4].Reset(_Min.X, _Max.Y, _Max.Z);
				Edges[5].Reset(_Min.X, _Min.Y, _Max.Z);
				Edges[6].Reset(_Min.X, _Max.Y, _Min.Z);
				Edges[7].Reset(_Min.X, _Min.Y, _Min.Z);

				for(int i = 0; i < 8; ++i)
					mat.TransformVector(&(Edges[i]));

				_Min = Edges[0];
				_Max = Edges[0];

				for(int i = 1; i < 8; ++i)
					AddPoint(Edges[i]);
			}


			//! Add a point to the box.
			virtual void AddPoint(Vector3& Point)
			{
				AddPoint(Point.X, Point.Y, Point.Z);
			}

			//! Add a point to the box.
			virtual void AddPoint(float X, float Y, float Z)
			{
				if(!_SetFirst)
				{
					_Min = _Max = Vector3(X, Y, Z);
					_SetFirst = true;
				}

				if(X > _Max.X) _Max.X = X;
				if(Y > _Max.Y) _Max.Y = Y;
				if(Z > _Max.Z) _Max.Z = Z;
				if(X < _Min.X) _Min.X = X;
				if(Y < _Min.Y) _Min.Y = Y;
				if(Z < _Min.Z) _Min.Z = Z;
			}

			//! Obtain information in string form.
			virtual std::string ToString()
			{
				char Str[24];sprintf(Str, "%i", _SetFirst ? 1 : 0);
				return std::string("(Min: ") + _Min.ToString() + "; Max: " + _Max.ToString() + "; SetFirst: " + Str + ")";
			}

		};



	} // NGin::Math
}// NGin

