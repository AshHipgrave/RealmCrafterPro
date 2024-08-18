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
#include <ITerrainManager.h>
#include "time.h"


using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;
#pragma comment(lib, "NGUI.lib")

using namespace RealmCrafter::RCT;

IDirect3D9* D3DObject;
IDirect3DDevice9* D3DDevice;
IGUIManager* GUIManager;
D3DXVECTOR3 CameraPosition, CameraRotation, CameraDirection;
D3DXMATRIX CameraProjection, CameraView, CameraViewProjection;
void UpdateCamera();
int Width, Height;

ILabel* TCountLabel, *DrawnCountLabel, *CulledCountLabel;

ITerrainManager* TerrainManager;
ITerrain* Terrain;

// Editor GUI
IWindow* EditorWindow;
ILabel* RadiusLabel, *HardnessLabel;
ITrackBar* RadiusTrackBar, *HardnessTrackBar;
ICheckBox* CircleCheckBox;

void RadiusTrackBar_ValueChanged(IControl* Sender, EventArgs* E)
{
	float Value = (float)RadiusTrackBar->Value;
	RadiusLabel->Text = WString("Radius: ") + Value;
	TerrainManager->SetBrushRadius(Value);
}

void HardnessTrackBar_ValueChanged(IControl* Sender, EventArgs* E)
{
	float Value = ((float)HardnessTrackBar->Value) / 100.0f;
	HardnessLabel->Text = WString("Hardness: ") + Value;
	TerrainManager->SetBrushHardness(Value);
}

void CircleCheckBox_CheckedChange(IControl* Sender, EventArgs* E)
{
	TerrainManager->SetBrushIsCircle(CircleCheckBox->Checked);
}

/*
TODO:

[07] Clean up code (Namespaces!)
[07] Generate DLL
[08] Check compatibility with BBDX
   - Initial rendering callbacks
   - Add BBDX commands to get point lights
[10] Add Collision code
   [09] Provide terrainmanager with a callback
   [09?] Terrain/Chunks will be able to generate a trianglelist (without indexing)
   [10] Step 1 - Pass all lists to callback for testing
   [10] Step 2 - Pass closest lists to callback, old lists can be destroyed
   [10] Test picking on the terrain with the mouse (Potential to try editor brush)
[11] Add hole generation
   - Find a method to discard triangles (but keep to efficiency)
   - Actually discard triangles from collision callback
[12] Add world transformations
[13] Add fog
[15] Write RCT.NET wrapper
   - Include basic serial code to prevent thieves
   - Use NGUINet wrapper as a base to save time
* Write C# testing application
   - Use NGUINet testing application to save time
   - Add side panel and embed a panel (so the panel can be copied)
   - Add required components for each edit mode
   - Add each transform tool and test
* Writing Saving/Loading code
* Write Plugin base
* Write L3DT Plugin
* Write RCTE Plugin (for old RCTE terrains)
* Write Heightmap Plugin
* Write Automatic texturing system
* Write a dumbed down autotexturing system for the PM
* Add RCT to GE
* Add RCT saving to GE (client zone format change)
* Add RCT to Client (remove old legacy terrains)
* Remove distance gravity from actors
* Tweak terrain settings to help with view distance
* Create PM converter
* Write documentation
* Party.



Triangle Remove
Local Indexbuffers
GetCollisionMesh
Highlighter
ModTools
World Transform (c'est tres important, don't forget it in the editor shader)
*/

