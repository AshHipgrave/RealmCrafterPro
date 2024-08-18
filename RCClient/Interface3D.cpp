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

#include "Interface3D.h"

// Stupid window definitions
#ifdef CreateWindow
#undef CreateWindow
#endif

int KnownSpellSort[1000];

bool EnableClickToMove = false;

bool BChatDown = false;
bool BMapDown = false;
bool BInventoryDown = false;
bool BSpellsDown = false;
bool BCharStatsDown = false;
bool BQuestLogDown = false;
bool BPartyDown = false;
bool BHelpDown = false;



void TextLines_MouseEnter(IControl* Sender, EventArgs* E)
{
	Dialog* D = (Dialog*)Sender->Tag;

	int Opt = -1;
	for(int i = 0; i < 14; ++i)
	{
		if(D->TextLines[i] == Sender && D->OptionNum[i] > 0)
		{
			Opt = D->OptionNum[i];
			break;
		}
	}

	if(Opt == -1)
		return;


	for(int i = 0; i < 14; ++i)
	{
		if(D->OptionNum[i] == Opt)
		{
			D->TextLines[i]->Text = D->TextText[i];
			D->TextLines[i]->ForeColor = Math::Color(0, 125, 255);
		}
	}
}


void TextLines_MouseLeave(IControl* Sender, EventArgs* E)
{
	Dialog* D = (Dialog*)Sender->Tag;

	for(int i = 0; i < 14; ++i)
	{
		if(D->TextLines[i] == Sender)
		{
			D->TextLines[i]->Text = D->TextText[i];
			D->TextLines[i]->ForeColor = Math::Color(D->TextR[i], D->TextG[i], D->TextB[i]);
		}
	}
}

void TextLines_Click(IControl* Sender, EventArgs* E)
{
	Dialog* D = (Dialog*)Sender->Tag;

	int Opt = -1;
	for(int i = 0; i < 14; ++i)
	{
		if(D->TextLines[i] == Sender && D->OptionNum[i] > 0)
		{
			Opt = D->OptionNum[i];
			break;
		}
	}

	if(Opt == -1)
		return;


	// Option selected!
	if(Opt > 0)
	{
		PlaySound(GY_SClick);
		NGin::CString Pa = NGin::CString::FormatReal("cic", 'O', D->ScriptHandle, Opt);
		CRCE_Send(Connection, Connection, P_Dialog, Pa, true);
		for(int i = 0; i < 14; ++i)
			D->OptionNum[i] = 0;
		D->TotalOptions = 0;
	}
}

void PlayerParty_Closed(IControl* Sender, EventArgs* E)
{
	BPartyDown = !PartyVisible;
	PlaySound(GY_SBeep);
}

void PlayerParty_Click(IControl* sender, EventArgs* e)
{
	if(sender == NULL || sender->Text.length() == 0)
		return;

	if(PlayerParty->IsActiveWindow())
	{
		ChatEntry->Visible = true;
		GUIManager->ControlFocus(ChatEntry);
		BChatDown = true;
		ChatEntry->Text = string("/p ") + sender->Text + string(" ");
	}
}

void PlayerParty_Leave(IControl* Sender, EventArgs* E)
{
	if(PartyVisible == true)
		CRCE_Send(Connection, Connection, P_ChatMessage, NGin::CString("/leave"), true);
}

void PlayerQuestLog_Closed(IControl* Sender, EventArgs* E)
{
	BQuestLogDown = !QuestLogVisible;
	PlaySound(GY_SBeep);
}

void BChat_Click(IControl* Sender, EventArgs* E)
{
	BChatDown = !BChatDown;


	bool OthersVisible = PlayerInventory->GetVisible() || CharStatsVisible || QuestLogVisible || SpellsVisible || (TextInput::TextInputList.Count() > 0);

	// Show chat
	if(ChatEntry->Visible == false && OthersVisible == false)
	{
		ChatEntry->Visible = true;
		GUIManager->ControlFocus(ChatEntry);
		BChatDown = true;
	}else // Cancel chat
	{
		ChatEntry->Visible = false;
		ChatEntry->Text = "";
		BChatDown = false;
	}
}

void BMap_Click(IControl* Sender, EventArgs* E)
{
	BMapDown = !BMapDown;
}

void BInventory_Click(IControl* Sender, EventArgs* E)
{
	BInventoryDown = !BInventoryDown;
}

void BSpells_Click(IControl* Sender, EventArgs* E)
{
	BSpellsDown = !BSpellsDown;
}

void BCharStats_Click(IControl* Sender, EventArgs* E)
{
	BCharStatsDown = !BCharStatsDown;
}

void BQuestLog_Click(IControl* Sender, EventArgs* E)
{
	BQuestLogDown = !BQuestLogDown;
}

void BParty_Click(IControl* Sender, EventArgs* E)
{
	BPartyDown = !BPartyDown;
}

void BHelp_Click(IControl* Sender, EventArgs* E)
{
	BHelpDown = !BHelpDown;
}


 
void PlayerActionBar_Click(RealmCrafter::IPlayerActionBar* ab, RealmCrafter::ActionBarEventArgs* e)
{
	int Slot = e->GetIndex();

	// Execute this slot
	if(RealmCrafter::Globals->CurrentMouseItem == NULL)
	{
		// Spell
		if(ActionBarSlots[Slot] < 0)
		{
			int Num, RechargeTime;
			NGin::CString Pa;
			if(RealmCrafter::Globals->RequireMemorise)
			{
				Num = ActionBarSlots[Slot] + 10;
				RechargeTime = SpellsList[Me->KnownSpells[Me->MemorisedSpells[Num]]]->RechargeTime;
				Pa = StrFromShort(Me->KnownSpells[Me->MemorisedSpells[Num]]);
			}else
			{
				Num = ActionBarSlots[Slot] + 1000;
				RechargeTime = SpellsList[Me->KnownSpells[Num]]->RechargeTime;
				Pa = StrFromShort(Me->KnownSpells[Num]);
			}

			// Recharged
			if(Me->SpellCharge[Num] <= 0)
			{
				if(PlayerTarget > 0)
				{
					ActorInstance* AI = (ActorInstance*)PlayerTarget;
					Pa.AppendRealShort(AI->RuntimeID);
				}
				CRCE_Send(Connection, Connection, P_SpellUpdate, NGin::CString("F") + Pa, true);

				Me->SpellCharge[Num] = RechargeTime;
			}else // Not recharged
				Output(LanguageString[LS_AbilityNotRecharged], 255, 50, 50);
		}else if(ActionBarSlots[Slot] < 65535) // Item
		{
			for(int ii = 0; ii < 50; ++ii)
			{
				if(Me->Inventory->Items[ii] != 0)
				{
					if(Me->Inventory->Items[ii]->Item->ID == ActionBarSlots[Slot])
					{
						UseItem(ii, 1);
						break;
					}
				}
			}
		}
	}else if(RealmCrafter::Globals->CurrentMouseItem->GetItemID() != 65535) // Add item to this slot
	{
		if(RealmCrafter::Globals->CurrentMouseItem->GetSourceLocation() == RealmCrafter::MIS_SDK)
			return;

		// Update slot
		ActionBarSlots[Slot] = RealmCrafter::Globals->CurrentMouseItem->GetItemID();

		// Tell server
		NGin::CString Pa = NGin::CString::FormatReal("cch", 'I', Slot, ActionBarSlots[Slot]);
		CRCE_Send(Connection, Connection, P_ActionBarUpdate, Pa, true);

		// Blank mouse slot
		if(RealmCrafter::Globals->CurrentMouseItem->GetSourceLocation() == RealmCrafter::MIS_Inventory)
		{
			RealmCrafter::IItemButton* ItemButton = reinterpret_cast<RealmCrafter::IItemButton*>(RealmCrafter::Globals->CurrentMouseItem->GetSourceData());
			ItemButton->SetHeldItem(RealmCrafter::Globals->CurrentMouseItem->GetItemID(), RealmCrafter::Globals->CurrentMouseItem->GetItemAmount());
		}
		delete RealmCrafter::Globals->CurrentMouseItem;
		RealmCrafter::Globals->CurrentMouseItem = NULL;

	}else if(RealmCrafter::Globals->CurrentMouseItem->GetSpellID() != 65535) // Add spell to this slot
	{
		Spell* Sp = SpellsList[RealmCrafter::Globals->CurrentMouseItem->GetSpellID()];

		if(Sp != NULL)
		{
			// Update slot
			if(RealmCrafter::Globals->RequireMemorise)
			{
				for(int j = 0; j <= 9; ++j)
				{
					if(Me->MemorisedSpells[j] != 5000)
					{
						Spell* Sp2 = SpellsList[Me->KnownSpells[Me->MemorisedSpells[j]]];
						if(Sp2 == Sp)
						{
							ActionBarSlots[Slot] = j - 10;
							break;
						}
					}
				}
			}else
			{
				for(int j = 0; j <= 999; ++j)
				{
					if(Me->SpellLevels[j] > 0)
					{
						Spell* Sp2 = SpellsList[Me->KnownSpells[j]];
						if(Sp2 == Sp)
						{
							ActionBarSlots[Slot] = j - 1000;
							break;
						}
					}
				}
			}

			// Tell server
			NGin::CString NSPName = Sp->Name.c_str();
			NGin::CString Pa = NGin::CString::FormatReal("ccs", 'S', Slot, &NSPName);
			CRCE_Send(Connection, Connection, P_ActionBarUpdate, Pa, true);

			delete RealmCrafter::Globals->CurrentMouseItem;
			RealmCrafter::Globals->CurrentMouseItem = NULL;
		}
	}
}

void PlayerActionBar_RightClick(RealmCrafter::IPlayerActionBar* ab, RealmCrafter::ActionBarEventArgs* e)
{
	int Slot = e->GetIndex();

	// Clear slot
	ActionBarSlots[Slot] = 65535;

	// Tell server
	NGin::CString Pa = NGin::CString::FormatReal("cc", 'N', Slot);
	CRCE_Send(Connection, Connection, P_ActionBarUpdate, Pa, true);

}

void BSpellRemoveOK_Click(IControl* Sender, EventArgs* E)
{
	// Tell server
	CRCE_Send(Connection, Connection, P_SpellUpdate, NGin::CString("U") + StrFromShort(Me->MemorisedSpells[SpellRemoveNum]), true);

	// Remove it from the action bar if it's there
	for(int i = 0; i < 36; ++i)
	{
		if(ActionBarSlots[i] == SpellRemoveNum - 10)
		{
			ActionBarSlots[i] = 65535;

			// Tell server
			NGin::CString Pa = NGin::CString::FormatReal("cc", 'N', i);
			CRCE_Send(Connection, Connection, P_ActionBarUpdate, Pa, true);
		}
	}
	// Remove it
	Me->MemorisedSpells[SpellRemoveNum] = 5000;

	// Hide the confirmation window
	WSpellRemove->Visible = false;
	PlayerSpells->BringToFront();
	SpellRemoveNum = -1;
}

void BSpellRemoveCancel_Click(IControl* Sender, EventArgs* E)
{
	WSpellRemove->Visible = false;
	PlayerSpells->BringToFront();
	SpellRemoveNum = -1;
}

void WSpellRemove_Closed(IControl* Sender, EventArgs* E)
{
	WSpellRemove->Visible = false;
	PlayerSpells->BringToFront();
	SpellRemoveNum = -1;
}

void BSpellError_Click(IControl* Sender, EventArgs* E)
{
	GUIManager->Destroy(WSpellError);
	PlayerSpells->BringToFront();
}

