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

#include "Actors3D.h"

string GubbinJoints[6] = {"Head", "Chest", "L_Shoulder", "R_Shoulder", "L_Forearm", "R_Forearm" };
char HideNametags = 0;
bool DisableCollisions = false;
int NametagFont;

void InvertScale(uint Child, uint Parent)
{
	float SetX = (1.0f / EntityScaleX(Parent, true)) * EntityScaleX(Child, false);
	float SetY = (1.0f / EntityScaleY(Parent, true)) * EntityScaleY(Child, false);
	float SetZ = (1.0f / EntityScaleZ(Parent, true)) * EntityScaleZ(Child, false);

	//ScaleEntity(Child, SetX, SetY, SetZ);
}

// Loads gubbin joint names from file
void LoadGubbinNames()
{
	FILE* F = ReadFile("Data\\Game Data\\Gubbins.dat");
	if(F == 0)
		RuntimeError("Could not read: Data\\Game Data\\Gubbins.dat");
	for(int i = 0; i < 6; ++i)
		GubbinJoints[i] = ReadString(F);
	CloseFile(F);
}

// Sets the weapon mesh
void SetActorWeapon(ActorInstance* AI, std::vector<GubbinTemplate*>* templates)
{
	AI->ShowGubbinSet(templates, AI->WeaponENs);
}


// Sets the shield mesh
void SetActorShield(ActorInstance* AI, std::vector<GubbinTemplate*>* templates)
{
	AI->ShowGubbinSet(templates, AI->ShieldENs);
}


// Sets the chest mesh
void SetActorChestArmour(ActorInstance* AI, std::vector<GubbinTemplate*>* templates)
{
	AI->ShowGubbinSet(templates, AI->ChestENs);
}


// Sets the hair/helmet mesh
void SetActorHat(ActorInstance* AI, std::vector<GubbinTemplate*>* templates)
{
	AI->HideGubbinSet(AI->HatENs);

	// No templates even set (unequip)
	if (templates == NULL)
	{
		if (AI->Gender == 0)
		{
			if(AI->Hair < AI->Actor->MaleHairs.size())
				AI->ShowGubbinSet(&AI->Actor->MaleHairs[AI->Hair], AI->HatENs);
			else
				AI->ShowGubbinSet(NULL, AI->HatENs);
		}
		else
		{
			if(AI->Hair < AI->Actor->FemaleHairs.size())
				AI->ShowGubbinSet(&AI->Actor->FemaleHairs[AI->Hair], AI->HatENs);
			else
				AI->ShowGubbinSet(NULL, AI->HatENs);
		}
	}else
		AI->ShowGubbinSet(templates, AI->HatENs);
}


