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
#include "BBThread.h"
//* BBDX - Collision
//*

void RealTempGenBuffers(IAnimatedMeshSceneNode* node, int index, irr::core::vector3df** vertices, irr::u32** indices, irr::u32* vertexCount, irr::u32* indexCount);
void RealTempRemBuffers(irr::core::vector3df** vertices, irr::u32** indices);

DLLPRE void DLLEX TempGenBuffers(IAnimatedMeshSceneNode* node, int index, irr::core::vector3df** vertices, irr::u32** indices, irr::u32* vertexCount, irr::u32* indexCount)
{
	SEHSTART;

	RealTempGenBuffers(node, index, vertices, indices, vertexCount, indexCount);

	SEHEND;
}

DLLPRE void DLLEX TempRemBuffers(irr::core::vector3df** vertices, irr::u32** indices)
{
	SEHSTART;

	RealTempRemBuffers(vertices, indices);

	SEHEND;
}

// Add mesh to collisions
void RealTempGenBuffers(IAnimatedMeshSceneNode* node, int index, irr::core::vector3df** vertices, irr::u32** indices, irr::u32* vertexCount, irr::u32* indexCount)
{
	// Get the mesh
	IMesh* Mesh = node->getLocalMesh()->getMesh(0);

	if(index < 0 || index >= Mesh->getMeshBufferCount())
		return;

	*vertexCount = Mesh->getMeshBuffer(index)->getVertexCount();
	*indexCount = Mesh->getMeshBuffer(index)->getIndexCount();

	(*vertices) = new irr::core::vector3df[*vertexCount];
	(*indices) = new irr::u32[*indexCount];

	void* vbd = Mesh->getMeshBuffer(index)->getVertices();
	irr::u32* ib = (irr::u32*)Mesh->getMeshBuffer(index)->getIndices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;
	irr::video::SAnimatedVert* vba = (irr::video::SAnimatedVert*)vbd;

	for(irr::u32 i = 0; i < *vertexCount; ++i)
	{
		switch(Mesh->getMeshBuffer(index)->getVertexType())
		{
		case irr::video::EVT_STANDARD:
			(*vertices)[i] = vb[i].Pos;
			break;
		case irr::video::EVT_2TCOORDS:
			(*vertices)[i] = vb2[i].Pos;
			break;
		case irr::video::EVT_TANGENTS:
			(*vertices)[i] = vbt[i].Pos;
			break;
		case irr::video::EVT_ANIMATEDVERT:
			(*vertices)[i] = vba[i].Pos;
			break;
		}
	}

	for(irr::u32 i = 0; i < *indexCount; ++i)
		(*indices)[i] = ib[i];
}

void RealTempRemBuffers(irr::core::vector3df** vertices, irr::u32** indices)
{
	delete *vertices;
	delete *indices;
}

// Update the 3D world
DLLPRE void DLLEX anyoneelse()
{
	SEHSTART;

	// Update Collisions
	//RunCollision();
	bbdx2_UpdateWorld();

	// Update Sound
	UpdateSound();

	// Threadsync
	bbdx2_ASyncJobSync();

	SEHEND;
}

// Get an ID to assign to collisions
// NOTE: NGC needs work to allow object handles
irr::s32 GetCollisionID()
{
	++SceneObjectCount;
	return SceneObjectCount;
}

DLLPRE void DLLEX bbdx2_ProjectVector3(D3DXVECTOR3* Point, D3DXVECTOR3* Out);
DLLPRE void DLLEX bbdx2_UnProjectVector3(D3DXVECTOR3* Point, D3DXVECTOR3* Out);

D3DXVECTOR3 ManagedProject(0, 0, 0);

DLLPRE void DLLEX bbdx2_ManagedProjectVector3(float x, float y, float z)
{
	D3DXVECTOR3 Point(x, y, z);
	bbdx2_ProjectVector3(&Point, &ManagedProject);
}

