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
#include "CDemoEnvironment.h"

using namespace std;
using namespace NGin;
using namespace NGin::Math;

namespace RealmCrafter
{
	CDemoEnvironment::CDemoEnvironment(uint camera)
		: Camera(camera), SkyEN(0), CloudEN(0), CameraUnderwater(false),
		FogNearDest(0), FogFarDest(0), FogNearNow(0), FogFarNow(0),
		Year(0), Day(0), TimeH(0), TimeM(0), TimeFactor(10),
		TimeUpdate(0), HourChanged(true), MinuteChanged(true), FogChange(true)
	{
		// Its possible for us to load data in the "LoadArea" section.
		// However, if a game is using a consistent environment throughout then
		// its easier to simply retain the data throughout execution

		// Load sky mesh
		SkyEN = LoadMesh("Data\\Meshes\\DemoEnvironment\\Skydome.b3d");
		if(SkyEN == 0)
			RuntimeError("Data\\Meshes\\DemoEnvironment\\Skydome.b3d");

		//ScaleMesh(SkyEN, XScale * 2, XScale * 0.5f, XScale * 2);

		// Manually override the rendering order (forcing the sky to render first)
		EntityOrder(SkyEN, 3);
		EntityFX(SkyEN, 1 + 8);

		// The clouds are just another version of the sky dome
		CloudEN = CopyEntity(SkyEN);
		EntityFX(CloudEN, 1 + 8);
		EntityOrder(CloudEN, 1);

		// Load and apply Sky and Cloud shaders
		EntityShader(SkyEN, LoadShader("Data\\Game Data\\Shaders\\DemoEnvironment\\SkyDome.fx"));
		EntityShader(CloudEN, LoadShader("Data\\Game Data\\Shaders\\Default\\nps_Cloud.fx"));

		// Apply default (empty) textures
		uint Cloud = LoadTexture("Data\\Meshes\\DemoEnironment\\DaySkydome.dds");
		uint Stars = LoadTexture("NOTEXTURE");
		uint SkyTag = LoadTexture("NOTEXTURE");
		/*EntityTexture(SkyEN, Cloud, 0);
		EntityTexture(SkyEN, Stars, 1);
		EntityTexture(SkyEN, SkyTag, 2);
		EntityTexture(CloudEN, Cloud, 0);
		EntityTexture(CloudEN, Stars, 1);
		EntityTexture(CloudEN, SkyTag, 2);*/

		// Create a mesh for the sun billboard
		SunEN = CreateMesh();
		int Surf = CreateSurface(SunEN);
		int v1 = AddVertex(Surf, -1.0f, -1.0f, 0.0f, 1.0f, 1.0f);
		int v2 = AddVertex(Surf,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f);
		int v3 = AddVertex(Surf,  1.0f,  1.0f, 0.0f, 0.0f, 0.0f);
		int v4 = AddVertex(Surf, -1.0f,  1.0f, 0.0f, 1.0f, 0.0f);
		AddTriangle(Surf, v1, v2, v3);
		AddTriangle(Surf, v1, v3, v4);
		ScaleMesh(SunEN, 7.5f, 7.5f, 1.0f);
		uint Tex = LoadTexture("Data\\Textures\\Suns & Moons\\Sun.PNG");
		if(Tex != 0)
			EntityTexture(SunEN, Tex);
		EntityFX(SunEN, 1 + 8 + 32);
		EntityShader(SunEN, LoadShader("Data\\Game Data\\Shaders\\Default\\FullBrightAdd.fx"));
		EntityOrder(SunEN, 2);

		// Same for moon
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

		// Hide environment until its needed
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

	CDemoEnvironment::~CDemoEnvironment()
	{
		// Clean up
		FreeEntity(SkyEN);
		FreeEntity(CloudEN);
		FreeEntity(SunEN);
		FreeEntity(MoonEN);
		FreeDLight(Light0);
		FreeDLight(Light1);
	}

	// A zone is shutting down
	void CDemoEnvironment::UnloadArea()
	{
		// Just hide rather than freeing
		Hide();
	}

	// Hide environment (mostly for the editor)
	void CDemoEnvironment::Hide()
	{
		HideEntity(SkyEN);
		HideEntity(CloudEN);
		HideEntity(SunEN);
		HideEntity(MoonEN);
		SetDLightActive(Light0, false);
		SetDLightActive(Light1, false);
	}

	// Show environment (mostly for the editor)
	void CDemoEnvironment::Show()
	{
		ShowEntity(SkyEN);
		//ShowEntity(CloudEN);
		ShowEntity(SunEN);
		ShowEntity(MoonEN);
		SetDLightActive(Light0, true);
		SetDLightActive(Light1, true);

	}

	// A zone is being loaded
	void CDemoEnvironment::LoadArea(std::string zoneName)
	{
		// Show hidden entities
		Show();

		// Reset Fog
		FogNearDest = Globals->FogNear;
		FogFarDest = Globals->FogFar;
		FogChange = true;
	}

	// Called when the environment time updates (usually before Update() is called)
	void CDemoEnvironment::SetTime(int timeH, int timeM, float secondInterpolation)
	{
		TimeH = timeH;
		TimeM = timeM;
		SecondInterp = secondInterpolation;
	}

	// Called when the day 'ticks over' allowing the year to be reset.
	// This code has little bearing on the operation of environments
	void CDemoEnvironment::SetDate(int* day, int* year)
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

	// Set a new view range
	// Near is really the fog start distance (rather than the frustum plane)
	// Far is the fog fadeout distance and camera far plane.
	void CDemoEnvironment::SetViewDistance(float Near, float Far, bool ForceSkyChange)
	{
		// Change fog values
		CameraFogRange(Camera, Near, Far);

		if(ForceSkyChange)
		{
			CameraRange(Camera, 0.8f, Far + 10.0f);
			//ScaleEntity(SkyEN, Far - 10.0f, Far - 10.0f, Far - 10.0f);
			ScaleEntity(CloudEN, Far - 10.0f, Far - 10.0f, Far - 10.0f);
		}
	}

	// Simple linear interpolation implementation.
	float CDemoEnvironment::Lerp(float a, float b, float t)
	{
		return a + ((b - a) * t);
	}

	// Simple value clamp implementation
	float CDemoEnvironment::Clamp(float value, float min, float max)
	{
		if(value < min)
			return min;
		if(value > max)
			return max;
		return value;
	}

	// Update representation of the environment
	void CDemoEnvironment::Update(float deltaTime)
	{
		// Reposition world entities around the player.
		PositionEntity(SkyEN, EntityX(Camera), EntityY(Camera) - 20.0f, EntityZ(Camera));
		PositionEntity(CloudEN, EntityX(Camera), EntityY(Camera), EntityZ(Camera));

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


			this->SetViewDistance(FogNearNow, FogFarNow, false);
			if(fabs(FogFarNow - FogFarDest) < 0.1f && fabs(FogNearNow - FogNearDest) < 0.1f)
				FogChange = false;
		}

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

				SetLightDirection(Light0, -SunDirection.X, -SunDirection.Y, -SunDirection.Z);
				SetDLightColor(Light0, (int)SunColor.X, (int)SunColor.Y, (int)SunColor.Z);

				SetLightDirection(Light1, -SunDirection.X, SunDirection.Y, SunDirection.Z);
				SetDLightColor(Light1, (int)MoonColor.X, (int)MoonColor.Y, (int)MoonColor.Z);

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

				SetLightDirection(Light0, -SunDirection.X, SunDirection.Y, SunDirection.Z);
				SetDLightColor(Light0, (int)MoonColor.X, (int)MoonColor.Y, (int)MoonColor.Z);


				SetLightDirection(Light1, -SunDirection.X, -SunDirection.Y, -SunDirection.Z);
				SetDLightColor(Light1, (int)SunColor.X, (int)SunColor.Y, (int)SunColor.Z);
			}


