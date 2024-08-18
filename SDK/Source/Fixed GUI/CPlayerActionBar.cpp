//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#include "CPlayerActionBar.h"
#include "IMouseItem.h"
#include "IMouseToolTip.h"

#ifdef CreateWindow
#undef CreateWindow
#endif

using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;

namespace RealmCrafter
{
	IPlayerActionBar* CreatePlayerActionBar(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerActionBar(guiManager);
	}


	CPlayerActionBar::CPlayerActionBar(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), Invalidated(true), CurrentPage(0), MouseOver(NULL), MouseOverTimer(0)
	{
		ClickEvent = new ActionBarEvent();
		RightClickEvent = new ActionBarEvent();
	}

	CPlayerActionBar::~CPlayerActionBar()
	{

	}

	void CPlayerActionBar::Update(SActionBar* playerBar, NGin::Math::Vector2 &mousePosition)
	{
		// Update displayed items
		for(int i = 0; i < 12; ++i)
		{
			int it = CurrentPage * 12 + i;

			if(InternalHandle.Spells[it] != playerBar->Spells[it]
				|| InternalHandle.Items[it] != playerBar->Items[it]
				|| InternalHandle.ItemAmounts[it] != playerBar->ItemAmounts[it]
				|| InternalHandle.RechargeTimes[it] != playerBar->RechargeTimes[it] || Invalidated)
			{
				InternalHandle.Spells[it] = playerBar->Spells[it];
				InternalHandle.Items[it] = playerBar->Items[it];
				InternalHandle.ItemAmounts[it] = playerBar->ItemAmounts[it];
				InternalHandle.RechargeTimes[it] = playerBar->RechargeTimes[it];
				
				if(InternalHandle.Spells[it] != 65535)
				{
					Slots[i]->SetHeldSpell(InternalHandle.Spells[it]);
					Slots[i]->SetTimerValue(((float)InternalHandle.RechargeTimes[it]));
				}else if(InternalHandle.Items[it] != 65535)
				{
					Slots[i]->SetHeldItem(InternalHandle.Items[it], InternalHandle.ItemAmounts[it]);
					Slots[i]->SetTimerValue(-1.0f);
				}else
				{
					Slots[i]->SetHeldItem(65535, 0);
					Slots[i]->SetTimerValue(-1.0f);
				}
			}
		}

		if(MouseOver != NULL)
		{
			// Check mouse is really over gadget
			if(Globals->CurrentMouseToolTip != NULL && Globals->CurrentMouseToolTip->GetParent() == MouseOver)
			{
				if(mousePosition.X < MouseOver->GlobalLocation().X
					|| mousePosition.Y < MouseOver->GlobalLocation().Y
					|| mousePosition.X > MouseOver->GlobalLocation().X + MouseOver->Size.X
					|| mousePosition.Y > MouseOver->GlobalLocation().Y + MouseOver->Size.Y)
				{
					delete Globals->CurrentMouseToolTip;
					Globals->CurrentMouseToolTip = NULL;
					MouseOver = NULL;
				}
			}else if(Globals->CurrentMouseItem == NULL && (MilliSecs() - MouseOverTimer) > 500)
			{
				if(mousePosition.X < MouseOver->GlobalLocation().X
					|| mousePosition.Y < MouseOver->GlobalLocation().Y
					|| mousePosition.X > MouseOver->GlobalLocation().X + MouseOver->Size.X
					|| mousePosition.Y > MouseOver->GlobalLocation().Y + MouseOver->Size.Y)
				{
					if(Globals->CurrentMouseToolTip != NULL)
					{
						delete Globals->CurrentMouseToolTip;
						Globals->CurrentMouseToolTip = NULL;
					}

					MouseOver = NULL;
					return;
				}

				if(Globals->CurrentMouseToolTip != NULL)
				{
					delete Globals->CurrentMouseToolTip;
					Globals->CurrentMouseToolTip = NULL;
				}

				int ID = GetSlot(MouseOver);
				int Index = ID + (CurrentPage * 12);

				if(InternalHandle.Items[Index] != 65535 || InternalHandle.Spells[Index] != 65535)
				{
					std::vector<std::string> Lines;
					std::vector<Math::Color> Colors;

					if(InternalHandle.Spells[Index] != 65535)
					{
						Lines.push_back(GetSpellName(InternalHandle.Spells[Index]));
					}else if(InternalHandle.Items[Index] != 65535)
					{
						Lines.push_back(GetItemName(InternalHandle.Items[Index]));
					}

					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));

					Globals->CurrentMouseToolTip = CreateMouseToolTip(GUIManager, MouseOver, Lines, Colors);
				}
			}
		}


		Invalidated = false;
	}

	void CPlayerActionBar::Initialize()
	{
 		ActionEN = GUIManager->CreatePictureBox(std::string("ActionBar::Background"), Math::Vector2(0, 1.0f - (1.0f / 6.0f)), Math::Vector2(1, 1.0f / 6.0f));
		if(!ActionEN->SetImage(std::string("Data\\Textures\\GUI\\Action Bar.bmp"), 0xff000000))
			ActionEN->SetImage(std::string("Data\\Textures\\GUI\\Action Bar.png"), 0xff000000);
 
 		XPEN = GUIManager->CreatePictureBox(std::string("ActionBar::XPEN"), Math::Vector2(0.179199218f, 1.0f - (1.0f / 6.0f)), Math::Vector2(0.641601562f, 1.0f / 6.0f));
		if(XPEN->SetImage(std::string("Data\\Textures\\GUI\\Action Bar XP.bmp"), 0xff000000))
			XPEN->SetImage(std::string("Data\\Textures\\GUI\\Action Bar XP.png"), 0xff000000);
 
 		BChat = GUIManager->CreateButton("", Math::Vector2(0.62890625f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 		BMap = GUIManager->CreateButton("", Math::Vector2(0.666015625f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 		BInventory = GUIManager->CreateButton("", Math::Vector2(0.702148437f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 		BSpells = GUIManager->CreateButton("", Math::Vector2(0.739257812f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 		BCharStats = GUIManager->CreateButton("", Math::Vector2(0.77734375f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 		BQuestLog = GUIManager->CreateButton("", Math::Vector2(0.813476562f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 		BParty = GUIManager->CreateButton("", Math::Vector2(0.850585937f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 		BHelp = GUIManager->CreateButton("", Math::Vector2(0.88671875f + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 
		BChat->SetUpImage(std::string("Data\\Textures\\GUI\\Chat.bmp"));
 		BMap->SetUpImage(std::string("Data\\Textures\\GUI\\Map.bmp"));
 		BInventory->SetUpImage(std::string("Data\\Textures\\GUI\\Inventory.bmp"));
 		BSpells->SetUpImage(std::string("Data\\Textures\\GUI\\Abilities.bmp"));
 		BCharStats->SetUpImage(std::string("Data\\Textures\\GUI\\Character.bmp"));
 		BQuestLog->SetUpImage(std::string("Data\\Textures\\GUI\\Quests.bmp"));
 		BParty->SetUpImage(std::string("Data\\Textures\\GUI\\Party.bmp"));
 		BHelp->SetUpImage(std::string("Data\\Textures\\GUI\\Help.bmp"));
 
 		BChat->Name = "ActionBar::ChatButton";
 		BMap->Name = "ActionBar::MapButton";
 		BInventory->Name = "ActionBar::InventoryButton";
 		BSpells->Name = "ActionBar::SpellsButton";
 		BCharStats->Name = "ActionBar::StatsButton";
 		BQuestLog->Name = "ActionBar::QuestLogButton";
 		BParty->Name = "ActionBar::PartyButton";
 		BHelp->Name = "ActionBar::HelpButton";
 
 		// Action bar quick-slot buttons
 		PreviousPage = GUIManager->CreateButton("ActionBar::PreviousBar", Math::Vector2(0.53f, 0.9415f - 0.001364583334f), Math::Vector2(0.02f, 0.015f));
 		NextPage = GUIManager->CreateButton("ActionBar::NextBar", Math::Vector2(0.53f, 0.9655f - 0.001364583334f), Math::Vector2(0.02f, 0.015f));
 		PreviousPage->Text = "";
 		NextPage->Text = "";
 		PreviousPage->Click()->AddEvent(this, &CPlayerActionBar::PreviousPage_Click);
 		NextPage->Click()->AddEvent(this, &CPlayerActionBar::NextPage_Click);
 
 		for(int i = 0; i < 12; ++i)
 		{
			Slots[i] = CreateItemButton(GUIManager, std::string("ActionBar::Slot") + std::toString(i), Vector2(0.089867187f + (((float)i) * 0.036132812f) + 0.003f, 0.9415f), Math::Vector2(0.033203125f - 0.006f, 0.044270833f - 0.008f));
 			Slots[i]->SetBackgroundImage("Data\\Textures\\GUI\\Backpack.bmp");
 
 			Slots[i]->Click()->AddEvent(this, &CPlayerActionBar::Slots_Click);
 			Slots[i]->RightClick()->AddEvent(this, &CPlayerActionBar::Slots_RightClick);
			Slots[i]->MouseEnter()->AddEvent(this, &CPlayerActionBar::Slots_MouseEnter);
 		}
 
 		GUIManager->SetProperties("ActionBar");
		UpdateXPBar(0);
	}

	void CPlayerActionBar::Slots_MouseEnter(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		// A bug?
		if(control == NULL || control->GetType() != Type("ZITB", "CItemButton SDK Item/Spell container"))
			return;
		IItemButton* Control = static_cast<IItemButton*>(control);

		MouseOver = NULL;
		MouseOverTimer = MilliSecs();

		// Check if a tooltip can be made
		if(Globals->CurrentMouseItem == NULL)
		{
			if(Globals->CurrentMouseToolTip != NULL)
			{
				delete Globals->CurrentMouseToolTip;
				Globals->CurrentMouseToolTip = NULL;
			}

			MouseOver = Control;
			return;
		}
	}

	void CPlayerActionBar::Slots_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		// A bug?
		if(control == NULL || control->GetType() != Type("ZITB", "CItemButton SDK Item/Spell container"))
			return;
		IItemButton* Control = static_cast<IItemButton*>(control);

		int ID = GetSlot(Control);

		ActionBarEventArgs E(ID + (CurrentPage * 12), CurrentPage, ID);
		ClickEvent->Execute(this, &E);
	}

	void CPlayerActionBar::Slots_RightClick(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		// A bug?
		if(control == NULL || control->GetType() != Type("ZITB", "CItemButton SDK Item/Spell container"))
			return;
		IItemButton* Control = static_cast<IItemButton*>(control);

		int ID = GetSlot(Control);

		ActionBarEventArgs E(ID + (CurrentPage * 12), CurrentPage, ID);
		RightClickEvent->Execute(this, &E);
	}

	void CPlayerActionBar::NextPage_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		if(CurrentPage < 2)
		{
			++CurrentPage;
			Invalidated = true;
		}
	}

	void CPlayerActionBar::PreviousPage_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		if(CurrentPage > 0)
		{
			--CurrentPage;
			Invalidated = true;
		}
	}

	int CPlayerActionBar::GetSlot(IItemButton* button )
	{
		for(int i = 0; i < 12; ++i)
		{
			if(Slots[i] == button)
				return i;
		}

		return -1;
	}

	void CPlayerActionBar::UpdateXPBar(char xpBarLevel)
	{
		float Amount = ((float)xpBarLevel) / 255.0f;
		XPEN->Size = Math::Vector2(Amount * ActionEN->Size.X, ActionEN->Size.Y);
		XPEN->MaxTexCoord = Math::Vector2(Amount, 1.0f);
	}

	void CPlayerActionBar::SetVisible(bool visible)
	{
		for(int i = 0; i < 12; ++i)
			Slots[i]->Visible = visible;

		NextPage->Visible = visible;
		PreviousPage->Visible = visible;
		XPEN->Visible = visible;
		ActionEN->Visible = visible;
		BChat->Visible = visible;
		BMap->Visible = visible;
		BInventory->Visible = visible;
		BSpells->Visible = visible;
		BCharStats->Visible = visible;
		BQuestLog->Visible = visible;
		BParty->Visible = visible;
		BHelp->Visible = visible;
	}

	int CPlayerActionBar::GetPage()
	{
		return CurrentPage;
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BChat_Click()
	{
		return BChat->Click();
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BMap_Click()
	{
		return BMap->Click();
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BInventory_Click()
	{
		return BInventory->Click();
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BSpells_Click()
	{
		return BSpells->Click();
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BCharStats_Click()
	{
		return BCharStats->Click();
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BQuestLog_Click()
	{
		return BQuestLog->Click();
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BParty_Click()
	{
		return BParty->Click();
	}

	NGin::GUI::EventHandler* CPlayerActionBar::BHelp_Click()
	{
		return BHelp->Click();
	}


	ActionBarEvent* CPlayerActionBar::Click()
	{
		return ClickEvent;
	}

	ActionBarEvent* CPlayerActionBar::RightClick()
	{
		return RightClickEvent;
	}
}