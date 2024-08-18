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

extern ID3DXFont* Font;
extern int Width, Height;
#define ToRadians(x) x * (D3DX_PI/180.0f)


// Custom normalization implementation
inline void MyD3DXVectorNormalize(D3DXVECTOR3* Vector)
{
	float Magnitude = (float)sqrt(Vector->x * Vector->x + Vector->y * Vector->y + Vector->z * Vector->z);

	Vector->x /= Magnitude;
	Vector->y /= Magnitude;
	Vector->z /= Magnitude;
}

// Custom cross implementation
inline void MyD3DXVectorCross(D3DXVECTOR3* Out, D3DXVECTOR3* Sub, D3DXVECTOR3* Other)
{
	Out->x = Sub->y * Other->z - Sub->z * Other->y;
	Out->y = Sub->z * Other->x - Sub->x * Other->z;
	Out->z = Sub->x * Other->y - Sub->y * Other->x;
}

inline bool KeyDown(int Key)
{
	short K = GetKeyState(Key);
	return ((K >> 16) & 0xffff) != 0;
}

// Draw text to a certain position
inline void DrawDXText(const char* Text, float X, float Y, D3DCOLOR Color)
{
	// Create drawing rectangle
	RECT DrawRect;
	DrawRect.left = X * Width;
	DrawRect.top = Y * Height;
	DrawRect.right = Width;
	DrawRect.bottom = Height;

	// Draw
	Font->DrawText(NULL, Text, -1, &DrawRect, 0, Color);
}