void WSpells_Closed(IControl* Sender, EventArgs* E)
{
	BSpellsDown = !SpellsVisible;
	PlaySound(GY_SBeep);
}

void PlayerSpells_Click(RealmCrafter::IPlayerSpells* Sender, RealmCrafter::SpellEventArgs* E)
{
	int FirstSpell = E->GetPage();

	if(FirstSpell < 0)
	{
		// if one is already in the mouse slot, just remove it
		if(RealmCrafter::Globals->CurrentMouseItem != NULL)
		{
			// Also check if we're holding an item, since we'll just do nothing in that case
			if(RealmCrafter::Globals->CurrentMouseItem->GetSpellID() == 65535)
				return;

			delete RealmCrafter::Globals->CurrentMouseItem;
			RealmCrafter::Globals->CurrentMouseItem = NULL;
		}else // Put selected item in slot
		{
			RealmCrafter::Globals->CurrentMouseItem = CreateMouseItem(GUIManager, 65535, Me->KnownSpells[Me->MemorisedSpells[E->GetOffset()]], 0, RealmCrafter::MIS_Spells, NULL);
		}

		return;
	}

	// Check for known spell buttons being clicked
	if(SMemorising == 0 && FirstSpell >= 0)
	{
		// Memorise if required
		if(!RealmCrafter::Globals->RequireMemorise)
		{
			// if one is already in the mouse slot, just remove it
			if(RealmCrafter::Globals->CurrentMouseItem != NULL)
			{
				// Also check if we're holding an item, since we'll just do nothing in that case
				if(RealmCrafter::Globals->CurrentMouseItem->GetSpellID() == 65535)
					return;

				delete RealmCrafter::Globals->CurrentMouseItem;
				RealmCrafter::Globals->CurrentMouseItem = NULL;
			}else // Put selected item in slot
			{
				RealmCrafter::Globals->CurrentMouseItem = CreateMouseItem(GUIManager, 65535, Me->KnownSpells[KnownSpellSort[FirstSpell + E->GetOffset()]], 0, RealmCrafter::MIS_Spells, NULL);
			}
		}
	}
}

void PlayerSpells_RightClick(RealmCrafter::IPlayerSpells* Sender, RealmCrafter::SpellEventArgs* E)
{
	int FirstSpell = E->GetPage();

	if(FirstSpell < 0 && RealmCrafter::Globals->CurrentMouseItem == NULL)
	{
		WSpellRemove->Visible = true;
		WSpellRemove->BringToFront();
		GUIManager->SetProperties("SpellRemove");
		WSpellRemove->Modal = true;
		SpellRemoveNum = E->GetOffset();

		return;
	}

	// Check for known spell buttons being clicked
	if(SMemorising == 0 && FirstSpell >= 0)
	{
		// Memorise if required
		if(RealmCrafter::Globals->RequireMemorise)
		{
			// Find known spell number
			MemoriseSpell = KnownSpellSort[FirstSpell + E->GetOffset()];

			// Check it's not already memorised
			bool Found = false;

			for(int j = 0; j < 10; ++j)
			{
				if(Me->MemorisedSpells[j] == MemoriseSpell)
				{
					Found = true;
					break;
				}
			}

			// Not already memorised, find free slot
			if(Found == false)
			{
				for(int j = 0; j < 10; ++j)
				{
					if(Me->MemorisedSpells[j] == 5000)
					{
						// Progress bar stuff
						LMemorising = GUIManager->CreateLabel("MemorisingProgress::Label", Math::Vector2(0.5f, 0.77f), Math::Vector2(0, 0));
						SMemorising = GUIManager->CreateProgressBar("MemorisingProgress::ProgressBar", Math::Vector2(0.3f, 0.8f), Math::Vector2(0.4f, 0.04f));
						LMemorising->Text = LanguageString[LS_MemorisingAbility];


						GUIManager->SetProperties("MemorisingProgress");
						
						LMemorising->ForeColor = Math::Color(255, 255, 255);
						LMemorising->Align = TextAlign_Center;
														
						MemoriseSlot = j;
						MemoriseProgress = 0;
						LastMemoriseUpdate = MilliSecs();
						Me->SetDestination(Me->Position);

						// Tell server
						CRCE_Send(Connection, Connection, P_SpellUpdate, NGin::CString("M") + StrFromShort(MemoriseSpell), true);

						// Done
						Found = true;
						break;
					}
				}

				// No free slot
				if(Found == false)
				{
					WSpellError = GUIManager->CreateWindow("SpellError::Window", Math::Vector2(0.2f, 0.4f), Math::Vector2(0.4f, 0.15f));
					ILabel* L = GUIManager->CreateLabel("SpellError::Label", Math::Vector2(0.5f, 0.05f), Math::Vector2(0, 0));
					
					WSpellError->Text = LanguageString[LS_MemoriseAbility];
					WSpellError->CloseButton = false;

					L->Text = LanguageString[LS_MaximumMemorised];
					L->ForeColor = Math::Color(255, 255, 255);
					L->Align = TextAlign_Center;
					L->Parent = WSpellError;

					BSpellError = GUIManager->CreateButton("SpellError::Button", Math::Vector2(0.35f, 0.6f), Math::Vector2(0.3f, 0.3f));
					BSpellError->Text = "OK";
					BSpellError->Parent = WSpellError;
					BSpellError->Click()->AddEvent(&BSpellError_Click);
					
					WSpellError->Visible = true;
					WSpellError->BringToFront();

					GUIManager->SetProperties("SpellError");

					WSpellError->Modal = true;
				}
			}else // Already memorised
			{
				WSpellError = GUIManager->CreateWindow("SpellError::Window", Math::Vector2(0.2f, 0.4f), Math::Vector2(0.4f, 0.15f));
				ILabel* L = GUIManager->CreateLabel("SpellError::Label", Math::Vector2(0.5f, 0.1f), Math::Vector2(0, 0));
				
				WSpellError->Text = LanguageString[LS_MemoriseAbility];
				WSpellError->CloseButton = false;

				L->Text = LanguageString[LS_AlreadyMemorised];
				L->ForeColor = Math::Color(255, 255, 255);
				L->Align = TextAlign_Center;
				L->Parent = WSpellError;

				BSpellError = GUIManager->CreateButton("SpellError::Button", Math::Vector2(0.35f, 0.6f), Math::Vector2(0.3f, 0.3f));
				BSpellError->Text = "OK";
				BSpellError->Parent = WSpellError;
				BSpellError->Click()->AddEvent(&BSpellError_Click);
				
				WSpellError->Visible = true;
				WSpellError->BringToFront();

				GUIManager->SetProperties("SpellError");

				WSpellError->Modal = true;
			}
		}else // if(memorisation is not required
		{
			int Num = KnownSpellSort[FirstSpell + E->GetOffset()];
			// Recharged
			if(Me->SpellCharge[Num] <= 0)
			{
				NGin::CString Pa = StrFromShort(Me->KnownSpells[Num]);
				if(PlayerTarget > 0)
				{
					ActorInstance* AI = (ActorInstance*)PlayerTarget;
					Pa.AppendRealShort(AI->RuntimeID);
				}
				CRCE_Send(Connection, Connection, P_SpellUpdate, NGin::CString("F") + Pa, true);
				Me->SpellCharge[Num] = SpellsList[Me->KnownSpells[Num]]->RechargeTime;
			}else // Not recharged
				Output(LanguageString[LS_AbilityNotRecharged], 255, 50, 50);
		}
	}
}

void PlayerInventory_Closed(IControl* Sender, EventArgs* E)
{
		BInventoryDown = !InventoryVisible;
		PlaySound(GY_SBeep);
}

void PlayerMap_Closed(IControl* Sender, EventArgs* E)
{
	BMapDown = !PlayerMapVisible;
	PlaySound(GY_SBeep);
}

void PlayerStats_Closed(IControl* Sender, EventArgs* E)
{
	BCharStatsDown = !CharStatsVisible;
	PlaySound(GY_SBeep);
}

void PlayerHelp_Closed(IControl* Sender, EventArgs* E)
{
	BHelpDown = !HelpVisible;
	PlaySound(GY_SBeep);
}

void TIAcceptButton_Click(IControl* Sender, EventArgs* E)
{
	TextInput* TI = (TextInput*)Sender->Tag;

	CRCE_Send(Connection, Connection, P_ScriptInput, StrFromInt(TI->ScriptHandle) + NGin::CString(TI->TextBox->Text.c_str()), true);
	FreeTextInput(Handle(TI));
}

// Creates a text input window and returns the handle
uint CreateTextInput(string Title, string Prompt, int Numeric, uint ScriptHandle)
{
	TextInput* TI = new TextInput();
	TI->ScriptHandle = ScriptHandle;

	TI->Win = GUIManager->CreateWindow("TextInput::Window", Math::Vector2(0.3f, 0.55f), Math::Vector2(0.5f, 0.17f));
	TI->Win->Text = Title;

	ILabel* PromptLabel = GUIManager->CreateLabel("TextInput::Label", Math::Vector2(0.05f, 0.2f), Math::Vector2(0, 0));
	PromptLabel->Text = Prompt;
	
	TI->TextBox = GUIManager->CreateTextBox("TextInput::TextBox", Math::Vector2(PromptLabel->Size.X + 0.1f, 0.2f), Math::Vector2(0.85f - PromptLabel->Size.X, 20 * (1.0f / GUIManager->GetResolution().Y)));
	TI->AcceptButton = GUIManager->CreateButton("TextInput::Button", Math::Vector2(0.75f, 0.7f), Math::Vector2(0.2f, 0.18f));
	TI->AcceptButton->Text = LanguageString[LS_Accept];

	switch(Numeric)
	{
		case 1: // Numbers disallowed
			TI->TextBox->ValidationExpression = "[^0-9]";
			break;
		case 2: // Numbers only (integer)
			TI->TextBox->ValidationExpression = "[0-9]";
			break;
		case 3: // Numbers with decimal points (floating)
			TI->TextBox->ValidationExpression = "[0-9.]";
			break;
		case 4: // Username verification
			TI->TextBox->ValidationExpression = "[0-9a-zA-Z_]";
			break;
		case 5: // Password Verification
			TI->TextBox->ValidationExpression = "[0-9a-zA-Z._]";
			break;
		case 6: // E-Mail Verification
			TI->TextBox->ValidationExpression = "[0-9a-zA-Z*+-.=_]";
			break;

	}

	TI->TextBox->Parent = TI->Win;
	TI->AcceptButton->Parent = TI->Win;
	PromptLabel->Parent = TI->Win;

	TI->AcceptButton->Click()->AddEvent(&TIAcceptButton_Click);
	TI->AcceptButton->Tag = TI;

	TI->Win->Visible = true;
	TI->Win->BringToFront();
	GUIManager->ControlFocus(TI->TextBox);
	InDialog = true;

	GUIManager->SetProperties("TextInput");

	return Handle(TI);
}

// Frees a text input dialog and all gadgets
void FreeTextInput(uint Han)
{
	TextInput* TI = (TextInput*)Han;
	if(TI != 0)
	{
		GUIManager->Destroy(TI->Win);
		TextInput::Delete(TI);
		TextInput::Clean();
	}
}

