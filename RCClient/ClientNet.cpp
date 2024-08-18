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

#include "ClientNet.h"

int TradePackets = 0;
string TradeMsg1, TradeMsg2, TradeMsg3;
float NewEntityX = 0, NewEntityY = 0, NewEntityZ = 0, OldEntityX = 0, OldEntityY = 0, OldEntityZ = 0, OldYaw = 0;

// Connects to the server
void Connect()
{
	//RuntimeError("Second Connection!");
	// Attempt connection
//     int Port = 11002;
// 	do
// 	{
// 		Connection = RCE_Connect(RealmCrafter::Globals->ServerHost.c_str(), RealmCrafter::Globals->ServerPort, Port, Me->Name.c_str(), "", "Data\\Logs\\Client Connection.txt", true);
// 		++Port;
// 	}while(Connection == RCE_ConnectionInUse || Connection == RCE_PortInUse);
// 
// 	switch(Connection)
// 	{
// 		case ConnectionNotFound: RuntimeError(LanguageString[LS_InvalidHost] + string(" (") + RealmCrafter::Globals->ServerHost + string(")"));break;
// 		case RCE_TimedOut: RuntimeError(LanguageString[LS_NoResponse]);break;
// 		case RCE_ServerFull: RuntimeError(LanguageString[LS_TooManyPlayers]);break;
// 	}


	//string Pa = StrFromChar(UName.Length()) + UName + StrFromChar(PWord.Length()) + PWord;
	//Pa += Chr(SelectedCharacter);

	NGin::CString NUName = UName.c_str();
	NGin::CString NPWord = PWord.c_str();
	NGin::CString Pa = NGin::CString::FormatReal("cscsc", NUName.Length(), &NUName, NPWord.Length(), &NPWord, SelectedCharacter);
	if(!CRCE_Send(Connection, Connection, P_StartGame, Pa, true))
		RuntimeError("Message Failed!");

	for(int i = 0; i < 36; ++i)
		ActionBarSlots[i] = 65535;

	// Await reply
	int Done = 0;
	while(Done < 13)
	{
		Delay(10);

		// We get signal!
		foreachc(MIt, RCE_Message, RCE_MessageList)
		{
			RCE_Message* M = *MIt;

			if(M->MessageType == P_StartGame)
			{
				// Error message
				if(M->MessageData == NGin::CString("N"))
					RuntimeError(LanguageString[LS_AlreadyInGame]);

				//DUMPSTRING(M->MessageData.c_str(), M->MessageData.Length());
				const char* Dat = M->MessageData.c_str();

				for(int i = 0; i < M->MessageData.Length(); ++i)
				{
					if(Dat[i] < 0x10)
						printf("0");
					printf("%x", Dat[i]);

					if(i != M->MessageData.Length() - 1)
						printf("-");
				}
				printf("; Len=%i;\n", M->MessageData.Length());


				// Action bar data
				if(M->MessageData.Length() > 2)
				{
					int SlotNum = CharFromStr(M->MessageData.Substr(0, 1));
					int Offset = 1;
					for(int i = 1; i <= 3; ++i)
					{
						// Get slot data
						int NameLen = ShortFromStr(M->MessageData.Substr(Offset, 2));
						NGin::CString Slot = M->MessageData.Substr(Offset + 2, NameLen);
						Offset = Offset + 2 + NameLen;

						// Set up slot
						if(Slot.Length() > 0)
						{
							// Ability
							if(Slot.Substr(0, 1) == NGin::CString("S"))
							{
								string SpellName = Slot.Substr(1).c_str();
								// Must be memorised
								if(RealmCrafter::Globals->RequireMemorise)
								{
									for(int j = 0; j <= 9; ++j)
									{
										if(Me->MemorisedSpells[j] != 5000)
										{
											Spell* Sp = SpellsList[Me->KnownSpells[Me->MemorisedSpells[j]]];
											if(Sp->Name.compare(SpellName) == 0)
											{
												ActionBarSlots[SlotNum] = j - 10;
												break;
											}
										}
									}
								}else // Must be known
								{
									for(int j = 0; j <= 999; ++j)
									{
										if(Me->SpellLevels[j] > 0)
										{
											Spell* Sp = SpellsList[Me->KnownSpells[j]];
											if(Sp->Name.compare(SpellName) == 0)
											{
												ActionBarSlots[SlotNum] = j - 1000;
												break;
											}
										}
									}
								}
							}else if(Slot.Substr(0, 1) == NGin::CString("I")) // Item
								ActionBarSlots[SlotNum] = ShortFromStr(Slot.Substr(1));
						}else
							ActionBarSlots[SlotNum] = 65535;
						++SlotNum;
					}
				}else // My actor's runtime ID and XP bar level
				{
					Me->RuntimeID = ShortFromStr(M->MessageData);
					RuntimeIDList[Me->RuntimeID] = Me;
				}
				RCE_Message::Delete(M);
				++Done;
			}else if(M->MessageType == ConnectionHasLeft)
			{
				RuntimeError(LanguageString[LS_LostConnection]);
				RCE_Message::Delete(M);
			}

			nextc(MIt, RCE_Message, RCE_MessageList);
		}
		RCE_Message::Clean();
		RCE_Update();
		RCE_CreateMessages();
	}
	RCE_Message::Clean();

	// Success
	WriteLog(MainLog, "Successfully connected to server");
}

