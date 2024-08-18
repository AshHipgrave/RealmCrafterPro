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
#include "math.h"
#include "Vector3.h"
#include "Matrix.h"

namespace NGin
{
	namespace Math
	{

		class Matrix;

		//! Quaternion Class
		struct Quaternion
		{
			
#pragma region Static Members

			//! Creates a quaternion from using a rotation about a specific axis
			static Quaternion CreateFromAxisAngle(Math::Vector3 &Axis, float Angle)
			{
				float HalfRads = (Angle * DEGTORAD) / 2.0f;
				float Scale = sin(HalfRads);
				
				return Quaternion(Axis.X * Scale,
					Axis.Y * Scale,
					Axis.Z * Scale,
					cos(HalfRads));
			}

#pragma endregion
#pragma region Member Variables
			float X, Y, Z, W;
#pragma endregion
#pragma region Constructors

			Quaternion(float StartX, float StartY, float StartZ, float StartW) : X(StartX), Y(StartY), Z(StartZ), W(StartW) {};
			Quaternion() : X(0.0f), Y(0.0f), Z(0.0f), W(1.0f) {};
#pragma endregion
#pragma region Methods

			//! Obtain information in string form
			std::string ToString()
			{
				char Str[128];
				sprintf_s(Str, "%f, %f, %f, %f", X, Y, Z, W);

				return Str;
			}

			//! Find the rotation matrix of this quaternion
			Math::Matrix ToMatrix()
			{
				Math::Matrix Result;

				//float xx = X * X;
				//float yy = Y * Y;
				//float zz = Z * Z;
				//float xy = X * Y;
				//float zw = Z * W;
				//float zx = Z * X;
				//float yw = Y * W;
				//float yz = Y * Z;
				//float xw = Z * W;

				//Result._11 = 1.0f - (2.0f * (yy + zz));
				//Result._12 = 2.0f * (xy + zw);
				//Result._13 = 2.0f * (zx - yw);
				//Result._14 = 0.0f;
				//Result._21 = 2.0f * (xy - zw);
				//Result._22 = 1.0f - (2.0f * (zz + xx));
				//Result._23 = 2.0f * (yz + xw);
				//Result._24 = 0.0f;
				//Result._31 = 2.0f * (zx + yw);
				//Result._32 = 2.0f * (yz - xw);
				//Result._33 = 1.0f - (2.0f * (yy + xx));
				//Result._34 = 0.0f;
				//Result._41 = 0.0f;
				//Result._42 = 0.0f;
				//Result._43 = 0.0f;
				//Result._44 = 1.0f;

							
				//Result._11 = 1.0f - 2.0f*Y*Y - 2.0f*Z*Z;
				//Result._12 = 2.0f*X*Y + 2.0f*Z*W;
				//Result._13 = 2.0f*X*Z - 2.0f*Y*W;
				//Result._14 = 0.0f;

				//Result._21 = 2.0f*X*Y - 2.0f*Z*W;
				//Result._22 = 1.0f - 2.0f*X*X - 2.0f*Z*Z;
				//Result._23 = 2.0f*Z*Y + 2.0f*X*W;
				//Result._24 = 0.0f;

				//Result._31 = 2.0f*X*Z + 2.0f*Y*W;
				//Result._32 = 2.0f*Z*Y - 2.0f*X*W;
				//Result._33 = 1.0f - 2.0f*X*X - 2.0f*Y*Y;
				//Result._34 = 0.0f;

				//Result._41 = 0.0f;
				//Result._42 = 0.0f;
				//Result._43 = 0.0f;
				//Result._44 = 1.0f;

				Result._11 = 1.0f - 2.0f*Y*Y - 2.0f*Z*Z;
				Result._21 = 2.0f*X*Y + 2.0f*Z*W;
				Result._31 = 2.0f*X*Z - 2.0f*Y*W;
				Result._41 = 0.0f;

				Result._12 = 2.0f*X*Y - 2.0f*Z*W;
				Result._22 = 1.0f - 2.0f*X*X - 2.0f*Z*Z;
				Result._32 = 2.0f*Z*Y + 2.0f*X*W;
				Result._42 = 0.0f;

				Result._13 = 2.0f*X*Z + 2.0f*Y*W;
				Result._23 = 2.0f*Z*Y - 2.0f*X*W;
				Result._33 = 1.0f - 2.0f*X*X - 2.0f*Y*Y;
				Result._43 = 0.0f;

				Result._14 = 0.0f;
				Result._24 = 0.0f;
				Result._34 = 0.0f;
				Result._44 = 1.0f;

				return Result;

				//Math::Matrix Mat;

				//Mat[0] = 1.0f - 2.0f * (Y * Y + Z * Z);
				//Mat[1] = 2.0f * (X * Y - W * Z);
				//Mat[2] = 2.0f * (X * Z + W * Y);

				//Mat[4] = 2.0f * (X * Y + W * Z);
				//Mat[5] = 1.0f - 2.0f * (X * X + Z * Z);
				//Mat[6] = 2.0f * (Y * Z - W * X);

				//Mat[8]  = 2.0f * (X * Z - W * Y);
				//Mat[9]  = 2.0f * (Y * Z + W * X);
				//Mat[10] = 1.0f - 2.0f * (X * X + Y * Y);

				//return Mat;
			}

