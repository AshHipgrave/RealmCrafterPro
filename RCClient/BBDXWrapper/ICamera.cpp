#include "IEntity.h"
#include "ICamera.h"

ICamera::ICamera(uint handle) : IEntity(handle)
{

}

void ICamera::zzSet_ClearColor(NGin::Math::Color color)
{
	CameraClsColor(_Handle,
		(int)(color.R * 255.0f),
		(int)(color.G * 255.0f),
		(int)(color.B * 255.0f));
}

void ICamera::Range(float nearRange, float farRange)
{
	CameraRange(_Handle, nearRange, farRange);
}

void ICamera::FogMode(int mode)
{
	CameraFogMode(_Handle, mode);
}

void ICamera::FogRange(float nearRange, float farRange)
{
	CameraFogRange(_Handle, nearRange, farRange);
}

void ICamera::zzSet_FogColor(NGin::Math::Color color)
{
	CameraFogColor(_Handle,
		(int)(color.R * 255.0f),
		(int)(color.G * 255.0f),
		(int)(color.B * 255.0f));
}

uint ICamera::Pick(NGin::Math::Vector2 location)
{
	return CameraPick(_Handle, location.X, location.Y);
}

