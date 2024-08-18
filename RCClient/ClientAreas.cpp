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

#include "ClientAreas.h"
#include "LightFunctionList.h"

ClassDef(Remove_Surf, Remove_SurfList, Remove_SurfDelete);
ClassDef(Cluster, ClusterList, ClusterDelete);
ClassDef(Scenery, SceneryList, SceneryDelete);
ClassDef(Water, WaterList, WaterDelete);
ClassDef(ColBox, ColBoxList, ColBoxDelete);
ClassDef(Emitter, EmitterList, EmitterDelete);
ClassDef(Terrain, TerrainList, TerrainDelete);
ClassDef(SoundZone, SoundZoneList, SoundZoneDelete);
ClassDef(CatchPlane, CatchPlaneList, CatchPlaneDelete);
ClassDef(ZoneLight, ZoneLightList, ZoneLightDelete);

int SkyTexID = 65535, CloudTexID = 65535, StormCloudTexID = 65535, StarsTexID = 65535;

char Outdoors;
unsigned char AmbientR = 100, AmbientG = 100, AmbientB = 100;
float DefaultLightPitch, DefaultLightYaw;
int LoadingTexID = 65535, LoadingMusicID = 65535;
int MapTexID;
float SlopeRestrict = 0.6;
float TerrainHeights[1024][1024];
unsigned short RainSoundId = 65535;
unsigned short StormSoundId0 = 65535;
unsigned short StormSoundId1 = 65535;
unsigned short StormSoundId2 = 65535;
unsigned short WindSoundId = 65535;
unsigned short SnowTextureId = 65535;
unsigned short RainTextureId = 65535;
NGin::Math::AABB ZoneSize;

NGin::Math::Vector3 MenuCameraPosition, MenuCameraLookAt, MenuActorSpawn;

// Creates a subdivided plane (used for water)
int CreateSubdividedPlane(int XDivs, int ZDivs, float UScale, float VScale, int Parent)
{
	int EN = CreateMesh(Parent);
	int Surf = CreateSurface(EN);

	for(int x = 0; x < XDivs; ++x)
		for(int z = 0; z < ZDivs; ++z)
		{
			float XPos = ((float)x) / ((float)XDivs - 1);
			float ZPos = ((float)z) / ((float)ZDivs - 1);
			int V = AddVertex(Surf, XPos, 0.0f, ZPos, XPos * UScale, ZPos * VScale);
			VertexNormal(Surf, V, 0.0f, 1.0f, 0.0f);
			if(x > 0 && z > 0)
			{
				int v1 = ((x - 1) * ZDivs) + (z - 1);
				int v2 = ((x - 1) * ZDivs) + z;
				int v3 = (x * ZDivs) + (z - 1);
				int v4 = (x * ZDivs) + z;
				AddTriangle(Surf, v1, v2, v4);
				AddTriangle(Surf, v1, v4, v3);
			}
		}
	PositionMesh(EN, -0.5f, 0.0f, 0.5f);
	return EN;
}

// Creates a water plane mesh
uint CreatePlane()
{
	uint Out = CreateMesh();
	uint Surf = CreateSurface(Out);
    AddVertex(Surf, -0.5f, 0.0f, -0.5f, 0.0f, 0.0f);
    AddVertex(Surf,  0.5f, 0.0f, -0.5f, 1.0f, 0.0f);
    AddVertex(Surf,  0.5f, 0.0f,  0.5f, 1.0f, 1.0f);
    AddVertex(Surf, -0.5f, 0.0f,  0.5f, 0.0f, 1.0f);
    AddTriangle(Surf, 0, 2, 1);
    AddTriangle(Surf, 0, 3, 2);
    AddTriangle(Surf, 1, 2, 0);
    AddTriangle(Surf, 2, 3, 0);
    UpdateNormals(Out);
    UpdateHardwareBuffers(Out);
    SetAlphaState(Out, true);
    return Out;
}

std::string ReadCCharString(FILE* F)
{
	unsigned char ToRead = ReadByte(F);

	char* Str = (char*)malloc(ToRead+1);
	fread(Str,1, ToRead,F);
	Str[ToRead] = 0;
	std::string Out = Str;
	free(Str);

	return Out;
}

