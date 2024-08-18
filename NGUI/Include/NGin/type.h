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
#include "TypeDefs.h"

namespace NGin
{
	#if !defined(NGIN_GUID)
		#define NGIN_GUID(Guid4) *((int*)Guid4)
	#endif

	//! Type
	/*!
	Types store GUIDs and descriptions of classes. Useful for comparing between
	class types.
	*/
	class Type
	{
	private:
		guid4 _Guid;
	
		#if defined(_DEBUG)
			std::string _Name;
		#endif

	public:

		//! Default contructor with default types
		Type()
		{
			_Guid = NGIN_GUID("NTYP");
			
			#if defined(_DEBUG)
				_Name = "NGin Type";
			#endif
		}

		//! Better constructor
		/*!
		\param Guid Four part GUID
		\param RealName Useful description
		*/
		Type(const char* Guid, const char* RealName)
		{
			_Guid = NGIN_GUID(Guid);

			#if defined(_DEBUG)
				_Name = RealName;
			#endif
		}

		~Type() {}

		//! Returns the GUID of the type
		guid4 Guid()
		{
			return _Guid;
		}

		//! Returns the name of the type
		std::string Name()
		{
			#if defined(_DEBUG)
				return _Name;
			#else
				return "";
			#endif
		}

		//! Returns a CString which describes the GUID
		std::string GuidString()
		{
			char* CGUID = (char*)&_Guid;
			char CStr[5];
			CStr[0] = CGUID[0];
			CStr[1] = CGUID[1];
			CStr[2] = CGUID[2];
			CStr[3] = CGUID[3];
			CStr[4] = 0;

			return std::string(CStr);
		}

		//! Compares one type with another
		bool operator==(Type& Other)
		{
			if(_Guid == Other.Guid())
				return true;
			else
				return false;
		}

		//! Not-Compares one type with another
		bool operator!=(Type& Other)
		{
			return !(*this == Other);
		}

		//! Some STL compatibility
		bool operator < (Type &other) const { return _Guid < other._Guid; }
		bool operator < (const Type &other) const { return _Guid < other._Guid; }
		bool operator > (Type &other) const { return _Guid > other._Guid; }
		bool operator > (const Type &other) const { return _Guid > other._Guid; }
	};
}