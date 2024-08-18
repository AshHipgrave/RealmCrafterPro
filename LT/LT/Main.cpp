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
#include "WinMain.h"
#include "MyD3DX.h"
#include <IGUIManager.h>

#include "ITreeManager.h"
#include "Source\\CD3D9TreeRenderer.h"

#include <ITerrainManager.h>
#include <Color.h>

using namespace RealmCrafter::RCT;
using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;
#pragma comment(lib, "NGUI.lib")

namespace std
{
	// Trim values using a bit of work (grr STL)
	inline std::string trim(const std::string &str)
	{
		std::string OutStr = "";
		size_t TrimStart = str.find_first_not_of(" \t");
		size_t TrimEnd = str.find_last_not_of(" \t");
		if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
			OutStr = "";
		else
			OutStr = str.substr(TrimStart, TrimEnd - TrimStart + 1);

		return OutStr;
	}

	// Convert a string to a float
	inline float toFloat(const std::string &str)
	{
		return atof(str.c_str());
	}

	// Convert a string to an integer
	inline int toInt(const std::string &str)
	{
		return atoi(str.c_str());
	}

	// Mini converter class
	class miniConverter
	{
		std::string Output;
	public:

		miniConverter(std::string &str) { Output = str; }
		miniConverter(int i) { char T[32]; sprintf(T, "%i", i); Output = T; }
		miniConverter(unsigned int i) { char T[32]; sprintf(T, "%u", i); Output = T; }
		miniConverter(float i) { char T[32]; sprintf(T, "%f", i); Output = T; }
		miniConverter(double i) { char T[32]; sprintf(T, "%L", i); Output = T; }
		miniConverter(unsigned long i) { char T[32]; sprintf(T, "%u", i); Output = T; }

		std::string str() { return Output; }
	};

	// Convert an integer to a string
	template <typename T>
	inline std::string toString(T in)
	{
		return miniConverter(in).str();
	}

	// Convert a string to lower case
	inline std::string stringToLower(const std::string &str)
	{
		std::string Out = str;
		std::transform(Out.begin(), Out.end(), Out.begin(), ::tolower);
		return Out;
	}

	// Convert a string to upper case
	inline std::string stringToUpper(const std::string &str)
	{
		std::string Out = str;
		std::transform(Out.begin(), Out.end(), Out.begin(), ::toupper);
		return Out;
	}
}

using namespace std;

IGUIManager* GUIManager;

class Monitor
{
	ILabel* DebugLabel;
	std::string Name;
	int StartTime;
	int Last[20];

public:

	int Time;

	Monitor(std::string name, int x, int y)
	{
		Name = name;
		StartTime = 0;

		memset(Last, 0, 20 * sizeof(int));
		DebugLabel = GUIManager->CreateLabel("Monitor", NGin::Math::Vector2(x, y) / GUIManager->GetResolution(), NGin::Math::Vector2(0, 0));
		DebugLabel->ForeColor = Color(255, 255, 255);
	}

	void Start()
	{
		StartTime = timeGetTime();
	}

	void Stop()
	{
		int Time = timeGetTime() - StartTime;

		for(int i = 18; i >= 0; --i)
			Last[i + 1] = Last[i];

		Last[0] = Time;

		for(int i = 1; i < 20; ++i)
			Time += Last[i];

		Time /= 20;

		DebugLabel->Text = Name + toString(Time);
	}
};

IDirect3D9* D3DObject;
IDirect3DDevice9* D3DDevice;
ITerrainManager* TerrainManager;
ITerrain* Terrain;

LT::ITreeManager* TreeManager;
LT::CD3D9TreeRenderer* TreeRenderer;
LT::ITreeInstance* Instance;
std::vector<LT::ITreeInstance*> Instances;
LT::ITreeZone* Zone;
LT::ITreeType* Tree1Type;

unsigned int ApproxTrees = 0;

D3DXVECTOR3 CameraPosition, CameraRotation, CameraDirection;
D3DXMATRIX CameraProjection, CameraView, CameraViewProjection;
void UpdateCamera();
int Width, Height;
D3DPRESENT_PARAMETERS PresParams;

