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
#pragma once

// Include
#include "IMovieTexture.h"
#include <windows.h>
#include <dshow.h>
#include <mmstream.h>
#include <amstream.h>
#include <ddstream.h>

class CMovieTexture : public IMovieTexture
{
private:
	IAMMultiMediaStream* AMStream;
	IMediaStream* PrimaryVideoStream;
	IDirectDrawMediaStream* DDrawStream;
	IDirectDrawStreamSample* Sample;
	IDirectDrawSurface* Surface;
	RECT MovieRect;
	LONG MoviePitch;
	void* MovieBuffer;
	DWORD Time;
	DWORD LastTick;
	RegisterMovieTextureCallBack RegisterFunction;
	RegisterMovieTextureCallBack UnRegisterFunction;
	IDirect3DTexture9* Texture;
	bool Loop;
	STREAM_TIME Duration;
	STREAM_TIME FrameTime;

public:

	CMovieTexture(const char* FileName,
		RegisterMovieTextureCallBack RegisterCallBack,
		RegisterMovieTextureCallBack UnRegisterCallBack, bool* Result);

	virtual ~CMovieTexture();
	virtual int GetMovieWidth();
	virtual int GetMovieHeight();
	virtual int GetFPS();
	virtual bool IsLooping();
	virtual void SetLooping(bool IsLooping);
	virtual void SetMovieTexture(IDirect3DTexture9* NewTexture);
	virtual void Update();

};