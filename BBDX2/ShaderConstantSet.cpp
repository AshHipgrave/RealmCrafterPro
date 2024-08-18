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
#include "ShaderConstantSet.h"
#include "CD3D9Texture.h"


#define SetMacro(IrrType, RealType, CastType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name){if(Name != n##Name){Name = n##Name;if(h##Name != NULL)Effect->Set##RealType(h##Name, (CastType)&Name);}}
#define SetMacroNoRef(IrrType, RealType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name){if(Name != n##Name){Name = n##Name;if(h##Name != NULL)Effect->Set##RealType(h##Name, Name);}}
#define SetMacroFloats(IrrType, RealType, CastType, Name, Length) void ShaderConstantSet::Set##Name(IrrType n##Name){if(Name != n##Name){Name = n##Name;if(h##Name != NULL)Effect->Set##RealType(h##Name, (CastType)&Name, Length);}}
#define SetMacroArray(IrrType, RealType, CastType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name, int Index){if(Name[Index] != n##Name){Name[Index] = n##Name;if(h##Name[Index] != NULL)Effect->Set##RealType(h##Name[Index], (CastType)&(Name[Index]));}}
#define SetMacroArrayNoRef(IrrType, RealType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name, int Index){if(Name[Index] != n##Name){Name[Index] = n##Name;if(h##Name[Index] != NULL)Effect->Set##RealType(h##Name[Index], (Name[Index]));}}
#define SetMacroArrayFloats(IrrType, RealType, CastType, Name, Length) void ShaderConstantSet::Set##Name(IrrType n##Name, int Index){if(Name[Index] != n##Name){Name[Index] = n##Name;if(h##Name[Index] != NULL)Effect->Set##RealType(h##Name[Index], (CastType)&(Name[Index]), Length);}}
#define SetMacroTexArray(IrrType, RealType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name, int Index){if(Name[Index] != n##Name){Name[Index] = n##Name;if(h##Name[Index] != NULL)Effect->Set##RealType(h##Name[Index], ((irr::video::CD3D9Texture*)Name[Index])->getDX9Texture());Effect->CommitChanges();}}
#define SetMacroTex(IrrType, RealType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name){if(Name != n##Name){Name = n##Name;if(h##Name != NULL)Effect->Set##RealType(h##Name, ((CD3D9Texture*)Name)->getDX9Texture());}}

#define SetMacroTexNC(IrrType, RealType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name){if(h##Name != NULL && n##Name != NULL){Effect->Set##RealType(h##Name, ((CD3D9Texture*)n##Name)->getDX9Texture());}}


#define SetMacroTexD3D(IrrType, RealType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name){if(Name != n##Name){Name = n##Name;if(h##Name != NULL)Effect->Set##RealType(h##Name, Name);}}

#define SetMacroArrayNoRefNoCommit(IrrType, RealType, Name) void ShaderConstantSet::Set##Name(IrrType n##Name, int Index){if(Name[Index] != n##Name)Name[Index] = n##Name;}
#define SetMacroArrayFloatsNoCommit(IrrType, RealType, CastType, Name, Length) void ShaderConstantSet::Set##Name(IrrType n##Name, int Index){if(Name[Index] != n##Name)Name[Index] = n##Name;}


//SetMacro(matrix4&, Matrix, D3DXMATRIX*, World);
SetMacro(matrix4&, Matrix, D3DXMATRIX*, View);
SetMacro(matrix4&, Matrix, D3DXMATRIX*, Projection);
SetMacro(matrix4&, Matrix, D3DXMATRIX*, ViewInverse);
SetMacro(matrix4&, Matrix, D3DXMATRIX*, ViewProjection);
SetMacro(matrix4&, Matrix, D3DXMATRIX*, WorldViewProjection);
SetMacro(matrix4&, Matrix, D3DXMATRIX*, LightProjection);

SetMacroFloats(vector3df&, FloatArray, const FLOAT*, CameraPosition, 3);
SetMacroNoRef(bool, Bool, IsAnimated);
SetMacroNoRef(float, Float, Time);
SetMacroNoRef(float, Float, Alpha);
SetMacroFloats(SColorf&, FloatArray, const FLOAT*, PassC, 4);

