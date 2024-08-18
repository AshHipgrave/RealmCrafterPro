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

#include "Temp.h"
#include <NGinString.h>
#include <List.h>
#include <BlitzPlus.h>

struct Actor;

#include "Actors.h"
#include "Default Project.h"
#include "Language.h"

// Stupid window definitions
#ifdef CreateWindow
#undef CreateWindow
#endif

namespace RealmCrafter
{
	class COptionsMenu
	{
	public:

		COptionsMenu();
		~COptionsMenu();

		void Initialize();

		void Show();
		void Hide();
		void Save();
		void Apply();

	private:

		void CopyControlsToLocal();
		void CopyLocalToControls();
		void ControlEditor_WaitForInputKeyCode(NGin::GUI::IControl* sender, RealmCrafter::ControlEventArgs* e);
		void OKButton_Click(NGin::GUI::IControl* sender, RealmCrafter::ControlEventArgs* e);
		void CancelButton_Click(NGin::GUI::IControl* sender, RealmCrafter::ControlEventArgs* e);

		string SelectedResolution;
		int SelectedAA, SelectedShadows, SelectedAnisotropy, SelectedQuality, SelectedGrassDistance;
		bool SelectedWindowed;
		float SelectedVolume;

		IWindow* Window;
		ITabControl* TabControl;

		IButton* OKButton;
		IButton* CancelButton;

		ILabel* ResolutionLabel, *AntiAliasLabel, *ShadowDetailLabel, *EffectQualityLabel, *AnisotropyLabel, *GrassDistanceLabel;
		IComboBox* ResolutionCombo, *WindowCombo, *AntiAliasCombo, *ShadowDetailCombo, *EffectQualityCombo, *AnisotropyCombo;
		ITrackBar* GrassDistanceTrack;

		IControlEditor* ControlEditor;

		ILabel* VolumeLabel;
		ITrackBar* VolumeTrack;
		
	};
}
