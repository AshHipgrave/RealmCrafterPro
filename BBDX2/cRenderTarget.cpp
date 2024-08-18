//#include "dxstdafx.h"
#include <d3d9.h>
#include <math.h>
#include "cRenderTarget.h"

#include <stdio.h>

IDirect3DSurface9* pNormalColorBuffer(NULL); // The back buffer

cRenderTarget::cRenderTarget()
{
	SurfaceWidth = SurfaceHeight = CurrentWidth = CurrentHeight = 0;
	pColorSurface = pZBufferSurface = NULL;
	pColorTexture = pZBufferTexture = NULL;
	iSetInSlot = 255;
	Semantic = "";
	Category = 0;
	bIsCubeMap = false;
	CurrentCubeFace = D3DCUBEMAP_FACE_POSITIVE_X;
	ColorFormat =  ZBufferFormat = D3DFMT_UNKNOWN;
	MultiSamplingType = D3DMULTISAMPLE_NONE;
}

cRenderTarget::cRenderTarget(IDirect3DDevice9* Device)
{
	TargetFormat = D3DFMT_X8R8G8B8;
	pd3dDevice = Device;
	RWidth = Width = RHeight = Height = 0;
	pTexture = NULL;
	_IsSet = false;
}

cRenderTarget::cRenderTarget(IDirect3DDevice9* Device, D3DFORMAT Format)
{
	TargetFormat = Format;
	pd3dDevice = Device;
	RWidth = Width = RHeight = Height = 0;
	pTexture = NULL;
	_IsSet = false;
}

cRenderTarget::~cRenderTarget()
{
	pTextureSurface->Release();
	pTexture->Release();
}

bool cRenderTarget::Create(uint Width, uint Height, bool Pow2)
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

	return true;
}

void cRenderTarget::Set(uint Width, uint Height, bool Clear)
{	
	//if(_IsSet)
	//	return;
	//else
	//	_IsSet = true;

	if (!pNormalColorBuffer)
		if(FAILED(pd3dDevice->GetRenderTarget(0, &pNormalColorBuffer)))
			OutputDebugString("No RT available!");

	pd3dDevice->GetViewport( &previousViewport );
	pd3dDevice->SetRenderTarget(0, pTextureSurface );

	Viewport = previousViewport;
	if (Viewport.Width > RWidth) Viewport.Width = RWidth;
	if (Viewport.Height > RHeight) Viewport.Height = RHeight;

	if (Width == 0 && Height == 0) ReSize(Viewport.Width, Viewport.Height);
	else
		if (Width != -1 && Height != -1) ReSize(Width, Height);

	pd3dDevice->SetViewport( &Viewport );

	if(Clear)
		pd3dDevice->Clear( 0L, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, 0xff000000, 1.0f, 0L);
}

void cRenderTarget::UnSet(bool ClearPrev, bool ClearZ)
{
	//if(!_IsSet)
	//	return;
	//else
	//	_IsSet = false;

	IDirect3DSurface9* pCurrentBuffer;
	pd3dDevice->GetRenderTarget(0, &pCurrentBuffer);
	pCurrentBuffer->Release();

	if(pCurrentBuffer != pTextureSurface)
		return;

	pd3dDevice->SetRenderTarget(0, pNormalColorBuffer );
	pd3dDevice->SetViewport( &previousViewport );
	pNormalColorBuffer->Release();
	pNormalColorBuffer = NULL;

	if(ClearPrev || ClearZ)
		pd3dDevice->Clear(0, NULL, (ClearPrev ? D3DCLEAR_TARGET : 0) | (ClearZ ? D3DCLEAR_ZBUFFER : 0), 0xff000000, 1.0f, 0);
}

void cRenderTarget::ReSize(uint Width, uint Height)
{
	if (Width < this->Width) RWidth = Width;
	else RWidth = this->Width;
	
	if (Height < this->Height) RHeight = Height;
	else RHeight = this->Height;
}

void cRenderTarget::GetDimension(float &Width, float &Height)
{
	Width = RWidth / (float)this->Width;
	Height =  RHeight / (float)this->Height;
}