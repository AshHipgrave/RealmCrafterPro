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
//* BBDX - Global
//*

#include "..\PhysX\PhysXBBDX.h"
#include "BBThread.h"
#include "DebugOverlay.h"
#include <Psapi.h>

// Set Ambient lighting
DLLPRE void DLLEX qwedfy(int r, int g, int b)
{
	((irr::video::CD3D9Driver*)driver)->AmbientLight = SColorf(((float)r) / 255.0f, ((float)g) / 255.0f, ((float)b) / 255.0f);
}

DLLPRE int DLLEX bbdx2_GetAmbientR()
{
	return (int)((irr::video::CD3D9Driver*)driver)->AmbientLight.r * 255.0f;
}

DLLPRE int DLLEX bbdx2_GetAmbientG()
{
	return (int)((irr::video::CD3D9Driver*)driver)->AmbientLight.g * 255.0f;
}

DLLPRE int DLLEX bbdx2_GetAmbientB()
{
	return (int)((irr::video::CD3D9Driver*)driver)->AmbientLight.b * 255.0f;
}


bool CheckDirectXVersion(int Version);
//irr::core::stringc GUIEffectPath = "";
//
//DLLPRE void DLLEX bbdx2_SetGUIEffectPath(const char* Path)
//{
//	GUIEffectPath = Path;
//}
//
//DLLPRE IGUIManager* DLLEX bbdx2_GetGUIManager()
//{
//	return GUIManager;
//}

DLLPRE IDirect3DDevice9* DLLEX bbdx2_GetIDirect3DDevice9()
{
	return ((CD3D9Driver*)driver)->pID3DDevice;
}



DLLPRE void DLLEX bbdx2_SaveLastFrame(const char* path)
{
	IDirect3DSurface9* RT = 0;
	((irr::video::CD3D9Driver*)driver)->pID3DDevice->GetRenderTarget(0, &RT);
	if(RT == 0)
		return;

	D3DXIMAGE_FILEFORMAT Format = D3DXIFF_BMP;

	irr::core::stringc Str = path;
	Str.make_lower();

	if(Str.subString(Str.size() - 3, 3) == "jpg")
		Format = D3DXIFF_JPG;
	if(Str.subString(Str.size() - 3, 3) == "tga")
		Format = D3DXIFF_TGA;
	if(Str.subString(Str.size() - 3, 3) == "png")
		Format = D3DXIFF_PNG;
	if(Str.subString(Str.size() - 3, 3) == "dds")
		Format = D3DXIFF_DDS;

	D3DXSaveSurfaceToFileA(path, Format, RT, NULL, NULL);

	RT->Release();
}

void Realackno(int Width, int Height, int Depth, int Fullscreen, int Antialias, int Anisotropic, irr::c8* DefaultTex);
DLLPRE void DLLEX ackno(int Width, int Height, int Depth, int Fullscreen, int Antialias, int Anisotropic, irr::c8* DefaultTex)
{
	SEHSTART;
	
	Realackno(Width, Height, Depth, Fullscreen, Antialias, Anisotropic, DefaultTex);

	SEHEND;
}

void BBFatalError(const char* error)
{
	MessageBox(NULL, error, "Fatal Error", MB_OK | MB_ICONERROR);
	exit(0);
}

