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
#include <windows.h>
#include <BlitzPlus.h>

struct SSplashScreen
{
	WNDCLASSEX wndClass;
	HWND hwnd;
	HBITMAP bitmap;
	std::string statusText;

} *SplashScreen = NULL;

//! Message Handler
LRESULT WINAPI MsgProc(HWND Hwnd, UINT Msg, WPARAM WParam, LPARAM LParam)
{
	switch(Msg)
	{
	case WM_CLOSE:
		{
			return 0;
			break;
		}
	case WM_DESTROY:
		{
			return 0;
			break;
		}
	case WM_SIZE:
		{
			return 0;
			break;
		}
	case WM_PAINT:
		{
			// Objects needed for drawing
			HFONT font = CreateFont(14, 0, 0, 0, FW_DONTCARE, FALSE, FALSE, FALSE, DEFAULT_CHARSET, OUT_OUTLINE_PRECIS,
				CLIP_DEFAULT_PRECIS, CLEARTYPE_QUALITY, VARIABLE_PITCH, TEXT("Microsoft Sans Serif"));

			BITMAP bitmap;
			GetObject( SplashScreen->bitmap, sizeof(bitmap), &bitmap );

			// Start painting
			PAINTSTRUCT Ps;
			HDC dc = BeginPaint(Hwnd, &Ps);

			// Draw background
			HDC dcMem = CreateCompatibleDC(dc);

			HGDIOBJ objPrev = SelectObject( dcMem, SplashScreen->bitmap);

			BitBlt( dc, 0, 0, bitmap.bmWidth, bitmap.bmHeight, dcMem, 0, 0, SRCCOPY );

			SelectObject( dcMem, objPrev );
			DeleteDC( dcMem );

			// Draw status
			SelectObject( dc, font );
			SetTextColor( dc, RGB(255, 255, 255) );
			SetBkMode( dc, TRANSPARENT );

			RECT rect;
			rect.left = bitmap.bmWidth - 165;
			rect.top = bitmap.bmHeight - 35;
			rect.right = rect.left + 165;
			rect.bottom = rect.top + 35;
			DrawText( dc, SplashScreen->statusText.c_str(), SplashScreen->statusText.length(), &rect, DT_TOP | DT_LEFT | DT_NOCLIP );
			//TextOut( dc, bitmap.bmWidth - 165, bitmap.bmHeight - 35, SplashScreen->statusText.c_str(), SplashScreen->statusText.length() );

			// Clean up
			EndPaint(Hwnd, &Ps);
			DeleteObject( font );

			return 0;
		}
	}

	return DefWindowProc(Hwnd, Msg, WParam, LParam);
}

void CreateAndShowSplash(const char* name)
{
	// Data for splash
	SplashScreen = new SSplashScreen();

	// Load bitmap
	SplashScreen->bitmap = (HBITMAP)LoadImage( NULL, "Data\\Splash.bmp", IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE );
	if( !SplashScreen->bitmap )
	{
		MessageBoxA(NULL, "Failed to load Data\\Splash.bmp", "", MB_OK);

		delete SplashScreen;
		SplashScreen = NULL;
		return;
	}

	// Create class
	WNDCLASSEX wndClass = { sizeof(WNDCLASSEX),
		CS_CLASSDC,
		MsgProc,
		0L,
		0L,
		GetModuleHandle(NULL),
		NULL, NULL, NULL, NULL, "SPLASHSCREEN", NULL };
	SplashScreen->wndClass = wndClass;
	RegisterClassEx(&(SplashScreen->wndClass));

	// Create window
	BITMAP bitmap;
	GetObject( SplashScreen->bitmap, sizeof(bitmap), &bitmap );

	int screenWidth = BBClientWidth(BBDesktop()) / 2;
	int screenHeight = BBClientHeight(BBDesktop()) / 2;

	SplashScreen->hwnd = CreateWindowA("SPLASHSCREEN", name,
		WS_POPUP,
		screenWidth - bitmap.bmWidth / 2, screenHeight - bitmap.bmHeight / 2,
		bitmap.bmWidth, bitmap.bmHeight,
		GetDesktopWindow(),
		NULL,
		SplashScreen->wndClass.hInstance,
		NULL);

	ShowWindow(SplashScreen->hwnd, SW_SHOWDEFAULT);
	UpdateWindow(SplashScreen->hwnd);

	InvalidateRect( SplashScreen->hwnd, NULL, TRUE );
}

void CloseSplash()
{
	if( !SplashScreen )
		return;

	DeleteObject( SplashScreen->bitmap );
	DestroyWindow( SplashScreen->hwnd );
	UnregisterClass( "SPLASHSCREEN", SplashScreen->wndClass.hInstance );

	delete SplashScreen;
	SplashScreen = NULL;
}

void UpdateSplashStatus( const char* status )
{
	if( !SplashScreen )
		return;

	SplashScreen->statusText = status;
	InvalidateRect( SplashScreen->hwnd, NULL, TRUE );
}