ILabel* Debug1;
ILabel* Debug2;
ILabel* Debug3;
ILabel* Debug4;
ILabel* Debug5;
ILabel* Debug6;
ILabel* Debug7;
ILabel* Debug8;
ILabel* Debug9;
ILabel* Debug10;

struct Light
{
	Vector3 Position;
	NGin::Math::Color Color;
	int Radius;
	bool Active;
	float DistSQ;

	Light(Vector3 color, Vector3 position, float radius, bool active)
	{
		this->Color = NGin::Math::Color(color.X, color.Y, color.Z);
		Position = position;
		Radius = radius;
		Active = active;
	}
};

ID3DXMesh* Aeroplane = 0;
ID3DXEffect* DiffuseTextureEffect = 0;

Monitor* TotalMon;
Monitor* UpdateMon;
Monitor* RenderMon;

struct NPointLight
{
	Vector3 Position;
	Color Color;
	int Radius;
	bool Active;
	float NUDistance;

	NPointLight(Vector3 &position, Math::Color &color, float radius)
	{
		Set(position, color, radius);
	}

	void Set(Vector3 &position, Math::Color &color, float radius)
	{
		Position = position;
		Color = color;
		Radius = radius;
		Active = true;
		NUDistance = 0.0f;
	}
};



bool OnDeviceSetup(HWND Hwnd)
{
	// Create the D3D Object;
	if ((D3DObject = Direct3DCreate9(D3D_SDK_VERSION)) == NULL)
		return false;

	Width = 1024;
	Height = 768;

	RECT Size;
	SetRect(&Size, 0, 0, Width, Height);
	AdjustWindowRect(&Size, WS_OVERLAPPEDWINDOW, false);

	int iWidth = Size.right - Size.left;
	int iHeight = Size.bottom - Size.top;

	SetWindowPos(Hwnd, 0, GetSystemMetrics(0) / 2 - (iWidth / 2), GetSystemMetrics(1) / 2 - (iHeight / 2), iWidth, iHeight, 0);

	// Setup the device parameters
	ZeroMemory(&PresParams, sizeof(PresParams));
	PresParams.Windowed					= TRUE;
	PresParams.SwapEffect				= D3DSWAPEFFECT_DISCARD;
	PresParams.BackBufferCount			= 0;
	PresParams.BackBufferFormat			= D3DFMT_UNKNOWN;
	PresParams.BackBufferWidth			= Width;
	PresParams.BackBufferHeight			= Height;
	PresParams.EnableAutoDepthStencil	= TRUE;
	PresParams.AutoDepthStencilFormat	= D3DFMT_D24S8;
	PresParams.PresentationInterval = D3DPRESENT_INTERVAL_IMMEDIATE;

	bool NVPerfHud = false;
	for (UINT Adapter=0;Adapter<D3DObject->GetAdapterCount();Adapter++)
	{
		D3DADAPTER_IDENTIFIER9 Identifier;
		HRESULT Res;
		Res = D3DObject->GetAdapterIdentifier(Adapter,0,&Identifier);
		if (strstr(Identifier.Description,"PerfHUD") != 0)
		{
			HRESULT hr = D3DObject->CreateDevice(Adapter, D3DDEVTYPE_REF, Hwnd,
				/*fpuPrecision |*/ D3DCREATE_HARDWARE_VERTEXPROCESSING, &PresParams, &D3DDevice);
			NVPerfHud = true;
			//MessageBox(0, "Using Perf", "", 0);

			if(FAILED(hr))
			{
				MessageBox(0, "PerfHUD Detected but isn't available", "Error", MB_OK | MB_ICONERROR);
				exit(0);
			}

			break;
		}
	}

	if(!NVPerfHud)
	{
		// Create the device
		if (FAILED(D3DObject->CreateDevice(D3DADAPTER_DEFAULT,
			D3DDEVTYPE_HAL,
			Hwnd,
			D3DCREATE_HARDWARE_VERTEXPROCESSING,
			&PresParams,
			&D3DDevice)))
			return false;
	}

	SetCursor(NULL);
	GUIManager = CreateGUIManager(D3DDevice, NGin::Math::Vector2(1024, 768), "Media\\GUI\\GUI.fx");//GetGUIManager();

	GUIManager->FontDirectory("Media\\GUI\\Fonts");
	GUIManager->LoadAndAddSkin("Media\\GUI\\Skins\\Default\\Theme.xml");

	Debug1 = GUIManager->CreateLabel("Debug1", Vector2(50, 50) / GUIManager->GetResolution(), Vector2(1, 1));
	Debug2 = GUIManager->CreateLabel("Debug2", Vector2(50, 64) / GUIManager->GetResolution(), Vector2());
	Debug3 = GUIManager->CreateLabel("Debug3", Vector2(50, 78) / GUIManager->GetResolution(), Vector2());
	Debug4 = GUIManager->CreateLabel("Debug4", Vector2(50, 92) / GUIManager->GetResolution(), Vector2());
	Debug5 = GUIManager->CreateLabel("Debug5", Vector2(50, 104) / GUIManager->GetResolution(), Vector2());
	Debug6 = GUIManager->CreateLabel("Debug6", Vector2(50, 118) / GUIManager->GetResolution(), Vector2());
	Debug7 = GUIManager->CreateLabel("Debug7", Vector2(50, 132) / GUIManager->GetResolution(), Vector2());
	Debug8 = GUIManager->CreateLabel("Debug8", Vector2(50, 146) / GUIManager->GetResolution(), Vector2());
	Debug9 = GUIManager->CreateLabel("Debug9", Vector2(50, 160) / GUIManager->GetResolution(), Vector2());
	Debug10 = GUIManager->CreateLabel("Debug10", Vector2(50, 174) / GUIManager->GetResolution(), Vector2());

	Debug1->Text = "Debug Label!";
	Debug2->Text = ":";
	Debug3->Text = ":";
	Debug4->Text = ":";
	Debug5->Text = ":";
	Debug6->Text = "";
	Debug7->Text = "";
	Debug8->Text = "";
	Debug9->Text = "";
	Debug10->Text = "";

	Debug1->ForeColor = Color(255, 255, 255);
	Debug2->ForeColor = Color(255, 255, 255);
	Debug3->ForeColor = Color(255, 255, 255);
	Debug4->ForeColor = Color(255, 255, 255);
	Debug5->ForeColor = Color(255, 255, 255);
	Debug6->ForeColor = Color(255, 255, 255);
	Debug7->ForeColor = Color(255, 255, 255);
	Debug8->ForeColor = Color(255, 255, 255);
	Debug9->ForeColor = Color(255, 255, 255);
	Debug10->ForeColor = Color(255, 255, 255);

	TreeRenderer = LT::CD3D9TreeRenderer::CreateDirect3D9TreeRenderer(D3DDevice,
		"Tree\\Trunk.fx", "Tree\\Leaf.fx", "Tree\\LOD.fx");
	if(TreeRenderer == 0)
	{
		MessageBox(0, "Failed to start tree renderer!", "", 0);
		exit(0);
	}

	TreeManager = LT::ITreeManager::CreateTreeManager(TreeRenderer);

	timeBeginPeriod(1);
	TotalMon = new Monitor("Total: ", 50, 130);
	UpdateMon = new Monitor("Update: ", 50, 144);
	RenderMon = new Monitor("Render: ", 50, 158);

	Tree1Type = TreeManager->LoadTreeType("Tree\\tree1.lt");
	if(Tree1Type == 0)
		MessageBox(0, "Failed to load type!", "", 0);

	Zone = TreeManager->CreateZone();
	//LT::ITreeInstance* Tree1_1 = Zone->AddTreeInstance(Tree1Type, Vector3(0, 0, 50));
	//LT::ITreeInstance* Tree1_2 = Zone->AddTreeInstance(Tree1Type, Vector3(0, 0, 100));
	//LT::ITreeInstance* Tree1_3 = Zone->AddTreeInstance(Tree1Type, Vector3(0, 0, 150));
	//LT::ITreeInstance* Tree1_4 = Zone->AddTreeInstance(Tree1Type, Vector3(0, 0, 200));
	//LT::ITreeInstance* Tree1_5 = Zone->AddTreeInstance(Tree1Type, Vector3(0, 0, 250));

	//Tree1_5->SetPosition(Vector3(100, 0, 100));
	//Tree1_5->SetScale(Vector3(0.1, 0.1, 0.1));

	Zone->Lock();

	srand(timeGetTime());

 	for(float z = 0; z < 500.0f; z += 20.0f)
 	{
 		for(float x = 0; x < 500.0f; x += 20.0f)
 		{
			float RndX = ((float)rand()) / ((float)RAND_MAX);
			float RndY = ((float)rand()) / ((float)RAND_MAX);
			RndX *= 500.0f;
			RndY *= 500.0f;

			Instance = Zone->AddTreeInstance(Tree1Type, Vector3(RndX, 0, RndY));
			Instance->SetScale(Vector3(0.1f, 0.1f, 0.1f));
			++ApproxTrees;
			Instances.push_back(Instance);
 		}
 	}

	Zone->Unlock();

	//LT::ITreeInstance* Instance = Zone->AddTreeInstance(Tree1Type, Vector3(0, 0, 0));
	//Instance->SetScale(Vector3(0.1f, 0.1f, 0.1f));

	//Instance = Zone->AddTreeInstance(Tree1Type, Vector3(430, 0, 430));
	//Instance->SetScale(Vector3(0.1f, 0.1f, 0.1f));
	//Instance = Zone->AddTreeInstance(Tree1Type, Vector3(410, 0, 410));
	//Instance->SetScale(Vector3(0.1f, 0.1f, 0.1f));
	//Instance = Zone->AddTreeInstance(Tree1Type, Vector3(420, 0, 420));
	//Instance->SetScale(Vector3(0.1f, 0.1f, 0.1f));

	//TreeManager->SaveZone(Zone, "TestZone.ltz");

//	LT::ITreeZone* Zone = TreeManager->LoadZone("TestZone.ltz");



	TerrainManager = CreateTerrainManager(D3DDevice,
		"RCT\\RCT.fx", "", "RCT\\Grass.fx", "RCT\\RCTDepth.fx");

	TerrainManager->LoadGrassTypes("RCT\\GrassTypes.xml");
	TerrainManager->SetEditorMode(false);
	TerrainManager->SetGrassDrawDistance(80.0f);
	
	Terrain = TerrainManager->CreateT1(512);
	Terrain->SetTexture(4, std::string("RCT\\1.png"));
	Terrain->SetTextureScale(4, 1.0f / 20.0f);
	Terrain->SetGrassType(0, "Base Grass");
	Transform_PaintGrass(Terrain, 1000, Vector2(100, 100), false, 1, true);


	NPointLight** Lights = new NPointLight*[64];
	for(int x = 0; x < 8; ++x)
		for(int y = 0; y < 8; ++y)
			Lights[x + (y * 8)] = new NPointLight(Vector3(x * 32, 8, y * 32) + Vector3(16, 0, 16), Color(((float)rand()) / RAND_MAX, ((float)rand()) / RAND_MAX, ((float)rand()) / RAND_MAX), 50.0f);
	
// 	for(int i = 0; i < 64; ++i)
// 	{
// 		char DBO[2048];
// 		sprintf(DBO, "%i: %s\n", i, Lights[i]->Position.ToString().c_str());
// 		OutputDebugString(DBO);
// 	}
	
	//TerrainManager->SetPointLights((void**)Lights, 64, sizeof(NPointLight));
	TerrainManager->SetAmbientLight(Color(60, 60, 60));

	TreeManager->GetRenderer()->SetFog(Color(1.0f, 1.0f, 1.0f), Vector2(50000, 50000));
	TreeManager->GetRenderer()->SetAmbientLight(Color(60, 60, 60));
	//TreeManager->GetRenderer()->SetPointLights((void**)Lights, 64, sizeof(NPointLight));

	TreeManager->GetRenderer()->SetLightNormal(0, Vector3(1, -1, -1));
	TerrainManager->SetLightNormal(0, Vector3(1, -1, -1));
	//TreeManager->GetRenderer()->SetLightNormal(1, Vector3(0, -1, 0));
	//TreeManager->GetRenderer()->SetLightNormal(2, Vector3(0, -1, 0));

	TreeManager->GetRenderer()->SetLightColor(0, Color(1.0f, 1.0f, 1.0f));
	TerrainManager->SetLightColor(0, Color(1.0f, 1.0f, 1.0f));
	//TreeManager->GetRenderer()->SetLightColor(1, Color(0.0f, 1.0f, 0.0f));
	//TreeManager->GetRenderer()->SetLightColor(2, Color(0.0f, 0.0f, 1.0f));

	TreeManager->GetRenderer()->SetSwayPower(0.8f);
	Vector2 Dir(1, 1);
	Dir.Normalize();
	TreeManager->GetRenderer()->SetSwayDirection(Dir);

	// Reset the camera
	//CameraPosition = D3DXVECTOR3(0, 10, -30);
	CameraPosition = D3DXVECTOR3(120, 50, 110);
	CameraRotation = D3DXVECTOR3(0,0,0);//-45, 0, 0);
	UpdateCamera();

	return true;
}

