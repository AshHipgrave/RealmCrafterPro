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

// Include D3D9
#include <d3dx9.h>


class IMovieTexture;
typedef void (*RegisterMovieTextureCallBack)(IMovieTexture* MovieTexture);

//! MovieTexture Interface
class IMovieTexture
{
public:

	IMovieTexture(){}
	virtual ~IMovieTexture(){}
	
	//! Obtain the movie width
	virtual int GetMovieWidth() = 0;

	//! Obtain the movie height
	virtual int GetMovieHeight() = 0;

	//! Obtain the frame time of the movie (in frames per second)
	virtual int GetFPS() = 0;

	//! Check if the movie is looping
	virtual bool IsLooping() = 0;

	//! Set the movie loop mode (True = loop)
	virtual void SetLooping(bool IsLooping) = 0;

	//! Set the texture on which the movie should render
	virtual void SetMovieTexture(IDirect3DTexture9* NewTexture) = 0;

	//! Update the movie texture
	virtual void Update() = 0;
};

//! Load a movie texture
/*! Loads the specified movie and returns a handle. Callbacks are used to register the movie
with your custom engine. This allows you to keep track of active movies.
\param FileName Path to movie resource (As DShow compatible AVI, WMV of DIVX)
\param RegisterCallBack Function to call when movie loads
\param UnRegisterCallBack Function to call when movie is disposed
*/
IMovieTexture* LoadMovieTexture(
	const char* FileName,
	RegisterMovieTextureCallBack RegisterCallBack,
	RegisterMovieTextureCallBack UnRegisterCallBack);