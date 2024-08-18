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
#include "LightFunctionList.h"
#include "Environment.h"

namespace RealmCrafter
{
	std::vector<LightFunction*> LightFunctionList::Functions;

	void LightFunctionList::ClearFunctions()
	{
		for(int i = 0; i < Functions.size(); ++i)
			delete Functions[i];
		Functions.clear();
	}

	void LightFunctionList::LoadFunctions()
	{
		ClearFunctions();

		XMLReader* X = ReadXMLFile("Data\\Game Data\\LightFunctions.xml");
		if(X == 0)
			return;

		while(X->Read())
		{
			string ElementNameLower = stringToLower(X->GetNodeName());

			if(X->GetNodeType() == NGin::XNT_Element && ElementNameLower.compare("function") == 0)
			{
				LightFunction* Function = new LightFunction();
				Function->Name = X->GetAttributeString("name");

				Function->Compile(X);
				Functions.push_back(Function);
			}
		}

		delete X;
	}

	void LightFunctionList::Update()
	{
		for(int i = 0; i < Functions.size(); ++i)
		{
			Functions[i]->Update(Environment->GetTimeH(), Environment->GetTimeM(), Environment->GetTimeS());
		}
	}

	bool LightFunctionList::Verify(LightFunction* function)
	{
		for(int i = 0; i < Functions.size(); ++i)
			if(Functions[i] == function)
				return true;
		return false;
	}
}