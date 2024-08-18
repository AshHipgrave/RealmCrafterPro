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
#define NOMINMAX
#include <BlitzPlus.h>
#include "..\RCClient\Temp.h"
#include "OOWrapper.h"
//#include "..\MT\MT\Include\IMTTerrainManager.h"
#include <ITerrainManager.h>
#include "ITreeRenderer.h"
#include "ITreeManager.h"

//#include "VehicleManager.h"
//#include "ClothInt.h"
#include "BBDX.h"


using namespace RealmCrafter;

//using namespace RealmCrafter::MT;
IGUIManager* GUIManager;
RCT::ITerrainManager* TerrainManager(0);
LT::ITreeRenderer* TreeRenderer(0);
LT::ITreeManager* TreeManager(0);
ICamera* Camera = 0;

NGin::GUI::ILabel* DebugLabels[10];

void RenderGUI()
{
	if(GUIManager != 0)
		GUIManager->Render();
}

void DeviceLost()
{
	if(GUIManager != 0)
		GUIManager->OnDeviceLost();
	if(TerrainManager != 0)
		TerrainManager->OnDeviceLost();
}

void DeviceReset()
{
	if(GUIManager != 0)
		GUIManager->OnDeviceReset(NGin::Math::Vector2(GraphicsWidth(), GraphicsHeight()));
	if(TerrainManager != 0)
		TerrainManager->OnDeviceReset();
	//CameraProjMode(Cam, 1);
}


void RenderDepth(const float* lightMatrix)
{
	if(TerrainManager != 0)// && GFXShadowDetail >= 2)
	{
		//TerrainManager->RenderDepth(lightMatrix);
	}
}

uint TerrainRenderTime = 0;

void RenderSolid(int rtIndex)
{
	if(rtIndex != 0)
		return;

	// For the upcoming version
	if(TerrainManager != 0 || TreeRenderer != 0)
	{
		//int TerrainRender = StartDebugTimer("Terrain Render");


		float** Directions = new float*[3];
		float** Colors = new float*[3];
		for(int i = 0; i < 3; ++i)
		{
			Directions[i] = new float[3];
			Colors[i] = new float[3];

			Directions[i][0] = 0.0f;
			Directions[i][1] = 0.0f;
			Directions[i][2] = 0.0f;
			Colors[i][0] = 0.0f;
			Colors[i][1] = 0.0f;
			Colors[i][2] = 0.0f;
		}

		bbdx2_GetDirectionalLights((float**)Directions, (float**)Colors);

		for(int i = 0; i < 3; ++i)
		{
			if(TerrainManager != 0)
			{
				TerrainManager->SetLightNormal(i, NGin::Math::Vector3(Directions[i][0], Directions[i][1], Directions[i][2]));
				TerrainManager->SetLightColor(i, NGin::Math::Color(Colors[i][0], Colors[i][1], Colors[i][2]));
			}

			if(TreeRenderer != 0)
			{
				TreeRenderer->SetLightNormal(i, NGin::Math::Vector3(Directions[i][0], Directions[i][1], Directions[i][2]));
				TreeRenderer->SetLightColor(i, NGin::Math::Color(Colors[i][0], Colors[i][1], Colors[i][2]));
			}
		}
// 
// 		int Count = 0;
// 		unsigned int Stride = 0;
// 		void** LightPtr = bbdx2_GetPointLights(&Count, &Stride);

		//if(TerrainManager != 0)
		//	TerrainManager->SetPointLights(LightPtr, Count, Stride);
// 		if(TreeRenderer != 0)
// 			TreeRenderer->SetPointLights(LightPtr, Count, Stride);

		float FogNear = bbdx2_GetFogNear();
		float FogFar = bbdx2_GetFogFar();
		unsigned int IntFogColor = bbdx2_GetFogColor();
		int R = (IntFogColor >> 16) & 0xff;
		int G = (IntFogColor >> 8) & 0xff;
		int B = IntFogColor & 0xff;
		NGin::Math::Color FogColor(R, G, B);

		if(TerrainManager != 0)
			TerrainManager->SetFog(FogColor, NGin::Math::Vector2(FogNear, FogFar));
		if(TreeRenderer != 0)
			TreeRenderer->SetFog(FogColor, NGin::Math::Vector2(FogNear, FogFar));

		D3DXMATRIX* View = (D3DXMATRIX*)bbdx2_GetViewMatrixPtr();
		D3DXMATRIX* Projection = (D3DXMATRIX*)bbdx2_GetProjectionMatrixPtr();

		D3DXMATRIX VP;
		D3DXMatrixMultiply(&VP, View, Projection);

		//TerrainManager->SetAmbientLight(NGin::Math::Color(1.0f, 1.0f, 1.0f));
		if(TerrainManager != 0)
		{
			uint TerrainRenderStartTime = timeGetTime();
			RCT::FrameStatistics Stats = TerrainManager->Render(rtIndex, (float*)&VP, GetLightMatrix(), GetShadowMap());
			TerrainRenderTime = ((uint)timeGetTime()) - TerrainRenderStartTime;
			
// 			char DBO[1024];
// 			sprintf(DBO, "%i, %i, %i\n", Stats.ChunksCulled, Stats.ChunksDrawn, Stats.TrianglesDrawn);
// 			OutputDebugStringA(DBO);
		}
		/*if(TreeManager != 0)
		TreeManager->Render(*(reinterpret_cast<NGin::Math::Matrix*>(View)),
		*(reinterpret_cast<NGin::Math::Matrix*>(Projection)),
		*(reinterpret_cast<NGin::Math::Vector3*>(&TCamPos)));*/

		bbdx2_FreeMatrixPtr((void*)View);
		bbdx2_FreeMatrixPtr((void*)Projection);
		for(int i = 0; i < 3; ++i)
		{
			delete Directions[i];
			delete Colors[i];
		}
		delete Directions;
		delete Colors;

		//StopDebugTimer(TerrainRender);
	}
}

