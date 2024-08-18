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
#include "CPictureBox.h"
#include <SGUIMeshBuilder.h>

namespace NGin
{
	namespace GUI
	{
		Type IPictureBox::TypeOf()
		{
			return Type("CPIC", "CPictureBox GUI PictureBox");
		}

		CPictureBox::CPictureBox(NGin::Math::Vector2& ScreenScale, IGUIManager* Manager) : IPictureBox(ScreenScale, Manager)
		{
			_Type = Type("CPIC", "CPictureBox GUI PictureBox");
			MeshBuffer = 0;
			Manager = 0;
			Texture = 0;
			MinTex = Math::Vector2(0, 0);
			MaxTex = Math::Vector2(1, 1);
			MouseCursor = false;
			UseScissor = false;
		}

		CPictureBox::~CPictureBox()
		{
			if(Texture != 0)
				Manager->GetRenderer()->FreeTexture(Texture);
			Texture = 0;

			if(MeshBuffer != 0)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = 0;
		}

		void CPictureBox::OnDeviceLost()
		{
			IControl::OnDeviceLost();
		}

		void CPictureBox::OnDeviceReset()
		{
			IControl::OnDeviceReset();
		}

		bool CPictureBox::Update(GUIUpdateParameters* Parameters)
		{
			if(_Visible && Parameters->MousePosition > _GlobalLocation && Parameters->MousePosition < (_GlobalLocation + _Size) && Parameters->Handled == false)
			{
				if(!MouseCursor)
					Parameters->MouseBusy = true;

				if(Parameters->MouseThumb == 0)
					Parameters->MouseThumb = this;

				if(Parameters->LeftDown)
					Manager->ControlFocus(this);
			}

			if(_Visible)
			{
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
			}

			return false;
		}

		bool CPictureBox::SetImage(std::string ImageFile)
		{
			return this->SetImageInternal(ImageFile, 0, false);
		}

		bool CPictureBox::SetImage(std::string ImageFile, unsigned int Mask)
		{
			return this->SetImageInternal(ImageFile, Mask, true);
		}

		bool CPictureBox::SetImageInternal(std::string& ImageFile, unsigned int Mask, bool UseMask)
		{
			printf("CPictureBox::SetImageInternal(%s) %s\n", ImageFile.c_str(), Name.c_str());
			// Free old texture
			if(Texture != 0)
				Manager->GetRenderer()->FreeTexture(Texture);

			Texture = Manager->GetRenderer()->GetTexture(ImageFile.c_str(), Mask, UseMask);

			if(Texture == 0)
				return false;

			this->RebuildMesh();
			return true;
		}

		void CPictureBox::SetImage(IGUITexture* texture)
		{
			if(Texture != 0)
				Manager->GetRenderer()->FreeTexture(Texture);

			Texture = Manager->GetRenderer()->GetTexture(texture);
		}

		void CPictureBox::SetImage(void* iTexture)
		{
			if(iTexture == 0)
				return;

			if(Texture != 0)
				Manager->GetRenderer()->FreeTexture(Texture);
			
			Texture = Manager->GetRenderer()->CreateTextureFromBase(iTexture, Vector2(0, 0));
		}

		void CPictureBox::Render()
		{
			if(!_Visible)
				return;
			if(MeshBuffer == 0)
				return;

			// Set us up for rendering
			if(Texture != 0)
				Manager->SetTexture(Texture);
			else
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

			if(UseScissor)
				WinSize = ScissorRegion;

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

		bool CPictureBox::Initialize(CGUIManager* Manager)
		{
			this->Manager = Manager;

			return true;
		}

		void CPictureBox::RebuildMesh()
		{
			if(_Locked)
				return;

			SGUIMeshBuilder Builder(Manager->GetRenderer());

			Builder.AddQuad(Vector2(0, 0), _Size, MinTex, MaxTex - MinTex, _BackColor);

			if(MeshBuffer != NULL)
				Manager->GetRenderer()->FreeMeshBuffer(MeshBuffer);
			MeshBuffer = Builder.Build();
		}

		void CPictureBox::OnSizeChange()
		{
			IControl::OnSizeChange();
			this->RebuildMesh();
		}

		void CPictureBox::OnBackColorChange()
		{
			IControl::OnBackColorChange();
			this->RebuildMesh();
		}

		Math::Vector2 CPictureBox::zzGet_MinTexCoord()
		{
			return MinTex;
		}

		Math::Vector2 CPictureBox::zzGet_MaxTexCoord()
		{
			return MaxTex;
		}

		void CPictureBox::zzPut_MinTexCoord(Math::Vector2 Coord)
		{
			MinTex = Coord;
			RebuildMesh();
		}

		void CPictureBox::zzPut_MaxTexCoord(Math::Vector2 Coord)
		{
			MaxTex = Coord;
			RebuildMesh();
		}

		void CPictureBox::zzPut_IsMouseCursor( bool isMouseCursor )
		{
			MouseCursor = isMouseCursor;
		}

		bool CPictureBox::zzGet_IsMouseCursor()
		{
			return MouseCursor;
		}

		void CPictureBox::zzPut_ForceScissoring(bool forceScissoring)
		{
			UseScissor = forceScissoring;
		}

		bool CPictureBox::zzGet_ForceScissoring()
		{
			return UseScissor;
		}

		void CPictureBox::zzPut_ScissorWindow(Math::Vector4 &scissorWindow)
		{
			ScissorRegion = scissorWindow;
		}

		Math::Vector4 CPictureBox::zzGet_ScissorWindow()
		{
			return ScissorRegion;
		}
	}
}
