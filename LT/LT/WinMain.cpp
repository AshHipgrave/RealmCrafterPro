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
#include "WinMain.h"
#include <IGUIManager.h>
using namespace NGin;

bool LMBDown;
bool RMBDown;
bool LeftKey, RightKey, UpKey, DownKey, Home, End, Insert, Delete;
ArrayList<int> InputBuffer;

LRESULT WINAPI MsgProc(HWND Hwnd, UINT Msg, WPARAM WParam, LPARAM LParam)
{
	switch(Msg)
	{
	case WM_CLOSE:
		{
			PostQuitMessage(0);
			return 0;
		}
	case WM_PAINT:
		{
			PAINTSTRUCT Ps;
			HDC DC = BeginPaint(Hwnd, &Ps);

			OnWindowPaint(Hwnd, DC);

			EndPaint(Hwnd, &Ps);
			return 0;
		}
	case WM_COMMAND:
		{
			OnWindowCommand((HWND)LParam);
			return 0;
		}
	case WM_CHAR:
		{
			InputBuffer.Add(WParam);
			return 0;
		}
	case WM_KEYDOWN:
		{
			switch(WParam)
			{
			case VK_LEFT:
				LeftKey = true;
				return 0;
				break;
			case VK_RIGHT:
				RightKey = true;
				return 0;
				break;
			case VK_UP:
				UpKey = true;
				return 0;
				break;
			case VK_DOWN:
				DownKey = true;
				return 0;
				break;
			case VK_HOME:
				Home = true;
				return 0;
				break;
			case VK_END:
				End = true;
				return 0;
				break;
			case VK_INSERT:
				Insert = true;
				return 0;
				break;
			case VK_DELETE:
				Delete = true;
				return 0;
				break;
			}
		}
	case WM_LBUTTONDOWN:
		{
			LMBDown = true;
			return 0;
		}
	case WM_LBUTTONUP:
		{
			LMBDown = false;
			return 0;
		}
	}

	return DefWindowProc(Hwnd, Msg, WParam, LParam);
}

HWND Hwnd;
int WINAPI WinMain(HINSTANCE HInstance, HINSTANCE, LPSTR, int)
{
	// Register window class
	WNDCLASSEX WinClass = {sizeof(WNDCLASSEX),
		CS_CLASSDC,
		MsgProc, 0L, 0L,
		GetModuleHandle(NULL), NULL, NULL, NULL, NULL,
		"DefWinApp", NULL};
	RegisterClassEx(&WinClass);

	// Create the window
	Hwnd = CreateWindowA("DefWinApp",
		"Barebones Application",
		WS_OVERLAPPEDWINDOW,
		100, 100,
		800, 600,
		GetDesktopWindow(),
		NULL,
		WinClass.hInstance,
		NULL);

	// Setup device
	if(OnDeviceSetup(Hwnd))
	{
		// Show window
		ShowWindow(Hwnd, SW_SHOWDEFAULT);
		UpdateWindow(Hwnd);

		// Enter message loop
		MSG Msg;
		ZeroMemory(&Msg, sizeof(Msg));
		while(Msg.message != WM_QUIT)
		{
			if(PeekMessage(&Msg, NULL, 0U, 0U, PM_REMOVE))
			{
				TranslateMessage(&Msg);
				DispatchMessage(&Msg);
			}else
				OnDeviceUpdate();
		}
	}else
		MessageBox(Hwnd, "Device creation error!", "Error", MB_ICONERROR | MB_OK);

	// Clean up
	OnDeviceDestroy(Hwnd);
	UnregisterClass("DefWinApp", WinClass.hInstance);
	return 0;
}