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

const char* OverlayEffect = 
	"// Constants\r\n"\
	"float4 Color;\r\n"\
	"\r\n"\
	"// Vertex Shader Input\r\n"\
	"struct VSIn\r\n"\
	"{\r\n"\
	"	float4 Position : POSITION;\r\n"\
	"};\r\n"\
	"\r\n"\
	"// Vertex Shader Output\r\n"\
	"struct VSOut\r\n"\
	"{\r\n"\
	"	float4 Position : POSITION;\r\n"\
	"};\r\n"\
	"\r\n"\
	"\r\n"\
	"// Vertex entrypoint\r\n"\
	"VSOut vs_main(VSIn In)\r\n"\
	"{\r\n"\
	"	VSOut Out;\r\n"\
	"	Out.Position = In.Position;\r\n"\
	"\r\n"\
	"	return Out;\r\n"\
	"}\r\n"\
	"\r\n"\
	"// Pixel Entrypoint\r\n"\
	"float4 ps_main( VSOut In ) : COLOR0\r\n"\
	"{\r\n"\
	"	return Color;\r\n"\
	"}\r\n"\
	"\r\n"\
	"technique Debug\r\n"\
	"{\r\n"\
	"	pass p0 \r\n"\
	"	{		\r\n"\
	"		VertexShader = compile vs_2_0 vs_main();\r\n"\
	"		PixelShader  = compile ps_2_0 ps_main();\r\n"\
	"		alphablendenable = true;\r\n"\
	"		srcblend = srcalpha;\r\n"\
	"		destblend = invsrcalpha;\r\n"\
	"	}\r\n"\
	"}";
