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
#include "CFont.h"

namespace NGin
{
	namespace GUI
	{
		CFont::CFont(CGUIManager* GUIManager, std::string FontName)
		{
			FontTexture = NULL;
			Name = FontName;
			std::transform(Name.begin(), Name.end(), Name.begin(), ::tolower);

			Manager = GUIManager;

			for(int i = 0; i < 256; ++i)
				this->Characters.Add(new CFont::CharCoord());
		}

		CFont::~CFont()
		{
			for(int i = 0; i < 256; ++i)
				delete this->Characters[i];

			if(FontTexture != NULL)
				Manager->GetRenderer()->FreeTexture(FontTexture);
		}

		void CFont::OnDeviceReset(Vector2& OldResolution, Vector2& NewResolution)
		{
			for(int i = 0; i < Characters.Size(); ++i)
			{
				Characters[i]->ScreenSize *= OldResolution;
				Characters[i]->ScreenSize /= NewResolution;
			}
		}

		NGin::Math::Vector2 CFont::GetCoord(int Character)
		{
			if(Character < 0 || Character >= this->Characters.Size())
				return NGin::Math::Vector2(0.0f, 0.0f);

			return this->Characters[Character]->TexPos;
		}

		NGin::Math::Vector2 CFont::GetSize(int Character)
		{
			if(Character < 0 || Character >= this->Characters.Size())
				return NGin::Math::Vector2(0.0f, 0.0f);

			return this->Characters[Character]->TexSize;
		}

		NGin::Math::Vector2 CFont::GetScreenCoord(int Character)
		{
			if(Character < 0 || Character >= this->Characters.Size())
				return NGin::Math::Vector2(0.0f, 0.0f);

			return this->Characters[Character]->ScreenSize;
		}

		void CFont::SetTexture()
		{
			Manager->SetTexture(this->FontTexture);
		}

		std::string CFont::GetName()
		{
			return Name;
		}

	}
}
