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
//#define _CRT_NONSTDC_NO_DEPRECATE
#pragma warning(disable : 4995)


// Includes
#include "Main.h"
//#include "..\NuclearGlory\NGC.h"

#include "Texture.h"
#include "SAText.h"
#include "SAQuad.h"
#include "Global.h"
#include "Brush.h"
#include "Mesh.h"
#include "Surface.h"
#include "Camera.h"
#include "Light.h"
#include "Pivot.h"
#include "Entity_State.h"
#include "Entity_Movement.h"
#include "Entity_Animation.h"
#include "Sounds.h"
#include "Entity_Collision.h"
#include "Entity_Control.h"
#include "3D_Maths.h"
#include "Graphics_Mode.h"
#include "ThreadLoaders.h"
#include "ShaderFunctions.h"
#include "LineSceneNode.h"
#include "ToB3d.h"
#include "..\PPChains.h"
#include "IOThread.h"

struct SUnmanagedToManagedTexture
{
	int Loaded;
	IDirect3DTexture9* Texture;
	void* LockedBits;
	unsigned int Pitch;
	unsigned int Width;
	unsigned int Height;
	int Format;
};


// Reload shaders
DLLPRE void bbdx2_ReloadShaders()
{
	((irr::video::CD3D9Driver*)driver)->ReloadShaders();
}

DLLPRE void bbdx2_ReloadShader(unsigned int i)
{
	((irr::video::CD3D9Driver*)driver)->ReloadEffect(i);
}

DLLPRE void bbdx2_FreeTextureBits(SUnmanagedToManagedTexture* texInfo)
{
	if(texInfo->Loaded == 0)
		return;

	texInfo->Texture->UnlockRect(0);
	texInfo->Texture->Release();
	texInfo->Loaded = 0;
}

DLLPRE void bbdx2_LoadTextureBits(const char* path, SUnmanagedToManagedTexture* texInfo)
{
	texInfo->Loaded = 0;
	texInfo->Texture = 0;
	texInfo->LockedBits = 0;
	texInfo->Pitch = 0;
	texInfo->Width = 0;
	texInfo->Height = 0;
	texInfo->Format = 0;

	IDirect3DTexture9* TTexture;
	D3DXIMAGE_INFO Image;

	if(D3DXCreateTextureFromFileEx(
		((irr::video::CD3D9Driver*)driver)->pID3DDevice,	// pDevice
		path,												// pSrcFile
		D3DX_DEFAULT,										// Width
		D3DX_DEFAULT,										// Height
		1,													// MipLevels
		0,													// Usage
		D3DFMT_A8R8G8B8,									// Format
		D3DPOOL_MANAGED,									// Pool
		D3DX_DEFAULT,										// Filter
		D3DX_DEFAULT,										// MipFilter
		0,													// ColorKey
		&Image,												// pSrcInfo
		NULL,												// pPalette
		&TTexture)											// ppTexture
		!= D3D_OK)
	{
		return;
	}else
	{
		texInfo->Loaded = 1;
	}

	texInfo->Texture = TTexture;
	texInfo->Width = Image.Width;
	texInfo->Height = Image.Height;

	D3DLOCKED_RECT LockedRect;
	TTexture->LockRect(0, &LockedRect, NULL, D3DLOCK_READONLY);

	texInfo->Pitch = LockedRect.Pitch;
	texInfo->LockedBits = LockedRect.pBits;

	if(LockedRect.Pitch != texInfo->Width * 4)
	{
		TTexture->UnlockRect(0);
		TTexture->Release();
		texInfo->Loaded = 0;
	}

	return;
}

DLLPRE void** bbdx2_GetPointLights(int* Count, unsigned int* Stride)
{
	return smgr->GetPointLights(Count, Stride);
}

DLLPRE void bbdx2_GetDirectionalLights(float** Directions, float** Colors)
{
	smgr->GetDirectionalLights(Directions, Colors);
}

DLLPRE void* bbdx2_GetViewMatrixPtr()
{
	/*matrix4* M = new matrix4();
	*M = smgr->getActiveCamera()->getViewMatrix();
	return M;*/
	return (void*)&smgr->getActiveCamera()->getViewMatrix();
}

