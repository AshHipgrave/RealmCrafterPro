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

#include "RottParticles.h"


ClassDefDB(RP_Emitter, RP_EmitterList, RP_EmitterDelete);
ClassDef(RP_Particle, RP_ParticleList, RP_ParticleDelete);
ClassDef(RP_EmitterConfig, RP_EmitterConfigList, RP_EmitterConfigDelete);

#define ConfigSet(Data1, Data2) RP_EmitterConfig* C = (RP_EmitterConfig*)ID; if(C != 0){ C->Data1 = Data2; return true; }else{return false;}


// Updates all emitters
void RP_Update(float Delta)
{
	// Clean list
	RP_Emitter::Clean();

	// Update emitters
	foreachc(EIt, RP_Emitter, RP_EmitterList)
	{
		RP_Emitter* E = *EIt;

		// Move mesh to new handle position - leaving them at 0, 0, 0 works but may work against Blitz's entity culling
		PositionEntity(E->MeshEN, EntityX(E->EmitterEN, true), EntityY(E->EmitterEN, true), EntityZ(E->EmitterEN, true));

		// Only update emitter if(it's enabled
		if(E->Enabled == true)
		{
			// Spawn new particles
			if(E->KillMode == 0)
			{
				E->ToSpawn = Ceil(((float)E->Config->ParticlesPerFrame) * Delta);
			}else if(E->ActiveParticles == 0) // if(emitter is being killed, free it after all particles are dead
			{
				switch(E->KillMode)
				{
				case 1:
						RP_FreeEmitter(E->EmitterEN, false, false); break;
				case 2:
						RP_FreeEmitter(E->EmitterEN, true, true); break;
				case 3:
						RP_FreeEmitter(E->EmitterEN, true, false); break;
				case 4:
						RP_FreeEmitter(E->EmitterEN, false, true); break;
				}
			}
		
		}else
			if(E->ActiveParticles == 0) // if(it is disabled and has no particles, hide it
				HideEntity(E->MeshEN); 

		nextc(EIt, RP_Emitter, RP_EmitterList);
	}

	RP_Particle::Clean();

	// Update particles
	foreachc(PIt, RP_Particle, RP_ParticleList)
	{
		RP_Particle* P = *PIt;

		// Get surface for this particle
		uint Surf = GetSurface(P->E->MeshEN, 1);

		// if in use, update it
		if(P->InUse == true)
		{
			// Update particle texture frame if necessary
			if(P->TexChange > 1.0f)
			{
				P->TexChange = P->TexChange - Delta;
				if(P->TexChange <= 1.0f)
				{
					int Frame = P->TexFrame + 1;
					if(Frame > (P->E->Config->TexAcross * P->E->Config->TexDown) - 1)
						Frame = 0;
					RP_SetParticleFrame(P, Frame);
				}
			}

			// Adjust force
			switch(P->E->Config->ForceShaping)
			{
			case RP_Linear:
				{
					P->FX = P->FX + (P->E->Config->ForceModX * Delta);
					P->FY = P->FY + (P->E->Config->ForceModY * Delta);
					P->FZ = P->FZ + (P->E->Config->ForceModZ * Delta);
					break;
				}
			case RP_Spherical:
				{
					uint Temp = CreatePivot();
					RotateEntity(Temp, P->E->Config->ForceModX * Delta, P->E->Config->ForceModY * Delta, P->E->Config->ForceModZ * Delta);
					TFormVector(P->FX, P->FY, P->FZ, Temp, 0);
					P->FX = TFormedX();
					P->FY = TFormedY();
					P->FZ = TFormedZ();
					FreeEntity(Temp);
					break;
				}
			}

			// Adjust velocity
			TFormVector(P->FX, P->FY, P->FZ, P->E->EmitterEN, 0);
			P->VX = P->VX + (TFormedX() * Delta);
			P->VY = P->VY + (TFormedY() * Delta);
			P->VZ = P->VZ + (TFormedZ() * Delta);

			// Move
			P->X = P->X + (P->VX * Delta);
			P->Y = P->Y + (P->VY * Delta);
			P->Z = P->Z + (P->VZ * Delta);

			// Update scale
			P->Scale = P->Scale + (P->E->Config->ScaleChange * Delta);

			// Update colour
			P->R = P->R + (P->E->Config->RChange * Delta);
			P->G = P->G + (P->E->Config->GChange * Delta);
			P->B = P->B + (P->E->Config->BChange * Delta);

			if(P->R > 255.0)
				P->R -= 255.0;
			else if(P->R < 0.0)
				P->R += 255.0;

			if(P->G > 255.0)
				P->G -= 255.0;
			else if(P->G < 0.0)
				P->G += 255.0;

			if(P->B > 255.0)
				P->B -= 255.0;
			else if(P->B < 0.0)
				P->B += 255.0;

			// Update alpha
			P->A += (P->E->Config->AlphaChange * Delta);
			if(P->A <= 0.0f)
				P->TimeToLive = -1.0f;

			// Update vertex positions and colours
			RP_UpdateParticleVertices(P);

			// Count down lifespan
			P->TimeToLive = P->TimeToLive - Delta;

			// Lifespan is up
			if(P->TimeToLive < 0.0f)
			{
				--P->E->ActiveParticles;
				// Respawn
				if(P->E->ToSpawn > 0)
					RP_SpawnParticle(P);
				else // More particles not needed - turn invisible until required
				{
					P->InUse = false;
					VertexColor(Surf, P->FirstVertex, 0.0f, 0.0f, 0.0f, 0.0f);
					VertexColor(Surf, P->FirstVertex + 1, 0.0f, 0.0f, 0.0f, 0.0f);
					VertexColor(Surf, P->FirstVertex + 2, 0.0f, 0.0f, 0.0f, 0.0f);
					VertexColor(Surf, P->FirstVertex + 3, 0.0f, 0.0f, 0.0f, 0.0f);
				}
			}
		}else // if(it is not in use, and the emitter still needs to spawn some, then spawn it
			if(P->E->ToSpawn > 0)
				RP_SpawnParticle(P);
		
		nextc(PIt, RP_Particle, RP_ParticleList);
	}

	// Clean again
	RP_Emitter::Clean();
	foreachc(EEIt, RP_Emitter, RP_EmitterList)
	{
		RP_Emitter* E = *EEIt;

		// Only update emitter if it's enabled
		if(E->Enabled == true)
		{
			
			UpdateHardwareBuffers(E->MeshEN);
		}else
			if(E->ActiveParticles != 0)
				UpdateHardwareBuffers(E->MeshEN); 

		nextc(EEIt, RP_Emitter, RP_EmitterList);
	}


}

