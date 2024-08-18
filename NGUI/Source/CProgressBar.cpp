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
#include "CProgressBar.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type IProgressBar::TypeOf()
		{
			return Type("CPRG", "CProgressBar GUI ProgressBar");
		}

		CProgressBar::CProgressBar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IProgressBar(ScreenScale, Manager)
		{
			_Type = Type("CPRG", "CProgressBar GUI ProgressBar");
			MeshBuffer = 0;
			Manager = 0;
			_Value = 0;
			Segs = 0;
			AccessibleWidth = 0.0f;
			AccessibleOffset = 0.0f;
			
			ButtonState = 0;
			UsingRight = _MouseOver = false;
			ClickEvent = new EventHandler();
			DownEvent = new EventHandler();
			RightClickEvent = new EventHandler();
			MouseEnterEvent = new EventHandler();
			MouseLeaveEvent = new EventHandler();
			MouseMoveEvent = new MouseEventHandler();
		}

		CProgressBar::~CProgressBar()
		{
			delete ClickEvent;
			delete DownEvent;
			delete RightClickEvent;
			delete MouseEnterEvent;
			delete MouseLeaveEvent;
			delete MouseMoveEvent;

			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CProgressBar::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CProgressBar::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CProgressBar::Update(GUIUpdateParameters* Parameters)
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
					Parameters->Handled = true;

					EventArgs A;
					DownEvent->Execute(this, &A);
				}

				if((Parameters->LeftDown || Parameters->RightDown) && ButtonState == 1)
				{
					UsingRight =  Parameters->RightDown;
					ButtonState = 2;
				}else if((Parameters->LeftDown == false && Parameters->RightDown == false) && ButtonState == 2)
				{
					// Hit!
					ButtonState = 1;
					
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
						ButtonState = 1;

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
				
				ButtonState = 0;
			}

			LastMouse = Parameters->MousePosition;

			return false;

			return false;
		}

		void CProgressBar::Render()
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

			WinSize.Z += 2;
			WinSize.W += 2;

			int OldLeft = WinSize.X;
			int OldTop = WinSize.Y;
			
			Manager->CorrectRect(WinSize);
			Renderer->PushScissorRect(WinSize);
			MeshBuffer->Set();

			Renderer->DrawMeshBuffer(MeshBuffer, 0, 6);

			float WidthScale = ((float)_Value) / 100.0f;
			OldLeft += AccessibleOffset * Manager->GetResolution().X;
			WinSize.Z = OldLeft + ((AccessibleWidth * WidthScale) * Manager->GetResolution().X);
			Manager->CorrectRect(WinSize);
			Renderer->PushScissorRect(WinSize);

			Renderer->DrawMeshBuffer(MeshBuffer, 6, Segs * 2);

			Renderer->PopScissorRect();

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();
		}

		bool CProgressBar::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			return true;
		}

		void CProgressBar::RebuildMesh()
		{
			if(_Locked)
				return;

			// Get skin
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Find out the number of center segments
			float SegCount = (_Size.X / Skin->GetScreenCoord(ProgressBar_Fill).X);
			Segs = (int)SegCount + 1;

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Builder.AddQuad(Vector2(0, 0), Skin->GetScreenCoord(ProgressBar_Left), Skin->GetCoord(ProgressBar_Left), Skin->GetSize(ProgressBar_Left), _BackColor);
			Builder.AddQuad(Vector2(Skin->GetScreenCoord(ProgressBar_Left).X, 0), Vector2(_Size.X - (Skin->GetScreenCoord(ProgressBar_Left).X + Skin->GetScreenCoord(ProgressBar_Right).X), Skin->GetScreenCoord(ProgressBar_Middle).Y), Skin->GetCoord(ProgressBar_Middle), Skin->GetSize(ProgressBar_Middle), _BackColor);
			Builder.AddQuad(Vector2(_Size.X - Skin->GetScreenCoord(ProgressBar_Right).X, 0), Skin->GetScreenCoord(ProgressBar_Right), Skin->GetCoord(ProgressBar_Right), Skin->GetSize(ProgressBar_Right), _BackColor);

			_Size.Y = Skin->GetScreenCoord(ProgressBar_Left).Y;
			AccessibleWidth = _Size.X - (Skin->GetScreenCoord(ProgressBar_Left).X + Skin->GetScreenCoord(ProgressBar_Right).X);
			AccessibleOffset = Skin->GetScreenCoord(ProgressBar_Left).X;

			Vector2 Pos = Vector2(AccessibleOffset, 0);
			Vector2 Sca = Skin->GetScreenCoord(ProgressBar_Fill);
			Vector2 Min = Skin->GetCoord(ProgressBar_Fill);
			Vector2 Max = Skin->GetSize(ProgressBar_Fill);
			for(int i = 0; i < Segs; ++i)
			{
				Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);
				Pos.X += Sca.X;
			}

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		void CProgressBar::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CProgressBar::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
		}

		int CProgressBar::zzGet_Value()
		{
			return _Value;
		}

		void CProgressBar::OnValueChange()
		{
			this->RebuildMesh();
		}

		void CProgressBar::zzPut_Value(int Value)
		{
			if(Value < 0)
				Value = 0;
			if(Value > 100)
				Value = 100;
			_Value = Value;
			OnValueChange();
		}

		EventHandler* CProgressBar::Click()
		{
			return ClickEvent;
		}

		EventHandler* CProgressBar::RightClick()
		{
			return RightClickEvent;
		}

		EventHandler* CProgressBar::MouseDown()
		{
			return DownEvent;
		}

		EventHandler* CProgressBar::MouseEnter()
		{
			return MouseEnterEvent;
		}

		EventHandler* CProgressBar::MouseLeave()
		{
			return MouseLeaveEvent;
		}

		MouseEventHandler* CProgressBar::MouseMove()
		{
			return MouseMoveEvent;
		}



	}
}
