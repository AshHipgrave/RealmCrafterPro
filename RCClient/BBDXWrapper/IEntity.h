#pragma once

#include <bbdx.h>
#include <Vector2.h>
#include <Vector3.h>
#include <Color.h>

struct CollisionHitInstance
{
	NGin::Math::Vector3 Position;
	NGin::Math::Vector3 Normal;
};

class IEntity
{
protected:

	uint _Handle;
public:

	//int EntityType;
	//void* NxHandle;
	//int NxHandleType;
	//bool Pickable;
	//ArrayList<CollisionHitInstance> CollisionHits;

	IEntity(uint handle);

	// Core
	virtual void Free(bool Delete = false);

	__declspec(property(get=zzGet_Handle)) uint Handle;
	virtual uint zzGet_Handle();

	__declspec(property(get=zzGet_Visible, put=zzSet_Visible)) bool Visible;
	virtual void zzSet_Visible(bool visible);
	virtual bool zzGet_Visible();

	__declspec(property(get=zzGet_GetTag, put=zzSet_SetTag)) std::string Tag;
	virtual void zzSet_Tag(std::string tag);
	virtual std::string zzGet_Tag();

	__declspec(property(put=zzSet_Order)) int Order;
	virtual void zzSet_Order(int order);

	__declspec(property(put=zzSet_FX)) int FX;
	virtual void zzSet_FX(int fx);

	// Mesh
	virtual void UpdateHardwareBuffers();

	// Material
	__declspec(property(put=zzSet_Shader)) uint Shader;
	virtual void zzSet_Shader(uint shader);
	
	// Collision
	__declspec(property(put=zzSet_CollisionType, get=zzGet_CollisionType)) int CollisionType;
	virtual void zzSet_CollisionType(int type);
	virtual int zz_GetCollisionType();

	__declspec(property(put=zzSet_Pickable)) bool Pickable;
	virtual void zzSet_Pickable(bool Pickable);

	__declspec(property(get=zzGet_CollisionPosition)) NGin::Math::Vector3 CollisionPosition[];
	virtual NGin::Math::Vector3 zzGet_CollisionPosition(int index);

	__declspec(property(get=zzGet_CollisionNormal)) NGin::Math::Vector3 CollisionNormal[];
	virtual NGin::Math::Vector3 zzGet_CollisionNormal(int index);

	virtual void CollisionRadius(float width, float height = -1.0f);
	virtual void CommitCollisionMesh();
	virtual void ResetCollisions();

	virtual int CollisionCount();


	// Transform
	virtual void Move(NGin::Math::Vector3& distance);
	virtual void Turn(NGin::Math::Vector3& distance);
	virtual void Translate(NGin::Math::Vector3& distance);
	virtual float DistanceFrom(IEntity* other);
	virtual void Point(IEntity* target);

	__declspec(property(get=zzGet_Position, put=zzSet_Position)) NGin::Math::Vector3 Position;
	virtual void zzSet_Position(NGin::Math::Vector3& position);
	virtual NGin::Math::Vector3 zzGet_Position();

	__declspec(property(get=zzGet_Scale, put=zzSet_Scale)) NGin::Math::Vector3 Scale;
	virtual void zzSet_Scale(NGin::Math::Vector3& scale);
	virtual NGin::Math::Vector3 zzGet_Scale();

	__declspec(property(get=zzGet_Rotation, put=zzSet_Rotation)) NGin::Math::Vector3 Rotation;
	virtual void zzSet_Rotation(NGin::Math::Vector3& rotation);
	virtual NGin::Math::Vector3 zzGet_Rotation();

	__declspec(property(get=zzGet_MeshSize)) NGin::Math::Vector3 MeshSize;
	virtual NGin::Math::Vector3 zzGet_MeshSize();
	
	// Animation
	virtual uint ExtractSequence(int start, int end);
	virtual void Animate(int mode, float speed = 1.0f, uint sequence = 0);
};

