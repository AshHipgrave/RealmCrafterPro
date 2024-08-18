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

#include <BBDX.h>
#include "MainMenu.h"
#include "CControlEditor2.h"

string CharNames[10];
int CharActors[10];
int CharGender[10];
int CharFaceTex[10];
int CharHair[10];
int CharBeard[10];
int CharBodyTex[10];
NGin::CString CharList;
string UName, PWord;



using namespace NGin;
using namespace NGin::Math;

class MainMenu;
MainMenu* M;


class MainMenu
{
public:

	// Manager handle
	IGUIManager* Manager;

	// Very main menu has controls parented to the screen
	ArrayList<IControl*> MainMenuControls;

	// Other windows
	IWindow* InGameMenu;

	// All controls
	IButton *OptionsButton, *ExitGame;
	IButton* RealmButton;

	// Login Screen
	IWindow* LoginWindow;
	ITextBox* LoginUser, *LoginPassword, *LoginEmail;
	IButton* LoginCreate, *LoginLogin;

	// Generic error messagebox
	IWindow* MessageWindow;
	IButton* MessageButton;
	ILabel* MessageLabel;
	IPictureBox* MessageIcon;

	// Selection Window
	IWindow* CSelWindow;
	IButton* BStart, *BDelete, *BLeft, *BRight, *NewButton;
	ArrayList<IButton*> CharButtons;

	// Creation windows
	IWindow* CActorWindow, *CAttributesWindow;
	IComboBox* CActorsList;
	ArrayList<IButton*> AttributesLeftSelect, AttributesRightSelect;
	ArrayList<ILabel*> AttrName, AttrValue;
	ILabel* RemainingLabel;
	IButton *BGenderL, *BClassL, *BHairL, *BFaceL, *BBeardL, *BClothesL;
	IButton *BGenderR, *BClassR, *BHairR, *BFaceR, *BBeardR, *BClothesR;
	ILabel* ActorInfo;
	ILabel* LName;
	ITextBox* TName;
	IButton* BDone, *BCancel;

	IWindow* EULAWindow;
	ILabel* EULALabel;
	IScrollBar* EULAScroll;
	IButton* EULAAccept;
	IButton* EULACancel;


	string CurrentArea;

	bool MenuOpen;

	// Actor Download variables
	bool HadAttributes;
	bool HadDamageTypes;
	bool HadEnvironment;
	int ActorsCreated;
	int ActorsRequired;
	int ItemsCreated;
	int ItemsRequired;
	int FactionsReceived;

	// Character Selection
	float CamAngle;
	uint Set;
	uint GPP;
	ActorInstance* PreviewA;
	int SelectedChar;


	int PointSpends[40];
	int PointsToSpend;

	int Quests;
	int RequiredQuests;
	int Spells;
	int RequiredSpells;
	int Memorised;
	int ItemsDone;
	int AttributesDone;

	static bool Loaded;
	
	//BInvertAxis1
	//BInvertAxis3
	//BControlsDone
	

	// Construct
	MainMenu(IGUIManager* GUIManager);

#pragma region Methods
	void RunMenu();
	void HideAll();

	void ShowInGameMenu();
	void ShowRootMenu();
	void ShowServerSelector();
	void ShowLoginScreen();
	void ShowErrorMessage(string Caption, string Message, EventHandler::CBFN* Callback);
	void SetupCharacterSelection();
	void ShowCharacterSelection();
	void SetupCharacterCreation();
	void ShowCharacterCreation();
	void ShowEULAWindow();
	void ConnectToServer();

	void NetCreateAccount(NGin::CString &Message);
	void NetLoginValidate(NGin::CString &Message);
	void NetFetchActors(NGin::CString& Message);
	void NetDeleteCharacter(NGin::CString& Message);
	void NetCreateCharacter(NGin::CString& Message);
	void NetFetchCharacter(NGin::CString& Message);

	void EULAScroll_Scroll(IControl* sender, ScrollEventArgs* e);
	void EULAAccept_Click(IControl* sender, EventArgs* e);
	void EULACancel_Click(IControl* sender, EventArgs* e);

	void ServerSelector_Closed(IControl* sender, EventArgs* e);
	void RealmButton_Click(IControl* sender, EventArgs* e);

#pragma endregion

#pragma region Static Methods
	static void MessageButton_Click(IControl* Sender, EventArgs* E);
	static void ReturnToStartError(IControl* Sender, EventArgs* E);
	static void ReturnToLoginError(IControl* Sender, EventArgs* E);
	static void ReturnToCreationError(IControl* Sender, EventArgs* E);
	static void ReturnToSelectionError(IControl* Sender, EventArgs* E);


	static void OptionsButton_Click(IControl* Sender, EventArgs* E);
	static void ExitGame_Click(IControl* Sender, EventArgs* E);

	static void SelectorWindow_Close(IControl* Sender, EventArgs* E);
	static void SelectorConnect_Click(IControl* Sender, EventArgs* E);
	static void SelectorList_SelectedIndexChanged(IControl* Sender, EventArgs* E);

	static void LoginCreate_Click(IControl* Sender, EventArgs* E);
	static void LoginLogin_Click(IControl* Sender, EventArgs* E);
	static void LoginWindow_Close(IControl* Sender, EventArgs* E);

	static void CharacterSelect_Click(IControl* Sender, EventArgs* E);
	static void BLeft_MouseDown(IControl* Sender, EventArgs* E);
	static void BRight_MouseDown(IControl* Sender, EventArgs* E);
	static void BDelete_Click(IControl* Sender, EventArgs* E);
	static void BStart_Click(IControl* Sender, EventArgs* E);
	static void BCreateChar_Click(IControl* Sender, EventArgs* E);
	static void BDeleteConfirm(IControl* Sender, EventArgs* E);

	static void CActorsList_SelectedIndexChanged(IControl* Sender, EventArgs* E);

	static void BGender_Click(IControl* Sender, EventArgs* E);

	static void BHairL_Click(IControl* Sender, EventArgs* E);
	static void BHairR_Click(IControl* Sender, EventArgs* E);

	static void BBeardL_Click(IControl* Sender, EventArgs* E);
	static void BBeardR_Click(IControl* Sender, EventArgs* E);

	static void BFaceL_Click(IControl* Sender, EventArgs* E);
	static void BFaceR_Click(IControl* Sender, EventArgs* E);

	static void BClothesL_Click(IControl* Sender, EventArgs* E);
	static void BClothesR_Click(IControl* Sender, EventArgs* E);

	static void BClassL_Click(IControl* Sender, EventArgs* E);
	static void BClassR_Click(IControl* Sender, EventArgs* E);

	static void AttrIncrease_Click(IControl* Sender, EventArgs* E);
	static void AttrDecrease_Click(IControl* Sender, EventArgs* E);

	static void BDone_Click(IControl* Sender, EventArgs* E);
	static void BCancel_Click(IControl* Sender, EventArgs* E);
#pragma endregion
};
bool MainMenu::Loaded = false;
MainMenu* MenuInstance = NULL;


