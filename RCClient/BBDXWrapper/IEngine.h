#pragma once

#include <bbdx.h>
#include <Vector2.h>
#include <Vector3.h>
#include <Color.h>

class ICamera;
class ILight;
class IEntity;
enum ELightType;

class IEngine
{
public:

	
	

protected:

	static ICamera* SingleCamera;

public:

	__declspec(property(get=zzGet_Direct3DDevice)) IDirect3DDevice9* Direct3DDevice;
	static IDirect3DDevice9* zzGet_Direct3DDevice();

	static void RenderWorld();
	static void UpdateWorld();

	// Entities
	static ICamera* CreateCamera(IEntity* parent = 0);
	static ILight* CreateLight(ELightType type);
	static IEntity* LoadMesh(std::string name, IEntity* parent = 0, bool animated = false);
	static IEntity* CreateMesh(IEntity* parent);
	static IEntity* CreatePivot(IEntity* parent);

	// Collisions
	static void Collisions(int srcType, int destType, int method, int response);

	// Lights
	static void AmbientLight(NGin::Math::Color& color);

	// Rendering
	static uint LoadShader(std::string name);
};