// Loads the 3D stuff for an actor instance
bool LoadActorInstance3D(ActorInstance* A, float Scale, bool SkipAttachments, bool SkipCollisions)
{
	A->Actor->Radius = 0.0f;
	//A->CollisionEN = LoadMesh("Data\\Sphere.b3d");ScaleMesh(A->CollisionEN, 5, 5, 5);
	A->CollisionEN = CreatePivot();
	AnimSet* ActorAnimSet = 0;
	MeshMinMaxVertices* MMV = 0;

	// Main mesh and textures
	if(A->Gender == 0)
	{
		ActorAnimSet = AnimList[A->Actor->MAnimationSet];
		string Name = GetMeshName(A->Actor->MaleMesh);

		A->EN = GetMesh(A->Actor->MaleMesh);
		TagEntity(A->EN, "ACTOR");
		if(A->EN == 0)
		{
			FreeEntity(A->CollisionEN);
			return false;
		}

		// Meshes exported from Max have a pointless root pivot - get rid of it
		if(CountSurfaces(A->EN) < 1)
		{
			// This is a server field, being used as a flag to say that the mesh is a 3DS Max model.
			// I have avoided adding a new field for this to conserve server memory as both client
			// and server use the same Actor type definition.
			// It is used when attaching gubbins, as Max meshes have different joint rotations to
			// all other meshes.
			// -- CPP Port Update: Specific Server Side functions and members have been removed, ::TeamID
			//    has been preserved for compatibility.
			A->TeamID = 1;
			for(int i = 1; i < CountChildren(A->EN) + 1; ++i)
			{
				int EN = GetChild(A->EN, i);
				if(CountSurfaces(EN) > 0)
				{
					A->EN = EN;
					break;
				}
			}
		}

		// Shadow related
		EntityShadowLevel(A->EN, 3);
		if(GetEntityShadowShader(A->EN) == 0)
			EntityShadowShader(A->EN, DefaultAnimatedDepthShader);

		Scale *= LoadedMeshScales[A->Actor->MaleMesh] * A->Actor->Scale;
		EntityParent(A->EN, A->CollisionEN);
		MMV = Mesh_MinMaxVertices(A->EN);
		GetAnimatedBoundingBox(A->EN, &(MMV->MinX), &(MMV->MinY), &(MMV->MinZ), &(MMV->MaxX), &(MMV->MaxY), &(MMV->MaxZ));

		PositionEntity(A->EN, 0.0f, (MMV->MaxY - MMV->MinY) / -2.0f, 0.0f);
		int FaceTex = A->FaceTex;
		int BodyTex = A->BodyTex;

		ActorTextureSet FaceSet(65535, 65535, 65535, 65535);
		ActorTextureSet BodySet(65535, 65535, 65535, 65535);

		if(FaceTex < A->Actor->MaleFaceIDs.size())
			FaceSet = A->Actor->MaleFaceIDs[FaceTex];
		if(BodyTex < A->Actor->MaleBodyIDs.size())
			BodySet = A->Actor->MaleBodyIDs[BodyTex];

		if(CountSurfaces(A->EN) > 1)
		{
			bool SurfacesBackwards = false;
			string FirstSurfaceTex = stringToUpper(SurfaceTexture(A->EN, 0, 0));

			// If the name of the first surface contains the word 'HEAD', the surfaces are backwards
			if(FirstSurfaceTex.find("HEAD", 0) != -1)
			{
				SurfacesBackwards = true;
			}else if(stringToUpper(SurfaceTexture(A->EN, 1, 0)).find("HEAD", 0) == -1)
			{
				for(int i = 0; i < A->Actor->MaleFaceIDs.size(); ++i)
				{
					if(A->Actor->MaleFaceIDs[i].Tex0 < 65535)
					{
						string Name2 = stringToUpper(GetTextureName(A->Actor->MaleFaceIDs[i].Tex0));

						if(Name2 == FirstSurfaceTex)
						{
							SurfacesBackwards = true;
							break;
						}
					}
				}
			}

			// Perform the texturing
			if(SurfacesBackwards)
			{
				if(BodySet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex0), 0, 1);
				if(BodySet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex1), 1, 1);
				if(BodySet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex2), 2, 1);
				if(BodySet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex3), 3, 1);
				if(FaceSet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex0), 0, 0);
				if(FaceSet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex1), 1, 0);
				if(FaceSet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex2), 2, 0);
				if(FaceSet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex3), 3, 0);
			}else
			{
				if(BodySet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex0), 0, 0);
				if(BodySet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex1), 1, 0);
				if(BodySet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex2), 2, 0);
				if(BodySet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex3), 3, 0);
				if(FaceSet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex0), 0, 1);
				if(FaceSet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex1), 1, 1);
				if(FaceSet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex2), 2, 1);
				if(FaceSet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex3), 3, 1);
			}
		}else if(BodyTex != 0)
			EntityTexture(A->EN, BodyTex);
		
		// Beard
		if(A->Beard < A->Actor->Beards.size())
			A->ShowGubbinSet(&(A->Actor->Beards[A->Beard]), A->BeardENs);

		// Get radius
		float MaxLength = MeshWidth(A->EN);
		if(MeshDepth(A->EN) > MaxLength)
			MaxLength = MeshDepth(A->EN);
		A->Actor->Radius = (MaxLength * LoadedMeshScales[A->Actor->MaleMesh] * A->Actor->Scale) / 2.0f;
	}else
	{
		ActorAnimSet = AnimList[A->Actor->FAnimationSet];
		string Name = GetMeshName(A->Actor->FemaleMesh);

		A->EN = GetMesh(A->Actor->FemaleMesh);
		if(A->EN == 0)
		{
			FreeEntity(A->CollisionEN);
			return false;
		}

		// Meshes exported from Max have a pointless root pivot - get rid of it
		if(CountSurfaces(A->EN) < 1)
		{
			// This is a server field, being used as a flag to say that the mesh is a 3DS Max model.
			// I have avoided adding a new field for this to conserve server memory as both client
			// and server use the same Actor type definition.
			// It is used when attaching gubbins, as Max meshes have different joint rotations to
			// all other meshes.
			// -- CPP Port Update: Specific Server Side functions and members have been removed, ::TeamID
			//    has been preserved for compatibility.
			A->TeamID = 1;
			for(int i = 1; i < CountChildren(A->EN) + 1; ++i)
			{
				int EN = GetChild(A->EN, i);
				if(CountSurfaces(EN) > 0)
				{
					A->EN = EN;
					break;
				}
			}
		}

		Scale *= LoadedMeshScales[A->Actor->FemaleMesh] * A->Actor->Scale;
		EntityParent(A->EN, A->CollisionEN);
		MMV = Mesh_MinMaxVertices(A->EN);
		GetAnimatedBoundingBox(A->EN, &(MMV->MinX), &(MMV->MinY), &(MMV->MinZ), &(MMV->MaxX), &(MMV->MaxY), &(MMV->MaxZ));

		PositionEntity(A->EN, 0.0f, (MMV->MaxY - MMV->MinY) / -2.0f, 0.0f);
		int FaceTex = A->FaceTex;
		int BodyTex = A->BodyTex;

		ActorTextureSet FaceSet(65535, 65535, 65535, 65535);
		ActorTextureSet BodySet(65535, 65535, 65535, 65535);

		if(FaceTex < A->Actor->FemaleFaceIDs.size())
			FaceSet = A->Actor->FemaleFaceIDs[FaceTex];
		if(BodyTex < A->Actor->FemaleBodyIDs.size())
			BodySet = A->Actor->FemaleBodyIDs[BodyTex];

		if(CountSurfaces(A->EN) > 1)
		{
			bool SurfacesBackwards = false;
			string FirstSurfaceTex = stringToUpper(SurfaceTexture(A->EN, 0, 0));

			// If the name of the first surface contains the word 'HEAD', the surfaces are backwards
			if(FirstSurfaceTex.find("HEAD", 0) != -1)
			{
				SurfacesBackwards = true;
			}else if(stringToUpper(SurfaceTexture(A->EN, 1, 0)).find("HEAD", 0) == -1)
			{
				for(int i = 0; i < A->Actor->FemaleFaceIDs.size(); ++i)
				{
					if(A->Actor->FemaleFaceIDs[i].Tex0 < 65535)
					{
						string Name2 = stringToUpper(GetTextureName(A->Actor->FemaleFaceIDs[i].Tex0));

						if(Name2 == FirstSurfaceTex)
						{
							SurfacesBackwards = true;
							break;
						}
					}
				}
			}

			// Perform the texturing
			if(SurfacesBackwards)
			{
				if(BodySet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex0), 0, 1);
				if(BodySet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex1), 1, 1);
				if(BodySet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex2), 2, 1);
				if(BodySet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex3), 3, 1);
				if(FaceSet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex0), 0, 0);
				if(FaceSet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex1), 1, 0);
				if(FaceSet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex2), 2, 0);
				if(FaceSet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex3), 3, 0);
			}else
			{
				if(BodySet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex0), 0, 0);
				if(BodySet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex1), 1, 0);
				if(BodySet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex2), 2, 0);
				if(BodySet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(BodySet.Tex3), 3, 0);
				if(FaceSet.Tex0 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex0), 0, 1);
				if(FaceSet.Tex1 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex1), 1, 1);
				if(FaceSet.Tex2 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex2), 2, 1);
				if(FaceSet.Tex3 < 65535)
					EntityTexture(A->EN, GetTexture(FaceSet.Tex3), 3, 1);
			}
		}else if(BodyTex != 0)
			EntityTexture(A->EN, BodyTex);

		// Get radius
		float MaxLength = MeshWidth(A->EN);
		if(MeshDepth(A->EN) > MaxLength)
			MaxLength = MeshDepth(A->EN);
		A->Actor->Radius = (MaxLength * LoadedMeshScales[A->Actor->FemaleMesh] * A->Actor->Scale) / 2.0f;
	}

	// Animations
	EntityShader(A->EN, Shader_Animated);
	if(ActorAnimSet != 0)
		for(int i = 0; i < 150; ++i)
			if(ActorAnimSet->AnimEnd[i] > 0)
				A->AnimSeqs[i] = ExtractAnimSeq(A->EN, ActorAnimSet->AnimStart[i], ActorAnimSet->AnimEnd[i]);

	// Scale
	ScaleEntity(A->CollisionEN, Scale, Scale, Scale);

	if(SkipAttachments == false)
	{
		// Attached Emitters
		CreateEntityEmitters(A->EN);

		// Gubbins!
		ShowGubbins(A);

		// Hair
		SetActorHat(A, NULL);

		if(!SkipCollisions)
		{
			// Collision
			float MaxLength = MMV->MaxX - MMV->MinX;
			if(MMV->MaxZ - MMV->MinZ > MaxLength)
				MaxLength = ((MMV->MaxZ - MMV->MinZ) + MaxLength) / 2.0f;
			EntityRadius(A->CollisionEN, (MaxLength * Scale) / 2.0f, ((MMV->MaxY - MMV->MinY) * Scale) / 2.0f);
		
			if(A->Actor->PolyCollision == 0)
			{
				EntityType(A->CollisionEN, C_Actor);
			}
			else
			{		
				EntityType(A->CollisionEN, C_ActorTri1);
				EntityType(A->EN, C_ActorTri2);
			}
		}

		// Items
		UpdateActorItems(A);

		// Nametag
		if(HideNametags != 1)
			CreateActorNametag(A);

	}

	// Free MinMaxVertices
	MeshMinMaxVertices::Delete(MMV);
	MeshMinMaxVertices::Clean();

	// Type Handle
	NameEntity(A->CollisionEN, toString(Handle(A)));
	NameEntity(A->EN, toString(Handle(A)));

	return true;
}


