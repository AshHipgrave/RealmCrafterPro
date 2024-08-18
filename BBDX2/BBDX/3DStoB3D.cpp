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
#pragma once 

#include <vector>
#include <math.h>

#include "ToB3D.h"
#include "ExportB3D.h"

using namespace std;

// Primary Chunk, at the beginning of each file
#define PRIMARY				0x4D4D
#define VERSION				0x0002				// This gives the version of the .3ds file
#define OBJECTINFO			0x3D3D				// This gives the version of the mesh and is found right before the material and object information
#define MATERIAL			0xAFFF				// This stored the texture info
#define MATNAME				0xA000				// This holds the material name
#define MATDIFFUSE			0xA020				// This holds the color of the object/material
#define MATMAP				0xA200				// This is a header for a new material
#define MATMAPFILE			0xA300				// This holds the file name of the texture

#define MAP_USCALE			0xA354				
#define MAP_VSCALE			0xA356				
#define MAP_UOFFSET			0xA358
#define MAP_VOFFSET			0xA35A


#define OBJECT				0x4000				// This stores the faces, vertices, etc...
#define OBJECT_MESH			0x4100				// This lets us know that we are reading a new object
#define OBJECT_VERTICES     0x4110			// The objects vertices
#define OBJECT_FACES		0x4120			// The objects faces
#define OBJECT_MATERIAL		0x4130			// This is found if the object has a material, either texture map or color
#define OBJECT_UV			0x4140			// The UV texture coordinates

unsigned int Buffer[50000];
char Name[64];

struct cVertex
{
	float X, Y, Z;

	cVertex(float X = 0, float Y = 0, float Z = 0) : X(X), Y(Y), Z(Z) {}
};

struct cVector3f
{
	float X, Y, Z;

	cVector3f(float X = 0, float Y = 0, float Z = 0) : X(X), Y(Y), Z(Z) {}
	cVector3f(cVertex A) : X(A.X), Y(A.Y), Z(A.Z) {}

	cVector3f operator-(cVector3f V)
	{
		return cVector3f(X - V.X, Y - V.Y, Z - V.Z);
	}

	void operator/=(float  S)
	{
		X /= S;
		Y /= S;
		Z /= S;
	}

	void operator+=(cVector3f V)
	{
		X += V.X;
		Y += V.Y;
		Z += V.Z;
	}

	void operator=(cVector3f V)
	{
		X = V.X;
		Y = V.Y;
		Z = V.Z;
	}

	float Magnitude()
	{
		return (float)sqrt(X * X + Y * Y + Z * Z);
	}

	void Normalize()
	{
		float Magnitud = Magnitude();

		X /= Magnitud;
		Y /= Magnitud;
		Z /= Magnitud;
	}

	void Reset()
	{
		X = Y = Z = 0;
	}


	void CrossProduct(cVector3f V)
	{
		cVector3f Temp = *this;

		X = Temp.Y * V.Z - Temp.Z * V.Y;
		Y = Temp.Z * V.X - Temp.X * V.Z;
		Z = Temp.X * V.Y - Temp.Y * V.X;
	}

};

struct cChunk
{
	unsigned short ID;
	unsigned int Length;
	unsigned int BytesRead;
};

struct cTexCoord
{
	float U, V;
	float U1, V1;
};

typedef cVertex cNormal;

struct cFace
{
	unsigned int VertexIndex[3];
};

struct cMaterial
{
	string MaterialName;
	unsigned char Diffuse[3];

	float U_Tiling, V_Tiling;
	float U_Offset, V_Offset;

	string TextureName;

	cMaterial()
	{
		U_Tiling = V_Tiling = 1;
		U_Offset = 0;
		V_Offset = 0;
	}
};

struct cMeshGroup
{
	vector<unsigned short> aFaces;
	int MaterialID;
};

struct cMesh 
{
	string Name; 

	vector<cVertex *> apVertexs;
	vector<cNormal *> apNormals;
	vector<cTexCoord *> apTexCoords;
	vector<cFace *> apFaces; 

