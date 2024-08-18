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

#include "CD3D9TreeTrunkMeshBuffer.h"
#include "SD3D9TreeTrunkVertex.h"

namespace LT
{
	const D3DVERTEXELEMENT9 tSD3D9TreeTrunkVertexDeclaration[] =
	{
		{0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
		{0, 12, D3DDECLTYPE_FLOAT16_2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
		{0, 16, D3DDECLTYPE_UBYTE4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 1},
		{0, 20, D3DDECLTYPE_UBYTE4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 2},
		{0, 24, D3DDECLTYPE_UBYTE4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 3},
		D3DDECL_END()
	};

	const D3DVERTEXELEMENT9 tSTreeTrunkVertexDeclaration[] =
	{
		{0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
		{0, 12, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL, 0},
		{0, 24, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_BINORMAL, 0},
		{0, 36, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TANGENT, 0},
		{0, 48, D3DDECLTYPE_FLOAT2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
		D3DDECL_END()
	};


	CD3D9TreeTrunkMeshBuffer::CD3D9TreeTrunkMeshBuffer(IDirect3DDevice9* device)
		: VertexBuffer(0), IndexBuffer(0),
		VertexCount(0), IndexCount(0), Device(device)
	{

	}

	CD3D9TreeTrunkMeshBuffer::~CD3D9TreeTrunkMeshBuffer()
	{
		if(VertexBuffer != 0)
			VertexBuffer->Release();
		if(IndexBuffer != 0)
			IndexBuffer->Release();
	}

	bool CD3D9TreeTrunkMeshBuffer::Set() const
	{
		if(FAILED(Device->SetIndices(IndexBuffer)))
			return false;

		if(FAILED(Device->SetStreamSource(0, VertexBuffer, 0, sizeof(SD3D9TreeTrunkVertex))))
			return false;

		return true;
	}
	
	bool CD3D9TreeTrunkMeshBuffer::Draw(unsigned int &trianglesDrawn) const
	{
		if(FAILED(Device->DrawIndexedPrimitive(D3DPT_TRIANGLELIST, 0, 0, VertexCount, 0, IndexCount / 3)))
			return false;

		trianglesDrawn += IndexCount / 3;
		return true;
	}
	
	CD3D9TreeTrunkMeshBuffer* CD3D9TreeTrunkMeshBuffer::CreateD3D9TreeTrunkMeshBuffer(
		IDirect3DDevice9* device,
		const STreeTrunkVertex* vertices, unsigned int vertexCount,
		const unsigned short* indices, unsigned int indexCount)
	{
		// Create a representation mesh
		ID3DXMesh* TMesh = 0;
		D3DXCreateMesh(indexCount / 3, vertexCount, 0, tSTreeTrunkVertexDeclaration, device, &TMesh);

		STreeTrunkVertex* LockedVertices2 = 0;
		TMesh->LockVertexBuffer(0, (void**)&LockedVertices2);
		for(unsigned int i = 0; i < vertexCount; ++i)
		{
			memcpy(LockedVertices2, vertices, vertexCount * sizeof(STreeTrunkVertex));
// 			LockedVertices[i].SetPosition(D3DXVECTOR3(vertices[i].PositionX, vertices[i].PositionY, vertices[i].PositionZ));
// 			LockedVertices[i].SetTexCoord(vertices[i].TexCoordU, vertices[i].TexCoordV);
// 			LockedVertices[i].SetNormal(D3DXVECTOR3(vertices[i].NormalX, vertices[i].NormalY, vertices[i].NormalZ));
// 			LockedVertices[i].SetBiNormal(D3DXVECTOR3(vertices[i].BiNormalX, vertices[i].BiNormalY, vertices[i].BiNormalZ));
// 			LockedVertices[i].SetTangent(D3DXVECTOR3(vertices[i].TangentX, vertices[i].TangentY, vertices[i].TangentZ));	
		}
		TMesh->UnlockVertexBuffer();

		unsigned short* LockedIndices = 0;
		TMesh->LockIndexBuffer(0, (void**)&LockedIndices);
		memcpy(LockedIndices, indices, indexCount * 2);
		TMesh->UnlockIndexBuffer();

		//D3DXComputeTangentFrameEx(TMesh, needthis, 0, D3DDECLUSAGE_TANGENT, 0, D3DDECLUSAGE_BINORMAL, 0, D3DDECLUSAGE_NORMAL, 0, 
		//D3DXComputeNormals(TMesh, NULL);
		//D3DXComputeTangentFrame(TMesh, D3DXTANGENT_CALCULATE_NORMALS);

		ID3DXMesh* TMeshOld = TMesh;
		DWORD* pAdj = new DWORD[3 * TMeshOld->GetNumFaces()];
		DWORD* pAdjo = new DWORD[3 * TMeshOld->GetNumFaces()];
		TMeshOld->GenerateAdjacency(0.1f, pAdj);
		memcpy(pAdjo, pAdj, 3 * TMeshOld->GetNumFaces() * sizeof(DWORD));
 		D3DXCleanMesh(D3DXCLEAN_SIMPLIFICATION, TMeshOld, pAdj, &TMesh, pAdjo, NULL);
 
 		TMeshOld->Release();

		TMeshOld = TMesh;
 		D3DXComputeTangentFrameEx(TMeshOld, D3DDECLUSAGE_TEXCOORD, 0, 
 			D3DDECLUSAGE_TANGENT, 0, D3DDECLUSAGE_BINORMAL, 0, D3DDECLUSAGE_NORMAL, 0,
 			D3DXTANGENT_CALCULATE_NORMALS, pAdjo, 0.01f, 0.25f, 0.01f, &TMesh, NULL);
 
 		TMeshOld->Release();

		delete[] pAdj;
		delete[] pAdjo;
		
		// Setup buffers first!
		IDirect3DVertexBuffer9* LocalVertices = 0;
		IDirect3DIndexBuffer9* LocalIndices = 0;

		// Vertex Buffer
		HRESULT Hr = device->CreateVertexBuffer(
			sizeof(SD3D9TreeTrunkVertex) * TMesh->GetNumVertices(), D3DUSAGE_WRITEONLY,
			0, D3DPOOL_MANAGED, &LocalVertices, NULL);

		if(Hr != D3D_OK)
			return 0;

		// Index Buffer
		Hr = device->CreateIndexBuffer(
			sizeof(unsigned short) * TMesh->GetNumFaces() * 3, D3DUSAGE_WRITEONLY,
			D3DFMT_INDEX16, D3DPOOL_MANAGED, &LocalIndices, NULL);

		if(Hr != D3D_OK)
		{
			LocalVertices->Release();
			return 0;
		}

		// Generate vertexbuffer
		SD3D9TreeTrunkVertex* LockedVertices = 0;
		LocalVertices->Lock(0, 0, (void**)&LockedVertices, 0);
		LockedVertices2 = 0;
		TMesh->LockVertexBuffer(0, (void**)&LockedVertices2);

//		memcpy(LockedVertices, LockedVertices2, sizeof(SD3D9TreeTrunkVertex) * TMesh->GetNumVertices());
 		for(unsigned int i = 0; i < TMesh->GetNumVertices(); ++i)
 		{
 			LockedVertices[i].SetPosition(D3DXVECTOR3(LockedVertices2[i].PositionX, LockedVertices2[i].PositionY, LockedVertices2[i].PositionZ));
 			LockedVertices[i].SetTexCoord(LockedVertices2[i].TexCoordU, LockedVertices2[i].TexCoordV);
			LockedVertices[i].SetNormal(D3DXVECTOR3(LockedVertices2[i].NormalX, LockedVertices2[i].NormalY, LockedVertices2[i].NormalZ));
 			LockedVertices[i].SetBiNormal(D3DXVECTOR3(LockedVertices2[i].BiNormalX, LockedVertices2[i].BiNormalY, LockedVertices2[i].BiNormalZ));
 			LockedVertices[i].SetTangent(D3DXVECTOR3(LockedVertices2[i].TangentX, LockedVertices2[i].TangentY, LockedVertices2[i].TangentZ));	
		}

		LocalVertices->Unlock();
		TMesh->UnlockVertexBuffer();

		// Generate indexbuffer
		unsigned short* LockedIndices2 = 0;
		LocalIndices->Lock(0, 0, (void**)&LockedIndices, 0);
		TMesh->LockIndexBuffer(0, (void**)&LockedIndices2);
		
		//memcpy(LockedIndices, indices, indexCount * 2);
		memcpy(LockedIndices, LockedIndices2, sizeof(unsigned short) * TMesh->GetNumFaces() * 3);

		LocalIndices->Unlock();
		TMesh->UnlockIndexBuffer();
		

		// Create
		CD3D9TreeTrunkMeshBuffer* Buffer = new CD3D9TreeTrunkMeshBuffer(device);
		Buffer->VertexBuffer = LocalVertices;
		Buffer->IndexBuffer = LocalIndices;
		Buffer->VertexCount = TMesh->GetNumVertices();
		Buffer->IndexCount = TMesh->GetNumFaces() * 3;

		TMesh->Release();

		return Buffer;
	}

	unsigned int CD3D9TreeTrunkMeshBuffer::GetTriangleCount() const
	{
		return IndexCount / 3;
	}
}