// Textures an actor instance's nametag entity
void CreateActorNametag(ActorInstance* A)
{
	if(A->NametagEN != 0)
		GUIManager->Destroy(A->NametagEN);
	if(A->TagEN != 0)
		GUIManager->Destroy(A->TagEN);

	MeshMinMaxVertices* MMV = Mesh_MinMaxVertices(A->EN);

	A->NametagEN = GUIManager->CreateLabel("ActorNameTag::NameLabel", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(0.0f, 0.0f));
	A->NametagEN->Text = A->Name;
	
	if(A->Tag.length() > 0)
	{
		A->TagEN = GUIManager->CreateLabel("ActorNameTag::TagLabel", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(0, 0));
		A->TagEN->Text = string("<") + A->Tag + ">";
		A->TagEN->Visible = false;
		A->TagEN->SendToBack();
	}

	A->NametagEN->Visible = false;
	A->NametagEN->SendToBack();

	GUIManager->SetProperties("ActorNameTag");


	MeshMinMaxVertices::Delete(MMV);
	MeshMinMaxVertices::Clean();
}


// Frees the 3D stuff for an actor instance, but leaves the instance
void FreeActorInstance3D(ActorInstance* A)
{
	A->HideGubbinSet(A->GubbinENs);
	A->HideGubbinSet(A->HatENs);
	A->HideGubbinSet(A->ShieldENs);
	A->HideGubbinSet(A->ChestENs);
	A->HideGubbinSet(A->WeaponENs);
	A->HideGubbinSet(A->BeardENs);

	if(A->NametagEN != 0)
	{
		GUIManager->Destroy(A->NametagEN);
		A->NametagEN = 0;
	}

	if(A->TagEN != 0)
	{
		GUIManager->Destroy(A->TagEN);
		A->TagEN = 0;
	}

	if(A->EN != 0)
	{
		FreeEntityEmitters(A->EN);
		FreeEntity(A->EN);
		A->EN = 0;
	}

	for(int i = 0; i > A->FloatingNumbers.Size(); ++i)
	{
		delete A->FloatingNumbers[i];
	}
	A->FloatingNumbers.Empty();

	FreeEntity(A->CollisionEN);
	A->CollisionEN = 0;
}


