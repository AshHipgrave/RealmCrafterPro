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
//* BBDX - Surface
//*



// Gets a verts tangent
vector3df getVertTangent(SMeshBuffer* Buffer, int Index)
{
	void* vbd = Buffer->getVertices();
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_TANGENTS:
		return vbt[Index].Tangent;
		break;
	}

	// Compiler Satisfaction
	return vector3df(0,0,0);
}

// Gets a vert binormal
vector3df getVertBiNormal(SMeshBuffer* Buffer, int Index)
{
	void* vbd = Buffer->getVertices();
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_TANGENTS:
		return vbt[Index].Binormal;
		break;
	}

	// Compiler Satisfaction
	return vector3df(0,0,0);
}

// Gets a vertex position
vector3df getVertPos(SMeshBuffer* Buffer, int Index)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		return vb[Index].Pos;
		break;
	case irr::video::EVT_2TCOORDS:
		return vb2[Index].Pos;
		break;
	case irr::video::EVT_TANGENTS:
		return vbt[Index].Pos;
		break;
	}

	// Compiler Satisfaction
	return vector3df(0,0,0);
}

// Sets a vertex position
void setVertPos(SMeshBuffer* Buffer, int Index, vector3df& NewPos)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		vb[Index].Pos = NewPos;
		break;
	case irr::video::EVT_2TCOORDS:
		vb2[Index].Pos = NewPos;
		break;
	case irr::video::EVT_TANGENTS:
		vbt[Index].Pos = NewPos;
		break;
	}
}

// Gets a verts texcoords
vector2df getVertCoords(SMeshBuffer* Buffer, int Index)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		return vb[Index].TCoords;
		break;
	case irr::video::EVT_2TCOORDS:
		return vb2[Index].TCoords;
		break;
	case irr::video::EVT_TANGENTS:
		return vbt[Index].TCoords;
		break;
	}

	// Compiler Satisfaction
	return vector2df(0,0);
}

// Sets a verts texcoords
void setVertCoords(SMeshBuffer* Buffer, int Index, vector2df& NewCoords)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		vb[Index].TCoords = NewCoords;
		break;
	case irr::video::EVT_2TCOORDS:
		vb2[Index].TCoords = NewCoords;
		break;
	case irr::video::EVT_TANGENTS:
		vbt[Index].TCoords = NewCoords;
		break;
	}
}

// Gets a verts normal
vector3df getVertNorm(SMeshBuffer* Buffer, int Index)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		return vb[Index].Normal;
		break;
	case irr::video::EVT_2TCOORDS:
		return vb2[Index].Normal;
		break;
	case irr::video::EVT_TANGENTS:
		return vbt[Index].Normal;
		break;
	}

	// Compiler Satisfaction
	return vector3df(0,0,0);
}

// Sets a verts normal
void setVertNorm(SMeshBuffer* Buffer, int Index, vector3df& NewNormal)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		vb[Index].Normal = NewNormal;
		break;
	case irr::video::EVT_2TCOORDS:
		vb2[Index].Normal = NewNormal;
		break;
	case irr::video::EVT_TANGENTS:
		vbt[Index].Normal = NewNormal;
		break;
	}
}

// Gets a vertex colour
SColor getVertCol(SMeshBuffer* Buffer, int Index)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		return vb[Index].Color;
		break;
	case irr::video::EVT_2TCOORDS:
		return vb2[Index].Color;
		break;
	case irr::video::EVT_TANGENTS:
		return vbt[Index].Color;
		break;
	}

	// Compiler Satisfaction
	return SColor(0,0,0,0);
}

// Sets a vertex colour
void setVertCol(SMeshBuffer* Buffer, int Index, SColor NewColor)
{
	void* vbd = Buffer->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

	switch(Buffer->getVertexType())
	{
	case irr::video::EVT_STANDARD:
		vb[Index].Color = NewColor;
		break;
	case irr::video::EVT_2TCOORDS:
		vb2[Index].Color = NewColor;
		break;
	case irr::video::EVT_TANGENTS:
		vbt[Index].Color = NewColor;
		break;
	}
}

