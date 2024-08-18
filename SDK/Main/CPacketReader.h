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

#include <vector2.h>
#include <vector3.h>
#include <vector4.h>
#include <Quaternion.h>
#include <Color.h>
#include <Matrix.h>
#include <string>

namespace RealmCrafter
{
	class CPacketReader
	{
		char* Data;
		unsigned int Length;
		unsigned int Location;
		bool Allocated;

	public:

		//! Construct using predefined data and length.
		/*!
		\param copy If set to true the data will be copied to new memory so existing data can be freed.
		*/
		CPacketReader(char* data, unsigned int length, bool copy)
			: Location(0)
		{
			Length = length;
			if(!copy)
			{
				Data = data;
				Allocated = false;
			}else
			{
				Data = new char[length];
				memcpy(Data, data, length);
				Allocated = true;
			}
		}

		//! Copy constructor
		CPacketReader(CPacketReader& other)
		{
			Length = other.Length;
			Location = other.Location;

			Data = new char[other.Length];
			Allocated = true;
			memcpy(Data, other.Data, Length);
		}

		//! Destructor
		~CPacketReader()
		{
			if(Allocated && Data != 0)
				delete[] Data;
			Data = 0;
			Length = 0;
			Location = 0;
		}

		//! Returns the length of the packet
		unsigned int GetLength() const
		{
			return Length;
		}

		//! Returns the current read position
		unsigned int GetLocation() const
		{
			return Location;
		}

		//! Read an array of raw data
		/*!
		\param ptr Memory pointer to read into.
		\param size Size in bytes of data to read.
		*/
		void Read(const void* ptr, unsigned int size)
		{
			if(ptr != 0 && Data != 0)
				memcpy(const_cast<void*>(ptr), Data + Location, size);
			Location += size;
		}

		//! Read a string with a pre-determined length
		std::string ReadString(int readLength)
		{
			unsigned int Len = readLength;
			if(Len == 0)
				return std::string("");

			char* Temp = new char[Len + 1];
			Temp[Len] = 0;

			Read(Temp, Len);

			std::string Out = Temp;
			delete[] Temp;

			return Out;
		}

		//! Read a string
		/*!
		Note: A 32-bit integer will read first to determine the length of the string.
		*/
		std::string ReadString()
		{
			unsigned int Len = ReadUInt32();
			if(Len == 0)
				return std::string("");

			char* Temp = new char[Len + 1];
			Temp[Len] = 0;

			Read(Temp, Len);

			std::string Out = Temp;
			delete[] Temp;

			return Out;
		}

		//! Read a an 8-bit integer
		unsigned char ReadByte()
		{
			unsigned char Out;
			Read(&Out, sizeof(unsigned char));
			return Out;
		}

		//! Read an unsigned short integer
		unsigned short ReadUInt16()
		{
			unsigned short Out;
			Read(&Out, sizeof(unsigned short));
			return Out;
		}

		//! Read a signed 32-bit integer
		int ReadInt32()
		{
			int Out;
			Read(&Out, sizeof(int));
			return Out;
		}

		//! Read an unsigned 32-bit integer
		unsigned int ReadUInt32()
		{
			unsigned int Out;
			Read(&Out, sizeof(unsigned int));
			return Out;
		}
		
		//! Read an signed 64-bit integer
		long long int ReadInt64()
		{
			long long int Out;
			Read(&Out, sizeof(long long int));
			return Out;
		}

		//! Read an unsigned 64-bit integer
		unsigned long long int ReadUInt64()
		{
			unsigned long long int Out;
			Read(&Out, sizeof(unsigned long long int));
			return Out;
		}

		//! Read an 32-bit floating point value
		float ReadSingle()
		{
			float Out;
			Read(&Out, sizeof(float));
			return Out;
		}

		//! Read a 64-bit floating point double
		double ReadDouble()
		{
			double Out;
			Read(&Out, sizeof(double));
			return Out;
		}

		//! Read a 2D Vector
		NGin::Math::Vector2 ReadVector2()
		{
			NGin::Math::Vector2 Out;
			Read(&Out, sizeof(NGin::Math::Vector2));
			return Out;
		}

		//! Read a 3D Vector
		NGin::Math::Vector3 ReadVector3()
		{
			NGin::Math::Vector3 Out;
			Read(&Out, sizeof(NGin::Math::Vector3));
			return Out;
		}

		//! Read a 4D Vector
		NGin::Math::Vector4 ReadVector4()
		{
			NGin::Math::Vector4 Out;
			Read(&Out, sizeof(NGin::Math::Vector4));
			return Out;
		}

		//! Read a Color value
		NGin::Math::Color ReadColor()
		{
			unsigned int TempColor = 0;
			Read(&TempColor, sizeof(unsigned int));

			int A = ((TempColor & 0xff000000) >> 24);
			int R = ((TempColor & 0xff0000) >> 16);
			int G = ((TempColor & 0xff00) >> 8);
			int B = (TempColor & 0xff);

			NGin::Math::Color Out(((float)R) / 255.0f, ((float)G) / 255.0f, ((float)B) / 255.0f, ((float)A) / 255.0f);

			return Out;
		}

		//! Read a 4x4 Matrix
		NGin::Math::Matrix ReadMatrix()
		{
			NGin::Math::Matrix Out;
			Read(&Out, sizeof(NGin::Math::Matrix));
			return Out;
		}
	};

	// Redefine the basic packetreader, the C prefix isn't always desirable.
	typedef CPacketReader PacketReader;
}