struct TerrainTagItem
{
	uint Mesh;
	NGin::Math::Vector3 Position;
};

// void Terrain_CollisionChanged(IMTTerrain* terrain, MTCollisionEventArgs* e)
// {
// 	OutputDebugStringA("Col");
// 	//return;
// 	ArrayList<TerrainTagItem*>* TerrainTag = (ArrayList<TerrainTagItem*>*)terrain->GetTag();
// 	if(TerrainTag == 0)
// 	{
// 		DebugLog("Terrain had no Tag, cannot set collision data");
// 		return;
// 	}
// 
// 	bool Found = false;
// 	for(int i = 0; i < TerrainTag->Size(); ++i)
// 	{
// 		if(((int)(*TerrainTag)[i]->Position.X) == ((int)e->GetPosition().X)
// 			&& ((int)(*TerrainTag)[i]->Position.Z) == ((int)e->GetPosition().Z))
// 		{
// 			if(e->GetTriangleList() == 0)
// 			{
// 				bbdx2_ASyncCancelInject((*TerrainTag)[i]->Mesh);
// 				bbdx2_FreeCollisionInstance((*TerrainTag)[i]->Mesh);
// 			}
// 			else
// 			{
// 				if(e->GetPosition().X > Camera->Position.X - 192.0f && e->GetPosition().X < Camera->Position.X + 192.0f
// 					&& e->GetPosition().Z > Camera->Position.Z - 192.0f && e->GetPosition().Z < Camera->Position.Z + 192.0f)
// 				{
// 					bbdx2_InjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount());
// 				}else
// 				{
// 					if(e->GetPosition().X > Camera->Position.X - 192.0f && e->GetPosition().X < Camera->Position.X + 128.0f
// 						&& e->GetPosition().Z > Camera->Position.Z - 192.0f && e->GetPosition().Z < Camera->Position.Z + 128.0f)
// 					{
// 						bbdx2_InjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount());
// 					}else
// 					{
// 						bbdx2_ASyncInjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount(), e->GetHighPriority() ? 1 : 0);
// 						//bbdx2_InjectCollisionMesh((*TerrainTag)[i]->Mesh, e->GetTriangleList(), e->GetVertexCount());
// 					}
// 				}
// 			}
// 			PositionEntity((*TerrainTag)[i]->Mesh, e->GetPosition().X + terrain->GetPosition().X, e->GetPosition().Y + terrain->GetPosition().Y, e->GetPosition().Z + terrain->GetPosition().Z);
// 
// 			Found = true;
// 		}
// 	}
// 
// 	if(!Found)
// 	{
// 
// 		uint Mesh = CreateMesh();
// 		TagEntity(Mesh, "TERRAIN");
// 
// 
// 
// 		if(e->GetTriangleList() == 0)
// 		{
// 			bbdx2_ASyncCancelInject(Mesh);
// 			bbdx2_FreeCollisionInstance(Mesh);
// 		}
// 		else
// 		{
// 			// If a tile is beneath the actor, it must be pushed immediately otherwise he'll fall through
// 			if(e->GetPosition().X > Camera->Position.X - 192.0f && e->GetPosition().X < Camera->Position.X + 192.0f
// 				&& e->GetPosition().Z > Camera->Position.Z - 192.0f && e->GetPosition().Z < Camera->Position.Z + 192.0f)
// 			{
// 				bbdx2_InjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount());
// 			}else
// 			{
// 				if(e->GetPosition().X > Camera->Position.X - 192.0f && e->GetPosition().X < Camera->Position.X + 128.0f
// 					&& e->GetPosition().Z > Camera->Position.Z - 192.0f && e->GetPosition().Z < Camera->Position.Z + 128.0f)
// 				{
// 					bbdx2_InjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount());
// 				}else
// 				{
// 					bbdx2_ASyncInjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount(), e->GetHighPriority() ? 1 : 0);
// 					//bbdx2_InjectCollisionMesh(Mesh, e->GetTriangleList(), e->GetVertexCount());
// 				}
// 			}
// 		}
// 
// 
// 		PositionEntity(Mesh, e->GetPosition().X + terrain->GetPosition().X, e->GetPosition().Y + terrain->GetPosition().Y, e->GetPosition().Z + terrain->GetPosition().Z);
// 		EntityType(Mesh, 2);
// 		EntityPickMode(Mesh, 2);
// 
// 		TerrainTagItem* TagItem = new TerrainTagItem();
// 		TagItem->Mesh = Mesh;
// 		TagItem->Position = e->GetPosition();
// 		TerrainTag->Add(TagItem);
// 	}
// }


