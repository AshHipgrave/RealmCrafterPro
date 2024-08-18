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

		void Transform_SetHeight(ITerrain* Terrain, float Radius, float Hardness, NGin::Math::Vector2 Position, bool Circular, float Height)
		{
			Position -= NGin::Math::Vector2(Terrain->GetPosition().X, Terrain->GetPosition().Z);

			if(Circular)
			{
				// Copy position
				float cx = Position.X;
				float cy = Position.Y;
				int LoopRad = (int)Radius;

				// Square radius and get 'inner' radius
				Radius *= Radius;

				for(int ty = ((int)cy) - LoopRad - 1; ty < ((int)cy) + LoopRad + 1; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad - 1; tx < ((int)cx) + LoopRad + 1; ++tx)
					{
						// Get floating vertex positions
						float fx = (float)tx;
						float fy = (float)ty;

						// Get distance from brush center
						float Dist = pow(cx - fx, 2) + pow(cy - fy, 2);

						// It's inside the circle
						if(Dist < Radius)
						{

							// Commit
							Terrain->SetHeight(tx, ty, Height);
						}
					}
				}
			}
			else
			{
				// Copy position
				float cx = Position.X;
				float cy = Position.Y;
				int LoopRad = (int)Radius;

				for(int ty = ((int)cy) - LoopRad; ty < ((int)cy) + LoopRad; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad ; tx < ((int)cx) + LoopRad; ++tx)
					{
						// Get floating vertex positions
						float fx = (float)tx;
						float fy = (float)ty;

						Terrain->SetHeight(tx, ty, Height);
					}
				}
			}
		}
	}
}

