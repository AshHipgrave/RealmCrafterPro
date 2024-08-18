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
#include "CPropertySet.h"

namespace NGin
{
	namespace GUI
	{
		CPropertySet::CPropertySet(std::string Name, CGUIManager* GUIManager)
		{
			this->Name = Name;
			this->Manager = GUIManager;
		}

		CPropertySet::~CPropertySet()
		{
			for(int i = 0; i < ComponentStates.Size(); ++i)
				delete ComponentStates[i];
		}

		std::string CPropertySet::GetName()
		{
			return Name;
		}

		void CPropertySet::Process(XMLReader* X)
		{
			if(X->GetAttributeString("name").length() == 0)
				return;

			SPropertyState* State = new SPropertyState();
			State->Name = X->GetAttributeString("name");
			std::transform(State->Name.begin(), State->Name.end(), State->Name.begin(), ::tolower);

			bool Found = false;

			for(int i = 0; i < ComponentStates.Size(); ++i)
				if(ComponentStates[i]->Name.compare(State->Name) == 0)
				{
					delete State;
					State = ComponentStates[i];
					Found = true;
					break;
				}

			if(Found == false)
				ComponentStates.Add(State);

			for(int i = 0; i < X->GetAttributeCount(); ++i)
			{
				std::string Attribute = X->GetAttributeName(i);
				std::transform(Attribute.begin(), Attribute.end(), Attribute.begin(), ::tolower);
				std::string Data = X->GetAttributeString(X->GetAttributeName(i));

				if(Attribute.compare("location") == 0)
				{
					// Split out location data
					std::string X, Y;

					// Find data comma
					int Comma = Data.find(",", 0);
					if(Comma == -1)
					{
						X = Data;

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);
					}else
					{
						X = Data.substr(0, Comma);
						Y = Data.substr(Comma + 1);

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);
						
						TrimStart = Y.find_first_not_of(" \t");
						TrimEnd = Y.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							Y = "";
						else
							Y = Y.substr(TrimStart, TrimEnd - TrimStart + 1);
						

						// Fix any vector which is larger than 2D
						int Comma2 = Y.find(",", 0);
						if(Comma2 > 0)
						{
							Y = Y.substr(0, Comma2);
							
							TrimStart = Y.find_first_not_of(" \t");
							TrimEnd = Y.find_last_not_of(" \t");
							if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
								Y = "";
							else
								Y = Y.substr(TrimStart, TrimEnd - TrimStart + 1);
						}
					}

					// Relations
					bool RelativeX = false;
					bool RelativeY = false;
					bool CenterX = false;
					bool CenterY = false;

					
					if(X.substr(X.length() - 1).compare("%") == 0)
					{
						X = X.substr(0, X.length() - 1);
						RelativeX = true;
					}

					if(Comma > -1)
					{
						if(Y.substr(Y.length() - 1).compare("%") == 0)
						{
							Y = Y.substr(0, Y.length() - 1);
							RelativeY = true;
						}
					}else
					{
						Y = X;
						RelativeY = RelativeX;
					}


					if(X.substr(X.length() - 1).compare("c") == 0)
					{
						X = X.substr(0, X.length() - 1);
						CenterX = true;
					}

					if(Comma > -1)
					{
						if(Y.substr(Y.length() - 1).compare("c") == 0)
						{
							Y = Y.substr(0, Y.length() - 1);
							CenterY = true;
						}
					}else
					{
						Y = X;
						CenterY = CenterX;
					}

					State->UseLocation = true;
					State->RelativeLocationX = RelativeX;
					State->RelativeLocationY = RelativeY;
					State->CenterLocationX = CenterX;
					State->CenterLocationY = CenterY;

					State->Location.X = atof(X.c_str());
					State->Location.Y = atof(Y.c_str());

				}else if(Attribute.compare("size") == 0)
				{
					// Split out location data
					std::string X, Y;

					// Find data comma
					int Comma = Data.find(",", 0);
					if(Comma == -1)
					{
						X = Data;

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);
					}else
					{
						X = Data.substr(0, Comma);
						Y = Data.substr(Comma + 1);

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);
						
						TrimStart = Y.find_first_not_of(" \t");
						TrimEnd = Y.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							Y = "";
						else
							Y = Y.substr(TrimStart, TrimEnd - TrimStart + 1);
						