// Creates a dialog window and returns the handle
uint CreateDialog(string Title, ActorInstance* A, uint ScriptHandle, int BackgroundTexID)
{
	// Play speech if(applicable
	if(A != 0)
		PlayActorSound(A, Rand(Speech_Greet1, Speech_Greet2));

	// Create dialog
	Dialog* D = new Dialog();
	D->ActorInstance = A;
	D->ScriptHandle = ScriptHandle;

	D->Win = GUIManager->CreateWindow("ScriptDialog::Window", Math::Vector2(0.1f, 0.1f), Math::Vector2(0.5f, 0.4f));
	D->Win->CloseButton = false;
	D->Win->Text = Title;

	float Y = 0.005f;
	for(int i = 0; i < 14; ++i)
	{
		D->TextLines[i] = GUIManager->CreateLabel(string("ScriptDialog::Label") + toString(i), Math::Vector2(0.05f, Y), Math::Vector2(0, 0));
		D->TextLines[i]->Text = BBString(" ", 75);
		D->TextLines[i]->Parent = D->Win;
		D->TextLines[i]->Text = "";
		D->TextLines[i]->MouseEnter()->AddEvent(&TextLines_MouseEnter);
		D->TextLines[i]->MouseLeave()->AddEvent(&TextLines_MouseLeave);
		D->TextLines[i]->Click()->AddEvent(&TextLines_Click);
		D->TextLines[i]->Tag = D;
		Y += 0.07f;
	}

	D->Win->Visible = true;
	D->Win->BringToFront();
	InDialog = true;

	GUIManager->SetProperties("ScriptDialog");

	return Handle(D);
}

// Frees a dialog and everything in it
void FreeDialog(uint Han)
{
	Dialog* D = (Dialog*)Han;
	if(D != 0)
	{
		// Play speech if(applicable
		if(D->ActorInstance != 0)
			PlayActorSound(D->ActorInstance, Rand(Speech_Bye1, Speech_Bye2));

		// Free dialog
		GUIManager->Destroy(D->Win);
		Dialog::Delete(D);
	}
}

// Adds an option to a dialog
void AddDialogOption(uint Han, string Opt)
{
	Dialog* D = (Dialog*)Han;
	if(D != 0)
	{
		++D->TotalOptions;
		DialogOutput(Han, Opt, 0, 255, 0, D->TotalOptions);
	}
}

// Adds text to a dialog
void DialogOutput(uint Han, string T, int R, int G, int B, int Opt)
{
	Dialog* D = (Dialog*)Han;

	ILabel* L = GUIManager->CreateLabel("", Math::Vector2(0, 0), Math::Vector2(0, 0));


	// Word wrap
	L->Text = T;
	if(L->InternalWidth >= D->Win->Size.X - (40.0f / GUIManager->GetResolution().X))
	{
		for(int i = T.length() -1; i >= 0; --i)
		{
			if(T.substr(i, 1) == string(" "))
			{
				L->Text = T.substr(0, i);
				if(L->InternalWidth < D->Win->Size.X - (40.0f / GUIManager->GetResolution().X))
				{
					DialogOutput(Han,T.substr(0, i), R, G, B, Opt);
					DialogOutput(Han, T.substr(i + 1), R, G, B, Opt);
					GUIManager->Destroy(L);
					return;
				}
			}
		}
		for(int i = T.length(); i >= 1; --i)
		{
			L->Text = T.substr(0, i);
			if(L->InternalWidth < D->Win->Size.X - (40.0f / GUIManager->GetResolution().X))
			{
				DialogOutput(Han,T.substr(0, i), R, G, B, Opt);
				DialogOutput(Han, T.substr(i), R, G, B, Opt);
				GUIManager->Destroy(L);
				return;
			}
		}
		GUIManager->Destroy(L);
		return;
	}

	GUIManager->Destroy(L);

	// Add the line
	if(D != 0)
	{
		if(D->LastLine <= 13)
		{
			D->TextLines[D->LastLine]->Text = T;
			D->TextLines[D->LastLine]->ForeColor = Math::Color(R, G, B);
			D->TextText[D->LastLine] = T;
			D->TextR[D->LastLine] = R;
			D->TextG[D->LastLine] = G;
			D->TextB[D->LastLine] = B;
			D->OptionNum[D->LastLine] = Opt;
			++D->LastLine;
		}else
		{
			for(int i = 0; i < 13; ++i)
			{
				D->TextLines[i]->Text = D->TextText[i + 1];
				D->TextLines[i]->ForeColor = Math::Color(D->TextR[i + 1], D->TextG[i + 1], D->TextB[i + 1]);
				D->TextText[i] = D->TextText[i + 1];
				D->TextR[i] = D->TextR[i + 1];
				D->TextG[i] = D->TextG[i + 1];
				D->TextB[i] = D->TextB[i + 1];
				D->OptionNum[i] = D->OptionNum[i + 1];
			}
			D->TextLines[13]->Text = T;
			D->TextLines[13]->ForeColor = Math::Color(R, G, B);
			D->TextText[13] = T;
			D->TextR[13] = R;
			D->TextG[13] = G;
			D->TextB[13] = B;
			D->OptionNum[13] = Opt;
		}
	}

}


// Creates a new chat bubble
void BubbleOutput(string Label, int R, int G, int B, ActorInstance* AI, bool NoText)
{
	// Normal output
	if(NoText == false)
	{
		string Name = AI->Name;
		Name = trim(Name);

		if(Name.length() == 0)
		{
			Name = AI->Actor->Race;
		}

		if(R == 0 && G == 0 && B == 0)
		{
			Output(string("<") + Name + string("> ") + Label, 255, 255, 255);
		}
		else
		{
			Output(string("<") + Name + string("> ") + Label, R, G, B);
		}
	}

	// Remove any chat bubbles already attached to this actor instance
	foreachc(BbIt, Bubble, BubbleList)
	{
		Bubble* Bb = *BbIt;

		if(Bb->ActorInstance == AI)
		{
			GUIManager->Destroy(Bb->EN);
			Bubble::Delete(Bb);
		}
		
		nextc(BbIt, Bubble, BubbleList);
	}
	Bubble::Clean();
	
	// Create new bubble
	Bubble* Bb = new Bubble();

	Bb->EN = RealmCrafter::CreateBubbleOutput(GUIManager, "BubbleOutput::BubbleOutput", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(200, 20) / GUIManager->GetResolution());
	Bb->EN->Text = Label;
	Bb->EN->SendToBack();
	Bb->ActorInstance = AI;
	Bb->Timer = MilliSecs();
	GUIManager->SetProperties("BubbleOutput");
	Bb->EN->ForeColor = NGin::Math::Color(R, G, B);
}

// This is really an alias, since lots of stuff uses this format
void Output(string Dat, int R, int G, int B)
{
	RealmCrafter::Globals->ChatBox->Output(0, Dat, NGin::Math::Color(R, G, B));
}

// Selects the player target from the entity clicked on
string GetTarget(uint EN)
{
	string Name = EntityName(EN);
	if(Name.length() == 0)
		return "";

	int* Obj = (int*)toInt(EntityName(EN));
	if(Obj == NULL)
		return "";

	if(Obj[0] == TYPE_DroppedItem)
		return "D";
	if(Obj[1] == TYPE_ActorInstance)
		return "A";
	
	// Scenery	
	return "";
}

// Updates the direction of the compass
void UpdateCompass()
{
	float Yaw = EntityYaw(Me->CollisionEN);

	PlayerCompass->Update(Yaw);
}


Math::Vector2 P, F, R, A, MarkSize;
Math::Vector2 One = Math::Vector2(1, 1);
void UpdateRadar( ActorInstance *pMainPlayer )
{
	IRadar* Box = RadarBox;

	NGin::Math::Vector2 ZoneDim;
	ZoneDim.X = ZoneSize.Max().X - ZoneSize.Min().X;
	ZoneDim.Y = ZoneSize.Max().Z - ZoneSize.Min().Z;

	if(KeyDown(74))
		Box->ViewRadius += One;

	if(KeyDown(78))
		Box->ViewRadius -= One;

	while(Box->ViewRadius.X > ZoneDim.X * 0.5f || Box->ViewRadius.Y > ZoneDim.Y * 0.5f)
		Box->ViewRadius -= One;

	while(Box->ViewRadius.X < 64.0f || Box->ViewRadius.Y < 64.0f)
		Box->ViewRadius += One;

	//JB: We should really do this with sectors, though the precision at this level *shouldn't* be a 
	// big issue, consider a fix later.
	P.X = pMainPlayer->Position.X + (pMainPlayer->Position.SectorSize * pMainPlayer->Position.SectorX);
	P.Y = pMainPlayer->Position.Z + (pMainPlayer->Position.SectorSize * pMainPlayer->Position.SectorZ);

	

	NGin::Math::Vector2 ZoneSizeMin(ZoneSize.Min().X, ZoneSize.Min().Z);
	P -= ZoneSizeMin;
	
	NGin::Math::Vector2 ViewMin = P - Box->ViewRadius;
	NGin::Math::Vector2 ViewMax = P + Box->ViewRadius;
	ViewMin /= ZoneDim;
	ViewMax /= ZoneDim;


	bool LockedMaxX = false;
	bool LockedMinX = false;
	bool LockedMaxY = false;
	bool LockedMinY = false;


	float XDiff = 0.0f;
	float YDiff = 0.0f;
	
	if(ViewMin.X < 0.0f)
	{
		XDiff = 0.0f - ViewMin.X;
		LockedMinX = true;
		ViewMax.X += XDiff;
		ViewMin.X = 0.0f;
	}

	if(ViewMin.Y < 0.0f)
	{
		YDiff = 0.0f - ViewMin.Y;
		LockedMinY = true;
		ViewMax.Y += YDiff;
		ViewMin.Y = 0.0f;
	}

	if(ViewMax.X > 1.0f)
	{
		float Diff = ViewMax.X - 1.0f;
		LockedMaxX = true;
		ViewMax.X = 1.0f;
		if(!LockedMinX)
		{
			XDiff = Diff;
			ViewMin.X -= Diff;
			if(ViewMin.X < 0.0f)
				ViewMin.X = 0.0f;
		}
	}

	if(ViewMax.Y > 1.0f)
	{
		float Diff = ViewMax.Y - 1.0f;
		LockedMaxY = true;
		ViewMax.Y = 1.0f;
		if(!LockedMinY)
		{
			YDiff = Diff;
			ViewMin.Y -= Diff;
			if(ViewMin.Y < 0.0f)
				ViewMin.Y = 0.0f;
		}
	}

	NGin::Math::Vector2 PLoc = P / ZoneDim;
	NGin::Math::Vector2 RLoc(0.5f, 0.5f);

	RLoc = ViewMax - ViewMin;
	RLoc = (PLoc - ViewMin) / RLoc;
	RLoc.Y = 1.0f - RLoc.Y;

	float T = ViewMax.Y;
	ViewMax.Y = 1.0f - ViewMin.Y;
	ViewMin.Y = 1.0f - T;

	Box->MinTexCoord = ViewMin;
	Box->MaxTexCoord = ViewMax;

	Math::Vector4 ScissorWindow;
	Math::Vector2 Resolution, Location, Size;
	Resolution = GUIManager->GetResolution();
	Location = Box->Location;
	Size = Box->Size;

	ScissorWindow.X = Location.X;
	ScissorWindow.Y = Location.Y;
	ScissorWindow.Z = ScissorWindow.X + Size.X;
	ScissorWindow.W = ScissorWindow.Y + Size.Y;

	ScissorWindow.X *= Resolution.X;
	ScissorWindow.Z *= Resolution.X;
	ScissorWindow.Y *= Resolution.Y;
	ScissorWindow.W *= Resolution.Y;
	
	pRadarMainPlayer->Location = (Box->Location + (Box->Size * RLoc)) - (pRadarMainPlayer->Size * 0.5f);
	pRadarMainPlayer->ForceScissoring = true;
	pRadarMainPlayer->ScissorWindow = ScissorWindow;

	//pRadarMainPlayer->Location = Box->Location + Box->Size / 2 - pRadarMainPlayer->Size / 2;
	for ( std::list<tpMark>::iterator it(lpRadarMarks.begin()); it!=lpRadarMarks.end(); it++)
	{
		if ( !ShowActorsOnRadar ) {	it->second->Visible = false; continue;	}
		if (it->first->CollisionEN == NULL)
			continue;

		A.X = EntityX(it->first->CollisionEN);
		A.Y = EntityZ(it->first->CollisionEN); 

		A -= ZoneSizeMin;
		PLoc = A / ZoneDim;
		PLoc.Y = 1.0f - PLoc.Y;

		RLoc = ViewMax - ViewMin;
		RLoc = (PLoc - ViewMin) / RLoc;
		//RLoc.Y = 1.0f - RLoc.Y;

		R = (Box->Location + (Box->Size * RLoc)) - (it->second->Size * 0.5f);
		F = it->second->Size * Resolution;
		A = R * Resolution;
		if(A.X < ScissorWindow.X - F.X ||
			A.X > ScissorWindow.Z ||
			A.Y < ScissorWindow.Y - F.Y ||
			A.Y > ScissorWindow.W)
		{
			it->second->Visible = false;
		}else
		{
			it->second->Visible = true;
			it->second->Location = R;
			it->second->ForceScissoring = true;
			it->second->ScissorWindow = ScissorWindow;
		}
	}
}