// Construct
MainMenu::MainMenu(IGUIManager* GUIManager)
{
#pragma region Default Members
	Manager = GUIManager;
	MenuOpen = true;

	HadAttributes = false;
	HadDamageTypes = false;
	HadEnvironment = false;
	ActorsCreated = 0;
	ActorsRequired = -1;
	ItemsCreated = 0;
	ItemsRequired = -1;
	FactionsReceived = 0;

	CamAngle = 0.0f;
	Set = 0;
	GPP = 0;
	PreviewA = 0;
	SelectedChar = -1;

	
	PointsToSpend = 0;

	Quests = 0;
	RequiredQuests = 1000;
	Spells = 0;
	RequiredSpells = 2000;
	Memorised = 0;
	ItemsDone = 0;
	AttributesDone = 0;


#pragma endregion

	// Use a macro to make life easier
	#define R(X, Y) (Vector2(X, Y) / GUIManager->GetResolution())

	// Setup all components
#pragma region OptionsButton

	Vector2 Position = Vector2(0.2f, 0.3f);
	Vector2 Size = R(372, 192);

	Position = Vector2(0.5f, 0.5f) - (Size / 2.0f);

#pragma endregion

#pragma region Misc Options

	Size = R(240, 130);

#pragma endregion

#pragma region Control Options

	Size = R(255, 605);

#pragma endregion

#pragma region Server Selector

	Size = R(232, 243);
	Position = ((R(Manager->GetResolution().X, Manager->GetResolution().Y)) / 2) - (Size / 2);



#pragma endregion

#pragma region Login Window

	Size = R(250, 156);
	Position = ((R(Manager->GetResolution().X, Manager->GetResolution().Y)) / 2) - (Size / 2);

	LoginWindow = Manager->CreateWindow("MainMenu::Login", Position, Size);
	LoginWindow->Text = "Login";
	LoginWindow->CloseButton = false;
	LoginWindow->Locked = true;
	
	ILabel* L = Manager->CreateLabel("MainMenu::Login::UsernameLabel", R(12, 15), Vector2(0, 0));
	L->Text = LanguageString[LS_Username];
	L->Parent = LoginWindow;

	L = Manager->CreateLabel("MainMenu::Login::PasswordLabel", R(12, 41), Vector2(0, 0));
	L->Text = LanguageString[LS_Password];
	L->Parent = LoginWindow;

	L = Manager->CreateLabel("MainMenu::Login::EmailLabel", R(12, 67), Vector2(0, 0));
	L->Text = LanguageString[LS_EmailAddr];
	L->Parent = LoginWindow;

	LoginUser = Manager->CreateTextBox("MainMenu::Login::UsernameTextBox", R(121, 12), R(100, 20));
	LoginPassword = Manager->CreateTextBox("MainMenu::Login::PasswordTextBox", R(121, 38), R(100, 20));
	LoginEmail = Manager->CreateTextBox("MainMenu::Login::EmailTextBox", R(121, 64), R(100, 20));

	LoginCreate = Manager->CreateButton("MainMenu::Login::CreateButton", R(15, 90), R(100, 23));
	LoginLogin = Manager->CreateButton("MainMenu::Login::LoginButton", R(121, 90), R(100, 23));

	LoginCreate->Text = "Create Account";
	LoginLogin->Text = "Login";
	LoginUser->Text = "";
	LoginPassword->Text = "";
	LoginEmail->Text = "";

	LoginUser->ValidationExpression = "[0-9a-zA-Z_]";
	LoginPassword->ValidationExpression = "[0-9a-zA-Z._]";
	LoginEmail->ValidationExpression = "[0-9a-zA-Z*+-.=_@]";

	LoginPassword->PasswordCharacter = "*";

	LoginCreate->Click()->AddEvent(&LoginCreate_Click);
	LoginLogin->Click()->AddEvent(&LoginLogin_Click);

	LoginWindow->Tag = this;
	LoginCreate->Tag = this;
	LoginLogin->Tag = this;

	LoginUser->Parent = LoginWindow;
	LoginPassword->Parent = LoginWindow;
	LoginEmail->Parent = LoginWindow;
	LoginCreate->Parent = LoginWindow;
	LoginLogin->Parent = LoginWindow;

#pragma endregion

#pragma region ErrorDialog

	Size = R(395, 115);
	Position = ((R(Manager->GetResolution().X, Manager->GetResolution().Y)) / 2) - (Size / 2);

	MessageWindow = Manager->CreateWindow("Error Dialog", Position, Size);
	MessageLabel = Manager->CreateLabel("Error Label", R(50, 12), R(312, 35));
	MessageIcon = Manager->CreatePictureBox(std::string("ErrorIcon"), R(12, 12), R(32, 32));
	MessageButton = Manager->CreateButton("OK", R(160, 47), R(75, 23));

	if(MessageIcon->SetImage(string("Data\\UI\\Warning.png")) == false)
		RuntimeError("Could not load Data\\UI\\Warning.png");

	MessageLabel->Multiline = true;
	MessageLabel->VAlign = TextAlign_Middle;

	MessageLabel->Parent = MessageWindow;
	MessageIcon->Parent = MessageWindow;
	MessageButton->Parent = MessageWindow;

	MessageButton->Tag = this;
	MessageWindow->Tag = this;

#pragma endregion

#pragma region Root Menu
	OptionsButton = GUIManager->CreateButton("MainMenu::OptionsButton", Vector2(0.371f, 0.396f), Vector2(0.258f, 0.043f));
	ExitGame = GUIManager->CreateButton("MainMenu::ExitGameButton", Vector2(0.371f, 0.596f), Vector2(0.258f, 0.043f));
	RealmButton = GUIManager->CreateButton("MainMenu::RealmButton", Vector2(0, 0), Vector2(0, 0));

	OptionsButton->Text = "Settings";
	ExitGame->Text = "Quit";

	OptionsButton->Click()->AddEvent(&OptionsButton_Click);
	ExitGame->Click()->AddEvent(&ExitGame_Click);
	RealmButton->Click()->AddEvent(this, &MainMenu::RealmButton_Click);

	OptionsButton->Tag = this;
	ExitGame->Tag = this;
	RealmButton->Tag = this;

#pragma endregion

#pragma region EULA Window

	EULAWindow = GUIManager->CreateWindow("MainMenu::EULAWindow::Window", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(0.6f, 0.6f));
	EULALabel = GUIManager->CreateLabel("MainMenu::EULAWindow::Label", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(1, 1));
	EULAScroll = GUIManager->CreateScrollBar("MainMenu::EULAWindow::Scrollbar", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(1, 1), VerticalScroll);
	EULAAccept = GUIManager->CreateButton("MainMenu::EULAWindow::Accept", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(1, 1));
	EULACancel = GUIManager->CreateButton("MainMenu::EULAWindow::Cancel", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(1, 1));

	EULALabel->Parent = EULAWindow;
	EULAScroll->Parent = EULAWindow;
	EULAAccept->Parent = EULAWindow;
	EULACancel->Parent = EULAWindow;

	//EULAWindow->Location = R(100, 100);
	//EULAWindow->Size = R(471, 481);
	//EULAWindow->Text = "";
	//EULAWindow->CloseButton = false;

	//EULALabel->Multiline = true;
	//EULALabel->Location = R(12, 12);
	//EULALabel->Size = R(411, 386);
	//EULALabel->ForceScissoring = true;
	//EULALabel->ScissorWindow = R(411, 386);

	//EULAScroll->Location = R(426, 12);
	//EULAScroll->Size = R(12, 386);

	//EULAAccept->Location = R(287, 410);
	//EULAAccept->Size = R(75, 23);
	//EULAAccept->Text = LanguageString[LS_Accept];

	//EULACancel->Location = R(368, 410);
	//EULACancel->Size = R(75, 23);

	EULAWindow->Location = R(100, 100);
	EULAWindow->Size = R(100, 100);
	EULAWindow->Text = "";
	EULAWindow->CloseButton = false;

	EULALabel->Multiline = true;
	EULALabel->Location = R(12, 12);
	EULALabel->Size = R(100, 100);
	EULALabel->ForceScissoring = true;
	EULALabel->ScissorWindow = R(411, 386);

	EULAScroll->Location = R(426, 12);
	EULAScroll->Size = R(100, 100);

	EULAAccept->Location = R(287, 410);
	EULAAccept->Size = R(100, 100);
	EULAAccept->Text = LanguageString[LS_Accept];

	EULACancel->Location = R(368, 410);
	EULACancel->Size = R(100, 100);
	EULACancel->Text = LanguageString[LS_Cancel];

	EULAScroll->Scroll()->AddEvent(this, &MainMenu::EULAScroll_Scroll);
	EULAAccept->Click()->AddEvent(this, &MainMenu::EULAAccept_Click);
	EULACancel->Click()->AddEvent(this, &MainMenu::EULACancel_Click);

	GUIManager->SetProperties("MainMenu::EULAWindow");

	FILE* F = ReadFile("Data\\Game Data\\EULA.txt");
	string UseStr = "";
	while((!Eof(F)) && (F != 0))
	{
		UseStr.append(ReadLine(F));
		UseStr.append("\n");
	}

	EULALabel->Text = UseStr;
	
	float Total = EULALabel->InternalHeight * GUIManager->GetResolution().Y;
	float Visible = EULALabel->ScissorWindow.Y * GUIManager->GetResolution().Y;
	float Extra = Total - Visible;

	if(Extra > 0)
	{
		EULAScroll->Minimum = 0;
		EULAScroll->Maximum = (int)Total;//Extra;
		EULAScroll->Value = 0;
		EULAScroll->SmallChange = Visible;
		EULAScroll->LargeChange = Visible;
	}

	EULAWindow->Visible = false;
	EULAWindow->Tag = this;
	EULAAccept->Tag = this;
	EULACancel->Tag = this;

#pragma region Character Selection

	BStart  = Manager->CreateButton("MainMenu::CharacterSelection::StartButton", Vector2(0.82f,  0.9f), Vector2(0.16f,  0.05f));
	BDelete = Manager->CreateButton("MainMenu::CharacterSelection::DeleteButton", Vector2(0.54f,  0.9f), Vector2(0.25f,  0.05f));
	BLeft   = Manager->CreateButton("MainMenu::CharacterSelection::LeftButton", Vector2(0.396f, 0.896f), Vector2(0.043f, 0.058f));
	BRight  = Manager->CreateButton("MainMenu::CharacterSelection::RightButton", Vector2(0.446f, 0.896f), Vector2(0.043f, 0.058f));
	
	BLeft->Text = "<<";
	BRight->Text = ">>";
	BStart->Text = "Start Game";
	BDelete->Text = "Delete Character";

	CSelWindow = Manager->CreateWindow("MainMenu::CharacterSelection", Vector2(0.03f, 0.05f), R(164, 400));
	CSelWindow->Text = "Character";
	CSelWindow->CloseButton = false;

	BLeft->Tag = this;
	BRight->Tag = this;
	BStart->Tag = this;
	BDelete->Tag = this;

	BLeft->MouseDown()->AddEvent(&BLeft_MouseDown);
	BRight->MouseDown()->AddEvent(&BRight_MouseDown);
	BDelete->Click()->AddEvent(&BDelete_Click);
	BStart->Click()->AddEvent(&BStart_Click);

#pragma endregion

#pragma region Character Creation

	CAttributesWindow = Manager->CreateWindow("MainMenu::CharacterCreation::AttributesWindow", Vector2(0.67f, 0.05f), R(217, 310));
	CActorWindow = Manager->CreateWindow("MainMenu::CharacterCreation::ActorWindow", Vector2(0.03f, 0.05f), R(217, 270));
	CAttributesWindow->Text = LanguageString[LS_AttributesTitle];
	CActorWindow->Text = LanguageString[LS_CharacterTitle];

	CAttributesWindow->CloseButton = false;
	CActorWindow->CloseButton = false;

	CActorsList = Manager->CreateComboBox("MainMenu::CharacterCreation::ActorsList", R(12, 12), R(174, 120));
	CActorsList->SelectedIndexChanged()->AddEvent(&CActorsList_SelectedIndexChanged);
	CActorsList->Tag = this;

	TName = Manager->CreateTextBox("MainMenu::CharacterCreation::NameTextBox", Vector2(0.36f, 0.86f), R(200, 20));
	BDone = Manager->CreateButton("MainMenu::CharacterCreation::CreateButton", Vector2(0.836f, 0.936f), Vector2(0.158f, 0.058f));
	BCancel = Manager->CreateButton("MainMenu::CharacterCreation::CancelButton", Vector2(0.636f, 0.936f), Vector2(0.158f, 0.058f));

	TName->Text = "";
	BDone->Text = "Create Character";
	BCancel->Text = "Cancel Creation";

	TName->ValidationExpression = "[0-9a-zA-Z._]";

	BDone->Tag = this;
	BCancel->Tag = this;

	BDone->Click()->AddEvent(&BDone_Click);
	BCancel->Click()->AddEvent(&BCancel_Click);

	LName = Manager->CreateLabel("MainMenu::CharacterCreation::NameLabel", Vector2(0.27f, 0.865f), Vector2(0, 0));
	LName->Text = LanguageString[LS_CharacterName] + "      ";
	LName->ForeColor = Math::Color(255, 255, 255);
	

	BGenderL	= Manager->CreateButton("MainMenu::CharacterCreation::GenderLButton", R(103, 39), R(23, 23));
	BClassL		= Manager->CreateButton("MainMenu::CharacterCreation::ClassLButton", R(103, 68), R(23, 23));
	BHairL		= Manager->CreateButton("MainMenu::CharacterCreation::HairLButton", R(103, 97), R(23, 23));
	BFaceL		= Manager->CreateButton("MainMenu::CharacterCreation::FaceLButton", R(103, 126), R(23, 23));
	BBeardL		= Manager->CreateButton("MainMenu::CharacterCreation::BeardLButton", R(103, 155), R(23, 23));
	BClothesL	= Manager->CreateButton("MainMenu::CharacterCreation::ClothesLButton", R(103, 184), R(23, 23));

	BGenderR	= Manager->CreateButton("MainMenu::CharacterCreation::GenderRButton", R(163, 39), R(23, 23));
	BClassR		= Manager->CreateButton("MainMenu::CharacterCreation::ClassRButton", R(163, 68), R(23, 23));
	BHairR		= Manager->CreateButton("MainMenu::CharacterCreation::HairRButton", R(163, 97), R(23, 23));
	BFaceR		= Manager->CreateButton("MainMenu::CharacterCreation::FaceRButton", R(163, 126), R(23, 23));
	BBeardR		= Manager->CreateButton("MainMenu::CharacterCreation::BeardRButton", R(163, 155), R(23, 23));
	BClothesR	= Manager->CreateButton("MainMenu::CharacterCreation::ClothesRButton", R(163, 184), R(23, 23));

	BGenderL->Text = "<";
	BClassL->Text = "<";
	BHairL->Text = "<";
	BFaceL->Text = "<";
	BBeardL->Text = "<";
	BClothesL->Text = "<";
	BGenderR->Text = ">";
	BClassR->Text = ">";
	BHairR->Text = ">";
	BFaceR->Text = ">";
	BBeardR->Text = ">";
	BClothesR->Text = ">";

	ActorInfo = Manager->CreateLabel("MainMenu::CharacterCreation::ActorInfoLabel", R(12, 210), R(174, 12));
	ActorInfo->Text = "";
	ActorInfo->Multiline = true;

	BGenderL->Tag = this;
	BClassL->Tag = this;
	BHairL->Tag = this;
	BFaceL->Tag = this;
	BBeardL->Tag = this;
	BClothesL->Tag = this;
	BGenderR->Tag = this;
	BClassR->Tag = this;
	BHairR->Tag = this;
	BFaceR->Tag = this;
	BBeardR->Tag = this;
	BClothesR->Tag = this;
	CActorsList->Tag = this;

	BGenderL->Enabled = false;
	BClassL->Enabled = false;
	BHairL->Enabled = false;
	BFaceL->Enabled = false;
	BBeardL->Enabled = false;
	BClothesL->Enabled = false;
	BGenderR->Enabled = false;
	BClassR->Enabled = false;
	BHairR->Enabled = false;
	BFaceR->Enabled = false;
	BBeardR->Enabled = false;
	BClothesR->Enabled = false;

	CActorsList->Parent = CActorWindow;
	ActorInfo->Parent = CActorWindow;
	BGenderL->Parent = CActorWindow;
	BClassL->Parent = CActorWindow;
	BHairL->Parent = CActorWindow;
	BFaceL->Parent = CActorWindow;
	BBeardL->Parent = CActorWindow;
	BClothesL->Parent = CActorWindow;
	BGenderR->Parent = CActorWindow;
	BClassR->Parent = CActorWindow;
	BHairR->Parent = CActorWindow;
	BFaceR->Parent = CActorWindow;
	BBeardR->Parent = CActorWindow;
	BClothesR->Parent = CActorWindow;

	//L = Manager->CreateLabel(LanguageString[LS_Gender], R(41, 44), R(65, 13));
	//L->Align(TextAlign_Center);
	//L->Parent(CActorWindow);

	//L = Manager->CreateLabel(LanguageString[LS_Class], R(41, 73), R(65, 13));
	//L->Align(TextAlign_Center);
	//L->Parent(CActorWindow);

	//L = Manager->CreateLabel(LanguageString[LS_Hair], R(41, 102), R(65, 13));
	//L->Align(TextAlign_Center);
	//L->Parent(CActorWindow);

	//L = Manager->CreateLabel(LanguageString[LS_Face], R(41, 131), R(65, 13));
	//L->Align(TextAlign_Center);
	//L->Parent(CActorWindow);

	//L = Manager->CreateLabel(LanguageString[LS_Beard], R(41, 160), R(65, 13));
	//L->Align(TextAlign_Center);
	//L->Parent(CActorWindow);

	//L = Manager->CreateLabel(LanguageString[LS_Clothes], R(41, 189), R(65, 13));
	//L->Align(TextAlign_Center);
	//L->Parent(CActorWindow);

	L = Manager->CreateLabel("MainMenu::CharacterCreation::GenderLabel", R(12, 44), R(65, 13));
	L->Text = LanguageString[LS_Gender];
	L->Parent = CActorWindow;

	L = Manager->CreateLabel("MainMenu::CharacterCreation::ClassLabel", R(12, 73), R(65, 13));
	L->Text = LanguageString[LS_Class];
	L->Parent = CActorWindow;

	L = Manager->CreateLabel("MainMenu::CharacterCreation::HairLabel", R(12, 102), R(65, 13));
	L->Text = LanguageString[LS_Hair];
	L->Parent = CActorWindow;

	L = Manager->CreateLabel("MainMenu::CharacterCreation::FaceLabel", R(12, 131), R(65, 13));
	L->Text = LanguageString[LS_Face];
	L->Parent = CActorWindow;

	L = Manager->CreateLabel("MainMenu::CharacterCreation::BeardLabel", R(12, 160), R(65, 13));
	L->Text = LanguageString[LS_Beard];
	L->Parent = CActorWindow;

	L = Manager->CreateLabel("MainMenu::CharacterCreation::ClothesLabel", R(12, 189), R(65, 13));
	L->Text = LanguageString[LS_Clothes];
	L->Parent = CActorWindow;

	RemainingLabel = Manager->CreateLabel("MainMenu::CharacterCreation::RemainingLabel", R(12, 12), R(191, 13));
	RemainingLabel->Text = "";
	RemainingLabel->Align = TextAlign_Center;
	RemainingLabel->Parent = CAttributesWindow;


	BGenderL->Click()->AddEvent(&BGender_Click);
	BClassL->Click()->AddEvent(&BClassL_Click);
	BHairL->Click()->AddEvent(&BHairL_Click);
	BFaceL->Click()->AddEvent(&BFaceL_Click);
	BBeardL->Click()->AddEvent(&BBeardL_Click);
	BClothesL->Click()->AddEvent(&BClothesL_Click);
	BGenderR->Click()->AddEvent(&BGender_Click);
	BClassR->Click()->AddEvent(&BClassR_Click);
	BHairR->Click()->AddEvent(&BHairR_Click);
	BFaceR->Click()->AddEvent(&BFaceR_Click);
	BBeardR->Click()->AddEvent(&BBeardR_Click);
	BClothesR->Click()->AddEvent(&BClothesR_Click);

#pragma endregion

	#undef R

	// Load animation sets
	int Result = LoadAnimSets("Data\\Game Data\\Animations.dat");
	if(Result == -1)
		RuntimeError("Could not open Data\\Game Data\\Animations.dat!");

	// Read whether new account creation is enabled
	F = ReadFile("Data\\Game Data\\Hosts.dat");
	if(F == 0)
		RuntimeError("Could not open Data\\Game Data\\Hosts.dat!");
	ReadLine(F);
	ReadLine(F);
	bool AccountsEnabled = (toInt(ReadLine(F)) > 0) ? true : false;
	LoginCreate->Visible = AccountsEnabled;
	CloseFile(F);

	// Read in last username/password
	F = ReadFile("Data\\Last Username.dat");
	if(F != 0)
	{
		LoginUser->Text = ReadLine(F);
		string TPass = ReadLine(F);
		NGin::CString NPass;
		NPass.Set(TPass.c_str(), TPass.length());
		LoginPassword->Text = Encrypt(NPass, 1).c_str();
		CloseFile(F);
	}


	// Character Set
	if(FileType("Data\\Meshes\\Character Set\\Set.eb3d") == 1)
	{
		Set = LoadMesh("Data\\Meshes\\Character Set\\Set.eb3d");
		if(Set == 0)
			RuntimeError("Could not load Data\\Meshes\\Character Set\\Set.eb3d!");
	}else
	{
		Set = LoadMesh("Data\\Meshes\\Character Set\\Set.b3d");
		if(Set == 0)
			RuntimeError("Could not load Data\\Meshes\\Character Set\\Set.b3d!");
	}
	PositionEntity(Set, -10.5f, -1.75f, -7.25f); // Delete me
	ScaleEntity(Set, 1.5f, 1.5f, 1.5f);         // Delete me too
	//EntityShader(Set, Shader_LitObject1);          // Delete me also (once this is auto-detected)
	EntityShader(Set, LoadShader("Data\\Game Data\\Shaders\\Default\\LitObject_Medium.fx"));          // Delete me also (once this is auto-detected)
	//EntityShadowLevel(Set, 5);
	HideEntity(Set);
	GPP = CreatePivot();
}

