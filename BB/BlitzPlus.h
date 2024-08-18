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
/********************************************************
*                                                       *
* File: BlitzBasic Functions                            *
* Description:                                          *
*   All used BlitzBasic functions ported to C++         *
* for use with ported applications.                     *
*                                                       *
* Author: Jared Belkus                                  *
* Date: 2007-08-02                                      *
*                                                       *
* Notes:                                                *
*  Blitz returns all handles(pointers) as integers,     *
*   I'm keeping the tradition here, as access to        *
*   internal structures isn't needed.                   *
*                                                       *
*  MD5() Function uses external source which can be     *
*    obtained here:                                     *
*  http://sourceforge.net/project/showfiles.php?group_id=42360 *
********************************************************/
#pragma once

#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include <string>
#include <sstream>
#include "XMLWrapper.h"

namespace NGin
{
	namespace GUI
	{
		struct GUIUpdateParameters;
	}
}

// Added for Docs
//! Blitzplus namespace
namespace BlitzPlus
{

	// Stop deprecation warnings
	#pragma warning(disable : 4996)

	// Use BB Prefix on some functions...
	// Some functions (CreateWindow) are the same as windows'
	//  functions. If you need to call them then just toggle
	//  this.
	#define _BB_USEPREFIX

	// Type Definitions - Making life a bit easier
	//typedef unsigned int uint;
	typedef unsigned char uchar;
	typedef unsigned short ushort;

#ifndef BB_UINT_DEF
	typedef unsigned int uint;
#define BB_UINT_DEF
#endif

	// Callback Template
	typedef void (*DrawCallbackFN)(void* DC);

	// Don't ever modify this
	static bool DInputSet = false;
	extern uint LastHWND;
	extern NGin::GUI::GUIUpdateParameters NGUIUpdateParameters;

	extern "C" __declspec(dllexport) void* GetBBCommandData();

	// Blitzplus
	#ifdef _BB_USEPREFIX
	//! Create a window
	uint BBCreateWindow(std::string Title, int X, int Y, int Width, int Height, uint Group = 0, int Style = 1);

	//! Wait for a window event. Wait for Time milliseconds
	uint BBWaitEvent(int Time= 0);

	//! Create a button on the window
	uint BBCreateButton(std::string Text, int X, int Y, int Width, int Height, uint Group, int Style = 0);

	//! Query the object for a handle
	uint BBQueryObject(uint Object, int Query);

	//! Set the window title
	void BBAppTitle(std::string Title, std::string QuitMessage = "");

	//! Set the text of a button
	void BBSetGadgetText(uint Gadget, std::string Text);

	//! Get the X Position of a gadget
	int BBGadgetX(uint Gadget);

	//! Get the Y Position of a gadget
	int BBGadgetY(uint Gadget);

	//! Get the internal width of a gadget
	int BBClientWidth(uint Gadget);

	//! Get the internal height of a gadget
	int BBClientHeight(uint Gadget);

	//! Get the handle of the desktop
	uint BBDesktop();

	//! Set the position and size of a gadget
	void BBSetGadgetShape(uint Gadget, int X, int Y, int Width, int Height);

	//! Hide a gadget
	void BBHideGadget(uint Gadget);

	//! Show a gadget
	void BBShowGadget(uint Gadget);

	//! Delete a file
	void BBDeleteFile(std::string File);

	//! Register a callback for WM_PAINT
	void BBRegisterDrawCallback(DrawCallbackFN Fn, uint Wnd);
	#else
	uint CreateWindow(String Title, int X, int Y, int Width, int Height, uint Group = 0, int Style = 1);
	uint WaitEvent(int Time= 0);
	uint CreateButton(String Text, int X, int Y, int Width, int Height, uint Group, int Style = 0);
	uint QueryObject(uint Object, int Query);
	void AppTitle(String Title, String QuitMessage = "");
	void SetGadgetText(uint Gadget, String Text);
	int GadgetX(uint Gadget);
	int GadgetY(uint Gadget);
	int ClientWidth(uint Gadget);
	int ClientHeight(uint Gadget);
	int Desktop();
	void SetGadgetShape(uint Gadget, int X, int Y, int Width, int Height);
	void HideGadget(uint Gadget);
	void ShowGadget(uint Gadget);
	void DeleteFile(String File);
	void RegisterDrawCallback(DrawCallbackFN Fn, uint Wnd);
	#endif


	// Blitz IO
	//! Open a file for reading
	FILE* ReadFile(std::string Filename);

	//! Open a file for writing
	FILE* WriteFile(std::string Filename);

	//! Open a file for both reading and writing
	FILE* OpenFile(std::string Filename);

	//! Close an open file
	void CloseFile(FILE* F);

	//! Read a single byte
	uchar ReadByte(FILE* F);

	//! Read a two-byte short int
	ushort ReadShort(FILE* F);

	//! Read an integeter
	uint ReadInt(FILE* F);

	//! Read a float
	float ReadFloat(FILE* F);

	//! Read a string
	std::string ReadString(FILE* F);

	//! Write a byte
	void WriteByte(FILE* F, uchar B);

	//! Write a short int
	void WriteShort(FILE* F, ushort B);

	//! Write an int
	void WriteInt(FILE* F, uint B);

	//! Write a flo9at
	void WriteFloat(FILE* F, float B);

	//! Write a String
	void WriteString(FILE* F, std::string B);

	//! Return the current position in the file
	uint FilePos(FILE* F);

	//! Seek to a specific file position
	void SeekFile(FILE* F, uint Pos);

	//! Returns the size of a file
	uint FileSize(std::string Filename);

