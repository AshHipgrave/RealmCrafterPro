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

#include "XMLWrapper.h"
#include "irrxml\irrXML.h"
#include <string>
#include <windows.h>

using namespace irr;
using namespace io;

namespace NGin
{
	XMLReader* ReadXMLFile(std::string FileName)
	{
		IrrXMLReader* R = createIrrXMLReader(FileName.c_str());
		if(R == 0)
			return 0;

		XMLReader* Reader = new XMLReader(R);
		return Reader;
	}

	XMLReader::XMLReader(void *NewReader)
	{
		Reader = NewReader;
	}

	XMLReader::~XMLReader()
	{
		delete Reader;
	}

	bool XMLReader::Read()
	{
		if(Reader && ((IrrXMLReader*)Reader)->read())
			return true;
		return false;
	}

	XMLNodeType XMLReader::GetNodeType()
	{
		return (XMLNodeType)((IrrXMLReader*)Reader)->getNodeType();
	}

	std::string XMLReader::GetNodeData()
	{
		return ((IrrXMLReader*)Reader)->getNodeData();
	}

	std::string XMLReader::GetNodeName()
	{
		return ((IrrXMLReader*)Reader)->getNodeName();
	}

	int XMLReader::GetAttributeCount()
	{
		return ((IrrXMLReader*)Reader)->getAttributeCount();
	}

	std::string XMLReader::GetAttributeName(int Index)
	{
		return ((IrrXMLReader*)Reader)->getAttributeName(Index);
	}

	int XMLReader::GetAttributeInt(std::string Name)
	{
		return ((IrrXMLReader*)Reader)->getAttributeValueAsInt(Name.c_str());
	}

	float XMLReader::GetAttributeFloat(std::string Name)
	{
		return ((IrrXMLReader*)Reader)->getAttributeValueAsFloat(Name.c_str());
	}

	std::string XMLReader::GetAttributeString(std::string Name)
	{
		const char* Str = ((IrrXMLReader*)Reader)->getAttributeValue(Name.c_str());
		if(Str == 0)
			return std::string("");
		else
			return std::string(Str);
		//OutputDebugString("S: ");
		//OutputDebugString(Str);
		//OutputDebugString("\n");
		return std::string(Str);
	}
		
}