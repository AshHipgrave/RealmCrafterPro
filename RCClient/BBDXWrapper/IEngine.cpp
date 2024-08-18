#include "ELightType.h"
#include "IEntity.h"
#include "ICamera.h"
#include "ILight.h"
#include "IEngine.h"


uint bbdx2_CreateCamera(uint Parent)
{
	return CreateCamera(Parent);
}

IDirect3DDevice9* IEngine::zzGet_Direct3DDevice()
{
	return GetIDirect3DDevice9();
}

void IEngine::RenderWorld()
{
	::RenderWorld();
}

void IEngine::UpdateWorld()
{
	::UpdateWorld();
}

uint IEngine::LoadShader(std::string name)
{
	return ::LoadShader(name);
}

void IEngine::AmbientLight(NGin::Math::Color& color)
{
	qwedfy((int)(color.R * 255.0f),
		(int)(color.G * 255.0f),
		(int)(color.B * 255.0f));
}

void IEngine::Collisions(int srcType, int destType, int method, int response)
{
	//anyonebutme(srcType, destType, method, response);
	bbdx2_Collisions(srcType, destType);
}

IEntity* IEngine::LoadMesh(std::string name, IEntity* parent , bool animated)
{
	uint ParentHandle = 0;
	if(parent != 0)
		ParentHandle = parent->Handle;

	uint Handle = ::LoadMesh(name, ParentHandle, animated);
	if(Handle != 0)
		return new IEntity(Handle);
	else
		return 0;
}

IEntity* IEngine::CreateMesh(IEntity* parent)
{
	uint ParentHandle = 0;
	if(parent != 0)
		ParentHandle = parent->Handle;

	return new IEntity(::CreateMesh(ParentHandle));
}

IEntity* IEngine::CreatePivot(IEntity* parent)
{
	uint ParentHandle = 0;
	if(parent != 0)
		ParentHandle = parent->Handle;

	return new IEntity(::CreatePivot(ParentHandle));
}

ILight* IEngine::CreateLight(ELightType type)
{
	uint Handle = (uint)-1;

	switch(type)
	{
	case ELT_Point:
		{
			Handle = CreatePointLight();
			break;
		}
	case ELT_Directional:
		{
			Handle = CreateDirectionalLight();
			break;
		}
	}

	if(Handle != (uint)-1)
		return new ILight(Handle, type);
	else
		return 0;
}

ICamera* IEngine::CreateCamera(IEntity* Parent)
{
	if(SingleCamera == 0)
		if(Parent != 0)
			SingleCamera = new ICamera(bbdx2_CreateCamera(Parent->Handle));
		else
			SingleCamera = new ICamera(bbdx2_CreateCamera(0));
	return SingleCamera;
}
ICamera* IEngine::SingleCamera = 0;