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

#include "ClientLoaders.h"

// General game options (host name, etc. - NOT stuff set by the player)
void LoadOptions(bool reload)
{
	if(!reload)
	{
		if(GY_SClick != 0)
			SoundVolume(GY_SClick, RealmCrafter::Globals->DefaultVolume);
		if(GY_SBeep != 0)
			SoundVolume(GY_SBeep, RealmCrafter::Globals->DefaultVolume);



		// Hosts
		FILE* F = ReadFile("Data\\Game Data\\Hosts.dat");
		if(F == 0)
			RuntimeError("Could not open Data\\Game Data\\Hosts.dat");
		RealmCrafter::Globals->ServerHost = ReadLine(F);
		UpdateHost = ReadLine(F);
		CloseFile(F);

		if(UpdateHost.substr(UpdateHost.length() -1, 1).compare("/") == 0)
			UpdateHost.append("/");

		// Misc stuff
		F = ReadFile("Data\\Game Data\\Misc.dat");
		if(F == 0)
			RuntimeError("Could not open Data\\Game Data\\Misc.dat!");
		RealmCrafter::Globals->GameName = ReadLine(F);
		UpdateGame = ReadLine(F);
		UpdateMusic = (toInt(ReadLine(F)) == 1);
		CloseFile(F);
		AppTitle(RealmCrafter::Globals->GameName);

	}

	// Control bindings
// 	bool Result = LoadControlBindings("Data\\Controls.dat");
// 	if(Result == false)
// 		RuntimeError("Could not open Data\\Controls.dat!");

	if(!reload)
	{
		// Other Settings
		FILE* F = ReadFile("Data\\Game Data\\Other.dat");
		if(F == 0)
			RuntimeError("Could not open Data\\Game Data\\Other.dat!");

		HideNametags = ReadByte(F);
		DisableCollisions = ReadByte(F);
		byte OldViewMode = ReadByte(F);
		RealmCrafter::Globals->ServerPort = ReadInt(F);
		if(RealmCrafter::Globals->ServerPort == 0)
			RealmCrafter::Globals->ServerPort = 25000;
		RealmCrafter::Globals->RequireMemorise = ReadByte(F);
		UseBubbles = ReadByte(F) - 1;
		BubblesR = ReadByte(F);
		BubblesG = ReadByte(F);
		BubblesB = ReadByte(F);
// 		AlwaysShowEULA = ReadByte(F);

		ShowActorsOnRadar = (bool)ReadByte(F);
		idPlayer = ReadShort(F);
		idEnemy = ReadShort(F);
		idFriendly = ReadShort(F);
		idNPC = ReadShort(F);
		idOthers = ReadShort(F);

		CloseFile(F);
	}

	WriteLog(MainLog, "Loaded options from file");

}

