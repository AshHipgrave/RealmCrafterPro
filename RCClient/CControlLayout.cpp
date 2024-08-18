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
#pragma once

#include "CControlLayout.h"
#include <BlitzPlus.h>
#include "Temp.h"

namespace RealmCrafter
{
	CControlLayout::CControlLayout()
	{

	}

	CControlLayout::~CControlLayout()
	{
		for(int i = 0; i < Instances.size(); ++i)
		{
			delete Instances[i];
		}
		Instances.clear();
	}

	void CControlLayout::AddInstance( std::string category, std::string name, int controlID )
	{
		// Don't re-add anything
		for(int i = 0; i < Instances.size(); ++i)
		{
			if(Instances[i]->Name == name)
				return;
		}

		Instances.push_back(new IControlLayout::SLayoutInstance(category, name, controlID));
	}

	IControlLayout::SLayoutInstance* CControlLayout::Get( std::string name )
	{
		for(int i = 0; i < Instances.size(); ++i)
		{
			if(Instances[i]->Name == name)
				return Instances[i];
		}

		return NULL;
	}

	const std::vector<IControlLayout::SLayoutInstance*>* CControlLayout::GetInstances()
	{
		return &Instances;
	}

	void CControlLayout::Save(std::string path)
	{
		FILE* F = WriteFile(path);

		if(F == 0)
		{
			DebugLog(std::string("Warning: Could not open: ") + path);
			return;
		}

		WriteLine(F, "<?xml version=\"1.0\"?>");
		WriteLine(F, "<keybindings>");

		for(int i = 0; i < Instances.size(); ++i)
		{
			WriteLine(F, std::string("\t<key name=\"") + Instances[i]->Name + std::string("\" id=\"") + toString(Instances[i]->ControlID) + string("\" />"));
		}

		// Done, close
		WriteLine(F, "</keybindings>");
		CloseFile(F);
	}

	void CControlLayout::Load(std::string path)
	{
		// Read options file
		XMLReader* X = ReadXMLFile(path);
		if(X == 0)
		{
			DebugLog(std::string("Warning: Could not open: ") + path);
			return;
		}

		// Get values
		while(X->Read())
		{
			string ElementNameLower = stringToLower(X->GetNodeName());

			// If its a <key> element
			if(X->GetNodeType() == XNT_Element && ElementNameLower.compare("key") == 0)
			{
				string AttributeNameLower = stringToLower(X->GetAttributeString("name"));
				int ID = X->GetAttributeInt("id");

				bool Found = false;
				for(int i = 0; i < Instances.size(); ++i)
				{
					if(stringToLower(Instances[i]->Name) == AttributeNameLower)
					{
						Instances[i]->ControlID = ID;
						Found = true;
						break;
					}
				}

				if(!Found)
				{
					DebugLog(std::string("Warning: KeyControl '") + AttributeNameLower + "' read but not found in layout. Definition was skipped");
				}
			}
		}

		// Close file
		delete X;
	}
}