	//! Returns true of the end of the file has been reached
	bool Eof(FILE* F);

	//! Read a line within the file
	std::string ReadLine(FILE* F);

	//! Write a line of text to the file
	void WriteLine(FILE* F, std::string B);

	//! Open a directory for reading
	uint ReadDir(std::string Directory);

	//! Returns the name of the next file within the directory
	std::string NextFile(uint Dir);

	//! Close an open directory
	void CloseDir(uint Dir);

	//! Returns the type of the file
	int FileType(std::string File);

	// Math
	//! Returns the Cosine of the input Angle
	float Cos(float Angle);

	//! Returns the Sine of the input Angle
	float Sin(float Angle);

	//! Returns the Power of the inputs
	float Pow(float Num, float Exp);

	//! Returns the Power of the inputs
	float Pow(float Num, int Exp);

	//! Returns the Power of the inputs
	int Pow(int Num, int Exp);

	//! Round a number down
	float Floor(float Val);

	//! Round a number up
	float Ceil(float Val);

	//! Find the square root
	float Sqr(float Val);

	//! Returns the absolute value of the input
	int Abs(int Val);

	//! Returns the absolute value of the input
	float Abs(float Val);

	//! Determines if a value is less than, equal too or more than 0.0
	float Sgn(float Val);

	#ifdef _BB_USEPREFIX
	inline void AppTitle(std::string Title, std::string QuitMessage = "")
	{
		BBAppTitle(Title, QuitMessage);
	}
	#endif

	// NET
	//! Download a file from the internet
	bool DownloadFile(std::string File, std::string SaveTo);

	// BlitzTime
	//! Returns the current computer ticks in Milliseconds
	uint MilliSecs();

	//! Returns the current date
	std::string CurrentDate();

	//! Returns the current time
	std::string CurrentTime();

	//! Pauses execution for TimeMS Milliseconds
	void Delay(int TimeMS);

	// General
	//! Throws a runtime error
	void RuntimeError(std::string Error);

	//! Outputs a debug message
	void DebugLog(std::string Data);

	//! Returns the Handle() of an structure
	inline uint Handle(void* In) { return (uint)In; }

	//! Returns the String version of an ASCII Character
	std::string Chr(int Character);

	//! Converts a String to an ASCII Character
	unsigned char Asc(std::string Word);

	//! Find a random number
	float Rnd(float From, float To = RAND_MAX);

	//! Find a ranbom number
	int Rand(int From, int To = RAND_MAX);

	//! Set a seed for random numbers
	void SeedRnd(int Seed);

	//! Repeat a string over and over
	std::string BBString(std::string Str, int Repeat); // FN Prefix due to RealString conflict

	//! Return the MD5 of a string 
	std::string MD5(std::string Data); // Creates an MD5 has, see top of include!

	//! Hide the mouse cursor
	void HidePointer();

	//! Show the mouse cursor
	void ShowPointer();

	//! Move the mouse
	void MoveMouse(int X, int Y);

	// Input
	//! Initialise the input engine
	void InitInput();

	//! Update the input engine
	void UpdateInput();

	//! Returns true if a key was pressed
	bool KeyHit(int Key);

	//! Returns true if a key is still pressed
	bool KeyDown(int Key);

	//! Flush all key events
	void FlushKeys();

	//! Flush all mouse events
	void FlushMouse();

	//! Flush all joystick events
	void FlushJoy();

	//! Returns true if a mouse button is down
	bool MouseDown(int);

	//! Returns true if a mouse button was hit
	bool MouseHit(int);

	//! Returns the X Position of the mouse relative to the window position
	int MouseX();

	//! Returns the Y Position of the mouse relative to the window position
	int MouseY();

	//! Returns the amount of X movement in the mouse
	int MouseXSpeed();

	//! Returns the amount of Y movement in the mouse
	int MouseYSpeed();

	//! Returns the amount of Z movement in the mouse
	int MouseZSpeed();

	//! Get the last ASCII key that was pressed
	int GetKey();

	//! Returns true if a joystick button was hit
	bool JoyHit(int);

	//! Returns true of a joystick button is held down
	bool JoyDown(int);

	//! Returns the D-Pad status of the joystick
	int JoyHat(int Port = 0);

	//! Returns the current X direction of the joystick
	int JoyXDir();

	//! Returns the current Y direction of the joystick
	int JoyYDir();
	int JoyType();

	//! Returns the value of the X axis on the joystick
	float JoyX();

	//! Returns the value of the Y axis on the joystick
	float JoyY();

	//! Returns the value of the Z axis on the joystick
	float JoyZ();

	//! Returns the value of the U axis on the joystick
	float JoyU();

	//! Returns the value of the V axis on the joystick
	float JoyV();

	//! Returns the value of the Pitch axis on the joystick
	float JoyPitch();

	//! Returns the value of the Yaw axis on the joystick
	float JoyYaw();

	//! Returns the value of the Roll axis on the joystick
	float JoyRoll();

	//! Set the vibration of the joystick. This is only for XInput joysticks
	void SetVibration(float LeftMotor, float RightMotor);
	void InitOffsets(uint Win);

	// Joystick constants for xinput controllers
	#define XBOX_A		1
	#define XBOX_B		2
	#define XBOX_Y		3
	#define XBOX_X		4
	#define XBOX_LB		5
	#define XBOX_RB		6
	#define XBOX_BACK	7
	#define XBOX_START	8
	#define XBOX_LSTICK	9
	#define XBOX_RSTICK	10

}
using namespace BlitzPlus;
#include "BBDX.h"

#pragma warning(default : 4996)

