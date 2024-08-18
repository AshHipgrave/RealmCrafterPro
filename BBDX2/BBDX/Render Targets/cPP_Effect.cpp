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
#include "cPP_Effect.h"
#include "headers.h"
#include "cRenderTargetManager.h"

D3DXHANDLE hEffect;

cParam::~cParam()	{	SAFE_DELETE( pValue );	}
void cParam::Load( XMLReader* X )
{
	Name = X->GetAttributeString("name");
	SetValue( X->GetAttributeString("type"), X->GetAttributeString("value") );
}

void cParam::Apply( ID3DXEffect *pEffect )
{
	switch (Type)
	{
		case PARAM_TYPE_INT:
			if ( Name == String("Samples"))
			{
				int p = *(int*)pValue;
				switch ( p )
				{
					case 9: pEffect->SetTechnique( pEffect->GetTechnique(0) ); break;
					case 7: pEffect->SetTechnique( pEffect->GetTechnique(1) ); break;
					case 5: pEffect->SetTechnique( pEffect->GetTechnique(2) ); break;
					case 3: pEffect->SetTechnique( pEffect->GetTechnique(3) ); break;
				}
			}
			else
			{
				hEffect	= pEffect->GetParameterByName(NULL, Name.c_str());
				V( pEffect->SetInt(hEffect, *(int*)pValue) );
			}
			break;
		case PARAM_TYPE_BOOL:
			hEffect	= pEffect->GetParameterByName(NULL, Name.c_str());
			V( pEffect->SetBool(hEffect, *(bool*)pValue) );
			break;
		case PARAM_TYPE_FLOAT:
			hEffect	= pEffect->GetParameterByName(NULL, Name.c_str());
			V( pEffect->SetFloat(hEffect, *(float*)pValue) );
			break;
		case PARAM_TYPE_FLOAT2:
			hEffect	= pEffect->GetParameterByName(NULL, Name.c_str());
			V( pEffect->SetFloatArray(hEffect, (float*)pValue, 2) );
			break;
		case PARAM_TYPE_FLOAT3:
			hEffect	= pEffect->GetParameterByName(NULL, Name.c_str());
			V( pEffect->SetFloatArray(hEffect, (float*)pValue, 3) );
			break;
		case PARAM_TYPE_FLOAT4:
 			hEffect	= pEffect->GetParameterByName(NULL, Name.c_str());
			V( pEffect->SetFloatArray(hEffect, (float*)pValue, 4) );
			break;
	}
}

void cParam::SetValue( ID3DXEffect *pEffect, ePARAM_TYPE type, String value )
{
	switch (type)
	{
		case PARAM_TYPE_INT:	SetValue( String("int"), value ); break;
		case PARAM_TYPE_BOOL:	SetValue( String("bool"), value ); break;
		case PARAM_TYPE_FLOAT:	SetValue( String("float"), value ); break;
		case PARAM_TYPE_FLOAT2:	SetValue( String("float2"), value ); break;
		case PARAM_TYPE_FLOAT3:	SetValue( String("float3"), value ); break;
		case PARAM_TYPE_FLOAT4:	SetValue( String("float4"), value ); break;
	}
}

void cParam::SetValue( String type, String value )
{
	Type = PARAM_TYPE_UNKNOWN;
	if ( type == String("int") )		
	{
		Type = PARAM_TYPE_INT;
		pValue = new int(value.ToInt());
	}
	else
	if ( type == String("bool") )
	{
		Type = PARAM_TYPE_BOOL;
		value.Lower();
		if ( value == String("true") ) pValue = new bool(true);
		else 
		if ( value == String("false") ) pValue = new bool(false);
	}
	else
	if ( type == String("float") )
	{
		Type = PARAM_TYPE_FLOAT;
		pValue = new float(value.ToFloat());
	}
	else
	if ( type == String("float2") )
	{
		Type = PARAM_TYPE_FLOAT2;
		String X, Y;

		int Sep = value.Instr(" ");
		X = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		X.Trim();

		Sep = value.Instr(" ");
		Y = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		Y.Trim();

		pValue = new float[2];
		((float*)pValue)[0] = X.ToFloat(); 
		((float*)pValue)[1] = Y.ToFloat(); 
	}
	else
	if ( type == String("float3") )	
	{
		Type = PARAM_TYPE_FLOAT3;
		String X, Y, Z;

		int Sep = value.Instr(" ");
		X = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		X.Trim();

		Sep = value.Instr(" ");
		Y = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		Y.Trim();

		Sep = value.Instr(" ");
		Z = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		Z.Trim();

		pValue = new float[3];
		((float*)pValue)[0] = X.ToFloat(); 
		((float*)pValue)[1] = Y.ToFloat(); 
		((float*)pValue)[2] = Z.ToFloat(); 
	}
	else
	if ( type == String("float4") )	
	{
		Type = PARAM_TYPE_FLOAT4;
		String X, Y, Z, W;

		int Sep = value.Instr(" ");
		X = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		X.Trim();
		
		Sep = value.Instr(" ");
		Y = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		Y.Trim();

		Sep = value.Instr(" ");
		Z = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		Z.Trim();

		Sep = value.Instr(" ");
		W = value.Substr(0, Sep); value = value.Substr( Sep+1, value.Length()-(Sep+1) );
		W.Trim();

		pValue = new float[4];
		((float*)pValue)[0] = X.ToFloat(); 
		((float*)pValue)[1] = Y.ToFloat(); 
		((float*)pValue)[2] = Z.ToFloat(); 
		((float*)pValue)[3] = W.ToFloat(); 
	}
}


