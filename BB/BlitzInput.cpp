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
#include "BlitzPlus.h"
#include <dinput.h>
#include <dinputd.h>
#include <xinput.h>
#include "ASCIIMap.h"

#pragma comment(lib, "dxguid.lib")
#pragma comment(lib, "dxerr.lib")
#pragma comment(lib, "dinput8.lib")
#pragma comment(lib, "xinput.lib")

namespace BlitzPlus
{

	LPDIRECTINPUT8 DIObject;
	LPDIRECTINPUTDEVICE8 DIKeyboard;
	LPDIRECTINPUTDEVICE8 DIMouse;
	LPDIRECTINPUTDEVICE8 DIJoy; // Not the happy kind...
	DIMOUSESTATE2 MouseState;
	DIJOYSTATE JoyState;
	char KeyBuffer[256];
	char KeysHit[256];
	bool MouseHits[8];
	bool JoyHits[32];
	int AMouseX = 0, AMouseY = 0, MXSpeed = 0, MYSpeed = 0;
	int LMouseX = 0, LMouseY = 0;
	float AJoyX, AJoyY, AJoyZ, AJoyYaw, AJoyPitch, AJoyRoll, AJoyU, AJoyV;
	bool XInputJoy = false;
	HWND ActiveWND;
	bool UsingJoy = false;
	int XOffset = 0, YOffset = 0;

	#define SAFE_DELETE(p)  { if(p) { delete (p);     (p)=NULL; } }
	#define SAFE_RELEASE(p) { if(p) { (p)->Release(); (p)=NULL; } }
	struct DI_ENUM_CONTEXT
	{
		DIJOYCONFIG* pPreferredJoyCfg;
		bool bPreferredJoyCfgValid;
	};


	int CALLBACK EnumJoysticksCallback(const DIDEVICEINSTANCE* DIDInstance, void* Context)
	{
		DI_ENUM_CONTEXT* EnumContext = (DI_ENUM_CONTEXT*)Context;

		if(EnumContext->bPreferredJoyCfgValid && !IsEqualGUID(DIDInstance->guidInstance, EnumContext->pPreferredJoyCfg->guidInstance))
			return 1;

		if(FAILED(DIObject->CreateDevice(DIDInstance->guidInstance, &DIJoy, NULL)))
			return 1;
		return 0;
	}