DLLPRE void* bbdx2_GetProjectionMatrixPtr()
{
	/*matrix4* M = new matrix4();
	*M = smgr->getActiveCamera()->getProjectionMatrix();

	//char O[2048];
	//sprintf(O, "%f, %f, %f, %f\n%f, %f, %f %f\n%f, %f, %f, %f\n%f, %f, %f, %f",
	//	M->M[0],  M->M[1],  M->M[2],   M->M[3], 
	//	M->M[4],  M->M[5],  M->M[6],   M->M[7], 
	//	M->M[8],  M->M[9],  M->M[10],  M->M[11], 
	//	M->M[12], M->M[13], M->M[14], M->M[15]);
	//MessageBox(0, O, "", 0);


	return M;*/
	return (void*)&smgr->getActiveCamera()->getProjectionMatrix();
}

DLLPRE void bbdx2_FreeMatrixPtr(void* M)
{
	//delete M;
}

DLLPRE void LoadPPChain(const char* File)
{
	LoadChain(File);
}

DLLPRE void SetPPFolder(const char* Folder)
{
	SetPPEffectDirectory(irr::core::stringc(Folder));
}

DLLPRE void SetPPActive(const char* Effect, int Active)
{
	SetPPEffectActive(irr::core::stringc(Effect), Active > 0);
}

DLLPRE int GetPPActive(const char* Effect)
{
	return GetPPEffectActive(irr::core::stringc(Effect)) ? 1 : 0;
}

DLLPRE void TestPP(const char* File)
{
	TestPPEffect(File);
}

DLLPRE const char* PPName()
{
	return GetName();
}

DLLPRE const char* PPDescription()
{
	return GetDescription();
}

DLLPRE int PPTextures()
{
	return GetTextures();
}


#define DWrite(x) fwrite(x, 4, 1, FOut)
#define FWrite(x) Fl = x; fwrite(&Fl, 4, 1, FOut)
DLLPRE void DLLEX BinDump(IAnimatedMeshSceneNode* Node, const char* File)
{
	float Fl;
	int In;
	
	FILE* FOut = fopen(File, "wb");

	IMesh* Mesh = Node->getLocalMesh()->getMesh(0);

	In = Mesh->getMeshBufferCount();
	DWrite(&In);

	for(int i = 0; i < Mesh->getMeshBufferCount(); ++i)
	{
		IMeshBuffer* Buffer = Mesh->getMeshBuffer(i);
		if(Buffer->getVertexType() != irr::video::EVT_2TCOORDS)
		{
			MessageBox(NULL, "Error dumping mesh!", "AHH", MB_OK);
			exit(0);
		}
		
		irr::video::S3DVertex2TCoords* Verts = (irr::video::S3DVertex2TCoords*)Buffer->getVertices();
		irr::u32* Indices = Buffer->getIndices();

		In = Buffer->getVertexCount();
		DWrite(&In);

		for(int f = 0; f < Buffer->getVertexCount(); ++f)
		{
			FWrite(Verts[f].Pos.X);
			FWrite(Verts[f].Pos.Y);
			FWrite(Verts[f].Pos.Z);
			FWrite(Verts[f].Normal.X);
			FWrite(Verts[f].Normal.X);
			FWrite(Verts[f].Normal.X);
			FWrite(Verts[f].TCoords.X);
			FWrite(Verts[f].TCoords.Y);
		}

		In = Buffer->getIndexCount();
		DWrite(&In);

		fwrite(Indices, 4, In, FOut);

		int TC = 0;
		for(int f = 0; f < 4; ++f)
			if(Buffer->getMaterial().Textures[f] > 0)
				TC++;

		DWrite(&TC);
	
		for(int f = 0; f < 4; ++f)
			if(Buffer->getMaterial().Textures[f] > 0)
			{
				irr::core::stringc TexName = Buffer->getMaterial().Textures[f]->getName();
				int TL = TexName.size();
				DWrite(&TL);
				fwrite(TexName.c_str(), 1, TL, FOut);
			}



	}
	
	fclose(FOut);


}

struct ConvertInput
{
	char Dat[2048];
};

const irr::c8* Realapplepie(const irr::c8* File);
const irr::c8* Realcherrypie(const irr::c8* File);
void Realapples(ConvertInput* In);
void Realpears(ConvertInput* In);

DLLPRE const irr::c8* DLLEX applepie(const irr::c8* File)
{
	SEHSTART;

	return Realapplepie(File);

	SEHEND;
}

DLLPRE const irr::c8* DLLEX cherrypie(const irr::c8* File)
{
	SEHSTART;

	return Realcherrypie(File);

	SEHEND;
}

