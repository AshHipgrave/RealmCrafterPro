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
#include "SShaderProfile.h"

#include <ArrayList.h>
ArrayList<SShaderProfile*> Profiles;

DLLPRE int GetParameterCount(int Effect)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	return ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size();
}

DLLPRE const char* GetParameterName(int Effect, int ID)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	if(ID < 0 || ID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size())
		return 0;

	return ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[ID]->Name.c_str();
}

DLLPRE int GetParameterType(int Effect, int ID)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	if(ID < 0 || ID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size())
		return 0;

	return (int)((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[ID]->Type;
}

DLLPRE int GetAnnotationCount(int Effect, int ID)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	if(ID < 0 || ID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size())
		return 0;

	return ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[ID]->Annotations.Size();
}

DLLPRE const char* GetAnnotationName(int Effect, int eID, int aID)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	if(eID < 0 || eID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size())
		return 0;

	if(aID < 0 || aID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations.Size())
		return 0;

	return ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations[aID]->Name.c_str();
}

DLLPRE int GetAnnotationType(int Effect, int eID, int aID)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	if(eID < 0 || eID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size())
		return 0;

	if(aID < 0 || aID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations.Size())
		return 0;

	return (int)((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations[aID]->Type;
}

DLLPRE int GetSizeOfType(int Type)
{
	return GetParameterSize((ShaderType)Type);
}

DLLPRE int GetParameterData(int Effect, int ID, void* Data)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	if(ID < 0 || ID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size())
		return 0;

	memcpy(Data, ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[ID]->Data, GetParameterSize(((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[ID]->Type));
	return GetParameterSize(((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[ID]->Type);
}


DLLPRE void* GetNodeParameterValue(ISceneNode* Node, const irr::c8* ParamName, int &outType)
{
	// Search in Node Parameters
	irr::core::stringc Name = ParamName;
	for(int i = 0; i < Node->EffectParameters.size(); ++i)
	{
		if(Node->EffectParameters[i].Name == Name)
		{
			outType = Node->EffectParameters[i].Type + ST_Float;
			return Node->EffectParameters[i].Value;
		}
	}

	// If not found, then search in FX parameters defaults
	u32 Fx = Node->getEffect();
	ShaderConstantSet* pFX = ((irr::video::CD3D9Driver*)driver)->EffectsList[Fx];
	NGin::CString GName = ParamName;

	for(int i = 0; i < pFX->Parameters.Size(); ++i)
	{
		if(pFX->Parameters[i]->Name == GName)
		{
			outType = pFX->Parameters[i]->Type;
			return pFX->Parameters[i]->Data;
		}
	}
	return NULL;
}

DLLPRE int GetNodeParameterCount(ISceneNode* Node)
{
	return Node->EffectParameters.size();
}

DLLPRE char * GetNodeParameterName(ISceneNode* Node, int iParam)
{
	if (iParam>=Node->EffectParameters.size()) return NULL;

	return (char*)Node->EffectParameters[iParam].Name.c_str();
}

DLLPRE void ResetNodeParameters( ISceneNode* Node )
{
	for(int i = 0; i < Node->EffectParameters.size(); ++i)
		delete Node->EffectParameters[i].Value;
	Node->EffectParameters.clear();
}



DLLPRE int GetAnnotationData(int Effect, int eID, int aID, void* Data)
{
	if(Effect < 1 || Effect >= ((irr::video::CD3D9Driver*)driver)->EffectsList.size())
		return 0;

	if(eID < 0 || eID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters.Size())
		return 0;

	if(aID < 0 || aID >= ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations.Size())
		return 0;

	ShaderParameter* A = ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations[aID];
	int CpySize = GetParameterSize(A->Type);


	if(A->Data == NULL || Data == NULL)
		return 0;

	memcpy(Data, A->Data, CpySize);

	return CpySize;
	

	//memcpy(Data, ((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations[aID]->Data, GetParameterSize(((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations[aID]->Type));
	//return GetParameterSize(((irr::video::CD3D9Driver*)driver)->EffectsList[Effect]->Parameters[eID]->Annotations[aID]->Type);
}

DLLPRE SShaderProfile* DLLEX LoadProfile(irr::c8* Filename)
{
	return 0;
}

DLLPRE SShaderProfile* DLLEX CreateProfile(irr::c8* Name)
{
	SShaderProfile* P = new SShaderProfile(Name);
	Profiles.Add(P);
	return P;
}

DLLPRE void DLLEX FreeProfile(SShaderProfile* Profile)
{
	if(Profile == 0)
		return;

	for(int i = 0; i < Profiles.Size(); ++i)
	{
		if(Profiles[i] == Profile)
		{
			Profiles.Remove(i);
			return;
		}
	}

	delete Profile;
}

DLLPRE void DLLEX SetProfileRange(SShaderProfile* Profile, float Range)
{
	if(Profile == 0)
		return;

	Profile->RangeScale = Range;
}

DLLPRE float DLLEX GetProfileRange(SShaderProfile* Profile)
{
	if(Profile == 0)
		return 0.0f;

	return Profile->RangeScale;
}

DLLPRE void DLLEX SetProfileEffect(SShaderProfile* Profile, int QualityLevel, int Distance, irr::u32 Effect)
{
	if(Profile == 0)
		return;

	if(QualityLevel == 2)
		if(Distance == 1)
			Profile->HighNear = Effect;
		else
			Profile->HighFar = Effect;
	else if(QualityLevel == 1)
		if(Distance == 1)
			Profile->MediumNear = Effect;
		else
			Profile->MediumFar = Effect;
	else
		if(Distance == 1)
			Profile->LowNear = Effect;
		else
			Profile->LowFar = Effect;
}

DLLPRE irr::u32 DLLEX GetProfileEffect(SShaderProfile* Profile, int QualityLevel, int Distance)
{
	if(Profile == 0)
		return 0 ;

	if(QualityLevel == 2)
		if(Distance == 1)
			return Profile->HighNear;
		else
			return Profile->HighFar;
	else if(QualityLevel == 1)
		if(Distance == 1)
			return Profile->MediumNear;
		else
			return Profile->MediumFar;
	else
		if(Distance == 1)
			return Profile->LowNear;
		else
			return Profile->LowFar;
}

DLLPRE void DLLEX EntityProfile(ISceneNode* Entity, SShaderProfile* Profile)
{
	if(Entity == 0)
		return;

	Entity->SetProfile(Profile);
}

DLLPRE void DLLEX SetQualityLevel(int QualityLevel)
{
	smgr->SetQualityLevel(QualityLevel);
}

DLLPRE int DLLEX GetQualityLevel()
{
	return smgr->GetQualityLevel();
}

irr::u32 SShaderProfile::GetEffect(irr::scene::ISceneNode* Node, irr::scene::ISceneManager* SceneManager)
{
	float DistSq = Node->getAbsolutePosition().getDistanceFromSQ(SceneManager->getActiveCamera()->getAbsolutePosition());
	float FarSq = SceneManager->getActiveCamera()->getFarValue();
	FarSq *= FarSq;

	return this->GetEffect(SceneManager->GetQualityLevel(), (1.0f / FarSq) * DistSq);
}

// Load a shader
DLLPRE irr::u32 DLLEX deterkis(irr::c8* Filename)
{
	// Load FX Shader!
	irr::u32 Shader = driver->loadFXShader(Filename);

	// Quick Debug
	if(DEBUGMODE && Shader == 0)
	{
			char out[128];
			sprintf(out,"Error - Could not load shader: %s", Filename);
			MessageBox(NULL, out, "BBDX Debugger", MB_ICONERROR | MB_OK);
	}

	return Shader;
}

// Apply shader
DLLPRE void DLLEX slobing(ISceneNode* Node, irr::u32 Shader)
{
	// Set it
	Node->setEffect(Shader);
}

DLLPRE void DLLEX EntityConstantFloat(ISceneNode* Node, const irr::c8* CName, float F0)
{
	irr::core::stringc Name = CName;
	float* Constant = (float*)malloc(4);
	Constant[0] = F0;
	Node->SetParameterValue(Name, EPT_Float, Constant);
}

DLLPRE void DLLEX EntityConstantFloat2(ISceneNode* Node, const irr::c8* CName, float F0, float F1)
{
	irr::core::stringc Name = CName;
	float* Constant = (float*)malloc(8);
	Constant[0] = F0;
	Constant[1] = F1;
	Node->SetParameterValue(Name, EPT_Float2, Constant);
}

DLLPRE void DLLEX EntityConstantFloat3(ISceneNode* Node, const irr::c8* CName, float F0, float F1, float F2)
{
	irr::core::stringc Name = CName;
	float* Constant = (float*)malloc(12);
	Constant[0] = F0;
	Constant[1] = F1;
	Constant[2] = F2;
	Node->SetParameterValue(Name, EPT_Float3, Constant);
}

DLLPRE void DLLEX EntityConstantFloat4(ISceneNode* Node, const irr::c8* CName, float F0, float F1, float F2, float F3)
{
	irr::core::stringc Name = CName;
	float* Constant = (float*)malloc(16);
	Constant[0] = F0;
	Constant[1] = F1;
	Constant[2] = F2;
	Constant[3] = F3;
	Node->SetParameterValue(Name, EPT_Float4, Constant);
}

DLLPRE void DLLEX GlobalShaderConstantFloat4(const char* Name, float V0, float V1, float V2, float V3)
{
	irr::video::CD3D9Driver* services = (irr::video::CD3D9Driver*)driver;

	float Params[] = { V0, V1, V2, V3 };

	// Setup basic constants and reset all effects
	for(int i = 1; i < services->EffectsList.size(); ++i)
	{
		ID3DXEffect* Effect = services->EffectsList[i]->GetEffect();

		D3DXHANDLE Handle = Effect->GetParameterByName(NULL, Name);
		if(Handle == NULL)
			Handle = Effect->GetParameterBySemantic(NULL, Name);
		if(Handle == NULL)
			continue;

		D3DXPARAMETER_DESC Desc;
		Effect->GetParameterDesc(Handle, &Desc);

		// Must be a float array
		if(Desc.Type != D3DXPT_FLOAT)
			continue;

		switch(Desc.Columns)
		{
		case 1:
			{
				Effect->SetFloat(Handle, V0);
				break;
			}
		case 2:
			{
				Effect->SetFloatArray(Handle, Params, 2);
				break;
			}
		case 3:
			{
				Effect->SetFloatArray(Handle, Params, 3);
				break;
			}
		case 4:
			{
				Effect->SetFloatArray(Handle, Params, 4);
				break;
			}
		}

	}
}