// Adds a new particle to an emitter
void RP_CreateParticle(RP_Emitter* E)
{
	uint Surf = GetSurface(E->MeshEN, 1);
	uint V1 = AddVertex(Surf, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
	uint V2 = AddVertex(Surf, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	uint V3 = AddVertex(Surf, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
	uint V4 = AddVertex(Surf, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f);
	VertexColor(Surf, V1, 255, 255, 255, 0.0f);
	VertexColor(Surf, V2, 255, 255, 255, 0.0f);
	VertexColor(Surf, V3, 255, 255, 255, 0.0f);
	VertexColor(Surf, V4, 255, 255, 255, 0.0f);

	AddTriangle(Surf, V1, V2, V3);
	AddTriangle(Surf, V1, V3, V4);

	RP_Particle* P = new RP_Particle();
	P->E = E;
	P->FirstVertex = V1;
	P->InUse = false;
}

// Spawns a particle
void RP_SpawnParticle(RP_Particle* P)
{
	uint Surf = GetSurface(P->E->MeshEN, 1);
	P->InUse = true;
	++P->E->ActiveParticles;

	// Initial forces
	P->FX = P->E->Config->ForceX;
	P->FY = P->E->Config->ForceY;
	P->FZ = P->E->Config->ForceZ;

	// Plain initial velocities
	float VX = P->E->Config->VelocityX + Rnd(-P->E->Config->VelocityRndX, P->E->Config->VelocityRndX);
	float VY = P->E->Config->VelocityY + Rnd(-P->E->Config->VelocityRndY, P->E->Config->VelocityRndY);
	float VZ = P->E->Config->VelocityZ + Rnd(-P->E->Config->VelocityRndZ, P->E->Config->VelocityRndZ);

	// Initial position (and changed velocities if(they are "shape based")
	switch(P->E->Config->Shape)
	{
	case RP_Sphere: // Sphere shape
		{
			// Position
			float Distance = Rnd(P->E->Config->MinRadius, P->E->Config->MaxRadius) * P->E->Scale;
			float Pitch = Rnd(-90.0f, 90.0f);
			float Yaw = Rnd(-180.0f, 180.0f);
			P->Y = Sin(Pitch) * Distance;
			float FDistance = Cos(Pitch) * Distance;
			P->X = Cos(Yaw) * FDistance;
			P->Z = Sin(Yaw) * FDistance;

			// Velocity
			if(P->E->Config->VShapeBased == RP_ShapeBased)
			{
				VX = Abs(VX) * Sgn(Cos(Yaw) * FDistance);
				VY = Abs(VY) * Sgn(Sin(Pitch) * Distance);
				VZ = Abs(VZ) * Sgn(Sin(Yaw) * FDistance);
			}else if(P->E->Config->VShapeBased == RP_HeavilyShapeBased)
			{
				VX = Abs(VX) * ((Cos(Yaw) * FDistance) / P->E->Config->MaxRadius);
				VY = Abs(VY) * ((Sin(Pitch) * Distance) / P->E->Config->MaxRadius);
				VZ = Abs(VZ) * ((Sin(Yaw) * FDistance) / P->E->Config->MaxRadius);
			}
			break;
		}	
	case RP_Cylinder: // Cylinder shape
		{
			// Position
			float Distance = Rnd(P->E->Config->MinRadius, P->E->Config->MaxRadius) * P->E->Scale;
			float Yaw = Rnd(-180.0f, 180.0f);
			float Height = Rnd((P->E->Config->Depth) / -2.0f, (P->E->Config->Depth) / 2.0f) * P->E->Scale;
			switch(P->E->Config->ShapeAxis)
			{
			case 1: // Cylinder lies along X axis
				{
					P->X = Height;
					P->Y = Cos(Yaw) * Distance;
					P->Z = Sin(Yaw) * Distance;
					break;
				}
			case 2: // Cylinder lies along Y axis
				{
					P->Y = Height;
					P->X = Cos(Yaw) * Distance;
					P->Z = Sin(Yaw) * Distance;
					break;
				}
			case 3: // Cylinder lies along Z axis
				{
					P->Z = Height;
					P->X = Cos(Yaw) * Distance;
					P->Y = Sin(Yaw) * Distance;
					break;
				}
			}

			// Velocity
			if(P->E->Config->VShapeBased == RP_ShapeBased)
			{
				switch(P->E->Config->ShapeAxis)
				{
				case 1: // Cylinder lies along X axis
					{
						VX = Abs(VX) * Sgn(Height);
						VY = Abs(VY) * Sgn(Cos(Yaw) * Distance);
						VZ = Abs(VZ) * Sgn(Sin(Yaw) * Distance);
						break;
					}
				case 2: // Cylinder lies along Y axis
					{
						VY = Abs(VY) * Sgn(Height);
						VX = Abs(VX) * Sgn(Cos(Yaw) * Distance);
						VZ = Abs(VZ) * Sgn(Sin(Yaw) * Distance);
						break;
					}
				case 3: // Cylinder lies along Z axis
					{
						VZ = Abs(VZ) * Sgn(Height);
						VX = Abs(VX) * Sgn(Cos(Yaw) * Distance);
						VY = Abs(VY) * Sgn(Sin(Yaw) * Distance);
						break;
					}
				}
			}else if(P->E->Config->VShapeBased == RP_HeavilyShapeBased)
			{
				switch(P->E->Config->ShapeAxis)
				{
				case 1: // Cylinder lies along X axis
					{
						VX = 0.0f;
						VY = Abs(VY) * ((Cos(Yaw) * Distance) / P->E->Config->MaxRadius);
						VZ = Abs(VZ) * ((Sin(Yaw) * Distance) / P->E->Config->MaxRadius);
						break;
					}
				case 2: // Cylinder lies along Y axis
					{
						VY = 0.0f;
						VX = Abs(VX) * ((Cos(Yaw) * Distance) / P->E->Config->MaxRadius);
						VZ = Abs(VZ) * ((Sin(Yaw) * Distance) / P->E->Config->MaxRadius);
						break;
					}
				case 3: // Cylinder lies along Z axis
					{
						VZ = 0.0f;
						VX = Abs(VX) * ((Cos(Yaw) * Distance) / P->E->Config->MaxRadius);
						VY = Abs(VY) * ((Sin(Yaw) * Distance) / P->E->Config->MaxRadius);
						break;
					}
				}
			}
		}
		
	case RP_Box: // Box shape
		{
			// Position
			P->X = Rnd(P->E->Config->Width / -2.0f, P->E->Config->Width / 2.0f) * P->E->Scale;
			P->Y = Rnd(P->E->Config->Height / -2.0f, P->E->Config->Height / 2.0f) * P->E->Scale;
			P->Z = Rnd(P->E->Config->Depth / -2.0f, P->E->Config->Depth / 2.0f) * P->E->Scale;

			// Velocity
			if(P->E->Config->VShapeBased == RP_ShapeBased)
			{
				VX = Abs(VX) * Sgn(P->X);
				VY = Abs(VY) * Sgn(P->Y);
				VZ = Abs(VZ) * Sgn(P->Z);
			}else if(P->E->Config->VShapeBased == RP_HeavilyShapeBased)
			{
				float X = P->X / (P->E->Config->Width / 2.0f);
				float Y = P->Y / (P->E->Config->Height / 2.0f);
				float Z = P->Z / (P->E->Config->Depth / 2.0f);
				if(Abs(X) > Abs(Y) && Abs(X) > Abs(Z))
				{
					VX = Abs(VX) * Sgn(X);
					VY = 0.0f;
					VZ = 0.0f;
				}else if(Abs(Y) > Abs(X) && Abs(Y) > Abs(Z))
				{
					VY = Abs(VY) * Sgn(Y);
					VX = 0.0f;
					VZ = 0.0f;
				}else
				{
					VZ = Abs(VZ) * Sgn(Z);
					VX = 0.0f;
					VY = 0.0f;
				}
			}
		}
	}

	// Transform positions and velocities to global space (so emitter shapes can be moved/rotated)
	TFormPoint(P->X, P->Y, P->Z, P->E->EmitterEN, 0);
	P->X = TFormedX();
	P->Y = TFormedY();
	P->Z = TFormedZ();
	TFormVector(VX, VY, VZ, P->E->EmitterEN, 0);
	P->VX = TFormedX();
	P->VY = TFormedY();
	P->VZ = TFormedZ();

	// Initial texture frame
	if(P->E->Config->RndStartFrame > 0)
		RP_SetParticleFrame(P, Rand(0, (P->E->Config->TexAcross * P->E->Config->TexDown) - 1));
	else
		RP_SetParticleFrame(P, 0);

	// Other bits
	P->Scale = P->E->Config->ScaleStart * P->E->Scale;
	P->R = P->E->Config->RStart;
	P->G = P->E->Config->GStart;
	P->B = P->E->Config->BStart;
	P->A = P->E->Config->AlphaStart;
	P->TimeToLive = P->E->Config->Lifespan;

	VertexColor(Surf, P->FirstVertex, P->R, P->G, P->B, P->A * 255.0f);
	VertexColor(Surf, P->FirstVertex + 1, P->R, P->G, P->B, P->A * 255.0f);
	VertexColor(Surf, P->FirstVertex + 2, P->R, P->G, P->B, P->A * 255.0f);
	VertexColor(Surf, P->FirstVertex + 3, P->R, P->G, P->B, P->A * 255.0f);

	--P->E->ToSpawn;
}

// Sets the texture frame for a particle
void RP_SetParticleFrame(RP_Particle* P, int Frame)
{
	int X = 0;
	if(P->E->Config->TexAcross > 0)
		X = Frame % P->E->Config->TexAcross;
	int Y = 0;
	if(Frame > P->E->Config->TexAcross - 1)
		if(P->E->Config->TexAcross > 1)
			Y = ((Frame - X) / (P->E->Config->TexAcross - 1)) - 1;
		else
			Y = Frame;

	float MinU = (1.0f / ((float)P->E->Config->TexAcross)) * ((float)X);
	float MaxU = (1.0f / ((float)P->E->Config->TexAcross)) * ((float)X + 1);
	float MinV = (1.0f / ((float)P->E->Config->TexDown)) * ((float)Y);
	float MaxV = (1.0f / ((float)P->E->Config->TexDown)) * ((float)Y + 1);

	uint Surf = GetSurface(P->E->MeshEN, 1);

	VertexTexCoords(Surf, P->FirstVertex, MinU, MaxV);
	VertexTexCoords(Surf, P->FirstVertex + 1, MinU, MinV);
	VertexTexCoords(Surf, P->FirstVertex + 2, MaxU, MinV);
	VertexTexCoords(Surf, P->FirstVertex + 3, MaxU, MaxV);

	P->TexFrame = Frame;
	P->TexChange = P->E->Config->TexAnimSpeed + 1;
}

// Updates particle vertices to new position (and makes each particle face camera);
void RP_UpdateParticleVertices(RP_Particle* P)
{
	uint Surf = GetSurface(P->E->MeshEN, 1);

	// Adjust for mesh position
	float PosX = P->X - EntityX(P->E->MeshEN, true);
	float PosY = P->Y - EntityY(P->E->MeshEN, true);
	float PosZ = P->Z - EntityZ(P->E->MeshEN, true);

	// Get the new position of each corner vertex
	TFormVector(-P->Scale, -P->Scale, 0.0f, P->E->Config->FaceEntity, 0);
	VertexCoords(Surf, P->FirstVertex, PosX + TFormedX(), PosY + TFormedY(), PosZ + TFormedZ());

	TFormVector(-P->Scale, P->Scale, 0.0f, P->E->Config->FaceEntity, 0);
	VertexCoords(Surf, P->FirstVertex + 1, PosX + TFormedX(), PosY + TFormedY(), PosZ + TFormedZ());

	TFormVector(P->Scale, P->Scale, 0.0f, P->E->Config->FaceEntity, 0);
	VertexCoords(Surf, P->FirstVertex + 2, PosX + TFormedX(), PosY + TFormedY(), PosZ + TFormedZ());

	TFormVector(P->Scale, -P->Scale, 0.0f, P->E->Config->FaceEntity, 0);
	VertexCoords(Surf, P->FirstVertex + 3, PosX + TFormedX(), PosY + TFormedY(), PosZ + TFormedZ());

	// Update vertex colours
	VertexColor(Surf, P->FirstVertex, P->R, P->G, P->B, P->A * 255.0f);
	VertexColor(Surf, P->FirstVertex + 1, P->R, P->G, P->B, P->A * 255.0f);
	VertexColor(Surf, P->FirstVertex + 2, P->R, P->G, P->B, P->A * 255.0f);
	VertexColor(Surf, P->FirstVertex + 3, P->R, P->G, P->B, P->A * 255.0f);
}

// Changes the particle lifespan of a config
bool RP_ConfigLifespan(uint ID, int Lifespan)
{
	// Set Config
	ConfigSet(Lifespan, Lifespan);
}

// Changes the particles generated per frame of a config
bool RP_ConfigSpawnRate(uint ID, int SpawnRate)
{
	// Set Config
	ConfigSet(ParticlesPerFrame, SpawnRate);
}

// Sets the initial velocites to "shape based", "heavily shape based", or "normal" for a config
bool RP_ConfigVelocityMode(uint ID, int Level)
{
	// Set Config
	ConfigSet(VShapeBased, Level);
}

// Changes the initial particle X-velocity of a config
bool RP_ConfigVelocityX(uint ID, float VelocityX)
{
	// Set Config
	ConfigSet(VelocityX, VelocityX);
}

// Changes the initial particle Y-velocity of a config
bool RP_ConfigVelocityY(uint ID, float VelocityY)
{
	// Set Config
	ConfigSet(VelocityY, VelocityY);
}

// Changes the initial particle Z-velocity of a config
bool RP_ConfigVelocityZ(uint ID, float VelocityZ)
{
	// Set Config
	ConfigSet(VelocityZ, VelocityZ);
}

// Changes the initial particle X-velocity randomness of a config
bool RP_ConfigVelocityRndX(uint ID, float VelocityX)
{
	// Set Config
	ConfigSet(VelocityRndX, VelocityX);
}

// Changes the initial particle Y-velocity randomness of a config
bool RP_ConfigVelocityRndY(uint ID, float VelocityY)
{
	// Set Config
	ConfigSet(VelocityRndY, VelocityY);
}

// Changes the initial particle Z-velocity randomness of a config
bool RP_ConfigVelocityRndZ(uint ID, float VelocityZ)
{
	// Set Config
	ConfigSet(VelocityRndZ, VelocityZ);
}

// Changes the particle delta X-velocity of a config
bool RP_ConfigForceX(uint ID, float VelocityX)
{
	// Set Config
	ConfigSet(ForceX, VelocityX);
}

// Changes the particle delta Y-velocity of a config
bool RP_ConfigForceY(uint ID, float VelocityY)
{
	// Set Config
	ConfigSet(ForceY, VelocityY);
}

// Changes the particle delta Z-velocity of a config
bool RP_ConfigForceZ(uint ID, float VelocityZ)
{
	// Set Config
	ConfigSet(ForceZ, VelocityZ);
}

// Changes the particle delta force shaping mode of a config
bool RP_ConfigForceModMode(uint ID, int Mode)
{
	// Set Config
	ConfigSet(ForceShaping, Mode);
}

// Changes the particle delta X-force of a config
bool RP_ConfigForceModX(uint ID, float ForceModX)
{
	// Set Config
	ConfigSet(ForceModX, ForceModX);
}

// Changes the particle delta Y-force of a config
bool RP_ConfigForceModY(uint ID, float ForceModY)
{
	// Set Config
	ConfigSet(ForceModY, ForceModY);
}

// Changes the particle delta X-force of a config
bool RP_ConfigForceModZ(uint ID, float ForceModZ)
{
	// Set Config
	ConfigSet(ForceModZ, ForceModZ);
}

// Changes the initial particle red colour of a config
bool RP_ConfigInitialRed(uint ID, int Cl)
{
	// Set Config
	ConfigSet(RStart, Cl);
}

// Changes the initial particle green colour of a config
bool RP_ConfigInitialGreen(uint ID, int Cl)
{
	// Set Config
	ConfigSet(GStart, Cl);
}

// Changes the initial particle blue colour of a config
bool RP_ConfigInitialBlue(uint ID, int Cl)
{
	// Set Config
	ConfigSet(BStart, Cl);
}

// Changes the delta particle red colour of a config
bool RP_ConfigRedChange(uint ID, float Cl)
{
	// Set Config
	ConfigSet(RChange, Cl);
}

// Changes the delta particle green colour of a config
bool RP_ConfigGreenChange(uint ID, float Cl)
{
	// Set Config
	ConfigSet(GChange, Cl);
}

// Changes the delta particle blue colour of a config
bool RP_ConfigBlueChange(uint ID, float Cl)
{
	// Set Config
	ConfigSet(BChange, Cl);
}

// Changes the initial particle alpha of a config
bool RP_ConfigInitialAlpha(uint ID, float Alpha)
{
	// Set Config
	ConfigSet(AlphaStart, Alpha);
}

// Changes the delta particle alpha of a config
bool RP_ConfigAlphaChange(uint ID, float Alpha)
{
	// Set Config
	ConfigSet(AlphaChange, Alpha);
}

// Changes the entity which particles face towards for a config
bool RP_ConfigFaceEntity(uint ID, uint Entity)
{
	// Set Config
	ConfigSet(FaceEntity, Entity);
}

// Changes the blend mode for a config
bool RP_ConfigBlendMode(uint ID, int Blend)
{
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		C->BlendMode = Blend;
		// for each emitter with this config, alter the blend mode
		foreachc(EIt, RP_Emitter, RP_EmitterList)
		{
			RP_Emitter* E = *EIt;

			if(E->Config == C)
			{
				switch(Blend)
				{
				case 1: EntityShader(E->MeshEN, Shader_FullbrightAlpha); break;
				case 2: EntityShader(E->MeshEN, Shader_FullbrightMultiply); break;
				case 3: EntityShader(E->MeshEN, Shader_FullbrightAdd); break;
				}
			}

			nextc(EIt, RP_Emitter, RP_EmitterList);
		}
		return true;
	}else
		return false;
}

// Changes the max number of particles for a config
bool RP_ConfigMaxParticles(uint ID, int MaxParticles)
{
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		int Difference = MaxParticles - C->MaxParticles;
		C->MaxParticles = MaxParticles;

		// For each emitter with this config, create/destroy particles as required
		foreachc(EIt, RP_Emitter, RP_EmitterList)
		{
			RP_Emitter* E = *EIt;

			if(E->Config == C)
			{
				// Add more
				if(Difference > 0)
					for(int i = 0; i < Difference; ++i)
						RP_CreateParticle(E);
				else if(Difference < 0) // Remove
				{
					ClearSurface(GetSurface(E->MeshEN, 1));
					for(int i = 0; i < C->MaxParticles; ++i)
						RP_CreateParticle(E);
				}
			}

			nextc(EIt, RP_Emitter, RP_EmitterList);
		}
		return true;
	}else
		return false;
}

// Changes the initial particle scale of a config
bool RP_ConfigInitialScale(uint ID, float Scale)
{
	// Set Config
	ConfigSet(ScaleStart, Scale);
}

// Changes the particle scale change of a config
bool RP_ConfigScaleChange(uint ID, float Scale)
{
	// Set Config
	ConfigSet(ScaleChange, Scale);
}

// Changes the texture of a config
bool RP_ConfigTexture(uint ID, uint Texture, int TilesX, int TilesY, bool FreePreviousTexture)
{
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		// Update textures
		if(FreePreviousTexture && C->Texture != 0)
			FreeTexture(C->Texture);
		C->Texture = Texture;
		C->TexAcross = TilesX;
		C->TexDown = TilesY;

		// For each emitter with this config, change the texture
		foreachc(EIt, RP_Emitter, RP_EmitterList)
		{
			RP_Emitter* E = *EIt;

			if(E->Config == C)
				EntityTexture(E->MeshEN, C->Texture);

			nextc(EIt, RP_Emitter, RP_EmitterList);
		}
		return true;
	}else
		return false;
}