//SetMacroFloats(SColorf&, FloatArray, const FLOAT*, FogColor, 4);
//#define SetMacroFloats(IrrType, RealType, CastType, Name, Length) 

void ShaderConstantSet::SetFogColor(SColorf& nFogColor)
{
	if(FogColor != nFogColor)
	{
		FogColor = nFogColor;
		if(hFogColor != NULL)
			Effect->SetFloatArray(hFogColor, (const FLOAT*)&FogColor, 4);
	}
}

void ShaderConstantSet::SetWorld(matrix4& World)
{
	if(hWorld != NULL)
		Effect->SetMatrix(hWorld, (D3DXMATRIX*)&World);
}

SetMacroFloats(vector2df&, FloatArray, const FLOAT*, FogData, 2);
SetMacroFloats(SColorf&, FloatArray, const FLOAT*, LightAmbient, 4);
SetMacroFloats(vector2df&, FloatArray, const FLOAT*, ViewPort, 2);

SetMacroArrayFloatsNoCommit(SColorf&, FloatArray, const FLOAT*, DirectionalColor, 4);
SetMacroArrayFloatsNoCommit(vector3df&, FloatArray, const FLOAT*, DirectionalNormal, 3);
SetMacroArrayFloatsNoCommit(SColorf&, FloatArray, const FLOAT*, PointColor, 4);
SetMacroArrayFloatsNoCommit(vector3df&, FloatArray, const FLOAT*, PointPosition, 3);
SetMacroArrayNoRefNoCommit(float, Float, PointRadius);

SetMacroArray(matrix4&, Matrix, D3DXMATRIX*, TextureTransform);
//SetMacroTexArray(ITexture*, Texture, TextureStage);
SetMacroTexD3D(IDirect3DTexture9*, Texture, ShadowMap);
SetMacroTexD3D(IDirect3DTexture9*, Texture, ReflectionMap);
SetMacro(matrix4&, Matrix, D3DXMATRIX*, ReflectionViewProj);
SetMacroTexNC(ITexture*, Texture, FrameBuffer);
SetMacroTexNC(ITexture*, Texture, FrameBuffer2);

// (IrrType, RealType, Name)
// 
// void ShaderConstantSet::Set##Name(IrrType nFrameBuffer2)
// {
// 	if(hFrameBuffer2 != NULL && Name != NULL)
// 	{
// 		Effect->Set##RealType(hFrameBuffer2, ((CD3D9Texture*)FrameBuffer2)->getDX9Texture());
// 	}
// }


inline ShaderType GetParameterType(D3DXPARAMETER_TYPE Type, int Collumns, int Rows)
{
	switch(Type)
	{
	case D3DXPT_VOID:
		return ST_Void;
	case D3DXPT_BOOL:
		return ST_Bool;
	case D3DXPT_INT:
		{
			if(Collumns == 1 && Rows == 1)
				return ST_Int;
			else
				return ST_Unknown;
		}
	case D3DXPT_FLOAT:
		{
			if(Collumns == 1 && Rows == 1)
				return ST_Float;
			else if(Collumns == 2 && Rows == 1)
				return ST_Float2;
			else if(Collumns == 3 && Rows == 1)
				return ST_Float3;
			else if(Collumns == 4 && Rows == 1)
				return ST_Float4;
			else if(Collumns == 3 && Rows == 2)
				return ST_Float3x2;
			else if(Collumns == 3 && Rows == 3)
				return ST_Float3x3;
			else if(Collumns == 4 && Rows == 4)
				return ST_Float4x4;
			else 
				return ST_Unknown;
		}
	case D3DXPT_STRING:
		return ST_String;
	}

	return ST_Unknown;
}




