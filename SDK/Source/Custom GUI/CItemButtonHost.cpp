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
#pragma once

#include "CItemButtonHost.h"
#include <windows.h>

namespace RealmCrafter
{
	namespace SDK
	{
		CItemButtonHost::CItemButtonHost(IControlHostManager* manager, IControlHost* master)
			: IControlHost(manager, master), Handle(NULL), CanDragToInventory(false), CanDragFromInventory(false), CanDragFromSpells(false)
		{
			if(Globals->TimerManager != NULL)
			{
				TooltipTimer = Globals->TimerManager->CreateTimer(500);
				TooltipTimer->AutoReset(false);
				TooltipTimer->Elapsed()->AddEvent(this, &CItemButtonHost::TooltipTimer_Elapsed);
				TooltipTimer->Stop();
			}else
			{
				TooltipTimer = NULL;
			}
		}

		CItemButtonHost::~CItemButtonHost()
		{
			if(Handle != NULL && Manager != NULL)
				Manager->GetGUIManager()->Destroy(Handle);
			Handle = NULL;
			if(TooltipTimer != NULL)
			{
				TooltipTimer->Stop();
				delete TooltipTimer;
			}
			TooltipTimer = NULL;
		}

		void CItemButtonHost::TooltipTimer_Elapsed(NGin::ITimer* timer, NGin::ElapsedEventArgs* e)
		{
			TooltipTimer->Stop();

			NGin::Math::Vector2 MousePosition = NGin::Math::Vector2((float)MouseX(), (float)MouseY()) / Manager->GetGUIManager()->GetResolution();

			if(MousePosition.X < Handle->GlobalLocation().X
				|| MousePosition.Y < Handle->GlobalLocation().Y
				|| MousePosition.X > Handle->GlobalLocation().X + Handle->Size.X
				|| MousePosition.Y > Handle->GlobalLocation().Y + Handle->Size.Y)
			{
				return;
			}

			if(Globals->CurrentMouseToolTip != NULL)
			{
				delete Globals->CurrentMouseToolTip;
				Globals->CurrentMouseToolTip = NULL;
			}

			if(Globals->CurrentMouseItem == NULL)
			{
				std::vector<std::string> Lines;
				std::vector<NGin::Math::Color> Colors;

				Colors.push_back(NGin::Math::Color(1.0f, 1.0f, 1.0f, 1.0f));

				if(Handle->GetHeldItem() != 65535)
				{
					Lines.push_back(GetItemName(Handle->GetHeldItem()));
					Globals->CurrentMouseToolTip = CreateMouseToolTip(Manager->GetGUIManager(), Handle, Lines, Colors);
				}else if(Handle->GetHeldSpell() != 65535)
				{
					Lines.push_back(GetSpellName(Handle->GetHeldSpell()));
					Globals->CurrentMouseToolTip = CreateMouseToolTip(Manager->GetGUIManager(), Handle, Lines, Colors);
				}
			}
		}

		NGin::GUI::IControl* CItemButtonHost::GetHandle()
		{
			return Handle;
		}

		bool CItemButtonHost::ReadProperty(PacketReader* reader, std::string &name)
		{
			// Default properties
			if(name.compare("TIME") == 0)
			{
				Handle->SetTimerValue((float)reader->ReadByte());
			}
			else if(name.compare("FLAG") == 0)
			{
				char Flags = reader->ReadByte();

				CanDragToInventory = (Flags & 1) > 0;
				CanDragFromInventory = (Flags & 2) > 0;
				CanDragFromSpells = (Flags & 4) > 0;

			}else if(name.compare("ITEM") == 0)
			{
				unsigned short ItemID = reader->ReadUInt16();
				unsigned short ItemAmount = reader->ReadUInt16();

				Handle->SetHeldItem(ItemID, ItemAmount);

			}else if(name.compare("ITID") == 0)
			{
				unsigned short ItemID = reader->ReadUInt16();

				Handle->SetHeldItem(ItemID, Handle->GetHeldAmount());

			}else if(name.compare("ITAM") == 0)
			{
				unsigned int ItemAmount = reader->ReadUInt16();

				Handle->SetHeldItem(Handle->GetHeldItem(), ItemAmount);

			}else if(name.compare("SPEL") == 0)
			{
				unsigned short SpellID = reader->ReadUInt16();

				Handle->SetHeldSpell(SpellID);

			}else if(name.compare("BGIM") == 0)
			{
				std::string BGPath = reader->ReadString();

				Handle->SetBackgroundImage(BGPath);
			}else
			{
				IControlHost::ReadProperty(reader, name);
			}

			return true;
		}

