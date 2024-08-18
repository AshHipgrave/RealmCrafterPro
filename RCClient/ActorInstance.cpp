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
#include "ActorInstance.h"
#include "LightFunctionList.h"

ActorInstance::ActorInstance()
{
	memset(reinterpret_cast<char*>(this) + 4, 0, sizeof(ActorInstance) - 4);
	//ClearMemory(ActorInstance);
	ActorInstanceList.Add(this);
}

ActorInstance::~ActorInstance()
{
	ActorInstanceList.Remove(this);
}

NGin::List<ActorInstance*> ActorInstance::ActorInstanceList;
NGin::List<ActorInstance*> ActorInstance::ActorInstanceDelete;

void ActorInstance::Delete(ActorInstance *Item)
{
	if(!ActorInstanceDelete.Find(Item))
	{
		ActorInstanceDelete.Add(Item);
	}
}

void ActorInstance::Clean()
{
	foreachc(dLIt, ActorInstance, ActorInstanceDelete)	
	{
		delete *dLIt;
		//ActorInstanceList.Remove(*dLIt);
		nextc(dLIt, ActorInstance, ActorInstanceDelete)
	}
	ActorInstance::ActorInstanceDelete.Clear();
}

ActorInstance* RuntimeIDList[65535];
int LastRuntimeID = 0;

void ActorInstance::UpdateShaderParameters()
{
	for(int i = 0 ; i < ShaderParameters.size(); ++i)
	{
		ScriptShaderParameter* Param = ShaderParameters[i];

		if(Param->ParameterType == SSPT_Time
			|| Param->ParameterType == SSPT_TimeReset
			|| Param->ParameterType == SSPT_TimeLoop)
		{
			float Time = (float)(MilliSecs() - Param->Time);
			Time *= 0.001f;

			if(Param->ParameterType == SSPT_TimeLoop)
			{
				if(Time > Param->Data.X)
				{
					Time = 0.0f;
					Param->Time = MilliSecs();
				}
			}

			if(Param->ParameterType == SSPT_TimeReset)
			{
				if(Time > Param->Data.X)
				{
					Time = Param->Data.Y;
					Param->Time = MilliSecs();
				}
			}

			EntityConstantFloat(EN, Param->Name.c_str(), Time);
		}else if(Param->ParameterType == SSPT_Float)
		{
			EntityConstantFloat(EN, Param->Name.c_str(), Param->Data.X);
		}else if(Param->ParameterType == SSPT_Float2)
		{
			EntityConstantFloat2(EN, Param->Name.c_str(), Param->Data.X, Param->Data.Y);
		}else if(Param->ParameterType == SSPT_Float3)
		{
			EntityConstantFloat3(EN, Param->Name.c_str(), Param->Data.X, Param->Data.Y, Param->Data.Z);
		}else if(Param->ParameterType == SSPT_Float4)
		{
			EntityConstantFloat4(EN, Param->Name.c_str(), Param->Data.X, Param->Data.Y, Param->Data.Z, Param->Data.W);
		}
	}
}

