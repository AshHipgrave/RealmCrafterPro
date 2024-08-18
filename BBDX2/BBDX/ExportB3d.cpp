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
#include "ExportB3D.h"

/////////////////////////////////////////////////////////////////////////
//  
/////////////////////////////////////////////////////////////////////////
int  cB3DChunk::GetLength()
{
	return  4+  // tag
		4; // length
}

void cB3DChunk::Export(FILE* file)
{
	fwrite(Tag, 1, 4, file );
	int length = GetLength()-8;
	fwrite(&length, sizeof(int), 1, file);
}

bool cB3DChunk::ExportToFile( char* FileName )
{
	FILE* file;
	fopen_s(&file, FileName, "wb");
	if (!file) return false;

	Export(file);

	fclose(file);
	return true;
}

/////////////////////////////////////////////////////////////////////////
//  BB3D
/////////////////////////////////////////////////////////////////////////
cB3DChunk_BB3D::cB3DChunk_BB3D()
{
	memcpy(Tag, "BB3D", 4);
}

cB3DChunk_BB3D::~cB3DChunk_BB3D()
{
	for (list<cB3DChunk_TEXS*>::iterator iter(lpTextures.begin()); iter!=lpTextures.end(); iter++)
		delete *iter;
	for (list<cB3DChunk_BRUS*>::iterator iter(lpMaterials.begin()); iter!=lpMaterials.end(); iter++)
		delete *iter;
	for (list<cB3DChunk_NODE*>::iterator iter(lpNodes.begin()); iter!=lpNodes.end(); iter++)
		delete *iter;
}


int  cB3DChunk_BB3D::GetLength()
{
	int length = cB3DChunk::GetLength()+
		sizeof(int); // version

	for (list<cB3DChunk_TEXS*>::iterator iter(lpTextures.begin()); iter!=lpTextures.end(); iter++)
		length += (*iter)->GetLength();

	for (list<cB3DChunk_BRUS*>::iterator iter(lpMaterials.begin()); iter!=lpMaterials.end(); iter++)
		length += (*iter)->GetLength();

	for (list<cB3DChunk_NODE*>::iterator iter(lpNodes.begin()); iter!=lpNodes.end(); iter++)
		length += (*iter)->GetLength();

	return length;
}

void cB3DChunk_BB3D::Export(FILE* file)
{
	cB3DChunk::Export(file);
	int n = 0*100+1;  // version = v0.01
	fwrite(&n, sizeof(int), 1, file);

	for (list<cB3DChunk_TEXS*>::iterator iter(lpTextures.begin()); iter!=lpTextures.end(); iter++)
		(*iter)->Export(file);
	for (list<cB3DChunk_BRUS*>::iterator iter(lpMaterials.begin()); iter!=lpMaterials.end(); iter++)
		(*iter)->Export(file);
	for (list<cB3DChunk_NODE*>::iterator iter(lpNodes.begin()); iter!=lpNodes.end(); iter++)
		(*iter)->Export(file);
}

void cB3DChunk_BB3D::InsertTexture(cB3DChunk_TEXS* pTex)
{
	pTex->Index = (int)lpTextures.size();
	lpTextures.push_back(pTex);
}

void cB3DChunk_BB3D::InsertMaterial(cB3DChunk_BRUS* pMat)
{
	pMat->Index = (int)lpMaterials.size();
	lpMaterials.push_back(pMat);
}


/////////////////////////////////////////////////////////////////////////
//  TEXS
/////////////////////////////////////////////////////////////////////////
cB3DChunk_TEXS::cB3DChunk_TEXS()
{
	memcpy(Tag, "TEXS", 4);
	FileName = "unset";
	flags = 1; blend = 2;
	x = y = 0; 
	x_scale = y_scale = 1; 
	rotation = 0;
}

int  cB3DChunk_TEXS::GetLength()
{
	return cB3DChunk::GetLength()+
		(int)FileName.length()+1+ // Filename
		2*sizeof(int)+ // flags, blend
		5*sizeof(float); // x,y, xscale, yscale, rotation
}

void cB3DChunk_TEXS::Export(FILE* file)
{
	cB3DChunk::Export(file);
	fwrite(FileName.c_str(), 1, FileName.length()+1, file);
	fwrite(&flags, sizeof(int), 1, file);
	fwrite(&blend, sizeof(int), 1, file);
	fwrite(&x, sizeof(float), 1, file);
	fwrite(&y, sizeof(float), 1, file);
	fwrite(&x_scale, sizeof(float), 1, file);
	fwrite(&y_scale, sizeof(float), 1, file);
	fwrite(&rotation, sizeof(float), 1, file);
}

