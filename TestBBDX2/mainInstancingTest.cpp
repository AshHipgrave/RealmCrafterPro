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
#define NOMINMAX
#include <BlitzPlus.h>
#include "..\RCClient\Temp.h"
#include "BBDX.h"

using namespace RealmCrafter;

uint Camera;

//int __stdcall WinMain(HINSTANCE, HINSTANCE, char*, int)
int main()
{
	uint hwnd = BBCreateWindow("GAME", 0, 0, 800, 600, 0, 1);
	Init(BBQueryObject(hwnd, 1), 0);
	Graphics3D(800, 600, 32, 2, 0, 0, "Data\\DefaultTex.png");

	Camera = CreateCamera();
	CameraClsColor(Camera, 0, 128, 255);
	CameraRange(Camera, 1, 3200);
	CameraFogMode(Camera, 1);
	CameraFogColor(Camera, 255, 255, 255);
	CameraFogRange(Camera, 500, 3200);

	PositionEntity(Camera, -50, 100, -50);
	RotateEntity(Camera, 20, -45, 0);

	AmbientLight(100, 100, 100);
	
	/*uint Solid = LoadShader("Data\\Game Data\\Shaders\\Default\\LitObject_Far.fx");
	uint Foliage = LoadShader("Data\\Game Data\\Shaders\\RapidShader_Final\\Foliage.fx");*/

	/*for(int i = 0; i < 2000; ++i)
	{
		uint obj = LoadMesh("Data\\Meshes\\foliage_\\fir_tree01_lod01.b3d");
		uint obj_lod1 = LoadMesh("Data\\Meshes\\foliage_\\fir_tree01_lod03.b3d");

		EntityShader(obj, Foliage);
		EntityShader(obj_lod1, Foliage);

		ScaleEntity(obj, 0.05f, 0.05f, 0.05f);
		ScaleEntity(obj_lod1, 0.05f, 0.05f, 0.05f);

		setMeshLOD(obj, 0, 0, 300.0f);
		setMeshLOD(obj, obj_lod1, 1, 2000.0f);
		setMeshLOD(obj, 0, 2, 2010.0f);

		FreeEntity(obj_lod1);

		PositionEntity(obj, Rnd(0, 1000), 0, Rnd(0, 1000));
	}*/

// 	LoadUserDefinedPP_FromXML("Data\\Game Data\\PostProcess\\PostProcess.xml");
// 	SetPP_Pipeline("Default");

	// Main Loop
	while(!KeyHit(1) || JoyHit(XBOX_BACK))
	{
		float MoveSpeed = 0.05f * 5.0f;

		

 		/*if(KeyDown(17)) MoveEntity(Camera, 0, 0, 3.1f * MoveSpeed);
 		if(KeyDown(31)) MoveEntity(Camera, 0, 0, -3.1f * MoveSpeed);
 		if(KeyDown(30)) MoveEntity(Camera, -2 * MoveSpeed, 0, 0);
 		if(KeyDown(32)) MoveEntity(Camera, 2 * MoveSpeed, 0, 0);

		if(KeyDown(200)) TurnEntity(Camera, -2.8f * MoveSpeed, 0, 0);
		if(KeyDown(208)) TurnEntity(Camera,  2.8f * MoveSpeed, 0, 0);
		if(KeyDown(203)) TurnEntity(Camera, 0,  2.8f * MoveSpeed, 0);
		if(KeyDown(205)) TurnEntity(Camera, 0, -2.8f * MoveSpeed, 0);*/

		BBWaitEvent(0);

		UpdateWorld();
		RenderWorld();
	}

	exit(0);
	End3DGraphics();


	exit(0);
}
