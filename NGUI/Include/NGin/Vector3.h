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

		//! 3-Dimensional Vector
		struct Vector3
		{
			
#pragma region Static Members

			//! Returns the vector of "up"
			static Vector3 Up()
			{
				return Vector3(0.0f, 1.0f, 0.0f);
			}

#pragma endregion
#pragma region Member Variables
			//! X Position
			float X;

			//! Y Position
			float Y;

			//! Z Position
			float Z;
#pragma endregion
#pragma region Constructors

			Vector3(float StartX, float StartY, float StartZ) : X(StartX), Y(StartY), Z(StartZ) {};
			Vector3() : X(0.0f), Y(0.0f), Z(0.0f) {};
#pragma endregion
#pragma region Methods

			//! Gets vector length
			float Length()
			{
				return sqrt(X * X + Y * Y + Z * Z);
			}

			//! Gets squared vector length
			float LengthSq()
			{
				return X * X + Y * Y + Z * Z;
			}

			//! If this point is on a line, is it between a start and an end point?
			bool BetweenPoints(Vector3 &Start, Vector3 &End)
			{
				float F = (End - Start).LengthSq();
				return DistanceSq(Start) < F && DistanceSq(End) < F;
			}

			//! Get the dinstance of this vector from another
			float DistanceSq(Vector3 &FarPoint)
			{
				float XDist = X - FarPoint.X;
				float YDist = Y - FarPoint.Y;
				float ZDist = Z - FarPoint.Z;

				return XDist * XDist + YDist * YDist + ZDist * ZDist;
			}

			//! Normalize the vector
			void Normalize()
			{
				float Magnitude = (float)sqrt(X * X + Y * Y + Z * Z);

				X /= Magnitude;
				Y /= Magnitude;
				Z /= Magnitude;
			}

			//! Empty the vector
			void Reset()
			{
				X = 0.0f;
				Y = 0.0f;
				Z = 0.0f;
			}

			//! Reset the vector
			void Reset(float x, float y, float z)
			{
				X = x;
				Y = y;
				Z = z;
			}

			//! Find the cross product of this and another vector
			Vector3 Cross(Vector3 &Other)
			{
				return Vector3(Y * Other.Z - Z * Other.Y,
					Z * Other.X - X * Other.Z,
					X * Other.Y - Y * Other.X);
			}

			//! Find the dot product of this and another vector
			float Dot(Vector3 &Other)
			{
				return X * Other.X + Y * Other.Y + Z * Other.Z;
			}

			//! Obtain information in string form
			std::string ToString()
			{
				char Str[128];
				sprintf_s(Str, "%f, %f, %f", X, Y, Z);

				return Str;
			}

			//! Fix rotation to a usable scale
			void FixDegrees()
			{
				while(X >  90.0f)  X -= 180.0f;
				while(X < -90.0f)  X += 180.0f;
				while(Y >  180.0f) Y -= 360.0f;
				while(Y < -180.0f) Y += 360.0f;
				while(Z >  180.0f) Z -= 360.0f;
				while(Z < -180.0f) Z += 360.0f;
			}

			//! Linear interpolate between this and another
			Vector3 Lerp(Vector3 &other, float d)
			{
				float InvD = 1.0f - d;

				Vector3 Out;
				Out.X = InvD * X + d * other.X;
				Out.Y = InvD * Y + d * other.Y;
				Out.Z = InvD * Z + d * other.Z;

				return Out;
			}

			static inline bool FloatEqual(float Float1, float Float2)
			{
				if(Float2 > Float1 - NGIN_FP_TOLERANCE && Float2 < Float1 + NGIN_FP_TOLERANCE)
					return true;
				return false;
			}

#pragma endregion
#pragma region Operators

			//! Addition operator
			Vector3 operator +(Vector3& Other)
			{
				return Vector3(X + Other.X, Y + Other.Y, Z + Other.Z);
			}

			//! Addition operator
			Vector3& operator +=(Vector3 &Other)
			{
				X += Other.X;
				Y += Other.Y;
				Z += Other.Z;
				return *this;
			}

			//! Subtraction operator
			Vector3 operator -(Vector3& Other)
			{
				return Vector3(X - Other.X, Y - Other.Y, Z - Other.Z);
			}

			//! Subtraction operator
			Vector3& operator -=(Vector3 &Other)
			{
				X -= Other.X;
				Y -= Other.Y;
				Z -= Other.Z;
				return *this;
			}

			//! Multiplication operator
			Vector3 operator *(Vector3 &Other)
			{
				return Vector3(X * Other.X, Y * Other.Y, Z * Other.Z);
			}

			//! Multiplication operator
			Vector3& operator *=(Vector3 &Other)
			{
				X *= Other.X;
				Y *= Other.Y;
				Z *= Other.Z;
				return *this;
			}

			//! Integer Scale
			Vector3 operator *(int Scale)
			{
				return Vector3(X * Scale, Y * Scale, Z * Scale);
			}

			//! Float Scale
			Vector3 operator *(float Scale)
			{
				return Vector3(X * Scale, Y * Scale, Z * Scale);
			}

			
			//! Integer Scale
			Vector3& operator *=(int Scale)
			{
				X *= Scale;
				Y *= Scale;
				Z *= Scale;
				return *this;
			}

			//! Float Scale
			Vector3 operator *=(float Scale)
			{
				X *= Scale;
				Y *= Scale;
				Z *= Scale;
				return *this;
			}

			//! Division operator
			Vector3 operator /(Vector3 &Other)
			{
				return Vector3(X / Other.X, Y / Other.Y, Z / Other.Z);
			}

			//! Division operator
			Vector3& operator /=(Vector3 &Other)
			{
				X /= Other.X;
				Y /= Other.Y;
				Z /= Other.Z;
				return *this;
			}

			//! Integer Scale
			Vector3 operator /(int Scale)
			{
				return Vector3(X / Scale, Y / Scale, Z / Scale);
			}

			//! Float Scale
			Vector3 operator /(float Scale)
			{
				return Vector3(X / Scale, Y / Scale, Z / Scale);
			}

			//! Integer Scale
			Vector3& operator /=(int Scale)
			{
				X /= Scale;
				Y /= Scale;
				Z /= Scale;
				return *this;
			}

			//! Float Scale
			Vector3 operator /=(float Scale)
			{
				X /= Scale;
				Y /= Scale;
				Z /= Scale;
				return *this;
			}

			//! Compare two vectors
			bool operator ==(Vector3 &Other)
			{
				return FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y) && FloatEqual(Z, Other.Z);
			}

			//! Not-Compare two vectors
			bool operator !=(Vector3 &Other)
			{
				return !(FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y) && FloatEqual(Z, Other.Z));
			}

			//! MoreThan-Compare two vectors
			bool operator >(Vector3 &Other)
			{
				return X > Other.X && Y > Other.Y && Z > Other.Z;
			}

			//! LessThan-Compare two vectors
			bool operator <(Vector3 &Other)
			{
				return X < Other.X && Y < Other.Y && Z < Other.Z;
			}

			//! MoreThanEqual-Compare two vectors
			bool operator >=(Vector3 &Other)
			{
				return X >= Other.X && Y >= Other.Y && Z >= Other.Z;
			}

			//! LessThanEqual-Compare two vectors
			bool operator <=(Vector3 &Other)
			{
				return X <= Other.X && Y <= Other.Y && Z <= Other.Z;
			}

			//! Set the vector to a new value
			Vector3& operator =(const Vector3 &Other)
			{
				X = Other.X;
				Y = Other.Y;
				Z = Other.Z;
				return *this;
			}

			//! Obtain the inverse vector
			Vector3 operator -()
			{
				return Vector3(-X, -Y, -Z);
			}

#pragma endregion
		};

	}
}
