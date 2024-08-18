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
//* BBDX - Camera
//*

// Camera globals
irr::f32 FogNear;
irr::f32 FogFar;
bool FogOn;
::SColor FogColor;

// Fog Mode
DLLPRE void DLLEX nofear(ICameraSceneNode* Camera, int Mode)
{
	// No Fog
	if(Mode == 0)
	{
		FogColor.set(0,0,0,0);
		FogNear = 0xFFFE;
		FogFar = 0XFFFF;
		FogOn = false;
	}else // Fog
	{
		FogNear = 1.0f;
		FogFar = 1000.0f;
		FogColor.set(0,0,0,0);
		FogOn = true;
	}

	driver->setFog(FogColor, true, FogNear, FogFar);
}

// Get Fog near
DLLPRE float DLLEX bbdx2_GetFogNear()
{
	return FogNear;
}

// Get Fog far
DLLPRE float DLLEX bbdx2_GetFogFar()
{
	return FogFar;
}

DLLPRE unsigned int DLLEX bbdx2_GetFogColor()
{
	return FogColor.color;
}

// Set Fog
DLLPRE void DLLEX kevlarboobs(ICameraSceneNode* Cam, irr::f32 Near, irr::f32 Far)
{
	// Check its enabled for the camera
	if(FogOn == true)
	{
		if(Near >= Far)
			Near = Far - 1;

		FogNear = Near;
		FogFar = Far;
		driver->setFog(FogColor, true, FogNear, FogFar);
	}
}

// Fog Color
DLLPRE void DLLEX goodoldkis(ICameraSceneNode* Cam, int r, int g, int b)
{
	// Check its enabled first
	if(FogOn == true)
	{
		FogColor.set(0, r, g, b);
		driver->setFog(FogColor, true, FogNear, FogFar);
	}
}

// Projection mode of camera
DLLPRE void DLLEX jackhammer(ICameraSceneNode* Camera, int Mode)
{
	// Blitz Index
	--Mode;

	// Disable camera
	if(Mode == -1)
	{
		Camera->setVisible(false);
		Camera->setFOV(0);
	}else
	{
		// Enable camera
		Camera->setVisible(true);

		// Ortho Camera
		if(Mode > 0)
			Camera->setIsOrthogonal(1);
		else
			Camera->setIsOrthogonal(0);

		// Reset FOV
		Camera->setFOV(90.0f * 180.0f / ((irr::f32)GRAD_PI2));
	}
}

// Add a camera node
DLLPRE ICameraSceneNode* DLLEX burryjimpol(ISceneNode* Parent)
{
	// Create the camera
	ICameraSceneNode* Node = smgr->addCameraSceneNodeFPS(Parent, 100, 500, -1, 0, 0, 0);
	
	// Setup Camera (Its not allowed to listen for the keyboard)
	Node->setInputReceiverEnabled(false);
	Node->CollisionID = GetCollisionID();

	// Set Field of View
	//Node->setFOV(90.0f * 180.0f / ((irr::f32)GRAD_PI2));

	// NGC Instructed us to use this
	//SetOctreeBuildPRO(0);

	// Default FOG
	nofear(Node, 0);

    return Node;
}

DLLPRE void DLLEX bbdx2_CameraClsColorAlpha(ICameraSceneNode* node, int r, int g, int b, int a)
{
	BackGround = SColor(a, r, g, b);
}

// Set CLS Color
DLLPRE void DLLEX knowham(ICameraSceneNode* Node, int r, int g, int b)
{
    BackGround = SColor(255, r, g, b);
}

// Set View Range
DLLPRE void DLLEX makemegame(ICameraSceneNode* Node, irr::f32 Near, irr::f32 Far)
{
	Node->setNearValue(Near);
	Node->setFarValue(Far);
}

// Projection/picking
irr::f32 ProjX, ProjY, ProjZ;

// Return Projected X
DLLPRE irr::f32 DLLEX ProjectedX()
{
	return ProjX;
}

// Return Projected Y
DLLPRE irr::f32 DLLEX ProjectedY()
{
	return ProjY;
}

// Return Projected Z
DLLPRE irr::f32 DLLEX ProjectedZ()
{
	return ProjZ;
}

// Get 2D coordinates from 3D ones
DLLPRE void DLLEX PointProject(irr::f32 x, irr::f32 y, irr::f32 z)
{
	// Get viewport and make a 2D vector
	irr::core::rect<irr::s32> ViewPort = driver->getViewPort();
	irr::core::dimension2d<irr::s32> Dimensions(ViewPort.getWidth(), ViewPort.getHeight());

	Dimensions.Width /= 2;
	Dimensions.Height /= 2;

	irr::f32 TransformedPos[4];
	irr::core::matrix4 Trans = smgr->getActiveCamera()->getProjectionMatrix();
	Trans *= smgr->getActiveCamera()->getViewMatrix();

	// Set Vector
	TransformedPos[0] = x;
	TransformedPos[1] = y;
	TransformedPos[2] = z;
	TransformedPos[3] = 1.0f;

	Trans.multiplyWith1x4Matrix(TransformedPos);

	if(TransformedPos[3] < 0.0f)
		return;

	irr::f32 zDiv = TransformedPos[3] == 0.0f ? 1.0f : (1.0f / TransformedPos[3]);

	ProjX = (irr::s32)(Dimensions.Width * TransformedPos[0] * zDiv) + Dimensions.Width;
	ProjY = ((irr::s32)(Dimensions.Height - (Dimensions.Height * (TransformedPos[1] * zDiv))));
	ProjZ = 0;
}

// Find the position of entity in screen space.
DLLPRE void DLLEX EntityProject(ISceneNode* Node)
{
	PointProject(Node->getAbsolutePosition().X, Node->getAbsolutePosition().Y, Node->getAbsolutePosition().Z); 
}