// Changes the texture animation speed for a config
bool RP_ConfigTextureAnimSpeed(uint ID, int Speed)
{
	// Set Config
	ConfigSet(TexAnimSpeed, Speed);
}

// Switches config between random texture start frame and 0 texture start frame
bool RP_ConfigTextureRandomStartFrame(uint ID, int Flag)
{
	// Set Config
	ConfigSet(RndStartFrame, Flag);
}

// Sets an emitter config to a sphere shape (default);
bool RP_ConfigShapeSphere(uint ID, float MinRadius, float MaxRadius)
{
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		C->Shape = RP_Sphere;
		C->MinRadius = MinRadius;
		C->MaxRadius = MaxRadius;
		return true;
	}else
		return false;
}

// Sets an emitter config to a cylinder shape
bool RP_ConfigShapeCylinder(uint ID, float MinRadius, float MaxRadius, float Length, int Axis)
{
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		C->Shape = RP_Sphere;
		C->MinRadius = MinRadius;
		C->MaxRadius = MaxRadius;
		C->Depth = Length;
		C->ShapeAxis = Axis;
		return true;
	}else
		return false;
}

// Sets an emitter config to a box shape
bool RP_ConfigShapeBox(uint ID, float Width, float Height, float Depth)
{
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		C->Shape = RP_Sphere;
		C->Width = Width;
		C->Height = Height;
		C->Depth = Depth;
		return true;
	}else
		return false;
}

