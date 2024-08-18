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

#include "ClientCombat.h"

ClassDef(BloodSpurt, BloodSpurtList, BloodSpurtDelete);

//ClassDef(FloatingNumber, FloatingNumberList, FloatingNumberDelete);
FloatingNumber::FloatingNumber()
{
	EN = 0;
	AI = 0;
	Lifespan = 0.0f;
}

int DamageInfoStyle;

// Attacks target if the player is able to
void UpdateCombat()
{
	// If I have a human target and I'm not riding a mount
	if(PlayerTarget > 0 && Me->Attributes->Value[RealmCrafter::Globals->HealthStat] > 0 && RealmCrafter::Globals->AttackTarget && Me->Mount == 0)
	{
		ActorInstance* A = (ActorInstance*)PlayerTarget;

		bool isRanged = false;

		// Get Allowed Range
		float MaxRange = 4.0f;
		if(Me->Inventory->Items[SlotI_Weapon] != 0)
		{
			if(Me->Inventory->Items[SlotI_Weapon]->Item->WeaponType == W_Ranged)
			{
				if(Me->Inventory->Items[SlotI_Weapon]->ItemHealth > 0)
				{
					MaxRange = Me->Inventory->Items[SlotI_Weapon]->Item->Range + 2.0f;
				}

				isRanged = true;
			}
		}
		MaxRange *= MaxRange;

		// If its in range
		float Dist = pow((EntityX(A->CollisionEN, true) - EntityX(Me->CollisionEN, true)), 2.0f)
			+ pow((EntityY(A->CollisionEN, true) - EntityY(Me->CollisionEN, true)), 2.0f)
			+ pow((EntityZ(A->CollisionEN, true) - EntityZ(Me->CollisionEN, true)), 2.0f);

		//printf("UpdateCombat(); Dist=%f; Max=%f;\n", Dist, (MaxRange + ((A->Actor->Radius + Me->Actor->Radius) * 0.05f)));

		if(Dist < (MaxRange + ((A->Actor->Radius + Me->Actor->Radius) * 0.05f)))
		{
			// Stop moving
			Me->Destination = Me->Position;
			Me->Destination.X = EntityX(Me->CollisionEN);
			Me->Destination.Z = EntityZ(Me->CollisionEN);
			Me->Destination.FixValues();

			// Face Target N);
			PointEntity(Me->CollisionEN, A->CollisionEN);
			RotateEntity(Me->CollisionEN, 0.0f, EntityYaw(Me->CollisionEN) + 180.0f, 0.0f);

			// Attack if enough time elapsed
			if(MilliSecs() - RealmCrafter::Globals->LastAttack > RealmCrafter::Globals->CombatDelay)
			{
				// Tell server
				CRCE_Send(Connection, Connection, P_AttackActor, StrFromShort(A->RuntimeID), true);
				RealmCrafter::Globals->LastAttack = MilliSecs();
			}
		}

		// Chase it
 		if(Dist > MaxRange + ((A->Actor->Radius + Me->Actor->Radius) * 0.05f))
		{
			if(CurrentSeq(Me) < Anim_DefaultAttack || !Animating(Me->EN))
			{
 				Me->SetDestination(A->Position, true);
			}
		}

	}

	// Update blood spurts
	foreachc(It, BloodSpurt, BloodSpurtList)
	{
		BloodSpurt* B = (*It);

		if(MilliSecs() - B->Timer > 600)
		{
			RP_KillEmitter(B->EmitterEN, false, false);
			BloodSpurt::Delete(B);
		}

		nextc(It, BloodSpurt, BloodSpurtList);
	}
	BloodSpurt::Clean();

}

// Loads combat settings from file
void LoadCombat()
{
	FILE* F = ReadFile("Data\\Game Data\\Combat.dat");
	if(F == 0)
		RuntimeError("Could not open Data\\Game Data\\Combat.dat!");

	int CombatDelay = ReadShort(F);
	char DamageInfoStyle = ReadByte(F);
	CloseFile(F);

	int LastAttack = MilliSecs();

	// Replace blood texture IDs with RootParticles config handles
	foreachc(It, Actor, ActorList)
	{
		Actor* A = (*It);

		int Tex = GetTexture(A->BloodTexID, true);
		if(Tex > 0)
			A->BloodTexID = RP_LoadEmitterConfig("Data\\Emitter Configs\\Blood.rpc", Tex, Cam);
		else
			A->BloodTexID = 0;

		nextc(It, Actor, ActorList);
	}
}

// Plays an actor's attack animation
void AnimateActorAttack(ActorInstance* A)
{
	int Anim = 0;

	// Choose animation and play it
	if(A->Inventory->Items[SlotI_Weapon] == 0)
		Anim = Anim_DefaultAttack;
	else
		switch(A->Inventory->Items[SlotI_Weapon]->Item->WeaponType)
		{
		case W_OneHand:
			Anim = Anim_RightAttack;
			break;
		case W_TwoHand:
			Anim = Anim_TwoHandAttack;
			break;
		case W_Ranged:
			{
				AnimSet* AS;
				if(A->Gender == 0)
					AS = AnimList[A->Actor->MAnimationSet];
				else
					AS = AnimList[A->Actor->FAnimationSet];
				Anim = FindAnimation(AS, A->Inventory->Items[SlotI_Weapon]->Item->RangedAnimation);
				if(Anim == -1)
					Anim = Anim_RangedAttack;
				break;
			}
	}
	PlayAnimation(A, 3, 0.5f, Anim, false);
	
}

