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

class ShaderConstantSet;
namespace irr
{
	namespace core{}
	namespace video{}
}

#include <d3dx9.h>
#include "Matrix4.h"
#include "scolor.h"
#include "vector2d.h"
#include "ITexture.h"
#include "CD3D9Driver.h"
#include "ISceneNode.h"

#include <ArrayList.h>
#include <NGinString.h>
#include <string>

using namespace irr;
using namespace irr::core;
using namespace irr::video;

#define SetMacroDef(IrrType, Name) void Set##Name(IrrType n##Name);
#define SetMacroArrayDef(IrrType, Name) void Set##Name(IrrType n##Name, int Index);
#define HSem(Var, Name) Var = Effect->GetParameterBySemantic(NULL, Name);

enum ShaderType
{
	ST_Void,
	ST_Bool,
	ST_Int,
	ST_Float,
	ST_Float2,
	ST_Float3,
	ST_Float4,
	ST_Float3x2,
	ST_Float3x3,
	ST_Float4x4,
	ST_String,
	ST_Unknown
};

inline int GetParameterSize(ShaderType Type)
{
	switch(Type)
	{
	case ST_Void:
		return 0;
	case ST_Bool:
		return sizeof(BOOL);
	case ST_Int:
		return sizeof(INT);
	case ST_Float:
		return sizeof(FLOAT);
	case ST_Float2:
		return sizeof(FLOAT) * 2;
	case ST_Float3:
		return sizeof(FLOAT) * 3;
	case ST_Float4:
		return sizeof(FLOAT) * 4;
	case ST_Float3x2:
		return sizeof(FLOAT) * 6;
	case ST_Float3x3:
		return sizeof(FLOAT) * 9;
	case ST_Float4x4:
		return sizeof(FLOAT) * 16;
	case ST_String:
		return 2048;
	}

	return 0;
}

struct ShaderParameter
{
	ArrayList<ShaderParameter*> Annotations;

	ShaderType Type;
	NGin::CString Name;
	void* Data;
};

class ShaderConstantSet
{
public:
	ArrayList<ShaderParameter*> Parameters;

private:
	std::string Path;

	D3DXHANDLE TechniqueDefault;
	D3DXHANDLE TechniqueRT0;
	D3DXHANDLE TechniqueRT1;
	D3DXHANDLE TechniqueRT2;

	D3DXHANDLE hWorld;
	D3DXHANDLE hView;
	D3DXHANDLE hProjection;
	D3DXHANDLE hViewInverse;
	D3DXHANDLE hViewProjection;
	D3DXHANDLE hWorldViewProjection;
	D3DXHANDLE hLightProjection;
	D3DXHANDLE hCameraPosition;
	D3DXHANDLE hBones;
	D3DXHANDLE hIsAnimated;
	D3DXHANDLE hTime;
	D3DXHANDLE hAlpha;
	D3DXHANDLE hPassC;
	D3DXHANDLE hFogColor;
	D3DXHANDLE hFogData;
	D3DXHANDLE hShadowMap;
	D3DXHANDLE hReflectionMap;
	D3DXHANDLE hReflectionViewProj;
	D3DXHANDLE hTextureTransform[4];
	D3DXHANDLE hTextureStage[4];
	D3DXHANDLE hLightAmbient;
	D3DXHANDLE hDirectionalColor;
	D3DXHANDLE hDirectionalNormal;
	D3DXHANDLE hPointColor;
	D3DXHANDLE hPointPosition;
	D3DXHANDLE hPointRadius;
	D3DXHANDLE hFrameBuffer;
	D3DXHANDLE hFrameBuffer2;
	D3DXHANDLE hViewPort;
	ID3DXEffect* Effect;

	matrix4 World;
	matrix4 View;
	matrix4 Projection;
	matrix4 ViewInverse;
	matrix4 ViewProjection;
	matrix4 WorldViewProjection;
	matrix4 LightProjection;
	vector3df CameraPosition;
	bool IsAnimated;
	float Time;
	float Alpha;
	SColorf PassC;
	SColorf FogColor;
	vector2df FogData;
	SColorf LightAmbient;
	SColorf DirectionalColor[3];
	vector3df DirectionalNormal[3];
	SColorf PointColor[5];
	vector3df PointPosition[5];
	float PointRadius[5];
	IDirect3DTexture9* ShadowMap;
	IDirect3DTexture9* ReflectionMap;
	matrix4 ReflectionViewProj;
	ITexture* FrameBuffer;
	ITexture* FrameBuffer2;
	matrix4 TextureTransform[4];
	ITexture* TextureStage[4];
	vector2df ViewPort;

public:
	SetMacroDef(matrix4&, World);
	SetMacroDef(matrix4&, View);
	SetMacroDef(matrix4&, Projection);
	SetMacroDef(matrix4&, ViewInverse);
	SetMacroDef(matrix4&, ViewProjection);
	SetMacroDef(matrix4&, WorldViewProjection);
	SetMacroDef(matrix4&, LightProjection);

	SetMacroDef(vector3df&, CameraPosition);
	SetMacroDef(bool, IsAnimated);
	SetMacroDef(float, Time);
	SetMacroDef(float, Alpha);
	SetMacroDef(SColorf&, PassC);
	SetMacroDef(SColorf&, FogColor);
	SetMacroDef(vector2df&, FogData);
	SetMacroDef(SColorf&, LightAmbient);
	SetMacroDef(vector2df&, ViewPort);

	SetMacroArrayDef(SColorf&, DirectionalColor);
	SetMacroArrayDef(vector3df&, DirectionalNormal);
	SetMacroArrayDef(SColorf&, PointColor);
	SetMacroArrayDef(vector3df&, PointPosition);
	SetMacroArrayDef(float, PointRadius);
	void CommitLights();

	SetMacroArrayDef(matrix4&, TextureTransform);
	SetMacroArrayDef(ITexture*, TextureStage);
	SetMacroDef(IDirect3DTexture9*, ShadowMap);
	SetMacroDef(IDirect3DTexture9*, ReflectionMap);
	SetMacroDef(matrix4&, ReflectionViewProj);
	SetMacroDef(ITexture*, FrameBuffer);
	SetMacroDef(ITexture*, FrameBuffer2);

	const char* GetPath()
	{
		return Path.c_str();
	}

	ShaderConstantSet(ID3DXEffect* nEffect, const char* path);
	~ShaderConstantSet();

	void ResetParameters();

	ID3DXEffect* GetEffect()
	{
		return Effect;
	}

	bool HasBones()
	{
		return hBones != NULL;
	}

	bool SetTechnique(int rtIndex);

	void SetBones(matrix4* Bones, int Length)
	{
		Effect->SetMatrixArray(hBones, (D3DXMATRIX*)Bones, Length);
	}

	void SetSceneNodeConstants(irr::scene::ISceneNode* Node);

	void Reset();

	//void SetWorld(matrix4& nWorld)
	//{
	//	if(World != nWorld)
	//	{
	//		World = nWorld;
	//		Effect->SetMatrix(hWorld, (D3DXMATRIX*)&World);
	//	}
	//}

};