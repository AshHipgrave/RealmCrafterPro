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
// Post processing class
#pragma once

#include <d3dx9.h>
#include "cRT.h"
#include "irrString.h"

// This just emulates C# get/set
#define Property(Type, Val, Local) Type Val() { return Local; } void Val(Type N) { Local = N; }

extern IDirect3DVertexBuffer9* FullScreenQuad;
#define SAQUADFVF (D3DFVF_XYZ | D3DFVF_NORMAL | D3DFVF_TEX1)
struct SAVert
{
	float x, y, z;
	float nx, ny, nz;
	float u, v;
};


const D3DVERTEXELEMENT9 DEFSAVert[] =
{
	{0, 0,  D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
	{0, 12,  D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL, 0},
	{0, 24, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
	D3DDECL_END()	
};
extern IDirect3DVertexDeclaration9* SAVertDecl;


// Initialisation command
void BuildQuad(IDirect3DDevice9* Device);
void DestroyQuad();

// Post processing effect
class CPPEffect
{
protected:

	// Locals
	ID3DXEffect* _Effect;
	IDirect3DDevice9* _Device;
	irr::core::stringc _Tag;
	bool _IsActive;
	D3DXHANDLE hFB;
	D3DXHANDLE hIn;
	D3DXHANDLE hT[4];
	float ScreenSize[2], ScreenSizeOne[2];
	

public:

	// Constructor
	CPPEffect(IDirect3DDevice9* Device, int Width, int Height, float sWidth, float sHeight) : _Effect(0), _Device(Device), _IsActive(true)
	{
		RT = new cRT(Device);
		RT->Create(Width, Height, false, D3DFMT_UNKNOWN);
		TextureCount = 0;

		ScreenSize[0] = (float)Width;
		ScreenSize[1] = (float)Height;

		ScreenSizeOne[0] = 1.0f / ((float)Width);
		ScreenSizeOne[1] = 1.0f / ((float)Height);
	}
	~CPPEffect() { }

	// Properties
	ID3DXEffect* Effect() { return _Effect; }
	void Effect(ID3DXEffect* N)
	{
		_Effect = N;
		hFB = _Effect->GetParameterBySemantic(NULL, "BackBuffer");
		hIn = _Effect->GetParameterBySemantic(NULL, "PreviousEffect");
		hT[0] = _Effect->GetParameterBySemantic(NULL, "TextureStage0");
		hT[1] = _Effect->GetParameterBySemantic(NULL, "TextureStage1");
		hT[2] = _Effect->GetParameterBySemantic(NULL, "TextureStage2");
		hT[3] = _Effect->GetParameterBySemantic(NULL, "TextureStage3");
	}

	Property(bool, Active, _IsActive);
	Property(irr::core::stringc, Tag, _Tag);

	// Rendering method
	void Render(IDirect3DTexture9* Input,  IDirect3DTexture9* FBTex);

	cRT* RT;
	IDirect3DTexture9* Textures[4];
	int TextureCount;
	int ID;
};