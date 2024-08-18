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
//* BBDX - Entity State
//*

// Globals
float TFormedX;
float TFormedY;
float TFormedZ;

// Get Absolute rotation of a node
//irr::core::vector3df getAbsoluteRotation(irr::scene::ISceneManager* SceneManager, irr::scene::ISceneNode* Node)
//{
//	irr::core::vector3df Rotation;
//	
//	while(1)
//	{
//		if(Node && Node != SceneManager->getRootSceneNode())
//			Rotation += Node->getRotation();
//		else
//			break;	
//			
//		Node = Node->getParent();
//	}
//	
//	return Rotation;
//}

DLLPRE void bbdx2_SetEntityRenderMask(irr::scene::ISceneNode* node, int mask)
{
	node->RenderMask = mask;
}

DLLPRE int bbdx2_GetEntityRenderMask(irr::scene::ISceneNode* node)
{
	return node->RenderMask;
}

DLLPRE void bbdx2_SetRenderMask(int mask)
{
	smgr->RenderMask = mask;
}

DLLPRE int bbdx2_GetRenderMask()
{
	return smgr->RenderMask;
}

irr::core::vector3df getAbsoluteRotation(irr::scene::ISceneManager* SceneManager, irr::scene::ISceneNode* Node)
{
	//irr::core::vector3df Rotation;
	irr::core::matrix4 Rotation;
	
	while(1)
	{
		if(Node && Node != SceneManager->getRootSceneNode())
		{
			irr::core::matrix4 TempRot;
			TempRot.setRotationDegrees(Node->getRotation());
			Rotation *= TempRot;
		}
		else
			break;	
			
		Node = Node->getParent();
	}
	
	return Rotation.getRotationDegrees();
}

// Get Absolute scale of a node
irr::core::vector3df getAbsoluteScale(irr::scene::ISceneManager* SceneManager, irr::scene::ISceneNode* Node)
{
	irr::core::vector3df Scale(1,1,1);
	
	while(1)
	{
		if(Node && Node != SceneManager->getRootSceneNode())
			Scale *= Node->getScale();
		else
			break;	
			
		Node = Node->getParent();
	}
	
	return Scale;
}


DLLPRE void DLLEX helplost(float x, float y, float z, ISceneNode* src, ISceneNode* dest)
{
	matrix4 mat;

	TFormedX = x;
	TFormedY = y;
	TFormedZ = z;


	// Convert point to global from source entity, if any
	if (src != 0)
	{
		mat = src->getAbsoluteTransformation();
	        TFormedX = (mat.M[0]  * x) + (mat.M[4] * y) + (mat.M[8] * z) + mat.M[12];
	        TFormedY = (mat.M[1] * x) + (mat.M[5] * y) + (mat.M[9] * z) + mat.M[13];
        	TFormedZ = (mat.M[2] * x) + (mat.M[6] * y) + (mat.M[10] * z) + mat.M[14];
			TFormedX -= src->getAbsolutePosition().X;
			TFormedY -= src->getAbsolutePosition().Y;
			TFormedZ -= src->getAbsolutePosition().Z;
	}

	// Convert point to local for destination entity, if any
	if (dest != 0)
	{
            mat = dest->getAbsoluteTransformation();
            mat.makeInverse();
			TFormedX += dest->getAbsolutePosition().X;
			TFormedY += dest->getAbsolutePosition().Y;
			TFormedZ += dest->getAbsolutePosition().Z;
            TFormedX = (mat.M[0] * TFormedX) + (mat.M[4] * TFormedY) + (mat.M[8] * TFormedZ) + mat.M[12];
            TFormedY = (mat.M[1] * TFormedX) + (mat.M[5] * TFormedY) + (mat.M[9] * TFormedZ) + mat.M[13];
            TFormedZ = (mat.M[2] * TFormedX) + (mat.M[6] * TFormedY) + (mat.M[10] * TFormedZ) + mat.M[14];
	}
}

