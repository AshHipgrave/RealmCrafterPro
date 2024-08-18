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

#include "BlitzPlus.h"
#include <windows.h>
#include <stdlib.h>
#include "MD5.h"
#include <ArrayList.h>

namespace BlitzPlus
{
	uint LastHWND = NULL;
	#include "BlitzGad.h"


	HWND LastWindow = 0;
	HWND DCallWnd = 0;
	CWindow* DeskWnd = 0;
	DrawCallbackFN DCall = 0;
	NGin::GUI::GUIUpdateParameters NGUIUpdateParameters;
	NGin::ArrayList<int> KeysInput;

	LRESULT WINAPI MsgProc(HWND Hwnd, UINT Msg, WPARAM WParam, LPARAM LParam);
	CWindow* GetWindowFromHwnd(HWND Hwnd);
	static bool BBClassRegistered = false;
	static WNDCLASSEX BBClass;
	static CGadget* EventSrc = 0;
	#define BBCLASSNAME "BBWindow"

	extern int LMouseX, LMouseY, MXSpeed, MYSpeed, AMouseX, AMouseY;

	#ifdef _BB_USEPREFIX
	uint BBCreateWindow(std::string Title, int X, int Y, int Width, int Height, uint Group, int Style)
	#else
	uint CreateWindow(std::string Title, int X, int Y, int Width, int Height, uint Group, int Style)
	#endif
	{
		// Register a class if needed
		if(!BBClassRegistered)
		{
			WNDCLASSEX TBBClass = {sizeof(WNDCLASSEX),
				CS_CLASSDC,
				MsgProc,
				0L,
				0L,
				GetModuleHandle(NULL),
				NULL, NULL, NULL, NULL, BBCLASSNAME, NULL};
			BBClass = TBBClass;
			RegisterClassEx(&BBClass);
			BBClassRegistered = true;
		}

		// Nice mod to increase the window size to include borders!
		RECT WinSize;
		SetRect(&WinSize, 0, 0, Width, Height);
		AdjustWindowRect(&WinSize, WS_OVERLAPPEDWINDOW, false);

		// Make the window
		HWND Hwnd = CreateWindowA(BBCLASSNAME, Title.c_str(),
			WS_OVERLAPPED | WS_CAPTION,
			X, Y,
			(WinSize.right - WinSize.left), (WinSize.bottom - WinSize.top),
			GetDesktopWindow(),
			NULL,
			BBClass.hInstance,
			NULL);

		EnableMenuItem(GetSystemMenu(Hwnd, false), SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);


		ShowWindow(Hwnd, SW_SHOWDEFAULT);
		UpdateWindow(Hwnd);

		LastHWND = (uint)Hwnd;
		CWindow* Win = new CWindow();
		Win->Hwnd = Hwnd;
		LastWindow = Hwnd;

		//DInputSet = true;
		if(DInputSet == false)
		{
			DebugLog("Input...");
			InitInput();
			DebugLog("Done...");
			InitOffsets((uint)Win);
			DInputSet = true;
		}

		return (uint)Win;

	}

	//! Create a button
	#ifdef _BB_USEPREFIX
	uint BBCreateButton(std::string Text, int X, int Y, int Width, int Height, uint Group, int Style)
	#else
	uint CreateButton(std::string Text, int X, int Y, int Width, int Height, uint Group, int Style)
	#endif
	{
		if(Group == 0)
			return 0;

		CWindow* Win = (CWindow*)Group;

		int dStyle = 0;
		if(Style == 2)
			dStyle = BS_AUTOCHECKBOX;
		if(Style == 3)
			dStyle = BS_AUTORADIOBUTTON;

		HWND Button = CreateWindowA("BUTTON", Text.c_str(),
			WS_VISIBLE | WS_CHILD | dStyle,
			X, Y, Width, Height,
			Win->Hwnd,
			NULL,
			BBClass.hInstance,
			NULL);

		CButton* But = new CButton();
		But->Handle = Button;
		But->Parent = Win;

		return (uint)But;
	}