// Loads general game media
void LoadGame(bool postMenu)
{
	if(postMenu)
	{
		// My mesh/textures etc.
		if(GPP == 0)
			GPP = CreatePivot();

		bool Result = LoadActorInstance3D(Me, 0.05f);
		if(Result == false)
			RuntimeError(string("Could not load actor mesh for ") + Me->Actor->Race + string("!"));
		Me->RNID = 1;
		EntityType(Me->CollisionEN, C_Player);

		if(Me->NametagEN)
			GUIManager->Destroy(Me->NametagEN);
		if(Me->TagEN != 0)
			GUIManager->Destroy(Me->TagEN);
		Me->NametagEN = 0;
		Me->TagEN = 0;
		uint Bonce = FindChild(Me->EN, "Head");
		if(Bonce == 0)
			RuntimeError(Me->Actor->Race + string(" actor mesh is missing a 'Head' joint!"));

		// Sort player known spells alphabetically
		SortSpells();

		return;
	}


	if(GY_SClick != 0)
		SoundVolume(GY_SClick, RealmCrafter::Globals->DefaultVolume);
	if(GY_SBeep != 0)
		SoundVolume(GY_SBeep, RealmCrafter::Globals->DefaultVolume);

	// Money settings
	FILE* F = ReadFile("Data\\Game Data\\Money.dat");
	if(F == 0)
		RuntimeError("Could not open Data\\Game Data\\Money.dat!");
	RealmCrafter::Globals->Money1 = ReadString(F);
	RealmCrafter::Globals->Money2 = ReadString(F);
	RealmCrafter::Globals->Money2x = ReadShort(F);
	RealmCrafter::Globals->Money3 = ReadString(F);
	RealmCrafter::Globals->Money3x = ReadShort(F);
	RealmCrafter::Globals->Money4 = ReadString(F);
	RealmCrafter::Globals->Money4x = ReadShort(F);
	CloseFile(F);


// 	GUIManager->ImportProperties("Data\\Game Data\\Interface.xml");


	// Camera and lighting
	CameraRange(Cam, 1.0f, 500.0f);
	//EntityRadius(Cam, 1.0f);
	//EntityType(Cam, C_Cam);
	
	AmbientLight(90, 90, 90);
	TerrainManager->SetAmbientLight(NGin::Math::Color(90, 90, 90));


	//SetPPFolder("Effects\\");
	//LoadPPChain("BlurChain.chn");
	//SetPPActive("Effect1", 1);
	//SetPPActive("Effect2", 1);

	// 3D Sound
	CreateListener(Cam, 0.1f, 1.0f, 3.0f);



	// Gubbin Joint Names
	LoadGubbinNames();


	// Loot bag mesh
	LootBagEN = LoadMesh("Data\\Meshes\\Loot Bag.b3d");
	if(LootBagEN == 0)
		RuntimeError("File not found: Data\\Meshes\\Loot Bag.b3d");
	ScaleMesh(LootBagEN, 0.075f, 0.075f, 0.075f);
	EntityRadius(LootBagEN, 1.0f);//MeshWidth(LootBagEN) * 0.6f);
	HideEntity(LootBagEN);

	

	// User inteface
	CreateInterface();

	// Combat settings
	LoadCombat();

	// Screen flash quad
	FlashEN = CreateSAQuad();
	SAQuadLayout(FlashEN, 0.0f, 0.0f, 1.0f, 1.0f);
	EntityAlpha(FlashEN, 0.0f);
	EntityOrder(FlashEN, -3020);
	uint DefaultTexture = LoadTexture("Data\\DefaultTex.png");
	if(DefaultTexture == 0)
		RuntimeError("Could not load: Data\\DefaultTex.png");
	EntityTexture(FlashEN, DefaultTexture);

	// Enable collisions
	if(DisableCollisions == 0)
	{
		Collisions(C_Player, C_Actor, 2, 3); // Actor->Scenery
		Collisions(C_Actor, C_Player, 2, 3);
		Collisions(C_Actor, C_Actor, 2, 3);
		Collisions(C_Player, C_ActorTri2, 2, 3);
		Collisions(C_Actor, C_ActorTri2, 2, 3);
	}

	Collisions(C_Player, C_Sphere, 2, 3);
	Collisions(C_Player, C_Box, 2, 3);
	Collisions(C_Player, C_Triangle, 2, 3);
	Collisions(C_Actor, C_Sphere, 2, 3);
	Collisions(C_Actor, C_Box, 2, 3);
	Collisions(C_Actor, C_Triangle, 2, 3);
	Collisions(C_ActorTri1, C_Sphere, 2, 3);
	Collisions(C_ActorTri1, C_Box, 2, 3);
	Collisions(C_ActorTri1, C_Triangle, 2, 3);

	// Selected actor highlights
	ActorSelectEN = CreateMesh();
	uint S = CreateSurface(ActorSelectEN);
	uint v1 = AddVertex(S, -1.0f, 0.0f, -1.0f, 0.0f, 0.0f);
	uint v2 = AddVertex(S, -1.0f, 0.0f,  1.0f, 0.0f, 1.0f);
	uint v3 = AddVertex(S,  1.0f, 0.0f,  1.0f, 1.0f, 1.0f);
	uint v4 = AddVertex(S,  1.0f, 0.0f, -1.0f, 1.0f, 0.0f);
	AddTriangle(S, v1, v2, v3);
	AddTriangle(S, v1, v3, v4);
	UpdateHardwareBuffers(ActorSelectEN);
	EntityFX(ActorSelectEN, 1 + 8 + 64);
	EntityShader(ActorSelectEN, Shader_FullbrightAdd);
	uint Tex = LoadTexture("Data\\Textures\\Select.png");
	if(Tex == 0)
		RuntimeError("File not found: Data\\Textures\\Select.png!");
	EntityTexture(ActorSelectEN, Tex);
	HideEntity(ActorSelectEN);

	// Movement click marker
	ClickMarkerEN = CopyEntity(ActorSelectEN);
	EntityFX(ClickMarkerEN, 1 + 8 + 64);
	TagEntity(ClickMarkerEN, "ClickMarker");
	Tex = LoadTexture("Data\\Textures\\Marker.png");
	if(Tex == 0)
		RuntimeError("File not found: Data\\Textures\\Marker.png!");
	EntityTexture(ClickMarkerEN, Tex);
	HideEntity(ClickMarkerEN);
	ClickMarkerTimer = MilliSecs();

	// Cound random images available for zone loading screens
	if(FileType("Data\\Textures\\Random") == 2)
	{
		uint D = ReadDir("Data\\Texture\\Random");
		while(true)
		{
			string File = NextFile(D);
			if(File.length() == 0)
				break;
			if(FileType(string("Data\\Textures\\Random\\") + File) == 1)
				++RandomImages;
		}
		CloseDir(D);
	}else
		RandomImages = 0;

	

	LoadUserDefinedPP_FromXML("Data\\Game Data\\PostProcess\\PostProcess.xml");
	SetPP_Pipeline("Default");

	// Done
	WriteLog(MainLog, "Loaded general game media");
}