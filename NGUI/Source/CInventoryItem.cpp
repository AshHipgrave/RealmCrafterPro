#include "CInventoryItem.h"

Type IInventoryItem::TypeOf()
{
	return Type("RCII", "CInventoryItem GUI InventoryItem");
}

CInventoryItem::CInventoryItem(NGin::Math::Vector2& ScreenScale) : IInventoryItem(ScreenScale)
{
	_Type = Type("RCII", "CInventoryItem GUI InventoryItem");
	Manager = 0;
	_BackgroundImage = 0;
	_IconImage = 0;
}

CInventoryItem::~CInventoryItem()
{
}

void CInventoryItem::OnDeviceLost()
{
	IControl::OnDeviceLost();
}

void CInventoryItem::OnDeviceReset()
{
	IControl::OnDeviceReset();
}

bool CInventoryItem::Update(GUIUpdateParameters* Parameters)
{
	if(!_Enabled)
		return true;

	List<IControl*> Controls;
	foreach(CIt, IControl, _Controls)
	{
		Controls.Insert((*CIt), 0);

		next(CIt, IControl, _Controls);
	}

	foreachf(cCIt, IControl, Controls)
	{
		(*cCIt)->Update(Parameters);

		nextf(cCIt, IControl, Controls);
	}

	return true;
}

void CInventoryItem::Render()
{
	if(!_Visible)
		return;
	if(_Skin == 0)
		return;

	foreach(CIt, IControl, _Controls)
	{
		(*CIt)->Render();

		next(CIt, IControl, _Controls);
	}
}

bool CInventoryItem::Initialize(CGUIManager* Manager)
{
	this->Manager = Manager;

	Button = Manager->CreateButton("CInventoryItem::Button", Vector2(0, 0), Vector2(0, 0));
	_Skin = Button->Skin();
	
	Button->Align(TextAlign_Right);
	Button->VAlign(TextAlign_Bottom);

	return true;
}

void CInventoryItem::RebuildMesh()
{
}

void CInventoryItem::OnSizeChange()
{
	IControl::OnSizeChange();
}

void CInventoryItem::OnTransform()
{
	IControl::OnTransform();

	Button->Size(_Size);
	Button->Location(_Location);
}

void CInventoryItem::OnTextChange()
{
	IControl::OnTextChange();

	Button->Text(_Text);
}

void CInventoryItem::OnEnabledChange()
{
	IControl::OnEnabledChange();

	Button->Enabled(_Enabled);
}

void CInventoryItem::OnBackColorChange()
{
	IControl::OnBackColorChange();

	Button->BackColor(_BackColor);
}

void CInventoryItem::OnForeColorChange()
{
	IControl::OnForeColorChange();

	Button->ForeColor(_ForeColor);
}

void CInventoryItem::OnSkinChange()
{
	IControl::OnSkinChange();

	Button->Skin(_Skin);
}

void CInventoryItem::BackgroundImage(void* Image)
{
	_BackgroundImage = Image;
}

void CInventoryItem::IconImage(void* Image)
{
	_IconImage = Image;
}

void* CInventoryItem::BackgroundImage()
{
	return _BackgroundImage;
}

void* CInventoryItem::IconImage()
{
	return _IconImage;
}

IInventoryItem* CreateInventoryItem(IGUIManager* Manager, NGin::WString Name, NGin::Math::Vector2 Location, NGin::Math::Vector2 Size)
{
	// Create
	CInventoryItem* II = new CInventoryItem(NGin::Math::Vector2(1.0f, 1.0f) / Manager->GetResolution());
	II->Initialize((CGUIManager*)Manager);
	II->Locked(true);
	II->Name(Name);
	II->Text(Name);
	II->Location(Location);
	II->Size(Size);

	II->Locked(false);

	II->Parent((CGUIManager*)Manager);

	return II;
}
