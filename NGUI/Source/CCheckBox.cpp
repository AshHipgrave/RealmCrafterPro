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
#include "CCheckBox.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type ICheckBox::TypeOf()
		{
			return Type("CCHK", "CCheckBox GUI Checkbox");
		}

		CCheckBox::CCheckBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : ICheckBox(ScreenScale, Manager)
		{
			_Type = Type("CCHK", "CCheckBox GUI Checkbox");
			MeshBuffer = 0;
			Manager = 0;
			ButtonState = 0;
			_Checked = false;
			CheckEvent = new EventHandler();
		}

		CCheckBox::~CCheckBox()
		{
			delete CheckEvent;

			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CCheckBox::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CCheckBox::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CCheckBox::Update(GUIUpdateParameters* Parameters)
		{
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;
				
				if(Parameters->LeftDown)
					Manager->ControlFocus(this);

				if(Parameters->LeftDown && ButtonState == 1)
				{
					ButtonState = 2;
					this->RebuildMesh();
				}else if(Parameters->LeftDown == false && ButtonState == 2)
				{
					// Hit!
					ButtonState = 1;
					this->Checked = !this->Checked;
					this->RebuildMesh();
					
					EventArgs A;
					CheckEvent->Execute(this, &A);
				}else
				{
					if(ButtonState == 0)
					{
						ButtonState = 1;
						this->RebuildMesh();
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
				}

				Parameters->Handled = true;
				return true;
			}else
			{
				if(ButtonState == 2)
				{
					ButtonState = 1;
					this->RebuildMesh();
				}else if(ButtonState == 1)
				{
					ButtonState = 0;
					this->RebuildMesh();
				}
			}

			return false;
		}

		void CCheckBox::Render()
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
			WinSize.W += 1;

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

		bool CCheckBox::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			this->Caption = Manager->CreateLabel("", Vector2(0, 0), Vector2(100, 100));
			this->Caption->Parent = this;
			this->Caption->Align = TextAlign_Left;
			this->Caption->VAlign = TextAlign_Middle;

			return true;
		}

		void CCheckBox::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Store all of the position/scale information of each quad
		#pragma region Quad Locations

			GUICoord CheckBox_Fill;

			switch(ButtonState)
			{
			case 0:
				CheckBox_Fill = CheckBox_Fill_Up;
				break;
			case 1:
				CheckBox_Fill = CheckBox_Fill_Hover;
				break;
			case 2:
				CheckBox_Fill = CheckBox_Fill_Down;
				break;
			}

			if(_Enabled == false)
				CheckBox_Fill = CheckBox_Fill_Disabled;

			Vector2 Cp(0, 0);
			Vector2 Cs = Skin->GetScreenCoord(CheckBox_Fill);
			Vector2 Ccs = Cs;

			if(!_Checked)
				Ccs = Cp;

		#pragma endregion
			
			// Build the GUI
		#pragma region GUI Building

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Builder.AddQuad(Cp, Cs, Skin->GetCoord(CheckBox_Fill), Skin->GetSize(CheckBox_Fill), _BackColor);
			Builder.AddQuad(Cp, Ccs, Skin->GetCoord(CheckBox_Check), Skin->GetSize(CheckBox_Check), _BackColor);

		#pragma endregion

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		EventHandler* CCheckBox::CheckedChange()
		{
			return CheckEvent;
		}

		bool CCheckBox::zzGet_Checked()
		{
			return _Checked;
		}

		void CCheckBox::zzPut_Checked(bool Check)
		{
			_Checked = Check;
			RebuildMesh();
		}

		void CCheckBox::OnTransform()
		{
			IControl::OnTransform();

			if(_Skin > 0)
			{
				ISkin* Skin = Manager->GetSkin(_Skin);
				Vector2 Loc(Skin->GetScreenCoord(CheckBox_Fill_Up).X, 0);
				Vector2 Size(_Size.X - Loc.X, /*_Size.Y -*/ Skin->GetScreenCoord(CheckBox_Fill_Up).Y);
				this->Caption->Location = Loc + Vector2(5 / Manager->GetResolution().X, 0);
				this->Caption->Size = Size;
			}
		}

		void CCheckBox::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CCheckBox::OnTextChange()
		{
			IControl::OnTextChange();

			this->Caption->Text = _Text;
		}

		void CCheckBox::OnEnabledChange()
		{
			IControl::OnEnabledChange();
			RebuildMesh();
		}

		void CCheckBox::OnForeColorChange()
		{
			IControl::OnForeColorChange();

			this->Caption->ForeColor = _ForeColor;
		}

		void CCheckBox::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();

			bool IsLocked = this->Caption->Locked;
			this->Caption->Locked = true;
			this->Caption->Font = Manager->GetSkin(_Skin)->GetFont(GUIFont_Control);
			this->Caption->Locked = IsLocked;
			OnTransform();
		}

	}
}
