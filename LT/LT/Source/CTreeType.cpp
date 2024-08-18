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
//COPYRIGHTNOTICE

#include <algorithm>

#include "CTreeType.h"

#include "STreeTrunkVertex.h"
#include "STreeLeafVertex.h"

#include <windows.h>
#include <d3dx9.h>

namespace LT
{
	CTreeType::CTreeType(ITreeManager* manager, std::string &name)
		: Manager(manager), Name(name)
	{
		CollisionData = 0;
		NameLower = name;
		std::transform(NameLower.begin(), NameLower.end(), NameLower.begin(), ::tolower);
	}

	CTreeType::~CTreeType()
	{
		Unload();
	}

	void CTreeType::Unload()
	{
		for(int i = 0; i < TrunkSurfaces.size(); ++i)
		{
			Manager->GetRenderer()->FreeMeshBuffer(TrunkSurfaces[i]->Buffer);
			Manager->GetRenderer()->FreeTexture(TrunkSurfaces[i]->DiffuseMap);
			Manager->GetRenderer()->FreeTexture(TrunkSurfaces[i]->NormalMap);
			Manager->GetRenderer()->FreeTexture(TrunkSurfaces[i]->MaskMap);
			delete TrunkSurfaces[i];
		}

		for(int i = 0; i < LeafSurfaces.size(); ++i)
		{
			Manager->GetRenderer()->FreeMeshBuffer(LeafSurfaces[i]->Buffer);
			Manager->GetRenderer()->FreeTexture(LeafSurfaces[i]->DiffuseMap);
			Manager->GetRenderer()->FreeTexture(LeafSurfaces[i]->NormalMap);
			Manager->GetRenderer()->FreeTexture(LeafSurfaces[i]->MaskMap);
			delete LeafSurfaces[i];
		}

		for(int i = 0; i < LODTextures.size(); ++i)
		{
			Manager->GetRenderer()->FreeTexture(LODTextures[i]);
		}

		if(CollisionData != 0)
		{
			if(CollisionData->Instances != 0)
				delete[] CollisionData->Instances;
			delete CollisionData;
			CollisionData = 0;
		}

		TrunkSurfaces.clear();
		LeafSurfaces.clear();
		LODTextures.clear();
	}

	const STreeCollisionData* CTreeType::GetCollisionData()
	{
		return CollisionData;
	}

	const char* CTreeType::GetName() const
	{
		return Name.c_str();
	}

	bool CTreeType::IsType(const char* inameLower) const
	{
		std::string nameLower = inameLower;

		if(NameLower.compare(nameLower) == 0)
			return true;
		return false;
	}

	inline std::string ReadString(FILE* file)
	{
		int Length = 0;
		fread(&Length, 4, 1, file);

		char* Str = new char[Length + 1];
		fread(Str, 1, Length, file);
		Str[Length] = 0;

		std::string out = Str;
		delete[] Str;

		return out;
	}

	inline std::string GetPath(const std::string& path)
	{
		const char* PathData = path.c_str();

		for(int i = path.length() - 1; i >= 0; --i)
		{
			if(PathData[i] == '\\' || PathData[i] == '/')
			{
				return path.substr(0, i + 1);
			}
		}

		return "";
	}

	template<class T> inline void RecalculateNormals(T* vertices, int vertexCount, unsigned short* indices, int indexCount)
	{
		for(int i = 0; i < vertexCount; ++i)
		{
			vertices[i].NormalX = 0.0f;
			vertices[i].NormalY = 0.0f;
			vertices[i].NormalZ = 0.0f;
		}

		for(int i = 0; i < indexCount; i+=3)
		{
			D3DXPLANE Plane;
			D3DXVECTOR3 P0(vertices[indices[i+0]].PositionX, vertices[indices[i+0]].PositionY, vertices[indices[i+0]].PositionZ);
			D3DXVECTOR3 P1(vertices[indices[i+1]].PositionX, vertices[indices[i+1]].PositionY, vertices[indices[i+1]].PositionZ);
			D3DXVECTOR3 P2(vertices[indices[i+2]].PositionX, vertices[indices[i+2]].PositionY, vertices[indices[i+2]].PositionZ);
			D3DXPlaneFromPoints(&Plane, &P0, &P1, &P2);

			vertices[indices[i+0]].NormalX += Plane.a;
			vertices[indices[i+0]].NormalY += Plane.b;
			vertices[indices[i+0]].NormalZ += Plane.c;

			vertices[indices[i+1]].NormalX += Plane.a;
			vertices[indices[i+1]].NormalY += Plane.b;
			vertices[indices[i+1]].NormalZ += Plane.c;

			vertices[indices[i+2]].NormalX += Plane.a;
			vertices[indices[i+2]].NormalY += Plane.b;
			vertices[indices[i+2]].NormalZ += Plane.c;
		}

		for(int i = 0; i < vertexCount; ++i)
		{
			NGin::Math::Vector3 TNorm(vertices[i].NormalX,
				vertices[i].NormalY,
				vertices[i].NormalZ);

			TNorm.Normalize();

			vertices[i].NormalX = TNorm.X;
			vertices[i].NormalY = TNorm.Y;
			vertices[i].NormalZ = TNorm.Z;
		}
	}

