//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#include "CDefaultEnvironment.h"

// Remove this
#include <windows.h>
#undef PlaySound

using namespace std;
using namespace NGin;
using namespace NGin::Math;

#define W_Sun   0
#define W_Rain  1
#define W_Snow  2
#define W_Fog   3
#define W_Storm 4
#define W_Wind  5

namespace RealmCrafter
{
	CDefaultEnvironment::CDefaultEnvironment(uint camera)
		: Visible(false)
	{
		Camera = camera;

		// Setup all the defaults
		SkyEN = CloudEN = 0;
		CurrentWeather = 0;

		CameraUnderwater = false;

		FogNearDest = FogFarDest = FogNearNow = FogFarNow = 0;

		CloudWeight = Vector3(1, 1, 1);
		CloudSunWeight = Vector3(1, 1, 1);
		CloudRainWeight = Vector3(0.6f, 0.6f, 0.6f);
		CloudStormWeight = Vector3(0.4f, 0.4f, 0.5f);
		CloudSnowWeight = Vector3(0.8f, 0.8f, 0.8f);
		CloudWindWeight = Vector3(1.0f, 1.0f, 1.0f);
		TargetWeight = Vector3(1, 1, 1);

		CloudAmount = 0.7f;
		CloudSunAmount = 0.7f;
		CloudRainAmount = 0.8f;
		CloudStormAmount = 0.999f;
		CloudFogAmount = 0.0f;
		CloudSnowAmount = 0.7f;
		CloudWindAmount = 0.5f;
		TargetAmount = 0.7f;

		TargetSpeedScale = 1.0f;
		CloudSpeedScale = 1.0f;
		CloudNormalSpeedScale = 1.0f;
		CloudWindSpeedScale = 1.8f;

		TargetRainVolume = 0.0f;
		TargetWindVolume = 0.0f;
		RainVolume = 0.0f;
		WindVolume = 0.0f;
		SwayTarget = 0.0f;
		SwayAmount = 0.0f;

		TargetEpsilon = 0.01f;

		RainEmitter = SnowEmitter = 0;

		LightningToDo = 0;
		Snd_Rain = Snd_Wind = SndC_Rain = SndC_Wind = 0;
		Snd_Thunder[0] = 0;
		Snd_Thunder[1] = 0;
		Snd_Thunder[2] = 0;

		CurrentSeason = Year = Day = TimeH = TimeM = 0;
		TimeFactor = 10;

		TimeUpdate = 0; 
		HourChanged = MinuteChanged = true;
		FogChange = true;

		SkyEN = LoadMesh("Data\\Meshes\\ScreenPlane.b3d");
		//	SkyEN = LoadMesh("Data\\Meshes\\Sky Sphere.b3d");//ScreenPlane.b3d");
		if(SkyEN == 0)
			RuntimeError("File not found: Data\\Meshes\\ScreenPlane.b3d");

		float XScale = 2.0f / (0.99f - -0.99f);
		float YScale = 2.0f / (1.0f - -1.0f);
		float ZScale = 2.0f / (1.0f - -1.0f);

		if(YScale < XScale) XScale = YScale;
		if(ZScale < XScale) XScale = ZScale;
		ScaleMesh(SkyEN, XScale * 2, XScale * 0.5f, XScale * 2);
		EntityOrder(SkyEN, 3);
		EntityFX(SkyEN, 1 + 8);

		CloudEN = LoadMesh("Data\\Meshes\\ScreenPlane.b3d");
		ScaleMesh(CloudEN, XScale * 2, XScale * 0.5f, XScale * 2);
		EntityFX(CloudEN, 1 + 8);
		EntityOrder(CloudEN, 1);

		EntityShader(SkyEN, LoadShader("Data\\Game Data\\Shaders\\Default\\nps_Sky.fx"));
		EntityShader(CloudEN, LoadShader("Data\\Game Data\\Shaders\\Default\\nps_Cloud.fx"));

		uint Cloud = LoadTexture("NOTEXTURE");
		uint Stars = LoadTexture("NOTEXTURE");
		uint SkyTag = LoadTexture("NOTEXTURE");
		EntityTexture(SkyEN, Cloud, 0);
		EntityTexture(SkyEN, Stars, 1);
		EntityTexture(SkyEN, SkyTag, 2);
		EntityTexture(CloudEN, Cloud, 0);
		EntityTexture(CloudEN, Stars, 1);
		EntityTexture(CloudEN, SkyTag, 2);

		// Emitters
		int Tex = LoadTexture("Data\\Textures\\Particles\\Rain.bmp", 1 + 4 + 8);
		if(Tex == 0)
			RuntimeError("Could not find Data\\Textures\\Particles\\Rain.bmp!");
		int Config = RP_LoadEmitterConfig("Data\\Emitter Configs\\Rain.rpc", Tex, Camera);
		RainEmitter = RP_CreateEmitter(Config);
		Tex = LoadTexture("Data\\Textures\\Particles\\Snow.bmp", 1 + 4 + 8);
		if(Tex == 0)
			RuntimeError("Could not find Data\\Textures\\Particles\\Snow.bmp!");
		Config = RP_LoadEmitterConfig("Data\\Emitter Configs\\Snow.rpc", Tex, Camera);
		SnowEmitter = RP_CreateEmitter(Config);
		RP_DisableEmitter(RainEmitter);
		RP_DisableEmitter(SnowEmitter);

		// Sounds
		Snd_Rain = LoadSound("Data\\Sounds\\Weather\\Rain.wav");
		if(Snd_Rain == 0)
			Snd_Rain = LoadSound("Data\\Sounds\\Weather\\Rain.ogg");
		SoundVolume(Snd_Rain, 0.0f);
		LoopSound(Snd_Rain);
		Snd_Wind = LoadSound("Data\\Sounds\\Weather\\Wind.wav");
		if(Snd_Wind == 0)
			Snd_Wind = LoadSound("Data\\Sounds\\Weather\\Wind.ogg");
		SoundVolume(Snd_Wind, 0.0f);
		LoopSound(Snd_Wind);
		SndC_Rain = BBPlaySound(Snd_Rain);
		SndC_Wind = BBPlaySound(Snd_Wind);
		Snd_Thunder[0] = LoadSound("Data\\Sounds\\Weather\\Thunder1.wav");
		SoundVolume(Snd_Thunder[0], Globals->DefaultVolume);
		Snd_Thunder[1] = LoadSound("Data\\Sounds\\Weather\\Thunder2.wav");
		SoundVolume(Snd_Thunder[1], Globals->DefaultVolume);
		Snd_Thunder[2] = LoadSound("Data\\Sounds\\Weather\\Thunder3.wav");
		SoundVolume(Snd_Thunder[2], Globals->DefaultVolume);

		SunEN = CreateMesh();
		int Surf = CreateSurface(SunEN);
		int v1 = AddVertex(Surf, -1.0f, -1.0f, 0.0f, 1.0f, 1.0f);
		int v2 = AddVertex(Surf,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f);
		int v3 = AddVertex(Surf,  1.0f,  1.0f, 0.0f, 0.0f, 0.0f);
		int v4 = AddVertex(Surf, -1.0f,  1.0f, 0.0f, 1.0f, 0.0f);
		AddTriangle(Surf, v1, v2, v3);
		AddTriangle(Surf, v1, v3, v4);
		ScaleMesh(SunEN, 7.5f, 7.5f, 1.0f);
		Tex = LoadTexture("Data\\Textures\\Suns & Moons\\Sun.PNG");
		if(Tex != 0)
			EntityTexture(SunEN, Tex);
		EntityFX(SunEN, 1 + 8 + 32);
		EntityShader(SunEN, LoadShader("Data\\Game Data\\Shaders\\Default\\FullBrightAdd.fx"));
		EntityOrder(SunEN, 2);

		MoonEN = CreateMesh();
		Surf = CreateSurface(MoonEN);
		v1 = AddVertex(Surf, -1.0f, -1.0f, 0.0f, 1.0f, 1.0f);
		v2 = AddVertex(Surf,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f);
		v3 = AddVertex(Surf,  1.0f,  1.0f, 0.0f, 0.0f, 0.0f);
		v4 = AddVertex(Surf, -1.0f,  1.0f, 0.0f, 1.0f, 0.0f);
		AddTriangle(Surf, v1, v2, v3);
		AddTriangle(Surf, v1, v3, v4);
		ScaleMesh(MoonEN, 7.5f, 7.5f, 1.0f);
		Tex = LoadTexture("Data\\Textures\\Suns & Moons\\Moon.PNG");
		if(Tex != 0)
			EntityTexture(MoonEN, Tex);
		EntityFX(MoonEN, 1 + 8 + 32);
		EntityShader(MoonEN, LoadShader("Data\\Game Data\\Shaders\\Default\\FullBrightAdd.fx"));
		EntityOrder(MoonEN, 2);

		HideEntity(SkyEN);
		HideEntity(CloudEN);
		HideEntity(SunEN);
		HideEntity(MoonEN);

		// Default Lights
		Light0 = CreateDirectionalLight();
		Light1 = CreateDirectionalLight();

		SetLightDirection(Light0, 0, -1, 0);
		SetLightDirection(Light1, 0, 1, 0);

		SetDLightColor(Light0, 200, 200, 200);
		SetDLightColor(Light1, 50, 50, 200);

		SetDLightActive(Light0, false);
		SetDLightActive(Light1, false);
	}

