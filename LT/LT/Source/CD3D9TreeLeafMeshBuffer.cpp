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

#include "CD3D9TreeLeafMeshBuffer.h"
#include "SD3D9TreeLeafVertex.h"

namespace LT
{
	CD3D9TreeLeafMeshBuffer::CD3D9TreeLeafMeshBuffer(IDirect3DDevice9* device)
		: VertexBuffer(0), IndexBuffer(0),
		VertexCount(0), IndexCount(0), Device(device)
	{

	}

	CD3D9TreeLeafMeshBuffer::~CD3D9TreeLeafMeshBuffer()
	{
		if(VertexBuffer != 0)
			VertexBuffer->Release();
		if(IndexBuffer != 0)
			IndexBuffer->Release();
	}

	bool CD3D9TreeLeafMeshBuffer::Set() const
	{
		if(FAILED(Device->SetIndices(IndexBuffer)))
			return false;

		if(FAILED(Device->SetStreamSource(0, VertexBuffer, 0, sizeof(SD3D9TreeLeafVertex))))
			return false;

		return true;
	}

	bool CD3D9TreeLeafMeshBuffer::Draw(unsigned int &trianglesDrawn) const
	{
		if(FAILED(Device->DrawIndexedPrimitive(D3DPT_TRIANGLELIST, 0, 0, VertexCount, 0, IndexCount / 3)))
			return false;

		trianglesDrawn += IndexCount / 3;
		return true;
	}
	
	CD3D9TreeLeafMeshBuffer* CD3D9TreeLeafMeshBuffer::CreateD3D9TreeLeafMeshBuffer(
		IDirect3DDevice9* device,
		const STreeLeafVertex* vertices, unsigned int vertexCount,
		const unsigned short* indices, unsigned int indexCount)
	{
		// Setup buffers first!
		IDirect3DVertexBuffer9* LocalVertices = 0;
		IDirect3DIndexBuffer9* LocalIndices = 0;

		// Vertex Buffer
		HRESULT Hr = device->CreateVertexBuffer(
			sizeof(SD3D9TreeLeafVertex) * vertexCount, D3DUSAGE_WRITEONLY,
			0, D3DPOOL_MANAGED, &LocalVertices, NULL);

		if(Hr != D3D_OK)
			return 0;

		// Index Buffer
		Hr = device->CreateIndexBuffer(
			sizeof(unsigned short) * indexCount, D3DUSAGE_WRITEONLY,
			D3DFMT_INDEX16, D3DPOOL_MANAGED, &LocalIndices, NULL);

		if(Hr != D3D_OK)
		{
			LocalVertices->Release();
			return 0;
		}

		// Generate vertexbuffer
		SD3D9TreeLeafVertex* LockedVertices = 0;
		LocalVertices->Lock(0, 0, (void**)&LockedVertices, 0);

		for(unsigned int i = 0; i < vertexCount; ++i)
		{
			LockedVertices[i].SetPosition(D3DXVECTOR3(vertices[i].PositionX, vertices[i].PositionY, vertices[i].PositionZ));
			LockedVertices[i].SetTexCoord(vertices[i].TexCoordU, vertices[i].TexCoordV);
		}

		LocalVertices->Unlock();

		// Generate indexbuffer
		unsigned short* LockedIndices = 0;
		LocalIndices->Lock(0, 0, (void**)&LockedIndices, 0);

		memcpy(LockedIndices, indices, indexCount * 2);

		LocalIndices->Unlock();

		// Create
		CD3D9TreeLeafMeshBuffer* Buffer = new CD3D9TreeLeafMeshBuffer(device);
		Buffer->VertexBuffer = LocalVertices;
		Buffer->IndexBuffer = LocalIndices;
		Buffer->VertexCount = vertexCount;
		Buffer->IndexCount = indexCount;

		return Buffer;
	}

	unsigned int CD3D9TreeLeafMeshBuffer::GetTriangleCount() const
	{
		return IndexCount / 3;
	}
}
