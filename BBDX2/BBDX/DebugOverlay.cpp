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
#include "DebugOverlay.h"
#include "OverlayEffect.h"
#include <d3dx9.h>
#include <string>
#include "DebugFrameProfiler.h"

#define BORDER_SIZE 20.0f
#define STR_APPLICATION_NAME "App: BBDX2"

const char* ViewModeStrings[] =
	{
		"Frame Profiler",
		"Job Profiler"
	};
int NumViewModeStrings = 2;

void DO_DrawRect( float x, float y, float width, float height, D3DXVECTOR4 &color );
void DO_DrawLine( float startX, float startY, float endX, float endY, D3DXVECTOR4 &color );
void DO_DrawRectLine( float x, float y, float width, float height, D3DXVECTOR4 &color );
void DO_DrawText( float x, float y, std::string &text, unsigned int color );
void DO_RectBegin();
void DO_RectEnd();
void DO_DrawFrameProfiler();
void DO_UpdateFrameProfiler();

namespace
{
	enum ViewMode
	{
		VM_FRAMEPROFILE,
		VM_JOBPROFILE
	};

	bool OverlayOpen = false;
	bool DrawFPOffset = false;

	ViewMode CurrentMode = VM_FRAMEPROFILE;
	int ScrollLine = 0;
	

	float ScreenWidth = 0;
	float ScreenHeight = 0;

	IDirect3DDevice9* Device = NULL;
	ID3DXFont* Font = NULL;
	ID3DXEffect* Effect;

	struct DOVertex
	{
		D3DXVECTOR3 Position;
	};

	const D3DVERTEXELEMENT9 QuadDecl[] = {
		{ 0,	0,	D3DDECLTYPE_FLOAT3,	NULL,	D3DDECLUSAGE_POSITION,	0},
		D3DDECL_END() };
	IDirect3DVertexDeclaration9* VertexDeclaration = NULL;

	inline bool KeyDown(int Key)
	{
		short K = GetKeyState(Key);
		return ((K >> 16) & 0xffff) != 0;
	}
}

void DO_Initialize(void* device, int width, int height)
{
	FP_Init();

	Device = reinterpret_cast<IDirect3DDevice9*>(device);
	ScreenWidth = (float)width;
	ScreenHeight = (float)height;
	
	if(FAILED(D3DXCreateFontA(Device, 16, 0, 0, 1, FALSE, 0, 0, 0, 0, "FixedSys", &Font)))
	{
		OutputDebugStringA("Could not load font: Courier New\n");
		return;
	}

	if(FAILED(Device->CreateVertexDeclaration(QuadDecl, &VertexDeclaration)))
	{
		OutputDebugStringA("Could not create vertex declaration.\n");
		return;
	}

	if(FAILED(D3DXCreateEffect( Device, OverlayEffect, strlen(OverlayEffect), NULL, NULL, 0, NULL, &Effect, NULL )))
	{
		OutputDebugStringA("Could not create effect.\n");
		return;
	}
}

void DO_LostDevice()
{
	Font->OnLostDevice();
	Effect->OnLostDevice();
}

void DO_ResetDevice()
{
	Font->OnResetDevice();
	Effect->OnResetDevice();
}

void DO_Render()
{
	if( !OverlayOpen )
	{
		return;
	}

	DO_RectBegin();

	DO_DrawRect( BORDER_SIZE, BORDER_SIZE, ScreenWidth - 2.0f * BORDER_SIZE, ScreenHeight - 2.0f * BORDER_SIZE, D3DXVECTOR4( 0.0f, 0.0f, 0.0f, 0.5f ) );
	DO_DrawRectLine( BORDER_SIZE, BORDER_SIZE, ScreenWidth - 2.0f * BORDER_SIZE, ScreenHeight - 2.0f * BORDER_SIZE, D3DXVECTOR4( 1.0f, 0.0f, 0.0f, 1.0f ) );

	DO_RectEnd();

	// Draw main text
	DO_DrawText( BORDER_SIZE + 5, BORDER_SIZE + 5, std::string(STR_APPLICATION_NAME), 0xffffffff );
	DO_DrawText( BORDER_SIZE + 200, BORDER_SIZE + 5, std::string("View Mode: ") + ViewModeStrings[CurrentMode] + " [F3]", 0xffffffff );
	DO_DrawText( ScreenWidth - BORDER_SIZE - 100, BORDER_SIZE + 5, std::string("[Hide F2]"), 0xffffffff );

	// Draw header for frame profiler
	if( CurrentMode == VM_FRAMEPROFILE )
	{
		DO_DrawFrameProfiler();
	}
}

