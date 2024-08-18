#include "ELightType.h"
#include "IEntity.h"
#include "ILight.h"

ILight::ILight(uint handle, ELightType type)
{
	_Handle = handle;
	_LightType = type;
}

void ILight::zzSet_Direction(NGin::Math::Vector3& direction)
{
	if(_LightType == ELT_Directional)
		SetLightDirection(_Handle, direction.X, direction.Y, direction.Z);
}

void ILight::zzSet_Position(NGin::Math::Vector3& position)
{
	if(_LightType == ELT_Point)
		SetLightPosition(_Handle, position.X, position.Y, position.Z);
}

void ILight::zzSet_Color(NGin::Math::Color& color)
{
	switch(_LightType)
	{
	case ELT_Point:
		{
			SetPLightColor(_Handle,
				(int)(color.R * 255.0f),
				(int)(color.G * 255.0f),
				(int)(color.B * 255.0f));
			break;
		}
	case ELT_Directional:
		{
			SetDLightColor(_Handle,
				(int)(color.R * 255.0f),
				(int)(color.G * 255.0f),
				(int)(color.B * 255.0f));
			break;
		}
	}
}

void ILight::zzSet_Radius(float radius)
{
	if(_LightType == ELT_Point)
		SetLightRadius(_Handle, radius);
}

void ILight::zzSet_Active(bool active)
{
	switch(_LightType)
	{
	case ELT_Point:
		{
			SetPLightActive(_Handle, active);
			break;
		}
	case ELT_Directional:
		{
			SetDLightActive(_Handle, active);
			break;
		}
	}
}
