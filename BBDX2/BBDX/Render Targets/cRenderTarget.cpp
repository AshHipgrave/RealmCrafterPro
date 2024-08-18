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
#include "cRenderTarget.h"

list<IDirect3DSurface9*> lpStackedRenderTargets[4];
list<IDirect3DSurface9*> lpZBuffers;

extern int bbdxTextureAlloc;
extern int bbdxTargetAlloc;


cRenderTarget::cRenderTarget(LPDIRECT3DDEVICE9 D3DDevice)
{
	SurfaceWidth = SurfaceHeight = CurrentWidth = CurrentHeight = 0;
	pColorSurface = pZBufferSurface = NULL;
	pColorTexture = pZBufferTexture = NULL;
	iSetInSlot = 255;
// 	Semantic = "";
// 	Category = 0;
	bIsCubeMap = false;
	CurrentCubeFace = D3DCUBEMAP_FACE_POSITIVE_X;
	pD3DDevice = D3DDevice;
	MultiSamplingType = D3DMULTISAMPLE_NONE;
}

cRenderTarget::~cRenderTarget()
{
	if (iSetInSlot < 4 )
		lpStackedRenderTargets[iSetInSlot].remove( pColorSurface );
	lpZBuffers.remove( pZBufferSurface );

	SAFE_RELEASE(pColorSurface); SAFE_RELEASE(pColorTexture);
	SAFE_RELEASE(pZBufferSurface); SAFE_RELEASE(pZBufferTexture);
}

		string GetD3DFormatText( D3DFORMAT Format )
		{
			switch( Format )
			{
				case 0						: return "none";
				case D3DFMT_A8R8G8B8		: return "A8R8G8B8";
				case D3DFMT_A2R10G10B10		: return "A2R10G10B10";

				case D3DFMT_G16R16			: return "G16R16";
				case D3DFMT_A16B16G16R16	: return "A16B16G16R16";

				case D3DFMT_G16R16F			: return "G16R16F";
				case D3DFMT_A16B16G16R16F	: return "A16B16G16R16F";

				case D3DFMT_R32F			: return "R32F";

				case D3DFMT_G32R32F			: return "G32R32F";
				case D3DFMT_A32B32G32R32F	: return "A32B32G32R32F";

				case D3DFMT_D24S8			: return "D24S8";
				case D3DFMT_D24X8			: return "D24X8";
				case 1515474505				: return "INTZ";
				case 1515667794				: return "RAWZ";
				case 875710020				: return "DF24";

				default						: return "unknown";
			}
		}

