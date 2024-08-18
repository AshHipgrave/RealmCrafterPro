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
#include "CBubbleOutput.h"
#include <SGUIMeshBuilder.h>

using namespace NGin;
using namespace NGin::Math;
using namespace NGin::GUI;

namespace RealmCrafter
{
	CBubbleOutput::CBubbleOutput(NGin::Math::Vector2& screenScale, NGin::GUI::IGUIManager* manager)
		: IControl(screenScale, manager)
	{
		Manager = manager;
		MeshBuffer = 0;
		Caption = 0;
	}

	CBubbleOutput::~CBubbleOutput()
	{
		if(MeshBuffer != 0)
			Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		MeshBuffer = 0;
	}

	void CBubbleOutput::OnDeviceLost()
	{
		IControl::OnDeviceLost();
	}

	void CBubbleOutput::OnDeviceReset()
	{
		IControl::OnDeviceReset();
	}

	void CBubbleOutput::OnTransform()
	{
		IControl::OnTransform();

		if(_Skin == 0)
			return;

		// Get Skin
		ISkin* Skin = Manager->GetSkin(_Skin);

		Vector2 CaptionPosition = Skin->GetScreenCoord("BubbleOutput_TL");
		Vector2 CaptionSize(
			_Size.X - (Skin->GetScreenCoord("BubbleOutput_TL").X + Skin->GetScreenCoord("BubbleOutput_TR").X), 
			_Size.Y - (Skin->GetScreenCoord("BubbleOutput_T").Y + Skin->GetScreenCoord("BubbleOutput_B").Y));

		Caption->Location = CaptionPosition;
		Caption->Size = CaptionSize;

		if(Caption->InternalHeight > CaptionSize.Y)
		{
			Vector2 NewSize = _Size;
			NewSize.Y = Caption->InternalHeight + (Skin->GetScreenCoord("BubbleOutput_T").Y + Skin->GetScreenCoord("BubbleOutput_B").Y);

			// Test size
			CaptionSize.Y = NewSize.Y - (Skin->GetScreenCoord("BubbleOutput_T").Y + Skin->GetScreenCoord("BubbleOutput_B").Y);
			if(Caption->InternalHeight > CaptionSize.Y)
				return;
			else
				this->Size = NewSize;
		}else
		{
			CaptionPosition -= Vector2(_Size.X * 0.5f, _Size.Y + Skin->GetScreenCoord("BubbleOutput_Point").Y);
			Caption->Location = CaptionPosition;
		}

	}

	void CBubbleOutput::OnTextChange()
	{
		IControl::OnTextChange();

		Caption->Text = _Text;
	}

	void CBubbleOutput::OnForeColorChange()
	{
		IControl::OnForeColorChange();
		Caption->ForeColor = _ForeColor;
	}

	void CBubbleOutput::OnBackColorChange()
	{
		IControl::OnBackColorChange();
		RebuildMesh();
	}

	void CBubbleOutput::OnSkinChange()
	{
		IControl::OnSkinChange();
		RebuildMesh();
		Caption->Font = Manager->GetSkin(_Skin)->GetFont(GUIFont_Control);
	}

	void CBubbleOutput::OnSizeChange()
	{
		IControl::OnSizeChange();

		RebuildMesh();
	}