			//! Directly set values
			void Set(float NewX, float NewY, float NewZ, float NewW)
			{
				X = NewX;
				Y = NewY;
				Z = NewZ;
				W = NewW;
			}

			inline bool FloatEqual(float Float1, float Float2)
			{
				if(Float2 > Float1 - NGIN_FP_TOLERANCE && Float2 < Float1 + NGIN_FP_TOLERANCE)
					return true;
				return false;
			}

#pragma endregion
#pragma region Operators

			//! Multiplication operator
			Quaternion operator *(Quaternion &Other)
			{
				Quaternion tmp;

				tmp.W = (Other.W * W) - (Other.X * X) - (Other.Y * Y) - (Other.Z * Z);
				tmp.X = (Other.W * X) + (Other.X * W) + (Other.Y * Z) - (Other.Z * Y);
				tmp.Y = (Other.W * Y) + (Other.Y * W) + (Other.Z * X) - (Other.X * Z);
				tmp.Z = (Other.W * Z) + (Other.Z * W) + (Other.X * Y) - (Other.Y * X);

				return tmp;

				Quaternion Out;
				float rx = Other.X;
				float ry = Other.Y;
				float rz = Other.Z;
				float rw = Other.W;
				float lx = X;
				float ly = Y;
				float lz = Z;
				float lw = W;
				float yz = (ry * lz) - (rz * ly);
				float xz = (rz * lx) - (rx * lz);
				float xy = (rx * ly) - (ry * lx);
				float length = ((rx * lx) + (ry * ly)) + (rz * lz);

				Out.X = ((rx * lw) + (lx * rw)) + yz;
				Out.Y = ((ry * lw) + (ly * rw)) + xz;
				Out.Z = ((rz * lw) + (lz * rw)) + xy;
				Out.W = (rw * lw) - length;

				return Out;

				//Quaternion New;
				//New.Set((Other.W * X) + (Other.X * W) + (Other.Y * Z) - (Other.Z * Y),
				//		(Other.W * Y) + (Other.Y * W) + (Other.Z * X) - (Other.X * Z),
				//		(Other.W * Z) + (Other.Z * W) + (Other.X * Y) - (Other.Y * X),
				//		(Other.W * W) - (Other.X * X) - (Other.Y * Y) - (Other.Z * Z));
				//return New;
			}

			//! Multiplication operator
			Quaternion& operator *=(Quaternion &Other)
			{
				*this = *this * Other;
				return *this;
			}

			//! Set the quaternion to a new value
			Quaternion& operator =(Quaternion &Other)
			{
				X = Other.X;
				Y = Other.Y;
				Z = Other.Z;
				W = Other.W;
				return *this;
			}

			//! Add two quaternions
			Quaternion operator +(Quaternion &Other)
			{
				return Quaternion(X + Other.X, Y + Other.Y, Z + Other.Z, W + Other.W);
			}

#pragma endregion
		};

	}
}
