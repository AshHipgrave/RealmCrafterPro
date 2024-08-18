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
//* BBDX - Mesh

#include <sstream>
#include <algorithm>
#include <locale>
#include "GroupedObject.h"
#include "..\BBDX\ASyncJobModule.h"

// Get bounding box details
DLLPRE irr::f32 DLLEX shelveall(IAnimatedMeshSceneNode* Node, int Index, int xyz)
{
	// Obtain the 8 outer vertices of the box
	aabbox3d<irr::f32> Box = Node->getBoundingBox();
	vector3d<irr::f32> Edges[8];
	Box.getEdges(&Edges[0]);
	
	switch(xyz)
	{
	case 0:
		return Edges[Index].X;
		break;
	case 1:
		return Edges[Index].Y;
		break;
	case 2:
		return Edges[Index].Z;
		break;
	}

	return 0;
}

// Generate a tangent mesh from a regular mesh
DLLPRE int DLLEX CalculateB3DTangents(IAnimatedMeshSceneNode* Node)
{
	SEHSTART;

	// If its not B3D, then don't do anything
	if(Node->getLocalMesh()->getMeshType() != irr::scene::EAMT_B3D)
		return 1;

	// Generate
	IAnimatedMeshB3d* B3dMesh = ((IAnimatedMeshB3d*)(Node->getLocalMesh()));

	if(B3dMesh->IsTangentMesh)
		return 1;


	bool CopyFail = B3dMesh->CopyBuffersToTangents();
	if(!CopyFail)
		return 0;

	smgr->getMeshManipulator()->createMeshWithTangents(B3dMesh->getMesh(0));
	B3dMesh->IsTangentMesh = true;
	
	// Update DirectX
	jockgnome(Node);

	return 1;

	SEHEND;
}

