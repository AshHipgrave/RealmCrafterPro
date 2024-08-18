//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
//* BBDX - Entity Movement
//*

#include "..\PhysX\PhysXBBDX.h"

// Update all the childrens transforms
void UpdateChildren(ISceneNode* Node, bool Dummy)
{
	//if(strcmp(Node->getName(), "Head") == 0 && Node->getType() == ESNT_DUMMY_TRANSFORMATION)
	//	OutputDebugString("Head!\n");

	const irr::core::list<ISceneNode*>& list = Node->getChildren();
	irr::core::list<ISceneNode*>::Iterator it = list.begin();
	for (; it!=list.end(); ++it)
	{
		Node = ( *it);
		if(Node)
		{
			//if(Dummy)
			//	 Dummy = Node->getType() == ::ESNT_DUMMY_TRANSFORMATION;

			//if(!Dummy)
			//{
			//	if(strcmp(Node->getName(), "Head") == 0)
			//		OutputDebugString("Still Updating!!! one one\n");
			//	else
			//	{
			//		Node->updateAbsolutePosition();
			//		UpdateChildren(Node);
			//	}
			//}

			//if(Node->getType() != ESNT_DUMMY_TRANSFORMATION)
			{
				Node->updateAbsolutePosition();
				if(Node->NxHandleType != 1)
					bbdx2_UpdateStaticPose(Node);
				UpdateChildren(Node);
			}
		}
	}

}

// Position a node
DLLPRE void DLLEX chaosdigs(ISceneNode* Node, float x, float y, float z, int Global)
{
	SEHSTART;

	// If there is no parent, then a global check will cause trouble
	if(Node->getParent() == 0)
		Global = 0;

	vector3df Displace = vector3df(x, y, z) - Node->getAbsolutePosition();

	if(Node->NxHandleType == 1)
	{
		bbdx2_PhysXMoveEntity(Node, Displace.X, Displace.Y, Displace.Z);
	}else
	{

		// Set relative or global positions
		if(Global == 0)
			Node->setPosition(vector3df(x,y,z));
		else
			Node->setPosition(irr::core::vector3df(x, y, z) - Node->getParent()->getAbsolutePosition());

		// Update twice
		Node->updateAbsolutePosition();

		// Update collisions
		//bbdx2_PhysXMoveEntity(Node, Displace.X, Displace.Y, Displace.Z);
		bbdx2_UpdateStaticPose(Node);

		// Update position and children
		Node->updateAbsolutePosition();
		UpdateChildren(Node);
	}

	SEHEND
}

// Move a node
DLLPRE void DLLEX gobstoper(ISceneNode* Node, float x, float y, float z)
{
	SEHSTART;

	matrix4 Matrix;
	vector3df Position = vector3df(x,y,z);
	Matrix.setTranslation(Node->getPosition());
	Matrix.setRotationDegrees(Node->getRotation());
	
	Matrix.transformVect(Position);

	vector3df NewPosition = vector3df(Position.X, Position.Y, Position.Z);
	vector3df OldPosition = Node->getPosition();

	if(Node->NxHandleType == 1)
	{
		vector3df Displace = NewPosition - OldPosition;//Matrix.getTranslation();
		bbdx2_PhysXMoveEntity(Node, Displace.X, Displace.Y, Displace.Z);
	}else
	{
		Node->setPosition(NewPosition);

		// Update twice
		Node->updateAbsolutePosition();

		// Update collisions
		vector3df Displace = NewPosition - OldPosition;//Matrix.getTranslation();
		//bbdx2_PhysXMoveEntity(Node, Displace.X, Displace.Y, Displace.Z);
		bbdx2_UpdateStaticPose(Node);


		Node->updateAbsolutePosition();
		UpdateChildren(Node);
	}

	SEHEND
}

// Translate a node
DLLPRE void DLLEX rzrtool(ISceneNode* Node, float x, float y, float z, int Global)
{
	SEHSTART;

    vector3df Position = Node->getPosition();

	if(Global != 0)
	{
		matrix4 Transform = Node->getParent()->getAbsoluteTransformation();
		Transform.makeInverse();
		x += Node->getParent()->getAbsolutePosition().X;
		y += Node->getParent()->getAbsolutePosition().Y;
		z += Node->getParent()->getAbsolutePosition().Z;
		x = (Transform.M[0] * x) + (Transform.M[4] * y) + (Transform.M[8]  * z) + Transform.M[12];
		y = (Transform.M[1] * x) + (Transform.M[5] * y) + (Transform.M[9]  * z) + Transform.M[13];
		z = (Transform.M[2] * x) + (Transform.M[6] * y) + (Transform.M[10] * z) + Transform.M[14];
	}
    Position.X += x;
    Position.Y += y;
    Position.Z += z;

	if(Node->NxHandleType == 1)
	{
		vector3df Displace = vector3df(x, y, z);
		bbdx2_PhysXMoveEntity(Node, Displace.X, Displace.Y, Displace.Z);
	}else
	{
		Node->setPosition(Position);

		// Update twice
		Node->updateAbsolutePosition();

		// Update collisions
		vector3df Displace = vector3df(x, y, z);
		//bbdx2_PhysXMoveEntity(Node, Displace.X, Displace.Y, Displace.Z);
		bbdx2_UpdateStaticPose(Node);
		
		Node->updateAbsolutePosition();
		UpdateChildren(Node, true);
	}

	SEHEND;
}

