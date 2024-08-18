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

#include "Matrix.h"
#include "Quaternion.h"

namespace NGin
{
	// Conversion Constants
	double PI64 = 3.1415926535897932384626433832795028841971693993751;
	double DEGTORAD64 = PI64 / 180.0f;
	double RADTODEG64 = 180.0f / PI64;

	float PI = 3.14159f;
	float DEGTORAD = PI / 180.0f;
	float RADTODEG = 180.0f / PI;
}

namespace NGin
{
	namespace Math
	{

		float& Matrix::operator [](int Index)
		{
			return this->M[Index];
		}

		float& Matrix::operator()(int Row, int Collumn)
		{
			return this->M[Row * 4 + Collumn];
		}

		Quaternion Matrix::Rotation()
		{
			Quaternion Result;

			float diag = _11 + _22 + _33 + 1;
			float scale = 0.0f;

			if( diag > 0.0f )
			{
				scale = sqrtf(diag) * 2.0f; // get scale from diagonal

				// TODO: speed this up
				Result.X = ( _32 - _23) / scale;
				Result.Y = ( _13 - _31) / scale;
				Result.Z = ( _21 - _12) / scale;
				Result.W = 0.25f * scale;
			}
			else
			{
				if ( _11 > _22 && _11 > _33)
				{
					// 1st element of diag is greatest value
					// find scale according to 1st element and double it
					scale = sqrtf( 1.0f + _11 - _22 - _33) * 2.0f;

					// TODO: speed this up
					Result.X = 0.25f * scale;
					Result.Y = (_12 + _21) / scale;
					Result.Z = (_31 + _13) / scale;
					Result.W = (_32 - _23) / scale;
				}
				else if ( _22 > _33)
				{
					// 2nd element of diag is greatest value
					// find scale according to 2nd element and double it
					scale = sqrtf( 1.0f + _22 - _11 - _33) * 2.0f;

					// TODO: speed this up
					Result.X = (_12 + _21 ) / scale;
					Result.Y = 0.25f * scale;
					Result.Z = (_23 + _32 ) / scale;
					Result.W = (_13 - _31 ) / scale;
				}
				else
				{
					// 3rd element of diag is greatest value
					// find scale according to 3rd element and double it
					scale  = sqrtf( 1.0f + _33 - _11 - _22) * 2.0f;

					// TODO: speed this up
					Result.X = (_13 + _31) / scale;
					Result.Y = (_23 + _32) / scale;
					Result.Z = 0.25f * scale;
					Result.W = (_21 - _12) / scale;
				}
			}

			return Result;

	/*		Quaternion Result;
			float Scale = _11 + _22 + _33;

			if(Scale > 0.0f)
			{
				float Sqrt = sqrt(Scale + 1.0f);

				Result.W = Sqrt * 0.5f;
				Sqrt = 0.5f / Sqrt;

				Result.X = (_23 - _32) * Sqrt;
				Result.Y = (_31 - _13) * Sqrt;
				Result.Z = (_12 - _21) * Sqrt;

				Sqrt = sqrt(4.0f);

				return Result;
			}

			if((_11 >= _22) && (_11 >= _33))
			{
				float Sqrt = sqrt(1.0f + _11 - _22 - _33);
				float Half = 0.5f / Sqrt;

				Result.X = 0.5f * Sqrt;
				Result.Y = (_12 + _21) * Half;
				Result.Z = (_13 + _31) * Half;
				Result.W = (_23 + _32) * Half;

				return Result;
			}

			if(_22 > _33)
			{
				float Sqrt = sqrt(1.0f + _22 - _11 - _33);
				float Half = 0.5f / Sqrt;

				Result.X = (_21 + _12) * Half;
				Result.Y = 0.5f * Sqrt;
				Result.Z = (_32 + _23) * Half;
				Result.W = (_31 + _13) * Half;

				return Result;
			}


			float Sqrt = sqrt(1.0f + _33 - _11 - _22);
			float Half = 0.5f / Sqrt;

			Result.X = (_31 + _13) * Half;
			Result.Y = (_32 + _23) * Half;
			Result.Z = 0.5f * Sqrt;
			Result.W = (_12 + _21) * Half;

			return Result;



			Quaternion Out;
			
			if(M[0] + M[5] + M[10] + 1.0f > 0.0f)
			{
				float Scale = sqrt(M[0] + M[5] + M[10] + 1.0f) * 2.0f;
				Out.Set(
					(M[9] - M[6]) / Scale,
					(M[2] - M[8]) / Scale,
					(M[4] - M[1]) / Scale,
					0.25f * Scale);
				Scale = sqrt(4.0f);
			}else
			{
				if(M[0] > M[5] && M[0] > M[10])
				{
					float Scale = sqrt(1.0f + M[0] - M[5] - M[10]) * 2.0f;
					Out.Set(
						0.25f * Scale,
						(M[1] + M[4]) / Scale,
						(M[2] + M[8]) / Scale,
						(M[6] + M[9]) / Scale);
				}else if(M[5] > M[10])
				{
					float Scale = sqrt(1.0f + M[5] - M[0] - M[10]) * 2.0f;
					Out.Set(
						(M[1] + M[4]) / Scale,
						0.25f * Scale,
						(M[6] + M[9]) / Scale,
						(M[2] + M[8]) / Scale);
				}else
				{
					float Scale = sqrt(1.0f + M[10] - M[5] - M[5]) * 2.0f;
					Out.Set(
						(M[2] + M[8]) / Scale,
						(M[6] + M[9]) / Scale,
						0.25f * Scale,
						(M[1] + M[4]) / Scale);
				}
			}

			return Out;*/
		}

