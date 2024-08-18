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

#include "Interface.h"

// JB: 2.50 Control Layout
RealmCrafter::IControlLayout* ControlLayout;

// JB: 2.52 fixed controls
RealmCrafter::IPlayerAttributeBars* PlayerAttributeBars = NULL;
RealmCrafter::IPlayerMap* PlayerMap = NULL;
bool PlayerMapVisible = false;
RealmCrafter::IPlayerCompass* PlayerCompass = NULL;
bool QuestLogVisible = false;
RealmCrafter::IPlayerQuestLog* PlayerQuestLog = NULL;
bool CharStatsVisible = false;
RealmCrafter::IPlayerStats* PlayerStats = NULL;
bool HelpVisible = false;
RealmCrafter::IPlayerHelp* PlayerHelp = NULL;
bool PartyVisible = false;
RealmCrafter::IPlayerParty* PlayerParty = NULL;

bool ItemWindowFromInventory = false;
IWindow* WItemWindow = 0;
char UseBubbles;
int BubblesR, BubblesG, BubblesB;

RealmCrafter::IPlayerActionBar* PlayerActionBar = NULL;
int ActionBarSlots[36]; // Values greater than 0 are item IDs, -1 to -10 are memorised spells, or if memorisation isn't used -1 to -1000 are known spells

bool ShowRadar = true, ShowActorsOnRadar;
int idPlayer, idEnemy, idFriendly, idNPC, idOthers;
std::list<tpMark> lpRadarMarks;	
IPictureBox* pRadarMainPlayer;

bool InventoryVisible = false;
RealmCrafter::IPlayerInventory* PlayerInventory = NULL;

RealmCrafter::IPlayerSpells* PlayerSpells = NULL;

bool SpellsVisible;
IWindow* WSpellRemove;
IButton* BSpellRemoveOK, *BSpellRemoveCancel;
int SpellRemoveNum;
IWindow* WSpellError;
IButton* BSpellError;
ILabel* LMemorising;
IProgressBar* SMemorising;
int MemoriseSpell, MemoriseSlot, MemoriseProgress, LastMemoriseUpdate;
uint LastSpellRecharge;

int LastMouseMove, LastLeftClick, MouseControl, InWaitPeriod, MouseDownTime, MouseClicks;
bool MouseWasDown;
uint RightDownTime;
int SelectKeyClickTime;
bool InDialog, SelectKeyWasDown, RightWasDown;

ITextBox* ChatEntry;

IRadar* RadarBox = NULL;

ClassDef(Dialog, DialogList, DialogDelete);
ClassDef(ScriptProgressBar, ScriptProgressBarList, ScriptProgressBarDelete);
ClassDef(TextInput, TextInputList, TextInputDelete);
ClassDef(Bubble, BubbleList, BubbleDelete);
ClassDef(EffectIcon, EffectIconList, EffectIconDelete);
ClassDef(EffectIconSlot, EffectIconSlotList, EffectIconSlotDelete);
ClassDef(InterfaceComponent, InterfaceComponentList, InterfaceComponentDelete);

// Gets the script handle for a dialog
uint DialogScriptHandle(uint Han)
{
	Dialog* D = (Dialog*)Han;
	if(D != 0)
		return D->ScriptHandle;
	return 0;
}