void DO_Update()
{
	if( !OverlayOpen )
	{
		// Allow open
		if( KeyDown( VK_CONTROL ) && KeyDown( VK_SHIFT ) && KeyDown( VK_F2 ) )
		{
			OverlayOpen = true;
		}

		return;
	}

	// Hide overlay
	if( KeyDown( VK_F2 ) && !KeyDown( VK_CONTROL ) && !KeyDown( VK_SHIFT ) )
	{
		OverlayOpen = false;
		return;
	}

	// Change view mode
	if( KeyDown( VK_F3 ) )
	{
		CurrentMode = (ViewMode)((int)CurrentMode + 1);
		if( CurrentMode >= NumViewModeStrings )
			CurrentMode = VM_FRAMEPROFILE;
		Sleep( 100 );
	}

	// Draw header for frame profiler
	if( CurrentMode == VM_FRAMEPROFILE )
	{
		DO_UpdateFrameProfiler();
	}
}

void DO_DrawFPGraphText( FP_Task* task, int nest, float &y, int &currentLine )
{
	if( y > (ScreenHeight - BORDER_SIZE - 48.0f))
		return;

	if( currentLine++ >= ScrollLine )
	{
		DO_DrawText( BORDER_SIZE + 20 + (nest * 20), y - 5, std::string(task->taskName), 0xffffffff );

		char globalTime[128];

		sprintf(globalTime, "%.02f", task->timeGlobal);

		if( task->numChildren > 0 )
			DO_DrawText( BORDER_SIZE + 760, y - 5, std::string(globalTime), 0xff80ff80 );
		else
			DO_DrawText( BORDER_SIZE + 760, y - 5, std::string(globalTime), 0xffffffff );

		y += 22.5f;
	}

	for( int i = 0; i < task->numChildren; ++i )
	{
		DO_DrawFPGraphText( task->children[i], nest + 1, y, currentLine);
	}
}

void DO_DrawFPGraphBG( FP_Task* task, float globalOff, float groupOff, float &y, int &currentLine )
{
	if( y > (ScreenHeight - BORDER_SIZE - 48.0f))
		return;

	if( currentLine++ >= ScrollLine )
	{
		if( currentLine % 2 == 0 )
			DO_DrawRect( BORDER_SIZE + 10, y - 3, 850 - BORDER_SIZE, 24.0f, D3DXVECTOR4(0.2f, 0.2f, 0.2f, 0.2f) );
		else
			DO_DrawRect( BORDER_SIZE + 10, y - 3, 850 - BORDER_SIZE, 24.0f, D3DXVECTOR4(0.4f, 0.4f, 0.4f, 0.2f) );

		globalOff += (task->percGlobal * 400.0f);
		groupOff += (task->percGroup * 400.0f);

		y += 24.0f;
	}

	groupOff = 0.0f;

	for( int i = 0; i < task->numChildren; ++i )
	{
		DO_DrawFPGraphBG( task->children[i], 0, groupOff, y, currentLine );

		if( DrawFPOffset )
			groupOff += task->children[i]->percGroup * 400.0f;
	}
}

void DO_DrawFPGraph( FP_Task* task, float globalOff, float groupOff, float &y, int &currentLine)
{
	if( y > (ScreenHeight - BORDER_SIZE - 48.0f))
		return;

	if( currentLine++ >= ScrollLine )
	{
		DO_DrawRect( BORDER_SIZE + 320 + globalOff, y, task->percGlobal * 250.0f, 7.0f, D3DXVECTOR4(0.5f, 0, 0, 1) );
		DO_DrawRectLine( BORDER_SIZE + 320 + globalOff, y, task->percGlobal * 250.0f, 7.0f, D3DXVECTOR4(1.0f, 0, 0, 1) );

		DO_DrawRect( BORDER_SIZE + 320 + groupOff, y + 7.0f, task->percGroup * 250.0f, 7.0f, D3DXVECTOR4(0.5f, 0.5f, 0, 1) );
		DO_DrawRectLine( BORDER_SIZE + 320 + groupOff, y + 7.0f, task->percGroup * 250.0f, 7.0f, D3DXVECTOR4(1.0f, 1.0f, 0, 1) );

		globalOff += (task->percGlobal * 400.0f);
		groupOff += (task->percGroup * 400.0f);

		y += 24.0f;
	}

	groupOff = 0.0f;

	for( int i = 0; i < task->numChildren; ++i )
	{
		DO_DrawFPGraph( task->children[i], 0, groupOff, y, currentLine );

		if( DrawFPOffset )
			groupOff += task->children[i]->percGroup * 400.0f;
	}
}

void DO_UpdateFrameProfiler()
{
	if( !FP_IsRunning() )
	{
		// Update Keys
		if( KeyDown( VK_F5 ) )
		{
			FP_BeginCapture();
		}

		if( KeyDown( VK_F6 ) )
		{
			// Save capture
		}

		if( KeyDown( VK_F7 ) )
		{
			int t = FP_GetCaptureFrames() - 1;
			if( t < 1 )
				t = 1;

			FP_SetCaptureFrames( t );
			Sleep( 50 );
		}

		if( KeyDown( VK_F8 ) )
		{
			int t = FP_GetCaptureFrames() + 1;

			FP_SetCaptureFrames( t );
			Sleep( 50 );
		}

		if( KeyDown( VK_F9 ) )
		{
			ScrollLine--;
			if(ScrollLine < 0)
				ScrollLine = 0;

			Sleep( 10 );
		}

		if( KeyDown( VK_F10 ) )
		{
			ScrollLine++;

			Sleep( 10 );
		}
	}
}