DLLPRE void DLLEX bbdx2_ManagedUnProjectVector3(float x, float y, float z)
{
	D3DXVECTOR3 Point(x, y, z);
	bbdx2_UnProjectVector3(&Point, &ManagedProject);
}

DLLPRE float DLLEX bbdx2_ProjectedX()
{
	return ManagedProject.x;
}

DLLPRE float DLLEX bbdx2_ProjectedY()
{
	return ManagedProject.y;
}

DLLPRE float DLLEX bbdx2_ProjectedZ()
{
	return ManagedProject.z;
}


DLLPRE void DLLEX bbdx2_ProjectVector3(D3DXVECTOR3* Point, D3DXVECTOR3* Out)
{
	D3DVIEWPORT9 ViewPort;
	((video::CD3D9Driver*)driver)->pID3DDevice->GetViewport(&ViewPort);

	D3DXMATRIX World, View, Projection;
	D3DXMatrixIdentity(&World);
	
	core::matrix4 MatView = smgr->getActiveCamera()->getViewMatrix();
	core::matrix4 MatProjection = smgr->getActiveCamera()->getProjectionMatrix();
	View = *((D3DXMATRIX*)&MatView);
	Projection = *((D3DXMATRIX*)&MatProjection);

	D3DXVec3Project(Out, Point, &ViewPort, &Projection, &View, &World);

	Out->x /= ViewPort.Width;
	Out->y /= ViewPort.Height;
}

DLLPRE void DLLEX bbdx2_UnProjectVector3(D3DXVECTOR3* Point, D3DXVECTOR3* Out)
{
	D3DVIEWPORT9 ViewPort;
	((video::CD3D9Driver*)driver)->pID3DDevice->GetViewport(&ViewPort);

	D3DXMATRIX World, View, Projection;
	D3DXMatrixIdentity(&World);
	
	core::matrix4 MatView = smgr->getActiveCamera()->getViewMatrix();
	core::matrix4 MatProjection = smgr->getActiveCamera()->getProjectionMatrix();
	View = *((D3DXMATRIX*)&MatView);
	Projection = *((D3DXMATRIX*)&MatProjection);
	
	D3DXVec3Unproject(Out, Point, &ViewPort, &Projection, &View, &World);
}

DLLPRE int DLLEX bbdx2_RayToPlane(float nx, float ny, float nz, float px, float py, float pz, float sx, float sy, float sz, float ex, float ey, float ez)
{
	irr::core::line3df Line;
	Line.start.X = sx;
	Line.start.Y = sy;
	Line.start.Z = sz;
	Line.end.X = ex;
	Line.end.Y = ey;
	Line.end.Z = ez;

	irr::core::vector3df Intersection;

	irr::core::plane3df Plane(px, py, pz, nx, ny, nz);
	if(!Plane.getIntersectionWithLimitedLine(Line.start, Line.end, Intersection))
		return 0;

	ManagedProject.x = Intersection.X;
	ManagedProject.y = Intersection.Y;
	ManagedProject.z = Intersection.Z;

	return 1;
}

// This is in here because its too much effort to import d3dx into C#:
DLLPRE int DLLEX bbdx2_RayToAABB(float minx, float miny, float minz, float maxx, float maxy, float maxz, float sx, float sy, float sz, float ex, float ey, float ez)
{
	irr::core::aabbox3df Box;
	Box.MinEdge.X = minx;
	Box.MinEdge.Y = miny;
	Box.MinEdge.Z = minz;
	Box.MaxEdge.X = maxx;
	Box.MaxEdge.Y = maxy;
	Box.MaxEdge.Z = maxz;

	irr::core::line3df Line;
	Line.start.X = sx;
	Line.start.Y = sy;
	Line.start.Z = sz;
	Line.end.X = ex;
	Line.end.Y = ey;
	Line.end.Z = ez;

	return Box.intersectsWithLine(Line) ? 1 : 0;
}

