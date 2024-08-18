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
// Post Processing Effect code

#include "CPPEffect.h"

// FullScreen Quad Declaration
IDirect3DVertexBuffer9* FullScreenQuad = NULL;
IDirect3DVertexDeclaration9* SAVertDecl;
#define SAQUADFVF (D3DFVF_XYZ | D3DFVF_NORMAL | D3DFVF_TEX1)

//! Create the post processing quad
void BuildQuad(IDirect3DDevice9* Device)
{
	// Create it or die
	if(FAILED(Device->CreateVertexBuffer( 4 * sizeof(SAVert), 0, SAQUADFVF, D3DPOOL_MANAGED, &FullScreenQuad, NULL)))
	{
		MessageBox(NULL, "Failed to create PostProcessSaQuad", "Runtime Error", MB_OK | MB_ICONERROR);
		exit(0);
	}

	Device->CreateVertexDeclaration(DEFSAVert, &SAVertDecl);

	// Lock buffer
	SAVert* Verts;
	FullScreenQuad->Lock(0, 0, (void**)(&Verts), 0);
	
	// Setup
	Verts[0].x =  1;	Verts[0].y =  1;	Verts[0].z = 0;
	Verts[1].x = -1;	Verts[1].y =  1;	Verts[1].z = 0;
	Verts[2].x =  1;	Verts[2].y = -1;	Verts[2].z = 0;
	Verts[3].x = -1;	Verts[3].y = -1;	Verts[3].z = 0;

	Verts[2].u = 1;	Verts[2].v = 1;
	Verts[3].u = 0;	Verts[3].v = 1;
	Verts[0].u = 1;	Verts[0].v = 0;
	Verts[1].u = 0;	Verts[1].v = 0;

	// Unlock
	FullScreenQuad->Unlock();
}

void DestroyQuad()
{
	if(FullScreenQuad != NULL)
		FullScreenQuad->Release();
}


// Render the effect
void CPPEffect::Render(IDirect3DTexture9* Input, IDirect3DTexture9* FBTex)
{
	// Error check
	if(!_IsActive || _Device == 0 || _Effect == 0)
		return;

	// Make this local and set renderstate
	IDirect3DDevice9* Device = _Device;
	Device->SetRenderState(D3DRS_CULLMODE, D3DCULL_CW);

	// Get Effect
	ID3DXEffect* Effect = _Effect;

	// Setup Effect
	D3DXHANDLE Technique;
	Effect->FindNextValidTechnique(NULL, &Technique);
	Effect->SetTechnique(Technique);


	// Set Constants
	if(hFB != NULL)
		Effect->SetTexture(hFB, FBTex);

	if(hIn != NULL)
		Effect->SetTexture(hIn, Input);

	for(int i = 0; i < TextureCount; ++i)
		Effect->SetTexture(hT[i], Textures[i]);

	D3DXHANDLE Hr = Effect->GetParameterBySemantic(NULL, "DimTexture");
	Effect->SetFloatArray(Hr != 0 ? Hr : "vDimTexture", ScreenSizeOne, 2);

	Hr = Effect->GetParameterBySemantic(NULL, "DimTextureRender");
	Effect->SetFloatArray(Hr != 0 ? Hr : "vDimRenderText", ScreenSize, 2);

	// Setup Quad
	//Device->SetFVF(SAQUADFVF);
	Device->SetVertexDeclaration(SAVertDecl);
	Device->SetStreamSource(0, FullScreenQuad, 0, sizeof(SAVert));
	
	// Begin Shader
	UINT PassCount;
	Effect->Begin(&PassCount, 0);
	for(UINT Pass = 0; Pass < PassCount; ++Pass)
	{
		// Render
		if(FAILED(Effect->BeginPass(Pass)))
			break;
		Device->DrawPrimitive(D3DPT_TRIANGLESTRIP, 0, 2);
		Effect->EndPass();
	}
	Effect->End();

	Device->SetRenderState(D3DRS_CULLMODE, D3DCULL_CCW);
}




