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
#include "PPChains.h"
#include <stdio.h>

IDirect3DDevice9* D3DDevice;
core::stringc Directory;
int ScreenWidth, ScreenHeight;
float fScreenWidth, fScreenHeight;

// Post Processing types
#define PPTYPE_NONE 0
#define PPTYPE_TEXTURE 1
#define PPTYPE_EFFECT 2
struct PPObject
{
	virtual int GetType() { return PPTYPE_NONE; }
};

struct PPTexture : public PPObject
{
	int ID;
	IDirect3DTexture9* Texture;
	virtual int GetType() { return PPTYPE_TEXTURE; }
};

struct PPEffect : public PPObject
{
	int ID;
	ID3DXEffect* Effect;
	virtual int GetType() { return PPTYPE_EFFECT; }
};

array<PPObject*> PPObjects;
array<CPPEffect*> PPChain;
array<CPPEffect*> Chain;

void PPInit(IDirect3DDevice9* Device, int Width, int Height)
{
	D3DDevice = Device;
	Directory = "";
	ScreenWidth = Width;
	ScreenHeight = Height;
	fScreenWidth = (float)Width;
	fScreenHeight = (float)Height;
}

void PPReset()
{
	for(int i = 0; i < PPChain.size(); ++i)
	{
		delete PPChain[i]->RT;
		PPChain[i]->Effect()->OnLostDevice();
	}

}

void PPSize(int Width, int Height)
{
	ScreenWidth = Width;
	ScreenHeight = Height;

	for(int i = 0; i < PPChain.size(); ++i)
	{
		float WScale = 1.0f / (((float)ScreenWidth) / ((float)PPChain[i]->RT->RWidth));
		float HScale = 1.0f / (((float)ScreenHeight) / ((float)PPChain[i]->RT->RHeight));
		
		PPChain[i]->RT = new cRT(D3DDevice);
		PPChain[i]->RT->Create(Width * WScale, Height * HScale, false, D3DFMT_UNKNOWN);
		PPChain[i]->Effect()->OnResetDevice();
	}

	ScreenWidth = Width;
	ScreenHeight = Height;
}

void SetPPEffectDirectory(stringc &New)
{
	Directory = New;
}

void SetPPEffectActive(stringc &Tag, bool Active)
{
	for(int i = 0; i < PPChain.size(); ++i)
		if(PPChain[i]->Tag() == Tag)
			PPChain[i]->Active(Active);
}

bool GetPPEffectActive(stringc &Tag)
{
	for(int i = 0; i < PPChain.size(); ++i)
		if(PPChain[i]->Tag() == Tag)
			return PPChain[i]->Active();
	return false;
}

core::stringc Description, Name;
int Textures;
void TestPPEffect(const char* File)
{
	Description = "";
	Name = "";
	ID3DXEffect* H = LoadEffect(File);

	if(H->GetParameterBySemantic(NULL, "TextureStage0") != NULL)
		if(H->GetParameterBySemantic(NULL, "TextureStage1") != NULL)
			if(H->GetParameterBySemantic(NULL, "TextureStage2") != NULL)
				if(H->GetParameterBySemantic(NULL, "TextureStage3") != NULL)
					Textures = 4;
				else
					Textures = 3;
			else
				Textures = 2;
		else
			Textures = 1;
	else
		Textures = 0;

	LPCSTR Desc;
	H->GetString("Description", &Desc);

	LPCSTR N;
	H->GetString("Name", &N);

	Description = Desc;
	Name = N;
}

const char* GetName()
{
	return Name.c_str();
}

const char* GetDescription()
{
	return Description.c_str();
}

int GetTextures()
{
	return Textures;
}

// Load Post Processing Effect
ID3DXEffect* LoadEffect(const char* Name)
{
	// Return Handle
	ID3DXEffect* EffectHandle;
	
	// Load Effect
	if(FAILED(D3DXCreateEffectFromFile(D3DDevice,
		Name, NULL, NULL, NULL,
		NULL, &EffectHandle, NULL)))
	{
		// Error on fail
		char Out[1024];
		sprintf(Out, "Failed to load shader: %s", Name);
		MessageBox(0, Out, "", 0);
		exit(0);
	}

	// Return effect
	return EffectHandle;
}

IDirect3DTexture9* LoadTexture(const char* Name)
{
	// Return handle
	IDirect3DTexture9* TexHandle;

	// Load Texture
	if(FAILED(D3DXCreateTextureFromFile(D3DDevice, Name, &TexHandle)))
	{
		// Error on fail
		char Out[1024];
		sprintf(Out, "Failed to load texture: %s", Name);
		MessageBox(NULL, Out, "", 0);
		exit(0);
	}

	// Return texure
	return TexHandle;
}

ID3DXEffect* EffectFromID(int ID)
{
	// Loop through every object and return it if its ID matched
	for(int i = 0; i < PPObjects.size(); ++i)
		if(PPObjects[i]->GetType() == PPTYPE_EFFECT)
			if(((PPEffect*)PPObjects[i])->ID == ID)
				return ((PPEffect*)PPObjects[i])->Effect;

	// Return error if it failed
	char Out[1024];
	sprintf(Out, "Failed to find effect: %i", ID);
	MessageBox(NULL, Out, "", 0);
	exit(0);
	return 0;
}

