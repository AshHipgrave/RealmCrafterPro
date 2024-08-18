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
#include "CSkin.h"
#include <XMLWrapper.h>

namespace NGin
{
	namespace GUI
	{
		#pragma region ElementNames
		std::string ElementNames[] =
		{
			"window_border_tl",
			"window_border_t",
			"window_border_tr",
			"window_border_l",
			"window_border_c",
			"window_border_r",
			"window_border_bl",
			"window_border_b",
			"window_border_br",
			"window_fill",
			"window_close_up",
			"window_close_hover",
			"window_close_down",
			"button_border_tl_up",
			"button_border_t_up",
			"button_border_tr_up",
			"button_border_l_up",
			"button_fill_up",
			"button_border_r_up",
			"button_border_bl_up",
			"button_border_b_up",
			"button_border_br_up",
			"button_border_tl_hover",
			"button_border_t_hover",
			"button_border_tr_hover",
			"button_border_l_hover",
			"button_fill_hover",
			"button_border_r_hover",
			"button_border_bl_hover",
			"button_border_b_hover",
			"button_border_br_hover",
			"button_border_tl_down",
			"button_border_t_down",
			"button_border_tr_down",
			"button_border_l_down",
			"button_fill_down",
			"button_border_r_down",
			"button_border_bl_down",
			"button_border_b_down",
			"button_border_br_down",
			"button_border_tl_disabled",
			"button_border_t_disabled",
			"button_border_tr_disabled",
			"button_border_l_disabled",
			"button_fill_disabled",
			"button_border_r_disabled",
			"button_border_bl_disabled",
			"button_border_b_disabled",
			"button_border_br_disabled",
			"checkbox_fill_up",
			"checkbox_fill_hover",
			"checkbox_fill_down",
			"checkbox_fill_disabled",
			"checkbox_check",
			"label_fill",
			"vscrollbar_normal_up",
			"vscrollbar_normal_down",
			"vscrollbar_normal_top",
			"vscrollbar_normal_middle",
			"vscrollbar_normal_bottom",
			"vscrollbar_normal_grip",
			"vscrollbar_hover_up",
			"vscrollbar_hover_down",
			"vscrollbar_hover_top",
			"vscrollbar_hover_middle",
			"vscrollbar_hover_bottom",
			"vscrollbar_hover_grip",
			"vscrollbar_down_up",
			"vscrollbar_down_down",
			"vscrollbar_down_top",
			"vscrollbar_down_middle",
			"vscrollbar_down_bottom",
			"vscrollbar_down_grip",
			"vscrollbar_fill",
			"hscrollbar_normal_up",
			"hscrollbar_normal_down",
			"hscrollbar_normal_top",
			"hscrollbar_normal_middle",
			"hscrollbar_normal_bottom",
			"hscrollbar_normal_grip",
			"hscrollbar_hover_up",
			"hscrollbar_hover_down",
			"hscrollbar_hover_top",
			"hscrollbar_hover_middle",
			"hscrollbar_hover_bottom",
			"hscrollbar_hover_grip",
			"hscrollbar_down_up",
			"hscrollbar_down_down",
			"hscrollbar_down_top",
			"hscrollbar_down_middle",
			"hscrollbar_down_bottom",
			"hscrollbar_down_grip",
			"hscrollbar_fill",
			"progressbar_left",
			"progressbar_middle",
			"progressbar_right",
			"progressbar_fill",
			"trackbar_button_up",
			"trackbar_button_hover",
			"trackbar_button_down",
			"trackbar_background_left",
			"trackbar_background_middle",
			"trackbar_background_right",
			"trackbar_tick_large",
			"trackbar_tick_small",
			"listbox_border_t",
			"listbox_border_l",
			"listbox_border_b",
			"listbox_border_r",
			"listbox_border_tl",
			"listbox_border_tr",
			"listbox_border_bl",
			"listbox_border_br",
			"listbox_fill",
			"combobox_left_up",
			"combobox_middle_up",
			"combobox_right_up",
			"combobox_fill_up",
			"combobox_left_hover",
			"combobox_middle_hover",
			"combobox_right_hover",
			"combobox_fill_hover",
			"combobox_left_down",
			"combobox_middle_down",
			"combobox_right_down",
			"combobox_fill_down",
			"combobox_left_disabled",
			"combobox_middle_disabled",
			"combobox_right_disabled",
			"combobox_fill_disabled",
			"textbox_border_tl",
			"textbox_border_t",
			"textbox_border_tr",
			"textbox_border_l",
			"textbox_border_r",
			"textbox_border_bl",
			"textbox_border_b",
			"textbox_border_br",
			"textbox_fill",
			"textbox_caret",
			"listbox_selectionfill"
		};
		const int MaxElements = 141;

		std::string FontNames[] =
		{
			"default",
			"windowtitle",
			"control"
		};
		const int MaxFonts = 4;

		std::string CursorNames[] =
		{
			"pointer",
			"textinput"
		};
		const int MaxCursors = 3;

		#pragma endregion

		CSkin::CSkin(CGUIManager* GUIManager)
		{
			SkinTexture = 0;
			Manager = GUIManager;
		}