void MainMenu::HideAll()
{
	RealmButton->Visible = false;
	OptionsButton->Visible = false;
	ExitGame->Visible = false;
	LoginWindow->Visible = false;
	MessageWindow->Visible = false;

	CSelWindow->Visible = false;
	BStart->Visible = false;
	BDelete->Visible = false;
	BLeft->Visible = false;
	BRight->Visible = false;

	CActorWindow->Visible = false;
	CAttributesWindow->Visible = false;
	TName->Visible = false;
	LName->Visible = false;
	BDone->Visible = false;
	BCancel->Visible = false;
}

void MainMenu::ShowErrorMessage(string Caption, string Message, EventHandler::CBFN* Callback)
{
	MessageWindow->Text = Caption;
	MessageLabel->Text = Message;
	MessageButton->Click()->ClearEvents();
	MessageButton->Click()->AddEvent(&MainMenu::MessageButton_Click);
	MessageButton->Click()->AddEvent(Callback);

	MessageWindow->Closed()->ClearEvents();
	MessageWindow->Closed()->AddEvent(&MainMenu::MessageButton_Click);
	MessageWindow->Closed()->AddEvent(Callback);

	HideAll();
	MessageWindow->Visible = true;
}

void MainMenu::MessageButton_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	Menu->MessageWindow->Visible = false;
}

void MainMenu::RunMenu()
{
#pragma region Menu Setups
	// Server listing
	if(RealmCrafter::Globals->ServerSelector->IsMultiServer())
	{
		RealmButton->Enabled = true;
		RealmButton->Text = RealmCrafter::Globals->ServerSelector->GetServerName();

		RealmCrafter::Globals->ServerHost = RealmCrafter::Globals->ServerSelector->GetServerAddress();
		RealmCrafter::Globals->ServerPort = RealmCrafter::Globals->ServerSelector->GetServerPort();
	}else
	{
		RealmButton->Enabled = false;
		RealmButton->Visible = false;
	}

	// Add closing event
	RealmCrafter::Globals->ServerSelector->Closed()->ClearEvents();
	RealmCrafter::Globals->ServerSelector->Closed()->AddEvent(this, &MainMenu::ServerSelector_Closed);


	// Load background zone
	CurrentArea = "MainMenu_Main";
	if(LoadArea(CurrentArea, Cam, false, false, true, true))
	{
		PositionEntity(Cam, MenuCameraPosition.X, MenuCameraPosition.Y, MenuCameraPosition.Z);
		uint GPPe = CreatePivot();
		PositionEntity(GPPe, MenuCameraLookAt.X, MenuCameraLookAt.Y, MenuCameraLookAt.Z);
		PointEntity(Cam, GPPe);
		FreeEntity(GPPe);
	}else
	{
		PositionEntity(GPP, 1.5f, -1.75f, 5.0f);
		TFormPoint(0.0f, 8.5f, -12.0f, GPP, 0);
		PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
		PointEntity(Cam, GPP);
		MoveEntity(Cam, -2.25f, 0.0f, 0.0f);
	}





#pragma endregion

	MenuOpen = true;

	HadAttributes = false;
	HadDamageTypes = false;
	HadEnvironment = false;
	ActorsCreated = 0;
	ActorsRequired = -1;
	ItemsCreated = 0;
	ItemsRequired = -1;
	FactionsReceived = 0;

	CamAngle = 0.0f;
	PreviewA = 0;
	SelectedChar = -1;

	PointsToSpend = 0;

	Quests = 0;
	RequiredQuests = 1000;
	Spells = 0;
	RequiredSpells = 2000;
	Memorised = 0;
	ItemsDone = 0;
	AttributesDone = 0;

	// Show 'main' page
	HideAll();
	ShowRootMenu();
	LoginCreate->Enabled = false;
	LoginLogin->Enabled = false;
	ConnectToServer();



	//string SlotNames[] = {
	//	"Weapon",
	//	"Shield",
	//	"Hat",
	//	"Chest",
	//	"Hand",
	//	"Belt",
	//	"Legs",
	//	"Feet",
	//	"Ring1",
	//	"Ring2",
	//	"Ring3",
	//	"Ring4",
	//	"Amulet1",
	//	"Amulet2",
	//	"0","1","2","3","4","5","6","7",
	//	"8","9","10","11","12","13","14","15",
	//	"16","17","18","19","20","21","22","23",
	//	"24","25","26","27","28","29","30","31"};

	//IButton* B[46];
	//IWindow* W = GUIManager->CreateWindow("Inventory", Math::Vector2(0, 0), Math::Vector2(0.4, 0.4));

	//for(int i = 0; i < 46; ++i)
	//{
	//	B[i] = GUIManager->CreateButton(string("Inventory::Slot") + SlotNames[i], Math::Vector2(i * 0.009765625f, 0), Math::Vector2(0.009765625f, 0.009765625f));
	//	B[i]->Parent(W);
	//}

	GUIManager->SetProperties("MainMenu");

	//MessageBoxYesNo("TestMSG", "Hello World! I love lamp!", MBI_Warning, 0);


	//ITextBox* T = Manager->CreateTextBox("", Vector2(0.1, 0.1), Vector2(200, 20) / Manager->GetResolution());

	//IButton* B = Manager->CreateButton("Allo", Vector2(0.1, 0.3), Vector2(0.1, 0.1));
	//B->Size(Vector2(0.1, 0.1));
	//B->Align(TextAlign_Right);
	//B->VAlign(TextAlign_Bottom);
	//B->ForeColor(NGin::Math::Color(255, 255, 255));
	//B->Click()->AddEvent(&B_Click);
	//B->MouseLeave()->AddEvent(&B_MouseLeave);
	//B->MouseMove()->AddEvent(&B_MouseMove);

	//IPictureBox* B = Manager->CreatePictureBox("Allo", Vector2(0.1, 0.3), Vector2(0.1, 0.1));
	//*if(!*/B->SetUpImage(WString("Data\\DefaultParticle.bmp"));//)
	//	RuntimeError("NOOO");
	//B->SetHoverImage(WString("Data\\Textures\\Blood.bmp"));
	//B->SetDownImage(WString("Data\\Textures\\Compass.png"));

	uint DeltaTime = 0;
	int DeltaBufferIndex = 0;
// 
// 	std::vector<std::string> lines;
// 	std::vector<Math::Color> colors;
// 
// 	lines.push_back("Name");
// 	colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));
// 
// 	RealmCrafter::CMouseToolTip* MTT = new RealmCrafter::CMouseToolTip(GUIManager, NULL, lines, colors);



	// Main Loop
	while(MenuOpen && !KeyDown(1))
	{

		// Delta timing bits
		DeltaTime = MilliSecs() - DeltaTime;
		DeltaBuffer[DeltaBufferIndex] = DeltaTime;
		DeltaBufferIndex = DeltaBufferIndex + 1;
		if(DeltaBufferIndex >= DeltaFrames)
			DeltaBufferIndex = 0;

		// Take average of last N frames to get delta time coefficient
		float Time = 0.0f;
		for(int i = 0; i < DeltaFrames; ++i)
			Time = Time + DeltaBuffer[i];
		
		Time = Time / ((float)DeltaFrames);
		FPS = 1000.0f / Time;
		Delta = BaseFramerate / FPS;
		DeltaTime = MilliSecs();
		if(Delta > 3.5f)
			Delta = 3.5f; // Don't let delta go too OTT

// 		PositionEntity(GPP, 1.5f, -1.75f, 5.0f);
// 		RotateEntity(GPP, 0.0f, CamAngle, 0.0f);
// 		TFormPoint(0.0f, 8.5f, -12.0f, GPP, 0);
// 		PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
// 		PointEntity(Cam, GPP);
// 		MoveEntity(Cam, -2.25f, 0.0f, 0.0f);
// 
// 
// 		if(CActorWindow->Visible)
// 		{
// 			PositionEntity(GPP, 1.5f, 0.0f, 5.0f);
// 			RotateEntity(GPP, 0.0f, CamAngle, 0.0f);
// 			TFormPoint(-1.5f, 1.5f, -9.0f, GPP, 0);
// 			PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
// 			PointEntity(Cam, GPP);
// 			MoveEntity(Cam, 0.6f, 0.0f, 0.0f);
// 		}

		//DebugLog(string("Camera: ") + toString(EntityX(Cam, true)) + ", " + toString(EntityY(Cam, true)) + ", " + toString(EntityZ(Cam, true)));

		//PositionEntity(Cam, 00, 105, -80);
		//PointEntity(Cam, GPP);


		if(Connection != 0)
		{
			LoginCreate->Enabled = true;
			LoginLogin->Enabled = true;

			// Check connection is still alive
			foreachc(MIt, RCE_Message, RCE_MessageList)
			{
				RCE_Message* M = *MIt;

				if(M->MessageType == RCE_Disconnected || M->MessageType == ConnectionHasLeft)
					RuntimeError(LanguageString[LS_LostConnection]);

				switch(M->MessageType)
				{
				case P_CreateAccount:
					NetCreateAccount(M->MessageData);
					break;
				case P_VerifyAccount:
					NetLoginValidate(M->MessageData);
					break;
				case P_FetchActors:
					NetFetchActors(M->MessageData);
					break;
				case P_DeleteCharacter:
					NetDeleteCharacter(M->MessageData);
					break;
				case P_CreateCharacter:
					NetCreateCharacter(M->MessageData);
					break;
				case P_FetchCharacter:
					NetFetchCharacter(M->MessageData);
					break;
				case ConnectionHasLeft:
				case RCE_Disconnected:
				case RCE_PlayerTimedOut:
				case RCE_TimedOut:
					Connection = 0;
					HideAll();
					ShowErrorMessage("Connection Error...", LanguageString[LS_LostConnection], &MainMenu::ReturnToStartError);
					break;
				case P_ConnectInit:
					{
						char ConnType = M->MessageData.GetRealChar(0);

						// Disconnect?
						if(ConnType == 'N')
						{
							Connection = 0;
							HideAll();
							ShowErrorMessage("Connection Error...", LanguageString[LS_LostConnection], &MainMenu::ReturnToStartError);
							break;
						}else
						{
							printf("Unknown ConnType: %i(%i)\n", ConnType, M->MessageData.Length());
						}

						break;
					}
				default:
					printf("Unhandled message: %i\n", M->MessageType);
					break;
				}


				RCE_Message::Delete(M);

				nextc(MIt, RCE_Message, RCE_MessageList);
			}
			RCE_Message::Clean();

			// Update everything
			RCE_Update();
			RCE_CreateMessages();
		}else
		{
			LoginCreate->Enabled = false;
			LoginLogin->Enabled = false;
		}

		if(PreviewA != NULL)
			PreviewA->UpdateGubbins();

		BBWaitEvent();
		if(MenuOpen)
			Environment->SetTime(12, 0);
		Environment->Update(1.0f);
		SeedRnd(MilliSecs());
		RP_Update(Delta);               // RottParticles
		UpdateAnimatedScenery();        // Animated scenery
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		if(TerrainManager != NULL)
			TerrainManager->Update(NGin::Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true)), true);
		Vector3 TreeUpdatePosition = NGin::Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true));
		if(TreeManager != NULL)
			TreeManager->Update(reinterpret_cast<const float*>(&TreeUpdatePosition));
		UpdateWorld();
		RenderWorld();
		EndDebugFrame();

		Perf->DO_Update();
	}

	if(MenuOpen == true)
		exit(0);

	// Menu completed. hide everything
	HideAll();

	SelectedCharacter = SelectedChar;
	//RCE_Disconnect();
	//Connection = 0;
}

void MainMenu::ShowInGameMenu(){};

void MainMenu::ShowEULAWindow()
{
	EULAWindow->Visible = true;
}

void MainMenu::ShowRootMenu()
{
	RealmButton->Visible = RealmCrafter::Globals->ServerSelector->IsMultiServer();
	OptionsButton->Visible = true;
	ExitGame->Visible = true;
	LoginWindow->Visible = true;
}

void MainMenu::ShowServerSelector()
{

}


void MainMenu::ShowLoginScreen()
{
}

