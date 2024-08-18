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
#pragma once

#include <ITextBox.h>
#include <ILabel.h>
#include "CGUIManager.h"

#include "regex/Matcher.h"
#include "regex/Pattern.h"
#include "regex/WCMatcher.h"
#include "regex/WCPattern.h"

namespace NGin
{
	namespace GUI
	{
		class CGUIManager;
		class CTextBox : public ITextBox
		{
		protected:

			struct CharacterEntry
			{
				unsigned char Value;
				float XPos, YPos;
				float Width, Height;
				int Char;
			};

			ArrayList<CharacterEntry> Entries;
			ArrayList<CharacterEntry> Visibles;
			int CaretPosition;
			float CaretOffset;
			int StrLen;
			bool MouseState;

			CGUIManager* Manager;
			IGUIMeshBuffer* MeshBuffer;
			IGUIMeshBuffer* LabelBuffer;

			std::string _PassChar;

			IFont* _Font;

			Pattern* Regex;
			std::string _ValidationExpression;

			bool KnownFocus;

			void RebuildMesh();

			virtual void OnTransform();
			virtual void OnTextChange();
			virtual void OnEnabledChange();
			virtual void OnBackColorChange();
			virtual void OnForeColorChange();
			virtual void OnSkinChange();
			virtual void OnSizeChange();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset();
			virtual bool Update(GUIUpdateParameters* Parameters);
			virtual void Render();

		public:

			CTextBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager);
			virtual ~CTextBox();
			virtual bool Initialize(CGUIManager* Manager);

			virtual void zzPut_PasswordCharacter(std::string PasswordCharacter);
			virtual std::string zzGet_PasswordCharacter();

			virtual void zzPut_ValidationExpression(std::string Expression);
			virtual std::string zzGet_ValidationExpression();
		};
	}
}