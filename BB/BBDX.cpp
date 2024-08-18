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
#include "BBDX.h"


int Shader_LitObject1 = 0, Shader_FullbrightMultiply = 0;
int Shader_LitObject2 = 0, Shader_Terrain = 0, Shader_SAQuad = 0, Shader_FullbrightAlpha = 0, Shader_FullbrightAdd = 0;
int Shader_Animated = 0, Shader_Line = 0, Shader_Sky = 0, Shader_Cloud = 0;
int Shader_Water = 0;
uint TemplateMesh_Box = 0, TemplateMesh_Sphere = 0;

void Graphics3D(int Width, int Height, int Depth, int Fullscreen, int AntiAlias, int, std::string DefaultTexture)
{
	ackno(Width, Height, Depth, Fullscreen, AntiAlias, 0, DefaultTexture.c_str());
	TemplateMesh_Box = LoadMesh(std::string("Data\\Box.b3d"));
	if(TemplateMesh_Box == 0)
		RuntimeError(std::string("Could not load Data\\Box.b3d!"));
	TemplateMesh_Sphere = LoadMesh(std::string("Data\\Sphere.b3d"));
	if(TemplateMesh_Sphere == 0)
		RuntimeError(std::string("Could not load Data\\Sphere.b3d"));
	HideEntity(TemplateMesh_Box);
	HideEntity(TemplateMesh_Sphere);
}

void End3DGraphics()
{
	if(TemplateMesh_Box != 0)
		FreeEntity(TemplateMesh_Box);

	if(TemplateMesh_Sphere != 0)
		FreeEntity(TemplateMesh_Sphere);

	lickme();
}

void LoadDefaultShaders()
{
	Shader_LitObject1 = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\LitObject_High.fx"));
	if(Shader_LitObject1 == 0) RuntimeError(std::string("Could not load LitObject1 shader!"));
	//Shader_LitObject2 = LoadShader(String("Data\\Game Data\\Shaders\\Default\\LitObject2.fx"));
	//if(Shader_LitObject2 == 0) RuntimeError(String("Could not load LitObject2 shader!"));
	Shader_Terrain = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\Terrain.fx"));
	if(Shader_Terrain == 0) RuntimeError(std::string("Could not load Terrain shader!"));
	//Shader_Water = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\Water.fx"));
	//if(Shader_Water == 0) RuntimeError(std::string("Could not load Water shader!"));
	Shader_SAQuad = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\InterfaceQuad.fx"));
	if(Shader_SAQuad == 0) RuntimeError(std::string("Could not load SAQuad shader!"));
	Shader_FullbrightAlpha = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\FullbrightAlpha.fx"));
	if(Shader_FullbrightAlpha == 0) RuntimeError(std::string("Could not load FullbrightAlpha shader!"));
	Shader_FullbrightAdd = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\FullbrightAdd.fx"));
	if(Shader_FullbrightAdd == 0) RuntimeError(std::string("Could not load FullbrightAdd shader!"));
	Shader_FullbrightMultiply = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\FullbrightMultiply.fx"));
	if(Shader_FullbrightMultiply == 0) RuntimeError(std::string("Could not load FullbrightMultiply shader!"));

	Shader_Animated = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\AnimatedMesh_Medium.fx"));
	if(Shader_Animated == 0) RuntimeError(std::string("Could not load AnimatedMesh shader!"));

	Shader_Line = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\3DLine.fx"));
	if(Shader_Line == 0) RuntimeError(std::string("Could not load 3DLine shader!"));

	Shader_Sky = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\nps_Sky.fx")); //nps_
	if(Shader_Sky == 0) RuntimeError(std::string("Could not load Sky shader!"));
	Shader_Cloud = LoadShader(std::string("Data\\Game Data\\Shaders\\Default\\nps_Cloud.fx"));
	if(Shader_Cloud == 0) RuntimeError(std::string("Could not load cloud shader!"));
}

uint LoadMesh(std::string Filename, uint Parent, bool Animated, bbdx2_ASyncJobFn completionCallback, void* userData)
{
	uint EN = jalkming(Filename.c_str(), Parent, Animated ? 1 : 0, completionCallback, userData);
	if(EN != 0)
		EntityShader(EN, Shader_LitObject1);
	return EN;
}