void ShaderConstantSet::ResetParameters()
{
	// Old Parameters
	for(int i = 0; i < Parameters.Size(); ++i)
	{
		for(int f = 0; f < Parameters[i]->Annotations.Size(); ++f)
		{
			delete Parameters[i]->Annotations[f]->Data;
			delete Parameters[i]->Annotations[f];
		}
		Parameters[i]->Annotations.Empty();
		delete Parameters[i];
	}
	Parameters.Empty();

	D3DXEFFECT_DESC EffectDesc;
	Effect->GetDesc(&EffectDesc);
	
	// Get new parameters
	for(UINT Index = 0; Index < EffectDesc.Parameters; ++Index)
	{
		D3DXHANDLE P = Effect->GetParameter(NULL, Index);
		if(P == NULL)
			continue;

		D3DXPARAMETER_DESC PDesc;
		Effect->GetParameterDesc(P, &PDesc);

		if(PDesc.Elements > 1)
			continue;

		ShaderType Type = GetParameterType(PDesc.Type, PDesc.Columns, PDesc.Rows);
		if(Type == ST_Unknown || Type == ST_Void)
			continue;

		ShaderParameter* Param = new ShaderParameter();
		Param->Type = Type;
		Param->Data = 0;
		Param->Name = PDesc.Name;

		Param->Data = new char[GetParameterSize(Type)];
		Effect->GetValue(P, (Param->Type == ST_String) ? &Param->Data : Param->Data, GetParameterSize(Type));

		for(int i = 0; i < PDesc.Annotations; ++i)
		{
			D3DXHANDLE A = Effect->GetAnnotation(P, i);
			if(A == NULL)
				continue;

			D3DXPARAMETER_DESC ADesc;
			Effect->GetParameterDesc(A, &ADesc);

			ShaderType AType = GetParameterType(ADesc.Type, ADesc.Columns, ADesc.Rows);
			if(AType == ST_Unknown || AType == ST_Void)
				continue;

			ShaderParameter* Annot = new ShaderParameter();
			Annot->Type = AType;
			Annot->Data = 0;
			Annot->Name = ADesc.Name;

			Annot->Data = new char[GetParameterSize(AType)];
			memset(Annot->Data, 0, GetParameterSize(AType));
			Effect->GetValue(A, (Annot->Type == ST_String) ? &Annot->Data : Annot->Data, GetParameterSize(AType));

			Param->Annotations.Add(Annot);
		}

		Parameters.Add(Param);

	}

	//// Output tree for debugging
	//char Output[20000];
	//sprintf(Output, "");
	//for(int i = 0; i < Parameters.Size(); ++i)
	//{
	//	sprintf(Output, "%s%i %s\n", Output, Parameters[i]->Type, Parameters[i]->Name.c_str());

	//	for(int f = 0; f < Parameters[i]->Annotations.Size(); ++f)
	//	{
	//		sprintf(Output, "%s    %i %s\n", Output, Parameters[i]->Annotations[f]->Type, Parameters[i]->Annotations[f]->Name.c_str());
	//	}
	//}

	//MessageBox(0, Output, 0, 0);
}

void ShaderConstantSet::CommitLights()
{
	D3DXVECTOR4 PointPositions[5];
	vector3df PointColors[5];

	vector3df DirectionalPositions[3];
	vector3df DirectionalColors[3];

	for(int i = 0; i < 5; ++i)
	{
		PointPositions[i].x = PointPosition[i].X;
		PointPositions[i].y = PointPosition[i].Y;
		PointPositions[i].z = PointPosition[i].Z;
		PointPositions[i].w = PointRadius[i];

		PointColors[i].X = PointColor[i].r;
		PointColors[i].Y = PointColor[i].g;
		PointColors[i].Z = PointColor[i].b;
	}

	for(int i = 0; i < 3; ++i)
	{
		DirectionalPositions[i].X = DirectionalNormal[i].X;
		DirectionalPositions[i].Y = DirectionalNormal[i].Y;
		DirectionalPositions[i].Z = DirectionalNormal[i].Z;

		DirectionalColors[i].X = DirectionalColor[i].r;
		DirectionalColors[i].Y = DirectionalColor[i].g;
		DirectionalColors[i].Z = DirectionalColor[i].b;
	}

	if(hPointPosition != NULL)
		Effect->SetValue(hPointPosition, PointPositions, 5 * sizeof(D3DXVECTOR4));

	if(hPointColor != NULL)
		Effect->SetValue(hPointColor, PointColors, 5 * sizeof(vector3df));

	if(hDirectionalNormal != NULL)
		Effect->SetValue(hDirectionalNormal, DirectionalPositions, 5 * sizeof(vector3df));

	if(hDirectionalColor != NULL)
		Effect->SetValue(hDirectionalColor, DirectionalColors, 5 * sizeof(vector3df));

}

