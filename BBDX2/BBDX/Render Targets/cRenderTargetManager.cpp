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
#include "cRenderTargetManager.h"

DWORD dwShaderFlags = 0;
LPDIRECT3DDEVICE9 pD3DDevice;
cRenderTargetManager *cRenderTargetManager::pRTM = NULL;

IDirect3DVertexBuffer9 *pVB = NULL;
string PP_EffectPath = "Data\\Game Data\\Shaders\\PostProcess\\";

struct SCREEN_VERTEX 
{
	float x, y, z;
	float u, v;
};

const D3DVERTEXELEMENT9 DEFSCREEN_VERTEX[] =
{
	{0, 0,  D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
	{0, 12, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
	D3DDECL_END()	
};
IDirect3DVertexDeclaration9* SCREEN_VERTEXDecl;

#define D3DFVF_SCREEN_VERTEX (D3DFVF_XYZ|D3DFVF_TEX1)

HRESULT hr;
ID3DXEffect *pEffectRenderTexture = NULL; 
D3DXHANDLE hEffectRenderTexture_vDimensions, hEffectRenderTexture_Texture0;

map<string, ID3DXEffect *> mpEffectsCache;	// Filename -> Effect

//////////////////////////////////////////////////////////////////////////
// PostProcess Functions
//////////////////////////////////////////////////////////////////////////
void PP_BuildQuad()
{
	V( pD3DDevice->CreateVertexBuffer(	4 * sizeof(SCREEN_VERTEX), D3DUSAGE_WRITEONLY|D3DUSAGE_DONOTCLIP,
		D3DFVF_SCREEN_VERTEX, D3DPOOL_MANAGED, &pVB, NULL) ); 

	pD3DDevice->CreateVertexDeclaration(DEFSCREEN_VERTEX, &SCREEN_VERTEXDecl);

	SCREEN_VERTEX* pVerts;

	if FAILED(pVB->Lock( 0, 0, (void**)(&pVerts) , 0 )) return;
	pVerts[0].x =  1;	pVerts[0].y =  1;	pVerts[0].z = 1;
	pVerts[1].x = -1;	pVerts[1].y =  1;	pVerts[1].z = 1;
	pVerts[2].x =  1;	pVerts[2].y = -1;	pVerts[2].z = 1;
	pVerts[3].x = -1;	pVerts[3].y = -1;	pVerts[3].z = 1;

	pVerts[0].u = 1;	pVerts[0].v = 0;
	pVerts[1].u = 0;	pVerts[1].v = 0;
	pVerts[2].u = 1;	pVerts[2].v = 1;
	pVerts[3].u = 0;	pVerts[3].v = 1;

	pVB->Unlock();
}

void PP_RenderQuad(ID3DXEffect *pEffect, cRenderTarget *Source, D3DXHANDLE hDimensionVar, D3DXHANDLE hTexture0, IDirect3DTexture9* BackBuffer2)
{
	if (!pVB) PP_BuildQuad();

	if (Source)
	{
		D3DVIEWPORT9 Viewport;
		pD3DDevice->GetViewport(&Viewport);

		//pD3DDevice->SetFVF(D3DFVF_SCREEN_VERTEX);
		pD3DDevice->SetVertexDeclaration(SCREEN_VERTEXDecl);
		pD3DDevice->SetStreamSource( 0, pVB, 0, sizeof(SCREEN_VERTEX) );

		float vDimensions[4];  // { UsedW/TextureW, UsedH/TextureH,  1/TextureW, 1/TextureH }

		vDimensions[0] = (Source->CurrentWidth)  / (float)Source->SurfaceWidth;
		vDimensions[1] = (Source->CurrentHeight) / (float)Source->SurfaceHeight;

		vDimensions[2] = 2.0f / (float)Source->SurfaceWidth;
		vDimensions[3] = 2.0f / (float)Source->SurfaceHeight;

		//	vDimensions[2] = 0.5f/((float)Viewport.Width-1);
		//	vDimensions[3] = 0.5f/((float)Viewport.Height-1);

		hDimensionVar	= pEffect->GetParameterByName(NULL, "vDimensions");
		hTexture0		= pEffect->GetParameterByName(NULL, "Texture0");
		D3DXHANDLE hTexture1		= pEffect->GetParameterByName(NULL, "Texture1");
		D3DXHANDLE hTexture1_2		= pEffect->GetParameterByName(NULL, "Texture1_2");

		if (hDimensionVar!=0)	{	V( pEffect->SetFloatArray(hDimensionVar, vDimensions, 4) )		}
		//else					{	V( pEffect->SetFloatArray("vDimensions", vDimensions, 4) )		}
		if (hTexture0!=0)		{	V( pEffect->SetTexture(hTexture0, Source->pColorTexture ))		}
		//else					{	V( pEffect->SetTexture("Texture0", Source->pColorTexture ) )	}

		if (hTexture1 != 0)
			pEffect->SetTexture(hTexture1, cRenderTargetManager::pRTM->pRT_Scene->pColorTexture);
		if (hTexture1_2 != 0)
			pEffect->SetTexture(hTexture1_2, BackBuffer2);
	}

	UINT cPasses;
	pEffect->Begin( &cPasses, 0 );
	for( DWORD p = 0; p < cPasses; ++p )
	{
		pEffect->BeginPass( p );
		pD3DDevice->DrawPrimitive(D3DPT_TRIANGLESTRIP, 0, 2);
		pEffect->EndPass();
	}
	pEffect->End();
}

void PP_RenderTexture(cRenderTarget *Source)
{
	if (!pEffectRenderTexture)
	{
		V( D3DXCreateEffectFromFileA( pD3DDevice, (PP_EffectPath+"ScreenQuad.fx").c_str(), NULL, NULL, dwShaderFlags, 
			NULL, &pEffectRenderTexture, NULL ) );

		hEffectRenderTexture_vDimensions	= pEffectRenderTexture->GetParameterByName(NULL, "vDimensions");
		hEffectRenderTexture_Texture0		= pEffectRenderTexture->GetParameterByName(NULL, "Texture0");

		mpEffectsCache.insert( make_pair("ScreenQuad", pEffectRenderTexture) );
	}

	PP_RenderQuad(pEffectRenderTexture, Source, hEffectRenderTexture_vDimensions, hEffectRenderTexture_Texture0, NULL);
}

void PP_DownSampler(cRenderTarget *Dest, cRenderTarget *Source)
{
	Dest->ReSizeViewport(Source->CurrentWidth / 2, Source->CurrentHeight / 2);

	RECT SourceRect;
		SourceRect.left = SourceRect.top = 0;
		SourceRect.right = Source->CurrentWidth;
		SourceRect.bottom = Source->CurrentHeight;
	RECT DestRect;
		DestRect.left = DestRect.top = 0;
		DestRect.right = Dest->CurrentWidth;
		DestRect.bottom = Dest->CurrentHeight;

	pD3DDevice->StretchRect(Source->pColorSurface, &SourceRect, Dest->pColorSurface, &DestRect, D3DTEXF_LINEAR);
}

void PP_UpSampler(cRenderTarget *Dest, cRenderTarget *Source)
{
	Dest->ReSizeViewport(Source->CurrentWidth * 2, Source->CurrentHeight * 2);

	RECT SourceRect;
		SourceRect.left = SourceRect.top = 0;
		SourceRect.right = Source->CurrentWidth;
		SourceRect.bottom = Source->CurrentHeight;
	RECT DestRect;
		DestRect.left = DestRect.top = 0;
		DestRect.right = Dest->CurrentWidth;
		DestRect.bottom = Dest->CurrentHeight;

	pD3DDevice->StretchRect(Source->pColorSurface, &SourceRect, Dest->pColorSurface, &DestRect, D3DTEXF_LINEAR);
}

void PP_ReleaseEffects()
{
	mpEffectsCache.clear();
	SAFE_RELEASE( pEffectRenderTexture );
	SAFE_RELEASE( pVB );
}

void PP_OnResetDevice()
{
	for (map<string, ID3DXEffect *>::iterator it = mpEffectsCache.begin(); it != mpEffectsCache.end(); ++it)
		it->second->OnResetDevice(); 
}

void PP_OnLostDevice()
{
	for (map<string, ID3DXEffect *>::iterator it = mpEffectsCache.begin(); it != mpEffectsCache.end(); ++it)
		it->second->OnLostDevice(); 
}
//////////////////////////////////////////////////////////////////////////
// cRenderTargetManager
//////////////////////////////////////////////////////////////////////////
cRenderTargetManager::cRenderTargetManager( LPDIRECT3DDEVICE9 D3DDevice )
{
	pD3DDevice = D3DDevice;
	pRTM = this;
}

cRenderTargetManager::~cRenderTargetManager()
{
 	PP_ReleaseEffects();
	FOR_EACH_VALUE( cRenderTarget*, pRT, list<cRenderTarget*>, lpRenderTargets) SAFE_DELETE(pRT);
	lpRenderTargets.clear();
}

cRenderTarget* cRenderTargetManager::GetRenderTarget( DWORD Width, DWORD Height, D3DFORMAT ColorFormat, D3DFORMAT ZBufferFormat, bool bIsCubeMap, bool bForcePow2, D3DMULTISAMPLE_TYPE msType)
{
	if ( (Width == 0) || (Height==0) )
	{
		D3DVIEWPORT9 Viewport;
		pD3DDevice->GetViewport( &Viewport );

		if (Width==0)  Width = Viewport.Width;
		if (Height==0) Height = Viewport.Height;
	}

	if (bForcePow2)
	{
		// adjust width/height to next smaller power of 2 if they are not power of 2
		short logWidth  = static_cast<short>(logf(static_cast<float>(Width-1)) /logf(2.0f));
		short logHeight = static_cast<short>(logf(static_cast<float>(Height-1))/logf(2.0f));

		Width  = (DWORD)powf(2, (float)(logWidth+1 ));
		Height = (DWORD)powf(2, (float)(logHeight+1));
	}

	cRenderTarget *pRT = new cRenderTarget(pD3DDevice);
	if (pRT->Create(Width, Height, ColorFormat, ZBufferFormat, bIsCubeMap, msType)) 
	{
		lpRenderTargets.push_back(pRT);
		return pRT;
	}

	delete pRT;
	return NULL;
}

void cRenderTargetManager::ReleaseRenderTarget( cRenderTarget* &pRenderTarget )
{
	if (pRenderTarget == NULL) {
		PrintLog("D3D", "Trying to release NULL Render Target\n" ); 
		return;
	}

	lpRenderTargets.remove( pRenderTarget );
	SAFE_DELETE( pRenderTarget );
}

void cRenderTargetManager::CreatePP_RenderTargets(int antiAlias)
{
	PP_OnResetDevice();
	pRT_Scene = GetRenderTarget( 0, 0, D3DFMT_A8R8G8B8, D3DFMT_UNKNOWN, false, false, (D3DMULTISAMPLE_TYPE)antiAlias);
	pRT0 = GetRenderTarget( 0, 0, D3DFMT_A8R8G8B8, D3DFMT_UNKNOWN );
	pRT1 = GetRenderTarget( 0, 0, D3DFMT_A8R8G8B8, D3DFMT_UNKNOWN );
}

void cRenderTargetManager::ReleasePP_RenderTargets()
{
	PP_OnLostDevice();
	ReleaseRenderTarget( pRT_Scene );
	ReleaseRenderTarget( pRT0 );
	ReleaseRenderTarget( pRT1 );
}

/////////////////////////////////////////////////////////
// POST PROCESSINGS
/////////////////////////////////////////////////////////
void cRenderTargetManager::CloneRenderTarget(cRenderTarget *Source, cRenderTarget *Dest)
{
	Dest->Set(0, Source->CurrentWidth, Source->CurrentHeight);
		PP_RenderTexture(Source);
	Dest->UnSet();
}

void cRenderTargetManager::RenderQuad(ID3DXEffect *pEffect, cRenderTarget *Dest, D3DXHANDLE hDimensionVar, D3DXHANDLE hTexture0, IDirect3DTexture9* BackBuffer2 )
{
	PP_RenderQuad( pEffect, Dest, hDimensionVar, hTexture0, BackBuffer2);
}

void cRenderTargetManager::RenderTexture(cRenderTarget *Source)
{
	PP_RenderTexture( Source );
}

void cRenderTargetManager::UpSampler(cRenderTarget *Dest, cRenderTarget *Source)
{
	PP_UpSampler(Dest, Source);
}

void cRenderTargetManager::DownSampler(cRenderTarget *Dest, cRenderTarget *Source)
{
	PP_DownSampler(Dest, Source);
}