// Frees an actor instance and the 3D as well
void SafeFreeActorInstance(ActorInstance* A)
{
	
	// Free actor instance
	if(Handle(A) == PlayerTarget)
		PlayerTarget = 0;

	FreeActorInstance3D(A);
	FreeActorInstance(A);
}


// Shows a gubbin
void ShowGubbins(ActorInstance* A)
{
	A->ShowGubbinSet(&(A->Actor->Gubbins), A->GubbinENs);
}

// Hides a gubbin
void HideGubbins(ActorInstance* A)
{
	A->HideGubbinSet(A->GubbinENs);
}

// Creates emitters on an entity's children based on names (RECURSIVE)
void CreateEntityEmitters(int E)
{
	for(int i = 1; i < CountChildren(E) + 1; ++i)
	{
		int CE = GetChild(E, i);
		CreateEntityEmitters(CE);
		string Name = EntityName(CE);
		if(Name.length() > 4)
		{
			if(stringToUpper(Name.substr(0, 2)).compare("E_") == 0)
			{
				Name = Name.substr(2);
				int Pos = Name.find("_", 0);
				if(Pos != -1)
				{
					string EmitterName = Name.substr(0, Pos - 1);
					int TextureID = toInt(Name.substr(Pos + 1));
					int Texture = GetTexture(TextureID);
					if(Texture != 0 && EmitterName.length() > 1)
					{
						int Config = RP_LoadEmitterConfig((string("Data\\Emitter Configs\\") + EmitterName + string(".rpc")).c_str(), Texture, Cam);
						if(Config != 0)
						{
							int EmitterEN = RP_CreateEmitter(Config);
							if(EmitterEN != 0)
								EntityParent(EmitterEN, CE, false);
							else
							{
								RP_FreeEmitterConfig(Config, false);
								UnloadTexture(TextureID);
							}
						}
					}
				}
			}
		}
	}
}

