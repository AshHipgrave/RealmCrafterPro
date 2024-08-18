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
#include "CGUIManager.h"
#include "CWindow.h"
#include "CPictureBox.h"
#include "CRadar.h"
#include "CButton.h"
#include "CCheckBox.h"
#include "CScrollBar.h"
#include "CProgressBar.h"
#include "CTrackBar.h"
#include "CListBox.h"
#include "CComboBox.h"
#include "CTextBox.h"
#include "CLabel.h"
#include "CFont.h"
#include "CPropertySet.h"
#include <XMLWrapper.h>

#include <ft2build.h>
#include <shlobj.h>
#include "CTabControl.h"
#include FT_FREETYPE_H

FT_Library FTLibrary;
bool FTStarted = false;

#ifdef LoadCursor
#undef LoadCursor
#endif

#ifdef CreateWindow
#undef CreateWindow
#endif

namespace NGin
{
	void GlobalFail(std::string Message)
	{
		MessageBox(0, Message.c_str(), "Global Fail", MB_ICONERROR);

		// Assert didn't work here, just cause a pointer error
		#if defined(_DEBUG)
			int* p = 0;
			*p = 0;
		#endif

		// Kill the app in release
		exit(0);
	}

	namespace GUI
	{

		CGUIManager::CGUIManager(NGin::Math::Vector2& ScreenScale) : IGUIManager(ScreenScale, 0),
			Renderer(0)
		{
			_Manager = this;
			FontDir = ".\\";
			_ControlFocus = 0;
			Lbl = 0;
			UsingPerfHUD = false;
			LeftBusyLastFrame = false;
			RightBusyLastFrame = false;
			DeleteLock = false;
		}

		CGUIManager::~CGUIManager()
		{
			
			for(int i = 0; i < this->Skins.Size(); ++i)
				delete this->Skins[i];
			for(int i = 0; i < this->Fonts.Size(); ++i)
				delete this->Fonts[i];
			for(int i = 0; i < this->Cursors.Size(); ++i)
			{
				Renderer->FreeTexture(Cursors[i]->Texture);
				delete Cursors[i];
			}

			// Reorder controls
			List<IControl*> Controls;
			foreachc(CIt, IControl, _Controls)
			{
				Controls.Insert((*CIt), 0);

				nextc(CIt, IControl, _Controls);
			}

			DeleteLock = true;
			foreachf(cCIt, IControl, Controls)
			{

				delete (*cCIt);

				nextf(cCIt, IControl, Controls);
			}
			DeleteLock = false;

			if(FTStarted)
			{
				FT_Done_FreeType(FTLibrary);
				FTStarted = false;
			}
		}

		void CGUIManager::OnDeviceLost()
		{
			Renderer->OnDeviceLost();

			IControl::OnDeviceLost();
		}

		void CGUIManager::OnDeviceReset(Vector2& NewResolution)
		{
			Renderer->OnDeviceReset();

			Vector2 OldResolution = Resolution;
			Resolution = NewResolution;

			for(int i = 0; i < this->Skins.Size(); ++i)
				this->Skins[i]->OnDeviceReset(OldResolution, NewResolution);
			for(int i = 0; i < this->Fonts.Size(); ++i)
				((CFont*)this->Fonts[i])->OnDeviceReset(OldResolution, NewResolution);

			if(this->Skins.Size() >= 1)
				this->SetCursor(Skins[0]->GetCursor(GUICursor_Pointer));

			IControl::OnDeviceReset();
		}

		bool CGUIManager::GetDeleteLock()
		{
			return DeleteLock;
		}

		void CGUIManager::BringToFront(IControl* Control)
		{
			FrontQueue.Add(Control);
		}

		void CGUIManager::SendToBack(IControl* Control)
		{
			BackQueue.Add(Control);
		}

		IControl* CGUIManager::GetSelf()
		{
			return this;
		}

		void CGUIManager::Destroy(IControl* Control)
		{
			DeleteQueue.Add(Control);
		}

