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
#include "SDKMain.h"
#include "CPlayerQuestLog.h"

namespace RealmCrafter
{
	IPlayerQuestLog* CreatePlayerQuestLog(NGin::GUI::IGUIManager* guiManager, SQuestLog* questLog)
	{
		return new CPlayerQuestLog(guiManager, questLog);
	}

	CPlayerQuestLog::CPlayerQuestLog(NGin::GUI::IGUIManager* guiManager, SQuestLog* questLog)
		: GUIManager(guiManager), QuestLog(questLog), Window(NULL)
	{
		
	}

	CPlayerQuestLog::~CPlayerQuestLog()
	{
		GUIManager->Destroy(Window);
	}

	void CPlayerQuestLog::Initialize()
	{
#define R(x, y) (NGin::Math::Vector2(x, y) / GUIManager->GetResolution())

		NGin::Math::Vector2 windowSize = R(560, 320);

		Window = GUIManager->CreateWindow("NewQuestLog::Window", NGin::Math::Vector2(0.5f, 0.5f) - (windowSize * 0.5f), windowSize);
		Window->Text = "Quest Log";

		Background = GUIManager->CreatePictureBox("NewQuestLog::Background", R(12, 12), R(520, 227));
		Background->MinTexCoord = NGin::Math::Vector2(0.9f, 0.9f);
		Background->MaximumSize = NGin::Math::Vector2(1, 1);
		Background->BackColor = NGin::Math::Color(0, 0, 0);

		ScrollBar = GUIManager->CreateScrollBar("NewQuestLog::ScrollBar",  R(518, 12), R(17, 227), NGin::GUI::VerticalScroll);
		ScrollBar->LargeChange = 17;
		ScrollBar->Scroll()->AddEvent(this, &CPlayerQuestLog::ScrollBar_Scroll);

		ShowCompletedCheckBox = GUIManager->CreateCheckBox("NewQuestLog::ShowCompletedCheckBox", R(12, 245), R(150, 17));
		ShowCompletedCheckBox->Text = "Show Completed";
		ShowCompletedCheckBox->CheckedChange()->AddEvent(this, &CPlayerQuestLog::ShowCompletedCheckBox_CheckedChange);

		Background->Parent = Window;
		ScrollBar->Parent = Window;
		ShowCompletedCheckBox->Parent = Window;

		// 17 Labels fit inside the default dimensions (maybe make this dynamic?)
		for(int i = 0; i < 17; ++i)
		{
			NGin::GUI::ILabel* label = GUIManager->CreateLabel("NewQuestLog::Label", R(15, (float)15 + i * 13), R(0, 0));
			label->Text = "";
			label->Parent = Window;

			QuestNames.push_back(label);

			label = GUIManager->CreateLabel("NewQuestLog::Label", R(260, (float)15 + i * 13), R(0, 0));
			label->Text = "";
			label->Parent = Window;

			QuestStats.push_back(label);
		}

		GUIManager->SetProperties("NewQuestLog");
		Window->Visible = true;
#undef R
	}

	uint CPlayerQuestLog::GetQuestCount(bool completed)
	{
		// Not good, add a better counter.
		uint count = 0;
		for(uint i = 0; i < 500; ++i)
		{
			if(QuestLog->EntryName[i].length() > 0)
			{
				if(completed)
					++count;
				else if(QuestLog->EntryStatus[i].length() >= 4 && QuestLog->EntryStatus[i][3] != (char)254)
					++count;
			}
		}

		return count;
	}

	void CPlayerQuestLog::Update()
	{
		uint count = GetQuestCount(ShowCompletedCheckBox->Checked);
		ScrollBar->Minimum = 0;
		ScrollBar->Maximum = count < 17 ? 17 : count;

		Rebuild();
	}

	void CPlayerQuestLog::Rebuild()
	{
		int i = ScrollBar->Value;
		int v = 0;
		bool showCompleted = ShowCompletedCheckBox->Checked;
		int count = 500 - i;

		for(; i < count && v < (int)QuestNames.size(); ++i, ++v)
		{
			if(QuestLog->EntryStatus[i].length() < 4 )
			{
				--v;
				continue;
			}

			if( QuestLog->EntryStatus[i][3] != (char)254 )
			{
				NGin::Math::Color color((int)QuestLog->EntryStatus[i][0], (int)QuestLog->EntryStatus[i][1], (int)QuestLog->EntryStatus[i][2]);
				QuestNames[v]->ForeColor = color;
				QuestStats[v]->ForeColor = color;

				QuestStats[v]->Text = QuestLog->EntryStatus[i].substr(3);
			}else
			{
				if(!showCompleted)
				{
					--v;
					continue;
				}

				QuestNames[v]->ForeColor = NGin::Math::Color(255, 255, 100);
				QuestStats[v]->ForeColor = NGin::Math::Color(255, 255, 100);
				QuestStats[v]->Text = "Completed";
			}

			QuestNames[v]->Text = QuestLog->EntryName[i];
		}

		for(; v < (int)QuestNames.size(); ++i, ++v)
		{
			QuestNames[v]->Text = "";
			QuestStats[v]->Text = "";
		}
	}

	void CPlayerQuestLog::ScrollBar_Scroll(NGin::GUI::IControl* sender, NGin::GUI::ScrollEventArgs* e)
	{
		Rebuild();
	}

	void CPlayerQuestLog::ShowCompletedCheckBox_CheckedChange(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		ScrollBar->Value = 0;
		Update();
	}

	void CPlayerQuestLog::SetVisible(bool visible)
	{
		Window->Visible = visible;
		if(visible)
			Window->BringToFront();
	}

	bool CPlayerQuestLog::GetVisible()
	{
		return Window->Visible;
	}

	bool CPlayerQuestLog::IsActiveWindow()
	{
		return Window->IsActiveWindow();
	}

	NGin::GUI::EventHandler* CPlayerQuestLog::Closed()
	{
		return Window->Closed();
	}
}