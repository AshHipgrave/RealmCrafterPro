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

#pragma once

#include <string>
#include <vector>
#include <d3d9.h>

#include "ITreeRenderer.h"
#include "CD3D9TreeTexture.h"
#include <ArrayList.h>
#include <vector4.h>

namespace LT
{
	//! Tree Renderer Interface
	class CD3D9TreeRenderer : public ITreeRenderer
	{
	protected:

		IDirect3DDevice9* Device;

	public:
		IDirect3DVertexDeclaration9* TrunkDeclaration;
		IDirect3DVertexDeclaration9* LeafDeclaration;
		IDirect3DVertexDeclaration9* LODDeclaration;
		IDirect3DVertexDeclaration9* LastDeclaration;

	protected:
		DWORD LastFVF;
		unsigned int TrianglesDrawn;

	public:
		ID3DXEffect* TrunkEffect;
		ID3DXEffect* LeafEffect;
		ID3DXEffect* LODEffect;
		ID3DXEffect* SetEffect;

	protected:
		D3DXHANDLE TrunkLightAmbient;
		D3DXHANDLE TrunkPointColor;
		D3DXHANDLE TrunkPointPosition;
		D3DXHANDLE TrunkDirectionalNormal;
		D3DXHANDLE TrunkDirectionalColor;
		D3DXHANDLE TrunkFogColor;
		D3DXHANDLE TrunkFogData;
		D3DXHANDLE TrunkTime;
		D3DXHANDLE TrunkSwayDirection;
		D3DXHANDLE TrunkSwayAmount;

		D3DXHANDLE LeafLightAmbient;
		D3DXHANDLE LeafPointColor;
		D3DXHANDLE LeafPointPosition;
		D3DXHANDLE LeafDirectionalNormal;
		D3DXHANDLE LeafDirectionalColor;
		D3DXHANDLE LeafFogColor;
		D3DXHANDLE LeafFogData;
		D3DXHANDLE LeafTime;
		D3DXHANDLE LeafSwayDirection;
		D3DXHANDLE LeafSwayAmount;

		D3DXHANDLE LODLightAmbient;
		D3DXHANDLE LODPointColor;
		D3DXHANDLE LODPointPosition;
		D3DXHANDLE LODDirectionalNormal;
		D3DXHANDLE LODDirectionalColor;
		D3DXHANDLE LODFogColor;
		D3DXHANDLE LODFogData;
		D3DXHANDLE LODTime;
		D3DXHANDLE LODSwayDirection;
		D3DXHANDLE LODSwayAmount;

	public:
		void GetHandles();

	protected:
		std::vector<CD3D9TreeTexture*> TextureCache;

		// Lighting data
		NGin::Math::Vector3 DirectionalNormals[3];
		NGin::Math::Vector3 DirectionalColors[3];
		NGin::Math::Vector3 AmbientLight;

		// Point lights
		struct PointLight
		{
			NGin::Math::Vector3 Position;
			NGin::Math::Color Color;
			int Radius;
			bool Active;
			float Distance;
		};
		NGin::ArrayList<PointLight*> PointLights;

		// Fog data
		NGin::Math::Color FogColor;
		NGin::Math::Vector2 FogData;

		NGin::Math::Vector2 SwayDirection;
		float SwayAmount;

		int QualityLevel;

	public:

		CD3D9TreeRenderer(IDirect3DDevice9* device);
		virtual ~CD3D9TreeRenderer();

		virtual bool PreRender();
		virtual bool PostRender();
		virtual unsigned int GetTrianglesDrawn() const;
		
		
		virtual ITreeMeshBuffer* CreateTrunkBuffer(const STreeTrunkVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount);
		virtual ITreeMeshBuffer* CreateLeafBuffer(const STreeLeafVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount);
		virtual ITreeMeshBuffer* CreateLODBuffer(const STreeLODVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount);

		virtual void FreeMeshBuffer(ITreeMeshBuffer* buffer);

		virtual ITreeTexture* GetTexture(const char* path, bool ismask = false);
		virtual void FreeTexture(ITreeTexture* texture);

		virtual bool PreRenderTrunk(int rtIndex);
		virtual bool PreRenderLeaf(int rtIndex);
		virtual bool PreRenderTrunkDepth();
		virtual bool PreRenderLeafDepth();
		virtual bool PreRenderLOD(int rtIndex);

		virtual void PostRenderTrunk();
		virtual void PostRenderLeaf();
		virtual void PostRenderTrunkDepth();
		virtual void PostRenderLeafDepth();
		virtual void PostRenderLOD();

		virtual bool SetMeshBuffer(const ITreeMeshBuffer* buffer);
		virtual bool SetTexture(const char* name, const ITreeTexture* texture);
		virtual bool SetParameter(const char* name, const IParameter &parameter, bool applyToAll);
		virtual bool SetParameter(const char* name, const IParameter &parameter);
		virtual void DrawMeshBuffer(const ITreeMeshBuffer* buffer);

		virtual void SetShadowMap(void* handle);

		virtual void SetLightNormal(int index, NGin::Math::Vector3 &normal);
		virtual void SetLightColor(int index, NGin::Math::Color &color);
		virtual void SetAmbientLight(NGin::Math::Color &color);
		virtual void SetPointLights(void** lights, int count, int stride);
		virtual void GetClosestLights(NGin::Math::Vector3** colors, NGin::Math::Vector4** positions, NGin::Math::Vector3 &position);
		virtual void SetFog(NGin::Math::Color& color, NGin::Math::Vector2& data);
		virtual void SetPointLightParameters(NGin::Math::Vector3 &position);
		virtual void SetSwayDirection(NGin::Math::Vector2 &direction);
		virtual void SetSwayPower(float power);

		virtual void SetQualityLevel(int qualityLevel);

		virtual void OnDeviceLost();
		virtual void OnDeviceReset();


	};


	//! Create the D3D9 Tree Renderer
#ifdef EXPORTS
	__declspec(dllexport)
#else
	__declspec(dllimport)
#endif
		ITreeRenderer* CreateDirect3D9TreeRenderer(IDirect3DDevice9* device,
		const char* trunkEffect, const char* leafEffect, const char* lodEffect);
}


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