	CDefaultEnvironment::~CDefaultEnvironment()
	{
	}

	void CDefaultEnvironment::UnloadArea()
	{
		Visible = false;

		HideEntity(SkyEN);
		HideEntity(CloudEN);
		HideEntity(SunEN);
		HideEntity(MoonEN);
		SetDLightActive(Light0, false);
		SetDLightActive(Light1, false);
	}

	void CDefaultEnvironment::Hide()
	{
		Visible = false;

		HideEntity(SkyEN);
		HideEntity(CloudEN);
		HideEntity(SunEN);
		HideEntity(MoonEN);
		SetDLightActive(Light0, false);
		SetDLightActive(Light1, false);
	}

	void CDefaultEnvironment::Show()
	{
		Visible = true; 

		ShowEntity(SkyEN);
		ShowEntity(CloudEN);
		ShowEntity(SunEN);
		ShowEntity(MoonEN);
		SetDLightActive(Light0, true);
		SetDLightActive(Light1, true);

	}

	void CDefaultEnvironment::LoadArea(std::string zoneName)
	{
		Visible = true;

		ShowEntity(SkyEN);
		ShowEntity(CloudEN);
		ShowEntity(SunEN);
		ShowEntity(MoonEN);
		SetDLightActive(Light0, true);
		SetDLightActive(Light1, true);

		uint CloudTex = LoadTexture("##DEFAULT");
		uint StarsTex = LoadTexture("##DEFAULT");
		uint SkyGradTex = LoadTexture("##DEFAULT");
		uint LensTex = LoadTexture("##DEFAULT");

		std::string EnvPath = string("Data\\Areas\\") + zoneName + string(".env");
		FILE* F = ReadFile(EnvPath);
		if(F != 0)
		{
			std::string TexPath = "Data\\Textures\\";

			CloudTex = LoadTexture(TexPath + ReadString(F));
			StarsTex = LoadTexture(TexPath + ReadString(F));
			SkyGradTex = LoadTexture(TexPath + ReadString(F));
			LensTex = LoadTexture(TexPath + ReadString(F));

			CloseFile(F);
		}else
		{
			// These correspond to the default project
			std::string TexPath = "Data\\Textures\\Skies\\";
			CloudTex = LoadTexture(TexPath + "n_h_1024_s.dds");
			StarsTex = LoadTexture(TexPath + "stars.dds");
			SkyGradTex = LoadTexture(TexPath + "SkyGrad.png");
			LensTex = LoadTexture(TexPath + "SunFlare.png");
		}


		EntityTexture(SkyEN, CloudTex, 0);
		EntityTexture(CloudEN, CloudTex, 0);

		EntityTexture(SkyEN, StarsTex, 1);
		EntityTexture(CloudEN, StarsTex, 1);

		EntityTexture(SkyEN, SkyGradTex, 2);
		EntityTexture(CloudEN, SkyGradTex, 2);

		EntityTexture(SkyEN, LensTex, 3);
		EntityTexture(CloudEN, LensTex, 3);

		CameraClsColor(Camera, Globals->FogR, Globals->FogG, Globals->FogB);
		CameraFogColor(Camera, Globals->FogR, Globals->FogG, Globals->FogB);
		FogNearDest = Globals->FogNear;
		FogFarDest = Globals->FogFar;
		FogChange = true;

		TargetAmount = CloudSunAmount;
		TargetWeight = CloudSunWeight;
		TargetSpeedScale = CloudNormalSpeedScale;
		TargetRainVolume = 0.0f;
		TargetWindVolume = 0.0f;
		SwayTarget = 0.1f;


	}

