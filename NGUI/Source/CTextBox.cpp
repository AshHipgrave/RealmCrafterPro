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
#include "CTextBox.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type ITextBox::TypeOf()
		{
			return Type("CTXT", "CTextBox GUI TextBox");
		}

		CTextBox::CTextBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : ITextBox(ScreenScale, Manager)
		{
			_Type = Type("CTXT", "CTextBox GUI TextBox");
			MeshBuffer = 0;
			LabelBuffer = 0;
			Manager = 0;
			CaretPosition = 0;
			CaretOffset = 0;
			_Font = 0;
			KnownFocus = false;
			StrLen = 0;
			MouseState = false;
			_PassChar = "";
			Regex = 0;
		}

		CTextBox::~CTextBox()
		{
			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			if(LabelBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(LabelBuffer);
		}

		void CTextBox::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CTextBox::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CTextBox::Update(GUIUpdateParameters* Parameters)
		{
			if(_Visible)
			{
				if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
				{
					Parameters->MouseBusy = true;
					if(Parameters->MouseThumb == 0)
						Parameters->MouseThumb = this;
					//Manager->SetCursor(Manager->GetSkin(_Skin)->GetCursor(GUICursor_TextInput));

					if(Parameters->LeftDown)
					{
						Manager->ControlFocus(this);

						if(MouseState == false)
						{
							ISkin* Skin = Manager->GetSkin(_Skin);
							float LW = Skin->GetScreenCoord(TextBox_Border_TL).X;
							float RW = Skin->GetScreenCoord(TextBox_Border_TR).X;
							float MW = _Size.X - LW - RW;

							float RX = LW + MW;
							RX -= 0.001f;
							LW += 0.001f;

							// Go through all characters to put the cursor in the appropriate place
							for(int i = 0; i < Visibles.Size(); ++i)
							{
								float PrevHalf = 0;

								if(i > 0)
									PrevHalf = Visibles[i - 1].Width / 2;

								float MinX = Visibles[i].XPos - PrevHalf;
								float MaxX = Visibles[i].XPos + (Visibles[i].Width / 2);

								if(MaxX > RX)
									MaxX = RX;

								if(MinX < LW)
									MinX = LW;

								if(Parameters->MousePosition.X > _GlobalLocation.X + MinX && Parameters->MousePosition.X < _GlobalLocation.X + MaxX)
								{
									CaretPosition = Visibles[i].Char;
									//CaretOffset = (Parameters->MousePosition.X - LW) - _GlobalLocation.X;//Visibles[i].XPos;
									CaretOffset = Visibles[i].XPos;
									this->RebuildMesh();

									break;
								}
							}

							if(Visibles.Size() > 0)
							{
								if(Parameters->MousePosition.X > _GlobalLocation.X + Visibles[Visibles.Size() - 1].XPos)
								{
									CaretPosition = Visibles[Visibles.Size() - 1].Char + 1;
									//CaretOffset = Visibles[Visibles.Size() - 1].XPos + Visibles[Visibles.Size() - 1].Width;
									CaretOffset = Visibles[Visibles.Size() - 1].XPos + Visibles[Visibles.Size() - 1].Width;

									this->RebuildMesh();
								}
							}

							MouseState = true;
						}
					}else
					{
						MouseState = false;
					}
				}

				if(Manager->ControlFocus() == this)
				{
					if(KnownFocus == false)
					{
						KnownFocus = true;
						this->RebuildMesh();
					}

					// Control Keys
					if(Parameters->KeyRight && CaretPosition < _Text.length())
					{
						std::string Rem = _Text.substr(CaretPosition, 1);

						++CaretPosition;

						if(_PassChar.length() > 0)
							Rem = _PassChar;
						CaretOffset += _Font->GetScreenCoord(Rem.c_str()[0]).X;

						this->RebuildMesh();
					}

					if(Parameters->KeyLeft && CaretPosition > 0)
					{
						std::string Rem = _Text.substr(CaretPosition - 1, 1);

						--CaretPosition;

						if(_PassChar.length() > 0)
							Rem = _PassChar;
						CaretOffset -= _Font->GetScreenCoord(Rem.c_str()[0]).X;

						this->RebuildMesh();
					}

					if(Parameters->KeyEnd)
					{
						CaretPosition = _Text.length();
						CaretOffset = 1.1f; // Too far

						this->RebuildMesh();
					}

					if(Parameters->KeyHome)
					{
						CaretPosition = 0;
						CaretOffset = 0;

						this->RebuildMesh();
					}

					if(Parameters->KeyDelete && _Text.length() > 0 && CaretPosition < _Text.length())
					{
						std::string Rem = _Text.substr(CaretPosition, 1);

						_Text = _Text.substr(0, CaretPosition) + _Text.substr(CaretPosition + 1);

						this->RebuildMesh();
					}

					for(int i = 0; i < Parameters->InputBuffer->size(); ++i)
					{
						int K = Parameters->InputBuffer[0][i];
						
						char L[2] = {0, 0};

						if(K > 31 && K < 255 && K != 127)
						{
							L[0] = (char)K;

							if(Regex != 0)
								if(!Regex->matches(std::string(L)))
									continue;
							
							_Text = _Text.substr(0, CaretPosition) + std::string(L) + _Text.substr(CaretPosition);
							
							++CaretPosition;

							int Kr = K;
							if(_PassChar.length() > 0)
								K = _PassChar.c_str()[0];

							CaretOffset += _Font->GetScreenCoord(K).X;

							this->RebuildMesh();
							

						}else
						{
							switch(K)
							{
							case 8: // BackSpace
								{
									//WString LT = Lbl->Text();
									//if(LT.Length() > 0)
									//{
									//	LT = LT.Substr(0, LT.Length() - 1);
									//	Lbl->Text(LT);
									//}

									if(CaretPosition > 0)
									{
										std::string Rem = _Text.substr(CaretPosition - 1, 1);

										_Text = _Text.substr(0, CaretPosition - 1) + _Text.substr(CaretPosition);

										--CaretPosition;

										if(_PassChar.length() > 0)
											Rem = _PassChar;

										CaretOffset -= _Font->GetScreenCoord(Rem.c_str()[0]).X;

										this->RebuildMesh();
									}
							


									break;
								}
							case 13: // Return

								break;
							case 9: // Tab

								break;
							default:
								{

								}
								break;
							}
						}

					}
				}else if(KnownFocus == true)
				{
					// Remove caret
					KnownFocus = false;
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

			return false;
		}

		void CTextBox::Render()
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

			if(_Font != 0 && LabelBuffer != 0 && _Text.length() > 0)
			{
				ISkin* Skin = Manager->GetSkin(_Skin);
				_Font->SetTexture();
				
				float LW = Skin->GetScreenCoord(TextBox_Border_TL).X;
				float MX = LW;

				float TH = Skin->GetScreenCoord(TextBox_Border_TL).Y;
				float MY = TH;

				WinSize.X += MX * Manager->GetResolution().X;
				WinSize.Z -= Skin->GetScreenCoord(TextBox_Border_TR).X * Manager->GetResolution().X;
				Renderer->PushScissorRect(WinSize);

				float PY1 = (1.0f / Manager->GetResolution().Y);
				Manager->SetPosition(_GlobalLocation + Vector2(MX, MY + PY1));

				LabelBuffer->Set();
				Renderer->DrawMeshBuffer(LabelBuffer);

				Renderer->PopScissorRect();
			}

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();
		}

		bool CTextBox::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			return true;
		}

		void CTextBox::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			Vector2 Min, Max, Pos, Sca;
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Store all of the position/scale information of each quad
			float LW = Skin->GetScreenCoord(TextBox_Border_TL).X;
			float RW = Skin->GetScreenCoord(TextBox_Border_TR).X;
			float MW = _Size.X - LW - RW;

			float LX = 0;
			float MX = LW;
			float RX = LW + MW;

			float TH = Skin->GetScreenCoord(TextBox_Border_TL).Y;
			float BH = Skin->GetScreenCoord(TextBox_Border_BL).Y;
			float MH = _Size.Y - TH - BH;

			float TY = 0;
			float MY = TH;
			float BY = TH + MH;

			bool DoneCaret = false;
			float CaretWidth = 0.0f;//Skin->GetScreenCoord(TextBox_Caret).X * (KnownFocus ? 1.0f : 0.0f);

			// Now for the text
			std::string Cs = this->_Text;
			const unsigned char* Str = (const unsigned char*)Cs.c_str();
			StrLen = _Text.length();

			std::string Padded = "";

			if(_PassChar.length() > 0)
			{
				for(int i = 0; i < StrLen; ++i)
					Padded.append(_PassChar.substr(0, 1));

				Str = (const unsigned char*)Padded.c_str();
			}

			Entries.Empty();
			Entries.SetUsed(StrLen);


			for(int i = 0; i < StrLen; ++i)
			{
				Vector2 Sca = _Font->GetScreenCoord(Str[i]);// * CharScale;	
				
				Entries[i].Value = Str[i];
				Entries[i].XPos = Pos.X;
				Entries[i].YPos = Pos.Y;
				Entries[i].Width = Sca.X;
				Entries[i].Height = Sca.Y;
			}

			if(CaretPosition > Entries.Size())
				CaretPosition = Entries.Size();
			
			if(CaretOffset > MW - CaretWidth )
				CaretOffset = MW - CaretWidth;

			if(CaretOffset < 0)
				CaretOffset = 0;

			int LeftSize = 0;
			int StartPos = 0;
			float LeftOffset = CaretOffset;


			
			for(int i = CaretPosition - 1; i >= 0; --i)
			{
				StartPos = i;
				++LeftSize;
				LeftOffset -= Entries[i].Width;
				
				if(LeftOffset < 0)
					break;
			}

			int RightSize = 0;
			int EndPos = CaretPosition;
			float RightOffset = CaretOffset;

			for(int i = CaretPosition; i < Entries.Size(); ++i)
			{
				EndPos = i;
				++RightSize;
				RightOffset += Entries[i].Width;

				if(RightOffset > _Size.X)
					break;
			}
			++EndPos;
			if(EndPos > _Text.length())
				EndPos = _Text.length();

				// Build the GUI
		#pragma region GUI Building


			Pos = Vector2(0, 0); Sca = Vector2(0, 0);

			// Out of range strlen stops a word or an entire string beign shifted if the box is too small for one line
			//int LastSpace = StrLen + 1;
			//bool DropLine = false;
			//int fIncrement = 0;
			//float CharScale = 1.0f;
			Math::Color CurrentColor = this->_ForeColor;

			StrLen = (EndPos - StartPos); //(RightSize - CaretPosition) + (CaretPosition - LeftSize);

			if(StrLen > 0)
			{
				SGUIMeshBuilder LabelBuilder(Manager->GetRenderer());

				Pos = Vector2(0, 0); Sca = Vector2(0, 0);
				
				//Pos.X = CaretOffset - LeftOffset;// - CaretWidth;
				Pos.X = LeftOffset;// - (CaretWidth / 2);
				Visibles.Empty();

				for(int f = StartPos; f < (StartPos + StrLen); ++f)
				{
					// An incremental value is used to skip characters
					/*CharScale = (fIncrement > 0 ? 0.0f : 1.0f);

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
					}*/

					if(f >= CaretPosition && DoneCaret == false)
					{
					//	CaretOffset = Pos.X;
						Pos.X += CaretWidth;
						CaretOffset = Pos.X;
						DoneCaret = true;
					}

					// Setup character dimensions
					Sca = _Font->GetScreenCoord(Str[f]);// * CharScale;	
					Min = _Font->GetCoord(Str[f]);
					Max = _Font->GetSize(Str[f]);

					CharacterEntry C;
					C.Width = Sca.X;
					C.XPos = Pos.X;
					C.Char = f;
					Visibles.Add(C);

					// Build character
					//VCnt = (f * 4);
					//ICnt = (f * 6);
					LabelBuilder.AddQuad(Pos, Sca, Min, Max, CurrentColor);

					// Calculate positions for next character
					if(Sca.Y + Pos.Y > _Size.Y)
						_Size.Y = Sca.Y + Pos.Y;
					Pos.X += Sca.X;



					// We need to move down to another line
					/*if(_Multiline && DropLine)
					{
						DropLine = false;
						Pos.Y += _Size.Y;
						Pos.X = 0;
						LastSpace = StrLen + 1;
					}else if(_Multiline && LastSpace < StrLen && Pos.X > _Size.X) // Same as above, but wrapped around space.
					{
						Pos.Y += _Size.Y;
						Pos.X = 0;
						f = LastSpace;
						LastSpace = StrLen + 1;
					}
					
					// Reset scale
					CharScale = 1.0f;*/
				}

			#pragma endregion

				if(LabelBuffer != NULL)
					Manager->GetRenderer()->FreeMeshBuffer(LabelBuffer);
				LabelBuffer = LabelBuilder.Build();

			}else
			{
				CaretOffset = 0;
			}

		#pragma region Main

			Pos = Vector2(0, 0);
			Sca = Vector2(0, 0);


			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Pos = Vector2(LX, TY); Sca = Vector2(LW, TH);
			Min = Skin->GetCoord(TextBox_Border_TL);
			Max = Skin->GetSize(TextBox_Border_TL);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(MX, TY); Sca = Vector2(MW, TH);
			Min = Skin->GetCoord(TextBox_Border_T);
			Max = Skin->GetSize(TextBox_Border_T);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(RX, TY); Sca = Vector2(RW, TH);
			Min = Skin->GetCoord(TextBox_Border_TR);
			Max = Skin->GetSize(TextBox_Border_TR);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(LX, MY); Sca = Vector2(LW, MH);
			Min = Skin->GetCoord(TextBox_Border_L);
			Max = Skin->GetSize(TextBox_Border_L);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(RX, MY); Sca = Vector2(RW, MH);
			Min = Skin->GetCoord(TextBox_Border_R);
			Max = Skin->GetSize(TextBox_Border_R);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(LX, BY); Sca = Vector2(LW, BH);
			Min = Skin->GetCoord(TextBox_Border_BL);
			Max = Skin->GetSize(TextBox_Border_BL);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(MX, BY); Sca = Vector2(MW, BH);
			Min = Skin->GetCoord(TextBox_Border_B);
			Max = Skin->GetSize(TextBox_Border_B);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(RX, BY); Sca = Vector2(RW, BH);
			Min = Skin->GetCoord(TextBox_Border_BR);
			Max = Skin->GetSize(TextBox_Border_BR);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(MX, MY); Sca = Vector2(MW, MH);
			Min = Skin->GetCoord(TextBox_Fill);
			Max = Skin->GetSize(TextBox_Fill);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			Pos = Vector2(MX + CaretOffset, MY); Sca = Skin->GetScreenCoord(TextBox_Caret)  * (KnownFocus ? 1.0f : 0.0f);
			Min = Skin->GetCoord(TextBox_Caret);
			Max = Skin->GetSize(TextBox_Caret);
			Builder.AddQuad(Pos, Sca, Min, Max, _BackColor);

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		#pragma endregion

		}

		void CTextBox::OnSizeChange()
		{
			IControl::OnSizeChange();

			this->RebuildMesh();
		}

		void CTextBox::OnTransform()
		{
			IControl::OnTransform();
		}

		void CTextBox::OnTextChange()
		{
			IControl::OnTextChange();

			CaretPosition = 0;
			CaretOffset = 0;
			this->RebuildMesh();
		}

		void CTextBox::OnEnabledChange()
		{
			IControl::OnEnabledChange();
			RebuildMesh();
		}

		void CTextBox::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		void CTextBox::OnForeColorChange()
		{
			IControl::OnForeColorChange();
		}

		void CTextBox::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->_Font = Manager->GetSkin(_Skin)->GetFont(GUIFont_Default);
			this->RebuildMesh();
		}

		void CTextBox::zzPut_PasswordCharacter(std::string PasswordCharacter)
		{
			if(PasswordCharacter.length() < 1)
				return;

			_PassChar = PasswordCharacter.substr(0, 1);
			this->RebuildMesh();
		}

		std::string CTextBox::zzGet_PasswordCharacter()
		{
			return _PassChar;
		}

		void CTextBox::zzPut_ValidationExpression(std::string Expression)
		{
			_ValidationExpression = Expression;

			if(Regex != NULL)
				delete Regex;
			Regex = NULL;

			if(Expression.length() > 0)
				Regex = Pattern::compile(std::string((const char*)Expression.c_str()));
		}

		std::string CTextBox::zzGet_ValidationExpression()
		{
			return _ValidationExpression;
		}

	}
}