		bool CGUIManager::Update(GUIUpdateParameters* Parameters)
		{
			Vector2 OMousePos = Parameters->MousePosition;
			Parameters->MousePosition /= Resolution;

			bool SetTo = false;
			if(Parameters->LeftDown && LeftBusyLastFrame)
				SetTo = true;
			if(Parameters->RightDown && RightBusyLastFrame)
				SetTo = true;
			

			Parameters->ModalProc = false;
			Parameters->Handled = false;
			Parameters->MouseOver = false;
			Parameters->MouseBusy = SetTo;
			Parameters->MouseThumb = 0;

			

			//if(Lbl == 0)
			//	Lbl = this->CreateLabel("Hello", Vector2(0, 0), Vector2(0, 0));

			

			//for(int i = 0; i < Parameters->InputBuffer->Size(); ++i)
			//{
			//	int K = Parameters->InputBuffer[0][i];
			//	
			//	char L[2] = {0, 0};

			//	if(K > 31 && K < 255 && K != 127)
			//	{
			//		L[0] = (char)K;
			//		Lbl->Text(Lbl->Text() + String(L));
			//	}else
			//	{
			//		switch(K)
			//		{
			//		case 8: // BackSpace
			//			{
			//				String LT = Lbl->Text();
			//				if(LT.Length() > 0)
			//				{
			//					LT = LT.Substr(0, LT.Length() - 1);
			//					Lbl->Text(LT);
			//				}


			//				break;
			//			}
			//		case 13: // Return

			//			break;
			//		case 9: // Tab

			//			break;
			//		default:
			//			{
			//				char OO[1024];
			//				sprintf(OO, "%i\n", K);
			//				OutputDebugString(OO);
			//			}
			//			break;
			//		}
			//	}

			//}

			// Delete all queued objects
			DeleteLock = true;
			foreachf(dIT, IControl, DeleteQueue)
			{
				FrontQueue.Remove(*dIT);
				BackQueue.Remove(*dIT);


				delete *dIT;

				nextf(dIT, IControl, DeleteQueue);
			}
			DeleteLock = false;
			DeleteQueue.Clear();

			foreachf(fIT, IControl, FrontQueue)
			{
				IControl* C = (*fIT);
				if(C->Parent != 0)
				{
					C->Parent->Controls()->Remove(C);
					C->Parent->Controls()->Add(C);
				}
				nextf(fIT, IControl, FrontQueue);
			}


			foreachf(bIT, IControl, BackQueue)
			{
				IControl* C = (*bIT);
				if(C->Parent != 0)
				{
					C->Parent->Controls()->Remove(C);
					C->Parent->Controls()->Add(C);
				}
				nextf(bIT, IControl, BackQueue);
			}
			FrontQueue.Clear();
			BackQueue.Clear();


			// Reorder controls
			//List<IControl*> Controls;
			std::vector<IControl*> Controls;
			foreachc(CIt, IControl, _Controls)
			{
				
			
				//Controls.Insert((*CIt), 0);
				Controls.push_back(*CIt);

				nextc(CIt, IControl, _Controls);
			}

			//foreachf(cCIt, IControl, Controls)
			//{

			//	(*cCIt)->Update(Parameters);
			//	if(Parameters->ModalProc)
			//		break;

			//	nextf(cCIt, IControl, Controls);
			//}
			if(Controls.size() > 0)
			{
				for(int i = Controls.size() - 1; i >= 0; --i)
				{
					Controls[i]->Update(Parameters);
					if(Parameters->ModalProc)
						break;
				}
			}

			if(Parameters->InputBuffer != 0)
				Parameters->InputBuffer->clear();

			// Update cursor
			CursorImage->Location = Parameters->MousePosition;
			CursorImage->BringToFront();

			// Reset
			Parameters->MousePosition = OMousePos;

			// Delete all queued objects
			DeleteLock = true;
			foreachf(ddIT, IControl, DeleteQueue)
			{
				FrontQueue.Remove(*dIT);
				BackQueue.Remove(*dIT);

				delete *ddIT;

				nextf(ddIT, IControl, DeleteQueue);
			}
			DeleteLock = false;
			DeleteQueue.Clear();

			foreachf(ffIT, IControl, FrontQueue)
			{
				IControl* C = (*ffIT);
				if(C->Parent != 0)
				{
					C->Parent->Controls()->Remove(C);
					C->Parent->Controls()->Add(C);
				}
				nextf(ffIT, IControl, FrontQueue);
			}


			foreachf(bbIT, IControl, BackQueue)
			{
				IControl* C = (*bbIT);
				if(C->Parent != 0)
				{
					C->Parent->Controls()->Remove(C);
					C->Parent->Controls()->Add(C);
				}
				nextf(bbIT, IControl, BackQueue);
			}
			FrontQueue.Clear();
			BackQueue.Clear();

			if(Parameters->MouseBusy)
			{
				LeftBusyLastFrame = Parameters->LeftDown;
				RightBusyLastFrame = Parameters->RightDown;
			}else
			{
				LeftBusyLastFrame = RightBusyLastFrame = false;
			}

			return true;
		}

		void CGUIManager::Render()
		{
			// Local values
			CurrentSkin = 0;

			// Start Render
			Renderer->PreRender();

			// Screen Scale
			Vector2 OneOverRes = (Vector2(1.0f, 1.0f) / this->Resolution) * Vector2(0.5f, 0.5f);
			TParameter<Vector2> ScaleOffset("ScaleOffset");
			ScaleOffset.SetData(OneOverRes);
			Renderer->SetParameter("ScaleOffset", ScaleOffset);

			Math::Vector4 TempScissor;
			TempScissor.Z = Resolution.X;
			TempScissor.W = Resolution.Y;
			Renderer->PushScissorRect(TempScissor);

			// Render every control
			foreachc(CIt, IControl, _Controls)
			{
				(*CIt)->Render();

				nextc(CIt, IControl, _Controls);
			}

			Renderer->PopScissorRect();

			// End render
			Renderer->PostRender();
		}

