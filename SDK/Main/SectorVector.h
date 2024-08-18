//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#pragma once

#include <string>
#include <Globals.h>

#pragma warning(push)
#pragma warning(disable:4244)

namespace RealmCrafter
{
	//! SectorVector structure
	struct SectorVector
	{
		static const float SectorSize;

#pragma region Member Variables
		//! X Position
		float X;

		//! Y Position
		float Y;

		//! Z Position
		float Z;

		//! SectorX
		unsigned short SectorX;

		//! SectorZ
		unsigned short SectorZ;
#pragma endregion
#pragma region Constructors

		SectorVector(float StartX, float StartY, float StartZ) : X(StartX), Y(StartY), Z(StartZ), SectorX(0), SectorZ(0) { FixValues(); };
		SectorVector(float StartX, float StartY, float StartZ, unsigned short sectorX, unsigned short sectorZ) : X(StartX), Y(StartY), Z(StartZ), SectorX(sectorX), SectorZ(sectorZ) { FixValues(); };
		SectorVector() : X(0.0f), Y(0.0f), Z(0.0f), SectorX(0), SectorZ(0) { FixValues(); };
#pragma endregion
#pragma region Methods

		//! Fix values within correct sector range
		void FixValues()
		{
			while (X > SectorSize)
			{
				++SectorX;
				X -= SectorSize;
			}

			while (X < 0.0f && SectorX > 0)
			{
				--SectorX;
				X += SectorSize;
			}

			while (Z > SectorSize)
			{
				++SectorZ;
				Z -= SectorSize;
			}

			while (Z < 0.0f && SectorZ > 0)
			{
				--SectorZ;
				Z += SectorSize;
			}
		}

		//! Empty the vector
		void Reset()
		{
			X = 0.0f;
			Y = 0.0f;
			Z = 0.0f;
			SectorX = 0;
			SectorZ = 0;
		}

		//! Reset the vector
		void Reset(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
			SectorX = 0;
			SectorZ = 0;
		}

		//! Reset the vector
		void Reset(float x, float y, float z, unsigned short sectorX, unsigned short sectorZ)
		{
			X = x;
			Y = y;
			Z = z;
			SectorX = sectorX;
			SectorZ = sectorZ;
		}



		//! Obtain information in string form
		std::string ToString()
		{
			char Str[128];
			sprintf_s(Str, "%i:%f, %f, %i:%f", SectorX, X, Y, SectorZ, Z);
			
			return Str;
		}

		static inline bool FloatEqual(float Float1, float Float2)
		{
			//if(Float2 > Float1 - NGIN_FP_TOLERANCE && Float2 < Float1 + NGIN_FP_TOLERANCE)
			if(fabs(Float1 - Float2) <= NGIN_FP_TOLERANCE)
				return true;
			return false;
		}


#pragma endregion
#pragma region Operators

		//! Addition operator
		SectorVector operator +(SectorVector& Other)
		{
			return SectorVector(X + Other.X, Y + Other.Y, Z + Other.Z, SectorX + Other.SectorX, SectorZ + Other.SectorZ);
		}

		//! Addition operator
		SectorVector& operator +=(SectorVector &Other)
		{
			X += Other.X;
			Y += Other.Y;
			Z += Other.Z;
			SectorX += Other.SectorX;
			SectorZ += Other.SectorZ;
			FixValues();
			return *this;
		}

		//! Subtraction operator
		static inline SectorVector Subtract(SectorVector &a, SectorVector &b)
		{
			int XDifference = (int)a.SectorX - (int)b.SectorX;
			int ZDifference = (int)a.SectorZ - (int)b.SectorZ;

			// Handle the unsigned error
			if (XDifference < 0)
			{
				b.X = (((float)XDifference) * SectorSize) - b.X;
				XDifference = 0;
			}

			if (ZDifference < 0)
			{
				b.Z = (((float)ZDifference) * SectorSize) - b.Z;
				ZDifference = 0;
			}

			return SectorVector(
				(unsigned short)XDifference,
				(unsigned short)ZDifference,
				a.X - b.X,
				a.Y - b.Y,
				a.Z - b.Z);
		}

		//! Get squared distance from another SectorVector
		float DistanceSq( SectorVector &other )
		{
			SectorVector d = Subtract(other, *this);
			
			float distX = ((float)d.SectorX) * SectorSize;
			float distZ = ((float)d.SectorZ) * SectorSize;
			float distY = d.Y;

			distX += d.X;
			distZ += d.Z;

			return distX * distX + distY * distY + distZ * distZ;
		}

		//! Compare two vectors
		bool operator ==(SectorVector &Other)
		{
			return FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y) && FloatEqual(Z, Other.Z) && SectorX == Other.SectorX && SectorZ == Other.SectorZ;
		}

		//! Not-Compare two vectors
		bool operator !=(SectorVector &Other)
		{
			return !(FloatEqual(X, Other.X) && FloatEqual(Y, Other.Y) && FloatEqual(Z, Other.Z) && SectorX == Other.SectorX && SectorZ == Other.SectorZ);
		}

		//! Set the vector to a new value
		SectorVector& operator =(const SectorVector &Other)
		{
			X = Other.X;
			Y = Other.Y;
			Z = Other.Z;
			SectorX = Other.SectorX;
			SectorZ = Other.SectorZ;
			return *this;
		}

		//! Check if the current value is within the given rectangle
		bool WithinSectorDimension(SectorVector b, float width, float depth)
		{
			SectorVector DistVec = SectorVector::Subtract(*this, b);
			float XDist = (((float)DistVec.SectorX) * SectorSize) + DistVec.X;
			float ZDist = (((float)DistVec.SectorZ) * SectorSize) + DistVec.Z;

			// If negative, then we aren't inside
			if (XDist < 0.0f || ZDist < 0.0f)
				return false;

			// If the distance from square origin is great than square dimensions, then we aren't inside
			if (XDist > width || ZDist > depth)
				return false;

			return true;
		}

#pragma endregion
	};
}

#pragma warning(pop)