void MainMenu::ConnectToServer()
{
	int Port = 11001;
	char ConOut[1024];
	int Attempts = 0;
	int JoinID = MilliSecs();

ProxyConnect:
	RCE_Message::Clean();

	do
	{
		sprintf(ConOut, "Attempting to connect to: %s:%i; localport: %i; attempts: %i\n", RealmCrafter::Globals->ServerHost.c_str(), RealmCrafter::Globals->ServerPort, Port, Attempts);
		DebugLog(ConOut);

		Connection = RCE_Connect(RealmCrafter::Globals->ServerHost.c_str(), RealmCrafter::Globals->ServerPort, Port, "X", "", "Data\\Logs\\Client Connection.txt", false);	
		++Port;
	}while(Connection == RCE_ConnectionInUse || Connection == RCE_PortInUse);

	string ConnectionError = "";

	switch(Connection)
	{
		case ConnectionNotFound:
			ConnectionError = LanguageString[LS_InvalidHost] + ": " + RealmCrafter::Globals->ServerHost;
			break;
		case RCE_TimedOut:
			ConnectionError = LanguageString[LS_NoResponse];
			break;
		case RCE_ServerFull:
			ConnectionError = LanguageString[LS_TooManyPlayers];
			break;
		default:
			{
				NGin::CString ConnectMsg;
				ConnectMsg.AppendRealInt(JoinID);
				CRCE_Send(Connection, Connection, P_ConnectInit, ConnectMsg, true);

				// 20 seconds is really too much, but its better to be slow and potentially redirected
				unsigned int TimerStart = MilliSecs();
				while(TimerStart + 15000 > MilliSecs())
				{
					// Check connection is still alive
					foreachc(MIt, RCE_Message, RCE_MessageList)
					{
						RCE_Message* M = *MIt;

						if(M->MessageType == RCE_Disconnected || M->MessageType == ConnectionHasLeft)
							RuntimeError(LanguageString[LS_LostConnection]);

						switch(M->MessageType)
						{
						case P_ConnectInit:
							{
								unsigned char AckType = M->MessageData.GetRealChar();
								if(AckType == 'N')
								{
									RCE_Disconnect(Connection);
									Connection = 0;

									unsigned char ErrType = M->MessageData.GetRealChar(1);

									if(ErrType == 'M')
									{
										ShowErrorMessage("Connection Error", "Target cluster has no master.", &MainMenu::ReturnToStartError);
									}else if(ErrType == 'T')
									{
										ShowErrorMessage("Connection Error", "Target cluster timed out (internal).", &MainMenu::ReturnToStartError);
									}else if(ErrType == 'S')
									{
										ShowErrorMessage("Connection Error", "Target cluster is in corrupt state.", &MainMenu::ReturnToStartError);
									}else
									{
										ShowErrorMessage("Connection Error", "Host Connection Denied!", &MainMenu::ReturnToStartError);
									}
									RCE_Message::Delete(M);
									return;
								}else if(AckType == 'Y')
								{
									OutputDebugString("Proxy connection accepted");
									RCE_Message::Delete(M);
									return;
								}else if(AckType == 'R')
								{
									RCE_Disconnect(Connection);
									Connection = 0;

									OutputDebugString("Proxy connection redirected");

									int IP[4] = {0, 0, 0, 0};
									IP[0] = (int)M->MessageData.GetRealChar(1);
									IP[1] = (int)M->MessageData.GetRealChar(2);
									IP[2] = (int)M->MessageData.GetRealChar(3);
									IP[3] = (int)M->MessageData.GetRealChar(4);

									char IPs[24];
									sprintf(IPs, "%i.%i.%i.%i", IP[0], IP[1], IP[2], IP[3]);

									NGin::CString NewHost = IPs;
									unsigned short NewPort = M->MessageData.GetRealShort(5);

									RealmCrafter::Globals->ServerHost = NewHost.c_str();
									RealmCrafter::Globals->ServerPort = NewPort;
									++Attempts;
									RCE_Message::Delete(M);

									if(Attempts == 3)
									{
										ShowErrorMessage("Connection Error", "Received too many Redirects!", &MainMenu::ReturnToStartError);
										return;
									}

									goto ProxyConnect;
								}

								
								
								return;
							}
						case ConnectionHasLeft:
						case RCE_Disconnected:
							{
								Connection = 0;
								HideAll();
								RCE_Message::Delete(M);
								ShowErrorMessage("Connection Error...", LanguageString[LS_LostConnection], &MainMenu::ReturnToStartError);
								return;
							}
						}

						nextc(MIt, RCE_Message, RCE_MessageList);
					}
					RCE_Message::Clean();

					// Update everything
					RCE_Update();
					RCE_CreateMessages();
				}

				ConnectionError = "No proxy response!";
				break;
			}
	}

	Connection = 0;
	ShowErrorMessage("Connection Error", ConnectionError, &MainMenu::ReturnToStartError);
}

void MainMenu::SetupCharacterSelection()
{
	#define R(X, Y) (Vector2(X, Y) / GUIManager->GetResolution())

	for(int i = 0; i < CharButtons.Size(); ++i)
		GUIManager->Destroy(CharButtons[i]);
	CharButtons.Empty();

	// Character buttons (max of 10 characters)
	int Offset = 0;
	int Number = 0;
	int HeightOffset = 12;
	IButton* LastButton = 0;

	while(Offset < CharList.Length())
	{
		// Extract data
		int Length = CharFromStr(CharList.Substr(Offset, 1));
		CharNames[Number] = CharList.Substr(Offset + 1, Length).c_str();
		Offset = Offset + Length + 1;
		CharActors[Number] = ShortFromStr(CharList.Substr(Offset, 2));
		CharGender[Number] = CharFromStr(CharList.Substr(Offset + 2, 1));
		CharFaceTex[Number] = CharFromStr(CharList.Substr(Offset + 3, 1));
		CharHair[Number] = CharFromStr(CharList.Substr(Offset + 4, 1));
		CharBeard[Number] = CharFromStr(CharList.Substr(Offset + 5, 1));
		CharBodyTex[Number] = CharFromStr(CharList.Substr(Offset + 6, 1));

		// Move on
		Offset = Offset + 7;
		Number = Number + 1;

		// Create button
		IButton* Button = Manager->CreateButton(string("MainMenu::CharacterSelection::CharSlot") + toString(Number - 1), R(12, HeightOffset), R(120, 23));
		Button->Text = CharNames[Number - 1];
		Button->Parent = CSelWindow;
		Button->Tag = this;
		Button->Click()->AddEvent(&MainMenu::CharacterSelect_Click);
		CharButtons.Add(Button);
		HeightOffset += 29;
		LastButton = Button;
	}
	int LastChar = Number;

	// New character button
	if(Number < 10)
	{
		HeightOffset += 29;
		IButton* Button = Manager->CreateButton(string("MainMenu::CharacterSelection::CharSlot") + toString(Number + 1), R(12, HeightOffset), R(120, 23));
		Button->Text = LanguageString[LS_NewCharacter];
		Button->Parent = CSelWindow;
		Button->Tag = this;
		Button->Click()->AddEvent(&MainMenu::BCreateChar_Click);
		
		CharButtons.Add(Button);
		LastButton = Button;
	}

	Manager->SetProperties("MainMenu::CharacterSelection");

	HeightOffset = (LastButton->Size.Y * Manager->GetResolution().Y) + (LastButton->Location.Y * Manager->GetResolution().Y);
	HeightOffset += CSelWindow->Size.Y * Manager->GetResolution().Y;
	//HeightOffset += (29 * 2) + 10;
	Vector2 Size = CSelWindow->Size;
	CSelWindow->Size = Vector2(Size.X, (1.0f / Manager->GetResolution().Y) * HeightOffset);

	SelectedChar = -1;
	PositionEntity(GPP, 30, -35, 100); // DR

	#undef R
}

void MainMenu::SetupCharacterCreation()
{
	ActorInfo->Text = "";
	TName->Text = "";

	//CActorWindow->Size = Math::Vector2(217, 270) / GUIManager->GetResolution();

	for(int i = 0; i < AttributesLeftSelect.Size(); ++i)
		GUIManager->Destroy(AttributesLeftSelect[i]);
	for(int i = 0; i < AttributesRightSelect.Size(); ++i)
		GUIManager->Destroy(AttributesRightSelect[i]);
	for(int i = 0; i < AttrName.Size(); ++i)
		GUIManager->Destroy(AttrName[i]);
	for(int i = 0; i < AttrValue.Size(); ++i)
		GUIManager->Destroy(AttrValue[i]);

	AttributesLeftSelect.Empty();
	AttributesRightSelect.Empty();
	AttrName.Empty();
	AttrValue.Empty();

	while(CActorsList->ItemCount() > 0)
		CActorsList->DeleteItem(0);

	// Clear point spends
	for(int i = 0; i <= 39; ++i)
		PointSpends[i] = 0;
	PointsToSpend = 0;

	// Race List
	foreachc(AIt, Actor, ActorList)
	{
		Actor* A = *AIt;

		if(A->Playable)
		{
			// Check every previous actor to make sure this race hasn't already been added
			bool AlreadyAdded = false;
			foreachc(A2It, Actor, ActorList)
			{
				Actor* A2 = *A2It;

				if(A2 == A)
					break;

				if(A2->Playable)
				{
					if(stringToUpper(A2->Race).compare(stringToUpper(A->Race)) == 0)
					{
						AlreadyAdded = true;
						break;
					}
				}

				nextc(A2It, Actor, ActorList);
			}

			if(AlreadyAdded == false)
			{
				DebugLog(A->Race);
				CActorsList->AddItem(A->Race);
			}
		}

		

		nextc(AIt, Actor, ActorList);
	}

	int Count = 0;
	int BHeight = 36, LHeight = 41;
	int HeightOffset = 12;
	IButton* LastButton = 0;

	for(int i = 0; i <= 39; ++i)
	{
		if(RealmCrafter::Globals->Attributes->Name[i].length() > 0
			&& RealmCrafter::Globals->Attributes->IsSkill[i] == false
			&& RealmCrafter::Globals->Attributes->Hidden[i] == false)
		{
			ILabel* AName = Manager->CreateLabel(string("MainMenu::CharacterCreation::Slot") + toString(Count) + string("NameLabel"), Vector2(12, LHeight) / Manager->GetResolution(), Vector2(1,1));
			ILabel* AVal = Manager->CreateLabel(string("MainMenu::CharacterCreation::Slot") + toString(Count) + string("ValueLabel"), Vector2(132, LHeight) / Manager->GetResolution(), Vector2(15,1));

			IButton* ALeft = Manager->CreateButton(string("MainMenu::CharacterCreation::Slot") + toString(Count) + string("MinusButton"), Vector2(103, BHeight) / Manager->GetResolution(), Vector2(23, 23) / Manager->GetResolution());
			IButton* ARight = Manager->CreateButton(string("MainMenu::CharacterCreation::Slot") + toString(Count) + string("PlusButton"), Vector2(163, BHeight) / Manager->GetResolution(), Vector2(23, 23) / Manager->GetResolution());

			AName->Text = RealmCrafter::Globals->Attributes->Name[i] + string(":");
			AVal->Text = string("000");
			ALeft->Text = "-";
			ARight->Text = "+";

			int* ip = (int*)i;
			AVal->Tag = ip;

			ALeft->Tag = this;
			ARight->Tag = this;

			ALeft->Click()->AddEvent(&MainMenu::AttrDecrease_Click);
			ARight->Click()->AddEvent(&MainMenu::AttrIncrease_Click);

			AName->Parent = CAttributesWindow;
			AVal->Parent = CAttributesWindow;
			ALeft->Parent = CAttributesWindow;
			ARight->Parent = CAttributesWindow;

			AttrName.Add(AName);
			AttrValue.Add(AVal);
			AttributesLeftSelect.Add(ALeft);
			AttributesRightSelect.Add(ARight);

			BHeight += 30;
			LHeight += 30;
			LastButton = ALeft;
			++Count;
		}
	}
//ILabel* T = Manager->CreateLabel(string("temp"), Vector2(12, 0) / Manager->GetResolution(), Vector2(0,0));
	

	Manager->SetProperties("MainMenu::CharacterCreation");
	
	//char OO[1024];
	//sprintf(OO, "%s\n", CActorWindow->Size.ToString().c_str());
	//OutputDebugString(OO);


	if(LastButton != NULL)
	{
		HeightOffset = (LastButton->Size.Y * Manager->GetResolution().Y) + (LastButton->Location.Y * Manager->GetResolution().Y);
		HeightOffset += CAttributesWindow->Size.Y * Manager->GetResolution().Y;
		//HeightOffset += (29 * 2) + 10;
		Vector2 Size = CAttributesWindow->Size;
		CAttributesWindow->Size = Vector2(Size.X, (1.0f / Manager->GetResolution().Y) * HeightOffset);
		//CAttributesWindow->Size(Vector2(CAttributesWindow->Size().X, (BHeight + 30) / Manager->GetResolution().Y));
	}

	CActorsList->SelectedIndex = 0;
	CActorsList_SelectedIndexChanged(CActorsList, 0);
}


void MainMenu::AttrIncrease_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	int Index = -1;

	for(int i = 0; i < Menu->AttributesRightSelect.Size(); ++i)
		if(Menu->AttributesRightSelect[i] == Sender)
			Index = i;

	int subIndex = 0;
	for(int i = 0; i <= 39; ++i)
	{
		if(RealmCrafter::Globals->Attributes->Name[i].length() > 0
			&& RealmCrafter::Globals->Attributes->IsSkill[i] == false
			&& RealmCrafter::Globals->Attributes->Hidden[i] == false)
		{
			if(subIndex == Index)
			{
				Index = subIndex;
				break;
			}

			++subIndex;
		}
	}

	if(Index == -1)
		OutputDebugString("Index is -1 at: MainMenu::AttrIncrease_Click(IControl* Sender, EventArgs* E)\n");

	if(Menu->PointsToSpend > 0 && (Menu->PreviewA->Attributes->Value[Index] + Menu->PointSpends[Index]) < Menu->PreviewA->Attributes->Maximum[Index])
	{
		--Menu->PointsToSpend;
		++Menu->PointSpends[Index];
		Menu->AttrValue[Index]->Text = toString(Menu->PreviewA->Attributes->Value[Index] + Menu->PointSpends[Index]);
		Menu->RemainingLabel->Text = LanguageString[LS_AttributePoints] + " " + toString(Menu->PointsToSpend);
	}
}

void MainMenu::AttrDecrease_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	int Index = -1;

	for(int i = 0; i < Menu->AttributesLeftSelect.Size(); ++i)
		if(Menu->AttributesLeftSelect[i] == Sender)
			Index = i;

	int subIndex = 0;
	for(int i = 0; i <= 39; ++i)
	{
		if(RealmCrafter::Globals->Attributes->Name[i].length() > 0
			&& RealmCrafter::Globals->Attributes->IsSkill[i] == false
			&& RealmCrafter::Globals->Attributes->Hidden[i] == false)
		{
			if(subIndex == Index)
			{
				Index = subIndex;
				break;
			}

			++subIndex;
		}
	}

	if(Index == -1)
		OutputDebugString("Index is -1 at: MainMenu::AttrDecrease_Click(IControl* Sender, EventArgs* E)\n");

	if(Menu->PointSpends[Index] > 0)
	{
		++Menu->PointsToSpend;
		--Menu->PointSpends[Index];
		Menu->AttrValue[Index]->Text = toString(Menu->PreviewA->Attributes->Value[Index] + Menu->PointSpends[Index]);
		Menu->RemainingLabel->Text = LanguageString[LS_AttributePoints] + string(" ") + toString(Menu->PointsToSpend);
	}
}

void MainMenu::ShowCharacterCreation()
{
	CActorWindow->Visible = true;
	CAttributesWindow->Visible = true;
	TName->Visible = true;
	LName->Visible = true;
	BDone->Visible = true;
	BCancel->Visible = true;
	BLeft->Visible = true;
	BRight->Visible = true;

	BLeft->Location = Vector2(0.506f, 0.936f);
	BLeft->Size = Vector2(0.043f, 0.058f);
	BRight->Location = Vector2(0.556f, 0.936f);
	BRight->Size = Vector2(0.043f, 0.058f);
}