/////////////////////////////////////////////////////////////////////////
//  BRUS
/////////////////////////////////////////////////////////////////////////
cB3DChunk_BRUS::cB3DChunk_BRUS()
{
	memcpy(Tag, "BRUS", 4);

	MaterialName = "unset";
	red = green = blue = alpha = 1;
	shininess = 0;
	blend = 1; fx = 0;
}

int  cB3DChunk_BRUS::GetLength()
{
	return cB3DChunk::GetLength()+
		sizeof(int)+ // nTextures
		(int)MaterialName.length()+1+
		5*sizeof(float)+ // r,g,b,a, shininess
		2*sizeof(int)+ // blend, fx
		sizeof(int)*(int)lpTextures.size();
}

void cB3DChunk_BRUS::Export(FILE* file)
{
	cB3DChunk::Export(file);

	int n = (int)lpTextures.size();
	fwrite(&n, sizeof(int), 1, file);
	fwrite(MaterialName.c_str(), 1, MaterialName.length()+1, file);

	fwrite(&red, sizeof(float), 1, file);
	fwrite(&green, sizeof(float), 1, file);
	fwrite(&blue, sizeof(float), 1, file);
	fwrite(&alpha, sizeof(float), 1, file);
	fwrite(&shininess, sizeof(float), 1, file);
	fwrite(&blend, sizeof(int), 1, file);
	fwrite(&fx, sizeof(int), 1, file);
	for (list<cB3DChunk_TEXS*>::iterator iter(lpTextures.begin()); iter!=lpTextures.end(); iter++)
		fwrite(&(*iter)->Index, sizeof(int), 1, file);
}


/////////////////////////////////////////////////////////////////////////
//  NODE
/////////////////////////////////////////////////////////////////////////
cB3DChunk_NODE::cB3DChunk_NODE()
{
	memcpy(Tag, "NODE", 4);

	pTypeChunk = NULL;
	Name = "unset";
	position[0] = position[1] = position[2] = 0;
	scale[0]=scale[1]=scale[2]=1;
	rotation[0]=1;
	rotation[1]=rotation[2]=rotation[3]=0;

	pAnim = NULL;
}

cB3DChunk_NODE::~cB3DChunk_NODE()
{
	for (list<cB3DChunk_KEYS*>::iterator iter(lpKeys.begin()); iter!=lpKeys.end(); iter++)
		delete *iter;
	for (list<cB3DChunk_NODE*>::iterator iter(lpNodes.begin()); iter!=lpNodes.end(); iter++)
		delete *iter;
	if (pAnim) delete pAnim;
	if (pTypeChunk) delete pTypeChunk;
}

int  cB3DChunk_NODE::GetLength()
{
	int Length = cB3DChunk::GetLength()+
		(int)Name.length()+1+
		10*sizeof(float); // position, scale, rotquat
	if (pTypeChunk) Length += pTypeChunk->GetLength();

	for (list<cB3DChunk_KEYS*>::iterator iter(lpKeys.begin()); iter!=lpKeys.end(); iter++)
		Length += (*iter)->GetLength();

	for (list<cB3DChunk_NODE*>::iterator iter(lpNodes.begin()); iter!=lpNodes.end(); iter++)
		Length += (*iter)->GetLength();

	if (pAnim) Length += pAnim->GetLength();

	return Length;
}

void cB3DChunk_NODE::Export(FILE* file)
{
	cB3DChunk::Export(file);

	fwrite(Name.c_str(), 1, Name.length()+1, file);
	fwrite(&position, sizeof(float), 3, file);
	fwrite(&scale, sizeof(float), 3, file);
	fwrite(&rotation, sizeof(float), 4, file);

	if (pTypeChunk) pTypeChunk->Export(file);

	if (pAnim) pAnim->Export(file);

	for (list<cB3DChunk_KEYS*>::iterator iter(lpKeys.begin()); iter!=lpKeys.end(); iter++)
		(*iter)->Export(file);

	for (list<cB3DChunk_NODE*>::iterator iter(lpNodes.begin()); iter!=lpNodes.end(); iter++)
		(*iter)->Export(file);
}

/////////////////////////////////////////////////////////////////////////
//  VRTS
/////////////////////////////////////////////////////////////////////////
cB3DChunk_VRTS::cB3DChunk_VRTS()
{
	memcpy(Tag, "VRTS", 4);
}

