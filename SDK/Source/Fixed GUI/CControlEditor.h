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

#include <IGUIManager.h>
#include "IControlEditor.h"
#include "IControlLayout.h"

namespace RealmCrafter
{
	// Control Bindings Editor
	class CControlEditor : public RealmCrafter::IControlEditor
	{
	protected:

		// Background texture and transparent scrolling panel
		NGin::GUI::IPictureBox* Background, *Scroller;
		NGin::GUI::IScrollBar* Scrollbar;

		// The last control layout set. This rarely changes
		IControlLayout* Layout;

		// This event will be called each frame that we are waiting for a key
		ControlEventHandler* WaitForInputKeyCodeEvent;

		// If we are waiting, the following will all be nonzero
		bool SuspendInput;
		IControlLayout::SLayoutInstance* SuspendedLayout;
		NGin::GUI::ILabel* SuspendedLabel;

		// Override parent functionality
		virtual void OnSizeChange();
		virtual void OnDeviceReset();
		virtual bool Update(NGin::GUI::GUIUpdateParameters* Parameters);
		virtual void Render();

		// Called when we need to update the view
		virtual void RebuildMesh();

		// Internal events, since we're just using a group of subcontrols
		virtual void Scrollbar_Scroll(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		virtual void KM_MouseEnter(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		virtual void KM_MouseLeave(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		virtual void KM_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);

		// Instance of a tree object
		struct Instance
		{
			std::string Name;
			int ID;
		};

		// Category, tree heading
		struct Category
		{
			std::string Name;
			std::vector<Instance> Instances;
		};

	public:

		CControlEditor(NGin::Math::Vector2& ScreenScale, NGin::GUI::IGUIManager* Manager);
		virtual ~CControlEditor();

		// Called from the engine after creation
		void Initialize();
		
		// Set a control layout to display
		virtual void SetLayout(IControlLayout* layout);

		// Event
		virtual ControlEventHandler* WaitForInputKeyCode() const;
		
		// Create an instance of this class.
		static CControlEditor* Create(NGin::GUI::IGUIManager* manager, std::string name, NGin::Math::Vector2 location, NGin::Math::Vector2 size);

		// Conform to NGUI type structures
		static NGin::Type TypeOf();
	};
}