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
#include "CPlayerInventory.h"
#include "IMouseItem.h"
#include "IMouseToolTip.h"
#include "..\Custom GUI\CItemButtonHost.h"

#ifdef CreateWindow
#undef CreateWindow
#endif

using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;

namespace RealmCrafter
{
	IPlayerInventory* CreatePlayerInventory(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerInventory(guiManager);
	}

	CPlayerInventory::CPlayerInventory(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), MouseOverTimer(0), MouseOver(NULL)
	{

	}

	CPlayerInventory::~CPlayerInventory()
	{

	}

	void CPlayerInventory::Update(SInventory* playerInventory, NGin::Math::Vector2 &mousePosition)
	{
		// NOTE: The item arrays go up to 50 (possibly for some sort of legacy support?) but the real limit is 32 backpack slots.
		for(int i = 0; i <= SlotI_Backpack + 31; ++i)
		{
			if(playerInventory->Items[i] != InternalHandle.Items[i]
				|| playerInventory->Amounts[i] != InternalHandle.Amounts[i])
			{
				InternalHandle.Items[i] = playerInventory->Items[i];
				InternalHandle.Amounts[i] = playerInventory->Amounts[i];

				Slots[i]->SetHeldItem(playerInventory->Items[i], playerInventory->Amounts[i]);
			}
		}

		if(Globals->CurrentMouseItem != NULL && Globals->CurrentMouseItem->GetItemID() != 65535)
		{
			UseButton->Enabled = true;
			DropButton->Enabled = true;
		}else
		{
			UseButton->Enabled = false;
			DropButton->Enabled = false;
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

				if(InternalHandle.Items[ID] != 65535)
				{
					unsigned short ItemID = InternalHandle.Items[ID];
					std::vector<std::string> Lines;
					std::vector<Math::Color> Colors;

					Lines.push_back(std::string("Name: ") + GetItemName(ItemID));
					Lines.push_back(std::string("Type: ") + GetItemTypeString(ItemID));

					if(GetItemTakesDamage(ItemID))
						Lines.push_back(std::string("Health: ") + std::toString(GetItemHealth(ID)) + "%");
					else
						Lines.push_back(std::string("Health: Indestructible"));

					Lines.push_back(std::string("Value: ") + std::toString(GetItemValue(ItemID)));
					Lines.push_back(std::string("Mass: ") + std::toString(GetItemMass(ItemID)));

					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));
					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));
					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));
					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));
					Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));

					if(GetItemStackable(ItemID))
					{
						Lines.push_back(std::string("Stackable"));
						Colors.push_back(Math::Color(0.0f, 1.0f, 0.0f, 1.0f));
					}
					else
					{
						Lines.push_back(std::string("Cannot Stack"));
						Colors.push_back(Math::Color(1.0f, 0.0f, 0.0f, 1.0f));
					}

					switch(GetItemType(ItemID))
					{
						case I_Weapon:
							{
								int Damage = GetItemWeaponDamage(ItemID);
								std::string DamageType = GetItemWeaponDamageType(ItemID);
								std::string WeaponType = GetItemWeaponType(ItemID);

								Lines.push_back(std::string("Damage: ") + std::toString(Damage));
								Lines.push_back(std::string("Damage Type: ") + DamageType);
								Lines.push_back(std::string("Weapon Type: ") + WeaponType);

								Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));
								Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));
								Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));

								break;
							}
						case I_Armour:
							{
								Lines.push_back(std::string("Armour Level: ") + std::toString(GetItemArmourLevel(ItemID)));
								Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));

								break;
							}
						case I_Ingredient:
						case I_Potion:
							{
								Lines.push_back(std::string("Effects Last: ") + std::toString(GetItemEatEffects(ItemID)) + "s");
								Colors.push_back(Math::Color(1.0f, 1.0f, 1.0f, 1.0f));

								break;
							}
					}

					std::string ExclusiveRace = GetItemExclusiveRace(ItemID);
					std::string ExclusiveClass = GetItemExclusiveClass(ItemID);
					bool RaceOK = GetPlayerIsExclusiveRace(ItemID);
					bool ClassOK = GetPlayerIsExclusiveClass(ItemID);

					if(ExclusiveRace.size() > 0)
					{
						Lines.push_back(std::string("Race: ") + ExclusiveRace);

						if(RaceOK)
							Colors.push_back(Math::Color(0.0f, 1.0f, 0.0f, 1.0f));
						else
							Colors.push_back(Math::Color(1.0f, 0.0f, 0.0f, 1.0f));
					}

					if(ExclusiveClass.size() > 0)
					{
						Lines.push_back(std::string("Class: ") + ExclusiveClass);

						if(ClassOK)
							Colors.push_back(Math::Color(0.0f, 1.0f, 0.0f, 1.0f));
						else
							Colors.push_back(Math::Color(1.0f, 0.0f, 0.0f, 1.0f));
					}

					Globals->CurrentMouseToolTip = CreateMouseToolTip(GUIManager, MouseOver, Lines, Colors);
				}
			}
		}
	}

	void CPlayerInventory::CreateInventoryButton(int slotID, std::string backgroundImage)
	{
		Slots[slotID] = RealmCrafter::CreateItemButton(GUIManager, std::string("Inventory::Slot") + std::toString(slotID), Vector2(0, 0), Vector2(0.1f, 0.1f));
		Slots[slotID]->Parent = Window;

		Slots[slotID]->Click()->AddEvent(this, &CPlayerInventory::Slots_Click);
		Slots[slotID]->RightClick()->AddEvent(this, &CPlayerInventory::Slots_RightClick);
		Slots[slotID]->MouseEnter()->AddEvent(this, &CPlayerInventory::Slots_MouseEnter);

		Slots[slotID]->SetBackgroundImage(backgroundImage);
	}

	void CPlayerInventory::Initialize()
	{
		Window = GUIManager->CreateWindow("Inventory::Window", Math::Vector2(0, 0), Math::Vector2(1, 1));
		MoneyLabel = GUIManager->CreateLabel("Inventory::MoneyLabel", Math::Vector2(0, 0), Math::Vector2(0, 0));
		DropButton = GUIManager->CreateButton("Inventory::DropButton", Math::Vector2(0, 0), Math::Vector2(1, 1));
		UseButton = GUIManager->CreateButton("Inventory::UseButton", Math::Vector2(0, 0), Math::Vector2(1, 1));

		Window->Text = "Inventory";
		MoneyLabel->Text = "00000000000000000000000000000000000000000000000000000000";
		DropButton->Text = "Drop";
		UseButton->Text = "Use";

		MoneyLabel->Parent = Window;
		DropButton->Parent = Window;
		UseButton->Parent = Window;

		DropButton->Enabled = false;
		UseButton->Enabled = false;

		DropButton->Click()->AddEvent(this, &CPlayerInventory::DropButton_Click);
		UseButton->Click()->AddEvent(this, &CPlayerInventory::UseButton_Click);
// 
// 		WInventory->Closed()->AddEvent(&WInventory_Closed);

		CreateInventoryButton(SlotI_Weapon, "Data\\Textures\\GUI\\Weapon.bmp");
		CreateInventoryButton(SlotI_Shield, "Data\\Textures\\GUI\\Shield.bmp");
		CreateInventoryButton(SlotI_Hat, "Data\\Textures\\GUI\\Hat.bmp");
		CreateInventoryButton(SlotI_Chest, "Data\\Textures\\GUI\\Chest.bmp");
		CreateInventoryButton(SlotI_Hand, "Data\\Textures\\GUI\\Hand.bmp");
		CreateInventoryButton(SlotI_Belt, "Data\\Textures\\GUI\\Belt.bmp");
		CreateInventoryButton(SlotI_Legs, "Data\\Textures\\GUI\\Legs.bmp");
		CreateInventoryButton(SlotI_Feet, "Data\\Textures\\GUI\\Feet.bmp");
		CreateInventoryButton(SlotI_Ring1, "Data\\Textures\\GUI\\Ring.bmp");
		CreateInventoryButton(SlotI_Ring2, "Data\\Textures\\GUI\\Ring.bmp");
		CreateInventoryButton(SlotI_Ring3, "Data\\Textures\\GUI\\Ring.bmp");
		CreateInventoryButton(SlotI_Ring4, "Data\\Textures\\GUI\\Ring.bmp");
		CreateInventoryButton(SlotI_Amulet1, "Data\\Textures\\GUI\\Amulet.bmp");
		CreateInventoryButton(SlotI_Amulet2, "Data\\Textures\\GUI\\Amulet.bmp");

		for(int i = SlotI_Backpack; i <= SlotI_Backpack + 31; ++i)
			CreateInventoryButton(i, "Data\\Textures\\GUI\\Backpack.bmp");

		GUIManager->SetProperties("Inventory");
	}

	void CPlayerInventory::Slots_MouseEnter(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
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

	void CPlayerInventory::Slots_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		// A bug?
		if(control->GetType() != Type("ZITB", "CItemButton SDK Item/Spell container"))
			return;
		IItemButton* Control = (IItemButton*)control;

		// User is already holding an item
		if(Globals->CurrentMouseItem != NULL)
		{
			// Its not a valid item
			if(Globals->CurrentMouseItem->GetItemID() == 65535)
				return;

			// If its from the SDK
			if(Globals->CurrentMouseItem->GetSourceLocation() == MIS_SDK)
			{
				SDK::CItemButtonHost* ItemHost = reinterpret_cast<SDK::CItemButtonHost*>(Globals->CurrentMouseItem->GetSourceData());
				if(ItemHost != NULL)
				{
					ItemHost->DraggedToInventory();
				}
				
				// Free handle
				delete Globals->CurrentMouseItem;
				Globals->CurrentMouseItem = NULL;
				return;
			}

			// Its not coming from the inventory, no item can do this.
			// ActionBar items are already here
			if(Globals->CurrentMouseItem->GetSourceLocation() != MIS_Inventory)
				return;

			unsigned short PlacementID = Globals->CurrentMouseItem->GetItemID();
			unsigned short PlacementAmount = Globals->CurrentMouseItem->GetItemAmount();
			IMouseItem* MouseItem = Globals->CurrentMouseItem;

// 			// Is it the same as what we have?
// 			if(PlacementID == Control->GetHeldItem())
// 			{
// 				// Pick up another one if control is pressed
// 				if(ControlDown(29) || ControlDown(157))
// 				{
// 					CurrentMouseItem = new CMouseItem(GUIManager, PlacementID, 65535, PlacementAmount + 1, MIS_Inventory, Control);
// 
// // 					if(Control->GetHeldAmount() > 1)
// // 						Control->SetHeldItem(Control->GetHeldItem(), Control->GetHeldAmount() - 1);
// // 					else
// // 						Control->SetHeldItem(65535, 0);
// 					InventorySwap(
// 
// 					delete MouseItem;
// 					return;
// 				}else
// 				{
// 					Control->SetHeldItem(PlacementID, PlacementAmount + Control->GetHeldAmount());
// 					CurrentMouseItem = NULL;
// 					delete MouseItem;
// 					return;
// 				}
// 			}

			// Do we need to switch with an item already in the slot?
			// Bug
			if(Control->GetHeldSpell() != 65535)
				return;

			if(Control->GetHeldItem() != 65535)
			{
				Globals->CurrentMouseItem = CreateMouseItem(GUIManager, Control->GetHeldItem(), 65535, Control->GetHeldAmount(), MIS_Inventory, Control);
			
				int SlotFrom = GetSlot(reinterpret_cast<IItemButton*>(MouseItem->GetSourceData()));
				int SlotTo = GetSlot(Control);
				if(SlotFrom != -1)
					InternalHandle.Items[SlotFrom] = 65535;
				if(SlotTo != -1)
					InternalHandle.Items[SlotTo] = 65535;
				InventorySwap(SlotFrom, SlotTo);
			}else
			{
				Globals->CurrentMouseItem = NULL;
			
				int SlotFrom = GetSlot(reinterpret_cast<IItemButton*>(MouseItem->GetSourceData()));
				int SlotTo = GetSlot(Control);
				if(SlotFrom != -1)
					InternalHandle.Items[SlotFrom] = 65535;
				if(SlotTo != -1)
					InternalHandle.Items[SlotTo] = 65535;
				InventorySwap(SlotFrom, SlotTo);
			}

			delete MouseItem;
		}else
		{
			if(Control->GetHeldItem() != 65535)
			{
// 				// Holding control (pick up/add one)
// 				if(ControlDown(29) || ControlDown(157))
// 				{
// 					CurrentMouseItem = new CMouseItem(GUIManager, Control->GetHeldItem(), 65535, 1, MIS_Inventory, Control);
// 
// 					if(Control->GetHeldAmount() > 1)
// 						Control->SetHeldItem(Control->GetHeldItem(), Control->GetHeldAmount() - 1);
// 					else
// 						Control->SetHeldItem(65535, 0);
// 				}else
				{
					Globals->CurrentMouseItem = CreateMouseItem(GUIManager, Control->GetHeldItem(), 65535, Control->GetHeldAmount(), MIS_Inventory, Control);


// 					int SlotTo = GetSlot(Control);
// 					if(SlotTo != -1)
// 						InternalHandle.Items[SlotTo] = 65535;

					Control->SetHeldItem(65535, 0);
				}
			}
		}
	}

	void CPlayerInventory::Slots_RightClick(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		int SlotID = GetSlot((IItemButton*)control);
		if(SlotID > -1)
			UseItem(SlotID, 1);
	}

	void CPlayerInventory::UseButton_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		if(Globals->CurrentMouseItem == NULL)
			return;

		Slots_RightClick(reinterpret_cast<NGin::GUI::IControl*>(Globals->CurrentMouseItem->GetSourceData()), e);
	}

	void CPlayerInventory::DropButton_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e)
	{
		// Unusable
		if(Globals->CurrentMouseItem == NULL || Globals->CurrentMouseItem->GetItemID() == 65535)
			return;

		int SlotID = GetSlot(reinterpret_cast<IItemButton*>(Globals->CurrentMouseItem->GetSourceData()));
		if(SlotID > -1)
			InventoryDrop(SlotID, Globals->CurrentMouseItem->GetItemAmount());
	}

	NGin::GUI::EventHandler* CPlayerInventory::Closed()
	{
		return Window->Closed();
	}

	void CPlayerInventory::SetVisible(bool visible)
	{
		Window->Visible = visible;
	}

	bool CPlayerInventory::GetVisible()
	{
		return Window->Visible;
	}

	int CPlayerInventory::GetSlot(IItemButton* button )
	{
		for(int i = 0; i < 50; ++i)
		{
			if(Slots[i] == button)
				return i;
		}

		return -1;
	}

	bool CPlayerInventory::IsActiveWindow()
	{
		return Window->IsActiveWindow();
	}

	void CPlayerInventory::BringToFront()
	{
		Window->BringToFront();
	}

	void CPlayerInventory::SetMoney( std::string money )
	{
		MoneyLabel->Text = money;
	}
}