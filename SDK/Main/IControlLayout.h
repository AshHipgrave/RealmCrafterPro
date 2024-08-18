//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#pragma once

#include <string>
#include <vector>

namespace RealmCrafter
{
	// Control Layout class for keybindings
	class IControlLayout
	{
	public:

		// Single controlbinding instance
		struct SLayoutInstance
		{
			std::string Category;
			std::string Name;
			int ControlID;
			int DefaultID;
			int EditID;

			SLayoutInstance()
				: Category("Unknown"), Name("Unk"), ControlID(1), DefaultID(1), EditID(1)
			{ }

			SLayoutInstance(std::string &category, std::string &name, int controlID)
				: Category(category), Name(name), ControlID(controlID), DefaultID(controlID), EditID(controlID)
			{ }
		};

		// Add a control binding to the layout.
		virtual void AddInstance(std::string category, std::string name, int controlID) = 0;

		// Get a control binding by name.
		virtual SLayoutInstance* Get(std::string name) = 0;

		// Get all control bindings.
		virtual const std::vector<SLayoutInstance*>* GetInstances() = 0;

		// Save control bindings
		virtual void Save(std::string path) = 0;

		// Reload control bindings
		virtual void Load(std::string path) = 0;
	};

}