// Creates a new emitter configuration and returns its ID
uint RP_CreateEmitterConfig(int MaxParticles, int SpawnRate, uint FaceEntity, uint Texture, int TilesX, int TilesY, const char* Name )
{
	// Create config with default values
	RP_EmitterConfig* C = new RP_EmitterConfig();
	C->FaceEntity = FaceEntity;
	C->AlphaStart = 1.0f;
	C->Texture = Texture;
	C->TexAcross = TilesX;
	C->TexDown = TilesY;
	C->RndStartFrame = true;
	C->MaxParticles = MaxParticles;
	C->ParticlesPerFrame = SpawnRate;
	C->Lifespan = 100;
	C->BlendMode = 3;
	C->ScaleStart = 1.0f;
	C->VShapeBased = RP_Normal;
	C->ForceShaping = RP_Linear;
	C->RStart = 255;
	C->GStart = 255;
	C->BStart = 255;
	C->Shape = RP_Sphere;
	C->Name = Name;
	return Handle(C);
}

// Copies an emitter configuration exactly
uint  RP_CopyEmitterConfig(uint ID)
{
	
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		RP_EmitterConfig* C2 = new RP_EmitterConfig();
		C2->MaxParticles = C->MaxParticles;
		C2->ParticlesPerFrame = C->ParticlesPerFrame;
		C2->Texture = C->Texture;
		C2->TexAcross = C->TexAcross;
		C2->TexDown = C->TexDown;
		C2->RndStartFrame = C->RndStartFrame;
		C2->TexAnimSpeed = C->TexAnimSpeed;
		C2->VShapeBased = C->VShapeBased;
		C2->VelocityX = C->VelocityX;
		C2->VelocityY = C->VelocityY;
		C2->VelocityZ = C->VelocityZ;
		C2->VelocityRndX = C->VelocityRndX;
		C2->VelocityRndY = C->VelocityRndY;
		C2->VelocityRndZ = C->VelocityRndZ;
		C2->ForceX = C->ForceX;
		C2->ForceY = C->ForceY;
		C2->ForceZ = C->ForceZ;
		C2->ForceModX = C->ForceModX;
		C2->ForceModY = C->ForceModY;
		C2->ForceModZ = C->ForceModZ;
		C2->ForceShaping = C->ForceShaping;
		C2->ScaleStart = C->ScaleStart;
		C2->ScaleChange = C->ScaleChange;
		C2->Lifespan = C->Lifespan;
		C2->RStart = C->RStart;
		C2->GStart = C->GStart;
		C2->BStart = C->BStart;
		C2->RChange = C->RChange;
		C2->GChange = C->GChange;
		C2->BChange = C->BChange;
		C2->AlphaStart = C->AlphaStart;
		C2->AlphaChange = C->AlphaChange;
		C2->FaceEntity = C->FaceEntity;
		C2->BlendMode = C->BlendMode;
		C2->Shape = C->Shape;
		C2->MinRadius = C->MinRadius;
		C2->MaxRadius = C->MaxRadius;
		C2->Width = C->Width;
		C2->Height = C->Height;
		C2->Depth = C->Depth;
		C2->ShapeAxis = C->ShapeAxis;
		C2->DefaultTextureID = C->DefaultTextureID; // Realm Crafter specific
		C2->Name = "Copied Emitter";
		return Handle(C2);
	}
		return false;
}

