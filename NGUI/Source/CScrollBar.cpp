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
#include "CScrollBar.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type IScrollBar::TypeOf()
		{
			return Type("CSRL", "CScrollBar GUI ScrollBar");
		}

		CScrollBar::CScrollBar(NGin::Math::Vector2& ScreenScale, ScrollOrientation ScrollOrientation, IGUIManager* Manager) : IScrollBar(ScreenScale, Manager)
		{
			_Type = Type("CSRL", "CScrollBar GUI ScrollBar");
			MeshBuffer = 0;
			Manager = 0;

			ScrollType =  ScrollOrientation;
			_LargeChange = 10;
			_SmallChange = 1;
			_Minimum = 0;
			_Maximum = 0;
			_Value = 0;
			UpState = 0;
			DownState = 0;
			GripState = 0;
			Dragging = false;
			DragOffset = 0.0f;

			ScrollEvent = new ScrollEventHandler();
		}

		CScrollBar::~CScrollBar()
		{
			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CScrollBar::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CScrollBar::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CScrollBar::Update(GUIUpdateParameters* Parameters)
		{
			if(!_Enabled)
			{
				if(UpState != 0)
				{
					UpState = 0;
					this->RebuildMesh();
				}
				
				if(DownState != 0)
				{
					DownState = 0;
					this->RebuildMesh();
				}
				
				if(GripState != 0)
				{
					GripState = 0;
					this->RebuildMesh();
				}

				return false;
			}
		if(ScrollType == VerticalScroll)
		{
		#pragma region Vertical
			ISkin* Skin = Manager->GetSkin(this->_Skin);
			GUICoord ScrollBar_Up = VScrollBar_Normal_Up;
			GUICoord ScrollBar_Down = VScrollBar_Normal_Down;

			if(UpState == 1)
				ScrollBar_Up = VScrollBar_Hover_Up;
			else if(UpState == 2)
				ScrollBar_Up = VScrollBar_Down_Up;
			
			if(DownState == 1)
				ScrollBar_Down = VScrollBar_Hover_Down;
			else if(DownState == 2)
				ScrollBar_Down = VScrollBar_Down_Down;


			float Width = Skin->GetScreenCoord(VScrollBar_Fill).X;
			float Height = _Size.Y;
			float UpHeight = Skin->GetScreenCoord(ScrollBar_Up).Y;
			float DnHeight = Skin->GetScreenCoord(ScrollBar_Down).Y;
			float CenterHeight = Height - UpHeight - DnHeight;
			

			//float ScrollMin = Skin->GetScreenCoord(ScrollBar_Top).Y + Skin->GetScreenCoord(ScrollBar_Bottom).Y + Skin->GetScreenCoord(ScrollBar_Grip).Y;
			
			float fMax, fMin, fLargeChange, fValue;
			fMax = (float)_Maximum;
			fMin = (float)_Minimum;
			fLargeChange = (float)_LargeChange;
			fValue = (float)_Value;

			float BarHeight = CenterHeight * (fLargeChange / (fMax - fMin));
			float ScrollArea = CenterHeight - BarHeight;
			float ScrollScale = (1.0f / ((fMax - fMin - fLargeChange) / (fValue - fMin)));
			if(ScrollScale > 1.0f)
				ScrollScale = 1.0f;
			float ScrollPos =  ScrollArea * ScrollScale;

			// Check we're over the gadget itself
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;

				if(Parameters->LeftDown)
					Manager->ControlFocus(this);

				Vector2 MousePos = Parameters->MousePosition - _GlobalLocation;
				bool Handled = false;

				if(MousePos < Skin->GetScreenCoord(ScrollBar_Up))
				{
					Handled = true;

					if(Parameters->LeftDown == true && UpState != 2)
					{
						UpState = 2;

						// Down!
						int LastValue = _Value;
						_Value -= _SmallChange;
						if(_Value < _Minimum)
							_Value = _Minimum;
						else
						{
							ScrollEventArgs A(_Value, LastValue, VerticalScroll);
							this->ScrollEvent->Execute(this, &A);
							this->RebuildMesh();
						}

						
					}else if(Parameters->LeftDown == false && UpState == 2)
					{
						UpState = 1;
						this->RebuildMesh();			
					}else if(UpState != 1)
					{
						UpState = 1;
						this->RebuildMesh();
					}
				}else if(UpState != 0)
				{
					UpState = 0;
					this->RebuildMesh();
				}

				if(MousePos.Y > _Size.Y - Skin->GetScreenCoord(ScrollBar_Down).Y)
				{
					Handled = true;

					if(Parameters->LeftDown == true && DownState != 2)
					{
						// Down!
						DownState = 2;
						
						int LastValue = _Value;
						_Value += _SmallChange;
						if(_Value > (_Maximum - _LargeChange + 1))
							_Value = (_Maximum - _LargeChange + 1);
						else
						{
							ScrollEventArgs A(_Value, LastValue, VerticalScroll);
							this->ScrollEvent->Execute(this, &A);
							this->RebuildMesh();
						}
					}else if(Parameters->LeftDown == false && DownState == 2)
					{
						// Hit!
						DownState = 1;
						this->RebuildMesh();
					}else if(DownState != 1)
					{
						DownState = 1;
						this->RebuildMesh();
					}
				}else if(DownState != 0)
				{
					DownState = 0;
					this->RebuildMesh();
				}

				if(MousePos.Y > Skin->GetScreenCoord(ScrollBar_Up).Y && MousePos.Y < _Size.Y - Skin->GetScreenCoord(ScrollBar_Down).Y)
					//MousePos.Y > ScrollPos + UpHeight && MousePos.Y < ScrollPos + UpHeight + BarHeight)
				{
					if(Parameters->LeftDown == true)
					{
						if(GripState != 2)
						{
							GripState = 2;
							this->RebuildMesh();
						}

						if(Dragging == false)
						{
							if(MousePos.Y > ScrollPos + UpHeight && MousePos.Y < ScrollPos + UpHeight + BarHeight)
							{
								Dragging = true;
								DragOffset = MousePos.Y;
							}
						}else
						{
			//				float MoveAmount = (MousePos.Y - DragOffset);
			//				float NewPosition = MoveAmount + ScrollPos;



			//float fMax, fMin, fLargeChange, fValue;
			//fMax = (float)_Maximum;
			//fMin = (float)_Minimum;
			//fLargeChange = (float)_LargeChange;
			//fValue = (float)_Value;

			//float BarHeight = CenterHeight * (fLargeChange / (fMax - fMin));
			//float ScrollArea = CenterHeight - BarHeight;
			//float ScrollScale = (1.0f / ((fMax - fMin - fLargeChange) / (fValue - fMin)));
			//if(ScrollScale > 1.0f)
			//	ScrollScale = 1.0f;
			//float ScrollPos =  ScrollArea * ScrollScale;


							float MoveAmount = (MousePos.Y - DragOffset) + ScrollPos;
							MoveAmount /= ScrollArea; // Find the ScrollScale
							MoveAmount = 1.0f / MoveAmount;
							MoveAmount = (fMax - fMin - fLargeChange) / MoveAmount;
							//MoveAmount = (fMax - fMin - fLargeChange) * MoveAmount;
							MoveAmount += fMin;

							if(MoveAmount > (fMax - fMin - fLargeChange) + 1.0f)
								MoveAmount = (fMax - fMin - fLargeChange) + 1.0f;
							if(MoveAmount < 0.0f)
								MoveAmount = 0.0f;

							if((int)(MoveAmount + 0.5f) != _Value)
							{
								int LastValue = _Value;
								_Value = (int)(MoveAmount + 0.5f);

								DragOffset = MousePos.Y;
								this->RebuildMesh();

								ScrollEventArgs A(_Value, LastValue, VerticalScroll);
								this->ScrollEvent->Execute(this, &A);
							}
						}

					}else if(Parameters->LeftDown == false && GripState == 2)
					{
						GripState = 1;
						Dragging = false;
						this->RebuildMesh();


					}else if(GripState != 1)
					{
						GripState = 1;
						this->RebuildMesh();
					}
				}else if(Dragging == true && Handled == false)
				{
					//float MoveAmount = (MousePos.Y - DragOffset) + ScrollPos;
					//MoveAmount /= ScrollArea; // Find the ScrollScale
					//MoveAmount = 1.0f / MoveAmount;
					//MoveAmount = (fMax - fMin - fLargeChange) / MoveAmount;
					//MoveAmount += fMin;

					//if(MoveAmount > (fMax - fMin - fLargeChange))
					//	MoveAmount = (fMax - fMin - fLargeChange);
					//if(MoveAmount < 0.0f)
					//	MoveAmount = 0.0f;

					//if((int)(MoveAmount + 0.5f) != _Value)
					//{
					//	int LastValue = _Value;
					//	_Value = (int)(MoveAmount + 0.5f);

					//	DragOffset = MousePos.Y;
					//	this->RebuildMesh();

					//	ScrollEventArgs A(_Value, LastValue, VerticalScroll);
					//	this->ScrollEvent->Execute(this, &A);
					//}
				}else if(GripState != 0 || Dragging == true)
				{
					Dragging = false;
					GripState = 0;
					this->RebuildMesh();
				}




				Parameters->Handled = true;
				return true;
			}else
			{
				Dragging = false;
				bool Rebuild = false;

				if(UpState != 0)
				{
					UpState = 0;
					Rebuild = true;
				}

				if(DownState != 0)
				{
					DownState = 0;
					Rebuild = true;
				}

				if(GripState != 0)
				{
					GripState = 0;
					Rebuild = true;
				}

				if(Rebuild)
					this->RebuildMesh();
			}
		#pragma endregion
		}else if(ScrollType == HorizontalScroll)
		{
		#pragma region Horizontal
			ISkin* Skin = Manager->GetSkin(this->_Skin);
			GUICoord ScrollBar_Up = HScrollBar_Normal_Up;
			GUICoord ScrollBar_Down = HScrollBar_Normal_Down;

			if(UpState == 1)
				ScrollBar_Up = HScrollBar_Hover_Up;
			else if(UpState == 2)
				ScrollBar_Up = HScrollBar_Down_Up;
			
			if(DownState == 1)
				ScrollBar_Down = HScrollBar_Hover_Down;
			else if(DownState == 2)
				ScrollBar_Down = HScrollBar_Down_Down;


			float Height = Skin->GetScreenCoord(HScrollBar_Fill).Y;
			float Width = _Size.X;
			float UpWidth = Skin->GetScreenCoord(ScrollBar_Up).X;
			float DnWidth = Skin->GetScreenCoord(ScrollBar_Down).X;
			float CenterWidth = Width - UpWidth - DnWidth;
			

			//float ScrollMin = Skin->GetScreenCoord(ScrollBar_Top).Y + Skin->GetScreenCoord(ScrollBar_Bottom).Y + Skin->GetScreenCoord(ScrollBar_Grip).Y;
			
			float fMax, fMin, fLargeChange, fValue;
			fMax = (float)_Maximum;
			fMin = (float)_Minimum;
			fLargeChange = (float)_LargeChange;
			fValue = (float)_Value;

			float BarWidth = CenterWidth * (fLargeChange / (fMax - fMin));
			float ScrollArea = CenterWidth - BarWidth;
			float ScrollScale = (1.0f / ((fMax - fMin - fLargeChange) / (fValue - fMin)));
			if(ScrollScale > 1.0f)
				ScrollScale = 1.0f;
			float ScrollPos =  ScrollArea * ScrollScale;

			// Check we're over the gadget itself
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;

				if(Parameters->LeftDown)
					Manager->ControlFocus(this);

				Vector2 MousePos = Parameters->MousePosition - _GlobalLocation;
				bool Handled = false;

				if(MousePos < Skin->GetScreenCoord(ScrollBar_Up))
				{
					Handled = true;

					if(Parameters->LeftDown == true && UpState != 2)
					{
						UpState = 2;

						// Down!
						int LastValue = _Value;
						_Value -= _SmallChange;
						if(_Value < _Minimum)
							_Value = _Minimum;
						else
						{
							ScrollEventArgs A(_Value, LastValue, VerticalScroll);
							this->ScrollEvent->Execute(this, &A);
							this->RebuildMesh();
						}

						
					}else if(Parameters->LeftDown == false && UpState == 2)
					{
						UpState = 1;
						this->RebuildMesh();			
					}else if(UpState != 1)
					{
						UpState = 1;
						this->RebuildMesh();
					}
				}else if(UpState != 0)
				{
					UpState = 0;
					this->RebuildMesh();
				}

				if(MousePos.X > _Size.X - Skin->GetScreenCoord(ScrollBar_Down).X)
				{
					Handled = true;

					if(Parameters->LeftDown == true && DownState != 2)
					{
						// Down!
						DownState = 2;
						
						int LastValue = _Value;
						_Value += _SmallChange;
						if(_Value > (_Maximum - _LargeChange + 1))
							_Value = (_Maximum - _LargeChange + 1);
						else
						{
							ScrollEventArgs A(_Value, LastValue, VerticalScroll);
							this->ScrollEvent->Execute(this, &A);
							this->RebuildMesh();
						}
					}else if(Parameters->LeftDown == false && DownState == 2)
					{
						// Hit!
						DownState = 1;
						this->RebuildMesh();
					}else if(DownState != 1)
					{
						DownState = 1;
						this->RebuildMesh();
					}
				}else if(DownState != 0)
				{
					DownState = 0;
					this->RebuildMesh();
				}

				if(MousePos.X > ScrollPos + UpWidth && MousePos.X < ScrollPos + UpWidth + BarWidth)
				{
					if(Parameters->LeftDown == true)
					{
						if(GripState != 2)
						{
							GripState = 2;
							this->RebuildMesh();
						}

						if(Dragging == false)
						{
							Dragging = true;
							DragOffset = MousePos.X;
						}else
						{
							float MoveAmount = (MousePos.X - DragOffset) + ScrollPos;
							MoveAmount /= ScrollArea; // Find the ScrollScale
							MoveAmount = 1.0f / MoveAmount;
							MoveAmount = (fMax - fMin - fLargeChange) / MoveAmount;
							MoveAmount += fMin;

							if(MoveAmount > (fMax - fMin - fLargeChange) + 1.0f)
								MoveAmount = (fMax - fMin - fLargeChange) + 1.0f;
							if(MoveAmount < 0.0f)
								MoveAmount = 0.0f;

							if((int)(MoveAmount + 0.5f) != _Value)
							{
								int LastValue = _Value;
								_Value = (int)(MoveAmount + 0.5f);

								DragOffset = MousePos.X;
								this->RebuildMesh();

								ScrollEventArgs A(_Value, LastValue, HorizontalScroll);
								this->ScrollEvent->Execute(this, &A);
							}
						}

					}else if(Parameters->LeftDown == false && GripState == 2)
					{
						GripState = 1;
						Dragging = false;
						this->RebuildMesh();


					}else if(GripState != 1)
					{
						GripState = 1;
						this->RebuildMesh();
					}
				}else if(Dragging == true && Handled == false)
				{
					float MoveAmount = (MousePos.X - DragOffset) + ScrollPos;
					MoveAmount /= ScrollArea; // Find the ScrollScale
					MoveAmount = 1.0f / MoveAmount;
					MoveAmount = (fMax - fMin - fLargeChange) / MoveAmount;
					MoveAmount += fMin;

					if(MoveAmount > (fMax - fMin - fLargeChange))
						MoveAmount = (fMax - fMin - fLargeChange);
					if(MoveAmount < 0.0f)
						MoveAmount = 0.0f;

					if((int)(MoveAmount + 0.5f) != _Value)
					{
						int LastValue = _Value;
						_Value = (int)(MoveAmount + 0.5f);

						DragOffset = MousePos.X;
						this->RebuildMesh();

						ScrollEventArgs A(_Value, LastValue, VerticalScroll);
						this->ScrollEvent->Execute(this, &A);
					}
				}else if(GripState != 0 || Dragging == true)
				{
					Dragging = false;
					GripState = 0;
					this->RebuildMesh();
				}




				Parameters->Handled = true;
				return true;
			}else
			{
				Dragging = false;
				bool Rebuild = false;

				if(UpState != 0)
				{
					UpState = 0;
					Rebuild = true;
				}

				if(DownState != 0)
				{
					DownState = 0;
					Rebuild = true;
				}

				if(GripState != 0)
				{
					GripState = 0;
					Rebuild = true;
				}

				if(Rebuild)
					this->RebuildMesh();
			}
		#pragma endregion
		}

			return false;
		}

		void CScrollBar::Render()
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

			if(_Enabled)
				Renderer->DrawMeshBuffer(MeshBuffer);
			else
				Renderer->DrawMeshBuffer(MeshBuffer, 0, 6);

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();
		}

		bool CScrollBar::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			return true;
		}

		void CScrollBar::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			SGUIMeshBuilder Builder(Manager->GetRenderer());

		if(ScrollType == VerticalScroll)
		{
		#pragma region Vertical
			GUICoord ScrollBar_Up = VScrollBar_Normal_Up;
			GUICoord ScrollBar_Down = VScrollBar_Normal_Down;
			GUICoord ScrollBar_Top = VScrollBar_Normal_Top;
			GUICoord ScrollBar_Middle = VScrollBar_Normal_Middle;
			GUICoord ScrollBar_Bottom = VScrollBar_Normal_Bottom;
			GUICoord ScrollBar_Grip = VScrollBar_Normal_Grip;

			switch(UpState)
			{
			case 1:
				{
					ScrollBar_Up = VScrollBar_Hover_Up;
					break;
				}
			case 2:
				{
					ScrollBar_Up = VScrollBar_Down_Up;
					break;
				}
			}
			
			switch(DownState)
			{
			case 1:
				{
					ScrollBar_Down = VScrollBar_Hover_Down;
					break;
				}
			case 2:
				{
					ScrollBar_Down = VScrollBar_Down_Down;
					break;
				}
			}
			
			switch(GripState)
			{
			case 1:
				{
					ScrollBar_Top = VScrollBar_Hover_Top;
					ScrollBar_Middle = VScrollBar_Hover_Middle;
					ScrollBar_Bottom = VScrollBar_Hover_Bottom;
					ScrollBar_Grip = VScrollBar_Hover_Grip;
					break;
				}
			case 2:
				{
					ScrollBar_Top = VScrollBar_Down_Top;
					ScrollBar_Middle = VScrollBar_Down_Middle;
					ScrollBar_Bottom = VScrollBar_Down_Bottom;
					ScrollBar_Grip = VScrollBar_Down_Grip;
					break;
				}
			}

			float Width = Skin->GetScreenCoord(VScrollBar_Fill).X;
			float Height = _Size.Y;
			_Size.X = Width;
			
			float UpHeight = Skin->GetScreenCoord(ScrollBar_Up).Y;
			float DnHeight = Skin->GetScreenCoord(ScrollBar_Down).Y;
			float CenterHeight = Height - UpHeight - DnHeight;
			float ScrollMin = Skin->GetScreenCoord(ScrollBar_Top).Y + Skin->GetScreenCoord(ScrollBar_Bottom).Y + Skin->GetScreenCoord(ScrollBar_Grip).Y;
			
			float fMax, fMin, fLargeChange, fValue;
			fMax = (float)_Maximum;
			fMin = (float)_Minimum;
			fLargeChange = (float)_LargeChange;
			fValue = (float)_Value;

			float BarHeight = CenterHeight * (fLargeChange / (fMax - fMin));
			float ScrollArea = CenterHeight - BarHeight;
			float ScrollScale = (1.0f / ((fMax - fMin - fLargeChange) / (fValue - fMin)));
			if(ScrollScale > 1.0f)
				ScrollScale = 1.0f;
			float ScrollPos =  ScrollArea * ScrollScale;

			float GripSize = BarHeight - Skin->GetScreenCoord(ScrollBar_Top).Y - Skin->GetScreenCoord(ScrollBar_Bottom).Y;
			if(GripSize < 0.0f)
				GripSize = 0.0f;

			float GripImageHeight = Skin->GetScreenCoord(ScrollBar_Grip).Y;


			Vector2 DnPos(0, Height - DnHeight);

			Builder.AddQuad(Vector2(0, 0), Vector2(Width, Height), Skin->GetCoord(VScrollBar_Fill), Skin->GetSize(VScrollBar_Fill), _BackColor);
			Builder.AddQuad(Vector2(0, 0), Skin->GetScreenCoord(ScrollBar_Up), Skin->GetCoord(ScrollBar_Up), Skin->GetSize(ScrollBar_Up), _BackColor);
			Builder.AddQuad(DnPos, Skin->GetScreenCoord(ScrollBar_Down), Skin->GetCoord(ScrollBar_Down), Skin->GetSize(ScrollBar_Down), _BackColor);
			Builder.AddQuad(Vector2(0, ScrollPos + UpHeight), Skin->GetScreenCoord(ScrollBar_Top), Skin->GetCoord(ScrollBar_Top), Skin->GetSize(ScrollBar_Top), _BackColor);
			Builder.AddQuad(Vector2(0, ScrollPos + UpHeight + Skin->GetScreenCoord(ScrollBar_Top).Y), Vector2(Skin->GetScreenCoord(ScrollBar_Middle).X, GripSize), Skin->GetCoord(ScrollBar_Middle), Skin->GetSize(ScrollBar_Middle), _BackColor);
			Builder.AddQuad(Vector2(0, ScrollPos + UpHeight + Skin->GetScreenCoord(ScrollBar_Top).Y + GripSize), Skin->GetScreenCoord(ScrollBar_Bottom), Skin->GetCoord(ScrollBar_Bottom), Skin->GetSize(ScrollBar_Bottom), _BackColor);
			Builder.AddQuad(Vector2(0, ScrollPos + UpHeight + Skin->GetScreenCoord(ScrollBar_Top).Y + ((GripSize / 2.0f) - (Skin->GetScreenCoord(ScrollBar_Grip).Y / 2.0f))), GripImageHeight > GripSize ? Vector2(0, 0) : Skin->GetScreenCoord(ScrollBar_Grip), Skin->GetCoord(ScrollBar_Grip), Skin->GetSize(ScrollBar_Grip), _BackColor);

		#pragma endregion
		}else if(ScrollType == HorizontalScroll)
		{
		#pragma region Horizontal
			GUICoord ScrollBar_Up = HScrollBar_Normal_Up;
			GUICoord ScrollBar_Down = HScrollBar_Normal_Down;
			GUICoord ScrollBar_Top = HScrollBar_Normal_Top;
			GUICoord ScrollBar_Middle = HScrollBar_Normal_Middle;
			GUICoord ScrollBar_Bottom = HScrollBar_Normal_Bottom;
			GUICoord ScrollBar_Grip = HScrollBar_Normal_Grip;

			switch(UpState)
			{
			case 1:
				{
					ScrollBar_Up = HScrollBar_Hover_Up;
					break;
				}
			case 2:
				{
					ScrollBar_Up = HScrollBar_Down_Up;
					break;
				}
			}
			
			switch(DownState)
			{
			case 1:
				{
					ScrollBar_Down = HScrollBar_Hover_Down;
					break;
				}
			case 2:
				{
					ScrollBar_Down = HScrollBar_Down_Down;
					break;
				}
			}
			
			switch(GripState)
			{
			case 1:
				{
					ScrollBar_Top = HScrollBar_Hover_Top;
					ScrollBar_Middle = HScrollBar_Hover_Middle;
					ScrollBar_Bottom = HScrollBar_Hover_Bottom;
					ScrollBar_Grip = HScrollBar_Hover_Grip;
					break;
				}
			case 2:
				{
					ScrollBar_Top = HScrollBar_Down_Top;
					ScrollBar_Middle = HScrollBar_Down_Middle;
					ScrollBar_Bottom = HScrollBar_Down_Bottom;
					ScrollBar_Grip = HScrollBar_Down_Grip;
					break;
				}
			}

			float Height = Skin->GetScreenCoord(HScrollBar_Fill).Y;
			float Width = _Size.X;
			_Size.Y = Height;
			
			float UpWidth = Skin->GetScreenCoord(ScrollBar_Up).X;
			float DnWidth = Skin->GetScreenCoord(ScrollBar_Down).X;
			float CenterWidth = Width - UpWidth - DnWidth;
			float ScrollMin = Skin->GetScreenCoord(ScrollBar_Top).X + Skin->GetScreenCoord(ScrollBar_Bottom).X + Skin->GetScreenCoord(ScrollBar_Grip).X;
			
			float fMax, fMin, fLargeChange, fValue;
			fMax = (float)_Maximum;
			fMin = (float)_Minimum;
			fLargeChange = (float)_LargeChange;
			fValue = (float)_Value;

			float BarWidth = CenterWidth * (fLargeChange / (fMax - fMin));
			float ScrollArea = CenterWidth - BarWidth;
			float ScrollScale = (1.0f / ((fMax - fMin - fLargeChange) / (fValue - fMin)));
			if(ScrollScale > 1.0f)
				ScrollScale = 1.0f;
			float ScrollPos =  ScrollArea * ScrollScale;

			float GripSize = BarWidth - Skin->GetScreenCoord(ScrollBar_Top).X - Skin->GetScreenCoord(ScrollBar_Bottom).X;
			if(GripSize < 0.0f)
				GripSize = 0.0f;

			float GripImageWidth = Skin->GetScreenCoord(ScrollBar_Grip).X;


			Vector2 DnPos(Width - DnWidth, 0);

			Builder.AddQuad(Vector2(0, 0), Vector2(Width, Height), Skin->GetCoord(HScrollBar_Fill), Skin->GetSize(HScrollBar_Fill), _BackColor);
			Builder.AddQuad(Vector2(0, 0), Skin->GetScreenCoord(ScrollBar_Up), Skin->GetCoord(ScrollBar_Up), Skin->GetSize(ScrollBar_Up), _BackColor);
			Builder.AddQuad(DnPos, Skin->GetScreenCoord(ScrollBar_Down), Skin->GetCoord(ScrollBar_Down), Skin->GetSize(ScrollBar_Down), _BackColor);
			Builder.AddQuad(Vector2(ScrollPos + UpWidth, 0), Skin->GetScreenCoord(ScrollBar_Top), Skin->GetCoord(ScrollBar_Top), Skin->GetSize(ScrollBar_Top), _BackColor);
			Builder.AddQuad(Vector2(ScrollPos + UpWidth + Skin->GetScreenCoord(ScrollBar_Top).X, 0), Vector2(GripSize, Skin->GetScreenCoord(ScrollBar_Middle).Y), Skin->GetCoord(ScrollBar_Middle), Skin->GetSize(ScrollBar_Middle), _BackColor);
			Builder.AddQuad(Vector2(ScrollPos + UpWidth + Skin->GetScreenCoord(ScrollBar_Top).X + GripSize, 0), Skin->GetScreenCoord(ScrollBar_Bottom), Skin->GetCoord(ScrollBar_Bottom), Skin->GetSize(ScrollBar_Bottom), _BackColor);
			Builder.AddQuad(
				Vector2(ScrollPos + UpWidth + Skin->GetScreenCoord(ScrollBar_Top).X + ((GripSize / 2.0f) - (Skin->GetScreenCoord(ScrollBar_Grip).X / 2.0f)), 0),
				GripImageWidth > GripSize ? Vector2(0, 0) : Skin->GetScreenCoord(ScrollBar_Grip), Skin->GetCoord(ScrollBar_Grip), Skin->GetSize(ScrollBar_Grip), _BackColor);

		#pragma endregion
		}

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		void CScrollBar::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CScrollBar::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
		}

		int CScrollBar::zzGet_LargeChange()
		{
			return _LargeChange;
		}

		int CScrollBar::zzGet_SmallChange()
		{
			return _SmallChange;
		}

		int CScrollBar::zzGet_Minimum()
		{
			return _Minimum;
		}

		int CScrollBar::zzGet_Maximum()
		{
			return _Maximum;
		}

		int CScrollBar::zzGet_Value()
		{
			return _Value;
		}

		void CScrollBar::OnValueChange()
		{
			// Check value does not exceed bounds
			if(_Value > _Maximum)
				_Value = _Maximum;
			if(_Value < _Minimum)
				_Value = _Minimum;
			this->RebuildMesh();
		}
		void CScrollBar::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		void CScrollBar::zzPut_LargeChange(int LargeChange)
		{
			_LargeChange = LargeChange;
			OnValueChange();
		}

		void CScrollBar::zzPut_SmallChange(int SmallChange)
		{
			_SmallChange = SmallChange;
			OnValueChange();
		}

		void CScrollBar::zzPut_Minimum(int Minimum)
		{
			_Minimum = Minimum;
			OnValueChange();
		}

		void CScrollBar::zzPut_Maximum(int Maximum)
		{
			_Maximum = Maximum;
			OnValueChange();
		}

		void CScrollBar::zzPut_Value(int Value)
		{
			_Value = Value;
			OnValueChange();
		}

		ScrollEventHandler* CScrollBar::Scroll()
		{
			return ScrollEvent;
		}


	}
}
