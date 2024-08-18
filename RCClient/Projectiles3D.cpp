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

#include "Projectiles3D.h"

ClassDef(ProjectileInstance, ProjectileInstanceList, ProjectileInstanceDelete);


// Creates a new projectile instance
void CreateProjectile(ActorInstance* Source, ActorInstance* Target, int MeshID, bool Homing, float Speed, string Emitter1, string Emitter2, int TexID1, int TexID2)
{
	// Create projectile
	ProjectileInstance* P = new ProjectileInstance();

	P->TexID1 = -1;
	P->TexID2 = -1;
	P->Speed = Speed * 2.0f;
	if(Homing)
		P->Target = Target;
	else
	{
		P->TargetX = EntityX(Target->CollisionEN);
		P->TargetY = EntityY(Target->CollisionEN);
		P->TargetZ = EntityZ(Target->CollisionEN);
	}

	// Create main mesh
	if(MeshID > -1 && MeshID < 65535)
	{
		P->EN = GetMesh(MeshID);
		if(P->EN != 0)
			ScaleEntity(P->EN, LoadedMeshScales[MeshID], LoadedMeshScales[MeshID], LoadedMeshScales[MeshID]);
	}
	if(P->EN == 0)
		P->EN = CreatePivot();

	// Create emitters
	if(Emitter1.length() != 0)
	{
		int Tex = GetTexture(TexID1);
		if(Tex != 0)
		{
			P->TexID1 = TexID1;
			int Config = RP_LoadEmitterConfig((string("Data\\Emitter Configs\\") + Emitter1 + string(".rpc")).c_str(), Tex, Cam);
			if(Config != 0)
			{
				P->EmitterEN1 = RP_CreateEmitter(Config);
				EntityParent(P->EmitterEN1, P->EN, false);
			}
		}
	}

	if(Emitter2.length() != 0)
	{
		int Tex = GetTexture(TexID2);
		if(Tex != 0)
		{
			P->TexID2 = TexID2;
			int Config = RP_LoadEmitterConfig((string("Data\\Emitter Configs\\") + Emitter2 + string(".rpc")).c_str(), Tex, Cam);
			if(Config != 0)
			{
				P->EmitterEN2 = RP_CreateEmitter(Config);
				EntityParent(P->EmitterEN2, P->EN, false);
			}
		}
	}

	// Initial position
	PositionEntity(P->EN, EntityX(Source->CollisionEN), EntityY(Source->CollisionEN), EntityZ(Source->CollisionEN));

}

// Updates all projectile instances
void UpdateProjectiles()
{
	foreachc(PIt, ProjectileInstance, ProjectileInstanceList)
	{
		ProjectileInstance* P = (*PIt);

		// Move
		if(P->Target != 0)
		{
			P->TargetX = EntityX(P->Target->CollisionEN);
			P->TargetY = EntityY(P->Target->CollisionEN);
			P->TargetZ = EntityZ(P->Target->CollisionEN);
		}
		PositionEntity(GPP, P->TargetX, P->TargetY, P->TargetZ);
		PointEntity(P->EN, GPP);
		MoveEntity(P->EN, 0, 0, P->Speed * Delta);

		// Destroy when close enough to target
		if(EntityDistance(P->EN, GPP) < 2.0f)
			FreeProjectileInstance(P);

		nextc(PIt, ProjectileInstance, ProjectileInstanceList);
	}
	ProjectileInstance::Clean();

}

// Frees a projectile instance
void FreeProjectileInstance(ProjectileInstance* P)
{
	if(P->TexID1 > -1)
		UnloadTexture(P->TexID1);
	if(P->TexID2 > -1)
		UnloadTexture(P->TexID2);
	
	if(P->EmitterEN1 != 0)
	{
		EntityParent(P->EmitterEN1, 0);
		RP_KillEmitter(P->EmitterEN1, true, false);
	}
	if(P->EmitterEN2 != 0)
	{
		EntityParent(P->EmitterEN2, 0);
		RP_KillEmitter(P->EmitterEN2, true, false);
	}
	FreeEntity(P->EN);
	ProjectileInstance::Delete(P);
	

}