// 	D3DXVECTOR3 LineStart(sx, sy, sz);
// 	D3DXVECTOR3 LineEnd(ex, ey, ez);
// 	D3DXVECTOR3 Min(minx, miny, minz);
// 	D3DXVECTOR3 Max(maxx, maxy, maxz);
// 
// 	D3DXVECTOR3 NormTop(0, 1, 0);
// 	D3DXVECTOR3 NormBottom(0, -1, 0);
// 	D3DXVECTOR3 NormLeft(-1, 0, 0);
// 	D3DXVECTOR3 NormRight(1, 0, 0);
// 	D3DXVECTOR3 NormFront(0, 0, -1);
// 	D3DXVECTOR3 NormBack(0, 0, 1);
// 
// 	D3DXPLANE Top, Bottom, Left, Right, Front, Back;
// 
// 	D3DXPlaneFromPointNormal(&Top,    &Max, &NormTop);
// 	D3DXPlaneFromPointNormal(&Bottom, &Min, &NormBottom);
// 	D3DXPlaneFromPointNormal(&Left,   &Min, &NormLeft);
// 	D3DXPlaneFromPointNormal(&Right,  &Max, &NormRight);
// 	D3DXPlaneFromPointNormal(&Front,  &Min, &NormFront);
// 	D3DXPlaneFromPointNormal(&Back,   &Max, &NormBack);
// 
// 
// 	D3DXVECTOR3 Intersect(0, 0, 0);
// 	if(D3DXPlaneIntersectLine(&Intersect, &Top, &LineStart, &LineEnd) != NULL)
// 	{
// 		if(Intersect.x >= Min.x && Intersect.y >= Min.y && Intersect.z >= Min.z
// 			&& Intersect.x <= Min.x && Intersect.y <= Max.y && Intersect.z <= Max.z)
// 			return 1;
// 	}
// 
// 	if(D3DXPlaneIntersectLine(&Intersect, &Bottom, &LineStart, &LineEnd) != NULL)
// 	{
// 		if(Intersect.x >= Min.x && Intersect.y >= Min.y && Intersect.z >= Min.z
// 			&& Intersect.x <= Min.x && Intersect.y <= Max.y && Intersect.z <= Max.z)
// 			return 1;
// 	}
// 
// 	if(D3DXPlaneIntersectLine(&Intersect, &Left, &LineStart, &LineEnd) != NULL)
// 	{
// 		if(Intersect.x >= Min.x && Intersect.y >= Min.y && Intersect.z >= Min.z
// 			&& Intersect.x <= Min.x && Intersect.y <= Max.y && Intersect.z <= Max.z)
// 			return 1;
// 	}
// 
// 	if(D3DXPlaneIntersectLine(&Intersect, &Right, &LineStart, &LineEnd) != NULL)
// 	{
// 		if(Intersect.x >= Min.x && Intersect.y >= Min.y && Intersect.z >= Min.z
// 			&& Intersect.x <= Min.x && Intersect.y <= Max.y && Intersect.z <= Max.z)
// 			return 1;
// 	}
// 
// 	if(D3DXPlaneIntersectLine(&Intersect, &Front, &LineStart, &LineEnd) != NULL)
// 	{
// 		if(Intersect.x >= Min.x && Intersect.y >= Min.y && Intersect.z >= Min.z
// 			&& Intersect.x <= Min.x && Intersect.y <= Max.y && Intersect.z <= Max.z)
// 			return 1;
// 	}
// 
// 	if(D3DXPlaneIntersectLine(&Intersect, &Back, &LineStart, &LineEnd) != NULL)
// 	{
// 		if(Intersect.x >= Min.x && Intersect.y >= Min.y && Intersect.z >= Min.z
// 			&& Intersect.x <= Min.x && Intersect.y <= Max.y && Intersect.z <= Max.z)
// 			return 1;
// 	}
// 
// 	return 0;
//}


