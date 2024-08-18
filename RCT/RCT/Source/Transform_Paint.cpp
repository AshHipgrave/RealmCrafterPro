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
#include <ITerrain.h>

namespace RealmCrafter
{
	namespace RCT
	{

		void SlideAlpha(int Stage, T1TexCol &Col, float Change)
		{
			// Bring into proper transform space
			Change *= 255.0f;

			int S0 = (int)Col.S0;
			int S1 = (int)Col.S1;
			int S2 = (int)Col.S2;
			int S3 = (int)Col.S3;
			int S4 = 255 - (S0 + S1 + S2 + S3);
			if(S4 < 0)
				S4 = 0;

			float fStageValues[5] = { (float)S0, (float)S1, (float)S2, (float)S3, (float)S4 };

			if(Change > 255.0f)
				Change = 255.0f;

			float StageWeights[5] = { 0, 0, 0, 0, 0};	
			float Max = 0.0f;

			for(int i = 0; i < 5; ++i)
			{
				if(Stage != i)
				{
					StageWeights[i] = (float)fStageValues[i];
					if(StageWeights[i] > Max)
						Max = StageWeights[i];
				}
			}

			// Change was already achieved
			if(Max == 0.0f)
				return;

			// Get 0-1 range
			for(int i = 0; i < 5; ++i)
				StageWeights[i] /= Max;

			// Get Min
			float Min = 1.0f;
			for(int i = 0; i < 5; ++i)
				if(StageWeights[i] < Min && StageWeights[i] > 0.0f)
					Min = StageWeights[i];

			// Get reciprocal
			Min = 1.0f / Min;

			// Multiply into ratio
			for(int i = 0; i < 5; ++i)
				StageWeights[i] *= Min;

			float Total = 0.0f;
			for(int i = 0; i < 5; ++i)
				Total += StageWeights[i];

			float One = Change / Total;
			float Remains = 0.0f;

			for(int i = 0; i < 5; ++i)
			{
				if(i == Stage)
					continue;

				if(fStageValues[i] - (One * StageWeights[i]) < 0.0f)
				{
					Remains += (One * StageWeights[i]) - fStageValues[i];
					fStageValues[i] = 0.0f;
				}else
				{
					fStageValues[i] -= (One * StageWeights[i]);
				}
			}

			fStageValues[Stage] += Change;
			if(fStageValues[Stage] > 255.0f)
				fStageValues[Stage] = 255.0f;

			S0 = (int)(fStageValues[0] + 0.5f);
			S1 = (int)(fStageValues[1] + 0.5f);
			S2 = (int)(fStageValues[2] + 0.5f);
			S3 = (int)(fStageValues[3] + 0.5f);
			S4 = (int)(fStageValues[4] + 0.5f);

			Col.S0 = (unsigned int)S0;
			Col.S1 = (unsigned int)S1;
			Col.S2 = (unsigned int)S2;
			Col.S3 = (unsigned int)S3;
		}