DLLPRE void DLLEX kieransan(float x, float y, float z, ISceneNode* src, ISceneNode* dest)
{
	matrix4 mat;

	TFormedX = x;
	TFormedY = y;
	TFormedZ = z;

	// Convert point to global from source entity, if any
	if (src != 0)
	{
		mat = src->getAbsoluteTransformation();
	        TFormedX = (mat.M[0]  * x) + (mat.M[4] * y) + (mat.M[8] * z) + mat.M[12];
	        TFormedY = (mat.M[1] * x) + (mat.M[5] * y) + (mat.M[9] * z) + mat.M[13];
        	TFormedZ = (mat.M[2] * x) + (mat.M[6] * y) + (mat.M[10] * z) + mat.M[14];
	}

	// Convert point to local for destination entity, if any
	if (dest != 0)
	{
            mat = dest->getAbsoluteTransformation();
            mat.makeInverse();
            TFormedX = (mat.M[0] * TFormedX) + (mat.M[4] * TFormedY) + (mat.M[8] * TFormedZ) + mat.M[12];
            TFormedY = (mat.M[1] * TFormedX) + (mat.M[5] * TFormedY) + (mat.M[9] * TFormedZ) + mat.M[13];
            TFormedZ = (mat.M[2] * TFormedX) + (mat.M[6] * TFormedY) + (mat.M[10] * TFormedZ) + mat.M[14];
	}
}

// Return Transformed point of vector
DLLPRE float DLLEX itsnoteasy()
{
	return TFormedX;
}

DLLPRE float DLLEX buymebeer()
{
	return TFormedY;
}

DLLPRE float DLLEX firstdance()
{
	return TFormedZ;
}

// Get entity distance
DLLPRE float DLLEX nottdos(ISceneNode* Source, ISceneNode* Destination)
{
	irr::core::line3d<irr::f32> DistanceLine = line3d<irr::f32>(Source->getAbsolutePosition(), Destination->getAbsolutePosition());
	irr::f64 Distance = DistanceLine.getLength();
	return (float)Distance;
}

// Get X
DLLPRE float DLLEX jingsu(ISceneNode* Node, int Global)
{
	if(Global == 1)
		return Node->getAbsolutePosition().X;
	else
		return Node->getPosition().X;
}

// Get Y
DLLPRE float DLLEX sostrong(ISceneNode* Node, int Global)
{
	if(Global == 1)
		return Node->getAbsolutePosition().Y;
	else
		return Node->getPosition().Y;
}

// Get Z
DLLPRE float DLLEX kisheadgone(ISceneNode* Node, int Global)
{
	if(Global == 1)
		return Node->getAbsolutePosition().Z;
	else
		return Node->getPosition().Z;
}

// Get Scale X
DLLPRE float DLLEX tcpwnsall(ISceneNode* Node, int Global)
{
	if(Global == 0)
		return Node->getScale().X;
	else
		return getAbsoluteScale(smgr, Node).X;
}

// Get Scale Y
DLLPRE float DLLEX habaki(ISceneNode* Node, int Global)
{
	if(Global == 0)
		return Node->getScale().Y;
	else
		return getAbsoluteScale(smgr, Node).Y;
}

// Get Scale Z
DLLPRE float DLLEX kimono(ISceneNode* Node, int Global)
{
	if(Global == 0)
		return Node->getScale().Z;
	else
		return getAbsoluteScale(smgr, Node).Z;
}


// Get Pitch Rotation
DLLPRE float DLLEX kissme(ISceneNode* Node, int Global)
{
    if(Global == 0)
		return Node->getRotation().X;
	else
		return getAbsoluteRotation(smgr, Node).X;
}

// Get Yaw Rotation
DLLPRE float DLLEX lolatme(ISceneNode* Node, int Global)
{
	if(Global == 0)
		return -Node->getRotation().Y;
	else
		return -getAbsoluteRotation(smgr, Node).Y;
}

// Get Roll Rotation
DLLPRE float DLLEX lambdin(ISceneNode* Node, int Global)
{
    if(Global == 0)
		return Node->getRotation().Z;
	else
		return getAbsoluteRotation(smgr, Node).Z;
}

// Get node name
DLLPRE const char* DLLEX likeyoumean(ISceneNode* Node)
{
    return Node->getName();
}