// Frees an emitter configuration
bool RP_FreeEmitterConfig(uint ID, uint FreeTex)
{
	RP_EmitterConfig* C = (RP_EmitterConfig*)ID;
	if(C != 0)
	{
		if(FreeTex)
		{
			// Free texture, making sure that all isntances of this texture int he parciel system are set to 0
			foreachc(C2It, RP_EmitterConfig, RP_EmitterConfigList)
			{
				RP_EmitterConfig* C2 = *C2It;

				if(C2 != C && C2->Texture == C->Texture)
					C2->Texture = 0;

				nextc(C2It, RP_EmitterConfig, RP_EmitterConfigList);
			}
			FreeTexture(C->Texture);
			C->Texture = 0;
		}
		RP_EmitterConfig::Delete(C);
		return true;
	}else
		return false;
}

// Loads an emitter configuration from file
uint RP_LoadEmitterConfig(const char* File, uint Texture, uint FaceEntity)
{
//	if(Texture != 0)
//		bbdx2_SetTextureAlpha(Texture);

	FILE* F = ReadFile(File);
	if(F == 0)
		return 0;

	RP_EmitterConfig* C = new RP_EmitterConfig();
	C->Texture = Texture;
	C->FaceEntity = FaceEntity;

// 	for(int i = File.length() - 1; i >= 0; --i)
// 	{
// 		if(File.substr(i, 1).compare(".") == 0)
// 		{
// 			File = File.substr(0, i);
// 		}else if(File.substr(i, 1).compare("\\") == 0 || File.substr(i, 1).compare("/") == 0)
// 		{
// 			File = File.substr(i + 1);
// 			break;
// 		}
// 	}

	C->Name = File;

	C->MaxParticles = ReadInt(F);
	C->ParticlesPerFrame = ReadInt(F);
	C->TexAcross = ReadInt(F);
	C->TexDown = ReadInt(F);
	C->RndStartFrame = ReadInt(F);
	C->TexAnimSpeed = ReadInt(F);
	C->VShapeBased = ReadInt(F);
	C->VelocityX = ReadFloat(F);
	C->VelocityY = ReadFloat(F);
	C->VelocityZ = ReadFloat(F);
	C->VelocityRndX = ReadFloat(F);
	C->VelocityRndY = ReadFloat(F);
	C->VelocityRndZ = ReadFloat(F);
	C->ForceX = ReadFloat(F);
	C->ForceY = ReadFloat(F);
	C->ForceZ = ReadFloat(F);
	C->ScaleStart = ReadFloat(F);
	C->ScaleChange = ReadFloat(F);
	C->Lifespan = ReadInt(F);
	C->AlphaStart = ReadFloat(F);
	C->AlphaChange = ReadFloat(F);
	C->BlendMode = ReadInt(F);
	C->Shape = ReadInt(F);
	C->MinRadius = ReadFloat(F);
	C->MaxRadius = ReadFloat(F);
	C->Width = ReadFloat(F);
	C->Height = ReadFloat(F);
	C->Depth = ReadFloat(F);
	C->ShapeAxis = ReadInt(F);
	C->DefaultTextureID = ReadShort(F); // Realm Crafter specific
	C->ForceModX = ReadFloat(F);
	C->ForceModY = ReadFloat(F);
	C->ForceModZ = ReadFloat(F);
	C->ForceShaping = ReadInt(F);
	C->RStart = ReadByte(F);
	C->GStart = ReadByte(F);
	C->BStart = ReadByte(F);
	C->RChange = ReadFloat(F);
	C->GChange = ReadFloat(F);
	C->BChange = ReadFloat(F);

	CloseFile(F);

	return Handle(C);
}

