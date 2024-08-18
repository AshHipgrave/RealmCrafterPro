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

		void Transform_Raise(ITerrain* Terrain, float Radius, float Hardness, NGin::Math::Vector2 Position, bool Circular, float Change)
		{
			Position -= NGin::Math::Vector2(Terrain->GetPosition().X, Terrain->GetPosition().Z);

			if(Circular)
			{
				// Copy position
				float cx = Position.X;
				float cy = Position.Y;
				float IRad = 0;
				int LoopRad = (int)Radius;

				// Square radius and get 'inner' radius
				Radius *= Radius;
				IRad = Radius * Hardness;

				for(int ty = ((int)cy) - LoopRad - 1; ty < ((int)cy) + LoopRad + 1; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad - 1; tx < ((int)cx) + LoopRad + 1; ++tx)
					{
						// Get floating vertex positions
						float fx = (float)tx;
						float fy = (float)ty;

						// Get distance from brush center
						float Dist = pow(cx - fx, 2) + pow(cy - fy, 2);
										
						// Default height
						float H = 0.0f;

						// It's inside the circle
						if(Dist < Radius)
						{
							// Its in the outer circle
							if(Dist > IRad)
							{
								// Determine interpolation
								float Interp = (Dist - IRad) / (Radius - IRad);

								float tH = Change;
								float mH = 0.0f;		

								H = ((mH - tH) * Interp) + tH;

							}else // Inside, set height
							{
								H = Change;
							}

							// Commit
							Terrain->SetHeight(tx, ty, Terrain->GetHeight(tx, ty) + H);
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

				for(int ty = ((int)cy) - LoopRad - 1; ty < ((int)cy) + LoopRad + 1; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad - 1; tx < ((int)cx) + LoopRad + 1; ++tx)
					{
						// Get floating vertex positions
						float fx = (float)tx;
						float fy = (float)ty;

						// Default height
						float H = 0.0f;

						if(tx >= IMinX && tx <= IMaxX && ty >= IMinY && ty <= IMaxY)
						{
							H = Change;
							goto Transform_RaiseSetHeightLabel;
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
							goto Transform_RaiseSetHeightLabel;
						}

						// 2
						if(tx > IMaxX && ty >= IMinY && ty <= IMaxY)
						{
							H = (-Change * ((fx - fIMaxX) / (fMaxX - fIMaxX))) + Change;
							goto Transform_RaiseSetHeightLabel;
						}

						// 3
						if(ty > IMaxY && tx >= IMinX && tx <= IMaxX)
						{
							H = (-Change * ((fy - fIMaxY) / (fMaxY - fIMaxY))) + Change;
							goto Transform_RaiseSetHeightLabel;
						}

						// 4
						if(ty < IMinY && tx >= IMinX && tx <= IMaxX)
						{
							H = (-Change * ((fy - fIMinY) / (fMinY - fIMinY))) + Change;
							goto Transform_RaiseSetHeightLabel;
						}

						// 1/3
						if(tx < IMinX && ty > IMaxY)
						{
							if((tx - IMinX) < -(ty - IMaxY))
								H = (-Change * ((fx - fIMinX) / (fMinX - fIMinX))) + Change;
							else
								H = (-Change * ((fy - fIMaxY) / (fMaxY - fIMaxY))) + Change;
							goto Transform_RaiseSetHeightLabel;
						}

						// 2/3
						if(tx > IMaxX && ty > IMaxY)
						{
							if((tx - IMaxX) > (ty - IMaxY))
								H = (-Change * ((fx - fIMaxX) / (fMaxX - fIMaxX))) + Change;
							else
								H = (-Change * ((fy - fIMaxY) / (fMaxY - fIMaxY))) + Change;
							goto Transform_RaiseSetHeightLabel;
						}

						// 1/4
						if(tx < IMinX && ty < IMinY)
						{
							if((tx - IMinX) < (ty - IMinY))
								H = (-Change * ((fx - fIMinX) / (fMinX - fIMinX))) + Change;
							else
								H = (-Change * ((fy - fIMinY) / (fMinY - fIMinY))) + Change;
							goto Transform_RaiseSetHeightLabel;
						}

						// 2/4
						if(tx > IMaxX && ty < IMinY)
						{
							if(-(tx - IMaxX) < (ty - IMinY))
								H = (-Change * ((fx - fIMaxX) / (fMaxX - fIMaxX))) + Change;
							else
								H = (-Change * ((fy - fIMinY) / (fMinY - fIMinY))) + Change;
							goto Transform_RaiseSetHeightLabel;
						}

						
						//if(H == 0.0f)
						//	H = Change;

						Transform_RaiseSetHeightLabel:

						// Remove the +1 -1 interp anomaliesn
						if(Change > 0.0f && H < 0.0f)
							H = 0.0f;
						else if(Change < 0.0f && H > 0.0f)
							H = 0.0f;

						Terrain->SetHeight(tx, ty, Terrain->GetHeight(tx, ty) + H);
					}
				}
			}
		}
	}
}