void ActorInstance::ShowGubbinSet(std::vector<GubbinTemplate*>* templates, std::vector<GubbinPreviewInstance*>& previews)
{
	HideGubbinSet(previews);

	if(templates == NULL)
		return;

	for(int tid = 0; tid < templates->size(); ++tid)
	{
		GubbinTemplate* T = (*templates)[tid];
		if(T == NULL)
			continue;

		GubbinActorTemplate* AT = NULL;

		for(int atid = 0; atid < T->ActorTemplates.size(); ++atid)
		{
			GubbinActorTemplate* tAT = T->ActorTemplates[atid];

			if (tAT->ActorID == this->Actor->ID && tAT->Gender == this->Gender)
			{
				AT = tAT;
				break;
			}
		}

		// No definition matches this actor
		if (AT == NULL)
			continue;

		// Create a preview instance, and set it up
		GubbinPreviewInstance* P = new GubbinPreviewInstance();
		P->ActorTemplate = AT;

		// Get bone attachment
		uint Bone = 0;
		if (EN != 0)
			Bone = FindChild(EN, AT->AssignedBoneName);

		// No bone, so avoid
		if (Bone == 0)
		{
			printf("Bone not found: %s\n", AT->AssignedBoneName.c_str());
			continue;
		}

		// Mesh
		if (AT->MeshID < 65535)
		{
			P->Mesh = GetMesh(AT->MeshID);

			if (P->Mesh != 0)
			{
				int Sequence = ExtractAnimSeq(P->Mesh, AT->AnimationStartFrame, AT->AnimationEndFrame);
				Animate(P->Mesh, 1, 1.0f, Sequence);

				// Should we use inherited animations? If so, its still animated anyway (above)
				if(AT->AnimationType != 0) // 1 = inherited
				{
					bbdx2_SetInheritAnimation(P->Mesh, 1);

					EntityParent(P->Mesh, EN, false);
				}else
				{
					EntityParent(P->Mesh, Bone, false);

					PositionEntity(P->Mesh, AT->Position.X, AT->Position.Y, AT->Position.Z);
					ScaleEntity(P->Mesh, AT->Scale.X, AT->Scale.Y, AT->Scale.Z);
					RotateEntity(P->Mesh, AT->Rotation.X, AT->Rotation.Y, AT->Rotation.Z);
				}
			}
		}

		// Emitter
		if (AT->Emitter.size() > 0)
		{
			// Load Config
			uint Config = RP_LoadEmitterConfig((string("Data\\Emitter Configs\\") + AT->Emitter + string(".rpc")).c_str(), 0, Cam);
			

			// Loaded successfully, create the emitter
			if(Config != 0)
			{
// 				int Tex = LoadTexture("Data\\Textures\\Particles\\DefaultTex.png");
// 				GrabTexture(Tex);
				int Tex = GetTexture(((RP_EmitterConfig*)Config)->DefaultTextureID, true);
				RP_ConfigTexture(Config, Tex, 1, 1, false);

				P->EmitterEN = RP_CreateEmitter(Config);
				//EntityParent(P->EmitterEN, Bone, false);

				// Transform
// 				PositionEntity(P->EmitterEN, AT->Position.X, AT->Position.Y, AT->Position.Z);
// 				ScaleEntity(P->EmitterEN, AT->Scale.X * 10.0f, AT->Scale.Y * 10.0f, AT->Scale.Z * 10.0f);
// 				RotateEntity(P->EmitterEN, AT->Rotation.X, AT->Rotation.Y, AT->Rotation.Z);
			}
		}

		// Light
		if (AT->UseLight)
		{
			P->Light = CreatePointLight();

			SetPLightColor(P->Light,
				(int)(AT->LightColor.X * 255.0f),
				(int)(AT->LightColor.Y * 255.0f),
				(int)(AT->LightColor.Z * 255.0f));
			SetLightRadius(P->Light, AT->LightRadius);
			SetLightPosition(P->Light, AT->Position.X, AT->Position.Y, AT->Position.Z);
		}

		// Add to list
		previews.push_back(P);
	}
}

void ActorInstance::HideGubbinSet(std::vector<GubbinPreviewInstance*>& previews)
{
	for(int i = 0; i < previews.size(); ++i)
	{
		GubbinPreviewInstance* P = previews[i];

		if (P->Mesh != 0)
		{
			FreeEntity(P->Mesh);
			P->Mesh = 0;
		}

		if (P->EmitterEN != 0)
		{
			RP_FreeEmitter(P->EmitterEN, true, true);
			P->EmitterEN = 0;
		}

		if (P->Light != 0)
		{
			FreePLight(P->Light);
			P->Light = 0;
		}

		delete P;
	}

	previews.clear();
}

void ActorInstance::UpdateGubbins()
{
	UpdateGubbinSet(GubbinENs);
	UpdateGubbinSet(HatENs);
	UpdateGubbinSet(ChestENs);
	UpdateGubbinSet(ShieldENs);
	UpdateGubbinSet(WeaponENs);
	UpdateGubbinSet(BeardENs);
}

void ActorInstance::UpdateGubbinSet(std::vector<GubbinPreviewInstance*>& previews)
{
	if(EN == 0)
		return;

	for(int i = 0; i < previews.size(); ++i)
	{
		GubbinPreviewInstance* P = previews[i];

		if(!P->ActorTemplate->UseLight)
			continue;
	
		if(P->ActorTemplate->AssignedBoneName.size() == 0)
			continue;

		uint Bone = FindChild(EN, P->ActorTemplate->AssignedBoneName);
		if(Bone == 0)
			continue;
		
		uint Piv = CreatePivot();
		EntityParent(Piv, Bone, false);
		PositionEntity(Piv, P->ActorTemplate->Position.X, P->ActorTemplate->Position.Y, P->ActorTemplate->Position.Z);
		ScaleEntity(Piv, P->ActorTemplate->Scale.X, P->ActorTemplate->Scale.Y, P->ActorTemplate->Scale.Z);
		RotateEntity(Piv, P->ActorTemplate->Rotation.X, P->ActorTemplate->Rotation.Y, P->ActorTemplate->Rotation.Z);

		if(P->Light != NULL)
		{
			SetLightPosition(P->Light, EntityX(Piv, true), EntityY(Piv, true), EntityZ(Piv, true));
		}

		if(P->EmitterEN != NULL)
		{
			PositionEntity(P->EmitterEN, EntityX(Piv, true), EntityY(Piv, true), EntityZ(Piv, true));
		}

		FreeEntity(Piv);

		if(P->ActorTemplate->LightFunction.size() > 0)
		{
			std::string FuncName = stringToLower(P->ActorTemplate->LightFunction);

			for(int i = 0; i < RealmCrafter::LightFunctionList::Functions.size(); ++i)
			{
				string TestNameLower = stringToLower(RealmCrafter::LightFunctionList::Functions[i]->Name);

				if(FuncName.compare(TestNameLower) == 0)
				{
					RealmCrafter::LightFunction* Fn = RealmCrafter::LightFunctionList::Functions[i];
					SetPLightColor(P->Light, Fn->CurrentColor.R * 255.0f, Fn->CurrentColor.G * 255.0f, Fn->CurrentColor.B * 255.0f);
					SetLightRadius(P->Light, Fn->CurrentRadius);
					break;
				}
			}
		}
	}
}