		std::string Matrix::ToString()
		{
			char Output[2048];
			sprintf(Output, "%f, %f, %f, %f\n%f, %f, %f, %f\n%f, %f, %f, %f\n%f, %f, %f, %f",
				M[0], M[1], M[2], M[3],
				M[4], M[5], M[6], M[7],
				M[8], M[9], M[10], M[11],
				M[12], M[13], M[14], M[15]);
			return Output;
		}

		 Matrix::Matrix()
		{
			MakeIdentity();
		}

		 void Matrix::MakeIdentity()
		{
			for(int i = 0; i < 16; ++i)
				M[i] = 0;
			M[0] = M[5] = M[10] = M[15] = 1.0f;
		}

		 Vector3 Matrix::Translation()
		{
			return Vector3(M[12], M[13], M[14]);
		}

		 void Matrix::Translation(Vector3& Trans)
		{
			M[12] = Trans.X;
			M[13] = Trans.Y;
			M[14] = Trans.Z;
		}

		 Vector3 Matrix::Scale()
		{
			return Vector3(M[0], M[5], M[10]);
		}

		 void Matrix::Scale(Vector3& Scal)
		{
			M[0]  = Scal.X;
			M[5]  = Scal.Y;
			M[10] = Scal.Z;
		}

		 void Matrix::RotationDeg(Vector3& Rot)
		{
			RotationRad(Rot * DEGTORAD);
		}

		 void Matrix::RotationRad(Vector3& Rot)
		{
			double CR = cos( Rot.X );
			double SR = sin( Rot.X );
			double CP = cos( Rot.Y );
			double SP = sin( Rot.Y );
			double CY = cos( Rot.Z );
			double SY = sin( Rot.Z );

			M[0] = (float)(CP * CY);
			M[1] = (float)(CP * SY);
			M[2] = (float)-SP;

			double SRSP = SR * SP;
			double CRSP = CR * SP;

			M[4] =  (float)(SRSP * CY - CR * SY);
			M[5] =  (float)(SRSP * SY + CR * CY);
			M[6] =  (float)(SR * CP);
			M[8] =  (float)(CRSP * CY + SR * SY);
			M[9] =  (float)(CRSP * SY - SR * CY);
			M[10] = (float)(CR * CP);
		}

		 Vector3 Matrix::RotationRad()
		{
			return RotationDeg() * DEGTORAD;
		}