DLLPRE void DLLEX apples(ConvertInput* In)
{
	SEHSTART;

	Realapples(In);

	SEHEND;
}

DLLPRE void DLLEX pears(ConvertInput* In)
{
	SEHSTART;

	Realpears(In);

	SEHEND;
}

// Older Conversion functions (unused)
const irr::c8* Realapplepie(const irr::c8* File)
{
	irr::core::stringc Source = File;
	irr::core::stringc Dest = Convert3DStoB3D(File);
	return Dest.c_str();
}

const irr::c8* Realcherrypie(const irr::c8* File)
{
	irr::core::stringc Source = File;
	irr::core::stringc Dest = ConvertXtoB3D(File);
	return Dest.c_str();
}



// New Converter (X to B3D)
void Realapples(ConvertInput* In)
{
	stringc D = ConvertXtoB3D(stringc(In->Dat)).c_str();
	const irr::c8* S = D.c_str();

	for(int i = 0; i < D.size(); ++i)
		In->Dat[i] = S[i];

	for(int i = D.size(); i < 2048; ++i)
		In->Dat[i] = 0;
}

// New Converter (X to B3d)
void Realpears(ConvertInput* In)
{
	stringc D = Convert3DStoB3D(stringc(In->Dat)).c_str();
	const irr::c8* S = D.c_str();

	for(int i = 0; i < D.size(); ++i)
		In->Dat[i] = S[i];

	for(int i = D.size(); i < 2048; ++i)
		In->Dat[i] = 0;
}

// Force alpha rendering on a object
DLLPRE void DLLEX SetAlphaState(ISceneNode* node, int mode)
{
	if(mode == 0)
		node->ForceAlpha = false;		
	else
		node->ForceAlpha = true;
}

// Check if alpha is forces
DLLPRE int DLLEX GetAlphaState(ISceneNode* node)
{
	if(node->ForceAlpha == true)
		return 1;
	else
		return 0;
}

// Get texture name from a surface
DLLPRE const irr::c8* DLLEX TexNameFromSurf(IAnimatedMeshSceneNode* node,int Index, int Surf)
{
	return node->getMaterial(Surf).Textures[Index]->getName().c_str();
}

DLLPRE void DLLEX reghoop(HWND hwnd)
{
	hWnd = hwnd;
}

// Initalise renderer
DLLPRE void DLLEX ghoop(HWND hwnd, HINSTANCE hinstance)
{
	SEHSTART;

	// Setup our windows information (0's if a new wndow is to be made)
    hWnd = hwnd;
    hInstance = hinstance;

	// Include the collisions funcs (June07 - NGC temporarily included)
	//#include "NGC_L.h"

	// Check the correct DX Runtime is installed
	if(!CheckDirectXVersion(33))
	{
		MessageBox(NULL, "Error: Please install the latest version of Direct X", "BBDX Error", MB_OK | MB_ICONERROR);
		exit(0);
	}

	// Setup Sound and local D3D device
	SoundSetup();
	LocalD3DInit();
	IOThreadInit();

	SEHEND;
}

// Texture an object
DLLPRE void DLLEX smokinhot(ISceneNode* node, ITexture* tex, int forgetframe, irr::s32 index, irr::s32 surface)
{
	if((int)tex == -1)
		tex = driver->getDefaultTexture();

	// If a surface was specified
	if(surface > -1)
	{
		// Free the old texture
		if(node->getMaterial(surface).Textures[index])
			emokid(node->getMaterial(surface).Textures[index]);

		// Set the new texture
		node->getMaterial(surface).Textures[index] = tex;

		//char OO[1024];
		//sprintf(OO, "Apply: %s, %i -> ", tex->getName().c_str(), tex->ReferenceCounter);

		// Increase Ref Counter
		if(tex != 0)
			tex->grab();

		//sprintf(OO, "%s%i\n", OO, tex->ReferenceCounter);
		//OutputDebugString(OO);

		return;
	}

	// Free every texture
	for(irr::s32 i = 0; i < node->getMaterialCount(); ++i)
		if(node->getMaterial(i).Textures[index])
			emokid(node->getMaterial(i).Textures[index]);

	// Set every texture
    node->setMaterialTexture(index, tex);

	// If texture was being removed, then return here
	if(tex == 0)
		return;

	// Increase reference counter
	for(irr::u32 i = 0; i < node->getMaterialCount(); ++i)
		tex->grab();
}

