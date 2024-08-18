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
#include "CChatBox.h"

using namespace std;
using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;

namespace RealmCrafter
{
	CChatBox::CChatBox(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), FadeTime(0)
	{
	}

	CChatBox::~CChatBox()
	{
	}

	void CChatBox::Initialize()
	{
		float W = GUIManager->GetResolution().X;
		float H = GUIManager->GetResolution().Y;

		// Setup the main tab control (it just assumes the properties of the older chat history box)
		TabControl = GUIManager->CreateTabControl("ChatHistory::Background", Vector2(0.1f, 0.1f), Vector2(300 / W, 200 / H));
		Color BackColor = TabControl->BackColor; BackColor.A = 0.0f;
		TabControl->BackColor = BackColor;

		// The 'SizeLine' is used to measure text to make sure it fits.
		SizeLine = GUIManager->CreateLabel("ChatBox::SizeLine", Vector2(0, 0), Vector2(0, 0));
		SizeLine->Visible = false;
	}

	void CChatBox::Update()
	{
		// Fading shouldn't happen too fast
		if(MilliSecs() - FadeTime < 10)
			return;
		FadeTime = MilliSecs();

		// ResetColor is used to prevent having to rebuild the whole mesh.
		Color BackColor = TabControl->BackColor;
		bool ResetColor = false;

		// Mouse position
		float MX = ((float)MouseX()) / GUIManager->GetResolution().X;
		float MY = ((float)MouseY()) / GUIManager->GetResolution().Y;

		if(MX > TabControl->Location.X && MY > TabControl->Location.Y
			&& MX < TabControl->Location.X + TabControl->Size.X
			&& MY < TabControl->Location.Y + TabControl->Size.Y)
		{
			if(BackColor.A < 0.9999f)
			{
				BackColor.A += 0.05f;
				ResetColor = true;

				if(BackColor.A > 1.0f)
					BackColor.A = 1.0f;
			}
		}else
		{
			if(BackColor.A > 0.001f)
			{
				BackColor.A -= 0.05f;
				ResetColor = true;

				if(BackColor.A < 0.0f)
					BackColor.A = 0.0f;
			}
		}

		if(ResetColor)
		{
			TabControl->BackColor = BackColor;

			for(int i = 0; i < (int)Tabs.size(); ++i)
			{
				Tabs[i]->ScrollBar->BackColor = BackColor;
			}
		}


	}

	void CChatBox::AddTab(int id, std::string &name, int width)
	{
		float W = GUIManager->GetResolution().X;
		float H = GUIManager->GetResolution().Y;

		// Cant allow more than one instance
		if(GetTabFromID(id) != NULL)
			return;

		// Setup basic tab
		ChatTab* T = new ChatTab();
		Tabs.push_back(T);
		T->ServerID = id;
		T->Name = name;
		T->Width = width;

		// Add actual tab page
		int TID = TabControl->AddTab(name, ((float)width) / W);
		T->TabPanel = TabControl->TabPanel(TID);

		// Scrollbar
		T->ScrollBar = GUIManager->CreateScrollBar("ChatBox::Scrollbar", Vector2(T->TabPanel->Size.X - (20 / W), 4 / H), Vector2(16 / W, T->TabPanel->Size.Y - (8 / H)), VerticalScroll);
		T->ScrollBar->Parent = T->TabPanel;
		T->ScrollBar->Tag = T;
		T->ScrollBar->Scroll()->AddEvent(this, &CChatBox::ChatTab_ScrollBar_Scroll);
		NGin::Math::Color ScrollCol = T->ScrollBar->BackColor;
		ScrollCol.A = TabControl->BackColor.A;
		T->ScrollBar->BackColor = ScrollCol;

		// Text
		float LineHeight = 13;
		LineHeight /= GUIManager->GetResolution().Y;

		int MaxChatLine = (int)((T->TabPanel->Size.Y - (8 / H)) / LineHeight);
		--MaxChatLine;

		for(int i = 0; i <= MaxChatLine; ++i)
		{
			ILabel* L = GUIManager->CreateLabel("ChatBox::Line", Vector2(12 / W, (4 / H) + (i * LineHeight)), Vector2(0, 0));
			L->Parent = T->TabPanel;
			L->Text = "";

			T->Labels.push_back(L);
		}

		// Scrollbar values
		T->ScrollBar->LargeChange = T->Labels.size();

		TabControl->SwitchTo(TID);
		TabControl->BackColor = TabControl->BackColor;
		


	}