		void CGUIManager::SetSkin(int Index)
		{
			if(Index == -1)
			{
				CurrentSkin = -1;
				return;
			}

			if(Index != CurrentSkin)
			{
				CSkin* S = (CSkin*)GetSkin(Index);
				
				Renderer->SetTexture("TextureStage0", S->Texture());
			}
		}

		void CGUIManager::SetTexture(IGUITexture* Texture)
		{
			Renderer->SetTexture("TextureStage0", Texture);
		}

		void CGUIManager::SetPosition(Vector2& Position)
		{
			Vector2 NewPos = Position;
			if(NewPos.X < -1.0f)
				NewPos.X += 2.0f;
			if(NewPos.Y < -1.0f)
				NewPos.Y -= 2.0f;

			TParameter<Vector2> GUIPosition("GUIPosition");
			GUIPosition.SetData(NewPos);
			Renderer->SetParameter("GUIPosition", GUIPosition);
		}

		void CGUIManager::ControlFocus(IControl* Control)
		{
			//Lbl->Text(Control->GetType().Name() + ": " + Control->Name());
			this->_ControlFocus = Control;
		}

		IControl* CGUIManager::ControlFocus()
		{
			return this->_ControlFocus;
		}

		IWindow* CGUIManager::CreateWindow(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a window!
			CWindow* Window = new CWindow(_ScreenScale, this);
			Window->Initialize(this);
			Window->Locked = true;
			Window->Name = Name;
			Window->Text = Name;
			Window->Location = Location;
			Window->Size = Size;

			Window->Locked = false;

			if(this->Skins.Size() > 0)
			{
				Window->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(Window, "Window");
			}

			Window->Parent = this;

			return Window;
		}

		IPictureBox* CGUIManager::CreatePictureBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a picturebox!
			CPictureBox* PictureBox = new CPictureBox(_ScreenScale, this);
			PictureBox->Initialize(this);
			PictureBox->Locked = true;
			PictureBox->Name =Name;
			PictureBox->Text = Name;
			PictureBox->Location = Location;

			PictureBox->Locked = false;

			PictureBox->Size = Size;

			if(this->Skins.Size() > 0)
			{
				PictureBox->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(PictureBox, "PictureBox");
			}

			PictureBox->Parent = this;

			return PictureBox;
		}

		ITabControl* CGUIManager::CreateTabControl(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a tabcontrol!
			CTabControl* TabControl = new CTabControl(_ScreenScale, this);
			TabControl->Initialize();
			TabControl->Locked = true;
			TabControl->Name = Name;
			TabControl->Text = Name;
			TabControl->Location = Location;

			TabControl->Locked = false;

			if(this->Skins.Size() > 0)
			{
				TabControl->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(TabControl, "TabControl");
			}

			TabControl->Size = Size;

			TabControl->Parent = this;

			return TabControl;
		}


		IRadar* CGUIManager::CreateRadar( std::string Name, NGin::Math::Vector2& Location, NGin::Math::Vector2& Size )
		{
			// Create a Radar!
			CRadar* Radar = new CRadar(_ScreenScale, this);
			Radar->Initialize(this);
			Radar->Locked = true;
			Radar->Name =Name;
			Radar->Text = Name;
			Radar->Location = Location;
			Radar->Size = Size;

			Radar->Locked = false;

			if(this->Skins.Size() > 0)
			{
				Radar->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(Radar, "Radar");
			}

			Radar->Parent = this;

			return Radar;
		}

		IButton* CGUIManager::CreateButton(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a Button!
			CButton* Button = new CButton(_ScreenScale, this);
			Button->Initialize(this);
			Button->Locked = true;
			Button->Name = Name;
			Button->Text = Name;
			Button->Location = Location;
			Button->Size = Size;

			Button->Locked = false;

			if(this->Skins.Size() > 0)
			{
				Button->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(Button, "Button");
			}

			Button->Parent = this;

			return Button;
		}

		ICheckBox* CGUIManager::CreateCheckBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a checkbox!
			CCheckBox* CheckBox = new CCheckBox(_ScreenScale, this);
			CheckBox->Initialize(this);
			CheckBox->Locked = true;
			CheckBox->Name = Name;
			CheckBox->Text = Name;
			CheckBox->Location = Location;
			CheckBox->Size = Size;

			CheckBox->Locked = false;

			if(this->Skins.Size() > 0)
			{
				CheckBox->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(CheckBox, "CheckBox");
			}

			CheckBox->Parent = this;

