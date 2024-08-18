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

#include "cRenderTarget.h"
class cRenderTargetManager
{
	public:
		cRenderTargetManager( LPDIRECT3DDEVICE9 D3DDevice );
		~cRenderTargetManager();
				
				//////////////////////////////////////////////////////
				// width = 0 uses the window view port and 
				// width > 0 use those dimensions
				//////////////////////////////////////////////////////
		virtual cRenderTarget* GetRenderTarget(DWORD Width, DWORD Height, D3DFORMAT ColorFormat, D3DFORMAT ZBufferFormat, bool bIsCubeMap = false, bool bForcePow2 = false, D3DMULTISAMPLE_TYPE msType = D3DMULTISAMPLE_NONE); 
		virtual void ReleaseRenderTarget( cRenderTarget* &pRenderTarget );

		virtual void CreatePP_RenderTargets(int antiAlias);
		virtual void ReleasePP_RenderTargets(); 


		//////////////////////
		// Post Processing
		//////////////////////////////////

			virtual void CloneRenderTarget(cRenderTarget *Source, cRenderTarget *Dest);

			virtual void RenderQuad(ID3DXEffect *pEffect, cRenderTarget *Dest = NULL, D3DXHANDLE hDimensionVar =0, D3DXHANDLE hTexture0 =0, IDirect3DTexture9* BackBuffer2 = NULL);
			virtual void RenderTexture(cRenderTarget *Source);

			virtual void UpSampler(cRenderTarget *Dest, cRenderTarget *Source);
			virtual void DownSampler(cRenderTarget *Dest, cRenderTarget *Source);


		static cRenderTargetManager *pRTM;
		std::list<cRenderTarget*> lpRenderTargets;
		cRenderTarget *pRT_Scene, *pRT0, *pRT1;
};