	//! Wait event/Message loop
	#ifdef _BB_USEPREFIX
	uint BBWaitEvent(int Time)
	#else
	uint WaitEvent(int Time)
	#endif
	{
		// Message
		MSG Msg;
		ZeroMemory(&Msg, sizeof(Msg));

		// Returns
		int Return = 0;

		// Timer
		int End = MilliSecs() + Time;

		UpdateInput();
		MXSpeed = 0;
		MYSpeed = 0;

		NGUIUpdateParameters.KeyLeft = false;
		NGUIUpdateParameters.KeyRight = false;
		NGUIUpdateParameters.KeyUp = false;
		NGUIUpdateParameters.KeyDown = false;
		
		NGUIUpdateParameters.KeyDelete = false;
		NGUIUpdateParameters.KeyInsert = false;
		NGUIUpdateParameters.KeyEnd = false;
		NGUIUpdateParameters.KeyHome = false;

		while(End > MilliSecs() || Time == 0)
		{
			if(PeekMessage(&Msg, NULL, 0U, 0U, PM_REMOVE))
			{
				//DebugLog(String("WinMsg: ") + String(Msg.message));

				switch(Msg.message)
				{
				case WM_CLOSE:
					{
						EventSrc = GetWindowFromHwnd(Msg.hwnd);
						Return = 0x803;

						//exit(0);
						break;
					}
				case WM_PAINT:
					{
						if(DCallWnd == Msg.hwnd)
						{
							PAINTSTRUCT Ps;
							HDC DC = BeginPaint(Msg.hwnd, &Ps);

							if(DCall != 0)
								DCall(DC);

							EndPaint(Msg.hwnd, &Ps);
						}
						break;
					}
				case WM_KEYUP:
					{

						break;
					}
				case WM_KEYDOWN:
					{
						switch(Msg.wParam)
						{
						case VK_LEFT:
							NGUIUpdateParameters.KeyLeft = true;
							break;
						case VK_RIGHT:
							NGUIUpdateParameters.KeyRight = true;
							break;
						case VK_UP:
							NGUIUpdateParameters.KeyUp = true;
							break;
						case VK_DOWN:
							NGUIUpdateParameters.KeyDown = true;
							break;
						case VK_HOME:
							NGUIUpdateParameters.KeyHome = true;
							break;
						case VK_END:
							NGUIUpdateParameters.KeyEnd = true;
							break;
						case VK_INSERT:
							NGUIUpdateParameters.KeyInsert = true;
							break;
						case VK_DELETE:
							NGUIUpdateParameters.KeyDelete = true;
							break;
						}

						break;
					}
				case WM_CHAR:
					{
						KeysInput.Add(Msg.wParam);
						break;
					}
				case WM_MOUSEMOVE:
					{
						// Store last positions
						int tLMouseX = AMouseX;
						int tLMouseY = AMouseY;

						AMouseX = LOWORD(Msg.lParam);
						AMouseY = HIWORD(Msg.lParam);

						// If its not in the bounds then reset it
						//if(AMouseX < 0 || AMouseX > Width || AMouseY < 0 || AMouseY > Height)
						//{
						//	AMouseX = tLMouseX;
						//	AMouseY = tLMouseY;
						//}

						MXSpeed = AMouseX - LMouseX;
						MYSpeed = AMouseY - LMouseY;

						LMouseX = AMouseX;
						LMouseY = AMouseY;

						break;
					}
				}

				if(Return == 0)
				{
					TranslateMessage(&Msg);
					DispatchMessage(&Msg);
				}
				
				/*if(Return > 0)
					return Return;*/
			}else if(Time == 0)
				break;
			
		}

		NGUIUpdateParameters.Handled = false;

		for(int i = 0; i < KeysInput.Size(); ++i)
			NGUIUpdateParameters.InputBuffer->push_back(KeysInput[i]);
		KeysInput.Empty();

		NGUIUpdateParameters.MousePosition = NGin::Math::Vector2(MouseX(), MouseY());
		NGUIUpdateParameters.LeftDown = MouseDown(1);
		NGUIUpdateParameters.RightDown = MouseDown(2);
		return 0;
	}

	CWindow* GetWindowFromHwnd(HWND Hwnd)
	{
		return 0;
	}

	//! Message Handler
	LRESULT WINAPI MsgProc(HWND Hwnd, UINT Msg, WPARAM WParam, LPARAM LParam)
	{
		//DebugLog(String("WinMsg: ") + String(Msg));

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
		}

