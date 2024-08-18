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

#include "IGUIRenderer.h"
#include "CD3D9GUITexture.h"
#include <d3d9.h>
#include <vector>
#include <stack>

namespace NGin
{
	namespace GUI
	{
		//! GUI Renderer Implementation
		class CD3D9GUIRenderer : public IGUIRenderer
		{
		protected:

			IDirect3DDevice9* Device;
			IDirect3DVertexDeclaration9* GUIDeclaration;
			IDirect3DVertexDeclaration9* LastDeclaration;

			ID3DXEffect* GUIEffect;

			DWORD LastFVF, LastScissorTestState, LastAntiAlias;
			unsigned int TrianglesDrawn;

			std::vector<CD3D9GUITexture*> TextureCache;
			std::stack<Math::Vector4> ScissorRegions;

		public:

			CD3D9GUIRenderer(IDirect3DDevice9* device, ID3DXEffect* guiEffect,
				IDirect3DVertexDeclaration9* guiDeclaration);
			virtual ~CD3D9GUIRenderer();

			virtual bool PreRender();
			virtual bool PostRender();
			virtual void SetManager(IGUIManager* manager);
			virtual unsigned int GetTrianglesDrawn() const;
			
			virtual IGUIMeshBuffer* CreateMeshBuffer(const SGUIVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount);
			virtual void FreeMeshBuffer(IGUIMeshBuffer* buffer);

			virtual IGUITexture* CreateFontTexture(int width, int height, const char* data);
			virtual IGUITexture* GetTexture(IGUITexture* handle);
			virtual IGUITexture* GetTexture(const char* path, unsigned int mask = 0, bool ismask = false);
			virtual IGUITexture* CreateTextureFromBase(void* base, Math::Vector2 &size);
			virtual bool SetTexture(const char* name, const IGUITexture* texture);
			virtual void FreeTexture(IGUITexture* texture);

			virtual bool SetParameter(const char* name, const IParameter &parameter);

			virtual bool SetMeshBuffer(const IGUIMeshBuffer* buffer);
			virtual void DrawMeshBuffer(const IGUIMeshBuffer* buffer);
			virtual void DrawMeshBuffer(const IGUIMeshBuffer* buffer, int triangleOffset, int triangleCount);

			virtual Math::Vector4 GetScissorRect();
			virtual void PushScissorRect(Math::Vector4 &rect);
			virtual void PopScissorRect();

			virtual void OnDeviceLost();
			virtual void OnDeviceReset();

			virtual void DebugOut(const char* message);
		};
	}
}

//! Create the D3D9 GUI Renderer
#ifdef EXPORTS
__declspec(dllexport)
#else
__declspec(dllimport)
#endif
NGin::GUI::IGUIRenderer* CreateDirect3D9GUIRenderer(void* device,
	const char* effectPath);


/*
C/ITreeManager
    - Render(ViewProj)
    - Update(Pos)
    - LoadTreeType(path)
    - CreateTree(type)
    - LoadZone(path)
    - SaveZone(path)



C/ITreeInstance
    - m_BaseType
    - Get/Set Position
    - Get/Set Scale
    - Get/Set Rotation

CTreeType
    - PreRenderSurface(index)
    - RenderSurface(index)
    - PreRenderBillboards(index)
    - RenderBillboards(index)
    - PreRenderLOD(index)
    - RenderLOD(index)


CTreeZoneSector // Zone area chunk
    - array<array<ITreeInstance>> Instances

STreeTrunkVertex
    - X, Y, Z, U, V, NBTX, NBTY, NBTZ

STreeLeafVertex
    - X, Y, Z, U, V

STreeLODVertex
    - X, Y, Z, U, V

ITreeRenderer
    - PreRender()
    - PostRender()
    - CreateTrunkBuffer(STreeTrunkVertex* Vertices, uint VertexCount, short* Indices, uint IndexCount);
    - CreateLeafBuffer(STreeLeafVertex* Vertices, uint VertexCount, short* Indices, uint IndexCount);
    - CreateLODBuffer(STreeLODVertex* Vertices, uint VertexCount, short* Indices, uint IndexCount);
    - FreeMeshBuffer()
    - GetTexture()
    - FreeTexture()
    - PreRenderTrunk()
    - PostRenderTrunk()
    - PreRenderLeaf()
    - PostRenderLeaf()
    - PreRenderLOD()
    - PostRenderLOD()
    - SetMeshBuffer()
    - SetTexture()
    - SetParameter(string name, value)
    - DrawMeshBuffer()

I/CMeshBuffer
   - OnDeviceLost/Reset

I/CTexture
   - OnDeviceLost/Reset
    
*/