void MainMenu::CActorsList_SelectedIndexChanged(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	if(Menu->PreviewA != 0)
		SafeFreeActorInstance(Menu->PreviewA);
	Menu->PreviewA = 0;

	Menu->PointsToSpend = AttributeAssignment;
	Menu->RemainingLabel->Text = LanguageString[LS_AttributePoints] + string(" ") + toString(Menu->PointsToSpend);

	for(int i = 0; i <= 39; ++i)
		Menu->PointSpends[i] = 0;

	string ChosenRace = stringToUpper(Menu->CActorsList->SelectedValue);
	Actor* Chosen = 0;

	foreachc(AIt, Actor, ActorList)
	{
		Actor* A = *AIt;

		if(stringToUpper(A->Race).compare(ChosenRace) == 0)
		{
			Chosen = A;
			break;
		}

		nextc(AIt, Actor, ActorList);
	}

	if(Chosen == 0)
		RuntimeError(string("Chosen actor ") + ChosenRace + " not found!");

	UnloadArea();
	HideEntity(Menu->Set);
	Menu->CurrentArea = string("MainMenu_") + Chosen->Race;
	
	if(!LoadArea(Menu->CurrentArea, Cam, false, false, false, true))
	{
		Loaded = false;
		ShowEntity(Menu->Set);
		Environment->LoadArea("none", "none");

		PositionEntity(Menu->GPP, 1.5f, -1.75f, 5.0f);
		TFormPoint(0.0f, 8.5f, -12.0f, Menu->GPP, 0);
		PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
		PointEntity(Cam, Menu->GPP);
		MoveEntity(Cam, -2.25f, 0.0f, 0.0f);
	}else
	{
		Loaded = true;
		PositionEntity(Cam, MenuCameraPosition.X, MenuCameraPosition.Y, MenuCameraPosition.Z);
		uint GPPe = CreatePivot();
		PositionEntity(GPPe, MenuCameraLookAt.X, MenuCameraLookAt.Y, MenuCameraLookAt.Z);
		PointEntity(Cam, GPPe);
		FreeEntity(GPPe);
	}

	Menu->PreviewA = CreateActorInstance(Chosen);
	bool Result = LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
	if(!Result)
		RuntimeError(string("Could not load actor mesh for ") + Chosen->Race + string("!"));
	PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);

	if(Loaded)
		PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
	else
		PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);
	if(Menu->PreviewA->NametagEN != 0)
		Menu->PreviewA->NametagEN->Visible = false;
	if(Menu->PreviewA->TagEN != 0)
		Menu->PreviewA->TagEN->Visible = false;

	PointEntity(Menu->PreviewA->CollisionEN, Cam);
	RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);

	// Attributes
	int Count = 0;
	for(int i = 0; i < ((Menu->AttrValue.Size() < 40) ? Menu->AttrValue.Size() : 40); ++i)
	{
		if(RealmCrafter::Globals->Attributes->Name[i].length() > 0
			&& RealmCrafter::Globals->Attributes->IsSkill[i] == false
			&& RealmCrafter::Globals->Attributes->Hidden[i] == false)
		{
			Menu->AttrValue[Count]->Text = toString(Menu->PreviewA->Attributes->Value[i] + Menu->PointSpends[i]);
			++Count;
		}
	}

	float AISize = Menu->ActorInfo->Size.Y;
	static bool fReset = false;
	//float AISizeOffset = Menu->CActorWindow->Size.Y - 

	//uint T = MilliSecs();
	Menu->Manager->SetSingleProperty("MainMenu::CharacterCreation", Menu->CActorWindow->Name, 2);
	
	//uint Len = MilliSecs() - T;
	//DebugLog(string("Apply Took: ") + string(Len) + "ms");
	//DebugLog(string("") + Menu->CActorWindow->Size.Y + ", " + Menu->ActorInfo->Location.X);


	//if(Menu->CActorWindow->Size.Y >= Menu->ActorInfo->Location.X)
	//{
		//float TSize = 
			//Menu->CActorWindow->Size = Vector2(Menu->CActorWindow->Size.X, Menu->CActorWindow->Size.Y - AISize);
		//fReset = true;
	//}

	Menu->ActorInfo->Text = Menu->PreviewA->Actor->Description;

	IControl* HighestControl = 0;
	foreachf(CIt, IControl, (*Menu->CActorWindow->Controls()))
	{
		if(HighestControl == 0)
			HighestControl = *CIt;

		if((*CIt)->Size.Y + (*CIt)->Location.Y > HighestControl->Size.Y + HighestControl->Location.Y)
			HighestControl = *CIt;

		nextf(CIt, IControl, (*Menu->CActorWindow->Controls()));
	}

	
	int HeightOffset = (HighestControl->Size.Y * Menu->Manager->GetResolution().Y) + (HighestControl->Location.Y * Menu->Manager->GetResolution().Y);
	HeightOffset += Menu->CActorWindow->Size.Y * Menu->Manager->GetResolution().Y;
	Vector2 Size = Menu->CActorWindow->Size;
	Menu->CActorWindow->Size = Vector2(Size.X, (1.0f / Menu->Manager->GetResolution().Y) * HeightOffset);

	//if(fReset)
	//	Menu->CActorWindow->Size = Vector2(Size.X, Size.Y + (Menu->ActorInfo->Size.Y - AISize));
	//else
	//	fReset = true;


	Menu->BGenderL->Enabled = Menu->PreviewA->Actor->Genders == 0;
	Menu->BClassL->Enabled = MultipleClasses(Menu->PreviewA->Actor);
	Menu->BHairL->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
	Menu->BFaceL->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
	Menu->BBeardL->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
	Menu->BClothesL->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);
	Menu->BGenderR->Enabled = Menu->PreviewA->Actor->Genders == 0;
	Menu->BClassR->Enabled = MultipleClasses(Menu->PreviewA->Actor);
	Menu->BHairR->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
	Menu->BFaceR->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
	Menu->BBeardR->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
	Menu->BClothesR->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);
}

void MainMenu::BDone_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	if(Menu->TName->Text.length() < 2)
	{
		Menu->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidCharName], &MainMenu::ReturnToCreationError);
		return;
	}

	// Send character information to server
	NGin::CString Pa = "";
	Pa.AppendRealChar(UName.length());
	Pa.Append(UName.c_str());
	Pa.AppendRealChar(PWord.length());
	Pa.Append(PWord.c_str());
	
	Pa.AppendRealShort(Menu->PreviewA->Actor->ID);
	Pa.AppendRealChar(Menu->PreviewA->Gender);
	Pa.AppendRealChar(Menu->PreviewA->FaceTex);
	Pa.AppendRealChar(Menu->PreviewA->Hair);
	Pa.AppendRealChar(Menu->PreviewA->Beard);     
	Pa.AppendRealChar(Menu->PreviewA->BodyTex);
	//if(AttributeAssignment > 0) - found out this will crash the server as it will send a wrong length packet to the server
		for(int i = 0; i <= 39; ++i)
		{
			int l = Pa.Length();
			Pa.AppendRealChar(Menu->PointSpends[i]);

			if(Pa.Length() == l) // if now changed
				printf("%i", i);

		}

	Pa.Append(Menu->TName->Text.c_str());

	CRCE_Send(Connection, Connection, P_CreateCharacter, Pa, true);
}

void MainMenu::BStart_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	// Check first
	if(Menu->SelectedChar == -1)
		return;

	// Send start
	NGin::CString NUName = UName.c_str();
	NGin::CString NPWord = PWord.c_str();
	NGin::CString Pa = NGin::CString::FormatReal("cscsc", NUName.Length(), &NUName, NPWord.Length(), &NPWord, Menu->SelectedChar);
	CRCE_Send(Connection, Connection, P_FetchCharacter, Pa, true);

	// Stuff I already know
	Me = CreateActorInstance(ActorList[CharActors[Menu->SelectedChar]]);
	Me->Name = CharNames[Menu->SelectedChar];
	Me->Gender = CharGender[Menu->SelectedChar];
	Me->FaceTex = CharFaceTex[Menu->SelectedChar];
	Me->Hair = CharHair[Menu->SelectedChar];
	Me->Beard = CharBeard[Menu->SelectedChar];
	Me->BodyTex = CharBodyTex[Menu->SelectedChar];

	// Hide everything
	Menu->HideAll();
	HideEntity(Menu->Set);
	if(Menu->PreviewA != 0)
		SafeFreeActorInstance(Menu->PreviewA);
	Menu->PreviewA = 0;


}

void MainMenu::NetFetchCharacter(NGin::CString& Message)
{
	int Offset = 0;

	// Character information
	if(Message.Substr(0, 1) == NGin::CString("C"))
	{
		// Block 1
		if(Message.Substr(1, 1) == NGin::CString("1"))
		{
			Me->Gold = IntFromStr(Message.Substr(32, 4));
			Me->Reputation = ShortFromStr(Message.Substr(6, 2));
			Me->Level = ShortFromStr(Message.Substr(8, 2));
			Me->XP = IntFromStr(Message.Substr(10, 4));
			Me->HomeFaction = CharFromStr(Message.Substr(14, 1));
			Offset = 15;
			while(Offset < Message.Length())
			{
				Me->Attributes->Value[AttributesDone] = ShortFromStr(Message.Substr(Offset, 2));
				Me->Attributes->Maximum[AttributesDone] = ShortFromStr(Message.Substr(Offset + 2, 2));
				++AttributesDone;
				Offset = Offset + 4;
			}
		// Block 2 <No longer exists, reserved for future use>
//							Elseif(Mid$(Message$, 2, 1) = "2"

		// Block 3
		}else if(Message.Substr(1, 1) == NGin::CString("3"))
		{
			Offset = 2;
			while(Offset < Message.Length())
			{
				int Position = CharFromStr(Message.Substr(Offset, 1));
				if(Position < 50)
				{
					NGin::CString Item = Message.Substr(Offset + 1, ItemInstanceStringLength());
					Me->Inventory->Items[Position] = ItemInstanceFromString(Item);
					Offset = Offset + 1 + ItemInstanceStringLength();
					Me->Inventory->Amounts[Position] = ShortFromStr(Message.Substr(Offset, 2));
					Offset = Offset + 2;
				}else
					++Offset;
				
				++ItemsDone;
			}
		}
	}else if(Message.Substr(0, 1) == NGin::CString("S")) // A known spells block
	{
		Offset = 1;
		while(Offset < Message.Length())
		{
			Me->SpellLevels[Spells] = ShortFromStr(Message.Substr(Offset, 2));
			Spell* Sp = new Spell();
			Sp->ID = ShortFromStr(Message.Substr(Offset + 2, 2));
			SpellsList[Sp->ID] = Sp;
			Me->KnownSpells[Spells] = Sp->ID;
			Sp->ThumbnailTexID = ShortFromStr(Message.Substr(Offset + 4, 2));
			Sp->RechargeTime = ShortFromStr(Message.Substr(Offset + 6, 2));
			Offset = Offset + 8;
			int NameLen = ShortFromStr(Message.Substr(Offset, 2));
			if (NameLen<0) NameLen = 0;
			Sp->Name = Message.Substr(Offset + 2, NameLen).c_str();
			Offset = Offset + 2 + NameLen;
			NameLen = ShortFromStr(Message.Substr(Offset, 2));
			if (NameLen<0) NameLen = 0;
			Sp->Description = Message.Substr(Offset + 2, NameLen).c_str();
			Offset = Offset + 2 + NameLen;
			if(CharFromStr(Message.Substr(Offset, 1)) > 0 && Memorised < 10)
			{
				Me->MemorisedSpells[Memorised] = Spells;
				++Memorised;
			}
			++Offset;
			++Spells;
		}
	}else if(Message.Substr(0, 1) == NGin::CString("Q")) // A quest log block
	{
		Offset = 1;
		while(Offset < Message.Length())
		{
			int NameLen = CharFromStr(Message.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			MyQuestLog->EntryName[Quests] = Message.Substr(Offset + 1, NameLen).c_str();
			Offset = Offset + 1 + NameLen;
			NameLen = ShortFromStr(Message.Substr(Offset, 2));
			if (NameLen<0) NameLen = 0;

			NGin::String paA = Message.Substr(Offset + 2, NameLen);
			MyQuestLog->EntryStatus[Quests] = std::string(paA.c_str(), paA.Length());

			Offset = Offset + 2 + NameLen;
			++Quests;
		}
	}else // Final block
	{
		RequiredQuests = ShortFromStr(Message.Substr(1, 2));
		RequiredSpells = ShortFromStr(Message.Substr(3, 2));
	}

	// Complete!
	if(Quests >= RequiredQuests && Spells >= RequiredSpells && AttributesDone > 39 && ItemsDone == 50)
	{
		// Start the game
		MenuOpen = false;

		// Unregister GUI render till the zone is loading
		SetRenderGUICallback(0, 0);
	}
}

void MainMenu::NetCreateCharacter(NGin::CString& Message)
{
	if(Message == NGin::CString("Y"))
	{
		CharList.AppendRealChar(TName->Text.length());
		CharList.Append(TName->Text.c_str());
		CharList.AppendRealShort(PreviewA->Actor->ID);
		CharList.AppendRealChar(PreviewA->Gender);
		CharList.AppendRealChar(PreviewA->FaceTex);
		CharList.AppendRealChar(PreviewA->Hair);
		CharList.AppendRealChar(PreviewA->Beard);
		CharList.AppendRealChar(PreviewA->BodyTex);

		if(PreviewA != 0)
			SafeFreeActorInstance(PreviewA);
		PreviewA = 0;

		HideAll();
		SetupCharacterSelection();
		ShowCharacterSelection();

	}else if(Message == NGin::CString("I"))
	{
		this->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidCharName], &MainMenu::ReturnToCreationError);
	}else
	{
		if(PreviewA != 0)
			SafeFreeActorInstance(PreviewA);
		PreviewA = 0;

		HideAll();
		this->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_CannotCreateChar], &MainMenu::ReturnToSelectionError);
	}
}

void MainMenu::BCancel_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	if(Menu->PreviewA != 0)
		SafeFreeActorInstance(Menu->PreviewA);
	Menu->PreviewA = 0;

	Menu->HideAll();
	Menu->SetupCharacterSelection();
	Menu->ShowCharacterSelection();
}