	bool HasTexture;
	vector<cMeshGroup *> apMeshGroups;

	~cMesh()
	{
		for (vector<cVertex *>::iterator it = apVertexs.begin(); it != apVertexs.end(); ++it)
			delete *it;

		for (vector<cNormal *>::iterator it = apNormals.begin(); it != apNormals.end(); ++it)
			delete *it;

		for (vector<cTexCoord *>::iterator it = apTexCoords.begin(); it != apTexCoords.end(); ++it)
			delete *it;

		for (vector<cFace *>::iterator it = apFaces.begin(); it != apFaces.end(); ++it)
			delete *it;

		for (vector<cMeshGroup *>::iterator it = apMeshGroups.begin(); it != apMeshGroups.end(); ++it)
			delete *it;
	}
};

class c3DS
{
	public:
		vector<cMesh *> apMeshs;
		vector<cMaterial *> apMaterials;

		c3DS()
		{
			m_pCurrentChunk = new cChunk;
			m_pTempChunk = new cChunk;
		}

		~c3DS()
		{
			for (vector<cMesh *>::iterator it = apMeshs.begin(); it != apMeshs.end(); ++it)
				delete *it;

			for (vector<cMaterial *>::iterator it = apMaterials.begin(); it != apMaterials.end(); ++it)
				delete *it;

			if (m_pCurrentChunk)
				delete m_pCurrentChunk;

			if (m_pTempChunk)
				delete m_pTempChunk;
		}

		bool Load3DS( irr::core::stringc Name, bool bAutoGenerateNormals = true )
		{
			fopen_s(&m_pFile, Name.c_str(), "rb");
			if ( !m_pFile )	return false;

			ReadChunk(m_pCurrentChunk);
			if (m_pCurrentChunk->ID != PRIMARY)
			{
				MessageBox(0, "Unable to load PRIMARY chunk.\n", "3DS to B3D Converter", MB_ICONERROR);
				fclose(m_pFile);

				return false;
			}

			if ( !ProcessNextChunk(m_pCurrentChunk) )
			{
				fclose(m_pFile);
				return false;
			}

			if (bAutoGenerateNormals) ComputeNormals();

			fclose(m_pFile);
			return true;
		}

	private:
		FILE *m_pFile;
		cChunk *m_pCurrentChunk, *m_pTempChunk;

		void ReadChunk(cChunk *pChunk)
		{
			pChunk->BytesRead = (unsigned int)fread(&pChunk->ID, 1, 2, m_pFile);			
			pChunk->BytesRead += (unsigned int)fread(&pChunk->Length, 1, 4, m_pFile); 
		}

		int GetString(char *pBuffer)
		{
			int Index = 0;

			fread(pBuffer, 1, 1, m_pFile);
			while ( *(pBuffer + Index++) != 0 )
				fread(pBuffer + Index, 1, 1, m_pFile);

			return Index;
		}

		void ReadVertices(cMesh *pMesh, cChunk *pChunk)
		{
			int CountVertexs = 0;
			pChunk->BytesRead += (unsigned int)fread(&CountVertexs, 1, 2, m_pFile);

			for (int i(0); i < CountVertexs; ++i)
			{
				pMesh->apVertexs.push_back(new cVertex);
				pChunk->BytesRead += (unsigned int)fread(pMesh->apVertexs.back(), 1, sizeof(cVertex), m_pFile);

				float Temp;
				Temp = pMesh->apVertexs.back()->Y;
				pMesh->apVertexs.back()->Y = pMesh->apVertexs.back()->Z;
				pMesh->apVertexs.back()->Z = Temp;
			}
		}

