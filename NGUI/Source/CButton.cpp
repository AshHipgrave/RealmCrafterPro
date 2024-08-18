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
#include "CButton.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type IButton::TypeOf()
		{
			return Type("CBUT", "CButton GUI Button");
		}

		CButton::CButton(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IButton(ScreenScale, Manager)
		{
			_Type = Type("CBUT", "CButton GUI Button");
			MeshBuffer = 0;
			Manager = 0;
			ButtonState = 0;
			UsingRight = false;
			_MouseOver = false;
			ClickEvent = new EventHandler();
			DownEvent = new EventHandler();
			RightClickEvent = new EventHandler();
			MouseEnterEvent = new EventHandler();
			MouseLeaveEvent = new EventHandler();
			MouseMoveEvent = new MouseEventHandler();

			UpTexture = 0;
			HoverTexture = 0;
			DownTexture = 0;
			_UseBorder = true;

			_Down = false;
		}

		CButton::~CButton()
		{
			delete ClickEvent;
			delete DownEvent;
			delete RightClickEvent;
			delete MouseEnterEvent;
			delete MouseLeaveEvent;
			delete MouseMoveEvent;

			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			if(UpTexture != 0)
				Manager->GetRenderer()->FreeTexture(UpTexture);
			if(HoverTexture != 0)
				Manager->GetRenderer()->FreeTexture(HoverTexture);
			if(DownTexture != 0)
				Manager->GetRenderer()->FreeTexture(DownTexture);
			UpTexture = 0;
			HoverTexture = 0;
			DownTexture = 0;
		}

		void CButton::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CButton::OnDeviceReset()
		{
			IControl::OnDeviceReset();

			CButton::OnTransform();
		}

		bool CButton::Update(GUIUpdateParameters* Parameters)
		{
			if(!_Enabled)
				return true;

			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;

				if(Parameters->MouseOver == false)
				{
					if(!_MouseOver)
					{
						_MouseOver = true;
						EventArgs E;
						this->MouseEnterEvent->Execute(this, &E);
					}
				}
				Parameters->MouseOver = true;
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;

				if(Parameters->MousePosition != LastMouse)
				{
					MouseEventArgs ME(Parameters->MousePosition);
					this->MouseMoveEvent->Execute(this, &ME);

					LastMouse = Parameters->MousePosition;
				}


				if((Parameters->LeftDown || Parameters->RightDown))
				{
					Manager->ControlFocus(this);

					EventArgs A;
					DownEvent->Execute(this, &A);
				}

				if((Parameters->LeftDown || Parameters->RightDown) && ButtonState == 1)
				{
					UsingRight =  Parameters->RightDown;
					ButtonState = 2;
					this->RebuildMesh();
				}else if((Parameters->LeftDown == false && Parameters->RightDown == false) && ButtonState == 2)
				{
					// Hit!
					ButtonState = 1;
					this->RebuildMesh();
					
					if(UsingRight)
					{
						EventArgs A;
						RightClickEvent->Execute(this, &A);
					}else
					{
						EventArgs A;
						ClickEvent->Execute(this, &A);
					}
					UsingRight = false;
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
				if(_MouseOver)
				{
					_MouseOver = false;
					EventArgs E;
					this->MouseLeaveEvent->Execute(this, &E);
				}

				UsingRight = false;
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

			LastMouse = Parameters->MousePosition;

			return false;
		}

		void CButton::Render()
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
			
			WinSize.Z += 5;
			WinSize.W += 5;

			Manager->CorrectRect(WinSize);
			Renderer->PushScissorRect(WinSize);
			MeshBuffer->Set();

			if(this->_UseBorder)
				Renderer->DrawMeshBuffer(MeshBuffer, 0, 16);

			if((this->ButtonState == 0 || this->_Enabled == false) && this->UpTexture != 0)
				Manager->SetTexture(this->UpTexture);
			else if(this->ButtonState == 1 && this->HoverTexture != 0)
				Manager->SetTexture(this->HoverTexture);
			else if(this->ButtonState == 2 && this->DownTexture != 0)
				Manager->SetTexture(this->DownTexture);
			else if(this->ButtonState == 1 && this->UpTexture != 0)
				Manager->SetTexture(this->UpTexture);
			else if(this->ButtonState == 2 && this->UpTexture != 0)
				Manager->SetTexture(this->UpTexture);

			Renderer->DrawMeshBuffer(MeshBuffer, 16, 2);	

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();
		}

		bool CButton::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;
			this->Caption = Manager->CreateLabel("Heh", Vector2(0, 0), Vector2(100, 100));
			this->Caption->Parent = this;
			this->Caption->Align = TextAlign_Center;
			this->Caption->VAlign = TextAlign_Middle;
			_ForeColor = this->Caption->ForeColor;

			return true;
		}

		void CButton::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Store all of the position/scale information of each quad
		#pragma region Quad Locations

			GUICoord Button_Border_T;
			GUICoord Button_Border_B;
			GUICoord Button_Border_L;
			GUICoord Button_Border_R;
			GUICoord Button_Border_TL;
			GUICoord Button_Border_BL;
			GUICoord Button_Border_TR;
			GUICoord Button_Border_BR;
			GUICoord Button_Fill;

			switch(ButtonState)
			{
			case 0:
				Button_Border_T = Button_Border_T_Up;
				Button_Border_B = Button_Border_B_Up;
				Button_Border_L = Button_Border_L_Up;
				Button_Border_R = Button_Border_R_Up;
				Button_Border_TL = Button_Border_TL_Up;
				Button_Border_BL = Button_Border_BL_Up;
				Button_Border_TR = Button_Border_TR_Up;
				Button_Border_BR = Button_Border_BR_Up;
				Button_Fill = Button_Fill_Up;
				break;
			case 1:
				Button_Border_T = Button_Border_T_Hover;
				Button_Border_B = Button_Border_B_Hover;
				Button_Border_L = Button_Border_L_Hover;
				Button_Border_R = Button_Border_R_Hover;
				Button_Fill = Button_Fill_Hover;
				Button_Border_TL = Button_Border_TL_Hover;
				Button_Border_BL = Button_Border_BL_Hover;
				Button_Border_TR = Button_Border_TR_Hover;
				Button_Border_BR = Button_Border_BR_Hover;
				break;
			case 2:
				Button_Border_T = Button_Border_T_Down;
				Button_Border_B = Button_Border_B_Down;
				Button_Border_L = Button_Border_L_Down;
				Button_Border_R = Button_Border_R_Down;
				Button_Border_TL = Button_Border_TL_Down;
				Button_Border_BL = Button_Border_BL_Down;
				Button_Border_TR = Button_Border_TR_Down;
				Button_Border_BR = Button_Border_BR_Down;
				Button_Fill = Button_Fill_Down;
				break;
			}

			if(_Enabled == false)
			{
				Button_Border_T = Button_Border_T_Disabled;
				Button_Border_B = Button_Border_B_Disabled;
				Button_Border_L = Button_Border_L_Disabled;
				Button_Border_R = Button_Border_R_Disabled;
				Button_Border_TL = Button_Border_TL_Disabled;
				Button_Border_BL = Button_Border_BL_Disabled;
				Button_Border_TR = Button_Border_TR_Disabled;
				Button_Border_BR = Button_Border_BR_Disabled;
				Button_Fill = Button_Fill_Disabled;
			}

			Vector2 TLp(0, 0);
			Vector2 TLs = Skin->GetScreenCoord(Button_Border_TL);

			Vector2 Tp(TLs.X, 0);
			Vector2 Ts(_Size.X - Skin->GetScreenCoord(Button_Border_TL).X - Skin->GetScreenCoord(Button_Border_TR).X, Skin->GetScreenCoord(Button_Border_T).Y);

			Vector2 TRp(Ts.X + TLs.X, 0);
			Vector2 TRs = Skin->GetScreenCoord(Button_Border_TR);

			Vector2 Lp(0, Skin->GetScreenCoord(Button_Border_TL).Y);
			Vector2 Ls = Vector2(Skin->GetScreenCoord(Button_Border_L).X, _Size.Y - Skin->GetScreenCoord(Button_Border_TL).Y - Skin->GetScreenCoord(Button_Border_BL).Y);

			Vector2 Rp(Ts.X + TLs.X, Skin->GetScreenCoord(Button_Border_TL).Y);
			Vector2 Rs = Vector2(Skin->GetScreenCoord(Button_Border_R).X, _Size.Y - Skin->GetScreenCoord(Button_Border_TR).Y - Skin->GetScreenCoord(Button_Border_BR).Y);
			
			Vector2 BLp(0, Rs.Y + TLs.Y);
			Vector2 BLs = Skin->GetScreenCoord(Button_Border_BL);
			
			Vector2 Bp(BLs.X, BLp.Y);
			Vector2 Bs(_Size.X - Skin->GetScreenCoord(Button_Border_BL).X - Skin->GetScreenCoord(Button_Border_BR).X, Skin->GetScreenCoord(Button_Border_B).Y);

			Vector2 BRp(Bs.X + BLs.X, Rs.Y + TRs.Y);
			Vector2 BRs = Skin->GetScreenCoord(Button_Border_BR);

			Vector2 Cp(TLs.X, Skin->GetScreenCoord(Button_Border_TL).Y);
			Vector2 Cs = Vector2(Ts.X, _Size.Y - Ts.Y - Bs.Y);

		#pragma endregion
			
			// Build the GUI
		#pragma region GUI Building

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			

			Builder.AddQuad(TLp, TLs, Skin->GetCoord(Button_Border_TL), Skin->GetSize(Button_Border_TL), _BackColor);
			Builder.AddQuad(Tp, Ts, Skin->GetCoord(Button_Border_T), Skin->GetSize(Button_Border_T), _BackColor);
			Builder.AddQuad(TRp, TRs, Skin->GetCoord(Button_Border_TR), Skin->GetSize(Button_Border_TR), _BackColor);
			Builder.AddQuad(Lp, Ls, Skin->GetCoord(Button_Border_L), Skin->GetSize(Button_Border_L), _BackColor);
			Builder.AddQuad(Rp, Rs, Skin->GetCoord(Button_Border_R), Skin->GetSize(Button_Border_R), _BackColor);
			Builder.AddQuad(BLp, BLs, Skin->GetCoord(Button_Border_BL), Skin->GetSize(Button_Border_BL), _BackColor);
			Builder.AddQuad(Bp, Bs, Skin->GetCoord(Button_Border_B), Skin->GetSize(Button_Border_B), _BackColor);
			Builder.AddQuad(BRp, BRs, Skin->GetCoord(Button_Border_BR), Skin->GetSize(Button_Border_BR), _BackColor);

			Vector2 Min = Skin->GetCoord(Button_Fill);
			Vector2 Max = Skin->GetSize(Button_Fill);

			if((this->ButtonState == 0 || this->_Enabled == false) && this->UpTexture != 0)
			{
				Min = Vector2(0, 0);
				Max = Vector2(1, 1);
			}
			else if((this->ButtonState == 1 && this->HoverTexture != 0) || (this->ButtonState == 1 && this->UpTexture != 0))
			{
				Min = Vector2(0, 0);
				Max = Vector2(1, 1);
			}
			else if((this->ButtonState == 2 && this->DownTexture != 0)
				|| (this->ButtonState == 2 && this->HoverTexture != 0)
				|| (this->ButtonState == 2 && this->UpTexture != 0))
			{
				Min = Vector2(0, 0);
				Max = Vector2(1, 1);
			}

			Builder.AddQuad(Cp, Cs, Min, Max, _BackColor);

		#pragma endregion

			// Create buffer
			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		EventHandler* CButton::Click()
		{
			return ClickEvent;
		}

		EventHandler* CButton::RightClick()
		{
			return RightClickEvent;
		}

		EventHandler* CButton::MouseDown()
		{
			return DownEvent;
		}

		EventHandler* CButton::MouseEnter()
		{
			return MouseEnterEvent;
		}

		EventHandler* CButton::MouseLeave()
		{
			return MouseLeaveEvent;
		}

		MouseEventHandler* CButton::MouseMove()
		{
			return MouseMoveEvent;
		}

		void CButton::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CButton::OnTransform()
		{
			IControl::OnTransform();
			
			if(_Skin == 0)
				return;

			ISkin* Skin = Manager->GetSkin(this->_Skin);

			GUICoord Button_Border_T;
			GUICoord Button_Border_B;
			GUICoord Button_Border_L;
			GUICoord Button_Border_R;

			switch(ButtonState)
			{
			case 0:
				Button_Border_T = Button_Border_T_Up;
				Button_Border_B = Button_Border_B_Up;
				Button_Border_L = Button_Border_L_Up;
				Button_Border_R = Button_Border_R_Up;
				break;
			case 1:
				Button_Border_T = Button_Border_T_Hover;
				Button_Border_B = Button_Border_B_Hover;
				Button_Border_L = Button_Border_L_Hover;
				Button_Border_R = Button_Border_R_Hover;
				break;
			case 2:
				Button_Border_T = Button_Border_T_Down;
				Button_Border_B = Button_Border_B_Down;
				Button_Border_L = Button_Border_L_Down;
				Button_Border_R = Button_Border_R_Down;
				break;
			}

			if(_Enabled == false)
			{
				Button_Border_T = Button_Border_T_Disabled;
				Button_Border_B = Button_Border_B_Disabled;
				Button_Border_L = Button_Border_L_Disabled;
				Button_Border_R = Button_Border_R_Disabled;
			}

			Vector2 Min(Skin->GetScreenCoord(Button_Border_L).X, Skin->GetScreenCoord(Button_Border_T).Y);
			Vector2 Max(Skin->GetScreenCoord(Button_Border_R).X, Skin->GetScreenCoord(Button_Border_B).Y);
			Max = _Size - Max;
			Max -= Min;

			this->Caption->Location = Min;//Vector2(0, 0);
			this->Caption->Size = Max;//_Size;
		}

		void CButton::OnTextChange()
		{
			IControl::OnTextChange();

			this->Caption->Text = _Text;
		}

		void CButton::OnEnabledChange()
		{
			IControl::OnEnabledChange();
			RebuildMesh();
		}

		void CButton::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		void CButton::OnForeColorChange()
		{
			IControl::OnForeColorChange();
			this->Caption->ForeColor = _ForeColor;
		}

		void CButton::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
			this->Caption->Font = Manager->GetSkin(_Skin)->GetFont(GUIFont_Control);
		}

		TextAlign CButton::zzGet_Align()
		{
			return Caption->Align;
		}

		TextAlign CButton::zzGet_VAlign()
		{
			return Caption->VAlign;
		}

		void CButton::zzPut_Align(TextAlign Alignment)
		{
			CButton::OnTransform();
			Caption->Align = Alignment;
		}

		void CButton::zzPut_VAlign(TextAlign Alignment)
		{
			CButton::OnTransform();
			Caption->VAlign = Alignment;
		}

		bool CButton::SetUpImage(std::string ImageFile)
		{
			return this->SetUpImageInternal(ImageFile, 0, false);
		}

		bool CButton::SetHoverImage(std::string ImageFile)
		{
			return this->SetHoverImageInternal(ImageFile, 0, false);
		}

		bool CButton::SetDownImage(std::string ImageFile)
		{
			return this->SetDownImageInternal(ImageFile, 0, false);
		}

		bool CButton::SetUpImage(std::string ImageFile, unsigned int Mask)
		{
			return this->SetUpImageInternal(ImageFile, Mask, true);
		}

		bool CButton::SetHoverImage(std::string ImageFile, unsigned int Mask)
		{
			return this->SetHoverImageInternal(ImageFile, Mask, true);
		}

		bool CButton::SetDownImage(std::string ImageFile, unsigned int Mask)
		{
			return this->SetDownImageInternal(ImageFile, Mask, true);
		}

		bool CButton::SetUpImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask)
		{
			// Free old texture
			if(UpTexture != 0)
				Manager->GetRenderer()->FreeTexture(UpTexture);
			UpTexture = 0;

			// Load new texture
			UpTexture = Manager->GetRenderer()->GetTexture(ImageFile.c_str(), Mask, UseMask);

			this->RebuildMesh();
			return true;
		}

		bool CButton::SetHoverImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask)
		{
			// Free old texture
			if(HoverTexture != 0)
				Manager->GetRenderer()->FreeTexture(HoverTexture);
			HoverTexture = 0;

			// Load new texture
			HoverTexture = Manager->GetRenderer()->GetTexture(ImageFile.c_str(), Mask, UseMask);

			this->RebuildMesh();
			return true;
		}

		bool CButton::SetDownImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask)
		{
			// Free old texture
			if(DownTexture != 0)
				Manager->GetRenderer()->FreeTexture(DownTexture);
			DownTexture = 0;

			// Load new texture
			DownTexture = Manager->GetRenderer()->GetTexture(ImageFile.c_str(), Mask, UseMask);

			this->RebuildMesh();
			return true;
		}

		void CButton::SetUpImage(void* iTexture)
		{
			if(iTexture == 0)
				return;

			if(UpTexture != 0)
				Manager->GetRenderer()->FreeTexture(UpTexture);
			UpTexture = 0;

			UpTexture = Manager->GetRenderer()->CreateTextureFromBase(iTexture, Math::Vector2(0, 0));

			RebuildMesh();
		}

		void CButton::SetHoverImage(void* iTexture)
		{
			if(iTexture == 0)
				return;

			if(HoverTexture != 0)
				Manager->GetRenderer()->FreeTexture(HoverTexture);
			HoverTexture = 0;

			HoverTexture = Manager->GetRenderer()->CreateTextureFromBase(iTexture, Math::Vector2(0, 0));

			RebuildMesh();
		}

		void CButton::SetDownImage(void* iTexture)
		{
			if(iTexture == 0)
				return;

			if(DownTexture != 0)
				Manager->GetRenderer()->FreeTexture(DownTexture);
			DownTexture = 0;

			DownTexture = Manager->GetRenderer()->CreateTextureFromBase(iTexture, Math::Vector2(0, 0));

			RebuildMesh();
		}

		bool CButton::zzGet_Down()
		{
			return _Down;
		}

		void CButton::zzPut_Down(bool Down)
		{
			_Down = Down;
		}

		void CButton::zzPut_UseBorder(bool UseBorder)
		{
			_UseBorder = UseBorder;
		}

		bool CButton::zzGet_UseBorder()
		{
			return _UseBorder;
		}

	}
}
