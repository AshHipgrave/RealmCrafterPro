#include "OOWrapper.h"
#include <BlitzPlus.h>
#include "..\RCT\RCT\Include\ITerrainManager.h"
#include "ITreeRenderer.h"
#include "ITreeManager.h"
#include "..\RCClient\Environment.h"
using namespace RealmCrafter::RCT;
IGUIManager* GUIManager;
ITerrainManager* TerrainManager(0);
LT::ITreeRenderer* TreeRenderer(0);
LT::ITreeManager* TreeManager(0);

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
	if(TerrainManager != 0);// && GFXShadowDetail >= 2)
	{
		TerrainManager->RenderDepth(lightMatrix);
	}
}

void RenderSolid()
{
	// For the upcoming version
	if(TerrainManager != 0 || TreeRenderer != 0)
	{
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

		int Count = 0;
		unsigned int Stride = 0;
		void** LightPtr = bbdx2_GetPointLights(&Count, &Stride);

		if(TerrainManager != 0)
			TerrainManager->SetPointLights(LightPtr, Count, Stride);
		if(TreeRenderer != 0)
			TreeRenderer->SetPointLights(LightPtr, Count, Stride);

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
			TerrainManager->Render((float*)&VP, GetLightMatrix(), GetShadowMap());
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
	}
}


