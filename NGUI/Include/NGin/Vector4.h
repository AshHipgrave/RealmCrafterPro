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
#include <sstream>
#include "Globals.h"
#include "math.h"

namespace NGin
{
	namespace Math
	{

		//! 4-Dimensional Vector
		struct Vector4
		{
#pragma region Member Variables
			//! X Position
			float X;

			//! Y Position
			float Y;

			//! Z Position
			float Z;

			//! W Position
			float W;
#pragma endregion
#pragma region Constructors

			Vector4(float StartX, float StartY, float StartZ, float StartW) : X(StartX), Y(StartY), Z(StartZ), W(StartW) {};
			Vector4() : X(0.0f), Y(0.0f), Z(0.0f), W(0.0f) {};
#pragma endregion
#pragma region Methods
			//! Normalize the vector
			void Normalize()
			{
				float Magnitude = (float)sqrt(X * X + Y * Y + Z * Z + W * W);

				X /= Magnitude;
				Y /= Magnitude;
				Z /= Magnitude;
				W /= Magnitude;
			}

			//! Empty the vector
			void Reset()
			{
				X = 0.0f;
				Y = 0.0f;
				Z = 0.0f;
				W = 0.0f;
			}

			//! Reset data
			void Reset(float x, float y, float z, float w)
			{
				X = x;
				Y = y;
				Z = z;
				W = w;
			}

			//! Obtain information in string form
			std::string ToString()
			{
				char Str[128];
				sprintf_s(Str, "%f, %f, %f, %f", X, Y, Z, W);

				return Str;
			}

			inline bool FloatEqual(float Float1, float Float2)
			{
				if(Float2 > Float1 - NGIN_FP_TOLERANCE && Float2 < Float1 + NGIN_FP_TOLERANCE)
					return true;
				return false;
			}

#pragma endregion
#pragma region Operators

			//! Addition operator
			Vector4 operator +(Vector4& Other)
			{
				return Vector4(X + Other.X, Y + Other.Y, Z + Other.Z, W + Other.W);
			}

			//! Addition operator
			Vector4& operator +=(Vector4 &Other)
			{
				X += Other.X;
				Y += Other.Y;
				Z += Other.Z;
				W += Other.W;
				return *this;
			}

			//! Subtraction operator
			Vector4 operator -(Vector4& Other)
			{
				return Vector4(X - Other.X, Y - Other.Y, Z - Other.Z, W - Other.W);
			}

			//! Subtraction operator
			Vector4& operator -=(Vector4 &Other)
			{
				X -= Other.X;
				Y -= Other.Y;
				Z -= Other.Z;
				W -= Other.W;
				return *this;
			}

			//! Multiplication operator
			Vector4 operator *(Vector4 &Other)
			{
				return Vector4(X * Other.X, Y * Other.Y, Z * Other.Z, W * Other.W);
			}

			//! Multiplication operator
			Vector4& operator *=(Vector4 &Other)
			{
				X *= Other.X;
				Y *= Other.Y;
				Z *= Other.Z;
				W *= Other.W;
				return *this;
			}

			//! Integer Scale
			Vector4 operator *(int Scale)
			{
				return Vector4(X * Scale, Y * Scale, Z * Scale, W * Scale);
			}

			//! Float Scale
			Vector4 operator *(float Scale)
			{
				return Vector4(X * Scale, Y * Scale, Z * Scale, W * Scale);
			}

			
			//! Integer Scale
			Vector4& operator *=(int Scale)
			{
				X *= Scale;
				Y *= Scale;
				Z *= Scale;
				W *= Scale;
				return *this;
			}

			//! Float Scale
			Vector4 operator *=(float Scale)
			{
				X *= Scale;
				Y *= Scale;
				Z *= Scale;
				W *= Scale;
				return *this;
			}

			//! Division operator
			Vector4 operator /(Vector4 &Other)
			{
				return Vector4(X / Other.X, Y / Other.Y, Z / Other.Z, W / Other.W);
			}

			//! Division operator
			Vector4& operator /=(Vector4 &Other)
			{
				X /= Other.X;
				Y /= Other.Y;
				Z /= Other.Z;
				W /= Other.W;
				return *this;
			}

			//! Integer Scale
			Vector4 operator /(int Scale)
			{
				return Vector4(X / Scale, Y / Scale, Z / Scale, W / Scale);
			}

			//! Float Scale
			Vector4 operator /(float Scale)
			{
				return Vector4(X / Scale, Y / Scale, Z / Scale, W / Scale);
			}

			//! Integer Scale
			Vector4& operator /=(int Scale)
			{
				X /= Scale;
				Y /= Scale;
				Z /= Scale;
				W /= Scale;
				return *this;
			}

			//! Float Scale
			Vector4 operator /=(float Scale)
			{
				X /= Scale;
				Y /= Scale;
				Z /= Scale;
				W /= Scale;
				return *this;
			}

			//! Compare two vectors
			bool operator ==(Vector4 &Other)
			{
				return FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y) && FloatEqual(Z, Other.Z) && FloatEqual(W, Other.W);
			}

			//! Not-Compare two vectors
			bool operator !=(Vector4 &Other)
			{
				return !(FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y) && FloatEqual(Z, Other.Z) && FloatEqual(W, Other.W));
			}

			//! MoreThan-Compare two vectors
			bool operator >(Vector4 &Other)
			{
				return X > Other.X && Y > Other.Y && Z > Other.Z && W > Other.W;
			}

			//! LessThan-Compare two vectors
			bool operator <(Vector4 &Other)
			{
				return X < Other.X && Y < Other.Y && Z < Other.Z && W < Other.W;
			}

			//! MoreThanEqual-Compare two vectors
			bool operator >=(Vector4 &Other)
			{
				return X >= Other.X && Y >= Other.Y && Z >= Other.Z && W >= Other.W;
			}

			//! LessThanEqual-Compare two vectors
			bool operator <=(Vector4 &Other)
			{
				return X <= Other.X && Y <= Other.Y && Z <= Other.Z && W <= Other.W;
			}

			//! Set the vector to a new value
			Vector4& operator =(Vector4 &Other)
			{
				X = Other.X;
				Y = Other.Y;
				Z = Other.Z;
				W = Other.W;
				return *this;
			}

			//! Obtain the inverse vector
			Vector4 operator -()
			{
				return Vector4(-X, -Y, -Z, -W);
			}

#pragma endregion
#pragma region Static Methods
			//! Linearly interpolate between vectors
			static Vector4 Lerp(Vector4 &A, Vector4 &B, float Time)
			{
				return A + ((B - A) * Time);
			}
#pragma endregion
		};

	}
}