// Creates a new emitter with a given configuration and returns the ID
uint RP_CreateEmitter(uint Configuration, float Scale)
{
	// Find config
	RP_EmitterConfig* C = (RP_EmitterConfig*)Configuration;
	if(C == 0)
		return false;

	// Create emitter
	RP_Emitter* E = new RP_Emitter();
	E->Enabled = true;
	E->Config = C;
	E->Scale = Scale;
	E->EmitterEN = CreatePivot();
	NameEntity(E->EmitterEN, toString(Handle(E)));
	E->MeshEN = CreateMesh();
	std::string Tag = "Emitter: ";
	Tag += C->Name;

	TagEntity(E->MeshEN, Tag);
	CreateSurface(E->MeshEN);
	EntityFX(E->MeshEN, 1 + 2 + 8 + 32 + 64);
	EntityTexture(E->MeshEN, E->Config->Texture, 0, 0);

	switch(E->Config->BlendMode)
	{
	case 1: EntityShader(E->MeshEN, Shader_FullbrightAlpha); break;
	case 2: EntityShader(E->MeshEN, Shader_FullbrightMultiply); break;
	case 3: EntityShader(E->MeshEN, Shader_FullbrightAdd); break;
	}

	// Create initial particles
	for(int i = 0; i < C->MaxParticles; ++i)
		RP_CreateParticle(E);

	UpdateHardwareBuffers(E->MeshEN);

	// return entity
	return E->EmitterEN;
}

