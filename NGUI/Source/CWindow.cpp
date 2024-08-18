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
#include "CWindow.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type IWindow::TypeOf()
		{
			return Type("CWND", "CWindow GUI Window");
		}

		CWindow::CWindow(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IWindow(ScreenScale, Manager)
		{
			_Type = Type("CWND", "CWindow GUI Window");
			MeshBuffer = 0;
			Manager = 0;
			ButtonState = 0;
			Dragging = false;
			_CloseButton = true;
			_Modal = false;
			CloseEvent = new EventHandler();
			MoveEvent = new EventHandler();
		}

		CWindow::~CWindow()
		{
			delete CloseEvent;
			delete MoveEvent;

			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CWindow::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CWindow::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CWindow::Update(GUIUpdateParameters* Parameters)
		{
			if(_Visible && _Modal)
				Parameters->ModalProc = true;

			// Over Window
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;
				

				// We're over the window, bring to front if the mouse is down
				if(Parameters->LeftDown)
				{
					this->BringToFront();
					Manager->ControlFocus(this);
				}

				// Setup all of the positions for window elements
				Vector2 Closep;
				Vector2 Closes;
				
				ISkin* Skin = Manager->GetSkin(_Skin);
				Vector2 TLs = Skin->GetScreenCoord(Window_Border_TL);
				Vector2 Ts(_Size.X - Skin->GetScreenCoord(Window_Border_TL).X - Skin->GetScreenCoord(Window_Border_TR).X, Skin->GetScreenCoord(Window_Border_T).Y);
				Vector2 Rp(Ts.X + TLs.X, Skin->GetScreenCoord(Window_Border_TL).Y);
				Vector2 Cs = Vector2(Ts.X, Skin->GetScreenCoord(Window_Border_C).Y);
				Vector2 TitleSize = Vector2(Ts.X + TLs.X, Cs.Y) + Skin->GetScreenCoord(Window_Border_TR);

				#define CloseButtonA(CLOSE) Closep = Vector2(Rp.X - Skin->GetScreenCoord(CLOSE).X, Skin->GetScreenCoord(Window_Border_T).Y + (Skin->GetScreenCoord(Window_Border_C).Y / 2.0f) - (Skin->GetScreenCoord(CLOSE).Y / 2.0f));Closes = Skin->GetScreenCoord(CLOSE);

				// Obtain the button dimensions
				switch(this->ButtonState)
				{
				case 0:
					{
						CloseButtonA(Window_Close_Up);
						break;
					}
				case 1:
					{
						CloseButtonA(Window_Close_Hover);
						break;
					}
				case 2:
					{
						CloseButtonA(Window_Close_Down);
						break;
					}
				};
				TitleSize.X -= Closes.X;

				// Over button
				if(_CloseButton && Parameters->MousePosition > _GlobalLocation + Closep && Parameters->MousePosition < (_GlobalLocation + Closep + Closes) && Parameters->Handled == false)
				{
					// Click button
					if(Parameters->LeftDown && ButtonState == 1)
					{
						ButtonState = 2;
						this->RebuildMesh();
					}else if(Parameters->LeftDown == false && ButtonState == 2)
					{
						// Hit!
						ButtonState = 1;
						this->Visible = false;
						this->RebuildMesh();
						
						EventArgs A;
						CloseEvent->Execute(this, &A);
					}else
					{
						// Hover button
						if(ButtonState == 0)
						{
							ButtonState = 1;
							this->RebuildMesh();
						}
					}

					// We've technically handled an event
					Parameters->Handled = true;
					return true;
				}else
				{
					// Over title bar
					if(Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + TitleSize) && Parameters->Handled == false && ButtonState == 0)
					{
						// Drag it
						if(Parameters->LeftDown)
						{
							if(Dragging == false)
							{
								Dragging = true;
								DragOffset = _GlobalLocation - Parameters->MousePosition;
							}
						}

					}

					// Not over button, reset it
					if(ButtonState == 2 || ButtonState == 1)
					{
						ButtonState = 0;
						this->RebuildMesh();
					}


					// We're dragging
					if(Dragging)
					{
						// Not anymore
						if(Parameters->LeftDown == false)
						{
							Dragging = false;
							EventArgs A;
							MoveEvent->Execute(this, &A);
						}else
						{
							// Update locate
							Vector2 Out;
							if(this->Parent != 0)
								Out = this->Parent->Location;

							this->Location = ((Parameters->MousePosition + DragOffset) - Out);

							// Handle this as an event
							Parameters->Handled = true;
							return true;
						}
					}
				}

				// Nothing great happening with window, so update children
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

				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;
				
				// This window was handled
				Parameters->Handled = true;
				return true;
			}else
			{
				// Not over window, reset button
				if(ButtonState == 2 || ButtonState == 1)
				{
					ButtonState = 0;
					this->RebuildMesh();
				}
			}

			// Update dragging when not over window
			if(Dragging)
			{
				if(Parameters->LeftDown == false || Parameters->Handled == true)
				{
					Dragging = false;
					EventArgs A;
					MoveEvent->Execute(this, &A);
				}else
				{
					Vector2 Out;
					if(this->Parent != 0)
						Out = this->Parent->Location;

					this->Location = ((Parameters->MousePosition + DragOffset) - Out);

					Parameters->Handled = true;
					return true;
				}
			}

			#undef CloseButtonA

			return false;
		}

		void CWindow::BringToFront()
		{
			if(_Parent == 0)
				return;

			IControl::BringToFront();

			//IControl* Top = 0;
			//int Index = -1, i = 0;

			//// Go through every other controls
			//foreachptr(CIt, IControl, _Parent->Controls())
			//{
			//	IControl* C = (*CIt);

			//	if(C->GetType() == IWindow::TypeOf())
			//		Index = i;

			//	++i;
			//	nextptr(CIt, IControl, _Parent->Controls());
			//}

			//if(Index != 0)
			//{
			//	_Parent->Controls()->Remove(this);
			//	_Parent->Controls()->Insert(this, Index);
			//}
		}

		void CWindow::Render()
		{
			if(!_Visible)
				return;
			if(_Skin == 0)
				return;
			if(MeshBuffer == 0)
				return;

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

		bool CWindow::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			this->Caption = Manager->CreateLabel("", Vector2(0, 0), Vector2(100, 100));
			this->Caption->Parent = this;
			this->Caption->Align = TextAlign_Left;
			this->Caption->VAlign = TextAlign_Middle;

			return true;
		}

		void CWindow::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Store all of the position/scale information of each quad
		#pragma region Quad Locations
			Vector2 TLp(0, 0);
			Vector2 TLs = Skin->GetScreenCoord(Window_Border_TL);

			Vector2 Tp(TLs.X, 0);
			Vector2 Ts(_Size.X - Skin->GetScreenCoord(Window_Border_TL).X - Skin->GetScreenCoord(Window_Border_TR).X, Skin->GetScreenCoord(Window_Border_T).Y);

			Vector2 TRp(Ts.X + TLs.X, 0);
			Vector2 TRs = Skin->GetScreenCoord(Window_Border_TR);

			Vector2 Lp(0, Skin->GetScreenCoord(Window_Border_TL).Y);
			Vector2 Ls = Vector2(Skin->GetScreenCoord(Window_Border_L).X, _Size.Y - Skin->GetScreenCoord(Window_Border_TL).Y - Skin->GetScreenCoord(Window_Border_BL).Y);

			Vector2 Cp(TLs.X, Skin->GetScreenCoord(Window_Border_TL).Y);
			Vector2 Cs = Vector2(Ts.X, Skin->GetScreenCoord(Window_Border_C).Y);

			Vector2 Rp(Ts.X + TLs.X, Skin->GetScreenCoord(Window_Border_TL).Y);
			Vector2 Rs = Vector2(Skin->GetScreenCoord(Window_Border_R).X, _Size.Y - Skin->GetScreenCoord(Window_Border_TR).Y - Skin->GetScreenCoord(Window_Border_BR).Y);
			
			Vector2 BLp(0, Rs.Y + TLs.Y);
			Vector2 BLs = Skin->GetScreenCoord(Window_Border_BL);
			
			Vector2 Bp(BLs.X, BLp.Y);
			Vector2 Bs(_Size.X - Skin->GetScreenCoord(Window_Border_BL).X - Skin->GetScreenCoord(Window_Border_BR).X, Skin->GetScreenCoord(Window_Border_B).Y);

			Vector2 BRp(Bs.X + BLs.X, Rs.Y + TRs.Y);
			Vector2 BRs = Skin->GetScreenCoord(Window_Border_BR);

			Vector2 Fp(Ls.X, Cs.Y + Ts.Y);
			Vector2 Fs(Cs.X, _Size.Y - Ts.Y - Bs.Y - Cs.Y);
		#pragma endregion
			
			// Build the GUI
		#pragma region GUI Building
			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Builder.AddQuad(TLp, TLs, Skin->GetCoord(Window_Border_TL), Skin->GetSize(Window_Border_TL), _BackColor);
			Builder.AddQuad(Tp, Ts, Skin->GetCoord(Window_Border_T), Skin->GetSize(Window_Border_T), _BackColor);
			Builder.AddQuad(TRp, TRs, Skin->GetCoord(Window_Border_TR), Skin->GetSize(Window_Border_TR), _BackColor);
			Builder.AddQuad(Lp, Ls, Skin->GetCoord(Window_Border_L), Skin->GetSize(Window_Border_L), _BackColor);
			Builder.AddQuad(Cp, Cs, Skin->GetCoord(Window_Border_C), Skin->GetSize(Window_Border_C), _BackColor);
			Builder.AddQuad(Rp, Rs, Skin->GetCoord(Window_Border_R), Skin->GetSize(Window_Border_R), _BackColor);
			Builder.AddQuad(BLp, BLs, Skin->GetCoord(Window_Border_BL), Skin->GetSize(Window_Border_BL), _BackColor);
			Builder.AddQuad(Bp, Bs, Skin->GetCoord(Window_Border_B), Skin->GetSize(Window_Border_B), _BackColor);
			Builder.AddQuad(BRp, BRs, Skin->GetCoord(Window_Border_BR), Skin->GetSize(Window_Border_BR), _BackColor);
			Builder.AddQuad(Fp, Fs, Skin->GetCoord(Window_Fill), Skin->GetSize(Window_Fill), _BackColor);
		#pragma endregion

			// Build the button	
			if(_CloseButton)
			{
				switch(this->ButtonState)
				{
				case 0:
					{
						GUICoord CloseCoord = Window_Close_Up;
						Builder.AddQuad(Vector2(Rp.X - Skin->GetScreenCoord(CloseCoord).X, Skin->GetScreenCoord(Window_Border_T).Y + (Skin->GetScreenCoord(Window_Border_C).Y / 2.0f) - (Skin->GetScreenCoord(CloseCoord).Y / 2.0f)),
							Skin->GetScreenCoord(CloseCoord),
							Skin->GetCoord(CloseCoord),
							Skin->GetSize(CloseCoord), _BackColor);
						break;
					}
				case 1:
					{
						GUICoord CloseCoord = Window_Close_Hover;
						Builder.AddQuad(Vector2(Rp.X - Skin->GetScreenCoord(CloseCoord).X, Skin->GetScreenCoord(Window_Border_T).Y + (Skin->GetScreenCoord(Window_Border_C).Y / 2.0f) - (Skin->GetScreenCoord(CloseCoord).Y / 2.0f)),
							Skin->GetScreenCoord(CloseCoord),
							Skin->GetCoord(CloseCoord),
							Skin->GetSize(CloseCoord), _BackColor);
						break;
					}
				case 2:
					{
						GUICoord CloseCoord = Window_Close_Down;
						Builder.AddQuad(Vector2(Rp.X - Skin->GetScreenCoord(CloseCoord).X, Skin->GetScreenCoord(Window_Border_T).Y + (Skin->GetScreenCoord(Window_Border_C).Y / 2.0f) - (Skin->GetScreenCoord(CloseCoord).Y / 2.0f)),
							Skin->GetScreenCoord(CloseCoord),
							Skin->GetCoord(CloseCoord),
							Skin->GetSize(CloseCoord), _BackColor);
						break;
					}
				};
			}else{
				Builder.AddQuad(Vector2(0, 0), Vector2(0, 0), Vector2(0, 0), Vector2(0, 0), _BackColor);
			}

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		EventHandler* CWindow::Closed()
		{
			return CloseEvent;
		}

		EventHandler* CWindow::Move()
		{
			return MoveEvent;
		}

		void CWindow::OnTransform()
		{
			IControl::OnTransform();

			if(_Skin > 0)
			{
				ISkin* Skin = Manager->GetSkin(_Skin);
				Vector2 Loc(Skin->GetScreenCoord(Window_Border_TL).X, Skin->GetScreenCoord(Window_Border_T).Y);
				Vector2 Siz(_Size.X - Skin->GetScreenCoord(Window_Border_TL).X - Skin->GetScreenCoord(Window_Border_TR).X, Skin->GetScreenCoord(Window_Border_C).Y);
				this->ClientPosition = Vector2(Skin->GetScreenCoord(Window_Border_L).X, Skin->GetScreenCoord(Window_Border_T).Y + Skin->GetScreenCoord(Window_Border_C).Y);
				this->Caption->Location = Loc - this->ClientPosition;
				this->Caption->Size = Siz;
			}

		}

		void CWindow::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CWindow::OnTextChange()
		{
			IControl::OnTextChange();

			this->Caption->Text = std::string("  ") + _Text;
		}

		void CWindow::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		NGin::Math::Vector2 CWindow::GlobalLocation()
		{
			return _GlobalLocation + this->ClientPosition;
		}

		void CWindow::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();

			ISkin* Skin = Manager->GetSkin(_Skin);
			this->OnTransform();

			bool IsLocked = this->Caption->Locked;
			this->Caption->Locked = true;
			this->Caption->Font = Manager->GetSkin(_Skin)->GetFont(GUIFont_Window_Title);
			this->Caption->Locked = IsLocked;
			OnTransform();
		}

		void CWindow::zzPut_CloseButton(bool CloseButton)
		{
			_CloseButton = CloseButton;
			this->RebuildMesh();
		}

		bool CWindow::zzGet_CloseButton()
		{
			return _CloseButton;
		}

		bool CWindow::IsActiveWindow()
		{
			if(_Parent == 0)
				return false;

			if(*_Parent->Controls()->End() == this)
				return true;
			return false;
		}

		void CWindow::ResizeToClientSize(IControl* Control)
		{
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			Vector2 Ts(_Size.X - Skin->GetScreenCoord(Window_Border_TL).X - Skin->GetScreenCoord(Window_Border_TR).X, Skin->GetScreenCoord(Window_Border_T).Y);
			Vector2 Ls = Vector2(Skin->GetScreenCoord(Window_Border_L).X, _Size.Y - Skin->GetScreenCoord(Window_Border_TL).Y - Skin->GetScreenCoord(Window_Border_BL).Y);
			Vector2 Cs = Vector2(Ts.X, Skin->GetScreenCoord(Window_Border_C).Y);
			Vector2 Bs(_Size.X - Skin->GetScreenCoord(Window_Border_BL).X - Skin->GetScreenCoord(Window_Border_BR).X, Skin->GetScreenCoord(Window_Border_B).Y);
			Vector2 Fp(Ls.X, Cs.Y + Ts.Y);
			Vector2 Fs(Cs.X, _Size.Y - Ts.Y - Bs.Y - Cs.Y);

			Vector2 Set = Control->Size;

			if(Set.X > Fs.X)
				Set.X = Fs.X;

			if(Set.Y > Fs.Y)
				Set.Y = Fs.Y;

			Control->Size = Set;

		}

		void CWindow::zzPut_Modal(bool Modal)
		{
			_Modal = Modal;
		}

		bool CWindow::zzGet_Modal()
		{
			return _Modal;
		}
	}
}
