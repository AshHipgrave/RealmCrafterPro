#pragma once

class IEntity;

#include <bbdx.h>
#include <Vector2.h>
#include <Vector3.h>
#include <Color.h>
#include "IEntity.h"

class ICamera : public IEntity
{
public:

	ICamera(uint handle);

	// Range/Fog
	__declspec(property(put=zzSet_ClearColor)) NGin::Math::Color ClearColor;
	virtual void zzSet_ClearColor(NGin::Math::Color color);
	virtual void Range(float near, float far);
	virtual void FogMode(int mode);
	virtual void FogRange(float near, float far);

	__declspec(property(put=zzSet_FogColor)) NGin::Math::Color FogColor;
	virtual void zzSet_FogColor(NGin::Math::Color color);

	// Collision
	virtual uint Pick(NGin::Math::Vector2 location);

};