void ShaderConstantSet::SetTextureStage(ITexture* nTextureStage, int Index)
{
	if(TextureStage[Index] != nTextureStage)
	{
		TextureStage[Index] = nTextureStage;
		if(hTextureStage[Index] != NULL)
		{
			Effect->SetTexture(hTextureStage[Index], ((irr::video::CD3D9Texture*)TextureStage[Index])->getDX9Texture());
			Effect->CommitChanges();
		}
	}
}

void ShaderConstantSet::Reset()
{
	World.makeIdentity();
	View.makeIdentity();
	Projection.makeIdentity();
	ViewInverse.makeIdentity();
	ViewProjection.makeIdentity();
	WorldViewProjection.makeIdentity();
	LightProjection.makeIdentity();
	CameraPosition.set(0,0,0);
	IsAnimated = false;
	Time = 0;
	Alpha = 0;
	PassC = SColorf(0, 0, 0, 0);
	FogColor = SColorf(0, 0, 0, 0);
	FogData.set(0, 0);
	LightAmbient = SColorf(0, 0, 0, 0);
	DirectionalColor[0] = SColorf(0, 0, 0, 0);
	DirectionalColor[1] = SColorf(0, 0, 0, 0);
	DirectionalColor[2] = SColorf(0, 0, 0, 0);
	PointColor[0] = SColorf(0, 0, 0, 0);
	PointColor[1] = SColorf(0, 0, 0, 0);
	PointColor[2] = SColorf(0, 0, 0, 0);
	PointColor[3] = SColorf(0, 0, 0, 0);
	PointColor[4] = SColorf(0, 0, 0, 0);
	DirectionalNormal[0].set(0, 0, 0);
	DirectionalNormal[1].set(0, 0, 0);
	DirectionalNormal[2].set(0, 0, 0);
	PointPosition[0].set(0, 0, 0);
	PointPosition[1].set(0, 0, 0);
	PointPosition[2].set(0, 0, 0);
	PointPosition[3].set(0, 0, 0);
	PointPosition[4].set(0, 0, 0);
	PointRadius[0] = 0;
	PointRadius[1] = 0;
	PointRadius[2] = 0;
	PointRadius[3] = 0;
	PointRadius[4] = 0;
	ShadowMap = 0;
	ReflectionMap = 0;
	TextureTransform[0].makeIdentity();
	TextureTransform[1].makeIdentity();
	TextureTransform[2].makeIdentity();
	TextureTransform[3].makeIdentity();
	TextureStage[0] = 0;
	TextureStage[1] = 0;
	TextureStage[2] = 0;
	TextureStage[3] = 0;
	FrameBuffer = 0;
	ViewPort.set(640,480);
}

void ShaderConstantSet::SetSceneNodeConstants(irr::scene::ISceneNode* Node)
{
	for(int i = 0; i < Node->EffectParameters.size(); ++i)
	{
		irr::scene::SEffectParameter P = Node->EffectParameters[i];
	
		D3DXHANDLE Handle = Effect->GetParameterBySemantic(NULL, P.Name.c_str());
		if(Handle == NULL)
			Handle = Effect->GetParameterByName(NULL, P.Name.c_str());
		if(Handle == NULL)
			continue;

		switch(P.Type)
		{
		case irr::scene::EPT_Float:
			Effect->SetFloat(Handle, ((float*)P.Value)[0]);
			break;
		case irr::scene::EPT_Float2:
			Effect->SetFloatArray(Handle, ((float*)P.Value), 2);
			break;
		case irr::scene::EPT_Float3:
			Effect->SetFloatArray(Handle, ((float*)P.Value), 3);
			break;
		case irr::scene::EPT_Float4:
			Effect->SetFloatArray(Handle, ((float*)P.Value), 4);
			break;
		}
	}

	Effect->CommitChanges();
}