			return CheckBox;
		}

		IProgressBar* CGUIManager::CreateProgressBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a ProgressBar!
			CProgressBar* ProgressBar = new CProgressBar(_ScreenScale, this);
			ProgressBar->Initialize(this);
			ProgressBar->Locked = true;
			ProgressBar->Name = Name;
			ProgressBar->Text = Name;
			ProgressBar->Location = Location;
			ProgressBar->Size = Size;

			ProgressBar->Locked = false;

			if(this->Skins.Size() > 0)
			{
				ProgressBar->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(ProgressBar, "ProgressBar");
			}

			ProgressBar->Parent = this;

			return ProgressBar;
		}

		ITrackBar* CGUIManager::CreateTrackBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a TrackBar!
			CTrackBar* TrackBar = new CTrackBar(_ScreenScale, this);
			TrackBar->Initialize(this);
			TrackBar->Locked = true;
			TrackBar->Name = Name;
			TrackBar->Text = Name;
			TrackBar->Location = Location;
			TrackBar->Size = Size;

			TrackBar->Locked = false;

			if(this->Skins.Size() > 0)
			{
				TrackBar->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(TrackBar, "TrackBar");
			}

			TrackBar->Parent = this;

			return TrackBar;
		}

		IListBox* CGUIManager::CreateListBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a ListBox!
			CListBox* ListBox = new CListBox(_ScreenScale, this);
			ListBox->Initialize(this);
			ListBox->Locked = true;
			ListBox->Name = Name;
			ListBox->Text = Name;
			ListBox->Location = Location;

			ListBox->Locked = false;

			if(this->Skins.Size() > 0)
			{
				ListBox->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(ListBox, "ListBox");
			}

			ListBox->Size = Size;
			ListBox->Parent = this;

			return ListBox;
		}

		IComboBox* CGUIManager::CreateComboBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a ComboBox!
			CComboBox* ComboBox = new CComboBox(_ScreenScale, this);
			ComboBox->Initialize(this);
			ComboBox->Locked = true;
			ComboBox->Name = Name;
			ComboBox->Text = Name;
			ComboBox->Location = Location;

			ComboBox->Locked = false;

			if(this->Skins.Size() > 0)
			{
				ComboBox->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(ComboBox, "ComboBox");
			}

			ComboBox->Size = Size;
			ComboBox->Parent = this;

			return ComboBox;
		}

		ITextBox* CGUIManager::CreateTextBox(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a TextBox!
			CTextBox* TextBox = new CTextBox(_ScreenScale, this);
			TextBox->Initialize(this);
			TextBox->Locked = true;
			TextBox->Name = Name;
			TextBox->Text = "";
			TextBox->Location = Location;

			TextBox->Locked = false;

			if(this->Skins.Size() > 0)
			{
				TextBox->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(TextBox, "TextBox");
			}

			TextBox->Size = Size;
			TextBox->Parent = this;

			return TextBox;
		}

		IScrollBar* CGUIManager::CreateScrollBar(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size, ScrollOrientation ScrollOrientation)
		{
			// Create a ScrollBar!
			CScrollBar* ScrollBar = new CScrollBar(_ScreenScale, ScrollOrientation, this);
			ScrollBar->Initialize(this);
			ScrollBar->Locked = true;
			ScrollBar->Name = Name;
			ScrollBar->Text = Name;
			ScrollBar->Location = Location;
			ScrollBar->Size = Size;

			ScrollBar->Locked = false;

			if(this->Skins.Size() > 0)
			{
				ScrollBar->Skin = 1;
				this->Skins[0]->ApplyDefaultProperty(ScrollBar, "ScrollBar");
			}

			ScrollBar->Parent = this;

			return ScrollBar;
		}

		ILabel* CGUIManager::CreateLabel(std::string Name,  NGin::Math::Vector2& Location,  NGin::Math::Vector2& Size)
		{
			// Create a Label!
			CLabel* Label = new CLabel(_ScreenScale, this);
			Label->Initialize(this);
			Label->Locked = true;
			Label->Name = Name;
			Label->Text = Name;
			Label->Location = Location;
			Label->Size = Size;

			Label->Locked = false;

			if(this->Skins.Size() > 0)
			{
				Label->Skin = 1;
				Label->Font = this->GetSkin(1)->GetFont(GUIFont_Control);
				this->Skins[0]->ApplyDefaultProperty(Label, "Label");
			}

			Label->Parent = this;

			return Label;
		}


		IGUIRenderer* CGUIManager::GetRenderer()
		{
			return Renderer;
		}

		NGin::Math::Vector2 CGUIManager::GetResolution()
		{
			return Resolution;
		}

		void CGUIManager::FontDirectory(std::string FontPath)
		{
			if(FontPath.substr(FontPath.length() - 1).compare("\\") != 0)
				FontPath += std::string("\\");
			FontDir = FontPath;
		}

		std::string CGUIManager::FontDirectory()
		{
			return FontDir;
		}

		bool CGUIManager::Initialize(IGUIRenderer* renderer, NGin::Math::Vector2 &ScreenRes)
		{
			// Basic setup
			this->Renderer = renderer;
			this->Resolution = ScreenRes;
			this->Scale = NGin::Math::Vector2(1.0f, 1.0f) / this->Resolution;
			CurrentSkin = 0;

			// Set the cursor
			CursorImage = this->CreatePictureBox(std::string("CursorBox"), Vector2(0, 0), Vector2(0, 0));
			CursorImage->IsMouseCursor = true;

			// Fine
			return true;
		}

		int CGUIManager::LoadAndAddSkin(std::string SkinFile)
		{
			// Create an initialize skin
			CSkin* Skin = new CSkin(this);
			if(Skin->Initialize(SkinFile, this->Resolution) == false)
			{
				delete Skin;
				return 0;
			}

			// Add to list
			this->Skins.Add(Skin);

			// First
			if(this->Skins.Size() == 1)
				this->SetCursor(Skins[0]->GetCursor(GUICursor_Pointer));

			// Done
			return this->Skins.Size();
		}

		ISkin* CGUIManager::GetSkin(int Index)
		{
			--Index;
			if(Index < 0 || Index >= this->Skins.Size())
				return 0;

			return this->Skins[Index];
		}

		void CGUIManager::SetCursor(int Cursor)
		{
			if(Cursor < 0 || Cursor > Cursors.Size())
				return;

			CursorImage->SetImage(Cursors[Cursor]->Texture);
			CursorImage->Size = Cursors[Cursor]->Size / this->Resolution;
		}

		int CGUIManager::LoadCursor(std::string Path)
		{
			std::transform(Path.begin(), Path.end(), Path.begin(), ::tolower);

			for(int i = 0; i < Cursors.Size(); ++i)
				if(Cursors[i]->Name.compare(Path) == 0)
					return i;

			IntCursor* I = new IntCursor();
			I->Name = Path;
			I->Texture = Renderer->GetTexture(Path.c_str(), 0xffff00ff, true);

			if(I->Texture != NULL)
				I->Size = I->Texture->GetSize();


			Cursors.Add(I);
			return Cursors.Size() - 1;

		}

		inline void Scanline_GlyphToTexture(char* textureBits, int texturePitch, FT_Bitmap* bitmap, int sx, int sy, int texWidth, int texHeight)
		{
			if(sy < 0)
				sy = 0;

			if(bitmap->width == 0 || bitmap->rows == 0)
				return;


			if(bitmap->pitch < 0)
				GlobalFail("Bitmap Pitch is backwards!");

			for(int y = 0; y < bitmap->rows; ++y)
			{
				for(int x = 0; x < bitmap->width; ++x)
				{

					if(sy + y > texHeight || sx + x > texWidth)
						continue;

					char* RowStart = (textureBits + ((sy + y) * texturePitch)) + ((sx + x) * 4);
					unsigned char* BufferStart = (bitmap->buffer + (y * bitmap->pitch)) + x;
					
					DWORD* TexVal = (DWORD*)RowStart;

					unsigned char Alpha = BufferStart[0];
// 					if(Alpha > 10)
// 						Alpha = 255;
// 					else
// 						Alpha = 0;

					TexVal[0] = ((((Alpha)&0xff)<<24)|(((BufferStart[0])&0xff)<<16)|(((BufferStart[0])&0xff)<<8)|((BufferStart[0])&0xff));
				}
			}
		}

		IFont* CGUIManager::LoadFont(std::string FontFile, int fontSize)
		{
			std::transform(FontFile.begin(), FontFile.end(), FontFile.begin(), ::tolower);

			if(FontFile.length() > 4 && FontFile.substr(FontFile.length() - 4) == ".xml")
				FontFile = FontFile.substr(0, FontFile.length() - 4);

			for(int i = 0; i < Fonts.Size(); ++i)
				if(FontFile.compare(Fonts[i]->GetName()) == 0)
					return Fonts[i];


			if(FTStarted == false)
			{
				if(FT_Init_FreeType(&FTLibrary))
				{
					GlobalFail("Critical Error thrown from FT_Init_FreeType (FreeType2) in NGUI::CGUIManager::LoadFont()");
				}

				FTStarted = true;
			}

			FT_Face Face;
			int Error = 0;

			Error = FT_New_Face( FTLibrary, (FontDir + FontFile).c_str(), 0, &Face);

			if(Error == FT_Err_Unknown_File_Format || Error > 0)
			{
				char WinFontsC[4096];
				if(SHGetSpecialFolderPath(0, WinFontsC, CSIDL_FONTS, FALSE))
				{
					std::string WinFonts = (char*)WinFontsC;

					Error = FT_New_Face( FTLibrary, (WinFonts + "\\" + FontFile).c_str(), 0, &Face);
				}else
				{
					Error = FT_Err_Unknown_File_Format;
				}

				if(Error == FT_Err_Unknown_File_Format)
				{
					std::string ErrStr = std::string("Error: Could not read font file '") + FontFile + "'!";
					MessageBox(0, ErrStr.c_str(), "NGUI Error", MB_OK);
					return 0;
				}else if(Error)
				{
					std::string ErrStr = std::string("Unknown error in font file '") + FontFile + "'!";
					MessageBox(0, ErrStr.c_str(), "NGUI Error", MB_OK);
					return 0;
				}
			}

			int PixelSize = fontSize;
			FT_Set_Pixel_Sizes(Face, 0, PixelSize);

			int x = 1;
			int y = 1;
			int PixelSizeMax = PixelSize;

			for(int i = 0; i < 256; ++i)
			{
				unsigned int GlyphIndex = FT_Get_Char_Index(Face, i);

				if(FT_Load_Glyph(Face, GlyphIndex, 0))
					continue;

				if(FT_Render_Glyph(Face->glyph, FT_RENDER_MODE_NORMAL))
					continue;

				int Height = ((PixelSize - Face->glyph->bitmap_top) + Face->glyph->bitmap.rows);
				if(Height < PixelSize)
					Height = PixelSize;

				int Width = (Face->glyph->advance.x >> 6) - Face->glyph->bitmap_left;

				if(Height > PixelSizeMax)
					PixelSizeMax = Height;
			}

			int RequiredWidth = (PixelSize + 2) * 16;
			bool HasSize = false;
			for(int i = 4; i < 12; ++i)
			{
				if(RequiredWidth < (int)pow(2.0f, i))
				{
					RequiredWidth = (int)pow(2.0f, i);
					HasSize = true;
					break;
				}
			}

			// Too wide!
			if(!HasSize)
			{
				std::string ErrStr = std::string("Error with font '") + FontFile + "', size is too large!";
				MessageBox(0, ErrStr.c_str(), "NGUI Error", MB_OK);
				return 0;
			}

			for(int i = 0; i < 256; ++i)
			{
				unsigned int GlyphIndex = FT_Get_Char_Index(Face, i);

				if(FT_Load_Glyph(Face, GlyphIndex, 0))
					continue;

				if(FT_Render_Glyph(Face->glyph, FT_RENDER_MODE_NORMAL))
					continue;

				int Height = ((PixelSize - Face->glyph->bitmap_top) + Face->glyph->bitmap.rows);
				if(Height < PixelSize)
					Height = PixelSize;

				int Width = (Face->glyph->advance.x >> 6) - Face->glyph->bitmap_left;

				if(x + Width >= RequiredWidth)
				{
					x = 1;
					y += PixelSizeMax;
				}

				x += Width;
				x += 2;
			}

			int RequiredHeight = y + PixelSizeMax + 16;
			HasSize = false;
			for(int i = 4; i < 12; ++i)
			{
				if(RequiredHeight < (int)pow(2.0f, i))
				{
					RequiredHeight = (int)pow(2.0f, i);
					HasSize = true;
					break;
				}
			}

			// Font is too big!
			if(!HasSize)
			{
				std::string ErrStr = std::string("Error with font '") + FontFile + "', size is too large!";
				MessageBox(0, ErrStr.c_str(), "NGUI Error", MB_OK);
				return 0;
			}

			// New font
			CFont* Font = new CFont(this, FontFile);

			DWORD* Data = new DWORD[RequiredWidth * RequiredHeight];
			memset(Data, 0, sizeof(DWORD) * RequiredWidth * RequiredHeight);

			x = 1;
			y = 1;

			for(int i = 0; i < 256; ++i)
			{
				unsigned int GlyphIndex = FT_Get_Char_Index(Face, i);

				if(FT_Load_Glyph(Face, GlyphIndex, 0))
					continue;

				if(FT_Render_Glyph(Face->glyph, FT_RENDER_MODE_NORMAL))
						continue;

				int Height = ((PixelSize - Face->glyph->bitmap_top) + Face->glyph->bitmap.rows);
				if(Height < PixelSize)
					Height = PixelSize;

				int Width = (Face->glyph->advance.x >> 6) - Face->glyph->bitmap_left;

				if(x + Width >= RequiredWidth)
				{
					x = 1;
					y += PixelSizeMax;

					if(y > RequiredHeight)
					{
						std::string ErrStr = std::string("Serious Error loading: '") + FontFile + "', Out of room on atlas: Can't fit all glyphs onto texture, bail!";
						MessageBox(0, ErrStr.c_str(), "NGUI Error", MB_OK);
						return 0;
					}
				}

				// Draw glyph
				Font->Characters[i]->TexPos = Vector2(x/* + Face->glyph->bitmap_left*/, y);
				Font->Characters[i]->TexSize = Vector2(Width, Height);
				Font->Characters[i]->ScreenSize = Font->Characters[i]->TexSize;
				Font->Characters[i]->TexPos /= Vector2(RequiredWidth, RequiredHeight);
				Font->Characters[i]->TexSize /= Vector2(RequiredWidth, RequiredHeight);
				Font->Characters[i]->ScreenSize /= Resolution;


				Scanline_GlyphToTexture(
					//(char*)LockedRect.pBits, LockedRect.Pitch,
					(char*)Data, RequiredWidth * 4,
					&(Face->glyph->bitmap),
					x + Face->glyph->bitmap_left, y + (PixelSize - Face->glyph->bitmap_top),
					RequiredWidth, RequiredHeight);


				x += Width;
				x += 2;
			}

			Font->FontTexture = Renderer->CreateFontTexture(RequiredWidth, RequiredHeight, (const char*)Data);

			FT_Done_Face(Face);

			Fonts.Add(Font);
			return Font;

// 			FontFile = FontDir + FontFile;
// 
// 			// Read XML
// 			XMLReader* X = ReadXMLFile(FontFile);
// 			if(X == 0)
// 			{
// 				delete Font;
// 				return false;
// 			}
// 
// 			size_t SlashPos = 0;
// 			size_t L = 0;
// 
// 			while(true)
// 			{
// 				L = FontFile.find("\\", SlashPos + 1);
// 				if(L != -1)
// 					SlashPos = L;
// 				else
// 					break;
// 			}
// 
// 			std::string Path = FontFile.substr(0, SlashPos) + "\\";
// 
// 			Vector2 TextureSize;
// 
// 			// Read
// 			while(X->Read())
// 			{
// 				std::string NodeName = X->GetNodeName();
// 				std::transform(NodeName.begin(), NodeName.end(), NodeName.begin(), ::tolower);
// 
// 				// If we reach the end of the skin, bail.
// 				if(X->GetNodeType() == XNT_Element_End && NodeName.compare("font") == 0)
// 					break;
// 
// 				// It's the opening tag of the skin
// 				if(X->GetNodeType() == XNT_Element && NodeName.compare("font") == 0)
// 				{
// 					// Get the path
// 					std::string TexPath = X->GetAttributeString("texture");
// 					std::string TexName = Path + TexPath;
// 					if(TexName.length() == 0)
// 					{
// 						OutputDebugString((std::string("No texture attribute found in font node: ") + FontFile + "\n").c_str());
// 						delete X;
// 						delete Font;
// 						return false;
// 					}
// 
// 					// Load texture
// 					if(D3DXCreateTextureFromFileEx(Device, TexName.c_str(), D3DX_DEFAULT, D3DX_DEFAULT, 1, NULL, D3DFMT_UNKNOWN, D3DPOOL_MANAGED, D3DX_DEFAULT, D3DX_DEFAULT, /*D3DCOLOR_ARGB(255,255,0,0)*/ 0xff000000, NULL, NULL, &(Font->FontTexture)) != D3D_OK)
// 					{
// 						OutputDebugString((std::string("Could not font texture: ") + TexName + "\n").c_str());
// 						delete X;
// 						delete Font;
// 						return false;
// 					}
// 
// 					// Get texture size
// 					D3DSURFACE_DESC SurfaceData;
// 					Font->FontTexture->GetLevelDesc(0, &SurfaceData);
// 
// 					TextureSize.X = SurfaceData.Width;
// 					TextureSize.Y = SurfaceData.Height;
// 				}
// 
// 				// It's an onscreen component
// 				if(X->GetNodeType() == XNT_Element && NodeName.compare("character") == 0)
// 				{
// 					// Get info
// 					std::string sCode = X->GetAttributeString("code");
// 					std::transform(sCode.begin(), sCode.end(), sCode.begin(), ::tolower);
// 					std::string sLocation = X->GetAttributeString("location");
// 					std::string sSize = X->GetAttributeString("size");
// 
// 					Vector2 vLocation, vSize;
// 					int C = 0;
// 					std::stringstream(sCode) >> C;
// 
// 					// Get Location
// 					if(sLocation.length() > 0)
// 					{
// 						std::string L, R;
// 						L = sLocation.substr(0, sLocation.find(",", 0));
// 						R = sLocation.substr(sLocation.find(",", 0) + 1);
// 
// 						// Trim values using a bit of work (grr STL)
// 						size_t TrimStart = L.find_first_not_of(" \t");
// 						size_t TrimEnd = L.find_last_not_of(" \t");
// 						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
// 							L = "";
// 						else
// 							L = L.substr(TrimStart, TrimEnd - TrimStart + 1);
// 
// 						TrimStart = R.find_first_not_of(" \t");
// 						TrimEnd = R.find_last_not_of(" \t");
// 						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
// 							R = "";
// 						else
// 							R = R.substr(TrimStart, TrimEnd - TrimStart + 1);
// 
// 						std::stringstream LToI(L);
// 						std::stringstream RToI(R);
// 
// 						LToI >> vLocation.X;
// 						RToI >> vLocation.Y;
// 					}
// 
// 					// Get Size
// 					if(sSize.length() > 0)
// 					{
// 						std::string L, R;
// 						L = sSize.substr(0, sSize.find(",", 0));
// 						R = sSize.substr(sSize.find(",", 0) + 1);
// 
// 						// Trim values using a bit of work (grr STL)
// 						size_t TrimStart = L.find_first_not_of(" \t");
// 						size_t TrimEnd = L.find_last_not_of(" \t");
// 						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
// 							L = "";
// 						else
// 							L = L.substr(TrimStart, TrimEnd - TrimStart + 1);
// 
// 						TrimStart = R.find_first_not_of(" \t");
// 						TrimEnd = R.find_last_not_of(" \t");
// 						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
// 							R = "";
// 						else
// 							R = R.substr(TrimStart, TrimEnd - TrimStart + 1);
// 
// 						std::stringstream LToI(L);
// 						std::stringstream RToI(R);
// 
// 						LToI >> vSize.X;
// 						RToI >> vSize.Y;
// 					}
// 
// 					Font->Characters[C]->TexPos = Vector2(vLocation.X, vLocation.Y);
// 					Font->Characters[C]->TexSize = Vector2(vSize.X, vSize.Y);
// 					Font->Characters[C]->ScreenSize = Vector2(vSize.X, vSize.Y);
// 
// 					
// 					Font->Characters[C]->TexPos /= TextureSize;
// 					Font->Characters[C]->TexSize /= TextureSize;
// 					Font->Characters[C]->ScreenSize /= Resolution;
// 
// 				}
// 			}
// 
// 			delete X;
// 
// 			Fonts.Add(Font);
// 			return Font;
		}

		bool CGUIManager::zzGet_CursorVisible()
		{
			return CursorImage->Visible;
		}

		void CGUIManager::zzPut_CursorVisible(bool CursorVisible)
		{
			CursorImage->Visible = CursorVisible;	
		}

		bool CGUIManager::ImportProperties(std::string Name)
		{
			XMLReader* X = ReadXMLFile(Name);
			if(X == 0)
			{
				std::string Fail;
				Fail += "Failed to read properties file: ";
				Fail += Name;
				Fail += "\n";
				OutputDebugString(Fail.c_str());
				return false;
			}

			CPropertySet* ActiveSet = 0;

			// Read file
			while(X->Read())
			{
				std::string NodeName = X->GetNodeName();
				std::transform(NodeName.begin(), NodeName.end(), NodeName.begin(), ::tolower);

				// Read the node
				if(X->GetNodeType() == XNT_Element && NodeName.compare("interface") == 0)
				{
					if(ActiveSet != 0)
					{
						OutputDebugString("Embedded interface element found. Cannot go on!\n");
						delete X;
						return false;
					}

					std::string SetName = X->GetAttributeString("set");
					std::transform(SetName.begin(), SetName.end(), SetName.begin(), ::tolower);

					if(SetName.length() == 0)
						continue;

					bool Found = false;
					for(int i = 0; i < PropertySets.Size(); ++i)
					{
						if(PropertySets[i]->GetName().compare(SetName) == 0)
						{
							ActiveSet = PropertySets[i];
							Found = true;
						}
					}

					if(Found == false)
						ActiveSet = new CPropertySet(std::string(SetName), this);

					if(ActiveSet != 0 || Found == false)
						PropertySets.Add(ActiveSet);
				}

				// End of node
				if(X->GetNodeType() == XNT_Element_End && NodeName.compare("interface") == 0)
				{
					ActiveSet = 0;
				}

				// Component node
				if(X->GetNodeType() == XNT_Element && NodeName.compare("component") == 0)
				{
					if(ActiveSet != 0)
						ActiveSet->Process(X);
				}
			}

			// Delete
			delete X;
			return true;
		}

		void CGUIManager::SetProperties(std::string SetName)
		{
			std::transform(SetName.begin(), SetName.end(), SetName.begin(), ::tolower);

			for(int i = 0; i < PropertySets.Size(); ++i)
				if(PropertySets[i]->GetName().compare(SetName) == 0)
					PropertySets[i]->ApplySet(this, "", 0xffffffffe);
		}

		void CGUIManager::SetSingleProperty(std::string SetName, std::string ControlName)
		{
			this->SetSingleProperty(SetName, ControlName, 0xfffffffe);
		}

		void CGUIManager::SetSingleProperty(std::string SetName, std::string ControlName, int SearchDepth)
		{
			std::transform(SetName.begin(), SetName.end(), SetName.begin(), ::tolower);

			for(int i = 0; i < PropertySets.Size(); ++i)
				if(PropertySets[i]->GetName().compare(SetName) == 0)
					PropertySets[i]->ApplySet(this, ControlName, SearchDepth);
		}

		void CGUIManager::CorrectRect(Math::Vector4 &R)
		{
			if(R.Y < 0)
				R.Y = 0;
			if(R.X < 0)
				R.X = 0;

			int Width = GetResolution().X;
			int Height = GetResolution().Y;

			if(R.Y > Height - 2)
				R.Y = Height - 2;
			if(R.X > Width - 2)
				R.X = Width - 2;

			if(R.W <= R.Y)
				R.W = R.Y + 1;
			if(R.Z <= R.X)
				R.Z = R.X + 1;

			if(R.W > Height - 1)
				R.W = Height - 1;
			if(R.Z> Width - 1)
				R.Z= Width - 1;
		}


		// Manager creation
		IGUIManager* CreateGUIManager(IGUIRenderer* renderer, NGin::Math::Vector2& Resolution)
		{
			// Create manager and initialize
			CGUIManager* Mgr = new CGUIManager(Math::Vector2(1.0f, 1.0f) / Resolution);
			renderer->SetManager(Mgr);

			// Success
			if(Mgr->Initialize(renderer, Resolution))
				return Mgr;

			// Fail
			delete Mgr;
			return 0;
		}
	}
}
