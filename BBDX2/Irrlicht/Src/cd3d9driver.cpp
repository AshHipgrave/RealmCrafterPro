// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#define _IRR_DONT_DO_MEMORY_DEBUGGING_HERE
#include "CD3D9Driver.h"
#include "os.h"
#include "S3DVertex.h"
#include "CD3D9Texture.h"
#include <stdio.h>
#include "..\..\PPChains.h"

#include "..\..\bbdx\render targets\cRenderTargetManager.h"
//#include "..\ShaderConstantSet.h"

#include "IrrCompileConfig.h"
#ifdef _IRR_WINDOWS_

#ifdef _IRR_COMPILE_WITH_DIRECT3D_9_

#include "..\..\CPPEffect.h"
#include "bbdx\DebugOverlay.h"
#include "BBDX\BBThread.h"

int bbdxTextureAlloc = 0;
int bbdxTargetAlloc = 0;
int bbdxVertexAlloc = 0;
int bbdxPixelAlloc = 0;

GeneralCallbackFN* RenderGUICallback[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
GeneralCallbackFN* RenderSolidCallback[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
RTCallbackFN* RenderSolidCallbackRT[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
GeneralCallbackFN* DeviceResetCallback[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
GeneralCallbackFN* DeviceLostCallback[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
XYCallbackFN* DeviceResetXYCallback[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
ShadowMapCallbackFN* RenderShadowDepthCallback[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
ShadowMapVPCallbackFN* RenderShadowDepthVPCallback[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

extern "C" __declspec(dllexport) void SetRenderGUICallback(int index, GeneralCallbackFN* Fn)
{
	RenderGUICallback[index] = Fn;
}

extern "C" __declspec(dllexport) void SetRenderSolidCallback(int index, GeneralCallbackFN* Fn)
{
	RenderSolidCallback[index] = Fn;
}

extern "C" __declspec(dllexport) void SetRenderSolidCallbackRT(int index, RTCallbackFN* Fn)
{
	RenderSolidCallbackRT[index] = Fn;
}

extern "C" __declspec(dllexport) void SetDeviceLostCallback(int index, GeneralCallbackFN* Fn)
{
	DeviceLostCallback[index] = Fn;
}

extern "C" __declspec(dllexport) void SetDeviceResetXYCallback(int index, XYCallbackFN* Fn)
{
	DeviceResetXYCallback[index] = Fn;
}

extern "C" __declspec(dllexport) void SetDeviceResetCallback(int index, GeneralCallbackFN* Fn)
{
	DeviceResetCallback[index] = Fn;
}

extern "C" __declspec(dllexport) void SetRenderShadowDepthCallback(int index, ShadowMapCallbackFN* Fn)
{
	RenderShadowDepthCallback[index] = Fn;
}

extern "C" __declspec(dllexport) void SetRenderShadowDepthVPCallback(int index, ShadowMapVPCallbackFN* Fn)
{
	RenderShadowDepthVPCallback[index] = Fn;
}


void CallRenderGUICallback()
{
	for(int i = 0; i < 10; ++i)
		if(RenderGUICallback[i] != 0)
			RenderGUICallback[i]();
}

void CallRenderSolidCallback()
{
	for(int i = 0; i < 10; ++i)
		if(RenderSolidCallback[i] != 0)
			RenderSolidCallback[i]();
}

void CallRenderSolidCallbackRT(int rtindex)
{
	for(int i = 0; i < 10; ++i)
		if(RenderSolidCallbackRT[i] != 0)
			RenderSolidCallbackRT[i](rtindex);
}

void CallDeviceResetCallback()
{
	for(int i = 0; i < 10; ++i)
		if(DeviceResetCallback[i] != 0)
			DeviceResetCallback[i]();
}

void CallDeviceLostCallback()
{	
	for(int i = 0; i < 10; ++i)
		if(DeviceLostCallback[i] != 0)
			DeviceLostCallback[i]();

}

void CallDeviceResetXYCallback(float x, float y)
{
	for(int i = 0; i < 10; ++i)
		if(DeviceResetXYCallback[i] != 0)
			DeviceResetXYCallback[i](x, y);
}

void CallShadowMapCallback(const float* mat, const float* lightView, const float* lightProjection)
{
	for(int i = 0; i < 10; ++i)
		if(RenderShadowDepthCallback[i] != 0)
			RenderShadowDepthCallback[i](mat);

	for(int i = 0; i < 10; ++i)
		if(RenderShadowDepthVPCallback[i] != 0)
			RenderShadowDepthVPCallback[i](lightView, lightProjection);
}

const D3DVERTEXELEMENT9 DEFS3DVertexVDecl[] =
{
	{0, 0,  D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
	{0, 12, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL,   0},
	{0, 24, D3DDECLTYPE_D3DCOLOR, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_COLOR,    0},
	{0, 28, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
	D3DDECL_END()	
};

const D3DVERTEXELEMENT9 DEFS3DVertex2TCoordsVDecl[] =
{
	{0, 0,  D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
	{0, 12, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL,   0},
	{0, 24, D3DDECLTYPE_D3DCOLOR, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_COLOR,    0},
	{0, 28, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
	{0, 36, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 1},
	D3DDECL_END()	
};

const D3DVERTEXELEMENT9 DEFS3DVertexTangentsVDecl[] =
{
	{0, 0,  D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
	{0, 12, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL,   0},
	{0, 24, D3DDECLTYPE_D3DCOLOR, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_COLOR,    0},
	{0, 28, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
	{0, 36, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TANGENT,  0},
	{0, 48, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_BINORMAL, 0},
	{0, 60, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 1},
	D3DDECL_END()	
};

const D3DVERTEXELEMENT9 DEFSAnimatedVertVDecl[] =
{
	{0, 0,  D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
	{0, 12, D3DDECLTYPE_FLOAT4,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_BLENDWEIGHT, 0},
	{0, 28, D3DDECLTYPE_UBYTE4,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_BLENDINDICES, 0},
	{0, 32, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL,   0},
	{0, 44, D3DDECLTYPE_D3DCOLOR, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_COLOR,    0},
	{0, 48, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
	{0, 56, D3DDECLTYPE_FLOAT2,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 1},
	{0, 64, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TANGENT,  0},
	{0, 76, D3DDECLTYPE_FLOAT3,   D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_BINORMAL,  0},
	D3DDECL_END()	
};

namespace irr
{
namespace video
{

	const irr::c8* TextureTransformVar[4];

inline IDirect3DDevice9* CD3D9Driver::GetDXDevice()
{
	return pID3DDevice;
}

//! constructor
CD3D9Driver::CD3D9Driver(const core::dimension2d<s32>& screenSize, HWND window,
				bool fullscreen, bool stencilbuffer,
				io::IFileSystem* io, bool pureSoftware)
: CNullDriver(io, screenSize), D3DLibrary(0), CurrentRenderMode(ERM_NONE), pID3DDevice(0),
 LastVertexType((video::E_VERTEX_TYPE)-1), ResetRenderStates(true), pID3D(0),
 LastSetLight(-1), Transformation3DChanged(0), StencilBuffer(stencilbuffer),
 DeviceLost(false), Fullscreen(fullscreen), PrevRenderTarget(0), CurrentRendertargetSize(0,0)
{
	#ifdef _DEBUG
	setDebugName("CD3D9Driver");
	#endif

	printVersion();

	for (int i=0; i<4; ++i)
	{
		CurrentTexture[i] = 0;
		LastTextureMipMapsAvailable[i] = false;
	}

	MaxLightDistance = sqrtf(FLT_MAX);
	ShadowMapSize = 256;
}




//! destructor
CD3D9Driver::~CD3D9Driver()
{
	deleteMaterialRenders();

	for (int i=0; i<4; ++i)
		if (CurrentTexture[i])
			CurrentTexture[i]->drop();

	// drop d3d9
	if (pID3DDevice)
		pID3DDevice->Release();

	if (pID3D)
		pID3D->Release();
}

//! initialises the Direct3D API
bool CD3D9Driver::initDriver(const core::dimension2d<s32>& screenSize, HWND hwnd,
				u32 bits, bool fullScreen, bool pureSoftware,
				bool highPrecisionFPU, bool vsync, int antiAlias)
{
	OutputDebugString("Renderer Starting...\n");

	HRESULT hr;
	Fullscreen = fullScreen;

	if (!pID3D)
	{
		D3DLibrary = LoadLibrary( "d3d9.dll" );

		if (!D3DLibrary)
		{
			os::Printer::log("Error, could not load d3d9.dll.", ELL_ERROR);
			return false;
		}

		typedef IDirect3D9 * (__stdcall *D3DCREATETYPE)(UINT);
		D3DCREATETYPE d3dCreate = (D3DCREATETYPE) GetProcAddress(D3DLibrary, "Direct3DCreate9");

		if (!d3dCreate)
		{
			os::Printer::log("Error, could not get proc adress of Direct3DCreate9.", ELL_ERROR);
			return false;
		}

		//just like pID3D = Direct3DCreate9(D3D_SDK_VERSION);
		pID3D = (*d3dCreate)(D3D_SDK_VERSION);


		if (!pID3D)
		{
			os::Printer::log("Error initializing D3D.", ELL_ERROR);
			return false;
		}
	}

	// print device information
	D3DADAPTER_IDENTIFIER9 dai;
	if (!FAILED(pID3D->GetAdapterIdentifier(D3DADAPTER_DEFAULT, 0, &dai)))
	{
		char tmp[512];

		s32 Product = HIWORD(dai.DriverVersion.HighPart);
		s32 Version = LOWORD(dai.DriverVersion.HighPart);
		s32 SubVersion = HIWORD(dai.DriverVersion.LowPart);
		s32 Build = LOWORD(dai.DriverVersion.LowPart);

		sprintf(tmp, "%s %s %d.%d.%d.%d", dai.Description, dai.Driver, Product, Version,
			SubVersion, Build);
		os::Printer::log(tmp, ELL_INFORMATION);
	}

	D3DDISPLAYMODE d3ddm;
	hr = pID3D->GetAdapterDisplayMode(D3DADAPTER_DEFAULT, &d3ddm);
	if (FAILED(hr))
	{
		os::Printer::log("Error: Could not get Adapter Display mode.", ELL_ERROR);
		return false;
	}

	ZeroMemory(&present, sizeof(present));

	present.SwapEffect		= fullScreen ? D3DSWAPEFFECT_FLIP : D3DSWAPEFFECT_COPY;
	present.Windowed		= fullScreen ? FALSE : TRUE;
	present.BackBufferFormat	= D3DFMT_A8R8G8B8;
	present.EnableAutoDepthStencil	= TRUE;
	present.PresentationInterval	= D3DPRESENT_INTERVAL_IMMEDIATE;
	present.BackBufferWidth = screenSize.Width;
	present.BackBufferHeight = screenSize.Height;

	

	if (fullScreen)
	{
		present.BackBufferWidth = screenSize.Width;
		present.BackBufferHeight = screenSize.Height;
		present.BackBufferFormat = D3DFMT_A8R8G8B8;
		present.BackBufferCount = 0;
		present.FullScreen_RefreshRateInHz = D3DPRESENT_RATE_DEFAULT;

		if (vsync)
			present.PresentationInterval = D3DPRESENT_INTERVAL_ONE;
	}

	D3DDEVTYPE devtype = D3DDEVTYPE_HAL;
	#ifndef _IRR_D3D_NO_SHADER_DEBUGGING
	devtype = D3DDEVTYPE_REF;
	#endif

	// enable anti alias if possible and desired
	if (antiAlias > 0)
	{
		DWORD qualityLevels = 0;

		if (!FAILED(pID3D->CheckDeviceMultiSampleType(D3DADAPTER_DEFAULT,
		           devtype, present.BackBufferFormat, !fullScreen,
		           (D3DMULTISAMPLE_TYPE)antiAlias, &qualityLevels)))
		{
			// enable multi sampling
			// Remove AA settings
			//present.SwapEffect         = D3DSWAPEFFECT_DISCARD;
			//present.MultiSampleType    = (D3DMULTISAMPLE_TYPE)antiAlias;
			//present.MultiSampleQuality = qualityLevels-1;
			char AAMessage[512];
			sprintf(AAMessage, "AntiAlias level: %i\n", antiAlias);
			OutputDebugString(AAMessage);
		}
		else
		{
			OutputDebugString("AntiAliasing disabled as selected level is unavailable\n");
			antiAlias = 0;
		}
	}
	AntiAlias = antiAlias;

	// check stencil buffer compatibility
	if (StencilBuffer)
	{
		present.AutoDepthStencilFormat = D3DFMT_D24S8;
		if(FAILED(pID3D->CheckDeviceFormat(D3DADAPTER_DEFAULT, devtype,
			present.BackBufferFormat, D3DUSAGE_DEPTHSTENCIL,
			D3DRTYPE_SURFACE, D3DFMT_D24S8)))
		{
			os::Printer::log("Device does not support stencilbuffer, disabling stencil buffer.", ELL_WARNING);
			StencilBuffer = false;
		}
		else
		if(FAILED(pID3D->CheckDepthStencilMatch(D3DADAPTER_DEFAULT, devtype,
			present.BackBufferFormat, present.BackBufferFormat, D3DFMT_D24S8)))
		{
			os::Printer::log("Depth-stencil format is not compatible with display format, disabling stencil buffer.", ELL_WARNING);
			StencilBuffer = false;
		}
	}

	// Pull out MSAA data
	//D3DMULTISAMPLE_TYPE MultiSampleType = present.MultiSampleType;
	//present.MultiSampleType = D3DMULTISAMPLE_NONE;

	if (!StencilBuffer)
		present.AutoDepthStencilFormat = D3DFMT_D24S8;


	// create device
	DWORD fpuPrecision = highPrecisionFPU ? D3DCREATE_FPU_PRESERVE : 0;
	if (pureSoftware)
	{
		hr = pID3D->CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_REF, hwnd,
				fpuPrecision | D3DCREATE_SOFTWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED, &present, &pID3DDevice);

		if (FAILED(hr))
			os::Printer::log("Was not able to create Direct3D9 software device.", ELL_ERROR);
	}
	else
	{

		OutputDebugString("Looking for PerfHUD...\n");
		bool NVPerfHud = false;

#ifndef USEPERF
#define USEPERF
#endif

#ifdef USEPERF
		for (UINT Adapter=0;Adapter<pID3D->GetAdapterCount();Adapter++)
		{
			D3DADAPTER_IDENTIFIER9 Identifier;
			HRESULT Res;
			Res = pID3D->GetAdapterIdentifier(Adapter,0,&Identifier);
			if (strstr(Identifier.Description,"PerfHUD") != 0)
			{
				OutputDebugString("PerfHUD Adapter found... creating\n");
				hr = pID3D->CreateDevice(Adapter, D3DDEVTYPE_REF, hwnd,
					/*fpuPrecision |*/ D3DCREATE_HARDWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED, &present, &pID3DDevice);
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
#endif

		OutputDebugString("Checking...\n");
		if(!NVPerfHud)
		{
			OutputDebugString("PerfHUD was not found; using primary device\n");
			hr = pID3D->CreateDevice(D3DADAPTER_DEFAULT, devtype, hwnd,
					fpuPrecision | D3DCREATE_HARDWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED, &present, &pID3DDevice);
		}

		if(FAILED(hr))
		{
			hr = pID3D->CreateDevice(D3DADAPTER_DEFAULT, devtype, hwnd,
					fpuPrecision | D3DCREATE_MIXED_VERTEXPROCESSING | D3DCREATE_MULTITHREADED , &present, &pID3DDevice);

			if(FAILED(hr))
			{
				hr = pID3D->CreateDevice(D3DADAPTER_DEFAULT, devtype, hwnd,
						fpuPrecision | D3DCREATE_SOFTWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED, &present, &pID3DDevice);

				if (FAILED(hr))
					os::Printer::log("Was not able to create Direct3D9 device.", ELL_ERROR);
			}
		}
	}

	char OO[1024];
	sprintf(OO, "Interval: %i\n", present.PresentationInterval);
	OutputDebugString(OO);

	if (!pID3DDevice)
	{
		os::Printer::log("Was not able to create DIRECT3D9 device.", ELL_ERROR);
		return false;
	}

	// get caps
	pID3DDevice->GetDeviceCaps(&Caps);

	// disable stencilbuffer if necessary
	if (StencilBuffer &&
		(!(Caps.StencilCaps & D3DSTENCILCAPS_DECRSAT) ||
		!(Caps.StencilCaps & D3DSTENCILCAPS_INCRSAT) ||
		!(Caps.StencilCaps & D3DSTENCILCAPS_KEEP)))
	{
		os::Printer::log("Device not able to use stencil buffer, disabling stencil buffer.", ELL_WARNING);
		StencilBuffer = false;
	}

	if(Caps.VertexShaderVersion < D3DVS_VERSION(2, 0))
	{
		MessageBox(NULL,"Your graphics hardware does not support Vertex Shader Version 2.0", "Device Creation Error", MB_OK | MB_ICONERROR);
		exit(0);
	}

	if(Caps.PixelShaderVersion < D3DPS_VERSION(2, 0))
	{
		MessageBox(NULL,"Your graphics hardware does not support Pixel Shader Version 2.0", "Device Creation Error", MB_OK | MB_ICONERROR);
		exit(0);
	}

	// enable antialiasing
	if (antiAlias)
		pID3DDevice->SetRenderState(D3DRS_MULTISAMPLEANTIALIAS, TRUE);

	// set fog mode
	setFog(FogColor, LinearFog, FogStart, FogEnd, FogDensity, PixelFog, RangeFog);

	// set exposed data
	ExposedData.D3D9.D3D9 = pID3D;
	ExposedData.D3D9.D3DDev9 = pID3DDevice;
	ExposedData.D3D9.HWnd =  reinterpret_cast<s32>(hwnd);

	ResetRenderStates = true;

	// set maximal anisotropy
	pID3DDevice->SetSamplerState(0, D3DSAMP_MAXANISOTROPY, min(16, Caps.MaxAnisotropy));
	pID3DDevice->SetSamplerState(1, D3DSAMP_MAXANISOTROPY, min(16, Caps.MaxAnisotropy));

	// Add one to the Effects list, so we can "return 0"
	this->EffectsList.push_back(0);

	PPInit(pID3DDevice, screenSize.Width, screenSize.Height);

	// Post Process setup
	BuildQuad(pID3DDevice);
	
	BackBuffer = 0;
	BackBuffer2 = new cRT(pID3DDevice, D3DFMT_A8R8G8B8);
	BackBuffer2->Create(screenSize.Width, screenSize.Height, false, D3DFMT_UNKNOWN);
	BackBuffer2->ClearColor = 0x00000000;

	TexBBFrame = NULL;
	TexBB2Frame = NULL;
	pID3DDevice->CreateTexture(screenSize.Width, screenSize.Height, 1, D3DUSAGE_RENDERTARGET, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT, &TexBBFrame, NULL);
	pID3DDevice->CreateTexture(screenSize.Width, screenSize.Height, 1, D3DUSAGE_RENDERTARGET, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT, &TexBB2Frame, NULL);
	BackBufferFrame = new CD3D9Texture(TexBBFrame, pID3DDevice, screenSize, "FRAMEBUFFER");
	BackBuffer2Frame = new CD3D9Texture(TexBB2Frame, pID3DDevice, screenSize, "FRAMEBUFFER2");

	if(CheckFormat(D3DFMT_D24S8, D3DFMT_G32R32F))
	{
		ShadowFormat = D3DFMT_G32R32F;
		OutputDebugString("Using ShadowMap format G32R32F.\n");
	}else if(CheckFormat(D3DFMT_D24S8, D3DFMT_G16R16))
	{
		ShadowFormat = D3DFMT_G16R16;
		OutputDebugString("Using ShadowMap format G16R16.\n");
	}else if(CheckFormat(D3DFMT_D24S8, D3DFMT_A8R8G8B8))
	{
		ShadowFormat = D3DFMT_A8R8G8B8;
		OutputDebugString("Using ShadowMap format A8R8G8B8.\n");
	}else
	{
		MessageBox(NULL, "Error: Your GPU does not support shadow mapping!", "BBDX Error", MB_ICONERROR);
		exit(0);
	}

	char Buf[255];
	sprintf(Buf, "Init Driver Size: %d\n", ShadowMapSize);
	OutputDebugString(Buf);

	ShadowMapRT = new cRT(pID3DDevice, ShadowFormat);
	ShadowMapRT->Create(ShadowMapSize, ShadowMapSize, false, D3DFMT_D24S8);
	ShadowMapRT->ClearColor = 0xffffffff;

	ShadowMapBlur = new cRT(pID3DDevice, ShadowFormat);
	ShadowMapBlur->Create(ShadowMapSize, ShadowMapSize, false, D3DFMT_D24S8);
	ShadowMapBlur->ClearColor = 0xffffffff;

	ReflectionMap= new cRT(pID3DDevice, D3DFMT_A8R8G8B8);
	ReflectionMap->Create( ViewPort.getWidth(), ViewPort.getHeight(), false, D3DFMT_UNKNOWN);
	ReflectionMap->ClearColor = 0xffffffff;

	pID3DDevice->CreateVertexDeclaration(DEFS3DVertexVDecl, &S3DVertexVDecl);
	pID3DDevice->CreateVertexDeclaration(DEFS3DVertex2TCoordsVDecl, &S3DVertex2TCoordsVDecl);
	pID3DDevice->CreateVertexDeclaration(DEFS3DVertexTangentsVDecl, &S3DVertexTangentsVDecl);
	pID3DDevice->CreateVertexDeclaration(DEFSAnimatedVertVDecl, &SAnimatedVertVDecl);

	// so far so good.
	return true;
}

void CD3D9Driver::setShadowMapSize(int newSize)
{
	ShadowMapSize = newSize;

	delete ShadowMapRT;
	delete ShadowMapBlur;

	char Buf[255];
	sprintf(Buf, "set shadowmap Size: %d\n", ShadowMapSize);
	OutputDebugString(Buf);

	ShadowMapRT = new cRT(pID3DDevice, ShadowFormat);
	ShadowMapRT->Create(ShadowMapSize, ShadowMapSize, false, D3DFMT_D24S8);
	ShadowMapRT->ClearColor = 0xffffffff;

	ShadowMapBlur = new cRT(pID3DDevice, ShadowFormat);
	ShadowMapBlur->Create(ShadowMapSize, ShadowMapSize, false, D3DFMT_D24S8);
	ShadowMapBlur->ClearColor = 0xffffffff;
}

int CD3D9Driver::getShadowMapSize()
{
	return ShadowMapSize;
}

bool CD3D9Driver::CheckFormat(D3DFORMAT depthFormat, D3DFORMAT targetFormat)
{
	// Check formats
	HRESULT Hr = pID3D->CheckDeviceFormat(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, D3DFMT_X8R8G8B8, 0, D3DRTYPE_TEXTURE, targetFormat);

	if(FAILED(Hr))
		return false;

	Hr = pID3D->CheckDeviceFormat(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, D3DFMT_X8R8G8B8, D3DUSAGE_DEPTHSTENCIL, D3DRTYPE_SURFACE, depthFormat);

	if(FAILED(Hr))
		return false;

	Hr = pID3D->CheckDepthStencilMatch(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, D3DFMT_X8R8G8B8, targetFormat, depthFormat);

	if(FAILED(Hr))
		return false;

	return true;
}

void CD3D9Driver::Destroy()
{
	for(int i = 0; i < this->EffectsList.size(); ++i)
		delete this->EffectsList[i];

	DestroyQuad();

	S3DVertexVDecl->Release();
	S3DVertex2TCoordsVDecl->Release();
	S3DVertexTangentsVDecl->Release();
	SAnimatedVertVDecl->Release();

	delete deftex;
	delete ShadowMapRT;
	delete ShadowMapBlur;
	delete ReflectionMap;
	delete BackBufferFrame;
	delete BackBuffer2Frame;
	//delete BackBuffer;
	delete BackBuffer2;

	pID3DDevice->Release();
	pID3D->Release();
}

// Resize the screen
void CD3D9Driver::ResizeScreen(dimension2d<irr::s32> NewSize, bool FullScreen, int AntiAlias, int Anisotropy)
{
	OutputDebugString("Device lost or reset, rebuilding presentation data\n");
	// Set new size
	this->present.BackBufferWidth = NewSize.Width;
	this->present.BackBufferHeight = NewSize.Height;

	// Fullscreen support
	if(FullScreen)
	{
		OutputDebugString("Fullscreen mode selected\n");
		present.SwapEffect		= D3DSWAPEFFECT_FLIP;
		present.Windowed		= FALSE;
		present.BackBufferFormat = D3DFMT_A8R8G8B8;
		present.BackBufferCount = 0;
		present.FullScreen_RefreshRateInHz = D3DPRESENT_RATE_DEFAULT;
		present.PresentationInterval = D3DPRESENT_INTERVAL_ONE;
	}else
	{
		OutputDebugString("Windowed mode selected\n");
		D3DDISPLAYMODE d3ddm;
		pID3D->GetAdapterDisplayMode(D3DADAPTER_DEFAULT, &d3ddm);

		present.SwapEffect		= D3DSWAPEFFECT_DISCARD;
		present.Windowed		= TRUE;
		present.BackBufferFormat = D3DFMT_A8R8G8B8;
		present.BackBufferCount = 0;
		present.FullScreen_RefreshRateInHz = D3DPRESENT_RATE_DEFAULT;
		present.PresentationInterval = D3DPRESENT_INTERVAL_IMMEDIATE;
	}
	
	// Check AA
	DWORD QualityLevels;
	if(pID3D->CheckDeviceMultiSampleType(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, present.BackBufferFormat, !FullScreen,
		(D3DMULTISAMPLE_TYPE)AntiAlias, &QualityLevels) != D3D_OK || AntiAlias == 0)
	{
		OutputDebugString((AntiAlias == 0) ? "Antialias not selected\n" : "Antialias level invalid!\n");
		present.MultiSampleType = (D3DMULTISAMPLE_TYPE)0;
		present.MultiSampleQuality = 0;
	}else
	{
		OutputDebugString("Antialiasing\n");
		//present.SwapEffect = D3DSWAPEFFECT_DISCARD;
		//present.MultiSampleType = (D3DMULTISAMPLE_TYPE)AntiAlias;
		//present.MultiSampleQuality = QualityLevels - 1;
		present.MultiSampleType = (D3DMULTISAMPLE_TYPE)0;
		present.MultiSampleQuality = 0;
	}

	D3DVIEWPORT9 ViewPort;
	ViewPort.X = ViewPort.Y = 0;
	ViewPort.Width = NewSize.Width;
	ViewPort.Height = NewSize.Height;
	ViewPort.MinZ = 0.0f;
	ViewPort.MaxZ = 1.0f;

	pID3DDevice->SetViewport(&ViewPort);

	// Reset device
	reset();
}


void CD3D9Driver::UpdateFrameBuffer()
{

	//BackBuffer->UnSet();

	IDirect3DSurface9* From = NULL;
	IDirect3DSurface9* To = NULL;

	//From = BackBuffer->pTextureSurface;
	pID3DDevice->GetRenderTarget(0, &From);
	
	
	((IDirect3DTexture9*)BackBufferFrame->getDX9Texture())->GetSurfaceLevel(0, &To);

	pID3DDevice->StretchRect(From, NULL, To, NULL, D3DTEXF_NONE);

	From->Release();
	To->Release();

	bool SetBuffer2 = false;
	if(BackBuffer2 != NULL)
	{
		((IDirect3DTexture9*)BackBuffer2Frame->getDX9Texture())->GetSurfaceLevel(0, &To);

		pID3DDevice->StretchRect(BackBuffer2->pTextureSurface, NULL, To, NULL, D3DTEXF_NONE);

		To->Release();
		SetBuffer2 = true;
	}

	// Second RT
// 	pID3DDevice->GetRenderTarget(1, &From);
// 	bool SetBuffer2 = false;
// 	if(From != NULL)
// 	{
// 		((IDirect3DTexture9*)BackBuffer2Frame->getDX9Texture())->GetSurfaceLevel(0, &To);
// 
// 		pID3DDevice->StretchRect(From, NULL, To, NULL, D3DTEXF_NONE);
// 
// 		From->Release();
// 		To->Release();
// 		SetBuffer2 = true;
// 	}


	// Set the texture
	for(int i = 1; i < EffectsList.size(); ++i)
	{
		EffectsList[i]->SetFrameBuffer(BackBufferFrame);

		if(SetBuffer2)
			EffectsList[i]->SetFrameBuffer2(BackBuffer2Frame);
	}
}

void CD3D9Driver::SetReflectionMapTarget()
{
	ReflectionMap->Set();
}

void CD3D9Driver::UnSetReflectionMapTarget()
{
	ReflectionMap->UnSet(false, true);
}


void CD3D9Driver::SetBackBuffer2Target(bool clear)
{
	BackBuffer2->Set(-1, -1, clear, 0, false);
}

void CD3D9Driver::UnSetBackBuffer2Target()
{
	BackBuffer2->UnSet(false, false);
}


void CD3D9Driver::SetShadowTarget()
{
	ShadowMapRT->Set();
}

void CD3D9Driver::UnSetShadowTarget()
{
	ShadowMapRT->UnSet(false, true);
}

void CD3D9Driver::SetBlurShadowTarget()
{
	ShadowMapBlur->Set();
}

void CD3D9Driver::UnSetBlurShadowTarget()
{
	ShadowMapBlur->UnSet(false, true);
}

void CD3D9Driver::SetBackBufferTarget()
{
}



//! applications must call this method before performing any rendering. returns false if failed.
bool CD3D9Driver::beginScene(bool backBuffer, bool zBuffer, SColor color)
{
	os::Timer::tick();
	CNullDriver::beginScene(backBuffer, zBuffer, color);
	HRESULT hr;

	if (!pID3DDevice)
		return false;

	D3DDEVICE_CREATION_PARAMETERS CreationParams;
	pID3DDevice->GetCreationParameters(&CreationParams);

	if(FAILED(hr = pID3DDevice->TestCooperativeLevel()))
	{
		if (hr == D3DERR_DEVICELOST)
			return false;
		if (hr == D3DERR_DEVICENOTRESET)
			reset();
		return false;
	}

	DWORD flags = 0;

	if (backBuffer)
		flags |= D3DCLEAR_TARGET;

	if (zBuffer)
		flags |= D3DCLEAR_ZBUFFER;

	if (StencilBuffer)
		flags |= D3DCLEAR_STENCIL;


	// Before the scene has begun, we need to update all the movies
	for(u32 i = 0; i < CD3D9Driver::ActiveMovies.size(); ++i)
		CD3D9Driver::ActiveMovies[i]->Update();
	

	//hr = pID3DDevice->Clear( 0, NULL, flags, 0xff000000, 1.0, 0);
	//if (FAILED(hr))
	//	os::Printer::log("DIRECT3D9 clear failed.", ELL_WARNING);

	// Set RT
	//PPActive = PreRenderChain();
	//if(PPActive)
	//	BackBuffer->Set();

	//BackBuffer2->Set(-1, -1, true, 1);
	
	
	hr = pID3DDevice->Clear( 0, NULL, flags, color.color, 1.0, 0);

	hr = pID3DDevice->BeginScene();
	if (FAILED(hr))
	{
		os::Printer::log("DIRECT3D9 begin scene failed.", ELL_WARNING);
		return false;
	}

	
	// Reset decl
	CurrentDecl = NULL;
	
	return true;
}


// Temporary for testing the sectors
bool FinalRender = true;
extern "C" __declspec(dllexport) void bbdx2_AllowRenderFrame(int allow)
{
	FinalRender = allow ? 1 : 0;
}



//! applications must call this method after performing any rendering. returns false if failed.
bool CD3D9Driver::endScene( s32 windowId, core::rect<s32>* sourceRect )
{
	if (DeviceLost)
		return false;

	if(windowId == 1)
	{

	}

	if(windowId != 0)
		return true;

	CallRenderGUICallback();

	CNullDriver::endScene();


	HRESULT hr = pID3DDevice->EndScene();
	if (FAILED(hr))
	{
		os::Printer::log("DIRECT3D9 end scene failed.", ELL_WARNING);
		return false;
	}

	RECT* srcRct = 0;
	RECT sourceRectData;
	if ( sourceRect )
	{
		srcRct = &sourceRectData;
		sourceRectData.left = sourceRect->UpperLeftCorner.X;
		sourceRectData.top = sourceRect->UpperLeftCorner.Y;
		sourceRectData.right = sourceRect->LowerRightCorner.X;
		sourceRectData.bottom = sourceRect->LowerRightCorner.Y;
	}

	if(FinalRender)
		hr = pID3DDevice->Present(NULL, NULL, NULL, NULL);
	FinalRender = true;

	if (hr == D3DERR_DEVICELOST)
	{
		DeviceLost = true;
		os::Printer::log("DIRECT3D9 device lost.", ELL_WARNING);
	}
	else
	if (FAILED(hr) && hr != D3DERR_INVALIDCALL)
	{
		os::Printer::log("DIRECT3D9 present failed.", ELL_WARNING);
		return false;
	}

	return true;
}



//! queries the features of the driver, returns true if feature is available
bool CD3D9Driver::queryFeature(E_VIDEO_DRIVER_FEATURE feature)
{
	switch (feature)
	{
	case EVDF_BILINEAR_FILTER:
		return true;
	case EVDF_RENDER_TO_TARGET:
		return Caps.NumSimultaneousRTs > 0;
	case EVDF_HARDWARE_TL:
		return (Caps.DevCaps & D3DDEVCAPS_HWTRANSFORMANDLIGHT) != 0;
	case EVDF_MIP_MAP:
		return (Caps.TextureCaps & D3DPTEXTURECAPS_MIPMAP) != 0;
	case EVDF_STENCIL_BUFFER:
		return StencilBuffer &&  Caps.StencilCaps;
	case EVDF_VERTEX_SHADER_1_1:
		return Caps.VertexShaderVersion >= D3DVS_VERSION(1,1);
	case EVDF_VERTEX_SHADER_2_0:
		return Caps.VertexShaderVersion >= D3DVS_VERSION(2,0);
	case EVDF_VERTEX_SHADER_3_0:
		return Caps.VertexShaderVersion >= D3DVS_VERSION(3,0);
	case EVDF_PIXEL_SHADER_1_1:
		return Caps.PixelShaderVersion >= D3DPS_VERSION(1,1);
	case EVDF_PIXEL_SHADER_1_2:
		return Caps.PixelShaderVersion >= D3DPS_VERSION(1,2);
	case EVDF_PIXEL_SHADER_1_3:
		return Caps.PixelShaderVersion >= D3DPS_VERSION(1,3);
	case EVDF_PIXEL_SHADER_1_4:
		return Caps.PixelShaderVersion >= D3DPS_VERSION(1,4);
	case EVDF_PIXEL_SHADER_2_0:
		return Caps.PixelShaderVersion >= D3DPS_VERSION(2,0);
	case EVDF_PIXEL_SHADER_3_0:
		return Caps.PixelShaderVersion >= D3DPS_VERSION(3,0);
	case EVDF_HLSL:
		return Caps.VertexShaderVersion >= D3DVS_VERSION(1,1);
	};

	return false;
}


void CD3D9Driver::setTransform(E_TRANSFORMATION_STATE state, core::matrix4& mat)
{
}


//! sets transformation
void CD3D9Driver::setTransformN(E_TRANSFORMATION_STATE state, core::matrix4& mat)
{

	Transformation3DChanged = true;

	Matrices[state] = mat;
}


void CD3D9Driver::setShader(irr::u32 Effect)
{
	CurrentEffect = Effect;
}


//! sets the current Texture
void CD3D9Driver::setTexture(s32 stage, video::ITexture* texture)
{
	if (texture && texture->getDriverType() != EDT_DIRECT3D9)
	{
		os::Printer::log("Fatal Error: Tried to set a texture not owned by this driver.", ELL_ERROR);
		return;
	}

	if (CurrentTexture[stage])
	{
		bool DropTwice = true;

		if(CurrentTexture[stage]->ReferenceCounter <= 2)
		{
			if(CurrentTexture[stage]->ReferenceCounter == 1)
				DropTwice = false;
			this->removeTexture(CurrentTexture[stage]);
		}

		if(DropTwice == true)
			CurrentTexture[stage]->drop();
	}

	CurrentTexture[stage] = texture;

	if (texture)
	{
		texture->grab();

		if(CurrentEffect)
		{
			ShaderConstantSet* Set = EffectsList[CurrentEffect];

			switch(stage)
			{
			case 0:
				Set->SetTextureTransform(texture->Transformation, 0);
				Set->SetTextureStage(texture, 0);
				break;
			case 1:
				Set->SetTextureTransform(texture->Transformation, 1);
				Set->SetTextureStage(texture, 1);
				break;
			case 2:
				Set->SetTextureTransform(texture->Transformation, 2);
				Set->SetTextureStage(texture, 2);
				break;
			case 3:
				Set->SetTextureTransform(texture->Transformation, 3);
				Set->SetTextureStage(texture, 3);
				break;
			}
		}

	}
}



//! sets a material
void CD3D9Driver::setMaterial(const SMaterial& material)
{
	Material = material;
	
	setTexture(0, Material.Texture1);
	setTexture(1, Material.Texture2);
	setTexture(2, Material.Texture3);
	setTexture(3, Material.Texture4);
}

IDirect3DBaseTexture9* CopyTextureT(IDirect3DBaseTexture9* Tex, IDirect3DDevice9* pID3DDevice)
{
	switch(Tex->GetType())
	{
	case D3DRTYPE_TEXTURE:
		{
			IDirect3DTexture9* tex = (IDirect3DTexture9*)Tex;
			D3DSURFACE_DESC SurfParams;
			tex->GetLevelDesc(0, &SurfParams);
		 
			IDirect3DTexture9* NewTexture = NULL;
		 
			pID3DDevice->CreateTexture(SurfParams.Width,SurfParams.Height,tex->GetLevelCount(),NULL,SurfParams.Format,D3DPOOL_MANAGED,&NewTexture,NULL);
			bbdxTextureAlloc += SurfParams.Width * SurfParams.Height * 4;

			IDirect3DSurface9* Source	= NULL;
			IDirect3DSurface9* Dest		= NULL;
		 
				for (int level = 0; level < tex->GetLevelCount(); level++)
				{
				tex->GetSurfaceLevel(level, &Source);
				NewTexture->GetSurfaceLevel(level, &Dest);
		 
				pID3DDevice->StretchRect(Source, NULL, Dest, NULL, D3DTEXF_NONE);
				}
		 
			return NewTexture;

			break;
		}
	case D3DRTYPE_CUBETEXTURE:
		{
			IDirect3DCubeTexture9* tex = (IDirect3DCubeTexture9*)Tex;

			D3DSURFACE_DESC SurfParams;
			tex->GetLevelDesc(0, &SurfParams);

			IDirect3DCubeTexture9* NewTexture = NULL;
			pID3DDevice->CreateCubeTexture(SurfParams.Width, tex->GetLevelCount(), NULL, SurfParams.Format, D3DPOOL_MANAGED, &NewTexture, NULL);
			bbdxTextureAlloc += SurfParams.Width * SurfParams.Width * 4;
			
			IDirect3DSurface9* Source	= NULL;
			IDirect3DSurface9* Dest		= NULL;
		 
			for (int level = 0; level < tex->GetLevelCount(); level++)
			{
				for(int face = 0; face < 6; ++face)
				{
					tex->GetCubeMapSurface((D3DCUBEMAP_FACES)face, level, &Source);
					NewTexture->GetCubeMapSurface((D3DCUBEMAP_FACES)face, level, &Dest);
		 
					pID3DDevice->StretchRect(Source, NULL, Dest, NULL, D3DTEXF_NONE);
				}
			}
		 
			return NewTexture;

			break;
		}
	case D3DRTYPE_VOLUMETEXTURE:
		{
			OutputDebugString("Error: Attempted to copy a volume texture resource. BBDX does not support this!\n");
			return NULL;
			break;
		}
	default:
		{
			OutputDebugString("Warning: Attempted to copy an unknown texture resource!\n");
			return NULL;
		}
	}
}

IDirect3DTexture9* CD3D9Driver::GetShadowTexture(bool useBlur)
{
	if(useBlur)
	{
		if(ShadowMapBlur != 0)
			return ShadowMapBlur->pTexture;
	}
	else
	{
		if(ShadowMapRT != 0)
			return ShadowMapRT->pTexture;
	}

	return 0;
}

IDirect3DTexture9* CD3D9Driver::GetReflectionTexture()
{
	if(ReflectionMap != 0)
		return ReflectionMap->pTexture;
	else return NULL;
}

IDirect3DTexture9* CD3D9Driver::GetBackBuffer2Texture()
{
	return BackBuffer2->pTexture;
}

video::ITexture* CD3D9Driver::CopyTexture(video::ITexture* tex)
{
	ITexture* NewTex = new CD3D9Texture(CopyTextureT(((CD3D9Texture*)tex)->getDX9Texture(), this->pID3DDevice), this->pID3DDevice, tex->getSize(),"FSDFSDFS");
	return NewTex;
}

irr::core::array<IMovieTexture*> CD3D9Driver::ActiveMovies;
void CD3D9Driver::RegisterMovieTexture(IMovieTexture* MovieTexture)
{
	CD3D9Driver::ActiveMovies.push_back(MovieTexture);	
}

void CD3D9Driver::UnRegisterMovieTexture(IMovieTexture* MovieTexture)
{
	for(u32 i = 0; i < CD3D9Driver::ActiveMovies.size(); ++i)
		if(CD3D9Driver::ActiveMovies[i] == MovieTexture)
			CD3D9Driver::ActiveMovies.erase(i);
}


ITexture* CD3D9Driver::getEmptyTexture(irr::c8* filename)
{
	irr::video::CD3D9Texture* Default = (irr::video::CD3D9Texture*)this->getDefaultTexture();
	return new CD3D9Texture(Default->Texture, pID3DDevice, Default->getSize(), filename);
}

#define TEXTURE_LOAD_TYPE 0x54455849
#define TEXTURE_LOAD_PRIORITY 70

struct LoadTextureData
{
	IDirect3DDevice9* pID3DDevice;
	CD3D9Texture* Handle;
	bool Mask, Cubemap, Volume;
	bool Mipmap;
};

int LoadTextureTask(ASyncJobDesc* jobDesc)
{

	return 1;
}

int LoadTextureSync(ASyncJobDesc* jobDesc)
{
	LoadTextureData* data = (LoadTextureData*)jobDesc->UserData;

	if( jobDesc->Buffer == NULL || data == NULL )
	{
		delete data;
		return 0;
	}

	if(!data->Cubemap && !data->Volume)
	{
		// Texture Handle
		LPDIRECT3DTEXTURE9 newTexture = NULL;

		// Is it masked? if so, load it differently
		if(data->Mask)
		{
			D3DXCreateTextureFromFileInMemoryEx( data->pID3DDevice,
				jobDesc->Buffer,
				jobDesc->BufferSize,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				1,
				NULL,
				D3DFMT_UNKNOWN,
				D3DPOOL_MANAGED,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				0xff000000,
				NULL,
				NULL,
				&newTexture );

		}else if(!data->Mipmap)
		{
			D3DXCreateTextureFromFileInMemoryEx(data->pID3DDevice,
				jobDesc->Buffer,
				jobDesc->BufferSize,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				1,
				NULL,
				D3DFMT_UNKNOWN,
				D3DPOOL_MANAGED,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				0,
				NULL,
				NULL,
				&newTexture);

		}else
			D3DXCreateTextureFromFileInMemory(data->pID3DDevice, jobDesc->Buffer, jobDesc->BufferSize, &newTexture);

		if( newTexture )
		{
			// Get Width/Height
			D3DSURFACE_DESC desc;
			newTexture->GetLevelDesc(0, &desc);
			core::dimension2d<s32> size(desc.Width, desc.Height);

			if( data->Handle->Texture != NULL )
				data->Handle->Texture->Release();
			data->Handle->Texture = newTexture;
			data->Handle->TextureSize = size;

			bbdxTextureAlloc += size.Width * size.Height * 4;
		}

	}else if(data->Cubemap)
	{
		// Texture Handle
		IDirect3DCubeTexture9* newTexture = NULL;

		// Is it masked? if so, load it differently
		if(data->Mask)
		{
			D3DXCreateCubeTextureFromFileInMemoryEx(data->pID3DDevice,
				jobDesc->Buffer,
				jobDesc->BufferSize,
				D3DX_DEFAULT,
				0,
				NULL,
				D3DFMT_UNKNOWN,
				D3DPOOL_MANAGED,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				0xff000000,
				NULL,
				NULL,
				&newTexture);

		}else
			D3DXCreateCubeTextureFromFileInMemory(data->pID3DDevice, jobDesc->Buffer, jobDesc->BufferSize, &newTexture);

		if( newTexture )
		{
			// Get Width/Height
			D3DSURFACE_DESC desc;
			newTexture->GetLevelDesc(0, &desc);
			core::dimension2d<s32> size(desc.Width, desc.Height);

			if( data->Handle->Texture != NULL )
				data->Handle->Texture->Release();
			data->Handle->Texture = newTexture;
			data->Handle->TextureSize = size;
		}

	} else if(data->Volume)
	{
		// Texture Handle
		IDirect3DVolumeTexture9* newTexture = NULL;

		// Is it masked? if so, load it differently
		if(data->Mask)
		{
			D3DXCreateVolumeTextureFromFileInMemoryEx(data->pID3DDevice,
				jobDesc->Buffer,
				jobDesc->BufferSize,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				0,
				NULL,
				D3DFMT_UNKNOWN,
				D3DPOOL_MANAGED,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				0xff000000,
				NULL,
				NULL,
				&newTexture);

		}else
			D3DXCreateVolumeTextureFromFileInMemory(data->pID3DDevice, jobDesc->Buffer, jobDesc->BufferSize, &newTexture);

		if( newTexture )
		{
			// Get Width/Height
			D3DVOLUME_DESC desc;
			newTexture->GetLevelDesc(0, &desc);
			core::dimension2d<s32> size(desc.Width, desc.Height);

			if( data->Handle->Texture != NULL )
				data->Handle->Texture->Release();
			data->Handle->Texture = newTexture;
			data->Handle->TextureSize = size;
		}
	}

	delete data;

	return 1;
}


// Load a DDS texture!
video::ITexture* CD3D9Driver::loadDDSTexture(irr::c8* filename, bool Mask, bool Cubemap, bool Volume)
{
	core::stringc StringName = filename;
	if(StringName.size() > 4)
	{
		core::stringc LFour = StringName.subString(StringName.size() - 4, 4);
		core::stringc LFive = StringName.subString(StringName.size() - 5, 5);
		LFour.make_lower();
		LFive.make_lower();

		// Check if its a movie
		if(LFour == ".avi" || LFour == ".wmv" || LFive == ".divx")
		{
			// Its a movie, lets load it
			IMovieTexture* Movie = LoadMovieTexture(filename, CD3D9Driver::RegisterMovieTexture, CD3D9Driver::UnRegisterMovieTexture);
			if(Movie == 0)
				return this->getDefaultTexture();

			// Create a texture for it
			IDirect3DTexture9* Texture = 0;
			if(pID3DDevice->CreateTexture(Movie->GetMovieWidth(), Movie->GetMovieHeight(), 1, 0, D3DFMT_X8R8G8B8, D3DPOOL_MANAGED, &Texture, 0) != D3D_OK)
			{
				delete Movie;
				return this->getDefaultTexture();
			}
			
			// Set Texture
			Movie->SetMovieTexture(Texture);

			// Create IrrTexture
			D3DSURFACE_DESC Desc;
			Texture->GetLevelDesc(0, &Desc);
			core::dimension2d<s32> size(Desc.Width, Desc.Height);
			CD3D9Texture* tex = new CD3D9Texture(Texture, pID3DDevice, size, filename);
			
			// Done
			return tex;
		}
	}

	IDirect3DBaseTexture9* defaultHandle = NULL;
	core::dimension2d<s32> size(0, 0);

	if( deftex != NULL )
	{
		CD3D9Texture* defaultTexture = (CD3D9Texture*)deftex;
		defaultTexture->Texture->AddRef();
		defaultHandle = defaultTexture->Texture;
		size = defaultTexture->TextureSize;
	}

	// Create it and Return it!
	CD3D9Texture* tex = new CD3D9Texture(defaultHandle, pID3DDevice, size, filename);
	
	// As new Async lib to run our task
	LoadTextureData* data = new LoadTextureData();
	data->pID3DDevice = pID3DDevice;
	data->Handle = tex;
	data->Volume = Volume;
	data->Cubemap = Cubemap;
	data->Mask = Mask;
	data->Mipmap = getTextureCreationFlag(ETCF_CREATE_MIP_MAPS);

	// Only insert if its not the default
	if( deftex != NULL )
	{
		bbdx2_ASyncInsertIOJob( filename, TEXTURE_LOAD_TYPE, &LoadTextureTask, &LoadTextureSync, data, TEXTURE_LOAD_PRIORITY );
	}
	else
	{
		FILE* fp = fopen( filename, "rb" );

		if( !fp )
			return NULL;

		fseek( fp, 0, SEEK_END );
		size_t len = ftell( fp );
		fseek( fp, 0, SEEK_SET );

		void* buffer = malloc( len );
		fread( buffer, 1, len, fp );
		fclose( fp );

		ASyncJobDesc desc;
		desc.Buffer = buffer;
		desc.BufferSize = len;
		desc.UserData = data;

		LoadTextureSync( &desc );

		free(buffer);

	}

	if(Mask)
		tex->isAlpha = true;
	return tex;
}


//! Enables or disables a texture creation flag.
void CD3D9Driver::setTextureCreationFlag(E_TEXTURE_CREATION_FLAG flag, bool enabled)
{
	if (flag == video::ETCF_CREATE_MIP_MAPS && !queryFeature(EVDF_MIP_MAP))
		enabled = false;

	CNullDriver::setTextureCreationFlag(flag, enabled);
}

//! sets a viewport
void CD3D9Driver::setViewPort(const core::rect<s32>& area)
{
	core::rect<s32> vp = area;
	core::rect<s32> rendert(0,0, ScreenSize.Width, ScreenSize.Height);
	vp.clipAgainst(rendert);

	D3DVIEWPORT9 viewPort;
	viewPort.X = vp.UpperLeftCorner.X;
	viewPort.Y = vp.UpperLeftCorner.Y;
	viewPort.Width = vp.getWidth();
	viewPort.Height = vp.getHeight();
	viewPort.MinZ = 0.0f;
	viewPort.MaxZ = 1.0f;

	HRESULT hr = D3DERR_INVALIDCALL;
	if (vp.getHeight()>0 && vp.getWidth()>0)
		hr = pID3DDevice->SetViewport(&viewPort);

	if (FAILED(hr))
		os::Printer::log("Failed setting the viewport.", ELL_WARNING);

	ViewPort = vp;
}



//! gets the area of the current viewport
const core::rect<s32>& CD3D9Driver::getViewPort() const
{
	return ViewPort;
}




u32 CD3D9Driver::getFVFSize(video::E_VERTEX_TYPE vType)
{
	int leng = 0;

	if (vType == video::EVT_STANDARD)
		leng = sizeof(video::S3DVertex);
	
	if (vType == video::EVT_2TCOORDS)
		leng = sizeof(video::S3DVertex2TCoords);
	
	if (vType == video::EVT_TANGENTS)
		leng = sizeof(video::S3DVertexTangents);

	if(vType == video::EVT_ANIMATEDVERT)
		leng = sizeof(video::SAnimatedVert);
	
	return leng;
}

bool CD3D9Driver::updateHardwareBufferLM(scene::SMeshBufferLightMap* buffer)
{
	int leng;

	// If the Index Buffer Exists
	if ( buffer->indexB != 0 )
	{

		// If the Index buffer has changed
		if(buffer->getIndexCount() != buffer->icnt)
		{

			((IDirect3DIndexBuffer9*)buffer->indexB)->Release();
			buffer->indexB = 0;
		}

	}

	// If the Vertex Buffer Exists
	if( buffer->vertexB != 0)
	{

		// If the vertex buffer has changed
		if(buffer->getVertexCount() != buffer->vcnt)
		{

			((IDirect3DVertexBuffer9*)buffer->vertexB)->Release();
			buffer->vertexB = 0;
		}

	}
	
	// Get Vertex info from Irrlicht
	leng = this->getFVFSize( buffer->getVertexType() );
	buffer->vcnt = buffer->getVertexCount();
	leng *= buffer->vcnt;

	if (leng == 0)
			return false;

	// Create Vertex buffer if it doesn't exist
	if( buffer->vertexB == 0)
	{
		// Init a new buffer
		IDirect3DVertexBuffer9* theNewVertexBuffer;

		// Create it
		pID3DDevice->CreateVertexBuffer(
			leng,
			D3DUSAGE_WRITEONLY,
			NULL,
			D3DPOOL_MANAGED,
			&theNewVertexBuffer,
			0);

		// Assign it
		buffer->vertexB = theNewVertexBuffer;
	}

	// Create an Index buffer
	if ( buffer->indexB == 0 )
	{

		// Get Index count
		int indxcnt = buffer->getIndexCount();
		buffer->icnt = indxcnt;
	
		// Define it
		IDirect3DIndexBuffer9* theNewIndexBuffer;

		// Create it
		pID3DDevice->CreateIndexBuffer(
			indxcnt * 4,
			D3DUSAGE_WRITEONLY,
			buffer->getIndexCount() > 65535 ? D3DFMT_INDEX32 : D3DFMT_INDEX16,
			D3DPOOL_MANAGED,
			&theNewIndexBuffer,
			0);

		// Assign it
		buffer->indexB = theNewIndexBuffer;

	}

	// Get buffers
	IDirect3DIndexBuffer9*  myIndexBuffer  = (IDirect3DIndexBuffer9*) buffer->indexB;
	IDirect3DVertexBuffer9* myVertexBuffer = (IDirect3DVertexBuffer9*)buffer->vertexB;

	// Define buffers we'll use
	void* myIndices;
	u16* myIndicesu16;

	// Lock index buffer
	myIndexBuffer->Lock( 0, 0, &myIndices, 0);

	// get 32-bit indices from DX's void pointer
	myIndicesu16 = (u16*)myIndices;

	//Get Irrlicht's Indices
	u32* indices = buffer->getIndices();
	int extL = buffer->getIndexCount();

	// Assign irrlichts indices to it.
	for (int cntr = 0; cntr < extL; cntr++)
	{
		myIndicesu16[cntr] = (u16)indices[cntr];
	}

	// Unlock buffer
	myIndexBuffer->Unlock();

	// Define buffer for verts
	void* bufferVertices;

	// Lock verts
	myVertexBuffer->Lock( 0, 0, &bufferVertices, 0);

	// I don't know what this does
	int cl = leng / 4;

	// Get DX and Irrlicht verts
	u32* bufferu32 = (u32*)bufferVertices;
	u32* src = (u32*)buffer->getVertices();

	// Copy array
	for (s32 count = 0; count < cl; count++)
	{
		bufferu32[count] = src[count];
	}

	// Unlock
	 myVertexBuffer->Unlock();

	// wewt
	return true;
}



bool CD3D9Driver::updateHardwareBuffer(scene::IMeshBuffer* buffer)
{
	int leng;

	// If the Index Buffer Exists
	if ( buffer->indexB != 0 )
	{

		// If the Index buffer has changed
		if(buffer->getIndexCount() != buffer->icnt)
		{

			((IDirect3DIndexBuffer9*)buffer->indexB)->Release();
			buffer->indexB = 0;
		}

	}

	// If the Vertex Buffer Exists
	if( buffer->vertexB != 0)
	{

		// If the vertex buffer has changed
		if(buffer->getVertexCount() != buffer->vcnt)
		{

			((IDirect3DVertexBuffer9*)buffer->vertexB)->Release();
			buffer->vertexB = 0;
		}

	}
	
	// Get Vertex info from Irrlicht
	int FormatSize;
	switch(buffer->getVertexType())
	{
	case video::EVT_STANDARD:
		FormatSize = sizeof(S3DVertex);
		break;
	case video::EVT_2TCOORDS:
		FormatSize = sizeof(S3DVertex2TCoords);
		break;
	case video::EVT_TANGENTS:
		FormatSize = sizeof(S3DVertexTangents);
		break;
	case video::EVT_ANIMATEDVERT:
		FormatSize = sizeof(SAnimatedVert);
		break;
	}
	leng = FormatSize;
	buffer->vcnt = buffer->getVertexCount();
	leng *= buffer->vcnt;

	if (leng == 0)
	{
			return false;
		
	}
	// Create Vertex buffer if it doesn't exist
	if( buffer->vertexB == 0)
	{
		// Init a new buffer
		IDirect3DVertexBuffer9* theNewVertexBuffer;

		// Create it
		pID3DDevice->CreateVertexBuffer(
			leng,
			D3DUSAGE_WRITEONLY,
			0,
			D3DPOOL_MANAGED,
			&theNewVertexBuffer,
			0);

		// Assign it
		buffer->vertexB = theNewVertexBuffer;
	}

	// Create an Index buffer
	if ( buffer->indexB == 0 )
	{

		// Get Index count
		int indxcnt = buffer->getIndexCount();
		buffer->icnt = indxcnt;
	
		// Define it
		IDirect3DIndexBuffer9* theNewIndexBuffer;

		// Create it
		pID3DDevice->CreateIndexBuffer(
			indxcnt * 4,
			D3DUSAGE_WRITEONLY,
			buffer->getIndexCount() > 65535 ? D3DFMT_INDEX32 : D3DFMT_INDEX16,
			D3DPOOL_MANAGED,
			&theNewIndexBuffer,
			0);

		// Assign it
		buffer->indexB = theNewIndexBuffer;

	}

	// Get buffers
	IDirect3DIndexBuffer9*  myIndexBuffer  = (IDirect3DIndexBuffer9*) buffer->indexB;
	IDirect3DVertexBuffer9* myVertexBuffer = (IDirect3DVertexBuffer9*)buffer->vertexB;

	// Define buffers we'll use
	void* myIndices;
	u16* myIndicesu16;

	// Lock index buffer
	myIndexBuffer->Lock( 0, 0, &myIndices, 0);

	// get 32-bit indices from DX's void pointer
	myIndicesu16 = (u16*)myIndices;

	//Get Irrlicht's Indices
	u32* indices = buffer->getIndices();
	int extL = buffer->getIndexCount();

	// Assign irrlichts indices to it.
	for (int cntr = 0; cntr < extL; cntr++)
	{
		myIndicesu16[cntr] = (u16)indices[cntr];
	}

	// Unlock buffer
	myIndexBuffer->Unlock();

	// Define buffer for verts
	void* bufferVertices;

	// Lock verts
	myVertexBuffer->Lock( 0, 0, &bufferVertices, 0);

	// I don't know what this does
	int cl = leng / 4;

	// Get DX and Irrlicht verts
	u32* bufferu32 = (u32*)bufferVertices;
	u32* src = (u32*)buffer->getVertices();

	// Copy array
	for (s32 count = 0; count < cl; count++)
	{
		bufferu32[count] = src[count];
	}

	// Unlock
	 myVertexBuffer->Unlock();

	// wewt
	return true;
}

bool CD3D9Driver::updateAnimatedHardwareBuffer(scene::IMeshBuffer* buffer, core::array<scene::VertexBlends>* Blends)
{
	int leng;

	// If the Index Buffer Exists
	if ( buffer->indexB != 0 )
	{

		// If the Index buffer has changed
		if(buffer->getIndexCount() != buffer->icnt)
		{

			((IDirect3DIndexBuffer9*)buffer->indexB)->Release();
			buffer->indexB = 0;
		}

	}

	// If the Vertex Buffer Exists
	if( buffer->vertexB != 0)
	{

		// If the vertex buffer has changed
		if(buffer->getVertexCount() != buffer->vcnt)
		{

			((IDirect3DVertexBuffer9*)buffer->vertexB)->Release();
			buffer->vertexB = 0;
		}

	}
	
	// Get Vertex info from Irrlicht
	leng = sizeof(video::SAnimatedVert);//88;//this->getFVFSize( buffer->getVertexType() ) + sizeof(D3DFVF_XYZB2) + sizeof(D3DFVF_LASTBETA_UBYTE4);
	buffer->vcnt = buffer->getVertexCount();
	leng *= buffer->vcnt;

	if (leng == 0)
			return false;

	// Create Vertex buffer if it doesn't exist
	if( buffer->vertexB == 0)
	{
		// Get vertex format
		u32 fv = D3DFVF_XYZB5 | D3DFVF_LASTBETA_UBYTE4 | D3DFVF_NORMAL | D3DFVF_DIFFUSE | D3DFVF_TEX3 |
				D3DFVF_TEXCOORDSIZE2(0) | // real texture coord
				D3DFVF_TEXCOORDSIZE2(1) | // Texcoord 2
				D3DFVF_TEXCOORDSIZE3(2); // Tangent!

		//fv = D3DFVF_XYZ | D3DFVF_NORMAL | D3DFVF_DIFFUSE | D3DFVF_TEX1 | D3DFVF_TEXCOORDSIZE2(0);
		

		// Init a new buffer
		IDirect3DVertexBuffer9* theNewVertexBuffer;

		// Create it
		pID3DDevice->CreateVertexBuffer(
			leng,
			D3DUSAGE_WRITEONLY,
			0,
			D3DPOOL_MANAGED,
			&theNewVertexBuffer,
			0);

		// Assign it
		buffer->vertexB = theNewVertexBuffer;
	}

	// Create an Index buffer
	if ( buffer->indexB == 0 )
	{

		// Get Index count
		int indxcnt = buffer->getIndexCount();
		buffer->icnt = indxcnt;
	
		// Define it
		IDirect3DIndexBuffer9* theNewIndexBuffer;

		// Create it
		pID3DDevice->CreateIndexBuffer(
			indxcnt * 4,
			D3DUSAGE_WRITEONLY,
			buffer->getIndexCount() > 65535 ? D3DFMT_INDEX32 : D3DFMT_INDEX16,
			D3DPOOL_MANAGED,
			&theNewIndexBuffer,
			0);

		// Assign it
		buffer->indexB = theNewIndexBuffer;

	}

	// Get buffers
	IDirect3DIndexBuffer9*  myIndexBuffer  = (IDirect3DIndexBuffer9*) buffer->indexB;
	IDirect3DVertexBuffer9* myVertexBuffer = (IDirect3DVertexBuffer9*)buffer->vertexB;

	// Define buffers we'll use
	void* myIndices;
	u16* myIndicesu16;

	// Lock index buffer
	myIndexBuffer->Lock( 0, 0, &myIndices, 0);

	// get 32-bit indices from DX's void pointer
	myIndicesu16 = (u16*)myIndices;

	//Get Irrlicht's Indices
	u32* indices = buffer->getIndices();
	int extL = buffer->getIndexCount();

	// Assign irrlichts indices to it.
	for (int cntr = 0; cntr < extL; cntr++)
	{
		myIndicesu16[cntr] = (u16)indices[cntr];
	}

	// Unlock buffer
	myIndexBuffer->Unlock();

	// Define buffer for verts
	void* bufferVertices;

	// Lock verts
	myVertexBuffer->Lock( 0, 0, &bufferVertices, 0);

	// Length is in bytes, make it ints
	int cl = leng / 4;

	// Get DX and Irrlicht verts
	SAnimatedVert* DXBuf = (SAnimatedVert*)bufferVertices;
	video::S3DVertexTangents* src = (video::S3DVertexTangents*)buffer->getVertices();



	// Loop through every irrvert
	for(int i = 0; i < buffer->getVertexCount(); ++i)
	{
	
		SAnimatedVert NV;
		NV.Pos = src[i].Pos;
		NV.Normal = src[i].Normal;
		NV.Tangent = src[i].Tangent;
		NV.Binormal = src[i].Binormal;
		NV.Color = src[i].Color;
		NV.TexCoord1 = src[i].TCoords;
		NV.TexCoord2 = src[i].TCoords2;
		NV.BlendIndex[0] = 0;
		NV.BlendWeights[0] = 1.0f;
		NV.BlendIndex[1] = 0;
		NV.BlendWeights[1] = 1.0f;
		NV.BlendIndex[2] = 0;
		NV.BlendWeights[2] = 1.0f;
		NV.BlendIndex[3] = 0;
		NV.BlendWeights[3] = 1.0f;
		//NV.Tangent = vector3df(1,0,0);



		if(Blends && Blends->size())
			for(int f = 0; f < Blends->size(); ++f)
				if((*Blends)[f].Vert == &src[i])
				{
					for(int g = 0; g < 4; ++g)
					{
						NV.BlendIndex[g] = (unsigned char)(*Blends)[f].BlendIndex[g];
						NV.BlendWeights[g] = (*Blends)[f].BlendWeight[g];			
					}
					//NV.Tangent = (*Blends)[f].Tangent;
				}

		array<unsigned char> Indices;
		array<float> Weights;

		Indices.push_back(NV.BlendIndex[0]);
		Weights.push_back(NV.BlendWeights[0]);
		
		for(int f = 1; f < 4; ++f)
		{
			bool Found = false;
			for(int g = 0; g < Weights.size(); ++g)
			{
				if(NV.BlendWeights[f] > Weights[g] && !Found)
				{
					Weights.insert(NV.BlendWeights[f], g);
					Indices.insert(NV.BlendIndex[f], g);
					Found = true;
				}
			}

			if(!Found)
			{
				Weights.push_back(NV.BlendWeights[f]);
				Indices.push_back(NV.BlendIndex[f]);
			}
		}

		for(int f = 0; f < 4; ++f)
		{
			NV.BlendIndex[f] = Indices[f];
			NV.BlendWeights[f] = Weights[f];
		}




		DXBuf[i] = NV;


	}

	// Unlock
	myVertexBuffer->Unlock();

	// wewt
	return true;
}

bool CD3D9Driver::deleteHardwareBuffer(scene::IMeshBuffer* buffer)
{
//OutputDebugString("deleteHardwareBuffer");
	if (!buffer)
		return false;

	if (buffer->vertexB)
		((IDirect3DVertexBuffer9*)buffer->vertexB)->Release();

	if (buffer->indexB)
		((IDirect3DIndexBuffer9*)buffer->indexB)->Release();

	return true;
}

IDirect3DVertexDeclaration9* CD3D9Driver::GetStandardDefinition()
{
	return S3DVertexVDecl;
}

void CD3D9Driver::ResetVertexDeclarationCache()
{
	CurrentDecl = 0;
}

void CD3D9Driver::drawIndexedTriangleListHw(void* indexP,void* vertexP,int indexC,int vertexC,video::E_VERTEX_TYPE vt, int TriangleCount)
{
		PrimitivesDrawn += indexC / 3;

		pID3DDevice->SetIndices( (IDirect3DIndexBuffer9*)indexP );

		IDirect3DVertexDeclaration9* NewDecl = NULL;
		int FormatSize;
		switch(vt)
		{
		case video::EVT_STANDARD:
			NewDecl = S3DVertexVDecl;
			FormatSize = sizeof(S3DVertex);
			break;
		case video::EVT_2TCOORDS:
			NewDecl = S3DVertex2TCoordsVDecl;
			FormatSize = sizeof(S3DVertex2TCoords);
			break;
		case video::EVT_TANGENTS:
			NewDecl = S3DVertexTangentsVDecl;
			FormatSize = sizeof(S3DVertexTangents);
			break;
		case video::EVT_ANIMATEDVERT:
			NewDecl = SAnimatedVertVDecl;
			FormatSize = sizeof(SAnimatedVert);
			break;
		}

		if(NewDecl == NULL)
		{
			MessageBox(0, "Could not find a suitable vertex format when attempting to render a mesh.", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		pID3DDevice->GetVertexDeclaration(&CurrentDecl);
		if(NewDecl != CurrentDecl)
		{
			pID3DDevice->SetVertexDeclaration(NewDecl);
			CurrentDecl = NewDecl;
		}

		pID3DDevice->SetStreamSource(
			0,
			(IDirect3DVertexBuffer9*)vertexP,
			0,
			FormatSize);

		pID3DDevice->DrawIndexedPrimitive(
			D3DPT_TRIANGLELIST,
			0,
			0,
			vertexC,
			0,
			TriangleCount);
		
	//}
}

//! \return Returns the name of the video driver. Example: In case of the DIRECT3D9
//! driver, it would return "Direct3D9.1".
const wchar_t* CD3D9Driver::getName()
{
	return L"Direct3D 9.0";
}

//! Returns the maximum amount of primitives (mostly vertices) which
//! the device is able to render with one drawIndexedTriangleList
//! call.
s32 CD3D9Driver::getMaximalPrimitiveCount()
{
	return Caps.MaxPrimitiveCount;
}


//! Sets the fog mode.
void CD3D9Driver::setFog(SColor color, bool linearFog, f32 start,
	f32 end, f32 density, bool pixelFog, bool rangeFog)
{
	CNullDriver::setFog(color, linearFog, start, end, density, pixelFog, rangeFog);

	if (!pID3DDevice)
		return;
}

//! Draws a 3d line.
void CD3D9Driver::draw3DLine(const core::vector3df& start,
	const core::vector3df& end, SColor color)
{
	video::S3DVertex v[2];
	v[0].Color = color;
	v[1].Color = color;
	v[0].Pos = start;
	v[1].Pos = end;

	pID3DDevice->SetVertexDeclaration(S3DVertexVDecl);
	CurrentDecl = S3DVertexVDecl;
	pID3DDevice->DrawPrimitiveUP(D3DPT_LINELIST, 1, v, sizeof(S3DVertex));
}

void* CD3D9Driver::LoadFont(const char* Filename, int Size, int Bold, int Italic)
{
	// REMOVE
	IDirect3DDevice9* Device = pID3DDevice;
	ID3DXFont* Font;
	if(FAILED(D3DXCreateFont(Device, Size, 0, (Bold > 0) ? FW_BOLD : FW_NORMAL, 0, Italic, DEFAULT_CHARSET, OUT_DEFAULT_PRECIS, DEFAULT_QUALITY, DEFAULT_PITCH | FF_DONTCARE, Filename, &Font)))
		return 0;

	Fonts.push_back(Font);

	return Font;
}

void CD3D9Driver::FreeFont(void* Font)
{
	// REMOVE
	int Finder = -1;
	for(int i = 0; i < Fonts.size(); ++i)
	{
		if(Fonts[i] == Font)
		{
			Fonts[i]->Release();
			Finder = i;
		}
	}

	if(Finder > -1)
		Fonts.erase(Finder);
}

//! resets the device
bool CD3D9Driver::reset()
{
	// Clear textures in use
	for (int i=0; i<4; ++i)
	{
		if (CurrentTexture[i])
			CurrentTexture[i]->drop();

		CurrentTexture[i] = 0;

		pID3DDevice->SetTexture(i, 0);
	}
	

	// Drop the framebuffer
	BackBufferFrame->drop();
	delete BackBuffer2Frame;

	// Drop shadow buffer
	delete ShadowMapRT;
	delete ShadowMapBlur;
	ShadowMapRT = 0;
	ShadowMapBlur = 0;

	SAFE_DELETE(ReflectionMap);

	SAFE_DELETE(BackBuffer2);

	// Remove backbuffer
	//if(PPActive)
	//	BackBuffer->UnSet();
	//BackBuffer2->UnSet(false, false, 1);
	//delete BackBuffer;
	//delete BackBuffer2;

	// Post Processes
	PPReset();
	cRenderTargetManager::pRTM->ReleasePP_RenderTargets();

	// Lose effects
	for(int i = 1; i < EffectsList.size(); ++i)
	{
		EffectsList[i]->GetEffect()->OnLostDevice();
	}

	for(int i = 0; i < Fonts.size(); ++i)
	{
		Fonts[i]->OnLostDevice();
	}

	DO_LostDevice();

	// Call for list
	CallDeviceLostCallback();

	// Reset
	char OO[1024];
	sprintf(OO, "Interval: %i\n", present.PresentationInterval);
	OutputDebugString(OO);

	HRESULT Hr = pID3DDevice->Reset(&present);

	if(FAILED(Hr))
	{
		char ErrorOut[2048], ErrorLine[2048];

		switch(Hr)
		{
		case D3DERR_DEVICELOST:
			sprintf(ErrorLine, "D3DERR_DEVICELOST");
			break;
		case D3DERR_DEVICEREMOVED:
			sprintf(ErrorLine, "D3DERR_DEVICEREMOVED");
			break;
		case D3DERR_DRIVERINTERNALERROR:
			sprintf(ErrorLine, "D3DERR_DRIVERINTERNALERROR");
			break;
		case D3DERR_OUTOFVIDEOMEMORY:
			sprintf(ErrorLine, "D3DERR_OUTOFVIDEOMEMORY");
			break;
		case D3D_OK:
			sprintf(ErrorLine, "D3D_OK");
			break;
		default:
			sprintf(ErrorLine, "UNKNOWN ERROR");
			break;
		}

		sprintf(ErrorOut, "Error attempting to reset device!\n%s in %s:%i", ErrorLine, "::reset()", __LINE__);


		MessageBox(NULL, ErrorOut, "IDirect3DDevice9", MB_ICONERROR);
		exit(0);
	}

	// Recover effects
	for(int i = 1; i < EffectsList.size(); ++i)
	{
		EffectsList[i]->GetEffect()->OnResetDevice();
	}

	for(int i = 0; i < Fonts.size(); ++i)
	{
		Fonts[i]->OnResetDevice();
	}

	DO_ResetDevice();

	// Create the FrameBuffer
	TexBBFrame = NULL;
	TexBB2Frame = NULL;
	pID3DDevice->CreateTexture(this->present.BackBufferWidth, this->present.BackBufferHeight, 1, D3DUSAGE_RENDERTARGET, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT, &TexBBFrame, NULL);
	pID3DDevice->CreateTexture(this->present.BackBufferWidth, this->present.BackBufferHeight, 1, D3DUSAGE_RENDERTARGET, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT, &TexBB2Frame, NULL);
	BackBufferFrame = new CD3D9Texture(TexBBFrame, pID3DDevice, irr::core::dimension2d<irr::s32>(this->present.BackBufferWidth, this->present.BackBufferHeight), "FRAMEBUFFER");
	BackBuffer2Frame = new CD3D9Texture(TexBB2Frame, pID3DDevice, irr::core::dimension2d<irr::s32>(this->present.BackBufferWidth, this->present.BackBufferHeight), "FRAMEBUFFER2");

	// Remake the BackBuffer
	//BackBuffer = new cRT(pID3DDevice);
	//BackBuffer->Create(this->present.BackBufferWidth, this->present.BackBufferHeight, false, D3DFMT_UNKNOWN);
	BackBuffer2 = new cRT(pID3DDevice, D3DFMT_A8R8G8B8);
	BackBuffer2->Create(this->present.BackBufferWidth, this->present.BackBufferHeight, false, D3DFMT_UNKNOWN);
	BackBuffer2->ClearColor = 0x00000000;

	setShadowMapSize(getShadowMapSize());

	SAFE_DELETE(ReflectionMap);
	ReflectionMap= new cRT(pID3DDevice);
	ReflectionMap->Create( ViewPort.getWidth(), ViewPort.getHeight(), false, D3DFMT_UNKNOWN);
	ReflectionMap->ClearColor = 0xffffffff;


	// Remake PPChains
	PPSize(this->present.BackBufferWidth, this->present.BackBufferHeight);
	cRenderTargetManager::pRTM->CreatePP_RenderTargets(AntiAlias);

	// Reset the rendering vertex declaration
	CurrentDecl = NULL;

	if(Smgr->getActiveCamera() != 0)
		Smgr->getActiveCamera()->setAspectRatio((float)this->present.BackBufferWidth / (float)this->present.BackBufferHeight);
		//Smgr->getActiveCamera()->setFOV(Smgr->getActiveCamera()->getFOV());

	// Call for reset
	CallDeviceResetCallback();
	CallDeviceResetXYCallback((float)this->present.BackBufferWidth, (float)this->present.BackBufferHeight);

	D3DVIEWPORT9 VP;
	pID3DDevice->GetViewport(&VP);
	VP.X = VP.Y = 0;
	VP.Width = present.BackBufferWidth;
	VP.Height = present.BackBufferHeight;

	
	return true;
}


void CD3D9Driver::OnResize(const core::dimension2d<s32>& size)
{
	if (!pID3DDevice)
		return;

	CNullDriver::OnResize(size);
	present.BackBufferWidth = size.Width;
	present.BackBufferHeight = size.Height;

	reset();
}

//! Returns type of video driver
E_DRIVER_TYPE CD3D9Driver::getDriverType()
{
	return EDT_DIRECT3D9;
}


//! Returns the transformation set by setTransform
const core::matrix4& CD3D9Driver::getTransform(E_TRANSFORMATION_STATE state)
{
	return Matrices[state];
}

//! Returns pointer to the IGPUProgrammingServices interface.
IGPUProgrammingServices* CD3D9Driver::getGPUProgrammingServices()
{
	return this;
}


u32 CD3D9Driver::loadFXShader(const c8* ShaderProgram)
{
	OutputDebugString(ShaderProgram);
	OutputDebugString("\n");

	ID3DXBuffer* Buffer;
	ID3DXEffect* Effect;
	if(D3DXCreateBuffer(2048, &Buffer) != D3D_OK)
	{
		return 0;
	}

	// DX Seems to hate it if you pass the wrong filename, it should check that
	std::ifstream istrm;
	istrm.open(ShaderProgram, std::ios::in);
	if(!istrm.is_open())
		return 0;

	istrm.close();

	//if(D3DXCreateEffectFromFile(pID3DDevice, ShaderProgram, 0, 0, 0, 0, &Effect, &Buffer) != D3D_OK)

	// FIX FOR THE MARCH 2009 SDK - PRESHADER GENERATION IS BROKEN!
	DWORD Flags = 0;

	std::string ProgramPath = ShaderProgram;
	if(ProgramPath.find_last_of('\\') != std::string::npos)
	{
		size_t Pos = ProgramPath.find_last_of('\\');
		std::string Filename = ProgramPath.substr(Pos + 1);

		if(Filename.length() > 4)
			if(Filename.substr(0, 4) == "nps_")
				Flags |= D3DXSHADER_NO_PRESHADER;
	}

	
	if(D3DXCreateEffectFromFile(pID3DDevice, ShaderProgram, 0, 0, Flags, 0, &Effect, &Buffer) != D3D_OK)
	{
		char* Error = (char*)Buffer->GetBufferPointer();
		MessageBox(NULL, Error, "FX Shader Compilation Failed", MB_ICONERROR | MB_OK);
		return 0;
	}

	OutputDebugString(ShaderProgram);
	OutputDebugString("\n");

	this->EffectsList.push_back(new ShaderConstantSet(Effect, ShaderProgram));

	return this->EffectsList.size()-1;
}

void CD3D9Driver::ReloadShaders()
{
	for(int i = 0; i < EffectsList.size(); ++i)
	{
		ReloadEffect(i);
	}
}

void CD3D9Driver::ReloadEffect(u32 i)
{
	ShaderConstantSet* PrevSet = EffectsList[i];
	if(PrevSet == 0)
		return;

	const char* ShaderProgram = PrevSet->GetPath();

	// Make sure file exists
	std::ifstream istrm;
	istrm.open(ShaderProgram, std::ios::in);
	if(!istrm.is_open())
		return;
	istrm.close();

	// FIX FOR THE MARCH 2009 SDK - PRESHADER GENERATION IS BROKEN!
	DWORD Flags = 0;

	std::string ProgramPath = ShaderProgram;
	if(ProgramPath.find_last_of('\\') != std::string::npos)
	{
		size_t Pos = ProgramPath.find_last_of('\\');
		std::string Filename = ProgramPath.substr(Pos + 1);

		if(Filename.length() > 4)
			if(Filename.substr(0, 4) == "nps_")
				Flags |= D3DXSHADER_NO_PRESHADER;
	}

	ID3DXBuffer* Buffer;
	ID3DXEffect* Effect;
	if(D3DXCreateBuffer(2048, &Buffer) != D3D_OK)
	{
		return;
	}

	if(D3DXCreateEffectFromFile(pID3DDevice, ShaderProgram, 0, 0, Flags, 0, &Effect, &Buffer) != D3D_OK)
	{
		char* Error = (char*)Buffer->GetBufferPointer();
		MessageBox(NULL, Error, "FX Shader Compilation Failed", MB_ICONERROR | MB_OK);
		return;
	}

	ShaderConstantSet* NewSet = new ShaderConstantSet(Effect, ShaderProgram);
	EffectsList[i] = NewSet;
	delete PrevSet;
}

ID3DXEffect* CD3D9Driver::getEffect(u32 i)
{
	if(i == 65535)
		return 0;
	return this->EffectsList[i]->GetEffect();	
}


//! Returns a pointer to the IVideoDriver interface. (Implementation for
//! IMaterialRendererServices)
IVideoDriver* CD3D9Driver::getVideoDriver()
{
	return this;
}

//! Clears the ZBuffer.
void CD3D9Driver::clearZBuffer()
{
	HRESULT hr = pID3DDevice->Clear( 0, NULL, D3DCLEAR_ZBUFFER, 0, 1.0, 0);

	if (FAILED(hr))
		os::Printer::log("CD3D9Driver clearZBuffer() failed.", ELL_WARNING);
}

// returns the current size of the screen or rendertarget
core::dimension2d<s32> CD3D9Driver::getCurrentRenderTargetSize()
{
	if ( CurrentRendertargetSize.Width == 0 )
		return ScreenSize;
	else
		return CurrentRendertargetSize;
}

void CD3D9Driver::removeTexture(ITexture* texture)
{
	
	for (u32 i=0; i<Textures.size(); ++i)
		if (Textures[i].Surface == texture)
		{
			texture->drop();
			Textures.erase(i);
		}

}

void CD3D9Driver::SetClipPlane( int iPlane, plane3df plane )
{
	if (iPlane != -1) 
	{
		D3DXVECTOR4 v(plane.Normal.X, plane.Normal.Y, plane.Normal.Z, plane.D);
		pD3DDevice->SetRenderState( D3DRS_CLIPPLANEENABLE, 1 );
		pD3DDevice->SetClipPlane(0, (float*)&v );
	}
	else
	{
		pD3DDevice->SetRenderState( D3DRS_CLIPPLANEENABLE, 0 );
	}
}



} // end namespace video
} // end namespace irr

#endif // _IRR_COMPILE_WITH_DIRECT3D_9_
#endif // _IRR_WINDOWS_




namespace irr
{
namespace video
{

#if (defined(_IRR_WINDOWS_) || defined(_XBOX))
//! creates a video driver
IVideoDriver* createDirectX9Driver(const core::dimension2d<s32>& screenSize, HWND window,
				u32 bits, bool fullscreen, bool stencilbuffer,
				io::IFileSystem* io, bool pureSoftware, bool highPrecisionFPU,
				bool vsync, int antiAlias)
{
	#ifdef _IRR_COMPILE_WITH_DIRECT3D_9_
	CD3D9Driver* dx9 =  new CD3D9Driver(screenSize, window, fullscreen, stencilbuffer, io, pureSoftware);
	if (!dx9->initDriver(screenSize, window, bits, fullscreen, pureSoftware, highPrecisionFPU, vsync, antiAlias))
	{
		dx9->drop();
		dx9 = 0;
	}

	return dx9;

	#else

	return 0;

	#endif // _IRR_COMPILE_WITH_DIRECT3D_9_
}
#endif

} // end namespace video
} // end namespace irr