// Graphics3D - Setup!
void Realackno(int Width, int Height, int Depth, int Fullscreen, int Antialias, int Anisotropic, irr::c8* DefaultTex)
{
	if(!CheckDirectXVersion(41))
	{
		MessageBox(0, "Error: Please update DirectX!", "Runtime Error", MB_OK | MB_ICONERROR);
		exit(0);
	}

	if(Fullscreen == 2)
	{
		// Set Window Position - Update Later!
	    RECT Size, Desktop;
		GetWindowRect(GetDesktopWindow(), &Desktop);
		SetRect(&Size, 0, 0, Width, Height);
		AdjustWindowRect(&Size, WS_OVERLAPPED | WS_CAPTION, false);
		int X, Y, W, H;
	
		W = (Size.right - Size.left);
		H = (Size.bottom - Size.top);
		X = Desktop.right / 2 - W / 2;
		Y = Desktop.bottom / 2 - H / 2;
		SetWindowPos(hWnd, 0, X, Y, W, H, 0);
	}

	irr::SIrrlichtCreationParameters param;

	if(driver != 0)
	{
		screenWidth = Width;
		screenHeight = Height;
		((video::CD3D9Driver*)driver)->present.hDeviceWindow = hWnd;
		driver->ResizeScreen(irr::core::dimension2d<irr::s32>(Width, Height), Fullscreen == 1 ? true : false, Antialias, Anisotropic);
		return;
	}

	param.WindowSize = irr::core::dimension2d<irr::s32>(Width, Height);

	// Fullscreen
	if(Fullscreen == 1)
	    param.Fullscreen = true;

	// Set the window handle
	param.WindowId = reinterpret_cast<irr::s32>(hWnd);

	// Set AA
	param.AntiAlias = Antialias;

	// Depth is 32 by default
    if(Depth == 0)
		Depth = 32;
	param.Bits = Depth;

	// Setup other details
	param.DriverType = irr::video::EDT_DIRECT3D9;
	param.Stencilbuffer = true;
	param.Vsync = false;

	// Initialise ASync
	unsigned int asyncBufferSize = 1024 * 1024 * 20; // 20MB
	void* asyncBuffer = malloc(asyncBufferSize);
	bbdx2_ASyncJobInit( &BBFatalError, asyncBuffer, asyncBufferSize );

	// Make the device
	device = irr::createDeviceEx(param);

	// Get managers
    driver = device->getVideoDriver();
	smgr = device->getSceneManager();
	((video::CD3D9Driver*)driver)->Smgr = smgr;

	// I don't think this effects anything anymore
	driver->setTextureCreationFlag(irr::video::ETCF_ALWAYS_32_BIT, true);

	// Background is black by default
	BackGround = SColor(255,0,0,0);

	// Resolution globals
	screenWidth = Width;
	screenHeight = Height;

	// Not used anymore/yet
	enableAnis = Anisotropic;

	// Setup basic saquad
	CreateAndDefineSAQuad();

	// Load default texture
	ITexture* DefaultTexture = driver->getTexture(DefaultTex, false, false, false);
	if(!DefaultTexture)
	{
		char TexErr[2048];
		sprintf(TexErr, "Could not load Default Texture: %s", DefaultTex);
		MessageBox(NULL, TexErr,"BBDX - Runtime Error", MB_OK | MB_ICONERROR );
		exit(0);
	}
	driver->setDefaultTexture(DefaultTexture);
	DefaultTexture->ReferenceCounter = 65535;

	//Start Collisions
	bbdx2_SetupPhysX();

	// Start debug overlay
	DO_Initialize( driver->getExposedVideoData().D3D9.D3DDev9, Width, Height );

//////////////////////////////////////////////////////////////////////////
// PostProcess
//////////////////////////////////////////////////////////////////////////
	pRTM = new cRenderTargetManager( bbdx2_GetIDirect3DDevice9() );
	pRTM->CreatePP_RenderTargets(dynamic_cast<irr::video::CD3D9Driver*>(driver)->GetAntiAlias());
 	ActivePP_Effect = new cPP_Effect();
	bEnablePP_Pipeline = true;

//	LoadUserDefinedPP_FromXML( "Data\\Game Data\\PostProcess.xml" );
// 	if ( !lpPP_Effects.empty() ) ActivePP_Effect = *lpPP_Effects.begin();

	// Simple Debugging
	std::ifstream strm2;
	strm2.open("DebugMe.txt", std::ios::in);
	if(strm2.is_open())
	{
		char a = 0;
		strm2.read(&a, 1);
		strm2.close();
		if(a != 'n')
			MessageBox(NULL, "Debug token found.\nAllowing loader and scene debugging.", "Debug Mode", MB_OK | MB_ICONINFORMATION);
		DEBUGMODE = true;	
	}


}






// Render world
DLLPRE void DLLEX laghimout()
{
	SEHSTART;

	

	// Begin scene
    bool Continue = driver->beginScene(true, true, BackGround);

	// If the device was reset then don't bother doing anything else
	if(Continue == false) return;

#if 1
	if ( bEnablePP_Pipeline && ActivePP_Effect )
	{
		StartDebugTimer("B-RScene");
 		pRTM->pRT_Scene->Set(0);
 		pRTM->pRT_Scene->pD3DDevice->Clear(NULL, 0, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, (D3DCOLOR)BackGround.color, 1, 0);
 		smgr->drawAll();
		StopDebugTimer();
		StartDebugTimer("B-PP1");
 		pRTM->pRT_Scene->UnSet();
 		pRTM->pRT_Scene->ResolveMSAA();
 		pRTM->pRTM->CloneRenderTarget(pRTM->pRT_Scene, pRTM->pRT0);
		pRTM->RenderTexture(  ActivePP_Effect->ApplyPP(pRTM->pRT1, pRTM->pRT0, pRTM->pRT_Scene, ((irr::video::CD3D9Driver*)driver)->GetBackBuffer2Texture()) );
		StopDebugTimer();
	}
	else
#endif
	{
		StartDebugTimer("B-RScene");
		smgr->drawAll();
		StopDebugTimer();
	}

	StartDebugTimer("B-REndS");
	// End scene
	driver->endScene(1);

	//GUIManager->Render();

	driver->endScene(0);

	StopDebugTimer();

	SEHEND
}

// Render world
DLLPRE void DLLEX SetWaterLevel( float WaterHeight )
{
	SEHSTART;

	smgr->WaterHeight = WaterHeight;

	SEHEND
}

// GraphicsWidth
DLLPRE int DLLEX simulators()
{
	return screenWidth;
}

// GraphicsHeight
DLLPRE int DLLEX flight()
{
	return screenHeight;
}

// End Graphics
DLLPRE void DLLEX lickme()
{
	// clean PP_Resources
	FOR_EACH_VALUE( cPP_Effect*, pPP_Effect, std::list<cPP_Effect *>, lpPP_Effects)
		SAFE_DELETE( pPP_Effect );
	lpPP_Effects.clear();
	SAFE_DELETE(pRTM);

	smgr->Destroy();
	driver->grab();
	device->drop();
	driver->Destroy();

	//StopCollisionPRO();
}

