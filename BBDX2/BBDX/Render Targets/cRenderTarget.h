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

#include "headers.h"
class cRenderTargetManager;

#define D3DFMT_INTZ	((D3DFORMAT)MAKEFOURCC('I', 'N', 'T', 'Z'))
#define D3DFMT_RAWZ	((D3DFORMAT)MAKEFOURCC('R', 'A', 'W', 'Z'))
#define D3DFMT_DF24	((D3DFORMAT)MAKEFOURCC('D', 'F', '2', '4'))

class cRenderTarget
{
	public:
		cRenderTarget(LPDIRECT3DDEVICE9 D3DDevice);
		~cRenderTarget();

			//////////////////////////////////////////////////////////////////////////////
			// if the Format or DepthStencilFormat is 0 then that buffer is not created
			///////////////////////////////////////////////////////////////////////////////////

		bool Create(DWORD Width, DWORD Height, D3DFORMAT ColorFormat, D3DFORMAT DepthStencilFormat = D3DFMT_UNKNOWN, bool bIsCubeMap = false,
			D3DMULTISAMPLE_TYPE MultiSamplingType = D3DMULTISAMPLE_NONE);

			///////////////////////////////////////////////////////////////////////////////////
			// Set: RTargetIndex   Can be any number between 0..3 to specify which MRT slot the colorBuffer will be assigned to
			//					   If >= 4 then the colorBuffer is not assigned and only the depthBuffe is used
			//		Width, Height -1 is to leave the view port as specified by the RT, 
			//					   0 use the window view port and 
			//					  >0 use those specified dimensions
			///////////////////////////////////////////////////////////////////////////////////

		virtual void Set(Byte RTargetIndex = 0, DWORD Width = 0, DWORD Height = 0);
		virtual void UnSet();

		virtual void EnableCubeFace( D3DCUBEMAP_FACES NewFace );

		virtual void ReSizeViewport(DWORD Width, DWORD Height);
		virtual void GetDimensionFactors( float &Width, float &Height );
		virtual float GetViewAspect();

		virtual void Reset();

// 		virtual void CreateDefaultResources();
// 		virtual void DestroyDefaultResources();
// 
// 		virtual void SetCategory( Byte Category );

		virtual void ResolveMSAA( bool KeepZBuffer=false);

		DWORD SurfaceWidth, SurfaceHeight, // The real dimensions of the RT 
			 CurrentWidth, CurrentHeight; // the dimensions used can be smaller like when downsampling for a blur
		D3DMULTISAMPLE_TYPE MultiSamplingType;
		D3DFORMAT ColorFormat, ZBufferFormat;

		IDirect3DBaseTexture9* pColorTexture;
		IDirect3DTexture9* pZBufferTexture;
		IDirect3DSurface9* pColorSurface, *pZBufferSurface; // reference to the 0 level surface of the corresponding texture

		D3DVIEWPORT9 previousViewport;

		float LastTime; // last time it was used (for the render target manager)
		Byte iSetInSlot;
// 		Byte Category;
		bool bIsCubeMap;
		D3DCUBEMAP_FACES CurrentCubeFace;
		LPDIRECT3DDEVICE9 pD3DDevice;


	private:
 		friend cRenderTargetManager;
};