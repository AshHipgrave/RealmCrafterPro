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

#include "CServerSelector.h"

using namespace NGin;
using namespace NGin::GUI;
using namespace NGin::Math;
using namespace std;

namespace RealmCrafter
{
	CServerSelector::CServerSelector(NGin::GUI::IGUIManager* guiManager)
		: LastSelectedIndex(0), GUIManager(guiManager)
	{
		ClosedEvent = new NGin::GUI::EventHandler();
	}

	CServerSelector::~CServerSelector()
	{
	}

	void CServerSelector::Initialize()
	{
#pragma region GUI Setup
		SelectorWindow = GUIManager->CreateWindow("MainMenu::ServerSelection", Vector2(0, 0), Vector2(0, 0));
		SelectorList = GUIManager->CreateListBox("MainMenu::ServerSelection::SelectorList", Vector2(12, 12), Vector2(190, 160));
		SelectorConnect = GUIManager->CreateButton("MainMenu::ServerSelection::SelectorConnect", Vector2(12, 178), Vector2(190, 23));

		SelectorWindow->Text = "Select Realm...";
		SelectorWindow->Modal = true;
		SelectorWindow->Visible = false;
		SelectorConnect->Text = "Connect";

		SelectorList->Parent = SelectorWindow;
		SelectorConnect->Parent = SelectorWindow;

		SelectorWindow->Closed()->AddEvent(this, &CServerSelector::CancelButton_Click);
		SelectorConnect->Click()->AddEvent(this, &CServerSelector::ConnectButton_Click);
#pragma endregion

#pragma region Check server or download list

		// Download server list
	 	if(FileType("Data\\Game Data\\Server Selector.dat"))
	 	{
	 		// Get URL of server list
	 		FILE* F = ReadFile("Data\\Game Data\\Server Selector.dat");
	 		if(F == 0)
	 			RuntimeError("Could not open file: Data\\Game Data\\Server Selector.dat!");
	 		string ListURL = ReadLine(F);
	 		CloseFile(F);
	 		if(ListURL.length() == 0)
	 			RuntimeError("Server list URL not found in Data\\Game Data\\Server Selector.dat!");
	 
	 		// Download server list
	 		uint Result = DownloadFile(ListURL, "Data\\Temp.dat");
	 		if(Result == 0)
	 			RuntimeError("Could not retrieve list, please check that you are connected to the internet!");
	 
	 		// Fill list
	 		F = ReadFile("Data\\Temp.dat");
	 		while(!Eof(F))
	 		{
	 			Selection* S = new Selection();
	 
	 			S->Name = ReadLine(F);
	 			S->Host = ReadLine(F);
				S->Port = 25000;

				if(S->Host.find(":", 0) != -1)
				{
					int Colon = S->Host.find(":", 0);

					S->Port = atoi(S->Host.substr(Colon + 1).c_str());
					//S->Port = std::toInt(S->Host.substr(Colon + 1));

					S->Host = S->Host.substr(0, Colon);
				}
	 
	 			Servers.push_back(S);
	 			
	 			SelectorList->AddItem(S->Name);
	 		}
	 		CloseFile(F);
	 		BBDeleteFile("Data\\Temp.dat");
	 
	 		if(SelectorList->ItemCount() == 0)
	 			RuntimeError("No available servers found!");
			SelectorList->SelectedIndex = 0;

			// See where the user was last time
			if(FileType("Data\\LastHost.dat"))
			{
				F = ReadFile("Data\\LastHost.dat");
				string CurrentName = ReadLine(F);

				for(int i = 0; i < (int)Servers.size(); ++i)
				{
					if(Servers[i]->Name.compare(CurrentName) == 0)
					{
						SelectorList->SelectedIndex = i;
						LastSelectedIndex = i;
						break;
					}
				}

				CloseFile(F);
			}
	 	}

#pragma endregion
	}

	void CServerSelector::Show()
	{
		if(!IsMultiServer())
		{
			SelectorWindow->Visible = false;
			return;
		}

		SelectorWindow->Visible = true;
		SelectorWindow->BringToFront();
	}

	void CServerSelector::CancelButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		SelectorList->SelectedIndex = LastSelectedIndex;

		if(ClosedEvent != NULL)
			ClosedEvent->Execute(SelectorWindow, NULL);
	}

	void CServerSelector::ConnectButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		LastSelectedIndex = SelectorList->SelectedIndex;
		SelectorWindow->Visible = false;

		// Save 'selection'
		FILE* F = WriteFile("Data\\LastHost.dat");
		if(F != NULL)
		{
			WriteLine(F, GetServerName());
			CloseFile(F);
		}

		if(ClosedEvent != NULL)
			ClosedEvent->Execute(SelectorWindow, NULL);
	}

	bool CServerSelector::IsMultiServer()
	{
		return SelectorList->ItemCount() > 0;
	}

	std::string CServerSelector::GetServerName()
	{
		return Servers[SelectorList->SelectedIndex]->Name;
	}

	std::string CServerSelector::GetServerAddress()
	{
		return Servers[SelectorList->SelectedIndex]->Host;
	}

	int CServerSelector::GetServerPort()
	{
		return Servers[SelectorList->SelectedIndex]->Port;
	}

	NGin::GUI::EventHandler* CServerSelector::Closed() const
	{
		return ClosedEvent;
	}
}