void OnDeviceDestroy(HWND Hwnd)
{
	D3DDevice->Release();
	D3DObject->Release();
	return;
}

NGin::Math::Vector2 MousePosition()
{
	POINT CPos;
	RECT WinPos;

	GetCursorPos(&CPos);
	GetWindowRect(Hwnd, &WinPos);

	return Vector2(CPos.x - WinPos.left - 8, CPos.y - WinPos.top - 29);
}




bool DNR = false;
void OnDeviceUpdate()
{
	if(DNR)
		return;

	TotalMon->Start();
	UpdateMon->Start();

	// Update controls
	if(KeyDown(VK_ESCAPE))
	{
		timeEndPeriod(1);
		exit(0);
		return;
	}

	if(KeyDown('R'))
	{
		for(int i = 0; i < Instances.size(); ++i)
		{
			--ApproxTrees;
			Instances[i]->Destroy();
		}
		Instances.clear();
		//if(Instance != 0)
		//	Instance->Destroy();
		//Instance = 0;
		//Sleep(2000);
	}

	if(KeyDown('T'))
	{
		float RndX = ((float)rand()) / ((float)RAND_MAX);
		float RndY = ((float)rand()) / ((float)RAND_MAX);
		RndX *= 500.0f;
		RndY *= 500.0f;

		Instance = Zone->AddTreeInstance(Tree1Type, Vector3(RndX, 0, RndY));
		Instance->SetScale(Vector3(0.1f, 0.1f, 0.1f));
		++ApproxTrees;
		Instances.push_back(Instance);
	}

	if(KeyDown('Y'))
	{
		if(Zone != 0)
			Zone->Destroy();
		Zone = 0;
	}

	float Speed = 0.3f;

	// Camera Position
	if(KeyDown('W'))
		CameraPosition += CameraDirection * Speed;
	if(KeyDown('S'))
		CameraPosition -= CameraDirection * Speed;

	D3DXVECTOR3 Crossed, Up;
	Up = D3DXVECTOR3(0, 1, 0);
	MyD3DXVectorCross(&Crossed, &CameraDirection, &Up);

	if(KeyDown('A'))
		CameraPosition += Crossed * Speed;
	if(KeyDown('D'))
		CameraPosition -= Crossed * Speed;

	// Camera Pitch/Yaw
	if(KeyDown(VK_UP))
		CameraRotation.x += 0.5f * Speed;
	if(KeyDown(VK_DOWN))
		CameraRotation.x -= 0.5f * Speed;
	
	if(KeyDown(VK_LEFT))
		CameraRotation.y -= 0.5f * Speed;
	if(KeyDown(VK_RIGHT))
		CameraRotation.y += 0.5f * Speed;


	if(KeyDown('O'))
		TreeManager->GetRenderer()->SetQualityLevel(0);
	if(KeyDown('P'))
		TreeManager->GetRenderer()->SetQualityLevel(1);
	
	


	GUIUpdateParameters Upd;
	Upd.MousePosition = MousePosition();

	Upd.InputBuffer->CopyFrom(InputBuffer);
	InputBuffer.Empty();
	Upd.LeftDown = LMBDown;
	Upd.RightDown = false;
	Upd.Handled = false;

	Upd.KeyLeft = LeftKey;
	Upd.KeyRight = RightKey;
	Upd.KeyUp = UpKey;
	Upd.KeyDown = DownKey;
	Upd.KeyHome = Home;
	Upd.KeyEnd = End;
	Upd.KeyInsert = Insert;
	Upd.KeyDelete = Delete;

	GUIManager->Update(&Upd);
	Upd.InputBuffer->Empty();
	LeftKey = RightKey = UpKey = DownKey = Home = End = Insert = Delete = false;

	UpdateCamera();
	TreeManager->Update(reinterpret_cast<const float*>(&CameraPosition));
	Vector3 TCameraPosition(CameraPosition.x, CameraPosition.y, CameraPosition.z);
	TerrainManager->Update(TCameraPosition);


	UpdateMon->Stop();
	RenderMon->Start();

	// Clear the screen
	D3DDevice->Clear(0, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, /*0xff6495ed*/ 0xff7ca5e3, 1.0f, 0);

	// Begin the rendering
	if( !SUCCEEDED(D3DDevice->BeginScene()))
		return;

	TerrainManager->Render(reinterpret_cast<float*>(&CameraViewProjection), NULL, NULL);
	TreeManager->Render(
		*(reinterpret_cast<NGin::Math::Matrix*>(&CameraView)),
		*(reinterpret_cast<NGin::Math::Matrix*>(&CameraProjection)),
		*(reinterpret_cast<NGin::Math::Vector3*>(&CameraPosition)));

	Debug1->Text = string("Camera: ") + reinterpret_cast<NGin::Math::Vector3*>(&CameraPosition)->ToString();


	//UINT Passes = 0;
	//DiffuseTextureEffect->Begin(&Passes, NULL);
	//DiffuseTextureEffect->BeginPass(0);

	//DiffuseTextureEffect->SetMatrix("ViewProjection", &CameraViewProjection);
	//
	//D3DXMATRIX World;
	//D3DXMatrixIdentity(&World);
	//DiffuseTextureEffect->SetMatrix("World", &World);

	//DiffuseTextureEffect->CommitChanges();

	//Aeroplane->DrawSubset(0);

	//DiffuseTextureEffect->EndPass();
	//DiffuseTextureEffect->End();


	GUIManager->Render();

RenderMon->Stop();

	// End the rendering
	D3DDevice->EndScene();
	D3DDevice->Present(NULL, NULL, NULL, NULL);

	//Sleep(10);

	
	TotalMon->Stop();

	double TotalFPS = static_cast<double>(TotalMon->Time);
	TotalFPS = 1000.0 / TotalFPS;

	Debug2->Text = string("LT Triangles Drawn: ") + toString(TreeRenderer->GetTrianglesDrawn());
	Debug3->Text = string("LT Trees Loaded (approx): ") + toString(ApproxTrees);
	Debug4->Text = string("LT Trees Drawn: ") + toString(TreeManager->GetTreesDrawn() + TreeManager->GetLODsDrawn());
	Debug5->Text = string("LT Normal Trees Drawn: ") + toString(TreeManager->GetTreesDrawn());
	Debug6->Text = string("LT LOD Trees Drawn: ") + toString(TreeManager->GetLODsDrawn());
}