	void CDefaultEnvironment::SetTime(int timeH, int timeM, float secondInterpolation)
	{
		TimeH = timeH;
		TimeM = timeM;
		SecondInterp = secondInterpolation;
	}

	void CDefaultEnvironment::SetDate(int* day, int* year)
	{
		Day = *day;
		Year = *year;

		if(Day > 365)
		{
			Day = 0;
			++Year;

			*day = Day;
			*year = Year;
		}
	}

	void CDefaultEnvironment::SetWeather(int weather)
	{
		RP_DisableEmitter(RainEmitter);
		RP_DisableEmitter(SnowEmitter);
		if(CameraUnderwater == false)
		{
			CameraClsColor(Camera, Globals->FogR, Globals->FogG, Globals->FogB);
			CameraFogColor(Camera, Globals->FogR, Globals->FogG, Globals->FogB);
		}

		switch(weather)
		{
		case W_Sun:
			{
				FogNearDest = Globals->FogNear;
				FogFarDest = Globals->FogFar;
				FogChange = true;

				TargetAmount = CloudSunAmount;
				TargetWeight = CloudSunWeight;
				TargetSpeedScale = CloudNormalSpeedScale;
				TargetRainVolume = 0.0f;
				TargetWindVolume = 0.0f;
				SwayTarget = 0.1f;

				break;
			}
		case W_Rain:
			{
				RP_EnableEmitter(RainEmitter);
				FogFarDest = Globals->FogFar - 50.0f;
				if(Globals->FogNear > 0.5f)
					FogNearDest = 0.5f;
				else
					FogNearDest = -50.0f;
				if(FogFarDest < FogNearDest + 10.0f)
					FogFarDest = FogNearDest + 10.0f;
				FogChange = true;

				TargetAmount = CloudRainAmount;
				TargetWeight = CloudRainWeight;
				TargetSpeedScale = CloudNormalSpeedScale;
				TargetRainVolume = 1.0f;
				TargetWindVolume = 0.0f;
				SwayTarget = 0.7f;

				break;
			}
		case W_Snow:
			{
				RP_EnableEmitter(SnowEmitter);
				FogFarDest = Globals->FogFar - 125.0f;
				if(Globals->FogNear > 0.5f)
					FogNearDest = 0.5f;
				else
					FogNearDest = -50.0f;
				if(FogFarDest < FogNearDest + 10.0f)
					FogFarDest = FogNearDest + 10.0f;
				FogChange = true;
				if(CameraUnderwater == false)
				{
					CameraClsColor(Camera, 200, 200, 200);
					CameraFogColor(Camera, 200, 200, 200);
				}

				TargetAmount = CloudSnowAmount;
				TargetWeight = CloudSnowWeight;
				TargetSpeedScale = CloudNormalSpeedScale;
				TargetRainVolume = 0.0f;
				TargetWindVolume = 0.0f;
				SwayTarget = 0.4f;

				break;
			}
		case W_Fog:
			{
				FogFarDest = Globals->FogFar - 500.0f;
				if(Globals->FogNear > 0.5f)
					FogNearDest = 0.5f;
				else
					FogNearDest = -50.0f;
				if(FogFarDest < FogNearDest + 10.0f)
					FogFarDest = FogNearDest + 10.0f;
				FogChange = true; 

				TargetAmount = CloudFogAmount;
				TargetWeight = Vector3(Globals->FogR, Globals->FogG, Globals->FogB);
				TargetSpeedScale = CloudNormalSpeedScale;
				TargetRainVolume = 0.0f;
				TargetWindVolume = 0.0f;
				SwayTarget = 0.1f;

				break;
			}
		case W_Storm:
			{
				RP_EnableEmitter(RainEmitter);
				FogFarDest = Globals->FogFar - 100.0f;
				if(Globals->FogNear > 0.5f)
					FogNearDest = 0.5f;
				else
					FogNearDest = -50.0f;
				if(FogFarDest < FogNearDest + 10.0f)
					FogFarDest = FogNearDest + 10.0f;
				FogChange = true;

				TargetAmount = CloudStormAmount;
				TargetWeight = CloudStormWeight;
				TargetSpeedScale = CloudNormalSpeedScale;
				TargetRainVolume = 1.0f;
				TargetWindVolume = 0.0f;
				SwayTarget = 1.0f;

				break;
			}
		case W_Wind:
			{
				FogNearDest = Globals->FogNear;
				FogFarDest = Globals->FogFar;
				FogChange = true;

				TargetAmount = CloudWindAmount;
				TargetWeight = CloudWindWeight;
				TargetSpeedScale = CloudWindSpeedScale;
				TargetRainVolume = 0.0f;
				TargetWindVolume = 1.0f;
				SwayTarget = 1.0f;

				break;
			}
		}
		if(CameraUnderwater)
			FogChange = false;
		CurrentWeather = weather;
	}