		void ReadFaces(cMesh *pMesh, cChunk *pChunk)
		{
			unsigned short Index = 0;

			int CountFaces = 0;
			pChunk->BytesRead += (unsigned int)fread(&CountFaces, 1, 2, m_pFile);

			for (int i(0); i < CountFaces; i++)
			{
				pMesh->apFaces.push_back(new cFace);
				for (int j(0); j < 4; j++)
				{
					pChunk->BytesRead += (unsigned int)fread(&Index, 1, sizeof(unsigned short), m_pFile);

					if (j < 3)
						pMesh->apFaces.back()->VertexIndex[j] = Index;
				}
			}
		}

		void ReadTexCoord(cMesh *pMesh, cChunk *pChunk)
		{
			int CountTexCoord = 0;
			pChunk->BytesRead += (unsigned int)fread(&CountTexCoord, 1, 2, m_pFile);

			for (int i(0); i < CountTexCoord; ++i)
			{
				pMesh->apTexCoords.push_back(new cTexCoord);
			pChunk->BytesRead += (unsigned int)fread(pMesh->apTexCoords.back(), 1, 8, m_pFile);
			}
		}

		void ReadObjectMaterial(cMesh *pMesh, cChunk *pChunk)
		{
			pChunk->BytesRead += GetString(Name);
			cMeshGroup* pGroup = new cMeshGroup;
			pMesh->apMeshGroups.push_back(pGroup);

			pGroup->MaterialID = -1;
			for (size_t i = 0; i < apMaterials.size(); i++)
				if (strcmp(Name, apMaterials[i]->MaterialName.c_str()) == 0 ) 
				{
					pGroup->MaterialID = (int)i;

					if (strlen(apMaterials[i]->TextureName.c_str()) > 0)
						pMesh->HasTexture = true;
					else
						pMesh->HasTexture = false;

					break;
				}

			unsigned short nFaces = 0;
			pChunk->BytesRead += (unsigned int)fread(&nFaces, 1, 2, m_pFile);

			pGroup->aFaces.resize( nFaces );
			
			// JB: nFaces appears to be 0 on some meshes. Skip reading.
			if(nFaces > 0)
				pChunk->BytesRead += (unsigned int)fread(&pGroup->aFaces[0], 1, 2*nFaces, m_pFile);
		}

		bool ProcessNextChunk(cChunk *pChunk)
		{
			unsigned int Version;
			m_pCurrentChunk = new cChunk;

			while (pChunk->BytesRead < pChunk->Length)
			{
				ReadChunk(m_pCurrentChunk);
				switch ( m_pCurrentChunk->ID )
				{
					case VERSION:
						m_pCurrentChunk->BytesRead += (unsigned int)fread(&Version, 1, sizeof(unsigned int), m_pFile);
						if (Version > 0x03)
						{
							MessageBox(0, "The 3ds file is over version 3.\n", "3DS to B3D Converter", MB_ICONERROR);
							return false;
						}
						break;
					case OBJECTINFO:
						ReadChunk(m_pTempChunk);
						m_pTempChunk->BytesRead += (unsigned int)fread(&Buffer, 1,
						m_pTempChunk->Length - m_pTempChunk->BytesRead, m_pFile);
						m_pCurrentChunk->BytesRead += m_pTempChunk->BytesRead; 
						ProcessNextChunk(m_pCurrentChunk);
						break;
					case OBJECT:
						apMeshs.push_back(new cMesh);
						m_pCurrentChunk->BytesRead += GetString( Name );
						apMeshs.back()->Name.assign( Name );
						ProcessNextObjectChuck(apMeshs.back(), m_pCurrentChunk);
						break;
					case MATERIAL:
						apMaterials.push_back(new cMaterial);
						ProcessNextMaterialChunk(apMaterials.back(), m_pCurrentChunk);
						break;
					default:
						m_pCurrentChunk->BytesRead += (unsigned int)fread(&Buffer, 1,
						m_pCurrentChunk->Length - m_pCurrentChunk->BytesRead, m_pFile);
				}

				pChunk->BytesRead += m_pCurrentChunk->BytesRead;
			}

			if (m_pCurrentChunk) delete m_pCurrentChunk;
			m_pCurrentChunk = pChunk;
			return true;
		}

