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

#include "Environment.h"
#include <vector4.h>

using namespace NGin::Math;

CEnvironmentManager::CEnvironmentManager()
{
	Year = Day = TimeH = TimeM = 0;
	TimeFactor = 10;

	TimeUpdate = 0; 
}

void CEnvironmentManager::SetTime(int timeH, int timeM)
{
	TimeH = timeH;
	TimeM = timeM;
}

void CEnvironmentManager::SetDate(int day, int year)
{
	Day = day;
	Year = year;
}

void CEnvironmentManager::ResetEnvironment(NGin::CString& networkPacket)
{
	Year = networkPacket.GetRealInt(0);
	Day = networkPacket.GetRealShort(4);
	TimeH = networkPacket.GetRealChar(6);
	TimeM = networkPacket.GetRealChar(7);
	TimeFactor = (int)(unsigned char)networkPacket.GetRealChar(8);

	if(networkPacket.Length() > 9)
	{
		RealmCrafter::PacketReader Reader((char*)networkPacket.c_str() + 9, networkPacket.Length() - 9, true);
		ProcessEnvNetPacket(Reader);
	}
}
// 
// void CEnvironmentManager::SetWeather(int weather)
// {
// 	std::string WeatherName = "";
// 
// 	switch(weather)
// 	{
// 	case W_Sun:
// 		{
// 			WeatherName = "Sun";
// 
// 			break;
// 		}
// 	case W_Rain:
// 		{
// 			WeatherName = "Rain";
// 
// 			break;
// 		}
// 	case W_Snow:
// 		{
// 			WeatherName = "Snow";
// 
// 			break;
// 		}
// 	case W_Fog:
// 		{
// 			WeatherName = "Fog";
// 
// 			break;
// 		}
// 	case W_Storm:
// 		{
// 			WeatherName = "Storm";
// 
// 			break;
// 		}
// 	case W_Wind:
// 		{
// 			WeatherName = "Wind";
// 
// 			break;
// 		}
// 	}
// 	
// 	ProcessEnvNetProperty("Weather", WeatherName);
// }

void CEnvironmentManager::LoadArea(std::string zoneName, std::string environmentName)
{
	ChangeEnvironment(environmentName, zoneName);
}

int CEnvironmentManager::GetTimeH()
{
	return TimeH;
}

int CEnvironmentManager::GetTimeM()
{
	return TimeM;
}

int CEnvironmentManager::GetTimeS()
{
	int MDiff = MilliSecs() - TimeUpdate;
	int Fact = 60000 / TimeFactor;

	float SecondInterp = ((float)MDiff) / ((float)Fact);

	return (int)(SecondInterp * 60.0f);
}

void CEnvironmentManager::SetCameraUnderWater(bool under, unsigned char destR, unsigned char destG, unsigned char destB)
{
	SetEnvCameraUnderWater(under, destR, destG, destB);
}

void CEnvironmentManager::Update(float deltaTime)
{
	// Scripted Emitters
	foreachc(It, ScriptedEmitter, ScriptedEmitterList)
	{
		ScriptedEmitter* SEm = (*It);

		if(MilliSecs() - SEm->StartTime > SEm->Length)
		{
			RP_KillEmitter(SEm->EN, true, false);
			ScriptedEmitter::Delete(SEm);
		}

		nextc(It, ScriptedEmitter, ScriptedEmitterList);
	}
	ScriptedEmitter::Clean();

	bool MinuteChanged = false;
	bool HourChanged = false;

	static int TimeS = 0;
		
	int MDiff = MilliSecs() - TimeUpdate;
	int Fact = 60000 / TimeFactor;

	float SecondInterp = ((float)MDiff) / ((float)Fact);

	if(MDiff > Fact)
	{
		TimeUpdate = MilliSecs();
		++TimeM;
		SecondInterp = 0.0f;
		MinuteChanged = true;
		if(TimeM > 59)
		{
			++TimeH;
			TimeM = 0;
			HourChanged = true;
			if(TimeH > 23)
			{
				TimeH = 0;
				++Day;

				SetEnvDate(&Day, &Year);
			}
		}
	}

	SetEnvTime(TimeH, TimeM, SecondInterp);
	UpdateEnvironment(deltaTime);
}

// Removes all particles from a given emitter which are beneath a water area or a catch plane
void RemoveUnderwaterParticles(int E)
{
	RP_Emitter* Em = (RP_Emitter*)toInt(EntityName(E));
	if(Em != 0)
	{
		// Collide with water
		foreachc(WIt, Water, WaterList)
		{
			Water* W = (*WIt);

			float MinX = EntityX(W->EN) - (W->ScaleX / 2.0f);
			float MinZ = EntityZ(W->EN) - (W->ScaleZ / 2.0f);
			float MaxX = MinX + W->ScaleX;
			float MaxZ = MinZ + W->ScaleZ;
			float Y = EntityY(W->EN);
			foreachc(PIt, RP_Particle, RP_ParticleList)
			{
				RP_Particle* P = (*PIt);

				// Particle belongs to this emitter
				if(P->E == Em)
					if(P->Y < Y) // Below water level
						if(P->X > MinX && P->X < MaxX) // Inside water boundaries
							if(P->Z > MinZ && P->Z < MaxZ)
								P->TimeToLive = 0.0f; // Remove particle

				nextc(PIt, RP_Particle, RP_ParticleList);
			}

			nextc(WIt, Water, WaterList);
		}

		// Collide with catch planes
		foreachc(CIt, CatchPlane, CatchPlaneList)
		{
			CatchPlane* CP = (*CIt);

			int MinSectorOffsetX = 0;
			int MinSectorOffsetZ = 0;
			int MaxSectorOffsetX = 0;
			int MaxSectorOffsetZ = 0;

			if(Me != NULL)
			{
				MinSectorOffsetX = (int)CP->Min.SectorX - (int)Me->Position.SectorX;
				MinSectorOffsetZ = (int)CP->Min.SectorZ - (int)Me->Position.SectorZ;
				MaxSectorOffsetX = (int)CP->Max.SectorX - (int)Me->Position.SectorX;
				MaxSectorOffsetZ = (int)CP->Max.SectorZ - (int)Me->Position.SectorZ;
			}
			float MinX = CP->Min.X + ((float)MinSectorOffsetX * RealmCrafter::SectorVector::SectorSize);
			float MinZ = CP->Min.Z + ((float)MinSectorOffsetZ * RealmCrafter::SectorVector::SectorSize);
			float MaxX = CP->Max.X + ((float)MaxSectorOffsetX * RealmCrafter::SectorVector::SectorSize);
			float MaxZ = CP->Max.Z + ((float)MaxSectorOffsetZ * RealmCrafter::SectorVector::SectorSize);

			foreachc(PIt, RP_Particle, RP_ParticleList)
			{
				RP_Particle* P = (*PIt);

				// Particle belongs to this emitter
				if(P->E == Em)
					if(P->Y < CP->Y) // Below water level
						if(P->X > MinX && P->X < MaxX) // Inside water boundaries
							if(P->Z > MinZ && P->Z < MaxZ)
								P->TimeToLive = 0.0f; // Remove particle

				nextc(PIt, RP_Particle, RP_ParticleList);
			}
			nextc(CIt, CatchPlane, CatchPlaneList);
		}
	}
}

bool CEnvironmentManager::PlayWetFootstep()
{
	return EnvPlayWetFootstep();
}