	void CDefaultEnvironment::SetViewDistance(float Near, float Far, bool ForceSkyChange)
	{
		CameraFogRange(Camera, Near, Far);

		if(ForceSkyChange)
		{
			CameraRange(Camera, 0.8f, Far + 10.0f);
			ScaleEntity(SkyEN, Far - 10.0f, Far - 10.0f, Far - 10.0f);
			ScaleEntity(CloudEN, Far - 10.0f, Far - 10.0f, Far - 10.0f);
		}
	}


	float CDefaultEnvironment::Lerp(float A, float B, float T)
	{
		return A + ((B - A) * T);
	}

	float CDefaultEnvironment::Clamp(float value, float min, float max)
	{
		if(value < min)
			return min;
		if(value > max)
			return max;
		return value;
	}

	void CDefaultEnvironment::Update(float deltaTime)
	{
		PositionEntity(SkyEN, EntityX(Camera), EntityY(Camera) - 100.0f, EntityZ(Camera));
		PositionEntity(CloudEN, EntityX(Camera), EntityY(Camera) - 100.0f, EntityZ(Camera));

		if(CurrentWeather == W_Rain || CurrentWeather == W_Storm)
		{
			PositionEntity(RainEmitter, EntityX(Camera), EntityY(Camera) + 25.0f, EntityZ(Camera));
			RemoveUnderwaterParticles(RainEmitter);
		}else if(CurrentWeather == W_Snow) // Snow emitter
		{
			PositionEntity(SnowEmitter, EntityX(Camera), EntityY(Camera) + 20.0f, EntityZ(Camera));
			RemoveUnderwaterParticles(SnowEmitter);
		}

		// Lightning
		if(CurrentWeather == W_Storm)
			if(LightningToDo == 0)
			{
				if(Rand(1, (int)(500.0f / deltaTime)) == 1)
					LightningToDo = Rand(1,3);
			}else if(Rand(1, 50) == 1)
			{
				--LightningToDo;
				ScreenFlash(255, 255, 255, 65535, Rand(200, 1000), 0.9f);
				if(LightningToDo == 0)
					BBPlaySound(Snd_Thunder[Rand(0, 2)]);
			}

		// Fog
		if(FogChange)
		{
			if(FogNearNow < FogNearDest)
			{
				FogNearNow += (3.0f * deltaTime);
				if(FogNearNow > FogNearDest)
					FogNearNow = FogNearDest;
			}else if(FogNearNow > FogNearDest)
			{
				FogNearNow -= (3.0f * deltaTime);
				if(FogNearNow < FogNearDest)
					FogNearNow = FogNearDest;
			}

			if(FogFarNow < FogFarDest)
			{
				FogFarNow += (3.0f * deltaTime);
				if(FogFarNow > FogFarDest)
					FogFarNow = FogFarDest;
			}else if(FogFarNow > FogFarDest)
			{
				FogFarNow -= (3.0f * deltaTime);
				if(FogFarNow < FogFarDest)
					FogFarNow = FogFarDest;
			}

			
			this->SetViewDistance(FogNearNow, FogFarNow, true);
			if(fabs(FogFarNow - FogFarDest) < 0.1f && fabs(FogNearNow - FogNearDest) < 0.1f)
				FogChange = false;
		}
		
		// Cloud Amount needs to increase
		if(CloudAmount < TargetAmount)
			if(!(CloudAmount + TargetEpsilon >= TargetAmount))
				CloudAmount += 0.01f / deltaTime;

		// Cloud Amount needs to decrease
		if(CloudAmount > TargetAmount)
			if(!(CloudAmount - TargetEpsilon <= TargetAmount))
				CloudAmount -= 0.01f / deltaTime;

		// Increasing Cloud Weights
		if(CloudWeight.X < TargetWeight.X)
			if(!(CloudWeight.X + TargetEpsilon >= TargetWeight.X))
				CloudWeight.X += 0.01f / deltaTime;

		if(CloudWeight.Y < TargetWeight.Y)
			if(!(CloudWeight.Y + TargetEpsilon >= TargetWeight.Y))
				CloudWeight.Y += 0.01f / deltaTime;

		if(CloudWeight.Z < TargetWeight.Z)
			if(!(CloudWeight.Z + TargetEpsilon >= TargetWeight.Z))
				CloudWeight.Z += 0.01f / deltaTime;

		// Decreasing Cloud Weights
		if(CloudWeight.X > TargetWeight.X)
			if(!(CloudWeight.X - TargetEpsilon <= TargetWeight.X))
				CloudWeight.X -= 0.01f / deltaTime;
		
		if(CloudWeight.Y > TargetWeight.Y)
			if(!(CloudWeight.Y - TargetEpsilon <= TargetWeight.Y))
				CloudWeight.Y -= 0.01f / deltaTime;
		
		if(CloudWeight.Z > TargetWeight.Z)
			if(!(CloudWeight.Z - TargetEpsilon <= TargetWeight.Z))
				CloudWeight.Z -= 0.01f / deltaTime;

		// Cloud Speed needs to increase
		if(CloudSpeedScale < TargetSpeedScale)
			if(!(CloudSpeedScale + TargetEpsilon >= TargetSpeedScale))
				CloudSpeedScale += 0.003f / deltaTime;

		// Cloud Spped needs to decrease
		if(CloudSpeedScale > TargetSpeedScale)
			if(!(CloudSpeedScale - TargetEpsilon <= TargetSpeedScale))
				CloudSpeedScale -= 0.003f / deltaTime;

		// Cloud Speed needs to increase
		if(RainVolume < TargetRainVolume)
			if(!(RainVolume + TargetEpsilon >= TargetRainVolume))
				RainVolume += 0.003f / deltaTime;
		
		if(WindVolume < TargetWindVolume)
			if(!(WindVolume + TargetEpsilon >= TargetWindVolume))
				WindVolume += 0.003f / deltaTime;

		// Cloud Spped needs to decrease
		if(RainVolume > TargetRainVolume)
			if(!(RainVolume - TargetEpsilon <= TargetRainVolume))
				RainVolume -= 0.01f / deltaTime;

		if(WindVolume > TargetWindVolume)
			if(!(WindVolume - TargetEpsilon <= TargetWindVolume))
				WindVolume -= 0.01f / deltaTime;

		if(SwayAmount > SwayTarget)
			if(!(SwayAmount - TargetEpsilon <= SwayTarget))
				SwayAmount -= 0.002f / deltaTime;

		if(SwayAmount < SwayTarget)
			if(!(SwayAmount + TargetEpsilon > SwayTarget))
				SwayAmount += 0.002f / deltaTime;
		
		if(ChannelPlaying(SndC_Rain))
			ChannelVolume(SndC_Rain, RainVolume);
		if(ChannelPlaying(SndC_Wind))
			ChannelVolume(SndC_Wind, WindVolume);

		bool IsDay = false;
		if(TimeH >= 5 && TimeH < 19)
			IsDay = true;

		int NextH = TimeH;
		int NextM = TimeM;

		++NextM;
		if(NextM > 59)
		{
			++NextH;
			NextM = 0;
			if(NextH > 23)
				NextH = 0;
		}

		Vector3 SunDirection;
		Math::Vector3 Black(0, 0, 0);
		Math::Vector3 SunOrange(230, 134, 17);
		Math::Vector3 SunBright(175, 200, 228);
		Math::Vector3 MoonBright(75, 95, 132);

		Math::Vector3 AmNight(27, 28, 45);
		Math::Vector3 AmDawn(234,240,197);
		Math::Vector3 AmDay(66,93,165);
		Math::Vector3 AmDusk(170,60,7);

		AmNight *= 0.5f;
		AmDawn *= 0.1f;
		AmDay *= 0.5f;
		AmDusk *= 0.5f;
 
		if(IsDay)
		{
			// Transform into a 0-based length
			int ProgH = TimeH - 5;
			NextH -= 5;

			// Get day progress in minutes (and next minute for interp) and
			// divide by day length to produce a scalar day length values/
			// Use double precision here to keep a smooth interpolation
			double ProgM = static_cast<double>(TimeDelta(0, 0, ProgH, TimeM)) / (14.0 * 60.0);
			double ProgN = static_cast<double>(TimeDelta(0, 0, NextH, NextM)) / (14.0 * 60.0);

			// Introduce our seconds interp from this frame to keep the minute-to-minute transitions smooth
			double Prog = ProgM + ((ProgN - ProgM) * static_cast<double>(SecondInterp));

			// Convert to an angle in degrees to transitions
			float SunAngle = static_cast<float>((Prog * 180.0) + 90.0);
			float XAngle = 20.0f;

			// Position sun and moon
			PositionEntity(SunEN, EntityX(Camera, true) + (100.0f * Sin(XAngle)),
				EntityY(Camera, true) + (100.0f * (-Cos(SunAngle) * Cos(XAngle))),
				EntityZ(Camera, true) + (100.0f * Sin(SunAngle)));
			PositionEntity(MoonEN, EntityX(Camera, true) + (100.0f * Sin(XAngle)),
				EntityY(Camera, true) - (100.0f * (-Cos(SunAngle) * Cos(XAngle))),
				EntityZ(Camera, true) - (100.0f * Sin(SunAngle)));
			PointEntity(SunEN, Camera);
			PointEntity(MoonEN, Camera);

			SunDirection = Vector3(Sin(XAngle), -Cos(SunAngle) * Cos(XAngle),
				Sin(SunAngle));
			SunDirection.Normalize();

			Math::Vector3 SunColor = SunBright;
			Math::Vector3 MoonColor = Black;

			Math::Vector3 AmbientColor = AmDay;


			if(SunDirection.Y < 0.1f)
			{
				float Interp = Clamp(SunDirection.Y * 10.0f, 0.0f, 1.0f);
				SunColor = Black.Lerp(SunOrange, Interp);
				MoonColor = MoonBright.Lerp(Black, Interp);
			}else if(SunDirection.Y < 0.2f)
			{
				float Interp = Clamp((SunDirection.Y - 0.1f) * 10.0f, 0.0f, 1.0f);
				SunColor = SunOrange.Lerp(SunBright, Interp);
			}

			if(SunDirection.Y < 0.4f)
			{
				float Interp = Clamp((SunDirection.Y - 0.3f) * 10.0f, 0.0f, 1.0f);

				// Morning
				if(TimeH < 12)
					AmbientColor = AmNight.Lerp(AmDawn, Interp);
				else // Night
					AmbientColor = AmNight.Lerp(AmDusk, Interp);

			}else if(SunDirection.Y < 0.5f)
			{
				float Interp = Clamp((SunDirection.Y - 0.4f) * 10.0f, 0.0f, 1.0f);

				// Morning
				if(TimeH < 12)
					AmbientColor = AmDawn.Lerp(AmDay, Interp);
				else // Night
					AmbientColor = AmDusk.Lerp(AmDay, Interp);
			}

			SetLightDirection(Light0, -SunDirection.X, -SunDirection.Y, -SunDirection.Z);
			SetDLightColor(Light0, (int)SunColor.X, (int)SunColor.Y, (int)SunColor.Z);
			
			SetLightDirection(Light1, -SunDirection.X, SunDirection.Y, SunDirection.Z);
			SetDLightColor(Light1, (int)MoonColor.X, (int)MoonColor.Y, (int)MoonColor.Z);

			AmbientLight((int)AmbientColor.X, (int)AmbientColor.Y, (int)AmbientColor.Z);

		}else
		{
			// Transform into a 0-based length
			int ProgH = TimeH;
			if(ProgH < 6)
				ProgH += 24;
			ProgH -= 19;

			if(NextH < 6)
				NextH += 24;
			NextH -= 19;

			// Get day progress in minutes (and next minute for interp) and
			// divide by day length to produce a scalar day length values/
			// Use double precision here to keep a smooth interpolation
			double ProgM = static_cast<double>(TimeDelta(0, 0, ProgH, TimeM)) / (10.0 * 60.0);
			double ProgN = static_cast<double>(TimeDelta(0, 0, NextH, NextM)) / (10.0 * 60.0);

			// Introduce our seconds interp from this frame to keep the minute-to-minute transitions smooth
			double Prog = ProgM + ((ProgN - ProgM) * static_cast<double>(SecondInterp));

			// Convert to an angle in degrees to transitions
			float SunAngle = static_cast<float>((Prog * 180.0) + 90.0);
			float XAngle = 20.0f;

			// Position sun and moon
			PositionEntity(SunEN, EntityX(Camera, true) + (100.0f * Sin(XAngle)),
				EntityY(Camera, true) - (100.0f * (-Cos(SunAngle) * Cos(XAngle))),
				EntityZ(Camera, true) - (100.0f * Sin(SunAngle)));
			PositionEntity(MoonEN, EntityX(Camera, true) + (100.0f * Sin(XAngle)),
				EntityY(Camera, true) + (100.0f * (-Cos(SunAngle) * Cos(XAngle))),
				EntityZ(Camera, true) + (100.0f * Sin(SunAngle)));
			PointEntity(SunEN, Camera);
			PointEntity(MoonEN, Camera);

			SunDirection = Vector3(Sin(XAngle), Cos(SunAngle) * Cos(XAngle),
				-Sin(SunAngle));
			SunDirection.Normalize();

			Math::Vector3 SunColor = Black;
			Math::Vector3 MoonColor = MoonBright;
			Math::Vector3 AmbientColor = AmNight;

			SetLightDirection(Light0, -SunDirection.X, SunDirection.Y, SunDirection.Z);
			SetDLightColor(Light0, (int)MoonColor.X, (int)MoonColor.Y, (int)MoonColor.Z);
			

			SetLightDirection(Light1, -SunDirection.X, -SunDirection.Y, -SunDirection.Z);
			SetDLightColor(Light1, (int)SunColor.X, (int)SunColor.Y, (int)SunColor.Z);

			AmbientLight((int)AmbientColor.X, (int)AmbientColor.Y, (int)AmbientColor.Z);
		}

		// Create a day timer.
		// 0          0.25            0.5          0.75          1
		// |___________|_______________|____________|____________|
		// Midnight   Sunrise       Midday       Sunset        Midnight

		float DayTimer = (float)(TimeH * 60 + TimeM);
		DayTimer /= 24.0f * 60.0f;


		EntityConstantFloat4(SkyEN, "CloudWeight", CloudWeight.X, CloudWeight.Y, CloudWeight.Z, 1.0f);
		EntityConstantFloat3(SkyEN, "ConstSunDir", SunDirection.X, SunDirection.Y, SunDirection.Z);
		EntityConstantFloat(SkyEN, "CloudAmount", CloudAmount);
		EntityConstantFloat(SkyEN, "WindSpeed", 0.07f * CloudSpeedScale);
		EntityConstantFloat(SkyEN, "DayTimer", DayTimer);
		
		EntityConstantFloat4(CloudEN, "CloudWeight", CloudWeight.X, CloudWeight.Y, CloudWeight.Z, 1.0f);
		EntityConstantFloat3(CloudEN, "ConstSunDir", SunDirection.X, SunDirection.Y, SunDirection.Z);
		EntityConstantFloat(CloudEN, "CloudAmount", CloudAmount);
		EntityConstantFloat(CloudEN, "WindSpeed", 0.07f * CloudSpeedScale);
		EntityConstantFloat(CloudEN, "DayTimer", DayTimer);

		GlobalShaderConstantFloat4("SunDirection", SunDirection.X, SunDirection.Y, SunDirection.Z, 0.0f);

		if(CameraUnderwater)
		{
			HideEntity(SkyEN);
			HideEntity(CloudEN);
			HideEntity(SunEN);
			HideEntity(MoonEN);
		}else
		{
			ShowEntity(SkyEN);
			ShowEntity(CloudEN);
			ShowEntity(SunEN);
			ShowEntity(MoonEN);
		}

	}