//////////////////////////////////////////////////////////////////////////
// class Effect
//////////////////////////////////////////////////////////////////////////
cEffect::cEffect( String name) : Name(name), pEffect(NULL) {}

cEffect::~cEffect() 
{
	FOR_EACH_VALUE( cParam*, pParam, std::list<cParam *>, lpParams)
		SAFE_DELETE( pParam );
	lpParams.clear();

//	SAFE_RELEASE( pEffect );
}

void cEffect::SetParameter( String Name, String Value )
{
	FOR_EACH_VALUE( cParam*, pParam, std::list<cParam *>, lpParams)
		if ( pParam->Name == Name ) 
		{
			SAFE_DELETE(pParam->pValue);
			pParam->SetValue( pEffect, pParam->Type, Value );
		}
}

void cEffect::Load( XMLReader* X )
{
	Name = X->GetAttributeString("name");
	Shader = X->GetAttributeString("shader");

	// Get values
	while(X->Read() && X->GetNodeType() != XNT_Element_End)
	{
		if(X->GetNodeType() == XNT_Element && X->GetNodeName().AsLower() == String("param"))
		{
			cParam* pParam = new cParam();
			lpParams.push_back( pParam );
			pParam->Load( X );
		}
	}
}

extern map<string, ID3DXEffect *> mpEffectsCache;
void cEffect::Apply( cRenderTarget *Dest, cRenderTarget *Source, IDirect3DTexture9* BackBuffer2)
{
	if ( Name == String("DownSampler") ) cRenderTargetManager::pRTM->DownSampler( Dest, Source );
	else
	if ( Name == String("UpSampler") ) cRenderTargetManager::pRTM->UpSampler( Dest, Source );
	else
	{
		map<string, ID3DXEffect *>::iterator iter = mpEffectsCache.find( Shader.c_str() );
		if (iter==mpEffectsCache.end())  
		{
			ID3DXBuffer* pErrors = NULL;
			D3DXCreateBuffer(4096, &pErrors);
			
			if(FAILED(D3DXCreateEffectFromFileA( pD3DDevice, String(String(PP_EffectPath.c_str()) + Shader).c_str(), NULL, NULL, dwShaderFlags, 
											NULL, &pEffect, &pErrors)))
			{
				MessageBoxA(0, (LPCSTR)pErrors->GetBufferPointer(), "cEffect::Apply", MB_OK);
				return;
			}

			OutputDebugStringA(String(String(PP_EffectPath.c_str()) + Shader + "\n").c_str());

			hEffect_vDimensions = pEffect->GetParameterByName(NULL, "vDimensions");
			hEffect_Texture0 = pEffect->GetParameterByName(NULL, "Texture0");

			mpEffectsCache.insert( make_pair(Shader.c_str(), pEffect) );
		}
		else
			pEffect = iter->second;

		FOR_EACH_VALUE( cParam*, pParam, list<cParam *>, lpParams )
			pParam->Apply( pEffect );

		Dest->ReSizeViewport(Source->CurrentWidth, Source->CurrentHeight);
		Dest->Set(0, -1, -1);
			cRenderTargetManager::pRTM->RenderQuad(pEffect, Source, hEffect_vDimensions, hEffect_Texture0, BackBuffer2);
		Dest->UnSet();
	}
}

//////////////////////////////////////////////////////////////////////////
// class PostProcess Effect
//////////////////////////////////////////////////////////////////////////
cPP_Effect::~cPP_Effect()
{
	FOR_EACH_VALUE( cEffect*, pEffect, std::list<cEffect *>, lpEffects)
		SAFE_DELETE( pEffect );
	lpEffects.clear();
}

void cPP_Effect::LoadFromXML( XMLReader* X )
{
	Name = X->GetAttributeString("name");

	// Get values
	while(X->Read() && X->GetNodeType() != XNT_Element_End)
	{
		// If its a <cfg> element
		if(X->GetNodeType() == XNT_Element && X->GetNodeName().AsLower() == String("effect"))
		{
			cEffect* pEffect = new cEffect();
			lpEffects.push_back( pEffect );
 			pEffect->Load( X );
		}
	}
}

cRenderTarget * cPP_Effect::ApplyPP(cRenderTarget *Dest, cRenderTarget *Source, cRenderTarget *RT_Scene, IDirect3DTexture9* BackBuffer2)
{
	bool p(true);
	cRenderTarget *pSource = Dest, *pDest = Source;
	FOR_EACH_VALUE( cEffect*, pEffect, std::list<cEffect*>, lpEffects)
	{
		if ( p )	{	pSource = Source;	pDest = Dest;	}
		else		{	pSource = Dest;		pDest = Source;	}

		pEffect->Apply(pDest, pSource, BackBuffer2);
		p = !p;
	}

	return pDest;
}