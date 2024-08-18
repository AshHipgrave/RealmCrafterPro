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

#include "SDKMain.h"
#include "CControlEditor.h"

using namespace NGin;
using namespace NGin::GUI;
using namespace NGin::Math;

namespace RealmCrafter
{
	// Conform to NGUI type structures
	NGin::Type CControlEditor::TypeOf()
	{
		return NGin::Type("RCCE", "RealmCrafter Controls Editor");
	}

	// Create an instance of the control
	CControlEditor* CControlEditor::Create(NGin::GUI::IGUIManager* manager, std::string name, NGin::Math::Vector2 location, NGin::Math::Vector2 size)
	{
		CControlEditor* CEditor = new CControlEditor(Vector2(1, 1), manager);
		CEditor->Locked = true;
		CEditor->Name = name;
		CEditor->Text = "ControlsEditor";
		CEditor->Location = location;
		CEditor->Size = size;
		CEditor->Locked = false;
		CEditor->Parent = manager;
		CEditor->Initialize();

		//manager->GetSkin(1)->ApplyDefaultProperty(CEditor, "CEditor");
		
		return CEditor;
	}

	// Constructor
	CControlEditor::CControlEditor(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager)
		: IControlEditor(ScreenScale, Manager),
		Layout(NULL),
		SuspendInput(false), SuspendedLayout(NULL), SuspendedLabel(NULL)
	{
		_Type = Type("RCCE", "RealmCrafter Controls Editor");
		_Size = Vector2(0, 0);

		WaitForInputKeyCodeEvent = new ControlEventHandler();
	}

	CControlEditor::~CControlEditor()
	{
	}

	void CControlEditor::OnDeviceReset()
	{
		IControl::OnDeviceReset();

		CControlEditor::OnTransform();
	}

	bool CControlEditor::Update(GUIUpdateParameters* Parameters)
	{
		if(!_Enabled)
			return true;

		if(_Visible)
		{
			// Waiting for a key
			if(SuspendInput && SuspendedLayout != NULL)
			{
				// Call engine function to wait for a key
				ControlEventArgs Args(SuspendedLayout);
				WaitForInputKeyCodeEvent->Execute(this, &Args);

				// Received a key
				if(Args.GetHandled())
				{
					// Reset label text and color
					if(SuspendedLabel != NULL)
					{
						SuspendedLabel->Text = ControlName(SuspendedLayout->EditID);
						SuspendedLabel->ForeColor = Color(0.6f, 0.6f, 0.6f);
					}

					// Clear suspension
					SuspendedLabel = NULL;
					SuspendedLayout = NULL;
					SuspendInput = false;
					Scrollbar->Enabled = true;
				}
			}
		}

		if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
		{
			// Mouse if over this control, so update subcontrols.

			// Copied list prevents deletion errors; technically obsolete.
			List<IControl*> Controls;
			foreachc(CIt, IControl, _Controls)
			{
				Controls.Insert((*CIt), 0);

				nextc(CIt, IControl, _Controls);
			}

			foreachf(cCIt, IControl, Controls)
			{
				(*cCIt)->Update(Parameters);

				nextf(cCIt, IControl, Controls);
			}
		}

		return false;
	}

	void CControlEditor::Render()
	{
		if(!_Visible)
			return;

		// Get renderer
		IGUIRenderer* Renderer = _Manager->GetRenderer();

		// Store old scissor region
		Vector4 OldRect = Renderer->GetScissorRect();

		// Create our scissor region
		Vector4 WinSize;
		WinSize.X = _GlobalLocation.X * _Manager->GetResolution().X;
		WinSize.Y = _GlobalLocation.Y * _Manager->GetResolution().Y;
		WinSize.Z = WinSize.X + (_Size.X * _Manager->GetResolution().X);
		WinSize.W = WinSize.Y + (_Size.Y * _Manager->GetResolution().Y);

		// Make sure we fit inside the parent region (We are a subparent control)
		if(WinSize.X < OldRect.X)
			WinSize.X = OldRect.X;
		if(WinSize.Z > OldRect.Z)
			WinSize.Z = OldRect.Z;
		if(WinSize.Y < OldRect.Y)
			WinSize.Y = OldRect.Y;
		if(WinSize.W > OldRect.W)
			WinSize.W = OldRect.W;

		WinSize.W -= 4;

		_Manager->CorrectRect(WinSize);
		Renderer->PushScissorRect(WinSize);

		//printf("render (%s; %s; %s)\n", WinSize.ToString().c_str(), _GlobalLocation.ToString().c_str(), _Size.ToString().c_str());

		foreachc(CIt, IControl, _Controls)
		{
			(*CIt)->Render();

			nextc(CIt, IControl, _Controls);
		}

		Renderer->PopScissorRect();
	}

