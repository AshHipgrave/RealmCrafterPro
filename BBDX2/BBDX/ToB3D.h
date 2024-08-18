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
#pragma once
//#include <string>
#include <irrlicht.h>


bool Convert3DSToB3D_Custom( irr::core::stringc _3DSFile, irr::core::stringc B3DFile );
bool ConvertXtoB3D_Custom( irr::core::stringc XFileName, irr::core::stringc B3DFileName, int OverrideFPS = 0 );

irr::core::stringc Convert3DStoB3D(irr::core::stringc _3DSFile, bool bAutoGenerateNormals = true);
irr::core::stringc ConvertXtoB3D(irr::core::stringc XFileName, int OverrideFPS = 0);

void XtoB3D_SetDX( PDIRECT3D9  pD3D, LPDIRECT3DDEVICE9 pD3DDevice );

