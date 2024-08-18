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

#include "Language.h"


#pragma region
string LanguageStringNames[MaxLanguageString] = {
"connectingtoserver",
"fileprogress",
"updatefile",
"updateprogress",
"invalidhost",
"noresponse",
"toomanyplayers",
"lostconnection",
"receivingfiles",
"checkingfiles",
"couldnotdownload",
"username",
"password",
"emailaddr",
"connected",
"error",
"invalidusername",
"invalidpassword",
"waitingforreply",
"downloadingchars",
"invalidcharname",
"attributepoints",
"graphicsoptions",
"selectresolution",
"enableaa",
"colourdepth",
"selectcolourdepth",
"bestavailable",
"otheroptions",
"sndvolume",
"skipmusic",
"controloptions",
"cforward",
"cstop",
"cturnright",
"cturnleft",
"cflyup",
"cflydown",
"crun",
"cjump",
"cviewmode",
"czoomin",
"czoomout",
"ccamright",
"ccamleft",
"cpresskey",
"unknown",
"alreadyingame",
"weapon",
"armour",
"ring",
"amulet",
"potion",
"ingredient",
"image",
"miscellaneous",
"onehanded",
"twohanded",
"ranged",
"money",
"cost",
"xpreceived",
"questlogupdate",
"youkilled",
"pickedupitem",
"playerleftzone",
"playerenteredzone",
"enteredzone",
"xhasjoinedparty",
"youhavejoinedparty",
"couldnotjoinparty",
"partyinvite",
"partyinviteinstruction",
"couldnotinviteparty",
"tradeinvite",
"tradeinviteinstruction",
"xisoffline",
"playernotfound",
"playersingame",
"playersinzone",
"season",
"abilitynotrecharged",
"youhit",
"for",
"damagewow",
"hitsyou",
"attacksyoumisses",
"youattack",
"andmiss",
"noinventoryspace",
"memoriseability",
"memorisingability",
"maximummemorised",
"alreadymemorised",
"reputation",
"level",
"experience",
"touse",
"type",
"damage",
"indestructible",
"value",
"mass",
"canbestacked",
"cannotbestacked",
"damagetype",
"weapontype",
"armourlevel",
"effectslast",
"seconds",
"raceonly",
"classonly",
"nodescription",
"memorisedyoumust",
"moveittoactionbar",
"trading",
"tradingnospace",
"tradingnomoney",
"escapeagaintoquit",
"memorisedabilities",
"page",
"quitprogress",
"map",
"chooseamount",
"chooseamountdetail",
"party",
"leaveparty",
"abilities",
"unmemorise",
"unmemorisedetail",
"yes",
"no",
"quests",
"showcompleted",
"up",
"down",
"character",
"attributes",
"inventory",
"drop",
"use",
"increasecost",
"decreasecost",
"accept",
"cancel",
"noquestsavailable",
"completed",
"charactertitle",
"attributestitle",
"race",
"gender",
"class",
"hair",
"face",
"beard",
"clothes",
"charactername",
"unusedpoints",
"warning",
"decline",
"invertaxis1",
"invertaxis3",
"cattacktarget",
"cannotcreatechar",
"reallydeletechar",
"calwaysrun",
"ccycletarget",
"windowedmode",
"youarebanned",
"accountdoesnotexist",
"invalidemailaddress",
"usernamealreadyexists",
"success",
"newaccountcreated",
"criticaldamage",
"weapondamaged",
"ignoring",
"unignoring",
"cmoveto",
"ctalkto",
"faction",
"interact",
"retrievinglist",
"connect",
"availableservers",
"cselect",
"anisotropylevel",
"disabled",
"newcharacter",
"help",
"shadowdisabled",
"shadowplayers",
"shadowallactors",
"shadowscenery",
"shadowtrees",
"shadowdetail",
"shaderquality",
"qualityhigh",
"qualitymedium",
"qualitylow",
"grassdistance",
"optionfullscreen",
"ok",
"loaddefaultshaders",
"loadshadowshaders",
"loadusershaders",
"loaduserparameters",
"loadterrainmanager",
"loadtreemanager",
"loadsdk",
"loadoptions",
"loadgubbins",
"loadclient"
};
#pragma endregion


string LanguageString[MaxLanguageString];

// Loads all language strings from file
bool LoadLanguage(string Filename)
{
	// Read XML
	XMLReader* X = ReadXMLFile("Data\\Game Data\\Language.xml");
	if(X == 0)
		return false;

	// Only read the <language> block
	bool ReadLanguage = false;
	while(X->Read())
	{
		string ElementNameLower = stringToLower(X->GetNodeName());

		if(X->GetNodeType() == XNT_Element)
		{
			// If its a language block, then allow reading
			if(ElementNameLower.compare("language") == 0)
				ReadLanguage = true;

			// If its an entry element, then add it to the known language list
			if(ElementNameLower.compare("entry") == 0 && ReadLanguage)
				for(int i = 0; i < MaxLanguageString; ++i)
					if(LanguageStringNames[i].compare(stringToLower(X->GetAttributeString("name"))) == 0)
						LanguageString[i] = X->GetAttributeString("value");
			
		}else if(X->GetNodeType() == XNT_Element_End) // If there is a closing language block, disable reading
			if(ElementNameLower.compare("language") == 0)
				ReadLanguage = false;
	}

	// Close file
	delete X;
	return true;
}

