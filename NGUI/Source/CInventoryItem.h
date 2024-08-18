#pragma once

#include "IInventoryItem.h"
#include "ILabel.h"
#include "CGUIManager.h"

class CGUIManager;
class CInventoryItem : public IInventoryItem
{
protected:

	CGUIManager* Manager;
	IButton* Button;

	void* _BackgroundImage, *_IconImage;

	void RebuildMesh();

	virtual void OnTransform();
	virtual void OnTextChange();
	virtual void OnEnabledChange();
	virtual void OnBackColorChange();
	virtual void OnForeColorChange();
	virtual void OnSkinChange();
	virtual void OnSizeChange();

	virtual void OnDeviceLost();
	virtual void OnDeviceReset();
	virtual bool Update(GUIUpdateParameters* Parameters);
	virtual void Render();

public:

	CInventoryItem(NGin::Math::Vector2& ScreenScale);
	virtual ~CInventoryItem();
	virtual bool Initialize(CGUIManager* Manager);

	virtual void BackgroundImage(void* Image);
	virtual void IconImage(void* Image);

	virtual void* BackgroundImage();
	virtual void* IconImage();
};