void MainMenu::BClassL_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	char Gender = Menu->PreviewA->Gender;
	Actor* A = Menu->PreviewA->Actor;
	
	while(true)
	{
		A = Actor::ActorList.Before(A);
		if(A == 0)
			A = *Actor::ActorList.End();
		if(stringToUpper(A->Race).compare(stringToUpper(Menu->PreviewA->Actor->Race)) == 0 && A->Playable == true)
		{
			SafeFreeActorInstance(Menu->PreviewA);
			Menu->PreviewA = 0;

			Menu->PreviewA = CreateActorInstance(A);
			if((Gender == 0 && A->Genders != 2) || (Gender == 1 && A->Genders != 1 && A->Genders != 3))
				Menu->PreviewA->Gender = Gender;
			
			bool Result = LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
			if(Result == false)
				RuntimeError(string("Could not load actor mesh for ") + A->Race + string("!"));

			PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);
			if(Loaded)
				PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
			else
				PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);

			PointEntity(Menu->PreviewA->CollisionEN, Cam);
			RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);

			if(Menu->PreviewA->NametagEN != 0)
				Menu->PreviewA->NametagEN->Visible = false;
			if(Menu->PreviewA->TagEN != 0)
				Menu->PreviewA->TagEN->Visible = false;
			
			Menu->PointsToSpend = AttributeAssignment;
			for(int i = 0; i <= 39; ++i)
				Menu->PointSpends[i] = 0;

			// Attributes
			int Count = 0;
			for(int i = 0; i <= 39; ++i)
			{
				if(RealmCrafter::Globals->Attributes->Name[i].length() > 0
					&& RealmCrafter::Globals->Attributes->IsSkill[i] == false
					&& RealmCrafter::Globals->Attributes->Hidden[i] == false)
				{
					Menu->AttrValue[Count]->Text = toString(Menu->PreviewA->Attributes->Value[i] + Menu->PointSpends[i]);
					++Count;
				}
			}
												  
			Menu->ActorInfo->Text = Menu->PreviewA->Actor->Description;
			Menu->CActorWindow->Size = Vector2(Menu->CActorWindow->Size.X, Menu->ActorInfo->Size.Y + Menu->ActorInfo->Location.Y + (Menu->BBeardL->Size.Y * 2.0f));

			Menu->BGenderL->Enabled = Menu->PreviewA->Actor->Genders == 0;
			Menu->BClassL->Enabled = MultipleClasses(Menu->PreviewA->Actor);
			Menu->BHairL->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BFaceL->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BBeardL->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
			Menu->BClothesL->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);
			Menu->BGenderR->Enabled = Menu->PreviewA->Actor->Genders == 0;
			Menu->BClassR->Enabled = MultipleClasses(Menu->PreviewA->Actor);
			Menu->BHairR->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BFaceR->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BBeardR->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
			Menu->BClothesR->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);
			break;
		}
	}
}

void MainMenu::BClassR_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	char Gender = Menu->PreviewA->Gender;
	Actor* A = Menu->PreviewA->Actor;
	
	while(true)
	{
		A = Actor::ActorList.After(A);
		if(A == 0)
			A = *Actor::ActorList.Begin();
		if(stringToUpper(A->Race).compare(stringToUpper(Menu->PreviewA->Actor->Race)) == 0 && A->Playable == true)
		{
			SafeFreeActorInstance(Menu->PreviewA);
			Menu->PreviewA = 0;

			Menu->PreviewA = CreateActorInstance(A);
			if((Gender == 0 && A->Genders != 2) || (Gender == 1 && A->Genders != 1 && A->Genders != 3))
				Menu->PreviewA->Gender = Gender;
			
			bool Result = LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
			if(Result == false)
				RuntimeError(string("Could not load actor mesh for ") + A->Race + string("!"));

			PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);
			if(Loaded)
				PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
			else
				PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);
			if(Menu->PreviewA->NametagEN != 0)
				Menu->PreviewA->NametagEN->Visible = false;
			if(Menu->PreviewA->TagEN != 0)
				Menu->PreviewA->TagEN->Visible = false;

			PointEntity(Menu->PreviewA->CollisionEN, Cam);
			RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);
			
			Menu->PointsToSpend = AttributeAssignment;
			for(int i = 0; i <= 39; ++i)
				Menu->PointSpends[i] = 0;

			// Attributes
			char OO[1024];
			sprintf(OO, "ATN: %i\n", Menu->AttrValue.Size());
			OutputDebugString(OO);

			int Count = 0;
			for(int i = 0; i <= 39; ++i)
			{
				if(RealmCrafter::Globals->Attributes->Name[i].length() > 0
					&& RealmCrafter::Globals->Attributes->IsSkill[i] == false
					&& RealmCrafter::Globals->Attributes->Hidden[i] == false)
				{
					Menu->AttrValue[Count]->Text = toString(Menu->PreviewA->Attributes->Value[i] + Menu->PointSpends[i]);
					++Count;
				}
			}
												  
			Menu->ActorInfo->Text = Menu->PreviewA->Actor->Description;
			Menu->CActorWindow->Size = Vector2(Menu->CActorWindow->Size.X, Menu->ActorInfo->Size.Y + Menu->ActorInfo->Location.Y + (Menu->BBeardL->Size.Y * 2.0f));

			Menu->BGenderL->Enabled = Menu->PreviewA->Actor->Genders == 0;
			Menu->BClassL->Enabled = MultipleClasses(Menu->PreviewA->Actor);
			Menu->BHairL->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BFaceL->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BBeardL->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
			Menu->BClothesL->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);
			Menu->BGenderR->Enabled = Menu->PreviewA->Actor->Genders == 0;
			Menu->BClassR->Enabled = MultipleClasses(Menu->PreviewA->Actor);
			Menu->BHairR->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BFaceR->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
			Menu->BBeardR->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
			Menu->BClothesR->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);
			break;
		}
	}
}

void MainMenu::BClothesL_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	ActorTextureSet NextTex(65535, 65535, 65535, 65535);
	int IterProtection = 0;
	do
	{
		--Menu->PreviewA->BodyTex;
		if(Menu->PreviewA->BodyTex < 0)
		{
			if(Menu->PreviewA->Gender == 0)
				Menu->PreviewA->BodyTex = Menu->PreviewA->Actor->MaleBodyIDs.size() - 1;
			else
				Menu->PreviewA->BodyTex = Menu->PreviewA->Actor->FemaleBodyIDs.size() - 1;
		}

		if(Menu->PreviewA->Gender == 0)
			NextTex = Menu->PreviewA->Actor->MaleBodyIDs[Menu->PreviewA->BodyTex];
		else
			NextTex = Menu->PreviewA->Actor->FemaleBodyIDs[Menu->PreviewA->BodyTex];

		++IterProtection;
		if(IterProtection > 300)
			RuntimeError(string("Infinate Loop Detected at:\n") + __FUNCSIG__ + ":" + toString(__LINE__));
	}while(NextTex.Tex0 == 65535 || NextTex.Tex0 == 0 || NextTex.Tex0 == -1);

	FreeActorInstance3D(Menu->PreviewA);
	LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
	PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);
	if(Loaded)
		PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
	else
		PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);
	if(Menu->PreviewA->NametagEN != 0)
		Menu->PreviewA->NametagEN->Visible = false;
	if(Menu->PreviewA->TagEN != 0)
		Menu->PreviewA->TagEN->Visible = false;

	PointEntity(Menu->PreviewA->CollisionEN, Cam);
	RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);
}

void MainMenu::BClothesR_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	ActorTextureSet NextTex(65535, 65535, 65535, 65535);
	int IterProtection = 0;
	do
	{
		++Menu->PreviewA->BodyTex;
		if(Menu->PreviewA->Gender == 0)
		{
			if(Menu->PreviewA->BodyTex >= Menu->PreviewA->Actor->MaleBodyIDs.size())
				Menu->PreviewA->BodyTex = 0;
		}else
		{
			if(Menu->PreviewA->BodyTex >= Menu->PreviewA->Actor->FemaleBodyIDs.size())
				Menu->PreviewA->BodyTex = 0;
		}

		if(Menu->PreviewA->Gender == 0)
			NextTex = Menu->PreviewA->Actor->MaleBodyIDs[Menu->PreviewA->BodyTex];
		else
			NextTex = Menu->PreviewA->Actor->FemaleBodyIDs[Menu->PreviewA->BodyTex];

		++IterProtection;
		if(IterProtection > 300)
			RuntimeError(string("Infinate Loop Detected at:\n") + __FUNCSIG__ + ":" + toString(__LINE__));
	}while(NextTex.Tex0 == 65535 || NextTex.Tex0 == 0 || NextTex.Tex0 == -1);

	FreeActorInstance3D(Menu->PreviewA);
	LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
	PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);
	if(Loaded)
		PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
	else
		PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);
	if(Menu->PreviewA->NametagEN != 0)
		Menu->PreviewA->NametagEN->Visible = false;
	if(Menu->PreviewA->TagEN != 0)
		Menu->PreviewA->TagEN->Visible = false;

	PointEntity(Menu->PreviewA->CollisionEN, Cam);
	RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);
}

void MainMenu::BFaceL_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	ActorTextureSet NextTex(65535, 65535, 65535, 65535);
	int IterProtection = 0;
	do
	{
		--Menu->PreviewA->FaceTex;
		if(Menu->PreviewA->FaceTex < 0)
		{
			if(Menu->PreviewA->Gender == 0)
				Menu->PreviewA->FaceTex = Menu->PreviewA->Actor->MaleFaceIDs.size() - 1;
			else
				Menu->PreviewA->FaceTex = Menu->PreviewA->Actor->FemaleFaceIDs.size() - 1;
		}

		if(Menu->PreviewA->Gender == 0)
			NextTex = Menu->PreviewA->Actor->MaleFaceIDs[Menu->PreviewA->FaceTex];
		else
			NextTex = Menu->PreviewA->Actor->FemaleFaceIDs[Menu->PreviewA->FaceTex];

		++IterProtection;
		if(IterProtection > 300)
			RuntimeError(string("Infinate Loop Detected at:\n") + __FUNCSIG__ + ":" + toString(__LINE__));
	}while(NextTex.Tex0 == 65535 || NextTex.Tex0 == 0 || NextTex.Tex0 == -1);

	FreeActorInstance3D(Menu->PreviewA);
	LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
	PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);
	if(Loaded)
		PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
	else
		PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);
	if(Menu->PreviewA->NametagEN != 0)
		Menu->PreviewA->NametagEN->Visible = false;
	if(Menu->PreviewA->TagEN != 0)
		Menu->PreviewA->TagEN->Visible = false;

	PointEntity(Menu->PreviewA->CollisionEN, Cam);
	RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);
}

void MainMenu::BFaceR_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	ActorTextureSet NextTex(65535, 65535, 65535, 65535);
	int IterProtection = 0;
	do
	{
		++Menu->PreviewA->FaceTex;
		if(Menu->PreviewA->Gender == 0)
		{
			if(Menu->PreviewA->FaceTex >= Menu->PreviewA->Actor->MaleFaceIDs.size())
				Menu->PreviewA->FaceTex = 0;
		}else
		{
			if(Menu->PreviewA->FaceTex >= Menu->PreviewA->Actor->FemaleFaceIDs.size())
				Menu->PreviewA->FaceTex = 0;
		}

		if(Menu->PreviewA->Gender == 0)
			NextTex = Menu->PreviewA->Actor->MaleFaceIDs[Menu->PreviewA->FaceTex];
		else
			NextTex = Menu->PreviewA->Actor->FemaleFaceIDs[Menu->PreviewA->FaceTex];

		++IterProtection;
		if(IterProtection > 300)
			RuntimeError(string("Infinate Loop Detected at:\n") + __FUNCSIG__ + ":" + toString(__LINE__));
	}while(NextTex.Tex0 == 65535 || NextTex.Tex0 == 0 || NextTex.Tex0 == -1);

	FreeActorInstance3D(Menu->PreviewA);
	LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
	PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);
	if(Loaded)
		PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
	else
		PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);
	if(Menu->PreviewA->NametagEN != 0)
		Menu->PreviewA->NametagEN->Visible = false;
	if(Menu->PreviewA->TagEN != 0)
		Menu->PreviewA->TagEN->Visible = false;

	PointEntity(Menu->PreviewA->CollisionEN, Cam);
	RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);
}

void MainMenu::BBeardL_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	
	--Menu->PreviewA->Beard;
	if(Menu->PreviewA->Beard < 0)
		Menu->PreviewA->Beard = Menu->PreviewA->Actor->Beards.size() - 1;

	Menu->PreviewA->ShowGubbinSet(&(Menu->PreviewA->Actor->Beards[Menu->PreviewA->Beard]), Menu->PreviewA->BeardENs);

		
}

void MainMenu::BBeardR_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	++Menu->PreviewA->Beard;
	if(Menu->PreviewA->Beard >= Menu->PreviewA->Actor->Beards.size())
		Menu->PreviewA->Beard = 0;

	Menu->PreviewA->ShowGubbinSet(&(Menu->PreviewA->Actor->Beards[Menu->PreviewA->Beard]), Menu->PreviewA->BeardENs);
}

void MainMenu::BHairL_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	--Menu->PreviewA->Hair;
	if(Menu->PreviewA->Gender == 0)
	{
		if(Menu->PreviewA->Hair < 0)
			Menu->PreviewA->Hair = Menu->PreviewA->Actor->MaleHairs.size() - 1;

		SetActorHat(Menu->PreviewA, NULL);
	}else
	{
		if(Menu->PreviewA->Hair < 0)
			Menu->PreviewA->Hair = Menu->PreviewA->Actor->FemaleHairs.size() - 1;

		SetActorHat(Menu->PreviewA, NULL);
	}
}

void MainMenu::BHairR_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	++Menu->PreviewA->Hair;
	if(Menu->PreviewA->Gender == 0)
	{
		if(Menu->PreviewA->Hair >= Menu->PreviewA->Actor->MaleHairs.size())
			Menu->PreviewA->Hair = 0;

		SetActorHat(Menu->PreviewA, NULL);
	}else
	{
		if(Menu->PreviewA->Hair >= Menu->PreviewA->Actor->FemaleHairs.size())
			Menu->PreviewA->Hair = 0;

		SetActorHat(Menu->PreviewA, NULL);
	}
}