uint ActorInstance::GetEN()
{
	return EN;
}

uint ActorInstance::GetCollisionEN()
{
	return CollisionEN;
}

float ActorInstance::GetYaw()
{
	return Yaw;
}

void ActorInstance::SetYaw(float yaw)
{
	Yaw = yaw;
}

RealmCrafter::SectorVector ActorInstance::GetDestination()
{
	return Destination;
}

RealmCrafter::SectorVector ActorInstance::GetPosition()
{
	return Position;
}

bool ActorInstance::GetRunning()
{
	return IsRunning;
}

void ActorInstance::SetRunning(bool running)
{
	IsRunning = running;
}

RealmCrafter::SDK::IActorInstance* ActorInstance::GetMount()
{
	return Mount;
}

bool ActorInstance::GetWalkingBackward()
{
	return WalkingBackward;
}

void ActorInstance::SetWalkingBackward(bool backward)
{
	WalkingBackward = backward;
}

bool ActorInstance::GetStrafingLeft()
{
	return StrafingLeft;
}

void ActorInstance::SetStrafingLeft(bool strafing)
{
	StrafingLeft = strafing;
}

bool ActorInstance::GetStrafingRight()
{
	return StrafingRight;
}

void ActorInstance::SetStrafingRight(bool strafing)
{
	StrafingRight = strafing;
}

RealmCrafter::SAttributes* ActorInstance::GetAttributes()
{
	return Attributes;
}

// Sets the destination point for an actor instance
void ActorInstance::SetDestination(RealmCrafter::SectorVector &destination, bool turnMe)
{
	destination.FixValues();

	// Do not allow movement into water areas for walking characters
	int EType = Actor->Environment;
	if(Mount != 0)
		EType = Mount->Actor->Environment;

	if(EType == Environment_Walk)
	{
		foreachc(WIt, Water, WaterList)
		{
			Water* W = *WIt;

			if(Position.Y < EntityY(W->EN))
			{
				if(Destination.WithinSectorDimension(W->Position, (W->ScaleX * 0.5f), (W->ScaleZ * 0.5f)))
					return;
			}

			nextc(WIt, Water, WaterList);
		}
	}

	// Set destination
	Destination = destination;
	if(Mount != 0)
	{
		Mount->Destination = destination;
		Mount->IsRunning = IsRunning;
		Mount->WalkingBackward = WalkingBackward;
		Mount->StrafingLeft = StrafingLeft;
		Mount->StrafingRight = StrafingRight;
	}

	// if it is the local player, reset walking backwards and quit progress
	if(this == Me)
	{
		if(Me->Mount != 0)
		{
			//Me->Mount->WalkingBackward = false;
			//Me->Mount->StrafingLeft = false;
			//Me->Mount->StrafingRight = false;
		}
				
		//WalkingBackward = false;
		//StrafingLeft = false;
		//StrafingRight = false;
		RealmCrafter::Globals->QuitActive = false;

		if(turnMe)
		{
			// Transform destination coords into local coords for angle test
			int AISectorOffsetX = 0;
			int AISectorOffsetZ = 0;

			if(Me != NULL)
			{
				AISectorOffsetX = (int)destination.SectorX - (int)Me->Position.SectorX;
				AISectorOffsetZ = (int)destination.SectorZ - (int)Me->Position.SectorZ;
			}
			float AIX = destination.X + ((float)AISectorOffsetX * RealmCrafter::SectorVector::SectorSize);
			float AIZ = destination.Z + ((float)AISectorOffsetZ * RealmCrafter::SectorVector::SectorSize);

			float Angle = atan2(AIX - EntityX(Me->CollisionEN, true), AIZ - EntityZ(Me->CollisionEN, true)) * (180.0f / D3DX_PI);
			EntityYaw(Me->CollisionEN, Angle);
			Me->Yaw = Angle;
		}
	}
}