		 Vector3 Matrix::RotationDeg()
		{
				double Y = -asin(M[2]);
				double C = cos(Y);
				Y *= RADTODEG64;

				double RotX, RotY, X, Z;

				if(fabs(C) > 0.0005f)
				{
					RotX = M[10] / C;
					RotY = M[6] / C;
					X  = atan2(RotY, RotX) * RADTODEG64;
					RotX = M[0] / C;
					RotY = M[1] / C;
					Z = atan2(RotY, RotX) * RADTODEG64;
				}
				else
				{
					X = 0.0f;
					RotX = M[5];
					RotY = -M[4];
					Z = atan2(RotY, RotX) * (float)RADTODEG64;
				}

				if(X < 0.0) X += 360.0;
				if(Y < 0.0) Y += 360.0;
				if(Z < 0.0) Z += 360.0;

				return Vector3((float)X, (float)Y, (float)Z);
		}

		 void Matrix::TransformVector(Vector3 *Vector)
		{
			Vector3 Temp;

			Temp.X = Vector->X * M[0] + Vector->Y * M[4] + Vector->Z * M[8] + M[12];
			Temp.Y = Vector->X * M[1] + Vector->Y * M[5] + Vector->Z * M[9] + M[13];
			Temp.Z = Vector->X * M[2] + Vector->Y * M[6] + Vector->Z * M[10] + M[14];

			Vector->X = Temp.X;
			Vector->Y = Temp.Y;
			Vector->Z = Temp.Z;
		}

		 void Matrix::CreatePerspectiveFOV(float FOV, float AspectRatio, float Near, float Far)
		{
			float H = 1.0f / (float)tan(FOV / 2.0f);
			float W = H / AspectRatio;

			for(int i = 0; i < 16; ++i)
				M[i] = 0.0f;

			M[0]  = W;
			M[5]  = H;
			M[10] = Far / (Far - Near);
			M[11] = 1.0f;
			M[14] = (-Near * Far) / (Far - Near);
		}

		 void Matrix::CreateLookAt(Vector3& Position, Vector3& Target, Vector3& Up)
		{
			Vector3 Z = Target - Position;
			Z.Normalize();
			

			Vector3 X = Up.Cross(Z);
			
			X.Normalize();

			Vector3 Y = Z.Cross(X);

			M[0]  = X.X;
			M[1]  = Y.X;
			M[2]  = Z.X;
			M[3] = 0.0f;

			M[4]  = X.Y;
			M[5]  = Y.Y;
			M[6]  = Z.Y;
			M[7] = 0.0f;

			M[8]  = X.Z;
			M[9]  = Y.Z;
			M[10] = Z.Z;
			M[11] = 0.0f;

			M[12]  = -X.Dot(Position);
			M[13]  = -Y.Dot(Position);
			M[14] = -Z.Dot(Position);
			M[15] = 1.0f;
		}

		// Set this matrix
		 Matrix& Matrix::operator =(Matrix &Other)
		{
			// Copy the data across
			for(int i = 0; i < 16; ++i)
				M[i] = Other.M[i];
			
			// Return
			return *this;
		}

		// Does this matrix equal another?
		 bool Matrix::operator ==(Matrix &Other)
		{
			// Loop and check
			for(int i = 0; i < 16; ++i)
				if(M[i] != Other.M[i])
					return false;

			// All was well
			return true;
		}

		// Does the matrix not equal?
		 bool Matrix::operator !=(Matrix &Other)
		{
			return !(*this == Other);
		}