int  cB3DChunk_VRTS::GetLength()
{
	int Length = cB3DChunk::GetLength()+
		3*sizeof(int); // flags, texcoord, tcoordcomponts, 
	if (!aVertices.empty())
		Length += ((int)aVertices.size())*(10*sizeof(float)+sizeof(float)*(int)aVertices[0].aTexcoordsU.size()*2);

	return Length;
}

void cB3DChunk_VRTS::Export(FILE* file)
{
	cB3DChunk::Export(file);
	int n;
	n = 3;		fwrite(&n, sizeof(int), 1, file);	// flags
	if (aVertices.empty()) n = 0; else n = (int)aVertices[0].aTexcoordsU.size();
	fwrite(&n, sizeof(int), 1, file); // tex_coord_sets
	n = 2;	fwrite(&n, sizeof(int), 1, file);	// num of texcoord components 2 = (u,v)

	for (size_t i(0); i<aVertices.size(); i++)
	{
		fwrite(&aVertices[i].x, sizeof(float), 1, file);
		fwrite(&aVertices[i].y, sizeof(float), 1, file);
		fwrite(&aVertices[i].z, sizeof(float), 1, file);

		fwrite(&aVertices[i].nx, sizeof(float), 1, file);
		fwrite(&aVertices[i].ny, sizeof(float), 1, file);
		fwrite(&aVertices[i].nz, sizeof(float), 1, file);

		fwrite(&aVertices[i].red, sizeof(float), 1, file);
		fwrite(&aVertices[i].green, sizeof(float), 1, file);
		fwrite(&aVertices[i].blue, sizeof(float), 1, file);
		fwrite(&aVertices[i].alpha, sizeof(float), 1, file);

		for (size_t j(0); j<aVertices[i].aTexcoordsU.size(); ++j)
		{
			fwrite(&aVertices[i].aTexcoordsU[j], sizeof(float), 1, file);
			fwrite(&aVertices[i].aTexcoordsV[j], sizeof(float), 1, file);
		}
	}
}

/////////////////////////////////////////////////////////////////////////
//  TRIS
/////////////////////////////////////////////////////////////////////////
cB3DChunk_TRIS::cB3DChunk_TRIS()
{
	memcpy(Tag, "TRIS", 4);
	pMaterial = NULL;
}

int  cB3DChunk_TRIS::GetLength()
{
	int Length = cB3DChunk::GetLength()+
		sizeof(int)+ // material
		3*sizeof(int)*(int)aTriangles.size();

	return Length;
}

void cB3DChunk_TRIS::Export(FILE* file)
{
	cB3DChunk::Export(file);

	int n;
	if (pMaterial)   n = pMaterial->Index;
	else n = -1;
	fwrite(&n, sizeof(int), 1, file);	// material index

	for (size_t i(0); i<aTriangles.size(); ++i)
		fwrite(aTriangles[i].aIndices, sizeof(int), 3, file);
}

/////////////////////////////////////////////////////////////////////////
//  MESH
/////////////////////////////////////////////////////////////////////////
cB3DChunk_MESH::cB3DChunk_MESH()
{
	memcpy(Tag, "MESH", 4);
	pMaterial = NULL;
}

int  cB3DChunk_MESH::GetLength()
{
	int Length = cB3DChunk::GetLength()+
		sizeof(int)+ // material
		Vertices.GetLength();
	for (size_t i(0); i<aTrianglesGroup.size(); ++i)
		Length += aTrianglesGroup[i].GetLength();

	return Length;
}

void cB3DChunk_MESH::Export(FILE* file)
{
	cB3DChunk::Export(file);

	int n;
	if (pMaterial)   n = pMaterial->Index;
	else n = -1;
	fwrite(&n, sizeof(int), 1, file);	// material index

	Vertices.Export(file);
	for (size_t i(0); i<aTrianglesGroup.size(); ++i)
		aTrianglesGroup[i].Export(file);
}

cB3DChunk_MESH::~cB3DChunk_MESH()
{
	if (pMaterial) delete pMaterial;
}

/////////////////////////////////////////////////////////////////////////
//  BONE
/////////////////////////////////////////////////////////////////////////
cB3DChunk_BONE::cB3DChunk_BONE()
{
	memcpy(Tag, "BONE", 4);
}

int  cB3DChunk_BONE::GetLength()
{
	int Length = cB3DChunk::GetLength()+
		(sizeof(int)+sizeof(float))*(int)aSkinInfo.size();

	return Length;
}