	void CControlEditor::Initialize()
	{
		// Correct width if this control is scaled to small.
		NGin::Math::Vector2 ScrollWidth = _Manager->GetSkin(1)->GetScreenCoord(VScrollBar_Fill);
		if(_Size.X <= ScrollWidth.X + 0.1f)
			_Size.X = ScrollWidth.X > 0.1f;

		// Core components
		Background = _Manager->CreatePictureBox(_Name + "::Background", Vector2(0, 0), _Size  - Vector2(ScrollWidth.X, 0.0f));
		Scroller = _Manager->CreatePictureBox(_Name + "::Scroller", Vector2(0, 0), _Size  - Vector2(ScrollWidth.X, 0.0f));
		Scrollbar = _Manager->CreateScrollBar(_Name + "::Scrollbar", Vector2(_Size.X - ScrollWidth.X, 0), Vector2(ScrollWidth.X, _Size.Y), VerticalScroll);

		// Scroller is just a panel containing our child controls, its transparent because the background should show through.
		Scroller->BackColor = Math::Color(0.0f, 0.0f, 0.0f, 0.0f);

		// Background will use the window background color.
		Background->MinTexCoord = _Manager->GetSkin(1)->GetCoord(Window_Fill);
		Background->MaxTexCoord = _Manager->GetSkin(1)->GetCoord(Window_Fill) + _Manager->GetSkin(1)->GetScreenCoord(Window_Fill);

		// Parenting
		Background->Parent = this;
		Scrollbar->Parent = this;
		Scroller->Parent = this;

		// Set internal scroll event.
		Scrollbar->Scroll()->AddEvent(this, &CControlEditor::Scrollbar_Scroll);
	}

	void CControlEditor::Scrollbar_Scroll(IControl* sender, EventArgs* e)
	{
		// Move the scroll parent based upon the scroll amount.
		Scroller->Location = Vector2(0, static_cast<float>(-Scrollbar->Value) / _Manager->GetResolution().Y);
	}

	void CControlEditor::KM_MouseEnter(IControl* sender, EventArgs* e)
	{
		// Mouse is over a control label
		if(!SuspendInput)
			sender->ForeColor = Color(0.6f, 0.6f, 0.6f);
	}

	void CControlEditor::KM_MouseLeave(IControl* sender, EventArgs* e)
	{
		// Mouse left a control label
		if(!SuspendInput)
			sender->ForeColor = Color(1.0f, 1.0f, 1.0f);
	}

	void CControlEditor::KM_Click(IControl* sender, EventArgs* e)
	{
		// User clicked a control label, check its valid
		if(SuspendInput || sender->GetType() != ILabel::TypeOf())
			return;

		// Suspend all inputs and prepare to wait for a key
		SuspendInput = true;
		SuspendedLabel = (ILabel*)sender;
		SuspendedLayout = (IControlLayout::SLayoutInstance*)sender->Tag;
		Scrollbar->Enabled = false;
		sender->ForeColor = Color(1.0f, 0.0f, 0.0f);
	}