// Plays an actor's parry animation
void AnimateActorParry(ActorInstance* A)
{
	int Anim = 0;

	// Choose animation and play it
	if(A->Inventory->Items[SlotI_Shield] != 0)
		Anim = Anim_ShieldParry;
	else if(A->Inventory->Items[SlotI_Weapon] == 0)
		Anim = Anim_DefaultParry;
	else
		switch(A->Inventory->Items[SlotI_Weapon]->Item->WeaponType)
		{
		case W_OneHand:
			Anim = Anim_RightParry;
			break;
		case W_TwoHand:
			Anim = Anim_TwoHandParry;
			break;
		case W_Ranged:
			Anim = Anim_DefaultParry;
			break;
		}
	PlayAnimation(A, 3, 0.5f, Anim, false);
}

// Displays a combat damage message
void CombatDamageOutput(ActorInstance* AI, int Amount, string DType)
{
	string Spc = string(" ");
	// Chat message
	if(DamageInfoStyle == 2)
	{
		string Name = AI->Name;
		Name = trim(Name);
		if(Name.length() == 0)
			Name = AI->Actor->Race;

		// You hit him
		if(Amount > 0)
			Output(LanguageString[LS_YouHit] + Spc + Name + Spc + LanguageString[LS_For] + Spc + toString(Amount) + Spc + DType + Spc + LanguageString[LS_DamageWow], 0, 255, 0);
		else if(Amount < 0) // He hit you
			Output(Name + Spc + LanguageString[LS_HitsYou] + Spc + toString(-Amount) + Spc + DType + Spc + LanguageString[LS_DamageWow], 255, 0, 0);
		else // Miss
			if(DType == string("1")) // He missed
				Output(Name + Spc + LanguageString[LS_AttacksYouMisses], 0, 0, 255);
			else // You missed
				Output(LanguageString[LS_YouAttack] + Spc + Name + Spc + LanguageString[LS_AndMiss], 0, 0, 255);
	}else if(DamageInfoStyle == 3) // floating number
	{
		if(Amount < 0) // He hit you
			CreateFloatingNumber(Me, Amount, 255, 0, 0);
		else if(Amount > 0) // You hit him
			CreateFloatingNumber(AI, -Amount, 50, 255, 0);
	}
}

// Creates a floating number
void CreateFloatingNumber(ActorInstance* AI, int Amount, int R, int G, int B)
{
	FloatingNumber* F = new FloatingNumber();
	F->EN = GUIManager->CreateLabel("FloatingNumber", Math::Vector2(0.5, 0.5), Math::Vector2(0, 0));
	F->EN->Text = toString(Amount);
	F->EN->ForeColor = Math::Color(R, G, B);
	F->AI = AI;
	AI->FloatingNumbers.Add(F);
}

// Updates all floating numbers
bool UpdateFloatingNumber(FloatingNumber* F)
{
	if(F == 0)
		return false;

	F->Lifespan += Delta;
	if(F->Lifespan > 50.0f)
	{
		GUIManager->Destroy(F->EN);
		//FloatingNumber::Delete(F);
		return false;
	}

	float Alpha = 1.0f;
	if(F->Lifespan > 40.0f)
	{
		Alpha = (50.0f - F->Lifespan) / 10.0f;
	}

	// Find a head, if not, use the body
	uint Head = FindChild(F->AI->EN, "Head");
	if(Head == 0)
		Head = F->AI->EN;
	if(Head != 0)
	{
		float Px = EntityX(Head, true);
		float Py = EntityY(Head, true);
		float Pz = EntityZ(Head, true);

		float Cx = EntityX(Cam, true);
		float Cy = EntityY(Cam, true);
		float Cz = EntityZ(Cam, true);

		float Dx = Abs(Px - Cx);
		float Dy = Abs(Py - Cy);
		float Dz = Abs(Pz - Cz);

		float Distance = (Dx * Dx + Dy * Dy + Dz * Dz);

		if(Distance > 1600.0f)
		{
			if(Distance > 1600.0f)
				Distance = 1600.0f;
			Alpha *= (1600.0f - Distance) / 10.0f;
		}

		// BBDX will help us find a position!
		D3DXVECTOR3 HeadPos(EntityX(Head, true), EntityY(Head, true), EntityZ(Head, true));
		D3DXVECTOR3 ProjectedPos;

		bbdx2_ProjectVector3(&HeadPos, &ProjectedPos);

		// If its out of the screen, move it miles away
		if(ProjectedPos.z < 0.0f || ProjectedPos.z > 1.0f)
		{
			F->EN->Location = Math::Vector2(-1, -1);
		}else
		{
			float Width = F->EN->InternalWidth;

			F->EN->Location = Math::Vector2(ProjectedPos.x - (Width * 0.5f), ProjectedPos.y - ((F->Lifespan * 2.0f) / GUIManager->GetResolution().Y));

			Math::Color C = F->EN->ForeColor;
			C.A = Alpha;
			F->EN->ForeColor = C;

			F->EN->SendToBack();
		}
	}

	return true;
}