// Loads control bindings
// bool LoadControlBindings(string Filename)
// {
// 	FILE* F = ReadFile(Filename);
// 	if(F == 0)
// 		return false;
// 
// 	Key_Forward        = ReadInt(F);
// 	Key_Back           = ReadInt(F);
// 	Key_TurnRight      = ReadInt(F);
// 	Key_TurnLeft       = ReadInt(F);
// 	Key_FlyUp          = ReadInt(F);
// 	Key_FlyDown        = ReadInt(F);
// 	Key_Run            = ReadInt(F);
// 	Key_ChangeViewMode = ReadInt(F);
// 	Key_CameraRight    = ReadInt(F);
// 	Key_CameraLeft     = ReadInt(F);
// 	Key_CameraIn       = ReadInt(F);
// 	Key_CameraOut      = ReadInt(F);
// 	Key_Jump           = ReadInt(F);
// 	InvertAxis1        = (ReadByte(F) == 0) ? -1 : 1;
// 	InvertAxis3        = (ReadByte(F) == 0) ? -1 : 1;
// 	Key_Attack         = ReadInt(F);
// 	Key_AlwaysRun      = ReadInt(F);
// 	Key_CycleTarget    = ReadInt(F);
// 	Key_MoveTo         = ReadInt(F);
// 	Key_TalkTo         = ReadInt(F);
// 	Key_Select         = ReadInt(F);
// 
// 	CloseFile(F);
// 	return true;
// }
// 
// // Saves control bindings
// bool SaveControlBindings(string Filename)
// {
// 	FILE* F = WriteFile(Filename);
// 	if(F == 0)
// 		return false;
// 
// 	WriteInt(F, Key_Forward);
// 	WriteInt(F, Key_Back);
// 	WriteInt(F, Key_TurnRight);
// 	WriteInt(F, Key_TurnLeft);
// 	WriteInt(F, Key_FlyUp);
// 	WriteInt(F, Key_FlyDown);
// 	WriteInt(F, Key_Run);
// 	WriteInt(F, Key_ChangeViewMode);
// 	WriteInt(F, Key_CameraRight);
// 	WriteInt(F, Key_CameraLeft);
// 	WriteInt(F, Key_CameraIn);
// 	WriteInt(F, Key_CameraOut);
// 	WriteInt(F, Key_Jump);
// 
// 	if(InvertAxis1 == -1)
// 		WriteByte(F, 0);
// 	else
// 		WriteByte(F, 2);
// 
// 	if(InvertAxis3 == -1)
// 		WriteByte(F, 0);
// 	else
// 		WriteByte(F, 2);
// 
// 	WriteInt(F, Key_Attack);
// 	WriteInt(F, Key_AlwaysRun);
// 	WriteInt(F, Key_CycleTarget);
// 	WriteInt(F, Key_MoveTo);
// 	WriteInt(F, Key_TalkTo);
// 	WriteInt(F, Key_Select);
// 
// 	CloseFile(F);
// 	return true;
// }



// Returns the position at which a string should be split to word wrap to a maximum number of characters
int WordWrap(string St, int MaxChars)
{
	if(St.length() <= MaxChars)
		return 0;
	for(int i = MaxChars -1; i >= 0; --i)
		if(St.substr(i, 1).compare(string(" ")) == 0)
			return i;
	return MaxChars;
}

inline bool TextBoxIsFocused()
{
	if(GUIManager->ControlFocus() != NULL)
		return GUIManager->ControlFocus()->GetType() == ITextBox::TypeOf();

	return false;
}

bool CanMovePlayer()
{
	bool ModalWindow = BlitzPlus::NGUIUpdateParameters.ModalProc;
	return ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && InDialog == false && SMemorising == 0 && !TextBoxIsFocused() && !ModalWindow;
}

// Returns the number of times a control has been pressed since the last call
bool ControlHit(int Ctrl)
{
	// RESOLVE THESE!
	int MXSpeed = 0, MYSpeed = 0, MZSpeed = 0;
	bool JoyHatUp = false;


	// Keyboard
	if(Ctrl < 500)
		return KeyHit(Ctrl);

	// Mouse
	if(Ctrl < 1000)
	{
		switch(Ctrl)
		{
		case 501: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? false : MouseHit(1);
		case 502: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? false : MouseHit(2);
		case 503: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? false : MouseHit(3);
		case 504: if(MYSpeed < 3) return true;
		case 505: if(MYSpeed > 3) return true;
		case 506: if(MXSpeed > 3) return true;
		case 507: if(MXSpeed < 3) return true;
		case 508: if(MZSpeed > 0) return true;
		case 509: if(MZSpeed < 0) return true;
		case 510: return !NGin::Math::Vector3::FloatEqual(JoyX(), 0.0f);
		case 511: return !NGin::Math::Vector3::FloatEqual(JoyY(), 0.0f);
		case 512: return !NGin::Math::Vector3::FloatEqual(JoyZ(), 0.0f);
		case 513: return !NGin::Math::Vector3::FloatEqual(JoyU(), 0.0f);
		case 514: return !NGin::Math::Vector3::FloatEqual(JoyV(), 0.0f);
		}
	}

	if(Ctrl < 1009)
		return JoyHit(Ctrl - 1000);

	switch(Ctrl)
	{
	case 1009:
		if(JoyHat() == 0 || JoyHat() == 45 || JoyHat() == 315 )
			if(JoyHatUp == false)
			{
				JoyHatUp = true;
				return true;
			}
	case 1010:
		if(JoyHat() == 180 || JoyHat() == 135 || JoyHat() == 225 )
			if(JoyHatUp == false)
			{
				JoyHatUp = true;
				return true;
			}
	case 1011:
		if(JoyHat() == 90 || JoyHat() == 45 || JoyHat() == 135 )
			if(JoyHatUp == false)
			{
				JoyHatUp = true;
				return true;
			}
	case 1012:
		if(JoyHat() == 270 || JoyHat() == 225 || JoyHat() == 315 )
			if(JoyHatUp == false)
			{
				JoyHatUp = true;
				return true;
			}
	case 1013: if(JoyYDir() == -1) return true;
	case 1014: if(JoyYDir() == 1) return true;
	case 1015: if(JoyXDir() == 1) return true;
	case 1016: if(JoyXDir() == -1) return true;
	}

	return false;
}