// Get matrix element
DLLPRE float DLLEX GetMatElement(ISceneNode* Node, int Row, int Collumn)
{
	// Sanity check
	if(Row < 0 || Collumn < 0 || Row > 3 || Collumn > 3)
		return 0.0f;

	return Node->getAbsoluteTransformation().M[Collumn + (Row * 4)];
}

// rotate a node
DLLPRE void DLLEX mingja(ISceneNode* Node, float x, float y, float z, int Global)
{
	SEHSTART;

	while(x > 90) x -= 180;  while(x < -90) x += 180;
	while(y > 180) y -= 360;  while(y < -180) y += 360;
	while(z > 180) z -= 360;  while(z < -180) z += 360;

	if(Global == 0)
		Node->setRotation(vector3df(x, -y, z));
	else
		Node->setRotation(vector3df(x, -y, z) - getAbsoluteRotation(smgr, Node->getParent()));

	if(Node->NxHandleType != 1)
	{
		// Update twice
		Node->updateAbsolutePosition();

		// Update collisions
		bbdx2_UpdateStaticPose(Node);
		
		Node->updateAbsolutePosition();
		UpdateChildren(Node);
	}

	SEHEND
}

// turn a node
DLLPRE void DLLEX moolad(ISceneNode* Node, float x, float y, float z, int Global)
{
	SEHSTART;

    vector3df Rotation = Node->getRotation();
    Rotation.X += x;
    Rotation.Y += -y;
    Rotation.Z += z;

	while(Rotation.X > 90) Rotation.X -= 180;  while(Rotation.X < -90) Rotation.X += 180;
	while(Rotation.Y > 180) Rotation.Y -= 360;  while(Rotation.Y < -180) Rotation.Y += 360;
	while(Rotation.Z > 180) Rotation.Z -= 360;  while(Rotation.Z < -180) Rotation.Z += 360;

    Node->setRotation(Rotation);

	if(Node->NxHandleType != 1)
	{
		// Update twice
		Node->updateAbsolutePosition();

		// Update collisions
		bbdx2_UpdateStaticPose(Node);


		Node->updateAbsolutePosition();
		UpdateChildren(Node);
	}

	SEHEND
}

// Scale a node
DLLPRE void DLLEX jaffamak(ISceneNode* Node, float x, float y, float z, int Global)
{
	SEHSTART;

	if(Global == 0)
		Node->setScale(vector3df(x,y,z));
	else
		Node->setScale(vector3df(x,y,z) / getAbsoluteScale(smgr, Node->getParent()));

	// Update collisions
	if(Node->NxHandle != 0 && Node->NxHandleType == 3)
		bbdx2_SetCollisionMesh(Node);
	
	if(Node->NxHandleType != 1)
	{
		Node->updateAbsolutePosition();
		UpdateChildren(Node);
	}

	SEHEND
}

// Align to a vector
DLLPRE void DLLEX chinook(ISceneNode* Node, irr::f32 x, irr::f32 y, irr::f32 z, int Axis)
{
	vector3df Vector = vector3df(x,y,z);
	vector3df Difference = Vector.getHorizontalAngle();
	vector3df Rotation = vector3df(Difference.X, Difference.Y, 0);

	Node->setRotation(Rotation);

	switch (Axis)
	{
		// X axis
		case 1:
			moolad(Node, 0, 90, 0, false);
			break;
		// Y axis
		case 2:
			moolad(Node, 90, 0, 0, false);
			break;
	}

	if(Node->NxHandleType != 1)
	{
		// Update twice
		Node->updateAbsolutePosition();

		// Update collisions
		bbdx2_UpdateStaticPose(Node);

		Node->updateAbsolutePosition();
	}

}

// Point entity
DLLPRE void DLLEX babygoo(ISceneNode* Node, ISceneNode* Target, irr::f32 Roll)
{
	chinook(Node,
		(Target->getPosition().X - Node->getPosition().X),
		(Target->getPosition().Y - Node->getPosition().Y),
		(Target->getPosition().Z - Node->getPosition().Z),
		3);
}