		void CItemButtonHost::Create(IControlHostManager* manager, ControlFactoryEventArgs* e)
		{
			// Setup ControlHost
			CItemButtonHost* Host = new CItemButtonHost(manager, e->Master);
			if(e->Master == NULL)
				Host->Master = Host;

			// Register
			manager->RegisterControlHost(Host);

			// Create Button handle
			Host->Handle = CreateItemButton(manager->GetGUIManager(), "UserItemButton", NGin::Math::Vector2(0, 0), NGin::Math::Vector2(75.0f, 23.0f) / manager->GetGUIManager()->GetResolution());
			Host->Handle->SetBackgroundImage("Data\\Textures\\GUI\\Backpack.bmp");
			Host->Handle->Click()->AddEvent(Host, &CItemButtonHost::Handle_Click);
			Host->Handle->RightClick()->AddEvent(Host, &CItemButtonHost::Handle_RightClick);
			Host->Handle->MouseEnter()->AddEvent(Host, &CItemButtonHost::Handle_MouseEnter);

			// Parenting
			if(e->Parent != NULL)
			{
				NGin::GUI::IControl* ParentHandle = e->Parent->GetHandle();
				if(ParentHandle != NULL)
					Host->Handle->Parent = ParentHandle;
			}

			// Set 'basehandle'.. it exists just to prevent our need to cast all the time.
			Host->BaseHandle = Host->Handle;

			// Read default properties
			while(e->Reader->GetLocation() < e->Reader->GetLength() - 1)
			{
				std::string Name = e->Reader->ReadString(4);

				if(!Host->ReadProperty(e->Reader, Name))
					break;
			}
		}

		void CItemButtonHost::Handle_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			//SendEvent("LCLK");

			// You can click it if you're holding something
			if(Globals->CurrentMouseItem != NULL)
			{
				// Unless you can drop it!
				if(Globals->CurrentMouseItem->GetSourceLocation() == MIS_SDK)
				{
					delete Globals->CurrentMouseItem;
					Globals->CurrentMouseItem = NULL;
				}
				else if(Globals->CurrentMouseItem->GetSourceLocation() == MIS_Inventory && CanDragFromInventory)
				{
					unsigned short ItemID = Globals->CurrentMouseItem->GetItemID();
					unsigned short ItemAmount = Globals->CurrentMouseItem->GetItemAmount();

					Handle->SetHeldItem(ItemID, ItemAmount);

					PacketWriter Pa;
					Pa.Write(AllocID);
					Pa.Write(std::string("FRIN"), false);
					Pa.Write(ItemID);
					Pa.Write(ItemAmount);

					Manager->PostEventPacket(Pa);

					IItemButton* Control = reinterpret_cast<IItemButton*>(Globals->CurrentMouseItem->GetSourceData());
					if(Control != NULL)
						Control->SetHeldItem(ItemID, ItemAmount);

					delete Globals->CurrentMouseItem;
					Globals->CurrentMouseItem = NULL;

				}else if(Globals->CurrentMouseItem->GetSourceLocation() == MIS_Spells && CanDragFromSpells)
				{
					unsigned short SpellID = Globals->CurrentMouseItem->GetSpellID();

					Handle->SetHeldSpell(SpellID);

					PacketWriter Pa;
					Pa.Write(AllocID);
					Pa.Write(std::string("FRSP"), false);
					Pa.Write(SpellID);

					Manager->PostEventPacket(Pa);

					delete Globals->CurrentMouseItem;
					Globals->CurrentMouseItem = NULL;
				}
			}else
			{
				// A Click event cannot be processed if it allows the user to "pick up"
				if(CanDragToInventory)
				{
					if(Handle->GetHeldItem() != 65535)
					{
						Globals->CurrentMouseItem = CreateMouseItem(Manager->GetGUIManager(), Handle->GetHeldItem(), 65535, Handle->GetHeldAmount(), MIS_SDK, this);
					}
				}else
				{
					SendEvent("LCLK");
				}
			}
		}

		void CItemButtonHost::DraggedToInventory()
		{
			SendEvent("TOIN");
		}

		void CItemButtonHost::Handle_RightClick(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			SendEvent("RCLK");
		}

		void CItemButtonHost::Handle_MouseEnter(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
		{
			if(TooltipTimer != NULL)
			{
				TooltipTimer->Stop();
				TooltipTimer->Start();
			}
		}
	}
}
