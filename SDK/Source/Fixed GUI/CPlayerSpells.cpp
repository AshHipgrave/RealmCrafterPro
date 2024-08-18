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
#include "CPlayerSpells.h"
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
	IPlayerSpells* CreatePlayerSpells(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerSpells(guiManager);
	}

	CPlayerSpells::CPlayerSpells(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), Invalidated(true), MouseOver(NULL), MouseOverTimer(0)
	{
		if(Globals->RequireMemorise)
			CurrentPage = -1;
		else
			CurrentPage = 0;

		ClickEvent = new SpellEvent();
		RightClickEvent = new SpellEvent();
	}

	CPlayerSpells::~CPlayerSpells()
	{

	}

	void CPlayerSpells::Update(SSpells* playerSpells, NGin::Math::Vector2 &mousePosition)
	{
		for(int i = 0; i < 10; ++i)
		{
			if(CurrentPage == -1)
			{
				if(InternalHandle.MemSpells[i] != playerSpells->MemSpells[i]
					|| InternalHandle.MemLevels[i] != playerSpells->MemLevels[i] || Invalidated)
				{
					InternalHandle.MemSpells[i] = playerSpells->MemSpells[i];
					InternalHandle.MemLevels[i] = playerSpells->MemLevels[i];

					Slots[i]->SetHeldSpell(InternalHandle.MemSpells[i]);
					if(InternalHandle.MemSpells[i] != 65535)
					{
						SpellLevels[i]->Text = std::string("Level: ") + std::toString(InternalHandle.MemLevels[i]);
						SpellNames[i]->Text = GetSpellNameFromID(InternalHandle.MemSpells[i]);
						Slots[i]->Visible = true;
					}else
					{
						SpellLevels[i]->Text = "";
						SpellNames[i]->Text = "";
						Slots[i]->Visible = false;
					}
				}
			}else
			{
				int it = CurrentPage * 10 + i;
				if(InternalHandle.Spells[it] != playerSpells->Spells[it]
					|| InternalHandle.Levels[it] != playerSpells->Levels[it] || Invalidated)
				{
					InternalHandle.Spells[it] = playerSpells->Spells[it];
					InternalHandle.Levels[it] = playerSpells->Levels[it];

					Slots[i]->SetHeldSpell(InternalHandle.Spells[it]);
					if(InternalHandle.Spells[it] != 65535)
					{
						SpellLevels[i]->Text = std::string("Level: ") + std::toString(InternalHandle.Levels[it]);
						SpellNames[i]->Text = GetSpellNameFromID(InternalHandle.Spells[it]);
						Slots[i]->Visible = true;
					}else
					{
						SpellLevels[i]->Text = "";
						SpellNames[i]->Text = "";
						Slots[i]->Visible = false;
					}
				}
			}
		}

		if(CurrentPage > 1 || (CurrentPage == 0 && Globals->RequireMemorise))
		{
			PreviousPage->Enabled = true;
		}else
		{
			PreviousPage->Enabled = false;
		}

		int MaxPage = 0;
		for(int i = 99; i >= 0; --i)
		{
			bool HasSpell = false;
			for(int f = 0; f < 10; ++f)
			{
				if(playerSpells->Spells[(i * 10) + f] != 65535)
				{
					HasSpell = true;
					break;
				}
			}

			if(HasSpell)
			{
				MaxPage = i;
				break;
			}
		}

		if(CurrentPage < MaxPage)
		{
			NextPage->Enabled = true;
		}else
		{
			NextPage->Enabled = false;
		}

		if(Invalidated)
		{
			if(CurrentPage == -1)
				PageLabel->Text = "Memorized";
			else
				PageLabel->Text = std::toString(CurrentPage + 1) + "/" + std::toString(MaxPage + 1);
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
				
				if((CurrentPage > -1 && InternalHandle.Spells[ID + CurrentPage * 10] != 65535) || (CurrentPage == -1 && InternalHandle.MemSpells[ID] != 65535))
				{
					unsigned short SpellID = InternalHandle.MemSpells[ID];

					if(CurrentPage >= 0)
					{
						SpellID = InternalHandle.Spells[ID + CurrentPage * 10];
					}

					std::vector<std::string> Lines;
					std::vector<Math::Color> Colors;

					Lines.push_back(std::string("Name: ") + GetSpellName(SpellID));
					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));

					if(CurrentPage == -1)
					{
						Lines.push_back(std::string("You must move this"));
						Colors.push_back(Math::Color(1.0f, 0.5f, 0.0f, 1.0f));

						Lines.push_back(std::string("to the ActionBar"));
						Colors.push_back(Math::Color(1.0f, 1.5f, 0.0f, 1.0f));
					}

					Lines.push_back(GetSpellDescription(SpellID));
					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));


					Globals->CurrentMouseToolTip = CreateMouseToolTip(GUIManager, MouseOver, Lines, Colors);
				}
			}
		}

		Invalidated = false;
	}

	void CPlayerSpells::Initialize()
	{
		Window = GUIManager->CreateWindow("SpellsWindow::Window", Math::Vector2(0.1f, 0.1f), Math::Vector2(0.6f, 0.5f));
		Window->Text = "Spells";
		PreviousPage = GUIManager->CreateButton("SpellsWindow::PreviousButton", Math::Vector2(0.01f, 0.94f), Math::Vector2(0.05f, 0.05f));
		NextPage = GUIManager->CreateButton("SpellsWindow::NextButton", Math::Vector2(0.94f, 0.94f), Math::Vector2(0.05f, 0.05f));
		PageLabel = GUIManager->CreateLabel("SpellsWindow::PageLabel", Math::Vector2(0.5f, 0.94f), Math::Vector2(0, 0));

		PreviousPage->Text = "<<";
		NextPage->Text = ">>";
		PageLabel->Text = "Memorized";

		PageLabel->ForeColor = Math::Color(255, 255, 255);
		PageLabel->Align = TextAlign_Center;

		PreviousPage->Parent = Window;
		NextPage->Parent = Window;
		PageLabel->Parent = Window;

		PreviousPage->Click()->AddEvent(this, &CPlayerSpells::PreviousPage_Click);
		NextPage->Click()->AddEvent(this, &CPlayerSpells::NextPage_Click);

		float X = 0.01f;
		float Y = 0.05f;

		for(int i = 0; i < 10; ++i)
		{
			// Create gadgets
			if(i == 5)
			{
				X = 0.51f;
				Y = 0.05f; // Move sideways to right side of window
			}
			Slots[i] = CreateItemButton(GUIManager, std::string("SpellsWindow::Slot") + std::toString(i), Math::Vector2(X + 0.39f, Y), Math::Vector2(0.1f, 0.15f));
			SpellNames[i] = GUIManager->CreateLabel(std::string("SpellsWindow::SlotName") + std::toString(i), Math::Vector2(X, Y), Math::Vector2(0, 0));
			SpellLevels[i] = GUIManager->CreateLabel(std::string("SpellsWindow::SlotLevel") + std::toString(i), Math::Vector2(X, Y + 0.06f), Math::Vector2(0, 0));

			Slots[i]->Text = "";
			SpellNames[i]->Text = "";
			SpellLevels[i]->Text = "";


			Slots[i]->Parent = Window;
			SpellNames[i]->Parent = Window;
			SpellLevels[i]->Parent = Window;

			Slots[i]->SetBackgroundImage("Data\\Textures\\GUI\\Backpack.bmp");

			Slots[i]->Click()->AddEvent(this, &CPlayerSpells::Slots_Click);
			Slots[i]->RightClick()->AddEvent(this, &CPlayerSpells::Slots_RightClick);
			Slots[i]->MouseEnter()->AddEvent(this, &CPlayerSpells::Slots_MouseEnter);

			Y += 0.175f;
		}

		GUIManager->SetProperties("SpellsWindow");
	}

	void CPlayerSpells::Slots_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		// A bug?
		if(control == NULL || control->GetType() != Type("ZITB", "CItemButton SDK Item/Spell container"))
			return;
		IItemButton* Control = static_cast<IItemButton*>(control);

		int ID = GetSlot(Control);

		SpellEventArgs E(ID + (CurrentPage * 10), CurrentPage, ID);
		ClickEvent->Execute(this, &E);
	}

	void CPlayerSpells::Slots_RightClick(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		// A bug?
		if(control == NULL || control->GetType() != Type("ZITB", "CItemButton SDK Item/Spell container"))
			return;
		IItemButton* Control = static_cast<IItemButton*>(control);

		int ID = GetSlot(Control);

		SpellEventArgs E(ID + (CurrentPage * 10), CurrentPage, ID);
		RightClickEvent->Execute(this, &E);
	}

	void CPlayerSpells::Slots_MouseEnter(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
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
		}
	}

	void CPlayerSpells::NextPage_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		int MaxPage = 0;
		for(int i = 100; i >= 0; --i)
		{
			bool HasSpell = false;
			for(int f = 0; f < 10; ++f)
			{
				if(InternalHandle.Spells[i + f] != 65535)
				{
					HasSpell = true;
					break;
				}
			}

			if(HasSpell)
			{
				MaxPage = i;
				break;
			}
		}

		if(CurrentPage < MaxPage)
		{
			++CurrentPage;
			Invalidated = true;
		}
	}

	void CPlayerSpells::PreviousPage_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		if(CurrentPage > (Globals->RequireMemorise ? -1 : 0))
		{
			--CurrentPage;
			Invalidated = true;
		}
	}

	NGin::GUI::EventHandler* CPlayerSpells::Closed()
	{
		return Window->Closed();
	}

	void CPlayerSpells::SetVisible(bool visible)
	{
		Window->Visible = visible;
		if(visible)
			Invalidated = true;
	}

	bool CPlayerSpells::GetVisible()
	{
		return Window->Visible;
	}

	int CPlayerSpells::GetSlot(IItemButton* button )
	{
		for(int i = 0; i < 10; ++i)
		{
			if(Slots[i] == button)
				return i;
		}

		return -1;
	}

	bool CPlayerSpells::IsActiveWindow()
	{
		return Window->IsActiveWindow();
	}

	void CPlayerSpells::BringToFront()
	{
		Window->BringToFront();
	}

	SpellEvent* CPlayerSpells::Click()
	{
		return ClickEvent;
	}

	SpellEvent* CPlayerSpells::RightClick()
	{
		return RightClickEvent;
	}
}