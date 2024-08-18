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

#include "ShaderLibrary.h"

ArrayList<LoadedEffect*> LoadedShaders;
ArrayList<uint> LoadedProfiles;
uint DefaultStatic = 0, DefaultAnimated = 0;

// Gets a particular profile
uint GetProfile(int ID)
{
	if(ID < 0 || ID >= LoadedProfiles.Size())
		return 0;

	return LoadedProfiles[ID];
}

// Gets a particular shader
int GetShader(int ID)
{
	if(ID < 0 || ID >= LoadedShaders.Size())
		return 0;

	return LoadedShaders[ID]->Effect;
}

// Load all shader names and profiles from file
void LoadShaders()
{
	int EffectCount = 0, ProfileCount = 0;

	// Read shaders file
	XMLReader* X = ReadXMLFile("Data\\Game Data\\Shaders\\Shaders.xml");
	if(X == 0)
		RuntimeError("Could not open Data\\Game Data\\Shaders\\Shaders.xml");

	// Get values
	while(X->Read())
	{
		std::string NodeName = X->GetNodeName();
		std::transform(NodeName.begin(), NodeName.end(), NodeName.begin(), ::tolower);

		// If its a <shader> element
		if(X->GetNodeType() == XNT_Element && NodeName.compare("shader") == 0)
		{
			LoadedEffect* Le = new LoadedEffect();
			Le->Name = X->GetAttributeString("name");
			Le->ID = X->GetAttributeInt("id");
			Le->Path = X->GetAttributeString("path");
			Le->Effect = LoadShader(std::string("Data\\Game Data\\Shaders\\") + Le->Path);

			LoadedShaders[Le->ID] = Le;
			++EffectCount;
		}
	}

	// Close file
	delete X;

	// Read profiles
	X = ReadXMLFile("Data\\Game Data\\Shaders\\Profiles.xml");
	if(X == 0)
		RuntimeError("Could not open Data\\Game Data\\Shaders\\Profiles.xml");

	// Read in
	while(X->Read())
	{
		std::string NodeName = X->GetNodeName();
		std::transform(NodeName.begin(), NodeName.end(), NodeName.begin(), ::tolower);

		// If its a <profile> element
		if(X->GetNodeType() == XNT_Element && NodeName.compare("profile") == 0)
		{
			int ID = X->GetAttributeInt("id");
			float Range = X->GetAttributeFloat("range");
			std::string Name = X->GetAttributeString("name");

			int hnid = X->GetAttributeInt("hn");
			int hfid = X->GetAttributeInt("hf");
			int mnid = X->GetAttributeInt("mn");
			int mfid = X->GetAttributeInt("mf");
			int lnid = X->GetAttributeInt("ln");
			int lfid = X->GetAttributeInt("lf");

			uint Pr = CreateProfile(Name.c_str());
			SetProfileEffect(Pr, 2, 1, GetShader(hnid));
			SetProfileEffect(Pr, 2, 0, GetShader(hfid));
			SetProfileEffect(Pr, 1, 1, GetShader(mnid));
			SetProfileEffect(Pr, 1, 0, GetShader(mfid));
			SetProfileEffect(Pr, 0, 1, GetShader(lnid));
			SetProfileEffect(Pr, 0, 0, GetShader(lfid));
			SetProfileRange(Pr, Range);

			std::string Default = X->GetAttributeString("default");
			std::transform(Default.begin(), Default.end(), Default.begin(), ::tolower);

			if(Default.compare("static") == 0)
				DefaultStatic = Pr;
			if(Default.compare("animated") == 0)
				DefaultAnimated = Pr;

			if(DefaultStatic == 0 && DefaultAnimated != Pr)
				DefaultStatic = Pr;
			if(DefaultAnimated == 0 && DefaultStatic != Pr)
				DefaultAnimated = Pr;

			LoadedProfiles[ID] = Pr;
			++ProfileCount;
		}
	}

	//DebugLog(std::string("Loaded: ") + std::stringstream(EffectCount).str() + " Effects");
	//DebugLog(std::string("Loaded: ") + std::stringstream(ProfileCount).str() + " Profiles");
}