		void ProcessNextObjectChuck(cMesh *pMesh, cChunk *pChunk)
		{
			m_pCurrentChunk = new cChunk;
			while (pChunk->BytesRead < pChunk->Length)
			{
				ReadChunk(m_pCurrentChunk);
				switch (m_pCurrentChunk->ID)
				{
					case OBJECT_MESH: 
						ProcessNextObjectChuck(pMesh, m_pCurrentChunk);
						break;

					case OBJECT_VERTICES:
						ReadVertices(pMesh, m_pCurrentChunk);
						break;

					case OBJECT_FACES:
						ReadFaces(pMesh, m_pCurrentChunk);
						break;

					case OBJECT_UV:
						ReadTexCoord(pMesh, m_pCurrentChunk);
						break;

					case OBJECT_MATERIAL:
						ReadObjectMaterial(pMesh, m_pCurrentChunk);
						break; 

					default:
						m_pCurrentChunk->BytesRead += (unsigned int)fread(&Buffer, 1, 
							m_pCurrentChunk->Length - m_pCurrentChunk->BytesRead, m_pFile);
				};

				pChunk->BytesRead += m_pCurrentChunk->BytesRead;
			}

			delete m_pCurrentChunk;
			m_pCurrentChunk = pChunk;
		}

		void ProcessNextMaterialChunk(cMaterial *pMaterial, cChunk *pChunk)
		{
			m_pCurrentChunk = new cChunk;
			while (pChunk->BytesRead < pChunk->Length)
			{
				ReadChunk(m_pCurrentChunk);

				switch (m_pCurrentChunk->ID)
				{
					case MATNAME:
						m_pCurrentChunk->BytesRead += (unsigned int)fread(&Name, 1, 
							m_pCurrentChunk->Length - m_pCurrentChunk->BytesRead, m_pFile);		

						pMaterial->MaterialName.assign( Name );
						break;

					case MATDIFFUSE:
						ReadMaterialColors(pMaterial, m_pCurrentChunk);
						break;

					case MATMAP:
						ProcessNextMaterialChunk(pMaterial, m_pCurrentChunk);
						break;
			case MAP_USCALE:
				m_pCurrentChunk->BytesRead += 
					(unsigned int)fread(&pMaterial->U_Tiling, 1, 4, m_pFile);
				break;

			case MAP_VSCALE:
				m_pCurrentChunk->BytesRead += 
					(unsigned int)fread(&pMaterial->V_Tiling, 1, 4, m_pFile);
				break;

			case MAP_UOFFSET:
				m_pCurrentChunk->BytesRead += 
					(unsigned int)fread(&pMaterial->U_Offset, 1, 4, m_pFile);
				break;

			case MAP_VOFFSET:
				m_pCurrentChunk->BytesRead += 
					(unsigned int)fread(&pMaterial->V_Offset, 1, 4, m_pFile);
				break;

					case MATMAPFILE:
						pChunk->BytesRead += (unsigned int)fread(&Name, 1,
							m_pCurrentChunk->Length - m_pCurrentChunk->BytesRead, m_pFile);

						pMaterial->TextureName.assign( Name );
						break; 

					default:
						m_pCurrentChunk->BytesRead += (unsigned int)fread(&Buffer, 1, 
							m_pCurrentChunk->Length - m_pCurrentChunk->BytesRead, m_pFile);
				};

				pChunk->BytesRead += m_pCurrentChunk->BytesRead;
			}

			delete m_pCurrentChunk;
			m_pCurrentChunk = pChunk;

		}

		void ReadMaterialColors(cMaterial *pMaterial, cChunk *pChunk)
		{
			ReadChunk(m_pTempChunk);

			m_pTempChunk->BytesRead += (unsigned int)fread(&pMaterial->Diffuse, 1, 3, m_pFile);
			pChunk->BytesRead += m_pTempChunk->BytesRead;
		}

