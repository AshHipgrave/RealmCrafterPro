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
#include "CComboBox.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type IComboBox::TypeOf()
		{
			return Type("CCOM", "CComboBox GUI ComboBox");
		}

		CComboBox::CComboBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IComboBox(ScreenScale, Manager)
		{
			_Type = Type("CCOM", "CComboBox GUI ComboBox");
			MeshBuffer = 0;
			Manager = 0;
			ComboBoxState = 0;
			MainHeight = 0.0f;
			ListHeight = 0.0f;
			MainOffset = 0.0f;
			Open = false;
			Clicking = 0;
			SelectedEvent = new EventHandler();
		}

		CComboBox::~CComboBox()
		{
			delete SelectedEvent;

			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CComboBox::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CComboBox::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CComboBox::Update(GUIUpdateParameters* Parameters)
		{
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + Vector2(_Size.X, MainHeight)) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;

				if(Parameters->LeftDown)
					Manager->ControlFocus(this);

				if(ComboBoxState == 0 && Parameters->LeftDown == false)
				{
					ComboBoxState = 1;
					this->RebuildMesh();
				}else if(ComboBoxState == 1 && Parameters->LeftDown)
				{
					ComboBoxState = 2;
					Clicking = 1;
					this->RebuildMesh();
				}else if(ComboBoxState == 2 && Parameters->LeftDown == false && Clicking == 1)
				{
					Clicking = 0;
					Open = true;

					// Open listbox
					this->ListBox->Visible = true;
					this->BringToFront();

					if(this->ListBox->ItemCount() == 0)
					{
						this->ListBox->Visible = false;
						Open = false;
						ComboBoxState = 1;
						this->RebuildMesh();
					}
				}else if(ComboBoxState == 2 && Parameters->LeftDown == true && Clicking == 0)
				{
					Clicking = 2;
				}else if(ComboBoxState == 2 && Parameters->LeftDown == false && Clicking == 2)
				{
					Clicking = 0;
					ComboBoxState = 1;
					this->RebuildMesh();

					Open = false;
					// Close listbox
					this->ListBox->Visible = false;
				}


				Parameters->Handled = true;
				return true;
			}else if(_Visible && Parameters->MousePosition > _GlobalLocation + Vector2(0, MainHeight) && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				if(ComboBoxState == 1)
				{
					ComboBoxState = 0;
					this->RebuildMesh();
				}
			}else
			{
				if(ComboBoxState == 1)
				{
					ComboBoxState = 0;
					this->RebuildMesh();
				}

				if(Parameters->LeftDown)
				{
					Clicking = 0;
					ComboBoxState = 0;
					this->RebuildMesh();

					Open = false;
					// Close listbox
					this->ListBox->Visible = false;
				}
			}

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

			return false;
		}

		void CComboBox::Render()
		{
			if(!_Visible)
				return;
			if(_Skin == 0)
				return;
			if(MeshBuffer == 0)
				return;

			// Set us up for rendering
			Manager->SetSkin(_Skin);
			Manager->SetPosition(_GlobalLocation);

			// Get renderer
			IGUIRenderer* Renderer = Manager->GetRenderer();

			// Store old scissor region
			Vector4 OldRect = Renderer->GetScissorRect();

			// Create our scissor region
			Vector4 WinSize;
			WinSize.X = _GlobalLocation.X * Manager->GetResolution().X;
			WinSize.Y = _GlobalLocation.Y * Manager->GetResolution().Y;
			WinSize.Z = WinSize.X + (_Size.X * Manager->GetResolution().X);
			WinSize.W = WinSize.Y + (_Size.Y * Manager->GetResolution().Y);

			// Make sure we fit inside the parent region (We are a subparent control)
			if(WinSize.X < OldRect.X)
				WinSize.X = OldRect.X;
			if(WinSize.Z > OldRect.Z)
				WinSize.Z = OldRect.Z;
			if(WinSize.Y < OldRect.Y)
				WinSize.Y = OldRect.Y;
			if(WinSize.W > OldRect.W)
				WinSize.W = OldRect.W;

			WinSize.Z += 1;
			WinSize.W += 2;

			Manager->CorrectRect(WinSize);
			Renderer->PushScissorRect(WinSize);
			MeshBuffer->Set();

			Renderer->DrawMeshBuffer(MeshBuffer);

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();
		}

		bool CComboBox::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;
			this->Caption = Manager->CreateLabel("CCOMBOBOXSELECTION", Vector2(0, 0), Vector2(100, 100));
			this->Caption->Text = "";
			this->Caption->Parent = this;
			this->Caption->Align = TextAlign_Left;
			this->Caption->VAlign = TextAlign_Middle;

			this->ListBox = Manager->CreateListBox("CCOMBOBOXLISTBOX", Vector2(0, MainHeight), Vector2(_Size.X, _Size.Y - MainHeight));
			this->ListBox->Parent = this;
			this->ListBox->Visible = false;
			this->ListBox->Tag = this;
			this->ListBox->SelectedIndexChanged()->AddEvent(&ComboBoxSelectedEventCallback);

			return true;
		}

		void CComboBox::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Store all of the position/scale information of each quad
		#pragma region Quad Locations

			GUICoord ComboBox_Left = ComboBox_Left_Up;
			GUICoord ComboBox_Middle = ComboBox_Middle_Up;
			GUICoord ComboBox_Right = ComboBox_Right_Up;
			GUICoord ComboBox_Fill = ComboBox_Fill_Up;

			switch(ComboBoxState)
			{
			case 1:
				ComboBox_Left = ComboBox_Left_Hover;
				ComboBox_Middle = ComboBox_Middle_Hover;
				ComboBox_Right = ComboBox_Right_Hover;
				ComboBox_Fill = ComboBox_Fill_Hover;
				break;
			case 2:
				ComboBox_Left = ComboBox_Left_Down;
				ComboBox_Middle = ComboBox_Middle_Down;
				ComboBox_Right = ComboBox_Right_Down;
				ComboBox_Fill = ComboBox_Fill_Down;
				break;
			}

			if(_Enabled == false)
			{
				ComboBox_Left = ComboBox_Left_Disabled;
				ComboBox_Middle = ComboBox_Middle_Disabled;
				ComboBox_Right = ComboBox_Right_Disabled;
				ComboBox_Fill = ComboBox_Fill_Disabled;
			}

			Vector2 Lp(0, 0);
			Vector2 Ls(Skin->GetScreenCoord(ComboBox_Left));

			Vector2 Rs(Skin->GetScreenCoord(ComboBox_Right));
			Vector2 Rp(_Size.X - Rs.X, 0);

			Vector2 Mp(Ls.X, 0);
			Vector2 Ms(_Size.X - Ls.X - Rs.X, Skin->GetScreenCoord(ComboBox_Middle).Y);

			Vector2 Fs(Skin->GetScreenCoord(ComboBox_Fill));
			Vector2 Fp(_Size.X - Fs.X, 0);

			MainHeight = Ms.Y;
			ListHeight = _Size.Y - Ms.Y;
			MainOffset = Mp.X;


		#pragma endregion
			
			// Build the GUI
		#pragma region GUI Building

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Builder.AddQuad(Lp, Ls, Skin->GetCoord(ComboBox_Left), Skin->GetSize(ComboBox_Left), _BackColor);
			Builder.AddQuad(Mp, Ms, Skin->GetCoord(ComboBox_Middle), Skin->GetSize(ComboBox_Middle), _BackColor);
			Builder.AddQuad(Rp, Rs, Skin->GetCoord(ComboBox_Right), Skin->GetSize(ComboBox_Right),_BackColor);
			Builder.AddQuad(Fp, Fs, Skin->GetCoord(ComboBox_Fill), Skin->GetSize(ComboBox_Fill), _BackColor);

		#pragma endregion

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		void CComboBox::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();


			this->ListBox->Location = Vector2(0, MainHeight);
			this->ListBox->Size = Vector2(_Size.X, _Size.Y - MainHeight);	
		}

		void CComboBox::OnTransform()
		{
			IControl::OnTransform();

			this->Caption->Location = Vector2(MainOffset * 2.0f, 0);
			this->Caption->Size = Vector2(_Size.X, MainHeight);

		}

		void CComboBox::OnEnabledChange()
		{
			IControl::OnEnabledChange();
			RebuildMesh();
		}

		void CComboBox::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		void CComboBox::OnForeColorChange()
		{
			IControl::OnForeColorChange();
			this->Caption->ForeColor = _ForeColor;
		}

		void CComboBox::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
			this->Caption->Font = Manager->GetSkin(_Skin)->GetFont(GUIFont_Control);
			this->ListBox->Skin = _Skin;
		}



		int CComboBox::AddItem(std::string Value)
		{
			return ListBox->AddItem(Value);
		}

		int CComboBox::zzGet_SelectedIndex()
		{
			return ListBox->SelectedIndex;
		}

		void CComboBox::zzPut_SelectedIndex(int SelectedIndex)
		{
			ListBox->SelectedIndex = SelectedIndex;
			this->Caption->Text = SelectedValue;
		}

		std::string CComboBox::zzGet_SelectedValue()
		{
			return ListBox->SelectedValue;
		}

		void CComboBox::zzPut_SelectedValue(std::string SelectedValue)
		{
			ListBox->SelectedValue = SelectedValue;
		}

		void CComboBox::zzPut_ItemValue(int Index, std::string Value)
		{
			ListBox->ItemValue[Index] = Value;
		}

		std::string CComboBox::zzGet_ItemValue(int Index)
		{
			return ListBox->ItemValue[Index];
		}

		EventHandler* CComboBox::SelectedIndexChanged()
		{
			return SelectedEvent;
		}

		void CComboBox::DeleteItem(int Index)
		{
			ListBox->DeleteItem(Index);
		}

		int CComboBox::ItemCount()
		{
			return ListBox->ItemCount();
		}

		void CComboBox::ComboBoxSelectedEventCallback(IControl* Sender, EventArgs* E)
		{
			IListBox* L = (IListBox*)Sender;
			CComboBox* C = (CComboBox*)L->Tag;

			C->Caption->Text = C->SelectedValue;
			C->Open = false;
			C->ListBox->Visible = false;
			C->ComboBoxState = 0;
			C->RebuildMesh();

			C->SelectedIndexChanged()->Execute(C, E);
		}

	}
}
