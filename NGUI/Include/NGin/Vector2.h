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

		//! 2-Dimensional Vector
		struct Vector2
		{
			
#pragma region Member Variables
			//! X Position
			float X;

			//! Y Position
			float Y;
#pragma endregion
#pragma region Constructors

			Vector2(float StartX, float StartY) : X(StartX), Y(StartY) {};
			Vector2() : X(0.0f), Y(0.0f) {};
#pragma endregion
#pragma region Methods
			//! Normalize the vector
			void Normalize()
			{
				float Magnitude = (float)sqrt(X * X + Y * Y );

				X /= Magnitude;
				Y /= Magnitude;
			}

			//! Empty the vector
			void Reset()
			{
				X = 0.0f;
				Y = 0.0f;
			}

			//! Obtain information in string form
			std::string ToString()
			{
				char Str[128];
				sprintf_s(Str, "%f, %f", X, Y);

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
			Vector2 operator +(Vector2& Other)
			{
				return Vector2(X + Other.X, Y + Other.Y);
			}

			//! Addition operator
			Vector2& operator +=(Vector2 &Other)
			{
				X += Other.X;
				Y += Other.Y;
				return *this;
			}

			//! Subtraction operator
			Vector2 operator -(Vector2& Other)
			{
				return Vector2(X - Other.X, Y - Other.Y);
			}

			//! Subtraction operator
			Vector2& operator -=(Vector2 &Other)
			{
				X -= Other.X;
				Y -= Other.Y;
				return *this;
			}

			//! Multiplication operator
			Vector2 operator *(Vector2 &Other)
			{
				return Vector2(X * Other.X, Y * Other.Y);
			}

			//! Multiplication operator
			Vector2& operator *=(Vector2 &Other)
			{
				X *= Other.X;
				Y *= Other.Y;
				return *this;
			}

			//! Integer Scale
			Vector2 operator *(int Scale)
			{
				return Vector2(X * Scale, Y * Scale);
			}

			//! Float Scale
			Vector2 operator *(float Scale)
			{
				return Vector2(X * Scale, Y * Scale);
			}

			
			//! Integer Scale
			Vector2& operator *=(int Scale)
			{
				X *= Scale;
				Y *= Scale;
				return *this;
			}

			//! Float Scale
			Vector2 operator *=(float Scale)
			{
				X *= Scale;
				Y *= Scale;
				return *this;
			}

			//! Division operator
			Vector2 operator /(Vector2 &Other)
			{
				return Vector2(X / Other.X, Y / Other.Y);
			}

			//! Division operator
			Vector2& operator /=(Vector2 &Other)
			{
				X /= Other.X;
				Y /= Other.Y;
				return *this;
			}

			//! Integer Scale
			Vector2 operator /(int Scale)
			{
				return Vector2(X / Scale, Y / Scale);
			}

			//! Float Scale
			Vector2 operator /(float Scale)
			{
				return Vector2(X / Scale, Y / Scale);
			}

			//! Integer Scale
			Vector2& operator /=(int Scale)
			{
				X /= Scale;
				Y /= Scale;
				return *this;
			}

			//! Float Scale
			Vector2 operator /=(float Scale)
			{
				X /= Scale;
				Y /= Scale;
				return *this;
			}

			//! Compare two vectors
			bool operator ==(Vector2 &Other)
			{
				return FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y);
			}

			//! Not-Compare two vectors
			bool operator !=(Vector2 &Other)
			{
				return !(FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y));
			}

			//! MoreThan-Compare two vectors
			bool operator >(Vector2 &Other)
			{
				return X > Other.X && Y > Other.Y;
			}

			//! LessThan-Compare two vectors
			bool operator <(Vector2 &Other)
			{
				return X < Other.X && Y < Other.Y;
			}

			//! MoreThanEqual-Compare two vectors
			bool operator >=(Vector2 &Other)
			{
				return X >= Other.X && Y >= Other.Y;
			}

			//! LessThanEqual-Compare two vectors
			bool operator <=(Vector2 &Other)
			{
				return X <= Other.X && Y <= Other.Y;
			}

			//! Set the vector to a new value
			Vector2& operator =(const Vector2 &Other)
			{
				X = Other.X;
				Y = Other.Y;
				return *this;
			}

			//! Obtain the inverse vector
			Vector2 operator -()
			{
				return Vector2(-X, -Y);
			}

#pragma endregion
		};

	}
}