		void Transform_Paint(ITerrain* Terrain, float Radius, float Hardness, NGin::Math::Vector2 Position, bool Circular, float Strength, int Texture, float Min, float Max, float MinHeight, float MaxHeight)
		{
			Position -= NGin::Math::Vector2(Terrain->GetPosition().X, Terrain->GetPosition().Z);

			if(Circular)
			{
				// Copy position
				float cx = Position.X;
				float cy = Position.Y;
				int LoopRad = (int)Radius;
				D3DXVECTOR3 Down(0, 1, 0);

				// Square radius and get 'inner' radius
				Radius *= Radius;
				float IRad = Radius * Hardness;

				for(int ty = ((int)cy) - LoopRad - 1; ty < ((int)cy) + LoopRad + 1; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad - 1; tx < ((int)cx) + LoopRad + 1; ++tx)
					{
						// Get floating vertex positions
						float fx = (float)tx;
						float fy = (float)ty;

						// Get distance from brush center
						float Dist = pow(cx - fx, 2) + pow(cy - fy, 2);

						float Height = Terrain->GetHeight(tx, ty);

						// It's inside the circle
						if(Dist < Radius && Height >= MinHeight && Height <= MaxHeight)
						{
							// Determine its normal
							D3DXVECTOR3 N(fx, Terrain->GetHeight(tx, ty + 1), fy + 1);
							D3DXVECTOR3 S(fx, Terrain->GetHeight(tx, ty - 1), fy - 1);
							D3DXVECTOR3 E(fx + 1, Terrain->GetHeight(tx + 1, ty), fy);
							D3DXVECTOR3 W(fx - 1, Terrain->GetHeight(tx - 1, ty), fy);
							D3DXVECTOR3 NE(fx + 1, Terrain->GetHeight(tx + 1, ty + 1), fy + 1);
							D3DXVECTOR3 SW(fx - 1, Terrain->GetHeight(tx - 1, ty - 1), fy - 1);

							D3DXVECTOR3 X(fx, Terrain->GetHeight(tx, ty), fy);

							D3DXPLANE T0, T1, T2, T3;
							D3DXPlaneFromPoints(&T0, &SW, &W, &X);
							D3DXPlaneFromPoints(&T1, &X, &S, &SW);
							D3DXPlaneFromPoints(&T2, &X, &N, &NE);
							D3DXPlaneFromPoints(&T3, &NE, &E, &X);

							D3DXVECTOR3 nT0(T0.a, T0.b, T0.c);
							D3DXVECTOR3 nT1(T1.a, T1.b, T1.c);
							D3DXVECTOR3 nT2(T2.a, T2.b, T2.c);
							D3DXVECTOR3 nT3(T3.a, T3.b, T3.c);

							D3DXVECTOR3 nX = nT0 + nT1 + nT2 + nT3;
							D3DXVec3Normalize(&nX, &nX);

							//float Min = 0;//.03f;//3333f; // 30^
							//float Max = 1;//0.666f;//6666f; // 60^

							//float Dot = 1.0f - D3DXVec3Dot(&nX, &Down);
							float Yv = ((D3DX_PI * 0.5f) - acos(fabs(nX.y))) / (D3DX_PI * 0.5f);

							//if(Yv <= Min || Yv >= Max)
							if(Yv <= Min && Yv >= Max)
							{
								T1TexCol Col = Terrain->GetColorChunk(NGin::Math::Vector2(fx, fy), 0, 0);

								// Its in the outer circle
								if(Dist > IRad)
								{
									// Determine interpolation
									float Interp = (Dist - IRad) / (Radius - IRad);

									float tH = Strength;
									float mH = 0.0f;		

									float S = ((mH - tH) * Interp) + tH;
									SlideAlpha(Texture, Col, S);

								}else // Inside, set height
								{
									SlideAlpha(Texture, Col, Strength);
								}
								
								Terrain->SetColorChunk(NGin::Math::Vector2(fx, fy), 0, 0, Col);
							}
						}
					}
				}
			}
			else
			{
				// Copy position
				float cx = Position.X;
				float cy = Position.Y;
				float IRad = 0;
				int LoopRad = (int)Radius;

				// Get Inner Radius
				IRad = Radius * Hardness;

				int IMinX = cx - IRad;
				int IMaxX = cx + IRad;
				int IMinY = cy - IRad;
				int IMaxY = cy + IRad;

				float fIMinX = (float)IMinX;
				float fIMaxX = (float)IMaxX;
				float fIMinY = (float)IMinY;
				float fIMaxY = (float)IMaxY;

				float fMinX = cx - LoopRad;
				float fMaxX = cx + LoopRad;
				float fMinY = cy - LoopRad;
				float fMaxY = cy + LoopRad;

				float Change = Strength;

				for(int ty = ((int)cy) - LoopRad - 1; ty < ((int)cy) + LoopRad + 1; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad - 1; tx < ((int)cx) + LoopRad + 1; ++tx)
					{
						// Get floating vertex positions
						float fx = (float)tx;
						float fy = (float)ty;

						// Default change (Used from Raise/Lower Interp)
						float H = 0.0f;

						float Height = Terrain->GetHeight(tx, ty);
						if(Height < MinHeight || Height > MaxHeight)
							continue;

						if(tx >= IMinX && tx <= IMaxX && ty >= IMinX && ty <= IMaxY)
						{
							H = Change;
							goto Transform_PaintLabel;
						}

						/*
						_______________
						|\           /|
						| \    3    / |
						|  \_______/  |
						|   |     |   |
						| 1 |     | 2 |
						|   |_____|   |
						|  /       \  |
						| /    4    \ |
						|/___________\|
						*/

						// 1
						if(tx < IMinX && ty >= IMinY && ty <= IMaxY)
						{
							H = (-Change * ((fx - fIMinX) / (fMinX - fIMinX))) + Change;
							goto Transform_PaintLabel;
						}

						// 2
						if(tx > IMaxX && ty >= IMinY && ty <= IMaxY)
						{
							H = (-Change * ((fx - fIMaxX) / (fMaxX - fIMaxX))) + Change;
							goto Transform_PaintLabel;
						}

						// 3
						if(ty > IMaxY && tx >= IMinX && tx <= IMaxX)
						{
							H = (-Change * ((fy - fIMaxY) / (fMaxY - fIMaxY))) + Change;
							goto Transform_PaintLabel;
						}

						// 4
						if(ty < IMinY && tx >= IMinX && tx <= IMaxX)
						{
							H = (-Change * ((fy - fIMinY) / (fMinY - fIMinY))) + Change;
							goto Transform_PaintLabel;
						}

						// 1/3
						if(tx < IMinX && ty > IMaxY)
						{
							if((tx - IMinX) < -(ty - IMaxY))
								H = (-Change * ((fx - fIMinX) / (fMinX - fIMinX))) + Change;
							else
								H = (-Change * ((fy - fIMaxY) / (fMaxY - fIMaxY))) + Change;
							goto Transform_PaintLabel;
						}

						// 2/3
						if(tx > IMaxX && ty > IMaxY)
						{
							if((tx - IMaxX) > (ty - IMaxY))
								H = (-Change * ((fx - fIMaxX) / (fMaxX - fIMaxX))) + Change;
							else
								H = (-Change * ((fy - fIMaxY) / (fMaxY - fIMaxY))) + Change;
							goto Transform_PaintLabel;
						}

						// 1/4
						if(tx < IMinX && ty < IMinY)
						{
							if((tx - IMinX) < (ty - IMinY))
								H = (-Change * ((fx - fIMinX) / (fMinX - fIMinX))) + Change;
							else
								H = (-Change * ((fy - fIMinY) / (fMinY - fIMinY))) + Change;
							goto Transform_PaintLabel;
						}

						// 2/4
						if(tx > IMaxX && ty < IMinY)
						{
							if(-(tx - IMaxX) < (ty - IMinY))
								H = (-Change * ((fx - fIMaxX) / (fMaxX - fIMaxX))) + Change;
							else
								H = (-Change * ((fy - fIMinY) / (fMinY - fIMinY))) + Change;
							goto Transform_PaintLabel;
						}

						Transform_PaintLabel:

						// Determine its normal
						D3DXVECTOR3 N(fx, Terrain->GetHeight(tx, ty + 1), fy + 1);
						D3DXVECTOR3 S(fx, Terrain->GetHeight(tx, ty - 1), fy - 1);
						D3DXVECTOR3 E(fx + 1, Terrain->GetHeight(tx + 1, ty), fy);
						D3DXVECTOR3 W(fx - 1, Terrain->GetHeight(tx - 1, ty), fy);
						D3DXVECTOR3 NE(fx + 1, Terrain->GetHeight(tx + 1, ty + 1), fy + 1);
						D3DXVECTOR3 SW(fx - 1, Terrain->GetHeight(tx - 1, ty - 1), fy - 1);

						D3DXVECTOR3 X(fx, Terrain->GetHeight(tx, ty), fy);

						D3DXPLANE T0, T1, T2, T3;
						D3DXPlaneFromPoints(&T0, &SW, &W, &X);
						D3DXPlaneFromPoints(&T1, &X, &S, &SW);
						D3DXPlaneFromPoints(&T2, &X, &N, &NE);
						D3DXPlaneFromPoints(&T3, &NE, &E, &X);

						D3DXVECTOR3 nT0(T0.a, T0.b, T0.c);
						D3DXVECTOR3 nT1(T1.a, T1.b, T1.c);
						D3DXVECTOR3 nT2(T2.a, T2.b, T2.c);
						D3DXVECTOR3 nT3(T3.a, T3.b, T3.c);

						D3DXVECTOR3 nX = nT0 + nT1 + nT2 + nT3;
						D3DXVec3Normalize(&nX, &nX);

						//float Min = 0;//.03f;//3333f; // 30^
						//float Max = 1;//0.666f;//6666f; // 60^

						//float Dot = 1.0f - D3DXVec3Dot(&nX, &Down);
						float Yv = ((D3DX_PI * 0.5f) - acos(fabs(nX.y))) / (D3DX_PI * 0.5f);

						//if(Yv >= Min && Yv <= Max)
						if(Yv <= Min && Yv >= Max)
						{
								
							// Remove the +1 -1 interp anomaliesn
							if(Change > 0.0f && H < 0.0f)
								H = 0.0f;

							T1TexCol Col = Terrain->GetColorChunk(NGin::Math::Vector2(fx, fy), 0, 0);

							SlideAlpha(Texture, Col, H);
							
							Terrain->SetColorChunk(NGin::Math::Vector2(fx, fy), 0, 0, Col);
						}
					}
				}
			}
		}
	}
}



