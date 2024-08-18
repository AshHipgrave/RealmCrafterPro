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
#include <algorithm>
#include <sstream>
#include "Globals.h"

namespace NGin
{
	namespace Math
	{

		//! Color
		/*!
		Colors by default are assumed as RGBA floating point types. This is because
		the engine only handles custom color with shaders, which assume the format
		of floating RGBA.

		<b>WARNING:</b> This class is 128-bits in size. It will cause problems if used incorrectly.
		Use Color::ToARGB() to obtain a 32-bit color.
		*/
		struct Color
		{
#pragma region Static Methods
			
			static Color FromString(std::string colorString)
			{
				if(colorString.length() < 6)
					return Color(0, 0, 0);

				std::transform(colorString.begin(), colorString.end(), colorString.begin(), ::tolower);

				const char* Str = (const char*)colorString.c_str();

				int R = 0, G = 0, B = 0;
				int CArray[6];

				for(int i = 0; i < 6; ++i)
				{
					CArray[i] = 0;

					if(Str[i] > 47 && Str[i] < 58)
						CArray[i] = Str[i] - 48;
					if(Str[i] > 96 && Str[i] < 103)
						CArray[i] = Str[i] - 87;
				}

				R = (CArray[0] * 16) + CArray[1];
				G = (CArray[2] * 16) + CArray[3];
				B = (CArray[4] * 16) + CArray[5];

				return Color(((float)R) / 255.0f, ((float)G) / 255.0f, ((float)B) / 255.0f);
			}

#pragma endregion
#pragma region Member Variables
			//! Red component
			float R;

			//! Green component
			float G;

			//! Blue component
			float B;

			//! Alpha component
			float A;
#pragma endregion
#pragma region Constructors

			Color(float StartR, float StartG, float StartB) : R(StartR), G(StartG), B(StartB), A(1.0f) {};
			Color(float StartR, float StartG, float StartB, float StartA) : R(StartR), G(StartG), B(StartB), A(StartA) {};
			Color() : R(0.0f), G(0.0f), B(0.0f), A(1.0f) {};
			Color(int StartR, int StartG, int StartB) { R = ((float)StartR) / 255.0f; G = ((float)StartG) / 255.0f; B = ((float)StartB) / 255.0f; A = 1.0f; }
			Color(int StartR, int StartG, int StartB, float StartA) { R = ((float)StartR) / 255.0f; G = ((float)StartG) / 255.0f; B = ((float)StartB) / 255.0f; A = StartA; }
#pragma endregion
#pragma region Methods


			//! Empty the vector
			void Reset()
			{
				R = 0.0f;
				G = 0.0f;
				B = 0.0f;
				A = 1.0f;
			}

			//! Re-initialize vector
			void Reset(float r, float g, float b, float a)
			{
				R = r;
				G = g;
				B = b;
				A = a;
			}

			//! Obtain information in string form
			std::string ToString()
			{
				char Str[128];
				sprintf_s(Str, "%f, %f, %f, %f", R, G, B, A);

				return Str;
			}

			//! Obtain the 32-bit representation of this color
			unsigned int ToARGB()
			{
				int a, r, g, b;
				a = (int)(A * 255.0f);
				r = (int)(R * 255.0f);
				g = (int)(G * 255.0f);
				b = (int)(B * 255.0f);

				return ((((a) & 0xff) << 24) | (((r) & 0xff) << 16) | (((g) & 0xff) << 8) | ((b) & 0xff));
			}

#pragma endregion
#pragma region Operators

			//! Set the vector to a new value
			Color& operator =(const Color &Other)
			{
				R = Other.R;
				G = Other.G;
				B = Other.B;
				A = Other.A;
				return *this;
			}

#pragma endregion
		};

	}
}
