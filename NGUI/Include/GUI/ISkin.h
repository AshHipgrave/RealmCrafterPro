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

#include "IGUIManager.h"
#include "IFont.h"

namespace NGin
{
	namespace GUI
	{
		enum GUICoord
		{
			Window_Border_TL = 0,
			Window_Border_T = 1,
			Window_Border_TR = 2,
			Window_Border_L = 3,
			Window_Border_C = 4,
			Window_Border_R = 5,
			Window_Border_BL = 6,
			Window_Border_B = 7,
			Window_Border_BR = 8,
			Window_Fill = 9,
			Window_Close_Up = 10,
			Window_Close_Hover = 11,
			Window_Close_Down = 12,

			Button_Border_TL_Up = 13,
			Button_Border_T_Up = 14,
			Button_Border_TR_Up = 15,
			Button_Border_L_Up = 16,
			Button_Fill_Up = 17,
			Button_Border_R_Up = 18,
			Button_Border_BL_Up = 19,
			Button_Border_B_Up = 20,
			Button_Border_BR_Up = 21,

			Button_Border_TL_Hover = 22,
			Button_Border_T_Hover = 23,
			Button_Border_TR_Hover = 24,
			Button_Border_L_Hover = 25,
			Button_Fill_Hover = 26,
			Button_Border_R_Hover = 27,
			Button_Border_BL_Hover = 28,
			Button_Border_B_Hover = 29,
			Button_Border_BR_Hover = 30,

			Button_Border_TL_Down = 31,
			Button_Border_T_Down = 32,
			Button_Border_TR_Down = 33,
			Button_Border_L_Down = 34,
			Button_Fill_Down = 35,
			Button_Border_R_Down = 36,
			Button_Border_BL_Down = 37,
			Button_Border_B_Down = 38,
			Button_Border_BR_Down = 39,

			Button_Border_TL_Disabled = 40,
			Button_Border_T_Disabled = 41,
			Button_Border_TR_Disabled = 42,
			Button_Border_L_Disabled = 43,
			Button_Fill_Disabled = 44,
			Button_Border_R_Disabled = 45,
			Button_Border_BL_Disabled = 46,
			Button_Border_B_Disabled = 47,
			Button_Border_BR_Disabled = 48,

			CheckBox_Fill_Up = 49,
			CheckBox_Fill_Hover = 50,
			CheckBox_Fill_Down = 51,
			CheckBox_Fill_Disabled = 52,
			CheckBox_Check = 53,

			Label_Fill = 54,

			VScrollBar_Normal_Up = 55,
			VScrollBar_Normal_Down = 56,
			VScrollBar_Normal_Top = 57,
			VScrollBar_Normal_Middle = 58,
			VScrollBar_Normal_Bottom = 59,
			VScrollBar_Normal_Grip = 60,
			VScrollBar_Hover_Up = 61,
			VScrollBar_Hover_Down = 62,
			VScrollBar_Hover_Top = 63,
			VScrollBar_Hover_Middle = 64,
			VScrollBar_Hover_Bottom = 65,
			VScrollBar_Hover_Grip = 66,
			VScrollBar_Down_Up = 67,
			VScrollBar_Down_Down = 68,
			VScrollBar_Down_Top = 69,
			VScrollBar_Down_Middle = 70,
			VScrollBar_Down_Bottom = 71,
			VScrollBar_Down_Grip = 72,
			VScrollBar_Fill = 73,

			HScrollBar_Normal_Up = 74,
			HScrollBar_Normal_Down = 75,
			HScrollBar_Normal_Top = 76,
			HScrollBar_Normal_Middle = 77,
			HScrollBar_Normal_Bottom = 78,
			HScrollBar_Normal_Grip = 79,
			HScrollBar_Hover_Up = 80,
			HScrollBar_Hover_Down = 81,
			HScrollBar_Hover_Top = 82,
			HScrollBar_Hover_Middle = 83,
			HScrollBar_Hover_Bottom = 84,
			HScrollBar_Hover_Grip = 85,
			HScrollBar_Down_Up = 86,
			HScrollBar_Down_Down = 87,
			HScrollBar_Down_Top = 88,
			HScrollBar_Down_Middle = 89,
			HScrollBar_Down_Bottom = 90,
			HScrollBar_Down_Grip = 91,
			HScrollBar_Fill = 92,

			ProgressBar_Left = 93,
			ProgressBar_Middle = 94,
			ProgressBar_Right = 95,
			ProgressBar_Fill = 96,

			TrackBar_Button_Up = 97,
			TrackBar_Button_Hover = 98,
			TrackBar_Button_Down = 99,