		// Multiply two matrices
		 Matrix Matrix::Multiply(Matrix& Mat1, Matrix& Mat2)
		{
			// Make the new matrix
			Matrix NewMat;

			// Make life easier
			float* M1 = Mat1.M;
			float* M2 = Mat2.M;
			float* MN = NewMat.M;

			// Multiply!
			MN[0]  = M1[0] * M2[0]  + M1[4] * M2[1]  + M1[8]  * M2[2]  + M1[12] * M2[3];
			MN[1]  = M1[1] * M2[0]  + M1[5] * M2[1]  + M1[9]  * M2[2]  + M1[13] * M2[3];
			MN[2]  = M1[2] * M2[0]  + M1[6] * M2[1]  + M1[10] * M2[2]  + M1[14] * M2[3];
			MN[3]  = M1[3] * M2[0]  + M1[7] * M2[1]  + M1[11] * M2[2]  + M1[15] * M2[3];

			MN[4]  = M1[0] * M2[4]  + M1[4] * M2[5]  + M1[8]  * M2[6]  + M1[12] * M2[7];
			MN[5]  = M1[1] * M2[4]  + M1[5] * M2[5]  + M1[9]  * M2[6]  + M1[13] * M2[7];
			MN[6]  = M1[2] * M2[4]  + M1[6] * M2[5]  + M1[10] * M2[6]  + M1[14] * M2[7];
			MN[7]  = M1[3] * M2[4]  + M1[7] * M2[5]  + M1[11] * M2[6]  + M1[15] * M2[7];

			MN[8]  = M1[0] * M2[8]  + M1[4] * M2[9]  + M1[8]  * M2[10] + M1[12] * M2[11];
			MN[9]  = M1[1] * M2[8]  + M1[5] * M2[9]  + M1[9]  * M2[10] + M1[13] * M2[11];
			MN[10] = M1[2] * M2[8]  + M1[6] * M2[9]  + M1[10] * M2[10] + M1[14] * M2[11];
			MN[11] = M1[3] * M2[8]  + M1[7] * M2[9]  + M1[11] * M2[10] + M1[15] * M2[11];

			MN[12] = M1[0] * M2[12] + M1[4] * M2[13] + M1[8]  * M2[14] + M1[12] * M2[15];
			MN[13] = M1[1] * M2[12] + M1[5] * M2[13] + M1[9]  * M2[14] + M1[13] * M2[15];
			MN[14] = M1[2] * M2[12] + M1[6] * M2[13] + M1[10] * M2[14] + M1[14] * M2[15];
			MN[15] = M1[3] * M2[12] + M1[7] * M2[13] + M1[11] * M2[14] + M1[15] * M2[15];

			// Return
			return NewMat;
		}

		// Two matrices multiply
		 Matrix Matrix::operator *(Matrix &Other)
		{
			return Matrix::Multiply(*this, Other);
		}

		// Multiply this with another
		 Matrix& Matrix::operator *=(Matrix &Other)
		{
			// Multiply
			Matrix Temp = Matrix::Multiply(*this, Other);

			// Copy Over
			for(int i = 0; i < 16; ++i)
				M[i] = Other.M[i];

			// Return
			return *this;
		}