void MainMenu::BGender_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	if(Menu->PreviewA->Actor->Genders == 0)
	{
		Menu->PreviewA->Gender = !Menu->PreviewA->Gender;
		Menu->PreviewA->BodyTex = 0;
		FreeActorInstance3D(Menu->PreviewA);
		LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
		PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);
		if(Loaded)
			PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
		else
			PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);
		if(Menu->PreviewA->NametagEN != 0)
			Menu->PreviewA->NametagEN->Visible = false;
		if(Menu->PreviewA->TagEN != 0)
			Menu->PreviewA->TagEN->Visible = false;

		Menu->BGenderL->Enabled = Menu->PreviewA->Actor->Genders == 0;
		Menu->BClassL->Enabled = MultipleClasses(Menu->PreviewA->Actor);
		Menu->BHairL->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
		Menu->BFaceL->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
		Menu->BBeardL->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
		Menu->BClothesL->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);
		Menu->BGenderR->Enabled = Menu->PreviewA->Actor->Genders == 0;
		Menu->BClassR->Enabled = MultipleClasses(Menu->PreviewA->Actor);
		Menu->BHairR->Enabled = ActorHasHair(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
		Menu->BFaceR->Enabled = ActorHasFace(Menu->PreviewA->Actor, Menu->PreviewA->Gender + 1);
		Menu->BBeardR->Enabled = (Menu->PreviewA->Gender == 1) ? false : ActorHasBeard(Menu->PreviewA->Actor);
		Menu->BClothesR->Enabled = ActorHasMultipleTextures(Menu->PreviewA->Actor, Menu->PreviewA->Gender);

		PointEntity(Menu->PreviewA->CollisionEN, Cam);
		RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);
	}
}
void MainMenu::CharacterSelect_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	int LastSelectedChar = Menu->SelectedChar;
	Menu->SelectedChar = -1;
	for(int i = 0; i < Menu->CharButtons.Size(); ++i)
		if(Menu->CharButtons[i] == Sender)
			Menu->SelectedChar = i;

	if(LastSelectedChar == Menu->SelectedChar)
		return;

	if(Menu->SelectedChar == -1)
		return;

	// Setup Actor
	Actor* A = ActorList[CharActors[Menu->SelectedChar]];
	if(Menu->PreviewA != 0)
		SafeFreeActorInstance(Menu->PreviewA);
	Menu->PreviewA = 0;

	char DBO[1024];
	sprintf(DBO, "Del: A: %x; CharActors: %i; Sel: %i;\n", A, CharActors[Menu->SelectedChar], Menu->SelectedChar);
	OutputDebugStringA(DBO);

	UnloadArea();
	HideEntity(Menu->Set);
	Menu->CurrentArea = string("MainMenu_") + A->Race;
	if(!LoadArea(Menu->CurrentArea, Cam, false, false, false, true))
	{
		Loaded = false;
		ShowEntity(Menu->Set);
		Environment->LoadArea("none", "none");

		PositionEntity(Menu->GPP, 1.5f, -1.75f, 5.0f);
		TFormPoint(0.0f, 8.5f, -12.0f, Menu->GPP, 0);
		PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
		PointEntity(Cam, Menu->GPP);
		MoveEntity(Cam, -2.25f, 0.0f, 0.0f);
	}else
	{
		Loaded = true;
		PositionEntity(Cam, MenuCameraPosition.X, MenuCameraPosition.Y, MenuCameraPosition.Z);
		uint GPPe = CreatePivot();
		PositionEntity(GPPe, MenuCameraLookAt.X, MenuCameraLookAt.Y, MenuCameraLookAt.Z);
		PointEntity(Cam, GPPe);
		FreeEntity(GPPe);
	}

	Menu->PreviewA = CreateActorInstance(A);
	Menu->PreviewA->Gender = CharGender[Menu->SelectedChar];
	Menu->PreviewA->FaceTex = CharFaceTex[Menu->SelectedChar];
	Menu->PreviewA->Hair = CharHair[Menu->SelectedChar];
	Menu->PreviewA->Beard = CharBeard[Menu->SelectedChar];
	Menu->PreviewA->BodyTex = CharBodyTex[Menu->SelectedChar];


	bool Result = LoadActorInstance3D(Menu->PreviewA, 0.05f, false, true);
	if(Result == false)
		RuntimeError(string("Could not load actor mesh for ") + A->Race + string("!"));
	if(Menu->PreviewA->NametagEN != 0)
		Menu->PreviewA->NametagEN->Visible = false;
	if(Menu->PreviewA->TagEN != 0)
		Menu->PreviewA->TagEN->Visible = false;
	PlayAnimation(Menu->PreviewA, 1, 0.003f, Anim_Idle);

	if(Loaded)
		PositionEntity(Menu->PreviewA->CollisionEN, MenuActorSpawn.X, MenuActorSpawn.Y, MenuActorSpawn.Z);
	else
		PositionEntity(Menu->PreviewA->CollisionEN, 1.5f, -(1.75f + EntityY(Menu->PreviewA->EN, true)), 5.0f);

	PointEntity(Menu->PreviewA->CollisionEN, Cam);
	RotateEntity(Menu->PreviewA->CollisionEN, 0, EntityYaw(Menu->PreviewA->CollisionEN) + 180.0f, 0);
}

void MainMenu::ShowCharacterSelection()
{
	HideAll();

	CSelWindow->Visible = true;
	BStart->Visible = true;
	BDelete->Visible = true;
	BLeft->Visible = true;
	BRight->Visible = true;
	//ShowEntity(Set);

	BLeft->Location = Vector2(0.396f, 0.896f);
	BLeft->Size = Vector2(0.043f, 0.058f);
	BRight->Location = Vector2(0.446f, 0.896f);
	BRight->Size = Vector2(0.043f, 0.058f);
}

void MainMenu::ReturnToStartError(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	Menu->ShowRootMenu();
}

void MainMenu::ReturnToLoginError(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	Menu->RealmButton->Visible = RealmCrafter::Globals->ServerSelector->IsMultiServer();
	Menu->OptionsButton->Visible = true;
	Menu->ExitGame->Visible = true;
	Menu->LoginWindow->Visible = true;
}

void MainMenu::ReturnToCreationError(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	Menu->ShowCharacterCreation();
}

void MainMenu::ReturnToSelectionError(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	Menu->SetupCharacterSelection();
	Menu->ShowCharacterSelection();
}


void MainMenu::OptionsButton_Click(IControl* Sender, EventArgs* E)
{
// 	// Get menu instance
// 	MainMenu* Menu = (MainMenu*)Sender->Tag;
// 
// 	Menu->HideAll();
// 	Menu->GraphicsMenu->Visible = true;
	OptionsMenu->Show();
}

void MainMenu::ExitGame_Click(IControl* Sender, EventArgs* E)
{
	exit(0);
}

void MainMenu::SelectorWindow_Close(IControl* Sender, EventArgs* E)
{
// 	// Get menu instance
// 	MainMenu* Menu = (MainMenu*)Sender->Tag;
// 
// 	Menu->ShowRootMenu();
// 	Menu->SelectorWindow->Visible = false;
}

void MainMenu::SelectorConnect_Click(IControl* Sender, EventArgs* E)
{
// 	// Get menu instance
// 	MainMenu* Menu = (MainMenu*)Sender->Tag;
// 
// 	for(int i = 0; i < Menu->Servers.Size(); ++i)
// 		if(Menu->Servers[i]->Name.compare(Menu->SelectorList->SelectedValue) == 0)
// 			RealmCrafter::Globals->ServerHost = Menu->Servers[i]->Host;
// 
// 	if(RealmCrafter::Globals->ServerHost.find(":", 0) != -1)
// 	{
// 		int Colon = RealmCrafter::Globals->ServerHost.find(":", 0);
// 
// 		RealmCrafter::Globals->ServerPort = toInt(RealmCrafter::Globals->ServerHost.substr(Colon + 1));
// 		RealmCrafter::Globals->ServerHost = RealmCrafter::Globals->ServerHost.substr(0, Colon);
// 	}else
// 		RealmCrafter::Globals->ServerPort = 25000;
// 
// 	Menu->SelectorWindow->Visible = false;
// 	if(AlwaysShowEULA > 0 || (EULAAccepted == 0 && Menu->EULALabel->Text.length() > 1))
// 		Menu->ShowEULAWindow();
// 	else
// 		Menu->ShowLoginScreen();
}

void MainMenu::ServerSelector_Closed(IControl* sender, EventArgs* e)
{
	RealmButton->Text = RealmCrafter::Globals->ServerSelector->GetServerName();
	RealmButton->Enabled = true;
	RealmButton->Visible = true;

	// Don't reconnect if we don't need to
	if(RealmCrafter::Globals->ServerHost.compare(RealmCrafter::Globals->ServerSelector->GetServerAddress()) == 0
		&& RealmCrafter::Globals->ServerPort == RealmCrafter::Globals->ServerSelector->GetServerPort())	
		return;

	// Reconnect
	RealmCrafter::Globals->ServerHost = RealmCrafter::Globals->ServerSelector->GetServerAddress();
	RealmCrafter::Globals->ServerPort = RealmCrafter::Globals->ServerSelector->GetServerPort();

	if(Connection != 0)
	{
		RCE_Disconnect(Connection);
		Connection = 0;
	}

	ConnectToServer();
}

void MainMenu::RealmButton_Click(IControl* sender, EventArgs* e)
{
	RealmCrafter::Globals->ServerSelector->Show();
}


void MainMenu::LoginCreate_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	// Send new account message
	string Name = Menu->LoginUser->Text;
	string Pass = Menu->LoginPassword->Text;
	string Email = Menu->LoginEmail->Text;

	// Error check
	if(Name.length() < 2)
	{
		Menu->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidUsername], &MainMenu::ReturnToLoginError);
		return;
	}
	if(Pass.length() < 2)
	{
		Menu->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidPassword], &MainMenu::ReturnToLoginError);
		return;
	}
	if(Email.length() < 5 || Email.find("@", 0) == -1 || Email.find(".", 0) == -1)
	{
		Menu->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidEmailAddress], &MainMenu::ReturnToLoginError);
		return;
	}

	// Build packet
	NGin::CString MD5Pass = MD5(Pass).c_str();
	NGin::CString EncMail = Email.c_str();
	NGin::CString NName = Name.c_str();
	NGin::CString Pa = NGin::CString::FormatReal("cscscs", NName.Length(), &NName, MD5Pass.Length(), &MD5Pass, EncMail.Length(), &EncMail);;

	// Send
	CRCE_Send(Connection, Connection, P_CreateAccount, Pa, true);

	// Hide the login window
	Menu->LoginWindow->Visible = false;
}

void MainMenu::LoginWindow_Close(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	RCE_Disconnect(Connection);
	Connection = 0;

	Menu->ShowRootMenu();
}
void MainMenu::NetCreateAccount(NGin::CString &Message)
{
	if(Message.GetRealChar(0) == 'Y')
		ShowErrorMessage(LanguageString[LS_Success], LanguageString[LS_NewAccountCreated], &MainMenu::ReturnToLoginError);
	else
		ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_UsernameAlreadyExists], &MainMenu::ReturnToLoginError);
}

void MainMenu::LoginLogin_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	Menu->HideAll();

	if(AlwaysShowEULA > 0 || (EULAAccepted == 0 && Menu->EULALabel->Text.length() > 1))
	{
		Menu->ShowEULAWindow();
		return;
	}

	// Fire off verification message
	string Name = Menu->LoginUser->Text;
	string Pass = Menu->LoginPassword->Text;

	if(Name.length() < 2)
	{
		Menu->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidUsername], &MainMenu::ReturnToLoginError);
		return;
	}
	if(Pass.length() < 2)
	{
		Menu->ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidPassword], &MainMenu::ReturnToLoginError);
		return;
	}

	NGin::CString MD5Pass = MD5(Pass).c_str();
	NGin::CString NName = Name.c_str();
	NGin::CString Pa = NGin::CString::FormatReal("cscs", NName.Length(), &NName, MD5Pass.Length(), &MD5Pass);
	CRCE_Send(Connection, Connection, P_VerifyAccount, Pa, true);

	// Hide the login window
	Menu->LoginWindow->Visible = false;
}

void MainMenu::NetLoginValidate(NGin::CString &Message)
{
	if(Message.Substr(0, 1) == NGin::CString("N"))
		ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_AccountDoesNotExist], &MainMenu::ReturnToLoginError);
	else if(Message.Substr(0, 1) == NGin::CString("P"))
		ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_InvalidPassword], &MainMenu::ReturnToLoginError);
	else if(Message.Substr(0, 1) == NGin::CString("B"))
		ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_YouAreBanned], &MainMenu::ReturnToLoginError);
	else if(Message.Substr(0, 1) == NGin::CString("L"))
		ShowErrorMessage(LanguageString[LS_Error], LanguageString[LS_AlreadyInGame], &MainMenu::ReturnToLoginError);
	else
	{
		CharList = Message.Substr(1);

		// Save username/password
		FILE* F = WriteFile("Data\\Last Username.dat");
		WriteLine(F, LoginUser->Text);
		WriteLine(F, Encrypt(LoginPassword->Text.c_str(), -1).c_str());
		CloseFile(F);	
		
		UName = LoginUser->Text;
		PWord = MD5(LoginPassword->Text);

		// Request actors list
		CRCE_Send(Connection, Connection, P_FetchActors, NGin::CString(""), true);
	}
}