int __stdcall WinMain(HINSTANCE, HINSTANCE, char*, int)
{
	uint hwnd = BBCreateWindow("GAME", 0, 0, 800, 600, 0, 1);
	Init(BBQueryObject(hwnd, 1), 0);
	Graphics3D(800, 600, 32, 2, 0, 0, "Data\\DefaultTex.png");

	IEngine::Collisions(1, 2, 2, 2);
	IEngine::Collisions(2, 1, 2, 2);

	ICamera* Camera = IEngine::CreateCamera();
	Camera->ClearColor = Color(0, 128, 255);
	Camera->Range(1, 1000);
	Camera->FogMode(1);
	Camera->FogColor = Color(255, 255, 255);
	Camera->FogRange(10000, 10000);

	Camera->CollisionType = 1;
	//Camera->CollisionRadius(2);
	EntityRadius(Camera->Handle, 2.0f, 0.1f);

	Camera->Position = Vector3(0, 10, -100);
	Camera->Rotation = Vector3(0, 0, 0);

	//Camera->Position = Vector3(-172.722000, 143.822510, -212.881500);
	//Camera->Rotation = Vector3(26.800003, -40.799980, 0.000000);


	// Ambient Lighting
	IEngine::AmbientLight(Color(0, 0, 0));

	ILight* D = IEngine::CreateLight(ELT_Directional);
	D->Direction = Vector3(1, -1, -1);
	D->Color = Color(255, 255, 255);



	uint Solid = LoadShader("Data\\Game Data\\Shaders\\Default\\LitObject_High.fx");
	uint DepthMapSolid = LoadShader("Data\\Game Data\\Shaders\\Default\\DepthMap.fx");

	IEntity* Sphere = IEngine::LoadMesh("Data\\Sphere.b3d");
	Sphere->Position = Vector3(50, 10, 50);
	Sphere->Scale = Vector3(2.5f, 2.5f, 2.5f);
	Sphere->Shader = Solid;
	EntityShadowLevel(Sphere->Handle, 4);

	// 	uint ShadowPreviewShader = LoadShader("Data\\Shaders\\Default\\ShadowPreview.fx");
	// 	uint ShadowPreview = CreateSAQuad(0);
	// 	EntityShader(ShadowPreview, ShadowPreviewShader);
	// 	SetScale(ShadowPreview, 0.3f, 0.3f);
	// 	EntityOrder(ShadowPreview, -1);

#define R(X, Y) (Math::Vector2(X, Y) / GUIManager->GetResolution())

	GUIManager = CreateGUIManager(GetIDirect3DDevice9(), NGin::Math::Vector2(1024, 768), "Data\\Game Data\\Shaders\\Default\\GUI.fx");

	GUIManager->FontDirectory("Data\\Fonts");
	GUIManager->LoadAndAddSkin("Data\\Skins\\Default\\Theme.xml");
	GUIManager->ImportProperties("Data\\Game Data\\Interface.xml");

	SetRenderGUICallback(0, &RenderGUI);
	SetDeviceLostCallback(0, &DeviceLost);
	SetDeviceResetCallback(0, &DeviceReset);

#undef R



	ShadowShader(DepthMapSolid);
	ShadowBlurShader(
		LoadShader("Data\\Game Data\\Shaders\\Default\\ShadowBlurH.fx"),
		LoadShader("Data\\Game Data\\Shaders\\Default\\ShadowBlurV.fx"));

	// Area which the map effects
	ShadowDistance(256.0f);

	// Depth of the light
	//LightDistance(1000);
	LightDistance(100);

	// Level of shadows to draw
	ShadowLevel(1);

	// Shadow map size determines the overall quality
	SetShadowMapSize(1024);

	// Post Processing
	//LoadUserDefinedPP_FromXML( "Data\\Game Data\\PostProcess\\PostProcess.xml" );
	//SetPP_Pipeline( "Default2" );


	TerrainManager = CreateTerrainManager(GetIDirect3DDevice9(),
		"Data\\Game Data\\Shaders\\Default\\RCT.fx",
		"",
		"Data\\Game Data\\Shaders\\Default\\Grass.fx",
		"Data\\Game Data\\Shaders\\Default\\RCTDepth.fx");

	TerrainManager->LoadGrassTypes("Data\\Game Data\\GrassTypes.xml");
	//TerrainManager->CollisionChanged()->AddEvent(&Terrain_CollisionChanged);
	TerrainManager->SetEditorMode(false);
	SetRenderSolidCallback(0, &RenderSolid);
	SetRenderShadowDepthCallback(0, &RenderDepth);

	ITerrain* T = TerrainManager->CreateT1(128);


	/*Environment = CEnvironment();
	Environment->Setup(Camera->Handle);
	Environment->Load();*/

	// Main Loop
	while(!KeyHit(1) || JoyHit(XBOX_BACK))
	{
		if(KeyDown(17)) Camera->Move(Vector3( 0,  0,  2));
		if(KeyDown(31)) Camera->Move(Vector3( 0,  0, -2));
		if(KeyDown(30)) Camera->Move(Vector3(-1,  0,  0));
		if(KeyDown(32)) Camera->Move(Vector3( 1,  0,  0));

		if(KeyDown(200)) Camera->Turn(Vector3(-0.8f, 0,    0));
		if(KeyDown(208)) Camera->Turn(Vector3( 0.8f, 0,    0));
		if(KeyDown(203)) Camera->Turn(Vector3( 0,    0.8f, 0));
		if(KeyDown(205)) Camera->Turn(Vector3( 0,   -0.8f, 0));

		Camera->Move(Vector3(0, 0, -0.0001f));


		BBWaitEvent(1);
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		TerrainManager->Update(Camera->Position);

		RenderWorld();
		UpdateWorld();
		//Environment->Update(Delta);
	}

	End3DGraphics();


	exit(0);

/*	uint hwnd = BBCreateWindow("GAME", 0, 0, 800, 600, 0, 1);
	Init(BBQueryObject(hwnd, 1), 0);
	Graphics3D(800, 600, 32, 2, 0, 0, "Data\\DefaultTex.png");

	ICamera* Camera = IEngine::CreateCamera();
	Camera->ClearColor = Color(0, 0, 255);
	Camera->Range(1, 1000);
	Camera->FogMode(1);
	Camera->FogColor = Color(255, 255, 255);
	Camera->FogRange(100, 1000);

	Camera->CollisionType = 1;
	Camera->CollisionRadius(2);

	Camera->Position = Vector3(0, 20, 0);
	Camera->Rotation = Vector3(30, -45, 0);

	// Ambient Lighting
	IEngine::AmbientLight(Color(0, 0, 0));

	ILight* D = IEngine::CreateLight(ELT_Directional);
	D->Direction = Vector3(0, -1, 1);
	D->Color = Color(80, 50, 50);

	ILight* D2 = IEngine::CreateLight(ELT_Directional);
	D2->Direction = Vector3(0, -1, 1);
	D2->Color = Color(50, 80, 50);

	ILight* D3 = IEngine::CreateLight(ELT_Directional);
	D3->Direction = Vector3(-1, -1, 0);
	D3->Color = Color(50, 50, 80);

	ILight* P1 = IEngine::CreateLight(ELT_Point);
	P1->Position = Vector3(16, 6, 16);
	P1->Radius = 16.0f;
	P1->Color = Color(0, 0, 255);

	ILight* P2 = IEngine::CreateLight(ELT_Point);
	P2->Position = Vector3(16, 6, 32);
	P2->Radius = 16.0f;
	P2->Color = Color(255, 0, 0);

	ILight* P3 = IEngine::CreateLight(ELT_Point);
	P3->Position = Vector3(32, 6, 16);
	P3->Radius = 16.0f;
	P3->Color = Color(0, 255, 0);

	uint AnimatedMesh = IEngine::LoadShader("Data\\Game Data\\Shaders\\Default\\AnimatedMesh_High.fx");

	IEntity* Human = IEngine::LoadMesh("Data\\Meshes\\Actors\\Lizardman\\lizardman.eb3d", 0, true);
	Human->Shader = AnimatedMesh;
	Human->Scale = Vector3(0.1f, 0.1f, 0.1f);
	Human->Position = Vector3(24.0f, 0.0f, 24.0f);

	uint Seq = Human->ExtractSequence(1, 34);
	Human->Animate(1, 1.0f, Seq);

	LoadUserDefinedPP_FromXML( "Data\\Game Data\\PostProcess\\PostProcess.xml" );
	SetPP_Pipeline( "Default2" );


	ITerrainManager* TerrainManager = CreateTerrainManager(GetIDirect3DDevice9(), 
		"Data\\Game Data\\Shaders\\Default\\RCT.fx",
		"",
		"Data\\Game Data\\Shaders\\Default\\Grass.fx",
		"Data\\Game Data\\Shaders\\Default\\RCTDepth.fx");
//	TerrainManager->LoadGrassTypes("Data\\Game Data\\GrassTypes.xml");
//	TerrainManager->SetEditorMode(false);


	float t = clock();
	float TurnSpeed = 80;
	float MoveSpeed = 100;
	// Main Loop
	while(!KeyHit(1))
	{
		BBWaitEvent(0);
		float dt = (clock()-t)/1000.0f;
		t = clock();


		if(KeyDown(17)) Camera->Move(Vector3( 0,  0,  MoveSpeed*dt));
		if(KeyDown(31))	Camera->Move(Vector3( 0,  0, -MoveSpeed*dt));
		if(KeyDown(30)) Camera->Move(Vector3(-MoveSpeed*dt,  0,  0));
		if(KeyDown(32)) Camera->Move(Vector3( MoveSpeed*dt,  0,  0));

		if(KeyDown(200)) Camera->Turn(Vector3(-TurnSpeed*dt, 0,    0));
		if(KeyDown(208)) Camera->Turn(Vector3( TurnSpeed*dt, 0,    0));
		if(KeyDown(203)) Camera->Turn(Vector3( 0,    TurnSpeed*dt, 0));
		if(KeyDown(205)) Camera->Turn(Vector3( 0,   -TurnSpeed*dt, 0));

		RenderWorld();
	}

	Human->Free(true);
	End3DGraphics();


	exit(0);*/
}