// returns how many particles are current alive in an emitter
int RP_EmitterActiveParticles(uint ID)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		return E->ActiveParticles;
	}else
		return -1;
}

// Enables an emitter
bool RP_EnableEmitter(uint ID)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		E->Enabled = true;
		ShowEntity(E->MeshEN);
		return true;
	}else
		return false;
}

// Disables an emitter
bool RP_DisableEmitter(uint ID)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		E->Enabled = false;
		return true;
	}else
		return false;
}

// Hides an emitter
bool RP_HideEmitter(uint ID)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		HideEntity(E->MeshEN);
		return true;
	}else
		return false;
}

// Shows an emitter
bool RP_ShowEmitter(uint ID)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		ShowEntity(E->MeshEN);
		return true;
	}else
		return false;
}

// Scales an emitter
bool RP_ScaleEmitter(uint ID, float Scale)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		E->Scale = Scale;
		return true;
	}else
		return false;
}

// Frees an emitter only after all existing particles have finished their lives
bool RP_KillEmitter(uint ID, bool FreeConfig, bool FreeTex)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	std::string Name = EntityName(ID);
	DebugLog(Name);
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		E->KillMode = 1;
		if(FreeConfig)
			if(FreeTex)
				E->KillMode = 2;
			else
				E->KillMode = 3;
		else if(FreeTex)
			E->KillMode = 4;
		return true;
	}else
		return false;
}