void AddRadarMark( ActorInstance *pActor, int IdMark )
{
	IPictureBox *pPicMark = GUIManager->CreatePictureBox(std::string("RadarMark"), Math::Vector2(0, 0), Math::Vector2(20, 20)/GUIManager->GetResolution());
		pPicMark->SetImage(string("Data\\Textures\\") + GetTextureNameNoFlag(IdMark));
	lpRadarMarks.push_back( make_pair(pActor, pPicMark) );
}

void RemoveRadarMark( ActorInstance *pActor )
{
	for ( std::list<tpMark>::iterator it(lpRadarMarks.begin()); it!=lpRadarMarks.end(); it++)
		if ( it->first == pActor ) 
		{
			GUIManager->Destroy(it->second);
			lpRadarMarks.erase(it);
			break;
		}
}


// Updates actor effect icons display
void UpdateEffectIcons()
{
	// Clear all slots
	foreachc(SlotIt, EffectIconSlot, EffectIconSlotList)
	{
		EffectIconSlot* Slot = *SlotIt;

		Slot->EN->Visible = false;
		Slot->Effect = 0;

		nextc(SlotIt, EffectIconSlot, EffectIconSlotList);
	}

	// Retexture
	EffectIconSlot* Slot = *EffectIconSlot::EffectIconSlotList.Begin();

	foreachc(EIt, EffectIcon, EffectIconList)
	{
		EffectIcon* E = *EIt;

		if(Slot == 0)
			return;

		Slot->Effect = E;
		Slot->EN->Visible = true;
		Slot->EN->SetImage(string("Data\\Textures\\") + GetTextureNameNoFlag(E->TextureID));
		Slot = EffectIconSlot::EffectIconSlotList.After(Slot);

		nextc(EIt, EffectIcon, EffectIconList);
	}
}



void HideInterface()
{

	// Hide all windows
	PlayerStats->SetVisible(false);
	PlayerQuestLog->SetVisible(false);
	PlayerParty->SetVisible(false);
	PlayerInventory->SetVisible(false);
	PlayerSpells->SetVisible(false);
	WSpellRemove->Visible = false;
	PlayerHelp->SetVisible(false);
	PlayerMap->SetVisible(false);
	foreachc(FIt, EffectIconSlot, EffectIconSlotList)
	{
		(*FIt)->EN->Visible = false;

		nextc(FIt, EffectIconSlot, EffectIconSlotList);
	}
	ChatEntry->Visible = false;

	// Visible by default
	RealmCrafter::Globals->ChatBox->SetVisible(false);
	PlayerActionBar->SetVisible(false);
	RadarBox->Visible = false;
	pRadarMainPlayer->Visible = false;
	for ( std::list<tpMark>::iterator it(lpRadarMarks.begin()); it!=lpRadarMarks.end(); it++)
		it->second->Visible = false;

	PlayerCompass->SetVisible(false);

	// Attribute displays
	PlayerAttributeBars->SetVisible( false );
}

void ShowDefaultInterface()
{
	// Visible by default
	RealmCrafter::Globals->ChatBox->SetVisible(true);
	PlayerActionBar->SetVisible(true);
	RadarBox->Visible = true;
	pRadarMainPlayer->Visible = true;
	GUIManager->SetProperties("Radar");

	PlayerCompass->SetVisible(true);

	// Attribute displays
	PlayerAttributeBars->SetVisible( true );
}


// Creates the GUI stuff for the interface ready to be displayed
void CreateInterface()
{
	// Use a macro to make life easier
	#define R(X, Y) (Math::Vector2(X, Y) / GUIManager->GetResolution())

	// Chat box
	GUIManager->SetProperties("ChatHistory");

	// Attribute displays
	PlayerAttributeBars = RealmCrafter::CreatePlayerAttributeBars(GUIManager);
	PlayerAttributeBars->Initialize();

	// Large map
	PlayerMap = RealmCrafter::CreatePlayerMap(GUIManager);
	PlayerMap->Initialize();
	PlayerMap->Closed()->AddEvent(&PlayerMap_Closed);

	// Chat entry
	ChatEntry = GUIManager->CreateTextBox("ChatEntry::TextBox", R(6, 539), R(350, 20));
	ChatEntry->Visible = false;

	GUIManager->SetProperties("ChatEntry");

	// Compass
	PlayerCompass = RealmCrafter::CreatePlayerCompass(GUIManager);
	PlayerCompass->Initialize();
	
	// Radar
	RadarBox = GUIManager->CreateRadar("Radar::Radar", Math::Vector2(0.0f, 0.0f), Math::Vector2(0.5f, 0.5f) );
	RadarBox->SetImage( "Data\\Textures\\Radar.png", "Data\\Textures\\Radar_Border.png" );
	RadarBox->BringToFront();

	pRadarMainPlayer = GUIManager->CreatePictureBox(std::string("RadarMainPlayer"), Math::Vector2(0.0f, 0.0f), Math::Vector2(20, 20)/GUIManager->GetResolution() );
	pRadarMainPlayer->SetImage( "Data\\Textures\\" + GetTextureNameNoFlag(idPlayer) );

	GUIManager->SetProperties("Radar");

	for(int i = 0; i < 20; ++i)
	{
		EffectIconSlot* EIS = new EffectIconSlot();
		EIS->EN = GUIManager->CreatePictureBox(string("EffectIconSlots::Slot") + toString(i), Math::Vector2(0, 0), Math::Vector2(0.1, 0.1));
		EIS->EN->Visible = false;
	}

	GUIManager->SetProperties("EffectIconSlots");
	

	// Action Bar
	PlayerActionBar = RealmCrafter::CreatePlayerActionBar(GUIManager);
	PlayerActionBar->Initialize();

	PlayerActionBar->BChat_Click()->AddEvent(&BChat_Click);
	PlayerActionBar->BMap_Click()->AddEvent(&BMap_Click);
	PlayerActionBar->BInventory_Click()->AddEvent(&BInventory_Click);
	PlayerActionBar->BSpells_Click()->AddEvent(&BSpells_Click);
	PlayerActionBar->BCharStats_Click()->AddEvent(&BCharStats_Click);
	PlayerActionBar->BQuestLog_Click()->AddEvent(&BQuestLog_Click);
	PlayerActionBar->BParty_Click()->AddEvent(&BParty_Click);
	PlayerActionBar->BHelp_Click()->AddEvent(&BHelp_Click);

	PlayerActionBar->Click()->AddEvent(&PlayerActionBar_Click);
	PlayerActionBar->RightClick()->AddEvent(&PlayerActionBar_RightClick);

	// Party window
	PlayerParty = RealmCrafter::CreatePlayerParty(GUIManager);
	PlayerParty->Initialize();
	PlayerParty->Closed()->AddEvent(&PlayerParty_Closed);
	PlayerParty->Click()->AddEvent(&PlayerParty_Click);
	PlayerParty->Leave()->AddEvent(&PlayerParty_Leave);

	// Spells/Abilities window
	PlayerSpells = RealmCrafter::CreatePlayerSpells(GUIManager);
	PlayerSpells->Initialize();
	PlayerSpells->Closed()->AddEvent(&WSpells_Closed);
	PlayerSpells->Click()->AddEvent(&PlayerSpells_Click);
	PlayerSpells->RightClick()->AddEvent(&PlayerSpells_RightClick);
	GUIManager->SetProperties("SpellsWindow");

	// Spell un-memorization confirmation box
	WSpellRemove = GUIManager->CreateWindow("SpellRemove::Window", Math::Vector2(0.35f, 0.4f), Math::Vector2(0.4f, 0.1f));
	ILabel* L = GUIManager->CreateLabel("SpellRemove::Label", Math::Vector2(0.5f, 0.05f), Math::Vector2(0, 0)); 
	BSpellRemoveOK = GUIManager->CreateButton("SpellRemove::OkButton", Math::Vector2(0.1f, 0.7f), Math::Vector2(0.3f, 0.2f));
	BSpellRemoveCancel = GUIManager->CreateButton("SpellRemove::CancelButton", Math::Vector2(0.6f, 0.7f), Math::Vector2(0.3f, 0.2f));

	WSpellRemove->Text = LanguageString[LS_Unmemorise];
	L->Text = LanguageString[LS_UnmemoriseDetail];
	BSpellRemoveOK->Text = LanguageString[LS_Yes];
	BSpellRemoveCancel->Text = LanguageString[LS_No];

	L->ForeColor = Math::Color(255, 255, 255);
	L->Align = TextAlign_Center;

	L->Parent = WSpellRemove;
	BSpellRemoveOK->Parent = WSpellRemove;
	BSpellRemoveCancel->Parent = WSpellRemove;

	WSpellRemove->Closed()->AddEvent(&WSpellRemove_Closed);
	BSpellRemoveOK->Click()->AddEvent(&BSpellRemoveOK_Click);
	BSpellRemoveCancel->Click()->AddEvent(&BSpellRemoveCancel_Click);

	GUIManager->SetProperties("SpellRemove");

	// Quest log window
	PlayerQuestLog = RealmCrafter::CreatePlayerQuestLog(GUIManager, MyQuestLog);
	PlayerQuestLog->Initialize();
	PlayerQuestLog->Closed()->AddEvent(&PlayerQuestLog_Closed);

	// Stats
	PlayerStats = RealmCrafter::CreatePlayerStats(GUIManager);
	PlayerStats->Initialize();
	PlayerStats->Closed()->AddEvent(&PlayerStats_Closed);

	// Help
	PlayerHelp = RealmCrafter::CreatePlayerHelp(GUIManager);
	PlayerHelp->Initialize();
	PlayerHelp->Closed()->AddEvent(&PlayerHelp_Closed);

	// Inventory
	PlayerInventory = RealmCrafter::CreatePlayerInventory(GUIManager);
	PlayerInventory->Initialize();
	PlayerInventory->Closed()->AddEvent(&PlayerInventory_Closed);
	GUIManager->SetProperties("Inventory");


	// Hide all windows
	PlayerStats->SetVisible(false);
	PlayerQuestLog->SetVisible(false);
	PlayerParty->SetVisible(false);
	PlayerInventory->SetVisible(false);
	PlayerSpells->SetVisible(false);
	WSpellRemove->Visible = false;
	PlayerHelp->SetVisible(false);
	PlayerMap->SetVisible(false);

	LastMouseMove = MilliSecs();
	LastLeftClick = MilliSecs();

	#undef R
}

