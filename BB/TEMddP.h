
	float CamAngle = 0.0f;
	uint Set = 0;
	// Background
	if(FileType("Data\\Meshes\\Character Set\\Set.eb3d") == 1)
	{
		Set = LoadMesh("Data\\Meshes\\Character Set\\Set.eb3d");
		if(Set == 0)
			RuntimeError("Could not load Data\\Meshes\\Character Set\\Set.eb3d!");
	}else
	{
		Set = LoadMesh("Data\\Meshes\\Character Set\\Set.b3d");
		if(Set == 0)
			RuntimeError("Could not load Data\\Meshes\\Character Set\\Set.b3d!");
	}
	PositionEntity(Set, -210.0f, -35.0f, -145.0f); // Delete me
	ScaleEntity(Set, 30.0f, 30.0f, 30.0f);         // Delete me too
	EntityShader(Set, Shader_LitObject1);          // Delete me also (once this is auto-detected)

	// Selection window
	uint Background = CreateSAQuad();
	SAQuadLayout(Background, 0.03f, 0.05f, 0.34f, 0.9f);
	EntityOrder(Background, -1);
	uint Tex = LoadTexture("Data\\Textures\\Menu\\Character Selection.png");
	EntityTexture(Background, Tex);
	FreeTexture(Tex);

	uint GPP = CreatePivot();
	ActorInstance* PreviewA = 0;

	RestartCharSelection:
	if(PreviewA != 0)
		SafeFreeActorInstance(PreviewA);

	// Create the window
	uint BStart  = GY_CreateCustomButton(0, 0.82f,  0.9f,   0.16f,  0.05f, LoadButtonU("SelectCharacter"), LoadButtonD("SelectCharacter"), LoadButtonH("SelectCharacter"));
	uint BDelete = GY_CreateCustomButton(0, 0.54f,  0.9f,   0.25f,  0.05f, LoadButtonU("DeleteCharacter"), LoadButtonD("DeleteCharacter"), LoadButtonH("DeleteCharacter"));
	uint BLeft   = GY_CreateCustomButton(0, 0.396f, 0.896f, 0.043f, 0.058f, LoadButtonU("LargeLeft"), LoadButtonD("LargeLeft"), LoadButtonH("LargeLeft"));
	uint BRight  = GY_CreateCustomButton(0, 0.446f, 0.896f, 0.043f, 0.058f, LoadButtonU("LargeRight"), LoadButtonD("LargeRight"), LoadButtonH("LargeRight"));
	GY_DropGadget(BStart);
	GY_DropGadget(BDelete);
	GY_DropGadget(BLeft);
	GY_DropGadget(BRight);

	// Character buttons (max of 10 characters)
	int Offset = 0;
	int Number = 0;

	while(Offset < CharList.Length())
	{
		// Extract data
		int Length = CharFromStr(CharList.Substr(Offset, 1));
		CharNames[Number] = CharList.Substr(Offset + 1, Length);
		Offset = Offset + Length + 1;
		CharActors[Number] = ShortFromStr(CharList.Substr(Offset, 2));
		CharGender[Number] = CharFromStr(CharList.Substr(Offset + 2, 1));
		CharFaceTex[Number] = CharFromStr(CharList.Substr(Offset + 3, 1));
		CharHair[Number] = CharFromStr(CharList.Substr(Offset + 4, 1));
		CharBeard[Number] = CharFromStr(CharList.Substr(Offset + 5, 1));
		CharBodyTex[Number] = CharFromStr(CharList.Substr(Offset + 6, 1));

		// Move on
		Offset = Offset + 7;
		Number = Number + 1;

		// Create button
		CharButtons[Number - 1] = GY_CreateButton(0, 0.0436f, 0.185f + (((float)Number) * 0.05f), 0.3128f, 0.036f, CharNames[Number - 1], 1);
		GY_DropGadget(CharButtons[Number - 1]);
	}
	int LastChar = Number;
	// New character button
	if(Number < 10)
	{
		CharButtons[Number] = GY_CreateButton(0, 0.081f, 0.23f + (((float)Number + 1) * 0.05f), 0.238f, 0.036f, LanguageString[LS_NewCharacter]);
		GY_DropGadget(CharButtons[Number]);
	}
	int SelectedChar = -1;
	bool Setup = false; // Dreamora: 1.23
	PositionEntity(GPP, 30, -35, 100); // DR

	// Event loop
	while(true)
	{
		int TargetTime = 0, LastUpdateTime = 0, TargetEntity = 0; // DR

		// Escape pressed (quit)
		if(KeyHit(1))
		{
			RCE_Disconnect();
			exit(0);
		}

		// Check for existing character buttons being pressed
		for(int i = 0; i < LastChar; ++i)
		{
			if(GY_ButtonHit(CharButtons[i]))
			{
				Setup = true;
				TargetEntity = 0; // DR

				// Untoggle old button
				if(SelectedChar > -1)
					GY_SetButtonState(CharButtons[SelectedChar], false);
				SelectedChar = i;
				GY_SetButtonState(CharButtons[i], true);

				Actor* A = ActorList[CharActors[i]];
				if(PreviewA != 0)
					SafeFreeActorInstance(PreviewA);
				PreviewA = CreateActorInstance(A);
				PreviewA->Gender = CharGender[i];
				PreviewA->FaceTex = CharFaceTex[i];
				PreviewA->Hair = CharHair[i];
				PreviewA->Beard = CharBeard[i];
				PreviewA->BodyTex = CharBodyTex[i];
				bool Result = LoadActorInstance3D(PreviewA);
				if(Result == false)
					RuntimeError(String("Could not load actor mesh for ") + A->Race + String("!"));
				if(PreviewA->ShadowEN != 0)
					HideEntity(PreviewA->ShadowEN);
				if(PreviewA->NametagEN != 0)
					HideEntity(PreviewA->NametagEN);
				PlayAnimation(PreviewA, 1, 0.003f, Anim_Idle);
				PositionEntity(PreviewA->CollisionEN, 30.0f, -(35.0f + EntityY(PreviewA->EN, true)), 100.0f);
				break;
			}
		}
		
		// Create new character button
		if(GY_ButtonHit(CharButtons[LastChar]))
		{
			for(int i = 0; i <= 9; ++i)
				if(CharButtons[i] != 0)
					GY_FreeGadget(CharButtons[i]);
			
			GY_FreeGadget(BStart);
			GY_FreeGadget(BDelete);
			GY_FreeGadget(BLeft);
			GY_FreeGadget(BRight);
			HideEntity(Background);
			if(PreviewA != 0)
				SafeFreeActorInstance(PreviewA);
			PreviewA = 0;
			int Result = CreateChar();
			if(Result == 1)
				GY_MessageBox(LanguageString[LS_Error], LanguageString[LS_CannotCreateChar]);
			ShowEntity(Background);
			goto RestartCharSelection;
		}

		// Delete character button
		if(GY_ButtonHit(BDelete) && SelectedChar > -1)
		{
			if(GY_RequestBox(LanguageString[LS_Warning], LanguageString[LS_ReallyDeleteChar]))
			{
				String Pa = String::FormatReal("cscsc", UName.Length(), &UName, PWord.Length(), &PWord, SelectedChar);
				CRCE_Send(Connection, Connection, P_DeleteCharacter, Pa, true);

				// Wait for reply
				bool Done = false;
				while(Done == false)
				{
					Delay(10);

					foreach(MIt, RCE_Message, RCE_MessageList)
					{
						RCE_Message* M = *MIt;

						if(M->MessageType == P_DeleteCharacter)
						{
							CharList = M->MessageData;
							RCE_Message::Delete(M);
							for(int i = 0; i <= 9; ++i)
							{
								if(CharButtons[i] != 0)
								{
									GY_FreeGadget(CharButtons[i]);
									CharButtons[i] = 0;
								}
							}
							
							GY_FreeGadget(BStart);
							GY_FreeGadget(BDelete);
							GY_FreeGadget(BLeft);
							GY_FreeGadget(BRight);
							if(PreviewA != 0)
								SafeFreeActorInstance(PreviewA);
							PreviewA = 0;
							goto RestartCharSelection;
						}else if(M->MessageType == ConnectionHasLeft || M->MessageType == RCE_Disconnected)
							RuntimeError(LanguageString[LS_LostConnection]);
						
						RCE_Message::Delete(M);

						next(MIt, RCE_Message, RCE_MessageList);
					}
					RCE_Message::Clean();

					RCE_Update();
					RCE_CreateMessages();
					GY_Update();
					GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
					UpdateWorld();
					RenderWorld();
				}
			}
		}

		// Start game button
		if(GY_ButtonHit(BStart) == true && SelectedChar > -1)
		{
			GY_LockGadget(BStart);
			GY_LockGadget(BDelete);
			for(int i = 0; i <= LastChar + 1; ++i)
				GY_LockGadget(CharButtons[i]);

			String Pa = String::FormatReal("cscsc", UName.Length(), &UName, PWord.Length(), &PWord, SelectedChar);
			CRCE_Send(Connection, Connection, P_FetchCharacter, Pa, true);

			// Stuff I already know
			Me = CreateActorInstance(ActorList[CharActors[SelectedChar]]);
			Me->Name = CharNames[SelectedChar];
			Me->Gender = CharGender[SelectedChar];
			Me->FaceTex = CharFaceTex[SelectedChar];
			Me->Hair = CharHair[SelectedChar];
			Me->Beard = CharBeard[SelectedChar];
			Me->BodyTex = CharBodyTex[SelectedChar];

			// Wait for replies
			int Quests = 0;
			int RequiredQuests = 1000;
			int Spells = 0;
			int RequiredSpells = 2000;
			int Memorised = 0;
			int ItemsDone = 0;
			int AttributesDone = 0;
			bool Done = false;
			while(Done == false)
			{
				Delay(10);

				foreach(MIt, RCE_Message, RCE_MessageList)
				{
					RCE_Message* M = *MIt;

					if(M->MessageType == P_FetchCharacter)
					{
						// Character information
						if(M->MessageData.Substr(0, 1) == String("C"))
						{
							// Block 1
							if(M->MessageData.Substr(1, 1) == String("1"))
							{
								Me->Gold = IntFromStr(M->MessageData.Substr(32, 4));
								Me->Reputation = ShortFromStr(M->MessageData.Substr(6, 2));
								Me->Level = ShortFromStr(M->MessageData.Substr(8, 2));
								Me->XP = IntFromStr(M->MessageData.Substr(10, 4));
								Me->HomeFaction = CharFromStr(M->MessageData.Substr(14, 1));
								Offset = 15;
								while(Offset < M->MessageData.Length())
								{
									Me->Attributes->Value[AttributesDone] = ShortFromStr(M->MessageData.Substr(Offset, 2));
									Me->Attributes->Maximum[AttributesDone] = ShortFromStr(M->MessageData.Substr(Offset + 2, 2));
									++AttributesDone;
									Offset = Offset + 4;
								}
							// Block 2 <No longer exists, reserved for future use>
//							Elseif(Mid$(M->MessageData$, 2, 1) = "2"

							// Block 3
							}else if(M->MessageData.Substr(1, 1) == String("3"))
							{
								Offset = 2;
								while(Offset < M->MessageData.Length())
								{
									int Position = Asc(M->MessageData.Substr(Offset, 1));
									if(Position < 50)
									{
										String Item = M->MessageData.Substr(Offset + 1, ItemInstanceStringLength());
										Me->Inventory->Items[Position] = ItemInstanceFromString(Item);
										Offset = Offset + 1 + ItemInstanceStringLength();
										Me->Inventory->Amounts[Position] = ShortFromStr(M->MessageData.Substr(Offset, 2));
										Offset = Offset + 2;
									}else
										++Offset;
									
									++ItemsDone;
								}
							}
						}else if(M->MessageData.Substr(0, 1) == String("S")) // A known spells block
						{
							Offset = 1;
							while(Offset < M->MessageData.Length())
							{
								Me->SpellLevels[Spells] = ShortFromStr(M->MessageData.Substr(Offset, 2));
								Spell* Sp = new Spell();
								Sp->ID = ShortFromStr(M->MessageData.Substr(Offset + 2, 2));
								SpellsList[Sp->ID] = Sp;
								Me->KnownSpells[Spells] = Sp->ID;
								Sp->ThumbnailTexID = ShortFromStr(M->MessageData.Substr(Offset + 4, 2));
								Sp->RechargeTime = ShortFromStr(M->MessageData.Substr(Offset + 6, 2));
								Offset = Offset + 8;
								int NameLen = ShortFromStr(M->MessageData.Substr(Offset, 2));
								if (NameLen<0) NameLen = 0;
								Sp->Name = M->MessageData.Substr(Offset + 2, NameLen);
								Offset = Offset + 2 + NameLen;
								NameLen = ShortFromStr(M->MessageData.Substr(Offset, 2));
								if (NameLen<0) NameLen = 0;
								Sp->Description = M->MessageData.Substr(Offset + 2, NameLen);
								Offset = Offset + 2 + NameLen;
								if(CharFromStr(M->MessageData.Substr(Offset, 1)) > 0 && Memorised < 10)
								{
									Me->MemorisedSpells[Memorised] = Spells;
									++Memorised;
								}
								++Offset;
								++Spells;
							}
						}else if(M->MessageData.Substr(0, 1) == String("Q")) // A quest log block
						{
							Offset = 1;
							while(Offset < M->MessageData.Length())
							{
								int NameLen = CharFromStr(M->MessageData.Substr(Offset, 1));
								if (NameLen<0) NameLen = 0;
								MyQuestLog->EntryName[Quests] = M->MessageData.Substr(Offset + 1, NameLen);
								Offset = Offset + 1 + NameLen;
								NameLen = ShortFromStr(M->MessageData.Substr(Offset, 2));
								if (NameLen<0) NameLen = 0;
								MyQuestLog->EntryStatus[Quests] = M->MessageData.Substr(Offset + 2, NameLen);
								Offset = Offset + 2 + NameLen;
								++Quests;
							}
						}else // Final block
						{
							RequiredQuests = ShortFromStr(M->MessageData.Substr(1, 2));
							RequiredSpells = ShortFromStr(M->MessageData.Substr(3, 2));
						}

						RCE_Message::Delete(M);

						// Complete!
						if(Quests >= RequiredQuests && Spells >= RequiredSpells && AttributesDone > 39 && ItemsDone == 50)
						{
							Done = true;
							break;
						}
					}else if(M->MessageType == ConnectionHasLeft || M->MessageType == RCE_Disconnected)
					{
						RuntimeError(LanguageString[LS_LostConnection]);
						RCE_Message::Delete(M);
					}

					next(MIt, RCE_Message, RCE_MessageList);
				}
				RCE_Message::Clean();

				RCE_Update();
				RCE_CreateMessages();
				GY_Update();
				GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
				UpdateWorld();
				RenderWorld();
			}

			// Start the game!
			RCE_Disconnect();
			SelectedCharacter = SelectedChar;
			break;
		}

		// Camera
		if(GY_ButtonDown(BRight))
			CamAngle += 1.5f;
		else if(GY_ButtonDown(BLeft))
			CamAngle -= 1.5f;
		
		if(Setup == false) // DR
		{
			PositionEntity(GPP, 30.0f, -35.0f, 100.0f);
			RotateEntity(GPP, 0.0f, CamAngle, 0.0f);
			TFormPoint(0.0f, 170.0f, -240.0f, GPP, 0);
			PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
			PointEntity(Cam, GPP);
			MoveEntity(Cam, -45.0f, 0.0f, 0.0f);
		}
		float dx = 0, dy = 0, dz= 0;
		
		// Dreamora: 1.23
		if(Setup)
		{
			if(TargetEntity == 0)
			{
				TargetTime = MilliSecs() + 500;
				LastUpdateTime = MilliSecs();
				
				TargetEntity = FindChild(PreviewA->EN, "CamTarget");
				if(TargetEntity == 0)
				{
					TargetEntity = FindChild(PreviewA->EN, "Chest");
					if(TargetEntity > 0)
					{
						TFormPoint(0,0,0, TargetEntity, GPP);
						dx = TFormedX(); dy = TFormedY(); dz = TFormedZ();
					}
				}else
				{
					TFormPoint(0,0,0, TargetEntity, GPP);
					dx = TFormedX(); dy = TFormedY(); dz = TFormedZ();
				}

				if(TargetEntity == 0)
				{
					TargetEntity = FindChild(PreviewA->EN, "Head");
					TFormPoint(0,0,0, TargetEntity, GPP);
					dx = TFormedX(); dy = TFormedY(); dz = TFormedZ();
				}
				
				if(TargetEntity == 0)
				{
					PositionEntity(GPP, 30, -35, 100);
					TargetEntity = GPP;
					goto SetupExit;
					Setup = false;
				}
			}
			
			if(EntityDistance(TargetEntity, GPP) < 0.5f)
			{
				Setup = false;
				goto SetupExit;
			}
			
			float value;
			
			if(MilliSecs() < TargetTime)
				value = (MilliSecs() - LastUpdateTime) / 500.0f;
			else
				value = (TargetTime - LastUpdateTime) / 500.0f;
			
			LastUpdateTime = MilliSecs();
			
			MoveEntity(GPP, dx * value, dy * value, dz * value);
			TFormPoint(0.0f, 120.0f, -150.0f, GPP, 0);
			PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
			PointEntity(Cam, GPP);
			MoveEntity(Cam, -40.0f, 0.0f, 0.0f);
			
			if(MilliSecs() > TargetTime)
				Setup = false;
			
			
		}
		SetupExit:


		// Check connection is still alive
		foreach(MIt, RCE_Message, RCE_MessageList)
		{
			RCE_Message* M = *MIt;
			
			if(M->MessageType == RCE_Disconnected || M->MessageType == ConnectionHasLeft)
				RuntimeError(LanguageString[LS_LostConnection]);

			RCE_Message::Delete(M);

			next(MIt, RCE_Message, RCE_MessageList);
		}
		RCE_Message::Clean();

		// Update everything
		RCE_Update();
		RCE_CreateMessages();
		GY_Update();
		RP_Update();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		UpdateWorld();
		RenderWorld();
	} // End infinite loop

	for(int i = 0; i <= 9; ++i)
		if(CharButtons[i] != 0)
			GY_FreeGadget(CharButtons[i]);
	
	GY_FreeGadget(BStart);
	GY_FreeGadget(BDelete);
	GY_FreeGadget(BLeft);
	GY_FreeGadget(BRight);
	if(PreviewA != 0)
		SafeFreeActorInstance(PreviewA);
	FreeEntity(Background);
	FreeEntity(Set);
	FreeEntity(GPP);GPP = 0;


	
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


