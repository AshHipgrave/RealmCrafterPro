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

#include <string>

namespace NGin
{
	enum XMLNodeType
	{
		//! Nothing
		XNT_None,

		//! Element like <element
		XNT_Element,

		//! Ending element like </element>
		XNT_Element_End,

		//! Text within an element <pre>Hello!</pre>
		XNT_Text,

		//! XML Comment <!-- -->
		XNT_Comment,

		//! XML CDATA section
		XNT_CData,

		//! Unknown section
		XNT_Unknown
	};

	//! XML Reader class
	class XMLReader
	{
		void* Reader;

	public:

		XMLReader(void* NewReader);
		~XMLReader();

		//! Read the next section of the file
		bool Read();

		//! Get the current node type
		XMLNodeType GetNodeType();

		//! Get the name of the current node <nodename>
		std::string GetNodeName();

		//! Get the data of the current name <pre>Data here</pre>
		std::string GetNodeData();

		//! Get the number of node attributes
		int GetAttributeCount();

		//! Get the name of an attribute at a specific index
		std::string GetAttributeName(int Index);

		//! Get an attribute as a string
		std::string GetAttributeString(std::string Name);

		//! Get an attribute as a float
		float GetAttributeFloat(std::string Name);

		//! Get an attribute as an integer
		int GetAttributeInt(std::string Name);
	};

	//! Read an XML File (returns 0 on error)
	XMLReader* ReadXMLFile(std::string FileName);
}