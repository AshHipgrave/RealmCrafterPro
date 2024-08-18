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
#include "CTabControl.h"
#include <SGUIMeshBuilder.h>
#include <windows.h>

using namespace NGin;
using namespace NGin::Math;

namespace NGin
{
	namespace GUI
	{
		Type ITabControl::TypeOf()
		{
			return Type("CTAB", "CTabControl GUI");
		}

		// ctor
		CTabControl::CTabControl(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : ITabControl(ScreenScale, Manager)
		{
			_Type = Type("CTAB", "CTabControl GUI");
			MeshBuffer = 0;
			SelectedTabIndex = 0;

			TabChangedEvent = new EventHandler();
		}

		// dtor
		CTabControl::~CTabControl()
		{
			delete TabChangedEvent;

			if(MeshBuffer != 0)
				_Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CTabControl::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CTabControl::OnDeviceReset()
		{
			IControl::OnDeviceReset();

			CTabControl::OnTransform();
		}

		// Update
		bool CTabControl::Update(GUIUpdateParameters* Parameters)
		{
			if(!_Enabled)
				return true;

			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				// Check buttons first
				std::vector<TabInstance> TInstances(Tabs.begin(), Tabs.end());
				for(int i = 0; i < TInstances.size(); ++i)
				{
					TInstances[i].Button->Update(Parameters);
				}

				// Buttons fine, update visible panel
				if(Parameters->Handled == false)
				{
					if(SelectedTabIndex >= 0 && SelectedTabIndex < Tabs.size())
					{
						Tabs[SelectedTabIndex].Panel->Update(Parameters);
					}
				}
			}

			return false;
		}

		void CTabControl::Render()
		{
			if(!_Visible)
				return;
			if(MeshBuffer == 0)
				return;
			if(_Skin == 0)
				return;

			// Get renderer
			IGUIRenderer* Renderer = _Manager->GetRenderer();

			// Store old scissor region
			Vector4 OldRect = Renderer->GetScissorRect();

			// Create our scissor region
			Vector4 WinSize;
			 			WinSize.X = (_GlobalLocation.X * _Manager->GetResolution().X);
			 			WinSize.Y = (_GlobalLocation.Y * _Manager->GetResolution().Y);
			 			WinSize.Z = (WinSize.X + (_Size.X * _Manager->GetResolution().X));
			 			WinSize.W = (WinSize.Y + (_Size.Y * _Manager->GetResolution().Y));		
			WinSize.Z += 5;
			WinSize.W += 5;

			_Manager->CorrectRect(WinSize);
			Renderer->PushScissorRect(WinSize);

			// Draw buttons first
			for(int i = 0; i < Tabs.size(); ++i)
			{
				Tabs[i].Button->Render();
			}

			// Set us up for rendering
			_Manager->SetSkin(_Skin);
			_Manager->SetPosition(_GlobalLocation);

			// Render border
			MeshBuffer->Set();
 
 			Renderer->DrawMeshBuffer(MeshBuffer);	

			// Draw panel/subcontrols
			if(SelectedTabIndex >= 0 && SelectedTabIndex < Tabs.size())
			{
				Tabs[SelectedTabIndex].Panel->Render();
			}

			Renderer->PopScissorRect();
		}

		bool CTabControl::Initialize()
		{
			return true;
		}

		// Add control
		int CTabControl::AddTab(std::string name, float width)
		{
			TabInstance Tab1;



			Tab1.Button = _Manager->CreateButton(_Name + "TABBUTTON", Vector2(0, 0), Vector2(width, 20 / _Manager->GetResolution().Y));
			Tab1.Button->Parent = this;
			Tab1.Panel = _Manager->CreatePictureBox(_Name + "PANEL", Vector2(0, 0), Vector2(0.1, 0.1));
			Tab1.Panel->BackColor = _BackColor;
			Tab1.Panel->MinTexCoord = _Manager->GetSkin(Tab1.Panel->Skin)->GetCoord(Window_Fill);
			Tab1.Panel->MaxTexCoord = Tab1.Panel->MinTexCoord + _Manager->GetSkin(Tab1.Panel->Skin)->GetSize(Window_Fill);
			Tab1.Panel->Parent = this;
			Tab1.Button->Text = name;
			Tab1.Button->Click()->AddEvent(this, &CTabControl::TabButton_Click);
			Tab1.Button->BackColor = _BackColor;
// 			Math::Color OldFore = Tab1.Button->ForeColor;
// 			OldFore.A = _BackColor.A;
// 			Tab1.Button->ForeColor = OldFore;

			Tabs.push_back(Tab1);
			RebuildMesh();
			return Tabs.size() - 1;
		}

		// Tab Count
		int CTabControl::GetTabCount()
		{
			return Tabs.size();
		}

		// Tab Index
		int CTabControl::GetSelectedTabIndex()
		{
			return SelectedTabIndex;
		}

		// Tab Width
		void CTabControl::SetTabWidth(int index, float width)
		{
			if(index < 0 || index >= Tabs.size())
				return;

			Tabs[index].Button->Size = Vector2(width, Tabs[index].Button->Size.Y);
		}

		// Remove Tab
		void CTabControl::RemoveTab(int index)
		{
			if(index < 0 || index >= Tabs.size())
				return;

			_Manager->Destroy(Tabs[index].Button);
			_Manager->Destroy(Tabs[index].Panel);
			Tabs.erase(Tabs.begin() + index);

			if(SelectedTabIndex == index)
				SwitchTo(0);
			else
				RebuildMesh();
		}

		// Get the panel handle of the control
		IPictureBox* CTabControl::TabPanel(int index)
		{
			if(index < 0 || index >= Tabs.size())
				return NULL;

			return Tabs[index].Panel;
		}

		// Tab clicked (change it)
		void CTabControl::TabButton_Click(IControl* sender, EventArgs* e)
		{
			for(int i = 0; i < Tabs.size(); ++i)
			{
				if(Tabs[i].Button == sender)
				{
					SwitchTo(i);
					return;
				}
			}
		}

		// Switch to given tab index
		void CTabControl::SwitchTo(int index)
		{
			if(index < 0 || index >= Tabs.size())
				return;

			for(int i = 0; i < Tabs.size(); ++i)
			{
				Tabs[i].Button->Enabled = true;
			}

			SelectedTabIndex = index;
			Tabs[index].Button->Enabled = false;
			RebuildMesh();

			EventArgs Args;
			if(TabChangedEvent != NULL)
				TabChangedEvent->Execute(this, &Args);
		}

		// Rebuild mesh
		void CTabControl::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			ISkin* Skin = _Manager->GetSkin(this->_Skin);
			if(Skin == NULL)
				return;

#pragma region Gadget Position

			float Y20 = 20 / _Manager->GetResolution().Y;
			float Y2 = 2 / _Manager->GetResolution().Y;
			float Y1 = 1 / _Manager->GetResolution().Y;
			float X50 = 50 / _Manager->GetResolution().X;

			float CumulativeX = 0.0f;
			for(int i = 0; i < Tabs.size(); ++i)
			{
				Tabs[i].Button->Location = Vector2(CumulativeX, (i == SelectedTabIndex) ? (Y2 + Y1) : Y1);
				Tabs[i].Button->Size = Vector2(Tabs[i].Button->Size.X, Y20);
				CumulativeX += Tabs[i].Button->Size.X;

				Tabs[i].Panel->Location = Vector2(0, Y20);
				Tabs[i].Panel->Size = Vector2(_Size.X, _Size.Y - Y20);
			}

#pragma endregion

			// Store all of the position/scale information of each quad
#pragma region Quad Locations



			Vector2 TLp(0, Y20);
			Vector2 TLs = Skin->GetScreenCoord(ListBox_Border_TL);

			Vector2 Tp(TLs.X, Y20);
			Vector2 Ts(_Size.X - Skin->GetScreenCoord(ListBox_Border_TL).X - Skin->GetScreenCoord(ListBox_Border_TR).X, Skin->GetScreenCoord(ListBox_Border_T).Y);

			Vector2 Coverp(0, Y20);
			Vector2 Covers(0, TLs.Y);

			if(SelectedTabIndex >= 0 && SelectedTabIndex < Tabs.size())
			{
				Coverp.X = Tabs[SelectedTabIndex].Button->Location.X;
				Covers.X = Tabs[SelectedTabIndex].Button->Size.X;
			}

			if(Coverp.X < TLs.X)
			{
				Coverp.X += TLs.X;
				Covers.X -= TLs.X;
			}


			Vector2 TRp(Ts.X + TLs.X, Y20);
			Vector2 TRs = Skin->GetScreenCoord(ListBox_Border_TR);

			Vector2 Lp(0, Skin->GetScreenCoord(ListBox_Border_TL).Y + Y20);
			Vector2 Ls = Vector2(Skin->GetScreenCoord(ListBox_Border_L).X, _Size.Y - Skin->GetScreenCoord(ListBox_Border_TL).Y - Skin->GetScreenCoord(ListBox_Border_BL).Y - (Y20 * 1));

			Vector2 Rp(Ts.X + TLs.X, Skin->GetScreenCoord(ListBox_Border_TL).Y + Y20);
			Vector2 Rs = Vector2(Skin->GetScreenCoord(ListBox_Border_R).X, _Size.Y - Skin->GetScreenCoord(ListBox_Border_TR).Y - Skin->GetScreenCoord(ListBox_Border_BR).Y - (Y20 * 1));

			Vector2 BLp(0, Rs.Y + TLs.Y + Y20);
			Vector2 BLs = Skin->GetScreenCoord(ListBox_Border_BL);

			Vector2 Bp(BLs.X, BLp.Y);
			Vector2 Bs(_Size.X - Skin->GetScreenCoord(ListBox_Border_BL).X - Skin->GetScreenCoord(ListBox_Border_BR).X, Skin->GetScreenCoord(ListBox_Border_B).Y);

			Vector2 BRp(Bs.X + BLs.X, Rs.Y + TRs.Y + Y20);
			Vector2 BRs = Skin->GetScreenCoord(ListBox_Border_BR);

			Vector2 Cp(TLs.X, Skin->GetScreenCoord(ListBox_Border_T).Y + Y20);
			Vector2 Cs = Vector2(Ts.X, _Size.Y - Ts.Y - Bs.Y - Y20);

#pragma endregion

			// Build the GUI
#pragma region GUI Building

			SGUIMeshBuilder Builder(_Manager->GetRenderer());

			Builder.AddQuad(TLp, TLs, Skin->GetCoord(ListBox_Border_TL), Skin->GetSize(ListBox_Border_TL), _BackColor);
			Builder.AddQuad(Tp, Ts, Skin->GetCoord(ListBox_Border_T), Skin->GetSize(ListBox_Border_T), _BackColor);
			Builder.AddQuad(TRp, TRs, Skin->GetCoord(ListBox_Border_TR), Skin->GetSize(ListBox_Border_TR), _BackColor);
			Builder.AddQuad(Lp, Ls, Skin->GetCoord(ListBox_Border_L), Skin->GetSize(ListBox_Border_L), _BackColor);
			Builder.AddQuad(Cp, Cs, Skin->GetCoord(ListBox_Fill), Skin->GetSize(ListBox_Fill), _BackColor);
			Builder.AddQuad(Rp, Rs, Skin->GetCoord(ListBox_Border_R), Skin->GetSize(ListBox_Border_R), _BackColor);
			Builder.AddQuad(BLp, BLs, Skin->GetCoord(ListBox_Border_BL), Skin->GetSize(ListBox_Border_BL), _BackColor);
			Builder.AddQuad(Bp, Bs, Skin->GetCoord(ListBox_Border_B), Skin->GetSize(ListBox_Border_B), _BackColor);
			Builder.AddQuad(BRp, BRs, Skin->GetCoord(ListBox_Border_BR), Skin->GetSize(ListBox_Border_BR), _BackColor);

			Builder.AddQuad(Coverp, Covers, Skin->GetCoord(ListBox_Fill), Skin->GetSize(ListBox_Fill), _BackColor);

#pragma endregion

			// Create buffer
			if(MeshBuffer != 0)
				_Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();


		}

		EventHandler* CTabControl::TabChanged()
		{
			return TabChangedEvent;
		}

		void CTabControl::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CTabControl::OnTransform()
		{
			IControl::OnTransform();
		}

		void CTabControl::OnTextChange()
		{
			IControl::OnTextChange();
		}

		void CTabControl::OnEnabledChange()
		{
			IControl::OnEnabledChange();
			RebuildMesh();
		}

		void CTabControl::OnBackColorChange()
		{
			IControl::OnBackColorChange();

			for(int i = 0; i < Tabs.size(); ++i)
			{
				Tabs[i].Button->BackColor = _BackColor;
				Math::Color OldFore = Tabs[i].Button->ForeColor;
				OldFore.A = _BackColor.A;
				Tabs[i].Button->ForeColor = OldFore;

				Tabs[i].Panel->BackColor = _BackColor;
			}


			this->RebuildMesh();
		}

		void CTabControl::OnForeColorChange()
		{
		}

		void CTabControl::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
		}


	}
}
