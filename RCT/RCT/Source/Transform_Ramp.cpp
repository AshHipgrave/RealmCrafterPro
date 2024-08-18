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
		void Transform_Ramp(ITerrain* Terrain, NGin::Math::Vector2 startPosition, NGin::Math::Vector2 endPosition, float startWidth, float endWidth)
		{
			startPosition -= NGin::Math::Vector2(Terrain->GetPosition().X, Terrain->GetPosition().Z);
			endPosition -= NGin::Math::Vector2(Terrain->GetPosition().X, Terrain->GetPosition().Z);

			float StartHeight = Terrain->GetHeight(startPosition.X, startPosition.Y);
			float EndHeight = Terrain->GetHeight(endPosition.X, endPosition.Y);

			NGin::Math::Vector2 Distance(endPosition - startPosition);
			float Length = sqrt((Distance.X * Distance.X) + (Distance.Y * Distance.Y));

			NGin::Math::Vector2 Gradient = Distance / Length;

			float Angle = atan2(Gradient.Y, Gradient.X) + (D3DX_PI * 0.5f);
			NGin::Math::Vector2 AdGradient(cos(Angle), sin(Angle));

			NGin::Math::Vector2 DrawPos = startPosition;

			float Smoothing = 1.0f;

			for(float i = 0.0f; i < Length; i += 1.0f)
			{
				float Height = StartHeight + ((EndHeight - StartHeight) * (i / Length));
				//Terrain->SetHeight(DrawPos.X, DrawPos.Y, Height);

				DrawPos += Gradient;

				float Width = startWidth + ((endWidth - startWidth) * (i / Length));

				NGin::Math::Vector2 TempPos = DrawPos - (AdGradient * (Width * 0.5f));

				for(float f = 0.0f; f < Width; f += 0.1f)
				{
					//float OldHeight = Terrain->GetHeight(TempPos.X, TempPos.Y);

					//float NewHeight = Height + ((OldHeight - Height) * 0.1f);//((fabs(f) / (Width * 0.5f)) * Smoothing));
					//float SmoothHeight = Height * (1.0f - (fabs(f - (Width * 0.5f)) / (Width * 0.5f)));

					Terrain->SetHeight(TempPos.X, TempPos.Y, Height);
					TempPos += (AdGradient * 0.1f);
				}
			}
		}
	}
}



