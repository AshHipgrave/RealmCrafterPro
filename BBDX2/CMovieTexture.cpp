//*****************************************************
//* MovieTexture DirectShow Resource                  *
//* Platform: Win32                                   *
//* Aut: Jared Belkus (jared.belkus@solstargames.com) *
//* Notes: None                                       *
//*****************************************************
//***************** LICENSE ***************************
//* Solstar Games, LLC. is granted a worldwide,       *
//* royalty free, perpetual, irrevocable license to   *
//* use, distribute, modify, publish, and otherwise   *
//* exploit the incorporated MovieTexture Resource in *
//* connection with Realm Crafter.                    *
//*****************************************************

// Include
#include "CMovieTexture.h"

// DirectShow Resource GUIDs
static GUID MY_CLSID_AMMultiMediaStream={0x49C47CE5,0x9BA4,0x11D0,0x82,0x12,0x00,0xC0,0x4F,0xC3,0x2C,0x45};
static GUID MY_IID_IAMMultiMediaStream={0xBEBE595C,0x9A6F,0x11D0,0x8F,0xDE,0x00,0xC0,0x4F,0xD9,0x18,0x9D};
static GUID MY_MSPID_PrimaryVideo={0xA35FF56A,0x9FDA,0x11D0,0x8F,0xDF,0x00,0xC0,0x4F,0xD9,0x18,0x9D};
static GUID MY_IID_IDirectDrawMediaStream={0xF4104FCE,0x9A70,0x11D0,0x8F,0xDE,0x00,0xC0,0x4F,0xD9,0x18,0x9D}; 
static GUID MY_MSPID_PrimaryAudio={0xA35FF56B,0x9FDA,0x11D0,0x8F,0xDF,0x00,0xC0,0x4F,0xD9,0x18,0x9D};

// Library
#pragma comment(lib, "ole32.lib")

CMovieTexture::CMovieTexture(const char* FileName,
		RegisterMovieTextureCallBack RegisterCallBack,
		RegisterMovieTextureCallBack UnRegisterCallBack, bool* Result)
{
	// Default Values
	CoInitialize(0);
	AMStream = 0;
	PrimaryVideoStream = 0;
	DDrawStream = 0;
	Sample = 0;
	Surface = 0;
	MovieBuffer = 0;
	Time = 10;
	FrameTime = 0;
	LastTick = timeGetTime();
	MoviePitch = 0;
	Texture = 0;
	Loop = true;

	RegisterFunction = RegisterCallBack;
	UnRegisterFunction = UnRegisterCallBack;

	// WString Filename
	WCHAR Buffer[1024];
	MultiByteToWideChar(CP_ACP, 0, FileName, -1, Buffer, 1024);
	
	// Create COM Instance
	CoCreateInstance(MY_CLSID_AMMultiMediaStream, 0, 1, MY_IID_IAMMultiMediaStream, (void**)&AMStream);
	
	// Setup the AM Stream
	AMStream->Initialize((STREAM_TYPE)0, 0, NULL);
	AMStream->AddMediaStream(0, &MY_MSPID_PrimaryVideo, 0, NULL);
	
	// Load movie
	if(AMStream->OpenFile(Buffer, 0) != S_OK)
	{
		char Output[2048];sprintf(Output, "Failed to load video file: %s\n", FileName);
		OutputDebugString(Output);
		Result[0] = false;
		return;
	}
	
	AMStream->GetMediaStream(MY_MSPID_PrimaryVideo, &PrimaryVideoStream);
	AMStream->GetDuration(&Duration);

	// Obtain video stream
	PrimaryVideoStream->QueryInterface(MY_IID_IDirectDrawMediaStream, (void**)&DDrawStream);

	DDrawStream->GetTimePerFrame(&FrameTime);
	Time = FrameTime / 10000;

	// Create the DDraw sampler that will hold the movie
	DDrawStream->CreateSample(NULL, NULL, 0, &Sample);

	// Get the actual surface of the sample
	Sample->GetSurface(&Surface, &MovieRect);

	// Play the movie
	AMStream->SetState((STREAM_STATE)1);

	// Register
	RegisterFunction(this);
}