	inline void Normalize(float& x, float& y, float& z)
	{
		NGin::Math::Vector3 Norm(x, y, z);
		Norm.Normalize();
		x = Norm.X;
		y = Norm.Y;
		z = Norm.Z;
	}

	template<class T> inline void CalculateVertexTangent(
		T &vertex,
		NGin::Math::Vector3 &pos1, NGin::Math::Vector3 &pos2, NGin::Math::Vector3 &pos3,
		NGin::Math::Vector2 &coord1, NGin::Math::Vector2 &coord2, NGin::Math::Vector2 &coord3)
	{
		// Calculate normal
		NGin::Math::Vector3 Vec1 = pos1 - pos2;
		NGin::Math::Vector3 Vec2 = pos3 - pos1;
		NGin::Math::Vector3 TNorm = Vec2.Cross(Vec1);
		vertex.NormalX = TNorm.X;
		vertex.NormalY = TNorm.Y;
		vertex.NormalZ = TNorm.Z;

		// BiNormal
		float DX1 = coord1.X - coord2.X;
		float DX2 = coord3.X - coord1.X;
		NGin::Math::Vector3 TBiNormal = (Vec1 * DX2) - (Vec2 * DX1);
		vertex.BiNormalX = TBiNormal.X;
		vertex.BiNormalY = TBiNormal.Y;
		vertex.BiNormalZ = TBiNormal.Z;

		// Tangent
		float DY1 = coord1.Y - coord2.Y;
		float DY2 = coord3.Y - coord1.Y;
		NGin::Math::Vector3 TTangent = (Vec1 * DY2) - (Vec2 * DY1);
		vertex.TangentX = TTangent.X;
		vertex.TangentY = TTangent.Y;
		vertex.TangentZ = TTangent.Z;

		// Normalize
		Normalize(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
		Normalize(vertex.BiNormalX, vertex.BiNormalY, vertex.BiNormalZ);
		Normalize(vertex.TangentX, vertex.TangentY, vertex.TangentZ);

		// Correction (if normal is facing the opposite direction)
		NGin::Math::Vector3 TangentToBiNormal = NGin::Math::Vector3(vertex.TangentX, vertex.TangentY, vertex.TangentZ).Cross(NGin::Math::Vector3(vertex.BiNormalX, vertex.BiNormalY, vertex.BiNormalZ));
		if(TangentToBiNormal.Dot(NGin::Math::Vector3(vertex.NormalX, vertex.NormalY, vertex.NormalZ)) < 0.0f)
		{
			vertex.TangentX *= -1.0f;
			vertex.TangentY *= -1.0f;
			vertex.TangentZ *= -1.0f;
			vertex.BiNormalX *= -1.0f;
			vertex.BiNormalY *= -1.0f;
			vertex.BiNormalZ *= -1.0f;
		}
	}

	template<class T> inline void RecalculateTangents(T* vertices, int vertexCount, unsigned short* indices, int indexCount)
	{
		for(int i = 0; i < indexCount; i += 3)
		{
			CalculateVertexTangent<T>(
				vertices[indices[i + 0]],
				NGin::Math::Vector3(vertices[indices[i + 0]].PositionX, vertices[indices[i + 0]].PositionY, vertices[indices[i + 0]].PositionZ),
				NGin::Math::Vector3(vertices[indices[i + 1]].PositionX, vertices[indices[i + 1]].PositionY, vertices[indices[i + 1]].PositionZ),
				NGin::Math::Vector3(vertices[indices[i + 2]].PositionX, vertices[indices[i + 2]].PositionY, vertices[indices[i + 2]].PositionZ),
				NGin::Math::Vector2(vertices[indices[i + 0]].TexCoordU, vertices[indices[i + 0]].TexCoordV),
				NGin::Math::Vector2(vertices[indices[i + 1]].TexCoordU, vertices[indices[i + 1]].TexCoordV),
				NGin::Math::Vector2(vertices[indices[i + 2]].TexCoordU, vertices[indices[i + 2]].TexCoordV));

			CalculateVertexTangent<T>(
				vertices[indices[i + 1]],
				NGin::Math::Vector3(vertices[indices[i + 1]].PositionX, vertices[indices[i + 1]].PositionY, vertices[indices[i + 1]].PositionZ),
				NGin::Math::Vector3(vertices[indices[i + 2]].PositionX, vertices[indices[i + 2]].PositionY, vertices[indices[i + 2]].PositionZ),
				NGin::Math::Vector3(vertices[indices[i + 0]].PositionX, vertices[indices[i + 0]].PositionY, vertices[indices[i + 0]].PositionZ),
				NGin::Math::Vector2(vertices[indices[i + 1]].TexCoordU, vertices[indices[i + 1]].TexCoordV),
				NGin::Math::Vector2(vertices[indices[i + 2]].TexCoordU, vertices[indices[i + 2]].TexCoordV),
				NGin::Math::Vector2(vertices[indices[i + 0]].TexCoordU, vertices[indices[i + 0]].TexCoordV));

			CalculateVertexTangent<T>(
				vertices[indices[i + 2]],
				NGin::Math::Vector3(vertices[indices[i + 2]].PositionX, vertices[indices[i + 2]].PositionY, vertices[indices[i + 2]].PositionZ),
				NGin::Math::Vector3(vertices[indices[i + 0]].PositionX, vertices[indices[i + 0]].PositionY, vertices[indices[i + 0]].PositionZ),
				NGin::Math::Vector3(vertices[indices[i + 1]].PositionX, vertices[indices[i + 1]].PositionY, vertices[indices[i + 1]].PositionZ),
				NGin::Math::Vector2(vertices[indices[i + 2]].TexCoordU, vertices[indices[i + 2]].TexCoordV),
				NGin::Math::Vector2(vertices[indices[i + 0]].TexCoordU, vertices[indices[i + 0]].TexCoordV),
				NGin::Math::Vector2(vertices[indices[i + 1]].TexCoordU, vertices[indices[i + 1]].TexCoordV));
		}
	}

	ITreeType* CTreeType::LoadTreeType(ITreeManager* manager, FILE* file, const std::string &path, CTreeType* treeType)
	{
		unsigned char FileVersion[2] = {0, 0};
		std::string TreeName;
		int TrunkCount = 0, LeafCount = 0;
		unsigned int VertexCount = 0, IndexCount = 0;
		std::string DiffuseName, NormalName, MaskName;
		NGin::Math::AABB BoundingBox;
		NGin::Math::Vector3 SwayCenter;

		std::vector<STreeBuffer*> Trunks;
		std::vector<STreeBuffer*> Leaves;
		std::vector<ITreeTexture*> LODTextures;

		char DBO[1024];

		fseek(file, 0, SEEK_SET);

		// Header (Version and Name)
		fread(FileVersion, 1, 2, file);
		TreeName = ReadString(file);

		// First format - revision has tree sway
		if(FileVersion[0] == 1 && FileVersion[1] == 0)
		{
			MessageBoxA(0, "Error: Tree type cannot be loaded as format LT1.0 is now invalid!", "CTreeType::LoadTreeType()", 0);
			return 0;
		}
		if(FileVersion[0] == 1 && FileVersion[1] == 1)
		{
			MessageBoxA(0, "Error: Tree type cannot be loaded as format LT1.1 is now invalid!", "CTreeType::LoadTreeType()", 0);
			return 0;
		}

		// Quick Verify
		if(FileVersion[0] != 1 || FileVersion[1] != 2)
			return 0;

		// Read sway center
		fread(&SwayCenter, sizeof(NGin::Math::Vector3), 1, file);

		// Read collision data
		unsigned int CollisionCount = 0;
		NGin::Math::Vector3 BodyPos, BodySize;
		char CollisionType;
		STreeCollisionData* CollisionData = new STreeCollisionData();

		fread(&CollisionCount, 4, 1, file);
		if(CollisionCount > 0)
		{
			CollisionData->InstanceCount = CollisionCount;
			CollisionData->Instances = new STreeCollisionData::DataInstance[CollisionCount];

			for(unsigned int c = 0; c < CollisionCount; ++c)
			{
				fread(&CollisionType, 1, 1, file);
				fread(&BodyPos, sizeof(NGin::Math::Vector3), 1, file);
				fread(&BodySize, sizeof(NGin::Math::Vector3), 1, file);

				CollisionData->Instances[c].Type = (ETreeCollisionType)(int)CollisionType;
				CollisionData->Instances[c].Offset = BodyPos;
				CollisionData->Instances[c].Dimensions = BodySize;
			}
		}else
		{
			CollisionData->Instances = 0;
			CollisionData->InstanceCount = 0;
		}

		// Read LODS
		std::string TexPath = GetPath(path);

		unsigned char cLODCount = 0;
		fread(&cLODCount, 1, 1, file);
		for(int i = 0; i < cLODCount; ++i)
		{
			std::string TexFile = ReadString(file);
			ITreeTexture* Tex = manager->GetRenderer()->GetTexture((TexPath + TexFile).c_str(), true);
			if(Tex != 0)
				LODTextures.push_back(Tex);
		}

		// Trunk data
		fread(&TrunkCount, 4, 1, file);

		for(int Trunk = 0; Trunk < TrunkCount; ++Trunk)
		{
			DiffuseName = ReadString(file);
			NormalName = ReadString(file);
			
			fread(&VertexCount, 4, 1, file);
			STreeTrunkVertex* TrunkVertices = new STreeTrunkVertex[VertexCount];

			for(unsigned int v = 0; v < VertexCount; ++v)
			{
				float X, Y, Z, U, V;

				fread(&X, 4, 1, file);
				fread(&Y, 4, 1, file);
				fread(&Z, 4, 1, file);
				fread(&U, 4, 1, file);
				fread(&V, 4, 1, file);

				TrunkVertices[v].PositionX = X;
				TrunkVertices[v].PositionY = Y;
				TrunkVertices[v].PositionZ = Z;
				TrunkVertices[v].TexCoordU = U;
				TrunkVertices[v].TexCoordV = V;

				BoundingBox.AddPoint(X, Y, Z);
			}

			fread(&IndexCount, 4, 1, file);

			// Its really the triangle count, so expand it
			IndexCount *= 3;

			unsigned short* TrunkIndices = new unsigned short[IndexCount];
			
			for(int i = 0; i < IndexCount; ++i)
			{
				unsigned short Idx = 0;
				fread(&Idx, 2, 1, file);
				TrunkIndices[i] = Idx;
			}

			RecalculateNormals<STreeTrunkVertex>(TrunkVertices, VertexCount, TrunkIndices, IndexCount);
			RecalculateTangents<STreeTrunkVertex>(TrunkVertices, VertexCount, TrunkIndices, IndexCount);

			STreeBuffer* TrunkBuffer = new STreeBuffer();		
			TrunkBuffer->Buffer = manager->GetRenderer()->CreateTrunkBuffer(TrunkVertices, VertexCount, TrunkIndices, IndexCount);
			TrunkBuffer->DiffuseMap = manager->GetRenderer()->GetTexture((TexPath + DiffuseName).c_str());
			TrunkBuffer->NormalMap = manager->GetRenderer()->GetTexture((TexPath + NormalName).c_str());
			Trunks.push_back(TrunkBuffer);

			delete[] TrunkVertices;
			delete[] TrunkIndices;
		}

		// Read Leaves
		fread(&LeafCount, 4, 1, file);

		for(int Leaf = 0; Leaf < LeafCount; ++Leaf)
		{
			DiffuseName = ReadString(file);
			NormalName = ReadString(file);

			float Scale = 0.0f;
			fread(&Scale, 4, 1, file);

			// This count is the number of leaves in the surface
			int SurfaceLeaves = 0;
			fread(&SurfaceLeaves, 4, 1, file);

			if(SurfaceLeaves == 0)
				continue;

			STreeLeafVertex* LeafVertices = new STreeLeafVertex[SurfaceLeaves * 4];
			unsigned short* LeafIndices = new unsigned short[SurfaceLeaves * 6];

			int VCnt = 0;
			int ICnt = 0;

			for(int i = 0; i < SurfaceLeaves; ++i)
			{
				float X, Y, Z;
				fread(&X, 4, 1, file);
				fread(&Y, 4, 1, file);
				fread(&Z, 4, 1, file);

				LeafVertices[VCnt + 0].Set(X, Y, Z, 0, 0);
				LeafVertices[VCnt + 1].Set(X, Y, Z, 1, 0);
				LeafVertices[VCnt + 2].Set(X, Y, Z, 0, 1);
				LeafVertices[VCnt + 3].Set(X, Y, Z, 1, 1);

				LeafIndices[ICnt + 0] = VCnt + 2;
				LeafIndices[ICnt + 1] = VCnt + 1;
				LeafIndices[ICnt + 2] = VCnt + 0;
				LeafIndices[ICnt + 3] = VCnt + 2;
				LeafIndices[ICnt + 4] = VCnt + 3;
				LeafIndices[ICnt + 5] = VCnt + 1;

				VCnt += 4;
				ICnt += 6;

				BoundingBox.AddPoint(X, Y, Z);
			}

			STreeBuffer* LeafBuffer = new STreeBuffer();
			LeafBuffer->Buffer = manager->GetRenderer()->CreateLeafBuffer(LeafVertices, SurfaceLeaves * 4, LeafIndices, SurfaceLeaves * 6);
			LeafBuffer->DiffuseMap = manager->GetRenderer()->GetTexture((TexPath + DiffuseName).c_str(), true);
			LeafBuffer->NormalMap = manager->GetRenderer()->GetTexture((TexPath + NormalName).c_str());
			LeafBuffer->Size = Scale;
			Leaves.push_back(LeafBuffer);

			delete[] LeafVertices;
			delete[] LeafIndices;
		}


		CTreeType* NewType = 0;
		if(treeType != 0)
		{
			treeType->Unload();
			NewType = treeType;
		}else
		{
			 NewType = new CTreeType(manager, TreeName);
		}

		NewType->SwayCenter = SwayCenter;
		NewType->BoundingBox = BoundingBox;
		NewType->CollisionData = CollisionData;

		for(int Trunk = 0; Trunk < Trunks.size(); ++Trunk)
			NewType->TrunkSurfaces.push_back(Trunks[Trunk]);
		for(int Leaf = 0; Leaf < Leaves.size(); ++Leaf)
			NewType->LeafSurfaces.push_back(Leaves[Leaf]);
		for(int Texture = 0; Texture < LODTextures.size(); ++Texture)
			NewType->LODTextures.push_back(LODTextures[Texture]);

		return NewType;

		

	}

	int CTreeType::GetTrunkSurfaceCount() const
	{
		return TrunkSurfaces.size();
	}

	int CTreeType::GetLeafSurfaceCount() const
	{
		return LeafSurfaces.size();
	}

	void CTreeType::BeginTrunk( int index )
	{
		if(index < 0 || index >= TrunkSurfaces.size())
			return;

		Manager->GetRenderer()->SetMeshBuffer(TrunkSurfaces[index]->Buffer);
		Manager->GetRenderer()->SetTexture("DiffuseMap", TrunkSurfaces[index]->DiffuseMap);
		Manager->GetRenderer()->SetTexture("NormalMap", TrunkSurfaces[index]->NormalMap);

		LT::TParameter<NGin::Math::Vector3> SwayParam;
		SwayParam.SetData(SwayCenter);
		Manager->GetRenderer()->SetParameter("SwayCenter", SwayParam);
	}

	void CTreeType::BeginLeaf( int index )
	{
		if(index < 0 || index >= LeafSurfaces.size())
			return;

		Manager->GetRenderer()->SetMeshBuffer(LeafSurfaces[index]->Buffer);
		Manager->GetRenderer()->SetTexture("DiffuseMap", LeafSurfaces[index]->DiffuseMap);
		Manager->GetRenderer()->SetTexture("MaskMap", LeafSurfaces[index]->DiffuseMap);
		Manager->GetRenderer()->SetTexture("NormalMap", LeafSurfaces[index]->NormalMap);

		LT::TParameter<float> ScaleParam;
		ScaleParam.SetData(LeafSurfaces[index]->Size);
		Manager->GetRenderer()->SetParameter("BillboardScale", ScaleParam);

		LT::TParameter<NGin::Math::Vector3> SwayParam;
		SwayParam.SetData(SwayCenter);
		Manager->GetRenderer()->SetParameter("SwayCenter", SwayParam);
	}

	void CTreeType::DrawTrunk( int index )
	{
		if(index < 0 || index >= TrunkSurfaces.size())
			return;

		Manager->GetRenderer()->DrawMeshBuffer(TrunkSurfaces[index]->Buffer);
	}

	void CTreeType::DrawLeaf( int index )
	{
		if(index < 0 || index >= LeafSurfaces.size())
			return;

		Manager->GetRenderer()->DrawMeshBuffer(LeafSurfaces[index]->Buffer);
	}

	void CTreeType::DrawLOD( ITreeMeshBuffer* buffer )
	{
		Manager->GetRenderer()->SetMeshBuffer(buffer);
		Manager->GetRenderer()->DrawMeshBuffer(buffer);
	}
	NGin::Math::AABB CTreeType::GetBoundingBox() const
	{
		return BoundingBox;
	}

	int CTreeType::GetLODTexturesCount() const
	{
		return LODTextures.size();
	}

	void CTreeType::BeginLOD( int index )
	{
		if(index < 0 || index >= LODTextures.size())
			return;

		Manager->GetRenderer()->SetTexture("DiffuseMap", LODTextures[index]);
	}
}
