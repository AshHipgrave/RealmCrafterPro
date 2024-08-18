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
#include "CSelectionSize.h"
#include <SGUIMeshBuilder.h>
#include <windows.h>

using namespace NGin;
using namespace NGin::Math;

namespace NGin
{
	namespace GUI
	{
		Type CSelectionSize::TypeOf()
		{
			return Type("CSEL", "CSelectionSize GUI");
		}

		CSelectionSize::CSelectionSize(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IControl(ScreenScale, Manager)
		{
			_Type = Type("CSEL", "CSelectionSize GUI");
			MeshBuffer = 0;
			ButtonState = 0;

			_MouseOver = false;
			ClickEvent = new EventHandler();
			DownEvent = new EventHandler();

			Texture = 0;
		}

		CSelectionSize::~CSelectionSize()
		{
			delete ClickEvent;
			delete DownEvent;

			if(MeshBuffer != 0)
				_Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			if(Texture != 0)
				_Manager->GetRenderer()->FreeTexture(Texture);
			Texture = 0;
		}

		void CSelectionSize::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CSelectionSize::OnDeviceReset()
		{
			IControl::OnDeviceReset();

			CSelectionSize::OnTransform();
		}

		bool CSelectionSize::Update(GUIUpdateParameters* Parameters)
		{
			if(!_Enabled)
				return true;

			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
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

		void CSelectionSize::Render()
		{
			if(!_Visible)
				return;
			if(MeshBuffer == 0)
				return;

			// Set us up for rendering
			_Manager->SetPosition(_GlobalLocation);

			// Get renderer
			IGUIRenderer* Renderer = _Manager->GetRenderer();

			// Store old scissor region
			Vector4 OldRect = Renderer->GetScissorRect();

			// Dimensions
			Vector2 CornerSize = Vector2(7, 7);

			// Create our scissor region
			Vector4 WinSize;
// 			WinSize.X = (_GlobalLocation.X * _Manager->GetResolution().X) - CornerSize.X;
// 			WinSize.Y = (_GlobalLocation.Y * _Manager->GetResolution().Y) - CornerSize.Y;
// 			WinSize.Z = (WinSize.X + (_Size.X * _Manager->GetResolution().X)) + (CornerSize.X * 2);
// 			WinSize.W = (WinSize.Y + (_Size.Y * _Manager->GetResolution().Y)) + (CornerSize.Y * 2);
	
			WinSize.X = 0;
			WinSize.Y = 0;
			WinSize.Z = _Manager->GetResolution().X;
			WinSize.W = _Manager->GetResolution().Y;


			// Make sure we fit inside the parent region (We are a subparent control)
// 			if(WinSize.X < OldRect.X)
// 				WinSize.X = OldRect.X;
// 			if(WinSize.Z > OldRect.Z)
// 				WinSize.Z = OldRect.Z;
// 			if(WinSize.Y < OldRect.Y)
// 				WinSize.Y = OldRect.Y;
// 			if(WinSize.W > OldRect.W)
// 				WinSize.W = OldRect.W;
// 			
// 			WinSize.Z += 5;
// 			WinSize.W += 5;

			_Manager->CorrectRect(WinSize);
			Renderer->PushScissorRect(WinSize);
			MeshBuffer->Set();

			_Manager->SetTexture(this->Texture);
			Renderer->DrawMeshBuffer(MeshBuffer);	

			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();
		}

		bool CSelectionSize::Initialize()
		{
			Texture = _Manager->GetRenderer()->GetTexture("Data\\Skins\\Default\\SizeTool.png", 0xffff00ff, true);

			return true;
		}

		void CSelectionSize::RebuildMesh()
		{
			if(_Locked)
				return;

			// Build the GUI
		#pragma region GUI Building

			SGUIMeshBuilder Builder(_Manager->GetRenderer());

			Vector2 CornerSize = Vector2(7, 7) / _Manager->GetResolution();

			Vector2 CornerMin = Vector2(0, 0);
			Vector2 CornerMax = Vector2(7, 7) / Vector2(128, 128);

			Vector2 HorizontalMin = Vector2(8, 0) / Vector2(128, 128);
			Vector2 HorizontalMax = Vector2(120, 7) / Vector2(128, 128);

			Vector2 VerticalMin = Vector2(0, 8) / Vector2(128, 128);
			Vector2 VerticalMax = Vector2(7, 120) / Vector2(128, 128);

			Vector2 HalfLocation = Vector2((Size.X * 0.5f), (Size.Y * 0.5f));
			Vector2 HalfCorner = CornerSize * 0.5f;

 			Builder.AddQuad(Vector2(0, -CornerSize.Y), Vector2(Size.X, CornerSize.Y), HorizontalMin, HorizontalMax, _BackColor);
  			Builder.AddQuad(Vector2(0, Size.Y), Vector2(Size.X, CornerSize.Y), HorizontalMin, HorizontalMax, _BackColor);
  			Builder.AddQuad(Vector2(-CornerSize.X, 0), Vector2(CornerSize.X, Size.Y), VerticalMin, VerticalMax, _BackColor);
  			Builder.AddQuad(Vector2(Size.X, 0), Vector2(CornerSize.X, Size.Y), VerticalMin, VerticalMax, _BackColor);

			Builder.AddQuad(Vector2(-CornerSize.X, -CornerSize.Y), CornerSize, CornerMin, CornerMax, _BackColor);
			Builder.AddQuad(Vector2(Size.X,-CornerSize.Y), CornerSize, CornerMin, CornerMax, _BackColor);
			Builder.AddQuad(Vector2(-CornerSize.X, Size.Y), CornerSize, CornerMin, CornerMax, _BackColor);
			Builder.AddQuad(Vector2(Size.X, Size.Y), CornerSize, CornerMin, CornerMax, _BackColor);

			Builder.AddQuad(Vector2(HalfLocation.X - HalfCorner.X, -CornerSize.Y), CornerSize, CornerMin, CornerMax, _BackColor);
			Builder.AddQuad(Vector2(HalfLocation.X - HalfCorner.X, Size.Y), CornerSize, CornerMin, CornerMax, _BackColor);
			Builder.AddQuad(Vector2(-CornerSize.X, HalfLocation.Y - HalfCorner.Y), CornerSize, CornerMin, CornerMax, _BackColor);
			Builder.AddQuad(Vector2(Size.X, HalfLocation.Y - HalfCorner.Y), CornerSize, CornerMin, CornerMax, _BackColor);

		#pragma endregion

			// Create buffer
			if(MeshBuffer != 0)
				_Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		EventHandler* CSelectionSize::Click()
		{
			return ClickEvent;
		}

		EventHandler* CSelectionSize::MouseDown()
		{
			return DownEvent;
		}

		void CSelectionSize::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CSelectionSize::OnTransform()
		{
			IControl::OnTransform();
		}

		void CSelectionSize::OnTextChange()
		{
			IControl::OnTextChange();
		}

		void CSelectionSize::OnEnabledChange()
		{
			IControl::OnEnabledChange();
			RebuildMesh();
		}

		void CSelectionSize::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		void CSelectionSize::OnForeColorChange()
		{
		}

		void CSelectionSize::OnSkinChange()
		{
			IControl::OnSkinChange();
			this->RebuildMesh();
		}


	}
}