void cB3DChunk_BONE::Export(FILE* file)
{
	cB3DChunk::Export(file);

	for (size_t i(0); i<aSkinInfo.size(); ++i)
	{
		fwrite(&aSkinInfo[i].iVertex, sizeof(int), 1, file);
		fwrite(&aSkinInfo[i].weight, sizeof(float), 1, file);
	}
}

/////////////////////////////////////////////////////////////////////////
//  KEYS
/////////////////////////////////////////////////////////////////////////
cB3DChunk_KEYS::cB3DChunk_KEYS()
{
	memcpy(Tag, "KEYS", 4);
	Flags = 0;
}

int  cB3DChunk_KEYS::GetLength()
{
	int Length = cB3DChunk::GetLength()+
		sizeof(int); // flags;

	Length+= sizeof(int)*(int)aKeyFrames.size();
	if (Flags & 1) Length+= 3*sizeof(float)*(int)aKeyFrames.size();
	if (Flags & 2) Length+= 3*sizeof(float)*(int)aKeyFrames.size();
	if (Flags & 4) Length+= 4*sizeof(float)*(int)aKeyFrames.size();

	return Length;
}

void cB3DChunk_KEYS::Export(FILE* file)
{
	cB3DChunk::Export(file);

	fwrite(&Flags, sizeof(int), 1, file);

	for (size_t i(0); i<aKeyFrames.size(); ++i)
	{
		fwrite(&aKeyFrames[i].frame, sizeof(int), 1, file);
		if (Flags & 1){
			fwrite(&aKeyFrames[i].position[0], sizeof(float), 1, file);
			fwrite(&aKeyFrames[i].position[1], sizeof(float), 1, file);
			fwrite(&aKeyFrames[i].position[2], sizeof(float), 1, file);
		}
		if (Flags & 2){
			fwrite(&aKeyFrames[i].scale[0], sizeof(float), 1, file);
			fwrite(&aKeyFrames[i].scale[1], sizeof(float), 1, file);
			fwrite(&aKeyFrames[i].scale[2], sizeof(float), 1, file);
		}
		if (Flags & 4){
			fwrite(&aKeyFrames[i].rotation[0], sizeof(float), 1, file);
			fwrite(&aKeyFrames[i].rotation[1], sizeof(float), 1, file);
			fwrite(&aKeyFrames[i].rotation[2], sizeof(float), 1, file);
			fwrite(&aKeyFrames[i].rotation[3], sizeof(float), 1, file);
		}
	}
}

/////////////////////////////////////////////////////////////////////////
//  ANIM
/////////////////////////////////////////////////////////////////////////
cB3DChunk_ANIM::cB3DChunk_ANIM()
{
	memcpy(Tag, "ANIM", 4);
	fps = 60;
	nFrames = 0;
}

int  cB3DChunk_ANIM::GetLength()
{
	int Length = cB3DChunk::GetLength()+
		2*sizeof(int)+ // flags, nFrames
		sizeof(float); // fps

	return Length;
}

void cB3DChunk_ANIM::Export(FILE* file)
{
	cB3DChunk::Export(file);

	int n = 0;
	fwrite(&n, sizeof(int), 1, file);  // flags
	fwrite(&nFrames, sizeof(int), 1, file);
	fwrite(&fps, sizeof(float), 1, file);
}


//////////////////////////////////////////////////////////////////////////
// MISC
//////////////////////////////////////////////////////////////////////////

irr::core::stringc ToB3D_FindDestinationName(irr::core::stringc Source)
{
	const char Extension[] = { ".b3d" };

	FILE *pFile = NULL;
	irr::core::stringc DestCopy, Dest;

	char StrNumber[4];
	int Number = 0;
	
	DestCopy = Source.subString(0, Source.findLast('.'));
	Dest = DestCopy;

	DestCopy.append( Extension );
	fopen_s( &pFile, DestCopy.c_str(), "r");
	while ( pFile )
	{
		fclose(pFile);

		if (Number > 999) return "";

		DestCopy = Dest;
		_itoa_s(++Number, StrNumber, 10);
		
		if (Number <= 9)
		{
			char C = StrNumber[0];

			StrNumber[0] = '0';
			StrNumber[1] = C;
			StrNumber[2] = '\0';
		}

		DestCopy.append( StrNumber);
		DestCopy.append( Extension );

		fopen_s( &pFile, DestCopy.c_str(), "r");
	}

	return DestCopy;
}