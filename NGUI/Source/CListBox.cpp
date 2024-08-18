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
#include "CListBox.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type IListBox::TypeOf()
		{
			return Type("CLIS", "CListBox GUI ListBox");
		}

		CListBox::CListBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IListBox(ScreenScale, Manager)
		{
			_Type = Type("CLIS", "CListBox GUI ListBox");
			MeshBuffer = 0;
			Manager = 0;
			ScrollPosition = 0;
			_SelectedIndex = 0;
			_SelectionBackColor = Color(0.2f, 0.6f, 1.0f);
			_SelectionForeColor = Color(1.0f, 1.0f, 1.0f);
			ButtonState = 0;

			SelectedIndexChangedEvent = new EventHandler();
		}

		CListBox::~CListBox()
		{
			delete SelectedIndexChangedEvent;

			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		}

		void CListBox::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CListBox::OnDeviceReset()
		{
			CListBox::OnSizeChange();

			IControl::OnDeviceReset();
		}

		bool CListBox::Update(GUIUpdateParameters* Parameters)
		{
			ISkin* Skin = Manager->GetSkin(_Skin);
			float T = Skin->GetScreenCoord(ListBox_Border_T).Y;
			float B = Skin->GetScreenCoord(ListBox_Border_B).Y;
			float R = Skin->GetScreenCoord(ListBox_Border_R).X;
			float L = Skin->GetScreenCoord(ListBox_Border_L).X;

			if(_Visible && Parameters->MousePosition > _GlobalLocation + Vector2(L, T) && Parameters->MousePosition < (_GlobalLocation + Vector2(AllowedSize, _Size.Y - (T + B))) && Parameters->Handled == false)
			{
				Parameters->MouseBusy = true;
				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;

				if(Parameters->LeftDown)
					Manager->ControlFocus(this);

				//if(Parameters->LeftDown)
				//{
				//	if(ButtonState == 1)
				//	{
				//		ButtonState = 2;

				//		if(ScrollPosition > Items.Size())
				//			ScrollPosition = Items.Size();

				//		float MaxY = _Size.Y - Skin->GetScreenCoord(ListBox_Border_B).Y;

				//		int LastI = -1;
				//		for(int i = ScrollPosition; i < ScrollPosition + VisibleItems; ++i)
				//		{
				//			ILabel* L = Items[i]->LabelHandle;

				//			if(Parameters->MousePosition.Y - _GlobalLocation.Y > L->Location().Y && Parameters->MousePosition.Y - _GlobalLocation.Y < L->Location().Y + L->Size().Y)
				//			{
				//				LastI = i;
				//				break;
				//			}
				//		}

				//		if(LastI > -1)
				//		{
				//			_SelectedIndex = LastI;

				//			EventArgs E;
				//			SelectedIndexChangedEvent->Execute(this, &E);

				//			RecalculateLabels();
				//		}
				//	}
				//}else
				//{
				//	ButtonState = 1;
				//}

				if(Parameters->LeftDown && ButtonState == 0)
				{
					ButtonState = 1;

					if(ScrollPosition > Items.Size())
						ScrollPosition = Items.Size();

					float MaxY = _Size.Y - Skin->GetScreenCoord(ListBox_Border_B).Y;

					int LastI = -1;
					if(Items.Size() > 0)
					{
						for(int i = ScrollPosition; i < ScrollPosition + VisibleItems; ++i)
						{
							ILabel* L = Items[i]->LabelHandle;

							if(Parameters->MousePosition.Y - _GlobalLocation.Y > L->Location.Y && Parameters->MousePosition.Y - _GlobalLocation.Y < L->Location.Y + L->Size.Y)
							{
								LastI = i;
								break;
							}
						}
					}

					if(LastI > -1)
					{
						_SelectedIndex = LastI;


						RecalculateLabels();
					}
				}else if(Parameters->LeftDown == false && ButtonState == 1)
				{
					ButtonState = 0;
					EventArgs E;
					SelectedIndexChangedEvent->Execute(this, &E);
				}


				Parameters->Handled = true;
				return true;
			}else
			{
				ButtonState = 0;
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

		void CListBox::Render()
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

		bool CListBox::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			VerticalScroll = Manager->CreateScrollBar("CCCVSCROLL", Vector2(0,0), Vector2(0,0), NGin::GUI::VerticalScroll);
			VerticalScroll->Parent = this;
			VerticalScroll->Enabled = false;
			VerticalScroll->Tag = this;
			VerticalScroll->Scroll()->AddEvent(&ListBoxScrollEventCallback);

			return true;
		}

		void CListBox::RebuildMesh()
		{
			if(_Locked)
				return;

			// Locals used for windows building
			ISkin* Skin = Manager->GetSkin(this->_Skin);

			// Store all of the position/scale information of each quad
		#pragma region Quad Locations

			Vector2 TLp(0, 0);
			Vector2 TLs = Skin->GetScreenCoord(ListBox_Border_TL);

			Vector2 Tp(TLs.X, 0);
			Vector2 Ts(_Size.X - Skin->GetScreenCoord(ListBox_Border_TL).X - Skin->GetScreenCoord(ListBox_Border_TR).X, Skin->GetScreenCoord(ListBox_Border_T).Y);

			Vector2 TRp(Ts.X + TLs.X, 0);
			Vector2 TRs = Skin->GetScreenCoord(ListBox_Border_TR);

			Vector2 Lp(0, Skin->GetScreenCoord(ListBox_Border_TL).Y);
			Vector2 Ls = Vector2(Skin->GetScreenCoord(ListBox_Border_L).X, _Size.Y - Skin->GetScreenCoord(ListBox_Border_TL).Y - Skin->GetScreenCoord(ListBox_Border_BL).Y);

			Vector2 Rp(Ts.X + TLs.X, Skin->GetScreenCoord(ListBox_Border_TL).Y);
			Vector2 Rs = Vector2(Skin->GetScreenCoord(ListBox_Border_R).X, _Size.Y - Skin->GetScreenCoord(ListBox_Border_TR).Y - Skin->GetScreenCoord(ListBox_Border_BR).Y);
			
			Vector2 BLp(0, Rs.Y + TLs.Y);
			Vector2 BLs = Skin->GetScreenCoord(ListBox_Border_BL);
			
			Vector2 Bp(BLs.X, BLp.Y);
			Vector2 Bs(_Size.X - Skin->GetScreenCoord(ListBox_Border_BL).X - Skin->GetScreenCoord(ListBox_Border_BR).X, Skin->GetScreenCoord(ListBox_Border_B).Y);

			Vector2 BRp(Bs.X + BLs.X, Rs.Y + TRs.Y);
			Vector2 BRs = Skin->GetScreenCoord(ListBox_Border_BR);

			Vector2 Cp(TLs.X, Skin->GetScreenCoord(ListBox_Border_T).Y);
			Vector2 Cs = Vector2(Ts.X, _Size.Y - Ts.Y - Bs.Y);

		#pragma endregion
			
			// Build the GUI
		#pragma region GUI Building

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Builder.AddQuad(TLp, TLs, Skin->GetCoord(ListBox_Border_TL), Skin->GetSize(ListBox_Border_TL), _BackColor);
			Builder.AddQuad(Tp, Ts, Skin->GetCoord(ListBox_Border_T), Skin->GetSize(ListBox_Border_T), _BackColor);
			Builder.AddQuad(TRp, TRs, Skin->GetCoord(ListBox_Border_TR), Skin->GetSize(ListBox_Border_TR), _BackColor);
			Builder.AddQuad(Lp, Ls, Skin->GetCoord(ListBox_Border_L), Skin->GetSize(ListBox_Border_L), _BackColor);
			Builder.AddQuad(Cp, Cs, Skin->GetCoord(ListBox_Fill), Skin->GetSize(ListBox_Fill), _BackColor);
			Builder.AddQuad(Rp, Rs, Skin->GetCoord(ListBox_Border_R), Skin->GetSize(ListBox_Border_R), _BackColor);
			Builder.AddQuad(BLp, BLs, Skin->GetCoord(ListBox_Border_BL), Skin->GetSize(ListBox_Border_BL), _BackColor);
			Builder.AddQuad(Bp, Bs, Skin->GetCoord(ListBox_Border_B), Skin->GetSize(ListBox_Border_B), _BackColor);
			Builder.AddQuad(BRp, BRs, Skin->GetCoord(ListBox_Border_BR), Skin->GetSize(ListBox_Border_BR), _BackColor);

			if(_SelectedIndex >= 0 && _SelectedIndex < Items.Size() && Items[_SelectedIndex]->LabelHandle->Visible)
			{
				Builder.AddQuad(
					Items[_SelectedIndex]->LabelHandle->Location,
					Vector2(AllowedSize, Items[_SelectedIndex]->LabelHandle->Size.Y),
					Skin->GetCoord(ListBox_SelectionFill),
					Skin->GetSize(ListBox_SelectionFill),
					_SelectionBackColor);
			}else
			{
				Builder.AddQuad(
					Vector2(0,0),
					Vector2(0,0),
					Skin->GetCoord(ListBox_SelectionFill),
					Skin->GetSize(ListBox_SelectionFill),
					_SelectionBackColor);
			}

		#pragma endregion

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		void CListBox::RecalculateLabels()
		{
			if(Items.Size() == 0)
				return;

			if(ScrollPosition > Items.Size())
				ScrollPosition = Items.Size();

			for(int i = 0; i < ScrollPosition; ++i)
				Items[i]->LabelHandle->Visible = false;

			ISkin* Skin = Manager->GetSkin(_Skin);
			float X = Skin->GetScreenCoord(ListBox_Border_L).X;
			float Y = Skin->GetScreenCoord(ListBox_Border_T).Y;

			float MaxY = _Size.Y - Skin->GetScreenCoord(ListBox_Border_B).Y;

			int LastI = ScrollPosition;
			for(int i = ScrollPosition; i < Items.Size(); ++i)
			{
				ILabel* L = Items[i]->LabelHandle;
				L->Visible = true;
				
				L->Locked = true;
				L->ForeColor = (i == _SelectedIndex) ? _SelectionForeColor : _ForeColor;
				L->Locked = false;

				L->Location = Vector2(X, Y);
				L->Size = Vector2(Items[i]->LabelHandle->Size.X, 0);
				Y += L->InternalHeight;

				LastI = i;
				if(Y > MaxY)
				{
					--LastI;
					Y -= L->InternalHeight;
					L->Visible = false;
					break;
				}
			}

			for(int i = LastI + 1; i < Items.Size(); ++i)
				Items[i]->LabelHandle->Visible = false;

			VisibleItems = ((LastI + 1) - ScrollPosition);
			if(VisibleItems < Items.Size())
			{
				VerticalScroll->Enabled = true;
				VerticalScroll->Minimum = 0;
				VerticalScroll->Maximum = Items.Size();
				VerticalScroll->SmallChange = 1;
				VerticalScroll->LargeChange = VisibleItems;
				VerticalScroll->Value = ScrollPosition;
			}else
				VerticalScroll->Enabled = false;

			this->RebuildMesh();
		}

		void CListBox::OnSizeChange()
		{
			IControl::OnSizeChange();

			if(_Skin == 0)
				return;

			ISkin* Skin = Manager->GetSkin(_Skin);
			float T = Skin->GetScreenCoord(ListBox_Border_T).Y;
			float B = Skin->GetScreenCoord(ListBox_Border_B).Y;
			float R = Skin->GetScreenCoord(ListBox_Border_R).X;

			Vector2 ScrollSize = _Size;
			ScrollSize.Y -= (T + B);
			VerticalScroll->Size = ScrollSize;
			
			Vector2 ScrollPos(_Size.X - VerticalScroll->Size.X - R, T);
			//Vector2 ScrollPos(0, 0);
			VerticalScroll->Location = ScrollPos;

			AllowedSize = ScrollPos.X;


			
			RecalculateLabels();
		}

		void CListBox::OnEnabledChange()
		{
			IControl::OnEnabledChange();
			RebuildMesh();
		}

		void CListBox::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		void CListBox::OnForeColorChange()
		{
			IControl::OnForeColorChange();
		}

		void CListBox::OnSkinChange()
		{
			IControl::OnSkinChange();
			VerticalScroll->Skin = _Skin;
			this->RebuildMesh();
		}

		int CListBox::AddItem(std::string Value)
		{
			CListBoxItem* I = new CListBoxItem();
			I->LabelHandle = Manager->CreateLabel(Value, Vector2(0, 0), Vector2(0,0));
			I->LabelHandle->Parent = this;
			I->Index = Items.Size();
			I->LabelHandle->Visible = false;
			Items.Add(I);

			RecalculateLabels();
			return I->Index;
		}

		void CListBox::zzPut_SelectionForeColor(Math::Color Value)
		{
			_SelectionForeColor = Value;
		}

		Math::Color CListBox::zzGet_SelectionForeColor()
		{
			return _SelectionForeColor;
		}

		void CListBox::zzPut_SelectionBackColor(Math::Color Value)
		{
			_SelectionBackColor = Value;
		}

		Math::Color CListBox::zzGet_SelectionBackColor()
		{
			return _SelectionBackColor;
		}


		int CListBox::zzGet_SelectedIndex()
		{
			return _SelectedIndex;
		}

		void CListBox::zzPut_SelectedIndex(int SelectedIndex)
		{
			if(SelectedIndex > 0 && SelectedIndex < Items.Size())
			{
				_SelectedIndex = SelectedIndex;
				RecalculateLabels();
			}
		}

		std::string CListBox::zzGet_SelectedValue()
		{
			if(_SelectedIndex > -1 && _SelectedIndex < Items.Size())
				return Items[_SelectedIndex]->LabelHandle->Text;
			return "";
		}

		void CListBox::zzPut_SelectedValue(std::string SelectedValue)
		{
			if(_SelectedIndex > -1 && _SelectedIndex < Items.Size())
				Items[_SelectedIndex]->LabelHandle->Text = SelectedValue;
		}

		void CListBox::zzPut_ItemValue(int Index, std::string Value)
		{
			if(Index > -1 && Index < Items.Size())
				Items[Index]->LabelHandle->Text = Value;
		}

		std::string CListBox::zzGet_ItemValue(int Index)
		{
			if(Index > -1 && Index < Items.Size())
				return Items[Index]->LabelHandle->Text;
			return "";
		}

		EventHandler* CListBox::SelectedIndexChanged()
		{
			return SelectedIndexChangedEvent;
		}

		void CListBox::DeleteItem(int Index)
		{
			if(Index > -1 && Index < Items.Size())
			{
				Manager->Destroy(Items[Index]->LabelHandle);
				delete Items[Index];
				Items.Remove(Index);
				RecalculateLabels();
			}
		}

		int CListBox::ItemCount()
		{
			return Items.Size();
		}

		void CListBox::ListBoxScrollEventCallback(IControl* Sender, ScrollEventArgs* E)
		{
			CListBox* L = (CListBox*)Sender->Tag;
			L->ScrollPosition = E->NewValue();
			if(L->ScrollPosition > L->Items.Size() - L->VisibleItems)
				L->ScrollPosition = L->Items.Size() - L->VisibleItems;
			L->RecalculateLabels();
		}


	}
}