	void CBubbleOutput::Render()
	{
		if(!_Visible)
			return;
		if(_Skin == 0)
			return;
		if(MeshBuffer == 0)
			return;

		// Set our rendering constants
		Manager->SetSkin(_Skin);

		// Get renderer
		IGUIRenderer* Renderer = Manager->GetRenderer();

		Vector2 RenderPosition = _GlobalLocation;
		RenderPosition.X -= _Size.X * 0.5f;
		RenderPosition.Y -= _Size.Y + Manager->GetSkin(_Skin)->GetScreenCoord("BubbleOutput_Point").Y;
		Manager->SetPosition(RenderPosition);

		// Forget a scissor region for the moment
		Vector4 WinSize;
		WinSize.X = WinSize.Y = 0;
		WinSize.Z = Manager->GetResolution().X;
		WinSize.W = Manager->GetResolution().Y;

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

	bool CBubbleOutput::Update(NGin::GUI::GUIUpdateParameters* parameters)
	{
		return true;
	}

	
	bool CBubbleOutput::Initialize()
	{
		Caption = Manager->CreateLabel("CBubbleOutput::ChildLabel", Vector2(0, 0), Vector2(100, 100));
		Caption->Parent = this;
		Caption->Multiline = true;

		return true;
	}

	void CBubbleOutput::RebuildMesh()
	{
		if(_Locked)
			return;

		// Locals used for windows building
		ISkin* Skin = Manager->GetSkin(this->_Skin);
		if(Skin == 0)
		{
			MessageBox(0, "Error: Skin does not exist!", "CBubbleOutput::RebuildMesh()", MB_ICONERROR);
			exit(0);
		}

		Vector2 TLp(0, 0);
		Vector2 TLs = Skin->GetScreenCoord("BubbleOutput_TL");

		Vector2 Tp(TLs.X, 0);
		Vector2 Ts(_Size.X - Skin->GetScreenCoord("BubbleOutput_TL").X - Skin->GetScreenCoord("BubbleOutput_TR").X, Skin->GetScreenCoord("BubbleOutput_T").Y);

		Vector2 TRp(Ts.X + TLs.X, 0);
		Vector2 TRs = Skin->GetScreenCoord("BubbleOutput_TR");

		Vector2 Lp(0, Skin->GetScreenCoord("BubbleOutput_TL").Y);
		Vector2 Ls = Vector2(Skin->GetScreenCoord("BubbleOutput_L").X, _Size.Y - Skin->GetScreenCoord("BubbleOutput_TL").Y - Skin->GetScreenCoord("BubbleOutput_BL").Y);

		Vector2 Rp(Ts.X + TLs.X, Skin->GetScreenCoord("BubbleOutput_TL").Y);
		Vector2 Rs = Vector2(Skin->GetScreenCoord("BubbleOutput_R").X, _Size.Y - Skin->GetScreenCoord("BubbleOutput_TR").Y - Skin->GetScreenCoord("BubbleOutput_BR").Y);

		Vector2 BLp(0, Rs.Y + TLs.Y);
		Vector2 BLs = Skin->GetScreenCoord("BubbleOutput_BL");

		Vector2 Bp(BLs.X, BLp.Y);
		Vector2 Bs(_Size.X - Skin->GetScreenCoord("BubbleOutput_BL").X - Skin->GetScreenCoord("BubbleOutput_BR").X, Skin->GetScreenCoord("BubbleOutput_B").Y);

		Vector2 BRp(Bs.X + BLs.X, Rs.Y + TRs.Y);
		Vector2 BRs = Skin->GetScreenCoord("BubbleOutput_BR");

		Vector2 Cp(TLs.X, Skin->GetScreenCoord("BubbleOutput_TL").Y);
		Vector2 Cs = Vector2(Ts.X, _Size.Y - Ts.Y - Bs.Y);

		Vector2 Pp(Cp.X + (Ts.X * 0.5f), Bp.Y + Bs.Y);
		Vector2 Ps = Skin->GetScreenCoord("BubbleOutput_Point");

		SGUIMeshBuilder Builder(Manager->GetRenderer());

		Builder.AddQuad(TLp, TLs, Skin->GetCoord("BubbleOutput_TL"), Skin->GetSize("BubbleOutput_TL"), _BackColor);
		Builder.AddQuad(Tp, Ts, Skin->GetCoord("BubbleOutput_T"), Skin->GetSize("BubbleOutput_T"), _BackColor);
		Builder.AddQuad(TRp, TRs, Skin->GetCoord("BubbleOutput_TR"), Skin->GetSize("BubbleOutput_TR"), _BackColor);
		Builder.AddQuad(Lp, Ls, Skin->GetCoord("BubbleOutput_L"), Skin->GetSize("BubbleOutput_L"), _BackColor);
		Builder.AddQuad(Rp, Rs, Skin->GetCoord("BubbleOutput_R"), Skin->GetSize("BubbleOutput_R"), _BackColor);
		Builder.AddQuad(BLp, BLs, Skin->GetCoord("BubbleOutput_BL"), Skin->GetSize("BubbleOutput_BL"), _BackColor);
		Builder.AddQuad(Bp, Bs, Skin->GetCoord("BubbleOutput_B"), Skin->GetSize("BubbleOutput_B"), _BackColor);
		Builder.AddQuad(BRp, BRs, Skin->GetCoord("BubbleOutput_BR"), Skin->GetSize("BubbleOutput_BR"), _BackColor);
		Builder.AddQuad(Cp, Cs, Skin->GetCoord("BubbleOutput_Fill"), Skin->GetSize("BubbleOutput_Fill"), _BackColor);
		Builder.AddQuad(Pp, Ps, Skin->GetCoord("BubbleOutput_Point"), Skin->GetSize("BubbleOutput_Point"), _BackColor);

		if(MeshBuffer != NULL)
			Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
		MeshBuffer = Builder.Build();
	}

	CBubbleOutput* CreateBubbleOutput(NGin::GUI::IGUIManager* manager, std::string name, NGin::Math::Vector2& location, NGin::Math::Vector2& size)
	{
		// Create a picturebox!
		CBubbleOutput* BubbleOutput = new CBubbleOutput(NGin::Math::Vector2(1.0f, 1.0f) / manager->GetResolution(), manager);
		BubbleOutput->Initialize();
		BubbleOutput->Locked = true;
		BubbleOutput->Name = name;
		BubbleOutput->Text = name;
		BubbleOutput->Location = location;

		BubbleOutput->Locked = false;

		ISkin* Skin = manager->GetSkin(1);
		if(Skin != 0)
		{
			BubbleOutput->Skin = 1;
			Skin->ApplyDefaultProperty(BubbleOutput, "BubbleOutput");
		}

		BubbleOutput->Size = size;
		BubbleOutput->Parent = manager;

		return BubbleOutput;
	}
}
