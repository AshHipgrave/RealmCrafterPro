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

#include "Gooey.h"


// Stupid window definitions
#ifdef CreateWindow
#undef CreateWindow
#endif


// Allow background windows to be activated?
bool GY_Modal = false;

// Input
bool GY_EnterHit;
bool GY_LeftClick, GY_RightClick;
bool GY_LeftWasDown, GY_RightWasDown;
int GY_MouseXSpeed, GY_MouseYSpeed;
float GY_MouseX, GY_MouseY;


// Sounds
uint GY_SClick, GY_SBeep;


// Sets the alpha for an entity and all its children
void GY_RecursiveEntityAlpha(uint EN, float A)
{
	EntityAlpha(EN, A);
	for(int i = 1; i <= CountChildren(EN); ++i)
		GY_RecursiveEntityAlpha(GetChild(EN, i), A);
}

// Sets up all Gooey data
void GY_Load()
{
	// Sounds
	GY_SClick = LoadSound(GY_Path + string("\\Click.wav"));
	GY_SBeep = LoadSound(GY_Path + string("\\Beep.wav"));
}

// Updates the whole Gooey GUI
void GY_Update()
{
	BBWaitEvent();

	GY_EnterHit = KeyHit(28) || KeyHit(156);

	if(MouseDown(1) == false && GY_LeftWasDown == true)
		GY_LeftClick = true;
	else
		GY_LeftClick = false;

	GY_LeftWasDown = MouseDown(1);

	if(MouseDown(2) == false && GY_RightWasDown == true)
		GY_RightClick = true;
	else
		GY_RightClick = false;

	GY_RightWasDown = MouseDown(2);

	GY_MouseXSpeed = MouseXSpeed();
	GY_MouseYSpeed = MouseYSpeed();

	// Turn mouse position/speed into screen co-ordinates (0.0 - 1.0)
	GY_MouseX = ((float)MouseX()) / ((float)(GraphicsWidth()));
	GY_MouseY = ((float)MouseY()) / ((float)(GraphicsHeight()));
}

void MessageBox_MessageButton_Click(IControl* Sender, EventArgs* E)
{
	IWindow* Window = (IWindow*)Sender->Tag;
	GUIManager->Destroy(Window);
}

void MessageBoxYesNo(string Title, string Message, MessageBoxIcon Icon, EventHandler::CBFN OkCallback, EventHandler::CBFN CancelCallback)
{
#define R(X, Y) (Math::Vector2(X, Y) / GUIManager->GetResolution())
	Math::Vector2 Size = R(395, 115);
	Math::Vector2 Position = ((R(GUIManager->GetResolution().X, GUIManager->GetResolution().Y)) / 2) - (Size / 2);

	IWindow* MessageWindow = GUIManager->CreateWindow("Error Dialog", Position, Size);
	ILabel* MessageLabel = GUIManager->CreateLabel("Error Label", R(50, 12), R(312, 35));
	IPictureBox* MessageIcon = GUIManager->CreatePictureBox(std::string("ErrorIcon"), R(12, 12), R(32, 32));
	IButton* MessageYes    = GUIManager->CreateButton("Yes", R(120, 47), R(75, 23));
	IButton* MessageCancel = GUIManager->CreateButton("No", R(200, 47), R(75, 23));

	string BoxIcon = "";
	switch(Icon)
	{
	case MBI_Warning:
		BoxIcon = "Data\\UI\\Warning.png";
		break;
	case MBI_Error:
		BoxIcon = "Data\\UI\\Error.png";
		break;
	case MBI_Information:
		BoxIcon = "Data\\UI\\Information.png";
		break;
	}

	if(BoxIcon.length() == 0)
	{
		if(MessageIcon->SetImage(BoxIcon) == false)
			RuntimeError(string("Could not load ") + BoxIcon);
	}else
		MessageIcon->Visible = false;

	MessageLabel->Multiline = true;
	MessageLabel->VAlign = TextAlign_Middle;

	MessageLabel->Parent = MessageWindow;
	MessageIcon->Parent = MessageWindow;
	MessageYes->Parent = MessageWindow;
	MessageCancel->Parent = MessageWindow;

	MessageYes->Tag = MessageWindow;
	MessageCancel->Tag = MessageWindow;
	MessageWindow->Tag = MessageWindow;

	MessageWindow->Text = Title;
	MessageLabel->Text = Message;
	MessageYes->Click()->ClearEvents();
	MessageYes->Click()->AddEvent(&MessageBox_MessageButton_Click);
	MessageCancel->Click()->ClearEvents();
	MessageCancel->Click()->AddEvent(&MessageBox_MessageButton_Click);
	

	MessageWindow->Closed()->ClearEvents();
	MessageWindow->Closed()->AddEvent(&MessageBox_MessageButton_Click);
	
	if(OkCallback != 0)
	{
		MessageYes->Click()->AddEvent(OkCallback);
	}

	if(CancelCallback != 0)
	{
		MessageCancel->Click()->AddEvent(CancelCallback);
		MessageWindow->Closed()->AddEvent(CancelCallback);
	}

	MessageWindow->Visible = true;
	MessageWindow->Modal = true;

	#undef R
}

void MessageBox(string Title, string Message, MessageBoxIcon Icon, EventHandler::CBFN ClickCallback)
{
#define R(X, Y) (Math::Vector2(X, Y) / GUIManager->GetResolution())
	Math::Vector2 Size = R(395, 115);
	Math::Vector2 Position = ((R(GUIManager->GetResolution().X, GUIManager->GetResolution().Y)) / 2) - (Size / 2);

	IWindow* MessageWindow = GUIManager->CreateWindow("Error Dialog", Position, Size);
	ILabel* MessageLabel = GUIManager->CreateLabel("Error Label", R(50, 12), R(312, 35));
	IPictureBox* MessageIcon = GUIManager->CreatePictureBox(std::string("ErrorIcon"), R(12, 12), R(32, 32));
	IButton* MessageButton = GUIManager->CreateButton("OK", R(160, 47), R(75, 23));

	string BoxIcon = "";
	switch(Icon)
	{
	case MBI_Warning:
		BoxIcon = "Data\\UI\\Warning.png";
		break;
	case MBI_Error:
		BoxIcon = "Data\\UI\\Error.png";
		break;
	case MBI_Information:
		BoxIcon = "Data\\UI\\Information.png";
		break;
	}

	if(BoxIcon.length() > 0)
	{
		if(MessageIcon->SetImage(BoxIcon) == false)
			RuntimeError(string("Could not load ") + BoxIcon);
	}else
		MessageIcon->Visible = false;

	MessageLabel->Multiline = true;
	MessageLabel->VAlign = TextAlign_Middle;

	MessageLabel->Parent = MessageWindow;
	MessageIcon->Parent = MessageWindow;
	MessageButton->Parent = MessageWindow;

	MessageButton->Tag = MessageWindow;
	MessageWindow->Tag = MessageWindow;

	MessageWindow->Text = Title;
	MessageLabel->Text = Message;
	MessageButton->Click()->ClearEvents();
	MessageButton->Click()->AddEvent(&MessageBox_MessageButton_Click);
	

	MessageWindow->Closed()->ClearEvents();
	MessageWindow->Closed()->AddEvent(&MessageBox_MessageButton_Click);
	
	if(ClickCallback != 0)
	{
		MessageButton->Click()->AddEvent(ClickCallback);
		MessageWindow->Closed()->AddEvent(ClickCallback);
	}

	MessageWindow->Visible = true;
	MessageWindow->Modal = true;

	#undef R
}

