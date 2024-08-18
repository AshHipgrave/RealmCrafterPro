#pragma once

#include "IControl.h"

class IGUIManager;

//! InventoryItem Interface class
/*!
TODO: InventoryItem Link
*/
class IInventoryItem : public IControl
{
public:

	IInventoryItem(NGin::Math::Vector2& ScreenScale) : IControl(ScreenScale) {}
	virtual ~IInventoryItem() {}


	virtual void BackgroundImage(void* Image) = 0;
	virtual void IconImage(void* Image) = 0;

	virtual void* BackgroundImage() = 0;
	virtual void* IconImage() = 0;

	//! Returns the IInventoryItem Type
	/*!
	TODO: Types
	*/
	static NGin::Type TypeOf();
};

IInventoryItem* CreateInventoryItem(IGUIManager* Manager, NGin::WString Name, NGin::Math::Vector2 Location, NGin::Math::Vector2 Size);