float ControlDownF(int Ctrl)
{
	// RESOLVE THESE!
	int MXSpeed = 0, MYSpeed = 0, MZSpeed = 0;

	// Keyboard
	if(Ctrl < 500)
		return KeyDown(Ctrl) ? 1.0f : 0.0f;

	// Mouse
	if(Ctrl < 1000)
	{
		switch(Ctrl)
		{
		case 501: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? 0.0f : MouseDown(1) ? 1.0f : 0.0f;
		case 502: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? 0.0f : MouseDown(2) ? 1.0f : 0.0f;
		case 503: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? 0.0f : MouseDown(3) ? 1.0f : 0.0f;
		case 504: return MYSpeed;
		case 505: return MYSpeed;
		case 506: return MXSpeed;
		case 507: return MXSpeed;
		case 508: return MZSpeed;
		case 509: return MZSpeed;
		case 510: return JoyX();
		case 511: return JoyY();
		case 512: return JoyZ();
		case 513: return JoyU();
		case 514: return JoyV();
		}
	}

	if(Ctrl < 1009)
		return JoyDown(Ctrl - 1000) ? 1.0f : 0.0f;

	switch(Ctrl)
	{
	case 1009:
		if(JoyHat() == 0 || JoyHat() == 45 || JoyHat() == 315 )
			return 1.0f;
	case 1010:
		if(JoyHat() == 180 || JoyHat() == 135 || JoyHat() == 225 )
			return 1.0f;
	case 1011:
		if(JoyHat() == 90 || JoyHat() == 45 || JoyHat() == 135 )
			return 1.0f;
	case 1012:
		if(JoyHat() == 270 || JoyHat() == 225 || JoyHat() == 315 )
			return 1.0f;
	case 1013: return JoyYDir();
	case 1014: return JoyYDir();
	case 1015: return JoyXDir();
	case 1016: return JoyXDir();
	}

	return 0.0f;
}

// Returns whether the specified control is being held down
bool ControlDown(int Ctrl)
{
	// RESOLVE THESE!
	int MXSpeed = 0, MYSpeed = 0, MZSpeed = 0;

	// Keyboard
	if(Ctrl < 500)
		return KeyDown(Ctrl);

	// Mouse
	if(Ctrl < 1000)
	{
		switch(Ctrl)
		{
		case 501: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? false : MouseDown(1);
		case 502: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? false : MouseDown(2);
		case 503: return (BlitzPlus::NGUIUpdateParameters.MouseBusy) ? false : MouseDown(3);
		case 504: if(MYSpeed < 3) return true;
		case 505: if(MYSpeed > 3) return true;
		case 506: if(MXSpeed > 3) return true;
		case 507: if(MXSpeed < 3) return true;
		case 508: if(MZSpeed > 0) return true;
		case 509: if(MZSpeed < 0) return true;
		case 510: return !NGin::Math::Vector3::FloatEqual(JoyX(), 0.0f);
		case 511: return !NGin::Math::Vector3::FloatEqual(JoyY(), 0.0f);
		case 512: return !NGin::Math::Vector3::FloatEqual(JoyZ(), 0.0f);
		case 513: return !NGin::Math::Vector3::FloatEqual(JoyU(), 0.0f);
		case 514: return !NGin::Math::Vector3::FloatEqual(JoyV(), 0.0f);
		}
	}

	if(Ctrl < 1009)
		return JoyDown(Ctrl - 1000);

	switch(Ctrl)
	{
	case 1009:
		if(JoyHat() == 0 || JoyHat() == 45 || JoyHat() == 315 )
				return true;
	case 1010:
		if(JoyHat() == 180 || JoyHat() == 135 || JoyHat() == 225 )
				return true;
	case 1011:
		if(JoyHat() == 90 || JoyHat() == 45 || JoyHat() == 135 )
				return true;
	case 1012:
		if(JoyHat() == 270 || JoyHat() == 225 || JoyHat() == 315 )
				return true;
	case 1013: if(JoyYDir() == -1) return true;
	case 1014: if(JoyYDir() == 1) return true;
	case 1015: if(JoyXDir() == 1) return true;
	case 1016: if(JoyXDir() == -1) return true;
	}

	return false;
}