IDirect3DTexture9* TextureFromID(int ID)
{
	// Find a texture object
	for(int i = 0; i < PPObjects.size(); ++i)
		if(PPObjects[i]->GetType() == PPTYPE_TEXTURE)
			if(((PPTexture*)PPObjects[i])->ID == ID)
				return ((PPTexture*)PPObjects[i])->Texture;

	// Find a render target from the chain!
	for(int i = 0; i < PPChain.size(); ++i)
		if(PPChain[i]->ID == ID)
			return PPChain[i]->RT->pTexture;
		
	// Error on fail
	char Out[1024];
	sprintf(Out, "Failed to find text: %i", ID);
	MessageBox(NULL, Out, "", 0);
	exit(0);
	return 0;
}

void LoadChain(const char* CFile)
{
	// Clear up all objects
	for(int i = 0; i < PPObjects.size(); ++i)
	{
		if(PPObjects[i]->GetType() == PPTYPE_EFFECT)
			((PPEffect*)PPObjects[i])->Effect->Release();
		else if(PPObjects[i]->GetType() == PPTYPE_TEXTURE)
			((PPTexture*)PPObjects[i])->Texture->Release();
		delete PPObjects[i];
	}

	// Clear the chain
	for(int i = 0; i < PPChain.size(); ++i)
		delete PPChain[i];
	PPObjects.clear();
	PPChain.clear();

	// Read file
	FILE* F = fopen(CFile, "r");
	if(F == 0)
	{
		MessageBox(0, "Failed to open Post Processing Chain File", "", 0);
		return;
	}

	// Locals
	int TextureCount, EffectCount, ChainCount;
	int TexID, EffID, CTC, CTI;
	float CW, CH;
	int SLen;
	char* SDat = (char*)malloc(2048);

	// Get amount of textures
	fread(&TextureCount, sizeof(int), 1, F);

	// Load each texture
	for(int i = 0; i < TextureCount; ++i)
	{
		ZeroMemory(SDat, 2048);
		fread(&TexID, sizeof(int), 1, F);
		fread(&SLen, sizeof(int), 1, F);
		fread(SDat, 1, SLen, F);
		
		// Create instance
		PPTexture* T = new PPTexture();
		T->ID = TexID;
		T->Texture = LoadTexture((Directory + stringc(SDat)).c_str());
		PPObjects.push_back(T);
	}
	
	// Get amount of effects
	fread(&EffectCount, sizeof(int), 1, F);

	// Load each effect
	for(int i = 0; i < EffectCount; ++i)
	{
		ZeroMemory(SDat, 2048);
		fread(&EffID, sizeof(int), 1, F);
		fread(&SLen, sizeof(int), 1, F);
		fread(SDat, 1, SLen, F);
		
		// Create instance
		PPEffect* E = new PPEffect();
		E->ID = EffID;
		E->Effect = LoadEffect((Directory + stringc(SDat)).c_str());
		PPObjects.push_back(E);
	}

	// Get amount of chain effects
	fread(&EffectCount, sizeof(int), 1, F);

	// Load each chain instance
	for(int i = 0; i < EffectCount; ++i)
	{
		ZeroMemory(SDat, 2048);
		fread(&TexID, sizeof(int), 1, F);
		fread(&SLen, sizeof(int), 1, F);
		fread(SDat, 1, SLen, F);
		fread(&EffID, sizeof(int), 1, F);
		fread(&CW, sizeof(float), 1, F);
		fread(&CH, sizeof(float), 1, F);
		fread(&CTC, sizeof(int), 1, F);

		// Get the correct width
		float Width = CW;
		float Height = CH;
		Width /= 100;
		Height /= 100;

		// Create instance
		CPPEffect* P = new CPPEffect(D3DDevice, fScreenWidth * Width, fScreenHeight * Height, fScreenWidth, fScreenHeight);
		P->Effect(EffectFromID(EffID));
		P->Tag(SDat);
		P->TextureCount = CTC;
		P->ID = TexID;

		// Load each instance texture
		for(int f = 0; f < CTC; ++f)
		{
			fread(&CTI, sizeof(int), 1, F);
			P->Textures[f] = TextureFromID(CTI);
		}

		// Store the chain
		PPChain.push_back(P);
	}

	fclose(F);
}

bool PreRenderChain()
{
	// Clear last post chain
	Chain.clear();

	// Create a new chain
	for(int i = 0; i < PPChain.size(); ++i)
		if(PPChain[i]->Active())
			Chain.push_back(PPChain[i]);

	// This is used to determine how the scene should be rendered. (Standard: No RTT for backbuffer, Refraction: No RTT for Alpha)
	return Chain.size() > 0;
}

void RenderChain(cRT* BackBuffer)
{
	
	// Can't render anything
	if(Chain.size() == 0)
		return;

	// Rendering just one, no RTT for final
	if(Chain.size() > 1)
		Chain[0]->RT->Set();
	
	// Render first
	Chain[0]->Render(BackBuffer->pTexture, BackBuffer->pTexture);
	
	// If rendering one, then just return, else unset the RTT
	if(Chain.size() > 1)
		Chain[0]->RT->UnSet();
	else
		return;

	// If we need to loop
	if(Chain.size() > 2)
	{
		// Loop from the second to second to last.
		for(int i = 1; i < PPChain.size() - 1; ++i)
		{
			Chain[i]->RT->Set();
			Chain[i]->Render(Chain[i - 1]->RT->pTexture, BackBuffer->pTexture);
			Chain[i]->RT->UnSet();
		}
	}

	// The final uses no RTT
	int i = Chain.size() - 1;
	Chain[i]->Render(Chain[i - 1]->RT->pTexture, BackBuffer->pTexture);

}