// Loads the client (3D) data for an area
bool LoadArea(string Name, int CameraEN, bool DisplayItems, bool UpdateRottNet, bool showLoadingBar, bool disableAllCollisions)
{
	// Rottnet update
	int RNUpdateTime = MilliSecs();
FILE* dbf = WriteFile("sceneries.txt");
	// Set render callback for GUI... This is only useful on the first zone load (prevents flickering effect)
	SetRenderGUICallback(0, &RenderGUI);

	static IProgressBar* LoadProgressBar = 0;
	static IPictureBox* LoadScreen = 0;
	int CLoadMusic = 0;
	
	LockMeshes();
	LockTextures();
	
	// Open file
	FILE* F = ReadFile(string("Data\\Areas\\") + Name + string(".dat"));
	if(F == 0)
		return false;

	// Loading Screen
	int LoadingTexID = ReadShort(F);
	int LoadingMusicID = ReadShort(F);

	
	// Progress bar
	if(LoadProgressBar == 0)
		LoadProgressBar = GUIManager->CreateProgressBar("LoadingScreen::ProgressBar", Math::Vector2(0.3f, 0.9f), Math::Vector2(0.4f, 0.03f));
	else
		LoadProgressBar->Visible = true;

	// Preset image
	if(LoadScreen == 0)
		LoadScreen = GUIManager->CreatePictureBox(std::string("LoadingScreen::Background"), Math::Vector2(0, 0), Math::Vector2(1, 1));
	else
		LoadScreen->Visible = true;

	if(showLoadingBar)
	{
		LoadScreen->SetImage(string("Data\\DefaultTex.png"));
		LoadScreen->BringToFront();
		LoadProgressBar->BringToFront();

		GUIManager->SetProperties("LoadingScreen");
	}else
	{
		LoadProgressBar->Visible = false;
		LoadScreen->Visible = false;
	}


	if(LoadingTexID < 65535)
	{
		LoadScreen->SetImage(string("Data\\Textures\\") + GetTextureNameNoFlag(LoadingTexID));
	}else if(RandomImages > 0) // Random image
	{
		int D = ReadDir(string("Data\\Textures\\Random"));
		if(D == 0)
			LoadScreen->BackColor = Math::Color(0, 0, 0);
		else
		{
			string File = "";
			for(int i = 1; i < Rand(1, RandomImages) +1; ++i)
			{
				do
				{
					File = NextFile(D);
				}while(FileType(string("Data\\Textures\\Random\\") + File) != 1 && File.length() > 0);
				if(File.length() == 0)
					break;
			}
			if(FileType(string("Data\\Textures\\Random\\") + File) == 1)
				LoadScreen->SetImage(string("Data\\Textures\\Random\\") + File);
			else
				LoadScreen->BackColor = Math::Color(0, 0, 0);
			CloseDir(D);
		}
	}else // No image
	{
		LoadScreen->BackColor = Math::Color(0, 0, 0);
	}

	// Music
	if(LoadingMusicID < 65535)
		CLoadMusic = PlayMusic(string("Data\\Music\\") + GetMusicName(LoadingMusicID));


	// Loading bar update
	if(showLoadingBar && LoadScreen != 0)
	{
		LoadProgressBar->Value = 0;
		LoadScreen->BringToFront();
		LoadProgressBar->BringToFront();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		RenderWorld();
	}

	// Environment
	int SkyTagID = ReadShort(F);
	int CloudTexID = ReadShort(F);
	int UnusedStormID = ReadShort(F);
	int StarsTexID = ReadShort(F);

	RealmCrafter::Globals->FogR = ReadByte(F);
	RealmCrafter::Globals->FogG = ReadByte(F);
	RealmCrafter::Globals->FogB = ReadByte(F);
	RealmCrafter::Globals->FogNear = ReadFloat(F);
	RealmCrafter::Globals->FogFar = ReadFloat(F);
	if(RealmCrafter::Globals->FogFar > MaxFogFar)
		RealmCrafter::Globals->FogFar = MaxFogFar;
//	FogNearNow = FogNear;
//	FogFarNow = FogFar;

	Math::AABB NewZoneSize;

	//Environment->SetSkyTextures(CloudTexID, StarsTexID, SkyTagID);


	// Camera
	if(CameraEN != 0)
	{
// 		Environment->SetViewDistance(FogNear, FogFar, true);
// 		CameraFogColor(CameraEN, FogR, FogG, FogB);
// 		CameraClsColor(CameraEN, FogR, FogG, FogB);
	}

	MapTexID = ReadShort(F);
	Outdoors = ReadByte(F);
	AmbientR = ReadByte(F);
	AmbientG = ReadByte(F);
	AmbientB = ReadByte(F);
	DefaultLightPitch = ReadFloat(F);
	DefaultLightYaw = ReadFloat(F);
	SlopeRestrict = ReadFloat(F);
	bbdx2_SetSlopeRestriction(SlopeRestrict);
	AmbientLight(AmbientR, AmbientG, AmbientB);
	//TerrainManager->SetAmbientLight(NGin::Math::Color(AmbientR, AmbientG, AmbientB));
	

	// RottNet update
	if(UpdateRottNet && MilliSecs() - RNUpdateTime > 500)
	{
		RCE_Update(); RCE_CreateMessages();
		RNUpdateTime = MilliSecs();
	}

	//  Loading bar update
	if(showLoadingBar && LoadScreen != 0)
	{
		LoadProgressBar->Value = 5;
		LoadScreen->BringToFront();
		LoadProgressBar->BringToFront();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		RenderWorld();
	}

	// Scenery
	int Sceneries = ReadShort(F);
	for(int i = 1; i < Sceneries + 1; ++i)
	{
		Scenery* S = new Scenery();

		

		// Mesh (from media database ID)
		S->MeshID = ReadShort(F);


		std::string mnm = GetMeshName(S->MeshID);
		char* mnms = (char*)mnm.c_str();
		mnms[mnm.length() - 1] = 0;
		fprintf(dbf, "%s\n", mnms);
		S->EN = GetMesh(S->MeshID);
//		if(S->MeshID == 0) HideEntity(S->EN);

		// Read transform
		float X = ReadFloat(F);
		float Y = ReadFloat(F);
		float Z = ReadFloat(F);
		float Pitch = ReadFloat(F);
		float Yaw = ReadFloat(F);
		float Roll = ReadFloat(F);
		S->ScaleX = ReadFloat(F);
		S->ScaleY = ReadFloat(F);
		S->ScaleZ = ReadFloat(F);

		S->Position.X = X;
		S->Position.Y = Y;
		S->Position.Z = Z;
		S->Position.FixValues();

		NewZoneSize.AddPoint(X, Y, Z);
		
		// Animation mode and ownership
		S->AnimationMode = ReadByte(F);
		S->SceneryID = ReadByte(F);
		
		// Retexturing
		S->TextureID = ReadShort(F);

		// Collision/picking
		byte Flags = ReadByte(F);
		S->CatchRain = (Flags & 1) > 0;
		S->Interactive = (Flags & 2) > 0;

		// Level of Detail
		char Collides;
		uchar CollisionMeshID = 65535, IsNewVersion = ReadByte(F);
		int CollisionMesh;
		if ( IsNewVersion == 255 )
		{
			float distLOD_High = ReadFloat(F);
			if (S->EN != 0 && distLOD_High != 0) setMeshLOD(S->EN, 0, 0, distLOD_High);

			int iLOD = 0;
			int MeshLOD_Medium = ReadShort(F);
			float distLOD_Medium = ReadFloat(F);
			if (S->EN != 0 &&  MeshLOD_Medium != 65535 )
			{
				iLOD = GetMesh(MeshLOD_Medium);
				EntityShadowLevel(S->EN, 2);
				setMeshLOD(S->EN, iLOD, 1, distLOD_Medium);
				FreeEntity(iLOD);
			}

			int MeshLOD_Low = ReadShort(F);
			float distLOD_Low = ReadFloat(F);
			if (S->EN != 0 &&  MeshLOD_Low != 65535 )
			{
				iLOD = GetMesh(MeshLOD_Low);
				EntityShadowLevel(S->EN, 2);
 				setMeshLOD(S->EN, iLOD, 2, distLOD_Low);
 				FreeEntity(iLOD);
			}

			// CollisionType
			Collides = ReadByte(F);
			if ( Collides == C_Triangle ) /*CollisionMesh = GetMesh(*/CollisionMeshID = ReadShort(F)/*)*/;
		}
		else
			Collides = IsNewVersion;

		// Lightmap information and RCTE data
		S->Lightmap = ReadString(F);
		S->RCTE = ReadString(F);
		
		if(S->EN != 0)
		{
			EntityShadowLevel(S->EN, 2);
			ResetEntity(S->EN);

			// Set trans
			PositionEntity(S->EN, X, Y, Z);
			RotateEntity(S->EN, Pitch, Yaw, Roll);
			ScaleEntity(S->EN, S->ScaleX, S->ScaleY, S->ScaleZ);

			// Lightmap
			if(S->Lightmap.length() != 0)
			{
				int LMap = LoadTexture(string("Data\\Textures\\Lightmaps\\") + S->Lightmap);
				EntityTexture(S->EN, LMap, 0, 1);
				FreeTexture(LMap);
			}

			// Retexturing
			if(S->TextureID < 65535)
				EntityTexture(S->EN, GetTexture(S->TextureID));

			// Type Handle
			NameEntity(S->EN, toString(Handle(S)));

			// Animation
			if(S->AnimationMode == 1)
				Animate(S->EN, 1);
			else if(S->AnimationMode == 2)
				Animate(S->EN, 2);

			// Set Collision/picking
			EntityType(S->EN, Collides);
			if(!disableAllCollisions)
			{
				if(Collides == C_Sphere)
				{
					EntityPickMode(S->EN, 1);
					float MaxLength = MeshWidth(S->EN) * S->ScaleX;
					if(MeshDepth(S->EN) * S->ScaleZ > MaxLength)
						MaxLength = MeshDepth(S->EN) * S->ScaleZ;
					EntityRadius(S->EN, MaxLength / 2.0f, (MeshHeight(S->EN) * S->ScaleY) / 2.0f);
				}else if(Collides == C_Triangle)
				{
					if ( IsNewVersion == 255 ) // New Version Area
					{
						if(CollisionMeshID != 65535 && CollisionMeshID != S->MeshID)
						{
							S->CollisionEN = GetMesh(CollisionMeshID);

							if(S->CollisionEN != NULL)
							{
								EntityPickMode(S->EN, 0);
								EntityType(S->EN, 0);
								EntityParent(S->CollisionEN, S->EN, false);
								EntityType(S->CollisionEN, Collides);
								EntityPickMode(S->CollisionEN, 2);
								SetCollisionMesh(S->CollisionEN);
								HideEntity(S->CollisionEN, false);
							}
						}else
						{
	// 					PositionEntity(CollisionMesh, X, Y, Z);
	// 					RotateEntity(CollisionMesh, Pitch, Yaw, Roll);
	// 					ScaleEntity(CollisionMesh, S->ScaleX, S->ScaleY, S->ScaleZ);
	// 
	// 					EntityPickMode(CollisionMesh, 2);
	// 					SetCollisionMesh(CollisionMesh);

							EntityPickMode(S->EN, 2);
							SetCollisionMesh(S->EN);
						}
					}
					else						// Old Version Area
					{
						EntityPickMode(S->EN, 2);
						SetCollisionMesh(S->EN);
					}
				}else if(Collides == C_Box)
				{
					EntityPickMode(S->EN, 3);
					float Width = MeshWidth(S->EN) * S->ScaleX;
					float Height = MeshHeight(S->EN) * S->ScaleY;
					float Depth = MeshDepth(S->EN) * S->ScaleZ;
					bbdx2_EntityBox(S->EN, Width / -2.0f, Height / -2.0f, Depth / -2.0f, Width, Height, Depth);
				}
			}
			

			

			// Create carch plane if required
			if(S->CatchRain)
			{
				MeshMinMaxVertices* MMV = MeshMinMaxVerticesTransformed(S->EN, Pitch, Yaw, Roll, S->ScaleX, S->ScaleY, S->ScaleZ);
				CatchPlane* CP = new CatchPlane();
				CP->Y = MMV->MaxY + Y;
				CP->Min.X = MMV->MinX + X;
				CP->Min.Z = MMV->MinZ + Z;
				CP->Max.X = MMV->MaxX + X;
				CP->Max.Z = MMV->MaxZ + Z;
				CP->Min.FixValues();
				CP->Max.FixValues();
			}
		}else // Mesh has been deleted or removed from the database!
		{	
			Warning(string("Could not find model with ID ") + toString(S->MeshID));
			Scenery::Delete(S);
			Scenery::Clean();
		}

		



		// Rottnet update
		if(UpdateRottNet && MilliSecs() - RNUpdateTime > 500)
		{
			RCE_Update(); RCE_CreateMessages();
			RNUpdateTime = MilliSecs();
		}

		// Loading bar update every alternate object
		if(showLoadingBar && LoadScreen != 0 && (i % 2) == 0 && Sceneries > 0)
		{
			LoadProgressBar->Value = 5 + (40 * i) / Sceneries;
			LoadScreen->BringToFront();
			LoadProgressBar->BringToFront();
			RenderWorld();
		}
	} // Next Scenery

	// Water
	int Waters = ReadShort(F);
	for(int i = 1; i < Waters + 1; ++i)
	{
		Water* W = new Water();

		uint ShaderWater = GetShader(ReadInt(F));
		W->TexScale = ReadFloat(F);

		// Position/Size
		float X = ReadFloat(F);
		float Y = ReadFloat(F);
		float Z = ReadFloat(F);

		W->Position.Reset(X, Y, Z);
		W->Position.FixValues();

		if (i==1) SetWaterLevel(Y);

		W->ScaleX = ReadFloat(F);
		W->ScaleZ = ReadFloat(F);
		int XDivs = (int)Ceil(W->ScaleX / 15.0f);
		int ZDivs = (int)Ceil(W->ScaleZ / 15.0f);
		if(XDivs > 70)
			XDivs = 70;
		if(ZDivs > 70)
			ZDivs = 70;
		
		W->EN = CreatePlane();
		ScaleEntity(W->EN, 16.0f, 1.0f, 16.0f);
		EntityShader(W->EN, ShaderWater);
		ScaleEntity(W->EN, W->ScaleX, (W->ScaleX + W->ScaleZ) * 0.5f, W->ScaleZ);
		PositionEntity(W->EN, X, Y, Z);

		// Colour
		W->Red = ReadByte(F);
		W->Green = ReadByte(F);
		W->Blue = ReadByte(F);

		// Opacity
		W->Opacity = ReadByte(F);
		if(W->Opacity >= 100)
			EntityFX(W->EN, 1 + 16);
		else
			EntityFX(W->EN, 16);
		float Alpha = ((float)W->Opacity) / 100.0f;
		if(Alpha > 1.0f)
			Alpha = 1.0f;
		EntityAlphaNoSolid(W->EN, Alpha);

		// Entity/Texture
		for (int t = 0; t < 4; ++t)
		{
			W->TexID[t] = ReadShort(F);
			W->TexHandle[t] = GetTexture(W->TexID[t], false);
			if (W->TexHandle[t] != 0)
			{
				ScaleTexture(W->TexHandle[t], W->TexScale, W->TexScale);
				EntityTexture(W->EN, W->TexHandle[t], t);
			}
		}

		// Type Handle
		NameEntity(W->EN, toString(Handle(W)));

		// If I am a walking only character, create a collision box here
		if(Me != 0)
			if(Me->Actor->Environment == Environment_Walk)
			{
				ColBox* C = new ColBox();
				C->EN = CreateCube();
				EntityAlpha(C->EN, 0.0f);

				// Transform
				Y -= 1000.0f;
				C->ScaleX = Abs(W->ScaleX / 2.0f);
				C->ScaleY = 1000.0f;
				C->ScaleZ = Abs(W->ScaleZ / 2.0f);
				PositionEntity(C->EN, X, Y, Z);
				ScaleEntity(C->EN, C->ScaleX, C->ScaleY, C->ScaleZ);

				// Collisions
				if(!disableAllCollisions)
				{
					SetCollisionMesh(C->EN);
					EntityType(C->EN, C_Box);
				}

				// Type Handle
				NameEntity(C->EN, toString(Handle(C)));
			}

		// RottNet update
		if(UpdateRottNet && MilliSecs() - RNUpdateTime > 500)
		{
			RCE_Update(); RCE_CreateMessages();
			RNUpdateTime = MilliSecs();
		}

		// Shader Parameters
		int nParams = ReadInt(F);
		for (int iParam = 0; iParam < nParams; ++iParam)
		{
			string MName = ReadCCharString(F);
			char ttype = ReadByte(F);
			switch(ttype) 
			{
				case 1:  { float X = ReadFloat(F);
						   EntityConstantFloat(W->EN, MName.c_str(), X); break; }
				case 2:  { float X = ReadFloat(F); float Y = ReadFloat(F);
						   EntityConstantFloat2(W->EN, MName.c_str(), X, Y); break; }
				case 3: { float X = ReadFloat(F); float Y = ReadFloat(F); float Z = ReadFloat(F);
						  EntityConstantFloat3(W->EN, MName.c_str(), X, Y, Z); break;}
				case 4: { float X = ReadFloat(F); float Y = ReadFloat(F); float Z = ReadFloat(F); float WW = ReadFloat(F);
						  EntityConstantFloat4(W->EN, MName.c_str(), X, Y, Z, WW); break; }
			}
		}
	} // Next Water

	
	// Loading bar update every alternate object
	if(showLoadingBar && LoadScreen != 0)
	{
		LoadProgressBar->Value = 60;
		LoadScreen->BringToFront();
		LoadProgressBar->BringToFront();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		RenderWorld();
	}

	// Collision zones
	int ColBoxes = ReadShort(F);
	for(int i = 1; i < ColBoxes + 1; ++i)
	{
		ColBox* C = new ColBox();
		C->EN = CreateCube();
		ScaleEntity(C->EN, 5, 5, 5);
		HideEntity(C->EN, false);

		// Transform
		float PosX = ReadFloat(F);
		float PosY = ReadFloat(F);
		float PosZ = ReadFloat(F);
		float Pitch = ReadFloat(F);
		float Yaw = ReadFloat(F);
		float Roll = ReadFloat(F);
		
		C->ScaleX = ReadFloat(F);
		C->ScaleY = ReadFloat(F);
		C->ScaleZ = ReadFloat(F);
		PositionEntity(C->EN, PosX, PosY, PosZ);
		RotateEntity(C->EN, Pitch, Yaw, Roll);
		ScaleEntity(C->EN, C->ScaleX, C->ScaleY, C->ScaleZ);

		C->Position.Reset(PosX, PosY, PosZ);
		C->Position.FixValues();
		
		// Collisions
		if(!disableAllCollisions)
		{
			EntityType(C->EN, C_Triangle);
			SetCollisionMesh(C->EN);
		}

		// Type handle
		NameEntity(C->EN, toString(Handle(C)));
	}

	// Loading bar update every alternate object
	if(showLoadingBar && LoadScreen != 0)
	{
		LoadProgressBar->Value = 65;
		LoadScreen->BringToFront();
		LoadProgressBar->BringToFront();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		RenderWorld();
	}

	// Emitters
	int Emitters = ReadShort(F);
	for(int i = 1; i < Emitters + 1; ++i)
	{
		// Create Emitters parent entity
		Emitter* E = new Emitter();
		E->EN = CreatePivot();

		// Read in emitter data
		E->ConfigName = ReadString(F);
		E->TexID = ReadShort(F);
		int Texture = GetTexture(E->TexID);
		float X = ReadFloat(F);
		float Y = ReadFloat(F);
		float Z = ReadFloat(F);
		float Pitch = ReadFloat(F);
		float Yaw = ReadFloat(F);
		float Roll = ReadFloat(F);

		E->Position.Reset(X, Y, Z);
		E->Position.FixValues();

		// Load Config
		E->Config = RP_LoadEmitterConfig((string("Data\\Emitter Configs\\") + E->ConfigName + string(".rpc")).c_str(), Texture, CameraEN);
		
		// Loaded successfully, create the emitter
		if(E->Config != 0)
		{
			int EmitterEN = RP_CreateEmitter(E->Config);
			EntityParent(EmitterEN, E->EN, false);
			EntityPickMode(E->EN, 2);
			NameEntity(E->EN, toString(Handle(E)));

			// Transform
			PositionEntity(E->EN, X, Y, Z);
			RotateEntity(E->EN, Pitch, Yaw, Roll);
		}else // Failed to load config
		{
			Warning(string("Could not load emitter: ") + E->ConfigName);
			FreeEntity(E->EN);
			Emitter::Delete(E);
			Emitter::Clean();
		}

		// RottNet update
		if(UpdateRottNet && MilliSecs() - RNUpdateTime > 500)
		{
			RCE_Update(); RCE_CreateMessages();
			RNUpdateTime = MilliSecs();
		}
	}

	// Loading bar update
	if(showLoadingBar && LoadScreen != 0)
	{
		LoadProgressBar->Value = 80;
		LoadScreen->BringToFront();
		LoadProgressBar->BringToFront();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		RenderWorld();
	}

	// Load basic mesh terrains
    ushort Terrains = ReadShort(F);
    for (int i = 0; i < Terrains; ++i)
    {
        Terrain* T = new Terrain();

		T->Path = ReadString(F);
		NGin::Math::Vector3 Pos;
		Pos.X = ReadFloat(F);
		Pos.Y = ReadFloat(F);
		Pos.Z = ReadFloat(F);
		T->Handle = TerrainManager->LoadT1(T->Path);

		if(T->Handle == NULL)
		{
			Terrain::Delete(T);
			continue;
		}

		if(!disableAllCollisions)
		{
			std::vector<TerrainTagItem*>* TerrainTag = new std::vector<TerrainTagItem*>();
			T->Handle->SetTag((void*)TerrainTag);
		}else
			T->Handle->SetTag(NULL);

		
		T->Handle->SetPosition(Pos);

		NewZoneSize.AddPoint(Pos);
		NewZoneSize.AddPoint(Pos + NGin::Math::Vector3(T->Handle->GetSize() * 2.0f, T->Handle->GetSize() * 2.0f, T->Handle->GetSize() * 2.0f));
	}

	Terrain::Clean();

	// Loading bar update every alternate object
	if(showLoadingBar && LoadScreen != 0 && Terrains > 0)
	{
		LoadProgressBar->Value = 80 + ((15 * 1) / Terrains);
		LoadScreen->BringToFront();
		LoadProgressBar->BringToFront();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		RenderWorld();
	}

	// Sound Zones
	int Sounds = ReadShort(F);
	for(int i = 1; i < Sounds + 1; ++i)
	{
		SoundZone* SZ = new SoundZone();
		SZ->EN = CreatePivot();

		// Transform
		float X = ReadFloat(F);
		float Y = ReadFloat(F);
		float Z = ReadFloat(F);
		SZ->Radius = ReadFloat(F);
		ScaleEntity(SZ->EN, SZ->Radius, SZ->Radius, SZ->Radius);
		PositionEntity(SZ->EN, X, Y, Z);

		SZ->Position.Reset(X, Y, Z);
		SZ->Position.FixValues();

		// Sound options
		SZ->SoundID = ReadShort(F);
		SZ->MusicID = ReadShort(F);
		SZ->RepeatTime = ReadInt(F);
		SZ->Volume = ReadByte(F);
		SZ->ChannelStopped = true;

		// Load Sound
		if(SZ->SoundID != 65535)
		{
			SZ->LoadedSound = GetSound(SZ->SoundID);
			SZ->Is3D = Asc(GetSoundName(SZ->SoundID).substr(GetSoundName(SZ->SoundID).length() - 1, 1));
		}else
			SZ->MusicFilename = string("Data\\Music\\") + GetMusicName(SZ->MusicID);

		// Type Handle
		NameEntity(SZ->EN, toString(Handle(SZ)));

		// RottNet update
		if(UpdateRottNet && MilliSecs() - RNUpdateTime > 500)
		{
			RCE_Update(); RCE_CreateMessages();
			RNUpdateTime = MilliSecs();
		}
	}

	// Lights
	int LightCount = 0;
	if(!Eof(F))
	{
		LightCount = ReadShort(F);
		if(!Eof(F))
		{
			for(int i = 0; i < LightCount; ++i)
			{
				// Get version
				unsigned char Version = ReadByte(F);

				switch(Version)
				{
				case 1:
					{
						ZoneLight* L = new ZoneLight();
						L->Handle = CreatePointLight();

						float PosX = ReadFloat(F);
						float PosY = ReadFloat(F);
						float PosZ = ReadFloat(F);

						L->Position.X = PosX;
						L->Position.Y = PosY;
						L->Position.Z = PosZ;
						L->Position.FixValues();
						
						L->R = ReadInt(F);
						L->G = ReadInt(F);
						L->B = ReadInt(F);
						L->Radius = ReadFloat(F);

						SetPLightColor(L->Handle, L->R, L->G, L->B);
						SetLightPosition(L->Handle, PosX, PosY, PosZ);
						SetLightRadius(L->Handle, L->Radius);
						

						break;
					}
				case 2:
					{
						ZoneLight* L = new ZoneLight();
						L->Handle = CreatePointLight();
						
						float PosX = ReadFloat(F);
						float PosY = ReadFloat(F);
						float PosZ = ReadFloat(F);

						L->Position.X = PosX;
						L->Position.Y = PosY;
						L->Position.Z = PosZ;
						L->Position.FixValues();

						L->R = ReadInt(F);
						L->G = ReadInt(F);
						L->B = ReadInt(F);
						L->Radius = ReadFloat(F);
						string FuncName = stringToLower(ReadString(F));

						for(int i = 0; i < RealmCrafter::LightFunctionList::Functions.size(); ++i)
						{
							string TestNameLower = stringToLower(RealmCrafter::LightFunctionList::Functions[i]->Name);

							if(FuncName.compare(TestNameLower) == 0)
							{
								L->Function = RealmCrafter::LightFunctionList::Functions[i];
								break;
							}
						}

						SetPLightColor(L->Handle, L->R, L->G, L->B);
						SetLightPosition(L->Handle, PosX, PosY, PosZ);
						SetLightRadius(L->Handle, L->Radius);
						

						break;
					}
				}

			}
		}
	}

	RainSoundId = 65535;
	StormSoundId0 = 65535;
	StormSoundId1 = 65535;
	StormSoundId2 = 65535;
	WindSoundId = 65535;
	SnowTextureId = 65535;
	RainTextureId = 65535;
	unsigned short SkyProfileId = 65535;
	unsigned short CloudProfileId = 65535;

	if(!Eof(F))
	{
		RainSoundId = ReadShort(F);
		StormSoundId0 = ReadShort(F);
		StormSoundId1 = ReadShort(F);
		StormSoundId2 = ReadShort(F);
		WindSoundId = ReadShort(F);
		SnowTextureId = ReadShort(F);
		RainTextureId = ReadShort(F);
	}
	if(!Eof(F))
	{
		SkyProfileId = ReadShort(F);
		CloudProfileId = ReadShort(F);
	}

	string EnvironmentName = "default";
	if(!Eof(F))
	{
		EnvironmentName = ReadString(F);
	}

	if(!Eof(F))
	{
		ushort MenuControlCount = ReadShort(F);
		for(int i = 0; i < MenuControlCount; ++i)
		{
			char MCType = ReadByte(F);
			float X = ReadFloat(F);
			float Y = ReadFloat(F);
			float Z = ReadFloat(F);

			if(MCType == 0)
				MenuCameraPosition.Reset(X, Y, Z);
			else if(MCType == 1)
				MenuCameraLookAt.Reset(X, Y, Z);
			else if(MCType == 2)
				MenuActorSpawn.Reset(X, Y, Z);
		}
	}
		

	// Put in the SDK!
// 	Environment->SetRainSound(RainSoundId);
// 	Environment->SetWindSound(WindSoundId);
// 	Environment->SetThunderSounds(StormSoundId0, StormSoundId1, StormSoundId2);
// 	Environment->SetRainTexture(RainTextureId);
// 	Environment->SetSnowTexture(SnowTextureId);
// 	Environment->SetSkyProfiles(SkyProfileId, CloudProfileId);

	Environment->LoadArea(Name, EnvironmentName);


	CloseFile(F);

	UnlockMeshes();
	UnlockTextures();

	// End loading screen
	if(showLoadingBar && LoadScreen != 0)
	{
		LoadScreen->Visible = false;
		LoadProgressBar->Visible = false;
		//GUIManager->Destroy(LoadScreen);
		//GUIManager->Destroy(LoadProgressBar);
	}

	if(ChannelPlaying(CLoadMusic))
		StopChannel(CLoadMusic);

	// FLT loader
	std::string ZonePath = std::string("Data\\Areas\\") + Name + ".ltz";
	TreeZone = TreeManager->LoadZone(ZonePath.c_str());
// 	LT::ITreeZone* TreeZone = TreeManager->CreateZone();
// 	TreeZone->Lock();
// 
// 	LT::ITreeType* TempType = TreeManager->FindTreeType("tree1");
// 	if(TempType == 0)
// 		RuntimeError("Couldn't refind type!");
// 
// 	for(int i = 0; i < 100; ++i)
// 	{
// 		float RndX = ((float)rand()) / ((float)RAND_MAX);
// 		float RndY = ((float)rand()) / ((float)RAND_MAX);
// 		RndX *= 200.0f;
// 		RndY *= 200.0f;
// 
// 		LT::ITreeInstance* Instance = TreeZone->AddTreeInstance(TempType, NGin::Math::Vector3(RndX, 0, RndY));
// 		Instance->SetScale(NGin::Math::Vector3(0.1f, 0.1f, 0.1f));
// 	}
// 
// 	TreeZone->Unlock();
// 
// 	TreeManager->SaveZone(TreeZone, "Tree\\TestZone.ltz");

	// Force terrains to update; we need to push the first collision tiles
	//TerrainManager->Update(NGin::Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true)), true);
	fclose(dbf);
	ZoneSize = NewZoneSize;

	return true;
}