	void CChatBox::RemoveTab(int id)
	{
		// Find
		ChatTab* Tab = GetTabFromID(id);
		if(Tab != NULL)
		{
			// Free associated controls
			for(int i = 0; i < (int)Tab->Labels.size(); ++i)
			{
				GUIManager->Destroy(Tab->Labels[i]);
			}
			GUIManager->Destroy(Tab->ScrollBar);

			// Remove tab
			int TID = GetNTabID(Tab);
			if(TID > -1)
				TabControl->RemoveTab(TID);

			// Remove tab from local list
			TID = -1;
			for(int i = 0; i < (int)Tabs.size(); ++i)
			{
				if(Tabs[i]->ServerID == id)
				{
					TID = i;
					break;
				}
			}
			if(TID > -1)
				Tabs.erase(Tabs.begin() + TID);
			delete Tab;

			// Switch to the main tab, if possible
			if(TabControl->GetTabCount() > 0)
				TabControl->SwitchTo(0);
		}

	}

	void CChatBox::SwitchToTab(int id)
	{
		ChatTab* Tab = GetTabFromID(id);
		if(Tab != NULL)
		{
			int TID = GetNTabID(Tab);
			if(TID > -1)
				TabControl->SwitchTo(TID);
		}
	}

	void CChatBox::Output(int id, std::string &text, NGin::Math::Color &color)
	{
		// If the text contains a linebreak, we immediatly know that this method will be recalled, so save any extra work now.
		std::size_t Pos =  text.find('\n');
		if(Pos != std::string::npos && Pos != text.size() - 1)
		{
			Output(id, text.substr(0, Pos), color);
			Output(id, text.substr(Pos + 1), color);
			return;
		}

		// Find
		ChatTab* Tab = GetTabFromID(id);
		if(Tab != NULL)
		{
			// Max width is just a but less than the scrollbar
			float MaxWidth = (Tab->TabPanel->Size.X - (24 / GUIManager->GetResolution().X)) - Tab->Labels[0]->Location.X;

			// Fit text into box
			SizeLine->Text = text;
			if(SizeLine->InternalWidth >= MaxWidth)
			{
				// Split to the nearest space
				for(int i = text.length() - 1; i >= 0; --i)
				{
					if(text.substr(i, 1) == string(" "))
					{
						SizeLine->Text = text.substr(0, i);
						if(SizeLine->InternalWidth < MaxWidth)
						{
							Output(id, text.substr(0, i), color);
							Output(id, text.substr(i + 1), color);
							return;
						}
					}
				}

				// No suitable space, split to the nearest letter
				for(int i = text.length() - 1; i >= 0; --i)
				{
					SizeLine->Text = text.substr(0, i - 1);
					if(SizeLine->InternalWidth < MaxWidth)
					{
						Output(id, text.substr(0, i - 1), color);
						Output(id, text.substr(i), color);
						return;
					}
				}
				return;
			}

			// Push a new texture instance and pop lines over a range
			ChatTab::TextInstance I;
			I.Text = text;
			I.Color = color;
			Tab->Lines.push_back(I);
			if(Tab->Lines.size() > 200)
				Tab->Lines.erase(Tab->Lines.begin());

			// Check to see if the amount of text extends beyond the box
			int LineCount = Tab->Lines.size();
			if(LineCount < (int)Tab->Labels.size())
			{
				// Disable scrollbar
				Tab->ScrollBar->Maximum = 0;
				Tab->ScrollBar->LargeChange = 0;
			}else
			{
				// Enable scrollbase
				Tab->ScrollBar->LargeChange = Tab->Labels.size();
				Tab->ScrollBar->Maximum = LineCount;

				// Have the scrollbar value track new text if its at the bottom
				// Do nothing if its not, since the player might be reading the scrollback
				if(Tab->ScrollBar->Value >= LineCount - ((int)Tab->Labels.size()))
					Tab->ScrollBar->Value = LineCount - ((int)Tab->Labels.size() - 1);
			}

			// Update visible text
			UpdateTab(Tab);
		}
	}

	void CChatBox::SetVisible(bool visibility)
	{
		TabControl->Visible = visibility;
	}

	bool CChatBox::GetVisible()
	{
		return TabControl->Visible;
	}

	void CChatBox::Reset()
	{
		// Remove all tabs
		for(int i = 0; i < (int)Tabs.size(); ++i)
		{
			ChatTab* Tab = Tabs[i];

			for(int l = 0; l < (int)Tab->Labels.size(); ++l)
				GUIManager->Destroy(Tab->Labels[l]);
			Tab->Labels.clear();

			GUIManager->Destroy(Tab->ScrollBar);
		}
		Tabs.clear();

		while(TabControl->GetTabCount() > 0)
			TabControl->RemoveTab(0);

	}

