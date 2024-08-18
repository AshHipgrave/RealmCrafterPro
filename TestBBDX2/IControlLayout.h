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

			SLayoutInstance()
				: Category("Unknown"), Name("Unk"), ControlID(1), DefaultID(1)
			{ }

			SLayoutInstance(std::string &category, std::string &name, int controlID)
				: Category(category), Name(name), ControlID(controlID), DefaultID(controlID)
			{ }
		};

		// Add a control binding to the layout.
		virtual void AddInstance(std::string category, std::string name, int controlID) = 0;

		// Get a control binding by name.
		virtual SLayoutInstance* Get(std::string name) = 0;

		// Get all control bindings.
		virtual const std::vector<SLayoutInstance*>* GetInstances() = 0;

	};

}