		CSkin::~CSkin()
		{
			if(SkinTexture != NULL)
				Manager->GetRenderer()->FreeTexture(SkinTexture);

			for(int i = 0; i < PropertySets.Size(); ++i)
				delete PropertySets[i];
		}

		void CSkin::OnDeviceReset(Vector2& OldResolution, Vector2& NewResolution)
		{
			for(int i = 0; i < Coords.Size(); ++i)
			{
				Coords[i].ScreenSize *= OldResolution;
				Coords[i].ScreenSize /= NewResolution;
			}
		}

		bool CSkin::Initialize(std::string SkinFile, NGin::Math::Vector2 &Res)
		{
			Resolution = Res;

			XMLReader* X = ReadXMLFile(SkinFile);
			if(X == 0)
				return false;

			size_t SlashPos = 0;
			size_t L = 0;

			while(true)
			{
				L = SkinFile.find("\\", SlashPos + 1);
				if(L != -1)
					SlashPos = L;
				else
					break;
			}

			std::string Path = SkinFile.substr(0, SlashPos) + "\\";

			Vector2 TextureSize;

			for(int i = 0; i < MaxCursors - 1; ++i)
				Cursors[i] = -1;

			// Read
			while(X->Read())
			{
				std::string NodeName = X->GetNodeName();
				std::transform(NodeName.begin(), NodeName.end(), NodeName.begin(), ::tolower);

				// If we reach the end of the skin, bail.
				if(X->GetNodeType() == XNT_Element_End && NodeName.compare("skin") == 0)
					break;

				// It's the opening tag of the skin
				if(X->GetNodeType() == XNT_Element && NodeName.compare("skin") == 0)
				{
					// Get the path
					std::string TexPath = X->GetAttributeString("texture");
					std::string TexName = Path + TexPath;
					if(TexName.length() == 0)
					{
						Manager->GetRenderer()->DebugOut((std::string("No texture attribute found in skin node: ") + SkinFile + "\n").c_str());
						delete X;
						return false;
					}

					SkinTexture = Manager->GetRenderer()->GetTexture(TexName.c_str(), 0xffff0000, true);
					if(SkinTexture == 0)
					{
						Manager->GetRenderer()->DebugOut((std::string("Could not load texture: ") + TexName + "\n").c_str());
						delete X;
						return false;
					}

					// Get texture size
					TextureSize = SkinTexture->GetSize();
				}

				// It's a font
				if(X->GetNodeType() == XNT_Element && NodeName.compare("font") == 0)
				{
					// Get info
					std::string FontType = X->GetAttributeString("type");
					std::transform(FontType.begin(), FontType.end(), FontType.begin(), ::tolower);
					std::string FontPath = X->GetAttributeString("src");
					int FontSize = X->GetAttributeInt("size");
					if(FontSize == 0)
						FontSize = 12;

					int FontPos = MaxFonts;

					// Find what the font is
					for(int i = 0; i < MaxFonts - 1; ++i)
						if(FontType == FontNames[i])
							FontPos = i;

					// Load
					IFont* Font = Manager->LoadFont(FontPath, FontSize);
					if(Font == 0)
					{
						Manager->GetRenderer()->DebugOut((std::string("Could not load font: ") + FontPath + "\n").c_str());
						delete X;
						return false;
					}
					this->Fonts[FontPos] = Font;
				}

				// It's a cursor
				if(X->GetNodeType() == XNT_Element && NodeName.compare("cursor") == 0)
				{
					// Get info
					std::string CursorType = X->GetAttributeString("type");
					std::transform(CursorType.begin(), CursorType.end(), CursorType.begin(), ::tolower);
					std::string CursorPath = X->GetAttributeString("src");
					int CursorPos = MaxCursors;

					// Find what the cursor is
					for(int i = 0; i < MaxCursors - 1; ++i)
						if(CursorType == CursorNames[i])
							CursorPos = i;

					// Load
					int Cursor = Manager->LoadCursor(Path + CursorPath);
					if(Cursor == -1)
					{
						Manager->GetRenderer()->DebugOut((std::string("Could not load cursor: ") + Path + CursorPath + "\n").c_str());
						delete X;
						return false;
					}

					this->Cursors[CursorPos] = (unsigned int)Cursor;
				}

				// It's a default propertyset
				if(X->GetNodeType() == XNT_Element && NodeName.compare("defaultproperty") == 0)
				{
					CPropertySet* Set = new CPropertySet(X->GetAttributeString("name"), this->Manager);
					Set->Process(X);
					this->PropertySets.Add(Set);
				}

				// It's an onscreen component
				if(X->GetNodeType() == XNT_Element && NodeName.compare("component") == 0)
				{
					// Get info
					std::string Name = X->GetAttributeString("name");
					std::transform(Name.begin(), Name.end(), Name.begin(), ::tolower);
					std::string sLocation = X->GetAttributeString("location");
					std::string sSize = X->GetAttributeString("size");

					Vector2 vLocation, vSize;
					int ComponentPos = MaxElements;

					// Which component is it?
					for(int i = 0; i < MaxElements; ++i)
						if(Name == ElementNames[i])
							ComponentPos = i;

					if(ComponentPos == MaxElements)
						ComponentPos = (MaxElements > this->Coords.Size()) ? MaxElements : this->Coords.Size();

					// Get Location
					if(sLocation.length() > 0)
					{
						std::string L, R;
						L = sLocation.substr(0, sLocation.find(",", 0));
						R = sLocation.substr(sLocation.find(",", 0) + 1);

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = L.find_first_not_of(" \t");
						size_t TrimEnd = L.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							L = "";
						else
							L = L.substr(TrimStart, TrimEnd - TrimStart + 1);

						TrimStart = R.find_first_not_of(" \t");
						TrimEnd = R.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							R = "";
						else
							R = R.substr(TrimStart, TrimEnd - TrimStart + 1);

						vLocation.X = atof(L.c_str());
						vLocation.Y = atof(R.c_str());

					}

					// Get Size
					if(sSize.length() > 0)
					{
						std::string L, R;
						L = sSize.substr(0, sSize.find(",", 0));
						R = sSize.substr(sSize.find(",", 0) + 1);

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = L.find_first_not_of(" \t");
						size_t TrimEnd = L.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							L = "";
						else
							L = L.substr(TrimStart, TrimEnd - TrimStart + 1);

						TrimStart = R.find_first_not_of(" \t");
						TrimEnd = R.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							R = "";
						else
							R = R.substr(TrimStart, TrimEnd - TrimStart + 1);

						vSize.X = atof(L.c_str());
						vSize.Y = atof(R.c_str());

					}

					// Build internal storage
					SkinCoord C;
					C.TexPos = vLocation;
					C.TexSize = vSize;
					C.ScreenSize = vSize;
					C.Name = Name;

					C.TexPos /= TextureSize;
					C.TexSize /= TextureSize;
					C.ScreenSize /= Resolution;

					Coords[ComponentPos] = C;
				}
			}

			delete X;

			// All done
			return true;
		}

