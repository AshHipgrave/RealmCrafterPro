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
#include "CTrackBar.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type ITrackBar::TypeOf()
		{
			return Type("CTRB", "CTrackBar GUI TrackBar");
		}

		CTrackBar::CTrackBar(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : ITrackBar(ScreenScale, Manager)
		{
			_Type = Type("CTRB", "CTrackBar GUI TrackBar");
			MeshBuffer = 0;
			Manager = 0;
			_Value = 10;
			_Minimum = 0;
			_Maximum = 10;
			_TickFrequency = 1;
			ButtonState = 0;
			AccessibleOffset = 0.0f;
			AccessibleSize = Vector2(0,0);
			TickJump = 0;
			Dragging = false;

			ValueChangedEvent = new EventHandler();
		}

		CTrackBar::~CTrackBar()
		{
			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CTrackBar::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CTrackBar::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CTrackBar::Update(GUIUpdateParameters* Parameters)
		{
			//if(_Visible && Parameters->MousePosition > Vector2(_GlobalLocation.X + AccessibleOffset, _GlobalLocation.Y) && Parameters->MousePosition < (Vector2(_GlobalLocation.X + AccessibleOffset, _GlobalLocation.Y) + AccessibleSize) && Parameters->Handled == false)
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;

				// Its inside the control area, so any clicks activate a focus
				if(Parameters->LeftDown)
					Manager->ControlFocus(this);

				if(_Visible && Parameters->MousePosition > Vector2(_GlobalLocation.X + AccessibleOffset, _GlobalLocation.Y) && Parameters->MousePosition < (Vector2(_GlobalLocation.X + AccessibleOffset, _GlobalLocation.Y) + AccessibleSize) && Parameters->Handled == false)
				{
					if(Parameters->LeftDown && ButtonState == 1)
					{
						ButtonState = 2;
						this->RebuildMesh();
					}else if(Parameters->LeftDown == false && ButtonState == 2)
					{
						ButtonState = 1;
						this->RebuildMesh();
					}else
					{
						if(ButtonState == 0)
						{
							ButtonState = 1;
							this->RebuildMesh();
						}
					}
				}else
				{
					int Distance = _Maximum - _Minimum;
					int Ticks = Distance / _TickFrequency;
					--Ticks;

					float SX = 0.0f;

					float MX = Parameters->MousePosition.X - _GlobalLocation.X;

					if(Parameters->LeftDown)
					{
						for(int i = 0; i < Ticks + 2; ++i)
						{
							if(MX > SX && MX < SX + TickJump)
							{
								_Value = (i * _TickFrequency) + _Minimum;

								// Event
								EventArgs A;
								ValueChangedEvent->Execute(this, &A);

								this->RebuildMesh();
								break;
							}
							SX += TickJump;
						}
					}else
					{
						ButtonState = 0;
						this->RebuildMesh();
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

		void CTrackBar::Render()
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

			Renderer->DrawMeshBuffer(MeshBuffer);

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();
		}

		bool CTrackBar::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			return true;
		}

		void CTrackBar::RebuildMesh()
		{
			if(_Locked)
				return;

			// Get skin
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Vertex/Index counts
			int VertexCount = 16, IndexCount = 24;

			// Calculate the tick count
			int Distance = _Maximum - _Minimum;
			int Ticks = Distance / _TickFrequency;
			--Ticks;

			GUICoord TrackBar_Button = TrackBar_Button_Up;

			if(ButtonState == 1)
				TrackBar_Button = TrackBar_Button_Hover;
			else if(ButtonState == 2)
				TrackBar_Button = TrackBar_Button_Down;

			Vector2 HalfOffset = Skin->GetScreenCoord(TrackBar_Button) * Vector2(0.5f, 0.5f);
			Vector2 HalfBar(0.0f, Skin->GetScreenCoord(TrackBar_BackGround_Left).Y * 0.5f);

			float AllowedDistance = _Size.X - Skin->GetScreenCoord(TrackBar_Button).X;
			float fValue = (float)(_Value - _Minimum);
			float fMax = (float)(_Maximum - _Minimum);
			float fScale = fValue / fMax;
			float SetDistance = AllowedDistance * fScale;
			AccessibleOffset = SetDistance;
			AccessibleSize = Skin->GetScreenCoord(TrackBar_Button);

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Vector2 Pos = HalfOffset - HalfBar;
			Builder.AddQuad(Pos, Skin->GetScreenCoord(TrackBar_BackGround_Left), Skin->GetCoord(TrackBar_BackGround_Left), Skin->GetSize(TrackBar_BackGround_Left), _BackColor);
			Pos.X = _Size.X - Pos.X - Skin->GetScreenCoord(TrackBar_BackGround_Right).X;
			Vector2 Sca = Skin->GetScreenCoord(TrackBar_BackGround_Right);
			Builder.AddQuad(Pos, Sca, Skin->GetCoord(TrackBar_BackGround_Right), Skin->GetSize(TrackBar_BackGround_Right), _BackColor);

			Sca.X = Pos.X - Skin->GetScreenCoord(TrackBar_BackGround_Left).X;
			Pos = HalfOffset-HalfBar;
			Pos.X += Skin->GetScreenCoord(TrackBar_BackGround_Left).X;

			Builder.AddQuad(Pos, Sca, Skin->GetCoord(TrackBar_BackGround_Middle), Skin->GetSize(TrackBar_BackGround_Middle), _BackColor);
			Builder.AddQuad(Vector2(SetDistance, 0), Skin->GetScreenCoord(TrackBar_Button), Skin->GetCoord(TrackBar_Button), Skin->GetSize(TrackBar_Button), _BackColor);

			Sca = Skin->GetScreenCoord(TrackBar_Tick_Small);
			Vector2 Min = Skin->GetCoord(TrackBar_Tick_Small);
			Vector2 Max = Skin->GetSize(TrackBar_Tick_Small);
			Pos = Skin->GetScreenCoord(TrackBar_Button);
			Pos.X *= 0.5f;
			TickJump = (_Size.X - (1.0f * (Pos.X * 2.0f))) / ((float)(Ticks + 1));

			for(int i = 0; i < Ticks; ++i)
			{
				Pos.X += TickJump;
				Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);
			}

			Pos.X += TickJump;
			Sca = Skin->GetScreenCoord(TrackBar_Tick_Large);
			Min = Skin->GetCoord(TrackBar_Tick_Large);
			Max = Skin->GetSize(TrackBar_Tick_Large);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Skin->GetScreenCoord(TrackBar_Button);
			Pos.X *= 0.5f;
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);
			_Size.Y = Pos.Y + Sca.Y;

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		void CTrackBar::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CTrackBar::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
		}

		int CTrackBar::zzGet_Value()
		{
			return _Value;
		}

		void CTrackBar::OnValueChange()
		{
			if(_Value < _Minimum)
				_Value = _Minimum;
			if(_Value > _Maximum)
				_Value = _Maximum;
			this->RebuildMesh();
		}

		void CTrackBar::zzPut_Value(int Value)
		{
			_Value = Value;
			OnValueChange();
		}

		int CTrackBar::zzGet_Minimum()
		{
			return _Minimum;
		}

		int CTrackBar::zzGet_Maximum()
		{
			return _Maximum;
		}

		int CTrackBar::zzGet_TickFrequency()
		{
			return _TickFrequency;
		}

		void CTrackBar::zzPut_Minimum(int Minimum)
		{
			_Minimum = Minimum;
			OnValueChange();
		}

		void CTrackBar::zzPut_Maximum(int Maximum)
		{
			_Maximum = Maximum;
			OnValueChange();
		}

		void CTrackBar::zzPut_TickFrequency(int TickFrequency)
		{
			_TickFrequency = TickFrequency;
			OnValueChange();
		}


		EventHandler* CTrackBar::ValueChanged()
		{
			return ValueChangedEvent;
		}


	}
}