bool cRenderTarget::Create(DWORD Width, DWORD Height, D3DFORMAT colorFormat, D3DFORMAT depthStencilFormat, bool isCubeMap, D3DMULTISAMPLE_TYPE multiSamplingType)
{
	CurrentWidth  = SurfaceWidth  = Width;
	CurrentHeight = SurfaceHeight = Height;
	ColorFormat = colorFormat;
	ZBufferFormat = depthStencilFormat;
//	Category = 0;
	bIsCubeMap = isCubeMap;
	MultiSamplingType = multiSamplingType;

	///////////////////////////////////////
	// C O L O R   B U F F E R
	//////////////////////////////
		SAFE_RELEASE( pColorSurface );
		SAFE_RELEASE( pColorTexture );

		if (ColorFormat != 0) 
		{
			if (MultiSamplingType !=0)
			{
				pColorTexture = NULL;
				pD3DDevice->CreateRenderTarget(SurfaceWidth, SurfaceHeight, ColorFormat, MultiSamplingType, 0, false, &pColorSurface, NULL);

				//JB: Adding resolve texture into RT class
				pD3DDevice->CreateTexture(SurfaceWidth, SurfaceHeight, 1, D3DUSAGE_RENDERTARGET, ColorFormat, D3DPOOL_DEFAULT, reinterpret_cast<IDirect3DTexture9**>(&pColorTexture), NULL);

				if (!pColorSurface)
				{
					PrintLog("D3D", "Unable to create MSAA Color Render Target [%s %dx%d @ %s [%dx]]\n", (bIsCubeMap?"CUBE":"2D"), Width, Height, GetD3DFormatText(ColorFormat).c_str(), MultiSamplingType );
					return false;
				}
			}
			else
			{
				if (bIsCubeMap)
				{
					pD3DDevice->CreateCubeTexture(SurfaceWidth, 1, D3DUSAGE_RENDERTARGET, ColorFormat, D3DPOOL_DEFAULT, (IDirect3DCubeTexture9**)&pColorTexture, NULL);
				}else
				{
					pD3DDevice->CreateTexture(SurfaceWidth, SurfaceHeight, 1, D3DUSAGE_RENDERTARGET, ColorFormat, D3DPOOL_DEFAULT, (IDirect3DTexture9**)&pColorTexture, NULL);
				}

				if (!pColorTexture)
				{
					PrintLog("D3D", "Unable to create Color Render Target [%s %dx%d @ %s]\n", (bIsCubeMap?"CUBE":"2D"), Width, Height, GetD3DFormatText(ColorFormat).c_str() );
					return false;
				}
				else
				{
					if (bIsCubeMap)
					{
						((IDirect3DCubeTexture9*)pColorTexture)->GetCubeMapSurface( CurrentCubeFace, 0, &pColorSurface );
					}else
					{
						((IDirect3DTexture9*)pColorTexture)->GetSurfaceLevel( 0, &pColorSurface );
					}
				}
			}
		}




	///////////////////////////////////////
	// D E P T H   B U F F E R
	//////////////////////////////
		SAFE_RELEASE( pZBufferSurface );
		SAFE_RELEASE( pZBufferTexture );
		if (ZBufferFormat != D3DFMT_UNKNOWN || MultiSamplingType != 0) 
		{
			//JB: If MSAA is used, then we more than likely need a new zbuffer.
			if(MultiSamplingType != 0)
				ZBufferFormat = D3DFMT_D24S8;

			if (MultiSamplingType!=0)
			{
				if FAILED(pD3DDevice->CreateDepthStencilSurface(SurfaceWidth, SurfaceHeight, ZBufferFormat, MultiSamplingType, 0, true, &pZBufferSurface, NULL))
				{
					PrintLog("D3D", "Unable to create MSAA ZBuffer [%s %dx%d @ %s (%dx)]\n", (bIsCubeMap?"CUBE":"2D"), Width, Height, GetD3DFormatText(ZBufferFormat).c_str(), MultiSamplingType );
					return false;
				}
			}
			else
			{
				if SUCCEEDED(pD3DDevice->CreateTexture(SurfaceWidth, SurfaceHeight, 1, D3DUSAGE_DEPTHSTENCIL, ZBufferFormat, D3DPOOL_DEFAULT, &pZBufferTexture, NULL))
					pZBufferTexture->GetSurfaceLevel( 0, &pZBufferSurface );
				else
					if FAILED(pD3DDevice->CreateDepthStencilSurface(SurfaceWidth, SurfaceHeight, ZBufferFormat, D3DMULTISAMPLE_NONE, 0, true, &pZBufferSurface, NULL))
					{
						PrintLog("D3D", "Unable to create ZBuffer [%s %dx%d @ %s]\n", (bIsCubeMap?"CUBE":"2D"), Width, Height, GetD3DFormatText(ZBufferFormat).c_str() );
						return false;
					}
			}
		}

	return true;
}

void cRenderTarget::Reset()
{
	if (iSetInSlot < 4 )
		lpStackedRenderTargets[iSetInSlot].remove( pColorSurface );
	lpZBuffers.remove( pZBufferSurface );

	SAFE_RELEASE(pColorSurface); SAFE_RELEASE(pColorTexture);
	SAFE_RELEASE(pZBufferSurface); SAFE_RELEASE(pZBufferTexture);
}

void cRenderTarget::EnableCubeFace( D3DCUBEMAP_FACES NewFace )
{
	if ( !bIsCubeMap || (CurrentCubeFace == NewFace) || !pColorTexture ) return;
	CurrentCubeFace = NewFace;
	SAFE_RELEASE( pColorSurface );

	((IDirect3DCubeTexture9*)pColorTexture)->GetCubeMapSurface( CurrentCubeFace, 0, &pColorSurface );
}

