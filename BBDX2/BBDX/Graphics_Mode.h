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
//* BBDX - Graphics_Mode
//*

// DX9 Object for getting display resolutions
IDirect3D9* D3DObject;

// Count the modes
irr::u32 DisplayModeCount;

// Displaymodes
struct DisplayMode
{
	irr::u32 Width;
	irr::u32 Height;
};
 
// Displaymodes list
irr::core::array<DisplayMode> DisplayModes;
int AntiAliasWindowed = 0;
int AntiAliasFull = 0;

// Check DX Runtime
bool CheckDirectXVersion(int Version)
{
	// Build String
	stringc Filename = "d3dx9_";
	Filename += Version;
	Filename += ".dll";

	// Load Library
	HINSTANCE Lib = LoadLibrary((LPCSTR)Filename.c_str());

	// Return 0 if failed
	if(!Lib)
		return false;

	// Free
	FreeLibrary(Lib);

	// Done
	return true;
}

// Build displaymodes list
void LocalD3DInit()
{
	// Create a DX object
	D3DObject = Direct3DCreate9(D3D_SDK_VERSION);
	D3DDISPLAYMODE Modes;
	D3DFORMAT Fmt;

	// Get display mode for adapter
	D3DObject->GetAdapterDisplayMode(D3DADAPTER_DEFAULT, &Modes);
	Fmt = Modes.Format;

	// Get count of modes
	DisplayModeCount = D3DObject->GetAdapterModeCount(D3DADAPTER_DEFAULT, Fmt);

	// Loop through each mode
	for(irr::u32 i = 0; i < DisplayModeCount - 1; ++i)
	{
		// Get mode
		HRESULT hr = D3DObject->EnumAdapterModes(D3DADAPTER_DEFAULT, Fmt, i, &Modes);
		
		// Produce an error if need be
		if(hr != D3D_OK)
		{
			char err[256];
			sprintf(err,"There was an error attempting to obtain resolution capabilities for your graphics hardware: ");
			
			switch(hr)
			{
			case D3DERR_INVALIDCALL:
				sprintf(err,"%s Display mode count was over estimated %i",err,i);
				break;
			case D3DERR_NOTAVAILABLE:
				sprintf(err,"%s Hardware acceleration is not available, or the XRGB8 surface format is not supported",err);
				break;
			}

			MessageBox(NULL,err,"Enumeration Error!",MB_ICONERROR | MB_OK);
			exit(0);
		}
		
		// Add the mode
		DisplayMode Mode;
		Mode.Width = Modes.Width;
		Mode.Height = Modes.Height;

		DisplayModes.push_back(Mode);
	}



	for(int i = 2; i < 17; ++i)
	{
		DWORD QualityLevels = 0;
		if(!FAILED(D3DObject->CheckDeviceMultiSampleType(D3DADAPTER_DEFAULT,
		           D3DDEVTYPE_HAL, Fmt, TRUE,
		           (D3DMULTISAMPLE_TYPE)i, &QualityLevels)))
		{
			AntiAliasWindowed = i;
		}
		if(!FAILED(D3DObject->CheckDeviceMultiSampleType(D3DADAPTER_DEFAULT,
		           D3DDEVTYPE_HAL, D3DFMT_X8R8G8B8, FALSE,
		           (D3DMULTISAMPLE_TYPE)i, &QualityLevels)))
		{
			AntiAliasFull = i;
		}
	}

	// Check H/w Caps
	D3DCAPS9 Caps;
	D3DObject->GetDeviceCaps(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, &Caps);
	
	bool VSOK = true;
	bool PSOK = true;
	char SHDO[1024];

	if(Caps.VertexShaderVersion < D3DVS_VERSION(2, 0))
		VSOK = false;

	if(Caps.PixelShaderVersion < D3DPS_VERSION(2, 0))
		PSOK = false;

	if(VSOK == false && PSOK == false)
		sprintf(SHDO, "Error: Your graphics hardware does not support Vertex or Pixel Shader Version 2.0");
	else if(VSOK == false)
		sprintf(SHDO, "Error: Your graphics hardware does not support Vertex Shader Version 2.0");
	else if(PSOK == false)
		sprintf(SHDO, "Error: Your graphics hardware does not support Pixel Shader Version 2.0");
	
	if(VSOK == false || PSOK == false)
	{
		MessageBox(NULL, SHDO, "Runtime Error", MB_OK | MB_ICONERROR);
		exit(0);
	}

	// And we are done!
	D3DObject->Release();

}

DLLPRE int DLLEX CountMultipleSamples(int Windowed)
{
	if(Windowed > 0)
		return AntiAliasWindowed;
	return AntiAliasFull;
}

// Get the number of modes
DLLPRE irr::u32 DLLEX CountGraphicsModes()
{
	return DisplayModes.size();
}

// Get a single mode
DLLPRE DisplayMode* DLLEX GraphicsMode(int Index)
{
	return &DisplayModes[Index];
}

// Get a Width
DLLPRE irr::u32 DLLEX GraphicsModeWidth(int Index)
{
	return DisplayModes[Index].Width;
}

// Get a height
DLLPRE irr::u32 DLLEX GraphicsModeHeight(int Index)
{
	return DisplayModes[Index].Height;
}


