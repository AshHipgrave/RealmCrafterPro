//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#include "CItemButton.h"
#include <SGUIMeshBuilder.h>

using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;

namespace RealmCrafter
{
	CItemButton::CItemButton(NGin::Math::Vector2& screenScale, IGUIManager* manager) : IItemButton(screenScale, manager),
		Manager(manager), MeshBuffer(NULL), Button(NULL), TimerValue(-1), HoldingItem(65535), HoldingSpell(65535), HoldingAmount(65535),
		CanHoldItems(true), CanHoldSpells(true), DisplayOnly(false)
	{
		_Type = Type("ZITB", "CItemButton SDK Item/Spell container");
		ClickEvent = new NGin::GUI::EventHandler();
		RightClickEvent = new NGin::GUI::EventHandler();
		MouseEnterEvent = new NGin::GUI::EventHandler();
	}

	CItemButton::~CItemButton()
	{
		if(MeshBuffer != 0)
			Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
	}

	void CItemButton::OnDeviceLost()
	{
		IControl::OnDeviceLost();
	}

	void CItemButton::OnDeviceReset()
	{
		IControl::OnDeviceReset();

		CItemButton::OnTransform();
	}

	bool CItemButton::Update(GUIUpdateParameters* Parameters)
	{
		if(!_Enabled)
			return true;

		if(DisplayOnly)
			return true;

		List<IControl*> Controls;
		foreachc(CIt, IControl, _Controls)
		{
			Controls.Insert((*CIt), 0);

			nextc(CIt, IControl, _Controls);
		}

		foreachf(cCIt, IControl, Controls)
		{
			if((*cCIt)->Update(Parameters))
				return true;

			nextf(cCIt, IControl, Controls);
		}

		return false;
	}

	void CItemButton::Render()
	{
		if(!_Visible)
			return;

		// Render the child controls first
		foreachc(CIt, IControl, _Controls)
		{
			(*CIt)->Render();

			nextc(CIt, IControl, _Controls);
		}

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

		Renderer->DrawMeshBuffer(MeshBuffer, 0, 16);




		Renderer->PopScissorRect();
	}

	bool CItemButton::Initialize()
	{
		this->Button = Manager->CreateButton(_Name + "INTERNALBUTTON", Vector2(0, 0), _Size);
		this->Button->Parent = this;
		this->Button->Align = TextAlign_Right;
		this->Button->VAlign = TextAlign_Bottom;
		_ForeColor = this->Button->ForeColor;


		Button->Click()->AddEvent(this, &CItemButton::Button_Click);
		Button->RightClick()->AddEvent(this, &CItemButton::Button_RightClick);
		Button->MouseEnter()->AddEvent(this, &CItemButton::Button_MouseEnter);

		Button->Text = "";

		SetHeldItem(65535, 65535);

		return true;
	}

