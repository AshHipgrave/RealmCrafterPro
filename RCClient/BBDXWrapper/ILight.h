#pragma once

class IEntity;

#include <bbdx.h>
#include <Vector2.h>
#include <Vector3.h>
#include <Color.h>

class ILight
{
protected:

	uint _Handle;
	ELightType _LightType;

public:

	ILight(uint handle, ELightType type);

	// Lighting
	__declspec(property(put=zzSet_Direction)) NGin::Math::Vector3 Direction;
	virtual void zzSet_Direction(NGin::Math::Vector3& direction);

	__declspec(property(put=zzSet_Position)) NGin::Math::Vector3 Position;
	virtual void zzSet_Position(NGin::Math::Vector3& position);

	__declspec(property(put=zzSet_Color)) NGin::Math::Color Color;
	virtual void zzSet_Color(NGin::Math::Color& color);

	__declspec(property(put=zzSet_Radius)) float Radius;
	virtual void zzSet_Radius(float radius);

	__declspec(property(put=zzSet_Active)) bool Active;
	virtual void zzSet_Active(bool active);
};