// Wrapper for End();
DLLPRE void DLLEX fathom()
{
	exit(1);
}


DLLPRE void DLLEX WriteSceneGraph(const char* FileName)
{
	smgr->WriteScene(FileName);
}

// CameraClsColor and ClsColor
DLLPRE void DLLEX witchcraft(int r, int g, int b)
{
	BackGround = SColor(255,r,g,b);
}

//////////////////////////////////////////////////////////////////////////
// Post Process Exported Functions
//////////////////////////////////////////////////////////////////////////
DLLPRE void DLLEX LoadUserDefinedPP_FromXML( const char* xmlName )
{
	XMLReader* X = ReadXMLFile( xmlName );
	if(X == 0)
	{
		MessageBoxA(NULL, "Could not open UserDefinedPostProcess file!", "Warning", 0);
		return;
	}//	RuntimeError( string( "Could not open " + xmlName).c_str() );

	// Get values
	while(X->Read())
	{
		// If its a <cfg> element
		if(X->GetNodeType() == XNT_Element && X->GetNodeName().AsLower() == String("postprocess"))
		{
			cPP_Effect* pPP_Effect = new cPP_Effect();
			lpPP_Effects.push_back( pPP_Effect );
			pPP_Effect->LoadFromXML( X );
		}
	}

	// Close file
	delete X;
}

DLLPRE void DLLEX AddPP_Effect( const char* EffectName, const char* ShaderSource )
{
	cEffect *pEffect = new cEffect;
		pEffect->Name = EffectName;
		pEffect->Shader = ShaderSource;
	ActivePP_Effect->lpEffects.push_back( pEffect );
}

DLLPRE void DLLEX SwapPP_Effect( int indexPP0, int indexPP1 )
{
	std::list<cEffect *>::iterator itEffect0(ActivePP_Effect->lpEffects.begin());
	for ( int i(0); i!= indexPP0; i++ ) itEffect0++;

	std::list<cEffect *>::iterator itEffect1(ActivePP_Effect->lpEffects.begin());
	for ( int i(0); i!= indexPP1; i++ ) itEffect1++;

	cEffect* p = *itEffect0;
	*itEffect0 = *itEffect1;
	*itEffect1 = p;
}

DLLPRE void DLLEX DeletePP_Effect( int IndexPP )
{
	std::list<cEffect *>::iterator itEffect(ActivePP_Effect->lpEffects.begin());
	for ( int i(0); i!= IndexPP; i++ ) itEffect++;

 	SAFE_DELETE( *itEffect );
	ActivePP_Effect->lpEffects.erase( itEffect );
}

DLLPRE void DLLEX AddEffect_Param( const char* Effect, const char* Param, const char* Type, const char* Value )
{
	for ( std::list<cEffect*>::iterator it(ActivePP_Effect->lpEffects.begin()); 
			it!= ActivePP_Effect->lpEffects.end(); it++)
		if ( (*it)->Name == String(Effect) ) 
		{	
			cParam *pParam = new cParam;
				(*it)->lpParams.push_back( pParam );
				pParam->Name = Param;
				pParam->SetValue( Type, Value );
			break; 
		}
}

DLLPRE void DLLEX SetEffect_Param( const char* Effect, const char* Param, const char* Value )
{
	for ( std::list<cEffect*>::iterator it(ActivePP_Effect->lpEffects.begin()); 
		it!= ActivePP_Effect->lpEffects.end(); it++)
		if ( (*it)->Name == String(Effect) ) {	(*it)->SetParameter( Param, Value ); break; }
}

DLLPRE void DLLEX EnablePP_Pipeline( bool state )	{	bEnablePP_Pipeline = state;	}
DLLPRE void DLLEX CleanPP_Pipeline()
{
	FOR_EACH_VALUE( cEffect*, pEffect, std::list<cEffect*>, ActivePP_Effect->lpEffects)
		SAFE_DELETE( pEffect );

	ActivePP_Effect->lpEffects.clear();
}

DLLPRE bool DLLEX SetPP_Pipeline( const char* PP_Name )
{
	FOR_EACH_VALUE( cPP_Effect*, pPP_Effect, std::list<cPP_Effect *>, lpPP_Effects)
	{
		if(pPP_Effect->Name == String(PP_Name))
		{
			ActivePP_Effect = pPP_Effect;
			return true;
		}
	}

	ActivePP_Effect = NULL;
	return false;
}

