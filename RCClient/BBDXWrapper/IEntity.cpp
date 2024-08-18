#include "IEntity.h"

IEntity::IEntity(uint handle)
{
	_Handle = handle;

	//EntityType = 1;
	//NxHandle = 0;
	//NxHandleType = 0;
	//Pickable = false;
}

#pragma region Core
void IEntity::Free(bool Delete)
{
	FreeEntity(_Handle);
	_Handle = 0;

	if(Delete)
		delete this;
}

uint IEntity::zzGet_Handle()
{
	return _Handle;
}

void IEntity::zzSet_Visible(bool visible)
{
	if(visible)
		ShowEntity(_Handle);
	else
		HideEntity(_Handle);
}

bool IEntity::zzGet_Visible()
{
	#pragma message("zz_GetVisible() is not implemented!")
	return false;
}

void IEntity::zzSet_Tag(std::string tag)
{
	TagEntity(_Handle, tag);
}

std::string IEntity::zzGet_Tag()
{
	return EntityTag(_Handle);
}

void IEntity::zzSet_Order(int order)
{
	EntityOrder(_Handle, order);
}

void IEntity::zzSet_FX(int fx)
{
	EntityFX(_Handle, fx);
}

#pragma endregion
#pragma region Mesh

void IEntity::UpdateHardwareBuffers()
{
	::UpdateHardwareBuffers(_Handle);
}

#pragma endregion
#pragma region Material

void IEntity::zzSet_Shader(uint shader)
{
	EntityShader(_Handle, shader);
}

#pragma endregion
#pragma region Collisions

void IEntity::zzSet_CollisionType(int type)
{
	EntityType(_Handle, type);
}

int IEntity::zz_GetCollisionType()
{
	return GetEntityType(_Handle);
}

void IEntity::zzSet_Pickable(bool pickable)
{
	EntityPickMode(_Handle, pickable ? 1 : 0);
}

NGin::Math::Vector3 IEntity::zzGet_CollisionPosition(int index)
{
	return NGin::Math::Vector3(CollisionX(_Handle, index), CollisionY(_Handle, index), CollisionZ(_Handle, index));
}

NGin::Math::Vector3 IEntity::zzGet_CollisionNormal(int index)
{
	return NGin::Math::Vector3(CollisionNX(_Handle, index), CollisionNY(_Handle, index), CollisionNZ(_Handle, index));
}

void IEntity::CommitCollisionMesh()
{
	SetCollisionMesh(_Handle);
}

void IEntity::ResetCollisions()
{
	ResetEntity(_Handle);
}

int IEntity::CollisionCount()
{
	return CountCollisions(_Handle);
}


void IEntity::CollisionRadius(float width, float height)
{
	EntityRadius(_Handle, width, height);
}

#pragma endregion
#pragma region Transform

//extern "C" __declspec(dllimport) void bbdx2_PhysXMoveEntity(uint node, float x, float y, float z);
//extern "C" __declspec(dllimport) void bbdx2_UpdateStaticPose(uint node);
//extern "C" __declspec(dllimport) void bbdx2_SetCollisionMesh(uint node);

void IEntity::Move(NGin::Math::Vector3& distance)
{
	MoveEntity(_Handle, distance.X, distance.Y, distance.Z);
	//bbdx2_PhysXMoveEntity(this->Handle, distance.X, distance.Y, distance.Z);
}

void IEntity::Turn(NGin::Math::Vector3& distance)
{
	TurnEntity(_Handle, distance.X, distance.Y, distance.Z);
	//bbdx2_UpdateStaticPose(this->Handle);
}

void IEntity::Translate(NGin::Math::Vector3& distance)
{
	TranslateEntity(_Handle, distance.X, distance.Y, distance.Z);
	//bbdx2_UpdateStaticPose(this->Handle);
}

float IEntity::DistanceFrom(IEntity* other)
{
	return EntityDistance(_Handle, other->Handle);
}

void IEntity::Point(IEntity* target)
{
	PointEntity(_Handle, target->Handle);
}

void IEntity::zzSet_Position(NGin::Math::Vector3& position)
{
	//NGin::Math::Vector3 Displace = position - this->Position;
	//bbdx2_PhysXMoveEntity(this->Handle, Displace.X, Displace.Y, Displace.Z);
	PositionEntity(_Handle, position.X, position.Y, position.Z);
	//bbdx2_UpdateStaticPose(this->Handle);
}

void IEntity::zzSet_Scale(NGin::Math::Vector3& scale)
{
	ScaleEntity(_Handle, scale.X, scale.Y, scale.Z);
	//if(NxHandle != 0 && NxHandleType == 3)
	//	bbdx2_SetCollisionMesh(this->Handle);
	//tbbdx2_UpdateStaticPose(this);
}

void IEntity::zzSet_Rotation(NGin::Math::Vector3& rotation)
{
	RotateEntity(_Handle, rotation.X, rotation.Y, rotation.Z);
	//bbdx2_UpdateStaticPose(this->Handle);
}

NGin::Math::Vector3 IEntity::zzGet_Position()
{
	return NGin::Math::Vector3(EntityX(_Handle), EntityY(_Handle), EntityZ(_Handle));
}

NGin::Math::Vector3 IEntity::zzGet_Scale()
{
	return NGin::Math::Vector3(EntityScaleX(_Handle), EntityScaleY(_Handle), EntityScaleZ(_Handle));
}

NGin::Math::Vector3 IEntity::zzGet_Rotation()
{
	return NGin::Math::Vector3(EntityPitch(_Handle), EntityYaw(_Handle), EntityRoll(_Handle));
}

NGin::Math::Vector3 IEntity::zzGet_MeshSize()
{
	return NGin::Math::Vector3(MeshWidth(_Handle), MeshHeight(_Handle), MeshDepth(_Handle));
}

#pragma endregion
#pragma region Animations

// Animations
uint IEntity::ExtractSequence(int start, int end)
{
	return ExtractAnimSeq(_Handle, start, end);
}

void IEntity::Animate(int mode, float speed, uint sequence)
{
	::Animate(_Handle, mode, speed, sequence, 0.0f);
}

#pragma endregion