		// Invert a matrix
		 void Matrix::Invert()
		{
			float N[16];
			float* M = this->M;

			float D = (M[0] * M[5]  - M[4]  * M[1]) * (M[10] * M[15] - M[14] * M[11])
				    - (M[0] * M[9]  - M[8]  * M[1]) * (M[6]  * M[15] - M[14] * M[7])
				    + (M[0] * M[13] - M[12] * M[1]) * (M[6]  * M[11] - M[10] * M[7])
				    + (M[4] * M[9]  - M[8]  * M[5]) * (M[2]  * M[15] - M[14] * M[3])
				    - (M[4] * M[13] - M[12] * M[5]) * (M[2]  * M[11] - M[10] * M[3])
				    + (M[8] * M[13] - M[12] * M[9]) * (M[2]  * M[7]  - M[6]  * M[3]);
			
			if(D == 0.0f)
				return;

			D = 1.0f / D;
		
			N[0]  = D * (M[5]  * (M[10] * M[15] - M[14] * M[11]) + M[9]  * (M[14] * M[7]  - M[6]  * M[15]) + M[13] * (M[6]  * M[11] - M[10] * M[7]));
			N[4]  = D * (M[6]  * (M[8]  * M[15] - M[12] * M[11]) + M[10] * (M[12] * M[7]  - M[4]  * M[15]) + M[14] * (M[4]  * M[11] - M[8]  * M[7]));
			N[8]  = D * (M[7]  * (M[8]  * M[13] - M[12] * M[9])  + M[11] * (M[12] * M[5]  - M[4]  * M[13]) + M[15] * (M[4]  * M[9]  - M[8]  * M[5]));
			N[12] = D * (M[4]  * (M[13] * M[10] - M[9]  * M[14]) + M[8]  * (M[5]  * M[14] - M[13] * M[6])  + M[12] * (M[9]  * M[6]  - M[5]  * M[10]));
			N[1]  = D * (M[9]  * (M[2]  * M[15] - M[14] * M[3])  + M[13] * (M[10] * M[3]  - M[2]  * M[11]) + M[1]  * (M[14] * M[11] - M[10] * M[15]));
			N[5]  = D * (M[10] * (M[0]  * M[15] - M[12] * M[3])  + M[14] * (M[8]  * M[3]  - M[0]  * M[11]) + M[2]  * (M[12] * M[11] - M[8]  * M[15]));
			N[9]  = D * (M[11] * (M[0]  * M[13] - M[12] * M[1])  + M[15] * (M[8]  * M[1]  - M[0]  * M[9])  + M[3]  * (M[12] * M[9]  - M[8]  * M[13]));
			N[13] = D * (M[8]  * (M[13] * M[2]  - M[1]  * M[14]) + M[12] * (M[1]  * M[10] - M[9]  * M[2])  + M[0]  * (M[9]  * M[14] - M[13] * M[10]));
			N[2]  = D * (M[13] * (M[2]  * M[7]  - M[6]  * M[3])  + M[1]  * (M[6]  * M[15] - M[14] * M[7])  + M[5]  * (M[14] * M[3]  - M[2]  * M[15]));
			N[6]  = D * (M[14] * (M[0]  * M[7]  - M[4]  * M[3])  + M[2]  * (M[4]  * M[15] - M[12] * M[7])  + M[6]  * (M[12] * M[3]  - M[0]  * M[15]));
			N[10] = D * (M[15] * (M[0]  * M[5]  - M[4]  * M[1])  + M[3]  * (M[4]  * M[13] - M[12] * M[5])  + M[7]  * (M[12] * M[1]  - M[0]  * M[13]));
			N[14] = D * (M[12] * (M[5]  * M[2]  - M[1]  * M[6])  + M[0]  * (M[13] * M[6]  - M[5]  * M[14]) + M[4]  * (M[1]  * M[14] - M[13] * M[2]));
			N[3]  = D * (M[1]  * (M[10] * M[7]  - M[6]  * M[11]) + M[5]  * (M[2]  * M[11] - M[10] * M[3])  + M[9]  * (M[6]  * M[3]  - M[2]  * M[7]));
			N[7]  = D * (M[2]  * (M[8]  * M[7]  - M[4]  * M[11]) + M[6]  * (M[0]  * M[11] - M[8]  * M[3])  + M[10] * (M[4]  * M[3]  - M[0]  * M[7]));
			N[11] = D * (M[3]  * (M[8]  * M[5]  - M[4]  * M[9])  + M[7]  * (M[0]  * M[9]  - M[8]  * M[1])  + M[11] * (M[4]  * M[1]  - M[0]  * M[5]));
			N[15] = D * (M[0]  * (M[5]  * M[10] - M[9]  * M[6])  + M[4]  * (M[9]  * M[2]  - M[1]  * M[10]) + M[8]  * (M[1]  * M[6]  - M[5]  * M[2]));
			
			for(int i = 0; i < 16; ++i)
				this->M[i] = N[i];
		}

		 
		Matrix Matrix::CreateTranslation(Vector3 &Position)
		{
			Matrix New;
			New.Translation(Position);
			return New;
		}

		Matrix Matrix::CreateScale(Vector3 &Scale)
		{
			Matrix New;
			New.Scale(Scale);
			return New;
		}
	
	}// Math
}//NGin