void DO_DrawFrameProfiler()
{
	unsigned int textColor = 0xffffffff;

	if( FP_IsRunning() )
		textColor = 0xff808080;

	int captureFrames = FP_GetCaptureFrames();
	int currentFrames = FP_GetFramesCaptured();

	char captureFramesStr[256];
	char currentFramesStr[256];

	sprintf( captureFramesStr, "Cap %i - [F7] + [F8]", captureFrames );
	sprintf( currentFramesStr, "Got %i", currentFrames );

	DO_DrawText( BORDER_SIZE + 5, BORDER_SIZE + 30, std::string("Capture [F5]"), textColor );
	DO_DrawText( BORDER_SIZE + 200, BORDER_SIZE + 30, std::string("Save Capture [F6]"), 0xff808080 );
	DO_DrawText( BORDER_SIZE + 400, BORDER_SIZE + 30, std::string(captureFramesStr), textColor );
	DO_DrawText( BORDER_SIZE + 600, BORDER_SIZE + 30, std::string(currentFramesStr), textColor );
	DO_DrawText( BORDER_SIZE + 850, BORDER_SIZE + 60, std::string("- / + [F9-F10]"), textColor );

	if( !FP_IsRunning() )
	{
		FP_Task* Graph = FP_GetGraph();
		int nest = 0;
		float y = BORDER_SIZE + 100;

		int currentLine = 0;

		if( Graph != NULL )
		{
			float ty = y;
			DO_RectBegin();
			DO_DrawFPGraphBG( Graph, 0.0f, 0.0f, y, currentLine );
			DO_RectEnd();

			currentLine = 0;
			y = ty;
			DO_DrawFPGraphText( Graph, nest, ty, currentLine );

			currentLine = 0;
			y = BORDER_SIZE + 100;
			DO_RectBegin();
			DO_DrawFPGraph( Graph, 0.0f, 0.0f, y, currentLine );
			DO_RectEnd();
		}

	}
}


void DO_DrawRect( float x, float y, float width, float height, D3DXVECTOR4 &color )
{
	float x2 = x + width;
	float y2 = y + height;

	x /= ScreenWidth;
	x2 /= ScreenWidth;
	y /= ScreenHeight;
	y2 /= ScreenHeight;

	x *= 2.0f;
	x2 *= 2.0f;
	y *= 2.0f;
	y2 *= 2.0f;

	x -= 1.0f;
	x2 -= 1.0f;
	y -= 1.0f;
	y2 -= 1.0f;

	y = -y;
	y2 = -y2;

	DOVertex vertices[4];
	memset( vertices, 0, sizeof(vertices) );

	vertices[0].Position.x = x;
	vertices[1].Position.x = x2;
	vertices[2].Position.x = x;
	vertices[3].Position.x = x2;

	vertices[0].Position.y = y;
	vertices[1].Position.y = y;
	vertices[2].Position.y = y2;
	vertices[3].Position.y = y2;

	Effect->SetVector( "Color", &color );
	Effect->CommitChanges();

	
	Device->DrawPrimitiveUP( D3DPT_TRIANGLESTRIP, 2, vertices, sizeof(DOVertex) );
}

void DO_DrawLine( float startX, float startY, float endX, float endY, D3DXVECTOR4 &color )
{
	DO_DrawRect( startX, startY, (endX - startX) + 1.0f, (endY - startY) + 1.0f, color );
}

void DO_DrawRectLine( float x, float y, float width, float height, D3DXVECTOR4 &color )
{
	DO_DrawLine( x, y, x + width, y, color );
	DO_DrawLine( x, y, x, y + height, color );
	DO_DrawLine( x + width, y, x + width, y + height, color );
	DO_DrawLine( x, y + height, x + width, y + height, color );
}

void DO_DrawText( float x, float y, std::string &text, unsigned int color )
{
	RECT rect;
	rect.left = (LONG)x;
	rect.top = (LONG)y;
	rect.right = rect.left + 1280;
	rect.bottom = rect.top + 720;

	Font->DrawTextA( NULL, text.c_str(), text.length(), &rect, DT_LEFT, color );
}

void DO_RectBegin()
{
	if( Effect != NULL )
	{
		UINT passes = 0;
		Effect->Begin( &passes, NULL );
		Effect->BeginPass( 0 );

		Device->SetVertexDeclaration( VertexDeclaration );
	}
}

void DO_RectEnd()
{
	if( Effect != NULL )
	{
		Effect->EndPass();
		Effect->End();
	}
}

