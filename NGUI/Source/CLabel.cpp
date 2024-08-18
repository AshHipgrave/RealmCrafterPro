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
#include <windows.h>
#include "CLabel.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type ILabel::TypeOf()
		{
			return Type("CLBL", "CLabel GUI Label");
		}

		CLabel::CLabel(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : ILabel(ScreenScale, Manager)
		{
			_Type = Type("CLBL", "CLabel GUI Label");
			MeshBuffer = 0;
			Manager = 0;
			_Font = 0;
			_Align = TextAlign_Left;
			_VAlign = TextAlign_Top;
			_Multiline = false;
			_InlineStringProcessing = true;
			_ForceScissoring = false;
			_InternalHeight = 0;
			_InternalWidth = 0;

			ButtonState = 0;
			UsingRight = _MouseOver = false;
			ClickEvent = new EventHandler();
			DownEvent = new EventHandler();
			RightClickEvent = new EventHandler();
			MouseEnterEvent = new EventHandler();
			MouseLeaveEvent = new EventHandler();
			MouseMoveEvent = new MouseEventHandler();
		}

		CLabel::~CLabel()
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

		void CLabel::OnDeviceLost()
		{
			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = 0;

			IControl::OnDeviceLost();
		}

		void CLabel::OnDeviceReset()
		{
			this->RebuildMesh();

			IControl::OnDeviceReset();
		}

		bool CLabel::Update(GUIUpdateParameters* Parameters)
		{
			if(!_Enabled)
				return true;

			

			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + Vector2(_InternalWidth, _InternalHeight)) && Parameters->Handled == false)
			{
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;
			}

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
		}

		void CLabel::Render()
		{
			if(!_Visible)
				return;
			if(_Skin == 0)
				return;
			if(_Font == 0)
				return;
			if(MeshBuffer == 0)
				return;



			// Set us up for rendering
			Manager->SetSkin(-1);
			_Font->SetTexture();
			Manager->SetPosition(_GlobalLocation + _ScrollOffset);

			IGUIRenderer* Renderer = Manager->GetRenderer();

			Math::Vector4 ScissorRect;
			Math::Vector4 OldRect;

			if(_ForceScissoring)
			{
				ScissorRect.X = (int)(_GlobalLocation.X * Manager->GetResolution().X);
				ScissorRect.Y = (int)(_GlobalLocation.Y * Manager->GetResolution().Y);
				ScissorRect.Z = ScissorRect.X + (int)(_ScissorWindow.X * Manager->GetResolution().X);
				ScissorRect.W = ScissorRect.Y + (int)(_ScissorWindow.Y * Manager->GetResolution().Y);

				Manager->CorrectRect(ScissorRect);

				OldRect = Renderer->GetScissorRect();
				Renderer->PushScissorRect(ScissorRect);
			}

			MeshBuffer->Set();
			Renderer->DrawMeshBuffer(MeshBuffer);

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			if(_ForceScissoring)
				Renderer->PopScissorRect();
		}

		bool CLabel::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			return true;
		}

		void CLabel::RebuildMesh()
		{
			if(_Locked)
				return;
			if(!_Font)
				return;
			if(!_Skin)
				return;

			_InternalWidth = 0;

			std::string Cs = this->_Text;
			const unsigned char* Str = (const unsigned char*)Cs.c_str();
			int StrLen = this->_Text.length();

			// Locals used for windows building
			Vector2 Min, Max, Pos, Sca;
			int VCnt = 0, ICnt = 0;
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			// Build the GUI
		#pragma region GUI Building


			//MessageBoxW(0, (LPCWSTR)WString((int)Str[0]).w_str(), 0,0);

			Vector2 LabelSize(0, 0);
			for(int f = 0; f < StrLen; ++f)
			{
				Sca = _Font->GetScreenCoord(Str[f]);	
				LabelSize.X += Sca.X;

				if(Sca.Y > LabelSize.Y)
					LabelSize.Y = Sca.Y;
			}


			Pos = Vector2(0, 0); Sca = Vector2(0, 0);

			switch(_Align)
			{
			case TextAlign_Center:
				Pos.X = (_Size.X / 2.0f) - (LabelSize.X / 2.0f);
				break;
			case TextAlign_Right:
				Pos.X = _Size.X - LabelSize.X;
				break;
			}

			switch(_VAlign)
			{
			case TextAlign_Middle:
				Pos.Y = (_Size.Y / 2.0f) - (LabelSize.Y / 2.0f);
				break;
			case TextAlign_Bottom:
				Pos.Y = _Size.Y - LabelSize.Y;
				break;
			}

			if(_Align == TextAlign_Left && _Multiline == true)
				_Size.Y = 0.0f;


			Min = Skin->GetCoord(Label_Fill);
			Max = Skin->GetSize(Label_Fill);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			// Out of range strlen stops a word or an entire string beign shifted if the box is too small for one line
			int LastSpace = StrLen + 1;
			bool DropLine = false;
			int fIncrement = 0;
			float CharScale = 1.0f;
			Math::Color CurrentColor = this->_ForeColor;

			float LineHeight = 0.0f;

			for(int f = 0; f < StrLen; ++f)
			{
				// An incremental value is used to skip characters
				CharScale = (fIncrement > 0 ? 0.0f : 1.0f);

				// Lower the increment
				if(fIncrement > 0)
					--fIncrement;

				// If this character is a space, store it for wrapping
				if(Str[f] == ' ')
					LastSpace = f;

				// An escape character has been found
				if(_InlineStringProcessing && Str[f] == '\\')
				{
					// New line
					if((StrLen - f) > 1 && Str[f + 1] == 'n')
					{
						DropLine = true;
						fIncrement = 1;
						CharScale = 0.0f;
					}

					// Color change
					if((StrLen - f) > 3 && Str[f + 1] == 'c' && Str[f + 2] == '=')
					{
						// HTML Color code
						if(Str[f + 3] == '#')
						{
							// Ensure the string is long enough
							if(StrLen - f > 3 + 6)
							{
								char ColorStr[7] = {'0', '0', '0', '0', '0', '0', 0};

								for(int fi = 0; fi < 6; ++fi)
									ColorStr[fi] = Str[f + 4 + fi];

								CurrentColor = Color::FromString(std::string(ColorStr));

								fIncrement = 9;
								CharScale = 0.0f;
							}
						}else if(Str[f + 3] == '\'') // Word Color
						{

						}
					}else if((StrLen - f) > 1 && Str[f + 1] == 'c') // Color reset
					{
						CurrentColor = this->_ForeColor;

						fIncrement = 1;
						CharScale = 0.0f;
					}
				}

				// An 'actual' linebreak
				if(_InlineStringProcessing && Str[f] == '\n')
				{
					DropLine = true;
					fIncrement = 0;
					CharScale = 0.0f;
				}

				// Setup character dimensions
				Sca = _Font->GetScreenCoord(Str[f]) * CharScale;	
				Min = _Font->GetCoord(Str[f]);
				Max = _Font->GetSize(Str[f]);

				// Build character
				VCnt = (f * 4) + 4;
				ICnt = (f * 6) + 6;
				Builder.AddQuad(Pos, Sca, Min, Max, CurrentColor);

				// Calculate positions for next character
				if(Sca.Y + Pos.Y > _Size.Y)
				{
					_Size.Y = Sca.Y + Pos.Y;
					LineHeight = Sca.Y;
				}
				Pos.X += Sca.X;

				if(Pos.X > _InternalWidth)
					_InternalWidth = Pos.X;

				// We need to move down to another line
				if(_Multiline && DropLine)
				{
					DropLine = false;
					Pos.Y += LineHeight;
					Pos.X = 0;
					LastSpace = StrLen + 1;
					LineHeight = 0;
				}else if(_Multiline && LastSpace < StrLen && Pos.X > _Size.X) // Same as above, but wrapped around space.
				{
					Builder.RemoveQuads(f - LastSpace);
					Pos.Y += LineHeight;
					Pos.X = 0;
					f = LastSpace;
					LastSpace = StrLen + 1;
					LineHeight = 0;
				}
				
				// Reset scale
				CharScale = 1.0f;
			}

			//_Size.Y = Pos.Y + LineHeight;
			_InternalHeight = Pos.Y + LineHeight;

		#pragma endregion

			// Create buffer
			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		IFont* CLabel::zzGet_Font()
		{
			return _Font;
		}

		void CLabel::zzPut_Font(IFont* Font)
		{
			_Font = Font;
			this->RebuildMesh();
		}

		void CLabel::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CLabel::OnTextChange()
		{
			IControl::OnTextChange();
			this->RebuildMesh();
		}

		void CLabel::OnForeColorChange()
		{
			IControl::OnForeColorChange();
			this->RebuildMesh();
		}

		void CLabel::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
		}

		TextAlign CLabel::zzGet_Align()
		{
			return _Align;
		}

		TextAlign CLabel::zzGet_VAlign()
		{
			return _VAlign;
		}

		void CLabel::zzPut_Align(TextAlign Alignment)
		{
			_Align = Alignment;
			this->RebuildMesh();
		}

		void CLabel::zzPut_VAlign(TextAlign Alignment)
		{
			_VAlign = Alignment;
			this->RebuildMesh();
		}

		bool CLabel::zzGet_Multiline()
		{
			return _Multiline;
		}

		bool CLabel::zzGet_InlineStringProcessing()
		{
			return _InlineStringProcessing;
		}

		void CLabel::zzPut_Multiline(bool Multiline)
		{
			_Multiline = Multiline;
			this->RebuildMesh();
		}

		void CLabel::zzPut_InlineStringProcessing(bool InlineStringProcessing)
		{
			_InlineStringProcessing = InlineStringProcessing;
			this->RebuildMesh();
		}

		void CLabel::zzPut_ScissorWindow(NGin::Math::Vector2 ScissorWindow)
		{
			_ScissorWindow = ScissorWindow;
		}

		NGin::Math::Vector2 CLabel::zzGet_ScissorWindow()
		{
			return _ScissorWindow;
		}

		void CLabel::zzPut_ForceScissoring(bool ForceScissoring)
		{
			_ForceScissoring = ForceScissoring;
		}

		bool CLabel::zzGet_ForceScissoring()
		{
			return _ForceScissoring;
		}

		void CLabel::zzPut_ScrollOffset(NGin::Math::Vector2 &Offset)
		{
			_ScrollOffset = Offset;
		}

		NGin::Math::Vector2 CLabel::zzGet_ScrollOffset()
		{
			return _ScrollOffset;
		}


		EventHandler* CLabel::Click()
		{
			return ClickEvent;
		}

		EventHandler* CLabel::RightClick()
		{
			return RightClickEvent;
		}

		EventHandler* CLabel::MouseDown()
		{
			return DownEvent;
		}

		EventHandler* CLabel::MouseEnter()
		{
			return MouseEnterEvent;
		}

		EventHandler* CLabel::MouseLeave()
		{
			return MouseLeaveEvent;
		}

		MouseEventHandler* CLabel::MouseMove()
		{
			return MouseMoveEvent;
		}

		float CLabel::zzGet_InternalHeight()
		{
			return _InternalHeight;
		}

		float CLabel::zzGet_InternalWidth()
		{
			return _InternalWidth;
		}

	}
}
