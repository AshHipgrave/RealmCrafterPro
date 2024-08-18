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
#include "CPlayerAttributeBars.h"

namespace RealmCrafter
{
	IPlayerAttributeBars* CreatePlayerAttributeBars(NGin::GUI::IGUIManager* guiManager)
	{
		return new CPlayerAttributeBars(guiManager);
	}

	CPlayerAttributeBars::CPlayerAttributeBars(NGin::GUI::IGUIManager* guiManager)
		: GUIManager(guiManager), Visible(true)
	{
		
	}

	CPlayerAttributeBars::~CPlayerAttributeBars()
	{
		for(int i = 0; i < 40; ++i)
		{
			GUIManager->Destroy(ProgressBars[i]);
			GUIManager->Destroy(Labels[i]);
		}
	}

	void CPlayerAttributeBars::Update(SAttributes* attributes)
	{
		if(attributes == NULL)
			return;

		for(int i = 0; i < 40; ++i)
		{
			int newValue = (int)(((float)attributes->Value[i] / (float)attributes->Maximum[i]) * 100.0f);
			std::string newText = std::toString(attributes->Value[i]) + " / " + std::toString(attributes->Maximum[i]);

			// Check if its changed. This is important since
			// a change (especially for text) invalidates the control
			// causing a rebuild.
			if(newValue != ProgressBars[i]->Value)
				ProgressBars[i]->Value = newValue;
			if(newText != Labels[i]->Text)
				Labels[i]->Text = newText;
		}
	}

	void CPlayerAttributeBars::Initialize()
	{
		for(int i = 0; i < 40; ++i)
		{
			// Everything is hidden by default (switched on/off through properties)
			ProgressBars[i] = GUIManager->CreateProgressBar(std::string("AttributeBars::Bar") + std::toString(i), NGin::Math::Vector2(0, 0), NGin::Math::Vector2(0, 0));
			ProgressBars[i]->Visible = false;

			Labels[i] = GUIManager->CreateLabel(std::string("AttributeBars::Label") + std::toString(i), NGin::Math::Vector2(0, 0), NGin::Math::Vector2(0, 0));
			Labels[i]->Text = "00000 / 00000";
			Labels[i]->Align = NGin::GUI::TextAlign_Center;
			Labels[i]->Visible = false;
		}

		// Set initial control states
		GUIManager->SetProperties("AttributeBars");
	}

	void CPlayerAttributeBars::SetVisible(bool visible)
	{
		Visible = visible;

		if(!Visible)
		{
			for(int i = 0; i < 40; ++i)
			{
				ProgressBars[i]->Visible = false;
				Labels[i]->Visible = false;
			}
		}
		else
		{
			GUIManager->SetProperties("AttributeBars");
		}
	}

	bool CPlayerAttributeBars::GetVisible() const
	{
		return Visible;
	}
}