// Sets picking modes
void SetPickModes(int TypeScenery, int Actors, bool NonCombatants, int DroppedItems)
{
	if(TypeScenery == 0)
	{
		foreachc(SIt, Scenery, SceneryList)
		{
			Scenery* S = *SIt;

			int Collides = GetEntityType(S->EN);
			if(Collides == C_Sphere)
				EntityPickMode(S->EN, 1);
			else if(Collides == C_Triangle || Collides == C_Box)
			{
				if(S->CollisionEN != NULL)
					EntityPickMode(S->CollisionEN, 2);
				else
					EntityPickMode(S->EN, 2);
			}

			nextc(SIt, Scenery, SceneryList);
		}
		foreachc(TIt, Terrain, TerrainList)
		{
			std::vector<TerrainTagItem*>* TerrainTag = (std::vector<TerrainTagItem*>*)((*TIt)->Handle->GetTag());
			if(TerrainTag != NULL)
			{
				for(int i = 0; i < TerrainTag->size(); ++i)
					if((*TerrainTag)[i] != NULL)
						EntityPickMode((*TerrainTag)[i]->Mesh, 2);
			}

			nextc(TIt, Terrain, TerrainList);
		}
	}else if(TypeScenery == -1)
	{
		foreachc(SIt, Scenery, SceneryList)
		{
			if((*SIt)->CollisionEN != NULL)
				EntityPickMode((*SIt)->CollisionEN, 0);
			EntityPickMode((*SIt)->EN, 0);
			nextc(SIt, Scenery, SceneryList);
		}
		foreachc(TIt, Terrain, TerrainList)
		{
			std::vector<TerrainTagItem*>* TerrainTag = (std::vector<TerrainTagItem*>*)((*TIt)->Handle->GetTag());
			for(int i = 0; i < TerrainTag->size(); ++i)
				EntityPickMode((*TerrainTag)[i]->Mesh, 0);
			nextc(TIt, Terrain, TerrainList);
		}
	}else
	{
		foreachc(SIt, Scenery, SceneryList)
		{
			if((*SIt)->CollisionEN != NULL)
				EntityPickMode((*SIt)->CollisionEN, TypeScenery);
			else
				EntityPickMode((*SIt)->EN, TypeScenery);
		
			nextc(SIt, Scenery, SceneryList);
		}
		foreachc(TIt, Terrain, TerrainList)
		{
			std::vector<TerrainTagItem*>* TerrainTag = (std::vector<TerrainTagItem*>*)((*TIt)->Handle->GetTag());
			for(int i = 0; i < TerrainTag->size(); ++i)
				EntityPickMode((*TerrainTag)[i]->Mesh, TypeScenery);
			nextc(TIt, Terrain, TerrainList);
		}
	}

	ActorInstance::Clean();
	foreachc(AIIt, ActorInstance, ActorInstanceList)
	{
		ActorInstance* AI = *AIIt;
	
		if(Actors == 4)
		{
			if(AI->Attributes->Value[RealmCrafter::Globals->HealthStat] > 0)
			{
				EntityPickMode(AI->EN, 0);
				EntityPickMode(AI->CollisionEN, 1);
			}else
			{
				EntityPickMode(AI->EN, 0);
				EntityPickMode(AI->CollisionEN, 0);
			}
		}else
		{
			if(AI != Me && AI->Rider == 0)
			{
				if(AI->Attributes->Value[RealmCrafter::Globals->HealthStat] <= 0 || (AI->Actor->Aggressiveness == 3 && NonCombatants == false) || (AI->RNID > 0 && PVPEnabled == false && NonCombatants == false))
				{
					EntityPickMode(AI->EN, 0);
					EntityPickMode(AI->CollisionEN, 0);
				}else if(Actors == 1)
				{
					EntityPickMode(AI->EN, 0);
					EntityPickMode(AI->CollisionEN, 1);
				}else if(Actors == 2)
				{
					EntityPickMode(AI->EN, 2);
					EntityPickMode(AI->CollisionEN, 0);
				}else if(Actors == 3)
				{
					EntityPickMode(AI->EN, 0);
					EntityPickMode(AI->CollisionEN, 1);
				}else
				{
					EntityPickMode(AI->EN, 0);
					EntityPickMode(AI->CollisionEN, 0);
				}
			}else
			{
				EntityPickMode(AI->EN, 0);
				EntityPickMode(AI->CollisionEN, 0);
			}
		}

		nextc(AIIt, ActorInstance, ActorInstanceList);
	}

	foreachc(DIt, DroppedItem, DroppedItemList)
	{
		EntityPickMode((*DIt)->EN, DroppedItems);
		nextc(DIt, DroppedItem, DroppedItemList);
	}
}