			EntityConstantFloat3(SkyEN, "ConstSunDir", SunDirection.X, SunDirection.Y, SunDirection.Z);


			EntityConstantFloat3(CloudEN, "ConstSunDir", SunDirection.X, SunDirection.Y, SunDirection.Z);


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
	int CDemoEnvironment::TimeDelta(int StartH, int StartM, int EndH, int EndM)
	{
		if(StartH == EndH) // Start and end are in the same hour
			return EndM - StartM;
		else if(StartH < EndH) // Start hour is before end hour
			return (60 - StartM) + EndM + (60 * (EndH - (StartH + 1)));
		else
			return (60 - StartM) + EndM + (60 * (24 - (StartH + 1))) + (60 * EndH);
	}

	// Called when the camera switches its position.
	void CDemoEnvironment::SetCameraUnderWater(bool under, unsigned char destR, unsigned char destG, unsigned char destB)
	{
		CameraClsColor(Camera, destR, destG, destB);
		CameraFogColor(Camera, destR, destG, destB);
		FogNearNow = 1.0f;
		FogFarNow = 50.0f;
		SetViewDistance(FogNearNow, FogFarNow, false);
	}

	// Called to check whether the rain footstep sound should be played
	// There is no weather in the demo game, so its always false.
	bool CDemoEnvironment::PlayWetFootstep()
	{
		return false;
	}

	// An environment packet was received from the server.
	// We do nothing with this since its usually for weather.
	void CDemoEnvironment::ProcessNetPacket(RealmCrafter::PacketReader &reader)
	{
	}

	// Return the name of this object.
	std::string CDemoEnvironment::GetName()
	{
		return "default";
	}
}