// Vertex X
DLLPRE irr::f32 DLLEX laptop(SMeshBuffer* Buffer, int Index)
{
	return getVertPos(Buffer, Index).X;
}

// Vertex Y
DLLPRE irr::f32 DLLEX watchingspace(SMeshBuffer* Buffer, int Index)
{
	return getVertPos(Buffer, Index).Y;
}

// Vertex Z
DLLPRE irr::f32 DLLEX garagedoor(SMeshBuffer* Buffer, int Index)
{
	return getVertPos(Buffer, Index).Z;
}

// Vertex NX
DLLPRE irr::f32 DLLEX unprecedented(SMeshBuffer* Buffer, int Index)
{
	return getVertNorm(Buffer, Index).X;
}

// Vertex NY
DLLPRE irr::f32 DLLEX processing(SMeshBuffer* Buffer, int Index)
{
	return getVertNorm(Buffer, Index).Y;
}

// Vertex NZ
DLLPRE irr::f32 DLLEX efficiency(SMeshBuffer* Buffer, int Index)
{
	return getVertNorm(Buffer, Index).Z;
}

// Vertex Red
DLLPRE int DLLEX broughtyou(SMeshBuffer* Buffer, int Index)
{
	return getVertCol(Buffer, Index).getRed();
}

// Vertex Green
DLLPRE int DLLEX concerned(SMeshBuffer* Buffer, int Index)
{
	return getVertCol(Buffer, Index).getGreen();
}

// Vertex Blue
DLLPRE int DLLEX excitingtimes(SMeshBuffer* Buffer, int Index)
{
	return getVertCol(Buffer, Index).getBlue();
}

// Vertex U
DLLPRE float DLLEX VertexU(SMeshBuffer* Buffer, int Index)
{
	return getVertCoords(Buffer, Index).X;
}

// Vertex V
DLLPRE float DLLEX VertexV(SMeshBuffer* Buffer, int Index)
{
	return getVertCoords(Buffer, Index).Y;
}

DLLPRE float DLLEX VertexTX(SMeshBuffer* Buffer, int Index)
{
	return getVertTangent(Buffer, Index).X;
}

DLLPRE float DLLEX VertexTY(SMeshBuffer* Buffer, int Index)
{
	return getVertTangent(Buffer, Index).Y;
}

DLLPRE float DLLEX VertexTZ(SMeshBuffer* Buffer, int Index)
{
	return getVertTangent(Buffer, Index).Z;
}

DLLPRE float DLLEX VertexBX(SMeshBuffer* Buffer, int Index)
{
	return getVertBiNormal(Buffer, Index).X;
}

DLLPRE float DLLEX VertexBY(SMeshBuffer* Buffer, int Index)
{
	return getVertBiNormal(Buffer, Index).Y;
}

DLLPRE float DLLEX VertexBZ(SMeshBuffer* Buffer, int Index)
{
	return getVertBiNormal(Buffer, Index).Z;
}

// Vertex Alpha
DLLPRE irr::f32 DLLEX supportdx(SMeshBuffer* Buffer, int Index)
{
	return getVertCol(Buffer, Index).getAlpha() / 255.0f;
}

// Get Vertex from triangle index, corner
DLLPRE int DLLEX evevista(SMeshBuffer* Buffer, int Index, int Corner)
{
	irr::u32* Indices = (irr::u32*)Buffer->getIndices();
	return Indices[(Index * 3) + Corner];
}

// Clear a buffer
DLLPRE void DLLEX wannabe(SMeshBuffer* Buffer)
{
	Buffer->Vertices.clear();
	Buffer->Indices.clear();
}

// Paint Surface
DLLPRE void DLLEX withluck(SMeshBuffer* Buffer, SMaterial* Material)
{
	Buffer->Material = *Material;
}

// Get Surface?
DLLPRE SMeshBuffer* DLLEX andtakeit(IAnimatedMeshSceneNode* Node)
{
	return (SMeshBuffer*)Node->getLocalMesh()->getMesh(0)->getMeshBuffer(0);
}

