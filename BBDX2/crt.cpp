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
//#include "dxstdafx.h"
#include <d3d9.h>
#include <math.h>
#include "cRT.h"

#include <stdio.h>

IDirect3DSurface9* pNormalColorBuffer(NULL); // The back buffer

cRT::cRT()
	: ClearColor(0xff000000), DepthStencilSurface(0), PreviousDepthStencilSurface(0)
{
	RWidth = Width = RHeight = Height = 0;
	pTexture = NULL;
}

cRT::cRT(IDirect3DDevice9* Device)
{
	TargetFormat = D3DFMT_A8R8G8B8;
	pd3dDevice = Device;
	RWidth = Width = RHeight = Height = 0;
	pTexture = NULL;
	_IsSet = false;
}

cRT::cRT(IDirect3DDevice9* Device, D3DFORMAT Format)
{
	TargetFormat = Format;
	pd3dDevice = Device;
	RWidth = Width = RHeight = Height = 0;
	pTexture = NULL;
	_IsSet = false;
	pTextureSurface = NULL;
	DepthStencilSurface = NULL;
}

cRT::~cRT()
{
	if(pTextureSurface != NULL)
		pTextureSurface->Release();

	if(pTexture != NULL)
		pTexture->Release();

	if(DepthStencilSurface != 0)
		DepthStencilSurface->Release();

	pTexture = NULL;
	pTextureSurface = NULL;
	DepthStencilSurface = NULL;
}

bool cRT::Create(uint Width, uint Height, bool Pow2, D3DFORMAT stencilFormat)
{
	RWidth  = this->Width = Width;
	RHeight = this->Height = Height;

    if (Pow2)
	{
		// adjust width/height to next smaller power of 2 if they are not power of 2
		short logWidth  = static_cast<short>(logf(static_cast<float>(Width)) /logf(2.0f));
		short logHeight = static_cast<short>(logf(static_cast<float>(Height))/logf(2.0f));

		this->Width  = (uint)powf(2, logWidth+1 );
		this->Height = (uint)powf(2, logHeight+1);
	}

	HRESULT hr; 
	hr = pd3dDevice->CreateTexture(this->Width, this->Height, 1, 
			D3DUSAGE_RENDERTARGET, TargetFormat, D3DPOOL_DEFAULT, &pTexture, NULL);
	if ( FAILED( hr) ) return false;

	

	pTexture->GetSurfaceLevel( 0, &pTextureSurface );


	if(stencilFormat != 0)
	{
		pd3dDevice->CreateDepthStencilSurface(Width, Height, stencilFormat, D3DMULTISAMPLE_NONE, 0, TRUE, &DepthStencilSurface, NULL);
	}else
	{
		DepthStencilSurface = 0;
		PreviousDepthStencilSurface = 0;
	}

	return true;
}

void cRT::Set(uint Width, uint Height, bool Clear, int index, bool clearZ)
{	
	//if(_IsSet)
	//	return;
	//else
	//	_IsSet = true;

	if (!pNormalColorBuffer)
	{
		if(FAILED(pd3dDevice->GetRenderTarget(index, &pNormalColorBuffer)))
		{
			pNormalColorBuffer = NULL;
		}
	}


	if(DepthStencilSurface != 0)
	{
		pd3dDevice->GetDepthStencilSurface(&PreviousDepthStencilSurface);
		pd3dDevice->SetDepthStencilSurface(DepthStencilSurface);
	}else
	{
		PreviousDepthStencilSurface = 0;
	}

	pd3dDevice->GetViewport( &previousViewport );
	pd3dDevice->SetRenderTarget(index, pTextureSurface );



	

	Viewport = previousViewport;
	if (Viewport.Width > RWidth) Viewport.Width = RWidth;
	if (Viewport.Height > RHeight) Viewport.Height = RHeight;

	//if (Width == 0 && Height == 0) ReSize(Viewport.Width, Viewport.Height);
	//else
	//	if (Width != -1 && Height != -1) ReSize(Width, Height);

	Viewport.Width = RWidth;
	Viewport.Height = RHeight;

	pd3dDevice->SetViewport( &Viewport );

	DWORD ClearZ = clearZ ? D3DCLEAR_ZBUFFER : 0;

	if(Clear)
		pd3dDevice->Clear( 0L, NULL, D3DCLEAR_TARGET | ClearZ | D3DCLEAR_STENCIL, ClearColor, 1.0f, 0L);
}

void cRT::UnSet(bool ClearPrev, bool ClearZ, int index)
{
	//if(!_IsSet)
	//	return;
	//else
	//	_IsSet = false;

	IDirect3DSurface9* pCurrentBuffer;
	pd3dDevice->GetRenderTarget(index, &pCurrentBuffer);

	if(pCurrentBuffer != 0)
		pCurrentBuffer->Release();

	if(PreviousDepthStencilSurface != 0)
	{
		pd3dDevice->SetDepthStencilSurface(PreviousDepthStencilSurface);
		PreviousDepthStencilSurface->Release();
	}

	if(pCurrentBuffer != pTextureSurface)
		return;

	

	pd3dDevice->SetRenderTarget(index, pNormalColorBuffer );
	pd3dDevice->SetViewport( &previousViewport );

	if(pNormalColorBuffer != NULL)
	{
		pNormalColorBuffer->Release();
		pNormalColorBuffer = NULL;
	}

	if(ClearPrev || ClearZ)
		pd3dDevice->Clear(0, NULL, (ClearPrev ? D3DCLEAR_TARGET : 0) | (ClearZ ? D3DCLEAR_ZBUFFER : 0), 0xff000000, 1.0f, 0);
}

void cRT::ReSize(uint Width, uint Height)
{
	if (Width < this->Width) RWidth = Width;
	else RWidth = this->Width;
	
	if (Height < this->Height) RHeight = Height;
	else RHeight = this->Height;
}

void cRT::GetDimension(float &Width, float &Height)
{
	Width = RWidth / (float)this->Width;
	Height =  RHeight / (float)this->Height;
}