	void InitInput()
	{
		ActiveWND = (HWND)LastHWND;



		if(FAILED(DirectInput8Create(GetModuleHandle(NULL),
			DIRECTINPUT_VERSION,
			IID_IDirectInput8,
			(void**)&DIObject,
			NULL)))
		{
			MessageBoxA(NULL, "Input setup failed!", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		if(FAILED(DIObject->CreateDevice(GUID_SysKeyboard, &DIKeyboard, NULL)))
		{
			MessageBoxA(NULL, "Could not create keyboard capture device!", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		if(FAILED(DIKeyboard->SetDataFormat(&c_dfDIKeyboard)))
		{
			MessageBoxA(NULL, "Could not setup keyboard input", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		if(FAILED(DIKeyboard->SetCooperativeLevel((HWND)LastHWND, DISCL_FOREGROUND | DISCL_NONEXCLUSIVE)))
		{
			MessageBoxA(NULL, "Could not set keyboard level", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		//if(FAILED(DIKeyboard->Acquire()))
		//{
		//	MessageBoxA(NULL, "Could not aquire keyboard device", "Runtime Error", MB_OK | MB_ICONERROR);
		//	exit(0);
		//}

		DIDEVCAPS MouseCaps;

		if(FAILED(DIObject->CreateDevice(GUID_SysMouse, &DIMouse, NULL)))
		{
			MessageBoxA(NULL, "Could not create mouse!", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		if(FAILED(DIMouse->SetDataFormat(&c_dfDIMouse2)))
		{
			MessageBoxA(NULL, "Mouse formatting failed!", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		if(FAILED(DIMouse->SetCooperativeLevel((HWND)LastHWND, DISCL_FOREGROUND | DISCL_NONEXCLUSIVE)))
		{
			MessageBoxA(NULL, "Could not set mouse level", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		//if(FAILED(DIMouse->Acquire()))
		//{
		//	MessageBoxA(NULL, "Could not acquire mouse!", "Runtime Error", MB_OK | MB_ICONERROR);
		//	exit(0);
		//}

		MouseCaps.dwSize = sizeof(MouseCaps);
		DIMouse->GetCapabilities(&MouseCaps);

		if(!(MouseCaps.dwFlags & DIDC_ATTACHED))
		{
			MessageBoxA(NULL, "No Mouse is attached!", "Runtime Error", MB_OK | MB_ICONERROR);
			exit(0);
		}

		SetCursorPos(0, 0);

		// Sexy XInput joysticks (yes, the preference!)
		DWORD Result;
		XINPUT_STATE State;

		ZeroMemory(&State, sizeof(XINPUT_STATE));

		// Get state using 0'th index
		Result = XInputGetState(0, &State);

		if(Result == ERROR_SUCCESS)
		{
			UsingJoy = true;
			XInputJoy = true;
			return;
		}
		return;

		// Regular joysticks
		DIJOYCONFIG PreferredJoyConfig = {0};
		DI_ENUM_CONTEXT EnumContext;
		EnumContext.pPreferredJoyCfg = &PreferredJoyConfig;
		EnumContext.bPreferredJoyCfgValid = false;

		IDirectInputJoyConfig8* JoyConfig = 0;
		if(FAILED(DIObject->QueryInterface(IID_IDirectInputJoyConfig8, (void**) &JoyConfig)))
			return;

		PreferredJoyConfig.dwSize = sizeof(PreferredJoyConfig);
		if(SUCCEEDED(JoyConfig->GetConfig(0, &PreferredJoyConfig, DIJC_GUIDINSTANCE)))
			EnumContext.bPreferredJoyCfgValid = true;
		SAFE_RELEASE(JoyConfig);

		if(FAILED(DIObject->EnumDevices(DI8DEVCLASS_GAMECTRL, EnumJoysticksCallback, &EnumContext, DIEDFL_ATTACHEDONLY)))
			return;

		if(FAILED(DIJoy->SetDataFormat(&c_dfDIJoystick)))
			return;

		if(FAILED(DIJoy->SetCooperativeLevel((HWND)LastHWND, DISCL_NONEXCLUSIVE | DISCL_BACKGROUND)))
			return;

		DIJoy->Acquire();

		UsingJoy = true;

	}

	void InitOffsets(uint Win)
	{
		uint B = BBCreateButton("NONE", 0, 0, 10, 10, Win, 0);
		XOffset = BBGadgetX(B);
		YOffset = BBGadgetY(B);
		BBHideGadget(B);
	}

	void UpdateInput()
	{
		if(LastHWND != (uint)GetActiveWindow())
			return;

		HRESULT KHr = DIKeyboard->GetDeviceState(sizeof(KeyBuffer), (LPVOID)&KeyBuffer);
		if(KHr == DIERR_INPUTLOST || KHr == DIERR_NOTACQUIRED)
		{
			DIKeyboard->Acquire();
		}else
		{
			for(int i = 0; i < 256; ++i)
				if(KeyBuffer[i] & 0x80)
				{
					if(KeysHit[i] == 0)
						KeysHit[i] = 1;
				}else if(KeysHit[i] >= 2)
					KeysHit[i] = 0;
					

		}

		HRESULT MHr = DIMouse->GetDeviceState(sizeof(MouseState), (LPVOID)&MouseState);
		if(MHr == DIERR_INPUTLOST || MHr == DIERR_NOTACQUIRED)
		{
			DIMouse->Acquire();
		}else
		{
			for(int i = 0; i < 8; ++i)
				if(MouseState.rgbButtons[i] & 0x80)
					MouseHits[i] = true;

			/*AMouseX += MouseState.lX;
			AMouseY += MouseState.lY;
			AMouseZ += MouseState.lZ;
			if(AMouseX < 0) AMouseX = 0;
			if(AMouseY < 0) AMouseY = 0;
			if(AMouseZ < 0) AMouseZ = 0;*/	




			//POINT CP;
			//RECT WP, CR;
			//GetCursorPos(&CP);
			//GetWindowRect((HWND)LastHWND, &WP);
			//GetClientRect((HWND)LastHWND, &CR);

			//// Stop last positions
			//int tLMouseX = AMouseX;
			//int tLMouseY = AMouseY;

			//// Get window relative position
			//AMouseX = CP.x - WP.left - XOffset;
			//AMouseY = CP.y - WP.top - YOffset;

			//// Work out the exact client bounds
			//int Width = WP.right - WP.left;
			//int Height = WP.bottom - WP.top;
			//
			//// If its not in the bounds then reset it
			//if(AMouseX < 0 || AMouseX > Width || AMouseY < 0 || AMouseY > Height)
			//{
			//	AMouseX = tLMouseX;
			//	AMouseY = tLMouseY;
			//}

			//MXSpeed = AMouseX - LMouseX;
			//MYSpeed = AMouseY - LMouseY;
			//LMouseX = AMouseX;
			//LMouseY = AMouseY;




			//MXSpeed = MouseX() - LMouseX;
			//LMouseX = MouseX();

//			SetWindowText((HWND)LastHWND, String(MXSpeed).c_str());

			//if(AMouseX < 0) AMouseX = 0;
			//if(AMouseY < 0) AMouseY = 0;
			//if(AMouseX > WP.right - WP.left) AMouseX = WP.right - WP.left;
			//if(AMouseY > WP.bottom - WP.top) AMouseY = WP.bottom - WP.top;
		}

		if(UsingJoy && !XInputJoy)
		{
			if(DIERR_INPUTLOST == DIJoy->GetDeviceState(sizeof(JoyState), (LPVOID)&JoyState))
			{
				DIJoy->Acquire();
			}else
			{
				for(int i = 0; i < 32; ++i)
					if(JoyState.rgbButtons[i] & 0x80)
						JoyHits[i] = true;
				AJoyX = JoyState.lX;
				AJoyY = JoyState.lY;
				AJoyZ = JoyState.lZ;
				AJoyPitch = JoyState.lRx;
				AJoyYaw = JoyState.lRy;
				AJoyRoll = JoyState.lRz;
				AJoyU = JoyState.rglSlider[0];
				AJoyV = JoyState.rglSlider[1];

				AJoyX /= 32767.0f;
				AJoyY /= 32767.0f;
				AJoyZ /= 32767.0f;
				AJoyU /= 32767.0f;
				AJoyV /= 32767.0f;
				AJoyPitch /= 32767.0f;
				AJoyYaw /= 32767.0f;
				AJoyRoll /= 32767.0f;

				//AJoyY = 1.0f - AJoyY;

				AJoyX -= 1.0f;
				AJoyY -= 1.0f;
				AJoyZ -= 1.0f;
				AJoyU -= 1.0f;
				AJoyV -= 1.0f;
				AJoyPitch -= 1.0f;
				AJoyYaw -= 1.0f;
				AJoyRoll -= 1.0f;

				AJoyPitch *= 180.0f;
				AJoyYaw *= 180.0f;
				AJoyRoll *= 180.0f;
				
			}
		}else if(UsingJoy && XInputJoy)
		{
			DWORD Result;
			XINPUT_STATE State;

			ZeroMemory(&State, sizeof(XINPUT_STATE));

			Result = XInputGetState(0, &State);


			if(Result == ERROR_SUCCESS)
			{
				// Remove deadzones automatically
				if(State.Gamepad.sThumbLX < XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE && State.Gamepad.sThumbLX > -XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE)
					State.Gamepad.sThumbLX = 0;
				if(State.Gamepad.sThumbLY < XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE && State.Gamepad.sThumbLY > -XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE)
					State.Gamepad.sThumbLY = 0;
				
				if(State.Gamepad.sThumbRX < XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE && State.Gamepad.sThumbRX > -XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE)
					State.Gamepad.sThumbRX = 0;
				if(State.Gamepad.sThumbRY < XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE && State.Gamepad.sThumbRY > -XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE)
					State.Gamepad.sThumbRY = 0;

				
				JoyState.rgbButtons[0] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_A) ? 255 : 0;
				JoyState.rgbButtons[1] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_B) ? 255 : 0;
				JoyState.rgbButtons[3] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_X) ? 255 : 0;
				JoyState.rgbButtons[2] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_Y) ? 255 : 0;
				JoyState.rgbButtons[7] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_START) ? 255 : 0;
				JoyState.rgbButtons[6] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_BACK) ? 255 : 0;
				JoyState.rgbButtons[4] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_LEFT_SHOULDER) ? 255 : 0;
				JoyState.rgbButtons[5] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_RIGHT_SHOULDER) ? 255 : 0;
				JoyState.rgbButtons[8] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_LEFT_THUMB) ? 255 : 0;
				JoyState.rgbButtons[9] = (State.Gamepad.wButtons & XINPUT_GAMEPAD_RIGHT_THUMB) ? 255 : 0;
				bool DPadUp = (State.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_UP);
				bool DPadDown = (State.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_DOWN);
				bool DPadLeft = (State.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_LEFT);
				bool DPadRight = (State.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_RIGHT);

				JoyState.rgdwPOV[0] = -1;
				if(DPadUp)
					JoyState.rgdwPOV[0] = 0;
				else if(DPadRight)
					JoyState.rgdwPOV[0] = 9000;
				else if(DPadDown)
					JoyState.rgdwPOV[0] = 18000;
				else if(DPadLeft)
					JoyState.rgdwPOV[0] = 27000;

				AJoyU = ((float)State.Gamepad.bLeftTrigger / 255);
				AJoyV = ((float)State.Gamepad.bRightTrigger / 255);
				AJoyX = (State.Gamepad.sThumbLX / 32768.0f);
				AJoyY = (State.Gamepad.sThumbLY / 32768.0f);
				AJoyZ = (State.Gamepad.sThumbRX / 32768.0f);
				AJoyPitch = (State.Gamepad.sThumbRY / 32768.0f) * 180.0f;
			}
		
		}
	}

	bool KeyHit(int Key)
	{
		if(KeysHit[Key] == 1 || KeysHit[Key] == 3)
		{
			KeysHit[Key] = 2;
			return true;
		}
		return false;
	}

	bool KeyDown(int Key)
	{
		if(KeyBuffer[Key] & 0x80)
			return true;
		return false;
	}

	void FlushKeys()
	{
		for(int i = 0; i < 256; ++i)
			KeysHit[i] = 2;
	}

	bool IsAccKey(int Key)
	{
		if(Key == 42 || Key == 54 || Key == 56 || Key == 29)
			return false;
		return true;
	}

	int GetKey()
	{
		uchar K = 0;
		if((KeyBuffer[42] & 0x80) || (KeyBuffer[54] & 0x80))
		{
			for(int i = 0; i < 86; ++i)
			{
				if(IsAccKey(i) && UpperCharMap[i] != 0 && KeysHit[i] == 1)//KeyHit(i))//KeyBuffer[i] & 0x80)
				{
					K = UpperCharMap[i];
					KeysHit[i] = 3;
				}
			}
		}else
		{
			for(int i = 0; i < 86; ++i)
			{
				if(IsAccKey(i) && LowerCharMap[i] != 0  && KeysHit[i] == 1)//KeyHit(i))// KeyBuffer[i] & 0x80)
				{
					K = LowerCharMap[i];
					KeysHit[i] = 3;
				}
			}
		}
		if(K == 0)
		{
			if(KeyBuffer[181] & 0x80)
				K = '/';
			if(KeyBuffer[156] & 0x80)
				K = 13;
		}
		return K;
	}

	void FlushMouse()
	{
		for(int i = 0; i < 8; ++i)
			MouseHits[i] = false;
	}


	bool MouseDown(int Button)
	{
		--Button;
		if(MouseState.rgbButtons[Button] & 0x80)
		{
			return true;
		}
		return false;
	}

	bool MouseHit(int Button)
	{
		--Button;
		if(MouseHits[Button])
		{
			MouseHits[Button] = false;
			return true;
		}
		return false;
	}

	int MouseX()
	{	
		return AMouseX;// - XOffset;
	}

	int MouseY()
	{
		return AMouseY;// - YOffset;
	}

	int MouseXSpeed()
	{
		return MXSpeed;
		//return MouseState.lX;
	}

	int MouseYSpeed()
	{
		return MYSpeed;
		//return MouseState.lY;
	}

	int MouseZSpeed()
	{
		return MouseState.lZ;
	}

	void FlushJoy()
	{
		for(int i = 0; i < 32; ++i)
			JoyHits[i] = false;
	}

	bool JoyHit(int Button)
	{
		--Button;
		if(JoyHits[Button])
		{
			JoyHits[Button] = false;
			return true;
		}
		return false;
	}

	bool JoyDown(int Button)
	{
		--Button;
		return (JoyState.rgbButtons[Button] & 0x80);
	}

	float JoyX()
	{
		return AJoyX;
	}

	float JoyY()
	{
		return AJoyY;
	}

	float JoyZ()
	{
		return AJoyZ;
	}

	float JoyU()
	{
		return AJoyU;
	}

	float JoyV()
	{
		return AJoyV;
	}

	float JoyPitch()
	{
		return AJoyPitch;
	}

	float JoyYaw()
	{
		return AJoyYaw;
	}

	float JoyRoll()
	{
		return AJoyRoll;
	}

	int JoyHat(int Port)
	{
		int State = JoyState.rgdwPOV[Port];
		if(State == -1)
			return -1;
		return State / 100;
	}

	int JoyXDir()
	{
		if(AJoyX < -0.1f)
			return -1;
		else if(AJoyX > 0.1f)
			return 1;
		return 0;
	}

	int JoyYDir()
	{
		if(AJoyY < -0.1f)
			return -1;
		else if(AJoyY > 0.1f)
			return 1;
		return 0;
	}

	int JoyType()
	{
		return UsingJoy ? 1 : 0;
	}

	void SetVibration(float LeftMotor, float RightMotor)
	{
		if(UsingJoy && XInputJoy)
		{
			XINPUT_VIBRATION Vibration;
			ZeroMemory( &Vibration, sizeof(XINPUT_VIBRATION));

			Vibration.wLeftMotorSpeed = (int)(LeftMotor * 65535.0f);
			Vibration.wRightMotorSpeed = (int)(RightMotor * 65535.0f);

			XInputSetState( 0, &Vibration);
		}
	}
}