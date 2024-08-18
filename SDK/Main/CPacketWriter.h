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
	class CPacketWriter
	{
		char* Data;
		unsigned int Allocated;
		unsigned int Length;

		void CheckResize(unsigned int size)
		{
			if(size + Length >= Allocated)
			{
				char* NewData = new char[Allocated + 1024];
				memcpy(NewData, Data, Allocated);
				delete[] Data;
				Data = NewData;
				Allocated += 1024;
			}
		}

	public:

		//! Constructor
		CPacketWriter()
			: Length(0)
		{
			Data = new char[1024];
			Allocated = 1024;
		}

		//! Destructor
		~CPacketWriter()
		{
			if(Data != 0)
				delete[] Data;
			Data = 0;
			Allocated = 0;
			Length = 0;
		}

		//! Clear current data and reset packet
		void Clear()
		{
			Length = 0;
		}

		//! Get the current length of the packet
		unsigned int GetLength() const
		{
			return Length;
		}

		//! Get the full packet as a byte array
		char* GetBytes() const
		{
			return Data;
		}

		//! Write raw data into the packet
		void Write(const void* ptr, unsigned int size)
		{
			CheckResize(size);
			memcpy(Data + Length, ptr, size);
			Length += size;
		}

		//! Write a string
		/*!
		\param writePrefix If true, a 32-bit integer will be written first containing the string length.
		*/
		void Write(std::string &data, bool writePrefix)
		{
			const char* Str = data.c_str();
			int Length = (int)data.size();

			CheckResize(Length);
			if(writePrefix)
				Write(&Length, sizeof(int));
			Write(Str, Length);
		}

		//! Append data from another packetwriter
		void Write(const CPacketWriter &other)
		{
			unsigned int Len = other.GetLength();
			const char* Dat = other.GetBytes();

			Write(Dat, Len);
		}

		void Write(unsigned char value)
		{
			Write(&value, sizeof(unsigned char));
		}

		void Write(unsigned short value)
		{
			Write(&value, sizeof(unsigned short));
		}

		void Write(int value)
		{
			Write(&value, sizeof(int));
		}

		void Write(unsigned int value)
		{
			Write(&value, sizeof(unsigned int));
		}

		void Write(long long int value)
		{
			Write(&value, sizeof(long long int));
		}

		void Write(unsigned long long int value)
		{
			Write(&value, sizeof(unsigned long long int));
		}

		void Write(float value)
		{
			Write(&value, sizeof(float));
		}

		void Write(double &value)
		{
			Write(&value, sizeof(double));
		}

		void Write(NGin::Math::Vector2 &value)
		{
			Write(&value, sizeof(NGin::Math::Vector2));
		}

		void Write(NGin::Math::Vector3 &value)
		{
			Write(&value, sizeof(NGin::Math::Vector3));
		}

		void Write(NGin::Math::Vector4 &value)
		{
			Write(&value, sizeof(NGin::Math::Vector4));
		}

		void Write(NGin::Math::Color &value)
		{
			unsigned int Value = value.ToARGB();
			Write(&Value, sizeof(unsigned int));
		}

		void Write(NGin::Math::Matrix &value)
		{
			Write(&value, sizeof(NGin::Math::Matrix));
		}

		//! Write a raw data pointer to the start of the packetwriter.
		void Prefix(const char* ptr, unsigned int length)
		{
			CheckResize(length);

			char* NewData = new char[Allocated];
			memcpy(NewData + length, Data, Allocated - length);
			memcpy(NewData, ptr, length);

			delete[] Data;
			Data = NewData;

			Length += length;
		}

		// Append data
		CPacketWriter& operator +=(const CPacketWriter &other)
		{
			Write(other);
			return *this;
		}
	};

	//
	typedef CPacketWriter PacketWriter;
}