		IFont* CSkin::GetFont(GUIFont Font)
		{
			int C = (int)Font;
			if(C < 0 || C >= Fonts.Size())
				return Fonts[0];

			return Fonts[C];
		}

		NGin::Math::Vector2 CSkin::GetCoord(std::string Coordinate)
		{
			std::transform(Coordinate.begin(), Coordinate.end(), Coordinate.begin(), ::tolower);

			for(int i = 0; i < this->Coords.Size(); ++i)
				if(this->Coords[i].Name == Coordinate)
					return Coords[i].TexPos;

			return Vector2(0, 0);
		}

		NGin::Math::Vector2 CSkin::GetCoord(GUICoord Coordinate)
		{
			int C = (int)Coordinate;
			if(C < 0 || C >= Coords.Size())
				return NGin::Math::Vector2(0.0f, 0.0f);

			return Coords[C].TexPos;
		}

		NGin::Math::Vector2 CSkin::GetSize(std::string Coordinate)
		{
			std::transform(Coordinate.begin(), Coordinate.end(), Coordinate.begin(), ::tolower);

			for(int i = 0; i < this->Coords.Size(); ++i)
				if(this->Coords[i].Name == Coordinate)
					return Coords[i].TexSize;

			return Vector2(0, 0);
		}

		NGin::Math::Vector2 CSkin::GetSize(GUICoord Coordinate)
		{
			int C = (int)Coordinate;
			if(C < 0 || C >= Coords.Size())
				return NGin::Math::Vector2(0.0f, 0.0f);

			return Coords[C].TexSize;
		}

		NGin::Math::Vector2 CSkin::GetScreenCoord(std::string Coordinate)
		{
			std::transform(Coordinate.begin(), Coordinate.end(), Coordinate.begin(), ::tolower);

			for(int i = 0; i < this->Coords.Size(); ++i)
				if(this->Coords[i].Name == Coordinate)
					return Coords[i].ScreenSize;

			return Vector2(0, 0);
		}

		NGin::Math::Vector2 CSkin::GetScreenCoord(GUICoord Coordinate)
		{
			int C = (int)Coordinate;
			if(C < 0 || C >= Coords.Size())
				return NGin::Math::Vector2(0.0f, 0.0f);

			return Coords[C].ScreenSize;
		}

		IGUITexture* CSkin::Texture()
		{
			return SkinTexture;
		}

		int CSkin::GetCursor(GUICursor Cursor)
		{
			int C = (int)Cursor;
			if(C < 0 || C >= Cursors.Size())
				return -1;

			return Cursors[C];
		}

		void CSkin::ApplyDefaultProperty(IControl* Control, std::string SetName)
		{
			std::transform(SetName.begin(), SetName.end(), SetName.begin(), ::tolower);

			CPropertySet* Set = 0;

			for(int i = 0; i < PropertySets.Size(); ++i)
			{
				std::string LowerName = PropertySets[i]->GetName();
				std::transform(LowerName.begin(), LowerName.end(), LowerName.begin(), ::tolower);
				if(LowerName.compare(SetName) == 0)
					Set = PropertySets[i];
			}

			if(Set == 0)
				return;

			Set->ApplyControl(Control);
		}
	}
}