bool ShaderConstantSet::SetTechnique(int rtIndex)
{
	if(rtIndex == 0)
	{
		if(TechniqueRT0 == NULL)
		{
			if(TechniqueDefault == NULL)
				return false;
			
			Effect->SetTechnique(TechniqueDefault);
		}else
		{
			Effect->SetTechnique(TechniqueRT0);
		}

		return true;

	}else if(rtIndex == 1)
	{
		if(TechniqueRT1 == NULL)
			return false;

		Effect->SetTechnique(TechniqueRT1);
		return true;
	}else if(rtIndex == 2)
	{
		if(TechniqueRT2 == NULL)
			return false;

		Effect->SetTechnique(TechniqueRT2);
		return true;
	}

	return false;
}

ShaderConstantSet::ShaderConstantSet(ID3DXEffect* nEffect, const char* path)
	: TechniqueRT0(NULL), TechniqueRT1(NULL), TechniqueRT2(NULL)
{
	Effect = nEffect;
	Path = path;

	// Search Techniques to find first and second passes
	D3DXHANDLE T = NULL;
	Effect->FindNextValidTechnique(NULL, &T);
	TechniqueDefault = T;

	while(T != NULL)
	{
		D3DXTECHNIQUE_DESC Desc;
		Effect->GetTechniqueDesc(T, &Desc);

		std::string name = Desc.Name;
		if(name.size() >= 3)
		{
			if(name.substr(0, 3) == std::string("RT0"))
				TechniqueRT0 = TechniqueDefault = T;
			else if(name.substr(0, 3) == std::string("RT1"))
				TechniqueRT1 = T;
			else if(name.substr(0, 3) == std::string("RT2"))
				TechniqueRT2 = T;
		}

		Effect->FindNextValidTechnique(T, &T);
	}

	// Set technique
	Effect->SetTechnique(TechniqueDefault);

	HSem(hWorld, "World");
	HSem(hView, "View");
	HSem(hProjection, "Projection");
	HSem(hViewInverse, "ViewInverse");
	HSem(hViewProjection, "ViewProjection");
	HSem(hWorldViewProjection, "WorldViewProjection");
	HSem(hLightProjection, "LightProjection");
	HSem(hCameraPosition, "CameraPosition");
	HSem(hBones, "Bones");
	HSem(hIsAnimated, "IsAnimated");
	HSem(hTime, "Time");
	HSem(hAlpha, "Alpha");
	HSem(hPassC, "PassC");
	HSem(hFogColor, "FogColor");
	HSem(hFogData, "FogData");
	HSem(hShadowMap, "ShadowMap");
	HSem(hReflectionMap, "ReflectionMap");
	HSem(hReflectionViewProj, "ReflectionViewProj");
	HSem(hTextureTransform[0], "TextureTransform0");
	HSem(hTextureTransform[1], "TextureTransform1");
	HSem(hTextureTransform[2], "TextureTransform2");
	HSem(hTextureTransform[3], "TextureTransform3");
	HSem(hTextureStage[0], "TextureStage0");
	HSem(hTextureStage[1], "TextureStage1");
	HSem(hTextureStage[2], "TextureStage2");
	HSem(hTextureStage[3], "TextureStage3");
	HSem(hLightAmbient, "LightAmbient");
	HSem(hDirectionalColor, "DirectionalColor");
	HSem(hDirectionalNormal, "DirectionalNormal");
	HSem(hPointColor, "PointColor");
	HSem(hPointPosition, "PointPosition");
	HSem(hFrameBuffer, "FrameBuffer");
	HSem(hFrameBuffer2, "FrameBuffer2");
	HSem(hViewPort, "ViewPort");

	ResetParameters();
}

ShaderConstantSet::~ShaderConstantSet()
{
	Effect->SetTechnique(NULL);
	Effect->Release();
}