// Get node class
DLLPRE const char* DLLEX setthefire(ISceneNode* Node)
{
	switch(Node->getType())
	{
	case irr::scene::ESNT_ANIMATED_MESH:
		return "Mesh";
	case irr::scene::ESNT_LIGHT:
		return "Light";
	case irr::scene::ESNT_CAMERA_FPS:
		return "Camera";
	case irr::scene::ESNT_TERRAIN:
		return "Terrain";
	}

	if(Node->IsJoint)
		return "Joint";
	return "Unknown";
}

// Get Angle to target
DLLPRE float DLLEX fightening(ISceneNode* Source, ISceneNode* Destination)
{
	float DistanceY = Destination->getPosition().Y - Source->getPosition().Y;
	float DistanceX = Destination->getPosition().X - Source->getPosition().X;
	return atan2(DistanceY, DistanceX);
}

// Get a Child handle by name
DLLPRE ISceneNode* DLLEX rendering(ISceneNode* Node, const irr::c8* cName)
{
	ISceneNode* ReturnNode = 0;
	irr::s32 ChildCount = 0;

	// Create an iterator to search through children
	const irr::core::list<ISceneNode*>& Children = Node->getChildren();
	irr::core::list<ISceneNode*>::Iterator it = Children.begin();
	irr::core::stringc Name = cName;

	// Search through children
	for (; it != Children.end(); ++it)
		if(*it)
		{
			irr::core::stringc ChildName = (*it)->getName();
			if(Name.equals_ignore_case(ChildName))
				ReturnNode = (*it);
		}

	// Return node
	return ReturnNode;
}

// Get a Child handle by ID
DLLPRE ISceneNode* DLLEX penondesk(ISceneNode* Node, irr::s32 ID)
{
	// Blitz Indices start from 1
	--ID;

	ISceneNode* ReturnNode = 0;
	irr::s32 ChildCount = 0;

	// Create an interator to search through children
	const irr::core::list<ISceneNode*>& Children = Node->getChildren();
	irr::core::list<ISceneNode*>::Iterator it = Children.begin();
	
	// Search through children
	for (; it != Children.end(); ++it)
	{
		if(*it)
		{
			if(ID == ChildCount)
				ReturnNode = (*it);
			++ChildCount;
		}
	}

	// Return node
	return ReturnNode;
}


// Get Child
DLLPRE ISceneNode* DLLEX penondeskOLD(ISceneNode* node, int index)
{
	// Blitz Compat
	--index;

	ISceneNode* ret = 0;
	const irr::core::list<ISceneNode*>& list = node->getChildren();
	irr::core::list<ISceneNode*>::Iterator it = list.begin();
	int cnt = 1;
	for (; it!=list.end(); ++it)
	{
		node = ( *it);
		if (cnt == index)
			ret = node;

		++cnt;
	}

	// Compiler Satisfaction
	return ret;
}

// Get child count
DLLPRE irr::s32 DLLEX mindless(ISceneNode* Node)
{
	irr::s32 Children = 0;
	irr::s32 Joints = 0;

	Children = (irr::s32)Node->getChildren().getSize();

	return Children + Joints;
}

DLLPRE int DLLEX GetAnimatedBoundingBox(ISceneNode* node, float* minX, float* minY, float* minZ, float* maxX, float* maxY, float* maxZ)
{
	if(node->getType() == irr::scene::ESNT_ANIMATED_MESH)
	{
		IAnimatedMesh* M = ((irr::scene::IAnimatedMeshSceneNode*)node)->getLocalMesh();
		if(M->getMeshType() == irr::scene::EAMT_B3D)
		{
			bool HasBox = false;
			irr::core::aabbox3df Box = ((irr::scene::IAnimatedMeshB3d*)M)->GetAnimatedBoundingBox(HasBox);

			if(!HasBox)
				return 0;

			*minX = Box.MinEdge.X;
			*minY = Box.MinEdge.Y;
			*minZ = Box.MinEdge.Z;
			*maxX = Box.MaxEdge.X;
			*maxY = Box.MaxEdge.Y;
			*maxZ = Box.MaxEdge.Z;

			return 1;
		}
	}

	return 0;
}