// Get Vertex Count
DLLPRE int DLLEX aboveandbeyond(IMeshBuffer* Buffer)
{
	return Buffer->getVertexCount();
}

// Triangle Count
DLLPRE int DLLEX motorcycle(IMeshBuffer* Buffer)
{
	return (int)(Buffer->getIndexCount() / 3);
}

// Add Vertex
DLLPRE int DLLEX withoutyou(SMeshBuffer* Buffer, irr::f32 x, irr::f32 y, irr::f32 z, irr::f32 u, irr::f32 v, irr::f32 w)
{
	Buffer->Vertices.push_back(S3DVertex(x, y, z, 0, -1, 0, SColor(255, 255, 255, 255), u, v));
	return Buffer->Vertices.size() - 1;
}

// Vertex Position
DLLPRE void DLLEX waverider(SMeshBuffer* Buffer, int Index, irr::f32 x, irr::f32 y, irr::f32 z)
{
	setVertPos(Buffer, Index, vector3df(x, y, z));
}

// Vertex Normal
DLLPRE void DLLEX groundbreaking(SMeshBuffer* Buffer, int Index, irr::f32 x, irr::f32 y, irr::f32 z)
{
	setVertNorm(Buffer, Index, vector3df(x, y, z));
}

// Vertex Coords
DLLPRE void DLLEX happyclap(SMeshBuffer* Buffer, int Index, irr::f32 u, irr::f32 v)
{
	setVertCoords(Buffer, Index, vector2df(u, v));
}

// Vertex Color
DLLPRE void DLLEX urallnoobs(SMeshBuffer* Buffer, int Index, int r, int g, int b, float a)
{	
	setVertCol(Buffer, Index, SColor(a * 255.0f, r, g, b));
}

// Add Triangle
DLLPRE int DLLEX tellme(SMeshBuffer* Buffer, int v0, int v1, int v2)
{
	irr::s32 IndexCount = Buffer->getIndexCount();
	Buffer->Indices.push_back(v0);
	Buffer->Indices.push_back(v1);
	Buffer->Indices.push_back(v2);
	return (int)IndexCount;
}

// Get Index Count
DLLPRE int DLLEX CountIndices(SMeshBuffer* Buffer)
{
	return Buffer->getIndexCount();	
}

// Get index data
DLLPRE int DLLEX GetIndex(SMeshBuffer* Buffer, int Index)
{
	irr::u32* Indices = Buffer->getIndices();
	return Indices[Index];
}

// Copy one surface to another
DLLPRE void DLLEX CopySurface(SMeshBuffer* Buf1, SMeshBuffer* Buf2)
{
	void* vbd = Buf1->getVertices();
	S3DVertex* vb = (S3DVertex*)vbd;
	irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
	irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;
	irr::u32* Indices = Buf1->getIndices();

	switch(Buf1->getVertexType())
	{
	case ::EVT_STANDARD:
		for(irr::s32 i = 0; i < Buf1->getVertexCount(); ++i)
			Buf2->Vertices.push_back(vb[i]);
		break;
	case ::EVT_2TCOORDS:
		for(irr::s32 i = 0; i < Buf1->getVertexCount(); ++i)
		{
			::S3DVertex V;
			V.Pos = vb2[i].Pos;
			V.Normal = vb2[i].Normal;
			V.Color = vb2[i].Color;
			V.TCoords = vb2[i].TCoords;
			Buf2->Vertices.push_back(V);
		}
		break;
	case ::EVT_TANGENTS:
		for(irr::s32 i = 0; i < Buf1->getVertexCount(); ++i)
		{
			::S3DVertex V;
			V.Pos = vbt[i].Pos;
			V.Normal = vbt[i].Normal;
			V.Color = vbt[i].Color;
			V.TCoords = vbt[i].TCoords;
			Buf2->Vertices.push_back(V);
		}
		break;
	}

	for(irr::s32 i = 0; i < Buf1->getIndexCount(); ++i)
		Buf2->Indices.push_back(Indices[i]);
}