						// Fix any vector which is larger than 2D
						int Comma2 = Y.find(",", 0);
						if(Comma2 > 0)
						{
							Y = Y.substr(0, Comma2);
							
							TrimStart = Y.find_first_not_of(" \t");
							TrimEnd = Y.find_last_not_of(" \t");
							if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
								Y = "";
							else
								Y = Y.substr(TrimStart, TrimEnd - TrimStart + 1);
						}
					}

					// Relations
					bool RelativeX = false;
					bool RelativeY = false;

					if(X.substr(X.length() - 1).compare("%") == 0)
					{
						X = X.substr(0, X.length() - 1);
						RelativeX = true;
					}

					if(Comma > -1)
					{
						if(Y.substr(Y.length() - 1).compare("%") == 0)
						{
							Y = Y.substr(0, Y.length() - 1);
							RelativeY = true;
						}
					}else
					{
						Y = X;
						RelativeY = RelativeX;
					}


					State->UseSize = true;
					State->RelativeSizeX = RelativeX;
					State->RelativeSizeY = RelativeY;

					State->Size.X = atof(X.c_str());
					State->Size.Y = atof(Y.c_str());

				}else if(Attribute.compare("text") == 0)
				{
					State->UseText = true;
					State->Text = Data;

				}else if(Attribute.compare("visible") == 0)
				{
					std::transform(Data.begin(), Data.end(), Data.begin(), ::tolower);
					if(Data.compare("true") == 0 || Data.compare("1") == 0)
						State->Visible = true;
					else
						State->Visible = false;
					State->UseVisible = true;

				}else if(Attribute.compare("enabled") == 0)
				{
					std::transform(Data.begin(), Data.end(), Data.begin(), ::tolower);
					if(Data.compare("true") == 0 || Data.compare("1") == 0)
						State->Enabled = true;
					else
						State->Enabled = false;
					State->UseEnabled = true;

				}else if(Attribute.compare("backcolor") == 0)
				{
					if(Data.substr(0, 1).compare("#") == 0)
						Data = Data.substr(1);

					float Alpha = 1.0f;
					if(Data.length() > 6)
					{
						std::string A = Data.substr(0, 2);
						Data = Data.substr(2);

						const char* Str = A.c_str();
						int StrI[2];

						if(Str[0] > 47 && Str[0] < 58)
							StrI[0] = Str[0] - 48;
						if(Str[0] > 96 && Str[0] < 103)
							StrI[0] = Str[0] - 87;
						
						if(Str[1] > 47 && Str[1] < 58)
							StrI[1] = Str[1] - 48;
						if(Str[1] > 96 && Str[1] < 103)
							StrI[1] = Str[1] - 87;

						Alpha = (float)((StrI[0] * 16) + StrI[1]);
						Alpha /= 255.0f;
					}

					Math::Color C = Math::Color::FromString(Data);
					C.A = Alpha;

					State->BackColor = C;
					State->UseBackColor = true;

				}else if(Attribute.compare("forecolor") == 0)
				{
					if(Data.substr(0, 1).compare("#") == 0)
						Data = Data.substr(1);

					float Alpha = 1.0f;
					if(Data.length() > 6)
					{
						std::string A = Data.substr(0, 2);
						Data = Data.substr(2);

						const char* Str = A.c_str();
						int StrI[2];

						if(Str[0] > 47 && Str[0] < 58)
							StrI[0] = Str[0] - 48;
						if(Str[0] > 96 && Str[0] < 103)
							StrI[0] = Str[0] - 87;
						
						if(Str[1] > 47 && Str[1] < 58)
							StrI[1] = Str[1] - 48;
						if(Str[1] > 96 && Str[1] < 103)
							StrI[1] = Str[1] - 87;

						Alpha = (float)((StrI[0] * 16) + StrI[1]);
						Alpha /= 255.0f;
					}

					Math::Color C = Math::Color::FromString(Data);
					C.A = Alpha;

					State->ForeColor = C;
					State->UseForeColor = true;

				}else if(Attribute.compare("selectionforecolor") == 0)
				{
					if(Data.substr(0, 1).compare("#") == 0)
						Data = Data.substr(1);

					float Alpha = 1.0f;
					if(Data.length() > 6)
					{
						std::string A = Data.substr(0, 2);
						Data = Data.substr(2);

						const char* Str = A.c_str();
						int StrI[2];

						if(Str[0] > 47 && Str[0] < 58)
							StrI[0] = Str[0] - 48;
						if(Str[0] > 96 && Str[0] < 103)
							StrI[0] = Str[0] - 87;
						
						if(Str[1] > 47 && Str[1] < 58)
							StrI[1] = Str[1] - 48;
						if(Str[1] > 96 && Str[1] < 103)
							StrI[1] = Str[1] - 87;

						Alpha = (float)((StrI[0] * 16) + StrI[1]);
						Alpha /= 255.0f;
					}

					Math::Color C = Math::Color::FromString(Data);
					C.A = Alpha;

					State->SelectionForeColor = C;
					State->UseSelectionForeColor = true;

				}else if(Attribute.compare("selectionbackcolor") == 0)
				{
					if(Data.substr(0, 1).compare("#") == 0)
						Data = Data.substr(1);

					float Alpha = 1.0f;
					if(Data.length() > 6)
					{
						std::string A = Data.substr(0, 2);
						Data = Data.substr(2);

						const char* Str = A.c_str();
						int StrI[2];

						if(Str[0] > 47 && Str[0] < 58)
							StrI[0] = Str[0] - 48;
						if(Str[0] > 96 && Str[0] < 103)
							StrI[0] = Str[0] - 87;
						
						if(Str[1] > 47 && Str[1] < 58)
							StrI[1] = Str[1] - 48;
						if(Str[1] > 96 && Str[1] < 103)
							StrI[1] = Str[1] - 87;

						Alpha = (float)((StrI[0] * 16) + StrI[1]);
						Alpha /= 255.0f;
					}

					Math::Color C = Math::Color::FromString(Data);
					C.A = Alpha;

					State->SelectionBackColor = C;
					State->UseSelectionBackColor = true;

				}else if(Attribute.compare("skin") == 0)
				{
					
					State->Skin = Data;
					State->UseSkin = true;

				}else if(Attribute.compare("align") == 0)
				{
					std::transform(Data.begin(), Data.end(), Data.begin(), ::tolower);
					if(Data.compare("left") == 0 || Data.compare("top") == 0)
					{
						State->Align = TextAlign_Left;
						State->UseAlign = true;
					}else if(Data.compare("center") == 0 || Data.compare("centre") == 0 || Data.compare("middle") == 0)
					{
						State->Align = TextAlign_Center;
						State->UseAlign = true;
					}else if(Data.compare("right") == 0 || Data.compare("bottom") == 0)
					{
						State->Align = TextAlign_Right;
						State->UseAlign = true;
					}

				}else if(Attribute.compare("valign") == 0)
				{
					std::transform(Data.begin(), Data.end(), Data.begin(), ::tolower);
					if(Data.compare("left") == 0 || Data.compare("top") == 0)
					{
						State->VAlign = TextAlign_Left;
						State->UseVAlign = true;
					}else if(Data.compare("center") == 0 || Data.compare("centre") == 0 || Data.compare("middle") == 0)
					{
						State->VAlign = TextAlign_Center;
						State->UseVAlign = true;
					}else if(Data.compare("right") == 0 || Data.compare("bottom") == 0)
					{
						State->VAlign = TextAlign_Right;
						State->UseVAlign = true;
					}

				}else if(Attribute.compare("useborder") == 0)
				{
					std::transform(Data.begin(), Data.end(), Data.begin(), ::tolower);
					if(Data.compare("true") == 0 || Data.compare("1") == 0)
						State->UseBorder = true;
					else
						State->UseBorder = false;
					State->UseUseBorder = true;

				}else if(Attribute.compare("upimage") == 0 && Data.length() > 0)
				{
					State->UpImage = Data;
					State->UseUpImage = true;

				}else if(Attribute.compare("downimage") == 0 && Data.length() > 0)
				{
					State->DownImage = Data;
					State->UseDownImage = true;

				}else if(Attribute.compare("hoverimage") == 0 && Data.length() > 0)
				{
					State->HoverImage = Data;
					State->UseHoverImage = true;

				}else if(Attribute.compare("image") == 0 && Data.length() > 0)
				{
					if ( Data.find("Data\\Textures") == -1 ) Data = "Data\\Textures\\" + Data;
					State->UpImage = Data;
					State->UseUpImage = true;

				}else if(Attribute.compare("radarborder") == 0 && Data.length() > 0)
				{
					if ( Data.find("Data\\Textures") == -1 ) Data = "Data\\Textures\\" + Data;
					State->borderImage = Data;
					State->UseBorderImage = true;

				}else if(Attribute.compare("viewradius") == 0)
				{
					// Split out location data
					std::string X, Y;

					// Find data comma
					int Comma = Data.find(",", 0);
					if(Comma == -1)
					{
						X = Data;

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);
					}else
					{
						X = Data.substr(0, Comma);
						Y = Data.substr(Comma + 1);

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);

						TrimStart = Y.find_first_not_of(" \t");
						TrimEnd = Y.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							Y = "";
						else
							Y = Y.substr(TrimStart, TrimEnd - TrimStart + 1);
					}

					State->UseViewRadius = true;
					State->vViewRadius.X = atof(X.c_str());
					State->vViewRadius.Y = atof(Y.c_str());
					

				}else if(Attribute.compare("imagetop") == 0)
				{
					// Split out location data
					std::string X, Y;

					// Find data comma
					int Comma = Data.find(",", 0);
					if(Comma == -1)
					{
						X = Data;

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);
					}else
					{
						X = Data.substr(0, Comma);
						Y = Data.substr(Comma + 1);

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);

						TrimStart = Y.find_first_not_of(" \t");
						TrimEnd = Y.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							Y = "";
						else
							Y = Y.substr(TrimStart, TrimEnd - TrimStart + 1);
					}

					State->UseRadarImageTop = true;
					State->vRadarImageTop.X = atof(X.c_str());
					State->vRadarImageTop.Y = atof(Y.c_str());

				}else if(Attribute.compare("imagesize") == 0)
				{
					// Split out location data
					std::string X, Y;

					// Find data comma
					int Comma = Data.find(",", 0);
					if(Comma == -1)
					{
						X = Data;

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);
					}else
					{
						X = Data.substr(0, Comma);
						Y = Data.substr(Comma + 1);

						// Trim values using a bit of work (grr STL)
						size_t TrimStart = X.find_first_not_of(" \t");
						size_t TrimEnd = X.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							X = "";
						else
							X = X.substr(TrimStart, TrimEnd - TrimStart + 1);

						TrimStart = Y.find_first_not_of(" \t");
						TrimEnd = Y.find_last_not_of(" \t");
						if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
							Y = "";
						else
							Y = Y.substr(TrimStart, TrimEnd - TrimStart + 1);
					}

					State->UseRadarImageSize = true;
					State->vRadarImageSize.X = atof(X.c_str());
					State->vRadarImageSize.Y = atof(Y.c_str());
				}
			}
		}

		void CPropertySet::ApplySet(IControl* ManagerSet, std::string ControlName, int Depth)
		{
			int DepthAt = 0;
			std::transform(ControlName.begin(), ControlName.end(), ControlName.begin(), ::tolower);
			this->RecursiveApplySet(ManagerSet, ControlName, Depth, DepthAt);
		}

		void CPropertySet::RecursiveApplySet(IControl* Parent, std::string ControlName, int Depth, int &DepthAt)
		{
			++DepthAt;
			if(DepthAt == Depth)
			{
				--DepthAt;
				return;
			}

			List<IControl*>* Children = Parent->Controls();

			foreachf(CIt, IControl, (*Children))
			{
				IControl* Child = *CIt;

				for(int i = 0; i < this->ComponentStates.Size(); ++i)
				{
					//char O[1024];
					//sprintf(O, "%s == %s\n", Child->Name().AsCString().c_str(), ComponentStates[i]->Name.AsCString().c_str());
					//OutputDebugString(O);

					std::string NameLower = Child->Name;
					std::transform(NameLower.begin(), NameLower.end(), NameLower.begin(), ::tolower);

					if((ControlName.length() == 0 && NameLower.compare(ComponentStates[i]->Name) == 0) || (ControlName.length() > 0 && NameLower.compare(ControlName) == 0 && NameLower.compare(ComponentStates[i]->Name) == 0))
					{
						//if(ControlName.length() > 0)
						//{
						//	String Out = ControlName.AsCString() + " == " + Child->Name.AsLower().AsCString() + " is: " + Child->GetType().Name().AsCString() + "\n";
						//	OutputDebugString(Out.c_str());
						//	//return;
						//}

						SPropertyState* State = ComponentStates[i];

						Math::Vector2 SetSize;

						if(State->UseSize)
						{
							if(State->RelativeSizeX)
								SetSize.X = (State->Size.X / 100.0f);
							else
								SetSize.X = State->Size.X / Manager->GetResolution().X;

							if(State->RelativeSizeY)
								SetSize.Y = (State->Size.Y / 100.0f);
							else
								SetSize.Y = State->Size.Y / Manager->GetResolution().Y;

							Child->Size = SetSize;
						}

						if(State->UseLocation)
						{
							Math::Vector2 SetLocation;

							if(State->RelativeLocationX)
								SetLocation.X = (State->Location.X / 100.0f);
							else if(State->CenterLocationX)
								SetLocation.X = (State->Location.X / 100.0f) - (SetSize.X / 2.0f);
							else
								SetLocation.X = State->Location.X / Manager->GetResolution().X;

							if(State->RelativeLocationY)
								SetLocation.Y = (State->Location.Y / 100.0f);
							else if(State->CenterLocationY)
								SetLocation.Y = (State->Location.Y / 100.0f) - (SetSize.Y / 2.0f);
							else
								SetLocation.Y = State->Location.Y / Manager->GetResolution().Y;

							if(SetLocation.X < 0)
								SetLocation.X = 1.0f + SetLocation.X;
							if(SetLocation.Y < 0)
								SetLocation.Y = 1.0f + SetLocation.Y;

							Child->Location = SetLocation;
						}

						if(State->UseUpImage)
						{
							bool Set = false;

							if(Child->GetType() == IButton::TypeOf())
								Set = ((IButton*)Child)->SetUpImage(State->UpImage);

							if(Child->GetType() == IPictureBox::TypeOf())
							{
								Set = ((IPictureBox*)Child)->SetImage(State->UpImage);
								printf("IPictureBox::SetImage() %s; %s\n", ControlName.c_str(), State->UpImage.c_str());
							}

							if(Child->GetType() == IRadar::TypeOf())
								Set = ((IRadar*)Child)->SetImage(State->UpImage);

							if(Set == false)
							{
								std::string Out = std::string("Unable to set image: ") + State->UpImage + "\n";
								Manager->GetRenderer()->DebugOut(Out.c_str());
							}
						}
						
						if(State->UseDownImage)
						{
							if(Child->GetType() == IButton::TypeOf())
								((IButton*)Child)->SetUpImage(State->UpImage);
						}

						if(State->UseHoverImage)
						{
							if(Child->GetType() == IButton::TypeOf())
								((IButton*)Child)->SetUpImage(State->UpImage);
						}

						if(State->UseText)
							Child->Text = State->Text;

						//if(UseSkin)
						//	Child->Skin(State->);

						if(State->UseVisible)
							Child->Visible = State->Visible;
						if(State->UseEnabled)
							Child->Enabled = State->Enabled;
						
						if(State->UseUseBorder)
						{
							if(Child->GetType() == IButton::TypeOf())
								((IButton*)Child)->UseBorder = State->UseBorder;
						}
						
						if(State->UseAlign)
						{
							if(Child->GetType() == IButton::TypeOf())
								((IButton*)Child)->Align = State->Align;

							if(Child->GetType() == ILabel::TypeOf())
								((ILabel*)Child)->Align = State->Align;
						}

						if(State->UseVAlign)
						{
							if(Child->GetType() == IButton::TypeOf())
								((IButton*)Child)->VAlign = State->VAlign;

							if(Child->GetType() == ILabel::TypeOf())
								((ILabel*)Child)->VAlign = State->VAlign;
						}

						if(State->UseSelectionForeColor)
							if(Child->GetType() == IListBox::TypeOf())
								((IListBox*)Child)->SelectionForeColor = State->SelectionForeColor;

						if(State->UseSelectionBackColor)
							if(Child->GetType() == IListBox::TypeOf())
								((IListBox*)Child)->SelectionBackColor = State->SelectionBackColor;

						if(State->UseForeColor) Child->ForeColor = State->ForeColor;
						if(State->UseBackColor) Child->BackColor = State->BackColor;

						if ( State->UseBorderImage )
						{
							bool Set = false;
							if(Child->GetType() == IRadar::TypeOf())
								Set = ((IRadar*)Child)->SetImage("", State->borderImage);

							if( !Set )
							{
								std::string Out = std::string("Unable to set image: ") + State->borderImage + "\n";
								Manager->GetRenderer()->DebugOut(Out.c_str());
							}
						}

						if ( State->UseViewRadius ) ((IRadar*)Child)->ViewRadius = State->vViewRadius;
						if ( State->UseRadarImageTop ) ((IRadar*)Child)->RadarImageTop = State->vRadarImageTop;
						if ( State->UseRadarImageSize ) ((IRadar*)Child)->RadarImageSize = State->vRadarImageSize;
					}

					//if(ControlName.Length() > 0)
					//	break;
				}

				RecursiveApplySet(Child, ControlName, Depth, DepthAt);

				nextf(CIt, IControl, (*Children));
			}

			--DepthAt;
		}

		void CPropertySet::ApplyControl(IControl* Child)
		{

			for(int i = 0; i < this->ComponentStates.Size(); ++i)
			{
				//char O[1024];
				//sprintf(O, "%s == %s\n", Child->Name().AsCString().c_str(), ComponentStates[i]->Name.AsCString().c_str());
				//OutputDebugString(O);

				SPropertyState* State = ComponentStates[i];

				Math::Vector2 SetSize;

				if(State->UseSize)
				{
					if(State->RelativeSizeX)
						SetSize.X = (State->Size.X / 100.0f);
					else
						SetSize.X = State->Size.X / Manager->GetResolution().X;

					if(State->RelativeSizeY)
						SetSize.Y = (State->Size.Y / 100.0f);
					else
						SetSize.Y = State->Size.Y / Manager->GetResolution().Y;

					Child->Size = SetSize;
				}

				if(State->UseLocation)
				{
					Math::Vector2 SetLocation;

					if(State->RelativeLocationX)
						SetLocation.X = (State->Location.X / 100.0f);
					else if(State->CenterLocationX)
						SetLocation.X = (State->Location.X / 100.0f) - (SetSize.X / 2.0f);
					else
						SetLocation.X = State->Location.X / Manager->GetResolution().X;

					if(State->RelativeLocationY)
						SetLocation.Y = (State->Location.Y / 100.0f);
					else if(State->CenterLocationY)
						SetLocation.Y = (State->Location.Y / 100.0f) - (SetSize.Y / 2.0f);
					else
						SetLocation.Y = State->Location.Y / Manager->GetResolution().Y;

					Child->Location = SetLocation;
				}

				if(State->UseUpImage)
				{
					bool Set = false;

					if(Child->GetType() == IButton::TypeOf())
						Set = ((IButton*)Child)->SetUpImage(State->UpImage);

					if(Child->GetType() == IPictureBox::TypeOf())
					{
						Set = ((IPictureBox*)Child)->SetImage(State->UpImage);
						printf("IPictureBox::SetImage() %s; %s\n", Child->Name.c_str(), State->UpImage.c_str());
					}

					if(Set == false)
					{
						std::string Out = std::string("Unable to set image: ") + State->UpImage + "\n";
						Manager->GetRenderer()->DebugOut(Out.c_str());
					}
				}
				
				if(State->UseDownImage)
				{
					if(Child->GetType() == IButton::TypeOf())
						((IButton*)Child)->SetUpImage(State->UpImage);
				}

				if(State->UseHoverImage)
				{
					if(Child->GetType() == IButton::TypeOf())
						((IButton*)Child)->SetUpImage(State->UpImage);
				}

				if(State->UseText)
					Child->Text = State->Text;

				//if(UseSkin)
				//	Child->Skin(State->);

				if(State->UseVisible)
					Child->Visible = State->Visible;
				if(State->UseEnabled)
					Child->Enabled = State->Enabled;
				
				if(State->UseUseBorder)
				{
					if(Child->GetType() == IButton::TypeOf())
						((IButton*)Child)->UseBorder = State->UseBorder;
				}
				
				if(State->UseAlign)
				{
					if(Child->GetType() == IButton::TypeOf())
						((IButton*)Child)->Align = State->Align;

					if(Child->GetType() == ILabel::TypeOf())
						((ILabel*)Child)->Align = State->Align;
				}

				if(State->UseVAlign)
				{
					if(Child->GetType() == IButton::TypeOf())
						((IButton*)Child)->VAlign = State->VAlign;

					if(Child->GetType() == ILabel::TypeOf())
						((ILabel*)Child)->VAlign = State->VAlign;
				}

				if(State->UseSelectionForeColor)
					if(Child->GetType() == IListBox::TypeOf())
						((IListBox*)Child)->SelectionForeColor = State->SelectionForeColor;

				if(State->UseSelectionBackColor)
					if(Child->GetType() == IListBox::TypeOf())
						((IListBox*)Child)->SelectionBackColor = State->SelectionBackColor;

				if(State->UseForeColor)
					Child->ForeColor = State->ForeColor;
				if(State->UseBackColor)
					Child->BackColor = State->BackColor;
			}
		}
	}
}