void cRenderTarget::Set(Byte RTargetIndex, DWORD Width, DWORD Height)
{	
	iSetInSlot = RTargetIndex;
	D3DVIEWPORT9 Viewport;
	pD3DDevice->GetViewport( &previousViewport );

	if ((iSetInSlot < 4) && (pColorSurface) )
	{
		IDirect3DSurface9* pCurrentRT(NULL); // The back buffer
		pD3DDevice->GetRenderTarget(iSetInSlot, &pCurrentRT);

		if SUCCEEDED(pD3DDevice->SetRenderTarget(iSetInSlot, pColorSurface )) 
			lpStackedRenderTargets[iSetInSlot].push_back( pCurrentRT );
		else 
			pCurrentRT->Release();
	}

	if (pZBufferSurface)
	{
		IDirect3DSurface9* pCurrentZBuffer(NULL); 
		pD3DDevice->GetDepthStencilSurface(&pCurrentZBuffer);

		if SUCCEEDED(pD3DDevice->SetDepthStencilSurface(pZBufferSurface )) 
			lpZBuffers.push_back( pCurrentZBuffer );
		else 
			pCurrentZBuffer->Release();
	}


	Viewport = previousViewport;

	if ((Width == 0) || (Height == 0)) {
		ReSizeViewport(Viewport.Width, Viewport.Height);
		return;
	}
	else
		if ((Width != -1) && (Height != -1)) ReSizeViewport(Width, Height);

	Viewport.Width  = CurrentWidth;
	Viewport.Height = CurrentHeight;

	pD3DDevice->SetViewport( &Viewport );
}

void cRenderTarget::UnSet()
{
	if ((iSetInSlot<4) && (pColorSurface)) 
	{
		IDirect3DSurface9* pPreviousRT = NULL;
		if (!lpStackedRenderTargets[iSetInSlot].empty()) 
		{
			pPreviousRT = lpStackedRenderTargets[iSetInSlot].back();
			lpStackedRenderTargets[iSetInSlot].pop_back();
		}

		pD3DDevice->SetRenderTarget(iSetInSlot, pPreviousRT );
		SAFE_RELEASE(pPreviousRT);
		//pColorTexture->GenerateMipSubLevels();
	}

	if (pZBufferSurface && !lpZBuffers.empty())
	{
		IDirect3DSurface9* pPreviousZBuffer = lpZBuffers.back();
		lpZBuffers.pop_back();
		pD3DDevice->SetDepthStencilSurface( pPreviousZBuffer );
		SAFE_RELEASE(pPreviousZBuffer);
	}

	pD3DDevice->SetViewport( &previousViewport );
	iSetInSlot = 255;
}


void cRenderTarget::ReSizeViewport(DWORD NewWidth, DWORD NewHeight)
{
	CurrentWidth  = MIN( NewWidth, SurfaceWidth );
	CurrentHeight = MIN( NewHeight, SurfaceHeight );
}

void cRenderTarget::GetDimensionFactors( float &Width, float &Height )
{
	Width  = CurrentWidth  / (float)SurfaceWidth;
	Height = CurrentHeight / (float)SurfaceHeight;
}

float cRenderTarget::GetViewAspect()
{
	return CurrentWidth/(float)CurrentHeight;
}

void cRenderTarget::ResolveMSAA( bool KeepZBuffer )
{
	if (MultiSamplingType == 0 || pColorSurface == 0 || pColorTexture == 0)
		return;

	//JB: Modified to resolve to a local texture than a whole new RT (so they can be treated the same)
	IDirect3DSurface9* TextureSurface = NULL;
	reinterpret_cast<IDirect3DTexture9*>(pColorTexture)->GetSurfaceLevel(0, &TextureSurface);
	if(TextureSurface == NULL)
		return;

	if(FAILED(pD3DDevice->StretchRect(pColorSurface, NULL, TextureSurface, NULL, D3DTEXF_LINEAR)))
	{
		OutputDebugString("Failed to resolve MSAA RT\n");
		TextureSurface->Release();
		return;
	}

	TextureSurface->Release();

}
