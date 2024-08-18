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
#include "cRenderTarget.h"
#include "XMLWrapper.h"

enum ePARAM_TYPE 
{
	PARAM_TYPE_INT,		
	PARAM_TYPE_FLOAT,		
	PARAM_TYPE_FLOAT2,		
	PARAM_TYPE_FLOAT3,		
	PARAM_TYPE_FLOAT4,		
	PARAM_TYPE_BOOL,
	PARAM_TYPE_UNKNOWN
};

//////////////////////////////////////////////////////////////////////////
// Param for Apply to Effect
//////////////////////////////////////////////////////////////////////////
class cParam 
{
	public:
		cParam( String name = "" ) : Name(name), pValue(NULL)	{}
		~cParam();

 		void Load( XMLReader* X );

		void Apply( ID3DXEffect *pEffect );

		void SetValue( ID3DXEffect *pEffect, ePARAM_TYPE type, String value );
		void SetValue( String type, String value );

		String Name;
		ePARAM_TYPE Type;
		void *pValue;
};

//////////////////////////////////////////////////////////////////////////
// single Effect
//////////////////////////////////////////////////////////////////////////
class cEffect
{
	public:
		cEffect( String name = "");
		~cEffect();

		void SetParameter( String Name, String Value );

		void Apply( cRenderTarget *Dest, cRenderTarget *Source, IDirect3DTexture9* BackBuffer2);
		void Load( XMLReader* X );

		String Name;
		String Shader;
		std::list<cParam *> lpParams;

		ID3DXEffect *pEffect;	
 		D3DXHANDLE hEffect_vDimensions, hEffect_Texture0;
};


//////////////////////////////////////////////////////////////////////////
// class Post Processing Effect
//////////////////////////////////////////////////////////////////////////
class cPP_Effect
{
	public:
		cPP_Effect( String name = "" ): Name(name)	{}
		~cPP_Effect();

		void LoadFromXML( XMLReader* X );
		cRenderTarget* ApplyPP(cRenderTarget *Dest, cRenderTarget *Source, cRenderTarget *RT_Scene, IDirect3DTexture9* BackBuffer2);


		String Name;
		std::list<cEffect *> lpEffects;
};