int CreateChar()
{

	// Clear point spends
	for(int i = 0; i <= 39; ++i)
		PointSpends[i] = 0;
	int PointsToSpend = 0;

	// Count assignable attributes
	int TotalAttributes = 0;
	for(int i = 0; i <= 39; ++i)
		if(AttributeNames[i].Length() > 0 && AttributeIsSkill[i] == false && AttributeHidden[i] == false)
			++TotalAttributes;

	// Create the windows
	uint WChar = GY_CreateWindow(LanguageString[LS_CharacterTitle], 0.01f, 0.05f, 0.22f, 0.4f, true, false, false);
	float StatWindowHeight = ((float)TotalAttributes + 1) * 0.05f;
	uint WStat = GY_CreateWindow(LanguageString[LS_AttributesTitle], 0.67f, 0.05f, 0.32f, StatWindowHeight, true, false, false);

	// Race list
	uint CRace = GY_CreateComboBox(WChar, 0.05f, 0.05f, 0.9f, 0.5f, LanguageString[LS_Race]);
	foreach(AIt, Actor, ActorList)
	{
		Actor* A = *AIt;

		if(A->Playable == true)
		{
			// Check every previous actor to make sure this race hasn't already been added
			bool AlreadyAdded = false;
			foreach(A2It, Actor, ActorList)
			{
				Actor* A2 = *A2It;

				if(A2 == A)
					break;
				if(A2->Playable == true)
				{
					if(A2->Race.AsUpper() == A->Race.AsUpper())
					{
						AlreadyAdded = true;
						break;
					}
				}

				next(A2It, Actor, ActorList);
			}
			if(AlreadyAdded == false)
				GY_AddComboBoxItem(CRace, A->Race);
		}

		next(AIt, Actor, ActorList);
	}

	// Camera movement
	uint BLeft  = GY_CreateCustomButton(0, 0.046f, 0.936f, 0.043f, 0.058f, LoadButtonU("LargeLeft"), LoadButtonD("LargeLeft"), LoadButtonH("LargeLeft"));
	uint BRight = GY_CreateCustomButton(0, 0.096f, 0.936f, 0.043f, 0.058f, LoadButtonU("LargeRight"), LoadButtonD("LargeRight"), LoadButtonH("LargeRight"));
	GY_DropGadget(BLeft);
	GY_DropGadget(BRight);

	// Character options
	GY_CreateLabel(WChar, 0.5f, 0.2f, LanguageString[LS_Gender], 255, 255, 255, Justify_Centre);
	uint BNextGender = GY_CreateButton(WChar, 0.86f, 0.2f, 0.1f, 0.07f, ">");
	uint BPrevGender = GY_CreateButton(WChar, 0.04f, 0.2f, 0.1f, 0.07f, "<");
	GY_CreateLabel(WChar, 0.5f, 0.33f, LanguageString[LS_Class], 255, 255, 255, Justify_Centre);
	uint BNextClass = GY_CreateButton(WChar, 0.86f, 0.33f, 0.1f, 0.07f, ">");
	uint BPrevClass = GY_CreateButton(WChar, 0.04f, 0.33f, 0.1f, 0.07f, "<");
	GY_CreateLabel(WChar, 0.5f, 0.46f, LanguageString[LS_Hair], 255, 255, 255, Justify_Centre);
	uint BNextHair = GY_CreateButton(WChar, 0.86f, 0.46f, 0.1f, 0.07f, ">");
	uint BPrevHair = GY_CreateButton(WChar, 0.04f, 0.46f, 0.1f, 0.07f, "<");
	GY_CreateLabel(WChar, 0.5f, 0.59f, LanguageString[LS_Face], 255, 255, 255, Justify_Centre);
	uint BNextFace = GY_CreateButton(WChar, 0.86f, 0.59f, 0.1f, 0.07f, ">");
	uint BPrevFace = GY_CreateButton(WChar, 0.04f, 0.59f, 0.1f, 0.07f, "<");
	GY_CreateLabel(WChar, 0.5f, 0.72f, LanguageString[LS_Beard], 255, 255, 255, Justify_Centre);
	uint BNextBeard = GY_CreateButton(WChar, 0.86f, 0.72f, 0.1f, 0.07f, ">");
	uint BPrevBeard = GY_CreateButton(WChar, 0.04f, 0.72f, 0.1f, 0.07f, "<");
	GY_CreateLabel(WChar, 0.5f, 0.85f, LanguageString[LS_Clothes], 255, 255, 255, Justify_Centre);
	uint BNextBody = GY_CreateButton(WChar, 0.86f, 0.85f, 0.1f, 0.07f, ">");
	uint BPrevBody = GY_CreateButton(WChar, 0.04f, 0.85f, 0.1f, 0.07f, "<");

	// Class description
	float Y = 0.46f;
	for(int i = 0; i <= 9; ++i)
	{
		LDescription[i] = GY_CreateLabel(0, 0.01f, Y, BBString("W", 50));
		GY_DropGadget(LDescription[i]);
		GY_UpdateLabel(LDescription[i], "");
		Y += 0.025f;
	}

	// Name box and Done button
	uint LName = GY_CreateLabel(0, 0.35f, 0.86f, LanguageString[LS_CharacterName], 255, 255, 255, Justify_Right);
	uint TName = GY_CreateTextField(0, 0.36f, 0.86f, 0.35f, 0, 25);
	uint BDone = GY_CreateCustomButton(0, 0.836f, 0.936f, 0.158f, 0.058f, LoadButtonU("CreateChar"), LoadButtonD("CreateChar"), LoadButtonH("CreateChar"));
	uint BCancel = GY_CreateCustomButton(0, 0.636f, 0.936f, 0.158f, 0.058f, LoadButtonU("CancelChar"), LoadButtonD("CancelChar"), LoadButtonH("CancelChar"));
	GY_DropGadget(LName);
	GY_DropGadget(TName);
	GY_DropGadget(BDone);
	GY_DropGadget(BCancel);


	uint RemainingLabel = 0;
	// Attributes assignment list
	if(AttributeAssignment > 0)
	{
		PointsToSpend = AttributeAssignment;
		RemainingLabel = GY_CreateLabel(WStat, 0.5f, 0.02f, LanguageString[LS_AttributePoints] + String(" ") + String(PointsToSpend), 255, 50, 50, Justify_Centre);
	}
	Y = 0.05f / StatWindowHeight;
	int Count = 0;
	for(int i = 0; i <= 39; ++i)
	{
		if(AttributeNames[i].Length() > 0 && AttributeIsSkill[i] == false && AttributeHidden[i] == false)
		{
			GY_CreateLabel(WStat, 0.02f, Y, AttributeNames[i] + String(":"));
			AttributeLabels[Count] = GY_CreateLabel(WStat, 0.8f, Y, String("000"), 255, 255, 255, Justify_Centre);
			GY_SetGadgetData(AttributeLabels[Count], i);
			if(AttributeAssignment > 0)
			{
				AttributeDecrease[Count] = GY_CreateButton(WStat, 0.64f, Y + 0.0052f, 0.06f, 0.028f / StatWindowHeight, String("<"));
				AttributeIncrease[Count] = GY_CreateButton(WStat, 0.9f, Y + 0.0052f, 0.06f, 0.028f / StatWindowHeight, String(">"));
			}
			Y += (0.05f / StatWindowHeight);
			++Count;
		}
	}

	// Set preview to first playable actor
	Actor* A = *Actor::ActorList.Begin();
	if(A == 0)
		RuntimeError("No actors in project!");
	while(A->Playable == false)
	{
		A = Actor::ActorList.After(A);
		if(A == 0)
			RuntimeError("No playable actors in project!");
	}

	ActorInstance* Preview = CreateActorInstance(A);

	if(GPP == 0)
		GPP = CreatePivot();

	bool Result = LoadActorInstance3D(Preview);
	if(Result == false)
		RuntimeError(String("Could not load actor mesh for ") + A->Race + String("!"));

	PlayAnimation(Preview, 1, 0.003, Anim_Idle);
	PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
	HideEntity(Preview->ShadowEN);
	SetUpPreview(Preview->Actor);

	GY_LockGadget(BNextFace, !ActorHasFace(Preview->Actor, Preview->Gender + 1));
	GY_LockGadget(BPrevFace, !ActorHasFace(Preview->Actor, Preview->Gender + 1));
	GY_LockGadget(BNextHair, !ActorHasHair(Preview->Actor, Preview->Gender + 1));
	GY_LockGadget(BPrevHair, !ActorHasHair(Preview->Actor, Preview->Gender + 1));
	GY_LockGadget(BNextBeard, !ActorHasBeard(Preview->Actor));
	GY_LockGadget(BPrevBeard, !ActorHasBeard(Preview->Actor));
	GY_LockGadget(BNextGender, Preview->Actor->Genders);
	GY_LockGadget(BPrevGender, Preview->Actor->Genders);
	GY_UpdateComboBox(CRace, Preview->Actor->Race);

	float CamAngle = 0.0f;

	// Event loop
	while(true)
	{
		// Escape pressed or cancel pressed
		if(KeyHit(1) || GY_ButtonHit(BCancel))
		{
			SafeFreeActorInstance(Preview);
			GY_FreeGadget(WChar);
			GY_FreeGadget(WStat);
			GY_FreeGadget(LName);
			GY_FreeGadget(TName);
			GY_FreeGadget(BDone);
			GY_FreeGadget(BCancel);
			GY_FreeGadget(BLeft);
			GY_FreeGadget(BRight);
			for(int i = 0; i <= 9; ++i)
				GY_FreeGadget(LDescription[i]);
			return 0;
		}

		// Remove special characters from character names
		String Name = GY_TextFieldText(TName);
		Name.Replace("\"", "");
		Name.Replace("/", "");
		Name.Replace("\\", "");
		Name.Replace("%", "");
		Name.Replace("&", "");
		Name.Replace("|", "");
		Name.Replace("¦", "");
		Name.Replace("?", "");
		Name.Replace("#", "");
		Name.Replace(",", "");
		GY_UpdateTextField(TName, Name);

		// Create button
		if(GY_ButtonHit(BDone))
		{

			// Check new character name is valid
			if(String(GY_TextFieldText(TName)).Length() < 2)
				GY_MessageBox(LanguageString[LS_Error], LanguageString[LS_InvalidCharName]);
			else
			{
				int Result = 1;
				if(PointsToSpend > 0)
					Result = GY_RequestBox(LanguageString[LS_Warning], LanguageString[LS_UnusedPoints]) ? 1 : 0;

				if(Result == 1)
				{
					// Send character information to server
					String Pa = "";
					Pa.AppendRealChar(UName.Length());
					Pa.Append(UName);
					Pa.AppendRealChar(PWord.Length());
					Pa.Append(PWord);
					
					Pa.AppendRealShort(Preview->Actor->ID);
					Pa.AppendRealChar(Preview->Gender);
					Pa.AppendRealChar(Preview->FaceTex);
					Pa.AppendRealChar(Preview->Hair);
					Pa.AppendRealChar(Preview->Beard);     
					Pa.AppendRealChar(Preview->BodyTex);
					if(AttributeAssignment > 0)
						for(int i = 0; i <= 39; ++i)
						{
							int l = Pa.Length();
							Pa.AppendRealChar(PointSpends[i]);

							if(Pa.Length() == l) // if now changed
								printf("%i", i);

						}

					Pa.Append(GY_TextFieldText(TName));




					CRCE_Send(Connection, Connection, P_CreateCharacter, Pa, true);

					// Wait for reply
					bool Done = false;
					Result = 0;
					while(Done == false)
					{
						Delay(10);

						foreach(MIt, RCE_Message, RCE_MessageList)
						{
							RCE_Message* M = *MIt;

							if(M->MessageType == P_CreateCharacter)
							{
								if(M->MessageData == String("Y"))
									Result = 2;
								else if(M->MessageData == String("I"))
									Result = 1;
								
								RCE_Message::Delete(M);
								Done = true;
								break;
							}else if(M->MessageType == ConnectionHasLeft || M->MessageType == RCE_Disconnected)
								RuntimeError(LanguageString[LS_LostConnection]);

							RCE_Message::Delete(M);

							next(MIt, RCE_Message, RCE_MessageList);
						}
						RCE_Message::Clean();

						RCE_Update();
						RCE_CreateMessages();
						GY_Update();
						GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
						UpdateWorld();
						RenderWorld();
					}

					// Add to character list and return
					if(Result == 2)
					{
						CharList.AppendRealChar(GY_TextFieldText(TName).Length());
						CharList.Append(GY_TextFieldText(TName));
						CharList.AppendRealShort(Preview->Actor->ID);
						CharList.AppendRealChar(Preview->Gender);
						CharList.AppendRealChar(Preview->FaceTex);
						CharList.AppendRealChar(Preview->Hair);
						CharList.AppendRealChar(Preview->Beard);
						CharList.AppendRealChar(Preview->BodyTex);

						SafeFreeActorInstance(Preview);
						GY_FreeGadget(WChar);
						GY_FreeGadget(WStat);
						GY_FreeGadget(LName);
						GY_FreeGadget(TName);
						GY_FreeGadget(BDone);
						GY_FreeGadget(BCancel);
						GY_FreeGadget(BLeft);
						GY_FreeGadget(BRight);
						for(int i = 0; i <= 9; ++i)
							GY_FreeGadget(LDescription[i]);
						return 2;
					}else if(Result == 1)
					{
						GY_MessageBox(LanguageString[LS_Error], LanguageString[LS_InvalidCharName]);
					}else
					{
						SafeFreeActorInstance(Preview);
						GY_FreeGadget(WChar);
						GY_FreeGadget(WStat);
						GY_FreeGadget(LName);
						GY_FreeGadget(TName);
						GY_FreeGadget(BDone);
						GY_FreeGadget(BCancel);
						GY_FreeGadget(BLeft);
						GY_FreeGadget(BRight);
						for(int i = 0; i <= 9; ++i)
							GY_FreeGadget(LDescription[i]);
						return 1;
					}
				}
			}
		}

		// Point spends
		if(AttributeAssignment > 0)
		{
			for(int i = 0; i <= 39; ++i)
			{
				if(GY_ButtonHit(AttributeDecrease[i]))
				{
					int Att = GY_GadgetData(AttributeLabels[i]).ToInt();
					if(PointSpends[Att] > 0)
					{
						++PointsToSpend;
						--PointSpends[Att];
						GY_UpdateLabel(AttributeLabels[i], String(Preview->Attributes->Value[Att] + PointSpends[Att]));
						GY_UpdateLabel(RemainingLabel, LanguageString[LS_AttributePoints] + String(" ") + String(PointsToSpend));
					}
				}else if(GY_ButtonHit(AttributeIncrease[i]))
				{
					int Att = GY_GadgetData(AttributeLabels[i]).ToInt();
					if(PointsToSpend > 0 && (Preview->Attributes->Value[Att] + PointSpends[Att]) < Preview->Attributes->Maximum[Att])
					{
						--PointsToSpend;
						++PointSpends[Att];
						GY_UpdateLabel(AttributeLabels[i], String(Preview->Attributes->Value[Att] + PointSpends[Att]));
						GY_UpdateLabel(RemainingLabel, LanguageString[LS_AttributePoints] + String(" ") + String(PointsToSpend));
					}
				}
			}
		}

		// Changed character race
		if(GY_ComboBoxItem(CRace) != Preview->Actor->Race)
		{
			SafeFreeActorInstance(Preview);
			PointsToSpend = AttributeAssignment;
			for(int i = 0; i <= 39; ++i)
				PointSpends[i] = 0;

			String ChosenRace = GY_ComboBoxItem(CRace).AsUpper();
			Actor* Chosen = 0;

			foreach(AIt, Actor, ActorList)
			{
				Actor* A = *AIt;

				if(A->Race.AsUpper() == ChosenRace)
				{
					Chosen = A;
					break;
				}

				next(AIt, Actor, ActorList);
			}
			ActorInstance* Preview = CreateActorInstance(Chosen);
			bool Result = LoadActorInstance3D(Preview);
			if(Result == false)
				RuntimeError(String("Could not load actor mesh for ") + Chosen->Race + String("!"));
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
			SetUpPreview(Preview->Actor);
			GY_UpdateLabel(RemainingLabel, LanguageString[LS_AttributePoints] + String(" ") + String(PointsToSpend));
			GY_LockGadget(BNextFace, !ActorHasFace(Preview->Actor, Preview->Gender + 1));
			GY_LockGadget(BPrevFace, !ActorHasFace(Preview->Actor, Preview->Gender + 1));
			GY_LockGadget(BNextHair, !ActorHasHair(Preview->Actor, Preview->Gender + 1));
			GY_LockGadget(BPrevHair, !ActorHasHair(Preview->Actor, Preview->Gender + 1));
			GY_LockGadget(BNextBeard, !ActorHasBeard(Preview->Actor));
			GY_LockGadget(BPrevBeard, !ActorHasBeard(Preview->Actor));
			GY_LockGadget(BNextGender, Preview->Actor->Genders);
			GY_LockGadget(BPrevGender, Preview->Actor->Genders);
		}

		// Next/Previous class
		if(GY_ButtonHit(BNextClass))
		{
			char Gender = Preview->Gender;
			Actor* A = Preview->Actor;
			
			while(true)
			{
				A = Actor::ActorList.After(A);
				if(A == 0)
					A = *Actor::ActorList.Begin();
				if(A->Race.AsUpper() == Preview->Actor->Race.AsUpper() && A->Playable == true)
				{
					SafeFreeActorInstance(Preview);
					ActorInstance* Preview = CreateActorInstance(A);
					if((Gender == 0 && A->Genders != 2) || (Gender == 1 && A->Genders != 1 && A->Genders != 3))
						Preview->Gender = Gender;
					
					bool Result = LoadActorInstance3D(Preview);
					if(Result == false)
						RuntimeError(String("Could not load actor mesh for ") + A->Race + String("!"));
					PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
					PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
					if(Preview->ShadowEN != 0)
						HideEntity(Preview->ShadowEN);
					if(Preview->NametagEN != 0)
						HideEntity(Preview->NametagEN);
					PointsToSpend = AttributeAssignment;
					for(int i = 0; i <= 39; ++i)
						PointSpends[i] = 0;
					SetUpPreview(Preview->Actor);
					GY_UpdateLabel(RemainingLabel, LanguageString[LS_AttributePoints] + String(" ") + String(PointsToSpend));
					bool AllowedFace = ActorHasFace(Preview->Actor, Preview->Gender + 1);
					GY_LockGadget(BNextFace, !AllowedFace);
					GY_LockGadget(BPrevFace, !AllowedFace);
					bool AllowedHair = ActorHasHair(Preview->Actor, Preview->Gender + 1);
					GY_LockGadget(BNextHair, !AllowedHair);
					GY_LockGadget(BPrevHair, !AllowedHair);
					GY_LockGadget(BNextBeard, !ActorHasBeard(Preview->Actor));
					GY_LockGadget(BPrevBeard, !ActorHasBeard(Preview->Actor));
					GY_LockGadget(BNextGender, Preview->Actor->Genders);
					GY_LockGadget(BPrevGender, Preview->Actor->Genders);
					break;
				}
			}
		}else if(GY_ButtonHit(BPrevClass))
		{
			char Gender = Preview->Gender;
			Actor* A = Preview->Actor;
			while(true)
			{
				A = Actor::ActorList.Before(A);
				if(A == 0)
					A = *Actor::ActorList.End();
				if(A->Race.AsUpper() == Preview->Actor->Race.AsUpper() && A->Playable)
				{
					SafeFreeActorInstance(Preview);
					ActorInstance* Preview = CreateActorInstance(A);
					if((Gender == 0 && A->Genders != 2) || (Gender == 1 && A->Genders != 1 && A->Genders != 3))
						Preview->Gender = Gender;
					
					Result = LoadActorInstance3D(Preview);
					if(Result == false)
						RuntimeError(String("Could not load actor mesh for ") + A->Race + String("!"));
					PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
					PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
					if(Preview->ShadowEN != 0)
						HideEntity(Preview->ShadowEN);
					if(Preview->NametagEN != 0)
						HideEntity(Preview->NametagEN);
					PointsToSpend = AttributeAssignment;
					for(int i = 0; i <= 39; ++i)
						PointSpends[i] = 0;
					
					SetUpPreview(Preview->Actor);
					GY_UpdateLabel(RemainingLabel, LanguageString[LS_AttributePoints] + String(" ") + String(PointsToSpend));
					bool AllowedFace = ActorHasFace(Preview->Actor, Preview->Gender + 1);
					GY_LockGadget(BNextFace, !AllowedFace);
					GY_LockGadget(BPrevFace, !AllowedFace);
					bool AllowedHair = ActorHasHair(Preview->Actor, Preview->Gender + 1);
					GY_LockGadget(BNextHair, !AllowedHair);
					GY_LockGadget(BPrevHair, !AllowedHair);
					GY_LockGadget(BNextBeard, !ActorHasBeard(Preview->Actor));
					GY_LockGadget(BPrevBeard, !ActorHasBeard(Preview->Actor));
					GY_LockGadget(BNextGender, Preview->Actor->Genders);
					GY_LockGadget(BPrevGender, Preview->Actor->Genders);
					break;
				}
			}
		}

		// Next/Previous gender
		if(GY_ButtonHit(BNextGender) || GY_ButtonHit(BPrevGender))
		{
			if(Preview->Actor->Genders == 0)
			{
				Preview->Gender = !Preview->Gender;
				Preview->BodyTex = 0;
				FreeActorInstance3D(Preview);
				LoadActorInstance3D(Preview);
				PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
				PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
				if(Preview->ShadowEN != 0)
					HideEntity(Preview->ShadowEN);
				if(Preview->NametagEN != 0)
					HideEntity(Preview->NametagEN);
			}
		}

		// Next/Previous beard
		if(GY_ButtonHit(BNextBeard))
		{
			int NextMesh = 0;
			do
			{
				++Preview->Beard;
				if(Preview->Beard > 4)
				{
					Preview->Beard = 0;
					break;
				}
				NextMesh = Preview->Actor->BeardIDs[Preview->Beard];
			}while(NextMesh == 65535);

			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
		}else if(GY_ButtonHit(BPrevBeard))
		{
			int NextMesh = 0;
			do
			{
				--Preview->Beard;
				if(Preview->Beard < 0)
					Preview->Beard = 4;
				else if(Preview->Beard == 0)
					break;
				
				NextMesh = Preview->Actor->BeardIDs[Preview->Beard];
			}while(NextMesh == 65535);
			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
			}

		// Next/Previous hair
		if(GY_ButtonHit(BNextHair))
		{
			int NextMesh = 0;
			do
			{
				++Preview->Hair;
				if(Preview->Hair > 4)
				{
					Preview->Hair = 0;
					break;
				}
				if(Preview->Gender == 0)
					NextMesh = Preview->Actor->MaleHairIDs[Preview->Hair];
				else
					NextMesh = Preview->Actor->FemaleHairIDs[Preview->Hair];
				
			}while(NextMesh == 65535);
			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
		}else if(GY_ButtonHit(BPrevHair))
		{
			int NextMesh = 0;
			do
			{
				--Preview->Hair;
				if(Preview->Hair < 0)
					Preview->Hair = 4;
				else if(Preview->Hair == 0);
					break;
				
				if(Preview->Gender == 0)
					NextMesh = Preview->Actor->MaleHairIDs[Preview->Hair];
				else
					NextMesh = Preview->Actor->FemaleHairIDs[Preview->Hair];
				
			}while(NextMesh == 65535);
			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
		}

		// Next/Previous body texture
		if(GY_ButtonHit(BNextBody))
		{
			int NextTex = 0;
			do
			{
				++Preview->BodyTex;
				if(Preview->BodyTex > 4)
					Preview->BodyTex = 0;
				if(Preview->Gender == 0)
					NextTex = Preview->Actor->MaleBodyIDs[Preview->BodyTex];
				else
					NextTex = Preview->Actor->FemaleBodyIDs[Preview->BodyTex];
				
			}while(NextTex == 65535);
			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0 )
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
		}else if(GY_ButtonHit(BPrevBody))
		{
			int NextTex = 0;
			do
			{
				--Preview->BodyTex;
				if(Preview->BodyTex < 0)
					Preview->BodyTex = 4;
				if(Preview->Gender == 0)
					NextTex = Preview->Actor->MaleBodyIDs[Preview->BodyTex];
				else
					NextTex = Preview->Actor->FemaleBodyIDs[Preview->BodyTex];

			}while(NextTex == 65535);
			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
		}

		// Next/Previous face texture
		if(GY_ButtonHit(BNextFace))
		{
			int NextTex = 0;
			do
			{
				++Preview->FaceTex;
				if(Preview->FaceTex > 4)
					Preview->FaceTex = 0;
				if(Preview->Gender == 0)
					NextTex = Preview->Actor->MaleFaceIDs[Preview->FaceTex];
				else
					NextTex = Preview->Actor->FemaleFaceIDs[Preview->FaceTex];

			}while(NextTex == 65535);
			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
		}else if(GY_ButtonHit(BPrevFace))
		{
			int NextTex = 0;
			do
			{
				--Preview->FaceTex;
				if(Preview->FaceTex < 0)
					Preview->FaceTex = 4;
				if(Preview->Gender == 0)
					NextTex = Preview->Actor->MaleFaceIDs[Preview->FaceTex];
				else
					NextTex = Preview->Actor->FemaleFaceIDs[Preview->FaceTex];
				
			}while(NextTex == 65535);
			FreeActorInstance3D(Preview);
			LoadActorInstance3D(Preview);
			PlayAnimation(Preview, 1, 0.003f, Anim_Idle);
			PositionEntity(Preview->CollisionEN, 30.0f, -(35.0f + EntityY(Preview->EN, true)), 100.0f);
			if(Preview->ShadowEN != 0)
				HideEntity(Preview->ShadowEN);
			if(Preview->NametagEN != 0)
				HideEntity(Preview->NametagEN);
		}

		// Camera
		if(GY_ButtonDown(BRight))
			CamAngle += 2.0f;
		else if(GY_ButtonDown(BLeft))
			CamAngle -= 2.0f;
		
		PositionEntity(GPP, 30.0f, 0.0f, 100.0f);
		RotateEntity(GPP, 0.0f, CamAngle, 0.0f);
		TFormPoint(-30.0f, 30.0f, -180.0f, GPP, 0);
		PositionEntity(Cam, TFormedX(), TFormedY(), TFormedZ());
		PointEntity(Cam, GPP);
		MoveEntity(Cam, 12.0f, 0.0f, 0.0f);

		// Check connection is still alive
		foreach(MIt, RCE_Message, RCE_MessageList)
		{
			RCE_Message* M = *MIt;

			if(M->MessageType == RCE_Disconnected || M->MessageType == ConnectionHasLeft)
				RuntimeError(LanguageString[LS_LostConnection]);

			next(MIt, RCE_Message, RCE_MessageList);
		}

		// Update everything
		RCE_Update();
		RCE_CreateMessages();
		GY_Update();
		RP_Update();
		GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
		UpdateWorld();
		RenderWorld();
	}
}