void UpdateCamera()
{
	// Create a view matrix
	float XL, ZL, YL, XR, YR;
	
	XL = sin(ToRadians(CameraRotation.y)) * 10;
	ZL = cos(ToRadians(CameraRotation.y)) * 10;
	YL = sin(ToRadians(CameraRotation.x)) * 10;
	XR = sin(ToRadians(CameraRotation.z)) * 10;
	YR = cos(ToRadians(CameraRotation.z)) * 10;

	D3DXVECTOR3 LookAt(CameraPosition.x + XL, CameraPosition.y + YL, CameraPosition.z + ZL);
	D3DXVECTOR3 Up(XR, YR, 0.0f);

	D3DXMatrixLookAtLH(&CameraView, &CameraPosition, &LookAt, &Up);

	// Create projection
	D3DXMatrixPerspectiveFovLH(&CameraProjection, D3DX_PI/4, (4.0f / 3.0f), 0.1f, 1000.0f);

	// Create VP
	D3DXMatrixMultiply(&CameraViewProjection, &CameraView, &CameraProjection);

	// Direction
	CameraDirection = LookAt - CameraPosition;
	MyD3DXVectorNormalize(&CameraDirection);
}

void OnWindowPaint(HWND Hwnd, HDC DC){}
void OnWindowCommand(HWND Gadget){}