// Returns the name of a control number
string ControlName(int ControlNumber)
{
	switch(ControlNumber)
	{
		// Keyboard
		case 1 : return "Escape";
		case 2 : return "1";
		case 3 : return "2";
		case 4 : return "3";
		case 5 : return "4";
		case 6 : return "5";
		case 7 : return "6";
		case 8 : return "7";
		case 9 : return "8";
		case 10 : return "9";
		case 11 : return "0";
		case 12 : return "-";
		case 13 : return "=";
		case 14 : return "Backspace";
		case 15 : return "Tab";
		case 16 : return "Q";
		case 17 : return "W";
		case 18 : return "E";
		case 19 : return "R";
		case 20 : return "T";
		case 21 : return "Y";
		case 22 : return "U";
		case 23 : return "I";
		case 24 : return "O";
		case 25 : return "P";
		case 26 : return "[";
		case 27 : return "]";
		case 28 : return "Return";
		case 29 : return "Left Control";
		case 30 : return "A";
		case 31 : return "S";
		case 32 : return "D";
		case 33 : return "F";
		case 34 : return "G";
		case 35 : return "H";
		case 36 : return "J";
		case 37 : return "K";
		case 38 : return "L";
		case 39 : return ";";
		case 40 : return "'";
		case 42 : return "Left Shift";
		case 43 : return "\\";
		case 44 : return "Z";
		case 45 : return "X";
		case 46 : return "C";
		case 47 : return "V";
		case 48 : return "B";
		case 49 : return "N";
		case 50 : return "M";
		case 51 : return ",";
		case 52 : return ".";
		case 53 : return "/";
		case 54 : return "Right Shift";
		case 55 : return "Numpad *";
		case 56 : return "Left Alt";
		case 57 : return "Space";
		case 58 : return "Caps Lock";
		case 59 : return "F1";
		case 60 : return "F2";
		case 61 : return "F3";
		case 62 : return "F4";
		case 63 : return "F5";
		case 64 : return "F6";
		case 65 : return "F7";
		case 66 : return "F8";
		case 67 : return "F9";
		case 68 : return "F10";
		case 71 : return "Numpad 7";
		case 72 : return "Numpad 8";
		case 73 : return "Numpad 9";
		case 74 : return "Numpad -";
		case 75 : return "Numpad 4";
		case 76 : return "Numpad 5";
		case 77 : return "Numpad 6";
		case 78 : return "Numpad +";
		case 79 : return "Numpad 1";
		case 80 : return "Numpad 2";
		case 81 : return "Numpad 3";
		case 82 : return "Numpad 0";
		case 83 : return "Numpad .";
		case 87 : return "F11";
		case 88 : return "F12";
		case 156 : return "Enter";
		case 157 : return "Right Control";
		case 181 : return "Numpad /";
		case 184 : return "Right Alt";
		case 197 : return "Pause";
		case 199 : return "Home";
		case 200 : return "Up Arrow";
		case 201 : return "Page Up";
		case 203 : return "Left Arrow";
		case 205 : return "Right Arrow";
		case 207 : return "End";
		case 208 : return "Down Arrow";
		case 209 : return "Page Down";
		case 210 : return "Insert";
		case 211 : return "Delete";
		// Mouse
		case 501 : return "Left Mouse Button";
		case 502 : return "Right Mouse Button";
		case 503 : return "Middle Mouse Button";
		case 504 : return "Mouse Up";
		case 505 : return "Mouse Down";
		case 506 : return "Mouse Right";
		case 507 : return "Mouse Left";
		case 508 : return "Mouse Scroll Wheel Up";
		case 509 : return "Mouse Scroll Wheel Down";
		// Joystick
		case 1001 : return "Joystick Button 1";
		case 1002 : return "Joystick Button 2";
		case 1003 : return "Joystick Button 3";
		case 1004 : return "Joystick Button 4";
		case 1005 : return "Joystick Button 5";
		case 1006 : return "Joystick Button 6";
		case 1007 : return "Joystick Button 7";
		case 1008 : return "Joystick Button 8";
		case 1009 : return "Joystick Hat Up";
		case 1010 : return "Joystick Hat Down";
		case 1011 : return "Joystick Hat Right";
		case 1012 : return "Joystick Hat Left";
		case 1013 : return "Joystick Up";
		case 1014 : return "Joystick Down";
		case 1015 : return "Joystick Right";
		case 1016 : return "Joystick Left";
	}

	return LanguageString[LS_Unknown];
}