	void CControlEditor::RebuildMesh()
	{
		if(_Locked)
			return;

		// Expand width if necessary, can't have a small control.
		NGin::Math::Vector2 ScrollWidth = _Manager->GetSkin(1)->GetScreenCoord(VScrollBar_Fill);
		if(_Size.X <= ScrollWidth.X + 0.1f)
			_Size.X = ScrollWidth.X > 0.1f;

		// Clear everything; prevents holding invalid pointers.
		SuspendedLayout = NULL;
		SuspendInput = false;

// 		List<IControl*> BControls = *Scroller->Controls();
// 		std::vector<IControl*> ItControls;
// 		foreachf(CIt, IControl, BControls)
// 		{
// 			//_Manager->Destroy((*CIt));
// 			ItControls.push_back(*CIt);
// 
// 			nextf(CIt, IControl, BControls);
// 		}
// 
// 		for(int i = 0; i < ItControls.size(); ++i)
// 		{
// 			_Manager->Destroy(ItControls[i]);
// 		}
// 		ItControls.clear();

		_Manager->Destroy(Scroller);
		Scroller = _Manager->CreatePictureBox(_Name + "::Scroller", Vector2(0, 0), _Size  - Vector2(ScrollWidth.X, 0.0f));
		Scroller->BackColor = Math::Color(0.0f, 0.0f, 0.0f, 0.0f);
		Scroller->Parent = this;

		if(Layout == NULL)
			return;



		// Move core components into correct locations with correct scales.
		Background->Location = Vector2(0, 0);
		Background->Size = Vector2(_Size.X - ScrollWidth.X, _Size.Y);
		Scroller->Location = Vector2(0, 0);
		Scroller->Size = Vector2(_Size.X - ScrollWidth.X, _Size.Y);
		Scrollbar->Location = Vector2(_Size.X - ScrollWidth.X, 0);
		Scrollbar->Size = Vector2(ScrollWidth.X, _Size.Y);


		// Build a tree based upon the control layout, so that we can group categories
		const std::vector<IControlLayout::SLayoutInstance*>* Instances = Layout->GetInstances();
		std::vector<Category> Categories;

		for(unsigned int i = 0; i < Instances->size(); ++i)
		{
			bool FoundCat = false;

			for(unsigned int c = 0; c < Categories.size(); ++c)
			{
				if(Instances->at(i)->Category == Categories[c].Name)
				{
					FoundCat = true;

					Instance I;
					I.ID = Instances->at(i)->ControlID;
					I.Name = Instances->at(i)->Name;
					Categories[c].Instances.push_back(I);

					break;
				}
			}

			if(!FoundCat)
			{
				Category C;
				C.Name = Instances->at(i)->Category;
				Instance I;
				I.ID = Instances->at(i)->ControlID;
				I.Name = Instances->at(i)->Name;
				C.Instances.push_back(I);
				Categories.push_back(C);


			}
		}

		// Useful scaling constants
		float Y = 10.0f / _Manager->GetResolution().Y;
		float YPlus = 2.0f / _Manager->GetResolution().Y;

		float LX = 10.0f / _Manager->GetResolution().X;
		float MX = LX + _Size.X * 0.5f;
		float SX = Size.X * 0.4f;

		float LH = 0.0f;

		for(unsigned int c = 0; c < Categories.size(); ++c)
		{
			// Create category
			ILabel* HL = _Manager->CreateLabel(_Name + "::Heading", Vector2(LX, Y), Vector2(0, 0));
			ILabel* HM = _Manager->CreateLabel(_Name + "::Heading", Vector2(MX + (MX * 0.2f), Y), Vector2(0, 0));
			HL->Parent = Scroller;
			HM->Parent = Scroller;

			Y += HL->Size.Y + (YPlus * 3);
			LH = HL->Size.Y + YPlus;

			HL->Text = Categories[c].Name;
			HM->Text = "Key";

			// Horizonal rule beneath heading.
			IPictureBox* HR = _Manager->CreatePictureBox(_Name + "::HeadingRule", Vector2(LX, Y - (YPlus * 1.5f)), Vector2((_Size.X - ScrollWidth.X) - (LX * 2.0f), 1.0f / _Manager->GetResolution().Y));
			HR->Parent = Scroller;

			// This is white on the default texture.
			HR->MinTexCoord = Vector2(0.97f, 0.97f);
			HR->MaxTexCoord = Vector2(1.0f, 1.0f);

			// Instances
			for(unsigned int i = 0; i < Categories[c].Instances.size(); ++i)
			{
				// Instance name and label
				ILabel* KL = _Manager->CreateLabel(_Name + "::Key", Vector2(LX, Y), Vector2(0, 0));
				ILabel* KM = _Manager->CreateLabel(_Name + "::Key", Vector2(MX, Y), Vector2(SX, KL->Size.Y));
				KL->Parent = Scroller;
				KM->Parent = Scroller;

				// Setup label to receive inputs
				KM->Tag = Layout->Get(Categories[c].Instances[i].Name);
				KM->Align = TextAlign_Center;
				KM->MouseEnter()->AddEvent(this, &CControlEditor::KM_MouseEnter);
				KM->MouseLeave()->AddEvent(this, &CControlEditor::KM_MouseLeave);
				KM->Click()->AddEvent(this, &CControlEditor::KM_Click);

				Y += KL->Size.Y + YPlus;
				LH = KL->Size.Y + YPlus;

				KL->Text = Categories[c].Instances[i].Name;
				KM->Text = ControlName(Categories[c].Instances[i].ID);
			}

			// Move the next heading down a bit
			Y += HL->Size.Y + (YPlus * 2);
		}

		// If we should be able to scroll, then activate the scrollbar
		if(Y > Scroller->Size.Y)
		{
			float Diff = Y - Scroller->Size.Y;
			Scroller->Size = Vector2(Scroller->Size.X, Y);

			Scrollbar->Minimum = 0;
			Scrollbar->Maximum = static_cast<int>(Y * _Manager->GetResolution().Y);
			Scrollbar->LargeChange = static_cast<int>(_Size.Y * _Manager->GetResolution().Y);
			Scrollbar->SmallChange = static_cast<int>(LH * _Manager->GetResolution().Y);
			Scrollbar->Value = 0;
		}else
		{
			Scrollbar->Minimum = 0;
			Scrollbar->Value = 0;
			Scrollbar->Maximum = 0;
		}

	}

	void CControlEditor::SetLayout(IControlLayout* layout)
	{
		// Layout forces rebuild.
		Layout = layout;
		RebuildMesh();
	}


	void CControlEditor::OnSizeChange()
	{
		IControl::OnSizeChange();
		this->RebuildMesh();
	}

	ControlEventHandler* CControlEditor::WaitForInputKeyCode() const
	{
		return WaitForInputKeyCodeEvent;
	}

}