	// Returns the delta (in minutes) between two times
	int CDefaultEnvironment::TimeDelta(int StartH, int StartM, int EndH, int EndM)
	{
		if(StartH == EndH) // Start and end are in the same hour
			return EndM - StartM;
		else if(StartH < EndH) // Start hour is before end hour
			return (60 - StartM) + EndM + (60 * (EndH - (StartH + 1)));
		else
			return (60 - StartM) + EndM + (60 * (24 - (StartH + 1))) + (60 * EndH);
	}

	void CDefaultEnvironment::SetCameraUnderWater( bool under, unsigned char destR, unsigned char destG, unsigned char destB )
	{
		CameraClsColor(Camera, destR, destG, destB);
		CameraFogColor(Camera, destR, destG, destB);
		FogNearNow = 1.0f;
		FogFarNow = 50.0f;
		SetViewDistance(FogNearNow, FogFarNow, false);
	}

	bool CDefaultEnvironment::PlayWetFootstep()
	{
		if(CurrentWeather == W_Rain || CurrentWeather == W_Storm)
			return true;
		return false;
	}

	void CDefaultEnvironment::ProcessNetPacket(RealmCrafter::PacketReader &reader)
	{
		SetWeather(reader.ReadByte());
	
	}

	std::string CDefaultEnvironment::GetName()
	{
		return "default";
	}
}