// Unloads the current area from memory
void UnloadArea()
{
	if(SkyTexID > -1 && SkyTexID < 65535) UnloadTexture(SkyTexID);
	if(CloudTexID > -1 && CloudTexID < 65535) UnloadTexture(CloudTexID);
	if(StormCloudTexID > -1 && StormCloudTexID < 65535) UnloadTexture(StormCloudTexID);
	if(StarsTexID > -1 && StarsTexID < 65535) UnloadTexture(StarsTexID);

	foreachc(SIt, Scenery, SceneryList)
	{
		Scenery* S = (*SIt);
		if(S->TextureID < 65535)
			UnloadTexture(S->TextureID);
		FreeEntity(S->EN);
		Scenery::Delete(S);
		if(S == SceneryTarget)
			SceneryTarget = NULL;
		nextc(SIt, Scenery, SceneryList);
	}
	Scenery::Clean();

	foreachc(WIt, Water, WaterList)
	{
		Water* W = (*WIt);
		
		for (int i(0); i<4; ++i)
		{
			UnloadTexture(W->TexID[i]);
			//FreeTexture(W->TexHandle[i]);
		}
		FreeEntity(W->EN);
		Water::Delete(W);

		nextc(WIt, Water, WaterList);
	}
	Water::Clean();

	foreachc(CIt, ColBox, ColBoxList)
	{
		ColBox* C = (*CIt);

		FreeEntity(C->EN);
		ColBox::Delete(C);

		nextc(CIt, ColBox, ColBoxList);
	}
	ColBox::Clean();

	foreachc(EIt, Emitter, EmitterList)
	{
		Emitter* E = (*EIt);

		RP_FreeEmitter(GetChild(E->EN, 1), true, false);
		UnloadTexture(E->TexID);
		FreeEntity(E->EN);
		Emitter::Delete(E);

		nextc(EIt, Emitter, EmitterList);
	}
	Emitter::Clean();

	foreachc(SZIt, SoundZone, SoundZoneList)
	{
		SoundZone* SZ = (*SZIt);

		if(SZ->Channel != 0)
			StopChannel(SZ->Channel);
		if(SZ->SoundID > 0 && SZ->SoundID < 65535)
			UnloadSound(SZ->SoundID);
		FreeEntity(SZ->EN);
		SoundZone::Delete(SZ);

		nextc(SZIt, SoundZone, SoundZoneList);
	}
	SoundZone::Clean();

	foreachc(TIt, Terrain, TerrainList)
	{
		Terrain* T = (*TIt);

		delete T->Handle;
		Terrain::Delete(T);

		nextc(TIt, Terrain, TerrainList);
	}
	Terrain::Clean();

	foreachc(LIt, ZoneLight, ZoneLightList)
	{
		ZoneLight* L = (*LIt);

		FreePLight(L->Handle);
		ZoneLight::Delete(L);

		nextc(LIt, ZoneLight, ZoneLightList);
	}
	ZoneLight::Clean();

	foreachc(CPIt, CatchPlane, CatchPlaneList)
	{
		CatchPlane::Delete((*CPIt));
		nextc(CPIt, CatchPlane, CatchPlaneList);
	}
	CatchPlane::Clean();
	CatchPlane::CatchPlaneList.Clear();

	// Remove trees
#ifdef USE_240
	if(TreeZone != 0)
		TreeZone->Destroy();
	TreeZone = 0;
#endif
}