	void CItemButton::RebuildMesh()
	{
		if(_Locked)
			return;

		// Locals used for windows building
		ISkin* Skin = Manager->GetSkin(this->_Skin);
		if(Skin == NULL)
			return;

		// Store all of the position/scale information of each quad
#pragma region Quad Locations


		Vector2 Ts(_Size.X - Skin->GetScreenCoord(Button_Border_TL_Up).X - Skin->GetScreenCoord(Button_Border_TR_Up).X, Skin->GetScreenCoord(Button_Border_T_Up).Y);
		Vector2 Bs(0, Skin->GetScreenCoord(Button_Border_B_Up).Y);

		Vector2 Cp = Skin->GetScreenCoord(Button_Border_TL_Up);
		Vector2 Cs = Vector2(Ts.X, _Size.Y - Ts.Y - Bs.Y);

		Vector2 Center = Cp + (Cs * 0.5f);
		Vector2 TopMid = Cp + Math::Vector2(Cs.X * 0.5f, 0.0f);
		Vector2 TopRight = Cp + Math::Vector2(Cs.X, 0.0f);
		Vector2 BottomRight = Cp + Cs;
		Vector2 BottomLeft = Cp + Math::Vector2(0.0f, Cs.Y);
		Vector2 TopLeft = Cp;

		Vector2 MinCoord = Skin->GetCoord(Window_Fill);
		Vector2 SizeCoord = Skin->GetSize(Window_Fill);

		Vector2 CenterTc = MinCoord + Vector2(0.5f, 0.5f) * SizeCoord;
		Vector2 TopMidTc = MinCoord + Vector2(0.5f, 0.0f) * SizeCoord;
		Vector2 TopRightTc = MinCoord + Vector2(1, 0) * SizeCoord;
		Vector2 BottomRightTc = MinCoord + Vector2(1, 1) * SizeCoord;
		Vector2 BottomLeftTc = MinCoord + Vector2(0, 1) * SizeCoord;
		Vector2 TopLeftTc = MinCoord + Vector2(0, 0) * SizeCoord;

		Color TColor = _BackColor;
		TColor.A *= 0.5f;

#pragma endregion

		// Build the GUI
#pragma region GUI Building

		SGUIMeshBuilder Builder(Manager->GetRenderer());

		if(TimerValue > -1)
		{
			float Scaled = ((TimerValue * -3.60f) * DEGTORAD) + 3.141592f;
			float Dx = sin(Scaled);
			float Dy = cos(Scaled);
			float Tx = Dx, Ty = Dy;

			//Dx *= 100.0f;
			//Dy *= 100.0f;
			Dx *= Cs.X * 0.7f;
			Dy *= Cs.Y * 0.7f;

			if(Dx > Cs.X * 0.5f)
				Dx = Cs.X * 0.5f;
			if(Dx < -Cs.X * 0.5f)
				Dx = -Cs.X * 0.5f;
			if(Dy > Cs.Y * 0.5f)
				Dy = Cs.Y * 0.5f;
			if(Dy < -Cs.Y * 0.5f)
				Dy = -Cs.Y * 0.5f;

			Dx += Center.X;
			Dy += Center.Y;
			Tx = MinCoord.X + (Tx * SizeCoord.X);
			Ty = MinCoord.Y + (Ty * SizeCoord.Y);

			if(TimerValue > 12.5f)
			{
				Builder.AddTriangle(TopMid, TopRight, Center, TopMidTc, TopRightTc, CenterTc, TColor);

				if(TimerValue > 37.5f)
				{
					Builder.AddTriangle(Center, TopRight, BottomRight, CenterTc, TopRightTc, BottomRightTc, TColor);

					if(TimerValue > 62.5f)
					{
						Builder.AddTriangle(BottomLeft, Center, BottomRight, BottomLeftTc, CenterTc, BottomRightTc, TColor);

						if(TimerValue > 87.5f)
						{
							Builder.AddTriangle(TopLeft, Center, BottomLeft, TopLeftTc, CenterTc, BottomLeftTc, TColor);

							if(TimerValue >= 100.0f)
							{
								Builder.AddTriangle(TopLeft, TopMid, Center, TopLeftTc, TopMidTc, CenterTc, TColor);
							}else
							{
								Builder.AddTriangle(TopLeft, Vector2(Dx, Dy), Center, TopLeftTc, Vector2(Tx, Ty), CenterTc, TColor);
							}
						}else
						{
							Builder.AddTriangle(Vector2(Dx, Dy), Center, BottomLeft, Vector2(Tx, Ty), CenterTc, BottomLeftTc, TColor);
						}
					}else
					{
						Builder.AddTriangle(Vector2(Dx, Dy), Center, BottomRight, Vector2(Tx, Ty), CenterTc, BottomRightTc, TColor);
					}
				}else
				{
					Builder.AddTriangle(Center, TopRight, Vector2(Dx, Dy), CenterTc, TopRightTc, Vector2(Tx, Ty), TColor);
				}
			}else
			{
				Builder.AddTriangle(TopMid, Vector2(Dx, Dy), Center, TopMidTc, Vector2(Tx, Ty), CenterTc, TColor);
			}

		}

#pragma endregion

		// Create buffer
		if(MeshBuffer != 0)
			Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		if(TimerValue > -1)
			MeshBuffer = Builder.Build();
		else
			MeshBuffer = NULL;
	}

	float CItemButton::GetTimerValue()
	{
		return TimerValue;
	}

	void CItemButton::SetTimerValue(float timer)
	{
		TimerValue = timer;
		RebuildMesh();
	}

	unsigned short CItemButton::GetHeldItem()
	{
		return HoldingItem;
	}

	unsigned short CItemButton::GetHeldSpell()
	{
		return HoldingSpell;
	}

	unsigned short CItemButton::GetHeldAmount()
	{
		return HoldingAmount;
	}

