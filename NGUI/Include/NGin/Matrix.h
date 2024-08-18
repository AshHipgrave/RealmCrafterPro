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
#include "math.h"

namespace NGin
{
	namespace Math
	{
		struct Quaternion;

		//! 4x4 Matrix
		/*!
		<b>Additional Notes:</b> The M[...] array stores the matrix contents, these can also be
		accessed with _11 to _44 members.
		*/
		class Matrix
		{
		public:

			union
			{
				struct
				{
					float _11, _12, _13, _14;
					float _21, _22, _23, _24;
					float _31, _32, _33, _34;
					float _41, _42, _43, _44;
				};

				//! Float Array
				float M[16];
			};
			

			// Constructor and destructor
			Matrix();
			~Matrix() { }

			//! Make the matrix identity
			void MakeIdentity();

			//! Get Translation
			Vector3 Translation();

			//! Set Translations
			void Translation(Vector3& Trans);

			//! Get Scale
			Vector3 Scale();

			//! Set Scale
			void Scale(Vector3& Scal);

			//! Get Rotation in Radians
			Vector3 RotationRad();

			//! Set Rotation in Radians
			void RotationRad(Vector3& Rot);

			//! Get Rotation in Degrees
			Vector3 RotationDeg();

			//! Set Rotation in Degrees
			void RotationDeg(Vector3& Rot);

			//! Transform Vector
			void TransformVector(Vector3* Vector);

			
			//! Invert a matrix
			void Invert();


			//! Create a Projection Matrix
			/*!
			Creates a projection matrix for a camera.
			\param FOV Camera Field of View
			\param AspectRatio The aspect ratio of the screen
			\param Near The near clipping range of the camera
			\param Far The far clipping range of the camera
			*/
			void CreatePerspectiveFOV(float FOV, float AspectRatio, float Near, float Far);

			//! Create a View Matrix
			/*!
			Creates a view matrix for a camera.
			\param Position The camera position
			\param Target The camera target
			\param Up The up vector of the camera
			*/
			void CreateLookAt(Vector3& Position, Vector3& Target, Vector3& Up);

			//! Mathematical function (for use with operators)
			static inline Matrix Multiply(Matrix& Mat1, Matrix& Mat2);

			//! Set
			Matrix& operator =(Matrix &Other);

			//! Is Equal
			bool operator ==(Matrix &Other);

			//! Is not Equal
			bool operator !=(Matrix &Other);

			//! Multiply
			Matrix operator *(Matrix &Other);

			//! Multiply
			Matrix& operator *=(Matrix &Other);

			//! Get index
			float& operator [](int Index);

			//! Get Row/Collumn
			float& operator()(int Row, int Collumn);

			//! Convert this matrix to a quaternion
			Quaternion Rotation();

			//! Obtain information in string form
			std::string ToString();

			static Matrix CreateTranslation(Vector3 &Position);
			static Matrix CreateScale(Vector3 &Scale);
		};

	
	}// Math
}//NGin