//int __stdcall WinMain(HINSTANCE, HINSTANCE, char*, int)
int main()
{
	//StartRemoteDebugging("localhost", 6543);




	printf("Test!\r\n");
	uint hwnd = BBCreateWindow("GAME", 0, 0, 800, 600, 0, 1);
	Init(BBQueryObject(hwnd, 1), 0);
	Graphics3D(800, 600, 32, 2, 0, 0, "Data\\DefaultTex.png");

	IEngine::Collisions(1, 2, 2, 2);
	IEngine::Collisions(2, 1, 2, 2);

	Camera = IEngine::CreateCamera();
	Camera->ClearColor = Color(0, 128, 255);
	Camera->Range(1, 3200);
	Camera->FogMode(1);
	Camera->FogColor = Color(255, 255, 255);
	Camera->FogRange(10000, 10000);

	//Camera->CollisionType = 1;
	//Camera->CollisionRadius(2);
	//EntityRadius(Camera->Handle, 2.0f, 0.1f);

	Camera->Position = Vector3(0, 0, -100);
	Camera->Position = Vector3(0, 5, -20);
	Camera->Rotation = Vector3(0, 0, 0);

	//Camera->Position = Vector3(-172.722000, 143.822510, -212.881500);
	//Camera->Rotation = Vector3(26.800003, -40.799980, 0.000000);


	// Ambient Lighting
	IEngine::AmbientLight(Color(100, 100, 100));

	ILight* D = IEngine::CreateLight(ELT_Directional);
	D->Direction = Vector3(-1, -1, 0);
	D->Color = Color(255, 255, 255);


	uint LineEffect = LoadShader("Data\\Game Data\\Shaders\\Default\\3DLine.fx");
	//uint Solid = LoadShader("Data\\Game Data\\Shaders\\Default\\AnimatedMesh_High_NormalMapped.fx");
	uint Solid = LoadShader("Data\\Game Data\\Shaders\\Default\\LitObject_Far.fx");
	uint Solid_T = LoadShader("Data\\Game Data\\Shaders\\Default\\LitObject_High_RS_T.fx");
	uint DepthMapSolid = LoadShader("Data\\Game Data\\Shaders\\Default\\DepthMap.fx");

#define R(X, Y) (Math::Vector2(X, Y) / GUIManager->GetResolution())

	
	HMODULE GUILib = LoadLibraryA("NGUID3D9.dll");
	if(GUILib == NULL)
		RuntimeError("Could not load DLL: NGUID3D9.dll");

	typedef NGin::GUI::IGUIRenderer* (CreateCmd)(void* device, const char* effectPath);
	CreateCmd* CreateDirect3D9GUIRenderer = (CreateCmd*)GetProcAddress(GUILib, "?CreateDirect3D9GUIRenderer@@YAPAVIGUIRenderer@GUI@NGin@@PAXPBD@Z");
	if(CreateDirect3D9GUIRenderer == NULL)
		RuntimeError("Could not find rendering EntryPoint in NGUID3D9.dll");

	NGin::GUI::IGUIRenderer* GUIRenderer = CreateDirect3D9GUIRenderer(GetIDirect3DDevice9(), "Data\\Game Data\\Shaders\\Default\\GUI.fx");
	if(GUIRenderer == NULL)
		RuntimeError("Could not create GUI Renderer");

	//FreeLibrary(GUILib);

	GUIManager = CreateGUIManager(GUIRenderer, NGin::Math::Vector2(800, 600));

	GUIManager->FontDirectory("Data\\Fonts");
	GUIManager->LoadAndAddSkin("Data\\Skins\\Default\\Theme.xml");
	GUIManager->ImportProperties("Data\\Game Data\\Interface.xml");

	SetRenderGUICallback(0, &RenderGUI);
	SetDeviceLostCallback(0, &DeviceLost);
	SetDeviceResetCallback(0, &DeviceReset);

	for(int i = 0; i < 10; ++i)
	{
		DebugLabels[i] = GUIManager->CreateLabel("DebugLabel", R(50, 50 + (i * 20)), R(0, 0));
		DebugLabels[i]->Text = "";
	}

#undef R

 /*	SetRenderSolidCallbackRT(0, &RenderSolid);

	TerrainManager = CreateMTTerrainManager(GetIDirect3DDevice9(), 
		"Data\\Game Data\\Shaders\\Default\\MT.fx",
		"",
		"Data\\Game Data\\Shaders\\Default\\Grass.fx",
		"Data\\Game Data\\Shaders\\Default\\MTDepth.fx");
	TerrainManager->LoadGrassTypes("Data\\Game Data\\GrassTypes.xml");
	//TerrainManager->CollisionChanged()->AddEvent(&Terrain_CollisionChanged);
	TerrainManager->SetEditorMode(false);
	TerrainManager->SetAmbientLight(Color(0.1f, 0.1f, 0.1f));

	IMTTerrain* T = TerrainManager->CreateMT(2048);
	std::vector<TerrainTagItem*>* TerrainTag = new std::vector<TerrainTagItem*>();
	T->SetTag((void*)TerrainTag);*/

	//IEntity* Sphere = IEngine::LoadMesh("Medi-lab\\Middleright.b3d");
	//Sphere->Shader = Solid;

	/*IEntity* Sphere = IEngine::LoadMesh("Data\\Meshes\\Actors\\Lizardman\\LMA1.eb3d", 0, true);
	uint Seq = Sphere->ExtractSequence(0, 0);
	Sphere->Shader = Solid;
	Sphere->Scale = Vector3(0.05, 0.05, 0.05);
	Sphere->Position = Vector3(0, 300, 0);
	EntityShadowLevel(Sphere->Handle, 4);
	Sphere->Animate(1, 1.0f, Seq);

	Sphere->CollisionType = 1;

	//EntityRadius(Sphere->Handle, 2.0f);

	uint desc = bbdx2_CreatePhysicsDesc(false, Sphere->Handle);
	bbdx2_AddSphere(desc, 0, 0, 0, 2.0f, 2.0f);
	bbdx2_ClosePhysicsDesc(desc);*/


	//Sphere->CommitCollisionMesh();

//	Sphere->CollisionType = 2;
//	Sphere->CommitCollisionMesh();
// 
// 	EntityTexture(Sphere->Handle, LoadTexture("EmptyNormalMap.png"), 1);
// 	EntityTexture(Sphere->Handle, LoadTexture("Glow.png"), 2);

// 	IEntity* S2 = IEngine::LoadMesh("DoubleCube.b3d");
// 	CalculateB3DTangents(S2->Handle);
// 	S2->Shader = Solid_T;
// 	S2->Scale = Vector3(0.3f, 0.3f, 0.3f);
// 	S2->Position = Vector3(0, -1.0f, 0);
// 	EntityAlpha(S2->Handle, 1.0f);

// 	LoadUserDefinedPP_FromXML("Data\\Game Data\\PostProcess\\PostProcess.xml");
// 	SetPP_Pipeline("Default");

// 	for(float x = 0.0f; x < 8192.0f; x += 1024.0f)
// 	{
// 		for(float z = 0.0f; z < 8192.0f; z += 10024.0f)
// 		{
// 			IEntity* PSphere = IEngine::LoadMesh("Data\\Meshes\\Sphere_Mapped.eb3d");
// 			PSphere->Shader = Solid;
// 			PSphere->Scale = Math::Vector3(2, 2, 2);
// 			PSphere->Position = Math::Vector3(x, 0, z);
// 		}
// 	}

	SetRenderSolidCallbackRT(0, &RenderSolid);

	TerrainManager = RCT::CreateTerrainManager(GetIDirect3DDevice9(), 
		"Data\\Game Data\\Shaders\\Default\\RCT.fx",
		"",
		"Data\\Game Data\\Shaders\\Default\\Grass.fx",
		"Data\\Game Data\\Shaders\\Default\\MTDepth.fx");
	TerrainManager->LoadGrassTypes("Data\\Game Data\\GrassTypes.xml");
	//TerrainManager->CollisionChanged()->AddEvent(&Terrain_CollisionChanged);
	TerrainManager->SetEditorMode(false);
	TerrainManager->SetAmbientLight(Color(0.1f, 0.1f, 0.1f));

	RCT::ITerrain* T = TerrainManager->CreateT1(128);
	std::vector<TerrainTagItem*>* TerrainTag = new std::vector<TerrainTagItem*>();
	T->SetTag((void*)TerrainTag);

	T->SetGrassType(0, "Base Grass");

	RCT::Transform_PaintGrass(T, 128.0f, NGin::Math::Vector2(64, 64), true, 0xff, true);

	/*uint Grid = LoadMesh("Data\\Box.b3d");
	ScaleEntity(Grid, 100, 0.1f, 100);
	EntityShader(Grid, Solid);
	EntityTexture(Grid, LoadTexture("tgrid.png"));
	
	EntityType(Grid, 2);
	SetCollisionMesh(Grid);*/


	RCPKit::gVehicleManager = new RCPKit::VehicleManager();
	if( !RCPKit::gVehicleManager->LoadVehicles("Vehicles.xml") )
		RuntimeError("Failed to load: Vehicles.xml!");


	std::vector<RCPKit::Vehicle*> cars;
	/*cars.push_back(RCPKit::gVehicleManager->CreateVehicle("FAV"));
	cars.push_back(RCPKit::gVehicleManager->CreateVehicle("FAV"));

	cars[1]->SetVehicleMode(RCPKit::VM_Local);*/

	//SetupCloth();

	uint lastNetTime = MilliSecs();


	int Frame = 0;
	uint LastTime = timeGetTime();

	float Angle = 0.0f;

	int MySectorX = 0;
	int MySectorZ = 0;

	bool ReloadTerrains = true;

	uint FrameAvg[100];
	uint TerrainRenderAvg[100];
	uint TerrainUpdateAvg[100];
	float MoveAvg[10] = {0, 0, 0, 0, 0, 0, 0 ,0 ,0 ,0};
	uint LastSecCap = MilliSecs();

	// Main Loop
	while(!KeyHit(1) || JoyHit(XBOX_BACK))
	{
		//BeginDebugFrame();
		//int MainFrameTime = StartDebugTimer("Main Frame");

		uint FrameStartTime = timeGetTime();

		Vector3 CamVel = Camera->Position;

		

		float MoveSpeed = 0.05f * 5.0f;

		if( cars.size() == 0 || cars.back()->GetVehicleMode() == RCPKit::VM_Remote )
		{
 			if(KeyDown(17)) Camera->Move(Vector3( 0,  0,  3.1f * MoveSpeed));
 			if(KeyDown(31)) Camera->Move(Vector3( 0,  0, -3.1f * MoveSpeed));
 			if(KeyDown(30)) Camera->Move(Vector3(-2 * MoveSpeed,  0,  0));
 			if(KeyDown(32)) Camera->Move(Vector3( 2 * MoveSpeed,  0,  0));
		}

		if(KeyDown(200)) Camera->Turn(Vector3(-2.8f * MoveSpeed, 0,    0));
		if(KeyDown(208)) Camera->Turn(Vector3( 2.8f * MoveSpeed, 0,    0));
		if(KeyDown(203)) Camera->Turn(Vector3( 0,    2.8f * MoveSpeed, 0));
		if(KeyDown(205)) Camera->Turn(Vector3( 0,   -2.8f * MoveSpeed, 0));

		if(KeyHit(19)) bbdx2_ReloadShaders();

		if(KeyHit(57))
		{
			uint t = LoadThreadMesh("Data\\Meshes\\Actors\\lizardman\\lizardman.eb3d", 0, 1);
			EntityShader(t, Solid);
			ScaleEntity(t, 2, 2, 2);
		}

		CamVel = Camera->Position - CamVel;

		//Camera->Move(Vector3(0, 0, -0.0001f));

		Vector3 CamPos = Camera->Position;

		bool Modded = false;

// 		if(CamPos.X > 128.0f)
// 		{
// 			float Mvx = fmod(CamPos.X, 128.0f);
// 			float Sectors = (CamPos.X - Mvx) / 128.0f;
// 
// 			MySectorX += (int)Sectors;
// 			CamPos.X = -Sectors * 128.0f;
// 			Modded = true;
// 		}else if(CamPos.X < 0.0f)
// 		{
// 			CamPos.X = -CamPos.X + 128.0f;
// 			float Mvx = fmod(CamPos.X, 128.0f);
// 			float Sectors = (CamPos.X - Mvx) / 128.0f;
// 
// 			MySectorX -= ((int)Sectors);
// 			CamPos.X = Sectors * 128.0f;
// 			Modded = true;
// 		}else
// 		{
// 			CamPos.X = 0;
// 		}
// 
// 		if(CamPos.Z > 128.0f)
// 		{
// 			float Mvz = fmod(CamPos.Z, 128.0f);
// 			float Sectors = (CamPos.Z - Mvz) / 128.0f;
// 
// 			MySectorZ += (int)Sectors;
// 			CamPos.Z = -Sectors * 128.0f;
// 			Modded = true;
// 		}else if(CamPos.Z < 0.0f)
// 		{
// 			CamPos.Z = -CamPos.Z + 128.0f;
// 			float Mvz = fmod(CamPos.Z, 128.0f);
// 			float Sectors = (CamPos.Z - Mvz) / 128.0f;
// 
// 			MySectorZ -= ((int)Sectors);
// 			CamPos.Z = Sectors * 128.0f;
// 			Modded = true;
// 		}
// 		else
// 		{
// 			CamPos.Z = 0;
// 		}
// 
// 		if(KeyHit(57))
// 		{
// 			MySectorX = 63;
// 			MySectorZ = 63;
// 			ReloadTerrains = true;
// 			//PositionEntity(Grid, (float)(-MySectorX * 128), 0, (float)(-MySectorZ * 128));
// 		}
// 
// 		
// 		if(Modded == true)
// 		{
// 			TranslateEntity(Camera->Handle, CamPos.X, 0, CamPos.Z);
// 
// 			//PositionEntity(Grid, (float)(-MySectorX * 128), 0, (float)(-MySectorZ * 128));
// 		}
// 
// 		AppTitle(std::toString(MySectorX) + ", " + std::toString(MySectorZ));
		
		//PositionEntity(SkyEN, Camera->Position.X, Camera->Position.Y - 0, Camera->Position.Z);


		//int InputUpdateTime = StartDebugTimer("Input Update");
		BBWaitEvent(1);
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		//StopDebugTimer(InputUpdateTime);

		//int UpdateTime = StartDebugTimer("World Update");

		uint TerrainUpdateStartTime = timeGetTime();

		MySectorX = 0;
		MySectorZ = 0;
		float PosX = Camera->Position.X;
		float PosY = Camera->Position.Y;
		float PosZ = Camera->Position.Z;
		while(PosX > 128.0f)
		{
			MySectorX++;
			PosX -= 128.0f;
		}

		while(PosZ > 128.0f)
		{
			MySectorZ++;
			PosZ -= 128.0f;
		}

		DebugLabels[1]->Text = string("T Cam Pos: ") + Vector2(MySectorX, MySectorZ).ToString() + " / " + Vector3(PosX, PosY, PosZ).ToString();
		DebugLabels[2]->Text = string("A Cam Pos: ") + Camera->Position.ToString();
		

		TerrainManager->Update(Camera->Position, false);
		//TerrainManager->Update(MySectorX, MySectorZ, Vector3(PosX, PosY, PosZ), true, ReloadTerrains);

		uint TerrainUpdateTime = ((uint)timeGetTime()) - TerrainUpdateStartTime;

		ReloadTerrains = false;
		UpdateWorld();
		//UpdateCloth();

		if( KeyHit(19) )
		{
			if( cars.back() )
				cars.back()->ReOrient( NGin::Math::Vector3(0, 20, 0), NGin::Math::Vector3( 30, 0, 0) );
		}

		/*for(uint i = 0; i < cars.size() - 1; ++i)
		{
			cars[i]->Update(false, 0);
		}

		if( cars.size() > 0 && cars.back() )
			cars.back()->Update(true, Camera->Handle);*/

		/*RCPKit::gVehicleManager->Update( cars.back(), Camera->Handle );


		if( MilliSecs() - lastNetTime > 500 )
		{
			lastNetTime = MilliSecs();

			RealmCrafter::PacketWriter writer;
			cars[1]->WriteLocalPacket( &writer );

			RealmCrafter::PacketReader reader( writer.GetBytes(), writer.GetLength(), false );
			cars[0]->ReadRemotePacket( &reader );
		}*/

		//StopDebugTimer(UpdateTime);

		//int RenderTime = StartDebugTimer("Render");
		RenderWorld();
		//StopDebugTimer(RenderTime);



		//StopDebugTimer(MainFrameTime);
		//EndDebugFrame();

		uint FrameTime = ((uint)timeGetTime()) - FrameStartTime;

		uint FrameTotal = 0;
		uint TerrainRenderTotal = 0;
		uint TerrainUpdateTotal = 0;

		for(int i = 0; i < 99; ++i)
		{
			FrameAvg[i] = FrameAvg[i + 1];
			TerrainRenderAvg[i] = TerrainRenderAvg[i + 1];
			TerrainUpdateAvg[i] = TerrainUpdateAvg[i + 1];

			FrameTotal += FrameAvg[i];
			TerrainRenderTotal += TerrainRenderAvg[i];
			TerrainUpdateTotal += TerrainUpdateAvg[i];
		}

		FrameAvg[99] = FrameTime;
		TerrainRenderAvg[99] = TerrainRenderTime;
		TerrainUpdateAvg[99] = TerrainUpdateTime;

		FrameTotal += FrameTime;
		TerrainRenderTotal += TerrainRenderTime;
		TerrainUpdateTotal += TerrainUpdateTime;

		//FrameTotal /= 10;
		//TerrainRenderTotal /= 10;
		//TerrainUpdateTotal /= 10;


		char FTO[1024];
		sprintf(FTO, "%f / %f / %f", ((float)TerrainRenderTotal) / 100.0f, ((float)TerrainUpdateTotal) / 100.0f, ((float)FrameTotal) / 100.0f);
		DebugLabels[0]->Text = string("Frame Times: ") + FTO;
		//AppTitle(FTO, "");

		MoveAvg[9] += CamVel.Length();

		if(MilliSecs() - LastSecCap >= 100)
		{
			LastSecCap = MilliSecs();

			float MoveTotal = 0.0f;
			for(int i = 0; i < 9; ++i)
			{
				MoveTotal += MoveAvg[i];
				MoveAvg[i] = MoveAvg[i + 1];
			}
			MoveTotal += MoveAvg[9];
			MoveAvg[9] = 0.0f;

			//MoveTotal *= 0.1f;

			DebugLabels[3]->Text = string("Speed: ") + toString(MoveTotal) + " UPS";
		}

	}

	exit(0);
	End3DGraphics();


	exit(0);
}