		void ComputeNormals()
		{
			vector<cVector3f> aNormals;
			cVector3f A, B, L, R;
			cMesh *pMesh;
			cFace *pFace;

			for (size_t i(0); i < apMeshs.size(); ++i)
			{
				pMesh = apMeshs[i];
				aNormals.resize(pMesh->apFaces.size());
				for (size_t j(0); j < pMesh->apFaces.size(); ++j) 
				{
					pFace = pMesh->apFaces[j];

					L = *pMesh->apVertexs[ pFace->VertexIndex[0] ];
					R = *pMesh->apVertexs[ pFace->VertexIndex[2] ];
					A = L - R; 
	
					L = *pMesh->apVertexs[ pFace->VertexIndex[2] ];
					R = *pMesh->apVertexs[ pFace->VertexIndex[1] ];
					B = L - R; 

					A.CrossProduct(B);
					aNormals[j] = A;
				}

				cVector3f SumNormals;
				int Shared = 0;

				for (size_t k (0); k < pMesh->apVertexs.size(); ++k)
				{
					for (size_t t(0); t < pMesh->apFaces.size(); ++t)
					{
						pFace = pMesh->apFaces[t];
						if (pFace->VertexIndex[0] == k || pFace->VertexIndex[1] == k ||
							pFace->VertexIndex[2] == k)
						{
							SumNormals += aNormals[t];
							Shared++;
						}
					}

					SumNormals /= (float)Shared;
					SumNormals.Normalize();

					pMesh->apNormals.push_back( new cVertex(SumNormals.X, SumNormals.Y, SumNormals.Z) );
					
					SumNormals.Reset();
					Shared = 0;
				}
			} 
		}
};