// Processes all network messages
void UpdateNetwork()
{
	// Incoming messages
	foreachc(MIt, RCE_Message, RCE_MessageList)
	{
		RCE_Message* M = *MIt;

		// What happen?
		switch(M->MessageType)
		{
		case P_OpenForm:
			{
				RealmCrafter::PacketReader Reader(const_cast<char*>(M->MessageData.c_str()), M->MessageData.Length(), false); 
				RealmCrafter::Globals->ControlHostManager->ProcessFormOpen(Reader);

				break;
			}
		case P_CloseForm:
			{
				RealmCrafter::PacketReader Reader(const_cast<char*>(M->MessageData.c_str()), M->MessageData.Length(), false); 
				RealmCrafter::Globals->ControlHostManager->ProcessFormClosed(Reader);

				break;
			}
		case P_PropertiesUpdate:
			{
				RealmCrafter::PacketReader Reader(const_cast<char*>(M->MessageData.c_str()), M->MessageData.Length(), false); 
				RealmCrafter::Globals->ControlHostManager->ProcessUpdateProperties(Reader);

				break;
			}
		case P_ReAllocateIDs:
			{
				int Pos = 0;
				while(Pos < M->MessageData.Length())
				{
					unsigned char ChangeType = M->MessageData.GetRealChar(Pos);
					int OldAllocID = M->MessageData.GetRealInt(Pos + 1);
					int NewAllocID = M->MessageData.GetRealInt(Pos + 5);

					Pos += 9;

					if(ChangeType == 'A')
					{
						foreachc(EIIt, EffectIcon, EffectIconList)
						{
							if((*EIIt)->ID == OldAllocID)
							{
								(*EIIt)->ID = NewAllocID;
								break;
							}

							nextc(EIIt, EffectIcon, EffectIconList);
						}
					}else if(ChangeType == 'I')
					{
						
					}else if(ChangeType == 'R')
					{
						if(Me != NULL)
							Me->RuntimeID = NewAllocID;
					}else if(ChangeType == 'P')
					{
						foreachc(DIt, ScriptProgressBar, ScriptProgressBarList)
						{
							if((*DIt)->ScriptHandle == OldAllocID)
							{
								(*DIt)->ScriptHandle = NewAllocID;
								break;
							}

							nextc(DIt, ScriptProgressBar, ScriptProgressBarList)
						}
					}

				}

				break;
			}
		case P_ProgressBar: // Scripted progress bar
			{
				// Create new
				if(M->MessageData.Substr(0, 1) == NGin::CString("C"))
				{
					uchar Red = CharFromStr(M->MessageData.Substr(1, 1));
					uchar Green = CharFromStr(M->MessageData.Substr(2, 1));
					uchar Blue = CharFromStr(M->MessageData.Substr(3, 1));
					float X = FloatFromStr(M->MessageData.Substr(4, 4));
					float Y = FloatFromStr(M->MessageData.Substr(8, 4));
					float W = FloatFromStr(M->MessageData.Substr(12, 4));
					float H = FloatFromStr(M->MessageData.Substr(16, 4));
					int AllocID = IntFromStr(M->MessageData.Substr(20, 4));
					short Max = ShortFromStr(M->MessageData.Substr(24, 2));
					short Value = ShortFromStr(M->MessageData.Substr(26, 2));
					string DisplayText = M->MessageData.Substr(28).c_str();

					IProgressBar* PBar = GUIManager->CreateProgressBar("", Math::Vector2(X, Y), Math::Vector2(W, H));
					PBar->BackColor = Math::Color(Red, Green, Blue);
					PBar->Tag = (void*)(int)Max;
					PBar->Value = (int)((((float)Value) / ((float)Max)) * 100.0f);
					ILabel* L = GUIManager->CreateLabel(DisplayText, Math::Vector2(0.5f, 0.5f - (0.015f / H)) / Math::Vector2(X, Y), Math::Vector2(0, 0));
					L->ForeColor = Math::Color(255, 255, 255);
					L->Parent = PBar;
					L->Text = DisplayText;
					L->Location = Math::Vector2(0, (H * 0.5f) - (L->InternalHeight * 0.5f));
					L->Size = Math::Vector2(W, 0);
					L->Align = TextAlign_Center;
#ifdef USE_250
					ScriptProgressBar* ScriptBar = new ScriptProgressBar();
					ScriptBar->BarHandle = PBar;
					ScriptBar->ScriptHandle = AllocID;
#else
					CRCE_Send(Connection, Connection, P_ProgressBar, NGin::CString("C") + M->MessageData.Substr(20, 4) + StrFromInt((int)PBar), true);
#endif
				// Update
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("U"))
				{
#ifdef USE_250
					int AllocID = IntFromStr(M->MessageData.Substr(1, 4));
					IProgressBar* PBar = NULL;
					foreachc(DIt, ScriptProgressBar, ScriptProgressBarList)
					{
						if((*DIt)->ScriptHandle == AllocID)
						{
							PBar = (*DIt)->BarHandle;
						}

						nextc(DIt, ScriptProgressBar, ScriptProgressBarList)
					}
#else
					IProgressBar* PBar = (IProgressBar*)IntFromStr(M->MessageData.Substr(1, 4));
#endif
					if(PBar != NULL)
					{
						int Value = ShortFromStr(M->MessageData.Substr(5, 2));
						int Max = (int)PBar->Tag;
						PBar->Value = (int)((((float)Value) / ((float)Max)) * 100.0f);
					}
				// Delete
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("D"))
				{
#ifdef USE_250
					int AllocID = IntFromStr(M->MessageData.Substr(1));
					IProgressBar* PBar = NULL;
					foreachc(DIt, ScriptProgressBar, ScriptProgressBarList)
					{
						if((*DIt)->ScriptHandle == AllocID)
						{
							PBar = (*DIt)->BarHandle;
						}

						nextc(DIt, ScriptProgressBar, ScriptProgressBarList)
					}
#else
					IProgressBar* PBar = (IProgressBar*)IntFromStr(M->MessageData.Substr(1));
#endif
					if(PBar != NULL)
						GUIManager->Destroy(PBar);
				}
				break;
			}
			
		case P_RepositionActor: // Reposition an actor
			{
				short RuntimeID = ShortFromStr(M->MessageData.Substr(1, 2));
				ActorInstance* AI = RuntimeIDList[RuntimeID];
				if(AI != 0)
				{
					// Move
					if(M->MessageData.Substr(0, 1) == NGin::CString("M"))
					{
						//TODO: Add a provision if this is 'me' becuase of a sector change!
						int PrevSectorX = AI->Position.SectorX;
						int PrevSectorZ = AI->Position.SectorZ;
						AI->Position.SectorX = M->MessageData.GetRealShort(3);
						AI->Position.SectorZ = M->MessageData.GetRealShort(5);
						AI->Position.X = M->MessageData.GetRealFloat(7);
						float Y = M->MessageData.GetRealFloat(11);
						AI->Position.Z = M->MessageData.GetRealFloat(15);
						AI->Position.FixValues();
						bool MoveCamera = CharFromStr(M->MessageData.Substr(19, 1)) > 0 ? true : false;

						AI->Destination = AI->Position;
						AI->Destination.Y = Y;
						
						// Ignore collision
						if(CharFromStr(M->MessageData.Substr(15, 1)) == 0)
							ResetEntity(AI->CollisionEN);

						int AISectorOffsetX = 0;
						int AISectorOffsetZ = 0;

						if(Me != NULL)
						{
							AISectorOffsetX = (int)AI->Position.SectorX - (int)Me->Position.SectorX;
							AISectorOffsetZ = (int)AI->Position.SectorZ - (int)Me->Position.SectorZ;
						}
						float AIX = AI->Position.X + ((float)AISectorOffsetX * RealmCrafter::SectorVector::SectorSize);
						float AIZ = AI->Position.Z + ((float)AISectorOffsetZ * RealmCrafter::SectorVector::SectorSize);

						PositionEntity(AI->CollisionEN, AIX, Y, AIZ);

						

						// Move the camera directly to the new spot, otherwise it will fly there
						if(MoveCamera == false)
						{
							ResetEntity(Cam);
							PositionEntity(Cam, AIX, Y, AIZ);
						}

						if(AI == Me)
						{
							if(PrevSectorX != Me->Position.SectorX || PrevSectorZ != Me->Position.SectorZ)
							{
								ResetEntity(Me->CollisionEN);
								PositionEntity(Me->CollisionEN,
									Me->Position.X,
									EntityY(Me->CollisionEN),
									Me->Position.Z);

								PlayerChangedSector(Me->Position.SectorX, Me->Position.SectorZ);
							}
						}

					// Rotate
					}else
					{
						AI->Yaw = FloatFromStr(M->MessageData.Substr(3));
						RotateEntity(AI->CollisionEN, 0.0f, AI->Yaw, 0.0f);
					}
				}
				break;
			}
			
		case P_FloatingNumber: // Floating number (for damage or whatever)
			{
				ushort RuntimeID = ShortFromStr(M->MessageData.Substr(0, 2));
				ActorInstance* AI = RuntimeIDList[RuntimeID];
				if(AI != 0)
				{
					int Amount = IntFromStr(M->MessageData.Substr(2, 4));
					uchar cR = CharFromStr(M->MessageData.Substr(6, 1));
					uchar cG = CharFromStr(M->MessageData.Substr(7, 1));
					uchar cB = CharFromStr(M->MessageData.Substr(8, 1));
					CreateFloatingNumber(AI, Amount, cR, cG, cB);
				}
				break;
			}
			
		case P_Projectile: // Projectile created
			{
				// Get source and target actor instance
				short RuntimeID = ShortFromStr(M->MessageData.Substr(0, 2));
				ActorInstance* AI = RuntimeIDList[RuntimeID];
				RuntimeID = ShortFromStr(M->MessageData.Substr(2, 2));
				ActorInstance* TargetAI = RuntimeIDList[RuntimeID];

				// Target is valid
				if(TargetAI != 0 && AI != 0)
				{
					// Get projectile data
					uint MeshID = ShortFromStr(M->MessageData.Substr(4, 2));
					int TexID1 = ShortFromStr(M->MessageData.Substr(6, 2));
					int TexID2 = ShortFromStr(M->MessageData.Substr(8, 2));
					bool Homing = CharFromStr(M->MessageData.Substr(10, 1)) > 0 ? true : false;
					float Speed = ((float)CharFromStr(M->MessageData.Substr(11, 1))) / 50.0f;
					int NameLen = CharFromStr(M->MessageData.Substr(12, 1));
					string Emitter1 = "";
					if(NameLen > 0)
						Emitter1 = M->MessageData.Substr(13, NameLen).c_str();
					string Emitter2 = M->MessageData.Substr(13 + NameLen).c_str();

					// Create it
					CreateProjectile(AI, TargetAI, MeshID, Homing, Speed, Emitter1, Emitter2, TexID1, TexID2);
				}
				break;
			}
			
		case P_Jump: // An actor has jumped
			{
				ActorInstance* AI = RuntimeIDList[ShortFromStr(M->MessageData)];
				if(AI != 0 && AI != Me)
				{
					PlayAnimation(AI, 3, 0.05f, Anim_Jump);
					AI->Position.Y = JumpStrength * Gravity;
				}
				break;
			}
			
		case P_ItemHealth: // Item health updated
			{
				int SlotI = CharFromStr(M->MessageData.Substr(0, 1));
				int Health = ShortFromStr(M->MessageData.Substr(M->MessageData.Length() - 2));
				if(Me->Inventory->Items[SlotI] != 0)
					Me->Inventory->Items[SlotI]->ItemHealth = Health;
				break;
			}
			
		case P_SelectScenery: // Owned scenery selected
			{
				Scenery* Sc = (Scenery*)IntFromStr(M->MessageData);
				if(Sc != 0)
					if(Sc->AnimationMode == 3 && Sc->SceneryID > 0)
						if(AnimTime(Sc->EN) > 0.0f)
							Animate(Sc->EN, 3, -1.0f, 0, 1.0f);
						else
							Animate(Sc->EN, 3, 1.0f, 0, 1.0f);
				break;
			}

		case P_ShaderConstant:
			{
				// Read in data
				ScriptShaderParameter* Param = new ScriptShaderParameter();
				unsigned short RuntimeID = M->MessageData.GetRealShort(0);
				Param->ParameterType = (ScriptShaderParameterType)M->MessageData.GetRealChar(2);
				Param->Data.X = M->MessageData.GetRealFloat(3);
				Param->Data.Y = M->MessageData.GetRealFloat(7);
				Param->Data.Z = M->MessageData.GetRealFloat(11);
				Param->Data.W = M->MessageData.GetRealFloat(15);
				int Len = M->MessageData.GetRealChar(19);
				Param->Name = M->MessageData.Substr(20, Len).c_str();

				// Get Actor
				ActorInstance* AI = RuntimeIDList[RuntimeID];
				if(AI != NULL)
				{
					bool Found = false;
					for(int i = 0; i < AI->ShaderParameters.size(); ++i)
					{
						if(AI->ShaderParameters[i]->Name.compare(Param->Name) == 0)
						{
							delete AI->ShaderParameters[i];
							AI->ShaderParameters[i] = Param;
							Found = true;
							break;
						}
					}

					if(!Found)
					{
						printf("Parameter: %s\n", Param->Name.c_str());
						AI->ShaderParameters.push_back(Param);
					}
				}

				break;
			}
			
		case P_AppearanceUpdate: // Actor appearance (clothes, face, etc.) changed
			{
				ActorInstance* AI = RuntimeIDList[ShortFromStr(M->MessageData.Substr(1, 2))];
				if(AI != 0)
				{
					switch(M->MessageData.Substr(0, 1).c_str()[0])
					{
						
					case 'C': // Entire actor
						{
							short ID = ShortFromStr(M->MessageData.Substr(M->MessageData.Length() - 2));
							AI->Actor = ActorList[ID];
							if(AI->Actor->Genders == 2 && AI->Gender != 1)
								AI->Gender = 1;
							if((AI->Actor->Genders == 1 || AI->Actor->Genders == 3) && AI->Gender != 0)
								AI->Gender = 0;
							float X = EntityX(AI->CollisionEN);
							float Y = EntityY(AI->CollisionEN);
							float Z = EntityZ(AI->CollisionEN);
							FreeActorInstance3D(AI);
							bool Result = LoadActorInstance3D(AI, 0.05f);
							if(Result == false)
								RuntimeError(string("Could not load actor mesh for ") + AI->Actor->Race + string("!"));
							AI->Position.Y = 0.0f;
							ResetEntity(AI->CollisionEN);
							PositionEntity(AI->CollisionEN, X, Y+ 2.0f, Z);
							
							if(AI == Me)
							{
								EntityType(Me->CollisionEN, C_Player);
								GUIManager->Destroy(Me->NametagEN);
								Me->NametagEN = 0;
								if(Me->TagEN != 0)
									GUIManager->Destroy(Me->TagEN);
								Me->TagEN = 0;
								uint Bonce = FindChild(Me->EN, "Head");
								if(Bonce == 0)
									RuntimeError(Me->Actor->Race + string(" actor mesh is missing a 'Head' joint!"));
							}
							break;
						}
						
					case 'G': // Gender
						{
							AI->Gender = Asc(M->MessageData.Substr(M->MessageData.Length() - 1).c_str());
							float X = EntityX(AI->CollisionEN);
							float Y = EntityY(AI->CollisionEN);
							float Z = EntityZ(AI->CollisionEN);
							FreeActorInstance3D(AI);
							bool Result = LoadActorInstance3D(AI, 0.05f);
							if(Result == false)
								RuntimeError(string("Could not load actor mesh for ") + AI->Actor->Race + string("!"));
							AI->Position.Y = 0.0f;
							ResetEntity(AI->CollisionEN);
							PositionEntity(AI->CollisionEN, X, Y, Z);
							
							break;
						}
						
					case 'D': // Beard
						{
							AI->Beard = Asc(M->MessageData.Substr(M->MessageData.Length() - 1).c_str());

							AI->ShowGubbinSet(&(AI->Actor->Beards[AI->Beard]), AI->BeardENs);

							break;
						}
						
					case 'H': // Hair
						{
							AI->Hair = Asc(M->MessageData.Substr(M->MessageData.Length() - 1).c_str());
							if(AI->Inventory->Items[SlotI_Hat] == 0)
								SetActorHat(AI, NULL);
							break;
						}
						
					case 'F': // Face
						{
							AI->FaceTex = Asc(M->MessageData.Substr(M->MessageData.Length() - 1).c_str());
							uint FaceTex = 0;
							int FaceSurface = 0;
							ActorTextureSet FaceSet(65535, 65535, 65535, 65535);

							// Repaint
							if(AI->Gender == 0)
							{
								FaceTex = AI->FaceTex;
								if(FaceTex >= AI->Actor->MaleFaceIDs.size())
									FaceTex = 0;

								//FaceTex = GetTexture(AI->Actor->MaleFaceIDs[FaceTex]);
								FaceSet = AI->Actor->MaleFaceIDs[FaceTex];
								

								if(CountSurfaces(AI->EN) > 1)
								{
									int FaceSurface = 1;
									string FirstSurfaceTex = stringToUpper(SurfaceTexture(AI->EN, 0, 0));

									// if the name of the first surface contains the word 'HEAD', the surfaces are backwards
									if(FirstSurfaceTex.find("HEAD", 0) != -1)
									{
										FaceSurface = 0;
									// if('HEAD' isn't found in the texture name of either surface, check if(the assigned texture is one of the face textures (rather than the dummy)
									}else if(stringToUpper(SurfaceTexture(AI->EN, 1, 0)).find("HEAD", 0) == -1)
									{
										for(int i = 0; i < AI->Actor->MaleFaceIDs.size(); ++i)
										{
											if(AI->Actor->MaleFaceIDs[i].Tex0 < 65535)
											{
												string Name2 = stringToUpper(GetTextureName(AI->Actor->MaleFaceIDs[i].Tex0));
												if(Name2 == FirstSurfaceTex)
												{
													FaceSurface = 0;
													break;
												}
											}
										}
									}
								}
							}else
							{
								FaceTex = AI->FaceTex;
								if(FaceTex >= AI->Actor->FemaleFaceIDs.size())
									FaceTex = 0;
								FaceSet = AI->Actor->FemaleFaceIDs[FaceTex];

								if(CountSurfaces(AI->EN) > 1 && FaceTex != 0)
								{
									int FaceSurface = 1;
									string FirstSurfaceTex = stringToUpper(SurfaceTexture(AI->EN, 0, 0));

									// if(the name of the first surface contains the word 'HEAD', the surfaces are backwards
									if(FirstSurfaceTex.find("HEAD", 0) != -1)
									{
										FaceSurface = 0;
									// if('HEAD' isn't found in the texture name of either surface, check if(the assigned texture is one of the face textures (rather than the dummy)
									}else if(stringToUpper(SurfaceTexture(AI->EN, 1, 0)).find("HEAD", 0) == -1)
									{
										for(int i = 0; i < AI->Actor->FemaleFaceIDs.size(); ++i)
										{
											if(AI->Actor->FemaleFaceIDs[i].Tex0 < 65535)
											{
												string Name2 = stringToUpper(GetTextureName(AI->Actor->FemaleFaceIDs[i].Tex0));
												if(Name2 == FirstSurfaceTex)
												{
													FaceSurface = 0;
													break;
												}
											}
										}
									}
								}
							}

							if(FaceSet.Tex0 < 65535)
								EntityTexture(AI->EN, GetTexture(FaceSet.Tex0), 0, FaceSurface);
							if(FaceSet.Tex1 < 65535)
								EntityTexture(AI->EN, GetTexture(FaceSet.Tex1), 1, FaceSurface);
							if(FaceSet.Tex2 < 65535)
								EntityTexture(AI->EN, GetTexture(FaceSet.Tex2), 2, FaceSurface);
							if(FaceSet.Tex3 < 65535)
								EntityTexture(AI->EN, GetTexture(FaceSet.Tex3), 3, FaceSurface);


							break;
						}
						
					case 'B': // Body
						{
							AI->BodyTex = Asc(M->MessageData.Substr(M->MessageData.Length() - 1).c_str());
							int BodySurface = 0;
							uint BodyTex;
							ActorTextureSet BodySet(65535, 65535, 65535, 65535);

							// Repaint
							if(CountSurfaces(AI->EN) < 2)
							{
								BodySurface = 0;
							}else
							{
								if(AI->Gender == 0)
								{
									BodyTex = AI->FaceTex;
									if(BodyTex >= AI->Actor->MaleBodyIDs.size())
										BodyTex = 0;
									BodySet = AI->Actor->MaleBodyIDs[BodyTex];

									if(BodyTex != 0)
									{
										BodySurface = 0;
										string FirstSurfaceTex = stringToUpper(SurfaceTexture(AI->EN, 0, 0));

										// if(the name of the first surface contains the word 'HEAD', the surfaces are backwards
										if(FirstSurfaceTex.find("HEAD", 0) != -1)
										{
											BodySurface = 1;
										// if('HEAD' isn't found in the texture name of either surface, check if(the assigned texture is one of the face textures (rather than the dummy)
										}else if(stringToUpper(SurfaceTexture(AI->EN, 1, 0)).find("HEAD", 0) == -1)
										{
											for(int i = 0; i <= AI->Actor->MaleFaceIDs.size(); ++i)
											{
												if(AI->Actor->MaleFaceIDs[i].Tex0 < 65535)
												{
													if(stringToUpper(GetTextureName(AI->Actor->MaleFaceIDs[i].Tex0)).compare(FirstSurfaceTex) == 0)
													{
														BodySurface = 1;
														break;
													}
												}
											}
										}
									}
								}else
								{
									BodyTex = AI->BodyTex;
									if(BodyTex >= AI->Actor->FemaleBodyIDs.size())
										BodyTex = 0;
									BodySet = AI->Actor->FemaleBodyIDs[BodyTex];

									if(BodyTex != 0)
									{
										BodySurface = 0;
										string FirstSurfaceTex = stringToUpper(SurfaceTexture(AI->EN, 0, 0));

										// if(the name of the first surface contains the word 'HEAD', the surfaces are backwards
										if(FirstSurfaceTex.find("HEAD", 0) != -1)
										{
											BodySurface = 1;
										// if('HEAD' isn't found in the texture name of either surface, check if(the assigned texture is one of the face textures (rather than the dummy)
										}else if(stringToUpper(SurfaceTexture(AI->EN, 1, 0)).find("HEAD", 0) == -1)
										{
											for(int i = 0; i < AI->Actor->FemaleFaceIDs.size(); ++i)
											{
												if(AI->Actor->FemaleFaceIDs[i].Tex0 < 65535)
												{
													if(stringToUpper(GetTextureName(AI->Actor->FemaleFaceIDs[i].Tex0)).compare(FirstSurfaceTex) == 0)
													{
														BodySurface = 1;
														break;
													}
												}
											}
										}
									}
								}
							}

							if(BodySet.Tex0 < 65535)
								EntityTexture(AI->EN, GetTexture(BodySet.Tex0), 0, BodySurface);
							if(BodySet.Tex1 < 65535)
								EntityTexture(AI->EN, GetTexture(BodySet.Tex1), 1, BodySurface);
							if(BodySet.Tex2 < 65535)
								EntityTexture(AI->EN, GetTexture(BodySet.Tex2), 2, BodySurface);
							if(BodySet.Tex3 < 65535)
								EntityTexture(AI->EN, GetTexture(BodySet.Tex3), 3, BodySurface);
							break;
						}
					}
				}
				break;
			}
			
		case P_PartyUpdate: // Party changed
			{
				int Offset = 0;

				std::string names[8];

				for(int i = 0; i < 8; ++i)
				{
					int NameLen = CharFromStr(M->MessageData.Substr(Offset, 1));
					names[i] = M->MessageData.Substr(Offset + 1, NameLen).c_str();
					Offset = Offset + 1 + NameLen;
				}

				PlayerParty->Update(names, 8);

				break;
			}
		case P_ActorEffect: // Actor effect added, removed, or updated
			{
				// Added
				if(M->MessageData.Substr(0, 1) == NGin::CString("A"))
				{
					EffectIcon* EI = new EffectIcon();
					EI->ID = IntFromStr(M->MessageData.Substr(1, 4));
					EI->TextureID = ShortFromStr(M->MessageData.Substr(5, 2));
					EI->Name = M->MessageData.Substr(7).c_str();
					UpdateEffectIcons();
				// Effect updated
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("E"))
				{
					int Att = CharFromStr(M->MessageData.Substr(1, 1));
					int Amount = IntFromStr(M->MessageData.Substr(2, 4));
					Me->Attributes->Value[Att] += Amount;
				// Removed
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("R"))
				{
					int ID = IntFromStr(M->MessageData.Substr(1, 4));

					foreachc(EIIt, EffectIcon, EffectIconList)
					{
						EffectIcon* EI = *EIIt;

						if(EI->ID == ID)
						{
							EffectIcon::Delete(EI);
							UpdateEffectIcons();
							break;
						}

						nextc(EIIt, EffectIcon, EffectIconList);
					}
					EffectIcon::Clean();

					for(int i = 0; i <= 39; ++i)
					{
						int Amount = IntFromStr(M->MessageData.Substr(5 + (i * 4), 4));
						Me->Attributes->Value[i] -= Amount;
					}
				}
				break;
			}
		case P_ScreenFlash: // Screen flash
			{
				uchar Red = CharFromStr(M->MessageData.Substr(0, 1));
				uchar Green = CharFromStr(M->MessageData.Substr(1, 1));
				uchar Blue = CharFromStr(M->MessageData.Substr(2, 1));
				float Alpha = ((float)CharFromStr(M->MessageData.Substr(3, 1))) / 255.0f;
				int Length = IntFromStr(M->MessageData.Substr(4, 4));
				int TexID = ShortFromStr(M->MessageData.Substr(8, 2));
				ScreenFlash(Red, Green, Blue, TexID, Length, Alpha);
				break;
			}
			
		case P_XPUpdate: // Received XP points, or somebody got a level-up
			{
				// The position of my XP bar changed
				if(M->MessageData.Substr(0, 1) == NGin::CString("B"))
				{
					Me->XPBarLevel = IntFromStr(M->MessageData.Substr(1));
					PlayerActionBar->UpdateXPBar(Me->XPBarLevel);
				// I received XP points
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("M"))
				{
					int XP = IntFromStr(M->MessageData.Substr(1));
					Me->XP += XP;
					Output(toString(XP) + string(" ") + LanguageString[LS_XPReceived], 255, 225, 100);
				// Another actor's level changed
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("L"))
				{
					ushort RuntimeID = ShortFromStr(M->MessageData.Substr(1, 2));
					short Level = ShortFromStr(M->MessageData.Substr(3, 2));
					ActorInstance* AI = RuntimeIDList[RuntimeID];
					if(AI != 0)
						AI->Level = Level;
				// My level has been changed
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("U"))
				{
					int Level = ShortFromStr(M->MessageData.Substr(1));
					Me->Level = Level;
					Me->XP = 0;
				}
				break;
			}
			
		case P_AnimateActor: // Animate actor
			{
				ushort RuntimeID = ShortFromStr(M->MessageData.Substr(0, 2));
				ActorInstance* A = RuntimeIDList[RuntimeID];
				if(A != 0)
				{
					int FixedSpeed = IntFromStr(M->MessageData.Substr(2, 1));
					float Speed = FloatFromStr(M->MessageData.Substr(3, 4));
					string Anim = M->MessageData.Substr(7).c_str();
					int ID = 0;
					if(A->Gender == 0)
						ID = FindAnimation(AnimList[A->Actor->MAnimationSet], Anim);
					else
						ID = FindAnimation(AnimList[A->Actor->FAnimationSet], Anim);
					
					if(ID > -1)
					{

						int AISectorOffsetX = 0;
						int AISectorOffsetZ = 0;

						if(Me != NULL)
						{
							AISectorOffsetX = (int)A->Position.SectorX - (int)Me->Position.SectorX;
							AISectorOffsetZ = (int)A->Position.SectorZ - (int)Me->Position.SectorZ;
						}
						float AIX = A->Position.X + ((float)AISectorOffsetX * RealmCrafter::SectorVector::SectorSize);
						float AIZ = A->Position.Z + ((float)AISectorOffsetZ * RealmCrafter::SectorVector::SectorSize);

						PlayAnimation(A, 3, Speed, ID, FixedSpeed);
						A->Destination = A->Position;
						A->Destination.X -= AIX - EntityX(A->CollisionEN);
						A->Destination.Z -= AIZ - EntityZ(A->CollisionEN);
						A->Destination.FixValues();
					}
				}
				break;
			}
			
		case P_Speech: // Actor speech
			{
				ushort ID = ShortFromStr(M->MessageData.Substr(0, 2));
				ActorInstance* AI = RuntimeIDList[(uint)ShortFromStr(M->MessageData.Substr(2, 2))];
				if(AI != 0)
					PlayActorSound(AI, ID);
				break;
			}
			
		case P_Sound: // Sound effect
			{
				ushort ID = ShortFromStr(M->MessageData.Substr(0, 2));
				string Name = GetSoundName(ID);
				uint S = GetSound(ID);
				if(Asc(Name.substr(Name.length() - 1)) > 0)
				{
					ActorInstance* AI = RuntimeIDList[(ushort)ShortFromStr(M->MessageData.Substr(2, 2))];
					if(AI != 0)
					{
						uint Channel = EmitSound(S, AI->CollisionEN);
						if(Channel != 0)
							ChannelVolume(Channel, RealmCrafter::Globals->DefaultVolume);
					}
				}else
				{
					uint Channel = PlaySound(S);
					if(Channel != 0)
						ChannelVolume(Channel, RealmCrafter::Globals->DefaultVolume);
				}
				break;
			}
			
		case P_Music: // Music
			{
				string Name = GetMusicName((ushort)ShortFromStr(M->MessageData.Substr(0, 2)));
				uint Channel = PlayMusic(string("Data\\Music\\") + Name);
				if(Channel != 0)
					ChannelVolume(Channel, RealmCrafter::Globals->DefaultVolume);
				break;
			}
			
		case P_CreateEmitter: // Particles effect
			{
				ushort TexID = ShortFromStr(M->MessageData.Substr(0, 2));
				int Time = IntFromStr(M->MessageData.Substr(2, 4));
				ushort RuntimeID = ShortFromStr(M->MessageData.Substr(6, 2));
				float XPos = FloatFromStr(M->MessageData.Substr(8, 4));
				float YPos = FloatFromStr(M->MessageData.Substr(12, 4));
				float ZPos = FloatFromStr(M->MessageData.Substr(16, 4));
				string Name = M->MessageData.Substr(20).c_str();
				ScriptedEmitter* Em = new ScriptedEmitter();
				Em->Length = Time;
				Em->StartTime = MilliSecs();
				uint Config = RP_LoadEmitterConfig((string("Data\\Emitter Configs\\") + Name + string(".rpc")).c_str(), GetTexture(TexID), Cam);
				if(Config == 0)
					RuntimeError(string("Could not load emitter: ") + Name + string("!"));
				Em->EN = RP_CreateEmitter(Config, 0.1f);
				ActorInstance* AI = RuntimeIDList[RuntimeID];
				if(AI != 0)
				{
					EntityParent(Em->EN, AI->CollisionEN);
					RotateEntity(Em->EN, EntityPitch(AI->CollisionEN), EntityYaw(AI->CollisionEN), EntityRoll(AI->CollisionEN));
					if(AI == Me)
						Em->AttachedToPlayer = true;
				}
				PositionEntity(Em->EN, XPos, YPos, ZPos);

				break;
			}
			
		case P_KnownSpellUpdate: // Known spell update
			{
				int Memorised = 0;

				// Spell added
				if(M->MessageData.Substr(0, 1) == NGin::CString("A"))
				{
					// Dreamora: 1.23
					if(Me->SpellLevels[Spells] == 0)
					{
						int Offset = 1;
						Me->SpellLevels[Spells] = M->MessageData.Substr(Offset, 2).GetRealShort();
						Spell* Sp = new Spell();
						Sp->ID = M->MessageData.Substr(Offset + 2, 2).GetRealShort();
						SpellsList[Sp->ID] = Sp;
						Me->KnownSpells[Spells] = Sp->ID;
						Sp->ThumbnailTexID = M->MessageData.Substr(Offset + 4, 2).GetRealShort();
						Sp->RechargeTime = M->MessageData.Substr(Offset + 6, 2).GetRealShort();
						Offset = Offset + 8;
						int NameLen = M->MessageData.Substr(Offset, 2).GetRealShort();
						Sp->Name = M->MessageData.Substr(Offset + 2, NameLen).c_str();
						Offset = Offset + 2 + NameLen;
						NameLen = M->MessageData.Substr(Offset, 2).GetRealShort();
						Sp->Description = M->MessageData.Substr(Offset + 2, NameLen).c_str();
						Offset = Offset + 2 + NameLen;
						if(M->MessageData.Substr(Offset, 2).GetRealChar() == 1 && Memorised < 10)
						{
							Me->MemorisedSpells[Memorised] = Spells;
							Memorised = Memorised + 1;
						}
						Spells = Spells + 1;
						SortSpells();
					}

				// Spell removed
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("D"))
				{
					string Name = M->MessageData.Substr(1).AsUpper().c_str();

					// Dreamora: 1.23
					// Remove the spell from spellbar
					bool Found = false;
					int Offset = 0;
					
					for(int it = 0; it < 36; ++it)
					{
						int slot = it;
						if(ActionBarSlots[slot] < 0)
						{
							if(RealmCrafter::Globals->RequireMemorise)
							{
								int Num = ActionBarSlots[slot] + 10;
								if(stringToUpper(SpellsList[Me->KnownSpells[Me->MemorisedSpells[Num]]]->Name).compare(Name) == 0)
									Found = true;
							}else
							{
								int Num = ActionBarSlots[slot] + 1000;
								if(stringToUpper(SpellsList[Me->KnownSpells[Num]]->Name).compare(Name) == 0)
									Found = true;
							}
							
							if(Found == true)
							{
								ActionBarSlots[slot] = 65535;
							}
							Found = false;
						}
					}


					// Remove memorised
					for(int i = 0; i <= 9; ++i)
						if(Me->MemorisedSpells[i] != 5000)
							if(stringToUpper(SpellsList[Me->KnownSpells[Me->MemorisedSpells[i]]]->Name).compare(Name) == 0)
								Me->MemorisedSpells[i] = 5000;
					
					// Remove known
					for(int i = 0; i <= 999; ++i)
						if(Me->SpellLevels[i] > 0)
							if(stringToUpper(SpellsList[Me->KnownSpells[i]]->Name).compare(Name) == 0)
							{
								Me->KnownSpells[i] = 0;
								Me->SpellLevels[i] = 0;
							}
					
				// Spell level update
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("L"))
				{
					int Level = IntFromStr(M->MessageData.Substr(1, 4));
					string Name = M->MessageData.Substr(5).AsUpper().c_str();
					for(int i = 0; i <= 999; ++i)
						if(Me->SpellLevels[i] > 0)
							if(stringToUpper(SpellsList[Me->KnownSpells[i]]->Name).compare(Name) == 0)
								Me->SpellLevels[i] = Level;
				}
				break;
			}
			
		case P_NameChange: // Name change
			{
				ushort RuntimeID = ShortFromStr(M->MessageData.Substr(0, 2));
				ActorInstance* A = RuntimeIDList[RuntimeID];
				if(A != 0)
				{
					int NameLen = CharFromStr(M->MessageData.Substr(2, 1));
					A->Name = M->MessageData.Substr(3, NameLen).c_str();
					A->Tag = M->MessageData.Substr(3 + NameLen).c_str();
					if(A != Me)
						CreateActorNametag(A);
				}
				break;
			}
			
		case P_GoldChange: // Gold change
			{
				int Amount = IntFromStr(M->MessageData.Substr(1));
				if(M->MessageData.Substr(0, 1) == NGin::CString("D"))
					Amount = 0 - Amount;
				Me->Gold += Amount;
				if(Me->Gold < 0)
					Me->Gold = 0;
				if(InventoryVisible == true)
					PlayerInventory->SetMoney(Money(Me->Gold));
				break;
			}
			
		case P_QuestLog: // Quest log update
			{
				// New entry
				if(M->MessageData.Substr(0, 1) == NGin::CString("N"))
				{
					for(int i = 0; i <= 499; ++i)
					{
						if(MyQuestLog->EntryName[i].length() == 0)
						{
							int NameLen = CharFromStr(M->MessageData.Substr(1, 1));
							MyQuestLog->EntryName[i] = M->MessageData.Substr(2, NameLen).c_str();
							int Offset = 2 + NameLen;
							NameLen = ShortFromStr(M->MessageData.Substr(Offset, 2));
							NGin::String paA = M->MessageData.Substr(Offset + 2, NameLen);
							MyQuestLog->EntryStatus[i] = std::string(paA.c_str(), paA.Length());
							Output(LanguageString[LS_QuestLogUpdate], 255, 225, 100);
							break;
						}
					}
				// Status update
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("U"))
				{
					int NameLen = CharFromStr(M->MessageData.Substr(1, 1));
					string Name = M->MessageData.Substr(2, NameLen).AsUpper().c_str();
					int Offset = 2 + NameLen;
					for(int i = 0; i <= 499; ++i)
					{
						if(stringToUpper(MyQuestLog->EntryName[i]).compare(Name) == 0)
						{
							NameLen = ShortFromStr(M->MessageData.Substr(Offset, 2));
							NGin::String paA = M->MessageData.Substr(Offset + 2, NameLen);
							MyQuestLog->EntryStatus[i] = std::string(paA.c_str(), paA.Length());
							Output(LanguageString[LS_QuestLogUpdate], 255, 225, 100);
							break;
						}
					}
				// Quest deleted
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("D"))
				{
					string Name = M->MessageData.Substr(1).AsUpper().c_str();
					for(int i = 0; i <= 499; ++i)
					{
						if(stringToUpper(MyQuestLog->EntryName[i]).compare(Name) == 0)
						{
							MyQuestLog->EntryName[i] = "";
							MyQuestLog->EntryStatus[i] = "";
							break;
						}
					}
				}
				if(QuestLogVisible)
					PlayerQuestLog->Update();
				break;
			}
			
		case P_StatUpdate: // Character stat update
			{
				ActorInstance* A = RuntimeIDList[(ushort)ShortFromStr(M->MessageData.Substr(1, 2))];
				if(M->MessageData.Substr(0, 1) == NGin::CString("A"))
				{
					int Attribute = CharFromStr(M->MessageData.Substr(3, 1));
					A->Attributes->Value[Attribute] = (int)(unsigned short)ShortFromStr(M->MessageData.Substr(4, 2));
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("M"))
				{
					int Attribute = CharFromStr(M->MessageData.Substr(3, 1));
					A->Attributes->Maximum[Attribute] = (int)(unsigned short)ShortFromStr(M->MessageData.Substr(4, 2));
				}else if(M->MessageData.Substr(0, 1) == NGin::CString("R"))
					A->Reputation = ShortFromStr(M->MessageData.Substr(3, 2));
				
				break;
			}
			
		case P_ScriptInput: // Scripted text input dialog
			{
				int NameLen = ShortFromStr(M->MessageData.Substr(5, 2));
				string Title = M->MessageData.Substr(7, NameLen).c_str();
				string Prompt = M->MessageData.Substr(7 + NameLen).c_str();
				CreateTextInput(Title, Prompt, CharFromStr(M->MessageData.Substr(4, 1)), IntFromStr(M->MessageData.Substr(0, 4)));
				break;
			}
			
		case P_Dialog: // Dialog message
			{
				switch(M->MessageData.Substr(0, 1).c_str()[0])
				{
					// New dialog
				case 'N':
					{
						ushort RuntimeID = ShortFromStr(M->MessageData.Substr(5, 2));
						
						ActorInstance* A = 0;
						if(RuntimeID < 65535)
							A = RuntimeIDList[RuntimeID];

						uint D = CreateDialog(M->MessageData.Substr(9).c_str(), A, IntFromStr(M->MessageData.Substr(1, 4)), ShortFromStr(M->MessageData.Substr(7, 2)));
						
#ifdef USE_250
						CRCE_Send(Connection, Connection, P_Dialog, NGin::CString("N") + M->MessageData.Substr(1, 4), true);
#else
						CRCE_Send(Connection, Connection, P_Dialog, NGin::CString("N") + M->MessageData.Substr(1, 4) + StrFromInt(D), true);
#endif

						Me->Destination = Me->Position;
						Me->Destination.X = EntityX(Me->CollisionEN);
						Me->Destination.Z = EntityZ(Me->CollisionEN);
						Me->Destination.FixValues();

						if(A != 0)
						{
							// Face player towards dialog actor
							PointEntity(Me->CollisionEN, A->CollisionEN);
							RotateEntity(Me->CollisionEN, 0.0f, EntityYaw(Me->CollisionEN) + 180.0f, 0.0f);
						}
						break;
					}
					// Dialog text
				case 'T':
					{
						uchar Red = CharFromStr(M->MessageData.Substr(1, 1));
						uchar Green = CharFromStr(M->MessageData.Substr(2, 1));
						uchar Blue = CharFromStr(M->MessageData.Substr(3, 1));
						uint D = IntFromStr(M->MessageData.Substr(4, 4));
						
						Dialog::Clean();
						foreachc(DIt, Dialog, DialogList)
						{
#ifdef USE_250
							if((*DIt)->ScriptHandle == D)
							{
								DialogOutput((uint)(*DIt), M->MessageData.Substr(8).c_str(), Red, Green, Blue);
								CRCE_Send(Connection, Connection, P_Dialog, NGin::CString("T") + StrFromInt((*DIt)->ScriptHandle), true);
							}
#else
							if((uint)(*DIt) == D)
							{
								DialogOutput(D, M->MessageData.Substr(8).c_str(), Red, Green, Blue);
								CRCE_Send(Connection, Connection, P_Dialog, NGin::CString("T") + StrFromInt(DialogScriptHandle(D)), true);
							}
#endif

							nextc(DIt, Dialog, DialogList)
						}


						break;
					}
					// Dialog options
				case 'O':
					{
						uint D = IntFromStr(M->MessageData.Substr(1, 4));

						Dialog::Clean();
						foreachc(DIt, Dialog, DialogList)
						{
#ifdef USE_250
							if((*DIt)->ScriptHandle == D)
							{
								int Offset = 5;
								while(Offset < M->MessageData.Length())
								{
									int NameLen = CharFromStr(M->MessageData.Substr(Offset, 1));
									AddDialogOption((uint)(*DIt), M->MessageData.Substr(Offset + 1, NameLen).c_str());
									Offset = Offset + NameLen + 1;
								}
							}
#else							
							if((uint)(*DIt) == D)
							{
								int Offset = 5;
								while(Offset < M->MessageData.Length())
								{
									int NameLen = CharFromStr(M->MessageData.Substr(Offset, 1));
									AddDialogOption(D, M->MessageData.Substr(Offset + 1, NameLen).c_str());
									Offset = Offset + NameLen + 1;
								}
							}
#endif

							nextc(DIt, Dialog, DialogList)
						}


						break;
					}
					// Close dialog
				case 'C':
					{
						uint DHandle = IntFromStr(M->MessageData.Substr(1));

						Dialog::Clean();
						foreachc(DIt, Dialog, DialogList)
						{
#ifdef USE_250
							if((*DIt)->ScriptHandle == DHandle)
								FreeDialog((uint)(*DIt));
#else
							if((uint)(*DIt) == DHandle)
								FreeDialog(DHandle);
#endif

							nextc(DIt, Dialog, DialogList)
						}
						Dialog::Clean();

						break;
					}
				}
				break;
			}
			
		case P_ActorDead: // Actor dead
			{
				ushort RuntimeID = ShortFromStr(M->MessageData.Substr(0, 2));
				ActorInstance* A = RuntimeIDList[RuntimeID];
				if(A != 0)
				{
					
					RemoveRadarMark( A );

					// Dismount if(required
					if(A->Mount != 0)
					{
						A->Mount->Rider = 0;
						A->Mount = 0;
					}

					if(A->Rider != 0)
					{
						A->Rider->Mount = 0;
						A->Rider = 0;
					}

					// if(I killed it, display message
					if(M->MessageData.Length() > 2)
					{
						RuntimeID = ShortFromStr(M->MessageData.Substr(2, 2));
						ActorInstance* Killer = RuntimeIDList[RuntimeID];
						string Name = A->Name;
						Name = trim(Name);
						if(Name.length() == 0)
							Name = A->Actor->Race;
						
						if(Killer == Me)
							Output(LanguageString[LS_YouKilled] + string(" ") + Name + string("!"), 0, 255, 0);
					}
					// Death sound/animation
					PlayActorSound(A, Speech_Death);
					Animate(A->EN, 0);
					PlayAnimation(A, 3, 0.02f, Rand(Anim_FirstDeath, Anim_LastDeath));

					// Remove the actor
					A->Attributes->Value[RealmCrafter::Globals->HealthStat] = 0;
					A->AIMode = 501; // For fade out
					GUIManager->Destroy(A->NametagEN);
					A->NametagEN = 0;
					if(A->TagEN != 0)
						GUIManager->Destroy(A->TagEN);
					A->TagEN = 0;
					EntityType(A->CollisionEN, 0);
					if(Handle(A) == PlayerTarget)
					{
						PlayerTarget = 0;
						RealmCrafter::Globals->AttackTarget = false;
					}

					// Free any dialogs
					foreachc(DiIt, Dialog, DialogList)
					{
						Dialog* Di = *DiIt;

						if(Di->ActorInstance == A)
							FreeDialog(Handle(Di));

						nextc(DiIt, Dialog, DialogList);
					}
				}
				break;
			}
			
		case P_AttackActor: // Actor attacked
			{
				ushort RuntimeID = ShortFromStr(M->MessageData.Substr(1, 2));
				ActorInstance* A = RuntimeIDList[RuntimeID];
				if(A != 0)
				{
					// I attacked someone else
					if(M->MessageData.Substr(0, 1) == NGin::CString("H"))
					{
						AnimateActorAttack(Me);
						PlayActorSound(Me, Rand(Speech_Attack1, Speech_Attack2));
						int Damage = ShortFromStr(M->MessageData.Substr(3, 2)) - 1;
						string DType = DamageTypes[(uint)CharFromStr(M->MessageData.Substr(5, 1))];
						// And hit them
						if(Damage > 0)
						{
							CombatDamageOutput(A, Damage, DType);
							PlayAnimation(A, 3, 0.035f, Rand(Anim_FirstHit, Anim_LastHit));
							PlayActorSound(A, Rand(Speech_Hit1, Speech_Hit2));
							if(A->Actor->BloodTexID > 0)
							{
								BloodSpurt* B = new BloodSpurt();
								B->Timer = MilliSecs();
								B->EmitterEN = RP_CreateEmitter(A->Actor->BloodTexID);
								PositionEntity(B->EmitterEN, EntityX(A->CollisionEN), EntityY(A->CollisionEN), EntityZ(A->CollisionEN));
								PointEntity(B->EmitterEN, Me->CollisionEN);
								MoveEntity(B->EmitterEN, 0.0f, 0.0f, 1.0f);
							}
						// And missed
						}else if(Damage < 0)
						{
							CombatDamageOutput(A, 0, "0");
							AnimateActorParry(A);
						}
					// Someone else attacked me
					}else if(M->MessageData.Substr(0, 1) == NGin::CString("Y"))
					{
						int Damage = (int)(unsigned short)ShortFromStr(M->MessageData.Substr(3, 2)) - 1;

						string DType = DamageTypes[(uint)CharFromStr(M->MessageData.Substr(5, 1))];

						// And hit me
						if(Damage > 0)
						{
							CombatDamageOutput(A, -Damage, DType);
							Me->Attributes->Value[RealmCrafter::Globals->HealthStat] -= Damage;
							AnimateActorAttack(A);
							PlayAnimation(Me, 3, 0.035f, Rand(Anim_FirstHit, Anim_LastHit));
							PlayActorSound(A, Rand(Speech_Attack1, Speech_Attack2));
							PlayActorSound(Me, Rand(Speech_Hit1, Speech_Hit2));
							if(Me->Actor->BloodTexID > 0)
							{
// 								BloodSpurt* B = new BloodSpurt();
// 								B->Timer = MilliSecs();
// 								B->EmitterEN = RP_CreateEmitter(Me->Actor->BloodTexID);
// 								PositionEntity(B->EmitterEN, EntityX(Me->CollisionEN), EntityY(Me->CollisionEN), EntityZ(Me->CollisionEN));
// 								PointEntity(B->EmitterEN, A->CollisionEN);
// 								MoveEntity(B->EmitterEN, 0.0f, 0.0f, 1.0f);
							}
						// And missed
						}else if(Damage < 0)
						{
							CombatDamageOutput(A, 0, "1");
							AnimateActorAttack(A);
							AnimateActorParry(Me);
							PlayActorSound(A, Rand(Speech_Attack1, Speech_Attack2));
							PointEntity(A->CollisionEN, Me->CollisionEN);
							RotateEntity(A->CollisionEN, 0.0f, EntityYaw(A->CollisionEN) + 180.0f, 0.0f);
						}
					// Someone else attacked someone else
					}else
					{
						RuntimeID = ShortFromStr(M->MessageData.Substr(3, 2));
						ActorInstance* A2 = RuntimeIDList[RuntimeID];
						if(A2 != 0)
						{
							AnimateActorAttack(A);
							PlayAnimation(A2, 3, 0.035f, Rand(Anim_FirstHit, Anim_LastHit));
							PlayActorSound(A, Rand(Speech_Attack1, Speech_Attack2));
							PlayActorSound(A2, Rand(Speech_Hit1, Speech_Hit2));
							//if(A2->Actor->BloodTexID > 0)
							//{
							//	BloodSpurt* B = new BloodSpurt();
							//	B->Timer = MilliSecs();
							//	B->EmitterEN = RP_CreateEmitter(A2->Actor->BloodTexID);
							//	PositionEntity(B->EmitterEN, EntityX(A2->CollisionEN), EntityY(A2->CollisionEN), EntityZ(A2->CollisionEN));
							//	PointEntity(B->EmitterEN, A->CollisionEN);
							//	MoveEntity(B->EmitterEN, 0.0f, 0.0f, 1.0f);
							//}
							PointEntity(A->CollisionEN, A2->CollisionEN);
							RotateEntity(A->CollisionEN, 0.0f, EntityYaw(A->CollisionEN) + 180.0f, 0.0f);
						}
					}
				}
				break;
			}
			
		case P_BubbleMessage: // Chat bubble message
			{
				ActorInstance* AI = RuntimeIDList[(uint)ShortFromStr(M->MessageData.Substr(0, 2))];
				if(AI != 0)
				{
					uchar Red = M->MessageData.Substr(2, 1).GetRealChar(0);
					uchar Green = M->MessageData.Substr(3, 1).GetRealChar(0);
					uchar Blue = M->MessageData.Substr(4, 1).GetRealChar(0);
					BubbleOutput(M->MessageData.Substr(5).c_str(), Red, Green, Blue, AI);
				}
				break;
			}

		case P_ChatTab:
			{
				if(M->MessageData.GetRealChar(0) == 'O')
				{
					int TabID = M->MessageData.GetRealChar(1);
					int TabWidth = M->MessageData.GetRealChar(2);
					int NameLength = M->MessageData.GetRealChar(3);
					std::string Name = M->MessageData.Substr(4, NameLength).c_str();

					RealmCrafter::Globals->ChatBox->AddTab(TabID, Name, TabWidth);

				}else if(M->MessageData.GetRealChar(0) == 'C')
				{
					int ID = M->MessageData.GetRealChar(1);
					RealmCrafter::Globals->ChatBox->RemoveTab(ID);
				}else if(M->MessageData.GetRealChar(0) == 'S')
				{
					int ID = M->MessageData.GetRealChar(1);
					RealmCrafter::Globals->ChatBox->SwitchToTab(ID);
				}

				break;
			}
			
		case P_ChatMessage: // Chat message
			{
				int TabID = M->MessageData.GetRealChar(0);

				// Special colours
				if(M->MessageData.GetRealChar(1) == 254)
					RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(2).c_str()), NGin::Math::Color(255, 255, 0));
				else if(M->MessageData.GetRealChar(1) == 253)
					RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(2).c_str()), NGin::Math::Color(255, 50, 50));
				else if(M->MessageData.GetRealChar(1) == 252)
					RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(2).c_str()), NGin::Math::Color(200, 10, 200));
				else if(M->MessageData.GetRealChar(1) == 251)
					RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(2).c_str()), NGin::Math::Color(20, 220, 50));
				else if(M->MessageData.GetRealChar(1) == 250)
				{
					int Red = M->MessageData.GetRealChar(2);
					int Green = M->MessageData.GetRealChar(3);
					int Blue = M->MessageData.GetRealChar(4);
					RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(5).c_str()), NGin::Math::Color(Red, Green, Blue));
				// Normal
				}else
				{

					// Use a chat bubble
					if(M->MessageData.Substr(1, 1) == NGin::CString("<") && UseBubbles > 0)
					{
						// Find actor
						bool Found = false;
						int Pos = M->MessageData.Instr(">", 1);
						ActorInstance* AI = 0;
						if(Pos >= 0)
						{
							string Name = M->MessageData.Substr(2, Pos - 2).c_str();
							AI = FindPlayerFromName(Name);
							if(AI != 0)
								Found = true;
						}

						// Found, create bubble
						if(Found == true)
						{
							if(AI == Me)
							{
								BubbleOutput(M->MessageData.Substr(Pos + 2).c_str(), 0, 128, 255, AI, (UseBubbles - 2) == 0);
							}
							else
							{
								BubbleOutput(M->MessageData.Substr(Pos + 2).c_str(), BubblesR, BubblesG, BubblesB, AI, (UseBubbles - 2) == 0);
							}
						}
						else // No actor found, use normal text output
						{
							if(M->MessageData.Instr(NGin::CString("<") + NGin::CString(Me->Name.c_str()) + NGin::CString(">"), 1) == 0)
							{
								RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(1).c_str()), NGin::Math::Color(0, 128, 255));
							}
							else
							{
								RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(1).c_str()), NGin::Math::Color(255, 255, 255));
							}
						}
					// Normal text output
					}else
						if(M->MessageData.Instr(NGin::CString("<") + NGin::CString(Me->Name.c_str()) + NGin::CString(">"), 1) == 0)
							RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(1).c_str()), NGin::Math::Color(0, 128, 255));
						else
							RealmCrafter::Globals->ChatBox->Output(TabID, std::string(M->MessageData.Substr(1).c_str()), NGin::Math::Color(255, 255, 255));

				}
				break;
			}
			
		case P_WeatherChange: // Weather change
			{
// 				int ServerArea = IntFromStr(M->MessageData.Substr(0, 4));
// 				if(ServerArea == CurrentAreaID)
// 					Environment->SetWeather((int)CharFromStr(M->MessageData.Substr(4, 1)));
				Environment->ResetEnvironment(M->MessageData);
				break;
			}
			
		case P_InventoryUpdate: // Inventory update
			{
				switch(M->MessageData.Substr(0, 1).c_str()[0])
				{
					// An item health has changed
				case 'H':
					{
						int SlotI = CharFromStr(M->MessageData.Substr(1, 1));
						int Amount = CharFromStr(M->MessageData.Substr(2, 1));
						if(Me->Inventory->Items[SlotI] != 0)
							Me->Inventory->Items[SlotI]->ItemHealth = Amount;
						
						break;
					}
					// An item has been taken from my inventory
				case 'T':
					{
						int SlotI = CharFromStr(M->MessageData.Substr(1, 1));
						int Amount = ShortFromStr(M->MessageData.Substr(2, 2));
						if(Me->Inventory->Items[SlotI] != 0)
						{
							Me->Inventory->Amounts[SlotI] -= Amount;

							// All gone
							if(Me->Inventory->Amounts[SlotI] <= 0)
							{
								// Remove item
								Me->Inventory->Amounts[SlotI] = 0;
								FreeItemInstance(Me->Inventory->Items[SlotI]);
								Me->Inventory->Items[SlotI] = 0;

								// Visual stuff
								if(InventoryVisible == true)
								{
									if(RealmCrafter::Globals->CurrentMouseItem != NULL && RealmCrafter::Globals->CurrentMouseItem->GetItemID() != 65535)
									{
										int SourceSlot = PlayerInventory->GetSlot(reinterpret_cast<RealmCrafter::IItemButton*>(RealmCrafter::Globals->CurrentMouseItem->GetSourceData()));
										if(SourceSlot == SlotI)
										{
											delete RealmCrafter::Globals->CurrentMouseItem;
											RealmCrafter::Globals->CurrentMouseItem = NULL;
										}
									}
								}
								UpdateActorItems(Me);
							// Not all gone but update the amount
							}else if(InventoryVisible == true)
							{

							}
							
						}
						break;
					}
					// I received a dropped item
				case 'R':
					{
						foreachc(DItemIt, DroppedItem, DroppedItemList)
						{
							DroppedItem* DItem = *DItemIt;

							if(DItem->ServerHandle == IntFromStr(M->MessageData.Substr(1, 4)))
							{
								// Put in slot
								int i = CharFromStr(M->MessageData.Substr(5, 1));
								if(Me->Inventory->Items[i] != 0)
								{
									FreeItemInstance(Me->Inventory->Items[i]);
									Me->Inventory->Items[i] = 0;
								}
								else
									Me->Inventory->Amounts[i] = 0;
								
								Me->Inventory->Items[i] = DItem->Item;
								Me->Inventory->Amounts[i] += DItem->Amount;

								// Visual stuff
								UpdateActorItems(Me);

								// Inform user and delete dropped item
								if(DItem->Amount > 1)
									Output(LanguageString[LS_PickedUpItem] + string(" ") + DItem->Item->Item->Name + string(" (x") + toString(DItem->Amount) + string(")"), 0, 255, 0);
								else
									Output(LanguageString[LS_PickedUpItem] + string(" ") + DItem->Item->Item->Name, 0, 255, 0);
								
								FreeEntity(DItem->EN);
								DroppedItem::Delete(DItem);
								break;
							}
						
							nextc(DItemIt, DroppedItem, DroppedItemList);
						}
						DroppedItem::Clean();

						break;
					}
					// Dropped item has been picked up by someone else
				case 'P':
					{
						foreachc(DItemIt, DroppedItem, DroppedItemList)
						{
							DroppedItem* DItem = *DItemIt;

							if(DItem->ServerHandle == IntFromStr(M->MessageData.Substr(1, 4)))
							{
								FreeEntity(DItem->EN);
								ItemInstance::Delete(DItem->Item);
								DroppedItem::Delete(DItem);
								break;
							}
							
							nextc(DItemIt, DroppedItem, DroppedItemList);
						}
						ItemInstance::Clean();
						DroppedItem::Clean();

						break;
					}
					// Item dropped
				case 'D':
					{
						// Create dropped item
						DroppedItem* DItem = new DroppedItem();
						DItem->TYPE = TYPE_DroppedItem;

						DItem->Amount = ShortFromStr(M->MessageData.Substr(1, 2));

						DItem->Position.SectorX = M->MessageData.GetRealShort(3);
						DItem->Position.SectorZ = M->MessageData.GetRealShort(5);
						DItem->Position.X = M->MessageData.GetRealShort(7);
						DItem->Position.Y = M->MessageData.GetRealShort(11);
						DItem->Position.Z = M->MessageData.GetRealShort(15);
						DItem->ServerHandle = IntFromStr(M->MessageData.Substr(19, 4));
						DItem->Item = ItemInstanceFromString(M->MessageData.Substr(23));

						float Scale = 0.0f;

						// Find suitable mesh
						if(DItem->Item != 0)
						{
							DItem->EN = 0;
// 							if(DItem->Item->Item->MMeshID < 65535)
// 							{
// 								DItem->EN = GetMesh(DItem->Item->Item->MMeshID);
// 								if(DItem->EN != 0)
// 									Scale = LoadedMeshScales[DItem->Item->Item->MMeshID] * 0.05f;
// 							}else if(DItem->Item->Item->FMeshID < 65535)
// 							{
// 								DItem->EN = GetMesh(DItem->Item->Item->FMeshID);
// 								if(DItem->EN != 0)
// 									Scale = LoadedMeshScales[DItem->Item->Item->FMeshID] * 0.05f;
// 							}
						}

						float Y = 0.0f;
						MeshMinMaxVertices* MMV = 0;

						// Use default loot bag mesh
						if(DItem->EN == 0)
						{
							DItem->EN = CopyEntity(LootBagEN);
							MMV = Mesh_MinMaxVertices(DItem->EN);
							Y = 0.0f;
						// Custom item mesh has been found - set scale and get offset for correct ground level position
						}else
						{
							ScaleEntity(DItem->EN, Scale, Scale, Scale);
							MMV = Mesh_MinMaxVertices(DItem->EN);
							Y = 0.0f - (MMV->MinY * Scale);

						}
						float MaxLength = MMV->MaxX - MMV->MinX;
						if(MMV->MaxZ - MMV->MinZ > MaxLength)
							MaxLength = ((MMV->MaxZ - MMV->MinZ) + MaxLength) / 2.0f;
						
						SetCollisionMesh(DItem->EN);
						//EntityRadius(DItem->EN, 1.0f);
						//EntityRadius(DItem->EN, (MaxLength * EntityScaleX(DItem->EN)) / 2.0f, ((MMV->MaxY - MMV->MinY) * EntityScaleY(DItem->EN)) / 2.0f);

						MeshMinMaxVertices::Delete(MMV);
						MeshMinMaxVertices::Clean();

						int AISectorOffsetX = 0;
						int AISectorOffsetZ = 0;

						if(Me != NULL)
						{
							AISectorOffsetX = (int)DItem->Position.SectorX - (int)Me->Position.SectorX;
							AISectorOffsetZ = (int)DItem->Position.SectorZ - (int)Me->Position.SectorZ;
						}
						float AIX = DItem->Position.X + ((float)AISectorOffsetX * RealmCrafter::SectorVector::SectorSize);
						float AIZ = DItem->Position.Z + ((float)AISectorOffsetZ * RealmCrafter::SectorVector::SectorSize);


						// Position mesh entity
						SetPickModes();
						uint Result = LinePick(AIX, DItem->Position.Y + 10.0f, AIZ, 0.0f, -1000.0f, 0.0f);
						if(Result != 0)
						{
							PositionEntity(DItem->EN, PickedX(), PickedY() + Y, PickedZ());
						}else
						{
							PositionEntity(DItem->EN, AIX, DItem->Position.Y + Y, AIZ);
						}
						
						RotateEntity(DItem->EN, 0.0f, Rnd(-180.0f, 180.0f), 0.0f);
						NameEntity(DItem->EN, toString(Handle(DItem)));
						break;
					}
					// Update for another actor
				case 'O':
					{
						ushort RuntimeID = ShortFromStr(M->MessageData.Substr(1, 2));
						ActorInstance* A = RuntimeIDList[RuntimeID];
						if(A != 0)
						{
							ushort WeaponID = ShortFromStr(M->MessageData.Substr(3, 2));
							ushort ShieldID = ShortFromStr(M->MessageData.Substr(5, 2));
							ushort ChestID = ShortFromStr(M->MessageData.Substr(7, 2));
							ushort HatID = ShortFromStr(M->MessageData.Substr(9, 2));
							if(A->Inventory->Items[SlotI_Weapon] != 0)
							{
								FreeItemInstance(A->Inventory->Items[SlotI_Weapon]);
								A->Inventory->Items[SlotI_Weapon] = 0;
							}

							if(A->Inventory->Items[SlotI_Shield] != 0)
							{
								FreeItemInstance(A->Inventory->Items[SlotI_Shield]);
								A->Inventory->Items[SlotI_Shield] = 0;
							}

							if(A->Inventory->Items[SlotI_Chest] != 0)
							{
								FreeItemInstance(A->Inventory->Items[SlotI_Chest]);
								A->Inventory->Items[SlotI_Chest] = 0;
							}

							if(A->Inventory->Items[SlotI_Hat] != 0)
							{
								FreeItemInstance(A->Inventory->Items[SlotI_Hat]);
								A->Inventory->Items[SlotI_Hat] = 0;
							}

							if(WeaponID < 65535)
								A->Inventory->Items[SlotI_Weapon] = CreateItemInstance(ItemList[WeaponID]);
							if(ShieldID < 65535)
								A->Inventory->Items[SlotI_Shield] = CreateItemInstance(ItemList[ShieldID]);
							if(ChestID < 65535)
								A->Inventory->Items[SlotI_Chest] = CreateItemInstance(ItemList[ChestID]);
							if(HatID < 65535)
								A->Inventory->Items[SlotI_Hat] = CreateItemInstance(ItemList[HatID]);
							UpdateActorItems(A);
// 							for(int i = 0; i <= 5; ++i)
// 								if(CharFromStr(M->MessageData.Substr(11 + i, 1)) > 0)
// 									ShowGubbin(A, i);
// 								else
// 									HideGubbin(A, i);
						}
						break;
					}
					// Given an item
				case 'G':
					{
						ushort ItemID = ShortFromStr(M->MessageData.Substr(5, 2));
						short Amount = ShortFromStr(M->MessageData.Substr(7, 2));
						// Find free slot
						bool Found = false;
						ItemInstance* II = CreateItemInstance(ItemList[ItemID]);
						for(int i = 0; i <= 49; ++i)
						{
							if(Me->Inventory->Items[i] == 0 || (ItemInstancesIdentical(II, Me->Inventory->Items[i]) && II->Item->Stackable == true && i >= SlotI_Backpack))
							{
								if(SlotsMatch(ItemList[ItemID], i) && ActorHasSlot(Me->Actor, i, ItemList[ItemID]))
								{
									// Put in slot
									if(Me->Inventory->Items[i] != 0)
									{
										FreeItemInstance(Me->Inventory->Items[i]);
										Me->Inventory->Items[i] = 0;
									}
									else
										Me->Inventory->Amounts[i] = 0;
									
									Me->Inventory->Items[i] = II;
									Me->Inventory->Amounts[i] += Amount;
									UpdateActorItems(Me);
									// Reply to server
									//string Pa = M->MessageData.Substr(1, 4);
									//Pa.AppendRealChar(i);
									//CRCE_Send(Connection, Connection, P_InventoryUpdate, string("GY") + Pa, true);
									NGin::CString Sm = M->MessageData.Substr(1, 4);
									NGin::CString Pad = "GY";
									NGin::CString Pa = NGin::CString::FormatReal("ssc", &Pad, &Sm, i);
									CRCE_Send(Connection, Connection, P_InventoryUpdate, Pa, true);
									Found = true;
									break;
								}
							}
						}
						// if(not found, tell server
						if(Found == false)
						{
							FreeItemInstance(II);
							CRCE_Send(Connection, Connection, P_InventoryUpdate, NGin::CString("GN") + M->MessageData.Substr(1, 4), true);
						}
						break;
					}
				}
				break;
			}
			
		case P_StandardUpdate: // A standard update for an actor instance
			{
				ushort RuntimeID = ShortFromStr(M->MessageData.Substr(0, 2));
				ActorInstance* A = RuntimeIDList[RuntimeID];
				if(A != 0 && A->IgnoreUpdate == false)// && A != Me)
				{
					// Get position
					int PrevSectorX = A->Position.SectorX;
					int PrevSectorZ = A->Position.SectorZ;

					A->Position.SectorX = M->MessageData.GetRealShort(2);
					A->Position.SectorZ = M->MessageData.GetRealShort(4);
					A->Position.X = M->MessageData.GetRealFloat(6);
					float Y = M->MessageData.GetRealFloat(10);
					A->Position.Z = M->MessageData.GetRealFloat(14);
					A->Position.FixValues();

					

					// Get mount if(the actor has one
					RuntimeID = ShortFromStr(M->MessageData.Substr(36, 2));
					if(RuntimeID != 0)
					{
						ActorInstance* Mount = RuntimeIDList[RuntimeID];
						if(Mount != 0)
						{
							// Actor did not have a mount until now
							if(A->Mount != Mount)
							{
								A->Mount = Mount;
								Mount->Rider = A;
								if(GetEntityType(A->CollisionEN) != C_None)
									EntityType(A->CollisionEN, C_None);
								if(A == Me)
								{
									if(Handle(Mount) == PlayerTarget)
									{
										PlayerTarget = 0;
										RealmCrafter::Globals->AttackTarget = false;
									}
									PlayActorSound(Mount, Rand(Speech_Greet1, Speech_Greet2));
								}
								Mount->Position.Y = 0.0f;
							}
						}
					// Unmount a previously mounted actor
					}else if(A->Mount != 0)
					{
						if(A == Me)
						{
							if(GetEntityType(A->CollisionEN) != C_Player)
								EntityType(A->CollisionEN, C_Player);
							PlayActorSound(A->Mount, Rand(Speech_Bye1, Speech_Bye2));
						}else if(A->Actor->PolyCollision == false)
						{
							if(GetEntityType(A->CollisionEN) != C_Actor)
								EntityType(A->CollisionEN, C_Actor);
						}else
						{
							if(GetEntityType(A->CollisionEN) != C_ActorTri1)
								EntityType(A->CollisionEN, C_ActorTri1);
						}
						Animate(A->EN, 0);
						A->Mount->Rider = 0;
						A->Mount = 0;
					}

					// For players other than me, read in movement details
					if(A != Me && A != Me->Mount)
					{
						A->IsRunning = CharFromStr(M->MessageData.Substr(18, 1));
						bool OldWalkBack = A->WalkingBackward;
						bool OldStrafeLeft = A->StrafingLeft;
						bool OldStrafeRight = A->StrafingRight;
						unsigned char Flags = CharFromStr(M->MessageData.Substr(19, 1));
						A->WalkingBackward = (Flags & 1) > 0;
						A->StrafingLeft = (Flags & 2) > 0;
						A->StrafingRight = (Flags & 4) > 0;
						
						short YawShort = (float)FloatFromStr(M->MessageData.Substr(38, 4));
						float Yaw = (float)YawShort;
						//Yaw /= 32767.0f;
						//Yaw *= 180.0f;

						A->LastUpdateTime = MilliSecs();
						A->PrevYaw = A->NewYaw;
						A->NewYaw = Yaw;
						A->Destination.SectorX = M->MessageData.GetRealShort(20);
						A->Destination.SectorZ = M->MessageData.GetRealShort(22);
						A->Destination.X = M->MessageData.GetRealFloat(24);
						A->Destination.Y = M->MessageData.GetRealFloat(28);
						A->Destination.Z = M->MessageData.GetRealFloat(32);

						if(A->Actor->Environment == Environment_Fly)
							A->Position.Y = Y;
						
						if(OldWalkBack != A->WalkingBackward || OldStrafeLeft != A->StrafingLeft || OldStrafeRight != A->StrafingRight)
							if(CurrentSeq(A) == Anim_Walk || CurrentSeq(A) == Anim_StrafeLeft || CurrentSeq(A) == Anim_StrafeRight)
								Animate(A->EN, 0);
						
					// For myself only, update my energy level if(the energy stat is present
					}else if(RealmCrafter::Globals->EnergyStat > -1)
						A->Attributes->Value[RealmCrafter::Globals->EnergyStat] = (int)(unsigned short)ShortFromStr(M->MessageData.Substr(40, 2));


					if(A == Me)
					{
						if(PrevSectorX != Me->Position.SectorX || PrevSectorZ != Me->Position.SectorZ)
						{
							ResetEntity(Me->CollisionEN);
							PositionEntity(Me->CollisionEN,
								Me->Position.X,
								EntityY(Me->CollisionEN),
								Me->Position.Z);

							PlayerChangedSector(Me->Position.SectorX, Me->Position.SectorZ);
						}
					}
				}else
				{
					printf("Actor with RuntimeID '%i' not found!\n", RuntimeID);
				}
				break;
			}
			
		case P_ActorGone: // An actor instance has left my area
			{
				ushort RuntimeID = ShortFromStr(M->MessageData);
				ActorInstance* A = RuntimeIDList[RuntimeID];
				if(A != 0)
				{
					if(A != Me)
					{
						RemoveRadarMark( A );

						// Free projectiles targeted at this actor
						foreachc(ProjIIt, ProjectileInstance, ProjectileInstanceList)
						{
							ProjectileInstance* ProjI = *ProjIIt;

							if(ProjI->Target == A)
								FreeProjectileInstance(ProjI);
							
							nextc(ProjIIt, ProjectileInstance, ProjectileInstanceList);
						}
						ProjectileInstance::Clean();

						// Display exit message
						if(A->RNID > 0)
							Output(LanguageString[LS_PlayerLeftZone] + string(" ") + A->Name, 255, 0, 0);

						// Free actor instance
						SafeFreeActorInstance(A);
					}
				}
				break;
			}
			
		case P_NewActor: // A new actor instance has entered my area
			{
				ActorInstance* A = ActorInstanceFromString(M->MessageData);
				if(A != 0)
				{
					if ( A->TeamID == Me->TeamID ) AddRadarMark( A, idFriendly );
					else AddRadarMark( A, idEnemy );

					bool Result = LoadActorInstance3D(A, 0.05f);
					if(Result == false)
						RuntimeError(string("Could not load actor mesh for ") + A->Actor->Race + string("!"));
					ResetEntity(A->CollisionEN);

					int AISectorOffsetX = 0;
					int AISectorOffsetZ = 0;

					if(Me != NULL)
					{
						AISectorOffsetX = (int)A->Position.SectorX - (int)Me->Position.SectorX;
						AISectorOffsetZ = (int)A->Position.SectorZ - (int)Me->Position.SectorZ;
					}
					float AX = A->Position.X + ((float)AISectorOffsetX * RealmCrafter::SectorVector::SectorSize);
					float AZ = A->Position.Z + ((float)AISectorOffsetZ * RealmCrafter::SectorVector::SectorSize);

					PositionEntity(A->CollisionEN, AX, A->Position.Y + 10.0f, AZ);

					printf("NewActorPos: %f, %f, %f\n", AX, A->Position.Y + 10.0f, AZ);


					//if(A->Actor->Environment != Environment_Fly)
					//{
					//	SetPickModes();
					//	float Height = LoadedMeshScales[A->Actor->MeshIDs[A->Gender]] * A->Actor->Scale * MeshHeight(A->EN) * 0.05f;
					//	uint Result = LinePick(A->X, A->Y + 5.0f, A->Z, 0.0f, -10000.0f, 0.0f);
					//	if(Result != 0)
					//	{
					//		PositionEntity(A->CollisionEN, A->X, PickedY() + Height, A->Z);
					//	}else
					//	{
					//		Result = LinePick(A->X, A->Y + 10000.0f, A->Z, 0.0f, -20000.0f, 0.0f);
					//		if(Result != 0)
					//			PositionEntity(A->CollisionEN, A->X, PickedY() + Height, A->Z);
					//	}
					//}
					RotateEntity(A->CollisionEN, 0.0f, A->Yaw, 0.0f);
					A->Position.Y = 1.0f;
					
					if(A->RNID > 0 && MilliSecs() - ZonedMS > 5000)
						Output(LanguageString[LS_PlayerEnteredZone] + string(" ") + A->Name, 0, 100, 255);
				}
				break;
			}
			
		case P_ChangeArea: // I have gone into a new area
			{

				Me->IgnoreUpdate = true;
				// Retrieve info for new zone
				string OldAreaName = RealmCrafter::Globals->AreaName;
				int OldAreaID = CurrentAreaID;

				Me->Position.SectorX = M->MessageData.GetRealShort(0);
				Me->Position.SectorZ = M->MessageData.GetRealShort(2);
				Me->Position.X = M->MessageData.GetRealFloat(4);
				float Y = M->MessageData.GetRealFloat(8);
				Me->Position.Z = M->MessageData.GetRealFloat(12);
				Me->Position.FixValues();

				// NOTE: PlayerChangedSector is called AFTER zone is loaded...

				if(Me->Actor->Environment == Environment_Fly)
					Me->Position.Y = Y;
				else
					Me->Position.Y = 0.0f;

				Me->Destination = Me->Position;

				DebugLog(string("Get: ") + toString(Me->Position.SectorX) + ":" + toString(Me->Position.X) + ", " + toString(Y) + ", " + toString(Me->Position.SectorZ) + ":" + toString(Me->Position.Z) + ";");



				float Yaw = M->MessageData.GetRealFloat(16);
				PVPEnabled = M->MessageData.GetRealChar(20) > 0 ? true : false;
				int Grav = M->MessageData.GetRealShort(21);
				Gravity = 0.05f * ((float)(Grav - 200) / 100);
				CurrentAreaID = M->MessageData.GetRealInt(23);
				int NameLen = M->MessageData.GetRealChar(28);
				RealmCrafter::Globals->AreaName = M->MessageData.Substr(29, NameLen).c_str();

				// Going to new zone or instance
				if(OldAreaID != CurrentAreaID)
				{
					// Remove scripted emitters
					foreachc(SEmIt, ScriptedEmitter, ScriptedEmitterList)
					{
						ScriptedEmitter* SEm = *SEmIt;

						if(SEm->AttachedToPlayer == false)
						{
							RP_FreeEmitter(SEm->EN, true, false);

							ScriptedEmitter::Delete(SEm);
						}

						nextc(SEmIt, ScriptedEmitter, ScriptedEmitterList);
					}
					ScriptedEmitter::Clean();
					RP_Emitter::Clean();

					foreachc(EIt, RP_Emitter, RP_EmitterList)
					{
						RP_Emitter* E = *EIt;

						if(E->KillMode != 0)
							RP_FreeEmitter(E->EmitterEN, true, false);

						nextc(EIt, RP_Emitter, RP_EmitterList);
					}
					RP_Emitter::Clean();


					// Remove all in-flight projectiles
					foreachc(ProjIIt, ProjectileInstance, ProjectileInstanceList)
					{
						FreeProjectileInstance(*ProjIIt);
						nextc(ProjIIt, ProjectileInstance, ProjectileInstanceList);
					}
					ProjectileInstance::Clean();

					// Remove old actor instances
					foreachc(AIt, ActorInstance, ActorInstanceList)
					{
						if(*AIt != Me)
						{
							RemoveRadarMark(*AIt);
							SafeFreeActorInstance(*AIt);
						}

						nextc(AIt, ActorInstance, ActorInstanceList);
					}
					ActorInstance::Clean();
				}

				// Remove dropped loot
				foreachc(DItemIt, DroppedItem, DroppedItemList)
				{
					FreeEntity((*DItemIt)->EN);
					DroppedItem::Delete(*DItemIt);
					nextc(DItemIt, DroppedItem, DroppedItemList);
				}
				DroppedItem::Clean();

				// Unload old and load new zone if(necessary
				if(RealmCrafter::Globals->AreaName != OldAreaName)
				{
					UnloadArea();
					LoadArea(RealmCrafter::Globals->AreaName, Cam, true);
				
					//foreachc(LIt, Light, LightList)
					//{
					//	Light* L = *LIt;

					//	L->R = 0.0f;
					//	L->G = 0.0f;
					//	L->B = 0.0f;
					//	L->DestR = 0;
					//	L->DestG = 0;
					//	L->DestB = 0;
					//	L->Enabled(false);

					//	nextc(LIt, Light, LightList)
					//}

					PositionEntity(GPP, 0.0f, 0.0f, 0.0f);
					RotateEntity(GPP, DefaultLightPitch, DefaultLightYaw, 0.0f);
					ScaleEntity(GPP, 0.0f, 0.0f, 0.0f);
					TFormVector(0.0f, 0.0f, 1.0f, GPP, 0);
					//SetLightDirection(DefaultLight->EN, TFormedX(), TFormedY(), TFormedZ());

					string TexPath = GetTextureNameNoFlag(MapTexID);
					if(TexPath.length() != 0)
					{
						TexPath = string("Data\\Textures\\") + TexPath;

						PlayerMap->SetImage(TexPath);
						RadarBox->SetImage(TexPath, "");
					}

					
					RealmCrafter::Globals->LastZoneLoad = 0;
				}

				if(TerrainManager != 0)
				{
					printf("ZoneLoad Pos: %f, %f;\n", Me->Position.X + (Me->Position.SectorX * RealmCrafter::SectorVector::SectorSize),
						Me->Position.Z + (Me->Position.SectorZ * RealmCrafter::SectorVector::SectorSize));
					TerrainManager->Update(NGin::Math::Vector3(Me->Position.X + (Me->Position.SectorX * RealmCrafter::SectorVector::SectorSize),
					Y + 10,
					Me->Position.Z + (Me->Position.SectorZ * RealmCrafter::SectorVector::SectorSize)), true);
				}

				//UpdateWorld();

				// Update settings
				//Environment->SetWeather((int)CharFromStr(M->MessageData.Substr(27, 1)));
				PlayerTarget = 0;
				SceneryTarget = NULL;
				RealmCrafter::Globals->AttackTarget = false;
				ResetEntity(Me->CollisionEN);
				PositionEntity(Me->CollisionEN, Me->Position.X, Y + 10.0f, Me->Position.Z);
				RotateEntity(Me->CollisionEN, 0.0f, Yaw, 0.0f);

				printf("CollisionReadOut: %f, %f\n", EntityX(Me->CollisionEN), EntityZ(Me->CollisionEN));

				DebugLog(string("Set: ") + toString(Me->Position.SectorX) + ":"  + toString(Me->Position.X) + ", " + toString(Y + 10.0f) + ", " + toString(Me->Position.SectorZ) + ":" + toString(Me->Position.Z) + ";     " + toString(EntityX(Me->CollisionEN, true)) + ", " + toString(EntityY(Me->CollisionEN, true)) + ", " + toString(EntityZ(Me->CollisionEN, true)));

				ResetEntity(Cam);
				PositionEntity(Cam, Me->Position.X, Y + 10.0f, Me->Position.Z);
				CameraController->ResetCameraPosition(Me->Position.X, Y + 10.0f, Me->Position.Z);
				PositionEntity(Cam, Me->Position.X, Y + 10.0f, Me->Position.Z);
				MoveMouse(GFXWidth / 2, GFXHeight / 2);
				ZonedMS = MilliSecs();

				// if the new zone is different to the old
				if(RealmCrafter::Globals->AreaName != OldAreaName)
					WriteLog(MainLog, string("Entered zone: ") + RealmCrafter::Globals->AreaName);
				else
					WriteLog(MainLog, "Reloaded current zone");

				// Server can resume player updates
				CRCE_Send(Connection, Connection, P_ChangeArea, M->MessageData, true);
				Me->IgnoreUpdate = false;

				// Update sector visibility
				PlayerChangedSector(Me->Position.SectorX, Me->Position.SectorZ);
				
				break;
			}
			
		case ConnectionHasLeft: // Host disconnected
			{
				RuntimeError(LanguageString[LS_LostConnection]);
				break;
			}

		case RCE_Disconnected: // Host disconnected
			{
				RuntimeError(LanguageString[LS_LostConnection]);
				break;
			}
		case P_KickedPlayer: // Kicked maybe?
			{
				RuntimeError(LanguageString[LS_LostConnection]);
				break;
			}
		}
		RCE_Message::Delete(M);
		
		nextc(MIt, RCE_Message, RCE_MessageList);
	}
	RCE_Message::Clean();

	// Send GUI Update
	RealmCrafter::PacketWriter GUIWriter;
	RealmCrafter::Globals->ControlHostManager->PopEventPacket(GUIWriter);
	if(GUIWriter.GetLength() > 0)
	{
		RCE_FSend(Connection, P_GUIEvent, GUIWriter.GetBytes(), 1, GUIWriter.GetLength());
	}

	// Send update packet
	NewEntityX = EntityX(Me->CollisionEN);
	NewEntityY = EntityY(Me->CollisionEN);
	NewEntityZ = EntityZ(Me->CollisionEN);

	if(!RealmCrafter::SectorVector::FloatEqual(OldEntityX, NewEntityX)
		|| !RealmCrafter::SectorVector::FloatEqual(OldEntityY, NewEntityY)
		|| !RealmCrafter::SectorVector::FloatEqual(OldEntityZ, NewEntityZ)
		|| !RealmCrafter::SectorVector::FloatEqual(OldYaw, Me->Yaw))
	{
		if(MilliSecs() - LastNetwork > NetworkMS)
		{
			char Flags = 0;
			Flags += (Me->WalkingBackward ? 1 : 0);
			Flags += (Me->StrafingLeft ? 2 : 0);
			Flags += (Me->StrafingRight ? 4 : 0);

			//while(Me->Yaw > 180.0f)
			//	Me->Yaw -= 360.0f;
			//while(Me->Yaw < -180.0f)
			//	Me->Yaw += 360.0f;

			//short YawShort = (short)((Me->Yaw / 180.0f) * 32767.0f);

			NGin::CString Pa = NGin::CString::FormatReal("hhfffhhffffcc",
				Me->Destination.SectorX, Me->Destination.SectorZ, Me->Destination.X, Me->Destination.Y, Me->Destination.Z,
				Me->Position.SectorX, Me->Position.SectorZ, EntityX(Me->CollisionEN), EntityY(Me->CollisionEN), EntityZ(Me->CollisionEN),
				(float)Me->Yaw, (char)Me->IsRunning, (char)Flags);

// 			NGin::CString Pa = NGin::CString::FormatReal("fffffcc",
// 				Me->DestX, Me->DestZ,
// 				EntityY(Me->CollisionEN), EntityX(Me->CollisionEN), EntityZ(Me->CollisionEN),
// 				(char)Me->IsRunning, (char)Me->WalkingBackward);

			CRCE_Send(Connection, Connection, P_StandardUpdate, Pa);
			LastNetwork = MilliSecs();

			OldEntityX = EntityX(Me->CollisionEN);
			OldEntityY = EntityY(Me->CollisionEN);
			OldEntityZ = EntityZ(Me->CollisionEN);
			OldYaw = Me->Yaw;
		}
	}
}