			TrackBar_BackGround_Left = 100,
			TrackBar_BackGround_Middle = 101,
			TrackBar_BackGround_Right = 102,

			TrackBar_Tick_Large = 103,
			TrackBar_Tick_Small = 104,

			ListBox_Border_T = 105,
			ListBox_Border_L = 106,
			ListBox_Border_B = 107,
			ListBox_Border_R = 108,
			ListBox_Border_TL = 109,
			ListBox_Border_TR = 110,
			ListBox_Border_BL = 111,
			ListBox_Border_BR = 112,
			ListBox_Fill = 113,

			ComboBox_Left_Up = 114,
			ComboBox_Middle_Up = 115,
			ComboBox_Right_Up = 116,
			ComboBox_Fill_Up = 117,

			ComboBox_Left_Hover = 118,
			ComboBox_Middle_Hover = 119,
			ComboBox_Right_Hover = 120,
			ComboBox_Fill_Hover = 121,

			ComboBox_Left_Down = 122,
			ComboBox_Middle_Down = 123,
			ComboBox_Right_Down = 124,
			ComboBox_Fill_Down = 125,

			ComboBox_Left_Disabled = 126,
			ComboBox_Middle_Disabled= 127,
			ComboBox_Right_Disabled = 128,
			ComboBox_Fill_Disabled = 129,

			TextBox_Border_TL = 130,
			TextBox_Border_T = 131,
			TextBox_Border_TR = 132,
			TextBox_Border_L = 133,
			TextBox_Border_R = 134,
			TextBox_Border_BL = 135,
			TextBox_Border_B = 136,
			TextBox_Border_BR = 137,
			TextBox_Fill = 138,
			TextBox_Caret = 139,

			ListBox_SelectionFill = 140
		};




		//! GUIFont enumeration
		enum GUIFont
		{
			//! Default GUI Font for onscreen elements
			GUIFont_Default = 0,

			//! GUI Font for window title
			GUIFont_Window_Title = 1,

			//! GUI Font for controls (Buttons, Labels)
			GUIFont_Control = 2
		};
		#define TotalFontCount 3

		//! GUICursor enumations
		enum GUICursor
		{
			//! Default pointy cursor
			GUICursor_Pointer = 0,

			//! Text input cursor
			GUICursor_TextInput = 1
		};

		class IFont;

		//! Skin Interface class
		/*!
		The Skin interface is used for controls to get information
		about GUI control images and GUI Fonts from any loaded skin.
		*/
		class ISkin
		{
		public:

			ISkin(){}
			virtual ~ISkin(){}

			//! Get texture UV of upper left corner of the GUI control image
			/*!
			\param Coordinate ID of GUI control image to to use
			\return Floating point UV Coordinate
			*/
			virtual NGin::Math::Vector2 GetCoord(GUICoord Coordinate) = 0;

			//! Get texture UV of lower right corner of the GUI control image
			/*!
			\param Coordinate ID of GUI control image to to use
			\return Floating point UV Coordinate
			*/
			virtual NGin::Math::Vector2 GetSize(GUICoord Coordinate) = 0;

			//! Get size of a GUI control image in relation to the screen resolution
			/*!
			\param Coordinate ID of GUI control image to to use
			\return Floating point size
			*/
			virtual NGin::Math::Vector2 GetScreenCoord(GUICoord Coordinate) = 0;

			//! Get texture UV of upper left corner of the GUI control image
			/*!
			\param Coordinate ID of GUI control image to to use
			\return Floating point UV Coordinate
			*/
			virtual NGin::Math::Vector2 GetCoord(std::string Coordinate) = 0;

			//! Get texture UV of lower right corner of the GUI control image
			/*!
			\param Coordinate ID of GUI control image to to use
			\return Floating point UV Coordinate
			*/
			virtual NGin::Math::Vector2 GetSize(std::string Coordinate) = 0;

			//! Get size of a GUI control image in relation to the screen resolution
			/*!
			\param Coordinate ID of GUI control image to to use
			\return Floating point size
			*/
			virtual NGin::Math::Vector2 GetScreenCoord(std::string Coordinate) = 0;

			//! Apply a default property set to the specific control (Used for custom controls)
			virtual void ApplyDefaultProperty(IControl* Control, std::string SetName) = 0;

			//! Get a font handle from a loaded skin
			/*!
			\param Font ID of GUI font to obtain
			\return Handle of IFont structure for use with controls
			*/
			virtual IFont* GetFont(GUIFont Font) = 0;

			//! Get a cursor handle from a loaded skin
			/*!
			\param Cursor ID of GUI Cursor to obtain
			\return Index of cursor
			*/
			virtual int GetCursor(GUICursor Cursor) = 0;
		};

	}
}