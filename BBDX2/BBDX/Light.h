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
//* BBDX - Lights
//* 

// Make a point light
DLLPRE irr::s32 DLLEX copliy()
{
	irr::s32 Light = smgr->addPointLight();
	smgr->setLightPosition(Light, vector3df(0,0,0));
	smgr->setLightRadius(Light, 200.0f);
	smgr->setPLightActive(Light, true);
	smgr->setPLightColor(Light, irr::video::SColorf(1.0f, 1.0f, 1.0f));
	return Light;
}

// Make a directional light
DLLPRE irr::s32 DLLEX boompx()
{
	irr::s32 Light = smgr->addDirectionalLight();
	smgr->setLightDirection(Light, irr::core::vector3df(0, 0, 1));
	smgr->setDLightColor(Light, irr::video::SColorf(1, 1, 1));
	smgr->setDLightActive(Light, true);
	return Light;
}

// Set point light position
DLLPRE void DLLEX lovepixels(irr::s32 Light, irr::f32 x, irr::f32 y, irr::f32 z)
{
	smgr->setLightPosition(Light, irr::core::vector3df(x, y, z));
}

// Set Directional light direction
DLLPRE void DLLEX sukusul(irr::s32 Light, irr::f32 x, irr::f32 y, irr::f32 z)
{
	smgr->setLightDirection(Light, irr::core::vector3df(x, y, z));
}

// Set point radius
DLLPRE void DLLEX gabbama(irr::s32 Light,irr::f32 Radius)
{
	smgr->setLightRadius(Light, Radius);
}

// Set point light colour
DLLPRE void DLLEX hoklig(irr::s32 Light, irr::s32 ir, irr::s32 ig, irr::s32 ib)
{
	irr::f32 r = (irr::f32)ir;
	irr::f32 g = (irr::f32)ig;
	irr::f32 b = (irr::f32)ib;
	smgr->setPLightColor(Light, irr::video::SColorf((r/255),(g/255),(b/255)));
}

// Set Directional colour
DLLPRE void DLLEX gonerum(irr::s32 Light, irr::s32 ir, irr::s32 ig, irr::s32 ib)
{
	irr::f32 r = (irr::f32)ir;
	irr::f32 g = (irr::f32)ig;
	irr::f32 b = (irr::f32)ib;
	smgr->setDLightColor(Light, irr::video::SColorf((r/255),(g/255),(b/255)));
}

// Set Point light actibe
DLLPRE void DLLEX wareflog(irr::s32 Light, int Active)
{
	if(Active == 1)
		smgr->setPLightActive(Light, true);
	else
		smgr->setPLightActive(Light, false);
}

// Set directional light active
DLLPRE void DLLEX klopil(irr::s32 Light, int Active)
{
	if(Active == 1)
		smgr->setDLightActive(Light, true);
	else
		smgr->setDLightActive(Light, false);
}

// Free a point light
DLLPRE void DLLEX lipphogg(irr::s32 Light)
{
	smgr->FreePLight(Light);
}

// Free a directional light
DLLPRE void DLLEX jewnjig(irr::s32 Light)
{
	smgr->FreeDLight(Light);
}

// Set distance from the camera that shows will reach
DLLPRE void DLLEX ShadowDistance(irr::f32 Distance)
{
	smgr->setShadowDistance(Distance);
}

// Set what entities will be shadowed
DLLPRE void DLLEX ShadowLevel(irr::s32 Level)
{
	smgr->setShadowLevel(Level);
}

DLLPRE void DLLEX EntityShadowLevel(ISceneNode* node, irr::s32 level)
{
	if(node != 0)
		node->setShadowLevel(level);
}

DLLPRE irr::s32 DLLEX GetEntityShadowLevel(ISceneNode* node)
{
	if(node != 0)
		return node->getShadowLevel();
	return 0;
}

DLLPRE void DLLEX EntityShadowShader(ISceneNode* node, irr::u32 shader)
{
	if(node != 0)
		node->setShadowShader(shader);
}

DLLPRE irr::u32 DLLEX GetEntityShadowShader(ISceneNode* node)
{
	if(node != 0)
		return node->getShadowShader();
	return 0;
}

DLLPRE void DLLEX SetShadowMapSize(int newSize)
{
	driver->setShadowMapSize(newSize);
}

DLLPRE int DLLEX GetShadowMapSize()
{
	return driver->getShadowMapSize();
}

DLLPRE const float* DLLEX GetLightMatrix()
{
	return smgr->getLightMatrix();
}

DLLPRE void* DLLEX GetShadowMap()
{
	return ((video::CD3D9Driver*)driver)->GetShadowTexture(false);
}

// Set the shader for shadows
DLLPRE void DLLEX ShadowShader(irr::u32 Shader)
{
	smgr->setShadowShader(Shader);
}

DLLPRE void DLLEX ShadowBlurShader(irr::u32 blurh, irr::u32 blurv)
{
	smgr->setBlurShadowShader(0, blurh);
	smgr->setBlurShadowShader(1, blurv);
}

// Set how far the shadowing light will be
DLLPRE void DLLEX LightDistance(irr::f32 Distance)
{
	smgr->setLightDistance(Distance);
}