bool Convert3DStoB3D_Custom(irr::core::stringc _3DSFile, irr::core::stringc B3DFile, bool bAutoGenerateNormals )
{
	cB3DChunk_BB3D bb3d;
	c3DS *p3ds = new c3DS;

	if ( !p3ds->Load3DS( _3DSFile, bAutoGenerateNormals) )
		return false;

	for (size_t i = 0; i < p3ds->apMaterials.size() ; ++i)
	{
		cMaterial *pMaterial = p3ds->apMaterials[i];
		cB3DChunk_BRUS *pBrus = new cB3DChunk_BRUS;

		bb3d.InsertMaterial(pBrus);
		pBrus->MaterialName = pMaterial->MaterialName;
		pBrus->red = pMaterial->Diffuse[0]/(float)255;
		pBrus->green = pMaterial->Diffuse[1]/(float)255;
		pBrus->blue = pMaterial->Diffuse[2]/(float)255;

		if ( !pMaterial->TextureName.empty() )
		{
			cB3DChunk_TEXS *pText = new cB3DChunk_TEXS;
			bb3d.InsertTexture(pText);
			pText->FileName = pMaterial->TextureName;

			pBrus->lpTextures.push_back(pText);
		}
	}

	for (size_t i(0); i < p3ds->apMeshs.size(); ++i)
	{
		cB3DChunk_NODE *pNode = new cB3DChunk_NODE;

		cB3DChunk_MESH* pB3DMesh = new cB3DChunk_MESH;
		cMesh *pMesh = p3ds->apMeshs[i];

		cB3DChunk_VRTS* pVertexs = &pB3DMesh->Vertices;
		pVertexs->aVertices.resize(pMesh->apVertexs.size());				
		for (size_t j = 0; j < pMesh->apVertexs.size() ; j++)
		{
			pVertexs->aVertices[j].x = pMesh->apVertexs[j]->X;
			pVertexs->aVertices[j].y = pMesh->apVertexs[j]->Y;
			pVertexs->aVertices[j].z = pMesh->apVertexs[j]->Z; 

			if (!pMesh->apNormals.empty())
			{
				pVertexs->aVertices[j].nx = pMesh->apNormals[j]->X;
				pVertexs->aVertices[j].ny = pMesh->apNormals[j]->Y;
				pVertexs->aVertices[j].nz = pMesh->apNormals[j]->Z;
			}

			pVertexs->aVertices[j].red = 1.0f;
			pVertexs->aVertices[j].green = 1.0f;
			pVertexs->aVertices[j].blue = 1.0f;
			pVertexs->aVertices[j].alpha = 1.0f;
		}

		if (pMesh->apMeshGroups.empty())
		{
			pB3DMesh->aTrianglesGroup.resize(1);
			pB3DMesh->aTrianglesGroup[0].aTriangles.resize(pMesh->apFaces.size());
			for (size_t i(0); i < pMesh->apFaces.size(); ++i)
			{
				for (int t(0); t < 3; t++)
					pB3DMesh->aTrianglesGroup[0].aTriangles[i].aIndices[2-t] = pMesh->apFaces[i]->VertexIndex[t];
			}
			
		}
		else
		{
			pB3DMesh->aTrianglesGroup.resize(pMesh->apMeshGroups.size());
			for (size_t i(0); i < pB3DMesh->aTrianglesGroup.size(); ++i)
			{
				pB3DMesh->aTrianglesGroup[i].aTriangles.resize(pMesh->apMeshGroups[i]->aFaces.size());

				for (size_t k(0); k < pB3DMesh->aTrianglesGroup[i].aTriangles.size(); ++k)
				{
					for (int t(0); t < 3; t++)
						pB3DMesh->aTrianglesGroup[i].aTriangles[k].aIndices[2-t] =
							pMesh->apFaces[ pMesh->apMeshGroups[i]->aFaces[k] ]->VertexIndex[t];

					int MaterialPos = 0;
					pB3DMesh->aTrianglesGroup[i].pMaterial = NULL;
					for (list<cB3DChunk_BRUS*>::iterator iter(bb3d.lpMaterials.begin()); iter != bb3d.lpMaterials.end(); iter++,MaterialPos++)
						if (MaterialPos == pMesh->apMeshGroups[i]->MaterialID)
						{
							pB3DMesh->aTrianglesGroup[i].pMaterial = *iter;
							break;
						}				

					int iMat = pMesh->apMeshGroups[i]->MaterialID;
					for (int t(0); t < 3; t++)
					{
						int v = pMesh->apFaces[ pMesh->apMeshGroups[i]->aFaces[k] ]->VertexIndex[t];
						pB3DMesh->aTrianglesGroup[i].aTriangles[k].aIndices[2-t] = v;

						if (pB3DMesh->aTrianglesGroup[i].pMaterial)
						{
							pMesh->apTexCoords[v]->U1 = pMesh->apTexCoords[v]->U * p3ds->apMaterials[iMat]->U_Tiling + p3ds->apMaterials[iMat]->U_Offset;
							pMesh->apTexCoords[v]->V1 = pMesh->apTexCoords[v]->V * p3ds->apMaterials[iMat]->V_Tiling + p3ds->apMaterials[iMat]->V_Offset;
						} 
					}

				}
			}
		}

		for (size_t j = 0; j < pMesh->apTexCoords.size() ; j++)
		{
			pVertexs->aVertices[j].aTexcoordsU.push_back(pMesh->apTexCoords[j]->U1);
			pVertexs->aVertices[j].aTexcoordsV.push_back(-pMesh->apTexCoords[j]->V1);
		}

		pNode->pTypeChunk = pB3DMesh;
		pNode->Name = pMesh->Name;

		bb3d.lpNodes.push_back(pNode);
	}

	bb3d.ExportToFile( (char *)B3DFile.c_str() );

	delete p3ds;
	return true;
}

irr::core::stringc ToB3D_FindDestinationName(irr::core::stringc Source);

irr::core::stringc Convert3DStoB3D(irr::core::stringc Source, bool bAutoGenerateNormals)
{
	irr::core::stringc Dest = ToB3D_FindDestinationName(Source);

	if (Dest.size() == 0) return "";

	return (Convert3DStoB3D_Custom(Source, Dest, bAutoGenerateNormals))?Dest:"";
}