// Update hardware buffers
DLLPRE void DLLEX jockgnome(IAnimatedMeshSceneNode* node)
{
	SEHSTART;

	// Get Mesh from animated mesh
	IAnimatedMesh* msh = node->getLocalMesh();
	IMesh* tmpMesh = msh->getMesh(0);
	
	// Locals
	vector3df VPos;
	VPos.set(0,0,0);
	::S3DVertex2TCoords* vb2;
	::S3DVertexTangents* vbt;
	
	// Check is a B3D
	if(msh->getMeshType() == ::EAMT_B3D)
	{
		// Loop through every buffer
		for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)
		{
			// If its animated, update it animatedly, or just normally
			if(((IAnimatedMeshB3d*)msh)->IsAnimated())
			{
				driver->updateAnimatedHardwareBuffer(tmpMesh->getMeshBuffer(f), &(((irr::scene::IAnimatedMeshB3d*)msh)->VBlends));
			}else
				driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
		}

		// Remake bounding box
		((::IAnimatedMeshB3d*)msh)->recalculateBoundingBox();
		node->Box = ((::IAnimatedMeshB3d*)msh)->BoundingBox;
		
		return;
	}

	/*
	* This section simple does a huge reclaculation of a bounding box,
	* it was used with other mesh formats, which we don't support so
	* just ignore it.
	*/
	((SMesh*)tmpMesh)->BoundingBox.reset(0,0,0);
	bool FBuf = true;

	for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)
	{
		
		driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
		((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.reset(0,0,0);
		bool FVert = true;

		// Update bounding Box
		for(int j = 0; j < tmpMesh->getMeshBuffer(f)->getVertexCount(); ++j)
		{
			switch(tmpMesh->getMeshBuffer(f)->getVertexType())
			{
			case irr::video::EVT_STANDARD:
				VPos = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->Vertices[j].Pos;
				break;
			case irr::video::EVT_2TCOORDS:
				//VPos = ((::S3DVertex2TCoords)((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->Vertices[j]).Pos;
				vb2 = (::S3DVertex2TCoords*)tmpMesh->getMeshBuffer(f)->getVertices();
				VPos = vb2[j].Pos;
				break;
			case irr::video::EVT_TANGENTS:
				//VPos = ((::S3DVertexTangents)((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->Vertices[j]).Pos;
				vbt = (::S3DVertexTangents*)tmpMesh->getMeshBuffer(f)->getVertices();
				VPos = vbt[j].Pos;
				break;
			}

			if(FVert)
			{
				((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge = VPos;
				((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge = VPos;
				FVert = false;
			}


			if(VPos.X < ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.X) { ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.X = VPos.X;}
			if(VPos.Y < ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Y) { ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Y = VPos.Y;}
			if(VPos.Z < ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Z) { ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Z = VPos.Z;}
			
			if(VPos.X > ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.X) { ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.X = VPos.X;}
			if(VPos.Y > ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Y) { ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Y = VPos.Y;}
			if(VPos.Z > ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Z) { ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Z = VPos.Z;}
		}


		if(FBuf)
		{
			((SMesh*)tmpMesh)->BoundingBox.MinEdge = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge;
			((SMesh*)tmpMesh)->BoundingBox.MaxEdge = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge;
			FBuf = false;
		}

		if( ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.X < ((SMesh*)tmpMesh)->BoundingBox.MinEdge.X ) { ((SMesh*)tmpMesh)->BoundingBox.MinEdge.X = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.X; }
		if( ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Y < ((SMesh*)tmpMesh)->BoundingBox.MinEdge.Y ) { ((SMesh*)tmpMesh)->BoundingBox.MinEdge.Y = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Y; }
		if( ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Z < ((SMesh*)tmpMesh)->BoundingBox.MinEdge.Z ) { ((SMesh*)tmpMesh)->BoundingBox.MinEdge.Z = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge.Z; }

		if( ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.X > ((SMesh*)tmpMesh)->BoundingBox.MaxEdge.X ) { ((SMesh*)tmpMesh)->BoundingBox.MaxEdge.X = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.X; }
		if( ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Y > ((SMesh*)tmpMesh)->BoundingBox.MaxEdge.Y ) { ((SMesh*)tmpMesh)->BoundingBox.MaxEdge.Y = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Y; }
		if( ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Z > ((SMesh*)tmpMesh)->BoundingBox.MaxEdge.Z ) { ((SMesh*)tmpMesh)->BoundingBox.MaxEdge.Z = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge.Z; }

		//((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->recalculateBoundingBox();
	}

	//for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)
	//{
	//	((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MinEdge *= node->getScale();
	//	((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->BoundingBox.MaxEdge *= node->getScale();
	//}

	node->Box = ((SMesh*)tmpMesh)->BoundingBox;
	//node->setAutomaticCulling(false);
	
	SEHEND;

}

// CreateMesh command
DLLPRE IAnimatedMeshSceneNode* DLLEX pathlier(IAnimatedMeshSceneNode* Parent)
{
	// Create a Meshbuffer for a new Mesh for a new Animated Mesh
	SMeshBuffer* Buffer = new SMeshBuffer();
	SMesh* Mesh = new SMesh();
	Mesh->setIsLoaded( true );
	SAnimatedMesh* AnimatedMesh = new SAnimatedMesh();
	Mesh->addMeshBuffer(Buffer);
	AnimatedMesh->addMesh(Mesh);

	// Make a new scenenode and seutp its default collision data
	IAnimatedMeshSceneNode* Node = smgr->addAnimatedMeshSceneNode(AnimatedMesh, Parent);
	Node->CollisionID = GetCollisionID();
	//Node->EntityType = 250;
	//CollisionTypePRO(Node->CollisionID, 250);
	//CollisionEntities.push_back(Node);

	// These are all defaults!
	Node->ForceAlpha = false;
	Node->setMaterialFlag(EMF_LIGHTING,true);
	Node->setAutomaticCulling(true);
	Node->TriangleCulling = false;
	Node->setDebugDataVisible(DEBUGMODE);

	// Test all of the textures
	for(irr::s32 i = 0; i < Node->getMaterialCount(); ++i)
	{
		if( !Node->getMaterial(i).Texture1 )
			Node->getMaterial(i).Texture1 = driver->getDefaultTexture();

		if( !Node->getMaterial(i).Texture2 )
			Node->getMaterial(i).Texture2 = driver->getDefaultTexture();

		if( !Node->getMaterial(i).Texture3 )
			Node->getMaterial(i).Texture3 = driver->getDefaultTexture();

		if( !Node->getMaterial(i).Texture4 )
			Node->getMaterial(i).Texture4 = driver->getDefaultTexture();
	}

	return Node;
}

// Rotate Mesh
DLLPRE void DLLEX setmedown(IAnimatedMeshSceneNode* Node, irr::f32 x, irr::f32 y, irr::f32 z)
{
	// Make a new matrix instance
	matrix4 mat;
	
	// Set Matrix rotation
	mat.setRotationDegrees(vector3df(x,y,z));

	// Get mesh
	IAnimatedMesh* Mesh = Node->getLocalMesh();
	
	// Iterate through surfaces
	for(int i = 0; i < Mesh->getMesh(0)->getMeshBufferCount(); i++)
	{
		// Get surface
		IMeshBuffer* Buffer = Mesh->getMesh(0)->getMeshBuffer(i);

		// Retrieve Vertex Buffer
		S3DVertex* Vertices = (S3DVertex*)Buffer->getVertices();
		
		// Iterate through verts and transform
		for(int f = 0; f < Buffer->getVertexCount(); f++)
		{
			mat.transformVect(Vertices[f].Pos);
		}
	}
	
	// Update hardware buffers
	jockgnome(Node);
}

// Flip surfaces
DLLPRE void DLLEX performance(IAnimatedMeshSceneNode* Node)
{
	smgr->getMeshManipulator()->flipSurfaces(Node->getLocalMesh()->getMesh(0));
	jockgnome(Node);
}

// PositionMesh
DLLPRE void DLLEX softskin(IAnimatedMeshSceneNode* node, irr::f32 x, irr::f32 y, irr::f32 z)
{
	// Make a vector of the position
	vector3df Pos = vector3df(x,y,z);

	// Get mesh
	SMesh* mesh = (SMesh*)node->getLocalMesh()->getMesh(node->getFrameNr());

	// Locals
	S3DVertex* vb;
	S3DVertex2TCoords* vb2;
	S3DVertexTangents* vbt;
	::SAnimatedVert* vba;

	// This min is bigger than max, to force rebuild of the aabb
	vector3df min = vector3df(10,10,10);
	vector3df max = vector3df(0,0,0);
	bool t = true;

	// Loop through every buffer
	for(int i = 0; i < mesh->getMeshBufferCount(); i++)
	{
		SMeshBuffer* buf = (SMeshBuffer*)mesh->getMeshBuffer(i);

		// What type of verts are they?
		switch(((SMeshBuffer*)mesh->getMeshBuffer(i))->getVertexType())
		{
		case irr::video::EVT_STANDARD:
			{
				vb = (S3DVertex*)buf->getVertices();
			
				for(int f = 0; f < buf->getVertexCount(); f++)
					vb[f].Pos += Pos;

				break;
			}
		case irr::video::EVT_2TCOORDS:
			{

				vb2 = (S3DVertex2TCoords*)buf->getVertices();
			
				for(int f = 0; f < buf->getVertexCount(); f++)
				{
					vb2[f].Pos += Pos;	
					min = vb2[f].Pos;
				}
			
				break;
			}
		case irr::video::EVT_TANGENTS:
			{
				vbt = (S3DVertexTangents*)buf->getVertices();
			
				for(int f = 0; f < buf->getVertexCount(); f++)
				{
					vbt[f].Pos += Pos;
				}
				break;
			}
		}

	}

	// Update buffers
	jockgnome(node);
}

// Scale Mesh
DLLPRE void DLLEX soreneck(IAnimatedMeshSceneNode* node, irr::f32 x, irr::f32 y, irr::f32 z)
{
	if( node->getLocalMesh() == NULL )
		return;

	// Get mesh and setup locals
	SMesh* mesh = (SMesh*)node->getLocalMesh()->getMesh(node->getFrameNr());
	vector3df VertexScale(x,y,z);
	S3DVertex* vb;
	S3DVertex2TCoords* vb2;
	S3DVertexTangents* vbt;
	SAnimatedVert* vba;

	// Loop through every buffer then vertex to position it
	for(int i = 0; i < mesh->getMeshBufferCount(); i++)
	{
		SMeshBuffer* buf = (SMeshBuffer*)mesh->getMeshBuffer(i);

		switch(((SMeshBuffer*)mesh->getMeshBuffer(i))->getVertexType())
		{
		case irr::video::EVT_STANDARD:
			vb = (S3DVertex*)buf->getVertices();
		
			for(int f = 0; f < buf->getVertexCount(); f++)
			{
				vb[f].Pos *= VertexScale;
			}
			break;
		case irr::video::EVT_2TCOORDS:


				vb2 = (S3DVertex2TCoords*)buf->getVertices();
			
				for(int f = 0; f < buf->getVertexCount(); f++)
				{
					vb2[f].Pos *= VertexScale;
				}
			break;
		case irr::video::EVT_TANGENTS:
			vbt = (S3DVertexTangents*)buf->getVertices();
		
			for(int f = 0; f < buf->getVertexCount(); f++)
			{
				vbt[f].Pos *= VertexScale;
			}
			break;
		}

	}

	// Uodate buffers
	jockgnome(node);
}

DLLPRE float DLLEX bbdx2_GetTransformedBoundingBoxMinX(IAnimatedMeshSceneNode* node)
{
	irr::core::aabbox3df BoundingBox(node->getLocalMesh()->getMesh(0)->getBoundingBox());
	node->getAbsoluteTransformation().transformBoxEx(BoundingBox);

	return BoundingBox.MinEdge.X;
}

DLLPRE float DLLEX bbdx2_GetTransformedBoundingBoxMinY(IAnimatedMeshSceneNode* node)
{
	irr::core::aabbox3df BoundingBox(node->getLocalMesh()->getMesh(0)->getBoundingBox());
	node->getAbsoluteTransformation().transformBoxEx(BoundingBox);

	return BoundingBox.MinEdge.Y;
}

DLLPRE float DLLEX bbdx2_GetTransformedBoundingBoxMinZ(IAnimatedMeshSceneNode* node)
{
	irr::core::aabbox3df BoundingBox(node->getLocalMesh()->getMesh(0)->getBoundingBox());
	node->getAbsoluteTransformation().transformBoxEx(BoundingBox);

	return BoundingBox.MinEdge.Z;
}

DLLPRE float DLLEX bbdx2_GetTransformedBoundingBoxMaxX(IAnimatedMeshSceneNode* node)
{
	irr::core::aabbox3df BoundingBox(node->getLocalMesh()->getMesh(0)->getBoundingBox());
	node->getAbsoluteTransformation().transformBoxEx(BoundingBox);

	return BoundingBox.MaxEdge.X;
}

DLLPRE float DLLEX bbdx2_GetTransformedBoundingBoxMaxY(IAnimatedMeshSceneNode* node)
{
	irr::core::aabbox3df BoundingBox(node->getLocalMesh()->getMesh(0)->getBoundingBox());
	node->getAbsoluteTransformation().transformBoxEx(BoundingBox);

	return BoundingBox.MaxEdge.Y;
}

DLLPRE float DLLEX bbdx2_GetTransformedBoundingBoxMaxZ(IAnimatedMeshSceneNode* node)
{
	irr::core::aabbox3df BoundingBox(node->getLocalMesh()->getMesh(0)->getBoundingBox());
	node->getAbsoluteTransformation().transformBoxEx(BoundingBox);

	return BoundingBox.MaxEdge.Z;
}

// Mesh Width
DLLPRE irr::f32 DLLEX manonworld(IAnimatedMeshSceneNode* Node)
{
	return Node->getLocalMesh()->getMesh(Node->getFrameNr())->getBoundingBox().getExtent().X;
}

// Mesh Height
DLLPRE irr::f32 DLLEX jumpin(IAnimatedMeshSceneNode* Node)
{
	return Node->getLocalMesh()->getMesh(Node->getFrameNr())->getBoundingBox().getExtent().Y;
}

// Mesh Depth
DLLPRE irr::f32 DLLEX needsleep(IAnimatedMeshSceneNode* Node)
{
	return Node->getLocalMesh()->getMesh(Node->getFrameNr())->getBoundingBox().getExtent().Z;
}

// Surface Count
DLLPRE int DLLEX openeyes(IAnimatedMeshSceneNode* Node)
{
	return (int)Node->getLocalMesh()->getMesh(Node->getFrameNr())->getMeshBufferCount();
}

// Get Surface
DLLPRE IMeshBuffer* DLLEX getupgetout(IAnimatedMeshSceneNode* Node, int Index)
{
	--Index; // Blitz indices count from 1
	return Node->getLocalMesh()->getMesh(Node->getFrameNr())->getMeshBuffer(Index);
}

// Update normals
DLLPRE void DLLEX eveclassic(IAnimatedMeshSceneNode* Node)
{
	smgr->getMeshManipulator()->recalculateNormals((IMesh*)Node->getLocalMesh()->getMesh(0));
	jockgnome(Node);
}


// Determine if the mesh contains alpha details
bool IsMeshAlpha(IAnimatedMesh* mesh)
{
	// Loop through every buffer
	for(int i = 0; i < mesh->getMesh(0)->getMeshBufferCount(); ++i)
	{
		// Obtain vertices
		IMeshBuffer* mb = mesh->getMesh(0)->getMeshBuffer(i);
		S3DVertex* vb;
		S3DVertex2TCoords* vb2;
		S3DVertexTangents* vbt;

		// Check alpha values of each vertex type
		switch(mb->getVertexType())
		{
		case ::EVT_STANDARD:
			vb = (::S3DVertex*)mb->getVertices();
			for(int f = 0; f < mb->getVertexCount(); ++f)
				if(vb[f].Color.getAlpha() < 1.0f)
				{
					mb->VertexAlpha = true;
					return true;
				}
			break;
		case ::EVT_2TCOORDS:
			vb2 = (::S3DVertex2TCoords*)mb->getVertices();
			for(int f = 0; f < mb->getVertexCount(); ++f)
				if(vb2[f].Color.getAlpha() < 1.0f)
				{
					mb->VertexAlpha = true;
					return true;
				}
			break;
		case ::EVT_TANGENTS:
			vbt = (::S3DVertexTangents*)mb->getVertices();
			for(int f = 0; f < mb->getVertexCount(); ++f)
				if(vbt[f].Color.getAlpha() < 1.0f)
				{
					mb->VertexAlpha = true;
					return true;
				}
			break;
		}
	}

	return false;
}




IAnimatedMeshSceneNode* Realjalkming(irr::c8* filename, ISceneNode* parent, int Animated, bbdx2_ASyncJobFn completionCallback, void* userData);

DLLPRE IAnimatedMeshSceneNode* DLLEX jalkming(irr::c8* filename, ISceneNode* parent, int Animated, bbdx2_ASyncJobFn completionCallback, void* userData)
{
	SEHSTART;

	return Realjalkming(filename, parent, Animated, completionCallback, userData);

	SEHEND;
}

// Load a mesh
IAnimatedMeshSceneNode* Realjalkming(irr::c8* filename, ISceneNode* parent, int Animated, bbdx2_ASyncJobFn completionCallback, void* userData)
{
	std::string LStr = filename;
	std::transform(LStr.begin(), LStr.end(), LStr.begin(), ::tolower);

	if(LStr.size() > 4 && LStr.substr(LStr.size() - 4) == ".xml")
	{
		return reinterpret_cast<IAnimatedMeshSceneNode*>(bbdx2_LoadGroupedObject(filename, parent));
	}

	// Load the mesh
	IAnimatedMesh* mesh = smgr->getMesh(filename, (Animated > 0), completionCallback, userData);

	// If it does exist, return 0 (or throw)
	if(!mesh)
	{
		if(DEBUGMODE)
		{
			char out[128];
			sprintf(out,"Error - Could not load mesh: %s",filename);
			MessageBox(NULL, out, "BBDX Debugger", MB_ICONERROR | MB_OK);
		}
		return 0;
	}



	// Add the scenenode
    IAnimatedMeshSceneNode* node = smgr->addAnimatedMeshSceneNode(mesh);

	//printf("Loaded: %x: %s\n", node, mesh->getDebugName());

	// Check it or throw
	if(!node)
	{
		if(DEBUGMODE)
		{
			char out[128];
			sprintf(out,"Error - Could not create a node: %s",filename);
			MessageBox(NULL, out, "BBDX Debugger", MB_ICONERROR | MB_OK);
		}
		return 0;
	}

	node->PublicName = filename;

	// Test all of the textures
	for(irr::s32 i = 0; i < node->getMaterialCount(); ++i)
	{
		if( !node->getMaterial(i).Texture1 )
			node->getMaterial(i).Texture1 = driver->getDefaultTexture();

		if( !node->getMaterial(i).Texture2 )
			node->getMaterial(i).Texture2 = driver->getDefaultTexture();

		if( !node->getMaterial(i).Texture3 )
			node->getMaterial(i).Texture3 = driver->getDefaultTexture();

		if( !node->getMaterial(i).Texture4 )
			node->getMaterial(i).Texture4 = driver->getDefaultTexture();
	}

	// Use this for alpha checking
	bool isAlpha = false;

	// Determine if any textures are alpha
	for(irr::s32 i = 0; i < mesh->getMesh(0)->getMeshBufferCount(); ++i)
		for(irr::u32 f = 0; f < 4; ++f)
			if(mesh->getMesh(0)->getMeshBuffer(i)->getMaterial().Textures[f] && mesh->getMesh(0)->getMeshBuffer(i)->getMaterial().Textures[f]->isAlpha)
			{
				isAlpha = true;
				mesh->getMesh(0)->getMeshBuffer(i)->TextureAlpha = true;
			}


	// Make sure the node knows this
	bool TexAlpha = isAlpha;
	node->TexAlpha = TexAlpha;

	// Check for alpha in the mesh, and if its forced
	node->ForceAlpha = node->ForceAlpha || IsMeshAlpha(mesh) || isAlpha;
	if(node->ForceAlpha)
		node->setMaterialType(::EMT_TRANSPARENT_ALPHA_CHANNEL);

	// RCTE terrains which use the horrible surface-layer system need to be drawn as solid
	std::string str = filename;
	if(str.find("\\RCTE\\",0) != std::string::npos || str.find("TE_",0) != std::string::npos)
	{
		node->ForceAlpha = false;
		node->setMaterialType(::EMT_SOLID);
	}

	// Used for animations
	node->setAnimationCPY();
	node->Anicnt = 0;

	// Setup default datas
	node->setName(filename);
	node->setDebugDataVisible(DEBUGMODE);
	node->CollisionID = GetCollisionID();
	//node->EntityType = 250;
	//CollisionTypePRO(node->CollisionID, 250);
	//CollisionEntities.push_back(node);

	// Create an interator to search through children
	const irr::core::list<ISceneNode*>& Children = node->getChildren();
	irr::core::list<ISceneNode*>::Iterator it = Children.begin();
	irr::core::array<ISceneNode*> Kids;
	// Search through children
	for (; it != Children.end(); ++it)
		if(*it)
		{
			irr::core::stringc NDat = "Bone: ";
			NDat += (*it)->getName();
			//DB(NDat.c_str());
		}

	// Disable Mesh Animation
	int Seq = ExtractAnimSeq(node,0,0);
	Animate(node,1,1,Seq);
 
    return node;
}

// FitMesh
DLLPRE void DLLEX reachout(IAnimatedMeshSceneNode* EN, float X, float Y, float Z, float Width, float Height, float Depth, int UniformFlag)
{
	// Get Width,Height,Depth
	irr::f32 MW = EN->getLocalMesh()->getMesh(EN->getFrameNr())->getBoundingBox().getExtent().X;
    irr::f32 MH = EN->getLocalMesh()->getMesh(EN->getFrameNr())->getBoundingBox().getExtent().Y;
    irr::f32 MD = EN->getLocalMesh()->getMesh(EN->getFrameNr())->getBoundingBox().getExtent().Z;
    irr::f32 MX, MY, MZ;

    // Scale mesh to new size
    if (UniformFlag == 0)
        soreneck(EN, Width / MW, Height / MH, Depth / MD);
    else
    {
        float XScale = Width / MW;
        float YScale = Height / MH;
        float ZScale = Depth / MD;
        float FinalScale = XScale;
        if (YScale < FinalScale)
            FinalScale = YScale;
        if (ZScale < FinalScale)
            FinalScale = ZScale;

        soreneck(EN, FinalScale, FinalScale, FinalScale);

        // Move offset to account for changes in scaling factor
        X *= FinalScale / XScale;
        Y *= FinalScale / YScale;
        Z *= FinalScale / ZScale;
    }

    // Set new mesh origin
    MX = EN->getLocalMesh()->getBoundingBox().MinEdge.X;
    MY = EN->getLocalMesh()->getBoundingBox().MinEdge.Y;
    MZ = EN->getLocalMesh()->getBoundingBox().MinEdge.Z;
    softskin(EN, X - MX, Y - MY, Z - MZ);
}

// Copy Mesh (better than the other!)
DLLPRE IAnimatedMeshSceneNode* DLLEX CopyMesh(IAnimatedMeshSceneNode* node, ISceneNode* parent)
{
	SAnimatedMesh* Mesh = new SAnimatedMesh();
	Mesh->addMesh(smgr->getMeshManipulator()->createMeshCopy(node->getLocalMesh()->getMesh(0)));
	IAnimatedMeshSceneNode* nu = smgr->addAnimatedMeshSceneNode(Mesh,parent);
	//node->getLocalMesh()->grab();
	nu->CollisionID = GetCollisionID();

	nu->setPosition(node->getPosition());
	nu->setRotation(node->getRotation());
	nu->setScale(node->getScale());

	nu->setVisible(true);

	for(irr::s32 i = 0; i < node->getMaterialCount(); ++i)
	{
		nu->getMaterial(i).AmbientColor = node->getMaterial(i).AmbientColor;
		nu->getMaterial(i).AnisotropicFilter = node->getMaterial(i).AnisotropicFilter;
		nu->getMaterial(i).BackfaceCulling = node->getMaterial(i).BackfaceCulling;
		nu->getMaterial(i).BilinearFilter = node->getMaterial(i).BilinearFilter;
		nu->getMaterial(i).DiffuseColor = node->getMaterial(i).DiffuseColor;
		nu->getMaterial(i).EmissiveColor = node->getMaterial(i).EmissiveColor;
		nu->getMaterial(i).FogEnable = node->getMaterial(i).FogEnable;
		nu->getMaterial(i).GouraudShading = node->getMaterial(i).GouraudShading;
		nu->getMaterial(i).Lighting = node->getMaterial(i).Lighting;
		nu->getMaterial(i).MaterialType = node->getMaterial(i).MaterialType;
		nu->getMaterial(i).MaterialTypeParam = node->getMaterial(i).MaterialTypeParam;
		nu->getMaterial(i).MaterialTypeParam2 = node->getMaterial(i).MaterialTypeParam2;
		nu->getMaterial(i).NormalizeNormals = node->getMaterial(i).NormalizeNormals;
		nu->getMaterial(i).PointCloud = node->getMaterial(i).PointCloud;
		nu->getMaterial(i).Shininess = node->getMaterial(i).Shininess;
		nu->getMaterial(i).SpecularColor = node->getMaterial(i).SpecularColor;
		nu->getMaterial(i).Texture1 = node->getMaterial(i).Texture1;
		nu->getMaterial(i).Texture2 = node->getMaterial(i).Texture2;
		nu->getMaterial(i).Texture3 = node->getMaterial(i).Texture3;
		nu->getMaterial(i).Texture4 = node->getMaterial(i).Texture4;
		nu->getMaterial(i).TrilinearFilter = node->getMaterial(i).TrilinearFilter;
		nu->getMaterial(i).Wireframe = node->getMaterial(i).Wireframe;
		nu->getMaterial(i).ZBuffer = node->getMaterial(i).ZBuffer;
		nu->getMaterial(i).ZWriteEnable = node->getMaterial(i).ZWriteEnable;

		if(nu->getMaterial(i).Texture1)
			nu->getMaterial(i).Texture1->grab();

		if(nu->getMaterial(i).Texture2)
			nu->getMaterial(i).Texture2->grab();

		if(nu->getMaterial(i).Texture3)
			nu->getMaterial(i).Texture3->grab();

		if(nu->getMaterial(i).Texture4)
			nu->getMaterial(i).Texture4->grab();

		

		for(irr::s32 f = 0; f < 12; ++f)
		{
			nu->getMaterial(i).Flags[f] = node->getMaterial(i).Flags[f];
		}
	}

	nu->setEffect(node->getEffect());
	//nu->setRCFXShader(node->getRCFXShader());

	//nu->EntityType = 250;
	//CollisionTypePRO(nu->CollisionID, 250);
	//CollisionEntities.push_back(nu);

	// Update buffers
	jockgnome(nu);

	return nu;

}

DLLPRE int DLLEX GetMaterialCount(IAnimatedMeshSceneNode* Node)
{
	return Node->getMaterialCount();
}

// Mesh LOD
DLLPRE void DLLEX closeeyes( IAnimatedMeshSceneNode* Node, IAnimatedMeshSceneNode* NodeLOD, int iLOD, float Distance )
{
	switch (iLOD)
	{
	case 0: Node->setMeshLOD_HIGH( (NodeLOD)?NodeLOD->getLocalMesh():NULL, Distance);	break;
	case 1: Node->setMeshLOD_MEDIUM((NodeLOD)?NodeLOD->getLocalMesh():NULL, Distance);	break;
	case 2: Node->setMeshLOD_LOW((NodeLOD)?NodeLOD->getLocalMesh():NULL, Distance);	break;
	}
}