void MainMenu::NetFetchActors(NGin::CString& Message)
{
	DebugLog("Msg");
	NGin::CString Pa = Message;
	
	NGin::CString Sub = Pa.Substr(0, 1);
	NGin::CString Sub2 = NGin::CString("A");
	if(Sub == Sub2)
		if(HadAttributes)
			HadAttributes = true;

	if(Pa[0] == 'A') // Attributes block
	{
		AttributeAssignment = CharFromStr(Pa.Substr(1, 1));
		int Offset = 2;
		for(int i = 0; i < 40; ++i)
		{
			RealmCrafter::Globals->Attributes->IsSkill[i] = CharFromStr(Pa.Substr(Offset, 1));
			RealmCrafter::Globals->Attributes->Hidden[i] = CharFromStr(Pa.Substr(Offset + 1, 1));
			int NameLen = CharFromStr(Pa.Substr(Offset + 2, 1));
			if (NameLen<0) NameLen = 0;
			RealmCrafter::Globals->Attributes->Name[i] = Pa.Substr(Offset + 3, NameLen).c_str();
			Offset = Offset + 3 + NameLen;
		}
		HadAttributes = true;
		DebugLog("Recv. Attributes");
//						RCE_Message::Delete(M);
	}else if(Pa[0] == 'D') // Damage types block
	{
		int Offset = 1;
		for(int i = 0; i <= 19; ++i)
		{
			int NameLen = CharFromStr(Pa.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			DamageTypes[i] = Pa.Substr(Offset + 1, NameLen).c_str();
			Offset = Offset + 1 + NameLen;
		}
		HadDamageTypes = true;
		DebugLog("Recv. Damage Types");
//						RCE_Message::Delete(M);
	}else if(Pa[0] == 'E') // Environment block
	{
		//Environment->ResetEnvironment(Pa);
		HadEnvironment = true;
		DebugLog("Recv. Environment");
//						RCE_Message::Delete(M);
	}else if(Pa[0] == 'F') // Factions block
	{
		int Offset = 1;
		while(Offset < Pa.Length())
		{
			int NameLen = CharFromStr(Pa.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			int Num = CharFromStr(Pa.Substr(Offset + 1, 1));
			FactionNames[Num] = Pa.Substr(Offset + 2, NameLen).c_str();
			FactionsReceived = FactionsReceived + 1;
			Offset = Offset + 2 + NameLen;
		}
		DebugLog("Recv. Factions");
//						RCE_Message::Delete(M);
	}else if(Pa[0] == 'I' && HadAttributes == true) // Item block
	{
		int Offset = 0;
		// if(final item block, we get an extra handy total number of items
		if(Pa[1] == 'Y')
		{
			
			ItemsRequired = ShortFromStr(Pa.Substr(2, 2));
			Offset = 4;
		}else
		{
			Offset = 2;
		}
		
		while(Offset < Pa.Length())
		{
			++ItemsCreated;
			Item* It = new Item();
			It->Attributes = new Attributes();
			It->ID = ShortFromStr(Pa.Substr(Offset, 2));
			ItemList[It->ID] = It;
			It->ItemType = CharFromStr(Pa.Substr(Offset + 2, 1));
			It->TakesDamage = CharFromStr(Pa.Substr(Offset + 3, 1));
			It->Value = IntFromStr(Pa.Substr(Offset + 4, 4));
			It->Mass = ShortFromStr(Pa.Substr(Offset + 8, 2));
			It->ThumbnailTexID = ShortFromStr(Pa.Substr(Offset + 10, 2));
			Offset = Offset + 12;

			int GubbinCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
			++Offset;
			for(int j = 0; j < GubbinCount; ++j)
			{
				unsigned short ID = ShortFromStr(Pa.Substr(Offset, 2));
				Offset += 2;

				GubbinTemplate* T = GubbinTemplate::FromID(ID);
				if(T != NULL)
					It->GubbinTemplates.push_back(T);
			}

			It->SlotType = ShortFromStr(Pa.Substr(Offset, 2));
			It->Stackable = CharFromStr(Pa.Substr(Offset + 2, 1));
			Offset = Offset + 3;
			for(int j = 0; j <= 39; ++j)
			{
				It->Attributes->Value[j] = ShortFromStr(Pa.Substr(Offset, 2)) - 5000;
				Offset = Offset + 2;
			}
			int NameLen = CharFromStr(Pa.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			It->Name = Pa.Substr(Offset + 1, NameLen).c_str();
			Offset = Offset + 1 + NameLen;
			NameLen = CharFromStr(Pa.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			It->ExclusiveRace = Pa.Substr(Offset + 1, NameLen).c_str();
			Offset = Offset + 1 + NameLen;
			NameLen = CharFromStr(Pa.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			It->ExclusiveClass = Pa.Substr(Offset + 1, NameLen).c_str();
			Offset = Offset + 1 + NameLen;
			switch(It->ItemType)
			{
			case I_Weapon:
				{
					It->WeaponDamage = ShortFromStr(Pa.Substr(Offset, 2));
					It->WeaponDamageType = ShortFromStr(Pa.Substr(Offset + 2, 2));
					It->WeaponType = ShortFromStr(Pa.Substr(Offset + 4, 2));
					It->Range = FloatFromStr(Pa.Substr(Offset + 6, 4));
					Offset = Offset + 10;
					break;
				}
			case I_Armour:
				{
					It->ArmourLevel = ShortFromStr(Pa.Substr(Offset, 2));
					Offset = Offset + 2;
					break;
				}
			case I_Potion:
				{
					It->EatEffectsLength = ShortFromStr(Pa.Substr(Offset, 2));
					Offset = Offset + 2;
					break;
				}
			case I_Ingredient:
				{
					It->EatEffectsLength = ShortFromStr(Pa.Substr(Offset, 2));
					Offset = Offset + 2;
					break;
				}
			case I_Image:
				{
					It->ImageID = ShortFromStr(Pa.Substr(Offset, 2));
					Offset = Offset + 2;
					break;
				}
			case I_Other:
				{
					NameLen = CharFromStr(Pa.Substr(Offset, 1));
					if (NameLen<0) NameLen = 0;
					It->MiscData = Pa.Substr(Offset + 1, NameLen).c_str();
					Offset = Offset + 1 + NameLen;
					break;
				}
			}
		}
		DebugLog("Recv. Items");
//						RCE_Message::Delete(M);
	}else if(HadAttributes == true) // Actor block
	{
		int Offset = 0;
		// if final actor block, we get an extra handy total number of actors
		if(Pa[0] == 'Y')
		{
			ActorsRequired = ShortFromStr(Pa.Substr(1, 2));
			Offset = 3;
		}else
			Offset = 1;
		
		while(Offset < Pa.Length())
		{
			++ActorsCreated;
			Actor* A = new Actor();
			A->Attributes = new Attributes();
			A->ID = ShortFromStr(Pa.Substr(Offset, 2));
			ActorList[A->ID] = A;
			A->Playable = CharFromStr(Pa.Substr(Offset + 2, 1));
			A->PolyCollision = CharFromStr(Pa.Substr(Offset + 3, 1));
			Offset += 4;

			A->MaleMesh = ShortFromStr(Pa.Substr(Offset + (0 * 2), 2));
			A->FemaleMesh = ShortFromStr(Pa.Substr(Offset + (1 * 2), 2));
			Offset += 4;

			int GubbinCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
			Offset += 1;
			for(int i = 0; i < GubbinCount; ++i)
			{
				unsigned short ID = ShortFromStr(Pa.Substr(Offset, 2));
				Offset += 2;

				GubbinTemplate* T = GubbinTemplate::FromID(ID);

				if(T != NULL)
					A->Gubbins.push_back(T);
			}

			int SetCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
			Offset += 1;
			for(int bid = 0; bid < SetCount; ++bid)
			{
				std::vector<GubbinTemplate*> Li;
				GubbinCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
				Offset += 1;


				for(int i = 0; i < GubbinCount; ++i)
				{
					unsigned short ID = ShortFromStr(Pa.Substr(Offset, 2));
					Offset += 2;

					GubbinTemplate* T = GubbinTemplate::FromID(ID);

					if(T != NULL)
						Li.push_back(T);
				}

				A->Beards.push_back(Li);
			}

			SetCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
			Offset += 1;
			for(int bid = 0; bid < SetCount; ++bid)
			{
				std::vector<GubbinTemplate*> Li;
				GubbinCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
				Offset += 1;


				for(int i = 0; i < GubbinCount; ++i)
				{
					unsigned short ID = ShortFromStr(Pa.Substr(Offset, 2));
					Offset += 2;

					GubbinTemplate* T = GubbinTemplate::FromID(ID);

					if(T != NULL)
						Li.push_back(T);
				}

				A->MaleHairs.push_back(Li);
			}

			SetCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
			Offset += 1;
			for(int bid = 0; bid < SetCount; ++bid)
			{
				std::vector<GubbinTemplate*> Li;
				GubbinCount = (unsigned char)CharFromStr(Pa.Substr(Offset, 1));
				Offset += 1;


				for(int i = 0; i < GubbinCount; ++i)
				{
					unsigned short ID = ShortFromStr(Pa.Substr(Offset, 2));
					Offset += 2;

					GubbinTemplate* T = GubbinTemplate::FromID(ID);

					if(T != NULL)
						Li.push_back(T);
				}

				A->FemaleHairs.push_back(Li);
			}
			
			int Cnt = CharFromStr(Pa.Substr(Offset++, 1));
			for(int i = 0; i < Cnt; ++i)
				A->MaleFaceIDs.push_back(ActorTextureSet(
					ShortFromStr(Pa.Substr(Offset + (i * 8) + 0, 2)),
					ShortFromStr(Pa.Substr(Offset + (i * 8) + 2, 2)),
					ShortFromStr(Pa.Substr(Offset + (i * 8) + 4, 2)),
					ShortFromStr(Pa.Substr(Offset + (i * 8) + 6, 2))));
			Offset += Cnt * 8;
			
			Cnt = CharFromStr(Pa.Substr(Offset++, 1));
			for(int i = 0; i < Cnt; ++i)
				A->FemaleFaceIDs.push_back(ActorTextureSet(
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 0, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 2, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 4, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 6, 2))));
			Offset += Cnt * 8;
			
			Cnt = CharFromStr(Pa.Substr(Offset++, 1));
			for(int i = 0; i < Cnt; ++i)
				A->MaleBodyIDs.push_back(ActorTextureSet(
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 0, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 2, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 4, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 6, 2))));
			Offset += Cnt * 8;
			
			Cnt = CharFromStr(Pa.Substr(Offset++, 1));
			for(int i = 0; i < Cnt; ++i)
				A->FemaleBodyIDs.push_back(ActorTextureSet(
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 0, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 2, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 4, 2)),
				ShortFromStr(Pa.Substr(Offset + (i * 8) + 6, 2))));
			Offset += Cnt * 8;
			
			for(int i = 0; i <= 15; ++i)
				A->MSpeechIDs[i] = ShortFromStr(Pa.Substr(Offset + (i * 2), 2));
			Offset += 32;
			
			for(int i = 0; i <= 15; ++i)
				A->FSpeechIDs[i] = ShortFromStr(Pa.Substr(Offset + (i * 2), 2));
			Offset += 32;
			
			A->Rideable       = CharFromStr(Pa.Substr(Offset, 1));
			A->TradeMode      = CharFromStr(Pa.Substr(Offset + 1, 1));
			A->BloodTexID     = ShortFromStr(Pa.Substr(Offset + 2, 2));
			A->Aggressiveness = CharFromStr(Pa.Substr(Offset + 4, 1));
			A->Genders        = CharFromStr(Pa.Substr(Offset + 5, 1));
			A->Environment    = CharFromStr(Pa.Substr(Offset + 6, 1));
			A->InventorySlots = ShortFromStr(Pa.Substr(Offset + 7, 2));
			A->MAnimationSet  = ShortFromStr(Pa.Substr(Offset + 9, 2));
			A->FAnimationSet  = ShortFromStr(Pa.Substr(Offset + 11, 2));
			A->Scale          = FloatFromStr(Pa.Substr(Offset + 13, 4));
			A->DefaultFaction = CharFromStr(Pa.Substr(Offset + 17, 1));
			Offset = Offset + 18;
			for(int i = 0; i <= 39; ++i)
			{
				A->Attributes->Value[i] = ShortFromStr(Pa.Substr(Offset, 2));
				A->Attributes->Maximum[i] = ShortFromStr(Pa.Substr(Offset + 2, 2));
				Offset = Offset + 4;
			}

			int NameLen = CharFromStr(Pa.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			A->Race = Pa.Substr(Offset + 1, NameLen).c_str();
			Offset = Offset + 1 + NameLen;
			NameLen = CharFromStr(Pa.Substr(Offset, 1));
			if (NameLen<0) NameLen = 0;
			A->Class = Pa.Substr(Offset + 1, NameLen).c_str();
			Offset = Offset + 1 + NameLen;
			NameLen = ShortFromStr(Pa.Substr(Offset, 2));
			if (NameLen<0) NameLen = 0;
			A->Description = Pa.Substr(Offset + 2, NameLen).c_str();
			Offset = Offset + 2 + NameLen;
		}
		DebugLog("Recv. Final");
//						RCE_Message::Delete(M);
	}
	
	// if we have all the required actors and items, we can continue
	if(ActorsCreated == ActorsRequired && ItemsCreated == ItemsRequired)
	{
		if(HadDamageTypes == true && HadEnvironment == true && HadAttributes == true && FactionsReceived == 100)
		{
//							RCE_Message::Delete(M);
			SetupCharacterSelection();
			ShowCharacterSelection();
		}
	}
}

void MainMenu::NetDeleteCharacter(NGin::CString& Message)
{
	CharList = Message;
	
	if(PreviewA != 0)
		SafeFreeActorInstance(PreviewA);
	PreviewA = 0;

	SetupCharacterSelection();
	ShowCharacterSelection();
}

void MainMenu::BLeft_MouseDown(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	//Menu->CamAngle -= 1.5f * Delta;
	if(Menu->PreviewA != NULL && Menu->PreviewA->CollisionEN != NULL)
		TurnEntity(Menu->PreviewA->CollisionEN, 0, -1.5f * Delta, 0);
}

void MainMenu::BRight_MouseDown(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;
	
	//Menu->CamAngle += 1.5f * Delta;
	if(Menu->PreviewA != NULL && Menu->PreviewA->CollisionEN != NULL)
		TurnEntity(Menu->PreviewA->CollisionEN, 0, 1.5f * Delta, 0);
}

void MainMenu::BDelete_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	if(Menu->SelectedChar == -1)
		return;

	MessageBoxYesNo("Confirm Delete", "Are you sure you want to delete this actor?", MBI_Warning, &MainMenu::BDeleteConfirm, 0);
}

void MainMenu::BDeleteConfirm(IControl* Sender, EventArgs* E)
{
	
	NGin::CString NUName = UName.c_str();
	NGin::CString NPWord = PWord.c_str();
	NGin::CString Pa = NGin::CString::FormatReal("cscsc", NUName.Length(), &NUName, NPWord.Length(), &NPWord, MenuInstance->SelectedChar);
	CRCE_Send(Connection, Connection, P_DeleteCharacter, Pa, true);
}


void MainMenu::BCreateChar_Click(IControl* Sender, EventArgs* E)
{
	// Get menu instance
	MainMenu* Menu = (MainMenu*)Sender->Tag;

	Menu->HideAll();

	if(Menu->PreviewA != 0)
		SafeFreeActorInstance(Menu->PreviewA);
	Menu->PreviewA = 0;

	Menu->SetupCharacterCreation();
	Menu->ShowCharacterCreation();
}

void MainMenu::EULAScroll_Scroll(IControl* sender, ScrollEventArgs* e)
{
	EULALabel->ScrollOffset = NGin::Math::Vector2(0, -((IScrollBar*)sender)->Value) / GUIManager->GetResolution();
}

void MainMenu::EULAAccept_Click(IControl* sender, EventArgs* e)
{
	EULAAccepted = 1;
	EULAWindow->Visible = false;
	OptionsMenu->Save();
	LoginLogin_Click(sender, e);
}

void MainMenu::EULACancel_Click(IControl* sender, EventArgs* e)
{
	EULAAccepted = 0;
	EULAWindow->Visible = false;
	ShowRootMenu();
}



void RunMenu()
{
	if(MenuInstance == NULL)
		MenuInstance = new MainMenu(GUIManager);
	
	MenuInstance->RunMenu();
}