	void CItemButton::SetHeldItem(unsigned short id, unsigned short amnt)
	{
		if(!CanHoldItems)
			return;

		// Save reloading an image
		if(HoldingItem == id && HoldingSpell == 65535)
		{
			HoldingAmount = amnt;	
			Button->Text = (amnt != 65535 && amnt > 1) ? std::toString(amnt) : "";
			return;
		}

		HoldingSpell = 65535;
		HoldingItem = id;
		HoldingAmount = amnt;

		if(HoldingSpell == 65535 && HoldingItem == 65535)
			Button->SetUpImage(BackgroundImage);
		else
			Button->SetUpImage(GetItemPathFromID(id));
		Button->Text = (amnt != 65535 && amnt > 1) ? std::toString(amnt) : "";
	}

	void CItemButton::SetHeldSpell(unsigned short id)
	{
		if(!CanHoldSpells)
			return;

		HoldingItem = 65535;
		HoldingSpell = id;
		HoldingAmount = 65535;

		if(HoldingSpell == 65535 && HoldingItem == 65535)
			Button->SetUpImage(BackgroundImage);
		else
			Button->SetUpImage(GetSpellPathFromID(id));
		Button->Text = "";
	}

	bool CItemButton::GetCanHoldItems()
	{
		return CanHoldItems;
	}

	bool CItemButton::GetCanHoldSpells()
	{
		return CanHoldSpells;
	}

	void CItemButton::SetCanHoldItems(bool enable)
	{
		CanHoldItems = enable;
	}

	void CItemButton::SetCanHoldSpells(bool enable)
	{
		CanHoldSpells = enable;
	}

	std::string CItemButton::GetBackgroundImage()
	{
		return BackgroundImage;
	}

	void CItemButton::SetBackgroundImage(std::string path)
	{
		BackgroundImage = path;

		if(HoldingSpell == 65535 && HoldingItem == 65535)
			Button->SetUpImage(path);
	}

	void CItemButton::OnSizeChange()
	{
		IControl::OnSizeChange();
		this->RebuildMesh();
	}

	void CItemButton::OnTransform()
	{
		IControl::OnTransform();

		Button->Location = Vector2(0, 0);
		Button->Size = _Size;
	}

	void CItemButton::OnEnabledChange()
	{
		IControl::OnEnabledChange();
		this->Button->Enabled = _Enabled;
	}

	void CItemButton::OnBackColorChange()
	{
		IControl::OnBackColorChange();
		this->Button->BackColor = _BackColor;
		this->RebuildMesh();
	}

	void CItemButton::OnForeColorChange()
	{
		IControl::OnForeColorChange();
		this->Button->ForeColor = _ForeColor;
	}

	void CItemButton::OnSkinChange()
	{
		IControl::OnSkinChange();
		this->RebuildMesh();
		this->Button->Skin = _Skin;
	}

	NGin::GUI::EventHandler* CItemButton::Click()
	{
		return ClickEvent;
	}

	NGin::GUI::EventHandler* CItemButton::RightClick()
	{
		return RightClickEvent;
	}

	void CItemButton::SetDisplayOnly( bool display )
	{
		DisplayOnly = display;
		Button->UseBorder = !display;
	}

	bool CItemButton::GetDisplayOnly()
	{
		return DisplayOnly;
	}

	void CItemButton::Button_RightClick( NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e )
	{
		RightClickEvent->Execute(this, e);
	}

	void CItemButton::Button_Click( NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e )
	{
		ClickEvent->Execute(this, e);
	}

	void CItemButton::Button_MouseEnter(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e)
	{
		MouseEnterEvent->Execute(this, e);
	}

	NGin::GUI::EventHandler* CItemButton::MouseEnter()
	{
		return MouseEnterEvent;
	}


	IItemButton* CreateItemButton(NGin::GUI::IGUIManager* manager, std::string name, NGin::Math::Vector2 location, NGin::Math::Vector2 size)
	{
		RealmCrafter::CItemButton* Button = new RealmCrafter::CItemButton(NGin::Math::Vector2(1.0f, 1.0f) / manager->GetResolution(), manager);
		Button->Initialize();
		Button->Locked = true;
		Button->Skin = 1;
		Button->Name = name;
		Button->Text = name;
		Button->Location = location;


		Button->Locked = false;
		Button->Parent = manager;

		Button->Size = size;

		return Button;
	}
}