D3DPRESENT_PARAMETERS PresParams;

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
	//PresParams.PresentationInterval = D3DPRESENT_INTERVAL_IMMEDIATE;

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

	TCountLabel = GUIManager->CreateLabel("TCountLabel", Math::Vector2(20, 50) / GUIManager->GetResolution(), Math::Vector2(0, 0));
	DrawnCountLabel = GUIManager->CreateLabel("DCountLabel", Math::Vector2(20, 70) / GUIManager->GetResolution(), Math::Vector2(0, 0));
	CulledCountLabel = GUIManager->CreateLabel("CCountLabel", Math::Vector2(20, 90) / GUIManager->GetResolution(), Math::Vector2(0, 0));
	
	TCountLabel->Text = "Terrain Triangles: ";
	DrawnCountLabel->Text = "Drawn Tiles: ";
	CulledCountLabel->Text = "Culled Tiles: ";

	TCountLabel->ForeColor = Math::Color(255, 255, 255);
	DrawnCountLabel->ForeColor = Math::Color(255, 255, 255);
	CulledCountLabel->ForeColor = Math::Color(255, 255, 255);

	EditorWindow = GUIManager->CreateWindow("EditorWindow", Math::Vector2(1024 - 226, 50) / GUIManager->GetResolution(), Math::Vector2(226, 198) / GUIManager->GetResolution());

	RadiusLabel = GUIManager->CreateLabel("RadiusLabel", Math::Vector2(12, 9) / GUIManager->GetResolution(), Vector2(0, 0));
	HardnessLabel = GUIManager->CreateLabel("HardnessLabel", Math::Vector2(12, 73) / GUIManager->GetResolution(), Vector2(0, 0));

	RadiusTrackBar = GUIManager->CreateTrackBar("RadiusTrackBar", Math::Vector2(12, 25) / GUIManager->GetResolution(), Vector2(182, 45) / GUIManager->GetResolution());
	HardnessTrackBar = GUIManager->CreateTrackBar("HardnessTrackBar", Math::Vector2(12, 89) / GUIManager->GetResolution(), Vector2(182, 45) / GUIManager->GetResolution());
	
	CircleCheckBox = GUIManager->CreateCheckBox("CircleCheckBox", Math::Vector2(12, 140) / GUIManager->GetResolution(), Vector2(80, 17) / GUIManager->GetResolution());

	EditorWindow->Text = "Terrain Editor";
	EditorWindow->CloseButton = false;

	RadiusLabel->Parent = EditorWindow;
	HardnessLabel->Parent = EditorWindow;
	RadiusTrackBar->Parent = EditorWindow;
	HardnessTrackBar->Parent = EditorWindow;
	CircleCheckBox->Parent = EditorWindow;

	HardnessTrackBar->Maximum = 100;
	RadiusTrackBar->Maximum = 100;

	CircleCheckBox->Checked = true;
	CircleCheckBox->Text = "Circle Brush";

	RadiusTrackBar->ValueChanged()->AddEvent(&RadiusTrackBar_ValueChanged);
	HardnessTrackBar->ValueChanged()->AddEvent(&HardnessTrackBar_ValueChanged);
	CircleCheckBox->CheckedChange()->AddEvent(&CircleCheckBox_CheckedChange);


	// Reset the camera
	CameraPosition = D3DXVECTOR3(0, 10, -30);
	CameraRotation = D3DXVECTOR3(0,0,0);//-45, 0, 0);
	UpdateCamera();

	TerrainManager = CreateTerrainManager(D3DDevice, "RCT.fx", "RCTEditor.fx", "Grass.fx", "RCTDepth.fx");
	TerrainManager->SetAmbientLight(Color(0.0f, 0.0f, 0.0f));
	TerrainManager->SetLightNormal(0, Vector3(0, -1.0f, 0));
	//TerrainManager->SetLightNormal(1, Vector3(0, -0.1f, 1));
	//TerrainManager->SetLightNormal(2, Vector3(-1, -0.1f, 0));
	TerrainManager->SetLightColor(0, Color(1.0f, 1.0f, 1.0f));
	//TerrainManager->SetLightColor(1, Color(0.0f, 1.0f, 0.0f));
	//TerrainManager->SetLightColor(2, Color(0.0f, 0.0f, 1.0f));

	//ArrayList<Light*> Lights;
	//for(int x = 0; x < 8; ++x)
	//	for(int z = 0; z < 8; ++z)
	//	{
	//		float r = ((float)rand()) / ((float)RAND_MAX) * 0.7f;
	//		float g = ((float)rand()) / ((float)RAND_MAX) * 0.7f;
	//		float b = ((float)rand()) / ((float)RAND_MAX) * 0.7f;
	//		r += 0.3f;
	//		g += 0.3f;
	//		b += 0.3f;
	//		Lights.Add(new Light(Vector3(r, g, b), Vector3((x * 16) + 8, 6, (z * 16) + 8), 10, true));
	//	}
	//TerrainManager->SetPointLights((void**)Lights.Pointer(), Lights.Size(), sizeof(Light));

	ITerrain* T = TerrainManager->CreateT1(128);
	Terrain = T;

	T->SetTexture(0, std::string("Media\\Terrain\\Texs\\0.png"));
	T->SetTexture(1, std::string("Media\\Terrain\\Texs\\1.png"));
	T->SetTexture(2, std::string("Media\\Terrain\\Texs\\2.png"));
	T->SetTexture(3, std::string("Media\\Terrain\\Texs\\4.png"));
	T->SetTexture(4, std::string("Media\\Terrain\\Texs\\3.png"));

	T->SetTextureScale(Vector2(6, 6));

	for(int z = 0; z < 128; ++z)
	for(int x = 0; x < 128; ++x)
	{
		float zo = ((float)rand()) / ((float)RAND_MAX);
		T->SetHeight(x, z, 0.0f);//zo * 5.0f);
		
	}

	Transform_Raise(T, 20.0f, 0.01f, NGin::Math::Vector2(64.0f, 64.0f), true, 6.0f);
		//T->SetHeight(x, z, -1.0f);

	////T->SetHeight(31, 16, 10.0f);

	RadiusTrackBar->Value = 10;
	HardnessTrackBar->Value = 10;

	RadiusTrackBar_ValueChanged(RadiusTrackBar, 0);
	HardnessTrackBar_ValueChanged(HardnessTrackBar, 0);
	CircleCheckBox_CheckedChange(CircleCheckBox, 0);

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

	// Update controls
	if(KeyDown(VK_ESCAPE))
	{
		DNR = true;

		delete TerrainManager;
		delete GUIManager;

		PostQuitMessage(0);
		return;
	}

	if(KeyDown('R'))
	{
		GUIManager->OnDeviceLost();
		TerrainManager->OnDeviceLost();

		D3DDevice->Reset(&PresParams);

		GUIManager->OnDeviceReset(Vector2(1024, 768));
		TerrainManager->OnDeviceReset();

		unsigned int T = clock();
		while(clock() - T < 2000);
	}

	// Camera Position
	if(KeyDown('W'))
		CameraPosition += CameraDirection;
	if(KeyDown('S'))
		CameraPosition -= CameraDirection;

	D3DXVECTOR3 Crossed, Up;
	Up = D3DXVECTOR3(0, 1, 0);
	MyD3DXVectorCross(&Crossed, &CameraDirection, &Up);

	if(KeyDown('A'))
		CameraPosition += Crossed;
	if(KeyDown('D'))
		CameraPosition -= Crossed;

	// Camera Pitch/Yaw
	if(KeyDown(VK_UP))
		CameraRotation.x += 0.5f;
	if(KeyDown(VK_DOWN))
		CameraRotation.x -= 0.5f;
	
	if(KeyDown(VK_LEFT))
		CameraRotation.y -= 0.5f;
	if(KeyDown(VK_RIGHT))
		CameraRotation.y += 0.5f;

	if(KeyDown(VK_SPACE))
	{
		//Transform_Raise(Terrain,
		//	TerrainManager->GetBrushRadius(),
		//	TerrainManager->GetBrushHardness(),
		//	TerrainManager->GetBrushPosition(),
		//	TerrainManager->GetBrushIsCircle(),
		//	10.0f);
		Transform_Paint(Terrain,
			TerrainManager->GetBrushRadius(),
			TerrainManager->GetBrushHardness(),
			TerrainManager->GetBrushPosition(),
			TerrainManager->GetBrushIsCircle(),
			0.1f,
			1);
		unsigned int T = clock();
		while(clock() - T < 100);
	}


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
	TerrainManager->Update(NGin::Math::Vector3(CameraPosition.x, CameraPosition.y, CameraPosition.z));

	// Clear the screen
	D3DDevice->Clear(0, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, /*0xff6495ed*/ 0xff0000c0, 1.0f, 0);

	// Begin the rendering
	if( !SUCCEEDED(D3DDevice->BeginScene()))
		return;

	D3DXMATRIX World;
	D3DXMatrixIdentity(&World);

	D3DXMATRIX WVP;
	D3DXMatrixMultiply(&WVP, &World, &CameraViewProjection);


	FrameStatistics Stats = TerrainManager->Render((float*)&CameraViewProjection);



	unsigned int TCount = Stats.TrianglesDrawn;
	unsigned int DCount = Stats.ChunksDrawn;
	unsigned int CCount = Stats.ChunksCulled;

	TCountLabel->Text = WString("Terrain Triangles: ") + WString(TCount);
	DrawnCountLabel->Text = WString("Drawn Tiles: ") + WString(DCount);
	CulledCountLabel->Text = WString("Culled Tiles: ") + WString(CCount);
	GUIManager->Render();

	// End the rendering
	D3DDevice->EndScene();
	D3DDevice->Present(NULL, NULL, NULL, NULL);
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