// Frees an emitter
bool RP_FreeEmitter(uint ID, bool FreeConfig, bool FreeTex)
{
	int Ptr = 0;
	Ptr = toInt(EntityName(ID));
	RP_Emitter* E = (RP_Emitter*)Ptr;
	if(E != 0)
	{
		if(FreeConfig)
			RP_FreeEmitterConfig(Handle(E->Config), FreeTex);
		else if(FreeTex)
		{
			// Free texture, making sure that all the instances of this texture in the particle system are set to o
			foreachc(CIt, RP_EmitterConfig, RP_EmitterConfigList)
			{
				RP_EmitterConfig* C = *CIt;

				if(C != E->Config && C->Texture == E->Config->Texture)
					C->Texture = 0;

				nextc(CIt, RP_EmitterConfig, RP_EmitterConfigList);
			}
		}
		foreachc(PIt, RP_Particle, RP_ParticleList)
		{
			RP_Particle* P = *PIt;

			if(P->E == E)
				RP_Particle::Delete(P);

			nextc(PIt, RP_Particle, RP_ParticleList);
		}
		RP_Particle::Clean();

		FreeEntity(E->MeshEN);
		FreeEntity(E->EmitterEN);
		RP_Emitter::Delete(E);
		return true;
	}else
		return false;
}

// Frees every emitter, particle and configuration
void RP_Clear(bool Configs, bool Textures)
{
	foreachc(EIt, RP_Emitter, RP_EmitterList)
	{
		RP_Emitter* E = *EIt;

		RP_FreeEmitter(E->EmitterEN, Configs, Textures);

		nextc(EIt, RP_Emitter, RP_EmitterList);
	}
}