		return DefWindowProc(Hwnd, Msg, WParam, LParam);
	}

	void RuntimeError(std::string Error)
	{
		MessageBoxA(NULL, Error.c_str(), "Runtime Error", MB_OK | MB_ICONERROR);
		exit(0);
	}

	void DebugLog(std::string Data)
	{
		OutputDebugString((Data + "\n").c_str());
	}

	#ifdef _BB_USEPREFIX
	void BBRegisterDrawCallback(DrawCallbackFN Fn, uint Object)
	#else
	void RegisterDrawCallback(DrawCallbackFN Fn,  uint Object)
	#endif
	{
		CGadget* Obj = (CGadget*)Object;
		
		if(Obj->GetType() != TYPE_WINDOW)
			return;
		DCallWnd = ((CWindow*)Obj)->Hwnd;

		DCall = Fn;
	}


	#ifdef _BB_USEPREFIX
	uint BBQueryObject(uint Object, int Query)
	#else
	uint QueryObject(uint Object, int Query)
	#endif
	{
		CGadget* Obj = (CGadget*)Object;
		
		if(Obj->GetType() != TYPE_WINDOW)
			return 0;

		switch(Query)
		{
		case 1:
			{
				return (uint)((CWindow*)Obj)->Hwnd;
				break;
			}
		}

		return 0;
	}

	#ifdef _BB_USEPREFIX
	void BBAppTitle(std::string Title, std::string QuitMessage)
	#else
	void AppTitle(std::string Title, std::string QuitMessage)
	#endif
	{
		if(LastWindow > 0)
		{
			SetWindowText(LastWindow, Title.c_str());
		}
	}

	#ifdef _BB_USEPREFIX
	void BBSetGadgetText(uint Gadget, std::string Text)
	#else
	void SetGadgetText(uint Gadget, std::string Text);
	#endif
	{
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				SetWindowText(((CWindow*)Gad)->Hwnd, Text.c_str());
				break;
			}
		case TYPE_BUTTON:
			{
				SetWindowText(((CButton*)Gad)->Handle, Text.c_str());
				break;
			}
		}
	}

	#ifdef _BB_USEPREFIX
	int BBGadgetX(uint Gadget)
	#else
	int GadgetX(uint Gadget)
	#endif
	{
		RECT Out;
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				GetWindowRect(((CWindow*)Gad)->Hwnd, &Out);
				return Out.left;
				break;
			}
		case TYPE_BUTTON:
			{
				GetWindowRect(((CButton*)Gad)->Handle, &Out);
				return Out.left - BBGadgetX((uint)((CButton*)Gad)->Parent);
				break;
			}
		}
		
		return 0;
	}


	#ifdef _BB_USEPREFIX
	int BBGadgetY(uint Gadget)
	#else
	int GadgetY(uint Gadget)
	#endif
	{
		RECT Out;
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				GetWindowRect(((CWindow*)Gad)->Hwnd, &Out);
				return Out.top;
				break;
			}
		case TYPE_BUTTON:
			{
				GetWindowRect(((CButton*)Gad)->Handle, &Out);
				return Out.top - BBGadgetY((uint)((CButton*)Gad)->Parent);
				break;
			}
		}
		
		return 0;
	}


	#ifdef _BB_USEPREFIX
	int BBClientWidth(uint Gadget)
	#else
	int ClientWidth(uint Gadget)
	#endif
	{
		RECT Out;
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				GetWindowRect(((CWindow*)Gad)->Hwnd, &Out);
				return Out.right - Out.left;
				break;
			}
		case TYPE_BUTTON:
			{
				GetWindowRect(((CButton*)Gad)->Handle, &Out);
				return Out.right - Out.left;
				break;
			}
		}
		
		return 0;
	}


	#ifdef _BB_USEPREFIX
	int BBClientHeight(uint Gadget)
	#else
	int ClientHeight(uint Gadget)
	#endif
	{
		RECT Out;
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				GetWindowRect(((CWindow*)Gad)->Hwnd, &Out);
				return Out.bottom - Out.top;
				break;
			}
		case TYPE_BUTTON:
			{
				GetWindowRect(((CButton*)Gad)->Handle, &Out);
				return Out.bottom - Out.top;
				break;
			}
		}
		
		return 0;
	}

	std::string Chr(int Character)
	{
		// Make two characters
		char New[2];

		// Set the ASCII number and null byte
		New[0] = (char)Character;
		New[1] = 0;

		// Make a RealString
		return std::string(New, 1);
	}

	unsigned char Asc(std::string Word)
	{
		// Check length letter
		if(Word.length() > 0)
			return Word.c_str()[0];
		return 0;
	}

	float Rnd(float From, float To)
	{
		float Tiny = ((float)rand()) / ((float)RAND_MAX);
		Tiny *= (To - From);
		Tiny += From;
		return Tiny;
	}

	int Rand(int From, int To)
	{
		return (int)Rnd(From, To);
	}

	void SeedRnd(int Seed)
	{
		srand(Seed);
	}

	std::string BBString(std::string Str, int Repeat)
	{
		// make new string
		std::string Out = "";
		
		// Append
		for(int i = 0; i < Repeat; ++i)
			Out.append(Str);

		// Done
		return Out;
	}

	std::string MD5(std::string Data)
	{
		// Setup state and output
		md5_state_t State;
		md5_byte_t BytesOut[16];

		// Actually create the hash
		md5_init(&State);
		md5_append(&State, (const md5_byte_t*)Data.c_str(), Data.length());
		md5_finish(&State, BytesOut);

		// Create a string for output
		char OutCStr[33];

		// Convert the output to a hex string. sprintf *is* old, but its useful
		sprintf(OutCStr, "%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x",
			BytesOut[0], BytesOut[1], BytesOut[2], BytesOut[3],
			BytesOut[4], BytesOut[5], BytesOut[6], BytesOut[7],
			BytesOut[8], BytesOut[9], BytesOut[10], BytesOut[11],
			BytesOut[12], BytesOut[13], BytesOut[14], BytesOut[15]);

		// Return realstring
		return std::string(OutCStr);
	}

	#ifdef _BB_USEPREFIX
	uint BBDesktop()
	#else
	uint Desktop()
	#endif
	{
		if(DeskWnd == 0)
		{
			DeskWnd = new CWindow();
			DeskWnd->Hwnd = GetDesktopWindow();
		}

		return (uint)DeskWnd;
	}

	#ifdef _BB_USEPREFIX
	void BBSetGadgetShape(uint Gadget, int X, int Y, int Width, int Height)
	#else
	void SetGadgetShape(uint Gadget, int X, int Y, int Width, int Height)
	#endif
	{
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				SetWindowPos(((CWindow*)Gad)->Hwnd, 0, X, Y, Width, Height, 0);
				break;
			}
		case TYPE_BUTTON:
			{
				SetWindowPos(((CButton*)Gad)->Handle, 0, X, Y, Width, Height, 0);
				break;
			}
		}
	}

	void HidePointer()
	{
		ShowCursor(false);
		//SetCursor(NULL);
	}

	void ShowPointer()
	{
		ShowCursor(true);
	}

	#ifdef _BB_USEPREFIX
	void BBHideGadget(uint Gadget)
	#else
	void HideGadget(uint Gadget)
	#endif
	{
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				ShowWindow(((CWindow*)Gad)->Hwnd, SW_HIDE);
				break;
			}
		case TYPE_BUTTON:
			{
				ShowWindow(((CButton*)Gad)->Handle, SW_HIDE);
				break;
			}
		}
	}

	#ifdef _BB_USEPREFIX
	void BBShowGadget(uint Gadget)
	#else
	void ShowGadget(uint Gadget)
	#endif
	{
		CGadget* Gad = (CGadget*)Gadget;

		switch(Gad->GetType())
		{
		case TYPE_WINDOW:
			{
				ShowWindow(((CWindow*)Gad)->Hwnd, SW_SHOW);
				break;
			}
		case TYPE_BUTTON:
			{
				ShowWindow(((CButton*)Gad)->Handle, SW_SHOW);
				break;
			}
		}
	}

	void MoveMouse(int X, int Y)
	{
		SetCursorPos(X, Y);
	}
}