CMovieTexture::~CMovieTexture()
{
	// Free up all resources
	if(PrimaryVideoStream)
		PrimaryVideoStream->Release();
	if(DDrawStream)
		DDrawStream->Release();
	if(Sample)
		Sample->Release();
	if(Surface)
		Surface->Release();
	if(AMStream)
		AMStream->Release();
	if(Texture)
		Texture->Release();
	CoUninitialize();

	// Unregister from host
	UnRegisterFunction(this);
}

int CMovieTexture::GetMovieWidth()
{
	return MovieRect.right - MovieRect.left;
}

int CMovieTexture::GetMovieHeight()
{
	return MovieRect.bottom - MovieRect.top;
}

int CMovieTexture::GetFPS()
{
	return Time;
}

bool CMovieTexture::IsLooping()
{
	return Loop;
}

void CMovieTexture::SetLooping(bool IsLooping)
{
	Loop = IsLooping;
}

void CMovieTexture::SetMovieTexture(IDirect3DTexture9* NewTexture)
{
	if(Texture)
		Texture->Release();

	if(NewTexture)
	{
		Texture = NewTexture;
		Texture->AddRef();
	}else
	{
		Texture = 0;
	}
}

void CMovieTexture::Update()
{
	// Update based on the movie FPS
	if(timeGetTime() - LastTick < Time)
		return;
	LastTick = timeGetTime();
	
	// Quick Sanity check
	if(Texture == 0 || Sample == 0)
		return;

	// Update the movie sample
	Sample->Update(0, NULL, NULL, 0);

	// Loop
	STREAM_TIME CurrentTime;
	AMStream->GetTime(&CurrentTime);
	if(CurrentTime >= Duration && Loop)
		AMStream->Seek(0);

	// Lock the DDraw Surface
	DDSURFACEDESC DDrawSurfaceDesc;
	DDrawSurfaceDesc.dwSize = sizeof(DDSURFACEDESC);
	if(Surface->Lock(NULL, &DDrawSurfaceDesc, DDLOCK_SURFACEMEMORYPTR | DDLOCK_WAIT, NULL) != S_OK)
		return;

	// Lock the Texture
	D3DLOCKED_RECT LockedRect;
	if(Texture->LockRect(0, &LockedRect, NULL, 0) != S_OK)
	{
		Surface->Unlock(NULL);
		return;
	}

	// Get texture info
	D3DSURFACE_DESC D3DSurfaceDesc;
	Texture->GetLevelDesc(0, &D3DSurfaceDesc);

	// Find the smallest width/height to copy (to prevent overflow)
	int CopyWidth = (DDrawSurfaceDesc.lPitch < (D3DSurfaceDesc.Width * 4)) ? DDrawSurfaceDesc.lPitch : (D3DSurfaceDesc.Width * 4);
	int CopyHeight = (DDrawSurfaceDesc.dwHeight < D3DSurfaceDesc.Height) ? DDrawSurfaceDesc.dwHeight : D3DSurfaceDesc.Height;

	// Scanline copy
	for(int y = 0; y < CopyHeight; ++y)
		memcpy((BYTE*)LockedRect.pBits + (y * CopyWidth), (BYTE*)DDrawSurfaceDesc.lpSurface + (y * CopyWidth), CopyWidth);
	
	// Unlock buffers
	Surface->Unlock(NULL);
	Texture->UnlockRect(0);	
}


IMovieTexture* LoadMovieTexture(
	const char* FileName,
	RegisterMovieTextureCallBack RegisterCallBack,
	RegisterMovieTextureCallBack UnRegisterCallBack)
{
	// Used to check for fails
	bool Result = true;

	// Create movie
	CMovieTexture* MovieTexture = new CMovieTexture(FileName, RegisterCallBack, UnRegisterCallBack, &Result);

	// Failed
	if(Result == false)
	{
		delete MovieTexture;
		return 0;
	}

	// Done
	return MovieTexture;
}