	void CChatBox::ResolutionChange(NGin::Math::Vector2 &newResolution)
	{
		float W = newResolution.X;
		float H = newResolution.Y;

		for(int i = 0; i < (int)Tabs.size(); ++i)
		{
			ChatTab* Tab = Tabs[i];

			// Calculate new tab width
			int TID = GetNTabID(Tab);
			float NewWidth = ((float)Tab->Width) / W;
			TabControl->SetTabWidth(TID, NewWidth);

			// Reposition scrollbar
			Tab->ScrollBar->Location = Vector2(Tab->TabPanel->Size.X - (20 / W), 4 / H);
			Tab->ScrollBar->Size = Vector2(16 / W, Tab->TabPanel->Size.Y - (8 / H));


			// Remove labels
			for(int l = 0; l < (int)Tab->Labels.size(); ++l)
				GUIManager->Destroy(Tab->Labels[l]);
			Tab->Labels.clear();

			// Text
			float LineHeight = 13;
			LineHeight /= GUIManager->GetResolution().Y;

			int MaxChatLine = (int)((Tab->TabPanel->Size.Y - (8 / H)) / LineHeight);
			--MaxChatLine;

			// Re-add labels
			for(int i = 0; i <= MaxChatLine; ++i)
			{
				ILabel* L = GUIManager->CreateLabel("ChatBox::Line", Vector2(12 / W, (4 / H) + (i * LineHeight)), Vector2(0, 0));
				L->Parent = Tab->TabPanel;
				L->Text = "";

				Tab->Labels.push_back(L);
			}

			// Update scrollbar (As in Output())
			int LineCount = Tab->Lines.size();
			if(LineCount < (int)Tab->Labels.size())
			{
				Tab->ScrollBar->Maximum = 0;
				Tab->ScrollBar->LargeChange = 0;
			}else
			{
				Tab->ScrollBar->LargeChange = Tab->Labels.size();
				Tab->ScrollBar->Maximum = LineCount;

				if(Tab->ScrollBar->Value >= LineCount - ((int)Tab->Labels.size()))
					Tab->ScrollBar->Value = LineCount - ((int)Tab->Labels.size() - 1);
			}

			// Update visible text
			UpdateTab(Tab);
		}

		// Switch back to the current tab.. this redraws the tab buttons
		TabControl->SwitchTo(TabControl->GetSelectedTabIndex());
	}

	void CChatBox::ChatTab_ScrollBar_Scroll(NGin::GUI::IControl* sender, NGin::GUI::ScrollEventArgs* e)
	{
		// If a valid scrollbar has moved, then the visible text needs to be updated
		if(sender->GetType() == IScrollBar::TypeOf())
		{
			if(sender->Tag != NULL)
			{
				ChatTab* Tab = reinterpret_cast<ChatTab*>(sender->Tag);
				UpdateTab(Tab);
			}
		}
	}

	void CChatBox::UpdateTab(CChatBox::ChatTab* tab)
	{
		if(tab == NULL)
			return;

		// Starting text line (the Maximum is an upper inclusive, so cap it)
		int ScrollLine = tab->ScrollBar->Value;
		if(ScrollLine == tab->ScrollBar->Maximum - (tab->Labels.size() - 1))
			--ScrollLine;

		// This shouldn't happen, but helps with any bounding errors, just in case.
		if(ScrollLine >= (int)tab->Lines.size())
			return;

		int LinesCount = tab->Lines.size() - ScrollLine;
		int LabelsCount = tab->Labels.size();

		// If there is less text that the visible area, then clear existing labels
		if(LinesCount < LabelsCount)
		{
			for(int i = 0; i < LabelsCount; ++i)
				tab->Labels[i]->Text = "";
		}

		// Set text and color values
		for(int i = 0; i < (LinesCount < LabelsCount ? LinesCount : LabelsCount); ++i)
		{
			tab->Labels[i]->Text = tab->Lines[i + ScrollLine].Text;
			tab->Labels[i]->ForeColor = tab->Lines[i + ScrollLine].Color;
		}

	}

	CChatBox::ChatTab* CChatBox::GetTabFromID(int id)
	{
		for(int i = 0; i < (int)Tabs.size(); ++i)
		{
			if(Tabs[i]->ServerID == id)
				return Tabs[i];
		}

		return NULL;
	}

	int CChatBox::GetNTabID(CChatBox::ChatTab* tab)
	{
		if(tab == NULL || TabControl == NULL)
			return -1;

		for(int i = 0; i < TabControl->GetTabCount(); ++i)
		{
			if(TabControl->TabPanel(i) == tab->TabPanel)
				return i;
		}

		return -1;
	}


}