// Frees emitters attached to an entity (RECURSIVE)
void FreeEntityEmitters(int E)
{
	for(int i = 1; i < CountChildren(E) + 1; ++i)
	{
		int CE = GetChild(E, i);
		FreeEntityEmitters(CE);
		if(((RP_Emitter*)(toInt(EntityName(CE)))) != 0)
			RP_FreeEmitter(CE, true, false);
	}
}

// Updates an actor instance's 3D based on what is on their inventory
void UpdateActorItems(ActorInstance* A)
{
	// Items
	if(A->Inventory->Items[SlotI_Weapon] != 0)
		SetActorWeapon(A, &(A->Inventory->Items[SlotI_Weapon]->Item->GubbinTemplates));
	else
		SetActorWeapon(A, NULL);

	if(A->Inventory->Items[SlotI_Shield] != 0)
		SetActorShield(A, &(A->Inventory->Items[SlotI_Shield]->Item->GubbinTemplates));
	else
		SetActorShield(A, NULL);

	if(A->Inventory->Items[SlotI_Chest] != 0)
		SetActorChestArmour(A, &(A->Inventory->Items[SlotI_Chest]->Item->GubbinTemplates));
	else
		SetActorChestArmour(A, NULL);

	if(A->Inventory->Items[SlotI_Hat] != 0)
		SetActorHat(A, &(A->Inventory->Items[SlotI_Hat]->Item->GubbinTemplates));
	else
		SetActorHat(A, NULL);

	// Gubbins
// 	for(int i = 0; i < 6; ++i)
// 		HideGubbin(A, i);
// 
// 	for(int i = 0; i < SlotI_Backpack; ++i)
// 		if(A->Inventory->Items[i] != 0)
// 			for(int j = 0; j < 6; ++j)
// 				if(A->Inventory->Items[i]->Item->Gubbins[j] > 0)
// 					ShowGubbin(A, j);
}

// Plays an actor speech sound
void PlayActorSound(ActorInstance* A, int Speech)
{
	int Result = 65535;
	if(A->Gender == 0)
	{
		if(A->Actor->MSpeechIDs[Speech] < 65535)
			Result = A->Actor->MSpeechIDs[Speech];
	}else
	{
		if(A->Actor->FSpeechIDs[Speech] < 65535)
			Result = A->Actor->FSpeechIDs[Speech];
	}

	if(Result < 65535)
	{
		int EN = FindChild(A->EN, "Head");
		if(EN == 0)
			EN = A->EN;
		EmitSound(GetSound(Result), EN);
	}
}