// Creates an action bar button
IButton* CreateActionBarButton(string TexName, float X, bool Toggle, bool FreeTex)
{
	IButton* Button = GUIManager->CreateButton("", Math::Vector2(X + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
	
	string* S = new string();
	*S = string("Data\\Textures\\GUI\\") + TexName;
	Button->SetUpImage(*S);
	Button->Tag = S;

	return Button;
}

void WItemWindow_Closed(IControl* Sender, EventArgs* E)
{
	if(WItemWindow != 0)
	{
		GUIManager->Destroy(WItemWindow);
		WItemWindow = 0;
		if(ItemWindowFromInventory)
			BInventoryDown = true;
	}
}

// Eats/equips/calls the script for an item
void UseItem(int SlotIndex, int Amount)
{
	if(Me->Inventory->Items[SlotIndex] != 0)
	{
		if(Me->Inventory->Amounts[SlotIndex] >= Amount)
		{
			// Eat food
			if(Me->Inventory->Items[SlotIndex]->Item->ItemType == I_Potion || Me->Inventory->Items[SlotIndex]->Item->ItemType == I_Ingredient)
			{
				if(Me->Inventory->Items[SlotIndex]->Item->ExclusiveClass.length() == 0 || stringToUpper(Me->Inventory->Items[SlotIndex]->Item->ExclusiveClass).compare(stringToUpper(Me->Actor->Class)) == 0)
				{
					if(Me->Inventory->Items[SlotIndex]->Item->ExclusiveRace.length() == 0 || stringToUpper(Me->Inventory->Items[SlotIndex]->Item->ExclusiveRace).compare(stringToUpper(Me->Actor->Race)) == 0)
					{
						NGin::CString Pa = NGin::CString::FormatReal("ch", SlotIndex, Amount);
						CRCE_Send(Connection, Connection, P_EatItem, Pa, true);
						Me->Inventory->Amounts[SlotIndex] -= Amount;
					}
				}
			}else if(Me->Inventory->Items[SlotIndex]->Item->ItemType == I_Image)
			{
				// Dreamora: 1.23
				// Image item
				float gwidth = GraphicsWidth();
				float gheight = GraphicsHeight();
				
				uint texture = GetTexture(Me->Inventory->Items[SlotIndex]->Item->ImageID, false);
				float sizex = gwidth / TextureWidth(texture);
				float sizey = gheight / TextureHeight(texture);
				float aspect = TextureWidth(texture) / (float)(TextureHeight(texture));
				
				if(sizex > 0.9f || sizey*aspect > 0.9f)
				{
					sizex = 0.9f;
					sizey = sizex/aspect;
				}

				if(sizey > 0.8f)
				{
					sizey = 0.8f;
					sizex = sizey*aspect;
				}

				if(sizex < 0.2f || sizey*aspect < 0.2f)
				{
					sizex = 0.2f;
					sizey = sizex/aspect;
				}
				
				if(sizey < 0.2f)
				{
					sizey = 0.2f;
					sizex = sizey * aspect;
				}
				
				if(BInventoryDown || ItemWindowFromInventory)
				{
					BInventoryDown = false;
					ItemWindowFromInventory = true;
				}else
					ItemWindowFromInventory = false;
				
				if(WItemWindow != 0)
					GUIManager->Destroy(WItemWindow);
				
				WItemWindow = GUIManager->CreateWindow(Me->Inventory->Items[SlotIndex]->Item->Name,
					Math::Vector2((1.0f - sizex) / 2, (1.0f - sizey) / 2 - 0.05f),
					Math::Vector2(sizex, sizey));

				WItemWindow->Closed()->AddEvent(&WItemWindow_Closed);
				WItemWindow->BringToFront();
				
			}else
			{
				// Item script
				NGin::CString Pa;
				Pa.AppendRealChar(SlotIndex);
				if(PlayerTarget > 0)
				{
					ActorInstance* AI = (ActorInstance*)PlayerTarget;
					Pa.AppendRealShort(AI->RuntimeID);
				}
				CRCE_Send(Connection, Connection, P_ItemScript, Pa, true);

				// Equip weapon
				if(Me->Inventory->Items[SlotIndex]->Item->ItemType == I_Weapon && SlotIndex >= SlotI_Backpack)
				{
					if(Me->Inventory->Items[SlotI_Weapon] == 0)
					{
						bool Result = InventorySwap(Me, SlotIndex, SlotI_Weapon, Amount);
						if(Result == true)
						{
							UpdateActorItems(Me);
						}
					}
				// Equip armour
				}else if(Me->Inventory->Items[SlotIndex]->Item->ItemType == I_Armour && SlotIndex >= SlotI_Backpack)
				{
					int i = Me->Inventory->Items[SlotIndex]->Item->SlotType - 1;
					if(Me->Inventory->Items[i] == 0)
					{
						bool Result = InventorySwap(Me, SlotIndex, i, Amount);
						if(Result == true)
						{
							UpdateActorItems(Me);
						}
					}
				}
			}
		}
	}
}

// Updates the standard components and handles player input
void UpdateInterface()
{
	// Gooey system
	GY_Update();

	int Key_AlwaysRun = 0;
	int Key_CycleTarget = 0;
	int Key_Jump = 0;
	int Key_TalkTo = 0;
	int Key_Select = 0;
	int Key_MoveTo = 0;
	int Key_Attack = 0;

	if(ControlLayout->Get("Always Run") != NULL)
		Key_AlwaysRun = ControlLayout->Get("Always Run")->ControlID;
	if(ControlLayout->Get("Cycle Target") != NULL)
		Key_CycleTarget = ControlLayout->Get("Cycle Target")->ControlID;
	if(ControlLayout->Get("Jump") != NULL)
		Key_Jump = ControlLayout->Get("Jump")->ControlID;
	if(ControlLayout->Get("Talk To") != NULL)
		Key_TalkTo = ControlLayout->Get("Talk To")->ControlID;
	if(ControlLayout->Get("Select") != NULL)
		Key_Select = ControlLayout->Get("Select")->ControlID;
	if(ControlLayout->Get("Move To") != NULL)
		Key_MoveTo = ControlLayout->Get("Move To")->ControlID;
	if(ControlLayout->Get("Attack") != NULL)
		Key_Attack = ControlLayout->Get("Attack")->ControlID;

	// Escape used both to close windows and quit the game
	bool EscapeHit = KeyHit(1);

	// Toggle always run mode
	if(ControlHit(Key_AlwaysRun))
		RealmCrafter::Globals->AlwaysRun = !RealmCrafter::Globals->AlwaysRun;

	// Cycle target
	if(ControlHit(Key_CycleTarget))
	{
		ActorInstance* StartAI = (ActorInstance*)PlayerTarget;
		if(StartAI == 0)
			StartAI = *(ActorInstance::ActorInstanceList.Begin());
		ActorInstance* AI = StartAI;
		while(true)
		{
			AI = ActorInstance::ActorInstanceList.After(AI);
			if(AI == 0)
				AI = *(ActorInstance::ActorInstanceList.Begin());
			if(AI == StartAI)
				break;
			if(AI->Actor->Aggressiveness < 3 && AI->RNID == 0)
			{
				uint OldTarget = PlayerTarget;
				PlayerTarget = Handle(AI);
				float MaxLength = MeshWidth(AI->EN);
				if(MeshDepth(AI->EN) > MaxLength)
					MaxLength = (MeshDepth(AI->EN) + MeshWidth(AI->EN)) / 2.0f;
				ScaleEntity(ActorSelectEN, MaxLength * 0.03f, 1.0f, MaxLength * 0.03f);
				ShowEntity(ActorSelectEN);
				RealmCrafter::Globals->AttackTarget = false;
				break;
			}
		}
	}

	// Hide movement marker after time expires
	if(MilliSecs() - ClickMarkerTimer > 3000)
		HideEntity(ClickMarkerEN);

	// Get environment type
	int EType = Me->Actor->Environment;
	if(Me->Mount != 0)
		EType = Me->Mount->Actor->Environment;

	// Keyboard controls
	if(ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && !TextBoxIsFocused())
	{
		// Jump
		static int JumpCount = 0;
		if(ControlHit(Key_Jump))
		{
			if(PlayerHasTouchedDown == true && Me->Underwater == 0)
			{
			if(Me->Mount == 0)
			{
				//Aprintf("JumpCount: %i\n", JumpCount);++JumpCount;
				Me->Position.Y = JumpStrength * Gravity * Delta;
				PlayAnimation(Me, 3, 0.05f, Anim_Jump);
				PlayerHasTouchedDown = false;
				CRCE_Send(Connection, Connection, P_Jump, NGin::CString(""), true);
				//Aprintf("Jumped\n");
			}
			}else
			{
				DebugLog(string("JumpNoTouch: ") + toString(MilliSecs()));
			}
		}

		// Flying/swimming
		if(EType == Environment_Fly || Me->Underwater != 0)
		{
#pragma message("2.50 Warning: CHANGE FLIGHT")
			// Up
			if(ControlDown(ControlLayout->Get("Fly Up")->ControlID))
			{
				if(Me->Mount == 0)
					TranslateEntity(Me->CollisionEN, 0.0f, 0.5f * Delta, 0.0f);
				else
					TranslateEntity(Me->Mount->CollisionEN, 0.0f, 0.5f * Delta, 0.0f);

				// Do not allow above water surface if swimming
				if(Me->Underwater != 0)
				{
					Water* W = (Water*)Me->Underwater;
					// JB 2.22
					if(EntityY(Me->CollisionEN) > EntityY(W->EN) - 0.5f)
						TranslateEntity(Me->CollisionEN, 0.0f, -0.5f * Delta, 0.0f);
				}
				RealmCrafter::Globals->QuitActive = false;
			}else if(ControlDown(ControlLayout->Get("Fly Down")->ControlID)) // Down
			{
				// Do not allow below water surface if flying
				bool Allowed = true;
				if(Me->Underwater == 0)
				{
					foreachc(WIt, Water, WaterList)
					{
						Water* W = *WIt;

						if(EntityY(Me->CollisionEN) <= EntityY(W->EN) + 3.0f)
							if(Abs(EntityX(Me->CollisionEN) - EntityX(W->EN)) < (W->ScaleX / 2.0f))
								if(Abs(EntityZ(Me->CollisionEN) - EntityZ(W->EN)) < (W->ScaleZ / 2.0f))
								{
									Allowed = false;
									break;
								}

						nextc(WIt, Water, WaterList);
					}
				}
				if(Allowed == true)
				{
					if(Me->Mount == 0)
						TranslateEntity(Me->CollisionEN, 0.0f, -0.5f * Delta, 0.0f);
					else
						TranslateEntity(Me->Mount->CollisionEN, 0.0f, -0.5f * Delta, 0.0f);
					RealmCrafter::Globals->QuitActive = false;
				}
			}
		}
	}// End keyboard controls

	// Update these buttons if(the mouse is not over a dialog or the action bar
	if(GY_MouseY < 0.85f && (CurrentSeq(Me) >= Anim_LookRound || Animating(Me->EN) == false) && InDialog == false && SMemorising == 0)
	{
		// Talk to button down, remember the time (so we ignore presses which take more than 500ms)
		if(ControlDown(Key_TalkTo))
		{
			if(RightWasDown == false)
				RightDownTime = MilliSecs();
			RightWasDown = true;
		}else
			RightWasDown = false;

		// Talk to button was clicked
		if(/*ControlHit(Key_TalkTo)*/ !RightWasDown && MilliSecs() - RightDownTime < 200)
		{
			RightDownTime = 0;
			bool ClickedTarget = false;

			if(PlayerTarget == NULL && SceneryTarget == NULL)
			{
				//SetPickModes(-1, 3, true);
				uint Result = CameraPick(Cam, MouseX(), MouseY());
				if(Result != 0)
				{
					string Target = GetTarget(Result);

					// Target is an actor
					if(Target == string("A"))
					{
						string TargetStr = EntityName(Result);
						ActorInstance* AISel = (ActorInstance*)toInt(TargetStr);
						SceneryTarget = NULL;

						if(pow((EntityX(AISel->CollisionEN, true) - EntityX(Me->CollisionEN, true)), 2.0f)
							+ pow((EntityY(AISel->CollisionEN, true) - EntityY(Me->CollisionEN, true)), 2.0f)
							+ pow((EntityZ(AISel->CollisionEN, true) - EntityZ(Me->CollisionEN, true)), 2.0f)
							< 100.0f)
						{
							CRCE_Send(Connection, Connection, P_RightClick, StrFromShort(AISel->RuntimeID), true);
							RealmCrafter::Globals->QuitActive = false;
							ClickedTarget = true;
						}
					}else if(Target != string("D"))
					{
						// Find scenery selection
						SceneryTarget = (Scenery*)toInt(EntityName(Result));
						PlayerTarget = NULL;

						if(SceneryTarget != NULL && SceneryTarget->Interactive)
						{
							if(pow((EntityX(SceneryTarget->EN, true) - EntityX(Me->CollisionEN, true)), 2.0f)
								+ pow((EntityY(SceneryTarget->EN, true) - EntityY(Me->CollisionEN, true)), 2.0f)
								+ pow((EntityZ(SceneryTarget->EN, true) - EntityZ(Me->CollisionEN, true)), 2.0f)
								< 100.0f)
							{
								RealmCrafter::PacketWriter Pa;
								Pa.Write(SceneryTarget->Position.X);
								Pa.Write(SceneryTarget->Position.Y);
								Pa.Write(SceneryTarget->Position.Z);
								Pa.Write((unsigned short)SceneryTarget->Position.SectorX);
								Pa.Write((unsigned short)SceneryTarget->Position.SectorZ);
								Pa.Write((unsigned short)SceneryTarget->MeshID);

								NGin::CString SendPkt;
								SendPkt.Set(Pa.GetBytes(), Pa.GetLength());

								CRCE_Send(Connection, Connection, P_SceneryInteract, SendPkt, true);
								RealmCrafter::Globals->QuitActive = false;
								ClickedTarget = true;
							}
						}else
						{
							SceneryTarget = NULL;
						}
					}
				}
			}

			// Selected target actor, show interaction window
			else if(PlayerTarget > 0)
			{
				ActorInstance* AI = (ActorInstance*)PlayerTarget;
				
				//if(EntityDistance(AI->CollisionEN, Me->CollisionEN) < 10.0f)
				if(pow((EntityX(AI->CollisionEN, true) - EntityX(Me->CollisionEN, true)), 2.0f)
					+ pow((EntityY(AI->CollisionEN, true) - EntityY(Me->CollisionEN, true)), 2.0f)
					+ pow((EntityZ(AI->CollisionEN, true) - EntityZ(Me->CollisionEN, true)), 2.0f)
					< 100.0f)
				{			
					// Verify clickability
					SetPickModes(0, 3, true);
					uint Result = CameraPick(Cam, MouseX(), MouseY());
					if(Result != 0)
					{
						string Target = GetTarget(Result);

						// Target is an actor
						if(Target == string("A"))
						{
							string TargetStr = EntityName(Result);
							ActorInstance* AISel = (ActorInstance*)toInt(TargetStr);

							if(AISel == AI)
							{
								CRCE_Send(Connection, Connection, P_RightClick, StrFromShort(AI->RuntimeID), true);
								RealmCrafter::Globals->QuitActive = false;
								ClickedTarget = true;
							}
						}else
						{
							PlayerTarget = NULL;
						}
					}else
					{
						PlayerTarget = NULL;
					}

				}
			}

			// Targeting a scenery
			else if(SceneryTarget != NULL)
			{
				if(SceneryTarget->Interactive &&
					pow((EntityX(SceneryTarget->EN, true) - EntityX(Me->CollisionEN, true)), 2.0f)
					+ pow((EntityY(SceneryTarget->EN, true) - EntityY(Me->CollisionEN, true)), 2.0f)
					+ pow((EntityZ(SceneryTarget->EN, true) - EntityZ(Me->CollisionEN, true)), 2.0f)
					< 150.0f)
				{
					RealmCrafter::PacketWriter Pa;
					Pa.Write(SceneryTarget->Position.X);
					Pa.Write(SceneryTarget->Position.Y);
					Pa.Write(SceneryTarget->Position.Z);
					Pa.Write((unsigned short)SceneryTarget->Position.SectorX);
					Pa.Write((unsigned short)SceneryTarget->Position.SectorZ);
					Pa.Write((unsigned short)SceneryTarget->MeshID);

					NGin::CString SendPkt;
					SendPkt.Set(Pa.GetBytes(), Pa.GetLength());

					CRCE_Send(Connection, Connection, P_SceneryInteract, SendPkt, true);
					RealmCrafter::Globals->QuitActive = false;
					ClickedTarget = true;
				}
			}
		}

		// Select target (actor, dropped item or usable scenery)
		bool UsedClick = false;
		if(ControlDown(Key_Select))
		{
			SelectKeyWasDown = true;
		}else if(SelectKeyWasDown == true)
		{
			// Was this a double click?
			bool IsDouble = false;
			if(MilliSecs() - SelectKeyClickTime < 500)
				IsDouble = true;

			// Store time of click
			SelectKeyClickTime = MilliSecs();
			SelectKeyWasDown = false;

			// Pick a target
			SetPickModes(0, 3, true, 1);
			uint Result = CameraPick(Cam, MouseX(), MouseY());
			if(Result != 0)
			{
				string Target = GetTarget(Result);

				// Target is another actor
				if(Target == string("A"))
				{
					UsedClick = true;

					// Single or double click selects the target
					uint OldTarget = PlayerTarget;
					PlayerTarget = toInt(EntityName(Result));
					SceneryTarget = NULL;
					ActorInstance* AI = (ActorInstance*)PlayerTarget;
					float MaxLength = MeshWidth(AI->EN);
					if(MeshDepth(AI->EN) > MaxLength)
						MaxLength = (MeshDepth(AI->EN) + MeshWidth(AI->EN)) / 2.0f;
					ScaleEntity(ActorSelectEN, MaxLength * 0.03f, 1.0f, MaxLength * 0.03f);
					ShowEntity(ActorSelectEN);

					// Double clicking the target makes you run towards it and attack if(in range
					if(IsDouble == true && OldTarget == PlayerTarget)
					{
						RealmCrafter::SectorVector PickedVector(PickedX(), PickedY(), PickedZ(), Me->Position.SectorX, Me->Position.SectorZ);
						Me->SetDestination(PickedVector);
						//CamYaw = 0.0f;
						//CamPitch = 0.0f;
						Me->IsRunning = true;
						if(Me->Mount != 0)
							Me->Mount->IsRunning = true;

						// Check target is a combatant
						if(AI->Actor->Aggressiveness < 3)
						{
							// Check faction rating
							if(Me->FactionRatings[AI->HomeFaction] <= 150)
								RealmCrafter::Globals->AttackTarget = true;
						}
					}else
						RealmCrafter::Globals->AttackTarget = false;
					HideEntity(ClickMarkerEN);
				
				}else if(Target == string("D")) // Target is a dropped item
				{
					// In range - pick it up if(room in inventory
					if(EntityDistance(Result, Me->EN) < 100.0f)
					{
						UsedClick = true;

						DroppedItem* DItem = (DroppedItem*)toInt(EntityName(Result));
						int FoundSlot = -1;
						for(int i = 0; i < 50; ++i)
							if(Me->Inventory->Items[i] == 0 || (ItemInstancesIdentical(DItem->Item, Me->Inventory->Items[i]) && DItem->Item->Item->Stackable == true && i >= SlotI_Backpack))
								if(SlotsMatch(DItem->Item->Item, i) && ActorHasSlot(Me->Actor, i, DItem->Item->Item))
								{
									FoundSlot = i;
									break;
								}

						// Room, request it from server
						if(FoundSlot > -1)
						{
							NGin::CString Pa = NGin::CString::FormatReal("cic", 'P', DItem->ServerHandle, FoundSlot);
							CRCE_Send(Connection, Connection, P_InventoryUpdate, Pa, true);
						}else{ // No room!
							Output(LanguageString[LS_NoInventorySpace], 255, 0, 0);
						}
					}
				}else if(Target.length() == 0) // Target is scenery
				{
					// if(I am riding a mount, get off
					if(IsDouble && Me->Mount != 0)
					{
						CRCE_Send(Connection, Connection, P_Dismount, NGin::CString(""), true);
					}else // Otherwise check if(I can use this scenery
					{
						Scenery* Sc = (Scenery*)toInt(EntityName(Result));
						if(Sc != 0)
						{
							// Its interactive scenery
							if(Sc->Interactive)
							{
								PlayerTarget = NULL;
								SceneryTarget = Sc;
								if(SceneryTarget->Interactive)
								{
								}else
								{
									SceneryTarget = NULL;
								}

							}
							
							// Its owned (different code path)
							else
							{
								PositionEntity(GPP, PickedX(), PickedY(), PickedZ());
								if(EntityDistance(Me->CollisionEN, GPP) < 10.0f)
								{
									UsedClick = true;

									// Animate now
									if(Sc->AnimationMode == 3 && Sc->SceneryID == 0)
										if(AnimTime(Sc->EN) > 0.0f)
											Animate(Sc->EN, 3, -1.0f, 0, 1.0f);
										else
											Animate(Sc->EN, 3, 1.0f, 0, 1.0f);
									
									// Ask server whether I own this scenery
									if(Sc->SceneryID > 0)
										CRCE_Send(Connection, Connection, P_SelectScenery, StrFromShort(Sc->SceneryID) + StrFromInt(Handle(Sc)), true);
								}
							}
						}
					}
				}
			}
		}

		// Move-to button is being held down
		if(ControlDown(Key_MoveTo))
		{
			MouseWasDown = true;
			// if(the mouse still on the same scenery target, continue updating player destination
			if(InDialog = false && SMemorising == 0)
			{
				SetPickModes(0, 3, true);
				uint Result = CameraPick(Cam, MouseX(), MouseY());
				if(Result != 0)
				{
					string Target = GetTarget(Result);
					if(Target.length() == 0)
					{
						RealmCrafter::SectorVector PickedVector(PickedX(), PickedY(), PickedZ(), Me->Position.SectorX, Me->Position.SectorZ);
						Me->SetDestination(PickedVector);
						if(Target.length() == 0)
						{
							ShowEntity(ClickMarkerEN);
							ClickMarkerTimer = MilliSecs();
							//PositionEntity(ClickMarkerEN, PickedX() + PickedNX(), PickedY() + PickedNY(), PickedZ() + PickedNZ());
							PositionEntity(ClickMarkerEN, PickedX(), PickedY(), PickedZ());
							AlignToVector(ClickMarkerEN, PickedNX(), Abs(PickedNY()), PickedNZ(), 2);
							MoveEntity(ClickMarkerEN, 0.0f, 0.085f, 0.0f);
						}
					}
				}
			}
		}else if(MouseWasDown == true) // Left mouse button was just clicked
		{
			// Was this a double click?
			bool IsDouble = false;
			if(MilliSecs() - LastLeftClick < 500)
				IsDouble = true;

			// Store time of click
			LastLeftClick = MilliSecs();
			MouseWasDown = false;

			// Pick a target unless this click was already used by the Select control
			if(UsedClick == false || Key_Select != Key_MoveTo)
			{
				SetPickModes(0);
				uint Result = CameraPick(Cam, MouseX(), MouseY());
				if(Result != 0)
				{
					string Target = GetTarget(Result);

					// Target is scenery
					if(Target.length() == 0)
					{
						if(EnableClickToMove)
						{
							if(IsDouble == false)
							{
								RealmCrafter::SectorVector PickedVector(PickedX(), PickedY(), PickedZ(), Me->Position.SectorX, Me->Position.SectorZ);
								Me->SetDestination(PickedVector);
								//CamYaw = 0.0f;
								//CamPitch = 0.0f;
								Me->IsRunning = RealmCrafter::Globals->AlwaysRun;
								if(Me->Mount != 0)
									Me->Mount->IsRunning = Me->IsRunning;
								RealmCrafter::Globals->AttackTarget = false;
								ShowEntity(ClickMarkerEN);
								ClickMarkerTimer = MilliSecs();
								//PositionEntity(ClickMarkerEN, PickedX() + PickedNX(), PickedY() + Abs(PickedNY()), PickedZ() + PickedNZ());
								PositionEntity(ClickMarkerEN, PickedX(), PickedY(), PickedZ());
								AlignToVector(ClickMarkerEN, PickedNX(), Abs(PickedNY()), PickedNZ(), 2);
								MoveEntity(ClickMarkerEN, 0.0f, 0.085f, 0.0f);

							}else
							{
								Me->IsRunning = true;
								if(Me->Mount != 0)
									Me->Mount->IsRunning = true;
							}
						}
					}

					// Do not run if(player has insufficient energy
					if(RealmCrafter::Globals->EnergyStat > -1 && Me->Mount == 0)
						if(Me->Attributes->Value[RealmCrafter::Globals->EnergyStat] <= 0)
							Me->IsRunning = false;
				}
			}
		}
	}// End mouse check

	// Attack selected actor with the attack key
	if(ControlHit(Key_Attack))
	{
		ActorInstance* AI = (ActorInstance*)PlayerTarget;
		if(AI != Me) // Stop hitting yourself.
		{
			if(AI != 0)
			{
				if(Me->Inventory->Items[SlotI_Weapon] != NULL)
				{
					if(Me->Inventory->Items[SlotI_Weapon]->Item->WeaponType != W_Ranged)
					{
						Me->SetDestination(AI->Position, true);

						//CamYaw = 0.0f;
						//CamPitch = 0.0f;
						Me->IsRunning = true;
						if(Me->Mount != 0)
							Me->Mount->IsRunning = true;
					}
				}
				else
				{
					Me->SetDestination(AI->Position, true);
					//CamYaw = 0.0f;
					//CamPitch = 0.0f;
					Me->IsRunning = true;
					if(Me->Mount != 0)
						Me->Mount->IsRunning = true;
				}

				// Check target is a combatant
				if(AI->Actor->Aggressiveness < 3)
				{
					// Check faction rating
					if(Me->FactionRatings[AI->HomeFaction] <= 150)
						RealmCrafter::Globals->AttackTarget = true;
				}
			}
		}
	}


	// Update selection highlight mesh
	if(PlayerTarget > 0)
	{
		ActorInstance* AI = (ActorInstance*)PlayerTarget;
		SetPickModes();
		uint Result = LinePick(EntityX(AI->CollisionEN), EntityY(AI->CollisionEN), EntityZ(AI->CollisionEN), 0.0f, -5000.0f, 0.0f);
		if(Result != 0)
		{
			//PositionEntity(ActorSelectEN, PickedX() + PickedNX(), PickedY() + Abs(PickedNY()), PickedZ() + PickedNZ());
			PositionEntity(ActorSelectEN, PickedX(), PickedY(), PickedZ());
			AlignToVector(ActorSelectEN, PickedNX(), Abs(PickedNY()), PickedNZ(), 2);
			MoveEntity(ActorSelectEN, 0.0f, 0.085f, 0.0f);
		}
	}
	else if(SceneryTarget != NULL)
	{
		SetPickModes();
		uint Result = LinePick(EntityX(SceneryTarget->EN), EntityY(SceneryTarget->EN), EntityZ(SceneryTarget->EN), 0.0f, -5000.0f, 0.0f);
		if(Result != 0)
		{
			ShowEntity(ActorSelectEN);
			//PositionEntity(ActorSelectEN, PickedX() + PickedNX(), PickedY() + Abs(PickedNY()), PickedZ() + PickedNZ());
			PositionEntity(ActorSelectEN, PickedX(), PickedY(), PickedZ());
			AlignToVector(ActorSelectEN, PickedNX(), Abs(PickedNY()), PickedNZ(), 2);
			MoveEntity(ActorSelectEN, 0.0f, 0.085f, 0.0f);
		}
	}
	else
	{
		HideEntity(ActorSelectEN);
	}

	// Update action bar quick-slot buttons
	for(int i = 0; i < 12; ++i)
	{
		// Execute this slot via F-key
		int FScan = 0;
		if(i < 10)
			FScan = 59 + i;
		else
			FScan = 77 + i;
		if(KeyHit(FScan))
		{
			int Slot = i;
			Slot += PlayerActionBar->GetPage() * 12;

			// Spell
			if(ActionBarSlots[Slot] < 0)
			{
				int Num, RechargeTime = 0;
				NGin::CString Pa;
				if(RealmCrafter::Globals->RequireMemorise)
				{
					Num = ActionBarSlots[Slot] + 10;
					RechargeTime = SpellsList[Me->KnownSpells[Me->MemorisedSpells[Num]]]->RechargeTime;
					Pa = StrFromShort(Me->KnownSpells[Me->MemorisedSpells[Num]]);
				}else
				{
					Num = ActionBarSlots[Slot] + 1000;
					RechargeTime = SpellsList[Me->KnownSpells[Num]]->RechargeTime;
					Pa = StrFromShort(Me->KnownSpells[Num]);
				}
				// Recharged
				if(Me->SpellCharge[Num] <= 0)
				{
					if(PlayerTarget > 0)
					{
						ActorInstance* AI = (ActorInstance*)PlayerTarget;
						Pa.AppendRealShort(AI->RuntimeID);
					}
					CRCE_Send(Connection, Connection, P_SpellUpdate, NGin::CString("F") + Pa, true);
					Me->SpellCharge[Num] = RechargeTime;
				}else // Not recharged
					Output(LanguageString[LS_AbilityNotRecharged], 255, 50, 50);
			}else if(ActionBarSlots[Slot] < 65535) // Item
				for(int ii = 0; ii < 50; ++ii)
					if(Me->Inventory->Items[ii] != 0)
						if(Me->Inventory->Items[ii]->Item->ID == ActionBarSlots[Slot])
						{
							PlaySound(GY_SBeep);
							UseItem(ii, 1);
							break;
						}
		}
	} // End of loop

	// Update compass direction
	UpdateCompass();

	// Radar
	UpdateRadar(Me);

	// Recharge spells
	if(MilliSecs() - LastSpellRecharge > 100)
	{
		if(RealmCrafter::Globals->RequireMemorise)
		{
			for(int i = 0; i < 10; ++i)
			{
				if(Me->SpellCharge[i] > 0)
				{
					Me->SpellCharge[i] -= 100;
					if(Me->SpellCharge[i] < 0)
						Me->SpellCharge[i] = 0;
				}
			}
		}else
		{
			for(int i = 0; i < 1000; ++i)
			{
				if(Me->SpellCharge[i] > 0)
				{
					Me->SpellCharge[i] -= 100;
					if(Me->SpellCharge[i] < 0)
						Me->SpellCharge[i] = 0;
				}
			}
		}
		LastSpellRecharge = MilliSecs();
	}

	// Update spell memorisation
	if(SMemorising != 0)
	{
		if(MilliSecs() - LastMemoriseUpdate > 100)
		{
			++MemoriseProgress;
			SMemorising->Value = (int)(((float)MemoriseProgress / 60.0f) * 100.0f);
			LastMemoriseUpdate = MilliSecs();
			// Done
			if(MemoriseProgress == 60)
			{
				GUIManager->Destroy(LMemorising);
				LMemorising = 0;
				GUIManager->Destroy(SMemorising);
				SMemorising = 0;
				Me->MemorisedSpells[MemoriseSlot] = MemoriseSpell;
			}
		}
	}

	// Right clicking while a spell is in the mouse slot removes it
	if(MouseDown(2) && RealmCrafter::Globals->CurrentMouseItem != NULL)
	{
		if(RealmCrafter::Globals->CurrentMouseItem->GetSpellID() != 65535)
		{
			delete RealmCrafter::Globals->CurrentMouseItem;
			RealmCrafter::Globals->CurrentMouseItem = NULL;
		}
	}

	if(Dialog::DialogList.Count() == 0 && TextInput::TextInputList.Count() == 0 && MouseDown(2) == false)
	{
		InDialog = false;
		FlushMouse();
	}

	// Update character stats window
	if(CharStatsVisible == true)
	{
		PlayerStats->Update(Me);
	}

	// Enable chat entry
	bool OthersVisible = InventoryVisible || CharStatsVisible || QuestLogVisible || SpellsVisible || (TextInput::TextInputList.Count() > 0);

	if((GY_EnterHit) && ChatEntry->Visible == false && OthersVisible == false && !TextBoxIsFocused())
	{
		ChatEntry->Visible = true;
		GUIManager->ControlFocus(ChatEntry);
		BChatDown = true;
	}else if(KeyHit(53) && ChatEntry->Visible == false && OthersVisible == false && !TextBoxIsFocused()) // Open chat entry with / key
	{
		ChatEntry->Visible = true;
		GUIManager->ControlFocus(ChatEntry);
		ChatEntry->Text = "";
		BChatDown = true;
	}else if(GY_EnterHit)//GY_TextFieldHit(ChatEntry->Component)) // Send message and disable chat entry
	{
		if(!TextBoxIsFocused() || GUIManager->ControlFocus() == ChatEntry)
		{
			if(ChatEntry->Text.length() > 0)
			{
				CRCE_Send(Connection, Connection, P_ChatMessage, NGin::CString(ChatEntry->Text.c_str()), true);
			}
			
			ChatEntry->Visible = false;
			ChatEntry->Text = "";
			BChatDown = false;
			FlushKeys();
			if(GUIManager->ControlFocus() == ChatEntry)
				GUIManager->ControlFocus(NULL);
		}
	}else if(ChatEntry->Visible == true && EscapeHit == true) // Close with escape
	{	
		EscapeHit = false;
		ChatEntry->Visible = false;
		ChatEntry->Text = "";
		if(GUIManager->ControlFocus() == ChatEntry)
			GUIManager->ControlFocus(NULL);
		BChatDown = false;
	}

	// Toggle inventory window
	if(EscapeHit && BInventoryDown == true && PlayerInventory->IsActiveWindow())
	{
		BInventoryDown = false;
		EscapeHit = false;
	}
	if((KeyHit(23) && ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && !TextBoxIsFocused()))// || WInventory->Visible() == false)
	{
		BInventoryDown = !InventoryVisible;
		PlaySound(GY_SBeep);
	}
	if(BInventoryDown != InventoryVisible)
	{
		InventoryVisible = BInventoryDown;
		// Show inventory
		if(InventoryVisible == true)
		{
			PlayerInventory->SetVisible(true);
			PlayerInventory->BringToFront();
			PlayerInventory->SetMoney(Money(Me->Gold));

			// Hide spellbook when showing inventory to prevent conflicts with the mouse slot
			BSpellsDown = false;
			PlayerSpells->SetVisible(false);
			if(SpellRemoveNum > -1)
			{
				WSpellRemove->Visible = false;
			}
			
			if(RealmCrafter::Globals->CurrentMouseItem != NULL)
			{
				if(RealmCrafter::Globals->CurrentMouseItem->GetSpellID() != 65535)
				{
					delete RealmCrafter::Globals->CurrentMouseItem;
					RealmCrafter::Globals->CurrentMouseItem = NULL;
				}
			}
			
			
		}else // Hide inventory
		{
			PlayerInventory->SetVisible(false);
		}
	}


	// Toggle quest log window
	if(EscapeHit && BQuestLogDown == true && PlayerQuestLog->IsActiveWindow())
	{
		BQuestLogDown = false;
		EscapeHit = false;
	}

	if(ControlLayout->Get("Quest Log") != NULL)
	{
		if((ControlHit(ControlLayout->Get("Quest Log")->ControlID) && ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && !TextBoxIsFocused()))
		{
			BQuestLogDown = !QuestLogVisible;
			PlaySound(GY_SBeep);
		}
	}
	if(BQuestLogDown != QuestLogVisible)
	{
		QuestLogVisible = BQuestLogDown;
		if(QuestLogVisible == true)
		{
			PlayerQuestLog->SetVisible(true);
			PlayerQuestLog->Update();
		}else
		{
			PlayerQuestLog->SetVisible(false);
		}
	}

	// Toggle party window
	if(EscapeHit && BPartyDown == true && PlayerParty->IsActiveWindow())
	{
		BPartyDown = false;
		EscapeHit = false;
	}

	if((KeyHit(25) && ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && !TextBoxIsFocused()))
	{
		BPartyDown = !PartyVisible;
		PlaySound(GY_SBeep);
	}
	if(BPartyDown != PartyVisible)
	{
		PartyVisible = BPartyDown;
		if(PartyVisible == true)
		{
			PlayerParty->SetVisible(true);
		}else
		{
			PlayerParty->SetVisible(false);
		}
	}

	// Toggle character stats window
	if(EscapeHit && BCharStatsDown == true && PlayerStats->IsActiveWindow())
	{
		BCharStatsDown = false;
		EscapeHit = false;
	}
	if((KeyHit(46) && ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && !TextBoxIsFocused()))// || GY_WindowClosed(WCharStats))
	{
		BCharStatsDown = !CharStatsVisible;
		PlaySound(GY_SBeep);
	}
	if(BCharStatsDown != CharStatsVisible)
	{
		CharStatsVisible = BCharStatsDown;
		if(CharStatsVisible == true)
		{
			PlayerStats->SetVisible(true);
		}else
		{
			PlayerStats->SetVisible(false);
		}
	}

	// Toggle spellbook
	if(EscapeHit && BSpellsDown == true && PlayerSpells->IsActiveWindow())
	{
		BSpellsDown = false;
		EscapeHit = false;
	}
	if((KeyHit(48) && ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && !TextBoxIsFocused()))
	{
		BSpellsDown = !SpellsVisible;
		PlaySound(GY_SBeep);
	}
	if(BSpellsDown != SpellsVisible)
	{
		SpellsVisible = BSpellsDown;
		// Show spellbook
		if(SpellsVisible == true)
		{
			PlayerSpells->SetVisible(true);
			PlayerSpells->BringToFront();

			// Turn inventory off when turning spellbook on to prevent conflicts with the mouse slot
			PlayerInventory->SetVisible(false);
			if(RealmCrafter::Globals->CurrentMouseItem != NULL)
			{
				delete RealmCrafter::Globals->CurrentMouseItem;
				RealmCrafter::Globals->CurrentMouseItem = NULL;
			}
			BInventoryDown = false;
		}else // Hide spellbook
		{
			// Hide any held cursor control
			if(RealmCrafter::Globals->CurrentMouseItem != NULL)
			{
				if(RealmCrafter::Globals->CurrentMouseItem->GetSpellID() != 65535)
				{
					delete RealmCrafter::Globals->CurrentMouseItem;
					RealmCrafter::Globals->CurrentMouseItem = NULL;
				}
			}

			PlayerSpells->SetVisible(false);
			if(SpellRemoveNum > -1)
			{
				WSpellRemove->Visible = false;
			}
		}
	}

	// Toggle help window
	if(EscapeHit && BHelpDown == true && PlayerHelp->IsActiveWindow())
	{
		BHelpDown = false;
		EscapeHit = false;
	}
	
	if(BHelpDown != HelpVisible)
	{
		HelpVisible = BHelpDown;
		if(HelpVisible == true)
		{
			PlayerHelp->SetVisible(true);
		}else
		{
			PlayerHelp->SetVisible(false);
		}
	}

	// Toggle large map
	if(EscapeHit && BMapDown == true && PlayerMap->IsActiveWindow())
	{
		BMapDown = false;
		EscapeHit = false;
	}
	if((KeyHit(50) && ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && !TextBoxIsFocused()))// || !WLargeMap->Visible())
	{
		BMapDown = !PlayerMapVisible;
		PlaySound(GY_SBeep);
	}

	if(BMapDown != PlayerMapVisible)
	{
		PlayerMapVisible = BMapDown;
		if(PlayerMapVisible)
			PlayerMap->SetVisible(true);
		else
			PlayerMap->SetVisible(false);
	}

	if(EscapeHit)
	{
		if(RealmCrafter::Globals->GameMenu->IsVisible())
		{
			RealmCrafter::Globals->GameMenu->Hide();
		}else
		{
			RealmCrafter::Globals->GameMenu->Show();
		}
	}

	// Attribute displays
	PlayerAttributeBars->Update( Me->Attributes );
}

void GameMenu_LogoutTimerStart(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
{
	RealmCrafter::Globals->QuitActive = true;
	//Me->Destination.X = EntityX(Me->CollisionEN);
	//Me->Destination.Z = EntityZ(Me->CollisionEN);
	Me->Destination = Me->Position;
	Me->Destination.FixValues();
}

void GameMenu_OptionsClick(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
{
	OptionsMenu->Show();
}

void GameMenu_Logout(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
{
	LogoutComplete = true;
}

void GameMenu_Quit(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
{
	LogoutComplete = true;
	QuitComplete = true;
}


